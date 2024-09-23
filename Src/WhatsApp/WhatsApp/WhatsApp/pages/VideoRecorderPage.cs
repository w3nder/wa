// Decompiled with JetBrains decompiler
// Type: WhatsApp.VideoRecorderPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public class VideoRecorderPage : PhoneApplicationPage
  {
    public static int TempFilenameCounter;
    private IObserver<WaVideoArgs> videoObserver_;
    private CaptureSource captureSource_;
    private VideoCaptureDevice captureDevice_;
    private FileSink fileSink_;
    private WriteableBitmap thumbnail_;
    private IDisposable initialImageSubscription_;
    private IDisposable thumbnailSubscrition_;
    private Dictionary<VideoRecorderPage.VideoRecorderState, AppBarWrapper> appbars_ = new Dictionary<VideoRecorderPage.VideoRecorderState, AppBarWrapper>();
    private VideoRecorderPage.VideoRecorderState vrState_ = VideoRecorderPage.VideoRecorderState.Busy;
    private MediaElementWrapper videoPlayerWrapper_;
    private DispatcherTimer timer_;
    private DateTime recordingStartTime_;
    private int duration_;
    private Uri playButtonIconUri_;
    private Uri stopButtonIconUri_;
    private bool acceptClicked_;
    internal CompositeTransform Transform;
    internal Grid LayoutRoot;
    internal ProgressBar LoadingIndicator;
    internal Rectangle ViewRect;
    internal Image ThumbnailImage;
    internal MediaElement VideoPlayer;
    internal Grid Overlay;
    internal TextBlock TimerBlock;
    private bool _contentLoaded;

    private string TempFilename
    {
      get => "tmp/video" + (object) VideoRecorderPage.TempFilenameCounter + ".tmp.mp4";
    }

    private static IObserver<WaVideoArgs> VideoObserverParam { get; set; }

    private bool HasMultipleCameras => false;

    private VideoRecorderPage.VideoRecorderState VRState
    {
      get => this.vrState_;
      set
      {
        this.vrState_ = value;
        this.UpdateUI();
      }
    }

    private AppBarWrapper AppBarForCurrentState
    {
      get
      {
        VideoRecorderPage.VideoRecorderState videoRecorderState = this.VRState;
        if (videoRecorderState == VideoRecorderPage.VideoRecorderState.Playback)
          videoRecorderState = VideoRecorderPage.VideoRecorderState.Ready;
        AppBarWrapper barForCurrentState = (AppBarWrapper) null;
        if (!this.appbars_.TryGetValue(videoRecorderState, out barForCurrentState))
        {
          barForCurrentState = this.CreateAppBar(videoRecorderState);
          this.appbars_[videoRecorderState] = barForCurrentState;
        }
        return barForCurrentState;
      }
    }

    public Uri PlayButtonIconUri
    {
      get
      {
        Uri playButtonIconUri = this.playButtonIconUri_;
        return (object) playButtonIconUri != null ? playButtonIconUri : (this.playButtonIconUri_ = new Uri("/Images/play.png", UriKind.Relative));
      }
    }

    public Uri StopButtonIconUri
    {
      get
      {
        Uri stopButtonIconUri = this.stopButtonIconUri_;
        return (object) stopButtonIconUri != null ? stopButtonIconUri : (this.stopButtonIconUri_ = new Uri("/Images/stop.png", UriKind.Relative));
      }
    }

    public VideoRecorderPage()
    {
      this.InitializeComponent();
      this.videoObserver_ = VideoRecorderPage.VideoObserverParam;
      VideoRecorderPage.VideoObserverParam = (IObserver<WaVideoArgs>) null;
      this.videoPlayerWrapper_ = new MediaElementWrapper(this.VideoPlayer);
      this.videoPlayerWrapper_.MediaEnded += new EventHandler(this.VideoPlayer_MediaEnded);
    }

    public static IObservable<WaVideoArgs> Start(NavigationService navService)
    {
      return Observable.Create<WaVideoArgs>((Func<IObserver<WaVideoArgs>, Action>) (observer =>
      {
        VideoRecorderPage.VideoObserverParam = observer;
        NavUtils.NavigateToPage(navService, nameof (VideoRecorderPage));
        return (Action) (() => { });
      }));
    }

    private void RefreshAppBar()
    {
      AppBarWrapper barForCurrentState = this.AppBarForCurrentState;
      barForCurrentState?.UpdateAppBar();
      if (barForCurrentState != null && this.ApplicationBar == barForCurrentState.AppBar)
        return;
      if (this.ApplicationBar != null)
        this.ApplicationBar.IsVisible = false;
      if (barForCurrentState == null)
        return;
      this.ApplicationBar = barForCurrentState.AppBar;
      this.ApplicationBar.IsVisible = true;
    }

    private AppBarWrapper CreateAppBar(VideoRecorderPage.VideoRecorderState state)
    {
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      AppBarWrapper appBar = new AppBarWrapper((IApplicationBar) bar);
      int buttonIndex = 0;
      int num1;
      switch (state)
      {
        case VideoRecorderPage.VideoRecorderState.Initialized:
          if (!this.HasMultipleCameras)
            return (AppBarWrapper) null;
          ApplicationBarIconButton applicationBarIconButton1 = new ApplicationBarIconButton()
          {
            IconUri = new Uri("/Images/front-cam.png", UriKind.Relative),
            Text = "FrontCam"
          };
          applicationBarIconButton1.Click += new EventHandler(this.FrontCam_Click);
          bar.Buttons.Add((object) applicationBarIconButton1);
          appBar.AddButtonUpdateAction(buttonIndex, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) => { }));
          num1 = buttonIndex + 1;
          break;
        case VideoRecorderPage.VideoRecorderState.Recording:
          if (!this.HasMultipleCameras)
            return (AppBarWrapper) null;
          ApplicationBarIconButton applicationBarIconButton2 = new ApplicationBarIconButton()
          {
            IconUri = this.StopButtonIconUri,
            Text = "Stop"
          };
          applicationBarIconButton2.Click += new EventHandler(this.StopRecording_Click);
          bar.Buttons.Add((object) applicationBarIconButton2);
          num1 = buttonIndex + 1;
          break;
        case VideoRecorderPage.VideoRecorderState.Ready:
          ApplicationBarIconButton applicationBarIconButton3 = new ApplicationBarIconButton()
          {
            IconUri = this.PlayButtonIconUri,
            Text = "Play"
          };
          applicationBarIconButton3.Click += new EventHandler(this.Play_Click);
          bar.Buttons.Add((object) applicationBarIconButton3);
          appBar.AddButtonUpdateAction(buttonIndex, (Action<object, ApplicationBarIconButton>) ((argObj, argButton) => argButton.IconUri = this.VRState == VideoRecorderPage.VideoRecorderState.Playback ? this.StopButtonIconUri : this.PlayButtonIconUri));
          int num2 = buttonIndex + 1;
          ApplicationBarIconButton applicationBarIconButton4 = new ApplicationBarIconButton()
          {
            IconUri = new Uri("/Images/check.png", UriKind.Relative),
            Text = "UseMedia"
          };
          applicationBarIconButton4.Click += new EventHandler(this.UseMedia_Click);
          bar.Buttons.Add((object) applicationBarIconButton4);
          num1 = num2 + 1;
          ApplicationBarMenuItem applicationBarMenuItem = new ApplicationBarMenuItem()
          {
            Text = "Retake"
          };
          applicationBarMenuItem.Click += new EventHandler(this.Retake_Click);
          bar.MenuItems.Add((object) applicationBarMenuItem);
          break;
      }
      bar.ForegroundColor = Colors.White;
      bar.BackgroundColor = Colors.Black;
      bar.IsMenuEnabled = true;
      bar.IsVisible = false;
      Localizable.LocalizeAppBar(bar);
      return appBar;
    }

    private void UpdateUI()
    {
      this.RefreshAppBar();
      this.UpdateContentSizes();
      if (this.VRState == VideoRecorderPage.VideoRecorderState.Initialized)
        this.TimerBlock.Text = "00:00";
      this.UpdateContentVisibilities();
    }

    private void UpdateContentSizes()
    {
      if (this.captureDevice_ == null)
        return;
      double num1 = Math.Min(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
      double num2 = Math.Max(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
      AppBarWrapper barForCurrentState = this.AppBarForCurrentState;
      double num3 = num1;
      double num4 = barForCurrentState == null ? 0.0 : barForCurrentState.AppBar.DefaultSize;
      double num5 = num2 - num4;
      VideoFormat desiredFormat = this.captureDevice_.DesiredFormat;
      double num6 = desiredFormat == null ? 1.0 : (double) desiredFormat.PixelWidth / (double) desiredFormat.PixelHeight;
      double num7 = num3;
      double num8 = num7 * num6;
      if (num8 > num5)
      {
        num8 = num5;
        num7 = num8 / num6;
      }
      FrameworkElement[] frameworkElementArray = new FrameworkElement[3]
      {
        (FrameworkElement) this.ViewRect,
        (FrameworkElement) this.VideoPlayer,
        (FrameworkElement) this.ThumbnailImage
      };
      foreach (FrameworkElement frameworkElement in frameworkElementArray)
      {
        frameworkElement.Width = num8;
        frameworkElement.Height = num7;
      }
      this.Overlay.Width = num8;
    }

    private void UpdateContentVisibilities()
    {
      Set<UIElement> set = new Set<UIElement>();
      switch (this.VRState)
      {
        case VideoRecorderPage.VideoRecorderState.Busy:
          set.Add((UIElement) this.LoadingIndicator);
          break;
        case VideoRecorderPage.VideoRecorderState.Initialized:
          set.Add((UIElement) this.ViewRect);
          set.Add((UIElement) this.Overlay);
          break;
        case VideoRecorderPage.VideoRecorderState.Recording:
          set.Add((UIElement) this.ViewRect);
          set.Add((UIElement) this.Overlay);
          break;
        case VideoRecorderPage.VideoRecorderState.Ready:
          set.Add((UIElement) this.ThumbnailImage);
          set.Add((UIElement) this.Overlay);
          break;
        case VideoRecorderPage.VideoRecorderState.Playback:
          set.Add((UIElement) this.VideoPlayer);
          break;
      }
      UIElement[] uiElementArray = new UIElement[5]
      {
        (UIElement) this.LoadingIndicator,
        (UIElement) this.ThumbnailImage,
        (UIElement) this.ViewRect,
        (UIElement) this.VideoPlayer,
        (UIElement) this.Overlay
      };
      foreach (UIElement key in uiElementArray)
        key.Visibility = set.Contains(key) ? Visibility.Visible : Visibility.Collapsed;
      this.Overlay.Opacity = this.VRState == VideoRecorderPage.VideoRecorderState.Recording ? 0.65 : 1.0;
    }

    private void StartTimer()
    {
      if (this.timer_ == null)
      {
        this.timer_ = new DispatcherTimer();
        this.timer_.Tick += new EventHandler(this.Timer_Tick);
        this.timer_.Interval = TimeSpan.FromMilliseconds(500.0);
      }
      this.recordingStartTime_ = DateTime.Now;
      this.duration_ = 0;
      this.timer_.Start();
    }

    private void StopTimer()
    {
      if (this.timer_ == null)
        return;
      this.timer_.Stop();
    }

    private void DisposeImageSubscriptions()
    {
      this.thumbnailSubscrition_.SafeDispose();
      this.thumbnailSubscrition_ = (IDisposable) null;
      this.initialImageSubscription_.SafeDispose();
      this.initialImageSubscription_ = (IDisposable) null;
    }

    private void InitializeCamera(VideoCaptureDevice device = null, Action onComplete = null)
    {
      this.VRState = VideoRecorderPage.VideoRecorderState.Busy;
      this.DisposeImageSubscriptions();
      this.ThumbnailImage.Source = (System.Windows.Media.ImageSource) null;
      if (this.captureSource_ != null)
        this.captureSource_.Stop();
      this.WaitForUIUpdate().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        this.captureDevice_ = device ?? CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
        if (this.captureDevice_ == null)
        {
          int num = (int) MessageBox.Show(AppResources.VideoCaptureNoDevice);
          this.NavigationService.GoBack();
        }
        else
        {
          VideoFormat highFormat = this.captureDevice_.SupportedFormats.OrderByDescending<VideoFormat, int>((Func<VideoFormat, int>) (vf => vf.PixelWidth)).FirstOrDefault<VideoFormat>();
          if (highFormat != null)
            this.captureDevice_.DesiredFormat = this.captureDevice_.SupportedFormats.Where<VideoFormat>((Func<VideoFormat, bool>) (vf => vf.PixelFormat == highFormat.PixelFormat)).OrderByDescending<VideoFormat, int>((Func<VideoFormat, int>) (vf => vf.PixelWidth)).LastOrDefault<VideoFormat>() ?? highFormat;
          this.InitVideoCaptureSource();
          this.initialImageSubscription_ = this.captureSource_.GetImageCompletedAsync().Take<IEvent<CaptureImageCompletedEventArgs>>(1).Subscribe<IEvent<CaptureImageCompletedEventArgs>>((Action<IEvent<CaptureImageCompletedEventArgs>>) (args => this.VRState = VideoRecorderPage.VideoRecorderState.Initialized));
          if (this.captureDevice_ == null)
            return;
          this.captureSource_.Start();
          this.captureSource_.CaptureImageAsync();
        }
      }));
    }

    private void InitVideoCaptureSource()
    {
      this.captureSource_ = new CaptureSource();
      this.captureSource_.CaptureFailed += new EventHandler<ExceptionRoutedEventArgs>(this.CaptureSource_CaptureFailed);
      this.captureSource_.VideoCaptureDevice = this.captureDevice_;
      VideoBrush videoBrush = new VideoBrush();
      videoBrush.SetSource(this.captureSource_);
      this.ViewRect.Fill = (Brush) videoBrush;
    }

    private void CleanUp()
    {
      this.DisposeImageSubscriptions();
      if (this.captureSource_ != null)
      {
        this.captureSource_.Stop();
        this.captureSource_ = (CaptureSource) null;
      }
      this.videoPlayerWrapper_.Stop();
      if (this.timer_ != null)
      {
        this.timer_.Stop();
        this.timer_ = (DispatcherTimer) null;
      }
      CameraButtons.ShutterKeyPressed -= new EventHandler(this.CameraButtons_ShutterKeyPressed);
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
      UIUtils.EnableWakeLock(true);
      if (this.captureDevice_ == null || this.captureSource_?.VideoCaptureDevice == null)
        return;
      this.DisposeImageSubscriptions();
      if (this.captureSource_.State == CaptureState.Started)
        this.captureSource_.Stop();
      this.VRState = VideoRecorderPage.VideoRecorderState.Busy;
      this.WaitForUIUpdate().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        if (this.fileSink_ == null)
          this.fileSink_ = new FileSink();
        this.InitVideoCaptureSource();
        this.fileSink_.CaptureSource = this.captureSource_;
        this.fileSink_.IsolatedStorageFileName = this.TempFilename;
        this.IsEnabled = false;
        this.thumbnailSubscrition_ = this.captureSource_.GetImageCompletedAsync().Take<IEvent<CaptureImageCompletedEventArgs>>(1).ObserveOnDispatcher<IEvent<CaptureImageCompletedEventArgs>>().Subscribe<IEvent<CaptureImageCompletedEventArgs>>((Action<IEvent<CaptureImageCompletedEventArgs>>) (args =>
        {
          WriteableBitmap result = args.EventArgs.Result;
          if (result == null)
            return;
          this.StartTimer();
          this.IsEnabled = true;
          this.VRState = VideoRecorderPage.VideoRecorderState.Recording;
          int num = this.SupportedOrientations == SupportedPageOrientation.Portrait ? 8 : 1;
          this.ThumbnailImage.Source = (System.Windows.Media.ImageSource) (this.thumbnail_ = JpegUtils.ApplyJpegOrientation((BitmapSource) result, new int?(num)));
        }));
        this.captureSource_.Start();
        this.captureSource_.CaptureImageAsync();
      }));
    }

    private void StopVideoRecording(bool useMedia)
    {
      UIUtils.EnableWakeLock(false);
      this.StopTimer();
      if (this.captureSource_ == null || this.captureSource_.State != CaptureState.Started)
        return;
      this.captureSource_.Stop();
      this.VRState = VideoRecorderPage.VideoRecorderState.Ready;
      if (!useMedia)
        return;
      this.UseMedia();
    }

    private void ToggleVideoRecording()
    {
      if (this.VRState == VideoRecorderPage.VideoRecorderState.Initialized)
      {
        this.StartVideoRecording();
      }
      else
      {
        if (this.VRState != VideoRecorderPage.VideoRecorderState.Recording)
          return;
        this.StopVideoRecording(true);
      }
    }

    private void StartPlayback()
    {
      try
      {
        this.VRState = VideoRecorderPage.VideoRecorderState.Playback;
        this.videoPlayerWrapper_.Play(new Uri(this.TempFilename));
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "play just recorded video");
      }
    }

    private void StopPlayback()
    {
      this.videoPlayerWrapper_.Stop();
      this.VRState = VideoRecorderPage.VideoRecorderState.Ready;
    }

    private void UseMedia()
    {
      if (this.videoObserver_ == null)
        return;
      Stream d = (Stream) null;
      try
      {
        string tempFilename = this.TempFilename;
        string str;
        try
        {
          str = MediaDownload.SaveMedia(tempFilename, FunXMPP.FMessage.Type.Video);
          if (str != null)
          {
            using (IMediaStorage mediaStorage = MediaStorage.Create(tempFilename))
              d = mediaStorage.OpenFile(tempFilename);
            if (str.StartsWith("file:"))
              str = str.Substring(5);
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "save media");
          str = (string) null;
          d.SafeDispose();
          d = (Stream) null;
        }
        if (d == null)
        {
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
                  d = (Stream) tempFile;
                }
              }
              catch (Exception ex)
              {
                Log.LogException(ex, "copy local file to native stream");
                MemoryStream destination = new MemoryStream();
                storageFileStream.CopyTo((Stream) destination);
                d = (Stream) destination;
              }
              d.Position = 0L;
            }
          }
        }
        this.videoObserver_.OnNext(new WaVideoArgs()
        {
          PreviewPlayPath = this.TempFilename,
          Stream = d,
          Thumbnail = this.thumbnail_,
          LargeThumbnail = this.thumbnail_,
          Duration = this.duration_,
          FileExtension = "mp4",
          ContentType = "video/mp4",
          FullPath = str
        });
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "return captured video");
        d.SafeDispose();
        this.videoObserver_.OnError(ex);
      }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.videoObserver_ == null)
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      }
      else
      {
        ++VideoRecorderPage.TempFilenameCounter;
        this.InitializeCamera(CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice());
        CameraButtons.ShutterKeyPressed -= new EventHandler(this.CameraButtons_ShutterKeyPressed);
        CameraButtons.ShutterKeyPressed += new EventHandler(this.CameraButtons_ShutterKeyPressed);
      }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);
      this.StopVideoRecording(false);
      UIUtils.EnableWakeLock(false);
      this.CleanUp();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      UIUtils.EnableWakeLock(false);
      this.CleanUp();
      this.videoPlayerWrapper_.Detach();
      if (this.videoObserver_ != null)
        this.videoObserver_.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      if (e.Orientation == PageOrientation.LandscapeLeft)
      {
        this.UpdateUI();
        base.OnOrientationChanged(e);
      }
      else
      {
        int orientation = (int) e.Orientation;
      }
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

    private void FrontCam_Click(object sender, EventArgs e)
    {
    }

    private void UseMedia_Click(object sender, EventArgs e)
    {
      this.videoPlayerWrapper_.Stop();
      this.IsEnabled = false;
      if (this.videoObserver_ == null || this.acceptClicked_)
        return;
      this.acceptClicked_ = true;
      this.UseMedia();
      NavUtils.GoBack(this.NavigationService);
    }

    private void Play_Click(object sender, EventArgs e)
    {
      if (this.VRState == VideoRecorderPage.VideoRecorderState.Playback)
        this.StopPlayback();
      else
        this.StartPlayback();
    }

    private void StopRecording_Click(object sender, EventArgs e) => this.StopVideoRecording(true);

    private void Retake_Click(object sender, EventArgs e)
    {
      if (this.captureSource_ != null)
      {
        this.captureSource_.Stop();
        this.captureSource_ = (CaptureSource) null;
      }
      this.videoPlayerWrapper_.Stop();
      this.InitializeCamera(this.captureDevice_);
    }

    private void Screen_Tap(object sender, GestureEventArgs e) => this.ToggleVideoRecording();

    private void VideoPlayer_MediaEnded(object sender, EventArgs e)
    {
      this.VRState = VideoRecorderPage.VideoRecorderState.Ready;
    }

    private void Timer_Tick(object sender, object e)
    {
      TimeSpan timeSpan = DateTime.Now - this.recordingStartTime_;
      this.TimerBlock.Text = string.Format("{0:00}:{1:00}", (object) timeSpan.Minutes, (object) timeSpan.Seconds);
      this.duration_ = (int) timeSpan.TotalSeconds;
      if (timeSpan.TotalSeconds <= 60.0)
        return;
      this.StopVideoRecording(true);
    }

    private void CameraButtons_ShutterKeyPressed(object sender, EventArgs e)
    {
      this.ToggleVideoRecording();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/VideoRecorderPage.xaml", UriKind.Relative));
      this.Transform = (CompositeTransform) this.FindName("Transform");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.LoadingIndicator = (ProgressBar) this.FindName("LoadingIndicator");
      this.ViewRect = (Rectangle) this.FindName("ViewRect");
      this.ThumbnailImage = (Image) this.FindName("ThumbnailImage");
      this.VideoPlayer = (MediaElement) this.FindName("VideoPlayer");
      this.Overlay = (Grid) this.FindName("Overlay");
      this.TimerBlock = (TextBlock) this.FindName("TimerBlock");
    }

    private enum VideoRecorderState
    {
      Busy = -1, // 0xFFFFFFFF
      Initialized = 0,
      Recording = 1,
      Ready = 2,
      Playback = 3,
    }
  }
}
