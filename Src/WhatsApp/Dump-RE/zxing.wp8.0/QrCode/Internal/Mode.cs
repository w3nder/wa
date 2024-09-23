// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.Mode
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// <p>See ISO 18004:2006, 6.4.1, Tables 2 and 3. This enum encapsulates the various modes in which
  /// data can be encoded to bits in the QR code standard.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public sealed class Mode
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode TERMINATOR = new Mode(new int[3], 0, nameof (TERMINATOR));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode NUMERIC = new Mode(new int[3]
    {
      10,
      12,
      14
    }, 1, nameof (NUMERIC));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode ALPHANUMERIC = new Mode(new int[3]
    {
      9,
      11,
      13
    }, 2, nameof (ALPHANUMERIC));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode STRUCTURED_APPEND = new Mode(new int[3], 3, nameof (STRUCTURED_APPEND));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode BYTE = new Mode(new int[3]
    {
      8,
      16,
      16
    }, 4, nameof (BYTE));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode ECI = new Mode((int[]) null, 7, nameof (ECI));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode KANJI = new Mode(new int[3]
    {
      8,
      10,
      12
    }, 8, nameof (KANJI));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode FNC1_FIRST_POSITION = new Mode((int[]) null, 5, nameof (FNC1_FIRST_POSITION));
    /// <summary>
    /// 
    /// </summary>
    public static readonly Mode FNC1_SECOND_POSITION = new Mode((int[]) null, 9, nameof (FNC1_SECOND_POSITION));
    /// <summary>See GBT 18284-2000; "Hanzi" is a transliteration of this mode name.</summary>
    public static readonly Mode HANZI = new Mode(new int[3]
    {
      8,
      10,
      12
    }, 13, nameof (HANZI));
    private readonly int[] characterCountBitsForVersions;
    private readonly int bits;
    private readonly string name;

    /// <summary>Gets the name.</summary>
    public string Name => this.name;

    private Mode(int[] characterCountBitsForVersions, int bits, string name)
    {
      this.characterCountBitsForVersions = characterCountBitsForVersions;
      this.bits = bits;
      this.name = name;
    }

    /// <summary>Fors the bits.</summary>
    /// <param name="bits">four bits encoding a QR Code data mode</param>
    /// <returns>
    ///   <see cref="T:ZXing.QrCode.Internal.Mode" /> encoded by these bits
    /// </returns>
    /// <exception cref="T:System.ArgumentException">if bits do not correspond to a known mode</exception>
    public static Mode forBits(int bits)
    {
      switch (bits)
      {
        case 0:
          return Mode.TERMINATOR;
        case 1:
          return Mode.NUMERIC;
        case 2:
          return Mode.ALPHANUMERIC;
        case 3:
          return Mode.STRUCTURED_APPEND;
        case 4:
          return Mode.BYTE;
        case 5:
          return Mode.FNC1_FIRST_POSITION;
        case 7:
          return Mode.ECI;
        case 8:
          return Mode.KANJI;
        case 9:
          return Mode.FNC1_SECOND_POSITION;
        case 13:
          return Mode.HANZI;
        default:
          throw new ArgumentException();
      }
    }

    /// <param name="version">version in question</param>
    /// <returns> number of bits used, in this QR Code symbol {@link Version}, to encode the
    /// count of characters that will follow encoded in this {@link Mode}
    /// </returns>
    public int getCharacterCountBits(Version version)
    {
      if (this.characterCountBitsForVersions == null)
        throw new ArgumentException("Character count doesn't apply to this mode");
      int versionNumber = version.VersionNumber;
      return this.characterCountBitsForVersions[versionNumber > 9 ? (versionNumber > 26 ? 2 : 1) : 0];
    }

    /// <summary>Gets the bits.</summary>
    public int Bits => this.bits;

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => this.name;
  }
}
