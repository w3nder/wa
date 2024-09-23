// Decompiled with JetBrains decompiler
// Type: WhatsApp.SuspiciousJid
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;


namespace WhatsApp
{
  public static class SuspiciousJid
  {
    private const string LogHeader = "suspicious jid";
    private static object cacheLock = new object();
    private static Dictionary<string, bool> suspiciousCache = (Dictionary<string, bool>) null;

    public static void MarkJidSuspicious(SqliteMessagesContext db, string jid, bool suspicious)
    {
      SuspiciousJid.MarkJidSuspiciousInCache(jid, suspicious);
      SuspiciousJid.MarkJidSuspiciousInDb(db, jid, suspicious);
    }

    public static void MarkJidSuspiciousInCache(string jid, bool suspicious)
    {
      if (jid == null)
        return;
      lock (SuspiciousJid.cacheLock)
      {
        SuspiciousJid.TryInitCache();
        SuspiciousJid.suspiciousCache[jid] = suspicious;
      }
    }

    private static void MarkJidSuspiciousInDb(
      SqliteMessagesContext db,
      string jid,
      bool suspicious)
    {
      if (jid == null)
        return;
      CreateResult result = CreateResult.None;
      JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound, out result);
      if ((result & CreateResult.CreatedToDb) == CreateResult.None && jidInfo.IsSuspicious.HasValue && jidInfo.IsSuspicious.Value == suspicious)
        return;
      jidInfo.IsSuspicious = new bool?(suspicious);
      db.SubmitChanges();
    }

    public static bool ShouldAllowLinksForJid(MessagesContext db, string jid)
    {
      bool allowLinks = !SuspiciousJid.IsJidSuspicious(db, jid, true);
      if (!allowLinks && JidHelper.IsUserJid(jid))
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          UserStatus userStatus = cdb.GetUserStatus(jid, false);
          if (userStatus == null || !userStatus.IsVerified() || userStatus.VerifiedLevel != VerifiedLevel.high)
            return;
          allowLinks = true;
        }));
      return allowLinks;
    }

    public static bool IsMessageSuspicious(MessagesContext db, Message m)
    {
      if (m.KeyFromMe)
        return false;
      bool flag = true;
      if (JidHelper.IsUserJid(m.KeyRemoteJid))
        flag = SuspiciousJid.IsJidSuspicious(db, m.KeyRemoteJid, true);
      else if (JidHelper.IsGroupJid(m.KeyRemoteJid))
        flag = SuspiciousJid.IsJidSuspicious(db, m.KeyRemoteJid, true) && SuspiciousJid.IsJidSuspicious(db, m.GetSenderJid(), true);
      return flag;
    }

    public static bool IsJidSuspicious(MessagesContext db, string jid, bool useCache)
    {
      if (jid == null)
        return false;
      bool isSuspicious = true;
      if (useCache)
      {
        lock (SuspiciousJid.cacheLock)
        {
          SuspiciousJid.TryInitCache();
          if (SuspiciousJid.suspiciousCache.TryGetValue(jid, out isSuspicious))
          {
            Log.d("suspicious jid", "fetched from cache | jid:{0},suspicious:{1}", (object) jid, (object) isSuspicious);
            return isSuspicious;
          }
        }
      }
      bool flag1 = false;
      bool flag2 = false;
      JidInfo jidInfo = db.GetJidInfo(jid, CreateOptions.None);
      if (jidInfo != null && jidInfo.IsSuspicious.HasValue && !jidInfo.IsSuspicious.Value)
      {
        isSuspicious = jidInfo.IsSuspicious.Value;
        Log.d("suspicious jid", "fetched from JidInfo | jid:{0},suspicious:{1}", (object) jid, (object) isSuspicious);
      }
      else
      {
        isSuspicious = !JidHelper.IsUserJid(jid) ? JidHelper.IsGroupJid(jid) && SuspiciousJid.IsGroupSuspiciousImpl(db, jid, useCache) : SuspiciousJid.IsUserSuspiciousImpl(jid);
        if (isSuspicious)
        {
          if (db.AnyOutgoingMessagesTo(jid))
            isSuspicious = false;
          else if (JidHelper.IsUserJid(jid) && !db.AnyIncomingMessagesFrom(jid, false))
          {
            isSuspicious = false;
            flag2 = true;
          }
        }
        if (isSuspicious && db.GetTrustedContactVCardsWithJid(jid).Length != 0)
        {
          isSuspicious = false;
          flag2 = false;
        }
        if (!isSuspicious)
          flag1 = true;
        Log.d("suspicious jid", "evaluated | jid:{0},suspicious:{1},value changed:{2},skip db:{3}", (object) jid, (object) isSuspicious, (object) flag1, (object) flag2);
      }
      SuspiciousJid.MarkJidSuspiciousInCache(jid, isSuspicious);
      if (flag1 && !flag2)
        AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db2 => SuspiciousJid.MarkJidSuspiciousInDb((SqliteMessagesContext) db2, jid, isSuspicious)))));
      return isSuspicious;
    }

    private static bool IsUserSuspiciousImpl(string userJid)
    {
      bool flag1 = true;
      if (userJid == null)
        return flag1;
      bool flag2 = false;
      for (int index = 0; index < userJid.Length; ++index)
      {
        char c = userJid[index];
        if (c != '@')
        {
          if (char.IsDigit(c))
          {
            flag2 = true;
            break;
          }
        }
        else
          break;
      }
      if (flag2)
      {
        if (Settings.MyJid == userJid)
        {
          flag1 = false;
        }
        else
        {
          UserStatus user = (UserStatus) null;
          ContactsContext.Instance((Action<ContactsContext>) (cdb => user = cdb.GetUserStatus(userJid, false)));
          UserStatus userStatus = user;
          if ((userStatus != null ? (userStatus.IsInDeviceContactList ? 1 : 0) : 0) != 0)
            flag1 = false;
        }
      }
      else
        flag1 = false;
      return flag1;
    }

    private static bool IsGroupSuspiciousImpl(MessagesContext db, string groupJid, bool useCache)
    {
      Conversation conversation = db.GetConversation(groupJid, CreateOptions.None);
      if (conversation == null)
        return true;
      string selfJid = Settings.MyJid;
      if (conversation.GroupOwner == selfJid || !SuspiciousJid.IsJidSuspicious(db, conversation.GroupOwner, useCache))
        return false;
      string[] adminsOtherThanSelf = (string[]) null;
      conversation.ParticipantSetAction((Action<GroupParticipants>) (set => adminsOtherThanSelf = set.Admins.Where<string>((Func<string, bool>) (jid => jid != selfJid)).ToArray<string>()));
      return adminsOtherThanSelf == null || !((IEnumerable<string>) adminsOtherThanSelf).Any<string>((Func<string, bool>) (jid => !SuspiciousJid.IsJidSuspicious(db, jid, useCache)));
    }

    private static void TryInitCache()
    {
      if (SuspiciousJid.suspiciousCache != null)
        return;
      SuspiciousJid.suspiciousCache = new Dictionary<string, bool>();
      ContactStore.ContactsUpdatedSubject.ObserveOnDispatcher<string[]>().Subscribe<string[]>((Action<string[]>) (updatedJids =>
      {
        lock (SuspiciousJid.cacheLock)
        {
          if (updatedJids == null || !((IEnumerable<string>) updatedJids).Any<string>())
            return;
          foreach (string updatedJid in updatedJids)
          {
            if (string.IsNullOrEmpty(updatedJid))
              Log.SendCrashLog((Exception) new ArgumentNullException("null updated jid"), "null updated jid from contact store, jids: " + string.Join(",", updatedJids), logOnlyForRelease: true);
            else
              SuspiciousJid.suspiciousCache.Remove(updatedJid);
          }
        }
      }));
    }
  }
}
