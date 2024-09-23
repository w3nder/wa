// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.BitMatrixParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// </summary>
  internal sealed class BitMatrixParser
  {
    private readonly BitMatrix mappingBitMatrix;
    private readonly BitMatrix readMappingMatrix;
    private readonly Version version;

    internal BitMatrixParser(BitMatrix bitMatrix)
    {
      int height = bitMatrix.Height;
      if (height < 8 || height > 144 || (height & 1) != 0)
        return;
      this.version = BitMatrixParser.readVersion(bitMatrix);
      if (this.version == null)
        return;
      this.mappingBitMatrix = this.extractDataRegion(bitMatrix);
      this.readMappingMatrix = new BitMatrix(this.mappingBitMatrix.Width, this.mappingBitMatrix.Height);
    }

    public Version Version => this.version;

    /// <summary>
    /// <p>Creates the version object based on the dimension of the original bit matrix from
    /// the datamatrix code.</p>
    /// 
    /// <p>See ISO 16022:2006 Table 7 - ECC 200 symbol attributes</p>
    /// 
    /// <param name="bitMatrix">Original <see cref="T:ZXing.Common.BitMatrix" />including alignment patterns</param>
    /// <returns><see cref="P:ZXing.Datamatrix.Internal.BitMatrixParser.Version" />encapsulating the Data Matrix Code's "version"</returns>
    /// <exception cref="T:ZXing.FormatException">if the dimensions of the mapping matrix are not valid</exception>
    /// Data Matrix dimensions.
    /// </summary>
    internal static Version readVersion(BitMatrix bitMatrix)
    {
      return Version.getVersionForDimensions(bitMatrix.Height, bitMatrix.Width);
    }

    /// <summary>
    /// <p>Reads the bits in the <see cref="T:ZXing.Common.BitMatrix" />representing the mapping matrix (No alignment patterns)
    /// in the correct order in order to reconstitute the codewords bytes contained within the
    /// Data Matrix Code.</p>
    /// 
    /// <returns>bytes encoded within the Data Matrix Code</returns>
    /// <exception cref="T:ZXing.FormatException">if the exact number of bytes expected is not read</exception>
    /// </summary>
    internal byte[] readCodewords()
    {
      byte[] numArray = new byte[this.version.getTotalCodewords()];
      int num1 = 0;
      int num2 = 4;
      int num3 = 0;
      int height = this.mappingBitMatrix.Height;
      int width = this.mappingBitMatrix.Width;
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      do
      {
        if (num2 == height && num3 == 0 && !flag1)
        {
          numArray[num1++] = (byte) this.readCorner1(height, width);
          num2 -= 2;
          num3 += 2;
          flag1 = true;
        }
        else if (num2 == height - 2 && num3 == 0 && (width & 3) != 0 && !flag2)
        {
          numArray[num1++] = (byte) this.readCorner2(height, width);
          num2 -= 2;
          num3 += 2;
          flag2 = true;
        }
        else if (num2 == height + 4 && num3 == 2 && (width & 7) == 0 && !flag3)
        {
          numArray[num1++] = (byte) this.readCorner3(height, width);
          num2 -= 2;
          num3 += 2;
          flag3 = true;
        }
        else if (num2 == height - 2 && num3 == 0 && (width & 7) == 4 && !flag4)
        {
          numArray[num1++] = (byte) this.readCorner4(height, width);
          num2 -= 2;
          num3 += 2;
          flag4 = true;
        }
        else
        {
          do
          {
            if (num2 < height && num3 >= 0 && !this.readMappingMatrix[num3, num2])
              numArray[num1++] = (byte) this.readUtah(num2, num3, height, width);
            num2 -= 2;
            num3 += 2;
          }
          while (num2 >= 0 && num3 < width);
          int num4 = num2 + 1;
          int num5 = num3 + 3;
          do
          {
            if (num4 >= 0 && num5 < width && !this.readMappingMatrix[num5, num4])
              numArray[num1++] = (byte) this.readUtah(num4, num5, height, width);
            num4 += 2;
            num5 -= 2;
          }
          while (num4 < height && num5 >= 0);
          num2 = num4 + 3;
          num3 = num5 + 1;
        }
      }
      while (num2 < height || num3 < width);
      return num1 != this.version.getTotalCodewords() ? (byte[]) null : numArray;
    }

    /// <summary>
    /// <p>Reads a bit of the mapping matrix accounting for boundary wrapping.</p>
    /// 
    /// <param name="row">Row to read in the mapping matrix</param>
    /// <param name="column">Column to read in the mapping matrix</param>
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>value of the given bit in the mapping matrix</returns>
    /// </summary>
    private bool readModule(int row, int column, int numRows, int numColumns)
    {
      if (row < 0)
      {
        row += numRows;
        column += 4 - (numRows + 4 & 7);
      }
      if (column < 0)
      {
        column += numColumns;
        row += 4 - (numColumns + 4 & 7);
      }
      this.readMappingMatrix[column, row] = true;
      return this.mappingBitMatrix[column, row];
    }

    /// <summary>
    /// <p>Reads the 8 bits of the standard Utah-shaped pattern.</p>
    /// 
    /// <p>See ISO 16022:2006, 5.8.1 Figure 6</p>
    /// 
    /// <param name="row">Current row in the mapping matrix, anchored at the 8th bit (LSB) of the pattern</param>
    /// <param name="column">Current column in the mapping matrix, anchored at the 8th bit (LSB) of the pattern</param>
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>byte from the utah shape</returns>
    /// </summary>
    private int readUtah(int row, int column, int numRows, int numColumns)
    {
      int num1 = 0;
      if (this.readModule(row - 2, column - 2, numRows, numColumns))
        num1 |= 1;
      int num2 = num1 << 1;
      if (this.readModule(row - 2, column - 1, numRows, numColumns))
        num2 |= 1;
      int num3 = num2 << 1;
      if (this.readModule(row - 1, column - 2, numRows, numColumns))
        num3 |= 1;
      int num4 = num3 << 1;
      if (this.readModule(row - 1, column - 1, numRows, numColumns))
        num4 |= 1;
      int num5 = num4 << 1;
      if (this.readModule(row - 1, column, numRows, numColumns))
        num5 |= 1;
      int num6 = num5 << 1;
      if (this.readModule(row, column - 2, numRows, numColumns))
        num6 |= 1;
      int num7 = num6 << 1;
      if (this.readModule(row, column - 1, numRows, numColumns))
        num7 |= 1;
      int num8 = num7 << 1;
      if (this.readModule(row, column, numRows, numColumns))
        num8 |= 1;
      return num8;
    }

    /// <summary>
    /// <p>Reads the 8 bits of the special corner condition 1.</p>
    /// 
    /// <p>See ISO 16022:2006, Figure F.3</p>
    /// 
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>byte from the Corner condition 1</returns>
    /// </summary>
    private int readCorner1(int numRows, int numColumns)
    {
      int num1 = 0;
      if (this.readModule(numRows - 1, 0, numRows, numColumns))
        num1 |= 1;
      int num2 = num1 << 1;
      if (this.readModule(numRows - 1, 1, numRows, numColumns))
        num2 |= 1;
      int num3 = num2 << 1;
      if (this.readModule(numRows - 1, 2, numRows, numColumns))
        num3 |= 1;
      int num4 = num3 << 1;
      if (this.readModule(0, numColumns - 2, numRows, numColumns))
        num4 |= 1;
      int num5 = num4 << 1;
      if (this.readModule(0, numColumns - 1, numRows, numColumns))
        num5 |= 1;
      int num6 = num5 << 1;
      if (this.readModule(1, numColumns - 1, numRows, numColumns))
        num6 |= 1;
      int num7 = num6 << 1;
      if (this.readModule(2, numColumns - 1, numRows, numColumns))
        num7 |= 1;
      int num8 = num7 << 1;
      if (this.readModule(3, numColumns - 1, numRows, numColumns))
        num8 |= 1;
      return num8;
    }

    /// <summary>
    /// <p>Reads the 8 bits of the special corner condition 2.</p>
    /// 
    /// <p>See ISO 16022:2006, Figure F.4</p>
    /// 
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>byte from the Corner condition 2</returns>
    /// </summary>
    private int readCorner2(int numRows, int numColumns)
    {
      int num1 = 0;
      if (this.readModule(numRows - 3, 0, numRows, numColumns))
        num1 |= 1;
      int num2 = num1 << 1;
      if (this.readModule(numRows - 2, 0, numRows, numColumns))
        num2 |= 1;
      int num3 = num2 << 1;
      if (this.readModule(numRows - 1, 0, numRows, numColumns))
        num3 |= 1;
      int num4 = num3 << 1;
      if (this.readModule(0, numColumns - 4, numRows, numColumns))
        num4 |= 1;
      int num5 = num4 << 1;
      if (this.readModule(0, numColumns - 3, numRows, numColumns))
        num5 |= 1;
      int num6 = num5 << 1;
      if (this.readModule(0, numColumns - 2, numRows, numColumns))
        num6 |= 1;
      int num7 = num6 << 1;
      if (this.readModule(0, numColumns - 1, numRows, numColumns))
        num7 |= 1;
      int num8 = num7 << 1;
      if (this.readModule(1, numColumns - 1, numRows, numColumns))
        num8 |= 1;
      return num8;
    }

    /// <summary>
    /// <p>Reads the 8 bits of the special corner condition 3.</p>
    /// 
    /// <p>See ISO 16022:2006, Figure F.5</p>
    /// 
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>byte from the Corner condition 3</returns>
    /// </summary>
    private int readCorner3(int numRows, int numColumns)
    {
      int num1 = 0;
      if (this.readModule(numRows - 1, 0, numRows, numColumns))
        num1 |= 1;
      int num2 = num1 << 1;
      if (this.readModule(numRows - 1, numColumns - 1, numRows, numColumns))
        num2 |= 1;
      int num3 = num2 << 1;
      if (this.readModule(0, numColumns - 3, numRows, numColumns))
        num3 |= 1;
      int num4 = num3 << 1;
      if (this.readModule(0, numColumns - 2, numRows, numColumns))
        num4 |= 1;
      int num5 = num4 << 1;
      if (this.readModule(0, numColumns - 1, numRows, numColumns))
        num5 |= 1;
      int num6 = num5 << 1;
      if (this.readModule(1, numColumns - 3, numRows, numColumns))
        num6 |= 1;
      int num7 = num6 << 1;
      if (this.readModule(1, numColumns - 2, numRows, numColumns))
        num7 |= 1;
      int num8 = num7 << 1;
      if (this.readModule(1, numColumns - 1, numRows, numColumns))
        num8 |= 1;
      return num8;
    }

    /// <summary>
    /// <p>Reads the 8 bits of the special corner condition 4.</p>
    /// 
    /// <p>See ISO 16022:2006, Figure F.6</p>
    /// 
    /// <param name="numRows">Number of rows in the mapping matrix</param>
    /// <param name="numColumns">Number of columns in the mapping matrix</param>
    /// <returns>byte from the Corner condition 4</returns>
    /// </summary>
    private int readCorner4(int numRows, int numColumns)
    {
      int num1 = 0;
      if (this.readModule(numRows - 3, 0, numRows, numColumns))
        num1 |= 1;
      int num2 = num1 << 1;
      if (this.readModule(numRows - 2, 0, numRows, numColumns))
        num2 |= 1;
      int num3 = num2 << 1;
      if (this.readModule(numRows - 1, 0, numRows, numColumns))
        num3 |= 1;
      int num4 = num3 << 1;
      if (this.readModule(0, numColumns - 2, numRows, numColumns))
        num4 |= 1;
      int num5 = num4 << 1;
      if (this.readModule(0, numColumns - 1, numRows, numColumns))
        num5 |= 1;
      int num6 = num5 << 1;
      if (this.readModule(1, numColumns - 1, numRows, numColumns))
        num6 |= 1;
      int num7 = num6 << 1;
      if (this.readModule(2, numColumns - 1, numRows, numColumns))
        num7 |= 1;
      int num8 = num7 << 1;
      if (this.readModule(3, numColumns - 1, numRows, numColumns))
        num8 |= 1;
      return num8;
    }

    /// <summary>
    /// <p>Extracts the data region from a <see cref="T:ZXing.Common.BitMatrix" />that contains
    /// alignment patterns.</p>
    /// 
    /// <param name="bitMatrix">Original <see cref="T:ZXing.Common.BitMatrix" />with alignment patterns</param>
    /// <returns>BitMatrix that has the alignment patterns removed</returns>
    /// </summary>
    private BitMatrix extractDataRegion(BitMatrix bitMatrix)
    {
      int symbolSizeRows = this.version.getSymbolSizeRows();
      int symbolSizeColumns = this.version.getSymbolSizeColumns();
      if (bitMatrix.Height != symbolSizeRows)
        throw new ArgumentException("Dimension of bitMarix must match the version size");
      int dataRegionSizeRows = this.version.getDataRegionSizeRows();
      int regionSizeColumns = this.version.getDataRegionSizeColumns();
      int num1 = symbolSizeRows / dataRegionSizeRows;
      int num2 = symbolSizeColumns / regionSizeColumns;
      int height = num1 * dataRegionSizeRows;
      BitMatrix dataRegion = new BitMatrix(num2 * regionSizeColumns, height);
      for (int index1 = 0; index1 < num1; ++index1)
      {
        int num3 = index1 * dataRegionSizeRows;
        for (int index2 = 0; index2 < num2; ++index2)
        {
          int num4 = index2 * regionSizeColumns;
          for (int index3 = 0; index3 < dataRegionSizeRows; ++index3)
          {
            int y1 = index1 * (dataRegionSizeRows + 2) + 1 + index3;
            int y2 = num3 + index3;
            for (int index4 = 0; index4 < regionSizeColumns; ++index4)
            {
              int x1 = index2 * (regionSizeColumns + 2) + 1 + index4;
              if (bitMatrix[x1, y1])
              {
                int x2 = num4 + index4;
                dataRegion[x2, y2] = true;
              }
            }
          }
        }
      }
      return dataRegion;
    }
  }
}
