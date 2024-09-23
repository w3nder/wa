// Decompiled with JetBrains decompiler
// Type: ZXing.Common.DetectorResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Common
{
  /// <summary> <p>Encapsulates the result of detecting a barcode in an image. This includes the raw
  /// matrix of black/white pixels corresponding to the barcode, and possibly points of interest
  /// in the image, like the location of finder patterns or corners of the barcode in the image.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  public class DetectorResult
  {
    public BitMatrix Bits { get; private set; }

    public ResultPoint[] Points { get; private set; }

    public DetectorResult(BitMatrix bits, ResultPoint[] points)
    {
      this.Bits = bits;
      this.Points = points;
    }
  }
}
