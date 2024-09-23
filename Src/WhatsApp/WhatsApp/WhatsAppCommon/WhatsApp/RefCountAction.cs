// Decompiled with JetBrains decompiler
// Type: WhatsApp.RefCountAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class RefCountAction
  {
    private Action onAcquired_;
    private Action onReleased_;
    private object syncLock_ = new object();
    private int refCount_;

    public bool InEffect => this.refCount_ > 0;

    public RefCountAction(Action onAcquired, Action onReleased)
    {
      this.onAcquired_ = onAcquired;
      this.onReleased_ = onReleased;
    }

    public static RefCountAction Replace(ref Action dispose)
    {
      RefCountAction refCountAction = new RefCountAction((Action) (() => { }), dispose);
      dispose = new Action(refCountAction.Subscribe().Dispose);
      return refCountAction;
    }

    public void Synchronize(Action a)
    {
      lock (this.syncLock_)
        a();
    }

    public IDisposable Subscribe()
    {
      lock (this.syncLock_)
      {
        if (this.refCount_++ == 0)
        {
          if (this.onAcquired_ != null)
            this.onAcquired_();
        }
      }
      return (IDisposable) new DisposableAction((Action) (() =>
      {
        lock (this.syncLock_)
        {
          if (--this.refCount_ != 0 || this.onReleased_ == null)
            return;
          this.onReleased_();
        }
      }));
    }
  }
}
