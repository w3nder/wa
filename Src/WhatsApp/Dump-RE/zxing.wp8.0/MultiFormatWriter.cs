// Decompiled with JetBrains decompiler
// Type: ZXing.MultiFormatWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Aztec;
using ZXing.Common;
using ZXing.Datamatrix;
using ZXing.OneD;
using ZXing.PDF417;
using ZXing.QrCode;

#nullable disable
namespace ZXing
{
  /// <summary> This is a factory class which finds the appropriate Writer subclass for the BarcodeFormat
  /// requested and encodes the barcode with the supplied contents.
  /// 
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public sealed class MultiFormatWriter : Writer
  {
    private static readonly IDictionary<BarcodeFormat, Func<Writer>> formatMap = (IDictionary<BarcodeFormat, Func<Writer>>) new Dictionary<BarcodeFormat, Func<Writer>>()
    {
      {
        BarcodeFormat.EAN_8,
        (Func<Writer>) (() => (Writer) new EAN8Writer())
      },
      {
        BarcodeFormat.EAN_13,
        (Func<Writer>) (() => (Writer) new EAN13Writer())
      },
      {
        BarcodeFormat.UPC_A,
        (Func<Writer>) (() => (Writer) new UPCAWriter())
      },
      {
        BarcodeFormat.QR_CODE,
        (Func<Writer>) (() => (Writer) new QRCodeWriter())
      },
      {
        BarcodeFormat.CODE_39,
        (Func<Writer>) (() => (Writer) new Code39Writer())
      },
      {
        BarcodeFormat.CODE_128,
        (Func<Writer>) (() => (Writer) new Code128Writer())
      },
      {
        BarcodeFormat.ITF,
        (Func<Writer>) (() => (Writer) new ITFWriter())
      },
      {
        BarcodeFormat.PDF_417,
        (Func<Writer>) (() => (Writer) new PDF417Writer())
      },
      {
        BarcodeFormat.CODABAR,
        (Func<Writer>) (() => (Writer) new CodaBarWriter())
      },
      {
        BarcodeFormat.MSI,
        (Func<Writer>) (() => (Writer) new MSIWriter())
      },
      {
        BarcodeFormat.PLESSEY,
        (Func<Writer>) (() => (Writer) new PlesseyWriter())
      },
      {
        BarcodeFormat.DATA_MATRIX,
        (Func<Writer>) (() => (Writer) new DataMatrixWriter())
      },
      {
        BarcodeFormat.AZTEC,
        (Func<Writer>) (() => (Writer) new AztecWriter())
      }
    };

    /// <summary>Gets the collection of supported writers.</summary>
    public static ICollection<BarcodeFormat> SupportedWriters => MultiFormatWriter.formatMap.Keys;

    public BitMatrix encode(string contents, BarcodeFormat format, int width, int height)
    {
      return this.encode(contents, format, width, height, (IDictionary<EncodeHintType, object>) null);
    }

    public BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (!MultiFormatWriter.formatMap.ContainsKey(format))
        throw new ArgumentException("No encoder available for format " + (object) format);
      return MultiFormatWriter.formatMap[format]().encode(contents, format, width, height, hints);
    }
  }
}
