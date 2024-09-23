// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.TileSizeToHeightConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class TileSizeToHeightConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double num = 0.0;
      switch ((TileSize) value)
      {
        case TileSize.Default:
          num = 173.0;
          break;
        case TileSize.Small:
          num = 99.0;
          break;
        case TileSize.Medium:
          num = 210.0;
          break;
        case TileSize.Large:
          num = 210.0;
          break;
      }
      double result;
      if (parameter == null || !double.TryParse(parameter.ToString(), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = 1.0;
      return (object) (num * result);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
