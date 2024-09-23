// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.FinderPatternInfo
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// <p>Encapsulates information about finder patterns in an image, including the location of
  /// the three finder patterns, and their estimated module size.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class FinderPatternInfo
  {
    private readonly FinderPattern bottomLeft;
    private readonly FinderPattern topLeft;
    private readonly FinderPattern topRight;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.FinderPatternInfo" /> class.
    /// </summary>
    /// <param name="patternCenters">The pattern centers.</param>
    public FinderPatternInfo(FinderPattern[] patternCenters)
    {
      this.bottomLeft = patternCenters[0];
      this.topLeft = patternCenters[1];
      this.topRight = patternCenters[2];
    }

    /// <summary>Gets the bottom left.</summary>
    public FinderPattern BottomLeft => this.bottomLeft;

    /// <summary>Gets the top left.</summary>
    public FinderPattern TopLeft => this.topLeft;

    /// <summary>Gets the top right.</summary>
    public FinderPattern TopRight => this.topRight;
  }
}
