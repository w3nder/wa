// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.FinderPatternFinder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.QrCode.Internal
{
  public class FinderPatternFinder
  {
    private const int CENTER_QUORUM = 2;
    /// <summary>1 pixel/module times 3 modules/center</summary>
    protected internal const int MIN_SKIP = 3;
    /// <summary>support up to version 10 for mobile clients</summary>
    protected internal const int MAX_MODULES = 57;
    private const int INTEGER_MATH_SHIFT = 8;
    private readonly BitMatrix image;
    private List<FinderPattern> possibleCenters;
    private bool hasSkipped;
    private readonly int[] crossCheckStateCount;
    private readonly ResultPointCallback resultPointCallback;

    /// <summary>
    /// <p>Creates a finder that will search the image for three finder patterns.</p>
    /// </summary>
    /// <param name="image">image to search</param>
    public FinderPatternFinder(BitMatrix image)
      : this(image, (ResultPointCallback) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.QrCode.Internal.FinderPatternFinder" /> class.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="resultPointCallback">The result point callback.</param>
    public FinderPatternFinder(BitMatrix image, ResultPointCallback resultPointCallback)
    {
      this.image = image;
      this.possibleCenters = new List<FinderPattern>();
      this.crossCheckStateCount = new int[5];
      this.resultPointCallback = resultPointCallback;
    }

    /// <summary>Gets the image.</summary>
    protected internal virtual BitMatrix Image => this.image;

    /// <summary>Gets the possible centers.</summary>
    protected internal virtual List<FinderPattern> PossibleCenters => this.possibleCenters;

    internal virtual FinderPatternInfo find(IDictionary<DecodeHintType, object> hints)
    {
      bool flag1 = hints != null && hints.ContainsKey(DecodeHintType.TRY_HARDER);
      bool pureBarcode = hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE);
      int height = this.image.Height;
      int width = this.image.Width;
      int num = 3 * height / 228;
      if (num < 3 || flag1)
        num = 3;
      bool flag2 = false;
      int[] stateCount = new int[5];
      for (int index1 = num - 1; index1 < height && !flag2; index1 += num)
      {
        stateCount[0] = 0;
        stateCount[1] = 0;
        stateCount[2] = 0;
        stateCount[3] = 0;
        stateCount[4] = 0;
        int index2 = 0;
        for (int index3 = 0; index3 < width; ++index3)
        {
          if (this.image[index3, index1])
          {
            if ((index2 & 1) == 1)
              ++index2;
            ++stateCount[index2];
          }
          else if ((index2 & 1) == 0)
          {
            if (index2 == 4)
            {
              if (FinderPatternFinder.foundPatternCross(stateCount))
              {
                if (this.handlePossibleCenter(stateCount, index1, index3, pureBarcode))
                {
                  num = 2;
                  if (this.hasSkipped)
                  {
                    flag2 = this.haveMultiplyConfirmedCenters();
                  }
                  else
                  {
                    int rowSkip = this.findRowSkip();
                    if (rowSkip > stateCount[2])
                    {
                      index1 += rowSkip - stateCount[2] - num;
                      index3 = width - 1;
                    }
                  }
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
        if (FinderPatternFinder.foundPatternCross(stateCount) && this.handlePossibleCenter(stateCount, index1, width, pureBarcode))
        {
          num = stateCount[0];
          if (this.hasSkipped)
            flag2 = this.haveMultiplyConfirmedCenters();
        }
      }
      FinderPattern[] finderPatternArray = this.selectBestPatterns();
      if (finderPatternArray == null)
        return (FinderPatternInfo) null;
      ResultPoint.orderBestPatterns((ResultPoint[]) finderPatternArray);
      return new FinderPatternInfo(finderPatternArray);
    }

    /// <summary> Given a count of black/white/black/white/black pixels just seen and an end position,
    /// figures the location of the center of this run.
    /// </summary>
    private static float? centerFromEnd(int[] stateCount, int end)
    {
      float f = (float) (end - stateCount[4] - stateCount[3]) - (float) stateCount[2] / 2f;
      return float.IsNaN(f) ? new float?() : new float?(f);
    }

    /// <param name="stateCount">count of black/white/black/white/black pixels just read
    /// </param>
    /// <returns> true iff the proportions of the counts is close enough to the 1/1/3/1/1 ratios
    /// used by finder patterns to be considered a match
    /// </returns>
    protected internal static bool foundPatternCross(int[] stateCount)
    {
      int num1 = 0;
      for (int index = 0; index < 5; ++index)
      {
        int num2 = stateCount[index];
        if (num2 == 0)
          return false;
        num1 += num2;
      }
      if (num1 < 7)
        return false;
      int num3 = (num1 << 8) / 7;
      int num4 = num3 / 2;
      return Math.Abs(num3 - (stateCount[0] << 8)) < num4 && Math.Abs(num3 - (stateCount[1] << 8)) < num4 && Math.Abs(3 * num3 - (stateCount[2] << 8)) < 3 * num4 && Math.Abs(num3 - (stateCount[3] << 8)) < num4 && Math.Abs(num3 - (stateCount[4] << 8)) < num4;
    }

    private int[] CrossCheckStateCount
    {
      get
      {
        this.crossCheckStateCount[0] = 0;
        this.crossCheckStateCount[1] = 0;
        this.crossCheckStateCount[2] = 0;
        this.crossCheckStateCount[3] = 0;
        this.crossCheckStateCount[4] = 0;
        return this.crossCheckStateCount;
      }
    }

    /// <summary>
    /// After a vertical and horizontal scan finds a potential finder pattern, this method
    /// "cross-cross-cross-checks" by scanning down diagonally through the center of the possible
    /// finder pattern to see if the same proportion is detected.
    /// </summary>
    /// <param name="startI">row where a finder pattern was detected</param>
    /// <param name="centerJ">center of the section that appears to cross a finder pattern</param>
    /// <param name="maxCount">maximum reasonable number of modules that should be observed in any reading state, based on the results of the horizontal scan</param>
    /// <param name="originalStateCountTotal">The original state count total.</param>
    /// <returns>true if proportions are withing expected limits</returns>
    private bool crossCheckDiagonal(
      int startI,
      int centerJ,
      int maxCount,
      int originalStateCountTotal)
    {
      int height = this.image.Height;
      int width = this.image.Width;
      int[] crossCheckStateCount = this.CrossCheckStateCount;
      int num1;
      for (num1 = 0; startI - num1 >= 0 && this.image[centerJ - num1, startI - num1]; ++num1)
        ++crossCheckStateCount[2];
      if (startI - num1 < 0 || centerJ - num1 < 0)
        return false;
      for (; startI - num1 >= 0 && centerJ - num1 >= 0 && !this.image[centerJ - num1, startI - num1] && crossCheckStateCount[1] <= maxCount; ++num1)
        ++crossCheckStateCount[1];
      if (startI - num1 < 0 || centerJ - num1 < 0 || crossCheckStateCount[1] > maxCount)
        return false;
      for (; startI - num1 >= 0 && centerJ - num1 >= 0 && this.image[centerJ - num1, startI - num1] && crossCheckStateCount[0] <= maxCount; ++num1)
        ++crossCheckStateCount[0];
      if (crossCheckStateCount[0] > maxCount)
        return false;
      int num2;
      for (num2 = 1; startI + num2 < height && centerJ + num2 < width && this.image[centerJ + num2, startI + num2]; ++num2)
        ++crossCheckStateCount[2];
      if (startI + num2 >= height || centerJ + num2 >= width)
        return false;
      for (; startI + num2 < height && centerJ + num2 < width && !this.image[centerJ + num2, startI + num2] && crossCheckStateCount[3] < maxCount; ++num2)
        ++crossCheckStateCount[3];
      if (startI + num2 >= height || centerJ + num2 >= width || crossCheckStateCount[3] >= maxCount)
        return false;
      for (; startI + num2 < height && centerJ + num2 < width && this.image[centerJ + num2, startI + num2] && crossCheckStateCount[4] < maxCount; ++num2)
        ++crossCheckStateCount[4];
      return crossCheckStateCount[4] < maxCount && Math.Abs(crossCheckStateCount[0] + crossCheckStateCount[1] + crossCheckStateCount[2] + crossCheckStateCount[3] + crossCheckStateCount[4] - originalStateCountTotal) < 2 * originalStateCountTotal && FinderPatternFinder.foundPatternCross(crossCheckStateCount);
    }

    /// <summary>
    ///   <p>After a horizontal scan finds a potential finder pattern, this method
    /// "cross-checks" by scanning down vertically through the center of the possible
    /// finder pattern to see if the same proportion is detected.</p>
    /// </summary>
    /// <param name="startI">row where a finder pattern was detected</param>
    /// <param name="centerJ">center of the section that appears to cross a finder pattern</param>
    /// <param name="maxCount">maximum reasonable number of modules that should be
    /// observed in any reading state, based on the results of the horizontal scan</param>
    /// <param name="originalStateCountTotal">The original state count total.</param>
    /// <returns>
    /// vertical center of finder pattern, or null if not found
    /// </returns>
    private float? crossCheckVertical(
      int startI,
      int centerJ,
      int maxCount,
      int originalStateCountTotal)
    {
      int height = this.image.Height;
      int[] crossCheckStateCount = this.CrossCheckStateCount;
      int y;
      for (y = startI; y >= 0 && this.image[centerJ, y]; --y)
        ++crossCheckStateCount[2];
      if (y < 0)
        return new float?();
      for (; y >= 0 && !this.image[centerJ, y] && crossCheckStateCount[1] <= maxCount; --y)
        ++crossCheckStateCount[1];
      if (y < 0 || crossCheckStateCount[1] > maxCount)
        return new float?();
      for (; y >= 0 && this.image[centerJ, y] && crossCheckStateCount[0] <= maxCount; --y)
        ++crossCheckStateCount[0];
      if (crossCheckStateCount[0] > maxCount)
        return new float?();
      int num;
      for (num = startI + 1; num < height && this.image[centerJ, num]; ++num)
        ++crossCheckStateCount[2];
      if (num == height)
        return new float?();
      for (; num < height && !this.image[centerJ, num] && crossCheckStateCount[3] < maxCount; ++num)
        ++crossCheckStateCount[3];
      if (num == height || crossCheckStateCount[3] >= maxCount)
        return new float?();
      for (; num < height && this.image[centerJ, num] && crossCheckStateCount[4] < maxCount; ++num)
        ++crossCheckStateCount[4];
      if (crossCheckStateCount[4] >= maxCount)
        return new float?();
      if (5 * Math.Abs(crossCheckStateCount[0] + crossCheckStateCount[1] + crossCheckStateCount[2] + crossCheckStateCount[3] + crossCheckStateCount[4] - originalStateCountTotal) >= 2 * originalStateCountTotal)
        return new float?();
      return !FinderPatternFinder.foundPatternCross(crossCheckStateCount) ? new float?() : FinderPatternFinder.centerFromEnd(crossCheckStateCount, num);
    }

    /// <summary> <p>Like {@link #crossCheckVertical(int, int, int, int)}, and in fact is basically identical,
    /// except it reads horizontally instead of vertically. This is used to cross-cross
    /// check a vertical cross check and locate the real center of the alignment pattern.</p>
    /// </summary>
    private float? crossCheckHorizontal(
      int startJ,
      int centerI,
      int maxCount,
      int originalStateCountTotal)
    {
      int width = this.image.Width;
      int[] crossCheckStateCount = this.CrossCheckStateCount;
      int x;
      for (x = startJ; x >= 0 && this.image[x, centerI]; --x)
        ++crossCheckStateCount[2];
      if (x < 0)
        return new float?();
      for (; x >= 0 && !this.image[x, centerI] && crossCheckStateCount[1] <= maxCount; --x)
        ++crossCheckStateCount[1];
      if (x < 0 || crossCheckStateCount[1] > maxCount)
        return new float?();
      for (; x >= 0 && this.image[x, centerI] && crossCheckStateCount[0] <= maxCount; --x)
        ++crossCheckStateCount[0];
      if (crossCheckStateCount[0] > maxCount)
        return new float?();
      int num;
      for (num = startJ + 1; num < width && this.image[num, centerI]; ++num)
        ++crossCheckStateCount[2];
      if (num == width)
        return new float?();
      for (; num < width && !this.image[num, centerI] && crossCheckStateCount[3] < maxCount; ++num)
        ++crossCheckStateCount[3];
      if (num == width || crossCheckStateCount[3] >= maxCount)
        return new float?();
      for (; num < width && this.image[num, centerI] && crossCheckStateCount[4] < maxCount; ++num)
        ++crossCheckStateCount[4];
      if (crossCheckStateCount[4] >= maxCount)
        return new float?();
      if (5 * Math.Abs(crossCheckStateCount[0] + crossCheckStateCount[1] + crossCheckStateCount[2] + crossCheckStateCount[3] + crossCheckStateCount[4] - originalStateCountTotal) >= originalStateCountTotal)
        return new float?();
      return !FinderPatternFinder.foundPatternCross(crossCheckStateCount) ? new float?() : FinderPatternFinder.centerFromEnd(crossCheckStateCount, num);
    }

    /// <summary>
    ///   <p>This is called when a horizontal scan finds a possible alignment pattern. It will
    /// cross check with a vertical scan, and if successful, will, ah, cross-cross-check
    /// with another horizontal scan. This is needed primarily to locate the real horizontal
    /// center of the pattern in cases of extreme skew.
    /// And then we cross-cross-cross check with another diagonal scan.</p>
    /// If that succeeds the finder pattern location is added to a list that tracks
    /// the number of times each location has been nearly-matched as a finder pattern.
    /// Each additional find is more evidence that the location is in fact a finder
    /// pattern center
    /// </summary>
    /// <param name="stateCount">reading state module counts from horizontal scan</param>
    /// <param name="i">row where finder pattern may be found</param>
    /// <param name="j">end of possible finder pattern in row</param>
    /// <param name="pureBarcode">if set to <c>true</c> [pure barcode].</param>
    /// <returns>
    /// true if a finder pattern candidate was found this time
    /// </returns>
    protected bool handlePossibleCenter(int[] stateCount, int i, int j, bool pureBarcode)
    {
      int originalStateCountTotal = stateCount[0] + stateCount[1] + stateCount[2] + stateCount[3] + stateCount[4];
      float? nullable1 = FinderPatternFinder.centerFromEnd(stateCount, j);
      if (!nullable1.HasValue)
        return false;
      float? nullable2 = this.crossCheckVertical(i, (int) nullable1.Value, stateCount[2], originalStateCountTotal);
      if (nullable2.HasValue)
      {
        nullable1 = this.crossCheckHorizontal((int) nullable1.Value, (int) nullable2.Value, stateCount[2], originalStateCountTotal);
        if (nullable1.HasValue && (!pureBarcode || this.crossCheckDiagonal((int) nullable2.Value, (int) nullable1.Value, stateCount[2], originalStateCountTotal)))
        {
          float num = (float) originalStateCountTotal / 7f;
          bool flag = false;
          for (int index = 0; index < this.possibleCenters.Count; ++index)
          {
            FinderPattern possibleCenter = this.possibleCenters[index];
            if (possibleCenter.aboutEquals(num, nullable2.Value, nullable1.Value))
            {
              this.possibleCenters.RemoveAt(index);
              this.possibleCenters.Insert(index, possibleCenter.combineEstimate(nullable2.Value, nullable1.Value, num));
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            FinderPattern point = new FinderPattern(nullable1.Value, nullable2.Value, num);
            this.possibleCenters.Add(point);
            if (this.resultPointCallback != null)
              this.resultPointCallback((ResultPoint) point);
          }
          return true;
        }
      }
      return false;
    }

    /// <returns> number of rows we could safely skip during scanning, based on the first
    /// two finder patterns that have been located. In some cases their position will
    /// allow us to infer that the third pattern must lie below a certain point farther
    /// down in the image.
    /// </returns>
    private int findRowSkip()
    {
      if (this.possibleCenters.Count <= 1)
        return 0;
      ResultPoint resultPoint = (ResultPoint) null;
      foreach (FinderPattern possibleCenter in this.possibleCenters)
      {
        if (possibleCenter.Count >= 2)
        {
          if (resultPoint == null)
          {
            resultPoint = (ResultPoint) possibleCenter;
          }
          else
          {
            this.hasSkipped = true;
            return (int) ((double) Math.Abs(resultPoint.X - possibleCenter.X) - (double) Math.Abs(resultPoint.Y - possibleCenter.Y)) / 2;
          }
        }
      }
      return 0;
    }

    /// <returns> true iff we have found at least 3 finder patterns that have been detected
    /// at least {@link #CENTER_QUORUM} times each, and, the estimated module size of the
    /// candidates is "pretty similar"
    /// </returns>
    private bool haveMultiplyConfirmedCenters()
    {
      int num1 = 0;
      float num2 = 0.0f;
      int count = this.possibleCenters.Count;
      foreach (FinderPattern possibleCenter in this.possibleCenters)
      {
        if (possibleCenter.Count >= 2)
        {
          ++num1;
          num2 += possibleCenter.EstimatedModuleSize;
        }
      }
      if (num1 < 3)
        return false;
      float num3 = num2 / (float) count;
      float num4 = 0.0f;
      for (int index = 0; index < count; ++index)
      {
        FinderPattern possibleCenter = this.possibleCenters[index];
        num4 += Math.Abs(possibleCenter.EstimatedModuleSize - num3);
      }
      return (double) num4 <= 0.05000000074505806 * (double) num2;
    }

    /// <returns> the 3 best {@link FinderPattern}s from our list of candidates. The "best" are
    /// those that have been detected at least {@link #CENTER_QUORUM} times, and whose module
    /// size differs from the average among those patterns the least
    /// </returns>
    private FinderPattern[] selectBestPatterns()
    {
      int count = this.possibleCenters.Count;
      if (count < 3)
        return (FinderPattern[]) null;
      if (count > 3)
      {
        float num1 = 0.0f;
        float num2 = 0.0f;
        foreach (FinderPattern possibleCenter in this.possibleCenters)
        {
          float estimatedModuleSize = possibleCenter.EstimatedModuleSize;
          num1 += estimatedModuleSize;
          num2 += estimatedModuleSize * estimatedModuleSize;
        }
        float f = num1 / (float) count;
        float val2 = (float) Math.Sqrt((double) num2 / (double) count - (double) f * (double) f);
        this.possibleCenters.Sort((IComparer<FinderPattern>) new FinderPatternFinder.FurthestFromAverageComparator(f));
        float num3 = Math.Max(0.2f * f, val2);
        for (int index = 0; index < this.possibleCenters.Count && this.possibleCenters.Count > 3; ++index)
        {
          if ((double) Math.Abs(this.possibleCenters[index].EstimatedModuleSize - f) > (double) num3)
          {
            this.possibleCenters.RemoveAt(index);
            --index;
          }
        }
      }
      if (this.possibleCenters.Count > 3)
      {
        float num = 0.0f;
        foreach (FinderPattern possibleCenter in this.possibleCenters)
          num += possibleCenter.EstimatedModuleSize;
        this.possibleCenters.Sort((IComparer<FinderPattern>) new FinderPatternFinder.CenterComparator(num / (float) this.possibleCenters.Count));
        this.possibleCenters = this.possibleCenters.GetRange(0, 3);
      }
      return new FinderPattern[3]
      {
        this.possibleCenters[0],
        this.possibleCenters[1],
        this.possibleCenters[2]
      };
    }

    /// <summary>Orders by furthest from average</summary>
    private sealed class FurthestFromAverageComparator : IComparer<FinderPattern>
    {
      private readonly float average;

      public FurthestFromAverageComparator(float f) => this.average = f;

      public int Compare(FinderPattern x, FinderPattern y)
      {
        float num1 = Math.Abs(y.EstimatedModuleSize - this.average);
        float num2 = Math.Abs(x.EstimatedModuleSize - this.average);
        if ((double) num1 < (double) num2)
          return -1;
        return (double) num1 != (double) num2 ? 1 : 0;
      }
    }

    /// <summary> <p>Orders by {@link FinderPattern#getCount()}, descending.</p></summary>
    private sealed class CenterComparator : IComparer<FinderPattern>
    {
      private readonly float average;

      public CenterComparator(float f) => this.average = f;

      public int Compare(FinderPattern x, FinderPattern y)
      {
        if (y.Count != x.Count)
          return y.Count - x.Count;
        float num1 = Math.Abs(y.EstimatedModuleSize - this.average);
        float num2 = Math.Abs(x.EstimatedModuleSize - this.average);
        if ((double) num1 < (double) num2)
          return 1;
        return (double) num1 != (double) num2 ? -1 : 0;
      }
    }
  }
}
