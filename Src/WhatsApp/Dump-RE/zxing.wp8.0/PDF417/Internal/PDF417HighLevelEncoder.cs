// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.PDF417HighLevelEncoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using BigIntegerLibrary;
using System;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// PDF417 high-level encoder following the algorithm described in ISO/IEC 15438:2001(E) in
  /// annex P.
  /// </summary>
  internal static class PDF417HighLevelEncoder
  {
    /// <summary>code for Text compaction</summary>
    private const int TEXT_COMPACTION = 0;
    /// <summary>code for Byte compaction</summary>
    private const int BYTE_COMPACTION = 1;
    /// <summary>code for Numeric compaction</summary>
    private const int NUMERIC_COMPACTION = 2;
    /// <summary>Text compaction submode Alpha</summary>
    private const int SUBMODE_ALPHA = 0;
    /// <summary>Text compaction submode Lower</summary>
    private const int SUBMODE_LOWER = 1;
    /// <summary>Text compaction submode Mixed</summary>
    private const int SUBMODE_MIXED = 2;
    /// <summary>Text compaction submode Punctuation</summary>
    private const int SUBMODE_PUNCTUATION = 3;
    /// <summary>mode latch to Text Compaction mode</summary>
    private const int LATCH_TO_TEXT = 900;
    /// <summary>
    /// mode latch to Byte Compaction mode (number of characters NOT a multiple of 6)
    /// </summary>
    private const int LATCH_TO_BYTE_PADDED = 901;
    /// <summary>mode latch to Numeric Compaction mode</summary>
    private const int LATCH_TO_NUMERIC = 902;
    /// <summary>mode shift to Byte Compaction mode</summary>
    private const int SHIFT_TO_BYTE = 913;
    /// <summary>
    /// mode latch to Byte Compaction mode (number of characters a multiple of 6)
    /// </summary>
    private const int LATCH_TO_BYTE = 924;
    /// <summary>
    /// identifier for a user defined Extended Channel Interpretation (ECI)
    /// </summary>
    private const int ECI_USER_DEFINED = 925;
    /// <summary>identifier for a general purpose ECO format</summary>
    private const int ECI_GENERAL_PURPOSE = 926;
    /// <summary>identifier for an ECI of a character set of code page</summary>
    private const int ECI_CHARSET = 927;
    /// <summary>Raw code table for text compaction Mixed sub-mode</summary>
    private static readonly sbyte[] TEXT_MIXED_RAW = new sbyte[30]
    {
      (sbyte) 48,
      (sbyte) 49,
      (sbyte) 50,
      (sbyte) 51,
      (sbyte) 52,
      (sbyte) 53,
      (sbyte) 54,
      (sbyte) 55,
      (sbyte) 56,
      (sbyte) 57,
      (sbyte) 38,
      (sbyte) 13,
      (sbyte) 9,
      (sbyte) 44,
      (sbyte) 58,
      (sbyte) 35,
      (sbyte) 45,
      (sbyte) 46,
      (sbyte) 36,
      (sbyte) 47,
      (sbyte) 43,
      (sbyte) 37,
      (sbyte) 42,
      (sbyte) 61,
      (sbyte) 94,
      (sbyte) 0,
      (sbyte) 32,
      (sbyte) 0,
      (sbyte) 0,
      (sbyte) 0
    };
    /// <summary>
    /// Raw code table for text compaction: Punctuation sub-mode
    /// </summary>
    private static readonly sbyte[] TEXT_PUNCTUATION_RAW = new sbyte[30]
    {
      (sbyte) 59,
      (sbyte) 60,
      (sbyte) 62,
      (sbyte) 64,
      (sbyte) 91,
      (sbyte) 92,
      (sbyte) 93,
      (sbyte) 95,
      (sbyte) 96,
      (sbyte) 126,
      (sbyte) 33,
      (sbyte) 13,
      (sbyte) 9,
      (sbyte) 44,
      (sbyte) 58,
      (sbyte) 10,
      (sbyte) 45,
      (sbyte) 46,
      (sbyte) 36,
      (sbyte) 47,
      (sbyte) 34,
      (sbyte) 124,
      (sbyte) 42,
      (sbyte) 40,
      (sbyte) 41,
      (sbyte) 63,
      (sbyte) 123,
      (sbyte) 125,
      (sbyte) 39,
      (sbyte) 0
    };
    private static readonly sbyte[] MIXED = new sbyte[128];
    private static readonly sbyte[] PUNCTUATION = new sbyte[128];
    internal static Encoding DEFAULT_ENCODING;

    static PDF417HighLevelEncoder()
    {
      for (int index = 0; index < PDF417HighLevelEncoder.MIXED.Length; ++index)
        PDF417HighLevelEncoder.MIXED[index] = (sbyte) -1;
      for (sbyte index1 = 0; (int) index1 < PDF417HighLevelEncoder.TEXT_MIXED_RAW.Length; ++index1)
      {
        sbyte index2 = PDF417HighLevelEncoder.TEXT_MIXED_RAW[(int) index1];
        if (index2 > (sbyte) 0)
          PDF417HighLevelEncoder.MIXED[(int) index2] = index1;
      }
      for (int index = 0; index < PDF417HighLevelEncoder.PUNCTUATION.Length; ++index)
        PDF417HighLevelEncoder.PUNCTUATION[index] = (sbyte) -1;
      for (sbyte index3 = 0; (int) index3 < PDF417HighLevelEncoder.TEXT_PUNCTUATION_RAW.Length; ++index3)
      {
        sbyte index4 = PDF417HighLevelEncoder.TEXT_PUNCTUATION_RAW[(int) index3];
        if (index4 > (sbyte) 0)
          PDF417HighLevelEncoder.PUNCTUATION[(int) index4] = index3;
      }
      PDF417HighLevelEncoder.DEFAULT_ENCODING = Encoding.GetEncoding("UTF-8");
    }

    /// <summary>
    /// Performs high-level encoding of a PDF417 message using the algorithm described in annex P
    /// of ISO/IEC 15438:2001(E). If byte compaction has been selected, then only byte compaction
    /// is used.
    /// 
    /// <param name="msg">the message</param>
    /// <returns>the encoded message (the char values range from 0 to 928)</returns>
    /// </summary>
    internal static string encodeHighLevel(
      string msg,
      Compaction compaction,
      Encoding encoding,
      bool disableEci)
    {
      StringBuilder sb = new StringBuilder(msg.Length);
      if (!PDF417HighLevelEncoder.DEFAULT_ENCODING.Equals((object) encoding) && !disableEci)
      {
        CharacterSetECI characterSetEciByName = CharacterSetECI.getCharacterSetECIByName(encoding.WebName);
        if (characterSetEciByName != null)
          PDF417HighLevelEncoder.encodingECI(characterSetEciByName.Value, sb);
      }
      int length = msg.Length;
      int startpos = 0;
      int initialSubmode = 0;
      byte[] bytes1 = (byte[]) null;
      switch (compaction)
      {
        case Compaction.TEXT:
          PDF417HighLevelEncoder.encodeText(msg, startpos, length, sb, initialSubmode);
          break;
        case Compaction.BYTE:
          byte[] bytes2 = encoding.GetBytes(msg);
          PDF417HighLevelEncoder.encodeBinary(bytes2, startpos, bytes2.Length, 1, sb);
          break;
        case Compaction.NUMERIC:
          sb.Append('Ά');
          PDF417HighLevelEncoder.encodeNumeric(msg, startpos, length, sb);
          break;
        default:
          int startmode = 0;
          while (startpos < length)
          {
            int consecutiveDigitCount = PDF417HighLevelEncoder.determineConsecutiveDigitCount(msg, startpos);
            if (consecutiveDigitCount >= 13)
            {
              sb.Append('Ά');
              startmode = 2;
              initialSubmode = 0;
              PDF417HighLevelEncoder.encodeNumeric(msg, startpos, consecutiveDigitCount, sb);
              startpos += consecutiveDigitCount;
            }
            else
            {
              int consecutiveTextCount = PDF417HighLevelEncoder.determineConsecutiveTextCount(msg, startpos);
              if (consecutiveTextCount >= 5 || consecutiveDigitCount == length)
              {
                if (startmode != 0)
                {
                  sb.Append('΄');
                  startmode = 0;
                  initialSubmode = 0;
                }
                initialSubmode = PDF417HighLevelEncoder.encodeText(msg, startpos, consecutiveTextCount, sb, initialSubmode);
                startpos += consecutiveTextCount;
              }
              else
              {
                if (bytes1 == null)
                  bytes1 = encoding.GetBytes(msg);
                int count = PDF417HighLevelEncoder.determineConsecutiveBinaryCount(msg, bytes1, startpos);
                if (count == 0)
                  count = 1;
                if (count == 1 && startmode == 0)
                {
                  PDF417HighLevelEncoder.encodeBinary(bytes1, startpos, 1, 0, sb);
                }
                else
                {
                  PDF417HighLevelEncoder.encodeBinary(bytes1, startpos, count, startmode, sb);
                  startmode = 1;
                  initialSubmode = 0;
                }
                startpos += count;
              }
            }
          }
          break;
      }
      return sb.ToString();
    }

    /// <summary>
    /// Encode parts of the message using Text Compaction as described in ISO/IEC 15438:2001(E),
    /// chapter 4.4.2.
    /// 
    /// <param name="msg">the message</param>
    /// <param name="startpos">the start position within the message</param>
    /// <param name="count">the number of characters to encode</param>
    /// <param name="sb">receives the encoded codewords</param>
    /// <param name="initialSubmode">should normally be SUBMODE_ALPHA</param>
    /// <returns>the text submode in which this method ends</returns>
    /// </summary>
    private static int encodeText(
      string msg,
      int startpos,
      int count,
      StringBuilder sb,
      int initialSubmode)
    {
      StringBuilder stringBuilder = new StringBuilder(count);
      int num1 = initialSubmode;
      int num2 = 0;
      do
      {
        char ch = msg[startpos + num2];
        switch (num1)
        {
          case 0:
            if (PDF417HighLevelEncoder.isAlphaUpper(ch))
            {
              if (ch == ' ')
              {
                stringBuilder.Append('\u001A');
                break;
              }
              stringBuilder.Append((char) ((uint) ch - 65U));
              break;
            }
            if (PDF417HighLevelEncoder.isAlphaLower(ch))
            {
              num1 = 1;
              stringBuilder.Append('\u001B');
              continue;
            }
            if (PDF417HighLevelEncoder.isMixed(ch))
            {
              num1 = 2;
              stringBuilder.Append('\u001C');
              continue;
            }
            stringBuilder.Append('\u001D');
            stringBuilder.Append((char) PDF417HighLevelEncoder.PUNCTUATION[(int) ch]);
            break;
          case 1:
            if (PDF417HighLevelEncoder.isAlphaLower(ch))
            {
              if (ch == ' ')
              {
                stringBuilder.Append('\u001A');
                break;
              }
              stringBuilder.Append((char) ((uint) ch - 97U));
              break;
            }
            if (PDF417HighLevelEncoder.isAlphaUpper(ch))
            {
              stringBuilder.Append('\u001B');
              stringBuilder.Append((char) ((uint) ch - 65U));
              break;
            }
            if (PDF417HighLevelEncoder.isMixed(ch))
            {
              num1 = 2;
              stringBuilder.Append('\u001C');
              continue;
            }
            stringBuilder.Append('\u001D');
            stringBuilder.Append((char) PDF417HighLevelEncoder.PUNCTUATION[(int) ch]);
            break;
          case 2:
            if (PDF417HighLevelEncoder.isMixed(ch))
            {
              stringBuilder.Append((char) PDF417HighLevelEncoder.MIXED[(int) ch]);
              break;
            }
            if (PDF417HighLevelEncoder.isAlphaUpper(ch))
            {
              num1 = 0;
              stringBuilder.Append('\u001C');
              continue;
            }
            if (PDF417HighLevelEncoder.isAlphaLower(ch))
            {
              num1 = 1;
              stringBuilder.Append('\u001B');
              continue;
            }
            if (startpos + num2 + 1 < count && PDF417HighLevelEncoder.isPunctuation(msg[startpos + num2 + 1]))
            {
              num1 = 3;
              stringBuilder.Append('\u0019');
              continue;
            }
            stringBuilder.Append('\u001D');
            stringBuilder.Append((char) PDF417HighLevelEncoder.PUNCTUATION[(int) ch]);
            break;
          default:
            if (PDF417HighLevelEncoder.isPunctuation(ch))
            {
              stringBuilder.Append((char) PDF417HighLevelEncoder.PUNCTUATION[(int) ch]);
              break;
            }
            num1 = 0;
            stringBuilder.Append('\u001D');
            continue;
        }
        ++num2;
      }
      while (num2 < count);
      char ch1 = char.MinValue;
      int length = stringBuilder.Length;
      for (int index = 0; index < length; ++index)
      {
        if (index % 2 != 0)
        {
          ch1 = (char) ((uint) ch1 * 30U + (uint) stringBuilder[index]);
          sb.Append(ch1);
        }
        else
          ch1 = stringBuilder[index];
      }
      if (length % 2 != 0)
        sb.Append((char) ((int) ch1 * 30 + 29));
      return num1;
    }

    /// <summary>
    /// Encode parts of the message using Byte Compaction as described in ISO/IEC 15438:2001(E),
    /// chapter 4.4.3. The Unicode characters will be converted to binary using the cp437
    /// codepage.
    /// 
    /// <param name="bytes">the message converted to a byte array</param>
    /// <param name="startpos">the start position within the message</param>
    /// <param name="count">the number of bytes to encode</param>
    /// <param name="startmode">the mode from which this method starts</param>
    /// <param name="sb">receives the encoded codewords</param>
    /// </summary>
    private static void encodeBinary(
      byte[] bytes,
      int startpos,
      int count,
      int startmode,
      StringBuilder sb)
    {
      if (count == 1 && startmode == 0)
        sb.Append('Α');
      else if (count % 6 == 0)
        sb.Append('Μ');
      else
        sb.Append('΅');
      int num1 = startpos;
      if (count >= 6)
      {
        char[] chArray = new char[5];
        for (; startpos + count - num1 >= 6; num1 += 6)
        {
          long num2 = 0;
          for (int index = 0; index < 6; ++index)
            num2 = (num2 << 8) + (long) ((int) bytes[num1 + index] & (int) byte.MaxValue);
          for (int index = 0; index < 5; ++index)
          {
            chArray[index] = (char) ((ulong) num2 % 900UL);
            num2 /= 900L;
          }
          for (int index = chArray.Length - 1; index >= 0; --index)
            sb.Append(chArray[index]);
        }
      }
      for (int index = num1; index < startpos + count; ++index)
      {
        int num3 = (int) bytes[index] & (int) byte.MaxValue;
        sb.Append((char) num3);
      }
    }

    private static void encodeNumeric(string msg, int startpos, int count, StringBuilder sb)
    {
      int num = 0;
      StringBuilder stringBuilder = new StringBuilder(count / 3 + 1);
      BigInteger b = new BigInteger(900L);
      BigInteger other = new BigInteger(0L);
      int length;
      for (; num < count - 1; num += length)
      {
        stringBuilder.Length = 0;
        length = Math.Min(44, count - num);
        BigInteger a = BigInteger.Parse('1'.ToString() + msg.Substring(startpos + num, length));
        do
        {
          BigInteger bigInteger = BigInteger.Modulo(a, b);
          stringBuilder.Append((char) bigInteger.GetHashCode());
          a = BigInteger.Division(a, b);
        }
        while (!a.Equals(other));
        for (int index = stringBuilder.Length - 1; index >= 0; --index)
          sb.Append(stringBuilder[index]);
      }
    }

    private static bool isDigit(char ch) => ch >= '0' && ch <= '9';

    private static bool isAlphaUpper(char ch)
    {
      if (ch == ' ')
        return true;
      return ch >= 'A' && ch <= 'Z';
    }

    private static bool isAlphaLower(char ch)
    {
      if (ch == ' ')
        return true;
      return ch >= 'a' && ch <= 'z';
    }

    private static bool isMixed(char ch) => PDF417HighLevelEncoder.MIXED[(int) ch] != (sbyte) -1;

    private static bool isPunctuation(char ch)
    {
      return PDF417HighLevelEncoder.PUNCTUATION[(int) ch] != (sbyte) -1;
    }

    private static bool isText(char ch)
    {
      if (ch == '\t' || ch == '\n' || ch == '\r')
        return true;
      return ch >= ' ' && ch <= '~';
    }

    /// <summary>
    /// Determines the number of consecutive characters that are encodable using numeric compaction.
    /// 
    /// <param name="msg">the message</param>
    /// <param name="startpos">the start position within the message</param>
    /// <returns>the requested character count</returns>
    /// </summary>
    private static int determineConsecutiveDigitCount(string msg, int startpos)
    {
      int consecutiveDigitCount = 0;
      int length = msg.Length;
      int index = startpos;
      if (index < length)
      {
        char ch = msg[index];
        while (PDF417HighLevelEncoder.isDigit(ch) && index < length)
        {
          ++consecutiveDigitCount;
          ++index;
          if (index < length)
            ch = msg[index];
        }
      }
      return consecutiveDigitCount;
    }

    /// <summary>
    /// Determines the number of consecutive characters that are encodable using text compaction.
    /// 
    /// <param name="msg">the message</param>
    /// <param name="startpos">the start position within the message</param>
    /// <returns>the requested character count</returns>
    /// </summary>
    private static int determineConsecutiveTextCount(string msg, int startpos)
    {
      int length = msg.Length;
      int index = startpos;
      while (index < length)
      {
        char ch = msg[index];
        int num = 0;
        while (num < 13 && PDF417HighLevelEncoder.isDigit(ch) && index < length)
        {
          ++num;
          ++index;
          if (index < length)
            ch = msg[index];
        }
        if (num >= 13)
          return index - startpos - num;
        if (num <= 0)
        {
          if (PDF417HighLevelEncoder.isText(msg[index]))
            ++index;
          else
            break;
        }
      }
      return index - startpos;
    }

    /// <summary>
    /// Determines the number of consecutive characters that are encodable using binary compaction.
    /// 
    /// <param name="msg">the message</param>
    /// <param name="bytes">the message converted to a byte array</param>
    /// <param name="startpos">the start position within the message</param>
    /// <returns>the requested character count</returns>
    /// </summary>
    private static int determineConsecutiveBinaryCount(string msg, byte[] bytes, int startpos)
    {
      int length = msg.Length;
      int index1;
      for (index1 = startpos; index1 < length; ++index1)
      {
        char ch1 = msg[index1];
        int num1;
        int index2;
        for (num1 = 0; num1 < 13 && PDF417HighLevelEncoder.isDigit(ch1); ch1 = msg[index2])
        {
          ++num1;
          index2 = index1 + num1;
          if (index2 >= length)
            break;
        }
        if (num1 >= 13)
          return index1 - startpos;
        int num2;
        int index3;
        for (num2 = 0; num2 < 5 && PDF417HighLevelEncoder.isText(ch1); ch1 = msg[index3])
        {
          ++num2;
          index3 = index1 + num2;
          if (index3 >= length)
            break;
        }
        if (num2 >= 5)
          return index1 - startpos;
        char ch2 = msg[index1];
        if (bytes[index1] == (byte) 63 && ch2 != '?')
          throw new WriterException("Non-encodable character detected: " + (object) ch2 + " (Unicode: " + (object) (int) ch2 + (object) ')');
      }
      return index1 - startpos;
    }

    private static void encodingECI(int eci, StringBuilder sb)
    {
      if (eci >= 0 && eci < 900)
      {
        sb.Append('Ο');
        sb.Append((char) eci);
      }
      else if (eci < 810900)
      {
        sb.Append('Ξ');
        sb.Append((char) (eci / 900 - 1));
        sb.Append((char) (eci % 900));
      }
      else
      {
        if (eci >= 811800)
          throw new WriterException("ECI number not in valid range from 0..811799, but was " + (object) eci);
        sb.Append('Ν');
        sb.Append((char) (810900 - eci));
      }
    }
  }
}
