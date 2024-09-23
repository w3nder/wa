// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPictureStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class ChatPictureStore
  {
    private const string LogHeader = "chatpic";
    private const string DirPath = "profilePictures";
    private const int TargetPixelSizeLarge = 480;
    private const int TargetPixelSizeSmall = 128;
    private static Subject<Pair<string, int?>> ForcePicUpdateSubject = new Subject<Pair<string, int?>>();
    private static Subject<Pair<string, string>> SavedPicChangedSubject = new Subject<Pair<string, string>>();
    private static Subject<string> PicIdChangedSubject = new Subject<string>();
    private static object subsLock = new object();
    private static Dictionary<string, ChatPictureStore.SubState> subs = new Dictionary<string, ChatPictureStore.SubState>();
    private static object pendingReqsLock = new object();
    private static Dictionary<string, DateTime> pendingPicRequests = new Dictionary<string, DateTime>();
    private static object cacheLock = new object();
    private static KeyValueCache<string, ImageSource> cachedPics = (KeyValueCache<string, ImageSource>) null;

    public static IObservable<ChatPictureStore.PicState> Get(
      string jid,
      bool preferLarge,
      bool forceRequest,
      bool fallbackToContactPic,
      ChatPictureStore.SubMode subMode = ChatPictureStore.SubMode.Default)
    {
      if (jid == null)
        return Observable.Empty<ChatPictureStore.PicState>();
      switch (JidHelper.GetJidType(jid))
      {
        case JidHelper.JidTypes.Broadcast:
          return Observable.Return<ChatPictureStore.PicState>(new ChatPictureStore.PicState((BitmapSource) null, (string) null, false));
        case JidHelper.JidTypes.Psa:
          return Observable.Return<ChatPictureStore.PicState>(new ChatPictureStore.PicState((BitmapSource) null, (string) null, false));
        default:
          return Observable.Create<ChatPictureStore.PicState>((Func<IObserver<ChatPictureStore.PicState>, Action>) (observer =>
          {
            object subLock = new object();
            bool disposed = false;
            bool subscribedToPicChange = false;
            IDisposable picSavedSub = (IDisposable) null;
            ChatPicture chatPicture = ContactsContext.Instance<ChatPicture>((Func<ContactsContext, ChatPicture>) (db => db.GetChatPictureState(jid, CreateOptions.None)));
            bool flag1 = chatPicture != null && chatPicture.IsPictureExpired();
            string photoId = chatPicture == null ? (string) null : chatPicture.WaPhotoId;
            bool flag2 = !flag1 && chatPicture != null && chatPicture.WaPhotoId == null;
            if (flag1)
            {
              subMode |= ChatPictureStore.SubMode.GetCurrent;
              Log.d("chatpic", "outdated | get current too | jid: {0}", (object) jid);
            }
            bool gotLarge = false;
            Stream picStream = (subMode & ChatPictureStore.SubMode.GetCurrent) == ChatPictureStore.SubMode.None || photoId == null || !(chatPicture == null | flag2) ? ChatPictureStore.GetStoredPictureStream(jid, photoId, preferLarge, out gotLarge) : (Stream) null;
            if (((!((subMode & ChatPictureStore.SubMode.GetCurrent) != 0 & flag2) ? 0 : (picStream == null ? 1 : 0)) & (fallbackToContactPic ? 1 : 0)) != 0 && JidHelper.IsUserJid(jid))
            {
              UserStatus userStatus = UserCache.Get(jid, false);
              if (userStatus != null && userStatus.PhotoPath != null)
              {
                Log.d("chatpic", "no profile pic | get phonebook pic | jid: {0}", (object) jid);
                picStream = ChatPictureStore.LoadFile(userStatus.PhotoPath);
              }
            }
            if (picStream == null)
            {
              if ((subMode & ChatPictureStore.SubMode.GetCurrent) != ChatPictureStore.SubMode.None)
              {
                ChatPictureStore.SetCache(jid, (ImageSource) null);
                observer.OnNext(new ChatPictureStore.PicState((BitmapSource) null, photoId, false));
                Log.d("chatpic", "notified null | jid:{0},pid:{1}", (object) jid, (object) (photoId ?? "n/a"));
              }
            }
            else if ((subMode & ChatPictureStore.SubMode.GetCurrent) != ChatPictureStore.SubMode.None)
              Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
              {
                BitmapSource imgSrc = (BitmapSource) null;
                using (picStream)
                  imgSrc = ChatPictureStore.CreateBitmapImageWithStream(picStream, gotLarge);
                if (!gotLarge)
                  ChatPictureStore.SetCache(jid, (ImageSource) imgSrc);
                observer.OnNext(new ChatPictureStore.PicState(imgSrc, photoId, gotLarge));
                Log.d("chatpic", "notified | jid:{0},pid:{1},large:{2}", (object) jid, (object) (photoId ?? "n/a"), (object) gotLarge);
                if ((subMode & ChatPictureStore.SubMode.TrackChange) != ChatPictureStore.SubMode.None)
                  return;
                observer.OnCompleted();
                Log.d("chatpic", "notified complete | A | jid:{0}", (object) jid);
              }));
            else if ((subMode & ChatPictureStore.SubMode.TrackChange) == ChatPictureStore.SubMode.None)
            {
              observer.OnCompleted();
              Log.d("chatpic", "notified complete | B | jid:{0}", (object) jid);
            }
            bool flag3 = (chatPicture == null ? 0 : (chatPicture.ShouldBlockPictureRequest() ? 1 : 0)) == 0 && !flag2 && (flag1 || picStream == null || preferLarge && !gotLarge);
            int num = flag3 | forceRequest ? 1 : 0;
            if (num != 0 || (subMode & ChatPictureStore.SubMode.TrackChange) != ChatPictureStore.SubMode.None)
            {
              ChatPictureStore.SubscribePictureChange(jid, preferLarge);
              subscribedToPicChange = true;
              IDisposable d = ChatPictureStore.SavedPicChangedSubject.Where<Pair<string, string>>((Func<Pair<string, string>, bool>) (p => p.First == jid)).Subscribe<Pair<string, string>>((Action<Pair<string, string>>) (p =>
              {
                if (p.Second == null)
                {
                  ChatPictureStore.SetCache(jid, (ImageSource) null);
                  observer.OnNext(new ChatPictureStore.PicState((BitmapSource) null, (string) null, false));
                  Log.d("chatpic", "notified null after request | jid:{0}", (object) jid);
                }
                else
                {
                  bool gotLarge2 = false;
                  Stream picStream2 = ChatPictureStore.GetStoredPictureStream(p.First, p.Second, preferLarge, out gotLarge2);
                  if (picStream2 == null)
                    return;
                  Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
                  {
                    BitmapSource imgSrc = (BitmapSource) null;
                    using (picStream2)
                      imgSrc = ChatPictureStore.CreateBitmapImageWithStream(picStream2, gotLarge2);
                    if (!gotLarge2)
                      ChatPictureStore.SetCache(jid, (ImageSource) imgSrc);
                    observer.OnNext(new ChatPictureStore.PicState(imgSrc, p.Second, gotLarge2));
                    Log.d("chatpic", "notified after request | jid:{0},pid:{1},large:{2}", (object) jid, (object) (p.Second ?? "n/a"), (object) gotLarge2);
                    if ((subMode & ChatPictureStore.SubMode.TrackChange) != ChatPictureStore.SubMode.None)
                      return;
                    observer.OnCompleted();
                    Log.d("chatpic", "notified complete after request | jid:{0}", (object) jid);
                  }));
                  if ((subMode & ChatPictureStore.SubMode.TrackChange) != ChatPictureStore.SubMode.None)
                    return;
                  lock (subLock)
                  {
                    picSavedSub.SafeDispose();
                    picSavedSub = (IDisposable) null;
                    if (!subscribedToPicChange)
                      return;
                    ChatPictureStore.UnsubscribePictureChange(jid, preferLarge);
                    subscribedToPicChange = false;
                  }
                }
              }));
              if (disposed)
                d.SafeDispose();
              else
                picSavedSub = d;
            }
            if (num != 0)
              ChatPictureStore.RequestPicture(jid, preferLarge, !forceRequest || flag3 ? (string) null : photoId);
            return (Action) (() =>
            {
              lock (subLock)
              {
                disposed = true;
                picSavedSub.SafeDispose();
                picSavedSub = (IDisposable) null;
                if (!subscribedToPicChange)
                  return;
                ChatPictureStore.UnsubscribePictureChange(jid, preferLarge);
                subscribedToPicChange = false;
              }
            });
          }));
      }
    }

    public static IObservable<ChatPictureStore.PicState> GetState(
      string jid,
      bool isLargeFormat,
      bool forceRequest,
      bool fallbackToContactPic)
    {
      return Observable.Create<ChatPictureStore.PicState>((Func<IObserver<ChatPictureStore.PicState>, Action>) (observer =>
      {
        object @lock = new object();
        bool disposed = false;
        IDisposable picFetchSub = (IDisposable) null;
        IDisposable picUpdateSub = (IDisposable) null;
        picUpdateSub = Observable.Return<int?>(new int?()).Concat<int?>(ChatPictureStore.PicIdChangedSubject.Where<string>((Func<string, bool>) (j => j == jid)).Select<string, int?>((Func<string, int?>) (_ => new int?()))).Merge<int?>(ChatPictureStore.ForcePicUpdateSubject.Where<Pair<string, int?>>((Func<Pair<string, int?>, bool>) (p => p.First == jid)).Select<Pair<string, int?>, int?>((Func<Pair<string, int?>, int?>) (p => p.Second))).ObserveOnDispatcher<int?>().Subscribe<int?>((Action<int?>) (errCode =>
        {
          if (errCode.HasValue)
            observer.OnNext(new ChatPictureStore.PicState((BitmapSource) null, (string) null, false)
            {
              ErrorCode = errCode
            });
          BitmapSource pendingPicture = ChatPictureStore.FindPendingPicture(jid, isLargeFormat);
          if (pendingPicture == null)
          {
            lock (@lock)
            {
              if (disposed || picFetchSub != null)
                return;
              picFetchSub = ChatPictureStore.Get(jid, isLargeFormat, forceRequest, fallbackToContactPic).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).Subscribe(observer);
            }
          }
          else
            observer.OnNext(new ChatPictureStore.PicState(pendingPicture, (string) null, true, true));
        }));
        return (Action) (() =>
        {
          lock (@lock)
          {
            disposed = true;
            picFetchSub.SafeDispose();
            picFetchSub = (IDisposable) null;
          }
          picUpdateSub.SafeDispose();
          picUpdateSub = (IDisposable) null;
        });
      }));
    }

    public static bool GetCache(string jid, out ImageSource cachedImgSrc)
    {
      cachedImgSrc = (ImageSource) null;
      if (jid == null)
        return false;
      lock (ChatPictureStore.cacheLock)
        return ChatPictureStore.cachedPics != null && ChatPictureStore.cachedPics.TryGet(jid, out cachedImgSrc);
    }

    public static void RemoveCache(string jid)
    {
      if (jid == null)
        return;
      lock (ChatPictureStore.cacheLock)
      {
        if (ChatPictureStore.cachedPics == null)
          return;
        ChatPictureStore.cachedPics.Remove(jid);
      }
    }

    public static void SetCache(string jid, ImageSource imgSrc)
    {
      if (jid == null)
        return;
      lock (ChatPictureStore.cacheLock)
      {
        if (ChatPictureStore.cachedPics == null)
          ChatPictureStore.cachedPics = new KeyValueCache<string, ImageSource>(AppState.IsDecentMemoryDevice ? 50 : 20, true);
        ChatPictureStore.cachedPics.Set(jid, imgSrc);
      }
    }

    public static string GetPicturePath(string jid, out bool toRequestLargePic)
    {
      toRequestLargePic = false;
      string storedPicturePath = ChatPictureStore.GetStoredPicturePath(jid, true);
      if (storedPicturePath == null)
      {
        toRequestLargePic = true;
        storedPicturePath = ChatPictureStore.GetStoredPicturePath(jid, false);
      }
      return storedPicturePath;
    }

    public static string GetStoredPicturePath(string jid, bool isLargeFormat)
    {
      ChatPicture chatPicture = ContactsContext.Instance<ChatPicture>((Func<ContactsContext, ChatPicture>) (db => db.GetChatPictureState(jid, CreateOptions.None)));
      if (chatPicture == null || chatPicture.WaPhotoId == null)
        return (string) null;
      string path = ChatPictureStore.GeneratePictureFilepath(jid, chatPicture.WaPhotoId, isLargeFormat);
      if (path != null)
      {
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (!storeForApplication.FileExists(path))
              path = (string) null;
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "chatpic: get stored path");
          path = (string) null;
        }
      }
      return path;
    }

    public static Stream GetStoredPictureStream(string jid, string photoId, bool largeFormat)
    {
      bool gotLarge = false;
      Stream storedPictureStream = ChatPictureStore.GetStoredPictureStream(jid, photoId, largeFormat, out gotLarge);
      if (largeFormat && !gotLarge)
        storedPictureStream = (Stream) null;
      return storedPictureStream;
    }

    public static Stream GetStoredPictureStream(
      string jid,
      string photoId,
      bool preferLarge,
      out bool gotLarge)
    {
      gotLarge = false;
      Stream storedPictureStream = ChatPictureStore.LoadFile(ChatPictureStore.GeneratePictureFilepath(jid, photoId, preferLarge));
      if (storedPictureStream != null)
        gotLarge = preferLarge;
      else if (preferLarge)
        storedPictureStream = ChatPictureStore.LoadFile(ChatPictureStore.GeneratePictureFilepath(jid, photoId, false));
      if (storedPictureStream == null)
      {
        byte[] picBytes = (byte[]) null;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
          if (chatPictureState == null || !(chatPictureState.WaPhotoId == photoId))
            return;
          if (preferLarge && chatPictureState.PictureData != null)
            picBytes = chatPictureState.PictureData;
          else
            picBytes = chatPictureState.ThumbnailData;
        }));
        if (picBytes != null)
          storedPictureStream = (Stream) new MemoryStream(picBytes);
      }
      return storedPictureStream;
    }

    public static bool IsPictureIdUptoDate(string jid, string photoId)
    {
      string prevPhotoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
        if (chatPictureState == null)
          return;
        prevPhotoId = chatPictureState.WaPhotoId;
      }));
      return photoId == prevPhotoId;
    }

    public static string UpdatePictureId(string jid, string photoId)
    {
      string prevPhotoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.CreateToDbIfNotFound);
        prevPhotoId = chatPictureState.WaPhotoId;
        chatPictureState.WaPhotoId = photoId;
        chatPictureState.LastPictureCheck = new DateTime?(DateTime.UtcNow);
        db.SubmitChanges();
      }));
      if (photoId != prevPhotoId)
      {
        ChatPictureStore.RemoveCachedAndStalePictures(jid, prevPhotoId);
        ChatPictureStore.PicIdChangedSubject.OnNext(jid);
      }
      Log.l("chatpic", "pic id updated | jid={0} pid={1}->{2}", (object) jid, (object) prevPhotoId, (object) photoId);
      return prevPhotoId;
    }

    private static void RemoveCachedAndStalePictures(string jid, string prevPhotoId)
    {
      ChatPictureStore.RemoveCache(jid);
      if (string.IsNullOrEmpty(prevPhotoId))
        return;
      ChatPictureStore.DeleteSavedPicture(jid, prevPhotoId);
    }

    public static void Reset(string jid, DateTime? lastPictureCheck)
    {
      Log.l("chatpic", "reset picture id | jid={0}", (object) jid);
      string prevPhotoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.CreateToDbIfNotFound);
        prevPhotoId = chatPictureState.WaPhotoId;
        chatPictureState.WaPhotoId = (string) null;
        chatPictureState.LastPictureCheck = lastPictureCheck;
        db.SubmitChanges();
      }));
      if (prevPhotoId == null)
        return;
      ChatPictureStore.RemoveCachedAndStalePictures(jid, (string) null);
      ITile chatTile = TileHelper.GetChatTile(jid);
      if (chatTile != null)
        TileHelper.UpdateChatTilePicture(chatTile, jid, (ImageSource) null);
      VoipPictureStore.DeleteVoipContactPhoto(jid);
      ChatPictureStore.PicIdChangedSubject.OnNext(jid);
    }

    public static void fetchNewPhoto(string jid, Action onComplete = null)
    {
      bool flag = false;
      bool fetchLarge = false;
      ChatPictureStore.SubState subState = (ChatPictureStore.SubState) null;
      lock (ChatPictureStore.subsLock)
      {
        if (ChatPictureStore.subs.TryGetValue(jid, out subState))
        {
          if (subState != null)
          {
            if (subState.SubCount > 0)
            {
              flag = true;
              fetchLarge = subState.SubCountForLarge > 0;
            }
          }
        }
      }
      if (!flag && JidHelper.IsUserJid(jid))
        flag = UserCache.Get(jid, false) != null;
      if (!fetchLarge && TileHelper.ChatTileExists(jid))
        flag = fetchLarge = true;
      Log.l("chatpic", "fetching new photo | jid={0}, now={1}, large = {2}", (object) jid, (object) flag, (object) fetchLarge);
      if (!flag)
        return;
      AppState.InvokeWhenConnected((Action<FunXMPP.Connection>) (conn =>
      {
        conn.SendGetPhoto(jid, (string) null, false, onComplete);
        if (!fetchLarge)
          return;
        conn.SendGetPhoto(jid, (string) null, true);
      }));
    }

    public static void UpdatePictureData(
      string jid,
      string photoId,
      byte[] smallPicBytes,
      byte[] largePicBytes)
    {
      if (jid == null || photoId == null)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        WaScheduledTask task1 = (WaScheduledTask) null;
        if (smallPicBytes != null && ((IEnumerable<byte>) smallPicBytes).Any<byte>())
        {
          task1 = ChatPictureStore.CreateSaveChatPictureTask(jid, photoId, smallPicBytes, false);
          db.InsertWaScheduledTaskOnSubmit(task1);
        }
        WaScheduledTask task2 = (WaScheduledTask) null;
        if (largePicBytes != null && ((IEnumerable<byte>) largePicBytes).Any<byte>())
        {
          task2 = ChatPictureStore.CreateSaveChatPictureTask(jid, photoId, largePicBytes, true);
          db.InsertWaScheduledTaskOnSubmit(task2);
        }
        db.SubmitChanges();
        db.AttemptScheduledTaskOnThreadPool(task1, 100);
        db.AttemptScheduledTaskOnThreadPool(task2, 500);
      }));
    }

    private static void OnPictureSaved(
      string jid,
      string photoId,
      string savedSmallPicPath,
      string savedLargePicPath)
    {
      ChatPictureStore.SavedPicChangedSubject.OnNext(new Pair<string, string>(jid, photoId));
      ITile tile = TileHelper.GetChatTile(jid);
      if (tile != null)
      {
        if (savedLargePicPath == null)
          ChatPictureStore.RequestPicture(jid, true);
        else
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => TileHelper.UpdateChatTilePicture(tile, jid, savedLargePicPath)));
      }
      VoipPictureStore.DeleteVoipContactPhoto(jid);
    }

    public static void Reset(string jid) => ChatPictureStore.Reset(jid, new DateTime?());

    public static void Delete(string jid)
    {
      if (string.IsNullOrEmpty(jid))
        return;
      string photoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
        if (chatPictureState == null)
          return;
        photoId = chatPictureState.WaPhotoId;
        db.DeleteChatPictureOnSubmit(chatPictureState);
        db.SubmitChanges();
      }));
      if (string.IsNullOrEmpty(photoId))
        return;
      ChatPictureStore.DeleteSavedPicture(jid, photoId);
    }

    private static void DeleteSavedPicture(string jid, string photoId)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        WaScheduledTask savedChatPictureTask = ChatPictureStore.CreateDeleteSavedChatPictureTask(jid, photoId);
        db.InsertWaScheduledTaskOnSubmit(savedChatPictureTask);
        db.SubmitChanges();
      }));
      ChatPictureStore.SavedPicChangedSubject.OnNext(new Pair<string, string>(jid, (string) null));
    }

    public static bool SaveToPhone(string jid)
    {
      if (jid == null)
      {
        Log.l("chatpic", "save to phone | null jid");
        return false;
      }
      string photoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
        if (chatPictureState == null)
          return;
        photoId = chatPictureState.WaPhotoId;
      }));
      if (photoId == null)
      {
        Log.l("chatpic", "save to phone | no photo id");
        return false;
      }
      string destFilename = string.Format("{0}.jpg", (object) ChatPictureStore.GenerateHashedFilename(jid, photoId, true));
      string storedPicturePath = ChatPictureStore.GetStoredPicturePath(jid, true);
      if (storedPicturePath == null)
      {
        Log.l("chatpic", "save to phone skipped | no file");
        return false;
      }
      bool phone = MediaDownload.SaveMedia(storedPicturePath, FunXMPP.FMessage.Type.Image, destFilename) != null;
      Log.l("chatpic", "pic for {0} {1}saved", (object) jid, phone ? (object) "" : (object) "NOT ");
      return phone;
    }

    private static void SubscribePictureChange(string jid, bool fetchLargeOnChange)
    {
      if (jid == null)
        return;
      lock (ChatPictureStore.subsLock)
      {
        ChatPictureStore.SubState subState = (ChatPictureStore.SubState) null;
        if (!ChatPictureStore.subs.TryGetValue(jid, out subState) || subState == null)
          ChatPictureStore.subs[jid] = subState = new ChatPictureStore.SubState();
        subState.Add(fetchLargeOnChange);
      }
    }

    private static void UnsubscribePictureChange(string jid, bool fetchLargeOnChange)
    {
      if (jid == null)
        return;
      lock (ChatPictureStore.subsLock)
      {
        ChatPictureStore.SubState subState = (ChatPictureStore.SubState) null;
        if (!ChatPictureStore.subs.TryGetValue(jid, out subState))
          return;
        subState?.Reduce(fetchLargeOnChange);
        if (subState != null && subState.SubCount > 0)
          return;
        ChatPictureStore.subs.Remove(jid);
      }
    }

    private static string GenerateFilename(string jid, string photoId, bool largeFormat)
    {
      return string.Format("{0}{1}{2}", (object) jid, (object) photoId, largeFormat ? (object) "" : (object) "_thumb");
    }

    private static string GenerateHashedFilename(string jid, string photoId, bool largeFormat)
    {
      using (SHA1Managed shA1Managed = new SHA1Managed())
        return shA1Managed.ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}-{1}{2}", (object) jid, (object) photoId, largeFormat ? (object) "" : (object) "-thumbnail"))).ToHexString();
    }

    public static string GeneratePictureFilepath(string jid, string photoId, bool largeFormat)
    {
      return jid != null && photoId != null ? string.Format("{0}/{1}", (object) "profilePictures", (object) ChatPictureStore.GenerateFilename(jid, photoId, largeFormat)) : (string) null;
    }

    private static BitmapSource CreateBitmapImageWithStream(Stream stream, bool largeFormat)
    {
      int num = largeFormat ? 480 : 128;
      if (JpegUtils.GetJpegOrientation(stream).HasValue)
        return (BitmapSource) BitmapUtils.CreateBitmap(stream, num, num);
      try
      {
        BitmapImage bitmapImageWithStream = new BitmapImage();
        bitmapImageWithStream.CreateOptions = BitmapCreateOptions.BackgroundCreation;
        bitmapImageWithStream.DecodePixelWidth = num;
        bitmapImageWithStream.DecodePixelHeight = num;
        bitmapImageWithStream.SetSource(stream);
        return (BitmapSource) bitmapImageWithStream;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "chatpic: decode image");
        return (BitmapSource) null;
      }
    }

    private static Stream LoadFile(string filepath)
    {
      if (filepath == null)
        return (Stream) null;
      Stream destination = (Stream) null;
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(filepath))
          {
            using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
              destination = (Stream) new MemoryStream();
              storageFileStream.CopyTo(destination);
              destination.Position = 0L;
            }
          }
        }
      }
      catch (Exception ex)
      {
        destination = (Stream) null;
      }
      return destination;
    }

    public static BitmapSource FindPendingPicture(string jid, bool isLargeFormat)
    {
      BitmapSource pendingPicture = (BitmapSource) null;
      byte[] bytes = (byte[]) null;
      int offset = 0;
      int length = 0;
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        foreach (PersistentAction.SetPhotoArgs setPhotoArgs in ((IEnumerable<PersistentAction>) db.GetPersistentActions(PersistentAction.Types.SetPhoto)).Where<PersistentAction>((Func<PersistentAction, bool>) (a => !a.PhotoAttempted)).Select<PersistentAction, PersistentAction.SetPhotoArgs>((Func<PersistentAction, PersistentAction.SetPhotoArgs>) (a => a.DeserializePhotoArgs())))
        {
          if (setPhotoArgs.Jid == jid)
          {
            PersistentAction.SetPhotoArgs.Buffer buffer = isLargeFormat ? setPhotoArgs.FullSize : setPhotoArgs.Thumbnail;
            if (buffer == null)
              break;
            bytes = buffer.Buf;
            offset = buffer.Offset;
            length = buffer.Length;
            break;
          }
        }
      }));
      if (bytes != null)
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
        bitmapImage.SetSource((Stream) new MemoryStream(bytes, offset, length, false));
        pendingPicture = (BitmapSource) bitmapImage;
      }
      return pendingPicture;
    }

    public static void ForcePendingPictureUpdate(string jid, int? errCode)
    {
      ChatPictureStore.ForcePicUpdateSubject.OnNext(new Pair<string, int?>(jid, errCode));
    }

    public static void RequestPicture(
      string jid,
      bool isLargeFormat,
      string expectedPhotoId = null,
      Action onError = null)
    {
      if (jid == Settings.MyJid)
        Log.d("chatpic", "self pic request | expected_pid={0} large={1}", (object) expectedPhotoId, (object) isLargeFormat);
      string pendingKey = string.Format("{0}-{1}", (object) jid, isLargeFormat ? (object) "L" : (object) "S");
      lock (ChatPictureStore.pendingReqsLock)
      {
        DateTime now = DateTime.Now;
        DateTime dateTime;
        if (ChatPictureStore.pendingPicRequests.TryGetValue(pendingKey, out dateTime) && now - dateTime < TimeSpan.FromMinutes(1.0))
        {
          Log.d("chatpic", "pic request skipped | pending | {0} {1}", (object) pendingKey, (object) dateTime);
          return;
        }
        ChatPictureStore.pendingPicRequests[pendingKey] = now;
      }
      AppState.ImageWorker.Enqueue((Action) (() => AppState.InvokeWhenConnected((Action<FunXMPP.Connection>) (conn =>
      {
        Action onComplete = (Action) (() =>
        {
          lock (ChatPictureStore.pendingReqsLock)
            ChatPictureStore.pendingPicRequests.Remove(pendingKey);
        });
        conn.SendGetPhoto(jid, expectedPhotoId, isLargeFormat, onComplete, (Action<int>) (errCode =>
        {
          Log.l("chatpic", "pic request | err_code={0} jid={1}", (object) errCode, (object) jid);
          if (errCode == 404 || errCode == 401)
            ChatPictureStore.Reset(jid, new DateTime?(DateTime.UtcNow));
          else if (errCode != 408)
            ContactsContext.Instance((Action<ContactsContext>) (db =>
            {
              ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.CreateToDbIfNotFound);
              if (chatPictureState == null)
                return;
              if (errCode == 501 || errCode == 503 || errCode == 500)
                chatPictureState.BlockPictureRequest(TimeSpan.FromHours(1.0));
              else
                chatPictureState.BlockPictureRequest(TimeSpan.FromDays(1.0));
              db.SubmitChanges();
            }));
          if (onError != null)
            onError();
          onComplete();
        }));
      }))));
    }

    private static WaScheduledTask CreateSaveChatPictureTask(
      string jid,
      string photoId,
      byte[] picData,
      bool isLarge)
    {
      if (jid == null || photoId == null || picData == null || !((IEnumerable<byte>) picData).Any<byte>())
        return (WaScheduledTask) null;
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(isLarge ? 1 : 0);
      binaryData.AppendStrWithLengthPrefix(photoId);
      binaryData.AppendBytes((IEnumerable<byte>) picData);
      return new WaScheduledTask(WaScheduledTask.Types.SaveChatPic, jid, binaryData.Get(), WaScheduledTask.Restrictions.FgOnly, new TimeSpan?(TimeSpan.FromDays(7.0)));
    }

    public static IObservable<Unit> PerformSaveChatPictureTask(WaScheduledTask task)
    {
      return task.TaskType != 3 ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        bool skip = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          WaScheduledTask[] waScheduledTasks = db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
          {
            WaScheduledTask.Types.SaveChatPic
          }, excludeExpired: false, lookupKey: task.LookupKey);
          if (waScheduledTasks.Length == 0)
          {
            skip = true;
          }
          else
          {
            if (waScheduledTasks.Length <= 1 || ((IEnumerable<WaScheduledTask>) waScheduledTasks).Last<WaScheduledTask>().TaskID == task.TaskID)
              return;
            skip = true;
          }
        }));
        if (skip)
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
          return (Action) (() => { });
        }
        BinaryData binaryData = new BinaryData(task.BinaryData);
        string jid = task.LookupKey;
        int offset = 0;
        bool isLarge = binaryData.ReadInt32(offset) != 0;
        int newOffset = offset + 4;
        string photoId = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
        byte[] photoData = binaryData.ReadBytes(newOffset);
        string str = string.Format("jid:{0},pid:{1},{2}", (object) jid, (object) photoId, isLarge ? (object) "large" : (object) "small");
        string savedTo = (string) null;
        string path = (string) null;
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            path = ChatPictureStore.GeneratePictureFilepath(jid, photoId, isLarge);
            if (path != null)
            {
              if (!storeForApplication.DirectoryExists("profilePictures"))
                storeForApplication.CreateDirectory("profilePictures");
              using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
                storageFileStream.Write(photoData, 0, photoData.Length);
              savedTo = "iso store";
            }
          }
        }
        catch (Exception ex)
        {
          path = (string) null;
          Log.l("chatpic", "save pic to iso store failed | {0}", (object) str);
          Log.d(ex, "chatpic: save pic to iso store failed");
        }
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
          if (chatPictureState == null)
            return;
          bool flag = false;
          if (savedTo == null && chatPictureState.WaPhotoId == photoId)
          {
            if (isLarge)
              chatPictureState.PictureData = photoData;
            else
              chatPictureState.ThumbnailData = photoData;
            flag = true;
            savedTo = "cdb";
          }
          else if (chatPictureState.PictureData != null)
          {
            if (isLarge)
              chatPictureState.PictureData = (byte[]) null;
            else
              chatPictureState.ThumbnailData = (byte[]) null;
            flag = true;
          }
          if (!flag)
            return;
          db.SubmitChanges();
        }));
        if (savedTo == null)
        {
          Log.l("chatpic", "pic save failed | {0}", (object) str);
        }
        else
        {
          Log.l("chatpic", "pic saved | {0} | {1}", (object) savedTo, (object) str);
          observer.OnNext(new Unit());
          ChatPictureStore.OnPictureSaved(jid, photoId, isLarge ? (string) null : path, isLarge ? path : (string) null);
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private static WaScheduledTask CreateDeleteSavedChatPictureTask(string jid, string photoId)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendStrWithLengthPrefix(photoId);
      return new WaScheduledTask(WaScheduledTask.Types.DeleteChatPic, jid, binaryData.Get(), WaScheduledTask.Restrictions.FgOnly, new TimeSpan?(TimeSpan.FromDays(7.0)));
    }

    public static IObservable<Unit> PerformDeleteSavedChatPictureTask(WaScheduledTask task)
    {
      return task.TaskType != 2 ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        BinaryData binaryData = new BinaryData(task.BinaryData);
        string jid = task.LookupKey;
        string photoId = binaryData.ReadStrWithLengthPrefix(0);
        bool onNext = false;
        bool onCompleted = false;
        ContactsContext.Instance((Action<ContactsContext>) (db =>
        {
          ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
          if (chatPictureState == null || chatPictureState.WaPhotoId != photoId)
          {
            try
            {
              using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
              {
                bool[] flagArray = new bool[2]
                {
                  false,
                  true
                };
                foreach (bool largeFormat in flagArray)
                {
                  string pictureFilepath = ChatPictureStore.GeneratePictureFilepath(jid, photoId, largeFormat);
                  if (pictureFilepath != null && storeForApplication.FileExists(pictureFilepath))
                  {
                    storeForApplication.DeleteFile(pictureFilepath);
                    Log.l("chatpic", "file deleted | jid:{0},path:{1}", (object) jid, (object) pictureFilepath);
                  }
                  else
                    Log.l("chatpic", "skip deletion | jid:{0},path:{1},large:{2}", (object) jid, (object) photoId, (object) largeFormat);
                }
              }
              onNext = true;
            }
            catch (Exception ex)
            {
              string context = string.Format("chatpic: delete | jid={0} pid={1}", (object) jid, (object) photoId);
              Log.LogException(ex, context);
            }
            finally
            {
              onCompleted = true;
            }
          }
          else
          {
            onNext = true;
            onCompleted = true;
            Log.l("chatpic", "deletion aborted | in use | jid={0}", (object) jid);
          }
        }));
        if (onNext)
          observer.OnNext(new Unit());
        if (onCompleted)
          observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private class SubState
    {
      public int SubCount;
      public int SubCountForLarge;

      public void Add(bool subToLarge)
      {
        ++this.SubCount;
        if (!subToLarge)
          return;
        ++this.SubCountForLarge;
      }

      public void Reduce(bool subToLarge)
      {
        this.SubCount = Math.Max(this.SubCount - 1, 0);
        if (!subToLarge)
          return;
        this.SubCountForLarge = Math.Max(this.SubCountForLarge - 1, 0);
      }
    }

    public class PicState
    {
      public BitmapSource Image { get; private set; }

      public string PhotoId { get; private set; }

      public bool IsLargeFormat { get; private set; }

      public bool IsPending { get; private set; }

      public int? ErrorCode { get; set; }

      public PicState(BitmapSource imgSrc, string photoId, bool isLargeFormat, bool isPending = false)
      {
        this.Image = imgSrc;
        this.PhotoId = photoId;
        this.IsLargeFormat = isLargeFormat;
        this.IsPending = isPending;
        this.ErrorCode = new int?();
      }
    }

    public enum SubMode
    {
      None = 0,
      GetCurrent = 1,
      TrackChange = 16, // 0x00000010
      Default = 17, // 0x00000011
    }
  }
}
