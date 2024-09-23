// Decompiled with JetBrains decompiler
// Type: ZXing.PlanarYUVLuminanceSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// This object extends LuminanceSource around an array of YUV data returned from the camera driver,
  /// with the option to crop to a rectangle within the full data. This can be used to exclude
  /// superfluous pixels around the perimeter and speed up decoding.
  /// It works for any pixel format where the Y channel is planar and appears first, including
  /// YCbCr_420_SP and YCbCr_422_SP.
  /// @author dswitkin@google.com (Daniel Switkin)
  /// </summary>
  public sealed class PlanarYUVLuminanceSource : BaseLuminanceSource
  {
    private const int THUMBNAIL_SCALE_FACTOR = 2;
    private readonly byte[] yuvData;
    private readonly int dataWidth;
    private readonly int dataHeight;
    private readonly int left;
    private readonly int top;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PlanarYUVLuminanceSource" /> class.
    /// </summary>
    /// <param name="yuvData">The yuv data.</param>
    /// <param name="dataWidth">Width of the data.</param>
    /// <param name="dataHeight">Height of the data.</param>
    /// <param name="left">The left.</param>
    /// <param name="top">The top.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="reverseHoriz">if set to <c>true</c> [reverse horiz].</param>
    public PlanarYUVLuminanceSource(
      byte[] yuvData,
      int dataWidth,
      int dataHeight,
      int left,
      int top,
      int width,
      int height,
      bool reverseHoriz)
      : base(width, height)
    {
      if (left + width > dataWidth || top + height > dataHeight)
        throw new ArgumentException("Crop rectangle does not fit within image data.");
      this.yuvData = yuvData;
      this.dataWidth = dataWidth;
      this.dataHeight = dataHeight;
      this.left = left;
      this.top = top;
      if (!reverseHoriz)
        return;
      this.reverseHorizontal(width, height);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PlanarYUVLuminanceSource" /> class.
    /// </summary>
    /// <param name="luminances">The luminances.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    private PlanarYUVLuminanceSource(byte[] luminances, int width, int height)
      : base(width, height)
    {
      this.yuvData = luminances;
      this.luminances = luminances;
      this.dataWidth = width;
      this.dataHeight = height;
      this.left = 0;
      this.top = 0;
    }

    /// <summary>
    /// Fetches one row of luminance data from the underlying platform's bitmap. Values range from
    /// 0 (black) to 255 (white). Because Java does not have an unsigned byte type, callers will have
    /// to bitwise and with 0xff for each value. It is preferable for implementations of this method
    /// to only fetch this row rather than the whole image, since no 2D Readers may be installed and
    /// getMatrix() may never be called.
    /// </summary>
    /// <param name="y">The row to fetch, 0 &lt;= y &lt; Height.</param>
    /// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
    /// Always use the returned object, and ignore the .length of the array.</param>
    /// <returns>An array containing the luminance data.</returns>
    public override byte[] getRow(int y, byte[] row)
    {
      if (y < 0 || y >= this.Height)
        throw new ArgumentException("Requested row is outside the image: " + (object) y);
      int width = this.Width;
      if (row == null || row.Length < width)
        row = new byte[width];
      Array.Copy((Array) this.yuvData, (y + this.top) * this.dataWidth + this.left, (Array) row, 0, width);
      return row;
    }

    /// <summary>
    /// 
    /// </summary>
    public override byte[] Matrix
    {
      get
      {
        int width = this.Width;
        int height = this.Height;
        if (width == this.dataWidth && height == this.dataHeight)
          return this.yuvData;
        int length = width * height;
        byte[] destinationArray = new byte[length];
        int sourceIndex = this.top * this.dataWidth + this.left;
        if (width == this.dataWidth)
        {
          Array.Copy((Array) this.yuvData, sourceIndex, (Array) destinationArray, 0, length);
          return destinationArray;
        }
        byte[] yuvData = this.yuvData;
        for (int index = 0; index < height; ++index)
        {
          int destinationIndex = index * width;
          Array.Copy((Array) yuvData, sourceIndex, (Array) destinationArray, destinationIndex, width);
          sourceIndex += this.dataWidth;
        }
        return destinationArray;
      }
    }

    /// <summary>
    /// </summary>
    /// <returns> Whether this subclass supports cropping.</returns>
    public override bool CropSupported => true;

    /// <summary>
    /// Returns a new object with cropped image data. Implementations may keep a reference to the
    /// original data rather than a copy. Only callable if CropSupported is true.
    /// </summary>
    /// <param name="left">The left coordinate, 0 &lt;= left &lt; Width.</param>
    /// <param name="top">The top coordinate, 0 &lt;= top &lt;= Height.</param>
    /// <param name="width">The width of the rectangle to crop.</param>
    /// <param name="height">The height of the rectangle to crop.</param>
    /// <returns>A cropped version of this object.</returns>
    public override LuminanceSource crop(int left, int top, int width, int height)
    {
      return (LuminanceSource) new PlanarYUVLuminanceSource(this.yuvData, this.dataWidth, this.dataHeight, this.left + left, this.top + top, width, height, false);
    }

    /// <summary>Renders the cropped greyscale bitmap.</summary>
    /// <returns></returns>
    public int[] renderThumbnail()
    {
      int num1 = this.Width / 2;
      int num2 = this.Height / 2;
      int[] numArray = new int[num1 * num2];
      byte[] yuvData = this.yuvData;
      int num3 = this.top * this.dataWidth + this.left;
      for (int index1 = 0; index1 < num2; ++index1)
      {
        int num4 = index1 * num1;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          int num5 = (int) yuvData[num3 + index2 * 2] & (int) byte.MaxValue;
          numArray[num4 + index2] = -16777216 | num5 * 65793;
        }
        num3 += this.dataWidth * 2;
      }
      return numArray;
    }

    /// <summary>width of image from {@link #renderThumbnail()}</summary>
    public int ThumbnailWidth => this.Width / 2;

    /// <summary>height of image from {@link #renderThumbnail()}</summary>
    public int ThumbnailHeight => this.Height / 2;

    private void reverseHorizontal(int width, int height)
    {
      byte[] yuvData = this.yuvData;
      int num1 = 0;
      int num2 = this.top * this.dataWidth + this.left;
      while (num1 < height)
      {
        int num3 = num2 + width / 2;
        int index1 = num2;
        int index2 = num2 + width - 1;
        while (index1 < num3)
        {
          byte num4 = yuvData[index1];
          yuvData[index1] = yuvData[index2];
          yuvData[index2] = num4;
          ++index1;
          --index2;
        }
        ++num1;
        num2 += this.dataWidth;
      }
    }

    protected override LuminanceSource CreateLuminanceSource(
      byte[] newLuminances,
      int width,
      int height)
    {
      return (LuminanceSource) new PlanarYUVLuminanceSource(newLuminances, width, height);
    }
  }
}
