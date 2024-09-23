// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpResponseHeaders
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the collection of Response Headers as defined in RFC 2616.</summary>
  public sealed class HttpResponseHeaders : HttpHeaders
  {
    private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> invalidHeaders;
    private HttpGeneralHeaders generalHeaders;
    private HttpHeaderValueCollection<string> acceptRanges;
    private HttpHeaderValueCollection<AuthenticationHeaderValue> wwwAuthenticate;
    private HttpHeaderValueCollection<AuthenticationHeaderValue> proxyAuthenticate;
    private HttpHeaderValueCollection<ProductInfoHeaderValue> server;
    private HttpHeaderValueCollection<string> vary;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<string> AcceptRanges
    {
      get
      {
        if (this.acceptRanges == null)
          this.acceptRanges = new HttpHeaderValueCollection<string>("Accept-Ranges", (HttpHeaders) this, HeaderUtilities.TokenValidator);
        return this.acceptRanges;
      }
    }

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? Age
    {
      get => HeaderUtilities.GetTimeSpanValue(nameof (Age), (HttpHeaders) this);
      set => this.SetOrRemoveParsedValue(nameof (Age), (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
    public EntityTagHeaderValue ETag
    {
      get => (EntityTagHeaderValue) this.GetParsedValues(nameof (ETag));
      set => this.SetOrRemoveParsedValue(nameof (ETag), (object) value);
    }

    /// <returns>Returns <see cref="T:System.Uri" />.</returns>
    public Uri Location
    {
      get => (Uri) this.GetParsedValues(nameof (Location));
      set => this.SetOrRemoveParsedValue(nameof (Location), (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate
    {
      get
      {
        if (this.proxyAuthenticate == null)
          this.proxyAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("Proxy-Authenticate", (HttpHeaders) this);
        return this.proxyAuthenticate;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" />.</returns>
    public RetryConditionHeaderValue RetryAfter
    {
      get => (RetryConditionHeaderValue) this.GetParsedValues("Retry-After");
      set => this.SetOrRemoveParsedValue("Retry-After", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<ProductInfoHeaderValue> Server
    {
      get
      {
        if (this.server == null)
          this.server = new HttpHeaderValueCollection<ProductInfoHeaderValue>(nameof (Server), (HttpHeaders) this);
        return this.server;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<string> Vary
    {
      get
      {
        if (this.vary == null)
          this.vary = new HttpHeaderValueCollection<string>(nameof (Vary), (HttpHeaders) this, HeaderUtilities.TokenValidator);
        return this.vary;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate
    {
      get
      {
        if (this.wwwAuthenticate == null)
          this.wwwAuthenticate = new HttpHeaderValueCollection<AuthenticationHeaderValue>("WWW-Authenticate", (HttpHeaders) this);
        return this.wwwAuthenticate;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" />.</returns>
    public CacheControlHeaderValue CacheControl
    {
      get => this.generalHeaders.CacheControl;
      set => this.generalHeaders.CacheControl = value;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<string> Connection => this.generalHeaders.Connection;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool? ConnectionClose
    {
      get => this.generalHeaders.ConnectionClose;
      set => this.generalHeaders.ConnectionClose = value;
    }

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? Date
    {
      get => this.generalHeaders.Date;
      set => this.generalHeaders.Date = value;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<NameValueHeaderValue> Pragma => this.generalHeaders.Pragma;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<string> Trailer => this.generalHeaders.Trailer;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
    {
      get => this.generalHeaders.TransferEncoding;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool? TransferEncodingChunked
    {
      get => this.generalHeaders.TransferEncodingChunked;
      set => this.generalHeaders.TransferEncodingChunked = value;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<ProductHeaderValue> Upgrade => this.generalHeaders.Upgrade;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<ViaHeaderValue> Via => this.generalHeaders.Via;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<WarningHeaderValue> Warning => this.generalHeaders.Warning;

    internal HttpResponseHeaders()
    {
      this.generalHeaders = new HttpGeneralHeaders((HttpHeaders) this);
      this.SetConfiguration(HttpResponseHeaders.parserStore, HttpResponseHeaders.invalidHeaders);
    }

    static HttpResponseHeaders()
    {
      HttpResponseHeaders.parserStore.Add("Accept-Ranges", GenericHeaderParser.TokenListParser);
      HttpResponseHeaders.parserStore.Add(nameof (Age), (HttpHeaderParser) TimeSpanHeaderParser.Parser);
      HttpResponseHeaders.parserStore.Add(nameof (ETag), GenericHeaderParser.SingleValueEntityTagParser);
      HttpResponseHeaders.parserStore.Add(nameof (Location), (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser);
      HttpResponseHeaders.parserStore.Add("Proxy-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
      HttpResponseHeaders.parserStore.Add("Retry-After", GenericHeaderParser.RetryConditionParser);
      HttpResponseHeaders.parserStore.Add(nameof (Server), (HttpHeaderParser) ProductInfoHeaderParser.MultipleValueParser);
      HttpResponseHeaders.parserStore.Add(nameof (Vary), GenericHeaderParser.TokenListParser);
      HttpResponseHeaders.parserStore.Add("WWW-Authenticate", GenericHeaderParser.MultipleValueAuthenticationParser);
      HttpGeneralHeaders.AddParsers(HttpResponseHeaders.parserStore);
      HttpResponseHeaders.invalidHeaders = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HttpContentHeaders.AddKnownHeaders(HttpResponseHeaders.invalidHeaders);
    }

    internal static void AddKnownHeaders(HashSet<string> headerSet)
    {
      Contract.Requires(headerSet != null);
      headerSet.Add("Accept-Ranges");
      headerSet.Add("Age");
      headerSet.Add("ETag");
      headerSet.Add("Location");
      headerSet.Add("Proxy-Authenticate");
      headerSet.Add("Retry-After");
      headerSet.Add("Server");
      headerSet.Add("Vary");
      headerSet.Add("WWW-Authenticate");
    }

    internal override void AddHeaders(HttpHeaders sourceHeaders)
    {
      base.AddHeaders(sourceHeaders);
      HttpResponseHeaders httpResponseHeaders = sourceHeaders as HttpResponseHeaders;
      Contract.Assert(httpResponseHeaders != null);
      this.generalHeaders.AddSpecialsFrom(httpResponseHeaders.generalHeaders);
    }
  }
}
