// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Linq.JValue
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace Newtonsoft.Json.Linq
{
  /// <summary>
  /// Represents a value in JSON (string, integer, date, etc).
  /// </summary>
  public class JValue : 
    JToken,
    IEquatable<JValue>,
    IFormattable,
    IComparable,
    IComparable<JValue>,
    IConvertible
  {
    private JTokenType _valueType;
    private object _value;

    internal JValue(object value, JTokenType type)
    {
      this._value = value;
      this._valueType = type;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class from another <see cref="T:Newtonsoft.Json.Linq.JValue" /> object.
    /// </summary>
    /// <param name="other">A <see cref="T:Newtonsoft.Json.Linq.JValue" /> object to copy from.</param>
    public JValue(JValue other)
      : this(other.Value, other.Type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(long value)
      : this((object) value, JTokenType.Integer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(Decimal value)
      : this((object) value, JTokenType.Float)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(char value)
      : this((object) value, JTokenType.String)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    [CLSCompliant(false)]
    public JValue(ulong value)
      : this((object) value, JTokenType.Integer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(double value)
      : this((object) value, JTokenType.Float)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(float value)
      : this((object) value, JTokenType.Float)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(DateTime value)
      : this((object) value, JTokenType.Date)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(DateTimeOffset value)
      : this((object) value, JTokenType.Date)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(bool value)
      : this((object) value, JTokenType.Boolean)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(string value)
      : this((object) value, JTokenType.String)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(Guid value)
      : this((object) value, JTokenType.Guid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(Uri value)
      : this((object) value, value != (Uri) null ? JTokenType.Uri : JTokenType.Null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(TimeSpan value)
      : this((object) value, JTokenType.TimeSpan)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Linq.JValue" /> class with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    public JValue(object value)
      : this(value, JValue.GetValueType(new JTokenType?(), value))
    {
    }

    internal override bool DeepEquals(JToken node)
    {
      if (!(node is JValue v2))
        return false;
      return v2 == this || JValue.ValuesEquals(this, v2);
    }

    /// <summary>
    /// Gets a value indicating whether this token has child tokens.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this token has child values; otherwise, <c>false</c>.
    /// </value>
    public override bool HasValues => false;

    internal static int Compare(JTokenType valueType, object objA, object objB)
    {
      if (objA == null && objB == null)
        return 0;
      if (objA != null && objB == null)
        return 1;
      if (objA == null && objB != null)
        return -1;
      switch (valueType)
      {
        case JTokenType.Comment:
        case JTokenType.String:
        case JTokenType.Raw:
          return string.CompareOrdinal(Convert.ToString(objA, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToString(objB, (IFormatProvider) CultureInfo.InvariantCulture));
        case JTokenType.Integer:
          if (objA is ulong || objB is ulong || objA is Decimal || objB is Decimal)
            return Convert.ToDecimal(objA, (IFormatProvider) CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(objB, (IFormatProvider) CultureInfo.InvariantCulture));
          return objA is float || objB is float || objA is double || objB is double ? JValue.CompareFloat(objA, objB) : Convert.ToInt64(objA, (IFormatProvider) CultureInfo.InvariantCulture).CompareTo(Convert.ToInt64(objB, (IFormatProvider) CultureInfo.InvariantCulture));
        case JTokenType.Float:
          return JValue.CompareFloat(objA, objB);
        case JTokenType.Boolean:
          return Convert.ToBoolean(objA, (IFormatProvider) CultureInfo.InvariantCulture).CompareTo(Convert.ToBoolean(objB, (IFormatProvider) CultureInfo.InvariantCulture));
        case JTokenType.Date:
          if (objA is DateTime dateTime1)
          {
            DateTime dateTime = !(objB is DateTimeOffset dateTimeOffset) ? Convert.ToDateTime(objB, (IFormatProvider) CultureInfo.InvariantCulture) : dateTimeOffset.DateTime;
            return dateTime1.CompareTo(dateTime);
          }
          DateTimeOffset dateTimeOffset1 = (DateTimeOffset) objA;
          if (!(objB is DateTimeOffset other))
            other = new DateTimeOffset(Convert.ToDateTime(objB, (IFormatProvider) CultureInfo.InvariantCulture));
          return dateTimeOffset1.CompareTo(other);
        case JTokenType.Bytes:
          if (!(objB is byte[]))
            throw new ArgumentException("Object must be of type byte[].");
          byte[] a1 = objA as byte[];
          byte[] a2 = objB as byte[];
          if (a1 == null)
            return -1;
          return a2 == null ? 1 : MiscellaneousUtils.ByteArrayCompare(a1, a2);
        case JTokenType.Guid:
          return objB is Guid guid ? ((Guid) objA).CompareTo(guid) : throw new ArgumentException("Object must be of type Guid.");
        case JTokenType.Uri:
          if ((object) (objB as Uri) == null)
            throw new ArgumentException("Object must be of type Uri.");
          Uri uri1 = (Uri) objA;
          Uri uri2 = (Uri) objB;
          return Comparer<string>.Default.Compare(uri1.ToString(), uri2.ToString());
        case JTokenType.TimeSpan:
          return objB is TimeSpan timeSpan ? ((TimeSpan) objA).CompareTo(timeSpan) : throw new ArgumentException("Object must be of type TimeSpan.");
        default:
          throw MiscellaneousUtils.CreateArgumentOutOfRangeException(nameof (valueType), (object) valueType, "Unexpected value type: {0}".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) valueType));
      }
    }

    private static int CompareFloat(object objA, object objB)
    {
      double d1 = Convert.ToDouble(objA, (IFormatProvider) CultureInfo.InvariantCulture);
      double d2 = Convert.ToDouble(objB, (IFormatProvider) CultureInfo.InvariantCulture);
      return MathUtils.ApproxEquals(d1, d2) ? 0 : d1.CompareTo(d2);
    }

    internal override JToken CloneToken() => (JToken) new JValue(this);

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> comment with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> comment with the given value.</returns>
    public static JValue CreateComment(string value)
    {
      return new JValue((object) value, JTokenType.Comment);
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> string with the given value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> string with the given value.</returns>
    public static JValue CreateString(string value)
    {
      return new JValue((object) value, JTokenType.String);
    }

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.
    /// </summary>
    /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.</returns>
    public static JValue CreateNull() => new JValue((object) null, JTokenType.Null);

    /// <summary>
    /// Creates a <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.
    /// </summary>
    /// <returns>A <see cref="T:Newtonsoft.Json.Linq.JValue" /> null value.</returns>
    public static JValue CreateUndefined() => new JValue((object) null, JTokenType.Undefined);

    private static JTokenType GetValueType(JTokenType? current, object value)
    {
      switch (value)
      {
        case null:
          return JTokenType.Null;
        case string _:
          return JValue.GetStringValueType(current);
        case long _:
        case int _:
        case short _:
        case sbyte _:
        case ulong _:
        case uint _:
        case ushort _:
        case byte _:
          return JTokenType.Integer;
        case Enum _:
          return JTokenType.Integer;
        case double _:
        case float _:
        case Decimal _:
          return JTokenType.Float;
        case DateTime _:
          return JTokenType.Date;
        case DateTimeOffset _:
          return JTokenType.Date;
        case byte[] _:
          return JTokenType.Bytes;
        case bool _:
          return JTokenType.Boolean;
        case Guid _:
          return JTokenType.Guid;
        default:
          if ((object) (value as Uri) != null)
            return JTokenType.Uri;
          if (value is TimeSpan)
            return JTokenType.TimeSpan;
          throw new ArgumentException("Could not determine JSON object type for type {0}.".FormatWith((IFormatProvider) CultureInfo.InvariantCulture, (object) value.GetType()));
      }
    }

    private static JTokenType GetStringValueType(JTokenType? current)
    {
      if (!current.HasValue)
        return JTokenType.String;
      switch (current.Value)
      {
        case JTokenType.Comment:
        case JTokenType.String:
        case JTokenType.Raw:
          return current.Value;
        default:
          return JTokenType.String;
      }
    }

    /// <summary>
    /// Gets the node type for this <see cref="T:Newtonsoft.Json.Linq.JToken" />.
    /// </summary>
    /// <value>The type.</value>
    public override JTokenType Type => this._valueType;

    /// <summary>Gets or sets the underlying token value.</summary>
    /// <value>The underlying token value.</value>
    public object Value
    {
      get => this._value;
      set
      {
        if ((this._value != null ? this._value.GetType() : (System.Type) null) != value?.GetType())
          this._valueType = JValue.GetValueType(new JTokenType?(this._valueType), value);
        this._value = value;
      }
    }

    /// <summary>
    /// Writes this token to a <see cref="T:Newtonsoft.Json.JsonWriter" />.
    /// </summary>
    /// <param name="writer">A <see cref="T:Newtonsoft.Json.JsonWriter" /> into which this method will write.</param>
    /// <param name="converters">A collection of <see cref="T:Newtonsoft.Json.JsonConverter" /> which will be used when writing the token.</param>
    public override void WriteTo(JsonWriter writer, params JsonConverter[] converters)
    {
      if (converters != null && converters.Length > 0 && this._value != null)
      {
        JsonConverter matchingConverter = JsonSerializer.GetMatchingConverter((IList<JsonConverter>) converters, this._value.GetType());
        if (matchingConverter != null && matchingConverter.CanWrite)
        {
          matchingConverter.WriteJson(writer, this._value, JsonSerializer.CreateDefault());
          return;
        }
      }
      switch (this._valueType)
      {
        case JTokenType.Comment:
          writer.WriteComment(this._value != null ? this._value.ToString() : (string) null);
          break;
        case JTokenType.Integer:
          if (this._value is int)
          {
            writer.WriteValue((int) this._value);
            break;
          }
          if (this._value is long)
          {
            writer.WriteValue((long) this._value);
            break;
          }
          if (this._value is ulong)
          {
            writer.WriteValue((ulong) this._value);
            break;
          }
          writer.WriteValue(Convert.ToInt64(this._value, (IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case JTokenType.Float:
          if (this._value is Decimal)
          {
            writer.WriteValue((Decimal) this._value);
            break;
          }
          if (this._value is double)
          {
            writer.WriteValue((double) this._value);
            break;
          }
          if (this._value is float)
          {
            writer.WriteValue((float) this._value);
            break;
          }
          writer.WriteValue(Convert.ToDouble(this._value, (IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case JTokenType.String:
          writer.WriteValue(this._value != null ? this._value.ToString() : (string) null);
          break;
        case JTokenType.Boolean:
          writer.WriteValue(Convert.ToBoolean(this._value, (IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case JTokenType.Null:
          writer.WriteNull();
          break;
        case JTokenType.Undefined:
          writer.WriteUndefined();
          break;
        case JTokenType.Date:
          if (this._value is DateTimeOffset)
          {
            writer.WriteValue((DateTimeOffset) this._value);
            break;
          }
          writer.WriteValue(Convert.ToDateTime(this._value, (IFormatProvider) CultureInfo.InvariantCulture));
          break;
        case JTokenType.Raw:
          writer.WriteRawValue(this._value != null ? this._value.ToString() : (string) null);
          break;
        case JTokenType.Bytes:
          writer.WriteValue((byte[]) this._value);
          break;
        case JTokenType.Guid:
        case JTokenType.Uri:
        case JTokenType.TimeSpan:
          writer.WriteValue(this._value != null ? this._value.ToString() : (string) null);
          break;
        default:
          throw MiscellaneousUtils.CreateArgumentOutOfRangeException("TokenType", (object) this._valueType, "Unexpected token type.");
      }
    }

    internal override int GetDeepHashCode()
    {
      int hashCode = this._value != null ? this._value.GetHashCode() : 0;
      return ((int) this._valueType).GetHashCode() ^ hashCode;
    }

    private static bool ValuesEquals(JValue v1, JValue v2)
    {
      if (v1 == v2)
        return true;
      return v1._valueType == v2._valueType && JValue.Compare(v1._valueType, v1._value, v2._value) == 0;
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(JValue other) => other != null && JValue.ValuesEquals(this, other);

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
    /// </summary>
    /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
    /// <returns>
    /// true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.
    /// </returns>
    /// <exception cref="T:System.NullReferenceException">
    /// The <paramref name="obj" /> parameter is null.
    /// </exception>
    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      return obj is JValue other ? this.Equals(other) : base.Equals(obj);
    }

    /// <summary>Serves as a hash function for a particular type.</summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Object" />.
    /// </returns>
    public override int GetHashCode() => this._value == null ? 0 : this._value.GetHashCode();

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this._value == null ? string.Empty : this._value.ToString();
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public string ToString(string format)
    {
      return this.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
      return this.ToString((string) null, formatProvider);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
      if (this._value == null)
        return string.Empty;
      return this._value is IFormattable formattable ? formattable.ToString(format, formatProvider) : this._value.ToString();
    }

    int IComparable.CompareTo(object obj)
    {
      return obj == null ? 1 : JValue.Compare(this._valueType, this._value, obj is JValue ? ((JValue) obj).Value : obj);
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
    /// Value
    /// Meaning
    /// Less than zero
    /// This instance is less than <paramref name="obj" />.
    /// Zero
    /// This instance is equal to <paramref name="obj" />.
    /// Greater than zero
    /// This instance is greater than <paramref name="obj" />.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="obj" /> is not the same type as this instance.
    /// </exception>
    public int CompareTo(JValue obj)
    {
      return obj == null ? 1 : JValue.Compare(this._valueType, this._value, obj._value);
    }

    TypeCode IConvertible.GetTypeCode()
    {
      if (this._value == null)
        return TypeCode.Empty;
      return this._value is DateTimeOffset ? TypeCode.DateTime : System.Type.GetTypeCode(this._value.GetType());
    }

    bool IConvertible.ToBoolean(IFormatProvider provider) => (bool) (JToken) this;

    char IConvertible.ToChar(IFormatProvider provider) => (char) (JToken) this;

    sbyte IConvertible.ToSByte(IFormatProvider provider) => (sbyte) (JToken) this;

    byte IConvertible.ToByte(IFormatProvider provider) => (byte) (JToken) this;

    short IConvertible.ToInt16(IFormatProvider provider) => (short) (JToken) this;

    ushort IConvertible.ToUInt16(IFormatProvider provider) => (ushort) (JToken) this;

    int IConvertible.ToInt32(IFormatProvider provider) => (int) (JToken) this;

    uint IConvertible.ToUInt32(IFormatProvider provider) => (uint) (JToken) this;

    long IConvertible.ToInt64(IFormatProvider provider) => (long) (JToken) this;

    ulong IConvertible.ToUInt64(IFormatProvider provider) => (ulong) (JToken) this;

    float IConvertible.ToSingle(IFormatProvider provider) => (float) (JToken) this;

    double IConvertible.ToDouble(IFormatProvider provider) => (double) (JToken) this;

    Decimal IConvertible.ToDecimal(IFormatProvider provider) => (Decimal) (JToken) this;

    DateTime IConvertible.ToDateTime(IFormatProvider provider) => (DateTime) (JToken) this;

    object IConvertible.ToType(System.Type conversionType, IFormatProvider provider)
    {
      return this.ToObject(conversionType);
    }
  }
}
