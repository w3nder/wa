// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.CallInfoStruct
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public struct CallInfoStruct
  {
    public CallState PreviousCallState;
    public CallState CallState;
    public CallerStatus CallerStatus;
    public long CallActiveTime;
    public int CallDuration;
    public int AudioDuration;
    public int VideoDuration;
    public bool EndedByMe;
    public uint BytesSent;
    public uint BytesReceived;
    public bool IsGroupCall;
    public bool EnableGroupCall;
    public bool CanInviteNewParticipant;
    public bool VideoEnabled;
    public bool VideoPreviewStarted;
    public VideoCodec Codec;
    public CameraInformation LocalCamera;
    public uint ParticipantCount;
    public bool IsHoldAndResumeCapable;
    public uint PendingCallCount;
    public uint HeldCallCount;
  }
}
