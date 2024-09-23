// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEANReader
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
  ///   <p>Encapsulates functionality and implementation that is common to UPC and EAN families
  /// of one-dimensional barcodes.</p>
  ///   <author>dswitkin@google.com (Daniel Switkin)</author>
  ///   <author>Sean Owen</author>
  ///   <author>alasdair@google.com (Alasdair Mackintosh)</author>
  /// </summary>
  public abstract class UPCEANReader : OneDReader
  {
    private static readonly int MAX_AVG_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.47999998927116394);
    private static readonly int MAX_INDIVIDUAL_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.699999988079071);
    /// <summary>Start/end guard pattern.</summary>
    internal static int[] START_END_PATTERN = new int[3]
    {
      1,
      1,
      1
    };
    /// <summary>
    /// Pattern marking the middle of a UPC/EAN pattern, separating the two halves.
    /// </summary>
    internal static int[] MIDDLE_PATTERN = new int[5]
    {
      1,
      1,
      1,
      1,
      1
    };
    /// <summary>"Odd", or "L" patterns used to encode UPC/EAN digits.</summary>
    internal static int[][] L_PATTERNS = new int[10][]
    {
      new int[4]{ 3, 2, 1, 1 },
      new int[4]{ 2, 2, 2, 1 },
      new int[4]{ 2, 1, 2, 2 },
      new int[4]{ 1, 4, 1, 1 },
      new int[4]{ 1, 1, 3, 2 },
      new int[4]{ 1, 2, 3, 1 },
      new int[4]{ 1, 1, 1, 4 },
      new int[4]{ 1, 3, 1, 2 },
      new int[4]{ 1, 2, 1, 3 },
      new int[4]{ 3, 1, 1, 2 }
    };
    /// <summary>
    /// As above but also including the "even", or "G" patterns used to encode UPC/EAN digits.
    /// </summary>
    internal static int[][] L_AND_G_PATTERNS = new int[20][];
    private readonly StringBuilder decodeRowStringBuffer;
    private readonly UPCEANExtensionSupport extensionReader;
    private readonly EANManufacturerOrgSupport eanManSupport;

    static UPCEANReader()
    {
      Array.Copy((Array) UPCEANReader.L_PATTERNS, 0, (Array) UPCEANReader.L_AND_G_PATTERNS, 0, 10);
      for (int index1 = 10; index1 < 20; ++index1)
      {
        int[] numArray1 = UPCEANReader.L_PATTERNS[index1 - 10];
        int[] numArray2 = new int[numArray1.Length];
        for (int index2 = 0; index2 < numArray1.Length; ++index2)
          numArray2[index2] = numArray1[numArray1.Length - index2 - 1];
        UPCEANReader.L_AND_G_PATTERNS[index1] = numArray2;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.UPCEANReader" /> class.
    /// </summary>
    protected UPCEANReader()
    {
      this.decodeRowStringBuffer = new StringBuilder(20);
      this.extensionReader = new UPCEANExtensionSupport();
      this.eanManSupport = new EANManufacturerOrgSupport();
    }

    internal static int[] findStartGuardPattern(BitArray row)
    {
      bool flag = false;
      int[] startGuardPattern = (int[]) null;
      int rowOffset = 0;
      int[] counters = new int[UPCEANReader.START_END_PATTERN.Length];
      while (!flag)
      {
        for (int index = 0; index < UPCEANReader.START_END_PATTERN.Length; ++index)
          counters[index] = 0;
        startGuardPattern = UPCEANReader.findGuardPattern(row, rowOffset, false, UPCEANReader.START_END_PATTERN, counters);
        if (startGuardPattern == null)
          return (int[]) null;
        int end = startGuardPattern[0];
        rowOffset = startGuardPattern[1];
        int start = end - (rowOffset - end);
        if (start >= 0)
          flag = row.isRange(start, end, false);
      }
      return startGuardPattern;
    }

    /// <summary>
    ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
    /// an image.</p>
    /// </summary>
    /// <param name="rowNumber">row number from top of the row</param>
    /// <param name="row">the black/white pixel data of the row</param>
    /// <param name="hints">decode hints</param>
    /// <returns>
    ///   <see cref="T:ZXing.Result" />containing encoded string and start/end of barcode or null, if an error occurs or barcode cannot be found
    /// </returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      return this.decodeRow(rowNumber, row, UPCEANReader.findStartGuardPattern(row), hints);
    }

    /// <summary>
    /// <p>Like decodeRow(int, BitArray, java.util.Map), but
    /// allows caller to inform method about where the UPC/EAN start pattern is
    /// found. This allows this to be computed once and reused across many implementations.</p>
    /// </summary>
    public virtual Result decodeRow(
      int rowNumber,
      BitArray row,
      int[] startGuardRange,
      IDictionary<DecodeHintType, object> hints)
    {
      ResultPointCallback hint1 = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      if (hint1 != null)
        hint1(new ResultPoint((float) (startGuardRange[0] + startGuardRange[1]) / 2f, (float) rowNumber));
      StringBuilder decodeRowStringBuffer = this.decodeRowStringBuffer;
      decodeRowStringBuffer.Length = 0;
      int num1 = this.decodeMiddle(row, startGuardRange, decodeRowStringBuffer);
      if (num1 < 0)
        return (Result) null;
      if (hint1 != null)
        hint1(new ResultPoint((float) num1, (float) rowNumber));
      int[] numArray = this.decodeEnd(row, num1);
      if (numArray == null)
        return (Result) null;
      if (hint1 != null)
        hint1(new ResultPoint((float) (numArray[0] + numArray[1]) / 2f, (float) rowNumber));
      int start = numArray[1];
      int end = start + (start - numArray[0]);
      if (end >= row.Size || !row.isRange(start, end, false))
        return (Result) null;
      string str1 = decodeRowStringBuffer.ToString();
      if (str1.Length < 8)
        return (Result) null;
      if (!this.checkChecksum(str1))
        return (Result) null;
      float x1 = (float) (startGuardRange[1] + startGuardRange[0]) / 2f;
      float x2 = (float) (numArray[1] + numArray[0]) / 2f;
      BarcodeFormat barcodeFormat = this.BarcodeFormat;
      Result result1 = new Result(str1, (byte[]) null, new ResultPoint[2]
      {
        new ResultPoint(x1, (float) rowNumber),
        new ResultPoint(x2, (float) rowNumber)
      }, barcodeFormat);
      Result result2 = this.extensionReader.decodeRow(rowNumber, row, numArray[1]);
      if (result2 != null)
      {
        result1.putMetadata(ResultMetadataType.UPC_EAN_EXTENSION, (object) result2.Text);
        result1.putAllMetadata(result2.ResultMetadata);
        result1.addResultPoints(result2.ResultPoints);
        int length = result2.Text.Length;
        int[] hint2 = hints == null || !hints.ContainsKey(DecodeHintType.ALLOWED_EAN_EXTENSIONS) ? (int[]) null : (int[]) hints[DecodeHintType.ALLOWED_EAN_EXTENSIONS];
        if (hint2 != null)
        {
          bool flag = false;
          foreach (int num2 in hint2)
          {
            if (length == num2)
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            return (Result) null;
        }
      }
      if (barcodeFormat == BarcodeFormat.EAN_13 || barcodeFormat == BarcodeFormat.UPC_A)
      {
        string str2 = this.eanManSupport.lookupCountryIdentifier(str1);
        if (str2 != null)
          result1.putMetadata(ResultMetadataType.POSSIBLE_COUNTRY, (object) str2);
      }
      return result1;
    }

    /// <summary>
    /// <returns>see checkStandardUPCEANChecksum(String)</returns>
    /// </summary>
    protected virtual bool checkChecksum(string s) => UPCEANReader.checkStandardUPCEANChecksum(s);

    /// <summary>
    /// Computes the UPC/EAN checksum on a string of digits, and reports
    /// whether the checksum is correct or not.
    /// </summary>
    /// <param name="s">string of digits to check</param>
    /// <returns>true iff string of digits passes the UPC/EAN checksum algorithm</returns>
    internal static bool checkStandardUPCEANChecksum(string s)
    {
      int length = s.Length;
      if (length == 0)
        return false;
      int num1 = 0;
      for (int index = length - 2; index >= 0; index -= 2)
      {
        int num2 = (int) s[index] - 48;
        if (num2 < 0 || num2 > 9)
          return false;
        num1 += num2;
      }
      int num3 = num1 * 3;
      for (int index = length - 1; index >= 0; index -= 2)
      {
        int num4 = (int) s[index] - 48;
        if (num4 < 0 || num4 > 9)
          return false;
        num3 += num4;
      }
      return num3 % 10 == 0;
    }

    /// <summary>Decodes the end.</summary>
    /// <param name="row">The row.</param>
    /// <param name="endStart">The end start.</param>
    /// <returns></returns>
    protected virtual int[] decodeEnd(BitArray row, int endStart)
    {
      return UPCEANReader.findGuardPattern(row, endStart, false, UPCEANReader.START_END_PATTERN);
    }

    internal static int[] findGuardPattern(
      BitArray row,
      int rowOffset,
      bool whiteFirst,
      int[] pattern)
    {
      return UPCEANReader.findGuardPattern(row, rowOffset, whiteFirst, pattern, new int[pattern.Length]);
    }

    /// <summary>
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="rowOffset">position to start search</param>
    /// <param name="whiteFirst">if true, indicates that the pattern specifies white/black/white/...</param>
    /// 
    ///             pixel counts, otherwise, it is interpreted as black/white/black/...
    ///             <param name="pattern">pattern of counts of number of black and white pixels that are being</param>
    /// 
    ///             searched for as a pattern
    ///             <param name="counters">array of counters, as long as pattern, to re-use</param>
    /// <returns>start/end horizontal offset of guard pattern, as an array of two ints</returns>
    internal static int[] findGuardPattern(
      BitArray row,
      int rowOffset,
      bool whiteFirst,
      int[] pattern,
      int[] counters)
    {
      int length = pattern.Length;
      int size = row.Size;
      bool flag = whiteFirst;
      rowOffset = whiteFirst ? row.getNextUnset(rowOffset) : row.getNextSet(rowOffset);
      int index = 0;
      int num = rowOffset;
      for (int i = rowOffset; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++counters[index];
        }
        else
        {
          if (index == length - 1)
          {
            if (OneDReader.patternMatchVariance(counters, pattern, UPCEANReader.MAX_INDIVIDUAL_VARIANCE) < UPCEANReader.MAX_AVG_VARIANCE)
              return new int[2]{ num, i };
            num += counters[0] + counters[1];
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

    /// <summary>Attempts to decode a single UPC/EAN-encoded digit.</summary>
    /// <param name="row">row of black/white values to decode</param>
    /// <param name="counters">the counts of runs of observed black/white/black/... values</param>
    /// <param name="rowOffset">horizontal offset to start decoding from</param>
    /// <param name="patterns">the set of patterns to use to decode -- sometimes different encodings</param>
    /// 
    ///             for the digits 0-9 are used, and this indicates the encodings for 0 to 9 that should
    ///             be used
    ///             <returns>horizontal offset of first pixel beyond the decoded digit</returns>
    internal static bool decodeDigit(
      BitArray row,
      int[] counters,
      int rowOffset,
      int[][] patterns,
      out int digit)
    {
      digit = -1;
      if (!OneDReader.recordPattern(row, rowOffset, counters))
        return false;
      int num1 = UPCEANReader.MAX_AVG_VARIANCE;
      int length = patterns.Length;
      for (int index = 0; index < length; ++index)
      {
        int[] pattern = patterns[index];
        int num2 = OneDReader.patternMatchVariance(counters, pattern, UPCEANReader.MAX_INDIVIDUAL_VARIANCE);
        if (num2 < num1)
        {
          num1 = num2;
          digit = index;
        }
      }
      return digit >= 0;
    }

    /// <summary>
    /// Get the format of this decoder.
    /// <returns>The 1D format.</returns>
    /// </summary>
    internal abstract BarcodeFormat BarcodeFormat { get; }

    /// <summary>
    /// Subclasses override this to decode the portion of a barcode between the start
    /// and end guard patterns.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="startRange">start/end offset of start guard pattern</param>
    /// <param name="resultString"><see cref="T:System.Text.StringBuilder" />to append decoded chars to</param>
    /// <returns>horizontal offset of first pixel after the "middle" that was decoded or -1 if decoding could not complete successfully</returns>
    protected internal abstract int decodeMiddle(
      BitArray row,
      int[] startRange,
      StringBuilder resultString);
  }
}
