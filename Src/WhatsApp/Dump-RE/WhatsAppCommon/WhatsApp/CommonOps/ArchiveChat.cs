// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ArchiveChat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp.CommonOps
{
  public static class ArchiveChat
  {
    private static void Archive(
      MessagesContext db,
      List<Conversation> chats,
      bool notifyQr,
      out Action postDbAction)
    {
      postDbAction = (Action) null;
      Action postDbAction1 = (Action) null;
      List<string> archivedJids = new List<string>();
      chats.ForEach((Action<Conversation>) (c =>
      {
        c.IsArchived = true;
        archivedJids.Add(c.Jid);
      }));
      PinChat.Unpin(db, chats, true, out postDbAction1);
      db.SubmitChanges();
      if (postDbAction1 != null)
        postDbAction1();
      if (!notifyQr || !AppState.QrPersistentAction.ShouldAttemptQrPersistentAction)
        return;
      postDbAction = (Action) (() =>
      {
        foreach (string jid in archivedJids)
          AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.Archive);
      });
    }

    public static void Archive(string[] jids, bool notifyQr)
    {
      Action postDbAct = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> list = ((IEnumerable<string>) jids).Select<string, Conversation>((Func<string, Conversation>) (jid => db.GetConversation(jid, CreateOptions.None))).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).ToList<Conversation>();
        ArchiveChat.Archive(db, list, notifyQr, out postDbAct);
      }));
      if (postDbAct == null)
        return;
      postDbAct();
    }

    public static void Archive(string jid, bool notifyQr = true)
    {
      ArchiveChat.Archive(new string[1]{ jid }, notifyQr);
    }

    public static void ArchiveAll()
    {
      Action postDbAct = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> conversations = db.GetConversations(new JidHelper.JidTypes[3]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group,
          JidHelper.JidTypes.Psa
        }, false);
        ArchiveChat.Archive(db, conversations, true, out postDbAct);
      }));
      if (postDbAct == null)
        return;
      postDbAct();
    }

    private static void Unarchive(
      MessagesContext db,
      List<Conversation> chats,
      bool notifyQr,
      out Action postDbAction)
    {
      postDbAction = (Action) null;
      List<string> unarchivedJids = new List<string>();
      chats.ForEach((Action<Conversation>) (c =>
      {
        c.IsArchived = false;
        unarchivedJids.Add(c.Jid);
      }));
      db.SubmitChanges();
      if (!notifyQr || !AppState.QrPersistentAction.ShouldAttemptQrPersistentAction)
        return;
      postDbAction = (Action) (() =>
      {
        foreach (string jid in unarchivedJids)
          AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.Unarchive);
      });
    }

    public static void Unarchive(string[] jids, bool notifyQr)
    {
      Action postDbAct = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> list = ((IEnumerable<string>) jids).Select<string, Conversation>((Func<string, Conversation>) (jid => db.GetConversation(jid, CreateOptions.None))).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).ToList<Conversation>();
        ArchiveChat.Unarchive(db, list, notifyQr, out postDbAct);
      }));
      if (postDbAct == null)
        return;
      postDbAct();
    }

    public static void Unarchive(string jid, bool notifyQr = true)
    {
      ArchiveChat.Unarchive(new string[1]{ jid }, notifyQr);
    }

    public static void UnarchiveAll()
    {
      Action postDbAct = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Conversation> list = db.GetArchivedConversationItems().Select<ConversationItem, Conversation>((Func<ConversationItem, Conversation>) (ci => ci.Conversation)).ToList<Conversation>();
        ArchiveChat.Unarchive(db, list, true, out postDbAct);
      }));
      if (postDbAct == null)
        return;
      postDbAct();
    }
  }
}
