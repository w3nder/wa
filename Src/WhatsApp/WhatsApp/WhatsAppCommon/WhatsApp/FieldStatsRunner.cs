// Decompiled with JetBrains decompiler
// Type: WhatsApp.FieldStatsRunner
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Info;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using WhatsApp.Events;
using WhatsAppNative;


namespace WhatsApp
{
  public static class FieldStatsRunner
  {
    private static string LogHdr = "FS";
    private static WorkQueue worker = (WorkQueue) null;
    public static int LoginRetryCount;
    private static bool initialized;
    private static bool syncRequested = false;
    private static IFieldStats instance;
    private static bool throttleClbUpload = false;

    public static WorkQueue Worker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref FieldStatsRunner.worker, (Func<WorkQueue>) (() => new WorkQueue(flags: AppState.IsBackgroundAgent ? WorkQueue.StartFlags.Unpausable : WorkQueue.StartFlags.None, identifierString: nameof (FieldStatsRunner))));
      }
    }

    public static bool Initialized => FieldStatsRunner.initialized;

    static FieldStatsRunner() => FieldStatsRunner.initialized = false;

    internal static IFieldStats Instance
    {
      get
      {
        if (!FieldStatsRunner.initialized)
          FieldStatsRunner.Initialize();
        return FieldStatsRunner.instance;
      }
    }

    public static void FieldStatsAction(Action a) => FieldStatsRunner.Worker.Enqueue(a);

    public static void FieldStatsAction(Action<IFieldStats> a)
    {
      FieldStatsRunner.FieldStatsAction((Action) (() =>
      {
        if (FieldStatsRunner.syncRequested)
          Log.d(FieldStatsRunner.LogHdr, "Action requested after sync");
        a(FieldStatsRunner.Instance);
      }));
    }

    private static void Initialize()
    {
      try
      {
        if (FieldStatsRunner.initialized)
          return;
        Log.l(FieldStatsRunner.LogHdr, "Initializing field stats v2");
        FieldStatsRunner.ConfigObject Config = new FieldStatsRunner.ConfigObject();
        if (FieldStatsRunner.instance == null)
          FieldStatsRunner.instance = (IFieldStats) NativeInterfaces.CreateInstance<FieldStatsInterop>();
        FieldStatsRunner.instance.Start((IFSConfig) Config);
        Log.l(FieldStatsRunner.LogHdr, "Initialized field stats v2");
        FieldStatsRunner.initialized = true;
        FieldStatsRunner.syncRequested = false;
      }
      catch (Exception ex)
      {
        FieldStatsRunner.initialized = false;
      }
    }

    public static void CollectStats()
    {
      FieldStatsRunner.FieldStatsAction((Action) (() =>
      {
        FieldStatsRunner.ReportPushStats();
        FieldStatsRunner.ReportMemoryUsage();
        DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
        DateTime? statsCollectedUtc1 = Settings.LastDailyStatsCollectedUtc;
        TimeSpan timeSpan;
        if (statsCollectedUtc1.HasValue)
        {
          timeSpan = currentServerTimeUtc - statsCollectedUtc1.Value;
          if (timeSpan.TotalDays <= 1.0)
            goto label_5;
        }
        FieldStatsRunner.CollectDailyStats();
        if (Settings.CipherTextPlaceholderShown > 0)
        {
          new E2ePlaceholdersViewed()
          {
            decryptionPlaceholderViews = new long?((long) Settings.CipherTextPlaceholderShown)
          }.SaveEvent();
          Settings.CipherTextPlaceholderShown = 0;
        }
        Settings.LastDailyStatsCollectedUtc = new DateTime?(currentServerTimeUtc);
label_5:
        DateTime? statsCollectedUtc2 = Settings.LastWeeklyStatsCollectedUtc;
        if (statsCollectedUtc2.HasValue)
        {
          timeSpan = currentServerTimeUtc - statsCollectedUtc2.Value;
          if (timeSpan.TotalDays <= 7.0)
            goto label_8;
        }
        Settings.LastWeeklyStatsCollectedUtc = new DateTime?(currentServerTimeUtc);
label_8:
        DateTime? monthlyStatsCollectUtc = Settings.LastMonthlyStatsCollectUtc;
        if (monthlyStatsCollectUtc.HasValue)
        {
          timeSpan = currentServerTimeUtc - monthlyStatsCollectUtc.Value;
          if (timeSpan.TotalDays <= 30.0)
            return;
        }
        Settings.LastMonthlyStatsCollectUtc = new DateTime?(currentServerTimeUtc);
      }));
    }

    private static void CollectDailyStats()
    {
      Daily fsEvent = new Daily();
      CELL_INFO cellInfo = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.All);
      fsEvent.simMcc = new long?((long) cellInfo.Mcc);
      fsEvent.simMcc = new long?((long) cellInfo.Mnc);
      fsEvent.networkIsRoaming = new bool?(cellInfo.Roaming);
      Version osVersion = AppState.OSVersion;
      string str = osVersion.Major.ToString() + "." + (object) osVersion.Minor;
      if (str != null)
        fsEvent.osBuildNumber = str;
      string lang;
      string locale;
      AppState.GetLangAndLocale(out lang, out locale);
      fsEvent.languageCode = lang;
      fsEvent.locationCode = locale;
      fsEvent.receiptsEnabled = new bool?();
      fsEvent.backupSchedule = new wam_enum_backup_schedule?(OneDriveUtils.GetBackupSceduleForFieldStats());
      fsEvent.backupNetworkSetting = OneDriveUtils.GetBackupNetworkSettingForFieldStats();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        int num5 = 0;
        MessagesContext messagesContext = db;
        JidHelper.JidTypes[] includeTypes = new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Broadcast
        };
        int? limit = new int?();
        foreach (Conversation conversation in messagesContext.GetConversations(includeTypes, true, limit: limit))
        {
          if (conversation.IsArchived)
          {
            if (conversation.IsUserChat())
              ++num4;
            else
              ++num2;
          }
          else if (conversation.IsUserChat())
            ++num5;
          else if (conversation.IsGroup())
            ++num3;
          else
            ++num1;
        }
        fsEvent.individualChatCount = new long?((long) num5);
        fsEvent.individualArchivedChatCount = new long?((long) num4);
        fsEvent.groupChatCount = new long?((long) num3);
        fsEvent.groupArchivedChatCount = new long?((long) num2);
        fsEvent.broadcastChatCount = new long?((long) num1);
        fsEvent.broadcastArchivedChatCount = new long?();
        long messagesCount = db.GetMessagesCount();
        if (messagesCount >= 0L)
          fsEvent.dbMessagesCnt = new long?(messagesCount);
        fsEvent.dbMessagesUnindexedCnt = new long?();
        fsEvent.dbMessagesStarredCnt = new long?();
        fsEvent.dbMessagesIndexedPct = new double?();
      }));
      ContactStore.GetAllContacts().ObserveOn<Contact[]>((IScheduler) FieldStatsRunner.Worker).Subscribe<Contact[]>((Action<Contact[]>) (contacts =>
      {
        fsEvent.addressbookSize = new long?((long) contacts.Length);
        fsEvent.addressbookWhatsappSize = new long?();
      }));
      fsEvent.chatDatabaseSize = new long?();
      fsEvent.mediaFolderSize = new long?();
      fsEvent.mediaFolderFileCount = new long?();
      fsEvent.videoFolderSize = new long?();
      fsEvent.videoFolderFileCount = new long?();
      fsEvent.wpBatsaver = AppState.BatterySaverEnabled ? "1" : "0";
      string pushState = PushSystem.Instance.PushState;
      fsEvent.wpIsPushDaemonConnected = pushState;
      try
      {
        DateTime? lastBackupTime = Backup.GetLastBackupTime();
        if (lastBackupTime.HasValue)
          fsEvent.wpLastBackup = new long?((long) (int) (DateTime.Now - lastBackupTime.Value).TotalHours);
      }
      catch (Exception ex)
      {
      }
      fsEvent.wpScheduled = new bool?(AppState.IsVoipScheduled());
      AgentExitReason voipExitReason = (AgentExitReason) Settings.VoipExitReason;
      fsEvent.wpVoipExitReason = voipExitReason.ToString();
      string exceptionMessage = Settings.VoipExceptionMessage;
      if (!string.IsNullOrEmpty(exceptionMessage))
        fsEvent.wpVoipException = exceptionMessage;
      Func<AutoDownloadSetting, bool[]> func = (Func<AutoDownloadSetting, bool[]>) (settingsValue =>
      {
        bool[] source = new bool[3];
        for (int index = 0; index < ((IEnumerable<bool>) source).Count<bool>(); ++index)
          source[index] = false;
        if ((settingsValue & AutoDownloadSetting.EnabledOnWifi) != AutoDownloadSetting.Disabled)
          source[0] = true;
        if ((settingsValue & AutoDownloadSetting.EnabledOnData) != AutoDownloadSetting.Disabled)
          source[1] = true;
        if ((settingsValue & AutoDownloadSetting.EnabledWhileRoaming) != AutoDownloadSetting.Disabled)
          source[2] = true;
        return source;
      });
      bool[] flagArray1 = func(Settings.AutoDownloadAudio);
      fsEvent.autoDlAudioWifi = new bool?(flagArray1[0]);
      fsEvent.autoDlAudioCellular = new bool?(flagArray1[1]);
      fsEvent.autoDlAudioRoaming = new bool?(flagArray1[2]);
      bool[] flagArray2 = func(Settings.AutoDownloadDocument);
      fsEvent.autoDlDocWifi = new bool?(flagArray2[0]);
      fsEvent.autoDlDocCellular = new bool?(flagArray2[1]);
      fsEvent.autoDlDocRoaming = new bool?(flagArray2[2]);
      bool[] flagArray3 = func(Settings.AutoDownloadImage);
      fsEvent.autoDlImageWifi = new bool?(flagArray3[0]);
      fsEvent.autoDlImageCellular = new bool?(flagArray3[1]);
      fsEvent.autoDlImageRoaming = new bool?(flagArray3[2]);
      bool[] flagArray4 = func(Settings.AutoDownloadVideo);
      fsEvent.autoDlVideoWifi = new bool?(flagArray4[0]);
      fsEvent.autoDlVideoCellular = new bool?(flagArray4[1]);
      fsEvent.autoDlVideoRoaming = new bool?(flagArray4[2]);
      fsEvent.SaveEvent();
    }

    private static void ReportMemoryUsage()
    {
      try
      {
        long total = DeviceStatus.ApplicationMemoryUsageLimit;
        if (total == 0L)
          return;
        long currentUsage = DeviceStatus.ApplicationCurrentMemoryUsage;
        long applicationPeakMemoryUsage = DeviceStatus.ApplicationPeakMemoryUsage;
        FieldStatsRunner.FieldStatsAction((Action) (() => new MemoryStat()
        {
          processType = (AppState.IsBackgroundAgent ? "BG" : "FG"),
          workingSetSize = new double?((double) currentUsage),
          workingSetPeakSize = new double?((double) total)
        }.SaveEvent()));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "field stats");
      }
    }

    private static void ReportPushStats()
    {
      FieldStatsRunner.FieldStatsAction((Action) (() =>
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (!storeForApplication.FileExists("stats"))
            return;
          Stream input;
          try
          {
            input = (Stream) storeForApplication.OpenFile("stats", FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
          }
          catch (Exception ex)
          {
            return;
          }
          if (input.Length < 16L)
            return;
          using (input)
          {
            using (BinaryReader binaryReader = new BinaryReader(input))
            {
              long num1 = binaryReader.ReadInt64();
              long num2 = binaryReader.ReadInt64();
              long num3 = num1 + num2;
              long num4 = 0;
              if (num3 != 0L)
                num4 = num2 * 100L / num3;
              new Wp8Dropped()
              {
                wp8TotalPushes = new double?((double) num1),
                wp8TotalDropped = new double?((double) num2),
                wp8TotalPctDropped = new double?((double) num4),
                wp8SessionDropped = new double?((double) Stats.DroppedThisSession)
              }.SaveEventSampled(20U);
            }
          }
        }
      }));
    }

    public static void TrySendStats(FieldStatsRunner.ForceLevel force = FieldStatsRunner.ForceLevel.None)
    {
      FieldStatsRunner.FieldStatsAction((Action) (() =>
      {
        Log.d(FieldStatsRunner.LogHdr, "TrySend {0}", (object) force);
        if ((force & FieldStatsRunner.ForceLevel.AlwaysSend) == FieldStatsRunner.ForceLevel.AlwaysSend)
        {
          try
          {
            FieldStatsRunner.Instance.RotateBuffer();
            Log.d(FieldStatsRunner.LogHdr, "Rotated Buffer");
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "rotate fieldstats");
          }
        }
        bool send = FieldStatsRunner.Instance.IsReadyToSend();
        Log.d(FieldStatsRunner.LogHdr, "Ready To Send {0}", (object) send);
        if (send)
        {
          IByteBuffer bb = (IByteBuffer) null;
          try
          {
            bb = FieldStatsRunner.Instance.LoadFile();
            Log.d(FieldStatsRunner.LogHdr, "Loaded data {0}", (object) bb.GetLength());
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "stats load");
          }
          if (bb != null && bb.GetLength() != 0)
          {
            byte[] buffer = bb.Get();
            bb.Reset();
            Log.l(FieldStatsRunner.LogHdr, "Sending {0} bytes of field stats", (object) buffer.Length);
            Action snap = (Action) (() => FieldStatsRunner.Instance.SendAck());
            Action onComplete = (Action) (() => FieldStatsRunner.FieldStatsAction(snap));
            try
            {
              AppState.ClientInstance.GetConnection().SendStats(buffer, onComplete);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "stats send");
            }
          }
          else
            Log.l(FieldStatsRunner.LogHdr, "Not sending field stats; nothing to send");
        }
        else
          Log.l(FieldStatsRunner.LogHdr, "Not sending field stats; last check was too recent");
      }));
    }

    public static long GetTime() => DateTime.UtcNow.ToUnixTime();

    public static void Shutdown()
    {
      if (!FieldStatsRunner.initialized && FieldStatsRunner.instance == null)
        return;
      Log.l(FieldStatsRunner.LogHdr, "Stopping field stats v2");
      try
      {
        FieldStatsRunner.instance.Stop();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "field stats shutdown");
      }
      FieldStatsRunner.instance = (IFieldStats) null;
      FieldStatsRunner.initialized = false;
      Log.l(FieldStatsRunner.LogHdr, "Stopped field stats v2");
    }

    public static void Sync()
    {
      if (!FieldStatsRunner.initialized && FieldStatsRunner.instance == null)
        return;
      Log.l(FieldStatsRunner.LogHdr, "Syncing field stats v2");
      try
      {
        FieldStatsRunner.instance.Stop();
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "field stats sync");
      }
      FieldStatsRunner.syncRequested = true;
      Log.l(FieldStatsRunner.LogHdr, "Synced field stats v2");
    }

    public static void InitializeNative()
    {
      NativeInterfaces.Misc.SetFieldStatsCallbacks((IFieldStatsManaged) new FieldStatsRunner.ManagedNativeWrapper());
    }

    private class ConfigObject : IFSConfig
    {
      private uint maxSendInterval = (uint) Math.Max(NonDbSettings.FieldStatsSendIntervalSecs, 300);

      public uint GetMaxSendInterval() => this.maxSendInterval;

      public int GetMaxBufferSize() => 8192;

      public ulong GetClientHash() => 0;

      public void GetKnownValues(IFieldStats stats)
      {
        FieldStatsRunner.initialized = true;
        Wam.SetPlatform(new wam_enum_platform_type?(wam_enum_platform_type.WP));
        string appVersion = AppState.GetAppVersion();
        if (appVersion != null)
          Wam.SetAppVersion(appVersion);
        Wam.SetAppIsBetaRelease(new bool?(true));
        Wam.SetAppBuild(new wam_enum_app_build_type?(wam_enum_app_build_type.BETA));
        Version osVersion = AppState.OSVersion;
        string str = osVersion.Major.ToString() + "." + (object) osVersion.Minor;
        if (str != null)
          Wam.SetOsVersion(str);
        if (DeviceStatus.DeviceName != null)
          Wam.SetDeviceName(DeviceStatus.DeviceName);
        try
        {
          CELL_INFO cellInfo = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.All);
          Wam.SetMcc(new long?((long) cellInfo.Mcc));
          Wam.SetMnc(new long?((long) cellInfo.Mnc));
          if (cellInfo.Is3GPlus)
            Wam.SetNetworkRadioTypeS("3G+");
          else if (cellInfo.Is2G)
            Wam.SetNetworkRadioTypeS("2G");
        }
        catch (Exception ex)
        {
        }
        try
        {
          int num = NetworkStateMonitor.IsWifiDataConnected() ? 1 : 0;
          Wam.SetNetworkIsWifi(new bool?(num != 0));
          wam_enum_radio_type? nullable = num != 0 ? new wam_enum_radio_type?(wam_enum_radio_type.WIFI_UNKNOWN) : NetworkStateMonitor.GetConnectedCellularFSType();
          if (nullable.HasValue)
            Wam.SetNetworkRadioType(nullable);
          Log.l("fieldstats", "Connection type {0}", (object) nullable);
        }
        catch (Exception ex)
        {
        }
        Utils.TimeSpentFieldStatsRecordOption spentRecordOption = (Utils.TimeSpentFieldStatsRecordOption) NonDbSettings.TimeSpentRecordOption;
        if (spentRecordOption != Utils.TimeSpentFieldStatsRecordOption.NoRecording)
        {
          wam_enum_user_activity_logging_method? nullable = new wam_enum_user_activity_logging_method?();
          if (spentRecordOption != Utils.TimeSpentFieldStatsRecordOption.BitArray)
          {
            if (spentRecordOption == Utils.TimeSpentFieldStatsRecordOption.SessionEvent)
              nullable = new wam_enum_user_activity_logging_method?(wam_enum_user_activity_logging_method.SESSION_END);
          }
          else
            nullable = new wam_enum_user_activity_logging_method?(wam_enum_user_activity_logging_method.BIT_ARRAY);
          if (nullable.HasValue)
            Wam.SetUserActivityLoggingMethod(nullable);
        }
        Wam.SetWpProcess(new wam_enum_wp_process?(AppState.IsBackgroundAgent ? wam_enum_wp_process.BACKGROUND : wam_enum_wp_process.FOREGROUND));
        Wam.SetDatacenter(NonDbSettings.LastDataCenterUsed);
      }
    }

    [Flags]
    public enum ForceLevel
    {
      None = 0,
      AlwaysSend = 1,
      IgnoreLengthCheck = 2,
    }

    public class ManagedNativeWrapper : IFieldStatsManaged
    {
      public void Invoke(IAction action)
      {
        if (action == null)
          return;
        FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (instance => action.Perform()));
      }

      public void Send(uint flags)
      {
        FieldStatsRunner.TrySendStats((FieldStatsRunner.ForceLevel) flags);
      }
    }
  }
}
