// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatusThread
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public class WaStatusThread
  {
    private WaStatus latestStatus;

    public string Jid { get; private set; }

    public WaStatus LatestStatus
    {
      get => this.latestStatus;
      set
      {
        if (this.latestStatus == value)
          return;
        this.latestStatus = value;
      }
    }

    public int Count { get; set; }

    public int ViewedCount { get; set; }

    public WaStatusThread(string jid, WaStatus latest)
    {
      this.Jid = jid;
      this.latestStatus = latest;
    }

    public virtual IObservable<WaStatus[]> LoadThreadAsync(
      bool unviewedOnly,
      TimeSpan withinTimeSpan)
    {
      return Observable.Create<WaStatus[]>((Func<IObserver<WaStatus[]>, Action>) (observer =>
      {
        WaStatus[] statuses = (WaStatus[]) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => statuses = db.GetStatuses(this.Jid, unviewedOnly, true, new TimeSpan?(withinTimeSpan))));
        observer.OnNext(statuses);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
