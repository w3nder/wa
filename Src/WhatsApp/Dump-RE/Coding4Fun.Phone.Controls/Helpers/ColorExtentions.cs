// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.Helpers.ColorExtentions
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls.Helpers
{
  public static class ColorExtentions
  {
    private static float Max(float val1, float val2, float val3)
    {
      return Math.Max(Math.Max(val1, val2), val3);
    }

    private static float Min(float val1, float val2, float val3)
    {
      return Math.Min(Math.Min(val1, val2), val3);
    }

    public static float GetHue(this Color color)
    {
      if ((int) color.R == (int) color.G && (int) color.G == (int) color.B)
        return 0.0f;
      float val1 = (float) color.R / (float) byte.MaxValue;
      float val2 = (float) color.G / (float) byte.MaxValue;
      float val3 = (float) color.B / (float) byte.MaxValue;
      float num1 = ColorExtentions.Min(val1, val2, val3);
      float num2 = ColorExtentions.Max(val1, val2, val3);
      float num3 = num2 - num1;
      float hue = ((double) val1 != (double) num2 ? ((double) val2 != (double) num2 ? (float) (4.0 + ((double) val1 - (double) val2) / (double) num3) : (float) (2.0 + ((double) val3 - (double) val1) / (double) num3)) : (val2 - val3) / num3) * 60f;
      if ((double) hue < 0.0)
        hue += 360f;
      return hue;
    }

    public static HSV GetHSV(this Color color)
    {
      return new HSV()
      {
        Hue = color.GetHue(),
        Saturation = color.GetSaturation(),
        Value = color.GetValue()
      };
    }

    public static float GetSaturation(this Color color)
    {
      float val1 = (float) color.R / (float) byte.MaxValue;
      float val2 = (float) color.G / (float) byte.MaxValue;
      float val3 = (float) color.B / (float) byte.MaxValue;
      float num1 = ColorExtentions.Min(val1, val2, val3);
      float num2 = ColorExtentions.Max(val1, val2, val3);
      return (double) num2 == (double) num1 || (double) num2 == 0.0 ? 0.0f : (float) (1.0 - 1.0 * (double) num1 / (double) num2);
    }

    public static float GetValue(this Color color)
    {
      return ColorExtentions.Max((float) color.R / (float) byte.MaxValue, (float) color.G / (float) byte.MaxValue, (float) color.B / (float) byte.MaxValue);
    }
  }
}
