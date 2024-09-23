// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.ITFWriter
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
  /// This object renders a ITF code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// 
  /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
  /// </summary>
  public sealed class ITFWriter : OneDimensionalCodeWriter
  {
    private static readonly int[] START_PATTERN = new int[4]
    {
      1,
      1,
      1,
      1
    };
    private static readonly int[] END_PATTERN = new int[3]
    {
      3,
      1,
      1
    };

    /// <summary>
    /// Encode the contents following specified format.
    /// {@code width} and {@code height} are required size. This method may return bigger size
    /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
    /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
    /// or {@code height}, {@code IllegalArgumentException} is thrown.
    /// </summary>
    /// <param name="contents"></param>
    /// <param name="format"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="hints"></param>
    /// <returns></returns>
    public override BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (format != BarcodeFormat.ITF)
        throw new ArgumentException("Can only encode ITF, but got " + (object) format);
      return base.encode(contents, format, width, height, hints);
    }

    /// <summary>
    /// Encode the contents to bool array expression of one-dimensional barcode.
    /// Start code and end code should be included in result, and side margins should not be included.
    /// <returns>a {@code bool[]} of horizontal pixels (false = white, true = black)</returns>
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public override bool[] encode(string contents)
    {
      int length = contents.Length;
      if (length % 2 != 0)
        throw new ArgumentException("The lenght of the input should be even");
      if (length > 80)
        throw new ArgumentException("Requested contents should be less than 80 digits long, but got " + (object) length);
      for (int index = 0; index < length; ++index)
      {
        if (!char.IsDigit(contents[index]))
          throw new ArgumentException("Requested contents should only contain digits, but got '" + (object) contents[index] + "'");
      }
      bool[] target = new bool[9 + 9 * length];
      int pos = OneDimensionalCodeWriter.appendPattern(target, 0, ITFWriter.START_PATTERN, true);
      for (int index1 = 0; index1 < length; index1 += 2)
      {
        int int32_1 = Convert.ToInt32(contents[index1].ToString(), 10);
        int int32_2 = Convert.ToInt32(contents[index1 + 1].ToString(), 10);
        int[] pattern = new int[18];
        for (int index2 = 0; index2 < 5; ++index2)
        {
          pattern[index2 << 1] = ITFReader.PATTERNS[int32_1][index2];
          pattern[(index2 << 1) + 1] = ITFReader.PATTERNS[int32_2][index2];
        }
        pos += OneDimensionalCodeWriter.appendPattern(target, pos, pattern, true);
      }
      OneDimensionalCodeWriter.appendPattern(target, pos, ITFWriter.END_PATTERN, true);
      return target;
    }
  }
}
