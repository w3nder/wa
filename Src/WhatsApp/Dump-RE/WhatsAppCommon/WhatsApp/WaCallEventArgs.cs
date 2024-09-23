// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCallEventArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class WaCallEventArgs
  {
    public string CallId { get; private set; }

    public string PeerJid { get; private set; }

    public WaCallEventArgs(string peerJid, string callId)
    {
      this.PeerJid = peerJid;
      this.CallId = callId;
    }
  }
}
