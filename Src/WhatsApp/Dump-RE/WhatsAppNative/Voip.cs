// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.Voip
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class Voip : IVoip, ICallInfo
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Voip();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void Dispose();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void StartCall([In] string PeerJid, [In] string CallId, [In] bool HasVideo);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void AcceptCall();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RejectCall();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EndCall([In] bool Terminate);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetAudioOutput([In] AudioOutput Output);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetOnHold([In] bool Enabled);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetMute([In] bool Enabled);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void NotifyNetworkChange();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetNetworkType([In] VoipNetworkType NetType);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCallbacks([In] IVoipCallbacks Callbacks);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern IVoipCallbacks GetCallbacks();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OnIncomingSignalData([In] byte[] Ptr);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetEventString([In] VoipEvent Event);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool IsMuted();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern CallDataUsage GetDataUsage();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void NotifyAudioOutputChange([In] AudioOutput Type);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PauseAudioStream();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ResumeAudioStream();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetSampleRates([In] int[] SampleRates);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRemotePlatform([In] string PlatformName, [In] string AppVersion);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void TimerTick();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PlayAlert([In] string Filename);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void StartVideoPreview();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ToggleCamera();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetCameraCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void OrientationChanged([In] ScreenOrientation Orientation);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PauseVideoStream();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ResumeVideoStream();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetVideoInfoString();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetBatteryState([In] float BatteryDrop, [In] float BatteryPct);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void DelayedCallInterrupted([In] uint DelayMs, [In] string PeerJid);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RequestKeyframe();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetPeerVideoFlowControl([In] uint MaxBitrate, [In] ushort MaxWidth, [In] ushort MaxFps);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RequestVideoUpgrade();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RequestVideoDowngrade();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void TurnCamera([In] bool on);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void AcceptVideoUpgrade();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RejectVideoUpgrade([In] UpgradeRequestEndReason reason);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CancelVideoUpgrade([In] UpgradeRequestEndReason reason);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void InviteParticipant([In] string PeerJid);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void CancelInviteParticipant([In] string PeerJid);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool ParticipantWasInvited();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SendRekeyRequest([In] string PeerJid, [In] uint Retry);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EndCallAndAcceptPendingCall([In] string Id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void RejectPendingCall([In] string Id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void HoldCallAndSwitchToCall([In] string Id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void EndHeldCall([In] string Id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetNextCallLogEntry([In] int Idx, out string Jid, out CallLogResult Result);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetCallInfo(
      out string CallId,
      out string PeerJid,
      out CallInfoStruct InfoStruct);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetParticipantInfo(
      [In] int ParticipantIdx,
      out string ParticipantJid,
      out CallParticipantDetailInfo Participant);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetPendingCallInfo([In] int Idx, out string PeerJid, out string CallId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetHeldCallInfo([In] int Idx, out string PeerJid, out string CallId);
  }
}
