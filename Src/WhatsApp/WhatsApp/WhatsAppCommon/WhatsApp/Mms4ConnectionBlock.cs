// Decompiled with JetBrains decompiler
// Type: WhatsApp.Mms4ConnectionBlock
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WhatsApp.Events;


namespace WhatsApp
{
  public class Mms4ConnectionBlock
  {
    private const string LogHdr = "mms4cb";
    private const int MINTTL_120_SECS = 120;
    private const int MINTTL_DOWNGRADE_PROPORTION = 5;
    private object updateLock = new object();
    public bool IsUpdated;
    private List<Mms4ConnectionBlock.CbHost> hosts = new List<Mms4ConnectionBlock.CbHost>();
    private const string AudioType = "audio";
    private const string DocType = "document";
    private const string GifType = "gif";
    private const string ImageType = "image";
    private const string PttType = "ptt";
    private const string StickerType = "sticker";
    private const string VideoType = "video";
    private static string[] Mms4MediaTypes = new string[8]
    {
      "image",
      "sticker",
      "ptt",
      "audio",
      "document",
      "video",
      "gif",
      "ppic"
    };

    public string AuthToken { get; private set; }

    public long RouteExpiryTime { get; private set; }

    public long AuthExpiryTime { get; private set; }

    public int HostCount
    {
      get
      {
        lock (this.updateLock)
          return this.hosts.Count<Mms4ConnectionBlock.CbHost>();
      }
    }

    public string[] GetHostList()
    {
      lock (this.updateLock)
      {
        string[] hostList = new string[this.hosts.Count<Mms4ConnectionBlock.CbHost>()];
        int index = 0;
        foreach (Mms4ConnectionBlock.CbHost host in this.hosts)
        {
          hostList[index] = host.HostName;
          ++index;
        }
        return hostList;
      }
    }

    public string GetHostname(bool downloadFlag, FunXMPP.FMessage.FunMediaType funMediaType)
    {
      string mms4MediaString = Mms4ConnectionBlock.ConvertFunMediaTypeToMms4MediaString(funMediaType);
      lock (this.updateLock)
      {
        foreach (Mms4ConnectionBlock.CbHost host in this.hosts)
        {
          if (host.IsMediaSupported(downloadFlag, mms4MediaString))
            return host.HostName;
        }
        foreach (Mms4ConnectionBlock.CbHost host in this.hosts)
        {
          if (host.IsTransferSupported(downloadFlag))
          {
            Log.l("mms4cb", "No host supported {0}, using first that supported {1}", (object) mms4MediaString, downloadFlag ? (object) "download" : (object) "upload");
            return host.HostName;
          }
        }
      }
      return (string) null;
    }

    internal bool MarkHostAsBad(string hostName)
    {
      Mms4ConnectionBlock.CbHost cbHost = (Mms4ConnectionBlock.CbHost) null;
      lock (this.updateLock)
      {
        foreach (Mms4ConnectionBlock.CbHost host in this.hosts)
        {
          if (hostName == host.HostName)
            cbHost = host;
        }
        if (cbHost != null)
          this.hosts.Remove(cbHost);
      }
      return cbHost != null;
    }

    internal bool[] MarkHostAsGood(string hostName)
    {
      bool[] flagArray = new bool[2];
      lock (this.updateLock)
      {
        foreach (Mms4ConnectionBlock.CbHost host in this.hosts)
        {
          if (hostName == host.HostName)
          {
            host.CheckedState = Mms4ConnectionBlock.CbHost.CbHostCheckStatus.OK;
            flagArray[0] = host.IsMediaSupported(true, "image");
            flagArray[1] = host.IsMediaSupported(false, "image");
            break;
          }
        }
      }
      return flagArray;
    }

    private Mms4ConnectionBlock()
    {
    }

    public Mms4ConnectionBlock(
      FunXMPP.ProtocolTreeNode mediaConnNode,
      ref RouteSelection routeSelectionFs)
    {
      long ticks = DateTime.UtcNow.Ticks;
      this.AuthToken = mediaConnNode.GetAttributeValue("auth");
      string attributeValue1 = mediaConnNode.GetAttributeValue("ttl");
      long num1 = 0;
      ref long local1 = ref num1;
      if (!long.TryParse(attributeValue1, out local1))
      {
        Log.l("mms4", "media connection node has incorrect ttl");
        throw new InvalidOperationException("Connection block is badly formatted");
      }
      long num2 = num1 - Math.Min(num1 / 5L, 120L);
      this.RouteExpiryTime = ticks + TimeSpan.FromSeconds((double) num2).Ticks;
      string attributeValue2 = mediaConnNode.GetAttributeValue("auth_ttl");
      long num3 = 0;
      ref long local2 = ref num3;
      if (!long.TryParse(attributeValue2, out local2))
      {
        Log.l("mms4", "media connection node has incorrect auth_ttl");
        num3 = num2;
      }
      this.AuthExpiryTime = ticks + TimeSpan.FromSeconds((double) num3).Ticks;
      if (mediaConnNode.GetAllChildren("host") != null)
      {
        foreach (FunXMPP.ProtocolTreeNode allChild in mediaConnNode.GetAllChildren("host"))
        {
          Mms4ConnectionBlock.CbHost cbHost = new Mms4ConnectionBlock.CbHost(allChild);
          if (cbHost.HostName != null)
            this.hosts.Add(cbHost);
        }
      }
      if (this.hosts.Count == 0)
      {
        Log.l("mms4", "auth request failed to find any nodes");
        throw new InvalidOperationException("Connection block is badly formatted");
      }
      Log.d("mms4", "found {0} nodes", (object) this.hosts.Count);
    }

    public static string ConvertFunMediaTypeToMms4MediaString(
      FunXMPP.FMessage.FunMediaType mediaType)
    {
      switch (mediaType)
      {
        case FunXMPP.FMessage.FunMediaType.Image:
          return "image";
        case FunXMPP.FMessage.FunMediaType.Audio:
          return "audio";
        case FunXMPP.FMessage.FunMediaType.Video:
          return "video";
        case FunXMPP.FMessage.FunMediaType.Document:
          return "document";
        case FunXMPP.FMessage.FunMediaType.Gif:
          return "gif";
        case FunXMPP.FMessage.FunMediaType.Sticker:
          return "sticker";
        case FunXMPP.FMessage.FunMediaType.Ptt:
          return "ptt";
        default:
          Log.l("mms4cb", "Unexpected asked to convert mediatype {0} to string", (object) mediaType.ToString());
          return (string) null;
      }
    }

    private class CbHost
    {
      public Mms4ConnectionBlock.CbHost.CbHostCheckStatus CheckedState;
      private uint uploadSupportBitmap;
      private uint downloadSupportBitmap;

      public string HostName { get; private set; }

      public Mms4ConnectionBlock.CbHost.CbHostTypes HostType { get; private set; }

      public string HostClass { get; private set; }

      public bool IsMediaSupported(bool downloadFlag, string mediaType)
      {
        uint num = downloadFlag ? this.downloadSupportBitmap : this.uploadSupportBitmap;
        if (num == 0U)
          return false;
        for (int index = 0; index < Mms4ConnectionBlock.Mms4MediaTypes.Length; ++index)
        {
          if (mediaType == Mms4ConnectionBlock.Mms4MediaTypes[index])
            return ((int) (num >> index) & 1) == 1;
        }
        return false;
      }

      public bool IsTransferSupported(bool downloadFlag)
      {
        return (downloadFlag ? this.downloadSupportBitmap : this.uploadSupportBitmap) > 0U;
      }

      public CbHost(FunXMPP.ProtocolTreeNode hostNode)
      {
        this.HostName = hostNode.GetAttributeValue("hostname");
        this.HostType = !(hostNode.GetAttributeValue("type") == "fallback") ? Mms4ConnectionBlock.CbHost.CbHostTypes.Primary : Mms4ConnectionBlock.CbHost.CbHostTypes.Fallback;
        this.HostClass = hostNode.GetAttributeValue("class");
        this.uploadSupportBitmap = uint.MaxValue;
        this.downloadSupportBitmap = uint.MaxValue;
        FunXMPP.ProtocolTreeNode[] children = hostNode.children;
        if (children == null)
          return;
        foreach (FunXMPP.ProtocolTreeNode directionNode in children)
        {
          if (directionNode.tag == "upload")
            this.uploadSupportBitmap = this.ExtractMediaSupported(directionNode);
          if (directionNode.tag == "download")
            this.downloadSupportBitmap = this.ExtractMediaSupported(directionNode);
        }
      }

      private uint ExtractMediaSupported(FunXMPP.ProtocolTreeNode directionNode)
      {
        uint mediaSupported = 0;
        foreach (FunXMPP.ProtocolTreeNode child in directionNode.children)
        {
          string tag = child.tag;
          for (int index = 0; index < Mms4ConnectionBlock.Mms4MediaTypes.Length; ++index)
          {
            if (Mms4ConnectionBlock.Mms4MediaTypes[index] == tag)
              mediaSupported |= (uint) (1 << index);
          }
        }
        return mediaSupported;
      }

      public enum CbHostCheckStatus
      {
        Unchecked,
        OK,
      }

      public enum CbHostTypes
      {
        Primary,
        Fallback,
      }
    }
  }
}
