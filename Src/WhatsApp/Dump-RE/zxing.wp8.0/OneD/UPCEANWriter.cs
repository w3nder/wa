// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.UPCEANWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>Encapsulates functionality and implementation that is common to UPC and EAN families
  /// of one-dimensional barcodes.</p>
  ///   <author>aripollak@gmail.com (Ari Pollak)</author>
  ///   <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
  /// </summary>
  public abstract class UPCEANWriter : OneDimensionalCodeWriter
  {
    /// <summary>Gets the default margin.</summary>
    public override int DefaultMargin => UPCEANReader.START_END_PATTERN.Length;
  }
}
