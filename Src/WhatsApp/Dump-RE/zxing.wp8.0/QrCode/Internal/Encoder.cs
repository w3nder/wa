// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.Encoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;
using ZXing.Common.ReedSolomon;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary>
  /// </summary>
  /// <author>satorux@google.com (Satoru Takabayashi) - creator</author>
  /// <author>dswitkin@google.com (Daniel Switkin) - ported from C++</author>
  public static class Encoder
  {
    private static readonly int[] ALPHANUMERIC_TABLE = new int[96]
    {
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      36,
      -1,
      -1,
      -1,
      37,
      38,
      -1,
      -1,
      -1,
      -1,
      39,
      40,
      -1,
      41,
      42,
      43,
      0,
      1,
      2,
      3,
      4,
      5,
      6,
      7,
      8,
      9,
      44,
      -1,
      -1,
      -1,
      -1,
      -1,
      -1,
      10,
      11,
      12,
      13,
      14,
      15,
      16,
      17,
      18,
      19,
      20,
      21,
      22,
      23,
      24,
      25,
      26,
      27,
      28,
      29,
      30,
      31,
      32,
      33,
      34,
      35,
      -1,
      -1,
      -1,
      -1,
      -1
    };
    internal static string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";

    private static int calculateMaskPenalty(ByteMatrix matrix)
    {
      return MaskUtil.applyMaskPenaltyRule1(matrix) + MaskUtil.applyMaskPenaltyRule2(matrix) + MaskUtil.applyMaskPenaltyRule3(matrix) + MaskUtil.applyMaskPenaltyRule4(matrix);
    }

    /// <summary>
    /// Encode "bytes" with the error correction level "ecLevel". The encoding mode will be chosen
    /// internally by chooseMode(). On success, store the result in "qrCode".
    /// We recommend you to use QRCode.EC_LEVEL_L (the lowest level) for
    /// "getECLevel" since our primary use is to show QR code on desktop screens. We don't need very
    /// strong error correction for this purpose.
    /// Note that there is no way to encode bytes in MODE_KANJI. We might want to add EncodeWithMode()
    /// with which clients can specify the encoding mode. For now, we don't need the functionality.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="ecLevel">The ec level.</param>
    public static QRCode encode(string content, ErrorCorrectionLevel ecLevel)
    {
      return Encoder.encode(content, ecLevel, (IDictionary<EncodeHintType, object>) null);
    }

    /// <summary>Encodes the specified content.</summary>
    /// <param name="content">The content.</param>
    /// <param name="ecLevel">The ec level.</param>
    /// <param name="hints">The hints.</param>
    /// <returns></returns>
    public static QRCode encode(
      string content,
      ErrorCorrectionLevel ecLevel,
      IDictionary<EncodeHintType, object> hints)
    {
      string str = (hints == null || !hints.ContainsKey(EncodeHintType.CHARACTER_SET) ? (string) null : (string) hints[EncodeHintType.CHARACTER_SET]) ?? Encoder.DEFAULT_BYTE_MODE_ENCODING;
      bool flag = !Encoder.DEFAULT_BYTE_MODE_ENCODING.Equals(str);
      Mode mode = Encoder.chooseMode(content, str);
      BitArray bitArray1 = new BitArray();
      if (mode == Mode.BYTE && flag)
      {
        CharacterSetECI characterSetEciByName = CharacterSetECI.getCharacterSetECIByName(str);
        if (characterSetEciByName != null && (hints == null || !hints.ContainsKey(EncodeHintType.DISABLE_ECI) || !(bool) hints[EncodeHintType.DISABLE_ECI]))
          Encoder.appendECI(characterSetEciByName, bitArray1);
      }
      Encoder.appendModeInfo(mode, bitArray1);
      BitArray bitArray2 = new BitArray();
      Encoder.appendBytes(content, mode, bitArray2, str);
      Version version1 = Encoder.chooseVersion(bitArray1.Size + mode.getCharacterCountBits(Version.getVersionForNumber(1)) + bitArray2.Size, ecLevel);
      Version version2 = Encoder.chooseVersion(bitArray1.Size + mode.getCharacterCountBits(version1) + bitArray2.Size, ecLevel);
      BitArray bits = new BitArray();
      bits.appendBitArray(bitArray1);
      Encoder.appendLengthInfo(mode == Mode.BYTE ? bitArray2.SizeInBytes : content.Length, version2, mode, bits);
      bits.appendBitArray(bitArray2);
      Version.ECBlocks ecBlocksForLevel = version2.getECBlocksForLevel(ecLevel);
      int numDataBytes = version2.TotalCodewords - ecBlocksForLevel.TotalECCodewords;
      Encoder.terminateBits(numDataBytes, bits);
      BitArray bitArray3 = Encoder.interleaveWithECBytes(bits, version2.TotalCodewords, numDataBytes, ecBlocksForLevel.NumBlocks);
      QRCode qrCode = new QRCode()
      {
        ECLevel = ecLevel,
        Mode = mode,
        Version = version2
      };
      int dimensionForVersion = version2.DimensionForVersion;
      ByteMatrix matrix = new ByteMatrix(dimensionForVersion, dimensionForVersion);
      int maskPattern = Encoder.chooseMaskPattern(bitArray3, ecLevel, version2, matrix);
      qrCode.MaskPattern = maskPattern;
      MatrixUtil.buildMatrix(bitArray3, ecLevel, version2, maskPattern, matrix);
      qrCode.Matrix = matrix;
      return qrCode;
    }

    /// <summary>Gets the alphanumeric code.</summary>
    /// <param name="code">The code.</param>
    /// <returns>the code point of the table used in alphanumeric mode or
    /// -1 if there is no corresponding code in the table.</returns>
    internal static int getAlphanumericCode(int code)
    {
      return code < Encoder.ALPHANUMERIC_TABLE.Length ? Encoder.ALPHANUMERIC_TABLE[code] : -1;
    }

    /// <summary>Chooses the mode.</summary>
    /// <param name="content">The content.</param>
    /// <returns></returns>
    public static Mode chooseMode(string content) => Encoder.chooseMode(content, (string) null);

    /// <summary>
    /// Choose the best mode by examining the content. Note that 'encoding' is used as a hint;
    /// if it is Shift_JIS, and the input is only double-byte Kanji, then we return {@link Mode#KANJI}.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns></returns>
    private static Mode chooseMode(string content, string encoding)
    {
      if ("Shift_JIS".Equals(encoding))
        return !Encoder.isOnlyDoubleByteKanji(content) ? Mode.BYTE : Mode.KANJI;
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < content.Length; ++index)
      {
        char code = content[index];
        switch (code)
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            flag1 = true;
            break;
          default:
            if (Encoder.getAlphanumericCode((int) code) == -1)
              return Mode.BYTE;
            flag2 = true;
            break;
        }
      }
      if (flag2)
        return Mode.ALPHANUMERIC;
      return flag1 ? Mode.NUMERIC : Mode.BYTE;
    }

    private static bool isOnlyDoubleByteKanji(string content)
    {
      byte[] bytes;
      try
      {
        bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
      }
      catch (Exception ex)
      {
        return false;
      }
      int length = bytes.Length;
      if (length % 2 != 0)
        return false;
      for (int index = 0; index < length; index += 2)
      {
        int num = (int) bytes[index] & (int) byte.MaxValue;
        if ((num < 129 || num > 159) && (num < 224 || num > 235))
          return false;
      }
      return true;
    }

    private static int chooseMaskPattern(
      BitArray bits,
      ErrorCorrectionLevel ecLevel,
      Version version,
      ByteMatrix matrix)
    {
      int num1 = int.MaxValue;
      int num2 = -1;
      for (int maskPattern = 0; maskPattern < QRCode.NUM_MASK_PATTERNS; ++maskPattern)
      {
        MatrixUtil.buildMatrix(bits, ecLevel, version, maskPattern, matrix);
        int maskPenalty = Encoder.calculateMaskPenalty(matrix);
        if (maskPenalty < num1)
        {
          num1 = maskPenalty;
          num2 = maskPattern;
        }
      }
      return num2;
    }

    private static Version chooseVersion(int numInputBits, ErrorCorrectionLevel ecLevel)
    {
      for (int versionNumber = 1; versionNumber <= 40; ++versionNumber)
      {
        Version versionForNumber = Version.getVersionForNumber(versionNumber);
        if (versionForNumber.TotalCodewords - versionForNumber.getECBlocksForLevel(ecLevel).TotalECCodewords >= (numInputBits + 7) / 8)
          return versionForNumber;
      }
      throw new WriterException("Data too big");
    }

    /// <summary>
    /// Terminate bits as described in 8.4.8 and 8.4.9 of JISX0510:2004 (p.24).
    /// </summary>
    /// <param name="numDataBytes">The num data bytes.</param>
    /// <param name="bits">The bits.</param>
    internal static void terminateBits(int numDataBytes, BitArray bits)
    {
      int num1 = numDataBytes << 3;
      if (bits.Size > num1)
        throw new WriterException("data bits cannot fit in the QR Code" + (object) bits.Size + " > " + (object) num1);
      for (int index = 0; index < 4 && bits.Size < num1; ++index)
        bits.appendBit(false);
      int num2 = bits.Size & 7;
      if (num2 > 0)
      {
        for (int index = num2; index < 8; ++index)
          bits.appendBit(false);
      }
      int num3 = numDataBytes - bits.SizeInBytes;
      for (int index = 0; index < num3; ++index)
        bits.appendBits((index & 1) == 0 ? 236 : 17, 8);
      if (bits.Size != num1)
        throw new WriterException("Bits size does not equal capacity");
    }

    /// <summary>
    /// Get number of data bytes and number of error correction bytes for block id "blockID". Store
    /// the result in "numDataBytesInBlock", and "numECBytesInBlock". See table 12 in 8.5.1 of
    /// JISX0510:2004 (p.30)
    /// </summary>
    /// <param name="numTotalBytes">The num total bytes.</param>
    /// <param name="numDataBytes">The num data bytes.</param>
    /// <param name="numRSBlocks">The num RS blocks.</param>
    /// <param name="blockID">The block ID.</param>
    /// <param name="numDataBytesInBlock">The num data bytes in block.</param>
    /// <param name="numECBytesInBlock">The num EC bytes in block.</param>
    internal static void getNumDataBytesAndNumECBytesForBlockID(
      int numTotalBytes,
      int numDataBytes,
      int numRSBlocks,
      int blockID,
      int[] numDataBytesInBlock,
      int[] numECBytesInBlock)
    {
      if (blockID >= numRSBlocks)
        throw new WriterException("Block ID too large");
      int num1 = numTotalBytes % numRSBlocks;
      int num2 = numRSBlocks - num1;
      int num3 = numTotalBytes / numRSBlocks;
      int num4 = num3 + 1;
      int num5 = numDataBytes / numRSBlocks;
      int num6 = num5 + 1;
      int num7 = num3 - num5;
      int num8 = num4 - num6;
      if (num7 != num8)
        throw new WriterException("EC bytes mismatch");
      if (numRSBlocks != num2 + num1)
        throw new WriterException("RS blocks mismatch");
      if (numTotalBytes != (num5 + num7) * num2 + (num6 + num8) * num1)
        throw new WriterException("Total bytes mismatch");
      if (blockID < num2)
      {
        numDataBytesInBlock[0] = num5;
        numECBytesInBlock[0] = num7;
      }
      else
      {
        numDataBytesInBlock[0] = num6;
        numECBytesInBlock[0] = num8;
      }
    }

    /// <summary>
    /// Interleave "bits" with corresponding error correction bytes. On success, store the result in
    /// "result". The interleave rule is complicated. See 8.6 of JISX0510:2004 (p.37) for details.
    /// </summary>
    /// <param name="bits">The bits.</param>
    /// <param name="numTotalBytes">The num total bytes.</param>
    /// <param name="numDataBytes">The num data bytes.</param>
    /// <param name="numRSBlocks">The num RS blocks.</param>
    /// <returns></returns>
    internal static BitArray interleaveWithECBytes(
      BitArray bits,
      int numTotalBytes,
      int numDataBytes,
      int numRSBlocks)
    {
      if (bits.SizeInBytes != numDataBytes)
        throw new WriterException("Number of bits and data bytes does not match");
      int num = 0;
      int val1_1 = 0;
      int val1_2 = 0;
      List<BlockPair> blockPairList = new List<BlockPair>(numRSBlocks);
      for (int blockID = 0; blockID < numRSBlocks; ++blockID)
      {
        int[] numDataBytesInBlock = new int[1];
        int[] numECBytesInBlock = new int[1];
        Encoder.getNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, blockID, numDataBytesInBlock, numECBytesInBlock);
        int length = numDataBytesInBlock[0];
        byte[] numArray = new byte[length];
        bits.toBytes(8 * num, numArray, 0, length);
        byte[] ecBytes = Encoder.generateECBytes(numArray, numECBytesInBlock[0]);
        blockPairList.Add(new BlockPair(numArray, ecBytes));
        val1_1 = Math.Max(val1_1, length);
        val1_2 = Math.Max(val1_2, ecBytes.Length);
        num += numDataBytesInBlock[0];
      }
      if (numDataBytes != num)
        throw new WriterException("Data bytes does not match offset");
      BitArray bitArray = new BitArray();
      for (int index = 0; index < val1_1; ++index)
      {
        foreach (BlockPair blockPair in blockPairList)
        {
          byte[] dataBytes = blockPair.DataBytes;
          if (index < dataBytes.Length)
            bitArray.appendBits((int) dataBytes[index], 8);
        }
      }
      for (int index = 0; index < val1_2; ++index)
      {
        foreach (BlockPair blockPair in blockPairList)
        {
          byte[] errorCorrectionBytes = blockPair.ErrorCorrectionBytes;
          if (index < errorCorrectionBytes.Length)
            bitArray.appendBits((int) errorCorrectionBytes[index], 8);
        }
      }
      if (numTotalBytes != bitArray.SizeInBytes)
        throw new WriterException("Interleaving error: " + (object) numTotalBytes + " and " + (object) bitArray.SizeInBytes + " differ.");
      return bitArray;
    }

    internal static byte[] generateECBytes(byte[] dataBytes, int numEcBytesInBlock)
    {
      int length = dataBytes.Length;
      int[] toEncode = new int[length + numEcBytesInBlock];
      for (int index = 0; index < length; ++index)
        toEncode[index] = (int) dataBytes[index] & (int) byte.MaxValue;
      new ReedSolomonEncoder(GenericGF.QR_CODE_FIELD_256).encode(toEncode, numEcBytesInBlock);
      byte[] ecBytes = new byte[numEcBytesInBlock];
      for (int index = 0; index < numEcBytesInBlock; ++index)
        ecBytes[index] = (byte) toEncode[length + index];
      return ecBytes;
    }

    /// <summary>
    /// Append mode info. On success, store the result in "bits".
    /// </summary>
    /// <param name="mode">The mode.</param>
    /// <param name="bits">The bits.</param>
    internal static void appendModeInfo(Mode mode, BitArray bits) => bits.appendBits(mode.Bits, 4);

    /// <summary>
    /// Append length info. On success, store the result in "bits".
    /// </summary>
    /// <param name="numLetters">The num letters.</param>
    /// <param name="version">The version.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="bits">The bits.</param>
    internal static void appendLengthInfo(
      int numLetters,
      Version version,
      Mode mode,
      BitArray bits)
    {
      int characterCountBits = mode.getCharacterCountBits(version);
      if (numLetters >= 1 << characterCountBits)
        throw new WriterException(numLetters.ToString() + " is bigger than " + (object) ((1 << characterCountBits) - 1));
      bits.appendBits(numLetters, characterCountBits);
    }

    /// <summary>
    /// Append "bytes" in "mode" mode (encoding) into "bits". On success, store the result in "bits".
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="mode">The mode.</param>
    /// <param name="bits">The bits.</param>
    /// <param name="encoding">The encoding.</param>
    internal static void appendBytes(string content, Mode mode, BitArray bits, string encoding)
    {
      if (mode.Equals((object) Mode.NUMERIC))
        Encoder.appendNumericBytes(content, bits);
      else if (mode.Equals((object) Mode.ALPHANUMERIC))
        Encoder.appendAlphanumericBytes(content, bits);
      else if (mode.Equals((object) Mode.BYTE))
      {
        Encoder.append8BitBytes(content, bits, encoding);
      }
      else
      {
        if (!mode.Equals((object) Mode.KANJI))
          throw new WriterException("Invalid mode: " + (object) mode);
        Encoder.appendKanjiBytes(content, bits);
      }
    }

    internal static void appendNumericBytes(string content, BitArray bits)
    {
      int length = content.Length;
      int index = 0;
      while (index < length)
      {
        int num1 = (int) content[index] - 48;
        if (index + 2 < length)
        {
          int num2 = (int) content[index + 1] - 48;
          int num3 = (int) content[index + 2] - 48;
          bits.appendBits(num1 * 100 + num2 * 10 + num3, 10);
          index += 3;
        }
        else if (index + 1 < length)
        {
          int num4 = (int) content[index + 1] - 48;
          bits.appendBits(num1 * 10 + num4, 7);
          index += 2;
        }
        else
        {
          bits.appendBits(num1, 4);
          ++index;
        }
      }
    }

    internal static void appendAlphanumericBytes(string content, BitArray bits)
    {
      int length = content.Length;
      int index = 0;
      while (index < length)
      {
        int alphanumericCode1 = Encoder.getAlphanumericCode((int) content[index]);
        if (alphanumericCode1 == -1)
          throw new WriterException();
        if (index + 1 < length)
        {
          int alphanumericCode2 = Encoder.getAlphanumericCode((int) content[index + 1]);
          if (alphanumericCode2 == -1)
            throw new WriterException();
          bits.appendBits(alphanumericCode1 * 45 + alphanumericCode2, 11);
          index += 2;
        }
        else
        {
          bits.appendBits(alphanumericCode1, 6);
          ++index;
        }
      }
    }

    internal static void append8BitBytes(string content, BitArray bits, string encoding)
    {
      byte[] bytes;
      try
      {
        bytes = Encoding.GetEncoding(encoding).GetBytes(content);
      }
      catch (Exception ex)
      {
        throw new WriterException(ex.Message, ex);
      }
      foreach (byte num in bytes)
        bits.appendBits((int) num, 8);
    }

    internal static void appendKanjiBytes(string content, BitArray bits)
    {
      byte[] bytes;
      try
      {
        bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
      }
      catch (Exception ex)
      {
        throw new WriterException(ex.Message, ex);
      }
      int length = bytes.Length;
      for (int index = 0; index < length; index += 2)
      {
        int num1 = ((int) bytes[index] & (int) byte.MaxValue) << 8 | (int) bytes[index + 1] & (int) byte.MaxValue;
        int num2 = -1;
        if (num1 >= 33088 && num1 <= 40956)
          num2 = num1 - 33088;
        else if (num1 >= 57408 && num1 <= 60351)
          num2 = num1 - 49472;
        if (num2 == -1)
          throw new WriterException("Invalid byte sequence");
        int num3 = (num2 >> 8) * 192 + (num2 & (int) byte.MaxValue);
        bits.appendBits(num3, 13);
      }
    }

    private static void appendECI(CharacterSetECI eci, BitArray bits)
    {
      bits.appendBits(Mode.ECI.Bits, 4);
      bits.appendBits(eci.Value, 8);
    }
  }
}
