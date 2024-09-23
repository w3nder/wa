// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatsListTabData
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
  public class ChatsListTabData : ListTabData
  {
    protected RegexSearch regexSearch = new RegexSearch();
    protected Dictionary<string, ChatItemViewModel> loadedItems;

    public ChatsListTabData()
    {
      this.Header = AppResources.RecentHeader;
      this.ItemTemplate = XamlReader.Load("\r\n                <DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                                xmlns:local=\"clr-namespace:WhatsApp;assembly=WhatsApp\">\r\n                    <local:ChatItemControl ViewModel=\"{Binding}\"\r\n                                            Margin=\"0,0,12,24\"/>\r\n                </DataTemplate>") as DataTemplate;
      this.ItemsObservable = Observable.Create<JidItemViewModel[]>((Func<IObserver<JidItemViewModel[]>, Action>) (observer =>
      {
        List<Conversation> convos = (List<Conversation>) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => convos = this.LoadFromDb(db)));
        ChatItemViewModel[] array1 = convos.Select<Conversation, ChatItemViewModel>((Func<Conversation, ChatItemViewModel>) (c =>
        {
          return new ChatItemViewModel(c, (Message) null)
          {
            EnableRecipientCheck = true,
            EnableChatPreview = false,
            EnableContextMenu = false
          };
        })).ToArray<ChatItemViewModel>();
        if (array1.Length > 20)
        {
          int num = 0;
          List<ChatItemViewModel> list = ((IEnumerable<ChatItemViewModel>) array1).ToList<ChatItemViewModel>();
          ManualResetEventSlim ev = new ManualResetEventSlim();
          while (list.Count > 0)
          {
            ev.Reset();
            int count = Math.Min(10 * (num + 1), list.Count);
            ChatItemViewModel[] array2 = list.Take<ChatItemViewModel>(count).ToArray<ChatItemViewModel>();
            list.RemoveRange(0, count);
            observer.OnNext((JidItemViewModel[]) array2);
            Deployment.Current.Dispatcher.BeginInvoke((Action) (() => ev.Set()));
            ev.Wait();
            ++num;
          }
          ev.Dispose();
        }
        else
          observer.OnNext((JidItemViewModel[]) array1);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public override FrameworkElement CreateItem() => (FrameworkElement) new ChatItemControl();

    public override IObservable<ListTabSearchResult> GetSearchObservable(string searchTerm)
    {
      return Observable.Create<ListTabSearchResult>((Func<IObserver<ListTabSearchResult>, Action>) (observer =>
      {
        string[] strArray1;
        if (!string.IsNullOrWhiteSpace(searchTerm))
          strArray1 = ((IEnumerable<string>) searchTerm.ToLower().Split(' ')).Where<string>((Func<string, bool>) (part => !string.IsNullOrEmpty(part))).ToArray<string>();
        else
          strArray1 = new string[0];
        string[] strArray2 = strArray1;
        if (((IEnumerable<string>) strArray2).Any<string>())
        {
          List<ChatItemViewModel> list = this.regexSearch.Lookup(strArray2).Select<SearchMethodBase.Match, ChatItemViewModel>((Func<SearchMethodBase.Match, ChatItemViewModel>) (m => m.Tag as ChatItemViewModel)).Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (item => item != null)).ToList<ChatItemViewModel>();
          string ftsQuery;
          ChatSearchPage.ProcessSearchTerm(searchTerm, out string _, out ftsQuery);
          List<UserStatus> matchedUsers = (List<UserStatus>) null;
          ContactsContext.Instance((Action<ContactsContext>) (db => matchedUsers = db.LookupUsersByName(ftsQuery)));
          ChatItemViewModel chatItem = (ChatItemViewModel) null;
          list.AddRange(matchedUsers.Select<UserStatus, ChatItemViewModel>((Func<UserStatus, ChatItemViewModel>) (u => this.loadedItems.TryGetValue(u.Jid, out chatItem) ? chatItem : (ChatItemViewModel) null)).Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (item => item != null)));
          list.Sort(new Comparison<ChatItemViewModel>(ChatsListTabData.CompareItemByTimestamp));
          ListTabSearchResult listTabSearchResult = new ListTabSearchResult(searchTerm);
          listTabSearchResult.Add((IEnumerable<JidItemViewModel>) list);
          observer.OnNext(listTabSearchResult);
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    private static int CompareItemByTimestamp(ChatItemViewModel item1, ChatItemViewModel item2)
    {
      return Conversation.CompareByTimestamp(item1.Conversation, item2.Conversation);
    }

    public override void OnAllLoaded(IEnumerable<JidItemViewModel> items)
    {
      ChatItemViewModel[] array = items.Cast<ChatItemViewModel>().Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (item => item != null)).ToArray<ChatItemViewModel>();
      try
      {
        this.loadedItems = ((IEnumerable<ChatItemViewModel>) array).ToDictionary<ChatItemViewModel, string, ChatItemViewModel>((Func<ChatItemViewModel, string>) (item => item.Conversation.Jid), (Func<ChatItemViewModel, ChatItemViewModel>) (item => item));
      }
      catch (Exception ex)
      {
        this.loadedItems = new Dictionary<string, ChatItemViewModel>();
        foreach (ChatItemViewModel chatItemViewModel in array)
          this.loadedItems.Add(chatItemViewModel.Conversation.Jid, chatItemViewModel);
      }
      this.regexSearch.Init(((IEnumerable<ChatItemViewModel>) array).Where<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (item => !item.Conversation.IsUserChat())).Select<ChatItemViewModel, KeyValuePair<string, object>>((Func<ChatItemViewModel, KeyValuePair<string, object>>) (item => new KeyValuePair<string, object>(item.Conversation.GroupSubject ?? "", (object) item))).ToArray<KeyValuePair<string, object>>());
    }

    protected virtual List<Conversation> LoadFromDb(MessagesContext db)
    {
      return db.GetConversations(new JidHelper.JidTypes[2]
      {
        JidHelper.JidTypes.User,
        JidHelper.JidTypes.Group
      }, true, true);
    }
  }
}
