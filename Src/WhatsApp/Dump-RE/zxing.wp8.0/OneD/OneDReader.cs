// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.OneDReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// Encapsulates functionality and implementation that is common to all families
  /// of one-dimensional barcodes.
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>Sean Owen</author>
  /// </summary>
  public abstract class OneDReader : Reader
  {
    /// <summary>
    /// 
    /// </summary>
    protected static int INTEGER_MATH_SHIFT = 8;
    /// <summary>
    /// 
    /// </summary>
    protected static int PATTERN_MATCH_RESULT_SCALE_FACTOR = 1 << OneDReader.INTEGER_MATH_SHIFT;

    /// <summary>
    /// Locates and decodes a barcode in some format within an image.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    /// Locates and decodes a barcode in some format within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// Note that we don't try rotation without the try harder flag, even if rotation was supported.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcode encodes</returns>
    public virtual Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      Result result = this.doDecode(image, hints);
      if (result == null)
      {
        bool flag1 = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
        bool flag2 = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER_WITHOUT_ROTATION);
        if (flag1 && !flag2 && image.RotateSupported)
        {
          BinaryBitmap image1 = image.rotateCounterClockwise();
          result = this.doDecode(image1, hints);
          if (result == null)
            return (Result) null;
          IDictionary<ResultMetadataType, object> resultMetadata = result.ResultMetadata;
          int num = 270;
          if (resultMetadata != null && resultMetadata.ContainsKey(ResultMetadataType.ORIENTATION))
            num = (num + (int) resultMetadata[ResultMetadataType.ORIENTATION]) % 360;
          result.putMetadata(ResultMetadataType.ORIENTATION, (object) num);
          ResultPoint[] resultPoints = result.ResultPoints;
          if (resultPoints != null)
          {
            int height = image1.Height;
            for (int index = 0; index < resultPoints.Length; ++index)
              resultPoints[index] = new ResultPoint((float) ((double) height - (double) resultPoints[index].Y - 1.0), resultPoints[index].X);
          }
        }
      }
      return result;
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    public virtual void reset()
    {
    }

    /// <summary>
    /// We're going to examine rows from the middle outward, searching alternately above and below the
    /// middle, and farther out each time. rowStep is the number of rows between each successive
    /// attempt above and below the middle. So we'd scan row middle, then middle - rowStep, then
    /// middle + rowStep, then middle - (2 * rowStep), etc.
    /// rowStep is bigger as the image is taller, but is always at least 1. We've somewhat arbitrarily
    /// decided that moving up and down by about 1/16 of the image is pretty good; we try more of the
    /// image if "trying harder".
    /// </summary>
    /// <param name="image">The image to decode</param>
    /// <param name="hints">Any hints that were requested</param>
    /// <returns>The contents of the decoded barcode</returns>
    private Result doDecode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      int width = image.Width;
      int height = image.Height;
      BitArray row = new BitArray(width);
      int num1 = height >> 1;
      bool flag1 = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
      int num2 = Math.Max(1, height >> (flag1 ? 8 : 5));
      int num3 = !flag1 ? 15 : height;
      for (int index1 = 0; index1 < num3; ++index1)
      {
        int num4 = index1 + 1 >> 1;
        bool flag2 = (index1 & 1) == 0;
        int num5 = num1 + num2 * (flag2 ? num4 : -num4);
        if (num5 >= 0 && num5 < height)
        {
          row = image.getBlackRow(num5, row);
          if (row != null)
          {
            for (int index2 = 0; index2 < 2; ++index2)
            {
              if (index2 == 1)
              {
                row.reverse();
                if (hints != null && hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
                {
                  IDictionary<DecodeHintType, object> dictionary = (IDictionary<DecodeHintType, object>) new Dictionary<DecodeHintType, object>();
                  foreach (KeyValuePair<DecodeHintType, object> hint in (IEnumerable<KeyValuePair<DecodeHintType, object>>) hints)
                  {
                    if (hint.Key != DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                      dictionary.Add(hint.Key, hint.Value);
                  }
                  hints = dictionary;
                }
              }
              Result result = this.decodeRow(num5, row, hints);
              if (result != null)
              {
                if (index2 == 1)
                {
                  result.putMetadata(ResultMetadataType.ORIENTATION, (object) 180);
                  ResultPoint[] resultPoints = result.ResultPoints;
                  if (resultPoints != null)
                  {
                    resultPoints[0] = new ResultPoint((float) ((double) width - (double) resultPoints[0].X - 1.0), resultPoints[0].Y);
                    resultPoints[1] = new ResultPoint((float) ((double) width - (double) resultPoints[1].X - 1.0), resultPoints[1].Y);
                  }
                }
                return result;
              }
            }
          }
        }
        else
          break;
      }
      return (Result) null;
    }

    /// <summary>
    /// Records the size of successive runs of white and black pixels in a row, starting at a given point.
    /// The values are recorded in the given array, and the number of runs recorded is equal to the size
    /// of the array. If the row starts on a white pixel at the given start point, then the first count
    /// recorded is the run of white pixels starting from that point; likewise it is the count of a run
    /// of black pixels if the row begin on a black pixels at that point.
    /// </summary>
    /// <param name="row">row to count from</param>
    /// <param name="start">offset into row to start at</param>
    /// <param name="counters">array into which to record counts</param>
    protected static bool recordPattern(BitArray row, int start, int[] counters)
    {
      return OneDReader.recordPattern(row, start, counters, counters.Length);
    }

    /// <summary>
    /// Records the size of successive runs of white and black pixels in a row, starting at a given point.
    /// The values are recorded in the given array, and the number of runs recorded is equal to the size
    /// of the array. If the row starts on a white pixel at the given start point, then the first count
    /// recorded is the run of white pixels starting from that point; likewise it is the count of a run
    /// of black pixels if the row begin on a black pixels at that point.
    /// </summary>
    /// <param name="row">row to count from</param>
    /// <param name="start">offset into row to start at</param>
    /// <param name="counters">array into which to record counts</param>
    protected static bool recordPattern(BitArray row, int start, int[] counters, int numCounters)
    {
      for (int index = 0; index < numCounters; ++index)
        counters[index] = 0;
      int size = row.Size;
      if (start >= size)
        return false;
      bool flag = !row[start];
      int index1 = 0;
      int i;
      for (i = start; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++counters[index1];
        }
        else
        {
          ++index1;
          if (index1 != numCounters)
          {
            counters[index1] = 1;
            flag = !flag;
          }
          else
            break;
        }
      }
      if (index1 == numCounters)
        return true;
      return index1 == numCounters - 1 && i == size;
    }

    /// <summary>Records the pattern in reverse.</summary>
    /// <param name="row">The row.</param>
    /// <param name="start">The start.</param>
    /// <param name="counters">The counters.</param>
    /// <returns></returns>
    protected static bool recordPatternInReverse(BitArray row, int start, int[] counters)
    {
      int length = counters.Length;
      bool flag = row[start];
      while (start > 0 && length >= 0)
      {
        if (row[--start] != flag)
        {
          --length;
          flag = !flag;
        }
      }
      return length < 0 && OneDReader.recordPattern(row, start + 1, counters);
    }

    /// <summary>
    /// Determines how closely a set of observed counts of runs of black/white values matches a given
    /// target pattern. This is reported as the ratio of the total variance from the expected pattern
    /// proportions across all pattern elements, to the length of the pattern.
    /// </summary>
    /// <param name="counters">observed counters</param>
    /// <param name="pattern">expected pattern</param>
    /// <param name="maxIndividualVariance">The most any counter can differ before we give up</param>
    /// <returns>ratio of total variance between counters and pattern compared to total pattern size,
    /// where the ratio has been multiplied by 256. So, 0 means no variance (perfect match); 256 means
    /// the total variance between counters and patterns equals the pattern length, higher values mean
    /// even more variance</returns>
    protected static int patternMatchVariance(
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
      int num3 = (num1 << OneDReader.INTEGER_MATH_SHIFT) / num2;
      maxIndividualVariance = maxIndividualVariance * num3 >> OneDReader.INTEGER_MATH_SHIFT;
      int num4 = 0;
      for (int index = 0; index < length; ++index)
      {
        int num5 = counters[index] << OneDReader.INTEGER_MATH_SHIFT;
        int num6 = pattern[index] * num3;
        int num7 = num5 > num6 ? num5 - num6 : num6 - num5;
        if (num7 > maxIndividualVariance)
          return int.MaxValue;
        num4 += num7;
      }
      return num4 / num1;
    }

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
    public abstract Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints);
  }
}
