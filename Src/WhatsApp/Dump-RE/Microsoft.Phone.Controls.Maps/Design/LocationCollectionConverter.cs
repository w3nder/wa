// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.LocationCollectionConverter
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
  public class LocationCollectionConverter : TypeConverter
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
      if (!(value is string str1))
        throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidLocationCollection);
      LocationCollection locationCollection = new LocationCollection();
      LocationConverter locationConverter = new LocationConverter();
      int num = -1;
      for (int index = 0; index < str1.Length + 1; ++index)
      {
        if (index >= str1.Length || char.IsWhiteSpace(str1[index]))
        {
          int startIndex = num + 1;
          int length = index - startIndex;
          if (length >= 1)
          {
            string str2 = str1.Substring(startIndex, length);
            locationCollection.Add((GeoCoordinate) locationConverter.ConvertFrom((object) str2));
          }
          num = index;
        }
      }
      return (object) locationCollection;
    }
  }
}
