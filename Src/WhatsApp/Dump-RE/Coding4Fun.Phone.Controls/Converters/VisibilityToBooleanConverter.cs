// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Converters.VisibilityToBooleanConverter
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Coding4Fun.Phone.Controls.Converters
{
  public class VisibilityToBooleanConverter : IValueConverter
  {
    private static readonly BooleanToVisibilityConverter BoolToVis = new BooleanToVisibilityConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return VisibilityToBooleanConverter.BoolToVis.ConvertBack(value, targetType, parameter, culture);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return VisibilityToBooleanConverter.BoolToVis.Convert(value, targetType, parameter, culture);
    }
  }
}
