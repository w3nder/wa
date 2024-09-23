// Decompiled with JetBrains decompiler
// Type: WhatsApp.CameraSharingState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class CameraSharingState : MediaSharingState
  {
    private LinkedList<CameraSharingState.Item> items_ = new LinkedList<CameraSharingState.Item>();

    public override IEnumerable<MediaSharingState.IItem> SelectedItems
    {
      get => (IEnumerable<MediaSharingState.IItem>) this.items_.ToArray<CameraSharingState.Item>();
    }

    public override int SelectedCount => this.items_.Count;

    public CameraSharingState()
      : base(MediaSharingState.SharingMode.TakePicture)
    {
    }

    public override void AddItem(MediaSharingState.IItem itemToAdd)
    {
      if (!(itemToAdd is CameraSharingState.Item obj))
        return;
      this.items_.AddLast(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Add);
    }

    public override void DeleteItem(MediaSharingState.IItem itemToDel)
    {
      if (!(itemToDel is CameraSharingState.Item obj))
        return;
      this.items_.Remove(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      obj.Cleanup();
    }

    public override void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null)
    {
      LinkedList<CameraSharingState.Item> linkedList1 = new LinkedList<CameraSharingState.Item>();
      LinkedList<CameraSharingState.Item> linkedList2 = new LinkedList<CameraSharingState.Item>();
      foreach (CameraSharingState.Item obj in this.items_)
      {
        if (deletingCriteria == null || deletingCriteria((MediaSharingState.IItem) obj))
          linkedList2.AddLast(obj);
        else
          linkedList1.AddLast(obj);
      }
      this.items_ = linkedList1;
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      foreach (CameraSharingState.Item obj in linkedList2)
        obj.Cleanup();
    }

    protected override void DisposeManagedResources()
    {
      foreach (CameraSharingState.Item obj in this.items_)
        obj.Cleanup();
      base.DisposeManagedResources();
    }

    public override MediaSharingState recreateForRestart()
    {
      CameraSharingState cameraSharingState = new CameraSharingState();
      cameraSharingState.items_ = this.items_;
      this.items_ = (LinkedList<CameraSharingState.Item>) null;
      foreach (CameraSharingState.Item obj in cameraSharingState.items_)
        obj.IsSent = false;
      cameraSharingState.Mode = this.Mode;
      return (MediaSharingState) cameraSharingState;
    }

    public class Item : MediaSharingState.IItem, MediaSharingState.IItemFieldStats
    {
      private NativeStream tempFilestream_;
      private string LocalFilePath = "";
      private string _caption;
      private BitmapSource thumb_;
      private long itemId = DateTime.Now.Ticks;

      public BitmapSource LargeThumb { get; set; }

      public IDisposable largeThumbSub { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      public Item(
        NativeStream tempFilestream,
        WriteableBitmap bitmap,
        System.Windows.Point zoomScale,
        wam_enum_media_picker_origin_type origin)
      {
        this.tempFilestream_ = tempFilestream;
        this.ZoomScale = zoomScale;
        this.VideoInfo = (WaVideoArgs) null;
        this.GifInfo = (GifArgs) null;
        this.FsItemOrigin = origin;
      }

      public Item(WaVideoArgs videoArgs, System.Windows.Point zoomScale, wam_enum_media_picker_origin_type origin)
      {
        this.tempFilestream_ = (NativeStream) null;
        this.VideoInfo = videoArgs;
        this.GifInfo = (GifArgs) null;
        this.ZoomScale = zoomScale;
        this.LargeThumb = (BitmapSource) this.VideoInfo.LargeThumbnail;
        this.FsItemOrigin = origin;
      }

      public IObservable<WriteableBitmap> FetchLargeThumbAsync()
      {
        return this.FetchLargeThumbForIItemAsync(NativeMediaStorage.MakeUri(this.VideoInfo.FullPath));
      }

      public IObservable<WriteableBitmap> VideoTimelineThumbsAsync()
      {
        if (this.VideoInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(this.VideoInfo.OrientationAngle, NativeMediaStorage.MakeUri(this.VideoInfo.FullPath));
        Log.l("CameraSharingState.Item", "error fetch video bitmap - video info is null");
        return (IObservable<WriteableBitmap>) null;
      }

      public string GetFullPath() => "";

      public int GetDuration()
      {
        WaVideoArgs videoInfo = this.VideoInfo;
        return videoInfo == null ? 0 : videoInfo.Duration;
      }

      public void Cleanup()
      {
        this.tempFilestream_.SafeDispose();
        this.tempFilestream_ = (NativeStream) null;
        this.largeThumbSub.SafeDispose();
        this.largeThumbSub = (IDisposable) null;
        if (this.VideoInfo == null || this.IsSent)
          return;
        this.VideoInfo.CleanUp();
        this.VideoInfo = (WaVideoArgs) null;
        this.RotatedTimes = 0;
        this.RelativeCropPos = new System.Windows.Point?();
        this.RelativeCropSize = new Size?();
      }

      public int RotatedTimes { get; set; }

      public bool IsSent { get; set; }

      public bool IsModified => this.RotatedTimes != 0;

      public WaVideoArgs VideoInfo { get; set; }

      public GifArgs GifInfo { get; set; }

      public string Caption
      {
        get => this._caption;
        set
        {
          this._caption = value;
          if (this.VideoInfo == null)
            return;
          this.VideoInfo.Caption = value;
        }
      }

      public Size? RelativeCropSize { get; set; }

      public System.Windows.Point? RelativeCropPos { get; set; }

      public System.Windows.Point ZoomScale { get; set; }

      public double? CropRatio => throw new NotImplementedException();

      public BitmapSource ThumbnailOverride { get; set; }

      public WaVideoArgs getVideoInfo() => this.VideoInfo;

      public BitmapSource GetThumbnail() => this.GetThumbnailForIItem(ref this.thumb_);

      public IObservable<WriteableBitmap> GetBitmapAsync(
        Size maxSize,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
        {
          WriteableBitmap bitmap = this.GetBitmap(maxSize, withCropping, withRotation, withDrawing);
          observer.OnNext(bitmap);
          observer.OnCompleted();
          return (Action) (() => { });
        }));
      }

      public WriteableBitmap GetBitmap(
        Size maxSize,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        bool isVideo = this.VideoInfo != null;
        WriteableBitmap bitmapForIitem = this.GetBitmapForIItem(isVideo, maxSize, withCropping, withRotation, withDrawing);
        if (!isVideo && this.thumb_ == null)
          this.thumb_ = (BitmapSource) ImageStore.CreateThumbnail((BitmapSource) bitmapForIitem, 98, new double?(1.0));
        return bitmapForIitem;
      }

      public void WriteToDrawingBitmapCache(Size maxSize)
      {
        if (this.VideoInfo != null || this.DrawArgs == null || !this.DrawArgs.HasDrawing)
          return;
        this.DrawingBitmapCache = this.ToPicInfo(maxSize).GetBitmap(withDrawing: true);
      }

      public FunXMPP.FMessage.Type GetMediaType()
      {
        if (this.GifInfo != null)
          return FunXMPP.FMessage.Type.Gif;
        return this.VideoInfo == null ? FunXMPP.FMessage.Type.Image : FunXMPP.FMessage.Type.Video;
      }

      public MediaSharingState.PicInfo ToPicInfo(Size maxSize)
      {
        if (this.LocalFilePath == "")
          this.LocalFilePath = MediaUpload.CopyLocal((Stream) this.tempFilestream_, MediaUpload.GenerateMediaFilename("jpg"));
        CameraSharingState.CapturedPicInfo picInfo = new CameraSharingState.CapturedPicInfo(this.tempFilestream_, this.GifInfo);
        picInfo.MaxSize = new Size?(maxSize);
        picInfo.PathForDb = this.LocalFilePath;
        picInfo.RotatedTimes = this.RotatedTimes;
        picInfo.Caption = this.Caption;
        picInfo.RelativeCropPos = this.RelativeCropPos;
        picInfo.RelativeCropSize = this.RelativeCropSize;
        picInfo.ZoomScale = this.ZoomScale;
        picInfo.DrawArgs = this.DrawArgs;
        picInfo.DrawingBitmapCache = this.DrawingBitmapCache;
        return (MediaSharingState.PicInfo) picInfo;
      }

      public wam_enum_media_picker_origin_type FsItemOrigin { get; private set; }

      public long FsItemId() => this.itemId;
    }

    public class CapturedPicInfo : MediaSharingState.PicInfo
    {
      private NativeStream tempFileStream_;
      private GifArgs gifInfo;

      public CapturedPicInfo(NativeStream tempFileStream, GifArgs gifInfo)
      {
        this.tempFileStream_ = tempFileStream;
        this.gifInfo = gifInfo;
      }

      public override IObservable<WriteableBitmap> GetBitmapAsync(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        throw new NotImplementedException();
      }

      public override WriteableBitmap GetBitmap(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        if (this.gifInfo != null)
          throw new NotImplementedException();
        WriteableBitmap bitmap = (WriteableBitmap) null;
        Stream imageStream = this.GetImageStream();
        if (imageStream != null)
        {
          int? orientationToApply = this.Orientation;
          if (!orientationToApply.HasValue)
          {
            ushort? jpegOrientation = JpegUtils.GetJpegOrientation(imageStream);
            orientationToApply = jpegOrientation.HasValue ? new int?((int) jpegOrientation.GetValueOrDefault()) : new int?();
          }
          bitmap = this.AdjustBitmap(this.GetBitmapFromStream(imageStream, orientationToApply, false, false), withCropping, withRotation, withDrawing);
        }
        return bitmap;
      }

      public override Stream GetImageStream() => (Stream) this.tempFileStream_;
    }
  }
}
