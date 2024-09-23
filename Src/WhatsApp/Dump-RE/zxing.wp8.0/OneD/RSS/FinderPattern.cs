// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.FinderPattern
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD.RSS
{
  /// <summary>
  /// 
  /// </summary>
  public sealed class FinderPattern
  {
    /// <summary>Gets the value.</summary>
    public int Value { get; private set; }

    /// <summary>Gets the start end.</summary>
    public int[] StartEnd { get; private set; }

    /// <summary>Gets the result points.</summary>
    public ResultPoint[] ResultPoints { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.RSS.FinderPattern" /> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="startEnd">The start end.</param>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="rowNumber">The row number.</param>
    public FinderPattern(int value, int[] startEnd, int start, int end, int rowNumber)
    {
      this.Value = value;
      this.StartEnd = startEnd;
      this.ResultPoints = new ResultPoint[2]
      {
        new ResultPoint((float) start, (float) rowNumber),
        new ResultPoint((float) end, (float) rowNumber)
      };
    }

    /// <summary>
    /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="o">The <see cref="T:System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object o)
    {
      return o is FinderPattern && this.Value == ((FinderPattern) o).Value;
    }

    /// <summary>Returns a hash code for this instance.</summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => this.Value;
  }
}
