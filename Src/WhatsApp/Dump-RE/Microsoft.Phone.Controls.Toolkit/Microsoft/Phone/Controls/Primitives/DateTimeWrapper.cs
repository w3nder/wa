// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.DateTimeWrapper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Phone.Controls.Primitives
{
  public class DateTimeWrapper
  {
    public DateTime DateTime { get; private set; }

    public string YearNumber
    {
      get => this.DateTime.ToString("yyyy", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string MonthNumber
    {
      get => this.DateTime.ToString("MM", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string MonthName
    {
      get => this.DateTime.ToString("MMMM", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string DayNumber
    {
      get => this.DateTime.ToString("dd", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string DayName
    {
      get => this.DateTime.ToString("dddd", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string HourNumber
    {
      get
      {
        return this.DateTime.ToString(DateTimeWrapper.CurrentCultureUsesTwentyFourHourClock() ? "%H" : "%h", (IFormatProvider) CultureInfo.CurrentCulture);
      }
    }

    public string MinuteNumber
    {
      get => this.DateTime.ToString("mm", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public string AmPmString
    {
      get => this.DateTime.ToString("tt", (IFormatProvider) CultureInfo.CurrentCulture);
    }

    public DateTimeWrapper(DateTime dateTime) => this.DateTime = dateTime;

    public static bool CurrentCultureUsesTwentyFourHourClock()
    {
      return !CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern.Contains("t");
    }
  }
}
