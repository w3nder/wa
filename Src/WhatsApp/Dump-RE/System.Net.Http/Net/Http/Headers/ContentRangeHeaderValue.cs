// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.ContentRangeHeaderValue
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
  /// <summary>Represents the value of the Content-Range header.</summary>
  public class ContentRangeHeaderValue : ICloneable
  {
    private string unit;
    private long? from;
    private long? to;
    private long? length;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Unit
    {
      get => this.unit;
      set
      {
        HeaderUtilities.CheckValidToken(value, nameof (value));
        this.unit = value;
      }
    }

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? From => this.from;

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? To => this.to;

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? Length => this.length;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool HasLength => this.length.HasValue;

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public bool HasRange => this.from.HasValue;

    public ContentRangeHeaderValue(long from, long to, long length)
    {
      if (length < 0L)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (length));
      if (to < 0L || to > length)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (to));
      this.from = from >= 0L && from <= to ? new long?(from) : throw new System.Net.Http.ArgumentOutOfRangeException(nameof (from));
      this.to = new long?(to);
      this.length = new long?(length);
      this.unit = "bytes";
    }

    public ContentRangeHeaderValue(long length)
    {
      this.length = length >= 0L ? new long?(length) : throw new System.Net.Http.ArgumentOutOfRangeException(nameof (length));
      this.unit = "bytes";
    }

    public ContentRangeHeaderValue(long from, long to)
    {
      if (to < 0L)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (to));
      this.from = from >= 0L && from <= to ? new long?(from) : throw new System.Net.Http.ArgumentOutOfRangeException(nameof (from));
      this.to = new long?(to);
      this.unit = "bytes";
    }

    private ContentRangeHeaderValue()
    {
    }

    private ContentRangeHeaderValue(ContentRangeHeaderValue source)
    {
      Contract.Requires(source != null);
      this.from = source.from;
      this.to = source.to;
      this.length = source.length;
      this.unit = source.unit;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is ContentRangeHeaderValue rangeHeaderValue))
        return false;
      long? from1 = this.from;
      long? from2 = rangeHeaderValue.from;
      if ((from1.GetValueOrDefault() != from2.GetValueOrDefault() ? 0 : (from1.HasValue == from2.HasValue ? 1 : 0)) != 0)
      {
        long? to1 = this.to;
        long? to2 = rangeHeaderValue.to;
        if ((to1.GetValueOrDefault() != to2.GetValueOrDefault() ? 0 : (to1.HasValue == to2.HasValue ? 1 : 0)) != 0)
        {
          long? length1 = this.length;
          long? length2 = rangeHeaderValue.length;
          if ((length1.GetValueOrDefault() != length2.GetValueOrDefault() ? 0 : (length1.HasValue == length2.HasValue ? 1 : 0)) != 0)
            return string.Compare(this.unit, rangeHeaderValue.unit, StringComparison.OrdinalIgnoreCase) == 0;
        }
      }
      return false;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.unit.ToLowerInvariant().GetHashCode();
      if (this.HasRange)
        hashCode = hashCode ^ this.from.GetHashCode() ^ this.to.GetHashCode();
      if (this.HasLength)
        hashCode ^= this.length.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.unit);
      stringBuilder.Append(' ');
      if (this.HasRange)
      {
        stringBuilder.Append(this.from.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
        stringBuilder.Append('-');
        stringBuilder.Append(this.to.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      }
      else
        stringBuilder.Append('*');
      stringBuilder.Append('/');
      if (this.HasLength)
        stringBuilder.Append(this.length.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      else
        stringBuilder.Append('*');
      return stringBuilder.ToString();
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.ContentRangeHeaderValue" />.</returns>
    public static ContentRangeHeaderValue Parse(string input)
    {
      int index = 0;
      return (ContentRangeHeaderValue) GenericHeaderParser.ContentRangeParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out ContentRangeHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (ContentRangeHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.ContentRangeParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (ContentRangeHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetContentRangeLength(string input, int startIndex, out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      string unit = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (whitespaceLength == 0)
        return 0;
      int current1 = startIndex1 + whitespaceLength;
      if (current1 == input.Length)
        return 0;
      int fromStartIndex = current1;
      int fromLength = 0;
      int toStartIndex = 0;
      int toLength = 0;
      if (!ContentRangeHeaderValue.TryGetRangeLength(input, ref current1, out fromLength, out toStartIndex, out toLength) || current1 == input.Length || input[current1] != '/')
        return 0;
      int startIndex2 = current1 + 1;
      int current2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      if (current2 == input.Length)
        return 0;
      int lengthStartIndex = current2;
      int lengthLength = 0;
      return !ContentRangeHeaderValue.TryGetLengthLength(input, ref current2, out lengthLength) || !ContentRangeHeaderValue.TryCreateContentRange(input, unit, fromStartIndex, fromLength, toStartIndex, toLength, lengthStartIndex, lengthLength, out parsedValue) ? 0 : current2 - startIndex;
    }

    private static bool TryGetLengthLength(string input, ref int current, out int lengthLength)
    {
      lengthLength = 0;
      if (input[current] == '*')
      {
        ++current;
      }
      else
      {
        lengthLength = HttpRuleParser.GetNumberLength(input, current, false);
        if (lengthLength == 0 || lengthLength > 19)
          return false;
        current += lengthLength;
      }
      current += HttpRuleParser.GetWhitespaceLength(input, current);
      return true;
    }

    private static bool TryGetRangeLength(
      string input,
      ref int current,
      out int fromLength,
      out int toStartIndex,
      out int toLength)
    {
      fromLength = 0;
      toStartIndex = 0;
      toLength = 0;
      if (input[current] == '*')
      {
        ++current;
      }
      else
      {
        fromLength = HttpRuleParser.GetNumberLength(input, current, false);
        if (fromLength == 0 || fromLength > 19)
          return false;
        current += fromLength;
        current += HttpRuleParser.GetWhitespaceLength(input, current);
        if (current == input.Length || input[current] != '-')
          return false;
        ++current;
        current += HttpRuleParser.GetWhitespaceLength(input, current);
        if (current == input.Length)
          return false;
        toStartIndex = current;
        toLength = HttpRuleParser.GetNumberLength(input, current, false);
        if (toLength == 0 || toLength > 19)
          return false;
        current += toLength;
      }
      current += HttpRuleParser.GetWhitespaceLength(input, current);
      return true;
    }

    private static bool TryCreateContentRange(
      string input,
      string unit,
      int fromStartIndex,
      int fromLength,
      int toStartIndex,
      int toLength,
      int lengthStartIndex,
      int lengthLength,
      out object parsedValue)
    {
      parsedValue = (object) null;
      long result1 = 0;
      if (fromLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(fromStartIndex, fromLength), out result1))
        return false;
      long result2 = 0;
      if (toLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(toStartIndex, toLength), out result2) || fromLength > 0 && toLength > 0 && result1 > result2)
        return false;
      long result3 = 0;
      if (lengthLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(lengthStartIndex, lengthLength), out result3) || toLength > 0 && lengthLength > 0 && result2 >= result3)
        return false;
      ContentRangeHeaderValue rangeHeaderValue = new ContentRangeHeaderValue();
      rangeHeaderValue.unit = unit;
      if (fromLength > 0)
      {
        rangeHeaderValue.from = new long?(result1);
        rangeHeaderValue.to = new long?(result2);
      }
      if (lengthLength > 0)
        rangeHeaderValue.length = new long?(result3);
      parsedValue = (object) rangeHeaderValue;
      return true;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new ContentRangeHeaderValue(this);
  }
}
