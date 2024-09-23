// Decompiled with JetBrains decompiler
// Type: WhatsApp.SchedulerBase
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public abstract class SchedulerBase : IScheduler
  {
    private static PooledTimer watchdog;

    protected abstract void ScheduleImpl(Action a);

    protected virtual SchedulerBase.DebugHookArgs DebugHook => (SchedulerBase.DebugHookArgs) null;

    protected virtual PooledTimer QueueTimer => PooledTimer.Instance;

    public static Action WrapExceptionHandler(Action a, SchedulerBase.DebugHookArgs args = null)
    {
      string stack = DebugEnvironment.TryGetStackTrace();
      Action<string> onDebugStack = (Action<string>) (s => { });
      IDisposable watchdogSub = (IDisposable) null;
      if (args != null && args.WatchdogTimeout.HasValue)
      {
        Utils.LazyInit<PooledTimer>(ref SchedulerBase.watchdog, (Func<PooledTimer>) (() => new PooledTimer()
        {
          PerformActionFunc = (Action<Action>) (fn => fn())
        }));
        watchdogSub = SchedulerBase.watchdog.Schedule(args.WatchdogTimeout.Value, (Action) (() => args.OnTimeout()));
      }
      if (stack != null && args != null && args.OnDebugStack != null)
        onDebugStack = args.OnDebugStack;
      return (Action) (() =>
      {
        try
        {
          onDebugStack(stack);
          a();
        }
        catch (DatabaseInvalidatedException ex)
        {
        }
        catch (Exception ex)
        {
          if (stack != null)
          {
            Log.LogException(ex, "exception on scheduler thread");
            Log.WriteLineDebug("enqueuer's stack trace:\n" + stack);
          }
          throw;
        }
        finally
        {
          watchdogSub.SafeDispose();
          onDebugStack((string) null);
        }
      });
    }

    public virtual IDisposable Schedule(Action a)
    {
      if (a == null)
        return (IDisposable) new DisposableAction((Action) (() => { }));
      bool cancel = false;
      Action aa = SchedulerBase.WrapExceptionHandler(a, this.DebugHook);
      a = (Action) (() =>
      {
        if (cancel)
          return;
        aa();
      });
      try
      {
        this.ScheduleImpl(a);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "scheduler exception");
        throw;
      }
      return (IDisposable) new DisposableAction((Action) (() => cancel = true));
    }

    public IDisposable Schedule(Action action, TimeSpan dueTime)
    {
      if (dueTime <= TimeSpan.Zero)
        return this.Schedule(action);
      object @lock = new object();
      IDisposable disp = (IDisposable) null;
      bool cancel = false;
      this.QueueTimer.Schedule(dueTime, (Action) (() =>
      {
        lock (@lock)
        {
          if (cancel)
            return;
          disp = this.Schedule(action);
        }
      }));
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (@lock)
        {
          cancel = true;
          disp.SafeDispose();
          disp = (IDisposable) null;
        }
      }));
    }

    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    public class DebugHookArgs
    {
      public TimeSpan? WatchdogTimeout;
      public Action OnTimeout;
      public Action<string> OnDebugStack;
    }
  }
}
