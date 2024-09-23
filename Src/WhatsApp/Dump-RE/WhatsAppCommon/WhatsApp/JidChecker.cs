// Decompiled with JetBrains decompiler
// Type: WhatsApp.JidChecker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class JidChecker
  {
    private const string LogHdr = "jidck";
    public const int PrimaryDeviceId = 0;
    public static readonly string[] SpecialUsers = new string[5]
    {
      "Server",
      "0",
      "status",
      "location",
      "1"
    };
    public static readonly string[] SpecialBroadcastUsers = new string[2]
    {
      "status",
      "location"
    };
    public const int MaxUserLength = 20;
    public const int MinUserLength = 1;
    public const int MaxGroupTimestampLength = 20;
    public const int MinGroupTimestampLength = 1;
    public const int MaxCallUserLength = 128;
    public const int MaxDeviceIdLength = 2;
    public const int MaxAgentIdLength = 2;
    private static long nextUploadTime = 0;
    private static readonly long minTimeBetweenUploads = (long) Math.Max(60, Settings.InvalidJidThrottleSeconds) * 10000000L;
    private static readonly int selectionRate = Math.Max(2, Settings.InvalidJidSampleRate);
    private static Random randomiser = new Random((int) (DateTime.Now.Ticks & 16777215L));

    public static bool IsValidDeviceId(int deviceId) => deviceId >= 0 && deviceId <= 99;

    public static bool CheckJidProtocolString(string jidProtocolString)
    {
      string userPart = (string) null;
      string serverPart = (string) null;
      if (!string.IsNullOrEmpty(jidProtocolString))
      {
        int length = jidProtocolString.IndexOf(Constants.JidDomainSeparator);
        if (length > 0)
          userPart = jidProtocolString.Substring(0, length);
        if (length < jidProtocolString.Length - 1)
          serverPart = jidProtocolString.Substring(length + 1);
      }
      return JidChecker.CheckJidParts(userPart, serverPart);
    }

    private static bool CheckJidParts(string userPart, string serverPart)
    {
      if (string.IsNullOrEmpty(serverPart))
      {
        Log.l("jidck", string.Format("Failed server part check: {0} @ {1}", (object) userPart, (object) serverPart));
        return false;
      }
      switch (serverPart)
      {
        case "s.whatsapp.net":
          return string.IsNullOrEmpty(userPart) || ((IEnumerable<string>) JidChecker.SpecialUsers).Contains<string>(userPart) || JidChecker.CheckUserPart(userPart);
        case "g.us":
          return string.IsNullOrEmpty(userPart) || JidChecker.CheckGroupPart(userPart);
        case "broadcast":
          if (((IEnumerable<string>) JidChecker.SpecialBroadcastUsers).Contains<string>(userPart))
            return true;
          for (int index = 0; index < userPart.Length; ++index)
          {
            if (!char.IsDigit(userPart[index]))
            {
              Log.l("jidck", string.Format("Failed broadcast user digit check: {0}", (object) userPart));
              return false;
            }
          }
          return true;
        case "call":
          if (userPart.Length > 128)
          {
            Log.l("jidck", string.Format("Failed call length check: {0}", (object) userPart));
            return false;
          }
          for (int index = 0; index < userPart.Length; ++index)
          {
            char ch = userPart[index];
            if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f' ? 1 : (ch < 'A' ? 0 : (ch <= 'F' ? 1 : 0))) == 0)
            {
              Log.l("jidck", string.Format("Failed call hex check: {0}", (object) userPart));
              return false;
            }
          }
          return true;
        case "business":
          return JidChecker.CheckUser(userPart, "BusinessChk");
        case "b.us":
          return JidChecker.CheckMultiAgentPart(userPart);
        default:
          Log.l("jidck", string.Format("Failed server check: {0}", (object) serverPart));
          return false;
      }
    }

    public static bool CheckUserJidProtocolString(string userjid)
    {
      if (userjid == null || !userjid.EndsWith("@s.whatsapp.net") || userjid.Length < "@s.whatsapp.net".Length + 1)
        return false;
      string userPart = userjid.Substring(0, userjid.Length - "@s.whatsapp.net".Length);
      return !string.IsNullOrEmpty(userPart) && !((IEnumerable<string>) JidChecker.SpecialUsers).Contains<string>(userPart) && JidChecker.CheckUserPart(userPart);
    }

    private static bool CheckUserPart(string userPart)
    {
      string userOnly = userPart;
      int length1 = userPart.IndexOf(Constants.JidDeviceSeparator);
      if (length1 > 0 && length1 < userOnly.Length - 1)
      {
        string s = userOnly.Substring(length1 + 1);
        if (s.Length > 2)
        {
          Log.l("jidck", string.Format("Failed device length check: {0}", (object) userPart));
          return false;
        }
        int result = -1;
        if (!int.TryParse(s, out result))
        {
          Log.l("jidck", string.Format("Failed device int check: {0}", (object) userPart));
          return false;
        }
        userOnly = userOnly.Substring(0, length1);
      }
      int length2 = userPart.IndexOf(Constants.JidAgentSeparator);
      if (length2 > 0 && length2 < userOnly.Length - 1)
      {
        string s = userOnly.Substring(length2 + 1);
        if (s.Length > 2)
        {
          Log.l("jidck", string.Format("Failed agent length check: {0}", (object) userPart));
          return false;
        }
        int result = -1;
        if (!int.TryParse(s, out result))
        {
          Log.l("jidck", string.Format("Failed agent int check: {0}", (object) userPart));
          return false;
        }
        userOnly = userOnly.Substring(0, length2);
      }
      return JidChecker.CheckUser(userOnly, "userchk");
    }

    private static bool CheckUser(string userOnly, string context)
    {
      int num = userOnly != null ? userOnly.Length : -1;
      if (num > 20 || num < 1)
      {
        Log.l("jidck", string.Format("{0} failed user length check: {1}", (object) context, (object) userOnly));
        return false;
      }
      for (int index = 0; index < num; ++index)
      {
        if (!char.IsDigit(userOnly[index]))
        {
          Log.l("jidck", string.Format("{0} failed user digit check: {1}", (object) context, (object) userOnly));
          return false;
        }
      }
      if (userOnly[0] != '0')
        return true;
      Log.l("jidck", string.Format("{0} failed first char non 0 check: {1}", (object) context, (object) userOnly));
      return false;
    }

    public static bool CheckGroupJidProtocolString(string groupjid)
    {
      return groupjid != null && groupjid.EndsWith("@g.us") && groupjid.Length >= "@g.us".Length + 1 && JidChecker.CheckGroupPart(groupjid.Substring(0, groupjid.Length - "@g.us".Length));
    }

    private static bool CheckGroupPart(string groupPart)
    {
      int length = groupPart.IndexOf(Constants.JidGroupTimestampSeparator);
      if (length < 1 || length > 20)
      {
        Log.l("jidck", string.Format("Failed group user length: {0}", (object) groupPart));
        return false;
      }
      if (length > groupPart.Length - 2)
      {
        Log.l("jidck", string.Format("Failed group timestamp min length: {0}", (object) groupPart));
        return false;
      }
      string str = groupPart.Substring(length + 1);
      if (str.Length > 20)
      {
        Log.l("jidck", string.Format("Failed group timestamp max length: {0}", (object) groupPart));
        return false;
      }
      for (int index = 0; index < str.Length; ++index)
      {
        if (!char.IsDigit(str[index]))
        {
          Log.l("jidck", string.Format("Failed group timestamp int check: {0}", (object) groupPart));
          return false;
        }
      }
      return JidChecker.CheckUser(groupPart.Substring(0, length), "groupchk");
    }

    private static bool CheckMultiAgentPart(string chatPart)
    {
      int length = chatPart.IndexOf(Constants.JidMultiAgentUserSeparator);
      if (length < 1 || length > 20)
      {
        Log.l("jidck", string.Format("Failed multi agent user 1 length: {0}", (object) chatPart));
        return false;
      }
      if (length > chatPart.Length - 2)
      {
        Log.l("jidck", string.Format("Failed multi agent user 2 min length: {0}", (object) chatPart));
        return false;
      }
      string str1 = chatPart.Substring(length + 1);
      if (str1.Length > 20)
      {
        Log.l("jidck", string.Format("Failed multi agent user 2 max length: {0}", (object) chatPart));
        return false;
      }
      string str2 = chatPart.Substring(0, length);
      if (!JidChecker.CheckUser(str2, "maChk1") || !JidChecker.CheckUser(str1, "maChk2"))
        return false;
      double num = double.Parse(str2);
      if (double.Parse(str1) > num)
        return true;
      Log.l("jidck", string.Format("Failed multi agent user comparison: {0}", (object) chatPart));
      return false;
    }

    public static bool MaybeSendJidErrorClb(string context, string jidString)
    {
      Log.l("jidck", string.Format("Invalid jid found: [{0}] - {1}", (object) (jidString ?? "<null>"), (object) context));
      if (DateTime.Now.Ticks > JidChecker.nextUploadTime)
      {
        int num = JidChecker.randomiser.Next(JidChecker.selectionRate);
        if (JidChecker.nextUploadTime == 0L || num == JidChecker.selectionRate - 1)
          JidChecker.SendJidErrorClb(context, jidString);
      }
      return false;
    }

    public static bool SendJidErrorClb(string context, string jidString)
    {
      JidChecker.nextUploadTime = DateTime.Now.Ticks + JidChecker.minTimeBetweenUploads;
      Log.l("jidck", new StackTrace().ToString());
      Log.SendCrashLog((Exception) new InvalidOperationException("Jid is invalid"), context, false, logOnlyForRelease: true);
      return false;
    }
  }
}
