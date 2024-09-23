// Decompiled with JetBrains decompiler
// Type: ZXing.Multi.ByQuadrantReader
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Multi
{
  /// <summary>
  /// This class attempts to decode a barcode from an image, not by scanning the whole image,
  /// but by scanning subsets of the image. This is important when there may be multiple barcodes in
  /// an image, and detecting a barcode may find parts of multiple barcode and fail to decode
  /// (e.g. QR Codes). Instead this scans the four quadrants of the image -- and also the center
  /// 'quadrant' to cover the case where a barcode is found in the center.
  /// </summary>
  /// <seealso cref="T:ZXing.Multi.GenericMultipleBarcodeReader" />
  public sealed class ByQuadrantReader : Reader
  {
    private readonly Reader @delegate;

    public ByQuadrantReader(Reader @delegate) => this.@delegate = @delegate;

    public Result decode(BinaryBitmap image)
    {
      return this.decode(image, (IDictionary<DecodeHintType, object>) null);
    }

    public Result decode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
    {
      int width = image.Width;
      int height = image.Height;
      int num1 = width / 2;
      int num2 = height / 2;
      Result result1 = this.@delegate.decode(image.crop(0, 0, num1, num2), hints);
      if (result1 != null)
        return result1;
      Result result2 = this.@delegate.decode(image.crop(num1, 0, num1, num2), hints);
      if (result2 != null)
        return result2;
      Result result3 = this.@delegate.decode(image.crop(0, num2, num1, num2), hints);
      if (result3 != null)
        return result3;
      Result result4 = this.@delegate.decode(image.crop(num1, num2, num1, num2), hints);
      if (result4 != null)
        return result4;
      int left = num1 / 2;
      int top = num2 / 2;
      return this.@delegate.decode(image.crop(left, top, num1, num2), hints);
    }

    public void reset() => this.@delegate.reset();
  }
}
