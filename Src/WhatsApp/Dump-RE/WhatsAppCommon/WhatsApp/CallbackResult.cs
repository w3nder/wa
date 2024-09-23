// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallbackResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public static class CallbackResult
  {
    public static ICallbackResult Create(Action onSuccess = null, Action<Exception> onError = null)
    {
      return (ICallbackResult) new CallbackResult.CallbackWrapper()
      {
        OnSuccessAction = (onSuccess ?? (Action) (() => { })),
        OnErrorAction = (onError ?? (Action<Exception>) (err => { }))
      };
    }

    private class CallbackWrapper : ICallbackResult
    {
      public Action OnSuccessAction;
      public Action<Exception> OnErrorAction;

      public void OnSuccess() => this.OnSuccessAction();

      public void OnError(Exception ex) => this.OnErrorAction(ex);
    }
  }
}
