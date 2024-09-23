// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipAgent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using WhatsApp.Events;


namespace WhatsApp
{
  public class VoipAgent : ScheduledTaskAgent, IWaBackgroundAgent
  {
    private BackgroundAgent agent = new BackgroundAgent("Scheduled Agent");
    private static object backupRestoreLock = new object();
    private static bool backupManagerSubscribed = false;
    private static bool restoreManagerSubscribed = false;
    private static RefCountAction voipEventAction = new RefCountAction((Action) (() => AppState.VoipEvent.Set()), (Action) (() => AppState.VoipEvent.Reset()));

    public string LogHeader => this.agent.LogHeader;

    protected override void OnInvoke(ScheduledTask task)
    {
      try
      {
        IDisposable disposable1 = BackgroundAgent.RegisterOutOfProc();
        if (disposable1 != null)
          this.agent.AddDtor(new Action(disposable1.Dispose));
        AsyncAction disposeCompleteSub = new AsyncAction();
        RefCountAction sub = new RefCountAction((Action) null, (Action) (() =>
        {
          disposeCompleteSub.Perform();
          this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete));
        }));
        this.agent.AddDtor(new Action(sub.Subscribe().Dispose));
        Action die = (Action) (() =>
        {
          this.agent.dieBlockEvent.Set();
          this.agent.Die();
        });
        new WpAgentInvoked()
        {
          wpAgentType = (!(task is VoipHttpIncomingCallTask) ? (!(task is ResourceIntensiveTask) ? new wam_enum_wp_agent_type?(wam_enum_wp_agent_type.SCHEDULED_TASK) : new wam_enum_wp_agent_type?(wam_enum_wp_agent_type.RESOURCE_INTENSIVE_TASK)) : new wam_enum_wp_agent_type?(wam_enum_wp_agent_type.VOIP_PUSH))
        }.SaveEvent();
        if (task is VoipHttpIncomingCallTask incomingCallTask)
        {
          this.agent.SetName("Voip Push");
          Log.l(this.LogHeader, "Woken up for VOIP!");
          IDisposable disposable2 = VoipHandler.AddIncomingCallSub(sub);
          disposeCompleteSub.SetAction(new Action(disposable2.Dispose));
          using (MemoryStream memoryStream = new MemoryStream(incomingCallTask.MessageBody, false))
          {
            XDocument xdocument = XDocument.Load((Stream) memoryStream);
            XElement xelement1 = xdocument.Root.Element((XName) "fbips");
            if (xelement1 != null)
              FunRunner.SaveFallbackIp(xelement1.Value);
            FunRunner.Hosts = FunRunner.CreateHosts();
            XElement xelement2 = xdocument.Root.Element((XName) "from");
            XElement xelement3 = xdocument.Root.Element((XName) "id");
            if (xelement2 != null)
            {
              if (xelement3 != null)
              {
                try
                {
                  Stats.OnPush(xelement2.Value, xelement3.Value);
                }
                catch (Exception ex)
                {
                  Log.l(ex, "stats");
                }
              }
            }
          }
          IDisposable voipEventSub = VoipAgent.voipEventAction.Subscribe();
          this.agent.AddDtor((Action) (() => voipEventSub.Dispose()));
          TimeSpan voipAgentTimeout = Constants.VoipAgentTimeout;
          this.agent.Invoke(voipAgentTimeout, voipAgentTimeout, false, BackgroundAgent.BackgroundAgentType.VoipAgent);
        }
        else if (task.Name == "WhatsApp.PeriodicBackupAgent" || task.Name == "WhatsApp.ResourceIntensiveAgent")
        {
          DateTime now = DateTime.Now;
          long tickCount = (long) NativeInterfaces.Misc.GetTickCount();
          PhoneNumberVerificationState verificationState = PhoneNumberVerificationState.NewlyEntered;
          bool regTimerToastShown = false;
          bool wnsRegistered = false;
          Log.l(this.LogHeader, "Woken up for {0}", (object) task.Name);
          this.agent.SafeSettingsAccess<bool>((Func<bool>) (() =>
          {
            verificationState = Settings.PhoneNumberVerificationState;
            regTimerToastShown = Settings.RegTimerToastShown;
            wnsRegistered = Settings.WnsRegistered;
            return true;
          }));
          if (verificationState != PhoneNumberVerificationState.Verified && verificationState != PhoneNumberVerificationState.VerifiedPendingBackupCheck && verificationState != PhoneNumberVerificationState.VerifiedPendingHistoryRestore && !regTimerToastShown)
          {
            long timeoutForRegState = this.agent.GetTimeoutForRegState(verificationState);
            DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
            if (timeoutForRegState != 0L && timeoutForRegState < currentServerTimeUtc.Ticks)
              this.agent.ShowRegToast();
            die();
          }
          else if (AppState.UseWindowsNotificationService && !wnsRegistered && verificationState == PhoneNumberVerificationState.Verified)
          {
            Log.l(this.LogHeader, "WNS not registered!  Starting socket loop.");
            IDisposable voipEventSub = VoipAgent.voipEventAction.Subscribe();
            this.agent.AddDtor((Action) (() => voipEventSub.Dispose()));
            TimeSpan timeSpan = TimeSpan.FromSeconds(8.0);
            PushSystem.ForegroundInstance.UriObservable.Zip<Uri, Unit, Uri>(this.agent.GetConnection().ConnectedObservable().Take<Unit>(1), (Func<Uri, Unit, Uri>) ((left, right) => left)).Subscribe<Uri>((Action<Uri>) (pushUri => AppState.SendPushConfig(pushUri, (IEnumerable<FunXMPP.Connection.GroupSetting>) null)));
            this.agent.Invoke(timeSpan, timeSpan, false, BackgroundAgent.BackgroundAgentType.VoipAgent);
          }
          else if (now.Hour >= 4 && now.Hour < 5)
            new StaleMediaSweeper().Sweep((Action) (() => ThreadPool.QueueUserWorkItem((WaitCallback) (_ => die()))));
          else if (now.Hour >= 5 && now.Hour < 6)
          {
            FrequentChats.CalculateScores();
            Log.l(this.LogHeader, "Frequent chat scores calculated");
            die();
          }
          else if (OneDriveRestoreManager.IsRestoreIncomplete)
          {
            OneDriveRestoreManager oneDriveRestoreManager = OneDriveRestoreManager.Instance;
            lock (VoipAgent.backupRestoreLock)
            {
              Log.l(this.LogHeader, "Resuming cloud restore in Agent " + task.Name);
              bool flag = task.Name == "WhatsApp.ResourceIntensiveAgent";
              if (!VoipAgent.restoreManagerSubscribed)
              {
                IDisposable killEventSub = this.agent.KillOneDrive.Subscribe<Unit>((Action<Unit>) (_ =>
                {
                  Log.l("odm", "Stopping restore in agent " + task.Name);
                  try
                  {
                    oneDriveRestoreManager.Stop(OneDriveRestoreStopError.StoppedByOS);
                  }
                  catch (Exception ex)
                  {
                    Log.l(ex, "Exception stopping restore");
                  }
                }));
                this.agent.AddDtor((Action) (() => killEventSub.SafeDispose()));
                oneDriveRestoreManager.RestoreStopped += (EventHandler<BkupRestStoppedEventArgs>) ((s, e) =>
                {
                  killEventSub.SafeDispose();
                  ThreadPool.QueueUserWorkItem((WaitCallback) (_ => die()));
                });
                int num = flag ? 570 : 15;
                IDisposable timerSub = (IDisposable) null;
                timerSub = Observable.Interval(TimeSpan.FromSeconds((double) num)).ObserveOn<long>(WAThreadPool.Scheduler).SubscribeOn<long>(WAThreadPool.Scheduler).Subscribe<long>((Action<long>) (tick =>
                {
                  Log.l("odm", "Stopping restore to prevent it exceeding time for {0}", (object) task.Name);
                  oneDriveRestoreManager.Stop(OneDriveRestoreStopError.StoppedByTimeout);
                  timerSub.SafeDispose();
                  timerSub = (IDisposable) null;
                }));
                this.agent.AddDtor((Action) (() => timerSub.SafeDispose()));
                VoipAgent.restoreManagerSubscribed = true;
              }
              oneDriveRestoreManager.Start(flag ? OneDriveBkupRestTrigger.BackgroundAgent : OneDriveBkupRestTrigger.BackgroundAgentShort);
            }
          }
          else
          {
            lock (VoipAgent.backupRestoreLock)
            {
              if (VoipAgent.restoreManagerSubscribed)
              {
                Log.l(this.LogHeader, "Restore completed during agent execution. Cannot proceed with backup process.");
                die();
              }
              else
              {
                OneDriveBackupManager oneDriveBackupManager = OneDriveBackupManager.Instance;
                Log.l(this.LogHeader, "Resuming cloud backup in Agent " + task.Name);
                bool flag = task.Name == "WhatsApp.ResourceIntensiveAgent";
                if (!VoipAgent.backupManagerSubscribed)
                {
                  IDisposable killEventSub = this.agent.KillOneDrive.Subscribe<Unit>((Action<Unit>) (_ =>
                  {
                    Log.l("odm", "Stopping backup in agent " + task.Name);
                    try
                    {
                      oneDriveBackupManager.Stop(OneDriveBackupStopError.StoppedByOS);
                    }
                    catch (Exception ex)
                    {
                      Log.l(ex, "Exception stopping backup");
                    }
                  }));
                  this.agent.AddDtor((Action) (() => killEventSub.SafeDispose()));
                  oneDriveBackupManager.BackupStopped += (EventHandler<BkupRestStoppedEventArgs>) ((s, e) =>
                  {
                    if (e.Reason != OneDriveBkupRestStopReason.Stop && e.Reason != OneDriveBkupRestStopReason.Abort)
                    {
                      OneDriveBkupRestTrigger? nullable = e.StartTrigger;
                      OneDriveBkupRestTrigger driveBkupRestTrigger1 = OneDriveBkupRestTrigger.BackgroundAgentShort;
                      if ((nullable.GetValueOrDefault() == driveBkupRestTrigger1 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                      {
                        nullable = e.ChangedTrigger;
                        OneDriveBkupRestTrigger driveBkupRestTrigger2 = OneDriveBkupRestTrigger.BackgroundAgent;
                        if ((nullable.GetValueOrDefault() == driveBkupRestTrigger2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                        {
                          Log.l(this.LogHeader, "Restarting backup manager due to unhandled trigger change.");
                          oneDriveBackupManager.Start(OneDriveBkupRestTrigger.BackgroundAgent);
                          return;
                        }
                      }
                    }
                    killEventSub.SafeDispose();
                    ThreadPool.QueueUserWorkItem((WaitCallback) (_ => die()));
                  });
                  int num = flag ? 570 : 15;
                  IDisposable timerSub = (IDisposable) null;
                  timerSub = Observable.Interval(TimeSpan.FromSeconds((double) num)).ObserveOn<long>(WAThreadPool.Scheduler).SubscribeOn<long>(WAThreadPool.Scheduler).Subscribe<long>((Action<long>) (tick =>
                  {
                    Log.l("odm", "Terminating backup to prevent it exceeding time for {0}", (object) task.Name);
                    oneDriveBackupManager.Stop(OneDriveBackupStopError.StoppedByTimeout);
                    timerSub.SafeDispose();
                    timerSub = (IDisposable) null;
                  }));
                  this.agent.AddDtor((Action) (() => timerSub.SafeDispose()));
                  VoipAgent.backupManagerSubscribed = true;
                }
                oneDriveBackupManager.Start(flag ? OneDriveBkupRestTrigger.BackgroundAgent : OneDriveBkupRestTrigger.BackgroundAgentShort);
              }
            }
          }
        }
        else
          die();
      }
      catch (DatabaseInvalidatedException ex)
      {
        this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete));
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "voip agent");
        this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete));
      }
    }
  }
}
