// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.LocationRectConverter
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Design
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class LocationRectConverter : TypeConverter
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
      if (!(value is string str))
        throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationRectFormat);
      char[] chArray = new char[1]{ ',' };
      string[] strArray = str.Split(chArray);
      return strArray.Length == 4 ? (object) new LocationRect(double.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), double.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture)) : throw new FormatException(ExceptionStrings.TypeConverter_InvalidLocationRectFormat);
    }
  }
}
