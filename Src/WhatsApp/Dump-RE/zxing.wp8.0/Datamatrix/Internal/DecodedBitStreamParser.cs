// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.Internal.DecodedBitStreamParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.Datamatrix.Internal
{
  /// <summary>
  /// <p>Data Matrix Codes can encode text as bits in one of several modes, and can use multiple modes
  /// in one Data Matrix Code. This class decodes the bits back into text.</p>
  /// 
  /// <p>See ISO 16022:2006, 5.2.1 - 5.2.9.2</p>
  /// 
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// <author>Sean Owen</author>
  /// </summary>
  internal static class DecodedBitStreamParser
  {
    /// <summary>
    /// See ISO 16022:2006, Annex C Table C.1
    /// The C40 Basic Character Set (*'s used for placeholders for the shift values)
    /// </summary>
    private static readonly char[] C40_BASIC_SET_CHARS = new char[40]
    {
      '*',
      '*',
      '*',
      ' ',
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
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z'
    };
    private static readonly char[] C40_SHIFT2_SET_CHARS = new char[27]
    {
      '!',
      '"',
      '#',
      '$',
      '%',
      '&',
      '\'',
      '(',
      ')',
      '*',
      '+',
      ',',
      '-',
      '.',
      '/',
      ':',
      ';',
      '<',
      '=',
      '>',
      '?',
      '@',
      '[',
      '\\',
      ']',
      '^',
      '_'
    };
    /// <summary>
    /// See ISO 16022:2006, Annex C Table C.2
    /// The Text Basic Character Set (*'s used for placeholders for the shift values)
    /// </summary>
    private static readonly char[] TEXT_BASIC_SET_CHARS = new char[40]
    {
      '*',
      '*',
      '*',
      ' ',
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
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z'
    };
    private static readonly char[] TEXT_SHIFT3_SET_CHARS = new char[32]
    {
      '`',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      '{',
      '|',
      '}',
      '~',
      '\u007F'
    };

    internal static DecoderResult decode(byte[] bytes)
    {
      BitSource bits = new BitSource(bytes);
      StringBuilder result = new StringBuilder(100);
      StringBuilder resultTrailer = new StringBuilder(0);
      List<byte[]> byteSegments = new List<byte[]>(1);
      DecodedBitStreamParser.Mode mode = DecodedBitStreamParser.Mode.ASCII_ENCODE;
      do
      {
        switch (mode)
        {
          case DecodedBitStreamParser.Mode.ASCII_ENCODE:
            if (!DecodedBitStreamParser.decodeAsciiSegment(bits, result, resultTrailer, out mode))
              return (DecoderResult) null;
            goto label_16;
          case DecodedBitStreamParser.Mode.C40_ENCODE:
            if (!DecodedBitStreamParser.decodeC40Segment(bits, result))
              return (DecoderResult) null;
            break;
          case DecodedBitStreamParser.Mode.TEXT_ENCODE:
            if (!DecodedBitStreamParser.decodeTextSegment(bits, result))
              return (DecoderResult) null;
            break;
          case DecodedBitStreamParser.Mode.ANSIX12_ENCODE:
            if (!DecodedBitStreamParser.decodeAnsiX12Segment(bits, result))
              return (DecoderResult) null;
            break;
          case DecodedBitStreamParser.Mode.EDIFACT_ENCODE:
            if (!DecodedBitStreamParser.decodeEdifactSegment(bits, result))
              return (DecoderResult) null;
            break;
          case DecodedBitStreamParser.Mode.BASE256_ENCODE:
            if (!DecodedBitStreamParser.decodeBase256Segment(bits, result, (IList<byte[]>) byteSegments))
              return (DecoderResult) null;
            break;
          default:
            return (DecoderResult) null;
        }
        mode = DecodedBitStreamParser.Mode.ASCII_ENCODE;
label_16:;
      }
      while (mode != DecodedBitStreamParser.Mode.PAD_ENCODE && bits.available() > 0);
      if (resultTrailer.Length > 0)
        result.Append(resultTrailer.ToString());
      return new DecoderResult(bytes, result.ToString(), byteSegments.Count == 0 ? (IList<byte[]>) null : (IList<byte[]>) byteSegments, (string) null);
    }

    /// <summary>See ISO 16022:2006, 5.2.3 and Annex C, Table C.2</summary>
    private static bool decodeAsciiSegment(
      BitSource bits,
      StringBuilder result,
      StringBuilder resultTrailer,
      out DecodedBitStreamParser.Mode mode)
    {
      bool flag = false;
      mode = DecodedBitStreamParser.Mode.ASCII_ENCODE;
      do
      {
        int num1 = bits.readBits(8);
        if (num1 == 0)
          return false;
        if (num1 <= 128)
        {
          if (flag)
            num1 += 128;
          result.Append((char) (num1 - 1));
          mode = DecodedBitStreamParser.Mode.ASCII_ENCODE;
          return true;
        }
        if (num1 == 129)
        {
          mode = DecodedBitStreamParser.Mode.PAD_ENCODE;
          return true;
        }
        if (num1 <= 229)
        {
          int num2 = num1 - 130;
          if (num2 < 10)
            result.Append('0');
          result.Append(num2);
        }
        else
        {
          if (num1 == 230)
          {
            mode = DecodedBitStreamParser.Mode.C40_ENCODE;
            return true;
          }
          if (num1 == 231)
          {
            mode = DecodedBitStreamParser.Mode.BASE256_ENCODE;
            return true;
          }
          if (num1 == 232)
            result.Append('\u001D');
          else if (num1 != 233 && num1 != 234)
          {
            switch (num1)
            {
              case 235:
                flag = true;
                break;
              case 236:
                result.Append("[)>\u001E05\u001D");
                resultTrailer.Insert(0, "\u001E\u0004");
                break;
              case 237:
                result.Append("[)>\u001E06\u001D");
                resultTrailer.Insert(0, "\u001E\u0004");
                break;
              case 238:
                mode = DecodedBitStreamParser.Mode.ANSIX12_ENCODE;
                return true;
              case 239:
                mode = DecodedBitStreamParser.Mode.TEXT_ENCODE;
                return true;
              case 240:
                mode = DecodedBitStreamParser.Mode.EDIFACT_ENCODE;
                return true;
              default:
                if (num1 != 241 && num1 >= 242 && (num1 != 254 || bits.available() != 0))
                  return false;
                break;
            }
          }
        }
      }
      while (bits.available() > 0);
      mode = DecodedBitStreamParser.Mode.ASCII_ENCODE;
      return true;
    }

    /// <summary>See ISO 16022:2006, 5.2.5 and Annex C, Table C.1</summary>
    private static bool decodeC40Segment(BitSource bits, StringBuilder result)
    {
      bool flag = false;
      int[] result1 = new int[3];
      int num = 0;
      while (bits.available() != 8)
      {
        int firstByte = bits.readBits(8);
        if (firstByte == 254)
          return true;
        DecodedBitStreamParser.parseTwoBytes(firstByte, bits.readBits(8), result1);
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int index2 = result1[index1];
          switch (num)
          {
            case 0:
              if (index2 < 3)
              {
                num = index2 + 1;
                break;
              }
              if (index2 >= DecodedBitStreamParser.C40_BASIC_SET_CHARS.Length)
                return false;
              char c40BasicSetChar = DecodedBitStreamParser.C40_BASIC_SET_CHARS[index2];
              if (flag)
              {
                result.Append((char) ((uint) c40BasicSetChar + 128U));
                flag = false;
                break;
              }
              result.Append(c40BasicSetChar);
              break;
            case 1:
              if (flag)
              {
                result.Append((char) (index2 + 128));
                flag = false;
              }
              else
                result.Append((char) index2);
              num = 0;
              break;
            case 2:
              if (index2 < DecodedBitStreamParser.C40_SHIFT2_SET_CHARS.Length)
              {
                char c40ShifT2SetChar = DecodedBitStreamParser.C40_SHIFT2_SET_CHARS[index2];
                if (flag)
                {
                  result.Append((char) ((uint) c40ShifT2SetChar + 128U));
                  flag = false;
                }
                else
                  result.Append(c40ShifT2SetChar);
              }
              else
              {
                switch (index2)
                {
                  case 27:
                    result.Append('\u001D');
                    break;
                  case 30:
                    flag = true;
                    break;
                  default:
                    return false;
                }
              }
              num = 0;
              break;
            case 3:
              if (flag)
              {
                result.Append((char) (index2 + 224));
                flag = false;
              }
              else
                result.Append((char) (index2 + 96));
              num = 0;
              break;
            default:
              return false;
          }
        }
        if (bits.available() <= 0)
          return true;
      }
      return true;
    }

    /// <summary>See ISO 16022:2006, 5.2.6 and Annex C, Table C.2</summary>
    private static bool decodeTextSegment(BitSource bits, StringBuilder result)
    {
      bool flag = false;
      int[] result1 = new int[3];
      int num = 0;
      while (bits.available() != 8)
      {
        int firstByte = bits.readBits(8);
        if (firstByte == 254)
          return true;
        DecodedBitStreamParser.parseTwoBytes(firstByte, bits.readBits(8), result1);
        for (int index1 = 0; index1 < 3; ++index1)
        {
          int index2 = result1[index1];
          switch (num)
          {
            case 0:
              if (index2 < 3)
              {
                num = index2 + 1;
                break;
              }
              if (index2 >= DecodedBitStreamParser.TEXT_BASIC_SET_CHARS.Length)
                return false;
              char textBasicSetChar = DecodedBitStreamParser.TEXT_BASIC_SET_CHARS[index2];
              if (flag)
              {
                result.Append((char) ((uint) textBasicSetChar + 128U));
                flag = false;
                break;
              }
              result.Append(textBasicSetChar);
              break;
            case 1:
              if (flag)
              {
                result.Append((char) (index2 + 128));
                flag = false;
              }
              else
                result.Append((char) index2);
              num = 0;
              break;
            case 2:
              if (index2 < DecodedBitStreamParser.C40_SHIFT2_SET_CHARS.Length)
              {
                char c40ShifT2SetChar = DecodedBitStreamParser.C40_SHIFT2_SET_CHARS[index2];
                if (flag)
                {
                  result.Append((char) ((uint) c40ShifT2SetChar + 128U));
                  flag = false;
                }
                else
                  result.Append(c40ShifT2SetChar);
              }
              else
              {
                switch (index2)
                {
                  case 27:
                    result.Append('\u001D');
                    break;
                  case 30:
                    flag = true;
                    break;
                  default:
                    return false;
                }
              }
              num = 0;
              break;
            case 3:
              if (index2 >= DecodedBitStreamParser.TEXT_SHIFT3_SET_CHARS.Length)
                return false;
              char textShifT3SetChar = DecodedBitStreamParser.TEXT_SHIFT3_SET_CHARS[index2];
              if (flag)
              {
                result.Append((char) ((uint) textShifT3SetChar + 128U));
                flag = false;
              }
              else
                result.Append(textShifT3SetChar);
              num = 0;
              break;
            default:
              return false;
          }
        }
        if (bits.available() <= 0)
          return true;
      }
      return true;
    }

    /// <summary>See ISO 16022:2006, 5.2.7</summary>
    private static bool decodeAnsiX12Segment(BitSource bits, StringBuilder result)
    {
      int[] result1 = new int[3];
      while (bits.available() != 8)
      {
        int firstByte = bits.readBits(8);
        if (firstByte == 254)
          return true;
        DecodedBitStreamParser.parseTwoBytes(firstByte, bits.readBits(8), result1);
        for (int index = 0; index < 3; ++index)
        {
          int num = result1[index];
          switch (num)
          {
            case 0:
              result.Append('\r');
              break;
            case 1:
              result.Append('*');
              break;
            case 2:
              result.Append('>');
              break;
            case 3:
              result.Append(' ');
              break;
            default:
              if (num < 14)
              {
                result.Append((char) (num + 44));
                break;
              }
              if (num >= 40)
                return false;
              result.Append((char) (num + 51));
              break;
          }
        }
        if (bits.available() <= 0)
          return true;
      }
      return true;
    }

    private static void parseTwoBytes(int firstByte, int secondByte, int[] result)
    {
      int num1 = (firstByte << 8) + secondByte - 1;
      int num2 = num1 / 1600;
      result[0] = num2;
      int num3 = num1 - num2 * 1600;
      int num4 = num3 / 40;
      result[1] = num4;
      result[2] = num3 - num4 * 40;
    }

    /// <summary>See ISO 16022:2006, 5.2.8 and Annex C Table C.3</summary>
    private static bool decodeEdifactSegment(BitSource bits, StringBuilder result)
    {
      while (bits.available() > 16)
      {
        for (int index = 0; index < 4; ++index)
        {
          int num = bits.readBits(6);
          if (num == 31)
          {
            int numBits = 8 - bits.BitOffset;
            if (numBits != 8)
              bits.readBits(numBits);
            return true;
          }
          if ((num & 32) == 0)
            num |= 64;
          result.Append((char) num);
        }
        if (bits.available() <= 0)
          return true;
      }
      return true;
    }

    /// <summary>See ISO 16022:2006, 5.2.9 and Annex B, B.2</summary>
    private static bool decodeBase256Segment(
      BitSource bits,
      StringBuilder result,
      IList<byte[]> byteSegments)
    {
      int num1 = 1 + bits.ByteOffset;
      int randomizedBase256Codeword = bits.readBits(8);
      int base256CodewordPosition = num1;
      int num2 = base256CodewordPosition + 1;
      int num3 = DecodedBitStreamParser.unrandomize255State(randomizedBase256Codeword, base256CodewordPosition);
      int length = num3 != 0 ? (num3 >= 250 ? 250 * (num3 - 249) + DecodedBitStreamParser.unrandomize255State(bits.readBits(8), num2++) : num3) : bits.available() / 8;
      if (length < 0)
        return false;
      byte[] bytes = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        if (bits.available() < 8)
          return false;
        bytes[index] = (byte) DecodedBitStreamParser.unrandomize255State(bits.readBits(8), num2++);
      }
      byteSegments.Add(bytes);
      try
      {
        result.Append(Encoding.GetEncoding("ISO-8859-1").GetString(bytes, 0, bytes.Length));
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Platform does not support required encoding: " + (object) ex);
      }
      return true;
    }

    /// <summary>See ISO 16022:2006, Annex B, B.2</summary>
    private static int unrandomize255State(
      int randomizedBase256Codeword,
      int base256CodewordPosition)
    {
      int num1 = 149 * base256CodewordPosition % (int) byte.MaxValue + 1;
      int num2 = randomizedBase256Codeword - num1;
      return num2 < 0 ? num2 + 256 : num2;
    }

    private enum Mode
    {
      PAD_ENCODE,
      ASCII_ENCODE,
      C40_ENCODE,
      TEXT_ENCODE,
      ANSIX12_ENCODE,
      EDIFACT_ENCODE,
      BASE256_ENCODE,
    }
  }
}
