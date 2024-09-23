// Decompiled with JetBrains decompiler
// Type: System.Collections.Concurrent.IDictionaryDebugView`2
// Assembly: Portable.ConcurrentDictionary, Version=1.0.2.0, Culture=neutral, PublicKeyToken=null
// MVID: DB56BACC-BDC4-4C60-BF1D-8E1E2F27714A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Portable.ConcurrentDictionary.dll

using System.Collections.Generic;
using System.Diagnostics;

#nullable disable
namespace System.Collections.Concurrent
{
  internal sealed class IDictionaryDebugView<K, V>
  {
    private readonly IDictionary<K, V> _dictionary;

    public IDictionaryDebugView(IDictionary<K, V> dictionary)
    {
      this._dictionary = dictionary != null ? dictionary : throw new ArgumentNullException(nameof (dictionary));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public KeyValuePair<K, V>[] Items
    {
      get
      {
        KeyValuePair<K, V>[] array = new KeyValuePair<K, V>[this._dictionary.Count];
        this._dictionary.CopyTo(array, 0);
        return array;
      }
    }
  }
}
