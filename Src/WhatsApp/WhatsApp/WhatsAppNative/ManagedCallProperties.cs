// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.ManagedCallProperties
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;


namespace WhatsAppNative
{
  [Version(100794368)]
  public struct ManagedCallProperties
  {
    public bool SendImmediately;
    public bool ShouldRateCall;
    public int CallRatingIntervalInSeconds;
    public bool ShouldUploadLogs;
    public bool AudioDriverErrorOccurred;
    public bool BadASN;
    public int ElapsedServerTime;
    public int CallerTimeout;
    public int MsftCallActiveRetries;
    public int BatteryPercentChanged;
    public bool LastCallSamePeer;
    public double LastCallInterval;
    public bool LastCallVideo;
    public bool ForbidP2PForNonContact;
  }
}
