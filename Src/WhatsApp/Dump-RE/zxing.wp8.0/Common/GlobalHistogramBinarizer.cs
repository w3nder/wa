// Decompiled with JetBrains decompiler
// Type: ZXing.Common.GlobalHistogramBinarizer
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common
{
  /// <summary> This Binarizer implementation uses the old ZXing global histogram approach. It is suitable
  /// for low-end mobile devices which don't have enough CPU or memory to use a local thresholding
  /// algorithm. However, because it picks a global black point, it cannot handle difficult shadows
  /// and gradients.
  /// 
  /// Faster mobile devices and all desktop applications should probably use HybridBinarizer instead.
  /// 
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>Sean Owen</author>
  /// </summary>
  public class GlobalHistogramBinarizer : Binarizer
  {
    private const int LUMINANCE_BITS = 5;
    private const int LUMINANCE_SHIFT = 3;
    private const int LUMINANCE_BUCKETS = 32;
    private static readonly byte[] EMPTY = new byte[0];
    private byte[] luminances;
    private readonly int[] buckets;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.GlobalHistogramBinarizer" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    public GlobalHistogramBinarizer(LuminanceSource source)
      : base(source)
    {
      this.luminances = GlobalHistogramBinarizer.EMPTY;
      this.buckets = new int[32];
    }

    /// <summary>
    /// Applies simple sharpening to the row data to improve performance of the 1D Readers.
    /// </summary>
    /// <param name="y"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public override BitArray getBlackRow(int y, BitArray row)
    {
      LuminanceSource luminanceSource = this.LuminanceSource;
      int width = luminanceSource.Width;
      if (row == null || row.Size < width)
        row = new BitArray(width);
      else
        row.clear();
      this.initArrays(width);
      byte[] row1 = luminanceSource.getRow(y, this.luminances);
      int[] buckets = this.buckets;
      for (int index = 0; index < width; ++index)
      {
        int num = (int) row1[index] & (int) byte.MaxValue;
        ++buckets[num >> 3];
      }
      int blackPoint;
      if (!GlobalHistogramBinarizer.estimateBlackPoint(buckets, out blackPoint))
        return (BitArray) null;
      int num1 = (int) row1[0] & (int) byte.MaxValue;
      int num2 = (int) row1[1] & (int) byte.MaxValue;
      for (int i = 1; i < width - 1; ++i)
      {
        int num3 = (int) row1[i + 1] & (int) byte.MaxValue;
        int num4 = (num2 << 2) - num1 - num3 >> 1;
        row[i] = num4 < blackPoint;
        num1 = num2;
        num2 = num3;
      }
      return row;
    }

    /// <summary>
    /// Does not sharpen the data, as this call is intended to only be used by 2D Readers.
    /// </summary>
    public override BitMatrix BlackMatrix
    {
      get
      {
        LuminanceSource luminanceSource = this.LuminanceSource;
        int width = luminanceSource.Width;
        int height = luminanceSource.Height;
        BitMatrix blackMatrix = new BitMatrix(width, height);
        this.initArrays(width);
        int[] buckets = this.buckets;
        for (int index1 = 1; index1 < 5; ++index1)
        {
          int y = height * index1 / 5;
          byte[] row = luminanceSource.getRow(y, this.luminances);
          int num1 = (width << 2) / 5;
          for (int index2 = width / 5; index2 < num1; ++index2)
          {
            int num2 = (int) row[index2] & (int) byte.MaxValue;
            ++buckets[num2 >> 3];
          }
        }
        int blackPoint;
        if (!GlobalHistogramBinarizer.estimateBlackPoint(buckets, out blackPoint))
          return (BitMatrix) null;
        byte[] matrix = luminanceSource.Matrix;
        for (int y = 0; y < height; ++y)
        {
          int num3 = y * width;
          for (int x = 0; x < width; ++x)
          {
            int num4 = (int) matrix[num3 + x] & (int) byte.MaxValue;
            blackMatrix[x, y] = num4 < blackPoint;
          }
        }
        return blackMatrix;
      }
    }

    /// <summary>
    /// Creates a new object with the same type as this Binarizer implementation, but with pristine
    /// state. This is needed because Binarizer implementations may be stateful, e.g. keeping a cache
    /// of 1 bit data. See Effective Java for why we can't use Java's clone() method.
    /// </summary>
    /// <param name="source">The LuminanceSource this Binarizer will operate on.</param>
    /// <returns>A new concrete Binarizer implementation object.</returns>
    public override Binarizer createBinarizer(LuminanceSource source)
    {
      return (Binarizer) new GlobalHistogramBinarizer(source);
    }

    private void initArrays(int luminanceSize)
    {
      if (this.luminances.Length < luminanceSize)
        this.luminances = new byte[luminanceSize];
      for (int index = 0; index < 32; ++index)
        this.buckets[index] = 0;
    }

    private static bool estimateBlackPoint(int[] buckets, out int blackPoint)
    {
      blackPoint = 0;
      int length = buckets.Length;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (int index = 0; index < length; ++index)
      {
        if (buckets[index] > num3)
        {
          num2 = index;
          num3 = buckets[index];
        }
        if (buckets[index] > num1)
          num1 = buckets[index];
      }
      int num4 = 0;
      int num5 = 0;
      for (int index = 0; index < length; ++index)
      {
        int num6 = index - num2;
        int num7 = buckets[index] * num6 * num6;
        if (num7 > num5)
        {
          num4 = index;
          num5 = num7;
        }
      }
      if (num2 > num4)
      {
        int num8 = num2;
        num2 = num4;
        num4 = num8;
      }
      if (num4 - num2 <= length >> 4)
        return false;
      int num9 = num4 - 1;
      int num10 = -1;
      for (int index = num4 - 1; index > num2; --index)
      {
        int num11 = index - num2;
        int num12 = num11 * num11 * (num4 - index) * (num1 - buckets[index]);
        if (num12 > num10)
        {
          num9 = index;
          num10 = num12;
        }
      }
      blackPoint = num9 << 3;
      return true;
    }
  }
}
