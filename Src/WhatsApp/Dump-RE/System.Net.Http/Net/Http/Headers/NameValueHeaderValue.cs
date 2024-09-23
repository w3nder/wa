// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.NameValueHeaderValue
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
  /// <summary>Represents a name/value pair.</summary>
  public class NameValueHeaderValue : ICloneable
  {
    private static readonly Func<NameValueHeaderValue> defaultNameValueCreator = new Func<NameValueHeaderValue>(NameValueHeaderValue.CreateNameValue);
    private string name;
    private string value;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Name => this.name;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Value
    {
      get => this.value;
      set
      {
        NameValueHeaderValue.CheckValueFormat(value);
        this.value = value;
      }
    }

    internal NameValueHeaderValue()
    {
    }

    public NameValueHeaderValue(string name)
      : this(name, (string) null)
    {
    }

    public NameValueHeaderValue(string name, string value)
    {
      NameValueHeaderValue.CheckNameValueFormat(name, value);
      this.name = name;
      this.value = value;
    }

    protected NameValueHeaderValue(NameValueHeaderValue source)
    {
      Contract.Requires(source != null);
      this.name = source.name;
      this.value = source.value;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      Contract.Assert(this.name != null);
      int hashCode = this.name.ToLowerInvariant().GetHashCode();
      if (string.IsNullOrEmpty(this.value))
        return hashCode;
      return this.value[0] == '"' ? hashCode ^ this.value.GetHashCode() : hashCode ^ this.value.ToLowerInvariant().GetHashCode();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is NameValueHeaderValue valueHeaderValue) || string.Compare(this.name, valueHeaderValue.name, StringComparison.OrdinalIgnoreCase) != 0)
        return false;
      if (string.IsNullOrEmpty(this.value))
        return string.IsNullOrEmpty(valueHeaderValue.value);
      return this.value[0] == '"' ? string.CompareOrdinal(this.value, valueHeaderValue.value) == 0 : string.Compare(this.value, valueHeaderValue.value, StringComparison.OrdinalIgnoreCase) == 0;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.NameValueHeaderValue" />.</returns>
    public static NameValueHeaderValue Parse(string input)
    {
      int index = 0;
      return (NameValueHeaderValue) GenericHeaderParser.SingleValueNameValueParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out NameValueHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (NameValueHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueNameValueParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (NameValueHeaderValue) parsedValue1;
      return true;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return !string.IsNullOrEmpty(this.value) ? this.name + "=" + this.value : this.name;
    }

    internal static void ToString(
      ICollection<NameValueHeaderValue> values,
      char separator,
      bool leadingSeparator,
      StringBuilder destination)
    {
      Contract.Assert(destination != null);
      if (values == null || values.Count == 0)
        return;
      foreach (NameValueHeaderValue valueHeaderValue in (IEnumerable<NameValueHeaderValue>) values)
      {
        if (leadingSeparator || destination.Length > 0)
        {
          destination.Append(separator);
          destination.Append(' ');
        }
        destination.Append(valueHeaderValue.ToString());
      }
    }

    internal static string ToString(
      ICollection<NameValueHeaderValue> values,
      char separator,
      bool leadingSeparator)
    {
      if (values == null || values.Count == 0)
        return (string) null;
      StringBuilder destination = new StringBuilder();
      NameValueHeaderValue.ToString(values, separator, leadingSeparator, destination);
      return destination.ToString();
    }

    internal static int GetHashCode(ICollection<NameValueHeaderValue> values)
    {
      if (values == null || values.Count == 0)
        return 0;
      int hashCode = 0;
      foreach (NameValueHeaderValue valueHeaderValue in (IEnumerable<NameValueHeaderValue>) values)
        hashCode ^= valueHeaderValue.GetHashCode();
      return hashCode;
    }

    internal static int GetNameValueLength(
      string input,
      int startIndex,
      out NameValueHeaderValue parsedValue)
    {
      return NameValueHeaderValue.GetNameValueLength(input, startIndex, NameValueHeaderValue.defaultNameValueCreator, out parsedValue);
    }

    internal static int GetNameValueLength(
      string input,
      int startIndex,
      Func<NameValueHeaderValue> nameValueCreator,
      out NameValueHeaderValue parsedValue)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0);
      Contract.Requires(nameValueCreator != null);
      parsedValue = (NameValueHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      string str = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int num = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (num == input.Length || input[num] != '=')
      {
        parsedValue = nameValueCreator();
        parsedValue.name = str;
        return num + HttpRuleParser.GetWhitespaceLength(input, num) - startIndex;
      }
      int startIndex2 = num + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex3);
      if (valueLength == 0)
        return 0;
      parsedValue = nameValueCreator();
      parsedValue.name = str;
      parsedValue.value = input.Substring(startIndex3, valueLength);
      int startIndex4 = startIndex3 + valueLength;
      return startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4) - startIndex;
    }

    internal static int GetNameValueListLength(
      string input,
      int startIndex,
      char delimiter,
      ICollection<NameValueHeaderValue> nameValueCollection)
    {
      Contract.Requires(nameValueCollection != null);
      Contract.Requires(startIndex >= 0);
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int startIndex1 = startIndex + HttpRuleParser.GetWhitespaceLength(input, startIndex);
      int index;
      while (true)
      {
        NameValueHeaderValue parsedValue = (NameValueHeaderValue) null;
        int nameValueLength = NameValueHeaderValue.GetNameValueLength(input, startIndex1, NameValueHeaderValue.defaultNameValueCreator, out parsedValue);
        if (nameValueLength != 0)
        {
          nameValueCollection.Add(parsedValue);
          int startIndex2 = startIndex1 + nameValueLength;
          index = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
          if (index != input.Length && (int) input[index] == (int) delimiter)
          {
            int startIndex3 = index + 1;
            startIndex1 = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
          }
          else
            goto label_6;
        }
        else
          break;
      }
      return 0;
label_6:
      return index - startIndex;
    }

    internal static NameValueHeaderValue Find(ICollection<NameValueHeaderValue> values, string name)
    {
      Contract.Requires(name != null && name.Length > 0);
      if (values == null || values.Count == 0)
        return (NameValueHeaderValue) null;
      foreach (NameValueHeaderValue valueHeaderValue in (IEnumerable<NameValueHeaderValue>) values)
      {
        if (string.Compare(valueHeaderValue.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
          return valueHeaderValue;
      }
      return (NameValueHeaderValue) null;
    }

    internal static int GetValueLength(string input, int startIndex)
    {
      Contract.Requires(input != null);
      if (startIndex >= input.Length)
        return 0;
      int length = HttpRuleParser.GetTokenLength(input, startIndex);
      return length == 0 && HttpRuleParser.GetQuotedStringLength(input, startIndex, out length) != HttpParseResult.Parsed ? 0 : length;
    }

    private static void CheckNameValueFormat(string name, string value)
    {
      HeaderUtilities.CheckValidToken(name, nameof (name));
      NameValueHeaderValue.CheckValueFormat(value);
    }

    private static void CheckValueFormat(string value)
    {
      if (!string.IsNullOrEmpty(value) && NameValueHeaderValue.GetValueLength(value, 0) != value.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) value));
    }

    private static NameValueHeaderValue CreateNameValue() => new NameValueHeaderValue();

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new NameValueHeaderValue(this);
  }
}
