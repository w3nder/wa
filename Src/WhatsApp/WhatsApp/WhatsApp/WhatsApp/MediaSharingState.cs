// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaSharingState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public abstract class MediaSharingState : WaDisposable
  {
    public static int MaxSelectedItem = AppState.IsLowMemoryDevice ? 10 : 30;
    private Subject<MediaSharingState.SelectedItemsChangeCause> selectedItemsChangedSubject = new Subject<MediaSharingState.SelectedItemsChangeCause>();

    public MediaSharingState.SharingMode Mode { get; set; }

    public abstract IEnumerable<MediaSharingState.IItem> SelectedItems { get; }

    public abstract int SelectedCount { get; }

    public string[] RecipientJids { get; set; }

    public MediaSharingState(MediaSharingState.SharingMode mode) => this.Mode = mode;

    public IDisposable SubscribeToSelectedItemsChange(
      Action<MediaSharingState.SelectedItemsChangeCause> a)
    {
      return this.selectedItemsChangedSubject.Subscribe<MediaSharingState.SelectedItemsChangeCause>(a);
    }

    protected void NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause cause)
    {
      this.selectedItemsChangedSubject.OnNext(cause);
    }

    protected override void DisposeManagedResources() => base.DisposeManagedResources();

    public abstract void AddItem(MediaSharingState.IItem item);

    public abstract void DeleteItem(MediaSharingState.IItem item);

    public abstract void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null);

    public virtual MediaSharingState recreateForRestart()
    {
      Log.l(nameof (MediaSharingState), "recreateForRestart not supported");
      return (MediaSharingState) null;
    }

    public interface IItem
    {
      WaVideoArgs VideoInfo { get; set; }

      GifArgs GifInfo { get; set; }

      DrawingArgs DrawArgs { get; set; }

      bool IsSent { get; set; }

      int RotatedTimes { get; set; }

      bool IsModified { get; }

      string Caption { get; set; }

      Size? RelativeCropSize { get; set; }

      System.Windows.Point? RelativeCropPos { get; set; }

      double? CropRatio { get; }

      System.Windows.Point ZoomScale { get; }

      BitmapSource ThumbnailOverride { get; set; }

      BitmapSource LargeThumb { get; set; }

      IDisposable largeThumbSub { get; set; }

      WriteableBitmap DrawingBitmapCache { get; set; }

      List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      BitmapSource GetThumbnail();

      IObservable<WriteableBitmap> GetBitmapAsync(
        Size maxSize,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false);

      WriteableBitmap GetBitmap(
        Size maxSize,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false);

      void WriteToDrawingBitmapCache(Size maxSize);

      FunXMPP.FMessage.Type GetMediaType();

      void Cleanup();

      IObservable<WriteableBitmap> FetchLargeThumbAsync();

      IObservable<WriteableBitmap> VideoTimelineThumbsAsync();

      MediaSharingState.PicInfo ToPicInfo(Size maxSize);

      string GetFullPath();

      int GetDuration();
    }

    public interface IItemFieldStats
    {
      wam_enum_media_picker_origin_type FsItemOrigin { get; }

      long FsItemId();
    }

    public abstract class PicInfo
    {
      public Size? MaxSize { get; set; }

      public string PathForDb { get; set; }

      public int RotatedTimes { get; set; }

      public int? Orientation { get; protected set; }

      public string Caption { get; set; }

      public Size? RelativeCropSize { get; set; }

      public System.Windows.Point? RelativeCropPos { get; set; }

      public System.Windows.Point ZoomScale { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public abstract IObservable<WriteableBitmap> GetBitmapAsync(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false);

      public abstract WriteableBitmap GetBitmap(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false);

      public WriteableBitmap GetBitmapFromStream(
        Stream imgStream,
        int? orientationToApply,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        WriteableBitmap bitmapFromStream;
        try
        {
          bitmapFromStream = JpegUtils.DecodeJpeg(imgStream, this.MaxSize);
          if (orientationToApply.HasValue)
            bitmapFromStream = JpegUtils.ApplyJpegOrientation((BitmapSource) bitmapFromStream, new int?(orientationToApply.Value));
          if (this.DrawArgs != null & withDrawing)
            bitmapFromStream = this.applyDrawing(this.DrawArgs, (double) bitmapFromStream.PixelWidth, (double) bitmapFromStream.PixelHeight, bitmapFromStream);
          if (withCropping && this.RelativeCropPos.HasValue && this.RelativeCropSize.HasValue)
          {
            System.Windows.Point location = new System.Windows.Point(this.RelativeCropPos.Value.X * (double) bitmapFromStream.PixelWidth, this.RelativeCropPos.Value.Y * (double) bitmapFromStream.PixelHeight);
            Size size = new Size(this.RelativeCropSize.Value.Width * (double) bitmapFromStream.PixelWidth, this.RelativeCropSize.Value.Height * (double) bitmapFromStream.PixelHeight);
            bitmapFromStream = bitmapFromStream.Crop(new Rect(location, size));
          }
          int num = this.RotatedTimes % 4;
          if (withRotation)
          {
            if (num > 0)
              bitmapFromStream = bitmapFromStream.Rotate(num * 90);
          }
        }
        catch (Exception ex)
        {
          bitmapFromStream = (WriteableBitmap) null;
          Log.LogException(ex, "get bitmap from pic info | picker");
        }
        return bitmapFromStream;
      }

      public WriteableBitmap applyDrawing(
        DrawingArgs DrawArgs,
        double oldWidth,
        double oldHeight,
        WriteableBitmap bitmap)
      {
        if (!DrawArgs.HasDrawing)
          return bitmap;
        if (this.DrawArgs.OriginalWidth > this.DrawArgs.OriginalHeight)
        {
          CompositeTransform compositeTransform = new CompositeTransform();
          double num1 = Math.Min(oldHeight / DrawArgs.Canvas.Clip.Bounds.Width, oldWidth / DrawArgs.Canvas.Clip.Bounds.Height);
          int num2 = (DrawArgs.OriginalHeight - DrawArgs.OriginalWidth) / 2;
          int num3 = (DrawArgs.OriginalWidth - DrawArgs.OriginalHeight) / 2;
          try
          {
            Log.l("ScaleX: {0} ScalyY: {1}", (object) num1, (object) num1);
            compositeTransform.ScaleX = num1;
            compositeTransform.ScaleY = num1;
            compositeTransform.TranslateX = -1.0 * (DrawArgs.Canvas.Clip.Bounds.X + (double) num2) * num1;
            compositeTransform.TranslateY = -1.0 * (DrawArgs.Canvas.Clip.Bounds.Y + (double) num3) * num1;
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "get bitmap transform failed");
          }
          DrawArgs.DrawGrid.Clip = (Geometry) new RectangleGeometry()
          {
            Rect = new Rect(DrawArgs.Canvas.Clip.Bounds.X + (double) num2, DrawArgs.Canvas.Clip.Bounds.Y + (double) num3, (double) DrawArgs.OriginalWidth, (double) DrawArgs.OriginalHeight)
          };
          DrawArgs.DrawGrid.Measure(new Size(DrawArgs.Canvas.Width, DrawArgs.Canvas.Height));
          DrawArgs.DrawGrid.Arrange(new Rect(0.0, 0.0, DrawArgs.Canvas.Width, DrawArgs.Canvas.Height));
          WriteableBitmap writeableBitmap = new WriteableBitmap((BitmapSource) bitmap);
          writeableBitmap.Render((UIElement) DrawArgs.DrawGrid, (Transform) compositeTransform);
          writeableBitmap.Invalidate();
          return writeableBitmap;
        }
        CompositeTransform compositeTransform1 = new CompositeTransform();
        double num = Math.Min(oldHeight / DrawArgs.Canvas.Clip.Bounds.Height, oldWidth / DrawArgs.Canvas.Clip.Bounds.Width);
        try
        {
          Log.l("ScaleX: {0} ScalyY: {1}", (object) num, (object) num);
          compositeTransform1.ScaleX = num;
          compositeTransform1.ScaleY = num;
          compositeTransform1.TranslateX = -1.0 * DrawArgs.Canvas.Clip.Bounds.X * num;
          compositeTransform1.TranslateY = -1.0 * DrawArgs.Canvas.Clip.Bounds.Y * num;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get bitmap transform failed");
        }
        DrawArgs.DrawGrid.Clip = (Geometry) new RectangleGeometry()
        {
          Rect = DrawArgs.Canvas.Clip.Bounds
        };
        DrawArgs.DrawGrid.Measure(new Size(DrawArgs.Canvas.Width, DrawArgs.Canvas.Height));
        DrawArgs.DrawGrid.Arrange(new Rect(0.0, 0.0, DrawArgs.Canvas.Width, DrawArgs.Canvas.Height));
        WriteableBitmap writeableBitmap1 = new WriteableBitmap((BitmapSource) bitmap);
        writeableBitmap1.Render((UIElement) DrawArgs.DrawGrid, (Transform) compositeTransform1);
        writeableBitmap1.Invalidate();
        return writeableBitmap1;
      }

      protected WriteableBitmap AdjustBitmap(
        WriteableBitmap bitmap,
        bool withCropping,
        bool withRotation,
        bool withDrawing)
      {
        if (bitmap == null)
        {
          Log.l("ManipulateImageForPicInfo", "supplied with null bitmap");
          return (WriteableBitmap) null;
        }
        System.Windows.Point zoomScale;
        if (this.ZoomScale.X != 1.0 || this.ZoomScale.Y != 1.0)
        {
          double pixelWidth = (double) bitmap.PixelWidth;
          zoomScale = this.ZoomScale;
          double x = zoomScale.X;
          double width = Math.Abs(pixelWidth / x);
          double pixelHeight = (double) bitmap.PixelHeight;
          zoomScale = this.ZoomScale;
          double y = zoomScale.Y;
          double height = Math.Abs(pixelHeight / y);
          bitmap = bitmap.Crop(new System.Windows.Point(((double) bitmap.PixelWidth - width) / 2.0, ((double) bitmap.PixelHeight - height) / 2.0), new Size(width, height));
        }
        if (this.DrawArgs != null & withDrawing)
          bitmap = this.applyDrawing(this.DrawArgs, (double) bitmap.PixelWidth, (double) bitmap.PixelHeight, bitmap);
        if (withCropping && this.RelativeCropPos.HasValue && this.RelativeCropSize.HasValue)
        {
          System.Windows.Point location;
          ref System.Windows.Point local = ref location;
          System.Windows.Point? relativeCropPos = this.RelativeCropPos;
          zoomScale = relativeCropPos.Value;
          double x = zoomScale.X * (double) bitmap.PixelWidth;
          relativeCropPos = this.RelativeCropPos;
          zoomScale = relativeCropPos.Value;
          double y = zoomScale.Y * (double) bitmap.PixelHeight;
          local = new System.Windows.Point(x, y);
          Size size = new Size(this.RelativeCropSize.Value.Width * (double) bitmap.PixelWidth, this.RelativeCropSize.Value.Height * (double) bitmap.PixelHeight);
          bitmap = bitmap.Crop(new Rect(location, size));
        }
        int num = this.RotatedTimes % 4;
        if (withRotation && num > 0)
          bitmap = bitmap.Rotate(num * 90);
        return bitmap;
      }

      public abstract Stream GetImageStream();

      public bool isChangedByUser()
      {
        if (this.RotatedTimes % 4 != 0 || this.RelativeCropPos.HasValue || this.RelativeCropSize.HasValue || this.ZoomScale.X > 1.0 || this.ZoomScale.Y > 1.0)
          return true;
        return this.DrawArgs != null && this.DrawArgs.HasDrawing;
      }
    }

    public enum SharingMode
    {
      ChooseMedia,
      ChoosePicture,
      ChooseVideo,
      TakePicture,
      TakeVideo,
      ExternalShare,
    }

    public enum SelectedItemsChangeCause
    {
      Undefined,
      Delete,
      Add,
    }
  }
}
