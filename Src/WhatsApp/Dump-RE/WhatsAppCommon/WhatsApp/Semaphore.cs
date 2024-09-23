// Decompiled with JetBrains decompiler
// Type: WhatsApp.Semaphore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public class Semaphore
  {
    private object syncLock = new object();
    private LinkedList<ManualResetEvent> waiters = new LinkedList<ManualResetEvent>();
    private int value;

    public Semaphore(int initial) => this.value = initial;

    public void Wait()
    {
      ManualResetEvent manualResetEvent = (ManualResetEvent) null;
      lock (this.syncLock)
      {
        if (--this.value < 0)
        {
          manualResetEvent = new ManualResetEvent(false);
          this.waiters.AddLast(manualResetEvent);
        }
      }
      if (manualResetEvent == null)
        return;
      manualResetEvent.WaitOne();
      manualResetEvent.Dispose();
    }

    public void Post()
    {
      ManualResetEvent manualResetEvent = (ManualResetEvent) null;
      lock (this.syncLock)
      {
        LinkedListNode<ManualResetEvent> first = this.waiters.First;
        if (++this.value <= 0)
        {
          if (first != null)
          {
            manualResetEvent = first.Value;
            this.waiters.RemoveFirst();
          }
        }
      }
      manualResetEvent?.Set();
    }
  }
}
