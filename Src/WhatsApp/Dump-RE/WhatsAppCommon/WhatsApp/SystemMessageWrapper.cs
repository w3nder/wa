// Decompiled with JetBrains decompiler
// Type: WhatsApp.SystemMessageWrapper
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
  public class SystemMessageWrapper
  {
    private const string LogHdr = "sysmsgw";
    private Message msg;

    public SystemMessageWrapper.MessageTypes MessageType
    {
      get
      {
        return this.msg == null || this.msg.BinaryData == null || !((IEnumerable<byte>) this.msg.BinaryData).Any<byte>() ? SystemMessageWrapper.MessageTypes.Undefined : (SystemMessageWrapper.MessageTypes) this.msg.BinaryData[0];
      }
    }

    public SystemMessageWrapper(Message m)
    {
      if (m.MediaWaType != FunXMPP.FMessage.Type.System)
        return;
      this.msg = m;
    }

    public string Get()
    {
      string str = (string) null;
      if (this.msg == null || this.msg.BinaryData == null || !((IEnumerable<byte>) this.msg.BinaryData).Any<byte>())
        return str;
      switch (this.MessageType)
      {
        case SystemMessageWrapper.MessageTypes.Rename:
        case SystemMessageWrapper.MessageTypes.GroupParticipantNumberChanged:
          str = this.FormatChangeNumber();
          break;
        case SystemMessageWrapper.MessageTypes.ConversationEncrypted:
          str = this.FormatConversationEncrypted();
          break;
        case SystemMessageWrapper.MessageTypes.MissedCall:
          str = this.FormatMissedCall(false);
          break;
        case SystemMessageWrapper.MessageTypes.MissedVideoCall:
          str = this.FormatMissedCall(true);
          break;
        case SystemMessageWrapper.MessageTypes.GroupInviteChanged:
          str = this.FormatGroupInviteChanged();
          break;
        case SystemMessageWrapper.MessageTypes.VerifiedBizInitial:
          str = this.FormatVerifiedBizInitial();
          break;
        case SystemMessageWrapper.MessageTypes.VerifiedBizTransit:
          str = this.FormatVerifiedBizTransit();
          break;
        case SystemMessageWrapper.MessageTypes.VerifiedBizInitial2Tier:
          str = this.FormatVerifiedBizInitial2Tier();
          break;
        case SystemMessageWrapper.MessageTypes.VerifiedBizTransit2Tier:
          str = this.FormatVerifiedBizTransit2Tier();
          break;
        case SystemMessageWrapper.MessageTypes.VerifiedBizOneTime2Tier:
          str = this.FormatVerifiedBizOneTime2Tier();
          break;
        default:
          if (Settings.IsWaAdmin)
          {
            str = "unimplemented sys-message: " + this.MessageType.ToString();
            break;
          }
          break;
      }
      return str ?? "";
    }

    private string FormatMissedCall(bool videoCall)
    {
      BinaryData binaryData = new BinaryData(this.msg.BinaryData);
      int offset = 1;
      long seconds = binaryData.ReadLong64(offset);
      int newOffset = offset + 8;
      binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
      DateTime phoneTime = DateTimeUtils.FunTimeToPhoneTime(DateTimeUtils.FromUnixTime(seconds));
      return string.Format(videoCall ? AppResources.MissedVideoCallSystemMessage : AppResources.MissedCallSystemMessage, (object) DateTimeUtils.FormatCompactTime(phoneTime));
    }

    private string FormatGroupInviteChanged() => AppResources.GroupInviteLinkChanged;

    private string FormatChangeNumber()
    {
      if (this.msg == null)
        return (string) null;
      string keyRemoteJid = this.msg.KeyRemoteJid;
      string remoteResource = this.msg.RemoteResource;
      string changeNumberText = SystemMessageUtils.GetChangeNumberText(this.msg);
      string str1 = remoteResource;
      string str2 = !keyRemoteJid.Equals(str1) ? AppResources.ChangeNumberTapToAdd : AppResources.ChangeNumberCurrentlyChatting;
      return string.Format("{0}\n{1}", (object) changeNumberText, (object) str2);
    }

    private string FormatVerifiedBizInitial2Tier()
    {
      bool inPhoneBoook = false;
      bool nameMatchesPhonebook = false;
      Triad<VerifiedLevel, string, VerifiedTier> initialSysMsg2Tier = SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg2Tier(this.msg, out inPhoneBoook, out nameMatchesPhonebook);
      string str = (string) null;
      switch (initialSysMsg2Tier != null ? initialSysMsg2Tier.Third : VerifiedTier.NotApplicable)
      {
        case VerifiedTier.Bottom:
          str = AppResources.VerifiedSysMsgInitialBottom;
          break;
        case VerifiedTier.Top:
          str = string.Format(AppResources.VerifiedSysMsgInitialTop, (object) initialSysMsg2Tier.Second);
          break;
        default:
          Log.l("sysmsgw", "Not creating system message for {0}", (object) (initialSysMsg2Tier?.Third.ToString() ?? "null"));
          break;
      }
      return str;
    }

    private string FormatVerifiedBizInitial()
    {
      string str = (string) null;
      Pair<VerifiedLevel, string> verifiedState = (Pair<VerifiedLevel, string>) null;
      SystemMessageUtils.GetDataFromVerifiedBizInitialSysMsg(this.msg, out verifiedState);
      switch (verifiedState != null ? verifiedState.First : VerifiedLevel.NotApplicable)
      {
        case VerifiedLevel.unknown:
          str = AppResources.VerifiedSysMsgInitialUnknown;
          break;
        case VerifiedLevel.low:
          str = string.Format(AppResources.VerifiedSysMsgInitialLow, (object) verifiedState.Second);
          break;
        case VerifiedLevel.high:
          str = string.Format(AppResources.VerifiedSysMsgInitialHigh, (object) verifiedState.Second);
          break;
      }
      return str;
    }

    private string FormatVerifiedBizTransit()
    {
      string str = (string) null;
      Pair<VerifiedLevel, string> verifiedState = (Pair<VerifiedLevel, string>) null;
      Pair<VerifiedLevel, string> prevVerifiedState = (Pair<VerifiedLevel, string>) null;
      SystemMessageUtils.GetDataFromVerifiedBizTransitSysMsg(this.msg, out verifiedState, out prevVerifiedState);
      if (verifiedState == null || prevVerifiedState == null)
        return (string) null;
      VerifiedLevel first1 = verifiedState.First;
      string second1 = verifiedState.Second;
      VerifiedLevel first2 = prevVerifiedState.First;
      string second2 = prevVerifiedState.Second;
      switch (first2)
      {
        case VerifiedLevel.NotApplicable:
          switch (first1)
          {
            case VerifiedLevel.unknown:
              str = AppResources.VerifiedSysMsgConsumerToUnknown;
              break;
            case VerifiedLevel.low:
              str = AppResources.VerifiedSysMsgConsumerToLow;
              break;
            case VerifiedLevel.high:
              str = string.Format(AppResources.VerifiedSysMsgConsumerToHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.unknown:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              str = AppResources.VerifiedSysMsgUnknownToConsumer;
              break;
            case VerifiedLevel.low:
              str = string.Format(AppResources.VerifiedSysMsgUnknownToLow, (object) second1);
              break;
            case VerifiedLevel.high:
              str = string.Format(AppResources.VerifiedSysMsgUnknownToHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.low:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              str = AppResources.VerifiedSysMsgLowToConsumer;
              break;
            case VerifiedLevel.unknown:
              str = string.Format(AppResources.VerifiedSysMsgLowToUnknown, (object) second2);
              break;
            case VerifiedLevel.high:
              str = string.Format(AppResources.VerifiedSysMsgLowToHigh, (object) second1);
              break;
          }
          break;
        case VerifiedLevel.high:
          switch (first1)
          {
            case VerifiedLevel.NotApplicable:
              str = AppResources.VerifiedSysMsgHighToConsumer;
              break;
            case VerifiedLevel.unknown:
              str = string.Format(AppResources.VerifiedSysMsgHighToUnknown, (object) second2);
              break;
            case VerifiedLevel.low:
              str = string.Format(AppResources.VerifiedSysMsgHighToLow, (object) second2);
              break;
          }
          break;
      }
      return str;
    }

    private string FormatVerifiedBizTransit2Tier()
    {
      bool inPhonebook = false;
      bool nameMatchesPhonebook = false;
      Triad<SystemMessageUtils.TransistionToDisplay2Tier, string, string> triad = SystemMessageUtils.DetectChange(this.msg, out inPhonebook, out nameMatchesPhonebook);
      if (triad == null || triad.First == SystemMessageUtils.TransistionToDisplay2Tier.NoChange)
        return (string) null;
      string str = (string) null;
      SystemMessageUtils.TransistionToDisplay2Tier first = triad.First;
      string second = triad.Second;
      switch (first)
      {
        case SystemMessageUtils.TransistionToDisplay2Tier.TopToBottom:
          str = string.Format(AppResources.VerifiedSysMsgChangedToBottom, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.TopToConsumer:
        case SystemMessageUtils.TransistionToDisplay2Tier.BottomToConsumer:
          str = AppResources.VerifiedSysMsgChangedToConsumer;
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.BottomToTop:
          str = string.Format(AppResources.VerifiedSysMsgChangedToTop, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToTop:
          str = string.Format(AppResources.VerifiedSysMsgInitialTop, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.ConsumerToBottom:
          str = string.Format(AppResources.VerifiedSysMsgInitialBottom, (object) second);
          break;
        case SystemMessageUtils.TransistionToDisplay2Tier.NameChange:
          str = string.Format(AppResources.VerifiedSysMsgChangedName, (object) second);
          break;
        default:
          Log.l("sysmsgw", "No system message for tranistion system message {0}", (object) first);
          break;
      }
      return str;
    }

    private string FormatVerifiedBizOneTime2Tier()
    {
      Triad<VerifiedLevel, string, VerifiedTier> oneTimeSysMsgTier2 = SystemMessageUtils.GetDataFromVerifiedBizOneTimeSysMsgTier2(this.msg);
      if (oneTimeSysMsgTier2 == null)
        return (string) null;
      string str = (string) null;
      VerifiedLevel first = oneTimeSysMsgTier2.First;
      string second = oneTimeSysMsgTier2.Second;
      switch (first)
      {
        case VerifiedLevel.low:
          str = string.Format(AppResources.VerifiedSysMsgOneTimeLow, (object) second);
          break;
        case VerifiedLevel.high:
          str = string.Format(AppResources.VerifiedSysMsgOneTimeHigh, (object) second);
          break;
        default:
          Log.l("sysmsgw", "No system message for one time system message for {0}", (object) second);
          break;
      }
      return str;
    }

    private string FormatConversationEncrypted()
    {
      string str = (string) null;
      string keyRemoteJid = this.msg.KeyRemoteJid;
      if (JidHelper.IsGroupJid(keyRemoteJid))
        str = AppResources.GroupEnabledEncrypted;
      else if (JidHelper.IsBroadcastJid(keyRemoteJid))
      {
        str = AppResources.BroadcastEnabledEncrypted;
      }
      else
      {
        if (JidHelper.IsUserJid(keyRemoteJid))
        {
          UserStatus user = UserCache.Get(keyRemoteJid, false);
          if ((user != null ? (user.IsEnterprise() ? 1 : 0) : 0) != 0)
            str = string.Format(AppResources.ChatEnabledEncryptedEnterprise, (object) (user.GetVerifiedNameForDisplay() ?? JidHelper.GetPhoneNumber(keyRemoteJid, true)));
        }
        if (str == null)
          str = AppResources.ChatEnabledEncrypted;
      }
      return str;
    }

    public enum MessageTypes
    {
      Undefined = -1, // 0xFFFFFFFF
      ParticipantChange = 0,
      SubjectChange = 1,
      GroupPhotoChange = 2,
      Rename = 3,
      BroadcastListCreated = 4,
      Error = 5,
      GainedAdmin = 6,
      GroupDeleted = 7,
      GroupCreated = 8,
      IdentityChanged = 9,
      ConversationEncrypted = 10, // 0x0000000A
      MissedCall = 11, // 0x0000000B
      MissedVideoCall = 12, // 0x0000000C
      GroupInviteChanged = 13, // 0x0000000D
      ConvBizIsVerified = 14, // 0x0000000E
      ConvBizIsUnVerified = 15, // 0x0000000F
      ConvBizNowStandard = 16, // 0x00000010
      ConvBizNowUnverified = 17, // 0x00000011
      ConvBizNowVerified = 19, // 0x00000013
      GroupDescriptionChanged = 20, // 0x00000014
      VerifiedBizInitial = 21, // 0x00000015
      VerifiedBizTransit = 22, // 0x00000016
      GroupRestrictionLocked = 23, // 0x00000017
      GroupRestrictionUnlocked = 24, // 0x00000018
      GroupMadeAnnouncementOnly = 25, // 0x00000019
      GroupMadeNotAnnouncementOnly = 26, // 0x0000001A
      LostAdmin = 27, // 0x0000001B
      GroupDescriptionDeleted = 28, // 0x0000001C
      GroupParticipantNumberChanged = 29, // 0x0000001D
      VerifiedBizInitial2Tier = 30, // 0x0000001E
      VerifiedBizTransit2Tier = 31, // 0x0000001F
      VerifiedBizOneTime2Tier = 32, // 0x00000020
    }
  }
}
