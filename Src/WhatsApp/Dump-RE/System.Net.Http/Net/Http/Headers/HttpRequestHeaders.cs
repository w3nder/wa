// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpRequestHeaders
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the collection of Request Headers as defined in RFC 2616.</summary>
  public sealed class HttpRequestHeaders : HttpHeaders
  {
    private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> invalidHeaders;
    private HttpGeneralHeaders generalHeaders;
    private HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept;
    private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> expect;
    private bool expectContinueSet;
    private HttpHeaderValueCollection<EntityTagHeaderValue> ifMatch;
    private HttpHeaderValueCollection<EntityTagHeaderValue> ifNoneMatch;
    private HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> te;
    private HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent;
    private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptCharset;
    private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptEncoding;
    private HttpHeaderValueCollection<StringWithQualityHeaderValue> acceptLanguage;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept
    {
      get
      {
        if (this.accept == null)
          this.accept = new HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>(nameof (Accept), (HttpHeaders) this);
        return this.accept;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset
    {
      get
      {
        if (this.acceptCharset == null)
          this.acceptCharset = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Charset", (HttpHeaders) this);
        return this.acceptCharset;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding
    {
      get
      {
        if (this.acceptEncoding == null)
          this.acceptEncoding = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Encoding", (HttpHeaders) this);
        return this.acceptEncoding;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage
    {
      get
      {
        if (this.acceptLanguage == null)
          this.acceptLanguage = new HttpHeaderValueCollection<StringWithQualityHeaderValue>("Accept-Language", (HttpHeaders) this);
        return this.acceptLanguage;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />.</returns>
    public AuthenticationHeaderValue Authorization
    {
      get => (AuthenticationHeaderValue) this.GetParsedValues(nameof (Authorization));
      set => this.SetOrRemoveParsedValue(nameof (Authorization), (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect => this.ExpectCore;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool? ExpectContinue
    {
      get
      {
        if (this.ExpectCore.IsSpecialValueSet)
          return new bool?(true);
        return this.expectContinueSet ? new bool?(false) : new bool?();
      }
      set
      {
        bool? nullable = value;
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        {
          this.expectContinueSet = true;
          this.ExpectCore.SetSpecialValue();
        }
        else
        {
          this.expectContinueSet = value.HasValue;
          this.ExpectCore.RemoveSpecialValue();
        }
      }
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string From
    {
      get => (string) this.GetParsedValues(nameof (From));
      set
      {
        if (value == string.Empty)
          value = (string) null;
        if (value != null && !HeaderUtilities.IsValidEmailAddress(value))
          throw new FormatException(SR.net_http_headers_invalid_from_header);
        this.SetOrRemoveParsedValue(nameof (From), (object) value);
      }
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Host
    {
      get => (string) this.GetParsedValues(nameof (Host));
      set
      {
        if (value == string.Empty)
          value = (string) null;
        string host = (string) null;
        if (value != null && HttpRuleParser.GetHostLength(value, 0, false, out host) != value.Length)
          throw new FormatException(SR.net_http_headers_invalid_host_header);
        this.SetOrRemoveParsedValue(nameof (Host), (object) value);
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch
    {
      get
      {
        if (this.ifMatch == null)
          this.ifMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-Match", (HttpHeaders) this);
        return this.ifMatch;
      }
    }

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? IfModifiedSince
    {
      get => HeaderUtilities.GetDateTimeOffsetValue("If-Modified-Since", (HttpHeaders) this);
      set => this.SetOrRemoveParsedValue("If-Modified-Since", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch
    {
      get
      {
        if (this.ifNoneMatch == null)
          this.ifNoneMatch = new HttpHeaderValueCollection<EntityTagHeaderValue>("If-None-Match", (HttpHeaders) this);
        return this.ifNoneMatch;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" />.</returns>
    public RangeConditionHeaderValue IfRange
    {
      get => (RangeConditionHeaderValue) this.GetParsedValues("If-Range");
      set => this.SetOrRemoveParsedValue("If-Range", (object) value);
    }

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? IfUnmodifiedSince
    {
      get => HeaderUtilities.GetDateTimeOffsetValue("If-Unmodified-Since", (HttpHeaders) this);
      set => this.SetOrRemoveParsedValue("If-Unmodified-Since", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public int? MaxForwards
    {
      get
      {
        object parsedValues = this.GetParsedValues("Max-Forwards");
        return parsedValues != null ? new int?((int) parsedValues) : new int?();
      }
      set => this.SetOrRemoveParsedValue("Max-Forwards", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />.</returns>
    public AuthenticationHeaderValue ProxyAuthorization
    {
      get => (AuthenticationHeaderValue) this.GetParsedValues("Proxy-Authorization");
      set => this.SetOrRemoveParsedValue("Proxy-Authorization", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RangeHeaderValue" />.</returns>
    public RangeHeaderValue Range
    {
      get => (RangeHeaderValue) this.GetParsedValues(nameof (Range));
      set => this.SetOrRemoveParsedValue(nameof (Range), (object) value);
    }

    /// <returns>Returns <see cref="T:System.Uri" />.</returns>
    public Uri Referrer
    {
      get => (Uri) this.GetParsedValues("Referer");
      set => this.SetOrRemoveParsedValue("Referer", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE
    {
      get
      {
        if (this.te == null)
          this.te = new HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue>(nameof (TE), (HttpHeaders) this);
        return this.te;
      }
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.HttpHeaderValueCollection`1" />.</returns>
    public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent
    {
      get
      {
        if (this.userAgent == null)
          this.userAgent = new HttpHeaderValueCollection<ProductInfoHeaderValue>("User-Agent", (HttpHeaders) this);
        return this.userAgent;
      }
    }

    private HttpHeaderValueCollection<NameValueWithParametersHeaderValue> ExpectCore
    {
      get
      {
        if (this.expect == null)
          this.expect = new HttpHeaderValueCollection<NameValueWithParametersHeaderValue>("Expect", (HttpHeaders) this, HeaderUtilities.ExpectContinue);
        return this.expect;
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

    internal HttpRequestHeaders()
    {
      this.generalHeaders = new HttpGeneralHeaders((HttpHeaders) this);
      this.SetConfiguration(HttpRequestHeaders.parserStore, HttpRequestHeaders.invalidHeaders);
    }

    static HttpRequestHeaders()
    {
      HttpRequestHeaders.parserStore.Add(nameof (Accept), (HttpHeaderParser) MediaTypeHeaderParser.MultipleValuesParser);
      HttpRequestHeaders.parserStore.Add("Accept-Charset", GenericHeaderParser.MultipleValueStringWithQualityParser);
      HttpRequestHeaders.parserStore.Add("Accept-Encoding", GenericHeaderParser.MultipleValueStringWithQualityParser);
      HttpRequestHeaders.parserStore.Add("Accept-Language", GenericHeaderParser.MultipleValueStringWithQualityParser);
      HttpRequestHeaders.parserStore.Add(nameof (Authorization), GenericHeaderParser.SingleValueAuthenticationParser);
      HttpRequestHeaders.parserStore.Add(nameof (Expect), GenericHeaderParser.MultipleValueNameValueWithParametersParser);
      HttpRequestHeaders.parserStore.Add(nameof (From), GenericHeaderParser.MailAddressParser);
      HttpRequestHeaders.parserStore.Add(nameof (Host), GenericHeaderParser.HostParser);
      HttpRequestHeaders.parserStore.Add("If-Match", GenericHeaderParser.MultipleValueEntityTagParser);
      HttpRequestHeaders.parserStore.Add("If-Modified-Since", (HttpHeaderParser) DateHeaderParser.Parser);
      HttpRequestHeaders.parserStore.Add("If-None-Match", GenericHeaderParser.MultipleValueEntityTagParser);
      HttpRequestHeaders.parserStore.Add("If-Range", GenericHeaderParser.RangeConditionParser);
      HttpRequestHeaders.parserStore.Add("If-Unmodified-Since", (HttpHeaderParser) DateHeaderParser.Parser);
      HttpRequestHeaders.parserStore.Add("Max-Forwards", (HttpHeaderParser) Int32NumberHeaderParser.Parser);
      HttpRequestHeaders.parserStore.Add("Proxy-Authorization", GenericHeaderParser.SingleValueAuthenticationParser);
      HttpRequestHeaders.parserStore.Add(nameof (Range), GenericHeaderParser.RangeParser);
      HttpRequestHeaders.parserStore.Add("Referer", (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser);
      HttpRequestHeaders.parserStore.Add(nameof (TE), (HttpHeaderParser) TransferCodingHeaderParser.MultipleValueWithQualityParser);
      HttpRequestHeaders.parserStore.Add("User-Agent", (HttpHeaderParser) ProductInfoHeaderParser.MultipleValueParser);
      HttpGeneralHeaders.AddParsers(HttpRequestHeaders.parserStore);
      HttpRequestHeaders.invalidHeaders = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HttpContentHeaders.AddKnownHeaders(HttpRequestHeaders.invalidHeaders);
    }

    internal static void AddKnownHeaders(HashSet<string> headerSet)
    {
      Contract.Requires(headerSet != null);
      headerSet.Add("Accept");
      headerSet.Add("Accept-Charset");
      headerSet.Add("Accept-Encoding");
      headerSet.Add("Accept-Language");
      headerSet.Add("Authorization");
      headerSet.Add("Expect");
      headerSet.Add("From");
      headerSet.Add("Host");
      headerSet.Add("If-Match");
      headerSet.Add("If-Modified-Since");
      headerSet.Add("If-None-Match");
      headerSet.Add("If-Range");
      headerSet.Add("If-Unmodified-Since");
      headerSet.Add("Max-Forwards");
      headerSet.Add("Proxy-Authorization");
      headerSet.Add("Range");
      headerSet.Add("Referer");
      headerSet.Add("TE");
      headerSet.Add("User-Agent");
    }

    internal override void AddHeaders(HttpHeaders sourceHeaders)
    {
      base.AddHeaders(sourceHeaders);
      HttpRequestHeaders httpRequestHeaders = sourceHeaders as HttpRequestHeaders;
      Contract.Assert(httpRequestHeaders != null);
      this.generalHeaders.AddSpecialsFrom(httpRequestHeaders.generalHeaders);
      if (this.ExpectContinue.HasValue)
        return;
      this.ExpectContinue = httpRequestHeaders.ExpectContinue;
    }
  }
}
