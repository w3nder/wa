// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.PlayAudioMessage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Windows;


namespace WhatsApp.CommonOps
{
  public class PlayAudioMessage : WaDisposable
  {
    private static PlayAudioMessage instance_;
    protected AudioPlaybackManager player_ = new AudioPlaybackManager();
    private IDisposable playbackChangedSub_;
    private IDisposable audioRoutingSub_;
    private bool startNotified;

    public event EventHandler PlaybackStarted;

    protected void NotifyPlaybackStarted()
    {
      if (this.PlaybackStarted == null)
        return;
      this.PlaybackStarted((object) this, new EventArgs());
    }

    public event EventHandler PlaybackStopped;

    protected void NotifyPlaybackStopped()
    {
      if (this.PlaybackStopped == null)
        return;
      this.PlaybackStopped((object) this, new EventArgs());
    }

    private static PlayAudioMessage GetInstance(bool createIfNotExists, out bool created)
    {
      created = false;
      if (PlayAudioMessage.instance_ == null & createIfNotExists)
      {
        PlayAudioMessage.instance_ = !WaAudioRouting.ProximitySupported ? (PlayAudioMessage) new PlayAudioMessage7() : (PlayAudioMessage) new PlayAudioMessage8();
        created = true;
        PlayAudioMessage.instance_.Init();
      }
      return PlayAudioMessage.instance_;
    }

    public static PlayAudioMessage GetInstance(bool createIfExists)
    {
      bool created = false;
      return PlayAudioMessage.GetInstance(createIfExists, out created);
    }

    public static void DisposeInstance()
    {
      PlayAudioMessage.instance_.SafeDispose();
      PlayAudioMessage.instance_ = (PlayAudioMessage) null;
    }

    public AudioPlaybackManager Player => this.player_;

    protected PlayAudioMessage()
    {
    }

    protected override void DisposeManagedResources()
    {
      this.audioRoutingSub_.SafeDispose();
      this.audioRoutingSub_ = (IDisposable) null;
      this.player_.Stop();
      this.player_.Detach();
      IDisposable playbackChangedSub = this.playbackChangedSub_;
      this.playbackChangedSub_ = (IDisposable) null;
      if (playbackChangedSub != null)
        Deployment.Current.Dispatcher.BeginInvoke(new Action(playbackChangedSub.Dispose));
      base.DisposeManagedResources();
    }

    protected virtual void Init()
    {
      this.playbackChangedSub_ = this.player_.GetPlaybackObservable().ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnPlaybackChanged));
      this.player_.ResetTrackPositions();
    }

    private void OnPlaybackChanged(Message msg)
    {
      if (this.Player.IsPaused || this.Player.IsStalled || msg == null)
        return;
      if (msg.PlaybackInProgress && this.Player.IsPlaying && !this.startNotified)
      {
        this.startNotified = true;
        ReadReceipts.SendMessageReceipt(msg, FunXMPP.FMessage.Status.PlayedByTarget);
        this.OnPlaybackStarted();
        this.NotifyPlaybackStarted();
      }
      else
      {
        if (this.Player.IsActive || !this.startNotified)
          return;
        this.startNotified = false;
        this.OnPlaybackStopped();
        this.NotifyPlaybackStopped();
      }
    }

    protected virtual void OnPlaybackStarted()
    {
      if (this.audioRoutingSub_ == null)
        this.audioRoutingSub_ = WaAudioRouting.GetEndpointChangedObservable().ObserveOnDispatcher<WaAudioRouting.Endpoint>().Subscribe<WaAudioRouting.Endpoint>(new Action<WaAudioRouting.Endpoint>(this.OnAudioOutputChanged));
      UIUtils.EnableWakeLock(true);
    }

    protected virtual void OnPlaybackStopped()
    {
      this.audioRoutingSub_.SafeDispose();
      this.audioRoutingSub_ = (IDisposable) null;
      UIUtils.EnableWakeLock(false);
    }

    protected virtual void OnAudioOutputChanged(WaAudioRouting.Endpoint newEndpoint)
    {
      WaAudioRouting.Endpoint audioOutput = this.player_.AudioOutput;
      if (newEndpoint == audioOutput || audioOutput == WaAudioRouting.Endpoint.Speaker || audioOutput == WaAudioRouting.Endpoint.Earpiece || newEndpoint != WaAudioRouting.Endpoint.Speaker && newEndpoint != WaAudioRouting.Endpoint.Earpiece)
        return;
      this.player_.Pause();
    }
  }
}
