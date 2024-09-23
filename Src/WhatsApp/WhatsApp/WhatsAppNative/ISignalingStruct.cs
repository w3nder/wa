// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ISignalingStruct
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Guid(998930057, 12053, 18830, 137, 114, 5, 208, 158, 158, 16, 18)]
  [Version(100794368)]
  public interface ISignalingStruct
  {
    byte[] GetBuffer();

    void PutBuffer([In] byte[] Input);

    SignalingMessageType GetMessageType();

    void SetMessageType([In] SignalingMessageType type);

    string GetCallId();

    void SetCallId([In] string id);

    string GetPeerJid();

    void SetPeerJid([In] string jid);

    string GetCallCreatorJid();

    void SetCallCreatorJid([In] string jid);

    int GetTransactionId();

    void SetTransactionId([In] int TransactionId);

    void SetIsNotContact([In] bool IsNotContact);

    Capabilities GetCapabilities();

    void SetCapabilities([In] Capabilities caps);

    ClientPlatformName GetPeerPlatform();

    void SetPeerPlatform([In] ClientPlatformName platform);

    int GetRelayCount();

    void SetRelayCount([In] int Count);

    byte[] GetRelayInformation([In] int Idx);

    void SetRelayInformation([In] int Idx, [In] byte[] Address);

    int GetRelayTokenCount();

    void SetRelayTokenCount([In] int count);

    byte[] GetRelayToken([In] int Idx);

    void SetRelayToken([In] int Idx, [In] byte[] Input);

    string GetReason();

    void SetReason([In] string s);

    int GetTransportCount();

    void SetTransportCount([In] int count);

    void GetTransportCandidate(
      [In] int idx,
      out byte Priority,
      out bool PortPredicting,
      out byte[] Address);

    void SetTransportCandidate([In] int idx, [In] byte Priority, [In] bool PortPredicting, [In] byte[] Address);

    int GetErrorCode();

    void SetErrorCode([In] int err);

    void GetRelayElection(out int Latency, out byte[] Address);

    void SetRelayElection([In] int Latency, [In] byte[] Address);

    int GetRelayLatencyCount();

    void SetRelayLatencyCount([In] int Count);

    void GetRelayLatency([In] int Index, out int Latency, out byte[] Address);

    void SetRelayLatency([In] int Index, [In] int Latency, [In] byte[] Address);

    void ParseCompressedCallParams(
      [In] byte[] CallParamsInCompressedJson,
      [In] PlatformCallParams PlatformParams);

    void ClearCallParams();

    string GetCallParam([In] string ParamPath);

    void SetCallKey([In] byte[] KeyBytes);

    void GetCallKey(out byte[] KeyBytes);

    void SetRetryCount([In] uint Count);

    uint GetRetryCount();

    void SetVideoParams([In] VideoParams Params);

    VideoParams GetVideoParams();

    int GetVideoParamsCount();

    void SetVideoParamsCount([In] int Count);

    void SetVideoParamsIdx([In] int Idx, [In] VideoParams Params);

    VideoParams GetVideoParamsIdx([In] int Idx);

    void SetVideoState([In] VideoState State);

    VideoState GetVideoState();

    void SetReflexiveAddress([In] byte[] Address);

    void SetNotifyParams([In] NotifyParams Params);

    NotifyParams GetNotifyParams();

    VoipNetworkType GetNetworkMedium();

    void SetNetworkMedium([In] VoipNetworkType Medium);

    void SetKeyGenVersion([In] uint Version);

    uint GetKeyGenVersion();

    void SetCapability([In] int Version, [In] byte[] Capability);

    void GetCapability(out int Version, out byte[] Capability);

    void SetGroupCapability([In] uint Index, [In] int Version, [In] byte[] Capability);

    void GetGroupCapability([In] uint Index, out int Version, out byte[] Capability);

    void SetMuted([In] bool Muted);

    bool GetMuted();

    void SetInterrupted([In] bool Interrupted);

    bool GetInterrupted();

    void SetGroupInfo(
      [In] int TransactionId,
      [In] bool Rekey,
      [In] string Media,
      [In] CallParticipantInfo[] ParticipantInfo);

    void GetGroupInfo(
      out int TransactionId,
      out bool Rekey,
      out string Media,
      out CallParticipantInfo[] ParticipantInfo);

    void SetSettingsType([In] VoipSettingsType SettingsType);

    VoipSettingsType GetSettingsType();

    void SetFlowControlParams([In] int Bitrate, [In] int Width, [In] int Fps);

    void GetFlowControlParams(out int Bitrate, out int Width, out int Fps);

    void SetPeerState([In] string Jid, [In] CallParticipantState State);

    void GetPeerState(out string Jid, out CallParticipantState State);

    void SetResume([In] bool Muted);

    bool GetResume();

    void SetDeviceName([In] DeviceNameSide Side, [In] string Name);

    string GetDeviceName([In] DeviceNameSide Side);
  }
}
