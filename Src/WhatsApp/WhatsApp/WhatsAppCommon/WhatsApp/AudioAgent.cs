// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioAgent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Media;
using WhatsApp.Events;


namespace WhatsApp
{
  public class AudioAgent : AudioStreamingAgent, IWaBackgroundAgent
  {
    private BackgroundAgent agent = new BackgroundAgent("Audio Agent");
    private bool started;
    private AudioAgent.AudioAgentVoipHandler voipHandler;

    public string LogHeader => this.agent.LogHeader;

    public AudioAgent()
    {
      this.voipHandler = new AudioAgent.AudioAgentVoipHandler(this.agent);
      this.agent.GetConnection().VoipEventHandler = (FunXMPP.VoipListener) this.voipHandler;
    }

    protected override void OnBeginStreaming(AudioTrack track, AudioStreamer streamer)
    {
      streamer.SetSource((MediaStreamSource) new AudioAgent.StreamSource());
      if (this.started)
        return;
      this.agent.AddDtor((Action) (() => this.agent.PerformAndLog("NotifyComplete", new Action(((Microsoft.Phone.BackgroundAgent) this).NotifyComplete))));
      new WpAgentInvoked()
      {
        wpAgentType = new wam_enum_wp_agent_type?(wam_enum_wp_agent_type.AUDIO_AGENT)
      }.SaveEventSampled(20U);
      Observable.Return<Unit>(new Unit()).SubscribeOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (__ =>
      {
        Log.l(this.LogHeader, "Starting audio agent");
        PhoneNumberVerificationState? state = new PhoneNumberVerificationState?();
        try
        {
          state = new PhoneNumberVerificationState?(this.agent.SafeSettingsAccess<PhoneNumberVerificationState>((Func<PhoneNumberVerificationState>) (() => Settings.PhoneNumberVerificationState)));
        }
        catch (Exception ex)
        {
        }
        bool flag = AppState.IsVoipScheduled();
        long timeoutForRegState;
        if (state.HasValue && (timeoutForRegState = this.agent.GetTimeoutForRegState(state.Value)) != 0L)
        {
          Log.l(this.LogHeader, "We are not registered...");
          long ticks = FunRunner.CurrentServerTimeUtc.Ticks;
          if (Settings.RegTimerToastShown)
            Log.l(this.LogHeader, "Already sent reg a toast, exiting...");
          else if (timeoutForRegState == 0L)
            Log.l(this.LogHeader, "Timer not set, exiting...");
          else if (timeoutForRegState < ticks)
          {
            this.agent.ShowRegToast();
          }
          else
          {
            TimeSpan delay = TimeSpan.FromTicks(timeoutForRegState - ticks);
            Log.l(this.LogHeader, "Waiting for {0}m{1}s until reg toast", (object) (int) delay.TotalMinutes, (object) ((int) delay.TotalSeconds % 60));
            WAThreadPool.RunAfterDelay(delay, (Action) (() =>
            {
              Settings.Invalidate();
              PhoneNumberVerificationState? nullable5 = new PhoneNumberVerificationState?();
              PhoneNumberVerificationState? nullable6;
              try
              {
                nullable6 = new PhoneNumberVerificationState?(this.agent.SafeSettingsAccess<PhoneNumberVerificationState>((Func<PhoneNumberVerificationState>) (() => Settings.PhoneNumberVerificationState)));
              }
              catch (DatabaseInvalidatedException ex)
              {
                this.agent.Die();
                return;
              }
              PhoneNumberVerificationState? nullable7 = state;
              PhoneNumberVerificationState? nullable8 = nullable6;
              if ((nullable7.GetValueOrDefault() == nullable8.GetValueOrDefault() ? (nullable7.HasValue == nullable8.HasValue ? 1 : 0) : 0) != 0)
                this.agent.ShowRegToast();
              this.agent.Die();
            }));
            return;
          }
          this.NotifyComplete();
        }
        else if (AudioAgent.Catch<bool, DatabaseInvalidatedException>((Func<bool>) (() => this.agent.SafeSettingsAccess<bool>((Func<bool>) (() => Settings.CorruptDb && SqliteRepair.IsRepairStarted())))))
          this.agent.Invoke(Constants.TaskCompletionTimeout, Constants.TaskCompletionTimeout, false, BackgroundAgent.BackgroundAgentType.AudioAgent, (Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.Repair()))));
        else if (OneDriveRestoreManager.IsRestoreIncomplete)
        {
          Log.l(this.LogHeader, "Resuming cloud restore in audio agent...");
          Action<string> stopRestore = (Action<string>) (stoppinglocation =>
          {
            Log.l("odm", "stopping restore in audio agent - {0}", (object) stoppinglocation);
            try
            {
              OneDriveRestoreManager.Instance.Stop(OneDriveRestoreStopError.StoppedByOS);
            }
            catch (Exception ex)
            {
              string context = string.Format("Exception stopping restore in {0}", (object) stoppinglocation);
              Log.l(ex, context);
            }
          });
          IDisposable killEventSub = this.agent.KillOneDrive.Subscribe<Unit>((Action<Unit>) (_ => stopRestore("killEvent")));
          this.agent.AddDtor((Action) (() => killEventSub.SafeDispose()));
          OneDriveRestoreManager.Instance.RestoreStopped += (EventHandler<BkupRestStoppedEventArgs>) ((s, e) =>
          {
            killEventSub.SafeDispose();
            ThreadPool.QueueUserWorkItem((WaitCallback) (_ => this.agent.Die()));
          });
          this.agent.Invoke(Constants.TaskCompletionTimeout, Constants.TaskCompletionTimeout, false, BackgroundAgent.BackgroundAgentType.AudioAgent, (Action) (() => OneDriveRestoreManager.Instance.Start(OneDriveBkupRestTrigger.BackgroundAgent)), (Action) (() => stopRestore("timerCompletion")));
        }
        else if (OneDriveBackupManager.IsBackupIncomplete)
        {
          Log.l(this.LogHeader, "Resuming cloud backup in audio agent...");
          OneDriveBackupManager oneDriveBackupManager = OneDriveBackupManager.Instance;
          Action<string> stopBackup = (Action<string>) (stoppinglocation =>
          {
            Log.l("odm", "Stopping backup in audio agent - {0}", (object) stoppinglocation);
            try
            {
              oneDriveBackupManager.Stop(OneDriveBackupStopError.StoppedByOS);
            }
            catch (Exception ex)
            {
              string context = string.Format("Exception stopping backup in {0}", (object) stoppinglocation);
              Log.l(ex, context);
            }
          });
          IDisposable killEventSub = this.agent.KillOneDrive.Subscribe<Unit>((Action<Unit>) (_ => stopBackup("killEvent")));
          this.agent.AddDtor((Action) (() => killEventSub.SafeDispose()));
          oneDriveBackupManager.BackupStopped += (EventHandler<BkupRestStoppedEventArgs>) ((s, e) =>
          {
            killEventSub.SafeDispose();
            ThreadPool.QueueUserWorkItem((WaitCallback) (_ => this.agent.Die()));
          });
          this.agent.Invoke(Constants.TaskCompletionTimeout, Constants.TaskCompletionTimeout, false, BackgroundAgent.BackgroundAgentType.AudioAgent, (Action) (() => oneDriveBackupManager.Start(OneDriveBkupRestTrigger.BackgroundAgent)), (Action) (() => stopBackup("timerCompletion")));
        }
        else if (!flag)
        {
          FunRunner.BackoffMax = 120000;
          Log.l(this.LogHeader, "VOIP appears to be off.  Doing periodic offline pop.");
          this.agent.StartInfinite();
        }
        else
        {
          FunRunner.BackoffMax = 30000;
          Log.l(this.LogHeader, "VOIP is active.  We'll time out after a few minutes.");
          Observable.Return<Unit>(new Unit()).ObserveOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (_ =>
          {
            AppState.VoipEvent.WaitOne();
            Log.l(this.LogHeader, "We got a VOIP push.  Exiting...");
            this.agent.Die();
          }));
          this.agent.Invoke(Constants.TaskCompletionTimeout, Constants.TaskCompletionTimeout, false, BackgroundAgent.BackgroundAgentType.AudioAgent, new Action(this.agent.ProcessWaScheduledTasks));
        }
        this.started = false;
      }));
      this.started = true;
    }

    private static T Catch<T, Exn>(Func<T> fn) where Exn : Exception
    {
      T obj = default (T);
      try
      {
        obj = fn();
      }
      catch (Exn ex)
      {
      }
      return obj;
    }

    protected override void OnCancel()
    {
      Log.l(this.LogHeader, "got cancel event");
      this.agent.Die();
    }

    private class AudioAgentVoipHandler : FunXMPP.VoipListener
    {
      private BackgroundAgent agent;

      public AudioAgentVoipHandler(BackgroundAgent agent) => this.agent = agent;

      public void HandleVoipAck(FunXMPP.ProtocolTreeNode node)
      {
        Log.l(this.agent.LogHeader, "Ignored voip ack");
      }

      public void HandleVoipNode(FunXMPP.ProtocolTreeNode node)
      {
        ThreadPool.QueueUserWorkItem((WaitCallback) (_ => this.agent.Die()));
      }

      public void HandleVoipOfferReceipt(FunXMPP.ProtocolTreeNode node)
      {
        Log.l(this.agent.LogHeader, "Ignored voip offer receipt");
      }

      public void HandleEncRekeyRetry(FunXMPP.ProtocolTreeNode node)
      {
        Log.l(this.agent.LogHeader, "Ignored voip enc rekey retry receipt");
      }
    }

    public class StreamSource : MediaStreamSource
    {
      private MediaStreamDescription descriptor;
      private MemoryStream samples;
      private int delay = 1000;
      private long dt;

      protected override void SwitchMediaStreamAsync(MediaStreamDescription mediaStreamDescription)
      {
      }

      protected override void SeekAsync(long seekToTime) => this.ReportSeekCompleted(seekToTime);

      protected override void OpenMediaAsync()
      {
        Dictionary<MediaSourceAttributesKeys, string> mediaStreamAttributes1 = new Dictionary<MediaSourceAttributesKeys, string>();
        List<MediaStreamDescription> availableMediaStreams = new List<MediaStreamDescription>();
        Dictionary<MediaStreamAttributeKeys, string> mediaStreamAttributes2 = new Dictionary<MediaStreamAttributeKeys, string>();
        mediaStreamAttributes1.Add(MediaSourceAttributesKeys.CanSeek, "0");
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            binaryWriter.Write((ushort) 1);
            binaryWriter.Write((ushort) 1);
            binaryWriter.Write(8192U);
            binaryWriter.Write(8192U);
            binaryWriter.Write((ushort) 1);
            binaryWriter.Write((ushort) 8);
            binaryWriter.Write(0U);
            binaryWriter.Flush();
            mediaStreamAttributes2.Add(MediaStreamAttributeKeys.CodecPrivateData, output.ToArray().ToHexString());
          }
        }
        availableMediaStreams.Add(this.descriptor = new MediaStreamDescription(MediaStreamType.Audio, (IDictionary<MediaStreamAttributeKeys, string>) mediaStreamAttributes2));
        this.ReportOpenMediaCompleted((IDictionary<MediaSourceAttributesKeys, string>) mediaStreamAttributes1, (IEnumerable<MediaStreamDescription>) availableMediaStreams);
      }

      private Stream Samples
      {
        get
        {
          if (this.samples == null)
          {
            this.samples = new MemoryStream(8192);
            this.samples.SetLength((long) this.samples.Capacity);
          }
          this.samples.Seek(0L, SeekOrigin.Begin);
          return (Stream) this.samples;
        }
      }

      protected override void GetSampleAsync(MediaStreamType mediaStreamType)
      {
        Observable.Return<Unit>(new Unit()).SubscribeOn<Unit>((IScheduler) Scheduler.NewThread).Subscribe<Unit>((Action<Unit>) (_ =>
        {
          Thread.Sleep(this.delay);
          this.delay = Math.Min(this.delay * 2, 520000);
          MediaStreamSample mediaStreamSample = new MediaStreamSample(this.descriptor, this.Samples, 0L, this.Samples.Length, this.dt, (IDictionary<MediaSampleAttributeKeys, string>) new Dictionary<MediaSampleAttributeKeys, string>());
          ++this.dt;
          this.ReportGetSampleCompleted(mediaStreamSample);
        }));
      }

      protected override void GetDiagnosticAsync(MediaStreamSourceDiagnosticKind diagnosticKind)
      {
      }

      protected override void CloseMedia()
      {
      }
    }
  }
}
