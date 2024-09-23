// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.DecodedBitStreamParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using BigIntegerLibrary;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// <p>This class contains the methods for decoding the PDF417 codewords.</p>
  /// 
  /// <author>SITA Lab (kevin.osullivan@sita.aero)</author>
  /// </summary>
  internal static class DecodedBitStreamParser
  {
    private const int TEXT_COMPACTION_MODE_LATCH = 900;
    private const int BYTE_COMPACTION_MODE_LATCH = 901;
    private const int NUMERIC_COMPACTION_MODE_LATCH = 902;
    private const int BYTE_COMPACTION_MODE_LATCH_6 = 924;
    private const int BEGIN_MACRO_PDF417_CONTROL_BLOCK = 928;
    private const int BEGIN_MACRO_PDF417_OPTIONAL_FIELD = 923;
    private const int MACRO_PDF417_TERMINATOR = 922;
    private const int MODE_SHIFT_TO_BYTE_COMPACTION_MODE = 913;
    private const int MAX_NUMERIC_CODEWORDS = 15;
    private const int PL = 25;
    private const int LL = 27;
    private const int AS = 27;
    private const int ML = 28;
    private const int AL = 28;
    private const int PS = 29;
    private const int PAL = 29;
    private const int NUMBER_OF_SEQUENCE_CODEWORDS = 2;
    private static readonly char[] PUNCT_CHARS = new char[29]
    {
      ';',
      '<',
      '>',
      '@',
      '[',
      '\\',
      '}',
      '_',
      '`',
      '~',
      '!',
      '\r',
      '\t',
      ',',
      ':',
      '\n',
      '-',
      '.',
      '$',
      '/',
      '"',
      '|',
      '*',
      '(',
      ')',
      '?',
      '{',
      '}',
      '\''
    };
    private static readonly char[] MIXED_CHARS = new char[25]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '&',
      '\r',
      '\t',
      ',',
      ':',
      '#',
      '-',
      '.',
      '$',
      '/',
      '+',
      '%',
      '*',
      '=',
      '^'
    };
    /// <summary>
    /// Table containing values for the exponent of 900.
    /// This is used in the numeric compaction decode algorithm.
    /// </summary>
    private static readonly BigInteger[] EXP900 = new BigInteger[16];

    static DecodedBitStreamParser()
    {
      DecodedBitStreamParser.EXP900[0] = BigInteger.One;
      BigInteger b = new BigInteger(900L);
      DecodedBitStreamParser.EXP900[1] = b;
      for (int index = 2; index < DecodedBitStreamParser.EXP900.Length; ++index)
        DecodedBitStreamParser.EXP900[index] = BigInteger.Multiplication(DecodedBitStreamParser.EXP900[index - 1], b);
    }

    internal static DecoderResult decode(int[] codewords, string ecLevel)
    {
      StringBuilder result = new StringBuilder(codewords.Length * 2);
      int num1 = 1;
      int[] numArray1 = codewords;
      int index1 = num1;
      int codeIndex1 = index1 + 1;
      int mode = numArray1[index1];
      PDF417ResultMetadata resultMetadata = new PDF417ResultMetadata();
      while (codeIndex1 < codewords[0])
      {
        int num2;
        switch (mode)
        {
          case 900:
            num2 = DecodedBitStreamParser.textCompaction(codewords, codeIndex1, result);
            break;
          case 901:
          case 913:
          case 924:
            num2 = DecodedBitStreamParser.byteCompaction(mode, codewords, codeIndex1, result);
            break;
          case 902:
            num2 = DecodedBitStreamParser.numericCompaction(codewords, codeIndex1, result);
            break;
          case 922:
          case 923:
            return (DecoderResult) null;
          case 928:
            num2 = DecodedBitStreamParser.decodeMacroBlock(codewords, codeIndex1, resultMetadata);
            break;
          default:
            int codeIndex2 = codeIndex1 - 1;
            num2 = DecodedBitStreamParser.textCompaction(codewords, codeIndex2, result);
            break;
        }
        if (num2 < 0)
          return (DecoderResult) null;
        if (num2 >= codewords.Length)
          return (DecoderResult) null;
        int[] numArray2 = codewords;
        int index2 = num2;
        codeIndex1 = index2 + 1;
        mode = numArray2[index2];
      }
      if (result.Length == 0)
        return (DecoderResult) null;
      return new DecoderResult((byte[]) null, result.ToString(), (IList<byte[]>) null, ecLevel)
      {
        Other = (object) resultMetadata
      };
    }

    private static int decodeMacroBlock(
      int[] codewords,
      int codeIndex,
      PDF417ResultMetadata resultMetadata)
    {
      if (codeIndex + 2 > codewords[0])
        return -1;
      int[] codewords1 = new int[2];
      int index = 0;
      while (index < 2)
      {
        codewords1[index] = codewords[codeIndex];
        ++index;
        ++codeIndex;
      }
      string s = DecodedBitStreamParser.decodeBase900toBase10(codewords1, 2);
      if (s == null)
        return -1;
      resultMetadata.SegmentIndex = int.Parse(s);
      StringBuilder result = new StringBuilder();
      codeIndex = DecodedBitStreamParser.textCompaction(codewords, codeIndex, result);
      resultMetadata.FileId = result.ToString();
      if (codewords[codeIndex] == 923)
      {
        ++codeIndex;
        int[] sourceArray = new int[codewords[0] - codeIndex];
        int length = 0;
        bool flag = false;
        while (codeIndex < codewords[0] && !flag)
        {
          int codeword = codewords[codeIndex++];
          if (codeword < 900)
          {
            sourceArray[length++] = codeword;
          }
          else
          {
            if (codeword != 922)
              return -1;
            resultMetadata.IsLastSegment = true;
            ++codeIndex;
            flag = true;
          }
        }
        resultMetadata.OptionalData = new int[length];
        Array.Copy((Array) sourceArray, (Array) resultMetadata.OptionalData, length);
      }
      else if (codewords[codeIndex] == 922)
      {
        resultMetadata.IsLastSegment = true;
        ++codeIndex;
      }
      return codeIndex;
    }

    /// <summary>
    /// Text Compaction mode (see 5.4.1.5) permits all printable ASCII characters to be
    /// encoded, i.e. values 32 - 126 inclusive in accordance with ISO/IEC 646 (IRV), as
    /// well as selected control characters.
    /// 
    /// <param name="codewords">The array of codewords (data + error)</param>
    /// <param name="codeIndex">The current index into the codeword array.</param>
    /// <param name="result">The decoded data is appended to the result.</param>
    /// <returns>The next index into the codeword array.</returns>
    /// </summary>
    private static int textCompaction(int[] codewords, int codeIndex, StringBuilder result)
    {
      int[] textCompactionData = new int[codewords[0] - codeIndex << 1];
      int[] byteCompactionData = new int[codewords[0] - codeIndex << 1];
      int length = 0;
      bool flag = false;
      while (codeIndex < codewords[0] && !flag)
      {
        int codeword1 = codewords[codeIndex++];
        if (codeword1 < 900)
        {
          textCompactionData[length] = codeword1 / 30;
          textCompactionData[length + 1] = codeword1 % 30;
          length += 2;
        }
        else
        {
          switch (codeword1)
          {
            case 900:
              textCompactionData[length++] = 900;
              continue;
            case 901:
            case 902:
            case 922:
            case 923:
            case 924:
            case 928:
              --codeIndex;
              flag = true;
              continue;
            case 913:
              textCompactionData[length] = 913;
              int codeword2 = codewords[codeIndex++];
              byteCompactionData[length] = codeword2;
              ++length;
              continue;
            default:
              continue;
          }
        }
      }
      DecodedBitStreamParser.decodeTextCompaction(textCompactionData, byteCompactionData, length, result);
      return codeIndex;
    }

    /// <summary>
    /// The Text Compaction mode includes all the printable ASCII characters
    /// (i.e. values from 32 to 126) and three ASCII control characters: HT or tab
    /// (ASCII value 9), LF or line feed (ASCII value 10), and CR or carriage
    /// return (ASCII value 13). The Text Compaction mode also includes various latch
    /// and shift characters which are used exclusively within the mode. The Text
    /// Compaction mode encodes up to 2 characters per codeword. The compaction rules
    /// for converting data into PDF417 codewords are defined in 5.4.2.2. The sub-mode
    /// switches are defined in 5.4.2.3.
    /// 
    /// <param name="textCompactionData">The text compaction data.</param>
    /// <param name="byteCompactionData">The byte compaction data if there</param>
    ///                           was a mode shift.
    /// <param name="length">The size of the text compaction and byte compaction data.</param>
    /// <param name="result">The decoded data is appended to the result.</param>
    /// </summary>
    private static void decodeTextCompaction(
      int[] textCompactionData,
      int[] byteCompactionData,
      int length,
      StringBuilder result)
    {
      DecodedBitStreamParser.Mode mode1 = DecodedBitStreamParser.Mode.ALPHA;
      DecodedBitStreamParser.Mode mode2 = DecodedBitStreamParser.Mode.ALPHA;
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = textCompactionData[index1];
        char? nullable1 = new char?();
        switch (mode1)
        {
          case DecodedBitStreamParser.Mode.ALPHA:
            if (index2 < 26)
            {
              nullable1 = new char?((char) (65 + index2));
              break;
            }
            switch (index2)
            {
              case 26:
                nullable1 = new char?(' ');
                break;
              case 27:
                mode1 = DecodedBitStreamParser.Mode.LOWER;
                break;
              case 28:
                mode1 = DecodedBitStreamParser.Mode.MIXED;
                break;
              case 29:
                mode2 = mode1;
                mode1 = DecodedBitStreamParser.Mode.PUNCT_SHIFT;
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 913:
                result.Append((char) byteCompactionData[index1]);
                break;
            }
            break;
          case DecodedBitStreamParser.Mode.LOWER:
            if (index2 < 26)
            {
              nullable1 = new char?((char) (97 + index2));
              break;
            }
            switch (index2)
            {
              case 26:
                nullable1 = new char?(' ');
                break;
              case 27:
                mode2 = mode1;
                mode1 = DecodedBitStreamParser.Mode.ALPHA_SHIFT;
                break;
              case 28:
                mode1 = DecodedBitStreamParser.Mode.MIXED;
                break;
              case 29:
                mode2 = mode1;
                mode1 = DecodedBitStreamParser.Mode.PUNCT_SHIFT;
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 913:
                result.Append((char) byteCompactionData[index1]);
                break;
            }
            break;
          case DecodedBitStreamParser.Mode.MIXED:
            if (index2 < 25)
            {
              nullable1 = new char?(DecodedBitStreamParser.MIXED_CHARS[index2]);
              break;
            }
            switch (index2)
            {
              case 25:
                mode1 = DecodedBitStreamParser.Mode.PUNCT;
                break;
              case 26:
                nullable1 = new char?(' ');
                break;
              case 27:
                mode1 = DecodedBitStreamParser.Mode.LOWER;
                break;
              case 28:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 29:
                mode2 = mode1;
                mode1 = DecodedBitStreamParser.Mode.PUNCT_SHIFT;
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 913:
                result.Append((char) byteCompactionData[index1]);
                break;
            }
            break;
          case DecodedBitStreamParser.Mode.PUNCT:
            if (index2 < 29)
            {
              nullable1 = new char?(DecodedBitStreamParser.PUNCT_CHARS[index2]);
              break;
            }
            switch (index2)
            {
              case 29:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 913:
                result.Append((char) byteCompactionData[index1]);
                break;
            }
            break;
          case DecodedBitStreamParser.Mode.ALPHA_SHIFT:
            mode1 = mode2;
            if (index2 < 26)
            {
              nullable1 = new char?((char) (65 + index2));
              break;
            }
            switch (index2)
            {
              case 26:
                nullable1 = new char?(' ');
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
            }
            break;
          case DecodedBitStreamParser.Mode.PUNCT_SHIFT:
            mode1 = mode2;
            if (index2 < 29)
            {
              nullable1 = new char?(DecodedBitStreamParser.PUNCT_CHARS[index2]);
              break;
            }
            switch (index2)
            {
              case 29:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 900:
                mode1 = DecodedBitStreamParser.Mode.ALPHA;
                break;
              case 913:
                result.Append((char) byteCompactionData[index1]);
                break;
            }
            break;
        }
        char? nullable2 = nullable1;
        if ((nullable2.HasValue ? new int?((int) nullable2.GetValueOrDefault()) : new int?()).HasValue)
          result.Append(nullable1.Value);
      }
    }

    /// <summary>
    /// Byte Compaction mode (see 5.4.3) permits all 256 possible 8-bit byte values to be encoded.
    /// This includes all ASCII characters value 0 to 127 inclusive and provides for international
    /// character set support.
    /// 
    /// <param name="mode">The byte compaction mode i.e. 901 or 924</param>
    /// <param name="codewords">The array of codewords (data + error)</param>
    /// <param name="codeIndex">The current index into the codeword array.</param>
    /// <param name="result">The decoded data is appended to the result.</param>
    /// <returns>The next index into the codeword array.</returns>
    /// </summary>
    private static int byteCompaction(
      int mode,
      int[] codewords,
      int codeIndex,
      StringBuilder result)
    {
      switch (mode)
      {
        case 901:
          int num1 = 0;
          long num2 = 0;
          char[] chArray1 = new char[6];
          int[] numArray = new int[6];
          bool flag1 = false;
          int codeword1 = codewords[codeIndex++];
          while (codeIndex < codewords[0] && !flag1)
          {
            numArray[num1++] = codeword1;
            num2 = 900L * num2 + (long) codeword1;
            codeword1 = codewords[codeIndex++];
            switch (codeword1)
            {
              case 900:
              case 901:
              case 902:
              case 922:
              case 923:
              case 924:
              case 928:
                --codeIndex;
                flag1 = true;
                continue;
              default:
                if (num1 % 5 == 0 && num1 > 0)
                {
                  for (int index = 0; index < 6; ++index)
                  {
                    chArray1[5 - index] = (char) ((ulong) num2 % 256UL);
                    num2 >>= 8;
                  }
                  result.Append(chArray1);
                  num1 = 0;
                  continue;
                }
                continue;
            }
          }
          if (codeIndex == codewords[0] && codeword1 < 900)
            numArray[num1++] = codeword1;
          for (int index = 0; index < num1; ++index)
            result.Append((char) numArray[index]);
          break;
        case 924:
          int num3 = 0;
          long num4 = 0;
          bool flag2 = false;
          while (codeIndex < codewords[0] && !flag2)
          {
            int codeword2 = codewords[codeIndex++];
            if (codeword2 < 900)
            {
              ++num3;
              num4 = 900L * num4 + (long) codeword2;
            }
            else if (codeword2 == 900 || codeword2 == 901 || codeword2 == 902 || codeword2 == 924 || codeword2 == 928 || codeword2 == 923 || codeword2 == 922)
            {
              --codeIndex;
              flag2 = true;
            }
            if (num3 % 5 == 0 && num3 > 0)
            {
              char[] chArray2 = new char[6];
              for (int index = 0; index < 6; ++index)
              {
                chArray2[5 - index] = (char) ((ulong) num4 & (ulong) byte.MaxValue);
                num4 >>= 8;
              }
              result.Append(chArray2);
              num3 = 0;
            }
          }
          break;
      }
      return codeIndex;
    }

    /// <summary>
    /// Numeric Compaction mode (see 5.4.4) permits efficient encoding of numeric data strings.
    /// 
    /// <param name="codewords">The array of codewords (data + error)</param>
    /// <param name="codeIndex">The current index into the codeword array.</param>
    /// <param name="result">The decoded data is appended to the result.</param>
    /// <returns>The next index into the codeword array.</returns>
    /// </summary>
    private static int numericCompaction(int[] codewords, int codeIndex, StringBuilder result)
    {
      int count = 0;
      bool flag = false;
      int[] codewords1 = new int[15];
      while (codeIndex < codewords[0] && !flag)
      {
        int codeword = codewords[codeIndex++];
        if (codeIndex == codewords[0])
          flag = true;
        if (codeword < 900)
        {
          codewords1[count] = codeword;
          ++count;
        }
        else if (codeword == 900 || codeword == 901 || codeword == 924 || codeword == 928 || codeword == 923 || codeword == 922)
        {
          --codeIndex;
          flag = true;
        }
        if (count % 15 == 0 || codeword == 902 || flag)
        {
          string str = DecodedBitStreamParser.decodeBase900toBase10(codewords1, count);
          if (str == null)
            return -1;
          result.Append(str);
          count = 0;
        }
      }
      return codeIndex;
    }

    /// <summary>
    /// Convert a list of Numeric Compacted codewords from Base 900 to Base 10.
    /// EXAMPLE
    /// Encode the fifteen digit numeric string 000213298174000
    /// Prefix the numeric string with a 1 and set the initial value of
    /// t = 1 000 213 298 174 000
    /// Calculate codeword 0
    /// d0 = 1 000 213 298 174 000 mod 900 = 200
    /// 
    /// t = 1 000 213 298 174 000 div 900 = 1 111 348 109 082
    /// Calculate codeword 1
    /// d1 = 1 111 348 109 082 mod 900 = 282
    /// 
    /// t = 1 111 348 109 082 div 900 = 1 234 831 232
    /// Calculate codeword 2
    /// d2 = 1 234 831 232 mod 900 = 632
    /// 
    /// t = 1 234 831 232 div 900 = 1 372 034
    /// Calculate codeword 3
    /// d3 = 1 372 034 mod 900 = 434
    /// 
    /// t = 1 372 034 div 900 = 1 524
    /// Calculate codeword 4
    /// d4 = 1 524 mod 900 = 624
    /// 
    /// t = 1 524 div 900 = 1
    /// Calculate codeword 5
    /// d5 = 1 mod 900 = 1
    /// t = 1 div 900 = 0
    /// Codeword sequence is: 1, 624, 434, 632, 282, 200
    /// 
    /// Decode the above codewords involves
    ///   1 x 900 power of 5 + 624 x 900 power of 4 + 434 x 900 power of 3 +
    /// 632 x 900 power of 2 + 282 x 900 power of 1 + 200 x 900 power of 0 = 1000213298174000
    /// 
    /// Remove leading 1 =&gt;  Result is 000213298174000
    /// <param name="codewords">The array of codewords</param>
    /// <param name="count">The number of codewords</param>
    /// <returns>The decoded string representing the Numeric data.</returns>
    /// </summary>
    private static string decodeBase900toBase10(int[] codewords, int count)
    {
      BigInteger a = BigInteger.Zero;
      for (int index = 0; index < count; ++index)
        a = BigInteger.Addition(a, BigInteger.Multiplication(DecodedBitStreamParser.EXP900[count - index - 1], new BigInteger((long) codewords[index])));
      string str = a.ToString();
      return str[0] != '1' ? (string) null : str.Substring(1);
    }

    private enum Mode
    {
      ALPHA,
      LOWER,
      MIXED,
      PUNCT,
      ALPHA_SHIFT,
      PUNCT_SHIFT,
    }
  }
}
