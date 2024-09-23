// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaDownload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WhatsApp.Resolvers;
using WhatsApp.WaCollections;
using WhatsAppNative;
using Windows.Storage;


namespace WhatsApp
{
  public static class MediaDownload
  {
    private static string LogHeader = nameof (MediaDownload);
    public static Subject<Message> VoiceMessageDownloadEndedSubj = new Subject<Message>();
    private const string PttExtension = ".waptt";
    private static Dictionary<string, bool> dirChecked_ = (Dictionary<string, bool>) null;
    private static double MAX_DOWNLOAD_PROPORTION = 0.8;
    private static string LastMediaPrefix = (string) null;
    private static int LastMediaInteger = 0;

    public static void AckMedia(string url, bool webrequest)
    {
      AppState.SchedulePersistentAction(PersistentAction.AckMedia(url, webrequest));
    }

    public static void InspectCodec(ref bool dirty, Message m)
    {
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          MediaType mediaType = CodecDetector.DetectMp4Codecs(m.LocalFileUri);
          if (mediaType.VideoStreamType == Mp4VideoStreamType.NotFound || mediaType.AudioStreamType != Mp4AudioStreamType.AmrWb)
            break;
          Log.l("codec info", "Download transcode will be needed");
          MessageMiscInfo misc = m.GetMiscInfo();
          if (misc == null)
          {
            misc = new MessageMiscInfo();
            m.SetMiscInfo(misc);
          }
          misc.TranscoderData = new TranscodeArgs()
          {
            Flags = TranscodeReason.TranscodeAudio
          }.Serialize();
          m.Status = FunXMPP.FMessage.Status.Pending;
          dirty = true;
          WAThreadPool.QueueUserWorkItem((Action) (() => AppState.ProcessPendingMessage(m)));
          break;
      }
    }

    private static void RescanCachedModel(ref Message m)
    {
      if (m != null)
      {
        Message m2 = m;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => MediaDownload.RescanCachedModel(db, ref m2)));
        m = m2;
      }
      else
        Log.l(MediaDownload.LogHeader, "Rescan initiated with null message");
    }

    private static void RescanCachedModel(MessagesContext db, ref Message m)
    {
      if (m != null)
      {
        string keyRemoteJid = m.KeyRemoteJid;
        bool keyFromMe = m.KeyFromMe;
        string keyId = m.KeyId;
        string remoteResource = m.RemoteResource;
        m = db.GetMessageById(m.MessageID);
        if (m == null)
          Log.l(MediaDownload.LogHeader, "Rescan found message deleted");
        else if (m.KeyRemoteJid != keyRemoteJid || m.KeyFromMe != keyFromMe || m.KeyId != keyId || m.RemoteResource != remoteResource)
        {
          Log.l(MediaDownload.LogHeader, "Rescan found message reused");
          m = (Message) null;
        }
        else
        {
          if (m.MediaWaType != FunXMPP.FMessage.Type.Revoked)
            return;
          Log.l(MediaDownload.LogHeader, "Rescan found message revoked");
          m = (Message) null;
        }
      }
      else
        Log.l(MediaDownload.LogHeader, "Rescan requested with null message");
    }

    private static void ProcessMediaDownloadWithoutDbLock(
      Message m,
      MediaDownload.MediaProgress ev,
      List<string> oldFiles,
      WhatsApp.Events.MediaDownload fsEvent,
      Action<Exception> onError,
      Action @finally,
      List<Action<Message>> modifications,
      ref Action dbPart,
      ref string personalRef,
      ref string fileName,
      ref byte[] cipherMediaHash)
    {
      try
      {
        using (AxolotlMediaCipher downloadCipher = AxolotlMediaCipher.CreateDownloadCipher(m))
        {
          personalRef = downloadCipher.GenerateMediaRef(Settings.MyJid);
          if (!ev.isDuplicated())
          {
            string extensionForMessage = downloadCipher.GetFileExtensionForMessage(m);
            long ticks = DateTime.UtcNow.Ticks;
            string filename = fileName + "_plaintext_" + (object) ticks + "." + extensionForMessage;
            Log.l("e2e", "Decrypting [{0}] -> [{1}]", (object) fileName, (object) filename);
            using (IMediaStorage mediaStorage = MediaStorage.Create(fileName))
            {
              Stream src;
              try
              {
                src = mediaStorage.OpenFile(fileName);
              }
              catch (Exception ex)
              {
                Action snap = dbPart;
                dbPart = (Action) (() =>
                {
                  MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                  {
                    MediaDownload.RescanCachedModel(db, ref m);
                    if (m == null || m.LocalFileUri != null)
                      return;
                    Message duplicateImpl = MediaDownload.FindDuplicateImpl(m);
                    if (duplicateImpl != null)
                    {
                      m.LocalFileUri = duplicateImpl.LocalFileUri;
                      db.LocalFileAddRef(m.LocalFileUri, m.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
                      db.SubmitChanges();
                    }
                    else
                      Log.SendCrashLog(ex, MediaDownload.LogHeader);
                  }));
                  Action action = snap;
                  if (action == null)
                    return;
                  action();
                });
                return;
              }
              using (src)
              {
                using (Stream dest = mediaStorage.OpenFile(filename, FileMode.Create, FileAccess.ReadWrite))
                  downloadCipher.DecryptMedia(m, src, dest, fsEvent);
              }
            }
            oldFiles.Add(fileName);
            fileName = filename;
            NativeInterfaces.Mp4Utils.CheckAndRepair(m, ref fileName, fileName + "_repaired_" + (object) ticks + "." + extensionForMessage, true, oldFiles, modifications);
            cipherMediaHash = downloadCipher.CipherMediaHash;
          }
          else
          {
            cipherMediaHash = ev.DuplicatedMediaHash;
            fileName = ev.FileName;
          }
        }
      }
      catch (Exception ex)
      {
        if (ex is CryptographicException || ex is Axolotl.AxolotlMediaDecryptException || ex is CheckAndRepairException)
        {
          Log.SendCrashLog(ex, "decrypt exception", logOnlyForRelease: true);
          Action snap = dbPart;
          dbPart = (Action) (() =>
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              uint num3 = (uint) ((int) m.InternalProperties?.MediaPropertiesField?.DecryptRetryCount ?? 0);
              MessageProperties messageProperties = m.InternalProperties ?? new MessageProperties();
              uint num4;
              (messageProperties.MediaPropertiesField ?? (messageProperties.MediaPropertiesField = new MessageProperties.MediaProperties())).DecryptRetryCount = new uint?(num4 = num3 + 1U);
              m.InternalProperties = messageProperties;
              if (num4 > 5U)
                m.Status = FunXMPP.FMessage.Status.Error;
              db.SubmitChanges();
            }));
            Action<Exception> action = onError;
            if (action != null)
              action(ex);
            snap();
          });
        }
        else
        {
          dbPart = (Action) null;
          Action<Exception> action = onError;
          if (action != null)
            action(ex);
          Log.SendCrashLog(ex, MediaDownload.LogHeader);
        }
        @finally();
      }
    }

    public static void StoreMediaPathToDb(
      Message m,
      string fileName,
      List<Action<Message>> modifications,
      List<string> oldFiles,
      MediaDownload.MediaProgress ev,
      byte[] cipherMediaHash,
      string personalRef,
      ref bool callback)
    {
      bool localCallback = callback;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (m == null)
          return;
        bool flag = m.ShouldSaveMedia(db);
        MediaDownload.RescanCachedModel(db, ref m);
        bool dirty = false;
        string str1 = (string) null;
        MessageMiscInfo messageMiscInfo = (MessageMiscInfo) null;
        string str2 = (string) null;
        if (m != null && m.Status != FunXMPP.FMessage.Status.Error && m.LocalFileUri == null)
        {
          Log.d(MediaDownload.LogHeader, "completed | key id:{0},filepath:{1}", (object) m.KeyId, (object) fileName);
          str2 = fileName;
          str1 = Mms4Helper.IsMms4DownloadMessage(m) ? (string) null : m.MediaUrl;
          if (cipherMediaHash != null)
          {
            MessageProperties forMessage1 = MessageProperties.GetForMessage(m);
            forMessage1.EnsureCommonProperties.CipherMediaHash = cipherMediaHash;
            forMessage1.Save();
            MediaDownload.MediaProgress mediaProgress = ev;
            if ((mediaProgress != null ? (mediaProgress.isDuplicated() ? 1 : 0) : 0) != 0)
            {
              m.MediaUrl = ev.DuplicatedMediaUrl;
              m.MediaKey = ev.DuplicatedMediaKey;
              MessageProperties forMessage2 = MessageProperties.GetForMessage(m);
              forMessage2.EnsureMediaProperties.MediaDirectPath = ev.DuplicatedDirectPath;
              forMessage2.Save();
            }
          }
          m.TransferValue = 1.0;
          dirty = true;
          localCallback = true;
        }
        if (m != null && messageMiscInfo == null)
          messageMiscInfo = m.GetMiscInfo((SqliteMessagesContext) db);
        if (messageMiscInfo != null && messageMiscInfo.BackgroundId != null)
        {
          messageMiscInfo.BackgroundId = (string) null;
          dirty = true;
        }
        if (str2 != null)
        {
          try
          {
            string str3 = (string) null;
            if (flag && m.CanSaveMedia(false))
            {
              string uri = str2;
              int mediaWaType = (int) m.MediaWaType;
              MessagesContext db1 = db;
              int num1 = m.IsPtt() ? 1 : 0;
              MediaDownload.MediaProgress mediaProgress = ev;
              int num2 = mediaProgress != null ? (mediaProgress.isDuplicated() ? 1 : 0) : 0;
              str3 = MediaDownload.SaveMedia(uri, (FunXMPP.FMessage.Type) mediaWaType, db: db1, isPtt: num1 != 0, duplicatingFile: num2 != 0);
            }
            if (str3 == null)
            {
              MediaDownload.MediaProgress mediaProgress = ev;
              if ((mediaProgress != null ? (mediaProgress.isDuplicated() ? 1 : 0) : 0) == 0)
              {
                string directoryPath = MediaDownload.GetDirectoryPath();
                string dirPath = Constants.IsoStorePath + "\\" + directoryPath.Replace('\\', '/');
                int num = str2.LastIndexOfAny(new char[2]
                {
                  '\\',
                  '/'
                });
                string suggestedName = num > 0 ? str2.Substring(num + 1) : str2;
                string srcName = dirPath + "\\" + suggestedName;
                using (NativeMediaStorage fs = new NativeMediaStorage())
                  str3 = directoryPath + "/" + MediaDownload.GenerateFilename(dirPath, suggestedName, m.MediaWaType, (MessagesContext) null, m.IsPtt(), tryReserveFile: (Action<string>) (attemptedName => fs.MoveFile(srcName, MediaStorage.GetAbsolutePath(attemptedName))));
              }
            }
            if (str3 != null)
            {
              string str4 = str2;
              m.LocalFileUri = str3;
              Log.l(MediaDownload.LogHeader, "{0} | saved | saved filepath:{1},old filepath:{2}", (object) m.LogInfo(), (object) str3, (object) str4);
              MediaDownload.MediaProgress mediaProgress = ev;
              if ((mediaProgress != null ? (mediaProgress.isDuplicated() ? 1 : 0) : 0) == 0)
                oldFiles.Add(str4);
            }
            else
              m.LocalFileUri = str2;
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "save to phone album");
            m.LocalFileUri = str2;
          }
          modifications.ForEach((Action<Action<Message>>) (f => f(m)));
          db.LocalFileAddRef(m.LocalFileUri, m.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
          dirty = true;
        }
        try
        {
          Message message = m;
          if ((message != null ? (message.Status != FunXMPP.FMessage.Status.Error ? 1 : 0) : 1) != 0)
            MediaDownload.InspectCodec(ref dirty, m);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "codec inspection");
        }
        if (dirty)
          db.SubmitChanges();
        if (string.IsNullOrEmpty(str1))
          return;
        MediaDownload.AckMedia(str1 + (personalRef != null ? "?ack=" + personalRef : ""), personalRef != null);
      }));
      oldFiles.ForEach(new Action<string>(MediaDownload.ClearOldFile));
      callback = localCallback;
    }

    public static IObservable<Unit> TransferForMessageObservable(
      Message m,
      IObservable<MediaDownload.MediaProgress> source,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      Mms4Helper.MaybeWarmupMms4Host(m);
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer => MediaDownload.TransferForMessage(m, source, fsEvent, (Action) (() => observer.OnNext(new Unit())), (Action<Exception>) (ex => observer.OnError(ex)), (Action) (() => observer.OnCompleted()))));
    }

    private static IDisposable TransferForMessage(
      Message m,
      IObservable<MediaDownload.MediaProgress> source,
      WhatsApp.Events.MediaDownload fsEvent,
      Action onSuccess = null,
      Action<Exception> onError = null,
      Action onFinal = null)
    {
      MediaDownload.RescanCachedModel(ref m);
      if (m == null || source == null || m.TransferInProgress || !string.IsNullOrEmpty(m.LocalFileUri) || m.MediaKey == null)
        return (IDisposable) new DisposableAction((Action) null);
      IDisposable sub = (IDisposable) null;
      Action release = Utils.IgnoreMultipleInvokes((Action) (() =>
      {
        sub.SafeDispose();
        sub = (IDisposable) null;
        Action action = onFinal;
        if (action == null)
          return;
        action();
      }));
      bool async = false;
      sub = source.Subscribe<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (ev =>
      {
        if (ev.TransferProgress.HasValue)
        {
          double num1 = Math.Max(m.TransferValue, ev.TransferProgress.Value);
          if (ev.TransferProgress.Value < m.TransferValue && ev.TransferProgress.Value > 0.0)
          {
            Log.d(MediaDownload.LogHeader, string.Format("resetting transfer value, was {0} now {1} message_id={2}", (object) m.TransferValue, (object) ev.TransferProgress.Value, (object) m.MessageID));
            num1 = ev.TransferProgress.Value;
          }
          double transferValue = m.TransferValue;
          m.TransferValue = num1;
          int num2 = (int) (num1 * 100.0);
          if (num2 > (int) (transferValue * 100.0 + 4.0) || num2 == 100 && transferValue < 1.0)
            Log.l(MediaDownload.LogHeader, string.Format("download_progress={0}%, message_id={1}", (object) num2, (object) m.MessageID));
        }
        if (ev.FileName != null)
        {
          Action @finally = release;
          bool callback = false;
          string personalRef = (string) null;
          byte[] cipherMediaHash = (byte[]) null;
          string fileName = ev.FileName;
          List<string> oldFiles = new List<string>();
          List<Action<Message>> modifications = new List<Action<Message>>();
          MediaDownload.RescanCachedModel(ref m);
          if (m != null && m.LocalFileUri != null)
          {
            Log.l(MediaDownload.LogHeader, "local file already set");
            m = (Message) null;
          }
          if (m != null && m.IsPrefetchingVideo)
          {
            Log.d(MediaDownload.LogHeader, string.Format("Prefetched video - leaving encrypted file: {0}", (object) fileName));
            m.IsAutoDownloading = false;
            Assert.IsFalse(m.IsPrefetchingVideo);
          }
          else
          {
            bool exception = false;
            Action dbPart = (Action) (() =>
            {
              try
              {
                if (exception || m == null)
                  return;
                MediaDownload.StoreMediaPathToDb(m, fileName, modifications, oldFiles, ev, cipherMediaHash, personalRef, ref callback);
                if (callback)
                {
                  if (onSuccess != null)
                    onSuccess();
                }
              }
              finally
              {
                release();
              }
              fsEvent.SaveEvent();
            });
            ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
            {
              if (m == null)
                return;
              Log.l(MediaDownload.LogHeader, "starting preamble {0}", (object) m.MessageID);
              MediaDownload.ProcessMediaDownloadWithoutDbLock(m, ev, oldFiles, fsEvent, (Action<Exception>) (ex =>
              {
                exception = true;
                Action<Exception> action = onError;
                if (action == null)
                  return;
                action(ex);
              }), @finally, modifications, ref dbPart, ref personalRef, ref fileName, ref cipherMediaHash);
              if (dbPart == null)
                return;
              WAThreadPool.QueueUserWorkItem(dbPart);
            }));
            async = true;
          }
        }
        else
        {
          if (ev.BackgroundId == null)
            return;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            bool flag = false;
            MessageMiscInfo miscInfo = m.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateIfNotFound);
            if (miscInfo.BackgroundId != ev.BackgroundId)
            {
              miscInfo.BackgroundId = ev.BackgroundId;
              flag = true;
            }
            if (db.SaveMessageMiscInfoOnSubmit(miscInfo))
              flag = true;
            if (!flag)
              return;
            db.SubmitChanges();
          }));
        }
      }), (Action<Exception>) (ex =>
      {
        Log.LogException(ex, "download");
        Action<Exception> action = onError;
        if (action != null)
          action(ex);
        release();
      }), (Action) (() =>
      {
        if (async)
          return;
        release();
      }));
      return (IDisposable) new DisposableAction(release);
    }

    private static void ClearOldFile(string oldFilepath)
    {
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          try
          {
            string destinationFileName = oldFilepath + "_tmp";
            storeForApplication.MoveFile(oldFilepath, destinationFileName);
            oldFilepath = destinationFileName;
          }
          catch (Exception ex)
          {
            Log.l(MediaDownload.LogHeader, "failed to move file! {0} {1}", (object) oldFilepath, (object) ex.GetFriendlyMessage());
          }
          storeForApplication.DeleteFile(oldFilepath);
        }
      }
      catch (Exception ex)
      {
        Log.l(MediaDownload.LogHeader, "failed to delete file {0} {1}", (object) oldFilepath, (object) ex.GetFriendlyMessage());
      }
    }

    public static void CancelMessageDownload(Message msg)
    {
      msg.CancelPendingMedia();
      MessageMiscInfo misc = msg.GetMiscInfo();
      if (misc == null)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => misc = msg.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateIfNotFound)));
      if (misc.BackgroundId == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        try
        {
          BackgroundTransferRequest request = BackgroundTransferService.Find(misc.BackgroundId);
          if (request != null)
            BackgroundTransferService.Remove(request);
        }
        catch (Exception ex)
        {
        }
        misc.BackgroundId = (string) null;
        db.SubmitChanges();
      }));
    }

    public static bool GetFileNames(Message m, out string btsPath, out string inAppPath)
    {
      btsPath = (string) null;
      inAppPath = (string) null;
      string str1 = m?.MediaUrl;
      if (Mms4Helper.IsMms4DownloadMessage(m))
      {
        byte[] cipherMediaHash = m.GetCipherMediaHash();
        if (cipherMediaHash != null)
          str1 = "mms4/" + Mms4Helper.ConvertBytesToUrlParm(cipherMediaHash);
      }
      if (str1 == null)
        return false;
      string str2 = str1.Substring(str1.LastIndexOf("/") + 1);
      int length = str2.LastIndexOf("?");
      if (length > 1)
        str2 = str2.Substring(0, length);
      string str3 = str2;
      string directoryPath = MediaDownload.GetDirectoryPath();
      btsPath = string.Format("{0}/{1}", (object) directoryPath, (object) str2);
      inAppPath = string.Format("{0}/{1}", (object) directoryPath, (object) str3);
      return true;
    }

    public static string GetDirectoryPath()
    {
      DateTime utcNow = DateTime.UtcNow;
      string dirPath = string.Format("shared/transfers/{0}_{1}", (object) utcNow.Year, (object) (utcNow.DayOfYear / 7));
      if (!MediaDownload.CheckDirectoryCreated(dirPath))
        MediaDownload.CreateDirectories(new string[3]
        {
          "shared",
          "shared/transfers",
          dirPath
        });
      return dirPath;
    }

    public static void CreateIsoStoreDirectory(string subdir)
    {
      if (string.IsNullOrEmpty(subdir) || MediaDownload.CheckDirectoryCreated(subdir))
        return;
      string[] strArray = subdir.Split(new char[1]{ '\\' }, StringSplitOptions.RemoveEmptyEntries);
      string[] dirPaths = new string[strArray.Length];
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append('/');
        stringBuilder.Append(strArray[index]);
        dirPaths[index] = stringBuilder.ToString();
      }
      MediaDownload.CreateDirectories(dirPaths);
    }

    private static bool CheckDirectoryCreated(string dirPath)
    {
      if (MediaDownload.dirChecked_ == null)
        MediaDownload.dirChecked_ = new Dictionary<string, bool>();
      bool flag = false;
      return MediaDownload.dirChecked_.TryGetValue(dirPath, out flag) & flag;
    }

    private static void CreateDirectories(string[] dirPaths)
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        foreach (string dirPath in dirPaths)
        {
          if (!storeForApplication.DirectoryExists(dirPath))
            storeForApplication.CreateDirectory(dirPath);
          MediaDownload.dirChecked_[dirPath] = true;
        }
      }
    }

    public static IObservable<MediaDownload.MediaProgress> TransferFromBackground(
      Message m,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      IObservable<MediaDownload.MediaProgress> duplicate = MediaDownload.FindDuplicate(m);
      if (duplicate != null)
      {
        fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.DEDUPED);
        return duplicate;
      }
      string inAppPath;
      return !MediaDownload.GetFileNames(m, out string _, out inAppPath) ? Observable.Never<MediaDownload.MediaProgress>() : MediaDownload.InAppDownloadMaybeMms4(inAppPath, true, m, fsEvent);
    }

    public static IObservable<MediaDownload.MediaProgress> TransferFromForeground(
      Message m,
      WhatsApp.Events.MediaDownload fsEvent,
      bool interactive)
    {
      IObservable<MediaDownload.MediaProgress> duplicate = MediaDownload.FindDuplicate(m);
      if (duplicate != null)
      {
        fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.DEDUPED);
        return duplicate;
      }
      string btsFile;
      string inAppFile;
      if (!MediaDownload.GetFileNames(m, out btsFile, out inAppFile))
        return Observable.Never<MediaDownload.MediaProgress>();
      long mediaSize = m.MediaSize;
      Log.d(MediaDownload.LogHeader, "Media size: {0}", (object) mediaSize);
      if (interactive && NetworkStateMonitor.Is2GConnection() || mediaSize < 1048576L)
        return MediaDownload.InAppDownloadMaybeMms4(inAppFile, true, m, fsEvent);
      if (m.IsPrefetchingVideo)
      {
        Assert.IsTrue(Settings.VideoPrefetchBytes < 1048576, string.Format("Cannot prefetch more than {0} bytes of a video while using InAppDownload", (object) 1048576L));
        return MediaDownload.InAppDownloadMaybeMms4(inAppFile, true, m, fsEvent);
      }
      return !interactive ? MediaDownload.BackgroundTransferMaybeMms4(m, btsFile) : Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
      {
        IDisposable btsSub = (IDisposable) null;
        IDisposable inAppSub = (IDisposable) null;
        object releaseLock = new object();
        Action release = Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          btsSub.SafeDispose();
          btsSub = (IDisposable) null;
          inAppSub.SafeDispose();
          inAppSub = (IDisposable) null;
          observer.OnCompleted();
        }));
        btsSub = MediaDownload.BackgroundTransferMaybeMms4(m, btsFile).Subscribe<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (ev =>
        {
          if (ev.BtsFailedToStart || ev.Status.HasValue && MediaDownload.BtsStatusIsFlakey(ev.Status.Value))
          {
            inAppSub = MediaDownload.InAppDownloadMaybeMms4(inAppFile, true, m, fsEvent).Subscribe<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (ev2 =>
            {
              observer.OnNext(ev2);
              if (ev2.FileName == null)
                return;
              release();
            }), (Action<Exception>) (err => observer.OnError(err)), (Action) (() => MediaDownload.SafeRelease(releaseLock, ref inAppSub, ref btsSub, release)));
          }
          else
          {
            observer.OnNext(ev);
            if (ev.FileName == null)
              return;
            release();
          }
        }), (Action<Exception>) (err => observer.OnError(err)), (Action) (() => MediaDownload.SafeRelease(releaseLock, ref btsSub, ref inAppSub, release)));
        return release;
      }));
    }

    public static Message FindDuplicateImpl(Message m)
    {
      Message duplicateMessage = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message[] messagesWithMediaHash = db.GetMessagesWithMediaHash(m.MediaHash, m.MediaWaType);
        duplicateMessage = messagesWithMediaHash.GetMediaCipherDuplicate(m, db);
        Log.l(MediaDownload.LogHeader, "duplicate check | simliar:{0},dup msg id:{1}", (object) ((IEnumerable<Message>) messagesWithMediaHash).Count<Message>(), duplicateMessage == null ? (object) "n/a" : (object) duplicateMessage.KeyId);
      }));
      return duplicateMessage;
    }

    public static Sticker FindDuplicateSticker(Message m)
    {
      Sticker duplicate = (Sticker) null;
      if (m.MediaWaType == FunXMPP.FMessage.Type.Sticker)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          duplicate = m.GetExistingSticker(db);
          if (duplicate == null)
            return;
          if (duplicate.EncodedFileHash == null || string.IsNullOrEmpty(duplicate.LocalFileUri))
          {
            duplicate = (Sticker) null;
          }
          else
          {
            using (IMediaStorage mediaStorage = MediaStorage.Create(duplicate.LocalFileUri))
            {
              if (mediaStorage.FileExists(duplicate.LocalFileUri))
                return;
              duplicate = (Sticker) null;
            }
          }
        }));
      return duplicate;
    }

    private static IObservable<MediaDownload.MediaProgress> FindDuplicate(Message m)
    {
      Message duplicateMessage = MediaDownload.FindDuplicateImpl(m);
      if (duplicateMessage == null)
      {
        Sticker duplicateSticker = MediaDownload.FindDuplicateSticker(m);
        if (duplicateSticker == null)
          return (IObservable<MediaDownload.MediaProgress>) null;
        Log.l(MediaDownload.LogHeader, "duplicated sticker selected | file={0}", (object) duplicateSticker.LocalFileUri);
        Sticker duplicatedSticker = duplicateSticker;
        return Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
        {
          try
          {
            bool flag = false;
            using (IMediaStorage mediaStorage = MediaStorage.Create(duplicatedSticker.LocalFileUri))
              flag = mediaStorage.FileExists(duplicatedSticker.LocalFileUri);
            if (duplicatedSticker.FileLength != m.MediaSize)
              observer.OnError((Exception) new IOException("duplicate sticker changed?!"));
            else if (!flag)
            {
              observer.OnError((Exception) new IOException("duplicate sticker local file removed"));
            }
            else
            {
              MediaDownload.ReportProgress(observer, m.MediaSize, m.MediaSize);
              observer.OnNext(new MediaDownload.MediaProgress()
              {
                FileName = duplicatedSticker.LocalFileUri,
                DuplicatedMediaKey = duplicatedSticker.MediaKey,
                DuplicatedMediaHash = duplicatedSticker.EncodedFileHash,
                DuplicatedMediaUrl = duplicatedSticker.Url,
                DuplicatedDirectPath = duplicateMessage.InternalProperties?.MediaPropertiesField?.MediaDirectPath
              });
            }
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "duplicate processing exception");
            observer.OnError(ex);
          }
          return (Action) (() => observer.OnCompleted());
        }));
      }
      Log.l(MediaDownload.LogHeader, "duplicated selected | messageKeyId={0} duplicate={1} file={2}", (object) m.KeyId, (object) duplicateMessage.KeyId, (object) duplicateMessage.LocalFileUri);
      Message duplicatedMessage = duplicateMessage;
      byte[] duplicatedCipherHash = duplicateMessage.GetCipherMediaHash();
      return Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
      {
        try
        {
          Log.l(MediaDownload.LogHeader, "duplicated | messageKeyId={0} duplicate={1}", (object) m.KeyId, (object) duplicatedMessage.KeyId);
          if (duplicatedMessage.MediaSize != m.MediaSize)
            observer.OnError((Exception) new IOException("duplicate message changed?!"));
          else if (!duplicatedMessage.LocalFileExists())
          {
            observer.OnError((Exception) new IOException("duplicate message local file removed"));
          }
          else
          {
            MediaDownload.ReportProgress(observer, m.MediaSize, m.MediaSize);
            observer.OnNext(new MediaDownload.MediaProgress()
            {
              FileName = duplicatedMessage.LocalFileUri,
              DuplicatedMediaKey = duplicatedMessage.MediaKey,
              DuplicatedMediaHash = duplicatedCipherHash,
              DuplicatedMediaUrl = duplicatedMessage.MediaUrl,
              DuplicatedDirectPath = duplicatedMessage.InternalProperties?.MediaPropertiesField?.MediaDirectPath
            });
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "duplicate processing exception");
          observer.OnError(ex);
        }
        return (Action) (() => observer.OnCompleted());
      }));
    }

    private static void SafeRelease(
      object releaseLock,
      ref IDisposable mySub,
      ref IDisposable otherSub,
      Action release)
    {
      bool flag = false;
      lock (releaseLock)
      {
        mySub.SafeDispose();
        mySub = (IDisposable) null;
        flag = otherSub == null;
      }
      if (!flag)
        return;
      release();
    }

    private static void ReportProgress(
      IObserver<MediaDownload.MediaProgress> observer,
      long current,
      long total)
    {
      if (total <= 0L)
        return;
      observer.OnNext(new MediaDownload.MediaProgress()
      {
        TransferProgress = new double?((double) current * MediaDownload.MAX_DOWNLOAD_PROPORTION / (double) total)
      });
    }

    private static void ReportProgress(
      IObserver<MediaDownload.MediaProgress> observer,
      BackgroundTransferRequest req)
    {
      MediaDownload.ReportProgress(observer, req.BytesReceived + req.BytesSent, req.TotalBytesToReceive + req.TotalBytesToSend);
    }

    public static string RedactUrl(string url) => url;

    private static IObservable<MediaDownload.MediaProgress> BackgroundTransferMaybeMms4(
      Message msg,
      string filename)
    {
      if (!Mms4Helper.IsMms4DownloadMessage(msg))
        return MediaDownload.BackgroundTransfer(msg.MediaUrl, filename);
      FunXMPP.FMessage.FunMediaType funType = msg.GetFunMediaType();
      byte[] cipherHash = msg.GetCipherMediaHash();
      return Mms4HostSelector.GetInstance().GetSelectedHostObservable(true, msg.GetFunMediaType(), true).SelectMany<Mms4HostSelector.Mms4HostSelection, MediaDownload.MediaProgress, MediaDownload.MediaProgress>((Func<Mms4HostSelector.Mms4HostSelection, IObservable<MediaDownload.MediaProgress>>) (hostSelection => MediaDownload.BackgroundTransfer(Mms4Helper.CreateUrlStringForDownload(hostSelection.HostName, funType, cipherHash, false, msg.GetDirectPath()), filename)), (Func<Mms4HostSelector.Mms4HostSelection, MediaDownload.MediaProgress, MediaDownload.MediaProgress>) ((hostSelection, obs) => obs));
    }

    private static IObservable<MediaDownload.MediaProgress> BackgroundTransfer(
      string url,
      string filename)
    {
      return Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
      {
        Log.l(MediaDownload.LogHeader, "Using BTS to download {0}", (object) MediaDownload.RedactUrl(url));
        BackgroundTransferRequest req = new BackgroundTransferRequest(new Uri(url));
        req.Method = "GET";
        req.DownloadLocation = new Uri(filename, UriKind.RelativeOrAbsolute);
        req.Headers["User-Agent"] = AppState.GetUserAgent();
        req.TransferPreferences = TransferPreferences.AllowCellularAndBattery;
        IDisposable transferSub = (IDisposable) null;
        Action release = Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          if (transferSub != null)
          {
            transferSub.Dispose();
            transferSub = (IDisposable) null;
          }
          observer.OnCompleted();
        }));
        observer.OnNext(new MediaDownload.MediaProgress()
        {
          Status = new TransferStatus?(req.TransferStatus)
        });
        MediaDownload.ReportProgress(observer, req);
        transferSub = req.GetTransferProgressChangedAsync().Do<IEvent<BackgroundTransferEventArgs>>((Action<IEvent<BackgroundTransferEventArgs>>) (a => MediaDownload.ReportProgress(observer, req.BytesReceived + req.BytesSent, req.TotalBytesToReceive + req.TotalBytesToSend))).Merge<IEvent<BackgroundTransferEventArgs>>(req.GetTransferStatusChangedAsync()).Do<IEvent<BackgroundTransferEventArgs>>((Action<IEvent<BackgroundTransferEventArgs>>) (a =>
        {
          TransferStatus transferStatus = a.EventArgs.Request.TransferStatus;
          Exception transferError = a.EventArgs.Request.TransferError;
          observer.OnNext(new MediaDownload.MediaProgress()
          {
            Status = new TransferStatus?(transferStatus)
          });
        })).Where<IEvent<BackgroundTransferEventArgs>>((Func<IEvent<BackgroundTransferEventArgs>, bool>) (a => a.EventArgs.Request.TransferStatus == TransferStatus.Completed || a.EventArgs.Request.TransferStatus == TransferStatus.Unknown)).Take<IEvent<BackgroundTransferEventArgs>>(1).Do<IEvent<BackgroundTransferEventArgs>>((Action<IEvent<BackgroundTransferEventArgs>>) (a =>
        {
          try
          {
            BackgroundTransferService.Remove(req);
          }
          catch (ObjectDisposedException ex)
          {
          }
          if (a.EventArgs.Request.TransferError != null)
          {
            Log.LogException(a.EventArgs.Request.TransferError, "background transfer");
            release();
          }
          else if ((int) a.EventArgs.Request.StatusCode < 200 || (int) a.EventArgs.Request.StatusCode > 299)
          {
            Log.l(MediaDownload.LogHeader, "background transfer - Unexpected status " + (object) a.EventArgs.Request.StatusCode);
            release();
          }
          else
          {
            string originalString = a.EventArgs.Request.DownloadLocation.OriginalString;
            observer.OnNext(new MediaDownload.MediaProgress()
            {
              TransferProgress = new double?(MediaDownload.MAX_DOWNLOAD_PROPORTION),
              FileName = originalString
            });
            release();
          }
        })).Subscribe<IEvent<BackgroundTransferEventArgs>>();
        if (req.TransferStatus == TransferStatus.Completed)
        {
          try
          {
            BackgroundTransferService.Remove(req);
          }
          catch (ObjectDisposedException ex)
          {
          }
          string originalString = req.DownloadLocation.OriginalString;
          observer.OnNext(new MediaDownload.MediaProgress()
          {
            TransferProgress = new double?(MediaDownload.MAX_DOWNLOAD_PROPORTION),
            FileName = originalString
          });
          release();
        }
        else if (BackgroundTransferService.Find(req.RequestId) == null)
        {
          try
          {
            AppState.ClientInstance.Add(req);
            observer.OnNext(new MediaDownload.MediaProgress()
            {
              BackgroundId = req.RequestId
            });
          }
          catch (Exception ex)
          {
            observer.OnNext(new MediaDownload.MediaProgress()
            {
              BtsFailedToStart = true
            });
          }
        }
        return release;
      }));
    }

    private static IObservable<MediaDownload.MediaProgress> InAppDownloadMaybeMms4(
      string filename,
      bool allowResume,
      Message msg,
      WhatsApp.Events.MediaDownload fsEvent)
    {
      if (Mms4Helper.IsMms4DownloadMessage(msg))
      {
        fsEvent.mmsVersion = new long?(4L);
        FunXMPP.FMessage.FunMediaType funType = msg.GetFunMediaType();
        byte[] cipherHash = msg.GetCipherMediaHash();
        long routeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
        return Mms4HostSelector.GetInstance().GetSelectedHostObservable(true, msg.GetFunMediaType(), true).SelectMany<Mms4HostSelector.Mms4HostSelection, MediaDownload.MediaProgress, MediaDownload.MediaProgress>((Func<Mms4HostSelector.Mms4HostSelection, IObservable<MediaDownload.MediaProgress>>) (hostSelection => MediaDownload.InAppDownload(Mms4Helper.CreateUrlStringForDownload(hostSelection.HostName, funType, cipherHash, false, msg.GetDirectPath()), (string) null, filename, true, msg, fsEvent, routeStartTimeUtcTicks)), (Func<Mms4HostSelector.Mms4HostSelection, MediaDownload.MediaProgress, MediaDownload.MediaProgress>) ((hostSelection, obs) => obs));
      }
      fsEvent.mmsVersion = new long?(3L);
      return MediaDownload.InAppDownload(msg.MediaUrl, (string) null, filename, true, msg, fsEvent, -1L);
    }

    private static IObservable<MediaDownload.MediaProgress> InAppDownload(
      string url,
      string mms4IpHint,
      string filename,
      bool allowResume,
      Message msg,
      WhatsApp.Events.MediaDownload fsEvent,
      long mms4RouteStartUtcTicks)
    {
      Action<IWebRequest> setResolver = (Action<IWebRequest>) null;
      Func<string, string> urlMap = (Func<string, string>) (u => u);
      bool bytesRead = false;
      bool isMms4 = mms4RouteStartUtcTicks > 0L;
      string ip = isMms4 ? mms4IpHint : msg.MediaIp;
      if (isMms4)
      {
        fsEvent.routeIp = mms4IpHint;
        fsEvent.routeSelectionDelayT = new long?((DateTime.UtcNow.Ticks - mms4RouteStartUtcTicks) / 10000L);
      }
      Log.d(MediaDownload.LogHeader, "creating observable for {0}", (object) msg.KeyId);
      IObservable<MediaDownload.MediaProgress> observable = Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
      {
        if (msg.Status == FunXMPP.FMessage.Status.Error)
          AppState.Worker.Enqueue((Action) (() =>
          {
            MessageMiscInfo misc = msg.GetMiscInfo();
            if (misc == null)
              return;
            int? errorCode = misc.ErrorCode;
            if (!errorCode.HasValue)
              return;
            errorCode = misc.ErrorCode;
            if (errorCode.Value != 4)
            {
              errorCode = misc.ErrorCode;
              if (errorCode.Value != 9)
                return;
            }
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              msg.Status = FunXMPP.FMessage.Status.Undefined;
              misc.ErrorCode = new int?(0);
              db.SubmitChanges();
            }));
          }));
        Stream output = (Stream) null;
        IsolatedStorageFile fs = IsolatedStorageFile.GetUserStoreForApplication();
        string inputFileName = filename;
        lock (msg)
        {
          Log.l(MediaDownload.LogHeader, "Using in-app HTTP APIs to download {0}, KeyId: {1}, Sender: {2}", (object) MediaDownload.RedactUrl(url), (object) msg.KeyId, (object) (msg.RemoteResource ?? msg.KeyRemoteJid));
          try
          {
            output = (Stream) fs.OpenFile(inputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
          }
          catch (Exception ex1)
          {
            Log.l(MediaDownload.LogHeader, "Unexpected exception creating {0}, {1}", (object) inputFileName, (object) ex1.GetFriendlyMessage());
            try
            {
              fs.DeleteFile(inputFileName);
              Log.l(MediaDownload.LogHeader, "Deleted work file");
              output = (Stream) fs.OpenFile(inputFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            }
            catch (Exception ex2)
            {
              Log.l(MediaDownload.LogHeader, "Further unexpected exception creating {0}, {1}", (object) inputFileName, (object) ex2.GetFriendlyMessage());
              Log.SendCrashLog((Exception) new InvalidOperationException("Download Isostore issues"), "Further exception", logOnlyForRelease: true);
              observer.OnError(ex1);
              observer.OnCompleted();
              return (Action) (() => { });
            }
            Log.SendCrashLog((Exception) new InvalidOperationException("Download Isostore issues"), "Initial exception", logOnlyForRelease: true);
          }
          Log.d(MediaDownload.LogHeader, "Downloading to {0}", (object) inputFileName);
        }
        Dictionary<string, string> headers = new Dictionary<string, string>();
        if (output.Length > 1L & allowResume || msg.IsPrefetchingVideo)
        {
          long num = output.Length > 1L & allowResume ? output.Length - 1L : 0L;
          string str9 = msg.IsPrefetchingVideo ? (Settings.VideoPrefetchBytes - 1).ToString() : (string) null;
          string str10 = string.Format("bytes={0}-{1}", (object) num, (object) str9);
          Log.l(string.Format("using Range: {0}", (object) str10));
          headers["Range"] = str10;
          fsEvent.downloadResumePoint = new long?((long) (int) num);
        }
        else
          fsEvent.downloadResumePoint = new long?(0L);
        IDisposable downloadSub = (IDisposable) null;
        IDisposable lockScreenSub = (IDisposable) null;
        object releaseLock = new object();
        int messageId = msg.MessageID;
        Action release = Utils.IgnoreMultipleInvokes((Action) (() =>
        {
          Log.l(MediaDownload.LogHeader, "running release {0}", (object) messageId);
          lock (releaseLock)
          {
            downloadSub.SafeDispose();
            downloadSub = (IDisposable) null;
            lockScreenSub.SafeDispose();
            lockScreenSub = (IDisposable) null;
            output.SafeDispose();
            output = (Stream) null;
            fs.SafeDispose();
            fs = (IsolatedStorageFile) null;
            observer.OnCompleted();
          }
        }));
        lock (releaseLock)
          downloadSub = NativeWeb.Create<Unit>(isMms4 ? NativeWeb.Options.Default | NativeWeb.Options.KeepAlive : NativeWeb.Options.Default, (Action<IWebRequest, IObserver<Unit>>) ((req, unitObs) =>
          {
            long len = 0;
            long totalRead = 0;
            fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
            Stopwatch timer = new Stopwatch();
            Action<Action> wrapExn = (Action<Action>) (a =>
            {
              try
              {
                a();
              }
              catch (Exception ex)
              {
                timer.Stop();
                fsEvent.mediaDownloadT = new long?(timer.ElapsedMilliseconds);
                req.Cancel();
                observer.OnError(ex);
                release();
              }
            });
            NativeWeb.Callback callbackObject = (NativeWeb.Callback) null;
            callbackObject = new NativeWeb.Callback()
            {
              OnBeginResponse = (Action<int, string>) ((code, headerStrings) => wrapExn((Action) (() =>
              {
                if (code < 200 || code > 299)
                {
                  if (isMms4)
                    Mms4RouteSelector.GetInstance().OnMediaTransferErrorOrResponseCode(code);
                  switch (code)
                  {
                    case 401:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_INVALID_URL);
                      break;
                    case 404:
                    case 410:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_TOO_OLD);
                      break;
                    case 408:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_TIMEOUT);
                      break;
                    case 416:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_CANNOT_RESUME);
                      break;
                    case 504:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_DNS);
                      break;
                    default:
                      fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_UNKNOWN);
                      break;
                  }
                  throw new HttpStatusException((HttpStatusCode) code);
                }
                headers.Clear();
                foreach (KeyValuePair<string, string> header in NativeWeb.ParseHeaders(headerStrings))
                  headers[header.Key.ToLowerInvariant()] = header.Value;
                string valueOrDefault3 = headers.GetValueOrDefault<string, string>("content-length");
                if (valueOrDefault3 == null || !long.TryParse(valueOrDefault3, out len))
                  throw new WebException("Could not parse content-length");
                string valueOrDefault4 = headers.GetValueOrDefault<string, string>("content-range");
                if (valueOrDefault4 != null)
                {
                  long start = MediaDownload.ParseContentRange(valueOrDefault4).Start;
                  Log.l(MediaDownload.LogHeader, "Starting download at byte {0}", (object) start);
                  output.Position = start;
                }
                if (len == 0L)
                  return;
                MediaDownload.ReportProgress(observer, 0L, len);
              }))),
              OnBytesIn = (Action<byte[]>) (buf => wrapExn((Action) (() =>
              {
                int length = buf.Length;
                if (length != 0)
                  bytesRead = true;
                try
                {
                  lock (releaseLock)
                  {
                    if (output == null)
                      throw new OperationCanceledException();
                    output.Write(buf, 0, length);
                  }
                }
                catch (IsolatedStorageException ex)
                {
                  fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.ERROR_INSUFFICIENT_SPACE);
                  throw;
                }
                if (totalRead == 0L && callbackObject != null && callbackObject.writeStartTimeUtcTicks > 0L)
                  fsEvent.connectT = new long?((callbackObject.writeStartTimeUtcTicks - callbackObject.createTimeUtcTicks) / 10000L);
                totalRead += (long) length;
                MediaDownload.ReportProgress(observer, totalRead, len);
              }))),
              OnEndResponse = (Action) (() => wrapExn((Action) (() =>
              {
                timer.Stop();
                fsEvent.mediaDownloadT = new long?(timer.ElapsedMilliseconds);
                if (callbackObject != null && callbackObject.writeStartTimeUtcTicks > 0L)
                {
                  fsEvent.connectT = new long?((callbackObject.writeStartTimeUtcTicks - callbackObject.createTimeUtcTicks) / 10000L);
                  fsEvent.networkDownloadT = new long?((DateTime.UtcNow.Ticks - callbackObject.createTimeUtcTicks) / 10000L);
                }
                bool flag = len == totalRead;
                lock (releaseLock)
                {
                  if (output == null || fs == null)
                    return;
                  if (flag)
                  {
                    string s = (string) null;
                    string str5 = (string) null;
                    if (!isMms4 && headers.TryGetValue("x-wa-metadata", out str5))
                    {
                      foreach (string str6 in ((IEnumerable<string>) str5.Split(';')).Select<string, string>((Func<string, string>) (a => a.Trim())))
                      {
                        int length = str6.IndexOf('=');
                        if (length > 0)
                        {
                          string str7 = str6.Substring(0, length);
                          string str8 = str6.Substring(length + 1);
                          switch (str7)
                          {
                            case "filehash":
                              s = str8.FromUrlSafeBase64String();
                              continue;
                            default:
                              continue;
                          }
                        }
                      }
                    }
                    if (s != null && !msg.IsPrefetchingVideo)
                    {
                      output.Position = 0L;
                      if (!MediaUpload.ComputeHash(output).IsEqualBytes(Convert.FromBase64String(s)))
                      {
                        flag = false;
                        output.Dispose();
                        output = (Stream) null;
                        fs.DeleteFile(inputFileName);
                        fs.Dispose();
                        fs = (IsolatedStorageFile) null;
                        observer.OnError((Exception) new IOException("Hash did not match remote hash."));
                        return;
                      }
                    }
                  }
                  output.Flush();
                  output.Dispose();
                  output = (Stream) null;
                  fs.Dispose();
                  fs = (IsolatedStorageFile) null;
                }
                if (!flag)
                  return;
                observer.OnNext(new MediaDownload.MediaProgress()
                {
                  TransferProgress = new double?(MediaDownload.MAX_DOWNLOAD_PROPORTION),
                  FileName = filename
                });
                fsEvent.mediaDownloadResult = new wam_enum_media_download_result_type?(wam_enum_media_download_result_type.OK);
              })))
            };
            try
            {
              timer.Start();
              Action<IWebRequest> action = setResolver;
              if (action != null)
                action(req);
              string str = urlMap(url);
              if (str != url)
                Log.l(MediaDownload.LogHeader, "Trying [{0}]...", (object) MediaDownload.RedactUrl(str));
              FieldStats.SetHostDetailsInDownloadEvent(fsEvent, str);
              req.Open(str, (IWebCallback) callbackObject, headers: string.Join("\r\n", headers.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => string.Format("{0}: {1}", (object) kv.Key, (object) kv.Value)))));
            }
            catch (Exception ex)
            {
              timer.Stop();
              fsEvent.mediaDownloadT = new long?(timer.ElapsedMilliseconds);
              req.Cancel();
              observer.OnError(ex);
            }
            finally
            {
              release();
            }
          })).Subscribe<Unit>();
        return release;
      })).ObserveOn<MediaDownload.MediaProgress>(WAThreadPool.Scheduler);
      if (allowResume)
      {
        IObservable<MediaDownload.MediaProgress> source = observable;
        observable = Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
        {
          object @lock = new object();
          IDisposable disp = (IDisposable) null;
          lock (@lock)
            disp = source.Subscribe<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (_ => observer.OnNext(_)), (Action<Exception>) (ex =>
            {
              HttpStatusCode? nullable = MediaDownload.StatusCodeFromException(ex);
              HttpStatusCode httpStatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
              if ((nullable.GetValueOrDefault() == httpStatusCode ? (nullable.HasValue ? 1 : 0) : 0) != 0)
              {
                allowResume = false;
                lock (@lock)
                {
                  if (disp == null)
                    return;
                  disp.Dispose();
                  disp = source.Subscribe(observer);
                }
              }
              else
                observer.OnError(ex);
            }), (Action) (() =>
            {
              if (!allowResume)
                return;
              observer.OnCompleted();
            }));
          return (Action) (() =>
          {
            lock (@lock)
            {
              disp.SafeDispose();
              disp = (IDisposable) null;
            }
          });
        }));
      }
      List<MediaDownload.FallbackOption> hints = new List<MediaDownload.FallbackOption>();
      MediaDownload.ApplyIpHint(hints, ip);
      if (!isMms4)
        hints.Add(new MediaDownload.FallbackOption()
        {
          UrlMap = (Func<string, string>) (oldUrl => MediaDownload.ReplaceUrlHostname(oldUrl, "mms.whatsapp.net"))
        });
      int idx = 0;
      IObservable<MediaDownload.MediaProgress> source1 = observable;
      IObservable<MediaDownload.MediaProgress> source2 = Observable.Create<MediaDownload.MediaProgress>((Func<IObserver<MediaDownload.MediaProgress>, Action>) (observer =>
      {
        Action subscribe = (Action) null;
        object @lock = new object();
        IDisposable currentSub = (IDisposable) null;
        bool cancel = false;
        subscribe = (Action) (() =>
        {
          MediaDownload.FallbackOption fallbackOption = hints[idx++];
          setResolver = fallbackOption.SetResolver;
          urlMap = fallbackOption.UrlMap ?? (Func<string, string>) (u => u);
          IDisposable d = source1.Subscribe<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (_ => observer.OnNext(_)), (Action<Exception>) (ex =>
          {
            if (bytesRead || idx >= hints.Count)
            {
              observer.OnError(ex);
              observer.OnCompleted();
            }
            else
              subscribe();
          }), (Action) (() =>
          {
            if (!bytesRead)
              return;
            observer.OnCompleted();
          }));
          lock (@lock)
          {
            if (cancel)
            {
              d.SafeDispose();
              d = (IDisposable) null;
            }
            currentSub.SafeDispose();
            currentSub = d;
          }
        });
        subscribe();
        return (Action) (() =>
        {
          lock (@lock)
          {
            cancel = true;
            currentSub.SafeDispose();
            currentSub = (IDisposable) null;
          }
        });
      })).Do<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (_ => { }), (Action<Exception>) (ex => MediaDownload.SetMessageErrorFromException(msg, ex)));
      Log.d(MediaDownload.LogHeader, "created observable for {0}", (object) msg.KeyId);
      return MediaDownload.RateLimit<MediaDownload.MediaProgress>(source2);
    }

    public static void SetMessageErrorFromException(Message msg, Exception ex)
    {
      HttpStatusCode? nullable = MediaDownload.StatusCodeFromException(ex);
      if (!nullable.HasValue)
        return;
      MessageMiscInfo.MessageError msgError = MessageMiscInfo.MessageError.None;
      switch (nullable.Value)
      {
        case HttpStatusCode.NotFound:
          msgError = MessageMiscInfo.MessageError.ItemNotFound;
          break;
        case HttpStatusCode.Gone:
          msgError = MessageMiscInfo.MessageError.FileGone;
          break;
      }
      if (msgError == MessageMiscInfo.MessageError.None)
        return;
      Log.l(MediaDownload.LogHeader, "Setting error status due to error {0}", (object) msgError.ToString());
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = db.GetMessage(msg.KeyRemoteJid, msg.KeyId, msg.KeyFromMe);
        if (message == null)
          return;
        message.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateToDbIfNotFound).ErrorCode = new int?((int) msgError);
        message.Status = FunXMPP.FMessage.Status.Error;
        db.SubmitChanges();
      }));
    }

    public static MediaDownload.ParsedContentRange ParseContentRange(string range)
    {
      Log.l(MediaDownload.LogHeader, "Content-Range: {0}", (object) range);
      string[] strArray1 = range.Split(' ');
      string str = strArray1.Length >= 2 ? strArray1[0] : throw new WebException("Could not parse content-range");
      if (str != "bytes")
        throw new WebException("Content-range has unusual Units.\nUnit = " + str);
      string[] strArray2 = strArray1[1].Split('/');
      if (strArray2.Length < 2)
        throw new WebException("Could not parse content-range, no slash");
      MediaDownload.ParsedContentRange contentRange = new MediaDownload.ParsedContentRange();
      if (strArray2[1] != "*")
      {
        long result = 0;
        if (!long.TryParse(strArray2[1], out result))
          throw new WebException("Could not parse content-range; bad size\nSize = " + strArray2[1]);
        contentRange.FileSize = new long?(result);
      }
      if (strArray2[0] == "*")
      {
        if (!contentRange.FileSize.HasValue)
          throw new WebException("Invalid content range */*");
        contentRange.End = contentRange.FileSize.Value;
        if (contentRange.End != 0L)
          --contentRange.End;
      }
      else
      {
        string[] strArray3 = strArray2[0].Split('-');
        if (strArray3.Length < 2)
          throw new WebException("Could not parse content range");
        if (!long.TryParse(strArray3[0], out contentRange.Start))
          throw new WebException("Could not parse content-range; no start");
        if (!long.TryParse(strArray3[1], out contentRange.End))
          throw new WebException("Could not parse content-range; no end");
      }
      return contentRange;
    }

    public static string ReplaceUrlHostname(string oldUrl, string newHost)
    {
      int num1 = oldUrl.IndexOf(':');
      if (num1 > 0)
      {
        int num2 = num1 + 1;
        while (num2 < oldUrl.Length && oldUrl[num2] == '/')
          ++num2;
        int startIndex = num2 < oldUrl.Length ? oldUrl.IndexOf('/', num2) : -1;
        if (startIndex > 0)
        {
          oldUrl.Substring(num2, startIndex - num2);
          return oldUrl.Substring(0, num2) + newHost + oldUrl.Substring(startIndex);
        }
      }
      return oldUrl;
    }

    private static void ApplyIpHint(
      List<MediaDownload.FallbackOption> r,
      string ip,
      Func<string, string> urlMap = null)
    {
      MediaDownload.FallbackOption fallbackOption1;
      if (ip != null)
      {
        List<MediaDownload.FallbackOption> fallbackOptionList = r;
        fallbackOption1 = new MediaDownload.FallbackOption();
        fallbackOption1.SetResolver = (Action<IWebRequest>) (req => req.SetResolver(new IpResolver()
        {
          Ip = ip
        }.ToNativeResolver()));
        fallbackOption1.UrlMap = urlMap;
        MediaDownload.FallbackOption fallbackOption2 = fallbackOption1;
        fallbackOptionList.Add(fallbackOption2);
      }
      List<MediaDownload.FallbackOption> fallbackOptionList1 = r;
      fallbackOption1 = new MediaDownload.FallbackOption();
      fallbackOption1.UrlMap = urlMap;
      MediaDownload.FallbackOption fallbackOption3 = fallbackOption1;
      fallbackOptionList1.Add(fallbackOption3);
    }

    private static HttpStatusCode? StatusCodeFromException(Exception ex)
    {
      HttpStatusCode? nullable = new HttpStatusCode?();
      switch (ex)
      {
        case HttpStatusException httpStatusException:
          return new HttpStatusCode?(httpStatusException.StatusCode);
        case WebException webException:
          StringBuilder stringBuilder = new StringBuilder();
          List<object> objectList = new List<object>();
          stringBuilder.Append("Managed HTTP API signalled failure: {0}");
          objectList.Add((object) webException.Status);
          if (webException.Response is HttpWebResponse response)
          {
            stringBuilder.Append(", response error: {1}");
            objectList.Add((object) response.StatusCode);
            if (response.ContentLength != 0L || response.SupportsHeaders && response.Headers.Count != 0)
            {
              stringBuilder.Append(" ({2}) - appears to come from server");
              objectList.Add((object) (int) response.StatusCode);
              nullable = new HttpStatusCode?(response.StatusCode);
            }
          }
          if (!nullable.HasValue)
            stringBuilder.Append(" - looks like transient connectivity error");
          Log.WriteLineDebug(stringBuilder.ToString(), objectList.ToArray());
          break;
      }
      return nullable;
    }

    public static string SanitizeIsoStorePath(string str, bool useBackSlash = true)
    {
      char newChar = useBackSlash ? '\\' : '/';
      char oldChar = useBackSlash ? '/' : '\\';
      str = str.Replace(oldChar, newChar);
      int num = 0;
      while (num < str.Length && (int) str[num] == (int) newChar)
        ++num;
      if (num > 0)
        str = str.Substring(num);
      return str;
    }

    private static async Task<string> CopyAsync(
      string dirOrUri,
      FunXMPP.FMessage.Type type,
      string filename,
      string targetFilename = null,
      MessagesContext db = null,
      bool isPtt = false,
      string destDir = "WhatsApp",
      bool duplicatingFile = false)
    {
      StorageFolder destLib = KnownFolders.PicturesLibrary;
      switch (destDir)
      {
        case "Camera Roll":
          destLib = KnownFolders.CameraRoll;
          break;
        case "Saved Pictures":
          destLib = KnownFolders.SavedPictures;
          break;
        default:
          destLib = await destLib.CreateFolderAsync(destDir, (CreationCollisionOption) 3);
          string str1 = (string) null;
          bool useWeekly = false;
          if (type == FunXMPP.FMessage.Type.Audio && isPtt)
          {
            str1 = "PTT";
            useWeekly = true;
          }
          if (str1 != null)
            destLib = await destLib.CreateFolderAsync(str1, (CreationCollisionOption) 3);
          if (useWeekly)
          {
            DateTime now = DateTime.Now;
            int num = now.DayOfYear / 7 + 1;
            destLib = await destLib.CreateFolderAsync(string.Format("{0}-{1}", (object) now.Year, (object) num.ToString().PadLeft(2, '0')), (CreationCollisionOption) 3);
            break;
          }
          break;
      }
      StorageFile storageFile;
      if (!duplicatingFile)
      {
        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        string[] strArray = dirOrUri.Split('\\');
        for (int index = 0; index < strArray.Length; ++index)
        {
          string str2 = strArray[index];
          storageFolder = await storageFolder.GetFolderAsync(str2);
        }
        strArray = (string[]) null;
        storageFile = await storageFolder.GetFileAsync(filename);
      }
      else
        storageFile = await StorageFile.GetFileFromPathAsync(MediaStorage.GetAbsolutePath(dirOrUri));
      string filename1 = MediaDownload.GenerateFilename(destLib.Path, targetFilename ?? filename, type, db, isPtt);
      Action<string> onPath = (Action<string>) (p => { });
      if (!AppState.IsWP10OrLater && destLib.Path.StartsWith("D:", StringComparison.OrdinalIgnoreCase) && (type == FunXMPP.FMessage.Type.Image || type == FunXMPP.FMessage.Type.Video || type == FunXMPP.FMessage.Type.Gif || type == FunXMPP.FMessage.Type.Sticker))
      {
        try
        {
          IMediaLibrary lib = NativeInterfaces.MediaLib;
          lib.SuppressNotification(ZMediaNotificationType.All, destDir + "\\" + filename1);
          onPath = (Action<string>) (p =>
          {
            lib.ForceInsert(p);
            lib.SuppressNotification(ZMediaNotificationType.None, p);
          });
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "sd card workaround");
        }
      }
      string path = (await storageFile.CopyAsync((IStorageFolder) destLib, filename1, (NameCollisionOption) 1)).Path;
      onPath(path);
      return path;
    }

    private static Pair<string, string> GenerateFilenamePrefixAndExtention(
      string suggestedName,
      FunXMPP.FMessage.Type msgType,
      bool isPtt,
      string prefixEnding)
    {
      string second = "";
      int startIndex = suggestedName.LastIndexOf('.');
      if (startIndex >= 0)
        second = suggestedName.Substring(startIndex);
      if (isPtt)
        second += ".waptt";
      string str;
      switch (msgType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Gif:
          str = "IMG";
          break;
        case FunXMPP.FMessage.Type.Audio:
          str = isPtt ? "PTT" : "AUD";
          break;
        case FunXMPP.FMessage.Type.Video:
          str = "VID";
          break;
        case FunXMPP.FMessage.Type.Document:
          str = "DOC";
          break;
        case FunXMPP.FMessage.Type.Sticker:
          str = "STK";
          break;
        default:
          str = "WA";
          break;
      }
      DateTime now = DateTime.Now;
      object[] objArray = new object[5]
      {
        (object) str,
        (object) now.Year,
        null,
        null,
        null
      };
      int num = now.Month;
      objArray[2] = (object) num.ToString().PadLeft(2, '0');
      num = now.Day;
      objArray[3] = (object) num.ToString().PadLeft(2, '0');
      objArray[4] = (object) prefixEnding;
      return new Pair<string, string>(string.Format("{0}-{1}{2}{3}-{4}", objArray), second);
    }

    public static string GenerateFilename(
      string dirPath,
      string suggestedName,
      FunXMPP.FMessage.Type type,
      MessagesContext db,
      bool isPtt,
      string prefixEnding = "WA",
      Action<string> tryReserveFile = null)
    {
      Pair<string, string> prefixAndExtention = MediaDownload.GenerateFilenamePrefixAndExtention(suggestedName, type, isPtt, prefixEnding);
      string first = prefixAndExtention.First;
      string second = prefixAndExtention.Second;
      using (NativeMediaStorage fs = new NativeMediaStorage())
      {
        if (tryReserveFile == null)
          tryReserveFile = (Action<string>) (name =>
          {
            using (fs.OpenFile(name, FileMode.CreateNew, FileAccess.Write))
              ;
          });
        StringBuilder stringBuilder = new StringBuilder(dirPath);
        stringBuilder.Append('\\');
        int length1 = stringBuilder.Length;
        stringBuilder.Append(first);
        int length2 = stringBuilder.Length;
        MediaDownload.FilenameSearchRetryState searchRetryState = new MediaDownload.FilenameSearchRetryState();
        int num = MediaDownload.LastMediaPrefix == first ? MediaDownload.LastMediaInteger : 1;
label_4:
        LocalFile existingFile;
        string filepath;
        do
        {
          do
          {
            stringBuilder.Length = length2;
            stringBuilder.Append(num++.ToString().PadLeft(4, '0'));
            stringBuilder.Append(second);
            filepath = stringBuilder.ToString();
          }
          while (fs.FileExists(filepath));
          existingFile = (LocalFile) null;
          Action<MessagesContext> onDb = (Action<MessagesContext>) (innerDb => existingFile = innerDb.GetLocalFileByUri(filepath));
          if (db == null)
            MessagesContext.Run((MessagesContext.MessagesCallback) (d => onDb(d)));
          else
            onDb(db);
        }
        while (existingFile != null);
        try
        {
          tryReserveFile(filepath);
        }
        catch (Exception ex)
        {
          MediaDownload.FilenameSearchRetryState retryState = searchRetryState;
          if (!MediaDownload.ShouldContinueFilenameSearch(ex, retryState))
            throw;
          else
            goto label_4;
        }
        MediaDownload.LastMediaInteger = num;
        MediaDownload.LastMediaPrefix = first;
        return filepath.Substring(length1);
      }
    }

    public static bool ShouldContinueFilenameSearch(
      Exception ex,
      MediaDownload.FilenameSearchRetryState retryState)
    {
      uint hresult = ex.GetHResult();
      ++retryState.NumAttempts;
      switch (hresult)
      {
        case 2147942405:
          if (retryState.NumAttempts > 5)
            return false;
          goto case 2147942432;
        case 2147942432:
        case 2147942480:
        case 2147942583:
          return true;
        default:
          return ex is IsolatedStorageException;
      }
    }

    public static string SaveMediaToCameraRoll(
      string uri,
      FunXMPP.FMessage.Type mediaType,
      string destFilename = null,
      MessagesContext db = null,
      bool isPtt = false,
      string saveAlbum = "WhatsApp")
    {
      return MediaDownload.SaveMedia(uri, mediaType, destFilename, db, isPtt, saveAlbum);
    }

    public static string SaveSticker(string uri, MessagesContext db = null)
    {
      string.Format("{0}\\Stickers", (object) Constants.IsoStorePath);
      return uri;
    }

    public static string SaveMedia(
      string uri,
      FunXMPP.FMessage.Type mediaType,
      string destFilename = null,
      MessagesContext db = null,
      bool isPtt = false,
      string saveAlbum = "WhatsApp",
      bool duplicatingFile = false)
    {
      switch (mediaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Document:
        case FunXMPP.FMessage.Type.Gif:
          uri = MediaDownload.SanitizeIsoStorePath(uri);
          int length = uri.LastIndexOf('\\');
          string str = uri.Substring(0, length);
          string name = uri.Substring(length + 1);
          string dirOrUri = duplicatingFile ? uri : str;
          string newUri = (string) null;
          IObservable<string> withDisposable = Observable.CreateWithDisposable<string>((Func<IObserver<string>, IDisposable>) (o => MediaDownload.CopyAsync(dirOrUri, mediaType, name, destFilename, db, isPtt, saveAlbum, duplicatingFile).ToObservable<string>().Subscribe(o)));
          ManualResetEvent ev = new ManualResetEvent(false);
          IScheduler scheduler = WAThreadPool.Scheduler;
          withDisposable.SubscribeOn<string>(scheduler).ObserveOn<string>(WAThreadPool.Scheduler).Subscribe<string>((Action<string>) (r =>
          {
            newUri = NativeMediaStorage.MakeUri(r);
            ev.Set();
          }), (Action<Exception>) (ex =>
          {
            Log.LogException(ex, "save to album");
            ev.Set();
          }));
          ev.WaitOne();
          ev.Dispose();
          return newUri;
        default:
          return (string) null;
      }
    }

    private static bool BtsStatusIsFlakey(TransferStatus status)
    {
      return ((IEnumerable<TransferStatus>) new TransferStatus[5]
      {
        TransferStatus.Waiting,
        TransferStatus.WaitingForExternalPower,
        TransferStatus.WaitingForExternalPowerDueToBatterySaverMode,
        TransferStatus.WaitingForNonVoiceBlockingNetwork,
        TransferStatus.WaitingForWiFi
      }).Contains<TransferStatus>(status);
    }

    public static IObservable<T> RateLimit<T>(IObservable<T> source) => source.ObserveInQueue<T>();

    public class MediaProgress
    {
      public double? TransferProgress;
      public string BackgroundId;
      public TransferStatus? Status;
      public string FileName;
      public bool BtsFailedToStart;
      public byte[] DuplicatedMediaKey;
      public byte[] DuplicatedMediaHash;
      public string DuplicatedMediaUrl;
      public string DuplicatedDirectPath;

      public bool isDuplicated() => this.DuplicatedMediaKey != null;
    }

    private class FileMagic
    {
      public string Extension;
      public byte[] Magic;
    }

    private struct FallbackOption
    {
      public Action<IWebRequest> SetResolver;
      public Func<string, string> UrlMap;
    }

    public struct ParsedContentRange
    {
      public long Start;
      public long End;
      public long? FileSize;
    }

    public class FilenameSearchRetryState
    {
      public int NumAttempts;
    }
  }
}
