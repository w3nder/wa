// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.MSIWriter
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
  /// This object renders a MSI code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// </summary>
  public sealed class MSIWriter : OneDimensionalCodeWriter
  {
    private static readonly int[] startWidths = new int[2]
    {
      2,
      1
    };
    private static readonly int[] endWidths = new int[3]
    {
      1,
      2,
      1
    };
    private static readonly int[][] numberWidths = new int[10][]
    {
      new int[8]{ 1, 2, 1, 2, 1, 2, 1, 2 },
      new int[8]{ 1, 2, 1, 2, 1, 2, 2, 1 },
      new int[8]{ 1, 2, 1, 2, 2, 1, 1, 2 },
      new int[8]{ 1, 2, 1, 2, 2, 1, 2, 1 },
      new int[8]{ 1, 2, 2, 1, 1, 2, 1, 2 },
      new int[8]{ 1, 2, 2, 1, 1, 2, 2, 1 },
      new int[8]{ 1, 2, 2, 1, 2, 1, 1, 2 },
      new int[8]{ 1, 2, 2, 1, 2, 1, 2, 1 },
      new int[8]{ 2, 1, 1, 2, 1, 2, 1, 2 },
      new int[8]{ 2, 1, 1, 2, 1, 2, 2, 1 }
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
      if (format != BarcodeFormat.MSI)
        throw new ArgumentException("Can only encode MSI, but got " + (object) format);
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
      int length = contents.Length;
      for (int index = 0; index < length; ++index)
      {
        if (MSIReader.ALPHABET_STRING.IndexOf(contents[index]) < 0)
          throw new ArgumentException("Requested contents contains a not encodable character: '" + (object) contents[index] + "'");
      }
      bool[] target = new bool[3 + length * 12 + 4];
      int pos = OneDimensionalCodeWriter.appendPattern(target, 0, MSIWriter.startWidths, true);
      for (int index1 = 0; index1 < length; ++index1)
      {
        int index2 = MSIReader.ALPHABET_STRING.IndexOf(contents[index1]);
        int[] numberWidth = MSIWriter.numberWidths[index2];
        pos += OneDimensionalCodeWriter.appendPattern(target, pos, numberWidth, true);
      }
      OneDimensionalCodeWriter.appendPattern(target, pos, MSIWriter.endWidths, true);
      return target;
    }
  }
}
