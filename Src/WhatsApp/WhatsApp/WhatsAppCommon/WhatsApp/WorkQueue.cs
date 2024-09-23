// Decompiled with JetBrains decompiler
// Type: WhatsApp.WorkQueue
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Threading;


namespace WhatsApp
{
  public class WorkQueue : SchedulerBase
  {
    private object syncLock = new object();
    private LinkedList<Action>[] items = new LinkedList<Action>[2]
    {
      new LinkedList<Action>(),
      new LinkedList<Action>()
    };
    private Semaphore consumerSem;
    private Semaphore producerSem;
    private bool cancelled;
    private bool pausable = true;
    private bool lowPri;
    private TimeSpan? watchdogTimeout;
    private static PooledTimer unpausableTimer;
    private static LinkedList<WorkQueue> activeWorkers = new LinkedList<WorkQueue>();
    private static object activeWorkersLock = new object();
    private static Func<Action> workerCreateAction = (Func<Action>) null;
    private string ownerStack;
    private SchedulerBase.DebugHookArgs debugHook;

    private static PooledTimer UnpausableTimer
    {
      get
      {
        return Utils.LazyInit<PooledTimer>(ref WorkQueue.unpausableTimer, (Func<PooledTimer>) (() => new PooledTimer()
        {
          PerformActionFunc = (Action<Action>) (a => a())
        }));
      }
    }

    protected override PooledTimer QueueTimer
    {
      get => this.pausable ? PooledTimer.Instance : WorkQueue.UnpausableTimer;
    }

    public WorkQueue(int maxItems = 0, WorkQueue.StartFlags flags = WorkQueue.StartFlags.None, string identifierString = null)
    {
      this.pausable = (flags & WorkQueue.StartFlags.Unpausable) == WorkQueue.StartFlags.None;
      int num = (flags & WorkQueue.StartFlags.DelayStart) == WorkQueue.StartFlags.None ? 1 : 0;
      this.lowPri = (flags & WorkQueue.StartFlags.LowPri) != 0;
      if ((flags & WorkQueue.StartFlags.WatchdogExcempt) == WorkQueue.StartFlags.None)
        this.watchdogTimeout = new TimeSpan?(Constants.WatchdogTimeout);
      if (maxItems != 0)
        this.producerSem = new Semaphore(maxItems);
      this.consumerSem = new Semaphore(0);
      if (num == 0)
        return;
      this.StartWorkerThread(identifierString);
    }

    public void Enqueue(Action a, WorkQueue.Priority prio = WorkQueue.Priority.Normal, bool filterException = true)
    {
      if (filterException)
        a = SchedulerBase.WrapExceptionHandler(a, this.DebugHook);
      if (this.producerSem != null)
        this.producerSem.Wait();
      lock (this.syncLock)
        this.items[(int) prio].AddLast(a);
      this.consumerSem.Post();
    }

    public void RunAfterDelay(TimeSpan delay, Action a, WorkQueue.Priority prio = WorkQueue.Priority.Normal)
    {
      this.QueueTimer.Schedule(delay, (Action) (() => this.Enqueue(a, prio)));
    }

    public Action Dequeue()
    {
      Action action = (Action) null;
      this.consumerSem.Wait();
      lock (this.syncLock)
      {
        for (int index = 0; index < this.items.Length; ++index)
        {
          LinkedList<Action> linkedList = this.items[index];
          LinkedListNode<Action> first = linkedList.First;
          if (first != null)
          {
            action = first.Value;
            linkedList.RemoveFirst();
            break;
          }
        }
      }
      if (action != null && this.producerSem != null)
        this.producerSem.Post();
      return action;
    }

    public void StartWorkerThread(string identifierString = null)
    {
      Action threadAction = (Action) (() =>
      {
        if (identifierString != null)
          Log.l(nameof (WorkQueue), "ID: {0}", (object) identifierString);
        LinkedListNode<WorkQueue> node = (LinkedListNode<WorkQueue>) null;
        while (this.pausable)
        {
          Action action = (Action) null;
          lock (WorkQueue.activeWorkersLock)
          {
            action = (WorkQueue.workerCreateAction ?? (Func<Action>) (() => (Action) null))();
            if (action == null)
              node = WorkQueue.activeWorkers.AddLast(this);
          }
          if (action != null)
            action();
          if (node != null)
            break;
        }
        while (!this.cancelled)
        {
          Action action = this.Dequeue();
          if (action != null)
          {
            try
            {
              action();
            }
            catch (Exception ex)
            {
              Log.SendCrashLog(ex, "worker thread");
            }
          }
        }
        if (!this.pausable)
          return;
        lock (WorkQueue.activeWorkersLock)
          node.List.Remove(node);
        foreach (Action action in this.items[0])
          action();
      });
      if (this.lowPri)
      {
        Action snap = threadAction;
        threadAction = (Action) (() => NativeInterfaces.Misc.LowerPriority(snap.AsComAction()));
      }
      new Thread((ThreadStart) (() => threadAction())).Start();
    }

    public void Stop(WorkQueue.Priority prio = WorkQueue.Priority.Normal, Action onStop = null)
    {
      this.Enqueue((Action) (() =>
      {
        this.cancelled = true;
        Action action = onStop;
        if (action == null)
          return;
        action();
      }), prio);
    }

    public static PausedThread Pause(bool discardAtResume = false)
    {
      ManualResetEvent allThreadsPausedEvent = new ManualResetEvent(false);
      ManualResetEvent unpauseEvent = new ManualResetEvent(false);
      int count = 0;
      int unpauseCount = 0;
      Action unpauseUnref = (Action) (() =>
      {
        if (Interlocked.Decrement(ref unpauseCount) != 0)
          return;
        unpauseEvent.Dispose();
      });
      Action unpauseWait = (Action) (() =>
      {
        unpauseEvent.WaitOne();
        unpauseUnref();
      });
      Action onWorkerThreadReadyForPause = (Action) (() =>
      {
        if (Interlocked.Decrement(ref count) != 0)
          return;
        allThreadsPausedEvent.Set();
      });
      lock (WorkQueue.activeWorkersLock)
      {
        count = WorkQueue.activeWorkers.Count + 1;
        unpauseCount = count + 1;
        WorkQueue.workerCreateAction = (Func<Action>) (() =>
        {
          Interlocked.Increment(ref unpauseCount);
          return unpauseWait;
        });
        foreach (WorkQueue activeWorker in WorkQueue.activeWorkers)
          activeWorker.Enqueue((Action) (() =>
          {
            onWorkerThreadReadyForPause();
            unpauseWait();
          }), WorkQueue.Priority.Interrupt);
      }
      onWorkerThreadReadyForPause();
      return new PausedThread((Action) (() =>
      {
        allThreadsPausedEvent.WaitOne();
        allThreadsPausedEvent.Dispose();
      }), (Action) (() =>
      {
        int num = 0;
        int jobs = 0;
        int discarded = 0;
        lock (WorkQueue.activeWorkersLock)
        {
          WorkQueue.workerCreateAction = (Func<Action>) null;
          num = WorkQueue.activeWorkers.Count;
          Action<LinkedList<Action>> onQueue = (Action<LinkedList<Action>>) (prio => jobs += prio.Count);
          Action<WorkQueue> action = (Action<WorkQueue>) (worker =>
          {
            foreach (LinkedList<Action> linkedList in worker.items)
              onQueue(linkedList);
          });
          if (discardAtResume)
          {
            Action<LinkedList<Action>> innerQueue = onQueue;
            onQueue = (Action<LinkedList<Action>>) (prio =>
            {
              discarded += prio.Count;
              prio.Clear();
              innerQueue(prio);
            });
            Action<WorkQueue> innerWorker = action;
            action = (Action<WorkQueue>) (worker =>
            {
              lock (worker.syncLock)
                innerWorker(worker);
            });
          }
          foreach (WorkQueue activeWorker in WorkQueue.activeWorkers)
            action(activeWorker);
        }
        Log.WriteLineDebug("Resuming {0} worker threads, {1} jobs resumed, {2} jobs discarded", (object) num, (object) jobs, (object) discarded);
        unpauseEvent.Set();
        unpauseUnref();
      }));
    }

    protected override void ScheduleImpl(Action a)
    {
      this.Enqueue((Action) (() =>
      {
        if (this.cancelled)
          return;
        a();
      }), filterException: false);
    }

    protected override SchedulerBase.DebugHookArgs DebugHook
    {
      get
      {
        return Utils.LazyInit<SchedulerBase.DebugHookArgs>(ref this.debugHook, (Func<SchedulerBase.DebugHookArgs>) (() => new SchedulerBase.DebugHookArgs()
        {
          WatchdogTimeout = this.watchdogTimeout,
          OnDebugStack = (Action<string>) (stack => this.ownerStack = stack),
          OnTimeout = (Action) (() =>
          {
            string ownerStack = this.ownerStack;
            Log.WriteLineDebug("WorkQueue appears blocked!" + (ownerStack != null ? " Owner's stack trace:\n" + ownerStack : ""));
          })
        }));
      }
    }

    public enum StartFlags
    {
      Default = 0,
      None = 0,
      DelayStart = 1,
      Unpausable = 2,
      WatchdogExcempt = 4,
      LowPri = 8,
    }

    public enum Priority
    {
      Interrupt,
      Normal,
    }
  }
}
