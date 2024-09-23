// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.Internal.AztecDetectorResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Aztec.Internal
{
  /// <summary>
  /// The class contains all information about the Aztec code which was found
  /// </summary>
  public class AztecDetectorResult : DetectorResult
  {
    /// <summary>
    /// Gets a value indicating whether this Aztec code is compact.
    /// </summary>
    /// <value>
    ///   <c>true</c> if compact; otherwise, <c>false</c>.
    /// </value>
    public bool Compact { get; private set; }

    /// <summary>Gets the nb datablocks.</summary>
    public int NbDatablocks { get; private set; }

    /// <summary>Gets the nb layers.</summary>
    public int NbLayers { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Aztec.Internal.AztecDetectorResult" /> class.
    /// </summary>
    /// <param name="bits">The bits.</param>
    /// <param name="points">The points.</param>
    /// <param name="compact">if set to <c>true</c> [compact].</param>
    /// <param name="nbDatablocks">The nb datablocks.</param>
    /// <param name="nbLayers">The nb layers.</param>
    public AztecDetectorResult(
      BitMatrix bits,
      ResultPoint[] points,
      bool compact,
      int nbDatablocks,
      int nbLayers)
      : base(bits, points)
    {
      this.Compact = compact;
      this.NbDatablocks = nbDatablocks;
      this.NbLayers = nbLayers;
    }
  }
}
