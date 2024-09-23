// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.DecodedBitStreamParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.QrCode.Internal
{
  /// <summary> <p>QR Codes can encode text as bits in one of several modes, and can use multiple modes
  /// in one QR Code. This class decodes the bits back into text.</p>
  /// 
  /// <p>See ISO 18004:2006, 6.4.3 - 6.4.7</p>
  /// <author>Sean Owen</author>
  /// </summary>
  internal static class DecodedBitStreamParser
  {
    private const int GB2312_SUBSET = 1;
    /// <summary>See ISO 18004:2006, 6.4.4 Table 5</summary>
    private static readonly char[] ALPHANUMERIC_CHARS = new char[45]
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
      ' ',
      '$',
      '%',
      '*',
      '+',
      '-',
      '.',
      '/',
      ':'
    };

    internal static DecoderResult decode(
      byte[] bytes,
      Version version,
      ErrorCorrectionLevel ecLevel,
      IDictionary<DecodeHintType, object> hints)
    {
      BitSource bits = new BitSource(bytes);
      StringBuilder result = new StringBuilder(50);
      List<byte[]> byteSegments = new List<byte[]>(1);
      int saSequence = -1;
      int saParity = -1;
      try
      {
        CharacterSetECI currentCharacterSetECI = (CharacterSetECI) null;
        bool fc1InEffect = false;
        Mode mode;
        do
        {
          if (bits.available() < 4)
          {
            mode = Mode.TERMINATOR;
          }
          else
          {
            try
            {
              mode = Mode.forBits(bits.readBits(4));
            }
            catch (ArgumentException ex)
            {
              return (DecoderResult) null;
            }
          }
          if (mode != Mode.TERMINATOR)
          {
            if (mode == Mode.FNC1_FIRST_POSITION || mode == Mode.FNC1_SECOND_POSITION)
              fc1InEffect = true;
            else if (mode == Mode.STRUCTURED_APPEND)
            {
              if (bits.available() < 16)
                return (DecoderResult) null;
              saSequence = bits.readBits(8);
              saParity = bits.readBits(8);
            }
            else if (mode == Mode.ECI)
            {
              currentCharacterSetECI = CharacterSetECI.getCharacterSetECIByValue(DecodedBitStreamParser.parseECIValue(bits));
              if (currentCharacterSetECI == null)
                return (DecoderResult) null;
            }
            else if (mode == Mode.HANZI)
            {
              int num = bits.readBits(4);
              int count = bits.readBits(mode.getCharacterCountBits(version));
              if (num == 1 && !DecodedBitStreamParser.decodeHanziSegment(bits, result, count))
                return (DecoderResult) null;
            }
            else
            {
              int count = bits.readBits(mode.getCharacterCountBits(version));
              if (mode == Mode.NUMERIC)
              {
                if (!DecodedBitStreamParser.decodeNumericSegment(bits, result, count))
                  return (DecoderResult) null;
              }
              else if (mode == Mode.ALPHANUMERIC)
              {
                if (!DecodedBitStreamParser.decodeAlphanumericSegment(bits, result, count, fc1InEffect))
                  return (DecoderResult) null;
              }
              else if (mode == Mode.BYTE)
              {
                if (!DecodedBitStreamParser.decodeByteSegment(bits, result, count, currentCharacterSetECI, (IList<byte[]>) byteSegments, hints))
                  return (DecoderResult) null;
              }
              else if (mode != Mode.KANJI || !DecodedBitStreamParser.decodeKanjiSegment(bits, result, count))
                return (DecoderResult) null;
            }
          }
        }
        while (mode != Mode.TERMINATOR);
      }
      catch (ArgumentException ex)
      {
        return (DecoderResult) null;
      }
      string text = result.ToString().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
      return new DecoderResult(bytes, text, byteSegments.Count == 0 ? (IList<byte[]>) null : (IList<byte[]>) byteSegments, ecLevel == null ? (string) null : ecLevel.ToString(), saSequence, saParity);
    }

    /// <summary>See specification GBT 18284-2000</summary>
    /// <param name="bits">The bits.</param>
    /// <param name="result">The result.</param>
    /// <param name="count">The count.</param>
    /// <returns></returns>
    private static bool decodeHanziSegment(BitSource bits, StringBuilder result, int count)
    {
      if (count * 13 > bits.available())
        return false;
      byte[] bytes = new byte[2 * count];
      int index = 0;
      for (; count > 0; --count)
      {
        int num1 = bits.readBits(13);
        int num2 = num1 / 96 << 8 | num1 % 96;
        int num3 = num2 >= 959 ? num2 + 42657 : num2 + 41377;
        bytes[index] = (byte) (num3 >> 8 & (int) byte.MaxValue);
        bytes[index + 1] = (byte) (num3 & (int) byte.MaxValue);
        index += 2;
      }
      try
      {
        result.Append(Encoding.GetEncoding(StringUtils.GB2312).GetString(bytes, 0, bytes.Length));
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private static bool decodeKanjiSegment(BitSource bits, StringBuilder result, int count)
    {
      if (count * 13 > bits.available())
        return false;
      byte[] bytes = new byte[2 * count];
      int index = 0;
      for (; count > 0; --count)
      {
        int num1 = bits.readBits(13);
        int num2 = num1 / 192 << 8 | num1 % 192;
        int num3 = num2 >= 7936 ? num2 + 49472 : num2 + 33088;
        bytes[index] = (byte) (num3 >> 8);
        bytes[index + 1] = (byte) num3;
        index += 2;
      }
      try
      {
        result.Append(Encoding.GetEncoding(StringUtils.SHIFT_JIS).GetString(bytes, 0, bytes.Length));
      }
      catch (Exception ex)
      {
        return false;
      }
      return true;
    }

    private static bool decodeByteSegment(
      BitSource bits,
      StringBuilder result,
      int count,
      CharacterSetECI currentCharacterSetECI,
      IList<byte[]> byteSegments,
      IDictionary<DecodeHintType, object> hints)
    {
      if (count << 3 > bits.available())
        return false;
      byte[] bytes = new byte[count];
      for (int index = 0; index < count; ++index)
        bytes[index] = (byte) bits.readBits(8);
      string name = currentCharacterSetECI != null ? currentCharacterSetECI.EncodingName : StringUtils.guessEncoding(bytes, hints);
      try
      {
        result.Append(Encoding.GetEncoding(name).GetString(bytes, 0, bytes.Length));
      }
      catch (Exception ex)
      {
        return false;
      }
      byteSegments.Add(bytes);
      return true;
    }

    private static char toAlphaNumericChar(int value)
    {
      if (value >= DecodedBitStreamParser.ALPHANUMERIC_CHARS.Length)
        throw ZXing.FormatException.Instance;
      return DecodedBitStreamParser.ALPHANUMERIC_CHARS[value];
    }

    private static bool decodeAlphanumericSegment(
      BitSource bits,
      StringBuilder result,
      int count,
      bool fc1InEffect)
    {
      int length = result.Length;
      for (; count > 1; count -= 2)
      {
        if (bits.available() < 11)
          return false;
        int num = bits.readBits(11);
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 45));
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(num % 45));
      }
      if (count == 1)
      {
        if (bits.available() < 6)
          return false;
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(bits.readBits(6)));
      }
      if (fc1InEffect)
      {
        for (int index = length; index < result.Length; ++index)
        {
          if (result[index] == '%')
          {
            if (index < result.Length - 1 && result[index + 1] == '%')
            {
              result.Remove(index + 1, 1);
            }
            else
            {
              result.Remove(index, 1);
              result.Insert(index, new char[1]{ '\u001D' });
            }
          }
        }
      }
      return true;
    }

    private static bool decodeNumericSegment(BitSource bits, StringBuilder result, int count)
    {
      for (; count >= 3; count -= 3)
      {
        if (bits.available() < 10)
          return false;
        int num = bits.readBits(10);
        if (num >= 1000)
          return false;
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 100));
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(num / 10 % 10));
        result.Append(DecodedBitStreamParser.toAlphaNumericChar(num % 10));
      }
      switch (count)
      {
        case 1:
          if (bits.available() < 4)
            return false;
          int num1 = bits.readBits(4);
          if (num1 >= 10)
            return false;
          result.Append(DecodedBitStreamParser.toAlphaNumericChar(num1));
          break;
        case 2:
          if (bits.available() < 7)
            return false;
          int num2 = bits.readBits(7);
          if (num2 >= 100)
            return false;
          result.Append(DecodedBitStreamParser.toAlphaNumericChar(num2 / 10));
          result.Append(DecodedBitStreamParser.toAlphaNumericChar(num2 % 10));
          break;
      }
      return true;
    }

    private static int parseECIValue(BitSource bits)
    {
      int num1 = bits.readBits(8);
      if ((num1 & 128) == 0)
        return num1 & (int) sbyte.MaxValue;
      if ((num1 & 192) == 128)
      {
        int num2 = bits.readBits(8);
        return (num1 & 63) << 8 | num2;
      }
      if ((num1 & 224) != 192)
        throw new ArgumentException("Bad ECI bits starting with byte " + (object) num1);
      int num3 = bits.readBits(16);
      return (num1 & 31) << 16 | num3;
    }
  }
}
