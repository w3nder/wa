// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class ImageStore
  {
    private static bool? isDarkTheme_;
    private static BitmapImage micIconGrey;
    private static BitmapImage micIconGreen;
    private static BitmapImage micIconBlue;
    private static BitmapImage playIcon;
    private static BitmapImage playIconActive;
    private static BitmapImage pauseIcon;
    private static BitmapImage pauseIconActive;
    private static BitmapImage downloadIcon;
    private static BitmapImage downloadIconActive;
    private static BitmapImage uploadIcon;
    private static BitmapImage uploadIconActive;
    private static BitmapImage cancelIcon;
    private static BitmapImage cancelIconActive;
    private static BitmapImage handsetIcon;
    private static BitmapImage sendEmailIcon;
    public static BitmapImage selectedCheckMark_;
    public static BitmapImage playButton_;
    public static BitmapImage whitePlayButton_;
    private static BitmapImage gifIcon_;
    private static BitmapImage sdCardIcon;
    private static BitmapImage oneDriveIcon;
    private static BitmapImage phoneIcon;
    private static BitmapImage smilingPhoneIcon;
    private static BitmapImage sadPhoneIcon;

    public static bool IsDarkTheme()
    {
      if (!ImageStore.isDarkTheme_.HasValue)
        Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => ImageStore.isDarkTheme_ = new bool?((Visibility) Application.Current.Resources[(object) "PhoneDarkThemeVisibility"] == Visibility.Visible)));
      return ImageStore.isDarkTheme_ ?? true;
    }

    public static WriteableBitmap CreateMessageLargeThumbnail(
      Stream picSrcStream,
      int picWidth,
      int picHeight,
      int targetThumbWidth,
      double maxWHRatio = 2.4)
    {
      if (picSrcStream == null || picWidth == 0 || picHeight == 0)
        return (WriteableBitmap) null;
      double num1 = (double) picWidth / (double) picHeight;
      WriteableBitmap messageLargeThumbnail;
      if (num1 < 1.0)
      {
        WriteableBitmap bitmap = BitmapUtils.CreateBitmap(picSrcStream, targetThumbWidth, (int) ((double) targetThumbWidth / num1));
        int y = (bitmap.PixelHeight - bitmap.PixelWidth) / 3;
        messageLargeThumbnail = bitmap.Crop(new System.Windows.Point(0.0, (double) y), new Size((double) bitmap.PixelWidth, (double) bitmap.PixelWidth));
      }
      else if (num1 > maxWHRatio)
      {
        double num2 = (double) targetThumbWidth / ((double) picHeight * maxWHRatio);
        int num3 = (int) ((double) picWidth * num2);
        WriteableBitmap bitmap = BitmapUtils.CreateBitmap(picSrcStream, num3, num3);
        int x = (bitmap.PixelWidth - targetThumbWidth) / 2;
        messageLargeThumbnail = bitmap.Crop(new System.Windows.Point((double) x, 0.0), new Size((double) targetThumbWidth, (double) bitmap.PixelHeight));
      }
      else
        messageLargeThumbnail = BitmapUtils.CreateBitmap(picSrcStream, targetThumbWidth, targetThumbWidth);
      return messageLargeThumbnail;
    }

    public static WriteableBitmap CreateThumbnail(
      BitmapSource bitmapSrc,
      int targetFitWidth,
      double? maxWidthHeightRatio = 2.4)
    {
      if (bitmapSrc == null)
        return (WriteableBitmap) null;
      bool flag = bitmapSrc.PixelHeight > bitmapSrc.PixelWidth;
      double num1 = (double) bitmapSrc.PixelWidth / (double) bitmapSrc.PixelHeight;
      if (!(bitmapSrc is WriteableBitmap writeableBitmap))
        writeableBitmap = new WriteableBitmap(bitmapSrc);
      WriteableBitmap bitmap1 = writeableBitmap;
      WriteableBitmap thumbnail = (WriteableBitmap) null;
      if (maxWidthHeightRatio.HasValue)
      {
        if (flag)
        {
          double scale = (double) targetFitWidth / (double) bitmap1.PixelWidth;
          WriteableBitmap bitmap2 = scale < 1.0 ? bitmap1.Scale(scale) : bitmap1;
          int y = (bitmap2.PixelHeight - bitmap2.PixelWidth) / 3;
          thumbnail = bitmap2.Crop(new System.Windows.Point(0.0, (double) y), new Size((double) bitmap2.PixelWidth, (double) bitmap2.PixelWidth));
        }
        else
        {
          double num2 = num1;
          double? nullable = maxWidthHeightRatio;
          double valueOrDefault = nullable.GetValueOrDefault();
          if ((num2 > valueOrDefault ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          {
            double num3 = (double) bitmap1.PixelHeight * maxWidthHeightRatio.Value;
            double scale = (double) targetFitWidth / num3;
            WriteableBitmap bitmap3 = scale < 1.0 ? bitmap1.Scale(scale) : bitmap1;
            double width = (double) bitmap3.PixelHeight * maxWidthHeightRatio.Value;
            double x = ((double) bitmap3.PixelWidth - width) / 2.0;
            thumbnail = bitmap3.Crop(new System.Windows.Point(x, 0.0), new Size(width, (double) bitmap3.PixelHeight));
          }
        }
      }
      if (thumbnail == null)
      {
        double scale = (double) targetFitWidth / (double) bitmap1.PixelWidth;
        thumbnail = scale < 1.0 ? bitmap1.Scale(scale) : bitmap1;
      }
      return thumbnail;
    }

    public static IObservable<WriteableBitmap> GetVideoFrameBitmap(
      int orientationAngle,
      long frameLocation,
      string videoFilepath,
      Stream clonedStream = null)
    {
      return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        VideoFrame videoFrame = (VideoFrame) null;
        bool cancelled = false;
        Assert.IsTrue(videoFilepath != null || clonedStream != null, "Must have a way to open the video");
        try
        {
          if (clonedStream != null)
          {
            using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(clonedStream, orientationAngle))
            {
              videoFrameGrabber.Seek(frameLocation);
              videoFrame = videoFrameGrabber.ReadFrame();
            }
          }
          else
          {
            using (VideoFrameGrabber videoFrameGrabber = new VideoFrameGrabber(videoFilepath, orientationAngle))
            {
              videoFrameGrabber.Seek(frameLocation);
              videoFrame = videoFrameGrabber.ReadFrame();
            }
          }
        }
        catch (Exception ex)
        {
          Log.l(nameof (GetVideoFrameBitmap), "Exception loading frame location {0} from {1}", (object) frameLocation, (object) (videoFilepath ?? "stream"));
          Log.LogException(ex, "get video frame from location");
          observer.OnError(ex);
          videoFrame = (VideoFrame) null;
        }
        if (videoFrame == null)
        {
          Log.d("VideoFrameGrabber", "No frame found for ", (object) frameLocation);
          observer.OnCompleted();
        }
        else
          Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
          {
            if (!cancelled)
            {
              try
              {
                using (videoFrame)
                {
                  Log.d("VideoFrameGrabber", "Frame @ {0} for {1}", (object) videoFrame.Timestamp, (object) frameLocation);
                  WriteableBitmap bitmap = videoFrame.Bitmap;
                  if (!ImageStore.IsBitmapAlmostAllZeroValuePixels(bitmap))
                    observer.OnNext(bitmap);
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "Get video frame bitmap");
                observer.OnError(ex);
              }
            }
            observer.OnCompleted();
          }));
        return (Action) (() => cancelled = true);
      })).Catch<WriteableBitmap>(Observable.Empty<WriteableBitmap>());
    }

    public static IObservable<WriteableBitmap> GetVideoFrameBitmapList(
      int orientationAngle,
      string videoFilepath,
      Stream stream = null)
    {
      return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        List<WriteableBitmap> writeableBitmapList = new List<WriteableBitmap>();
        VideoFrame videoFrame = (VideoFrame) null;
        bool cancel = false;
        AppState.ImageWorker.Enqueue((Action) (() =>
        {
          VideoFrameGrabber videoFrameGrabber = stream == null ? new VideoFrameGrabber(videoFilepath, orientationAngle, new int?(8)) : (!(stream is MemoryStream memoryStream2) ? new VideoFrameGrabber(stream) : new VideoFrameGrabber((Stream) new MemoryStream(memoryStream2.GetBuffer(), 0, (int) memoryStream2.Length, false, true)));
          using (videoFrameGrabber)
          {
            long durationTicks = videoFrameGrabber.DurationTicks;
            long num3 = durationTicks / 8L;
            int num4 = 0;
            for (long ticks = 0; ticks <= durationTicks; ticks += num3)
            {
              if (!cancel)
              {
                try
                {
                  videoFrameGrabber.Seek(ticks);
                  videoFrame = videoFrameGrabber.ReadFrame();
                }
                catch (Exception ex)
                {
                  string context = string.Format("get video frame {0}", (object) ticks);
                  Log.LogException(ex, context);
                  videoFrame = (VideoFrame) null;
                }
                if (videoFrame != null)
                {
                  try
                  {
                    using (videoFrame)
                    {
                      Log.d(nameof (GetVideoFrameBitmapList), "running frame {0} {1}", (object) ticks, (object) cancel);
                      WriteableBitmap bitmap = videoFrame.GetScaledBitmap();
                      if (!ImageStore.IsBitmapAlmostAllZeroValuePixels(bitmap))
                      {
                        ++num4;
                        if (!cancel)
                          Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => observer.OnNext(bitmap)));
                      }
                    }
                  }
                  catch (Exception ex)
                  {
                    string context = string.Format("Get video frame bitmap {0}", (object) ticks);
                    Log.LogException(ex, context);
                  }
                }
              }
              else
                break;
            }
            observer.OnCompleted();
          }
        }));
        return (Action) (() => cancel = true);
      }));
    }

    private static bool IsBitmapAlmostAllZeroValuePixels(WriteableBitmap bitmap)
    {
      int[] pixels = bitmap.Pixels;
      int length = pixels.Length;
      int[] numArray = new int[4]{ 53, 23, 13, 3 };
      foreach (int num in numArray)
      {
        for (int index = 0; index < length; index += num)
        {
          if (pixels[index] != 0)
            return false;
        }
      }
      return true;
    }

    public static IObservable<WriteableBitmap> GetVideoFrameBitmap(
      string videoFilepath,
      int numFrames)
    {
      return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
      {
        VideoFrame videoFrame = (VideoFrame) null;
        bool cancelled = false;
        VideoFrameGrabber videoFrameGrabber;
        try
        {
          videoFrameGrabber = new VideoFrameGrabber(videoFilepath);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get video frames from path");
          observer.OnCompleted();
          return (Action) (() => { });
        }
        using (videoFrameGrabber)
        {
          long durationTicks = videoFrameGrabber.DurationTicks;
          long num = durationTicks / (long) numFrames;
          long ticks = 0;
          for (int index = numFrames; index > 0; --index)
          {
            if (!cancelled)
            {
              videoFrameGrabber.Seek(ticks);
              try
              {
                videoFrame = videoFrameGrabber.ReadFrame();
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "get video frame after seek");
              }
              if (videoFrame == null)
                observer.OnCompleted();
              else
                Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
                {
                  if (cancelled)
                    return;
                  try
                  {
                    using (videoFrame)
                      observer.OnNext(videoFrame.Bitmap);
                  }
                  catch (Exception ex)
                  {
                    Log.LogException(ex, "Get video frame bitmap");
                  }
                }));
              ticks += num;
              if (ticks > durationTicks || ticks + num / 2L > durationTicks)
                ticks = durationTicks;
            }
            else
              break;
          }
        }
        observer.OnCompleted();
        return (Action) (() => cancelled = true);
      }));
    }

    public static BitmapImage GetStockIcon(string path)
    {
      DateTime? start = PerformanceTimer.Start();
      BitmapImage r = (BitmapImage) null;
      Deployment.Current.Dispatcher.InvokeSynchronous((Action) (() => r = new BitmapImage()
      {
        CreateOptions = BitmapCreateOptions.None,
        UriSource = new Uri(path, UriKind.Relative)
      }));
      PerformanceTimer.End("GetStockIcon: " + path + " Found " + (r != null).ToString(), start);
      return r;
    }

    public static BitmapImage GetStockIcon(string light, string dark)
    {
      return ImageStore.GetStockIcon(ImageStore.IsDarkTheme() ? dark : light);
    }

    public static BitmapImage GetCachedBi(
      ref BitmapImage cached,
      Func<BitmapImage> create,
      bool forceCache = false)
    {
      return !forceCache ? create() : cached ?? (cached = create());
    }

    public static BitmapImage MicIconLocalNew
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.micIconGreen, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/mic-green-hd.png")));
      }
    }

    public static BitmapImage MicIconLocalPlayed
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.micIconBlue, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/mic-blue-hd.png")));
      }
    }

    public static BitmapImage MicIconRemotePlayed
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.micIconBlue, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/mic-blue-hd.png")));
      }
    }

    public static BitmapImage MicIconRemoteUnplayed
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.micIconGrey, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/mic-grey-hd.png")));
      }
    }

    public static BitmapImage PlayIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.playIcon, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/play.png")));
      }
    }

    public static BitmapImage PlayIconActive
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.playIconActive, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/play-active.png")));
      }
    }

    public static BitmapImage PauseIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.pauseIcon, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/pause.png")));
      }
    }

    public static BitmapImage PauseIconActive
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.pauseIconActive, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/pause-active.png")));
      }
    }

    public static BitmapImage DownloadIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.downloadIcon, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/download.png")));
      }
    }

    public static BitmapImage DownloadIconActive
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.downloadIconActive, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/download-active.png")));
      }
    }

    public static BitmapImage UploadIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.uploadIcon, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/upload.png")));
      }
    }

    public static BitmapImage UploadIconActive
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.uploadIconActive, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/upload-active.png")));
      }
    }

    public static BitmapImage CancelIconActive
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.cancelIconActive, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/x-active.png")));
      }
    }

    public static BitmapImage SendEmailIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.sendEmailIcon, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/contact-support.png")));
      }
    }

    public static BitmapImage SelectedCheckMark
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.selectedCheckMark_, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/multisend-selected-mark.png")), true);
      }
    }

    public static BitmapImage PlayButton
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.playButton_, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/play-button-black.png", "/Images/play-button.png")), true);
      }
    }

    public static BitmapImage WhitePlayButton
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.whitePlayButton_, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/play-button.png")), true);
      }
    }

    public static BitmapImage GifIcon
    {
      get
      {
        return ImageStore.GetCachedBi(ref ImageStore.gifIcon_, (Func<BitmapImage>) (() => ImageStore.GetStockIcon("/Images/gif-icon.png")), true);
      }
    }

    public static BitmapImage SDCardIcon
    {
      get
      {
        return ImageStore.sdCardIcon ?? (ImageStore.sdCardIcon = ImageStore.GetStockIcon("/Images/backup-card.png"));
      }
    }

    public static BitmapImage OneDriveIcon
    {
      get
      {
        return ImageStore.oneDriveIcon ?? (ImageStore.oneDriveIcon = ImageStore.GetStockIcon("/Images/backup-onedrive.png"));
      }
    }

    public static BitmapImage PhoneIcon
    {
      get
      {
        return ImageStore.phoneIcon ?? (ImageStore.phoneIcon = ImageStore.GetStockIcon("/Images/backup-phone.png"));
      }
    }

    public static BitmapImage SmilingPhoneIcon
    {
      get
      {
        return ImageStore.smilingPhoneIcon ?? (ImageStore.smilingPhoneIcon = ImageStore.GetStockIcon("/Images/backup-smile.png"));
      }
    }

    public static BitmapImage SadPhoneIcon
    {
      get
      {
        return ImageStore.sadPhoneIcon ?? (ImageStore.sadPhoneIcon = ImageStore.GetStockIcon("/Images/sad-phone.png"));
      }
    }
  }
}
