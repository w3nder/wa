// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents the value of the Range header.</summary>
  public class RangeHeaderValue : ICloneable
  {
    private string unit;
    private ICollection<RangeItemHeaderValue> ranges;

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

    /// <returns>Returns <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
    public ICollection<RangeItemHeaderValue> Ranges
    {
      get
      {
        if (this.ranges == null)
          this.ranges = (ICollection<RangeItemHeaderValue>) new ObjectCollection<RangeItemHeaderValue>();
        return this.ranges;
      }
    }

    public RangeHeaderValue() => this.unit = "bytes";

    public RangeHeaderValue(long? from, long? to)
    {
      this.unit = "bytes";
      this.Ranges.Add(new RangeItemHeaderValue(from, to));
    }

    private RangeHeaderValue(RangeHeaderValue source)
    {
      Contract.Requires(source != null);
      this.unit = source.unit;
      if (source.ranges == null)
        return;
      foreach (ICloneable range in (IEnumerable<RangeItemHeaderValue>) source.ranges)
        this.Ranges.Add((RangeItemHeaderValue) range.Clone());
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.unit);
      stringBuilder.Append('=');
      bool flag = true;
      foreach (RangeItemHeaderValue range in (IEnumerable<RangeItemHeaderValue>) this.Ranges)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(", ");
        stringBuilder.Append((object) range.From);
        stringBuilder.Append('-');
        stringBuilder.Append((object) range.To);
      }
      return stringBuilder.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      return obj is RangeHeaderValue rangeHeaderValue && string.Compare(this.unit, rangeHeaderValue.unit, StringComparison.OrdinalIgnoreCase) == 0 && HeaderUtilities.AreEqualCollections<RangeItemHeaderValue>(this.Ranges, rangeHeaderValue.Ranges);
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.unit.ToLowerInvariant().GetHashCode();
      foreach (RangeItemHeaderValue range in (IEnumerable<RangeItemHeaderValue>) this.Ranges)
        hashCode ^= range.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RangeHeaderValue" />.</returns>
    public static RangeHeaderValue Parse(string input)
    {
      int index = 0;
      return (RangeHeaderValue) GenericHeaderParser.RangeParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out RangeHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (RangeHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.RangeParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (RangeHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetRangeLength(string input, int startIndex, out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      RangeHeaderValue rangeHeaderValue = new RangeHeaderValue();
      rangeHeaderValue.unit = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int index = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index == input.Length || input[index] != '=')
        return 0;
      int startIndex2 = index + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      int rangeItemListLength = RangeItemHeaderValue.GetRangeItemListLength(input, startIndex3, rangeHeaderValue.Ranges);
      if (rangeItemListLength == 0)
        return 0;
      int num = startIndex3 + rangeItemListLength;
      Contract.Assert(num == input.Length, "GetRangeItemListLength() should consume the whole string or fail.");
      parsedValue = (object) rangeHeaderValue;
      return num - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new RangeHeaderValue(this);
  }
}
