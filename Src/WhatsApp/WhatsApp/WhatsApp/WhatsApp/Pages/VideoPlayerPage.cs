// Decompiled with JetBrains decompiler
// Type: WhatsApp.Pages.VideoPlayerPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.Events;
using WhatsApp.Streaming;
using WhatsApp.UtilsFrontend;


namespace WhatsApp.Pages
{
  public class VideoPlayerPage : 
    PhoneApplicationPage,
    IVideoStreamSourceBufferingCallbacks,
    IVideoStreamSourceErrorCallbacks,
    IVideoPlaybackDownloadCallbacks
  {
    private const string LogHeader = "VideoPlayerPage";
    private TimeSpan jumpBackInterval = TimeSpan.FromSeconds(10.0);
    private TimeSpan jumpForwardInterval = TimeSpan.FromSeconds(30.0);
    private TimeSpan sliderUpdateInterval = TimeSpan.FromMilliseconds(250.0);
    private TimeSpan downloadedVideoOpenWaitingErrorInterval = TimeSpan.FromSeconds(10.0);
    private const double controlBarBackgroundOpacity = 0.8;
    private const double bufferingOverlayBackgroundOpacity = 0.5;
    private const long timestampPerMillisecond = 10000;
    private const string rightTriangleIcon = "\uE102";
    private const string leftTriangleIcon = "◀";
    private const string pauseIcon = "\uE103";
    private const string seekLeftIcon = "\uE100";
    private const string seekRightIcon = "\uE101";
    private bool isLoaded;
    private bool isPlaying;
    private bool controlsVisible;
    private TimeSpan videoDuration = TimeSpan.Zero;
    private int rotationAngle;
    private bool isRTL = new CultureInfo(AppResources.CultureString).IsRightToLeft();
    private string playIcon = "\uE102";
    private DispatcherTimer sliderUpdateTimer = new DispatcherTimer();
    private VideoPlayback videoPlayback;
    private string videoFilePath;
    private VideoStreamSource videoStreamSource;
    private FunXMPP.FMessage.Key associatedMessageKey;
    private bool isPageNavigatedTo;
    private bool isPageRemoved;
    private System.Threading.Timer downloadedVideoOpenWaitingErrorTimer;
    private VideoPlay videoPlayEvent;
    private TimeSpan totalPlayTime = TimeSpan.Zero;
    private TimeSpan totalBufferingTime = TimeSpan.Zero;
    private TimeSpan initialBufferingTime = TimeSpan.Zero;
    private DateTime playerLaunchTime;
    private DateTime? lastPlayStartTime;
    private DateTime? lastBufferStartTime;
    private int numberOfTimesBuffered;
    private static VideoPlayback VideoPlayback;
    private static VideoPlay VideoPlayEvent;
    private static string VideoFilePath;
    private static VideoStreamSource VideoStreamSource;
    private static FunXMPP.FMessage.Key AssociatedMessageKey;
    private IDisposable updateMessageWaTypeSub;
    internal Grid LayoutRoot;
    internal Canvas videoCanvas;
    internal Grid bufferingOverlay;
    internal SolidColorBrush bufferingOverlayBackground;
    internal ProgressBar bufferingBar;
    internal Grid controlBar;
    internal SolidColorBrush controlBarBackground;
    internal Slider progressSlider;
    internal ProgressBar bufferingProgressBar;
    internal Button jumpBackButton;
    internal Button playButton;
    internal Button jumpForwardButton;
    internal Border timestampsRow;
    internal TextBlock timeWatchedText;
    internal TextBlock timeLeftText;
    internal Image thumbnailImage;
    internal MediaElement mediaPlayer;
    private bool _contentLoaded;

    public VideoPlayerPage()
    {
      this.InitializeComponent();
      this.SetRightToLeftIcons();
      this.StartListeningForControlsEvents();
      this.ShouldShowControls(false);
      if (!this.SaveStaticVariables() || !this.SetVideoSource())
        Log.d(nameof (VideoPlayerPage), "Could not construct VideoPlayerPage as static variables could not be saved or video source could not be set");
      else
        this.SetThumbnail();
    }

    private void SetRightToLeftIcons()
    {
      if (!this.isRTL)
        return;
      this.playIcon = "◀";
      if (!this.isPlaying)
        this.playButton.Content = (object) this.playIcon;
      this.jumpBackButton.Content = (object) "\uE101";
      this.jumpForwardButton.Content = (object) "\uE100";
    }

    private bool SaveStaticVariables()
    {
      if (!Assert.IsTrue(VideoPlayerPage.VideoPlayback != null, "The VideoPlayback wrapper must have opened the page"))
        return false;
      this.videoPlayback = VideoPlayerPage.VideoPlayback;
      this.videoPlayback.downloadCallbacks = (IVideoPlaybackDownloadCallbacks) this;
      if (!Assert.IsTrue(VideoPlayerPage.VideoPlayEvent != null, "The video player event must be set when opening the video"))
        return false;
      this.videoPlayEvent = VideoPlayerPage.VideoPlayEvent;
      if (!Assert.IsTrue(VideoPlayerPage.VideoFilePath != null || VideoPlayerPage.VideoStreamSource != null, "There must be a way to open the video"))
        return false;
      this.videoFilePath = VideoPlayerPage.VideoFilePath;
      this.videoStreamSource = VideoPlayerPage.VideoStreamSource;
      this.associatedMessageKey = VideoPlayerPage.AssociatedMessageKey;
      VideoPlayerPage.VideoPlayback = (VideoPlayback) null;
      VideoPlayerPage.VideoPlayEvent = (VideoPlay) null;
      VideoPlayerPage.VideoFilePath = (string) null;
      VideoPlayerPage.VideoStreamSource = (VideoStreamSource) null;
      VideoPlayerPage.AssociatedMessageKey = (FunXMPP.FMessage.Key) null;
      return true;
    }

    private bool SetVideoSource()
    {
      if (this.videoStreamSource != null)
      {
        this.mediaPlayer.SetSource((MediaStreamSource) this.videoStreamSource);
        this.videoStreamSource.BufferingCallbacks = (IVideoStreamSourceBufferingCallbacks) this;
        this.videoStreamSource.ErrorCallbacks = (IVideoStreamSourceErrorCallbacks) this;
        this.videoPlayEvent.videoPlayType = new wam_enum_video_play_type?(wam_enum_video_play_type.STREAM);
        Log.l(nameof (VideoPlayerPage), "Successfully set the video source (VideoStreamSource)");
      }
      else
      {
        if (this.videoFilePath == null)
          return Assert.Failed("Must set either the VideoStreamSource or the VideoFilePath");
        this.mediaPlayer.Source = new Uri(MediaStorage.GetAbsolutePath(this.videoFilePath));
        this.videoPlayEvent.videoPlayType = new wam_enum_video_play_type?(wam_enum_video_play_type.FILE);
        Log.l(nameof (VideoPlayerPage), string.Format("Successfully set the video source (file: {0})", (object) this.mediaPlayer.Source));
        Assert.IsTrue(this.downloadedVideoOpenWaitingErrorTimer == null, "Should not be starting the downloaded video open waiting error timer twice");
        this.downloadedVideoOpenWaitingErrorTimer = new System.Threading.Timer(new TimerCallback(this.StartDownloadedVideoOpenWaitingTimer), (object) this, (int) this.downloadedVideoOpenWaitingErrorInterval.TotalMilliseconds, -1);
      }
      return true;
    }

    private void SetThumbnail()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        BitmapSource blurredThumbnail = this.videoPlayback?.GetBlurredThumbnail();
        if (blurredThumbnail == null)
          return;
        this.thumbnailImage.Source = (System.Windows.Media.ImageSource) blurredThumbnail;
        Log.d(nameof (VideoPlayerPage), "Set the thumbnail");
      }));
    }

    private void StartListeningForControlsEvents()
    {
      this.controlBar.Loaded += new RoutedEventHandler(this.ControlBar_Loaded);
      this.mediaPlayer.MediaOpened += new RoutedEventHandler(this.MediaPlayer_MediaOpened);
      this.mediaPlayer.MediaEnded += new RoutedEventHandler(this.MediaPlayer_MediaEnded);
      this.mediaPlayer.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(this.MediaPlayer_MediaFailed);
      this.playButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.PlayButton_Tap);
      this.jumpBackButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.JumpBackButton_Tap);
      this.jumpForwardButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.JumpForwardButton_Tap);
      this.progressSlider.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.ProgressSlider_ManipulationCompleted);
      this.progressSlider.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.ProgressSlider_ManipulationStarted);
      this.progressSlider.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.ProgressSlider_ManipulationDelta);
      this.videoCanvas.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.VideoCanvas_Tap);
      if (this.updateMessageWaTypeSub != null)
        return;
      this.updateMessageWaTypeSub = MessagesContext.Events.UpdatedMessagesMediaWaTypeSubject.ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnMessageWaMediaTypeUpdate));
    }

    private void OnMessageWaMediaTypeUpdate(Message m)
    {
      if (m == null || !m.IsRevoked() || this.associatedMessageKey == null || !(this.associatedMessageKey.id == m.KeyId) || !(this.associatedMessageKey.remote_jid == m.KeyRemoteJid) || this.associatedMessageKey.from_me != m.KeyFromMe)
        return;
      Log.l(nameof (VideoPlayerPage), "current video revoked {0}", (object) this.associatedMessageKey.id);
      this.Pause();
      this.GoBack();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      bool flag = false;
      if (this.videoPlayback == null || this.videoFilePath == null && this.videoStreamSource == null)
      {
        Log.l(nameof (VideoPlayerPage), "Navigating back from VideoPlayerPage since the video was not playable");
        flag = true;
      }
      if (this.isPageNavigatedTo)
        flag = true;
      else
        this.isPageNavigatedTo = true;
      if (flag)
        this.GoBack();
      else
        this.playerLaunchTime = DateTime.Now;
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);
      this.Pause();
      this.mediaPlayer.Source = (Uri) null;
      if (this.lastBufferStartTime.HasValue)
      {
        this.totalBufferingTime += DateTime.Now.Subtract(this.lastBufferStartTime.Value);
        this.lastBufferStartTime = new DateTime?();
      }
      this.StopTimer();
      this.ShouldShowControls(false);
      this.ShouldShowBufferingOverlay(false);
      this.thumbnailImage.Visibility = Visibility.Collapsed;
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      Log.d(nameof (VideoPlayerPage), "Removed from journal");
      this.updateMessageWaTypeSub.SafeDispose();
      this.videoPlayback.SafeDispose();
      this.videoDuration = TimeSpan.Zero;
      this.sliderUpdateTimer = (DispatcherTimer) null;
      this.videoPlayback = (VideoPlayback) null;
      this.videoStreamSource = (VideoStreamSource) null;
      this.SendVideoPlayEvent();
      this.LayoutRoot.Children.Remove((UIElement) this.mediaPlayer);
      AudioPlaybackManager.BackgroundMedia.Resume();
      this.isPageRemoved = true;
    }

    public static void Start(
      VideoPlayback videoPlayback,
      VideoPlay videoPlayEvent,
      string videoFilePath,
      FunXMPP.FMessage.Key associatedMessageKey)
    {
      if (!Assert.IsTrue(videoPlayback != null, "Cannot open a video without the VideoPlayback wrapper") || !Assert.IsTrue(videoPlayEvent != null, "Cannot start a video without an initialized VideoPlay event") || !Assert.IsTrue(videoFilePath != null, "Cannot open a video file without setting the file path"))
        return;
      VideoPlayerPage.VideoPlayback = videoPlayback;
      VideoPlayerPage.VideoPlayEvent = videoPlayEvent;
      VideoPlayerPage.VideoFilePath = videoFilePath;
      VideoPlayerPage.AssociatedMessageKey = associatedMessageKey;
      NavUtils.NavigateToPage(nameof (VideoPlayerPage));
    }

    public static void Start(
      VideoPlayback videoPlayback,
      VideoPlay videoPlayEvent,
      VideoStreamSource videoStreamSource,
      FunXMPP.FMessage.Key associatedMessageKey)
    {
      if (!Assert.IsTrue(videoPlayback != null, "Cannot open a video without the VideoPlayback wrapper") || !Assert.IsTrue(videoPlayEvent != null, "Cannot start a video without an initialized VideoPlay event") || !Assert.IsTrue(videoStreamSource != null, "Cannot stream from a video without a VideoStreamSource reference"))
        return;
      VideoPlayerPage.VideoPlayback = videoPlayback;
      VideoPlayerPage.VideoPlayEvent = videoPlayEvent;
      VideoPlayerPage.VideoStreamSource = videoStreamSource;
      VideoPlayerPage.AssociatedMessageKey = associatedMessageKey;
      NavUtils.NavigateToPage(nameof (VideoPlayerPage));
    }

    private void GoBack()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
          return;
        NavUtils.GoBack(this.NavigationService);
      }));
    }

    private void SendVideoPlayEvent()
    {
      if (!Assert.IsTrue(this.videoPlayEvent != null, "There should be a video play event when video is finished playing"))
        return;
      this.videoPlayEvent.videoPlayResult = new wam_enum_video_play_result?(wam_enum_video_play_result.OK);
      this.videoPlayEvent.videoPlayT = new long?((long) (int) this.totalPlayTime.TotalSeconds);
      this.videoPlayEvent.videoInitialBufferingT = new long?((long) (int) this.initialBufferingTime.TotalMilliseconds);
      Log.l(nameof (VideoPlayerPage), string.Format("Stats: [play time: {0}s] [initial buffer time: {1}s] [total buffer time: {2}s ({3} times)]", (object) this.totalPlayTime.TotalSeconds, (object) this.initialBufferingTime.TotalSeconds, (object) this.totalBufferingTime.TotalSeconds, (object) this.numberOfTimesBuffered));
      this.videoPlayEvent.SaveEvent();
    }

    private void StartDownloadedVideoOpenWaitingTimer(object state)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (!Assert.IsTrue(this.videoFilePath != null, "The downloaded video opening timer should not have been triggered if we are streaming the video") || !Assert.IsTrue(this.videoStreamSource == null, "The downloaded video opening timer should not have been triggered if we are streaming the video") || !Assert.IsTrue(this.downloadedVideoOpenWaitingErrorTimer != null, "The downloaded video opening timer should not have gone off if the timer did not exist"))
          return;
        this.downloadedVideoOpenWaitingErrorTimer.SafeDispose();
        this.downloadedVideoOpenWaitingErrorTimer = (System.Threading.Timer) null;
        if (this.isLoaded || this.isPageRemoved)
          return;
        int num = (int) MessageBox.Show(AppResources.VideoErrorPleaseReboot);
        this.GoBack();
      }));
    }

    private void ControlBar_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring control bar loading event since the page was disposed");
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), "Video playback controls loaded");
        this.RelocateControlBar();
      }
    }

    private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
    {
      Log.l(nameof (VideoPlayerPage), "Video successfully opened");
      this.isLoaded = true;
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring media opened event since the page was disposed");
      }
      else
      {
        this.downloadedVideoOpenWaitingErrorTimer.SafeDispose();
        this.downloadedVideoOpenWaitingErrorTimer = (System.Threading.Timer) null;
        this.RotateVideo(this.videoPlayback.GetRotationAngle());
        if (this.videoStreamSource != null)
          this.progressSlider.IsEnabled = this.mediaPlayer.CanSeek;
        else if (this.videoFilePath != null)
          this.progressSlider.IsEnabled = true;
        this.videoDuration = this.mediaPlayer.NaturalDuration.TimeSpan;
        this.progressSlider.Maximum = this.videoDuration.TotalMilliseconds;
        this.bufferingProgressBar.Maximum = this.videoDuration.TotalMilliseconds;
        this.StartTimer();
        TimeSpan timeSpan = DateTime.Now.Subtract(this.playerLaunchTime);
        this.initialBufferingTime += timeSpan;
        this.totalBufferingTime += timeSpan;
        if (this.videoFilePath != null)
        {
          this.ShouldShowBufferingOverlay(false);
          this.Play();
          this.thumbnailImage.Visibility = Visibility.Collapsed;
        }
        this.bufferingBar.IsIndeterminate = false;
      }
    }

    private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring media ended event as the page was disposed");
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), "Video playback finished");
        this.GoBack();
      }
    }

    private void MediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
    {
      Log.l(nameof (VideoPlayerPage), "Media failed to open");
      Log.LogException(e.ErrorException, "Opening video file");
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring media failed event since the page was disposed");
      }
      else
      {
        this.ShouldShowBufferingOverlay(false);
        this.ShouldShowControls(false);
        this.thumbnailImage.Visibility = Visibility.Collapsed;
        if (this.videoStreamSource != null)
        {
          Log.l(nameof (VideoPlayerPage), "Media Failed Exception detected: {0}", (object) e.ErrorException.GetFriendlyMessage());
          int num = (int) MessageBox.Show(AppResources.VideoNetworkError);
        }
        else
        {
          Assert.IsTrue(this.videoFilePath != null, "Must have video file path if not streaming");
          int num = (int) MessageBox.Show(AppResources.VideoErrorPleaseReboot);
        }
        this.GoBack();
      }
    }

    private void Media_State_Changed(object sender, RoutedEventArgs e)
    {
      Log.d(nameof (VideoPlayerPage), "State Change {0}", (object) this.mediaPlayer.CurrentState);
    }

    private void SliderTimer_Tick(object sender, EventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring slider timer tick event since the page was disposed");
      }
      else
      {
        if (this.progressSlider.Maximum <= 0.0)
          return;
        this.UpdateSliderPosition();
        this.UpdateBufferingProgress();
        this.UpdateTimestamps();
      }
    }

    private void PlayButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.isPageRemoved)
        Log.d(nameof (VideoPlayerPage), "Ignoring play button tap event since the page was disposed");
      else if (this.isPlaying)
        this.Pause();
      else
        this.Play();
    }

    private void JumpBackButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring jump back button event since the page was disposed");
      }
      else
      {
        this.JumpBackward();
        this.UpdateTimestamps();
        this.UpdateSliderPosition();
      }
    }

    private void JumpForwardButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring jump forward button event since the page was disposed");
      }
      else
      {
        this.JumpForward();
        this.UpdateTimestamps();
        this.UpdateSliderPosition();
      }
    }

    private void ProgressSlider_ManipulationCompleted(
      object sender,
      ManipulationCompletedEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring progress slider manipulation completed event since the page was disposed");
      }
      else
      {
        this.mediaPlayer.Position = TimeSpan.FromMilliseconds(this.progressSlider.Value);
        Log.l(nameof (VideoPlayerPage), string.Format("Stopped scrubbing the video - now at position {0}", (object) this.mediaPlayer.Position.ToString()));
        this.Play();
      }
    }

    private void ProgressSlider_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring progress slider manipulation started event since the page was disposed");
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), string.Format("Started scrubbing the video from position {0}", (object) this.mediaPlayer.Position.ToString()));
        this.Pause();
      }
    }

    private void ProgressSlider_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring progress slider manipulation delta event since the page was disposed");
      }
      else
      {
        this.mediaPlayer.Position = TimeSpan.FromMilliseconds(this.progressSlider.Value);
        this.UpdateTimestamps();
      }
    }

    private void VideoCanvas_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring video canvas tap event since the page was disposed");
      }
      else
      {
        if (!this.isLoaded)
          return;
        if (!this.controlsVisible)
        {
          this.ShouldShowControls(true);
        }
        else
        {
          if (e.GetPosition((UIElement) this.videoCanvas).Y >= this.videoCanvas.ActualHeight - this.controlBar.ActualHeight)
            return;
          this.ShouldShowControls(false);
        }
      }
    }

    private void SizeChangedEvent(object sender, SizeChangedEventArgs e)
    {
      if (this.isPageRemoved)
      {
        Log.d(nameof (VideoPlayerPage), "Ignoring size changed event as the page was disposed");
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), "Size of the page changed - relocating and resizing views");
        this.ChangeOrientation(e.NewSize.Width, e.NewSize.Height);
      }
    }

    public void StartedBuffering()
    {
      Log.d(nameof (VideoPlayerPage), "Started buffering");
      this.StartBuffering();
    }

    public void BufferingProgressChanged(double progress) => this.ChangeBufferingProgress(progress);

    public void FinishedBuffering()
    {
      Log.d(nameof (VideoPlayerPage), "Finished buffering");
      this.FinishBuffering();
    }

    private void StartBuffering()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
        {
          Log.d(nameof (VideoPlayerPage), "Ignoring buffering start event as the page was disposed");
        }
        else
        {
          this.bufferingProgressBar.Value = 1.0;
          ++this.numberOfTimesBuffered;
          this.lastBufferStartTime = new DateTime?(DateTime.Now);
          this.ShouldShowBufferingOverlay(true);
          this.Pause();
        }
      }));
    }

    private void ChangeBufferingProgress(double progress)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
          Log.d(nameof (VideoPlayerPage), "Ignoring buffering progress event as the page was disposed");
        else
          this.bufferingBar.Value = progress * 100.0;
      }));
    }

    private void FinishBuffering()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
        {
          Log.d(nameof (VideoPlayerPage), "Ignoring buffering finished event as the page was disposed");
        }
        else
        {
          if (Assert.IsTrue(this.lastBufferStartTime.HasValue, "Cannot finish buffering before we start"))
          {
            TimeSpan timeSpan = DateTime.Now.Subtract(this.lastBufferStartTime.Value);
            this.totalBufferingTime += timeSpan;
            if (this.numberOfTimesBuffered == 1)
              this.initialBufferingTime += timeSpan;
            this.lastBufferStartTime = new DateTime?();
          }
          this.ShouldShowBufferingOverlay(false);
          this.Play();
        }
      }));
    }

    public void CouldNotStream()
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        Log.d(nameof (VideoPlayerPage), "VideoStreamSource could not stream video");
        if (this.isPageRemoved)
        {
          Log.d(nameof (VideoPlayerPage), "Ignoring could not stream event as the page was disposed");
        }
        else
        {
          this.videoStreamSource = (VideoStreamSource) null;
          this.bufferingBar.IsIndeterminate = false;
          this.bufferingProgressBar.Maximum = 100.0;
          this.bufferingProgressBar.Value = 1.0;
          this.ShouldShowBufferingOverlay(true);
          this.videoPlayback.SwitchToDownload(this.Dispatcher);
        }
      }));
    }

    public void UpdateVideoDownloadProgress(double progress)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
          Log.d(nameof (VideoPlayerPage), "Ignoring download progress event as the page was disposed");
        else
          this.bufferingProgressBar.Value = progress * 100.0;
      }));
    }

    public void VideoDownloadComplete(string videoFilePath)
    {
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.isPageRemoved)
        {
          Log.d(nameof (VideoPlayerPage), "Ignoring download complete event as the page was disposed");
        }
        else
        {
          Log.l(nameof (VideoPlayerPage), string.Format("Video finished downloading - will play from {0}", (object) videoFilePath));
          this.videoStreamSource = (VideoStreamSource) null;
          this.videoFilePath = videoFilePath;
          this.SetVideoSource();
          this.ShouldShowBufferingOverlay(false);
        }
      }));
    }

    private void ShouldShowBufferingOverlay(bool showOverlay)
    {
      this.bufferingOverlay.Visibility = showOverlay ? Visibility.Visible : Visibility.Collapsed;
      this.bufferingOverlayBackground.Opacity = showOverlay ? 0.5 : 0.0;
    }

    private void ShouldShowControls(bool showController)
    {
      this.controlsVisible = showController;
      Visibility visibility = showController ? Visibility.Visible : Visibility.Collapsed;
      this.controlBarBackground.Opacity = showController ? 0.8 : 0.0;
      this.jumpBackButton.Visibility = visibility;
      this.playButton.Visibility = visibility;
      this.jumpForwardButton.Visibility = visibility;
      this.timestampsRow.Visibility = visibility;
      this.progressSlider.Visibility = visibility;
      this.bufferingProgressBar.Visibility = visibility;
    }

    private void RelocateControlBar()
    {
      double length = this.videoCanvas.ActualHeight - this.controlBar.ActualHeight;
      Canvas.SetTop((UIElement) this.controlBar, length);
      Canvas.SetTop((UIElement) this.timestampsRow, length + this.controlBar.ActualHeight / 2.0 - this.timestampsRow.Height);
    }

    private void ChangeOrientation(double newWidth, double newHeight)
    {
      this.RotateVideo(this.rotationAngle);
      this.controlBar.Width = newWidth;
      this.timestampsRow.Width = newWidth;
      this.RelocateControlBar();
      this.bufferingOverlay.Width = newWidth;
      this.bufferingOverlay.Height = newHeight;
      if (this.thumbnailImage.Source == null || this.thumbnailImage.Visibility != Visibility.Visible)
        return;
      this.thumbnailImage.Width = newWidth;
      this.thumbnailImage.Height = newHeight;
    }

    private void UpdateSliderPosition()
    {
      if (this.videoStreamSource != null)
        this.progressSlider.Value = (double) (this.videoStreamSource.GetVideoPosition() / 10000L);
      else
        this.progressSlider.Value = this.mediaPlayer.Position.TotalMilliseconds;
    }

    private void UpdateBufferingProgress()
    {
      if (this.videoStreamSource == null)
        return;
      this.bufferingProgressBar.Value = (double) (this.videoStreamSource.GetTotalBufferedTime() / 10000L);
    }

    private void UpdateTimestamps()
    {
      TimeSpan timeSpan = this.videoStreamSource == null ? this.mediaPlayer.Position : TimeSpan.FromMilliseconds((double) (this.videoStreamSource.GetVideoPosition() / 10000L));
      this.timeWatchedText.Text = timeSpan.ToFriendlyString();
      this.timeLeftText.Text = this.videoDuration.Subtract(timeSpan).ToFriendlyString();
    }

    private void StartTimer()
    {
      this.sliderUpdateTimer.Interval = this.sliderUpdateInterval;
      this.sliderUpdateTimer.Tick += new EventHandler(this.SliderTimer_Tick);
      this.sliderUpdateTimer.Start();
      Log.l(nameof (VideoPlayerPage), string.Format("Started the video timer, will update every {0} milliseconds", (object) this.sliderUpdateTimer.Interval.TotalMilliseconds.ToString()));
    }

    private void StopTimer()
    {
      this.sliderUpdateTimer.Stop();
      Log.l(nameof (VideoPlayerPage), "Stopping the video timer");
    }

    private void Play()
    {
      if (this.isPlaying)
      {
        Log.d(nameof (VideoPlayerPage), "Video playing - ignore play");
      }
      else
      {
        this.isPlaying = true;
        this.mediaPlayer.Play();
        this.playButton.Content = (object) "\uE103";
        this.lastPlayStartTime = new DateTime?(DateTime.Now);
        if (this.videoStreamSource != null)
        {
          this.thumbnailImage.Visibility = Visibility.Collapsed;
          this.videoStreamSource.ExitBufferingState();
        }
        Log.l(nameof (VideoPlayerPage), "Started playing the video");
      }
    }

    private void Pause()
    {
      if (!this.isPlaying)
      {
        Log.d(nameof (VideoPlayerPage), "Video not playing - ignore pause");
      }
      else
      {
        this.isPlaying = false;
        this.mediaPlayer.Pause();
        this.playButton.Content = (object) this.playIcon;
        if (this.lastPlayStartTime.HasValue)
        {
          this.totalPlayTime += DateTime.Now.Subtract(this.lastPlayStartTime.Value);
          this.lastPlayStartTime = new DateTime?();
        }
        Log.l(nameof (VideoPlayerPage), "Video paused");
      }
    }

    private void JumpForward()
    {
      if (!Assert.IsTrue(this.mediaPlayer != null, "Must have a MediaElement reference before jumping forward"))
        return;
      if (TimeSpan.MaxValue - this.jumpForwardInterval < this.mediaPlayer.Position)
      {
        Log.l(nameof (VideoPlayerPage), "Jumping to the end of the video");
        this.mediaPlayer.Position = TimeSpan.MaxValue;
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), string.Format("Jumping forward by {0} seconds", (object) this.jumpForwardInterval.TotalSeconds.ToString()));
        this.mediaPlayer.Position += this.jumpForwardInterval;
      }
    }

    private void JumpBackward()
    {
      if (!Assert.IsTrue(this.mediaPlayer != null, "Must have a MediaElement reference before jumping backwards"))
        return;
      if (this.mediaPlayer.Position < this.jumpBackInterval)
      {
        Log.l(nameof (VideoPlayerPage), "Jumping to the beginning of the video");
        this.mediaPlayer.Position = TimeSpan.Zero;
      }
      else
      {
        Log.l(nameof (VideoPlayerPage), string.Format("Jumping back by {0} seconds", (object) this.jumpBackInterval.TotalSeconds.ToString()));
        this.mediaPlayer.Position -= this.jumpBackInterval;
      }
    }

    private void RotateVideo(int angle)
    {
      if (!Assert.IsTrue(angle >= 0 && angle < 360, "The angle must be in [0, 360) degres") || !Assert.IsTrue(this.videoCanvas != null, "Cannot rotate a video on a null canvas") || !Assert.IsTrue(this.mediaPlayer != null, "Cannot rotate a video without MediaElement"))
        return;
      this.rotationAngle = angle;
      double actualHeight = this.videoCanvas.ActualHeight;
      double actualWidth = this.videoCanvas.ActualWidth;
      this.mediaPlayer.RenderTransform = (Transform) new CompositeTransform()
      {
        Rotation = 0.0,
        CenterX = 0.0,
        CenterY = 0.0
      };
      CompositeTransform renderTransform = (CompositeTransform) this.mediaPlayer.RenderTransform;
      renderTransform.Rotation = (double) -this.rotationAngle;
      Log.l(nameof (VideoPlayerPage), string.Format("Rotating back the video by {0} degrees", (object) this.rotationAngle));
      switch (this.rotationAngle)
      {
        case 90:
          if (this.isRTL)
            renderTransform.TranslateX = actualHeight - actualWidth;
          renderTransform.TranslateY = actualHeight;
          this.mediaPlayer.Width = actualHeight;
          this.mediaPlayer.Height = actualWidth;
          break;
        case 180:
          renderTransform.TranslateX = actualWidth;
          renderTransform.TranslateY = actualHeight;
          this.mediaPlayer.Width = actualWidth;
          this.mediaPlayer.Height = actualHeight;
          break;
        case 270:
          renderTransform.TranslateX = this.isRTL ? actualHeight : actualWidth;
          this.mediaPlayer.Width = actualHeight;
          this.mediaPlayer.Height = actualWidth;
          break;
        default:
          this.mediaPlayer.Width = actualWidth;
          this.mediaPlayer.Height = actualHeight;
          break;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/VideoPlayerPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.videoCanvas = (Canvas) this.FindName("videoCanvas");
      this.bufferingOverlay = (Grid) this.FindName("bufferingOverlay");
      this.bufferingOverlayBackground = (SolidColorBrush) this.FindName("bufferingOverlayBackground");
      this.bufferingBar = (ProgressBar) this.FindName("bufferingBar");
      this.controlBar = (Grid) this.FindName("controlBar");
      this.controlBarBackground = (SolidColorBrush) this.FindName("controlBarBackground");
      this.progressSlider = (Slider) this.FindName("progressSlider");
      this.bufferingProgressBar = (ProgressBar) this.FindName("bufferingProgressBar");
      this.jumpBackButton = (Button) this.FindName("jumpBackButton");
      this.playButton = (Button) this.FindName("playButton");
      this.jumpForwardButton = (Button) this.FindName("jumpForwardButton");
      this.timestampsRow = (Border) this.FindName("timestampsRow");
      this.timeWatchedText = (TextBlock) this.FindName("timeWatchedText");
      this.timeLeftText = (TextBlock) this.FindName("timeLeftText");
      this.thumbnailImage = (Image) this.FindName("thumbnailImage");
      this.mediaPlayer = (MediaElement) this.FindName("mediaPlayer");
    }
  }
}
