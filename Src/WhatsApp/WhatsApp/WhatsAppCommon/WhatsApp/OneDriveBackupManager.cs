// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveBackupManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Graph;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WhatsAppNative;
using Windows.Security.Authentication.OnlineId;


namespace WhatsApp
{
  public sealed class OneDriveBackupManager : IDisposable
  {
    private static object initLock = new object();
    private static OneDriveBackupManager instance;
    private object threadLock = new object();
    private object eventLock = new object();
    private Task workerTask;
    private CancellationTokenSource cancellationSource;
    private OneDriveBkupRestStopReason stopReason;
    private OneDriveBackupStopError stopError;
    private OneDriveBackupProcessor processor;
    private bool authenticationInProcess;
    private OneDriveBackupState backupState;
    private OneDriveBkupRestTrigger? startUpTrigger;
    private OneDriveBkupRestTrigger? activeBackupTrigger;
    private BackupProperties backupProperties;
    private long progressValue;
    private long progressMaximum;
    private ConnectionType? lastConnectionType;
    private bool? lastCallActiveStatus;
    private bool foregroundResumeAfterStop;
    private static readonly string localBackupInProgressMarker = Constants.IsoStorePath + "\\onedrive_backup.db";
    private IDisposable settingsSub;
    private IDisposable voipCallStartedSub;
    private IDisposable voipCallEndedSub;
    public static readonly string[] AuthenticationErrorCodes = new string[3]
    {
      "authenticationNeverOccured",
      "authenticationCancelled",
      "authenticationFailure"
    };

    public event EventHandler<bool> IsAuthenticationInProgressChanged;

    public event EventHandler<OneDriveBackupState> StateChanged;

    public event EventHandler<BackupProperties> BackupPropertiesChanged;

    public event EventHandler<long> ProgressValueChanged;

    public event EventHandler<long> ProgressMaximumChanged;

    public event EventHandler<bool> IsProgressIndeterminateChanged;

    public event EventHandler<OneDriveBkupRestTrigger> BackupStarted;

    public event EventHandler<BkupRestStoppedEventArgs> BackupStopped;

    public static OneDriveBackupManager Instance
    {
      get
      {
        return Utils.LazyInit<OneDriveBackupManager>(ref OneDriveBackupManager.instance, (Func<OneDriveBackupManager>) (() => new OneDriveBackupManager()), OneDriveBackupManager.initLock);
      }
    }

    public bool IsAuthenticationInProgress
    {
      get => this.authenticationInProcess;
      private set
      {
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (this.authenticationInProcess == value)
            return;
          this.authenticationInProcess = value;
          this.OnIsAuthenticationInProgressChanged(value);
        }));
      }
    }

    public OneDriveBackupState State
    {
      get
      {
        lock (this.eventLock)
          return this.backupState;
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

    public OneDriveBackupStopError StopError
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
          return OneDriveBackupManager.IsBackupStateIndeterminate(this.backupState) || this.progressValue < 0L;
      }
    }

    public static bool IsBackupIncomplete
    {
      get
      {
        bool backupIncomplete;
        using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
          backupIncomplete = oneDriveManifest.IsOneDriveBackupInProgress();
        if (!backupIncomplete)
          backupIncomplete = OneDriveBackupManager.IsLocalBackupInProgress();
        Log.d("odm", "IsBackupIncomplete {0}", (object) backupIncomplete);
        return backupIncomplete;
      }
    }

    private static bool IsLocalBackupInProgress()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(OneDriveBackupManager.localBackupInProgressMarker);
    }

    private void SetLocalBackupInProgress(bool inProgress)
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        bool flag = nativeMediaStorage.FileExists(OneDriveBackupManager.localBackupInProgressMarker);
        if (inProgress)
        {
          if (flag)
            return;
          Log.l("onedrive", "setting local db in-progress marker");
          nativeMediaStorage.OpenFile(OneDriveBackupManager.localBackupInProgressMarker, FileMode.Create, FileAccess.Read).Close();
        }
        else
        {
          if (!flag)
            return;
          Log.l("onedrive", "clearing local db in-progress marker");
          nativeMediaStorage.DeleteFile(OneDriveBackupManager.localBackupInProgressMarker);
        }
      }
    }

    private OneDriveBackupManager()
    {
      this.backupState = OneDriveBackupState.Idle;
      this.processor = new OneDriveBackupProcessor();
      this.RefreshBackupProperties();
      DeviceNetworkInformation.NetworkAvailabilityChanged += new EventHandler<NetworkNotificationEventArgs>(this.DeviceNetworkInformation_NetworkAvailabilityChanged);
      this.voipCallStartedSub = VoipHandler.CallStartedSubject.Subscribe<Unit>((Action<Unit>) (u => this.OnCallStateChanged(true)));
      this.voipCallEndedSub = VoipHandler.CallEndedSubject.Subscribe<WaCallEndedEventArgs>((Action<WaCallEndedEventArgs>) (u => this.OnCallStateChanged(false)));
      this.settingsSub = Settings.GetSettingsChangedObservable(new Settings.Key[2]
      {
        Settings.Key.OneDriveBackupNetwork,
        Settings.Key.LoginFailed
      }).ObserveOnDispatcher<Settings.Key>().Subscribe<Settings.Key>(new Action<Settings.Key>(this.OnSettingsChanged));
    }

    public void Dispose()
    {
      DeviceNetworkInformation.NetworkAvailabilityChanged -= new EventHandler<NetworkNotificationEventArgs>(this.DeviceNetworkInformation_NetworkAvailabilityChanged);
      this.voipCallStartedSub?.Dispose();
      this.voipCallEndedSub?.Dispose();
      this.settingsSub?.Dispose();
    }

    public static void OnAppActivated()
    {
      Log.d("odm", "OneDriveBackupManager.OnAppActivated");
      OneDriveBackupStatusReporting.MaybeMigrateBackupSettings();
      if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified || !OneDriveBackupManager.IsBackupIncomplete || Voip.IsInCall)
        return;
      OneDriveBackupManager.Instance.SetTrigger(OneDriveBkupRestTrigger.ForegroundResume);
    }

    public static void OnAppDeactivated()
    {
      Log.d("odm", "OneDriveBackupManager.OnAppDeactivated");
      try
      {
        bool flag = false;
        lock (OneDriveBackupManager.initLock)
          flag = OneDriveBackupManager.instance != null;
        if (!flag)
          return;
        OneDriveBackupManager.Instance.Stop(OneDriveBackupStopError.StoppedByOS);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception stopping OneDrive backup on App deactivation");
      }
    }

    public async Task<bool> AuthenticateUser(
      WAWebAccountProvider selectedProvider = null,
      bool reauthenticate = false)
    {
      Log.l("odm", "interactive user authenticate");
      try
      {
        this.IsAuthenticationInProgress = true;
        if (!reauthenticate)
          reauthenticate = Settings.OneDriveUserReauthenticate;
        if (reauthenticate || selectedProvider != null)
          await this.processor.Reset();
        if (!this.processor.IsAuthenticated)
        {
          if (!await this.processor.Authenticate(new CredentialPromptType?(reauthenticate ? (CredentialPromptType) 1 : (CredentialPromptType) 0), selectedProvider))
            return false;
        }
        bool success = await this.processor.QueryUserMetadata(CancellationToken.None);
        if (success)
        {
          await this.processor.QueryDriveMetadata(CancellationToken.None);
          Settings.OneDriveUserReauthenticate = false;
        }
        return success;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "odm: Exception authenticating user");
        return false;
      }
      finally
      {
        this.IsAuthenticationInProgress = false;
      }
    }

    public bool IsDbBackupPathInUse(string directory)
    {
      string str = directory.EndsWith("\\") ? directory : directory + "\\";
      bool flag = false;
      lock (this.threadLock)
      {
        switch (this.State)
        {
          case OneDriveBackupState.Idle:
          case OneDriveBackupState.LocalBackup:
            using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
            {
              if (oneDriveManifest.Exists)
              {
                if (oneDriveManifest.IsOneDriveBackupInProgress())
                {
                  using (List<LocalBackupFile>.Enumerator enumerator = oneDriveManifest.LocalDatabaseFilesToUpload().GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                    {
                      if (enumerator.Current.FileReference.ToAbsolutePath().StartsWith(str, StringComparison.InvariantCultureIgnoreCase))
                      {
                        flag = true;
                        break;
                      }
                    }
                    break;
                  }
                }
                else
                  break;
              }
              else
                break;
            }
          default:
            flag = true;
            break;
        }
      }
      return flag;
    }

    private void DeviceNetworkInformation_NetworkAvailabilityChanged(
      object sender,
      NetworkNotificationEventArgs e)
    {
      Log.l("odm", "backup detected network availability change: {0} {1} {2}", (object) e.NetworkInterface.InterfaceType.ToString(), (object) e.NetworkInterface.InterfaceSubtype.ToString(), (object) e.NotificationType.ToString());
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
            Log.l("odm", "triggering backup abort due to login failure");
            this.StopImpl(OneDriveBkupRestStopReason.Abort, OneDriveBackupStopError.LoginFailure);
            break;
          }
          catch (Exception ex)
          {
            break;
          }
        case Settings.Key.OneDriveBackupNetwork:
          Log.l("odm", "detected network settings change");
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.HandleNetworkChange()));
          break;
      }
    }

    private void HandleNetworkChange()
    {
      lock (this.threadLock)
      {
        OneDriveBackupState state = this.State;
        bool flag;
        switch (state)
        {
          case OneDriveBackupState.Authenticating:
          case OneDriveBackupState.PreparingBackupPath:
          case OneDriveBackupState.SynchronizingManifest:
          case OneDriveBackupState.UploadingDatabases:
          case OneDriveBackupState.UploadingMedia:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        if (this.workerTask != null & flag && !this.CheckCloudBackupNetwork())
        {
          Log.l("odm", "stopping in-progress cloud backup due to network change");
          this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
        }
        else
        {
          if (state != OneDriveBackupState.Idle || this.stopReason != OneDriveBkupRestStopReason.NetworkChange || !OneDriveBackupManager.IsBackupIncomplete || !this.CheckCloudBackupNetwork())
            return;
          if (AppState.IsBackgroundAgent)
          {
            Log.l("odm", "restarting in-progress cloud backup due to network change");
            this.Start(OneDriveBkupRestTrigger.BackgroundResume);
          }
          else
          {
            Log.l("odm", "restarting in-progress cloud backup due to network change");
            this.Start(OneDriveBkupRestTrigger.ForegroundResume);
          }
        }
      }
    }

    public void ClearIncompleteBackup()
    {
      Log.l("odm", "clear incomplete backup");
      lock (this.threadLock)
      {
        if (this.State != OneDriveBackupState.Idle)
        {
          Log.l("odm", "cannot clear, backup in progress");
          throw new InvalidOperationException("backup in progress");
        }
        if (this.workerTask != null)
        {
          Log.l("odm", "cannot clear, worker task is running");
          throw new InvalidOperationException("worker task is running");
        }
        using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
        {
          Log.l("odm", "clearing backup session");
          oneDriveManifest.ClearCurrentOneDriveBackup();
        }
        Settings.OneDriveLargeUploadUrls = (Dictionary<string, string>) null;
      }
      this.RefreshBackupProperties();
    }

    public void DeleteBackupManifest()
    {
      Log.l("odm", "delete backup manifest");
      lock (this.threadLock)
      {
        if (this.State != OneDriveBackupState.Idle)
        {
          Log.l("odm", "cannot delete, backup in progress");
          throw new InvalidOperationException("backup in progress");
        }
        if (this.workerTask != null)
        {
          Log.l("odm", "cannot delete, worker task is running");
          throw new InvalidOperationException("worker task is running");
        }
        using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
          oneDriveManifest.Delete();
      }
      this.RefreshBackupProperties();
    }

    public void SetTrigger(OneDriveBkupRestTrigger backupTrigger)
    {
      lock (OneDriveBackupManager.initLock)
        this.startUpTrigger = new OneDriveBkupRestTrigger?(backupTrigger);
      Log.l("odm", "backup manager set trigger={0}", (object) backupTrigger.ToString());
    }

    public void MaybeStart()
    {
      OneDriveBkupRestTrigger? nullable = new OneDriveBkupRestTrigger?();
      lock (OneDriveBackupManager.initLock)
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
        Log.l("odm", "No need to start backup");
    }

    public void Stop(OneDriveBackupStopError error)
    {
      this.Terminate(OneDriveBkupRestStopReason.Stop, error);
    }

    public void Abort(OneDriveBackupStopError error)
    {
      this.Terminate(OneDriveBkupRestStopReason.Abort, error);
    }

    private void Terminate(OneDriveBkupRestStopReason reason, OneDriveBackupStopError error)
    {
      bool flag = false;
      lock (OneDriveBackupManager.initLock)
      {
        OneDriveBkupRestTrigger? startUpTrigger = this.startUpTrigger;
        this.startUpTrigger = new OneDriveBkupRestTrigger?();
        if (error == OneDriveBackupStopError.StoppedByOS)
        {
          if (OneDriveBackupManager.instance != null)
          {
            if (OneDriveBackupManager.instance.State == OneDriveBackupState.Idle)
              flag = true;
          }
        }
      }
      if (flag)
        Log.l("odm", "backup manager idle on app shutdown, ignoring shutdown");
      else
        this.StopImpl(reason, error);
    }

    public void Start(OneDriveBkupRestTrigger backupTrigger)
    {
      Log.l("odm", "backup manager start (trigger={0})", (object) backupTrigger.ToString());
      PhoneNumberVerificationState? nullable = new PhoneNumberVerificationState?();
      try
      {
        nullable = new PhoneNumberVerificationState?(Settings.PhoneNumberVerificationState);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "odm exception checking verification state");
      }
      if (!nullable.HasValue || nullable.Value != PhoneNumberVerificationState.Verified)
      {
        Log.l("odm", "cannot start due to verification state: {0}", nullable.HasValue ? (object) nullable.Value.ToString() : (object) "null");
        OneDriveBackupStatusReporting.SetLastBackupState("", 11);
      }
      else
      {
        lock (this.threadLock)
        {
          if (Settings.LoginFailed)
          {
            Log.l("odm", "cannot start due to login failure");
            OneDriveBackupStatusReporting.SetLastBackupState("", 5);
          }
          else
          {
            if (this.workerTask != null && this.activeBackupTrigger.HasValue)
            {
              Log.l("odm", "backup manager restarted while already running ({0} -> {1})", (object) this.activeBackupTrigger.Value.ToString(), (object) backupTrigger.ToString());
              if (this.activeBackupTrigger.Value == OneDriveBkupRestTrigger.BackgroundAgentShort && backupTrigger == OneDriveBkupRestTrigger.BackgroundAgent)
              {
                Log.l("odm", "short to long, changing active trigger mode");
                this.activeBackupTrigger = new OneDriveBkupRestTrigger?(backupTrigger);
                return;
              }
              if (this.activeBackupTrigger.Value == OneDriveBkupRestTrigger.BackgroundAgent && backupTrigger == OneDriveBkupRestTrigger.BackgroundAgentShort)
              {
                Log.l("odm", "long to short, ignoring start request");
                return;
              }
              if (this.activeBackupTrigger.Value == backupTrigger && (backupTrigger == OneDriveBkupRestTrigger.BackgroundAgent || backupTrigger == OneDriveBkupRestTrigger.BackgroundAgentShort))
              {
                Log.l("odm", "no change, ignoring start request");
                return;
              }
            }
            if (backupTrigger == OneDriveBkupRestTrigger.ForegroundResume && this.State != OneDriveBackupState.Idle && this.workerTask == null)
            {
              Log.l("odm", "foreground resume during suspended cancellation");
              this.foregroundResumeAfterStop = true;
            }
            else
            {
              if (this.State != OneDriveBackupState.Idle)
              {
                Log.l("odm", "cannot start, backup in progress");
                throw new InvalidOperationException("backup in progress");
              }
              if (this.workerTask != null && !this.workerTask.IsCanceled && !this.workerTask.IsCompleted)
              {
                Log.l("odm", "cannot start, worker task is running");
                throw new InvalidOperationException("worker task is running");
              }
              if (OneDriveBackupStatusReporting.IsLastBackupTimedOut() && backupTrigger == OneDriveBkupRestTrigger.BackgroundAgentShort)
              {
                Log.l("odm", "start in background agent short ignored as previous attempt did not complete");
              }
              else
              {
                OneDriveBackupStatusReporting.SetLastBackupState("", backupTrigger == OneDriveBkupRestTrigger.UserInteraction ? 13 : 12, OneDriveBackupStatusReporting.BackupStateDisplayOptions.Normal);
                this.cancellationSource = new CancellationTokenSource();
                CancellationToken cancellationToken = this.cancellationSource.Token;
                this.stopReason = OneDriveBkupRestStopReason.Completed;
                this.stopError = OneDriveBackupStopError.None;
                this.activeBackupTrigger = new OneDriveBkupRestTrigger?(backupTrigger);
                this.foregroundResumeAfterStop = false;
                this.workerTask = Task.Run((Func<Task>) (async () =>
                {
                  WhatsApp.Events.Backup fsEvent = (WhatsApp.Events.Backup) null;
                  Log.l("odm", "worker task starting");
                  this.OnBackupStarted(backupTrigger);
                  this.SetNewBackupState(OneDriveBackupState.Starting);
                  try
                  {
                    this.CheckCloudBackupEnabled();
                    bool flag = this.InitializeCloudBackupFromSavedSummary(cancellationToken);
                    bool localBackup1 = this.ShouldCreateLocalBackup(backupTrigger);
                    if (localBackup1)
                    {
                      this.SetLocalBackupInProgress(true);
                      BackupSummary localBackup2 = this.CreateLocalBackup(backupTrigger, cancellationToken);
                      cancellationToken.ThrowIfCancellationRequested();
                      if (localBackup2 != null)
                      {
                        try
                        {
                          this.InitializeCloudBackup(localBackup2, cancellationToken);
                          Log.l("odm", "removing saved summary after in-memory prepare");
                          Backup.RemoveSavedSummary();
                        }
                        catch (Exception ex)
                        {
                          Log.l("odm", "unable to prepare cloud backup");
                          this.StopImpl(OneDriveBkupRestStopReason.AbortError);
                          throw;
                        }
                      }
                    }
                    fsEvent = this.CreateBackupEvent();
                    this.SetLocalBackupInProgress(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    this.RefreshBackupProperties();
                    this.LogBackupProperties();
                    lock (this.threadLock)
                    {
                      if (this.activeBackupTrigger.HasValue)
                      {
                        if (this.activeBackupTrigger.Value != backupTrigger)
                        {
                          Log.l("odm", "updating active backup trigger ({0} -> {1})", (object) backupTrigger.ToString(), (object) this.activeBackupTrigger.Value.ToString());
                          backupTrigger = this.activeBackupTrigger.Value;
                        }
                      }
                    }
                    if (AppState.BatterySaverEnabled)
                    {
                      Log.l("odm", "backup worker task cancelled because of battery state");
                      throw new OneDriveBatterySaverStatexception("Battery state too low to start OneDrive backup");
                    }
                    if (!this.ShouldProcessCloudBackup(backupTrigger, flag | localBackup1))
                      return;
                    this.ProgressValue = 0L;
                    if (this.CheckCloudBackupNetwork())
                    {
                      this.processor.ResetCounters();
                      await this.ProcessCloudBackup(backupTrigger, cancellationToken, fsEvent);
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
                    if (ex.IsMatch("quotaLimitReached"))
                    {
                      fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.REMOTE_STORAGE_IS_FULL);
                      this.StopImpl(OneDriveBkupRestStopReason.AbortError, OneDriveBackupStopError.QuotaLimitReached);
                    }
                    else if (ex.IsMatch("unauthenticated"))
                    {
                      fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
                      this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveBackupStopError.Unauthenticated);
                    }
                    else if (ex.IsMatch("serviceNotAvailable"))
                    {
                      fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.BACKUP_SERVER_NOT_WORKING);
                      this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveBackupStopError.ServiceNotAvailable);
                    }
                    else if (ex.IsMatch("generalException") && !this.CheckCloudBackupNetwork())
                      this.StopImpl(OneDriveBkupRestStopReason.NetworkChange);
                    else if (((IEnumerable<string>) OneDriveBackupManager.AuthenticationErrorCodes).Contains<string>(ex.Error?.Code ?? ""))
                    {
                      fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
                      this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveBackupStopError.Unauthenticated);
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
                    if (this.State == OneDriveBackupState.LocalBackup)
                    {
                      this.StopImpl(OneDriveBkupRestStopReason.StopError);
                    }
                    else
                    {
                      this.StopImpl(OneDriveBkupRestStopReason.AbortError);
                      uint hresult = ex.GetHResult();
                      if ((int) hresult != (int) Sqlite.HRForError(11U) && (int) hresult != (int) Sqlite.HRForError(19U))
                        return;
                      Log.l("odm", "deleting manifest due to error");
                      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
                        oneDriveManifest.Delete();
                    }
                  }
                  catch (OneDriveBatterySaverStatexception ex)
                  {
                    Log.l("odm", "worker battery state exception");
                    Log.LogException((Exception) ex, "odm");
                    if (this.cancellationSource.IsCancellationRequested)
                      return;
                    fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.UNKNOWN_ERROR);
                    this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveBackupStopError.StoppedByBattery);
                  }
                  catch (Exception ex)
                  {
                    Log.l("odm", "backup worker task exception: {0}", (object) ex.GetFriendlyMessage());
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
                    if (ex is OutOfMemoryException && !this.cancellationSource.IsCancellationRequested)
                    {
                      Stats.LogMemoryUsage();
                      Stats.LogMemoryUsage(gc: true);
                    }
                    if (this.cancellationSource.IsCancellationRequested)
                      return;
                    this.StopImpl(OneDriveBkupRestStopReason.StopError);
                  }
                  finally
                  {
                    bool cancellationRequested = this.cancellationSource.IsCancellationRequested;
                    Log.l("odm", "backup worker task complete: cancelled={0}, stopReason={1}, stopError={2}", (object) cancellationRequested, (object) this.stopReason, (object) this.stopError);
                    if (this.stopReason == OneDriveBkupRestStopReason.Completed)
                      OneDriveBackupStatusReporting.SetLastBackupState(this.BackupProperties.BackupId, 1, OneDriveBackupStatusReporting.BackupStateDisplayOptions.Normal);
                    OneDriveBkupRestTrigger? changedTrigger = new OneDriveBkupRestTrigger?();
                    lock (this.threadLock)
                    {
                      if (this.activeBackupTrigger.HasValue && this.activeBackupTrigger.Value != backupTrigger && !cancellationRequested)
                        changedTrigger = new OneDriveBkupRestTrigger?(this.activeBackupTrigger.Value);
                      this.workerTask = (Task) null;
                      this.cancellationSource.Dispose();
                      this.cancellationSource = (CancellationTokenSource) null;
                      this.activeBackupTrigger = new OneDriveBkupRestTrigger?();
                      Settings.OneDriveBackupRequestCount += this.processor.RequestCount;
                      if (!cancellationRequested && this.stopReason != OneDriveBkupRestStopReason.Completed)
                        ++Settings.OneDriveBackupFailedCount;
                      if (fsEvent != null)
                        this.SubmitBackupEvent(fsEvent);
                      if (this.stopReason != OneDriveBkupRestStopReason.Abort)
                      {
                        if (this.stopReason != OneDriveBkupRestStopReason.AbortError)
                          goto label_81;
                      }
                      try
                      {
                        using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
                        {
                          if (oneDriveManifest.IsOneDriveBackupInProgress())
                          {
                            Log.l("odm", "clearing incomplete backup session");
                            oneDriveManifest.ClearCurrentOneDriveBackup();
                          }
                        }
                        Settings.OneDriveLargeUploadUrls = (Dictionary<string, string>) null;
                      }
                      catch (Exception ex)
                      {
                        Log.l("odm", "worker task cleanup exception");
                        Log.LogException(ex, "odm");
                      }
                    }
label_81:
                    this.RefreshBackupProperties();
                    this.SetNewBackupState(OneDriveBackupState.Idle);
                    this.OnBackupStopped(this.stopReason, new OneDriveBkupRestTrigger?(backupTrigger), changedTrigger);
                    if (this.foregroundResumeAfterStop)
                    {
                      this.foregroundResumeAfterStop = false;
                      this.Start(OneDriveBkupRestTrigger.ForegroundResume);
                    }
                  }
                }), cancellationToken);
              }
            }
          }
        }
      }
    }

    private WhatsApp.Events.Backup CreateBackupEvent()
    {
      ConnectionType connectionType;
      lock (this.threadLock)
        connectionType = !this.lastConnectionType.HasValue ? AppState.GetUserConnectionType() : this.lastConnectionType.Value;
      WhatsApp.Events.Backup backupEvent = new WhatsApp.Events.Backup();
      backupEvent.backupRestoreStage = new wam_enum_backup_restore_stage?(wam_enum_backup_restore_stage.UNKNOWN);
      switch (connectionType)
      {
        case ConnectionType.Wifi:
          backupEvent.backupRestoreIsWifi = new long?(1L);
          break;
        case ConnectionType.Cellular_2G:
        case ConnectionType.Cellular_3G:
          backupEvent.backupRestoreIsWifi = new long?(0L);
          break;
      }
      backupEvent.backupRestoreIncludeVideos = new bool?(this.backupProperties.Settings.IncludeVideos);
      return backupEvent;
    }

    private void SubmitBackupEvent(WhatsApp.Events.Backup fsEvent)
    {
      long ticks1 = DateTime.UtcNow.Ticks;
      ConnectionType connectionType;
      OneDriveBkupRestStopReason stopReason;
      lock (this.threadLock)
      {
        connectionType = !this.lastConnectionType.HasValue ? AppState.GetUserConnectionType() : this.lastConnectionType.Value;
        stopReason = this.stopReason;
      }
      if (stopReason == OneDriveBkupRestStopReason.Abort || stopReason == OneDriveBkupRestStopReason.Stop)
        return;
      switch (connectionType)
      {
        case ConnectionType.Wifi:
          fsEvent.backupRestoreFinishedOverWifi = new long?(1L);
          break;
        case ConnectionType.Cellular_2G:
        case ConnectionType.Cellular_3G:
          fsEvent.backupRestoreFinishedOverWifi = new long?(0L);
          break;
      }
      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
      {
        if (oneDriveManifest.Exists)
        {
          BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
          if (backupProperties != null)
          {
            fsEvent.backupRestoreChatdbSize = new double?((double) oneDriveManifest.GetRemoteDatabaseSizeToRestore());
            fsEvent.backupRestoreMediaSize = new double?((double) oneDriveManifest.GetRemoteMediaSizeToRestore());
            fsEvent.backupRestoreMediaFileCount = new double?((double) oneDriveManifest.GetRemoteMediaFileCount());
            fsEvent.backupRestoreTotalSize = new double?((double) backupProperties.Size);
            fsEvent.backupRestoreIsFull = new bool?(backupProperties.Size == backupProperties.IncrementalSize);
            long ticks2 = backupProperties.StartTime.Ticks;
            if (ticks2 > 0L)
            {
              if (ticks1 > ticks2)
                fsEvent.backupRestoreT = new long?((ticks1 - ticks2) / 10000L);
            }
          }
        }
      }
      fsEvent.backupRestoreTransferSize = new double?((double) this.ProgressValue);
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
          int driveBackupNetwork = (int) Settings.OneDriveBackupNetwork;
          bool flag = (driveBackupNetwork & 2) != 0;
          fsEvent.backupRestoreResult = (driveBackupNetwork & 1) == 0 || flag ? new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.DATA_CONNECTION_REQUIRED_BUT_MISSING) : new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.WIFI_REQUIRED_BUT_MISSING);
          break;
      }
      fsEvent.backupRestoreRetryCount = new long?((long) Settings.OneDriveBackupFailedCount);
      fsEvent.backupRestoreNetworkRequestCount = new double?((double) Settings.OneDriveBackupRequestCount);
      fsEvent.SaveEvent();
      Settings.OneDriveBackupFailedCount = 0;
      Settings.OneDriveBackupRequestCount = 0;
    }

    private void StopImpl(OneDriveBkupRestStopReason reason, OneDriveBackupStopError error = OneDriveBackupStopError.Unspecified)
    {
      Log.l("odm", "backup manager stopping (reason={0}, error={1})", (object) reason.ToString(), (object) error.ToString());
      if (error == OneDriveBackupStopError.StoppedByTimeout)
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
      OneDriveBackupStatusReporting.SetLastBackupState(this.backupProperties?.BackupId, reason, error);
      if (reason != OneDriveBkupRestStopReason.Abort && reason != OneDriveBkupRestStopReason.AbortError && reason != OneDriveBkupRestStopReason.StopError)
        error = OneDriveBackupStopError.None;
      lock (this.threadLock)
      {
        if ((reason == OneDriveBkupRestStopReason.Abort || reason == OneDriveBkupRestStopReason.AbortError) && this.State == OneDriveBackupState.Idle && OneDriveBackupManager.IsBackupIncomplete)
        {
          this.ClearIncompleteBackup();
          this.stopReason = reason;
          this.stopError = error;
          this.OnBackupStopped(this.stopReason);
        }
        else if (this.workerTask == null || this.cancellationSource == null)
        {
          if (error != OneDriveBackupStopError.StoppedByTimeout)
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

    private void CheckCloudBackupEnabled()
    {
      if (Settings.OneDriveBackupFrequency != OneDriveBackupFrequency.Off && !OneDriveRestoreManager.IsRestoreIncomplete)
        return;
      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
      {
        if (oneDriveManifest.Exists)
        {
          oneDriveManifest.CurrentOneDriveBackupProperties();
          if (oneDriveManifest.IsOneDriveBackupInProgress())
            oneDriveManifest.ClearCurrentOneDriveBackup();
        }
      }
      Settings.OneDriveLargeUploadUrls = (Dictionary<string, string>) null;
      Backup.RemoveSavedSummary();
    }

    private bool ShouldCreateLocalBackup(OneDriveBkupRestTrigger backupTrigger)
    {
      if (backupTrigger == OneDriveBkupRestTrigger.UserInteraction)
        return true;
      DateTime? lastBackupTime = Backup.GetLastBackupTime();
      if (OneDriveBackupStatusReporting.IsBackUpUserInitiatedAndActive())
        return (!lastBackupTime.HasValue ? 0 : (OneDriveBackupStatusReporting.IsAfterStartTime(lastBackupTime.Value) ? 1 : 0)) == 0;
      DateTime now = DateTime.Now;
      ulong tickCount = NativeInterfaces.Misc.GetTickCount();
      bool flag1 = now.Hour >= 3 && now.Hour < 4;
      bool flag2 = (double) tickCount > TimeSpan.FromHours(1.0).TotalMilliseconds && (double) tickCount < TimeSpan.FromHours(3.0).TotalMilliseconds;
      if (!(flag1 | flag2))
        return false;
      if (Settings.CorruptDb)
      {
        Log.l("odm", "DB is marked as corrupt.  Bailing");
        return false;
      }
      if (!Backup.CanBackup())
      {
        Log.l("odm", "This device looks ineligible for backup.");
        return false;
      }
      if (lastBackupTime.HasValue && lastBackupTime.Value > DateTime.UtcNow.AddHours(flag1 ? -4.0 : -24.0))
      {
        Log.l("odm", "Last backup at {0} was too recent", (object) lastBackupTime.Value.ToLocalTime());
        return false;
      }
      bool doBackup = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        long messagesCount = db.GetMessagesCount();
        int conversationsCount = db.GetConversationsCount(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, true);
        if (messagesCount <= 0L && conversationsCount <= 0)
          return;
        Log.l("odm", "Backing up {0} messages, {1} chats...", (object) messagesCount, (object) conversationsCount);
        doBackup = true;
      }));
      return doBackup;
    }

    private bool ShouldInitNewCloudBackup(OneDriveBkupRestTrigger backupTrigger)
    {
      DateTime now = DateTime.Now;
      OneDriveBackupFrequency driveBackupFrequency = Settings.OneDriveBackupFrequency;
      bool flag1 = OneDriveBackupStatusReporting.IsBackUpUserInitiatedAndActive();
      bool flag2;
      if (OneDriveRestoreManager.IsRestoreIncomplete)
      {
        Log.l("odm", "restore in-progress, not preparing a cloud backup");
        flag2 = false;
      }
      else if (backupTrigger == OneDriveBkupRestTrigger.UserInteraction | flag1)
      {
        flag2 = driveBackupFrequency != 0;
        if (flag2)
        {
          using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
          {
            if (oneDriveManifest.Exists)
            {
              if (oneDriveManifest.IsOneDriveBackupInProgress())
              {
                Log.l("odm", "clearing in-progress cloud backup due to user request");
                oneDriveManifest.ClearCurrentOneDriveBackup();
              }
            }
          }
          Settings.OneDriveLargeUploadUrls = (Dictionary<string, string>) null;
        }
      }
      else
      {
        bool flag3 = false;
        DateTime dateTime1 = DateTime.MinValue;
        using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
        {
          if (oneDriveManifest.Exists)
          {
            BackupProperties backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
            if (backupProperties != null)
            {
              if (oneDriveManifest.IsOneDriveBackupInProgress())
              {
                DateTime dateTime2 = backupProperties.StartTime;
                dateTime2 = dateTime2.AddDays(7.0);
                if (dateTime2.CompareTo(now.ToUniversalTime()) < 0)
                {
                  Log.l("odm", "in-progress cloud backup is more than 7 days old, clearing");
                  oneDriveManifest.ClearCurrentOneDriveBackup();
                  Settings.OneDriveLargeUploadUrls = (Dictionary<string, string>) null;
                }
                else
                  flag3 = true;
              }
              else
                dateTime1 = string.IsNullOrEmpty(backupProperties.BackupId) ? backupProperties.LastStartTime : backupProperties.StartTime;
            }
            else
              Log.l("odm", "manifest does not contain props");
          }
        }
        if (flag3)
        {
          Log.l("odm", "in-progress cloud backup, not preparing a new one");
          flag2 = false;
        }
        else
        {
          DateTime date1 = dateTime1.Date;
          DateTime date2 = now.ToUniversalTime().Date;
          Log.l("odm", "last cloud backup was on {0}", (object) dateTime1.ToLocalTime().ToString());
          switch (driveBackupFrequency)
          {
            case OneDriveBackupFrequency.Daily:
              flag2 = date1.AddDays(1.0).CompareTo(date2) <= 0;
              break;
            case OneDriveBackupFrequency.Weekly:
              flag2 = date1.AddDays(7.0).CompareTo(date2) <= 0;
              break;
            case OneDriveBackupFrequency.Monthly:
              flag2 = date1.AddMonths(1).CompareTo(date2) <= 0;
              break;
            default:
              flag2 = false;
              break;
          }
        }
      }
      if (flag2)
        Log.l("odm", "cloud backup will be initialized after local backup");
      return flag2;
    }

    private bool ShouldProcessCloudBackup(
      OneDriveBkupRestTrigger backupTrigger,
      bool localBackupProcessed)
    {
      if (!OneDriveBackupManager.IsBackupIncomplete)
        return false;
      if (backupTrigger != OneDriveBkupRestTrigger.BackgroundAgentShort)
        return true;
      return !localBackupProcessed && this.BackupProperties.InProgress && this.BackupProperties.IncompleteSize == 0L;
    }

    private BackupSummary CreateLocalBackup(
      OneDriveBkupRestTrigger backupTrigger,
      CancellationToken cancellationToken)
    {
      Log.l("odm", "create local backup");
      this.SetNewBackupState(OneDriveBackupState.LocalBackup, new long?(0L), new long?(100L));
      DateTime now = DateTime.Now;
      BackupSummary backupSummary = this.ShouldInitNewCloudBackup(backupTrigger) ? new BackupSummary() : (BackupSummary) null;
      Log.l("odm", "starting local backup process");
      using (ManualResetEvent ev = new ManualResetEvent(false))
      {
        WAThreadPool.QueueUserWorkItem((Action) (() =>
        {
          Action onComplete = Utils.IgnoreMultipleInvokes((Action) (() => ev.Set()));
          try
          {
            Backup.Save(onComplete, (Action<int>) (v => this.ProgressValue = (long) v), backupSummary, new CancellationToken?(cancellationToken));
            Log.l("odm", "finished local backup");
          }
          catch (Exception ex)
          {
            backupSummary = (BackupSummary) null;
            if (!(ex is TaskCanceledException))
              Log.SendCrashLog(ex, "create backup");
            onComplete();
          }
        }));
        if (backupTrigger == OneDriveBkupRestTrigger.UserInteraction)
        {
          TimeSpan timeSpan1 = DateTime.Now - now;
          TimeSpan timeSpan2 = TimeSpan.FromSeconds(3.0);
          if (timeSpan1 < timeSpan2)
            Thread.Sleep(timeSpan2 - timeSpan1);
        }
        ev.WaitOne();
      }
      FieldStats.ReportBackupConvo();
      return !cancellationToken.IsCancellationRequested && backupSummary != null && backupSummary.Databases.Count > 0 ? backupSummary : (BackupSummary) null;
    }

    private static bool PrepareLocalBackupFile(
      ref LocalBackupFile backupFile,
      bool updateLocalFiles = false)
    {
      bool flag = false;
      if (backupFile.Sha1Hash != null && backupFile.Sha1Hash.Length != 0)
      {
        if (backupFile.Size >= 0L)
        {
          try
          {
            string absolutePath = backupFile.FileReference.ToAbsolutePath();
            using (IMediaStorage mediaStorage = MediaStorage.Create(absolutePath))
            {
              if (mediaStorage.FileExists(absolutePath))
              {
                flag = true;
                goto label_49;
              }
              else
              {
                Log.p("odm", "local file exist error: file does not exist {0}", (object) backupFile.FileReference.ToString());
                goto label_49;
              }
            }
          }
          catch (Exception ex)
          {
            Log.WriteLineDebug("local file exist check error: " + ex.ToString());
            Log.LogException(ex, "local file exist check");
            goto label_49;
          }
        }
      }
      if (backupFile.Sha1Hash == null)
      {
        string localFileUri = backupFile.FileReference.ToAbsolutePath();
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(localFileUri))
          {
            if (mediaStorage.FileExists(localFileUri))
            {
              using (Stream inputStream = mediaStorage.OpenFile(localFileUri))
              {
                using (SHA1Managed shA1Managed = new SHA1Managed())
                {
                  backupFile.Sha1Hash = shA1Managed.ComputeHash(inputStream);
                  backupFile.Size = inputStream.Length;
                  flag = true;
                }
              }
            }
            else
              Log.p("odm", "local file hash error: file does not exist {0}", (object) backupFile.FileReference.ToString());
          }
        }
        catch (Exception ex)
        {
          Log.WriteLineDebug("local file hash error: " + ex.ToString());
          Log.LogException(ex, "local file hash");
        }
        if (updateLocalFiles & flag)
        {
          try
          {
            byte[] sha1Hash = backupFile.Sha1Hash;
            long fileSize = backupFile.Size;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.UpdateLocalFileHash(localFileUri, sha1Hash, new long?(fileSize))));
          }
          catch (Exception ex)
          {
            Log.WriteLineDebug("unable to update LocalFiles with computed hash: " + ex.ToString());
          }
        }
      }
      else if (backupFile.Sha1Hash != null && backupFile.Sha1Hash.Length != 0 && backupFile.Size < 0L)
      {
        string localFileUri = backupFile.FileReference.ToAbsolutePath();
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(localFileUri))
          {
            if (mediaStorage.FileExists(localFileUri))
            {
              using (Stream stream = mediaStorage.OpenFile(localFileUri))
              {
                backupFile.Size = stream.Length;
                flag = true;
              }
            }
            else
              Log.p("odm", "local file size error: file does not exist");
          }
        }
        catch (Exception ex)
        {
          Log.WriteLineDebug("local file size error: " + ex.ToString());
          Log.LogException(ex, "local file size");
        }
        if (updateLocalFiles & flag)
        {
          try
          {
            long fileSize = backupFile.Size;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.UpdateLocalFileSize(localFileUri, fileSize)));
          }
          catch (Exception ex)
          {
            Log.WriteLineDebug("unable to update LocalFiles with file size: " + ex.ToString());
          }
        }
      }
      else
        Log.l("odm", "skipping file due to incomplete data: {0}", (object) backupFile.FileReference.ToString());
label_49:
      return flag;
    }

    private void InitializeCloudBackup(
      BackupSummary backupSummary,
      CancellationToken cancellationToken)
    {
      Log.l("odm", "starting backup session initialize");
      this.SetNewBackupState(OneDriveBackupState.InitializingBackup);
      bool driveIncludeVideos = Settings.OneDriveIncludeVideos;
      OneDriveBackupFrequency driveBackupFrequency = Settings.OneDriveBackupFrequency;
      AutoDownloadSetting driveBackupNetwork = Settings.OneDriveBackupNetwork;
      Settings.OneDriveBackupFailedCount = 0;
      Settings.OneDriveBackupRequestCount = 0;
      List<LocalBackupFile> localFiles1 = new List<LocalBackupFile>(backupSummary.Databases.Count);
      foreach (BackupMediaFile database in backupSummary.Databases)
      {
        LocalBackupFile backupFile = new LocalBackupFile()
        {
          FileReference = database.FileRef,
          Sha1Hash = database.Sha1Hash,
          Size = database.Size ?? -1L
        };
        if (!OneDriveBackupManager.PrepareLocalBackupFile(ref backupFile))
          throw new Exception("Could not prepare DB file for backup, cannot proceed");
        localFiles1.Add(backupFile);
        cancellationToken.ThrowIfCancellationRequested();
      }
      List<LocalBackupFile> localFiles2 = new List<LocalBackupFile>(backupSummary.MediaFiles.Count);
      int num1 = 0;
      int num2 = 0;
      foreach (BackupMediaFile mediaFile in backupSummary.MediaFiles)
      {
        if (!driveIncludeVideos)
        {
          string filePart = mediaFile.FileRef.FilePart;
          if (filePart.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase) || filePart.EndsWith(".3gp", StringComparison.InvariantCultureIgnoreCase) || filePart.EndsWith(".mov", StringComparison.InvariantCultureIgnoreCase))
          {
            ++num1;
            continue;
          }
        }
        LocalBackupFile backupFile = new LocalBackupFile()
        {
          FileReference = mediaFile.FileRef,
          Sha1Hash = mediaFile.Sha1Hash,
          Size = mediaFile.Size ?? -1L
        };
        if (OneDriveBackupManager.PrepareLocalBackupFile(ref backupFile, true))
          localFiles2.Add(backupFile);
        else
          ++num2;
        cancellationToken.ThrowIfCancellationRequested();
      }
      Log.l("odm", "{0} of {1} media files will be included in backup, skip counts {2},{3}", (object) localFiles2.Count, (object) backupSummary.MediaFiles.Count, (object) num1, (object) num2);
      using (OneDriveManifest oneDriveManifest1 = new OneDriveManifest())
      {
        Log.l("odm", "updating local manifest with databases");
        Log.l("odm", "local manifest databases: {0}", oneDriveManifest1.UpdateLocalDatabaseFiles((IEnumerable<LocalBackupFile>) localFiles1) ? (object) "updated" : (object) "unchanged");
        cancellationToken.ThrowIfCancellationRequested();
        Log.l("odm", "updating local manifest with media");
        Log.l("odm", "local manifest media: {0}", oneDriveManifest1.UpdateLocalMediaFiles((IEnumerable<LocalBackupFile>) localFiles2) ? (object) "updated" : (object) "unchanged");
        cancellationToken.ThrowIfCancellationRequested();
        Log.l("odm", "initializing backup session");
        OneDriveManifest oneDriveManifest2 = oneDriveManifest1;
        long ticks;
        if (!backupSummary.Timestamp.HasValue)
        {
          ticks = DateTime.UtcNow.Ticks;
        }
        else
        {
          DateTime universalTime = backupSummary.Timestamp.Value;
          universalTime = universalTime.ToUniversalTime();
          ticks = universalTime.Ticks;
        }
        BackupSettings backupSettings = new BackupSettings()
        {
          BackupFrequency = driveBackupFrequency,
          BackupNetwork = driveBackupNetwork,
          IncludeVideos = driveIncludeVideos
        };
        oneDriveManifest2.StartNewBackup(ticks, backupSettings);
        Log.l("odm", "backup session initialized");
      }
      Log.l("odm", "finished backup session initialize");
    }

    private bool InitializeCloudBackupFromSavedSummary(CancellationToken cancellationToken)
    {
      BackupSummary savedSummary = Backup.GetSavedSummary();
      if (savedSummary == null)
        return false;
      if (savedSummary.Timestamp.HasValue && savedSummary.Timestamp.Value.AddDays(3.0).CompareTo(DateTime.UtcNow) < 0)
      {
        Log.l("odm", "removing old backup summary from: {0}", (object) savedSummary.Timestamp.Value.ToString());
        Backup.RemoveSavedSummary();
        return false;
      }
      try
      {
        Log.l("odm", "preparing cloud backup from saved summary: {0}", (object) savedSummary.Timestamp.Value.ToString());
        this.InitializeCloudBackup(savedSummary, cancellationToken);
        Log.l("odm", "removing saved summary after prepare");
        Backup.RemoveSavedSummary();
      }
      catch (SqliteException ex)
      {
        Log.l("odm", "manifest error preparing cloud backup from saved summary: " + ex.ToString());
        if ((int) ex.GetHResult() == (int) Sqlite.HRForError(11U))
        {
          Log.l("odm", "deleting manifest due to error");
          using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
            oneDriveManifest.Delete();
        }
        else
          Log.LogException((Exception) ex, "summary init DB");
      }
      catch (Exception ex)
      {
        Log.l("odm", "unable to prepare cloud backup from saved summary: " + ex.ToString());
        Log.LogException(ex, "summary init");
      }
      return true;
    }

    private bool CheckCloudBackupNetwork()
    {
      AutoDownloadSetting driveBackupNetwork = Settings.OneDriveBackupNetwork;
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
            flag2 = (driveBackupNetwork & AutoDownloadSetting.EnabledOnWifi) != 0;
            break;
          case ConnectionType.Cellular_2G:
          case ConnectionType.Cellular_3G:
            bool flag3;
            try
            {
              flag3 = NativeInterfaces.Misc.GetCellInfo(CellInfoFlags.NetworkInfo).Roaming;
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "roaming check");
              flag3 = true;
            }
            flag2 = !flag3 ? (driveBackupNetwork & AutoDownloadSetting.EnabledOnData) != 0 : (driveBackupNetwork & AutoDownloadSetting.EnabledWhileRoaming) != 0;
            break;
          default:
            flag2 = false;
            break;
        }
      }
      if (!flag2)
      {
        if (flag1)
          Log.l("odm", "cloud backup not allowed due to VoIP call");
        else
          Log.l("odm", "cloud backup not allowed due to network settings");
      }
      return flag2;
    }

    private async Task ProcessCloudBackup(
      OneDriveBkupRestTrigger backupTrigger,
      CancellationToken cancellationToken,
      WhatsApp.Events.Backup fsEvent)
    {
      Log.l("odm", "starting cloud backup process");
      if (backupTrigger == OneDriveBkupRestTrigger.UserInteraction || backupTrigger == OneDriveBkupRestTrigger.ForegroundResume)
        this.processor.CredentialPrompt = (CredentialPromptType) 0;
      else
        this.processor.CredentialPrompt = (CredentialPromptType) 2;
      if (!this.processor.IsAuthenticated)
      {
        this.SetNewBackupState(OneDriveBackupState.Authenticating);
        if (!await this.processor.Authenticate())
        {
          fsEvent.backupRestoreStage = new wam_enum_backup_restore_stage?(wam_enum_backup_restore_stage.AUTH_REQUEST);
          fsEvent.backupRestoreResult = new wam_enum_backup_restore_result?(wam_enum_backup_restore_result.AUTH_FAILED);
          this.StopImpl(OneDriveBkupRestStopReason.StopError, OneDriveBackupStopError.Unauthenticated);
        }
        if (string.IsNullOrEmpty(Settings.OneDriveUserId) || string.IsNullOrEmpty(Settings.OneDriveUserDisplayName) || string.IsNullOrEmpty(Settings.OneDriveUserAccountEmail))
        {
          int num = await this.processor.QueryUserMetadata(cancellationToken) ? 1 : 0;
        }
      }
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewBackupState(OneDriveBackupState.PreparingBackupPath);
      cancellationToken.ThrowIfCancellationRequested();
      this.RefreshBackupProperties();
      cancellationToken.ThrowIfCancellationRequested();
      await this.PrepareBackupPath(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewBackupState(OneDriveBackupState.SynchronizingManifest);
      int num1 = await this.SynchronizeManifest(cancellationToken) ? 1 : 0;
      cancellationToken.ThrowIfCancellationRequested();
      int num2 = await this.UploadManifest(cancellationToken) ? 1 : 0;
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewBackupState(OneDriveBackupState.UploadingDatabases);
      await this.BackupDatabases(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewBackupState(OneDriveBackupState.UploadingMedia);
      await this.BackupMedia(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
      this.SetNewBackupState(OneDriveBackupState.SynchronizingManifestFinal);
      await this.PurgeDeletedMedia(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();
      int num3 = await this.SynchronizeManifest(cancellationToken) ? 1 : 0;
      cancellationToken.ThrowIfCancellationRequested();
      int num4 = await this.UploadManifest(cancellationToken) ? 1 : 0;
      cancellationToken.ThrowIfCancellationRequested();
      await this.FinalizeBackup(cancellationToken);
      Log.l("odm", "backup process complete");
      this.SetNewBackupState(OneDriveBackupState.Complete);
      this.RefreshBackupProperties();
    }

    private async Task PrepareBackupPath(CancellationToken cancellationToken)
    {
      Log.l("odm", "preparing backup path");
      using (OneDriveManifest manifest = new OneDriveManifest())
        await this.processor.PrepareBackupPath(manifest, cancellationToken);
      Log.l("odm", "backup path prepared");
    }

    private async Task<bool> SynchronizeManifest(CancellationToken cancellationToken)
    {
      bool flag = false;
      Log.l("odm", "updating remote manifest");
      using (OneDriveManifest manifest = new OneDriveManifest())
        flag = await this.processor.SynchronizeManifest(manifest, cancellationToken);
      Log.l("odm", "remote manifest: {0}", flag ? (object) "updated" : (object) "unchanged");
      return flag;
    }

    private async Task<bool> UploadManifest(CancellationToken cancellationToken)
    {
      bool flag = false;
      Log.l("odm", "uploading manifest");
      using (OneDriveManifest manifest = new OneDriveManifest())
        flag = await this.processor.UploadManifest(manifest, cancellationToken);
      Log.l("odm", "manifest on onedrive: {0}", flag ? (object) "updated" : (object) "unchanged");
      return flag;
    }

    private async Task BackupDatabases(CancellationToken cancellationToken)
    {
      Progress<long> progress = new Progress<long>((Action<long>) (p => this.ProgressValue += p));
      using (OneDriveManifest manifest = new OneDriveManifest())
      {
        manifest.Open();
        await this.processor.BackupDatabases(manifest, cancellationToken, (IProgress<long>) progress);
      }
    }

    private async Task BackupMedia(CancellationToken cancellationToken)
    {
      Progress<long> progress = new Progress<long>((Action<long>) (p => this.ProgressValue += p));
      using (OneDriveManifest manifest = new OneDriveManifest())
      {
        manifest.Open();
        await this.processor.BackupMedia(manifest, cancellationToken, (IProgress<long>) progress);
      }
    }

    private async Task PurgeDeletedMedia(CancellationToken cancellationToken)
    {
      using (OneDriveManifest manifest = new OneDriveManifest())
      {
        manifest.Open();
        await this.processor.PurgeDeletedMedia(manifest, cancellationToken);
      }
    }

    private async Task FinalizeBackup(CancellationToken cancellationToken)
    {
      using (OneDriveManifest manifest = new OneDriveManifest())
      {
        manifest.Open();
        await this.processor.FinalizeBackup(manifest, cancellationToken);
      }
    }

    private void RefreshBackupProperties()
    {
      BackupProperties backupProperties = (BackupProperties) null;
      using (OneDriveManifest oneDriveManifest = new OneDriveManifest())
      {
        if (oneDriveManifest.Exists)
        {
          oneDriveManifest.Open();
          backupProperties = oneDriveManifest.CurrentOneDriveBackupProperties();
        }
      }
      if (backupProperties == null)
        backupProperties = new BackupProperties();
      this.BackupProperties = backupProperties;
      Log.d("odm", "RefreshBackupProperties: Id={0}, IncrementalSize={1}, IncompleteSize={2}", (object) backupProperties.BackupId, (object) backupProperties.IncrementalSize, (object) backupProperties.IncompleteSize);
      this.ProgressMaximum = Math.Max(backupProperties.IncompleteSize, backupProperties.IncrementalSize);
      this.ProgressValue = this.ProgressMaximum - backupProperties.IncompleteSize;
    }

    private void LogBackupProperties()
    {
      BackupProperties backupProperties = this.BackupProperties;
      if (backupProperties == null || !backupProperties.InProgress)
        return;
      Log.l("odm", "BackupProperties: Id={0}, StartTime={1}, Size={2}, IncrementalSize={3}, IncompleteSize={4}", (object) backupProperties.BackupId, (object) backupProperties.StartTime.ToString(), (object) backupProperties.Size, (object) backupProperties.IncrementalSize, (object) backupProperties.IncompleteSize);
    }

    private void SetNewBackupState(
      OneDriveBackupState state,
      long? progressValue = null,
      long? progressMaximum = null)
    {
      lock (this.eventLock)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        int num1 = OneDriveBackupManager.IsBackupStateIndeterminate(this.backupState) ? 1 : (this.progressValue < 0L ? 1 : 0);
        if (this.backupState != state)
        {
          this.backupState = state;
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
        bool e = OneDriveBackupManager.IsBackupStateIndeterminate(this.backupState) || this.progressValue < 0L;
        if (flag1)
          this.OnStateChanged(this.backupState);
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

    private static bool IsBackupStateIndeterminate(OneDriveBackupState state)
    {
      return state != OneDriveBackupState.LocalBackup && state != OneDriveBackupState.UploadingDatabases && state != OneDriveBackupState.UploadingMedia;
    }

    private void OnIsAuthenticationInProgressChanged(bool e)
    {
      EventHandler<bool> inProgressChanged = this.IsAuthenticationInProgressChanged;
      if (inProgressChanged == null)
        return;
      inProgressChanged((object) this, e);
    }

    private void OnStateChanged(OneDriveBackupState e)
    {
      EventHandler<OneDriveBackupState> stateChanged = this.StateChanged;
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

    private void OnBackupStarted(OneDriveBkupRestTrigger e)
    {
      EventHandler<OneDriveBkupRestTrigger> backupStarted = this.BackupStarted;
      if (backupStarted == null)
        return;
      backupStarted((object) this, e);
    }

    private void OnBackupStopped(
      OneDriveBkupRestStopReason reason,
      OneDriveBkupRestTrigger? startTrigger = null,
      OneDriveBkupRestTrigger? changedTrigger = null)
    {
      EventHandler<BkupRestStoppedEventArgs> backupStopped = this.BackupStopped;
      if (backupStopped == null)
        return;
      BkupRestStoppedEventArgs e = new BkupRestStoppedEventArgs()
      {
        Reason = reason,
        StartTrigger = startTrigger,
        ChangedTrigger = changedTrigger
      };
      backupStopped((object) this, e);
    }
  }
}
