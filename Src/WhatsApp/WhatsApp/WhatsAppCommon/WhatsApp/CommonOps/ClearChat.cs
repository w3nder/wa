// Decompiled with JetBrains decompiler
// Type: WhatsApp.CommonOps.ClearChat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;


namespace WhatsApp.CommonOps
{
  public static class ClearChat
  {
    private const int MessageBatchDeleteLimit = 100;

    public static void Clear(string jid, bool keepStarred)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => ClearChat.ClearImpl(db, jid, keepStarred, false)));
    }

    public static void ClearAndDelete(string jid, bool keepStarred)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => ClearChat.ClearImpl(db, jid, keepStarred, true)));
    }

    public static void ClearAll(bool keepStarred)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => ClearChat.ClearAllImpl(db, keepStarred, false)));
    }

    public static void ClearAndDeleteAll(bool keepStarred)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => ClearChat.ClearAllImpl(db, keepStarred, true)));
    }

    private static void ClearAllImpl(MessagesContext db, bool keepStarred, bool deleteChat)
    {
      Log.l("clear all chats", "keep starred:{0},delete:{1}", (object) keepStarred, (object) deleteChat);
      if (db == null)
        return;
      bool hasStarredMsgsToKeep = false;
      if (keepStarred)
        hasStarredMsgsToKeep = db.AnyStarredMessages();
      int? deleteUpperBound = new int?();
      deleteUpperBound = new int?((int) db.GetLastMsgId() + 1);
      List<Conversation> conversations1;
      if (hasStarredMsgsToKeep)
      {
        List<Conversation> conversations2 = db.GetConversations(new JidHelper.JidTypes[2]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group
        }, true, true);
        conversations1 = db.GetConversations(new JidHelper.JidTypes[2]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group
        }, true, false);
        foreach (Conversation convo in conversations2)
        {
          db.MarkMessagesAsDeleted(convo.Jid, true);
          convo.Status = new Conversation.ConversationStatus?(Conversation.ConversationStatus.Clearing);
          convo.FirstUnreadMessageID = new int?();
          convo.UnreadMessageCount = new int?(0);
          db.UpdateChatLastMessage(convo);
          string jid = convo.Jid;
          JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
          if (jidInfo != null && jidInfo.SupportsFullEncryption == JidInfo.FullEncryptionState.SupportedAndNotified)
          {
            Message conversationEncrypted = SystemMessageUtils.CreateConversationEncrypted(db, jid);
            db.InsertMessageOnSubmit(conversationEncrypted);
          }
          SystemMessageUtils.TryGenerateInitialBizSystemMessage2Tier((SqliteMessagesContext) db, jid, false);
          AppState.QrPersistentAction.NotifyChatStatus(jid, FunXMPP.ChatStatusForwardAction.ClearNotStarred);
        }
      }
      else
        conversations1 = db.GetConversations(new JidHelper.JidTypes[2]
        {
          JidHelper.JidTypes.User,
          JidHelper.JidTypes.Group
        }, true);
      foreach (Conversation convo in conversations1)
      {
        convo.EffectiveFirstMessageID = deleteUpperBound;
        bool flag = false;
        if (convo.JidType == JidHelper.JidTypes.Group)
          flag = true;
        convo.Status = new Conversation.ConversationStatus?(!deleteChat || flag ? Conversation.ConversationStatus.Clearing : Conversation.ConversationStatus.Deleting);
        convo.FirstUnreadMessageID = new int?();
        convo.UnreadMessageCount = new int?(0);
        db.UpdateChatLastMessage(convo);
        string jid = convo.Jid;
        JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo != null && jidInfo.SupportsFullEncryption == JidInfo.FullEncryptionState.SupportedAndNotified)
        {
          Message conversationEncrypted = SystemMessageUtils.CreateConversationEncrypted(db, jid);
          db.InsertMessageOnSubmit(conversationEncrypted);
        }
        Message liveLocationMessage = db.GetLatestLiveLocationMessage(jid, Settings.MyJid);
        if (liveLocationMessage != null && liveLocationMessage.isCurrentlyLiveLocationMessage())
          LiveLocationManager.Instance.DisableLocationSharing(jid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
        SystemMessageUtils.TryGenerateInitialBizSystemMessage2Tier((SqliteMessagesContext) db, jid, false);
        if (deleteChat && !flag)
          db.DeletedConversationSubject.OnNext(convo);
        AppState.QrPersistentAction.NotifyChatStatus(jid, !deleteChat || flag ? FunXMPP.ChatStatusForwardAction.Clear : FunXMPP.ChatStatusForwardAction.Delete);
      }
      WaScheduledTask clearAllMessagesTask = ClearChat.CreateClearAllMessagesTask(deleteUpperBound, deleteChat, hasStarredMsgsToKeep);
      db.InsertWaScheduledTaskOnSubmit(clearAllMessagesTask);
      db.SubmitChanges();
      db.AttemptScheduledTaskOnThreadPool(clearAllMessagesTask, 1000);
    }

    private static void ClearImpl(
      MessagesContext db,
      string jid,
      bool keepStarred,
      bool deleteChat)
    {
      Log.l("clear chat", "jid:{0},keep starred:{1},delete:{2}", (object) jid, (object) keepStarred, (object) deleteChat);
      if (jid == null || db == null)
        return;
      bool flag = false;
      if (keepStarred && db.AnyStarredMessagesWith(jid))
      {
        flag = true;
        deleteChat = false;
      }
      int? deleteUpperBound1 = new int?();
      Conversation conversation = db.GetConversation(jid, CreateOptions.None);
      if (conversation != null)
      {
        int? effectiveFirstMessageId = conversation.EffectiveFirstMessageID;
        if (flag)
        {
          db.MarkMessagesAsDeleted(jid, true);
          deleteUpperBound1 = effectiveFirstMessageId;
        }
        else
        {
          Message message = ((IEnumerable<Message>) db.GetLatestMessages(jid, new int?(), new int?(1), new int?())).FirstOrDefault<Message>();
          if (message != null)
            deleteUpperBound1 = new int?(message.MessageID + 1);
          if (effectiveFirstMessageId.HasValue && (!deleteUpperBound1.HasValue || effectiveFirstMessageId.Value > deleteUpperBound1.Value))
            deleteUpperBound1 = effectiveFirstMessageId;
          conversation.EffectiveFirstMessageID = deleteUpperBound1;
        }
      }
      if (conversation != null)
      {
        conversation.Status = new Conversation.ConversationStatus?(deleteChat ? Conversation.ConversationStatus.Deleting : Conversation.ConversationStatus.Clearing);
        conversation.FirstUnreadMessageID = new int?();
        conversation.UnreadMessageCount = new int?(0);
        db.UpdateChatLastMessage(conversation);
      }
      WaScheduledTask task = ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
      {
        WaScheduledTask.Types.ClearMessages
      }, excludeExpired: false, lookupKey: jid)).FirstOrDefault<WaScheduledTask>();
      if (task == null)
      {
        task = ClearChat.CreateClearMessagesTask(jid, deleteUpperBound1, deleteChat);
        db.InsertWaScheduledTaskOnSubmit(task);
      }
      else
      {
        Log.l("clear chat", "updated scheduled task");
        int num = new BinaryData(task.BinaryData).ReadInt32(0);
        int? deleteUpperBound2 = num < 0 ? new int?() : new int?(num);
        if (!deleteUpperBound2.HasValue || deleteUpperBound1.HasValue && deleteUpperBound2.Value <= deleteUpperBound1.Value)
          deleteUpperBound2 = deleteUpperBound1;
        byte[] numArray = ClearChat.WriteTaskBinaryData(deleteUpperBound2, deleteChat);
        task.BinaryData = numArray;
        task.ExpirationUtc = new DateTime?(DateTime.UtcNow.AddDays(7.0));
      }
      JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
      if (jidInfo != null && jidInfo.SupportsFullEncryption == JidInfo.FullEncryptionState.SupportedAndNotified)
      {
        Message conversationEncrypted = SystemMessageUtils.CreateConversationEncrypted(db, jid);
        db.InsertMessageOnSubmit(conversationEncrypted);
      }
      Message liveLocationMessage = db.GetLatestLiveLocationMessage(jid, Settings.MyJid);
      if (liveLocationMessage != null && liveLocationMessage.isCurrentlyLiveLocationMessage())
        LiveLocationManager.Instance.DisableLocationSharing(jid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
      SystemMessageUtils.TryGenerateInitialBizSystemMessage2Tier((SqliteMessagesContext) db, jid, false);
      db.SubmitChanges();
      if (deleteChat && conversation != null)
        db.DeletedConversationSubject.OnNext(conversation);
      AppState.QrPersistentAction.NotifyChatStatus(jid, deleteChat ? FunXMPP.ChatStatusForwardAction.Delete : (flag ? FunXMPP.ChatStatusForwardAction.ClearNotStarred : FunXMPP.ChatStatusForwardAction.Clear));
      db.AttemptScheduledTaskOnThreadPool(task, 1000);
    }

    private static WaScheduledTask CreateClearAllMessagesTask(
      int? deleteUpperBound,
      bool deleteAfterwards,
      bool hasStarredMsgsToKeep)
    {
      byte[] data = ClearChat.WriteTaskBinaryData(deleteUpperBound, deleteAfterwards, hasStarredMsgsToKeep);
      Log.d("clear all chats", "create clear all msgs task | upper bound:{0},delete:{1} keep starred: {2}", (object) deleteUpperBound, (object) deleteAfterwards, (object) hasStarredMsgsToKeep);
      return new WaScheduledTask(WaScheduledTask.Types.ClearAllMessages, (string) null, data, WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(7.0)));
    }

    private static WaScheduledTask CreateClearMessagesTask(
      string jid,
      int? deleteUpperBound,
      bool deleteAfterwards)
    {
      byte[] data = ClearChat.WriteTaskBinaryData(deleteUpperBound, deleteAfterwards);
      Log.d("clear chat", "create clear msg task | jid:{0},upper bound:{1},delete:{2}", (object) jid, (object) deleteUpperBound, (object) deleteAfterwards);
      return new WaScheduledTask(WaScheduledTask.Types.ClearMessages, jid, data, WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(7.0)));
    }

    private static byte[] WriteTaskBinaryData(
      int? deleteUpperBound,
      bool deleteAfterwards,
      bool hasStarredMsgsToKeep = false)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(deleteUpperBound ?? -1);
      binaryData.AppendInt32(deleteAfterwards ? 1 : 0);
      binaryData.AppendInt32(hasStarredMsgsToKeep ? 1 : 0);
      return binaryData.Get();
    }

    public static IObservable<Unit> PerformClearAllChatHistory()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          WaScheduledTask task = ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
          {
            WaScheduledTask.Types.ClearAllMessages
          }, excludeExpired: false)).FirstOrDefault<WaScheduledTask>();
          if (task == null)
            return;
          BinaryData binaryData = new BinaryData(task.BinaryData);
          int offset3 = 0;
          int num = binaryData.ReadInt32(offset3);
          int? nullable = num < 0 ? new int?() : new int?(num);
          int offset4 = offset3 + 4;
          bool flag = binaryData.ReadInt32(offset4) != 0;
          bool hasStarredMsgsToKeep = binaryData.ReadInt32(offset4 + 4) != 0;
          int deleted = 0;
          db.BatchDeleteMessages((string) null, 100, nullable, out deleted, hasStarredMsgsToKeep);
          Log.d("clear all chats", "batch delete | upper bound:{0},delete chat:{1},batch count:{2},deleted:{3}", (object) nullable, (object) flag, (object) 100, (object) deleted);
          if (!db.AnyPendingMessageDeletions(nullable, hasStarredMsgsToKeep))
          {
            observer.OnNext(new Unit());
            foreach (Conversation c in db.GetConversationsToDeleteOrClear())
            {
              Conversation.ConversationStatus? status = c.Status;
              if (status.HasValue)
              {
                status = c.Status;
                if (status.Value == Conversation.ConversationStatus.Deleting)
                {
                  c.SkipDeleteNotification = true;
                  db.DeleteConversationOnSubmit(c);
                  goto label_7;
                }
              }
              c.Status = new Conversation.ConversationStatus?(Conversation.ConversationStatus.None);
label_7:
              c.EffectiveFirstMessageID = new int?();
            }
            Log.l("clear/delete all chats done");
            db.SubmitChanges();
          }
          else
          {
            observer.OnCompleted();
            db.AttemptScheduledTaskOnThreadPool(task, 1500, true);
          }
        }));
        PerformanceTimer.End("clear all messages attempt", start);
        return (Action) (() => { });
      }));
    }

    public static IObservable<Unit> PerformClearChatHistory(string jid)
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          WaScheduledTask task = ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
          {
            WaScheduledTask.Types.ClearMessages
          }, excludeExpired: false, lookupKey: jid)).FirstOrDefault<WaScheduledTask>();
          if (task == null)
            return;
          BinaryData binaryData = new BinaryData(task.BinaryData);
          int offset = 0;
          int num = binaryData.ReadInt32(offset);
          int? upperBound = num < 0 ? new int?() : new int?(num);
          bool flag = binaryData.ReadInt32(offset + 4) != 0;
          int deleted = 0;
          db.BatchDeleteMessages(jid, 100, upperBound, out deleted);
          Log.d("clear chat", "batch delete | jid:{0},upper bound:{1},delete chat:{2},batch count:{3},deleted:{4}", (object) jid, (object) upperBound, (object) flag, (object) 100, (object) deleted);
          if (!db.AnyPendingMessageDeletions(jid))
          {
            observer.OnNext(new Unit());
            Conversation conversation = db.GetConversation(jid, CreateOptions.None);
            if (conversation == null)
              return;
            Conversation.ConversationStatus? status = conversation.Status;
            if (status.HasValue)
            {
              status = conversation.Status;
              if (status.Value == Conversation.ConversationStatus.Deleting)
              {
                conversation.SkipDeleteNotification = true;
                db.DeleteConversationOnSubmit(conversation);
                Log.l("delete chat", "done | jid:{0}", (object) jid);
                goto label_7;
              }
            }
            conversation.Status = new Conversation.ConversationStatus?(Conversation.ConversationStatus.None);
            Log.l("clear chat", "done | jid:{0}", (object) jid);
label_7:
            conversation.EffectiveFirstMessageID = new int?();
            db.SubmitChanges();
          }
          else
          {
            observer.OnCompleted();
            db.AttemptScheduledTaskOnThreadPool(task, 1500, true);
          }
        }));
        PerformanceTimer.End(string.Format("clear messages attempt for {0}", (object) jid), start);
        return (Action) (() => { });
      }));
    }
  }
}
