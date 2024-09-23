// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.RSS14Reader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD.RSS
{
  /// <summary>
  /// Decodes RSS-14, including truncated and stacked variants. See ISO/IEC 24724:2006.
  /// </summary>
  public sealed class RSS14Reader : AbstractRSSReader
  {
    private static readonly int[] OUTSIDE_EVEN_TOTAL_SUBSET = new int[5]
    {
      1,
      10,
      34,
      70,
      126
    };
    private static readonly int[] INSIDE_ODD_TOTAL_SUBSET = new int[4]
    {
      4,
      20,
      48,
      81
    };
    private static readonly int[] OUTSIDE_GSUM = new int[5]
    {
      0,
      161,
      961,
      2015,
      2715
    };
    private static readonly int[] INSIDE_GSUM = new int[4]
    {
      0,
      336,
      1036,
      1516
    };
    private static readonly int[] OUTSIDE_ODD_WIDEST = new int[5]
    {
      8,
      6,
      4,
      3,
      1
    };
    private static readonly int[] INSIDE_ODD_WIDEST = new int[4]
    {
      2,
      4,
      6,
      8
    };
    private static readonly int[][] FINDER_PATTERNS = new int[9][]
    {
      new int[4]{ 3, 8, 2, 1 },
      new int[4]{ 3, 5, 5, 1 },
      new int[4]{ 3, 3, 7, 1 },
      new int[4]{ 3, 1, 9, 1 },
      new int[4]{ 2, 7, 4, 1 },
      new int[4]{ 2, 5, 6, 1 },
      new int[4]{ 2, 3, 8, 1 },
      new int[4]{ 1, 5, 7, 1 },
      new int[4]{ 1, 3, 9, 1 }
    };
    private readonly List<Pair> possibleLeftPairs;
    private readonly List<Pair> possibleRightPairs;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.RSS.RSS14Reader" /> class.
    /// </summary>
    public RSS14Reader()
    {
      this.possibleLeftPairs = new List<Pair>();
      this.possibleRightPairs = new List<Pair>();
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
      RSS14Reader.addOrTally((IList<Pair>) this.possibleLeftPairs, this.decodePair(row, false, rowNumber, hints));
      row.reverse();
      RSS14Reader.addOrTally((IList<Pair>) this.possibleRightPairs, this.decodePair(row, true, rowNumber, hints));
      row.reverse();
      int count1 = this.possibleLeftPairs.Count;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        Pair possibleLeftPair = this.possibleLeftPairs[index1];
        if (possibleLeftPair.Count > 1)
        {
          int count2 = this.possibleRightPairs.Count;
          for (int index2 = 0; index2 < count2; ++index2)
          {
            Pair possibleRightPair = this.possibleRightPairs[index2];
            if (possibleRightPair.Count > 1 && RSS14Reader.checkChecksum(possibleLeftPair, possibleRightPair))
              return RSS14Reader.constructResult(possibleLeftPair, possibleRightPair);
          }
        }
      }
      return (Result) null;
    }

    private static void addOrTally(IList<Pair> possiblePairs, Pair pair)
    {
      if (pair == null)
        return;
      bool flag = false;
      foreach (Pair possiblePair in (IEnumerable<Pair>) possiblePairs)
      {
        if (possiblePair.Value == pair.Value)
        {
          possiblePair.incrementCount();
          flag = true;
          break;
        }
      }
      if (flag)
        return;
      possiblePairs.Add(pair);
    }

    /// <summary>Resets this instance.</summary>
    public override void reset()
    {
      this.possibleLeftPairs.Clear();
      this.possibleRightPairs.Clear();
    }

    private static Result constructResult(Pair leftPair, Pair rightPair)
    {
      string str = (4537077L * (long) leftPair.Value + (long) rightPair.Value).ToString();
      StringBuilder stringBuilder = new StringBuilder(14);
      for (int index = 13 - str.Length; index > 0; --index)
        stringBuilder.Append('0');
      stringBuilder.Append(str);
      int num1 = 0;
      for (int index = 0; index < 13; ++index)
      {
        int num2 = (int) stringBuilder[index] - 48;
        num1 += (index & 1) == 0 ? 3 * num2 : num2;
      }
      int num3 = 10 - num1 % 10;
      if (num3 == 10)
        num3 = 0;
      stringBuilder.Append(num3);
      ResultPoint[] resultPoints1 = leftPair.FinderPattern.ResultPoints;
      ResultPoint[] resultPoints2 = rightPair.FinderPattern.ResultPoints;
      return new Result(stringBuilder.ToString(), (byte[]) null, new ResultPoint[4]
      {
        resultPoints1[0],
        resultPoints1[1],
        resultPoints2[0],
        resultPoints2[1]
      }, BarcodeFormat.RSS_14);
    }

    private static bool checkChecksum(Pair leftPair, Pair rightPair)
    {
      int num1 = (leftPair.ChecksumPortion + 16 * rightPair.ChecksumPortion) % 79;
      int num2 = 9 * leftPair.FinderPattern.Value + rightPair.FinderPattern.Value;
      if (num2 > 72)
        --num2;
      if (num2 > 8)
        --num2;
      return num1 == num2;
    }

    private Pair decodePair(
      BitArray row,
      bool right,
      int rowNumber,
      IDictionary<DecodeHintType, object> hints)
    {
      int[] finderPattern = this.findFinderPattern(row, 0, right);
      if (finderPattern == null)
        return (Pair) null;
      FinderPattern foundFinderPattern = this.parseFoundFinderPattern(row, rowNumber, right, finderPattern);
      if (foundFinderPattern == null)
        return (Pair) null;
      ResultPointCallback hint = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      if (hint != null)
      {
        float x = (float) (finderPattern[0] + finderPattern[1]) / 2f;
        if (right)
          x = (float) (row.Size - 1) - x;
        hint(new ResultPoint(x, (float) rowNumber));
      }
      DataCharacter dataCharacter1 = this.decodeDataCharacter(row, foundFinderPattern, true);
      if (dataCharacter1 == null)
        return (Pair) null;
      DataCharacter dataCharacter2 = this.decodeDataCharacter(row, foundFinderPattern, false);
      return dataCharacter2 == null ? (Pair) null : new Pair(1597 * dataCharacter1.Value + dataCharacter2.Value, dataCharacter1.ChecksumPortion + 4 * dataCharacter2.ChecksumPortion, foundFinderPattern);
    }

    private DataCharacter decodeDataCharacter(
      BitArray row,
      FinderPattern pattern,
      bool outsideChar)
    {
      int[] characterCounters = this.getDataCharacterCounters();
      characterCounters[0] = 0;
      characterCounters[1] = 0;
      characterCounters[2] = 0;
      characterCounters[3] = 0;
      characterCounters[4] = 0;
      characterCounters[5] = 0;
      characterCounters[6] = 0;
      characterCounters[7] = 0;
      if (outsideChar)
      {
        OneDReader.recordPatternInReverse(row, pattern.StartEnd[0], characterCounters);
      }
      else
      {
        OneDReader.recordPattern(row, pattern.StartEnd[1] + 1, characterCounters);
        int index1 = 0;
        for (int index2 = characterCounters.Length - 1; index1 < index2; --index2)
        {
          int num = characterCounters[index1];
          characterCounters[index1] = characterCounters[index2];
          characterCounters[index2] = num;
          ++index1;
        }
      }
      int numModules = outsideChar ? 16 : 15;
      float num1 = (float) AbstractRSSReader.count(characterCounters) / (float) numModules;
      int[] oddCounts = this.getOddCounts();
      int[] evenCounts = this.getEvenCounts();
      float[] oddRoundingErrors = this.getOddRoundingErrors();
      float[] evenRoundingErrors = this.getEvenRoundingErrors();
      for (int index3 = 0; index3 < characterCounters.Length; ++index3)
      {
        float num2 = (float) characterCounters[index3] / num1;
        int num3 = (int) ((double) num2 + 0.5);
        if (num3 < 1)
          num3 = 1;
        else if (num3 > 8)
          num3 = 8;
        int index4 = index3 >> 1;
        if ((index3 & 1) == 0)
        {
          oddCounts[index4] = num3;
          oddRoundingErrors[index4] = num2 - (float) num3;
        }
        else
        {
          evenCounts[index4] = num3;
          evenRoundingErrors[index4] = num2 - (float) num3;
        }
      }
      if (!this.adjustOddEvenCounts(outsideChar, numModules))
        return (DataCharacter) null;
      int num4 = 0;
      int num5 = 0;
      for (int index = oddCounts.Length - 1; index >= 0; --index)
      {
        num5 = num5 * 9 + oddCounts[index];
        num4 += oddCounts[index];
      }
      int num6 = 0;
      int num7 = 0;
      for (int index = evenCounts.Length - 1; index >= 0; --index)
      {
        num6 = num6 * 9 + evenCounts[index];
        num7 += evenCounts[index];
      }
      int checksumPortion = num5 + 3 * num6;
      if (outsideChar)
      {
        if ((num4 & 1) != 0 || num4 > 12 || num4 < 4)
          return (DataCharacter) null;
        int index = (12 - num4) / 2;
        int maxWidth1 = RSS14Reader.OUTSIDE_ODD_WIDEST[index];
        int maxWidth2 = 9 - maxWidth1;
        int rsSvalue1 = RSSUtils.getRSSvalue(oddCounts, maxWidth1, false);
        int rsSvalue2 = RSSUtils.getRSSvalue(evenCounts, maxWidth2, true);
        int num8 = RSS14Reader.OUTSIDE_EVEN_TOTAL_SUBSET[index];
        int num9 = RSS14Reader.OUTSIDE_GSUM[index];
        return new DataCharacter(rsSvalue1 * num8 + rsSvalue2 + num9, checksumPortion);
      }
      if ((num7 & 1) != 0 || num7 > 10 || num7 < 4)
        return (DataCharacter) null;
      int index5 = (10 - num7) / 2;
      int maxWidth3 = RSS14Reader.INSIDE_ODD_WIDEST[index5];
      int maxWidth4 = 9 - maxWidth3;
      int rsSvalue3 = RSSUtils.getRSSvalue(oddCounts, maxWidth3, true);
      int rsSvalue4 = RSSUtils.getRSSvalue(evenCounts, maxWidth4, false);
      int num10 = RSS14Reader.INSIDE_ODD_TOTAL_SUBSET[index5];
      int num11 = RSS14Reader.INSIDE_GSUM[index5];
      return new DataCharacter(rsSvalue4 * num10 + rsSvalue3 + num11, checksumPortion);
    }

    private int[] findFinderPattern(BitArray row, int rowOffset, bool rightFinderPattern)
    {
      int[] decodeFinderCounters = this.getDecodeFinderCounters();
      decodeFinderCounters[0] = 0;
      decodeFinderCounters[1] = 0;
      decodeFinderCounters[2] = 0;
      decodeFinderCounters[3] = 0;
      int size = row.Size;
      bool flag = false;
      for (; rowOffset < size; ++rowOffset)
      {
        flag = !row[rowOffset];
        if (rightFinderPattern == flag)
          break;
      }
      int index = 0;
      int num = rowOffset;
      for (int i = rowOffset; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++decodeFinderCounters[index];
        }
        else
        {
          if (index == 3)
          {
            if (AbstractRSSReader.isFinderPattern(decodeFinderCounters))
              return new int[2]{ num, i };
            num += decodeFinderCounters[0] + decodeFinderCounters[1];
            decodeFinderCounters[0] = decodeFinderCounters[2];
            decodeFinderCounters[1] = decodeFinderCounters[3];
            decodeFinderCounters[2] = 0;
            decodeFinderCounters[3] = 0;
            --index;
          }
          else
            ++index;
          decodeFinderCounters[index] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    private FinderPattern parseFoundFinderPattern(
      BitArray row,
      int rowNumber,
      bool right,
      int[] startEnd)
    {
      bool flag = row[startEnd[0]];
      int i = startEnd[0] - 1;
      while (i >= 0 && flag ^ row[i])
        --i;
      int num1 = i + 1;
      int num2 = startEnd[0] - num1;
      int[] decodeFinderCounters = this.getDecodeFinderCounters();
      Array.Copy((Array) decodeFinderCounters, 0, (Array) decodeFinderCounters, 1, decodeFinderCounters.Length - 1);
      decodeFinderCounters[0] = num2;
      int num3;
      if (!AbstractRSSReader.parseFinderValue(decodeFinderCounters, RSS14Reader.FINDER_PATTERNS, out num3))
        return (FinderPattern) null;
      int start = num1;
      int end = startEnd[1];
      if (right)
      {
        start = row.Size - 1 - start;
        end = row.Size - 1 - end;
      }
      return new FinderPattern(num3, new int[2]
      {
        num1,
        startEnd[1]
      }, start, end, rowNumber);
    }

    private bool adjustOddEvenCounts(bool outsideChar, int numModules)
    {
      int num1 = AbstractRSSReader.count(this.getOddCounts());
      int num2 = AbstractRSSReader.count(this.getEvenCounts());
      int num3 = num1 + num2 - numModules;
      bool flag1 = (num1 & 1) == (outsideChar ? 1 : 0);
      bool flag2 = (num2 & 1) == 1;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      bool flag6 = false;
      if (outsideChar)
      {
        if (num1 > 12)
          flag4 = true;
        else if (num1 < 4)
          flag3 = true;
        if (num2 > 12)
          flag6 = true;
        else if (num2 < 4)
          flag5 = true;
      }
      else
      {
        if (num1 > 11)
          flag4 = true;
        else if (num1 < 5)
          flag3 = true;
        if (num2 > 10)
          flag6 = true;
        else if (num2 < 4)
          flag5 = true;
      }
      switch (num3)
      {
        case -1:
          if (flag1)
          {
            if (flag2)
              return false;
            flag3 = true;
            break;
          }
          if (!flag2)
            return false;
          flag5 = true;
          break;
        case 0:
          if (flag1)
          {
            if (!flag2)
              return false;
            if (num1 < num2)
            {
              flag3 = true;
              flag6 = true;
              break;
            }
            flag4 = true;
            flag5 = true;
            break;
          }
          if (flag2)
            return false;
          break;
        case 1:
          if (flag1)
          {
            if (flag2)
              return false;
            flag4 = true;
            break;
          }
          if (!flag2)
            return false;
          flag6 = true;
          break;
        default:
          return false;
      }
      if (flag3)
      {
        if (flag4)
          return false;
        AbstractRSSReader.increment(this.getOddCounts(), this.getOddRoundingErrors());
      }
      if (flag4)
        AbstractRSSReader.decrement(this.getOddCounts(), this.getOddRoundingErrors());
      if (flag5)
      {
        if (flag6)
          return false;
        AbstractRSSReader.increment(this.getEvenCounts(), this.getOddRoundingErrors());
      }
      if (flag6)
        AbstractRSSReader.decrement(this.getEvenCounts(), this.getEvenRoundingErrors());
      return true;
    }
  }
}
