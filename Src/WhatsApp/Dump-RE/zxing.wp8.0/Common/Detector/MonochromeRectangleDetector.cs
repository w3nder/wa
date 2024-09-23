// Decompiled with JetBrains decompiler
// Type: ZXing.Common.Detector.MonochromeRectangleDetector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common.Detector
{
  /// <summary> <p>A somewhat generic detector that looks for a barcode-like rectangular region within an image.
  /// It looks within a mostly white region of an image for a region of black and white, but mostly
  /// black. It returns the four corners of the region, as best it can determine.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class MonochromeRectangleDetector
  {
    private const int MAX_MODULES = 32;
    private BitMatrix image;

    public MonochromeRectangleDetector(BitMatrix image) => this.image = image;

    /// <summary> <p>Detects a rectangular region of black and white -- mostly black -- with a region of mostly
    /// white, in an image.</p>
    /// 
    /// </summary>
    /// <returns> {@link ResultPoint}[] describing the corners of the rectangular region. The first and
    /// last points are opposed on the diagonal, as are the second and third. The first point will be
    /// the topmost point and the last, the bottommost. The second point will be leftmost and the
    /// third, the rightmost
    /// </returns>
    public ResultPoint[] detect()
    {
      int height = this.image.Height;
      int width = this.image.Width;
      int centerY = height >> 1;
      int centerX = width >> 1;
      int deltaY = Math.Max(1, height / 256);
      int deltaX = Math.Max(1, width / 256);
      int top1 = 0;
      int bottom1 = height;
      int left1 = 0;
      int right1 = width;
      ResultPoint cornerFromCenter1 = this.findCornerFromCenter(centerX, 0, left1, right1, centerY, -deltaY, top1, bottom1, centerX >> 1);
      if (cornerFromCenter1 == null)
        return (ResultPoint[]) null;
      int top2 = (int) cornerFromCenter1.Y - 1;
      ResultPoint cornerFromCenter2 = this.findCornerFromCenter(centerX, -deltaX, left1, right1, centerY, 0, top2, bottom1, centerY >> 1);
      if (cornerFromCenter2 == null)
        return (ResultPoint[]) null;
      int left2 = (int) cornerFromCenter2.X - 1;
      ResultPoint cornerFromCenter3 = this.findCornerFromCenter(centerX, deltaX, left2, right1, centerY, 0, top2, bottom1, centerY >> 1);
      if (cornerFromCenter3 == null)
        return (ResultPoint[]) null;
      int right2 = (int) cornerFromCenter3.X + 1;
      ResultPoint cornerFromCenter4 = this.findCornerFromCenter(centerX, 0, left2, right2, centerY, deltaY, top2, bottom1, centerX >> 1);
      if (cornerFromCenter4 == null)
        return (ResultPoint[]) null;
      int bottom2 = (int) cornerFromCenter4.Y + 1;
      ResultPoint cornerFromCenter5 = this.findCornerFromCenter(centerX, 0, left2, right2, centerY, -deltaY, top2, bottom2, centerX >> 2);
      if (cornerFromCenter5 == null)
        return (ResultPoint[]) null;
      return new ResultPoint[4]
      {
        cornerFromCenter5,
        cornerFromCenter2,
        cornerFromCenter3,
        cornerFromCenter4
      };
    }

    /// <summary> Attempts to locate a corner of the barcode by scanning up, down, left or right from a center
    /// point which should be within the barcode.
    /// 
    /// </summary>
    /// <param name="centerX">center's x component (horizontal)</param>
    /// <param name="deltaX">same as deltaY but change in x per step instead</param>
    /// <param name="left">minimum value of x</param>
    /// <param name="right">maximum value of x</param>
    /// <param name="centerY">center's y component (vertical)</param>
    /// <param name="deltaY">change in y per step. If scanning up this is negative; down, positive;
    /// left or right, 0
    /// </param>
    /// <param name="top">minimum value of y to search through (meaningless when di == 0)
    /// </param>
    /// <param name="bottom">maximum value of y</param>
    /// <param name="maxWhiteRun">maximum run of white pixels that can still be considered to be within
    /// the barcode
    /// </param>
    /// <returns> a {@link com.google.zxing.ResultPoint} encapsulating the corner that was found
    /// </returns>
    private ResultPoint findCornerFromCenter(
      int centerX,
      int deltaX,
      int left,
      int right,
      int centerY,
      int deltaY,
      int top,
      int bottom,
      int maxWhiteRun)
    {
      int[] numArray1 = (int[]) null;
      int fixedDimension1 = centerY;
      for (int fixedDimension2 = centerX; fixedDimension1 < bottom && fixedDimension1 >= top && fixedDimension2 < right && fixedDimension2 >= left; fixedDimension2 += deltaX)
      {
        int[] numArray2 = deltaX != 0 ? this.blackWhiteRange(fixedDimension2, maxWhiteRun, top, bottom, false) : this.blackWhiteRange(fixedDimension1, maxWhiteRun, left, right, true);
        if (numArray2 == null)
        {
          if (numArray1 == null)
            return (ResultPoint) null;
          if (deltaX == 0)
          {
            int y = fixedDimension1 - deltaY;
            if (numArray1[0] >= centerX)
              return new ResultPoint((float) numArray1[1], (float) y);
            return numArray1[1] > centerX ? new ResultPoint(deltaY > 0 ? (float) numArray1[0] : (float) numArray1[1], (float) y) : new ResultPoint((float) numArray1[0], (float) y);
          }
          int x = fixedDimension2 - deltaX;
          if (numArray1[0] >= centerY)
            return new ResultPoint((float) x, (float) numArray1[1]);
          return numArray1[1] > centerY ? new ResultPoint((float) x, deltaX < 0 ? (float) numArray1[0] : (float) numArray1[1]) : new ResultPoint((float) x, (float) numArray1[0]);
        }
        numArray1 = numArray2;
        fixedDimension1 += deltaY;
      }
      return (ResultPoint) null;
    }

    /// <summary> Computes the start and end of a region of pixels, either horizontally or vertically, that could
    /// be part of a Data Matrix barcode.
    /// 
    /// </summary>
    /// <param name="fixedDimension">if scanning horizontally, this is the row (the fixed vertical location)
    /// where we are scanning. If scanning vertically it's the column, the fixed horizontal location
    /// </param>
    /// <param name="maxWhiteRun">largest run of white pixels that can still be considered part of the
    /// barcode region
    /// </param>
    /// <param name="minDim">minimum pixel location, horizontally or vertically, to consider
    /// </param>
    /// <param name="maxDim">maximum pixel location, horizontally or vertically, to consider
    /// </param>
    /// <param name="horizontal">if true, we're scanning left-right, instead of up-down
    /// </param>
    /// <returns> int[] with start and end of found range, or null if no such range is found
    /// (e.g. only white was found)
    /// </returns>
    private int[] blackWhiteRange(
      int fixedDimension,
      int maxWhiteRun,
      int minDim,
      int maxDim,
      bool horizontal)
    {
      int num1 = minDim + maxDim >> 1;
      int num2 = num1;
      while (num2 >= minDim)
      {
        if ((horizontal ? (this.image[num2, fixedDimension] ? 1 : 0) : (this.image[fixedDimension, num2] ? 1 : 0)) != 0)
        {
          --num2;
        }
        else
        {
          int num3 = num2;
          do
          {
            --num2;
          }
          while (num2 >= minDim && (horizontal ? (this.image[num2, fixedDimension] ? 1 : 0) : (this.image[fixedDimension, num2] ? 1 : 0)) == 0);
          int num4 = num3 - num2;
          if (num2 < minDim || num4 > maxWhiteRun)
          {
            num2 = num3;
            break;
          }
        }
      }
      int num5 = num2 + 1;
      int num6 = num1;
      while (num6 < maxDim)
      {
        if ((horizontal ? (this.image[num6, fixedDimension] ? 1 : 0) : (this.image[fixedDimension, num6] ? 1 : 0)) != 0)
        {
          ++num6;
        }
        else
        {
          int num7 = num6;
          do
          {
            ++num6;
          }
          while (num6 < maxDim && (horizontal ? (this.image[num6, fixedDimension] ? 1 : 0) : (this.image[fixedDimension, num6] ? 1 : 0)) == 0);
          int num8 = num6 - num7;
          if (num6 >= maxDim || num8 > maxWhiteRun)
          {
            num6 = num7;
            break;
          }
        }
      }
      int num9 = num6 - 1;
      if (num9 <= num5)
        return (int[]) null;
      return new int[2]{ num5, num9 };
    }
  }
}
