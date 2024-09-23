// Decompiled with JetBrains decompiler
// Type: System.Net.Http.Headers.RangeConditionHeaderValue
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;

#nullable disable
namespace System.Net.Http.Headers
{
  /// <summary>Represents a header value which can either be a date/time or an entity-tag value.</summary>
  public class RangeConditionHeaderValue : ICloneable
  {
    private DateTimeOffset? date;
    private EntityTagHeaderValue entityTag;

    /// <returns>Returns <see cref="T:System.DateTimeOffset" />.</returns>
    public DateTimeOffset? Date => this.date;

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.EntityTagHeaderValue" />.</returns>
    public EntityTagHeaderValue EntityTag => this.entityTag;

    public RangeConditionHeaderValue(DateTimeOffset date) => this.date = new DateTimeOffset?(date);

    public RangeConditionHeaderValue(EntityTagHeaderValue entityTag)
    {
      this.entityTag = entityTag != null ? entityTag : throw new ArgumentNullException(nameof (entityTag));
    }

    public RangeConditionHeaderValue(string entityTag)
      : this(new EntityTagHeaderValue(entityTag))
    {
    }

    private RangeConditionHeaderValue(RangeConditionHeaderValue source)
    {
      Contract.Requires(source != null);
      this.entityTag = source.entityTag;
      this.date = source.date;
    }

    private RangeConditionHeaderValue()
    {
    }

    /// <returns>Returns <see cref="T:System.String" />.</returns>
    public override string ToString()
    {
      return this.entityTag == null ? HttpRuleParser.DateToString(this.date.Value) : this.entityTag.ToString();
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is RangeConditionHeaderValue conditionHeaderValue))
        return false;
      if (this.entityTag != null)
        return this.entityTag.Equals((object) conditionHeaderValue.entityTag);
      return conditionHeaderValue.date.HasValue && this.date.Value == conditionHeaderValue.date.Value;
    }

    /// <returns>Returns <see cref="T:System.Int32" />.</returns>
    public override int GetHashCode()
    {
      return this.entityTag == null ? this.date.Value.GetHashCode() : this.entityTag.GetHashCode();
    }

    /// <returns>Returns <see cref="T:System.Net.Http.Headers.RangeConditionHeaderValue" />.</returns>
    public static RangeConditionHeaderValue Parse(string input)
    {
      int index = 0;
      return (RangeConditionHeaderValue) GenericHeaderParser.RangeConditionParser.ParseValue(input, (object) null, ref index);
    }

    /// <returns>Returns <see cref="T:System.Boolean" />.</returns>
    public static bool TryParse(string input, out RangeConditionHeaderValue parsedValue)
    {
      int index = 0;
      parsedValue = (RangeConditionHeaderValue) null;
      object parsedValue1;
      if (!GenericHeaderParser.RangeConditionParser.TryParseValue(input, (object) null, ref index, out parsedValue1))
        return false;
      parsedValue = (RangeConditionHeaderValue) parsedValue1;
      return true;
    }

    internal static int GetRangeConditionLength(
      string input,
      int startIndex,
      out object parsedValue)
    {
      Contract.Requires(startIndex >= 0);
      parsedValue = (object) null;
      if (string.IsNullOrEmpty(input) || startIndex + 1 >= input.Length)
        return 0;
      int num1 = startIndex;
      DateTimeOffset result = DateTimeOffset.MinValue;
      EntityTagHeaderValue parsedValue1 = (EntityTagHeaderValue) null;
      char ch1 = input[num1];
      char ch2 = input[num1 + 1];
      int num2;
      switch (ch1)
      {
        case '"':
          int entityTagLength = EntityTagHeaderValue.GetEntityTagLength(input, num1, out parsedValue1);
          if (entityTagLength == 0)
            return 0;
          num2 = num1 + entityTagLength;
          if (num2 != input.Length)
            return 0;
          break;
        case 'W':
        case 'w':
          if (ch2 != '/')
            goto default;
          else
            goto case '"';
        default:
          if (!HttpRuleParser.TryStringToDate(input.Substring(num1), out result))
            return 0;
          num2 = input.Length;
          break;
      }
      RangeConditionHeaderValue conditionHeaderValue = new RangeConditionHeaderValue();
      if (parsedValue1 == null)
        conditionHeaderValue.date = new DateTimeOffset?(result);
      else
        conditionHeaderValue.entityTag = parsedValue1;
      parsedValue = (object) conditionHeaderValue;
      return num2 - startIndex;
    }

    /// <returns>Returns <see cref="T:System.Object" />.</returns>
    object ICloneable.Clone() => (object) new RangeConditionHeaderValue(this);
  }
}
