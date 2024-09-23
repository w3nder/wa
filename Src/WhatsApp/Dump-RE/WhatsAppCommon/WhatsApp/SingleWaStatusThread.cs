// Decompiled with JetBrains decompiler
// Type: WhatsApp.SingleWaStatusThread
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;

#nullable disable
namespace WhatsApp
{
  public class SingleWaStatusThread : WaStatusThread
  {
    public WaStatus Status { get; private set; }

    public SingleWaStatusThread(WaStatus status)
      : base(status.Jid, status)
    {
      this.Status = status;
      this.Count = 1;
      this.ViewedCount = status.IsViewed ? 1 : 0;
    }

    public override IObservable<WaStatus[]> LoadThreadAsync(
      bool unviewedOnly,
      TimeSpan withinTimeSpan)
    {
      return Observable.Return<WaStatus[]>(new WaStatus[1]
      {
        this.Status
      });
    }
  }
}
