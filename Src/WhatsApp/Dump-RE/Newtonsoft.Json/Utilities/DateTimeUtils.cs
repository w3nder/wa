// Decompiled with JetBrains decompiler
// Type: Newtonsoft.Json.Utilities.DateTimeUtils
// Assembly: Newtonsoft.Json, Version=7.0.0.0, Culture=neutral, PublicKeyToken=30ad4fe6b2a6aeed
// MVID: 0D551458-BD0A-4E39-8947-735723526F43
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\Newtonsoft.Json.xml

using System;
using System.Globalization;
using System.IO;

#nullable disable
namespace Newtonsoft.Json.Utilities
{
  internal static class DateTimeUtils
  {
    private const int DaysPer100Years = 36524;
    private const int DaysPer400Years = 146097;
    private const int DaysPer4Years = 1461;
    private const int DaysPerYear = 365;
    private const long TicksPerDay = 864000000000;
    internal static readonly long InitialJavaScriptDateTicks = 621355968000000000;
    private static readonly int[] DaysToMonth365 = new int[13]
    {
      0,
      31,
      59,
      90,
      120,
      151,
      181,
      212,
      243,
      273,
      304,
      334,
      365
    };
    private static readonly int[] DaysToMonth366 = new int[13]
    {
      0,
      31,
      60,
      91,
      121,
      152,
      182,
      213,
      244,
      274,
      305,
      335,
      366
    };

    public static TimeSpan GetUtcOffset(this DateTime d) => TimeZoneInfo.Local.GetUtcOffset(d);

    internal static DateTime EnsureDateTime(DateTime value, DateTimeZoneHandling timeZone)
    {
      switch (timeZone)
      {
        case DateTimeZoneHandling.Local:
          value = DateTimeUtils.SwitchToLocalTime(value);
          goto case DateTimeZoneHandling.RoundtripKind;
        case DateTimeZoneHandling.Utc:
          value = DateTimeUtils.SwitchToUtcTime(value);
          goto case DateTimeZoneHandling.RoundtripKind;
        case DateTimeZoneHandling.Unspecified:
          value = new DateTime(value.Ticks, DateTimeKind.Unspecified);
          goto case DateTimeZoneHandling.RoundtripKind;
        case DateTimeZoneHandling.RoundtripKind:
          return value;
        default:
          throw new ArgumentException("Invalid date time handling value.");
      }
    }

    private static DateTime SwitchToLocalTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Unspecified:
          return new DateTime(value.Ticks, DateTimeKind.Local);
        case DateTimeKind.Utc:
          return value.ToLocalTime();
        case DateTimeKind.Local:
          return value;
        default:
          return value;
      }
    }

    private static DateTime SwitchToUtcTime(DateTime value)
    {
      switch (value.Kind)
      {
        case DateTimeKind.Unspecified:
          return new DateTime(value.Ticks, DateTimeKind.Utc);
        case DateTimeKind.Utc:
          return value;
        case DateTimeKind.Local:
          return value.ToUniversalTime();
        default:
          return value;
      }
    }

    private static long ToUniversalTicks(DateTime dateTime)
    {
      return dateTime.Kind == DateTimeKind.Utc ? dateTime.Ticks : DateTimeUtils.ToUniversalTicks(dateTime, dateTime.GetUtcOffset());
    }

    private static long ToUniversalTicks(DateTime dateTime, TimeSpan offset)
    {
      if (dateTime.Kind == DateTimeKind.Utc || dateTime == DateTime.MaxValue || dateTime == DateTime.MinValue)
        return dateTime.Ticks;
      long num = dateTime.Ticks - offset.Ticks;
      if (num > 3155378975999999999L)
        return 3155378975999999999;
      return num < 0L ? 0L : num;
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, TimeSpan offset)
    {
      return DateTimeUtils.UniversialTicksToJavaScriptTicks(DateTimeUtils.ToUniversalTicks(dateTime, offset));
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime)
    {
      return DateTimeUtils.ConvertDateTimeToJavaScriptTicks(dateTime, true);
    }

    internal static long ConvertDateTimeToJavaScriptTicks(DateTime dateTime, bool convertToUtc)
    {
      return DateTimeUtils.UniversialTicksToJavaScriptTicks(convertToUtc ? DateTimeUtils.ToUniversalTicks(dateTime) : dateTime.Ticks);
    }

    private static long UniversialTicksToJavaScriptTicks(long universialTicks)
    {
      return (universialTicks - DateTimeUtils.InitialJavaScriptDateTicks) / 10000L;
    }

    internal static DateTime ConvertJavaScriptTicksToDateTime(long javaScriptTicks)
    {
      return new DateTime(javaScriptTicks * 10000L + DateTimeUtils.InitialJavaScriptDateTicks, DateTimeKind.Utc);
    }

    internal static bool TryParseDateIso(
      string text,
      DateParseHandling dateParseHandling,
      DateTimeZoneHandling dateTimeZoneHandling,
      out object dt)
    {
      DateTimeParser dateTimeParser = new DateTimeParser();
      if (!dateTimeParser.Parse(text))
      {
        dt = (object) null;
        return false;
      }
      DateTime dateTime = new DateTime(dateTimeParser.Year, dateTimeParser.Month, dateTimeParser.Day, dateTimeParser.Hour, dateTimeParser.Minute, dateTimeParser.Second);
      dateTime = dateTime.AddTicks((long) dateTimeParser.Fraction);
      if (dateParseHandling == DateParseHandling.DateTimeOffset)
      {
        TimeSpan offset;
        switch (dateTimeParser.Zone)
        {
          case ParserTimeZone.Utc:
            offset = new TimeSpan(0L);
            break;
          case ParserTimeZone.LocalWestOfUtc:
            offset = new TimeSpan(-dateTimeParser.ZoneHour, -dateTimeParser.ZoneMinute, 0);
            break;
          case ParserTimeZone.LocalEastOfUtc:
            offset = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
            break;
          default:
            offset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
            break;
        }
        long num = dateTime.Ticks - offset.Ticks;
        if (num < 0L || num > 3155378975999999999L)
        {
          dt = (object) null;
          return false;
        }
        dt = (object) new DateTimeOffset(dateTime, offset);
        return true;
      }
      switch (dateTimeParser.Zone)
      {
        case ParserTimeZone.Utc:
          dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
          break;
        case ParserTimeZone.LocalWestOfUtc:
          TimeSpan timeSpan1 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
          long ticks1 = dateTime.Ticks + timeSpan1.Ticks;
          if (ticks1 <= DateTime.MaxValue.Ticks)
          {
            dateTime = new DateTime(ticks1, DateTimeKind.Utc).ToLocalTime();
            break;
          }
          long ticks2 = ticks1 + dateTime.GetUtcOffset().Ticks;
          if (ticks2 > DateTime.MaxValue.Ticks)
            ticks2 = DateTime.MaxValue.Ticks;
          dateTime = new DateTime(ticks2, DateTimeKind.Local);
          break;
        case ParserTimeZone.LocalEastOfUtc:
          TimeSpan timeSpan2 = new TimeSpan(dateTimeParser.ZoneHour, dateTimeParser.ZoneMinute, 0);
          long ticks3 = dateTime.Ticks - timeSpan2.Ticks;
          if (ticks3 >= DateTime.MinValue.Ticks)
          {
            dateTime = new DateTime(ticks3, DateTimeKind.Utc).ToLocalTime();
            break;
          }
          long ticks4 = ticks3 + dateTime.GetUtcOffset().Ticks;
          if (ticks4 < DateTime.MinValue.Ticks)
            ticks4 = DateTime.MinValue.Ticks;
          dateTime = new DateTime(ticks4, DateTimeKind.Local);
          break;
      }
      dt = (object) DateTimeUtils.EnsureDateTime(dateTime, dateTimeZoneHandling);
      return true;
    }

    internal static bool TryParseDateTime(
      string s,
      DateParseHandling dateParseHandling,
      DateTimeZoneHandling dateTimeZoneHandling,
      string dateFormatString,
      CultureInfo culture,
      out object dt)
    {
      if (s.Length > 0)
      {
        if (s[0] == '/')
        {
          if (s.StartsWith("/Date(", StringComparison.Ordinal) && s.EndsWith(")/", StringComparison.Ordinal) && DateTimeUtils.TryParseDateMicrosoft(s, dateParseHandling, dateTimeZoneHandling, out dt))
            return true;
        }
        else if (s.Length >= 19 && s.Length <= 40 && char.IsDigit(s[0]) && s[10] == 'T' && DateTimeUtils.TryParseDateIso(s, dateParseHandling, dateTimeZoneHandling, out dt))
          return true;
        if (!string.IsNullOrEmpty(dateFormatString) && DateTimeUtils.TryParseDateExact(s, dateParseHandling, dateTimeZoneHandling, dateFormatString, culture, out dt))
          return true;
      }
      dt = (object) null;
      return false;
    }

    private static bool TryParseDateMicrosoft(
      string text,
      DateParseHandling dateParseHandling,
      DateTimeZoneHandling dateTimeZoneHandling,
      out object dt)
    {
      string s = text.Substring(6, text.Length - 8);
      DateTimeKind dateTimeKind = DateTimeKind.Utc;
      int num = s.IndexOf('+', 1);
      if (num == -1)
        num = s.IndexOf('-', 1);
      TimeSpan offset = TimeSpan.Zero;
      if (num != -1)
      {
        dateTimeKind = DateTimeKind.Local;
        offset = DateTimeUtils.ReadOffset(s.Substring(num));
        s = s.Substring(0, num);
      }
      long result;
      if (!long.TryParse(s, NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
      {
        dt = (object) null;
        return false;
      }
      DateTime dateTime1 = DateTimeUtils.ConvertJavaScriptTicksToDateTime(result);
      if (dateParseHandling == DateParseHandling.DateTimeOffset)
      {
        dt = (object) new DateTimeOffset(dateTime1.Add(offset).Ticks, offset);
        return true;
      }
      DateTime dateTime2;
      switch (dateTimeKind)
      {
        case DateTimeKind.Unspecified:
          dateTime2 = DateTime.SpecifyKind(dateTime1.ToLocalTime(), DateTimeKind.Unspecified);
          break;
        case DateTimeKind.Local:
          dateTime2 = dateTime1.ToLocalTime();
          break;
        default:
          dateTime2 = dateTime1;
          break;
      }
      dt = (object) DateTimeUtils.EnsureDateTime(dateTime2, dateTimeZoneHandling);
      return true;
    }

    private static bool TryParseDateExact(
      string text,
      DateParseHandling dateParseHandling,
      DateTimeZoneHandling dateTimeZoneHandling,
      string dateFormatString,
      CultureInfo culture,
      out object dt)
    {
      if (dateParseHandling == DateParseHandling.DateTimeOffset)
      {
        DateTimeOffset result;
        if (DateTimeOffset.TryParseExact(text, dateFormatString, (IFormatProvider) culture, DateTimeStyles.RoundtripKind, out result))
        {
          dt = (object) result;
          return true;
        }
      }
      else
      {
        DateTime result;
        if (DateTime.TryParseExact(text, dateFormatString, (IFormatProvider) culture, DateTimeStyles.RoundtripKind, out result))
        {
          result = DateTimeUtils.EnsureDateTime(result, dateTimeZoneHandling);
          dt = (object) result;
          return true;
        }
      }
      dt = (object) null;
      return false;
    }

    private static TimeSpan ReadOffset(string offsetText)
    {
      bool flag = offsetText[0] == '-';
      int num1 = int.Parse(offsetText.Substring(1, 2), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      int num2 = 0;
      if (offsetText.Length >= 5)
        num2 = int.Parse(offsetText.Substring(3, 2), NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture);
      TimeSpan timeSpan = TimeSpan.FromHours((double) num1) + TimeSpan.FromMinutes((double) num2);
      if (flag)
        timeSpan = timeSpan.Negate();
      return timeSpan;
    }

    internal static void WriteDateTimeString(
      TextWriter writer,
      DateTime value,
      DateFormatHandling format,
      string formatString,
      CultureInfo culture)
    {
      if (string.IsNullOrEmpty(formatString))
      {
        char[] chArray = new char[64];
        int count = DateTimeUtils.WriteDateTimeString(chArray, 0, value, new TimeSpan?(), value.Kind, format);
        writer.Write(chArray, 0, count);
      }
      else
        writer.Write(value.ToString(formatString, (IFormatProvider) culture));
    }

    internal static int WriteDateTimeString(
      char[] chars,
      int start,
      DateTime value,
      TimeSpan? offset,
      DateTimeKind kind,
      DateFormatHandling format)
    {
      int num1 = start;
      int start1;
      if (format == DateFormatHandling.MicrosoftDateFormat)
      {
        TimeSpan offset1 = offset ?? value.GetUtcOffset();
        long javaScriptTicks = DateTimeUtils.ConvertDateTimeToJavaScriptTicks(value, offset1);
        "\\/Date(".CopyTo(0, chars, num1, 7);
        int destinationIndex = num1 + 7;
        string str = javaScriptTicks.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        str.CopyTo(0, chars, destinationIndex, str.Length);
        int num2 = destinationIndex + str.Length;
        switch (kind)
        {
          case DateTimeKind.Unspecified:
            if (value != DateTime.MaxValue && value != DateTime.MinValue)
            {
              num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset1, format);
              break;
            }
            break;
          case DateTimeKind.Local:
            num2 = DateTimeUtils.WriteDateTimeOffset(chars, num2, offset1, format);
            break;
        }
        ")\\/".CopyTo(0, chars, num2, 3);
        start1 = num2 + 3;
      }
      else
      {
        start1 = DateTimeUtils.WriteDefaultIsoDate(chars, num1, value);
        switch (kind)
        {
          case DateTimeKind.Utc:
            chars[start1++] = 'Z';
            break;
          case DateTimeKind.Local:
            start1 = DateTimeUtils.WriteDateTimeOffset(chars, start1, offset ?? value.GetUtcOffset(), format);
            break;
        }
      }
      return start1;
    }

    internal static int WriteDefaultIsoDate(char[] chars, int start, DateTime dt)
    {
      int num1 = 19;
      int year;
      int month;
      int day;
      DateTimeUtils.GetDateValues(dt, out year, out month, out day);
      DateTimeUtils.CopyIntToCharArray(chars, start, year, 4);
      chars[start + 4] = '-';
      DateTimeUtils.CopyIntToCharArray(chars, start + 5, month, 2);
      chars[start + 7] = '-';
      DateTimeUtils.CopyIntToCharArray(chars, start + 8, day, 2);
      chars[start + 10] = 'T';
      DateTimeUtils.CopyIntToCharArray(chars, start + 11, dt.Hour, 2);
      chars[start + 13] = ':';
      DateTimeUtils.CopyIntToCharArray(chars, start + 14, dt.Minute, 2);
      chars[start + 16] = ':';
      DateTimeUtils.CopyIntToCharArray(chars, start + 17, dt.Second, 2);
      int num2 = (int) (dt.Ticks % 10000000L);
      if (num2 != 0)
      {
        int digits = 7;
        for (; num2 % 10 == 0; num2 /= 10)
          --digits;
        chars[start + 19] = '.';
        DateTimeUtils.CopyIntToCharArray(chars, start + 20, num2, digits);
        num1 += digits + 1;
      }
      return start + num1;
    }

    private static void CopyIntToCharArray(char[] chars, int start, int value, int digits)
    {
      while (digits-- != 0)
      {
        chars[start + digits] = (char) (value % 10 + 48);
        value /= 10;
      }
    }

    internal static int WriteDateTimeOffset(
      char[] chars,
      int start,
      TimeSpan offset,
      DateFormatHandling format)
    {
      chars[start++] = offset.Ticks >= 0L ? '+' : '-';
      int num1 = Math.Abs(offset.Hours);
      DateTimeUtils.CopyIntToCharArray(chars, start, num1, 2);
      start += 2;
      if (format == DateFormatHandling.IsoDateFormat)
        chars[start++] = ':';
      int num2 = Math.Abs(offset.Minutes);
      DateTimeUtils.CopyIntToCharArray(chars, start, num2, 2);
      start += 2;
      return start;
    }

    internal static void WriteDateTimeOffsetString(
      TextWriter writer,
      DateTimeOffset value,
      DateFormatHandling format,
      string formatString,
      CultureInfo culture)
    {
      if (string.IsNullOrEmpty(formatString))
      {
        char[] chArray = new char[64];
        int count = DateTimeUtils.WriteDateTimeString(chArray, 0, format == DateFormatHandling.IsoDateFormat ? value.DateTime : value.UtcDateTime, new TimeSpan?(value.Offset), DateTimeKind.Local, format);
        writer.Write(chArray, 0, count);
      }
      else
        writer.Write(value.ToString(formatString, (IFormatProvider) culture));
    }

    private static void GetDateValues(DateTime td, out int year, out int month, out int day)
    {
      int num1 = (int) (td.Ticks / 864000000000L);
      int num2 = num1 / 146097;
      int num3 = num1 - num2 * 146097;
      int num4 = num3 / 36524;
      if (num4 == 4)
        num4 = 3;
      int num5 = num3 - num4 * 36524;
      int num6 = num5 / 1461;
      int num7 = num5 - num6 * 1461;
      int num8 = num7 / 365;
      if (num8 == 4)
        num8 = 3;
      year = num2 * 400 + num4 * 100 + num6 * 4 + num8 + 1;
      int num9 = num7 - num8 * 365;
      int[] numArray = num8 == 3 && (num6 != 24 || num4 == 3) ? DateTimeUtils.DaysToMonth366 : DateTimeUtils.DaysToMonth365;
      int index = num9 >> 6;
      while (num9 >= numArray[index])
        ++index;
      month = index;
      day = num9 - numArray[index - 1] + 1;
    }
  }
}
