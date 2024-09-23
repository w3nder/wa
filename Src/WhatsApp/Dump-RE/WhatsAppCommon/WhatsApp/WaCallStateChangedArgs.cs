// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCallStateChangedArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class WaCallStateChangedArgs : WaCallEventArgs
  {
    public UiCallState PrevState { get; private set; }

    public UiCallState CurrState { get; private set; }

    public WaCallStateChangedArgs(
      string peerJid,
      string callId,
      UiCallState oldState,
      UiCallState newState)
      : base(peerJid, callId)
    {
      this.PrevState = oldState;
      this.CurrState = newState;
    }
  }
}
