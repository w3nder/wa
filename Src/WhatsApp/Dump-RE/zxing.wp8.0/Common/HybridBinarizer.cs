// Decompiled with JetBrains decompiler
// Type: ZXing.Common.HybridBinarizer
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common
{
  /// <summary> This class implements a local thresholding algorithm, which while slower than the
  /// GlobalHistogramBinarizer, is fairly efficient for what it does. It is designed for
  /// high frequency images of barcodes with black data on white backgrounds. For this application,
  /// it does a much better job than a global blackpoint with severe shadows and gradients.
  /// However it tends to produce artifacts on lower frequency images and is therefore not
  /// a good general purpose binarizer for uses outside ZXing.
  /// 
  /// This class extends GlobalHistogramBinarizer, using the older histogram approach for 1D readers,
  /// and the newer local approach for 2D readers. 1D decoding using a per-row histogram is already
  /// inherently local, and only fails for horizontal gradients. We can revisit that problem later,
  /// but for now it was not a win to use local blocks for 1D.
  /// 
  /// This Binarizer is the default for the unit tests and the recommended class for library users.
  /// 
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class HybridBinarizer : GlobalHistogramBinarizer
  {
    private const int BLOCK_SIZE_POWER = 3;
    private const int BLOCK_SIZE = 8;
    private const int BLOCK_SIZE_MASK = 7;
    private const int MINIMUM_DIMENSION = 40;
    private const int MIN_DYNAMIC_RANGE = 24;
    private BitMatrix matrix;

    public override BitMatrix BlackMatrix
    {
      get
      {
        this.binarizeEntireImage();
        return this.matrix;
      }
    }

    public HybridBinarizer(LuminanceSource source)
      : base(source)
    {
    }

    public override Binarizer createBinarizer(LuminanceSource source)
    {
      return (Binarizer) new HybridBinarizer(source);
    }

    /// <summary>
    /// Calculates the final BitMatrix once for all requests. This could be called once from the
    /// constructor instead, but there are some advantages to doing it lazily, such as making
    /// profiling easier, and not doing heavy lifting when callers don't expect it.
    /// </summary>
    private void binarizeEntireImage()
    {
      if (this.matrix != null)
        return;
      LuminanceSource luminanceSource = this.LuminanceSource;
      int width = luminanceSource.Width;
      int height = luminanceSource.Height;
      if (width >= 40 && height >= 40)
      {
        byte[] matrix1 = luminanceSource.Matrix;
        int subWidth = width >> 3;
        if ((width & 7) != 0)
          ++subWidth;
        int subHeight = height >> 3;
        if ((height & 7) != 0)
          ++subHeight;
        int[][] blackPoints = HybridBinarizer.calculateBlackPoints(matrix1, subWidth, subHeight, width, height);
        BitMatrix matrix2 = new BitMatrix(width, height);
        HybridBinarizer.calculateThresholdForBlock(matrix1, subWidth, subHeight, width, height, blackPoints, matrix2);
        this.matrix = matrix2;
      }
      else
        this.matrix = base.BlackMatrix;
    }

    /// <summary>
    /// For each 8x8 block in the image, calculate the average black point using a 5x5 grid
    /// of the blocks around it. Also handles the corner cases (fractional blocks are computed based
    /// on the last 8 pixels in the row/column which are also used in the previous block).
    /// </summary>
    /// <param name="luminances">The luminances.</param>
    /// <param name="subWidth">Width of the sub.</param>
    /// <param name="subHeight">Height of the sub.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="blackPoints">The black points.</param>
    /// <param name="matrix">The matrix.</param>
    private static void calculateThresholdForBlock(
      byte[] luminances,
      int subWidth,
      int subHeight,
      int width,
      int height,
      int[][] blackPoints,
      BitMatrix matrix)
    {
      for (int index1 = 0; index1 < subHeight; ++index1)
      {
        int yoffset = index1 << 3;
        int num1 = height - 8;
        if (yoffset > num1)
          yoffset = num1;
        for (int index2 = 0; index2 < subWidth; ++index2)
        {
          int xoffset = index2 << 3;
          int num2 = width - 8;
          if (xoffset > num2)
            xoffset = num2;
          int index3 = HybridBinarizer.cap(index2, 2, subWidth - 3);
          int num3 = HybridBinarizer.cap(index1, 2, subHeight - 3);
          int num4 = 0;
          for (int index4 = -2; index4 <= 2; ++index4)
          {
            int[] blackPoint = blackPoints[num3 + index4];
            num4 = num4 + blackPoint[index3 - 2] + blackPoint[index3 - 1] + blackPoint[index3] + blackPoint[index3 + 1] + blackPoint[index3 + 2];
          }
          int threshold = num4 / 25;
          HybridBinarizer.thresholdBlock(luminances, xoffset, yoffset, threshold, width, matrix);
        }
      }
    }

    private static int cap(int value, int min, int max)
    {
      if (value < min)
        return min;
      return value <= max ? value : max;
    }

    /// <summary>Applies a single threshold to an 8x8 block of pixels.</summary>
    /// <param name="luminances">The luminances.</param>
    /// <param name="xoffset">The xoffset.</param>
    /// <param name="yoffset">The yoffset.</param>
    /// <param name="threshold">The threshold.</param>
    /// <param name="stride">The stride.</param>
    /// <param name="matrix">The matrix.</param>
    private static void thresholdBlock(
      byte[] luminances,
      int xoffset,
      int yoffset,
      int threshold,
      int stride,
      BitMatrix matrix)
    {
      int num1 = yoffset * stride + xoffset;
      int num2 = 0;
      while (num2 < 8)
      {
        for (int index = 0; index < 8; ++index)
        {
          int num3 = (int) luminances[num1 + index] & (int) byte.MaxValue;
          matrix[xoffset + index, yoffset + num2] = num3 <= threshold;
        }
        ++num2;
        num1 += stride;
      }
    }

    /// <summary>
    /// Calculates a single black point for each 8x8 block of pixels and saves it away.
    /// See the following thread for a discussion of this algorithm:
    /// http://groups.google.com/group/zxing/browse_thread/thread/d06efa2c35a7ddc0
    /// </summary>
    /// <param name="luminances">The luminances.</param>
    /// <param name="subWidth">Width of the sub.</param>
    /// <param name="subHeight">Height of the sub.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    private static int[][] calculateBlackPoints(
      byte[] luminances,
      int subWidth,
      int subHeight,
      int width,
      int height)
    {
      int[][] blackPoints = new int[subHeight][];
      for (int index = 0; index < subHeight; ++index)
        blackPoints[index] = new int[subWidth];
      for (int index1 = 0; index1 < subHeight; ++index1)
      {
        int num1 = index1 << 3;
        int num2 = height - 8;
        if (num1 > num2)
          num1 = num2;
        for (int index2 = 0; index2 < subWidth; ++index2)
        {
          int num3 = index2 << 3;
          int num4 = width - 8;
          if (num3 > num4)
            num3 = num4;
          int num5 = 0;
          int num6 = (int) byte.MaxValue;
          int num7 = 0;
          int num8 = 0;
          int num9 = num1 * width + num3;
          while (num8 < 8)
          {
            for (int index3 = 0; index3 < 8; ++index3)
            {
              int num10 = (int) luminances[num9 + index3] & (int) byte.MaxValue;
              num5 += num10;
              if (num10 < num6)
                num6 = num10;
              if (num10 > num7)
                num7 = num10;
            }
            if (num7 - num6 > 24)
            {
              ++num8;
              num9 += width;
              while (num8 < 8)
              {
                for (int index4 = 0; index4 < 8; ++index4)
                  num5 += (int) luminances[num9 + index4] & (int) byte.MaxValue;
                ++num8;
                num9 += width;
              }
            }
            ++num8;
            num9 += width;
          }
          int num11 = num5 >> 6;
          if (num7 - num6 <= 24)
          {
            num11 = num6 >> 1;
            if (index1 > 0 && index2 > 0)
            {
              int num12 = blackPoints[index1 - 1][index2] + 2 * blackPoints[index1][index2 - 1] + blackPoints[index1 - 1][index2 - 1] >> 2;
              if (num6 < num12)
                num11 = num12;
            }
          }
          blackPoints[index1][index2] = num11;
        }
      }
      return blackPoints;
    }
  }
}
