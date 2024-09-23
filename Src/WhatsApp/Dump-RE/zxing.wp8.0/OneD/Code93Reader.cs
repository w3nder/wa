// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code93Reader
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
  ///   <p>Decodes Code 93 barcodes.</p>
  /// 	<author>Sean Owen</author>
  /// <see cref="T:ZXing.OneD.Code39Reader" />
  /// </summary>
  public sealed class Code93Reader : OneDReader
  {
    private const string ALPHABET_STRING = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%abcd*";
    private static readonly char[] ALPHABET = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%abcd*".ToCharArray();
    /// <summary>
    /// These represent the encodings of characters, as patterns of wide and narrow bars.
    /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow.
    /// </summary>
    private static readonly int[] CHARACTER_ENCODINGS = new int[48]
    {
      276,
      328,
      324,
      322,
      296,
      292,
      290,
      336,
      274,
      266,
      424,
      420,
      418,
      404,
      402,
      394,
      360,
      356,
      354,
      308,
      282,
      344,
      332,
      326,
      300,
      278,
      436,
      434,
      428,
      422,
      406,
      410,
      364,
      358,
      310,
      314,
      302,
      468,
      466,
      458,
      366,
      374,
      430,
      294,
      474,
      470,
      306,
      350
    };
    private static readonly int ASTERISK_ENCODING = Code93Reader.CHARACTER_ENCODINGS[47];
    private readonly StringBuilder decodeRowResult;
    private readonly int[] counters;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.Code93Reader" /> class.
    /// </summary>
    public Code93Reader()
    {
      this.decodeRowResult = new StringBuilder(20);
      this.counters = new int[6];
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
      int[] asteriskPattern = this.findAsteriskPattern(row);
      if (asteriskPattern == null)
        return (Result) null;
      int nextSet = row.getNextSet(asteriskPattern[1]);
      int size = row.Size;
      while (OneDReader.recordPattern(row, nextSet, this.counters))
      {
        int pattern = Code93Reader.toPattern(this.counters);
        if (pattern < 0)
          return (Result) null;
        char c;
        if (!Code93Reader.patternToChar(pattern, out c))
          return (Result) null;
        this.decodeRowResult.Append(c);
        int num1 = nextSet;
        foreach (int counter in this.counters)
          nextSet += counter;
        nextSet = row.getNextSet(nextSet);
        if (c == '*')
        {
          this.decodeRowResult.Remove(this.decodeRowResult.Length - 1, 1);
          int num2 = 0;
          foreach (int counter in this.counters)
            num2 += counter;
          if (nextSet == size || !row[nextSet])
            return (Result) null;
          if (this.decodeRowResult.Length < 2)
            return (Result) null;
          if (!Code93Reader.checkChecksums(this.decodeRowResult))
            return (Result) null;
          this.decodeRowResult.Length -= 2;
          string text = Code93Reader.decodeExtended(this.decodeRowResult);
          if (text == null)
            return (Result) null;
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
          }, BarcodeFormat.CODE_93);
        }
      }
      return (Result) null;
    }

    private int[] findAsteriskPattern(BitArray row)
    {
      int size = row.Size;
      int nextSet = row.getNextSet(0);
      for (int index = 0; index < this.counters.Length; ++index)
        this.counters[index] = 0;
      int index1 = 0;
      int num = nextSet;
      bool flag = false;
      int length = this.counters.Length;
      for (int i = nextSet; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++this.counters[index1];
        }
        else
        {
          if (index1 == length - 1)
          {
            if (Code93Reader.toPattern(this.counters) == Code93Reader.ASTERISK_ENCODING)
              return new int[2]{ num, i };
            num += this.counters[0] + this.counters[1];
            Array.Copy((Array) this.counters, 2, (Array) this.counters, 0, length - 2);
            this.counters[length - 2] = 0;
            this.counters[length - 1] = 0;
            --index1;
          }
          else
            ++index1;
          this.counters[index1] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    private static int toPattern(int[] counters)
    {
      int length = counters.Length;
      int num1 = 0;
      foreach (int counter in counters)
        num1 += counter;
      int pattern = 0;
      for (int index1 = 0; index1 < length; ++index1)
      {
        int num2 = (counters[index1] << OneDReader.INTEGER_MATH_SHIFT) * 9 / num1;
        int num3 = num2 >> OneDReader.INTEGER_MATH_SHIFT;
        if ((num2 & (int) byte.MaxValue) > (int) sbyte.MaxValue)
          ++num3;
        if (num3 < 1 || num3 > 4)
          return -1;
        if ((index1 & 1) == 0)
        {
          for (int index2 = 0; index2 < num3; ++index2)
            pattern = pattern << 1 | 1;
        }
        else
          pattern <<= num3;
      }
      return pattern;
    }

    private static bool patternToChar(int pattern, out char c)
    {
      for (int index = 0; index < Code93Reader.CHARACTER_ENCODINGS.Length; ++index)
      {
        if (Code93Reader.CHARACTER_ENCODINGS[index] == pattern)
        {
          c = Code93Reader.ALPHABET[index];
          return true;
        }
      }
      c = '*';
      return false;
    }

    private static string decodeExtended(StringBuilder encoded)
    {
      int length = encoded.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        char ch1 = encoded[index];
        switch (ch1)
        {
          case 'a':
          case 'b':
          case 'c':
          case 'd':
            if (index >= length - 1)
              return (string) null;
            char ch2 = encoded[index + 1];
            char ch3 = char.MinValue;
            switch (ch1)
            {
              case 'a':
                if (ch2 < 'A' || ch2 > 'Z')
                  return (string) null;
                ch3 = (char) ((uint) ch2 - 64U);
                break;
              case 'b':
                if (ch2 >= 'A' && ch2 <= 'E')
                {
                  ch3 = (char) ((uint) ch2 - 38U);
                  break;
                }
                if (ch2 < 'F' || ch2 > 'W')
                  return (string) null;
                ch3 = (char) ((uint) ch2 - 11U);
                break;
              case 'c':
                if (ch2 >= 'A' && ch2 <= 'O')
                {
                  ch3 = (char) ((uint) ch2 - 32U);
                  break;
                }
                if (ch2 != 'Z')
                  return (string) null;
                ch3 = ':';
                break;
              case 'd':
                if (ch2 < 'A' || ch2 > 'Z')
                  return (string) null;
                ch3 = (char) ((uint) ch2 + 32U);
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

    private static bool checkChecksums(StringBuilder result)
    {
      int length = result.Length;
      return Code93Reader.checkOneChecksum(result, length - 2, 20) && Code93Reader.checkOneChecksum(result, length - 1, 15);
    }

    private static bool checkOneChecksum(StringBuilder result, int checkPosition, int weightMax)
    {
      int num1 = 1;
      int num2 = 0;
      for (int index = checkPosition - 1; index >= 0; --index)
      {
        num2 += num1 * "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%abcd*".IndexOf(result[index]);
        if (++num1 > weightMax)
          num1 = 1;
      }
      return (int) result[checkPosition] == (int) Code93Reader.ALPHABET[num2 % 47];
    }
  }
}
