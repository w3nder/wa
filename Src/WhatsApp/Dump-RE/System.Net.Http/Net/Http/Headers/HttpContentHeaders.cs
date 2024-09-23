// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpContentHeaders
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the collection of Content Headers as defined in RFC 2616.</summary>
  public sealed class HttpContentHeaders : HttpHeaders
  {
    private static readonly Dictionary<string, HttpHeaderParser> parserStore = new Dictionary<string, HttpHeaderParser>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> invalidHeaders;
    private Func<long?> calculateLengthFunc;
    private bool contentLengthSet;
    private HttpHeaderValueCollection<string> allow;
    private HttpHeaderValueCollection<string> contentEncoding;
    private HttpHeaderValueCollection<string> contentLanguage;

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<string> Allow
    {
      get
      {
        if (this.allow == null)
          this.allow = new HttpHeaderValueCollection<string>(nameof (Allow), (HttpHeaders) this, HeaderUtilities.TokenValidator);
        return (ICollection<string>) this.allow;
      }
    }

    public ContentDispositionHeaderValue ContentDisposition
    {
      get => (ContentDispositionHeaderValue) this.GetParsedValues("Content-Disposition");
      set => this.SetOrRemoveParsedValue("Content-Disposition", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<string> ContentEncoding
    {
      get
      {
        if (this.contentEncoding == null)
          this.contentEncoding = new HttpHeaderValueCollection<string>("Content-Encoding", (HttpHeaders) this, HeaderUtilities.TokenValidator);
        return (ICollection<string>) this.contentEncoding;
      }
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<string> ContentLanguage
    {
      get
      {
        if (this.contentLanguage == null)
          this.contentLanguage = new HttpHeaderValueCollection<string>("Content-Language", (HttpHeaders) this, HeaderUtilities.TokenValidator);
        return (ICollection<string>) this.contentLanguage;
      }
    }

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? ContentLength
    {
      get
      {
        object parsedValues = this.GetParsedValues("Content-Length");
        if (!this.contentLengthSet && parsedValues == null)
        {
          long? contentLength = this.calculateLengthFunc();
          if (contentLength.HasValue)
            this.SetParsedValue("Content-Length", (object) contentLength.Value);
          return contentLength;
        }
        return parsedValues == null ? new long?() : new long?((long) parsedValues);
      }
      set
      {
        this.SetOrRemoveParsedValue("Content-Length", (object) value);
        this.contentLengthSet = true;
      }
    }

    /// <returns>Returns <see cref="T:System.Uri" />.</returns>
    public Uri ContentLocation
    {
      get => (Uri) this.GetParsedValues("Content-Location");
      set => this.SetOrRemoveParsedValue("Content-Location", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Byte" />.</returns>
    public byte[] ContentMD5
    {
      get => (byte[]) this.GetParsedValues("Content-MD5");
      set => this.SetOrRemoveParsedValue("Content-MD5", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" />.</returns>
    public ContentRangeHeaderValue ContentRange
    {
      get => (ContentRangeHeaderValue) this.GetParsedValues("Content-Range");
      set => this.SetOrRemoveParsedValue("Content-Range", (object) value);
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.MediaTypeHeaderValue" />.</returns>
    public MediaTypeHeaderValue ContentType
    {
      get => (MediaTypeHeaderValue) this.GetParsedValues("Content-Type");
      set => this.SetOrRemoveParsedValue("Content-Type", (object) value);
    }

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? Expires
    {
      get => HeaderUtilities.GetDateTimeOffsetValue(nameof (Expires), (HttpHeaders) this);
      set => this.SetOrRemoveParsedValue(nameof (Expires), (object) value);
    }

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? LastModified
    {
      get => HeaderUtilities.GetDateTimeOffsetValue("Last-Modified", (HttpHeaders) this);
      set => this.SetOrRemoveParsedValue("Last-Modified", (object) value);
    }

    internal HttpContentHeaders(Func<long?> calculateLengthFunc)
    {
      this.calculateLengthFunc = calculateLengthFunc;
      this.SetConfiguration(HttpContentHeaders.parserStore, HttpContentHeaders.invalidHeaders);
    }

    static HttpContentHeaders()
    {
      HttpContentHeaders.parserStore.Add(nameof (Allow), GenericHeaderParser.TokenListParser);
      HttpContentHeaders.parserStore.Add("Content-Disposition", GenericHeaderParser.ContentDispositionParser);
      HttpContentHeaders.parserStore.Add("Content-Encoding", GenericHeaderParser.TokenListParser);
      HttpContentHeaders.parserStore.Add("Content-Language", GenericHeaderParser.TokenListParser);
      HttpContentHeaders.parserStore.Add("Content-Length", (HttpHeaderParser) Int64NumberHeaderParser.Parser);
      HttpContentHeaders.parserStore.Add("Content-Location", (HttpHeaderParser) UriHeaderParser.RelativeOrAbsoluteUriParser);
      HttpContentHeaders.parserStore.Add("Content-MD5", (HttpHeaderParser) ByteArrayHeaderParser.Parser);
      HttpContentHeaders.parserStore.Add("Content-Range", GenericHeaderParser.ContentRangeParser);
      HttpContentHeaders.parserStore.Add("Content-Type", (HttpHeaderParser) MediaTypeHeaderParser.SingleValueParser);
      HttpContentHeaders.parserStore.Add(nameof (Expires), (HttpHeaderParser) DateHeaderParser.Parser);
      HttpContentHeaders.parserStore.Add("Last-Modified", (HttpHeaderParser) DateHeaderParser.Parser);
      HttpContentHeaders.invalidHeaders = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HttpRequestHeaders.AddKnownHeaders(HttpContentHeaders.invalidHeaders);
      HttpResponseHeaders.AddKnownHeaders(HttpContentHeaders.invalidHeaders);
      HttpGeneralHeaders.AddKnownHeaders(HttpContentHeaders.invalidHeaders);
    }

    internal static void AddKnownHeaders(HashSet<string> headerSet)
    {
      Contract.Requires(headerSet != null);
      headerSet.Add("Allow");
      headerSet.Add("Content-Disposition");
      headerSet.Add("Content-Encoding");
      headerSet.Add("Content-Language");
      headerSet.Add("Content-Length");
      headerSet.Add("Content-Location");
      headerSet.Add("Content-MD5");
      headerSet.Add("Content-Range");
      headerSet.Add("Content-Type");
      headerSet.Add("Expires");
      headerSet.Add("Last-Modified");
    }
  }
}
