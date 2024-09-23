// Decompiled with JetBrains decompiler
// Type: WhatsApp.NativeMediaPickerState
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
using Windows.Storage;
using Windows.Storage.Streams;


namespace WhatsApp
{
  public class NativeMediaPickerState : MediaSharingState
  {
    private LinkedList<NativeMediaPickerState.Item> items_ = new LinkedList<NativeMediaPickerState.Item>();

    public override IEnumerable<MediaSharingState.IItem> SelectedItems
    {
      get
      {
        return (IEnumerable<MediaSharingState.IItem>) this.items_.ToArray<NativeMediaPickerState.Item>();
      }
    }

    public override int SelectedCount => this.items_.Count;

    public NativeMediaPickerState()
      : base(MediaSharingState.SharingMode.ChooseMedia)
    {
    }

    public override void AddItem(MediaSharingState.IItem itemToAdd)
    {
      if (!(itemToAdd is NativeMediaPickerState.Item obj))
        return;
      this.items_.AddLast(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Add);
    }

    public override void DeleteItem(MediaSharingState.IItem itemToDel)
    {
      if (!(itemToDel is NativeMediaPickerState.Item obj))
        return;
      this.items_.Remove(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      obj.Cleanup();
    }

    public override void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null)
    {
      LinkedList<NativeMediaPickerState.Item> linkedList1 = new LinkedList<NativeMediaPickerState.Item>();
      LinkedList<NativeMediaPickerState.Item> linkedList2 = new LinkedList<NativeMediaPickerState.Item>();
      foreach (NativeMediaPickerState.Item obj in this.items_)
      {
        if (deletingCriteria == null || deletingCriteria((MediaSharingState.IItem) obj))
          linkedList2.AddLast(obj);
        else
          linkedList1.AddLast(obj);
      }
      this.items_ = linkedList1;
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      foreach (NativeMediaPickerState.Item obj in linkedList2)
        obj.Cleanup();
    }

    protected override void DisposeManagedResources()
    {
      foreach (NativeMediaPickerState.Item obj in this.items_)
        obj.Cleanup();
      base.DisposeManagedResources();
    }

    public class Item : MediaSharingState.IItem, MediaSharingState.IItemFieldStats
    {
      private FileRandomAccessStream tempFilestream_;
      private string _caption;
      private BitmapSource thumb_;
      private long itemId = DateTime.Now.Ticks;

      public BitmapSource LargeThumb { get; set; }

      public IDisposable largeThumbSub { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      public StorageFile file { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      public Item(StorageFile file, FileRandomAccessStream stream)
      {
        this.file = file;
        this.VideoInfo = (WaVideoArgs) null;
        this.tempFilestream_ = stream;
        this.FsItemOrigin = wam_enum_media_picker_origin_type.CHAT_PHOTO_LIBRARY;
      }

      public Item(WaVideoArgs videoargs)
      {
        this.tempFilestream_ = (FileRandomAccessStream) null;
        this.VideoInfo = videoargs;
        this.LargeThumb = (BitmapSource) this.VideoInfo.LargeThumbnail;
        this.FsItemOrigin = wam_enum_media_picker_origin_type.CHAT_PHOTO_LIBRARY;
      }

      public IObservable<WriteableBitmap> FetchLargeThumbAsync()
      {
        return this.FetchLargeThumbForIItemAsync(this.VideoInfo.PreviewPlayPath);
      }

      public IObservable<WriteableBitmap> VideoTimelineThumbsAsync()
      {
        if (this.VideoInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(this.VideoInfo.OrientationAngle, this.VideoInfo.PreviewPlayPath);
        Log.l("NativeMediaPickerState.Item", "error fetch video bitmap - video info is null");
        return (IObservable<WriteableBitmap>) null;
      }

      public string GetFullPath() => this.file.Path;

      public int GetDuration()
      {
        WaVideoArgs videoInfo = this.VideoInfo;
        return videoInfo == null ? 0 : videoInfo.Duration;
      }

      public void Cleanup()
      {
        ((IDisposable) this.tempFilestream_).SafeDispose();
        this.tempFilestream_ = (FileRandomAccessStream) null;
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
        if (this.VideoInfo == null)
          return this.ToPicInfo(maxSize).GetBitmapAsync(withCropping, withRotation, withDrawing);
        return this.LargeThumb == null ? this.FetchLargeThumbAsync() : Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
        {
          observer.OnNext(this.AlterLargeThumbForIItem(withCropping, withRotation));
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
        return this.VideoInfo == null ? FunXMPP.FMessage.Type.Image : FunXMPP.FMessage.Type.Video;
      }

      public MediaSharingState.PicInfo ToPicInfo(Size maxSize)
      {
        NativeMediaPickerState.PickerPicInfo picInfo = new NativeMediaPickerState.PickerPicInfo(((IRandomAccessStream) this.tempFilestream_).AsStream());
        picInfo.MaxSize = new Size?(maxSize);
        picInfo.PathForDb = this.file.Path;
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

    public class PickerPicInfo : MediaSharingState.PicInfo
    {
      private Stream tempFileStream_;

      public PickerPicInfo(Stream tempFileStream) => this.tempFileStream_ = tempFileStream;

      public override IObservable<WriteableBitmap> GetBitmapAsync(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
        {
          observer.OnNext(this.GetBitmap(withCropping, withRotation, withDrawing));
          observer.OnCompleted();
          return (Action) (() => { });
        }));
      }

      public override WriteableBitmap GetBitmap(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
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

      public override Stream GetImageStream() => this.tempFileStream_;
    }
  }
}
