// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.YearDataSource
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class YearDataSource : DataSource
  {
    protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
    {
      if (1601 == relativeDate.Year || 3000 == relativeDate.Year)
        return new DateTime?();
      int year = relativeDate.Year + delta;
      int day = Math.Min(relativeDate.Day, DateTime.DaysInMonth(year, relativeDate.Month));
      return new DateTime?(new DateTime(year, relativeDate.Month, day, relativeDate.Hour, relativeDate.Minute, relativeDate.Second));
    }
  }
}
