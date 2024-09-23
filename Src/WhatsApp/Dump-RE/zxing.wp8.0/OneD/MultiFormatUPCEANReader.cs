// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.MultiFormatUPCEANReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>A reader that can read all available UPC/EAN formats. If a caller wants to try to
  /// read all such formats, it is most efficient to use this implementation rather than invoke
  /// individual readers.</p>
  ///   <author>Sean Owen</author>
  /// </summary>
  public sealed class MultiFormatUPCEANReader : OneDReader
  {
    private readonly UPCEANReader[] readers;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.OneD.MultiFormatUPCEANReader" /> class.
    /// </summary>
    /// <param name="hints">The hints.</param>
    public MultiFormatUPCEANReader(IDictionary<DecodeHintType, object> hints)
    {
      IList<BarcodeFormat> hint = hints == null || !hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? (IList<BarcodeFormat>) null : (IList<BarcodeFormat>) hints[DecodeHintType.POSSIBLE_FORMATS];
      List<UPCEANReader> upceanReaderList = new List<UPCEANReader>();
      if (hint != null)
      {
        if (hint.Contains(BarcodeFormat.EAN_13) || hint.Contains(BarcodeFormat.All_1D))
          upceanReaderList.Add((UPCEANReader) new EAN13Reader());
        else if (hint.Contains(BarcodeFormat.UPC_A) || hint.Contains(BarcodeFormat.All_1D))
          upceanReaderList.Add((UPCEANReader) new UPCAReader());
        if (hint.Contains(BarcodeFormat.EAN_8) || hint.Contains(BarcodeFormat.All_1D))
          upceanReaderList.Add((UPCEANReader) new EAN8Reader());
        if (hint.Contains(BarcodeFormat.UPC_E) || hint.Contains(BarcodeFormat.All_1D))
          upceanReaderList.Add((UPCEANReader) new UPCEReader());
      }
      if (upceanReaderList.Count == 0)
      {
        upceanReaderList.Add((UPCEANReader) new EAN13Reader());
        upceanReaderList.Add((UPCEANReader) new EAN8Reader());
        upceanReaderList.Add((UPCEANReader) new UPCEReader());
      }
      this.readers = upceanReaderList.ToArray();
    }

    /// <summary>
    ///   <p>Attempts to decode a one-dimensional barcode format given a single row of
    /// an image.</p>
    /// </summary>
    /// <param name="rowNumber">row number from top of the row</param>
    /// <param name="row">the black/white pixel data of the row</param>
    /// <param name="hints">decode hints</param>
    /// <returns>
    ///   <see cref="T:ZXing.Result" />containing encoded string and start/end of barcode or null if an error occurs or barcode cannot be found
    /// </returns>
    public override Result decodeRow(
      int rowNumber,
      BitArray row,
      IDictionary<DecodeHintType, object> hints)
    {
      int[] startGuardPattern = UPCEANReader.findStartGuardPattern(row);
      if (startGuardPattern == null)
        return (Result) null;
      foreach (UPCEANReader reader in this.readers)
      {
        Result result1 = reader.decodeRow(rowNumber, row, startGuardPattern, hints);
        if (result1 != null)
        {
          bool flag1 = result1.BarcodeFormat == BarcodeFormat.EAN_13 && result1.Text[0] == '0';
          IList<BarcodeFormat> hint = hints == null || !hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? (IList<BarcodeFormat>) null : (IList<BarcodeFormat>) hints[DecodeHintType.POSSIBLE_FORMATS];
          bool flag2 = hint == null || hint.Contains(BarcodeFormat.UPC_A) || hint.Contains(BarcodeFormat.All_1D);
          if (!flag1 || !flag2)
            return result1;
          Result result2 = new Result(result1.Text.Substring(1), result1.RawBytes, result1.ResultPoints, BarcodeFormat.UPC_A);
          result2.putAllMetadata(result1.ResultMetadata);
          return result2;
        }
      }
      return (Result) null;
    }

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    public override void reset()
    {
      foreach (Reader reader in this.readers)
        reader.reset();
    }
  }
}
