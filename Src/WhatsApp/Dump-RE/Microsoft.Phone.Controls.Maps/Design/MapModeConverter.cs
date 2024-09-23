// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Design.MapModeConverter
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
  public class MapModeConverter : TypeConverter
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
      if (value is string b)
      {
        foreach (PropertyInfo property in typeof (MapModes).GetProperties())
        {
          if (string.Equals(property.Name, b, StringComparison.OrdinalIgnoreCase) || string.Equals(property.PropertyType.FullName, b, StringComparison.OrdinalIgnoreCase))
            return property.GetValue((object) null, (object[]) null);
        }
        throw new FormatException(ExceptionStrings.TypeConverter_InvalidMapMode);
      }
      throw new NotSupportedException(ExceptionStrings.TypeConverter_InvalidMapMode);
    }
  }
}
