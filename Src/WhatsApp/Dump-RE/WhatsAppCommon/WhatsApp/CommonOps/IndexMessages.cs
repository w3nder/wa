// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.IndexMessages
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class IndexMessages
  {
    public static WaScheduledTask CreateScheduledTask()
    {
      return new WaScheduledTask(WaScheduledTask.Types.IndexMessages, (string) null, (byte[]) null, WaScheduledTask.Restrictions.None, new TimeSpan?());
    }

    public static bool IndexingInProgress
    {
      get
      {
        return MessagesContext.Select<bool>((Func<MessagesContext, bool>) (db => ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
        {
          WaScheduledTask.Types.IndexMessages
        }, excludeExpired: false, limit: new int?(1))).Any<WaScheduledTask>()));
      }
    }

    public static IObservable<Unit> BatchMessageIndex()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        bool cancel = false;
        Action attempt = (Action) null;
        attempt = (Action) (() =>
        {
          if (cancel)
            return;
          Log.l("message index", "attempting");
          int numIndexed = 0;
          try
          {
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => numIndexed = db.IndexMessageBatchForSearch(500)));
          }
          catch (Exception ex)
          {
            observer.OnError(ex);
            observer.OnCompleted();
            return;
          }
          Log.l("message index", "{0} message(s) indexed", (object) numIndexed);
          if (numIndexed <= 0)
          {
            Log.l("message index", "finished", (object) numIndexed);
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }
          else
            PooledTimer.Instance.Schedule(TimeSpan.FromMilliseconds(4000.0), attempt);
        });
        WAThreadPool.QueueUserWorkItem(attempt);
        return (Action) (() => cancel = true);
      }));
    }
  }
}
