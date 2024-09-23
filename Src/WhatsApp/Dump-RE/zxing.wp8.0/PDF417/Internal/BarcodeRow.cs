// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.BarcodeRow
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// <author>Jacob Haynes</author>
  /// </summary>
  internal sealed class BarcodeRow
  {
    private readonly sbyte[] row;
    private int currentLocation;

    /// <summary>Creates a Barcode row of the width</summary>
    /// <param name="width">The width.</param>
    internal BarcodeRow(int width)
    {
      this.row = new sbyte[width];
      this.currentLocation = 0;
    }

    /// <summary>
    /// Sets a specific location in the bar
    /// 
    /// <param name="x">The location in the bar</param>
    /// <param name="value">Black if true, white if false;</param>
    /// </summary>
    internal sbyte this[int x]
    {
      get => this.row[x];
      set => this.row[x] = value;
    }

    /// <summary>
    /// Sets a specific location in the bar
    /// 
    /// <param name="x">The location in the bar</param>
    /// <param name="black">Black if true, white if false;</param>
    /// </summary>
    internal void set(int x, bool black) => this.row[x] = black ? (sbyte) 1 : (sbyte) 0;

    /// <summary>
    /// <param name="black">A boolean which is true if the bar black false if it is white</param>
    /// <param name="width">How many spots wide the bar is.</param>
    /// </summary>
    internal void addBar(bool black, int width)
    {
      for (int index = 0; index < width; ++index)
        this.set(this.currentLocation++, black);
    }

    /// <summary>
    /// This function scales the row
    /// 
    /// <param name="scale">How much you want the image to be scaled, must be greater than or equal to 1.</param>
    /// <returns>the scaled row</returns>
    /// </summary>
    internal sbyte[] getScaledRow(int scale)
    {
      sbyte[] scaledRow = new sbyte[this.row.Length * scale];
      for (int index = 0; index < scaledRow.Length; ++index)
        scaledRow[index] = this.row[index / scale];
      return scaledRow;
    }
  }
}
