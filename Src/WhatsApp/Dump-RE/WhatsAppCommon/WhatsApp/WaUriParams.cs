// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaUriParams
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

#nullable disable
namespace WhatsApp
{
  public class WaUriParams
  {
    private Dictionary<string, string> paramDict = new Dictionary<string, string>();

    public WaUriParams()
    {
    }

    public WaUriParams(WaUriParams other)
    {
      if (other == null || !other.Any())
        return;
      this.paramDict = other.ToDictionary();
    }

    public WaUriParams(IDictionary<string, string> d)
    {
      if (d == null)
        return;
      this.paramDict = new Dictionary<string, string>(d);
    }

    public bool Any() => this.paramDict.Any<KeyValuePair<string, string>>();

    public void AddBool(string key, bool val) => this.AddString(key, val ? "true" : "false");

    public void AddInt(string key, int i) => this.AddString(key, i.ToString());

    public void AddString(string key, string val)
    {
      if (key == null)
        return;
      this.paramDict[key] = val;
    }

    public void AddParams(WaUriParams otherParams, bool overrideConflict = true)
    {
      if (otherParams == null || !otherParams.Any())
        return;
      foreach (KeyValuePair<string, string> keyValuePair in otherParams.ToDictionary())
      {
        if (overrideConflict || !this.paramDict.ContainsKey(keyValuePair.Key))
          this.paramDict[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public bool ContainsKey(string key) => this.paramDict.ContainsKey(key);

    public bool TryGetStrValue(string key, out string val)
    {
      return this.paramDict.TryGetValue(key, out val);
    }

    public bool TryGetBoolValue(string key, out bool val)
    {
      string val1 = (string) null;
      val = false;
      return this.TryGetStrValue(key, out val1) && !string.IsNullOrEmpty(val1) && bool.TryParse(val1, out val);
    }

    public bool TryGetIntValue(string key, out int val)
    {
      string val1 = (string) null;
      val = 0;
      return this.TryGetStrValue(key, out val1) && !string.IsNullOrEmpty(val1) && int.TryParse(val1, out val);
    }

    public string ToUriString()
    {
      string str = (string) null;
      if (this.paramDict.Any<KeyValuePair<string, string>>())
      {
        LinkedList<string> values = new LinkedList<string>();
        foreach (KeyValuePair<string, string> keyValuePair in this.paramDict)
          values.AddLast(string.Format("{0}={1}", (object) keyValuePair.Key, (object) Uri.EscapeDataString(keyValuePair.Value ?? "")));
        str = string.Join("&", (IEnumerable<string>) values);
      }
      return str ?? "";
    }

    public Dictionary<string, string> ToDictionary()
    {
      return new Dictionary<string, string>((IDictionary<string, string>) this.paramDict);
    }

    public static WaUriParams FromUri(Uri uri)
    {
      return WaUriParams.FromUriString(uri == (Uri) null ? "" : uri.OriginalString);
    }

    public static WaUriParams FromUriString(string uriStr)
    {
      if (uriStr == null)
        uriStr = "";
      int num = uriStr.IndexOf('?');
      return WaUriParams.FromUriParamString(num < 0 ? "" : uriStr.Substring(num + 1));
    }

    public static WaUriParams FromUriParamString(string s)
    {
      WaUriParams waUriParams = new WaUriParams();
      if (string.IsNullOrEmpty(s))
        return waUriParams;
      if (s.StartsWith("?"))
        s = s.Substring(1);
      string str1 = s;
      char[] chArray = new char[1]{ '&' };
      foreach (string str2 in str1.Split(chArray))
      {
        int length = str2.IndexOf('=');
        string url1;
        string url2;
        if (length < 0)
        {
          url1 = str2;
          url2 = "";
        }
        else
        {
          url1 = str2.Substring(0, length);
          url2 = str2.Substring(length + 1);
        }
        waUriParams.AddString(HttpUtility.UrlDecode(url1), HttpUtility.UrlDecode(url2));
      }
      return waUriParams;
    }

    public static WaUriParams ForChatPage(
      string jid,
      string source,
      bool includeTimestamp = true,
      string clearStackTo = "ContactsPage")
    {
      WaUriParams waUriParams = new WaUriParams();
      waUriParams.AddString(nameof (jid), jid);
      if (!string.IsNullOrEmpty(source))
        waUriParams.AddString("Source", source);
      if (!string.IsNullOrEmpty(clearStackTo))
        waUriParams.AddString("clr2", clearStackTo);
      if (includeTimestamp)
        waUriParams.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
      return waUriParams;
    }

    public static WaUriParams ForCallScreen(string peerJid)
    {
      WaUriParams waUriParams = new WaUriParams();
      if (peerJid != null)
        waUriParams.AddString("jid", peerJid);
      return waUriParams;
    }

    public static class Keys
    {
      public const string StrJid = "jid";
      public const string StrSource = "Source";
      public const string StrTimestamp = "Timestamp";
      public const string StrClearStackTo = "clr2";
      public const string StrAction = "Action";
      public const string StrCallId = "callid";
      public const string StrContext = "context";
      public const string StrKeyId = "keyid";
      public const string StrMsgId = "msgid";
      public const string StrCookie = "cookie";
      public const string StrType = "type";
      public const string StrCode = "code";
      public const string IntKnownCallState = "state";
      public const string BoolClearStack = "ClearStack";
      public const string BoolPageReplace = "PageReplace";
      public const string BoolVideoCall = "VideoCall";
    }

    public static class Values
    {
      public const string BoolTrue = "true";
      public const string BoolFalse = "false";
      public const string StrVoipToast = "vToast";
      public const string StrNonVoipToast = "nvToast";
      public const string StrChatTile = "SecondaryTile";
      public const string StrInvite = "invite";
    }
  }
}
