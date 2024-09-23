// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpWebRequest
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Globalization;
using System.IO;
using System.Text;

#nullable disable
namespace System.Net.Http
{
  internal class HttpWebRequest
  {
    private System.Net.HttpWebRequest actualRequest;
    private bool requestSubmitted;
    private Uri hostUri;
    private bool hasHostPort;
    private bool keepAlive = true;
    private DecompressionMethods automaticDecompression;

    public HttpWebRequest(Uri uri)
    {
      this.actualRequest = (System.Net.HttpWebRequest) WebRequest.Create(uri);
      HttpWebRequestLightup.Current.TrySetAllowReadStreamBuffering(this.actualRequest, false);
      this.UseDefaultCredentials = false;
    }

    public string Accept
    {
      get => this.actualRequest.Accept;
      set => this.actualRequest.Accept = value;
    }

    public string ContentType
    {
      get => this.actualRequest.ContentType;
      set => this.actualRequest.ContentType = value;
    }

    public CookieContainer CookieContainer
    {
      get => this.actualRequest.CookieContainer;
      set
      {
        try
        {
          this.actualRequest.CookieContainer = value;
        }
        catch (NotImplementedException ex)
        {
          if (value.Count == 0)
            return;
          throw;
        }
      }
    }

    public ICredentials Credentials
    {
      get => this.actualRequest.Credentials;
      set
      {
        try
        {
          this.actualRequest.Credentials = value;
        }
        catch (NotImplementedException ex)
        {
          if (value == null)
            return;
          throw;
        }
      }
    }

    public bool HaveResponse => this.actualRequest.HaveResponse;

    public WebHeaderCollection Headers
    {
      get => this.actualRequest.Headers;
      set => this.actualRequest.Headers = value;
    }

    public string Method
    {
      get => this.actualRequest.Method;
      set => this.actualRequest.Method = value;
    }

    public Uri RequestUri => this.actualRequest.RequestUri;

    public virtual bool SupportsCookieContainer => this.actualRequest.SupportsCookieContainer;

    public bool UseDefaultCredentials
    {
      get => this.actualRequest.UseDefaultCredentials;
      set
      {
        try
        {
          this.actualRequest.UseDefaultCredentials = value;
        }
        catch (NotSupportedException ex)
        {
          if (!value)
            return;
          throw;
        }
      }
    }

    public void Abort() => this.actualRequest.Abort();

    public IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
    {
      this.UpdateRequest();
      this.requestSubmitted = true;
      return this.actualRequest.BeginGetRequestStream(callback, state);
    }

    public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
    {
      this.UpdateRequest();
      this.requestSubmitted = true;
      return this.actualRequest.BeginGetResponse(callback, state);
    }

    public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context)
    {
      context = (TransportContext) null;
      return this.actualRequest.EndGetRequestStream(asyncResult);
    }

    public WebResponse EndGetResponse(IAsyncResult asyncResult)
    {
      return HttpWebResponse.CreateDecorator(this.actualRequest.EndGetResponse(asyncResult), this.automaticDecompression);
    }

    public DecompressionMethods AutomaticDecompression
    {
      get => this.automaticDecompression;
      set
      {
        if (this.requestSubmitted)
          throw new InvalidOperationException(SysSR.net_writestarted);
        this.automaticDecompression = value == DecompressionMethods.None || Capabilities.SupportsAutomaticDecompression ? value : throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.AutomaticDecompression", (object) "HttpClientHandler.SupportsAutomaticDecompression"));
      }
    }

    public bool AllowAutoRedirect
    {
      get
      {
        bool allowAutoRedirect;
        if (!HttpWebRequestLightup.Current.TryGetAllowAutoRedirect(this.actualRequest, out allowAutoRedirect))
          allowAutoRedirect = true;
        return allowAutoRedirect;
      }
      set
      {
        if (!HttpWebRequestLightup.Current.TrySetAllowAutoRedirect(this.actualRequest, value) && !value)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.AllowAutoRedirect", (object) "HttpClientHandler.SupportsAllowAutoRedirect()"));
      }
    }

    public string Connection
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetConnection(this.actualRequest, value))
          return;
        string lower = value.ToLower();
        if (lower.Contains("keep-alive") || lower.Contains("close"))
          throw new ArgumentException(SysSR.net_connarg);
        this.SetHeader(HttpRequestHeader.Connection, value);
      }
    }

    public long ContentLength
    {
      set => HttpWebRequestLightup.Current.SetContentLength(this.actualRequest, value);
    }

    public DateTime Date
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetDate(this.actualRequest, value))
          return;
        this.SetDateHelper(value, HttpRequestHeader.Date);
      }
    }

    public string Expect
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetExpect(this.actualRequest, value))
          return;
        this.SetHeader(HttpRequestHeader.Expect, value);
      }
    }

    public string Host
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetHost(this.actualRequest, value))
          return;
        if (this.requestSubmitted)
          throw new InvalidOperationException(SysSR.net_writestarted);
        if (value == null)
          throw new ArgumentNullException();
        Uri hostUri;
        if (value.IndexOf('/') != -1 || !this.TryGetHostUri(value, out hostUri))
          throw new ArgumentException(SysSR.net_invalid_host);
        this.hostUri = hostUri;
        if (!UriHelper.IsDefaultPort(hostUri))
          this.hasHostPort = true;
        else if (value.IndexOf(':') == -1)
        {
          this.hasHostPort = false;
        }
        else
        {
          int num = value.IndexOf(']');
          if (num == -1)
            this.hasHostPort = true;
          else
            this.hasHostPort = value.LastIndexOf(':') > num;
        }
      }
    }

    public DateTime IfModifiedSince
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetIfModifiedSince(this.actualRequest, value))
          return;
        this.SetDateHelper(value, HttpRequestHeader.IfModifiedSince);
      }
    }

    public bool KeepAlive
    {
      set => this.keepAlive = value;
    }

    public int MaximumAutomaticRedirections
    {
      set
      {
        if (5 != value)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.MaximumAutomaticRedirections", (object) "HttpClientHandler.SupportsRedirectConfiguration"));
      }
    }

    public bool PreAuthenticate
    {
      set
      {
        if (value)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.PreAuthenticate", (object) "HttpClientHandler.SupportsPreAuthenticate()"));
      }
    }

    public Version ProtocolVersion
    {
      set
      {
        if (value != HttpVersion.Version11)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.ProtocolVersion", (object) "HttpClientHandler.SupportsProtocolVersion()"));
      }
    }

    public IWebProxy Proxy
    {
      set
      {
        if (value != null)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.Proxy", (object) "HttpClientHandler.SupportsProxy"));
        throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "HttpClientHandler.UseProxy", (object) "HttpClientHandler.SupportsUseProxy()"));
      }
    }

    public string Referer
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetReferer(this.actualRequest, value))
          return;
        this.SetHeader(HttpRequestHeader.Referer, value);
      }
    }

    public bool SendChunked
    {
      set
      {
        if (value)
          throw new NotSupportedException(string.Format(SysSR.http_capabilitynotsupported, (object) "\"Transfer-Encoding: chunked\"", (object) "HttpClientHandler.SupportsSendChunked()"));
      }
    }

    public ServicePoint ServicePoint => ServicePoint.Instance;

    public int Timeout
    {
      set => HttpWebRequestLightup.Current.TrySetTimeout(this.actualRequest, value);
    }

    public string TransferEncoding
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetTransferEncoding(this.actualRequest, value))
          return;
        this.SetHeader(HttpRequestHeader.TransferEncoding, value);
      }
    }

    public string UserAgent
    {
      set
      {
        if (HttpWebRequestLightup.Current.TrySetUserAgent(this.actualRequest, value))
          return;
        this.SetHeader(HttpRequestHeader.UserAgent, value);
      }
    }

    public void AddRange(long range)
    {
      if (HttpWebRequestLightup.Current.TryCallAddRange(this.actualRequest, range))
        return;
      this.actualRequest.Headers[HttpRequestHeader.Range] = this.CreateRangeString(range.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo), range >= 0L ? "" : (string) null);
    }

    public void AddRange(long from, long to)
    {
      if (HttpWebRequestLightup.Current.TryCallAddRange(this.actualRequest, from, to))
        return;
      if (from < 0L || to < 0L)
        throw new ArgumentOutOfRangeException("from, to", (object) string.Format("{0}, {1}", (object) from, (object) to), SysSR.net_rangetoosmall);
      this.actualRequest.Headers[HttpRequestHeader.Range] = from <= to ? this.CreateRangeString(from.ToString(), to.ToString()) : throw new ArgumentOutOfRangeException("from, to", (object) string.Format("{0}, {1}", (object) from, (object) to), SysSR.net_fromto);
    }

    private string CreateRangeString(string from, string to)
    {
      string strB = "bytes";
      string header = this.actualRequest.Headers[HttpRequestHeader.Range];
      string str;
      if (string.IsNullOrEmpty(header))
      {
        str = strB + "=";
      }
      else
      {
        if (string.Compare(header.Substring(0, header.IndexOf('=')), strB, StringComparison.OrdinalIgnoreCase) != 0)
          throw new InvalidOperationException(SysSR.net_rangetype);
        str = string.Empty;
      }
      string rangeString = str + from.ToString();
      if (to != null)
        rangeString = rangeString + "-" + to.ToString();
      return rangeString;
    }

    public static IWebProxy DefaultWebProxy => (IWebProxy) null;

    private void DisableWriteStreamBuffering()
    {
      if (this.AllowAutoRedirect)
        return;
      HttpWebRequestLightup.Current.TrySetAllowWriteStreamBuffering(this.actualRequest, false);
    }

    private static string GetSafeHostAndPort(Uri sourceUri, bool addDefaultPort)
    {
      return !addDefaultPort && UriHelper.IsDefaultPort(sourceUri) ? sourceUri.DnsSafeHost : sourceUri.DnsSafeHost + ":" + (object) sourceUri.Port;
    }

    private void SetDateHelper(DateTime value, HttpRequestHeader header)
    {
      string str = value == DateTime.MinValue ? (string) null : value.ToUniversalTime().ToString("R", (IFormatProvider) new DateTimeFormatInfo());
      this.SetHeader(header, str);
    }

    private void SetHeader(HttpRequestHeader header, string value)
    {
      if (string.IsNullOrEmpty(value))
        this.actualRequest.Headers[header] = (string) null;
      else
        this.actualRequest.Headers[header] = value;
    }

    private void TryAddHeader(string header, string value)
    {
      try
      {
        this.actualRequest.Headers.Add(header, value);
      }
      catch (ArgumentException ex)
      {
      }
    }

    private bool TryGetHostUri(string hostName, out Uri hostUri)
    {
      StringBuilder stringBuilder = new StringBuilder(this.actualRequest.RequestUri.Scheme);
      stringBuilder.Append("://");
      stringBuilder.Append(hostName);
      stringBuilder.Append(this.actualRequest.RequestUri.AbsolutePath);
      stringBuilder.Append(this.actualRequest.RequestUri.Query);
      return Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out hostUri);
    }

    private void UpdateRequest()
    {
      if (this.requestSubmitted)
        return;
      this.DisableWriteStreamBuffering();
      if ((Uri) null != this.hostUri)
        this.actualRequest.Headers[HttpRequestHeader.Host] = HttpWebRequest.GetSafeHostAndPort(this.hostUri, this.hasHostPort);
      if (!HttpWebRequestLightup.Current.TrySetKeepAlive(this.actualRequest, this.keepAlive))
      {
        string str = this.keepAlive ? "Keep-Alive" : "Close";
        if (!string.IsNullOrEmpty(this.actualRequest.Headers[HttpRequestHeader.Connection]))
          str = string.Join(", ", new string[2]
          {
            this.actualRequest.Headers[HttpRequestHeader.Connection],
            str
          });
        try
        {
          this.actualRequest.Headers[HttpRequestHeader.Connection] = str;
        }
        catch (ArgumentException ex)
        {
        }
      }
      string str1 = this.actualRequest.Headers[HttpRequestHeader.AcceptEncoding] ?? string.Empty;
      if ((this.AutomaticDecompression & DecompressionMethods.GZip) != DecompressionMethods.None && str1.IndexOf("gzip", StringComparison.OrdinalIgnoreCase) < 0)
      {
        if ((this.AutomaticDecompression & DecompressionMethods.Deflate) != DecompressionMethods.None && str1.IndexOf("deflate", StringComparison.OrdinalIgnoreCase) < 0)
          this.TryAddHeader("Accept-Encoding", "gzip, deflate");
        else
          this.TryAddHeader("Accept-Encoding", "gzip");
      }
      else
      {
        if ((this.AutomaticDecompression & DecompressionMethods.Deflate) == DecompressionMethods.None || str1.IndexOf("deflate", StringComparison.OrdinalIgnoreCase) >= 0)
          return;
        this.TryAddHeader("Accept-Encoding", "deflate");
      }
    }
  }
}
