// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Converters.BooleanToVisibilityConverter
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace Coding4Fun.Phone.Controls.Converters
{
  public class BooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag = System.Convert.ToBoolean(value);
      if (parameter != null)
        flag = !flag;
      return (object) (Visibility) (flag ? 0 : 1);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return (object) value.Equals((object) (Visibility) 0);
    }
  }
}
