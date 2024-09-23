// Decompiled with JetBrains decompiler
// Type: WhatsApp.GifSharingState
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

#nullable disable
namespace WhatsApp
{
  public class GifSharingState : MediaSharingState
  {
    private LinkedList<GifSharingState.Item> items_ = new LinkedList<GifSharingState.Item>();

    public override IEnumerable<MediaSharingState.IItem> SelectedItems
    {
      get => (IEnumerable<MediaSharingState.IItem>) this.items_.ToArray<GifSharingState.Item>();
    }

    public override int SelectedCount => this.items_.Count;

    public string SearchTerm { get; set; }

    public GifSharingState()
      : base(MediaSharingState.SharingMode.TakePicture)
    {
    }

    public override void AddItem(MediaSharingState.IItem itemToAdd)
    {
      if (!(itemToAdd is GifSharingState.Item obj))
        return;
      this.items_.AddLast(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Add);
    }

    public override void DeleteItem(MediaSharingState.IItem itemToDel)
    {
      if (!(itemToDel is GifSharingState.Item obj))
        return;
      this.items_.Remove(obj);
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      obj.Cleanup();
    }

    public override void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null)
    {
      LinkedList<GifSharingState.Item> linkedList1 = new LinkedList<GifSharingState.Item>();
      LinkedList<GifSharingState.Item> linkedList2 = new LinkedList<GifSharingState.Item>();
      foreach (GifSharingState.Item obj in this.items_)
      {
        if (deletingCriteria == null || deletingCriteria((MediaSharingState.IItem) obj))
          linkedList2.AddLast(obj);
        else
          linkedList1.AddLast(obj);
      }
      this.items_ = linkedList1;
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
      foreach (GifSharingState.Item obj in linkedList2)
        obj.Cleanup();
    }

    protected override void DisposeManagedResources()
    {
      foreach (GifSharingState.Item obj in this.items_)
        obj.Cleanup();
      base.DisposeManagedResources();
    }

    public class Item : MediaSharingState.IItem, MediaSharingState.IItemFieldStats
    {
      private Stream tempFilestream_;
      private string _caption;
      private BitmapSource thumb_;
      private long itemId = DateTime.Now.Ticks;

      public BitmapSource LargeThumb { get; set; }

      public IDisposable largeThumbSub { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      private GifSearchResult gifItem { get; set; }

      public Item(GifSearchResult item, Stream stream)
      {
        this.gifItem = item;
        this.GifInfo = new GifArgs()
        {
          GifAttribution = item.Attribution
        };
        this.tempFilestream_ = stream;
      }

      public IObservable<WriteableBitmap> FetchLargeThumbAsync()
      {
        throw new NotImplementedException();
      }

      public IObservable<WriteableBitmap> VideoTimelineThumbsAsync()
      {
        if (this.GifInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(0, this.GetFullPath(), this.tempFilestream_);
        Log.l("GifSharingState.Item", "error fetch video bitmap - video info is null");
        return (IObservable<WriteableBitmap>) null;
      }

      public string GetFullPath() => this.gifItem.Mp4Path;

      public int GetDuration()
      {
        WaVideoArgs videoInfo = this.VideoInfo;
        return videoInfo == null ? 0 : videoInfo.Duration;
      }

      public Stream GetGifStream() => this.tempFilestream_;

      public void Cleanup()
      {
        this.tempFilestream_.SafeDispose();
        this.tempFilestream_ = (Stream) null;
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

      public FunXMPP.FMessage.Type GetMediaType() => FunXMPP.FMessage.Type.Gif;

      public MediaSharingState.PicInfo ToPicInfo(Size maxSize)
      {
        GifSharingState.GifPicInfo picInfo = new GifSharingState.GifPicInfo(this.gifItem, this.tempFilestream_);
        picInfo.MaxSize = new Size?(maxSize);
        picInfo.PathForDb = this.gifItem.Mp4PreviewPath;
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

    public class GifPicInfo : MediaSharingState.PicInfo
    {
      private Stream urlFileStream;
      private GifSearchResult gifInfo;

      public GifPicInfo(GifSearchResult gif, Stream stream)
      {
        this.gifInfo = gif;
        this.urlFileStream = stream;
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
        return this.AdjustBitmap(new WriteableBitmap((BitmapSource) this.gifInfo.bitmap), withCropping, withRotation, withDrawing);
      }

      public override Stream GetImageStream() => this.urlFileStream;
    }
  }
}
