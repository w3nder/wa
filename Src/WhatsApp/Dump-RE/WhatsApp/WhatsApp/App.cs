// Decompiled with JetBrains decompiler
// Type: WhatsApp.App
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Devices;
using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using WhatsApp.CommonOps;
using WhatsAppNative;
using Windows.ApplicationModel;
using Windows.Phone.Management.Deployment;

#nullable disable
namespace WhatsApp
{
  public class App : Application
  {
    private const string LogHeader = "app";
    public static bool Active = true;
    public static bool SuppressSync = false;
    private bool shouldTryEnablePauseOnBack = true;
    public static ISubject<Unit> ApplicationActivatedSubject = (ISubject<Unit>) new Subject<Unit>();
    public static Subject<ApplicationUnhandledExceptionEventArgs> UnhandledExceptionSubject = new Subject<ApplicationUnhandledExceptionEventArgs>();
    private static bool ExceptionHandlerRegistered = false;
    public static DateTime? LastLoginTime = new DateTime?();
    public static bool AccountJustDeleted = false;
    private string[] tempFilesToDelete_;
    private Subject<Uri> PushUriSubject = new Subject<Uri>();
    private Uri PushUri;
    private IDisposable ChannelUriSubscription;
    private IDisposable MessagesTableOutgoingSubscription;
    private IDisposable MessagesTableIncomingSubscription;
    private IDisposable ConversationDeleteSubscription;
    private NavUtils.BackStackOpParams backStackOpParams;
    private Subject<NavigatingCancelEventArgs> navigatingSubject = new Subject<NavigatingCancelEventArgs>();
    private ManualResetEventSlim minimalStartup = new ManualResetEventSlim(false);
    private LinkedList<Action> scheduledNavigatedWork = new LinkedList<Action>();
    private bool toHandleFastResumeTargetNav;
    private string resetNavUri;
    private string[] preservedPageUris;
    private bool navFromExternal = true;
    private PageWrapper currPageWrapper;
    private bool resetConnOnStartup;
    private bool DatabaseConnected;
    public static List<Action> OnDeactivated = new List<Action>();
    private bool phoneApplicationInitialized;
    public const string VersionFileName = "version";
    private static Guid ProductionGuid = new Guid("218a0ebb-1585-4c7e-a9ec-054cf4569a79");
    private static Guid BetaGuid = new Guid("6b587088-a2bd-4597-8416-6c77f0a3ec6d");
    private static Guid AlphaGuid = new Guid("b70630b7-9716-4a7a-841a-6e9f936fea6c");
    private bool _contentLoaded;

    public static App CurrentApp => Application.Current as App;

    public TransitionFrame RootFrame { get; private set; }

    public PhoneApplicationPage CurrentPage => this.RootFrame.Content as PhoneApplicationPage;

    public Transform OriginalRootFrameTransform { get; private set; }

    public FunXMPP.Connection Connection { get; private set; }

    public IDisposable ConnectionSubscription { get; private set; }

    public TreeInForestSubject<Unit> ConnectionResetSubject => AppState.ConnectionResetSubject;

    public IObservable<Uri> PushUriObservable
    {
      get
      {
        return this.PushUriSubject.Merge<Uri>(Observable.Defer<Uri>((Func<IObservable<Uri>>) (() => Observable.Return<Uri>(this.PushUri).Where<Uri>((Func<Uri, bool>) (_ => _ != (Uri) null))))).DistinctUntilChanged<Uri>();
      }
    }

    public string RecentNavigatedFromUri { get; private set; }

    public App()
    {
      FieldStats.MaybeSetFgAppLaunchTime();
      if (!AppState.UseWindowsNotificationService)
        PushSystem.ForegroundInstance = (IPushSystemForeground) new MpnsForeground();
      PushSystem.PushBoundSubject.Subscribe<Unit>((Action<Unit>) (_ => BackgroundAgentHelper.ScheduleVoip()));
      AppState.Worker.Enqueue((Action) (() =>
      {
        this.PushUriSubject.Subscribe<Uri>((Action<Uri>) (uri => this.PushUri = uri));
        this.InitializePush();
      }), WorkQueue.Priority.Interrupt);
      WAThreadPool.QueueUserWorkItem((Action) (() =>
      {
        NativeInterfaces.Misc.SetCancelEvent(Constants.KillEventName);
        Log.Initialize();
        Log.l("App constructed: {0)", AppState.GetAppVersion());
        IcuDataManager.InitializeIcu();
        AppState.UpdateLogConnectionInfo();
        App.ApplicationActivatedSubject.Subscribe<Unit>((Action<Unit>) (_ =>
        {
          NativeInterfaces.Misc.SetLogUserConnection(AppState.GetUserConnectionType());
          Action action = TimeSpentManager.GetInstance().AppForegrounded();
          if (action == null)
            return;
          App.OnDeactivated.Add(action);
        }));
        BackgroundAgentHelper.KillAgent(false);
        AppState.OnSafeToTouchDatabase();
        AppState.PatchLocale();
        OffOnConverter.On = AppResources.On;
        OffOnConverter.Off = AppResources.Off;
        try
        {
          Backup.OnAppStarted();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "initial restore");
        }
        AppState.ClientInstance = (AppState.Client) new App.Instance();
        this.minimalStartup.Set();
        Log.d(nameof (App), "minimal startup set");
        FieldStatsRunner.InitializeNative();
        this.FindTempFilesForPurge();
        this.DetectVersionChange();
        WAThreadPool.QueueUserWorkItem(new Action(this.TrySync));
        App.ApplicationActivatedSubject.Skip<Unit>(1).Where<Unit>((Func<Unit, bool>) (_ => !App.SuppressSync)).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          ContactStore.InvalidateDeviceContacts();
          this.TrySync();
        }));
        this.InitializeConnection();
        this.RunInBackground((Action) (() =>
        {
          this.InitializeBackgroundTransfers();
          App.ApplicationActivatedSubject.Subscribe<Unit>((Action<Unit>) (_ => this.OnForegrounded()));
          this.CleanupTempFiles();
          if (Settings.LiveLocationData != null)
          {
            LiveLocationManager instance = LiveLocationManager.Instance;
          }
          App.CheckForUpdate();
          if (AppState.UseWindowsNotificationService)
            return;
          Settings.MPNSChatTileExists = TileHelper.ChatTileExists();
        }));
        Observable.Return<Unit>(new Unit()).ObserveOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          while (true)
          {
            AppState.VoipEvent.WaitOne();
            Log.WriteLineDebug("Got a VOIP push - kicking socket...");
            this.ConnectionResetSubject.OnNext(new Unit());
            FunRunner.BackoffEvent.Set();
            Thread.Sleep(TimeSpan.FromSeconds(45.0));
          }
        }));
        System.Windows.Deployment.Current.Dispatcher.BeginInvoke((Action) (() => ResolutionHelper.Init()));
        this.ConnectDatabase();
        this.ConnectNetwork();
        System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          this.DatabaseConnected = true;
          Log.d(nameof (App), nameof (DatabaseConnected));
          this.OnStartupStateChanged();
          if (Settings.E2EVerificationCleanup)
            return;
          AppState.SchedulePersistentAction(PersistentAction.DisplayFullEncryptionToAllChats());
        }));
      }));
      this.InitializeComponent();
      this.InitializePhoneApplication();
      this.InitializeRootFrame();
      if (!App.ExceptionHandlerRegistered)
      {
        this.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(this.Application_UnhandledException);
        App.ExceptionHandlerRegistered = true;
      }
      if (Debugger.IsAttached)
      {
        Application.Current.Host.Settings.EnableFrameRateCounter = true;
        if (PhoneApplicationService.Current != null)
          PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
      }
      Log.d(nameof (App), "completed ctor");
    }

    private void TrySync()
    {
      if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified)
        return;
      if (Settings.LastFullSyncUtc.HasValue)
      {
        Log.d(nameof (TrySync), "SyncBackground");
        ContactStore.SyncBackground();
      }
      else
      {
        Log.d(nameof (TrySync), "SyncReg");
        ContactStore.SyncReg();
      }
    }

    private void InitializeBackgroundTransfers()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = false;
        foreach (Message message in db.GetMessagesWithBackgroundID())
        {
          MessageMiscInfo miscInfo = message.GetMiscInfo((SqliteMessagesContext) db);
          if (miscInfo != null)
          {
            if (miscInfo.BackgroundId != null)
            {
              try
              {
                if (BackgroundTransferService.Find(miscInfo.BackgroundId) == null)
                {
                  miscInfo.BackgroundId = (string) null;
                  flag = true;
                }
              }
              catch (Exception ex)
              {
                Log.l(ex, "bts find");
              }
            }
          }
        }
        if (flag)
          db.SubmitChanges();
        BackgroundTransferRequest[] backgroundTransferRequestArray = (BackgroundTransferRequest[]) null;
        try
        {
          backgroundTransferRequestArray = BackgroundTransferService.Requests.Where<BackgroundTransferRequest>((Func<BackgroundTransferRequest, bool>) (r => r != null)).ToArray<BackgroundTransferRequest>();
        }
        catch (Exception ex)
        {
          Log.l(ex, "bts request enumerate");
        }
        foreach (BackgroundTransferRequest request in backgroundTransferRequestArray ?? new BackgroundTransferRequest[0])
        {
          Message[] messageArray = (Message[]) null;
          Uri requestUri = request.RequestUri;
          if (requestUri != (Uri) null && requestUri.OriginalString != null)
            messageArray = db.GetMessagesWithMediaUrl(requestUri.OriginalString);
          if (messageArray == null || messageArray.Length == 0)
          {
            BackgroundTransferService.Remove(request);
          }
          else
          {
            BackgroundTransferRequest req = request;
            Message[] msgs = messageArray;
            AppState.Worker.Enqueue((Action) (() =>
            {
              foreach (Message message in msgs)
              {
                WhatsApp.Events.MediaDownload mediaDownloadEvent = FieldStats.GetFsMediaDownloadEvent(message);
                message.SetPendingMediaSubscription("Media download", PendingMediaTransfer.TransferTypes.Download_Foreground_NotInteractive, MediaDownload.TransferForMessageObservable(message, MediaDownload.TransferFromForeground(message, mediaDownloadEvent, false).Do<MediaDownload.MediaProgress>((Action<MediaDownload.MediaProgress>) (ev =>
                {
                  if (!ev.BtsFailedToStart)
                    return;
                  BackgroundTransferService.Remove(req);
                })), mediaDownloadEvent));
              }
            }));
          }
        }
      }));
    }

    private void InitializePush()
    {
      this.ChannelUriSubscription.SafeDispose();
      this.ChannelUriSubscription = PushSystem.ForegroundInstance.UriObservable.Subscribe((IObserver<Uri>) this.PushUriSubject);
    }

    private void OnLoggedIn()
    {
      AppState.ProcessSyncPendingNetworkTasks();
      AppState.Worker.Enqueue((Action) (() =>
      {
        App.LastLoginTime = new DateTime?(DateTime.Now);
        AppState.ProcessPendingNetworkTasks();
      }));
      DateTime? nullable;
      if (!Settings.ForceServerPropsReload)
      {
        nullable = Settings.LastPropertiesQueryUtc;
        if (nullable.HasValue)
        {
          nullable = Settings.LastPropertiesQueryUtc;
          if (!(nullable.Value < FunRunner.CurrentServerTimeUtc - Constants.ServerPropPullInterval))
            goto label_4;
        }
      }
      this.Connection.SendGetServerProperties();
label_4:
      nullable = Settings.LastPrivacyCheckUtc;
      if (!nullable.HasValue || Settings.StatusVisibilityInDoubt || Settings.LastSeenVisibilityInDoubt || Settings.ProfilePhotoVisibilityInDoubt)
        this.Connection.SendGetPrivacySettings();
      nullable = Settings.LastBlockListCheckUtc;
      if (!nullable.HasValue)
        this.Connection.SendGetPrivacyList();
      if (Settings.OldChatID != null)
        AccountManagement.ChangePhoneNumber();
      if (Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.Undefined)
        return;
      this.Connection.SendGetStatusV3PrivacyLists((Action) null);
    }

    private void OnPushUri(Uri uri)
    {
      FunXMPP.Connection.GroupSetting[] array = ((IEnumerable<JidInfo>) new JidInfo[0]).Select<JidInfo, FunXMPP.Connection.GroupSetting>((Func<JidInfo, FunXMPP.Connection.GroupSetting>) (ji => new FunXMPP.Connection.GroupSetting()
      {
        Jid = ji.Jid,
        Enabled = true,
        MuteExpiry = ji.MuteExpirationUtc
      })).ToArray<FunXMPP.Connection.GroupSetting>();
      Log.l("app", "sending push URI");
      AppState.SendPushConfig(uri, (IEnumerable<FunXMPP.Connection.GroupSetting>) array, onError: (Action<int>) (errorCode =>
      {
        if (errorCode != 500)
          return;
        Log.l("app", "push URI {0} was rejected.", (object) uri.OriginalString);
        DateTime? channelReopenUtc = Settings.LastChannelReopenUtc;
        if (channelReopenUtc.HasValue)
        {
          DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
          DateTime? nullable1 = channelReopenUtc;
          TimeSpan? nullable2 = nullable1.HasValue ? new TimeSpan?(currentServerTimeUtc - nullable1.GetValueOrDefault()) : new TimeSpan?();
          TimeSpan timeSpan = TimeSpan.FromHours(6.0);
          if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() < timeSpan ? 1 : 0) : 0) != 0)
          {
            Log.l("app", "last attempted reset was too recent.  Not asking for a new push channel just yet.");
            return;
          }
        }
        Log.l("app", "requesting new push URI.");
        PushSystem.ForegroundInstance.RequestNewUri();
        this.InitializePush();
      }));
      this.minimalStartup.WaitOne();
      byte[] hash = MD5Core.GetHash(Encoding.UTF8.GetBytes(uri.OriginalString));
      byte[] lastPushUriHash = Settings.LastPushUriHash;
      int index = 0;
      bool flag;
      for (flag = hash.Length == lastPushUriHash.Length; flag && index < hash.Length; ++index)
        flag = (int) hash[index] == (int) lastPushUriHash[index];
      if (!flag)
      {
        Settings.LastPushUriHash = hash;
        ++Settings.PushUriCount;
      }
      PushSystem.ForegroundInstance.BindPush();
      if (AppState.IsVoipScheduled())
        return;
      TileHelper.ClearMainTile();
    }

    public static void CheckForUpdate(Action onComplete = null)
    {
      if (onComplete == null)
        onComplete = (Action) (() => { });
      if (Settings.IsUpdateAvailable)
      {
        onComplete();
      }
      else
      {
        if (Settings.LastCheckForUpdatesUtc.HasValue)
        {
          DateTime utcNow = DateTime.UtcNow;
          DateTime dateTime = Settings.LastCheckForUpdatesUtc.Value;
          if (utcNow > dateTime && utcNow - dateTime < TimeSpan.FromDays(1.0))
          {
            onComplete();
            return;
          }
        }
        string chatId = Settings.ChatID;
        string str = "00";
        if (!string.IsNullOrEmpty(chatId) && chatId.Length > 2)
          str = chatId.Substring(chatId.Length - 3, 2);
        App.GetAvailableVersionNumber(string.Format("https://beta.whatsapp.com/wp/{0}/WhatsApp.version", (object) str)).Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => onComplete()), (Action<Exception>) (ex => onComplete()));
      }
    }

    private static IObservable<Unit> GetAvailableVersionNumber(string versionUri)
    {
      Func<Exception, IObservable<string>> handler = (Func<Exception, IObservable<string>>) (ex =>
      {
        Log.l(ex, "trying to check for latest available version number");
        return Observable.Return<string>((string) null);
      });
      return NativeWeb.SimpleGet(versionUri, flags: NativeWeb.Options.CacheDns).Select<Stream, string>((Func<Stream, string>) (stream =>
      {
        Log.l("app", "check update | got response");
        string str = (string) null;
        using (stream)
          str = new StreamReader(stream).ReadToEnd();
        return (str ?? "").Trim();
      })).Timeout<string>(TimeSpan.FromSeconds(30.0)).Catch<string, Exception>(handler).Take<string>(1).Where<string>((Func<string, bool>) (_ => _ != null)).ObserveOnDispatcher<string>().Select<string, Unit>((Func<string, Unit>) (availableVersionNumber =>
      {
        string appVersion = AppState.GetAppVersion();
        int[] version1 = App.ParseVersion(appVersion);
        int[] version2 = App.ParseVersion(availableVersionNumber);
        bool flag = false;
        int index1 = 0;
        for (int index2 = Math.Min(version1.Length, version2.Length); index1 < index2; ++index1)
        {
          if (version1[index1] < version2[index1])
          {
            flag = true;
            break;
          }
        }
        Log.l("app", "check update | current:{0},latest:{1} | {2}", (object) appVersion, (object) availableVersionNumber, flag ? (object) "new version found" : (object) "up-to-date");
        Settings.IsUpdateAvailable = flag;
        Settings.LastCheckForUpdatesUtc = new DateTime?(DateTime.UtcNow);
        return new Unit();
      }));
    }

    public static int[] ParseVersion(string version)
    {
      return ((IEnumerable<string>) version.Split('.')).Select<string, int>((Func<string, int>) (s =>
      {
        int result = 0;
        int.TryParse(s, out result);
        return result;
      })).ToArray<int>();
    }

    private void FindTempFilesForPurge()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        IEnumerable<WIN32_FIND_DATA> source = (IEnumerable<WIN32_FIND_DATA>) null;
        try
        {
          source = (IEnumerable<WIN32_FIND_DATA>) nativeMediaStorage.FindFiles(Constants.IsoStorePath + "\\tmp\\*").Where<WIN32_FIND_DATA>((Func<WIN32_FIND_DATA, bool>) (f => f.cFileName != "." && f.cFileName != "..")).ToArray<WIN32_FIND_DATA>();
        }
        catch (Exception ex1)
        {
          try
          {
            using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
              storeForApplication.CreateDirectory("tmp");
          }
          catch (Exception ex2)
          {
          }
        }
        if (source == null)
          return;
        try
        {
          this.tempFilesToDelete_ = source.Where<WIN32_FIND_DATA>((Func<WIN32_FIND_DATA, bool>) (f => !f.IsDirectory())).Select<WIN32_FIND_DATA, string>((Func<WIN32_FIND_DATA, string>) (f => "tmp\\" + f.cFileName)).Concat<string>(nativeMediaStorage.FindFiles(Constants.IsoStorePath + "\\*").Where<WIN32_FIND_DATA>((Func<WIN32_FIND_DATA, bool>) (f => !f.IsDirectory() && f.cFileName.EndsWith(".tmp", StringComparison.InvariantCultureIgnoreCase))).Select<WIN32_FIND_DATA, string>((Func<WIN32_FIND_DATA, string>) (f => f.cFileName))).ToArray<string>();
        }
        catch (Exception ex)
        {
          Log.l(ex, "find temp files");
        }
        DateTime dateTime = DateTime.Now;
        dateTime = dateTime.AddDays(-1.0);
        long time = dateTime.ToFileTime();
        string[] dirs = source.Where<WIN32_FIND_DATA>((Func<WIN32_FIND_DATA, bool>) (f => f.IsDirectory() && f.ftLastWriteTime < time)).Select<WIN32_FIND_DATA, string>((Func<WIN32_FIND_DATA, string>) (f => f.cFileName)).ToArray<string>();
        if (dirs.Length != 0)
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
          {
            string prefix = Constants.IsoStorePath + "\\tmp\\";
            foreach (string path in ((IEnumerable<string>) dirs).Select<string, string>((Func<string, string>) (path => prefix + path)))
            {
              try
              {
                NativeInterfaces.Misc.RemoveDirectoryRecursive(path);
              }
              catch (Exception ex)
              {
                Log.l(ex, "remove temp dir");
              }
            }
          }));
        Log.d("app", "find temp files to purge | counts: {0}, {1}", (object) source.Count<WIN32_FIND_DATA>(), (object) dirs.Length);
      }
    }

    private void CleanupTempFiles()
    {
      string[] strArray = this.tempFilesToDelete_ ?? new string[0];
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        foreach (string file in strArray)
        {
          try
          {
            storeForApplication.DeleteFile(file);
            Log.d("app", "deleted tmp file {0}", (object) file);
          }
          catch (Exception ex)
          {
          }
        }
      }
      DateTime? lastMediaSweepTime = Settings.LastMediaSweepTime;
      if (lastMediaSweepTime.HasValue)
      {
        DateTime? nullable = lastMediaSweepTime;
        DateTime dateTime = DateTime.UtcNow.AddDays(-7.0);
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTime ? 1 : 0) : 0) == 0)
          return;
      }
      WAThreadPool.QueueUserWorkItem((Action) (() => new StaleMediaSweeper().Sweep()));
    }

    private void InitializeConnection()
    {
      this.Connection = new FunXMPP.Connection();
      FunEventHandler funEventHandler = new FunEventHandler(this.Connection);
      this.Connection.EventHandler = (FunXMPP.Listener) funEventHandler;
      this.Connection.GroupEventHandler = (FunXMPP.GroupListener) funEventHandler;
      this.Connection.VoipEventHandler = (FunXMPP.VoipListener) VoipSignaling.Instance;
      VoipHandler.IncomingCallSubject.ObserveOnDispatcher<WaCallEventArgs>().Subscribe<WaCallEventArgs>((Action<WaCallEventArgs>) (args => CallScreenPage.Launch(args.PeerJid, new UiCallState?(UiCallState.ReceivedCall))));
      VoipHandler.BatteryLevelChanged.ObserveOnDispatcher<WaCallBatteryLevelLowArgs>().Subscribe<WaCallBatteryLevelLowArgs>((Action<WaCallBatteryLevelLowArgs>) (args =>
      {
        string msg = (string) null;
        switch (args.Source)
        {
          case UiBatteryLevelSource.Self:
            msg = AppResources.CallSelfBatteryLow;
            break;
          case UiBatteryLevelSource.Peer:
            msg = string.Format(AppResources.CallPeerBatteryLow, (object) UserCache.Get(args.PeerJid, true).GetDisplayName(true));
            break;
        }
        if (msg == null)
          return;
        InAppToast.ShowForCallBatteryLevel(msg);
      }));
      VoipHandler.AudioFallbackSubject.ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => InAppToast.ShowForError(AppResources.CallFallbackToAudio)));
      this.Connection.LoginSubject.Subscribe<WAProtocol>((Action<WAProtocol>) (_ => this.OnLoggedIn()));
      this.PushUriObservable.CombineLatest<Uri, Unit, Uri>((IObservable<Unit>) ((FunEventHandler) this.Connection.EventHandler).OfflineCompletedSubject, (Func<Uri, Unit, Uri>) ((uri, login) => uri)).ObserveOn<Uri>((IScheduler) AppState.Worker).Subscribe<Uri>(new Action<Uri>(this.OnPushUri));
    }

    private void InitializeFlowDirection()
    {
      try
      {
        this.RootFrame.FlowDirection = new CultureInfo(AppResources.CultureString).IsRightToLeft() ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
      }
      catch (Exception ex)
      {
        Log.l(ex, "FlowDirection init");
      }
    }

    private void InitializeRootFrame()
    {
      this.InitializeFlowDirection();
      this.RootFrame.Navigating += new NavigatingCancelEventHandler(this.RootFrame_Navigating);
      this.RootFrame.Navigated += new NavigatedEventHandler(this.RootFrame_Navigated);
      if (!AppState.IsWP10OrLater)
        return;
      if (DeviceProfile.Instance.HasSoftNavBar)
      {
        Thickness margin = this.RootFrame.Margin;
        margin.Bottom += Math.Floor(12.0 * ResolutionHelper.ZoomMultiplier);
        this.RootFrame.Margin = margin;
      }
      this.RootFrame.GetType().GetEvent("NavigationBarVisibilityChanged", BindingFlags.Instance | BindingFlags.Public)?.AddMethod.Invoke((object) this.RootFrame, new object[1]
      {
        (object) new EventHandler(this.OnNavBarVisibilityChanged)
      });
    }

    public void OnNavBarVisibilityChanged(object sender, EventArgs e)
    {
      PropertyInfo propertyInfo = ((IEnumerable<PropertyInfo>) e.GetType().GetProperties()).SingleOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name == "OccludedHeight"));
      if (propertyInfo == null)
        return;
      this.RootFrame.Margin = new Thickness(0.0, 0.0, 0.0, (double) propertyInfo.GetValue((object) e));
    }

    public void ScheduleRootFrameNavigatedWork(Action a) => this.scheduledNavigatedWork.AddLast(a);

    private void PerformRootFrameNavigatedWorks()
    {
      if (!this.scheduledNavigatedWork.Any<Action>())
        return;
      Action[] array = this.scheduledNavigatedWork.ToArray<Action>();
      this.scheduledNavigatedWork.Clear();
      foreach (Action action in array)
      {
        if (action != null)
          action();
      }
    }

    public bool ToHandleFastResumeTargetNav
    {
      set => this.toHandleFastResumeTargetNav = value;
    }

    private bool ShouldPreserveOnFastResume(string currentPageUri)
    {
      if (this.preservedPageUris == null)
      {
        List<string> list = ((IEnumerable<string>) new string[6]
        {
          "ContactsPage",
          "CallScreenPage",
          "JidItemPickerPage",
          "PicturePreviewPage",
          "VideoPlayerPage",
          "GdprAgreementPage"
        }).Select<string, string>((Func<string, string>) (pageName => UriUtils.CreatePageUriStr(pageName))).ToList<string>();
        list.Add(UriUtils.CreatePageUriStr("GdprReportPage", (WaUriParams) null, "Pages/Settings"));
        this.preservedPageUris = list.ToArray();
      }
      foreach (string preservedPageUri in this.preservedPageUris)
      {
        if (currentPageUri.StartsWith(preservedPageUri))
        {
          if (!preservedPageUri.StartsWith(UriUtils.CreatePageUriStr("JidItemPickerPage")))
            return true;
          JournalEntry journalEntry = this.CurrentPage.NavigationService.BackStack.FirstOrDefault<JournalEntry>();
          return journalEntry != null && journalEntry.Source.OriginalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage"));
        }
      }
      return false;
    }

    private void HandleFastResumeTargetNavigation(NavigatingCancelEventArgs e)
    {
      if (this.resetNavUri == null)
        return;
      string resetNavUri = this.resetNavUri;
      this.resetNavUri = (string) null;
      if (e.Cancel)
        Log.l("app", "fast resume | target nav canceled");
      else if (e.NavigationMode != NavigationMode.New)
      {
        Log.l("app", "fast resume | expected mode=New navigation didn't come");
      }
      else
      {
        Uri uri = e.Uri;
        string originalString = uri.OriginalString;
        Log.l("app", "fast resume | target_uri={0}", (object) originalString);
        WaUriParams waUriParams1 = WaUriParams.FromUri(uri);
        if (ExternalShare81.ShareContentNavigation)
          Log.l("app", "fast resume | to share content page");
        else if (originalString.StartsWith("/PageSelect", StringComparison.OrdinalIgnoreCase))
        {
          if (this.ShouldPreserveOnFastResume(resetNavUri))
          {
            e.Cancel = true;
            Log.l("app", "fast resume | preserve current page | uri={0}", (object) resetNavUri);
          }
        }
        else if (!originalString.StartsWith(WaUris.ChatPageStr()))
        {
          if (!originalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")))
          {
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("JidItemPickerPage")))
            {
              JournalEntry journalEntry = this.CurrentPage.NavigationService.BackStack.FirstOrDefault<JournalEntry>();
              if ((journalEntry != null ? (journalEntry.Source.OriginalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")) ? 1 : 0) : 0) != 0)
                goto label_15;
            }
            if (originalString.StartsWith("/ChatPage.xaml", StringComparison.OrdinalIgnoreCase))
            {
              Log.l("app", "fast resume | from non-voip toast | to target chat | uri={0}", (object) originalString);
              goto label_21;
            }
            else
            {
              Log.l("app", "fast resume | unexpected target uri | uri={0}", (object) originalString);
              goto label_21;
            }
          }
label_15:
          string val = (string) null;
          waUriParams1.TryGetStrValue("jid", out val);
          if (JidHelper.IsUserJid(val) && resetNavUri.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")))
          {
            WaUriParams waUriParams2 = WaUriParams.FromUriString(resetNavUri);
            string str = (string) null;
            ref string local = ref str;
            if (waUriParams2.TryGetStrValue("jid", out local) && str == val)
            {
              e.Cancel = true;
              Log.l("app", "fast resume | preserve current call screen | uri={0}", (object) resetNavUri);
            }
          }
        }
label_21:
        if (e.Cancel)
          return;
        Log.l("app", "fast resume | to target uri | uri={0}", (object) originalString);
        DateTime start = DateTime.UtcNow;
        this.RootFrame.GetNavigatedAsync().Take<NavigationEventArgs>(1).Subscribe<NavigationEventArgs>((Action<NavigationEventArgs>) (args =>
        {
          if ((DateTime.UtcNow - start).Seconds > 3)
            Log.d(new Exception(), "fast resume | took longer than expected to hit 'New' nav after 'Reset'");
          if (args.NavigationMode == NavigationMode.New)
          {
            NavUtils.ClearBackStack();
            Log.l("app", "fast resume | clear backstack on nav finish | uri:{0}", (object) args.Uri.OriginalString);
          }
          else
            Log.l("app", "fast resume | expected mode=New nav didn't come");
        }));
      }
    }

    private void RootFrame_BackKeyPress(object sender, CancelEventArgs e)
    {
      if (this.RootFrame.BackStack.Any<JournalEntry>() || this.CurrentPage is ContactsPage)
        return;
      Log.l("app", "wrong back stack state | redirect to home");
      e.Cancel = true;
      System.Windows.Deployment.Current.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateHome()));
    }

    private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
      this.minimalStartup.WaitOne();
      if (this.currPageWrapper != null)
        this.currPageWrapper.ClosePendingContextMenu();
      string originalString = e.Uri.OriginalString;
      Log.l("Navigating", "uri:{0},mode:{1}", (object) originalString, (object) e.NavigationMode);
      Stats.LogMemoryUsage();
      this.navigatingSubject.OnNext(e);
      if (originalString.StartsWith("app://external/", StringComparison.OrdinalIgnoreCase))
      {
        this.toHandleFastResumeTargetNav = false;
        if (e.Cancel)
          return;
        App.ApplicationActivatedSubject.Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => AppState.ProcessResetActions()));
        this.navigatingSubject.Where<NavigatingCancelEventArgs>((Func<NavigatingCancelEventArgs, bool>) (args => !args.Cancel && !args.Uri.OriginalString.StartsWith("app://external", StringComparison.OrdinalIgnoreCase))).Take<NavigatingCancelEventArgs>(1).Subscribe<NavigatingCancelEventArgs>((Action<NavigatingCancelEventArgs>) (_ => this.Activate(true)));
        this.navFromExternal = true;
      }
      else
      {
        if (e.NavigationMode == NavigationMode.Back)
          TimeSpentManager.GetInstance().UserAction();
        Uri redirectedUri = (Uri) null;
        this.backStackOpParams = (NavUtils.BackStackOpParams) null;
        if (e.NavigationMode == NavigationMode.Reset)
        {
          Log.l("app", "fast resume | reset_uri={0}", (object) originalString);
          this.toHandleFastResumeTargetNav = true;
          this.resetNavUri = originalString;
        }
        else
        {
          int num = this.toHandleFastResumeTargetNav ? 1 : 0;
          this.toHandleFastResumeTargetNav = false;
          if (num != 0 && e.NavigationMode == NavigationMode.New)
          {
            if (this.CurrentPage is ChatPage currentPage && currentPage.IsPickingFiles)
            {
              Log.l("app", "fast resume | cancel target nav from MS filepicker bug");
              e.Cancel = true;
              return;
            }
            this.ValidateNavigation(e, true, false, false, out redirectedUri, out this.backStackOpParams);
            if (redirectedUri == (Uri) null)
            {
              this.HandleFastResumeTargetNavigation(e);
              if (e.Cancel)
              {
                Log.l("app", "fast resume | nav canceled");
                if (!Voip.IsInCall || this.CurrentPage is CallScreenPage)
                  return;
                if (this.CurrentPage is JidItemPickerPage)
                {
                  JournalEntry journalEntry = this.CurrentPage.NavigationService.BackStack.FirstOrDefault<JournalEntry>();
                  if ((journalEntry != null ? (journalEntry.Source.OriginalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")) ? 1 : 0) : 0) != 0)
                    return;
                }
                InAppVoipBanner.ShowAsync();
                return;
              }
            }
            else
              this.toHandleFastResumeTargetNav = true;
          }
          if (redirectedUri == (Uri) null)
            this.ValidateNavigation(e, this.navFromExternal, true, true, out redirectedUri, out this.backStackOpParams);
          this.navFromExternal = false;
          if (redirectedUri != (Uri) null)
          {
            Log.l("app", "nav redirect | target_uri={0} new_uri={1}", (object) originalString, (object) redirectedUri.OriginalString);
            e.Cancel = true;
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.RootFrame.Navigate(redirectedUri)));
          }
          else if (this.RootFrame != null && this.RootFrame.CurrentSource != (Uri) null)
            this.RecentNavigatedFromUri = this.RootFrame.CurrentSource.OriginalString;
          else
            this.RecentNavigatedFromUri = (string) null;
        }
      }
    }

    private void RootFrame_Navigated(object sender, NavigationEventArgs e)
    {
      NavigationMode navigationMode = e.NavigationMode;
      Log.d("Navigated", "uri:{0} mode:{1}", (object) e.Uri.OriginalString, (object) navigationMode);
      FieldStats.MaybeReportAppLaunch(navigationMode == NavigationMode.Back || navigationMode == NavigationMode.Reset);
      this.currPageWrapper.SafeDispose();
      this.currPageWrapper = (PageWrapper) null;
      PhoneApplicationPage currentPage = this.CurrentPage;
      if (currentPage != null)
        this.currPageWrapper = PageWrapper.Create(currentPage);
      if (navigationMode == NavigationMode.New && this.backStackOpParams != null && this.backStackOpParams.IsActionable)
      {
        NavUtils.BackStackOpParams bsoParams = this.backStackOpParams;
        this.backStackOpParams = (NavUtils.BackStackOpParams) null;
        System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          bool flag1 = bsoParams.ClearBackStack;
          int num1 = bsoParams.BackStackEntriesToRemove;
          if (!flag1 && !string.IsNullOrEmpty(bsoParams.ClearBackStackTo))
          {
            int num2 = 0;
            bool flag2 = false;
            foreach (JournalEntry back in this.RootFrame.BackStack)
            {
              if (back.Source.OriginalString.IndexOf(bsoParams.ClearBackStackTo + ".xaml") != -1)
              {
                flag2 = true;
                break;
              }
              ++num2;
            }
            if (flag2)
              num1 = num2;
            else
              flag1 = true;
          }
          if (flag1)
            num1 = this.RootFrame.BackStack.Count<JournalEntry>();
          if (num1 <= 0)
            return;
          for (int index = 0; index < num1; ++index)
            this.RootFrame.RemoveBackEntry();
        }));
      }
      if ((e.NavigationMode == NavigationMode.Back || e.NavigationMode == NavigationMode.New) && Voip.IsInCall && !e.Uri.OriginalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")))
      {
        if (e.Uri.OriginalString.StartsWith(UriUtils.CreatePageUriStr("JidItemPickerPage")))
        {
          JournalEntry journalEntry = this.CurrentPage.NavigationService.BackStack.FirstOrDefault<JournalEntry>();
          if ((journalEntry != null ? (journalEntry.Source.OriginalString.StartsWith(UriUtils.CreatePageUriStr("CallScreenPage")) ? 1 : 0) : 0) != 0)
            goto label_8;
        }
        InAppVoipBanner.ShowAsync();
        goto label_9;
      }
label_8:
      InAppVoipBanner.CloseInstance();
label_9:
      this.PerformRootFrameNavigatedWorks();
      if (!this.shouldTryEnablePauseOnBack || currentPage == null)
        return;
      this.shouldTryEnablePauseOnBack = false;
      if (!AppState.IsDecentMemoryDevice)
        return;
      currentPage.NavigationService.PauseOnBack = true;
    }

    private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      string str = (string) null;
      if (e.Uri != (Uri) null)
        str = e.Uri.OriginalString;
      Log.SendCrashLog(e.Exception, string.Format("NavigationFailed, uri = [{0}]", (object) (str ?? "")));
      if (!Debugger.IsAttached)
        return;
      Debugger.Break();
    }

    private void RootFrame_Obscured(object sender, ObscuredEventArgs e)
    {
      Log.l("app", "obscured");
      AppState.ObscuredEvent.Set();
    }

    private void RootFrame_Unobscured(object sender, EventArgs e)
    {
      Log.l("app", "unobscured");
      AppState.ObscuredEvent.Reset();
    }

    public void ValidateNavigation(
      NavigatingCancelEventArgs e,
      bool fromExternal,
      bool processPageSelect,
      bool processChatPage,
      out Uri newUri,
      out NavUtils.BackStackOpParams backStackOpParams)
    {
      newUri = (Uri) null;
      backStackOpParams = (NavUtils.BackStackOpParams) null;
      if (e == null)
        return;
      this.ValidateNavigation(e.Uri, e.NavigationMode, fromExternal, processPageSelect, processChatPage, out newUri, out backStackOpParams);
    }

    public void ValidateNavigation(
      Uri pageUri,
      NavigationMode navMode,
      bool fromExternal,
      bool processPageSelect,
      bool processChatPage,
      out Uri newUri,
      out NavUtils.BackStackOpParams backStackOpParams)
    {
      bool flag1 = false;
      newUri = (Uri) null;
      backStackOpParams = (NavUtils.BackStackOpParams) null;
      if (string.IsNullOrEmpty(pageUri?.OriginalString))
      {
        Log.d("app", "validate nav | skip | empty uri");
      }
      else
      {
        string originalString = pageUri.OriginalString;
        Log.d("app", "validate nav | uri:{0},from external:{1}", (object) originalString, (object) fromExternal);
        if (originalString.StartsWith("app://"))
          return;
        bool flag2 = true;
        bool flag3 = originalString.StartsWith("/PageSelect", StringComparison.OrdinalIgnoreCase);
        if (fromExternal)
        {
          bool flag4 = GdprTos.ShouldShowOnAppEntry(true);
          if (AppState.IsExpired)
          {
            App.LogExpiryRelatedVariables("AppState.IsExpired");
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("UpdateVersion")))
              flag1 = true;
            else
              newUri = UriUtils.CreatePageUri("UpdateVersion", "ClearStack=true");
          }
          else if (App.ShouldPromptExpirationWarning())
          {
            App.LogExpiryRelatedVariables("ShouldPromptExpirationWarning");
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("UpdateVersion")))
            {
              flag1 = true;
            }
            else
            {
              WaUriParams uriParams = new WaUriParams();
              uriParams.AddBool("ClearStack", true);
              uriParams.AddBool("UpdateLater", true);
              newUri = UriUtils.CreatePageUri("UpdateVersion", uriParams);
            }
          }
          else if (flag4)
          {
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("GdprAgreementPage")))
            {
              flag1 = true;
            }
            else
            {
              WaUriParams uriParams = new WaUriParams();
              uriParams.AddBool("ClearStack", true);
              uriParams.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
              newUri = UriUtils.CreatePageUri("GdprAgreementPage", uriParams);
            }
          }
          else if (!Settings.EULAAcceptedUtc.HasValue)
          {
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("EULA")))
              flag1 = true;
            else
              newUri = UriUtils.CreatePageUri("EULA", "ClearStack=true");
          }
          else if (!flag3 && Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified && !Settings.LoginFailed && (!Settings.LastFullSyncUtc.HasValue || BackgroundDataDisabledPage.Applicable))
            newUri = WaUris.HomePage();
          else if (Settings.CorruptDb || SqliteRepair.IsRepairDone())
          {
            if (originalString.StartsWith(UriUtils.CreatePageUriStr("HistoryRepair")))
              flag1 = true;
            else
              newUri = UriUtils.CreatePageUri("HistoryRepair", "ClearStack=true");
          }
          else
          {
            DiskSpace diskSpace = NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath);
            if (diskSpace.FreeBytes / 1024UL / 1024UL < 15UL || diskSpace.FreeBytes / 1024UL / 1024UL < 100UL)
            {
              if (originalString.StartsWith(UriUtils.CreatePageUriStr("LowSpaceWarning")))
              {
                flag1 = true;
              }
              else
              {
                newUri = UriUtils.CreatePageUri("LowSpaceWarning", "ClearStack=true");
                Log.d("app", "low storage warning displayed: {0} mb free, {1} mb capacity ({2}% free)", (object) (diskSpace.FreeBytes / 1024UL / 1024UL), (object) (diskSpace.TotalBytes / 1024UL / 1024UL), (object) (ulong) (diskSpace.TotalBytes == 0UL ? 0L : (long) (diskSpace.FreeBytes * 100UL / diskSpace.TotalBytes)));
              }
            }
          }
        }
        WaUriParams uriParams1 = WaUriParams.FromUri(pageUri);
        if (!(newUri != (Uri) null | flag1))
        {
          if (AppState.IsPhoneTimeBadlySkewed())
          {
            if (!originalString.StartsWith(UriUtils.CreatePageUriStr("ClockSkewPage")) && !originalString.StartsWith(UriUtils.CreatePageUriStr("UpdateVersion")))
              newUri = UriUtils.CreatePageUri("ClockSkewPage");
          }
          else if (flag3)
          {
            if (processPageSelect)
            {
              if (string.IsNullOrEmpty(Settings.PhoneNumber) || string.IsNullOrEmpty(Settings.CountryCode) || Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified && string.IsNullOrEmpty(Settings.ChatID))
              {
                Log.l("app", "phone num:{0},country:{1},state:{2},chat id:{3}", (object) Settings.PhoneNumber, (object) Settings.CountryCode, (object) Settings.PhoneNumberVerificationState, (object) Settings.ChatID);
                Settings.PhoneNumberVerificationState = PhoneNumberVerificationState.NewlyEntered;
                newUri = UriUtils.CreatePageUri("PhoneNumberEntry", "ClearStack=true");
              }
              else if (Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified)
              {
                GdprReport.DeleteReport(true, "need to reg/verify");
                newUri = UriUtils.CreatePageUri("VerifyStart", "ClearStack=true", "verify");
              }
              else if (string.IsNullOrEmpty(Settings.PushName) || Settings.ShowPushNameScreen)
              {
                Log.l("app", "displaying push screen - {0}, {1}", (object) Settings.PushName, (object) Settings.ShowPushNameScreen);
                this.resetConnOnStartup = true;
                if (!Settings.IsWaAdmin && this.ConnectionResetSubject != null)
                  this.ConnectionResetSubject.OnNext(new Unit());
                newUri = UriUtils.CreatePageUri("PushNameEntry", "ClearStack=true");
              }
              else if (!Settings.LastFullSyncUtc.HasValue && !CreateFavorites.allowSkipThisPage)
              {
                this.resetConnOnStartup = true;
                newUri = UriUtils.CreatePageUri("CreateFavorites", "ClearStack=true");
              }
              else if (Settings.LoginFailed)
              {
                bool flag5 = Settings.LoginFailedReason != null;
                newUri = UriUtils.CreatePageUri(flag5 ? "LoginBanned" : "LoginInvalid", "ClearStack=true");
              }
              else if (BackgroundDataDisabledPage.Applicable)
                newUri = UriUtils.CreatePageUri("BackgroundDataDisabledPage", "ClearStack=true");
              else if (TwoFactorAuthentication.ShouldPromptCodeEntry)
              {
                newUri = UriUtils.CreatePageUri("ValidateCode", "ClearStack=true", "Pages/TwoFactor");
              }
              else
              {
                if (this.resetConnOnStartup && this.ConnectionResetSubject != null)
                {
                  this.resetConnOnStartup = false;
                  this.ConnectionResetSubject.OnNext(new Unit());
                }
                newUri = !ExternalShare81.ShareContentNavigation ? UriUtils.CreatePageUri("ContactsPage", uriParams1) : UriUtils.CreatePageUri("ShareContent", uriParams1);
              }
            }
          }
          else if (originalString.StartsWith("/ChatPage.xaml", StringComparison.OrdinalIgnoreCase))
          {
            if (processChatPage)
            {
              Log.l("app", "non-voip toast | uri:{0}", (object) originalString);
              uriParams1.AddString("clr2", "ContactsPage");
              uriParams1.AddString("Source", "nvToast");
              uriParams1.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
              newUri = WaUris.ChatPage(uriParams1);
            }
          }
          else if (originalString.StartsWith(UriUtils.CreatePageUriStr("ChatPage"), StringComparison.OrdinalIgnoreCase))
          {
            string val1 = (string) null;
            string val2 = (string) null;
            if (!Settings.LastFullSyncUtc.HasValue || string.IsNullOrEmpty(Settings.PushName) || Settings.LoginFailed)
              newUri = WaUris.HomePage();
            else if (processChatPage)
            {
              if (!uriParams1.TryGetStrValue("Timestamp", out val2) || string.IsNullOrEmpty(val2))
              {
                uriParams1.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
                newUri = WaUris.ChatPage(uriParams1);
              }
              else if (uriParams1.TryGetStrValue("Source", out val1) && val1 == "SecondaryTile" && (!uriParams1.ContainsKey("Timestamp") || !uriParams1.ContainsKey("clr2")))
              {
                uriParams1.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
                uriParams1.AddString("clr2", "ContactsPage");
                newUri = WaUris.ChatPage(uriParams1);
              }
            }
          }
          else if (originalString.StartsWith("/Protocol?"))
          {
            flag2 = false;
            string val = (string) null;
            if (uriParams1.TryGetStrValue("encodedLaunchUri", out val) && val != null)
            {
              int length = val.IndexOf('?');
              if (length >= 0)
              {
                string uri = val.Substring(0, length);
                WaUriParams uriParams2 = WaUriParams.FromUriParamString(val.Substring(length + 1));
                newUri = App.ProcessCustomScheme(uri, uriParams2);
              }
            }
            if (newUri == (Uri) null)
              newUri = new Uri("/PageSelect?ClearStack=true", UriKind.Relative);
          }
        }
        backStackOpParams = !flag2 || navMode != NavigationMode.New || !uriParams1.Any() ? (NavUtils.BackStackOpParams) null : NavUtils.BackStackOpParams.FromUriParams(uriParams1);
        Log.d("app", "validate nav | redirect uri:{0}", (object) (newUri?.OriginalString ?? "n/a"));
      }
    }

    private static Uri ProcessCustomScheme(string uri, WaUriParams uriParams)
    {
      while (uri.Length != 0 && uri[uri.Length - 1] == '/')
        uri = uri.Substring(0, uri.Length - 1);
      if (uri == "whatsapp://send" && Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified)
      {
        string val1 = (string) null;
        uriParams.TryGetStrValue("text", out val1);
        string val2 = (string) null;
        uriParams.TryGetStrValue(DeepLinkData.DeeplinkParamPhone, out val2);
        string val3 = (string) null;
        uriParams.TryGetStrValue(DeepLinkData.DeeplinkParamSource, out val3);
        string val4 = (string) null;
        uriParams.TryGetStrValue(DeepLinkData.DeeplinkParamData, out val4);
        if (!string.IsNullOrEmpty(val1) || !string.IsNullOrEmpty(val2))
        {
          WaUriParams uriParams1 = new WaUriParams();
          uriParams1.AddString("Source", UriShareContent.URI_SCHEME);
          uriParams1.AddString(UriShareContent.SHARE_TYPE_ID, UriShareContent.SHARE_TYPE_DATA);
          if (!string.IsNullOrEmpty(val1))
            uriParams1.AddString(UriShareContent.SHARE_DATA_TEXT, val1);
          if (!string.IsNullOrEmpty(val2))
            uriParams1.AddString(UriShareContent.SHARE_DATA_PHONE, val2);
          if (!string.IsNullOrEmpty(val3))
            uriParams1.AddString(UriShareContent.SHARE_DATA_SOURCE, val3);
          if (!string.IsNullOrEmpty(val4))
            uriParams1.AddString(UriShareContent.SHARE_DATA_DATA, val4);
          return UriUtils.CreatePageUri("ShareContent", uriParams1);
        }
      }
      string val = (string) null;
      if (uri == "whatsapp://r" && Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified && uriParams.TryGetStrValue("c", out val) && !string.IsNullOrEmpty(val))
      {
        WaUriParams uriParams2 = new WaUriParams();
        uriParams2.AddString("type", "sms");
        uriParams2.AddString("code", val);
        return UriUtils.CreatePageUri("EnterCode", uriParams2, "verify");
      }
      val = (string) null;
      if (!(uri == "whatsapp://chat") || Settings.PhoneNumberVerificationState != PhoneNumberVerificationState.Verified || !uriParams.TryGetStrValue("code", out val) || string.IsNullOrEmpty(val))
        return (Uri) null;
      WaUriParams uriParams3 = new WaUriParams();
      uriParams3.AddString("type", "invite");
      uriParams3.AddString("code", val);
      return UriUtils.CreatePageUri("ContactsPage", uriParams3);
    }

    private void Application_Launching(object sender, LaunchingEventArgs e)
    {
      ShareLaunchingEventArgs launchingEventArgs = e as ShareLaunchingEventArgs;
      Log.d("app", "launching sharing {0}", (object) (launchingEventArgs != null));
      if (launchingEventArgs == null)
        return;
      ExternalShare81.ShareOperation = launchingEventArgs.ShareTargetActivatedEventArgs.ShareOperation;
    }

    private void RunInBackground(Action a) => AppState.Worker.Enqueue(a);

    public void ConnectDatabase()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (this.MessagesTableOutgoingSubscription == null)
          this.MessagesTableOutgoingSubscription = db.NewMessagesSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyFromMe)).Subscribe<Message>((Action<Message>) (msg =>
          {
            Action a = (Action) null;
            if (msg.Status == FunXMPP.FMessage.Status.Unsent || msg.Status == FunXMPP.FMessage.Status.Relay)
              a = (Action) (() => AppState.SendMessage(this.Connection, msg));
            else if ((msg.Status == FunXMPP.FMessage.Status.Uploading || msg.Status == FunXMPP.FMessage.Status.UploadingCustomHash) && !msg.UploadContext.isActiveStreamingUpload())
              a = (Action) (() => msg.SetPendingMediaSubscription("Media upload on db connect", PendingMediaTransfer.TransferTypes.Upload_NotWeb, MediaUpload.SendMediaObservable(msg)));
            else if (msg.Status == FunXMPP.FMessage.Status.Pending)
              a = (Action) (() => AppState.ProcessPendingMessage(msg));
            if (a == null)
              return;
            AppState.Worker.Enqueue(a);
            if (!Settings.KeydBackoffUtc.HasValue)
              return;
            this.Connection.Encryption.ResetKeydBackoff();
          }));
        if (this.MessagesTableIncomingSubscription == null)
          this.MessagesTableIncomingSubscription = db.NewMessagesSubject.Merge<Message>((IObservable<Message>) MediaDownload.VoiceMessageDownloadEndedSubj).Merge<Message>(db.UpdatedMessageMediaWaTypeSubject.Where<Message>((Func<Message, bool>) (msg =>
          {
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
              return false;
            bool shouldUpdate = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 =>
            {
              if (db2.GetMessagesAfterCount(msg) >= 9)
                return;
              shouldUpdate = true;
            }));
            return shouldUpdate;
          }))).Where<Message>((Func<Message, bool>) (msg =>
          {
            bool flag = msg.ShouldAlert();
            if (GdprTos.ShouldBlockNotifications())
            {
              flag = false;
              Log.l("app", "block new msg alert | gdpr stage:{0}", (object) Settings.GdprTosCurrentStage);
            }
            Log.d("app", "{0} new msg alert | keyid:{1},jid:{2},from self:{3},type:{4}", flag ? (object) "proceed" : (object) "skip", (object) msg.KeyId, (object) msg.KeyRemoteJid, (object) msg.KeyFromMe, (object) msg.MediaWaType);
            return flag;
          })).ObserveOnDispatcher<Message>().Do<Message>((Action<Message>) (msg =>
          {
            ITile chatTile = TileHelper.GetChatTile(msg.KeyRemoteJid);
            if (chatTile == null)
              return;
            Conversation convo = (Conversation) null;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 => convo = db2.GetConversation(msg.KeyRemoteJid, CreateOptions.None)));
            if (convo == null)
              return;
            int? lastMessageId = convo.LastMessageID;
            int messageId = msg.MessageID;
            if ((lastMessageId.GetValueOrDefault() == messageId ? (lastMessageId.HasValue ? 1 : 0) : 0) != 0)
            {
              Log.d("app", "process new msg alert | update tile count | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
              chatTile.SetChatCountAndContent(convo.GetUnreadMessagesCount(), TileDataSource.CreateForMessage(msg));
            }
            else
              Log.l("app", "process new msg alert | skip update tile count | keyid:{0},mid:{1},real last msg:{2}", (object) msg.KeyId, (object) msg.MessageID, (object) convo.LastMessageID);
          })).Sample<Message>(TimeSpan.FromSeconds(1.0)).ObserveOnDispatcher<Message>().Subscribe<Message>((Action<Message>) (msg =>
          {
            if (msg.ShouldAutoMute())
            {
              Log.l("app", "process new msg alert | skip | auto muted | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
            }
            else
            {
              PhoneApplicationPage currentPage = App.CurrentApp.CurrentPage;
              ChatPage chatPage = currentPage as ChatPage;
              int num3 = !(currentPage is ContactsPage contactsPage2) ? 0 : (contactsPage2.IsOnChatList ? 1 : 0);
              bool flag4 = chatPage != null && chatPage.Jid == msg.KeyRemoteJid;
              bool flag5 = flag4 && chatPage.IsScrolledToRecent;
              int num4 = flag4 ? 1 : 0;
              bool flag6 = (num3 | num4) != 0;
              if (!flag6)
              {
                bool shouldDelayNotify = false;
                MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db2 => shouldDelayNotify = msg.ShouldDelayNotifyUntilAutoDownloadAttempt(db2)));
                if (shouldDelayNotify)
                {
                  Log.l("app", "process new msg alert | delay | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
                  return;
                }
              }
              msg.IsAlerted = true;
              if ((flag6 ? 1 : (!Settings.EnableInAppNotificationToast ? 1 : 0)) != 0)
              {
                Log.d("app", "process new msg alert | skip toast | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
              }
              else
              {
                Log.d("app", "process new msg alert | toast | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
                InAppToast.ShowForMessage(msg);
              }
              if (((ChatPage.Current != null && ChatPage.Current.IsPttRecordingInProgress || !Settings.EnableInAppNotificationVibrate ? 1 : (Voip.IsInCall ? 1 : 0)) | (flag5 ? 1 : 0)) != 0)
              {
                Log.d("app", "process new msg alert | skip buzz | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
              }
              else
              {
                Log.d("app", "process new msg alert | buzz | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
                VibrateController.Default.Start(TimeSpan.FromMilliseconds(200.0));
              }
              if ((ChatPage.Current == null || !ChatPage.Current.IsPttRecordingInProgress ? (!Settings.EnableInAppNotificationSound ? 1 : 0) : 1) != 0)
              {
                Log.d("app", "process new msg alert | skip sound | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
              }
              else
              {
                Log.d("app", "process new msg alert | sound | keyid:{0},jid:{1}", (object) msg.KeyId, (object) msg.KeyRemoteJid);
                this.PlayInAppNotificationSound(msg.KeyRemoteJid);
              }
            }
          }));
        if (this.ConversationDeleteSubscription != null)
          return;
        this.ConversationDeleteSubscription = db.DeletedConversationSubject.Subscribe<Conversation>((Action<Conversation>) (c =>
        {
          if (c.IsBroadcast())
            AppState.SchedulePersistentAction(PersistentAction.SendDeleteBroadcastList(c.Jid));
          db.UpdatedConversationSubject.OnNext(new ConvoAndMessage()
          {
            LastMessage = (Message) null,
            Conversation = c
          });
        }));
      }));
    }

    public void PlayInAppNotificationSound(string jid)
    {
      string soundFile = "Sounds\\incoming.wav";
      if (Voip.IsInCall)
      {
        soundFile = "Sounds\\incomingshort.wav";
        Voip.Worker.Enqueue((Action) (() =>
        {
          CallInfoStruct? callInfo = Voip.Instance.GetCallInfo();
          if (!callInfo.HasValue || callInfo.Value.CallState == CallState.None || callInfo.Value.VideoEnabled)
            return;
          Voip.Instance.PlayAlert(AppState.AppInstallDir + "\\" + soundFile);
        }));
      }
      else
        new SimpleSound(soundFile, 0.5f).Play();
    }

    public void Reset()
    {
      GdprReport.DeleteReport(true, "app reset post deleting account");
      Settings.Reset();
      this.ConnectionResetSubject.OnNext(new Unit());
      try
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          nativeMediaStorage.DeleteFile(NativeInterfaces.Misc.GetString(10));
      }
      catch (Exception ex)
      {
      }
      (this.Connection ?? new FunXMPP.Connection()).Encryption.Reset();
      MessagesContext.Delete();
      ContactsContext.Delete();
      CallLog.DeleteDb();
      SqliteHsm.DeleteDb();
      SqlitePayments.DeleteDb();
      SqliteEmojiSearch.DeleteDb();
      AppState.ProcessResetActions();
      try
      {
        Backup.DeleteAll();
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "Delete backup");
      }
      this.ConnectDatabase();
      System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => NavUtils.ClearBackStack()));
    }

    public void CloseDb()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        MessagesContext.Reset();
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          ContactsContext.Reset();
          Axolotl.Close();
        }));
      }));
    }

    private void ConnectNetwork()
    {
      FieldStatsRunner.LoginRetryCount = 0;
      this.ConnectionSubscription.SafeDispose();
      this.ConnectionSubscription = FunRunner.ConnectAndRead(this.Connection, (IObservable<Unit>) this.ConnectionResetSubject, (Func<IObservable<Unit>, IObservable<Unit>>) (obs => obs.RepeatInForeground<Unit>()));
    }

    private void OnStartupStateChanged()
    {
      if (!this.DatabaseConnected || !App.Active)
        return;
      App.ApplicationActivatedSubject.OnNext(new Unit());
      if (OneDriveRestoreManager.OnAppActivated())
        return;
      OneDriveBackupManager.OnAppActivated();
    }

    private void Activate(bool preserved)
    {
      FieldStats.MaybeSetFgAppLaunchTime();
      if (App.Active & preserved)
        return;
      Log.l("app", "activating");
      Utils.ClearTimeZoneCache();
      this.minimalStartup.WaitOne();
      App.Active = true;
      App.LastLoginTime = new DateTime?();
      ContactsPage.NaggedOnExit = false;
      BackgroundAgentHelper.KillAgent();
      FunRunner.FailedLoginRetryTimerEnabled = true;
      this.OnStartupStateChanged();
      NetworkStateMonitor.Instance.Observe();
    }

    private void OnForegrounded()
    {
      this.InitializePush();
      Settings.ClearCachedSysFontSize();
      WAThreadPool.RunAfterDelay(TimeSpan.FromMilliseconds(5000.0), (Action) (() => WaScheduledTask.ProcessPendingTasks(true)));
      Action action = TimeSpentManager.GetInstance().AppForegrounded();
      if (action == null)
        return;
      App.OnDeactivated.Add(action);
    }

    private void Application_Activated(object sender, ActivatedEventArgs e)
    {
      Log.l("app", "application activated");
      this.Activate(e.IsApplicationInstancePreserved);
    }

    public void Deactivate()
    {
      if (!App.Active)
        return;
      App.Active = false;
      Log.l("app", "deactivating");
      ContactStore.OnAppLeaving();
      OneDriveRestoreManager.OnAppDeactivated();
      OneDriveBackupManager.OnAppDeactivated();
      FunRunner.CloseSocket();
      App.OnDeactivated.ForEach((Action<Action>) (a => a()));
      BackgroundAgentHelper.OnAppLeaving();
    }

    private void Application_Deactivated(object sender, DeactivatedEventArgs e)
    {
      Log.l("app", "deactivated");
      this.Deactivate();
    }

    private void Application_Closing(object sender, ClosingEventArgs e)
    {
      Log.l("app", "closing");
      this.Deactivate();
      Log.SuppressCrashLog = true;
      App.ApplicationActivatedSubject.Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => Log.SuppressCrashLog = false));
    }

    private void Application_UnhandledException(
      object sender,
      ApplicationUnhandledExceptionEventArgs e)
    {
      App.UnhandledExceptionSubject.OnNext(e);
      if (e.Handled)
        return;
      e.Handled = true;
      try
      {
        Log.SendCrashLog(e.ExceptionObject, "UnhandledException");
      }
      catch (Exception ex)
      {
      }
      if (!Debugger.IsAttached)
        return;
      Debugger.Break();
    }

    private void InitializePhoneApplication()
    {
      if (this.phoneApplicationInitialized)
        return;
      TransitionFrame transitionFrame1 = new TransitionFrame();
      transitionFrame1.CacheMode = (CacheMode) new BitmapCache();
      TransitionFrame transitionFrame2 = transitionFrame1;
      transitionFrame2.Navigated += new NavigatedEventHandler(this.CompleteInitializePhoneApplication);
      transitionFrame2.NavigationFailed += new NavigationFailedEventHandler(this.RootFrame_NavigationFailed);
      transitionFrame2.Obscured += new EventHandler<ObscuredEventArgs>(this.RootFrame_Obscured);
      transitionFrame2.Unobscured += new EventHandler(this.RootFrame_Unobscured);
      this.OriginalRootFrameTransform = transitionFrame2.RenderTransform;
      this.RootFrame = transitionFrame2;
      this.phoneApplicationInitialized = true;
    }

    private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
    {
      if (this.RootVisual != this.RootFrame)
        this.RootVisual = (UIElement) this.RootFrame;
      this.RootFrame.Navigated -= new NavigatedEventHandler(this.CompleteInitializePhoneApplication);
    }

    public static bool ShouldAnimate(bool isComplicatedAnimation)
    {
      return !AppState.IsLowMemoryDevice && (!isComplicatedAnimation || AppState.IsDecentMemoryDevice);
    }

    public static Stream OpenFromXAP(string path) => AppState.OpenFromXAP(path);

    public static void SummarizeRestore()
    {
      long restored = 0;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => restored = db.GetMessagesCount()));
      int num;
      System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => num = (int) MessageBox.Show(Plurals.Instance.GetString(AppResources.RestoredPlural, (int) restored))));
    }

    private void DetectVersionChange()
    {
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile("version", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
          {
            string str = (string) null;
            if (storageFileStream.Length != 0L)
            {
              MemoryStream destination = new MemoryStream();
              storageFileStream.CopyTo((Stream) destination);
              byte[] array = destination.ToArray();
              str = Encoding.UTF8.GetString(array, 0, array.Length);
              storageFileStream.Position = 0L;
            }
            string appVersion = AppState.GetAppVersion();
            if (!(str != appVersion))
              return;
            if (str != null)
              this.OnVersionChange();
            byte[] bytes = Encoding.UTF8.GetBytes(appVersion);
            storageFileStream.SetLength((long) bytes.Length);
            storageFileStream.Write(bytes, 0, bytes.Length);
          }
        }
      }
      catch (Exception ex)
      {
        Log.l(ex, "detect version change");
      }
    }

    private void OnVersionChange()
    {
      Log.l("app", "upgraded to version {0}", (object) AppState.GetAppVersion());
      Settings.IsUpdateAvailable = false;
      Settings.LastCheckForUpdatesUtc = new DateTime?(DateTime.UtcNow);
      Stats.OnUpgrade();
      Settings.ForceServerPropsReload = true;
      Settings.Delete(Settings.Key.LastWarningTime);
      AppState.SchedulePersistentAction(PersistentAction.SendVerifyAxolotlDigest());
      AppState.SchedulePersistentAction(PersistentAction.CapabilitiesRefresh());
      AppState.SchedulePersistentAction(PersistentAction.ProtocolBufferMessageUpgrade());
      string myJid = Settings.MyJid;
      if (myJid != null && ChatPictureStore.GetStoredPicturePath(myJid, true) == null)
        Settings.ShowPushNameScreen = true;
      if (!Settings.E2EVerificationCleanup)
        AppState.SchedulePersistentAction(PersistentAction.DisplayFullEncryptionToAllChats());
      DateTime? nullable = Settings.BizChat2TierOneTimeSysMsgAdded;
      if (!nullable.HasValue)
        AppState.SchedulePersistentAction(PersistentAction.AddOneTime2TierSysMsgToBizChats());
      nullable = new DateTime?();
      Settings.ServerExpirationOverride = nullable;
      FunRunner.ClearSkewValues(nameof (OnVersionChange));
      Settings.Mms4CurrentRoute = (string) null;
      Settings.Mms4QueryBackoffUntilTimeUtcTicks = 0L;
      App.LogExpiryRelatedVariables(nameof (OnVersionChange));
      nullable = new DateTime?();
      Settings.LastDeprecationNagTime = nullable;
    }

    public static bool ShouldPromptExpirationWarning()
    {
      double num1 = AppState.DaysUntilExpiration();
      Log.l("app", "days till expiration:{0}", (object) num1);
      bool flag;
      if (num1 > 30.0)
        flag = false;
      else if (num1 < (double) Settings.LastWarningTime)
      {
        bool isUpdateAvailable = Settings.IsUpdateAvailable;
        Log.l("app", "update available:{0},last nag range:{1}", (object) isUpdateAvailable, (object) Settings.LastWarningTime);
        if (isUpdateAvailable)
        {
          flag = true;
          int num2 = num1 < 15.0 ? (num1 < 8.0 ? (num1 < 6.0 ? (num1 < 4.0 ? (num1 != 3.0 ? (num1 != 2.0 ? 1 : 2) : 3) : 4) : 6) : 8) : 15;
          Settings.LastWarningTime = num2;
          Log.l("app", "new update nag range floor:{0}", (object) num2);
        }
        else
        {
          flag = false;
          App.CheckForUpdate();
        }
      }
      else
        flag = false;
      return flag;
    }

    public static void LogExpiryRelatedVariables(string context)
    {
      Log.l("Expiry", "Days til Expiry: {0}, ServerOverride: {1}, Funtime {2},\n expiry days {3}, Jitter {4},\n GetTimeSinceBuild(null): {5}, GetTimeSinceBuild(UtcNow): {6}, GetBuildTime universal: {7}\n skewed: {8}, build valid: {9}, update avail: {10}, expired: {11}, upgrade reqd: {12}\n current skew {13}, Local Skew {14}\n context: {15}, Utc: {16}", (object) AppState.DaysUntilExpiration(), (object) Settings.ServerExpirationOverride, (object) FunRunner.CurrentServerTimeUtc, (object) 45, (object) Settings.Jitter, (object) AppState.GetTimeSinceBuild(), (object) AppState.GetTimeSinceBuild(new DateTime?(DateTime.UtcNow)), (object) AppState.GetBuildTime().ToUniversalTime(), (object) AppState.IsPhoneTimeBadlySkewed(), (object) AppState.IsPhoneTimeValidForBuild(), (object) Settings.IsUpdateAvailable, (object) AppState.IsExpired, (object) AppState.AppUpgradeRequired, (object) FunRunner.CurrentTimeSkew, (object) Settings.LastLocalServerTimeDiff, (object) context, (object) DateTime.UtcNow);
    }

    private static void GetInstalledPackages(
      out App.WhatsAppInstallTypes currentPackage,
      out App.WhatsAppInstallTypes installedPackages)
    {
      currentPackage = App.WhatsAppInstallTypes.Production;
      installedPackages = App.WhatsAppInstallTypes.None;
      Guid appId = Windows.ApplicationModel.Store.CurrentApp.AppId;
      if (appId == App.BetaGuid)
        currentPackage = App.WhatsAppInstallTypes.Beta;
      else if (appId == App.AlphaGuid)
        currentPackage = App.WhatsAppInstallTypes.Alpha;
      foreach (Package package in InstallationManager.FindPackagesForCurrentPublisher())
      {
        Guid guid = Guid.Parse(package.Id.ProductId);
        if (guid == App.ProductionGuid)
          installedPackages |= App.WhatsAppInstallTypes.Production;
        else if (guid == App.BetaGuid)
          installedPackages |= App.WhatsAppInstallTypes.Beta;
        else if (guid == App.AlphaGuid)
          installedPackages |= App.WhatsAppInstallTypes.Alpha;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/App.xaml", UriKind.Relative));
    }

    private class Instance : AppState.Client
    {
      private static RefCountAction lockScreen = new RefCountAction((Action) (() => PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled), (Action) (() =>
      {
        try
        {
          PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Enabled;
        }
        catch (Exception ex)
        {
        }
      }));
      private static LinkedList<Action> leavingFgActions = (LinkedList<Action>) null;
      private static object leavingFgLock = new object();

      public FunXMPP.Connection GetConnection() => App.CurrentApp.Connection;

      public void OnDatabaseCorruption()
      {
        System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          if (App.CurrentApp.CurrentPage is HistoryRepair)
            return;
          Uri pageUri = UriUtils.CreatePageUri("HistoryRepair", "ClearStack=true");
          App currentApp = App.CurrentApp;
          while (currentApp.RootFrame.BackStack.Any<JournalEntry>())
            currentApp.RootFrame.RemoveBackEntry();
          currentApp.RootFrame.Navigate(pageUri);
        }));
      }

      public void OnLoginException(FunXMPP.LoginFailureException ex)
      {
        System.Windows.Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          Uri pageUri;
          switch (ex.Type)
          {
            case WAProtocol.LoginFailedReason.TempBanned:
              pageUri = UriUtils.CreatePageUri("LoginBanned", "ClearStack=true");
              Settings.LoginFailedExpirationTotalSeconds = ex.BanTotalSeconds;
              Settings.LoginFailedExpirationUtc = ex.BanExpirationUtc;
              Settings.LoginFailedRetryUtc = ex.RetryUtc;
              Settings.LoginFailedReason = ex.BanReason ?? "banned";
              Settings.LoginFailedReasonCode = ex.FailedLoginReason;
              Settings.LoginFailed = true;
              break;
            case WAProtocol.LoginFailedReason.ClientTooOld:
            case WAProtocol.LoginFailedReason.BadUserAgent:
              WaUriParams uriParams = new WaUriParams();
              uriParams.AddBool("ClearStack", true);
              pageUri = UriUtils.CreatePageUri("UpdateVersion", uriParams);
              AppState.AppUpgradeRequired = true;
              break;
            case WAProtocol.LoginFailedReason.ServerError:
              return;
            case WAProtocol.LoginFailedReason.ServerBackoffRequest:
              Settings.ServerRequestedFibBackoffState = Math.Max(Settings.ServerRequestedFibBackoffStateSaved, 1);
              Log.l("FunXMPP", "Server backoff requested: {0} {1}", (object) Settings.ServerRequestedFibBackoffState, (object) "LoginFailureException");
              return;
            default:
              pageUri = UriUtils.CreatePageUri("LoginInvalid", "ClearStack=true");
              Settings.DeleteMany((IEnumerable<Settings.Key>) new Settings.Key[3]
              {
                Settings.Key.LoginFailedReason,
                Settings.Key.LoginFailedRetryUtc,
                Settings.Key.LoginFailedExpirationUtc
              });
              Settings.LoginFailed = true;
              break;
          }
          App currentApp = App.CurrentApp;
          currentApp.ConnectionSubscription.SafeDispose();
          currentApp.ConnectionSubscription = (IDisposable) null;
          while (currentApp.RootFrame.BackStack.Any<JournalEntry>())
            currentApp.RootFrame.RemoveBackEntry();
          if (Settings.LoginFailed)
            currentApp.ConnectNetwork();
          currentApp.RootFrame.Navigate(pageUri);
        }));
      }

      public void Add(BackgroundTransferRequest req) => BackgroundTransferService.Add(req);

      public void ShowWebTask(Uri uri)
      {
        new WebBrowserTask() { Uri = uri }.Show();
      }

      public IDisposable LockScreenSubscription() => App.Instance.lockScreen.Subscribe();

      public void ResetConnection() => App.CurrentApp.ConnectionResetSubject.OnNext(new Unit());

      public void ShowToast(string str, string destUri) => InAppToast.ShowForString(str, destUri);

      public void ShowMessageBox(string str)
      {
        int num;
        System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => num = (int) MessageBox.Show(str)));
      }

      public void ShowErrorMessage(string errMsg, bool useMsgBox)
      {
        System.Windows.Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          if (useMsgBox)
          {
            int num = (int) MessageBox.Show(errMsg);
          }
          else
            InAppToast.ShowForError(errMsg);
        }));
      }

      public void PromptRateCall(string peerJid, byte[] fsCookie)
      {
        CallRatingPage.Start(peerJid, fsCookie, false, true);
      }

      public string GetBuildHash()
      {
        string buildHash = "";
        try
        {
          using (Stream file = App.OpenFromXAP(string.Format("{0}.dll", (object) AppState.GetAssemblyName(Assembly.GetExecutingAssembly()))))
            buildHash = BuildHash.Create(file).ToHexString().ToLower();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "build hash");
        }
        return buildHash;
      }

      public Subject<ApplicationUnhandledExceptionEventArgs> GetUnhandledExceptionSubject()
      {
        return App.UnhandledExceptionSubject;
      }

      private void ProcessLeavingActions()
      {
        Action[] actionArray = (Action[]) null;
        lock (App.Instance.leavingFgLock)
          actionArray = App.Instance.leavingFgActions.ToArray<Action>();
        foreach (Action action in actionArray)
          action();
      }

      public IDisposable PerformWhenLeavingFg(Action a)
      {
        bool cancelled = false;
        Dispatcher dispatcher = System.Windows.Deployment.Current.Dispatcher;
        LinkedListNode<Action> node = (LinkedListNode<Action>) null;
        Action a1 = (Action) (() =>
        {
          lock (App.Instance.leavingFgLock)
          {
            if (cancelled)
              return;
            if (App.Instance.leavingFgActions == null)
            {
              App.Instance.leavingFgActions = new LinkedList<Action>();
              App.OnDeactivated.Add(new Action(this.ProcessLeavingActions));
            }
            node = App.Instance.leavingFgActions.AddLast((Action) (() =>
            {
              if (cancelled)
                return;
              a();
            }));
          }
        });
        dispatcher.BeginInvokeIfNeeded(a1);
        return (IDisposable) new DisposableAction((Action) (() =>
        {
          lock (App.Instance.leavingFgLock)
          {
            cancelled = true;
            if (node == null)
              return;
            App.Instance.leavingFgActions.Remove(node);
          }
        }));
      }

      public void CheckPushName()
      {
      }

      public IObservable<bool> Decision(
        IObservable<bool> src,
        string prompt,
        string positive = null,
        string negative = null,
        string title = null)
      {
        return src.Decision(prompt, positive, negative, title);
      }

      public bool IsActive() => App.Active;

      public void OnUILanguageChanged()
      {
        OffOnConverter.On = AppResources.On;
        OffOnConverter.Off = AppResources.Off;
        App.CurrentApp.InitializeFlowDirection();
      }
    }

    [Flags]
    private enum WhatsAppInstallTypes
    {
      None = 0,
      Production = 1,
      Beta = 2,
      Alpha = 4,
    }
  }
}
