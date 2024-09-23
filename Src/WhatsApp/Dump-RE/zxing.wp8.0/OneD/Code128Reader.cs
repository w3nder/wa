// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code128Reader
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
  /// <p>Decodes Code 128 barcodes.</p>
  /// 
  /// <author>Sean Owen</author>
  /// </summary>
  public sealed class Code128Reader : OneDReader
  {
    private const int CODE_SHIFT = 98;
    private const int CODE_CODE_C = 99;
    private const int CODE_CODE_B = 100;
    private const int CODE_CODE_A = 101;
    private const int CODE_FNC_1 = 102;
    private const int CODE_FNC_2 = 97;
    private const int CODE_FNC_3 = 96;
    private const int CODE_FNC_4_A = 101;
    private const int CODE_FNC_4_B = 100;
    private const int CODE_START_A = 103;
    private const int CODE_START_B = 104;
    private const int CODE_START_C = 105;
    private const int CODE_STOP = 106;
    internal static int[][] CODE_PATTERNS = new int[107][]
    {
      new int[6]{ 2, 1, 2, 2, 2, 2 },
      new int[6]{ 2, 2, 2, 1, 2, 2 },
      new int[6]{ 2, 2, 2, 2, 2, 1 },
      new int[6]{ 1, 2, 1, 2, 2, 3 },
      new int[6]{ 1, 2, 1, 3, 2, 2 },
      new int[6]{ 1, 3, 1, 2, 2, 2 },
      new int[6]{ 1, 2, 2, 2, 1, 3 },
      new int[6]{ 1, 2, 2, 3, 1, 2 },
      new int[6]{ 1, 3, 2, 2, 1, 2 },
      new int[6]{ 2, 2, 1, 2, 1, 3 },
      new int[6]{ 2, 2, 1, 3, 1, 2 },
      new int[6]{ 2, 3, 1, 2, 1, 2 },
      new int[6]{ 1, 1, 2, 2, 3, 2 },
      new int[6]{ 1, 2, 2, 1, 3, 2 },
      new int[6]{ 1, 2, 2, 2, 3, 1 },
      new int[6]{ 1, 1, 3, 2, 2, 2 },
      new int[6]{ 1, 2, 3, 1, 2, 2 },
      new int[6]{ 1, 2, 3, 2, 2, 1 },
      new int[6]{ 2, 2, 3, 2, 1, 1 },
      new int[6]{ 2, 2, 1, 1, 3, 2 },
      new int[6]{ 2, 2, 1, 2, 3, 1 },
      new int[6]{ 2, 1, 3, 2, 1, 2 },
      new int[6]{ 2, 2, 3, 1, 1, 2 },
      new int[6]{ 3, 1, 2, 1, 3, 1 },
      new int[6]{ 3, 1, 1, 2, 2, 2 },
      new int[6]{ 3, 2, 1, 1, 2, 2 },
      new int[6]{ 3, 2, 1, 2, 2, 1 },
      new int[6]{ 3, 1, 2, 2, 1, 2 },
      new int[6]{ 3, 2, 2, 1, 1, 2 },
      new int[6]{ 3, 2, 2, 2, 1, 1 },
      new int[6]{ 2, 1, 2, 1, 2, 3 },
      new int[6]{ 2, 1, 2, 3, 2, 1 },
      new int[6]{ 2, 3, 2, 1, 2, 1 },
      new int[6]{ 1, 1, 1, 3, 2, 3 },
      new int[6]{ 1, 3, 1, 1, 2, 3 },
      new int[6]{ 1, 3, 1, 3, 2, 1 },
      new int[6]{ 1, 1, 2, 3, 1, 3 },
      new int[6]{ 1, 3, 2, 1, 1, 3 },
      new int[6]{ 1, 3, 2, 3, 1, 1 },
      new int[6]{ 2, 1, 1, 3, 1, 3 },
      new int[6]{ 2, 3, 1, 1, 1, 3 },
      new int[6]{ 2, 3, 1, 3, 1, 1 },
      new int[6]{ 1, 1, 2, 1, 3, 3 },
      new int[6]{ 1, 1, 2, 3, 3, 1 },
      new int[6]{ 1, 3, 2, 1, 3, 1 },
      new int[6]{ 1, 1, 3, 1, 2, 3 },
      new int[6]{ 1, 1, 3, 3, 2, 1 },
      new int[6]{ 1, 3, 3, 1, 2, 1 },
      new int[6]{ 3, 1, 3, 1, 2, 1 },
      new int[6]{ 2, 1, 1, 3, 3, 1 },
      new int[6]{ 2, 3, 1, 1, 3, 1 },
      new int[6]{ 2, 1, 3, 1, 1, 3 },
      new int[6]{ 2, 1, 3, 3, 1, 1 },
      new int[6]{ 2, 1, 3, 1, 3, 1 },
      new int[6]{ 3, 1, 1, 1, 2, 3 },
      new int[6]{ 3, 1, 1, 3, 2, 1 },
      new int[6]{ 3, 3, 1, 1, 2, 1 },
      new int[6]{ 3, 1, 2, 1, 1, 3 },
      new int[6]{ 3, 1, 2, 3, 1, 1 },
      new int[6]{ 3, 3, 2, 1, 1, 1 },
      new int[6]{ 3, 1, 4, 1, 1, 1 },
      new int[6]{ 2, 2, 1, 4, 1, 1 },
      new int[6]{ 4, 3, 1, 1, 1, 1 },
      new int[6]{ 1, 1, 1, 2, 2, 4 },
      new int[6]{ 1, 1, 1, 4, 2, 2 },
      new int[6]{ 1, 2, 1, 1, 2, 4 },
      new int[6]{ 1, 2, 1, 4, 2, 1 },
      new int[6]{ 1, 4, 1, 1, 2, 2 },
      new int[6]{ 1, 4, 1, 2, 2, 1 },
      new int[6]{ 1, 1, 2, 2, 1, 4 },
      new int[6]{ 1, 1, 2, 4, 1, 2 },
      new int[6]{ 1, 2, 2, 1, 1, 4 },
      new int[6]{ 1, 2, 2, 4, 1, 1 },
      new int[6]{ 1, 4, 2, 1, 1, 2 },
      new int[6]{ 1, 4, 2, 2, 1, 1 },
      new int[6]{ 2, 4, 1, 2, 1, 1 },
      new int[6]{ 2, 2, 1, 1, 1, 4 },
      new int[6]{ 4, 1, 3, 1, 1, 1 },
      new int[6]{ 2, 4, 1, 1, 1, 2 },
      new int[6]{ 1, 3, 4, 1, 1, 1 },
      new int[6]{ 1, 1, 1, 2, 4, 2 },
      new int[6]{ 1, 2, 1, 1, 4, 2 },
      new int[6]{ 1, 2, 1, 2, 4, 1 },
      new int[6]{ 1, 1, 4, 2, 1, 2 },
      new int[6]{ 1, 2, 4, 1, 1, 2 },
      new int[6]{ 1, 2, 4, 2, 1, 1 },
      new int[6]{ 4, 1, 1, 2, 1, 2 },
      new int[6]{ 4, 2, 1, 1, 1, 2 },
      new int[6]{ 4, 2, 1, 2, 1, 1 },
      new int[6]{ 2, 1, 2, 1, 4, 1 },
      new int[6]{ 2, 1, 4, 1, 2, 1 },
      new int[6]{ 4, 1, 2, 1, 2, 1 },
      new int[6]{ 1, 1, 1, 1, 4, 3 },
      new int[6]{ 1, 1, 1, 3, 4, 1 },
      new int[6]{ 1, 3, 1, 1, 4, 1 },
      new int[6]{ 1, 1, 4, 1, 1, 3 },
      new int[6]{ 1, 1, 4, 3, 1, 1 },
      new int[6]{ 4, 1, 1, 1, 1, 3 },
      new int[6]{ 4, 1, 1, 3, 1, 1 },
      new int[6]{ 1, 1, 3, 1, 4, 1 },
      new int[6]{ 1, 1, 4, 1, 3, 1 },
      new int[6]{ 3, 1, 1, 1, 4, 1 },
      new int[6]{ 4, 1, 1, 1, 3, 1 },
      new int[6]{ 2, 1, 1, 4, 1, 2 },
      new int[6]{ 2, 1, 1, 2, 1, 4 },
      new int[6]{ 2, 1, 1, 2, 3, 2 },
      new int[7]{ 2, 3, 3, 1, 1, 1, 2 }
    };
    private static readonly int MAX_AVG_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.25);
    private static readonly int MAX_INDIVIDUAL_VARIANCE = (int) ((double) OneDReader.PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.699999988079071);

    private static int[] findStartPattern(BitArray row)
    {
      int size = row.Size;
      int nextSet = row.getNextSet(0);
      int index1 = 0;
      int[] numArray = new int[6];
      int end = nextSet;
      bool flag = false;
      int length = numArray.Length;
      for (int i = nextSet; i < size; ++i)
      {
        if (row[i] ^ flag)
        {
          ++numArray[index1];
        }
        else
        {
          if (index1 == length - 1)
          {
            int num1 = Code128Reader.MAX_AVG_VARIANCE;
            int num2 = -1;
            for (int index2 = 103; index2 <= 105; ++index2)
            {
              int num3 = OneDReader.patternMatchVariance(numArray, Code128Reader.CODE_PATTERNS[index2], Code128Reader.MAX_INDIVIDUAL_VARIANCE);
              if (num3 < num1)
              {
                num1 = num3;
                num2 = index2;
              }
            }
            if (num2 >= 0 && row.isRange(Math.Max(0, end - (i - end) / 2), end, false))
              return new int[3]{ end, i, num2 };
            end += numArray[0] + numArray[1];
            Array.Copy((Array) numArray, 2, (Array) numArray, 0, length - 2);
            numArray[length - 2] = 0;
            numArray[length - 1] = 0;
            --index1;
          }
          else
            ++index1;
          numArray[index1] = 1;
          flag = !flag;
        }
      }
      return (int[]) null;
    }

    private static bool decodeCode(BitArray row, int[] counters, int rowOffset, out int code)
    {
      code = -1;
      if (!OneDReader.recordPattern(row, rowOffset, counters))
        return false;
      int num1 = Code128Reader.MAX_AVG_VARIANCE;
      for (int index = 0; index < Code128Reader.CODE_PATTERNS.Length; ++index)
      {
        int[] pattern = Code128Reader.CODE_PATTERNS[index];
        int num2 = OneDReader.patternMatchVariance(counters, pattern, Code128Reader.MAX_INDIVIDUAL_VARIANCE);
        if (num2 < num1)
        {
          num1 = num2;
          code = index;
        }
      }
      return code >= 0;
    }

    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      bool flag1 = hints != null && hints.ContainsKey(DecodeHintType.ASSUME_GS1);
      int[] startPattern = Code128Reader.findStartPattern(row);
      if (startPattern == null)
        return (Result) null;
      int num1 = startPattern[2];
      List<byte> byteList = new List<byte>(20);
      byteList.Add((byte) num1);
      int num2;
      switch (num1)
      {
        case 103:
          num2 = 101;
          break;
        case 104:
          num2 = 100;
          break;
        case 105:
          num2 = 99;
          break;
        default:
          return (Result) null;
      }
      bool flag2 = false;
      bool flag3 = false;
      StringBuilder stringBuilder = new StringBuilder(20);
      int num3 = startPattern[0];
      int num4 = startPattern[1];
      int[] counters = new int[6];
      int num5 = 0;
      int code = 0;
      int num6 = num1;
      int num7 = 0;
      bool flag4 = true;
      bool flag5 = false;
      bool flag6 = false;
      while (!flag2)
      {
        bool flag7 = flag3;
        flag3 = false;
        num5 = code;
        if (!Code128Reader.decodeCode(row, counters, num4, out code))
          return (Result) null;
        byteList.Add((byte) code);
        if (code != 106)
          flag4 = true;
        if (code != 106)
        {
          ++num7;
          num6 += num7 * code;
        }
        num3 = num4;
        foreach (int num8 in counters)
          num4 += num8;
        switch (code)
        {
          case 103:
          case 104:
          case 105:
            return (Result) null;
          default:
            switch (num2)
            {
              case 99:
                if (code < 100)
                {
                  if (code < 10)
                    stringBuilder.Append('0');
                  stringBuilder.Append(code);
                  break;
                }
                if (code != 106)
                  flag4 = false;
                switch (code - 100)
                {
                  case 0:
                    num2 = 100;
                    break;
                  case 1:
                    num2 = 101;
                    break;
                  case 2:
                    if (flag1)
                    {
                      if (stringBuilder.Length == 0)
                      {
                        stringBuilder.Append("]C1");
                        break;
                      }
                      stringBuilder.Append('\u001D');
                      break;
                    }
                    break;
                  case 6:
                    flag2 = true;
                    break;
                }
                break;
              case 100:
                if (code < 96)
                {
                  if (flag6 == flag5)
                    stringBuilder.Append((char) (32 + code));
                  else
                    stringBuilder.Append((char) (32 + code + 128));
                  flag6 = false;
                  break;
                }
                if (code != 106)
                  flag4 = false;
                switch (code - 96)
                {
                  case 2:
                    flag3 = true;
                    num2 = 101;
                    break;
                  case 3:
                    num2 = 99;
                    break;
                  case 4:
                    if (!flag5 && flag6)
                    {
                      flag5 = true;
                      flag6 = false;
                      break;
                    }
                    if (flag5 && flag6)
                    {
                      flag5 = false;
                      flag6 = false;
                      break;
                    }
                    flag6 = true;
                    break;
                  case 5:
                    num2 = 101;
                    break;
                  case 6:
                    if (flag1)
                    {
                      if (stringBuilder.Length == 0)
                      {
                        stringBuilder.Append("]C1");
                        break;
                      }
                      stringBuilder.Append('\u001D');
                      break;
                    }
                    break;
                  case 10:
                    flag2 = true;
                    break;
                }
                break;
              case 101:
                if (code < 64)
                {
                  if (flag6 == flag5)
                    stringBuilder.Append((char) (32 + code));
                  else
                    stringBuilder.Append((char) (32 + code + 128));
                  flag6 = false;
                  break;
                }
                if (code < 96)
                {
                  if (flag6 == flag5)
                    stringBuilder.Append((char) (code - 64));
                  else
                    stringBuilder.Append((char) (code + 64));
                  flag6 = false;
                  break;
                }
                if (code != 106)
                  flag4 = false;
                switch (code - 96)
                {
                  case 2:
                    flag3 = true;
                    num2 = 100;
                    break;
                  case 3:
                    num2 = 99;
                    break;
                  case 4:
                    num2 = 100;
                    break;
                  case 5:
                    if (!flag5 && flag6)
                    {
                      flag5 = true;
                      flag6 = false;
                      break;
                    }
                    if (flag5 && flag6)
                    {
                      flag5 = false;
                      flag6 = false;
                      break;
                    }
                    flag6 = true;
                    break;
                  case 6:
                    if (flag1)
                    {
                      if (stringBuilder.Length == 0)
                      {
                        stringBuilder.Append("]C1");
                        break;
                      }
                      stringBuilder.Append('\u001D');
                      break;
                    }
                    break;
                  case 10:
                    flag2 = true;
                    break;
                }
                break;
            }
            if (flag7)
            {
              num2 = num2 == 101 ? 100 : 101;
              continue;
            }
            continue;
        }
      }
      int num9 = num4 - num3;
      int nextUnset = row.getNextUnset(num4);
      if (!row.isRange(nextUnset, Math.Min(row.Size, nextUnset + (nextUnset - num3) / 2), false))
        return (Result) null;
      if ((num6 - num7 * num5) % 103 != num5)
        return (Result) null;
      int length = stringBuilder.Length;
      if (length == 0)
        return (Result) null;
      if (length > 0 && flag4)
      {
        if (num2 == 99)
          stringBuilder.Remove(length - 2, 2);
        else
          stringBuilder.Remove(length - 1, 1);
      }
      float x1 = (float) (startPattern[1] + startPattern[0]) / 2f;
      float x2 = (float) num3 + (float) num9 / 2f;
      ResultPointCallback hint = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
      if (hint != null)
      {
        hint(new ResultPoint(x1, (float) rowNumber));
        hint(new ResultPoint(x2, (float) rowNumber));
      }
      int count = byteList.Count;
      byte[] rawBytes = new byte[count];
      for (int index = 0; index < count; ++index)
        rawBytes[index] = byteList[index];
      return new Result(stringBuilder.ToString(), rawBytes, new ResultPoint[2]
      {
        new ResultPoint(x1, (float) rowNumber),
        new ResultPoint(x2, (float) rowNumber)
      }, BarcodeFormat.CODE_128);
    }
  }
}
