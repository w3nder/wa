// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code39Writer
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
  /// This object renders a CODE39 code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// <author>erik.barbara@gmail.com (Erik Barbara)</author>
  /// </summary>
  public sealed class Code39Writer : OneDimensionalCodeWriter
  {
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
      if (format != BarcodeFormat.CODE_39)
        throw new ArgumentException("Can only encode CODE_39, but got " + (object) format);
      return base.encode(contents, format, width, height, hints);
    }

    /// <summary>
    /// Encode the contents to byte array expression of one-dimensional barcode.
    /// Start code and end code should be included in result, and side margins should not be included.
    /// <returns>a {@code boolean[]} of horizontal pixels (false = white, true = black)</returns>
    /// </summary>
    /// <param name="contents"></param>
    /// <returns></returns>
    public override bool[] encode(string contents)
    {
      int length1 = contents.Length;
      if (length1 > 80)
        throw new ArgumentException("Requested contents should be less than 80 digits long, but got " + (object) length1);
      for (int index = 0; index < length1; ++index)
      {
        if (Code39Reader.ALPHABET_STRING.IndexOf(contents[index]) < 0)
          throw new ArgumentException("Requested contents contains a not encodable character: '" + (object) contents[index] + "'");
      }
      int[] numArray = new int[9];
      int length2 = 25 + length1;
      for (int index1 = 0; index1 < length1; ++index1)
      {
        int index2 = Code39Reader.ALPHABET_STRING.IndexOf(contents[index1]);
        if (index2 < 0)
          throw new ArgumentException("Bad contents: " + contents);
        Code39Writer.toIntArray(Code39Reader.CHARACTER_ENCODINGS[index2], numArray);
        foreach (int num in numArray)
          length2 += num;
      }
      bool[] target = new bool[length2];
      Code39Writer.toIntArray(Code39Reader.CHARACTER_ENCODINGS[39], numArray);
      int pos1 = OneDimensionalCodeWriter.appendPattern(target, 0, numArray, true);
      int[] pattern = new int[1]{ 1 };
      int pos2 = pos1 + OneDimensionalCodeWriter.appendPattern(target, pos1, pattern, false);
      for (int index3 = 0; index3 < length1; ++index3)
      {
        int index4 = Code39Reader.ALPHABET_STRING.IndexOf(contents[index3]);
        Code39Writer.toIntArray(Code39Reader.CHARACTER_ENCODINGS[index4], numArray);
        int pos3 = pos2 + OneDimensionalCodeWriter.appendPattern(target, pos2, numArray, true);
        pos2 = pos3 + OneDimensionalCodeWriter.appendPattern(target, pos3, pattern, false);
      }
      Code39Writer.toIntArray(Code39Reader.CHARACTER_ENCODINGS[39], numArray);
      OneDimensionalCodeWriter.appendPattern(target, pos2, numArray, true);
      return target;
    }

    private static void toIntArray(int a, int[] toReturn)
    {
      for (int index = 0; index < 9; ++index)
      {
        int num = a & 1 << 8 - index;
        toReturn[index] = num == 0 ? 1 : 2;
      }
    }
  }
}
