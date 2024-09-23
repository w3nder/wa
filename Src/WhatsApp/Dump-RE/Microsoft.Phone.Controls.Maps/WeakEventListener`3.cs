// Decompiled with JetBrains decompiler
// Type: WeakEventListener`3
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
[Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
{
  private readonly WeakReference _weakInstance;

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
