// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.LocationConverter
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Design
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class LocationConverter : TypeConverter
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
      if (value is string str)
      {
        char[] chArray = new char[1]{ ',' };
        string[] strArray = str.Split(chArray);
        switch (strArray.Length)
        {
          case 2:
            double result1;
            double result2;
            if (double.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1) && double.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
              return (object) new GeoCoordinate()
              {
                Latitude = result1,
                Longitude = result2
              };
            break;
          case 3:
            double result3;
            double result4;
            double result5;
            if (double.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3) && double.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result4) && double.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result5))
              return (object) new GeoCoordinate()
              {
                Latitude = result3,
                Longitude = result4,
                Altitude = result5
              };
            break;
          case 4:
            double result6;
            double result7;
            double result8;
            if (double.TryParse(strArray[0], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result6) && double.TryParse(strArray[1], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result7) && double.TryParse(strArray[2], NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result8))
              return (object) new GeoCoordinate()
              {
                Latitude = result6,
                Longitude = result7,
                Altitude = result8
              };
            break;
        }
        throw new FormatException(ExceptionStrings.TypeConverter_InvalidLocationFormat);
      }
      throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationFormat);
    }
  }
}
