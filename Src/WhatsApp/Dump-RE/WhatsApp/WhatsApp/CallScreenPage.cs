// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallScreenPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WhatsApp.CommonOps;
using WhatsApp.WaViewModels;
using WhatsAppNative;
using Windows.Devices.Sensors;
using Windows.Foundation;

#nullable disable
namespace WhatsApp
{
  public class CallScreenPage : PhoneApplicationPage
  {
    public const string LogHeader = "callscreen";
    private static UiCallState? NextInstanceInitialState = new UiCallState?();
    private static string NextInstanceJid = (string) null;
    private static List<string> NextInstanceJids = new List<string>();
    private static bool? NextInstanceHasVideo = new bool?();
    private CallScreenViewModel viewModel;
    private string currPeerJid;
    private List<string> currPeersJids = new List<string>();
    private string currCallId;
    private IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState> currPeerStates = (IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>) new Dictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>();
    private UiCallState? initialState;
    private IDisposable callEndSub;
    private IDisposable cameraRestartSub;
    private IDisposable videoRestartedSub;
    private IDisposable vmSub;
    private IDisposable ringerSub;
    private IDisposable hideButtonsSub;
    private IDictionary<CallScreenViewModel.Peer, WaAnimations.FadeOutAndInTransition> stateAndDurationTransition = (IDictionary<CallScreenViewModel.Peer, WaAnimations.FadeOutAndInTransition>) new Dictionary<CallScreenViewModel.Peer, WaAnimations.FadeOutAndInTransition>();
    private bool closed;
    private bool pageRemoved;
    private bool ratingsPageNavigated;
    private bool actedOnIncoming;
    private bool ignoredWithTextReply;
    private bool textReplyPickerActed;
    private CallScreenPage.TextReplyOption selectedTextReplyOption;
    private const int callAddParticipantConfirmationMaxCount = 5;
    private Storyboard previewAnimation;
    private Storyboard videoLocationAnimation;
    private CallScreenHelper helper;
    private bool endedBySelf;
    private bool previewFullscreen;
    private GeneralTransform initialTransform;
    private IDisposable localVideoSubject;
    private IDisposable remoteVideoSubject;
    private SimpleOrientationSensor orientationSensor;
    internal Grid LayoutRoot;
    internal Grid remoteVideoGrid;
    internal ScaleTransform remoteScale;
    internal TranslateTransform remoteTranslation;
    internal MediaElement remoteVideo;
    internal Grid previewVideoGrid;
    internal TranslateTransform previewTranslation;
    internal ScaleTransform previewScale;
    internal MediaElement previewVideo;
    internal Grid previewVideoIconGrid;
    internal TranslateTransform previewIconTranslation;
    internal Rectangle UpperGradient;
    internal StackPanel LogoPanel;
    internal Image WhatsAppIcon;
    internal TextBlock TitleBlock;
    internal ZoomBox PeerProfile;
    internal Grid PeerOne;
    internal Ellipse MuteIconOverlayOne;
    internal ImageBrush AvatarOne;
    internal TextBlock StateAndDurationBlockOne;
    internal Grid PeerTwo;
    internal Ellipse MuteIconOverlayTwo;
    internal ImageBrush AvatarTwo;
    internal TextBlock StateAndDurationBlockTwo;
    internal Button CancelCallButtonTwo;
    internal Grid PeerThree;
    internal Ellipse MuteIconOverlayThree;
    internal ImageBrush AvatarThree;
    internal TextBlock StateAndDurationBlockThree;
    internal Button CancelCallButtonThree;
    internal ZoomBox IncomingPeerProfileOne;
    internal ZoomBox IncomingPeerProfileTwo;
    internal ZoomBox IncomingPeerProfileThree;
    internal ZoomBox videoPausedOverlay;
    internal TextBlock videoInfoButton;
    internal StackPanel videoInfoPanel;
    internal Grid CallButtons;
    internal Grid CallIncomingButtons;
    internal Button AnswerButton;
    internal Button TextReplyButton;
    internal Button IgnoreButton;
    internal Grid CallInProgressButtons;
    internal Button MessageButton;
    internal Button SpeakerButton;
    internal Button SwitchCameraButton;
    internal Button MuteButton;
    internal Button UpgradeButton;
    internal Button DowngradeButton;
    internal Button BluetoothButton;
    internal Button AddParticipantButton;
    internal Button EndCallButton;
    internal Grid CallUpgradeButtons;
    internal Button UpgradeConfirmButton;
    internal Button UpgradeDenyButton;
    internal Grid CallEndedPanel;
    internal Button CancelButton;
    internal Button RedialButton;
    internal ZoomBox UpgradeRequestOverlay;
    internal Border CancelUpgradeButton;
    internal ListPicker MessagePicker;
    private bool _contentLoaded;

    private bool PreviewFullscreen
    {
      get
      {
        switch (this.viewModel.CallState)
        {
          case UiCallState.Calling:
          case UiCallState.Ringing:
            this.previewFullscreen = true;
            break;
          default:
            if (this.viewModel.UpgradeState != UiUpgradeState.None)
            {
              this.previewFullscreen = true;
              break;
            }
            break;
        }
        return this.previewFullscreen;
      }
      set => this.previewFullscreen = value;
    }

    public CallScreenPage()
    {
      this.InitializeComponent();
      this.currPeerJid = CallScreenPage.NextInstanceJid;
      CallScreenPage.NextInstanceJid = (string) null;
      this.currPeersJids = new List<string>((IEnumerable<string>) CallScreenPage.NextInstanceJids);
      CallScreenPage.NextInstanceJids.Clear();
      this.initialState = CallScreenPage.NextInstanceInitialState;
      CallScreenPage.NextInstanceInitialState = new UiCallState?();
      this.viewModel = new CallScreenViewModel(this.currPeersJids);
      if (this.initialState.HasValue)
        this.viewModel.CallState = this.initialState.Value;
      if (CallScreenPage.NextInstanceHasVideo.HasValue)
      {
        this.viewModel.VideoEnabled = CallScreenPage.NextInstanceHasVideo.Value;
        CallScreenPage.NextInstanceHasVideo = new bool?();
      }
      this.vmSub = this.viewModel.GetObservable().ObserveOnDispatcher<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>(new Action<KeyValuePair<string, object>>(this.OnViewModelNotified));
      this.helper = new CallScreenHelper();
      this.DataContext = (object) this.viewModel;
      this.InitUI();
    }

    private void InitUI()
    {
      CallScreenViewModel viewModel = this.viewModel;
      this.TitleBlock.Text = AppResources.WhatsAppCallHeader;
      this.UpdateStateAndDurationBlock();
      this.CallIncomingButtons.Visibility = viewModel.CallIncomingButtonsVisibility;
      this.CallInProgressButtons.Visibility = viewModel.CallInProgressButtonsVisibility;
      this.UpdateIncomingPeerProfile();
      this.PeerProfile.Visibility = viewModel.SmallProfileVisibility;
      this.LogoPanel.HorizontalAlignment = this.viewModel.WhatsAppLogoAlignment;
      foreach (CallScreenViewModel.Peer peer in Enum.GetValues(typeof (CallScreenViewModel.Peer)))
      {
        Grid name = (Grid) this.FindName("Peer" + peer.ToString());
        if (name != null)
          name.Visibility = this.viewModel.PeerVisibility(peer);
      }
      if (!ImageStore.IsDarkTheme())
        SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.AnswerButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Answer_Tap);
      this.IgnoreButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Ignore_Tap);
      this.TextReplyButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.TextReply_Tap);
      this.MessageButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.MessageDuringCall_Tap);
      this.MuteButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Mute_Tap);
      this.SpeakerButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Speaker_Tap);
      this.EndCallButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.EndCall_Tap);
      this.SwitchCameraButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.SwitchCamera_Tap);
      this.BluetoothButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Bluetooth_Tap);
      this.UpgradeButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.UpgradeButton_Tap);
      this.DowngradeButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.DowngradeButton_Tap);
      this.UpgradeConfirmButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.UpgradeConfirmButton_Tap);
      this.UpgradeDenyButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.UpgradeDenyButton_Tap);
      this.CancelUpgradeButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.CancelUpgradeButton_Tap);
      this.AddParticipantButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.AddParticipant_Tap);
      this.CancelCallButtonTwo.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.CancelCallButton_Tap);
      this.CancelCallButtonThree.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.CancelCallButton_Tap);
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      if (Settings.IsWaAdmin)
      {
        this.videoInfoButton.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.VideoInfoButton_Tap);
        this.videoInfoPanel.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.VideoInfoPanel_Tap);
      }
      this.ResetVideoSubscriptions(this.viewModel.VideoEnabled);
    }

    private void UpdateIncomingPeerProfile()
    {
      foreach (CallScreenViewModel.Peer peer in Enum.GetValues(typeof (CallScreenViewModel.Peer)))
      {
        if (this.FindName("IncomingPeerProfile" + peer.ToString()) is ZoomBox name)
        {
          int num = peer == (CallScreenViewModel.Peer) this.currPeersJids.Count ? (int) this.viewModel.CallIncomingButtonsVisibility : 1;
          name.Visibility = (Visibility) num;
        }
      }
    }

    private void UpgradeDenyButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      e.Handled = true;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          voip.RejectVideoUpgrade(UpgradeRequestEndReason.EndedByUser);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, nameof (UpgradeDenyButton_Tap), false);
        }
      }));
    }

    private void UpgradeConfirmButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      e.Handled = true;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        if (WaAudioRouting.GetCurrentAudioEndpoint() == WaAudioRouting.Endpoint.Earpiece)
          WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Speaker);
        voip.AcceptVideoUpgrade();
      }));
    }

    private void CancelUpgradeButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      e.Handled = true;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          voip.CancelVideoUpgrade(UpgradeRequestEndReason.EndedByUser);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, nameof (CancelUpgradeButton_Tap), false);
        }
      }));
    }

    private bool CheckIfAVSwitchAllowed(IVoip voip)
    {
      ICallInfo info = voip as ICallInfo;
      string errMsg = (string) null;
      CallParticipantDetail? firstPeer = info.GetFirstPeer();
      if (firstPeer.HasValue)
      {
        if (firstPeer.Value.IsAudioVideoSwitchSupported && firstPeer.Value.IsAudioVideoSwitchEnabled)
          return true;
        string displayName = UserCache.Get(this.currPeerJid, true).GetDisplayName(true);
        errMsg = !firstPeer.Value.IsAudioVideoSwitchSupported ? string.Format(AppResources.CallPeerAudioVideoSwitchNotSupported, (object) displayName) : string.Format(AppResources.CallPeerAudioVideoSwitchNotEnabled, (object) displayName);
      }
      if (errMsg != null)
      {
        int num;
        this.Dispatcher.BeginInvoke((Action) (() => num = (int) MessageBox.Show(errMsg)));
      }
      return false;
    }

    private void UpgradeButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent() || this.viewModel.usersJids.Count > 1)
        return;
      e.Handled = true;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        if (!this.CheckIfAVSwitchAllowed(voip))
          return;
        if (this.viewModel.VideoEnabled)
          voip.TurnCamera(true);
        else
          voip.RequestVideoUpgrade();
      }));
    }

    private void DowngradeButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent())
        return;
      if (this.CancelButton.Visibility == Visibility.Visible)
        this.CancelUpgradeButton_Tap(sender, e);
      e.Handled = true;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        if (!this.CheckIfAVSwitchAllowed(voip))
          return;
        voip.TurnCamera(false);
      }));
    }

    private void VideoInfoPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.HideVideoInfo();
      e.Handled = true;
    }

    private void VideoInfoButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.ShowVideoInfo();
      e.Handled = true;
    }

    private void RedialButton_Click(object sender, EventArgs e)
    {
      if (this.viewModel.VideoEnabled)
        CallContact.VideoCall(this.currPeerJid, true, true, "redial");
      else
        CallContact.Call(this.currPeerJid, true, true, "redial");
    }

    private void CancelButton_Click(object sender, EventArgs e) => this.Close();

    private void CompleteVideoMove(TranslateTransform location, ManipulationCompletedEventArgs e)
    {
      this.AnimateVideoToClosestCorner(location);
    }

    private void MoveVideo(TranslateTransform transform, ManipulationDeltaEventArgs e)
    {
      if (this.initialTransform == null)
        this.initialTransform = e.ManipulationContainer.TransformToVisual((UIElement) this.LayoutRoot);
      System.Windows.Point point1 = this.initialTransform.Transform(e.DeltaManipulation.Translation);
      System.Windows.Point point2 = this.initialTransform.Transform(new System.Windows.Point());
      transform.X += point1.X - point2.X;
      transform.Y += point1.Y - point2.Y;
      e.Handled = true;
    }

    private void RemoteVideo_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (!this.PreviewFullscreen)
        return;
      this.initialTransform = (GeneralTransform) null;
      this.CompleteVideoMove(this.remoteTranslation, e);
      e.Handled = true;
    }

    private void RemoteVideo_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (!this.PreviewFullscreen)
        return;
      this.MoveVideo(this.remoteTranslation, e);
    }

    private void PreviewVideo_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.PreviewFullscreen)
        return;
      this.initialTransform = (GeneralTransform) null;
      this.CompleteVideoMove(this.previewTranslation, e);
      e.Handled = true;
    }

    private void PreviewVideo_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.PreviewFullscreen)
        return;
      this.MoveVideo(this.previewTranslation, e);
    }

    private void RemoteVideo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!this.PreviewFullscreen)
        return;
      this.MinimizePreview();
      e.Handled = true;
    }

    private void PreviewVideo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.PreviewFullscreen)
        return;
      this.MaximizePreview();
      e.Handled = true;
    }

    private CallScreenPage.PreviewLocation ClosestCorner(double currX, double currY)
    {
      CallScreenPage.PreviewLocation previewLocation = (CallScreenPage.PreviewLocation) 0;
      if (currX < 0.0)
        previewLocation |= CallScreenPage.PreviewLocation.Left;
      if (currY < 0.0)
        previewLocation |= CallScreenPage.PreviewLocation.Top;
      return previewLocation;
    }

    private void AnimateVideoToClosestCorner(TranslateTransform location)
    {
      this.AnimateVideoToLocation(location, this.ClosestCorner(location.X, location.Y));
    }

    private void AnimateVideoToLocation(
      TranslateTransform location,
      CallScreenPage.PreviewLocation destination)
    {
      double coordinate1 = (destination.HasFlag((Enum) CallScreenPage.PreviewLocation.Left) ? -1.0 : 1.0) * 0.7 * this.ActualWidth / 2.0;
      double coordinate2 = (destination.HasFlag((Enum) CallScreenPage.PreviewLocation.Top) ? -1.0 : 1.0) * 0.7 * this.ActualHeight / 2.0;
      if (this.hideButtonsSub != null && coordinate2 > 0.0)
        coordinate2 -= 200.0;
      this.videoLocationAnimation = new Storyboard();
      this.videoLocationAnimation.Children.Add((Timeline) this.CreateAnimation(coordinate1, location, TranslateTransform.XProperty));
      this.videoLocationAnimation.Children.Add((Timeline) this.CreateAnimation(coordinate2, location, TranslateTransform.YProperty));
      this.videoLocationAnimation.Begin();
      Settings.VideoCallPreviewLocation = (int) destination;
    }

    private DoubleAnimation CreateAnimation(
      double coordinate,
      TranslateTransform location,
      DependencyProperty property)
    {
      DoubleAnimation element = new DoubleAnimation();
      element.To = new double?(coordinate);
      ExponentialEase exponentialEase = new ExponentialEase();
      exponentialEase.EasingMode = EasingMode.EaseOut;
      element.EasingFunction = (IEasingFunction) exponentialEase;
      element.Duration = (Duration) TimeSpan.FromSeconds(0.3);
      Storyboard.SetTargetProperty((Timeline) element, new PropertyPath((object) property));
      Storyboard.SetTarget((Timeline) element, (DependencyObject) location);
      return element;
    }

    private void CallScreenPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!this.viewModel.VideoEnabled || this.viewModel.CallState == UiCallState.Ending)
        return;
      if (this.hideButtonsSub == null)
      {
        IObservable<Unit> rightSource1 = Observable.FromEvent<ManipulationStartedEventArgs>((Action<EventHandler<ManipulationStartedEventArgs>>) (handler => this.ManipulationStarted += handler), (Action<EventHandler<ManipulationStartedEventArgs>>) (handler => this.ManipulationStarted -= handler)).SubscribeOnDispatcher<IEvent<ManipulationStartedEventArgs>>().Select<IEvent<ManipulationStartedEventArgs>, Unit>((Func<IEvent<ManipulationStartedEventArgs>, Unit>) (_ => new Unit()));
        IObservable<Unit> rightSource2 = Observable.FromEvent<ManipulationDeltaEventArgs>((Action<EventHandler<ManipulationDeltaEventArgs>>) (handler => this.ManipulationDelta += handler), (Action<EventHandler<ManipulationDeltaEventArgs>>) (handler => this.ManipulationDelta -= handler)).SubscribeOnDispatcher<IEvent<ManipulationDeltaEventArgs>>().Select<IEvent<ManipulationDeltaEventArgs>, Unit>((Func<IEvent<ManipulationDeltaEventArgs>, Unit>) (_ => new Unit()));
        IObservable<Unit> rightSource3 = Observable.FromEvent<ManipulationCompletedEventArgs>((Action<EventHandler<ManipulationCompletedEventArgs>>) (handler => this.ManipulationCompleted += handler), (Action<EventHandler<ManipulationCompletedEventArgs>>) (handler => this.ManipulationCompleted -= handler)).SubscribeOnDispatcher<IEvent<ManipulationCompletedEventArgs>>().Select<IEvent<ManipulationCompletedEventArgs>, Unit>((Func<IEvent<ManipulationCompletedEventArgs>, Unit>) (_ => new Unit()));
        this.hideButtonsSub = Observable.Return<Unit>(new Unit()).Merge<Unit>(rightSource1).Merge<Unit>(rightSource2).Merge<Unit>(rightSource3).Throttle<Unit>(TimeSpan.FromSeconds(5.0)).Take<Unit>(1).ObserveOnDispatcher<Unit>().Do<Unit>((Action<Unit>) (_ => this.HideButtons())).Subscribe<Unit>();
        this.ShowButtons();
      }
      else
        this.HideButtons();
    }

    private void OnRemoteVideoStateChanged(UiVideoState? newState)
    {
      if (!newState.HasValue)
        return;
      this.remoteVideo.SetMediaPlayerState(newState.Value, newSource: new Uri("ms-media-stream-id:MediaStreamer-867"));
      UiVideoState? nullable = newState;
      if (!nullable.HasValue)
        return;
      switch (nullable.GetValueOrDefault())
      {
        case UiVideoState.None:
          this.MaximizePreview();
          this.ShowButtons();
          break;
        case UiVideoState.Playing:
          this.MinimizePreview();
          this.HideButtons();
          break;
      }
    }

    private void Voip_OnVideoPlayerRestarted(Unit arg)
    {
      Log.l("callscreen", "VideoPlayerRestarted");
      this.remoteVideo.SetMediaPlayerState(UiVideoState.None, true);
      this.remoteVideo.SetMediaPlayerState(UiVideoState.Playing, true, new Uri("ms-media-stream-id:MediaStreamer-867"));
    }

    private void OnLocalVideoStateChanged(UiVideoState? newState)
    {
      if (!newState.HasValue)
        return;
      this.previewVideo.SetMediaPlayerState(newState.Value);
    }

    private void RunPreviewAnimation(Storyboard newAnimation)
    {
      if (this.previewAnimation == newAnimation)
        return;
      if (this.previewAnimation != null)
      {
        this.previewAnimation.Completed -= new EventHandler(this.OnPreviewAnimationCompleted);
        this.previewAnimation.Stop();
        this.previewAnimation = (Storyboard) null;
      }
      this.previewAnimation = newAnimation;
      this.previewAnimation.Completed += new EventHandler(this.OnPreviewAnimationCompleted);
      this.previewAnimation.Begin();
    }

    private void OnPreviewAnimationCompleted(object sender, EventArgs e)
    {
      if (this.previewAnimation != sender)
        return;
      this.previewAnimation.Completed -= new EventHandler(this.OnPreviewAnimationCompleted);
      this.previewAnimation = (Storyboard) null;
    }

    private void MinimizePreview()
    {
      if (this.viewModel.CallState == UiCallState.ReceivedCall)
        return;
      this.AnimateVideoToLocation(this.previewTranslation, (CallScreenPage.PreviewLocation) Settings.VideoCallPreviewLocation);
      this.RunPreviewAnimation((Storyboard) this.Resources[(object) "minimizePreview"]);
      this.PreviewFullscreen = false;
    }

    private void MaximizePreview()
    {
      this.AnimateVideoToLocation(this.remoteTranslation, (CallScreenPage.PreviewLocation) Settings.VideoCallPreviewLocation);
      this.RunPreviewAnimation((Storyboard) this.Resources[(object) "maximizePreview"]);
      this.PreviewFullscreen = true;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.viewModel.RenderWindowSize = e.NewSize;
    }

    private void HideButtons()
    {
      if (this.viewModel.CallState == UiCallState.ReceivedCall)
        return;
      Storyboard resource = (Storyboard) this.Resources[(object) "hideUi"];
      resource.Completed += new EventHandler(this.FadeOut_Completed);
      resource.Begin();
      this.hideButtonsSub.SafeDispose();
      this.hideButtonsSub = (IDisposable) null;
      this.AnimateVideoToClosestCorner(this.PreviewFullscreen ? this.remoteTranslation : this.previewTranslation);
    }

    private void FadeOut_Completed(object sender, EventArgs e)
    {
      ((Timeline) this.Resources[(object) "hideUi"]).Completed -= new EventHandler(this.FadeOut_Completed);
    }

    private void ShowButtons()
    {
      Storyboard resource = (Storyboard) this.Resources[(object) "hideUi"];
      resource.Seek(TimeSpan.FromSeconds(0.0));
      resource.Stop();
      this.AnimateVideoToClosestCorner(this.PreviewFullscreen ? this.remoteTranslation : this.previewTranslation);
    }

    public static void Launch(
      string peerJid,
      UiCallState? initialState,
      bool replacePage = false,
      bool hasVideo = false,
      bool forceNavigate = false)
    {
      List<string> peersJids = new List<string>()
      {
        peerJid
      };
      try
      {
        peersJids = Voip.Instance.GetCallPeers().Select<CallParticipantDetail, string>((Func<CallParticipantDetail, string>) (p => p.Jid)).ToList<string>();
        UiCallState? nullable = initialState;
        UiCallState uiCallState = UiCallState.ReceivedCall;
        if ((nullable.GetValueOrDefault() == uiCallState ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          if (peersJids != null)
          {
            if (peersJids.Count > 1)
              VoipPictureStore.EnsureVoipContactsPhoto(peersJids.ToArray());
          }
        }
      }
      catch
      {
      }
      CallScreenPage.Launch(peersJids, initialState, replacePage, hasVideo, forceNavigate);
    }

    public static void Launch(
      List<string> peersJids,
      UiCallState? initialState,
      bool replacePage = false,
      bool hasVideo = false,
      bool forceNavigate = false)
    {
      CallScreenPage.NextInstanceJids = peersJids;
      string peerJid;
      CallScreenPage.NextInstanceJid = peerJid = peersJids.FirstOrDefault<string>();
      CallScreenPage.NextInstanceInitialState = initialState;
      CallScreenPage.NextInstanceHasVideo = new bool?(hasVideo);
      WaUriParams uriParams = WaUriParams.ForCallScreen(peerJid);
      if (replacePage)
        uriParams.AddBool("PageReplace", replacePage);
      if (hasVideo)
        uriParams.AddBool("VideoCall", hasVideo);
      if (forceNavigate)
        uriParams.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
      NavUtils.NavigateToPage(nameof (CallScreenPage), uriParams);
    }

    private void InitVoipSubscriptions()
    {
      this.callEndSub.SafeDispose();
      this.callEndSub = (IDisposable) null;
      this.cameraRestartSub.SafeDispose();
      this.cameraRestartSub = (IDisposable) null;
      this.videoRestartedSub.SafeDispose();
      this.videoRestartedSub = (IDisposable) null;
      this.callEndSub = VoipHandler.CallEndedSubject.Take<WaCallEndedEventArgs>(1).ObserveOnDispatcher<WaCallEndedEventArgs>().Subscribe<WaCallEndedEventArgs>(new Action<WaCallEndedEventArgs>(this.Voip_CallEnded));
      this.cameraRestartSub = VoipHandler.CameraRestartSubject.Subscribe<bool>(new Action<bool>(this.Voip_CameraRestarted));
      this.videoRestartedSub = VoipHandler.VideoRestarted.ObserveOnDispatcher<Unit>().Subscribe<Unit>(new Action<Unit>(this.Voip_OnVideoPlayerRestarted));
    }

    private void ResetVideoSubscriptions(bool videoEnabled)
    {
      if ((this.localVideoSubject != null ? 1 : (this.remoteVideoSubject != null ? 1 : 0)) == (videoEnabled ? 1 : 0))
        return;
      this.localVideoSubject.SafeDispose();
      this.localVideoSubject = (IDisposable) null;
      this.remoteVideoSubject.SafeDispose();
      this.remoteVideoSubject = (IDisposable) null;
      if (videoEnabled)
      {
        this.previewVideoGrid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewVideo_Tap);
        this.remoteVideoGrid.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.RemoteVideo_Tap);
        this.previewVideoGrid.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.PreviewVideo_ManipulationDelta);
        this.previewVideoGrid.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.PreviewVideo_ManipulationCompleted);
        this.remoteVideoGrid.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.RemoteVideo_ManipulationCompleted);
        this.remoteVideoGrid.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.RemoteVideo_ManipulationDelta);
        this.localVideoSubject = this.viewModel.LocalVideoStateChangedSubject.Subscribe<UiVideoState?>(new Action<UiVideoState?>(this.OnLocalVideoStateChanged));
        this.remoteVideoSubject = this.viewModel.RemoteVideoStateChangedSubject.Subscribe<UiVideoState?>(new Action<UiVideoState?>(this.OnRemoteVideoStateChanged));
        this.remoteVideo.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(this.OnRemoteVideoMediaFailed);
        this.remoteVideo.CurrentStateChanged += new RoutedEventHandler(this.OnRemoteVideoCurrentStateChanged);
        this.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.CallScreenPage_Tap);
        this.EnableOrientationSensor = true;
      }
      else
      {
        this.EnableOrientationSensor = false;
        this.previewVideoGrid.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.PreviewVideo_Tap);
        this.remoteVideoGrid.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.RemoteVideo_Tap);
        this.previewVideoGrid.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.PreviewVideo_ManipulationDelta);
        this.previewVideoGrid.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.PreviewVideo_ManipulationCompleted);
        this.remoteVideoGrid.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.RemoteVideo_ManipulationCompleted);
        this.remoteVideoGrid.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.RemoteVideo_ManipulationDelta);
        this.remoteVideo.MediaFailed -= new EventHandler<ExceptionRoutedEventArgs>(this.OnRemoteVideoMediaFailed);
        this.remoteVideo.CurrentStateChanged -= new RoutedEventHandler(this.OnRemoteVideoCurrentStateChanged);
        this.Tap -= new EventHandler<System.Windows.Input.GestureEventArgs>(this.CallScreenPage_Tap);
      }
    }

    private void OnRemoteVideoMediaFailed(object s, ExceptionRoutedEventArgs e)
    {
      Log.l("VoipMSS", e.ErrorException.ToString());
    }

    private void OnRemoteVideoCurrentStateChanged(object s, RoutedEventArgs e)
    {
      Log.l("VoipMSS", this.remoteVideo.CurrentState.ToString());
    }

    private void ScheduleClose(
      int miliseconds,
      CallScreenPage.CloseOption closeOption,
      string context,
      byte[] ratingCookie = null)
    {
      if (!this.Dispatcher.CheckAccess())
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.ScheduleClose(miliseconds, closeOption, context, ratingCookie)));
      }
      else
      {
        Log.l("callscreen", "schedule close | delay:{0},context:{1}", (object) miliseconds, (object) context);
        this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds((double) miliseconds), (Action) (() => this.Close(closeOption, context, ratingCookie)));
      }
    }

    private void Close(CallScreenPage.CloseOption closeOption = CallScreenPage.CloseOption.None, string context = null, byte[] ratingCookie = null)
    {
      Log.d("callscreen", "close | context:{0},close option:{1}", (object) context, (object) closeOption);
      bool closed = this.closed;
      this.closed = true;
      if (closeOption == CallScreenPage.CloseOption.ToRating && !this.ratingsPageNavigated)
      {
        this.ratingsPageNavigated = true;
        CallRatingPage.Start(this.currPeerJid, ratingCookie, true, false);
      }
      else if (closed)
      {
        Log.d("callscreen", "already closed");
      }
      else
      {
        if (!(App.CurrentApp.CurrentPage is CallScreenPage))
          Log.d("callscreen", "wrong nav | no longer on call screen");
        if (closeOption == CallScreenPage.CloseOption.BackToChat && this.currPeerJid != null)
        {
          ChatPage.NextInstanceInitState = new ChatPage.InitState()
          {
            InputMode = ChatPage.InputMode.Keyboard
          };
          NavUtils.NavigateToChat(this.NavigationService, this.currPeerJid, true);
        }
        else
          NavUtils.GoBack(this.NavigationService);
      }
    }

    private bool Ticking
    {
      get
      {
        CallParticipantDetail details = this.viewModel.PeersDetails.FirstOrDefault<CallParticipantDetail>();
        UiCallParticipantState participantState = Voip.TranslateCallParticipantState(details);
        return !details.IsMuted && participantState == UiCallParticipantState.Active || this.viewModel.VideoEnabled;
      }
    }

    private void UpdateOnUICallStateChanged()
    {
      if (this.closed)
        return;
      this.helper.SyncPresetMuteState(this.viewModel.CallState).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (muted => this.viewModel.IsMuted = muted));
      if (this.viewModel.CallState == UiCallState.Active)
        this.helper.GetDuration().ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (durationInMs => this.viewModel.SetDuration(new int?(durationInMs / 1000), true)));
      else
        this.viewModel.SetDuration(new int?(), false);
      this.CallIncomingButtons.Visibility = this.viewModel.CallIncomingButtonsVisibility;
      this.CallInProgressButtons.Visibility = this.viewModel.CallInProgressButtonsVisibility;
      this.UpdateIncomingPeerProfile();
      this.PeerProfile.Visibility = this.viewModel.SmallProfileVisibility;
      this.CallEndedPanel.Visibility = this.viewModel.CallEndedButtonsVisibility;
      this.LogoPanel.HorizontalAlignment = this.viewModel.WhatsAppLogoAlignment;
    }

    private void UpdateOnUIPeersStateChanged()
    {
      foreach (CallScreenViewModel.Peer peer1 in Enum.GetValues(typeof (CallScreenViewModel.Peer)))
      {
        CallScreenViewModel.Peer peer = peer1;
        Grid name1 = (Grid) this.FindName("Peer" + peer.ToString());
        if (name1 != null)
          name1.Visibility = this.viewModel.PeerVisibility(peer);
        WaAnimations.FadeOutAndInTransition outAndInTransition = (WaAnimations.FadeOutAndInTransition) null;
        this.stateAndDurationTransition.TryGetValue(peer, out outAndInTransition);
        if (outAndInTransition == null)
        {
          TextBlock name2 = (TextBlock) this.FindName("StateAndDurationBlock" + peer.ToString());
          if (name2 != null)
            this.stateAndDurationTransition[peer] = new WaAnimations.FadeOutAndInTransition((UIElement) name2, (Action) (() => this.UpdateStateAndDurationBlockForPeer(peer)));
        }
        CallScreenViewModel.PeerCallState peerCallState1 = new CallScreenViewModel.PeerCallState();
        this.currPeerStates.TryGetValue(peer, out peerCallState1);
        CallScreenViewModel.PeerCallState peerCallState2 = new CallScreenViewModel.PeerCallState();
        this.viewModel.PeerCallStates.TryGetValue(peer, out peerCallState2);
        if (peerCallState1.State != peerCallState2.State || peerCallState1.Muted != peerCallState2.Muted)
          this.stateAndDurationTransition[peer].Perform();
      }
    }

    private void UpdateWithCurrentCallInfo()
    {
      if (this.closed)
        return;
      UiCallState? cachedInitialState = this.initialState;
      this.initialState = new UiCallState?();
      this.helper.PerformVoipWorkObservable((Action<IVoip>) (voip =>
      {
        string peerJid = (string) null;
        string callId = (string) null;
        CallInfoStruct callInfo;
        if (!voip.GetCallInfo(out callId, out peerJid, out callInfo) || callInfo.CallState == CallState.None)
          return;
        CallParticipantDetail? selfInfo = voip.GetSelf();
        List<CallParticipantDetail> peersInfo = voip.GetCallPeers();
        List<string> peersJids = peersInfo.Select<CallParticipantDetail, string>((Func<CallParticipantDetail, string>) (p => p.Jid)).ToList<string>();
        if (!selfInfo.HasValue || peersInfo.Count <= 0)
          return;
        int cameraCount = 0;
        if (callInfo.VideoEnabled)
          cameraCount = voip.GetCameraCount();
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.closed || this.pageRemoved)
            return;
          UiCallState uiCallState = Voip.TranslateCallState(callInfo.CallState);
          this.currCallId = callId;
          if (this.currPeersJids.Except<string>((IEnumerable<string>) peersJids).Any<string>() || peersJids.Except<string>((IEnumerable<string>) this.currPeersJids).Any<string>())
          {
            this.viewModel.SetPeersJids(peersJids);
            this.currPeersJids = peersJids;
          }
          IObservable<bool> source;
          if (uiCallState == UiCallState.Active)
          {
            this.viewModel.SetDuration(new int?(callInfo.CallDuration / 1000), true);
            source = this.helper.SyncPresetMuteState(uiCallState);
          }
          else
            source = this.helper.GetMuteState(uiCallState);
          source.ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (muted =>
          {
            if (this.closed)
              return;
            this.viewModel.IsMuted = muted;
          }));
          this.viewModel.UpdateFromCallInfo(callId, callInfo, selfInfo.Value, peersInfo);
          if (callInfo.VideoEnabled)
          {
            this.viewModel.CameraCount = cameraCount;
            this.previewVideo.SetMediaPlayerState(Voip.CallInfoToLocalVideoState(callInfo, selfInfo.Value), true, this.viewModel.LocalCameraSource);
          }
          WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(500.0), (Action) (() => this.viewModel.StartAudioRoutingSubscription()));
        }));
      })).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (success =>
      {
        if (success)
          return;
        if (cachedInitialState.HasValue && cachedInitialState.Value == UiCallState.Calling)
        {
          this.Dispatcher.RunAfterDelay(TimeSpan.FromMilliseconds(1000.0), (Action) (() => this.UpdateWithCurrentCallInfo()));
        }
        else
        {
          Log.l("callscreen", "update with current info failed");
          if (this.pageRemoved)
            Log.l("callscreen", "page is already removed, don't request close");
          else
            this.Close(context: "update with current info failed");
        }
      }));
    }

    private void UpdateStateAndDurationBlock()
    {
      if (this.viewModel == null)
        return;
      foreach (CallScreenViewModel.Peer peer in Enum.GetValues(typeof (CallScreenViewModel.Peer)))
        this.UpdateStateAndDurationBlockForPeer(peer);
    }

    private void UpdateStateAndDurationBlockForPeer(CallScreenViewModel.Peer peer)
    {
      if (this.viewModel == null)
        return;
      TextBlock name1 = (TextBlock) this.FindName("StateAndDurationBlock" + peer.ToString());
      if (name1 != null)
      {
        name1.Text = this.viewModel.StateAndDuration(peer);
        name1.FontFamily = this.viewModel.StateAndDurationFontFamily(peer);
        name1.Foreground = this.viewModel.StateAndDurationBrush(peer);
        ImageBrush name2 = (ImageBrush) this.FindName("Avatar" + peer.ToString());
        if (name2 != null)
          name2.Opacity = this.viewModel.PeerMuted(peer) ? 0.3 : 1.0;
        if (this.FindName("MuteIconOverlay" + peer.ToString()) is Ellipse name3)
          name3.Visibility = this.viewModel.PeerMuted(peer).ToVisibility();
      }
      CallScreenViewModel.PeerCallState peerCallState = new CallScreenViewModel.PeerCallState();
      this.viewModel.PeerCallStates.TryGetValue(peer, out peerCallState);
      this.currPeerStates[peer] = peerCallState;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      if (Voip.CancelDelayedCall())
        this.viewModel.CallState = UiCallState.Ending;
      if (this.viewModel.VideoEnabled)
      {
        this.previewVideo.SetMediaPlayerState(UiVideoState.None, true);
        this.remoteVideo.SetMediaPlayerState(UiVideoState.None, true);
      }
      base.OnNavigatedFrom(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.selectedTextReplyOption != null)
      {
        CallScreenPage.TextReplyOption selectedTextReplyOption = this.selectedTextReplyOption;
        this.selectedTextReplyOption = (CallScreenPage.TextReplyOption) null;
        selectedTextReplyOption.Act();
        if (selectedTextReplyOption.TryNavigation())
          return;
      }
      if (this.viewModel.CallState == UiCallState.Ending)
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.Close(context: "nav to ending call screen")));
      }
      else
      {
        WaUriParams waUriParams = new WaUriParams(this.NavigationContext.QueryString);
        if (this.currPeerJid == null)
        {
          string val = (string) null;
          if (waUriParams.TryGetStrValue("jid", out val) && !string.IsNullOrEmpty(val))
          {
            this.currPeerJid = val;
            this.viewModel.SetPeersJids(new List<string>()
            {
              this.currPeerJid
            });
          }
        }
        else
          this.viewModel.UpdatePeersCallStates();
        bool val1;
        if (waUriParams.TryGetBoolValue("VideoCall", out val1))
        {
          this.viewModel.VideoEnabled = val1;
          this.ResetVideoSubscriptions(val1);
        }
        if (this.viewModel.CallState == UiCallState.ReceivedCall && this.ringerSub == null)
          this.ringerSub = InAppRinger.GetRingObservable(this.currPeerJid).Subscribe<Unit>();
        this.InitVoipSubscriptions();
        this.Dispatcher.BeginInvoke((Action) (() => this.UpdateWithCurrentCallInfo()));
        NavUtils.LogBackStack(true);
        int num1 = 0;
        int num2 = 0;
        foreach (JournalEntry back in this.NavigationService.BackStack)
        {
          ++num1;
          if (back.Source.OriginalString.Contains(UriUtils.CreatePageUriStr(nameof (CallScreenPage))))
            num2 = num1;
        }
        if (num2 <= 0)
          return;
        for (int index = 0; index < num2; ++index)
          this.NavigationService.RemoveBackEntry();
        Log.d("callscreen", "removed {0} back entries", (object) num2);
        NavUtils.LogBackStack(true);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      base.OnRemovedFromJournal(e);
      this.EnableOrientationSensor = false;
      this.videoRestartedSub.SafeDispose();
      this.videoRestartedSub = (IDisposable) null;
      this.vmSub.SafeDispose();
      this.vmSub = (IDisposable) null;
      this.viewModel.SafeDispose();
      this.helper.SafeDispose();
      this.ringerSub.SafeDispose();
      this.ringerSub = (IDisposable) null;
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      e.Cancel = true;
      base.OnBackKeyPress(e);
      if (this.viewModel.CallState == UiCallState.ReceivedCall)
        return;
      NavUtils.GoBack(this.NavigationService);
    }

    private void OnViewModelNotified(KeyValuePair<string, object> p)
    {
      if (p.Key == "e:UICallStateChanged")
        this.UpdateOnUICallStateChanged();
      else if (p.Key == "r:Duration")
      {
        if (!this.Ticking)
          return;
        this.StateAndDurationBlockOne.Text = this.viewModel.DurationStr;
      }
      else if (p.Key == "e:PictureChanged")
        this.WhatsAppIcon.Source = this.viewModel.WhatsAppIconSource;
      else if (p.Key == "e:VideoEnabled")
      {
        this.ResetVideoSubscriptions(this.viewModel.VideoEnabled);
        this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
        {
          int cameraCount = 0;
          if (this.viewModel.VideoEnabled)
            cameraCount = voip.GetCameraCount();
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            if (!this.viewModel.VideoEnabled)
              return;
            this.viewModel.CameraCount = cameraCount;
          }));
        }));
      }
      else if (p.Key == "e:UIUpgradeStateChanged")
      {
        this.CallUpgradeButtons.Visibility = this.viewModel.UpgradeButtonsVisibility;
        this.CallInProgressButtons.Visibility = this.viewModel.CallInProgressButtonsVisibility;
        this.CancelUpgradeButton.Visibility = this.viewModel.UpgradeCancelVisibility;
      }
      else
      {
        if (!(p.Key == "e:PeersChanged"))
          return;
        this.UpdateOnUIPeersStateChanged();
      }
    }

    private void Answer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.actedOnIncoming)
        return;
      string myJid = Settings.MyJid;
      Log.l("callscreen", "answer incoming call");
      this.ringerSub.SafeDispose();
      this.ringerSub = (IDisposable) null;
      this.actedOnIncoming = true;
      this.helper.PerformVoipWorkObservable((Action<IVoip>) (voip => voip.AcceptCall())).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (success =>
      {
        if (success)
          return;
        this.Close(context: "accept call error");
      }));
      e.Handled = true;
    }

    private void Ignore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.actedOnIncoming)
        return;
      Log.l("callscreen", "ignore incoming call");
      this.ringerSub.SafeDispose();
      this.ringerSub = (IDisposable) null;
      this.actedOnIncoming = true;
      this.helper.PerformVoipWorkObservable((Action<IVoip>) (voip => voip.RejectCall())).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (success =>
      {
        if (success)
          return;
        this.Close(context: "ignore call error");
      }));
      e.Handled = true;
    }

    private void MessageDuringCall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel.usersJids.Count > 1 || this.helper.IsTapTooFrequent())
        return;
      this.Dispatcher.BeginInvoke((Action) (() => this.Close(CallScreenPage.CloseOption.BackToChat, "message during call")));
      e.Handled = true;
    }

    private void TextReply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent() || this.actedOnIncoming || this.ignoredWithTextReply)
        return;
      this.ignoredWithTextReply = true;
      this.ringerSub.SafeDispose();
      this.ringerSub = (IDisposable) null;
      this.helper.PerformVoipWorkObservable((Action<IVoip>) (voip => voip.RejectCall())).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (success =>
      {
        if (success)
          return;
        this.Close(context: "ignore call with text");
      }));
      this.MessagePicker.FullModeHeader = (object) string.Format(AppResources.CallScreenTextReplyPickerHeader, (object) UserCache.Get(this.currPeerJid, true).GetDisplayName());
      CallScreenPage.TextReplyOption textReplyOption = new CallScreenPage.TextReplyOption("", (Action) null);
      this.MessagePicker.ItemsSource = (IEnumerable) new CallScreenPage.TextReplyOption[4]
      {
        new CallScreenPage.TextReplyOption(string.Format("\"{0}\"", (object) AppResources.IgnoreCallTextReply0), (Action) (() => this.helper.SendTextReply(this.currPeerJid, AppResources.IgnoreCallTextReply0))),
        new CallScreenPage.TextReplyOption(string.Format("\"{0}\"", (object) AppResources.IgnoreCallTextReply1), (Action) (() => this.helper.SendTextReply(this.currPeerJid, AppResources.IgnoreCallTextReply1))),
        new CallScreenPage.TextReplyOption(AppResources.IgnoreCallTypeAMessage, (Action) null, (Action) (() => this.Dispatcher.BeginInvoke((Action) (() => this.Close(CallScreenPage.CloseOption.BackToChat, "text reply for incoming call"))))),
        textReplyOption
      };
      this.MessagePicker.SelectedItem = (object) textReplyOption;
      this.MessagePicker.Open();
      e.Handled = true;
    }

    private void Mute_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent())
        return;
      this.helper.ToggleMute(this.viewModel.CallState).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (muted => this.viewModel.IsMuted = muted));
      e.Handled = true;
    }

    private void Bluetooth_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent())
        return;
      if (this.viewModel.VideoEnabled)
      {
        if (WaAudioRouting.GetCurrentAudioEndpoint() == WaAudioRouting.Endpoint.Bluetooth)
          WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Speaker);
        else
          WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Bluetooth);
      }
      else
        WaAudioRouting.ToggleBluetooth();
      e.Handled = true;
    }

    private void Speaker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent())
        return;
      WaAudioRouting.ToggleSpeaker();
      e.Handled = true;
    }

    private void SwitchCamera_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.helper.IsTapTooFrequent())
        return;
      e.Handled = true;
      if (this.viewModel.CameraCount <= 1)
        return;
      this.viewModel.PreviewVisibility = Visibility.Collapsed;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        CallParticipantDetail? selfInfo = voip.GetSelf();
        List<CallParticipantDetail> peersInfo = voip.GetCallPeers();
        voip.ToggleCamera();
        string callId;
        CallInfoStruct callInfo;
        if (!voip.GetCallInfo(out callId, out string _, out callInfo) || !selfInfo.HasValue || peersInfo.Count <= 0)
          return;
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.viewModel.UpdateFromCallInfo(callId, callInfo, selfInfo.Value, peersInfo);
          if (!this.PreviewFullscreen)
            this.previewTranslation.X = -this.previewTranslation.X;
          this.viewModel.PreviewVisibility = Visibility.Visible;
        }));
      }));
    }

    private void EndCall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      Log.l("callscreen", "end call");
      this.viewModel.CallState = UiCallState.Ending;
      this.endedBySelf = true;
      if (!Voip.CancelDelayedCall())
        this.helper.EnqueueVoipWork((Action<IVoip>) (voip => voip.EndCall(true)));
      this.ScheduleClose(3000, CallScreenPage.CloseOption.None, "to be safe");
      e.Handled = true;
    }

    private void TextReplyOption_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.textReplyPickerActed)
        return;
      this.textReplyPickerActed = true;
      if (sender is FrameworkElement frameworkElement)
        this.selectedTextReplyOption = frameworkElement.Tag as CallScreenPage.TextReplyOption;
      e.Handled = true;
    }

    private void AddParticipant_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel.CanInviteNewParticipant)
      {
        int addToCallPromptCount = NonDbSettings.AddParticipantToCallPromptCount;
        Func<string, IObservable<bool>> confirmSelectionFunc = (Func<string, IObservable<bool>>) (jid => addToCallPromptCount < 5 ? Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
        {
          UIUtils.Decision(string.Format(AppResources.CallScreenAddParticipantConfirmation, (object) (UserCache.Get(jid, false)?.GetDisplayName() ?? "")), AppResources.Add, AppResources.CancelButton).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
          {
            if (!confirmed)
              return;
            if (!Voip.Instance.ParticipantWasInvited())
              ++NonDbSettings.AddParticipantToCallPromptCount;
            BlockContact.PromptUnblockIfBlocked(jid).Subscribe<bool>((Action<bool>) (notBlocked =>
            {
              if (!notBlocked)
                return;
              observer.OnNext(true);
            }));
          }));
          return (Action) (() => { });
        })) : Observable.Create<bool>((Func<IObserver<bool>, Action>) (observer =>
        {
          BlockContact.PromptUnblockIfBlocked(jid).Subscribe<bool>((Action<bool>) (notBlocked =>
          {
            if (!notBlocked)
              return;
            observer.OnNext(true);
          }));
          return (Action) (() => { });
        })));
        ListTabData[] tabs = new ListTabData[1];
        WaCallableContactsListTabData contactsListTabData = new WaCallableContactsListTabData();
        contactsListTabData.EnableCache = true;
        contactsListTabData.ItemVisibleFilter = (Func<JidItemViewModel, bool>) (item => !this.viewModel.usersJids.Contains(item.Jid));
        contactsListTabData.Header = (string) null;
        tabs[0] = (ListTabData) contactsListTabData;
        JidItemPickerPage.Start(tabs, AppResources.CallScreenAddParticipant, confirmSelectionFunc).ObserveOnDispatcher<List<string>>().Subscribe<List<string>>((Action<List<string>>) (selJids =>
        {
          try
          {
            string PeerJid = selJids.FirstOrDefault<string>();
            if (PeerJid == null)
              return;
            Voip.Instance.InviteParticipant(PeerJid);
            this.NavigationService.JumpBackTo(nameof (CallScreenPage));
          }
          catch
          {
          }
        }));
      }
      e.Handled = true;
    }

    private void CancelCallButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      try
      {
        CallScreenViewModel.Peer peer;
        switch ((VisualTreeHelper.GetParent((DependencyObject) (sender as Button)) as Grid).Name)
        {
          case "PeerTwo":
            peer = CallScreenViewModel.Peer.Two;
            break;
          case "PeerThree":
            peer = CallScreenViewModel.Peer.Three;
            break;
          default:
            return;
        }
        string PeerJid = this.viewModel.usersJids.ElementAtOrDefault<string>((int) (peer - 1));
        if (PeerJid != null)
          Voip.Instance.CancelInviteParticipant(PeerJid);
      }
      catch
      {
      }
      e.Handled = true;
    }

    private void Voip_CameraRestarted(bool completed)
    {
      if (!this.viewModel.VideoEnabled)
        return;
      if (!completed)
      {
        ManualResetEvent ev = new ManualResetEvent(false);
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.previewVideo.SetMediaPlayerState(UiVideoState.None, true);
          this.viewModel.CameraRestarting = true;
          this.Dispatcher.BeginInvoke((Action) (() => ev.Set()));
        }));
        ev.WaitOne();
      }
      else
        this.Dispatcher.BeginInvoke((Action) (() =>
        {
          this.viewModel.CameraRestarting = false;
          this.previewVideo.SetMediaPlayerState(UiVideoState.Playing, true, this.viewModel.LocalCameraSource);
        }));
    }

    private void Voip_CallEnded(WaCallEndedEventArgs args)
    {
      this.EnableOrientationSensor = false;
      this.callEndSub.SafeDispose();
      this.callEndSub = (IDisposable) null;
      this.cameraRestartSub.SafeDispose();
      this.cameraRestartSub = (IDisposable) null;
      this.videoRestartedSub.SafeDispose();
      this.videoRestartedSub = (IDisposable) null;
      Log.l("callscreen", "voip event | call ended | reason:{0},should rate:{1}", (object) args.Reason, (object) args.ShouldRateCall);
      this.viewModel.EndReason = args.Reason;
      if (this.viewModel.CallState == UiCallState.Calling && args.Reason == CallEndReason.Unknown && !this.endedBySelf)
      {
        this.viewModel.CallState = UiCallState.Voicemail;
      }
      else
      {
        this.viewModel.CallState = UiCallState.Ending;
        this.previewVideo.SetMediaPlayerState(UiVideoState.None, true);
        this.remoteVideo.SetMediaPlayerState(UiVideoState.None, true);
        if (this.ignoredWithTextReply)
          return;
        bool hasError = false;
        CallScreenHelper.ProcessCallEndReason(args.PeerJid, args.Reason, this.viewModel.VideoEnabled, out hasError);
        if (this.pageRemoved)
        {
          Log.l("callscreen", "voip event | call ended | page removed already");
          if (!args.ShouldRateCall || hasError || this.ratingsPageNavigated)
            return;
          this.ratingsPageNavigated = true;
          CallRatingPage.Start(args.PeerJid, args.RatingCookie, true, false);
        }
        else
          this.ScheduleClose(1000, args.ShouldRateCall && !hasError ? CallScreenPage.CloseOption.ToRating : CallScreenPage.CloseOption.None, "call ended", args.RatingCookie);
      }
    }

    private bool EnableOrientationSensor
    {
      set
      {
        if (value && this.orientationSensor == null)
        {
          this.orientationSensor = SimpleOrientationSensor.GetDefault();
          if (this.orientationSensor != null)
          {
            SimpleOrientationSensor orientationSensor = this.orientationSensor;
            WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>>(new Func<TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>, EventRegistrationToken>(orientationSensor.add_OrientationChanged), new Action<EventRegistrationToken>(orientationSensor.remove_OrientationChanged), new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>((object) this, __methodptr(OrientationSensor_OrientationChanged)));
          }
        }
        if (value || this.orientationSensor == null)
          return;
        WindowsRuntimeMarshal.RemoveEventHandler<TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>>(new Action<EventRegistrationToken>(this.orientationSensor.remove_OrientationChanged), new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>((object) this, __methodptr(OrientationSensor_OrientationChanged)));
        this.orientationSensor = (SimpleOrientationSensor) null;
      }
    }

    private void OrientationSensor_OrientationChanged(
      SimpleOrientationSensor sender,
      SimpleOrientationSensorOrientationChangedEventArgs args)
    {
      ScreenOrientation orientation;
      switch ((int) args.Orientation)
      {
        case 0:
          orientation = ScreenOrientation.Portrait;
          break;
        case 1:
          orientation = ScreenOrientation.LandscapeLeft;
          break;
        case 2:
          orientation = ScreenOrientation.PortraitUpsideDown;
          break;
        case 3:
          orientation = ScreenOrientation.LandscapeRight;
          break;
        case 4:
          return;
        case 5:
          return;
        default:
          return;
      }
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          Voip.Instance.OrientationChanged(orientation);
        }
        catch
        {
        }
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/CallScreenPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.remoteVideoGrid = (Grid) this.FindName("remoteVideoGrid");
      this.remoteScale = (ScaleTransform) this.FindName("remoteScale");
      this.remoteTranslation = (TranslateTransform) this.FindName("remoteTranslation");
      this.remoteVideo = (MediaElement) this.FindName("remoteVideo");
      this.previewVideoGrid = (Grid) this.FindName("previewVideoGrid");
      this.previewTranslation = (TranslateTransform) this.FindName("previewTranslation");
      this.previewScale = (ScaleTransform) this.FindName("previewScale");
      this.previewVideo = (MediaElement) this.FindName("previewVideo");
      this.previewVideoIconGrid = (Grid) this.FindName("previewVideoIconGrid");
      this.previewIconTranslation = (TranslateTransform) this.FindName("previewIconTranslation");
      this.UpperGradient = (Rectangle) this.FindName("UpperGradient");
      this.LogoPanel = (StackPanel) this.FindName("LogoPanel");
      this.WhatsAppIcon = (Image) this.FindName("WhatsAppIcon");
      this.TitleBlock = (TextBlock) this.FindName("TitleBlock");
      this.PeerProfile = (ZoomBox) this.FindName("PeerProfile");
      this.PeerOne = (Grid) this.FindName("PeerOne");
      this.MuteIconOverlayOne = (Ellipse) this.FindName("MuteIconOverlayOne");
      this.AvatarOne = (ImageBrush) this.FindName("AvatarOne");
      this.StateAndDurationBlockOne = (TextBlock) this.FindName("StateAndDurationBlockOne");
      this.PeerTwo = (Grid) this.FindName("PeerTwo");
      this.MuteIconOverlayTwo = (Ellipse) this.FindName("MuteIconOverlayTwo");
      this.AvatarTwo = (ImageBrush) this.FindName("AvatarTwo");
      this.StateAndDurationBlockTwo = (TextBlock) this.FindName("StateAndDurationBlockTwo");
      this.CancelCallButtonTwo = (Button) this.FindName("CancelCallButtonTwo");
      this.PeerThree = (Grid) this.FindName("PeerThree");
      this.MuteIconOverlayThree = (Ellipse) this.FindName("MuteIconOverlayThree");
      this.AvatarThree = (ImageBrush) this.FindName("AvatarThree");
      this.StateAndDurationBlockThree = (TextBlock) this.FindName("StateAndDurationBlockThree");
      this.CancelCallButtonThree = (Button) this.FindName("CancelCallButtonThree");
      this.IncomingPeerProfileOne = (ZoomBox) this.FindName("IncomingPeerProfileOne");
      this.IncomingPeerProfileTwo = (ZoomBox) this.FindName("IncomingPeerProfileTwo");
      this.IncomingPeerProfileThree = (ZoomBox) this.FindName("IncomingPeerProfileThree");
      this.videoPausedOverlay = (ZoomBox) this.FindName("videoPausedOverlay");
      this.videoInfoButton = (TextBlock) this.FindName("videoInfoButton");
      this.videoInfoPanel = (StackPanel) this.FindName("videoInfoPanel");
      this.CallButtons = (Grid) this.FindName("CallButtons");
      this.CallIncomingButtons = (Grid) this.FindName("CallIncomingButtons");
      this.AnswerButton = (Button) this.FindName("AnswerButton");
      this.TextReplyButton = (Button) this.FindName("TextReplyButton");
      this.IgnoreButton = (Button) this.FindName("IgnoreButton");
      this.CallInProgressButtons = (Grid) this.FindName("CallInProgressButtons");
      this.MessageButton = (Button) this.FindName("MessageButton");
      this.SpeakerButton = (Button) this.FindName("SpeakerButton");
      this.SwitchCameraButton = (Button) this.FindName("SwitchCameraButton");
      this.MuteButton = (Button) this.FindName("MuteButton");
      this.UpgradeButton = (Button) this.FindName("UpgradeButton");
      this.DowngradeButton = (Button) this.FindName("DowngradeButton");
      this.BluetoothButton = (Button) this.FindName("BluetoothButton");
      this.AddParticipantButton = (Button) this.FindName("AddParticipantButton");
      this.EndCallButton = (Button) this.FindName("EndCallButton");
      this.CallUpgradeButtons = (Grid) this.FindName("CallUpgradeButtons");
      this.UpgradeConfirmButton = (Button) this.FindName("UpgradeConfirmButton");
      this.UpgradeDenyButton = (Button) this.FindName("UpgradeDenyButton");
      this.CallEndedPanel = (Grid) this.FindName("CallEndedPanel");
      this.CancelButton = (Button) this.FindName("CancelButton");
      this.RedialButton = (Button) this.FindName("RedialButton");
      this.UpgradeRequestOverlay = (ZoomBox) this.FindName("UpgradeRequestOverlay");
      this.CancelUpgradeButton = (Border) this.FindName("CancelUpgradeButton");
      this.MessagePicker = (ListPicker) this.FindName("MessagePicker");
    }

    public class TextReplyOption
    {
      private Action onSelected;
      private Action navAfter;

      public string Label { get; private set; }

      public TextReplyOption(string label, Action onSelected, Action navigationAfter = null)
      {
        this.Label = label;
        this.onSelected = onSelected;
        this.navAfter = navigationAfter;
      }

      public void Act()
      {
        if (this.onSelected == null)
          return;
        this.onSelected();
      }

      public bool TryNavigation()
      {
        if (this.navAfter == null)
          return false;
        this.navAfter();
        return true;
      }
    }

    private enum CloseOption
    {
      None,
      BackToChat,
      ToRating,
    }

    [Flags]
    private enum PreviewLocation
    {
      Top = 1,
      Left = 2,
    }
  }
}
