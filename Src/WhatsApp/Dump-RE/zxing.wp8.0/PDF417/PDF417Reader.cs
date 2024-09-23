// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.PDF417Reader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Multi;
using ZXing.PDF417.Internal;

#nullable disable
namespace ZXing.PDF417
{
  /// <summary>
  /// This implementation can detect and decode PDF417 codes in an image.
  /// 
  /// <author>SITA Lab (kevin.osullivan@sita.aero)</author>
  /// <author>Guenther Grau</author>
  /// </summary>
  public sealed class PDF417Reader : Reader, MultipleBarcodeReader
  {
    /// <summary>
    /// Locates and decodes a PDF417 code in an image.
    /// 
    /// <returns>a String representing the content encoded by the PDF417 code</returns>
    /// <exception cref="T:ZXing.FormatException">if a PDF417 cannot be decoded</exception>
    /// </summary>
    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    /// Locates and decodes a barcode in some format within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// **Note** this will return the FIRST barcode discovered if there are many.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcode encodes</returns>
    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      Result[] resultArray = PDF417Reader.decode(image, hints, false);
      return resultArray.Length == 0 ? (Result) null : resultArray[0];
    }

    /// <summary>
    /// Locates and decodes Multiple PDF417 codes in an image.
    /// 
    /// <returns>an array of Strings representing the content encoded by the PDF417 codes</returns>
    /// </summary>
    public Result[] decodeMultiple(BinaryBitmap image)
    {
      return this.decodeMultiple(image, (IDictionary<DecodeHintType, object>) null);
    }

    /// <summary>
    /// Locates and decodes multiple barcodes in some format within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.</param>
    /// <returns>String which the barcodes encode</returns>
    public Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      return PDF417Reader.decode(image, hints, true);
    }

    /// <summary>
    /// Decode the specified image, with the hints and optionally multiple barcodes.
    /// Based on Owen's Comments in <see cref="T:ZXing.ReaderException" />, this method has been modified to continue silently
    /// if a barcode was not decoded where it was detected instead of throwing a new exception object.
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="hints">Hints.</param>
    /// <param name="multiple">If set to <c>true</c> multiple.</param>
    private static Result[] decode(
      BinaryBitmap image,
      IDictionary<DecodeHintType, object> hints,
      bool multiple)
    {
      List<Result> resultList = new List<Result>();
      PDF417DetectorResult f417DetectorResult = Detector.detect(image, hints, multiple);
      foreach (ResultPoint[] point in f417DetectorResult.Points)
      {
        DecoderResult decoderResult = PDF417ScanningDecoder.decode(f417DetectorResult.Bits, point[4], point[5], point[6], point[7], PDF417Reader.getMinCodewordWidth(point), PDF417Reader.getMaxCodewordWidth(point));
        if (decoderResult != null)
        {
          Result result = new Result(decoderResult.Text, decoderResult.RawBytes, point, BarcodeFormat.PDF_417);
          result.putMetadata(ResultMetadataType.ERROR_CORRECTION_LEVEL, (object) decoderResult.ECLevel);
          PDF417ResultMetadata other = (PDF417ResultMetadata) decoderResult.Other;
          if (other != null)
            result.putMetadata(ResultMetadataType.PDF417_EXTRA_METADATA, (object) other);
          resultList.Add(result);
        }
      }
      return resultList.ToArray();
    }

    /// <summary>Gets the maximum width of the barcode</summary>
    /// <returns>The max width.</returns>
    /// <param name="p1">P1.</param>
    /// <param name="p2">P2.</param>
    private static int getMaxWidth(ResultPoint p1, ResultPoint p2)
    {
      return p1 == null || p2 == null ? 0 : (int) Math.Abs(p1.X - p2.X);
    }

    /// <summary>Gets the minimum width of the barcode</summary>
    /// <returns>The minimum width.</returns>
    /// <param name="p1">P1.</param>
    /// <param name="p2">P2.</param>
    private static int getMinWidth(ResultPoint p1, ResultPoint p2)
    {
      return p1 == null || p2 == null ? int.MaxValue : (int) Math.Abs(p1.X - p2.X);
    }

    /// <summary>Gets the maximum width of the codeword.</summary>
    /// <returns>The max codeword width.</returns>
    /// <param name="p">P.</param>
    private static int getMaxCodewordWidth(ResultPoint[] p)
    {
      return Math.Max(Math.Max(PDF417Reader.getMaxWidth(p[0], p[4]), PDF417Reader.getMaxWidth(p[6], p[2]) * PDF417Common.MODULES_IN_CODEWORD / PDF417Common.MODULES_IN_STOP_PATTERN), Math.Max(PDF417Reader.getMaxWidth(p[1], p[5]), PDF417Reader.getMaxWidth(p[7], p[3]) * PDF417Common.MODULES_IN_CODEWORD / PDF417Common.MODULES_IN_STOP_PATTERN));
    }

    /// <summary>Gets the minimum width of the codeword.</summary>
    /// <returns>The minimum codeword width.</returns>
    /// <param name="p">P.</param>
    private static int getMinCodewordWidth(ResultPoint[] p)
    {
      return Math.Min(Math.Min(PDF417Reader.getMinWidth(p[0], p[4]), PDF417Reader.getMinWidth(p[6], p[2]) * PDF417Common.MODULES_IN_CODEWORD / PDF417Common.MODULES_IN_STOP_PATTERN), Math.Min(PDF417Reader.getMinWidth(p[1], p[5]), PDF417Reader.getMinWidth(p[7], p[3]) * PDF417Common.MODULES_IN_CODEWORD / PDF417Common.MODULES_IN_STOP_PATTERN));
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
