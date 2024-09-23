// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaUploadMms4
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Windows;
using WhatsApp.Resolvers;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class MediaUploadMms4
  {
    private const string LogHeader = "mms4 upload";

    public static IObservable<Unit> SendMediaObservableImplMms4(Message msg, bool webRetry = false)
    {
      bool cancel = false;
      string localUri = (string) null;
      string altName = (string) null;
      AxolotlMediaCipher mediaCipher = (AxolotlMediaCipher) null;
      byte[] cipherMediaHash = (byte[]) null;
      WhatsApp.Events.MediaUpload fsEvent = FieldStats.GetFsMediaUploadEvent(msg);
      fsEvent.mmsVersion = new long?(4L);
      Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>> actualUploadObservable = (Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>) null;
      FunXMPP.Connection connection = AppState.ClientInstance.GetConnection();
      if (msg.UploadContext.isOptimisticUpload())
      {
        if (connection == null || !connection.IsConnected)
          return Observable.Return<Unit>(new Unit());
        fsEvent.optimisticFlag = new wam_enum_optimistic_flag_type?(wam_enum_optimistic_flag_type.OPTIMISTIC);
        localUri = msg.LocalFileUri;
      }
      else
      {
        if (!MediaUpload.OptimisticUploadAllowed)
          fsEvent.optimisticFlag = new wam_enum_optimistic_flag_type?(wam_enum_optimistic_flag_type.OPT_DISABLED);
        else if (msg.UploadContext.wasOptimisticallyUploaded())
        {
          fsEvent.optimisticFlag = new wam_enum_optimistic_flag_type?(wam_enum_optimistic_flag_type.OPT_USED);
          FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.OK);
          DateTime? funTimestamp = msg.FunTimestamp;
          if (funTimestamp.HasValue)
          {
            WhatsApp.Events.MediaUpload mediaUpload = fsEvent;
            long unixTime1 = DateTime.Now.ToUnixTime();
            funTimestamp = msg.FunTimestamp;
            long unixTime2 = funTimestamp.Value.ToUnixTime();
            long? nullable = new long?((unixTime1 - unixTime2) * 1000L);
            mediaUpload.userVisibleT = nullable;
          }
        }
        else
          fsEvent.optimisticFlag = new wam_enum_optimistic_flag_type?(wam_enum_optimistic_flag_type.NONE);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          switch (msg.Status)
          {
            case FunXMPP.FMessage.Status.Uploading:
            case FunXMPP.FMessage.Status.UploadingCustomHash:
              localUri = MediaUpload.GetAndSanitizeFilePath(db, msg, ref altName);
              break;
            default:
              Log.WriteLineDebug("media upload: unexpected state {0} | msg_id={1}", (object) msg.Status, (object) msg.MessageID);
              cancel = true;
              break;
          }
        }));
      }
      if (cancel)
      {
        FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_CANCEL);
        return Observable.Return<Unit>(new Unit());
      }
      if (msg.UploadContext.wasOptimisticallyUploaded())
      {
        OptimisticJpegUploadContextMms4 optMsgContext = msg.UploadContext as OptimisticJpegUploadContextMms4;
        Log.l("OPU", "using optimistically uploaded file | OuId={0}", (object) optMsgContext.OuId);
        mediaCipher = optMsgContext.MediaCipher;
        cipherMediaHash = optMsgContext.MediaCipherHash;
        msg.UploadContext = (UploadContext) null;
        return Observable.Create<MediaUploadMms4.Mms4UploadResult>((Func<IObserver<MediaUploadMms4.Mms4UploadResult>, Action>) (observer =>
        {
          observer.OnNext(optMsgContext.uploadResultMms4);
          return (Action) (() => { });
        })).Do<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (resp => MediaUploadMms4.ProcessUploadResponse(msg, mediaCipher, resp, fsEvent)), (Action<Exception>) (ex => Log.SendCrashLog(ex, "Exception processing optimistically uploaded file"))).Select<MediaUploadMms4.Mms4UploadResult, Unit>((Func<MediaUploadMms4.Mms4UploadResult, Unit>) (resumeResponse => new Unit())).Do<Unit>((Action<Unit>) (_ => fsEvent.SaveEvent()), (Action<Exception>) (ex => fsEvent.SaveEvent()));
      }
      Func<Exception, WhatsApp.Events.MediaUpload, Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>> func = (Func<Exception, WhatsApp.Events.MediaUpload, Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>>) ((err, fsUpEvent) => (Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>) (p => Observable.Create<MediaUploadMms4.Mms4UploadResult>((Func<IObserver<MediaUploadMms4.Mms4UploadResult>, Action>) (observer =>
      {
        FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_FNF);
        observer.OnError(err);
        observer.OnCompleted();
        return (Action) (() => { });
      }))));
      if (localUri == null)
      {
        actualUploadObservable = func((Exception) new FunXMPP.Connection.UnforwardableMessageException(), fsEvent);
      }
      else
      {
        try
        {
          MediaUpload.Mp4CheckAndRepair(msg, ref localUri);
        }
        catch (CheckAndRepairException ex)
        {
          actualUploadObservable = func((Exception) ex, fsEvent);
        }
      }
      if (!msg.UploadContext.isOptimisticUpload())
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          cipherMediaHash = msg.GetCipherMediaHash(db);
          if (cipherMediaHash != null)
            return;
          Message duplicateImpl = MediaDownload.FindDuplicateImpl(msg);
          if (duplicateImpl == null)
            return;
          cipherMediaHash = duplicateImpl.GetCipherMediaHash(db);
          msg.MediaKey = duplicateImpl.MediaKey;
          MessageProperties forMessage = MessageProperties.GetForMessage(msg);
          forMessage.EnsureCommonProperties.CipherMediaHash = cipherMediaHash;
          if (duplicateImpl.HasSidecar())
            forMessage.EnsureMediaProperties.Sidecar = duplicateImpl.InternalProperties.MediaPropertiesField.Sidecar;
          forMessage.Save();
          db.SubmitChanges();
        }));
      mediaCipher = AxolotlMediaCipher.CreateUploadCipher(msg, connection.Encryption, webRetry);
      if (mediaCipher == null)
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg.Status = FunXMPP.FMessage.Status.Error;
          db.SubmitChanges();
        }));
        FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_OOM);
        return Observable.Empty<Unit>();
      }
      if (cipherMediaHash == null)
      {
        byte[] generatedHash = (byte[]) null;
        long cipherFileLength;
        long fileLength;
        if (msg.UploadContext.isOptimisticUpload())
          MediaUploadMms4.HashMediaFile(((OptimisticJpegUploadContext) msg.UploadContext).JpegStream, mediaCipher, out generatedHash, out cipherMediaHash, out cipherFileLength, out fileLength);
        else if (!string.IsNullOrEmpty(localUri))
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(localUri))
          {
            using (Stream stream = mediaStorage.OpenFile(localUri))
              MediaUploadMms4.HashMediaFile(stream, mediaCipher, out generatedHash, out cipherMediaHash, out cipherFileLength, out fileLength);
          }
        }
        if (msg.MediaHash == null && generatedHash != null)
          MediaUploadMms4.UpdateHash(msg, generatedHash);
      }
      if (actualUploadObservable == null)
      {
        if (msg.MediaHash == null)
        {
          byte[] generatedHash = (byte[]) null;
          using (IMediaStorage mediaStorage = MediaStorage.Create(localUri))
          {
            using (Stream stream = mediaStorage.OpenFile(localUri))
              generatedHash = MediaUpload.ComputeHash(stream);
          }
          MediaUploadMms4.UpdateHash(msg, generatedHash);
        }
        actualUploadObservable = (Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>) (resp => Observable.Defer<MediaUploadMms4.Mms4UploadResult>((Func<IObservable<MediaUploadMms4.Mms4UploadResult>>) (() =>
        {
          Message msg1 = msg;
          string localUri1 = localUri;
          string basename = altName;
          string uploadUrl = resp.UploadUrl;
          string ipHint1 = resp.IpHint;
          long resumeFrom = resp.ResumeFrom;
          AxolotlMediaCipher mediaCipher1 = mediaCipher;
          WhatsApp.Events.MediaUpload fsEvent1 = fsEvent;
          string ipHint2 = ipHint1;
          int num = webRetry ? 1 : 0;
          return MediaUploadMms4.GetUploadObservableOnDisk(msg1, localUri1, basename, uploadUrl, resumeFrom, mediaCipher1, fsEvent1, ipHint: ipHint2, webRetry: num != 0);
        })));
      }
      byte[] mediaHash = msg.MediaHash;
      IObservable<MediaUploadMms4.Mms4UploadResult> uploadRequestObservable = MediaUploadMms4.SendEncryptedUploadRequest(mediaCipher.MediaResumeUrl, cipherMediaHash, msg.GetFunMediaType(), false, msg.UploadContext.isOptimisticUpload(), fsEvent);
      return connection.ConnectedObservable().SelectMany((Func<Unit, IObservable<MediaUploadMms4.Mms4UploadResult>>) (_ => Observable.Defer<MediaUploadMms4.Mms4UploadResult>((Func<IObservable<MediaUploadMms4.Mms4UploadResult>>) (() => uploadRequestObservable.ObserveOn<MediaUploadMms4.Mms4UploadResult>((IScheduler) AppState.Worker)))), (_, uploadIqResponse) => new
      {
        _ = _,
        uploadIqResponse = uploadIqResponse
      }).SelectMany(_param1 => Observable.If<MediaUploadMms4.Mms4UploadResult>((Func<bool>) (() => _param1.uploadIqResponse.UploadUrl == null), Observable.Return<MediaUploadMms4.Mms4UploadResult>(_param1.uploadIqResponse), Observable.Defer<MediaUploadMms4.Mms4UploadResult>((Func<IObservable<MediaUploadMms4.Mms4UploadResult>>) (() => actualUploadObservable(_param1.uploadIqResponse)))).ObserveOn<MediaUploadMms4.Mms4UploadResult>((IScheduler) AppState.Worker).Do<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (resp => MediaUploadMms4.ProcessUploadResponse(msg, mediaCipher, resp, fsEvent))), (_param1, uploadResult) => new Unit()).Catch<Unit, Exception>((Func<Exception, IObservable<Unit>>) (ex =>
      {
        FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_UPLOAD);
        bool flag = false;
        string errorMsg = (string) null;
        if (!msg.UploadContext.isOptimisticUpload())
          flag = MediaUpload.TryGetFriendlyErrorMessage(msg.MediaWaType, ex, out errorMsg);
        else
          Log.LogException(ex, "OPU Ignoring exception for optimistic upload");
        return flag ? Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          try
          {
            WAThreadPool.QueueUserWorkItem((Action) (() =>
            {
              try
              {
                Log.WriteLineDebug("Setting error status due to error: {0}", (object) errorMsg);
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  msg.Status = FunXMPP.FMessage.Status.Error;
                  db.SubmitChanges();
                }));
                if (!AppState.GetConnection().EventHandler.Qr.Session.Active)
                  return;
                AppState.GetConnection().SendQrReceived(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId), FunXMPP.FMessage.Status.Error);
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
              }
            }));
            if (!AppState.IsBackgroundAgent && errorMsg != null)
              Deployment.Current.Dispatcher.BeginInvoke((Action) (() => AppState.ClientInstance.ShowMessageBox(errorMsg)));
            observer.OnNext(new Unit());
          }
          catch (Exception ex1)
          {
            observer.OnError(ex1);
          }
          finally
          {
            observer.OnCompleted();
          }
          return (Action) (() => observer.OnCompleted());
        })) : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          observer.OnError(ex);
          observer.OnCompleted();
          return (Action) (() => observer.OnCompleted());
        }));
      })).ObserveUntilLeavingFg<Unit>().Do<Unit>((Action<Unit>) (_ => fsEvent.SaveEvent()), (Action<Exception>) (ex => fsEvent.SaveEvent()));
    }

    public static void UpdateHash(Message msg, byte[] generatedHash)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.MediaHash = generatedHash;
        msg.ClearCipherMediaHash(db);
        db.SubmitChanges();
      }));
    }

    public static bool HashMediaFile(
      Stream stream,
      AxolotlMediaCipher mediaCipher,
      out byte[] generatedHash,
      out byte[] cipherMediaHash,
      out long cipherFileLength,
      out long fileLength)
    {
      generatedHash = (byte[]) null;
      cipherMediaHash = (byte[]) null;
      cipherFileLength = 0L;
      fileLength = 0L;
      stream.Position = 0L;
      long length1 = stream.Length;
      fileLength = length1;
      mediaCipher.EnsureCrypto();
      int inputBlockSize = mediaCipher.InputBlockSize;
      byte[] numArray1 = new byte[inputBlockSize];
      byte[] cipherBlock = new byte[inputBlockSize];
      while (length1 >= (long) inputBlockSize)
      {
        int num1 = stream.Read(numArray1, 0, (int) Math.Min(length1, (long) numArray1.Length));
        int num2 = mediaCipher.EncryptMedia(numArray1, cipherBlock);
        if (num1 != num2 || num1 != inputBlockSize)
          throw new IOException("Unexpected value " + (object) num1 + " " + (object) num2);
        length1 -= (long) num1;
        cipherFileLength += (long) num2;
      }
      byte[] numArray2;
      if (length1 > 0L)
      {
        int length2 = stream.Read(numArray1, 0, (int) Math.Min(length1, (long) numArray1.Length));
        numArray2 = mediaCipher.EncryptMediaFinal(numArray1, length2);
      }
      else
        numArray2 = mediaCipher.EncryptMediaFinal(numArray1, 0);
      cipherFileLength += (long) numArray2.Length;
      cipherMediaHash = mediaCipher.CipherMediaHash;
      generatedHash = mediaCipher.PlaintextHash;
      stream.Position = 0L;
      mediaCipher.ResetToInitialState();
      return true;
    }

    private static IObservable<MediaUploadMms4.Mms4UploadResult> SendEncryptedUploadRequest(
      string resumeUrl,
      byte[] cipherhash,
      FunXMPP.FMessage.FunMediaType funType,
      bool streaming,
      bool optimistic,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      long routeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
      Action<Mms4HostSelector.Mms4HostSelection> onNext = (Action<Mms4HostSelector.Mms4HostSelection>) (route =>
      {
        if (fsEvent == null)
          return;
        fsEvent.routeSelectionDelayT = new long?((DateTime.UtcNow.Ticks - routeStartTimeUtcTicks) / 10000L);
      });
      return resumeUrl != null ? Mms4HostSelector.GetInstance().GetSelectedHostObservable(false, funType, false).Do<Mms4HostSelector.Mms4HostSelection>(onNext).SelectMany<Mms4HostSelector.Mms4HostSelection, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>((Func<Mms4HostSelector.Mms4HostSelection, IObservable<MediaUploadMms4.Mms4UploadResult>>) (hostSelection => MediaUploadMms4.SendResumeRequest(Mms4Helper.CreateUrlStringForUpload(hostSelection.HostName, funType, hostSelection.AuthToken, cipherhash, streaming, optimistic, false), (string) null, fsEvent)), (Func<Mms4HostSelector.Mms4HostSelection, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>) ((hostSelection, obs) => obs)) : Mms4HostSelector.GetInstance().GetSelectedHostObservable(false, funType, false).Do<Mms4HostSelector.Mms4HostSelection>(onNext).Select<Mms4HostSelector.Mms4HostSelection, MediaUploadMms4.Mms4UploadResult>((Func<Mms4HostSelector.Mms4HostSelection, MediaUploadMms4.Mms4UploadResult>) (hostSelection => new MediaUploadMms4.Mms4UploadResult()
      {
        ResumeFrom = -1L,
        UploadUrl = Mms4Helper.CreateUrlStringForUpload(hostSelection.HostName, funType, hostSelection.AuthToken, cipherhash, streaming, optimistic, false),
        IpHint = (string) null
      }));
    }

    public static IObservable<MediaUploadMms4.Mms4UploadResult> SendResumeRequest(
      string mediaCiperResumeUrl,
      string ipHint,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      string resumeUrl = mediaCiperResumeUrl;
      FieldStats.SetHostDetailsInUploadEvent(fsEvent, resumeUrl);
      IObservable<MediaUploadMms4.Mms4UploadResult> source = Observable.Create<MediaUploadMms4.Mms4UploadResult>((Func<IObserver<MediaUploadMms4.Mms4UploadResult>, Action>) (observer =>
      {
        object releaseLock = new object();
        IDisposable uploadSub = (IDisposable) null;
        Action releaseCore = (Action) (() =>
        {
          if (uploadSub != null)
          {
            uploadSub.Dispose();
            uploadSub = (IDisposable) null;
          }
          observer.OnCompleted();
        });
        Action release = (Action) (() =>
        {
          lock (releaseLock)
          {
            if (releaseCore == null)
              return;
            releaseCore();
            releaseCore = (Action) null;
          }
        });
        Action<Exception> onError = (Action<Exception>) (e =>
        {
          try
          {
            observer.OnError(e);
          }
          finally
          {
            release();
          }
        });
        try
        {
          uploadSub = MediaUploadMms4.PostToServer(resumeUrl + "&resume=1", ipHint, (Stream) null, 0L, (AxolotlMediaCipher) null, (string) null, false).ObserveOn<MediaUploadMms4.Mms4UploadResponse>((IScheduler) AppState.Worker).Subscribe<MediaUploadMms4.Mms4UploadResponse>((Action<MediaUploadMms4.Mms4UploadResponse>) (args =>
          {
            if (args.ResponseCode == -1)
              return;
            if (fsEvent != null)
              fsEvent.httpResponseCode = new long?((long) args.ResponseCode);
            using (Stream result = args.Result)
            {
              try
              {
                MediaUploadMms4.Mms4JsonResult mms4Result = MediaUploadMms4.ParseMms4Result(result);
                if (mms4Result == null)
                {
                  observer.OnError((Exception) new MediaUploadException("Mms4 Media upload resume response was null", statusCode: args.ResponseCode));
                }
                else
                {
                  if (fsEvent != null)
                  {
                    if (args.ConnectTimeMs > 0L)
                      fsEvent.connectT = new long?(args.ConnectTimeMs);
                    if (args.NetworkTimeMs > 0L)
                      fsEvent.resumeCheckT = new long?(args.NetworkTimeMs);
                  }
                  if (mms4Result.ResumeString != null && mms4Result.ResumeString != "complete")
                  {
                    observer.OnNext(new MediaUploadMms4.Mms4UploadResult()
                    {
                      ResumeFrom = mms4Result.Resume.Value,
                      UploadUrl = mediaCiperResumeUrl,
                      DownloadDirectPath = mms4Result.DirectPath
                    });
                  }
                  else
                  {
                    FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.OK);
                    observer.OnNext(new MediaUploadMms4.Mms4UploadResult()
                    {
                      DownloadUrl = mms4Result.Url,
                      DownloadDirectPath = mms4Result.DirectPath
                    });
                  }
                }
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
              }
              finally
              {
                release();
              }
            }
          }), onError);
        }
        catch (Exception ex)
        {
          onError(ex);
        }
        return release;
      }));
      return source.Catch<MediaUploadMms4.Mms4UploadResult, Exception>((Func<Exception, IObservable<MediaUploadMms4.Mms4UploadResult>>) (ex =>
      {
        if (!(ex is MediaUploadException mediaUploadException2) || mediaUploadException2.ResponseCode >= 1)
          return Observable.Create<MediaUploadMms4.Mms4UploadResult>((Func<IObserver<MediaUploadMms4.Mms4UploadResult>, Action>) (observer =>
          {
            observer.OnError(ex);
            observer.OnCompleted();
            return (Action) (() => { });
          }));
        resumeUrl = MediaDownload.ReplaceUrlHostname(resumeUrl, "mms.whatsapp.net");
        return source;
      }));
    }

    private static void CopyUploadResponseFields(
      Message msg,
      AxolotlMediaCipher mediaCipher,
      MediaUploadMms4.Mms4UploadResult resp,
      MessagesContext db)
    {
      msg.MediaName = resp.MediaName;
      byte[] plaintextHash = mediaCipher.PlaintextHash;
      if (plaintextHash != null && !plaintextHash.IsEqualBytes(msg.MediaHash))
      {
        Log.WriteLineDebug("WARNING! Plaintext hash changed during upload? {0} -> {1} - using the computed version", msg.MediaHash == null ? (object) "(null)" : (object) Convert.ToBase64String(msg.MediaHash), (object) Convert.ToBase64String(plaintextHash));
        msg.MediaHash = plaintextHash;
      }
      msg.MediaUrl = resp.DownloadUrl;
      byte[] cipherMediaHash = mediaCipher.CipherMediaHash;
      if (cipherMediaHash != null)
      {
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureCommonProperties.CipherMediaHash = cipherMediaHash;
        forMessage.Save();
      }
      if (!string.IsNullOrEmpty(resp.DownloadDirectPath))
      {
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureMediaProperties.MediaDirectPath = resp.DownloadDirectPath;
        forMessage.Save();
      }
      byte[] sidecar = mediaCipher.Sidecar;
      if (sidecar == null)
        return;
      MessageProperties forMessage1 = MessageProperties.GetForMessage(msg);
      forMessage1.EnsureMediaProperties.Sidecar = sidecar;
      forMessage1.Save();
    }

    internal static void ProcessUploadResponse(
      Message m,
      AxolotlMediaCipher mediaCipher,
      MediaUploadMms4.Mms4UploadResult resp,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      if (m.UploadContext.isOptimisticUpload())
      {
        OptimisticJpegUploadContextMms4 uploadContext = (OptimisticJpegUploadContextMms4) m.UploadContext;
        uploadContext.uploadResultMms4 = resp;
        Log.d("OPU", "media send: optimistic upload | OuId={0}", (object) uploadContext.OuId);
        m.MediaName = resp.MediaName;
        byte[] plaintextHash = mediaCipher.PlaintextHash;
        if (plaintextHash != null && !plaintextHash.IsEqualBytes(m.MediaHash))
        {
          Log.WriteLineDebug("WARNING! Plaintext hash changed during upload? {0} -> {1} - using the computed version", m.MediaHash == null ? (object) "(null)" : (object) Convert.ToBase64String(m.MediaHash), (object) Convert.ToBase64String(plaintextHash));
          m.MediaHash = plaintextHash;
        }
        m.MediaUrl = resp.DownloadUrl;
        uploadContext.MediaCipher = mediaCipher;
        uploadContext.MediaCipherHash = mediaCipher.CipherMediaHash;
        uploadContext.UploadedFlag = true;
      }
      else
      {
        bool skipSend = false;
        Message msg = (Message) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          msg = db.GetMessage(m.KeyRemoteJid, m.KeyId, m.KeyFromMe);
          if (msg == null)
            skipSend = true;
          else if (msg.IsDeliveredToServer())
          {
            skipSend = true;
          }
          else
          {
            if (msg.Status == FunXMPP.FMessage.Status.Canceled)
              skipSend = true;
            else
              msg.Status = FunXMPP.FMessage.Status.Unsent;
            MediaUploadMms4.CopyUploadResponseFields(msg, mediaCipher, resp, db);
            db.SubmitChanges();
            Log.l("mms4 upload", "media upload: complete | download_url={0}", (object) MediaDownload.RedactUrl(resp.DownloadUrl));
          }
        }));
        if (skipSend)
        {
          if (msg == null)
            Log.l("mms4 upload", "msg deleted before upload complete | id={0}", (object) m.MessageID);
          else
            Log.l("mms4 upload", "skip sending msg after upload complete | id={0}", (object) msg.MessageID);
        }
        else
        {
          Log.l("mms4 upload", "send msg after upload complete | id={0}", (object) msg.MessageID);
          if (fsEvent != null)
          {
            DateTime? funTimestamp = msg.FunTimestamp;
            if (funTimestamp.HasValue)
            {
              WhatsApp.Events.MediaUpload mediaUpload = fsEvent;
              long unixTime1 = DateTime.Now.ToUnixTime();
              funTimestamp = msg.FunTimestamp;
              long unixTime2 = funTimestamp.Value.ToUnixTime();
              long? nullable = new long?((unixTime1 - unixTime2) * 1000L);
              mediaUpload.userVisibleT = nullable;
            }
          }
          AppState.SendMessage(AppState.ClientInstance.GetConnection(), msg);
          AppState.QrPersistentAction.NotifyMessage(msg, QrMessageForwardType.Update);
          if (!msg.IsPtt())
            return;
          msg.MoveMediaFromIsoStoreToAlbum();
        }
      }
    }

    public static IObservable<MediaUploadMms4.Mms4UploadResult> GetUploadObservableForResendMaybeMms4(
      Message msg,
      string participant,
      bool fromWeb,
      int? attempts = null)
    {
      return (msg == null || !msg.KeyFromMe ? (Mms4Helper.IsMms4DownloadMessage(msg) ? 1 : 0) : (Mms4Helper.IsMms4UploadMessage(msg) ? 1 : 0)) != 0 ? MediaUploadMms4.GetUploadObservableForResend(msg, participant, fromWeb, attempts) : (IObservable<MediaUploadMms4.Mms4UploadResult>) MediaUpload.GetUploadObservableForResend(msg, participant, fromWeb, attempts);
    }

    public static IObservable<MediaUploadMms4.Mms4UploadResult> GetUploadObservableForResend(
      Message msg,
      string participant,
      bool fromWeb,
      int? attempts = null)
    {
      WhatsApp.Events.MediaUpload fsEvent = FieldStats.GetFsMediaUploadEvent(msg);
      fsEvent.mmsVersion = new long?(4L);
      WhatsApp.Events.MediaUpload mediaUpload = fsEvent;
      int? nullable1 = attempts;
      long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
      mediaUpload.retryCount = nullable2;
      string altName = (string) null;
      string localUri = (string) null;
      byte[] hash = (byte[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => localUri = MediaUpload.GetAndSanitizeFilePath(db, msg, ref altName)));
      if (localUri != null)
      {
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(localUri))
          {
            using (Stream stream = mediaStorage.OpenFile(localUri))
            {
              long length = stream.Length;
              hash = MediaUpload.ComputeHash(stream);
            }
          }
        }
        catch (Exception ex)
        {
          localUri = (string) null;
        }
      }
      if (localUri == null)
        return (IObservable<MediaUploadMms4.Mms4UploadResult>) null;
      msg.NotifyTransfer = false;
      Axolotl encryption = AppState.GetConnection().Encryption;
      AxolotlMediaCipher mediaCipher = AxolotlMediaCipher.CreateUploadCipher(msg, encryption, fromWeb);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => hash = msg.GetCipherMediaHash(db)));
      if (hash == null)
        hash = mediaCipher.UploadHash;
      IObservable<MediaUploadMms4.Mms4UploadResult> source;
      if (fromWeb || mediaCipher.Participants != null && mediaCipher.Participants.Contains<string>(participant))
        source = MediaUploadMms4.SendEncryptedUploadRequest((string) null, hash, msg.GetFunMediaType(), false, false, fsEvent);
      else
        source = Observable.Return<MediaUploadMms4.Mms4UploadResult>(new MediaUploadMms4.Mms4UploadResult()
        {
          SkipUpload = true
        });
      return source.ObserveOn<MediaUploadMms4.Mms4UploadResult>((IScheduler) AppState.Worker).SelectMany<MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>((Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>) (uploadRequestResponse => Observable.If<MediaUploadMms4.Mms4UploadResult>((Func<bool>) (() => uploadRequestResponse.SkipUpload || uploadRequestResponse.UploadUrl == null), Observable.Return<MediaUploadMms4.Mms4UploadResult>(uploadRequestResponse), Observable.Defer<MediaUploadMms4.Mms4UploadResult>((Func<IObservable<MediaUploadMms4.Mms4UploadResult>>) (() =>
      {
        Message msg1 = msg;
        string localUri1 = localUri;
        string basename = altName;
        string uploadUrl = uploadRequestResponse.UploadUrl;
        string ipHint1 = uploadRequestResponse.IpHint;
        long resumeFrom = uploadRequestResponse.ResumeFrom;
        AxolotlMediaCipher mediaCipher1 = mediaCipher;
        WhatsApp.Events.MediaUpload fsEvent1 = fsEvent;
        string ipHint2 = ipHint1;
        int num = fromWeb ? 1 : 0;
        return MediaUploadMms4.GetUploadObservableOnDisk(msg1, localUri1, basename, uploadUrl, resumeFrom, mediaCipher1, fsEvent1, false, ipHint2, num != 0);
      })).ObserveOn<MediaUploadMms4.Mms4UploadResult>((IScheduler) AppState.Worker))), (Func<MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>) ((uploadRequestResponse, uploadResult) => uploadResult)).Where<MediaUploadMms4.Mms4UploadResult>((Func<MediaUploadMms4.Mms4UploadResult, bool>) (res =>
      {
        if (!res.SkipUpload)
        {
          try
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              MediaUploadMms4.CopyUploadResponseFields(msg, mediaCipher, res, db);
              db.SubmitChanges();
            }));
          }
          catch (DatabaseInvalidatedException ex)
          {
            return false;
          }
        }
        return true;
      })).Do<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (_ => fsEvent.SaveEvent()), (Action<Exception>) (ex => fsEvent.SaveEvent()));
    }

    private static IObservable<MediaUploadMms4.Mms4UploadResult> GetUploadObservableOnDisk(
      Message msg,
      string localUri,
      string basename,
      string postUrl,
      long resumeAt,
      AxolotlMediaCipher mediaCipher,
      WhatsApp.Events.MediaUpload fsEvent,
      bool progress = true,
      string ipHint = null,
      bool webRetry = false)
    {
      IMediaStorage fs = (IMediaStorage) null;
      Stream stream = (Stream) null;
      if (msg.UploadContext.isOptimisticUpload())
      {
        fs = (IMediaStorage) null;
        OptimisticJpegUploadContext uploadContext = (OptimisticJpegUploadContext) msg.UploadContext;
        stream = uploadContext.JpegStream;
        uploadContext.JpegStream = (Stream) null;
        stream.Position = 0L;
      }
      else
      {
        fs = MediaStorage.Create(localUri);
        stream = fs.OpenFile(localUri);
      }
      if (stream.Length < resumeAt)
      {
        Log.l("mms4 upload", "Resetting resume point since server value {0} exceeds input size {1}", (object) resumeAt, (object) stream.Length);
        resumeAt = -1L;
      }
      Log.l("mms4 upload", "media upload: starting upload from offset {0} | msg_id={1} {2} {3}", (object) resumeAt, (object) msg.MessageID, (object) msg.UploadContext.isOptimisticUpload(), (object) stream.Length);
      bool flag = true;
      mediaCipher.MediaResumeUrl = postUrl;
      if (resumeAt >= 0L && fsEvent != null)
        fsEvent.uploadResumePoint = new long?((long) (int) resumeAt);
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("Content-Type", "application/octet-stream");
      if (resumeAt > 0L)
        dictionary.Add("Content-Range", string.Format("bytes {0}-*/*", (object) resumeAt));
      if (flag)
      {
        dictionary.Add("Transfer-Encoding", "chunked");
      }
      else
      {
        long num = (stream.Length / 16L + 1L) * 16L + 10L - Math.Max(0L, resumeAt);
        dictionary.Add("Content-Length", string.Format("{0}", (object) num));
      }
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append("\r\n");
        stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
        {
          keyValuePair.Value
        })));
      }
      string str1 = stringBuilder.ToString();
      Action<long, long> action = (Action<long, long>) null;
      if (msg.NotifyTransfer)
      {
        msg.TransferValue = Math.Min(0.5, msg.TransferValue);
        double start = msg.TransferValue;
        action = (Action<long, long>) ((current, total) => msg.TransferValue = Math.Max(start + (total == 0L ? 0.0 : (double) current / (double) total) * (1.0 - start), msg.TransferValue));
      }
      string postUrl1 = postUrl;
      string str2 = ipHint;
      string keyRemoteJid = msg.KeyRemoteJid;
      string logPrefix = string.Format("id={0} type={1}", (object) msg.MessageID, (object) msg.MediaWaType);
      Stream stream1 = stream;
      long resumeAt1 = resumeAt;
      AxolotlMediaCipher mediaCipher1 = mediaCipher;
      string headers = str1;
      int num1 = flag ? 1 : 0;
      WhatsApp.Events.MediaUpload fsEvent1 = fsEvent;
      Action releaseFileSource = (Action) (() =>
      {
        stream.Dispose();
        fs.SafeDispose();
      });
      Action<long, long> onProgress = action;
      string ipHint1 = str2;
      return MediaUploadMms4.GetUploadObservable(postUrl1, keyRemoteJid, logPrefix, stream1, resumeAt1, (IObservable<byte[]>) null, mediaCipher1, headers, num1 != 0, fsEvent1, releaseFileSource, onProgress, ipHint1);
    }

    public static void StreamMedia(
      StreamingUploadContextMms4 context,
      string remoteJid,
      FunXMPP.FMessage.FunMediaType mediaType,
      string contentType,
      string extension)
    {
      try
      {
        FunXMPP.Connection connection = AppState.ClientInstance.GetConnection();
        Message message = new Message()
        {
          UploadContext = (UploadContext) context,
          KeyRemoteJid = remoteJid,
          MediaWaType = FunXMPP.FMessage.TypeFromFunMediaType(mediaType),
          MediaMimeType = contentType
        };
        AxolotlMediaCipher mediaCipher = AxolotlMediaCipher.CreateUploadCipher(message, connection.Encryption, false);
        if (mediaCipher == null)
          return;
        context.Hash = mediaCipher.UploadHash;
        context.MediaKey = message.MediaKey;
        context.ParticipantHash = message.ParticipantsHash;
        context.MediaCipher = mediaCipher;
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        dictionary.Add("Content-Type", "application/octet-stream");
        dictionary.Add("Transfer-Encoding", "chunked");
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append("\r\n");
          stringBuilder.Append(string.Format("{0}: {1}", (object) keyValuePair.Key, (object) string.Join(" ", new string[1]
          {
            keyValuePair.Value
          })));
        }
        string headers = stringBuilder.ToString();
        context.TransferSubscription = MediaUploadMms4.SendEncryptedUploadRequest((string) null, context.Hash, mediaType, true, false, (WhatsApp.Events.MediaUpload) null).SelectMany<MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>((Func<MediaUploadMms4.Mms4UploadResult, IObservable<MediaUploadMms4.Mms4UploadResult>>) (res =>
        {
          string uploadUrl = res.UploadUrl;
          string ipHint1 = res.IpHint;
          string remoteJid1 = remoteJid;
          IObservable<byte[]> streamingBytes = context.AsObservable();
          AxolotlMediaCipher mediaCipher1 = mediaCipher;
          string headers1 = headers;
          string ipHint2 = ipHint1;
          return MediaUploadMms4.GetUploadObservable(uploadUrl, remoteJid1, "(streaming)", (Stream) null, 0L, streamingBytes, mediaCipher1, headers1, true, ipHint: ipHint2);
        }), (Func<MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult, MediaUploadMms4.Mms4UploadResult>) ((res, uploadRes) => uploadRes)).ObserveUntilLeavingFg<MediaUploadMms4.Mms4UploadResult>().Subscribe<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (resp => context.Mms4UploadResult = new MediaUploadMms4.Mms4UploadResult()
        {
          DownloadUrl = resp.DownloadUrl,
          DownloadDirectPath = resp.DownloadDirectPath,
          IpHint = resp.IpHint
        }), (Action<Exception>) (ex => context.OnUploadError(ex)), (Action) (() =>
        {
          Log.d("mms4 upload", "Disposing of TransferSubscription");
          context.TransferSubscription.SafeDispose();
          context.TransferSubscription = (IDisposable) null;
        }));
      }
      catch (Exception ex)
      {
        context.OnUploadError(ex);
      }
    }

    private static IObservable<MediaUploadMms4.Mms4UploadResult> GetUploadObservable(
      string postUrl,
      string remoteJid,
      string logPrefix,
      Stream stream,
      long resumeAt,
      IObservable<byte[]> streamingBytes,
      AxolotlMediaCipher mediaCipher,
      string headers,
      bool chunked,
      WhatsApp.Events.MediaUpload fsEvent = null,
      Action releaseFileSource = null,
      Action<long, long> onProgress = null,
      string ipHint = null)
    {
      return Observable.Create<MediaUploadMms4.Mms4UploadResult>((Func<IObserver<MediaUploadMms4.Mms4UploadResult>, Action>) (observer =>
      {
        object releaseLock = new object();
        IDisposable lockScreenSub = (IDisposable) null;
        IDisposable uploadSub = (IDisposable) null;
        Action releaseTransferProgress = (Action) null;
        Action releaseCore = (Action) (() =>
        {
          if (lockScreenSub != null)
          {
            lockScreenSub.Dispose();
            lockScreenSub = (IDisposable) null;
          }
          if (releaseFileSource != null)
          {
            releaseFileSource();
            releaseFileSource = (Action) null;
          }
          if (releaseTransferProgress != null)
          {
            releaseTransferProgress();
            releaseTransferProgress = (Action) null;
          }
          if (uploadSub != null)
          {
            uploadSub.Dispose();
            uploadSub = (IDisposable) null;
          }
          observer.OnCompleted();
        });
        Action release = (Action) (() =>
        {
          Log.l("mms4 upload", "media upload: release | {0}", (object) logPrefix);
          lock (releaseLock)
          {
            if (releaseCore == null)
              return;
            releaseCore();
            releaseCore = (Action) null;
          }
        });
        Stopwatch timer = new Stopwatch();
        Action<Exception> onError = (Action<Exception>) (e =>
        {
          timer.Stop();
          if (fsEvent != null)
            fsEvent.mediaUploadT = new long?(timer.ElapsedMilliseconds);
          try
          {
            observer.OnError(e);
          }
          finally
          {
            release();
          }
        });
        try
        {
          lockScreenSub = AppState.ClientInstance.LockScreenSubscription();
          Log.l("mms4 upload", "media upload: start | {0} posturl={1}", (object) logPrefix, (object) postUrl);
          FieldStats.SetHostDetailsInUploadEvent(fsEvent, postUrl);
          IObservable<MediaUploadMms4.Mms4UploadResponse> source = streamingBytes == null ? MediaUploadMms4.PostToServer(postUrl, ipHint, stream, resumeAt, mediaCipher, headers, chunked, onProgress != null) : MediaUploadMms4.PostToServerStreaming(postUrl, ipHint, streamingBytes, mediaCipher, headers, chunked, onProgress != null);
          timer.Start();
          FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_UPLOAD);
          uploadSub = source.ObserveOn<MediaUploadMms4.Mms4UploadResponse>((IScheduler) AppState.Worker).Subscribe<MediaUploadMms4.Mms4UploadResponse>((Action<MediaUploadMms4.Mms4UploadResponse>) (args =>
          {
            if (onProgress != null)
              onProgress(args.CurrentProgress, args.TotalProgress);
            if (args.ResponseCode == -1)
              return;
            timer.Stop();
            if (fsEvent != null)
            {
              fsEvent.mediaUploadT = new long?(timer.ElapsedMilliseconds);
              fsEvent.httpResponseCode = new long?((long) args.ResponseCode);
            }
            if (streamingBytes != null)
            {
              Log.l("mms4 upload", "Release of streaming upload for finalize");
              uploadSub.SafeDispose();
              uploadSub = (IDisposable) null;
              if (args.ResponseCode == 200)
                uploadSub = MediaUploadMms4.PostToServerStreamingFinalize(postUrl + "&final_hash=" + Mms4Helper.ConvertBytesToUrlParm(mediaCipher.CipherMediaHash), ipHint).Subscribe<MemoryStream>((Action<MemoryStream>) (resp =>
                {
                  try
                  {
                    MediaUploadMms4.ProcessMms4Response(observer, new MediaUploadMms4.Mms4UploadResponse()
                    {
                      Result = (Stream) resp,
                      ResponseCode = 200
                    }, logPrefix, fsEvent);
                  }
                  catch (Exception ex)
                  {
                    observer.OnError(ex);
                  }
                  finally
                  {
                    release();
                  }
                }), (Action<Exception>) (ex => observer.OnError(ex)));
              else
                observer.OnError((Exception) new MediaUploadException("Exception on upload", statusCode: args.ResponseCode));
            }
            else
            {
              try
              {
                MediaUploadMms4.ProcessMms4Response(observer, args, logPrefix, fsEvent);
              }
              catch (Exception ex)
              {
                observer.OnError(ex);
              }
              finally
              {
                release();
              }
            }
          }), onError);
        }
        catch (Exception ex)
        {
          onError(ex);
        }
        return release;
      }));
    }

    private static void ProcessMms4Response(
      IObserver<MediaUploadMms4.Mms4UploadResult> observer,
      MediaUploadMms4.Mms4UploadResponse args,
      string logPrefix,
      WhatsApp.Events.MediaUpload fsEvent = null)
    {
      if (args.Result == null)
      {
        observer.OnError((Exception) new MediaUploadException("Mms4 Media upload response was null", statusCode: args.ResponseCode));
      }
      else
      {
        using (Stream result = args.Result)
        {
          MediaUploadMms4.Mms4JsonResult mms4Result = MediaUploadMms4.ParseMms4Result(result);
          if (mms4Result == null)
          {
            observer.OnError((Exception) new MediaUploadException("Mms4 Media upload response was null", statusCode: args.ResponseCode));
          }
          else
          {
            try
            {
              Uri uri = new Uri(mms4Result.Url);
            }
            catch (Exception ex)
            {
              Log.l("mms4 upload", "media upload: failed to parse download uri {0}: {1} {2}", (object) MediaDownload.RedactUrl(mms4Result.Url), (object) ex.GetType().Name, (object) (ex.Message ?? ""));
              observer.OnError((Exception) new MediaUploadException("Failed to parse download URI", ex, args.ResponseCode));
              return;
            }
            Log.l("mms4 upload", "media upload:  completed | {0}", (object) logPrefix);
            if (fsEvent != null)
            {
              FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.OK);
              if (args.NetworkTimeMs > 0L)
                fsEvent.networkUploadT = new long?(args.NetworkTimeMs);
              if (args.ConnectTimeMs > 0L)
                fsEvent.connectT = new long?(args.ConnectTimeMs);
            }
            observer.OnNext(new MediaUploadMms4.Mms4UploadResult()
            {
              DownloadUrl = mms4Result.Url,
              DownloadDirectPath = mms4Result.DirectPath
            });
          }
        }
      }
    }

    public static MediaUploadMms4.Mms4JsonResult ParseMms4Result(Stream resp)
    {
      try
      {
        if (resp != null)
        {
          Log.d("mms4 upload", new StreamReader(resp).ReadToEnd());
          resp.Position = 0L;
        }
      }
      catch (Exception ex)
      {
      }
      try
      {
        return new DataContractJsonSerializer(typeof (MediaUploadMms4.Mms4JsonResult)).ReadObject(resp) as MediaUploadMms4.Mms4JsonResult;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "mms4 upload exception parsing json");
      }
      return (MediaUploadMms4.Mms4JsonResult) null;
    }

    private static IObservable<MediaUploadMms4.Mms4UploadResponse> PostToServer(
      string url,
      string ipHint,
      Stream stream,
      long resumeAt,
      AxolotlMediaCipher mediaCipher,
      string headers,
      bool chunked,
      bool progress = false)
    {
      return NativeWeb.Create<MediaUploadMms4.Mms4UploadResponse>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<MediaUploadMms4.Mms4UploadResponse>>) ((req, observer) =>
      {
        if (ipHint != null)
          req.SetResolver(new IpResolver() { Ip = ipHint }.ToNativeResolver());
        MediaUploadMms4.Mms4WebCallback callbackObject = new MediaUploadMms4.Mms4WebCallback(observer, stream, resumeAt, mediaCipher, chunked, progress);
        try
        {
          req.Open(url, (IWebCallback) callbackObject, "POST", headers: headers);
        }
        finally
        {
          long bytesWritten = callbackObject.BytesWritten;
          if (bytesWritten != 0L)
            Log.l("mms4 upload", "HTTP Mms4 POST: wrote {0} bytes of post data", (object) bytesWritten);
        }
      }));
    }

    private static IObservable<MediaUploadMms4.Mms4UploadResponse> PostToServerStreaming(
      string url,
      string ipHint,
      IObservable<byte[]> streamingObs,
      AxolotlMediaCipher mediaCipher,
      string headers,
      bool chunked,
      bool progress = false)
    {
      return NativeWeb.Create<MediaUploadMms4.Mms4UploadResponse>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<MediaUploadMms4.Mms4UploadResponse>>) ((req, observer) =>
      {
        MediaUploadMms4.Mms4WebCallback callbackObject = new MediaUploadMms4.Mms4WebCallback(observer, streamingObs, mediaCipher, chunked, progress);
        if (ipHint != null)
          req.SetResolver(new IpResolver() { Ip = ipHint }.ToNativeResolver());
        try
        {
          req.Open(url, (IWebCallback) callbackObject, "POST", headers: headers);
        }
        finally
        {
          long bytesWritten = callbackObject.BytesWritten;
          if (bytesWritten != 0L)
            Log.l("mms4 upload", "HTTP streaming Mms4 POST: wrote {0} bytes of post data", (object) bytesWritten);
        }
      }));
    }

    public static IObservable<MemoryStream> PostToServerStreamingFinalize(string url, string ipHint)
    {
      return NativeWeb.Create<MemoryStream>(NativeWeb.Options.Default | NativeWeb.Options.KeepAlive, (Action<IWebRequest, IObserver<MemoryStream>>) ((req, observer) =>
      {
        MemoryStream mem = new MemoryStream();
        req.Open(url, (IWebCallback) new MediaUploadMms4.FinalizeCallback()
        {
          OnBeginResponse = (Action<int, string>) ((code, headers) =>
          {
            if (code == 200)
              return;
            observer.OnError(new Exception(string.Format("PostToServerStreamingFinalize Unexpected response {0}", (object) code)));
          }),
          OnBytesIn = (Action<byte[]>) (b => mem.Write(b, 0, b.Length)),
          OnEndResponse = (Action) (() =>
          {
            mem.Position = 0L;
            observer.OnNext(mem);
          })
        }, "POST");
      }));
    }

    public static void DeleteMedia(string url)
    {
      NativeWeb.EmptyDelete(url).Subscribe<int>((Action<int>) (code => Log.l("mms4 upload", "Delete code = {0} for url:{1}", (object) code, (object) url)), (Action<Exception>) (ex =>
      {
        Log.l("mms4 upload", "Exception deleting url:{1}", (object) url);
        Log.LogException(ex, "exception deleting media from server");
      }));
    }

    [DataContract]
    public class Mms4JsonResult
    {
      [DataMember(Name = "direct_path")]
      public string DirectPath;
      public string IpHint;

      [DataMember(Name = "url")]
      public string Url { get; set; }

      [DataMember(Name = "resume")]
      public string ResumeString { get; set; }

      public long? Resume
      {
        get => this.ResumeString != null ? new long?(long.Parse(this.ResumeString)) : new long?();
      }
    }

    public class Mms4UploadResult
    {
      public string UploadUrl;
      public string IpHint;
      public long ResumeFrom = -1;
      public string DownloadUrl;
      public string DownloadDirectPath;
      private string mediaName;
      public bool SkipUpload;

      public string MediaName
      {
        get
        {
          if (this.mediaName == null)
          {
            int num;
            if (this.DownloadUrl != null && (num = this.DownloadUrl.LastIndexOf('/')) >= 0)
              this.mediaName = this.DownloadUrl.Substring(num + 1);
          }
          return this.mediaName;
        }
        set => this.mediaName = value;
      }
    }

    public class Mms4UploadResponse
    {
      public long CurrentProgress;
      public long TotalProgress;
      public int ResponseCode = -1;
      public Stream Result;
      public long ConnectTimeMs;
      public long NetworkTimeMs;
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class Mms4WebCallback : IWebCallback
    {
      private MemoryStream response = new MemoryStream();
      private IObserver<MediaUploadMms4.Mms4UploadResponse> observer;
      internal bool showProgress;
      private bool chunked;
      private bool streaming;
      private Stream dataStream;
      private long resumeAt;
      private IObservable<byte[]> streamingObs;
      private AxolotlMediaCipher mediaCipher;
      private bool canceledUpload;
      private Action terminate = (Action) (() => { });
      private long bytesWritten;
      public long createTimeUtcTicks = DateTime.UtcNow.Ticks;
      public long writeStartTimeUtcTicks = -1;
      private IByteBuffer bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();

      public long BytesWritten => this.bytesWritten;

      public Mms4WebCallback(
        IObserver<MediaUploadMms4.Mms4UploadResponse> observer,
        Stream dataStream,
        long resumeAt,
        AxolotlMediaCipher cipher,
        bool chunked,
        bool showProgress)
      {
        this.observer = observer;
        this.dataStream = dataStream;
        this.resumeAt = resumeAt;
        this.streamingObs = (IObservable<byte[]>) null;
        this.mediaCipher = cipher;
        this.chunked = chunked;
        this.showProgress = showProgress;
        this.streaming = false;
      }

      public Mms4WebCallback(
        IObserver<MediaUploadMms4.Mms4UploadResponse> observer,
        IObservable<byte[]> streamingObs,
        AxolotlMediaCipher cipher,
        bool chunked,
        bool showProgress)
      {
        this.observer = observer;
        this.dataStream = (Stream) null;
        this.resumeAt = -1L;
        this.streamingObs = streamingObs;
        this.mediaCipher = cipher;
        this.chunked = chunked;
        this.showProgress = showProgress;
        this.streaming = true;
      }

      public void Write(IWebWriter writer)
      {
        if (this.writeStartTimeUtcTicks < 0L)
          this.writeStartTimeUtcTicks = DateTime.UtcNow.Ticks;
        long progress = 0;
        long totalLength = this.dataStream == null ? 0L : this.dataStream.Length;
        this.observer.OnNext(new MediaUploadMms4.Mms4UploadResponse()
        {
          CurrentProgress = progress,
          TotalProgress = totalLength
        });
        Action<byte[], int, int> actualWrite = (Action<byte[], int, int>) ((bytes, offset, len) =>
        {
          this.bb.Put(bytes, offset, len);
          writer.Write(this.bb);
          this.bb.Reset();
          this.bytesWritten += (long) len;
        });
        Action<byte[], int, int, int> write = (Action<byte[], int, int, int>) ((buf, offset, len, progLen) =>
        {
          if (len == 0)
            return;
          actualWrite(buf, offset, len);
          progress += (long) progLen;
          if (!this.showProgress || progLen == 0)
            return;
          this.observer.OnNext(new MediaUploadMms4.Mms4UploadResponse()
          {
            CurrentProgress = progress,
            TotalProgress = totalLength
          });
        });
        Action flushChunkBuffer = (Action) null;
        if (this.chunked)
          write = this.WriteToChunkBuffer(write, out flushChunkBuffer);
        Action flush = (Action) null;
        Action<Stream, long> seek;
        MultiPartUploader.FormDataCryptoWrapper.Create(this.mediaCipher, ref write, out seek, out flush);
        if (this.streaming)
          this.StreamingWrite(write, flush);
        else if (this.dataStream != null && this.dataStream.Length > 0L)
        {
          using (this.dataStream)
          {
            this.dataStream.Position = 0L;
            long length = this.dataStream.Length;
            byte[] buffer = new byte[4096];
            if (seek != null && this.resumeAt > 0L)
              seek((Stream) null, this.resumeAt);
            while (length >= 1L)
            {
              int num = this.dataStream.Read(buffer, 0, (int) Math.Min(length, (long) buffer.Length));
              length -= (long) num;
              write(buffer, 0, num, num);
            }
          }
        }
        if (flush != null)
          flush();
        if (flushChunkBuffer != null)
          flushChunkBuffer();
        if (this.chunked)
        {
          byte[] bytes = Encoding.UTF8.GetBytes("0\r\n\r\n");
          actualWrite(bytes, 0, bytes.Length);
        }
        if (this.canceledUpload)
          throw new OperationCanceledException();
      }

      private void StreamingWrite(Action<byte[], int, int, int> write, Action flush)
      {
        ManualResetEvent ev = new ManualResetEvent(false);
        Action error = (Action) null;
        IDisposable disp = (IDisposable) null;
        object @lock = new object();
        Action setUnlocked = (Action) (() =>
        {
          if (disp != null)
          {
            disp.Dispose();
            disp = (IDisposable) null;
          }
          ev?.Set();
        });
        Action set = (Action) (() =>
        {
          lock (@lock)
            setUnlocked();
        });
        WorkQueue workQueue = new WorkQueue();
        disp = this.streamingObs.ObserveOn<byte[]>((IScheduler) workQueue).Subscribe<byte[]>((Action<byte[]>) (payload =>
        {
          try
          {
            write(payload, 0, payload.Length, 0);
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "write async");
            error = ex.GetRethrowAction();
            set();
          }
        }), (Action<Exception>) (ex =>
        {
          Log.LogException(ex, "write async - observable OnError");
          if (ex is OperationCanceledException)
            this.canceledUpload = true;
          else
            error = ex.GetRethrowAction();
          set();
        }), set);
        this.terminate = (Action) (() =>
        {
          lock (@lock)
          {
            this.canceledUpload = true;
            setUnlocked();
          }
        });
        ev.WaitOne();
        lock (@lock)
        {
          ev.Dispose();
          ev = (ManualResetEvent) null;
        }
        workQueue.Stop();
        this.terminate = (Action) (() => { });
      }

      public int ResponseCode { get; private set; } = -1;

      public void OnResponseCode(int code)
      {
        this.ResponseCode = code;
        if (code == 200 || code == 206)
          return;
        Log.d("mms4 upload", "Error response code {0}", (object) code);
        Mms4RouteSelector.GetInstance().OnMediaTransferErrorOrResponseCode(code);
      }

      public void OnHeaders(string headers)
      {
      }

      public void ResponseBytesIn(IByteBuffer buf)
      {
        byte[] buffer = buf.Get();
        buf = (IByteBuffer) null;
        this.response.Write(buffer, 0, buffer.Length);
      }

      public void EndResponse()
      {
        this.response.Position = 0L;
        long num1 = -1;
        long num2 = -1;
        if (this.writeStartTimeUtcTicks > 0L)
        {
          num2 = (this.writeStartTimeUtcTicks - this.createTimeUtcTicks) / 10000L;
          num1 = (DateTime.UtcNow.Ticks - this.createTimeUtcTicks) / 10000L;
        }
        this.observer.OnNext(new MediaUploadMms4.Mms4UploadResponse()
        {
          CurrentProgress = 1L,
          TotalProgress = 1L,
          Result = (Stream) this.response,
          ResponseCode = this.ResponseCode,
          NetworkTimeMs = num1,
          ConnectTimeMs = num2
        });
      }

      public void Terminate() => this.terminate();

      private Action<byte[], int, int, int> WriteToChunkBuffer(
        Action<byte[], int, int, int> write,
        out Action flushChunkBuffer)
      {
        byte[] buffer = new byte[4096];
        int currentLength = 0;
        int progress = 0;
        Action flush = (Action) (() =>
        {
          this.WriteChunk(write, buffer, 0, currentLength, progress);
          currentLength = 0;
          progress = 0;
        });
        flushChunkBuffer = flush;
        return (Action<byte[], int, int, int>) ((buf, offset, length, progLen) =>
        {
          progress += progLen;
          while (length != 0)
          {
            int length1 = Math.Min(length, buffer.Length - currentLength);
            if (length1 != 0)
            {
              Array.Copy((Array) buf, offset, (Array) buffer, currentLength, length1);
              currentLength += length1;
              offset += length1;
              length -= length1;
            }
            if (currentLength == buffer.Length)
              flush();
          }
        });
      }

      private void WriteChunk(
        Action<byte[], int, int, int> write,
        byte[] buf,
        int offset,
        int length,
        int progLen)
      {
        if (length == 0)
          return;
        byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}\r\n", (object) length.ToString("x")));
        write(bytes, 0, bytes.Length, 0);
        write(buf, offset, length, progLen);
        write(bytes, bytes.Length - 2, 2, 0);
      }
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class FinalizeCallback : IWebCallback
    {
      public Action<Action<byte[], int, int>> OnWrite;
      public Action<byte[]> OnBytesIn;
      public Action OnEndResponse;
      public Action<int, string> OnBeginResponse;
      private int? ResponseCode;
      private string Headers;
      public long createTimeUtcTicks = DateTime.UtcNow.Ticks;
      public long writeStartTimeUtcTicks = -1;

      public void OnResponseCode(int code)
      {
        this.ResponseCode = new int?(code);
        this.TryOnBeginResponse();
      }

      public void OnHeaders(string headers)
      {
        this.Headers = headers;
        this.TryOnBeginResponse();
      }

      private void TryOnBeginResponse()
      {
        if (this.OnBeginResponse == null)
        {
          this.Headers = (string) null;
        }
        else
        {
          if (!this.ResponseCode.HasValue || this.Headers == null)
            return;
          this.OnBeginResponse(this.ResponseCode.Value, this.Headers);
        }
      }

      public void ResponseBytesIn(IByteBuffer bb)
      {
        Action<byte[]> onBytesIn = this.OnBytesIn;
        if (onBytesIn == null)
          return;
        onBytesIn(bb.Get());
      }

      public void EndResponse()
      {
        Action onEndResponse = this.OnEndResponse;
        if (onEndResponse == null)
          return;
        onEndResponse();
      }

      public void Write(IWebWriter writer)
      {
      }
    }
  }
}
