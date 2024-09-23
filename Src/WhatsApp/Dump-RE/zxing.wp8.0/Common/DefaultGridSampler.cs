// Decompiled with JetBrains decompiler
// Type: ZXing.Common.DefaultGridSampler
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.Common
{
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class DefaultGridSampler : GridSampler
  {
    public override BitMatrix sampleGrid(
      BitMatrix image,
      int dimensionX,
      int dimensionY,
      float p1ToX,
      float p1ToY,
      float p2ToX,
      float p2ToY,
      float p3ToX,
      float p3ToY,
      float p4ToX,
      float p4ToY,
      float p1FromX,
      float p1FromY,
      float p2FromX,
      float p2FromY,
      float p3FromX,
      float p3FromY,
      float p4FromX,
      float p4FromY)
    {
      PerspectiveTransform quadrilateral = PerspectiveTransform.quadrilateralToQuadrilateral(p1ToX, p1ToY, p2ToX, p2ToY, p3ToX, p3ToY, p4ToX, p4ToY, p1FromX, p1FromY, p2FromX, p2FromY, p3FromX, p3FromY, p4FromX, p4FromY);
      return this.sampleGrid(image, dimensionX, dimensionY, quadrilateral);
    }

    public override BitMatrix sampleGrid(
      BitMatrix image,
      int dimensionX,
      int dimensionY,
      PerspectiveTransform transform)
    {
      if (dimensionX <= 0 || dimensionY <= 0)
        return (BitMatrix) null;
      BitMatrix bitMatrix = new BitMatrix(dimensionX, dimensionY);
      float[] points = new float[dimensionX << 1];
      for (int y = 0; y < dimensionY; ++y)
      {
        int length = points.Length;
        float num = (float) y + 0.5f;
        for (int index = 0; index < length; index += 2)
        {
          points[index] = (float) (index >> 1) + 0.5f;
          points[index + 1] = num;
        }
        transform.transformPoints(points);
        if (!GridSampler.checkAndNudgePoints(image, points))
          return (BitMatrix) null;
        try
        {
          for (int index = 0; index < length; index += 2)
            bitMatrix[index >> 1, y] = image[(int) points[index], (int) points[index + 1]];
        }
        catch (IndexOutOfRangeException ex)
        {
          return (BitMatrix) null;
        }
      }
      return bitMatrix;
    }
  }
}
