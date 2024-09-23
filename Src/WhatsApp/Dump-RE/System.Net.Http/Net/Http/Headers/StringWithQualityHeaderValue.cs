// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.StringWithQualityHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a string header value with an optional quality.</summary>
  public class StringWithQualityHeaderValue : ICloneable
  {
    private string value;
    private double? quality;

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public string Value => this.value;

    /// <returns>Returns <see cref="T:System.Double" />.</returns>
    public double? Quality => this.quality;

    public StringWithQualityHeaderValue(string value)
    {
      HeaderUtilities.CheckValidToken(value, nameof (value));
      this.value = value;
    }

    public StringWithQualityHeaderValue(string value, double quality)
    {
      HeaderUtilities.CheckValidToken(value, nameof (value));
      if (quality < 0.0 || quality > 1.0)
        throw new System.Net.Http.ArgumentOutOfRangeException(nameof (quality));
      this.value = value;
      this.quality = new double?(quality);
    }

    private StringWithQualityHeaderValue(StringWithQualityHeaderValue source)
    {
      Contract.Requires(source != null);
      this.value = source.value;
      this.quality = source.quality;
    }

    private StringWithQualityHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.quality.HasValue ? this.value + "; q=" + this.quality.Value.ToString("0.0##", (IFormatProvider) NumberFormatInfo.InvariantInfo) : this.value;
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is StringWithQualityHeaderValue qualityHeaderValue) || string.Compare(this.value, qualityHeaderValue.value, StringComparison.OrdinalIgnoreCase) != 0)
        return false;
      if (!this.quality.HasValue)
        return !qualityHeaderValue.quality.HasValue;
      return qualityHeaderValue.quality.HasValue && this.quality.Value == qualityHeaderValue.quality.Value;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      int hashCode = this.value.ToLowerInvariant().GetHashCode();
      if (this.quality.HasValue)
        hashCode ^= this.quality.Value.GetHashCode();
      return hashCode;
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.StringWithQualityHeaderValue" />.</returns>
    public static StringWithQualityHeaderValue Parse(string input)
    {
      int index = 0;
      return (StringWithQualityHeaderValue) GenericHeaderParser.SingleValueStringWithQualityParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out StringWithQualityHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (StringWithQualityHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.SingleValueStringWithQualityParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (StringWithQualityHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetStringWithQualityLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
      if (tokenLength == 0)
        return 0;
      StringWithQualityHeaderValue result = new StringWithQualityHeaderValue();
      result.value = input.Substring(startIndex, tokenLength);
      int startIndex1 = startIndex + tokenLength;
      int index1 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index1 == input.Length || input[index1] != ';')
      {
        parsedValue = (object) result;
        return index1 - startIndex;
      }
      int startIndex2 = index1 + 1;
      int index2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      if (!StringWithQualityHeaderValue.TryReadQuality(input, result, ref index2))
        return 0;
      parsedValue = (object) result;
      return index2 - startIndex;
    }

    private static bool TryReadQuality(
      string input,
      StringWithQualityHeaderValue result,
      ref int index)
    {
      int index1 = index;
      if (index1 == input.Length || input[index1] != 'q' && input[index1] != 'Q')
        return false;
      int startIndex1 = index1 + 1;
      int index2 = startIndex1 + HttpRuleParser.GetWhitespaceLength(input, startIndex1);
      if (index2 == input.Length || input[index2] != '=')
        return false;
      int startIndex2 = index2 + 1;
      int startIndex3 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
      if (startIndex3 == input.Length)
        return false;
      int numberLength = HttpRuleParser.GetNumberLength(input, startIndex3, true);
      if (numberLength == 0)
        return false;
      double result1 = 0.0;
      if (!double.TryParse(input.Substring(startIndex3, numberLength), NumberStyles.AllowDecimalPoint, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result1) || result1 < 0.0 || result1 > 1.0)
        return false;
      result.quality = new double?(result1);
      int startIndex4 = startIndex3 + numberLength;
      int num = startIndex4 + HttpRuleParser.GetWhitespaceLength(input, startIndex4);
      index = num;
      return true;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new StringWithQualityHeaderValue(this);
  }
}
