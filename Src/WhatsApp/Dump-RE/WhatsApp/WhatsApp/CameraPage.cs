// Decompiled with JetBrains decompiler
// Type: WhatsApp.CameraPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WhatsApp.CommonOps;
using WhatsAppNative;
using Windows.Phone.Media.Capture;

#nullable disable
namespace WhatsApp
{
  public class CameraPage : PhoneApplicationPage
  {
    private const string LogHeader = "cam page";
    private static int NextInstanceImageMaxWidth;
    private static int NextInstanceImageMaxHeight;
    private static TakePicture.Mode NextInstanceMode;
    public static IObserver<TakePicture.CapturedPictureArgs> PictureDataObserver;
    public static bool TakePictureOnly;
    public static bool IsTakingProfilePicture;
    public static bool IsTakingProfilePictureForGroup;
    private bool cameraRollOpen = true;
    private bool keepCameraRollClosed;
    private bool videoCameraRecording;
    private bool TakingPicture;
    public PageOrientation currentOrientation;
    private bool onFirstOrientationChange = true;
    private TakePicture.Mode mode;
    private int imageMaxWidth;
    private int imageMaxHeight;
    private int videoMaxDuration = 60;
    private DispatcherTimer FocusTimer = new DispatcherTimer();
    private MediaListWithName albumsSource;
    private const int MaxCapacity = 1000000;
    private const int MaxLoaded = 50;
    public static int TempFilenameCounter;
    private CaptureSource videoCaptureSource;
    private VideoCaptureDevice videoCaptureDevice;
    private FileSink fileSink;
    private VideoCaptureDevice primaryVideoCamera;
    private VideoCaptureDevice frontFacingVideoCamera;
    private VideoBrush vidBrush;
    private WriteableBitmap thumbnail;
    private IDisposable initialImageSubscription;
    private IDisposable thumbnailSubscription;
    private CameraPage.VideoRecorderState vrState = CameraPage.VideoRecorderState.Busy;
    private DispatcherTimer videoTimer;
    private DispatcherTimer flashTimer;
    private int flashTimerTicks;
    private DateTime recordingStartTime;
    private int duration;
    private DispatcherTimer videoHoldTimer;
    private DateTime videoHoldStartTime;
    private Uri playButtonIconUri;
    private Uri stopButtonIconUri;
    private WACamera currentCamera;
    private WACamera primaryCamera;
    private WACamera frontFacingCamera;
    public bool CapturingPicture;
    public VideoBrush viewfinderBrush;
    public CompositeTransform viewfinderTransform;
    public Storyboard popupTextFadeOut_;
    public Brush transparentGrayBrush_;
    private System.Windows.Point initialswipepoint;
    private System.Windows.Point? CurrentFocusPoint;
    private DispatcherTimer t = new DispatcherTimer();
    private int ellipseTimer;
    private Accelerometer AutoFocusAccelerometer;
    private bool needFocus = true;
    private bool FocusReady = true;
    private Vector3 PreviousAcceleration;
    private double tempwidth;
    private double tempheight;
    internal SolidColorBrush HoldVideoBrush;
    internal Grid LayoutRoot;
    internal Grid Camera;
    internal System.Windows.Shapes.Rectangle VideoRect;
    internal System.Windows.Shapes.Rectangle VideoRectCancelOverlay;
    internal System.Windows.Shapes.Rectangle viewfinderCanvas;
    internal Canvas FocusAtPointCanvas;
    internal Ellipse FocusOnPointEllipse;
    internal Border SettingsBarBorder;
    internal CompositeTransform SettingsBarX;
    internal Button CameraToggleButton;
    internal Image CameraSwapIcon;
    internal Button FlashToggleButton;
    internal Image FlashIcon;
    internal Button AlbumButton;
    internal Image AlbumButtonImage;
    internal Border TipBlockBorder;
    internal TextBlock PhotoTipBlock;
    internal Grid RecordingIndicator;
    internal Canvas RecordingCircle;
    internal Ellipse RecordingDot;
    internal TextBlock TimerBlock;
    internal Grid CameraRoll;
    internal CompositeTransform CameraRollY;
    internal ListBox PreviewItemsList;
    internal Grid BottomPanel;
    internal Canvas CaptureButtonVideoRecordingCanvas;
    internal Ellipse CaptureButtonVideoRecordingBorder;
    internal Border CaptureButtonBorder;
    internal CompositeTransform CameraButtonTransformY;
    internal Button CaptureButton;
    internal Image CaptureButtonImage;
    private bool _contentLoaded;

    private void LoadAsync()
    {
      Log.l("cam page", "async camera roll loading begin");
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        this.albumsSource = ((IEnumerable<MediaListWithName>) NativeInterfaces.MediaLib.GetAlbums().Where<MediaListWithName>((Func<MediaListWithName, bool>) (a => a.Guid == "{9ae241c6-e6cc-4080-a2ba-245e0f7c47c5}")).ToArray<MediaListWithName>()).FirstOrDefault<MediaListWithName>();
        IEnumerable<CameraPage.PreviewItem> picsVideos = (IEnumerable<CameraPage.PreviewItem>) null;
        if (this.CurrentCamera == null || !this.CurrentCamera.IsInitialized)
        {
          Log.l("cam page", "terminating load: {0}", (object) (this.CurrentCamera == null));
        }
        else
        {
          if (this.albumsSource == null || this.albumsSource.List.GetItemCount() <= 0)
            return;
          picsVideos = (IEnumerable<CameraPage.PreviewItem>) this.LoadItems();
          if (picsVideos == null)
            return;
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (this.CurrentCamera == null || !this.CurrentCamera.IsInitialized)
              Log.l("cam page", "terminating preview list: {0}", (object) (this.CurrentCamera == null));
            else
              this.PreviewItemsList.ItemsSource = (IEnumerable) picsVideos;
          }));
        }
      }));
    }

    private LinkedList<CameraPage.PreviewItem> LoadItems()
    {
      Log.l("cam page", "load camera roll items");
      LinkedList<MediaPickerState.Item> linkedList1 = new LinkedList<MediaPickerState.Item>();
      LinkedList<CameraPage.PreviewItem> linkedList2 = new LinkedList<CameraPage.PreviewItem>();
      int itemCount = this.albumsSource.List.GetItemCount();
      for (int Index = itemCount - 1; Index >= (itemCount - 50 > 0 ? itemCount - 50 : 0); --Index)
        linkedList1.AddLast(new MediaPickerState.Item(1000000 + Index, wam_enum_media_picker_origin_type.CHAT_BUTTON_CAMERA_MEDIA_STRIP, this.albumsSource.List.GetItem(Index)));
      foreach (MediaPickerState.Item obj in (IEnumerable<MediaPickerState.Item>) linkedList1)
        linkedList2.AddLast(new CameraPage.PreviewItem((MediaSharingState.IItem) obj));
      return linkedList2;
    }

    private IEnumerable<MediaPickerState.Item> OrderByDate(IEnumerable<MediaPickerState.Item> source)
    {
      return source.Select(item =>
      {
        try
        {
          var data = new
          {
            Item = item,
            FileTime = item.MediaItem.GetFileTimeAttribute(MediaItemTimes.Date)
          };
          return data;
        }
        catch (Exception ex)
        {
          var data = null;
          return data;
        }
      }).Where(item => item != null).OrderBy(item => item.FileTime).Select(item => item.Item);
    }

    private void PreviewItemsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      CameraPage.PreviewItem previewItem = (CameraPage.PreviewItem) null;
      IEnumerator enumerator = e.AddedItems.GetEnumerator();
      if (enumerator != null)
      {
        try
        {
          enumerator.MoveNext();
          previewItem = enumerator.Current as CameraPage.PreviewItem;
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Camera page PreviewItemsList_SelectionChanged exception", logOnlyForRelease: true);
          previewItem = (CameraPage.PreviewItem) null;
        }
      }
      if (previewItem == null)
        return;
      this.CurrentCamera.Enabled = false;
      this.viewfinderCanvas.Opacity = 0.0;
      this.SettingsBarBorder.Visibility = Visibility.Collapsed;
      MediaPickerState.Item bindedMediaItem = previewItem.BindedMediaItem as MediaPickerState.Item;
      bool flag = false;
      if (bindedMediaItem == null)
      {
        Log.l("cam page", "null selected item");
      }
      else
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            using (Stream stream = nativeMediaStorage.OpenFile(bindedMediaItem.GetFullPath(), FileMode.Open, FileAccess.Read))
            {
              if (stream.Length == 0L)
                throw new Exception("Length was 0");
            }
          }
        }
        catch (Exception ex1)
        {
          flag = true;
          MediaItemTypes mediaItemTypes = MediaItemTypes.Unknown;
          try
          {
            mediaItemTypes = bindedMediaItem.MediaItem.GetType();
          }
          catch (Exception ex2)
          {
          }
          Log.l(ex1, string.Format("open media from camera roll | type:{0}", (object) mediaItemTypes));
          string messageBoxText;
          switch (mediaItemTypes)
          {
            case MediaItemTypes.Video:
              messageBoxText = AppResources.CouldNotOpenVideo;
              break;
            case MediaItemTypes.Picture:
              messageBoxText = AppResources.CouldNotOpenPhoto;
              break;
            default:
              messageBoxText = AppResources.CouldNotOpenMediaFile;
              break;
          }
          int num = (int) MessageBox.Show(messageBoxText);
        }
        if (flag)
          return;
        previewItem.ThumbnailOpacity = 0.6;
        CameraPage.PictureDataObserver.OnNext(new TakePicture.CapturedPictureArgs(bindedMediaItem));
      }
    }

    public async void Initialize(WACamera waCam)
    {
      if (!await waCam.InitializeCamera())
      {
        Log.l("cam page", "terminating initialization");
      }
      else
      {
        if (waCam.cameraState == CameraLoadedState.Unloaded)
        {
          waCam.cam.Dispose();
          waCam.cam = (PhotoCaptureDevice) null;
        }
        else
          this.CameraInitialized();
        waCam.IsInitialized = true;
        waCam.Enabled = true;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.CurrentCamera == null || !this.CurrentCamera.IsInitialized)
          {
            Log.l("cam page", "terminating initialization: {0}", (object) (this.CurrentCamera == null));
          }
          else
          {
            this.viewfinderBrush.SetSource((object) this.CurrentCamera.cam);
            this.viewfinderTransform.Rotation = (double) this.CurrentCamera.CameraRotationOffset;
            this.viewfinderCanvas.Fill = (Brush) this.viewfinderBrush;
            if (this.CurrentCamera.FlipImage)
              this.viewfinderTransform.ScaleY = -1.0;
            else
              this.viewfinderTransform.ScaleY = 1.0;
          }
        }));
        if (CameraPage.TakePictureOnly || CameraPage.IsTakingProfilePicture)
          return;
        this.LoadAsync();
      }
    }

    private string TempFilename
    {
      get => "tmp/video" + (object) CameraPage.TempFilenameCounter + ".tmp.mp4";
    }

    private CameraPage.VideoRecorderState VRState
    {
      get => this.vrState;
      set
      {
        this.vrState = value;
        this.UpdateUI();
      }
    }

    public Uri PlayButtonIconUri
    {
      get
      {
        Uri playButtonIconUri = this.playButtonIconUri;
        return (object) playButtonIconUri != null ? playButtonIconUri : (this.playButtonIconUri = new Uri("/Images/play.png", UriKind.Relative));
      }
    }

    public Uri StopButtonIconUri
    {
      get
      {
        Uri stopButtonIconUri = this.stopButtonIconUri;
        return (object) stopButtonIconUri != null ? stopButtonIconUri : (this.stopButtonIconUri = new Uri("/Images/stop.png", UriKind.Relative));
      }
    }

    private void UpdateUI()
    {
      if (this.VRState == CameraPage.VideoRecorderState.Initialized)
        this.TimerBlock.Text = "00:00";
      this.PhotoTipBlock.Opacity = this.VRState == CameraPage.VideoRecorderState.Recording ? 0.65 : 1.0;
    }

    private void StartVideoTimer()
    {
      if (this.videoTimer == null)
      {
        this.videoTimer = new DispatcherTimer();
        this.videoTimer.Tick += new EventHandler(this.VideoTimer_Tick);
        this.videoTimer.Interval = TimeSpan.FromMilliseconds(500.0);
      }
      this.recordingStartTime = DateTime.Now;
      this.duration = 0;
      this.videoTimer.Start();
    }

    private void StartFlashTimer()
    {
      if (this.flashTimer == null)
      {
        this.flashTimer = new DispatcherTimer();
        this.flashTimer.Tick += new EventHandler(this.FlashTimer_Tick);
        this.flashTimer.Interval = TimeSpan.FromMilliseconds(500.0);
      }
      this.flashTimerTicks = 0;
      this.flashTimer.Start();
    }

    private void StopFlashTimer()
    {
      if (this.flashTimer == null)
        return;
      this.flashTimer.Stop();
      this.flashTimer = (DispatcherTimer) null;
    }

    private void StartHoldTimer()
    {
      if (this.videoHoldTimer != null)
        return;
      this.videoHoldTimer = new DispatcherTimer();
      this.videoHoldTimer.Tick += new EventHandler(this.HoldTimer_Tick);
      this.videoHoldTimer.Interval = TimeSpan.FromMilliseconds(500.0);
      this.videoHoldStartTime = DateTime.Now;
      this.videoHoldTimer.Start();
    }

    private void DisposeImageSubscriptions()
    {
      this.thumbnailSubscription.SafeDispose();
      this.thumbnailSubscription = (IDisposable) null;
      this.initialImageSubscription.SafeDispose();
      this.initialImageSubscription = (IDisposable) null;
    }

    private void InitializeVideoCamera(VideoCaptureDevice device = null, Action onComplete = null)
    {
      this.VRState = CameraPage.VideoRecorderState.Busy;
      this.DisposeImageSubscriptions();
      if (this.videoCaptureSource != null)
        this.videoCaptureSource.Stop();
      this.WaitForUIUpdate().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.CurrentCamera == null)
        {
          Log.l("cam page", "Stopping InitializeVideoCamera, looks like user has closed page");
        }
        else
        {
          this.videoCaptureDevice = device ?? CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
          if (this.videoCaptureDevice == null)
          {
            int num = (int) MessageBox.Show(AppResources.VideoCaptureNoDevice);
            this.NavigationService.GoBack();
          }
          else
          {
            VideoFormat highFormat = this.videoCaptureDevice.SupportedFormats.OrderByDescending<VideoFormat, int>((Func<VideoFormat, int>) (vf => vf.PixelWidth)).FirstOrDefault<VideoFormat>();
            if (highFormat != null)
              this.videoCaptureDevice.DesiredFormat = this.videoCaptureDevice.SupportedFormats.Where<VideoFormat>((Func<VideoFormat, bool>) (vf => vf.PixelFormat == highFormat.PixelFormat)).OrderByDescending<VideoFormat, int>((Func<VideoFormat, int>) (vf => vf.PixelWidth)).LastOrDefault<VideoFormat>() ?? highFormat;
            this.InitVideoCaptureSource();
            this.initialImageSubscription = this.videoCaptureSource.GetImageCompletedAsync().Take<IEvent<CaptureImageCompletedEventArgs>>(1).Subscribe<IEvent<CaptureImageCompletedEventArgs>>((Action<IEvent<CaptureImageCompletedEventArgs>>) (args => this.VRState = CameraPage.VideoRecorderState.Initialized));
          }
        }
      }));
    }

    private void InitVideoCaptureSource()
    {
      this.videoCaptureSource = new CaptureSource();
      this.videoCaptureSource.CaptureFailed += new EventHandler<ExceptionRoutedEventArgs>(this.CaptureSource_CaptureFailed);
      this.videoCaptureSource.VideoCaptureDevice = this.videoCaptureDevice;
      this.vidBrush = new VideoBrush();
      if (this.CurrentCamera.CameraType == CameraType.FrontFacing)
      {
        this.videoCaptureDevice = this.frontFacingVideoCamera;
        this.vidBrush.RelativeTransform = (Transform) new CompositeTransform()
        {
          Rotation = 90.0,
          CenterX = 0.5,
          CenterY = 0.5,
          ScaleX = -1.0,
          ScaleY = 1.0
        };
      }
      else
      {
        this.videoCaptureDevice = this.primaryVideoCamera;
        this.vidBrush.RelativeTransform = (Transform) new CompositeTransform()
        {
          Rotation = 90.0,
          CenterX = 0.5,
          CenterY = 0.5,
          ScaleX = 1.0,
          ScaleY = 1.0
        };
      }
      this.vidBrush.SetSource(this.videoCaptureSource);
      this.VideoRect.Fill = (Brush) this.vidBrush;
    }

    private void CleanUp()
    {
      this.DisposeImageSubscriptions();
      if (this.videoCaptureSource != null)
      {
        this.videoCaptureSource.Stop();
        this.videoCaptureSource = (CaptureSource) null;
      }
      if (this.videoTimer != null)
      {
        this.videoTimer.Stop();
        this.videoTimer = (DispatcherTimer) null;
      }
      if (this.videoHoldTimer != null)
      {
        this.videoHoldTimer.Stop();
        this.videoHoldTimer = (DispatcherTimer) null;
      }
      this.StopFlashTimer();
    }

    private IObservable<Unit> WaitForUIUpdate()
    {
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
      {
        IDisposable disposable = this.GetLayoutUpdatedAsync().Take<Unit>(1).ObserveOnDispatcher<Unit>().Subscribe(observer);
        this.InvalidateMeasure();
        this.UpdateLayout();
        return disposable;
      })).SubscribeOnDispatcher<Unit>();
    }

    private void StartVideoRecording()
    {
      this.VideoRect.Opacity = 1.0;
      this.RecordingIndicator.Visibility = Visibility.Visible;
      this.TipBlockBorder.Visibility = Visibility.Collapsed;
      this.StopFlashTimer();
      UIUtils.EnableWakeLock(true);
      if (this.videoCaptureDevice == null || this.videoCaptureSource?.VideoCaptureDevice == null)
        return;
      this.DisposeImageSubscriptions();
      if (this.CurrentCamera.cam != null)
        this.CurrentCamera.Dispose();
      if (this.videoCaptureSource.State == CaptureState.Started)
        this.videoCaptureSource.Stop();
      this.VRState = CameraPage.VideoRecorderState.Busy;
      this.WaitForUIUpdate().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.fileSink == null)
          this.fileSink = new FileSink();
        this.InitVideoCaptureSource();
        this.fileSink.CaptureSource = this.videoCaptureSource;
        this.fileSink.IsolatedStorageFileName = this.TempFilename;
        this.IsEnabled = false;
        int orientation = 6;
        if (this.CurrentCamera.CameraType == CameraType.FrontFacing)
        {
          orientation = 8;
          switch (this.currentOrientation)
          {
            case PageOrientation.LandscapeLeft:
              orientation = 2;
              break;
            case PageOrientation.LandscapeRight:
              orientation = 4;
              break;
          }
        }
        else
        {
          switch (this.currentOrientation)
          {
            case PageOrientation.LandscapeLeft:
              orientation = 1;
              break;
            case PageOrientation.LandscapeRight:
              orientation = 3;
              break;
          }
        }
        this.thumbnailSubscription = this.videoCaptureSource.GetImageCompletedAsync().Take<IEvent<CaptureImageCompletedEventArgs>>(1).ObserveOnDispatcher<IEvent<CaptureImageCompletedEventArgs>>().Subscribe<IEvent<CaptureImageCompletedEventArgs>>((Action<IEvent<CaptureImageCompletedEventArgs>>) (args =>
        {
          WriteableBitmap result = args.EventArgs.Result;
          if (result == null)
            return;
          this.StartVideoTimer();
          this.IsEnabled = true;
          this.VRState = CameraPage.VideoRecorderState.Recording;
          this.thumbnail = JpegUtils.ApplyJpegOrientation((BitmapSource) result, new int?(orientation));
        }));
        this.videoCaptureSource.Start();
        this.videoCaptureSource.CaptureImageAsync();
        this.videoCameraRecording = true;
      }));
    }

    private void StopVideoRecording(bool useMedia)
    {
      UIUtils.EnableWakeLock(false);
      if (this.videoCaptureSource == null || this.videoCaptureSource.State != CaptureState.Started)
        return;
      this.TimerBlock.Text = "00:00";
      this.TipBlockBorder.Visibility = Visibility.Visible;
      this.RecordingIndicator.Visibility = Visibility.Collapsed;
      this.videoCaptureSource.Stop();
      this.VRState = CameraPage.VideoRecorderState.Ready;
      this.videoCameraRecording = false;
      this.CleanUp();
      if (useMedia)
        this.UseMedia();
      else
        this.Dispatcher.BeginInvoke((Action) (() => this.InitializeVideoCamera()));
    }

    private void CancelVideo(bool saveVideo = false)
    {
      this.VideoRectCancelOverlay.Opacity = 0.0;
      this.CaptureButtonBorder.Background = (Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 66, (byte) 0, (byte) 0, (byte) 0));
      this.viewfinderCanvas.Opacity = 1.0;
      this.VideoRect.Opacity = 0.0;
      this.SettingsBarBorder.Visibility = Visibility.Visible;
      this.CurrentCamera.Enabled = true;
      this.StopVideoRecording(false);
      if (Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.FrontFacing) && this.CurrentCamera.CameraType == CameraType.FrontFacing && this.frontFacingVideoCamera != null)
      {
        this.CurrentCamera = this.FrontFacingCamera;
        this.InitializeVideoCamera(this.frontFacingVideoCamera);
      }
      else if (Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.Primary) && this.primaryVideoCamera != null)
      {
        this.CurrentCamera = this.PrimaryCamera;
        this.InitializeVideoCamera(this.primaryVideoCamera);
      }
      this.CaptureButtonVideoRecordingBorder.Opacity = 0.0;
      if (!saveVideo)
        return;
      this.UseMedia(true);
    }

    private void UseMedia(bool saveMediaOnly = false)
    {
      if (CameraPage.PictureDataObserver == null)
        return;
      Stream stream = (Stream) null;
      try
      {
        string tempFilename = this.TempFilename;
        int angle = this.CurrentCamera.CameraType == CameraType.FrontFacing ? 90 : 270;
        switch (this.currentOrientation)
        {
          case PageOrientation.LandscapeLeft:
            angle = 0;
            break;
          case PageOrientation.LandscapeRight:
            angle = 180;
            break;
        }
        string str1;
        try
        {
          Mp4Atom.OrientationMatrix orientationMatrix = VideoFrameGrabber.MatrixForAngle(angle);
          if (orientationMatrix != null)
            VideoFrameGrabber.WriteRotationMatrix(tempFilename, orientationMatrix.Matrix);
          str1 = MediaDownload.SaveMedia(tempFilename, FunXMPP.FMessage.Type.Video, saveAlbum: "Camera Roll");
          if (str1 != null)
          {
            using (IMediaStorage mediaStorage = MediaStorage.Create(tempFilename))
              stream = mediaStorage.OpenFile(tempFilename);
            if (str1.StartsWith("file:"))
              str1 = str1.Substring(5);
          }
        }
        catch (Exception ex)
        {
          Log.l(ex, "camera page save media");
          str1 = (string) null;
          stream.SafeDispose();
          stream = (Stream) null;
        }
        if (saveMediaOnly)
          return;
        string str2;
        if (stream == null)
        {
          Log.l("cam page", "album save failed - save to iso store");
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(this.TempFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
              try
              {
                using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
                {
                  NativeStream tempFile = nativeMediaStorage.GetTempFile();
                  storageFileStream.CopyTo((Stream) tempFile);
                  stream = (Stream) tempFile;
                }
              }
              catch (Exception ex)
              {
                Log.l(ex, "copy local file to native stream");
                MemoryStream destination = new MemoryStream();
                storageFileStream.CopyTo((Stream) destination);
                stream = (Stream) destination;
              }
              stream.Position = 0L;
            }
          }
          string mediaFilename = MediaUpload.GenerateMediaFilename("mp4");
          str2 = MediaUpload.CopyLocal(stream, mediaFilename);
        }
        else
          str2 = (string) null;
        CameraPage.PictureDataObserver.OnNext(new TakePicture.CapturedPictureArgs((NativeStream) null, (WriteableBitmap) null, new WaVideoArgs()
        {
          PreviewPlayPath = str2,
          Stream = stream,
          Thumbnail = this.thumbnail,
          LargeThumbnail = this.thumbnail,
          Duration = this.duration,
          FileExtension = "mp4",
          ContentType = "video/mp4",
          FullPath = str1,
          OrientationAngle = angle,
          IsCameraVideo = true
        }));
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "camera page return captured video");
        stream.SafeDispose();
        CameraPage.PictureDataObserver.OnError(ex);
      }
    }

    private WACamera CurrentCamera
    {
      get => this.currentCamera;
      set
      {
        if (this.currentCamera != null)
          this.currentCamera.Dispose();
        this.currentCamera = value;
        this.viewfinderCanvas.Fill = (Brush) UIUtils.BackgroundBrush;
        this.FlashToggleButton.Visibility = Visibility.Collapsed;
        this.CameraToggleButton.Visibility = Visibility.Collapsed;
        if (this.currentCamera == null)
          return;
        this.Initialize(this.currentCamera);
      }
    }

    private WACamera PrimaryCamera
    {
      get
      {
        if (this.primaryCamera == null)
          this.primaryCamera = new WACamera(CameraType.Primary);
        return this.primaryCamera;
      }
    }

    private WACamera FrontFacingCamera
    {
      get
      {
        if (this.frontFacingCamera == null)
          this.frontFacingCamera = new WACamera(CameraType.FrontFacing);
        return this.frontFacingCamera;
      }
    }

    public Storyboard PopupTextFadeOut
    {
      get
      {
        if (this.popupTextFadeOut_ == null)
        {
          this.popupTextFadeOut_ = new Storyboard();
          DoubleAnimation doubleAnimation = new DoubleAnimation();
          QuadraticEase quadraticEase = new QuadraticEase();
          quadraticEase.EasingMode = EasingMode.EaseInOut;
          doubleAnimation.EasingFunction = (IEasingFunction) quadraticEase;
          doubleAnimation.Duration = (Duration) TimeSpan.FromMilliseconds(800.0);
          doubleAnimation.From = new double?(1.0);
          doubleAnimation.To = new double?(0.0);
          DoubleAnimation element = doubleAnimation;
          Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Opacity", new object[0]));
          this.popupTextFadeOut_.Children.Add((Timeline) element);
        }
        return this.popupTextFadeOut_;
      }
    }

    public Brush TransparentGrayBrush
    {
      get
      {
        if (this.transparentGrayBrush_ == null)
          this.transparentGrayBrush_ = (Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 170, (byte) 68, (byte) 68, (byte) 68));
        return this.transparentGrayBrush_;
      }
    }

    public CameraPage()
    {
      this.InitializeComponent();
      this.imageMaxWidth = CameraPage.NextInstanceImageMaxWidth;
      this.imageMaxHeight = CameraPage.NextInstanceImageMaxHeight;
      this.mode = CameraPage.NextInstanceMode;
      CameraPage.NextInstanceImageMaxHeight = 0;
      CameraPage.NextInstanceImageMaxWidth = 0;
      CameraPage.NextInstanceMode = TakePicture.Mode.Regular;
      if (this.mode == TakePicture.Mode.StatusOnly)
        this.videoMaxDuration = Settings.StatusVideoMaxDuration;
      this.viewfinderBrush = new VideoBrush();
      this.viewfinderTransform = new CompositeTransform();
      this.viewfinderTransform.CenterX = 0.5;
      this.viewfinderTransform.CenterY = 0.5;
      this.viewfinderBrush.RelativeTransform = (Transform) this.viewfinderTransform;
      this.CaptureButtonImage.Source = (System.Windows.Media.ImageSource) AssetStore.LumiaCameraIcon;
      this.TipBlockBorder.Visibility = Visibility.Visible;
      if (CameraPage.IsTakingProfilePicture)
        this.TipBlockBorder.Visibility = Visibility.Collapsed;
      this.t.Interval = TimeSpan.FromSeconds(0.1);
      this.t.Tick += new EventHandler(this.flashEllipse);
      this.FocusTimer.Interval = TimeSpan.FromMilliseconds(1000.0);
      this.FocusTimer.Tick += (EventHandler) ((d, p) =>
      {
        this.FocusTimer.Stop();
        this.FocusReady = true;
      });
      bool flag = false;
      try
      {
        flag = NativeInterfaces.MediaLib.GetAlbums().Any<MediaListWithName>((Func<MediaListWithName, bool>) (a => a.Guid != "{9ae241c6-e6cc-4080-a2ba-245e0f7c47c6}" && a.List.GetItemCount() > 0));
      }
      catch (Exception ex)
      {
        Log.l(ex, "cam page failed getting any album item count");
      }
      if (CameraPage.TakePictureOnly || CameraPage.IsTakingProfilePicture || !flag)
      {
        CameraPage.TakePictureOnly = false;
        this.keepCameraRollClosed = true;
        this.PreviewItemsList.Visibility = Visibility.Collapsed;
        this.AlbumButton.Visibility = Visibility.Collapsed;
        this.AnimateCollapseList();
      }
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        this.Width = this.LayoutRoot.ActualWidth;
        this.Height = this.LayoutRoot.ActualHeight;
      }));
    }

    public static IObservable<TakePicture.CapturedPictureArgs> Start(
      TakePicture.Mode mode,
      int imgMaxWidth,
      int imgMaxHeight,
      bool isProfile = false,
      bool isGroupProfile = false)
    {
      return Observable.Create<TakePicture.CapturedPictureArgs>((Func<IObserver<TakePicture.CapturedPictureArgs>, Action>) (observer =>
      {
        CameraPage.NextInstanceMode = mode;
        CameraPage.NextInstanceImageMaxWidth = imgMaxWidth;
        CameraPage.NextInstanceImageMaxHeight = imgMaxHeight;
        CameraPage.IsTakingProfilePicture = isProfile;
        CameraPage.PictureDataObserver = observer;
        CameraPage.IsTakingProfilePictureForGroup = isGroupProfile;
        NavUtils.NavigateToPage(nameof (CameraPage));
        return (Action) (() => { });
      }));
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      PageOrientation orientation = e.Orientation;
      if (this.onFirstOrientationChange)
      {
        base.OnOrientationChanged(e);
        this.onFirstOrientationChange = false;
        this.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
        this.currentOrientation = PageOrientation.PortraitUp;
      }
      Log.d("cam page", "ORIENTATION CHANGED: {0}", (object) orientation.ToString());
      RotateTransition rotateTransition = new RotateTransition();
      RotateTransform rotateTransform = new RotateTransform();
      MatrixTransform matrixTransform = new MatrixTransform();
      switch (orientation)
      {
        case PageOrientation.Portrait:
        case PageOrientation.PortraitUp:
          int anglefrom1 = this.currentOrientation != PageOrientation.LandscapeLeft ? -90 : 90;
          this.RotateElement(anglefrom1, 0, (UIElement) this.CaptureButton);
          this.RotateElement(anglefrom1, 0, (UIElement) this.FlashToggleButton);
          this.RotateElement(anglefrom1, 0, (UIElement) this.CameraToggleButton);
          break;
        case PageOrientation.Landscape:
        case PageOrientation.LandscapeRight:
          int anglefrom2 = this.currentOrientation != PageOrientation.PortraitUp ? 90 : 0;
          this.RotateElement(anglefrom2, -90, (UIElement) this.CaptureButton);
          this.RotateElement(anglefrom2, -90, (UIElement) this.FlashToggleButton);
          this.RotateElement(anglefrom2, -90, (UIElement) this.CameraToggleButton);
          break;
        case PageOrientation.LandscapeLeft:
          int anglefrom3 = this.currentOrientation != PageOrientation.LandscapeRight ? 0 : -90;
          this.RotateElement(anglefrom3, 90, (UIElement) this.CaptureButton);
          this.RotateElement(anglefrom3, 90, (UIElement) this.FlashToggleButton);
          this.RotateElement(anglefrom3, 90, (UIElement) this.CameraToggleButton);
          break;
      }
      this.currentOrientation = orientation;
    }

    public void RotateElement(int anglefrom, int angle, UIElement element)
    {
      RotateTransform target = new RotateTransform();
      target.CenterX = element.RenderSize.Width / 2.0;
      target.CenterY = element.RenderSize.Height / 2.0;
      if (element.RenderSize.Width == 0.0 || element.RenderSize.Height == 0.0)
      {
        target.CenterX = 25.0;
        target.CenterY = 25.0;
      }
      Duration duration = new Duration(TimeSpan.FromSeconds(1.0));
      Storyboard storyboard = new Storyboard();
      storyboard.Duration = duration;
      DoubleAnimation element1 = new DoubleAnimation();
      element1.Duration = duration;
      storyboard.Children.Add((Timeline) element1);
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) target);
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("Angle", new object[0]));
      element1.From = new double?((double) anglefrom);
      element1.To = new double?((double) angle);
      element.RenderTransform = (Transform) target;
      storyboard.Begin();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (CameraPage.PictureDataObserver == null)
      {
        NavUtils.GoBack(this.NavigationService);
      }
      else
      {
        CameraButtons.ShutterKeyHalfPressed += new EventHandler(this.OnButtonHalfPress);
        CameraButtons.ShutterKeyPressed += new EventHandler(this.OnButtonFullPress);
        this.FlashToggleButton.Visibility = Visibility.Collapsed;
        this.CameraToggleButton.Visibility = Visibility.Collapsed;
        this.FocusOnPointEllipse.Visibility = Visibility.Collapsed;
        foreach (VideoCaptureDevice videoCaptureDevice in CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices())
        {
          if (videoCaptureDevice.FriendlyName == "Primary camera")
            this.primaryVideoCamera = videoCaptureDevice;
          if (videoCaptureDevice.FriendlyName == "Self portrait camera")
            this.frontFacingVideoCamera = videoCaptureDevice;
        }
        if (CameraPage.IsTakingProfilePicture && !CameraPage.IsTakingProfilePictureForGroup && Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.FrontFacing) && this.frontFacingVideoCamera != null)
        {
          this.CurrentCamera = this.FrontFacingCamera;
          this.FlashToggleButton.Opacity = 0.4;
          this.InitializeVideoCamera(this.frontFacingVideoCamera);
        }
        else
        {
          CameraType? cameraTypeSetting = Settings.UserCameraTypeSetting;
          if (cameraTypeSetting.HasValue)
          {
            cameraTypeSetting = Settings.UserCameraTypeSetting;
            CameraType cameraType = CameraType.Primary;
            if ((cameraTypeSetting.GetValueOrDefault() == cameraType ? (cameraTypeSetting.HasValue ? 1 : 0) : 0) == 0)
              goto label_19;
          }
          if (Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.Primary) && this.primaryVideoCamera != null)
          {
            this.CurrentCamera = this.PrimaryCamera;
            this.FlashToggleButton.Opacity = 1.0;
            this.InitializeVideoCamera(this.primaryVideoCamera);
            goto label_22;
          }
label_19:
          if (Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.FrontFacing) && this.frontFacingVideoCamera != null)
          {
            this.CurrentCamera = this.FrontFacingCamera;
            this.FlashToggleButton.Opacity = 0.4;
            this.InitializeVideoCamera(this.frontFacingVideoCamera);
          }
          else
          {
            Log.l("cam page", "No supported camera on device");
            NavUtils.GoBack(this.NavigationService);
            return;
          }
        }
label_22:
        if (this.Orientation == PageOrientation.PortraitUp)
        {
          this.SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;
          this.onFirstOrientationChange = false;
        }
        ++CameraPage.TempFilenameCounter;
        this.currentOrientation = PageOrientation.PortraitUp;
        if (e.NavigationMode != NavigationMode.Back)
          return;
        this.AnimateTakePicture(true);
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      Log.d("cam page", "navigated from");
      base.OnNavigatedFrom(e);
      if (this.videoCameraRecording)
        this.CancelVideo(true);
      CameraButtons.ShutterKeyHalfPressed -= new EventHandler(this.OnButtonHalfPress);
      CameraButtons.ShutterKeyPressed -= new EventHandler(this.OnButtonFullPress);
      UIUtils.EnableWakeLock(false);
      this.CleanUp();
      if (this.AutoFocusAccelerometer != null)
      {
        this.AutoFocusAccelerometer.Stop();
        this.AutoFocusAccelerometer.Dispose();
        this.AutoFocusAccelerometer = (Accelerometer) null;
      }
      this.CurrentCamera = (WACamera) null;
      if (this.PrimaryCamera != null && this.PrimaryCamera.IsInitialized)
        this.PrimaryCamera.Dispose();
      if (this.FrontFacingCamera == null || !this.FrontFacingCamera.IsInitialized)
        return;
      this.FrontFacingCamera.Dispose();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      UIUtils.EnableWakeLock(false);
      this.CleanUp();
      if (CameraPage.PictureDataObserver != null)
        CameraPage.PictureDataObserver.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    private void CaptureSource_CaptureFailed(object sender, ExceptionRoutedEventArgs e)
    {
      Log.SendCrashLog(e.ErrorException, "CaptureSource");
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        int num = (int) MessageBox.Show(AppResources.VideoCaptureFailed);
        this.NavigationService.GoBack();
      }));
    }

    private void CaptureButton_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.CaptureButtonBorder.Background = (Brush) UIUtils.AccentBrush;
    }

    private void CaptureButton_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (!this.videoCameraRecording)
        return;
      if (e.CumulativeManipulation.Translation.Y < -30.0)
      {
        this.VideoRectCancelOverlay.Opacity = 0.3;
        this.PhotoTipBlock.Text = AppResources.CameraButtonVideoCancelText;
      }
      else
      {
        this.VideoRectCancelOverlay.Opacity = 0.0;
        this.PhotoTipBlock.Text = AppResources.CameraButtonVideoText;
      }
    }

    private void CaptureButton_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      this.CaptureButtonBorder.Background = (Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 66, (byte) 0, (byte) 0, (byte) 0));
      if (!this.videoCameraRecording)
        return;
      this.VideoRectCancelOverlay.Opacity = 0.0;
      this.PhotoTipBlock.Text = AppResources.CameraButtonText;
      if (e.TotalManipulation.Translation.Y < -30.0 || this.duration < 2)
        this.CancelVideo();
      else
        this.StopVideoRecording(true);
    }

    private void OnButtonHalfPress(object sender, EventArgs e)
    {
      if (this.videoCameraRecording)
        return;
      this.focusAtPoint(new System.Windows.Point(this.viewfinderCanvas.ActualWidth / 2.0, this.viewfinderCanvas.ActualHeight / 2.0));
    }

    private void OnButtonFullPress(object sender, EventArgs e)
    {
      if (this.videoCameraRecording)
        return;
      this.TakePhoto_Tap((object) null, (System.Windows.Input.GestureEventArgs) null);
    }

    private void TakeVideo_Hold(object sender, RoutedEventArgs e)
    {
      if (CameraPage.IsTakingProfilePicture || this.TakingPicture)
        return;
      this.CurrentCamera.Enabled = false;
      this.CaptureButtonBorder.Background = (Brush) this.HoldVideoBrush;
      this.CaptureButtonVideoRecordingBorder.Opacity = 0.3;
      this.viewfinderCanvas.Opacity = 0.0;
      this.SettingsBarBorder.Visibility = Visibility.Collapsed;
      TouchPanel.GetState();
      this.AnimateCollapseList();
      this.StartVideoRecording();
    }

    private void VideoTimer_Tick(object sender, object e)
    {
      TimeSpan timeSpan = DateTime.Now - this.recordingStartTime;
      this.TimerBlock.Text = string.Format("{0:00}:{1:00}", (object) timeSpan.Minutes, (object) timeSpan.Seconds);
      this.duration = (int) timeSpan.TotalSeconds;
      if (this.duration < 2 && TouchPanel.GetState().Count == 0)
        this.CancelVideo();
      if (timeSpan.TotalSeconds <= (double) this.videoMaxDuration)
        return;
      this.StopVideoRecording(true);
    }

    private void HoldTimer_Tick(object sender, object e)
    {
      if ((DateTime.Now - this.videoHoldStartTime).Seconds < 2)
        return;
      this.TakeVideo_Hold((object) null, (RoutedEventArgs) null);
      if (this.videoHoldTimer == null)
        return;
      this.videoHoldTimer.Stop();
      this.videoHoldTimer = (DispatcherTimer) null;
    }

    private void FlashTimer_Tick(object sender, object e)
    {
      ++this.flashTimerTicks;
      if (this.flashTimerTicks > 5)
        this.StopFlashTimer();
      if (this.CurrentCamera == null || this.CurrentCamera.cam == null || this.TakingPicture || this.videoCameraRecording)
        return;
      this.CurrentCamera.SetFlash(Settings.UserFlashSetting);
    }

    private async void TakePhoto_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.CurrentCamera == null || !this.CurrentCamera.Enabled || !this.CurrentCamera.IsInitialized)
        Log.l("cam page", "not ready to take photo");
      else if (this.TakingPicture || this.videoCameraRecording)
      {
        Log.l("cam page", "taking picture/video already in progress");
      }
      else
      {
        try
        {
          this.TakingPicture = true;
          this.StopFlashTimer();
          Log.l("cam page", "taking photo");
          this.AnimateTakePicture();
          await this.CurrentCamera.CaptureImage();
          Log.l("cam page", "photo capture finished");
          this.viewfinderCanvas.Fill = (Brush) UIUtils.BackgroundBrush;
          if (this.CurrentCamera.CameraType == CameraType.FrontFacing)
          {
            this.CurrentCamera.EncodedImageOrientation = 7;
            switch (this.currentOrientation)
            {
              case PageOrientation.LandscapeLeft:
                this.CurrentCamera.EncodedImageOrientation = 2;
                break;
              case PageOrientation.LandscapeRight:
                this.CurrentCamera.EncodedImageOrientation = 4;
                break;
            }
          }
          else
          {
            this.CurrentCamera.EncodedImageOrientation = 6;
            switch (this.currentOrientation)
            {
              case PageOrientation.LandscapeLeft:
                this.CurrentCamera.EncodedImageOrientation = 1;
                break;
              case PageOrientation.LandscapeRight:
                this.CurrentCamera.EncodedImageOrientation = 3;
                break;
            }
          }
          this.OnCaptureImageAvailable();
        }
        catch (Exception ex)
        {
          Log.l(ex, "Failed to take photo");
          this.TakingPicture = false;
        }
      }
    }

    private void CameraInitialized()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        Log.l("cam page", "initialized");
        if (this.CurrentCamera == null || !this.CurrentCamera.IsInitialized)
        {
          Log.l("cam page", "terminating initialization as it seems camera has been disposed: {0}", (object) (this.CurrentCamera == null));
        }
        else
        {
          Transform renderTransform = this.viewfinderCanvas.RenderTransform;
          if (Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.FrontFacing) && Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.Primary))
            this.CameraToggleButton.Visibility = Visibility.Visible;
          this.SetFlashIcon(Settings.UserFlashSetting);
          if (Settings.UserFlashSetting.HasValue)
          {
            this.CurrentCamera.SetFlash(Settings.UserFlashSetting);
            this.StopFlashTimer();
            this.StartFlashTimer();
          }
          if (PhotoCaptureDevice.IsFocusSupported(this.CurrentCamera.CameraType == CameraType.Primary ? (CameraSensorLocation) 0 : (CameraSensorLocation) 1))
            this.FocusAtPointCanvas.Visibility = Visibility.Visible;
          else
            this.FocusAtPointCanvas.Visibility = Visibility.Collapsed;
          Windows.Foundation.Size captureResolution = this.CurrentCamera.cam.CaptureResolution;
          if (captureResolution.Height / captureResolution.Width == 0.75)
          {
            this.viewfinderCanvas.Height = 640.0;
            this.viewfinderCanvas.Margin = new Thickness(0.0, 60.0, 0.0, 0.0);
          }
          else
          {
            this.viewfinderCanvas.Height = 854.0;
            this.viewfinderCanvas.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
          }
          this.StartAutoFocus();
        }
      }));
    }

    public void SetFlashIcon(FlashMode? mode)
    {
      this.FlashToggleButton.Visibility = Visibility.Visible;
      if (!mode.HasValue)
        return;
      FlashMode? nullable = mode;
      if (!nullable.HasValue)
        return;
      switch (nullable.GetValueOrDefault())
      {
        case FlashMode.On:
          this.FlashIcon.Source = (System.Windows.Media.ImageSource) AssetStore.FlashIcon;
          break;
        case FlashMode.Off:
          this.FlashIcon.Source = (System.Windows.Media.ImageSource) AssetStore.NoFlashIcon;
          break;
        case FlashMode.Auto:
          this.FlashIcon.Source = (System.Windows.Media.ImageSource) AssetStore.FlashAutoIcon;
          break;
      }
    }

    private void OnCaptureImageAvailable()
    {
      Log.l("cam page", "capture image available");
      try
      {
        TakePicture.CapturedPictureArgs capturedPictureArgs = TakePicture.ProcessChosenPhoto((Stream) this.CurrentCamera.captureStream, true, this.imageMaxWidth, this.imageMaxHeight, new int?(this.CurrentCamera.EncodedImageOrientation));
        capturedPictureArgs.ZoomScale = new System.Windows.Point(this.viewfinderTransform.ScaleX, this.viewfinderTransform.ScaleY);
        if (capturedPictureArgs == null || capturedPictureArgs.Bitmap == null || CameraPage.PictureDataObserver == null)
          return;
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.ContinuumBackwardOut), (DependencyObject) this.LayoutRoot);
        CameraPage.PictureDataObserver.OnNext(capturedPictureArgs);
      }
      finally
      {
        Log.l("cam page", "finished processing photo");
      }
    }

    private void AnimateCollapseList()
    {
      if (!this.cameraRollOpen)
        return;
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      double num = this.CameraRoll.ActualHeight + 140.0 - this.CameraRoll.ActualHeight;
      child1.From = new double?(this.CameraRollY.TranslateY);
      child1.To = new double?(num);
      DoubleAnimation child2 = resource.Children[3] as DoubleAnimation;
      child2.From = new double?(1.0);
      child2.To = new double?(0.0);
      Storyboarder.Perform(resource, false);
      this.cameraRollOpen = false;
    }

    private void AnimateExpandList()
    {
      if (this.keepCameraRollClosed || this.cameraRollOpen)
        return;
      Storyboard resource = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource.Children[0] as DoubleAnimation;
      double num = 0.0;
      child1.From = new double?(this.CameraRollY.TranslateY);
      child1.To = new double?(num);
      DoubleAnimation child2 = resource.Children[3] as DoubleAnimation;
      child2.From = new double?(0.0);
      child2.To = new double?(1.0);
      Storyboarder.Perform(resource, false);
      this.cameraRollOpen = true;
    }

    private void AnimateTakePicture(bool rewind = false)
    {
      if (!this.cameraRollOpen)
        this.PreviewItemsList.Visibility = Visibility.Collapsed;
      Storyboard resource1 = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child1 = resource1.Children[1] as DoubleAnimation;
      child1.From = new double?(this.CameraButtonTransformY.TranslateY);
      child1.To = new double?(rewind ? 0.0 : 200.0);
      Storyboarder.Perform(resource1, false);
      Storyboard resource2 = this.Resources[(object) "MoveList"] as Storyboard;
      DoubleAnimation child2 = resource2.Children[2] as DoubleAnimation;
      child2.From = new double?(this.SettingsBarX.TranslateX);
      child2.To = new double?(rewind ? 0.0 : 100.0);
      Storyboarder.Perform(resource2, false);
      this.AnimateCollapseList();
    }

    private void CameraToggle(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.CurrentCamera == null || !this.CurrentCamera.Enabled)
        return;
      if (this.CurrentCamera.CameraType == CameraType.Primary && Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.FrontFacing))
      {
        this.CurrentCamera = this.FrontFacingCamera;
        this.videoCaptureDevice = this.frontFacingVideoCamera;
        Settings.UserCameraTypeSetting = new CameraType?(CameraType.FrontFacing);
        this.FlashToggleButton.Opacity = 0.4;
      }
      else if (this.CurrentCamera.CameraType == CameraType.FrontFacing && Microsoft.Devices.Camera.IsCameraTypeSupported(CameraType.Primary))
      {
        this.CurrentCamera = this.PrimaryCamera;
        this.videoCaptureDevice = this.primaryVideoCamera;
        Settings.UserCameraTypeSetting = new CameraType?(CameraType.Primary);
        this.FlashToggleButton.Opacity = 1.0;
      }
      else
      {
        Log.l("cam page", "Tried to toggle camera when only one is supported.");
        this.CameraToggleButton.Visibility = Visibility.Collapsed;
      }
      this.InitializeVideoCamera(this.videoCaptureDevice);
      this.viewfinderTransform.ScaleX = 1.0;
      if (this.CurrentCamera.FlipImage)
        this.viewfinderTransform.ScaleY = -1.0;
      else
        this.viewfinderTransform.ScaleY = 1.0;
    }

    private void FlashToggle(object sender = null, RoutedEventArgs e = null)
    {
      if (this.CurrentCamera == null || this.CurrentCamera.CameraType == CameraType.FrontFacing || !this.CurrentCamera.Enabled || this.TakingPicture)
        return;
      this.CurrentCamera.ToggleFlash();
      this.SetFlashIcon(Settings.UserFlashSetting);
    }

    private void viewfinderCanvas_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.initialswipepoint = e.ManipulationOrigin;
    }

    private void CameraButtonBackground_ManipulationStarted(
      object sender,
      ManipulationStartedEventArgs e)
    {
      this.initialswipepoint = e.ManipulationOrigin;
    }

    private void CameraButtonBackground_ManipulationDelta(
      object sender,
      ManipulationDeltaEventArgs e)
    {
      System.Windows.Point translation = e.CumulativeManipulation.Translation;
      if (translation.Y - this.initialswipepoint.Y >= 80.0)
      {
        this.AnimateCollapseList();
        this.cameraRollOpen = false;
        e.Complete();
      }
      if (translation.Y - this.initialswipepoint.Y > -80.0)
        return;
      this.AnimateExpandList();
      this.cameraRollOpen = true;
      e.Complete();
    }

    private void viewfinderCanvas_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      System.Windows.Point translation = e.CumulativeManipulation.Translation;
      if (translation.Y - this.initialswipepoint.Y >= 80.0)
      {
        this.AnimateCollapseList();
        this.cameraRollOpen = false;
        e.Complete();
      }
      if (translation.Y - this.initialswipepoint.Y <= -80.0)
      {
        this.AnimateExpandList();
        this.cameraRollOpen = true;
        e.Complete();
      }
      CompositeTransform viewfinderTransform = this.viewfinderTransform;
      double num = (e.DeltaManipulation.Scale.Y + e.DeltaManipulation.Scale.X) / 2.0;
      if (viewfinderTransform == null || num == 0.0)
        return;
      viewfinderTransform.ScaleX *= num;
      viewfinderTransform.ScaleY *= num;
      if (viewfinderTransform.ScaleX > 2.0)
        viewfinderTransform.ScaleX = 2.0;
      if (viewfinderTransform.ScaleX < 1.0)
        viewfinderTransform.ScaleX = 1.0;
      double scaleY = viewfinderTransform.ScaleY;
      if (this.CurrentCamera.FlipImage)
      {
        if (viewfinderTransform.ScaleY < -2.0)
          viewfinderTransform.ScaleY = -2.0;
        if (viewfinderTransform.ScaleY <= -1.0)
          return;
        viewfinderTransform.ScaleY = -1.0;
      }
      else
      {
        if (viewfinderTransform.ScaleY > 2.0)
          viewfinderTransform.ScaleY = 2.0;
        if (viewfinderTransform.ScaleY >= 1.0)
          return;
        viewfinderTransform.ScaleY = 1.0;
      }
    }

    private void viewfinderCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.TakingPicture)
        return;
      this.focusAtPoint(e.GetPosition((UIElement) this.FocusAtPointCanvas));
    }

    private void focusAtPoint(System.Windows.Point p)
    {
      if (!this.FocusReady)
        return;
      double num = this.FocusOnPointEllipse.Width / 2.0;
      System.Windows.Point point1 = new System.Windows.Point(p.X, p.Y);
      System.Windows.Point point2 = new System.Windows.Point(p.X - num, p.Y - num);
      if (this.FocusOnPointEllipse.Visibility == Visibility.Collapsed)
        this.FocusOnPointEllipse.Visibility = Visibility.Visible;
      Canvas.SetLeft((UIElement) this.FocusOnPointEllipse, point2.X);
      Canvas.SetTop((UIElement) this.FocusOnPointEllipse, point2.Y);
      Duration duration = new Duration(TimeSpan.FromSeconds(1.0));
      DoubleAnimation element1 = new DoubleAnimation();
      DoubleAnimation element2 = new DoubleAnimation();
      DoubleAnimation element3 = new DoubleAnimation();
      DoubleAnimation element4 = new DoubleAnimation();
      element1.Duration = duration;
      element2.Duration = duration;
      element3.Duration = duration;
      element4.Duration = duration;
      Storyboard storyboard = new Storyboard();
      storyboard.Duration = duration;
      storyboard.Children.Add((Timeline) element1);
      storyboard.Children.Add((Timeline) element2);
      storyboard.Children.Add((Timeline) element3);
      storyboard.Children.Add((Timeline) element4);
      Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.FocusOnPointEllipse);
      Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.FocusOnPointEllipse);
      Storyboard.SetTarget((Timeline) element3, (DependencyObject) this.FocusOnPointEllipse);
      Storyboard.SetTarget((Timeline) element4, (DependencyObject) this.FocusOnPointEllipse);
      Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("Width", new object[0]));
      Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("Height", new object[0]));
      Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath("(Canvas.Left)", new object[0]));
      Storyboard.SetTargetProperty((Timeline) element4, new PropertyPath("(Canvas.Top)", new object[0]));
      element1.From = new double?(125.0);
      element2.From = new double?(125.0);
      element1.To = new double?(109.0);
      element2.To = new double?(109.0);
      element3.To = new double?(point2.X + 8.0);
      element4.To = new double?(point2.Y + 8.0);
      storyboard.Begin();
      if (this.t.IsEnabled)
      {
        this.ellipseTimer = 0;
        this.t.Stop();
        this.t.Start();
      }
      else
        this.t.Start();
      this.FocusTimer.Start();
      this.FocusReady = false;
      this.needFocus = true;
      this.CurrentFocusPoint = new System.Windows.Point?(point1);
      Log.l("cam page", "TAPPED focused {0}", (object) point1);
    }

    private void flashEllipse(object sender, EventArgs e)
    {
      if (this.ellipseTimer == 10)
      {
        this.FocusOnPointEllipse.Visibility = Visibility.Collapsed;
        this.ellipseTimer = 0;
        this.t.Stop();
      }
      else if (this.ellipseTimer % 2 == 0)
      {
        this.FocusOnPointEllipse.Opacity = 1.0;
        ++this.ellipseTimer;
      }
      else
      {
        this.FocusOnPointEllipse.Opacity = 0.2;
        ++this.ellipseTimer;
      }
    }

    public void StartAutoFocus()
    {
      this.tempwidth = this.Width;
      this.tempheight = this.Height;
      if (!Accelerometer.IsSupported || this.CurrentCamera == null)
        return;
      if (this.AutoFocusAccelerometer == null)
      {
        this.AutoFocusAccelerometer = new Accelerometer();
        this.AutoFocusAccelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(200.0);
        this.AutoFocusAccelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(this.AccelerometerStart);
      }
      this.AutoFocusAccelerometer.Start();
    }

    public async void AccelerometerStart(
      object sender,
      SensorReadingEventArgs<AccelerometerReading> e)
    {
      if (this.CurrentCamera == null || !this.CurrentCamera.Enabled || !this.CurrentCamera.IsInitialized)
        return;
      Vector3 previousAcceleration = this.PreviousAcceleration;
      AccelerometerReading sensorReading = e.SensorReading;
      Vector3 acceleration = sensorReading.Acceleration;
      double num1 = Math.Sqrt(Math.Pow((double) acceleration.X, 2.0) + Math.Pow((double) acceleration.Y, 2.0) + Math.Pow((double) acceleration.Z, 2.0));
      double num2 = Math.Sqrt(Math.Pow((double) this.PreviousAcceleration.X, 2.0) + Math.Pow((double) this.PreviousAcceleration.Y, 2.0) + Math.Pow((double) this.PreviousAcceleration.Z, 2.0)) - num1;
      if (Math.Abs(num2) < 0.06)
      {
        if (this.needFocus)
        {
          try
          {
            if (this.CurrentFocusPoint.HasValue)
            {
              if (this.CurrentFocusPoint.HasValue)
              {
                Log.p("cam page", "camera focused {0}", (object) num2);
                this.needFocus = false;
                await this.CurrentCamera.FocusAtPoint(this.CurrentFocusPoint.Value.X, this.CurrentFocusPoint.Value.Y);
                this.CurrentFocusPoint = new System.Windows.Point?();
              }
            }
          }
          catch (Exception ex)
          {
            Log.l("cam page", "focus error: {0}", (object) ex.ToString());
          }
          this.needFocus = false;
        }
      }
      else
        this.needFocus = true;
      sensorReading = e.SensorReading;
      this.PreviousAcceleration = sensorReading.Acceleration;
    }

    private void AlbumButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      CameraPage.PictureDataObserver.OnNext((TakePicture.CapturedPictureArgs) null);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CameraPage.xaml", UriKind.Relative));
      this.HoldVideoBrush = (SolidColorBrush) this.FindName("HoldVideoBrush");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Camera = (Grid) this.FindName("Camera");
      this.VideoRect = (System.Windows.Shapes.Rectangle) this.FindName("VideoRect");
      this.VideoRectCancelOverlay = (System.Windows.Shapes.Rectangle) this.FindName("VideoRectCancelOverlay");
      this.viewfinderCanvas = (System.Windows.Shapes.Rectangle) this.FindName("viewfinderCanvas");
      this.FocusAtPointCanvas = (Canvas) this.FindName("FocusAtPointCanvas");
      this.FocusOnPointEllipse = (Ellipse) this.FindName("FocusOnPointEllipse");
      this.SettingsBarBorder = (Border) this.FindName("SettingsBarBorder");
      this.SettingsBarX = (CompositeTransform) this.FindName("SettingsBarX");
      this.CameraToggleButton = (Button) this.FindName("CameraToggleButton");
      this.CameraSwapIcon = (Image) this.FindName("CameraSwapIcon");
      this.FlashToggleButton = (Button) this.FindName("FlashToggleButton");
      this.FlashIcon = (Image) this.FindName("FlashIcon");
      this.AlbumButton = (Button) this.FindName("AlbumButton");
      this.AlbumButtonImage = (Image) this.FindName("AlbumButtonImage");
      this.TipBlockBorder = (Border) this.FindName("TipBlockBorder");
      this.PhotoTipBlock = (TextBlock) this.FindName("PhotoTipBlock");
      this.RecordingIndicator = (Grid) this.FindName("RecordingIndicator");
      this.RecordingCircle = (Canvas) this.FindName("RecordingCircle");
      this.RecordingDot = (Ellipse) this.FindName("RecordingDot");
      this.TimerBlock = (TextBlock) this.FindName("TimerBlock");
      this.CameraRoll = (Grid) this.FindName("CameraRoll");
      this.CameraRollY = (CompositeTransform) this.FindName("CameraRollY");
      this.PreviewItemsList = (ListBox) this.FindName("PreviewItemsList");
      this.BottomPanel = (Grid) this.FindName("BottomPanel");
      this.CaptureButtonVideoRecordingCanvas = (Canvas) this.FindName("CaptureButtonVideoRecordingCanvas");
      this.CaptureButtonVideoRecordingBorder = (Ellipse) this.FindName("CaptureButtonVideoRecordingBorder");
      this.CaptureButtonBorder = (Border) this.FindName("CaptureButtonBorder");
      this.CameraButtonTransformY = (CompositeTransform) this.FindName("CameraButtonTransformY");
      this.CaptureButton = (Button) this.FindName("CaptureButton");
      this.CaptureButtonImage = (Image) this.FindName("CaptureButtonImage");
    }

    public class PreviewItem : PropChangedBase
    {
      private double thumbnailOpacity = 1.0;

      public System.Windows.Media.ImageSource PlayButtonIcon
      {
        get
        {
          return this.BindedMediaItem == null || this.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Video ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.WhitePlayButton;
        }
      }

      public Visibility PlayOverlayVisibility
      {
        get
        {
          return (this.BindedMediaItem != null && this.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Video).ToVisibility();
        }
      }

      public Visibility GifOverlayVisibility
      {
        get => (this.BindedMediaItem.GetMediaType() == FunXMPP.FMessage.Type.Gif).ToVisibility();
      }

      public System.Windows.Media.ImageSource GifIcon
      {
        get
        {
          return this.BindedMediaItem.GetMediaType() != FunXMPP.FMessage.Type.Gif ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) ImageStore.GifIcon;
        }
      }

      public MediaSharingState.IItem BindedMediaItem { get; private set; }

      public BitmapSource Thumbnail
      {
        get
        {
          return this.BindedMediaItem != null ? this.BindedMediaItem.GetThumbnail() : (BitmapSource) null;
        }
        set
        {
          this.BindedMediaItem.ThumbnailOverride = value;
          this.NotifyPropertyChanged(nameof (Thumbnail));
        }
      }

      public double ThumbnailOpacity
      {
        get => this.thumbnailOpacity;
        set
        {
          if (value == this.thumbnailOpacity)
            return;
          this.thumbnailOpacity = value;
          this.NotifyPropertyChanged(nameof (ThumbnailOpacity));
        }
      }

      public int ThumbRotationAngle => this.BindedMediaItem != null ? this.RotatedTimes * 90 : 0;

      public Thickness ItemMargin => new Thickness(10.0, 0.0, 0.0, 8.0);

      public int RotatedTimes => this.BindedMediaItem.RotatedTimes;

      public PreviewItem(MediaSharingState.IItem item) => this.BindedMediaItem = item;

      public void Rotate()
      {
        this.BindedMediaItem.RotatedTimes = (this.RotatedTimes + 1) % 4;
        this.NotifyPropertyChanged("ThumbRotationAngle");
      }
    }

    private enum VideoRecorderState
    {
      Busy = -1, // 0xFFFFFFFF
      Initialized = 0,
      Recording = 1,
      Ready = 2,
    }
  }
}
