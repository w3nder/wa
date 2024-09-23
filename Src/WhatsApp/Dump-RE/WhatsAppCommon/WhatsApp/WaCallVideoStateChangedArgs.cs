// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCallVideoStateChangedArgs
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class WaCallVideoStateChangedArgs : WaCallEventArgs
  {
    public CameraInformation LocalCamera;

    public UiVideoState LocalVideoState { get; private set; }

    public UiVideoState RemoteVideoState { get; private set; }

    public UiUpgradeState UpgradeState { get; private set; }

    public WaCallVideoStateChangedArgs(
      string peerJid,
      string callId,
      UiVideoState localVideoState,
      UiVideoState remoteVideoState,
      UiUpgradeState upgradeState,
      CameraInformation localCamera)
      : base(peerJid, callId)
    {
      this.LocalVideoState = localVideoState;
      this.RemoteVideoState = remoteVideoState;
      this.UpgradeState = upgradeState;
      this.LocalCamera = localCamera;
    }
  }
}
