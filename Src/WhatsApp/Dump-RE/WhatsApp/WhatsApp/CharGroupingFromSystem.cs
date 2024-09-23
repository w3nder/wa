// Decompiled with JetBrains decompiler
// Type: WhatsApp.CharGroupingFromSystem
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class CharGroupingFromSystem : CharGroupingImpl
  {
    private SortedLocaleGrouping slg;
    private List<string> groupingKeys;

    public CharGroupingFromSystem(CultureInfo info) => this.slg = new SortedLocaleGrouping(info);

    public List<string> GroupingKeys
    {
      get => this.groupingKeys ?? (this.groupingKeys = this.GroupingKeysReadOnly.ToList<string>());
    }

    private IReadOnlyList<string> GroupingKeysReadOnly => this.slg.GroupDisplayNames;

    public string GetGroupingKey(string str)
    {
      string str1 = (string) null;
      int groupIndex = this.slg.GetGroupIndex(str);
      if (groupIndex >= 0 && groupIndex < this.slg.GroupDisplayNames.Count)
      {
        string groupDisplayName = this.slg.GroupDisplayNames[groupIndex];
        if (groupDisplayName != "...")
          str1 = groupDisplayName;
      }
      return str1 ?? "\uD83C\uDF10";
    }
  }
}
