// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.LengthConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class LengthConverter : TypeConverter
  {
    private static Dictionary<string, double> UnitToPixelConversions = new Dictionary<string, double>()
    {
      {
        "px",
        1.0
      },
      {
        "in",
        96.0
      },
      {
        "cm",
        4800.0 / (double) sbyte.MaxValue
      },
      {
        "pt",
        4.0 / 3.0
      }
    };

    public override bool CanConvertFrom(
      ITypeDescriptorContext typeDescriptorContext,
      Type sourceType)
    {
      switch (Type.GetTypeCode(sourceType))
      {
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
        case TypeCode.String:
          return true;
        default:
          return false;
      }
    }

    public override object ConvertFrom(
      ITypeDescriptorContext typeDescriptorContext,
      CultureInfo cultureInfo,
      object source)
    {
      if (source == null)
        throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypeConverters_ConvertFrom_CannotConvertFromType, (object) this.GetType().Name, (object) "null"));
      if (!(source is string strA))
        return (object) Convert.ToDouble(source, (IFormatProvider) cultureInfo);
      if (string.Compare(strA, "Auto", StringComparison.OrdinalIgnoreCase) == 0)
        return (object) double.NaN;
      string str = strA;
      double num = 1.0;
      foreach (KeyValuePair<string, double> toPixelConversion in LengthConverter.UnitToPixelConversions)
      {
        if (str.EndsWith(toPixelConversion.Key, StringComparison.Ordinal))
        {
          num = toPixelConversion.Value;
          str = strA.Substring(0, str.Length - toPixelConversion.Key.Length);
          break;
        }
      }
      try
      {
        return (object) (num * Convert.ToDouble(str, (IFormatProvider) cultureInfo));
      }
      catch (FormatException ex)
      {
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.TypeConverters_Convert_CannotConvert, (object) this.GetType().Name, (object) strA, (object) typeof (double).Name));
      }
    }

    public override bool CanConvertTo(
      ITypeDescriptorContext typeDescriptorContext,
      Type destinationType)
    {
      return TypeConverters.CanConvertTo<double>(destinationType);
    }

    public override object ConvertTo(
      ITypeDescriptorContext typeDescriptorContext,
      CultureInfo cultureInfo,
      object value,
      Type destinationType)
    {
      if (!(value is double num) || destinationType != typeof (string))
        return TypeConverters.ConvertTo((TypeConverter) this, value, destinationType);
      return !num.IsNaN() ? (object) Convert.ToString(num, (IFormatProvider) cultureInfo) : (object) "Auto";
    }
  }
}
