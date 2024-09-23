// Decompiled with JetBrains decompiler
// Type: ZXing.BarcodeReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Windows.Media.Imaging;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A smart class to decode the barcode inside a bitmap object
  /// </summary>
  public class BarcodeReader : 
    BarcodeReaderGeneric<WriteableBitmap>,
    IBarcodeReader,
    IMultipleBarcodeReader
  {
    private static readonly Func<WriteableBitmap, LuminanceSource> defaultCreateLuminanceSource = (Func<WriteableBitmap, LuminanceSource>) (bitmap => (LuminanceSource) new BitmapLuminanceSource(bitmap));

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReader" /> class.
    /// </summary>
    public BarcodeReader()
      : this((Reader) new MultiFormatReader(), BarcodeReader.defaultCreateLuminanceSource, (Func<LuminanceSource, Binarizer>) null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReader" /> class.
    /// </summary>
    /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
    /// If null then MultiFormatReader is used</param>
    /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
    /// If null, an exception is thrown when Decode is called</param>
    /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
    /// If null then HybridBinarizer is used</param>
    public BarcodeReader(
      Reader reader,
      Func<WriteableBitmap, LuminanceSource> createLuminanceSource,
      Func<LuminanceSource, Binarizer> createBinarizer)
      : base(reader, createLuminanceSource ?? BarcodeReader.defaultCreateLuminanceSource, createBinarizer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeReader" /> class.
    /// </summary>
    /// <param name="reader">Sets the reader which should be used to find and decode the barcode.
    /// If null then MultiFormatReader is used</param>
    /// <param name="createLuminanceSource">Sets the function to create a luminance source object for a bitmap.
    /// If null, an exception is thrown when Decode is called</param>
    /// <param name="createBinarizer">Sets the function to create a binarizer object for a luminance source.
    /// If null then HybridBinarizer is used</param>
    public BarcodeReader(
      Reader reader,
      Func<WriteableBitmap, LuminanceSource> createLuminanceSource,
      Func<LuminanceSource, Binarizer> createBinarizer,
      Func<byte[], int, int, RGBLuminanceSource.BitmapFormat, LuminanceSource> createRGBLuminanceSource)
      : base(reader, createLuminanceSource ?? BarcodeReader.defaultCreateLuminanceSource, createBinarizer, createRGBLuminanceSource)
    {
    }
  }
}
