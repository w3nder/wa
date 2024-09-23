// Decompiled with JetBrains decompiler
// Type: WhatsApp.UserCache
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
  public static class UserCache
  {
    private static KeyValueCache<string, UserStatus> cache;

    private static KeyValueCache<string, UserStatus> Cache
    {
      get
      {
        if (UserCache.cache == null)
          UserCache.cache = new KeyValueCache<string, UserStatus>(Settings.MaxGroupParticipants * 2, true, true);
        return UserCache.cache;
      }
    }

    public static void Clear() => UserCache.Cache.Clear();

    public static UserStatus GetCacheOnly(string jid)
    {
      UserStatus val = (UserStatus) null;
      UserCache.Cache.TryGet(jid, out val);
      return val;
    }

    public static UserStatus Get(string jid, bool createToDbIfNotFound)
    {
      UserStatus user = (UserStatus) null;
      if (jid != null && (!UserCache.Cache.TryGet(jid, out user) || user == null))
      {
        ContactsContext.Instance((Action<ContactsContext>) (db => user = db.GetUserStatus(jid, createToDbIfNotFound)));
        if (user != null)
          UserCache.Cache.Set(jid, user);
      }
      return user;
    }

    public static void Add(IEnumerable<UserStatus> users)
    {
      UserCache.Cache.Set(users.Where<UserStatus>((Func<UserStatus, bool>) (u => u != null && u.Jid != null)).Select<UserStatus, KeyValuePair<string, UserStatus>>((Func<UserStatus, KeyValuePair<string, UserStatus>>) (u => new KeyValuePair<string, UserStatus>(u.Jid, u))).ToArray<KeyValuePair<string, UserStatus>>());
    }
  }
}
