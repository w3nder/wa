// Decompiled with JetBrains decompiler
// Type: WhatsApp.SortedLocaleGrouping
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using System.Globalization;


namespace WhatsApp
{
  public class SortedLocaleGrouping
  {
    private Microsoft.Phone.Globalization.SortedLocaleGrouping slg;

    public SortedLocaleGrouping(CultureInfo info) => this.slg = new Microsoft.Phone.Globalization.SortedLocaleGrouping(info);

    public int GetGroupIndex(string str) => this.slg.GetGroupIndex(str);

    public IReadOnlyList<string> GroupDisplayNames
    {
      get => (IReadOnlyList<string>) this.slg.GroupDisplayNames;
    }

    public bool SupportsPhonetics => this.slg.SupportsPhonetics;
  }
}
