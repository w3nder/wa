// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.CallParticipantDetailInfo
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  public struct CallParticipantDetailInfo
  {
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
