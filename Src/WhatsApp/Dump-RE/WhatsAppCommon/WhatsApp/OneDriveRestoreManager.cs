// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveRestoreManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WhatsApp.Events;
using WhatsAppNative;
using Windows.Security.Authentication.OnlineId;

#nullable disable
namespace WhatsApp
{
  public sealed class OneDriveRestoreManager : IDisposable
  {
    private static object initLock = new object();
    private static OneDriveRestoreManager instance;
    private object threadLock = new object();
    private object eventLock = new object();
    private Task workerTask;
    private CancellationTokenSource cancellationSource;
    private OneDriveBkupRestStopReason stopReason;
    private OneDriveRestoreStopError stopError;
    private OneDriveRestoreProcessor processor;
    private OneDriveRestoreState restoreState;
    private OneDriveBkupRestTrigger? startUpTrigger;
    private BackupProperties backupProperties;
    private long progressValue;
    private long progressMaximum;
    private ConnectionType? lastConnectionType;
    private bool? lastCallActiveStatus;
    private IDisposable voipCallStartedSub;
    private IDisposable voipCallEndedSub;

    public event EventHandler<OneDriveRestoreState> StateChanged;

    public event EventHandler<BackupProperties> BackupPropertiesChanged;

    public event EventHandler<long> ProgressValueChanged;

    public event EventHandler<long> ProgressMaximumChanged;

    public event EventHandler<bool> IsProgressIndeterminateChanged;

    public event EventHandler<OneDriveBkupRestTrigger> RestoreStarted;

    public event EventHandler<BkupRestStoppedEventArgs> RestoreStopped;

    public static OneDriveRestoreManager Instance
    {
      get
      {
        return Utils.LazyInit<OneDriveRestoreManager>(ref OneDriveRestoreManager.instance, (Func<OneDriveRestoreManager>) (() => new OneDriveRestoreManager()), OneDriveRestoreManager.initLock);
      }
    }

    public OneDriveRestoreState State
    {
      get
      {
        lock (this.eventLock)
          return this.restoreState;
      }
    }

    public OneDriveBkupRestStopReason StopReason
    {
      get
      {
        lock (this.eventLock)
          return this.stopReason;
      }
    }

    public OneDriveRestoreStopError StopError
    {
      get
      {
        lock (this.eventLock)
          return this.stopError;
      }
    }

    public BackupProperties BackupProperties
    {
      get
      {
        lock (this.eventLock)
          return this.backupProperties;
      }
      private set
      {
        lock (this.eventLock)
        {
          this.backupProperties = value;
          this.OnBackupPropertiesChanged(value);
        }
      }
    }

    public long ProgressValue
    {
      get
      {
        lock (this.eventLock)
          return this.progressValue >= 0L ? this.progressValue : 0L;
      }
      private set
      {
        lock (this.eventLock)
        {
          if (this.progressValue == value)
            return;
          int num = this.progressValue < 0L != value < 0L ? 1 : 0;
          this.progressValue = value;
          this.OnProgressValueChanged(value);
          if (num == 0)
            return;
          this.OnIsProgressIndeterminateChanged(this.progressValue < 0L);
        }
      }
    }

    public long ProgressMaximum
    {
      get
      {
        lock (this.eventLock)
          return this.progressMaximum;
      }
      private set
      {
        lock (this.eventLock)
        {
          if (this.progressMaximum == value)
            return;
          this.progressMaximum = value;
          this.OnProgressMaximumChanged(value);
        }
      }
    }

    public bool IsProgressIndeterminate
    {
      get
      {
        lock (this.eventLock)
          return OneDriveRestoreManager.IsRestoreStateIndeterminate(this.restoreState) || this.progressValue < 0L;
      }
    }

    public static bool IsRestoreIncomplete
    {
      get
      {
        using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
          return oneDriveManifest != null;
      }
    }

    private OneDriveRestoreManager()
    {
      this.restoreState = OneDriveRestoreState.Idle;
      this.processor = new OneDriveRestoreProcessor();
      this.RefreshBackupProperties();
      DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(this.DeviceNetworkInformation_NetworkAvailabilityChanged);
      this.voipCallStartedSub = VoipHandler.CallStartedSubject.Subscribe<Unit>((Action<Unit>) (u => this.OnCallStateChanged(true)));
      this.voipCallEndedSub = VoipHandler.CallEndedSubject.Subscribe<WaCallEndedEventArgs>((Action<WaCallEndedEventArgs>) (u => this.OnCallStateChanged(false)));
      Settings.GetSettingsChangedObservable(new Settings.Key[2]
      {
        Settings.Key.OneDriveRestoreNetwork,
        Settings.Key.LoginFailed
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>(new Action<Settings.Key>(this.OnSettingsChanged));
    }

    public void Dispose()
    {
      DeviceNetworkInformation.NetworkAvailabilityChanged -= new EventHandler<NetworkNotificationEventArgs>(this.DeviceNetworkInformation_NetworkAvailabilityChanged);
      this.voipCallStartedSub?.Dispose();
      this.voipCallEndedSub?.Dispose();
    }

    public static bool OnAppActivated()
    {
      Log.d("odm", "OneDriveRestoreManager.OnAppActivated");
      bool flag = false;
      if (Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified)
      {
        if (OneDriveRestoreManager.IsRestoreIncomplete)
        {
          flag = true;
          Log.d("odm", "OneDriveRestoreManager.OnAppActivated");
          OneDriveRestoreManager.Instance.SetTrigger(OneDriveBkupRestTrigger.ForegroundResume);
        }
      }
      else
      {
        flag = true;
        IDisposable stopSettingObservation = (IDisposable) null;
        stopSettingObservation = Settings.GetSettingsChangedObservable(new Settings.Key[1]
        {
          Settings.Key.PhoneNumberVerificationState
        }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>((Action<Settings.Key>) (k =>
        {
          if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified)
            return;
          try
          {
            Log.l("odm", "media restore after phone number verification");
            if (OneDriveRestoreManager.IsRestoreIncomplete)
              OneDriveRestoreManager.Instance.Start(OneDriveBkupRestTrigger.UserInteraction);
            stopSettingObservation.SafeDispose();
          }
          catch (Exception ex)
          {
          }
        }));
      }
      return flag;
    }

    public static void OnAppDeactivated()
    {
      Log.d("odm", "OneDriveRestoreManager.OnAppDeactivated");
      bool flag = false;
      lock (OneDriveRestoreManager.initLock)
        flag = OneDriveRestoreManager.instance != null;
      if (!flag)
        return;
      try
      {
        OneDriveRestoreManager.Instance.Stop(OneDriveRestoreStopError.StoppedByOS);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception stopping restore on App deactivation");
      }
    }

    private void DeviceNetworkInformation_NetworkAvailabilityChanged(
      object sender,
      NetworkNotificationEventArgs e)
    {
      Log.l("odm", "restore detected network availability change: {0} {1} {2}", (object) e.NetworkInterface.InterfaceType.ToString(), (object) e.NetworkInterface.InterfaceSubtype.ToString(), (object) e.NotificationType.ToString());
      lock (this.threadLock)
      {
        ConnectionType? nullable1 = new ConnectionType?();
        switch (e.NetworkInterface.InterfaceType)
        {
          case NetworkInterfaceType.Wireless80211:
            nullable1 = new ConnectionType?(ConnectionType.Wifi);
            break;
          case NetworkInterfaceType.MobileBroadbandGsm:
          case NetworkInterfaceType.MobileBroadbandCdma:
            nullable1 = new ConnectionType?(ConnectionType.Cellular_3G);
            break;
        }
        if (nullable1.HasValue)
        {
          if (e.NotificationType == NetworkNotificationType.InterfaceConnected)
          {
            if (this.lastConnectionType.HasValue)
            {
              if (this.lastConnectionType.Value == ConnectionType.Cellular_3G)
              {
                if (nullable1.Value == ConnectionType.Wifi)
                  this.lastConnectionType = nullable1;
              }
            }
            else
              this.lastConnectionType = nullable1;
          }
          else if (e.NotificationType == NetworkNotificationType.InterfaceDisconnected)
          {
            ConnectionType? lastConnectionType = this.lastConnectionType;
            ConnectionType? nullable2 = nullable1;
            if ((lastConnectionType.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (lastConnectionType.HasValue == nullable2.HasValue ? 1 : 0) : 0) != 0)
              this.lastConnectionType = new ConnectionType?();
          }
        }
      }
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.HandleNetworkChange()));
    }

    private void OnCallStateChanged(bool started)
    {
      Log.l("odm", "detected call state change: {0}", started ? (object) nameof (started) : (object) "ended");
      lock (this.threadLock)
        this.lastCallActiveStatus = new bool?(started);
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.HandleNetworkChange()));
    }

    private void OnSettingsChanged(Settings.Key changedItem)
    {
      switch (changedItem)
      {
        case Settings.Key.LoginFailed:
          if (!Settings.LoginFailed)
            break;
          try
          {
            bool flag = false;
            lock (OneDriveRestoreManager.initLock)
            {
              if (OneDriveRestoreManager.instance != null)
              {
                if (OneDriveRestoreManager.instance.State != OneDriveRestoreState.Idle)
                  flag = true;
              }
            }
            if (!flag)
              break;
            Log.l("odm", "triggering restore abort due to login failure");
            this.StopImpl(OneDriveBkupRestStopReason.Abort, OneDriveRestoreStopError.LoginFailure);
            break;
          }
          catch (Exception ex)
          {
            break;
          }
        case Settings.Key.OneDriveRestoreNetwork:
          Log.l("odm", "detected network settings change");
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.HandleNetworkChange()));
          break;
      }
    }

    private void HandleNetworkChange()
    {
      lock (this.threadLock)
      {
        OneDriveRestoreState state = this.State;
        bool flag;
        switch (state)
        {
          case OneDriveRestoreState.Authenticating:
          case OneDriveRestoreState.SynchronizingManifest:
          case OneDriveRestoreState.RestoringMedia:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        if (this.workerTask != null & flag && !this.CheckCloudRestoreNetwork())
        {
          Log.l("odm", "stopping in-progress cloud restore due to network change");
          this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
        }
        else
        {
          if (state != OneDriveRestoreState.Idle || this.stopReason != OneDriveBkupRestStopReason.NetworkChange && this.stopReason != OneDriveBkupRestStopReason.StopError || !OneDriveRestoreManager.IsRestoreIncomplete || !this.CheckCloudRestoreNetwork())
            return;
          Log.l("odm", "restarting in-progress cloud restore due to network change");
          this.Start(AppState.IsBackgroundAgent ? OneDriveBkupRestTrigger.BackgroundResume : OneDriveBkupRestTrigger.ForegroundResume);
        }
      }
    }

    public void Start(OneDriveBkupRestTrigger restoreTrigger)
    {
      Log.l("odm", "restore manager start (trigger={0})", (object) restoreTrigger.ToString());
      lock (this.threadLock)
      {
        if (Settings.LoginFailed)
        {
          Log.l("odm", "cannot start due to login failure");
        }
        else
        {
          if (this.workerTask != null)
          {
            Log.l("odm", "restore manager restarted while already running");
            if (restoreTrigger == OneDriveBkupRestTrigger.BackgroundAgent || restoreTrigger == OneDriveBkupRestTrigger.BackgroundAgentShort)
            {
              Log.l("odm", "background agent re-invoke case, ignoring");
              return;
            }
          }
          if (this.State != OneDriveRestoreState.Idle)
          {
            Log.l("odm", "cannot start, restore in progress");
            throw new InvalidOperationException("restore in progress");
          }
          if (this.workerTask != null && !this.workerTask.IsCanceled && !this.workerTask.IsCompleted)
          {
            Log.l("odm", "cannot start, worker task is running");
            throw new InvalidOperationException("worker task is running");
          }
          this.cancellationSource = new CancellationTokenSource();
          CancellationToken cancellationToken = this.cancellationSource.Token;
          this.stopReason = OneDriveBkupRestStopReason.Completed;
          this.stopError = OneDriveRestoreStopError.None;
          this.workerTask = Task.Run((Func<Task>) (async () =>
          {
            Restore fsEvent = (Restore) null;
            long cloudRestoreStartTime = 0;
            Log.l("odm", "worker task starting");
            this.OnRestoreStarted(restoreTrigger);
            this.SetNewRestoreState(OneDriveRestoreState.Starting, new long?(0L));
            try
            {
              cancellationToken.ThrowIfCancellationRequested();
              fsEvent = this.CreateRestoreEvent();
              if (AppState.BatterySaverEnabled)
              {
                if (this.cancellationSource.IsCancellationRequested)
                  Log.l("cancelled");
                Log.l("odm", "restore worker task cancelled because of battery state");
                throw new OneDriveBatterySaverStatexception("Battery state too low to start restore");
              }
              if (this.CheckCloudRestoreNetwork())
              {
                cloudRestoreStartTime = DateTime.Now.Ticks;
                this.processor.ResetCounters();
                this.RefreshBackupProperties();
                await this.ProcessCloudRestore(restoreTrigger, cancellationToken, fsEvent);
                if (!this.VerifyRestoreComplete())
                  throw new Exception("Media restore failed");
                this.processor.ClearTemporaryData();
              }
              else
                this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
            }
            catch (ServiceException ex)
            {
              StringBuilder stringBuilder = new StringBuilder();
              for (Error error = ex.Error; error != null; error = error.InnerError)
              {
                if (stringBuilder.Length > 0)
                  stringBuilder.Append(", ");
                stringBuilder.Append((object) error);
              }
              Log.l("odm", "worker task OneDrive exception: {0}", (object) stringBuilder.ToString());
              Log.LogException((Exception) ex, "odm");
              if (ex.IsMatch("unauthenticated"))
              {
                fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
                this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.Unauthenticated);
              }
              else if (ex.IsMatch("serviceNotAvailable"))
              {
                fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.BACKUP_SERVER_NOT_WORKING);
                this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.ServiceNotAvailable);
              }
              else if (ex.IsMatch("generalException") && !this.CheckCloudRestoreNetwork())
                this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
              else if (((IEnumerable<string>) OneDriveBackupManager.AuthenticationErrorCodes).Contains<string>(ex.Error?.Code ?? ""))
              {
                fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
                this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.Unauthenticated);
              }
              else
                this.StopImpl(OneDriveBkupRestStopReason.StopError);
            }
            catch (SqliteException ex)
            {
              Log.l("odm", "worker task DB exception");
              Log.LogException((Exception) ex, "odm");
              if (this.cancellationSource.IsCancellationRequested)
                return;
              throw;
            }
            catch (OneDriveDiskFullException ex)
            {
              Log.l("odm", "worker task disk full exception");
              Log.LogException((Exception) ex, "odm");
              if (this.cancellationSource.IsCancellationRequested)
                return;
              fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.LOCAL_STORAGE_IS_FULL);
              this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.LocalDiskFull);
            }
            catch (OneDriveBatterySaverStatexception ex)
            {
              Log.l("odm", "worker battery state exception");
              Log.LogException((Exception) ex, "odm");
              if (this.cancellationSource.IsCancellationRequested)
                return;
              fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.UNKNOWN_ERROR);
              this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.StoppedByBattery);
            }
            catch (Exception ex)
            {
              Log.l("odm", "worker task exception");
              switch (ex)
              {
                case OperationCanceledException _:
                case TaskCanceledException _:
                  if (this.cancellationSource.IsCancellationRequested)
                    break;
                  goto default;
                default:
                  Log.LogException(ex, "odm");
                  break;
              }
              if (!this.cancellationSource.IsCancellationRequested && AppState.GetUserConnectionType() == ConnectionType.Disconnected)
              {
                Log.l("odm", "network disconnected during restore");
                this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
              }
              else
              {
                if (this.cancellationSource.IsCancellationRequested)
                  return;
                if (ex.GetHResult() == 2147942512U)
                {
                  fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.LOCAL_STORAGE_IS_FULL);
                  this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.LocalDiskFull);
                }
                else
                  this.StopImpl(OneDriveBkupRestStopReason.StopError);
              }
            }
            finally
            {
              Log.l("odm", "restore worker task complete {0}, {1}, {2}", (object) this.cancellationSource.IsCancellationRequested, (object) this.stopReason, (object) this.stopError);
              long ticks = DateTime.Now.Ticks;
              lock (this.threadLock)
              {
                this.workerTask = (Task) null;
                this.cancellationSource.Dispose();
                this.cancellationSource = (CancellationTokenSource) null;
                if (fsEvent != null)
                {
                  if (cloudRestoreStartTime > 0L && ticks > cloudRestoreStartTime)
                    fsEvent.backupRestoreT = new long?((ticks - cloudRestoreStartTime) / 10000L);
                  this.SubmitRestoreEvent(fsEvent);
                }
                if (this.stopReason != OneDriveBkupRestStopReason.Abort)
                {
                  if (this.stopReason != OneDriveBkupRestStopReason.AbortError)
                    goto label_53;
                }
                try
                {
                  Log.l("odm", "clearing incomplete media restore state");
                  this.processor.ClearTemporaryData();
                }
                catch (Exception ex)
                {
                  Log.l("odm", "worker task cleanup exception");
                  Log.LogException(ex, "odm");
                }
              }
label_53:
              if (this.stopError != OneDriveRestoreStopError.StoppedByOS)
                this.RefreshBackupProperties();
              this.SetNewRestoreState(OneDriveRestoreState.Idle);
              this.OnRestoreStopped(this.stopReason, new OneDriveBkupRestTrigger?(restoreTrigger));
            }
          }), cancellationToken);
        }
      }
    }

    private bool CheckCloudRestoreNetwork()
    {
      AutoDownloadSetting driveRestoreNetwork = Settings.OneDriveRestoreNetwork;
      ConnectionType connectionType;
      bool flag1;
      lock (this.threadLock)
      {
        connectionType = !this.lastConnectionType.HasValue ? AppState.GetUserConnectionType() : this.lastConnectionType.Value;
        flag1 = !this.lastCallActiveStatus.HasValue ? Voip.IsInCall : this.lastCallActiveStatus.Value;
      }
      bool flag2;
      if (flag1)
      {
        flag2 = false;
      }
      else
      {
        switch (connectionType)
        {
          case ConnectionType.Wifi:
            flag2 = (driveRestoreNetwork & AutoDownloadSetting.EnabledOnWifi) != 0;
            break;
          case ConnectionType.Cellular_2G:
          case ConnectionType.Cellular_3G:
            flag2 = (driveRestoreNetwork & AutoDownloadSetting.EnabledOnData) != 0;
            break;
          default:
            flag2 = false;
            break;
        }
      }
      if (!flag2)
      {
        if (flag1)
          Log.l("odm", "restore not allowed due to VoIP call");
        else
          Log.l("odm", "restore not allowed due to network settings {0} {1}", (object) driveRestoreNetwork, (object) connectionType);
      }
      return flag2;
    }

    private Restore CreateRestoreEvent()
    {
      Restore restoreEvent = new Restore();
      switch (AppState.GetUserConnectionType())
      {
        case ConnectionType.Wifi:
          restoreEvent.backupRestoreIsWifi = new long?(1L);
          break;
        case ConnectionType.Cellular_2G:
        case ConnectionType.Cellular_3G:
          restoreEvent.backupRestoreIsWifi = new long?(0L);
          break;
      }
      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
      {
        if (oneDriveManifest.Exists)
        {
          BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
          if (backupProperties != null)
          {
            restoreEvent.backupRestoreIncludeVideos = new bool?(backupProperties.Settings.IncludeVideos);
            restoreEvent.backupRestoreChatdbSize = new double?((double) oneDriveManifest.GetRemoteDatabaseSizeToRestore());
            restoreEvent.backupRestoreMediaSize = new double?((double) oneDriveManifest.GetRemoteMediaSizeToRestore());
            restoreEvent.backupRestoreMediaFileCount = new double?((double) oneDriveManifest.GetRemoteMediaFileCount());
            restoreEvent.backupRestoreTotalSize = new double?((double) backupProperties.Size);
          }
        }
      }
      return restoreEvent;
    }

    private void SubmitRestoreEvent(Restore fsEvent)
    {
      ConnectionType userConnectionType = AppState.GetUserConnectionType();
      OneDriveBkupRestStopReason stopReason;
      lock (this.threadLock)
        stopReason = this.stopReason;
      if (stopReason == OneDriveBkupRestStopReason.Abort || stopReason == OneDriveBkupRestStopReason.Stop)
        return;
      if (userConnectionType == ConnectionType.Cellular_2G || userConnectionType == ConnectionType.Cellular_3G)
        fsEvent.backupRestoreIsWifi = new long?(0L);
      fsEvent.backupRestoreTransferSize = new double?((double) this.ProgressValue);
      fsEvent.backupRestoreNetworkRequestCount = new double?((double) this.processor.RequestCount);
      switch (stopReason)
      {
        case OneDriveBkupRestStopReason.Completed:
          fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.OK);
          break;
        case OneDriveBkupRestStopReason.StopError:
        case OneDriveBkupRestStopReason.AbortError:
          if (!fsEvent.backupRestoreResult.HasValue)
          {
            fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.UNKNOWN_ERROR);
            break;
          }
          break;
        case OneDriveBkupRestStopReason.NetworkChange:
          int driveRestoreNetwork = (int) Settings.OneDriveRestoreNetwork;
          bool flag = (driveRestoreNetwork & 2) != 0;
          fsEvent.backupRestoreResult = (driveRestoreNetwork & 1) == 0 || flag ? new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.DATA_CONNECTION_REQUIRED_BUT_MISSING) : new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.WIFI_REQUIRED_BUT_MISSING);
          break;
      }
      fsEvent.SaveEvent();
    }

    public void SetTrigger(OneDriveBkupRestTrigger restoreTrigger)
    {
      lock (OneDriveRestoreManager.initLock)
        this.startUpTrigger = new OneDriveBkupRestTrigger?(restoreTrigger);
      Log.l("odm", "restore manager set trigger={0}", (object) restoreTrigger.ToString());
    }

    public bool MaybeStart()
    {
      OneDriveBkupRestTrigger? nullable = new OneDriveBkupRestTrigger?();
      lock (OneDriveRestoreManager.initLock)
      {
        if (this.startUpTrigger.HasValue)
        {
          nullable = this.startUpTrigger;
          this.startUpTrigger = new OneDriveBkupRestTrigger?();
        }
      }
      if (nullable.HasValue)
      {
        try
        {
          this.Start(nullable.Value);
        }
        catch (Exception ex)
        {
        }
      }
      else
        Log.l("odm", "No need to start restore");
      return !nullable.HasValue;
    }

    public void Stop(OneDriveRestoreStopError error)
    {
      this.Terminate(OneDriveBkupRestStopReason.Stop, error);
    }

    public void Abort(OneDriveRestoreStopError error)
    {
      this.Terminate(OneDriveBkupRestStopReason.Abort, error);
    }

    private void Terminate(OneDriveBkupRestStopReason reason, OneDriveRestoreStopError error)
    {
      bool flag = false;
      lock (OneDriveRestoreManager.initLock)
      {
        OneDriveBkupRestTrigger? startUpTrigger = this.startUpTrigger;
        this.startUpTrigger = new OneDriveBkupRestTrigger?();
        if (error == OneDriveRestoreStopError.StoppedByOS)
        {
          if (OneDriveRestoreManager.instance != null)
          {
            if (OneDriveRestoreManager.instance.State == OneDriveRestoreState.Idle)
              flag = true;
          }
        }
      }
      if (flag)
        Log.l("odm", "restore manager idle on app shutdown, ignoring shutdown");
      else
        this.StopImpl(reason, error);
    }

    private void StopImpl(OneDriveBkupRestStopReason reason, OneDriveRestoreStopError error = OneDriveRestoreStopError.Unspecified)
    {
      Log.l("odm", "restore manager stopping (reason={0}, error={1})", (object) reason.ToString(), (object) error.ToString());
      if (error == OneDriveRestoreStopError.StoppedByTimeout)
      {
        lock (this.threadLock)
        {
          if (this.workerTask != null)
          {
            if (this.cancellationSource != null)
              goto label_8;
          }
          Log.l("odm", "processing already stopped before timeout");
          return;
        }
      }
label_8:
      lock (this.threadLock)
      {
        if ((reason == OneDriveBkupRestStopReason.Abort || reason == OneDriveBkupRestStopReason.AbortError) && this.State == OneDriveRestoreState.Idle && OneDriveRestoreManager.IsRestoreIncomplete)
        {
          this.processor.ClearTemporaryData();
          this.stopReason = reason;
          this.stopError = error;
          this.OnRestoreStopped(this.stopReason);
        }
        else if (this.workerTask == null || this.cancellationSource == null)
        {
          if (error != OneDriveRestoreStopError.StoppedByTimeout)
            throw new InvalidOperationException("cancellable task not running");
          Log.l("odm", "processing already stopped during timeout detection");
        }
        else
        {
          Log.l("odm", "cancelling task");
          this.stopReason = reason;
          this.stopError = error;
          this.cancellationSource.Cancel();
        }
      }
    }

    private async Task ProcessCloudRestore(
      OneDriveBkupRestTrigger restoreTrigger,
      CancellationToken cancellationToken,
      Restore fsEvent)
    {
      Log.l("odm", "starting restore process");
      this.processor.CredentialPrompt = AppState.IsBackgroundAgent ? (CredentialPromptType) 2 : (CredentialPromptType) 0;
      if (!this.processor.IsAuthenticated)
      {
        this.SetNewRestoreState(OneDriveRestoreState.Authenticating);
        if (!await this.processor.Authenticate())
        {
          fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
          this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveRestoreStopError.Unauthenticated);
        }
      }
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewRestoreState(OneDriveRestoreState.SynchronizingManifest);
      this.ApplyManifestRenameJournal(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
      int num = await this.SynchronizeManifest(cancellationToken) ? 1 : 0;
      cancellationToken.ThrowIfCancellationRequested();
      this.RefreshBackupProperties();
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewRestoreState(OneDriveRestoreState.RestoringMedia);
      await this.RestoreMedia(restoreTrigger, cancellationToken);
      Log.l("odm", "restore process complete");
      this.SetNewRestoreState(OneDriveRestoreState.Complete);
      this.RefreshBackupProperties();
    }

    private void ApplyManifestRenameJournal(CancellationToken cancellationToken)
    {
      using (OneDriveManifest manifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        manifest.Open();
        manifest.PrepareMediaRenameJournalTable();
        foreach (MediaRenameJournalEntry renameJournalEntry in manifest.GetMediaRenameJournal())
        {
          MediaRenameJournalEntry entry = renameJournalEntry;
          object[] objArray = new object[2];
          FileRef fileRef = entry.OldFileReference;
          objArray[0] = (object) fileRef.ToString();
          fileRef = entry.NewFileReference;
          objArray[1] = (object) fileRef.ToString();
          Log.l("onedrive", "journaled media file change from {0} to {1}", objArray);
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            db.LocalFileRename(entry.OldFileReference, entry.NewFileReference, entry.Sha1Hash);
            manifest.ApplyMediaRenameFromJournal(entry);
          }));
        }
      }
    }

    private async Task<bool> SynchronizeManifest(CancellationToken cancellationToken)
    {
      bool flag = false;
      Log.l("odm", "updating local manifest");
      using (OneDriveManifest manifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        manifest.NormalizeManifest();
        cancellationToken.ThrowIfCancellationRequested();
        flag = await this.processor.SynchronizeManifest(manifest, cancellationToken);
      }
      Log.l("odm", "local manifest: {0}", flag ? (object) "updated" : (object) "unchanged");
      return flag;
    }

    private async Task RestoreMedia(
      OneDriveBkupRestTrigger restoreTrigger,
      CancellationToken cancellationToken)
    {
      long progressBase = this.ProgressValue;
      Progress<long> progress = new Progress<long>((Action<long>) (p => this.ProgressValue = progressBase + p));
      using (OneDriveManifest manifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        manifest.Open();
        await this.processor.RestoreMedia(manifest, restoreTrigger, cancellationToken, (IProgress<long>) progress);
      }
    }

    private bool VerifyRestoreComplete()
    {
      using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        oneDriveManifest.Open();
        List<RemoteMediaFile> restore = oneDriveManifest.RemoteMediaFilesToRestore(1);
        if (restore.Count == 0)
        {
          Log.l("odm", "all media has been restored");
          return true;
        }
        Log.l("odm", "restore not complete, {0} (or more) media files not restored ", (object) restore.Count);
        return false;
      }
    }

    public void RefreshBackupProperties()
    {
      BackupProperties backupProperties = (BackupProperties) null;
      long num = 0;
      using (OneDriveManifest oneDriveManifest = OneDriveRestoreProcessor.RemoteBackupManifest())
      {
        if (oneDriveManifest != null)
        {
          oneDriveManifest.Open();
          backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
          num = oneDriveManifest.GetRemoteMediaSizeToRestore();
        }
      }
      if (backupProperties == null)
        backupProperties = new BackupProperties();
      this.BackupProperties = backupProperties;
      this.ProgressValue = backupProperties.RestoredSize;
      if (backupProperties.RestoredSize <= 0L && backupProperties.RestoreSizeEstimate.HasValue)
        this.ProgressMaximum = backupProperties.RestoreSizeEstimate.Value;
      else
        this.ProgressMaximum = num + backupProperties.RestoredSize;
    }

    private void SetNewRestoreState(
      OneDriveRestoreState state,
      long? progressValue = null,
      long? progressMaximum = null)
    {
      lock (this.eventLock)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        int num1 = OneDriveRestoreManager.IsRestoreStateIndeterminate(this.restoreState) ? 1 : (this.progressValue < 0L ? 1 : 0);
        if (this.restoreState != state)
        {
          this.restoreState = state;
          flag1 = true;
        }
        if (progressValue.HasValue && this.progressValue != progressValue.Value)
        {
          this.progressValue = progressValue.Value;
          flag2 = true;
        }
        if (progressMaximum.HasValue && this.progressMaximum != progressMaximum.Value)
        {
          this.progressMaximum = progressMaximum.Value;
          flag3 = true;
        }
        bool e = OneDriveRestoreManager.IsRestoreStateIndeterminate(this.restoreState) || this.progressValue < 0L;
        if (flag1)
          this.OnStateChanged(this.restoreState);
        if (flag2)
          this.OnProgressValueChanged(this.progressValue);
        if (flag3)
          this.OnProgressMaximumChanged(this.progressMaximum);
        int num2 = e ? 1 : 0;
        if (num1 == num2)
          return;
        this.OnIsProgressIndeterminateChanged(e);
      }
    }

    private static bool IsRestoreStateIndeterminate(OneDriveRestoreState state)
    {
      return state != OneDriveRestoreState.RestoringMedia;
    }

    private void OnStateChanged(OneDriveRestoreState e)
    {
      EventHandler<OneDriveRestoreState> stateChanged = this.StateChanged;
      if (stateChanged == null)
        return;
      stateChanged((object) this, e);
    }

    private void OnBackupPropertiesChanged(BackupProperties e)
    {
      EventHandler<BackupProperties> propertiesChanged = this.BackupPropertiesChanged;
      if (propertiesChanged == null)
        return;
      propertiesChanged((object) this, e);
    }

    private void OnProgressValueChanged(long e)
    {
      EventHandler<long> progressValueChanged = this.ProgressValueChanged;
      if (progressValueChanged == null)
        return;
      progressValueChanged((object) this, e);
    }

    private void OnProgressMaximumChanged(long e)
    {
      EventHandler<long> progressMaximumChanged = this.ProgressMaximumChanged;
      if (progressMaximumChanged == null)
        return;
      progressMaximumChanged((object) this, e);
    }

    private void OnIsProgressIndeterminateChanged(bool e)
    {
      EventHandler<bool> indeterminateChanged = this.IsProgressIndeterminateChanged;
      if (indeterminateChanged == null)
        return;
      indeterminateChanged((object) this, e);
    }

    private void OnRestoreStarted(OneDriveBkupRestTrigger e)
    {
      EventHandler<OneDriveBkupRestTrigger> restoreStarted = this.RestoreStarted;
      if (restoreStarted == null)
        return;
      try
      {
        restoreStarted((object) this, e);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception processing OnRestoreStarted handler");
      }
    }

    private void OnRestoreStopped(
      OneDriveBkupRestStopReason reason,
      OneDriveBkupRestTrigger? startTrigger = null)
    {
      EventHandler<BkupRestStoppedEventArgs> restoreStopped = this.RestoreStopped;
      if (restoreStopped == null)
        return;
      BkupRestStoppedEventArgs e = new BkupRestStoppedEventArgs()
      {
        Reason = reason,
        StartTrigger = startTrigger
      };
      try
      {
        restoreStopped((object) this, e);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception processing OnRestoreStopped handler");
      }
    }
  }
}
