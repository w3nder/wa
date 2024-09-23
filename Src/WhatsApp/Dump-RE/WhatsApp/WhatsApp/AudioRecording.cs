// Decompiled with JetBrains decompiler
// Type: WhatsApp.AudioRecording
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.IO;

#nullable disable
namespace WhatsApp
{
  public abstract class AudioRecording
  {
    private AudioRecordingScheduler scheduler;
    protected bool isStopRequested_;
    private int duration_;

    protected AudioRecording()
    {
      this.DurationChangedSubject = new Subject<Unit>();
      this.AudioResultSubject = new Subject<WaAudioArgs>();
      this.DeviceAsyncStoppedSubject = new Subject<Unit>();
    }

    public AudioRecordingScheduler Scheduler
    {
      get
      {
        return Utils.LazyInit<AudioRecordingScheduler>(ref this.scheduler, (Func<AudioRecordingScheduler>) (() => new AudioRecordingScheduler()));
      }
      protected set => this.scheduler = value;
    }

    public abstract string MimeType { get; }

    public abstract string FileExtension { get; }

    public virtual bool ShouldStream => false;

    public abstract bool IsReady { get; }

    public Subject<Unit> DurationChangedSubject { get; private set; }

    public Subject<WaAudioArgs> AudioResultSubject { get; private set; }

    public Subject<Unit> DeviceAsyncStoppedSubject { get; private set; }

    public int Duration
    {
      get => this.duration_;
      protected set
      {
        if (this.duration_ == value)
          return;
        this.duration_ = value;
        this.DurationChangedSubject.OnNext(new Unit());
      }
    }

    public abstract ulong FileSize { get; }

    public static AudioRecording Create() => (AudioRecording) new OpusAudioRecording();

    public abstract bool Start(StreamingUploadContext context = null);

    public abstract void Stop(bool skipResultNotify = false);

    protected void NotifyAudioResult(
      MemoryStream stream,
      int duration,
      StreamingUploadContext uploadContext = null)
    {
      uploadContext?.OnEof();
      this.AudioResultSubject.OnNext(new WaAudioArgs()
      {
        Stream = (Stream) stream,
        Duration = duration,
        MimeType = this.MimeType,
        FileExtension = this.FileExtension,
        AudioStreamingUploadContext = uploadContext,
        FileSize = this.FileSize
      });
    }
  }
}
