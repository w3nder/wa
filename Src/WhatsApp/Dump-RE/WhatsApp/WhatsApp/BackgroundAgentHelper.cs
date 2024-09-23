// Decompiled with JetBrains decompiler
// Type: WhatsApp.BackgroundAgentHelper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Windows.ApplicationModel.Background;

#nullable disable
namespace WhatsApp
{
  public class BackgroundAgentHelper
  {
    private static EventWaitHandle killEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Constants.KillEventName);
    private const string IncomingCallTaskName = "WhatsApp.IncomingCallAgent";
    private const string KeepAliveTaskName = "WhatsApp.KeepAliveAgent";
    public static bool BackgroundAgentDisabled = false;
    private static IDisposable workerThreadPauseSub = (IDisposable) null;
    private static object workerThreadLock = new object();

    private static ScheduledAction FindScheduledAction(
      string name,
      Action<AgentExitReason> onExitReason = null)
    {
      ScheduledAction scheduledAction = (ScheduledAction) null;
      try
      {
        scheduledAction = ScheduledActionService.Find(name);
      }
      catch (Exception ex)
      {
        BackgroundAgentHelper.LogVoipException(ex, "find scheduled action");
      }
      if (scheduledAction is ScheduledTask scheduledTask)
      {
        AgentExitReason lastExitReason = scheduledTask.LastExitReason;
        switch (lastExitReason)
        {
          case AgentExitReason.None:
          case AgentExitReason.Completed:
            if (onExitReason != null)
            {
              onExitReason(lastExitReason);
              break;
            }
            break;
          default:
            Log.WriteLineDebug(name + ": unexpected exit reason " + (object) lastExitReason);
            goto case AgentExitReason.None;
        }
      }
      return scheduledAction;
    }

    private static void LogVoipException(Exception ex, string descr) => Log.LogException(ex, descr);

    public static void DescheduleVoip(bool @lock = true)
    {
      Action a = (Action) (() =>
      {
        if (BackgroundAgentHelper.FindScheduledAction("WhatsApp.IncomingCallAgent") == null)
          return;
        try
        {
          ScheduledActionService.Remove("WhatsApp.IncomingCallAgent");
        }
        catch (Exception ex)
        {
          BackgroundAgentHelper.LogVoipException(ex, "remove [DescheduleVoip]");
        }
      });
      if (@lock)
        PushSystem.PushLock.PerformWithLock(a);
      else
        a();
    }

    private static void ScheduleVoipBegin(Action onComplete, Action<Exception> onError)
    {
      Action action = (Action) (() =>
      {
        Action snap = onComplete;
        onComplete = (Action) (() =>
        {
          try
          {
            snap();
          }
          catch (Exception ex)
          {
            onError(ex);
          }
        });
        Action<Exception> onError1 = (Action<Exception>) (ex => Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          onComplete();
          onError(ex);
        })));
        try
        {
          BackgroundExecutionManager.RequestAccessAsync().AsTask<BackgroundAccessStatus>().ToObservable<BackgroundAccessStatus>().ObserveOnDispatcher<BackgroundAccessStatus>().Subscribe<BackgroundAccessStatus>((Action<BackgroundAccessStatus>) (status => onComplete()), onError1);
        }
        catch (Exception ex)
        {
          onError1(ex);
        }
      });
      try
      {
        action();
      }
      catch (Exception ex)
      {
        onError(ex);
      }
    }

    public static void ScheduleVoip()
    {
      BackgroundAgentHelper.ScheduleVoipBegin((Action) (() => PushSystem.PushLock.PerformWithLock((Action) (() =>
      {
        BackgroundAgentHelper.TaskRegistrar[] taskRegistrarArray = new BackgroundAgentHelper.TaskRegistrar[3]
        {
          (BackgroundAgentHelper.TaskRegistrar) new BackgroundAgentHelper.VoipTaskRegistrar()
          {
            Name = "WhatsApp.IncomingCallAgent",
            Create = (Func<string, ScheduledAction>) (a =>
            {
              return (ScheduledAction) new VoipHttpIncomingCallTask(a, "message")
              {
                Description = AppResources.BackgroundAgentDescription
              };
            }),
            Critical = true
          },
          new BackgroundAgentHelper.TaskRegistrar()
          {
            Name = "WhatsApp.PeriodicBackupAgent",
            Create = (Func<string, ScheduledAction>) (a =>
            {
              return (ScheduledAction) new PeriodicTask(a)
              {
                Description = AppResources.BackgroundAgentDescription,
                ExpirationTime = DateTime.Now.AddDays(14.0)
              };
            })
          },
          new BackgroundAgentHelper.TaskRegistrar()
          {
            Name = "WhatsApp.ResourceIntensiveAgent",
            Create = (Func<string, ScheduledAction>) (a =>
            {
              return (ScheduledAction) new ResourceIntensiveTask(a)
              {
                Description = AppResources.BackgroundAgentDescription,
                ExpirationTime = DateTime.Now.AddDays(14.0)
              };
            })
          }
        };
        foreach (BackgroundAgentHelper.TaskRegistrar taskRegistrar in taskRegistrarArray)
        {
          if (taskRegistrar.NeedsReschedule())
            taskRegistrar.Schedule();
        }
      }))), (Action<Exception>) (ex => Log.SendCrashLog(ex, "register voip", logOnlyForRelease: true)));
    }

    private static void LockDb()
    {
      lock (BackgroundAgentHelper.workerThreadLock)
      {
        if (BackgroundAgentHelper.workerThreadPauseSub != null)
          return;
        BackgroundAgentHelper.workerThreadPauseSub = AppState.PauseWorkerThreads();
      }
    }

    private static void UnlockDb()
    {
      lock (BackgroundAgentHelper.workerThreadLock)
      {
        BackgroundAgentHelper.workerThreadPauseSub.SafeDispose();
        BackgroundAgentHelper.workerThreadPauseSub = (IDisposable) null;
      }
    }

    private static bool ActionTypeShouldScheduleAudio(PersistentAction.Types type)
    {
      return type != PersistentAction.Types.AutoDownload;
    }

    public static void OnAppLeaving()
    {
      bool shouldScheduleAudio = true;
      App currentApp = App.CurrentApp;
      bool flag1 = false;
      FieldStats.ClearFgLaunch();
      bool flag2 = SqliteRepair.IsRepairInProgress() || OneDriveRestoreManager.IsRestoreIncomplete || OneDriveBackupManager.IsBackupIncomplete;
      shouldScheduleAudio = shouldScheduleAudio && (Settings.SuccessfulLoginUtc.HasValue | flag2 || (flag1 = BackgroundAgent.GetTimeoutForRegState(Settings.PhoneNumberVerificationState, (Func<Func<long>, long>) null) > 0L));
      try
      {
        shouldScheduleAudio = shouldScheduleAudio && AudioUtils.BackgroundAudioAvailable;
      }
      catch (Exception ex)
      {
        shouldScheduleAudio = false;
      }
      if (shouldScheduleAudio && !flag1 && AppState.IsVoipScheduled() && !flag2)
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          int length = db.GetUnsentMessages().Length;
          if (length > 0)
          {
            Log.l("bg agent", "found {0} pending outgoing msgs to send", (object) length);
          }
          else
          {
            int num = ((IEnumerable<PersistentAction>) db.GetPersistentActions()).Where<PersistentAction>((Func<PersistentAction, bool>) (a => BackgroundAgentHelper.ActionTypeShouldScheduleAudio((PersistentAction.Types) a.ActionType))).Count<PersistentAction>();
            if (num > 0)
              Log.l("bg agent", "found {0} pending persist actions to process", (object) num);
            else if (db.GetWaScheduledTasksCount(excludeTypes: new WaScheduledTask.Types[1]
            {
              WaScheduledTask.Types.PurgeStatuses
            }, restriction: WaScheduledTask.Restrictions.BgOnly) > 0L)
            {
              Log.l("bg agent", "found {0} pending scheduled tasks to process", (object) num);
            }
            else
            {
              DateTime beforeUtc = FunRunner.CurrentServerTimeUtc - WaStatus.Expiration;
              if (((IEnumerable<Message>) db.GetMessagesBefore("status@broadcast", new int?(), beforeUtc, new int?(1), new int?())).Any<Message>())
                Log.l("bg agent", "found at least 1 status to purge");
              else
                shouldScheduleAudio = false;
            }
          }
        }));
      BackgroundAgentHelper.LockDb();
      currentApp.CloseDb();
      BackgroundAgentHelper.killEvent.Reset();
      WhatsApp.Voip.OnForegroundAppLeaving();
      Log.d("bg agent", "should start audio agent? {0}", (object) shouldScheduleAudio);
      if (!shouldScheduleAudio)
        return;
      try
      {
        AudioTrack audioTrack1 = (AudioTrack) null;
        if (NativeInterfaces.ScheduleWithTimeout<AudioTrack>((Func<AudioTrack>) (() =>
        {
          AudioTrack audioTrack2 = new AudioTrack();
          BackgroundAudioPlayer.Instance.Track = audioTrack2;
          BackgroundAudioPlayer.Instance.Play();
          return audioTrack2;
        }), (Action<Action>) (act => ThreadPool.QueueUserWorkItem((WaitCallback) (_ => act()))), new TimeSpan?(TimeSpan.FromSeconds(4.5)), out audioTrack1))
          return;
        Log.l("bg agent", "hit timeout trying to launch");
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Failed to start audio agent", false);
      }
    }

    public static void KillAgent(bool settingsReset = true)
    {
      BackgroundAgentHelper.killEvent.Set();
      try
      {
        Log.WriteLineDebug("Waiting for BG agent to die");
        AppState.BgMutex.WaitOne();
        AppState.BgMutex.ReleaseMutex();
        Log.WriteLineDebug("BG agent confirmed done.");
      }
      catch (Exception ex)
      {
      }
      if (settingsReset)
        Settings.Invalidate();
      BackgroundAgentHelper.UnlockDb();
    }

    private class TaskRegistrar
    {
      public string Name;
      public Func<string, ScheduledAction> Create;
      public bool Critical;

      public virtual bool NeedsReschedule()
      {
        ScheduledAction scheduledAction = BackgroundAgentHelper.FindScheduledAction(this.Name, new Action<AgentExitReason>(this.OnExitReason));
        if (scheduledAction != null && !scheduledAction.IsScheduled)
        {
          this.Remove();
          scheduledAction = (ScheduledAction) null;
        }
        return scheduledAction == null;
      }

      public void LogException(Exception ex, string message)
      {
        Log.LogException(ex, message);
        if (!(ex is InvalidOperationException) || !(ex.Message == "BNS Error: The action is disabled\r\n"))
          return;
        BackgroundAgentHelper.BackgroundAgentDisabled = true;
      }

      public virtual void OnExitReason(AgentExitReason reason)
      {
      }

      public void Remove()
      {
        Log.WriteLineDebug("Agent [{0}] has been de-scheduled", (object) this.Name);
        try
        {
          ScheduledActionService.Remove(this.Name);
        }
        catch (Exception ex)
        {
          this.LogException(ex, "remove agent");
        }
      }

      public virtual void Schedule()
      {
        Log.WriteLineDebug("Registering " + this.Name);
        ScheduledAction obj = this.Create(this.Name);
        Action action = (Action) (() =>
        {
          ScheduledActionService.Add(obj);
          if (!(this.Name == "WhatsApp.PeriodicBackupAgent"))
            return;
          BackgroundAgentHelper.BackgroundAgentDisabled = false;
        });
        if (this.Critical)
        {
          action();
        }
        else
        {
          try
          {
            action();
          }
          catch (Exception ex)
          {
            this.LogException(ex, "Agent " + this.Name + " failed to register");
          }
        }
      }
    }

    private class VoipTaskRegistrar : BackgroundAgentHelper.TaskRegistrar
    {
      public override bool NeedsReschedule()
      {
        bool flag = base.NeedsReschedule();
        if (!flag && Settings.PushUriCount != Settings.VoipPushUriCount)
        {
          Log.WriteLineDebug("Push URI has changed; forcing re-register");
          this.Remove();
          flag = true;
        }
        return flag;
      }

      public override void OnExitReason(AgentExitReason reason)
      {
        base.OnExitReason(reason);
        Settings.VoipExitReason = (int) reason;
      }

      public override void Schedule()
      {
        string[] strArray = new string[2]
        {
          "WhatsApp.IncomingCallAgent",
          "WhatsApp.KeepAliveAgent"
        };
        foreach (string name in strArray)
        {
          try
          {
            ScheduledActionService.Remove(name);
          }
          catch (Exception ex)
          {
          }
        }
        try
        {
          base.Schedule();
        }
        catch (Exception ex)
        {
          Settings.VoipExceptionMessage = ex.GetSynopsis();
          throw;
        }
        Settings.VoipPushUriCount = Settings.PushUriCount;
        Settings.VoipExceptionMessage = (string) null;
      }
    }
  }
}
