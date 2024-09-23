// Decompiled with JetBrains decompiler
// Type: ZXing.MultiFormatReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using ZXing.Aztec;
using ZXing.Datamatrix;
using ZXing.Maxicode;
using ZXing.OneD;
using ZXing.PDF417;
using ZXing.QrCode;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// MultiFormatReader is a convenience class and the main entry point into the library for most uses.
  /// By default it attempts to decode all barcode formats that the library supports. Optionally, you
  /// can provide a hints object to request different behavior, for example only decoding QR codes.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source</author>
  public sealed class MultiFormatReader : Reader
  {
    private IDictionary<DecodeHintType, object> hints;
    private IList<Reader> readers;

    /// <summary> This version of decode honors the intent of Reader.decode(BinaryBitmap) in that it
    /// passes null as a hint to the decoders. However, that makes it inefficient to call repeatedly.
    /// Use setHints() followed by decodeWithState() for continuous scan applications.
    /// 
    /// </summary>
    /// <param name="image">The pixel data to decode</param>
    /// <returns>The contents of the image</returns>
    /// <throws>  ReaderException Any errors which occurred </throws>
    public Result decode(BinaryBitmap image)
    {
      this.Hints = (IDictionary<DecodeHintType, object>) null;
      return this.decodeInternal(image);
    }

    /// <summary> Decode an image using the hints provided. Does not honor existing state.
    /// 
    /// </summary>
    /// <param name="image">The pixel data to decode</param>
    /// <param name="hints">The hints to use, clearing the previous state.</param>
    /// <returns>The contents of the image</returns>
    /// <throws>  ReaderException Any errors which occurred </throws>
    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      this.Hints = hints;
      return this.decodeInternal(image);
    }

    /// <summary> Decode an image using the state set up by calling setHints() previously. Continuous scan
    /// clients will get a <b>large</b> speed increase by using this instead of decode().
    /// 
    /// </summary>
    /// <param name="image">The pixel data to decode</param>
    /// <returns>The contents of the image</returns>
    /// <throws>  ReaderException Any errors which occurred </throws>
    public Result decodeWithState(BinaryBitmap image)
    {
      if (this.readers == null)
        this.Hints = (IDictionary<DecodeHintType, object>) null;
      return this.decodeInternal(image);
    }

    /// <summary> This method adds state to the MultiFormatReader. By setting the hints once, subsequent calls
    /// to decodeWithState(image) can reuse the same set of readers without reallocating memory. This
    /// is important for performance in continuous scan clients.
    /// 
    /// </summary>
    /// <param name="hints">The set of hints to use for subsequent calls to decode(image)
    /// </param>
    public IDictionary<DecodeHintType, object> Hints
    {
      set
      {
        this.hints = value;
        bool flag1 = value != null && value.ContainsKey(DecodeHintType.TRY_HARDER);
        IList<BarcodeFormat> barcodeFormatList = value == null || !value.ContainsKey(DecodeHintType.POSSIBLE_FORMATS) ? (IList<BarcodeFormat>) null : (IList<BarcodeFormat>) value[DecodeHintType.POSSIBLE_FORMATS];
        if (barcodeFormatList != null)
        {
          bool flag2 = barcodeFormatList.Contains(BarcodeFormat.All_1D) || barcodeFormatList.Contains(BarcodeFormat.UPC_A) || barcodeFormatList.Contains(BarcodeFormat.UPC_E) || barcodeFormatList.Contains(BarcodeFormat.EAN_13) || barcodeFormatList.Contains(BarcodeFormat.EAN_8) || barcodeFormatList.Contains(BarcodeFormat.CODABAR) || barcodeFormatList.Contains(BarcodeFormat.CODE_39) || barcodeFormatList.Contains(BarcodeFormat.CODE_93) || barcodeFormatList.Contains(BarcodeFormat.CODE_128) || barcodeFormatList.Contains(BarcodeFormat.ITF) || barcodeFormatList.Contains(BarcodeFormat.RSS_14) || barcodeFormatList.Contains(BarcodeFormat.RSS_EXPANDED);
          this.readers = (IList<Reader>) new List<Reader>();
          if (flag2 && !flag1)
            this.readers.Add((Reader) new MultiFormatOneDReader(value));
          if (barcodeFormatList.Contains(BarcodeFormat.QR_CODE))
            this.readers.Add((Reader) new QRCodeReader());
          if (barcodeFormatList.Contains(BarcodeFormat.DATA_MATRIX))
            this.readers.Add((Reader) new DataMatrixReader());
          if (barcodeFormatList.Contains(BarcodeFormat.AZTEC))
            this.readers.Add((Reader) new AztecReader());
          if (barcodeFormatList.Contains(BarcodeFormat.PDF_417))
            this.readers.Add((Reader) new PDF417Reader());
          if (barcodeFormatList.Contains(BarcodeFormat.MAXICODE))
            this.readers.Add((Reader) new MaxiCodeReader());
          if (flag2 && flag1)
            this.readers.Add((Reader) new MultiFormatOneDReader(value));
        }
        if (this.readers != null && this.readers.Count != 0)
          return;
        this.readers = this.readers ?? (IList<Reader>) new List<Reader>();
        if (!flag1)
          this.readers.Add((Reader) new MultiFormatOneDReader(value));
        this.readers.Add((Reader) new QRCodeReader());
        this.readers.Add((Reader) new DataMatrixReader());
        this.readers.Add((Reader) new AztecReader());
        this.readers.Add((Reader) new PDF417Reader());
        this.readers.Add((Reader) new MaxiCodeReader());
        if (!flag1)
          return;
        this.readers.Add((Reader) new MultiFormatOneDReader(value));
      }
    }

    public void reset()
    {
      if (this.readers == null)
        return;
      foreach (Reader reader in (IEnumerable<Reader>) this.readers)
        reader.reset();
    }

    private Result decodeInternal(BinaryBitmap image)
    {
      if (this.readers != null)
      {
        ResultPointCallback hint = this.hints == null || !this.hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK) ? (ResultPointCallback) null : (ResultPointCallback) this.hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
        for (int index = 0; index < this.readers.Count; ++index)
        {
          Reader reader = this.readers[index];
          reader.reset();
          Result result = reader.decode(image, this.hints);
          if (result != null)
          {
            this.readers.RemoveAt(index);
            this.readers.Insert(0, reader);
            return result;
          }
          if (hint != null)
            hint((ResultPoint) null);
        }
      }
      return (Result) null;
    }
  }
}
