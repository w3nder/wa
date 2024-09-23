// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.MathHelpers
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Xna.Framework;
using System;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal static class MathHelpers
  {
    public static double GetAngle(double deltaX, double deltaY)
    {
      double num = Math.Atan2(deltaY, deltaX);
      if (num < 0.0)
        num = 2.0 * Math.PI + num;
      return num * 360.0 / (2.0 * Math.PI);
    }

    public static double GetDistance(System.Windows.Point p0, System.Windows.Point p1)
    {
      double num1 = p0.X - p1.X;
      double num2 = p0.Y - p1.Y;
      return Math.Sqrt(num1 * num1 + num2 * num2);
    }

    public static System.Windows.Point ToPoint(this Vector2 v)
    {
      return new System.Windows.Point((double) v.X, (double) v.Y);
    }
  }
}
