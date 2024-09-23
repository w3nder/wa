// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.QRCode
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
  /// <author>dswitkin@google.com (Daniel Switkin) - ported from C++</author>
  public sealed class QRCode
  {
    /// <summary>
    /// 
    /// </summary>
    public static int NUM_MASK_PATTERNS = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.QRCode" /> class.
    /// </summary>
    public QRCode() => this.MaskPattern = -1;

    /// <summary>Gets or sets the mode.</summary>
    /// <value>The mode.</value>
    public Mode Mode { get; set; }

    /// <summary>Gets or sets the EC level.</summary>
    /// <value>The EC level.</value>
    public ErrorCorrectionLevel ECLevel { get; set; }

    /// <summary>Gets or sets the version.</summary>
    /// <value>The version.</value>
    public Version Version { get; set; }

    /// <summary>Gets or sets the mask pattern.</summary>
    /// <value>The mask pattern.</value>
    public int MaskPattern { get; set; }

    /// <summary>Gets or sets the matrix.</summary>
    /// <value>The matrix.</value>
    public ByteMatrix Matrix { get; set; }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(200);
      stringBuilder.Append("<<\n");
      stringBuilder.Append(" mode: ");
      stringBuilder.Append((object) this.Mode);
      stringBuilder.Append("\n ecLevel: ");
      stringBuilder.Append((object) this.ECLevel);
      stringBuilder.Append("\n version: ");
      if (this.Version == null)
        stringBuilder.Append("null");
      else
        stringBuilder.Append((object) this.Version);
      stringBuilder.Append("\n maskPattern: ");
      stringBuilder.Append(this.MaskPattern);
      if (this.Matrix == null)
      {
        stringBuilder.Append("\n matrix: null\n");
      }
      else
      {
        stringBuilder.Append("\n matrix:\n");
        stringBuilder.Append(this.Matrix.ToString());
      }
      stringBuilder.Append(">>\n");
      return stringBuilder.ToString();
    }

    /// <summary>Check if "mask_pattern" is valid.</summary>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <returns>
    ///   <c>true</c> if [is valid mask pattern] [the specified mask pattern]; otherwise, <c>false</c>.
    /// </returns>
    public static bool isValidMaskPattern(int maskPattern)
    {
      return maskPattern >= 0 && maskPattern < QRCode.NUM_MASK_PATTERNS;
    }
  }
}
