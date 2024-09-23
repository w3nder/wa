// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeItemHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a byte-range header value.</summary>
  public class RangeItemHeaderValue : ICloneable
  {
    private long? from;
    private long? to;

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? From => this.from;

    /// <returns>Returns <see cref="T:System.Int64" />.</returns>
    public long? To => this.to;

    public RangeItemHeaderValue(long? from, long? to)
    {
      if (!from.HasValue && !to.HasValue)
        throw new ArgumentException(SR.net_http_headers_invalid_range);
      if (from.HasValue && from.Value < 0L)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (from));
      if (to.HasValue && to.Value < 0L)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (to));
      if (from.HasValue && to.HasValue && from.Value > to.Value)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (from));
      this.from = from;
      this.to = to;
    }

    private RangeItemHeaderValue(RangeItemHeaderValue source)
    {
      Contract.Requires(source != null);
      this.from = source.from;
      this.to = source.to;
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      if (!this.from.HasValue)
        return "-" + this.to.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      return !this.to.HasValue ? this.from.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "-" : this.from.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "-" + this.to.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is RangeItemHeaderValue rangeItemHeaderValue))
        return false;
      long? from1 = this.from;
      long? from2 = rangeItemHeaderValue.from;
      if ((from1.GetValueOrDefault() != from2.GetValueOrDefault() ? 0 : (from1.HasValue == from2.HasValue ? 1 : 0)) == 0)
        return false;
      long? to1 = this.to;
      long? to2 = rangeItemHeaderValue.to;
      return to1.GetValueOrDefault() == to2.GetValueOrDefault() && to1.HasValue == to2.HasValue;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      if (!this.from.HasValue)
        return this.to.GetHashCode();
      return !this.to.HasValue ? this.from.GetHashCode() : this.from.GetHashCode() ^ this.to.GetHashCode();
    }

    internal static int GetRangeItemListLength(
      string input,
      int startIndex,
      ICollection<RangeItemHeaderValue> rangeCollection)
    {
      Contract.Requires(rangeCollection != null);
      Contract.Requires(startIndex >= 0);
      Contract.Ensures(Contract.Result<int>() == 0 || rangeCollection.Count > 0, "If we can parse the string, then we expect to have at least one range item.");
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      bool separatorFound = false;
      int orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
      if (orWhitespaceIndex == input.Length)
        return 0;
      RangeItemHeaderValue parsedValue = (RangeItemHeaderValue) null;
      do
      {
        int rangeItemLength = RangeItemHeaderValue.GetRangeItemLength(input, orWhitespaceIndex, out parsedValue);
        if (rangeItemLength == 0)
          return 0;
        rangeCollection.Add(parsedValue);
        int startIndex1 = orWhitespaceIndex + rangeItemLength;
        orWhitespaceIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex1, true, out separatorFound);
        if (orWhitespaceIndex < input.Length && !separatorFound)
          return 0;
      }
      while (orWhitespaceIndex != input.Length);
      return orWhitespaceIndex - startIndex;
    }

    internal static int GetRangeItemLength(
      string input,
      int startIndex,
      out RangeItemHeaderValue parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (RangeItemHeaderValue) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int startIndex1 = startIndex;
      int startIndex2 = startIndex1;
      int numberLength = HttpRuleParser.GetNumberLength(input, startIndex1, false);
      if (numberLength > 19)
        return 0;
      int startIndex3 = startIndex1 + numberLength;
      int index = startIndex3 + HttpRuleParser.GetWhitespaceLength(input, startIndex3);
      if (index == input.Length || input[index] != '-')
        return 0;
      int startIndex4 = index + 1;
      int startIndex5 = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
      int startIndex6 = startIndex5;
      int length = 0;
      if (startIndex5 < input.Length)
      {
        length = HttpRuleParser.GetNumberLength(input, startIndex5, false);
        if (length > 19)
          return 0;
        int startIndex7 = startIndex5 + length;
        startIndex5 = startIndex7 + HttpRuleParser.GetWhitespaceLength(input, startIndex7);
      }
      if (numberLength == 0 && length == 0)
        return 0;
      long result1 = 0;
      if (numberLength > 0 && !HeaderUtilities.TryParseInt64(input.Substring(startIndex2, numberLength), out result1))
        return 0;
      long result2 = 0;
      if (length > 0 && !HeaderUtilities.TryParseInt64(input.Substring(startIndex6, length), out result2) || numberLength > 0 && length > 0 && result1 > result2)
        return 0;
      parsedValue = new RangeItemHeaderValue(numberLength == 0 ? new long?() : new long?(result1), length == 0 ? new long?() : new long?(result2));
      return startIndex5 - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new RangeItemHeaderValue(this);
  }
}
