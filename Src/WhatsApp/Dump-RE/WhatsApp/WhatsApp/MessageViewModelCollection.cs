// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageViewModelCollection
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public class MessageViewModelCollection : WaDisposable
  {
    private bool renderInvertly = true;
    private bool asc;
    private ObservableCollection<MessageViewModel> messageViewModels_;
    private IDisposable updatedMsgSub_;
    private HashSet<MessageViewModel> mediaElementMessages = new HashSet<MessageViewModel>();

    public int TargetLandingPos
    {
      get
      {
        return this.TargetLandingItem != null ? this.messageViewModels_.IndexOf(this.TargetLandingItem) : -1;
      }
    }

    public MessageViewModel TargetLandingItem { get; private set; }

    public MessageViewModel UnreadDividerItem { get; private set; }

    public int Count => this.messageViewModels_ != null ? this.messageViewModels_.Count : 0;

    public MessageViewModel FirstOrDefaultForMessage(int msgId)
    {
      if (this.messageViewModels_ == null)
        return (MessageViewModel) null;
      return this.asc ? this.messageViewModels_.FirstOrDefault<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm.Message.MessageID == msgId)) : this.messageViewModels_.LastOrDefault<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm.Message.MessageID == msgId));
    }

    public MessageViewModel FirstOrDefaultForMessage(Message msg)
    {
      return this.FirstOrDefaultForMessage(msg.MessageID);
    }

    public MessageViewModel NewestOrDefault()
    {
      if (!this.asc)
      {
        ObservableCollection<MessageViewModel> messageViewModels = this.messageViewModels_;
        return messageViewModels == null ? (MessageViewModel) null : messageViewModels.FirstOrDefault<MessageViewModel>();
      }
      ObservableCollection<MessageViewModel> messageViewModels1 = this.messageViewModels_;
      return messageViewModels1 == null ? (MessageViewModel) null : messageViewModels1.LastOrDefault<MessageViewModel>();
    }

    public MessageViewModel OldestOrDefault()
    {
      if (!this.asc)
        return this.messageViewModels_.LastOrDefault<MessageViewModel>();
      ObservableCollection<MessageViewModel> messageViewModels = this.messageViewModels_;
      return messageViewModels == null ? (MessageViewModel) null : messageViewModels.FirstOrDefault<MessageViewModel>();
    }

    public ObservableCollection<MessageViewModel> Get() => this.messageViewModels_;

    public void Set(
      Message[] msgs,
      bool asc,
      bool renderInvertly,
      int? targetLandingIndex,
      MessageSearchResult searchRes = null,
      bool forStarredView = false,
      string jidForContactCardView = null)
    {
      this.asc = asc;
      this.renderInvertly = renderInvertly;
      this.Clear();
      if (searchRes != null && searchRes.Message != null && targetLandingIndex.HasValue)
      {
        Message message = ((IEnumerable<Message>) msgs).ElementAtOrDefault<Message>(targetLandingIndex.Value);
        if (message == null || message.MessageID != searchRes.Message.MessageID)
          searchRes = (MessageSearchResult) null;
      }
      bool reverseSplitted = this.renderInvertly;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (searchRes == null)
        {
          Action<MessageViewModel> postModifier = (Action<MessageViewModel>) null;
          if (jidForContactCardView != null)
            postModifier = (Action<MessageViewModel>) (mvm => mvm.JidForContactCardView = jidForContactCardView);
          this.messageViewModels_ = new ObservableCollection<MessageViewModel>(((IEnumerable<Message>) msgs).SelectMany<Message, MessageViewModel>((Func<Message, IEnumerable<MessageViewModel>>) (m => MessageViewModel.CreateForMessage(m, renderInvertly, reverseSplitted, true, forStarredView: forStarredView, postModifier: postModifier))));
        }
        else
        {
          int resMsgId = searchRes.Message.MessageID;
          this.messageViewModels_ = new ObservableCollection<MessageViewModel>(((IEnumerable<Message>) msgs).SelectMany<Message, MessageViewModel>((Func<Message, IEnumerable<MessageViewModel>>) (m => m.MessageID == resMsgId ? MessageViewModel.CreateForMessage(m, renderInvertly, reverseSplitted, true, searchRes, forStarredView) : MessageViewModel.CreateForMessage(m, renderInvertly, reverseSplitted, true, forStarredView: forStarredView))));
        }
        if (!forStarredView)
          MessageViewModel.ProcessMessagesGrouping((IList<MessageViewModel>) this.messageViewModels_, renderInvertly, true);
        string jid = ((IEnumerable<Message>) msgs).Select<Message, string>((Func<Message, string>) (msg => msg.KeyRemoteJid)).FirstOrDefault<string>();
        if (jid != null)
        {
          this.updatedMsgSub_.SafeDispose();
          this.updatedMsgSub_ = db.UpdatedMessageMediaWaTypeSubject.Where<Message>((Func<Message, bool>) (msg => msg.KeyRemoteJid == jid)).ObserveOnDispatcher<Message>().Subscribe<Message>(new Action<Message>(this.OnMessageUpdated));
        }
        db.SubmitChanges();
      }));
      this.TargetLandingItem = (MessageViewModel) null;
      this.UnreadDividerItem = (MessageViewModel) null;
      if (targetLandingIndex.HasValue)
      {
        if (this.messageViewModels_.Count == msgs.Length)
        {
          this.TargetLandingItem = this.messageViewModels_.ElementAtOrDefault<MessageViewModel>(targetLandingIndex.Value);
        }
        else
        {
          Message message = ((IEnumerable<Message>) msgs).ElementAtOrDefault<Message>(targetLandingIndex.Value);
          if (message != null)
          {
            int targetLandingMsgId = message.MessageID;
            this.TargetLandingItem = this.messageViewModels_.FirstOrDefault<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm.MessageID == targetLandingMsgId));
          }
        }
        if (this.TargetLandingItem != null && this.TargetLandingItem.Message.MediaWaType == FunXMPP.FMessage.Type.Divider)
          this.UnreadDividerItem = this.TargetLandingItem;
      }
      foreach (MessageViewModel messageViewModel in (Collection<MessageViewModel>) this.messageViewModels_)
      {
        if (messageViewModel.ContainsInlineVideo)
          this.mediaElementMessages.Add(messageViewModel);
      }
    }

    public void AddToRecent(Message[] msgs)
    {
      if (this.messageViewModels_ == null || msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      bool reverseSplitted = this.asc == this.renderInvertly;
      MessageViewModel mostRecentVm = this.NewestOrDefault();
      List<MessageViewModel> list = (mostRecentVm == null ? (IEnumerable<Message>) msgs : ((IEnumerable<Message>) msgs).Where<Message>((Func<Message, bool>) (m => m.MessageID > mostRecentVm.Message.MessageID))).SelectMany<Message, MessageViewModel>((Func<Message, IEnumerable<MessageViewModel>>) (m => MessageViewModel.CreateForMessage(m, this.renderInvertly, reverseSplitted, false))).ToList<MessageViewModel>();
      MessageViewModel.ProcessMessagesGrouping((IList<MessageViewModel>) list, false, false, mostRecentVm);
      if (this.asc)
      {
        foreach (MessageViewModel messageViewModel in list)
          this.messageViewModels_.Add(messageViewModel);
      }
      else
      {
        foreach (MessageViewModel messageViewModel in list)
        {
          this.messageViewModels_.Insert(0, messageViewModel);
          if (messageViewModel.ContainsInlineVideo)
            this.mediaElementMessages.Add(messageViewModel);
        }
      }
    }

    public void AddToOldest(Message[] msgs)
    {
      if (this.messageViewModels_ == null || msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      bool reverseSplitted = this.asc != this.renderInvertly;
      List<MessageViewModel> list = ((IEnumerable<Message>) msgs).SelectMany<Message, MessageViewModel>((Func<Message, IEnumerable<MessageViewModel>>) (m => MessageViewModel.CreateForMessage(m, this.renderInvertly, reverseSplitted, false))).ToList<MessageViewModel>();
      MessageViewModel.ProcessMessagesGrouping((IList<MessageViewModel>) list, true, true, this.OldestOrDefault());
      if (this.asc)
      {
        foreach (MessageViewModel messageViewModel in list)
          this.messageViewModels_.Insert(0, messageViewModel);
      }
      else
      {
        foreach (MessageViewModel messageViewModel in list)
        {
          this.messageViewModels_.Add(messageViewModel);
          if (messageViewModel.ContainsInlineVideo)
            this.mediaElementMessages.Add(messageViewModel);
        }
      }
    }

    public void Remove(Message msg)
    {
      if (this.messageViewModels_ == null)
        return;
      this.messageViewModels_.RemoveWhere<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm.Message.MessageID == msg.MessageID));
      this.mediaElementMessages.RemoveWhere((Predicate<MessageViewModel>) (vm => vm.Message.MessageID == msg.MessageID));
      List<MessageViewModel> messageViewModelList = new List<MessageViewModel>();
      for (int index = 0; index < this.messageViewModels_.Count; ++index)
      {
        MessageViewModel messageViewModel = this.messageViewModels_[index];
        if (messageViewModel is DateDividerViewModel)
        {
          if (this.asc)
          {
            if (index == this.messageViewModels_.Count - 1)
              messageViewModelList.Add(messageViewModel);
            else if (this.messageViewModels_[index + 1] is DateDividerViewModel)
              messageViewModelList.Add(messageViewModel);
          }
          else if (index == 0)
            messageViewModelList.Add(messageViewModel);
          else if (this.messageViewModels_[index - 1] is DateDividerViewModel)
            messageViewModelList.Add(messageViewModel);
        }
      }
      foreach (MessageViewModel messageViewModel in messageViewModelList)
        this.messageViewModels_.Remove(messageViewModel);
    }

    public bool IncreaseUnreadDivider(int n)
    {
      if (this.UnreadDividerItem == null || !(this.UnreadDividerItem is UnreadDividerViewModel unreadDividerItem))
        return false;
      unreadDividerItem.IncreaseUnreadCount(n);
      return true;
    }

    public void RemoveUnreadDivider()
    {
      if (this.UnreadDividerItem == null)
        return;
      if (this.messageViewModels_ != null)
        this.messageViewModels_.Remove(this.UnreadDividerItem);
      if (this.TargetLandingItem == this.UnreadDividerItem)
        this.TargetLandingItem = (MessageViewModel) null;
      this.UnreadDividerItem = (MessageViewModel) null;
    }

    public void Clear()
    {
      if (this.messageViewModels_ == null || !this.messageViewModels_.Any<MessageViewModel>())
        return;
      MessageViewModel[] vmSnap = this.messageViewModels_.ToArray<MessageViewModel>();
      this.messageViewModels_.Clear();
      if (((IEnumerable<MessageViewModel>) vmSnap).Any<MessageViewModel>())
        WAThreadPool.QueueUserWorkItem((Action) (() =>
        {
          foreach (IDisposable d in vmSnap)
            d.SafeDispose();
        }));
      this.mediaElementMessages.Clear();
    }

    public void OnMessageUpdated(Message msg)
    {
      if (this.messageViewModels_ == null)
        return;
      MessageViewModel messageViewModel1 = this.messageViewModels_.FirstOrDefault<MessageViewModel>((Func<MessageViewModel, bool>) (vm => vm.Message.MessageID == msg.MessageID));
      if (messageViewModel1 == null || !messageViewModel1.ShouldReplace && msg.MediaWaType != FunXMPP.FMessage.Type.Revoked)
        return;
      Log.d(nameof (OnMessageUpdated), "swapping vm vm.shoudReplace:{0} msg.isRevoked:{1}, msg.MessageID:{2}", (object) messageViewModel1.ShouldReplace, (object) (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked), (object) msg.MessageID);
      int index = this.messageViewModels_.IndexOf(messageViewModel1);
      this.messageViewModels_.RemoveAt(index);
      bool renderInvertly = this.renderInvertly;
      List<MessageViewModel> list = MessageViewModel.CreateForMessage(msg, this.renderInvertly, renderInvertly, false).ToList<MessageViewModel>();
      MessageViewModel.ProcessMessagesGrouping((IList<MessageViewModel>) list, false, true);
      foreach (MessageViewModel messageViewModel2 in list)
      {
        this.messageViewModels_.Insert(index, messageViewModel2);
        if (messageViewModel2.ContainsInlineVideo)
          this.mediaElementMessages.Add(messageViewModel2);
      }
      messageViewModel1.Cleanup();
    }

    public void ApplyToEach(Action<MessageViewModel> a)
    {
      if (this.messageViewModels_ == null)
        return;
      List<MessageViewModel> vmSnap = this.messageViewModels_.ToList<MessageViewModel>();
      AppState.Worker.Enqueue((Action) (() => vmSnap.ForEach(a)));
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.updatedMsgSub_.SafeDispose();
      this.updatedMsgSub_ = (IDisposable) null;
      Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() => this.Clear()));
    }

    public bool HasMediaElementMessages => this.mediaElementMessages.Any<MessageViewModel>();

    public IEnumerable<MessageViewModel> MediaElementMessages
    {
      get => (IEnumerable<MessageViewModel>) this.mediaElementMessages;
    }
  }
}
