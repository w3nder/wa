// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioPlaybackManager
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp
{
  public class AudioPlaybackManager : WaDisposable
  {
    private IWaMediaPlayer nativePlayer;
    private Dictionary<string, double> trackPositions = new Dictionary<string, double>();
    private Message currentPlayingMsg;
    private DispatcherTimer timer;
    private const double TimerInterval = 450.0;
    private double durationProgress;
    private TimeSpan? seekPosAfterOpen;
    private static Dictionary<AudioPlaybackManager.PlaybackState, AudioPlaybackManager.PlaybackState[]> validPlaybackStateChanges = new Dictionary<AudioPlaybackManager.PlaybackState, AudioPlaybackManager.PlaybackState[]>()
    {
      {
        AudioPlaybackManager.PlaybackState.None,
        new AudioPlaybackManager.PlaybackState[2]
        {
          AudioPlaybackManager.PlaybackState.Playing,
          AudioPlaybackManager.PlaybackState.Stopped
        }
      },
      {
        AudioPlaybackManager.PlaybackState.Playing,
        new AudioPlaybackManager.PlaybackState[3]
        {
          AudioPlaybackManager.PlaybackState.Stalled,
          AudioPlaybackManager.PlaybackState.Paused,
          AudioPlaybackManager.PlaybackState.Stopped
        }
      },
      {
        AudioPlaybackManager.PlaybackState.Stalled,
        new AudioPlaybackManager.PlaybackState[3]
        {
          AudioPlaybackManager.PlaybackState.Playing,
          AudioPlaybackManager.PlaybackState.Paused,
          AudioPlaybackManager.PlaybackState.Stopped
        }
      },
      {
        AudioPlaybackManager.PlaybackState.Paused,
        new AudioPlaybackManager.PlaybackState[3]
        {
          AudioPlaybackManager.PlaybackState.Playing,
          AudioPlaybackManager.PlaybackState.Paused,
          AudioPlaybackManager.PlaybackState.Stopped
        }
      },
      {
        AudioPlaybackManager.PlaybackState.Stopped,
        new AudioPlaybackManager.PlaybackState[1]
        {
          AudioPlaybackManager.PlaybackState.Playing
        }
      }
    };
    private AudioPlaybackManager.PlaybackState audioPlaybackState;
    private WaAudioRouting.Endpoint audioOutput = WaAudioRouting.Endpoint.Undefined;
    private static bool bgMediaResumeAdded = false;
    private Subject<Message> playbackSubj = new Subject<Message>();

    private AudioPlaybackManager.PlaybackState AudioPlaybackState
    {
      get => this.audioPlaybackState;
      set
      {
        if (this.audioPlaybackState == value)
          return;
        if (!((IEnumerable<AudioPlaybackManager.PlaybackState>) AudioPlaybackManager.validPlaybackStateChanges[this.audioPlaybackState]).Contains<AudioPlaybackManager.PlaybackState>(value))
          throw new Exception("Invalid Playback state change - from " + (object) this.audioPlaybackState + " to " + (object) value);
        this.audioPlaybackState = value;
        this.NotifyPlaybackChanged(this.currentPlayingMsg);
      }
    }

    public bool IsPlaying => this.AudioPlaybackState == AudioPlaybackManager.PlaybackState.Playing;

    public bool IsPaused => this.AudioPlaybackState == AudioPlaybackManager.PlaybackState.Paused;

    public bool IsStalled => this.AudioPlaybackState == AudioPlaybackManager.PlaybackState.Stalled;

    public bool IsActive
    {
      get
      {
        return this.AudioPlaybackState != AudioPlaybackManager.PlaybackState.None && this.AudioPlaybackState != AudioPlaybackManager.PlaybackState.Stopped;
      }
    }

    public WaAudioRouting.Endpoint AudioOutput => this.audioOutput;

    public AudioPlaybackManager()
    {
      this.timer = new DispatcherTimer()
      {
        Interval = TimeSpan.FromMilliseconds(450.0)
      };
      this.timer.Tick += new EventHandler(this.Timer_Tick);
      if (!AudioPlaybackManager.bgMediaResumeAdded)
      {
        App.OnDeactivated.Add((Action) (() => AudioPlaybackManager.BackgroundMedia.Resume()));
        AudioPlaybackManager.bgMediaResumeAdded = true;
      }
      this.nativePlayer = (IWaMediaPlayer) new PttDispatchMediaPlayer();
      this.nativePlayer.MediaOpened += new EventHandler(this.AudioPlayer_MediaOpened);
      this.nativePlayer.MediaStarted += new EventHandler(this.AudioPlayer_MediaStarted);
      this.nativePlayer.MediaEnded += new EventHandler(this.AudioPlayer_MediaEnded);
      this.nativePlayer.MediaFailed += new EventHandler<ErrorEventArgs>(this.AudioPlayer_MediaFailed);
    }

    public IObservable<Message> GetPlaybackObservable() => (IObservable<Message>) this.playbackSubj;

    private void NotifyPlaybackChanged(Message m) => this.playbackSubj.OnNext(m);

    public void ResetTrackPositions() => this.trackPositions = new Dictionary<string, double>();

    public void Play(Message msg, WaAudioRouting.Endpoint targetOutput = WaAudioRouting.Endpoint.Undefined)
    {
      if (Voip.IsInCall)
      {
        Log.l("audio playback", "skip playback during voip call");
      }
      else
      {
        if (msg.MediaWaType != FunXMPP.FMessage.Type.Audio)
          return;
        WaAudioRouting.Endpoint currentAudioEndpoint = WaAudioRouting.GetCurrentAudioEndpoint();
        switch (currentAudioEndpoint)
        {
          case WaAudioRouting.Endpoint.Speaker:
          case WaAudioRouting.Endpoint.Earpiece:
            this.audioOutput = targetOutput != WaAudioRouting.Endpoint.Undefined ? targetOutput : (WaAudioRouting.ProximitySupported ? (WaAudioRouting.IsBluetoothAvailable() ? WaAudioRouting.Endpoint.Earpiece : WaAudioRouting.Endpoint.Speaker) : (WaAudioRouting.Endpoint) Settings.DefaultAudioEndpoint);
            break;
          default:
            this.audioOutput = currentAudioEndpoint;
            break;
        }
        try
        {
          this.PlayImpl(msg);
        }
        catch (Exception ex)
        {
          string context = string.Format("playback mgr: play audio message failed | file={0}", (object) msg.LocalFileUri);
          Log.LogException(ex, context);
          this.Stop(false, true);
          msg.PlaybackInProgress = false;
          msg.PlaybackValue = 0.0;
        }
      }
    }

    private void PlayImpl(Message msg)
    {
      bool flag = this.currentPlayingMsg != null && msg.MessageID == this.currentPlayingMsg.MessageID;
      if (!flag)
        this.Stop(true, true);
      this.seekPosAfterOpen = new TimeSpan?();
      double num1 = this.GetPos(msg.LocalFileUri);
      if (!flag)
      {
        if (num1 > 450.0)
          this.seekPosAfterOpen = new TimeSpan?(TimeSpan.FromMilliseconds(num1));
        else
          num1 = 0.0;
      }
      msg.PlaybackValue = this.durationProgress = num1;
      int num2 = (int) AudioPlaybackManager.BackgroundMedia.Stop();
      this.currentPlayingMsg = msg;
      if (flag)
      {
        this.nativePlayer.Play();
        this.timer.Start();
      }
      else
        this.nativePlayer.Play(msg.LocalFileUri);
      this.currentPlayingMsg.PlaybackInProgress = true;
      this.AudioPlaybackState = AudioPlaybackManager.PlaybackState.Playing;
      Log.d("audio playback", "{0}play | keyid:{1},duration:{2},start pos:{3}", flag ? (object) "resumed " : (object) "", this.currentPlayingMsg == null ? (object) "n/a" : (object) this.currentPlayingMsg.KeyId, (object) (this.currentPlayingMsg == null ? -1 : this.currentPlayingMsg.MediaDurationSeconds), (object) (int) num1);
    }

    public void Pause()
    {
      this.AudioPlaybackState = AudioPlaybackManager.PlaybackState.Paused;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (this.currentPlayingMsg == null)
          return;
        this.currentPlayingMsg.PlaybackInProgress = false;
        this.SetPos(this.currentPlayingMsg.LocalFileUri, this.durationProgress);
      }));
      this.nativePlayer.Pause();
      Log.l("audio playback", "paused");
      AudioPlaybackManager.BackgroundMedia.Resume();
    }

    public void UnStall()
    {
      Log.l("audio playback", "unstall");
      this.AudioPlaybackState = AudioPlaybackManager.PlaybackState.Playing;
      this.nativePlayer.Play();
    }

    public void Stall()
    {
      Log.l("audio playback", "stall");
      this.AudioPlaybackState = AudioPlaybackManager.PlaybackState.Stalled;
      this.nativePlayer.Pause();
    }

    public void Stop()
    {
      try
      {
        this.Stop(false);
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "Stopping audio", logOnlyForRelease: true);
      }
    }

    public void Stop(bool retainPosition, bool skipBackgroundMediaResume = false)
    {
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        if (!this.timer.IsEnabled)
          return;
        this.timer.Stop();
      }));
      this.nativePlayer.Stop();
      if (!skipBackgroundMediaResume)
        AudioPlaybackManager.BackgroundMedia.Resume();
      if (this.currentPlayingMsg != null)
      {
        if (retainPosition)
        {
          this.SetPos(this.currentPlayingMsg.LocalFileUri, this.durationProgress);
        }
        else
        {
          this.currentPlayingMsg.PlaybackValue = 0.0;
          this.SetPos(this.currentPlayingMsg.LocalFileUri, 0.0);
        }
        this.currentPlayingMsg.PlaybackInProgress = false;
      }
      this.AudioPlaybackState = AudioPlaybackManager.PlaybackState.Stopped;
      this.currentPlayingMsg = (Message) null;
      this.durationProgress = 0.0;
    }

    public void Detach() => this.nativePlayer.Detach();

    public void Seek(Message msg, double? durationPos)
    {
      if (durationPos.HasValue)
      {
        this.SetPos(msg.LocalFileUri, durationPos.Value);
        msg.PlaybackValue = durationPos.Value;
      }
      else
        this.SetPos(msg.LocalFileUri, 0.0);
    }

    public void RelativeSeek(double timeInMilliseconds)
    {
      if (!this.IsActive || this.currentPlayingMsg == null)
        return;
      double num = this.nativePlayer.Position.TotalMilliseconds + timeInMilliseconds;
      if (num < 450.0)
        num = 0.0;
      this.currentPlayingMsg.PlaybackValue = this.durationProgress = num;
      this.nativePlayer.Position = TimeSpan.FromMilliseconds(num);
    }

    public void SwitchAudioOutput(WaAudioRouting.Endpoint targetEndpoint)
    {
      if (!this.IsActive || this.currentPlayingMsg == null)
        return;
      WaAudioRouting.SetAudioEndpoint(this.audioOutput = targetEndpoint);
    }

    private double GetPos(string filepath)
    {
      double pos = 0.0;
      if (!this.trackPositions.TryGetValue(filepath, out pos))
        this.SetPos(filepath, pos);
      return pos;
    }

    private void SetPos(string filepath, double pos) => this.trackPositions[filepath] = pos;

    private void Timer_Tick(object sender, EventArgs e)
    {
      if (!this.IsPlaying)
        return;
      this.durationProgress += 450.0;
      this.currentPlayingMsg.PlaybackValue = this.durationProgress;
    }

    private void AudioPlayer_MediaStarted(object sender, EventArgs e)
    {
      if (this.AudioPlaybackState == AudioPlaybackManager.PlaybackState.Stalled)
      {
        this.nativePlayer.Pause();
      }
      else
      {
        if (this.audioOutput != WaAudioRouting.Endpoint.Speaker)
          return;
        WaAudioRouting.SetAudioEndpoint(WaAudioRouting.Endpoint.Speaker);
      }
    }

    private void AudioPlayer_MediaOpened(object sender, EventArgs e)
    {
      if (this.seekPosAfterOpen.HasValue)
        this.nativePlayer.Position = this.seekPosAfterOpen.Value;
      this.timer.Start();
    }

    private void AudioPlayer_MediaEnded(object sender, EventArgs e) => this.Stop(false);

    private void AudioPlayer_MediaFailed(object sender, ErrorEventArgs e)
    {
      string fileUrl = this.currentPlayingMsg == null ? "" : this.currentPlayingMsg.LocalFileUri;
      Log.LogException(e.ErrorException, string.Format("playback mgr: media failed event | file={0}", (object) fileUrl));
      this.Stop(false, true);
      uint[] numArray = new uint[3]
      {
        3222077440U,
        2684485632U,
        2684551168U
      };
      foreach (uint num1 in numArray)
      {
        Exception errorException = e.ErrorException;
        if ((errorException != null ? (((int) errorException.GetHResult() & -65536) == (int) num1 ? 1 : 0) : 0) != 0)
        {
          Log.LogException(e.ErrorException, "audio decoder");
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            FileRoot? root = MediaStorage.DetermineRoot(fileUrl);
            FileRoot fileRoot = FileRoot.SdCard;
            int num2 = (int) MessageBox.Show((root.GetValueOrDefault() == fileRoot ? (root.HasValue ? 1 : 0) : 0) != 0 ? AppResources.CouldNotOpenAudioOnSdCard : AppResources.CouldNotOpenAudio);
          }));
          return;
        }
      }
      if (e.ErrorException is VoipAudioPlayer.AudioDeviceException errorException1)
      {
        Log.SendCrashLog(errorException1.InnerException ?? (Exception) errorException1, "audio player", logOnlyForRelease: true);
        int num;
        Deployment.Current.Dispatcher.BeginInvoke((Action) (() => num = (int) MessageBox.Show(AppResources.CannotOpenAudioDevice)));
      }
      else
        e.ErrorException.GetRethrowAction()();
    }

    public static class BackgroundMedia
    {
      private static MediaState prevState;

      public static MediaState Stop()
      {
        FrameworkDispatcher.Update();
        try
        {
          AudioPlaybackManager.BackgroundMedia.prevState = MediaPlayer.State;
          if (!MediaPlayer.GameHasControl)
            MediaPlayer.Pause();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "Exception stopping BackgroundMedia", logOnlyForRelease: true);
          AudioPlaybackManager.BackgroundMedia.prevState = MediaState.Stopped;
        }
        return AudioPlaybackManager.BackgroundMedia.prevState;
      }

      public static void Resume(bool force = false)
      {
        FrameworkDispatcher.Update();
        try
        {
          if (MediaPlayer.State == MediaState.Playing)
            return;
          if (AudioPlaybackManager.BackgroundMedia.prevState == MediaState.Playing | force)
          {
            MediaPlayer.Resume();
            AudioPlaybackManager.BackgroundMedia.prevState = MediaState.Stopped;
          }
          else
          {
            if (AudioPlaybackManager.BackgroundMedia.prevState != MediaState.Paused)
              return;
            MediaPlayer.Pause();
          }
        }
        catch (Exception ex)
        {
          Log.l(nameof (BackgroundMedia), "Resume problem, prev state: {0}, force: {1}", (object) AudioPlaybackManager.BackgroundMedia.prevState, (object) force);
          Log.SendCrashLog(ex, "Exception finding state before resuming BackgroundMedia", logOnlyForRelease: true);
        }
      }
    }

    public enum PlaybackState
    {
      None,
      Playing,
      Stalled,
      Paused,
      Stopped,
    }
  }
}
