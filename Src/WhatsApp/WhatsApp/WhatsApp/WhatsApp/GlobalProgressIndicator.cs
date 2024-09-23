// Decompiled with JetBrains decompiler
// Type: WhatsApp.GlobalProgressIndicator
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Shell;
using System;
using System.Windows;


namespace WhatsApp
{
  public class GlobalProgressIndicator
  {
    private int refCount;
    private ProgressIndicator progressIndicator = new ProgressIndicator();
    private DependencyObject dep;

    public bool IsAcquired => this.refCount > 0;

    public GlobalProgressIndicator(DependencyObject depObj)
    {
      this.dep = depObj;
      this.progressIndicator.IsIndeterminate = true;
      this.progressIndicator.IsVisible = true;
    }

    private void Invoke(Action a) => this.dep.Dispatcher.BeginInvokeIfNeeded(a);

    public void Acquire()
    {
      this.Invoke((Action) (() =>
      {
        if (this.refCount++ != 0)
          return;
        SystemTray.SetProgressIndicator(this.dep, this.progressIndicator);
      }));
    }

    public void Reacquire()
    {
      this.Invoke((Action) (() =>
      {
        if (this.refCount > 0)
          SystemTray.SetProgressIndicator(this.dep, this.progressIndicator);
        else
          SystemTray.SetProgressIndicator(this.dep, (ProgressIndicator) null);
      }));
    }

    public void Release()
    {
      if (this.refCount <= 0)
        return;
      this.Invoke((Action) (() =>
      {
        if (--this.refCount < 0)
          this.refCount = 0;
        if (this.refCount != 0)
          return;
        SystemTray.SetProgressIndicator(this.dep, (ProgressIndicator) null);
      }));
    }

    public void ReleaseAll()
    {
      this.Invoke((Action) (() =>
      {
        this.refCount = 0;
        SystemTray.SetProgressIndicator(this.dep, (ProgressIndicator) null);
      }));
    }
  }
}
