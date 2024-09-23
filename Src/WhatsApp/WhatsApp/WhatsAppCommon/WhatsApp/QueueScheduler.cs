// Decompiled with JetBrains decompiler
// Type: WhatsApp.QueueScheduler
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Threading;


namespace WhatsApp
{
  public class QueueScheduler : SchedulerBase, IDisposable
  {
    private object @lock = new object();
    private object disposeLock = new object();
    private List<Action> actions = new List<Action>();
    private AutoResetEvent ev = new AutoResetEvent(false);
    private bool complete;

    public void Dispose()
    {
      lock (this.@lock)
      {
        int num = this.complete ? 1 : 0;
        this.complete = true;
        this.actions.Clear();
        if (num == 0)
          this.ev.Set();
      }
      lock (this.disposeLock)
      {
        if (this.ev == null)
          return;
        this.ev.Dispose();
        this.ev = (AutoResetEvent) null;
      }
    }

    protected override void ScheduleImpl(Action a)
    {
      lock (this.@lock)
      {
        if (this.complete)
          return;
        int num = this.actions.Count == 0 ? 1 : 0;
        this.actions.Add(a);
        if (num == 0)
          return;
        this.ev.Set();
      }
    }

    public void OnComplete()
    {
      lock (this.@lock)
      {
        if (this.complete)
          return;
        this.complete = true;
        this.ev.Set();
      }
    }

    public bool Complete => this.complete;

    public void Drain()
    {
      lock (this.@lock)
      {
        if (this.complete)
        {
          if (this.actions.Count == 0)
            return;
        }
      }
      lock (this.disposeLock)
      {
        if (this.ev == null)
          return;
        this.ev.WaitOne();
      }
      List<Action> actionList = (List<Action>) null;
      lock (this.@lock)
      {
        actionList = this.actions;
        this.actions = new List<Action>();
      }
      actionList.ForEach((Action<Action>) (a => a()));
    }
  }
}
