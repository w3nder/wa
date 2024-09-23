// Decompiled with JetBrains decompiler
// Type: ZXing.Reader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// Implementations of this interface can decode an image of a barcode in some format into
  /// the String it encodes. For example, <see cref="T:ZXing.QrCode.QRCodeReader" /> can
  /// decode a QR code. The decoder may optionally receive hints from the caller which may help
  /// it decode more quickly or accurately.
  /// 
  /// See <see cref="T:ZXing.MultiFormatReader" />, which attempts to determine what barcode
  /// format is present within the image as well, and then decodes it accordingly.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  public interface Reader
  {
    /// <summary>
    /// Locates and decodes a barcode in some format within an image.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <returns>String which the barcode encodes</returns>
    Result decode(BinaryBitmap image);

    /// <summary> Locates and decodes a barcode in some format within an image. This method also accepts
    /// hints, each possibly associated to some data, which may help the implementation decode.
    /// </summary>
    /// <param name="image">image of barcode to decode</param>
    /// <param name="hints">passed as a <see cref="T:System.Collections.Generic.IDictionary`2" /> from <see cref="T:ZXing.DecodeHintType" />
    /// to arbitrary data. The
    /// meaning of the data depends upon the hint type. The implementation may or may not do
    /// anything with these hints.
    /// </param>
    /// <returns>String which the barcode encodes</returns>
    Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints);

    /// <summary>
    /// Resets any internal state the implementation has after a decode, to prepare it
    /// for reuse.
    /// </summary>
    void reset();
  }
}
