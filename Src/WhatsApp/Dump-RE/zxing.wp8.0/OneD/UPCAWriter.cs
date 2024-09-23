// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCAWriter
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
  /// This object renders a UPC-A code as a <see cref="T:ZXing.Common.BitMatrix" />.
  /// <author>qwandor@google.com (Andrew Walbran)</author>
  /// </summary>
  public class UPCAWriter : Writer
  {
    private readonly EAN13Writer subWriter = new EAN13Writer();

    /// <summary>Encode a barcode using the default settings.</summary>
    /// <param name="contents">The contents to encode in the barcode</param>
    /// <param name="format">The barcode format to generate</param>
    /// <param name="width">The preferred width in pixels</param>
    /// <param name="height">The preferred height in pixels</param>
    /// <returns>
    /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
    /// </returns>
    public BitMatrix encode(string contents, BarcodeFormat format, int width, int height)
    {
      return this.encode(contents, format, width, height, (IDictionary<EncodeHintType, object>) null);
    }

    /// <summary>
    /// </summary>
    /// <param name="contents">The contents to encode in the barcode</param>
    /// <param name="format">The barcode format to generate</param>
    /// <param name="width">The preferred width in pixels</param>
    /// <param name="height">The preferred height in pixels</param>
    /// <param name="hints">Additional parameters to supply to the encoder</param>
    /// <returns>
    /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
    /// </returns>
    public BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (format != BarcodeFormat.UPC_A)
        throw new ArgumentException("Can only encode UPC-A, but got " + (object) format);
      return this.subWriter.encode(UPCAWriter.preencode(contents), BarcodeFormat.EAN_13, width, height, hints);
    }

    /// <summary>
    /// Transform a UPC-A code into the equivalent EAN-13 code, and add a check digit if it is not
    /// already present.
    /// </summary>
    private static string preencode(string contents)
    {
      switch (contents.Length)
      {
        case 11:
          int num = 0;
          for (int index = 0; index < 11; ++index)
            num += ((int) contents[index] - 48) * (index % 2 == 0 ? 3 : 1);
          contents += (string) (object) ((1000 - num) % 10);
          goto case 12;
        case 12:
          return '0'.ToString() + contents;
        default:
          throw new ArgumentException("Requested contents should be 11 or 12 digits long, but got " + (object) contents.Length);
      }
    }
  }
}
