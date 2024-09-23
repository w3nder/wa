// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.AztecReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Aztec.Internal;
using ZXing.Common;

#nullable disable
namespace ZXing.Aztec
{
  /// <summary>
  /// This implementation can detect and decode Aztec codes in an image.
  /// </summary>
  /// <author>David Olivier</author>
  public class AztecReader : Reader
  {
    /// <summary>
    /// Locates and decodes a barcode in some format within an image.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <returns>
    /// a String representing the content encoded by the Data Matrix code
    /// </returns>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>Locates and decodes a Data Matrix code in an image.</summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a {@link java.util.Hashtable} from {@link com.google.zxing.DecodeHintType}
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      BitMatrix blackMatrix = image.BlackMatrix;
      if (blackMatrix == null)
        return (Result) null;
      Detector detector = new Detector(blackMatrix);
      ResultPoint[] resultPoints = (ResultPoint[]) null;
      DecoderResult decoderResult = (DecoderResult) null;
      AztecDetectorResult detectorResult = detector.detect(false);
      if (detectorResult != null)
      {
        resultPoints = detectorResult.Points;
        decoderResult = new Decoder().decode(detectorResult);
      }
      if (decoderResult == null)
      {
        detectorResult = detector.detect(true);
        if (detectorResult == null)
          return (Result) null;
        resultPoints = detectorResult.Points;
        decoderResult = new Decoder().decode(detectorResult);
        if (decoderResult == null)
          return (Result) null;
      }
      if (hints != null && hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK))
      {
        ResultPointCallback hint = (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
        if (hint != null)
        {
          foreach (ResultPoint point in resultPoints)
            hint(point);
        }
      }
      Result result = new Result(decoderResult.Text, decoderResult.RawBytes, resultPoints, BarcodeFormat.AZTEC);
      IList<byte[]> byteSegments = decoderResult.ByteSegments;
      if (byteSegments != null)
        result.putMetadata(ResultMetadataType.BYTE_SEGMENTS, (object) byteSegments);
      string ecLevel = decoderResult.ECLevel;
      if (ecLevel != null)
        result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, (object) ecLevel);
      result.putMetadata(ResultMetadataType.AZTEC_EXTRA_METADATA, (object) new AztecResultMetadata(detectorResult.Compact, detectorResult.NbDatablocks, detectorResult.NbLayers));
      return result;
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    public void reset()
    {
    }
  }
}
