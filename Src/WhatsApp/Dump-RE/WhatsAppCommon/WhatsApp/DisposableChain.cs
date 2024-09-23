// Decompiled with JetBrains decompiler
// Type: WhatsApp.DisposableChain
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public class DisposableChain : WaDisposable
  {
    private List<IDisposable> disposables;

    public DisposableChain(params IDisposable[] disposables)
    {
      this.disposables = new List<IDisposable>((IEnumerable<IDisposable>) disposables);
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.disposables == null)
        return;
      IDisposable[] array = this.disposables.ToArray();
      this.disposables.Clear();
      foreach (IDisposable d in array)
        d.SafeDispose();
    }
  }
}
