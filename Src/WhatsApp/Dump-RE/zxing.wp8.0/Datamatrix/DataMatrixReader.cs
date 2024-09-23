// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.DataMatrixReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;
using ZXing.Datamatrix.Internal;

#nullable disable
namespace ZXing.Datamatrix
{
  /// <summary>
  /// This implementation can detect and decode Data Matrix codes in an image.
  /// 
  /// <author>bbrown@google.com (Brian Brown)</author>
  /// </summary>
  public sealed class DataMatrixReader : Reader
  {
    private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];
    private readonly Decoder decoder = new Decoder();

    /// <summary>
    /// Locates and decodes a Data Matrix code in an image.
    /// 
    /// <returns>a String representing the content encoded by the Data Matrix code</returns>
    /// <exception cref="T:ZXing.FormatException">if a Data Matrix code cannot be decoded</exception>
    /// </summary>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      DecoderResult decoderResult;
      ResultPoint[] resultPoints;
      if (hints != null && hints.ContainsKey(DecodeHintType.PURE_BARCODE))
      {
        BitMatrix pureBits = DataMatrixReader.extractPureBits(image.BlackMatrix);
        if (pureBits == null)
          return (Result) null;
        decoderResult = this.decoder.decode(pureBits);
        resultPoints = DataMatrixReader.NO_POINTS;
      }
      else
      {
        DetectorResult detectorResult = new Detector(image.BlackMatrix).detect();
        if (detectorResult == null)
          return (Result) null;
        decoderResult = this.decoder.decode(detectorResult.Bits);
        resultPoints = detectorResult.Points;
      }
      if (decoderResult == null)
        return (Result) null;
      Result result = new Result(decoderResult.Text, decoderResult.RawBytes, resultPoints, BarcodeFormat.DATA_MATRIX);
      IList<byte[]> byteSegments = decoderResult.ByteSegments;
      if (byteSegments != null)
        result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, (object) byteSegments);
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
    /// <seealso cref="M:ZXing.QrCode.QRCodeReader.extractPureBits(ZXing.Common.BitMatrix)" />
    /// </summary>
    private static BitMatrix extractPureBits(BitMatrix image)
    {
      int[] topLeftOnBit = image.getTopLeftOnBit();
      int[] bottomRightOnBit = image.getBottomRightOnBit();
      if (topLeftOnBit == null || bottomRightOnBit == null)
        return (BitMatrix) null;
      int modulesize;
      if (!DataMatrixReader.moduleSize(topLeftOnBit, image, out modulesize))
        return (BitMatrix) null;
      int num1 = topLeftOnBit[1];
      int num2 = bottomRightOnBit[1];
      int num3 = topLeftOnBit[0];
      int width = (bottomRightOnBit[0] - num3 + 1) / modulesize;
      int height = (num2 - num1 + 1) / modulesize;
      if (width <= 0 || height <= 0)
        return (BitMatrix) null;
      int num4 = modulesize >> 1;
      int num5 = num1 + num4;
      int num6 = num3 + num4;
      BitMatrix pureBits = new BitMatrix(width, height);
      for (int y1 = 0; y1 < height; ++y1)
      {
        int y2 = num5 + y1 * modulesize;
        for (int x = 0; x < width; ++x)
        {
          if (image[num6 + x * modulesize, y2])
            pureBits[x, y1] = true;
        }
      }
      return pureBits;
    }

    private static bool moduleSize(int[] leftTopBlack, BitMatrix image, out int modulesize)
    {
      int width = image.Width;
      int x = leftTopBlack[0];
      int y = leftTopBlack[1];
      while (x < width && image[x, y])
        ++x;
      if (x == width)
      {
        modulesize = 0;
        return false;
      }
      modulesize = x - leftTopBlack[0];
      return modulesize != 0;
    }
  }
}
