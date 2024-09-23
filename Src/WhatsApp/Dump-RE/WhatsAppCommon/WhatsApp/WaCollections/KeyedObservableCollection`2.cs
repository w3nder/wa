// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.KeyedObservableCollection`2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#nullable disable
namespace WhatsApp.WaCollections
{
  public class KeyedObservableCollection<TKey, TItem> : ObservableCollection<TItem>
  {
    public TKey Key { protected set; get; }

    public KeyedObservableCollection(TKey key) => this.Key = key;

    public KeyedObservableCollection(TKey key, IEnumerable<TItem> items)
      : base(items)
    {
      this.Key = key;
    }

    public KeyedObservableCollection(IGrouping<TKey, TItem> grouping)
      : base((IEnumerable<TItem>) grouping)
    {
      this.Key = grouping.Key;
    }
  }
}
