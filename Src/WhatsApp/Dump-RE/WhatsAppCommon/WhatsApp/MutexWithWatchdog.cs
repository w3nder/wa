// Decompiled with JetBrains decompiler
// Type: WhatsApp.MutexWithWatchdog
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Diagnostics;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public class MutexWithWatchdog
  {
    private Mutex m;
    private int? owner;
    private string name;
    public static int GlobalAcquireCount;
    private string ownerStack;
    private bool IsSettingsLock;

    public MutexWithWatchdog(string name, bool deadlockTracking = true)
    {
      this.m = new Mutex(false, name + AppState.AppUniqueSuffix);
      this.name = name;
      this.IsSettingsLock = !deadlockTracking;
    }

    public Mutex Base => this.m;

    private int GetThreadId() => Thread.CurrentThread.ManagedThreadId;

    public bool IsOwner()
    {
      int? owner = this.owner;
      int threadId = this.GetThreadId();
      return owner.HasValue && owner.Value == threadId;
    }

    private bool WaitWithWatchdog(int timeout = -1)
    {
      string str = (string) null;
      bool flag = false;
      if (timeout >= 0)
        return this.m.WaitOne(timeout);
      while (!this.m.WaitOne(AppState.IsBackgroundAgent ? 5000 : 9000))
      {
        Log.WriteLineDebug("taking a long time to acquire {0}", (object) this.name);
        string ownerStack = this.ownerStack;
        if (ownerStack != null)
          Log.WriteLineDebug("Owner's stack trace:\n{0}", (object) ownerStack);
        if (str == null)
          str = AppState.GetStackTrace();
        if (str != null)
          Log.WriteLineDebug("My stack (victim) trace:\n{0}", (object) str);
        flag = true;
      }
      if (flag)
        Log.WriteLineDebug("acquired {0} after some delay", (object) this.name);
      return true;
    }

    public bool WaitOne(int timeout = -1)
    {
      bool flag = false;
      Interlocked.Increment(ref MutexWithWatchdog.GlobalAcquireCount);
      try
      {
        try
        {
          flag = this.WaitWithWatchdog(timeout);
        }
        catch (Exception ex)
        {
          if (ex is AbandonedMutexException || ex.Message == "The wait completed due to an abandoned mutex.")
          {
            Log.WriteLineDebug("{0} - hit abandoned mutex.", (object) this.name);
            flag = true;
          }
          else
            throw;
        }
        if (flag)
        {
          if (this.owner.HasValue)
          {
            string stackTrace = DebugEnvironment.TryGetStackTrace();
            Log.SendCrashLog(new Exception("Lock used recursively"), "lock check" + (!string.IsNullOrEmpty(stackTrace) ? "\n" + stackTrace : ""));
            if (Debugger.IsAttached)
              Debugger.Break();
          }
          this.owner = new int?(this.GetThreadId());
          if (!this.IsSettingsLock)
            this.ownerStack = AppState.GetStackTrace();
        }
      }
      finally
      {
        if (!flag)
          Interlocked.Decrement(ref MutexWithWatchdog.GlobalAcquireCount);
      }
      return flag;
    }

    public void ReleaseMutex()
    {
      this.ownerStack = (string) null;
      this.owner = new int?();
      this.m.ReleaseMutex();
      Interlocked.Decrement(ref MutexWithWatchdog.GlobalAcquireCount);
    }
  }
}
