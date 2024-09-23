// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.MSIReader
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
  /// <summary>Decodes MSI barcodes.</summary>
  public sealed class MSIReader : OneDReader
  {
    private const int START_ENCODING = 6;
    private const int END_ENCODING = 9;
    internal static string ALPHABET_STRING = "0123456789";
    private static readonly char[] ALPHABET = MSIReader.ALPHABET_STRING.ToCharArray();
    /// <summary>
    /// These represent the encodings of characters, as patterns of wide and narrow bars.
    /// The 9 least-significant bits of each int correspond to the pattern of wide and narrow,
    /// with 1s representing "wide" and 0s representing narrow.
    /// </summary>
    internal static int[] CHARACTER_ENCODINGS = new int[10]
    {
      2340,
      2342,
      2356,
      2358,
      2468,
      2470,
      2484,
      2486,
      3364,
      3366
    };
    private readonly bool usingCheckDigit;
    private readonly StringBuilder decodeRowResult;
    private readonly int[] counters;
    private int averageCounterWidth;
    private static readonly int[] doubleAndCrossSum = new int[10]
    {
      0,
      2,
      4,
      6,
      8,
      1,
      3,
      5,
      7,
      9
    };

    /// <summary>
    /// Creates a reader that assumes all encoded data is data, and does not treat the final
    /// character as a check digit.
    /// </summary>
    public MSIReader()
      : this(false)
    {
    }

    /// <summary>
    /// Creates a reader that can be configured to check the last character as a check digit,
    /// </summary>
    /// <param name="usingCheckDigit">if true, treat the last data character as a check digit, not
    /// data, and verify that the checksum passes.</param>
    public MSIReader(bool usingCheckDigit)
    {
      this.usingCheckDigit = usingCheckDigit;
      this.decodeRowResult = new StringBuilder(20);
      this.counters = new int[8];
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
      int[] startPattern = this.findStartPattern(row, this.counters);
      if (startPattern == null)
        return (Result) null;
      int nextSet = row.getNextSet(startPattern[1]);
      int num;
      while (OneDReader.recordPattern(row, nextSet, this.counters, 8))
      {
        char c;
        if (!MSIReader.patternToChar(this.toPattern(this.counters, 8), out c))
        {
          int[] endPattern = this.findEndPattern(row, nextSet, this.counters);
          if (endPattern == null)
            return (Result) null;
          num = nextSet;
          nextSet = endPattern[1];
          goto label_18;
        }
        else
        {
          this.decodeRowResult.Append(c);
          num = nextSet;
          foreach (int counter in this.counters)
            nextSet += counter;
          nextSet = row.getNextSet(nextSet);
          if (c == '*')
            goto label_18;
        }
      }
      int[] endPattern1 = this.findEndPattern(row, nextSet, this.counters);
      if (endPattern1 == null)
        return (Result) null;
      num = nextSet;
      nextSet = endPattern1[1];
label_18:
      if (this.decodeRowResult.Length < 3)
        return (Result) null;
      byte[] bytes = Encoding.UTF8.GetBytes(this.decodeRowResult.ToString());
      string text = this.decodeRowResult.ToString();
      if (this.usingCheckDigit)
      {
        string number = text.Substring(0, text.Length - 1);
        if ((int) (ushort) (MSIReader.CalculateChecksumLuhn(number) + 48) != (int) text[number.Length])
          return (Result) null;
      }
      float x1 = (float) (startPattern[1] + startPattern[0]) / 2f;
      float x2 = (float) (nextSet + num) / 2f;
      ResultPointCallback hint = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      if (hint != null)
      {
        hint(new ResultPoint(x1, (float) rowNumber));
        hint(new ResultPoint(x2, (float) rowNumber));
      }
      return new Result(text, bytes, new ResultPoint[2]
      {
        new ResultPoint(x1, (float) rowNumber),
        new ResultPoint(x2, (float) rowNumber)
      }, BarcodeFormat.MSI);
    }

    private int[] findStartPattern(BitArray row, int[] counters)
    {
      int size = row.Size;
      int nextSet = row.getNextSet(0);
      int index = 0;
      int end = nextSet;
      bool flag = false;
      counters[0] = 0;
      counters[1] = 0;
      for (int i = nextSet; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++counters[index];
        }
        else
        {
          if (index == 1)
          {
            float num = (float) counters[0] / (float) counters[1];
            if ((double) num >= 1.5 && (double) num <= 5.0)
            {
              this.calculateAverageCounterWidth(counters, 2);
              if (this.toPattern(counters, 2) == 6 && row.isRange(Math.Max(0, end - (i - end >> 1)), end, false))
                return new int[2]{ end, i };
            }
            end += counters[0] + counters[1];
            Array.Copy((Array) counters, 2, (Array) counters, 0, 0);
            counters[0] = 0;
            counters[1] = 0;
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

    private int[] findEndPattern(BitArray row, int rowOffset, int[] counters)
    {
      int size = row.Size;
      int index1 = 0;
      int num1 = rowOffset;
      bool flag = false;
      counters[0] = 0;
      counters[1] = 0;
      counters[2] = 0;
      for (int index2 = rowOffset; index2 < size; ++index2)
      {
        if (row[index2] ^ flag)
        {
          ++counters[index1];
        }
        else
        {
          if (index1 == 2)
          {
            float num2 = (float) counters[1] / (float) counters[0];
            if ((double) num2 >= 1.5 && (double) num2 <= 5.0 && this.toPattern(counters, 3) == 9)
            {
              int end = Math.Min(row.Size - 1, index2 + (index2 - num1 >> 1));
              if (row.isRange(index2, end, false))
                return new int[2]{ num1, index2 };
            }
            return (int[]) null;
          }
          ++index1;
          counters[index1] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    private void calculateAverageCounterWidth(int[] counters, int patternLength)
    {
      int num1 = int.MaxValue;
      int num2 = 0;
      for (int index = 0; index < patternLength; ++index)
      {
        int counter = counters[index];
        if (counter < num1)
          num1 = counter;
        if (counter > num2)
          num2 = counter;
      }
      this.averageCounterWidth = ((num2 << 8) + (num1 << 8)) / 2;
    }

    private int toPattern(int[] counters, int patternLength)
    {
      int pattern = 0;
      int num1 = 1;
      int num2 = 3;
      for (int index = 0; index < patternLength; ++index)
      {
        pattern = counters[index] << 8 >= this.averageCounterWidth ? pattern << 2 | num2 : pattern << 1 | num1;
        num1 ^= 1;
        num2 ^= 3;
      }
      return pattern;
    }

    private static bool patternToChar(int pattern, out char c)
    {
      for (int index = 0; index < MSIReader.CHARACTER_ENCODINGS.Length; ++index)
      {
        if (MSIReader.CHARACTER_ENCODINGS[index] == pattern)
        {
          c = MSIReader.ALPHABET[index];
          return true;
        }
      }
      c = '*';
      return false;
    }

    private static int CalculateChecksumLuhn(string number)
    {
      int num1 = 0;
      for (int index = number.Length - 2; index >= 0; index -= 2)
      {
        int num2 = (int) number[index] - 48;
        num1 += num2;
      }
      for (int index = number.Length - 1; index >= 0; index -= 2)
      {
        int num3 = MSIReader.doubleAndCrossSum[(int) number[index] - 48];
        num1 += num3;
      }
      return (10 - num1 % 10) % 10;
    }
  }
}
