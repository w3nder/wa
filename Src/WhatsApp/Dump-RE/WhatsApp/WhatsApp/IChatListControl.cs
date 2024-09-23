// Decompiled with JetBrains decompiler
// Type: WhatsApp.IChatListControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;

#nullable disable
namespace WhatsApp
{
  public interface IChatListControl
  {
    bool IsMultiSelectionEnabled { get; set; }

    bool IsMultiSelectionAllowed { get; set; }

    IObservable<Conversation> ChatRequestedObservable();

    IObservable<Unit> MultiSelectionChangedObservable();

    IObservable<bool> PendingSelectionObservable();

    void SetSourceAsync(
      IObservable<List<ConversationItem>> itemsObs,
      Func<Conversation, bool> chatFilter,
      Action<ChatCollection> onComplete,
      string context);

    void Clear();

    List<string> GetMultiSelectedChats();

    void ScrollToTop();

    void EnsurePreviousSelectedChatVisible();

    void SetTooltip(string s);

    void EnableMultiSelection(bool enable);

    void OnChatCollectionChanged();
  }
}
