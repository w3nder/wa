// Decompiled with JetBrains decompiler
// Type: ZXing.Common.BitMatrix
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Text;
using System.Windows.Media.Imaging;

#nullable disable
namespace ZXing.Common
{
  /// <summary>
  ///   <p>Represents a 2D matrix of bits. In function arguments below, and throughout the common
  /// module, x is the column position, and y is the row position. The ordering is always x, y.
  /// The origin is at the top-left.</p>
  ///   <p>Internally the bits are represented in a 1-D array of 32-bit ints. However, each row begins
  /// with a new int. This is done intentionally so that we can copy out a row into a BitArray very
  /// efficiently.</p>
  ///   <p>The ordering of bits is row-major. Within each int, the least significant bits are used first,
  /// meaning they represent lower x values. This is compatible with BitArray's implementation.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  public sealed class BitMatrix
  {
    private readonly int width;
    private readonly int height;
    private readonly int rowSize;
    private readonly int[] bits;

    /// <returns>The width of the matrix</returns>
    public int Width => this.width;

    /// <returns>The height of the matrix</returns>
    public int Height => this.height;

    /// <summary> This method is for compatibility with older code. It's only logical to call if the matrix
    /// is square, so I'm throwing if that's not the case.
    /// 
    /// </summary>
    /// <returns>row/column dimension of this matrix</returns>
    public int Dimension
    {
      get
      {
        return this.width == this.height ? this.width : throw new ArgumentException("Can't call getDimension() on a non-square matrix");
      }
    }

    public BitMatrix(int dimension)
      : this(dimension, dimension)
    {
    }

    public BitMatrix(int width, int height)
    {
      this.width = width >= 1 && height >= 1 ? width : throw new ArgumentException("Both dimensions must be greater than 0");
      this.height = height;
      this.rowSize = width + 31 >> 5;
      this.bits = new int[this.rowSize * height];
    }

    private BitMatrix(int width, int height, int rowSize, int[] bits)
    {
      this.width = width;
      this.height = height;
      this.rowSize = rowSize;
      this.bits = bits;
    }

    /// <summary> <p>Gets the requested bit, where true means black.</p>
    /// 
    /// </summary>
    /// <param name="x">The horizontal component (i.e. which column)</param>
    /// <param name="y">The vertical component (i.e. which row)</param>
    /// <returns>value of given bit in matrix</returns>
    public bool this[int x, int y]
    {
      get => (this.bits[y * this.rowSize + (x >> 5)] >>> (x & 31) & 1) != 0;
      set
      {
        if (!value)
          return;
        this.bits[y * this.rowSize + (x >> 5)] |= 1 << x;
      }
    }

    /// <summary> <p>Flips the given bit.</p>
    /// 
    /// </summary>
    /// <param name="x">The horizontal component (i.e. which column)</param>
    /// <param name="y">The vertical component (i.e. which row)</param>
    public void flip(int x, int y) => this.bits[y * this.rowSize + (x >> 5)] ^= 1 << x;

    /// <summary> Clears all bits (sets to false).</summary>
    public void clear()
    {
      int length = this.bits.Length;
      for (int index = 0; index < length; ++index)
        this.bits[index] = 0;
    }

    /// <summary> <p>Sets a square region of the bit matrix to true.</p>
    /// 
    /// </summary>
    /// <param name="left">The horizontal position to begin at (inclusive)</param>
    /// <param name="top">The vertical position to begin at (inclusive)</param>
    /// <param name="width">The width of the region</param>
    /// <param name="height">The height of the region</param>
    public void setRegion(int left, int top, int width, int height)
    {
      if (top < 0 || left < 0)
        throw new ArgumentException("Left and top must be nonnegative");
      if (height < 1 || width < 1)
        throw new ArgumentException("Height and width must be at least 1");
      int num1 = left + width;
      int num2 = top + height;
      if (num2 > this.height || num1 > this.width)
        throw new ArgumentException("The region must fit inside the matrix");
      for (int index1 = top; index1 < num2; ++index1)
      {
        int num3 = index1 * this.rowSize;
        for (int index2 = left; index2 < num1; ++index2)
          this.bits[num3 + (index2 >> 5)] |= 1 << index2;
      }
    }

    /// <summary> A fast method to retrieve one row of data from the matrix as a BitArray.
    /// 
    /// </summary>
    /// <param name="y">The row to retrieve</param>
    /// <param name="row">An optional caller-allocated BitArray, will be allocated if null or too small
    /// </param>
    /// <returns> The resulting BitArray - this reference should always be used even when passing
    /// your own row
    /// </returns>
    public BitArray getRow(int y, BitArray row)
    {
      if (row == null || row.Size < this.width)
        row = new BitArray(this.width);
      else
        row.clear();
      int num = y * this.rowSize;
      for (int index = 0; index < this.rowSize; ++index)
        row.setBulk(index << 5, this.bits[num + index]);
      return row;
    }

    /// <summary>Sets the row.</summary>
    /// <param name="y">row to set</param>
    /// <param name="row">{@link BitArray} to copy from</param>
    public void setRow(int y, BitArray row)
    {
      Array.Copy((Array) row.Array, 0, (Array) this.bits, y * this.rowSize, this.rowSize);
    }

    /// <summary>
    /// Modifies this {@code BitMatrix} to represent the same but rotated 180 degrees
    /// </summary>
    public void rotate180()
    {
      int width = this.Width;
      int height = this.Height;
      BitArray row1 = new BitArray(width);
      BitArray row2 = new BitArray(width);
      for (int y = 0; y < (height + 1) / 2; ++y)
      {
        row1 = this.getRow(y, row1);
        row2 = this.getRow(height - 1 - y, row2);
        row1.reverse();
        row2.reverse();
        this.setRow(y, row2);
        this.setRow(height - 1 - y, row1);
      }
    }

    /// <summary>
    /// This is useful in detecting the enclosing rectangle of a 'pure' barcode.
    /// </summary>
    /// <returns>{left,top,width,height} enclosing rectangle of all 1 bits, or null if it is all white</returns>
    public int[] getEnclosingRectangle()
    {
      int num1 = this.width;
      int num2 = this.height;
      int num3 = -1;
      int num4 = -1;
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.rowSize; ++index2)
        {
          int bit = this.bits[index1 * this.rowSize + index2];
          if (bit != 0)
          {
            if (index1 < num2)
              num2 = index1;
            if (index1 > num4)
              num4 = index1;
            if (index2 * 32 < num1)
            {
              int num5 = 0;
              while (bit << 31 - num5 == 0)
                ++num5;
              if (index2 * 32 + num5 < num1)
                num1 = index2 * 32 + num5;
            }
            if (index2 * 32 + 31 > num3)
            {
              int num6 = 31;
              while (bit >>> num6 == 0)
                --num6;
              if (index2 * 32 + num6 > num3)
                num3 = index2 * 32 + num6;
            }
          }
        }
      }
      int num7 = num3 - num1;
      int num8 = num4 - num2;
      if (num7 < 0 || num8 < 0)
        return (int[]) null;
      return new int[4]{ num1, num2, num7, num8 };
    }

    /// <summary>
    /// This is useful in detecting a corner of a 'pure' barcode.
    /// </summary>
    /// <returns>{x,y} coordinate of top-left-most 1 bit, or null if it is all white</returns>
    public int[] getTopLeftOnBit()
    {
      int index = 0;
      while (index < this.bits.Length && this.bits[index] == 0)
        ++index;
      if (index == this.bits.Length)
        return (int[]) null;
      int num1 = index / this.rowSize;
      int num2 = index % this.rowSize << 5;
      int bit = this.bits[index];
      int num3 = 0;
      while (bit << 31 - num3 == 0)
        ++num3;
      return new int[2]{ num2 + num3, num1 };
    }

    public int[] getBottomRightOnBit()
    {
      int index = this.bits.Length - 1;
      while (index >= 0 && this.bits[index] == 0)
        --index;
      if (index < 0)
        return (int[]) null;
      int num1 = index / this.rowSize;
      int num2 = index % this.rowSize << 5;
      int bit = this.bits[index];
      int num3 = 31;
      while (bit >>> num3 == 0)
        --num3;
      return new int[2]{ num2 + num3, num1 };
    }

    public override bool Equals(object obj)
    {
      if (!(obj is BitMatrix))
        return false;
      BitMatrix bitMatrix = (BitMatrix) obj;
      if (this.width != bitMatrix.width || this.height != bitMatrix.height || this.rowSize != bitMatrix.rowSize || this.bits.Length != bitMatrix.bits.Length)
        return false;
      for (int index = 0; index < this.bits.Length; ++index)
      {
        if (this.bits[index] != bitMatrix.bits[index])
          return false;
      }
      return true;
    }

    public override int GetHashCode()
    {
      int hashCode = 31 * (31 * (31 * this.width + this.width) + this.height) + this.rowSize;
      foreach (int bit in this.bits)
        hashCode = 31 * hashCode + bit.GetHashCode();
      return hashCode;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.height * (this.width + 1));
      for (int y = 0; y < this.height; ++y)
      {
        for (int x = 0; x < this.width; ++x)
          stringBuilder.Append(this[x, y] ? "X " : "  ");
        stringBuilder.AppendLine("");
      }
      return stringBuilder.ToString();
    }

    public object Clone()
    {
      return (object) new BitMatrix(this.width, this.height, this.rowSize, (int[]) this.bits.Clone());
    }

    [Obsolete("Use BarcodeWriter instead")]
    public WriteableBitmap ToBitmap() => this.ToBitmap(BarcodeFormat.EAN_8, (string) null);

    /// <summary>Converts this ByteMatrix to a black and white bitmap.</summary>
    /// <returns>A black and white bitmap converted from this ByteMatrix.</returns>
    [Obsolete("Use BarcodeWriter instead")]
    public WriteableBitmap ToBitmap(BarcodeFormat format, string content)
    {
      BarcodeWriter barcodeWriter = new BarcodeWriter();
      barcodeWriter.Format = format;
      return barcodeWriter.Write(content);
    }
  }
}
