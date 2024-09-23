// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.CodaBarReader
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
  /// <p>Decodes Codabar barcodes.</p>
  /// 
  /// <author>Bas Vijfwinkel</author>
  /// </summary>
  public sealed class CodaBarReader : OneDReader
  {
    private const string ALPHABET_STRING = "0123456789-$:/.+ABCD";
    private const int MIN_CHARACTER_LENGTH = 3;
    private static readonly int MAX_ACCEPTABLE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 2.0);
    private static readonly int PADDING = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 1.5);
    internal static readonly char[] ALPHABET = "0123456789-$:/.+ABCD".ToCharArray();
    /// These represent the encodings of characters, as patterns of wide and narrow bars. The 7 least-significant bits of
    ///             each int correspond to the pattern of wide and narrow, with 1s representing "wide" and 0s representing narrow.
    internal static int[] CHARACTER_ENCODINGS = new int[20]
    {
      3,
      6,
      9,
      96,
      18,
      66,
      33,
      36,
      48,
      72,
      12,
      24,
      69,
      81,
      84,
      21,
      26,
      41,
      11,
      14
    };
    private static readonly char[] STARTEND_ENCODING = new char[4]
    {
      'A',
      'B',
      'C',
      'D'
    };
    private readonly StringBuilder decodeRowResult;
    private int[] counters;
    private int counterLength;

    public CodaBarReader()
    {
      this.decodeRowResult = new StringBuilder(20);
      this.counters = new int[80];
      this.counterLength = 0;
    }

    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      for (int index = 0; index < this.counters.Length; ++index)
        this.counters[index] = 0;
      if (!this.setCounters(row))
        return (Result) null;
      int startPattern = this.findStartPattern();
      if (startPattern < 0)
        return (Result) null;
      int position = startPattern;
      this.decodeRowResult.Length = 0;
      int narrowWidePattern;
      do
      {
        narrowWidePattern = this.toNarrowWidePattern(position);
        if (narrowWidePattern == -1)
          return (Result) null;
        this.decodeRowResult.Append((char) narrowWidePattern);
        position += 8;
      }
      while ((this.decodeRowResult.Length <= 1 || !CodaBarReader.arrayContains(CodaBarReader.STARTEND_ENCODING, CodaBarReader.ALPHABET[narrowWidePattern])) && position < this.counterLength);
      int counter = this.counters[position - 1];
      int num1 = 0;
      for (int index = -8; index < -1; ++index)
        num1 += this.counters[position + index];
      if (position < this.counterLength && counter < num1 / 2)
        return (Result) null;
      if (!this.validatePattern(startPattern))
        return (Result) null;
      for (int index = 0; index < this.decodeRowResult.Length; ++index)
        this.decodeRowResult[index] = CodaBarReader.ALPHABET[(int) this.decodeRowResult[index]];
      char key1 = this.decodeRowResult[0];
      if (!CodaBarReader.arrayContains(CodaBarReader.STARTEND_ENCODING, key1))
        return (Result) null;
      char key2 = this.decodeRowResult[this.decodeRowResult.Length - 1];
      if (!CodaBarReader.arrayContains(CodaBarReader.STARTEND_ENCODING, key2))
        return (Result) null;
      if (this.decodeRowResult.Length <= 3)
        return (Result) null;
      if (!SupportClass.GetValue<bool>(hints, DecodeHintType.RETURN_CODABAR_START_END, false))
      {
        this.decodeRowResult.Remove(this.decodeRowResult.Length - 1, 1);
        this.decodeRowResult.Remove(0, 1);
      }
      int num2 = 0;
      for (int index = 0; index < startPattern; ++index)
        num2 += this.counters[index];
      float x1 = (float) num2;
      for (int index = startPattern; index < position - 1; ++index)
        num2 += this.counters[index];
      float x2 = (float) num2;
      ResultPointCallback resultPointCallback = SupportClass.GetValue<ResultPointCallback>(hints, DecodeHintType.NEED_RESULT_POINT_CALLBACK, (ResultPointCallback) null);
      if (resultPointCallback != null)
      {
        resultPointCallback(new ResultPoint(x1, (float) rowNumber));
        resultPointCallback(new ResultPoint(x2, (float) rowNumber));
      }
      return new Result(this.decodeRowResult.ToString(), (byte[]) null, new ResultPoint[2]
      {
        new ResultPoint(x1, (float) rowNumber),
        new ResultPoint(x2, (float) rowNumber)
      }, BarcodeFormat.CODABAR);
    }

    private bool validatePattern(int start)
    {
      int[] numArray1 = new int[4];
      int[] numArray2 = new int[4];
      int num1 = this.decodeRowResult.Length - 1;
      int num2 = start;
      int index1 = 0;
      while (true)
      {
        int num3 = CodaBarReader.CHARACTER_ENCODINGS[(int) this.decodeRowResult[index1]];
        for (int index2 = 6; index2 >= 0; --index2)
        {
          int index3 = (index2 & 1) + (num3 & 1) * 2;
          numArray1[index3] += this.counters[num2 + index2];
          ++numArray2[index3];
          num3 >>= 1;
        }
        if (index1 < num1)
        {
          num2 += 8;
          ++index1;
        }
        else
          break;
      }
      int[] numArray3 = new int[4];
      int[] numArray4 = new int[4];
      for (int index4 = 0; index4 < 2; ++index4)
      {
        numArray4[index4] = 0;
        numArray4[index4 + 2] = (numArray1[index4] << OneDReader.INTEGER_MATH_SHIFT) / numArray2[index4] + (numArray1[index4 + 2] << OneDReader.INTEGER_MATH_SHIFT) / numArray2[index4 + 2] >> 1;
        numArray3[index4] = numArray4[index4 + 2];
        numArray3[index4 + 2] = (numArray1[index4 + 2] * CodaBarReader.MAX_ACCEPTABLE + CodaBarReader.PADDING) / numArray2[index4 + 2];
      }
      int num4 = start;
      int index5 = 0;
      while (true)
      {
        int num5 = CodaBarReader.CHARACTER_ENCODINGS[(int) this.decodeRowResult[index5]];
        for (int index6 = 6; index6 >= 0; --index6)
        {
          int index7 = (index6 & 1) + (num5 & 1) * 2;
          int num6 = this.counters[num4 + index6] << OneDReader.INTEGER_MATH_SHIFT;
          if (num6 < numArray4[index7] || num6 > numArray3[index7])
            return false;
          num5 >>= 1;
        }
        if (index5 < num1)
        {
          num4 += 8;
          ++index5;
        }
        else
          break;
      }
      return true;
    }

    /// <summary>
    /// Records the size of all runs of white and black pixels, starting with white.
    /// This is just like recordPattern, except it records all the counters, and
    /// uses our builtin "counters" member for storage.
    /// </summary>
    /// <param name="row">row to count from</param>
    private bool setCounters(BitArray row)
    {
      this.counterLength = 0;
      int nextUnset = row.getNextUnset(0);
      int size = row.Size;
      if (nextUnset >= size)
        return false;
      bool flag = true;
      int e = 0;
      for (; nextUnset < size; ++nextUnset)
      {
        if (row[nextUnset] ^ flag)
        {
          ++e;
        }
        else
        {
          this.counterAppend(e);
          e = 1;
          flag = !flag;
        }
      }
      this.counterAppend(e);
      return true;
    }

    private void counterAppend(int e)
    {
      this.counters[this.counterLength] = e;
      ++this.counterLength;
      if (this.counterLength < this.counters.Length)
        return;
      int[] destinationArray = new int[this.counterLength * 2];
      Array.Copy((Array) this.counters, 0, (Array) destinationArray, 0, this.counterLength);
      this.counters = destinationArray;
    }

    private int findStartPattern()
    {
      for (int position = 1; position < this.counterLength; position += 2)
      {
        int narrowWidePattern = this.toNarrowWidePattern(position);
        if (narrowWidePattern != -1 && CodaBarReader.arrayContains(CodaBarReader.STARTEND_ENCODING, CodaBarReader.ALPHABET[narrowWidePattern]))
        {
          int num = 0;
          for (int index = position; index < position + 7; ++index)
            num += this.counters[index];
          if (position == 1 || this.counters[position - 1] >= num / 2)
            return position;
        }
      }
      return -1;
    }

    internal static bool arrayContains(char[] array, char key)
    {
      if (array != null)
      {
        foreach (int num in array)
        {
          if (num == (int) key)
            return true;
        }
      }
      return false;
    }

    private int toNarrowWidePattern(int position)
    {
      int num1 = position + 7;
      if (num1 >= this.counterLength)
        return -1;
      int[] counters = this.counters;
      int num2 = 0;
      int num3 = int.MaxValue;
      for (int index = position; index < num1; index += 2)
      {
        int num4 = counters[index];
        if (num4 < num3)
          num3 = num4;
        if (num4 > num2)
          num2 = num4;
      }
      int num5 = (num3 + num2) / 2;
      int num6 = 0;
      int num7 = int.MaxValue;
      for (int index = position + 1; index < num1; index += 2)
      {
        int num8 = counters[index];
        if (num8 < num7)
          num7 = num8;
        if (num8 > num6)
          num6 = num8;
      }
      int num9 = (num7 + num6) / 2;
      int num10 = 128;
      int num11 = 0;
      for (int index = 0; index < 7; ++index)
      {
        int num12 = (index & 1) == 0 ? num5 : num9;
        num10 >>= 1;
        if (counters[position + index] > num12)
          num11 |= num10;
      }
      for (int narrowWidePattern = 0; narrowWidePattern < CodaBarReader.CHARACTER_ENCODINGS.Length; ++narrowWidePattern)
      {
        if (CodaBarReader.CHARACTER_ENCODINGS[narrowWidePattern] == num11)
          return narrowWidePattern;
      }
      return -1;
    }
  }
}
