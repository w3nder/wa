// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code39Reader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>Decodes Code 39 barcodes. This does not support "Full ASCII Code 39" yet.</p>
  /// 	<author>Sean Owen</author>
  /// @see Code93Reader
  /// </summary>
  public sealed class Code39Reader : OneDReader
  {
    internal static string ALPHABET_STRING = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. *$/+%";
    private static readonly char[] ALPHABET = Code39Reader.ALPHABET_STRING.ToCharArray();
    /// <summary>
    /// These represent the encodings of characters, as patterns of wide and narrow bars.
    /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow,
    /// with 1s representing "wide" and 0s representing narrow.
    /// </summary>
    internal static int[] CHARACTER_ENCODINGS = new int[44]
    {
      52,
      289,
      97,
      352,
      49,
      304,
      112,
      37,
      292,
      100,
      265,
      73,
      328,
      25,
      280,
      88,
      13,
      268,
      76,
      28,
      259,
      67,
      322,
      19,
      274,
      82,
      7,
      262,
      70,
      22,
      385,
      193,
      448,
      145,
      400,
      208,
      133,
      388,
      196,
      148,
      168,
      162,
      138,
      42
    };
    private static readonly int ASTERISK_ENCODING = Code39Reader.CHARACTER_ENCODINGS[39];
    private readonly bool usingCheckDigit;
    private readonly bool extendedMode;
    private readonly StringBuilder decodeRowResult;
    private readonly int[] counters;

    /// <summary>
    /// Creates a reader that assumes all encoded data is data, and does not treat the final
    /// character as a check digit. It will not decoded "extended Code 39" sequences.
    /// </summary>
    public Code39Reader()
      : this(false)
    {
    }

    /// <summary>
    /// Creates a reader that can be configured to check the last character as a check digit.
    /// It will not decoded "extended Code 39" sequences.
    /// </summary>
    /// <param name="usingCheckDigit">if true, treat the last data character as a check digit, not
    /// data, and verify that the checksum passes.</param>
    public Code39Reader(bool usingCheckDigit)
      : this(usingCheckDigit, false)
    {
    }

    /// <summary>
    /// Creates a reader that can be configured to check the last character as a check digit,
    /// or optionally attempt to decode "extended Code 39" sequences that are used to encode
    /// the full ASCII character set.
    /// </summary>
    /// <param name="usingCheckDigit">if true, treat the last data character as a check digit, not
    /// data, and verify that the checksum passes.</param>
    /// <param name="extendedMode">if true, will attempt to decode extended Code 39 sequences in the text.</param>
    public Code39Reader(bool usingCheckDigit, bool extendedMode)
    {
      this.usingCheckDigit = usingCheckDigit;
      this.extendedMode = extendedMode;
      this.decodeRowResult = new StringBuilder(20);
      this.counters = new int[9];
    }

    /// <summary>
    ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
    /// an image.</p>
    /// </summary>
    /// <param name="rowNumber">row number from top of the row</param>
    /// <param name="row">the black/white pixel data of the row</param>
    /// <param name="hints">decode hints</param>
    /// <returns><see cref="T:ZXing.Result" />containing encoded string and start/end of barcode</returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      for (int index = 0; index < this.counters.Length; ++index)
        this.counters[index] = 0;
      this.decodeRowResult.Length = 0;
      int[] asteriskPattern = Code39Reader.findAsteriskPattern(row, this.counters);
      if (asteriskPattern == null)
        return (Result) null;
      int nextSet = row.getNextSet(asteriskPattern[1]);
      int size = row.Size;
      while (OneDReader.recordPattern(row, nextSet, this.counters))
      {
        int narrowWidePattern = Code39Reader.toNarrowWidePattern(this.counters);
        if (narrowWidePattern < 0)
          return (Result) null;
        char c;
        if (!Code39Reader.patternToChar(narrowWidePattern, out c))
          return (Result) null;
        this.decodeRowResult.Append(c);
        int num1 = nextSet;
        foreach (int counter in this.counters)
          nextSet += counter;
        nextSet = row.getNextSet(nextSet);
        if (c == '*')
        {
          --this.decodeRowResult.Length;
          int num2 = 0;
          foreach (int counter in this.counters)
            num2 += counter;
          int num3 = nextSet - num1 - num2;
          if (nextSet != size && num3 << 1 < num2)
            return (Result) null;
          bool flag1 = this.usingCheckDigit;
          if (hints != null && hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT))
            flag1 = (bool) hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT];
          if (flag1)
          {
            int index1 = this.decodeRowResult.Length - 1;
            int num4 = 0;
            for (int index2 = 0; index2 < index1; ++index2)
              num4 += Code39Reader.ALPHABET_STRING.IndexOf(this.decodeRowResult[index2]);
            if ((int) this.decodeRowResult[index1] != (int) Code39Reader.ALPHABET[num4 % 43])
              return (Result) null;
            this.decodeRowResult.Length = index1;
          }
          if (this.decodeRowResult.Length == 0)
            return (Result) null;
          bool flag2 = this.extendedMode;
          if (hints != null && hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE))
            flag2 = (bool) hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
          string text;
          if (flag2)
          {
            text = Code39Reader.decodeExtended(this.decodeRowResult.ToString());
            if (text == null)
            {
              if (hints == null || !hints.ContainsKey(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE) || !Convert.ToBoolean(hints[DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE]))
                return (Result) null;
              text = this.decodeRowResult.ToString();
            }
          }
          else
            text = this.decodeRowResult.ToString();
          float x1 = (float) (asteriskPattern[1] + asteriskPattern[0]) / 2f;
          float x2 = (float) num1 + (float) num2 / 2f;
          ResultPointCallback hint = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
          if (hint != null)
          {
            hint(new ResultPoint(x1, (float) rowNumber));
            hint(new ResultPoint(x2, (float) rowNumber));
          }
          return new Result(text, (byte[]) null, new ResultPoint[2]
          {
            new ResultPoint(x1, (float) rowNumber),
            new ResultPoint(x2, (float) rowNumber)
          }, BarcodeFormat.CODE_39);
        }
      }
      return (Result) null;
    }

    private static int[] findAsteriskPattern(BitArray row, int[] counters)
    {
      int size = row.Size;
      int nextSet = row.getNextSet(0);
      int index = 0;
      int end = nextSet;
      bool flag = false;
      int length = counters.Length;
      for (int i = nextSet; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++counters[index];
        }
        else
        {
          if (index == length - 1)
          {
            if (Code39Reader.toNarrowWidePattern(counters) == Code39Reader.ASTERISK_ENCODING && row.isRange(Math.Max(0, end - (i - end >> 1)), end, false))
              return new int[2]{ end, i };
            end += counters[0] + counters[1];
            Array.Copy((Array) counters, 2, (Array) counters, 0, length - 2);
            counters[length - 2] = 0;
            counters[length - 1] = 0;
            --index;
          }
          else
            ++index;
          counters[index] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    private static int toNarrowWidePattern(int[] counters)
    {
      int length = counters.Length;
      int num1 = 0;
      int num2;
      do
      {
        int num3 = int.MaxValue;
        foreach (int counter in counters)
        {
          if (counter < num3 && counter > num1)
            num3 = counter;
        }
        num1 = num3;
        num2 = 0;
        int num4 = 0;
        int narrowWidePattern = 0;
        for (int index = 0; index < length; ++index)
        {
          int counter = counters[index];
          if (counter > num1)
          {
            narrowWidePattern |= 1 << length - 1 - index;
            ++num2;
            num4 += counter;
          }
        }
        if (num2 == 3)
        {
          for (int index = 0; index < length && num2 > 0; ++index)
          {
            int counter = counters[index];
            if (counter > num1)
            {
              --num2;
              if (counter << 1 >= num4)
                return -1;
            }
          }
          return narrowWidePattern;
        }
      }
      while (num2 > 3);
      return -1;
    }

    private static bool patternToChar(int pattern, out char c)
    {
      for (int index = 0; index < Code39Reader.CHARACTER_ENCODINGS.Length; ++index)
      {
        if (Code39Reader.CHARACTER_ENCODINGS[index] == pattern)
        {
          c = Code39Reader.ALPHABET[index];
          return true;
        }
      }
      c = '*';
      return false;
    }

    private static string decodeExtended(string encoded)
    {
      int length = encoded.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        char ch1 = encoded[index];
        switch (ch1)
        {
          case '$':
          case '%':
          case '+':
          case '/':
            if (index + 1 >= encoded.Length)
              return (string) null;
            char ch2 = encoded[index + 1];
            char ch3 = char.MinValue;
            switch (ch1)
            {
              case '$':
                if (ch2 < 'A' || ch2 > 'Z')
                  return (string) null;
                ch3 = (char) ((uint) ch2 - 64U);
                break;
              case '%':
                if (ch2 >= 'A' && ch2 <= 'E')
                {
                  ch3 = (char) ((uint) ch2 - 38U);
                  break;
                }
                if (ch2 < 'F' || ch2 > 'W')
                  return (string) null;
                ch3 = (char) ((uint) ch2 - 11U);
                break;
              case '+':
                if (ch2 < 'A' || ch2 > 'Z')
                  return (string) null;
                ch3 = (char) ((uint) ch2 + 32U);
                break;
              case '/':
                if (ch2 >= 'A' && ch2 <= 'O')
                {
                  ch3 = (char) ((uint) ch2 - 32U);
                  break;
                }
                if (ch2 != 'Z')
                  return (string) null;
                ch3 = ':';
                break;
            }
            stringBuilder.Append(ch3);
            ++index;
            break;
          default:
            stringBuilder.Append(ch1);
            break;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
