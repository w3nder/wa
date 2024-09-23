// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.Internal.AlignmentPatternFinder
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
  /// <summary> <p>This class attempts to find alignment patterns in a QR Code. Alignment patterns look like finder
  /// patterns but are smaller and appear at regular intervals throughout the image.</p>
  /// 
  /// <p>At the moment this only looks for the bottom-right alignment pattern.</p>
  /// 
  /// <p>This is mostly a simplified copy of {@link FinderPatternFinder}. It is copied,
  /// pasted and stripped down here for maximum performance but does unfortunately duplicate
  /// some code.</p>
  /// 
  /// <p>This class is thread-safe but not reentrant. Each thread must allocate its own object.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class AlignmentPatternFinder
  {
    private readonly BitMatrix image;
    private readonly IList<AlignmentPattern> possibleCenters;
    private readonly int startX;
    private readonly int startY;
    private readonly int width;
    private readonly int height;
    private readonly float moduleSize;
    private readonly int[] crossCheckStateCount;
    private readonly ResultPointCallback resultPointCallback;

    /// <summary> <p>Creates a finder that will look in a portion of the whole image.</p>
    /// 
    /// </summary>
    /// <param name="image">image to search</param>
    /// <param name="startX">left column from which to start searching</param>
    /// <param name="startY">top row from which to start searching</param>
    /// <param name="width">width of region to search</param>
    /// <param name="height">height of region to search</param>
    /// <param name="moduleSize">estimated module size so far</param>
    internal AlignmentPatternFinder(
      BitMatrix image,
      int startX,
      int startY,
      int width,
      int height,
      float moduleSize,
      ResultPointCallback resultPointCallback)
    {
      this.image = image;
      this.possibleCenters = (IList<AlignmentPattern>) new List<AlignmentPattern>(5);
      this.startX = startX;
      this.startY = startY;
      this.width = width;
      this.height = height;
      this.moduleSize = moduleSize;
      this.crossCheckStateCount = new int[3];
      this.resultPointCallback = resultPointCallback;
    }

    /// <summary> <p>This method attempts to find the bottom-right alignment pattern in the image. It is a bit messy since
    /// it's pretty performance-critical and so is written to be fast foremost.</p>
    /// 
    /// </summary>
    /// <returns>{@link AlignmentPattern} if found</returns>
    internal AlignmentPattern find()
    {
      int startX = this.startX;
      int height = this.height;
      int j = startX + this.width;
      int num1 = this.startY + (height >> 1);
      int[] stateCount = new int[3];
      for (int index1 = 0; index1 < height; ++index1)
      {
        int num2 = num1 + ((index1 & 1) == 0 ? index1 + 1 >> 1 : -(index1 + 1 >> 1));
        stateCount[0] = 0;
        stateCount[1] = 0;
        stateCount[2] = 0;
        int num3 = startX;
        while (num3 < j && !this.image[num3, num2])
          ++num3;
        int index2 = 0;
        for (; num3 < j; ++num3)
        {
          if (this.image[num3, num2])
          {
            switch (index2)
            {
              case 1:
                ++stateCount[index2];
                continue;
              case 2:
                if (this.foundPatternCross(stateCount))
                {
                  AlignmentPattern alignmentPattern = this.handlePossibleCenter(stateCount, num2, num3);
                  if (alignmentPattern != null)
                    return alignmentPattern;
                }
                stateCount[0] = stateCount[2];
                stateCount[1] = 1;
                stateCount[2] = 0;
                index2 = 1;
                continue;
              default:
                ++stateCount[++index2];
                continue;
            }
          }
          else
          {
            if (index2 == 1)
              ++index2;
            ++stateCount[index2];
          }
        }
        if (this.foundPatternCross(stateCount))
        {
          AlignmentPattern alignmentPattern = this.handlePossibleCenter(stateCount, num2, j);
          if (alignmentPattern != null)
            return alignmentPattern;
        }
      }
      return this.possibleCenters.Count != 0 ? this.possibleCenters[0] : (AlignmentPattern) null;
    }

    /// <summary> Given a count of black/white/black pixels just seen and an end position,
    /// figures the location of the center of this black/white/black run.
    /// </summary>
    private static float? centerFromEnd(int[] stateCount, int end)
    {
      float f = (float) (end - stateCount[2]) - (float) stateCount[1] / 2f;
      return float.IsNaN(f) ? new float?() : new float?(f);
    }

    /// <param name="stateCount">count of black/white/black pixels just read</param>
    /// <returns> true iff the proportions of the counts is close enough to the 1/1/1 ratios
    /// used by alignment patterns to be considered a match
    /// </returns>
    private bool foundPatternCross(int[] stateCount)
    {
      float num = this.moduleSize / 2f;
      for (int index = 0; index < 3; ++index)
      {
        if ((double) Math.Abs(this.moduleSize - (float) stateCount[index]) >= (double) num)
          return false;
      }
      return true;
    }

    /// <summary>
    ///   <p>After a horizontal scan finds a potential alignment pattern, this method
    /// "cross-checks" by scanning down vertically through the center of the possible
    /// alignment pattern to see if the same proportion is detected.</p>
    /// </summary>
    /// <param name="startI">row where an alignment pattern was detected</param>
    /// <param name="centerJ">center of the section that appears to cross an alignment pattern</param>
    /// <param name="maxCount">maximum reasonable number of modules that should be
    /// observed in any reading state, based on the results of the horizontal scan</param>
    /// <param name="originalStateCountTotal">The original state count total.</param>
    /// <returns>
    /// vertical center of alignment pattern, or null if not found
    /// </returns>
    private float? crossCheckVertical(
      int startI,
      int centerJ,
      int maxCount,
      int originalStateCountTotal)
    {
      int height = this.image.Height;
      int[] crossCheckStateCount = this.crossCheckStateCount;
      crossCheckStateCount[0] = 0;
      crossCheckStateCount[1] = 0;
      crossCheckStateCount[2] = 0;
      int y;
      for (y = startI; y >= 0 && this.image[centerJ, y] && crossCheckStateCount[1] <= maxCount; --y)
        ++crossCheckStateCount[1];
      if (y < 0 || crossCheckStateCount[1] > maxCount)
        return new float?();
      for (; y >= 0 && !this.image[centerJ, y] && crossCheckStateCount[0] <= maxCount; --y)
        ++crossCheckStateCount[0];
      if (crossCheckStateCount[0] > maxCount)
        return new float?();
      int num;
      for (num = startI + 1; num < height && this.image[centerJ, num] && crossCheckStateCount[1] <= maxCount; ++num)
        ++crossCheckStateCount[1];
      if (num == height || crossCheckStateCount[1] > maxCount)
        return new float?();
      for (; num < height && !this.image[centerJ, num] && crossCheckStateCount[2] <= maxCount; ++num)
        ++crossCheckStateCount[2];
      if (crossCheckStateCount[2] > maxCount)
        return new float?();
      if (5 * Math.Abs(crossCheckStateCount[0] + crossCheckStateCount[1] + crossCheckStateCount[2] - originalStateCountTotal) >= 2 * originalStateCountTotal)
        return new float?();
      return !this.foundPatternCross(crossCheckStateCount) ? new float?() : AlignmentPatternFinder.centerFromEnd(crossCheckStateCount, num);
    }

    /// <summary> <p>This is called when a horizontal scan finds a possible alignment pattern. It will
    /// cross check with a vertical scan, and if successful, will see if this pattern had been
    /// found on a previous horizontal scan. If so, we consider it confirmed and conclude we have
    /// found the alignment pattern.</p>
    /// 
    /// </summary>
    /// <param name="stateCount">reading state module counts from horizontal scan</param>
    /// <param name="i">row where alignment pattern may be found</param>
    /// <param name="j">end of possible alignment pattern in row</param>
    /// <returns> {@link AlignmentPattern} if we have found the same pattern twice, or null if not
    /// </returns>
    private AlignmentPattern handlePossibleCenter(int[] stateCount, int i, int j)
    {
      int originalStateCountTotal = stateCount[0] + stateCount[1] + stateCount[2];
      float? nullable1 = AlignmentPatternFinder.centerFromEnd(stateCount, j);
      if (!nullable1.HasValue)
        return (AlignmentPattern) null;
      float? nullable2 = this.crossCheckVertical(i, (int) nullable1.Value, 2 * stateCount[1], originalStateCountTotal);
      if (nullable2.HasValue)
      {
        float num = (float) (stateCount[0] + stateCount[1] + stateCount[2]) / 3f;
        foreach (AlignmentPattern possibleCenter in (IEnumerable<AlignmentPattern>) this.possibleCenters)
        {
          if (possibleCenter.aboutEquals(num, nullable2.Value, nullable1.Value))
            return possibleCenter.combineEstimate(nullable2.Value, nullable1.Value, num);
        }
        AlignmentPattern point = new AlignmentPattern(nullable1.Value, nullable2.Value, num);
        this.possibleCenters.Add(point);
        if (this.resultPointCallback != null)
          this.resultPointCallback((ResultPoint) point);
      }
      return (AlignmentPattern) null;
    }
  }
}
