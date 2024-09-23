// Decompiled with JetBrains decompiler
// Type: WhatsApp.ObservableQueueExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public static class ObservableQueueExtensions
  {
    private static ObservableQueue Instance = new ObservableQueue();

    public static IObservable<T> ObserveInQueue<T>(
      this IObservable<T> source,
      ObservableQueue queue = null)
    {
      return (queue ?? ObservableQueueExtensions.Instance).GetQueuedObservable<T>(source);
    }
  }
}
