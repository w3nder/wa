// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.RSS.Expanded.RSSExpandedReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.OneD.RSS.Expanded.Decoders;

#nullable disable
namespace ZXing.OneD.RSS.Expanded
{
  /// <summary>
  /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
  /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
  /// </summary>
  public sealed class RSSExpandedReader : AbstractRSSReader
  {
    private const int FINDER_PAT_A = 0;
    private const int FINDER_PAT_B = 1;
    private const int FINDER_PAT_C = 2;
    private const int FINDER_PAT_D = 3;
    private const int FINDER_PAT_E = 4;
    private const int FINDER_PAT_F = 5;
    private const int MAX_PAIRS = 11;
    private static readonly int[] SYMBOL_WIDEST = new int[5]
    {
      7,
      5,
      4,
      3,
      1
    };
    private static readonly int[] EVEN_TOTAL_SUBSET = new int[5]
    {
      4,
      20,
      52,
      104,
      204
    };
    private static readonly int[] GSUM = new int[5]
    {
      0,
      348,
      1388,
      2948,
      3988
    };
    private static readonly int[][] FINDER_PATTERNS = new int[6][]
    {
      new int[4]{ 1, 8, 4, 1 },
      new int[4]{ 3, 6, 4, 1 },
      new int[4]{ 3, 4, 6, 1 },
      new int[4]{ 3, 2, 8, 1 },
      new int[4]{ 2, 6, 5, 1 },
      new int[4]{ 2, 2, 9, 1 }
    };
    private static readonly int[][] WEIGHTS = new int[23][]
    {
      new int[8]{ 1, 3, 9, 27, 81, 32, 96, 77 },
      new int[8]{ 20, 60, 180, 118, 143, 7, 21, 63 },
      new int[8]{ 189, 145, 13, 39, 117, 140, 209, 205 },
      new int[8]{ 193, 157, 49, 147, 19, 57, 171, 91 },
      new int[8]{ 62, 186, 136, 197, 169, 85, 44, 132 },
      new int[8]{ 185, 133, 188, 142, 4, 12, 36, 108 },
      new int[8]{ 113, 128, 173, 97, 80, 29, 87, 50 },
      new int[8]{ 150, 28, 84, 41, 123, 158, 52, 156 },
      new int[8]{ 46, 138, 203, 187, 139, 206, 196, 166 },
      new int[8]{ 76, 17, 51, 153, 37, 111, 122, 155 },
      new int[8]{ 43, 129, 176, 106, 107, 110, 119, 146 },
      new int[8]{ 16, 48, 144, 10, 30, 90, 59, 177 },
      new int[8]{ 109, 116, 137, 200, 178, 112, 125, 164 },
      new int[8]{ 70, 210, 208, 202, 184, 130, 179, 115 },
      new int[8]{ 134, 191, 151, 31, 93, 68, 204, 190 },
      new int[8]{ 148, 22, 66, 198, 172, 94, 71, 2 },
      new int[8]{ 6, 18, 54, 162, 64, 192, 154, 40 },
      new int[8]{ 120, 149, 25, 75, 14, 42, 126, 167 },
      new int[8]{ 79, 26, 78, 23, 69, 207, 199, 175 },
      new int[8]{ 103, 98, 83, 38, 114, 131, 182, 124 },
      new int[8]
      {
        161,
        61,
        183,
        (int) sbyte.MaxValue,
        170,
        88,
        53,
        159
      },
      new int[8]{ 55, 165, 73, 8, 24, 72, 5, 15 },
      new int[8]{ 45, 135, 194, 160, 58, 174, 100, 89 }
    };
    private static readonly int[][] FINDER_PATTERN_SEQUENCES = new int[10][]
    {
      new int[2],
      new int[3]{ 0, 1, 1 },
      new int[4]{ 0, 2, 1, 3 },
      new int[5]{ 0, 4, 1, 3, 2 },
      new int[6]{ 0, 4, 1, 3, 3, 5 },
      new int[7]{ 0, 4, 1, 3, 4, 5, 5 },
      new int[8]{ 0, 0, 1, 1, 2, 2, 3, 3 },
      new int[9]{ 0, 0, 1, 1, 2, 2, 3, 4, 4 },
      new int[10]{ 0, 0, 1, 1, 2, 2, 3, 4, 5, 5 },
      new int[11]{ 0, 0, 1, 1, 2, 3, 3, 4, 4, 5, 5 }
    };
    private readonly List<ExpandedPair> pairs = new List<ExpandedPair>(11);
    private readonly List<ExpandedRow> rows = new List<ExpandedRow>();
    private readonly int[] startEnd = new int[2];
    private bool startFromEven;

    internal List<ExpandedPair> Pairs => this.pairs;

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
      this.pairs.Clear();
      this.startFromEven = false;
      if (this.decodeRow2pairs(rowNumber, row))
        return RSSExpandedReader.constructResult(this.pairs);
      this.pairs.Clear();
      this.startFromEven = true;
      return this.decodeRow2pairs(rowNumber, row) ? RSSExpandedReader.constructResult(this.pairs) : (Result) null;
    }

    /// <summary>Resets this instance.</summary>
    public override void reset()
    {
      this.pairs.Clear();
      this.rows.Clear();
    }

    internal bool decodeRow2pairs(int rowNumber, BitArray row)
    {
      while (true)
      {
        ExpandedPair expandedPair = this.retrieveNextPair(row, this.pairs, rowNumber);
        if (expandedPair != null)
          this.pairs.Add(expandedPair);
        else
          break;
      }
      if (this.pairs.Count == 0)
        return false;
      if (this.checkChecksum())
        return true;
      bool flag = this.rows.Count != 0;
      bool wasReversed = false;
      this.storeRow(rowNumber, wasReversed);
      return flag && (this.checkRows(false) != null || this.checkRows(true) != null);
    }

    private List<ExpandedPair> checkRows(bool reverse)
    {
      if (this.rows.Count > 25)
      {
        this.rows.Clear();
        return (List<ExpandedPair>) null;
      }
      this.pairs.Clear();
      if (reverse)
        this.rows.Reverse();
      List<ExpandedPair> expandedPairList = this.checkRows(new List<ExpandedRow>(), 0);
      if (reverse)
        this.rows.Reverse();
      return expandedPairList;
    }

    private List<ExpandedPair> checkRows(List<ExpandedRow> collectedRows, int currentRow)
    {
      for (int index1 = currentRow; index1 < this.rows.Count; ++index1)
      {
        ExpandedRow row = this.rows[index1];
        this.pairs.Clear();
        int count = collectedRows.Count;
        for (int index2 = 0; index2 < count; ++index2)
          this.pairs.AddRange((IEnumerable<ExpandedPair>) collectedRows[index2].Pairs);
        this.pairs.AddRange((IEnumerable<ExpandedPair>) row.Pairs);
        if (RSSExpandedReader.isValidSequence(this.pairs))
        {
          if (this.checkChecksum())
            return this.pairs;
          List<ExpandedRow> collectedRows1 = new List<ExpandedRow>();
          collectedRows1.AddRange((IEnumerable<ExpandedRow>) collectedRows);
          collectedRows1.Add(row);
          List<ExpandedPair> expandedPairList = this.checkRows(collectedRows1, index1 + 1);
          if (expandedPairList != null)
            return expandedPairList;
        }
      }
      return (List<ExpandedPair>) null;
    }

    private static bool isValidSequence(List<ExpandedPair> pairs)
    {
      foreach (int[] numArray in RSSExpandedReader.FINDER_PATTERN_SEQUENCES)
      {
        if (pairs.Count <= numArray.Length)
        {
          bool flag = true;
          for (int index = 0; index < pairs.Count; ++index)
          {
            if (pairs[index].FinderPattern.Value != numArray[index])
            {
              flag = false;
              break;
            }
          }
          if (flag)
            return true;
        }
      }
      return false;
    }

    private void storeRow(int rowNumber, bool wasReversed)
    {
      int index = 0;
      bool flag1 = false;
      bool flag2 = false;
      for (; index < this.rows.Count; ++index)
      {
        ExpandedRow row = this.rows[index];
        if (row.RowNumber > rowNumber)
        {
          flag2 = row.IsEquivalent(this.pairs);
          break;
        }
        flag1 = row.IsEquivalent(this.pairs);
      }
      if (flag2 || flag1 || RSSExpandedReader.isPartialRow((IEnumerable<ExpandedPair>) this.pairs, (IEnumerable<ExpandedRow>) this.rows))
        return;
      this.rows.Insert(index, new ExpandedRow(this.pairs, rowNumber, wasReversed));
      RSSExpandedReader.removePartialRows(this.pairs, this.rows);
    }

    private static void removePartialRows(List<ExpandedPair> pairs, List<ExpandedRow> rows)
    {
      for (int index = 0; index < rows.Count; ++index)
      {
        ExpandedRow row = rows[index];
        if (row.Pairs.Count != pairs.Count)
        {
          bool flag1 = true;
          foreach (ExpandedPair pair1 in row.Pairs)
          {
            bool flag2 = false;
            foreach (ExpandedPair pair2 in pairs)
            {
              if (pair1.Equals((object) pair2))
              {
                flag2 = true;
                break;
              }
            }
            if (!flag2)
            {
              flag1 = false;
              break;
            }
          }
          if (flag1)
            rows.RemoveAt(index);
        }
      }
    }

    private static bool isPartialRow(IEnumerable<ExpandedPair> pairs, IEnumerable<ExpandedRow> rows)
    {
      foreach (ExpandedRow row in rows)
      {
        bool flag1 = true;
        foreach (ExpandedPair pair1 in pairs)
        {
          bool flag2 = false;
          foreach (ExpandedPair pair2 in row.Pairs)
          {
            if (pair1.Equals((object) pair2))
            {
              flag2 = true;
              break;
            }
          }
          if (!flag2)
          {
            flag1 = false;
            break;
          }
        }
        if (flag1)
          return true;
      }
      return false;
    }

    internal List<ExpandedRow> Rows => this.rows;

    internal static Result constructResult(List<ExpandedPair> pairs)
    {
      string information = AbstractExpandedDecoder.createDecoder(BitArrayBuilder.buildBitArray(pairs)).parseInformation();
      if (information == null)
        return (Result) null;
      ResultPoint[] resultPoints1 = pairs[0].FinderPattern.ResultPoints;
      ResultPoint[] resultPoints2 = pairs[pairs.Count - 1].FinderPattern.ResultPoints;
      return new Result(information, (byte[]) null, new ResultPoint[4]
      {
        resultPoints1[0],
        resultPoints1[1],
        resultPoints2[0],
        resultPoints2[1]
      }, BarcodeFormat.RSS_EXPANDED);
    }

    private bool checkChecksum()
    {
      ExpandedPair pair1 = this.pairs[0];
      DataCharacter leftChar = pair1.LeftChar;
      DataCharacter rightChar1 = pair1.RightChar;
      if (rightChar1 == null)
        return false;
      int checksumPortion = rightChar1.ChecksumPortion;
      int num1 = 2;
      for (int index = 1; index < this.pairs.Count; ++index)
      {
        ExpandedPair pair2 = this.pairs[index];
        checksumPortion += pair2.LeftChar.ChecksumPortion;
        ++num1;
        DataCharacter rightChar2 = pair2.RightChar;
        if (rightChar2 != null)
        {
          checksumPortion += rightChar2.ChecksumPortion;
          ++num1;
        }
      }
      int num2 = checksumPortion % 211;
      return 211 * (num1 - 4) + num2 == leftChar.Value;
    }

    private static int getNextSecondBar(BitArray row, int initialPos)
    {
      int nextSecondBar;
      if (row[initialPos])
      {
        int nextUnset = row.getNextUnset(initialPos);
        nextSecondBar = row.getNextSet(nextUnset);
      }
      else
      {
        int nextSet = row.getNextSet(initialPos);
        nextSecondBar = row.getNextUnset(nextSet);
      }
      return nextSecondBar;
    }

    internal ExpandedPair retrieveNextPair(
      BitArray row,
      List<ExpandedPair> previousPairs,
      int rowNumber)
    {
      bool flag1 = previousPairs.Count % 2 == 0;
      if (this.startFromEven)
        flag1 = !flag1;
      bool flag2 = true;
      int forcedOffset = -1;
      while (this.findNextPair(row, previousPairs, forcedOffset))
      {
        FinderPattern foundFinderPattern = this.parseFoundFinderPattern(row, rowNumber, flag1);
        if (foundFinderPattern == null)
          forcedOffset = RSSExpandedReader.getNextSecondBar(row, this.startEnd[0]);
        else
          flag2 = false;
        if (!flag2)
        {
          DataCharacter leftChar = this.decodeDataCharacter(row, foundFinderPattern, flag1, true);
          if (leftChar == null)
            return (ExpandedPair) null;
          if (previousPairs.Count != 0 && previousPairs[previousPairs.Count - 1].MustBeLast)
            return (ExpandedPair) null;
          DataCharacter rightChar = this.decodeDataCharacter(row, foundFinderPattern, flag1, false);
          return new ExpandedPair(leftChar, rightChar, foundFinderPattern, true);
        }
      }
      return (ExpandedPair) null;
    }

    private bool findNextPair(BitArray row, List<ExpandedPair> previousPairs, int forcedOffset)
    {
      int[] decodeFinderCounters = this.getDecodeFinderCounters();
      decodeFinderCounters[0] = 0;
      decodeFinderCounters[1] = 0;
      decodeFinderCounters[2] = 0;
      decodeFinderCounters[3] = 0;
      int size = row.Size;
      int i1 = forcedOffset < 0 ? (previousPairs.Count != 0 ? previousPairs[previousPairs.Count - 1].FinderPattern.StartEnd[1] : 0) : forcedOffset;
      bool flag1 = previousPairs.Count % 2 != 0;
      if (this.startFromEven)
        flag1 = !flag1;
      bool flag2 = false;
      for (; i1 < size; ++i1)
      {
        flag2 = !row[i1];
        if (!flag2)
          break;
      }
      int index = 0;
      int num = i1;
      for (int i2 = i1; i2 < size; ++i2)
      {
        if (row[i2] ^ flag2)
        {
          ++decodeFinderCounters[index];
        }
        else
        {
          if (index == 3)
          {
            if (flag1)
              RSSExpandedReader.reverseCounters(decodeFinderCounters);
            if (AbstractRSSReader.isFinderPattern(decodeFinderCounters))
            {
              this.startEnd[0] = num;
              this.startEnd[1] = i2;
              return true;
            }
            if (flag1)
              RSSExpandedReader.reverseCounters(decodeFinderCounters);
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
          flag2 = !flag2;
        }
      }
      return false;
    }

    private static void reverseCounters(int[] counters)
    {
      int length = counters.Length;
      for (int index = 0; index < length / 2; ++index)
      {
        int counter = counters[index];
        counters[index] = counters[length - index - 1];
        counters[length - index - 1] = counter;
      }
    }

    private FinderPattern parseFoundFinderPattern(BitArray row, int rowNumber, bool oddPattern)
    {
      int num1;
      int start;
      int nextUnset;
      if (oddPattern)
      {
        int i = this.startEnd[0] - 1;
        while (i >= 0 && !row[i])
          --i;
        int num2 = i + 1;
        num1 = this.startEnd[0] - num2;
        start = num2;
        nextUnset = this.startEnd[1];
      }
      else
      {
        start = this.startEnd[0];
        nextUnset = row.getNextUnset(this.startEnd[1] + 1);
        num1 = nextUnset - this.startEnd[1];
      }
      int[] decodeFinderCounters = this.getDecodeFinderCounters();
      Array.Copy((Array) decodeFinderCounters, 0, (Array) decodeFinderCounters, 1, decodeFinderCounters.Length - 1);
      decodeFinderCounters[0] = num1;
      int num3;
      if (!AbstractRSSReader.parseFinderValue(decodeFinderCounters, RSSExpandedReader.FINDER_PATTERNS, out num3))
        return (FinderPattern) null;
      return new FinderPattern(num3, new int[2]
      {
        start,
        nextUnset
      }, start, nextUnset, rowNumber);
    }

    internal DataCharacter decodeDataCharacter(
      BitArray row,
      FinderPattern pattern,
      bool isOddPattern,
      bool leftChar)
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
      if (leftChar)
      {
        if (!OneDReader.recordPatternInReverse(row, pattern.StartEnd[0], characterCounters))
          return (DataCharacter) null;
      }
      else
      {
        if (!OneDReader.recordPattern(row, pattern.StartEnd[1], characterCounters))
          return (DataCharacter) null;
        int index1 = 0;
        for (int index2 = characterCounters.Length - 1; index1 < index2; --index2)
        {
          int num = characterCounters[index1];
          characterCounters[index1] = characterCounters[index2];
          characterCounters[index2] = num;
          ++index1;
        }
      }
      float num1 = (float) AbstractRSSReader.count(characterCounters) / 17f;
      float num2 = (float) (pattern.StartEnd[1] - pattern.StartEnd[0]) / 15f;
      if ((double) Math.Abs(num1 - num2) / (double) num2 > 0.30000001192092896)
        return (DataCharacter) null;
      int[] oddCounts = this.getOddCounts();
      int[] evenCounts = this.getEvenCounts();
      float[] oddRoundingErrors = this.getOddRoundingErrors();
      float[] evenRoundingErrors = this.getEvenRoundingErrors();
      for (int index3 = 0; index3 < characterCounters.Length; ++index3)
      {
        float num3 = 1f * (float) characterCounters[index3] / num1;
        int num4 = (int) ((double) num3 + 0.5);
        if (num4 < 1)
        {
          if ((double) num3 < 0.30000001192092896)
            return (DataCharacter) null;
          num4 = 1;
        }
        else if (num4 > 8)
        {
          if ((double) num3 > 8.6999998092651367)
            return (DataCharacter) null;
          num4 = 8;
        }
        int index4 = index3 >> 1;
        if ((index3 & 1) == 0)
        {
          oddCounts[index4] = num4;
          oddRoundingErrors[index4] = num3 - (float) num4;
        }
        else
        {
          evenCounts[index4] = num4;
          evenRoundingErrors[index4] = num3 - (float) num4;
        }
      }
      if (!this.adjustOddEvenCounts(17))
        return (DataCharacter) null;
      int index5 = 4 * pattern.Value + (isOddPattern ? 0 : 2) + (leftChar ? 0 : 1) - 1;
      int num5 = 0;
      int num6 = 0;
      for (int index6 = oddCounts.Length - 1; index6 >= 0; --index6)
      {
        if (RSSExpandedReader.isNotA1left(pattern, isOddPattern, leftChar))
        {
          int num7 = RSSExpandedReader.WEIGHTS[index5][2 * index6];
          num6 += oddCounts[index6] * num7;
        }
        num5 += oddCounts[index6];
      }
      int num8 = 0;
      for (int index7 = evenCounts.Length - 1; index7 >= 0; --index7)
      {
        if (RSSExpandedReader.isNotA1left(pattern, isOddPattern, leftChar))
        {
          int num9 = RSSExpandedReader.WEIGHTS[index5][2 * index7 + 1];
          num8 += evenCounts[index7] * num9;
        }
      }
      int checksumPortion = num6 + num8;
      if ((num5 & 1) != 0 || num5 > 13 || num5 < 4)
        return (DataCharacter) null;
      int index8 = (13 - num5) / 2;
      int maxWidth1 = RSSExpandedReader.SYMBOL_WIDEST[index8];
      int maxWidth2 = 9 - maxWidth1;
      int rsSvalue1 = RSSUtils.getRSSvalue(oddCounts, maxWidth1, true);
      int rsSvalue2 = RSSUtils.getRSSvalue(evenCounts, maxWidth2, false);
      int num10 = RSSExpandedReader.EVEN_TOTAL_SUBSET[index8];
      int num11 = RSSExpandedReader.GSUM[index8];
      return new DataCharacter(rsSvalue1 * num10 + rsSvalue2 + num11, checksumPortion);
    }

    private static bool isNotA1left(FinderPattern pattern, bool isOddPattern, bool leftChar)
    {
      return pattern.Value != 0 || !isOddPattern || !leftChar;
    }

    private bool adjustOddEvenCounts(int numModules)
    {
      int num1 = AbstractRSSReader.count(this.getOddCounts());
      int num2 = AbstractRSSReader.count(this.getEvenCounts());
      int num3 = num1 + num2 - numModules;
      bool flag1 = (num1 & 1) == 1;
      bool flag2 = (num2 & 1) == 0;
      bool flag3 = false;
      bool flag4 = false;
      if (num1 > 13)
        flag4 = true;
      else if (num1 < 4)
        flag3 = true;
      bool flag5 = false;
      bool flag6 = false;
      if (num2 > 13)
        flag6 = true;
      else if (num2 < 4)
        flag5 = true;
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
