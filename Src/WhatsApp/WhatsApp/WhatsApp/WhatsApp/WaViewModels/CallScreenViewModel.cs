// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaViewModels.CallScreenViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsAppNative;


namespace WhatsApp.WaViewModels
{
  public class CallScreenViewModel : PageViewModelBase
  {
    private const string LogHeader = "callscreen";
    private CallScreenHelper helper;
    private IDisposable videoStateSub;
    private IDisposable videoOrientationSub;
    private IDisposable peerMutedSub;
    private IDisposable offerAckReceivedSub;
    private IDisposable callStateSub;
    private IDisposable groupInfoSub;
    private IDisposable groupMutedSub;
    private UiVideoState localVideoState;
    private UiVideoState remoteVideoState;
    private bool? videoEnabled;
    private bool peerMuted;
    private List<CallParticipantDetail> peersDetails = new List<CallParticipantDetail>();
    private List<UserStatus> users = new List<UserStatus>();
    private string callId;
    private UiCallState callState;
    private IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState> peerCallStates = (IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>) new Dictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>();
    private UiUpgradeState upgradeState;
    private CallEndReason endReason;
    private IDisposable audioRoutingSub;
    private WaAudioRouting.Endpoint lastKnownAudioOutput = WaAudioRouting.Endpoint.Undefined;
    private bool isMuted;
    private IDictionary<CallScreenViewModel.Peer, IDisposable> picSubs = (IDictionary<CallScreenViewModel.Peer, IDisposable>) new Dictionary<CallScreenViewModel.Peer, IDisposable>();
    private IDictionary<string, System.Windows.Media.ImageSource> userPicCache = (IDictionary<string, System.Windows.Media.ImageSource>) new Dictionary<string, System.Windows.Media.ImageSource>();
    private int? duration;
    private IDisposable timerSub;
    private string debugText;
    private bool videoInfoVisible;
    public IDisposable videoInfoSub;
    private double remoteVideoRotation = 90.0;
    private double localVideoRotation = -90.0;
    private Visibility previewVisibility;
    private bool audioVideoSwitchEnabled;
    private bool cameraRestarting;
    private CameraLocation localCamera;
    private Size renderSize = new Size(1.0, 1.0);
    private System.Windows.Media.ImageSource speakerButtonIconOn;
    private System.Windows.Media.ImageSource speakerButtonIconOff;
    private System.Windows.Media.ImageSource muteButtonIconUnMuted;
    private System.Windows.Media.ImageSource muteButtonIconMuted;
    private System.Windows.Media.ImageSource muteButtonIconOverlay;
    private BitmapSource bluetoothIcon;
    private BitmapSource addParticipantButtonIcon;
    private BitmapSource messageButtonIcon;
    private BitmapSource switchCameraIcon;
    private BitmapSource answerIcon;
    private BitmapSource ignoreIcon;
    private BitmapSource endCallIcon;
    private BitmapSource cancelInvitationIcon;
    private BitmapSource addVideoIcon;
    private BitmapSource removeVideoIcon;
    private int cameraCount = 1;
    public BehaviorSubject<UiVideoState?> LocalVideoStateChangedSubject = new BehaviorSubject<UiVideoState?>(new UiVideoState?());
    public BehaviorSubject<UiVideoState?> RemoteVideoStateChangedSubject = new BehaviorSubject<UiVideoState?>(new UiVideoState?());

    public List<CallParticipantDetail> PeersDetails => this.peersDetails;

    public List<string> usersJids
    {
      get
      {
        return this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToList<string>();
      }
    }

    public UiCallState CallState
    {
      get => this.callState;
      set
      {
        if (this.callState == value)
          return;
        this.callState = value;
        this.UpdatePeersCallStates();
        this.Notify("e:UICallStateChanged");
        this.UpdateAddParticipantButtonOpacity();
        this.UpdateAddParticipantButtonVisibility();
        this.UpdateEnableVideoOpacity();
      }
    }

    public IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState> PeerCallStates
    {
      get => this.peerCallStates;
      set
      {
        if (value.OrderBy<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>, CallScreenViewModel.Peer>((Func<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>, CallScreenViewModel.Peer>) (kvp => kvp.Key)).SequenceEqual<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>>((IEnumerable<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>>) this.peerCallStates.OrderBy<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>, CallScreenViewModel.Peer>((Func<KeyValuePair<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>, CallScreenViewModel.Peer>) (kvp => kvp.Key))))
          return;
        this.peerCallStates = value;
        this.UpdateParticipantsProperties();
        this.Notify("e:PeersChanged");
        this.NotifyPropertyChanged("ParticipantIconRadius");
        this.NotifyPropertyChanged("ParticipantInterval");
      }
    }

    internal void UpdatePeersCallStates()
    {
      if (this.callState == UiCallState.Ending || this.callState == UiCallState.None)
        return;
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          this.peersDetails = voip.GetCallPeers();
          Dictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState> dictionary = new Dictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>();
          Array values = Enum.GetValues(typeof (CallScreenViewModel.Peer));
          for (int index = 0; index < this.peersDetails.Count; ++index)
          {
            CallParticipantDetail peersDetail = this.peersDetails[index];
            object key = values.Length > index ? values.GetValue(index) : (object) null;
            if (key != null)
            {
              UiCallParticipantState state = WhatsApp.Voip.TranslateCallParticipantState(peersDetail);
              dictionary[(CallScreenViewModel.Peer) key] = new CallScreenViewModel.PeerCallState(state, peersDetail.IsMuted);
            }
          }
          this.PeerCallStates = (IDictionary<CallScreenViewModel.Peer, CallScreenViewModel.PeerCallState>) dictionary;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, nameof (UpdatePeersCallStates), false);
        }
      }));
    }

    public UiUpgradeState UpgradeState
    {
      get => this.upgradeState;
      set
      {
        if (this.upgradeState == value)
          return;
        this.upgradeState = value;
        this.Notify("e:UIUpgradeStateChanged");
        this.Notify("e:PeersChanged");
        this.NotifyPropertyChanged("UpgradeRequestOverlayVisibility");
        this.NotifyPropertyChanged("UpgradeOverlayString");
        this.NotifyPropertyChanged("VideoOverlayVisibility");
        if (value != UiUpgradeState.RequestedBySelf || WaAudioRouting.GetCurrentAudioEndpoint() != WaAudioRouting.Endpoint.Earpiece)
          return;
        WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Speaker);
      }
    }

    public CallEndReason EndReason
    {
      get => this.endReason;
      set
      {
        if (this.endReason == value)
          return;
        this.endReason = value;
        this.Notify("e:UICallStateChanged");
        this.UpdatePeersCallStates();
      }
    }

    public bool IsMuted
    {
      set
      {
        this.isMuted = value;
        this.NotifyPropertyChanged("MuteButtonBackgroundBrush");
        this.NotifyPropertyChanged("MuteButtonForegroundBrush");
        this.NotifyPropertyChanged("MuteButtonIcon");
        this.NotifyPropertyChanged("PreviewIconVisibility");
      }
    }

    public Visibility DebugPanelVisibility => Visibility.Collapsed;

    public string DebugText
    {
      get
      {
        LinkedList<string> values = new LinkedList<string>();
        if (this.debugText != null)
          values.AddLast(this.debugText);
        values.AddLast(this.CallState.ToString());
        return string.Join(" | ", (IEnumerable<string>) values);
      }
      set
      {
        this.debugText = value;
        this.NotifyPropertyChanged(nameof (DebugText));
      }
    }

    public Visibility VideoInfoButtonVisibility
    {
      get => (Settings.IsWaAdmin && !this.videoInfoVisible).ToVisibility();
    }

    public Visibility VideoInfoTextVisibility
    {
      get => (Settings.IsWaAdmin && this.videoInfoVisible).ToVisibility();
    }

    public string VideoInfoText { get; set; }

    public double RemoteVideoRotation
    {
      get => this.remoteVideoRotation;
      set
      {
        if (this.remoteVideoRotation == value)
          return;
        this.remoteVideoRotation = value;
        this.NotifyPropertyChanged(nameof (RemoteVideoRotation));
        this.NotifyPropertyChanged("RemoteVideoScale");
      }
    }

    public double LocalVideoRotation
    {
      get => this.localVideoRotation;
      set
      {
        if (this.localVideoRotation == value)
          return;
        this.localVideoRotation = value;
        this.NotifyPropertyChanged(nameof (LocalVideoRotation));
        this.NotifyPropertyChanged("LocalVideoScale");
      }
    }

    public Visibility PreviewVisibility
    {
      get => this.VideoEnabled ? this.previewVisibility : this.VideoVisibility;
      set
      {
        if (value == this.PreviewVisibility)
          return;
        this.previewVisibility = value;
        this.NotifyPropertyChanged(nameof (PreviewVisibility));
        this.NotifyPropertyChanged("PreviewIconVisibility");
      }
    }

    public Visibility PreviewIconVisibility
    {
      get => this.isMuted && this.VideoEnabled ? this.previewVisibility : Visibility.Collapsed;
    }

    public Visibility EnableVideoVisibility
    {
      get
      {
        return (Settings.AudioVideoSwitchEnabled && (!this.VideoEnabled || this.localVideoState == UiVideoState.Stopped)).ToVisibility();
      }
    }

    public double EnableVideoOpacity { get; private set; }

    private void UpdateEnableVideoOpacity()
    {
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          CallInfoStruct? callInfo = voip.GetCallInfo();
          this.EnableVideoOpacity = !callInfo.HasValue || callInfo.Value.CallState != WhatsAppNative.CallState.CallActive || this.peersDetails.Count >= 2 ? 0.3 : 1.0;
        }
        catch (Exception ex)
        {
          this.EnableVideoOpacity = 0.3;
          Log.LogException(ex, nameof (UpdateEnableVideoOpacity), false);
        }
      }), (Action) (() => this.NotifyPropertyChanged("EnableVideoOpacity")));
    }

    public Visibility DisableVideoVisibility
    {
      get => (Settings.AudioVideoSwitchEnabled && this.EnableVideoVisibility != 0).ToVisibility();
    }

    public bool VideoEnabled
    {
      get => this.videoEnabled.HasValue && this.videoEnabled.Value;
      set
      {
        bool hasValue = this.videoEnabled.HasValue;
        int num1 = value ? 1 : 0;
        bool? videoEnabled = this.videoEnabled;
        int num2 = videoEnabled.GetValueOrDefault() ? 1 : 0;
        if ((num1 == num2 ? (!videoEnabled.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.videoEnabled = new bool?(value);
        if (hasValue)
        {
          List<string> list = this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToList<string>();
          if (NonDbSettings.SendCallStateForVideoEnabled)
            CallScreenPage.Launch(list, new UiCallState?(this.CallState), true, value);
          else
            CallScreenPage.Launch(list, new UiCallState?(), true, value);
        }
        else
        {
          this.Notify("e:VideoEnabled");
          this.NotifyPropertyChanged("VideoVisibility");
          this.NotifyPropertyChanged("NonVideoVisibility");
          this.NotifyPropertyChanged("PreviewVisibility");
          this.NotifyPropertyChanged("EnableVideoVisibility");
          this.NotifyPropertyChanged("DisableVideoVisibility");
          this.NotifyPropertyChanged("TextForegroundBrush");
        }
      }
    }

    public Uri LocalCameraSource
    {
      get
      {
        if (this.localVideoState == UiVideoState.Starting)
          return (Uri) null;
        return this.cameraRestarting ? (Uri) null : new Uri(this.localCamera == CameraLocation.Front ? "ms-media-stream-id:camera-FrontFacing" : "ms-media-stream-id:camera-RearFacing");
      }
    }

    public bool CameraRestarting
    {
      set
      {
        if (value == this.cameraRestarting)
          return;
        this.cameraRestarting = value;
        this.NotifyPropertyChanged("LocalCameraSource");
      }
    }

    public FlowDirection PreviewMirror
    {
      get
      {
        return this.localCamera != CameraLocation.Front ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;
      }
    }

    public double RemoteVideoScale
    {
      get
      {
        return this.RemoteVideoRotation == 0.0 || this.RemoteVideoRotation == 180.0 ? 1.0 : this.renderSize.Height / this.renderSize.Width;
      }
    }

    public double LocalVideoScale
    {
      get
      {
        return this.LocalVideoRotation == 0.0 || this.LocalVideoRotation == 180.0 ? 1.0 : this.renderSize.Height / this.renderSize.Width;
      }
    }

    public Size RenderWindowSize
    {
      set
      {
        this.renderSize = value;
        this.NotifyPropertyChanged("LocalVideoScale");
        this.NotifyPropertyChanged("RemoteVideoScale");
      }
    }

    public Visibility VideoVisibility
    {
      get => !this.VideoEnabled ? Visibility.Collapsed : Visibility.Visible;
    }

    public Visibility NonVideoVisibility
    {
      get => !this.VideoEnabled ? Visibility.Visible : Visibility.Collapsed;
    }

    public System.Windows.Media.ImageSource PictureSourceOne
    {
      get => this.PictureSource(CallScreenViewModel.Peer.One);
    }

    public System.Windows.Media.ImageSource PictureSourceTwo
    {
      get => this.PictureSource(CallScreenViewModel.Peer.Two);
    }

    public System.Windows.Media.ImageSource PictureSourceThree
    {
      get => this.PictureSource(CallScreenViewModel.Peer.Three);
    }

    private System.Windows.Media.ImageSource PictureSource(CallScreenViewModel.Peer peer)
    {
      UserStatus user = this.users.ElementAtOrDefault<UserStatus>((int) (peer - 1));
      if (user != null && this.userPicCache.ContainsKey(user.Jid))
        return this.userPicCache[user.Jid];
      if ((!this.picSubs.ContainsKey(peer) || this.picSubs[peer] == null) && user != null)
        this.picSubs[peer] = ChatPictureStore.Get(user.Jid, true, false, true).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (args =>
        {
          this.userPicCache[user.Jid] = args == null || args.Image == null ? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIconLarge : (System.Windows.Media.ImageSource) args.Image;
          this.NotifyPropertyChanged(nameof (PictureSource) + peer.ToString());
        }));
      return (System.Windows.Media.ImageSource) null;
    }

    public string IncomingCallString => AppResources.CallScreenLabelIncomingCall;

    public string IncomingGroupCallString => AppResources.CallScreenLabelIncomingGroupCall;

    public string AnswerButtonStr => AppResources.CallScreenButtonAnswer;

    public string IgnoreButtonStr => AppResources.CallScreenButtonIgnore;

    public string MessageButtonStr => AppResources.CallScreenButtonMessage;

    public string UpgradeButtonStr => "Upgrade";

    public string TextReplyButtonStr => AppResources.CallScreenButtonTextReply;

    public Visibility MessageVisibility => (!Settings.AudioVideoSwitchEnabled).ToVisibility();

    public Visibility UpgradeVisibility => Settings.AudioVideoSwitchEnabled.ToVisibility();

    public System.Windows.Media.ImageSource SpeakerButtonIcon
    {
      get
      {
        return this.lastKnownAudioOutput == WaAudioRouting.Endpoint.Speaker ? this.speakerButtonIconOn ?? (this.speakerButtonIconOn = (System.Windows.Media.ImageSource) AssetStore.CallScreenSpeakerIconWhite) : this.speakerButtonIconOff ?? (this.speakerButtonIconOff = this.VideoEnabled ? (System.Windows.Media.ImageSource) AssetStore.CallScreenSpeakerIconWhite : (System.Windows.Media.ImageSource) AssetStore.CallScreenSpeakerIcon);
      }
    }

    public System.Windows.Media.ImageSource MuteButtonIcon
    {
      get
      {
        return this.isMuted ? this.muteButtonIconMuted ?? (this.muteButtonIconMuted = (System.Windows.Media.ImageSource) AssetStore.CallScreenMuteIconWhite) : this.muteButtonIconUnMuted ?? (this.muteButtonIconUnMuted = this.VideoEnabled ? (System.Windows.Media.ImageSource) AssetStore.CallScreenMuteIconWhite : (System.Windows.Media.ImageSource) AssetStore.CallScreenMuteIcon);
      }
    }

    public System.Windows.Media.ImageSource MuteButtonIconOverlay
    {
      get
      {
        return this.muteButtonIconOverlay ?? (this.muteButtonIconOverlay = (System.Windows.Media.ImageSource) AssetStore.CallScreenMuteIcon);
      }
    }

    public System.Windows.Media.ImageSource BluetoothButtonIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.bluetoothIcon ?? (System.Windows.Media.ImageSource) (this.bluetoothIcon = this.VideoEnabled ? AssetStore.CallScreenBluetoothIconWhite : AssetStore.CallScreenBluetoothIcon);
      }
    }

    public System.Windows.Media.ImageSource AddParticipantButtonIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.addParticipantButtonIcon ?? (System.Windows.Media.ImageSource) (this.addParticipantButtonIcon = this.VideoEnabled ? AssetStore.CallScreenAddIconWhite : AssetStore.CallScreenAddIcon);
      }
    }

    public System.Windows.Media.ImageSource MessageButtonIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.messageButtonIcon ?? (System.Windows.Media.ImageSource) (this.messageButtonIcon = this.VideoEnabled ? AssetStore.CallScreenMessageIconWhite : AssetStore.CallScreenMessageIcon);
      }
    }

    public System.Windows.Media.ImageSource SwitchCameraIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.switchCameraIcon ?? (System.Windows.Media.ImageSource) (this.switchCameraIcon = this.VideoEnabled ? AssetStore.CallScreenCameraIconWhite : AssetStore.CallScreenCameraIcon);
      }
    }

    public System.Windows.Media.ImageSource AnswerIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.answerIcon ?? (System.Windows.Media.ImageSource) (this.answerIcon = this.VideoEnabled ? AssetStore.CallScreenAnswerIconWhite : AssetStore.CallScreenAnswerIcon);
      }
    }

    public System.Windows.Media.ImageSource IgnoreIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.ignoreIcon ?? (System.Windows.Media.ImageSource) (this.ignoreIcon = this.VideoEnabled ? AssetStore.CallScreenDeclineIconWhite : AssetStore.CallScreenDeclineIcon);
      }
    }

    public System.Windows.Media.ImageSource EndCallIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.endCallIcon ?? (System.Windows.Media.ImageSource) (this.endCallIcon = this.VideoEnabled ? AssetStore.CallScreenEndCallIconWhite : AssetStore.CallScreenEndCallIcon);
      }
    }

    public System.Windows.Media.ImageSource CancelInvitationIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.cancelInvitationIcon ?? (System.Windows.Media.ImageSource) (this.cancelInvitationIcon = this.VideoEnabled ? AssetStore.CallScreenCancelInvitationIconWhite : AssetStore.CallScreenCancelInvitationIcon);
      }
    }

    public System.Windows.Media.ImageSource AddVideoIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.addVideoIcon ?? (System.Windows.Media.ImageSource) (this.addVideoIcon = this.VideoEnabled ? AssetStore.CallScreenVideoOnIconWhite : AssetStore.CallScreenVideoOnIcon);
      }
    }

    public System.Windows.Media.ImageSource RemoveVideoIcon
    {
      get
      {
        return (System.Windows.Media.ImageSource) this.removeVideoIcon ?? (System.Windows.Media.ImageSource) (this.removeVideoIcon = this.VideoEnabled ? AssetStore.CallScreenVideoOffIconWhite : AssetStore.CallScreenVideoOffIcon);
      }
    }

    public string SpeakerButtonStr => AppResources.CallScreenButtonSpeaker;

    public string SwitchCameraButtonStr => AppResources.CallScreenSwitchCamera;

    public string MuteButtonStr => AppResources.CallScreenButtonMute;

    public string BluetoothButtonStr => AppResources.CallScreenButtonBluetooth;

    public string AddParticipantButtonStr => AppResources.CallScreenAddParticipant;

    public string EndCallButtonStr => AppResources.CallScreenButtonEndCall;

    public string AddVideoString
    {
      get => !this.VideoEnabled ? AppResources.MediaVideo : AppResources.CallScreenButtonVideoOn;
    }

    public string RemoveVideoString => AppResources.CallScreenButtonVideoOff;

    public Brush PageBackgroundBrush
    {
      get => !ImageStore.IsDarkTheme() ? UIUtils.PhoneChromeBrush : (Brush) UIUtils.BlackBrush;
    }

    public Thickness WhatsAppCallHeaderMargin
    {
      get
      {
        return new Thickness(24.0 * ResolutionHelper.ZoomMultiplier, UIUtils.SystemTraySizePortrait + 24.0, 0.0, 0.0);
      }
    }

    public string VideoOverlayStr
    {
      get
      {
        string displayName = this.users.Any<UserStatus>() ? this.users.First<UserStatus>().GetDisplayName(true) : (string) null;
        switch (this.remoteVideoState)
        {
          case UiVideoState.Paused:
            return string.IsNullOrEmpty(displayName) ? AppResources.CallScreenVideoPausedNoUser : string.Format(AppResources.CallScreenVideoPaused, (object) displayName);
          case UiVideoState.Interrupted:
            return AppResources.VideoCallPoorConnection;
          case UiVideoState.Stopped:
            return string.IsNullOrEmpty(displayName) ? AppResources.CallScreenPeerVideoPausedNoUser : string.Format(AppResources.CallScreenPeerVideoPaused, (object) displayName);
          default:
            if (this.peerMuted)
            {
              bool? videoEnabled = this.videoEnabled;
              bool flag = true;
              if ((videoEnabled.GetValueOrDefault() == flag ? (videoEnabled.HasValue ? 1 : 0) : 0) != 0)
                return string.IsNullOrEmpty(displayName) ? AppResources.CallScreenVideoPeerMutedNoUser : string.Format(AppResources.CallScreenVideoPeerMuted, (object) displayName);
            }
            return "";
        }
      }
    }

    public string UpgradeOverlayString
    {
      get
      {
        switch (this.upgradeState)
        {
          case UiUpgradeState.None:
            return "";
          case UiUpgradeState.RequestedByPeer:
            return string.Format(AppResources.CallPeerRequestedUpgrade, this.users.Any<UserStatus>() ? (object) this.users.First<UserStatus>().GetDisplayName(true) : (object) (string) null);
          case UiUpgradeState.RequestedBySelf:
            return AppResources.CallSelfRequestedUpgrade;
          default:
            return "";
        }
      }
    }

    public string NameStrIncoming
    {
      get
      {
        return !this.users.Any<UserStatus>() ? (string) null : Utils.CommaSeparate(this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName(true))));
      }
    }

    public string NameStrOne => this.UserName(CallScreenViewModel.Peer.One);

    public string NameStrTwo => this.UserName(CallScreenViewModel.Peer.Two);

    public string NameStrThree => this.UserName(CallScreenViewModel.Peer.Three);

    private string UserName(CallScreenViewModel.Peer peer)
    {
      return this.users.ElementAtOrDefault<UserStatus>((int) (peer - 1))?.GetDisplayName();
    }

    public string PhoneNumberStr
    {
      get
      {
        return !this.users.Any<UserStatus>() || !this.users.First<UserStatus>().IsInDeviceContactList ? (string) null : JidHelper.GetPhoneNumber(this.users.First<UserStatus>().Jid, true);
      }
    }

    public string DurationStr
    {
      get
      {
        return !this.duration.HasValue ? (string) null : DateTimeUtils.FormatDuration(this.duration.Value);
      }
    }

    internal string StateAndDuration(CallScreenViewModel.Peer peer)
    {
      string str = (string) null;
      CallScreenViewModel.PeerCallState peerCallState;
      if (this.peerCallStates.TryGetValue(peer, out peerCallState))
      {
        switch (peerCallState.State)
        {
          case UiCallParticipantState.Calling:
            str = AppResources.CallScreenLabelDialing;
            break;
          case UiCallParticipantState.Ringing:
            str = AppResources.CallScreenLabelRinging;
            break;
          case UiCallParticipantState.ReceivedCall:
            str = AppResources.CallScreenLabelIncomingCall;
            break;
          case UiCallParticipantState.Active:
            str = !peerCallState.Muted || this.VideoEnabled ? (peer != CallScreenViewModel.Peer.One ? " " : this.DurationStr) : AppResources.CallScreenLabelMuted;
            break;
          case UiCallParticipantState.Busy:
            str = string.Format(AppResources.CallScreenLabelContactIsOnAnotherCall, (object) Utils.CommaSeparate(this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName(true)))));
            break;
          case UiCallParticipantState.Connecting:
            str = AppResources.CallScreenLabelConnecting;
            break;
          case UiCallParticipantState.Reconnecting:
            str = AppResources.CallScreenLabelReconnecting;
            break;
          case UiCallParticipantState.Voicemail:
          case UiCallParticipantState.Ending:
            switch (this.EndReason)
            {
              case CallEndReason.Rejected:
                str = AppResources.CallScreenLabelRejected;
                break;
              case CallEndReason.PeerUnavailable:
                str = AppResources.CallScreenLabelUnavailable;
                break;
              default:
                str = AppResources.CallScreenLabelEnded;
                break;
            }
            break;
        }
      }
      return str;
    }

    internal FontFamily StateAndDurationFontFamily(CallScreenViewModel.Peer peer)
    {
      return Application.Current.Resources[(object) "PhoneFontFamilySemiBold"] as FontFamily;
    }

    internal Brush StateAndDurationBrush(CallScreenViewModel.Peer peer)
    {
      CallScreenViewModel.PeerCallState peerCallState;
      return !this.peerCallStates.TryGetValue(peer, out peerCallState) || peerCallState.State != UiCallParticipantState.Active || peerCallState.Muted && ((int) this.videoEnabled ?? 0) == 0 ? (Brush) UIUtils.AccentBrush : this.TextForegroundBrush;
    }

    internal bool PeerMuted(CallScreenViewModel.Peer peer)
    {
      CallScreenViewModel.PeerCallState peerCallState;
      return this.peerCallStates.TryGetValue(peer, out peerCallState) && peerCallState.Muted;
    }

    public Brush SpeakerButtonForegroundBrush
    {
      get
      {
        return this.lastKnownAudioOutput != WaAudioRouting.Endpoint.Speaker ? this.TextForegroundBrush : (Brush) UIUtils.WhiteBrush;
      }
    }

    public Brush SpeakerButtonBackgroundBrush
    {
      get
      {
        return this.lastKnownAudioOutput != WaAudioRouting.Endpoint.Speaker ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.TranslucentAccentBrush;
      }
    }

    public Brush MuteButtonForegroundBrush
    {
      get => !this.isMuted ? this.TextForegroundBrush : (Brush) UIUtils.WhiteBrush;
    }

    public Brush MuteButtonBackgroundBrush
    {
      get
      {
        return !this.isMuted ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.TranslucentAccentBrush;
      }
    }

    public double BluetoothButtonOpacity => !WaAudioRouting.IsBluetoothAvailable() ? 0.3 : 1.0;

    public Brush BluetoothButtonForegroundBrush
    {
      get
      {
        return this.lastKnownAudioOutput != WaAudioRouting.Endpoint.Bluetooth ? this.TextForegroundBrush : (Brush) UIUtils.WhiteBrush;
      }
    }

    public Brush BluetoothButtonBackgroundBrush
    {
      get
      {
        return this.lastKnownAudioOutput != WaAudioRouting.Endpoint.Bluetooth ? (Brush) UIUtils.TransparentBrush : (Brush) UIUtils.TranslucentAccentBrush;
      }
    }

    public double AddParticipantButtonOpacity => !this.CanInviteNewParticipant ? 0.3 : 1.0;

    public bool CanInviteNewParticipant { get; private set; }

    private void UpdateAddParticipantButtonOpacity()
    {
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          CallInfoStruct? callInfo = voip.GetCallInfo();
          this.CanInviteNewParticipant = callInfo.HasValue && callInfo.Value.CanInviteNewParticipant;
        }
        catch (Exception ex)
        {
          this.CanInviteNewParticipant = false;
          Log.LogException(ex, nameof (UpdateAddParticipantButtonOpacity), false);
        }
      }), (Action) (() => this.NotifyPropertyChanged("AddParticipantButtonOpacity")));
    }

    public Visibility AddParticipantButtonVisibility { get; private set; }

    private void UpdateAddParticipantButtonVisibility()
    {
      this.helper.EnqueueVoipWork((Action<IVoip>) (voip =>
      {
        try
        {
          CallInfoStruct? callInfo = voip.GetCallInfo();
          this.AddParticipantButtonVisibility = (!this.VideoEnabled && callInfo.HasValue && callInfo.Value.EnableGroupCall).ToVisibility();
        }
        catch (Exception ex)
        {
          this.AddParticipantButtonVisibility = Visibility.Collapsed;
          Log.LogException(ex, nameof (UpdateAddParticipantButtonVisibility), false);
        }
      }), (Action) (() => this.NotifyPropertyChanged("AddParticipantButtonVisibility")));
    }

    public double MessageButtonOpacity => this.peersDetails.Count <= 1 ? 1.0 : 0.3;

    public Color GradientColor
    {
      get => !this.VideoEnabled && !ImageStore.IsDarkTheme() ? Colors.White : Colors.Black;
    }

    public double ToggleCameraOpacity => this.CameraCount <= 1 ? 0.3 : 1.0;

    public Visibility CallIncomingButtonsVisibility
    {
      get => (this.CallState == UiCallState.ReceivedCall).ToVisibility();
    }

    public Visibility CallInProgressButtonsVisibility
    {
      get
      {
        return (this.CallState != UiCallState.ReceivedCall && this.CallState != UiCallState.Ending && this.CallState != UiCallState.Voicemail && this.CallState != UiCallState.None && this.UpgradeState != UiUpgradeState.RequestedByPeer).ToVisibility();
      }
    }

    public Visibility UpgradeButtonsVisibility
    {
      get
      {
        return (this.CallState == UiCallState.Active && this.UpgradeState == UiUpgradeState.RequestedByPeer).ToVisibility();
      }
    }

    public Visibility UpgradeCancelVisibility
    {
      get
      {
        return (this.CallState == UiCallState.Active && this.UpgradeState == UiUpgradeState.RequestedBySelf).ToVisibility();
      }
    }

    public Visibility SmallProfileVisibility
    {
      get => (this.CallState != UiCallState.ReceivedCall).ToVisibility();
    }

    public Visibility CallEndedButtonsVisibility
    {
      get => (this.CallState == UiCallState.Voicemail).ToVisibility();
    }

    public Visibility UpgradeRequestOverlayVisibility => (this.upgradeState != 0).ToVisibility();

    public Visibility PeerVisibility(CallScreenViewModel.Peer peer)
    {
      return ((CallScreenViewModel.Peer) this.peersDetails.Count >= peer).ToVisibility();
    }

    public GridLength ParticipantIconRadius
    {
      get => this.peersDetails.Count >= 3 ? new GridLength(85.0) : new GridLength(105.0);
    }

    public GridLength ParticipantInterval
    {
      get => this.peersDetails.Count >= 3 ? new GridLength(20.0) : new GridLength(30.0);
    }

    public Visibility CancelCallButtonTwoVisibility
    {
      get => this.CancellCallButtonVisibility(CallScreenViewModel.Peer.Two);
    }

    public Visibility CancelCallButtonThreeVisibility
    {
      get => this.CancellCallButtonVisibility(CallScreenViewModel.Peer.Three);
    }

    private Visibility CancellCallButtonVisibility(CallScreenViewModel.Peer peer)
    {
      CallParticipantDetail participantDetail = this.peersDetails.ElementAtOrDefault<CallParticipantDetail>((int) (peer - 1));
      CallScreenViewModel.PeerCallState peerCallState;
      return (this.peerCallStates.TryGetValue(peer, out peerCallState) && participantDetail.IsInvitedBySelf && (peerCallState.State == UiCallParticipantState.Calling || peerCallState.State == UiCallParticipantState.Ringing)).ToVisibility();
    }

    public string CancelText => AppResources.CallScreenCancel;

    public string RedialText => AppResources.CallAgain;

    public System.Windows.Media.ImageSource RedialIconSource
    {
      get => (System.Windows.Media.ImageSource) AssetStore.PhoneIconWhite;
    }

    public Visibility VideoOverlayVisibility
    {
      get
      {
        if (this.UpgradeRequestOverlayVisibility == Visibility.Visible)
          return Visibility.Collapsed;
        int num;
        if (this.remoteVideoState != UiVideoState.Paused && this.remoteVideoState != UiVideoState.Stopped && this.remoteVideoState != UiVideoState.Interrupted)
        {
          if (this.peerMuted)
          {
            bool? videoEnabled = this.videoEnabled;
            bool flag = true;
            num = videoEnabled.GetValueOrDefault() == flag ? (videoEnabled.HasValue ? 1 : 0) : 0;
          }
          else
            num = 0;
        }
        else
          num = 1;
        return (num != 0).ToVisibility();
      }
    }

    public double WhatsAppIconSize => 25.0 * ResolutionHelper.ZoomMultiplier;

    public HorizontalAlignment WhatsAppLogoAlignment
    {
      get
      {
        return this.CallState != UiCallState.ReceivedCall ? HorizontalAlignment.Left : HorizontalAlignment.Center;
      }
    }

    public double TitleBlockFontSize => 23.0 * ResolutionHelper.ZoomMultiplier;

    public System.Windows.Media.ImageSource WhatsAppIconSource
    {
      get
      {
        BitmapImage iconSrc = AssetStore.LoadAsset("wa-callscreen-icon.png", AssetStore.ThemeSetting.Dark);
        if (iconSrc == null)
          return (System.Windows.Media.ImageSource) null;
        return !ImageStore.IsDarkTheme() && !this.VideoEnabled ? (System.Windows.Media.ImageSource) IconUtils.CreateColorIcon((BitmapSource) iconSrc, (Brush) UIUtils.ForegroundBrush, new double?((double) iconSrc.PixelWidth)) : (System.Windows.Media.ImageSource) iconSrc;
      }
    }

    public Brush TextForegroundBrush
    {
      get => !this.VideoEnabled ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.WhiteBrush;
    }

    public int CameraCount
    {
      get => this.cameraCount;
      set
      {
        if (value == this.cameraCount)
          return;
        this.cameraCount = value;
        this.NotifyPropertyChanged("ToggleCameraOpacity");
      }
    }

    public CallScreenViewModel(List<string> jids)
      : base(PageOrientation.Portrait)
    {
      this.helper = new CallScreenHelper();
      this.SetPeersJids(jids, true);
      this.InitVoipSubscriptions();
      this.UpdateEnableVideoOpacity();
      this.UpdateAddParticipantButtonVisibility();
      this.UpdateAddParticipantButtonOpacity();
    }

    public void SetPeersJids(List<string> jids) => this.SetPeersJids(jids, false);

    private void SetPeersJids(List<string> jids, bool init)
    {
      Log.d("callscreen", "set peers jids | {0} -> {1}", (object) string.Join(", ", (IEnumerable<string>) this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToList<string>()), (object) string.Join(", ", (IEnumerable<string>) jids));
      this.users = jids.Select<string, UserStatus>((Func<string, UserStatus>) (j => UserCache.Get(j, true))).ToList<UserStatus>();
      if (init)
        return;
      foreach (KeyValuePair<CallScreenViewModel.Peer, IDisposable> picSub in (IEnumerable<KeyValuePair<CallScreenViewModel.Peer, IDisposable>>) this.picSubs)
        picSub.Value.SafeDispose();
      this.picSubs.Clear();
      this.UpdatePeersCallStates();
      this.UpdateParticipantsProperties();
      this.NotifyPropertyChanged("VideoOverlayStr");
      this.UpdateAddParticipantButtonOpacity();
      this.UpdateEnableVideoOpacity();
      this.NotifyPropertyChanged("MessageButtonOpacity");
    }

    private void UpdateParticipantsProperties()
    {
      foreach (CallScreenViewModel.Peer peer in Enum.GetValues(typeof (CallScreenViewModel.Peer)))
      {
        this.NotifyPropertyChanged("PictureSource" + peer.ToString());
        this.NotifyPropertyChanged("NameStr" + peer.ToString());
        this.NotifyPropertyChanged("PhoneNumberStr");
        this.NotifyPropertyChanged("CancelCallButton" + peer.ToString() + "Visibility");
      }
    }

    private void InitVoipSubscriptions()
    {
      this.callStateSub = VoipHandler.CallStateChangedSubject.ObserveOnDispatcher<WaCallStateChangedArgs>().Subscribe<WaCallStateChangedArgs>(new Action<WaCallStateChangedArgs>(this.Voip_CallStateChanged));
      this.offerAckReceivedSub = VoipHandler.OfferAckReceivedSubject.ObserveOnDispatcher<bool>().Subscribe<bool>(new Action<bool>(this.Voip_OfferAckReceived));
      this.videoStateSub = VoipHandler.VideoStateChanged.ObserveOnDispatcher<WaCallVideoStateChangedArgs>().Subscribe<WaCallVideoStateChangedArgs>(new Action<WaCallVideoStateChangedArgs>(this.Voip_VideoStateChanged));
      this.videoOrientationSub = VoipHandler.VideoOrientationChanged.ObserveOnDispatcher<WaCallVideoOrientationChangedArgs>().Subscribe<WaCallVideoOrientationChangedArgs>(new Action<WaCallVideoOrientationChangedArgs>(this.Voip_VideoOrientationChanged));
      this.peerMutedSub = VoipHandler.PeerMutedSubject.ObserveOnDispatcher<bool>().Subscribe<bool>(new Action<bool>(this.Voip_PeerMutedChanged));
      this.groupInfoSub = VoipHandler.GroupInfoChanged.ObserveOnDispatcher<WAGroupCallChangedArgs>().Subscribe<WAGroupCallChangedArgs>(new Action<WAGroupCallChangedArgs>(this.Voip_GroupInfoChanged));
      this.groupMutedSub = VoipHandler.GroupStateChanged.ObserveOnDispatcher<Unit>().Subscribe<Unit>(new Action<Unit>(this.Voip_GroupStateChanged));
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.callStateSub.SafeDispose();
      this.callStateSub = (IDisposable) null;
      this.offerAckReceivedSub.SafeDispose();
      this.offerAckReceivedSub = (IDisposable) null;
      this.audioRoutingSub.SafeDispose();
      this.audioRoutingSub = (IDisposable) null;
      foreach (KeyValuePair<CallScreenViewModel.Peer, IDisposable> picSub in (IEnumerable<KeyValuePair<CallScreenViewModel.Peer, IDisposable>>) this.picSubs)
        picSub.Value.SafeDispose();
      this.picSubs.Clear();
      this.timerSub.SafeDispose();
      this.timerSub = (IDisposable) null;
      this.videoStateSub.SafeDispose();
      this.videoStateSub = (IDisposable) null;
      this.videoOrientationSub.SafeDispose();
      this.videoOrientationSub = (IDisposable) null;
      this.videoInfoSub.SafeDispose();
      this.videoInfoSub = (IDisposable) null;
      this.peerMutedSub.SafeDispose();
      this.peerMutedSub = (IDisposable) null;
      this.groupInfoSub.SafeDispose();
      this.groupInfoSub = (IDisposable) null;
      this.groupMutedSub.SafeDispose();
      this.groupMutedSub = (IDisposable) null;
    }

    public void StartAudioRoutingSubscription()
    {
      IObservable<WaAudioRouting.Endpoint> first = Observable.Return<WaAudioRouting.Endpoint>(this.lastKnownAudioOutput = WaAudioRouting.GetCurrentAudioEndpoint());
      this.audioRoutingSub.SafeDispose();
      this.audioRoutingSub = first.Concat<WaAudioRouting.Endpoint>(WaAudioRouting.GetEndpointChangedObservable()).ObserveOnDispatcher<WaAudioRouting.Endpoint>().Subscribe<WaAudioRouting.Endpoint>(new Action<WaAudioRouting.Endpoint>(this.OnAudioEndpointChanged));
    }

    public void SetDuration(int? durationInSeconds, bool ticking)
    {
      this.duration = durationInSeconds;
      if (ticking)
      {
        if (this.timerSub != null)
          return;
        this.timerSub = Observable.Timer(TimeSpan.FromSeconds(1.0)).Repeat<long>().ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
        {
          this.duration = new int?((this.duration ?? 0) + 1);
          this.Notify("r:Duration");
        }));
      }
      else
      {
        this.timerSub.SafeDispose();
        this.timerSub = (IDisposable) null;
      }
    }

    public void UpdateFromCallInfo(
      string callId,
      CallInfoStruct callInfo,
      CallParticipantDetail selfDetails,
      List<CallParticipantDetail> peersDetails)
    {
      this.audioVideoSwitchEnabled = selfDetails.IsAudioVideoSwitchEnabled && peersDetails.All<CallParticipantDetail>((Func<CallParticipantDetail, bool>) (p => p.IsAudioVideoSwitchEnabled));
      this.VideoEnabled = callInfo.VideoEnabled;
      this.CallState = WhatsApp.Voip.TranslateCallState(callInfo.CallState);
      this.callId = callId;
      if (!this.VideoEnabled)
        return;
      CallParticipantDetail peerDetails = peersDetails.FirstOrDefault<CallParticipantDetail>();
      this.UpdateVideoState(new WaCallVideoStateChangedArgs(peerDetails.Jid, callId, WhatsApp.Voip.CallInfoToLocalVideoState(callInfo, selfDetails), WhatsApp.Voip.CallInfoToRemoteVideoState(callInfo, peerDetails), WhatsApp.Voip.CallInfoToUpgradeState(selfDetails, peerDetails), callInfo.LocalCamera), true);
      this.UpdateCameraLocation(callInfo.LocalCamera, selfDetails.VideoOrientation);
      this.RemoteVideoRotation = this.VideoOrientationToDegrees(peerDetails.VideoOrientation, false);
      this.LocalVideoRotation = this.VideoOrientationToDegrees(selfDetails.VideoOrientation, true);
    }

    public void ShowVideoInfo()
    {
      if (this.videoInfoVisible)
        return;
      this.videoInfoSub = Observable.Interval(TimeSpan.FromSeconds(1.0)).StartWith<long>(new long[1]).ObserveOn<long>((IScheduler) WhatsApp.Voip.Worker).Select<long, string>((Func<long, string>) (_ => WhatsApp.Voip.Instance.GetVideoInfoString())).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (s =>
      {
        this.VideoInfoText = s;
        this.NotifyPropertyChanged("VideoInfoText");
      }));
      this.videoInfoVisible = true;
      this.NotifyPropertyChanged("VideoInfoButtonVisibility");
      this.NotifyPropertyChanged("VideoInfoTextVisibility");
    }

    public void HideVideoInfo()
    {
      if (!this.videoInfoVisible)
        return;
      this.videoInfoVisible = false;
      this.videoInfoSub.SafeDispose();
      this.videoInfoSub = (IDisposable) null;
      this.NotifyPropertyChanged("VideoInfoButtonVisibility");
      this.NotifyPropertyChanged("VideoInfoTextVisibility");
    }

    public void UpdateCameraLocation(
      CameraInformation LocalCamera,
      VideoOrientation SensorOrientation)
    {
      if (LocalCamera.SensorLocation == this.localCamera)
        return;
      this.localCamera = LocalCamera.SensorLocation;
      this.LocalVideoRotation = this.VideoOrientationToDegrees(SensorOrientation, true);
      this.NotifyPropertyChanged("LocalCameraSource");
      this.NotifyPropertyChanged("PreviewMirror");
    }

    private void OnAudioEndpointChanged(WaAudioRouting.Endpoint newEndpoint)
    {
      Log.d("callscreen", "audio output change {0} -> {1}", (object) this.lastKnownAudioOutput, (object) newEndpoint);
      this.lastKnownAudioOutput = newEndpoint;
      this.NotifyPropertyChanged("SpeakerButtonBackgroundBrush");
      this.NotifyPropertyChanged("SpeakerButtonForegroundBrush");
      this.NotifyPropertyChanged("SpeakerButtonIcon");
      this.NotifyPropertyChanged("BluetoothButtonOpacity");
      this.NotifyPropertyChanged("BluetoothButtonBackgroundBrush");
      this.NotifyPropertyChanged("BluetoothButtonForegroundBrush");
      this.NotifyPropertyChanged("BluetoothButtonIcon");
    }

    private void Voip_VideoStateChanged(WaCallVideoStateChangedArgs args)
    {
      this.UpdateVideoState(args);
    }

    private void Voip_VideoOrientationChanged(WaCallVideoOrientationChangedArgs args)
    {
      this.RemoteVideoRotation = this.VideoOrientationToDegrees(args.RemoteOrientation, false);
    }

    private void Voip_PeerMutedChanged(bool muted)
    {
      this.peerMuted = muted;
      this.NotifyPropertyChanged("VideoOverlayStr");
      this.NotifyPropertyChanged("VideoOverlayVisibility");
    }

    private void Voip_GroupInfoChanged(WAGroupCallChangedArgs args)
    {
      this.SetPeersJids(args.Peers.Select<CallParticipantDetail, string>((Func<CallParticipantDetail, string>) (p => p.Jid)).ToList<string>());
    }

    private void Voip_GroupStateChanged(Unit arg) => this.UpdatePeersCallStates();

    private void UpdateVideoState(WaCallVideoStateChangedArgs args, bool forceUpdate = false)
    {
      if (!WhatsApp.Voip.IsInCall)
        return;
      this.VideoEnabled = ((args.LocalVideoState == UiVideoState.None ? 1 : (args.LocalVideoState == UiVideoState.Stopped ? 1 : 0)) & (args.RemoteVideoState == UiVideoState.None ? (true ? 1 : 0) : (args.RemoteVideoState == UiVideoState.Stopped ? 1 : 0))) == 0;
      if (!this.VideoEnabled)
        return;
      this.UpgradeState = args.UpgradeState;
      if (forceUpdate || args.LocalVideoState != this.localVideoState)
      {
        UiVideoState localVideoState = this.localVideoState;
        this.localVideoState = args.LocalVideoState;
        this.FireLocalVideoStateChanged(localVideoState, this.localVideoState);
        if (localVideoState == UiVideoState.Starting && this.localVideoState == UiVideoState.Playing)
          this.NotifyPropertyChanged("LocalCameraSource");
        if (localVideoState == UiVideoState.Stopped || this.localVideoState == UiVideoState.Stopped)
        {
          this.NotifyPropertyChanged("AddVideoString");
          this.NotifyPropertyChanged("EnableVideoVisibility");
          this.NotifyPropertyChanged("DisableVideoVisibility");
        }
      }
      if (!forceUpdate && args.RemoteVideoState == this.remoteVideoState)
        return;
      UiVideoState remoteVideoState = this.remoteVideoState;
      this.remoteVideoState = args.RemoteVideoState;
      this.FireRemoteVideoStateChanged(remoteVideoState, this.remoteVideoState);
      this.NotifyPropertyChanged("VideoOverlayStr");
      this.NotifyPropertyChanged("VideoOverlayVisibility");
    }

    private void Voip_OfferAckReceived(bool groupCallEnabled)
    {
      Log.d("callscreen", "voip event | offer ack received with group calls enabled: {0}", (object) groupCallEnabled);
      this.UpdateAddParticipantButtonVisibility();
    }

    private void Voip_CallStateChanged(WaCallStateChangedArgs args)
    {
      Log.d("callscreen", "voip event | state changed:{0}->{1},jid:{2},keyid:{3}", (object) args.PrevState, (object) args.CurrState, (object) args.PeerJid, (object) args.CallId);
      if (this.CallState == UiCallState.Ending || this.CallState == UiCallState.Voicemail)
        return;
      if (this.callId == null)
        this.callId = args.CallId;
      if (args.PrevState == UiCallState.ReceivedCall && (args.CurrState == UiCallState.Active || args.CurrState == UiCallState.Connecting))
        CallScreenPage.Launch(args.PeerJid, new UiCallState?(args.CurrState), true, this.VideoEnabled);
      else if (this.users.Any<UserStatus>() && this.users.Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).Contains<string>(args.PeerJid) && this.callId == args.CallId)
        this.CallState = args.CurrState;
      else
        Log.l("callscreen", "unexpected call screen state");
    }

    private void FireLocalVideoStateChanged(UiVideoState oldState, UiVideoState newState)
    {
      Log.l("callscreen", "LocalVideoStateChanged: {0}->{1}", (object) oldState, (object) newState);
      this.LocalVideoStateChangedSubject.OnNext(new UiVideoState?(newState));
    }

    private void FireRemoteVideoStateChanged(UiVideoState oldState, UiVideoState newState)
    {
      Log.l("callscreen", "RemoteVideoStateChanged: {0}->{1}", (object) oldState, (object) newState);
      this.RemoteVideoStateChangedSubject.OnNext(new UiVideoState?(newState));
    }

    private double VideoOrientationToDegrees(VideoOrientation orientation, bool localSource)
    {
      double degrees = 0.0;
      switch (orientation)
      {
        case VideoOrientation.Default:
          degrees = 0.0;
          break;
        case VideoOrientation.Orientation90:
          degrees = -90.0;
          break;
        case VideoOrientation.Orientation180:
          degrees = 180.0;
          break;
        case VideoOrientation.Orientation270:
          degrees = 90.0;
          break;
        default:
          Log.l("callscreen", "Unrecognized video orientation {0}", (object) orientation.ToString());
          break;
      }
      if (localSource)
        degrees = this.localCamera == CameraLocation.Front ? -90.0 : 90.0;
      return degrees;
    }

    public enum Peer
    {
      One = 1,
      Two = 2,
      Three = 3,
    }

    public struct PeerCallState
    {
      public readonly UiCallParticipantState State;
      public readonly bool Muted;

      public PeerCallState(UiCallParticipantState state, bool muted)
      {
        this.State = state;
        this.Muted = muted;
      }
    }
  }
}
