// Decompiled with JetBrains decompiler
// Type: ScreenSizeSupport.Misc.MathHelpers
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;

#nullable disable
namespace ScreenSizeSupport.Misc
{
  public static class MathHelpers
  {
    public const double Epsilon = 0.001;

    public static bool IsCloseEnoughTo(this double d1, double d2) => Math.Abs(d1 - d2) < 0.001;

    public static bool IsCloseEnoughOrSmallerThan(this double d1, double d2) => d1 < d2 + 0.001;

    public static double NudgeToClosestPoint(this double currentValue, int nudgeValue)
    {
      return Math.Floor(currentValue * 10.0 / (double) nudgeValue + 0.001) / 10.0 * (double) nudgeValue;
    }
  }
}
