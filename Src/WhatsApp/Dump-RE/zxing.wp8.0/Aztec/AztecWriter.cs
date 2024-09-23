// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.AztecWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Aztec.Internal;
using ZXing.Common;

#nullable disable
namespace ZXing.Aztec
{
  /// <summary>Generates Aztec 2D barcodes.</summary>
  public sealed class AztecWriter : Writer
  {
    private static readonly Encoding DEFAULT_CHARSET = Encoding.GetEncoding("ISO-8859-1");

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
      Encoding charset = AztecWriter.DEFAULT_CHARSET;
      int eccPercent = 33;
      int layers = 0;
      if (hints != null)
      {
        if (hints.ContainsKey(EncodeHintType.CHARACTER_SET))
        {
          object hint = hints[EncodeHintType.CHARACTER_SET];
          if (hint != null)
            charset = Encoding.GetEncoding(hint.ToString());
        }
        if (hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
        {
          object hint = hints[EncodeHintType.ERROR_CORRECTION];
          if (hint != null)
            eccPercent = Convert.ToInt32(hint);
        }
        if (hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
        {
          object hint = hints[EncodeHintType.AZTEC_LAYERS];
          if (hint != null)
            layers = Convert.ToInt32(hint);
        }
      }
      return AztecWriter.encode(contents, format, width, height, charset, eccPercent, layers);
    }

    private static BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      Encoding charset,
      int eccPercent,
      int layers)
    {
      if (format != BarcodeFormat.AZTEC)
        throw new ArgumentException("Can only encode AZTEC code, but got " + (object) format);
      return AztecWriter.renderResult(ZXing.Aztec.Internal.Encoder.encode(charset.GetBytes(contents), eccPercent, layers), width, height);
    }

    private static BitMatrix renderResult(AztecCode code, int width, int height)
    {
      BitMatrix matrix = code.Matrix;
      int val2 = matrix != null ? matrix.Width : throw new InvalidOperationException("No input code matrix");
      int height1 = matrix.Height;
      int width1 = Math.Max(width, val2);
      int height2 = Math.Max(height, height1);
      int num1 = Math.Min(width1 / val2, height2 / height1);
      int num2 = (width1 - val2 * num1) / 2;
      int num3 = (height2 - height1 * num1) / 2;
      BitMatrix bitMatrix = new BitMatrix(width1, height2);
      int y = 0;
      int top = num3;
      while (y < height1)
      {
        int x = 0;
        int left = num2;
        while (x < val2)
        {
          if (matrix[x, y])
            bitMatrix.setRegion(left, top, num1, num1);
          ++x;
          left += num1;
        }
        ++y;
        top += num1;
      }
      return bitMatrix;
    }
  }
}
