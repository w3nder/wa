// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.ThreadDateTimeConverter
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.Properties;
using System;
using System.Globalization;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class ThreadDateTimeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime dateTime))
        throw new ArgumentException(Resources.InvalidDateTimeArgument);
      DateTime now = DateTime.Now;
      if (DateTimeFormatHelper.IsFutureDateTime(now, dateTime))
        throw new NotSupportedException(Resources.NonSupportedDateTime);
      return !DateTimeFormatHelper.IsAnOlderWeek(now, dateTime) ? (!DateTimeFormatHelper.IsPastDayOfWeekWithWindow(now, dateTime) ? (object) DateTimeFormatHelper.GetSuperShortTime(dateTime) : (object) DateTimeFormatHelper.GetAbbreviatedDay(dateTime)) : (object) DateTimeFormatHelper.GetMonthAndDay(dateTime);
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
