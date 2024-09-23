// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.DeleteChat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp.CommonOps
{
  public static class DeleteChat
  {
    public static IObservable<bool> PromptForDelete(IObservable<bool> source, Conversation convo)
    {
      int pendingMsgCount = 0;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => pendingMsgCount = db.GetPendingMessages(convo.Jid).Length));
      IObservable<bool> observable = (IObservable<bool>) null;
      string prompt = (string) null;
      string title = (string) null;
      string positive = AppResources.Delete;
      if (convo.IsGroup())
      {
        if (!convo.IsGroupParticipant())
          prompt = string.Format(AppResources.ConfirmDeleteGroup, (object) (convo.GroupSubject ?? AppResources.GroupChatSubject));
        else if (pendingMsgCount > 0)
        {
          prompt = string.Format(AppResources.ConfirmLeaveGroup, (object) (convo.GroupSubject ?? AppResources.GroupChatSubject)) + " " + Plurals.Instance.GetString(AppResources.YouHavePendingMessagesInGroupPlural, pendingMsgCount);
          positive = AppResources.ExitButton;
        }
        else
        {
          prompt = string.Format(AppResources.ConfirmLeaveGroup, (object) (convo.GroupSubject ?? AppResources.GroupChatSubject));
          positive = AppResources.ExitButton;
        }
      }
      else if (convo.IsUserChat())
      {
        UserStatus userStatus = ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (db => db.GetUserStatus(convo.Jid, false)));
        prompt = string.Format(AppResources.ConfirmDeletingChat, userStatus == null ? (object) JidHelper.GetPhoneNumber(convo.Jid, true) : (object) userStatus.GetDisplayName());
        if (pendingMsgCount > 0)
          prompt = string.Format("{0} {1}", (object) prompt, (object) Plurals.Instance.GetString(AppResources.YouHavePendingMessagesPlural, pendingMsgCount));
      }
      else if (convo.IsBroadcast())
      {
        title = AppResources.DeleteBroadcastListConfirmTitle;
        prompt = convo.GroupSubject == null ? AppResources.DeleteBroadcastListConfirmBodyNoName : string.Format(AppResources.DeleteBroadcastListConfirmBody, (object) convo.GetName());
        if (pendingMsgCount > 0)
          prompt = string.Format("{0} {1}", (object) prompt, (object) Plurals.Instance.GetString(AppResources.YouHavePendingMessagesPlural, pendingMsgCount));
      }
      if (prompt != null)
        observable = AppState.ClientInstance.Decision(source, prompt, positive, AppResources.CancelButton, title);
      return observable ?? source;
    }

    public static void Delete(string[] jids)
    {
      Dictionary<string, Conversation> convos = new Dictionary<string, Conversation>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (string jid in jids)
        {
          if (JidHelper.IsUserJid(jid))
            convos[jid] = db.GetConversation(jid, CreateOptions.None);
        }
      }));
      foreach (KeyValuePair<string, Conversation> keyValuePair in convos)
        DeleteChat.DeleteImpl(keyValuePair.Key, keyValuePair.Value);
    }

    public static void Delete(string jid)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
      DeleteChat.DeleteImpl(jid, convo);
    }

    public static void DeleteSpam(string jid)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(jid, CreateOptions.None)));
      DeleteChat.DeleteImpl(jid, convo, true);
    }

    private static void DeleteImpl(string jid, Conversation convo, bool isSpam = false)
    {
      if (convo != null && jid != convo.Jid)
        return;
      Log.l("delete chat", "started | jid:{0}", (object) jid);
      if (convo != null && convo.IsGroup() && convo.IsGroupParticipant())
      {
        if (isSpam)
        {
          AppState.SchedulePersistentAction(PersistentAction.LeaveAndDeleteGroup(jid));
        }
        else
        {
          AppState.SchedulePersistentAction(PersistentAction.LeaveGroup(jid));
          return;
        }
      }
      if (convo != null && convo.IsPinned())
        PinChat.Unpin(convo, false);
      ClearChat.ClearAndDelete(jid, false);
      TileHelper.GetChatTile(jid)?.Delete();
      if (convo == null)
        return;
      PushSystem.Instance.ClearToastHistoryGroup(convo.ConversationID.ToString());
    }
  }
}
