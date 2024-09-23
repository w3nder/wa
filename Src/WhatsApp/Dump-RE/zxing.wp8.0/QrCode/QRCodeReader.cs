// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.QRCodeReader
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
  /// This implementation can detect and decode QR Codes in an image.
  /// <author>Sean Owen</author>
  /// </summary>
  public class QRCodeReader : Reader
  {
    private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
    private readonly Decoder decoder = new Decoder();

    /// <summary>Gets the decoder.</summary>
    /// <returns></returns>
    protected Decoder getDecoder() => this.decoder;

    /// <summary>
    /// Locates and decodes a QR code in an image.
    /// 
    /// <returns>a String representing the content encoded by the QR code</returns>
    /// </summary>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    /// Locates and decodes a barcode in some format within an image. This method also accepts
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
      if (image == null || image.BlackMatrix == null)
        return (Result) null;
      DecoderResult decoderResult;
      ResultPoint[] resultPointArray;
      if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
      {
        BitMatrix pureBits = QRCodeReader.extractPureBits(image.BlackMatrix);
        if (pureBits == null)
          return (Result) null;
        decoderResult = this.decoder.decode(pureBits, hints);
        resultPointArray = QRCodeReader.NO_POINTS;
      }
      else
      {
        DetectorResult detectorResult = new Detector(image.BlackMatrix).detect(hints);
        if (detectorResult == null)
          return (Result) null;
        decoderResult = this.decoder.decode(detectorResult.Bits, hints);
        resultPointArray = detectorResult.Points;
      }
      if (decoderResult == null)
        return (Result) null;
      if (decoderResult.Other is QRCodeDecoderMetaData other)
        other.applyMirroredCorrection(resultPointArray);
      Result result = new Result(decoderResult.Text, decoderResult.RawBytes, resultPointArray, BarcodeFormat.QR_CODE);
      IList<byte[]> byteSegments = decoderResult.ByteSegments;
      if (byteSegments != null)
        result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, (object) byteSegments);
      string ecLevel = decoderResult.ECLevel;
      if (ecLevel != null)
        result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, (object) ecLevel);
      if (decoderResult.StructuredAppend)
      {
        result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_SEQUENCE, (object) decoderResult.StructuredAppendSequenceNumber);
        result.putMetadata(ResultMetadataType.STRUCTURED_APPEND_PARITY, (object) decoderResult.StructuredAppendParity);
      }
      return result;
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
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
    /// </summary>
    private static BitMatrix extractPureBits(BitMatrix image)
    {
      int[] topLeftOnBit = image.getTopLeftOnBit();
      int[] bottomRightOnBit = image.getBottomRightOnBit();
      if (topLeftOnBit == null || bottomRightOnBit == null)
        return (BitMatrix) null;
      float msize;
      if (!QRCodeReader.moduleSize(topLeftOnBit, image, out msize))
        return (BitMatrix) null;
      int num1 = topLeftOnBit[1];
      int num2 = bottomRightOnBit[1];
      int num3 = topLeftOnBit[0];
      int num4 = bottomRightOnBit[0];
      if (num3 >= num4 || num1 >= num2)
        return (BitMatrix) null;
      if (num2 - num1 != num4 - num3)
        num4 = num3 + (num2 - num1);
      int width = (int) Math.Round((double) (num4 - num3 + 1) / (double) msize);
      int height = (int) Math.Round((double) (num2 - num1 + 1) / (double) msize);
      if (width <= 0 || height <= 0)
        return (BitMatrix) null;
      if (height != width)
        return (BitMatrix) null;
      int num5 = (int) ((double) msize / 2.0);
      int num6 = num1 + num5;
      int num7 = num3 + num5;
      int num8 = num7 + (int) ((double) (width - 1) * (double) msize) - (num4 - 1);
      if (num8 > 0)
      {
        if (num8 > num5)
          return (BitMatrix) null;
        num7 -= num8;
      }
      int num9 = num6 + (int) ((double) (height - 1) * (double) msize) - (num2 - 1);
      if (num9 > 0)
      {
        if (num9 > num5)
          return (BitMatrix) null;
        num6 -= num9;
      }
      BitMatrix pureBits = new BitMatrix(width, height);
      for (int y1 = 0; y1 < height; ++y1)
      {
        int y2 = num6 + (int) ((double) y1 * (double) msize);
        for (int x = 0; x < width; ++x)
        {
          if (image[num7 + (int) ((double) x * (double) msize), y2])
            pureBits[x, y1] = true;
        }
      }
      return pureBits;
    }

    private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out float msize)
    {
      int height = image.Height;
      int width = image.Width;
      int x = leftTopBlack[0];
      int y = leftTopBlack[1];
      bool flag = true;
      int num = 0;
      for (; x < width && y < height; ++y)
      {
        if (flag != image[x, y])
        {
          if (++num != 5)
            flag = !flag;
          else
            break;
        }
        ++x;
      }
      if (x == width || y == height)
      {
        msize = 0.0f;
        return false;
      }
      msize = (float) (x - leftTopBlack[0]) / 7f;
      return true;
    }
  }
}
