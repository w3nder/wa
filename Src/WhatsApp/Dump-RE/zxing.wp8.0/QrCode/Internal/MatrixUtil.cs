// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.MatrixUtil
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
  public static class MatrixUtil
  {
    private const int VERSION_INFO_POLY = 7973;
    private const int TYPE_INFO_POLY = 1335;
    private const int TYPE_INFO_MASK_PATTERN = 21522;
    private static readonly int[][] POSITION_DETECTION_PATTERN = new int[7][]
    {
      new int[7]{ 1, 1, 1, 1, 1, 1, 1 },
      new int[7]{ 1, 0, 0, 0, 0, 0, 1 },
      new int[7]{ 1, 0, 1, 1, 1, 0, 1 },
      new int[7]{ 1, 0, 1, 1, 1, 0, 1 },
      new int[7]{ 1, 0, 1, 1, 1, 0, 1 },
      new int[7]{ 1, 0, 0, 0, 0, 0, 1 },
      new int[7]{ 1, 1, 1, 1, 1, 1, 1 }
    };
    private static readonly int[][] POSITION_ADJUSTMENT_PATTERN = new int[5][]
    {
      new int[5]{ 1, 1, 1, 1, 1 },
      new int[5]{ 1, 0, 0, 0, 1 },
      new int[5]{ 1, 0, 1, 0, 1 },
      new int[5]{ 1, 0, 0, 0, 1 },
      new int[5]{ 1, 1, 1, 1, 1 }
    };
    private static readonly int[][] POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE = new int[40][]
    {
      new int[7]{ -1, -1, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 18, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 22, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 26, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 30, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 34, -1, -1, -1, -1, -1 },
      new int[7]{ 6, 22, 38, -1, -1, -1, -1 },
      new int[7]{ 6, 24, 42, -1, -1, -1, -1 },
      new int[7]{ 6, 26, 46, -1, -1, -1, -1 },
      new int[7]{ 6, 28, 50, -1, -1, -1, -1 },
      new int[7]{ 6, 30, 54, -1, -1, -1, -1 },
      new int[7]{ 6, 32, 58, -1, -1, -1, -1 },
      new int[7]{ 6, 34, 62, -1, -1, -1, -1 },
      new int[7]{ 6, 26, 46, 66, -1, -1, -1 },
      new int[7]{ 6, 26, 48, 70, -1, -1, -1 },
      new int[7]{ 6, 26, 50, 74, -1, -1, -1 },
      new int[7]{ 6, 30, 54, 78, -1, -1, -1 },
      new int[7]{ 6, 30, 56, 82, -1, -1, -1 },
      new int[7]{ 6, 30, 58, 86, -1, -1, -1 },
      new int[7]{ 6, 34, 62, 90, -1, -1, -1 },
      new int[7]{ 6, 28, 50, 72, 94, -1, -1 },
      new int[7]{ 6, 26, 50, 74, 98, -1, -1 },
      new int[7]{ 6, 30, 54, 78, 102, -1, -1 },
      new int[7]{ 6, 28, 54, 80, 106, -1, -1 },
      new int[7]{ 6, 32, 58, 84, 110, -1, -1 },
      new int[7]{ 6, 30, 58, 86, 114, -1, -1 },
      new int[7]{ 6, 34, 62, 90, 118, -1, -1 },
      new int[7]{ 6, 26, 50, 74, 98, 122, -1 },
      new int[7]{ 6, 30, 54, 78, 102, 126, -1 },
      new int[7]{ 6, 26, 52, 78, 104, 130, -1 },
      new int[7]{ 6, 30, 56, 82, 108, 134, -1 },
      new int[7]{ 6, 34, 60, 86, 112, 138, -1 },
      new int[7]{ 6, 30, 58, 86, 114, 142, -1 },
      new int[7]{ 6, 34, 62, 90, 118, 146, -1 },
      new int[7]{ 6, 30, 54, 78, 102, 126, 150 },
      new int[7]{ 6, 24, 50, 76, 102, 128, 154 },
      new int[7]{ 6, 28, 54, 80, 106, 132, 158 },
      new int[7]{ 6, 32, 58, 84, 110, 136, 162 },
      new int[7]{ 6, 26, 54, 82, 110, 138, 166 },
      new int[7]{ 6, 30, 58, 86, 114, 142, 170 }
    };
    private static readonly int[][] TYPE_INFO_COORDINATES = new int[15][]
    {
      new int[2]{ 8, 0 },
      new int[2]{ 8, 1 },
      new int[2]{ 8, 2 },
      new int[2]{ 8, 3 },
      new int[2]{ 8, 4 },
      new int[2]{ 8, 5 },
      new int[2]{ 8, 7 },
      new int[2]{ 8, 8 },
      new int[2]{ 7, 8 },
      new int[2]{ 5, 8 },
      new int[2]{ 4, 8 },
      new int[2]{ 3, 8 },
      new int[2]{ 2, 8 },
      new int[2]{ 1, 8 },
      new int[2]{ 0, 8 }
    };

    /// <summary>
    /// Set all cells to 2.  2 means that the cell is empty (not set yet).
    /// 
    /// JAVAPORT: We shouldn't need to do this at all. The code should be rewritten to begin encoding
    /// with the ByteMatrix initialized all to zero.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    public static void clearMatrix(ByteMatrix matrix) => matrix.clear((byte) 2);

    /// <summary>
    /// Build 2D matrix of QR Code from "dataBits" with "ecLevel", "version" and "getMaskPattern". On
    /// success, store the result in "matrix" and return true.
    /// </summary>
    /// <param name="dataBits">The data bits.</param>
    /// <param name="ecLevel">The ec level.</param>
    /// <param name="version">The version.</param>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <param name="matrix">The matrix.</param>
    public static void buildMatrix(
      BitArray dataBits,
      ErrorCorrectionLevel ecLevel,
      Version version,
      int maskPattern,
      ByteMatrix matrix)
    {
      MatrixUtil.clearMatrix(matrix);
      MatrixUtil.embedBasicPatterns(version, matrix);
      MatrixUtil.embedTypeInfo(ecLevel, maskPattern, matrix);
      MatrixUtil.maybeEmbedVersionInfo(version, matrix);
      MatrixUtil.embedDataBits(dataBits, maskPattern, matrix);
    }

    /// <summary>
    /// Embed basic patterns. On success, modify the matrix and return true.
    /// The basic patterns are:
    /// - Position detection patterns
    /// - Timing patterns
    /// - Dark dot at the left bottom corner
    /// - Position adjustment patterns, if need be
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="matrix">The matrix.</param>
    public static void embedBasicPatterns(Version version, ByteMatrix matrix)
    {
      MatrixUtil.embedPositionDetectionPatternsAndSeparators(matrix);
      MatrixUtil.embedDarkDotAtLeftBottomCorner(matrix);
      MatrixUtil.maybeEmbedPositionAdjustmentPatterns(version, matrix);
      MatrixUtil.embedTimingPatterns(matrix);
    }

    /// <summary>
    /// Embed type information. On success, modify the matrix.
    /// </summary>
    /// <param name="ecLevel">The ec level.</param>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <param name="matrix">The matrix.</param>
    public static void embedTypeInfo(
      ErrorCorrectionLevel ecLevel,
      int maskPattern,
      ByteMatrix matrix)
    {
      BitArray bits = new BitArray();
      MatrixUtil.makeTypeInfoBits(ecLevel, maskPattern, bits);
      for (int index = 0; index < bits.Size; ++index)
      {
        int num = bits[bits.Size - 1 - index] ? 1 : 0;
        int x1 = MatrixUtil.TYPE_INFO_COORDINATES[index][0];
        int y1 = MatrixUtil.TYPE_INFO_COORDINATES[index][1];
        matrix[x1, y1] = num;
        if (index < 8)
        {
          int x2 = matrix.Width - index - 1;
          int y2 = 8;
          matrix[x2, y2] = num;
        }
        else
        {
          int x3 = 8;
          int y3 = matrix.Height - 7 + (index - 8);
          matrix[x3, y3] = num;
        }
      }
    }

    /// <summary>
    /// Embed version information if need be. On success, modify the matrix and return true.
    /// See 8.10 of JISX0510:2004 (p.47) for how to embed version information.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="matrix">The matrix.</param>
    public static void maybeEmbedVersionInfo(Version version, ByteMatrix matrix)
    {
      if (version.VersionNumber < 7)
        return;
      BitArray bits = new BitArray();
      MatrixUtil.makeVersionInfoBits(version, bits);
      int i = 17;
      for (int index1 = 0; index1 < 6; ++index1)
      {
        for (int index2 = 0; index2 < 3; ++index2)
        {
          int num = bits[i] ? 1 : 0;
          --i;
          matrix[index1, matrix.Height - 11 + index2] = num;
          matrix[matrix.Height - 11 + index2, index1] = num;
        }
      }
    }

    /// <summary>
    /// Embed "dataBits" using "getMaskPattern". On success, modify the matrix and return true.
    /// For debugging purposes, it skips masking process if "getMaskPattern" is -1.
    /// See 8.7 of JISX0510:2004 (p.38) for how to embed data bits.
    /// </summary>
    /// <param name="dataBits">The data bits.</param>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <param name="matrix">The matrix.</param>
    public static void embedDataBits(BitArray dataBits, int maskPattern, ByteMatrix matrix)
    {
      int i = 0;
      int num1 = -1;
      int num2 = matrix.Width - 1;
      int y = matrix.Height - 1;
      for (; num2 > 0; num2 -= 2)
      {
        if (num2 == 6)
          --num2;
        for (; y >= 0 && y < matrix.Height; y += num1)
        {
          for (int index = 0; index < 2; ++index)
          {
            int x = num2 - index;
            if (MatrixUtil.isEmpty(matrix[x, y]))
            {
              int num3;
              if (i < dataBits.Size)
              {
                num3 = dataBits[i] ? 1 : 0;
                ++i;
              }
              else
                num3 = 0;
              if (maskPattern != -1 && MaskUtil.getDataMaskBit(maskPattern, x, y))
                num3 ^= 1;
              matrix[x, y] = num3;
            }
          }
        }
        num1 = -num1;
        y += num1;
      }
      if (i != dataBits.Size)
        throw new WriterException("Not all bits consumed: " + (object) i + (object) '/' + (object) dataBits.Size);
    }

    /// <summary>
    /// Return the position of the most significant bit set (to one) in the "value". The most
    /// significant bit is position 32. If there is no bit set, return 0. Examples:
    /// - findMSBSet(0) =&gt; 0
    /// - findMSBSet(1) =&gt; 1
    /// - findMSBSet(255) =&gt; 8
    /// </summary>
    /// <param name="value_Renamed">The value_ renamed.</param>
    /// <returns></returns>
    public static int findMSBSet(int value_Renamed)
    {
      int msbSet = 0;
      while (value_Renamed != 0)
      {
        value_Renamed >>>= 1;
        ++msbSet;
      }
      return msbSet;
    }

    /// <summary>
    /// Calculate BCH (Bose-Chaudhuri-Hocquenghem) code for "value" using polynomial "poly". The BCH
    /// code is used for encoding type information and version information.
    /// Example: Calculation of version information of 7.
    /// f(x) is created from 7.
    ///   - 7 = 000111 in 6 bits
    ///   - f(x) = x^2 + x^2 + x^1
    /// g(x) is given by the standard (p. 67)
    ///   - g(x) = x^12 + x^11 + x^10 + x^9 + x^8 + x^5 + x^2 + 1
    /// Multiply f(x) by x^(18 - 6)
    ///   - f'(x) = f(x) * x^(18 - 6)
    ///   - f'(x) = x^14 + x^13 + x^12
    /// Calculate the remainder of f'(x) / g(x)
    ///         x^2
    ///         __________________________________________________
    ///   g(x) )x^14 + x^13 + x^12
    ///         x^14 + x^13 + x^12 + x^11 + x^10 + x^7 + x^4 + x^2
    ///         --------------------------------------------------
    ///                              x^11 + x^10 + x^7 + x^4 + x^2
    /// 
    /// The remainder is x^11 + x^10 + x^7 + x^4 + x^2
    /// Encode it in binary: 110010010100
    /// The return value is 0xc94 (1100 1001 0100)
    /// 
    /// Since all coefficients in the polynomials are 1 or 0, we can do the calculation by bit
    /// operations. We don't care if cofficients are positive or negative.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="poly">The poly.</param>
    /// <returns></returns>
    public static int calculateBCHCode(int value, int poly)
    {
      int msbSet = MatrixUtil.findMSBSet(poly);
      value <<= msbSet - 1;
      while (MatrixUtil.findMSBSet(value) >= msbSet)
        value ^= poly << MatrixUtil.findMSBSet(value) - msbSet;
      return value;
    }

    /// <summary>
    /// Make bit vector of type information. On success, store the result in "bits" and return true.
    /// Encode error correction level and mask pattern. See 8.9 of
    /// JISX0510:2004 (p.45) for details.
    /// </summary>
    /// <param name="ecLevel">The ec level.</param>
    /// <param name="maskPattern">The mask pattern.</param>
    /// <param name="bits">The bits.</param>
    public static void makeTypeInfoBits(
      ErrorCorrectionLevel ecLevel,
      int maskPattern,
      BitArray bits)
    {
      if (!QRCode.isValidMaskPattern(maskPattern))
        throw new WriterException("Invalid mask pattern");
      int num = ecLevel.Bits << 3 | maskPattern;
      bits.appendBits(num, 5);
      int bchCode = MatrixUtil.calculateBCHCode(num, 1335);
      bits.appendBits(bchCode, 10);
      BitArray other = new BitArray();
      other.appendBits(21522, 15);
      bits.xor(other);
      if (bits.Size != 15)
        throw new WriterException("should not happen but we got: " + (object) bits.Size);
    }

    /// <summary>
    /// Make bit vector of version information. On success, store the result in "bits" and return true.
    /// See 8.10 of JISX0510:2004 (p.45) for details.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <param name="bits">The bits.</param>
    public static void makeVersionInfoBits(Version version, BitArray bits)
    {
      bits.appendBits(version.VersionNumber, 6);
      int bchCode = MatrixUtil.calculateBCHCode(version.VersionNumber, 7973);
      bits.appendBits(bchCode, 12);
      if (bits.Size != 18)
        throw new WriterException("should not happen but we got: " + (object) bits.Size);
    }

    /// <summary>Check if "value" is empty.</summary>
    /// <param name="value">The value.</param>
    /// <returns>
    ///   <c>true</c> if the specified value is empty; otherwise, <c>false</c>.
    /// </returns>
    private static bool isEmpty(int value) => value == 2;

    private static void embedTimingPatterns(ByteMatrix matrix)
    {
      for (int index = 8; index < matrix.Width - 8; ++index)
      {
        int num = (index + 1) % 2;
        if (MatrixUtil.isEmpty(matrix[index, 6]))
          matrix[index, 6] = num;
        if (MatrixUtil.isEmpty(matrix[6, index]))
          matrix[6, index] = num;
      }
    }

    /// <summary>
    /// Embed the lonely dark dot at left bottom corner. JISX0510:2004 (p.46)
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    private static void embedDarkDotAtLeftBottomCorner(ByteMatrix matrix)
    {
      if (matrix[8, matrix.Height - 8] == 0)
        throw new WriterException();
      matrix[8, matrix.Height - 8] = 1;
    }

    private static void embedHorizontalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
    {
      for (int index = 0; index < 8; ++index)
      {
        if (!MatrixUtil.isEmpty(matrix[xStart + index, yStart]))
          throw new WriterException();
        matrix[xStart + index, yStart] = 0;
      }
    }

    private static void embedVerticalSeparationPattern(int xStart, int yStart, ByteMatrix matrix)
    {
      for (int index = 0; index < 7; ++index)
      {
        if (!MatrixUtil.isEmpty(matrix[xStart, yStart + index]))
          throw new WriterException();
        matrix[xStart, yStart + index] = 0;
      }
    }

    /// <summary>
    /// Note that we cannot unify the function with embedPositionDetectionPattern() despite they are
    /// almost identical, since we cannot write a function that takes 2D arrays in different sizes in
    /// C/C++. We should live with the fact.
    /// </summary>
    /// <param name="xStart">The x start.</param>
    /// <param name="yStart">The y start.</param>
    /// <param name="matrix">The matrix.</param>
    private static void embedPositionAdjustmentPattern(int xStart, int yStart, ByteMatrix matrix)
    {
      for (int index1 = 0; index1 < 5; ++index1)
      {
        for (int index2 = 0; index2 < 5; ++index2)
          matrix[xStart + index2, yStart + index1] = MatrixUtil.POSITION_ADJUSTMENT_PATTERN[index1][index2];
      }
    }

    private static void embedPositionDetectionPattern(int xStart, int yStart, ByteMatrix matrix)
    {
      for (int index1 = 0; index1 < 7; ++index1)
      {
        for (int index2 = 0; index2 < 7; ++index2)
          matrix[xStart + index2, yStart + index1] = MatrixUtil.POSITION_DETECTION_PATTERN[index1][index2];
      }
    }

    /// <summary>
    /// Embed position detection patterns and surrounding vertical/horizontal separators.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    private static void embedPositionDetectionPatternsAndSeparators(ByteMatrix matrix)
    {
      int length = MatrixUtil.POSITION_DETECTION_PATTERN[0].Length;
      MatrixUtil.embedPositionDetectionPattern(0, 0, matrix);
      MatrixUtil.embedPositionDetectionPattern(matrix.Width - length, 0, matrix);
      MatrixUtil.embedPositionDetectionPattern(0, matrix.Width - length, matrix);
      MatrixUtil.embedHorizontalSeparationPattern(0, 7, matrix);
      MatrixUtil.embedHorizontalSeparationPattern(matrix.Width - 8, 7, matrix);
      MatrixUtil.embedHorizontalSeparationPattern(0, matrix.Width - 8, matrix);
      MatrixUtil.embedVerticalSeparationPattern(7, 0, matrix);
      MatrixUtil.embedVerticalSeparationPattern(matrix.Height - 7 - 1, 0, matrix);
      MatrixUtil.embedVerticalSeparationPattern(7, matrix.Height - 7, matrix);
    }

    /// <summary>Embed position adjustment patterns if need be.</summary>
    /// <param name="version">The version.</param>
    /// <param name="matrix">The matrix.</param>
    private static void maybeEmbedPositionAdjustmentPatterns(Version version, ByteMatrix matrix)
    {
      if (version.VersionNumber < 2)
        return;
      int index1 = version.VersionNumber - 1;
      int[] numArray = MatrixUtil.POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index1];
      int length = MatrixUtil.POSITION_ADJUSTMENT_PATTERN_COORDINATE_TABLE[index1].Length;
      for (int index2 = 0; index2 < length; ++index2)
      {
        for (int index3 = 0; index3 < length; ++index3)
        {
          int y = numArray[index2];
          int x = numArray[index3];
          if (x != -1 && y != -1 && MatrixUtil.isEmpty(matrix[x, y]))
            MatrixUtil.embedPositionAdjustmentPattern(x - 2, y - 2, matrix);
        }
      }
    }
  }
}
