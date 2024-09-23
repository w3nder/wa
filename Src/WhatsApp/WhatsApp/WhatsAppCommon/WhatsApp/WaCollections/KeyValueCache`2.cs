// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.KeyValueCache`2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.WaCollections
{
  public class KeyValueCache<TK, TV>
  {
    private Dictionary<TK, Pair<TV, LinkedListNode<TK>>> data_ = new Dictionary<TK, Pair<TV, LinkedListNode<TK>>>();
    private LinkedList<TK> order_;
    private int maxCapacity_;
    private object accessLock_;
    private bool lru_;

    public KeyValueCache(int maxCapacity, bool lru, bool threadSafe = false)
    {
      this.accessLock_ = threadSafe ? new object() : (object) null;
      if (maxCapacity <= 0)
        return;
      this.maxCapacity_ = maxCapacity;
      this.lru_ = lru;
      this.order_ = new LinkedList<TK>();
    }

    public int Count
    {
      get
      {
        if (this.accessLock_ == null)
          return this.data_.Count;
        lock (this.accessLock_)
          return this.data_.Count;
      }
    }

    public void Set(KeyValuePair<TK, TV>[] pairs)
    {
      if (this.accessLock_ == null)
      {
        this.SetNolock(pairs);
      }
      else
      {
        lock (this.accessLock_)
          this.SetNolock(pairs);
      }
    }

    public void Set(TK key, TV val)
    {
      this.Set(new KeyValuePair<TK, TV>[1]
      {
        new KeyValuePair<TK, TV>(key, val)
      });
    }

    private void SetNolock(KeyValuePair<TK, TV>[] pairs)
    {
      if (this.maxCapacity_ > 0)
      {
        bool flag = false;
        foreach (KeyValuePair<TK, TV> pair1 in pairs)
        {
          Pair<TV, LinkedListNode<TK>> pair2 = (Pair<TV, LinkedListNode<TK>>) null;
          if (this.data_.TryGetValue(pair1.Key, out pair2))
          {
            if (pair2 != null)
              this.RemoveKeyFromOrderList(pair1.Key, pair2.Second);
          }
          else
            flag = true;
          LinkedListNode<TK> second = this.order_.AddLast(pair1.Key);
          this.data_[pair1.Key] = new Pair<TV, LinkedListNode<TK>>(pair1.Value, second);
        }
        if (!flag)
          return;
        while (this.data_.Count > this.maxCapacity_)
          this.RemoveFrontNolock(Math.Max(pairs.Length, (int) ((double) this.maxCapacity_ * 0.1)));
      }
      else
      {
        foreach (KeyValuePair<TK, TV> pair in pairs)
          this.data_[pair.Key] = new Pair<TV, LinkedListNode<TK>>(pair.Value, (LinkedListNode<TK>) null);
      }
    }

    public bool TryGet(TK key, out TV val)
    {
      if (this.accessLock_ == null)
        return this.TryGetNoLock(key, out val);
      lock (this.accessLock_)
        return this.TryGetNoLock(key, out val);
    }

    private bool TryGetNoLock(TK key, out TV val)
    {
      bool noLock = false;
      Pair<TV, LinkedListNode<TK>> pair = (Pair<TV, LinkedListNode<TK>>) null;
      if ((object) key != null && this.data_.TryGetValue(key, out pair) && pair != null)
      {
        noLock = true;
        val = pair.First;
        if (this.lru_)
        {
          this.RemoveKeyFromOrderList(key, pair.Second);
          pair.Second = this.order_.AddLast(key);
        }
      }
      else
        val = default (TV);
      return noLock;
    }

    public bool Remove(TK key)
    {
      if (this.accessLock_ == null)
        return this.RemoveNolock(key);
      lock (this.accessLock_)
        return this.RemoveNolock(key);
    }

    private bool RemoveNolock(TK key)
    {
      if (!this.data_.Remove(key))
        return false;
      this.RemoveKeyFromOrderList(key);
      return true;
    }

    private void RemoveKeyFromOrderList(TK key, LinkedListNode<TK> llNode = null)
    {
      bool flag = false;
      if (llNode != null)
      {
        try
        {
          this.order_.Remove(llNode);
          flag = true;
        }
        catch (Exception ex)
        {
        }
      }
      if (flag)
        return;
      this.order_.Remove(key);
    }

    public void Clear()
    {
      if (this.accessLock_ == null)
      {
        this.ClearImpl();
      }
      else
      {
        lock (this.accessLock_)
          this.ClearImpl();
      }
    }

    private void ClearImpl()
    {
      this.data_.Clear();
      this.order_.Clear();
    }

    private void RemoveFrontNolock(int n)
    {
      if (n < this.data_.Count)
      {
        foreach (TK key in this.order_.Take<TK>(n))
          this.data_.Remove(key);
        for (int index = 0; index < n; ++index)
          this.order_.RemoveFirst();
      }
      else
        this.ClearImpl();
    }
  }
}
