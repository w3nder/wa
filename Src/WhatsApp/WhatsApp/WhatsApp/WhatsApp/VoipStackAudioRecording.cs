// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipStackAudioRecording
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using WhatsAppNative;


namespace WhatsApp
{
  public abstract class VoipStackAudioRecording : AudioRecording, ISampleSink
  {
    private ISoundPort soundPort;
    private bool recording_;
    private long samplesCount_;
    private int bytesPerSample;
    private int sampleRate_;
    private StreamingUploadContext streamingContext;

    public override bool Start(StreamingUploadContext streamingContext = null)
    {
      if (this.recording_)
        this.StopRecording(true);
      this.streamingContext = streamingContext;
      return this.StartRecording();
    }

    public override void Stop(bool skipResultNotify = false)
    {
      this.StopRecording(skipResultNotify);
    }

    protected virtual void OnMetadata(int sampleRate, Channels channelCount, int bitsPerSample)
    {
    }

    protected abstract void OnSamples(byte[] b, StreamingUploadContext streamingContext);

    protected abstract void NotifyAudioResult(StreamingUploadContext streamingContext);

    public override bool IsReady => true;

    private bool StartRecording()
    {
      int num = (int) AudioPlaybackManager.BackgroundMedia.Stop();
      this.recording_ = true;
      AudioMetadata Metadata = new AudioMetadata()
      {
        Channels = 1,
        BitsPerSample = 16,
        SampleRate = DeviceSpecificSampleRates.Get().SampleRateFromDriver
      };
      Metadata.SamplesPerFrame = Metadata.SampleRate * 20 / 1000;
      this.sampleRate_ = Metadata.SampleRate;
      this.bytesPerSample = Metadata.BitsPerSample / 8 * Metadata.Channels;
      this.OnMetadata(Metadata.SampleRate, (Channels) Metadata.Channels, Metadata.BitsPerSample);
      this.samplesCount_ = 0L;
      this.Duration = 0;
      VoipAudioPlayer.InitInProcVoipStack();
      this.soundPort = (ISoundPort) NativeInterfaces.CreateInstance<SoundPort>();
      this.ApplyDeviceSpecificHacks(this.soundPort);
      try
      {
        this.soundPort.Initialize((ISoundSource) null, (ISampleSink) this, Metadata);
      }
      catch (UnauthorizedAccessException ex)
      {
        if (!AppState.IsWP10OrLater)
        {
          throw;
        }
        else
        {
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => UIUtils.MessageBox(AppResources.MicrophoneAccessTitle, AppResources.MicrophoneAccess, (IEnumerable<string>) new string[2]
          {
            AppResources.DismissButton,
            AppResources.Settings
          }, (Action<int>) (idx =>
          {
            if (idx != 1)
              return;
            NavUtils.NavigateExternal("ms-settings:privacy-microphone");
          }))));
          this.StopRecording(true);
          return false;
        }
      }
      this.soundPort.Start();
      return true;
    }

    private void ApplyDeviceSpecificHacks(ISoundPort soundPort)
    {
      Func<string, bool> f = (Func<string, bool>) null;
      if (DeviceProfile.Instance.Model == "RM-1045")
        f = (Func<string, bool>) (s => s.StartsWith("Surround Microphone", StringComparison.Ordinal));
      try
      {
        soundPort.EnumerateDevices(true, (IStringSelector) new VoipStackAudioRecording.ManagedStringSelector(f));
      }
      catch (Exception ex)
      {
      }
    }

    private void StopRecording(bool skipResultNotify)
    {
      this.recording_ = false;
      Marshal.ReleaseComObject((object) this.soundPort);
      if (!skipResultNotify)
        this.NotifyAudioResult(this.streamingContext);
      else if (this.streamingContext != null)
        this.streamingContext.Cancel();
      AudioPlaybackManager.BackgroundMedia.Resume();
    }

    public void OnSampleAvailable(IByteBuffer buf, long ts, long duration)
    {
      this.OnSamples(buf.Get(), this.streamingContext);
      this.samplesCount_ += (long) (buf.GetLength() / this.bytesPerSample);
      this.Duration = (int) (this.samplesCount_ / (long) this.sampleRate_);
    }

    private class ManagedStringSelector : IStringSelector
    {
      public Func<string, bool> func;

      public ManagedStringSelector(Func<string, bool> f) => this.func = f;

      public bool Select(string s)
      {
        Func<string, bool> func = this.func;
        return func != null && func(s);
      }
    }
  }
}
