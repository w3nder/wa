// Decompiled with JetBrains decompiler
// Type: WhatsApp.MediaPickerState
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
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class MediaPickerState : MediaSharingState
  {
    protected Dictionary<int, MediaPickerState.Item> pendingItems_ = new Dictionary<int, MediaPickerState.Item>();

    public bool AllowMultiSelection { get; private set; }

    public override IEnumerable<MediaSharingState.IItem> SelectedItems
    {
      get
      {
        return (IEnumerable<MediaSharingState.IItem>) this.pendingItems_.Where<KeyValuePair<int, MediaPickerState.Item>>((Func<KeyValuePair<int, MediaPickerState.Item>, bool>) (pi => pi.Value.IsSelected)).OrderBy<KeyValuePair<int, MediaPickerState.Item>, DateTime?>((Func<KeyValuePair<int, MediaPickerState.Item>, DateTime?>) (pi => pi.Value.AddedTime)).Select<KeyValuePair<int, MediaPickerState.Item>, MediaPickerState.Item>((Func<KeyValuePair<int, MediaPickerState.Item>, MediaPickerState.Item>) (pi => pi.Value));
      }
    }

    public override int SelectedCount
    {
      get
      {
        return this.pendingItems_.Where<KeyValuePair<int, MediaPickerState.Item>>((Func<KeyValuePair<int, MediaPickerState.Item>, bool>) (p => p.Value.IsSelected)).Count<KeyValuePair<int, MediaPickerState.Item>>();
      }
    }

    public MediaPickerState(bool allowMultiSelection, MediaSharingState.SharingMode mode)
      : base(mode)
    {
      this.AllowMultiSelection = allowMultiSelection && (mode == MediaSharingState.SharingMode.ChoosePicture || mode == MediaSharingState.SharingMode.ChooseMedia || mode == MediaSharingState.SharingMode.ChooseVideo);
    }

    public override void DeleteItem(MediaSharingState.IItem itemToDel)
    {
      if (!(itemToDel is MediaPickerState.Item itemToClear))
        return;
      itemToClear.IsSelected = false;
      this.ClearItemInfo((MediaSharingState.IItem) itemToClear);
      this.ProcessItemSelectionChange(itemToClear);
    }

    public void ClearItemInfo(MediaSharingState.IItem itemToClear)
    {
      itemToClear.VideoInfo = (WaVideoArgs) null;
      itemToClear.LargeThumb = (BitmapSource) null;
      itemToClear.largeThumbSub = (IDisposable) null;
      itemToClear.RelativeCropPos = new System.Windows.Point?();
      itemToClear.RelativeCropSize = new Size?();
      itemToClear.RotatedTimes = 0;
    }

    public override void DeleteItems(
      Func<MediaSharingState.IItem, bool> deletingCriteria = null)
    {
      bool flag = false;
      foreach (KeyValuePair<int, MediaPickerState.Item> pendingItem in this.pendingItems_)
      {
        if (deletingCriteria == null || deletingCriteria((MediaSharingState.IItem) pendingItem.Value))
        {
          pendingItem.Value.IsSelected = false;
          flag = true;
        }
      }
      if (deletingCriteria == null)
        this.pendingItems_.Clear();
      else
        this.pendingItems_ = this.pendingItems_.Where<KeyValuePair<int, MediaPickerState.Item>>((Func<KeyValuePair<int, MediaPickerState.Item>, bool>) (p => !deletingCriteria((MediaSharingState.IItem) p.Value))).ToDictionary<KeyValuePair<int, MediaPickerState.Item>, int, MediaPickerState.Item>((Func<KeyValuePair<int, MediaPickerState.Item>, int>) (p => p.Key), (Func<KeyValuePair<int, MediaPickerState.Item>, MediaPickerState.Item>) (p => p.Value));
      if (!flag)
        return;
      this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
    }

    public override void AddItem(MediaSharingState.IItem itemToAdd)
    {
      if (!(itemToAdd is MediaPickerState.Item obj))
        return;
      obj.IsSelected = true;
      this.ProcessItemSelectionChange(obj);
    }

    public void ProcessItemSelectionChange(MediaPickerState.Item item)
    {
      if (item == null)
        return;
      MediaPickerState.Item obj = (MediaPickerState.Item) null;
      if (this.pendingItems_.TryGetValue(item.ItemId, out obj) && obj != null)
      {
        if (item.IsSelected)
          return;
        this.pendingItems_.Remove(obj.ItemId);
        this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Delete);
        item.Cleanup();
      }
      else
      {
        if (!item.IsSelected)
          return;
        item.AddedTime = new DateTime?(DateTime.Now);
        this.pendingItems_[item.ItemId] = item;
        this.NotifySelectedItemsChanged(MediaSharingState.SelectedItemsChangeCause.Add);
      }
    }

    public override MediaSharingState recreateForRestart()
    {
      MediaPickerState mediaPickerState = new MediaPickerState(this.AllowMultiSelection, this.Mode);
      mediaPickerState.pendingItems_ = this.pendingItems_;
      this.pendingItems_ = (Dictionary<int, MediaPickerState.Item>) null;
      foreach (MediaPickerState.Item obj in mediaPickerState.pendingItems_.Values)
        obj.IsSent = false;
      return (MediaSharingState) mediaPickerState;
    }

    public class Item : 
      MediaMultiSelector.Item,
      MediaSharingState.IItem,
      MediaSharingState.IItemFieldStats
    {
      private int rotatedTimes_;
      private bool forceModified_;
      private string _caption;
      private string imagePath_;
      private long itemId = DateTime.Now.Ticks;

      public WaVideoArgs VideoInfo { get; set; }

      public GifArgs GifInfo { get; set; }

      public DrawingArgs DrawArgs { get; set; }

      public DateTime? AddedTime { get; set; }

      public IMediaItem MediaItem { get; private set; }

      public int RotatedTimes
      {
        get => this.rotatedTimes_;
        set => this.rotatedTimes_ = value;
      }

      public bool IsSent { get; set; }

      public bool IsModified
      {
        get
        {
          return this.RotatedTimes != 0 || this.forceModified_ || this.DrawArgs != null && this.DrawArgs.HasDrawing || this.RelativeCropSize.HasValue;
        }
      }

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

      public System.Windows.Point ZoomScale { get; } = new System.Windows.Point(1.0, 1.0);

      public double? CropRatio
      {
        get
        {
          return !this.RelativeCropSize.HasValue ? new double?() : new double?(this.RelativeCropSize.Value.Width / this.RelativeCropSize.Value.Height);
        }
      }

      public BitmapSource ThumbnailOverride { get; set; }

      public BitmapSource LargeThumb { get; set; }

      public IDisposable largeThumbSub { get; set; }

      public WriteableBitmap DrawingBitmapCache { get; set; }

      public List<WriteableBitmap> MediaTimelineThumbnails { get; set; }

      public override FunXMPP.FMessage.Type MediaType
      {
        get
        {
          if (base.MediaType == FunXMPP.FMessage.Type.Undefined)
          {
            try
            {
              FunXMPP.FMessage.Type? nullable = new FunXMPP.FMessage.Type?();
              switch (this.MediaItem.GetType())
              {
                case MediaItemTypes.Video:
                  base.MediaType = (FunXMPP.FMessage.Type) ((int) nullable ?? 3);
                  break;
                case MediaItemTypes.Picture:
                  string stringAttribute = this.MediaItem.GetStringAttribute(MediaItemStrings.FilePath);
                  if ((stringAttribute != null ? stringAttribute.ExtractFileExtension() : (string) null).ToLower() == "gif")
                  {
                    try
                    {
                      using (Stream input = MediaStorage.OpenFile(stringAttribute))
                      {
                        if (ExternalShareUtils.IsAnimatedGif(input))
                          nullable = new FunXMPP.FMessage.Type?(FunXMPP.FMessage.Type.Gif);
                      }
                    }
                    catch (Exception ex)
                    {
                    }
                  }
                  base.MediaType = (FunXMPP.FMessage.Type) ((int) nullable ?? 1);
                  break;
              }
            }
            catch (Exception ex)
            {
              if (ex.HResult == -2147024809 || ex.HResult == -2143682558)
                Log.l(nameof (MediaType), "{0}", (object) ex.GetFriendlyMessage());
              else
                Log.LogException(ex, "get media item type");
            }
          }
          return base.MediaType;
        }
        protected set => base.MediaType = value;
      }

      public Item(int itemId, wam_enum_media_picker_origin_type itemOrigin, IMediaItem mediaItem)
        : base(itemId)
      {
        this.FsItemOrigin = itemOrigin;
        this.MediaItem = mediaItem;
      }

      public IObservable<WriteableBitmap> FetchLargeThumbAsync()
      {
        return this.FetchLargeThumbForIItemAsync(this.VideoInfo.PreviewPlayPath);
      }

      public IObservable<WriteableBitmap> VideoTimelineThumbsAsync()
      {
        if (this.VideoInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(this.VideoInfo.OrientationAngle, NativeMediaStorage.MakeUri(this.GetFullPath()));
        if (this.GifInfo != null)
          return this.GetVideoTimelineThumbsForIItemAsync(0, NativeMediaStorage.MakeUri(this.GetFullPath()));
        Log.l("MediaPickerState.Item", "error fetch video bitmap - video info is null");
        return (IObservable<WriteableBitmap>) null;
      }

      protected override void DisposeManagedResources()
      {
        this.MediaItem.Dispose();
        this.MediaItem = (IMediaItem) null;
        base.DisposeManagedResources();
      }

      protected override string GetGroupingKey()
      {
        try
        {
          return DateTime.FromFileTime(this.MediaItem.GetFileTimeAttribute(MediaItemTimes.Date)).ToString("y");
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get media item timestamp");
        }
        return "";
      }

      public override IObservable<BitmapSource> GetThumbnailObservable()
      {
        return this.ThumbnailOverride != null ? Observable.Return<BitmapSource>(this.ThumbnailOverride) : Observable.Create<BitmapSource>((Func<IObserver<BitmapSource>, Action>) (observer =>
        {
          IByteBuffer bb = (IByteBuffer) null;
          if (this.MediaItem != null)
          {
            try
            {
              bb = this.MediaItem.GetThumbnail();
            }
            catch (Exception ex)
            {
              bb = (IByteBuffer) null;
              Log.LogException(ex, "fetch media picker item thumbnail in GetThumbnailObservable");
            }
          }
          else
            Log.d("media picker", "no media item for thumbnail observable");
          if (bb == null)
          {
            observer.OnCompleted();
          }
          else
          {
            byte[] bytes = bb.Get();
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              observer.OnNext((BitmapSource) BitmapUtils.CreateBitmap(bytes, 98, 98));
              observer.OnCompleted();
            }));
          }
          return (Action) (() => { });
        }));
      }

      public override BitmapSource GetThumbnail()
      {
        if (this.ThumbnailOverride != null)
          return this.ThumbnailOverride;
        try
        {
          return (BitmapSource) BitmapUtils.CreateBitmap(this.MediaItem.GetThumbnail().Get(), 98, 98);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "fetch media picker item thumbnail in GetThumbnail");
        }
        return base.GetThumbnail();
      }

      private void ClearModifications()
      {
        this.RotatedTimes = 0;
        this.RelativeCropSize = new Size?();
        this.RelativeCropPos = new System.Windows.Point?();
        this.ThumbnailOverride = (BitmapSource) null;
      }

      protected override void OnSelectionToggled()
      {
        base.OnSelectionToggled();
        this.ClearModifications();
      }

      public FunXMPP.FMessage.Type GetMediaType() => this.MediaType;

      public string GetFullPath()
      {
        if (this.VideoInfo != null)
          return this.VideoInfo.FullPath;
        if (this.imagePath_ == null)
          this.imagePath_ = this.MediaItem.GetStringAttribute(MediaItemStrings.FilePath);
        return this.imagePath_;
      }

      public string GetFullPathSafe(string logContext)
      {
        if (this.VideoInfo != null)
          return this.VideoInfo.FullPath;
        if (this.imagePath_ == null)
        {
          try
          {
            this.imagePath_ = this.MediaItem.GetStringAttribute(MediaItemStrings.FilePath);
          }
          catch (Exception ex)
          {
            if (ex.HResult == -2147024809 || ex.HResult == -2143682558)
              Log.l(nameof (GetFullPathSafe), "{0} ex: {1}", (object) logContext, (object) ex.GetFriendlyMessage());
            else
              Log.LogException(ex, "GetFullPathSafe - " + logContext);
          }
        }
        return this.imagePath_;
      }

      public int GetDuration()
      {
        int duration = 0;
        try
        {
          duration = (int) this.MediaItem.GetIntAttribute(MediaItemIntegers.Duration) / 1000;
        }
        catch (Exception ex)
        {
          if (ex.HResult == -2143682549)
          {
            Log.l(nameof (GetDuration), "{0} ex: {1}", (object) (this.imagePath_ ?? "null"), (object) ex.GetFriendlyMessage());
            Log.SendCrashLog(ex, "Unexpected ZERROR_E_UNSUPPORTEDATTR", logOnlyForRelease: true);
          }
          else
            throw;
        }
        return duration;
      }

      public int GetOrientation()
      {
        return new int[9]
        {
          0,
          0,
          0,
          180,
          180,
          0,
          270,
          270,
          0
        }[(int) this.MediaItem.GetIntAttribute(MediaItemIntegers.Orientation)];
      }

      public IObservable<WriteableBitmap> GetBitmapAsync(
        Size maxSize,
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        if (this.MediaType != FunXMPP.FMessage.Type.Video)
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
        return this.GetBitmapForIItem(this.mediaType_ == FunXMPP.FMessage.Type.Video, maxSize, withCropping, withRotation, withDrawing);
      }

      public void WriteToDrawingBitmapCache(Size maxSize)
      {
        if (this.VideoInfo != null || this.DrawArgs == null || !this.DrawArgs.HasDrawing)
          return;
        this.DrawingBitmapCache = this.ToPicInfo(maxSize).GetBitmap(withDrawing: true);
      }

      public MediaSharingState.PicInfo ToPicInfo(Size maxSize)
      {
        int? zmediaOrientation = new int?();
        try
        {
          zmediaOrientation = new int?((int) this.MediaItem.GetIntAttribute(MediaItemIntegers.Orientation));
          int intAttribute1 = (int) this.MediaItem.GetIntAttribute(MediaItemIntegers.Width);
          int intAttribute2 = (int) this.MediaItem.GetIntAttribute(MediaItemIntegers.Height);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "get zmedia attributes");
        }
        int? nullable = zmediaOrientation;
        int num = 1;
        if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          zmediaOrientation = new int?();
        if (zmediaOrientation.HasValue)
          this.forceModified_ = true;
        string fullPath = this.GetFullPath();
        MediaPickerState.ChosenPicInfo picInfo = new MediaPickerState.ChosenPicInfo(fullPath, zmediaOrientation);
        picInfo.MaxSize = new Size?(maxSize);
        picInfo.PathForDb = this.IsModified ? (string) null : fullPath;
        picInfo.RotatedTimes = this.RotatedTimes;
        picInfo.Caption = this.Caption;
        picInfo.RelativeCropPos = this.RelativeCropPos;
        picInfo.RelativeCropSize = this.RelativeCropSize;
        picInfo.DrawArgs = this.DrawArgs;
        picInfo.DrawingBitmapCache = this.DrawingBitmapCache;
        return (MediaSharingState.PicInfo) picInfo;
      }

      public wam_enum_media_picker_origin_type FsItemOrigin { get; private set; }

      public long FsItemId() => this.itemId;

      public void Cleanup()
      {
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
    }

    public class ChosenPicInfo : MediaSharingState.PicInfo
    {
      public string PathForOpen { get; set; }

      public ChosenPicInfo(string pathForOpen, int? zmediaOrientation)
      {
        this.PathForOpen = pathForOpen;
        this.Orientation = zmediaOrientation;
      }

      public override IObservable<WriteableBitmap> GetBitmapAsync(
        bool withCropping = true,
        bool withRotation = true,
        bool withDrawing = false)
      {
        return Observable.Create<WriteableBitmap>((Func<IObserver<WriteableBitmap>, Action>) (observer =>
        {
          Stream imgStream = this.GetImageStream();
          if (imgStream != null)
          {
            int? orientationToApply = this.Orientation;
            if (!orientationToApply.HasValue)
            {
              ushort? jpegOrientation = JpegUtils.GetJpegOrientation(imgStream);
              orientationToApply = jpegOrientation.HasValue ? new int?((int) jpegOrientation.GetValueOrDefault()) : new int?();
            }
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
            {
              using (imgStream)
              {
                observer.OnNext(this.GetBitmapFromStream(imgStream, orientationToApply, withCropping, withRotation, withDrawing));
                observer.OnCompleted();
              }
            }));
          }
          else
          {
            observer.OnNext((WriteableBitmap) null);
            observer.OnCompleted();
          }
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
          using (imageStream)
          {
            int? orientationToApply = this.Orientation;
            if (!orientationToApply.HasValue)
            {
              ushort? jpegOrientation = JpegUtils.GetJpegOrientation(imageStream);
              orientationToApply = jpegOrientation.HasValue ? new int?((int) jpegOrientation.GetValueOrDefault()) : new int?();
            }
            bitmap = this.GetBitmapFromStream(imageStream, orientationToApply, withCropping, withRotation, withDrawing);
          }
        }
        return bitmap;
      }

      public override Stream GetImageStream()
      {
        Stream imageStream = (Stream) null;
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            imageStream = nativeMediaStorage.OpenFile(this.PathForOpen, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
          imageStream = (Stream) null;
          Log.LogException(ex, "get img stream from pic info | picker");
          Log.WriteLineDebug("filename: {0}", (object) this.PathForOpen);
        }
        return imageStream;
      }
    }
  }
}
