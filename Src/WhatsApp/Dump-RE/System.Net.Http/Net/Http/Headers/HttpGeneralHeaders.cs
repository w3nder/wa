// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpGeneralHeaders
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal sealed class HttpGeneralHeaders
  {
    private HttpHeaderValueCollection<string> connection;
    private HttpHeaderValueCollection<string> trailer;
    private HttpHeaderValueCollection<TransferCodingHeaderValue> transferEncoding;
    private HttpHeaderValueCollection<ProductHeaderValue> upgrade;
    private HttpHeaderValueCollection<ViaHeaderValue> via;
    private HttpHeaderValueCollection<WarningHeaderValue> warning;
    private HttpHeaderValueCollection<NameValueHeaderValue> pragma;
    private HttpHeaders parent;
    private bool transferEncodingChunkedSet;
    private bool connectionCloseSet;

    public CacheControlHeaderValue CacheControl
    {
      get => (CacheControlHeaderValue) this.parent.GetParsedValues("Cache-Control");
      set => this.parent.SetOrRemoveParsedValue("Cache-Control", (object) value);
    }

    public HttpHeaderValueCollection<string> Connection => this.ConnectionCore;

    public bool? ConnectionClose
    {
      get
      {
        if (this.ConnectionCore.IsSpecialValueSet)
          return new bool?(true);
        return this.connectionCloseSet ? new bool?(false) : new bool?();
      }
      set
      {
        bool? nullable = value;
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        {
          this.connectionCloseSet = true;
          this.ConnectionCore.SetSpecialValue();
        }
        else
        {
          this.connectionCloseSet = value.HasValue;
          this.ConnectionCore.RemoveSpecialValue();
        }
      }
    }

    public DateTimeOffset? Date
    {
      get => HeaderUtilities.GetDateTimeOffsetValue(nameof (Date), this.parent);
      set => this.parent.SetOrRemoveParsedValue(nameof (Date), (object) value);
    }

    public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
    {
      get
      {
        if (this.pragma == null)
          this.pragma = new HttpHeaderValueCollection<NameValueHeaderValue>(nameof (Pragma), this.parent);
        return this.pragma;
      }
    }

    public HttpHeaderValueCollection<string> Trailer
    {
      get
      {
        if (this.trailer == null)
          this.trailer = new HttpHeaderValueCollection<string>(nameof (Trailer), this.parent, HeaderUtilities.TokenValidator);
        return this.trailer;
      }
    }

    public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
    {
      get => this.TransferEncodingCore;
    }

    public bool? TransferEncodingChunked
    {
      get
      {
        if (this.TransferEncodingCore.IsSpecialValueSet)
          return new bool?(true);
        return this.transferEncodingChunkedSet ? new bool?(false) : new bool?();
      }
      set
      {
        bool? nullable = value;
        if ((!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) != 0)
        {
          this.transferEncodingChunkedSet = true;
          this.TransferEncodingCore.SetSpecialValue();
        }
        else
        {
          this.transferEncodingChunkedSet = value.HasValue;
          this.TransferEncodingCore.RemoveSpecialValue();
        }
      }
    }

    public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
    {
      get
      {
        if (this.upgrade == null)
          this.upgrade = new HttpHeaderValueCollection<ProductHeaderValue>(nameof (Upgrade), this.parent);
        return this.upgrade;
      }
    }

    public HttpHeaderValueCollection<ViaHeaderValue> Via
    {
      get
      {
        if (this.via == null)
          this.via = new HttpHeaderValueCollection<ViaHeaderValue>(nameof (Via), this.parent);
        return this.via;
      }
    }

    public HttpHeaderValueCollection<WarningHeaderValue> Warning
    {
      get
      {
        if (this.warning == null)
          this.warning = new HttpHeaderValueCollection<WarningHeaderValue>(nameof (Warning), this.parent);
        return this.warning;
      }
    }

    private HttpHeaderValueCollection<string> ConnectionCore
    {
      get
      {
        if (this.connection == null)
          this.connection = new HttpHeaderValueCollection<string>("Connection", this.parent, "close", HeaderUtilities.TokenValidator);
        return this.connection;
      }
    }

    private HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncodingCore
    {
      get
      {
        if (this.transferEncoding == null)
          this.transferEncoding = new HttpHeaderValueCollection<TransferCodingHeaderValue>("Transfer-Encoding", this.parent, HeaderUtilities.TransferEncodingChunked);
        return this.transferEncoding;
      }
    }

    internal HttpGeneralHeaders(HttpHeaders parent)
    {
      Contract.Requires(parent != null);
      this.parent = parent;
    }

    internal static void AddParsers(Dictionary<string, HttpHeaderParser> parserStore)
    {
      Contract.Requires(parserStore != null);
      parserStore.Add("Cache-Control", (HttpHeaderParser) CacheControlHeaderParser.Parser);
      parserStore.Add("Connection", GenericHeaderParser.TokenListParser);
      parserStore.Add("Date", (HttpHeaderParser) DateHeaderParser.Parser);
      parserStore.Add("Pragma", GenericHeaderParser.MultipleValueNameValueParser);
      parserStore.Add("Trailer", GenericHeaderParser.TokenListParser);
      parserStore.Add("Transfer-Encoding", (HttpHeaderParser) TransferCodingHeaderParser.MultipleValueParser);
      parserStore.Add("Upgrade", GenericHeaderParser.MultipleValueProductParser);
      parserStore.Add("Via", GenericHeaderParser.MultipleValueViaParser);
      parserStore.Add("Warning", GenericHeaderParser.MultipleValueWarningParser);
    }

    internal static void AddKnownHeaders(HashSet<string> headerSet)
    {
      Contract.Requires(headerSet != null);
      headerSet.Add("Cache-Control");
      headerSet.Add("Connection");
      headerSet.Add("Date");
      headerSet.Add("Pragma");
      headerSet.Add("Trailer");
      headerSet.Add("Transfer-Encoding");
      headerSet.Add("Upgrade");
      headerSet.Add("Via");
      headerSet.Add("Warning");
    }

    internal void AddSpecialsFrom(HttpGeneralHeaders sourceHeaders)
    {
      if (!this.TransferEncodingChunked.HasValue)
        this.TransferEncodingChunked = sourceHeaders.TransferEncodingChunked;
      if (this.ConnectionClose.HasValue)
        return;
      this.ConnectionClose = sourceHeaders.ConnectionClose;
    }
  }
}
