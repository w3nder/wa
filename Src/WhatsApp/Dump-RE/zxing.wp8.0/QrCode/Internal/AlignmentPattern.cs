// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.AlignmentPattern
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary> <p>Encapsulates an alignment pattern, which are the smaller square patterns found in
  /// all but the simplest QR Codes.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class AlignmentPattern : ResultPoint
  {
    private float estimatedModuleSize;

    internal AlignmentPattern(float posX, float posY, float estimatedModuleSize)
      : base(posX, posY)
    {
      this.estimatedModuleSize = estimatedModuleSize;
    }

    /// <summary> <p>Determines if this alignment pattern "about equals" an alignment pattern at the stated
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
    /// with a new estimate. It returns a new {@code FinderPattern} containing an average of the two.
    /// </summary>
    /// <param name="i">The i.</param>
    /// <param name="j">The j.</param>
    /// <param name="newModuleSize">New size of the module.</param>
    /// <returns></returns>
    internal AlignmentPattern combineEstimate(float i, float j, float newModuleSize)
    {
      return new AlignmentPattern((float) (((double) this.X + (double) j) / 2.0), (float) (((double) this.Y + (double) i) / 2.0), (float) (((double) this.estimatedModuleSize + (double) newModuleSize) / 2.0));
    }
  }
}
