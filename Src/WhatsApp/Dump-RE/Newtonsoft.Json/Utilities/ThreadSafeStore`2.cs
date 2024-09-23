// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.ThreadSafeStore`2
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections.Generic;
using System.Threading;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class ThreadSafeStore<TKey, TValue>
  {
    private readonly object _lock = new object();
    private Dictionary<TKey, TValue> _store;
    private readonly Func<TKey, TValue> _creator;

    public ThreadSafeStore(Func<TKey, TValue> creator)
    {
      this._creator = creator != null ? creator : throw new ArgumentNullException(nameof (creator));
      this._store = new Dictionary<TKey, TValue>();
    }

    public TValue Get(TKey key)
    {
      TValue obj;
      return !this._store.TryGetValue(key, out obj) ? this.AddValue(key) : obj;
    }

    private TValue AddValue(TKey key)
    {
      TValue obj1 = this._creator(key);
      lock (this._lock)
      {
        if (this._store == null)
        {
          this._store = new Dictionary<TKey, TValue>();
          this._store[key] = obj1;
        }
        else
        {
          TValue obj2;
          if (this._store.TryGetValue(key, out obj2))
            return obj2;
          Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>((IDictionary<TKey, TValue>) this._store);
          dictionary[key] = obj1;
          Thread.MemoryBarrier();
          this._store = dictionary;
        }
        return obj1;
      }
    }
  }
}
