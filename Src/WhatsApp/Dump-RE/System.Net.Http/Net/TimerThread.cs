// Decompiled with JetBrains decompiler
// Type: System.Net.TimerThread
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

#nullable disable
namespace System.Net
{
  internal static class TimerThread
  {
    private const int c_ThreadIdleTimeoutMilliseconds = 30000;
    private const int c_CacheScanPerIterations = 32;
    private const int c_TickCountResolution = 15;
    private static LinkedList<WeakReference> s_Queues = new LinkedList<WeakReference>();
    private static LinkedList<WeakReference> s_NewQueues = new LinkedList<WeakReference>();
    private static int s_ThreadState = 0;
    private static AutoResetEvent s_ThreadReadyEvent = new AutoResetEvent(false);
    private static ManualResetEvent s_ThreadShutdownEvent = new ManualResetEvent(false);
    private static WaitHandle[] s_ThreadEvents;
    private static int s_CacheScanIteration;
    private static Dictionary<int, WeakReference> s_QueuesCache = new Dictionary<int, WeakReference>();

    static TimerThread()
    {
      TimerThread.s_ThreadEvents = new WaitHandle[2]
      {
        (WaitHandle) TimerThread.s_ThreadShutdownEvent,
        (WaitHandle) TimerThread.s_ThreadReadyEvent
      };
    }

    internal static TimerThread.Queue CreateQueue(int durationMilliseconds)
    {
      if (durationMilliseconds == -1)
        return (TimerThread.Queue) new TimerThread.InfiniteTimerQueue();
      if (durationMilliseconds < 0)
        throw new ArgumentOutOfRangeException(nameof (durationMilliseconds));
      TimerThread.TimerQueue target;
      lock (TimerThread.s_NewQueues)
      {
        target = new TimerThread.TimerQueue(durationMilliseconds);
        WeakReference weakReference = new WeakReference((object) target);
        TimerThread.s_NewQueues.AddLast(weakReference);
      }
      return (TimerThread.Queue) target;
    }

    internal static TimerThread.Queue GetOrCreateQueue(int durationMilliseconds)
    {
      if (durationMilliseconds == -1)
        return (TimerThread.Queue) new TimerThread.InfiniteTimerQueue();
      if (durationMilliseconds < 0)
        throw new ArgumentOutOfRangeException(nameof (durationMilliseconds));
      WeakReference weakReference = (WeakReference) null;
      TimerThread.s_QueuesCache.TryGetValue(durationMilliseconds, out weakReference);
      TimerThread.TimerQueue target;
      if (weakReference == null || (target = (TimerThread.TimerQueue) weakReference.Target) == null)
      {
        lock (TimerThread.s_NewQueues)
        {
          TimerThread.s_QueuesCache.TryGetValue(durationMilliseconds, out weakReference);
          if (weakReference != null)
          {
            if ((target = (TimerThread.TimerQueue) weakReference.Target) != null)
              goto label_21;
          }
          target = new TimerThread.TimerQueue(durationMilliseconds);
          weakReference = new WeakReference((object) target);
          TimerThread.s_NewQueues.AddLast(weakReference);
          TimerThread.s_QueuesCache[durationMilliseconds] = weakReference;
          if (++TimerThread.s_CacheScanIteration % 32 == 0)
          {
            List<int> intList = new List<int>();
            foreach (KeyValuePair<int, WeakReference> keyValuePair in TimerThread.s_QueuesCache)
            {
              if (keyValuePair.Value.Target == null)
                intList.Add(keyValuePair.Key);
            }
            for (int index = 0; index < intList.Count; ++index)
              TimerThread.s_QueuesCache.Remove(intList[index]);
          }
        }
      }
label_21:
      return (TimerThread.Queue) target;
    }

    private static void Prod()
    {
      TimerThread.s_ThreadReadyEvent.Set();
      if (Interlocked.CompareExchange(ref TimerThread.s_ThreadState, 1, 0) != 0)
        return;
      new Task(new Action(TimerThread.ThreadProc)).Start();
    }

    private static void ThreadProc()
    {
      lock (TimerThread.s_Queues)
      {
        if (Interlocked.CompareExchange(ref TimerThread.s_ThreadState, 1, 1) != 1)
          return;
        bool flag1 = true;
label_30:
        while (flag1)
        {
          try
          {
            TimerThread.s_ThreadReadyEvent.Reset();
            do
            {
              bool flag2;
              do
              {
                if (TimerThread.s_NewQueues.Count > 0)
                {
                  lock (TimerThread.s_NewQueues)
                  {
                    for (LinkedListNode<WeakReference> first = TimerThread.s_NewQueues.First; first != null; first = TimerThread.s_NewQueues.First)
                    {
                      TimerThread.s_NewQueues.Remove(first);
                      TimerThread.s_Queues.AddLast(first);
                    }
                  }
                }
                int tickCount1 = Environment.TickCount;
                int end = 0;
                flag2 = false;
                LinkedListNode<WeakReference> node = TimerThread.s_Queues.First;
                while (node != null)
                {
                  TimerThread.TimerQueue target = (TimerThread.TimerQueue) node.Value.Target;
                  if (target == null)
                  {
                    LinkedListNode<WeakReference> next = node.Next;
                    TimerThread.s_Queues.Remove(node);
                    node = next;
                  }
                  else
                  {
                    int nextExpiration;
                    if (target.Fire(out nextExpiration) && (!flag2 || TimerThread.IsTickBetween(tickCount1, end, nextExpiration)))
                    {
                      end = nextExpiration;
                      flag2 = true;
                    }
                    node = node.Next;
                  }
                }
                int tickCount2 = Environment.TickCount;
                int millisecondsTimeout = flag2 ? (TimerThread.IsTickBetween(tickCount1, end, tickCount2) ? (int) Math.Min((uint) (end - tickCount2), 2147483632U) + 15 : 0) : 30000;
                switch (WaitHandle.WaitAny(TimerThread.s_ThreadEvents, millisecondsTimeout))
                {
                  case 0:
                    flag1 = false;
                    goto label_30;
                  case 258:
                    continue;
                  default:
                    continue;
                }
              }
              while (flag2);
              Interlocked.CompareExchange(ref TimerThread.s_ThreadState, 0, 1);
            }
            while (TimerThread.s_ThreadReadyEvent.WaitOne(0) && Interlocked.CompareExchange(ref TimerThread.s_ThreadState, 1, 0) == 0);
            flag1 = false;
          }
          catch (Exception ex)
          {
            if (NclUtilities.IsFatal(ex))
            {
              throw;
            }
            else
            {
              if (Logging.On)
                Logging.PrintError(Logging.Web, "TimerThread#" + Thread.CurrentThread.ManagedThreadId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "::ThreadProc() - Exception:" + ex.ToString());
              new ManualResetEvent(false).WaitOne(1000);
            }
          }
        }
      }
    }

    private static void StopTimerThread()
    {
      Interlocked.Exchange(ref TimerThread.s_ThreadState, 2);
      TimerThread.s_ThreadShutdownEvent.Set();
    }

    private static bool IsTickBetween(int start, int end, int comparand)
    {
      return start <= comparand == end <= comparand != start <= end;
    }

    private static void OnDomainUnload(object sender, EventArgs e)
    {
      try
      {
        TimerThread.StopTimerThread();
      }
      catch
      {
      }
    }

    internal abstract class Queue
    {
      private readonly int m_DurationMilliseconds;

      internal Queue(int durationMilliseconds)
      {
        this.m_DurationMilliseconds = durationMilliseconds;
      }

      internal int Duration => this.m_DurationMilliseconds;

      internal TimerThread.Timer CreateTimer()
      {
        return this.CreateTimer((TimerThread.Callback) null, (object) null);
      }

      internal abstract TimerThread.Timer CreateTimer(TimerThread.Callback callback, object context);
    }

    internal abstract class Timer : IDisposable
    {
      private readonly int m_StartTimeMilliseconds;
      private readonly int m_DurationMilliseconds;

      internal Timer(int durationMilliseconds)
      {
        this.m_DurationMilliseconds = durationMilliseconds;
        this.m_StartTimeMilliseconds = Environment.TickCount;
      }

      internal int Duration => this.m_DurationMilliseconds;

      internal int StartTime => this.m_StartTimeMilliseconds;

      internal int Expiration => this.m_StartTimeMilliseconds + this.m_DurationMilliseconds;

      internal int TimeRemaining
      {
        get
        {
          if (this.HasExpired)
            return 0;
          if (this.Duration == -1)
            return -1;
          int tickCount = Environment.TickCount;
          int num = TimerThread.IsTickBetween(this.StartTime, this.Expiration, tickCount) ? (int) Math.Min((uint) (this.Expiration - tickCount), (uint) int.MaxValue) : 0;
          return num >= 2 ? num : num + 1;
        }
      }

      internal abstract bool Cancel();

      internal abstract bool HasExpired { get; }

      public void Dispose() => this.Cancel();
    }

    internal delegate void Callback(TimerThread.Timer timer, int timeNoticed, object context);

    private enum TimerThreadState
    {
      Idle,
      Running,
      Stopped,
    }

    private class TimerQueue : TimerThread.Queue
    {
      private readonly TimerThread.TimerNode m_Timers;

      internal TimerQueue(int durationMilliseconds)
        : base(durationMilliseconds)
      {
        this.m_Timers = new TimerThread.TimerNode();
        this.m_Timers.Next = this.m_Timers;
        this.m_Timers.Prev = this.m_Timers;
      }

      internal override TimerThread.Timer CreateTimer(TimerThread.Callback callback, object context)
      {
        TimerThread.TimerNode timer = new TimerThread.TimerNode(callback, context, this.Duration, (object) this.m_Timers);
        bool flag = false;
        lock (this.m_Timers)
        {
          if (this.m_Timers.Next == this.m_Timers)
            flag = true;
          timer.Next = this.m_Timers;
          timer.Prev = this.m_Timers.Prev;
          this.m_Timers.Prev.Next = timer;
          this.m_Timers.Prev = timer;
        }
        if (flag)
          TimerThread.Prod();
        return (TimerThread.Timer) timer;
      }

      internal bool Fire(out int nextExpiration)
      {
        TimerThread.TimerNode next;
        do
        {
          next = this.m_Timers.Next;
          if (next == this.m_Timers)
          {
            lock (this.m_Timers)
            {
              next = this.m_Timers.Next;
              if (next == this.m_Timers)
              {
                nextExpiration = 0;
                return false;
              }
            }
          }
        }
        while (next.Fire());
        nextExpiration = next.Expiration;
        return true;
      }
    }

    private class InfiniteTimerQueue : TimerThread.Queue
    {
      internal InfiniteTimerQueue()
        : base(-1)
      {
      }

      internal override TimerThread.Timer CreateTimer(TimerThread.Callback callback, object context)
      {
        return (TimerThread.Timer) new TimerThread.InfiniteTimer();
      }
    }

    private class TimerNode : TimerThread.Timer
    {
      private TimerThread.TimerNode.TimerState m_TimerState;
      private TimerThread.Callback m_Callback;
      private object m_Context;
      private object m_QueueLock;
      private TimerThread.TimerNode next;
      private TimerThread.TimerNode prev;

      internal TimerNode(
        TimerThread.Callback callback,
        object context,
        int durationMilliseconds,
        object queueLock)
        : base(durationMilliseconds)
      {
        if (callback != null)
        {
          this.m_Callback = callback;
          this.m_Context = context;
        }
        this.m_TimerState = TimerThread.TimerNode.TimerState.Ready;
        this.m_QueueLock = queueLock;
      }

      internal TimerNode()
        : base(0)
      {
        this.m_TimerState = TimerThread.TimerNode.TimerState.Sentinel;
      }

      internal override bool HasExpired
      {
        get => this.m_TimerState == TimerThread.TimerNode.TimerState.Fired;
      }

      internal TimerThread.TimerNode Next
      {
        get => this.next;
        set => this.next = value;
      }

      internal TimerThread.TimerNode Prev
      {
        get => this.prev;
        set => this.prev = value;
      }

      internal override bool Cancel()
      {
        if (this.m_TimerState == TimerThread.TimerNode.TimerState.Ready)
        {
          lock (this.m_QueueLock)
          {
            if (this.m_TimerState == TimerThread.TimerNode.TimerState.Ready)
            {
              this.Next.Prev = this.Prev;
              this.Prev.Next = this.Next;
              this.Next = (TimerThread.TimerNode) null;
              this.Prev = (TimerThread.TimerNode) null;
              this.m_Callback = (TimerThread.Callback) null;
              this.m_Context = (object) null;
              this.m_TimerState = TimerThread.TimerNode.TimerState.Cancelled;
              return true;
            }
          }
        }
        return false;
      }

      internal bool Fire()
      {
        if (this.m_TimerState != TimerThread.TimerNode.TimerState.Ready)
          return true;
        int tickCount = Environment.TickCount;
        if (TimerThread.IsTickBetween(this.StartTime, this.Expiration, tickCount))
          return false;
        bool flag = false;
        lock (this.m_QueueLock)
        {
          if (this.m_TimerState == TimerThread.TimerNode.TimerState.Ready)
          {
            this.m_TimerState = TimerThread.TimerNode.TimerState.Fired;
            this.Next.Prev = this.Prev;
            this.Prev.Next = this.Next;
            this.Next = (TimerThread.TimerNode) null;
            this.Prev = (TimerThread.TimerNode) null;
            flag = this.m_Callback != null;
          }
        }
        if (flag)
        {
          try
          {
            TimerThread.Callback callback = this.m_Callback;
            object context = this.m_Context;
            this.m_Callback = (TimerThread.Callback) null;
            this.m_Context = (object) null;
            callback((TimerThread.Timer) this, tickCount, context);
          }
          catch (Exception ex)
          {
            if (NclUtilities.IsFatal(ex))
              throw;
            else if (Logging.On)
              Logging.PrintError(Logging.Web, "TimerThreadTimer#" + this.StartTime.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "::Fire() - " + (object) ex);
          }
        }
        return true;
      }

      private enum TimerState
      {
        Ready,
        Fired,
        Cancelled,
        Sentinel,
      }
    }

    private class InfiniteTimer : TimerThread.Timer
    {
      private int cancelled;

      internal InfiniteTimer()
        : base(-1)
      {
      }

      internal override bool HasExpired => false;

      internal override bool Cancel() => Interlocked.Exchange(ref this.cancelled, 1) == 0;
    }
  }
}
