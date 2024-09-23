// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.Dimensions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// Data object to specify the minimum and maximum number of rows and columns for a PDF417 barcode.
  /// @author qwandor@google.com (Andrew Walbran)
  /// </summary>
  public sealed class Dimensions
  {
    private readonly int minCols;
    private readonly int maxCols;
    private readonly int minRows;
    private readonly int maxRows;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.Dimensions" /> class.
    /// </summary>
    /// <param name="minCols">The min cols.</param>
    /// <param name="maxCols">The max cols.</param>
    /// <param name="minRows">The min rows.</param>
    /// <param name="maxRows">The max rows.</param>
    public Dimensions(int minCols, int maxCols, int minRows, int maxRows)
    {
      this.minCols = minCols;
      this.maxCols = maxCols;
      this.minRows = minRows;
      this.maxRows = maxRows;
    }

    /// <summary>Gets the min cols.</summary>
    public int MinCols => this.minCols;

    /// <summary>Gets the max cols.</summary>
    public int MaxCols => this.maxCols;

    /// <summary>Gets the min rows.</summary>
    public int MinRows => this.minRows;

    /// <summary>Gets the max rows.</summary>
    public int MaxRows => this.maxRows;
  }
}
