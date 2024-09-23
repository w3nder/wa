// Decompiled with JetBrains decompiler
// Type: WhatsApp.TimerBasedAudioRecording
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;

#nullable disable
namespace WhatsApp
{
  public abstract class TimerBasedAudioRecording : AudioRecording
  {
    private DateTime? recordingStart;
    private IDisposable timerSub;

    public override bool IsReady => true;

    protected void StartTimer()
    {
      this.StopTimer();
      this.recordingStart = new DateTime?(DateTime.Now);
      this.Duration = 0;
      this.timerSub = Observable.Timer(TimeSpan.FromMilliseconds(333.0)).Repeat<long>().ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        if (this.isStopRequested_)
          return;
        this.Duration = this.GetDuration();
      }));
    }

    protected void StopTimer()
    {
      if (this.timerSub == null)
        return;
      this.timerSub.Dispose();
      this.timerSub = (IDisposable) null;
    }

    protected void ResetTimer()
    {
      this.StopTimer();
      this.recordingStart = new DateTime?();
    }

    protected virtual int GetDuration()
    {
      return !this.recordingStart.HasValue ? 0 : (int) (DateTime.Now - this.recordingStart.Value).TotalSeconds;
    }
  }
}
