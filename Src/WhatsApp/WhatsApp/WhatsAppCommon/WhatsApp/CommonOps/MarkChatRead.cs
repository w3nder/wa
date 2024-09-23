// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.MarkChatRead
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp.CommonOps
{
  public static class MarkChatRead
  {
    public static void MarkReadToMessage(
      MessagesContext db,
      string jid,
      int? lastReadMsgId,
      bool notifyQr,
      bool submitDb)
    {
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation == null)
      {
        Log.l("CommonOps", "mark read | chat not found | jid: {0}", (object) jid);
      }
      else
      {
        bool flag = true;
        Message[] source = (Message[]) null;
        if (lastReadMsgId.HasValue)
        {
          Message messageById = db.GetMessageById(lastReadMsgId.Value);
          if (messageById != null && messageById.KeyRemoteJid == jid && conversation.LastMessageID.HasValue && messageById.MessageID < conversation.LastMessageID.Value)
          {
            source = ((IEnumerable<Message>) db.GetMessagesAfter(jid, conversation.MessageLoadingStart(), new int?(lastReadMsgId.Value), false, new int?(), new int?())).Where<Message>((Func<Message, bool>) (m => !m.KeyFromMe)).ToArray<Message>();
            if (((IEnumerable<Message>) source).Any<Message>())
            {
              Log.d("CommonOps", "mark read | partial | jid: {0}, last read: {1}, last: {2}, remain: {3}", (object) jid, (object) lastReadMsgId.Value, (object) conversation.LastMessageID.Value, (object) source.Length);
              flag = false;
            }
          }
        }
        if (conversation.FirstUnreadMessageID.HasValue || db.HasDecryptedCipherTextReceipts(jid))
        {
          ReceiptSpec[] array = db.GetOutgoingReadReceipts(jid, conversation.FirstUnreadMessageID, flag ? new int?() : lastReadMsgId).ToArray<ReceiptSpec>();
          Log.d("common ops", "mark read | jid: {0}, {1} read receipts to be sent", (object) jid, (object) array.Length);
          ReadReceipts.Send(db, array);
        }
        int unreadMessagesCount = conversation.GetUnreadMessagesCount();
        int num = 0;
        if (source != null && ((IEnumerable<Message>) source).Any<Message>())
        {
          num = source.Length;
          conversation.UnreadMessageCount = num > 0 ? new int?(num) : new int?();
          Message message = ((IEnumerable<Message>) source).FirstOrDefault<Message>();
          conversation.FirstUnreadMessageID = message == null ? new int?() : new int?(message.MessageID);
          TileHelper.DecrementChatTile(db, jid, unreadMessagesCount - num);
        }
        else
        {
          conversation.UnreadMessageCount = new int?();
          conversation.FirstUnreadMessageID = new int?();
          TileHelper.ClearChatTile(db, conversation.Jid);
        }
        Log.d("common ops", "mark read | jid: {0}, unread count: {1}->{2}", (object) jid, (object) unreadMessagesCount, (object) num);
        if (notifyQr)
          AppState.QrPersistentAction.NotifyRead(conversation.Jid, true);
        conversation.AutomuteTimer = new DateTime?();
        if (!submitDb)
          return;
        db.SubmitChanges();
      }
    }

    public static void MarkRead(MessagesContext db, string jid, bool notifyQr, bool submitDb)
    {
      MarkChatRead.MarkReadToMessage(db, jid, new int?(), notifyQr, submitDb);
    }

    public static void MarkUnread(MessagesContext db, string jid, bool notifyQr)
    {
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation == null || !conversation.IsRead())
        return;
      conversation.UnreadMessageCount = new int?(-1);
      if (notifyQr)
        AppState.QrPersistentAction.NotifyRead(conversation.Jid, false);
      db.SubmitChanges();
    }

    public static void ToggleReadState(MessagesContext db, string jid, bool notifyQr)
    {
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation == null)
        return;
      if (conversation.IsRead())
        MarkChatRead.MarkUnread(db, jid, notifyQr);
      else
        MarkChatRead.MarkRead(db, jid, notifyQr, true);
    }
  }
}
