// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.Detector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.Detector;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// <p>Encapsulates logic that can detect a QR Code in an image, even if the QR Code
  /// is rotated or skewed, or partially obscured.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public class Detector
  {
    private readonly BitMatrix image;
    private ResultPointCallback resultPointCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.Detector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public Detector(BitMatrix image) => this.image = image;

    /// <summary>Gets the image.</summary>
    protected internal virtual BitMatrix Image => this.image;

    /// <summary>Gets the result point callback.</summary>
    protected internal virtual ResultPointCallback ResultPointCallback => this.resultPointCallback;

    /// <summary>
    ///   <p>Detects a QR Code in an image, simply.</p>
    /// </summary>
    /// <returns>
    ///   <see cref="T:ZXing.Common.DetectorResult" /> encapsulating results of detecting a QR Code
    /// </returns>
    public virtual DetectorResult detect()
    {
      return this.detect((IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    ///   <p>Detects a QR Code in an image, simply.</p>
    /// </summary>
    /// <param name="hints">optional hints to detector</param>
    /// <returns>
    ///   <see cref="T:ZXing.Common.DetectorResult" /> encapsulating results of detecting a QR Code
    /// </returns>
    public virtual DetectorResult detect(IDictionary<DecodeHintType, object> hints)
    {
      this.resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      FinderPatternInfo info = new FinderPatternFinder(this.image, this.resultPointCallback).find(hints);
      return info == null ? (DetectorResult) null : this.processFinderPatternInfo(info);
    }

    /// <summary>Processes the finder pattern info.</summary>
    /// <param name="info">The info.</param>
    /// <returns></returns>
    protected internal virtual DetectorResult processFinderPatternInfo(FinderPatternInfo info)
    {
      FinderPattern topLeft = info.TopLeft;
      FinderPattern topRight = info.TopRight;
      FinderPattern bottomLeft = info.BottomLeft;
      float moduleSize = this.calculateModuleSize((ResultPoint) topLeft, (ResultPoint) topRight, (ResultPoint) bottomLeft);
      if ((double) moduleSize < 1.0)
        return (DetectorResult) null;
      int dimension;
      if (!ZXing.QrCode.Internal.Detector.computeDimension((ResultPoint) topLeft, (ResultPoint) topRight, (ResultPoint) bottomLeft, moduleSize, out dimension))
        return (DetectorResult) null;
      Version versionForDimension = Version.getProvisionalVersionForDimension(dimension);
      if (versionForDimension == null)
        return (DetectorResult) null;
      int num1 = versionForDimension.DimensionForVersion - 7;
      AlignmentPattern alignmentPattern = (AlignmentPattern) null;
      if (versionForDimension.AlignmentPatternCenters.Length > 0)
      {
        float num2 = topRight.X - topLeft.X + bottomLeft.X;
        float num3 = topRight.Y - topLeft.Y + bottomLeft.Y;
        float num4 = (float) (1.0 - 3.0 / (double) num1);
        int estAlignmentX = (int) ((double) topLeft.X + (double) num4 * ((double) num2 - (double) topLeft.X));
        int estAlignmentY = (int) ((double) topLeft.Y + (double) num4 * ((double) num3 - (double) topLeft.Y));
        for (int allowanceFactor = 4; allowanceFactor <= 16; allowanceFactor <<= 1)
        {
          alignmentPattern = this.findAlignmentInRegion(moduleSize, estAlignmentX, estAlignmentY, (float) allowanceFactor);
          if (alignmentPattern != null)
            break;
        }
      }
      BitMatrix bits = ZXing.QrCode.Internal.Detector.sampleGrid(this.image, ZXing.QrCode.Internal.Detector.createTransform((ResultPoint) topLeft, (ResultPoint) topRight, (ResultPoint) bottomLeft, (ResultPoint) alignmentPattern, dimension), dimension);
      if (bits == null)
        return (DetectorResult) null;
      ResultPoint[] points;
      if (alignmentPattern == null)
        points = new ResultPoint[3]
        {
          (ResultPoint) bottomLeft,
          (ResultPoint) topLeft,
          (ResultPoint) topRight
        };
      else
        points = new ResultPoint[4]
        {
          (ResultPoint) bottomLeft,
          (ResultPoint) topLeft,
          (ResultPoint) topRight,
          (ResultPoint) alignmentPattern
        };
      return new DetectorResult(bits, points);
    }

    private static PerspectiveTransform createTransform(
      ResultPoint topLeft,
      ResultPoint topRight,
      ResultPoint bottomLeft,
      ResultPoint alignmentPattern,
      int dimension)
    {
      float num = (float) dimension - 3.5f;
      float x2p;
      float y2p;
      float y2;
      float x2;
      if (alignmentPattern != null)
      {
        x2p = alignmentPattern.X;
        y2p = alignmentPattern.Y;
        x2 = y2 = num - 3f;
      }
      else
      {
        x2p = topRight.X - topLeft.X + bottomLeft.X;
        y2p = topRight.Y - topLeft.Y + bottomLeft.Y;
        x2 = y2 = num;
      }
      return PerspectiveTransform.quadrilateralToQuadrilateral(3.5f, 3.5f, num, 3.5f, x2, y2, 3.5f, num, topLeft.X, topLeft.Y, topRight.X, topRight.Y, x2p, y2p, bottomLeft.X, bottomLeft.Y);
    }

    private static BitMatrix sampleGrid(
      BitMatrix image,
      PerspectiveTransform transform,
      int dimension)
    {
      return GridSampler.Instance.sampleGrid(image, dimension, dimension, transform);
    }

    /// <summary> <p>Computes the dimension (number of modules on a size) of the QR Code based on the position
    /// of the finder patterns and estimated module size.</p>
    /// </summary>
    private static bool computeDimension(
      ResultPoint topLeft,
      ResultPoint topRight,
      ResultPoint bottomLeft,
      float moduleSize,
      out int dimension)
    {
      int num1 = MathUtils.round(ResultPoint.distance(topLeft, topRight) / moduleSize);
      int num2 = MathUtils.round(ResultPoint.distance(topLeft, bottomLeft) / moduleSize);
      dimension = (num1 + num2 >> 1) + 7;
      switch (dimension & 3)
      {
        case 0:
          ++dimension;
          break;
        case 2:
          --dimension;
          break;
        case 3:
          return true;
      }
      return true;
    }

    /// <summary> <p>Computes an average estimated module size based on estimated derived from the positions
    /// of the three finder patterns.</p>
    /// </summary>
    protected internal virtual float calculateModuleSize(
      ResultPoint topLeft,
      ResultPoint topRight,
      ResultPoint bottomLeft)
    {
      return (float) (((double) this.calculateModuleSizeOneWay(topLeft, topRight) + (double) this.calculateModuleSizeOneWay(topLeft, bottomLeft)) / 2.0);
    }

    /// <summary> <p>Estimates module size based on two finder patterns -- it uses
    /// {@link #sizeOfBlackWhiteBlackRunBothWays(int, int, int, int)} to figure the
    /// width of each, measuring along the axis between their centers.</p>
    /// </summary>
    private float calculateModuleSizeOneWay(ResultPoint pattern, ResultPoint otherPattern)
    {
      float f1 = this.sizeOfBlackWhiteBlackRunBothWays((int) pattern.X, (int) pattern.Y, (int) otherPattern.X, (int) otherPattern.Y);
      float f2 = this.sizeOfBlackWhiteBlackRunBothWays((int) otherPattern.X, (int) otherPattern.Y, (int) pattern.X, (int) pattern.Y);
      if (float.IsNaN(f1))
        return f2 / 7f;
      return float.IsNaN(f2) ? f1 / 7f : (float) (((double) f1 + (double) f2) / 14.0);
    }

    /// <summary> See {@link #sizeOfBlackWhiteBlackRun(int, int, int, int)}; computes the total width of
    /// a finder pattern by looking for a black-white-black run from the center in the direction
    /// of another point (another finder pattern center), and in the opposite direction too.
    /// </summary>
    private float sizeOfBlackWhiteBlackRunBothWays(int fromX, int fromY, int toX, int toY)
    {
      float num1 = this.sizeOfBlackWhiteBlackRun(fromX, fromY, toX, toY);
      float num2 = 1f;
      int num3 = fromX - (toX - fromX);
      if (num3 < 0)
      {
        num2 = (float) fromX / (float) (fromX - num3);
        num3 = 0;
      }
      else if (num3 >= this.image.Width)
      {
        num2 = (float) (this.image.Width - 1 - fromX) / (float) (num3 - fromX);
        num3 = this.image.Width - 1;
      }
      int toY1 = (int) ((double) fromY - (double) (toY - fromY) * (double) num2);
      float num4 = 1f;
      if (toY1 < 0)
      {
        num4 = (float) fromY / (float) (fromY - toY1);
        toY1 = 0;
      }
      else if (toY1 >= this.image.Height)
      {
        num4 = (float) (this.image.Height - 1 - fromY) / (float) (toY1 - fromY);
        toY1 = this.image.Height - 1;
      }
      int toX1 = (int) ((double) fromX + (double) (num3 - fromX) * (double) num4);
      return num1 + this.sizeOfBlackWhiteBlackRun(fromX, fromY, toX1, toY1) - 1f;
    }

    /// <summary> <p>This method traces a line from a point in the image, in the direction towards another point.
    /// It begins in a black region, and keeps going until it finds white, then black, then white again.
    /// It reports the distance from the start to this point.</p>
    /// 
    /// <p>This is used when figuring out how wide a finder pattern is, when the finder pattern
    /// may be skewed or rotated.</p>
    /// </summary>
    private float sizeOfBlackWhiteBlackRun(int fromX, int fromY, int toX, int toY)
    {
      bool flag = Math.Abs(toY - fromY) > Math.Abs(toX - fromX);
      if (flag)
      {
        int num1 = fromX;
        fromX = fromY;
        fromY = num1;
        int num2 = toX;
        toX = toY;
        toY = num2;
      }
      int num3 = Math.Abs(toX - fromX);
      int num4 = Math.Abs(toY - fromY);
      int num5 = -num3 >> 1;
      int num6 = fromX < toX ? 1 : -1;
      int num7 = fromY < toY ? 1 : -1;
      int num8 = 0;
      int num9 = toX + num6;
      int aX = fromX;
      int aY = fromY;
      for (; aX != num9; aX += num6)
      {
        int x = flag ? aY : aX;
        int y = flag ? aX : aY;
        if (num8 == 1 == this.image[x, y])
        {
          if (num8 == 2)
            return MathUtils.distance(aX, aY, fromX, fromY);
          ++num8;
        }
        num5 += num4;
        if (num5 > 0)
        {
          if (aY != toY)
          {
            aY += num7;
            num5 -= num3;
          }
          else
            break;
        }
      }
      return num8 == 2 ? MathUtils.distance(toX + num6, toY, fromX, fromY) : float.NaN;
    }

    /// <summary>
    ///   <p>Attempts to locate an alignment pattern in a limited region of the image, which is
    /// guessed to contain it. This method uses {@link AlignmentPattern}.</p>
    /// </summary>
    /// <param name="overallEstModuleSize">estimated module size so far</param>
    /// <param name="estAlignmentX">x coordinate of center of area probably containing alignment pattern</param>
    /// <param name="estAlignmentY">y coordinate of above</param>
    /// <param name="allowanceFactor">number of pixels in all directions to search from the center</param>
    /// <returns>
    ///   <see cref="T:ZXing.QrCode.Internal.AlignmentPattern" /> if found, or null otherwise
    /// </returns>
    protected AlignmentPattern findAlignmentInRegion(
      float overallEstModuleSize,
      int estAlignmentX,
      int estAlignmentY,
      float allowanceFactor)
    {
      int num1 = (int) ((double) allowanceFactor * (double) overallEstModuleSize);
      int startX = Math.Max(0, estAlignmentX - num1);
      int num2 = Math.Min(this.image.Width - 1, estAlignmentX + num1);
      if ((double) (num2 - startX) < (double) overallEstModuleSize * 3.0)
        return (AlignmentPattern) null;
      int startY = Math.Max(0, estAlignmentY - num1);
      int num3 = Math.Min(this.image.Height - 1, estAlignmentY + num1);
      return new AlignmentPatternFinder(this.image, startX, startY, num2 - startX, num3 - startY, overallEstModuleSize, this.resultPointCallback).find();
    }
  }
}
