// Decompiled with JetBrains decompiler
// Type: ZXing.RGB565LuminanceSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// 
  /// </summary>
  [Obsolete("Use RGBLuminanceSource with the argument BitmapFormat.RGB565")]
  public class RGB565LuminanceSource : RGBLuminanceSource
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGB565LuminanceSource" /> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected RGB565LuminanceSource(int width, int height)
      : base(width, height)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.RGB565LuminanceSource" /> class.
    /// </summary>
    /// <param name="rgb565RawData">The RGB565 raw data.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public RGB565LuminanceSource(byte[] rgb565RawData, int width, int height)
      : base(rgb565RawData, width, height, RGBLuminanceSource.BitmapFormat.RGB565)
    {
    }

    /// <summary>
    /// Should create a new luminance source with the right class type.
    /// The method is used in methods crop and rotate.
    /// </summary>
    /// <param name="newLuminances">The new luminances.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <returns></returns>
    protected override LuminanceSource CreateLuminanceSource(
      byte[] newLuminances,
      int width,
      int height)
    {
      RGB565LuminanceSource luminanceSource = new RGB565LuminanceSource(width, height);
      luminanceSource.luminances = newLuminances;
      return (LuminanceSource) luminanceSource;
    }
  }
}
