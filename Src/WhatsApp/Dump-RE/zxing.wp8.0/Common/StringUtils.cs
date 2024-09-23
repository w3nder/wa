// Decompiled with JetBrains decompiler
// Type: ZXing.Common.StringUtils
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;

#nullable disable
namespace ZXing.Common
{
  /// <summary>Common string-related functions.</summary>
  /// <author>Sean Owen</author>
  /// <author>Alex Dupre</author>
  public static class StringUtils
  {
    private const string PLATFORM_DEFAULT_ENCODING = "UTF-8";
    private const string EUC_JP = "EUC-JP";
    private const string UTF8 = "UTF-8";
    private const string ISO88591 = "ISO-8859-1";
    public static string SHIFT_JIS = "SJIS";
    public static string GB2312 = nameof (GB2312);
    private static readonly bool ASSUME_SHIFT_JIS = string.Compare(StringUtils.SHIFT_JIS, "UTF-8", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare("EUC-JP", "UTF-8", StringComparison.OrdinalIgnoreCase) == 0;

    /// <summary>Guesses the encoding.</summary>
    /// <param name="bytes">bytes encoding a string, whose encoding should be guessed</param>
    /// <param name="hints">decode hints if applicable</param>
    /// <returns>name of guessed encoding; at the moment will only guess one of:
    /// {@link #SHIFT_JIS}, {@link #UTF8}, {@link #ISO88591}, or the platform
    /// default encoding if none of these can possibly be correct</returns>
    public static string guessEncoding(byte[] bytes, IDictionary<DecodeHintType, object> hints)
    {
      if (hints != null && hints.ContainsKey(DecodeHintType.CHARACTER_SET))
      {
        string hint = (string) hints[DecodeHintType.CHARACTER_SET];
        if (hint != null)
          return hint;
      }
      int length = bytes.Length;
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = true;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      int num8 = 0;
      int num9 = 0;
      int num10 = 0;
      int num11 = 0;
      bool flag4 = bytes.Length > 3 && bytes[0] == (byte) 239 && bytes[1] == (byte) 187 && bytes[2] == (byte) 191;
      for (int index = 0; index < length && (flag1 || flag2 || flag3); ++index)
      {
        int num12 = (int) bytes[index] & (int) byte.MaxValue;
        if (flag3)
        {
          if (num1 > 0)
          {
            if ((num12 & 128) == 0)
              flag3 = false;
            else
              --num1;
          }
          else if ((num12 & 128) != 0)
          {
            if ((num12 & 64) == 0)
            {
              flag3 = false;
            }
            else
            {
              ++num1;
              if ((num12 & 32) == 0)
              {
                ++num2;
              }
              else
              {
                ++num1;
                if ((num12 & 16) == 0)
                {
                  ++num3;
                }
                else
                {
                  ++num1;
                  if ((num12 & 8) == 0)
                    ++num4;
                  else
                    flag3 = false;
                }
              }
            }
          }
        }
        if (flag1)
        {
          if (num12 > (int) sbyte.MaxValue && num12 < 160)
            flag1 = false;
          else if (num12 > 159 && (num12 < 192 || num12 == 215 || num12 == 247))
            ++num11;
        }
        if (flag2)
        {
          if (num5 > 0)
          {
            if (num12 < 64 || num12 == (int) sbyte.MaxValue || num12 > 252)
              flag2 = false;
            else
              --num5;
          }
          else if (num12 == 128 || num12 == 160 || num12 > 239)
            flag2 = false;
          else if (num12 > 160 && num12 < 224)
          {
            ++num6;
            num8 = 0;
            ++num7;
            if (num7 > num9)
              num9 = num7;
          }
          else if (num12 > (int) sbyte.MaxValue)
          {
            ++num5;
            num7 = 0;
            ++num8;
            if (num8 > num10)
              num10 = num8;
          }
          else
          {
            num7 = 0;
            num8 = 0;
          }
        }
      }
      if (flag3 && num1 > 0)
        flag3 = false;
      if (flag2 && num5 > 0)
        flag2 = false;
      if (flag3 && (flag4 || num2 + num3 + num4 > 0))
        return "UTF-8";
      if (flag2 && (StringUtils.ASSUME_SHIFT_JIS || num9 >= 3 || num10 >= 3))
        return StringUtils.SHIFT_JIS;
      if (flag1 && flag2)
        return (num9 != 2 || num6 != 2) && num11 * 10 < length ? "ISO-8859-1" : StringUtils.SHIFT_JIS;
      if (flag1)
        return "ISO-8859-1";
      if (flag2)
        return StringUtils.SHIFT_JIS;
      return flag3 ? "UTF-8" : "UTF-8";
    }
  }
}
