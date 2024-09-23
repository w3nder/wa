// Decompiled with JetBrains decompiler
// Type: ZXing.BarcodeWriterSvg
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Rendering;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A smart class to encode some content to a svg barcode image
  /// </summary>
  public class BarcodeWriterSvg : BarcodeWriterGeneric<SvgRenderer.SvgImage>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeWriter" /> class.
    /// </summary>
    public BarcodeWriterSvg()
    {
      this.Renderer = (IBarcodeRenderer<SvgRenderer.SvgImage>) new SvgRenderer();
    }
  }
}
