// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaCollections.KeyedList`2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.WaCollections
{
  public class KeyedList<TKey, TItem> : List<TItem>
  {
    public TKey Key { protected set; get; }

    public KeyedList(TKey key, IEnumerable<TItem> items)
      : base(items)
    {
      this.Key = key;
    }

    public KeyedList(IGrouping<TKey, TItem> grouping)
      : base((IEnumerable<TItem>) grouping)
    {
      this.Key = grouping.Key;
    }
  }
}
