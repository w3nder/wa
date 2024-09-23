// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCAReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.Text;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>Implements decoding of the UPC-A format.</p>
  ///   <author>dswitkin@google.com (Daniel Switkin)</author>
  ///   <author>Sean Owen</author>
  /// </summary>
  public sealed class UPCAReader : UPCEANReader
  {
    private readonly UPCEANReader ean13Reader = (UPCEANReader) new EAN13Reader();

    /// <summary>
    ///   <p>Like decodeRow(int, BitArray, java.util.Map), but
    /// allows caller to inform method about where the UPC/EAN start pattern is
    /// found. This allows this to be computed once and reused across many implementations.</p>
    /// </summary>
    /// <param name="rowNumber"></param>
    /// <param name="row"></param>
    /// <param name="startGuardRange"></param>
    /// <param name="hints"></param>
    /// <returns></returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      int[] startGuardRange,
      IDictionary<DecodeHintType, object> hints)
    {
      return UPCAReader.maybeReturnResult(this.ean13Reader.decodeRow(rowNumber, row, startGuardRange, hints));
    }

    /// <summary>
    ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
    /// an image.</p>
    /// </summary>
    /// <param name="rowNumber">row number from top of the row</param>
    /// <param name="row">the black/white pixel data of the row</param>
    /// <param name="hints">decode hints</param>
    /// <returns>
    ///   <see cref="T:ZXing.Result" />containing encoded string and start/end of barcode or null, if an error occurs or barcode cannot be found
    /// </returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      return UPCAReader.maybeReturnResult(this.ean13Reader.decodeRow(rowNumber, row, hints));
    }

    /// <summary>Decodes the specified image.</summary>
    /// <param name="image">The image.</param>
    /// <param name="hints">The hints.</param>
    /// <returns></returns>
    public override Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      return UPCAReader.maybeReturnResult(this.ean13Reader.decode(image, hints));
    }

    /// <summary>
    /// Get the format of this decoder.
    /// <returns>The 1D format.</returns>
    /// </summary>
    internal override BarcodeFormat BarcodeFormat => BarcodeFormat.UPC_A;

    /// <summary>
    /// Subclasses override this to decode the portion of a barcode between the start
    /// and end guard patterns.
    /// </summary>
    /// <param name="row">row of black/white values to search</param>
    /// <param name="startRange">start/end offset of start guard pattern</param>
    /// <param name="resultString"><see cref="T:System.Text.StringBuilder" />to append decoded chars to</param>
    /// <returns>
    /// horizontal offset of first pixel after the "middle" that was decoded or -1 if decoding could not complete successfully
    /// </returns>
    protected internal override int decodeMiddle(
      BitArray row,
      int[] startRange,
      StringBuilder resultString)
    {
      return this.ean13Reader.decodeMiddle(row, startRange, resultString);
    }

    private static Result maybeReturnResult(Result result)
    {
      if (result == null)
        return (Result) null;
      string text = result.Text;
      return text[0] == '0' ? new Result(text.Substring(1), (byte[]) null, result.ResultPoints, BarcodeFormat.UPC_A) : (Result) null;
    }
  }
}
