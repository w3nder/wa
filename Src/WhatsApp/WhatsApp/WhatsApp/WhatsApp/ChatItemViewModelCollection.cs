// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatItemViewModelCollection
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ChatItemViewModelCollection : ChatCollection
  {
    private ObservableCollection<ChatItemViewModel> viewModels;
    private Dictionary<string, ChatItemViewModel> viewModelsDict = new Dictionary<string, ChatItemViewModel>();
    private IDisposable contactUpdateSub;

    public ChatItemViewModelCollection(
      MessagesContext db,
      IEnumerable<ConversationItem> chats,
      Func<Conversation, bool> chatFilter,
      string context)
      : base(db, chats, chatFilter, context)
    {
      this.viewModels = new ObservableCollection<ChatItemViewModel>(chats.Select<ConversationItem, ChatItemViewModel>((Func<ConversationItem, ChatItemViewModel>) (ci => this.GetViewModelForChatItem(ci, true))));
      this.contactUpdateSub = ContactsContext.Events.UserStatusUpdatedSubject.ObserveOnDispatcher<DbDataUpdate>().Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (dbUpdate => this.OnWaContactUpdated(dbUpdate.UpdatedObj as UserStatus)));
    }

    public ObservableCollection<ChatItemViewModel> GetViewModels() => this.viewModels;

    public ChatItemViewModel GetViewModelForChatItem(ConversationItem ci, bool createIfNotFound)
    {
      if (ci == null)
        return (ChatItemViewModel) null;
      ChatItemViewModel modelForChatItem = (ChatItemViewModel) null;
      if (((!this.viewModelsDict.TryGetValue(ci.Jid, out modelForChatItem) ? 1 : (modelForChatItem == null ? 1 : 0)) & (createIfNotFound ? 1 : 0)) != 0)
      {
        modelForChatItem = new ChatItemViewModel(ci.Conversation, ci.Message)
        {
          EnableChatPreview = !ci.Conversation.IsBroadcast()
        };
        this.viewModelsDict[ci.Jid] = modelForChatItem;
      }
      modelForChatItem.SetMessage(ci.Message, false);
      return modelForChatItem;
    }

    protected override void DisposeManagedResources()
    {
      this.contactUpdateSub.SafeDispose();
      this.contactUpdateSub = (IDisposable) null;
      List<ChatItemViewModel> list = this.viewModels.ToList<ChatItemViewModel>();
      this.viewModels.Clear();
      list.ForEach((Action<ChatItemViewModel>) (vm => vm.SafeDispose()));
      this.viewModelsDict.Clear();
      base.DisposeManagedResources();
    }

    protected override void UpdateChatItem(ConversationItem ci)
    {
      ChatItemViewModel modelForChatItem = this.GetViewModelForChatItem(ci, false);
      if (modelForChatItem == null)
      {
        Log.l("chat list", "update chat item failed | jid={0}", ci == null ? (object) "n/a" : (object) ci.Jid);
      }
      else
      {
        modelForChatItem.Refresh();
        if (!modelForChatItem.IsSelected)
          return;
        this.NotifyCollectionChanged();
      }
    }

    public override void Append(ConversationItem ci)
    {
      base.Append(ci);
      this.viewModels.Add(this.GetViewModelForChatItem(ci, true));
    }

    protected override void Remove(string[] jids)
    {
      base.Remove(jids);
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) jids);
      this.viewModels.RemoveWhere<ChatItemViewModel>((Func<ChatItemViewModel, bool>) (vm => ((IEnumerable<string>) jids).Contains<string>(vm.Jid)));
      ChatItemViewModel d = (ChatItemViewModel) null;
      foreach (string jid in jids)
      {
        if (this.viewModelsDict.TryGetValue(jid, out d))
        {
          this.viewModelsDict.Remove(jid);
          d.SafeDispose();
        }
      }
    }

    protected override bool RemoveAt(int i, ConversationItem ci, bool cleanup)
    {
      bool flag = false;
      ChatItemViewModel d = (ChatItemViewModel) null;
      if (base.RemoveAt(i, ci, cleanup))
      {
        this.viewModels.RemoveAt(i);
        if (this.viewModelsDict.TryGetValue(ci.Jid, out d))
          this.viewModelsDict.Remove(ci.Jid);
        flag = true;
      }
      if (cleanup)
        d.SafeDispose();
      return flag;
    }

    protected override void InsertAt(int i, ConversationItem ci)
    {
      base.InsertAt(i, ci);
      this.viewModels.Insert(i, this.GetViewModelForChatItem(ci, true));
    }

    private void OnWaContactUpdated(UserStatus user)
    {
      if (user == null)
        return;
      foreach (ChatItemViewModel viewModel in (Collection<ChatItemViewModel>) this.viewModels)
      {
        if (viewModel.Jid == user.Jid)
        {
          viewModel.ResetVerifiedState();
          viewModel.Refresh();
          break;
        }
      }
    }
  }
}
