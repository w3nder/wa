// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.RecurringDaysPicker
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Controls.LocalizedResources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls
{
  public class RecurringDaysPicker : ListPicker
  {
    private const string CommaSpace = ", ";
    private const string EnglishLanguage = "en";
    private string[] DayNames = CultureInfo.CurrentCulture.DateTimeFormat.DayNames;
    private string[] ShortestDayNames = CultureInfo.CurrentCulture.DateTimeFormat.ShortestDayNames;

    public RecurringDaysPicker()
    {
      if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        this.ShortestDayNames = new string[7]
        {
          "Sun",
          "Mon",
          "Tue",
          "Wed",
          "Thu",
          "Fri",
          "Sat"
        };
      DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
      for (int index = 0; index < ((IEnumerable<string>) this.DayNames).Count<string>(); ++index)
        this.Items.Add((object) this.DayNames[(int) (firstDayOfWeek + index) % ((IEnumerable<string>) this.DayNames).Count<string>()]);
      this.SelectionMode = SelectionMode.Multiple;
      this.SummaryForSelectedItemsDelegate = new Func<IList, string>(this.SummarizeDaysOfWeek);
    }

    protected string SummarizeDaysOfWeek(IList selection)
    {
      string repeatsOnlyOnce = ControlResources.RepeatsOnlyOnce;
      if (selection != null)
      {
        List<string> daysList = new List<string>();
        foreach (object obj in (IEnumerable) selection)
          daysList.Add((string) obj);
        repeatsOnlyOnce = this.DaysOfWeekToString(daysList);
      }
      return repeatsOnlyOnce;
    }

    private string DaysOfWeekToString(List<string> daysList)
    {
      List<string> days1 = new List<string>();
      foreach (string days2 in daysList)
      {
        if (!days1.Contains(days2))
          days1.Add(days2);
      }
      if (days1.Count == 0)
        return ControlResources.RepeatsOnlyOnce;
      StringBuilder stringBuilder = new StringBuilder();
      IEnumerable<string> unhandledDays;
      stringBuilder.Append(RecurringDaysPicker.HandleGroups(days1, out unhandledDays));
      if (stringBuilder.Length > 0)
        stringBuilder.Append(", ");
      DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
      for (int index1 = 0; index1 < ((IEnumerable<string>) this.DayNames).Count<string>(); ++index1)
      {
        int index2 = (int) (firstDayOfWeek + index1) % ((IEnumerable<string>) this.DayNames).Count<string>();
        string dayName = this.DayNames[index2];
        if (unhandledDays.Contains<string>(dayName))
        {
          stringBuilder.Append(this.ShortestDayNames[index2]);
          stringBuilder.Append(", ");
        }
      }
      stringBuilder.Length -= ", ".Length;
      return stringBuilder.ToString();
    }

    private static string HandleGroups(List<string> days, out IEnumerable<string> unhandledDays)
    {
      if (days.Count == 7)
      {
        unhandledDays = (IEnumerable<string>) new List<string>();
        return ControlResources.RepeatsEveryDay;
      }
      ReadOnlyCollection<string> weekdays = CultureInfo.CurrentCulture.Weekdays();
      ReadOnlyCollection<string> weekends = CultureInfo.CurrentCulture.Weekends();
      if (days.Intersect<string>((IEnumerable<string>) weekdays).Count<string>() == weekdays.Count)
      {
        unhandledDays = days.Where<string>((Func<string, bool>) (day => !weekdays.Contains(day)));
        return ControlResources.RepeatsOnWeekdays;
      }
      if (days.Intersect<string>((IEnumerable<string>) weekends).Count<string>() == weekends.Count)
      {
        unhandledDays = days.Where<string>((Func<string, bool>) (day => !weekends.Contains(day)));
        return ControlResources.RepeatsOnWeekends;
      }
      unhandledDays = (IEnumerable<string>) days;
      return string.Empty;
    }
  }
}
