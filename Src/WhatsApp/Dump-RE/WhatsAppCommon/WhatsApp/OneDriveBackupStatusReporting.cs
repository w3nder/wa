// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveBackupStatusReporting
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

#nullable disable
namespace WhatsApp
{
  public static class OneDriveBackupStatusReporting
  {
    private static readonly string OneDriveBackupSettingsFile = Constants.IsoStorePath + "\\onedrive_backupsettings.json";
    public const string UNDEFINED_BACKUP_ID = "";
    public const int BKUP_COMPLETED_OK = 1;
    public const int BKUP_CANCELLED_BY_USER = 2;
    public const int BKUP_FAILURE_ONE_DRIVE_AUTH = 4;
    public const int BKUP_FAILURE_NO_LOGIN = 5;
    public const int BKUP_FAILURE_ONE_DRIVE_NA = 10;
    public const int BKUP_FAILURE_ONE_DRIVE_QUOTA = 9;
    public const int BKUP_FAILURE_UNEXPECTED = 11;
    public const int BKUP_STARTED = 12;
    public const int BKUP_STARTED_BY_USER = 13;
    public const int BKUP_STOPPED_WILL_RESTART = 6;
    public const int BKUP_STOPPPED_CONNECTION_NOT_SUITABLE = 7;
    public const int BKUP_STOPPPED_BY_TIMEOUT = 15;
    public const int BKUP_STOPPPED_BATTERY_STATE_NOT_SUITABLE = 14;
    private static byte[] cachedSettings = (byte[]) null;
    private static object lockCachedSettings = new object();

    public static string CreateDisplayString(int statusId, string parm1, string parm2)
    {
      Log.l("odu", "displaying status string for {0}", (object) statusId);
      switch (statusId)
      {
        case 1:
          return "OneDrive backup completed";
        case 2:
          return AppResources.OneDriveBackupCancelled;
        case 4:
          return AppResources.OneDriveBackupAuthError;
        case 5:
          return AppResources.OneDriveBackupLoginFailure;
        case 6:
          return AppResources.OneDriveBackupUnexpectedFailure;
        case 7:
          return AppResources.OneDriveBackupProgressWaitingForNetwork;
        case 9:
          return parm1 != null ? string.Format(AppResources.OneDriveBackupErrorQuota, (object) parm1) : AppResources.OneDriveBackupErrorQuotaUnknownSize;
        case 10:
          return AppResources.OneDriveNotAvailableForBackup;
        case 11:
          return AppResources.OneDriveBackupUnexpectedFailure;
        case 12:
        case 13:
          return "Backup started";
        case 14:
          return AppResources.OneDriveBackupBatteryTooLow;
        default:
          return AppResources.OneDriveBackupUnexpectedFailure;
      }
    }

    public static OneDriveLastBackupState GetLastBackupState()
    {
      return OneDriveBackupStatusReporting.GetLastBackupStatus()?.LastState;
    }

    public static bool IsBackUpIncomplete()
    {
      OneDriveBackupStatusReporting.OneDriveBackupStatus lastBackupStatus = OneDriveBackupStatusReporting.GetLastBackupStatus();
      bool flag = false;
      if (lastBackupStatus != null && lastBackupStatus.StartTimestampTicks > 0L && lastBackupStatus.EndTimestampTicks <= 0L)
        flag = DateTime.UtcNow.Ticks - lastBackupStatus.StartTimestampTicks < TimeSpan.FromDays(1.0).Ticks;
      Log.d("odm", "IsBackUpIncomplete {0}", (object) flag);
      return flag;
    }

    public static bool IsBackUpUserInitiatedAndActive()
    {
      OneDriveBackupStatusReporting.OneDriveBackupStatus lastBackupStatus = OneDriveBackupStatusReporting.GetLastBackupStatus();
      bool flag = false;
      if (lastBackupStatus != null && lastBackupStatus.StartedByUser && lastBackupStatus.EndTimestampTicks <= 0L)
        flag = DateTime.UtcNow.Ticks - lastBackupStatus.StartTimestampTicks < TimeSpan.FromDays(1.0).Ticks;
      Log.d("odm", "IsBackUpUserInitiatedAndActive {0}", (object) flag);
      return flag;
    }

    public static bool IsAfterStartTime(DateTime lastLocalBackupTime)
    {
      OneDriveBackupStatusReporting.OneDriveBackupStatus lastBackupStatus = OneDriveBackupStatusReporting.GetLastBackupStatus();
      bool flag = true;
      if (lastBackupStatus != null)
      {
        object[] objArray = new object[2];
        DateTime universalTime = lastLocalBackupTime.ToUniversalTime();
        objArray[0] = (object) universalTime.Ticks;
        objArray[1] = (object) new DateTime(lastBackupStatus.StartTimestampTicks);
        Log.d("odm", "checking {0} and {1}", objArray);
        universalTime = lastLocalBackupTime.ToUniversalTime();
        flag = universalTime.Ticks > lastBackupStatus.StartTimestampTicks;
      }
      Log.d("odm", "WasBackupInitiatedBefore {0}", (object) flag);
      return flag;
    }

    public static bool IsLastBackupTimedOut()
    {
      bool flag = false;
      OneDriveBackupStatusReporting.OneDriveBackupStatus lastBackupStatus = OneDriveBackupStatusReporting.GetLastBackupStatus();
      if (lastBackupStatus != null && lastBackupStatus.LastState != null)
        flag = lastBackupStatus.LastState.Id == 15;
      return flag;
    }

    public static void SetLastBackupState(
      string currentBackupId,
      OneDriveBkupRestStopReason reason,
      OneDriveBackupStopError error,
      BackupProperties autoBkupProperties = null)
    {
      Log.l("odm", "Setting backup state {0}, {1}", (object) reason, (object) error);
      switch (reason)
      {
        case OneDriveBkupRestStopReason.Completed:
          OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, 1);
          break;
        case OneDriveBkupRestStopReason.Stop:
          int statusId1;
          switch (error)
          {
            case OneDriveBackupStopError.StoppedByOS:
              statusId1 = 6;
              break;
            case OneDriveBackupStopError.StoppedByTimeout:
              statusId1 = 15;
              break;
            default:
              statusId1 = 11;
              break;
          }
          OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, statusId1);
          break;
        case OneDriveBkupRestStopReason.StopError:
        case OneDriveBkupRestStopReason.Abort:
        case OneDriveBkupRestStopReason.AbortError:
          int statusId2;
          switch (error)
          {
            case OneDriveBackupStopError.Unauthenticated:
              statusId2 = 4;
              break;
            case OneDriveBackupStopError.ServiceNotAvailable:
              statusId2 = 10;
              break;
            case OneDriveBackupStopError.QuotaLimitReached:
              statusId2 = 9;
              break;
            case OneDriveBackupStopError.CancelledByUser:
              statusId2 = 2;
              break;
            case OneDriveBackupStopError.LoginFailure:
              statusId2 = 5;
              break;
            case OneDriveBackupStopError.StoppedByBattery:
              statusId2 = 14;
              break;
            default:
              statusId2 = 11;
              break;
          }
          if (statusId2 == 9)
          {
            long bytes = autoBkupProperties == null ? OneDriveBackupManager.Instance.BackupProperties.IncompleteSize : autoBkupProperties.IncompleteSize;
            string parm1 = bytes >= 1024L ? Utils.FileSizeFormatter.Format(bytes) : (string) null;
            OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, 9, parm1: parm1);
            break;
          }
          OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, statusId2);
          break;
        case OneDriveBkupRestStopReason.NetworkChange:
          OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, 7);
          break;
        default:
          OneDriveBackupStatusReporting.SetLastBackupState(currentBackupId, 11);
          break;
      }
    }

    public static void SetLastBackupState(
      string backupId,
      int statusId,
      OneDriveBackupStatusReporting.BackupStateDisplayOptions displayOption = OneDriveBackupStatusReporting.BackupStateDisplayOptions.ShouldDisplay,
      string parm1 = null,
      string parm2 = null)
    {
      string str = backupId ?? "";
      try
      {
        OneDriveBackupStatusReporting.OneDriveBackupStatus driveBackupStatus1 = OneDriveBackupStatusReporting.GetLastBackupStatus();
        DateTime utcNow;
        if (driveBackupStatus1 == null)
        {
          OneDriveBackupStatusReporting.OneDriveBackupStatus driveBackupStatus2 = new OneDriveBackupStatusReporting.OneDriveBackupStatus();
          driveBackupStatus2.BackupId = str;
          utcNow = DateTime.UtcNow;
          driveBackupStatus2.StartTimestampTicks = utcNow.Ticks;
          driveBackupStatus1 = driveBackupStatus2;
        }
        else
          driveBackupStatus1.BackupId = backupId;
        if (statusId == 13 || statusId == 12)
        {
          if (statusId == 13 || driveBackupStatus1.StartTimestampTicks == 0L || driveBackupStatus1.EndTimestampTicks > 0L)
            driveBackupStatus1.StartedByUser = statusId == 13;
          if (!driveBackupStatus1.StartedByUser || driveBackupStatus1.StartTimestampTicks == 0L)
          {
            OneDriveBackupStatusReporting.OneDriveBackupStatus driveBackupStatus3 = driveBackupStatus1;
            utcNow = DateTime.UtcNow;
            long ticks = utcNow.Ticks;
            driveBackupStatus3.StartTimestampTicks = ticks;
          }
          driveBackupStatus1.EndTimestampTicks = 0L;
        }
        if (statusId == 1 || statusId == 2)
        {
          OneDriveBackupStatusReporting.OneDriveBackupStatus driveBackupStatus4 = driveBackupStatus1;
          utcNow = DateTime.UtcNow;
          long ticks = utcNow.Ticks;
          driveBackupStatus4.EndTimestampTicks = ticks;
        }
        OneDriveBackupStatusReporting.OneDriveBackupStatus driveBackupStatus5 = driveBackupStatus1;
        OneDriveLastBackupState driveLastBackupState = new OneDriveLastBackupState();
        utcNow = DateTime.UtcNow;
        driveLastBackupState.TimestampTicks = utcNow.Ticks;
        driveLastBackupState.Id = statusId;
        driveLastBackupState.DisplayFlag = (int) displayOption;
        driveBackupStatus5.LastState = driveLastBackupState;
        if (parm1 != null)
          driveBackupStatus1.LastState.Parm1 = parm1;
        if (parm2 != null)
          driveBackupStatus1.LastState.Parm2 = parm2;
        MemoryStream memoryStream1 = new MemoryStream();
        DataContractJsonSerializer contractJsonSerializer = new DataContractJsonSerializer(typeof (OneDriveBackupStatusReporting.OneDriveBackupStatus));
        memoryStream1.Position = 0L;
        MemoryStream memoryStream2 = memoryStream1;
        OneDriveBackupStatusReporting.OneDriveBackupStatus graph = driveBackupStatus1;
        contractJsonSerializer.WriteObject((Stream) memoryStream2, (object) graph);
        OneDriveBackupStatusReporting.SetBackupSettingsBytes(memoryStream1.ToArray());
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "odm Exception saving last backup state");
      }
      Log.l("odm", "setLastBackupState {0}", (object) statusId);
    }

    private static OneDriveBackupStatusReporting.OneDriveBackupStatus GetLastBackupStatus()
    {
      OneDriveBackupStatusReporting.OneDriveBackupStatus lastBackupStatus = (OneDriveBackupStatusReporting.OneDriveBackupStatus) null;
      int num = 2;
      bool flushCache = false;
      while (lastBackupStatus == null && num > 0)
      {
        --num;
        byte[] backupSettingsBytes = OneDriveBackupStatusReporting.GetBackupSettingsBytes(flushCache);
        if (backupSettingsBytes != null)
        {
          if (backupSettingsBytes.Length >= 1)
          {
            try
            {
              using (MemoryStream memoryStream = new MemoryStream(backupSettingsBytes))
              {
                lastBackupStatus = new DataContractJsonSerializer(typeof (OneDriveBackupStatusReporting.OneDriveBackupStatus)).ReadObject((Stream) memoryStream) as OneDriveBackupStatusReporting.OneDriveBackupStatus;
                continue;
              }
            }
            catch (Exception ex)
            {
              Log.l("odm", "GetLastBackupStatus data: {0}", (object) backupSettingsBytes.ToHexString());
              Log.LogException(ex, "odm exception getting backup status");
              if (num == 0)
                Log.SendCrashLog((Exception) new ArgumentException("One Drive corrupted backup settings"), "odm backup setting exception");
              lastBackupStatus = (OneDriveBackupStatusReporting.OneDriveBackupStatus) null;
              flushCache = true;
              continue;
            }
          }
        }
        return (OneDriveBackupStatusReporting.OneDriveBackupStatus) null;
      }
      return lastBackupStatus;
    }

    public static void MaybeMigrateBackupSettings()
    {
      bool flag = false;
      try
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          flag = nativeMediaStorage.FileExists(OneDriveBackupStatusReporting.OneDriveBackupSettingsFile);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "odm - Exception migrating settings");
      }
      if (flag)
        return;
      OneDriveBackupStatusReporting.SetBackupSettingsBytes(Settings.OneDriveBackupStatus);
      Settings.OneDriveBackupStatus = (byte[]) null;
    }

    private static byte[] GetBackupSettingsBytes(bool flushCache)
    {
      lock (OneDriveBackupStatusReporting.lockCachedSettings)
      {
        if (flushCache)
          OneDriveBackupStatusReporting.cachedSettings = (byte[]) null;
        if (OneDriveBackupStatusReporting.cachedSettings == null)
        {
          OneDriveBackupStatusReporting.cachedSettings = new byte[0];
          try
          {
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              if (nativeMediaStorage.FileExists(OneDriveBackupStatusReporting.OneDriveBackupSettingsFile))
              {
                using (Stream stream = nativeMediaStorage.OpenFile(OneDriveBackupStatusReporting.OneDriveBackupSettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                  int length = (int) stream.Length;
                  byte[] buffer = new byte[length];
                  int num = stream.Read(buffer, 0, length);
                  if (num != length)
                  {
                    Log.l("odm", "Unexpected length reading settings file {0} {1}", (object) num, (object) length);
                    throw new EndOfStreamException("unexpected EOF reading settings");
                  }
                  OneDriveBackupStatusReporting.cachedSettings = buffer;
                }
              }
            }
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "odm - Exception get settings");
          }
        }
        return OneDriveBackupStatusReporting.cachedSettings != null && OneDriveBackupStatusReporting.cachedSettings.Length != 0 ? OneDriveBackupStatusReporting.cachedSettings : (byte[]) null;
      }
    }

    private static void SetBackupSettingsBytes(byte[] updatedSettings)
    {
      lock (OneDriveBackupStatusReporting.lockCachedSettings)
      {
        byte[] buffer = updatedSettings == null ? new byte[0] : updatedSettings;
        OneDriveBackupStatusReporting.cachedSettings = buffer;
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            using (Stream stream = nativeMediaStorage.OpenFile(OneDriveBackupStatusReporting.OneDriveBackupSettingsFile, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
              stream.Write(buffer, 0, buffer.Length);
              stream.SetLength((long) buffer.Length);
              stream.Flush();
            }
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "odm - Exception set settings");
        }
      }
    }

    public enum BackupStateDisplayOptions
    {
      Normal,
      ShouldDisplay,
    }

    [DataContract]
    private class OneDriveBackupStatus
    {
      [DataMember(Name = "backupId")]
      public string BackupId { get; set; }

      [DataMember(Name = "startTime")]
      public long StartTimestampTicks { get; set; }

      [DataMember(Name = "startedByUser")]
      public bool StartedByUser { get; set; }

      [DataMember(Name = "endTime")]
      public long EndTimestampTicks { get; set; }

      [DataMember(Name = "lastState")]
      public OneDriveLastBackupState LastState { get; set; }
    }
  }
}
