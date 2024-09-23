// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.FullViewDateTimeConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Properties;
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class FullViewDateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime dt))
        throw new ArgumentException(Resources.InvalidDateTimeArgument);
      StringBuilder stringBuilder = new StringBuilder(string.Empty);
      if (DateTimeFormatHelper.IsCurrentCultureJapanese() || DateTimeFormatHelper.IsCurrentCultureKorean())
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1} {2}", (object) DateTimeFormatHelper.GetMonthAndDay(dt), (object) DateTimeFormatHelper.GetAbbreviatedDay(dt), (object) DateTimeFormatHelper.GetShortTime(dt));
      else if (DateTimeFormatHelper.IsCurrentCultureTurkish())
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1} {2}", (object) DateTimeFormatHelper.GetMonthAndDay(dt), (object) DateTimeFormatHelper.GetAbbreviatedDay(dt), (object) DateTimeFormatHelper.GetShortTime(dt));
      else
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}, {2}", (object) DateTimeFormatHelper.GetAbbreviatedDay(dt), (object) DateTimeFormatHelper.GetMonthAndDay(dt), (object) DateTimeFormatHelper.GetShortTime(dt));
      if (DateTimeFormatHelper.IsCurrentUICultureFrench())
        stringBuilder.Replace(",", string.Empty);
      return (object) stringBuilder.ToString();
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
