// Decompiled with JetBrains decompiler
// Type: WhatsApp.JidHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public static class JidHelper
  {
    public const char JidDelimiterChar = ',';
    public const string JidDelimiterStr = ",";

    public static JidHelper.JidTypes GetJidType(string jid)
    {
      JidHelper.JidTypes jidType = JidHelper.JidTypes.Undefined;
      if (jid != null)
      {
        if (jid.EndsWith("@s.whatsapp.net", StringComparison.Ordinal) && jid.Length > "@s.whatsapp.net".Length)
          jidType = jid == "0@s.whatsapp.net" ? JidHelper.JidTypes.Psa : JidHelper.JidTypes.User;
        else if (jid.EndsWith("@g.us", StringComparison.Ordinal) && jid.Length > "@g.us".Length)
          jidType = JidHelper.JidTypes.Group;
        else if (jid == "status@broadcast")
          jidType = JidHelper.JidTypes.Status;
        else if (jid.EndsWith("@broadcast", StringComparison.Ordinal))
          jidType = JidHelper.JidTypes.Broadcast;
      }
      return jidType;
    }

    public static bool IsSelfJid(string jid) => jid == Settings.MyJid;

    public static bool IsStatusJid(string jid) => jid == "status@broadcast";

    public static bool IsBroadcastJid(string jid)
    {
      return jid != null && !JidHelper.IsStatusJid(jid) && jid.EndsWith("@broadcast", StringComparison.Ordinal);
    }

    public static bool IsPsaJid(string jid) => jid == "0@s.whatsapp.net";

    public static bool IsGroupJid(string jid)
    {
      return jid != null && jid.EndsWith("@g.us", StringComparison.Ordinal);
    }

    public static bool IsUserJid(string jid)
    {
      return jid != null && jid.EndsWith("@s.whatsapp.net", StringComparison.Ordinal) && !JidHelper.IsPsaJid(jid);
    }

    public static bool IsMultiParticipantsChatJid(string jid)
    {
      bool flag = false;
      switch (JidHelper.GetJidType(jid))
      {
        case JidHelper.JidTypes.Group:
        case JidHelper.JidTypes.Broadcast:
        case JidHelper.JidTypes.Status:
          flag = true;
          break;
      }
      return flag;
    }

    public static string GetDisplayNameForContactJid(string jid)
    {
      UserStatus userStatus = UserCache.Get(jid, false);
      return userStatus != null ? userStatus.GetDisplayName() : JidHelper.GetPhoneNumber(jid, true);
    }

    public static string GetShortDisplayNameForContactJid(string jid)
    {
      UserStatus userStatus = UserCache.Get(jid, false);
      return userStatus != null ? userStatus.GetDisplayName(true) : JidHelper.GetPhoneNumber(jid, true);
    }

    public static bool IsJidInAddressBook(string jid)
    {
      UserStatus userStatus = UserCache.Get(jid, false);
      return userStatus != null && userStatus.IsInDeviceContactList;
    }

    public static bool IsJidBlocked(ContactsContext db, string jid)
    {
      return JidHelper.IsUserJid(jid) && db.BlockListSet.ContainsKey(jid);
    }

    public static bool HaveVCardForJid(string jid)
    {
      bool haveVcard = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => haveVcard = ((IEnumerable<Message>) db.GetContactVCardsWithJid(jid)).Any<Message>()));
      return haveVcard;
    }

    public static string GetDisplayNameForContactJidNoNumber(string jid)
    {
      return UserCache.Get(jid, false)?.GetDisplayName(getNumberIfNoName: false, getFormattedNumber: false);
    }

    public static string GetPhoneNumber(string jid, bool getFormattedNumber)
    {
      string phoneNumber = (string) null;
      if (JidHelper.IsUserJid(jid))
      {
        string number = new string(jid.Substring(0, jid.IndexOf('@')).Where<char>((Func<char, bool>) (c => char.IsDigit(c))).ToArray<char>());
        phoneNumber = getFormattedNumber ? AppState.FormatPhoneNumber(number) : number;
      }
      else if (JidHelper.IsPsaJid(jid))
        phoneNumber = Constants.OffcialName;
      return phoneNumber;
    }

    public static string GroupId2Jid(string gid) => gid + "@g.us";

    public static string FromRawNumber(string rawNumber) => rawNumber + "@s.whatsapp.net";

    public static string JoinJids(IEnumerable<string> jids)
    {
      return jids != null && jids.Any<string>() ? string.Join(",", jids.Select<string, string>((Func<string, string>) (jid => jid.Trim())).Where<string>((Func<string, bool>) (jid => !string.IsNullOrEmpty(jid)))) : (string) null;
    }

    public static string[] SplitJids(string jidsStr)
    {
      if (string.IsNullOrEmpty(jidsStr))
        return new string[0];
      return ((IEnumerable<string>) jidsStr.Split(',')).Select<string, string>((Func<string, string>) (s => s.Trim())).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))).ToArray<string>();
    }

    public enum JidTypes
    {
      Undefined,
      User,
      Group,
      Broadcast,
      Status,
      Psa,
    }
  }
}
