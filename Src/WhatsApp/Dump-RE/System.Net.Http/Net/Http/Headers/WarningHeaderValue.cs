// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.WarningHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a warning value used by the Warning header.</summary>
  public class WarningHeaderValue : ICloneable
  {
    private int code;
    private string agent;
    private string text;
    private DateTimeOffset? date;

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public int Code => this.code;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Agent => this.agent;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Text => this.text;

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? Date => this.date;

    public WarningHeaderValue(int code, string agent, string text)
    {
      WarningHeaderValue.CheckCode(code);
      WarningHeaderValue.CheckAgent(agent);
      HeaderUtilities.CheckValidQuotedString(text, nameof (text));
      this.code = code;
      this.agent = agent;
      this.text = text;
    }

    public WarningHeaderValue(int code, string agent, string text, DateTimeOffset date)
    {
      WarningHeaderValue.CheckCode(code);
      WarningHeaderValue.CheckAgent(agent);
      HeaderUtilities.CheckValidQuotedString(text, nameof (text));
      this.code = code;
      this.agent = agent;
      this.text = text;
      this.date = new DateTimeOffset?(date);
    }

    private WarningHeaderValue()
    {
    }

    private WarningHeaderValue(WarningHeaderValue source)
    {
      Contract.Requires(source != null);
      this.code = source.code;
      this.agent = source.agent;
      this.text = source.text;
      this.date = source.date;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.code.ToString("000", (IFormatProvider) NumberFormatInfo.InvariantInfo));
      stringBuilder.Append(' ');
      stringBuilder.Append(this.agent);
      stringBuilder.Append(' ');
      stringBuilder.Append(this.text);
      if (this.date.HasValue)
      {
        stringBuilder.Append(" \"");
        stringBuilder.Append(HttpRuleParser.DateToString(this.date.Value));
        stringBuilder.Append('"');
      }
      return stringBuilder.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is WarningHeaderValue warningHeaderValue) || this.code != warningHeaderValue.code || string.Compare(this.agent, warningHeaderValue.agent, StringComparison.OrdinalIgnoreCase) != 0 || string.CompareOrdinal(this.text, warningHeaderValue.text) != 0)
        return false;
      if (!this.date.HasValue)
        return !warningHeaderValue.date.HasValue;
      return warningHeaderValue.date.HasValue && this.date.Value == warningHeaderValue.date.Value;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.code.GetHashCode() ^ this.agent.ToLowerInvariant().GetHashCode() ^ this.text.GetHashCode();
      if (this.date.HasValue)
        hashCode ^= this.date.Value.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.WarningHeaderValue" />.</returns>
    public static WarningHeaderValue Parse(string input)
    {
      int index = 0;
      return (WarningHeaderValue) GenericHeaderParser.SingleValueWarningParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out WarningHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (WarningHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueWarningParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (WarningHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetWarningLength(string input, int startIndex, out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int current1 = startIndex;
      int code;
      string agent;
      if (!WarningHeaderValue.TryReadCode(input, ref current1, out code) || !WarningHeaderValue.TryReadAgent(input, current1, ref current1, out agent))
        return 0;
      int length = 0;
      int startIndex1 = current1;
      if (HttpRuleParser.GetQuotedStringLength(input, current1, out length) != HttpParseResult.Parsed)
        return 0;
      int current2 = current1 + length;
      DateTimeOffset? date = new DateTimeOffset?();
      if (!WarningHeaderValue.TryReadDate(input, ref current2, out date))
        return 0;
      parsedValue = (object) new WarningHeaderValue()
      {
        code = code,
        agent = agent,
        text = input.Substring(startIndex1, length),
        date = date
      };
      return current2 - startIndex;
    }

    private static bool TryReadAgent(
      string input,
      int startIndex,
      ref int current,
      out string agent)
    {
      agent = (string) null;
      int hostLength = HttpRuleParser.GetHostLength(input, startIndex, true, out agent);
      if (hostLength == 0)
        return false;
      current += hostLength;
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
      current += whitespaceLength;
      return whitespaceLength != 0 && current != input.Length;
    }

    private static bool TryReadCode(string input, ref int current, out int code)
    {
      code = 0;
      int numberLength = HttpRuleParser.GetNumberLength(input, current, false);
      if (numberLength == 0 || numberLength > 3)
        return false;
      if (!HeaderUtilities.TryParseInt32(input.Substring(current, numberLength), out code))
      {
        Contract.Assert(false, "Unable to parse value even though it was parsed as <=3 digits string. Input: '" + input + "', Current: " + (object) current + ", CodeLength: " + (object) numberLength);
        return false;
      }
      current += numberLength;
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
      current += whitespaceLength;
      return whitespaceLength != 0 && current != input.Length;
    }

    private static bool TryReadDate(string input, ref int current, out DateTimeOffset? date)
    {
      date = new DateTimeOffset?();
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
      current += whitespaceLength;
      if (current < input.Length && input[current] == '"')
      {
        if (whitespaceLength == 0)
          return false;
        ++current;
        int startIndex = current;
        while (current < input.Length && input[current] != '"')
          ++current;
        DateTimeOffset result;
        if (current == input.Length || current == startIndex || !HttpRuleParser.TryStringToDate(input.Substring(startIndex, current - startIndex), out result))
          return false;
        date = new DateTimeOffset?(result);
        ++current;
        current += HttpRuleParser.GetWhitespaceLength(input, current);
      }
      return true;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new WarningHeaderValue(this);

    private static void CheckCode(int code)
    {
      if (code < 0 || code > 999)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (code));
    }

    private static void CheckAgent(string agent)
    {
      if (string.IsNullOrEmpty(agent))
        throw new ArgumentException(SR.net_http_argument_empty_string, nameof (agent));
      string host = (string) null;
      if (HttpRuleParser.GetHostLength(agent, 0, true, out host) != agent.Length)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, SR.net_http_headers_invalid_value, (object) agent));
    }
  }
}
