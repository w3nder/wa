// Decompiled with JetBrains decompiler
// Type: WhatsApp.EncodeThreadAudioRecordingScheduler
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public class EncodeThreadAudioRecordingScheduler : AudioRecordingScheduler
  {
    private static WorkQueue worker;
    private static RefCountAction workerThreadSub = new RefCountAction((Action) (() => EncodeThreadAudioRecordingScheduler.worker = new WorkQueue(flags: WorkQueue.StartFlags.Unpausable | WorkQueue.StartFlags.WatchdogExcempt | WorkQueue.StartFlags.LowPri)), (Action) (() =>
    {
      EncodeThreadAudioRecordingScheduler.worker.Stop();
      EncodeThreadAudioRecordingScheduler.worker = (WorkQueue) null;
    }));
    private IDisposable thisWorkerSub;

    public override void PerformWithBuffer(byte[] buf, Action<byte[]> a)
    {
      if (this.thisWorkerSub == null)
        this.thisWorkerSub = EncodeThreadAudioRecordingScheduler.workerThreadSub.Subscribe();
      EncodeThreadAudioRecordingScheduler.worker.Enqueue((Action) (() => a(buf)));
    }

    public override byte[] GetFinalBuffer(Func<byte[]> get)
    {
      byte[] b = (byte[]) null;
      if (this.thisWorkerSub == null)
        return (byte[]) null;
      using (ManualResetEventSlim ev = new ManualResetEventSlim())
      {
        EncodeThreadAudioRecordingScheduler.worker.Enqueue((Action) (() =>
        {
          b = get();
          ev.Set();
        }));
        ev.WaitOne();
      }
      this.thisWorkerSub.SafeDispose();
      this.thisWorkerSub = (IDisposable) null;
      return b;
    }
  }
}
