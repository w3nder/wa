// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.WeakEventListener`3
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
  {
    private WeakReference _weakInstance;

    public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

    public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

    public WeakEventListener(TInstance instance)
    {
      this._weakInstance = (object) instance != null ? new WeakReference((object) instance) : throw new ArgumentNullException(nameof (instance));
    }

    public void OnEvent(TSource source, TEventArgs eventArgs)
    {
      TInstance target = (TInstance) this._weakInstance.Target;
      if ((object) target != null)
      {
        if (this.OnEventAction == null)
          return;
        this.OnEventAction(target, source, eventArgs);
      }
      else
        this.Detach();
    }

    public void Detach()
    {
      if (this.OnDetachAction == null)
        return;
      this.OnDetachAction(this);
      this.OnDetachAction = (Action<WeakEventListener<TInstance, TSource, TEventArgs>>) null;
    }
  }
}
