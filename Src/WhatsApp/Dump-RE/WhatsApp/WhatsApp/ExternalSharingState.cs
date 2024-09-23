// Decompiled with JetBrains decompiler
// Type: WhatsApp.ExternalSharingState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class ExternalSharingState : MediaSharingState
  {
    private MediaSharingState.IItem item_;

    public override IEnumerable<MediaSharingState.IItem> SelectedItems
    {
      get
      {
        if (this.item_ == null)
          return (IEnumerable<MediaSharingState.IItem>) new MediaSharingState.IItem[0];
        return (IEnumerable<MediaSharingState.IItem>) new MediaSharingState.IItem[1]
        {
          this.item_
        };
      }
    }

    public override int SelectedCount => this.item_ != null ? 1 : 0;

    public ExternalSharingState()
      : base(MediaSharingState.SharingMode.ExternalShare)
    {
    }

    public override void AddItem(MediaSharingState.IItem itemToAdd)
    {
      this.item_ = this.item_ == null ? itemToAdd : throw new NotImplementedException("Can't add multiple items to ExternalSharingState");
    }

    public override void DeleteItem(MediaSharingState.IItem itemToDel)
    {
      throw new NotImplementedException("Can't delete item from ExternalSharingState");
    }

    public override void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null)
    {
      throw new NotImplementedException("Can't delete items from ExternalSharingState");
    }

    protected override void DisposeManagedResources()
    {
      this.item_?.Cleanup();
      this.item_ = (MediaSharingState.IItem) null;
      base.DisposeManagedResources();
    }

    public class Item : MediaSharingState.IItem, MediaSharingState.IItemFieldStats
    {
      private Stream externalStream_;
      private IWAStream waStream;
      private string _caption;
      private BitmapSource thumb_;
      private long itemId = DateTime.Now.Ticks;

      public BitmapSource LargeThumb { get; set; }

      public IDisposable largeThumbSub { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      public Item(Stream tempFilestream)
      {
        this.externalStream_ = tempFilestream;
        this.ZoomScale = new System.Windows.Point(1.0, 1.0);
        this.VideoInfo = (WaVideoArgs) null;
        this.GifInfo = (GifArgs) null;
        this.FsItemOrigin = wam_enum_media_picker_origin_type.SHARE_EXTENSION;
      }

      public Item(Stream externalStream, WaVideoArgs videoArgs)
      {
        this.VideoInfo = videoArgs;
        this.GifInfo = (GifArgs) null;
        this.externalStream_ = externalStream;
        this.ZoomScale = new System.Windows.Point(1.0, 1.0);
        this.LargeThumb = (BitmapSource) this.VideoInfo.LargeThumbnail;
        this.FsItemOrigin = wam_enum_media_picker_origin_type.SHARE_EXTENSION;
      }

      public Item(Stream externalStream, GifArgs gifArgs, WaVideoArgs videoArgs)
      {
        this.externalStream_ = externalStream;
        Log.l("Media Sharing", "Gif stream {0} {1}", (object) this.externalStream_.Position, (object) this.externalStream_.Length);
        this.VideoInfo = videoArgs;
        this.GifInfo = gifArgs;
        this.ZoomScale = new System.Windows.Point(1.0, 1.0);
        this.LargeThumb = (BitmapSource) this.VideoInfo.LargeThumbnail;
        this.FsItemOrigin = wam_enum_media_picker_origin_type.SHARE_EXTENSION;
      }

      public IObservable<WriteableBitmap> FetchLargeThumbAsync()
      {
        return this.FetchLargeThumbForIItemAsync((string) null, this.CloneExternalStream());
      }

      public IObservable<WriteableBitmap> VideoTimelineThumbsAsync()
      {
        if (this.VideoInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(this.VideoInfo.OrientationAngle, (string) null, this.CloneExternalStream());
        Log.l("CameraSharingState.Item", "error fetch video bitmap - video info is null");
        return (IObservable<WriteableBitmap>) null;
      }

      private Stream CloneExternalStream()
      {
        if (this.waStream == null)
          this.waStream = this.externalStream_.ToWaStream(true);
        NativeStream nativeStream = new NativeStream(this.waStream.Clone());
        nativeStream.Position = 0L;
        return (Stream) nativeStream;
      }

      public string GetFullPath() => "";

      public int GetDuration()
      {
        WaVideoArgs videoInfo = this.VideoInfo;
        return videoInfo == null ? 0 : videoInfo.Duration;
      }

      public void Cleanup()
      {
        this.externalStream_?.Dispose();
        this.externalStream_ = (Stream) null;
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

      public Stream TakeOverStream()
      {
        Stream externalStream = this.externalStream_;
        this.externalStream_ = (Stream) null;
        return externalStream;
      }

      public Stream GetGifStream() => this.CloneExternalStream();

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
        ExternalSharingState.SharedPicInfo picInfo = new ExternalSharingState.SharedPicInfo(this.externalStream_, this.GifInfo);
        picInfo.MaxSize = new Size?(maxSize);
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

    public class SharedPicInfo : MediaSharingState.PicInfo
    {
      private Stream tempFileStream_;
      private GifArgs gifInfo;

      public SharedPicInfo(Stream tempFileStream, GifArgs gifInfo)
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

      public override Stream GetImageStream() => this.tempFileStream_;
    }
  }
}
