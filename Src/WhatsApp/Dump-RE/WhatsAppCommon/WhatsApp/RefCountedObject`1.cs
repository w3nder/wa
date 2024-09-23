// Decompiled with JetBrains decompiler
// Type: WhatsApp.RefCountedObject`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class RefCountedObject<T> where T : class
  {
    private Action<T> dispose;
    private RefCountedObject<T>.Data value;
    private object @lock = new object();

    public RefCountedObject(Action<T> dispose) => this.dispose = dispose;

    public IDisposable Get(out T t)
    {
      RefCountedObject<T>.Data snap = (RefCountedObject<T>.Data) null;
      lock (this.@lock)
      {
        if ((snap = this.value) != null)
          ++snap.RefCount;
      }
      if (snap != null)
      {
        t = snap.Pointer;
        return (IDisposable) new DisposableAction((Action) (() => this.Release(snap)));
      }
      t = default (T);
      return (IDisposable) null;
    }

    public void Get(Action<T> onT)
    {
      RefCountedObject<T>.Data snap = (RefCountedObject<T>.Data) null;
      lock (this.@lock)
      {
        if ((snap = this.value) != null)
          ++snap.RefCount;
      }
      if (snap == null)
        return;
      onT(snap.Pointer);
      this.Release(snap);
    }

    public void Set(T data)
    {
      RefCountedObject<T>.Data snap = (RefCountedObject<T>.Data) null;
      RefCountedObject<T>.Data data1;
      if ((object) data != null)
        data1 = new RefCountedObject<T>.Data()
        {
          Pointer = data,
          RefCount = 1
        };
      else
        data1 = (RefCountedObject<T>.Data) null;
      RefCountedObject<T>.Data data2 = data1;
      lock (this.@lock)
      {
        snap = this.value;
        this.value = data2;
      }
      if (snap == null)
        return;
      this.Release(snap);
    }

    private void Release(RefCountedObject<T>.Data snap)
    {
      bool flag = false;
      lock (this.@lock)
      {
        if (--snap.RefCount == 0)
        {
          flag = true;
          if (this.value == snap)
            this.value = (RefCountedObject<T>.Data) null;
        }
      }
      if (!flag)
        return;
      T pointer = snap.Pointer;
      snap.Pointer = default (T);
      this.dispose(pointer);
      T obj = default (T);
    }

    private class Data
    {
      public int RefCount;
      public T Pointer;
    }
  }
}
