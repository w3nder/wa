// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IUserVoipCallbacks
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(110370395, 60857, 20041, 138, 111, 19, 86, 195, 5, 28, 204)]
  [Version(100794368)]
  public interface IUserVoipCallbacks
  {
    void OnIncomingCall([In] string Jid, [In] string CallId, [In] bool videoEnabled);

    void OnCallStarted([In] string PeerJid, [In] string CallId, [In] bool videoEnabled);

    void OnVideoStateChanged(
      [In] string Jid,
      [In] string CallId,
      [In] UiVideoState localVideoState,
      [In] UiVideoState remoteVideoState,
      [In] UiUpgradeState upgradeState,
      [In] CameraInformation LocalCamera);

    void OnVideoOrientationChanged([In] string Jid, [In] string CallId, [In] VideoOrientation remoteOrientation);

    void OnCameraRestartBegin();

    void OnCameraRestartEnd();

    void OnCallEnded([In] string PeerJid, [In] string CallId, [In] byte[] Cookie, [In] CallEndedNativeArgs Args);

    void OnCallStateChanged(
      [In] string PeerJid,
      [In] string CallId,
      [In] UiCallState oldState,
      [In] UiCallState newState);

    void OnOfferAckReceived([In] bool groupCallEnabled);

    void OnMissedCall(
      [In] string PeerJid,
      [In] string CallId,
      [In] long Time,
      [In] int elapsedMs,
      [In] bool NewRecord,
      [In] bool HasVideo);

    void OnVideoPlayerRestart();

    void OnBatteryLevelLow([In] string PeerJid, [In] string CallId, [In] UiBatteryLevelSource Source);

    void OnAudioFallback([In] string PeerJid, [In] string CallId);

    void OnGroupInfoChanged();

    void OnGroupStateChanged();
  }
}
