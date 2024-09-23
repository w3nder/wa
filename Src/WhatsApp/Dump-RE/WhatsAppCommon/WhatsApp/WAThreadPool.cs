// Decompiled with JetBrains decompiler
// Type: WhatsApp.WAThreadPool
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public static class WAThreadPool
  {
    private static object @lock = new object();
    private static int numJobs;
    private static Action onNoJobsLeft;
    private static Action<Action, string> onJobQueued;
    private static LinkedList<string> activeStacks;

    public static void QueueUserWorkItem(Action a, bool filterExceptions = true)
    {
      if (filterExceptions)
        a = SchedulerBase.WrapExceptionHandler(a);
      string stack = DebugEnvironment.TryGetStackTrace();
      lock (WAThreadPool.@lock)
      {
        if (WAThreadPool.onJobQueued != null)
        {
          WAThreadPool.onJobQueued(a, stack);
          return;
        }
        ++WAThreadPool.numJobs;
      }
      ThreadPool.QueueUserWorkItem((WaitCallback) (_ => WAThreadPool.ExecuteJob(a, stack)));
    }

    private static void ExecuteJob(Action a, string stack = null)
    {
      IDisposable d = (IDisposable) null;
      try
      {
        if (stack != null)
        {
          Utils.LazyInit<LinkedList<string>>(ref WAThreadPool.activeStacks, (Func<LinkedList<string>>) (() => new LinkedList<string>()));
          stack = "thread id: " + (object) NativeInterfaces.Misc.GetThreadId() + "\n" + stack;
          lock (WAThreadPool.@lock)
          {
            LinkedListNode<string> node = WAThreadPool.activeStacks.AddLast(stack);
            d = (IDisposable) new DisposableAction((Action) (() => node.List.Remove(node)));
          }
        }
        a();
      }
      catch (DatabaseInvalidatedException ex)
      {
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "thread pool exception");
      }
      finally
      {
        lock (WAThreadPool.@lock)
        {
          d.SafeDispose();
          if (--WAThreadPool.numJobs == 0)
          {
            Action onNoJobsLeft = WAThreadPool.onNoJobsLeft;
            if (onNoJobsLeft != null)
              onNoJobsLeft();
          }
        }
      }
    }

    public static PausedThread Pause(bool discardAtResume = false)
    {
      List<WAThreadPool.ActionWithStack> queue = new List<WAThreadPool.ActionWithStack>();
      ManualResetEvent waitForNoJobs = (ManualResetEvent) null;
      lock (WAThreadPool.@lock)
      {
        if (WAThreadPool.numJobs != 0)
        {
          waitForNoJobs = new ManualResetEvent(false);
          WAThreadPool.onNoJobsLeft = Utils.IgnoreMultipleInvokes((Action) (() => waitForNoJobs?.Set()));
        }
        WAThreadPool.onJobQueued = discardAtResume ? (Action<Action, string>) ((a, s) => { }) : (Action<Action, string>) ((a, s) => queue.Add(new WAThreadPool.ActionWithStack()
        {
          Action = a,
          Stack = s
        }));
      }
      return new PausedThread((Action) (() =>
      {
        if (waitForNoJobs == null)
          return;
        TimeSpan timeout = TimeSpan.FromSeconds(3.0);
        while (!waitForNoJobs.WaitOne(timeout))
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.AppendLine("Taking a long time waiting on thread pool");
          stringBuilder.AppendLine("Active stacks:");
          Utils.LazyInit<LinkedList<string>>(ref WAThreadPool.activeStacks, (Func<LinkedList<string>>) (() => new LinkedList<string>()));
          lock (WAThreadPool.@lock)
          {
            foreach (string str in (IEnumerable<string>) WAThreadPool.activeStacks ?? ((IEnumerable<string>) new string[0]).AsEnumerable<string>())
              stringBuilder.AppendLine(str);
          }
          Log.WriteLineDebug(stringBuilder.ToString());
          Log.SendCrashLog(new Exception("Thread pool watchdog exception"), "Thread pool watchdog exception");
          if (AppState.IsBackgroundAgent)
            break;
        }
        Interlocked.Exchange<ManualResetEvent>(ref waitForNoJobs, (ManualResetEvent) null)?.Dispose();
      }), (Action) (() =>
      {
        lock (WAThreadPool.@lock)
        {
          WAThreadPool.onJobQueued = (Action<Action, string>) null;
          WAThreadPool.onNoJobsLeft = (Action) null;
          WAThreadPool.numJobs += queue.Count;
        }
        if (queue.Count == 0)
          return;
        Log.WriteLineDebug("Resuming {0} thread pool jobs", (object) queue.Count);
        queue.ForEach((Action<WAThreadPool.ActionWithStack>) (a => ThreadPool.QueueUserWorkItem((WaitCallback) (_ => WAThreadPool.ExecuteJob(a.Action, a.Stack)))));
      }));
    }

    public static void RunAfterDelay(TimeSpan delay, Action a)
    {
      PooledTimer.Instance.Schedule(delay, (Action) (() => WAThreadPool.QueueUserWorkItem(a)));
    }

    public static IScheduler Scheduler => (IScheduler) new WAThreadPool.SchedulerImpl();

    private struct ActionWithStack
    {
      public Action Action;
      public string Stack;
    }

    public class SchedulerImpl : SchedulerBase
    {
      protected override void ScheduleImpl(Action a) => WAThreadPool.QueueUserWorkItem(a, false);
    }
  }
}
