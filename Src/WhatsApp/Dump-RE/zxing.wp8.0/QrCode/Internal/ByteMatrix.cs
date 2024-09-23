// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.ByteMatrix
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// JAVAPORT: The original code was a 2D array of ints, but since it only ever gets assigned
  /// 0, 1 and 2 I'm going to use less memory and go with bytes.
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  public sealed class ByteMatrix
  {
    private readonly byte[][] bytes;
    private readonly int width;
    private readonly int height;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.ByteMatrix" /> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public ByteMatrix(int width, int height)
    {
      this.bytes = new byte[height][];
      for (int index = 0; index < height; ++index)
        this.bytes[index] = new byte[width];
      this.width = width;
      this.height = height;
    }

    /// <summary>Gets the height.</summary>
    public int Height => this.height;

    /// <summary>Gets the width.</summary>
    public int Width => this.width;

    /// <summary>
    /// Gets or sets the <see cref="T:System.Int32" /> with the specified x.
    /// </summary>
    public int this[int x, int y]
    {
      get => (int) this.bytes[y][x];
      set => this.bytes[y][x] = (byte) value;
    }

    /// <summary>
    /// an internal representation as bytes, in row-major order. array[y][x] represents point (x,y)
    /// </summary>
    public byte[][] Array => this.bytes;

    /// <summary>Sets the specified x.</summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="value">The value.</param>
    public void set(int x, int y, byte value) => this.bytes[y][x] = value;

    /// <summary>Sets the specified x.</summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public void set(int x, int y, bool value) => this.bytes[y][x] = value ? (byte) 1 : (byte) 0;

    /// <summary>Clears the specified value.</summary>
    /// <param name="value">The value.</param>
    public void clear(byte value)
    {
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.width; ++index2)
          this.bytes[index1][index2] = value;
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(2 * this.width * this.height + 2);
      for (int index1 = 0; index1 < this.height; ++index1)
      {
        for (int index2 = 0; index2 < this.width; ++index2)
        {
          switch (this.bytes[index1][index2])
          {
            case 0:
              stringBuilder.Append(" 0");
              break;
            case 1:
              stringBuilder.Append(" 1");
              break;
            default:
              stringBuilder.Append("  ");
              break;
          }
        }
        stringBuilder.Append('\n');
      }
      return stringBuilder.ToString();
    }
  }
}
