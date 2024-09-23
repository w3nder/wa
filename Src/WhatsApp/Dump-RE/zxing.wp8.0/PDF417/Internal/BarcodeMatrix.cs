// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.BarcodeMatrix
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// Holds all of the information for a barcode in a format where it can be easily accessable
  /// 
  /// <author>Jacob Haynes</author>
  /// </summary>
  internal sealed class BarcodeMatrix
  {
    private readonly BarcodeRow[] matrix;
    private int currentRow;
    private readonly int height;
    private readonly int width;

    /// <summary>
    /// <param name="height">the height of the matrix (Rows)</param>
    /// <param name="width">the width of the matrix (Cols)</param>
    /// </summary>
    internal BarcodeMatrix(int height, int width)
    {
      this.matrix = new BarcodeRow[height];
      int index = 0;
      for (int length = this.matrix.Length; index < length; ++index)
        this.matrix[index] = new BarcodeRow((width + 4) * 17 + 1);
      this.width = width * 17;
      this.height = height;
      this.currentRow = -1;
    }

    internal void set(int x, int y, sbyte value) => this.matrix[y][x] = value;

    internal void startRow() => ++this.currentRow;

    internal BarcodeRow getCurrentRow() => this.matrix[this.currentRow];

    internal sbyte[][] getMatrix() => this.getScaledMatrix(1, 1);

    internal sbyte[][] getScaledMatrix(int xScale, int yScale)
    {
      sbyte[][] scaledMatrix = new sbyte[this.height * yScale][];
      for (int index = 0; index < this.height * yScale; ++index)
        scaledMatrix[index] = new sbyte[this.width * xScale];
      int num = this.height * yScale;
      for (int index = 0; index < num; ++index)
        scaledMatrix[num - index - 1] = this.matrix[index / yScale].getScaledRow(xScale);
      return scaledMatrix;
    }
  }
}
