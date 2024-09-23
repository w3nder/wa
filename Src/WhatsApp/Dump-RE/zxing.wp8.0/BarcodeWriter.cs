// Decompiled with JetBrains decompiler
// Type: ZXing.BarcodeWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Windows.Media.Imaging;
using ZXing.Rendering;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A smart class to encode some content to a barcode image
  /// </summary>
  public class BarcodeWriter : BarcodeWriterGeneric<WriteableBitmap>, IBarcodeWriter
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BarcodeWriter" /> class.
    /// </summary>
    public BarcodeWriter()
    {
      this.Renderer = (IBarcodeRenderer<WriteableBitmap>) new WriteableBitmapRenderer();
    }
  }
}
