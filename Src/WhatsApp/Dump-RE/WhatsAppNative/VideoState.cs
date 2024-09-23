// Decompiled with JetBrains decompiler
// Type: WhatsAppNative.VideoState
// Assembly: WhatsAppNative, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime
// MVID: B2F38A15-831E-4A18-9B26-41B1DE190E2F
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppNative.winmd

using Windows.Foundation.Metadata;

#nullable disable
namespace WhatsAppNative
{
  [Version(100794368)]
  public enum VideoState
  {
    Disabled = 0,
    Enabled = 1,
    Paused = 2,
    UpgradeRequest = 3,
    UpgradeAccept = 4,
    UpgradeReject = 5,
    Stopped = 6,
    UpgradeRejectByTimeout = 7,
    UpgradeCancel = 8,
    UpgradeCancelByTimeout = 9,
    Error = 20, // 0x00000014
  }
}
