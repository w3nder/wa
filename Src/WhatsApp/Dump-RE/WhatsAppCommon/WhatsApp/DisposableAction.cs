// Decompiled with JetBrains decompiler
// Type: WhatsApp.DisposableAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Threading;

#nullable disable
namespace WhatsApp
{
  public class DisposableAction : IDisposable
  {
    private Action actionOnDispose;

    public DisposableAction(Action a) => this.actionOnDispose = a;

    public void Dispose()
    {
      Action action = Interlocked.Exchange<Action>(ref this.actionOnDispose, (Action) null);
      if (action == null)
        return;
      action();
    }
  }
}
