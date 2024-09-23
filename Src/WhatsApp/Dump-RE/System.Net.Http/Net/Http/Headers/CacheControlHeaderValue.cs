// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.CacheControlHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the value of the Cache-Control header.</summary>
  public class CacheControlHeaderValue : ICloneable
  {
    private const string maxAgeString = "max-age";
    private const string maxStaleString = "max-stale";
    private const string minFreshString = "min-fresh";
    private const string mustRevalidateString = "must-revalidate";
    private const string noCacheString = "no-cache";
    private const string noStoreString = "no-store";
    private const string noTransformString = "no-transform";
    private const string onlyIfCachedString = "only-if-cached";
    private const string privateString = "private";
    private const string proxyRevalidateString = "proxy-revalidate";
    private const string publicString = "public";
    private const string sharedMaxAgeString = "s-maxage";
    private static readonly HttpHeaderParser nameValueListParser = GenericHeaderParser.MultipleValueNameValueParser;
    private static readonly Action<string> checkIsValidToken = new Action<string>(CacheControlHeaderValue.CheckIsValidToken);
    private bool noCache;
    private ICollection<string> noCacheHeaders;
    private bool noStore;
    private TimeSpan? maxAge;
    private TimeSpan? sharedMaxAge;
    private bool maxStale;
    private TimeSpan? maxStaleLimit;
    private TimeSpan? minFresh;
    private bool noTransform;
    private bool onlyIfCached;
    private bool publicField;
    private bool privateField;
    private ICollection<string> privateHeaders;
    private bool mustRevalidate;
    private bool proxyRevalidate;
    private ICollection<NameValueHeaderValue> extensions;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool NoCache
    {
      get => this.noCache;
      set => this.noCache = value;
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<string> NoCacheHeaders
    {
      get
      {
        if (this.noCacheHeaders == null)
          this.noCacheHeaders = (ICollection<string>) new ObjectCollection<string>(CacheControlHeaderValue.checkIsValidToken);
        return this.noCacheHeaders;
      }
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool NoStore
    {
      get => this.noStore;
      set => this.noStore = value;
    }

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? MaxAge
    {
      get => this.maxAge;
      set => this.maxAge = value;
    }

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? SharedMaxAge
    {
      get => this.sharedMaxAge;
      set => this.sharedMaxAge = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool MaxStale
    {
      get => this.maxStale;
      set => this.maxStale = value;
    }

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? MaxStaleLimit
    {
      get => this.maxStaleLimit;
      set => this.maxStaleLimit = value;
    }

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? MinFresh
    {
      get => this.minFresh;
      set => this.minFresh = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool NoTransform
    {
      get => this.noTransform;
      set => this.noTransform = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool OnlyIfCached
    {
      get => this.onlyIfCached;
      set => this.onlyIfCached = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Public
    {
      get => this.publicField;
      set => this.publicField = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Private
    {
      get => this.privateField;
      set => this.privateField = value;
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<string> PrivateHeaders
    {
      get
      {
        if (this.privateHeaders == null)
          this.privateHeaders = (ICollection<string>) new ObjectCollection<string>(CacheControlHeaderValue.checkIsValidToken);
        return this.privateHeaders;
      }
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool MustRevalidate
    {
      get => this.mustRevalidate;
      set => this.mustRevalidate = value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool ProxyRevalidate
    {
      get => this.proxyRevalidate;
      set => this.proxyRevalidate = value;
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<NameValueHeaderValue> Extensions
    {
      get
      {
        if (this.extensions == null)
          this.extensions = (ICollection<NameValueHeaderValue>) new ObjectCollection<NameValueHeaderValue>();
        return this.extensions;
      }
    }

    public CacheControlHeaderValue()
    {
    }

    private CacheControlHeaderValue(CacheControlHeaderValue source)
    {
      Contract.Requires(source != null);
      this.noCache = source.noCache;
      this.noStore = source.noStore;
      this.maxAge = source.maxAge;
      this.sharedMaxAge = source.sharedMaxAge;
      this.maxStale = source.maxStale;
      this.maxStaleLimit = source.maxStaleLimit;
      this.minFresh = source.minFresh;
      this.noTransform = source.noTransform;
      this.onlyIfCached = source.onlyIfCached;
      this.publicField = source.publicField;
      this.privateField = source.privateField;
      this.mustRevalidate = source.mustRevalidate;
      this.proxyRevalidate = source.proxyRevalidate;
      if (source.noCacheHeaders != null)
      {
        foreach (string noCacheHeader in (IEnumerable<string>) source.noCacheHeaders)
          this.NoCacheHeaders.Add(noCacheHeader);
      }
      if (source.privateHeaders != null)
      {
        foreach (string privateHeader in (IEnumerable<string>) source.privateHeaders)
          this.PrivateHeaders.Add(privateHeader);
      }
      if (source.extensions == null)
        return;
      foreach (ICloneable extension in (IEnumerable<NameValueHeaderValue>) source.extensions)
        this.Extensions.Add((NameValueHeaderValue) extension.Clone());
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.noStore, "no-store");
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.noTransform, "no-transform");
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.onlyIfCached, "only-if-cached");
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.publicField, "public");
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.mustRevalidate, "must-revalidate");
      CacheControlHeaderValue.AppendValueIfRequired(stringBuilder, this.proxyRevalidate, "proxy-revalidate");
      if (this.noCache)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "no-cache");
        if (this.noCacheHeaders != null && this.noCacheHeaders.Count > 0)
        {
          stringBuilder.Append("=\"");
          CacheControlHeaderValue.AppendValues(stringBuilder, (IEnumerable<string>) this.noCacheHeaders);
          stringBuilder.Append('"');
        }
      }
      if (this.maxAge.HasValue)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "max-age");
        stringBuilder.Append('=');
        stringBuilder.Append(((int) this.maxAge.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      }
      if (this.sharedMaxAge.HasValue)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "s-maxage");
        stringBuilder.Append('=');
        stringBuilder.Append(((int) this.sharedMaxAge.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      }
      if (this.maxStale)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "max-stale");
        if (this.maxStaleLimit.HasValue)
        {
          stringBuilder.Append('=');
          stringBuilder.Append(((int) this.maxStaleLimit.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        }
      }
      if (this.minFresh.HasValue)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "min-fresh");
        stringBuilder.Append('=');
        stringBuilder.Append(((int) this.minFresh.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      }
      if (this.privateField)
      {
        CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(stringBuilder, "private");
        if (this.privateHeaders != null && this.privateHeaders.Count > 0)
        {
          stringBuilder.Append("=\"");
          CacheControlHeaderValue.AppendValues(stringBuilder, (IEnumerable<string>) this.privateHeaders);
          stringBuilder.Append('"');
        }
      }
      NameValueHeaderValue.ToString(this.extensions, ',', false, stringBuilder);
      return stringBuilder.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is CacheControlHeaderValue controlHeaderValue) || this.noCache != controlHeaderValue.noCache || this.noStore != controlHeaderValue.noStore)
        return false;
      TimeSpan? maxAge1 = this.maxAge;
      TimeSpan? maxAge2 = controlHeaderValue.maxAge;
      if ((maxAge1.HasValue != maxAge2.HasValue ? 1 : (!maxAge1.HasValue ? 0 : (maxAge1.GetValueOrDefault() != maxAge2.GetValueOrDefault() ? 1 : 0))) == 0)
      {
        TimeSpan? sharedMaxAge1 = this.sharedMaxAge;
        TimeSpan? sharedMaxAge2 = controlHeaderValue.sharedMaxAge;
        if ((sharedMaxAge1.HasValue != sharedMaxAge2.HasValue ? 1 : (!sharedMaxAge1.HasValue ? 0 : (sharedMaxAge1.GetValueOrDefault() != sharedMaxAge2.GetValueOrDefault() ? 1 : 0))) == 0 && this.maxStale == controlHeaderValue.maxStale)
        {
          TimeSpan? maxStaleLimit1 = this.maxStaleLimit;
          TimeSpan? maxStaleLimit2 = controlHeaderValue.maxStaleLimit;
          if ((maxStaleLimit1.HasValue != maxStaleLimit2.HasValue ? 1 : (!maxStaleLimit1.HasValue ? 0 : (maxStaleLimit1.GetValueOrDefault() != maxStaleLimit2.GetValueOrDefault() ? 1 : 0))) == 0)
          {
            TimeSpan? minFresh1 = this.minFresh;
            TimeSpan? minFresh2 = controlHeaderValue.minFresh;
            if ((minFresh1.HasValue != minFresh2.HasValue ? 1 : (!minFresh1.HasValue ? 0 : (minFresh1.GetValueOrDefault() != minFresh2.GetValueOrDefault() ? 1 : 0))) == 0 && this.noTransform == controlHeaderValue.noTransform && this.onlyIfCached == controlHeaderValue.onlyIfCached && this.publicField == controlHeaderValue.publicField && this.privateField == controlHeaderValue.privateField && this.mustRevalidate == controlHeaderValue.mustRevalidate && this.proxyRevalidate == controlHeaderValue.proxyRevalidate && HeaderUtilities.AreEqualCollections<string>(this.noCacheHeaders, controlHeaderValue.noCacheHeaders, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<string>(this.privateHeaders, controlHeaderValue.privateHeaders, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && HeaderUtilities.AreEqualCollections<NameValueHeaderValue>(this.extensions, controlHeaderValue.extensions))
              return true;
          }
        }
      }
      return false;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.noCache.GetHashCode() ^ this.noStore.GetHashCode() << 1 ^ this.maxStale.GetHashCode() << 2 ^ this.noTransform.GetHashCode() << 3 ^ this.onlyIfCached.GetHashCode() << 4 ^ this.publicField.GetHashCode() << 5 ^ this.privateField.GetHashCode() << 6 ^ this.mustRevalidate.GetHashCode() << 7 ^ this.proxyRevalidate.GetHashCode() << 8 ^ (this.maxAge.HasValue ? this.maxAge.Value.GetHashCode() ^ 1 : 0) ^ (this.sharedMaxAge.HasValue ? this.sharedMaxAge.Value.GetHashCode() ^ 2 : 0) ^ (this.maxStaleLimit.HasValue ? this.maxStaleLimit.Value.GetHashCode() ^ 4 : 0) ^ (this.minFresh.HasValue ? this.minFresh.Value.GetHashCode() ^ 8 : 0);
      if (this.noCacheHeaders != null && this.noCacheHeaders.Count > 0)
      {
        foreach (string noCacheHeader in (IEnumerable<string>) this.noCacheHeaders)
          hashCode ^= noCacheHeader.ToLowerInvariant().GetHashCode();
      }
      if (this.privateHeaders != null && this.privateHeaders.Count > 0)
      {
        foreach (string privateHeader in (IEnumerable<string>) this.privateHeaders)
          hashCode ^= privateHeader.ToLowerInvariant().GetHashCode();
      }
      if (this.extensions != null && this.extensions.Count > 0)
      {
        foreach (NameValueHeaderValue extension in (IEnumerable<NameValueHeaderValue>) this.extensions)
          hashCode ^= extension.GetHashCode();
      }
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.CacheControlHeaderValue" />.</returns>
    public static CacheControlHeaderValue Parse(string input)
    {
      int index = 0;
      return (CacheControlHeaderValue) CacheControlHeaderParser.Parser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out CacheControlHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (CacheControlHeaderValue) null;
      object parsedValue1;
      if (!CacheControlHeaderParser.Parser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (CacheControlHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetCacheControlLength(
      string input,
      int startIndex,
      CacheControlHeaderValue storeValue,
      out CacheControlHeaderValue parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (CacheControlHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int index = startIndex;
      object parsedValue1 = (object) null;
      List<NameValueHeaderValue> nameValueList = new List<NameValueHeaderValue>();
      while (index < input.Length)
      {
        if (!CacheControlHeaderValue.nameValueListParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
          return 0;
        nameValueList.Add(parsedValue1 as NameValueHeaderValue);
      }
      CacheControlHeaderValue cc = storeValue ?? new CacheControlHeaderValue();
      if (!CacheControlHeaderValue.TrySetCacheControlValues(cc, nameValueList))
        return 0;
      if (storeValue == null)
        parsedValue = cc;
      return input.Length - startIndex;
    }

    private static bool TrySetCacheControlValues(
      CacheControlHeaderValue cc,
      List<NameValueHeaderValue> nameValueList)
    {
      foreach (NameValueHeaderValue nameValue in nameValueList)
      {
        bool flag = true;
        switch (nameValue.Name.ToLowerInvariant())
        {
          case "no-cache":
            flag = CacheControlHeaderValue.TrySetOptionalTokenList(nameValue, ref cc.noCache, ref cc.noCacheHeaders);
            break;
          case "no-store":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.noStore);
            break;
          case "max-age":
            flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc.maxAge);
            break;
          case "max-stale":
            flag = nameValue.Value == null || CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc.maxStaleLimit);
            if (flag)
            {
              cc.maxStale = true;
              break;
            }
            break;
          case "min-fresh":
            flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc.minFresh);
            break;
          case "no-transform":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.noTransform);
            break;
          case "only-if-cached":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.onlyIfCached);
            break;
          case "public":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.publicField);
            break;
          case "private":
            flag = CacheControlHeaderValue.TrySetOptionalTokenList(nameValue, ref cc.privateField, ref cc.privateHeaders);
            break;
          case "must-revalidate":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.mustRevalidate);
            break;
          case "proxy-revalidate":
            flag = CacheControlHeaderValue.TrySetTokenOnlyValue(nameValue, ref cc.proxyRevalidate);
            break;
          case "s-maxage":
            flag = CacheControlHeaderValue.TrySetTimeSpan(nameValue, ref cc.sharedMaxAge);
            break;
          default:
            cc.Extensions.Add(nameValue);
            break;
        }
        if (!flag)
          return false;
      }
      return true;
    }

    private static bool TrySetTokenOnlyValue(NameValueHeaderValue nameValue, ref bool boolField)
    {
      if (nameValue.Value != null)
        return false;
      boolField = true;
      return true;
    }

    private static bool TrySetOptionalTokenList(
      NameValueHeaderValue nameValue,
      ref bool boolField,
      ref ICollection<string> destination)
    {
      Contract.Requires(nameValue != null);
      if (nameValue.Value == null)
      {
        boolField = true;
        return true;
      }
      string input = nameValue.Value;
      if (input.Length < 3 || input[0] != '"' || input[input.Length - 1] != '"')
        return false;
      int startIndex = 1;
      int num = input.Length - 1;
      bool separatorFound = false;
      int count = destination == null ? 0 : destination.Count;
      int orWhitespaceIndex;
      int tokenLength;
      for (; startIndex < num; startIndex = orWhitespaceIndex + tokenLength)
      {
        orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
        if (orWhitespaceIndex != num)
        {
          tokenLength = HttpRuleParser.GetTokenLength(input, orWhitespaceIndex);
          if (tokenLength == 0)
            return false;
          if (destination == null)
            destination = (ICollection<string>) new ObjectCollection<string>(CacheControlHeaderValue.checkIsValidToken);
          destination.Add(input.Substring(orWhitespaceIndex, tokenLength));
        }
        else
          break;
      }
      if (destination == null || destination.Count <= count)
        return false;
      boolField = true;
      return true;
    }

    private static bool TrySetTimeSpan(NameValueHeaderValue nameValue, ref TimeSpan? timeSpan)
    {
      Contract.Requires(nameValue != null);
      int result;
      if (nameValue.Value == null || !HeaderUtilities.TryParseInt32(nameValue.Value, out result))
        return false;
      timeSpan = new TimeSpan?(new TimeSpan(0, 0, result));
      return true;
    }

    private static void AppendValueIfRequired(StringBuilder sb, bool appendValue, string value)
    {
      if (!appendValue)
        return;
      CacheControlHeaderValue.AppendValueWithSeparatorIfRequired(sb, value);
    }

    private static void AppendValueWithSeparatorIfRequired(StringBuilder sb, string value)
    {
      if (sb.Length > 0)
        sb.Append(", ");
      sb.Append(value);
    }

    private static void AppendValues(StringBuilder sb, IEnumerable<string> values)
    {
      bool flag = true;
      foreach (string str in values)
      {
        if (flag)
          flag = false;
        else
          sb.Append(", ");
        sb.Append(str);
      }
    }

    private static void CheckIsValidToken(string item)
    {
      HeaderUtilities.CheckValidToken(item, nameof (item));
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new CacheControlHeaderValue(this);
  }
}
