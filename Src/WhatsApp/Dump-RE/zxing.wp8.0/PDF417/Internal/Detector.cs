// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.Detector
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// <p>Encapsulates logic that can detect a PDF417 Code in an image, even if the
  /// PDF417 Code is rotated or skewed, or partially obscured.</p>
  /// 
  /// <author>SITA Lab (kevin.osullivan@sita.aero)</author>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author> Guenther Grau</author>
  /// </summary>
  public sealed class Detector
  {
    private const int INTEGER_MATH_SHIFT = 8;
    private const int PATTERN_MATCH_RESULT_SCALE_FACTOR = 256;
    private const int MAX_AVG_VARIANCE = 107;
    private const int MAX_INDIVIDUAL_VARIANCE = 204;
    private const int MAX_PIXEL_DRIFT = 3;
    private const int MAX_PATTERN_DRIFT = 5;
    /// <summary>
    /// if we set the value too low, then we don't detect the correct height of the bar if the start patterns are damaged.
    /// if we set the value too high, then we might detect the start pattern from a neighbor barcode.
    /// </summary>
    private const int SKIPPED_ROW_COUNT_MAX = 25;
    /// <summary>
    /// A PDF471 barcode should have at least 3 rows, with each row being &gt;= 3 times the module width. Therefore it should be at least
    /// 9 pixels tall. To be conservative, we use about half the size to ensure we don't miss it.
    /// </summary>
    private const int ROW_STEP = 5;
    private const int BARCODE_MIN_HEIGHT = 10;
    private static readonly int[] INDEXES_START_PATTERN = new int[4]
    {
      0,
      4,
      1,
      5
    };
    private static readonly int[] INDEXES_STOP_PATTERN = new int[4]
    {
      6,
      2,
      7,
      3
    };
    /// <summary>
    /// B S B S B S B S Bar/Space pattern
    /// 11111111 0 1 0 1 0 1 000.
    /// </summary>
    private static readonly int[] START_PATTERN = new int[8]
    {
      8,
      1,
      1,
      1,
      1,
      1,
      1,
      3
    };
    /// <summary>1111111 0 1 000 1 0 1 00 1</summary>
    private static readonly int[] STOP_PATTERN = new int[9]
    {
      7,
      1,
      1,
      3,
      1,
      1,
      1,
      2,
      1
    };

    /// <summary>
    /// <p>Detects a PDF417 Code in an image. Only checks 0 and 180 degree rotations.</p>
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="hints">Hints.</param>
    /// <param name="multiple">If set to <c>true</c> multiple.</param>
    /// <returns><see cref="T:ZXing.PDF417.Internal.PDF417DetectorResult" /> encapsulating results of detecting a PDF417 code </returns>
    public static PDF417DetectorResult detect(
      BinaryBitmap image,
      IDictionary<DecodeHintType, object> hints,
      bool multiple)
    {
      BitMatrix bitMatrix = image.BlackMatrix;
      List<ResultPoint[]> points = Detector.detect(multiple, bitMatrix);
      if (points.Count == 0)
      {
        bitMatrix = (BitMatrix) bitMatrix.Clone();
        bitMatrix.rotate180();
        points = Detector.detect(multiple, bitMatrix);
      }
      return new PDF417DetectorResult(bitMatrix, points);
    }

    /// <summary>
    /// Detects PDF417 codes in an image. Only checks 0 degree rotation (so rotate the matrix and check again outside of this method)
    /// </summary>
    /// <param name="multiple">multiple if true, then the image is searched for multiple codes. If false, then at most one code will be found and returned.</param>
    /// <param name="bitMatrix">bit matrix to detect barcodes in.</param>
    /// <returns>List of ResultPoint arrays containing the coordinates of found barcodes</returns>
    private static List<ResultPoint[]> detect(bool multiple, BitMatrix bitMatrix)
    {
      List<ResultPoint[]> resultPointArrayList = new List<ResultPoint[]>();
      int num = 0;
      int startColumn = 0;
      bool flag = false;
      while (num < bitMatrix.Height)
      {
        ResultPoint[] vertices = Detector.findVertices(bitMatrix, num, startColumn);
        if (vertices[0] == null && vertices[3] == null)
        {
          if (flag)
          {
            flag = false;
            startColumn = 0;
            foreach (ResultPoint[] resultPointArray in resultPointArrayList)
            {
              if (resultPointArray[1] != null)
                num = (int) Math.Max((float) num, resultPointArray[1].Y);
              if (resultPointArray[3] != null)
                num = Math.Max(num, (int) resultPointArray[3].Y);
            }
            num += 5;
          }
          else
            break;
        }
        else
        {
          flag = true;
          resultPointArrayList.Add(vertices);
          if (multiple)
          {
            if (vertices[2] != null)
            {
              startColumn = (int) vertices[2].X;
              num = (int) vertices[2].Y;
            }
            else
            {
              startColumn = (int) vertices[4].X;
              num = (int) vertices[4].Y;
            }
          }
          else
            break;
        }
      }
      return resultPointArrayList;
    }

    /// <summary>
    /// Locate the vertices and the codewords area of a black blob using the Start and Stop patterns as locators.
    /// </summary>
    /// <param name="matrix">Matrix.</param>
    /// <param name="startRow">Start row.</param>
    /// <param name="startColumn">Start column.</param>
    /// <returns> an array containing the vertices:
    ///           vertices[0] x, y top left barcode
    ///           vertices[1] x, y bottom left barcode
    ///           vertices[2] x, y top right barcode
    ///           vertices[3] x, y bottom right barcode
    ///           vertices[4] x, y top left codeword area
    ///           vertices[5] x, y bottom left codeword area
    ///           vertices[6] x, y top right codeword area
    ///           vertices[7] x, y bottom right codeword area
    /// </returns>
    private static ResultPoint[] findVertices(BitMatrix matrix, int startRow, int startColumn)
    {
      int height = matrix.Height;
      int width = matrix.Width;
      ResultPoint[] result = new ResultPoint[8];
      Detector.copyToResult(result, Detector.findRowsWithPattern(matrix, height, width, startRow, startColumn, Detector.START_PATTERN), Detector.INDEXES_START_PATTERN);
      if (result[4] != null)
      {
        startColumn = (int) result[4].X;
        startRow = (int) result[4].Y;
      }
      Detector.copyToResult(result, Detector.findRowsWithPattern(matrix, height, width, startRow, startColumn, Detector.STOP_PATTERN), Detector.INDEXES_STOP_PATTERN);
      return result;
    }

    /// <summary>Copies the temp data to the final result</summary>
    /// <param name="result">Result.</param>
    /// <param name="tmpResult">Temp result.</param>
    /// <param name="destinationIndexes">Destination indexes.</param>
    private static void copyToResult(
      ResultPoint[] result,
      ResultPoint[] tmpResult,
      int[] destinationIndexes)
    {
      for (int index = 0; index < destinationIndexes.Length; ++index)
        result[destinationIndexes[index]] = tmpResult[index];
    }

    /// <summary>Finds the rows with the given pattern.</summary>
    /// <returns>The rows with pattern.</returns>
    /// <param name="matrix">Matrix.</param>
    /// <param name="height">Height.</param>
    /// <param name="width">Width.</param>
    /// <param name="startRow">Start row.</param>
    /// <param name="startColumn">Start column.</param>
    /// <param name="pattern">Pattern.</param>
    private static ResultPoint[] findRowsWithPattern(
      BitMatrix matrix,
      int height,
      int width,
      int startRow,
      int startColumn,
      int[] pattern)
    {
      ResultPoint[] rowsWithPattern = new ResultPoint[4];
      bool flag = false;
      int[] counters = new int[pattern.Length];
      for (; startRow < height; startRow += 5)
      {
        int[] numArray = Detector.findGuardPattern(matrix, startColumn, startRow, width, false, pattern, counters);
        if (numArray != null)
        {
          while (startRow > 0)
          {
            int[] guardPattern = Detector.findGuardPattern(matrix, startColumn, --startRow, width, false, pattern, counters);
            if (guardPattern != null)
            {
              numArray = guardPattern;
            }
            else
            {
              ++startRow;
              break;
            }
          }
          rowsWithPattern[0] = new ResultPoint((float) numArray[0], (float) startRow);
          rowsWithPattern[1] = new ResultPoint((float) numArray[1], (float) startRow);
          flag = true;
          break;
        }
      }
      int num1 = startRow + 1;
      if (flag)
      {
        int num2 = 0;
        int[] numArray = new int[2]
        {
          (int) rowsWithPattern[0].X,
          (int) rowsWithPattern[1].X
        };
        for (; num1 < height; ++num1)
        {
          int[] guardPattern = Detector.findGuardPattern(matrix, numArray[0], num1, width, false, pattern, counters);
          if (guardPattern != null && Math.Abs(numArray[0] - guardPattern[0]) < 5 && Math.Abs(numArray[1] - guardPattern[1]) < 5)
          {
            numArray = guardPattern;
            num2 = 0;
          }
          else if (num2 <= 25)
            ++num2;
          else
            break;
        }
        num1 -= num2 + 1;
        rowsWithPattern[2] = new ResultPoint((float) numArray[0], (float) num1);
        rowsWithPattern[3] = new ResultPoint((float) numArray[1], (float) num1);
      }
      if (num1 - startRow < 10)
      {
        for (int index = 0; index < rowsWithPattern.Length; ++index)
          rowsWithPattern[index] = (ResultPoint) null;
      }
      return rowsWithPattern;
    }

    /// <summary>
    /// Finds the guard pattern.  Uses System.Linq.Enumerable.Repeat to fill in counters.  This might be a performance issue?
    /// </summary>
    /// <returns>start/end horizontal offset of guard pattern, as an array of two ints.</returns>
    /// <param name="matrix">matrix row of black/white values to search</param>
    /// <param name="column">column x position to start search.</param>
    /// <param name="row">row y position to start search.</param>
    /// <param name="width">width the number of pixels to search on this row.</param>
    /// <param name="whiteFirst">If set to <c>true</c> search the white patterns first.</param>
    /// <param name="pattern">pattern of counts of number of black and white pixels that are being searched for as a pattern.</param>
    /// <param name="counters">counters array of counters, as long as pattern, to re-use .</param>
    private static int[] findGuardPattern(
      BitMatrix matrix,
      int column,
      int row,
      int width,
      bool whiteFirst,
      int[] pattern,
      int[] counters)
    {
      SupportClass.Fill<int>(counters, 0);
      int length = pattern.Length;
      bool flag = whiteFirst;
      int x1 = column;
      int num = 0;
      while (matrix[x1, row] && x1 > 0 && num++ < 3)
        --x1;
      int x2 = x1;
      int index = 0;
      for (; x2 < width; ++x2)
      {
        if (matrix[x2, row] ^ flag)
        {
          ++counters[index];
        }
        else
        {
          if (index == length - 1)
          {
            if (Detector.patternMatchVariance(counters, pattern, 204) < 107)
              return new int[2]{ x1, x2 };
            x1 += counters[0] + counters[1];
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
      if (index != length - 1 || Detector.patternMatchVariance(counters, pattern, 204) >= 107)
        return (int[]) null;
      return new int[2]{ x1, x2 - 1 };
    }

    /// <summary>
    /// Determines how closely a set of observed counts of runs of black/white.
    /// values matches a given target pattern. This is reported as the ratio of
    /// the total variance from the expected pattern proportions across all
    /// pattern elements, to the length of the pattern.
    /// </summary>
    /// <returns>
    /// ratio of total variance between counters and pattern compared to
    /// total pattern size, where the ratio has been multiplied by 256.
    /// So, 0 means no variance (perfect match); 256 means the total
    /// variance between counters and patterns equals the pattern length,
    /// higher values mean even more variance
    /// </returns>
    /// <param name="counters">observed counters.</param>
    /// <param name="pattern">expected pattern.</param>
    /// <param name="maxIndividualVariance">The most any counter can differ before we give up.</param>
    private static int patternMatchVariance(
      int[] counters,
      int[] pattern,
      int maxIndividualVariance)
    {
      int length = counters.Length;
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < length; ++index)
      {
        num1 += counters[index];
        num2 += pattern[index];
      }
      if (num1 < num2)
        return int.MaxValue;
      int num3 = (num1 << 8) / num2;
      maxIndividualVariance = maxIndividualVariance * num3 >> 8;
      int num4 = 0;
      for (int index = 0; index < length; ++index)
      {
        int num5 = counters[index] << 8;
        int num6 = pattern[index] * num3;
        int num7 = num5 > num6 ? num5 - num6 : num6 - num5;
        if (num7 > maxIndividualVariance)
          return int.MaxValue;
        num4 += num7;
      }
      return num4 / num1;
    }
  }
}
