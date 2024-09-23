// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackgroundAgent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.BackgroundTransfer;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using WhatsAppNative;


namespace WhatsApp
{
  public class BackgroundAgent : AppState.Client, IWaBackgroundAgent
  {
    private static FunXMPP.Connection conn;
    private static bool regCrashLog;
    private ManualResetEvent termEv = new ManualResetEvent(false);
    private ManualResetEvent dieEvent = new ManualResetEvent(false);
    private ManualResetEvent dieCompleteEvent = new ManualResetEvent(false);
    public ManualResetEvent dieBlockEvent = new ManualResetEvent(false);
    private bool offlineMarker;
    private static bool mutexLocked = false;
    private List<Action> dtors = new List<Action>();
    public static BackgroundAgent.BackgroundAgentType agentType = BackgroundAgent.BackgroundAgentType.None;
    private static IDisposable workerThreadPauseSub = (IDisposable) null;
    private IDisposable eventSubs;
    private static EventWaitHandle killEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Constants.KillEventName);
    private static IDisposable stackBlockSub = (IDisposable) null;
    private string agentName;
    private string logHeader = "bg agent";
    private static Subject<Unit> killEventSubject = new Subject<Unit>();
    private static AutoResetEvent killEventReset = new AutoResetEvent(false);
    private static AutoResetEvent killEventResetComplete = new AutoResetEvent(false);
    private static volatile bool killEventReentrantCheck = false;
    private static RefCountAction killEventThread = new RefCountAction((Action) (() => Observable.Return<Unit>(new Unit()).ObserveOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (_ =>
    {
      WaitHandle[] handles = new WaitHandle[2]
      {
        (WaitHandle) BackgroundAgent.killEvent,
        (WaitHandle) BackgroundAgent.killEventReset
      };
      Func<int> func = (Func<int>) (() => WaitHandle.WaitAny(handles));
      try
      {
        if (func() != 0)
          return;
        BackgroundAgent.killEventReentrantCheck = true;
        BackgroundAgent.killEventSubject.OnNext(new Unit());
      }
      finally
      {
        BackgroundAgent.killEventReentrantCheck = false;
        BackgroundAgent.killEventResetComplete.Set();
      }
    }))), (Action) (() =>
    {
      if (BackgroundAgent.killEventReentrantCheck)
        return;
      BackgroundAgent.killEventReset.Set();
      BackgroundAgent.killEventResetComplete.WaitOne();
    }));
    private bool pushNameSent;
    private static WorkQueue syncWorker;
    private static object workerLock = new object();
    private Random rnd = new Random();
    private Subject<ApplicationUnhandledExceptionEventArgs> UnhandledExceptionSubject = new Subject<ApplicationUnhandledExceptionEventArgs>();
    private bool LocPatchApplied;
    private static bool toastHandlerRegistered = false;

    private ISubject<Unit> connReset => (ISubject<Unit>) AppState.ConnectionResetSubject;

    public Subject<Unit> KillOneDrive { get; } = new Subject<Unit>();

    public string LogHeader => this.logHeader;

    public BackgroundAgent(string name = null, bool needKillEvent = true)
    {
      AppState.ClientInstance = (AppState.Client) this;
      AppState.IsBackgroundAgent = true;
      AppState.PingTimeout = Constants.BackgroundPingTimeout;
      NativeInterfaces.Misc.SetBackground();
      NativeInterfaces.Misc.SetCancelEvent(Constants.KillEventName);
      FieldStatsRunner.InitializeNative();
      NativeInterfaces.Misc.SetLogUserConnection(AppState.GetUserConnectionType());
      AppState.UpdateLogConnectionInfo();
      this.SetName(name);
      Log.Initialize();
      Log.l(this.LogHeader, "BackgroundAgent: ctor - " + (object) AppState.BatteryPercentage + "% Battery");
      Stats.LogMemoryUsage();
      IcuDataManager.InitializeIcu();
      Utils.ClearTimeZoneCache();
      Settings.AccessSafeFromBgAgent = (Func<bool>) (() => BackgroundAgent.mutexLocked);
      if (BackgroundAgent.stackBlockSub == null)
        BackgroundAgent.stackBlockSub = DebugStack.ApplyFilter((Func<Action, Action>) (a => (Action) (() =>
        {
          if (!Settings.AccessSafeFromBgAgent())
            return;
          a();
        })));
      if (!BackgroundAgent.regCrashLog)
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (BackgroundAgent.regCrashLog)
            return;
          Application.Current.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(this.Current_UnhandledException);
          BackgroundAgent.regCrashLog = true;
        }));
      if (BackgroundAgent.conn == null)
      {
        FunXMPP.Connection c = new FunXMPP.Connection();
        FunEventHandler funEventHandler = new FunEventHandler(c);
        c.EventHandler = (FunXMPP.Listener) funEventHandler;
        c.GroupEventHandler = (FunXMPP.GroupListener) funEventHandler;
        c.VoipEventHandler = (FunXMPP.VoipListener) VoipSignaling.Instance;
        BackgroundAgent.conn = c;
      }
      List<IDisposable> disposableList = new List<IDisposable>();
      disposableList.Add(BackgroundAgent.conn.LoginSubject.Subscribe<WAProtocol>((Action<WAProtocol>) (_ =>
      {
        try
        {
          AppState.ProcessSyncPendingNetworkTasks();
        }
        catch (DatabaseInvalidatedException ex)
        {
        }
        catch (Exception ex)
        {
          Log.l(ex, "unfinished tasks");
        }
        AppState.Worker.Enqueue((Action) (() =>
        {
          Log.l(this.LogHeader, "Connected");
          try
          {
            AppState.ProcessPendingNetworkTasks();
          }
          catch (DatabaseInvalidatedException ex)
          {
          }
          catch (Exception ex)
          {
            Log.l(ex, "unfinished tasks");
            throw;
          }
        }));
      })));
      if (needKillEvent)
        disposableList.Add(this.KillEventListen());
      this.eventSubs = (IDisposable) new CompositeDisposable(disposableList.ToArray());
    }

    public void SetName(string name)
    {
      if (!(this.agentName != name))
        return;
      string agentName = this.agentName;
      this.agentName = name;
      this.logHeader = name == null ? "bg agent" : string.Format("bg agent | {0}", (object) name);
      Log.l(this.LogHeader, "setting agent name | [{0}] -> [{1}]", (object) agentName, (object) name);
    }

    public IDisposable KillEventListen()
    {
      Observable.Return<Unit>(new Unit()).ObserveOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (_ =>
      {
        try
        {
          this.dieEvent.WaitOne();
          this.DieCore();
        }
        finally
        {
          this.dieCompleteEvent.Set();
        }
      }));
      return BackgroundAgent.KillEventObservable().Subscribe<Unit>((Action<Unit>) (_ =>
      {
        Log.l(this.LogHeader, "kill event signalled");
        this.Die();
      }));
    }

    public void PerformAndLog(string descr, Action a)
    {
      Log.l(this.LogHeader, "{0} begin", (object) descr);
      try
      {
        a();
      }
      catch (Exception ex)
      {
        string context = "exception performing " + descr;
        Log.l(ex, context);
        throw;
      }
      Log.l(this.LogHeader, "{0} end", (object) descr);
    }

    public static IObservable<Unit> KillEventObservable()
    {
      return Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
      {
        IDisposable disposable = BackgroundAgent.killEventSubject.Subscribe(observer);
        if (BackgroundAgent.killEvent.WaitOne(0))
          ThreadPool.QueueUserWorkItem((WaitCallback) (_ => observer.OnNext(new Unit())));
        else
          disposable = (IDisposable) new CompositeDisposable(new IDisposable[2]
          {
            disposable,
            BackgroundAgent.killEventThread.Subscribe()
          });
        return disposable;
      }));
    }

    public void CheckPushName()
    {
      if (this.pushNameSent)
        return;
      this.GetConnection().SendAvailableForChat(true);
      this.pushNameSent = true;
    }

    public static IDisposable RegisterOutOfProc() => Voip.RegisterOutOfProc();

    public static WorkQueue SyncWorker
    {
      get
      {
        return Utils.LazyInit<WorkQueue>(ref BackgroundAgent.syncWorker, (Func<WorkQueue>) (() => new WorkQueue(flags: WorkQueue.StartFlags.Unpausable)), BackgroundAgent.workerLock);
      }
    }

    private void AcquireMutex(bool releaseThreads = true)
    {
      WorkQueue syncWorker = BackgroundAgent.SyncWorker;
      string exceptionMessage = (string) null;
      Action action = (Action) (() =>
      {
        if (BackgroundAgent.killEvent.WaitOne(0) || BackgroundAgent.mutexLocked)
          exceptionMessage = "Lock acquisition not valid at this time";
        else if (!BackgroundAgent.mutexLocked)
        {
          if (!AppState.BgMutex.WaitOne(3000))
            exceptionMessage = "Mutex is already acquired";
          else if (BackgroundAgent.killEvent.WaitOne(0))
          {
            AppState.BgMutex.ReleaseMutex();
            exceptionMessage = "Kill event was signalled.";
          }
          else
          {
            BackgroundAgent.mutexLocked = true;
            Log.l(this.LogHeader, "Acquired BG mutex");
            if (releaseThreads)
              this.UnpauseThreads();
            AppState.OnSafeToTouchDatabase();
          }
        }
        else
        {
          if (!releaseThreads)
            return;
          this.UnpauseThreads();
        }
      });
      syncWorker.InvokeSynchronous(action);
      if (exceptionMessage == null)
        return;
      AppState.ThrowInvalidated("Told to exit while acquiring lock. {0}", (object) exceptionMessage);
    }

    private void PauseThreads()
    {
      if (BackgroundAgent.workerThreadPauseSub != null)
        return;
      BackgroundAgent.workerThreadPauseSub = AppState.PauseWorkerThreads(true);
    }

    private void UnpauseThreads()
    {
      if (BackgroundAgent.workerThreadPauseSub == null)
        return;
      Log.l(this.LogHeader, "Resuming previous threads");
      BackgroundAgent.workerThreadPauseSub.Dispose();
      BackgroundAgent.workerThreadPauseSub = (IDisposable) null;
    }

    private void ReleaseMutex()
    {
      BackgroundAgent.SyncWorker.InvokeSynchronous((Action) (() =>
      {
        try
        {
          if (!BackgroundAgent.mutexLocked)
            return;
          AppState.BgMutex.ReleaseMutex();
          BackgroundAgent.mutexLocked = false;
          Log.l(this.LogHeader, "Released BG mutex");
        }
        catch (Exception ex)
        {
          Log.l(ex, "bg mutex release");
        }
      }));
    }

    private bool ShouldReconnect()
    {
      this.pushNameSent = false;
      return !this.termEv.WaitOne(0) && !BackgroundAgent.killEvent.WaitOne(0);
    }

    public void Invoke(
      TimeSpan timeout,
      TimeSpan wifiTimeout,
      bool repeat,
      BackgroundAgent.BackgroundAgentType thisAgentType,
      Action onInit = null,
      Action onTimeout = null)
    {
      this.dieBlockEvent.Set();
      if (!this.termEv.WaitOne(0))
      {
        if (!BackgroundAgent.killEvent.WaitOne(0))
        {
          try
          {
            this.AcquireMutex();
            BackgroundAgent.agentType = thisAgentType;
            if (onInit != null)
              onInit();
            this.RegisterToastHandler();
            do
            {
              Log.l(this.LogHeader, "Connecting");
              bool timingOut = false;
              IDisposable disposable = FunRunner.ConnectAndRead(BackgroundAgent.conn, (IObservable<Unit>) this.connReset, (Func<IObservable<Unit>, IObservable<Unit>>) (obs => obs.RepeatWhile<Unit>((Func<bool>) (() => this.ShouldReconnect() && !timingOut))));
              bool flag = this.termEv.WaitOne(false ? wifiTimeout : timeout);
              if (flag)
              {
                if (this.offlineMarker)
                {
                  this.offlineMarker = flag = false;
                  this.termEv.Reset();
                }
                else
                  repeat = false;
              }
              if (!flag && onTimeout != null)
              {
                timingOut = true;
                this.dieBlockEvent.Reset();
                try
                {
                  FunRunner.CloseSocket();
                  try
                  {
                    disposable.Dispose();
                  }
                  catch (Exception ex)
                  {
                    Log.l(ex, "bg funrunner dispose");
                  }
                }
                finally
                {
                  this.dieBlockEvent.Set();
                }
                onTimeout();
                timingOut = false;
              }
            }
            while (repeat);
          }
          catch (DatabaseInvalidatedException ex)
          {
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "background agent");
          }
          this.Die();
          return;
        }
      }
      Log.l(this.LogHeader, "Invoke: got kill event, returning early");
    }

    public void AddDtor(Action a) => this.dtors.Add(a);

    public void Die()
    {
      this.dieEvent.Set();
      this.dieCompleteEvent.WaitOne();
    }

    private void DieCore()
    {
      this.termEv.Set();
      FunRunner.Invalidate();
      try
      {
        FunRunner.CloseSocket();
      }
      catch (Exception ex)
      {
        Log.l(ex, "close socket");
      }
      this.KillOneDrive.OnNext(new Unit());
      SqliteRepair.PauseRepair();
      this.dieBlockEvent.WaitOne();
      try
      {
        lock (MessagesContext.BgLock)
        {
          lock (ContactsContext.BgLock)
          {
            MessagesContext.Reset(true);
            ContactsContext.Reset(true);
            Axolotl.Close();
            AppState.InvalidateDatabases();
          }
        }
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
      BackgroundAgent.SyncWorker.InvokeSynchronous((Action) (() =>
      {
        this.PauseThreads();
        AppState.RevertInvalidateDatabases();
        FunRunner.RevertInvalidate();
        this.termEv.Reset();
        Settings.Invalidate();
      }));
      if (this.eventSubs != null)
      {
        this.eventSubs.Dispose();
        this.eventSubs = (IDisposable) null;
      }
      Log.l(this.LogHeader, "Leaving");
      PushSystem.Instance.OnAppReset();
      this.ReleaseMutex();
      int num = 4;
      while (this.LocksOutstanding() && num-- != 0)
        Thread.Sleep(500);
      try
      {
        foreach (Action dtor in this.dtors)
        {
          try
          {
            dtor();
          }
          catch (Exception ex)
          {
            Log.l(this.LogHeader, "Exception running dtor {0}", (object) dtor.ToString());
            Log.SendCrashLog(ex, "BackgroundAgent: Exception running dtor");
          }
        }
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "BackgroundAgent: Exception running dtors");
      }
      Log.l(this.LogHeader, "BackgroundAgent: dtor - " + (object) AppState.BatteryPercentage + "% Battery");
      Stats.LogMemoryUsage();
    }

    private bool LocksOutstanding()
    {
      int globalAcquireCount = MutexWithWatchdog.GlobalAcquireCount;
      bool flag = false;
      if (globalAcquireCount != 0)
      {
        Log.l(this.LogHeader, "Global acquire count was {0}, waiting...", (object) globalAcquireCount);
        flag = true;
      }
      return flag;
    }

    public void ProcessWaScheduledTasks()
    {
      WAThreadPool.QueueUserWorkItem((Action) (() => WaScheduledTask.ProcessPendingTasks(false)));
    }

    public void StartInfinite()
    {
      TimeSpan popTimeout = Constants.PopTimeout;
      TimeSpan wifiPopTimeout = Constants.WifiPopTimeout;
      bool activeUser = false;
      Action onInit = (Action) (() =>
      {
        int num1 = 0;
        int num2 = 0;
        string[] jids = (string[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => jids = db.GetAllConversationJids()));
        foreach (string str in jids)
        {
          if (str.IsGroupJid())
            ++num1;
          else
            ++num2;
        }
        activeUser = num1 >= 5 && num2 >= 5;
        this.ProcessWaScheduledTasks();
      });
      IDisposable onOfflineMarker = (IDisposable) null;
      Action onTimeout = (Action) (() =>
      {
        TimeSpan timeout = Constants.GetTimeBetweenPops(activeUser);
        DateTime now = DateTime.Now;
        if (now.Hour >= 1 && now.Hour < 7)
        {
          TimeSpan timeSpan = TimeSpan.FromHours((double) (7 - now.Hour)) - TimeSpan.FromMinutes((double) (now.Minute + this.rnd.Next(-30, 30))) - TimeSpan.FromSeconds((double) now.Second);
          if (timeSpan > timeout)
            timeout = timeSpan;
        }
        Log.l(this.LogHeader, "{0}: waiting for {1}", (object) now, (object) timeout);
        this.termEv.WaitOne(timeout);
        if (onOfflineMarker != null)
          return;
        onOfflineMarker = FunXMPP.OfflineMarkerSubject.Subscribe<Unit>((Action<Unit>) (_ =>
        {
          Log.l(this.LogHeader, "got offline marker, closing connection");
          this.offlineMarker = true;
          this.termEv.Set();
        }));
      });
      this.Invoke(popTimeout, wifiPopTimeout, true, BackgroundAgent.BackgroundAgentType.AudioAgent, onInit, onTimeout);
    }

    public void TryBackup(Action onComplete = null)
    {
      try
      {
        Log.l(this.LogHeader, "attempting automatic backup");
        if (Settings.CorruptDb)
          Log.l(this.LogHeader, "DB is marked as corrupt.  Bailing");
        else if (!Backup.CanBackup())
        {
          Log.l(this.LogHeader, "This device looks ineligible for backup.");
        }
        else
        {
          DateTime? lastBackupTime = Backup.GetLastBackupTime();
          if (lastBackupTime.HasValue && lastBackupTime.Value > DateTime.Now.AddDays(-1.0))
            Log.l(this.LogHeader, "Last backup at {0} was too recent", (object) lastBackupTime.Value);
          else if (BackgroundAgent.killEvent.WaitOne(0))
          {
            Log.l(this.LogHeader, "FG app is open, bailing...");
          }
          else
          {
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
              Log.l(this.LogHeader, "Backing up {0} messages, {1} chats...", (object) messagesCount, (object) conversationsCount);
              doBackup = true;
            }));
            if (doBackup)
            {
              try
              {
                Backup.Save(onComplete);
              }
              finally
              {
                onComplete = (Action) null;
              }
            }
            else
              Log.l(this.LogHeader, "Nothing to back up.");
            Log.l(this.LogHeader, "Backup complete.");
          }
        }
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "create backup");
      }
      finally
      {
        if (onComplete != null)
          onComplete();
      }
    }

    public T SafeSettingsAccess<T>(Func<T> func)
    {
      if (BackgroundAgent.mutexLocked)
        return func();
      this.AcquireMutex(false);
      try
      {
        return func();
      }
      finally
      {
        this.ReleaseMutex();
      }
    }

    public long GetTimeoutForRegState(PhoneNumberVerificationState verificationState)
    {
      try
      {
        return BackgroundAgent.GetTimeoutForRegState(verificationState, new Func<Func<long>, long>(this.SafeSettingsAccess<long>));
      }
      catch (DatabaseInvalidatedException ex)
      {
        return 0;
      }
    }

    public static long GetTimeoutForRegState(
      PhoneNumberVerificationState verificationState,
      Func<Func<long>, long> mapper)
    {
      if (mapper == null)
        mapper = (Func<Func<long>, long>) (value => value());
      switch (verificationState)
      {
        case PhoneNumberVerificationState.SameDeviceFailed:
          return mapper((Func<long>) (() => Settings.PhoneNumberVerificationRetryUtc.Ticks));
        case PhoneNumberVerificationState.ServerSentSms:
        case PhoneNumberVerificationState.ServerSendSmsFailed:
        case PhoneNumberVerificationState.ServerSentVoice:
        case PhoneNumberVerificationState.ServerSendVoiceFailed:
          return mapper((Func<long>) (() =>
          {
            long ticks = Settings.CodeEntryWaitToRetryUtc.Ticks;
            if (ticks <= 0L)
              ticks = Settings.PhoneNumberVerificationRetryUtc.Ticks;
            return ticks;
          }));
        case PhoneNumberVerificationState.VerifiedPendingBackupCheck:
        case PhoneNumberVerificationState.VerifiedPendingHistoryRestore:
        case PhoneNumberVerificationState.Verified:
          return 0;
        default:
          return -1;
      }
    }

    private void Current_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
    {
      this.UnhandledExceptionSubject.OnNext(e);
      e.Handled = true;
      try
      {
        Log.SendCrashLog(e.ExceptionObject, "Unhandled exception thrown from background agent", filter: false);
        this.termEv.Set();
      }
      catch (Exception ex)
      {
      }
    }

    private void RegisterToastHandler()
    {
      if (BackgroundAgent.toastHandlerRegistered)
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (!BackgroundAgent.toastHandlerRegistered)
        {
          db.NewMessagesSubject.SimpleObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnNewMessage));
          db.UpdatedMessageMediaWaTypeSubject.Where<Message>((Func<Message, bool>) (msg =>
          {
            if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked && !msg.KeyFromMe)
              return true;
            bool shouldUpdate = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db2 =>
            {
              if (db2.GetMessagesAfterCount(msg) >= 9)
                return;
              shouldUpdate = true;
            }));
            return shouldUpdate;
          })).SimpleObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnNewMessage));
        }
        BackgroundAgent.toastHandlerRegistered = true;
      }));
    }

    private void OnNewMessage(Message msg)
    {
      if (msg.Status == FunXMPP.FMessage.Status.Relay)
      {
        AppState.SendMessage(this.GetConnection(), msg);
      }
      else
      {
        string conversationId = (string) null;
        if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
        {
          Log.l(this.LogHeader, "revoke msg | msg_id={0} jid={1}", (object) msg.MessageID, (object) msg.KeyRemoteJid);
          if (AppState.UseWindowsNotificationService)
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
              if (conversation == null)
                return;
              conversationId = conversation.ConversationID.ToString();
            }));
            PushSystem.Instance.ClearToastHistoryMessage(msg.GetPushTag(), conversationId);
          }
        }
        if (GdprTos.ShouldBlockNotifications())
          return;
        if (msg.ShouldAlert())
        {
          if (!this.LocPatchApplied)
          {
            AppState.PatchLocale();
            this.LocPatchApplied = true;
          }
          string[] twoLineForMessage = NotificationString.GetTwoLineForMessage(msg, Settings.PreviewEnabled);
          if (twoLineForMessage != null)
          {
            msg.IsAlerted = true;
            bool muted = false;
            if (msg.ShouldAutoMute())
            {
              Log.l(this.LogHeader, "skip msg notify | auto muted | msg_id={0} jid={1}", (object) msg.MessageID, (object) msg.KeyRemoteJid);
              muted = true;
            }
            else if (JidHelper.IsGroupJid(msg.KeyRemoteJid) && !Settings.EnableGroupAlerts)
              muted = true;
            else if (JidHelper.IsUserJid(msg.KeyRemoteJid) && !Settings.EnableIndividualAlerts)
              muted = true;
            else
              Log.l(this.LogHeader, "notify msg | msg_id={0} jid={1}", (object) msg.MessageID, (object) msg.KeyRemoteJid);
            if (AppState.UseWindowsNotificationService || !muted)
            {
              string notificationSoundPath = CustomTones.GetNotificationSoundPath(msg.KeyRemoteJid);
              string uri = WaUris.ChatPageStr(WaUriParams.ForChatPage(msg.KeyRemoteJid, "vToast"));
              if (AppState.UseWindowsNotificationService && conversationId == null)
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
                  if (conversation == null)
                    return;
                  conversationId = conversation.ConversationID.ToString();
                }));
              PushSystem.Instance.ShellToastEx(twoLineForMessage, conversationId, uri, muted, notificationSoundPath, msg.GetPushTag());
            }
          }
        }
        if (msg.ShouldUpdateTile())
          this.UpdateTileForMessage(msg);
        else
          Log.d(this.LogHeader, "skip tile update | keyid:{0}", (object) msg.KeyId);
      }
    }

    private void UpdateTileForMessage(Message msg)
    {
      try
      {
        int inc = msg.IsQualifiedForUnread() ? 1 : 0;
        bool isChatMuted = msg.IsChatOrSenderMuted().First;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => TileHelper.IncrementTiles(db, TileDataSource.CreateForMessage(msg), inc, isChatMuted)));
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
    }

    public FunXMPP.Connection GetConnection() => BackgroundAgent.conn;

    public void ResetConnection() => this.connReset.OnNext(new Unit());

    public void OnLoginException(FunXMPP.LoginFailureException ex) => this.termEv.Set();

    public void OnDatabaseCorruption() => this.termEv.Set();

    public Subject<ApplicationUnhandledExceptionEventArgs> GetUnhandledExceptionSubject()
    {
      return this.UnhandledExceptionSubject;
    }

    public bool IsActive() => true;

    public void ShowToast(string str, string destUri)
    {
      Log.l(this.LogHeader, "show toast | str:{0},uri:{1}");
      PushSystem.Instance.ShellToast(str, destUri);
    }

    public void ShowRegToast()
    {
      Log.l(this.LogHeader, "showing reg toast");
      PushSystem.Instance.ShellToast(AppResources.RegTimeoutReached, WaUris.HomePageStr());
      try
      {
        this.SafeSettingsAccess<bool>((Func<bool>) (() => Settings.RegTimerToastShown = true));
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
    }

    public void Add(BackgroundTransferRequest req)
    {
    }

    public void ShowWebTask(Uri uri)
    {
    }

    public IDisposable LockScreenSubscription()
    {
      return (IDisposable) new DisposableAction((Action) (() => { }));
    }

    public void ShowMessageBox(string _)
    {
    }

    public void ShowErrorMessage(string _, bool __)
    {
    }

    public string GetBuildHash() => "";

    public IDisposable PerformWhenLeavingFg(Action a) => throw new NotSupportedException();

    public IObservable<bool> Decision(
      IObservable<bool> src,
      string prompt,
      string positive = null,
      string negative = null,
      string title = null)
    {
      return Observable.Never<bool>();
    }

    public void PromptRateCall(string peerJid, byte[] fsCookie)
    {
      FieldStatsRunner.FieldStatsAction((Action<IFieldStats>) (fs => fs.SubmitVoipNullRating(fsCookie)));
    }

    public void OnUILanguageChanged()
    {
    }

    public enum BackgroundAgentType
    {
      None,
      AudioAgent,
      VoipAgent,
      BackupAgent,
    }
  }
}
