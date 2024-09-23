// Decompiled with JetBrains decompiler
// Type: System.Windows.Controls.TimeTypeConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Properties;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace System.Windows.Controls
{
  public class TimeTypeConverter : TypeConverter
  {
    private static readonly string[] _timeFormats = new string[6]
    {
      "h:mm tt",
      "h:mm:ss tt",
      "HH:mm",
      "HH:mm:ss",
      "H:mm",
      "H:mm:ss"
    };
    private static readonly string[] _dateFormats = new string[1]
    {
      "M/d/yyyy"
    };

    public override bool CanConvertFrom(
      ITypeDescriptorContext typeDescriptorContext,
      Type sourceType)
    {
      return Type.GetTypeCode(sourceType) == TypeCode.String;
    }

    public override bool CanConvertTo(
      ITypeDescriptorContext typeDescriptorContext,
      Type destinationType)
    {
      return Type.GetTypeCode(destinationType) == TypeCode.String || TypeConverters.CanConvertTo<DateTime?>(destinationType);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext typeDescriptorContext,
      CultureInfo cultureInfo,
      object source)
    {
      if (source == null)
        return (object) null;
      if (!(source is string s))
        throw new InvalidCastException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypeConverters_Convert_CannotConvert, (object) this.GetType().Name, source, (object) typeof (DateTime).Name));
      if (string.IsNullOrEmpty(s))
        return (object) null;
      DateTime result;
      foreach (string timeFormat in TimeTypeConverter._timeFormats)
      {
        if (DateTime.TryParseExact(s, timeFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
          return (object) DateTime.Now.Date.Add(result.TimeOfDay);
      }
      foreach (string dateFormat in TimeTypeConverter._dateFormats)
      {
        foreach (string timeFormat in TimeTypeConverter._timeFormats)
        {
          if (DateTime.TryParseExact(s, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) dateFormat, (object) timeFormat), (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            return (object) result;
        }
      }
      foreach (string dateFormat in TimeTypeConverter._dateFormats)
      {
        if (DateTime.TryParseExact(s, dateFormat, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.NoCurrentDateDefault, out result))
          return (object) result;
      }
      throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypeConverters_Convert_CannotConvert, (object) this.GetType().Name, (object) s, (object) typeof (DateTime).Name));
    }

    public override object ConvertTo(
      ITypeDescriptorContext typeDescriptorContext,
      CultureInfo cultureInfo,
      object value,
      Type destinationType)
    {
      if (destinationType == typeof (string))
      {
        if (value == null)
          return (object) string.Empty;
        if (value is DateTime dateTime)
          return (object) dateTime.ToString("HH:mm:ss", (IFormatProvider) new CultureInfo("en-US"));
      }
      return TypeConverters.ConvertTo((TypeConverter) this, value, destinationType);
    }
  }
}
