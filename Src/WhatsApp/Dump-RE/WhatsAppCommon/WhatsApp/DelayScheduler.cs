﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.DelayScheduler
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;

#nullable disable
namespace WhatsApp
{
  public class DelayScheduler : SchedulerBase
  {
    private IScheduler sched;
    private TimeSpan delay;

    public DelayScheduler(IScheduler sched, TimeSpan delay)
    {
      this.sched = sched;
      this.delay = delay;
    }

    protected override void ScheduleImpl(Action a) => this.sched.Schedule(a, this.delay);

    public override IDisposable Schedule(Action a) => this.sched.Schedule(a, this.delay);
  }
}
