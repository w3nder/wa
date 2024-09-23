// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MonthDataSource
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class MonthDataSource : DataSource
  {
    protected override DateTime? GetRelativeTo(DateTime relativeDate, int delta)
    {
      int num = 12;
      int month = (num + relativeDate.Month - 1 + delta) % num + 1;
      int day = Math.Min(relativeDate.Day, DateTime.DaysInMonth(relativeDate.Year, month));
      return new DateTime?(new DateTime(relativeDate.Year, month, day, relativeDate.Hour, relativeDate.Minute, relativeDate.Second));
    }
  }
}
