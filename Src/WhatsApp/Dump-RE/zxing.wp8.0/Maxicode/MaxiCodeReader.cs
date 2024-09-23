// Decompiled with JetBrains decompiler
// Type: ZXing.Maxicode.MaxiCodeReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;
using ZXing.Maxicode.Internal;

#nullable disable
namespace ZXing.Maxicode
{
  /// <summary>
  /// This implementation can detect and decode a MaxiCode in an image.
  /// </summary>
  public sealed class MaxiCodeReader : Reader
  {
    private const int MATRIX_WIDTH = 30;
    private const int MATRIX_HEIGHT = 33;
    private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
    private readonly Decoder decoder = new Decoder();

    /// <summary>
    /// Locates and decodes a MaxiCode in an image.
    /// 
    /// <returns>a String representing the content encoded by the MaxiCode</returns>
    /// <exception cref="T:ZXing.FormatException">if a MaxiCode cannot be decoded</exception>
    /// </summary>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    /// Locates and decodes a MaxiCode within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      if (hints == null || !hints.ContainsKey(DecodeHintType.PURE_BARCODE))
        return (Result) null;
      BitMatrix pureBits = MaxiCodeReader.extractPureBits(image.BlackMatrix);
      if (pureBits == null)
        return (Result) null;
      DecoderResult decoderResult = this.decoder.decode(pureBits, hints);
      if (decoderResult == null)
        return (Result) null;
      ResultPoint[] noPoints = MaxiCodeReader.NO_POINTS;
      Result result = new Result(decoderResult.Text, decoderResult.RawBytes, noPoints, BarcodeFormat.MAXICODE);
      string ecLevel = decoderResult.ECLevel;
      if (ecLevel != null)
        result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, (object) ecLevel);
      return result;
    }

    public void reset()
    {
    }

    /// <summary>
    /// This method detects a code in a "pure" image -- that is, pure monochrome image
    /// which contains only an unrotated, unskewed, image of a code, with some white border
    /// around it. This is a specialized method that works exceptionally fast in this special
    /// case.
    /// 
    /// <seealso cref="M:ZXing.Datamatrix.DataMatrixReader.extractPureBits(ZXing.Common.BitMatrix)" />
    /// <seealso cref="M:ZXing.QrCode.QRCodeReader.extractPureBits(ZXing.Common.BitMatrix)" />
    /// </summary>
    private static BitMatrix extractPureBits(BitMatrix image)
    {
      int[] enclosingRectangle = image.getEnclosingRectangle();
      if (enclosingRectangle == null)
        return (BitMatrix) null;
      int num1 = enclosingRectangle[0];
      int num2 = enclosingRectangle[1];
      int num3 = enclosingRectangle[2];
      int num4 = enclosingRectangle[3];
      BitMatrix pureBits = new BitMatrix(30, 33);
      for (int y1 = 0; y1 < 33; ++y1)
      {
        int y2 = num2 + (y1 * num4 + num4 / 2) / 33;
        for (int x1 = 0; x1 < 30; ++x1)
        {
          int x2 = num1 + (x1 * num3 + num3 / 2 + (y1 & 1) * num3 / 2) / 30;
          if (image[x2, y2])
            pureBits[x1, y1] = true;
        }
      }
      return pureBits;
    }
  }
}
