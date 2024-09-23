// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.QRCodeWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.QrCode
{
  /// <summary>
  /// This object renders a QR Code as a BitMatrix 2D array of greyscale values.
  /// 
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// </summary>
  public sealed class QRCodeWriter : Writer
  {
    private const int QUIET_ZONE_SIZE = 4;

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
      if (string.IsNullOrEmpty(contents))
        throw new ArgumentException("Found empty contents");
      if (format != BarcodeFormat.QR_CODE)
        throw new ArgumentException("Can only encode QR_CODE, but got " + (object) format);
      if (width < 0 || height < 0)
        throw new ArgumentException("Requested dimensions are too small: " + (object) width + (object) 'x' + (object) height);
      ErrorCorrectionLevel ecLevel = ErrorCorrectionLevel.L;
      int quietZone = 4;
      if (hints != null)
      {
        ErrorCorrectionLevel hint = hints.ContainsKey(EncodeHintType.ERROR_CORRECTION) ? (ErrorCorrectionLevel) hints[EncodeHintType.ERROR_CORRECTION] : (ErrorCorrectionLevel) null;
        if (hint != null)
          ecLevel = hint;
        int? nullable = hints.ContainsKey(EncodeHintType.MARGIN) ? new int?((int) hints[EncodeHintType.MARGIN]) : new int?();
        if (nullable.HasValue)
          quietZone = nullable.Value;
      }
      return QRCodeWriter.renderResult(Encoder.encode(contents, ecLevel, hints), width, height, quietZone);
    }

    private static BitMatrix renderResult(QRCode code, int width, int height, int quietZone)
    {
      ByteMatrix matrix = code.Matrix;
      int num1 = matrix != null ? matrix.Width : throw new InvalidOperationException();
      int height1 = matrix.Height;
      int val2_1 = num1 + (quietZone << 1);
      int val2_2 = height1 + (quietZone << 1);
      int width1 = Math.Max(width, val2_1);
      int height2 = Math.Max(height, val2_2);
      int num2 = Math.Min(width1 / val2_1, height2 / val2_2);
      int num3 = (width1 - num1 * num2) / 2;
      int num4 = (height2 - height1 * num2) / 2;
      BitMatrix bitMatrix = new BitMatrix(width1, height2);
      int y = 0;
      int top = num4;
      while (y < height1)
      {
        int x = 0;
        int left = num3;
        while (x < num1)
        {
          if (matrix[x, y] == 1)
            bitMatrix.setRegion(left, top, num2, num2);
          ++x;
          left += num2;
        }
        ++y;
        top += num2;
      }
      return bitMatrix;
    }
  }
}
