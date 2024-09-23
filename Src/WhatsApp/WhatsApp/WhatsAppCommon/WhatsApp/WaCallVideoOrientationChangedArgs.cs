// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCallVideoOrientationChangedArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;


namespace WhatsApp
{
  public class WaCallVideoOrientationChangedArgs : WaCallEventArgs
  {
    public VideoOrientation RemoteOrientation { get; private set; }

    public WaCallVideoOrientationChangedArgs(
      string peerJid,
      string callId,
      VideoOrientation remoteOrientation)
      : base(peerJid, callId)
    {
      this.RemoteOrientation = remoteOrientation;
    }
  }
}
