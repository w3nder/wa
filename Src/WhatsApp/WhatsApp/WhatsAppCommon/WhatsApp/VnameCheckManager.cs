// Decompiled with JetBrains decompiler
// Type: WhatsApp.VnameCheckManager
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace WhatsApp
{
  internal static class VnameCheckManager
  {
    private static string pattern = "[^\\p{L}\\p{M}\\p{N}]+";
    private static Regex splitPattern = (Regex) null;

    public static void OnVNameCheckRequest(string jid, string vName, string fromTo)
    {
      Dictionary<string, int> dictionary = VnameCheckManager.checkVname(jid, vName);
      FunXMPP.Connection connection = AppState.ClientInstance.GetConnection();
      if (connection == null)
        return;
      Action<int> onError = (Action<int>) (err => Log.l("vnamecheck", "Error {0} for {1}", (object) err, (object) jid));
      Action onComplete = (Action) (() => Log.l("vnamecheck", "OK for {0}", (object) jid));
      try
      {
        connection.SendSetBizVNameCheck(jid, vName, fromTo, dictionary, onComplete, onError);
      }
      catch (Exception ex)
      {
        string context = string.Format("Sending OnVNameCheckRequest response");
        Log.LogException(ex, context);
      }
    }

    private static Dictionary<string, int> checkVname(string jid, string vName)
    {
      UserStatus contact = UserCache.Get(jid, false);
      return contact == null ? (Dictionary<string, int>) null : VnameCheckManager.checkVnameForContact(contact, VnameCheckManager.NormalizeString(vName));
    }

    private static Dictionary<string, int> checkVnameForContact(
      UserStatus contact,
      HashSet<string> parts)
    {
      HashSet<string> stringSet = new HashSet<string>();
      if (!string.IsNullOrEmpty(contact.ContactName))
        stringSet.UnionWith((IEnumerable<string>) VnameCheckManager.NormalizeString(contact.ContactName));
      if (!string.IsNullOrEmpty(contact.FirstName))
        stringSet.UnionWith((IEnumerable<string>) VnameCheckManager.NormalizeString(contact.FirstName));
      string displayName = contact.GetDisplayName();
      if (!string.IsNullOrEmpty(displayName))
        stringSet.UnionWith((IEnumerable<string>) VnameCheckManager.NormalizeString(displayName));
      if (!string.IsNullOrEmpty(contact.PushName))
        stringSet.UnionWith((IEnumerable<string>) VnameCheckManager.NormalizeString(contact.PushName));
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      if (stringSet.Count > 0)
        return dictionary;
      foreach (string part in parts)
      {
        int num = 1000;
        foreach (string s2 in stringSet)
        {
          int levenshteinDistance = Utils.CalculateLevenshteinDistance(part, s2);
          if (num > levenshteinDistance)
            num = levenshteinDistance;
        }
        dictionary.Add(part, num);
      }
      return dictionary;
    }

    private static HashSet<string> NormalizeString(string vName)
    {
      if (VnameCheckManager.splitPattern == null)
        VnameCheckManager.splitPattern = new Regex(VnameCheckManager.pattern);
      return new HashSet<string>((IEnumerable<string>) VnameCheckManager.splitPattern.Split(vName.Trim().ToLower()));
    }
  }
}
