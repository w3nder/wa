// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.Detector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Common.Detector;

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// <p>Encapsulates logic that can detect a Data Matrix Code in an image, even if the Data Matrix Code
  /// is rotated or skewed, or partially obscured.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class Detector
  {
    private readonly BitMatrix image;
    private readonly WhiteRectangleDetector rectangleDetector;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Datamatrix.Internal.Detector" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    public Detector(BitMatrix image)
    {
      this.image = image;
      this.rectangleDetector = WhiteRectangleDetector.Create(image);
    }

    /// <summary>
    /// <p>Detects a Data Matrix Code in an image.</p>
    /// </summary>
    /// <returns><see cref="T:ZXing.Common.DetectorResult" />encapsulating results of detecting a Data Matrix Code or null</returns>
    public DetectorResult detect()
    {
      if (this.rectangleDetector == null)
        return (DetectorResult) null;
      ResultPoint[] resultPointArray = this.rectangleDetector.detect();
      if (resultPointArray == null)
        return (DetectorResult) null;
      ResultPoint resultPoint1 = resultPointArray[0];
      ResultPoint resultPoint2 = resultPointArray[1];
      ResultPoint resultPoint3 = resultPointArray[2];
      ResultPoint to = resultPointArray[3];
      List<ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions> pointsAndTransitionsList = new List<ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions>(4);
      pointsAndTransitionsList.Add(this.transitionsBetween(resultPoint1, resultPoint2));
      pointsAndTransitionsList.Add(this.transitionsBetween(resultPoint1, resultPoint3));
      pointsAndTransitionsList.Add(this.transitionsBetween(resultPoint2, to));
      pointsAndTransitionsList.Add(this.transitionsBetween(resultPoint3, to));
      pointsAndTransitionsList.Sort((IComparer<ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions>) new ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitionsComparator());
      ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions pointsAndTransitions1 = pointsAndTransitionsList[0];
      ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions pointsAndTransitions2 = pointsAndTransitionsList[1];
      Dictionary<ResultPoint, int> table = new Dictionary<ResultPoint, int>();
      ZXing.Datamatrix.Internal.Detector.increment((IDictionary<ResultPoint, int>) table, pointsAndTransitions1.From);
      ZXing.Datamatrix.Internal.Detector.increment((IDictionary<ResultPoint, int>) table, pointsAndTransitions1.To);
      ZXing.Datamatrix.Internal.Detector.increment((IDictionary<ResultPoint, int>) table, pointsAndTransitions2.From);
      ZXing.Datamatrix.Internal.Detector.increment((IDictionary<ResultPoint, int>) table, pointsAndTransitions2.To);
      ResultPoint resultPoint4 = (ResultPoint) null;
      ResultPoint resultPoint5 = (ResultPoint) null;
      ResultPoint resultPoint6 = (ResultPoint) null;
      foreach (KeyValuePair<ResultPoint, int> keyValuePair in table)
      {
        ResultPoint key = keyValuePair.Key;
        if (keyValuePair.Value == 2)
          resultPoint5 = key;
        else if (resultPoint4 == null)
          resultPoint4 = key;
        else
          resultPoint6 = key;
      }
      if (resultPoint4 == null || resultPoint5 == null || resultPoint6 == null)
        return (DetectorResult) null;
      ResultPoint[] patterns = new ResultPoint[3]
      {
        resultPoint4,
        resultPoint5,
        resultPoint6
      };
      ResultPoint.orderBestPatterns(patterns);
      ResultPoint resultPoint7 = patterns[0];
      ResultPoint bottomLeft = patterns[1];
      ResultPoint resultPoint8 = patterns[2];
      ResultPoint resultPoint9 = table.ContainsKey(resultPoint1) ? (table.ContainsKey(resultPoint2) ? (table.ContainsKey(resultPoint3) ? to : resultPoint3) : resultPoint2) : resultPoint1;
      int transitions1 = this.transitionsBetween(resultPoint8, resultPoint9).Transitions;
      int transitions2 = this.transitionsBetween(resultPoint7, resultPoint9).Transitions;
      if ((transitions1 & 1) == 1)
        ++transitions1;
      int num1 = transitions1 + 2;
      if ((transitions2 & 1) == 1)
        ++transitions2;
      int num2 = transitions2 + 2;
      ResultPoint resultPoint10;
      BitMatrix bits;
      if (4 * num1 >= 7 * num2 || 4 * num2 >= 7 * num1)
      {
        resultPoint10 = this.correctTopRightRectangular(bottomLeft, resultPoint7, resultPoint8, resultPoint9, num1, num2) ?? resultPoint9;
        int transitions3 = this.transitionsBetween(resultPoint8, resultPoint10).Transitions;
        int transitions4 = this.transitionsBetween(resultPoint7, resultPoint10).Transitions;
        if ((transitions3 & 1) == 1)
          ++transitions3;
        if ((transitions4 & 1) == 1)
          ++transitions4;
        bits = ZXing.Datamatrix.Internal.Detector.sampleGrid(this.image, resultPoint8, bottomLeft, resultPoint7, resultPoint10, transitions3, transitions4);
      }
      else
      {
        int dimension = Math.Min(num2, num1);
        resultPoint10 = this.correctTopRight(bottomLeft, resultPoint7, resultPoint8, resultPoint9, dimension) ?? resultPoint9;
        int num3 = Math.Max(this.transitionsBetween(resultPoint8, resultPoint10).Transitions, this.transitionsBetween(resultPoint7, resultPoint10).Transitions) + 1;
        if ((num3 & 1) == 1)
          ++num3;
        bits = ZXing.Datamatrix.Internal.Detector.sampleGrid(this.image, resultPoint8, bottomLeft, resultPoint7, resultPoint10, num3, num3);
      }
      if (bits == null)
        return (DetectorResult) null;
      return new DetectorResult(bits, new ResultPoint[4]
      {
        resultPoint8,
        bottomLeft,
        resultPoint7,
        resultPoint10
      });
    }

    /// <summary>
    /// Calculates the position of the white top right module using the output of the rectangle detector
    /// for a rectangular matrix
    /// </summary>
    private ResultPoint correctTopRightRectangular(
      ResultPoint bottomLeft,
      ResultPoint bottomRight,
      ResultPoint topLeft,
      ResultPoint topRight,
      int dimensionTop,
      int dimensionRight)
    {
      float num1 = (float) ZXing.Datamatrix.Internal.Detector.distance(bottomLeft, bottomRight) / (float) dimensionTop;
      int num2 = ZXing.Datamatrix.Internal.Detector.distance(topLeft, topRight);
      if (num2 == 0)
        return (ResultPoint) null;
      float num3 = (topRight.X - topLeft.X) / (float) num2;
      float num4 = (topRight.Y - topLeft.Y) / (float) num2;
      ResultPoint resultPoint1 = new ResultPoint(topRight.X + num1 * num3, topRight.Y + num1 * num4);
      float num5 = (float) ZXing.Datamatrix.Internal.Detector.distance(bottomLeft, topLeft) / (float) dimensionRight;
      int num6 = ZXing.Datamatrix.Internal.Detector.distance(bottomRight, topRight);
      if (num6 == 0)
        return (ResultPoint) null;
      float num7 = (topRight.X - bottomRight.X) / (float) num6;
      float num8 = (topRight.Y - bottomRight.Y) / (float) num6;
      ResultPoint resultPoint2 = new ResultPoint(topRight.X + num5 * num7, topRight.Y + num5 * num8);
      return !this.isValid(resultPoint1) ? (this.isValid(resultPoint2) ? resultPoint2 : (ResultPoint) null) : (!this.isValid(resultPoint2) || Math.Abs(dimensionTop - this.transitionsBetween(topLeft, resultPoint1).Transitions) + Math.Abs(dimensionRight - this.transitionsBetween(bottomRight, resultPoint1).Transitions) <= Math.Abs(dimensionTop - this.transitionsBetween(topLeft, resultPoint2).Transitions) + Math.Abs(dimensionRight - this.transitionsBetween(bottomRight, resultPoint2).Transitions) ? resultPoint1 : resultPoint2);
    }

    /// <summary>
    /// Calculates the position of the white top right module using the output of the rectangle detector
    /// for a square matrix
    /// </summary>
    private ResultPoint correctTopRight(
      ResultPoint bottomLeft,
      ResultPoint bottomRight,
      ResultPoint topLeft,
      ResultPoint topRight,
      int dimension)
    {
      float num1 = (float) ZXing.Datamatrix.Internal.Detector.distance(bottomLeft, bottomRight) / (float) dimension;
      int num2 = ZXing.Datamatrix.Internal.Detector.distance(topLeft, topRight);
      if (num2 == 0)
        return (ResultPoint) null;
      float num3 = (topRight.X - topLeft.X) / (float) num2;
      float num4 = (topRight.Y - topLeft.Y) / (float) num2;
      ResultPoint resultPoint1 = new ResultPoint(topRight.X + num1 * num3, topRight.Y + num1 * num4);
      float num5 = (float) ZXing.Datamatrix.Internal.Detector.distance(bottomLeft, topLeft) / (float) dimension;
      int num6 = ZXing.Datamatrix.Internal.Detector.distance(bottomRight, topRight);
      if (num6 == 0)
        return (ResultPoint) null;
      float num7 = (topRight.X - bottomRight.X) / (float) num6;
      float num8 = (topRight.Y - bottomRight.Y) / (float) num6;
      ResultPoint resultPoint2 = new ResultPoint(topRight.X + num5 * num7, topRight.Y + num5 * num8);
      return !this.isValid(resultPoint1) ? (this.isValid(resultPoint2) ? resultPoint2 : (ResultPoint) null) : (!this.isValid(resultPoint2) || Math.Abs(this.transitionsBetween(topLeft, resultPoint1).Transitions - this.transitionsBetween(bottomRight, resultPoint1).Transitions) <= Math.Abs(this.transitionsBetween(topLeft, resultPoint2).Transitions - this.transitionsBetween(bottomRight, resultPoint2).Transitions) ? resultPoint1 : resultPoint2);
    }

    private bool isValid(ResultPoint p)
    {
      return (double) p.X >= 0.0 && (double) p.X < (double) this.image.Width && (double) p.Y > 0.0 && (double) p.Y < (double) this.image.Height;
    }

    private static int distance(ResultPoint a, ResultPoint b)
    {
      return MathUtils.round(ResultPoint.distance(a, b));
    }

    /// <summary>Increments the Integer associated with a key by one.</summary>
    private static void increment(IDictionary<ResultPoint, int> table, ResultPoint key)
    {
      if (table.ContainsKey(key))
      {
        int num = table[key];
        table[key] = num + 1;
      }
      else
        table[key] = 1;
    }

    private static BitMatrix sampleGrid(
      BitMatrix image,
      ResultPoint topLeft,
      ResultPoint bottomLeft,
      ResultPoint bottomRight,
      ResultPoint topRight,
      int dimensionX,
      int dimensionY)
    {
      return GridSampler.Instance.sampleGrid(image, dimensionX, dimensionY, 0.5f, 0.5f, (float) dimensionX - 0.5f, 0.5f, (float) dimensionX - 0.5f, (float) dimensionY - 0.5f, 0.5f, (float) dimensionY - 0.5f, topLeft.X, topLeft.Y, topRight.X, topRight.Y, bottomRight.X, bottomRight.Y, bottomLeft.X, bottomLeft.Y);
    }

    /// <summary>
    /// Counts the number of black/white transitions between two points, using something like Bresenham's algorithm.
    /// </summary>
    private ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions transitionsBetween(
      ResultPoint from,
      ResultPoint to)
    {
      int num1 = (int) from.X;
      int num2 = (int) from.Y;
      int num3 = (int) to.X;
      int num4 = (int) to.Y;
      bool flag1 = Math.Abs(num4 - num2) > Math.Abs(num3 - num1);
      if (flag1)
      {
        int num5 = num1;
        num1 = num2;
        num2 = num5;
        int num6 = num3;
        num3 = num4;
        num4 = num6;
      }
      int num7 = Math.Abs(num3 - num1);
      int num8 = Math.Abs(num4 - num2);
      int num9 = -num7 >> 1;
      int num10 = num2 < num4 ? 1 : -1;
      int num11 = num1 < num3 ? 1 : -1;
      int transitions = 0;
      bool flag2 = this.image[flag1 ? num2 : num1, flag1 ? num1 : num2];
      int num12 = num1;
      int num13 = num2;
      for (; num12 != num3; num12 += num11)
      {
        bool flag3 = this.image[flag1 ? num13 : num12, flag1 ? num12 : num13];
        if (flag3 != flag2)
        {
          ++transitions;
          flag2 = flag3;
        }
        num9 += num8;
        if (num9 > 0)
        {
          if (num13 != num4)
          {
            num13 += num10;
            num9 -= num7;
          }
          else
            break;
        }
      }
      return new ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions(from, to, transitions);
    }

    /// <summary>
    /// Simply encapsulates two points and a number of transitions between them.
    /// </summary>
    private sealed class ResultPointsAndTransitions
    {
      public ResultPoint From { get; private set; }

      public ResultPoint To { get; private set; }

      public int Transitions { get; private set; }

      public ResultPointsAndTransitions(ResultPoint from, ResultPoint to, int transitions)
      {
        this.From = from;
        this.To = to;
        this.Transitions = transitions;
      }

      public override string ToString()
      {
        return this.From.ToString() + "/" + (object) this.To + (object) '/' + (object) this.Transitions;
      }
    }

    /// <summary>
    /// Orders ResultPointsAndTransitions by number of transitions, ascending.
    /// </summary>
    private sealed class ResultPointsAndTransitionsComparator : 
      IComparer<ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions>
    {
      public int Compare(
        ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions o1,
        ZXing.Datamatrix.Internal.Detector.ResultPointsAndTransitions o2)
      {
        return o1.Transitions - o2.Transitions;
      }
    }
  }
}
