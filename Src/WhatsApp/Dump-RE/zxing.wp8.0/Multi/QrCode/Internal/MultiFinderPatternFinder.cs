// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.QrCode.Internal.MultiFinderPatternFinder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.Multi.QrCode.Internal
{
  internal sealed class MultiFinderPatternFinder : FinderPatternFinder
  {
    /// <summary>
    /// More or less arbitrary cutoff point for determining if two finder patterns might belong
    /// to the same code if they differ less than DIFF_MODSIZE_CUTOFF pixels/module in their
    /// estimated modules sizes.
    /// </summary>
    private const float DIFF_MODSIZE_CUTOFF = 0.5f;
    private static FinderPatternInfo[] EMPTY_RESULT_ARRAY = new FinderPatternInfo[0];
    private static float MAX_MODULE_COUNT_PER_EDGE = 180f;
    private static float MIN_MODULE_COUNT_PER_EDGE = 9f;
    /// <summary>
    /// More or less arbitrary cutoff point for determining if two finder patterns might belong
    /// to the same code if they differ less than DIFF_MODSIZE_CUTOFF_PERCENT percent in their
    /// estimated modules sizes.
    /// </summary>
    private static float DIFF_MODSIZE_CUTOFF_PERCENT = 0.05f;

    /// <summary>
    /// <p>Creates a finder that will search the image for three finder patterns.</p>
    /// 
    /// <param name="image">image to search</param>
    /// </summary>
    internal MultiFinderPatternFinder(BitMatrix image)
      : base(image)
    {
    }

    internal MultiFinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
      : base(image, resultPointCallback)
    {
    }

    /// <summary>
    /// </summary>
    /// <returns>the 3 best <see cref="T:ZXing.QrCode.Internal.FinderPattern" />s from our list of candidates. The "best" are
    ///         those that have been detected at least CENTER_QUORUM times, and whose module
    ///         size differs from the average among those patterns the least
    /// </returns>
    private FinderPattern[][] selectMutipleBestPatterns()
    {
      List<FinderPattern> possibleCenters = this.PossibleCenters;
      int count = possibleCenters.Count;
      if (count < 3)
        return (FinderPattern[][]) null;
      if (count == 3)
        return new FinderPattern[1][]
        {
          new FinderPattern[3]
          {
            possibleCenters[0],
            possibleCenters[1],
            possibleCenters[2]
          }
        };
      possibleCenters.Sort((IComparer<FinderPattern>) new MultiFinderPatternFinder.ModuleSizeComparator());
      List<FinderPattern[]> finderPatternArrayList = new List<FinderPattern[]>();
      for (int index1 = 0; index1 < count - 2; ++index1)
      {
        FinderPattern finderPattern1 = possibleCenters[index1];
        if (finderPattern1 != null)
        {
          for (int index2 = index1 + 1; index2 < count - 1; ++index2)
          {
            FinderPattern finderPattern2 = possibleCenters[index2];
            if (finderPattern2 != null)
            {
              float num1 = (finderPattern1.EstimatedModuleSize - finderPattern2.EstimatedModuleSize) / Math.Min(finderPattern1.EstimatedModuleSize, finderPattern2.EstimatedModuleSize);
              if ((double) Math.Abs(finderPattern1.EstimatedModuleSize - finderPattern2.EstimatedModuleSize) <= 0.5 || (double) num1 < (double) MultiFinderPatternFinder.DIFF_MODSIZE_CUTOFF_PERCENT)
              {
                for (int index3 = index2 + 1; index3 < count; ++index3)
                {
                  FinderPattern finderPattern3 = possibleCenters[index3];
                  if (finderPattern3 != null)
                  {
                    float num2 = (finderPattern2.EstimatedModuleSize - finderPattern3.EstimatedModuleSize) / Math.Min(finderPattern2.EstimatedModuleSize, finderPattern3.EstimatedModuleSize);
                    if ((double) Math.Abs(finderPattern2.EstimatedModuleSize - finderPattern3.EstimatedModuleSize) <= 0.5 || (double) num2 < (double) MultiFinderPatternFinder.DIFF_MODSIZE_CUTOFF_PERCENT)
                    {
                      FinderPattern[] finderPatternArray = new FinderPattern[3]
                      {
                        finderPattern1,
                        finderPattern2,
                        finderPattern3
                      };
                      ResultPoint.orderBestPatterns((ResultPoint[]) finderPatternArray);
                      FinderPatternInfo finderPatternInfo = new FinderPatternInfo(finderPatternArray);
                      float val1_1 = ResultPoint.distance((ResultPoint) finderPatternInfo.TopLeft, (ResultPoint) finderPatternInfo.BottomLeft);
                      float val1_2 = ResultPoint.distance((ResultPoint) finderPatternInfo.TopRight, (ResultPoint) finderPatternInfo.BottomLeft);
                      float val2_1 = ResultPoint.distance((ResultPoint) finderPatternInfo.TopLeft, (ResultPoint) finderPatternInfo.TopRight);
                      float num3 = (float) (((double) val1_1 + (double) val2_1) / ((double) finderPattern1.EstimatedModuleSize * 2.0));
                      if ((double) num3 <= (double) MultiFinderPatternFinder.MAX_MODULE_COUNT_PER_EDGE && (double) num3 >= (double) MultiFinderPatternFinder.MIN_MODULE_COUNT_PER_EDGE && (double) Math.Abs((val1_1 - val2_1) / Math.Min(val1_1, val2_1)) < 0.10000000149011612)
                      {
                        float val2_2 = (float) Math.Sqrt((double) val1_1 * (double) val1_1 + (double) val2_1 * (double) val2_1);
                        if ((double) Math.Abs((val1_2 - val2_2) / Math.Min(val1_2, val2_2)) < 0.10000000149011612)
                          finderPatternArrayList.Add(finderPatternArray);
                      }
                    }
                    else
                      break;
                  }
                }
              }
              else
                break;
            }
          }
        }
      }
      return finderPatternArrayList.Count != 0 ? finderPatternArrayList.ToArray() : (FinderPattern[][]) null;
    }

    public FinderPatternInfo[] findMulti(IDictionary<DecodeHintType, object> hints)
    {
      bool flag = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
      bool pureBarcode = hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE);
      BitMatrix image = this.Image;
      int height = image.Height;
      int width = image.Width;
      int num = (int) ((double) height / 228.0 * 3.0);
      if (num < 3 || flag)
        num = 3;
      int[] stateCount = new int[5];
      for (int index1 = num - 1; index1 < height; index1 += num)
      {
        stateCount[0] = 0;
        stateCount[1] = 0;
        stateCount[2] = 0;
        stateCount[3] = 0;
        stateCount[4] = 0;
        int index2 = 0;
        for (int index3 = 0; index3 < width; ++index3)
        {
          if (image[index3, index1])
          {
            if ((index2 & 1) == 1)
              ++index2;
            ++stateCount[index2];
          }
          else if ((index2 & 1) == 0)
          {
            if (index2 == 4)
            {
              if (FinderPatternFinder.foundPatternCross(stateCount) && this.handlePossibleCenter(stateCount, index1, index3, pureBarcode))
              {
                index2 = 0;
                stateCount[0] = 0;
                stateCount[1] = 0;
                stateCount[2] = 0;
                stateCount[3] = 0;
                stateCount[4] = 0;
              }
              else
              {
                stateCount[0] = stateCount[2];
                stateCount[1] = stateCount[3];
                stateCount[2] = stateCount[4];
                stateCount[3] = 1;
                stateCount[4] = 0;
                index2 = 3;
              }
            }
            else
              ++stateCount[++index2];
          }
          else
            ++stateCount[index2];
        }
        if (FinderPatternFinder.foundPatternCross(stateCount))
          this.handlePossibleCenter(stateCount, index1, width, pureBarcode);
      }
      FinderPattern[][] finderPatternArray1 = this.selectMutipleBestPatterns();
      if (finderPatternArray1 == null)
        return MultiFinderPatternFinder.EMPTY_RESULT_ARRAY;
      List<FinderPatternInfo> finderPatternInfoList = new List<FinderPatternInfo>();
      foreach (FinderPattern[] finderPatternArray2 in finderPatternArray1)
      {
        ResultPoint.orderBestPatterns((ResultPoint[]) finderPatternArray2);
        finderPatternInfoList.Add(new FinderPatternInfo(finderPatternArray2));
      }
      return finderPatternInfoList.Count == 0 ? MultiFinderPatternFinder.EMPTY_RESULT_ARRAY : finderPatternInfoList.ToArray();
    }

    /// <summary>
    /// A comparator that orders FinderPatterns by their estimated module size.
    /// </summary>
    private sealed class ModuleSizeComparator : IComparer<FinderPattern>
    {
      public int Compare(FinderPattern center1, FinderPattern center2)
      {
        float num = center2.EstimatedModuleSize - center1.EstimatedModuleSize;
        if ((double) num < 0.0)
          return -1;
        return (double) num <= 0.0 ? 0 : 1;
      }
    }
  }
}
