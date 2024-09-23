// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.FinderPattern
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// <p>Encapsulates a finder pattern, which are the three square patterns found in
  /// the corners of QR Codes. It also encapsulates a count of similar finder patterns,
  /// as a convenience to the finder's bookkeeping.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class FinderPattern : ResultPoint
  {
    private readonly float estimatedModuleSize;
    private int count;

    internal FinderPattern(float posX, float posY, float estimatedModuleSize)
      : this(posX, posY, estimatedModuleSize, 1)
    {
      this.estimatedModuleSize = estimatedModuleSize;
      this.count = 1;
    }

    internal FinderPattern(float posX, float posY, float estimatedModuleSize, int count)
      : base(posX, posY)
    {
      this.estimatedModuleSize = estimatedModuleSize;
      this.count = count;
    }

    /// <summary>Gets the size of the estimated module.</summary>
    /// <value>The size of the estimated module.</value>
    public float EstimatedModuleSize => this.estimatedModuleSize;

    internal int Count => this.count;

    /// <summary> <p>Determines if this finder pattern "about equals" a finder pattern at the stated
    /// position and size -- meaning, it is at nearly the same center with nearly the same size.</p>
    /// </summary>
    internal bool aboutEquals(float moduleSize, float i, float j)
    {
      if ((double) Math.Abs(i - this.Y) > (double) moduleSize || (double) Math.Abs(j - this.X) > (double) moduleSize)
        return false;
      float num = Math.Abs(moduleSize - this.estimatedModuleSize);
      return (double) num <= 1.0 || (double) num <= (double) this.estimatedModuleSize;
    }

    /// <summary>
    /// Combines this object's current estimate of a finder pattern position and module size
    /// with a new estimate. It returns a new {@code FinderPattern} containing a weighted average
    /// based on count.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <param name="j">The j.</param>
    /// <param name="newModuleSize">New size of the module.</param>
    /// <returns></returns>
    internal FinderPattern combineEstimate(float i, float j, float newModuleSize)
    {
      int count = this.count + 1;
      return new FinderPattern(((float) this.count * this.X + j) / (float) count, ((float) this.count * this.Y + i) / (float) count, ((float) this.count * this.estimatedModuleSize + newModuleSize) / (float) count, count);
    }
  }
}
