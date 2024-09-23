// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.IVoipCallbacks
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  [Guid(3582232639, 25735, 16616, 174, 2, 104, 194, 91, 185, 215, 207)]
  public interface IVoipCallbacks
  {
    void OnEvent([In] VoipEvent Event, [In] string PeerJid, [In] string CallId, [In] object Value);

    void OnSignalingData([In] byte[] Ptr, [In] SignalingDataArgs Args);

    void CancelRingTimers();

    IClosable SubscribeSignalHandler([In] IVoipSignalingCallbacks Callbacks);

    IClosable SubscribeUserCallbacks([In] IUserVoipCallbacks Callbacks, [In] bool FromForegroundApp);

    void SetPeerMetadata([In] string Name);

    void PlumbThroughError([In] string PeerJid, [In] string CallId, [In] string Str);

    void NotifyCallStart(
      [In] string PeerJid,
      [In] string CallId,
      [In] bool Incoming,
      [In] bool HasVideo,
      [In] string DisplayName);

    bool IsMsftCallbackRegistered();

    void OnRatingCookie([In] byte[] Ptr);

    void OnCallDataUsage([In] CallDataUsage Data);

    void ClearManagedCallProperties();

    void SetManagedCallProperties([In] string selfJid, [In] ManagedCallProperties props);

    ManagedCallProperties GetManagedCallProperties(out bool hasValue);

    void GetRandomBytes(out byte[] RawBytes);

    void CallKeysFromCipherKeyV1(
      [In] byte[] CipherKey,
      out byte[] callerSrtpBytes,
      out byte[] calleeSrtpBytes,
      out byte[] callerP2pBytes,
      out byte[] calleeP2pBytes);

    void CallKeysFromCipherKeyV2(
      [In] string Jid,
      [In] byte[] CipherKey,
      out byte[] srtpBytes,
      out byte[] p2pBytes);

    uint GetSecureSSRC([In] string CallId, [In] string Jid, [In] uint SsrcTag);

    void OnVideoFrameReceived(
      [In] byte[] Buffer,
      [In] VideoCodec codec,
      [In] int Width,
      [In] int Height,
      [In] long Timestamp,
      [In] bool keyframe,
      [In] int Orientation);

    void OnCameraRestartBegin();

    void OnCameraRestartEnd();

    void OnVideoPlayerRestart();

    CallApplicationSettings GetApplicationSettings();
  }
}
