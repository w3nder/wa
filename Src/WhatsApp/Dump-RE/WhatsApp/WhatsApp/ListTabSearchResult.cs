// Decompiled with JetBrains decompiler
// Type: WhatsApp.ListTabSearchResult
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Collections.Generic;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ListTabSearchResult
  {
    protected List<JidItemViewModel> items;

    public string SearchTerm { get; private set; }

    public List<JidItemViewModel> Items => this.items ?? new List<JidItemViewModel>();

    public ListTabSearchResult(string searchTerm) => this.SearchTerm = searchTerm;

    public void Add(IEnumerable<JidItemViewModel> itemsToAdd)
    {
      if (itemsToAdd == null)
        return;
      if (this.items == null)
        this.items = new List<JidItemViewModel>(itemsToAdd);
      else
        this.items.AddRange(itemsToAdd);
    }
  }
}
