// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.BezierHelper
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal static class BezierHelper
  {
    private const double SmoothBy = 0.4;

    public static double GetSmoothedValue(
      double progress,
      double beforeFrom,
      double from,
      double to,
      double afterTo)
    {
      if (double.IsNaN(beforeFrom) && double.IsNaN(afterTo))
        return BezierHelper.Bezier(progress, from, to);
      if (double.IsNaN(beforeFrom) && !double.IsNaN(afterTo))
        return BezierHelper.Bezier(progress, from, from + 1.0 / 10.0 * (afterTo - from) - ((from + afterTo) / 2.0 - to), to);
      return !double.IsNaN(beforeFrom) && double.IsNaN(afterTo) ? BezierHelper.Bezier(progress, from, beforeFrom + 0.9 * (to - beforeFrom) - ((beforeFrom + to) / 2.0 - from), to) : BezierHelper.Bezier(progress, from, beforeFrom + 0.9 * (to - beforeFrom) - ((beforeFrom + to) / 2.0 - from), from + 1.0 / 10.0 * (afterTo - from) - ((from + afterTo) / 2.0 - to), to);
    }

    private static double Bezier(double progress, double p0, double p1, double p2, double p3)
    {
      double num = progress * progress;
      return num * progress * (p3 + 3.0 * (p1 - p2) - p0) + 3.0 * num * (p0 - 2.0 * p1 + p2) + 3.0 * progress * (p1 - p0) + p0;
    }

    private static double Bezier(double progress, double p0, double p1, double p2)
    {
      return progress * progress * (p0 - 2.0 * p1 + p2) + 2.0 * progress * (p1 - p0) + p0;
    }

    private static double Bezier(double progress, double p0, double p1)
    {
      return p0 + progress * (p1 - p0);
    }
  }
}
