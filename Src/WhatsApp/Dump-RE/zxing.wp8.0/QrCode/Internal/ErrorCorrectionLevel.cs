// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.ErrorCorrectionLevel
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// <p>See ISO 18004:2006, 6.5.1. This enum encapsulates the four error correction levels
  /// defined by the QR code standard.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class ErrorCorrectionLevel
  {
    /// <summary> L = ~7% correction</summary>
    public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 1, nameof (L));
    /// <summary> M = ~15% correction</summary>
    public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0, nameof (M));
    /// <summary> Q = ~25% correction</summary>
    public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 3, nameof (Q));
    /// <summary> H = ~30% correction</summary>
    public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 2, nameof (H));
    private static readonly ErrorCorrectionLevel[] FOR_BITS = new ErrorCorrectionLevel[4]
    {
      ErrorCorrectionLevel.M,
      ErrorCorrectionLevel.L,
      ErrorCorrectionLevel.H,
      ErrorCorrectionLevel.Q
    };
    private readonly int bits;
    private readonly int ordinal_Renamed_Field;
    private readonly string name;

    private ErrorCorrectionLevel(int ordinal, int bits, string name)
    {
      this.ordinal_Renamed_Field = ordinal;
      this.bits = bits;
      this.name = name;
    }

    /// <summary>Gets the bits.</summary>
    public int Bits => this.bits;

    /// <summary>Gets the name.</summary>
    public string Name => this.name;

    /// <summary>Ordinals this instance.</summary>
    /// <returns></returns>
    public int ordinal() => this.ordinal_Renamed_Field;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.name;

    /// <summary>Fors the bits.</summary>
    /// <param name="bits">int containing the two bits encoding a QR Code's error correction level</param>
    /// <returns>
    ///   <see cref="T:ZXing.QrCode.Internal.ErrorCorrectionLevel" /> representing the encoded error correction level
    /// </returns>
    public static ErrorCorrectionLevel forBits(int bits)
    {
      if (bits < 0 || bits >= ErrorCorrectionLevel.FOR_BITS.Length)
        throw new ArgumentException();
      return ErrorCorrectionLevel.FOR_BITS[bits];
    }
  }
}
