// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversationItem
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;

#nullable disable
namespace WhatsApp
{
  public class ConversationItem
  {
    public UserStatus userStatus_;
    public Subject<Unit> UpdateSubject = new Subject<Unit>();

    public string Jid => this.Conversation.Jid;

    public UserStatus UserStatus
    {
      get
      {
        if (this.userStatus_ == null && JidHelper.IsUserJid(this.Jid))
          this.userStatus_ = ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (db => db.GetUserStatus(this.Jid)));
        return this.userStatus_;
      }
    }

    public Conversation Conversation { get; private set; }

    public Message Message { get; set; }

    public ConversationItem(Conversation c) => this.Conversation = c;

    public static int CompareBySortKeyAndTimestamp(ConversationItem ci1, ConversationItem ci2)
    {
      if (ci1 == null)
        return 1;
      if (ci2 == null)
        return -1;
      int num = Conversation.CompareBySortKey(ci1.Conversation, ci2.Conversation);
      return num == 0 ? Conversation.CompareByTimestamp(ci1.Conversation, ci2.Conversation) : num;
    }

    public static int CompareByTimestamp(ConversationItem ci1, ConversationItem ci2)
    {
      if (ci1 == null)
        return 1;
      return ci2 == null ? -1 : Conversation.CompareByTimestamp(ci1.Conversation, ci2.Conversation);
    }

    public override bool Equals(object obj)
    {
      return obj is ConversationItem conversationItem && this.Jid == conversationItem.Jid;
    }

    public override int GetHashCode() => this.Jid.GetHashCode();
  }
}
