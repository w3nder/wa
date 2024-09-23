// Decompiled with JetBrains decompiler
// Type: WhatsApp.PooledTimer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Threading;


namespace WhatsApp
{
  public class PooledTimer
  {
    private LinkedList<PooledTimer.Timer> timers = new LinkedList<PooledTimer.Timer>();
    private object @lock = new object();
    private ManualResetEvent ev = new ManualResetEvent(false);
    private bool threadStarted;
    public Action<Action> PerformActionFunc = (Action<Action>) (a => WAThreadPool.QueueUserWorkItem(a));
    private static PooledTimer instance = (PooledTimer) null;
    private static object instanceLock = new object();

    private void ThreadProc()
    {
      while (true)
      {
        List<Action> pendingActions = new List<Action>();
        int millisecondsTimeout = -1;
        lock (this.@lock)
        {
          this.ev.Reset();
          ulong tickCount = NativeInterfaces.Misc.GetTickCount();
          LinkedListNode<PooledTimer.Timer> next;
          for (LinkedListNode<PooledTimer.Timer> node = this.timers.First; node != null && (long) node.Value.DueTime - (long) tickCount <= 0L; node = next)
          {
            next = node.Next;
            pendingActions.Add(node.Value.OnSignalled);
            this.timers.Remove(node);
          }
          LinkedListNode<PooledTimer.Timer> first = this.timers.First;
          if (first != null)
            millisecondsTimeout = (int) ((long) first.Value.DueTime - (long) tickCount);
        }
        if (pendingActions.Count != 0)
          this.PerformActionFunc((Action) (() => pendingActions.ForEach((Action<Action>) (a => a()))));
        this.ev.WaitOne(millisecondsTimeout);
      }
    }

    public IDisposable Schedule(TimeSpan delay, Action a)
    {
      LinkedListNode<PooledTimer.Timer> node = (LinkedListNode<PooledTimer.Timer>) null;
      lock (this.@lock)
      {
        if (!this.threadStarted)
        {
          new Thread((ParameterizedThreadStart) (_ => this.ThreadProc())).Start();
          this.threadStarted = true;
        }
        ulong tickCount = NativeInterfaces.Misc.GetTickCount();
        node = this.timers.InsertInOrder<PooledTimer.Timer>(new PooledTimer.Timer()
        {
          DueTime = (ulong) ((double) tickCount + delay.TotalMilliseconds),
          OnSignalled = a
        }, (Func<PooledTimer.Timer, PooledTimer.Timer, bool>) ((old, @new) => (long) @new.DueTime - (long) old.DueTime < 0L));
        if (node == this.timers.First)
          this.ev.Set();
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (this.@lock)
        {
          if (node != null && node.List != null)
            node.List.Remove(node);
          node = (LinkedListNode<PooledTimer.Timer>) null;
        }
      }));
    }

    public static PooledTimer Instance
    {
      get
      {
        return Utils.LazyInit<PooledTimer>(ref PooledTimer.instance, (Func<PooledTimer>) (() => new PooledTimer()), PooledTimer.instanceLock);
      }
    }

    internal class Timer
    {
      public ulong DueTime;
      public Action OnSignalled;
    }
  }
}
