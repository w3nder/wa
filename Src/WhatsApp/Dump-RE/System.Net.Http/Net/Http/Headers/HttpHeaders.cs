// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaders
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>A collection of headers and their values as defined in RFC 2616.</summary>
  public abstract class HttpHeaders : 
    IEnumerable<KeyValuePair<string, IEnumerable<string>>>,
    IEnumerable
  {
    private Dictionary<string, HttpHeaders.HeaderStoreItemInfo> headerStore;
    private Dictionary<string, HttpHeaderParser> parserStore;
    private HashSet<string> invalidHeaders;

    public void Add(string name, string value)
    {
      this.CheckHeaderName(name);
      HttpHeaders.HeaderStoreItemInfo info;
      bool addToStore;
      this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
      this.ParseAndAddValue(name, info, value);
      if (!addToStore || info.ParsedValue == null)
        return;
      this.AddHeaderToStore(name, info);
    }

    public void Add(string name, IEnumerable<string> values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      this.CheckHeaderName(name);
      HttpHeaders.HeaderStoreItemInfo info;
      bool addToStore;
      this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
      try
      {
        foreach (string str in values)
          this.ParseAndAddValue(name, info, str);
      }
      finally
      {
        if (addToStore && info.ParsedValue != null)
          this.AddHeaderToStore(name, info);
      }
    }

    public bool TryAddWithoutValidation(string name, string value)
    {
      if (!this.TryCheckHeaderName(name))
        return false;
      if (value == null)
        value = string.Empty;
      HttpHeaders.AddValue(this.GetOrCreateHeaderInfo(name, false), (object) value, HttpHeaders.StoreLocation.Raw);
      return true;
    }

    public bool TryAddWithoutValidation(string name, IEnumerable<string> values)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      if (!this.TryCheckHeaderName(name))
        return false;
      HttpHeaders.HeaderStoreItemInfo headerInfo = this.GetOrCreateHeaderInfo(name, false);
      foreach (string str in values)
        HttpHeaders.AddValue(headerInfo, (object) (str ?? string.Empty), HttpHeaders.StoreLocation.Raw);
      return true;
    }

    public void Clear()
    {
      if (this.headerStore == null)
        return;
      this.headerStore.Clear();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Remove(string name)
    {
      this.CheckHeaderName(name);
      return this.headerStore != null && this.headerStore.Remove(name);
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
    public IEnumerable<string> GetValues(string name)
    {
      this.CheckHeaderName(name);
      IEnumerable<string> values;
      if (!this.TryGetValues(name, out values))
        throw new InvalidOperationException(SR.net_http_headers_not_found);
      return values;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool TryGetValues(string name, out IEnumerable<string> values)
    {
      if (!this.TryCheckHeaderName(name))
      {
        values = (IEnumerable<string>) null;
        return false;
      }
      if (this.headerStore == null)
      {
        values = (IEnumerable<string>) null;
        return false;
      }
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      if (this.TryGetAndParseHeaderInfo(name, out info))
      {
        values = (IEnumerable<string>) HttpHeaders.GetValuesAsStrings(info);
        return true;
      }
      values = (IEnumerable<string>) null;
      return false;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool Contains(string name)
    {
      this.CheckHeaderName(name);
      if (this.headerStore == null)
        return false;
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      return this.TryGetAndParseHeaderInfo(name, out info);
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in this)
      {
        stringBuilder.Append(keyValuePair.Key);
        stringBuilder.Append(": ");
        stringBuilder.Append(this.GetHeaderString(keyValuePair.Key));
        stringBuilder.Append("\r\n");
      }
      return stringBuilder.ToString();
    }

    internal IEnumerable<KeyValuePair<string, string>> GetHeaderStrings()
    {
      if (this.headerStore != null)
      {
        foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> header in this.headerStore)
        {
          HttpHeaders.HeaderStoreItemInfo info = header.Value;
          string stringValue = this.GetHeaderString(info);
          yield return new KeyValuePair<string, string>(header.Key, stringValue);
        }
      }
    }

    internal string GetHeaderString(string headerName)
    {
      return this.GetHeaderString(headerName, (object) null);
    }

    internal string GetHeaderString(string headerName, object exclude)
    {
      HttpHeaders.HeaderStoreItemInfo info;
      return !this.TryGetHeaderInfo(headerName, out info) ? string.Empty : this.GetHeaderString(info, exclude);
    }

    private string GetHeaderString(HttpHeaders.HeaderStoreItemInfo info)
    {
      return this.GetHeaderString(info, (object) null);
    }

    private string GetHeaderString(HttpHeaders.HeaderStoreItemInfo info, object exclude)
    {
      string empty = string.Empty;
      string[] valuesAsStrings = HttpHeaders.GetValuesAsStrings(info, exclude);
      string headerString;
      if (valuesAsStrings.Length == 1)
      {
        headerString = valuesAsStrings[0];
      }
      else
      {
        string separator = ", ";
        if (info.Parser != null && info.Parser.SupportsMultipleValues)
          separator = info.Parser.Separator;
        headerString = string.Join(separator, valuesAsStrings);
      }
      return headerString;
    }

    /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerator`1" />.</returns>
    public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
    {
      if (this.headerStore != null)
      {
        List<string> invalidHeaders = (List<string>) null;
        foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> header in this.headerStore)
        {
          HttpHeaders.HeaderStoreItemInfo info = header.Value;
          if (!this.ParseRawHeaderValues(header.Key, info, false))
          {
            if (invalidHeaders == null)
              invalidHeaders = new List<string>();
            invalidHeaders.Add(header.Key);
          }
          else
          {
            string[] values = HttpHeaders.GetValuesAsStrings(info);
            yield return new KeyValuePair<string, IEnumerable<string>>(header.Key, (IEnumerable<string>) values);
          }
        }
        if (invalidHeaders != null)
        {
          Contract.Assert(this.headerStore != null);
          foreach (string key in invalidHeaders)
            this.headerStore.Remove(key);
        }
      }
    }

    /// <returns>Returns <see cref="T:System.Collections.IEnumerator" />.</returns>
    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    internal void SetConfiguration(
      Dictionary<string, HttpHeaderParser> parserStore,
      HashSet<string> invalidHeaders)
    {
      Contract.Assert(this.parserStore == null, "Parser store was already set.");
      this.parserStore = parserStore;
      this.invalidHeaders = invalidHeaders;
    }

    internal void AddParsedValue(string name, object value)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      Contract.Requires(value != null);
      HttpHeaders.HeaderStoreItemInfo headerInfo = this.GetOrCreateHeaderInfo(name, true);
      Contract.Assert(headerInfo.Parser != null, "Can't add parsed value if there is no parser available.");
      Contract.Assert(headerInfo.CanAddValue, "Header '" + name + "' doesn't support multiple values");
      HttpHeaders.AddValue(headerInfo, value, HttpHeaders.StoreLocation.Parsed);
    }

    internal void SetParsedValue(string name, object value)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      Contract.Requires(value != null);
      HttpHeaders.HeaderStoreItemInfo headerInfo = this.GetOrCreateHeaderInfo(name, true);
      Contract.Assert(headerInfo.Parser != null, "Can't add parsed value if there is no parser available.");
      headerInfo.InvalidValue = (object) null;
      headerInfo.ParsedValue = (object) null;
      headerInfo.RawValue = (object) null;
      HttpHeaders.AddValue(headerInfo, value, HttpHeaders.StoreLocation.Parsed);
    }

    internal void SetOrRemoveParsedValue(string name, object value)
    {
      if (value == null)
        this.Remove(name);
      else
        this.SetParsedValue(name, value);
    }

    internal bool RemoveParsedValue(string name, object value)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      Contract.Requires(value != null);
      if (this.headerStore == null)
        return false;
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      if (!this.TryGetAndParseHeaderInfo(name, out info))
        return false;
      Contract.Assert(info.Parser != null, "Can't add parsed value if there is no parser available.");
      Contract.Assert(info.Parser.SupportsMultipleValues, "This method should not be used for single-value headers. Use Remove(string) instead.");
      bool flag = false;
      if (info.ParsedValue == null)
        return false;
      IEqualityComparer comparer = info.Parser.Comparer;
      if (!(info.ParsedValue is List<object> parsedValue))
      {
        Contract.Assert(info.ParsedValue.GetType() == value.GetType(), "Stored value does not have the same type as 'value'.");
        if (this.AreEqual(value, info.ParsedValue, comparer))
        {
          info.ParsedValue = (object) null;
          flag = true;
        }
      }
      else
      {
        foreach (object storeValue in parsedValue)
        {
          Contract.Assert(storeValue.GetType() == value.GetType(), "One of the stored values does not have the same type as 'value'.");
          if (this.AreEqual(value, storeValue, comparer))
          {
            flag = parsedValue.Remove(storeValue);
            break;
          }
        }
        if (parsedValue.Count == 0)
          info.ParsedValue = (object) null;
      }
      if (info.IsEmpty)
        Contract.Assert(this.Remove(name), "Existing header '" + name + "' couldn't be removed.");
      return flag;
    }

    internal bool ContainsParsedValue(string name, object value)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      Contract.Requires(value != null);
      if (this.headerStore == null)
        return false;
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      if (!this.TryGetAndParseHeaderInfo(name, out info))
        return false;
      Contract.Assert(info.Parser != null, "Can't add parsed value if there is no parser available.");
      Contract.Assert(info.Parser.SupportsMultipleValues, "This method should not be used for single-value headers. Use equality comparer instead.");
      if (info.ParsedValue == null)
        return false;
      List<object> parsedValue = info.ParsedValue as List<object>;
      IEqualityComparer comparer = info.Parser.Comparer;
      if (parsedValue == null)
      {
        Contract.Assert(info.ParsedValue.GetType() == value.GetType(), "Stored value does not have the same type as 'value'.");
        return this.AreEqual(value, info.ParsedValue, comparer);
      }
      foreach (object storeValue in parsedValue)
      {
        Contract.Assert(storeValue.GetType() == value.GetType(), "One of the stored values does not have the same type as 'value'.");
        if (this.AreEqual(value, storeValue, comparer))
          return true;
      }
      return false;
    }

    internal virtual void AddHeaders(HttpHeaders sourceHeaders)
    {
      Contract.Requires(sourceHeaders != null);
      Contract.Assert(this.parserStore == sourceHeaders.parserStore, "Can only copy headers from an instance with the same header parsers.");
      if (sourceHeaders.headerStore == null)
        return;
      List<string> stringList = (List<string>) null;
      foreach (KeyValuePair<string, HttpHeaders.HeaderStoreItemInfo> keyValuePair in sourceHeaders.headerStore)
      {
        if (this.headerStore == null || !this.headerStore.ContainsKey(keyValuePair.Key))
        {
          HttpHeaders.HeaderStoreItemInfo headerStoreItemInfo = keyValuePair.Value;
          if (!sourceHeaders.ParseRawHeaderValues(keyValuePair.Key, headerStoreItemInfo, false))
          {
            if (stringList == null)
              stringList = new List<string>();
            stringList.Add(keyValuePair.Key);
          }
          else
            this.AddHeaderInfo(keyValuePair.Key, headerStoreItemInfo);
        }
      }
      if (stringList == null)
        return;
      Contract.Assert(sourceHeaders.headerStore != null);
      foreach (string key in stringList)
        sourceHeaders.headerStore.Remove(key);
    }

    private void AddHeaderInfo(string headerName, HttpHeaders.HeaderStoreItemInfo sourceInfo)
    {
      HttpHeaders.HeaderStoreItemInfo addHeaderToStore = this.CreateAndAddHeaderToStore(headerName);
      Contract.Assert(sourceInfo.Parser == addHeaderToStore.Parser, "Expected same parser on both source and destination header store for header '" + headerName + "'.");
      if (addHeaderToStore.Parser == null)
      {
        Contract.Assert(sourceInfo.RawValue == null && sourceInfo.InvalidValue == null, "No raw or invalid values expected for custom headers.");
        addHeaderToStore.ParsedValue = HttpHeaders.CloneStringHeaderInfoValues(sourceInfo.ParsedValue);
      }
      else
      {
        addHeaderToStore.InvalidValue = HttpHeaders.CloneStringHeaderInfoValues(sourceInfo.InvalidValue);
        if (sourceInfo.ParsedValue == null)
          return;
        if (!(sourceInfo.ParsedValue is List<object> parsedValue))
        {
          HttpHeaders.CloneAndAddValue(addHeaderToStore, sourceInfo.ParsedValue);
        }
        else
        {
          foreach (object source in parsedValue)
            HttpHeaders.CloneAndAddValue(addHeaderToStore, source);
        }
      }
    }

    private static void CloneAndAddValue(
      HttpHeaders.HeaderStoreItemInfo destinationInfo,
      object source)
    {
      if (source is ICloneable cloneable)
        HttpHeaders.AddValue(destinationInfo, cloneable.Clone(), HttpHeaders.StoreLocation.Parsed);
      else
        HttpHeaders.AddValue(destinationInfo, source, HttpHeaders.StoreLocation.Parsed);
    }

    private static object CloneStringHeaderInfoValues(object source)
    {
      if (source == null)
        return (object) null;
      return !(source is List<object> collection) ? source : (object) new List<object>((IEnumerable<object>) collection);
    }

    private HttpHeaders.HeaderStoreItemInfo GetOrCreateHeaderInfo(string name, bool parseRawValues)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      Contract.Ensures(Contract.Result<HttpHeaders.HeaderStoreItemInfo>() != null);
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      if (!(!parseRawValues ? this.TryGetHeaderInfo(name, out info) : this.TryGetAndParseHeaderInfo(name, out info)))
        info = this.CreateAndAddHeaderToStore(name);
      return info;
    }

    private HttpHeaders.HeaderStoreItemInfo CreateAndAddHeaderToStore(string name)
    {
      HttpHeaders.HeaderStoreItemInfo info = new HttpHeaders.HeaderStoreItemInfo(this.GetParser(name));
      this.AddHeaderToStore(name, info);
      return info;
    }

    private void AddHeaderToStore(string name, HttpHeaders.HeaderStoreItemInfo info)
    {
      if (this.headerStore == null)
        this.headerStore = new Dictionary<string, HttpHeaders.HeaderStoreItemInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.headerStore.Add(name, info);
    }

    private bool TryGetHeaderInfo(string name, out HttpHeaders.HeaderStoreItemInfo info)
    {
      if (this.headerStore != null)
        return this.headerStore.TryGetValue(name, out info);
      info = (HttpHeaders.HeaderStoreItemInfo) null;
      return false;
    }

    private bool TryGetAndParseHeaderInfo(string name, out HttpHeaders.HeaderStoreItemInfo info)
    {
      return this.TryGetHeaderInfo(name, out info) && this.ParseRawHeaderValues(name, info, true);
    }

    private bool ParseRawHeaderValues(
      string name,
      HttpHeaders.HeaderStoreItemInfo info,
      bool removeEmptyHeader)
    {
      Contract.Ensures(info.RawValue == null);
      lock (info)
      {
        if (info.RawValue != null)
        {
          if (!(info.RawValue is List<string> rawValue))
            HttpHeaders.ParseSingleRawHeaderValue(name, info);
          else
            HttpHeaders.ParseMultipleRawHeaderValues(name, info, rawValue);
          info.RawValue = (object) null;
          if (info.InvalidValue == null)
          {
            if (info.ParsedValue == null)
            {
              if (removeEmptyHeader)
              {
                Contract.Assert(this.headerStore != null);
                this.headerStore.Remove(name);
              }
              return false;
            }
          }
        }
      }
      return true;
    }

    private static void ParseMultipleRawHeaderValues(
      string name,
      HttpHeaders.HeaderStoreItemInfo info,
      List<string> rawValues)
    {
      if (info.Parser == null)
      {
        foreach (string rawValue in rawValues)
        {
          if (!HttpHeaders.ContainsInvalidNewLine(rawValue, name))
            HttpHeaders.AddValue(info, (object) rawValue, HttpHeaders.StoreLocation.Parsed);
        }
      }
      else
      {
        foreach (string rawValue in rawValues)
        {
          if (!HttpHeaders.TryParseAndAddRawHeaderValue(name, info, rawValue, true) && Logging.On)
            Logging.PrintWarning(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_headers_invalid_value, (object) name, (object) rawValue));
        }
      }
    }

    private static void ParseSingleRawHeaderValue(string name, HttpHeaders.HeaderStoreItemInfo info)
    {
      string rawValue = info.RawValue as string;
      Contract.Assert(rawValue != null, "RawValue must either be List<string> or string.");
      if (info.Parser == null)
      {
        if (HttpHeaders.ContainsInvalidNewLine(rawValue, name))
          return;
        info.ParsedValue = info.RawValue;
      }
      else
      {
        if (HttpHeaders.TryParseAndAddRawHeaderValue(name, info, rawValue, true) || !Logging.On)
          return;
        Logging.PrintWarning(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_headers_invalid_value, (object) name, (object) rawValue));
      }
    }

    internal bool TryParseAndAddValue(string name, string value)
    {
      HttpHeaders.HeaderStoreItemInfo info;
      bool addToStore;
      this.PrepareHeaderInfoForAdd(name, out info, out addToStore);
      bool addRawHeaderValue = HttpHeaders.TryParseAndAddRawHeaderValue(name, info, value, false);
      if (addRawHeaderValue && addToStore && info.ParsedValue != null)
        this.AddHeaderToStore(name, info);
      return addRawHeaderValue;
    }

    private static bool TryParseAndAddRawHeaderValue(
      string name,
      HttpHeaders.HeaderStoreItemInfo info,
      string value,
      bool addWhenInvalid)
    {
      Contract.Requires(info != null);
      Contract.Requires(info.Parser != null);
      if (!info.CanAddValue)
      {
        if (addWhenInvalid)
          HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Invalid);
        return false;
      }
      int index = 0;
      object parsedValue = (object) null;
      if (info.Parser.TryParseValue(value, info.ParsedValue, ref index, out parsedValue))
      {
        if (value == null || index == value.Length)
        {
          if (parsedValue != null)
            HttpHeaders.AddValue(info, parsedValue, HttpHeaders.StoreLocation.Parsed);
          return true;
        }
        Contract.Assert(index < value.Length, "Parser must return an index value within the string length.");
        List<object> objectList = new List<object>();
        if (parsedValue != null)
          objectList.Add(parsedValue);
        while (index < value.Length)
        {
          if (info.Parser.TryParseValue(value, info.ParsedValue, ref index, out parsedValue))
          {
            if (parsedValue != null)
              objectList.Add(parsedValue);
          }
          else
          {
            if (!HttpHeaders.ContainsInvalidNewLine(value, name) && addWhenInvalid)
              HttpHeaders.AddValue(info, (object) value, HttpHeaders.StoreLocation.Invalid);
            return false;
          }
        }
        foreach (object obj in objectList)
          HttpHeaders.AddValue(info, obj, HttpHeaders.StoreLocation.Parsed);
        return true;
      }
      if (!HttpHeaders.ContainsInvalidNewLine(value, name) && addWhenInvalid)
        HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Invalid);
      return false;
    }

    private static void AddValue(
      HttpHeaders.HeaderStoreItemInfo info,
      object value,
      HttpHeaders.StoreLocation location)
    {
      Contract.Assert(info.Parser != null || info.Parser == null && value.GetType() == typeof (string), "If no parser is defined, then the value must be string.");
      switch (location)
      {
        case HttpHeaders.StoreLocation.Raw:
          object rawValue = info.RawValue;
          HttpHeaders.AddValueToStoreValue<string>(info, value, ref rawValue);
          info.RawValue = rawValue;
          break;
        case HttpHeaders.StoreLocation.Invalid:
          object invalidValue = info.InvalidValue;
          HttpHeaders.AddValueToStoreValue<string>(info, value, ref invalidValue);
          info.InvalidValue = invalidValue;
          break;
        case HttpHeaders.StoreLocation.Parsed:
          Contract.Assert(value == null || !(value is List<object>), "Header value types must not derive from List<object> since this type is used internally to store lists of values. So we would not be able to distinguish between a single value and a list of values.");
          object parsedValue = info.ParsedValue;
          HttpHeaders.AddValueToStoreValue<object>(info, value, ref parsedValue);
          info.ParsedValue = parsedValue;
          break;
        default:
          Contract.Assert(false, "Unknown StoreLocation value: " + location.ToString());
          break;
      }
    }

    private static void AddValueToStoreValue<T>(
      HttpHeaders.HeaderStoreItemInfo info,
      object value,
      ref object currentStoreValue)
      where T : class
    {
      if (currentStoreValue == null)
      {
        currentStoreValue = value;
      }
      else
      {
        if (!(currentStoreValue is List<T> objList))
        {
          objList = new List<T>(2);
          Contract.Assert(value is T);
          objList.Add(currentStoreValue as T);
          currentStoreValue = (object) objList;
        }
        Contract.Assert(value is T);
        objList.Add(value as T);
      }
    }

    internal object GetParsedValues(string name)
    {
      Contract.Requires(name != null && name.Length > 0);
      Contract.Requires(HttpRuleParser.GetTokenLength(name, 0) == name.Length);
      HttpHeaders.HeaderStoreItemInfo info = (HttpHeaders.HeaderStoreItemInfo) null;
      return !this.TryGetAndParseHeaderInfo(name, out info) ? (object) null : info.ParsedValue;
    }

    private void PrepareHeaderInfoForAdd(
      string name,
      out HttpHeaders.HeaderStoreItemInfo info,
      out bool addToStore)
    {
      info = (HttpHeaders.HeaderStoreItemInfo) null;
      addToStore = false;
      if (this.TryGetAndParseHeaderInfo(name, out info))
        return;
      info = new HttpHeaders.HeaderStoreItemInfo(this.GetParser(name));
      addToStore = true;
    }

    private void ParseAndAddValue(string name, HttpHeaders.HeaderStoreItemInfo info, string value)
    {
      Contract.Requires(info != null);
      if (info.Parser == null)
      {
        HttpHeaders.CheckInvalidNewLine(value);
        HttpHeaders.AddValue(info, (object) (value ?? string.Empty), HttpHeaders.StoreLocation.Parsed);
      }
      else
      {
        if (!info.CanAddValue)
          throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_single_value_header, (object) name));
        int index = 0;
        object obj1 = info.Parser.ParseValue(value, info.ParsedValue, ref index);
        if (value == null || index == value.Length)
        {
          if (obj1 == null)
            return;
          HttpHeaders.AddValue(info, obj1, HttpHeaders.StoreLocation.Parsed);
        }
        else
        {
          Contract.Assert(index < value.Length, "Parser must return an index value within the string length.");
          List<object> objectList = new List<object>();
          if (obj1 != null)
            objectList.Add(obj1);
          while (index < value.Length)
          {
            object obj2 = info.Parser.ParseValue(value, info.ParsedValue, ref index);
            if (obj2 != null)
              objectList.Add(obj2);
          }
          foreach (object obj3 in objectList)
            HttpHeaders.AddValue(info, obj3, HttpHeaders.StoreLocation.Parsed);
        }
      }
    }

    private HttpHeaderParser GetParser(string name)
    {
      if (this.parserStore == null)
        return (HttpHeaderParser) null;
      HttpHeaderParser httpHeaderParser = (HttpHeaderParser) null;
      return this.parserStore.TryGetValue(name, out httpHeaderParser) ? httpHeaderParser : (HttpHeaderParser) null;
    }

    private void CheckHeaderName(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (name));
      if (HttpRuleParser.GetTokenLength(name, 0) != name.Length)
        throw new FormatException(SR.net_http_headers_invalid_header_name);
      if (this.invalidHeaders != null && this.invalidHeaders.Contains(name))
        throw new InvalidOperationException(SR.net_http_headers_not_allowed_header_name);
    }

    private bool TryCheckHeaderName(string name)
    {
      return !string.IsNullOrEmpty(name) && HttpRuleParser.GetTokenLength(name, 0) == name.Length && (this.invalidHeaders == null || !this.invalidHeaders.Contains(name));
    }

    private static void CheckInvalidNewLine(string value)
    {
      if (value != null && HttpRuleParser.ContainsInvalidNewLine(value))
        throw new FormatException(SR.net_http_headers_no_newlines);
    }

    private static bool ContainsInvalidNewLine(string value, string name)
    {
      if (!HttpRuleParser.ContainsInvalidNewLine(value))
        return false;
      if (Logging.On)
        Logging.PrintError(Logging.Http, string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_log_headers_no_newlines, (object) name, (object) value));
      return true;
    }

    private static string[] GetValuesAsStrings(HttpHeaders.HeaderStoreItemInfo info)
    {
      return HttpHeaders.GetValuesAsStrings(info, (object) null);
    }

    private static string[] GetValuesAsStrings(HttpHeaders.HeaderStoreItemInfo info, object exclude)
    {
      Contract.Ensures(Contract.Result<string[]>() != null);
      int valueCount = HttpHeaders.GetValueCount(info);
      string[] valuesAsStrings = new string[valueCount];
      if (valueCount > 0)
      {
        int currentIndex = 0;
        HttpHeaders.ReadStoreValues<string>(valuesAsStrings, info.RawValue, (HttpHeaderParser) null, (string) null, ref currentIndex);
        HttpHeaders.ReadStoreValues<object>(valuesAsStrings, info.ParsedValue, info.Parser, exclude, ref currentIndex);
        HttpHeaders.ReadStoreValues<string>(valuesAsStrings, info.InvalidValue, (HttpHeaderParser) null, (string) null, ref currentIndex);
        if (currentIndex < valueCount)
        {
          string[] destinationArray = new string[currentIndex];
          Array.Copy((Array) valuesAsStrings, (Array) destinationArray, currentIndex);
          valuesAsStrings = destinationArray;
        }
      }
      return valuesAsStrings;
    }

    private static int GetValueCount(HttpHeaders.HeaderStoreItemInfo info)
    {
      Contract.Requires(info != null);
      int valueCount = 0;
      HttpHeaders.UpdateValueCount<string>(info.RawValue, ref valueCount);
      HttpHeaders.UpdateValueCount<string>(info.InvalidValue, ref valueCount);
      HttpHeaders.UpdateValueCount<object>(info.ParsedValue, ref valueCount);
      return valueCount;
    }

    private static void UpdateValueCount<T>(object valueStore, ref int valueCount)
    {
      if (valueStore == null)
        return;
      if (valueStore is List<T> objList)
        valueCount += objList.Count;
      else
        ++valueCount;
    }

    private static void ReadStoreValues<T>(
      string[] values,
      object storeValue,
      HttpHeaderParser parser,
      T exclude,
      ref int currentIndex)
    {
      Contract.Requires(values != null);
      if (storeValue == null)
        return;
      if (!(storeValue is List<T> objList))
      {
        if (!HttpHeaders.ShouldAdd<T>(storeValue, parser, exclude))
          return;
        values[currentIndex] = parser == null ? storeValue.ToString() : parser.ToString(storeValue);
        ++currentIndex;
      }
      else
      {
        foreach (T obj in objList)
        {
          object storeValue1 = (object) obj;
          if (HttpHeaders.ShouldAdd<T>(storeValue1, parser, exclude))
          {
            values[currentIndex] = parser == null ? storeValue1.ToString() : parser.ToString(storeValue1);
            ++currentIndex;
          }
        }
      }
    }

    private static bool ShouldAdd<T>(object storeValue, HttpHeaderParser parser, T exclude)
    {
      bool flag = true;
      if (parser != null && (object) exclude != null)
        flag = parser.Comparer == null ? !exclude.Equals(storeValue) : !parser.Comparer.Equals((object) exclude, storeValue);
      return flag;
    }

    private bool AreEqual(object value, object storeValue, IEqualityComparer comparer)
    {
      Contract.Requires(value != null);
      return comparer != null ? comparer.Equals(value, storeValue) : value.Equals(storeValue);
    }

    private enum StoreLocation
    {
      Raw,
      Invalid,
      Parsed,
    }

    private class HeaderStoreItemInfo
    {
      private object rawValue;
      private object invalidValue;
      private object parsedValue;
      private HttpHeaderParser parser;

      internal object RawValue
      {
        get => this.rawValue;
        set => this.rawValue = value;
      }

      internal object InvalidValue
      {
        get => this.invalidValue;
        set => this.invalidValue = value;
      }

      internal object ParsedValue
      {
        get => this.parsedValue;
        set => this.parsedValue = value;
      }

      internal HttpHeaderParser Parser => this.parser;

      internal bool CanAddValue
      {
        get
        {
          Contract.Assert(this.parser != null, "There should be no reason to call CanAddValue if there is no parser for the current header.");
          if (this.parser.SupportsMultipleValues)
            return true;
          return this.invalidValue == null && this.parsedValue == null;
        }
      }

      internal bool IsEmpty
      {
        get => this.rawValue == null && this.invalidValue == null && this.parsedValue == null;
      }

      internal HeaderStoreItemInfo(HttpHeaderParser parser) => this.parser = parser;
    }
  }
}
