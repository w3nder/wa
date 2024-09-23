// Decompiled with JetBrains decompiler
// Type: WhatsApp.SystemMessageUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class SystemMessageUtils
  {
    private const string LogHdr = "sysmsgu";
    public const string contentIdForVerifiedLearnMoreUrl = "26000089";

    public static Message CreateParticipantChanged(
      MessagesContext db,
      SystemMessageUtils.ParticipantChange changeType,
      string chatJid,
      string participantJid,
      string authorJid,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 0);
      binaryData.AppendByte((byte) changeType);
      if (!string.IsNullOrEmpty(authorJid))
      {
        byte length = (byte) authorJid.Length;
        binaryData.AppendByte(length);
        binaryData.AppendBytes(((IEnumerable<byte>) Encoding.UTF8.GetBytes(authorJid)).Take<byte>((int) length));
      }
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, participantJid, binaryData.Get(), dt);
    }

    public static Message CreateSubjectChanged(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      string newSubject,
      DateTime dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(newSubject));
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), new DateTime?(dtUtc));
    }

    public static Message CreateGroupDescriptionChanged(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 20);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), new DateTime?(dtUtc));
    }

    public static Message CreateGroupDescriptionDeleted(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 28);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), new DateTime?(dtUtc));
    }

    public static Message CreateGroupRestrictionUnlocked(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 24);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateGroupRestrictionLocked(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 23);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateGroupMadeAnnouncementOnly(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 25);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateGroupMadeNotAnnouncementOnly(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 26);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateGroupPictureChanged(
      MessagesContext db,
      string chatJid,
      string paticipantJid,
      bool isPhotoDeleted)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 2);
      binaryData.AppendByte(isPhotoDeleted ? (byte) 0 : (byte) 1);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, paticipantJid, binaryData.Get());
    }

    public static Message CreateWaNumberChanged(
      MessagesContext db,
      string chatJid,
      string newJid,
      string oldJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 3);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(oldJid));
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, newJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateGroupParticipantNumberChanged(
      MessagesContext db,
      string chatJid,
      string newJid,
      string oldJid,
      DateTime? dtUtc)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 29);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(oldJid));
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, newJid, binaryData.Get(), dtUtc);
    }

    public static Message CreateBroadcastListCreated(
      MessagesContext db,
      string blJid,
      int participantsCount)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 4);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(participantsCount.ToString()));
      return Message.CreateSystemMessage((SqliteMessagesContext) db, blJid, (string) null, binaryData.Get());
    }

    public static Message CreateErrorMessage(
      MessagesContext db,
      string chatJid,
      MessageMiscInfo.MessageError errType)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 5);
      binaryData.AppendByte((byte) errType);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, (string) null, binaryData.Get());
    }

    public static Message CreateGroupAdminGained(
      MessagesContext db,
      string groupJid,
      string participantJid,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 6);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, groupJid, participantJid, binaryData.Get(), dt);
    }

    public static Message CreateGroupAdminLost(
      MessagesContext db,
      string groupJid,
      string participantJid,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 27);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, groupJid, participantJid, binaryData.Get(), dt);
    }

    public static Message CreateGroupInviteChanged(
      MessagesContext db,
      string groupJid,
      string authorJid,
      string newCode,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 13);
      binaryData.AppendStrWithLengthPrefix(newCode);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, groupJid, authorJid, binaryData.Get(), dt);
    }

    public static Message CreateGroupDeleted(
      MessagesContext db,
      string groupJid,
      string authorJid,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 7);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, groupJid, authorJid, binaryData.Get(), dt);
    }

    public static Message CreateGroupCreated(
      MessagesContext db,
      string groupJid,
      string creatorJid,
      string groupSubject,
      DateTime? dt)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 8);
      binaryData.AppendBytes((IEnumerable<byte>) Encoding.UTF8.GetBytes(groupSubject));
      return Message.CreateSystemMessage((SqliteMessagesContext) db, groupJid, creatorJid, binaryData.Get(), dt);
    }

    public static Message CreateIdentityChanged(
      MessagesContext db,
      string chatJid,
      string participantJid)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 9);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, participantJid, binaryData.Get());
    }

    public static Message CreateConversationEncrypted(MessagesContext db, string chatJid)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 10);
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, (string) null, binaryData.Get());
    }

    public static Message CreateMissedCall(
      SqliteMessagesContext db,
      string callerJid,
      DateTime dtUtc,
      bool video)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte(video ? (byte) 12 : (byte) 11);
      binaryData.AppendLong64(DateTimeUtils.SanitizeTimestamp(dtUtc.ToUnixTime()));
      binaryData.AppendStrWithLengthPrefix(callerJid);
      return Message.CreateSystemMessage(db, callerJid, (string) null, binaryData.Get());
    }

    public static Message MaybeCreateOneTimeBizSystemMessage2Tier(
      MessagesContext db,
      string chatJid,
      string verifiedName,
      VerifiedLevel verifiedLevel,
      VerifiedTier verifiedTier)
    {
      Message systemMessage2Tier = (Message) null;
      if (verifiedLevel == VerifiedLevel.high || verifiedLevel == VerifiedLevel.low)
      {
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 32);
        binaryData.AppendStrWithLengthPrefix(verifiedName ?? "");
        binaryData.AppendInt32((int) verifiedLevel);
        binaryData.AppendInt32((int) verifiedTier);
        systemMessage2Tier = Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, (string) null, binaryData.Get());
      }
      return systemMessage2Tier;
    }

    public static bool TryGenerateInitialBizSystemMessage2Tier(
      SqliteMessagesContext mdb,
      string jid,
      bool submitChanges)
    {
      bool systemMessage2Tier = false;
      if (!JidHelper.IsUserJid(jid))
        return systemMessage2Tier;
      UserStatus user = JidHelper.IsUserJid(jid) ? UserCache.Get(jid, false) : (UserStatus) null;
      if (user != null)
      {
        Triad<VerifiedLevel, string, VerifiedTier> verifiedStateForDisplay = user.GetVerifiedStateForDisplay();
        VerifiedLevel verifiedLevel = verifiedStateForDisplay.First;
        if (verifiedLevel == VerifiedLevel.NotApplicable)
        {
          Log.l("sysmsgu", "skip generating initial biz sys msg | jid:{0},level:{1}", (object) jid, (object) verifiedLevel);
        }
        else
        {
          string verifiedName = verifiedStateForDisplay.Second;
          VerifiedTier verifiedTier = verifiedStateForDisplay.Third;
          Log.l("sysmsgu", "generating initial biz sys msg | jid:{0},level:{1},name:{2},tier:{3}", (object) jid, (object) verifiedLevel, (object) verifiedName, (object) verifiedTier);
          bool inPhoneBook = true;
          bool nameMatchesPhoneBook = true;
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            UserStatus userStatus = cdb.GetUserStatus(jid);
            UserStatusProperties forUserStatus = UserStatusProperties.GetForUserStatus(userStatus);
            forUserStatus.EnsureBusinessUserProperties.LastDisplayedVerifiedLevel = new int?((int) verifiedLevel);
            forUserStatus.EnsureBusinessUserProperties.LastDisplayedVerifiedName = verifiedName;
            forUserStatus.EnsureBusinessUserProperties.LastDisplayedTier = new int?((int) verifiedTier);
            forUserStatus.Save();
            cdb.SubmitChanges();
            inPhoneBook = userStatus != null && userStatus.IsInDevicePhonebook;
            nameMatchesPhoneBook = userStatus != null && userStatus.VerifiedNameMatchesContactName();
          }));
          if (mdb.GetConversation(jid, CreateOptions.None) != null)
          {
            Message verifiedBizInitial2Tier = SystemMessageUtils.CreateVerifiedBizInitial2Tier(mdb, jid, verifiedLevel, verifiedName, verifiedTier, inPhoneBook, nameMatchesPhoneBook);
            if (verifiedBizInitial2Tier != null)
            {
              mdb.InsertMessageOnSubmit(verifiedBizInitial2Tier);
              if (submitChanges)
                mdb.SubmitChanges();
              systemMessage2Tier = true;
              Log.l("sysmsgu", "generated initial biz sys msg | jid:{0},v level:{1},v name:{2},tier:{3},in:{4}", (object) jid, (object) verifiedLevel, (object) verifiedName, (object) verifiedTier, (object) inPhoneBook);
            }
            else
              Log.d("sysmsgu", "skip biz sys msg | sys msg not created | jid:{0}", (object) jid);
          }
          else
            Log.d("sysmsgu", "skip biz sys msg | convo not found | jid:{0}", (object) jid);
        }
      }
      return systemMessage2Tier;
    }

    public static WaScheduledTask CreateGenerateTransitBizSystemMessageTask(
      string jid,
      VerifiedLevel verifiedLevel,
      string verifiedName,
      VerifiedLevel prevVerifiedLevel,
      string prevVerifiedName,
      bool inPhonebook,
      bool nameMatchesPhonebook,
      VerifiedTier currTier,
      VerifiedTier prevTier)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32((int) verifiedLevel);
      binaryData.AppendInt32((int) prevVerifiedLevel);
      binaryData.AppendStrWithLengthPrefix(verifiedName ?? "");
      binaryData.AppendStrWithLengthPrefix(prevVerifiedName ?? "");
      binaryData.AppendByte(inPhonebook ? (byte) 1 : (byte) 0);
      binaryData.AppendByte(nameMatchesPhonebook ? (byte) 1 : (byte) 0);
      binaryData.AppendInt32((int) currTier);
      binaryData.AppendInt32((int) prevTier);
      return new WaScheduledTask(WaScheduledTask.Types.GenerateTransitBizSystemMessage, jid, binaryData.Get(), WaScheduledTask.Restrictions.None, new TimeSpan?(TimeSpan.FromDays(7.0)));
    }

    public static IObservable<Unit> PerformGenerateTransitBizSystemMessage(WaScheduledTask task)
    {
      return task.TaskType != 1001 ? Observable.Empty<Unit>() : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        string jid = task.LookupKey;
        BinaryData binaryData = new BinaryData(task.BinaryData);
        int offset1 = 0;
        VerifiedLevel verifiedLevel = (VerifiedLevel) binaryData.ReadInt32(offset1);
        int offset2 = offset1 + 4;
        VerifiedLevel prevVerifiedLevel = (VerifiedLevel) binaryData.ReadInt32(offset2);
        int newOffset = offset2 + 4;
        string verifiedName = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
        string prevVerifiedName = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
        bool inPhonebook = false;
        bool nameMatchesPhonebook = false;
        VerifiedTier prevTier = VerifiedNamesCertifier.ConvertVerifiedLevel(prevVerifiedLevel);
        VerifiedTier currTier = VerifiedNamesCertifier.ConvertVerifiedLevel(verifiedLevel);
        if (newOffset <= binaryData.Length() + 10)
        {
          inPhonebook = binaryData.ReadByte(newOffset) == (byte) 1;
          int offset3 = newOffset + 1;
          nameMatchesPhonebook = binaryData.ReadByte(offset3) == (byte) 1;
          int offset4 = offset3 + 1;
          currTier = (VerifiedTier) binaryData.ReadInt32(offset4);
          int offset5 = offset4 + 4;
          prevTier = (VerifiedTier) binaryData.ReadInt32(offset5);
          int num = offset5 + 4;
        }
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (db.GetConversation(jid, CreateOptions.None) != null)
          {
            Message m;
            if (db.AnyMessages(jid))
            {
              m = SystemMessageUtils.CreateVerifiedBizTransit2Tier((SqliteMessagesContext) db, jid, verifiedLevel, verifiedName, prevVerifiedLevel, prevVerifiedName, inPhonebook, nameMatchesPhonebook, currTier, prevTier);
              if (m != null)
                Log.l("sysmsgu", "generated transit biz sys msg | jid:{0},v level:{1}->{2},v name:{3}->{4}", (object) jid, (object) prevVerifiedLevel, (object) verifiedLevel, (object) prevVerifiedName, (object) verifiedName);
            }
            else
            {
              bool inPhoneBook = false;
              ContactsContext.Instance((Action<ContactsContext>) (cdb =>
              {
                UserStatus userStatus = cdb.GetUserStatus(jid);
                inPhoneBook = userStatus != null && userStatus.IsInDevicePhonebook;
                nameMatchesPhonebook = userStatus != null && userStatus.VerifiedNameMatchesContactName();
              }));
              m = SystemMessageUtils.CreateVerifiedBizInitial2Tier((SqliteMessagesContext) db, jid, verifiedLevel, verifiedName, currTier, inPhoneBook, nameMatchesPhonebook);
              if (m != null)
                Log.l("sysmsgu", "generated initial biz sys msg | jid:{0},v level:{1},v name:{2},tier:{3},in:{4}", (object) jid, (object) verifiedLevel, (object) verifiedName, (object) currTier, (object) inPhoneBook);
            }
            if (m != null)
            {
              db.InsertMessageOnSubmit(m);
              db.SubmitChanges();
            }
            else
              Log.d("sysmsgu", "skipped generating biz sys msg | jid:{0}", (object) jid);
          }
          else
            Log.d("sysmsgu", "skip biz sys msg | convo not found | jid:{0}", (object) jid);
        }));
        observer.OnNext(new Unit());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static Message CreateVerifiedBizInitial2Tier(
      SqliteMessagesContext db,
      string chatJid,
      VerifiedLevel verifiedLevel,
      string verifiedName,
      VerifiedTier verifiedTier,
      bool inPhoneBoook,
      bool nameMatchesPhonebook)
    {
      if (verifiedLevel == VerifiedLevel.NotApplicable)
        return (Message) null;
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 30);
      binaryData.AppendInt32((int) verifiedLevel);
      binaryData.AppendStrWithLengthPrefix(verifiedName ?? "");
      binaryData.AppendInt32((int) verifiedTier);
      binaryData.AppendByte(inPhoneBoook ? (byte) 1 : (byte) 0);
      binaryData.AppendByte(nameMatchesPhonebook ? (byte) 1 : (byte) 0);
      return Message.CreateSystemMessage(db, chatJid, (string) null, binaryData.Get());
    }

    internal static Triad<VerifiedLevel, string, VerifiedTier> GetDataFromVerifiedBizInitialSysMsg2Tier(
      Message msg,
      out bool inPhoneBoook,
      out bool nameMatchesPhonebook)
    {
      inPhoneBoook = false;
      nameMatchesPhonebook = false;
      Triad<VerifiedLevel, string, VerifiedTier> initialSysMsg2Tier = (Triad<VerifiedLevel, string, VerifiedTier>) null;
      if (msg != null)
      {
        BinaryData binaryData = new BinaryData(msg.BinaryData);
        int offset = 1;
        int num = binaryData.ReadInt32(offset);
        int newOffset = offset + 4;
        string second = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
        VerifiedTier third = VerifiedNamesCertifier.ConvertVerifiedLevel((VerifiedLevel) num);
        if (newOffset <= binaryData.Length() + 6)
        {
          third = (VerifiedTier) binaryData.ReadInt32(newOffset);
          newOffset += 4;
          inPhoneBoook = binaryData.ReadByte(newOffset) == (byte) 1;
          ++newOffset;
          nameMatchesPhonebook = binaryData.ReadByte(newOffset) == (byte) 1;
          ++newOffset;
        }
        initialSysMsg2Tier = new Triad<VerifiedLevel, string, VerifiedTier>((VerifiedLevel) num, second, third);
      }
      return initialSysMsg2Tier;
    }

    public static Pair<string, string> GetPopupMessageForVerifiedBizInitial2Tier(Message msg)
    {
      bool inPhoneBoook = false;
      bool nameMatchesPhonebook = false;
      Triad<VerifiedLevel, string, VerifiedTier> initialSysMsg2Tier = SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg2Tier(msg, out inPhoneBoook, out nameMatchesPhonebook);
      return SystemMessageUtils.GetPopupMessageForVerifiedState2Tier(msg.KeyRemoteJid, initialSysMsg2Tier != null ? initialSysMsg2Tier.First : VerifiedLevel.NotApplicable, initialSysMsg2Tier?.Second ?? "", initialSysMsg2Tier != null ? initialSysMsg2Tier.Third : VerifiedTier.NotApplicable, inPhoneBoook, nameMatchesPhonebook);
    }

    public static Triad<VerifiedLevel, string, VerifiedTier> GetDataForVerifiedBizInitialSysMsg2Tier(
      Message msg,
      out bool inPhoneBook,
      out bool nameMatchesPhonebook)
    {
      Triad<VerifiedLevel, string, VerifiedTier> initialSysMsg2Tier = SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg2Tier(msg, out inPhoneBook, out nameMatchesPhonebook);
      UserStatus user = UserCache.Get(msg.KeyRemoteJid, false);
      inPhoneBook = user != null && user.IsInDevicePhonebook;
      nameMatchesPhonebook = user != null && user.VerifiedNameMatchesContactName();
      return initialSysMsg2Tier;
    }

    public static Pair<string, string> GetPopupMessageForVerifiedState2Tier(
      string jid,
      VerifiedLevel verifiedLevel,
      string verifiedName,
      VerifiedTier verfiedTier,
      bool inPhonebook,
      bool nameMatchesPhonebook)
    {
      Pair<string, string> verifiedState2Tier = (Pair<string, string>) null;
      string first = (string) null;
      switch (verfiedTier)
      {
        case VerifiedTier.Bottom:
          first = string.Format(AppResources.VerifiedAlertInitialBottom, (object) UserCache.GetCacheOnly(jid)?.GetDisplayName());
          break;
        case VerifiedTier.Top:
          first = !inPhonebook ? string.Format(AppResources.VerifiedAlertInitialTopNotIn, (object) verifiedName) : (!nameMatchesPhonebook ? string.Format(AppResources.VerifiedAlertInitialTopInDiffName, (object) verifiedName) : string.Format(AppResources.VerifiedAlertInitialTopIn, (object) verifiedName));
          break;
      }
      if (!string.IsNullOrEmpty(first))
        verifiedState2Tier = new Pair<string, string>(first, "26000089");
      return verifiedState2Tier;
    }

    public static void GetDataFromVerifiedBizInitialSysMsg(
      Message msg,
      out Pair<VerifiedLevel, string> verifiedState)
    {
      verifiedState = (Pair<VerifiedLevel, string>) null;
      if (msg == null)
        return;
      BinaryData binaryData = new BinaryData(msg.BinaryData);
      int offset = 1;
      VerifiedLevel first = (VerifiedLevel) binaryData.ReadInt32(offset);
      string second = binaryData.ReadStrWithLengthPrefix(offset + 4);
      verifiedState = new Pair<VerifiedLevel, string>(first, second);
    }

    public static Pair<string, string> GetPopupMessageForVerifiedBizInitial(Message msg)
    {
      Pair<VerifiedLevel, string> verifiedState = (Pair<VerifiedLevel, string>) null;
      SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg(msg, out verifiedState);
      return SystemMessageUtils.GetPopupMessageForVerifiedState(msg.KeyRemoteJid, verifiedState != null ? verifiedState.First : VerifiedLevel.NotApplicable, verifiedState?.Second ?? "");
    }

    public static Pair<string, string> GetPopupMessageForVerifiedState(
      string jid,
      VerifiedLevel verifiedLevel,
      string verifiedName)
    {
      Pair<string, string> forVerifiedState = (Pair<string, string>) null;
      string first = (string) null;
      string second = "26000089";
      switch (verifiedLevel)
      {
        case VerifiedLevel.unknown:
          first = string.Format(AppResources.VerifiedAlertInitialUnknown, (object) (UserCache.Get(jid, true).GetDisplayName() ?? ""));
          break;
        case VerifiedLevel.low:
          first = string.Format(AppResources.VerifiedAlertInitialLow, (object) verifiedName);
          break;
        case VerifiedLevel.high:
          first = string.Format(AppResources.VerifiedAlertInitialHigh, (object) verifiedName);
          break;
      }
      if (!string.IsNullOrEmpty(first))
        forVerifiedState = new Pair<string, string>(first, second);
      return forVerifiedState;
    }

    public static Message CreateVerifiedBizTransit2Tier(
      SqliteMessagesContext db,
      string chatJid,
      VerifiedLevel verifiedLevel,
      string verifiedName,
      VerifiedLevel prevVerifiedLevel,
      string prevVerifiedName,
      bool inPhonebook,
      bool nameMatchedPhonebook,
      VerifiedTier currTier,
      VerifiedTier prevTier)
    {
      Message verifiedBizTransit2Tier = (Message) null;
      if (currTier != prevTier || currTier == VerifiedTier.Top && verifiedName != prevVerifiedName)
      {
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 31);
        binaryData.AppendInt32((int) verifiedLevel);
        binaryData.AppendInt32((int) prevVerifiedLevel);
        binaryData.AppendStrWithLengthPrefix(verifiedName);
        binaryData.AppendStrWithLengthPrefix(prevVerifiedName);
        binaryData.AppendByte(inPhonebook ? (byte) 1 : (byte) 0);
        binaryData.AppendByte(nameMatchedPhonebook ? (byte) 1 : (byte) 0);
        binaryData.AppendInt32((int) currTier);
        binaryData.AppendInt32((int) prevTier);
        verifiedBizTransit2Tier = Message.CreateSystemMessage(db, chatJid, (string) null, binaryData.Get());
      }
      return verifiedBizTransit2Tier;
    }

    public static void GetDataFromVerifiedBizTransitSysMsg(
      Message msg,
      out Pair<VerifiedLevel, string> verifiedState,
      out Pair<VerifiedLevel, string> prevVerifiedState)
    {
      verifiedState = (Pair<VerifiedLevel, string>) null;
      prevVerifiedState = (Pair<VerifiedLevel, string>) null;
      if (msg == null)
        return;
      BinaryData binaryData = new BinaryData(msg.BinaryData);
      int offset1 = 1;
      VerifiedLevel first1 = (VerifiedLevel) binaryData.ReadInt32(offset1);
      int offset2 = offset1 + 4;
      VerifiedLevel first2 = (VerifiedLevel) binaryData.ReadInt32(offset2);
      int newOffset = offset2 + 4;
      string second1 = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      string second2 = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      verifiedState = new Pair<VerifiedLevel, string>(first1, second1);
      prevVerifiedState = new Pair<VerifiedLevel, string>(first2, second2);
    }

    public static void GetDataFromVerifiedBizTransitSysMsgTier2(
      Message msg,
      out Triad<VerifiedLevel, string, VerifiedTier> verifiedState,
      out Triad<VerifiedLevel, string, VerifiedTier> prevVerifiedState,
      out bool inPhonebook,
      out bool nameMatchesPhonebook)
    {
      verifiedState = (Triad<VerifiedLevel, string, VerifiedTier>) null;
      prevVerifiedState = (Triad<VerifiedLevel, string, VerifiedTier>) null;
      inPhonebook = false;
      nameMatchesPhonebook = false;
      if (msg == null)
        return;
      BinaryData binaryData = new BinaryData(msg.BinaryData);
      int offset1 = 1;
      VerifiedLevel verifiedLevel1 = (VerifiedLevel) binaryData.ReadInt32(offset1);
      int offset2 = offset1 + 4;
      VerifiedLevel verifiedLevel2 = (VerifiedLevel) binaryData.ReadInt32(offset2);
      int newOffset = offset2 + 4;
      string second1 = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      string second2 = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      VerifiedTier third1 = VerifiedNamesCertifier.ConvertVerifiedLevel(verifiedLevel1);
      VerifiedTier third2 = VerifiedNamesCertifier.ConvertVerifiedLevel(verifiedLevel2);
      if (newOffset <= binaryData.Length() - 10)
      {
        inPhonebook = binaryData.ReadByte(newOffset) == (byte) 1;
        ++newOffset;
        nameMatchesPhonebook = binaryData.ReadByte(newOffset) == (byte) 1;
        ++newOffset;
        third1 = (VerifiedTier) binaryData.ReadInt32(newOffset);
        newOffset += 4;
        third2 = (VerifiedTier) binaryData.ReadInt32(newOffset);
        newOffset += 4;
      }
      verifiedState = new Triad<VerifiedLevel, string, VerifiedTier>(verifiedLevel1, second1, third1);
      prevVerifiedState = new Triad<VerifiedLevel, string, VerifiedTier>(verifiedLevel2, second2, third2);
    }

    public static Pair<string, string> GetPopupMessageForVerifiedBizTransit2Tier(Message msg)
    {
      bool inPhonebook = false;
      bool nameMatchesPhonebook = false;
      Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string> triad = SystemMessageUtils.DetectChange(msg, out inPhonebook, out nameMatchesPhonebook);
      string first1 = (string) null;
      SystemMessageUtils.TransistionToDisplay2Tier first2 = triad.First;
      string second = triad.Second;
      string third = triad.Third;
      switch (first2)
      {
        case SystemMessageUtils.TransistionToDisplay2Tier.TopToBottom:
          first1 = string.Format(AppResources.VerifiedAlertChangedToBottom, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.TopToConsumer:
        case SystemMessageUtils.TransistionToDisplay2Tier.BottomToConsumer:
          first1 = AppResources.VerifiedAlertChangedToConsumer;
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.BottomToTop:
          first1 = string.Format(AppResources.VerifiedAlertChangedToTop, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToTop:
          first1 = !inPhonebook ? string.Format(AppResources.VerifiedAlertInitialTopNotIn, (object) second) : (!nameMatchesPhonebook ? string.Format(AppResources.VerifiedAlertInitialTopInDiffName, (object) second) : string.Format(AppResources.VerifiedAlertInitialTopIn, (object) second));
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToBottom:
          first1 = string.Format(AppResources.VerifiedAlertInitialBottom, (object) third);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.NameChange:
          first1 = string.Format(AppResources.VerifiedAlertChangedName, (object) second);
          break;
        default:
          Log.l("sysmsgu", "No popup message for transition system message");
          break;
      }
      Pair<string, string> verifiedBizTransit2Tier = (Pair<string, string>) null;
      if (!string.IsNullOrEmpty(first1))
        verifiedBizTransit2Tier = new Pair<string, string>(first1, "26000089");
      return verifiedBizTransit2Tier;
    }

    public static Triad<VerifiedLevel, string, VerifiedTier> GetDataFromVerifiedBizOneTimeSysMsgTier2(
      Message msg)
    {
      if (msg == null)
        return (Triad<VerifiedLevel, string, VerifiedTier>) null;
      BinaryData binaryData = new BinaryData(msg.BinaryData);
      int newOffset = 1;
      string second = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      VerifiedLevel first = (VerifiedLevel) binaryData.ReadInt32(newOffset);
      int offset = newOffset + 4;
      VerifiedTier third = (VerifiedTier) binaryData.ReadInt32(offset);
      int num = offset + 4;
      return new Triad<VerifiedLevel, string, VerifiedTier>(first, second, third);
    }

    public static Pair<string, string> GetPopupMessageForVerifiedBizOneTime2Tier(Message msg)
    {
      Triad<VerifiedLevel, string, VerifiedTier> oneTimeSysMsgTier2 = SystemMessageUtils.GetDataFromVerifiedBizOneTimeSysMsgTier2(msg);
      if (oneTimeSysMsgTier2 == null)
      {
        Log.l("sysmsgu", "No data for one time system message");
        return (Pair<string, string>) null;
      }
      int first1 = (int) oneTimeSysMsgTier2.First;
      string second = oneTimeSysMsgTier2.Second;
      string first2 = (string) null;
      switch (oneTimeSysMsgTier2.First)
      {
        case VerifiedLevel.low:
          first2 = string.Format(AppResources.VerifiedAlertOneTimeLow, (object) second);
          break;
        case VerifiedLevel.high:
          first2 = string.Format(AppResources.VerifiedAlertOneTimeHigh, (object) second);
          break;
        default:
          Log.l("sysmsgu", "No popup message for one time system message");
          break;
      }
      Pair<string, string> verifiedBizOneTime2Tier = (Pair<string, string>) null;
      if (!string.IsNullOrEmpty(first2))
        verifiedBizOneTime2Tier = new Pair<string, string>(first2, "26000089");
      return verifiedBizOneTime2Tier;
    }

    public static Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string> DetectChange(
      Message msg,
      out bool inPhonebook,
      out bool nameMatchesPhonebook)
    {
      Triad<VerifiedLevel, string, VerifiedTier> verifiedState = (Triad<VerifiedLevel, string, VerifiedTier>) null;
      Triad<VerifiedLevel, string, VerifiedTier> prevVerifiedState = (Triad<VerifiedLevel, string, VerifiedTier>) null;
      SystemMessageUtils.GetDataFromVerifiedBizTransitSysMsgTier2(msg, out verifiedState, out prevVerifiedState, out inPhonebook, out nameMatchesPhonebook);
      UserStatus user = UserCache.Get(msg.KeyRemoteJid, false);
      inPhonebook = user != null && user.IsInDevicePhonebook;
      nameMatchesPhonebook = user != null && user.VerifiedNameMatchesContactName();
      string displayName = user?.GetDisplayName();
      if (verifiedState == null)
      {
        Log.l("sysmsgu", "New state is not registered!");
        return (Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string>) null;
      }
      VerifiedTier third = verifiedState.Third;
      string second1 = verifiedState.Second;
      VerifiedTier verifiedTier = prevVerifiedState != null ? prevVerifiedState.Third : VerifiedTier.NotApplicable;
      string str = prevVerifiedState?.Second ?? (string) null;
      string second2 = (string) null;
      SystemMessageUtils.TransistionToDisplay2Tier first = SystemMessageUtils.TransistionToDisplay2Tier.NoChange;
      if (third != verifiedTier)
      {
        if (third == VerifiedTier.Top && verifiedTier != VerifiedTier.Top)
        {
          second2 = second1;
          first = verifiedTier != VerifiedTier.Bottom ? SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToTop : SystemMessageUtils.TransistionToDisplay2Tier.BottomToTop;
        }
        else if (third == VerifiedTier.Bottom && verifiedTier != VerifiedTier.Bottom)
        {
          second2 = second1;
          first = verifiedTier != VerifiedTier.Top ? SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToBottom : SystemMessageUtils.TransistionToDisplay2Tier.TopToBottom;
        }
        else if (third == VerifiedTier.NotApplicable && verifiedTier != VerifiedTier.NotApplicable)
        {
          second2 = second1;
          first = verifiedTier != VerifiedTier.Top ? SystemMessageUtils.TransistionToDisplay2Tier.BottomToConsumer : SystemMessageUtils.TransistionToDisplay2Tier.TopToConsumer;
        }
        else
          Log.l("sysmsgu", "No system message for tier change {0}->{1}", (object) verifiedTier, (object) third);
      }
      if (third == VerifiedTier.Top && first == SystemMessageUtils.TransistionToDisplay2Tier.NoChange && second1 != str && str != null)
      {
        second2 = second1;
        first = SystemMessageUtils.TransistionToDisplay2Tier.NameChange;
      }
      if (first != SystemMessageUtils.TransistionToDisplay2Tier.NoChange)
        return new Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string>(first, second2, displayName);
      Log.l("sysmsgu", "No biz change detected for message");
      return (Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string>) null;
    }

    public static string GetChangeNumberText(Message msg)
    {
      if (msg.GetSystemMessageType() == SystemMessageWrapper.MessageTypes.Rename && msg.GetSystemMessageType() == SystemMessageWrapper.MessageTypes.GroupParticipantNumberChanged)
        throw new ArgumentException("not a change number message");
      if (msg == null)
        return (string) null;
      string keyRemoteJid = msg.KeyRemoteJid;
      string remoteResource = msg.RemoteResource;
      string oldJid = Encoding.UTF8.GetString(msg.BinaryData, 1, msg.BinaryData.Length - 1);
      string name = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(oldJid, false);
        if (userStatus != null)
          name = userStatus.GetDisplayName();
        else
          name = JidHelper.GetPhoneNumber(oldJid, true);
      }));
      string numberSystemMessage = AppResources.ChangeNumberSystemMessage;
      return NotificationString.RtlSafeFormat(AppResources.ChangeNumberSystemMessage, name);
    }

    public static Pair<string, string> GetPopupMessageForVerifiedBizTransit(Message msg)
    {
      Pair<string, string> verifiedBizTransit = (Pair<string, string>) null;
      Pair<VerifiedLevel, string> verifiedState = (Pair<VerifiedLevel, string>) null;
      Pair<VerifiedLevel, string> prevVerifiedState = (Pair<VerifiedLevel, string>) null;
      SystemMessageUtils.GetDataFromVerifiedBizTransitSysMsg(msg, out verifiedState, out prevVerifiedState);
      if (verifiedState == null || prevVerifiedState == null)
        return verifiedBizTransit;
      VerifiedLevel first1 = verifiedState.First;
      string second1 = verifiedState.Second;
      VerifiedLevel first2 = prevVerifiedState.First;
      string second2 = prevVerifiedState.Second;
      string first3 = (string) null;
      string second3 = "26000089";
      switch (first2)
      {
        case VerifiedLevel.NotApplicable:
          switch (first1)
          {
            case VerifiedLevel.unknown:
              first3 = string.Format(AppResources.VerifiedAlertConsumerToUnknown, (object) (UserCache.Get(msg.KeyRemoteJid, true).GetDisplayName() ?? ""));
              break;
            case VerifiedLevel.low:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeLow, (object) second1);
              break;
            case VerifiedLevel.high:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.unknown:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              first3 = AppResources.VerifiedAlertUnknownToConsumer;
              break;
            case VerifiedLevel.low:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeLow, (object) second1);
              break;
            case VerifiedLevel.high:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.low:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              first3 = AppResources.VerifiedAlertLowToConsumer;
              break;
            case VerifiedLevel.unknown:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeLow, (object) second2);
              break;
            case VerifiedLevel.high:
              first3 = string.Format(AppResources.VerifiedAlertOneTimeHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.high:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              first3 = AppResources.VerifiedAlertHighToConsumer;
              break;
            case VerifiedLevel.unknown:
              first3 = string.Format(AppResources.VerifiedAlertHighToUnknown, (object) second2);
              break;
            case VerifiedLevel.low:
              first3 = string.Format(AppResources.VerifiedAlertHighToLow, (object) second2);
              break;
          }
          break;
      }
      if (!string.IsNullOrEmpty(first3))
        verifiedBizTransit = new Pair<string, string>(first3, second3);
      return verifiedBizTransit;
    }

    public static Message CreateBizSystemMessage(
      MessagesContext db,
      string chatJid,
      SystemMessageWrapper.MessageTypes bizMessageType)
    {
      return Message.CreateSystemMessage((SqliteMessagesContext) db, chatJid, (string) null, SystemMessageUtils.CreateBizSystemMessageData(chatJid, bizMessageType));
    }

    private static byte[] CreateBizSystemMessageData(
      string chatJid,
      SystemMessageWrapper.MessageTypes bizMessageType)
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) bizMessageType);
      binaryData.AppendStrWithLengthPrefix(JidHelper.GetDisplayNameForContactJidNoNumber(chatJid));
      return binaryData.Get();
    }

    public static string ExtractVnameFromBizSystemMessageData(byte[] binaryData, string remoteJid)
    {
      BinaryData binaryData1 = new BinaryData(binaryData);
      int newOffset = 1;
      return newOffset + 4 > binaryData1.Length() ? JidHelper.GetDisplayNameForContactJidNoNumber(remoteJid) : binaryData1.ReadStrWithLengthPrefix(newOffset, out newOffset);
    }

    public static VerifiedLevel ExtractVlevelFromInitialBizSystemMessageData(byte[] binaryData)
    {
      return (VerifiedLevel) new BinaryData(binaryData).ReadInt32(1);
    }

    public static string ExtractVnameFromInitialBizSystemMessageData(
      byte[] binaryData,
      string remoteJid)
    {
      BinaryData binaryData1 = new BinaryData(binaryData);
      int newOffset = 1;
      return newOffset + 8 > binaryData1.Length() ? JidHelper.GetDisplayNameForContactJidNoNumber(remoteJid) : binaryData1.ReadStrWithLengthPrefix(newOffset + 4, out newOffset);
    }

    public static void ExtractVInfoFromTransitBizSystemMessageData(
      byte[] binaryData,
      out VerifiedLevel newLevel,
      out VerifiedLevel prevLevel,
      out string newvName)
    {
      BinaryData binaryData1 = new BinaryData(binaryData);
      int offset1 = 1;
      newLevel = (VerifiedLevel) binaryData1.ReadInt32(offset1);
      int offset2 = offset1 + 4;
      prevLevel = (VerifiedLevel) binaryData1.ReadInt32(offset2);
      int newOffset = offset2 + 4;
      newvName = binaryData1.ReadStrWithLengthPrefix(newOffset, out newOffset);
    }

    public static long ExtractMissedCallUnixTime(byte[] binaryData)
    {
      return new BinaryData(binaryData).ReadLong64(1);
    }

    public enum ParticipantChange
    {
      Leave,
      Join,
      Removed,
      Added,
      Invite,
    }

    public enum TransistionToDisplay2Tier
    {
      NoChange = 0,
      TopToBottom = 1,
      TopToConsumer = 2,
      BottomToTop = 3,
      BottomToConsumer = 4,
      ConsumerToTop = 5,
      ConsumerToBottom = 6,
      NameChange = 9,
    }
  }
}
