// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public static class OneDriveUtils
  {
    private static string[] ONEDRIVE_AUTH_ERROR_OS_LEVELS = new string[3]
    {
      "10.0.15007.1000",
      "10.0.15014.1000",
      "10.0.15025.1001"
    };

    public static bool UseWP10AccountInterface()
    {
      bool flag = false;
      if (AppState.IsWP10OrLater)
      {
        flag = true;
        string str = AppState.OSVersion.ToString();
        if (Array.IndexOf<string>(OneDriveUtils.ONEDRIVE_AUTH_ERROR_OS_LEVELS, str) >= 0 || str == Settings.OneDriveWP10AuthInterfaceError)
          flag = false;
      }
      return flag;
    }

    private static bool ShouldReportWP10AccountInterfaceError()
    {
      string str = AppState.OSVersion.ToString();
      if (Array.IndexOf<string>(OneDriveUtils.ONEDRIVE_AUTH_ERROR_OS_LEVELS, str) >= 0 || !(str != Settings.OneDriveWP10AuthInterfaceError))
        return false;
      Settings.OneDriveWP10AuthInterfaceError = AppState.OSVersion.ToString();
      return true;
    }

    public static bool TryWP10AccountInterface(Action wp10Action, string description)
    {
      bool flag = true;
      try
      {
        wp10Action();
      }
      catch (Exception ex)
      {
        string context = string.Format("TryWP10Action Exception {0}", (object) description);
        if (OneDriveUtils.ShouldReportWP10AccountInterfaceError())
          Log.SendCrashLog(ex, context);
        else
          Log.LogException(ex, context);
        flag = false;
      }
      return flag;
    }

    public static wam_enum_backup_schedule GetBackupSceduleForFieldStats()
    {
      switch (Settings.OneDriveBackupFrequency)
      {
        case OneDriveBackupFrequency.Off:
          return wam_enum_backup_schedule.OFF;
        case OneDriveBackupFrequency.Daily:
          return wam_enum_backup_schedule.DAILY;
        case OneDriveBackupFrequency.Weekly:
          return wam_enum_backup_schedule.WEEKLY;
        case OneDriveBackupFrequency.Monthly:
          return wam_enum_backup_schedule.MONTHLY;
        default:
          return wam_enum_backup_schedule.MANUAL;
      }
    }

    public static wam_enum_backup_network_setting? GetBackupNetworkSettingForFieldStats()
    {
      int driveBackupNetwork = (int) Settings.OneDriveBackupNetwork;
      bool flag1 = (driveBackupNetwork & 2) != 0;
      bool flag2 = (driveBackupNetwork & 1) != 0;
      if (flag2 & flag1)
        return new wam_enum_backup_network_setting?(wam_enum_backup_network_setting.WIFI_OR_CELLULAR);
      return flag2 ? new wam_enum_backup_network_setting?(wam_enum_backup_network_setting.WIFI_ONLY) : new wam_enum_backup_network_setting?();
    }
  }
}
