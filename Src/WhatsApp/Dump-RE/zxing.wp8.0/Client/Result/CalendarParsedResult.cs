// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.CalendarParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class CalendarParsedResult : ParsedResult
  {
    private static readonly Regex RFC2445_DURATION = new Regex("\\A(?:P(?:(\\d+)W)?(?:(\\d+)D)?(?:T(?:(\\d+)H)?(?:(\\d+)M)?(?:(\\d+)S)?)?)\\z", RegexOptions.Compiled);
    private static readonly long[] RFC2445_DURATION_FIELD_UNITS = new long[5]
    {
      604800000L,
      86400000L,
      3600000L,
      60000L,
      1000L
    };
    private static readonly Regex DATE_TIME = new Regex("\\A(?:[0-9]{8}(T[0-9]{6}Z?)?)\\z", RegexOptions.Compiled);
    private readonly string summary;
    private readonly DateTime start;
    private readonly bool startAllDay;
    private readonly DateTime? end;
    private readonly bool endAllDay;
    private readonly string location;
    private readonly string organizer;
    private readonly string[] attendees;
    private readonly string description;
    private readonly double latitude;
    private readonly double longitude;

    public CalendarParsedResult(
      string summary,
      string startString,
      string endString,
      string durationString,
      string location,
      string organizer,
      string[] attendees,
      string description,
      double latitude,
      double longitude)
      : base(ParsedResultType.CALENDAR)
    {
      this.summary = summary;
      try
      {
        this.start = CalendarParsedResult.parseDate(startString);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(ex.ToString());
      }
      if (endString == null)
      {
        long durationMs = CalendarParsedResult.parseDurationMS(durationString);
        this.end = durationMs < 0L ? new DateTime?() : new DateTime?(this.start + new TimeSpan(0, 0, 0, 0, (int) durationMs));
      }
      else
      {
        try
        {
          this.end = new DateTime?(CalendarParsedResult.parseDate(endString));
        }
        catch (Exception ex)
        {
          throw new ArgumentException(ex.ToString());
        }
      }
      this.startAllDay = startString.Length == 8;
      this.endAllDay = endString != null && endString.Length == 8;
      this.location = location;
      this.organizer = organizer;
      this.attendees = attendees;
      this.description = description;
      this.latitude = latitude;
      this.longitude = longitude;
      StringBuilder result = new StringBuilder(100);
      ParsedResult.maybeAppend(summary, result);
      ParsedResult.maybeAppend(CalendarParsedResult.format(this.startAllDay, new DateTime?(this.start)), result);
      ParsedResult.maybeAppend(CalendarParsedResult.format(this.endAllDay, this.end), result);
      ParsedResult.maybeAppend(location, result);
      ParsedResult.maybeAppend(organizer, result);
      ParsedResult.maybeAppend(attendees, result);
      ParsedResult.maybeAppend(description, result);
      this.displayResultValue = result.ToString();
    }

    public string Summary => this.summary;

    /// <summary>Gets the start.</summary>
    public DateTime Start => this.start;

    /// <summary>Determines whether [is start all day].</summary>
    /// <returns>if start time was specified as a whole day</returns>
    public bool isStartAllDay() => this.startAllDay;

    /// <summary>May return null if the event has no duration.</summary>
    public DateTime? End => this.end;

    /// <summary>
    /// Gets a value indicating whether this instance is end all day.
    /// </summary>
    /// <value>true if end time was specified as a whole day</value>
    public bool isEndAllDay => this.endAllDay;

    public string Location => this.location;

    public string Organizer => this.organizer;

    public string[] Attendees => this.attendees;

    public string Description => this.description;

    public double Latitude => this.latitude;

    public double Longitude => this.longitude;

    /// <summary>
    /// Parses a string as a date. RFC 2445 allows the start and end fields to be of type DATE (e.g. 20081021)
    /// or DATE-TIME (e.g. 20081021T123000 for local time, or 20081021T123000Z for UTC).
    /// </summary>
    /// <param name="when">The string to parse</param>
    /// <returns></returns>
    /// <exception cref="T:System.ArgumentException">if not a date formatted string</exception>
    private static DateTime parseDate(string when)
    {
      if (!CalendarParsedResult.DATE_TIME.Match(when).Success)
        throw new ArgumentException(string.Format("no date format: {0}", (object) when));
      return when.Length == 8 ? DateTime.ParseExact(when, CalendarParsedResult.buildDateFormat(), (IFormatProvider) CultureInfo.InvariantCulture) : (when.Length != 16 || when[15] != 'Z' ? DateTime.ParseExact(when, CalendarParsedResult.buildDateTimeFormat(), (IFormatProvider) CultureInfo.InvariantCulture) : TimeZoneInfo.ConvertTime(DateTime.ParseExact(when.Substring(0, 15), CalendarParsedResult.buildDateTimeFormat(), (IFormatProvider) CultureInfo.InvariantCulture), TimeZoneInfo.Local));
    }

    private static string format(bool allDay, DateTime? date)
    {
      if (!date.HasValue)
        return (string) null;
      return allDay ? date.Value.ToString("D", (IFormatProvider) CultureInfo.CurrentCulture) : date.Value.ToString("F", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    private static long parseDurationMS(string durationString)
    {
      if (durationString == null)
        return -1;
      Match match = CalendarParsedResult.RFC2445_DURATION.Match(durationString);
      if (!match.Success)
        return -1;
      long durationMs = 0;
      for (int index = 0; index < CalendarParsedResult.RFC2445_DURATION_FIELD_UNITS.Length; ++index)
      {
        string s = match.Groups[index + 1].Value;
        if (!string.IsNullOrEmpty(s))
          durationMs += CalendarParsedResult.RFC2445_DURATION_FIELD_UNITS[index] * (long) int.Parse(s);
      }
      return durationMs;
    }

    private static string buildDateFormat() => "yyyyMMdd";

    private static string buildDateTimeFormat() => "yyyyMMdd'T'HHmmss";
  }
}
