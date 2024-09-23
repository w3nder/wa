// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.CollectionWrapper`1
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal class CollectionWrapper<T> : 
    ICollection<T>,
    IEnumerable<T>,
    IWrappedCollection,
    IList,
    ICollection,
    IEnumerable
  {
    private readonly IList _list;
    private readonly ICollection<T> _genericCollection;
    private object _syncRoot;

    public CollectionWrapper(IList list)
    {
      ValidationUtils.ArgumentNotNull((object) list, nameof (list));
      if (list is ICollection<T>)
        this._genericCollection = (ICollection<T>) list;
      else
        this._list = list;
    }

    public CollectionWrapper(ICollection<T> list)
    {
      ValidationUtils.ArgumentNotNull((object) list, nameof (list));
      this._genericCollection = list;
    }

    public virtual void Add(T item)
    {
      if (this._genericCollection != null)
        this._genericCollection.Add(item);
      else
        this._list.Add((object) item);
    }

    public virtual void Clear()
    {
      if (this._genericCollection != null)
        this._genericCollection.Clear();
      else
        this._list.Clear();
    }

    public virtual bool Contains(T item)
    {
      return this._genericCollection != null ? this._genericCollection.Contains(item) : this._list.Contains((object) item);
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
      if (this._genericCollection != null)
        this._genericCollection.CopyTo(array, arrayIndex);
      else
        this._list.CopyTo((Array) array, arrayIndex);
    }

    public virtual int Count
    {
      get => this._genericCollection != null ? this._genericCollection.Count : this._list.Count;
    }

    public virtual bool IsReadOnly
    {
      get
      {
        return this._genericCollection != null ? this._genericCollection.IsReadOnly : this._list.IsReadOnly;
      }
    }

    public virtual bool Remove(T item)
    {
      if (this._genericCollection != null)
        return this._genericCollection.Remove(item);
      bool flag = this._list.Contains((object) item);
      if (flag)
        this._list.Remove((object) item);
      return flag;
    }

    public virtual IEnumerator<T> GetEnumerator()
    {
      return this._genericCollection != null ? this._genericCollection.GetEnumerator() : this._list.Cast<T>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this._genericCollection != null ? (IEnumerator) this._genericCollection.GetEnumerator() : this._list.GetEnumerator();
    }

    int IList.Add(object value)
    {
      CollectionWrapper<T>.VerifyValueType(value);
      this.Add((T) value);
      return this.Count - 1;
    }

    bool IList.Contains(object value)
    {
      return CollectionWrapper<T>.IsCompatibleObject(value) && this.Contains((T) value);
    }

    int IList.IndexOf(object value)
    {
      if (this._genericCollection != null)
        throw new InvalidOperationException("Wrapped ICollection<T> does not support IndexOf.");
      return CollectionWrapper<T>.IsCompatibleObject(value) ? this._list.IndexOf((object) (T) value) : -1;
    }

    void IList.RemoveAt(int index)
    {
      if (this._genericCollection != null)
        throw new InvalidOperationException("Wrapped ICollection<T> does not support RemoveAt.");
      this._list.RemoveAt(index);
    }

    void IList.Insert(int index, object value)
    {
      if (this._genericCollection != null)
        throw new InvalidOperationException("Wrapped ICollection<T> does not support Insert.");
      CollectionWrapper<T>.VerifyValueType(value);
      this._list.Insert(index, (object) (T) value);
    }

    bool IList.IsFixedSize
    {
      get
      {
        return this._genericCollection != null ? this._genericCollection.IsReadOnly : this._list.IsFixedSize;
      }
    }

    void IList.Remove(object value)
    {
      if (!CollectionWrapper<T>.IsCompatibleObject(value))
        return;
      this.Remove((T) value);
    }

    object IList.this[int index]
    {
      get
      {
        if (this._genericCollection != null)
          throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
        return this._list[index];
      }
      set
      {
        if (this._genericCollection != null)
          throw new InvalidOperationException("Wrapped ICollection<T> does not support indexer.");
        CollectionWrapper<T>.VerifyValueType(value);
        this._list[index] = (object) (T) value;
      }
    }

    void ICollection.CopyTo(Array array, int arrayIndex) => this.CopyTo((T[]) array, arrayIndex);

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot
    {
      get
      {
        if (this._syncRoot == null)
          Interlocked.CompareExchange(ref this._syncRoot, new object(), (object) null);
        return this._syncRoot;
      }
    }

    private static void VerifyValueType(object value)
    {
      if (!CollectionWrapper<T>.IsCompatibleObject(value))
        throw new ArgumentException("The value '{0}' is not of type '{1}' and cannot be used in this generic collection.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, value, (object) typeof (T)), nameof (value));
    }

    private static bool IsCompatibleObject(object value)
    {
      return value is T || value == null && (!typeof (T).IsValueType() || ReflectionUtils.IsNullableType(typeof (T)));
    }

    public object UnderlyingCollection
    {
      get
      {
        return this._genericCollection != null ? (object) this._genericCollection : (object) this._list;
      }
    }
  }
}
