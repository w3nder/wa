// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.Set`1
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp.WaCollections
{
  public class Set<T> : IEnumerable<T>, IEnumerable
  {
    private object lock_;
    private Dictionary<T, object> data_ = new Dictionary<T, object>();

    public Set(bool threadSafe = false)
    {
      if (!threadSafe)
        return;
      this.lock_ = new object();
    }

    public Set(IEnumerable<T> items, bool threadSafe = false)
    {
      if (threadSafe)
        this.lock_ = new object();
      this.AddRange(items);
    }

    public void Add(T key)
    {
      if (this.lock_ == null)
      {
        this.data_[key] = (object) null;
      }
      else
      {
        lock (this.lock_)
          this.data_[key] = (object) null;
      }
    }

    public void AddRange(IEnumerable<T> items)
    {
      if (this.lock_ == null)
      {
        foreach (T key in items)
          this.data_[key] = (object) null;
      }
      else
      {
        lock (this.lock_)
        {
          foreach (T key in items)
            this.data_[key] = (object) null;
        }
      }
    }

    public void Remove(T key)
    {
      if (this.lock_ == null)
      {
        this.data_.Remove(key);
      }
      else
      {
        lock (this.lock_)
          this.data_.Remove(key);
      }
    }

    public void Clear()
    {
      if (this.lock_ == null)
      {
        this.data_.Clear();
      }
      else
      {
        lock (this.lock_)
          this.data_.Clear();
      }
    }

    public bool Contains(T key)
    {
      if (this.lock_ == null)
        return this.data_.ContainsKey(key);
      lock (this.lock_)
        return this.data_.ContainsKey(key);
    }

    public int Count
    {
      get
      {
        if (this.lock_ == null)
          return this.data_.Count;
        lock (this.lock_)
          return this.data_.Count;
      }
    }

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.data_.Keys.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.data_.Keys).GetEnumerator();
  }
}
