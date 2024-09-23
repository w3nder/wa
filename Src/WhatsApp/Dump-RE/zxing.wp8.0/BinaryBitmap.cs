// Decompiled with JetBrains decompiler
// Type: ZXing.BinaryBitmap
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing
{
  /// <summary> This class is the core bitmap class used by ZXing to represent 1 bit data. Reader objects
  /// accept a BinaryBitmap and attempt to decode it.
  /// 
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class BinaryBitmap
  {
    private Binarizer binarizer;
    private BitMatrix matrix;

    public BinaryBitmap(Binarizer binarizer)
    {
      this.binarizer = binarizer != null ? binarizer : throw new ArgumentException("Binarizer must be non-null.");
    }

    /// <returns>The width of the bitmap.</returns>
    public int Width => this.binarizer.Width;

    /// <returns>The height of the bitmap.</returns>
    public int Height => this.binarizer.Height;

    /// <summary> Converts one row of luminance data to 1 bit data. May actually do the conversion, or return
    /// cached data. Callers should assume this method is expensive and call it as seldom as possible.
    /// This method is intended for decoding 1D barcodes and may choose to apply sharpening.
    /// 
    /// </summary>
    /// <param name="y">The row to fetch, 0 &lt;= y &lt; bitmap height.</param>
    /// <param name="row">An optional preallocated array. If null or too small, it will be ignored.
    /// If used, the Binarizer will call BitArray.clear(). Always use the returned object.
    /// </param>
    /// <returns>The array of bits for this row (true means black).</returns>
    public BitArray getBlackRow(int y, BitArray row) => this.binarizer.getBlackRow(y, row);

    /// <summary> Converts a 2D array of luminance data to 1 bit. As above, assume this method is expensive
    /// and do not call it repeatedly. This method is intended for decoding 2D barcodes and may or
    /// may not apply sharpening. Therefore, a row from this matrix may not be identical to one
    /// fetched using getBlackRow(), so don't mix and match between them.
    /// 
    /// </summary>
    /// <returns>The 2D array of bits for the image (true means black).</returns>
    public BitMatrix BlackMatrix
    {
      get
      {
        if (this.matrix == null)
          this.matrix = this.binarizer.BlackMatrix;
        return this.matrix;
      }
    }

    /// <returns>Whether this bitmap can be cropped.</returns>
    public bool CropSupported => this.binarizer.LuminanceSource.CropSupported;

    /// <summary> Returns a new object with cropped image data. Implementations may keep a reference to the
    /// original data rather than a copy. Only callable if isCropSupported() is true.
    /// 
    /// </summary>
    /// <param name="left">The left coordinate, 0 &lt;= left &lt; getWidth().</param>
    /// <param name="top">The top coordinate, 0 &lt;= top &lt;= getHeight().</param>
    /// <param name="width">The width of the rectangle to crop.</param>
    /// <param name="height">The height of the rectangle to crop.</param>
    /// <returns>A cropped version of this object.</returns>
    public BinaryBitmap crop(int left, int top, int width, int height)
    {
      return new BinaryBitmap(this.binarizer.createBinarizer(this.binarizer.LuminanceSource.crop(left, top, width, height)));
    }

    /// <returns>Whether this bitmap supports counter-clockwise rotation.</returns>
    public bool RotateSupported => this.binarizer.LuminanceSource.RotateSupported;

    /// <summary>
    /// Returns a new object with rotated image data by 90 degrees counterclockwise.
    /// Only callable if {@link #isRotateSupported()} is true.
    /// </summary>
    /// <returns>A rotated version of this object.</returns>
    public BinaryBitmap rotateCounterClockwise()
    {
      return new BinaryBitmap(this.binarizer.createBinarizer(this.binarizer.LuminanceSource.rotateCounterClockwise()));
    }

    /// <summary>
    /// Returns a new object with rotated image data by 45 degrees counterclockwise.
    /// Only callable if {@link #isRotateSupported()} is true.
    /// </summary>
    /// <returns>A rotated version of this object.</returns>
    public BinaryBitmap rotateCounterClockwise45()
    {
      return new BinaryBitmap(this.binarizer.createBinarizer(this.binarizer.LuminanceSource.rotateCounterClockwise45()));
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      BitMatrix blackMatrix = this.BlackMatrix;
      return blackMatrix == null ? string.Empty : blackMatrix.ToString();
    }
  }
}
