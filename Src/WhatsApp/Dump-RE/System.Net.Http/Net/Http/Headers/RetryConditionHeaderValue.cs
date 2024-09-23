// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RetryConditionHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a header value which can either be a date/time or a timespan value.</summary>
  public class RetryConditionHeaderValue : ICloneable
  {
    private DateTimeOffset? date;
    private TimeSpan? delta;

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? Date => this.date;

    /// <returns>Returns <see cref="T:System.TimeSpan" />.</returns>
    public TimeSpan? Delta => this.delta;

    public RetryConditionHeaderValue(DateTimeOffset date) => this.date = new DateTimeOffset?(date);

    public RetryConditionHeaderValue(TimeSpan delta)
    {
      this.delta = delta.TotalSeconds <= (double) int.MaxValue ? new TimeSpan?(delta) : throw new System.Net.Http.ArgumentOutOfRangeException(nameof (delta));
    }

    private RetryConditionHeaderValue(RetryConditionHeaderValue source)
    {
      Contract.Requires(source != null);
      this.delta = source.delta;
      this.date = source.date;
    }

    private RetryConditionHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.delta.HasValue ? ((int) this.delta.Value.TotalSeconds).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) : HttpRuleParser.DateToString(this.date.Value);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is RetryConditionHeaderValue conditionHeaderValue))
        return false;
      return this.delta.HasValue ? conditionHeaderValue.delta.HasValue && this.delta.Value == conditionHeaderValue.delta.Value : conditionHeaderValue.date.HasValue && this.date.Value == conditionHeaderValue.date.Value;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return !this.delta.HasValue ? this.date.Value.GetHashCode() : this.delta.Value.GetHashCode();
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RetryConditionHeaderValue" />.</returns>
    public static RetryConditionHeaderValue Parse(string input)
    {
      int index = 0;
      return (RetryConditionHeaderValue) GenericHeaderParser.RetryConditionParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out RetryConditionHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (RetryConditionHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.RetryConditionParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (RetryConditionHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetRetryConditionLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex >= input.Length)
        return 0;
      int num1 = startIndex;
      DateTimeOffset result1 = DateTimeOffset.MinValue;
      int result2 = -1;
      int num2;
      switch (input[num1])
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          int startIndex1 = num1;
          int numberLength = HttpRuleParser.GetNumberLength(input, num1, false);
          if (numberLength == 0 || numberLength > 10)
            return 0;
          int startIndex2 = num1 + numberLength;
          num2 = startIndex2 + HttpRuleParser.GetWhitespaceLength(input, startIndex2);
          if (num2 != input.Length || !HeaderUtilities.TryParseInt32(input.Substring(startIndex1, numberLength), out result2))
            return 0;
          break;
        default:
          if (!HttpRuleParser.TryStringToDate(input.Substring(num1), out result1))
            return 0;
          num2 = input.Length;
          break;
      }
      RetryConditionHeaderValue conditionHeaderValue = new RetryConditionHeaderValue();
      if (result2 == -1)
        conditionHeaderValue.date = new DateTimeOffset?(result1);
      else
        conditionHeaderValue.delta = new TimeSpan?(new TimeSpan(0, 0, result2));
      parsedValue = (object) conditionHeaderValue;
      return num2 - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new RetryConditionHeaderValue(this);
  }
}
