// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCallEndedEventArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;


namespace WhatsApp
{
  public class WaCallEndedEventArgs : WaCallEventArgs
  {
    public CallEndReason Reason { get; set; }

    public bool ShouldRateCall { get; set; }

    public byte[] RatingCookie { get; set; }

    public CallDataUsage DataUsage { get; set; }

    public WaCallEndedEventArgs(string peerJid, string callId)
      : base(peerJid, callId)
    {
    }
  }
}
