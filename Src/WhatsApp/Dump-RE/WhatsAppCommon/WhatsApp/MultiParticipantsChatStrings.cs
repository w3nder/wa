// Decompiled with JetBrains decompiler
// Type: WhatsApp.MultiParticipantsChatStrings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable
namespace WhatsApp
{
  public static class MultiParticipantsChatStrings
  {
    private static string FormatSystemMessageString(
      string jid,
      string pushName,
      string fromMeRes,
      string fromOtherRes,
      params object[] args)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(false, jid, pushName, fromMeRes, fromOtherRes, args);
    }

    private static string FormatSystemMessageString(
      bool reverseArgs,
      string jid,
      string pushName,
      string fromMeRes,
      string fromOtherRes,
      params object[] args)
    {
      int num = StringComparer.Ordinal.Equals(jid, Settings.MyJid) ? 1 : 0;
      List<object> objectList = new List<object>(args.Length + 1);
      if (num == 0)
      {
        string str = pushName ?? JidHelper.GetDisplayNameForContactJid(jid);
        objectList.Add((object) str);
      }
      objectList.AddRange((IEnumerable<object>) args);
      string fmt = num != 0 ? fromMeRes : fromOtherRes;
      if (reverseArgs)
        objectList.Reverse();
      return objectList.Count <= 0 ? fmt : MultiParticipantsChatStrings.RtlSafeFormat(fmt, objectList.ToArray());
    }

    public static string GetSystemMessage(
      this Message m,
      string participantJid = null,
      bool usePushName = false,
      bool actionable = false)
    {
      string systemMessage = (string) null;
      if (m.MediaWaType != FunXMPP.FMessage.Type.System)
        return systemMessage;
      if (m.Data != null)
        systemMessage = m.Data;
      else if (m.BinaryData != null && m.BinaryData.Length != 0)
      {
        if (participantJid == null)
          participantJid = m.GetSenderJid();
        string str = (string) null;
        if (usePushName)
        {
          string pushName = m.PushName;
          if (!string.IsNullOrEmpty(pushName))
            str = pushName;
        }
        try
        {
          switch (m.GetSystemMessageType())
          {
            case SystemMessageWrapper.MessageTypes.ParticipantChange:
              systemMessage = MultiParticipantsChatStrings.FormatParticipantChange(m.KeyRemoteJid, participantJid, m.GetSystemMessageChangeType(), m.GetSystemMessageChangeAuthor(), str);
              break;
            case SystemMessageWrapper.MessageTypes.SubjectChange:
              systemMessage = MultiParticipantsChatStrings.FormatSubjectChange(participantJid, str, m.GetSystemMessageNewSubject());
              break;
            case SystemMessageWrapper.MessageTypes.GroupPhotoChange:
              systemMessage = MultiParticipantsChatStrings.FormatPhotoChange(participantJid, str, m.GetSystemMessageHasPhoto());
              break;
            case SystemMessageWrapper.MessageTypes.BroadcastListCreated:
              systemMessage = MultiParticipantsChatStrings.FormatBroadcastListCreated(m);
              break;
            case SystemMessageWrapper.MessageTypes.Error:
              systemMessage = MultiParticipantsChatStrings.FormatError(MultiParticipantsChatStrings.GetErrorType(m), m.KeyRemoteJid);
              break;
            case SystemMessageWrapper.MessageTypes.GainedAdmin:
              systemMessage = MultiParticipantsChatStrings.FormatGainedAdmin(participantJid);
              break;
            case SystemMessageWrapper.MessageTypes.GroupDeleted:
              systemMessage = MultiParticipantsChatStrings.FormatGroupDeleted(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.GroupCreated:
              systemMessage = MultiParticipantsChatStrings.FormatGroupCreated(participantJid, str, m.GetSystemMessageNewSubject());
              break;
            case SystemMessageWrapper.MessageTypes.IdentityChanged:
              systemMessage = MultiParticipantsChatStrings.FormatIdentityChanged(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.ConvBizIsVerified:
              systemMessage = AppResources.VerifiedHighChat;
              break;
            case SystemMessageWrapper.MessageTypes.ConvBizIsUnVerified:
              systemMessage = AppResources.VerifiedLowUnknownChat;
              break;
            case SystemMessageWrapper.MessageTypes.ConvBizNowStandard:
              systemMessage = AppResources.VerifiedNowStandardTransition;
              break;
            case SystemMessageWrapper.MessageTypes.ConvBizNowUnverified:
              systemMessage = AppResources.VerifiedNowUnverifiedTransition;
              break;
            case SystemMessageWrapper.MessageTypes.ConvBizNowVerified:
              systemMessage = AppResources.VerifiedNowVerifiedTransition;
              break;
            case SystemMessageWrapper.MessageTypes.GroupDescriptionChanged:
              systemMessage = MultiParticipantsChatStrings.FormatGroupDescriptionChanged(participantJid, str, actionable);
              break;
            case SystemMessageWrapper.MessageTypes.GroupRestrictionLocked:
              systemMessage = MultiParticipantsChatStrings.FormatGroupAllowedOnlyAdmins(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.GroupRestrictionUnlocked:
              systemMessage = MultiParticipantsChatStrings.FormatGroupAllowedAllParticipants(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.GroupMadeAnnouncementOnly:
              systemMessage = MultiParticipantsChatStrings.FormatGroupMadeAnnouncementOnly(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.GroupMadeNotAnnouncementOnly:
              systemMessage = MultiParticipantsChatStrings.FormatGroupMadeNotAnnouncementOnly(participantJid, str);
              break;
            case SystemMessageWrapper.MessageTypes.LostAdmin:
              systemMessage = MultiParticipantsChatStrings.FormatLostAdmin(participantJid);
              break;
            case SystemMessageWrapper.MessageTypes.GroupDescriptionDeleted:
              systemMessage = MultiParticipantsChatStrings.FormatGroupDescriptionDeleted(participantJid, str);
              break;
            default:
              systemMessage = new SystemMessageWrapper(m).Get();
              break;
          }
        }
        catch (Exception ex)
        {
        }
      }
      return systemMessage;
    }

    public static MessageMiscInfo.MessageError GetErrorType(Message m)
    {
      return (MessageMiscInfo.MessageError) m.BinaryData[1];
    }

    public static SystemMessageUtils.ParticipantChange GetSystemMessageChangeType(this Message m)
    {
      return (SystemMessageUtils.ParticipantChange) m.BinaryData[1];
    }

    public static string GetSystemMessageChangeAuthor(this Message m)
    {
      if (m.BinaryData.Length < 3)
        return (string) null;
      byte count = m.BinaryData[2];
      return Encoding.UTF8.GetString(m.BinaryData, 3, (int) count);
    }

    public static string GetSystemMessageNewSubject(this Message m)
    {
      return Encoding.UTF8.GetString(m.BinaryData, 1, m.BinaryData.Length - 1);
    }

    public static bool GetSystemMessageIsGroupPhotoDeleted(this Message m)
    {
      return m.BinaryData[1] == (byte) 0;
    }

    public static bool GetSystemMessageHasPhoto(this Message m) => m.BinaryData[1] != (byte) 0;

    public static string GetSystemMessagePreviousJid(this Message m)
    {
      return Encoding.UTF8.GetString(m.BinaryData, 1, m.BinaryData.Length - 1);
    }

    private static void DecodeBroadcastListCreated(Message m, out int participantsCount)
    {
      participantsCount = 0;
      int.TryParse(Encoding.UTF8.GetString(m.BinaryData, 1, m.BinaryData.Length - 1), out participantsCount);
    }

    public static int GetSystemMessageBroadcastListCount(this Message m)
    {
      if (m.GetSystemMessageType() != SystemMessageWrapper.MessageTypes.BroadcastListCreated)
        return -1;
      int participantsCount;
      MultiParticipantsChatStrings.DecodeBroadcastListCreated(m, out participantsCount);
      return participantsCount;
    }

    public static string FormatError(MessageMiscInfo.MessageError errType, string remoteJid)
    {
      return errType.GetErrorString(remoteJid);
    }

    public static string FormatParticipantChange(
      string chatJid,
      string userJid,
      SystemMessageUtils.ParticipantChange type,
      string authorJid,
      string authorPushName)
    {
      string fromMeRes = (string) null;
      bool flag = true;
      List<object> objectList = new List<object>();
      if (authorJid != null && authorJid != Settings.MyJid)
      {
        objectList.Add((object) (authorPushName ?? JidHelper.GetDisplayNameForContactJid(authorJid)));
        flag = false;
      }
      string fromOtherRes;
      switch (type)
      {
        case SystemMessageUtils.ParticipantChange.Leave:
          if (authorJid == userJid)
          {
            fromMeRes = AppResources.YouLeftGroup;
            fromOtherRes = AppResources.LeftGroup;
            break;
          }
          fromMeRes = AppResources.YouRemovedFromGroup;
          fromOtherRes = AppResources.RemovedFromGroup;
          break;
        case SystemMessageUtils.ParticipantChange.Join:
        case SystemMessageUtils.ParticipantChange.Added:
          if (JidHelper.IsBroadcastJid(chatJid))
          {
            fromOtherRes = AppResources.AddedToList;
            break;
          }
          if (authorJid == userJid)
          {
            fromMeRes = AppResources.YouJoinedGroup;
            fromOtherRes = AppResources.JoinedGroup;
            break;
          }
          if (authorJid != null)
          {
            fromMeRes = flag ? AppResources.YouJoinedGroup : AppResources.UserAddedYou;
            fromOtherRes = flag ? AppResources.YouAddedUser : AppResources.UserAddedUser;
            break;
          }
          fromMeRes = AppResources.YouWereAdded;
          fromOtherRes = AppResources.UserWasAdded;
          break;
        case SystemMessageUtils.ParticipantChange.Removed:
          if (JidHelper.IsBroadcastJid(chatJid))
          {
            fromOtherRes = AppResources.RemovedFromList;
            break;
          }
          if (authorJid != null)
          {
            fromMeRes = flag ? AppResources.YouLeftGroup : AppResources.UserRemovedYou;
            fromOtherRes = flag ? AppResources.YouRemovedUser : AppResources.UserRemovedUser;
            break;
          }
          fromMeRes = AppResources.YouRemovedFromGroup;
          fromOtherRes = AppResources.RemovedFromGroup;
          break;
        case SystemMessageUtils.ParticipantChange.Invite:
          fromMeRes = AppResources.YouJoinedViaInviteLink;
          fromOtherRes = AppResources.JoinedViaInviteLink;
          break;
        default:
          throw new Exception("Unexpected change type");
      }
      return MultiParticipantsChatStrings.FormatSystemMessageString(!flag, userJid, (string) null, fromMeRes, fromOtherRes, objectList.ToArray());
    }

    public static string FormatSubjectChange(string jid, string pushName, string subject)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouChangedSubject, AppResources.ChangedSubject, (object) subject);
    }

    public static string FormatPhotoChange(string jid, string pushName, bool photo)
    {
      string fromMeRes;
      string fromOtherRes;
      if (!photo)
      {
        fromMeRes = AppResources.YouDeletedImage;
        fromOtherRes = AppResources.DeletedImage;
      }
      else
      {
        fromMeRes = AppResources.YouSetImage;
        fromOtherRes = AppResources.SetImage;
      }
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, fromMeRes, fromOtherRes);
    }

    public static string FormatBroadcastListCreated(Message msg)
    {
      int participantsCount = 0;
      MultiParticipantsChatStrings.DecodeBroadcastListCreated(msg, out participantsCount);
      return Plurals.Instance.GetString(AppResources.CreatedBroadcastListWithNRecipientsPlural, participantsCount);
    }

    public static string FormatGainedAdmin(string jid) => AppResources.YouAreNowAdmin;

    public static string FormatLostAdmin(string jid) => AppResources.YouAreNoLongerAdmin;

    public static string FormatGroupCreated(string jid, string pushName, string subject)
    {
      bool flag = !string.IsNullOrEmpty(subject);
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, flag ? AppResources.YouCreatedGroupWithSubject : AppResources.YouCreatedGroup, flag ? AppResources.UserCreatedGroupWithSubject : AppResources.UserCreatedGroup, (object) subject);
    }

    public static string FormatGroupDeleted(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouDeletedGroup, AppResources.UserDeletedGroup);
    }

    public static string FormatGroupDescriptionChanged(
      string jid,
      string pushName,
      bool actionable = false)
    {
      return actionable ? MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouChangedGroupDescriptionTappable, AppResources.UserChangedGroupDescriptionTappable) : MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouChangedGroupDescription, AppResources.UserChangedGroupDescription);
    }

    public static string FormatGroupDescriptionDeleted(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouDeletedGroupDescription, AppResources.UserDeletedGroupDescription);
    }

    public static string FormatIdentityChanged(string jid, string pushName, bool web = false)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, web ? AppResources.IdentityChangedSystemMessageWeb : AppResources.IdentityChangedSystemMessage, web ? AppResources.IdentityChangedSystemMessageWeb : AppResources.IdentityChangedSystemMessage);
    }

    public static string FormatConversationEncrypted(string jid, bool web = false)
    {
      return JidHelper.IsGroupJid(jid) ? (!web ? AppResources.GroupEnabledEncrypted : AppResources.GroupEnabledEncryptedWeb) : (JidHelper.IsBroadcastJid(jid) ? (!web ? AppResources.BroadcastEnabledEncrypted : AppResources.BroadcastEnabledEncryptedWeb) : (!web ? AppResources.ChatEnabledEncrypted : AppResources.ChatEnabledEncryptedWeb));
    }

    private static string RtlSafeFormat(string fmt, params object[] objs)
    {
      return NotificationString.RtlSafeFormat(fmt, ((IEnumerable<object>) objs).Select<object, string>((Func<object, string>) (a => a.ToString())).ToArray<string>());
    }

    public static string FormatGroupAllowedAllParticipants(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouAllowedAllParticipants, AppResources.UserAllowedAllParticipants);
    }

    public static string FormatGroupAllowedOnlyAdmins(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouAllowedOnlyAdmin, AppResources.UserAllowedOnlyAdmin);
    }

    public static string FormatGroupMadeAnnouncementOnly(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouMadeGroupAnnouncementOnly, AppResources.UserMadeGroupAnnouncementOnly);
    }

    public static string FormatGroupMadeNotAnnouncementOnly(string jid, string pushName)
    {
      return MultiParticipantsChatStrings.FormatSystemMessageString(jid, pushName, AppResources.YouMadeGroupNotAnnouncementOnly, AppResources.UserMadeGroupNotAnnouncementOnly);
    }
  }
}
