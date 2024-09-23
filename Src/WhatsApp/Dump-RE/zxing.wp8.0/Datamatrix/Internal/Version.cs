// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.Version
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// The Version object encapsulates attributes about a particular
  /// size Data Matrix Code.
  /// 
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// </summary>
  public sealed class Version
  {
    private static readonly Version[] VERSIONS = Version.buildVersions();
    private readonly int versionNumber;
    private readonly int symbolSizeRows;
    private readonly int symbolSizeColumns;
    private readonly int dataRegionSizeRows;
    private readonly int dataRegionSizeColumns;
    private readonly Version.ECBlocks ecBlocks;
    private readonly int totalCodewords;

    internal Version(
      int versionNumber,
      int symbolSizeRows,
      int symbolSizeColumns,
      int dataRegionSizeRows,
      int dataRegionSizeColumns,
      Version.ECBlocks ecBlocks)
    {
      this.versionNumber = versionNumber;
      this.symbolSizeRows = symbolSizeRows;
      this.symbolSizeColumns = symbolSizeColumns;
      this.dataRegionSizeRows = dataRegionSizeRows;
      this.dataRegionSizeColumns = dataRegionSizeColumns;
      this.ecBlocks = ecBlocks;
      int num = 0;
      int ecCodewords = ecBlocks.ECCodewords;
      foreach (Version.ECB ecb in ecBlocks.ECBlocksValue)
        num += ecb.Count * (ecb.DataCodewords + ecCodewords);
      this.totalCodewords = num;
    }

    public int getVersionNumber() => this.versionNumber;

    public int getSymbolSizeRows() => this.symbolSizeRows;

    public int getSymbolSizeColumns() => this.symbolSizeColumns;

    public int getDataRegionSizeRows() => this.dataRegionSizeRows;

    public int getDataRegionSizeColumns() => this.dataRegionSizeColumns;

    public int getTotalCodewords() => this.totalCodewords;

    internal Version.ECBlocks getECBlocks() => this.ecBlocks;

    /// <summary>
    /// <p>Deduces version information from Data Matrix dimensions.</p>
    /// 
    /// <param name="numRows">Number of rows in modules</param>
    /// <param name="numColumns">Number of columns in modules</param>
    /// <returns>Version for a Data Matrix Code of those dimensions</returns>
    /// <exception cref="T:ZXing.FormatException">if dimensions do correspond to a valid Data Matrix size</exception>
    /// </summary>
    public static Version getVersionForDimensions(int numRows, int numColumns)
    {
      if ((numRows & 1) != 0 || (numColumns & 1) != 0)
        return (Version) null;
      foreach (Version versionForDimensions in Version.VERSIONS)
      {
        if (versionForDimensions.symbolSizeRows == numRows && versionForDimensions.symbolSizeColumns == numColumns)
          return versionForDimensions;
      }
      return (Version) null;
    }

    public override string ToString() => this.versionNumber.ToString();

    /// <summary>See ISO 16022:2006 5.5.1 Table 7</summary>
    private static Version[] buildVersions()
    {
      return new Version[30]
      {
        new Version(1, 10, 10, 8, 8, new Version.ECBlocks(5, new Version.ECB(1, 3))),
        new Version(2, 12, 12, 10, 10, new Version.ECBlocks(7, new Version.ECB(1, 5))),
        new Version(3, 14, 14, 12, 12, new Version.ECBlocks(10, new Version.ECB(1, 8))),
        new Version(4, 16, 16, 14, 14, new Version.ECBlocks(12, new Version.ECB(1, 12))),
        new Version(5, 18, 18, 16, 16, new Version.ECBlocks(14, new Version.ECB(1, 18))),
        new Version(6, 20, 20, 18, 18, new Version.ECBlocks(18, new Version.ECB(1, 22))),
        new Version(7, 22, 22, 20, 20, new Version.ECBlocks(20, new Version.ECB(1, 30))),
        new Version(8, 24, 24, 22, 22, new Version.ECBlocks(24, new Version.ECB(1, 36))),
        new Version(9, 26, 26, 24, 24, new Version.ECBlocks(28, new Version.ECB(1, 44))),
        new Version(10, 32, 32, 14, 14, new Version.ECBlocks(36, new Version.ECB(1, 62))),
        new Version(11, 36, 36, 16, 16, new Version.ECBlocks(42, new Version.ECB(1, 86))),
        new Version(12, 40, 40, 18, 18, new Version.ECBlocks(48, new Version.ECB(1, 114))),
        new Version(13, 44, 44, 20, 20, new Version.ECBlocks(56, new Version.ECB(1, 144))),
        new Version(14, 48, 48, 22, 22, new Version.ECBlocks(68, new Version.ECB(1, 174))),
        new Version(15, 52, 52, 24, 24, new Version.ECBlocks(42, new Version.ECB(2, 102))),
        new Version(16, 64, 64, 14, 14, new Version.ECBlocks(56, new Version.ECB(2, 140))),
        new Version(17, 72, 72, 16, 16, new Version.ECBlocks(36, new Version.ECB(4, 92))),
        new Version(18, 80, 80, 18, 18, new Version.ECBlocks(48, new Version.ECB(4, 114))),
        new Version(19, 88, 88, 20, 20, new Version.ECBlocks(56, new Version.ECB(4, 144))),
        new Version(20, 96, 96, 22, 22, new Version.ECBlocks(68, new Version.ECB(4, 174))),
        new Version(21, 104, 104, 24, 24, new Version.ECBlocks(56, new Version.ECB(6, 136))),
        new Version(22, 120, 120, 18, 18, new Version.ECBlocks(68, new Version.ECB(6, 175))),
        new Version(23, 132, 132, 20, 20, new Version.ECBlocks(62, new Version.ECB(8, 163))),
        new Version(24, 144, 144, 22, 22, new Version.ECBlocks(62, new Version.ECB(8, 156), new Version.ECB(2, 155))),
        new Version(25, 8, 18, 6, 16, new Version.ECBlocks(7, new Version.ECB(1, 5))),
        new Version(26, 8, 32, 6, 14, new Version.ECBlocks(11, new Version.ECB(1, 10))),
        new Version(27, 12, 26, 10, 24, new Version.ECBlocks(14, new Version.ECB(1, 16))),
        new Version(28, 12, 36, 10, 16, new Version.ECBlocks(18, new Version.ECB(1, 22))),
        new Version(29, 16, 36, 14, 16, new Version.ECBlocks(24, new Version.ECB(1, 32))),
        new Version(30, 16, 48, 14, 22, new Version.ECBlocks(28, new Version.ECB(1, 49)))
      };
    }

    /// <summary>
    /// <p>Encapsulates a set of error-correction blocks in one symbol version. Most versions will
    /// use blocks of differing sizes within one version, so, this encapsulates the parameters for
    /// each set of blocks. It also holds the number of error-correction codewords per block since it
    /// will be the same across all blocks within one version.</p>
    /// </summary>
    internal sealed class ECBlocks
    {
      private readonly int ecCodewords;
      private readonly Version.ECB[] _ecBlocksValue;

      internal ECBlocks(int ecCodewords, Version.ECB ecBlocks)
      {
        this.ecCodewords = ecCodewords;
        this._ecBlocksValue = new Version.ECB[1]{ ecBlocks };
      }

      internal ECBlocks(int ecCodewords, Version.ECB ecBlocks1, Version.ECB ecBlocks2)
      {
        this.ecCodewords = ecCodewords;
        this._ecBlocksValue = new Version.ECB[2]
        {
          ecBlocks1,
          ecBlocks2
        };
      }

      internal int ECCodewords => this.ecCodewords;

      internal Version.ECB[] ECBlocksValue => this._ecBlocksValue;
    }

    /// <summary>
    /// <p>Encapsualtes the parameters for one error-correction block in one symbol version.
    /// This includes the number of data codewords, and the number of times a block with these
    /// parameters is used consecutively in the Data Matrix code version's format.</p>
    /// </summary>
    internal sealed class ECB
    {
      private readonly int count;
      private readonly int dataCodewords;

      internal ECB(int count, int dataCodewords)
      {
        this.count = count;
        this.dataCodewords = dataCodewords;
      }

      internal int Count => this.count;

      internal int DataCodewords => this.dataCodewords;
    }
  }
}
