// Decompiled with JetBrains decompiler
// Type: ZXing.Common.Detector.MathUtils
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common.Detector
{
  public static class MathUtils
  {
    /// <summary>
    /// Ends up being a bit faster than {@link Math#round(float)}. This merely rounds its
    /// argument to the nearest int, where x.5 rounds up to x+1.
    /// </summary>
    /// <param name="d">The d.</param>
    /// <returns></returns>
    public static int round(float d) => (int) ((double) d + 0.5);

    public static float distance(float aX, float aY, float bX, float bY)
    {
      float num1 = aX - bX;
      float num2 = aY - bY;
      return (float) Math.Sqrt((double) num1 * (double) num1 + (double) num2 * (double) num2);
    }

    public static float distance(int aX, int aY, int bX, int bY)
    {
      int num1 = aX - bX;
      int num2 = aY - bY;
      return (float) Math.Sqrt((double) (num1 * num1 + num2 * num2));
    }
  }
}
