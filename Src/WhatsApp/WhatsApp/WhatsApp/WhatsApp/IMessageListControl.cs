// Decompiled with JetBrains decompiler
// Type: WhatsApp.IMessageListControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public interface IMessageListControl
  {
    event EventHandler MultiSelectionsChanged;

    event EventHandler SelectionChanged;

    event EventHandler OlderMessagesRequested;

    event EventHandler NewerMessagesRequested;

    event EventHandler ScrollToBottomRequested;

    bool IsRenderingInverted { get; set; }

    void RefreshMessages(Action<MessageViewModel> refresh);

    void SetMessages(
      Message[] msgs,
      bool asc,
      int? targetLandingIndex,
      MessageSearchResult searchRes = null,
      bool forStarredMessagesView = false,
      string jidForContactCardView = null);

    void AddToRecent(Message msg);

    void AddToRecent(Message[] msgs);

    void AddToOldest(Message[] msgs);

    void UpdateMessages(Message[] msgs);

    void Remove(Message msg);

    void RemoveUnreadDivider();

    bool IncreaseUnreadDivider(int n = 1);

    void RemoveAll();

    bool IsEmpty();

    IObservable<Message> NewerMessageRealized();

    bool IsMultiSelectionEnabled { get; set; }

    bool IsAnyMessagesSelected { get; }

    List<Message> GetMultiSelectedMessages();

    Message LastTappedQuotedMsg { get; set; }

    Message LastTappedReplyMsg { get; set; }

    void InitializeScrolling();

    void ScrollToInitialPosition();

    bool ScrollToMessage(Message m);

    bool ScrollToMessage(int msgId);

    void ScrollToRecent(string context);

    void ScrollToOldest();

    void ScrollFromNewMessage();

    bool IsScrolledToRecent();

    void SetTopPadding(double padding);

    void Dispose();

    void OnViewReload();

    void SaveTappedReplyMsgBookmarks(Message quotedMsg, Message replyMsg);

    void ClearTappedReplyMsgBookmarks();

    bool IsMessageRealized(Message message);
  }
}
