// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Encoder.DefaultPlacement
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Datamatrix.Encoder
{
  /// <summary>
  /// Symbol Character Placement Program. Adapted from Annex M.1 in ISO/IEC 16022:2000(E).
  /// </summary>
  public class DefaultPlacement
  {
    private readonly string codewords;
    private readonly int numrows;
    private readonly int numcols;
    private readonly byte[] bits;

    /// <summary>Main constructor</summary>
    /// <param name="codewords">the codewords to place</param>
    /// <param name="numcols">the number of columns</param>
    /// <param name="numrows">the number of rows</param>
    public DefaultPlacement(string codewords, int numcols, int numrows)
    {
      this.codewords = codewords;
      this.numcols = numcols;
      this.numrows = numrows;
      this.bits = new byte[numcols * numrows];
      SupportClass.Fill<byte>(this.bits, (byte) 2);
    }

    public int Numrows => this.numrows;

    public int Numcols => this.numcols;

    public byte[] Bits => this.bits;

    public bool getBit(int col, int row) => this.bits[row * this.numcols + col] == (byte) 1;

    public void setBit(int col, int row, bool bit)
    {
      this.bits[row * this.numcols + col] = bit ? (byte) 1 : (byte) 0;
    }

    public bool hasBit(int col, int row) => this.bits[row * this.numcols + col] < (byte) 2;

    public void place()
    {
      int num = 0;
      int row1 = 4;
      int col1 = 0;
      do
      {
        if (row1 == this.numrows && col1 == 0)
          this.corner1(num++);
        if (row1 == this.numrows - 2 && col1 == 0 && this.numcols % 4 != 0)
          this.corner2(num++);
        if (row1 == this.numrows - 2 && col1 == 0 && this.numcols % 8 == 4)
          this.corner3(num++);
        if (row1 == this.numrows + 4 && col1 == 2 && this.numcols % 8 == 0)
          this.corner4(num++);
        do
        {
          if (row1 < this.numrows && col1 >= 0 && !this.hasBit(col1, row1))
            this.utah(row1, col1, num++);
          row1 -= 2;
          col1 += 2;
        }
        while (row1 >= 0 && col1 < this.numcols);
        int row2 = row1 + 1;
        int col2 = col1 + 3;
        do
        {
          if (row2 >= 0 && col2 < this.numcols && !this.hasBit(col2, row2))
            this.utah(row2, col2, num++);
          row2 += 2;
          col2 -= 2;
        }
        while (row2 < this.numrows && col2 >= 0);
        row1 = row2 + 3;
        col1 = col2 + 1;
      }
      while (row1 < this.numrows || col1 < this.numcols);
      if (this.hasBit(this.numcols - 1, this.numrows - 1))
        return;
      this.setBit(this.numcols - 1, this.numrows - 1, true);
      this.setBit(this.numcols - 2, this.numrows - 2, true);
    }

    private void module(int row, int col, int pos, int bit)
    {
      if (row < 0)
      {
        row += this.numrows;
        col += 4 - (this.numrows + 4) % 8;
      }
      if (col < 0)
      {
        col += this.numcols;
        row += 4 - (this.numcols + 4) % 8;
      }
      int num = (int) this.codewords[pos] & 1 << 8 - bit;
      this.setBit(col, row, num != 0);
    }

    /// <summary>
    /// Places the 8 bits of a utah-shaped symbol character in ECC200.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="col">The col.</param>
    /// <param name="pos">character position</param>
    private void utah(int row, int col, int pos)
    {
      this.module(row - 2, col - 2, pos, 1);
      this.module(row - 2, col - 1, pos, 2);
      this.module(row - 1, col - 2, pos, 3);
      this.module(row - 1, col - 1, pos, 4);
      this.module(row - 1, col, pos, 5);
      this.module(row, col - 2, pos, 6);
      this.module(row, col - 1, pos, 7);
      this.module(row, col, pos, 8);
    }

    private void corner1(int pos)
    {
      this.module(this.numrows - 1, 0, pos, 1);
      this.module(this.numrows - 1, 1, pos, 2);
      this.module(this.numrows - 1, 2, pos, 3);
      this.module(0, this.numcols - 2, pos, 4);
      this.module(0, this.numcols - 1, pos, 5);
      this.module(1, this.numcols - 1, pos, 6);
      this.module(2, this.numcols - 1, pos, 7);
      this.module(3, this.numcols - 1, pos, 8);
    }

    private void corner2(int pos)
    {
      this.module(this.numrows - 3, 0, pos, 1);
      this.module(this.numrows - 2, 0, pos, 2);
      this.module(this.numrows - 1, 0, pos, 3);
      this.module(0, this.numcols - 4, pos, 4);
      this.module(0, this.numcols - 3, pos, 5);
      this.module(0, this.numcols - 2, pos, 6);
      this.module(0, this.numcols - 1, pos, 7);
      this.module(1, this.numcols - 1, pos, 8);
    }

    private void corner3(int pos)
    {
      this.module(this.numrows - 3, 0, pos, 1);
      this.module(this.numrows - 2, 0, pos, 2);
      this.module(this.numrows - 1, 0, pos, 3);
      this.module(0, this.numcols - 2, pos, 4);
      this.module(0, this.numcols - 1, pos, 5);
      this.module(1, this.numcols - 1, pos, 6);
      this.module(2, this.numcols - 1, pos, 7);
      this.module(3, this.numcols - 1, pos, 8);
    }

    private void corner4(int pos)
    {
      this.module(this.numrows - 1, 0, pos, 1);
      this.module(this.numrows - 1, this.numcols - 1, pos, 2);
      this.module(0, this.numcols - 3, pos, 3);
      this.module(0, this.numcols - 2, pos, 4);
      this.module(0, this.numcols - 1, pos, 5);
      this.module(1, this.numcols - 3, pos, 6);
      this.module(1, this.numcols - 2, pos, 7);
      this.module(1, this.numcols - 1, pos, 8);
    }
  }
}
