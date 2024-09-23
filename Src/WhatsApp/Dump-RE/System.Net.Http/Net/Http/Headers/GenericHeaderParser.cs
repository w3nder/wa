// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.GenericHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections;
using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  internal sealed class GenericHeaderParser : BaseHeaderParser
  {
    internal static readonly HttpHeaderParser HostParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseHost), (IEqualityComparer) StringComparer.OrdinalIgnoreCase);
    internal static readonly HttpHeaderParser TokenListParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseTokenList), (IEqualityComparer) StringComparer.OrdinalIgnoreCase);
    internal static readonly HttpHeaderParser SingleValueNameValueWithParametersParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(NameValueWithParametersHeaderValue.GetNameValueWithParametersLength));
    internal static readonly HttpHeaderParser MultipleValueNameValueWithParametersParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(NameValueWithParametersHeaderValue.GetNameValueWithParametersLength));
    internal static readonly HttpHeaderParser SingleValueNameValueParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseNameValue));
    internal static readonly HttpHeaderParser MultipleValueNameValueParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseNameValue));
    internal static readonly HttpHeaderParser MailAddressParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseMailAddress));
    internal static readonly HttpHeaderParser SingleValueProductParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseProduct));
    internal static readonly HttpHeaderParser MultipleValueProductParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseProduct));
    internal static readonly HttpHeaderParser RangeConditionParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(RangeConditionHeaderValue.GetRangeConditionLength));
    internal static readonly HttpHeaderParser SingleValueAuthenticationParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(AuthenticationHeaderValue.GetAuthenticationLength));
    internal static readonly HttpHeaderParser MultipleValueAuthenticationParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(AuthenticationHeaderValue.GetAuthenticationLength));
    internal static readonly HttpHeaderParser RangeParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(RangeHeaderValue.GetRangeLength));
    internal static readonly HttpHeaderParser RetryConditionParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(RetryConditionHeaderValue.GetRetryConditionLength));
    internal static readonly HttpHeaderParser ContentRangeParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(ContentRangeHeaderValue.GetContentRangeLength));
    internal static readonly HttpHeaderParser ContentDispositionParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(ContentDispositionHeaderValue.GetDispositionTypeLength));
    internal static readonly HttpHeaderParser SingleValueStringWithQualityParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(StringWithQualityHeaderValue.GetStringWithQualityLength));
    internal static readonly HttpHeaderParser MultipleValueStringWithQualityParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(StringWithQualityHeaderValue.GetStringWithQualityLength));
    internal static readonly HttpHeaderParser SingleValueEntityTagParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseSingleEntityTag));
    internal static readonly HttpHeaderParser MultipleValueEntityTagParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(GenericHeaderParser.ParseMultipleEntityTags));
    internal static readonly HttpHeaderParser SingleValueViaParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(ViaHeaderValue.GetViaLength));
    internal static readonly HttpHeaderParser MultipleValueViaParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(ViaHeaderValue.GetViaLength));
    internal static readonly HttpHeaderParser SingleValueWarningParser = (HttpHeaderParser) new GenericHeaderParser(false, new GenericHeaderParser.GetParsedValueLengthDelegate(WarningHeaderValue.GetWarningLength));
    internal static readonly HttpHeaderParser MultipleValueWarningParser = (HttpHeaderParser) new GenericHeaderParser(true, new GenericHeaderParser.GetParsedValueLengthDelegate(WarningHeaderValue.GetWarningLength));
    private GenericHeaderParser.GetParsedValueLengthDelegate getParsedValueLength;
    private IEqualityComparer comparer;

    public override IEqualityComparer Comparer => this.comparer;

    private GenericHeaderParser(
      bool supportsMultipleValues,
      GenericHeaderParser.GetParsedValueLengthDelegate getParsedValueLength)
      : this(supportsMultipleValues, getParsedValueLength, (IEqualityComparer) null)
    {
    }

    private GenericHeaderParser(
      bool supportsMultipleValues,
      GenericHeaderParser.GetParsedValueLengthDelegate getParsedValueLength,
      IEqualityComparer comparer)
      : base(supportsMultipleValues)
    {
      Contract.Assert(getParsedValueLength != null);
      this.getParsedValueLength = getParsedValueLength;
      this.comparer = comparer;
    }

    protected override int GetParsedValueLength(
      string value,
      int startIndex,
      object storeValue,
      out object parsedValue)
    {
      return this.getParsedValueLength(value, startIndex, out parsedValue);
    }

    private static int ParseNameValue(string value, int startIndex, out object parsedValue)
    {
      NameValueHeaderValue parsedValue1 = (NameValueHeaderValue) null;
      int nameValueLength = NameValueHeaderValue.GetNameValueLength(value, startIndex, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return nameValueLength;
    }

    private static int ParseProduct(string value, int startIndex, out object parsedValue)
    {
      ProductHeaderValue parsedValue1 = (ProductHeaderValue) null;
      int productLength = ProductHeaderValue.GetProductLength(value, startIndex, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return productLength;
    }

    private static int ParseSingleEntityTag(string value, int startIndex, out object parsedValue)
    {
      EntityTagHeaderValue parsedValue1 = (EntityTagHeaderValue) null;
      parsedValue = (object) null;
      int entityTagLength = EntityTagHeaderValue.GetEntityTagLength(value, startIndex, out parsedValue1);
      if (parsedValue1 == EntityTagHeaderValue.Any)
        return 0;
      parsedValue = (object) parsedValue1;
      return entityTagLength;
    }

    private static int ParseMultipleEntityTags(
      string value,
      int startIndex,
      out object parsedValue)
    {
      EntityTagHeaderValue parsedValue1 = (EntityTagHeaderValue) null;
      int entityTagLength = EntityTagHeaderValue.GetEntityTagLength(value, startIndex, out parsedValue1);
      parsedValue = (object) parsedValue1;
      return entityTagLength;
    }

    private static int ParseMailAddress(string value, int startIndex, out object parsedValue)
    {
      parsedValue = (object) null;
      if (HttpRuleParser.ContainsInvalidNewLine(value, startIndex))
        return 0;
      string str = value.Substring(startIndex);
      if (!HeaderUtilities.IsValidEmailAddress(str))
        return 0;
      parsedValue = (object) str;
      return str.Length;
    }

    private static int ParseHost(string value, int startIndex, out object parsedValue)
    {
      string host = (string) null;
      int hostLength = HttpRuleParser.GetHostLength(value, startIndex, false, out host);
      parsedValue = (object) host;
      return hostLength;
    }

    private static int ParseTokenList(string value, int startIndex, out object parsedValue)
    {
      int tokenLength = HttpRuleParser.GetTokenLength(value, startIndex);
      parsedValue = (object) value.Substring(startIndex, tokenLength);
      return tokenLength;
    }

    private delegate int GetParsedValueLengthDelegate(
      string value,
      int startIndex,
      out object parsedValue);
  }
}
