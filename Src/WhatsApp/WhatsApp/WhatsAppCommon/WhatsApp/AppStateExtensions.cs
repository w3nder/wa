// Decompiled with JetBrains decompiler
// Type: WhatsApp.AppStateExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;


namespace WhatsApp
{
  public static class AppStateExtensions
  {
    public static IObservable<T> ObserveUntilLeavingFg<T>(this IObservable<T> source)
    {
      return AppState.IsBackgroundAgent ? source : Observable.Create<T>((Func<IObserver<T>, Action>) (observer =>
      {
        IDisposable srcSub = (IDisposable) null;
        IDisposable leavingSub = (IDisposable) null;
        object @lock = new object();
        bool cancelled = false;
        Action release = (Action) (() =>
        {
          lock (@lock)
          {
            if (srcSub != null)
            {
              srcSub.Dispose();
              srcSub = (IDisposable) null;
            }
            if (leavingSub != null)
            {
              leavingSub.Dispose();
              leavingSub = (IDisposable) null;
            }
            cancelled = true;
            observer.OnCompleted();
          }
        });
        srcSub = source.Do<T>(observer).Do<T>((Action<T>) (_ => { }), (Action<Exception>) (ex => release()), (Action) (() => release())).Subscribe<T>();
        leavingSub = AppState.PerformWhenLeavingFg(release);
        if (cancelled)
          release();
        return release;
      }));
    }
  }
}
