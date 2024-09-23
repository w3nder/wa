// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.EAN13Writer
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
  /// This object renders an EAN13 code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// <author>aripollak@gmail.com (Ari Pollak)</author>
  /// </summary>
  public sealed class EAN13Writer : UPCEANWriter
  {
    private const int CODE_WIDTH = 95;

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
      if (format != BarcodeFormat.EAN_13)
        throw new ArgumentException("Can only encode EAN_13, but got " + (object) format);
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
      if (contents.Length < 12 || contents.Length > 13)
        throw new ArgumentException("Requested contents should be 12 (without checksum digit) or 13 digits long, but got " + (object) contents.Length);
      foreach (char content in contents)
      {
        if (!char.IsDigit(content))
          throw new ArgumentException("Requested contents should only contain digits, but got '" + (object) content + "'");
      }
      if (contents.Length == 12)
        contents = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(contents);
      else if (!UPCEANReader.checkStandardUPCEANChecksum(contents))
        throw new ArgumentException("Contents do not pass checksum");
      int index1 = int.Parse(contents.Substring(0, 1));
      int num = EAN13Reader.FIRST_DIGIT_ENCODINGS[index1];
      bool[] target = new bool[95];
      int pos1 = 0;
      int pos2 = pos1 + OneDimensionalCodeWriter.appendPattern(target, pos1, UPCEANReader.START_END_PATTERN, true);
      for (int startIndex = 1; startIndex <= 6; ++startIndex)
      {
        int index2 = int.Parse(contents.Substring(startIndex, 1));
        if ((num >> 6 - startIndex & 1) == 1)
          index2 += 10;
        pos2 += OneDimensionalCodeWriter.appendPattern(target, pos2, UPCEANReader.L_AND_G_PATTERNS[index2], false);
      }
      int pos3 = pos2 + OneDimensionalCodeWriter.appendPattern(target, pos2, UPCEANReader.MIDDLE_PATTERN, false);
      for (int startIndex = 7; startIndex <= 12; ++startIndex)
      {
        int index3 = int.Parse(contents.Substring(startIndex, 1));
        pos3 += OneDimensionalCodeWriter.appendPattern(target, pos3, UPCEANReader.L_PATTERNS[index3], true);
      }
      OneDimensionalCodeWriter.appendPattern(target, pos3, UPCEANReader.START_END_PATTERN, true);
      return target;
    }
  }
}
