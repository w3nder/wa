// Decompiled with JetBrains decompiler
// Type: ZXing.ResultPoint
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text;
using ZXing.Common.Detector;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Encapsulates a point of interest in an image containing a barcode. Typically, this
  /// would be the location of a finder pattern or the corner of the barcode, for example.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source</author>
  public class ResultPoint
  {
    private readonly float x;
    private readonly float y;
    private readonly byte[] bytesX;
    private readonly byte[] bytesY;
    private string toString;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.ResultPoint" /> class.
    /// </summary>
    public ResultPoint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.ResultPoint" /> class.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    public ResultPoint(float x, float y)
    {
      this.x = x;
      this.y = y;
      this.bytesX = BitConverter.GetBytes(x);
      this.bytesY = BitConverter.GetBytes(y);
    }

    /// <summary>Gets the X.</summary>
    public virtual float X => this.x;

    /// <summary>Gets the Y.</summary>
    public virtual float Y => this.y;

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="T:System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object other)
    {
      return other is ResultPoint resultPoint && (double) this.x == (double) resultPoint.x && (double) this.y == (double) resultPoint.y;
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode()
    {
      return 31 * (((int) this.bytesX[0] << 24) + ((int) this.bytesX[1] << 16) + ((int) this.bytesX[2] << 8) + (int) this.bytesX[3]) + ((int) this.bytesY[0] << 24) + ((int) this.bytesY[1] << 16) + ((int) this.bytesY[2] << 8) + (int) this.bytesY[3];
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      if (this.toString == null)
      {
        StringBuilder stringBuilder = new StringBuilder(25);
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.CurrentUICulture, "({0}, {1})", (object) this.x, (object) this.y);
        this.toString = stringBuilder.ToString();
      }
      return this.toString;
    }

    /// <summary>
    /// Orders an array of three ResultPoints in an order [A,B,C] such that AB &lt; AC and
    /// BC &lt; AC and the angle between BC and BA is less than 180 degrees.
    /// </summary>
    public static void orderBestPatterns(ResultPoint[] patterns)
    {
      float num1 = ResultPoint.distance(patterns[0], patterns[1]);
      float num2 = ResultPoint.distance(patterns[1], patterns[2]);
      float num3 = ResultPoint.distance(patterns[0], patterns[2]);
      ResultPoint pattern;
      ResultPoint pointA;
      ResultPoint pointC;
      if ((double) num2 >= (double) num1 && (double) num2 >= (double) num3)
      {
        pattern = patterns[0];
        pointA = patterns[1];
        pointC = patterns[2];
      }
      else if ((double) num3 >= (double) num2 && (double) num3 >= (double) num1)
      {
        pattern = patterns[1];
        pointA = patterns[0];
        pointC = patterns[2];
      }
      else
      {
        pattern = patterns[2];
        pointA = patterns[0];
        pointC = patterns[1];
      }
      if ((double) ResultPoint.crossProductZ(pointA, pattern, pointC) < 0.0)
      {
        ResultPoint resultPoint = pointA;
        pointA = pointC;
        pointC = resultPoint;
      }
      patterns[0] = pointA;
      patterns[1] = pattern;
      patterns[2] = pointC;
    }

    /// <returns>distance between two points</returns>
    public static float distance(ResultPoint pattern1, ResultPoint pattern2)
    {
      return MathUtils.distance(pattern1.x, pattern1.y, pattern2.x, pattern2.y);
    }

    /// <summary>
    /// Returns the z component of the cross product between vectors BC and BA.
    /// </summary>
    private static float crossProductZ(ResultPoint pointA, ResultPoint pointB, ResultPoint pointC)
    {
      float x = pointB.x;
      float y = pointB.y;
      return (float) (((double) pointC.x - (double) x) * ((double) pointA.y - (double) y) - ((double) pointC.y - (double) y) * ((double) pointA.x - (double) x));
    }
  }
}
