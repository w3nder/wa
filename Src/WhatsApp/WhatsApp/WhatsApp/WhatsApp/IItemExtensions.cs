// Decompiled with JetBrains decompiler
// Type: WhatsApp.IItemExtensions
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public static class IItemExtensions
  {
    public static BitmapSource GetThumbnailForIItem(
      this MediaSharingState.IItem iItem,
      ref BitmapSource thumbRef)
    {
      if (thumbRef == null)
      {
        if (iItem.VideoInfo != null)
          return iItem.LargeThumb != null ? (BitmapSource) new WriteableBitmap(iItem.LargeThumb) : (BitmapSource) null;
        WriteableBitmap bitmap = iItem.GetBitmap(Constants.ItemThumbnailSize);
        thumbRef = (BitmapSource) ImageStore.CreateThumbnail((BitmapSource) bitmap, 98, new double?(1.0));
      }
      return thumbRef;
    }

    public static WriteableBitmap GetBitmapForIItem(
      this MediaSharingState.IItem iItem,
      bool isVideo,
      Size maxSize,
      bool withCropping = true,
      bool withRotation = true,
      bool withDrawing = false)
    {
      if (isVideo)
        return iItem.LargeThumb != null ? iItem.AlterLargeThumbForIItem(withCropping, withRotation) : (WriteableBitmap) null;
      if (iItem.DrawingBitmapCache != null & withCropping & withRotation & withDrawing)
        return iItem.DrawingBitmapCache;
      WriteableBitmap bitmap = iItem.ToPicInfo(maxSize).GetBitmap(withCropping, withRotation, withDrawing);
      if (withCropping & withRotation & withDrawing)
        iItem.DrawingBitmapCache = bitmap;
      return bitmap;
    }

    public static IObservable<WriteableBitmap> FetchLargeThumbForIItemAsync(
      this MediaSharingState.IItem iItem,
      string filePath,
      Stream clonedStream = null)
    {
      if (iItem.VideoInfo != null)
      {
        TimeCrop? timeCrop = iItem.VideoInfo.TimeCrop;
        long num;
        if (!timeCrop.HasValue)
        {
          num = 0L;
        }
        else
        {
          timeCrop = iItem.VideoInfo.TimeCrop;
          num = timeCrop.Value.StartTime.Ticks;
        }
        long frameLocation = num;
        return ImageStore.GetVideoFrameBitmap(iItem.VideoInfo.IsCameraVideo ? iItem.VideoInfo.OrientationAngle : -1, frameLocation, filePath, clonedStream).SubscribeOn<WriteableBitmap>((IScheduler) AppState.ImageWorker).Take<WriteableBitmap>(1).ObserveOnDispatcher<WriteableBitmap>().Select<WriteableBitmap, WriteableBitmap>((Func<WriteableBitmap, WriteableBitmap>) (bitmap =>
        {
          if (iItem.LargeThumb == null)
            iItem.LargeThumb = (BitmapSource) ImageStore.CreateThumbnail((BitmapSource) bitmap, MessageViewModel.LargeThumbPixelWidth);
          iItem.LargeThumb = (BitmapSource) bitmap;
          iItem.VideoInfo.LargeThumbnail = bitmap;
          iItem.VideoInfo.Thumbnail = bitmap;
          iItem.largeThumbSub.SafeDispose();
          iItem.largeThumbSub = (IDisposable) null;
          return bitmap;
        }));
      }
      Log.l("error fetch video bitmap", "video info is null");
      return (IObservable<WriteableBitmap>) null;
    }

    public static WriteableBitmap AlterLargeThumbForIItem(
      this MediaSharingState.IItem iItem,
      bool withCropping = true,
      bool withRotation = true)
    {
      WriteableBitmap writeableBitmap = new WriteableBitmap(iItem.LargeThumb);
      System.Windows.Point zoomScale;
      if (iItem.ZoomScale.X == 1.0)
      {
        zoomScale = iItem.ZoomScale;
        if (zoomScale.Y == 1.0)
          goto label_3;
      }
      double pixelWidth = (double) writeableBitmap.PixelWidth;
      zoomScale = iItem.ZoomScale;
      double x1 = zoomScale.X;
      double width1 = Math.Abs(pixelWidth / x1);
      double pixelHeight = (double) writeableBitmap.PixelHeight;
      zoomScale = iItem.ZoomScale;
      double y1 = zoomScale.Y;
      double height1 = Math.Abs(pixelHeight / y1);
      writeableBitmap = writeableBitmap.Crop(new System.Windows.Point(((double) writeableBitmap.PixelWidth - width1) / 2.0, ((double) writeableBitmap.PixelHeight - height1) / 2.0), new Size(width1, height1));
label_3:
      if (withCropping)
      {
        System.Windows.Point? relativeCropPos = iItem.RelativeCropPos;
        if (relativeCropPos.HasValue)
        {
          Size? relativeCropSize = iItem.RelativeCropSize;
          if (relativeCropSize.HasValue)
          {
            System.Windows.Point location;
            ref System.Windows.Point local1 = ref location;
            relativeCropPos = iItem.RelativeCropPos;
            zoomScale = relativeCropPos.Value;
            double x2 = zoomScale.X * (double) writeableBitmap.PixelWidth;
            relativeCropPos = iItem.RelativeCropPos;
            zoomScale = relativeCropPos.Value;
            double y2 = zoomScale.Y * (double) writeableBitmap.PixelHeight;
            local1 = new System.Windows.Point(x2, y2);
            Size size;
            ref Size local2 = ref size;
            relativeCropSize = iItem.RelativeCropSize;
            double width2 = relativeCropSize.Value.Width * (double) writeableBitmap.PixelWidth;
            relativeCropSize = iItem.RelativeCropSize;
            double height2 = relativeCropSize.Value.Height * (double) writeableBitmap.PixelHeight;
            local2 = new Size(width2, height2);
            writeableBitmap = writeableBitmap.Crop(new Rect(location, size));
          }
        }
      }
      int num = iItem.RotatedTimes % 4;
      if (withRotation && num > 0)
        writeableBitmap = writeableBitmap.Rotate(num * 90);
      return writeableBitmap;
    }

    public static IObservable<WriteableBitmap> GetVideoTimelineThumbsForIItemAsync(
      this MediaSharingState.IItem iItem,
      int orientationAngle,
      string filePath,
      Stream stream = null)
    {
      return ImageStore.GetVideoFrameBitmapList(orientationAngle, filePath, stream).SimpleSubscribeOn<WriteableBitmap>((IScheduler) AppState.ImageWorker).Select<WriteableBitmap, WriteableBitmap>((Func<WriteableBitmap, WriteableBitmap>) (bitmap =>
      {
        if (iItem.MediaTimelineThumbnails == null)
          iItem.MediaTimelineThumbnails = new List<WriteableBitmap>();
        iItem.MediaTimelineThumbnails.Add(bitmap);
        return bitmap;
      }));
    }
  }
}
