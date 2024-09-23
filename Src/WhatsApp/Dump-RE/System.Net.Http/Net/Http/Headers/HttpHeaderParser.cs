// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.HttpHeaderParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections;
using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  internal abstract class HttpHeaderParser
  {
    internal const string DefaultSeparator = ", ";
    private bool supportsMultipleValues;
    private string separator;

    public bool SupportsMultipleValues => this.supportsMultipleValues;

    public string Separator
    {
      get
      {
        Contract.Assert(this.supportsMultipleValues);
        return this.separator;
      }
    }

    public virtual IEqualityComparer Comparer => (IEqualityComparer) null;

    protected HttpHeaderParser(bool supportsMultipleValues)
    {
      this.supportsMultipleValues = supportsMultipleValues;
      if (!supportsMultipleValues)
        return;
      this.separator = ", ";
    }

    protected HttpHeaderParser(bool supportsMultipleValues, string separator)
    {
      Contract.Requires(!string.IsNullOrEmpty(separator));
      this.supportsMultipleValues = supportsMultipleValues;
      this.separator = separator;
    }

    public abstract bool TryParseValue(
      string value,
      object storeValue,
      ref int index,
      out object parsedValue);

    public object ParseValue(string value, object storeValue, ref int index)
    {
      Contract.Requires(value == null || index >= 0 && index <= value.Length);
      object parsedValue = (object) null;
      if (!this.TryParseValue(value, storeValue, ref index, out parsedValue))
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, value == null ? (object) "<null>" : (object) value.Substring(index)));
      return parsedValue;
    }

    public virtual string ToString(object value)
    {
      Contract.Requires(value != null);
      return value.ToString();
    }
  }
}
