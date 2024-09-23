// Decompiled with JetBrains decompiler
// Type: ZXing.Common.Detector.WhiteRectangleDetector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common.Detector
{
  /// <summary>
  /// Detects a candidate barcode-like rectangular region within an image. It
  /// starts around the center of the image, increases the size of the candidate
  /// region until it finds a white rectangular region. By keeping track of the
  /// last black points it encountered, it determines the corners of the barcode.
  /// </summary>
  /// <author>David Olivier</author>
  public sealed class WhiteRectangleDetector
  {
    private const int INIT_SIZE = 10;
    private const int CORR = 1;
    private readonly BitMatrix image;
    private readonly int height;
    private readonly int width;
    private readonly int leftInit;
    private readonly int rightInit;
    private readonly int downInit;
    private readonly int upInit;

    /// <summary>Creates a WhiteRectangleDetector instance</summary>
    /// <param name="image">The image.</param>
    /// <returns>null, if image is too small, otherwise a WhiteRectangleDetector instance</returns>
    public static WhiteRectangleDetector Create(BitMatrix image)
    {
      if (image == null)
        return (WhiteRectangleDetector) null;
      WhiteRectangleDetector rectangleDetector = new WhiteRectangleDetector(image);
      return rectangleDetector.upInit < 0 || rectangleDetector.leftInit < 0 || rectangleDetector.downInit >= rectangleDetector.height || rectangleDetector.rightInit >= rectangleDetector.width ? (WhiteRectangleDetector) null : rectangleDetector;
    }

    /// <summary>Creates a WhiteRectangleDetector instance</summary>
    /// <param name="image">The image.</param>
    /// <param name="initSize">Size of the init.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>
    /// null, if image is too small, otherwise a WhiteRectangleDetector instance
    /// </returns>
    public static WhiteRectangleDetector Create(BitMatrix image, int initSize, int x, int y)
    {
      WhiteRectangleDetector rectangleDetector = new WhiteRectangleDetector(image, initSize, x, y);
      return rectangleDetector.upInit < 0 || rectangleDetector.leftInit < 0 || rectangleDetector.downInit >= rectangleDetector.height || rectangleDetector.rightInit >= rectangleDetector.width ? (WhiteRectangleDetector) null : rectangleDetector;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.Detector.WhiteRectangleDetector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <exception cref="T:System.ArgumentException">if image is too small</exception>
    internal WhiteRectangleDetector(BitMatrix image)
      : this(image, 10, image.Width / 2, image.Height / 2)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.Detector.WhiteRectangleDetector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="initSize">Size of the init.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    internal WhiteRectangleDetector(BitMatrix image, int initSize, int x, int y)
    {
      this.image = image;
      this.height = image.Height;
      this.width = image.Width;
      int num = initSize / 2;
      this.leftInit = x - num;
      this.rightInit = x + num;
      this.upInit = y - num;
      this.downInit = y + num;
    }

    /// <summary>
    /// Detects a candidate barcode-like rectangular region within an image. It
    /// starts around the center of the image, increases the size of the candidate
    /// region until it finds a white rectangular region.
    /// </summary>
    /// <returns><see cref="T:ZXing.ResultPoint" />[] describing the corners of the rectangular
    /// region. The first and last points are opposed on the diagonal, as
    /// are the second and third. The first point will be the topmost
    /// point and the last, the bottommost. The second point will be
    /// leftmost and the third, the rightmost</returns>
    public ResultPoint[] detect()
    {
      int leftInit = this.leftInit;
      int rightInit = this.rightInit;
      int upInit = this.upInit;
      int downInit = this.downInit;
      bool flag1 = false;
      bool flag2 = true;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      bool flag6 = false;
      bool flag7 = false;
      while (flag2)
      {
        flag2 = false;
        bool flag8 = true;
        while ((flag8 || !flag4) && rightInit < this.width)
        {
          flag8 = this.containsBlackPoint(upInit, downInit, rightInit, false);
          if (flag8)
          {
            ++rightInit;
            flag2 = true;
            flag4 = true;
          }
          else if (!flag4)
            ++rightInit;
        }
        if (rightInit >= this.width)
        {
          flag1 = true;
          break;
        }
        bool flag9 = true;
        while ((flag9 || !flag5) && downInit < this.height)
        {
          flag9 = this.containsBlackPoint(leftInit, rightInit, downInit, true);
          if (flag9)
          {
            ++downInit;
            flag2 = true;
            flag5 = true;
          }
          else if (!flag5)
            ++downInit;
        }
        if (downInit >= this.height)
        {
          flag1 = true;
          break;
        }
        bool flag10 = true;
        while ((flag10 || !flag6) && leftInit >= 0)
        {
          flag10 = this.containsBlackPoint(upInit, downInit, leftInit, false);
          if (flag10)
          {
            --leftInit;
            flag2 = true;
            flag6 = true;
          }
          else if (!flag6)
            --leftInit;
        }
        if (leftInit < 0)
        {
          flag1 = true;
          break;
        }
        bool flag11 = true;
        while ((flag11 || !flag7) && upInit >= 0)
        {
          flag11 = this.containsBlackPoint(leftInit, rightInit, upInit, true);
          if (flag11)
          {
            --upInit;
            flag2 = true;
            flag7 = true;
          }
          else if (!flag7)
            --upInit;
        }
        if (upInit < 0)
        {
          flag1 = true;
          break;
        }
        if (flag2)
          flag3 = true;
      }
      if (flag1 || !flag3)
        return (ResultPoint[]) null;
      int num = rightInit - leftInit;
      ResultPoint z = (ResultPoint) null;
      for (int index = 1; index < num; ++index)
      {
        z = this.getBlackPointOnSegment((float) leftInit, (float) (downInit - index), (float) (leftInit + index), (float) downInit);
        if (z != null)
          break;
      }
      if (z == null)
        return (ResultPoint[]) null;
      ResultPoint t = (ResultPoint) null;
      for (int index = 1; index < num; ++index)
      {
        t = this.getBlackPointOnSegment((float) leftInit, (float) (upInit + index), (float) (leftInit + index), (float) upInit);
        if (t != null)
          break;
      }
      if (t == null)
        return (ResultPoint[]) null;
      ResultPoint x = (ResultPoint) null;
      for (int index = 1; index < num; ++index)
      {
        x = this.getBlackPointOnSegment((float) rightInit, (float) (upInit + index), (float) (rightInit - index), (float) upInit);
        if (x != null)
          break;
      }
      if (x == null)
        return (ResultPoint[]) null;
      ResultPoint y = (ResultPoint) null;
      for (int index = 1; index < num; ++index)
      {
        y = this.getBlackPointOnSegment((float) rightInit, (float) (downInit - index), (float) (rightInit - index), (float) downInit);
        if (y != null)
          break;
      }
      return y == null ? (ResultPoint[]) null : this.centerEdges(y, z, x, t);
    }

    private ResultPoint getBlackPointOnSegment(float aX, float aY, float bX, float bY)
    {
      int num1 = MathUtils.round(MathUtils.distance(aX, aY, bX, bY));
      float num2 = (bX - aX) / (float) num1;
      float num3 = (bY - aY) / (float) num1;
      for (int index = 0; index < num1; ++index)
      {
        int x = MathUtils.round(aX + (float) index * num2);
        int y = MathUtils.round(aY + (float) index * num3);
        if (this.image[x, y])
          return new ResultPoint((float) x, (float) y);
      }
      return (ResultPoint) null;
    }

    /// <summary>
    /// recenters the points of a constant distance towards the center
    /// </summary>
    /// <param name="y">bottom most point</param>
    /// <param name="z">left most point</param>
    /// <param name="x">right most point</param>
    /// <param name="t">top most point</param>
    /// <returns><see cref="T:ZXing.ResultPoint" />[] describing the corners of the rectangular
    /// region. The first and last points are opposed on the diagonal, as
    /// are the second and third. The first point will be the topmost
    /// point and the last, the bottommost. The second point will be
    /// leftmost and the third, the rightmost</returns>
    private ResultPoint[] centerEdges(ResultPoint y, ResultPoint z, ResultPoint x, ResultPoint t)
    {
      float x1 = y.X;
      float y1 = y.Y;
      float x2 = z.X;
      float y2 = z.Y;
      float x3 = x.X;
      float y3 = x.Y;
      float x4 = t.X;
      float y4 = t.Y;
      return (double) x1 < (double) this.width / 2.0 ? new ResultPoint[4]
      {
        new ResultPoint(x4 - 1f, y4 + 1f),
        new ResultPoint(x2 + 1f, y2 + 1f),
        new ResultPoint(x3 - 1f, y3 - 1f),
        new ResultPoint(x1 + 1f, y1 - 1f)
      } : new ResultPoint[4]
      {
        new ResultPoint(x4 + 1f, y4 + 1f),
        new ResultPoint(x2 + 1f, y2 - 1f),
        new ResultPoint(x3 - 1f, y3 + 1f),
        new ResultPoint(x1 - 1f, y1 - 1f)
      };
    }

    /// <summary>Determines whether a segment contains a black point</summary>
    /// <param name="a">min value of the scanned coordinate</param>
    /// <param name="b">max value of the scanned coordinate</param>
    /// <param name="fixed">value of fixed coordinate</param>
    /// <param name="horizontal">set to true if scan must be horizontal, false if vertical</param>
    /// <returns>true if a black point has been found, else false.</returns>
    private bool containsBlackPoint(int a, int b, int @fixed, bool horizontal)
    {
      if (horizontal)
      {
        for (int x = a; x <= b; ++x)
        {
          if (this.image[x, @fixed])
            return true;
        }
      }
      else
      {
        for (int y = a; y <= b; ++y)
        {
          if (this.image[@fixed, y])
            return true;
        }
      }
      return false;
    }
  }
}
