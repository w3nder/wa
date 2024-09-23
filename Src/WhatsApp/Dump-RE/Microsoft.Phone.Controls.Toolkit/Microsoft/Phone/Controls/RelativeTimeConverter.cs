// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.RelativeTimeConverter
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
  public class RelativeTimeConverter : IValueConverter
  {
    private const double Minute = 60.0;
    private const double Hour = 3600.0;
    private const double Day = 86400.0;
    private const double Week = 604800.0;
    private const double Month = 2635200.0;
    private const double Year = 31536000.0;
    private const string DefaultCulture = "en-US";
    private string[] PluralHourStrings;
    private string[] PluralMinuteStrings;
    private string[] PluralSecondStrings;

    private void SetLocalizationCulture(CultureInfo culture)
    {
      if (!culture.Name.Equals("en-US", StringComparison.Ordinal))
        ControlResources.Culture = culture;
      this.PluralHourStrings = new string[4]
      {
        ControlResources.XHoursAgo_2To4,
        ControlResources.XHoursAgo_EndsIn1Not11,
        ControlResources.XHoursAgo_EndsIn2To4Not12To14,
        ControlResources.XHoursAgo_Other
      };
      this.PluralMinuteStrings = new string[4]
      {
        ControlResources.XMinutesAgo_2To4,
        ControlResources.XMinutesAgo_EndsIn1Not11,
        ControlResources.XMinutesAgo_EndsIn2To4Not12To14,
        ControlResources.XMinutesAgo_Other
      };
      this.PluralSecondStrings = new string[4]
      {
        ControlResources.XSecondsAgo_2To4,
        ControlResources.XSecondsAgo_EndsIn1Not11,
        ControlResources.XSecondsAgo_EndsIn2To4Not12To14,
        ControlResources.XSecondsAgo_Other
      };
    }

    private static string GetPluralMonth(int month)
    {
      if (month >= 2 && month <= 4)
        return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ControlResources.XMonthsAgo_2To4, (object) month.ToString((IFormatProvider) ControlResources.Culture));
      if (month < 5 || month > 12)
        throw new ArgumentException(Resources.InvalidNumberOfMonths);
      return string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ControlResources.XMonthsAgo_5To12, (object) month.ToString((IFormatProvider) ControlResources.Culture));
    }

    private static string GetPluralTimeUnits(int units, string[] resources)
    {
      int num1 = units % 10;
      int num2 = units % 100;
      if (units <= 1)
        throw new ArgumentException(Resources.InvalidNumberOfTimeUnits);
      return units >= 2 && units <= 4 ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, resources[0], (object) units.ToString((IFormatProvider) ControlResources.Culture)) : (num1 == 1 && num2 != 11 ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, resources[1], (object) units.ToString((IFormatProvider) ControlResources.Culture)) : (num1 >= 2 && num1 <= 4 && (num2 < 12 || num2 > 14) ? string.Format((IFormatProvider) CultureInfo.CurrentUICulture, resources[2], (object) units.ToString((IFormatProvider) ControlResources.Culture)) : string.Format((IFormatProvider) CultureInfo.CurrentUICulture, resources[3], (object) units.ToString((IFormatProvider) ControlResources.Culture))));
    }

    private static string GetLastDayOfWeek(DayOfWeek dow)
    {
      string lastDayOfWeek;
      switch (dow)
      {
        case DayOfWeek.Sunday:
          lastDayOfWeek = ControlResources.last_Sunday;
          break;
        case DayOfWeek.Monday:
          lastDayOfWeek = ControlResources.last_Monday;
          break;
        case DayOfWeek.Tuesday:
          lastDayOfWeek = ControlResources.last_Tuesday;
          break;
        case DayOfWeek.Wednesday:
          lastDayOfWeek = ControlResources.last_Wednesday;
          break;
        case DayOfWeek.Thursday:
          lastDayOfWeek = ControlResources.last_Thursday;
          break;
        case DayOfWeek.Friday:
          lastDayOfWeek = ControlResources.last_Friday;
          break;
        case DayOfWeek.Saturday:
          lastDayOfWeek = ControlResources.last_Saturday;
          break;
        default:
          lastDayOfWeek = ControlResources.last_Sunday;
          break;
      }
      return lastDayOfWeek;
    }

    private static string GetOnDayOfWeek(DayOfWeek dow)
    {
      string onDayOfWeek;
      switch (dow)
      {
        case DayOfWeek.Sunday:
          onDayOfWeek = ControlResources.on_Sunday;
          break;
        case DayOfWeek.Monday:
          onDayOfWeek = ControlResources.on_Monday;
          break;
        case DayOfWeek.Tuesday:
          onDayOfWeek = ControlResources.on_Tuesday;
          break;
        case DayOfWeek.Wednesday:
          onDayOfWeek = ControlResources.on_Wednesday;
          break;
        case DayOfWeek.Thursday:
          onDayOfWeek = ControlResources.on_Thursday;
          break;
        case DayOfWeek.Friday:
          onDayOfWeek = ControlResources.on_Friday;
          break;
        case DayOfWeek.Saturday:
          onDayOfWeek = ControlResources.on_Saturday;
          break;
        default:
          onDayOfWeek = ControlResources.on_Sunday;
          break;
      }
      return onDayOfWeek;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      DateTime given = value is DateTime dateTime ? dateTime.ToLocalTime() : throw new ArgumentException(Resources.InvalidDateTimeArgument);
      DateTime now = DateTime.Now;
      TimeSpan timeSpan = now - given;
      this.SetLocalizationCulture(culture);
      if (DateTimeFormatHelper.IsFutureDateTime(now, given))
        RelativeTimeConverter.GetPluralTimeUnits(2, this.PluralSecondStrings);
      string str;
      if (timeSpan.TotalSeconds > 31536000.0)
        str = ControlResources.OverAYearAgo;
      else if (timeSpan.TotalSeconds > 3952800.0)
        str = RelativeTimeConverter.GetPluralMonth((int) ((timeSpan.TotalSeconds + 1317600.0) / 2635200.0));
      else if (timeSpan.TotalSeconds >= 2116800.0)
        str = ControlResources.AboutAMonthAgo;
      else if (timeSpan.TotalSeconds >= 604800.0)
      {
        int num = (int) (timeSpan.TotalSeconds / 604800.0);
        if (num > 1)
          str = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ControlResources.XWeeksAgo_2To4, (object) num.ToString((IFormatProvider) ControlResources.Culture));
        else
          str = ControlResources.AboutAWeekAgo;
      }
      else
        str = timeSpan.TotalSeconds < 432000.0 ? (timeSpan.TotalSeconds < 86400.0 ? (timeSpan.TotalSeconds < 7200.0 ? (timeSpan.TotalSeconds < 3600.0 ? (timeSpan.TotalSeconds < 120.0 ? (timeSpan.TotalSeconds < 60.0 ? RelativeTimeConverter.GetPluralTimeUnits((double) (int) timeSpan.TotalSeconds > 1.0 ? (int) timeSpan.TotalSeconds : 2, this.PluralSecondStrings) : ControlResources.AboutAMinuteAgo) : RelativeTimeConverter.GetPluralTimeUnits((int) (timeSpan.TotalSeconds / 60.0), this.PluralMinuteStrings)) : ControlResources.AboutAnHourAgo) : RelativeTimeConverter.GetPluralTimeUnits((int) (timeSpan.TotalSeconds / 3600.0), this.PluralHourStrings)) : RelativeTimeConverter.GetOnDayOfWeek(given.DayOfWeek)) : RelativeTimeConverter.GetLastDayOfWeek(given.DayOfWeek);
      return (object) str;
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
