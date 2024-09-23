// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.ITFReader
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
  /// <p>Implements decoding of the ITF format, or Interleaved Two of Five.</p>
  /// 
  /// <p>This Reader will scan ITF barcodes of certain lengths only.
  /// At the moment it reads length 6, 8, 10, 12, 14, 16, 18, 20, 24, 44 and 48 as these have appeared "in the wild". Not all
  /// lengths are scanned, especially shorter ones, to avoid false positives. This in turn is due to a lack of
  /// required checksum function.</p>
  /// 
  /// <p>The checksum is optional and is not applied by this Reader. The consumer of the decoded
  /// value will have to apply a checksum if required.</p>
  /// 
  /// <p><a href="http://en.wikipedia.org/wiki/Interleaved_2_of_5">http://en.wikipedia.org/wiki/Interleaved_2_of_5</a>
  /// is a great reference for Interleaved 2 of 5 information.</p>
  /// 
  /// <author>kevin.osullivan@sita.aero, SITA Lab.</author>
  /// </summary>
  public sealed class ITFReader : OneDReader
  {
    private const int W = 3;
    private const int N = 1;
    private const int LARGEST_DEFAULT_ALLOWED_LENGTH = 14;
    private static readonly int MAX_AVG_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.41999998688697815);
    private static readonly int MAX_INDIVIDUAL_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.77999997138977051);
    /// <summary>
    /// Valid ITF lengths. Anything longer than the largest value is also allowed.
    /// </summary>
    private static readonly int[] DEFAULT_ALLOWED_LENGTHS = new int[5]
    {
      6,
      8,
      10,
      12,
      14
    };
    private int narrowLineWidth = -1;
    /// <summary>
    /// Start/end guard pattern.
    /// 
    /// Note: The end pattern is reversed because the row is reversed before
    /// searching for the END_PATTERN
    /// </summary>
    private static readonly int[] START_PATTERN = new int[4]
    {
      1,
      1,
      1,
      1
    };
    private static readonly int[] END_PATTERN_REVERSED = new int[3]
    {
      1,
      1,
      3
    };
    /// <summary>
    /// Patterns of Wide / Narrow lines to indicate each digit
    /// </summary>
    internal static int[][] PATTERNS = new int[10][]
    {
      new int[5]{ 1, 1, 3, 3, 1 },
      new int[5]{ 3, 1, 1, 1, 3 },
      new int[5]{ 1, 3, 1, 1, 3 },
      new int[5]{ 3, 3, 1, 1, 1 },
      new int[5]{ 1, 1, 3, 1, 3 },
      new int[5]{ 3, 1, 3, 1, 1 },
      new int[5]{ 1, 3, 3, 1, 1 },
      new int[5]{ 1, 1, 1, 3, 3 },
      new int[5]{ 3, 1, 1, 3, 1 },
      new int[5]{ 1, 3, 1, 3, 1 }
    };

    /// <summary>
    /// Attempts to decode a one-dimensional barcode format given a single row of
    /// an image.
    /// </summary>
    /// <param name="rowNumber">row number from top of the row</param>
    /// <param name="row">the black/white pixel data of the row</param>
    /// <param name="hints">decode hints</param>
    /// <returns>
    ///   <see cref="T:ZXing.Result" />containing encoded string and start/end of barcode
    /// </returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      int[] numArray1 = this.decodeStart(row);
      if (numArray1 == null)
        return (Result) null;
      int[] numArray2 = this.decodeEnd(row);
      if (numArray2 == null)
        return (Result) null;
      StringBuilder resultString = new StringBuilder(20);
      if (!ITFReader.decodeMiddle(row, numArray1[1], numArray2[0], resultString))
        return (Result) null;
      string text = resultString.ToString();
      int[] numArray3 = (int[]) null;
      int num1 = 14;
      if (hints != null && hints.ContainsKey(DecodeHintType.ALLOWED_LENGTHS))
      {
        numArray3 = (int[]) hints[DecodeHintType.ALLOWED_LENGTHS];
        num1 = 0;
      }
      if (numArray3 == null)
      {
        numArray3 = ITFReader.DEFAULT_ALLOWED_LENGTHS;
        num1 = 14;
      }
      int length = text.Length;
      bool flag = length > 14;
      if (!flag)
      {
        foreach (int num2 in numArray3)
        {
          if (length == num2)
          {
            flag = true;
            break;
          }
          if (num2 > num1)
            num1 = num2;
        }
        if (!flag && length > num1)
          flag = true;
        if (!flag)
          return (Result) null;
      }
      ResultPointCallback hint = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      if (hint != null)
      {
        hint(new ResultPoint((float) numArray1[1], (float) rowNumber));
        hint(new ResultPoint((float) numArray2[0], (float) rowNumber));
      }
      return new Result(text, (byte[]) null, new ResultPoint[2]
      {
        new ResultPoint((float) numArray1[1], (float) rowNumber),
        new ResultPoint((float) numArray2[0], (float) rowNumber)
      }, BarcodeFormat.ITF);
    }

    /// <summary>
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="payloadStart">offset of start pattern</param>
    /// <param name="payloadEnd">The payload end.</param>
    /// <param name="resultString"><see cref="T:System.Text.StringBuilder" />to append decoded chars to</param>
    /// <returns>false, if decoding could not complete successfully</returns>
    private static bool decodeMiddle(
      BitArray row,
      int payloadStart,
      int payloadEnd,
      StringBuilder resultString)
    {
      int[] counters1 = new int[10];
      int[] counters2 = new int[5];
      int[] counters3 = new int[5];
      while (payloadStart < payloadEnd)
      {
        if (!OneDReader.recordPattern(row, payloadStart, counters1))
          return false;
        for (int index1 = 0; index1 < 5; ++index1)
        {
          int index2 = index1 << 1;
          counters2[index1] = counters1[index2];
          counters3[index1] = counters1[index2 + 1];
        }
        int bestMatch;
        if (!ITFReader.decodeDigit(counters2, out bestMatch))
          return false;
        resultString.Append((char) (48 + bestMatch));
        if (!ITFReader.decodeDigit(counters3, out bestMatch))
          return false;
        resultString.Append((char) (48 + bestMatch));
        foreach (int num in counters1)
          payloadStart += num;
      }
      return true;
    }

    /// <summary>
    /// Identify where the start of the middle / payload section starts.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <returns>Array, containing index of start of 'start block' and end of 'start block'</returns>
    private int[] decodeStart(BitArray row)
    {
      int rowOffset = ITFReader.skipWhiteSpace(row);
      if (rowOffset < 0)
        return (int[]) null;
      int[] guardPattern = ITFReader.findGuardPattern(row, rowOffset, ITFReader.START_PATTERN);
      if (guardPattern == null)
        return (int[]) null;
      this.narrowLineWidth = guardPattern[1] - guardPattern[0] >> 2;
      return !this.validateQuietZone(row, guardPattern[0]) ? (int[]) null : guardPattern;
    }

    /// <summary>
    /// The start &amp; end patterns must be pre/post fixed by a quiet zone. This
    /// zone must be at least 10 times the width of a narrow line.  Scan back until
    /// we either get to the start of the barcode or match the necessary number of
    /// quiet zone pixels.
    /// 
    /// Note: Its assumed the row is reversed when using this method to find
    /// quiet zone after the end pattern.
    /// 
    /// ref: http://www.barcode-1.net/i25code.html
    /// </summary>
    /// <param name="row">bit array representing the scanned barcode.</param>
    /// <param name="startPattern">index into row of the start or end pattern.</param>
    /// <returns>false, if the quiet zone cannot be found</returns>
    private bool validateQuietZone(BitArray row, int startPattern)
    {
      int num1 = this.narrowLineWidth * 10;
      int num2 = num1 < startPattern ? num1 : startPattern;
      for (int i = startPattern - 1; num2 > 0 && i >= 0 && !row[i]; --i)
        --num2;
      return num2 == 0;
    }

    /// <summary>
    /// Skip all whitespace until we get to the first black line.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <returns>index of the first black line or -1 if no black lines are found in the row.</returns>
    private static int skipWhiteSpace(BitArray row)
    {
      int size = row.Size;
      int nextSet = row.getNextSet(0);
      return nextSet == size ? -1 : nextSet;
    }

    /// <summary>
    /// Identify where the end of the middle / payload section ends.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <returns>Array, containing index of start of 'end block' and end of 'end
    /// block' or null, if nothing found</returns>
    private int[] decodeEnd(BitArray row)
    {
      row.reverse();
      int rowOffset = ITFReader.skipWhiteSpace(row);
      if (rowOffset < 0)
        return (int[]) null;
      int[] guardPattern = ITFReader.findGuardPattern(row, rowOffset, ITFReader.END_PATTERN_REVERSED);
      if (guardPattern == null)
      {
        row.reverse();
        return (int[]) null;
      }
      if (!this.validateQuietZone(row, guardPattern[0]))
      {
        row.reverse();
        return (int[]) null;
      }
      int num = guardPattern[0];
      guardPattern[0] = row.Size - guardPattern[1];
      guardPattern[1] = row.Size - num;
      row.reverse();
      return guardPattern;
    }

    /// <summary>
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="rowOffset">position to start search</param>
    /// <param name="pattern">pattern of counts of number of black and white pixels that are being searched for as a pattern</param>
    /// <returns>start/end horizontal offset of guard pattern, as an array of two ints</returns>
    private static int[] findGuardPattern(BitArray row, int rowOffset, int[] pattern)
    {
      int length = pattern.Length;
      int[] numArray = new int[length];
      int size = row.Size;
      bool flag = false;
      int index = 0;
      int num = rowOffset;
      for (int i = rowOffset; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++numArray[index];
        }
        else
        {
          if (index == length - 1)
          {
            if (OneDReader.patternMatchVariance(numArray, pattern, ITFReader.MAX_INDIVIDUAL_VARIANCE) < ITFReader.MAX_AVG_VARIANCE)
              return new int[2]{ num, i };
            num += numArray[0] + numArray[1];
            Array.Copy((Array) numArray, 2, (Array) numArray, 0, length - 2);
            numArray[length - 2] = 0;
            numArray[length - 1] = 0;
            --index;
          }
          else
            ++index;
          numArray[index] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    /// <summary>
    /// Attempts to decode a sequence of ITF black/white lines into single
    /// digit.
    /// </summary>
    /// <param name="counters">the counts of runs of observed black/white/black/... values</param>
    /// <param name="bestMatch">The decoded digit</param>
    /// <returns>false, if digit cannot be decoded</returns>
    private static bool decodeDigit(int[] counters, out int bestMatch)
    {
      int num1 = ITFReader.MAX_AVG_VARIANCE;
      bestMatch = -1;
      int length = ITFReader.PATTERNS.Length;
      for (int index = 0; index < length; ++index)
      {
        int[] pattern = ITFReader.PATTERNS[index];
        int num2 = OneDReader.patternMatchVariance(counters, pattern, ITFReader.MAX_INDIVIDUAL_VARIANCE);
        if (num2 < num1)
        {
          num1 = num2;
          bestMatch = index;
        }
      }
      return bestMatch >= 0;
    }
  }
}
