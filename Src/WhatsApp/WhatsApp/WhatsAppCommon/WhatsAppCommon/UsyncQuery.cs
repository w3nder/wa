// Decompiled with JetBrains decompiler
// Type: WhatsAppCommon.UsyncQuery
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WhatsApp;
using WhatsApp.ProtoBuf;


namespace WhatsAppCommon
{
  public sealed class UsyncQuery
  {
    public static string LogHdr = "usyncQ";
    public string sid;
    public int index;
    private bool last;
    private Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode> queryProtocols;
    private Dictionary<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>> userDictionary;
    private Dictionary<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>> sidelistDictionary;
    private List<FunXMPP.ProtocolTreeNode> userlessProtocolData;
    private HashSet<UsyncQuery.UsyncProtocol> protocols;
    private FunXMPP.Connection.SyncMode syncMode;
    private FunXMPP.Connection.SyncContext syncContext;
    private Action<FunXMPP.Connection.SyncResult> onGetContacts;
    private Action<int> onGetContactsError;
    private Action<FunXMPP.Connection.SyncResult> onGetSidelist;
    private Action<int> onGetSidelistError;
    private Action<Dictionary<string, RemoteClientCaps>, Dictionary<string, RemoteClientCaps>> onGetRemoteCapabilities;
    private Action<int> onGetRemoteCapabilitiesError;
    private Action<IEnumerable<UsyncQuery.PhotoIdResult>, IEnumerable<UsyncQuery.PhotoIdResult>> onGetPhotoIds;
    private Action<int> onGetPhotoIdsError;
    private Action<IEnumerable<UsyncQuery.StatusResult>, IEnumerable<UsyncQuery.StatusResult>> onGetStatuses;
    private Action<int> onGetStatusesError;
    private Action<IEnumerable<UsyncQuery.BusinessResult>, IEnumerable<UsyncQuery.BusinessResult>> onGetBusinesses;
    private Action<int> onGetBusinessesError;

    public static UsyncQuery.UsyncProtocol ParseProtocol(string protocolName)
    {
      switch (protocolName)
      {
        case "business":
          return UsyncQuery.UsyncProtocol.Business;
        case "contact":
          return UsyncQuery.UsyncProtocol.Contact;
        case "feature":
          return UsyncQuery.UsyncProtocol.Feature;
        case "picture":
          return UsyncQuery.UsyncProtocol.Picture;
        case "sidelist":
          return UsyncQuery.UsyncProtocol.Sidelist;
        case "status":
          return UsyncQuery.UsyncProtocol.Status;
        default:
          return UsyncQuery.UsyncProtocol.Invalid;
      }
    }

    public static string ProtocolToString(UsyncQuery.UsyncProtocol protocol)
    {
      switch (protocol)
      {
        case UsyncQuery.UsyncProtocol.Contact:
          return "contact";
        case UsyncQuery.UsyncProtocol.Picture:
          return "picture";
        case UsyncQuery.UsyncProtocol.Status:
          return "status";
        case UsyncQuery.UsyncProtocol.Feature:
          return "feature";
        case UsyncQuery.UsyncProtocol.Business:
          return "business";
        case UsyncQuery.UsyncProtocol.Sidelist:
          return "sidelist";
        default:
          return (string) null;
      }
    }

    public UsyncQuery(FunXMPP.Connection.SyncMode mode, FunXMPP.Connection.SyncContext context)
    {
      this.syncMode = mode;
      this.syncContext = context;
      this.sid = DateTime.UtcNow.ToFileTimeUtc().ToString();
      this.last = true;
      this.queryProtocols = new Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>();
      this.userDictionary = new Dictionary<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>>();
      this.sidelistDictionary = new Dictionary<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>>();
      this.userlessProtocolData = new List<FunXMPP.ProtocolTreeNode>();
      this.protocols = new HashSet<UsyncQuery.UsyncProtocol>();
    }

    public FunXMPP.ProtocolTreeNode ToIq(string id)
    {
      FunXMPP.ProtocolTreeNode[] iq = this.ProtocolDataToIq();
      if (iq == null)
        return (FunXMPP.ProtocolTreeNode) null;
      return new FunXMPP.ProtocolTreeNode("iq", new FunXMPP.KeyValue[4]
      {
        new FunXMPP.KeyValue("to", Settings.MyJid),
        new FunXMPP.KeyValue("type", "get"),
        new FunXMPP.KeyValue(nameof (id), id),
        new FunXMPP.KeyValue("xmlns", "usync")
      }, new FunXMPP.ProtocolTreeNode("usync", new FunXMPP.KeyValue[5]
      {
        new FunXMPP.KeyValue("sid", this.sid),
        new FunXMPP.KeyValue("index", this.index.ToString()),
        new FunXMPP.KeyValue("last", this.last ? "true" : "false"),
        new FunXMPP.KeyValue("mode", this.syncMode.ToString().ToLowerInvariant()),
        new FunXMPP.KeyValue("context", this.syncContext.ToString().ToLowerInvariant())
      }, iq));
    }

    private FunXMPP.ProtocolTreeNode[] ProtocolDataToIq()
    {
      List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList1 = new List<FunXMPP.ProtocolTreeNode>();
      if (this.userDictionary.Any<KeyValuePair<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>>>())
      {
        foreach (string key in this.userDictionary.Keys)
        {
          if (!JidChecker.CheckUserJidProtocolString(key))
          {
            JidChecker.MaybeSendJidErrorClb("Usync user list", key);
            this.userDictionary.Remove(key);
          }
        }
      }
      if (this.userDictionary.Any<KeyValuePair<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>>>() || this.userlessProtocolData.Any<FunXMPP.ProtocolTreeNode>())
      {
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList2 = new List<FunXMPP.ProtocolTreeNode>();
        foreach (KeyValuePair<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>> user in this.userDictionary)
          protocolTreeNodeList2.Add(new FunXMPP.ProtocolTreeNode("user", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("jid", user.Key)
          }, user.Value.Values.ToArray<FunXMPP.ProtocolTreeNode>()));
        protocolTreeNodeList2.AddRange((IEnumerable<FunXMPP.ProtocolTreeNode>) this.userlessProtocolData);
        protocolTreeNodeList1.Add(new FunXMPP.ProtocolTreeNode("list", (FunXMPP.KeyValue[]) null, protocolTreeNodeList2.ToArray()));
      }
      else if (this.syncMode == FunXMPP.Connection.SyncMode.Full)
      {
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList3 = new List<FunXMPP.ProtocolTreeNode>();
        protocolTreeNodeList1.Add(new FunXMPP.ProtocolTreeNode("list", (FunXMPP.KeyValue[]) null, protocolTreeNodeList3.ToArray()));
      }
      if ((this.sidelistDictionary == null || !this.sidelistDictionary.Any<KeyValuePair<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>>>() ? 0 : (Settings.UsyncSidelist ? 1 : 0)) != 0)
      {
        if (this.syncMode == FunXMPP.Connection.SyncMode.Full || this.syncMode == FunXMPP.Connection.SyncMode.Delta)
          this.AddSimpleQuery(UsyncQuery.UsyncProtocol.Sidelist, (FunXMPP.ProtocolTreeNode[]) null);
        List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList4 = new List<FunXMPP.ProtocolTreeNode>();
        foreach (KeyValuePair<string, Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>> sidelist in this.sidelistDictionary)
          protocolTreeNodeList4.Add(new FunXMPP.ProtocolTreeNode("user", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("jid", sidelist.Key)
          }, sidelist.Value.Values.ToArray<FunXMPP.ProtocolTreeNode>()));
        protocolTreeNodeList1.Add(new FunXMPP.ProtocolTreeNode("side_list", (FunXMPP.KeyValue[]) null, protocolTreeNodeList4.ToArray()));
      }
      if (protocolTreeNodeList1.Count <= 0)
        return (FunXMPP.ProtocolTreeNode[]) null;
      FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("query", (FunXMPP.KeyValue[]) null, this.queryProtocols.Values.ToArray<FunXMPP.ProtocolTreeNode>());
      protocolTreeNodeList1.Insert(0, protocolTreeNode);
      return protocolTreeNodeList1.ToArray();
    }

    public string ToLogString()
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      stringBuilder1.Append("Mode=" + this.syncMode.ToString());
      stringBuilder1.Append(", Context=" + this.syncContext.ToString());
      if (this.protocols.Any<UsyncQuery.UsyncProtocol>())
      {
        stringBuilder1.Append(", Protools=[");
        foreach (UsyncQuery.UsyncProtocol protocol in this.protocols)
          stringBuilder1.Append(protocol.ToString() + ",");
        stringBuilder1.Append(']');
      }
      StringBuilder stringBuilder2 = stringBuilder1;
      int count;
      string str1;
      if (this.userDictionary == null)
      {
        str1 = "null";
      }
      else
      {
        count = this.userDictionary.Count;
        str1 = count.ToString();
      }
      string str2;
      if (this.sidelistDictionary == null)
      {
        str2 = "null";
      }
      else
      {
        count = this.sidelistDictionary.Count;
        str2 = count.ToString();
      }
      string str3;
      if (this.userlessProtocolData == null)
      {
        str3 = "null";
      }
      else
      {
        count = this.userlessProtocolData.Count;
        str3 = count.ToString();
      }
      string str4 = string.Format(", UserList={0}, Sidelist={1}, Userless={2}", (object) str1, (object) str2, (object) str3);
      stringBuilder2.Append(str4);
      return stringBuilder1.ToString();
    }

    public FunXMPP.IqResultHandler GenerateIqHandler(
      Action onIQReceive = null,
      Action onComplete = null,
      Action<int> onError = null)
    {
      return new FunXMPP.IqResultHandler((Action<FunXMPP.ProtocolTreeNode, string>) ((node, from) =>
      {
        Action action1 = onIQReceive;
        if (action1 != null)
          action1();
        FunXMPP.ProtocolTreeNode child = node.GetChild("usync")?.GetChild("result");
        if (child == null)
          return;
        if (this.protocols.Contains(UsyncQuery.UsyncProtocol.Contact))
        {
          this.ParseProtocolResult(UsyncQuery.UsyncProtocol.Contact, node, child);
          this.protocols.Remove(UsyncQuery.UsyncProtocol.Contact);
        }
        foreach (UsyncQuery.UsyncProtocol protocol in this.protocols)
          this.ParseProtocolResult(protocol, node, child);
        Action action2 = onComplete;
        if (action2 == null)
          return;
        action2();
      }), (Action<FunXMPP.ProtocolTreeNode>) (resp =>
      {
        int result1 = 500;
        FunXMPP.ProtocolTreeNode child = resp.GetChild("error");
        if (child != null)
        {
          string attributeValue1 = child.GetAttributeValue("code");
          if (attributeValue1 != null)
            int.TryParse(attributeValue1, out result1);
          string attributeValue2 = child.GetAttributeValue("backoff");
          int result2;
          if (attributeValue2 != null && int.TryParse(attributeValue2, out result2))
            Settings.UsyncBackoffUtc = new DateTime?(FunRunner.CurrentServerTimeUtc.AddSeconds((double) result2));
        }
        Action<int> action = onError;
        if (action == null)
          return;
        action(result1);
      }));
    }

    private void ParseProtocolResult(
      UsyncQuery.UsyncProtocol protocol,
      FunXMPP.ProtocolTreeNode node,
      FunXMPP.ProtocolTreeNode resultNode)
    {
      if (protocol == UsyncQuery.UsyncProtocol.Invalid)
        return;
      FunXMPP.ProtocolTreeNode child1 = resultNode.GetChild(UsyncQuery.ProtocolToString(protocol));
      if (child1 == null)
        return;
      FunXMPP.ProtocolTreeNode child2 = child1.GetChild("error");
      int? attributeInt = child1.GetAttributeInt("refresh");
      if (child2 == null)
      {
        switch (protocol)
        {
          case UsyncQuery.UsyncProtocol.Contact:
            this.OnGetContacts(this.ParseContacts(node));
            break;
          case UsyncQuery.UsyncProtocol.Picture:
            List<UsyncQuery.PhotoIdResult> photoIds1 = this.ParsePhotoIds(node, UsyncQuery.UsyncListOption.List);
            List<UsyncQuery.PhotoIdResult> photoIds2 = this.ParsePhotoIds(node, UsyncQuery.UsyncListOption.SideList);
            this.RemoveDuplicatedPictureUsers(ref photoIds2, ref photoIds1);
            this.OnGetPhotoIds((IEnumerable<UsyncQuery.PhotoIdResult>) photoIds1, (IEnumerable<UsyncQuery.PhotoIdResult>) photoIds2);
            break;
          case UsyncQuery.UsyncProtocol.Status:
            List<UsyncQuery.StatusResult> statuses1 = this.ParseStatuses(node, UsyncQuery.UsyncListOption.List);
            List<UsyncQuery.StatusResult> statuses2 = this.ParseStatuses(node, UsyncQuery.UsyncListOption.SideList);
            this.RemoveDuplicatedStatusUsers(ref statuses2, ref statuses1);
            this.OnGetStatuses((IEnumerable<UsyncQuery.StatusResult>) statuses1, (IEnumerable<UsyncQuery.StatusResult>) statuses2);
            break;
          case UsyncQuery.UsyncProtocol.Feature:
            Dictionary<string, RemoteClientCaps> features1 = this.ParseFeatures(node, UsyncQuery.UsyncListOption.List);
            try
            {
              Dictionary<string, RemoteClientCaps> features2 = this.ParseFeatures(node, UsyncQuery.UsyncListOption.SideList);
              this.OnGetRemoteCapabilities(features1, features2);
              break;
            }
            catch (Exception ex)
            {
              string context = UsyncQuery.LogHdr + "Exception processing usync features";
              Log.SendCrashLog(ex, context);
              this.OnGetRemoteCapabilities(features1, (Dictionary<string, RemoteClientCaps>) null);
              break;
            }
          case UsyncQuery.UsyncProtocol.Business:
            List<UsyncQuery.BusinessResult> businesses1 = this.ParseBusinesses(node, UsyncQuery.UsyncListOption.List);
            List<UsyncQuery.BusinessResult> businesses2 = this.ParseBusinesses(node, UsyncQuery.UsyncListOption.SideList);
            this.RemoveDuplicatedBizUsers(ref businesses2, ref businesses1);
            this.OnGetBusinesses((IEnumerable<UsyncQuery.BusinessResult>) businesses1, (IEnumerable<UsyncQuery.BusinessResult>) businesses2);
            break;
          case UsyncQuery.UsyncProtocol.Sidelist:
            this.OnGetSidelist(this.ParseSidelist(node));
            break;
        }
      }
      else
      {
        switch (protocol)
        {
          case UsyncQuery.UsyncProtocol.Contact:
            this.ProcessErrorCodes(child1, this.OnGetContactsError);
            this.OnGetContacts((FunXMPP.Connection.SyncResult) null);
            break;
          case UsyncQuery.UsyncProtocol.Picture:
            this.ProcessErrorCodes(child1, this.OnGetPhotoIdsError);
            this.OnGetPhotoIds((IEnumerable<UsyncQuery.PhotoIdResult>) null, (IEnumerable<UsyncQuery.PhotoIdResult>) null);
            break;
          case UsyncQuery.UsyncProtocol.Status:
            this.ProcessErrorCodes(child1, this.OnGetStatusesError);
            this.OnGetStatuses((IEnumerable<UsyncQuery.StatusResult>) null, (IEnumerable<UsyncQuery.StatusResult>) null);
            break;
          case UsyncQuery.UsyncProtocol.Feature:
            this.ProcessErrorCodes(child1, this.OnGetRemoteCapabilitiesError);
            this.OnGetRemoteCapabilities((Dictionary<string, RemoteClientCaps>) null, (Dictionary<string, RemoteClientCaps>) null);
            break;
          case UsyncQuery.UsyncProtocol.Business:
            this.ProcessErrorCodes(child1, this.OnGetBusinessesError);
            this.OnGetBusinesses((IEnumerable<UsyncQuery.BusinessResult>) null, (IEnumerable<UsyncQuery.BusinessResult>) null);
            break;
          case UsyncQuery.UsyncProtocol.Sidelist:
            this.ProcessErrorCodes(child1, this.onGetSidelistError);
            this.OnGetSidelist((FunXMPP.Connection.SyncResult) null);
            break;
        }
        this.UpdateProtocolBackoff(protocol, child1);
      }
      if (this.syncMode != FunXMPP.Connection.SyncMode.Full)
        return;
      this.UpdateRefreshInterval(protocol, attributeInt);
    }

    private void UpdateRefreshInterval(UsyncQuery.UsyncProtocol protocol, int? seconds)
    {
      DateTime? nullable = new DateTime?();
      if (seconds.HasValue)
        nullable = new DateTime?(FunRunner.CurrentServerTimeUtc.AddSeconds((double) seconds.Value));
      switch (protocol)
      {
        case UsyncQuery.UsyncProtocol.Picture:
          Settings.NextUsyncPictureRefreshUtc = nullable;
          break;
        case UsyncQuery.UsyncProtocol.Status:
          Settings.NextUsyncStatusRefreshUtc = nullable;
          break;
        case UsyncQuery.UsyncProtocol.Feature:
          Settings.NextUsyncFeatureRefreshUtc = nullable;
          break;
        case UsyncQuery.UsyncProtocol.Business:
          Settings.NextUsyncBusinessRefreshUtc = nullable;
          break;
        case UsyncQuery.UsyncProtocol.Sidelist:
          Settings.NextUsyncSidelistRefreshUtc = nullable;
          break;
      }
    }

    private void UpdateProtocolBackoff(
      UsyncQuery.UsyncProtocol protocol,
      FunXMPP.ProtocolTreeNode queryErrorNode)
    {
      FunXMPP.ProtocolTreeNode child = queryErrorNode.GetChild("error");
      if (child == null)
        return;
      int? nullable = child.GetAttributeInt("backoff");
      if (!nullable.HasValue)
      {
        Log.l(UsyncQuery.LogHdr, "No backoff supplied - defaulting to 2 hours");
        nullable = new int?(7200);
      }
      if (!nullable.HasValue)
        return;
      DateTime dateTime = FunRunner.CurrentServerTimeUtc.AddSeconds((double) nullable.Value);
      switch (protocol)
      {
        case UsyncQuery.UsyncProtocol.Contact:
          Settings.SyncBackoffUtc = new DateTime?(dateTime);
          break;
        case UsyncQuery.UsyncProtocol.Picture:
          Settings.UsyncPictureBackoffUtc = new DateTime?(dateTime);
          break;
        case UsyncQuery.UsyncProtocol.Status:
          Settings.UsyncStatusBackoffUtc = new DateTime?(dateTime);
          break;
        case UsyncQuery.UsyncProtocol.Feature:
          Settings.UsyncFeatureBackoffUtc = new DateTime?(dateTime);
          break;
        case UsyncQuery.UsyncProtocol.Business:
          Settings.UsyncBusinessBackoffUtc = new DateTime?(dateTime);
          break;
        case UsyncQuery.UsyncProtocol.Sidelist:
          Settings.UsyncSidelistBackoffUtc = new DateTime?(dateTime);
          break;
      }
    }

    private IEnumerable<int> ParseErrorCodes(FunXMPP.ProtocolTreeNode queryErrorNode)
    {
      return queryErrorNode.GetAllChildren("error").Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (errorNode => errorNode != null)).Select<FunXMPP.ProtocolTreeNode, string>((Func<FunXMPP.ProtocolTreeNode, string>) (errorNode => errorNode.GetAttributeValue("code"))).Select<string, int>((Func<string, int>) (codeS => int.Parse(codeS, (IFormatProvider) CultureInfo.InvariantCulture)));
    }

    private void ProcessErrorCodes(FunXMPP.ProtocolTreeNode queryErrorNode, Action<int> onError)
    {
      int num = -1;
      foreach (int errorCode in this.ParseErrorCodes(queryErrorNode))
        num = errorCode;
      onError(num);
    }

    private List<UsyncQuery.BusinessResult> ParseBusinesses(
      FunXMPP.ProtocolTreeNode usyncNode,
      UsyncQuery.UsyncListOption listOption)
    {
      List<UsyncQuery.BusinessResult> businesses = new List<UsyncQuery.BusinessResult>();
      FunXMPP.ProtocolTreeNode usyncResultUserList = UsyncQuery.GetUsyncResultUserList(usyncNode, listOption);
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((usyncResultUserList != null ? (object) usyncResultUserList.GetAllChildren("user") : (object) null) ?? (object) new FunXMPP.ProtocolTreeNode[0]))
      {
        FunXMPP.ProtocolTreeNode child1 = protocolTreeNode.GetChild("business");
        string attributeValue = protocolTreeNode.GetAttributeValue("jid");
        if (attributeValue != null && (this.syncMode == FunXMPP.Connection.SyncMode.Delta || child1 != null))
        {
          if (!JidChecker.CheckUserJidProtocolString(attributeValue))
          {
            JidChecker.MaybeSendJidErrorClb("Usync biz user", attributeValue);
          }
          else
          {
            UsyncQuery.BusinessResult businessResult = new UsyncQuery.BusinessResult()
            {
              Jid = attributeValue,
              IsBusiness = child1 != null
            };
            businesses.Add(businessResult);
            if (businessResult.IsBusiness)
            {
              FunXMPP.ProtocolTreeNode child2 = child1.GetChild("verified_name");
              if (child2 != null && child2.GetAttributeValue("v") == "1")
              {
                businessResult.VerifiedLevel = child2.GetAttributeValue("verified_level");
                businessResult.Certificate = child2.data;
              }
              FunXMPP.ProtocolTreeNode child3 = child1.GetChild("profile");
              if (child3 != null)
                businessResult.BizProfile = BizProfileDetails.ExtractProfileDetails(child3);
            }
          }
        }
      }
      return businesses;
    }

    private void RemoveDuplicatedBizUsers(
      ref List<UsyncQuery.BusinessResult> sidelist,
      ref List<UsyncQuery.BusinessResult> list)
    {
      if (this.syncMode != FunXMPP.Connection.SyncMode.Delta)
        return;
      List<UsyncQuery.BusinessResult> businessResultList = new List<UsyncQuery.BusinessResult>();
      foreach (UsyncQuery.BusinessResult businessResult1 in list)
      {
        string jid = businessResult1.Jid;
        foreach (UsyncQuery.BusinessResult businessResult2 in sidelist)
        {
          if (businessResult2.Jid == jid)
          {
            businessResultList.Add(businessResult2);
            break;
          }
        }
      }
      if (businessResultList.Count <= 0)
        return;
      Log.d(UsyncQuery.LogHdr, "Removing {0} duplicated sidelist biz users", (object) businessResultList.Count);
      foreach (UsyncQuery.BusinessResult businessResult in businessResultList)
        sidelist.Remove(businessResult);
    }

    private FunXMPP.Connection.SyncResult ParseContacts(FunXMPP.ProtocolTreeNode node)
    {
      FunXMPP.ProtocolTreeNode userListNode = UsyncQuery.GetUsyncResultUserList(node, UsyncQuery.UsyncListOption.List);
      FunXMPP.ProtocolTreeNode child1 = node?.GetChild("usync")?.GetChild("result")?.GetChild("contact");
      if (userListNode == null)
        return (FunXMPP.Connection.SyncResult) null;
      Func<string, bool, IEnumerable<FunXMPP.Connection.SyncResult.User>> func = (Func<string, bool, IEnumerable<FunXMPP.Connection.SyncResult.User>>) ((str, checkValidity) =>
      {
        IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = userListNode.GetAllChildren("user");
        return allChildren == null ? (IEnumerable<FunXMPP.Connection.SyncResult.User>) new FunXMPP.Connection.SyncResult.User[0] : allChildren.Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (user =>
        {
          string attributeValue3 = user.GetChild("contact")?.GetAttributeValue("type");
          int num = attributeValue3 == null ? 0 : (attributeValue3.Equals(str) ? 1 : 0);
          bool flag = true;
          if ((num & (checkValidity ? 1 : 0)) != 0)
          {
            string attributeValue4 = user.GetAttributeValue("jid");
            flag = JidChecker.CheckUserJidProtocolString(attributeValue4);
            if (!flag)
              JidChecker.MaybeSendJidErrorClb("usync contacts", attributeValue4);
          }
          return (num & (flag ? 1 : 0)) != 0;
        })).Select<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.SyncResult.User>((Func<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.SyncResult.User>) (user =>
        {
          FunXMPP.ProtocolTreeNode child3 = user.GetChild("contact");
          string attributeValue = user.GetAttributeValue("jid");
          string dataString = child3.GetDataString();
          if (dataString == null || dataString.StartsWith("000") || attributeValue == null || attributeValue.StartsWith("s."))
            Log.d(UsyncQuery.LogHdr, "Unexpected details {0}, {1}", (object) attributeValue, (object) dataString);
          return new FunXMPP.Connection.SyncResult.User()
          {
            Jid = attributeValue,
            OriginalNumber = dataString
          };
        }));
      });
      FunXMPP.Connection.SyncResult contacts = new FunXMPP.Connection.SyncResult();
      contacts.SwellFolks = func("in", true).Where<FunXMPP.Connection.SyncResult.User>((Func<FunXMPP.Connection.SyncResult.User, bool>) (u => u.Jid != null)).ToArray<FunXMPP.Connection.SyncResult.User>();
      contacts.Holdouts = func("out", true).ToArray<FunXMPP.Connection.SyncResult.User>();
      contacts.NormalizationErrors = func("invalid", false).Select<FunXMPP.Connection.SyncResult.User, string>((Func<FunXMPP.Connection.SyncResult.User, string>) (u => u.OriginalNumber)).ToArray<string>();
      string attributeValue5 = child1?.GetAttributeValue("refresh");
      int result = 0;
      if (attributeValue5 != null && int.TryParse(attributeValue5, out result))
        contacts.NextFullSyncUtc = new DateTime?(FunRunner.CurrentServerTimeUtc.AddSeconds((double) result));
      return contacts;
    }

    private FunXMPP.Connection.SyncResult ParseSidelist(FunXMPP.ProtocolTreeNode node)
    {
      FunXMPP.ProtocolTreeNode userListNode = UsyncQuery.GetUsyncResultUserList(node, UsyncQuery.UsyncListOption.SideList);
      node?.GetChild("usync")?.GetChild("result")?.GetChild("sidelist");
      if (userListNode == null)
        return (FunXMPP.Connection.SyncResult) null;
      Func<string, bool, IEnumerable<FunXMPP.Connection.SyncResult.User>> func = (Func<string, bool, IEnumerable<FunXMPP.Connection.SyncResult.User>>) ((str, chkvalidity) =>
      {
        IEnumerable<FunXMPP.ProtocolTreeNode> allChildren = userListNode.GetAllChildren("user");
        return allChildren == null || !allChildren.Any<FunXMPP.ProtocolTreeNode>() ? (IEnumerable<FunXMPP.Connection.SyncResult.User>) new FunXMPP.Connection.SyncResult.User[0] : allChildren.Where<FunXMPP.ProtocolTreeNode>((Func<FunXMPP.ProtocolTreeNode, bool>) (user =>
        {
          string attributeValue3 = user.GetChild("sidelist")?.GetAttributeValue("type");
          int num = attributeValue3 == null ? 0 : (attributeValue3.Equals(str) ? 1 : 0);
          string attributeValue4 = user.GetAttributeValue("jid");
          bool flag = JidChecker.CheckUserJidProtocolString(attributeValue4);
          if (!flag)
            JidChecker.MaybeSendJidErrorClb("usync side list contacts", attributeValue4);
          return ((attributeValue3 == null ? 0 : (attributeValue3.Equals(str) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
        })).Select<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.SyncResult.User>((Func<FunXMPP.ProtocolTreeNode, FunXMPP.Connection.SyncResult.User>) (user =>
        {
          user.GetChild("sidelist");
          return new FunXMPP.Connection.SyncResult.User()
          {
            Jid = user.GetAttributeValue("jid")
          };
        }));
      });
      return new FunXMPP.Connection.SyncResult()
      {
        SwellFolks = func("in", true).Where<FunXMPP.Connection.SyncResult.User>((Func<FunXMPP.Connection.SyncResult.User, bool>) (u => u.Jid != null)).ToArray<FunXMPP.Connection.SyncResult.User>(),
        Holdouts = func("out", true).ToArray<FunXMPP.Connection.SyncResult.User>()
      };
    }

    private List<UsyncQuery.StatusResult> ParseStatuses(
      FunXMPP.ProtocolTreeNode usyncNode,
      UsyncQuery.UsyncListOption listOption)
    {
      List<UsyncQuery.StatusResult> statuses = new List<UsyncQuery.StatusResult>();
      FunXMPP.ProtocolTreeNode usyncResultUserList = UsyncQuery.GetUsyncResultUserList(usyncNode, listOption);
      foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((usyncResultUserList != null ? (object) usyncResultUserList.GetAllChildren("user") : (object) null) ?? (object) new FunXMPP.ProtocolTreeNode[0]))
      {
        FunXMPP.ProtocolTreeNode child = protocolTreeNode.GetChild("status");
        if (child != null)
        {
          string attributeValue1 = protocolTreeNode.GetAttributeValue("jid");
          if (!JidChecker.CheckUserJidProtocolString(attributeValue1))
            JidChecker.MaybeSendJidErrorClb("usync parser status", attributeValue1);
          if (child.GetAttributeValue("type") != "fail")
          {
            statuses.Add(new UsyncQuery.StatusResult()
            {
              jid = attributeValue1,
              failed = false,
              status = child.GetDataString(),
              timestamp = child.GetAttributeDateTime("t")
            });
          }
          else
          {
            string attributeValue2 = child.GetAttributeValue("code");
            int result = 500;
            if (attributeValue2 != null)
              int.TryParse(attributeValue2, out result);
            statuses.Add(new UsyncQuery.StatusResult()
            {
              jid = attributeValue1,
              failed = true,
              code = result
            });
          }
        }
      }
      return statuses;
    }

    private void RemoveDuplicatedStatusUsers(
      ref List<UsyncQuery.StatusResult> sidelist,
      ref List<UsyncQuery.StatusResult> list)
    {
      if (this.syncMode != FunXMPP.Connection.SyncMode.Delta)
        return;
      List<UsyncQuery.StatusResult> statusResultList = new List<UsyncQuery.StatusResult>();
      foreach (UsyncQuery.StatusResult statusResult1 in list)
      {
        string jid = statusResult1.jid;
        foreach (UsyncQuery.StatusResult statusResult2 in sidelist)
        {
          if (statusResult2.jid == jid)
          {
            statusResultList.Add(statusResult2);
            break;
          }
        }
      }
      if (statusResultList.Count <= 0)
        return;
      Log.d(UsyncQuery.LogHdr, "Removing {0} duplicated sidelist status users", (object) statusResultList.Count);
      foreach (UsyncQuery.StatusResult statusResult in statusResultList)
        sidelist.Remove(statusResult);
    }

    private List<UsyncQuery.PhotoIdResult> ParsePhotoIds(
      FunXMPP.ProtocolTreeNode usyncNode,
      UsyncQuery.UsyncListOption listOption)
    {
      string attributeValue1 = usyncNode.GetAttributeValue("type");
      List<UsyncQuery.PhotoIdResult> photoIds = new List<UsyncQuery.PhotoIdResult>();
      if (StringComparer.Ordinal.Equals(attributeValue1, "result"))
      {
        FunXMPP.ProtocolTreeNode usyncResultUserList = UsyncQuery.GetUsyncResultUserList(usyncNode, listOption);
        foreach (FunXMPP.ProtocolTreeNode protocolTreeNode in (IEnumerable<FunXMPP.ProtocolTreeNode>) ((usyncResultUserList != null ? (object) usyncResultUserList.GetAllChildren("user") : (object) null) ?? (object) new FunXMPP.ProtocolTreeNode[0]))
        {
          string attributeValue2 = protocolTreeNode.GetAttributeValue("jid");
          FunXMPP.ProtocolTreeNode child = protocolTreeNode.GetChild("picture");
          if (attributeValue2 != null && child != null)
          {
            if (JidChecker.CheckUserJidProtocolString(attributeValue2))
            {
              string attributeValue3 = child?.GetAttributeValue("id");
              photoIds.Add(new UsyncQuery.PhotoIdResult()
              {
                jid = attributeValue2,
                id = attributeValue3
              });
            }
            else
              JidChecker.MaybeSendJidErrorClb("Usync ParsePhoto", attributeValue2);
          }
        }
      }
      return photoIds;
    }

    private void RemoveDuplicatedPictureUsers(
      ref List<UsyncQuery.PhotoIdResult> sidelist,
      ref List<UsyncQuery.PhotoIdResult> list)
    {
      if (this.syncMode != FunXMPP.Connection.SyncMode.Delta)
        return;
      List<UsyncQuery.PhotoIdResult> photoIdResultList = new List<UsyncQuery.PhotoIdResult>();
      foreach (UsyncQuery.PhotoIdResult photoIdResult1 in list)
      {
        string jid = photoIdResult1.jid;
        foreach (UsyncQuery.PhotoIdResult photoIdResult2 in sidelist)
        {
          if (photoIdResult2.jid == jid)
          {
            photoIdResultList.Add(photoIdResult2);
            break;
          }
        }
      }
      if (photoIdResultList.Count <= 0)
        return;
      Log.d(UsyncQuery.LogHdr, "Removing {0} duplicated sidelist picture users", (object) photoIdResultList.Count);
      foreach (UsyncQuery.PhotoIdResult photoIdResult in photoIdResultList)
        sidelist.Remove(photoIdResult);
    }

    private Dictionary<string, RemoteClientCaps> ParseFeatures(
      FunXMPP.ProtocolTreeNode usyncNode,
      UsyncQuery.UsyncListOption listOption)
    {
      DateTime utcNow = DateTime.UtcNow;
      Dictionary<string, RemoteClientCaps> features = new Dictionary<string, RemoteClientCaps>();
      FunXMPP.ProtocolTreeNode usyncResultUserList = UsyncQuery.GetUsyncResultUserList(usyncNode, listOption);
      if (usyncResultUserList != null)
      {
        foreach (FunXMPP.ProtocolTreeNode allChild in usyncResultUserList.GetAllChildren("user"))
        {
          string attributeValue = allChild.GetAttributeValue("jid");
          FunXMPP.ProtocolTreeNode child = allChild.GetChild("feature");
          if (attributeValue != null && child != null)
          {
            RemoteClientCaps remoteClientCaps = (RemoteClientCaps) null;
            if (!features.TryGetValue(attributeValue, out remoteClientCaps))
            {
              remoteClientCaps = FunXMPP.Connection.ParseRemoteClientCaps(attributeValue, utcNow, child);
              features[attributeValue] = remoteClientCaps;
            }
          }
        }
      }
      return features;
    }

    private static FunXMPP.ProtocolTreeNode GetUsyncResultUserList(
      FunXMPP.ProtocolTreeNode iq,
      UsyncQuery.UsyncListOption listOption)
    {
      string tag = listOption == UsyncQuery.UsyncListOption.List ? "list" : "side_list";
      return iq?.GetChild("usync")?.GetChild(tag);
    }

    public void RequestContacts(
      string sid,
      IEnumerable<string> numbers,
      IEnumerable<string> deletedJids)
    {
      this.RequestContacts(sid, 0, true, numbers, deletedJids);
    }

    public void RequestContacts(
      string sid,
      int idx,
      bool last,
      IEnumerable<string> numbers,
      IEnumerable<string> deletedJids)
    {
      this.AddQuery(UsyncQuery.UsyncProtocol.Contact, new FunXMPP.ProtocolTreeNode("contact", new FunXMPP.KeyValue[3]
      {
        new FunXMPP.KeyValue(nameof (sid), sid),
        new FunXMPP.KeyValue("index", idx.ToString()),
        new FunXMPP.KeyValue(nameof (last), last ? "true" : "false")
      }));
      int num = 0;
      foreach (string number in numbers)
      {
        this.AddUserlessProtocolData(new FunXMPP.ProtocolTreeNode("contact", (FunXMPP.KeyValue[]) null, number));
        ++num;
      }
      foreach (string deletedJid in deletedJids)
      {
        if (deletedJid != null)
        {
          this.AddListProtocolData(deletedJid, UsyncQuery.UsyncProtocol.Contact, new FunXMPP.ProtocolTreeNode("contact", new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("type", "delete")
          }));
          ++num;
        }
      }
      Log.l(UsyncQuery.LogHdr, "Sending {0} numbers to sync", (object) num);
    }

    public Action<FunXMPP.Connection.SyncResult> OnGetContacts
    {
      get => this.onGetContacts ?? (Action<FunXMPP.Connection.SyncResult>) (contacts => { });
      set => this.onGetContacts = value;
    }

    public Action<int> OnGetContactsError
    {
      get => this.onGetContactsError ?? (Action<int>) (c => { });
      set => this.onGetContactsError = value;
    }

    public void RequestSidelist(IEnumerable<string> sidelistJids)
    {
      sidelistJids.CheckNotNull<IEnumerable<string>>("Requesting null Sidlist");
      foreach (string sidelistJid in sidelistJids)
        this.AddSidelistProtocolData(sidelistJid, UsyncQuery.UsyncProtocol.Sidelist, (FunXMPP.ProtocolTreeNode) null);
    }

    public Action<FunXMPP.Connection.SyncResult> OnGetSidelist
    {
      get => this.onGetSidelist ?? (Action<FunXMPP.Connection.SyncResult>) (sidelist => { });
      set => this.onGetSidelist = value;
    }

    public Action<int> OnGetSidelistError
    {
      get => this.onGetSidelistError ?? (Action<int>) (c => { });
      set => this.onGetSidelistError = value;
    }

    public bool RequestRemoteCapabilities(
      IEnumerable<string> jids,
      IEnumerable<string> sidelistJids)
    {
      FunXMPP.ProtocolTreeNode[] array = ((IEnumerable<string>) new string[0]).Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (s => new FunXMPP.ProtocolTreeNode(s, (FunXMPP.KeyValue[]) null))).ToArray<FunXMPP.ProtocolTreeNode>();
      if (array == null || !((IEnumerable<FunXMPP.ProtocolTreeNode>) array).Any<FunXMPP.ProtocolTreeNode>())
        return false;
      this.AddSimpleQuery(UsyncQuery.UsyncProtocol.Feature, array);
      foreach (string jid in jids)
        this.AddListProtocolData(jid, UsyncQuery.UsyncProtocol.Feature, (FunXMPP.ProtocolTreeNode) null);
      foreach (string sidelistJid in sidelistJids)
        this.AddSidelistProtocolData(sidelistJid, UsyncQuery.UsyncProtocol.Feature, (FunXMPP.ProtocolTreeNode) null);
      return true;
    }

    public Action<Dictionary<string, RemoteClientCaps>, Dictionary<string, RemoteClientCaps>> OnGetRemoteCapabilities
    {
      get
      {
        return this.onGetRemoteCapabilities ?? (Action<Dictionary<string, RemoteClientCaps>, Dictionary<string, RemoteClientCaps>>) ((features, sidelistFeatures) => Log.l(UsyncQuery.LogHdr, "No OnGetRemoteCapabilities for list={0} and sidelist={1}", features != null ? (object) "?" : (object) "-1", sidelistFeatures != null ? (object) "?" : (object) "-1"));
      }
      set => this.onGetRemoteCapabilities = value;
    }

    public Action<int> OnGetRemoteCapabilitiesError
    {
      get => this.onGetRemoteCapabilitiesError ?? (Action<int>) (err => { });
      set => this.onGetRemoteCapabilitiesError = value;
    }

    public void RequestPictureIds(IEnumerable<string> jids, IEnumerable<string> sidelistJids)
    {
      this.AddSimpleQuery(UsyncQuery.UsyncProtocol.Picture, (FunXMPP.ProtocolTreeNode[]) null);
      foreach (string jid in jids)
        this.AddListProtocolData(jid, UsyncQuery.UsyncProtocol.Picture, (FunXMPP.ProtocolTreeNode) null);
      foreach (string sidelistJid in sidelistJids)
        this.AddSidelistProtocolData(sidelistJid, UsyncQuery.UsyncProtocol.Picture, (FunXMPP.ProtocolTreeNode) null);
    }

    public Action<IEnumerable<UsyncQuery.PhotoIdResult>, IEnumerable<UsyncQuery.PhotoIdResult>> OnGetPhotoIds
    {
      get
      {
        return this.onGetPhotoIds ?? (Action<IEnumerable<UsyncQuery.PhotoIdResult>, IEnumerable<UsyncQuery.PhotoIdResult>>) ((listIds, sidelistIds) => Log.l(UsyncQuery.LogHdr, "No OnGetPhotoIds for list={0} and sidelist={1}", listIds != null ? (object) "?" : (object) "-1", sidelistIds != null ? (object) "?" : (object) "-1"));
      }
      set => this.onGetPhotoIds = value;
    }

    public Action<int> OnGetPhotoIdsError
    {
      get => this.onGetPhotoIdsError ?? (Action<int>) (err => { });
      set => this.onGetPhotoIdsError = value;
    }

    public void RequestStatuses(
      IEnumerable<FunXMPP.Connection.StatusRequest> jids,
      IEnumerable<FunXMPP.Connection.StatusRequest> sidelistJids)
    {
      this.AddSimpleQuery(UsyncQuery.UsyncProtocol.Status, (FunXMPP.ProtocolTreeNode[]) null);
      foreach (FunXMPP.Connection.StatusRequest jid1 in jids)
      {
        DateTime? lastUpdated = jid1.LastUpdated;
        string jid2 = jid1.Jid;
        FunXMPP.KeyValue[] attrs;
        if (!lastUpdated.HasValue)
          attrs = (FunXMPP.KeyValue[]) null;
        else
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("t", lastUpdated.Value.ToUnixTime().ToString())
          };
        FunXMPP.ProtocolTreeNode protocolData = new FunXMPP.ProtocolTreeNode("status", attrs);
        this.AddListProtocolData(jid2, UsyncQuery.UsyncProtocol.Status, protocolData);
      }
      foreach (FunXMPP.Connection.StatusRequest sidelistJid in sidelistJids)
      {
        DateTime? lastUpdated = sidelistJid.LastUpdated;
        string jid = sidelistJid.Jid;
        FunXMPP.KeyValue[] attrs;
        if (!lastUpdated.HasValue)
          attrs = (FunXMPP.KeyValue[]) null;
        else
          attrs = new FunXMPP.KeyValue[1]
          {
            new FunXMPP.KeyValue("t", lastUpdated.Value.ToUnixTime().ToString())
          };
        FunXMPP.ProtocolTreeNode protocolData = new FunXMPP.ProtocolTreeNode("status", attrs);
        this.AddSidelistProtocolData(jid, UsyncQuery.UsyncProtocol.Status, protocolData);
      }
    }

    public Action<IEnumerable<UsyncQuery.StatusResult>, IEnumerable<UsyncQuery.StatusResult>> OnGetStatuses
    {
      get
      {
        return this.onGetStatuses ?? (Action<IEnumerable<UsyncQuery.StatusResult>, IEnumerable<UsyncQuery.StatusResult>>) ((listStatus, sidelistStatus) => Log.l(UsyncQuery.LogHdr, "No OnGetStatuses from list={0} and sidelist={1}", listStatus != null ? (object) "?" : (object) "-1", sidelistStatus != null ? (object) "?" : (object) "-1"));
      }
      set => this.onGetStatuses = value;
    }

    public Action<int> OnGetStatusesError
    {
      get => this.onGetStatusesError ?? (Action<int>) (code => { });
      set => this.onGetStatusesError = value;
    }

    public void RequestBusinesses(
      IEnumerable<FunXMPP.Connection.BusinessRequest> possibleBizUsers,
      IEnumerable<FunXMPP.Connection.BusinessRequest> possibleSidelistUsers)
    {
      this.AddSimpleQuery(UsyncQuery.UsyncProtocol.Business, ((IEnumerable<string>) new string[2]
      {
        "profile",
        "verified_name"
      }).Select<string, FunXMPP.ProtocolTreeNode>((Func<string, FunXMPP.ProtocolTreeNode>) (s => new FunXMPP.ProtocolTreeNode(s, (FunXMPP.KeyValue[]) null))).ToArray<FunXMPP.ProtocolTreeNode>());
      List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList1 = new List<FunXMPP.ProtocolTreeNode>();
      List<FunXMPP.ProtocolTreeNode> protocolTreeNodeList2 = new List<FunXMPP.ProtocolTreeNode>();
      foreach (FunXMPP.Connection.BusinessRequest businessRequest in possibleBizUsers.Concat<FunXMPP.Connection.BusinessRequest>(possibleSidelistUsers))
      {
        if (JidHelper.IsUserJid(businessRequest.Jid))
        {
          UserStatus user = businessRequest.UserStatus != null ? businessRequest.UserStatus : UserCache.Get(businessRequest.Jid, false);
          if (user != null)
          {
            bool deviceContactList = user.IsInDeviceContactList;
            if (user.IsVerified())
            {
              UserStatusProperties.BusinessUserProperties userPropertiesField = user.InternalProperties?.BusinessUserPropertiesField;
              if (userPropertiesField != null)
              {
                string tag = userPropertiesField.Tag;
                if (!string.IsNullOrEmpty(tag))
                {
                  FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("profile", new FunXMPP.KeyValue[1]
                  {
                    new FunXMPP.KeyValue("tag", tag)
                  });
                  if (deviceContactList)
                    protocolTreeNodeList1.Add(protocolTreeNode);
                  else
                    protocolTreeNodeList2.Add(protocolTreeNode);
                }
              }
              VerifiedNameCertificate.Details certificateDetails1 = user.VerifiedNameCertificateDetails;
              ulong? nullable1;
              int num;
              if (certificateDetails1 == null)
              {
                num = 0;
              }
              else
              {
                nullable1 = certificateDetails1.Serial;
                num = nullable1.HasValue ? 1 : 0;
              }
              if (num != 0)
              {
                FunXMPP.KeyValue[] attrs = new FunXMPP.KeyValue[1];
                VerifiedNameCertificate.Details certificateDetails2 = user.VerifiedNameCertificateDetails;
                ulong? nullable2;
                if (certificateDetails2 == null)
                {
                  nullable1 = new ulong?();
                  nullable2 = nullable1;
                }
                else
                {
                  nullable1 = certificateDetails2.Serial;
                  nullable2 = new ulong?(nullable1.Value);
                }
                nullable1 = nullable2;
                attrs[0] = new FunXMPP.KeyValue("serial", ((long) nullable1.Value).ToString());
                FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("verified_name", attrs);
                if (deviceContactList)
                  protocolTreeNodeList1.Add(protocolTreeNode);
                else
                  protocolTreeNodeList2.Add(protocolTreeNode);
              }
            }
            if (deviceContactList)
            {
              FunXMPP.ProtocolTreeNode protocolData = (FunXMPP.ProtocolTreeNode) null;
              if (protocolTreeNodeList1.Count > 0)
                protocolData = new FunXMPP.ProtocolTreeNode("business", (FunXMPP.KeyValue[]) null, protocolTreeNodeList1.ToArray());
              this.AddListProtocolData(businessRequest.Jid, UsyncQuery.UsyncProtocol.Business, protocolData);
            }
            else
            {
              if (protocolTreeNodeList2.Count > 0)
              {
                FunXMPP.ProtocolTreeNode protocolTreeNode = new FunXMPP.ProtocolTreeNode("business", (FunXMPP.KeyValue[]) null, protocolTreeNodeList2.ToArray());
              }
              this.AddSidelistProtocolData(businessRequest.Jid, UsyncQuery.UsyncProtocol.Business, (FunXMPP.ProtocolTreeNode) null);
            }
          }
        }
      }
    }

    public Action<IEnumerable<UsyncQuery.BusinessResult>, IEnumerable<UsyncQuery.BusinessResult>> OnGetBusinesses
    {
      get
      {
        return this.onGetBusinesses ?? (Action<IEnumerable<UsyncQuery.BusinessResult>, IEnumerable<UsyncQuery.BusinessResult>>) ((list, sidelist) => Log.l(UsyncQuery.LogHdr, "No OnGetBusinesses for list={0} and sidelist={1}", list != null ? (object) "?" : (object) "-1", sidelist != null ? (object) "?" : (object) "-1"));
      }
      set => this.onGetBusinesses = value;
    }

    public Action<int> OnGetBusinessesError
    {
      get
      {
        return this.onGetBusinessesError ?? (Action<int>) (code => Log.l(UsyncQuery.LogHdr, "No OnGetBusinessError: {0}", (object) code));
      }
      set => this.onGetBusinessesError = value;
    }

    private void AddSimpleQuery(
      UsyncQuery.UsyncProtocol protocol,
      FunXMPP.ProtocolTreeNode[] children)
    {
      this.AddQuery(protocol, new FunXMPP.ProtocolTreeNode(UsyncQuery.ProtocolToString(protocol), (FunXMPP.KeyValue[]) null, children));
    }

    private void AddQuery(UsyncQuery.UsyncProtocol protocol, FunXMPP.ProtocolTreeNode requestData)
    {
      this.protocols.Add(protocol);
      this.queryProtocols[protocol] = requestData;
    }

    private void AddListProtocolData(
      string userJid,
      UsyncQuery.UsyncProtocol protocol,
      FunXMPP.ProtocolTreeNode protocolData)
    {
      if (!JidChecker.CheckUserJidProtocolString(userJid))
      {
        Log.l(UsyncQuery.LogHdr, "Non user jid found {0}, proto {1}", (object) userJid, (object) protocol);
        JidChecker.MaybeSendJidErrorClb("Usync add user list", userJid);
      }
      else
      {
        Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode> dictionary = (Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>) null;
        if (!this.userDictionary.TryGetValue(userJid, out dictionary))
        {
          dictionary = new Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>();
          this.userDictionary.Add(userJid, dictionary);
        }
        if (protocolData == null)
          return;
        dictionary[protocol] = protocolData;
      }
    }

    private void AddUserlessProtocolData(FunXMPP.ProtocolTreeNode[] protocolData)
    {
      this.userlessProtocolData.Add(new FunXMPP.ProtocolTreeNode("user", (FunXMPP.KeyValue[]) null, protocolData));
    }

    private void AddSidelistProtocolData(
      string userJid,
      UsyncQuery.UsyncProtocol protocol,
      FunXMPP.ProtocolTreeNode protocolData)
    {
      Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode> dictionary = (Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>) null;
      if (!this.sidelistDictionary.TryGetValue(userJid, out dictionary))
      {
        if (JidChecker.CheckUserJidProtocolString(userJid))
        {
          dictionary = new Dictionary<UsyncQuery.UsyncProtocol, FunXMPP.ProtocolTreeNode>();
          this.sidelistDictionary.Add(userJid, dictionary);
        }
        else
        {
          JidChecker.MaybeSendJidErrorClb("usync sidelist", userJid);
          return;
        }
      }
      if (protocolData == null)
        return;
      dictionary[protocol] = protocolData;
    }

    private void AddUserlessProtocolData(FunXMPP.ProtocolTreeNode protocolData)
    {
      this.AddUserlessProtocolData(new FunXMPP.ProtocolTreeNode[1]
      {
        protocolData
      });
    }

    public enum UsyncProtocol
    {
      Invalid = 1,
      Contact = 2,
      Picture = 3,
      Status = 4,
      Feature = 5,
      Business = 6,
      Sidelist = 7,
    }

    public enum UsyncListOption
    {
      List,
      SideList,
    }

    public sealed class BusinessResult
    {
      public string Jid;
      public bool IsBusiness;
      public string VerifiedLevel;
      public byte[] Certificate;
      public BizProfileDetails BizProfile;
    }

    public sealed class StatusResult
    {
      public string jid;
      public bool failed;
      public int code;
      public string status;
      public DateTime? timestamp;
    }

    public sealed class PhotoIdResult
    {
      public string jid;
      public string id;
    }
  }
}
