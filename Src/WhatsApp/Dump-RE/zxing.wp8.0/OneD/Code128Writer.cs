// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code128Writer
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
  /// This object renders a CODE128 code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// 
  /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
  /// </summary>
  public sealed class Code128Writer : OneDimensionalCodeWriter
  {
    private const int CODE_START_B = 104;
    private const int CODE_START_C = 105;
    private const int CODE_CODE_B = 100;
    private const int CODE_CODE_C = 99;
    private const int CODE_STOP = 106;
    private const char ESCAPE_FNC_1 = 'ñ';
    private const char ESCAPE_FNC_2 = 'ò';
    private const char ESCAPE_FNC_3 = 'ó';
    private const char ESCAPE_FNC_4 = 'ô';
    private const int CODE_FNC_1 = 102;
    private const int CODE_FNC_2 = 97;
    private const int CODE_FNC_3 = 96;
    private const int CODE_FNC_4_B = 100;
    private bool forceCodesetB;

    public override BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (format != BarcodeFormat.CODE_128)
        throw new ArgumentException("Can only encode CODE_128, but got " + (object) format);
      this.forceCodesetB = hints != null && hints.ContainsKey(EncodeHintType.CODE128_FORCE_CODESET_B) && (bool) hints[EncodeHintType.CODE128_FORCE_CODESET_B];
      return base.encode(contents, format, width, height, hints);
    }

    public override bool[] encode(string contents)
    {
      int length1 = contents.Length;
      if (length1 < 1 || length1 > 80)
        throw new ArgumentException("Contents length should be between 1 and 80 characters, but got " + (object) length1);
      for (int index = 0; index < length1; ++index)
      {
        char content = contents[index];
        if (content < ' ' || content > '~')
        {
          switch (content)
          {
            case 'ñ':
            case 'ò':
            case 'ó':
            case 'ô':
              continue;
            default:
              throw new ArgumentException("Bad character in input: " + (object) content);
          }
        }
      }
      List<int[]> numArrayList = new List<int[]>();
      int num1 = 0;
      int num2 = 1;
      int num3 = 0;
      int num4 = 0;
      while (num4 < length1)
      {
        int length2 = num3 == 99 ? 2 : 4;
        int num5 = !Code128Writer.isDigits(contents, num4, length2) ? 100 : (this.forceCodesetB ? 100 : 99);
        int index;
        if (num5 == num3)
        {
          switch (contents[num4])
          {
            case 'ñ':
              index = 102;
              break;
            case 'ò':
              index = 97;
              break;
            case 'ó':
              index = 96;
              break;
            case 'ô':
              index = 100;
              break;
            default:
              if (num3 == 100)
              {
                index = (int) contents[num4] - 32;
                break;
              }
              index = int.Parse(contents.Substring(num4, 2));
              ++num4;
              break;
          }
          ++num4;
        }
        else
        {
          index = num3 != 0 ? num5 : (num5 != 100 ? 105 : 104);
          num3 = num5;
        }
        numArrayList.Add(Code128Reader.CODE_PATTERNS[index]);
        num1 += index * num2;
        if (num4 != 0)
          ++num2;
      }
      int index1 = num1 % 103;
      numArrayList.Add(Code128Reader.CODE_PATTERNS[index1]);
      numArrayList.Add(Code128Reader.CODE_PATTERNS[106]);
      int length3 = 0;
      foreach (int[] numArray in numArrayList)
      {
        foreach (int num6 in numArray)
          length3 += num6;
      }
      bool[] target = new bool[length3];
      int pos = 0;
      foreach (int[] pattern in numArrayList)
        pos += OneDimensionalCodeWriter.appendPattern(target, pos, pattern, true);
      return target;
    }

    private static bool isDigits(string value, int start, int length)
    {
      int num = start + length;
      int length1 = value.Length;
      for (int index = start; index < num && index < length1; ++index)
      {
        char ch = value[index];
        if (ch < '0' || ch > '9')
        {
          if (ch != 'ñ')
            return false;
          ++num;
        }
      }
      return num <= length1;
    }
  }
}
