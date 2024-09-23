// Decompiled with JetBrains decompiler
// Type: System.Collections.Concurrent.ConcurrentDictionary`2
// Assembly: Portable.ConcurrentDictionary, Version=1.0.2.0, Culture=neutral, PublicKeyToken=null
// MVID: DB56BACC-BDC4-4C60-BF1D-8E1E2F27714A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Portable.ConcurrentDictionary.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable disable
namespace System.Collections.Concurrent
{
  [DebuggerTypeProxy(typeof (IDictionaryDebugView<,>))]
  [DebuggerDisplay("Count = {Count}")]
  public class ConcurrentDictionary<TKey, TValue> : 
    IDictionary<TKey, TValue>,
    ICollection<KeyValuePair<TKey, TValue>>,
    IEnumerable<KeyValuePair<TKey, TValue>>,
    IEnumerable,
    IDictionary,
    ICollection,
    IReadOnlyDictionary<TKey, TValue>,
    IReadOnlyCollection<KeyValuePair<TKey, TValue>>
  {
    private volatile ConcurrentDictionary<TKey, TValue>.Tables _tables;
    private readonly IEqualityComparer<TKey> _comparer;
    private readonly bool _growLockArray;
    private int _budget;
    private const int DefaultCapacity = 31;
    private const int MaxLockNumber = 1024;
    private static readonly bool s_isValueWriteAtomic = ConcurrentDictionary<TKey, TValue>.IsValueWriteAtomic();

    private static bool IsValueWriteAtomic()
    {
      Type type = typeof (TValue);
      bool flag = !type.GetTypeInfo().IsValueType || type == typeof (bool) || type == typeof (char) || type == typeof (byte) || type == typeof (sbyte) || type == typeof (short) || type == typeof (ushort) || type == typeof (int) || type == typeof (uint) || type == typeof (float);
      if (!flag && IntPtr.Size == 8)
        flag = type == typeof (double) || type == typeof (long) || type == typeof (ulong);
      return flag;
    }

    public ConcurrentDictionary()
      : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrentDictionary(int concurrencyLevel, int capacity)
      : this(concurrencyLevel, capacity, false, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
      : this(collection, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default)
    {
    }

    public ConcurrentDictionary(IEqualityComparer<TKey> comparer)
      : this(ConcurrentDictionary<TKey, TValue>.DefaultConcurrencyLevel, 31, true, comparer)
    {
    }

    public ConcurrentDictionary(
      IEnumerable<KeyValuePair<TKey, TValue>> collection,
      IEqualityComparer<TKey> comparer)
      : this(comparer)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      this.InitializeFromCollection(collection);
    }

    public ConcurrentDictionary(
      int concurrencyLevel,
      IEnumerable<KeyValuePair<TKey, TValue>> collection,
      IEqualityComparer<TKey> comparer)
      : this(concurrencyLevel, 31, false, comparer)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (comparer == null)
        throw new ArgumentNullException(nameof (comparer));
      this.InitializeFromCollection(collection);
    }

    private void InitializeFromCollection(IEnumerable<KeyValuePair<TKey, TValue>> collection)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in collection)
      {
        if ((object) keyValuePair.Key == null)
          ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
        if (!this.TryAddInternal(keyValuePair.Key, this._comparer.GetHashCode(keyValuePair.Key), keyValuePair.Value, false, false, out TValue _))
          throw new ArgumentException(SR.ConcurrentDictionary_SourceContainsDuplicateKeys);
      }
      if (this._budget != 0)
        return;
      this._budget = this._tables._buckets.Length / this._tables._locks.Length;
    }

    public ConcurrentDictionary(
      int concurrencyLevel,
      int capacity,
      IEqualityComparer<TKey> comparer)
      : this(concurrencyLevel, capacity, false, comparer)
    {
    }

    internal ConcurrentDictionary(
      int concurrencyLevel,
      int capacity,
      bool growLockArray,
      IEqualityComparer<TKey> comparer)
    {
      if (concurrencyLevel < 1)
        throw new ArgumentOutOfRangeException(nameof (concurrencyLevel), SR.ConcurrentDictionary_ConcurrencyLevelMustBePositive);
      if (capacity < 0)
        throw new ArgumentOutOfRangeException(nameof (capacity), SR.ConcurrentDictionary_CapacityMustNotBeNegative);
      if (comparer == null)
        throw new ArgumentNullException(nameof (comparer));
      if (capacity < concurrencyLevel)
        capacity = concurrencyLevel;
      object[] locks = new object[concurrencyLevel];
      for (int index = 0; index < locks.Length; ++index)
        locks[index] = new object();
      int[] countPerLock = new int[locks.Length];
      ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[capacity];
      this._tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, locks, countPerLock);
      this._comparer = comparer;
      this._growLockArray = growLockArray;
      this._budget = buckets.Length / locks.Length;
    }

    public bool TryAdd(TKey key, TValue value)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return this.TryAddInternal(key, this._comparer.GetHashCode(key), value, false, true, out TValue _);
    }

    public bool ContainsKey(TKey key)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return this.TryGetValue(key, out TValue _);
    }

    public bool TryRemove(TKey key, out TValue value)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return this.TryRemoveInternal(key, out value, false, default (TValue));
    }

    private bool TryRemoveInternal(TKey key, out TValue value, bool matchValue, TValue oldValue)
    {
      int hashCode = this._comparer.GetHashCode(key);
label_1:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this._tables;
      int bucketNo;
      int lockNo;
      ConcurrentDictionary<TKey, TValue>.GetBucketAndLockNo(hashCode, out bucketNo, out lockNo, tables._buckets.Length, tables._locks.Length);
      lock (tables._locks[lockNo])
      {
        if (tables == this._tables)
        {
          ConcurrentDictionary<TKey, TValue>.Node node1 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node node2 = tables._buckets[bucketNo]; node2 != null; node2 = node2._next)
          {
            if (hashCode == node2._hashcode && this._comparer.Equals(node2._key, key))
            {
              if (matchValue && !EqualityComparer<TValue>.Default.Equals(oldValue, node2._value))
              {
                value = default (TValue);
                return false;
              }
              if (node1 == null)
                Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables._buckets[bucketNo], node2._next);
              else
                node1._next = node2._next;
              value = node2._value;
              --tables._countPerLock[lockNo];
              return true;
            }
            node1 = node2;
          }
        }
        else
          goto label_1;
      }
      value = default (TValue);
      return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return this.TryGetValueInternal(key, this._comparer.GetHashCode(key), out value);
    }

    private bool TryGetValueInternal(TKey key, int hashcode, out TValue value)
    {
      ConcurrentDictionary<TKey, TValue>.Tables tables = this._tables;
      int bucket = ConcurrentDictionary<TKey, TValue>.GetBucket(hashcode, tables._buckets.Length);
      for (ConcurrentDictionary<TKey, TValue>.Node node = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref tables._buckets[bucket]); node != null; node = node._next)
      {
        if (hashcode == node._hashcode && this._comparer.Equals(node._key, key))
        {
          value = node._value;
          return true;
        }
      }
      value = default (TValue);
      return false;
    }

    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return this.TryUpdateInternal(key, this._comparer.GetHashCode(key), newValue, comparisonValue);
    }

    private bool TryUpdateInternal(
      TKey key,
      int hashcode,
      TValue newValue,
      TValue comparisonValue)
    {
      IEqualityComparer<TValue> equalityComparer = (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default;
label_1:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this._tables;
      int bucketNo;
      int lockNo;
      ConcurrentDictionary<TKey, TValue>.GetBucketAndLockNo(hashcode, out bucketNo, out lockNo, tables._buckets.Length, tables._locks.Length);
      lock (tables._locks[lockNo])
      {
        if (tables == this._tables)
        {
          ConcurrentDictionary<TKey, TValue>.Node node1 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node node2 = tables._buckets[bucketNo]; node2 != null; node2 = node2._next)
          {
            if (hashcode == node2._hashcode && this._comparer.Equals(node2._key, key))
            {
              if (!equalityComparer.Equals(node2._value, comparisonValue))
                return false;
              if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
              {
                node2._value = newValue;
              }
              else
              {
                ConcurrentDictionary<TKey, TValue>.Node node3 = new ConcurrentDictionary<TKey, TValue>.Node(node2._key, newValue, hashcode, node2._next);
                if (node1 == null)
                  tables._buckets[bucketNo] = node3;
                else
                  node1._next = node3;
              }
              return true;
            }
            node1 = node2;
          }
          return false;
        }
        goto label_1;
      }
    }

    public void Clear()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        ConcurrentDictionary<TKey, TValue>.Tables tables = new ConcurrentDictionary<TKey, TValue>.Tables(new ConcurrentDictionary<TKey, TValue>.Node[31], this._tables._locks, new int[this._tables._countPerLock.Length]);
        this._tables = tables;
        this._budget = Math.Max(1, tables._buckets.Length / tables._locks.Length);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(
      KeyValuePair<TKey, TValue>[] array,
      int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), SR.ConcurrentDictionary_IndexIsNegative);
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        int num = 0;
        for (int index1 = 0; index1 < this._tables._locks.Length && num >= 0; ++index1)
          num += this._tables._countPerLock[index1];
        if (array.Length - num < index || num < 0)
          throw new ArgumentException(SR.ConcurrentDictionary_ArrayNotLargeEnough);
        this.CopyToPairs(array, index);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    public KeyValuePair<TKey, TValue>[] ToArray()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        int length = 0;
        int index = 0;
        while (index < this._tables._locks.Length)
        {
          checked { length += this._tables._countPerLock[index]; }
          checked { ++index; }
        }
        if (length == 0)
          return ArrayHelper.Empty<TKey, TValue>();
        KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[length];
        this.CopyToPairs(array, 0);
        return array;
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private void CopyToPairs(KeyValuePair<TKey, TValue>[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node bucket in this._tables._buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node = bucket; node != null; node = node._next)
        {
          array[index] = new KeyValuePair<TKey, TValue>(node._key, node._value);
          ++index;
        }
      }
    }

    private void CopyToEntries(DictionaryEntry[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node bucket in this._tables._buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node = bucket; node != null; node = node._next)
        {
          array[index] = new DictionaryEntry((object) node._key, (object) node._value);
          ++index;
        }
      }
    }

    private void CopyToObjects(object[] array, int index)
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node bucket in this._tables._buckets)
      {
        for (ConcurrentDictionary<TKey, TValue>.Node node = bucket; node != null; node = node._next)
        {
          array[index] = (object) new KeyValuePair<TKey, TValue>(node._key, node._value);
          ++index;
        }
      }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      foreach (ConcurrentDictionary<TKey, TValue>.Node bucket in this._tables._buckets)
      {
        ConcurrentDictionary<TKey, TValue>.Node current;
        for (current = Volatile.Read<ConcurrentDictionary<TKey, TValue>.Node>(ref bucket); current != null; current = current._next)
          yield return new KeyValuePair<TKey, TValue>(current._key, current._value);
        current = (ConcurrentDictionary<TKey, TValue>.Node) null;
      }
    }

    private bool TryAddInternal(
      TKey key,
      int hashcode,
      TValue value,
      bool updateIfExists,
      bool acquireLock,
      out TValue resultingValue)
    {
label_0:
      ConcurrentDictionary<TKey, TValue>.Tables tables = this._tables;
      int bucketNo;
      int lockNo;
      ConcurrentDictionary<TKey, TValue>.GetBucketAndLockNo(hashcode, out bucketNo, out lockNo, tables._buckets.Length, tables._locks.Length);
      bool flag = false;
      bool lockTaken = false;
      try
      {
        if (acquireLock)
          Monitor.Enter(tables._locks[lockNo], ref lockTaken);
        if (tables == this._tables)
        {
          ConcurrentDictionary<TKey, TValue>.Node node1 = (ConcurrentDictionary<TKey, TValue>.Node) null;
          for (ConcurrentDictionary<TKey, TValue>.Node node2 = tables._buckets[bucketNo]; node2 != null; node2 = node2._next)
          {
            if (hashcode == node2._hashcode && this._comparer.Equals(node2._key, key))
            {
              if (updateIfExists)
              {
                if (ConcurrentDictionary<TKey, TValue>.s_isValueWriteAtomic)
                {
                  node2._value = value;
                }
                else
                {
                  ConcurrentDictionary<TKey, TValue>.Node node3 = new ConcurrentDictionary<TKey, TValue>.Node(node2._key, value, hashcode, node2._next);
                  if (node1 == null)
                    tables._buckets[bucketNo] = node3;
                  else
                    node1._next = node3;
                }
                resultingValue = value;
              }
              else
                resultingValue = node2._value;
              return false;
            }
            node1 = node2;
          }
          Volatile.Write<ConcurrentDictionary<TKey, TValue>.Node>(ref tables._buckets[bucketNo], new ConcurrentDictionary<TKey, TValue>.Node(key, value, hashcode, tables._buckets[bucketNo]));
          checked { ++tables._countPerLock[lockNo]; }
          if (tables._countPerLock[lockNo] > this._budget)
            flag = true;
        }
        else
          goto label_0;
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(tables._locks[lockNo]);
      }
      if (flag)
        this.GrowTable(tables);
      resultingValue = value;
      return true;
    }

    public TValue this[TKey key]
    {
      get
      {
        TValue obj;
        if (!this.TryGetValue(key, out obj))
          ConcurrentDictionary<TKey, TValue>.ThrowKeyNotFoundException();
        return obj;
      }
      set
      {
        if ((object) key == null)
          ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
        this.TryAddInternal(key, this._comparer.GetHashCode(key), value, true, true, out TValue _);
      }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowKeyNotFoundException() => throw new KeyNotFoundException();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowKeyNullException() => throw new ArgumentNullException("key");

    public int Count
    {
      get
      {
        int count = 0;
        int locksAcquired = 0;
        try
        {
          this.AcquireAllLocks(ref locksAcquired);
          for (int index = 0; index < this._tables._countPerLock.Length; ++index)
            count += this._tables._countPerLock[index];
        }
        finally
        {
          this.ReleaseLocks(0, locksAcquired);
        }
        return count;
      }
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      if (valueFactory == null)
        throw new ArgumentNullException(nameof (valueFactory));
      int hashCode = this._comparer.GetHashCode(key);
      TValue resultingValue;
      if (!this.TryGetValueInternal(key, hashCode, out resultingValue))
        this.TryAddInternal(key, hashCode, valueFactory(key), false, true, out resultingValue);
      return resultingValue;
    }

    public TValue GetOrAdd(TKey key, TValue value)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      int hashCode = this._comparer.GetHashCode(key);
      TValue resultingValue;
      if (!this.TryGetValueInternal(key, hashCode, out resultingValue))
        this.TryAddInternal(key, hashCode, value, false, true, out resultingValue);
      return resultingValue;
    }

    public TValue AddOrUpdate(
      TKey key,
      Func<TKey, TValue> addValueFactory,
      Func<TKey, TValue, TValue> updateValueFactory)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      if (addValueFactory == null)
        throw new ArgumentNullException(nameof (addValueFactory));
      if (updateValueFactory == null)
        throw new ArgumentNullException(nameof (updateValueFactory));
      int hashCode = this._comparer.GetHashCode(key);
      TValue comparisonValue;
      TValue newValue;
      do
      {
        while (!this.TryGetValueInternal(key, hashCode, out comparisonValue))
        {
          TValue resultingValue;
          if (this.TryAddInternal(key, hashCode, addValueFactory(key), false, true, out resultingValue))
            return resultingValue;
        }
        newValue = updateValueFactory(key, comparisonValue);
      }
      while (!this.TryUpdateInternal(key, hashCode, newValue, comparisonValue));
      return newValue;
    }

    public TValue AddOrUpdate(
      TKey key,
      TValue addValue,
      Func<TKey, TValue, TValue> updateValueFactory)
    {
      if ((object) key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      if (updateValueFactory == null)
        throw new ArgumentNullException(nameof (updateValueFactory));
      int hashCode = this._comparer.GetHashCode(key);
      TValue comparisonValue;
      TValue newValue;
      do
      {
        while (!this.TryGetValueInternal(key, hashCode, out comparisonValue))
        {
          TValue resultingValue;
          if (this.TryAddInternal(key, hashCode, addValue, false, true, out resultingValue))
            return resultingValue;
        }
        newValue = updateValueFactory(key, comparisonValue);
      }
      while (!this.TryUpdateInternal(key, hashCode, newValue, comparisonValue));
      return newValue;
    }

    public bool IsEmpty
    {
      get
      {
        int locksAcquired = 0;
        try
        {
          this.AcquireAllLocks(ref locksAcquired);
          for (int index = 0; index < this._tables._countPerLock.Length; ++index)
          {
            if (this._tables._countPerLock[index] != 0)
              return false;
          }
        }
        finally
        {
          this.ReleaseLocks(0, locksAcquired);
        }
        return true;
      }
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
      if (!this.TryAdd(key, value))
        throw new ArgumentException(SR.ConcurrentDictionary_KeyAlreadyExisted);
    }

    bool IDictionary<TKey, TValue>.Remove(TKey key) => this.TryRemove(key, out TValue _);

    public ICollection<TKey> Keys => (ICollection<TKey>) this.GetKeys();

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => (IEnumerable<TKey>) this.GetKeys();

    public ICollection<TValue> Values => (ICollection<TValue>) this.GetValues();

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
    {
      get => (IEnumerable<TValue>) this.GetValues();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
    {
      ((IDictionary<TKey, TValue>) this).Add(keyValuePair.Key, keyValuePair.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
    {
      TValue x;
      return this.TryGetValue(keyValuePair.Key, out x) && EqualityComparer<TValue>.Default.Equals(x, keyValuePair.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
    {
      if ((object) keyValuePair.Key == null)
        throw new ArgumentNullException(SR.ConcurrentDictionary_ItemKeyIsNull);
      return this.TryRemoveInternal(keyValuePair.Key, out TValue _, true, keyValuePair.Value);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    void IDictionary.Add(object key, object value)
    {
      if (key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      if (!(key is TKey))
        throw new ArgumentException(SR.ConcurrentDictionary_TypeOfKeyIncorrect);
      TValue obj;
      try
      {
        obj = (TValue) value;
      }
      catch (InvalidCastException ex)
      {
        throw new ArgumentException(SR.ConcurrentDictionary_TypeOfValueIncorrect);
      }
      ((IDictionary<TKey, TValue>) this).Add((TKey) key, obj);
    }

    bool IDictionary.Contains(object key)
    {
      if (key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      return key is TKey key1 && this.ContainsKey(key1);
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
      return (IDictionaryEnumerator) new ConcurrentDictionary<TKey, TValue>.DictionaryEnumerator(this);
    }

    bool IDictionary.IsFixedSize => false;

    bool IDictionary.IsReadOnly => false;

    ICollection IDictionary.Keys => (ICollection) this.GetKeys();

    void IDictionary.Remove(object key)
    {
      if (key == null)
        ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
      if (!(key is TKey key1))
        return;
      this.TryRemove(key1, out TValue _);
    }

    ICollection IDictionary.Values => (ICollection) this.GetValues();

    object IDictionary.this[object key]
    {
      get
      {
        if (key == null)
          ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
        TValue obj;
        return key is TKey key1 && this.TryGetValue(key1, out obj) ? (object) obj : (object) null;
      }
      set
      {
        if (key == null)
          ConcurrentDictionary<TKey, TValue>.ThrowKeyNullException();
        if (!(key is TKey))
          throw new ArgumentException(SR.ConcurrentDictionary_TypeOfKeyIncorrect);
        this[(TKey) key] = value is TValue obj ? obj : throw new ArgumentException(SR.ConcurrentDictionary_TypeOfValueIncorrect);
      }
    }

    void ICollection.CopyTo(Array array, int index)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), SR.ConcurrentDictionary_IndexIsNegative);
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        ConcurrentDictionary<TKey, TValue>.Tables tables = this._tables;
        int num = 0;
        for (int index1 = 0; index1 < tables._locks.Length && num >= 0; ++index1)
          num += tables._countPerLock[index1];
        if (array.Length - num < index || num < 0)
          throw new ArgumentException(SR.ConcurrentDictionary_ArrayNotLargeEnough);
        switch (array)
        {
          case KeyValuePair<TKey, TValue>[] array1:
            this.CopyToPairs(array1, index);
            break;
          case DictionaryEntry[] array2:
            this.CopyToEntries(array2, index);
            break;
          case object[] array3:
            this.CopyToObjects(array3, index);
            break;
          default:
            throw new ArgumentException(SR.ConcurrentDictionary_ArrayIncorrectType, nameof (array));
        }
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot
    {
      get => throw new NotSupportedException(SR.ConcurrentCollection_SyncRoot_NotSupported);
    }

    private void GrowTable(ConcurrentDictionary<TKey, TValue>.Tables tables)
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireLocks(0, 1, ref locksAcquired);
        if (tables != this._tables)
          return;
        long num = 0;
        for (int index = 0; index < tables._countPerLock.Length; ++index)
          num += (long) tables._countPerLock[index];
        if (num < (long) (tables._buckets.Length / 4))
        {
          this._budget = 2 * this._budget;
          if (this._budget >= 0)
            return;
          this._budget = int.MaxValue;
        }
        else
        {
          int length1 = 0;
          bool flag = false;
          try
          {
            length1 = checked (tables._buckets.Length * 2 + 1);
            while (length1 % 3 == 0 || length1 % 5 == 0 || length1 % 7 == 0)
              checked { length1 += 2; }
            if (length1 > 2146435071)
              flag = true;
          }
          catch (OverflowException ex)
          {
            flag = true;
          }
          if (flag)
          {
            length1 = 2146435071;
            this._budget = int.MaxValue;
          }
          this.AcquireLocks(1, tables._locks.Length, ref locksAcquired);
          object[] objArray = tables._locks;
          if (this._growLockArray && tables._locks.Length < 1024)
          {
            objArray = new object[tables._locks.Length * 2];
            Array.Copy((Array) tables._locks, 0, (Array) objArray, 0, tables._locks.Length);
            for (int length2 = tables._locks.Length; length2 < objArray.Length; ++length2)
              objArray[length2] = new object();
          }
          ConcurrentDictionary<TKey, TValue>.Node[] buckets = new ConcurrentDictionary<TKey, TValue>.Node[length1];
          int[] countPerLock = new int[objArray.Length];
          ConcurrentDictionary<TKey, TValue>.Node next;
          for (int index = 0; index < tables._buckets.Length; ++index)
          {
            for (ConcurrentDictionary<TKey, TValue>.Node node = tables._buckets[index]; node != null; node = next)
            {
              next = node._next;
              int bucketNo;
              int lockNo;
              ConcurrentDictionary<TKey, TValue>.GetBucketAndLockNo(node._hashcode, out bucketNo, out lockNo, buckets.Length, objArray.Length);
              buckets[bucketNo] = new ConcurrentDictionary<TKey, TValue>.Node(node._key, node._value, node._hashcode, buckets[bucketNo]);
              checked { ++countPerLock[lockNo]; }
            }
          }
          this._budget = Math.Max(1, buckets.Length / objArray.Length);
          this._tables = new ConcurrentDictionary<TKey, TValue>.Tables(buckets, objArray, countPerLock);
        }
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private static int GetBucket(int hashcode, int bucketCount)
    {
      return (hashcode & int.MaxValue) % bucketCount;
    }

    private static void GetBucketAndLockNo(
      int hashcode,
      out int bucketNo,
      out int lockNo,
      int bucketCount,
      int lockCount)
    {
      bucketNo = (hashcode & int.MaxValue) % bucketCount;
      lockNo = bucketNo % lockCount;
    }

    private static int DefaultConcurrencyLevel => PlatformHelper.ProcessorCount;

    private void AcquireAllLocks(ref int locksAcquired)
    {
      this.AcquireLocks(0, 1, ref locksAcquired);
      this.AcquireLocks(1, this._tables._locks.Length, ref locksAcquired);
    }

    private void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
    {
      object[] locks = this._tables._locks;
      for (int index = fromInclusive; index < toExclusive; ++index)
      {
        bool lockTaken = false;
        try
        {
          Monitor.Enter(locks[index], ref lockTaken);
        }
        finally
        {
          if (lockTaken)
            ++locksAcquired;
        }
      }
    }

    private void ReleaseLocks(int fromInclusive, int toExclusive)
    {
      for (int index = fromInclusive; index < toExclusive; ++index)
        Monitor.Exit(this._tables._locks[index]);
    }

    private ReadOnlyCollection<TKey> GetKeys()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        List<TKey> list = new List<TKey>();
        for (int index = 0; index < this._tables._buckets.Length; ++index)
        {
          for (ConcurrentDictionary<TKey, TValue>.Node node = this._tables._buckets[index]; node != null; node = node._next)
            list.Add(node._key);
        }
        return new ReadOnlyCollection<TKey>((IList<TKey>) list);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private ReadOnlyCollection<TValue> GetValues()
    {
      int locksAcquired = 0;
      try
      {
        this.AcquireAllLocks(ref locksAcquired);
        List<TValue> list = new List<TValue>();
        for (int index = 0; index < this._tables._buckets.Length; ++index)
        {
          for (ConcurrentDictionary<TKey, TValue>.Node node = this._tables._buckets[index]; node != null; node = node._next)
            list.Add(node._value);
        }
        return new ReadOnlyCollection<TValue>((IList<TValue>) list);
      }
      finally
      {
        this.ReleaseLocks(0, locksAcquired);
      }
    }

    private sealed class Tables
    {
      internal readonly ConcurrentDictionary<TKey, TValue>.Node[] _buckets;
      internal readonly object[] _locks;
      internal volatile int[] _countPerLock;

      internal Tables(
        ConcurrentDictionary<TKey, TValue>.Node[] buckets,
        object[] locks,
        int[] countPerLock)
      {
        this._buckets = buckets;
        this._locks = locks;
        this._countPerLock = countPerLock;
      }
    }

    private sealed class Node
    {
      internal readonly TKey _key;
      internal TValue _value;
      internal volatile ConcurrentDictionary<TKey, TValue>.Node _next;
      internal readonly int _hashcode;

      internal Node(
        TKey key,
        TValue value,
        int hashcode,
        ConcurrentDictionary<TKey, TValue>.Node next)
      {
        this._key = key;
        this._value = value;
        this._next = next;
        this._hashcode = hashcode;
      }
    }

    private sealed class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
    {
      private IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

      internal DictionaryEnumerator(ConcurrentDictionary<TKey, TValue> dictionary)
      {
        this._enumerator = dictionary.GetEnumerator();
      }

      public DictionaryEntry Entry
      {
        get
        {
          KeyValuePair<TKey, TValue> current = this._enumerator.Current;
          __Boxed<TKey> key = (object) current.Key;
          current = this._enumerator.Current;
          __Boxed<TValue> local = (object) current.Value;
          return new DictionaryEntry((object) key, (object) local);
        }
      }

      public object Key => (object) this._enumerator.Current.Key;

      public object Value => (object) this._enumerator.Current.Value;

      public object Current => (object) this.Entry;

      public bool MoveNext() => this._enumerator.MoveNext();

      public void Reset() => this._enumerator.Reset();
    }
  }
}
