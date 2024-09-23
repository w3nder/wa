// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.RSSUtils
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS
{
  /// <summary>
  /// Adapted from listings in ISO/IEC 24724 Appendix B and Appendix G.
  /// </summary>
  public static class RSSUtils
  {
    /// <summary>Gets the RS svalue.</summary>
    /// <param name="widths">The widths.</param>
    /// <param name="maxWidth">Width of the max.</param>
    /// <param name="noNarrow">if set to <c>true</c> [no narrow].</param>
    /// <returns></returns>
    public static int getRSSvalue(int[] widths, int maxWidth, bool noNarrow)
    {
      int length = widths.Length;
      int num1 = 0;
      foreach (int width in widths)
        num1 += width;
      int rsSvalue = 0;
      int num2 = 0;
      for (int index1 = 0; index1 < length - 1; ++index1)
      {
        int num3 = 1;
        num2 |= 1 << index1;
        while (num3 < widths[index1])
        {
          int num4 = RSSUtils.combins(num1 - num3 - 1, length - index1 - 2);
          if (noNarrow && num2 == 0 && num1 - num3 - (length - index1 - 1) >= length - index1 - 1)
            num4 -= RSSUtils.combins(num1 - num3 - (length - index1), length - index1 - 2);
          if (length - index1 - 1 > 1)
          {
            int num5 = 0;
            for (int index2 = num1 - num3 - (length - index1 - 2); index2 > maxWidth; --index2)
              num5 += RSSUtils.combins(num1 - num3 - index2 - 1, length - index1 - 3);
            num4 -= num5 * (length - 1 - index1);
          }
          else if (num1 - num3 > maxWidth)
            --num4;
          rsSvalue += num4;
          ++num3;
          num2 &= ~(1 << index1);
        }
        num1 -= num3;
      }
      return rsSvalue;
    }

    private static int combins(int n, int r)
    {
      int num1;
      int num2;
      if (n - r > r)
      {
        num1 = r;
        num2 = n - r;
      }
      else
      {
        num1 = n - r;
        num2 = r;
      }
      int num3 = 1;
      int num4 = 1;
      for (int index = n; index > num2; --index)
      {
        num3 *= index;
        if (num4 <= num1)
        {
          num3 /= num4;
          ++num4;
        }
      }
      for (; num4 <= num1; ++num4)
        num3 /= num4;
      return num3;
    }
  }
}
