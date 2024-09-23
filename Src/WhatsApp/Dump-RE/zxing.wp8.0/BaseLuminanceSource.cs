// Decompiled with JetBrains decompiler
// Type: ZXing.BaseLuminanceSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// The base class for luminance sources which supports
  /// cropping and rotating based upon the luminance values.
  /// </summary>
  public abstract class BaseLuminanceSource : LuminanceSource
  {
    protected const int RChannelWeight = 19562;
    protected const int GChannelWeight = 38550;
    protected const int BChannelWeight = 7424;
    protected const int ChannelWeight = 16;
    /// <summary>
    /// 
    /// </summary>
    protected byte[] luminances;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BaseLuminanceSource" /> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected BaseLuminanceSource(int width, int height)
      : base(width, height)
    {
      this.luminances = new byte[width * height];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BaseLuminanceSource" /> class.
    /// </summary>
    /// <param name="luminanceArray">The luminance array.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected BaseLuminanceSource(byte[] luminanceArray, int width, int height)
      : base(width, height)
    {
      this.luminances = new byte[width * height];
      Buffer.BlockCopy((Array) luminanceArray, 0, (Array) this.luminances, 0, width * height);
    }

    /// <summary>
    /// Fetches one row of luminance data from the underlying platform's bitmap. Values range from
    /// 0 (black) to 255 (white). It is preferable for implementations of this method
    /// to only fetch this row rather than the whole image, since no 2D Readers may be installed and
    /// getMatrix() may never be called.
    /// </summary>
    /// <param name="y">The row to fetch, 0 &lt;= y &lt; Height.</param>
    /// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
    /// Always use the returned object, and ignore the .length of the array.</param>
    /// <returns>An array containing the luminance data.</returns>
    public override byte[] getRow(int y, byte[] row)
    {
      int width = this.Width;
      if (row == null || row.Length < width)
        row = new byte[width];
      for (int index = 0; index < width; ++index)
        row[index] = this.luminances[y * width + index];
      return row;
    }

    public override byte[] Matrix => this.luminances;

    /// <summary>
    /// Returns a new object with rotated image data by 90 degrees counterclockwise.
    /// Only callable if {@link #isRotateSupported()} is true.
    /// </summary>
    /// <returns>A rotated version of this object.</returns>
    public override LuminanceSource rotateCounterClockwise()
    {
      byte[] newLuminances = new byte[this.Width * this.Height];
      int height = this.Height;
      int width = this.Width;
      byte[] matrix = this.Matrix;
      for (int index1 = 0; index1 < this.Height; ++index1)
      {
        for (int index2 = 0; index2 < this.Width; ++index2)
        {
          int num1 = width - index2 - 1;
          int num2 = index1;
          newLuminances[num1 * height + num2] = matrix[index1 * this.Width + index2];
        }
      }
      return this.CreateLuminanceSource(newLuminances, height, width);
    }

    /// <summary>TODO: not implemented yet</summary>
    /// <returns>A rotated version of this object.</returns>
    public override LuminanceSource rotateCounterClockwise45() => base.rotateCounterClockwise45();

    /// <summary>
    /// </summary>
    /// <returns> Whether this subclass supports counter-clockwise rotation.</returns>
    public override bool RotateSupported => true;

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
      if (left + width > this.Width || top + height > this.Height)
        throw new ArgumentException("Crop rectangle does not fit within image data.");
      byte[] newLuminances = new byte[width * height];
      byte[] matrix = this.Matrix;
      int width1 = this.Width;
      int num1 = left + width;
      int num2 = top + height;
      int num3 = top;
      int num4 = 0;
      while (num3 < num2)
      {
        int num5 = left;
        int num6 = 0;
        while (num5 < num1)
        {
          newLuminances[num4 * width + num6] = matrix[num3 * width1 + num5];
          ++num5;
          ++num6;
        }
        ++num3;
        ++num4;
      }
      return this.CreateLuminanceSource(newLuminances, width, height);
    }

    /// <summary>
    /// </summary>
    /// <returns> Whether this subclass supports cropping.</returns>
    public override bool CropSupported => true;

    /// <summary>
    /// </summary>
    /// <returns>Whether this subclass supports invertion.</returns>
    public override bool InversionSupported => true;

    /// <summary>
    /// Inverts the luminance values (newValue = 255 - oldValue)
    /// </summary>
    public override LuminanceSource invert()
    {
      return (LuminanceSource) new InvertedLuminanceSource((LuminanceSource) this);
    }

    /// <summary>
    /// Should create a new luminance source with the right class type.
    /// The method is used in methods crop and rotate.
    /// </summary>
    /// <param name="newLuminances">The new luminances.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    protected abstract LuminanceSource CreateLuminanceSource(
      byte[] newLuminances,
      int width,
      int height);
  }
}
