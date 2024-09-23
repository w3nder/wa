// Decompiled with JetBrains decompiler
// Type: System.Net.HttpWebRequestLightup
// Assembly: System.Net.Http.Extensions, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4532C01-CAE1-4FEB-922A-3FFFB1361F31
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.Extensions.xml

#nullable disable
namespace System.Net
{
  internal class HttpWebRequestLightup : Lightup
  {
    public static readonly HttpWebRequestLightup Current = new HttpWebRequestLightup();
    private Delegate _getAllowAutoRedirect;
    private Delegate _setAllowAutoRedirect;
    private Delegate _setAllowReadStreamBuffering;
    private Delegate _setAllowWriteStreamBuffering;
    private Delegate _setConnection;
    private Delegate _setContentLength;
    private Delegate _setDate;
    private Delegate _setExpect;
    private Delegate _setHost;
    private Delegate _setIfModifiedSince;
    private Delegate _setKeepAlive;
    private Delegate _setReferer;
    private Delegate _setTimeout;
    private Delegate _setTransferEncoding;
    private Delegate _setUserAgent;
    private Delegate _addRange1;
    private Delegate _addRange2;

    public HttpWebRequestLightup()
      : base(typeof (HttpWebRequest))
    {
    }

    public bool TryGetAllowAutoRedirect(HttpWebRequest instance, out bool value)
    {
      return this.TryGet<HttpWebRequest, bool>(ref this._getAllowAutoRedirect, instance, "AllowAutoRedirect", out value);
    }

    public bool TrySetAllowAutoRedirect(HttpWebRequest instance, bool value)
    {
      return this.TrySet<HttpWebRequest, bool>(ref this._setAllowAutoRedirect, instance, "AllowAutoRedirect", value);
    }

    public bool TrySetAllowReadStreamBuffering(HttpWebRequest instance, bool value)
    {
      return this.TrySet<HttpWebRequest, bool>(ref this._setAllowReadStreamBuffering, instance, "AllowReadStreamBuffering", value);
    }

    public bool TrySetAllowWriteStreamBuffering(HttpWebRequest instance, bool value)
    {
      return this.TrySet<HttpWebRequest, bool>(ref this._setAllowWriteStreamBuffering, instance, "AllowWriteStreamBuffering", value);
    }

    public bool TrySetConnection(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setConnection, instance, "Connection", value);
    }

    public void SetContentLength(HttpWebRequest instance, long value)
    {
      this.Set<HttpWebRequest, long>(ref this._setContentLength, instance, "ContentLength", value);
    }

    public bool TrySetDate(HttpWebRequest instance, DateTime value)
    {
      return this.TrySet<HttpWebRequest, DateTime>(ref this._setDate, instance, "Date", value);
    }

    public bool TrySetExpect(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setExpect, instance, "Expect", value);
    }

    public bool TrySetHost(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setHost, instance, "Host", value);
    }

    public bool TrySetIfModifiedSince(HttpWebRequest instance, DateTime value)
    {
      return this.TrySet<HttpWebRequest, DateTime>(ref this._setIfModifiedSince, instance, "IfModifiedSince", value);
    }

    public bool TrySetKeepAlive(HttpWebRequest instance, bool value)
    {
      return this.TrySet<HttpWebRequest, bool>(ref this._setKeepAlive, instance, "KeepAlive", value);
    }

    public bool TrySetReferer(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setReferer, instance, "Referer", value);
    }

    public bool TrySetTimeout(HttpWebRequest instance, int value)
    {
      return this.TrySet<HttpWebRequest, int>(ref this._setTimeout, instance, "Timeout", value);
    }

    public bool TrySetTransferEncoding(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setTransferEncoding, instance, "TransferEncoding", value);
    }

    public bool TrySetUserAgent(HttpWebRequest instance, string value)
    {
      return this.TrySet<HttpWebRequest, string>(ref this._setUserAgent, instance, "UserAgent", value);
    }

    public bool TryCallAddRange(HttpWebRequest instance, long from, long to)
    {
      return this.TryCall<HttpWebRequest, long, long>(ref this._addRange2, instance, "AddRange", from, to);
    }

    public bool TryCallAddRange(HttpWebRequest instance, long range)
    {
      return this.TryCall<HttpWebRequest, long>(ref this._addRange1, instance, "AddRange", range);
    }
  }
}
