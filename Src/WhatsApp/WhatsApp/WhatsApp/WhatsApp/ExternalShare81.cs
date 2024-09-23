// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExternalShare81
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;


namespace WhatsApp
{
  public class ExternalShare81 : IExternalShare
  {
    private const string LogHeader = "external share";
    public static ShareOperation ShareOperation = (ShareOperation) null;
    public ShareOperation ShareOperationInstance;
    private ExternalShare81.SharingTypes sharingType = ExternalShare81.SharingTypes.Unknown;
    public const string SHARE_CONTENT_TYPE_VIDEO = "video";
    public const string SHARE_CONTENT_TYPE_IMAGE = "image";
    public const string SHARE_CONTENT_TYPE_AUDIO = "audio";
    public const string SHARE_CONTENT_TYPE_VCARD = "vcard";
    private static string OPENXML_DOCX_PAGES_REGEX = "<Pages[^>]*?>([0-9]+?)<\\/Pages>";
    private static string OPENXML_PPTX_SLIDES_REGEX = "<Slides[^>]*?>([0-9]+?)<\\/Slides>";
    private static string OPENXML_COUNT_EXTRACT_REGEX = ">([0-9]+?)<";
    private static string OPENXML_PROPERTIES_FILE = "docProps/app.xml";
    private static string OPENXML_THUMBNAIL_FILE = "docProps/thumbnail.jpeg";

    public static bool ShareContentNavigation => ExternalShare81.ShareOperation != null;

    public ExternalShare81()
    {
      this.ShareOperationInstance = ExternalShare81.ShareOperation;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => ExternalShare81.ShareOperation = (ShareOperation) null));
      this.sharingType = ExternalShare81.DetermineSharingType(this.ShareOperationInstance);
    }

    private static ExternalShare81.SharingTypes DetermineSharingType(ShareOperation currentShare)
    {
      DataPackageView data = currentShare?.Data;
      if (data == null)
      {
        Log.l("external share", "No data found");
        return ExternalShare81.SharingTypes.Unknown;
      }
      IReadOnlyList<string> availableFormats = data.AvailableFormats;
      foreach (object availableFormat in (IEnumerable<string>) data.AvailableFormats)
        Log.l("external share", "available data format: {0}", availableFormat);
      if (availableFormats.Contains<string>(StandardDataFormats.Rtf))
        return ExternalShare81.SharingTypes.Rtf;
      if (availableFormats.Contains<string>(StandardDataFormats.Uri) || availableFormats.Contains<string>(StandardDataFormats.WebLink))
        return ExternalShare81.SharingTypes.Uri;
      if (availableFormats.Contains<string>(StandardDataFormats.Bitmap))
        return ExternalShare81.SharingTypes.Bitmap;
      if (availableFormats.Contains<string>(StandardDataFormats.Text))
        return ExternalShare81.SharingTypes.Text;
      return availableFormats.Contains<string>(StandardDataFormats.StorageItems) ? ExternalShare81.SharingTypes.Storage : ExternalShare81.SharingTypes.Unknown;
    }

    private bool IsSingleTextItem()
    {
      return this.sharingType == ExternalShare81.SharingTypes.Rtf || this.sharingType == ExternalShare81.SharingTypes.Text || this.sharingType == ExternalShare81.SharingTypes.Uri;
    }

    public bool ShouldConfirmSending() => !this.IsSingleTextItem();

    public IObservable<bool> GetTruncationCheck()
    {
      DataPackageView data = this.ShareOperationInstance?.Data;
      if (data == null)
        return (IObservable<bool>) null;
      Task<bool> task = (Task<bool>) null;
      switch (this.sharingType)
      {
        case ExternalShare81.SharingTypes.Rtf:
          task = ExternalShare81.CheckRtfString(data, 65536);
          break;
        case ExternalShare81.SharingTypes.Text:
          task = ExternalShare81.CheckDataString(data, 65536);
          break;
        case ExternalShare81.SharingTypes.Uri:
          task = ExternalShare81.CheckUriString(data, 65536);
          break;
      }
      return task == null ? (IObservable<bool>) null : task.ToObservable<bool>();
    }

    private static async Task<bool> CheckUriString(DataPackageView data, int maxLength)
    {
      return (await ExternalShare81.GetUriString(data) ?? "").Length > maxLength;
    }

    private static async Task<bool> CheckRtfString(DataPackageView data, int maxLength)
    {
      return (await data.GetRtfAsync() ?? "").Length > maxLength;
    }

    private static async Task<bool> CheckDataString(DataPackageView data, int maxLength)
    {
      return (await data.GetTextAsync() ?? "").Length > maxLength;
    }

    public IObservable<ExternalShare.ExternalShareResult> ShareContent(List<string> jids)
    {
      return Observable.Defer<ExternalShare.ExternalShareResult>((Func<IObservable<ExternalShare.ExternalShareResult>>) (() => this.ShareContentAsync(jids).ToObservable<ExternalShare.ExternalShareResult>()));
    }

    public string DescribeError(ExternalShare.ExternalShareResult result)
    {
      Log.l("ExternalShare", "Error: {0}", (object) result);
      string notOpenMediaFile;
      if (result == ExternalShare.ExternalShareResult.Discarded || result == ExternalShare.ExternalShareResult.Shared)
      {
        Log.SendCrashLog((Exception) new ArgumentException(string.Format("Unexpected result {0}", (object) result)), nameof (DescribeError));
        notOpenMediaFile = AppResources.CouldNotOpenMediaFile;
      }
      else
        notOpenMediaFile = AppResources.CouldNotOpenMediaFile;
      return notOpenMediaFile;
    }

    public IObservable<bool> ShouldEnableSharingToStatus()
    {
      return Observable.Defer<bool>((Func<IObservable<bool>>) (() => this.ShouldEnableSharingToStatusAsync().ToObservable<bool>()));
    }

    private async Task<bool> ShouldEnableSharingToStatusAsync()
    {
      Log.l("external share", "checking status sharing applicable");
      DataPackageView data = this.ShareOperationInstance?.Data;
      if (data == null)
        return false;
      if (this.sharingType == ExternalShare81.SharingTypes.Bitmap)
        return true;
      if (this.sharingType != ExternalShare81.SharingTypes.Storage)
        return false;
      IReadOnlyList<string> availableFormats = data.AvailableFormats;
      bool r = false;
      IReadOnlyList<IStorageItem> storageItemsAsync = await data.GetStorageItemsAsync();
      if (storageItemsAsync.Count == 1 && storageItemsAsync.FirstOrDefault<IStorageItem>() is IStorageFile istorageFile)
      {
        string contentType = istorageFile.ContentType;
        Log.l("external share", "chekcing status sharing applicable | content type:{0}", (object) contentType);
        if (contentType.Contains("image") || contentType.Contains("video"))
          r = true;
      }
      return r;
    }

    private async Task<ExternalShare.ExternalShareResult> ShareContentAsync(List<string> jids)
    {
      Log.l("external share", "sharing content to {0}", (object) jids);
      ExternalShare.ExternalShareResult shareResult = ExternalShare.ExternalShareResult.None;
      DataPackageView data = this.ShareOperationInstance?.Data;
      if (data == null)
      {
        Log.l("external share", "abort | data is null");
        return ExternalShare.ExternalShareResult.MissingData;
      }
      bool setDraftOnly = this.IsSingleTextItem() && jids.Count < 2;
      List<string> jids1;
      if (this.sharingType == ExternalShare81.SharingTypes.Rtf)
      {
        jids1 = jids;
        string rtfAsync = await data.GetRtfAsync();
        shareResult = ExternalShare.ShareTextContent(jids1, rtfAsync.ToString(), setDraftOnly);
        jids1 = (List<string>) null;
      }
      else if (this.sharingType == ExternalShare81.SharingTypes.Uri)
        shareResult = await ExternalShare81.ShareUriContent(jids, data, setDraftOnly);
      else if (this.sharingType == ExternalShare81.SharingTypes.Bitmap)
      {
        using (IRandomAccessStreamWithContentType photoStream = await (await data.GetBitmapAsync()).OpenReadAsync())
        {
          if (ExternalShareUtils.IsPreviewEnabled())
            return await ExternalShare81.SharePhotoStreamContentAsync(jids, ((IInputStream) photoStream).AsStreamForRead());
          shareResult = ExternalShare81.SharePhotoStreamContent(jids, ((IInputStream) photoStream).AsStreamForRead());
        }
      }
      else if (this.sharingType == ExternalShare81.SharingTypes.Text)
      {
        jids1 = jids;
        string textAsync = await data.GetTextAsync();
        shareResult = ExternalShare.ShareTextContent(jids1, textAsync, setDraftOnly);
        jids1 = (List<string>) null;
      }
      else if (this.sharingType == ExternalShare81.SharingTypes.Storage)
      {
        shareResult = ExternalShare.ExternalShareResult.Shared;
        foreach (IStorageItem istorageItem in (IEnumerable<IStorageItem>) await data.GetStorageItemsAsync())
        {
          if (istorageItem is IStorageFile file)
          {
            ExternalShare.ExternalShareResult externalShareResult = await ExternalShare81.ShareStorageFileAsync(jids, file);
            if (externalShareResult != ExternalShare.ExternalShareResult.Shared)
              shareResult = externalShareResult;
          }
        }
      }
      return shareResult;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareVideoContent(
      List<string> jids,
      IStorageFile file)
    {
      Log.l("external share", "share video");
      ExternalShare.ExternalShareResult r = ExternalShare.ExternalShareResult.Unknown;
      try
      {
        StorageFile cFile = file as StorageFile;
        Log.l("external share", "getting video properties");
        VideoProperties videoProperties = await cFile.Properties.GetVideoPropertiesAsync();
        WriteableBitmap thumbnail = (WriteableBitmap) null;
        Log.l("external share", "getting video thumbnail");
        try
        {
          using (StorageItemThumbnail thumbnailAsync = await cFile.GetThumbnailAsync((ThumbnailMode) 1, 100U))
          {
            if (thumbnailAsync != null)
            {
              if (thumbnailAsync.Type == null)
              {
                BitmapImage source = new BitmapImage();
                source.SetSource(((IRandomAccessStream) thumbnailAsync).AsStream());
                thumbnail = new WriteableBitmap((BitmapSource) source);
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get thumbnail from share contract");
        }
        Log.d("external share", "Orientation: {0}", (object) videoProperties.Orientation);
        WaVideoArgs waVideoArgs1 = new WaVideoArgs();
        waVideoArgs1.FileExtension = file.FileType;
        waVideoArgs1.ContentType = file.ContentType;
        waVideoArgs1.FullPath = ((IStorageItem) file).Path;
        waVideoArgs1.Duration = (int) videoProperties.Duration.TotalSeconds;
        WaVideoArgs waVideoArgs2 = waVideoArgs1;
        Stream stream1 = waVideoArgs2.Stream;
        Stream stream2 = await file.OpenStreamForReadAsync();
        waVideoArgs2.Stream = stream2;
        waVideoArgs1.Thumbnail = thumbnail;
        waVideoArgs1.OrientationAngle = (360 - videoProperties.Orientation) % 360;
        WaVideoArgs args = waVideoArgs1;
        waVideoArgs2 = (WaVideoArgs) null;
        waVideoArgs1 = (WaVideoArgs) null;
        MediaUpload.SendVideo(jids, args);
        r = ExternalShare.ExternalShareResult.Shared;
        cFile = (StorageFile) null;
        videoProperties = (VideoProperties) null;
        thumbnail = (WriteableBitmap) null;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "ShareContract - Video");
        throw new ExternalShareException(AppResources.CouldNotOpenVideo);
      }
      return r;
    }

    private static async Task<ExternalShare.ExternalShareResult> SharePhotoContent(
      List<string> jids,
      IStorageFile file)
    {
      bool isGif = false;
      using (Stream stream = await file.OpenStreamForReadAsync())
      {
        isGif = ExternalShareUtils.IsAnimatedGif(stream);
        if (!isGif)
        {
          if (!ExternalShareUtils.IsPreviewEnabled())
            return ExternalShare81.SharePhotoStreamContent(jids, stream);
        }
      }
      if (isGif && !ExternalShareUtils.IsPreviewEnabled())
      {
        WriteableBitmap thumbnail = (WriteableBitmap) null;
        try
        {
          using (Stream stream = await file.OpenStreamForReadAsync())
            thumbnail = ImageStore.CreateThumbnail((BitmapSource) JpegUtils.DecodeJpeg(stream), 100);
        }
        catch (Exception ex)
        {
        }
        WaVideoArgs waVideoArgs1 = new WaVideoArgs();
        waVideoArgs1.FileExtension = file.FileType;
        waVideoArgs1.ContentType = file.ContentType;
        waVideoArgs1.FullPath = ((IStorageItem) file).Path;
        WaVideoArgs waVideoArgs2 = waVideoArgs1;
        Stream stream1 = waVideoArgs2.Stream;
        Stream stream2 = await file.OpenStreamForReadAsync();
        waVideoArgs2.Stream = stream2;
        waVideoArgs1.CodecInfo = CodecInfo.NeedsTranscode;
        waVideoArgs1.TranscodeReason = TranscodeReason.BadCodec;
        waVideoArgs1.Thumbnail = thumbnail;
        waVideoArgs1.LoopingPlayback = true;
        WaVideoArgs args = waVideoArgs1;
        waVideoArgs2 = (WaVideoArgs) null;
        waVideoArgs1 = (WaVideoArgs) null;
        MediaUpload.SendVideo(jids, args);
        return ExternalShare.ExternalShareResult.Shared;
      }
      if (isGif)
        return await ExternalShare81.ShareGifFileContentAsync(jids, file);
      List<string> jids1 = jids;
      Stream stream3 = await file.OpenStreamForReadAsync();
      return await ExternalShare81.SharePhotoStreamContentAsync(jids1, stream3);
    }

    private static ExternalShare.ExternalShareResult SharePhotoStreamContent(
      List<string> jids,
      Stream photoStream)
    {
      Log.l("external share", "share photo stream");
      try
      {
        using (photoStream)
        {
          WriteableBitmap bitmap = (WriteableBitmap) null;
          Stream jpegStream = MediaUpload.CreateImageStreamAndPicture(photoStream, ref bitmap);
          if (bitmap == null || jpegStream == null)
          {
            Log.l("external share", "SharePhotoStreamContent did not create required data {0} {1}", (object) (bitmap == null), (object) (jpegStream == null));
            throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
          }
          int thumbnailWidth = MessageViewModel.LargeThumbPixelWidth;
          AppState.Worker.Enqueue((Action) (() =>
          {
            using (jpegStream)
              MediaUpload.SendPicture(jids, jpegStream, thumbnailWidth, bitmap);
          }));
          return ExternalShare.ExternalShareResult.Shared;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "external share photo stream");
        throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
      }
    }

    private static async Task<ExternalShare.ExternalShareResult> SharePhotoStreamContentAsync(
      List<string> jids,
      Stream stream)
    {
      Log.l("external share", "share photo async");
      ExternalShare.ExternalShareResult task;
      try
      {
        WriteableBitmap bitmap = (WriteableBitmap) null;
        Stream tempFilestream = (Stream) null;
        using (stream)
        {
          tempFilestream = MediaUpload.CreateImageStreamAndPicture(stream, ref bitmap);
          if (bitmap != null)
          {
            if (tempFilestream != null)
              goto label_8;
          }
          Log.l("external share", "SharePhotoStreamContentAsync did not create required data {0} {1}", (object) (bitmap == null), (object) (tempFilestream == null));
          throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
        }
label_8:
        ExternalSharingState es = new ExternalSharingState();
        es.RecipientJids = jids.ToArray();
        ExternalSharingState.Item obj1 = new ExternalSharingState.Item(tempFilestream);
        es.AddItem((MediaSharingState.IItem) obj1);
        TaskCompletionSource<ExternalShare.ExternalShareResult> tcs = new TaskCompletionSource<ExternalShare.ExternalShareResult>();
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          string returnTopage = (string) null;
          PhoneApplicationPage currentPage = App.CurrentApp.CurrentPage;
          if (currentPage?.NavigationService != null)
            returnTopage = UriUtils.ExtractPageNameFromUrl(currentPage.NavigationService.Source.OriginalString);
          PicturePreviewPage.Start((MediaSharingState) es, true).ObserveOnDispatcher<MediaSharingArgs>().Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
          {
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              if (returnTopage != null)
                App.CurrentApp.RootFrame.JumpBackTo(returnTopage);
              else
                NavUtils.GoBack();
            }));
            if (args.Status == MediaSharingArgs.SharingStatus.Canceled)
            {
              tcs.SetResult(ExternalShare.ExternalShareResult.Discarded);
            }
            else
            {
              if (args.Status != MediaSharingArgs.SharingStatus.Submitted)
                return;
              try
              {
                int thumbnailWidth = MessageViewModel.LargeThumbPixelWidth;
                int imageMaxEdge = Settings.ImageMaxEdge;
                MediaSharingState.IItem obj3 = args.SharingState.SelectedItems.First<MediaSharingState.IItem>();
                string caption = obj3.Caption;
                MediaSharingState.PicInfo picInfo = obj3.ToPicInfo(new Size((double) imageMaxEdge, (double) imageMaxEdge));
                Stream picStream = (Stream) null;
                if (picInfo.isChangedByUser())
                {
                  bitmap = (WriteableBitmap) null;
                  picStream = (Stream) new NativeMediaStorage().GetTempFile();
                  Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
                  {
                    bitmap = picInfo.DrawingBitmapCache ?? picInfo.GetBitmap(withDrawing: true);
                    bitmap.SaveJpeg(picStream, bitmap.PixelWidth, bitmap.PixelHeight, 0, Settings.JpegQuality);
                  }));
                  picStream.Position = 0L;
                }
                else
                  picStream = ((ExternalSharingState.Item) obj3).TakeOverStream();
                AppState.Worker.Enqueue((Action) (() =>
                {
                  try
                  {
                    using (picStream)
                    {
                      MediaUpload.SendPicture(jids, picStream, thumbnailWidth, bitmap, caption: caption, canUseSuppliedStream: true);
                      tcs.SetResult(ExternalShare.ExternalShareResult.Shared);
                    }
                  }
                  catch (Exception ex)
                  {
                    Log.LogException(ex, "Exception sending shared image");
                    tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
                  }
                }));
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "Exception sharing image");
                tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
              }
            }
          }));
        }));
        task = await tcs.Task;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "sendImageThroughPickerPage");
        throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
      }
      return task;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareGifFileContentAsync(
      List<string> jids,
      IStorageFile file)
    {
      Log.l("external share", "share gif async");
      ExternalShare.ExternalShareResult task;
      try
      {
        WriteableBitmap writeableBitmap = (WriteableBitmap) null;
        using (Stream stream = await file.OpenStreamForReadAsync())
        {
          writeableBitmap = ImageStore.CreateThumbnail((BitmapSource) JpegUtils.DecodeJpeg(stream), 100);
          stream.Position = 0L;
        }
        WaVideoArgs baseVideoArgs = new WaVideoArgs()
        {
          FileExtension = file.FileType,
          ContentType = file.ContentType,
          CodecInfo = CodecInfo.NeedsTranscode,
          TranscodeReason = TranscodeReason.BadCodec,
          Thumbnail = writeableBitmap,
          LoopingPlayback = true
        };
        GifArgs gifInfo = new GifArgs()
        {
          GifAttribution = MessageProperties.MediaProperties.Attribution.NONE
        };
        ExternalSharingState es = new ExternalSharingState();
        es.RecipientJids = jids.ToArray();
        ExternalSharingState.Item obj1 = new ExternalSharingState.Item(await file.OpenStreamForReadAsync(), gifInfo, baseVideoArgs);
        es.AddItem((MediaSharingState.IItem) obj1);
        obj1.FetchLargeThumbAsync().Subscribe<WriteableBitmap>();
        TaskCompletionSource<ExternalShare.ExternalShareResult> tcs = new TaskCompletionSource<ExternalShare.ExternalShareResult>();
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() => PicturePreviewPage.Start((MediaSharingState) es, true).ObserveOnDispatcher<MediaSharingArgs>().Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
        {
          if (args.Status == MediaSharingArgs.SharingStatus.Canceled)
          {
            tcs.SetResult(ExternalShare.ExternalShareResult.Discarded);
            args.SharingState.SafeDispose();
          }
          else
          {
            if (args.Status != MediaSharingArgs.SharingStatus.Submitted)
              return;
            try
            {
              int imageMaxEdge = Settings.ImageMaxEdge;
              MediaSharingState.IItem obj2 = args.SharingState.SelectedItems.First<MediaSharingState.IItem>();
              Stream overStream = ((ExternalSharingState.Item) obj2).TakeOverStream();
              WriteableBitmap bitmap1 = obj2.GetBitmap(new Size((double) imageMaxEdge, (double) imageMaxEdge));
              WaVideoArgs editedVideoArgs = new WaVideoArgs()
              {
                FileExtension = ".gif",
                ContentType = "image/gif",
                Stream = overStream,
                CodecInfo = CodecInfo.NeedsTranscode,
                TranscodeReason = TranscodeReason.BadCodec,
                LargeThumbnail = bitmap1,
                Thumbnail = bitmap1,
                OrientationAngle = obj2.RotatedTimes * -90,
                LoopingPlayback = true,
                TimeCrop = obj2.GifInfo.TimeCrop,
                GifAttribution = obj2.GifInfo.GifAttribution,
                Caption = obj2.Caption
              };
              editedVideoArgs.TranscodeReason = TranscodeReason.BadCodec;
              editedVideoArgs.CodecInfo = CodecInfo.NeedsTranscode;
              System.Windows.Point? relativeCropPos = obj2.RelativeCropPos;
              if (relativeCropPos.HasValue)
              {
                WriteableBitmap bitmap2 = obj2.GetBitmap(new Size((double) imageMaxEdge, (double) imageMaxEdge), false, false);
                CropRectangle cropRectangle1 = new CropRectangle();
                ref CropRectangle local1 = ref cropRectangle1;
                double pixelHeight = (double) bitmap2.PixelHeight;
                Size? relativeCropSize = obj2.RelativeCropSize;
                double height = relativeCropSize.Value.Height;
                int num1 = (int) (pixelHeight * height);
                local1.Height = num1;
                ref CropRectangle local2 = ref cropRectangle1;
                double pixelWidth = (double) bitmap2.PixelWidth;
                relativeCropSize = obj2.RelativeCropSize;
                double width = relativeCropSize.Value.Width;
                int num2 = (int) (pixelWidth * width);
                local2.Width = num2;
                ref CropRectangle local3 = ref cropRectangle1;
                relativeCropPos = obj2.RelativeCropPos;
                System.Windows.Point point = relativeCropPos.Value;
                int num3 = (int) (point.X * (double) bitmap2.PixelWidth);
                local3.XOffset = num3;
                ref CropRectangle local4 = ref cropRectangle1;
                relativeCropPos = obj2.RelativeCropPos;
                point = relativeCropPos.Value;
                int num4 = (int) (point.Y * (double) bitmap2.PixelHeight);
                local4.YOffset = num4;
                CropRectangle cropRectangle2 = cropRectangle1;
                editedVideoArgs.CropRectangle = new CropRectangle?(cropRectangle2);
              }
              args.SharingState.SafeDispose();
              AppState.Worker.Enqueue((Action) (() =>
              {
                try
                {
                  MediaUpload.SendVideo(jids, editedVideoArgs);
                  tcs.SetResult(ExternalShare.ExternalShareResult.Shared);
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "Exception sending shared gif");
                  tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
                }
              }));
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "Exception sharing gif");
              tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
            }
          }
        }))));
        task = await tcs.Task;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "sendGifThroughPickerPage");
        throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
      }
      return task;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareVideoFileContentAsync(
      List<string> jids,
      IStorageFile file)
    {
      Log.l("external share", "share video async");
      ExternalShare.ExternalShareResult task;
      try
      {
        StorageFile cFile = file as StorageFile;
        Log.l("external share", "getting video properties");
        VideoProperties videoProperties = await cFile.Properties.GetVideoPropertiesAsync();
        WriteableBitmap thumbnail = (WriteableBitmap) null;
        Log.l("external share", "getting video thumbnail");
        try
        {
          using (StorageItemThumbnail thumbnailAsync = await cFile.GetThumbnailAsync((ThumbnailMode) 1, 100U))
          {
            if (thumbnailAsync != null)
            {
              if (thumbnailAsync.Type == null)
              {
                BitmapImage source = new BitmapImage();
                source.SetSource(((IRandomAccessStream) thumbnailAsync).AsStream());
                thumbnail = new WriteableBitmap((BitmapSource) source);
              }
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get thumbnail from share contract");
        }
        Log.d("external share", "Orientation: {0}", (object) videoProperties.Orientation);
        WaVideoArgs waVideoArgs1 = new WaVideoArgs();
        waVideoArgs1.FileExtension = file.FileType;
        waVideoArgs1.ContentType = file.ContentType;
        waVideoArgs1.Duration = (int) videoProperties.Duration.TotalSeconds;
        WaVideoArgs waVideoArgs2 = waVideoArgs1;
        Stream stream1 = waVideoArgs2.Stream;
        Stream stream2 = await file.OpenStreamForReadAsync();
        waVideoArgs2.Stream = stream2;
        waVideoArgs1.Thumbnail = thumbnail;
        waVideoArgs1.OrientationAngle = (360 - videoProperties.Orientation) % 360;
        WaVideoArgs baseVideoArgs = waVideoArgs1;
        waVideoArgs2 = (WaVideoArgs) null;
        waVideoArgs1 = (WaVideoArgs) null;
        ExternalSharingState es = new ExternalSharingState();
        es.RecipientJids = jids.ToArray();
        ExternalSharingState.Item obj = new ExternalSharingState.Item(await file.OpenStreamForReadAsync(), baseVideoArgs);
        es.AddItem((MediaSharingState.IItem) obj);
        obj.FetchLargeThumbAsync().Subscribe<WriteableBitmap>();
        TaskCompletionSource<ExternalShare.ExternalShareResult> tcs = new TaskCompletionSource<ExternalShare.ExternalShareResult>();
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() => PicturePreviewPage.Start((MediaSharingState) es, true).ObserveOnDispatcher<MediaSharingArgs>().Subscribe<MediaSharingArgs>((Action<MediaSharingArgs>) (args =>
        {
          if (args.Status == MediaSharingArgs.SharingStatus.Canceled)
          {
            tcs.SetResult(ExternalShare.ExternalShareResult.Discarded);
            args.SharingState.Dispose();
          }
          else
          {
            if (args.Status != MediaSharingArgs.SharingStatus.Submitted)
              return;
            try
            {
              Log.l("external share", "video send");
              MediaSharingState.IItem item = args.SharingState.SelectedItems.First<MediaSharingState.IItem>();
              AppState.Worker.Enqueue((Action) (() =>
              {
                try
                {
                  MediaUpload.SendVideo(jids, item.VideoInfo);
                  tcs.SetResult(ExternalShare.ExternalShareResult.Shared);
                }
                catch (Exception ex)
                {
                  Log.LogException(ex, "Exception sending shared video");
                  tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
                }
              }));
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "Exception sharing video");
              tcs.SetResult(ExternalShare.ExternalShareResult.Unknown);
            }
          }
        }))));
        task = await tcs.Task;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "sendVideoThroughPickerPage");
        throw new ExternalShareException(AppResources.CouldNotOpenPhoto);
      }
      return task;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareUriContent(
      List<string> jids,
      DataPackageView data,
      bool setDraftOnly)
    {
      Log.l("external share", "sharing uri");
      string uriString = await ExternalShare81.GetUriString(data);
      return uriString != null ? ExternalShare.ShareTextContent(jids, uriString, setDraftOnly) : ExternalShare.ExternalShareResult.MissingData;
    }

    private static async Task<string> GetUriString(DataPackageView data)
    {
      string uriStr = (string) null;
      if (data.AvailableFormats.Contains<string>(StandardDataFormats.WebLink))
      {
        Uri webLinkAsync = await data.GetWebLinkAsync();
        if (webLinkAsync != (Uri) null)
          uriStr = webLinkAsync.ToString();
      }
      if (uriStr == null && data.AvailableFormats.Contains<string>(StandardDataFormats.Uri))
      {
        Uri uriAsync = await data.GetUriAsync();
        if (uriAsync != (Uri) null)
          uriStr = uriAsync.ToString();
      }
      if (uriStr == null)
        return (string) null;
      if (data.Properties.Title != null)
        uriStr = data.Properties.Title + "\n\n" + uriStr;
      return uriStr;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareAudioContent(
      List<string> jids,
      IStorageFile file)
    {
      Log.l("external share", "share audio");
      ExternalShare.ExternalShareResult r = ExternalShare.ExternalShareResult.Unknown;
      try
      {
        MusicProperties musicPropertiesAsync = await (file as StorageFile).Properties.GetMusicPropertiesAsync();
        WaAudioArgs waAudioArgs1 = new WaAudioArgs();
        waAudioArgs1.FileExtension = file.FileType;
        waAudioArgs1.Duration = (int) musicPropertiesAsync.Duration.TotalSeconds;
        WaAudioArgs waAudioArgs2 = waAudioArgs1;
        Stream stream1 = waAudioArgs2.Stream;
        Stream stream2 = await file.OpenStreamForReadAsync();
        waAudioArgs2.Stream = stream2;
        WaAudioArgs args = waAudioArgs1;
        waAudioArgs2 = (WaAudioArgs) null;
        waAudioArgs1 = (WaAudioArgs) null;
        MediaUpload.SendAudio(jids, args, false);
        r = ExternalShare.ExternalShareResult.Shared;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "ShareContract - Audio");
        throw new ExternalShareException(AppResources.CouldNotOpenAudio);
      }
      return r;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareContactContent(
      List<string> jids,
      IStorageFile file)
    {
      Log.l("external share", "share contact");
      ExternalShare.ExternalShareResult r = ExternalShare.ExternalShareResult.Unknown;
      try
      {
        ContactVCard vCard = ContactVCard.Create(await FileIO.ReadTextAsync(file));
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          foreach (string jid in jids)
          {
            Message message = vCard.ToMessage(db);
            message.KeyRemoteJid = jid;
            db.InsertMessageOnSubmit(message);
          }
          db.SubmitChanges();
        }));
        r = ExternalShare.ExternalShareResult.Shared;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "ShareContract - Contact");
        throw new ExternalShareException(AppResources.CouldNotOpenContact);
      }
      return r;
    }

    private static async Task<ExternalShare.ExternalShareResult> ShareDocumentContent(
      List<string> jids,
      IStorageFile file,
      Message quotedMsg,
      string quotedChat,
      bool c2cStarted)
    {
      Log.l("external share", "sharing doc to {0}", (object) jids);
      ExternalShare.ExternalShareResult result = ExternalShare.ExternalShareResult.Unknown;
      try
      {
        if (!(file is StorageFile storageFile))
        {
          Log.l("external share", "null storage file");
        }
        else
        {
          BasicProperties basicPropertiesAsync = await storageFile.GetBasicPropertiesAsync();
          Log.l("external share", "file size:{0}", (object) Utils.FileSizeFormatter.Format((long) basicPropertiesAsync.Size));
          if (basicPropertiesAsync.Size > (ulong) Settings.MaxFileSize)
          {
            Log.l("external share", "file too big");
            throw new ExternalShareException(string.Format(AppResources.UploadDocumentTooBig, (object) Utils.FileSizeFormatter.Format((long) Settings.MaxFileSize)));
          }
          DocumentMessageUtils.DocumentData documentData1 = new DocumentMessageUtils.DocumentData();
          documentData1.MimeType = storageFile.ContentType;
          documentData1.Title = storageFile.DisplayName;
          documentData1.Filename = storageFile.DisplayName;
          documentData1.FileExtension = file.FileType;
          documentData1.Thumbnail = (WriteableBitmap) null;
          DocumentMessageUtils.DocumentData documentData2 = documentData1;
          Stream stream1 = await file.OpenStreamForReadAsync();
          documentData2.Stream = stream1;
          DocumentMessageUtils.DocumentData docData = documentData1;
          documentData2 = (DocumentMessageUtils.DocumentData) null;
          documentData1 = (DocumentMessageUtils.DocumentData) null;
          if (file.ContentType.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document") || file.ContentType.Contains("application/vnd.openxmlformats-officedocument.presentationml.presentation") || file.ContentType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
          {
            Log.l("external share", "parsing {0}", (object) file.ContentType);
            Stream zipFileStream = (Stream) null;
            try
            {
              zipFileStream = await file.OpenStreamForReadAsync();
              zipFileStream.Position = 0L;
              if (file.ContentType.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document") || file.ContentType.Contains("application/vnd.openxmlformats-officedocument.presentationml.presentation"))
              {
                Stream stream2 = (Stream) null;
                try
                {
                  stream2 = (Stream) Extensions.ExtractZipFile(zipFileStream, ExternalShare81.OPENXML_PROPERTIES_FILE);
                  if (stream2 != null)
                  {
                    string extractionRegex = file.ContentType.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document") ? ExternalShare81.OPENXML_DOCX_PAGES_REGEX : ExternalShare81.OPENXML_PPTX_SLIDES_REGEX;
                    string fromXmlPropsStream = Extensions.extractItemCountFromXmlPropsStream(stream2, extractionRegex, ExternalShare81.OPENXML_COUNT_EXTRACT_REGEX);
                    if (fromXmlPropsStream != null && fromXmlPropsStream.Length > 2)
                      docData.PageCount = int.Parse(fromXmlPropsStream.Substring(1, fromXmlPropsStream.Length - 2));
                  }
                  Log.l("external share", "got count for {0} | {1}", (object) file.ContentType, (object) docData.PageCount);
                }
                finally
                {
                  stream2.SafeDispose();
                }
              }
              else if (file.ContentType.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
              {
                int xlsxSheetCount = Extensions.getXlsxSheetCount(zipFileStream);
                if (xlsxSheetCount > 0)
                  docData.PageCount = xlsxSheetCount;
                Log.l("external share", "got count for {0} | {1}", (object) file.ContentType, (object) docData.PageCount);
              }
              zipFileStream.SafeDispose();
              zipFileStream = await file.OpenStreamForReadAsync();
              zipFileStream.Position = 0L;
              Stream bitmapStream = (Stream) null;
              try
              {
                bitmapStream = (Stream) Extensions.ExtractZipFile(zipFileStream, ExternalShare81.OPENXML_THUMBNAIL_FILE);
                if (bitmapStream != null)
                {
                  WriteableBitmap decodedImage = (WriteableBitmap) null;
                  Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() =>
                  {
                    decodedImage = JpegUtils.DecodeJpeg(bitmapStream);
                    Log.l("external share", "got thumbnail for {0}", (object) file.ContentType);
                  }));
                  docData.Thumbnail = decodedImage;
                }
              }
              finally
              {
                bitmapStream.SafeDispose();
                bitmapStream = (Stream) null;
              }
            }
            catch (Exception ex)
            {
              string context = "parse " + file.ContentType;
              Log.LogException(ex, context);
            }
            finally
            {
              zipFileStream.SafeDispose();
              zipFileStream = (Stream) null;
            }
            zipFileStream = (Stream) null;
          }
          if (file.ContentType.Contains("application/pdf"))
          {
            try
            {
              Log.l("external share", "parsing pdf");
              Stream pdfStream = await file.OpenStreamForReadAsync();
              docData.PageCount = PdfUtils.Parse(pdfStream).PageCount;
              Log.l("external share", "pdf parsed");
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "parse pdf");
            }
          }
          MediaUpload.SendDocument(jids, docData, quotedMsg, quotedChat, c2cStarted);
          result = ExternalShare.ExternalShareResult.Shared;
          docData = (DocumentMessageUtils.DocumentData) null;
        }
        storageFile = (StorageFile) null;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "share contract - document");
        if (!(ex is ExternalShareException))
          throw new ExternalShareException(AppResources.CouldNotOpenDocument);
        throw;
      }
      Log.l("external share", "doc {0}", (object) result);
      return result;
    }

    public static async Task<ExternalShare.ExternalShareResult> ShareStorageFileAsync(
      List<string> jids,
      IStorageFile file,
      Message quotedMsg = null,
      string quotedChat = null,
      bool c2cStarted = false)
    {
      ExternalShare.ExternalShareResult externalShareResult1 = ExternalShare.ExternalShareResult.Unknown;
      if (file == null)
        return externalShareResult1;
      string contentType = file.ContentType;
      Log.l("external share", "content type:{0}", (object) contentType);
      ExternalShare.ExternalShareResult externalShareResult2;
      if (contentType.Contains("image"))
        externalShareResult2 = await ExternalShare81.SharePhotoContent(jids, file);
      else if (contentType.Contains("video"))
        externalShareResult2 = await ExternalShare81.ShareVideoContent(jids, file);
      else if (contentType.Contains("audio"))
        externalShareResult2 = await ExternalShare81.ShareAudioContent(jids, file);
      else if (contentType.Contains("vcard"))
        externalShareResult2 = await ExternalShare81.ShareContactContent(jids, file);
      else
        externalShareResult2 = await ExternalShare81.ShareDocumentContent(jids, file, quotedMsg, quotedChat, c2cStarted);
      return externalShareResult2;
    }

    private enum SharingTypes
    {
      Bitmap,
      Rtf,
      Storage,
      Text,
      Unknown,
      Uri,
    }
  }
}
