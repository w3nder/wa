// Decompiled with JetBrains decompiler
// Type: WhatsApp.DateTimeUtils
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Globalization;
using System.Text;
using WhatsApp.RegularExpressions;


namespace WhatsApp
{
  public static class DateTimeUtils
  {
    private static string fullDateFormat;
    private static string compactDateFormat;
    private static string compactTimeFormat;

    public static int GetDateTimeGroupingKey(DateTime? dt, DateTime dtRef)
    {
      if (!dt.HasValue)
        return 0;
      TimeSpan timeSpan = dtRef - dt.Value;
      if (timeSpan < Constants.TwoDays)
        return 1;
      if (timeSpan < Constants.OneWeek)
        return 2;
      bool flag = dt.Value.Month == dtRef.Month;
      if (flag && timeSpan < Constants.ThirtyDays)
        return 3;
      return !flag && timeSpan < Constants.OneYear ? 100 + dt.Value.Month : dt.Value.Year;
    }

    public static string GetDateTimeGroupingTitle(int groupingKeyVal)
    {
      string str;
      switch (groupingKeyVal)
      {
        case 0:
          str = AppResources.GroupingMissingDates;
          break;
        case 1:
          str = AppResources.GroupingRecent;
          break;
        case 2:
          str = Plurals.Instance.GetString(AppResources.GroupingLastNDaysPlural, 7);
          break;
        case 3:
          str = Plurals.Instance.GetString(AppResources.GroupingLastNDaysPlural, 30);
          break;
        default:
          str = groupingKeyVal <= 100 || groupingKeyVal > 112 ? groupingKeyVal.ToString() : new DateTime(2016, groupingKeyVal - 100, 1).ToString("MMMM");
          break;
      }
      return str ?? AppResources.GroupingMissingDates;
    }

    public static long ToUnixTime(this DateTime dt)
    {
      if (dt.Kind == DateTimeKind.Local)
        dt = dt.ToUniversalTime();
      return (long) (dt - FunXMPP.UnixEpoch).TotalSeconds;
    }

    public static DateTime FromUnixTime(long seconds)
    {
      return FunXMPP.UnixEpoch.AddSeconds((double) seconds);
    }

    public static DateTime? NullableFromUnixTime(long seconds)
    {
      return seconds != 0L ? new DateTime?(DateTimeUtils.FromUnixTime(seconds)) : new DateTime?();
    }

    public static DateTime FunTimeToPhoneTime(DateTime funTime)
    {
      DateTime localTime = funTime.ToLocalTime();
      int lastKnownTimeSkew = FunRunner.LastKnownTimeSkew;
      return lastKnownTimeSkew != 0 ? localTime + TimeSpan.FromSeconds((double) lastKnownTimeSkew) : localTime;
    }

    public static DateTime PhoneTimeToFunTime(DateTime phoneTime)
    {
      DateTime universalTime = phoneTime.ToUniversalTime();
      int lastKnownTimeSkew = FunRunner.LastKnownTimeSkew;
      return lastKnownTimeSkew != 0 ? universalTime - TimeSpan.FromSeconds((double) lastKnownTimeSkew) : universalTime;
    }

    public static long SanitizeTimestamp(long timestamp) => timestamp & (long) uint.MaxValue;

    private static string FullDateFormat
    {
      get
      {
        if (DateTimeUtils.fullDateFormat == null)
          DateTimeUtils.fullDateFormat = CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern;
        return DateTimeUtils.fullDateFormat;
      }
    }

    private static string CompactDateFormat
    {
      get
      {
        if (DateTimeUtils.compactDateFormat == null)
          DateTimeUtils.compactDateFormat = new Regex("m.+d", RegexOptions.IgnoreCase).Match(CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern).Success ? "M/d" : "d.M";
        return DateTimeUtils.compactDateFormat;
      }
    }

    private static string CompactTimeFormat
    {
      get
      {
        if (DateTimeUtils.compactTimeFormat == null)
          DateTimeUtils.compactTimeFormat = DateTimeFormatInfo.CurrentInfo.LongTimePattern.Replace("hh", "h").Replace("tt", "t").Replace(" ", "").Replace(":ss", "").Replace(".ss", "").Replace("ss", "");
        return DateTimeUtils.compactTimeFormat;
      }
    }

    public static string FormatDurationDescriptive(int seconds)
    {
      if (seconds < 0)
        return "";
      int number1 = seconds / 3600;
      seconds %= 3600;
      int number2 = seconds / 60;
      seconds %= 60;
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      if (number1 > 0)
      {
        stringBuilder.Append(Plurals.Instance.GetString(AppResources.HoursPlural, number1));
        flag = true;
      }
      if (number2 > 0)
      {
        if (flag)
          stringBuilder.Append(" ");
        stringBuilder.Append(Plurals.Instance.GetString(AppResources.MinutesPlural, number2));
        flag = true;
      }
      if (seconds > 0)
      {
        if (flag)
          stringBuilder.Append(" ");
        stringBuilder.Append(Plurals.Instance.GetString(AppResources.SecondsPlural, seconds));
      }
      return stringBuilder.ToString();
    }

    public static string FormatDuration(int seconds)
    {
      if (seconds < 0)
        return "";
      int num1 = seconds / 3600;
      seconds %= 3600;
      int num2 = seconds / 60;
      seconds %= 60;
      return num1 <= 0 ? string.Format("{0:0}:{1:00}", (object) num2, (object) seconds) : string.Format("{0}:{1:00}:{2:00}", (object) num1, (object) num2, (object) seconds);
    }

    public static string FormatCompact(
      DateTime dateTimeValue,
      DateTimeUtils.TimeDisplay timeDisp,
      bool useYesterdayTodayTomorrow = false)
    {
      bool usedYesterdayTodayTomorrow = false;
      return DateTimeUtils.Format(dateTimeValue, timeDisp, useYesterdayTodayTomorrow, false, out usedYesterdayTodayTomorrow);
    }

    public static string FormatCompactTime(DateTime dateTimeVal)
    {
      return dateTimeVal.ToString(DateTimeUtils.CompactTimeFormat).ToLower();
    }

    public static string FormatCompactDate(DateTime dateTimeVal, bool useTodayYesterday = false)
    {
      if (useTodayYesterday)
      {
        DateTime now = DateTime.Now;
        if (now - dateTimeVal < Constants.TwoDays)
        {
          switch (now.DayOfYear - dateTimeVal.DayOfYear)
          {
            case 0:
              return AppResources.Today;
            case 1:
              return AppResources.Yesterday;
          }
        }
      }
      return dateTimeVal.ToString(DateTimeUtils.CompactDateFormat);
    }

    public static string ToMonthDayString(this DateTime dt)
    {
      CultureInfo currentUiCulture = CultureInfo.CurrentUICulture;
      return dt.ToString(currentUiCulture.DateTimeFormat.MonthDayPattern);
    }

    public static string FormatLastSeen(DateTime dateTimeVal)
    {
      bool usedTodayYesterday = false;
      return DateTimeUtils.FormatLastSeen(dateTimeVal, out usedTodayYesterday);
    }

    public static string FormatLastSeen(DateTime dateTimeValue, out bool usedTodayYesterday)
    {
      usedTodayYesterday = false;
      return DateTimeUtils.Format(dateTimeValue, DateTimeUtils.TimeDisplay.SameMonthOnly, true, false, out usedTodayYesterday);
    }

    public static string FormatTimeSince(DateTime dateTimeValUtc)
    {
      TimeSpan timeSpan1 = FunRunner.CurrentServerTimeUtc - dateTimeValUtc;
      DateTime phoneTime = DateTimeUtils.FunTimeToPhoneTime(dateTimeValUtc);
      if (timeSpan1 < TimeSpan.FromDays(0.0))
        return DateTimeUtils.FormatFull(phoneTime);
      TimeSpan timeSpan2 = timeSpan1.Duration();
      if (!(timeSpan2 < Constants.ThirtyOneDays))
        return phoneTime.ToString(DateTimeUtils.FullDateFormat);
      string locString;
      int number;
      if (timeSpan2.TotalMinutes < 1.0)
      {
        locString = AppResources.SecondsAgoPlural;
        number = (int) timeSpan2.TotalSeconds;
      }
      else if (timeSpan2.TotalHours < 1.0)
      {
        locString = AppResources.MinutesAgoPlural;
        number = (int) timeSpan2.TotalMinutes;
      }
      else if (timeSpan2.TotalDays < 1.0)
      {
        locString = AppResources.HoursAgoPlural;
        number = (int) timeSpan2.TotalHours;
      }
      else
      {
        locString = AppResources.DaysAgoPlural;
        number = (int) timeSpan2.TotalDays;
      }
      return Plurals.Instance.GetString(locString, number);
    }

    public static string FormatMuteEndTime(DateTime dateTimeVal)
    {
      if (dateTimeVal.Kind != DateTimeKind.Local)
        dateTimeVal = dateTimeVal.ToLocalTime();
      DateTime now = DateTime.Now;
      if (!((now - dateTimeVal).Duration() < Constants.TwoDays) || now.DayOfYear - dateTimeVal.DayOfYear >= 1)
        return DateTimeUtils.FormatFull(dateTimeVal);
      bool usedYesterdayTodayTomorrow = false;
      return DateTimeUtils.Format(dateTimeVal, DateTimeUtils.TimeDisplay.Always, true, true, out usedYesterdayTodayTomorrow);
    }

    public static string FormatFull(DateTime dateTimeVal)
    {
      return string.Format("{0} {1}", (object) dateTimeVal.ToString(DateTimeUtils.FullDateFormat), (object) DateTimeUtils.FormatCompactTime(dateTimeVal));
    }

    private static string Format(
      DateTime dateTimeVal,
      DateTimeUtils.TimeDisplay timeDisp,
      bool useYesterdayTodayTomorrow,
      bool isFutureTime,
      out bool usedYesterdayTodayTomorrow)
    {
      usedYesterdayTodayTomorrow = false;
      if (dateTimeVal.Kind != DateTimeKind.Local)
        dateTimeVal = dateTimeVal.ToLocalTime();
      DateTime now = DateTime.Now;
      TimeSpan timeSpan1 = now - dateTimeVal;
      if (!isFutureTime && timeSpan1 < TimeSpan.FromDays(-1.0))
        return DateTimeUtils.FormatFull(dateTimeVal);
      TimeSpan timeSpan2 = timeSpan1.Duration();
      if (useYesterdayTodayTomorrow && timeSpan2 < Constants.TwoDays)
      {
        switch (now.DayOfYear - dateTimeVal.DayOfYear)
        {
          case -1:
            usedYesterdayTodayTomorrow = true;
            return string.Format(AppResources.TimeFormatWithAt, (object) AppResources.Tomorrow, (object) DateTimeUtils.FormatCompactTime(dateTimeVal));
          case 0:
            if (isFutureTime)
              return DateTimeUtils.FormatCompactTime(dateTimeVal);
            usedYesterdayTodayTomorrow = true;
            return string.Format(AppResources.TimeFormatWithAt, (object) AppResources.Today, (object) DateTimeUtils.FormatCompactTime(dateTimeVal));
          case 1:
            usedYesterdayTodayTomorrow = true;
            return string.Format(AppResources.TimeFormatWithAt, (object) AppResources.Yesterday, (object) DateTimeUtils.FormatCompactTime(dateTimeVal));
        }
      }
      if (timeSpan2 < Constants.TenHours || dateTimeVal.Day == now.Day && timeSpan2 < Constants.OneDay)
        return DateTimeUtils.FormatCompactTime(dateTimeVal);
      bool flag;
      switch (timeDisp)
      {
        case DateTimeUtils.TimeDisplay.SameWeekOnly:
          flag = timeSpan2 < Constants.SixDays;
          break;
        case DateTimeUtils.TimeDisplay.SameMonthOnly:
          flag = timeSpan2 < Constants.ThirtyOneDays;
          break;
        default:
          flag = timeDisp == DateTimeUtils.TimeDisplay.Always;
          break;
      }
      string str = !(timeSpan2 < Constants.SixDays) ? (!(timeSpan2 < Constants.ThirtyOneDays) ? dateTimeVal.ToString(DateTimeUtils.FullDateFormat) : dateTimeVal.ToString(DateTimeUtils.CompactDateFormat)) : string.Format("{0:ddd}", (object) dateTimeVal);
      return !flag ? str : string.Format("{0} {1}", (object) str, (object) DateTimeUtils.FormatCompactTime(dateTimeVal));
    }

    public static string GetShortTimestampId(DateTime dt)
    {
      return string.Format("{0}{1}{2}", (object) (dt.Year % 100), (object) dt.DayOfYear.ToString("000"), (object) (int) dt.TimeOfDay.TotalSeconds);
    }

    public static string FormatFromDateSpan(DateTime dt)
    {
      TimeSpan timeSpan = DateTime.Now.Date - dt;
      if (timeSpan.TotalDays < 1.0)
        return AppResources.Today;
      if (timeSpan.TotalDays < 2.0)
        return AppResources.Yesterday;
      return timeSpan.TotalDays < 6.0 ? DateTimeFormatInfo.CurrentInfo.GetDayName(dt.DayOfWeek) : dt.ToString(DateTimeUtils.FullDateFormat);
    }

    public static string FormatLiveLocationUpdatedTime(DateTime updatedTime)
    {
      TimeSpan timeSpan = FunRunner.CurrentServerTimeUtc.Subtract(updatedTime);
      if (timeSpan.Hours > 0)
        return string.Format(Plurals.Instance.GetString(AppResources.LiveLocationUpdatedHoursAgoPlural, timeSpan.Hours), (object) timeSpan.Hours);
      return timeSpan.Minutes > 0 ? string.Format(Plurals.Instance.GetString(AppResources.LiveLocationUpdatedMinutesAgoPlural, timeSpan.Minutes), (object) timeSpan.Minutes) : AppResources.LiveLocationUpdatedJustNow;
    }

    public static string FormatLiveLocationTimeLeft(long expiryInSeconds)
    {
      TimeSpan timeSpan = DateTimeUtils.FromUnixTime(expiryInSeconds).Subtract(FunRunner.CurrentServerTimeUtc);
      return timeSpan.Hours > 0 ? string.Format(Plurals.Instance.GetString(AppResources.LiveLocationHoursLeftPlural, timeSpan.Hours), (object) timeSpan.Hours) : string.Format(Plurals.Instance.GetString(AppResources.LiveLocationMinutesLeftPlural, timeSpan.Minutes), (object) timeSpan.Minutes);
    }

    public enum DateTimeGroupingKeys
    {
      None = 0,
      Recent = 1,
      Last7Days = 2,
      Last30Days = 3,
      MonthNumberBase = 100, // 0x00000064
    }

    public enum TimeDisplay
    {
      Never,
      SameDayOnly,
      SameWeekOnly,
      SameMonthOnly,
      Always,
    }
  }
}
