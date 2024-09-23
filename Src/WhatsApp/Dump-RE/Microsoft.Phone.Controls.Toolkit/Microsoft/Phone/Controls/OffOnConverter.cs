// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.OffOnConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.LocalizedResources;
using Microsoft.Phone.Controls.Properties;
using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class OffOnConverter : IValueConverter
  {
    public static string On = ControlResources.On;
    public static string Off = ControlResources.Off;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (targetType == null)
        throw new ArgumentNullException(nameof (targetType));
      if (targetType != typeof (object))
        throw new ArgumentException(Resources.UnexpectedType, nameof (targetType));
      switch (value)
      {
        case bool? _:
        case null:
          bool? nullable = (bool?) value;
          return (!nullable.GetValueOrDefault() ? 0 : (nullable.HasValue ? 1 : 0)) == 0 ? (object) OffOnConverter.Off : (object) OffOnConverter.On;
        default:
          throw new ArgumentException(Resources.UnexpectedType, nameof (value));
      }
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
