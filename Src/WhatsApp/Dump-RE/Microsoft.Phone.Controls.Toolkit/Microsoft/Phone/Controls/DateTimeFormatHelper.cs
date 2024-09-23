// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.DateTimeFormatHelper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class DateTimeFormatHelper
  {
    private const double Hour = 60.0;
    private const double Day = 1440.0;
    private const string SingleMeridiemDesignator = "t";
    private const string DoubleMeridiemDesignator = "tt";
    private static DateTimeFormatInfo formatInfo_GetSuperShortTime = (DateTimeFormatInfo) null;
    private static DateTimeFormatInfo formatInfo_GetMonthAndDay = (DateTimeFormatInfo) null;
    private static DateTimeFormatInfo formatInfo_GetShortTime = (DateTimeFormatInfo) null;
    private static object lock_GetSuperShortTime = new object();
    private static object lock_GetMonthAndDay = new object();
    private static object lock_GetShortTime = new object();
    private static readonly Regex rxMonthAndDay = new Regex("(d{1,2}[^A-Za-z]M{1,3})|(M{1,3}[^A-Za-z]d{1,2})");
    private static readonly Regex rxSeconds = new Regex("([^A-Za-z]s{1,2})");

    public static int GetRelativeDayOfWeek(DateTime dt)
    {
      return (dt.DayOfWeek - CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek + 7) % 7;
    }

    public static bool IsFutureDateTime(DateTime relative, DateTime given) => relative < given;

    public static bool IsAnOlderYear(DateTime relative, DateTime given)
    {
      return relative.Year > given.Year;
    }

    public static bool IsAnOlderWeek(DateTime relative, DateTime given)
    {
      return DateTimeFormatHelper.IsAtLeastOneWeekOld(relative, given) || DateTimeFormatHelper.GetRelativeDayOfWeek(given) > DateTimeFormatHelper.GetRelativeDayOfWeek(relative);
    }

    public static bool IsAtLeastOneWeekOld(DateTime relative, DateTime given)
    {
      return (double) (int) (relative - given).TotalMinutes >= 10080.0;
    }

    public static bool IsPastDayOfWeek(DateTime relative, DateTime given)
    {
      return DateTimeFormatHelper.GetRelativeDayOfWeek(relative) > DateTimeFormatHelper.GetRelativeDayOfWeek(given);
    }

    public static bool IsPastDayOfWeekWithWindow(DateTime relative, DateTime given)
    {
      return DateTimeFormatHelper.IsPastDayOfWeek(relative, given) && (double) (int) (relative - given).TotalMinutes > 180.0;
    }

    public static bool IsCurrentCultureJapanese()
    {
      return CultureInfo.CurrentCulture.Name.StartsWith("ja", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCurrentCultureKorean()
    {
      return CultureInfo.CurrentCulture.Name.StartsWith("ko", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCurrentCultureTurkish()
    {
      return CultureInfo.CurrentCulture.Name.StartsWith("tr", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCurrentCultureHungarian()
    {
      return CultureInfo.CurrentCulture.Name.StartsWith("hu", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCurrentUICultureFrench()
    {
      return CultureInfo.CurrentUICulture.Name.Equals("fr-FR", StringComparison.Ordinal);
    }

    public static string GetAbbreviatedDay(DateTime dt)
    {
      return DateTimeFormatHelper.IsCurrentCultureJapanese() || DateTimeFormatHelper.IsCurrentCultureKorean() ? "(" + dt.ToString("ddd", (IFormatProvider) CultureInfo.CurrentCulture) + ")" : dt.ToString("ddd", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public static string GetSuperShortTime(DateTime dt)
    {
      if (DateTimeFormatHelper.formatInfo_GetSuperShortTime == null)
      {
        lock (DateTimeFormatHelper.lock_GetSuperShortTime)
        {
          StringBuilder stringBuilder = new StringBuilder(string.Empty);
          DateTimeFormatHelper.formatInfo_GetSuperShortTime = (DateTimeFormatInfo) CultureInfo.CurrentCulture.DateTimeFormat.Clone();
          stringBuilder.Append(DateTimeFormatHelper.formatInfo_GetSuperShortTime.LongTimePattern);
          string oldValue = DateTimeFormatHelper.rxSeconds.Match(stringBuilder.ToString()).Value;
          stringBuilder.Replace(" ", string.Empty);
          stringBuilder.Replace(oldValue, string.Empty);
          if (!DateTimeFormatHelper.IsCurrentCultureJapanese() && !DateTimeFormatHelper.IsCurrentCultureKorean() && !DateTimeFormatHelper.IsCurrentCultureHungarian())
            stringBuilder.Replace("tt", "t");
          DateTimeFormatHelper.formatInfo_GetSuperShortTime.ShortTimePattern = stringBuilder.ToString();
        }
      }
      return dt.ToString("t", (IFormatProvider) DateTimeFormatHelper.formatInfo_GetSuperShortTime).ToLowerInvariant();
    }

    public static string GetMonthAndDay(DateTime dt)
    {
      if (DateTimeFormatHelper.formatInfo_GetMonthAndDay == null)
      {
        lock (DateTimeFormatHelper.lock_GetMonthAndDay)
        {
          StringBuilder stringBuilder = new StringBuilder(string.Empty);
          DateTimeFormatHelper.formatInfo_GetMonthAndDay = (DateTimeFormatInfo) CultureInfo.CurrentCulture.DateTimeFormat.Clone();
          stringBuilder.Append(DateTimeFormatHelper.rxMonthAndDay.Match(DateTimeFormatHelper.formatInfo_GetMonthAndDay.ShortDatePattern).Value);
          if (stringBuilder.ToString().Contains("."))
            stringBuilder.Append(".");
          DateTimeFormatHelper.formatInfo_GetMonthAndDay.ShortDatePattern = stringBuilder.ToString();
        }
      }
      return dt.ToString("d", (IFormatProvider) DateTimeFormatHelper.formatInfo_GetMonthAndDay);
    }

    public static string GetShortDate(DateTime dt)
    {
      return dt.ToString("d", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public static string GetShortTime(DateTime dt)
    {
      if (DateTimeFormatHelper.formatInfo_GetShortTime == null)
      {
        lock (DateTimeFormatHelper.lock_GetShortTime)
        {
          StringBuilder stringBuilder = new StringBuilder(string.Empty);
          DateTimeFormatHelper.formatInfo_GetShortTime = (DateTimeFormatInfo) CultureInfo.CurrentCulture.DateTimeFormat.Clone();
          stringBuilder.Append(DateTimeFormatHelper.formatInfo_GetShortTime.LongTimePattern);
          string oldValue = DateTimeFormatHelper.rxSeconds.Match(stringBuilder.ToString()).Value;
          stringBuilder.Replace(oldValue, string.Empty);
          DateTimeFormatHelper.formatInfo_GetShortTime.ShortTimePattern = stringBuilder.ToString();
        }
      }
      return dt.ToString("t", (IFormatProvider) DateTimeFormatHelper.formatInfo_GetShortTime);
    }
  }
}
