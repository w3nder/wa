// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.QRCodeDecoderMetaData
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// Meta-data container for QR Code decoding. Instances of this class may be used to convey information back to the
  /// decoding caller. Callers are expected to process this.
  /// </summary>
  public sealed class QRCodeDecoderMetaData
  {
    private readonly bool mirrored;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.QRCodeDecoderMetaData" /> class.
    /// </summary>
    /// <param name="mirrored">if set to <c>true</c> [mirrored].</param>
    public QRCodeDecoderMetaData(bool mirrored) => this.mirrored = mirrored;

    /// <summary>true if the QR Code was mirrored.</summary>
    public bool IsMirrored => this.mirrored;

    /// <summary>
    /// Apply the result points' order correction due to mirroring.
    /// </summary>
    /// <param name="points">Array of points to apply mirror correction to.</param>
    public void applyMirroredCorrection(ResultPoint[] points)
    {
      if (!this.mirrored || points == null || points.Length < 3)
        return;
      ResultPoint point = points[0];
      points[0] = points[2];
      points[2] = point;
    }
  }
}
