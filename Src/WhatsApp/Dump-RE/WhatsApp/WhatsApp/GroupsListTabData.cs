// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupsListTabData
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class GroupsListTabData : ChatsListTabData
  {
    public GroupsListTabData() => this.Header = AppResources.GroupsHeader;

    protected override List<Conversation> LoadFromDb(MessagesContext db)
    {
      List<Conversation> conversations = db.GetConversations(new JidHelper.JidTypes[1]
      {
        JidHelper.JidTypes.Group
      }, true);
      conversations.Sort(new Comparison<Conversation>(Conversation.CompareByName));
      return conversations;
    }

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
          ListTabSearchResult listTabSearchResult = new ListTabSearchResult(searchTerm);
          listTabSearchResult.Add(this.regexSearch.Lookup(strArray2).Select<SearchMethodBase.Match, JidItemViewModel>((Func<SearchMethodBase.Match, JidItemViewModel>) (m => m.Tag as JidItemViewModel)).Where<JidItemViewModel>((Func<JidItemViewModel, bool>) (item => item != null)));
          observer.OnNext(listTabSearchResult);
        }
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }
  }
}
