// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.PositionOriginConverter
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Design
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class PositionOriginConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return sourceType == typeof (string);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      string[] strArray = value is string name ? name.Split(',') : throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidPositionOriginFormat);
      if (strArray.Length == 2)
        return (object) new PositionOrigin(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
      return (typeof (PositionOrigin).GetField(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static) ?? throw new FormatException(ExceptionStrings.TypeConverter_InvalidPositionOriginFormat)).GetValue((object) null);
    }
  }
}
