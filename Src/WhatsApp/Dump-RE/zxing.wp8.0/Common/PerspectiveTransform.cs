// Decompiled with JetBrains decompiler
// Type: ZXing.Common.PerspectiveTransform
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common
{
  /// <summary> <p>This class implements a perspective transform in two dimensions. Given four source and four
  /// destination points, it will compute the transformation implied between them. The code is based
  /// directly upon section 3.4.2 of George Wolberg's "Digital Image Warping"; see pages 54-56.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class PerspectiveTransform
  {
    private float a11;
    private float a12;
    private float a13;
    private float a21;
    private float a22;
    private float a23;
    private float a31;
    private float a32;
    private float a33;

    private PerspectiveTransform(
      float a11,
      float a21,
      float a31,
      float a12,
      float a22,
      float a32,
      float a13,
      float a23,
      float a33)
    {
      this.a11 = a11;
      this.a12 = a12;
      this.a13 = a13;
      this.a21 = a21;
      this.a22 = a22;
      this.a23 = a23;
      this.a31 = a31;
      this.a32 = a32;
      this.a33 = a33;
    }

    public static PerspectiveTransform quadrilateralToQuadrilateral(
      float x0,
      float y0,
      float x1,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3,
      float x0p,
      float y0p,
      float x1p,
      float y1p,
      float x2p,
      float y2p,
      float x3p,
      float y3p)
    {
      PerspectiveTransform square = PerspectiveTransform.quadrilateralToSquare(x0, y0, x1, y1, x2, y2, x3, y3);
      return PerspectiveTransform.squareToQuadrilateral(x0p, y0p, x1p, y1p, x2p, y2p, x3p, y3p).times(square);
    }

    public void transformPoints(float[] points)
    {
      int length = points.Length;
      float a11 = this.a11;
      float a12 = this.a12;
      float a13 = this.a13;
      float a21 = this.a21;
      float a22 = this.a22;
      float a23 = this.a23;
      float a31 = this.a31;
      float a32 = this.a32;
      float a33 = this.a33;
      for (int index = 0; index < length; index += 2)
      {
        float point1 = points[index];
        float point2 = points[index + 1];
        float num = (float) ((double) a13 * (double) point1 + (double) a23 * (double) point2) + a33;
        points[index] = ((float) ((double) a11 * (double) point1 + (double) a21 * (double) point2) + a31) / num;
        points[index + 1] = ((float) ((double) a12 * (double) point1 + (double) a22 * (double) point2) + a32) / num;
      }
    }

    /// <summary>Convenience method, not optimized for performance. </summary>
    public void transformPoints(float[] xValues, float[] yValues)
    {
      int length = xValues.Length;
      for (int index = 0; index < length; ++index)
      {
        float xValue = xValues[index];
        float yValue = yValues[index];
        float num = (float) ((double) this.a13 * (double) xValue + (double) this.a23 * (double) yValue) + this.a33;
        xValues[index] = ((float) ((double) this.a11 * (double) xValue + (double) this.a21 * (double) yValue) + this.a31) / num;
        yValues[index] = ((float) ((double) this.a12 * (double) xValue + (double) this.a22 * (double) yValue) + this.a32) / num;
      }
    }

    public static PerspectiveTransform squareToQuadrilateral(
      float x0,
      float y0,
      float x1,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3)
    {
      float num1 = x0 - x1 + x2 - x3;
      float num2 = y0 - y1 + y2 - y3;
      if ((double) num1 == 0.0 && (double) num2 == 0.0)
        return new PerspectiveTransform(x1 - x0, x2 - x1, x0, y1 - y0, y2 - y1, y0, 0.0f, 0.0f, 1f);
      float num3 = x1 - x2;
      float num4 = x3 - x2;
      float num5 = y1 - y2;
      float num6 = y3 - y2;
      float num7 = (float) ((double) num3 * (double) num6 - (double) num4 * (double) num5);
      float a13 = (float) ((double) num1 * (double) num6 - (double) num4 * (double) num2) / num7;
      float a23 = (float) ((double) num3 * (double) num2 - (double) num1 * (double) num5) / num7;
      return new PerspectiveTransform((float) ((double) x1 - (double) x0 + (double) a13 * (double) x1), (float) ((double) x3 - (double) x0 + (double) a23 * (double) x3), x0, (float) ((double) y1 - (double) y0 + (double) a13 * (double) y1), (float) ((double) y3 - (double) y0 + (double) a23 * (double) y3), y0, a13, a23, 1f);
    }

    public static PerspectiveTransform quadrilateralToSquare(
      float x0,
      float y0,
      float x1,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3)
    {
      return PerspectiveTransform.squareToQuadrilateral(x0, y0, x1, y1, x2, y2, x3, y3).buildAdjoint();
    }

    internal PerspectiveTransform buildAdjoint()
    {
      return new PerspectiveTransform((float) ((double) this.a22 * (double) this.a33 - (double) this.a23 * (double) this.a32), (float) ((double) this.a23 * (double) this.a31 - (double) this.a21 * (double) this.a33), (float) ((double) this.a21 * (double) this.a32 - (double) this.a22 * (double) this.a31), (float) ((double) this.a13 * (double) this.a32 - (double) this.a12 * (double) this.a33), (float) ((double) this.a11 * (double) this.a33 - (double) this.a13 * (double) this.a31), (float) ((double) this.a12 * (double) this.a31 - (double) this.a11 * (double) this.a32), (float) ((double) this.a12 * (double) this.a23 - (double) this.a13 * (double) this.a22), (float) ((double) this.a13 * (double) this.a21 - (double) this.a11 * (double) this.a23), (float) ((double) this.a11 * (double) this.a22 - (double) this.a12 * (double) this.a21));
    }

    internal PerspectiveTransform times(PerspectiveTransform other)
    {
      return new PerspectiveTransform((float) ((double) this.a11 * (double) other.a11 + (double) this.a21 * (double) other.a12 + (double) this.a31 * (double) other.a13), (float) ((double) this.a11 * (double) other.a21 + (double) this.a21 * (double) other.a22 + (double) this.a31 * (double) other.a23), (float) ((double) this.a11 * (double) other.a31 + (double) this.a21 * (double) other.a32 + (double) this.a31 * (double) other.a33), (float) ((double) this.a12 * (double) other.a11 + (double) this.a22 * (double) other.a12 + (double) this.a32 * (double) other.a13), (float) ((double) this.a12 * (double) other.a21 + (double) this.a22 * (double) other.a22 + (double) this.a32 * (double) other.a23), (float) ((double) this.a12 * (double) other.a31 + (double) this.a22 * (double) other.a32 + (double) this.a32 * (double) other.a33), (float) ((double) this.a13 * (double) other.a11 + (double) this.a23 * (double) other.a12 + (double) this.a33 * (double) other.a13), (float) ((double) this.a13 * (double) other.a21 + (double) this.a23 * (double) other.a22 + (double) this.a33 * (double) other.a23), (float) ((double) this.a13 * (double) other.a31 + (double) this.a23 * (double) other.a32 + (double) this.a33 * (double) other.a33));
    }
  }
}
