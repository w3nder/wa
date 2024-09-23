// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipAudioPlayer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class VoipAudioPlayer : IDisposable, IWaMediaPlayer
  {
    private static IVoip voipStack;
    private RefCountedObject<ISoundPort> native = new RefCountedObject<ISoundPort>((Action<ISoundPort>) (n => Marshal.ReleaseComObject((object) n)));
    private object @lock = new object();
    private Func<NativeStream, ISoundPort> setSource;

    public event EventHandler MediaStarted;

    public event EventHandler MediaOpened;

    public event EventHandler MediaEnded;

    public event EventHandler<ErrorEventArgs> MediaFailed;

    public VoipAudioPlayer(Func<Stream, ISoundSource> getSource)
    {
      VoipAudioPlayer voipAudioPlayer = this;
      this.setSource = (Func<NativeStream, ISoundPort>) (str => voipAudioPlayer.InitializeNative(getSource((Stream) str)));
    }

    public VoipAudioPlayer(IEnumerable<SoundPlaybackCodec> codecs)
    {
      VoipAudioPlayer voipAudioPlayer = this;
      this.setSource = (Func<NativeStream, ISoundPort>) (str => voipAudioPlayer.InitializeNative(NativeInterfaces.Misc.CreateSoundSource(codecs, str.GetNative())));
    }

    private ISoundPort InitializeNative(ISoundSource src)
    {
      ISoundPort soundPort = (ISoundPort) null;
      int millisecondsTimeout = 0;
      int num1 = 500;
      Exception innerException = (Exception) null;
      for (int index = 0; index < 4; ++index)
      {
        if (soundPort == null)
          soundPort = (ISoundPort) NativeInterfaces.CreateInstance<SoundPort>();
        try
        {
          soundPort.Initialize(src);
          return soundPort;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "audio device trouble");
          innerException = ex;
          Marshal.ReleaseComObject((object) soundPort);
          soundPort = (ISoundPort) null;
        }
        if (index != 3)
        {
          Thread.Sleep(millisecondsTimeout);
          int num2 = millisecondsTimeout + num1;
          millisecondsTimeout = num1;
          num1 = num2;
        }
      }
      throw new VoipAudioPlayer.AudioDeviceException("Failed to open audio device", innerException);
    }

    public static void InitInProcVoipStack()
    {
      if (AppState.IsBackgroundAgent)
        VoipAudioPlayer.voipStack = Voip.Instance;
      else
        Utils.LazyInit<IVoip>(ref VoipAudioPlayer.voipStack, (Func<IVoip>) (() => (IVoip) NativeInterfaces.CreateInstance<WhatsAppNative.Voip>()));
    }

    private void Init(Func<ISoundPort> setSource, Action dispose = null, Action<Action> dispatch = null)
    {
      if (dispatch == null)
        dispatch = (Action<Action>) (b => b());
      VoipAudioPlayer.InitInProcVoipStack();
      ISoundPort data = setSource();
      this.native.Set(data);
      Action a = (Action) (() => dispatch((Action) (() =>
      {
        EventHandler mediaEnded = this.MediaEnded;
        if (mediaEnded == null)
          return;
        mediaEnded((object) this, (EventArgs) new RoutedEventArgs());
      })));
      data.SetOnComplete(a.AsComAction());
      if (dispose != null)
        data.SetOnDispose(dispose.AsComAction());
      ManualResetEventSlim ev = new ManualResetEventSlim();
      dispatch((Action) (() =>
      {
        EventHandler mediaOpened = this.MediaOpened;
        if (mediaOpened != null)
          mediaOpened((object) this, (EventArgs) new RoutedEventArgs());
        ev.Set();
      }));
      using (ev)
        ev.WaitOne();
    }

    public void Dispose() => this.native.Set((ISoundPort) null);

    public void Detach() => this.Dispose();

    private void NativeAction(Action<ISoundPort> callback) => this.native.Get(callback);

    public void Play(string filePath, Action<Action> dispatch)
    {
      if (Deployment.Current.Dispatcher.CheckAccess())
      {
        AppState.Worker.Enqueue((Action) (() => this.Play(filePath, (Action<Action>) (a => Deployment.Current.Dispatcher.BeginInvoke(a)))));
      }
      else
      {
        if (dispatch == null)
          dispatch = (Action<Action>) (a => a());
        this.WrapFailedEvent((Action) (() =>
        {
          lock (this.@lock)
          {
            if (this.native != null)
              this.Dispose();
          }
          NativeStream stream = (NativeStream) null;
          filePath = MediaStorage.GetAbsolutePath(filePath);
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            stream = (NativeStream) nativeMediaStorage.OpenFile(filePath, FileMode.Open, FileAccess.Read);
          try
          {
            this.Init((Func<ISoundPort>) (() => this.setSource(stream)), new Action(((Stream) stream).Dispose), dispatch);
          }
          catch (Exception ex)
          {
            stream.Dispose();
            throw;
          }
          this.Start();
        }), dispatch);
      }
    }

    public void Play(string filePath) => this.Play(filePath, (Action<Action>) null);

    public void Play()
    {
      this.NativeAction((Action<ISoundPort>) (n =>
      {
        n.Resume();
        EventHandler mediaStarted = this.MediaStarted;
        if (mediaStarted == null)
          return;
        mediaStarted((object) this, new EventArgs());
      }));
    }

    private void Start()
    {
      this.NativeAction((Action<ISoundPort>) (n =>
      {
        n.Start();
        EventHandler mediaStarted = this.MediaStarted;
        if (mediaStarted == null)
          return;
        mediaStarted((object) this, new EventArgs());
      }));
    }

    private void WrapFailedEvent(Action a, Action<Action> dispatch = null)
    {
      if (dispatch == null)
        dispatch = (Action<Action>) (b => b());
      try
      {
        a();
      }
      catch (Exception ex)
      {
        dispatch((Action) (() =>
        {
          EventHandler<ErrorEventArgs> mediaFailed = this.MediaFailed;
          if (mediaFailed == null)
            return;
          mediaFailed((object) this, new ErrorEventArgs()
          {
            ErrorException = ex
          });
        }));
      }
    }

    public void Stop()
    {
      try
      {
        this.NativeAction((Action<ISoundPort>) (n => n.Stop()));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "audio player stop");
      }
    }

    public void Pause()
    {
      try
      {
        this.NativeAction((Action<ISoundPort>) (n => n.Pause()));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "audio player pause");
      }
    }

    public TimeSpan Position
    {
      get
      {
        long l = 0;
        try
        {
          this.NativeAction((Action<ISoundPort>) (n => l = n.GetPosition()));
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "audio player get position");
        }
        return TimeSpan.FromMilliseconds((double) l);
      }
      set => this.NativeAction((Action<ISoundPort>) (n => n.Seek((long) value.TotalMilliseconds)));
    }

    public Duration Duration
    {
      get
      {
        long l = 0;
        this.NativeAction((Action<ISoundPort>) (n => l = n.GetDuration()));
        return new Duration(TimeSpan.FromMilliseconds((double) l));
      }
    }

    public int Volume
    {
      get
      {
        int r = 100;
        this.NativeAction((Action<ISoundPort>) (n => r = n.GetVolume()));
        return r;
      }
      set => this.NativeAction((Action<ISoundPort>) (n => n.SetVolume(value)));
    }

    public class AudioDeviceException : Exception
    {
      public AudioDeviceException(string message, Exception innerException)
        : base(message, innerException)
      {
        if (innerException == null)
          return;
        this.HResult = innerException.HResult;
      }
    }
  }
}
