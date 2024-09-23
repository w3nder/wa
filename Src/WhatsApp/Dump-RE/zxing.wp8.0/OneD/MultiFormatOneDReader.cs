// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.MultiFormatOneDReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;
using ZXing.OneD.RSS;
using ZXing.OneD.RSS.Expanded;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>Sean Owen</author>
  /// </summary>
  public sealed class MultiFormatOneDReader : OneDReader
  {
    private readonly IList<OneDReader> readers;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.MultiFormatOneDReader" /> class.
    /// </summary>
    /// <param name="hints">The hints.</param>
    public MultiFormatOneDReader(IDictionary<DecodeHintType, object> hints)
    {
      IList<BarcodeFormat> hint = hints == null || !hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? (IList<BarcodeFormat>) null : (IList<BarcodeFormat>) hints[DecodeHintType.POSSIBLE_FORMATS];
      this.readers = (IList<OneDReader>) new List<OneDReader>();
      if (hint != null)
      {
        if (hint.Contains(BarcodeFormat.All_1D) || hint.Contains(BarcodeFormat.EAN_13) || hint.Contains(BarcodeFormat.UPC_A) || hint.Contains(BarcodeFormat.EAN_8) || hint.Contains(BarcodeFormat.UPC_E))
          this.readers.Add((OneDReader) new MultiFormatUPCEANReader(hints));
        if (hint.Contains(BarcodeFormat.MSI))
          this.readers.Add((OneDReader) new MSIReader(hints.ContainsKey(DecodeHintType.ASSUME_MSI_CHECK_DIGIT) && (bool) hints[DecodeHintType.ASSUME_MSI_CHECK_DIGIT]));
        if (hint.Contains(BarcodeFormat.CODE_39) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new Code39Reader(hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT) && (bool) hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT], hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE) && (bool) hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE]));
        if (hint.Contains(BarcodeFormat.CODE_93) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new Code93Reader());
        if (hint.Contains(BarcodeFormat.CODE_128) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new Code128Reader());
        if (hint.Contains(BarcodeFormat.ITF) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new ITFReader());
        if (hint.Contains(BarcodeFormat.CODABAR) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new CodaBarReader());
        if (hint.Contains(BarcodeFormat.RSS_14) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new RSS14Reader());
        if (hint.Contains(BarcodeFormat.RSS_EXPANDED) || hint.Contains(BarcodeFormat.All_1D))
          this.readers.Add((OneDReader) new RSSExpandedReader());
      }
      if (this.readers.Count != 0)
        return;
      bool usingCheckDigit = hints != null && hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT) && (bool) hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT];
      bool extendedMode = hints != null && hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE) && (bool) hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
      this.readers.Add((OneDReader) new MultiFormatUPCEANReader(hints));
      this.readers.Add((OneDReader) new Code39Reader(usingCheckDigit, extendedMode));
      this.readers.Add((OneDReader) new CodaBarReader());
      this.readers.Add((OneDReader) new Code93Reader());
      this.readers.Add((OneDReader) new Code128Reader());
      this.readers.Add((OneDReader) new ITFReader());
      this.readers.Add((OneDReader) new RSS14Reader());
      this.readers.Add((OneDReader) new RSSExpandedReader());
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
      foreach (OneDReader reader in (IEnumerable<OneDReader>) this.readers)
      {
        Result result = reader.decodeRow(rowNumber, row, hints);
        if (result != null)
          return result;
      }
      return (Result) null;
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    public override void reset()
    {
      foreach (Reader reader in (IEnumerable<OneDReader>) this.readers)
        reader.reset();
    }
  }
}
