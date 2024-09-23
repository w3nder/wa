// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.VEventResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// Partially implements the iCalendar format's "VEVENT" format for specifying a
  /// calendar event. See RFC 2445. This supports SUMMARY, DTSTART and DTEND fields.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class VEventResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null)
        return (ParsedResult) null;
      if (text.IndexOf("BEGIN:VEVENT") < 0)
        return (ParsedResult) null;
      string summary = VEventResultParser.matchSingleVCardPrefixedField("SUMMARY", text, true);
      string startString = VEventResultParser.matchSingleVCardPrefixedField("DTSTART", text, true);
      if (startString == null)
        return (ParsedResult) null;
      string endString = VEventResultParser.matchSingleVCardPrefixedField("DTEND", text, true);
      string durationString = VEventResultParser.matchSingleVCardPrefixedField("DURATION", text, true);
      string location = VEventResultParser.matchSingleVCardPrefixedField("LOCATION", text, true);
      string organizer = VEventResultParser.stripMailto(VEventResultParser.matchSingleVCardPrefixedField("ORGANIZER", text, true));
      string[] attendees = VEventResultParser.matchVCardPrefixedField("ATTENDEE", text, true);
      if (attendees != null)
      {
        for (int index = 0; index < attendees.Length; ++index)
          attendees[index] = VEventResultParser.stripMailto(attendees[index]);
      }
      string description = VEventResultParser.matchSingleVCardPrefixedField("DESCRIPTION", text, true);
      string str = VEventResultParser.matchSingleVCardPrefixedField("GEO", text, true);
      double result1;
      double result2;
      if (str == null)
      {
        result1 = double.NaN;
        result2 = double.NaN;
      }
      else
      {
        int length = str.IndexOf(';');
        if (length < 0)
          return (ParsedResult) null;
        if (!double.TryParse(str.Substring(0, length), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          return (ParsedResult) null;
        if (!double.TryParse(str.Substring(length + 1), NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
          return (ParsedResult) null;
      }
      try
      {
        return (ParsedResult) new CalendarParsedResult(summary, startString, endString, durationString, location, organizer, attendees, description, result1, result2);
      }
      catch (ArgumentException ex)
      {
        return (ParsedResult) null;
      }
    }

    private static string matchSingleVCardPrefixedField(string prefix, string rawText, bool trim)
    {
      List<string> stringList = VCardResultParser.matchSingleVCardPrefixedField(prefix, rawText, trim, false);
      return stringList != null && stringList.Count != 0 ? stringList[0] : (string) null;
    }

    private static string[] matchVCardPrefixedField(string prefix, string rawText, bool trim)
    {
      List<List<string>> stringListList = VCardResultParser.matchVCardPrefixedField(prefix, rawText, trim, false);
      if (stringListList == null || stringListList.Count == 0)
        return (string[]) null;
      int count = stringListList.Count;
      string[] strArray = new string[count];
      for (int index = 0; index < count; ++index)
        strArray[index] = stringListList[index][0];
      return strArray;
    }

    private static string stripMailto(string s)
    {
      if (s != null && (s.StartsWith("mailto:") || s.StartsWith("MAILTO:")))
        s = s.Substring(7);
      return s;
    }
  }
}
