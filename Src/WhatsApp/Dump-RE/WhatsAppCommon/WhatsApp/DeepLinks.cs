// Decompiled with JetBrains decompiler
// Type: WhatsApp.DeepLinks
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public static class DeepLinks
  {
    public const string BackgroundTasksUrl = "app://25BB0297-A723-479c-BDC8-9274FC6C6470/_default";
    public const string BatterySaverUrl = "ms-settings-power:";
    public const string LocationUrl = "ms-settings-location:";
    public static readonly string DateTimeUrl = AppState.IsWP10OrLater ? "ms-settings:dateandtime" : "app://5B04B775-356B-4AA0-AAF8-6491FFEA5606/_default";
    public const string DataSenseUrl = "app://5B04B775-356B-4AA0-AAF8-6491FFEA5646/Settings";
    public const string CellularSettingsUrl = "ms-settings-cellular:";
    public const string LockScreenUrl = "ms-settings-lock:";
    public const string ScreenRotationUrl = "ms-settings-screenrotation:";
    public static readonly string EaseOfAccessUrl = AppState.IsWP10OrLater ? "ms-settings:easeofaccess-otheroptions" : "app://5B04B775-356B-4AA0-AAF8-6491FFEA5801/_default";
    public static readonly string OneDriveUrl = AppState.IsWP10OrLater ? "ms-onedrive:" : "app://AD543082-80EC-45BB-AA02-FFE7F4182BA8/_default";
    public const string MicrophonePermissionsUrl = "ms-settings:privacy-microphone";
  }
}
