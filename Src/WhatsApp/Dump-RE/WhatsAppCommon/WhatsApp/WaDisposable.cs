// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaDisposable
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public abstract class WaDisposable : IDisposable
  {
    private bool isDisposed;

    public bool IsDisposed => this.isDisposed;

    ~WaDisposable() => this.DisposeImpl(false);

    public void Dispose()
    {
      this.DisposeImpl(true);
      GC.SuppressFinalize((object) this);
    }

    protected void DisposeImpl(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
        this.DisposeManagedResources();
      this.DisposeUnmanagedResources();
      this.isDisposed = true;
    }

    protected virtual void DisposeManagedResources()
    {
    }

    protected virtual void DisposeUnmanagedResources()
    {
    }
  }
}
