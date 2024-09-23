// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaUpload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class MediaUpload
  {
    private const string LogHeader = "media upload";
    private static bool? optimisticUploadAllowed;

    public static bool OptimisticUploadAllowed
    {
      get
      {
        if (!MediaUpload.optimisticUploadAllowed.HasValue)
        {
          string myJid = Settings.MyJid;
          byte num = 0;
          using (SHA1Managed shA1Managed = new SHA1Managed())
          {
            byte[] bytes = Encoding.UTF8.GetBytes(myJid);
            byte[] hash = shA1Managed.ComputeHash(bytes, 0, bytes.Length);
            num = hash[hash.Length - 1];
          }
          MediaUpload.optimisticUploadAllowed = new bool?((int) num % 2 == 1);
        }
        return MediaUpload.optimisticUploadAllowed.Value;
      }
    }

    private static bool TryScale(
      double pixelLimit,
      double width,
      double height,
      out double newWidth,
      out double newHeight)
    {
      newHeight = Math.Sqrt(pixelLimit * height / width);
      newWidth = width * (newHeight / height);
      return newHeight > 0.0 && newWidth > 0.0;
    }

    public static void SendMediaMessage(IObservable<Message> msgObs, Action onCompleted = null)
    {
      bool procd = false;
      if (onCompleted == null)
        onCompleted = (Action) (() => { });
      msgObs.ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>((Action<Message>) (msg =>
      {
        procd = true;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          db.InsertMessageOnSubmit(msg);
          db.LocalFileAddRef(msg.LocalFileUri, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
          if (msg.DataFileName != null)
            db.LocalFileAddRef(msg.DataFileName, LocalFileType.Thumbnail);
          MessageMiscInfo miscInfo = msg.GetMiscInfo((SqliteMessagesContext) db);
          if (miscInfo != null && miscInfo.AlternateUploadUri != null)
            db.LocalFileAddRef(miscInfo.AlternateUploadUri, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
          db.SubmitChanges();
        }));
        Log.l("media upload", "saved outgoing media msg | msg_id={0} type={1}", (object) msg.MessageID, (object) msg.MediaWaType);
        if (msg.UploadContext.isType(UploadContext.UploadContextType.Streaming))
          ((StreamingUploadContext) msg.UploadContext).Message = msg;
        onCompleted();
      }), (Action<Exception>) (ex => Log.LogException(ex, "send media obs")), (Action) (() =>
      {
        if (procd)
          return;
        onCompleted();
      }));
    }

    private static byte[] ComputeSha1Hash(string filename)
    {
      if (string.IsNullOrEmpty(filename))
        return (byte[]) null;
      byte[] sha1Hash = (byte[]) null;
      try
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(filename))
        {
          using (Stream inputStream = mediaStorage.OpenFile(filename))
          {
            using (SHA1Managed shA1Managed = new SHA1Managed())
              sha1Hash = shA1Managed.ComputeHash(inputStream);
          }
        }
      }
      catch (Exception ex)
      {
        Log.WriteLineDebug("media upload - unable to compute hash " + ex.ToString());
      }
      return sha1Hash;
    }

    public static string GenerateMediaFilename(string extension)
    {
      using (SHA1Managed shA1Managed = new SHA1Managed())
      {
        shA1Managed.Initialize();
        byte[] bytes = Encoding.UTF8.GetBytes(Settings.PhoneNumber + (object) DateTime.Now.Ticks);
        return string.Format("{0}.{1}", (object) shA1Managed.ComputeHash(bytes).ToHexString(), (object) extension);
      }
    }

    public static string CopyLocal(Stream inStream, string filename)
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        string path = string.Format("{0}/{1}", (object) MediaDownload.GetDirectoryPath(), (object) filename);
        if (storeForApplication.FileExists(path))
          return path;
        inStream.Position = 0L;
        using (IsolatedStorageFileStream file = storeForApplication.CreateFile(path))
          inStream.CopyTo((Stream) file);
        inStream.Position = 0L;
        return path;
      }
    }

    private static MediaUpload.OutgoingMediaInfo SaveOutgoingMedia(
      MediaUpload.MediaDescriptor media,
      Stream inStream,
      string filename = null)
    {
      MediaUpload.OutgoingMediaInfo outgoingMediaInfo = new MediaUpload.OutgoingMediaInfo();
      outgoingMediaInfo.TargetFilename = MediaUpload.GenerateMediaFilename(media.Extension);
      if (filename != null)
      {
        outgoingMediaInfo.LocalFileUri = filename;
        return outgoingMediaInfo;
      }
      if (media.FullPath != null)
      {
        if (media.WaType == FunXMPP.FMessage.Type.Image)
          outgoingMediaInfo.ScaledFileUri = MediaUpload.CopyLocal(inStream, outgoingMediaInfo.TargetFilename);
        outgoingMediaInfo.LocalFileUri = filename != null ? filename : NativeMediaStorage.MakeUri(media.FullPath);
      }
      else
      {
        outgoingMediaInfo.LocalFileUri = MediaUpload.CopyLocal(inStream, outgoingMediaInfo.TargetFilename);
        outgoingMediaInfo.TargetFilename = (string) null;
      }
      return outgoingMediaInfo;
    }

    public static void SendPicture(
      List<string> jids,
      Stream jpegStream,
      int largeThumbnailMaxWidth,
      WriteableBitmap picture = null,
      string fullpath = null,
      string caption = null,
      Action<Message> messageModifier = null,
      bool canUseSuppliedStream = false)
    {
      if (jpegStream == null)
        return;
      bool flag = jids.Count == 1 && jids.First<string>() == "status@broadcast";
      WriteableBitmap largeThumb = (WriteableBitmap) null;
      Stream sendingStream = (Stream) null;
      try
      {
        sendingStream = !canUseSuppliedStream || flag ? MediaUpload.CreateImageStreamAndPicture(jpegStream, ref picture, flag ? new int?(Settings.StatusImageMaxEdge) : new int?(), flag ? new int?(Settings.StatusImageQuality) : new int?()) : jpegStream;
        Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => largeThumb = ImageStore.CreateMessageLargeThumbnail(sendingStream, picture.PixelWidth, picture.PixelHeight, largeThumbnailMaxWidth)));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "SendPicture exception creating stream/large thumb");
        sendingStream?.Dispose();
        sendingStream = (Stream) null;
      }
      if (sendingStream == null || picture == null)
        return;
      byte[] thumbBytes = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        double num = 100.0 / (double) Math.Max(picture.PixelWidth, picture.PixelHeight);
        picture.SaveJpegWithMaxSize((Stream) memoryStream, (int) ((double) picture.PixelWidth * num), (int) ((double) picture.PixelHeight * num), 0, Settings.JpegQuality, 48128);
        memoryStream.Position = 0L;
        thumbBytes = memoryStream.ToArray();
      }
      byte[] largeThumbBytes = (byte[]) null;
      if (largeThumb != null)
      {
        using (MemoryStream targetStream = new MemoryStream())
        {
          largeThumb.SaveJpeg((Stream) targetStream, largeThumb.PixelWidth, largeThumb.PixelHeight, 0, 70);
          targetStream.Position = 0L;
          largeThumbBytes = targetStream.ToArray();
        }
      }
      picture = (WriteableBitmap) null;
      MediaUpload.SendMediaMessage(MediaUpload.CreateMessageWithPicture(jids, sendingStream, largeThumbBytes, thumbBytes, fullpath, caption, messageModifier));
    }

    public static Stream CreateImageStreamAndPicture(
      Stream jpegStream,
      ref WriteableBitmap picture,
      int? maxEdge = null,
      int? jpegQuality = null)
    {
      if (!maxEdge.HasValue)
        maxEdge = new int?(Settings.ImageMaxEdge);
      if (!jpegQuality.HasValue)
        jpegQuality = new int?(Settings.JpegQuality);
      Stream sendingStream = (Stream) null;
      bool flag = false;
      int? exifOrientation = new int?();
      if (jpegStream == null)
      {
        picture = (WriteableBitmap) null;
        return (Stream) null;
      }
      try
      {
        jpegStream.Position = 0L;
        ushort? jpegOrientation = JpegUtils.GetJpegOrientation(jpegStream);
        exifOrientation = jpegOrientation.HasValue ? new int?((int) jpegOrientation.GetValueOrDefault()) : new int?();
        jpegStream.Position = 0L;
        if (jpegStream.Length > (long) (Settings.ImageMaxKbytes * 1024))
          flag = true;
        else if ((exifOrientation ?? 1) != 1)
        {
          flag = true;
        }
        else
        {
          if (picture == null)
          {
            WriteableBitmap decodedImage = (WriteableBitmap) null;
            Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => decodedImage = JpegUtils.DecodeJpeg(jpegStream)));
            picture = decodedImage;
          }
          if (Math.Max(picture.PixelHeight, picture.PixelWidth) > maxEdge.Value)
          {
            picture = (WriteableBitmap) null;
            flag = true;
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "createImageStream exception determining re-encoding requirement");
        flag = true;
      }
      try
      {
        if (!flag)
        {
          sendingStream = (Stream) new NativeMediaStorage().GetTempFile();
          if (JpegUtils.StripApplicationData(jpegStream, sendingStream))
          {
            sendingStream.Position = 0L;
            Log.l("Stripped application data from input stream - was {0}, now {1}", (object) jpegStream.Length, (object) sendingStream.Length);
          }
          else
          {
            jpegStream.Position = 0L;
            sendingStream.Dispose();
            sendingStream = (Stream) null;
            flag = true;
            Log.l("Image not valid for stripping");
          }
        }
        if (flag)
        {
          WriteableBitmap decodedImage = (WriteableBitmap) null;
          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
          {
            jpegStream.Position = 0L;
            decodedImage = JpegUtils.DecodeJpeg(jpegStream, maxEdge.Value, maxEdge.Value);
            if ((exifOrientation ?? 1) != 1)
              decodedImage = JpegUtils.ApplyJpegOrientation((BitmapSource) decodedImage, exifOrientation);
            bool adjusted = false;
            Size size = MediaUpload.CalcImageSizeWithMaxEdge(new Size((double) decodedImage.PixelWidth, (double) decodedImage.PixelHeight), (double) maxEdge.Value, out adjusted);
            sendingStream = (Stream) new NativeMediaStorage().GetTempFile();
            decodedImage.SaveJpegWithMaxSize(sendingStream, (int) size.Width, (int) size.Height, 0, jpegQuality.Value, Settings.ImageMaxKbytes * 1024);
          }));
          picture = decodedImage;
        }
      }
      catch (Exception ex)
      {
        string context = string.Format("createImageStream exception processing {0}", (object) flag);
        Log.LogException(ex, context);
        sendingStream?.Dispose();
        sendingStream = (Stream) null;
        picture = (WriteableBitmap) null;
      }
      return sendingStream;
    }

    public static void SendAudio(
      List<string> jids,
      WaAudioArgs args,
      bool isPtt,
      Action onCompleted = null,
      StreamingUploadContext ctx = null)
    {
      MediaUpload.SendMediaMessage(MediaUpload.CreateMessageWithAudio(jids, args, isPtt, ctx), onCompleted);
    }

    public static void SendVideo(List<string> jids, WaVideoArgs args, Action onCompleted = null)
    {
      MediaUpload.SendMediaMessage(MediaUpload.CreateMessageWithVideo(jids, args), onCompleted);
    }

    public static void SendDocument(
      List<string> jids,
      DocumentMessageUtils.DocumentData docData,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted,
      Action onCompleted = null)
    {
      Log.l("media upload", "send doc | jid:{0}", (object) jids);
      MediaUpload.SendMediaMessage(MediaUpload.CreateMessageWithDocument(jids, docData, quotedMsg, quotedChat, c2cStarted), onCompleted);
    }

    public static void SendSticker(
      List<string> jids,
      Sticker sticker,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted,
      Action onCompleted = null)
    {
      Log.l("media upload", "send sticker | jid:{0}", (object) jids);
      MediaUpload.SendMediaMessage(MediaUpload.CreateMessageWithSticker(jids, sticker, quotedMsg, quotedChat, c2cStarted), onCompleted);
    }

    public static Size CalcImageSizeWithMaxEdge(
      Size originalSize,
      double maxEdgeSize,
      out bool adjusted)
    {
      adjusted = false;
      double width = originalSize.Width;
      double height = originalSize.Height;
      double num1 = Math.Max(width, height);
      if (num1 > maxEdgeSize)
      {
        double num2 = maxEdgeSize / num1;
        width *= num2;
        height *= num2;
        adjusted = true;
      }
      Size size = new Size(width, height);
      if (adjusted)
        Log.WriteLineDebug("scaling: [{0},{1}] -> [{2},{3}]", (object) (int) originalSize.Width, (object) (int) originalSize.Height, (object) (int) size.Width, (object) (int) size.Height);
      return size;
    }

    public static IObservable<Message> CreateMessageWithPicture(
      List<string> jids,
      Stream jpegStream,
      byte[] largeThumbBytes,
      byte[] thumbBytes,
      string fullpath = null,
      string caption = null,
      Action<Message> messageModifier = null)
    {
      jpegStream.Position = 0L;
      MediaUpload.MediaDescriptor media = new MediaUpload.MediaDescriptor()
      {
        Extension = "jpg",
        Stream = jpegStream,
        FullPath = fullpath,
        ContentType = "image/jpeg",
        ThumbData = thumbBytes,
        LargeThumbData = largeThumbBytes,
        WaType = FunXMPP.FMessage.Type.Image,
        Caption = caption
      };
      return MediaUpload.CreateMessageWithMedia(jids, media, messageModifier);
    }

    public static IObservable<Message> CreateMessageWithAudio(
      List<string> jids,
      WaAudioArgs args,
      bool isPtt,
      StreamingUploadContext ctx = null)
    {
      if (ctx == null)
      {
        try
        {
          string mimeType = CodecDetector.DetectAudioCodec(args.Stream).MimeType;
          if (mimeType != null)
          {
            args.MimeType = mimeType;
            Log.l("codec detector", "Detected mime type was {0}", (object) args.MimeType);
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "codec detector");
        }
      }
      MediaUpload.MediaDescriptor media = new MediaUpload.MediaDescriptor()
      {
        Extension = args.FileExtension,
        ContentType = args.MimeType,
        Stream = args.Stream,
        ThumbData = args.Thumbnail,
        WaType = FunXMPP.FMessage.Type.Audio,
        IsLive = isPtt,
        StreamingUploadContext = ctx
      };
      bool transcode = !((IEnumerable<string>) WhatsApp.ProtoBuf.Message.SupportedAudioTypes).Contains<string>(args.MimeType);
      return MediaUpload.CreateMessageWithMedia(jids, media, (Action<Message>) (msg =>
      {
        msg.MediaDurationSeconds = args.Duration;
        msg.SetQuote(args.QuotedMessage, args.QuotedChat);
        msg.SetC2cFlags(args.C2cStarted);
        if (ctx != null || !transcode)
          return;
        MessageMiscInfo misc = msg.GetMiscInfo();
        if (misc == null)
        {
          misc = new MessageMiscInfo();
          msg.SetMiscInfo(misc);
        }
        misc.TranscoderData = new TranscodeArgs().Serialize();
        msg.Status = FunXMPP.FMessage.Status.Pending;
      }), ctx, !transcode);
    }

    public static IObservable<Message> CreateMessageWithVideo(List<string> jids, WaVideoArgs args)
    {
      MediaUpload.MediaDescriptor media = new MediaUpload.MediaDescriptor()
      {
        Extension = args.FileExtension,
        ContentType = args.ContentType ?? "video/mp4",
        Stream = args.Stream,
        FullPath = args.FullPath,
        TempFileName = !args.IsCameraVideo || args.FullPath == null ? args.PreviewPlayPath : (string) null,
        WaType = args.LoopingPlayback ? FunXMPP.FMessage.Type.Gif : FunXMPP.FMessage.Type.Video,
        Caption = args.Caption
      };
      if (media.FullPath != null && media.TempFileName == NativeMediaStorage.MakeUri(media.FullPath))
        media.TempFileName = (string) null;
      try
      {
        if (args.Thumbnail == null)
        {
          using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(MediaStorage.GetAbsolutePath(media.FullPath ?? media.TempFileName), args.OrientationAngle, new int?(8)))
          {
            using (VideoFrame videoFrame = videoFrameGrabber.ReadFrame())
              args.Thumbnail = videoFrame.Bitmap;
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "thumbnail generator");
      }
      try
      {
        string path = media.FullPath ?? media.TempFileName;
        if (path != null)
        {
          string videoMimeType = CodecDetector.GetVideoMimeType(MediaStorage.GetAbsolutePath(path));
          if (videoMimeType != null)
          {
            media.ContentType = videoMimeType;
            Log.l("codec detector", "Detected mime type was {0}", (object) media.ContentType);
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "codec detector");
      }
      if (media.FullPath != null)
      {
        string fullPath = media.FullPath;
        if (fullPath.IndexOf("temp", StringComparison.InvariantCultureIgnoreCase) > 0 || fullPath.IndexOf("tmp", StringComparison.InvariantCultureIgnoreCase) > 0 || args.ContentType == "image/gif")
          media.FullPath = (string) null;
        if (media.FullPath != null)
        {
          try
          {
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              using (nativeMediaStorage.OpenFile(media.FullPath, FileMode.Open, FileAccess.Read))
                ;
            }
          }
          catch (Exception ex)
          {
            media.FullPath = (string) null;
          }
        }
      }
      if (args.Thumbnail != null)
      {
        MemoryStream d = new MemoryStream();
        try
        {
          double num = 100.0 / (double) Math.Max(args.Thumbnail.PixelHeight, args.Thumbnail.PixelWidth);
          args.Thumbnail.SaveJpegWithMaxSize((Stream) d, (int) ((double) args.Thumbnail.PixelWidth * num), (int) ((double) args.Thumbnail.PixelHeight * num), 0, Settings.JpegQuality, 48128);
          d.Position = 0L;
          media.ThumbData = d.ToArray();
        }
        catch (Exception ex)
        {
        }
        finally
        {
          d.SafeDispose();
        }
      }
      if (args.LargeThumbnail != null)
        media.LargeThumbData = args.LargeThumbnail.ToJpegByteArray(48128);
      if (args.CodecInfo == CodecInfo.Unsupported)
        return Observable.Return<Unit>(new Unit()).ObserveOnDispatcher<Unit>().Where<Unit>((Func<Unit, bool>) (u =>
        {
          AppState.ClientInstance.ShowMessageBox(AppResources.BadCodecNoTranscoder);
          return false;
        })).Select<Unit, Message>((Func<Unit, Message>) (a => new Message()));
      TranscodeReason transcodeReason = args.TranscodeReason;
      bool shouldTranscode = args.ShouldTranscode;
      if (shouldTranscode)
        Log.l("media upload", "need transcode, reasons={0}", (object) transcodeReason.ToString());
      return MediaUpload.CreateMessageWithMedia(jids, media, (Action<Message>) (msg =>
      {
        TimeCrop? timeCrop1 = args.TimeCrop;
        TimeCrop timeCrop2;
        if (JidHelper.IsStatusJid(msg.KeyRemoteJid))
        {
          if (timeCrop1.HasValue)
          {
            timeCrop2 = timeCrop1.Value;
            if (timeCrop2.DesiredDuration.TotalSeconds > (double) Settings.StatusVideoMaxDuration)
            {
              ref TimeCrop? local = ref timeCrop1;
              timeCrop2 = new TimeCrop();
              timeCrop2.StartTime = timeCrop1.Value.StartTime;
              timeCrop2.DesiredDuration = TimeSpan.FromSeconds((double) Settings.StatusVideoMaxDuration);
              TimeCrop timeCrop3 = timeCrop2;
              local = new TimeCrop?(timeCrop3);
            }
          }
          else if (args.Duration > Settings.StatusVideoMaxDuration)
          {
            ref TimeCrop? local = ref timeCrop1;
            timeCrop2 = new TimeCrop();
            timeCrop2.StartTime = TimeSpan.FromSeconds(0.0);
            timeCrop2.DesiredDuration = TimeSpan.FromSeconds((double) Settings.StatusVideoMaxDuration);
            TimeCrop timeCrop4 = timeCrop2;
            local = new TimeCrop?(timeCrop4);
          }
        }
        int num1 = shouldTranscode ? 1 : (timeCrop1.HasValue ? 1 : 0);
        msg.MediaDurationSeconds = args.Duration;
        msg.SetQuote(args.QuotedMessage, args.QuotedChat);
        msg.SetC2cFlags(args.C2cStarted);
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureMediaProperties.GifAttribution = new MessageProperties.MediaProperties.Attribution?(args.GifAttribution);
        forMessage.Save();
        if (num1 == 0)
          return;
        TranscodeArgs transcodeArgs1 = new TranscodeArgs();
        transcodeArgs1.Flags = transcodeReason;
        if (timeCrop1.HasValue)
        {
          TranscodeArgs transcodeArgs2 = transcodeArgs1;
          timeCrop2 = timeCrop1.Value;
          int? nullable1 = new int?((int) timeCrop2.StartTime.TotalMilliseconds);
          transcodeArgs2.StartMilliseconds = nullable1;
          TranscodeArgs transcodeArgs3 = transcodeArgs1;
          timeCrop2 = timeCrop1.Value;
          int? nullable2 = new int?((int) timeCrop2.DesiredDuration.TotalMilliseconds);
          transcodeArgs3.DurationMilliseconds = nullable2;
          msg.MediaDurationSeconds = transcodeArgs1.DurationMilliseconds.Value / 1000;
        }
        if (args.CropRectangle.HasValue)
        {
          transcodeArgs1.XOffset = new int?(args.CropRectangle.Value.XOffset);
          transcodeArgs1.YOffset = new int?(args.CropRectangle.Value.YOffset);
          transcodeArgs1.Width = new int?(args.CropRectangle.Value.Width);
          transcodeArgs1.Height = new int?(args.CropRectangle.Value.Height);
        }
        if (args.OrientationAngle != 0)
        {
          int num2 = JpegUtils.ExifCodeForRotation((360 - args.OrientationAngle % 360) / 90);
          if (num2 != 0)
            transcodeArgs1.Rotation = new int?(num2);
        }
        msg.Status = FunXMPP.FMessage.Status.Pending;
        MessageMiscInfo misc = msg.GetMiscInfo();
        if (misc == null)
        {
          misc = new MessageMiscInfo();
          msg.SetMiscInfo(misc);
        }
        misc.TranscoderData = transcodeArgs1.Serialize();
      }), sizeCheck: !shouldTranscode);
    }

    public static IObservable<Message> CreateMessageWithDocument(
      List<string> jids,
      DocumentMessageUtils.DocumentData docData,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted)
    {
      MediaUpload.MediaDescriptor media = new MediaUpload.MediaDescriptor()
      {
        Extension = docData.FileExtension,
        ContentType = docData.MimeType,
        Stream = docData.Stream,
        WaType = FunXMPP.FMessage.Type.Document
      };
      if (docData.Thumbnail != null)
      {
        MemoryStream d = new MemoryStream();
        try
        {
          double num = 100.0 / (double) Math.Max(docData.Thumbnail.PixelHeight, docData.Thumbnail.PixelWidth);
          docData.Thumbnail.SaveJpegWithMaxSize((Stream) d, (int) ((double) docData.Thumbnail.PixelWidth * num), (int) ((double) docData.Thumbnail.PixelHeight * num), 0, Settings.JpegQuality, 48128);
          d.Position = 0L;
          media.ThumbData = d.ToArray();
        }
        catch (Exception ex)
        {
        }
        finally
        {
          d.SafeDispose();
        }
      }
      return MediaUpload.CreateMessageWithMedia(jids, media, (Action<Message>) (msg =>
      {
        DocumentMessageWrapper documentMessageWrapper = new DocumentMessageWrapper(msg)
        {
          Title = docData.Title,
          PageCount = docData.PageCount,
          Filename = docData.Filename
        };
        msg.SetQuote(quotedMsg, quotedChat);
        msg.SetC2cFlags(c2cStarted);
      }));
    }

    public static IObservable<Message> CreateMessageWithSticker(
      List<string> jids,
      Sticker sticker,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted)
    {
      MediaUpload.MediaDescriptor media = new MediaUpload.MediaDescriptor()
      {
        Extension = "webp",
        Stream = (Stream) sticker.GetThumbnailStream(),
        ContentType = sticker.MimeType,
        WaType = FunXMPP.FMessage.Type.Sticker
      };
      return MediaUpload.CreateMessageWithMedia(jids, media, (Action<Message>) (msg =>
      {
        msg.SetQuote(quotedMsg, quotedChat);
        msg.SetC2cFlags(c2cStarted);
      }));
    }

    private static IObservable<Message> CreateMessageWithMedia(
      List<string> jids,
      MediaUpload.MediaDescriptor media,
      Action<Message> messageModifier,
      StreamingUploadContext streamingCtx = null,
      bool sizeCheck = true)
    {
      IObservable<bool> observable = Observable.Return<bool>(true);
      long sizeInBytes = media.Stream.Length;
      if (sizeCheck && sizeInBytes > (long) Settings.MaxMediaSize)
      {
        long num1 = sizeInBytes / 1048576L;
        int maxMediaSize = Settings.MaxMediaSize;
        int num2 = maxMediaSize / 1048576;
        string prompt = (string) null;
        if (media.WaType == FunXMPP.FMessage.Type.Video)
          prompt = string.Format(num1 == (long) num2 ? AppResources.UploadVideoTooBigNearLimit : AppResources.UploadVideoTooBig, (object) Utils.FileSizeFormatter.Format(sizeInBytes), (object) Utils.FileSizeFormatter.Format((long) maxMediaSize));
        else if (media.WaType == FunXMPP.FMessage.Type.Audio)
          prompt = string.Format(num1 == (long) num2 ? AppResources.UploadAudioTooBigNearLimit : AppResources.UploadAudioTooBig, (object) Utils.FileSizeFormatter.Format(sizeInBytes), (object) Utils.FileSizeFormatter.Format((long) maxMediaSize));
        if (prompt != null)
          observable = AppState.ClientInstance.Decision(observable, prompt, AppResources.Send, AppResources.CancelButton).Do<bool>((Action<bool>) (chosen =>
          {
            if (!chosen)
              return;
            try
            {
              MediaUpload.ProcessMediaTooBig(media);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "media trimmer");
              chosen = false;
            }
          }));
      }
      return observable.Do<bool>((Action<bool>) (chosen =>
      {
        if (chosen)
          return;
        media.Stream.SafeDispose();
        media.Stream = (Stream) null;
        if (media.TempFileName == null)
          return;
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          storeForApplication.DeleteFile(media.TempFileName);
      })).Where<bool>((Func<bool, bool>) (@continue => @continue)).SelectMany<bool, Message, Message>((Func<bool, IObservable<Message>>) (@continue => Observable.Create<Message>((Func<IObserver<Message>, Action>) (observer =>
      {
        MediaUpload.OutgoingMediaInfo outgoingMediaInfo = (MediaUpload.OutgoingMediaInfo) null;
        byte[] numArray = (byte[]) null;
        using (media.Stream)
        {
          Log.l("media upload", "saving outgoing file");
          outgoingMediaInfo = MediaUpload.SaveOutgoingMedia(media, media.Stream, media.TempFileName);
          if (streamingCtx != null)
            numArray = MediaUpload.ComputeHash(media.Stream);
        }
        int num = jids.Count<string>();
        Log.l("media upload", "sending {0} messages", (object) num);
        foreach (string jid in jids)
        {
          Message message = new Message(true)
          {
            Status = FunXMPP.FMessage.Status.Uploading,
            BinaryData = media.ThumbData,
            KeyFromMe = true,
            KeyId = FunXMPP.GenerateMessageId(),
            KeyRemoteJid = jid,
            MediaWaType = media.WaType,
            LocalFileUri = outgoingMediaInfo.LocalFileUri,
            MediaMimeType = media.ContentType,
            UploadContext = (UploadContext) streamingCtx,
            MediaCaption = media.Caption
          };
          if (num > 1)
          {
            MessageProperties forMessage = MessageProperties.GetForMessage(message);
            forMessage.EnsureCommonProperties.Multicast = new bool?(true);
            forMessage.Save();
          }
          if (media.LargeThumbData != null)
            message.SaveBinaryDataFile(media.LargeThumbData);
          if (outgoingMediaInfo.ScaledFileUri != null || outgoingMediaInfo.TargetFilename != null)
          {
            MessageMiscInfo misc = new MessageMiscInfo();
            message.SetMiscInfo(misc);
            misc.TargetFilename = outgoingMediaInfo.TargetFilename;
            misc.AlternateUploadUri = outgoingMediaInfo.ScaledFileUri;
          }
          if (media.IsLive)
            message.MediaOrigin = "live";
          message.MediaSize = sizeInBytes;
          if (streamingCtx != null)
          {
            message.ParticipantsHash = streamingCtx.ParticipantHash;
            message.MediaKey = streamingCtx.MediaKey;
            message.MediaHash = numArray;
          }
          if (messageModifier != null)
            messageModifier(message);
          observer.OnNext(message);
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }))), (Func<bool, Message, Message>) ((@continue, m) => m));
    }

    public static void CreateTempFileForMp4Truncation(out string filename, out Stream stream)
    {
      filename = "tmp\\" + MediaUpload.GenerateMediaFilename("bin");
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        stream = (Stream) storeForApplication.OpenFile(filename, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
    }

    private static void ProcessMediaTooBig(MediaUpload.MediaDescriptor media)
    {
      Log.l("media trimmer", "Media file too large; length = {0}, type = {1}, content-type = {2}", (object) media.Stream.Length, (object) media.WaType.ToString(), (object) media.ContentType);
      string lowerInvariant = media.ContentType.ToLowerInvariant();
      if (lowerInvariant == "audio/mp4" || lowerInvariant == "audio/3gpp" || lowerInvariant == "video/3gpp" || lowerInvariant == "video/mp4")
      {
        if (media.FullPath == null && media.TempFileName == null)
        {
          Log.l("media trimmer", "media was not spec'd by filename, copying to temp file");
          string filename = (string) null;
          Stream stream = (Stream) null;
          try
          {
            MediaUpload.CreateTempFileForMp4Truncation(out filename, out stream);
            media.Stream.CopyTo(stream);
            stream.Position = 0L;
            media.Stream.SafeDispose();
            media.Stream = stream;
            stream = (Stream) null;
            media.TempFileName = filename;
          }
          finally
          {
            stream.SafeDispose();
          }
        }
        using (media.Stream)
        {
          media.Stream = (Stream) null;
          IMp4Utils mp4Utils = NativeInterfaces.Mp4Utils;
          string absolutePath1 = MediaStorage.GetAbsolutePath(media.FullPath ?? media.TempFileName);
          media.FullPath = media.TempFileName = (string) null;
          MediaUpload.CreateTempFileForMp4Truncation(out media.TempFileName, out media.Stream);
          string absolutePath2 = MediaStorage.GetAbsolutePath(media.TempFileName);
          string InputPath = absolutePath1;
          string OutputPath = absolutePath2;
          long maxMediaSize = (long) Settings.MaxMediaSize;
          mp4Utils.TrimMp4File(InputPath, OutputPath, maxMediaSize);
        }
      }
      else
      {
        Log.l("media trimmer", "not an mp4 file; using crude truncation method");
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          NativeStream tempFile = nativeMediaStorage.GetTempFile();
          byte[] buffer = new byte[8192];
          int maxMediaSize = Settings.MaxMediaSize;
          while (maxMediaSize != 0)
          {
            int count = media.Stream.Read(buffer, 0, buffer.Length);
            if (count != 0)
            {
              tempFile.Write(buffer, 0, count);
              if (count < buffer.Length)
                break;
            }
            else
              break;
          }
          tempFile.Position = 0L;
          media.Stream = (Stream) tempFile;
          media.TempFileName = media.FullPath = (string) null;
        }
      }
    }

    public static IObservable<Unit> SendMediaObservable(Message msg, bool webRetry = false)
    {
      if (!(msg.KeyRemoteJid == Settings.MyJid))
        return MediaDownload.RateLimit<Unit>(Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          object @lock = new object();
          bool cancel = false;
          IDisposable disp = (IDisposable) null;
          Log.d("RateLimit", "queuing sendObservable {0}", (object) msg.KeyId);
          WAThreadPool.QueueUserWorkItem((Action) (() =>
          {
            try
            {
              lock (@lock)
              {
                if (cancel)
                  return;
              }
              Log.d("RateLimit", "creating sendObservable {0}", (object) msg.KeyId);
              IObservable<Unit> source = Mms4Helper.IsMms4UploadMessage(msg) ? MediaUploadMms4.SendMediaObservableImplMms4(msg, webRetry) : MediaUpload.SendMediaObservableImpl(msg, webRetry);
              Log.d("RateLimit", "got sendObservable {0}", (object) msg.KeyId);
              lock (@lock)
              {
                if (cancel)
                  return;
                disp = source.ObserveOn<Unit>(WAThreadPool.Scheduler).SubscribeOn<Unit>(WAThreadPool.Scheduler).Subscribe(observer);
                Log.d("RateLimit", "subscribed sendObservable {0}", (object) msg.KeyId);
              }
            }
            catch (Exception ex)
            {
              observer.OnError(ex);
              observer.OnCompleted();
            }
          }));
          return (Action) (() =>
          {
            Log.d("RateLimit", "dispose sendObservable {0} {1}", (object) msg.KeyId, (object) (disp != null));
            lock (@lock)
            {
              Log.l("RateLimit", "disposing sendObservable {0} {1}", (object) msg.KeyId, (object) (disp != null));
              disp?.Dispose();
              cancel = true;
            }
            Log.d("RateLimit", "disposed sendObservable {0} {1}", (object) msg.KeyId, (object) (disp != null));
            observer.OnCompleted();
          });
        })).Take<Unit>(1));
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.Status = FunXMPP.FMessage.Status.ReadByTarget;
        db.SubmitChanges();
      }));
      return Observable.Empty<Unit>();
    }

    public static string GetAndSanitizeFilePath(
      MessagesContext db,
      Message msg,
      ref string altName)
    {
      bool flag1 = false;
      bool flag2 = false;
      string str = (string) null;
      if (msg.LocalFileUri != null && !msg.LocalFileExists())
      {
        msg.LocalFileUri = (string) null;
        flag1 = true;
      }
      MessageMiscInfo miscInfo = msg.GetMiscInfo((SqliteMessagesContext) db);
      if (miscInfo != null && miscInfo.AlternateUploadUri != null)
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(miscInfo.AlternateUploadUri))
        {
          if (!mediaStorage.FileExists(miscInfo.AlternateUploadUri))
          {
            miscInfo.AlternateUploadUri = (string) null;
            flag1 = true;
            flag2 = false;
          }
        }
      }
      if (flag1)
        db.SubmitChanges();
      if (miscInfo != null)
      {
        str = miscInfo.AlternateUploadUri;
        altName = miscInfo.TargetFilename;
      }
      string sanitizeFilePath = str ?? msg.LocalFileUri;
      if (flag2 && msg.LocalFileUri != null && msg.MediaWaType == FunXMPP.FMessage.Type.Image)
        sanitizeFilePath = (string) null;
      return sanitizeFilePath;
    }

    private static IObservable<Unit> SendMediaObservableImpl(Message msg, bool webRetry = false)
    {
      bool cancel = false;
      string localUri = (string) null;
      string altName = (string) null;
      AxolotlMediaCipher mediaCipher = (AxolotlMediaCipher) null;
      byte[] cipherMediaHash = (byte[]) null;
      WhatsApp.Events.MediaUpload fsEvent = FieldStats.GetFsMediaUploadEvent(msg);
      Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>> getUploadObservable = (Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) null;
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
        OptimisticJpegUploadContext optMsgContext = msg.UploadContext as OptimisticJpegUploadContext;
        Log.l("OPU", "using optimistically uploaded file | OuId={0}", (object) optMsgContext.OuId);
        mediaCipher = optMsgContext.MediaCipher;
        cipherMediaHash = optMsgContext.MediaCipherHash;
        msg.UploadContext = (UploadContext) null;
        IObservable<FunXMPP.Connection.UploadResult> uploadResumeObservable = Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
        {
          AppState.SchedulePersistentAction(PersistentAction.ResumeUpload(Convert.ToBase64String(cipherMediaHash), mediaCipher.MediaResumeUrl, mediaCipher.GenerateUploadRefs(), optMsgContext.OuId));
          observer.OnNext(optMsgContext.uploadResult);
          return (Action) (() => { });
        }));
        return connection.ConnectedObservable().SelectMany<Unit, FunXMPP.Connection.UploadResult, Unit>((Func<Unit, IObservable<FunXMPP.Connection.UploadResult>>) (_ => uploadResumeObservable.Do<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (resp => MediaUpload.ProcessUploadResponse(msg, mediaCipher, resp, fsEvent)), (Action<Exception>) (ex => Log.SendCrashLog(ex, "Exception processing optimistically uploaded file")))), (Func<Unit, FunXMPP.Connection.UploadResult, Unit>) ((_, resumeResponse) => new Unit())).Do<Unit>((Action<Unit>) (_ => fsEvent.SaveEvent()), (Action<Exception>) (ex => fsEvent.SaveEvent()));
      }
      Func<Exception, WhatsApp.Events.MediaUpload, Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>> func = (Func<Exception, WhatsApp.Events.MediaUpload, Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>>) ((err, fsUpEvent) => (Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) (p => Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
      {
        observer.OnError(err);
        observer.OnCompleted();
        FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_FNF);
        return (Action) (() => { });
      }))));
      if (localUri == null)
      {
        getUploadObservable = func((Exception) new FunXMPP.Connection.UnforwardableMessageException(), fsEvent);
      }
      else
      {
        try
        {
          MediaUpload.Mp4CheckAndRepair(msg, ref localUri);
        }
        catch (CheckAndRepairException ex)
        {
          getUploadObservable = func((Exception) ex, fsEvent);
        }
        if (getUploadObservable == null)
        {
          if (msg.MediaHash == null)
          {
            byte[] generatedHash = (byte[]) null;
            using (IMediaStorage mediaStorage = MediaStorage.Create(localUri))
            {
              using (Stream stream = mediaStorage.OpenFile(localUri))
                generatedHash = MediaUpload.ComputeHash(stream);
            }
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              msg.MediaHash = generatedHash;
              msg.ClearCipherMediaHash(db);
              db.SubmitChanges();
            }));
          }
          getUploadObservable = (Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) (resp => Observable.Defer<FunXMPP.Connection.UploadResult>((Func<IObservable<FunXMPP.Connection.UploadResult>>) (() =>
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
            return MediaUpload.GetUploadObservableOnDisk(msg1, localUri1, basename, uploadUrl, resumeFrom, mediaCipher1, fsEvent1, ipHint: ipHint2, webRetry: num != 0);
          })));
        }
      }
      byte[] mediaHash = msg.MediaHash;
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
      IObservable<FunXMPP.Connection.UploadResult> uploadRequestObservable = (IObservable<FunXMPP.Connection.UploadResult>) null;
      mediaCipher = AxolotlMediaCipher.CreateUploadCipher(msg, connection.Encryption, webRetry);
      if (mediaCipher != null)
      {
        byte[] cipherMediaHash1 = localUri != null ? cipherMediaHash : (byte[]) null;
        uploadRequestObservable = MediaUpload.SendEncryptedUploadRequest(mediaCipher.UploadHash, new long?(), msg.GetFunMediaType(), cipherMediaHash1, mediaCipher, fsEvent);
        if (msg.UploadContext.isOptimisticUpload())
          ((OptimisticJpegUploadContext) msg.UploadContext).PersonalRef = mediaCipher.GenerateMediaRef(Settings.MyJid);
        return connection.ConnectedObservable().SelectMany((Func<Unit, IObservable<FunXMPP.Connection.UploadResult>>) (_ => Observable.Defer<FunXMPP.Connection.UploadResult>((Func<IObservable<FunXMPP.Connection.UploadResult>>) (() => uploadRequestObservable.ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker)))), (_, uploadIqResponse) => new
        {
          _ = _,
          uploadIqResponse = uploadIqResponse
        }).SelectMany(_param1 => Observable.If<FunXMPP.Connection.UploadResult>((Func<bool>) (() => _param1.uploadIqResponse.UploadUrl == null), Observable.Return<FunXMPP.Connection.UploadResult>(_param1.uploadIqResponse), Observable.Defer<FunXMPP.Connection.UploadResult>((Func<IObservable<FunXMPP.Connection.UploadResult>>) (() => getUploadObservable(_param1.uploadIqResponse)))).ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker).Do<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (resp => MediaUpload.ProcessUploadResponse(msg, mediaCipher, resp, fsEvent))), (_param1, uploadResult) => new Unit()).Catch<Unit, Exception>((Func<Exception, IObservable<Unit>>) (ex =>
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
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg.Status = FunXMPP.FMessage.Status.Error;
        db.SubmitChanges();
      }));
      FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_OOM);
      return Observable.Empty<Unit>();
    }

    public static bool TryGetFriendlyErrorMessage(
      FunXMPP.FMessage.Type type,
      Exception ex,
      out string uiMsg)
    {
      switch (ex)
      {
        case FunXMPP.Connection.UnforwardableMessageException _:
          uiMsg = AppResources.BadForward;
          return true;
        case CheckAndRepairException _:
          switch (type)
          {
            case FunXMPP.FMessage.Type.Audio:
              uiMsg = AppResources.Mp4CheckFailureAudio;
              break;
            case FunXMPP.FMessage.Type.Video:
            case FunXMPP.FMessage.Type.Gif:
              uiMsg = AppResources.Mp4CheckFailureVideo;
              break;
            default:
              uiMsg = AppResources.MediaUploadInvalidType;
              break;
          }
          return true;
        case MediaUploadException mediaUploadException:
          if (mediaUploadException.ResponseCode == 415)
          {
            uiMsg = AppResources.MediaUploadInvalidType;
            return true;
          }
          break;
      }
      uiMsg = (string) null;
      return false;
    }

    private static IObservable<FunXMPP.Connection.UploadResult> SendEncryptedUploadRequest(
      byte[] uploadHash,
      long? size,
      FunXMPP.FMessage.FunMediaType mediaWaType,
      byte[] cipherMediaHash,
      AxolotlMediaCipher mediaCipher,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      return mediaCipher.MediaResumeUrl != null ? MediaUpload.SendResumeRequest(Convert.ToBase64String(cipherMediaHash ?? uploadHash), mediaCipher.MediaResumeUrl, mediaCipher.GenerateUploadRefs(), fsEvent) : AppState.ClientInstance.GetConnection().SendUploadRequest(uploadHash, size, mediaWaType, (byte[]) null, fsEvent).ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker).Do<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (iqResultTemp =>
      {
        if (cipherMediaHash == null)
          return;
        mediaCipher.MediaResumeUrl = iqResultTemp.UploadUrl;
      })).SelectMany<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>((Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) (iqResult => Observable.If<FunXMPP.Connection.UploadResult>((Func<bool>) (() => cipherMediaHash == null), Observable.Return<FunXMPP.Connection.UploadResult>(iqResult), Observable.Defer<FunXMPP.Connection.UploadResult>((Func<IObservable<FunXMPP.Connection.UploadResult>>) (() => MediaUpload.SendResumeRequest(Convert.ToBase64String(cipherMediaHash), mediaCipher.MediaResumeUrl, mediaCipher.GenerateUploadRefs(), fsEvent).ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker))))), (Func<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>) ((iqResult, uploadResult) => uploadResult));
    }

    private static void ApplyGifNormalizations(
      Message msg,
      ref string localUri,
      List<string> oldFiles)
    {
      IMp4Utils mp4Utils = NativeInterfaces.Mp4Utils;
      if (CodecDetector.DetectMp4Codecs(localUri).AudioStreamType != Mp4AudioStreamType.NotFound)
      {
        string path = MediaDownload.GetDirectoryPath() + "/" + MediaUpload.GenerateMediaFilename(localUri.ExtractFileExtension());
        IMp4TrackRemover o = mp4Utils.OpenTrackRemover(MediaStorage.GetAbsolutePath(localUri));
        List<int> source = new List<int>();
        int Index = 0;
        for (int trackCount = o.GetTrackCount(); Index < trackCount; ++Index)
        {
          if (o.GetTrackDescription(Index).StartsWith("Audio", StringComparison.CurrentCultureIgnoreCase))
            source.Add(o.GetTrackId(Index));
        }
        if (source.Any<int>())
        {
          o.RemoveTracks(source.ToArray(), MediaStorage.GetAbsolutePath(path));
          Marshal.ReleaseComObject((object) o);
          oldFiles.Add(localUri);
          localUri = path;
        }
        else
          oldFiles.Add(path);
      }
      if (mp4Utils.IsWaAnimGif(MediaStorage.GetAbsolutePath(localUri)))
        return;
      string path1 = MediaDownload.GetDirectoryPath() + "/" + MediaUpload.GenerateMediaFilename(localUri.ExtractFileExtension());
      mp4Utils.TagWaAnimatedGif(MediaStorage.GetAbsolutePath(localUri), MediaStorage.GetAbsolutePath(path1));
      oldFiles.Add(localUri);
      localUri = path1;
    }

    public static void Mp4CheckAndRepair(Message msg, ref string localUri)
    {
      if (((int) msg.InternalProperties?.MediaPropertiesField?.CheckAndRepairRun ?? 0) != 0)
        return;
      List<string> oldFiles = new List<string>();
      string str = localUri;
      string newName = MediaDownload.GetDirectoryPath() + "/" + MediaUpload.GenerateMediaFilename(localUri.ExtractFileExtension());
      List<Action<Message>> modifications = new List<Action<Message>>();
      NativeInterfaces.Mp4Utils.CheckAndRepair(msg, ref localUri, newName, false, oldFiles, modifications, false);
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Gif)
        MediaUpload.ApplyGifNormalizations(msg, ref localUri, oldFiles);
      if (localUri != str)
      {
        oldFiles.Remove(str);
        newName = localUri;
        using (Stream stream = MediaStorage.OpenFile(localUri))
        {
          byte[] hash = MediaUpload.ComputeHash(stream);
          long length = stream.Length;
          modifications.Add((Action<Message>) (m =>
          {
            m.MediaSize = length;
            m.MediaHash = hash;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => m.ClearCipherMediaHash(db)));
          }));
        }
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          modifications.ForEach((Action<Action<Message>>) (f => f(msg)));
          db.LocalFileRelease(msg.LocalFileUri, LocalFileType.MessageMedia);
          db.LocalFileAddRef(newName, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
          msg.LocalFileUri = newName;
          MessageProperties messageProperties = msg.InternalProperties ?? new MessageProperties();
          if (messageProperties.MediaPropertiesField == null)
            messageProperties.MediaPropertiesField = new MessageProperties.MediaProperties();
          messageProperties.MediaPropertiesField.CheckAndRepairRun = new bool?(true);
          msg.InternalProperties = messageProperties;
          db.SubmitChanges();
        }));
      }
      foreach (string path in oldFiles)
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(path))
        {
          try
          {
            mediaStorage.DeleteFile(path);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    public static IObservable<FunXMPP.Connection.UploadResult> SendResumeRequest(
      string hashBase64,
      string mediaCiperResumeUrl,
      string mediaCipherRefs,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      string resumeUrl = mediaCiperResumeUrl;
      FieldStats.SetHostDetailsInUploadEvent(fsEvent, resumeUrl);
      IObservable<FunXMPP.Connection.UploadResult> source = Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
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
          string url = resumeUrl;
          MultiPartUploader.FormData[] formData = new MultiPartUploader.FormData[3]
          {
            (MultiPartUploader.FormData) new MultiPartUploader.FormDataString()
            {
              Name = "resume",
              Content = "31"
            },
            (MultiPartUploader.FormData) new MultiPartUploader.FormDataString()
            {
              Name = "hash",
              Content = hashBase64
            },
            (MultiPartUploader.FormData) new MultiPartUploader.FormDataString()
            {
              Name = "refs",
              Content = mediaCipherRefs
            }
          };
          uploadSub = MultiPartUploader.Open(url, formData).ObserveOn<MultiPartUploader.Args>((IScheduler) AppState.Worker).Subscribe<MultiPartUploader.Args>((Action<MultiPartUploader.Args>) (args =>
          {
            if (args.Result == null)
              return;
            using (Stream result3 = args.Result)
            {
              try
              {
                MediaUpload.Result result4 = MediaUpload.ParseResult(result3);
                if (result4 == null)
                  observer.OnError((Exception) new MediaUploadException("Media upload response was null", statusCode: args.ResponseCode));
                else if (result4.Error != null)
                {
                  observer.OnError((Exception) new MediaUploadException("Media upload returned " + result4.Error, statusCode: args.ResponseCode));
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
                  if (result4.Resume.HasValue)
                  {
                    IObserver<FunXMPP.Connection.UploadResult> observer1 = observer;
                    observer1.OnNext(new FunXMPP.Connection.UploadResult()
                    {
                      ResumeFrom = result4.Resume.Value,
                      UploadUrl = mediaCiperResumeUrl
                    });
                  }
                  else
                  {
                    FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.OK);
                    IObserver<FunXMPP.Connection.UploadResult> observer2 = observer;
                    observer2.OnNext(new FunXMPP.Connection.UploadResult()
                    {
                      DownloadUrl = result4.Url,
                      MimeType = result4.MimeType,
                      MediaName = result4.Name,
                      FileSize = result4.Size,
                      Hash = result4.FileHash,
                      DurationSeconds = result4.DurationSeconds
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
      return source.Catch<FunXMPP.Connection.UploadResult, Exception>((Func<Exception, IObservable<FunXMPP.Connection.UploadResult>>) (ex =>
      {
        if (!(ex is MediaUploadException mediaUploadException2) || mediaUploadException2.ResponseCode != 0)
          return Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
          {
            observer.OnError(ex);
            observer.OnCompleted();
            return (Action) (() => { });
          }));
        resumeUrl = MediaDownload.ReplaceUrlHostname(resumeUrl, "mms.whatsapp.net");
        return source;
      }));
    }

    public static IObservable<Unit> ResumePersistentAction(
      string hashBase64,
      string mediaCiperResumeUrl,
      string mediaCipherRefs)
    {
      IObservable<FunXMPP.Connection.UploadResult> resume = MediaUpload.SendResumeRequest(hashBase64, mediaCiperResumeUrl, mediaCipherRefs, (WhatsApp.Events.MediaUpload) null);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        resume.Subscribe<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (resp =>
        {
          if (resp.MimeType == null)
            Log.l("OPU", "response persistent action failure");
          else
            Log.l("OPU", "response persistent worked!");
          observer.OnNext(new Unit());
        }), (Action<Exception>) (ex =>
        {
          Log.l("OPU", "response persistent action exception");
          observer.OnError(ex);
        }));
        return (Action) (() => { });
      }));
    }

    public static byte[] ComputeHash(Stream stream)
    {
      using (SHA256Managed shA256Managed = new SHA256Managed())
        return shA256Managed.ComputeHash(stream);
    }

    public static void StreamMedia(
      StreamingUploadContext context,
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
        context.TransferSubscription = connection.SendUploadRequest(context.Hash, new long?(0L), mediaType, (byte[]) null, streaming: true).SelectMany<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>((Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) (res => MediaUpload.GetUploadObservableStreaming(res.UploadUrl, res.IpHint, remoteJid, mediaCipher, contentType, extension, context.AsObservable())), (Func<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>) ((res, upload) => upload)).ObserveUntilLeavingFg<FunXMPP.Connection.UploadResult>().Subscribe<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (resp => context.UploadResult = resp), (Action<Exception>) (ex => context.OnUploadError(ex)), (Action) (() => context.TransferSubscription?.Dispose()));
      }
      catch (Exception ex)
      {
        context.OnUploadError(ex);
      }
    }

    private static string FilterUrl(string url) => url + "?f=j";

    public static void CopyUploadResponseFields(
      Message msg,
      AxolotlMediaCipher mediaCipher,
      FunXMPP.Connection.UploadResult resp,
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
      byte[] sidecar = mediaCipher.Sidecar;
      if (sidecar != null)
      {
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureMediaProperties.Sidecar = sidecar;
        forMessage.Save();
      }
      if (!resp.DurationSeconds.HasValue || msg.MediaDurationSeconds != 0)
        return;
      msg.MediaDurationSeconds = resp.DurationSeconds.Value;
    }

    public static void CopyMediaFields(Message msg, Message src)
    {
      msg.MediaMimeType = src.MediaMimeType;
      msg.MediaUrl = src.MediaUrl;
      msg.MediaSize = src.MediaSize;
      msg.MediaName = src.MediaName;
      msg.MediaHash = src.MediaHash;
      msg.MediaKey = src.MediaKey;
      byte[] sidecar = MessageProperties.GetForMessage(src).MediaPropertiesField?.Sidecar;
      if (sidecar == null)
        return;
      MessageProperties forMessage = MessageProperties.GetForMessage(msg);
      forMessage.EnsureMediaProperties.Sidecar = sidecar;
      forMessage.Save();
    }

    internal static void ProcessUploadResponse(
      Message m,
      AxolotlMediaCipher mediaCipher,
      FunXMPP.Connection.UploadResult resp,
      WhatsApp.Events.MediaUpload fsEvent)
    {
      if (m.UploadContext.isOptimisticUpload())
      {
        OptimisticJpegUploadContext uploadContext = (OptimisticJpegUploadContext) m.UploadContext;
        uploadContext.uploadResult = resp;
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
            MediaUpload.CopyUploadResponseFields(msg, mediaCipher, resp, db);
            db.SubmitChanges();
            Log.WriteLineDebug("media upload: complete | download_url={0}", (object) MediaDownload.RedactUrl(resp.DownloadUrl));
          }
        }));
        if (skipSend)
        {
          if (msg == null)
            Log.WriteLineDebug("media send: msg deleted before upload complete | id={0}", (object) m.MessageID);
          else
            Log.WriteLineDebug("media send: skip sending msg after upload complete | id={0}", (object) msg.MessageID);
        }
        else
        {
          Log.WriteLineDebug("media send: send msg after upload complete | id={0}", (object) msg.MessageID);
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

    public static IObservable<FunXMPP.Connection.UploadResult> GetUploadObservableForResend(
      Message msg,
      string participant,
      bool fromWeb,
      int? attempts = null)
    {
      WhatsApp.Events.MediaUpload fsEvent = FieldStats.GetFsMediaUploadEvent(msg);
      WhatsApp.Events.MediaUpload mediaUpload = fsEvent;
      int? nullable1 = attempts;
      long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
      mediaUpload.retryCount = nullable2;
      string altName = (string) null;
      string localUri = (string) null;
      byte[] hash = (byte[]) null;
      long? nullable3 = new long?();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => localUri = MediaUpload.GetAndSanitizeFilePath(db, msg, ref altName)));
      if (localUri != null)
      {
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(localUri))
          {
            using (Stream stream = mediaStorage.OpenFile(localUri))
            {
              nullable3 = new long?(stream.Length);
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
        return (IObservable<FunXMPP.Connection.UploadResult>) null;
      msg.NotifyTransfer = false;
      Axolotl encryption = AppState.GetConnection().Encryption;
      AxolotlMediaCipher mediaCipher = AxolotlMediaCipher.CreateUploadCipher(msg, encryption, fromWeb);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => hash = msg.GetCipherMediaHash(db)));
      if (hash == null)
        hash = mediaCipher.UploadHash;
      long? size = new long?();
      byte[] cipherMediaHash = (byte[]) null;
      IObservable<FunXMPP.Connection.UploadResult> source;
      if (fromWeb || mediaCipher.Participants != null && mediaCipher.Participants.Contains<string>(participant))
      {
        source = MediaUpload.SendEncryptedUploadRequest(hash, size, msg.GetFunMediaType(), cipherMediaHash, mediaCipher, fsEvent);
      }
      else
      {
        FunXMPP.Connection.UploadResult uploadResult = new FunXMPP.Connection.UploadResult();
        uploadResult.SkipUpload = true;
        source = Observable.Return<FunXMPP.Connection.UploadResult>(uploadResult);
      }
      return source.ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker).SelectMany<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>((Func<FunXMPP.Connection.UploadResult, IObservable<FunXMPP.Connection.UploadResult>>) (uploadIqResponse => Observable.If<FunXMPP.Connection.UploadResult>((Func<bool>) (() => uploadIqResponse.SkipUpload || uploadIqResponse.UploadUrl == null), Observable.Return<FunXMPP.Connection.UploadResult>(uploadIqResponse), Observable.Defer<FunXMPP.Connection.UploadResult>((Func<IObservable<FunXMPP.Connection.UploadResult>>) (() =>
      {
        Message msg1 = msg;
        string localUri1 = localUri;
        string basename = altName;
        string uploadUrl = uploadIqResponse.UploadUrl;
        string ipHint1 = uploadIqResponse.IpHint;
        long resumeFrom = uploadIqResponse.ResumeFrom;
        AxolotlMediaCipher mediaCipher1 = mediaCipher;
        WhatsApp.Events.MediaUpload fsEvent1 = fsEvent;
        string ipHint2 = ipHint1;
        int num = fromWeb ? 1 : 0;
        return MediaUpload.GetUploadObservableOnDisk(msg1, localUri1, basename, uploadUrl, resumeFrom, mediaCipher1, fsEvent1, false, ipHint2, num != 0);
      })).ObserveOn<FunXMPP.Connection.UploadResult>((IScheduler) AppState.Worker))), (Func<FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult, FunXMPP.Connection.UploadResult>) ((uploadIqResponse, uploadResult) => uploadResult)).Where<FunXMPP.Connection.UploadResult>((Func<FunXMPP.Connection.UploadResult, bool>) (res =>
      {
        if (!res.SkipUpload)
        {
          try
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              MediaUpload.CopyUploadResponseFields(msg, mediaCipher, res, db);
              db.SubmitChanges();
            }));
          }
          catch (DatabaseInvalidatedException ex)
          {
            return false;
          }
        }
        return true;
      })).Do<FunXMPP.Connection.UploadResult>((Action<FunXMPP.Connection.UploadResult>) (_ => fsEvent.SaveEvent()), (Action<Exception>) (ex => fsEvent.SaveEvent()));
    }

    private static IObservable<FunXMPP.Connection.UploadResult> GetUploadObservableOnDisk(
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
      List<MultiPartUploader.FormData> nodes = new List<MultiPartUploader.FormData>();
      if (stream.Length < resumeAt)
      {
        Log.l("media upload", "Resetting resume point since server value {0} exceeds input size {1}", (object) resumeAt, (object) stream.Length);
        resumeAt = -1L;
      }
      Log.WriteLineDebug("media upload: starting upload from offset {0} | msg_id={1} {2} {3}", (object) resumeAt, (object) msg.MessageID, (object) msg.UploadContext.isOptimisticUpload(), (object) stream.Length);
      string str1 = (string) null;
      string str2 = "application/octet-stream";
      bool flag = true;
      mediaCipher.MediaResumeUrl = postUrl;
      if (resumeAt >= 0L && fsEvent != null)
        fsEvent.uploadResumePoint = new long?((long) (int) resumeAt);
      List<MultiPartUploader.FormData> formDataList = nodes;
      MultiPartUploader.FormDataFile formDataFile1 = new MultiPartUploader.FormDataFile();
      formDataFile1.Filename = str1;
      formDataFile1.Name = "file";
      formDataFile1.ContentType = str2;
      formDataFile1.Content = stream;
      formDataFile1.Offset = Math.Max(0L, resumeAt);
      formDataFile1.ContentRange = resumeAt > 0L ? string.Format("bytes {0}-*/*", (object) resumeAt) : (string) null;
      formDataFile1.MediaCipher = mediaCipher;
      formDataList.Add((MultiPartUploader.FormData) formDataFile1);
      MediaUpload.AddE2eNodes(nodes, mediaCipher, webRetry);
      foreach (MultiPartUploader.FormData formData in nodes)
      {
        if (formData is MultiPartUploader.FormDataFile formDataFile2)
          Log.WriteLineDebug("media upload: offset={0}, range={1}", (object) formDataFile2.Offset, (object) (formDataFile2.ContentRange ?? "*"));
      }
      Action<long, long> action = (Action<long, long>) null;
      if (msg.NotifyTransfer)
      {
        msg.TransferValue = Math.Min(0.5, msg.TransferValue);
        double start = msg.TransferValue;
        action = (Action<long, long>) ((current, total) => msg.TransferValue = Math.Max(start + (total == 0L ? 0.0 : (double) current / (double) total) * (1.0 - start), msg.TransferValue));
      }
      string postUrl1 = postUrl;
      string str3 = ipHint;
      int num = flag ? 1 : 0;
      string keyRemoteJid = msg.KeyRemoteJid;
      string logPrefix = string.Format("id={0} type={1}", (object) msg.MessageID, (object) msg.MediaWaType);
      List<MultiPartUploader.FormData> fileNodes = nodes;
      WhatsApp.Events.MediaUpload fsEvent1 = fsEvent;
      Action releaseFileSource = (Action) (() =>
      {
        stream.Dispose();
        fs.SafeDispose();
      });
      Action<long, long> onProgress = action;
      string ipHint1 = str3;
      return MediaUpload.GetUploadObservable(postUrl1, num != 0, keyRemoteJid, logPrefix, (IEnumerable<MultiPartUploader.FormData>) fileNodes, fsEvent1, releaseFileSource, onProgress, ipHint1);
    }

    private static void AddE2eNodes(
      List<MultiPartUploader.FormData> nodes,
      AxolotlMediaCipher mediaCipher,
      bool webRetry = false)
    {
      if (mediaCipher == null)
        return;
      List<MultiPartUploader.FormData> formDataList1 = nodes;
      MultiPartUploader.FormDataString formDataString1 = new MultiPartUploader.FormDataString();
      formDataString1.Name = "hash";
      formDataString1.Content = (string) null;
      formDataString1.ContentGenerator = (Func<string>) (() => Convert.ToBase64String(mediaCipher.CipherMediaHash));
      formDataList1.Add((MultiPartUploader.FormData) formDataString1);
      List<MultiPartUploader.FormData> formDataList2 = nodes;
      MultiPartUploader.FormDataString formDataString2 = new MultiPartUploader.FormDataString();
      formDataString2.Name = "refs";
      formDataString2.Content = mediaCipher.GenerateUploadRefs(webRetry);
      formDataList2.Add((MultiPartUploader.FormData) formDataString2);
    }

    private static IObservable<FunXMPP.Connection.UploadResult> GetUploadObservableStreaming(
      string postUrl,
      string ipHint,
      string remoteJid,
      AxolotlMediaCipher mediaCipher,
      string contentType,
      string extension,
      IObservable<byte[]> bytes)
    {
      if (mediaCipher != null)
      {
        mediaCipher.MediaResumeUrl = postUrl;
        contentType = "application/octet-stream";
      }
      List<MultiPartUploader.FormData> nodes = new List<MultiPartUploader.FormData>();
      MultiPartUploader.FormDataCancellableAsync cancellableAsync1 = new MultiPartUploader.FormDataCancellableAsync(bytes);
      cancellableAsync1.Filename = MediaUpload.GenerateMediaFilename(extension);
      cancellableAsync1.Name = "file";
      cancellableAsync1.ContentType = contentType;
      cancellableAsync1.MediaCipher = mediaCipher;
      MultiPartUploader.FormDataCancellableAsync cancellableAsync2 = cancellableAsync1;
      nodes.Add((MultiPartUploader.FormData) cancellableAsync2);
      MediaUpload.AddE2eNodes(nodes, mediaCipher);
      string postUrl1 = postUrl;
      string str = ipHint;
      string remoteJid1 = remoteJid;
      List<MultiPartUploader.FormData> fileNodes = nodes;
      Action releaseFileSource = new Action(cancellableAsync2.Terminate);
      string ipHint1 = str;
      return MediaUpload.GetUploadObservable(postUrl1, true, remoteJid1, "(streaming)", (IEnumerable<MultiPartUploader.FormData>) fileNodes, releaseFileSource: releaseFileSource, ipHint: ipHint1);
    }

    private static IObservable<FunXMPP.Connection.UploadResult> GetUploadObservable(
      string postUrl,
      bool chunked,
      string remoteJid,
      string logPrefix,
      IEnumerable<MultiPartUploader.FormData> fileNodes,
      WhatsApp.Events.MediaUpload fsEvent = null,
      Action releaseFileSource = null,
      Action<long, long> onProgress = null,
      string ipHint = null)
    {
      postUrl = MediaUpload.FilterUrl(postUrl);
      return Observable.Create<FunXMPP.Connection.UploadResult>((Func<IObserver<FunXMPP.Connection.UploadResult>, Action>) (observer =>
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
          Log.WriteLineDebug("media upload: release | {0}", (object) logPrefix);
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
          Log.WriteLineDebug("media upload: start | {0} posturl={1}", (object) logPrefix, (object) postUrl);
          FieldStats.SetHostDetailsInUploadEvent(fsEvent, postUrl);
          IObservable<MultiPartUploader.Args> source = MultiPartUploader.Open(postUrl, fileNodes.ToArray<MultiPartUploader.FormData>(), chunked, onProgress != null);
          timer.Start();
          FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.ERROR_UPLOAD);
          uploadSub = source.ObserveOn<MultiPartUploader.Args>((IScheduler) AppState.Worker).Subscribe<MultiPartUploader.Args>((Action<MultiPartUploader.Args>) (args =>
          {
            if (onProgress != null)
              onProgress(args.CurrentProgress, args.TotalProgress);
            if (args.Result == null)
              return;
            timer.Stop();
            if (fsEvent != null)
              fsEvent.mediaUploadT = new long?(timer.ElapsedMilliseconds);
            using (Stream result3 = args.Result)
            {
              try
              {
                MediaUpload.Result result4 = MediaUpload.ParseResult(result3);
                if (result4 == null)
                  observer.OnError((Exception) new MediaUploadException("Media upload response was null", statusCode: args.ResponseCode));
                else if (result4.Error != null)
                {
                  observer.OnError((Exception) new MediaUploadException("Media upload returned " + result4.Error, statusCode: args.ResponseCode));
                }
                else
                {
                  try
                  {
                    Uri uri = new Uri(result4.Url);
                  }
                  catch (Exception ex)
                  {
                    Log.WriteLineDebug("media upload: failed to parse download uri {0}: {1} {2}", (object) MediaDownload.RedactUrl(result4.Url), (object) ex.GetType().Name, (object) (ex.Message ?? ""));
                    observer.OnError((Exception) new MediaUploadException("Failed to parse download URI", ex, args.ResponseCode));
                    return;
                  }
                  Log.WriteLineDebug("media upload: completed | {0} mime-type={1}", (object) logPrefix, (object) result4.MimeType);
                  if (fsEvent != null)
                  {
                    FieldStats.SetResultInUploadEvent(fsEvent, wam_enum_media_upload_result_type.OK);
                    fsEvent.mediaSize = new double?((double) result4.Size);
                    if (args.NetworkTimeMs > 0L)
                      fsEvent.networkUploadT = new long?(args.NetworkTimeMs);
                    if (args.ConnectTimeMs > 0L)
                      fsEvent.connectT = new long?(args.ConnectTimeMs);
                  }
                  IObserver<FunXMPP.Connection.UploadResult> observer1 = observer;
                  observer1.OnNext(new FunXMPP.Connection.UploadResult()
                  {
                    DownloadUrl = result4.Url,
                    MimeType = result4.MimeType,
                    MediaName = result4.Name,
                    FileSize = result4.Size,
                    Hash = result4.FileHash,
                    DurationSeconds = result4.DurationSeconds
                  });
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
    }

    public static MediaUpload.Result ParseResult(Stream resp)
    {
      return new DataContractJsonSerializer(typeof (MediaUpload.Result)).ReadObject(resp) as MediaUpload.Result;
    }

    public static OptimisticJpegUploadContext CreateOptimisticUploadWithPicture(
      string ouId,
      Stream jpegStream,
      string fullpath)
    {
      if (string.IsNullOrEmpty(fullpath))
      {
        Log.l("OPU", "Skip Optimistic Upload - no local file");
        return (OptimisticJpegUploadContext) null;
      }
      OptimisticJpegUploadContext optUpload = Mms4ServerPropHelper.IsMms4EnabledForType(OptimisticJpegUploadContext.JpegFunXMPPMediaType, true) ? (OptimisticJpegUploadContext) new OptimisticJpegUploadContextMms4(ouId, jpegStream) : new OptimisticJpegUploadContext(ouId, jpegStream);
      optUpload.LocalFileUri = fullpath;
      jpegStream.Position = 0L;
      optUpload.Hash = MediaUpload.ComputeHash(jpegStream);
      if (optUpload.Hash == null)
        return (OptimisticJpegUploadContext) null;
      if (MessagesContext.Select<bool>((Func<MessagesContext, bool>) (db => db.GetMessagesWithMediaHash(optUpload.Hash, FunXMPP.FMessage.Type.Image).GetMediaCipherDuplicate((Message) null, db) != null)))
      {
        Log.l("OPU", "Skip optimistic upload - found duplicate");
        return (OptimisticJpegUploadContext) null;
      }
      Log.l("OPU", "Created optimistic upload {0}", optUpload.IsMms4Upload ? (object) "mms4" : (object) "mms3");
      return optUpload;
    }

    public static IObservable<Unit> SendOptimisticMediaObservable(
      OptimisticJpegUploadContext optUploadContext)
    {
      optUploadContext.Message = new Message()
      {
        UploadContext = (UploadContext) optUploadContext,
        KeyRemoteJid = Settings.MyJid,
        KeyFromMe = true,
        MessageID = -1,
        MediaWaType = optUploadContext.MediaWaType,
        MediaMimeType = optUploadContext.MediaMimeType,
        MediaHash = optUploadContext.Hash,
        MediaSize = optUploadContext.Size,
        LocalFileUri = optUploadContext.LocalFileUri
      };
      Log.l("OPU", "sending msg | OuId={0} Id = {1} Mms4:{2}", (object) optUploadContext.OuId, (object) optUploadContext.Message.MessageID, (object) optUploadContext.IsMms4Upload);
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        object @lock = new object();
        bool cancel = false;
        IDisposable disp = (IDisposable) null;
        WAThreadPool.QueueUserWorkItem((Action) (() =>
        {
          try
          {
            lock (@lock)
            {
              if (cancel)
                return;
              Message message = optUploadContext.Message;
              disp = (Mms4Helper.IsMms4UploadMessage(message) ? MediaUploadMms4.SendMediaObservableImplMms4(message) : MediaUpload.SendMediaObservableImpl(message)).ObserveOn<Unit>(WAThreadPool.Scheduler).SubscribeOn<Unit>(WAThreadPool.Scheduler).Subscribe(observer);
            }
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            observer.OnCompleted();
          }
        }));
        return (Action) (() =>
        {
          lock (@lock)
          {
            disp?.Dispose();
            cancel = true;
          }
          observer.OnCompleted();
        });
      }));
    }

    public class MediaDescriptor
    {
      public StreamingUploadContext StreamingUploadContext;
      public string Extension;
      public Stream Stream;
      public string FullPath;
      public string TempFileName;
      public string ContentType;
      public FunXMPP.FMessage.Type WaType;
      public bool IsLive;
      public byte[] ThumbData;
      public byte[] LargeThumbData;
      public string Caption;
    }

    private class OutgoingMediaInfo
    {
      public string LocalFileUri;
      public string TargetFilename;
      public string ScaledFileUri;
    }

    [DataContract]
    public class Result
    {
      [DataMember(Name = "error")]
      public string Error { get; set; }

      [DataMember(Name = "name")]
      public string Name { get; set; }

      [DataMember(Name = "url")]
      public string Url { get; set; }

      [DataMember(Name = "size")]
      public string SizeString { get; set; }

      public long Size
      {
        get
        {
          long result = 0;
          if (this.SizeString != null)
            long.TryParse(this.SizeString, out result);
          return result;
        }
      }

      [DataMember(Name = "duration")]
      public string DurationString { get; set; }

      public int? DurationSeconds
      {
        get
        {
          int? durationSeconds = new int?();
          int result;
          if (this.DurationString != null && int.TryParse(this.DurationString, out result))
            durationSeconds = new int?(result);
          return durationSeconds;
        }
      }

      [DataMember(Name = "mimetype")]
      public string MimeType { get; set; }

      [DataMember(Name = "filehash")]
      public string FileHashString { get; set; }

      public byte[] FileHash
      {
        get
        {
          return this.FileHashString == null ? (byte[]) null : Convert.FromBase64String(this.FileHashString);
        }
      }

      [DataMember(Name = "resume")]
      public string ResumeString { get; set; }

      public long? Resume
      {
        get => this.ResumeString != null ? new long?(long.Parse(this.ResumeString)) : new long?();
      }

      [DataMember(Name = "blob")]
      public string BlobString { get; set; }

      public byte[] Blob
      {
        get => this.BlobString != null ? Convert.FromBase64String(this.BlobString) : (byte[]) null;
      }
    }
  }
}
