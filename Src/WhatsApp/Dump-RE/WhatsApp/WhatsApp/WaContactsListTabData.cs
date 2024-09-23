// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaContactsListTabData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class WaContactsListTabData : ListTabData
  {
    private UserViewModel[] cachedItems;
    private bool enableCache;

    public bool EnableCache
    {
      get => this.enableCache;
      set
      {
        this.enableCache = value;
        if (this.enableCache)
          return;
        this.cachedItems = (UserViewModel[]) null;
      }
    }

    public Func<JidItemViewModel, bool> ItemVisibleFilter { get; set; }

    public WaContactsListTabData()
    {
      this.Header = AppResources.ContactsHeader;
      this.ItemTemplate = XamlReader.Load("\r\n                <DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                                xmlns:local=\"clr-namespace:WhatsApp;assembly=WhatsApp\">\r\n                    <local:UserItemControl ViewModel=\"{Binding}\"\r\n                                            Margin=\"0,0,12,24\"/>\r\n                </DataTemplate>") as DataTemplate;
      this.ItemsObservable = Observable.Create<JidItemViewModel[]>((Func<IObserver<JidItemViewModel[]>, Action>) (observer =>
      {
        JidItemViewModel[] source;
        if (this.EnableCache && this.cachedItems != null)
        {
          source = (JidItemViewModel[]) this.cachedItems;
        }
        else
        {
          UserStatus[] waContacts = (UserStatus[]) null;
          ContactsContext.Instance((Action<ContactsContext>) (db => waContacts = this.LoadFromDb(db)));
          source = (JidItemViewModel[]) ((IEnumerable<UserStatus>) waContacts).Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u => new UserViewModel(u, false))).ToArray<UserViewModel>();
        }
        if (this.ItemVisibleFilter != null)
          source = ((IEnumerable<JidItemViewModel>) source).Where<JidItemViewModel>(this.ItemVisibleFilter).ToArray<JidItemViewModel>();
        if (source.Length > 20)
        {
          int num = 0;
          List<JidItemViewModel> list = ((IEnumerable<JidItemViewModel>) source).ToList<JidItemViewModel>();
          ManualResetEventSlim ev = new ManualResetEventSlim();
          while (list.Count > 0)
          {
            ev.Reset();
            int count = Math.Min(10 * (num + 1), list.Count);
            JidItemViewModel[] array = list.Take<JidItemViewModel>(count).ToArray<JidItemViewModel>();
            list.RemoveRange(0, count);
            observer.OnNext(array);
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() => ev.Set()));
            ev.Wait();
            ++num;
          }
          ev.Dispose();
        }
        else
          observer.OnNext(source);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public override FrameworkElement CreateItem() => (FrameworkElement) new UserItemControl();

    public virtual UserStatus[] LoadFromDb(ContactsContext db)
    {
      return ((IEnumerable<UserStatus>) db.GetWaContacts(false)).OrderBy<UserStatus, string>((Func<UserStatus, string>) (u => u.GetDisplayName())).ToArray<UserStatus>();
    }

    public override IObservable<ListTabSearchResult> GetSearchObservable(string searchTerm)
    {
      return Observable.Create<ListTabSearchResult>((Func<IObserver<ListTabSearchResult>, Action>) (observer =>
      {
        ListTabSearchResult listTabSearchResult = new ListTabSearchResult(searchTerm);
        List<UserStatus> matchedUsers = (List<UserStatus>) null;
        string ftsQuery;
        ChatSearchPage.ProcessSearchTerm(searchTerm, out string _, out ftsQuery);
        ContactsContext.Instance((Action<ContactsContext>) (db => matchedUsers = db.LookupUsersByName(ftsQuery)));
        IEnumerable<UserViewModel> source = matchedUsers.Select<UserStatus, UserViewModel>((Func<UserStatus, UserViewModel>) (u =>
        {
          return new UserViewModel(u)
          {
            EnableContextMenu = false
          };
        }));
        listTabSearchResult.Add(this.ItemVisibleFilter == null ? (IEnumerable<JidItemViewModel>) source : ((IEnumerable<JidItemViewModel>) source).Where<JidItemViewModel>(this.ItemVisibleFilter));
        observer.OnNext(listTabSearchResult);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
