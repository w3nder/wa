// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.MultipleBarcodeReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Multi
{
  /// <summary>
  /// Implementation of this interface attempt to read several barcodes from one image.
  /// <author>Sean Owen</author>
  /// 	<seealso cref="T:ZXing.Reader" />
  /// </summary>
  public interface MultipleBarcodeReader
  {
    /// <summary>Decodes the multiple.</summary>
    /// <param name="image">The image.</param>
    /// <returns></returns>
    Result[] decodeMultiple(BinaryBitmap image);

    /// <summary>Decodes the multiple.</summary>
    /// <param name="image">The image.</param>
    /// <param name="hints">The hints.</param>
    /// <returns></returns>
    Result[] decodeMultiple(BinaryBitmap image, IDictionary<DecodeHintType, object> hints);
  }
}
