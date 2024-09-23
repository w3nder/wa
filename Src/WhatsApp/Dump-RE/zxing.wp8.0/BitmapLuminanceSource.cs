// Decompiled with JetBrains decompiler
// Type: ZXing.BitmapLuminanceSource
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Windows.Media;
using System.Windows.Media.Imaging;

#nullable disable
namespace ZXing
{
  public class BitmapLuminanceSource : BaseLuminanceSource
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.BitmapLuminanceSource" /> class.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected BitmapLuminanceSource(int width, int height)
      : base(width, height)
    {
    }

    public BitmapLuminanceSource(WriteableBitmap writeableBitmap)
      : base(writeableBitmap.PixelWidth, writeableBitmap.PixelHeight)
    {
      int pixelHeight = writeableBitmap.PixelHeight;
      int pixelWidth = writeableBitmap.PixelWidth;
      int[] pixels = writeableBitmap.Pixels;
      int index1 = 0;
      int num1 = pixelWidth * pixelHeight;
      for (int index2 = 0; index2 < num1; ++index2)
      {
        int num2 = pixels[index2];
        Color color = Color.FromArgb((byte) (num2 >> 24 & (int) byte.MaxValue), (byte) (num2 >> 16 & (int) byte.MaxValue), (byte) (num2 >> 8 & (int) byte.MaxValue), (byte) (num2 & (int) byte.MaxValue));
        this.luminances[index1] = (byte) (19562 * (int) color.R + 38550 * (int) color.G + 7424 * (int) color.B >> 16);
        ++index1;
      }
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
      BitmapLuminanceSource luminanceSource = new BitmapLuminanceSource(width, height);
      luminanceSource.luminances = newLuminances;
      return (LuminanceSource) luminanceSource;
    }
  }
}
