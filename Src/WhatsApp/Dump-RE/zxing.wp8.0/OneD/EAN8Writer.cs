// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.EAN8Writer
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
  /// This object renders an EAN8 code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// <author>aripollak@gmail.com (Ari Pollak)</author>
  /// </summary>
  public sealed class EAN8Writer : UPCEANWriter
  {
    private const int CODE_WIDTH = 67;

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
      if (format != BarcodeFormat.EAN_8)
        throw new ArgumentException("Can only encode EAN_8, but got " + (object) format);
      return base.encode(contents, format, width, height, hints);
    }

    /// <summary>
    /// </summary>
    /// <returns>
    /// a byte array of horizontal pixels (false = white, true = black)
    /// </returns>
    public override bool[] encode(string contents)
    {
      if (contents.Length < 7 || contents.Length > 8)
        throw new ArgumentException("Requested contents should be 7 (without checksum digit) or 8 digits long, but got " + (object) contents.Length);
      foreach (char content in contents)
      {
        if (!char.IsDigit(content))
          throw new ArgumentException("Requested contents should only contain digits, but got '" + (object) content + "'");
      }
      if (contents.Length == 7)
        contents = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(contents);
      bool[] target = new bool[67];
      int pos1 = 0;
      int pos2 = pos1 + OneDimensionalCodeWriter.appendPattern(target, pos1, UPCEANReader.START_END_PATTERN, true);
      for (int startIndex = 0; startIndex <= 3; ++startIndex)
      {
        int index = int.Parse(contents.Substring(startIndex, 1));
        pos2 += OneDimensionalCodeWriter.appendPattern(target, pos2, UPCEANReader.L_PATTERNS[index], false);
      }
      int pos3 = pos2 + OneDimensionalCodeWriter.appendPattern(target, pos2, UPCEANReader.MIDDLE_PATTERN, false);
      for (int startIndex = 4; startIndex <= 7; ++startIndex)
      {
        int index = int.Parse(contents.Substring(startIndex, 1));
        pos3 += OneDimensionalCodeWriter.appendPattern(target, pos3, UPCEANReader.L_PATTERNS[index], true);
      }
      OneDimensionalCodeWriter.appendPattern(target, pos3, UPCEANReader.START_END_PATTERN, true);
      return target;
    }
  }
}
