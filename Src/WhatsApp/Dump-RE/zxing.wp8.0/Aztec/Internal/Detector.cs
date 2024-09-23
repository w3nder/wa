// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.Detector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;
using ZXing.Common.Detector;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>
  /// Encapsulates logic that can detect an Aztec Code in an image, even if the Aztec Code
  /// is rotated or skewed, or partially obscured.
  /// </summary>
  /// <author>David Olivier</author>
  public sealed class Detector
  {
    private readonly BitMatrix image;
    private bool compact;
    private int nbLayers;
    private int nbDataBlocks;
    private int nbCenterLayers;
    private int shift;
    private static readonly int[] EXPECTED_CORNER_BITS = new int[4]
    {
      3808,
      476,
      2107,
      1799
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Aztec.Internal.Detector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public Detector(BitMatrix image) => this.image = image;

    /// <summary>Detects an Aztec Code in an image.</summary>
    public AztecDetectorResult detect() => this.detect(false);

    /// <summary>Detects an Aztec Code in an image.</summary>
    /// <param name="isMirror">if set to <c>true</c> [is mirror].</param>
    /// <returns>encapsulating results of detecting an Aztec Code</returns>
    public AztecDetectorResult detect(bool isMirror)
    {
      ZXing.Aztec.Internal.Detector.Point matrixCenter = this.getMatrixCenter();
      if (matrixCenter == null)
        return (AztecDetectorResult) null;
      ResultPoint[] bullsEyeCorners = this.getBullsEyeCorners(matrixCenter);
      if (bullsEyeCorners == null)
        return (AztecDetectorResult) null;
      if (isMirror)
      {
        ResultPoint resultPoint = bullsEyeCorners[0];
        bullsEyeCorners[0] = bullsEyeCorners[2];
        bullsEyeCorners[2] = resultPoint;
      }
      if (!this.extractParameters(bullsEyeCorners))
        return (AztecDetectorResult) null;
      BitMatrix bits = this.sampleGrid(this.image, bullsEyeCorners[this.shift % 4], bullsEyeCorners[(this.shift + 1) % 4], bullsEyeCorners[(this.shift + 2) % 4], bullsEyeCorners[(this.shift + 3) % 4]);
      if (bits == null)
        return (AztecDetectorResult) null;
      ResultPoint[] matrixCornerPoints = this.getMatrixCornerPoints(bullsEyeCorners);
      return matrixCornerPoints == null ? (AztecDetectorResult) null : new AztecDetectorResult(bits, matrixCornerPoints, this.compact, this.nbDataBlocks, this.nbLayers);
    }

    /// <summary>
    /// Extracts the number of data layers and data blocks from the layer around the bull's eye
    /// </summary>
    /// <param name="bullsEyeCorners">bullEyeCornerPoints the array of bull's eye corners</param>
    /// <returns></returns>
    private bool extractParameters(ResultPoint[] bullsEyeCorners)
    {
      if (!this.isValid(bullsEyeCorners[0]) || !this.isValid(bullsEyeCorners[1]) || !this.isValid(bullsEyeCorners[2]) || !this.isValid(bullsEyeCorners[3]))
        return false;
      int num1 = 2 * this.nbCenterLayers;
      int[] sides = new int[4]
      {
        this.sampleLine(bullsEyeCorners[0], bullsEyeCorners[1], num1),
        this.sampleLine(bullsEyeCorners[1], bullsEyeCorners[2], num1),
        this.sampleLine(bullsEyeCorners[2], bullsEyeCorners[3], num1),
        this.sampleLine(bullsEyeCorners[3], bullsEyeCorners[0], num1)
      };
      this.shift = ZXing.Aztec.Internal.Detector.getRotation(sides, num1);
      if (this.shift < 0)
        return false;
      long parameterData = 0;
      for (int index = 0; index < 4; ++index)
      {
        int num2 = sides[(this.shift + index) % 4];
        parameterData = !this.compact ? (parameterData << 10) + (long) ((num2 >> 2 & 992) + (num2 >> 1 & 31)) : (parameterData << 7) + (long) (num2 >> 1 & (int) sbyte.MaxValue);
      }
      int correctedParameterData = ZXing.Aztec.Internal.Detector.getCorrectedParameterData(parameterData, this.compact);
      if (correctedParameterData < 0)
        return false;
      if (this.compact)
      {
        this.nbLayers = (correctedParameterData >> 6) + 1;
        this.nbDataBlocks = (correctedParameterData & 63) + 1;
      }
      else
      {
        this.nbLayers = (correctedParameterData >> 11) + 1;
        this.nbDataBlocks = (correctedParameterData & 2047) + 1;
      }
      return true;
    }

    private static int getRotation(int[] sides, int length)
    {
      int num1 = 0;
      foreach (int side in sides)
      {
        int num2 = (side >> length - 2 << 1) + (side & 1);
        num1 = (num1 << 3) + num2;
      }
      int num3 = ((num1 & 1) << 11) + (num1 >> 1);
      for (int rotation = 0; rotation < 4; ++rotation)
      {
        if (SupportClass.bitCount(num3 ^ ZXing.Aztec.Internal.Detector.EXPECTED_CORNER_BITS[rotation]) <= 2)
          return rotation;
      }
      return -1;
    }

    /// <summary>
    /// Corrects the parameter bits using Reed-Solomon algorithm
    /// </summary>
    /// <param name="parameterData">paremeter bits</param>
    /// <param name="compact">compact true if this is a compact Aztec code</param>
    /// <returns></returns>
    private static int getCorrectedParameterData(long parameterData, bool compact)
    {
      int length;
      int num;
      if (compact)
      {
        length = 7;
        num = 2;
      }
      else
      {
        length = 10;
        num = 4;
      }
      int twoS = length - num;
      int[] received = new int[length];
      for (int index = length - 1; index >= 0; --index)
      {
        received[index] = (int) parameterData & 15;
        parameterData >>= 4;
      }
      if (!new ReedSolomonDecoder(GenericGF.AZTEC_PARAM).decode(received, twoS))
        return -1;
      int correctedParameterData = 0;
      for (int index = 0; index < num; ++index)
        correctedParameterData = (correctedParameterData << 4) + received[index];
      return correctedParameterData;
    }

    /// <summary>
    /// Finds the corners of a bull-eye centered on the passed point
    /// This returns the centers of the diagonal points just outside the bull's eye
    /// Returns [topRight, bottomRight, bottomLeft, topLeft]
    /// </summary>
    /// <param name="pCenter">Center point</param>
    /// <returns>The corners of the bull-eye</returns>
    private ResultPoint[] getBullsEyeCorners(ZXing.Aztec.Internal.Detector.Point pCenter)
    {
      ZXing.Aztec.Internal.Detector.Point point1 = pCenter;
      ZXing.Aztec.Internal.Detector.Point init1 = pCenter;
      ZXing.Aztec.Internal.Detector.Point init2 = pCenter;
      ZXing.Aztec.Internal.Detector.Point point2 = pCenter;
      bool color = true;
      for (this.nbCenterLayers = 1; this.nbCenterLayers < 9; ++this.nbCenterLayers)
      {
        ZXing.Aztec.Internal.Detector.Point firstDifferent1 = this.getFirstDifferent(point1, color, 1, -1);
        ZXing.Aztec.Internal.Detector.Point firstDifferent2 = this.getFirstDifferent(init1, color, 1, 1);
        ZXing.Aztec.Internal.Detector.Point firstDifferent3 = this.getFirstDifferent(init2, color, -1, 1);
        ZXing.Aztec.Internal.Detector.Point firstDifferent4 = this.getFirstDifferent(point2, color, -1, -1);
        if (this.nbCenterLayers > 2)
        {
          float num = (float) ((double) ZXing.Aztec.Internal.Detector.distance(firstDifferent4, firstDifferent1) * (double) this.nbCenterLayers / ((double) ZXing.Aztec.Internal.Detector.distance(point2, point1) * (double) (this.nbCenterLayers + 2)));
          if ((double) num < 0.75 || (double) num > 1.25 || !this.isWhiteOrBlackRectangle(firstDifferent1, firstDifferent2, firstDifferent3, firstDifferent4))
            break;
        }
        point1 = firstDifferent1;
        init1 = firstDifferent2;
        init2 = firstDifferent3;
        point2 = firstDifferent4;
        color = !color;
      }
      if (this.nbCenterLayers != 5 && this.nbCenterLayers != 7)
        return (ResultPoint[]) null;
      this.compact = this.nbCenterLayers == 5;
      return ZXing.Aztec.Internal.Detector.expandSquare(new ResultPoint[4]
      {
        new ResultPoint((float) point1.X + 0.5f, (float) point1.Y - 0.5f),
        new ResultPoint((float) init1.X + 0.5f, (float) init1.Y + 0.5f),
        new ResultPoint((float) init2.X - 0.5f, (float) init2.Y + 0.5f),
        new ResultPoint((float) point2.X - 0.5f, (float) point2.Y - 0.5f)
      }, (float) (2 * this.nbCenterLayers - 3), (float) (2 * this.nbCenterLayers));
    }

    /// <summary>
    /// Finds a candidate center point of an Aztec code from an image
    /// </summary>
    /// <returns>the center point</returns>
    private ZXing.Aztec.Internal.Detector.Point getMatrixCenter()
    {
      WhiteRectangleDetector rectangleDetector1 = WhiteRectangleDetector.Create(this.image);
      if (rectangleDetector1 == null)
        return (ZXing.Aztec.Internal.Detector.Point) null;
      ResultPoint[] resultPointArray1 = rectangleDetector1.detect();
      ResultPoint resultPoint1;
      ResultPoint resultPoint2;
      ResultPoint resultPoint3;
      ResultPoint resultPoint4;
      if (resultPointArray1 != null)
      {
        resultPoint1 = resultPointArray1[0];
        resultPoint2 = resultPointArray1[1];
        resultPoint3 = resultPointArray1[2];
        resultPoint4 = resultPointArray1[3];
      }
      else
      {
        int num1 = this.image.Width / 2;
        int num2 = this.image.Height / 2;
        resultPoint1 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(num1 + 7, num2 - 7), false, 1, -1).toResultPoint();
        resultPoint2 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(num1 + 7, num2 + 7), false, 1, 1).toResultPoint();
        resultPoint3 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(num1 - 7, num2 + 7), false, -1, 1).toResultPoint();
        resultPoint4 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(num1 - 7, num2 - 7), false, -1, -1).toResultPoint();
      }
      int x = MathUtils.round((float) (((double) resultPoint1.X + (double) resultPoint4.X + (double) resultPoint2.X + (double) resultPoint3.X) / 4.0));
      int y = MathUtils.round((float) (((double) resultPoint1.Y + (double) resultPoint4.Y + (double) resultPoint2.Y + (double) resultPoint3.Y) / 4.0));
      WhiteRectangleDetector rectangleDetector2 = WhiteRectangleDetector.Create(this.image, 15, x, y);
      if (rectangleDetector2 == null)
        return (ZXing.Aztec.Internal.Detector.Point) null;
      ResultPoint[] resultPointArray2 = rectangleDetector2.detect();
      ResultPoint resultPoint5;
      ResultPoint resultPoint6;
      ResultPoint resultPoint7;
      ResultPoint resultPoint8;
      if (resultPointArray2 != null)
      {
        resultPoint5 = resultPointArray2[0];
        resultPoint6 = resultPointArray2[1];
        resultPoint7 = resultPointArray2[2];
        resultPoint8 = resultPointArray2[3];
      }
      else
      {
        resultPoint5 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(x + 7, y - 7), false, 1, -1).toResultPoint();
        resultPoint6 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(x + 7, y + 7), false, 1, 1).toResultPoint();
        resultPoint7 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(x - 7, y + 7), false, -1, 1).toResultPoint();
        resultPoint8 = this.getFirstDifferent(new ZXing.Aztec.Internal.Detector.Point(x - 7, y - 7), false, -1, -1).toResultPoint();
      }
      return new ZXing.Aztec.Internal.Detector.Point(MathUtils.round((float) (((double) resultPoint5.X + (double) resultPoint8.X + (double) resultPoint6.X + (double) resultPoint7.X) / 4.0)), MathUtils.round((float) (((double) resultPoint5.Y + (double) resultPoint8.Y + (double) resultPoint6.Y + (double) resultPoint7.Y) / 4.0)));
    }

    /// <summary>
    /// Gets the Aztec code corners from the bull's eye corners and the parameters.
    /// </summary>
    /// <param name="bullsEyeCorners">the array of bull's eye corners</param>
    /// <returns>the array of aztec code corners</returns>
    private ResultPoint[] getMatrixCornerPoints(ResultPoint[] bullsEyeCorners)
    {
      return ZXing.Aztec.Internal.Detector.expandSquare(bullsEyeCorners, (float) (2 * this.nbCenterLayers), (float) this.getDimension());
    }

    /// <summary>
    /// Creates a BitMatrix by sampling the provided image.
    /// topLeft, topRight, bottomRight, and bottomLeft are the centers of the squares on the
    /// diagonal just outside the bull's eye.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="topLeft">The top left.</param>
    /// <param name="bottomLeft">The bottom left.</param>
    /// <param name="bottomRight">The bottom right.</param>
    /// <param name="topRight">The top right.</param>
    /// <returns></returns>
    private BitMatrix sampleGrid(
      BitMatrix image,
      ResultPoint topLeft,
      ResultPoint topRight,
      ResultPoint bottomRight,
      ResultPoint bottomLeft)
    {
      GridSampler instance = GridSampler.Instance;
      int dimension = this.getDimension();
      float num1 = (float) dimension / 2f - (float) this.nbCenterLayers;
      float num2 = (float) dimension / 2f + (float) this.nbCenterLayers;
      return instance.sampleGrid(image, dimension, dimension, num1, num1, num2, num1, num2, num2, num1, num2, topLeft.X, topLeft.Y, topRight.X, topRight.Y, bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
    }

    /// <summary>Samples a line</summary>
    /// <param name="p1">start point (inclusive)</param>
    /// <param name="p2">end point (exclusive)</param>
    /// <param name="size">number of bits</param>
    /// <returns> the array of bits as an int (first bit is high-order bit of result)</returns>
    private int sampleLine(ResultPoint p1, ResultPoint p2, int size)
    {
      int num1 = 0;
      float num2 = ZXing.Aztec.Internal.Detector.distance(p1, p2);
      float num3 = num2 / (float) size;
      float x = p1.X;
      float y = p1.Y;
      float num4 = num3 * (p2.X - p1.X) / num2;
      float num5 = num3 * (p2.Y - p1.Y) / num2;
      for (int index = 0; index < size; ++index)
      {
        if (this.image[MathUtils.round(x + (float) index * num4), MathUtils.round(y + (float) index * num5)])
          num1 |= 1 << size - index - 1;
      }
      return num1;
    }

    /// <summary>
    /// Determines whether [is white or black rectangle] [the specified p1].
    /// </summary>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <param name="p3">The p3.</param>
    /// <param name="p4">The p4.</param>
    /// <returns>true if the border of the rectangle passed in parameter is compound of white points only
    /// or black points only</returns>
    private bool isWhiteOrBlackRectangle(
      ZXing.Aztec.Internal.Detector.Point p1,
      ZXing.Aztec.Internal.Detector.Point p2,
      ZXing.Aztec.Internal.Detector.Point p3,
      ZXing.Aztec.Internal.Detector.Point p4)
    {
      p1 = new ZXing.Aztec.Internal.Detector.Point(p1.X - 3, p1.Y + 3);
      p2 = new ZXing.Aztec.Internal.Detector.Point(p2.X - 3, p2.Y - 3);
      p3 = new ZXing.Aztec.Internal.Detector.Point(p3.X + 3, p3.Y - 3);
      p4 = new ZXing.Aztec.Internal.Detector.Point(p4.X + 3, p4.Y + 3);
      int color = this.getColor(p4, p1);
      return color != 0 && this.getColor(p1, p2) == color && this.getColor(p2, p3) == color && this.getColor(p3, p4) == color;
    }

    /// <summary>Gets the color of a segment</summary>
    /// <param name="p1">The p1.</param>
    /// <param name="p2">The p2.</param>
    /// <returns>1 if segment more than 90% black, -1 if segment is more than 90% white, 0 else</returns>
    private int getColor(ZXing.Aztec.Internal.Detector.Point p1, ZXing.Aztec.Internal.Detector.Point p2)
    {
      float num1 = ZXing.Aztec.Internal.Detector.distance(p1, p2);
      float num2 = (float) (p2.X - p1.X) / num1;
      float num3 = (float) (p2.Y - p1.Y) / num1;
      int num4 = 0;
      float x = (float) p1.X;
      float y = (float) p1.Y;
      bool flag = this.image[p1.X, p1.Y];
      for (int index = 0; (double) index < (double) num1; ++index)
      {
        x += num2;
        y += num3;
        if (this.image[MathUtils.round(x), MathUtils.round(y)] != flag)
          ++num4;
      }
      float num5 = (float) num4 / num1;
      if ((double) num5 > 0.10000000149011612 && (double) num5 < 0.89999997615814209)
        return 0;
      return (double) num5 <= 0.10000000149011612 != flag ? -1 : 1;
    }

    /// <summary>
    /// Gets the coordinate of the first point with a different color in the given direction
    /// </summary>
    /// <param name="init">The init.</param>
    /// <param name="color">if set to <c>true</c> [color].</param>
    /// <param name="dx">The dx.</param>
    /// <param name="dy">The dy.</param>
    /// <returns></returns>
    private ZXing.Aztec.Internal.Detector.Point getFirstDifferent(
      ZXing.Aztec.Internal.Detector.Point init,
      bool color,
      int dx,
      int dy)
    {
      int x1 = init.X + dx;
      int y1;
      for (y1 = init.Y + dy; this.isValid(x1, y1) && this.image[x1, y1] == color; y1 += dy)
        x1 += dx;
      int x2 = x1 - dx;
      int y2 = y1 - dy;
      while (this.isValid(x2, y2) && this.image[x2, y2] == color)
        x2 += dx;
      int x3 = x2 - dx;
      while (this.isValid(x3, y2) && this.image[x3, y2] == color)
        y2 += dy;
      int y3 = y2 - dy;
      return new ZXing.Aztec.Internal.Detector.Point(x3, y3);
    }

    /// <summary>
    /// Expand the square represented by the corner points by pushing out equally in all directions
    /// </summary>
    /// <param name="cornerPoints">the corners of the square, which has the bull's eye at its center</param>
    /// <param name="oldSide">the original length of the side of the square in the target bit matrix</param>
    /// <param name="newSide">the new length of the size of the square in the target bit matrix</param>
    /// <returns>the corners of the expanded square</returns>
    private static ResultPoint[] expandSquare(
      ResultPoint[] cornerPoints,
      float oldSide,
      float newSide)
    {
      float num1 = newSide / (2f * oldSide);
      float num2 = cornerPoints[0].X - cornerPoints[2].X;
      float num3 = cornerPoints[0].Y - cornerPoints[2].Y;
      float num4 = (float) (((double) cornerPoints[0].X + (double) cornerPoints[2].X) / 2.0);
      float num5 = (float) (((double) cornerPoints[0].Y + (double) cornerPoints[2].Y) / 2.0);
      ResultPoint resultPoint1 = new ResultPoint(num4 + num1 * num2, num5 + num1 * num3);
      ResultPoint resultPoint2 = new ResultPoint(num4 - num1 * num2, num5 - num1 * num3);
      float num6 = cornerPoints[1].X - cornerPoints[3].X;
      float num7 = cornerPoints[1].Y - cornerPoints[3].Y;
      float num8 = (float) (((double) cornerPoints[1].X + (double) cornerPoints[3].X) / 2.0);
      float num9 = (float) (((double) cornerPoints[1].Y + (double) cornerPoints[3].Y) / 2.0);
      ResultPoint resultPoint3 = new ResultPoint(num8 + num1 * num6, num9 + num1 * num7);
      ResultPoint resultPoint4 = new ResultPoint(num8 - num1 * num6, num9 - num1 * num7);
      return new ResultPoint[4]
      {
        resultPoint1,
        resultPoint3,
        resultPoint2,
        resultPoint4
      };
    }

    private bool isValid(int x, int y)
    {
      return x >= 0 && x < this.image.Width && y > 0 && y < this.image.Height;
    }

    private bool isValid(ResultPoint point)
    {
      return this.isValid(MathUtils.round(point.X), MathUtils.round(point.Y));
    }

    private static float distance(ZXing.Aztec.Internal.Detector.Point a, ZXing.Aztec.Internal.Detector.Point b)
    {
      return MathUtils.distance(a.X, a.Y, b.X, b.Y);
    }

    private static float distance(ResultPoint a, ResultPoint b)
    {
      return MathUtils.distance(a.X, a.Y, b.X, b.Y);
    }

    private int getDimension()
    {
      if (this.compact)
        return 4 * this.nbLayers + 11;
      return this.nbLayers <= 4 ? 4 * this.nbLayers + 15 : 4 * this.nbLayers + 2 * ((this.nbLayers - 4) / 8 + 1) + 15;
    }

    internal sealed class Point
    {
      public int X { get; private set; }

      public int Y { get; private set; }

      public ResultPoint toResultPoint() => new ResultPoint((float) this.X, (float) this.Y);

      internal Point(int x, int y)
      {
        this.X = x;
        this.Y = y;
      }

      public override string ToString()
      {
        return "<" + (object) this.X + (object) ' ' + (object) this.Y + (object) '>';
      }
    }
  }
}
