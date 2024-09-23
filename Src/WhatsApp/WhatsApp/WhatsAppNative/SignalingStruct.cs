// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.SignalingStruct
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [MarshalingBehavior]
  [Version(100794368)]
  [Activatable(100794368)]
  public sealed class SignalingStruct : ISignalingStruct
  {
    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern SignalingStruct();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern byte[] GetBuffer();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void PutBuffer([In] byte[] Input);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern SignalingMessageType GetMessageType();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetMessageType([In] SignalingMessageType type);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetCallId();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCallId([In] string id);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetPeerJid();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetPeerJid([In] string jid);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetCallCreatorJid();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCallCreatorJid([In] string jid);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetTransactionId();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTransactionId([In] int TransactionId);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetIsNotContact([In] bool IsNotContact);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern Capabilities GetCapabilities();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCapabilities([In] Capabilities caps);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern ClientPlatformName GetPeerPlatform();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetPeerPlatform([In] ClientPlatformName platform);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetRelayCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayCount([In] int Count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern byte[] GetRelayInformation([In] int Idx);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayInformation([In] int Idx, [In] byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetRelayTokenCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayTokenCount([In] int count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern byte[] GetRelayToken([In] int Idx);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayToken([In] int Idx, [In] byte[] Input);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetReason();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetReason([In] string s);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetTransportCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTransportCount([In] int count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetTransportCandidate(
      [In] int idx,
      out byte Priority,
      out bool PortPredicting,
      out byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetTransportCandidate(
      [In] int idx,
      [In] byte Priority,
      [In] bool PortPredicting,
      [In] byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetErrorCode();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetErrorCode([In] int err);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetRelayElection(out int Latency, out byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayElection([In] int Latency, [In] byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetRelayLatencyCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayLatencyCount([In] int Count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetRelayLatency([In] int Index, out int Latency, out byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRelayLatency([In] int Index, [In] int Latency, [In] byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ParseCompressedCallParams(
      [In] byte[] CallParamsInCompressedJson,
      [In] PlatformCallParams PlatformParams);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void ClearCallParams();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetCallParam([In] string ParamPath);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCallKey([In] byte[] KeyBytes);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetCallKey(out byte[] KeyBytes);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetRetryCount([In] uint Count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetRetryCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVideoParams([In] VideoParams Params);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern VideoParams GetVideoParams();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern int GetVideoParamsCount();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVideoParamsCount([In] int Count);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVideoParamsIdx([In] int Idx, [In] VideoParams Params);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern VideoParams GetVideoParamsIdx([In] int Idx);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetVideoState([In] VideoState State);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern VideoState GetVideoState();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetReflexiveAddress([In] byte[] Address);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetNotifyParams([In] NotifyParams Params);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern NotifyParams GetNotifyParams();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern VoipNetworkType GetNetworkMedium();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetNetworkMedium([In] VoipNetworkType Medium);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetKeyGenVersion([In] uint Version);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern uint GetKeyGenVersion();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetCapability([In] int Version, [In] byte[] Capability);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetCapability(out int Version, out byte[] Capability);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetGroupCapability([In] uint Index, [In] int Version, [In] byte[] Capability);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetGroupCapability([In] uint Index, out int Version, out byte[] Capability);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetMuted([In] bool Muted);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetMuted();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetInterrupted([In] bool Interrupted);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetInterrupted();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetGroupInfo(
      [In] int TransactionId,
      [In] bool Rekey,
      [In] string Media,
      [In] CallParticipantInfo[] ParticipantInfo);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetGroupInfo(
      out int TransactionId,
      out bool Rekey,
      out string Media,
      out CallParticipantInfo[] ParticipantInfo);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetSettingsType([In] VoipSettingsType SettingsType);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern VoipSettingsType GetSettingsType();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetFlowControlParams([In] int Bitrate, [In] int Width, [In] int Fps);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetFlowControlParams(out int Bitrate, out int Width, out int Fps);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetPeerState([In] string Jid, [In] CallParticipantState State);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void GetPeerState(out string Jid, out CallParticipantState State);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetResume([In] bool Muted);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern bool GetResume();

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern void SetDeviceName([In] DeviceNameSide Side, [In] string Name);

    [MethodImpl(MethodCodeType = MethodCodeType.Runtime)]
    public extern string GetDeviceName([In] DeviceNameSide Side);
  }
}
