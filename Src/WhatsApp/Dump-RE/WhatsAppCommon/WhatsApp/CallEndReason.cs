// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallEndReason
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public enum CallEndReason
  {
    Unknown,
    Rejected,
    InternalError,
    Timeout,
    YourCountryNotAllowed,
    PeerCountryNotAllowed,
    YourClientNotVoipCapable,
    PeerUnavailable,
    PeerAppTooOld,
    PeerOsTooOld,
    PeerBadPlatform,
    UnknownErrorCode,
    PeerBusy,
    PeerUncallable,
    IncompatibleSrtpKeyExchange,
    SrtpKeyGenError,
    UnsupportedAudio,
    RebootRequired,
    PeerRelayBindFailed,
    YourASNBad,
    RelayBindFailed,
    BadPrivacySettings,
  }
}
