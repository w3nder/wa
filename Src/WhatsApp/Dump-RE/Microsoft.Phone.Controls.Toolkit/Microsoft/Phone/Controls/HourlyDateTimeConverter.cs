// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.HourlyDateTimeConverter
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
  public class HourlyDateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime dateTime))
        throw new ArgumentException(Resources.InvalidDateTimeArgument);
      StringBuilder stringBuilder = new StringBuilder(string.Empty);
      DateTime now = DateTime.Now;
      if (DateTimeFormatHelper.IsFutureDateTime(now, dateTime))
        throw new NotSupportedException(Resources.NonSupportedDateTime);
      if (DateTimeFormatHelper.IsAnOlderYear(now, dateTime))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1}", (object) DateTimeFormatHelper.GetShortDate(dateTime), (object) DateTimeFormatHelper.GetSuperShortTime(dateTime));
      else if (DateTimeFormatHelper.IsAnOlderWeek(now, dateTime))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1}", (object) DateTimeFormatHelper.GetMonthAndDay(dateTime), (object) DateTimeFormatHelper.GetSuperShortTime(dateTime));
      else if (DateTimeFormatHelper.IsPastDayOfWeekWithWindow(now, dateTime))
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentCulture, "{0}, {1}", (object) DateTimeFormatHelper.GetAbbreviatedDay(dateTime), (object) DateTimeFormatHelper.GetSuperShortTime(dateTime));
      else
        stringBuilder.Append(DateTimeFormatHelper.GetSuperShortTime(dateTime));
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
