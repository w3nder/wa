// Decompiled with JetBrains decompiler
// Type: WhatsApp.ListTabData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ListTabData
  {
    public string Header { get; set; }

    public DataTemplate ItemTemplate { get; protected set; }

    public virtual FrameworkElement CreateItem() => (FrameworkElement) null;

    public IObservable<JidItemViewModel[]> ItemsObservable { get; protected set; }

    public Func<string, IObservable<ListTabSearchResult>> SearchFunc { get; set; }

    public ListTabData()
    {
    }

    public ListTabData(
      string header,
      DataTemplate itemTemplate,
      IObservable<JidItemViewModel[]> itemObs)
    {
      this.Header = header;
      this.ItemTemplate = itemTemplate;
      this.ItemsObservable = itemObs;
    }

    public virtual void OnAllLoaded(IEnumerable<JidItemViewModel> items)
    {
    }

    public virtual IObservable<ListTabSearchResult> GetSearchObservable(string searchTerm)
    {
      return this.SearchFunc != null ? this.SearchFunc(searchTerm) : Observable.Empty<ListTabSearchResult>();
    }
  }
}
