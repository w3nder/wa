// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IVoip
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(2635100850, 47306, 18640, 177, 14, 213, 100, 30, 218, 234, 201)]
  public interface IVoip
  {
    void Dispose();

    void StartCall([In] string PeerJid, [In] string CallId, [In] bool HasVideo);

    void AcceptCall();

    void RejectCall();

    void EndCall([In] bool Terminate);

    void SetAudioOutput([In] AudioOutput Output);

    void SetOnHold([In] bool Enabled);

    void SetMute([In] bool Enabled);

    void NotifyNetworkChange();

    void SetNetworkType([In] VoipNetworkType NetType);

    void SetCallbacks([In] IVoipCallbacks Callbacks);

    IVoipCallbacks GetCallbacks();

    void OnIncomingSignalData([In] byte[] Ptr);

    string GetEventString([In] VoipEvent Event);

    bool IsMuted();

    CallDataUsage GetDataUsage();

    void NotifyAudioOutputChange([In] AudioOutput Type);

    void PauseAudioStream();

    void ResumeAudioStream();

    void SetSampleRates([In] int[] SampleRates);

    void SetRemotePlatform([In] string PlatformName, [In] string AppVersion);

    void TimerTick();

    void PlayAlert([In] string Filename);

    void StartVideoPreview();

    void ToggleCamera();

    int GetCameraCount();

    void OrientationChanged([In] ScreenOrientation Orientation);

    void PauseVideoStream();

    void ResumeVideoStream();

    string GetVideoInfoString();

    void SetBatteryState([In] float BatteryDrop, [In] float BatteryPct);

    void DelayedCallInterrupted([In] uint DelayMs, [In] string PeerJid);

    void RequestKeyframe();

    void SetPeerVideoFlowControl([In] uint MaxBitrate, [In] ushort MaxWidth, [In] ushort MaxFps);

    void RequestVideoUpgrade();

    void RequestVideoDowngrade();

    void TurnCamera([In] bool on);

    void AcceptVideoUpgrade();

    void RejectVideoUpgrade([In] UpgradeRequestEndReason reason);

    void CancelVideoUpgrade([In] UpgradeRequestEndReason reason);

    void InviteParticipant([In] string PeerJid);

    void CancelInviteParticipant([In] string PeerJid);

    bool ParticipantWasInvited();

    void SendRekeyRequest([In] string PeerJid, [In] uint Retry);

    void EndCallAndAcceptPendingCall([In] string Id);

    void RejectPendingCall([In] string Id);

    void HoldCallAndSwitchToCall([In] string Id);

    void EndHeldCall([In] string Id);

    bool GetNextCallLogEntry([In] int Idx, out string Jid, out CallLogResult Result);
  }
}
