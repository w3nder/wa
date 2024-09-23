// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallParticipantDetail
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using WhatsAppNative;


namespace WhatsApp
{
  public struct CallParticipantDetail
  {
    public string Jid;
    public CallParticipantState State;
    public bool IsSelf;
    public bool IsMuted;
    public bool IsInterrupted;
    public bool VideoRenderStarted;
    public bool VideoDecodeStarted;
    public bool VideoDecodePaused;
    public VideoState VideoStreamState;
    public int VideoWidth;
    public int VideoHeight;
    public VideoOrientation VideoOrientation;
    public bool IsAudioVideoSwitchEnabled;
    public bool IsAudioVideoSwitchSupported;
    public bool IsInvitedBySelf;
    public bool RxConnecting;
    public bool RxTimedOut;
    public int RxAudioPacketCount;
  }
}
