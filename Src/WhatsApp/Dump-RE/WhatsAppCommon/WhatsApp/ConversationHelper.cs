// Decompiled with JetBrains decompiler
// Type: WhatsApp.ConversationHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public static class ConversationHelper
  {
    private const string LogHeader = "convo helper";

    public static bool IsGroup(this Conversation convo)
    {
      if (convo == null)
        return false;
      return convo.JidType != JidHelper.JidTypes.Undefined ? convo.JidType == JidHelper.JidTypes.Group : JidHelper.IsGroupJid(convo.Jid);
    }

    public static bool IsBroadcast(this Conversation convo)
    {
      if (convo == null)
        return false;
      return convo.JidType != JidHelper.JidTypes.Undefined ? convo.JidType == JidHelper.JidTypes.Broadcast : JidHelper.IsBroadcastJid(convo.Jid);
    }

    public static bool IsMultiParticipantsChat(this Conversation convo)
    {
      if (convo == null)
        return false;
      if (convo.JidType == JidHelper.JidTypes.Undefined)
        return JidHelper.IsMultiParticipantsChatJid(convo.Jid);
      return convo.JidType == JidHelper.JidTypes.Group || convo.JidType == JidHelper.JidTypes.Broadcast || convo.JidType == JidHelper.JidTypes.Status;
    }

    public static bool IsPsaChat(this Conversation convo)
    {
      if (convo == null)
        return false;
      return convo.JidType != JidHelper.JidTypes.Undefined ? convo.JidType == JidHelper.JidTypes.Psa : JidHelper.IsPsaJid(convo.Jid);
    }

    public static bool IsUserChat(this Conversation convo)
    {
      if (convo == null)
        return false;
      return convo.JidType != JidHelper.JidTypes.Undefined ? convo.JidType == JidHelper.JidTypes.User : JidHelper.IsUserJid(convo.Jid);
    }

    public static bool IsMuted(this Conversation convo)
    {
      return convo.MuteExpiration.HasValue && convo.MuteExpiration.Value > FunRunner.CurrentServerTimeUtc;
    }

    public static bool IsPinned(this Conversation convo) => convo != null && convo.SortKey.HasValue;

    public static int? MessageLoadingStart(this Conversation convo)
    {
      return convo.EffectiveFirstMessageID.HasValue && convo.Status.HasValue && (convo.Status.Value == Conversation.ConversationStatus.Clearing || convo.Status.Value == Conversation.ConversationStatus.Deleting) ? new int?(convo.EffectiveFirstMessageID.Value) : new int?();
    }

    public static string GetName(this Conversation convo, bool forChatPageTitle = false)
    {
      string name = (string) null;
      JidHelper.JidTypes jidType = JidHelper.GetJidType(convo?.Jid);
      switch (jidType)
      {
        case JidHelper.JidTypes.User:
          UserStatus userStatus = UserCache.Get(convo.Jid, false);
          name = userStatus == null ? JidHelper.GetPhoneNumber(convo.Jid, true) : userStatus.GetDisplayName();
          break;
        case JidHelper.JidTypes.Group:
        case JidHelper.JidTypes.Broadcast:
          if (!string.IsNullOrEmpty(convo.GroupSubject))
          {
            name = Emoji.ConvertToUnicode(new string(convo.GroupSubject.Trim().Where<char>((Func<char, bool>) (ch => ch != '\n' && ch != '\r')).ToArray<char>()));
            break;
          }
          switch (jidType)
          {
            case JidHelper.JidTypes.Group:
              name = AppResources.GroupChatSubject;
              break;
            case JidHelper.JidTypes.Broadcast:
              name = forChatPageTitle ? AppResources.PageTitleBroadcastList : string.Format("{0}: {1}", (object) AppResources.Recipients, (object) convo.GetParticipantCount(true));
              break;
          }
          break;
        case JidHelper.JidTypes.Psa:
          name = Constants.OffcialName;
          break;
      }
      return name;
    }

    public static IEnumerable<LinkDetector.Result> GetGroupSubjectPerformanceHint(
      this Conversation convo)
    {
      if (string.IsNullOrEmpty(convo.GroupSubject))
        return (IEnumerable<LinkDetector.Result>) new LinkDetector.Result[0];
      IEnumerable<LinkDetector.Result> matches = (IEnumerable<LinkDetector.Result>) null;
      if (convo.GroupSubjectPerformanceHint == null)
      {
        matches = LinkDetector.GetMatches(Emoji.ConvertToUnicode(convo.GroupSubject));
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          convo.GroupSubjectPerformanceHint = LinkDetector.Result.Serialize(matches);
          db.SubmitChanges();
        }));
      }
      else
        matches = (IEnumerable<LinkDetector.Result>) LinkDetector.Result.Deserialize(Emoji.ConvertToUnicode(convo.GroupSubject), convo.GroupSubjectPerformanceHint);
      return matches;
    }

    public static UserStatus[] GetParticipants(
      string jid,
      bool excludeSelf,
      bool sortByFirstName,
      int firstNToGet = 0,
      Func<UserStatus, bool> filter = null)
    {
      UserStatus[] participants = (UserStatus[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (mdb =>
      {
        Conversation convo = mdb.GetConversation(jid, CreateOptions.None);
        ContactsContext.Instance((Action<ContactsContext>) (cdb => participants = ConversationHelper.GetParticipantsImpl(cdb, convo, excludeSelf, sortByFirstName, firstNToGet, filter)));
      }));
      return participants ?? new UserStatus[0];
    }

    public static UserStatus[] GetParticipants(
      this Conversation convo,
      bool excludeSelf,
      bool sortByFirstName,
      int firstNToGet = 0,
      Func<UserStatus, bool> filter = null)
    {
      UserStatus[] participants = (UserStatus[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (mdb => ContactsContext.Instance((Action<ContactsContext>) (cdb => participants = ConversationHelper.GetParticipantsImpl(cdb, convo, excludeSelf, sortByFirstName, firstNToGet, filter)))));
      return participants ?? new UserStatus[0];
    }

    private static UserStatus[] GetParticipantsImpl(
      ContactsContext cdb,
      Conversation convo,
      bool excludeSelf,
      bool sortByFirstName,
      int firstNToGet = 0,
      Func<UserStatus, bool> filter = null)
    {
      LinkedList<UserStatus> source = new LinkedList<UserStatus>();
      string[] participantJids = convo.GetParticipantJids();
      List<UserStatus> userStatuses = cdb.GetUserStatuses((IEnumerable<string>) participantJids, false, false);
      ParticipantSort.Sort(userStatuses, sortByFirstName, true);
      int num = 0;
      Dictionary<string, bool> coveredJids = new Dictionary<string, bool>();
      string myJid = excludeSelf ? Settings.MyJid : (string) null;
      foreach (UserStatus userStatus in userStatuses)
      {
        coveredJids.Add(userStatus.Jid, true);
        if ((filter == null || filter(userStatus)) && userStatus.Jid != myJid)
        {
          source.AddLast(userStatus);
          ++num;
          if (firstNToGet > 0 && num >= firstNToGet)
            return source.ToArray<UserStatus>();
        }
      }
      foreach (string jid in ((IEnumerable<string>) participantJids).Where<string>((Func<string, bool>) (jid => !coveredJids.ContainsKey(jid))))
      {
        UserStatus userStatus = cdb.GetUserStatus(jid);
        if (userStatus != null && (filter == null || filter(userStatus)) && userStatus.Jid != myJid)
        {
          source.AddLast(userStatus);
          ++num;
          if (firstNToGet > 0)
          {
            if (num >= firstNToGet)
              break;
          }
        }
      }
      return source.ToArray<UserStatus>();
    }

    public static string GetParticipantNames(this Conversation convo, int firstNToGet = 0)
    {
      Func<UserStatus, bool> filter = (Func<UserStatus, bool>) null;
      List<UserStatus> list1 = ((IEnumerable<UserStatus>) convo.GetParticipants(true, true, firstNToGet, filter)).ToList<UserStatus>();
      if (list1.Count == 0)
        return (string) null;
      List<string> list2 = list1.Select<UserStatus, string>((Func<UserStatus, string>) (user => user.GetDisplayName(true))).ToList<string>();
      if (JidHelper.IsGroupJid(convo.Jid) && list2.Count < firstNToGet && convo.ContainsParticipant(Settings.MyJid))
        list2.Add(AppResources.You);
      string participantNames = Utils.CommaSeparate((IEnumerable<string>) list2);
      if (list2.Count < convo.GetParticipantCount())
        participantNames += " …";
      return participantNames;
    }

    public static bool IsInDeleting(this Conversation convo)
    {
      Conversation.ConversationStatus? status = convo.Status;
      Conversation.ConversationStatus conversationStatus = Conversation.ConversationStatus.Deleting;
      return (status.GetValueOrDefault() == conversationStatus ? (status.HasValue ? 1 : 0) : 0) != 0 && (!convo.EffectiveFirstMessageID.HasValue || !convo.LastMessageID.HasValue || convo.LastMessageID.Value < convo.EffectiveFirstMessageID.Value);
    }

    public static bool IsInClearing(this Conversation convo)
    {
      return convo.Status.HasValue && convo.Status.Value == Conversation.ConversationStatus.Clearing && convo.EffectiveFirstMessageID.HasValue;
    }

    public static Message[] GetLatestMessages(
      this Conversation c,
      MessagesContext db,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      FunXMPP.FMessage.Type? mediaType = null)
    {
      int? loadingStart = c.MessageLoadingStart();
      return db.GetLatestMessages(c.Jid, loadingStart, limit, offset, includeMiscInfo, mediaType);
    }

    public static Message[] GetMessagesFromStart(
      this Conversation c,
      MessagesContext db,
      int? limit,
      int? offset,
      bool includeMiscInfo = false)
    {
      int? loadingStart = c.MessageLoadingStart();
      return db.GetMessagesAfter(c.Jid, loadingStart, new int?(), false, limit, offset, includeMiscInfo);
    }

    public static bool UpdateParticipants(
      this Conversation c,
      SqliteMessagesContext db,
      string[] participants)
    {
      Dictionary<string, bool> toAdd = new Dictionary<string, bool>();
      List<string> toRemove = new List<string>();
      c.ParticipantSetAction((Action<GroupParticipants>) (currentParticipants =>
      {
        Set<string> set = new Set<string>();
        foreach (string participant in participants)
        {
          if (currentParticipants.ContainsKey(participant))
            set.Add(participant);
          else
            toAdd[participant] = false;
        }
        foreach (string key in currentParticipants.Keys)
        {
          if (!set.Contains(key))
            toRemove.Add(key);
        }
      }));
      return c.UpdateParticipants(db, toAdd, (IEnumerable<string>) toRemove);
    }

    public static void SetDraft(MessagesContext db, string jid, string draft)
    {
      Log.l("convo helper", "set draft | jid:{0},draft:{1}", (object) jid, (object) draft);
      CreateResult result = CreateResult.None;
      Conversation conversation = db.GetConversation(jid, CreateOptions.CreateToDbIfNotFound, out result);
      bool flag = (result & CreateResult.CreatedToDb) != 0;
      if (conversation.ComposingText != draft)
      {
        conversation.ComposingText = draft;
        flag = true;
      }
      if (!flag)
        return;
      db.SubmitChanges();
    }
  }
}
