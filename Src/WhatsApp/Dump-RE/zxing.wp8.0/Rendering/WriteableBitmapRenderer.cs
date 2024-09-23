// Decompiled with JetBrains decompiler
// Type: ZXing.Rendering.WriteableBitmapRenderer
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Common;

#nullable disable
namespace ZXing.Rendering
{
  /// <summary>
  /// Renders a <see cref="T:ZXing.Common.BitMatrix" /> to a <see cref="T:System.Windows.Media.Imaging.WriteableBitmap" />
  /// </summary>
  public class WriteableBitmapRenderer : IBarcodeRenderer<WriteableBitmap>
  {
    private static readonly FontFamily DefaultFontFamily = new FontFamily("Arial");

    /// <summary>Gets or sets the foreground color.</summary>
    /// <value>The foreground color.</value>
    public Color Foreground { get; set; }

    /// <summary>Gets or sets the background color.</summary>
    /// <value>The background color.</value>
    public Color Background { get; set; }

    /// <summary>Gets or sets the font family.</summary>
    /// <value>The font family.</value>
    public FontFamily FontFamily { get; set; }

    /// <summary>Gets or sets the size of the font.</summary>
    /// <value>The size of the font.</value>
    public double FontSize { get; set; }

    /// <summary>Gets or sets the font stretch.</summary>
    /// <value>The font stretch.</value>
    public FontStretch FontStretch { get; set; }

    /// <summary>Gets or sets the font style.</summary>
    /// <value>The font style.</value>
    public FontStyle FontStyle { get; set; }

    /// <summary>Gets or sets the font weight.</summary>
    /// <value>The font weight.</value>
    public FontWeight FontWeight { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Rendering.WriteableBitmapRenderer" /> class.
    /// </summary>
    public WriteableBitmapRenderer()
    {
      this.Foreground = Colors.Black;
      this.Background = Colors.White;
      this.FontFamily = WriteableBitmapRenderer.DefaultFontFamily;
      this.FontSize = 10.0;
      this.FontStretch = FontStretches.Normal;
      this.FontStyle = FontStyles.Normal;
      this.FontWeight = FontWeights.Normal;
    }

    /// <summary>Renders the specified matrix.</summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The content.</param>
    /// <returns></returns>
    public WriteableBitmap Render(BitMatrix matrix, BarcodeFormat format, string content)
    {
      return this.Render(matrix, format, content, (EncodingOptions) null);
    }

    /// <summary>Renders the specified matrix.</summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The content.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public virtual WriteableBitmap Render(
      BitMatrix matrix,
      BarcodeFormat format,
      string content,
      EncodingOptions options)
    {
      int width = matrix.Width;
      int height = matrix.Height;
      int num1 = (options == null || !options.PureBarcode) && !string.IsNullOrEmpty(content) && (format == BarcodeFormat.CODE_39 || format == BarcodeFormat.CODE_128 || format == BarcodeFormat.EAN_13 || format == BarcodeFormat.EAN_8 || format == BarcodeFormat.CODABAR || format == BarcodeFormat.ITF || format == BarcodeFormat.UPC_A || format == BarcodeFormat.MSI || format == BarcodeFormat.PLESSEY) ? 16 : 0;
      int num2 = 1;
      if (options != null)
      {
        if (options.Width > width)
          width = options.Width;
        if (options.Height > height)
          height = options.Height;
        num2 = width / matrix.Width;
        if (num2 > height / matrix.Height)
          num2 = height / matrix.Height;
      }
      int num3 = (int) this.Foreground.A << 24 | (int) this.Foreground.R << 16 | (int) this.Foreground.G << 8 | (int) this.Foreground.B;
      int num4 = (int) this.Background.A << 24 | (int) this.Background.R << 16 | (int) this.Background.G << 8 | (int) this.Background.B;
      WriteableBitmap writeableBitmap = new WriteableBitmap(width, height);
      int[] pixels = writeableBitmap.Pixels;
      int num5 = 0;
      for (int y = 0; y < matrix.Height - num1; ++y)
      {
        for (int index1 = 0; index1 < num2; ++index1)
        {
          for (int x = 0; x < matrix.Width; ++x)
          {
            int num6 = matrix[x, y] ? num3 : num4;
            for (int index2 = 0; index2 < num2; ++index2)
              pixels[num5++] = num6;
          }
          for (int index3 = num2 * matrix.Width; index3 < width; ++index3)
            pixels[num5++] = num4;
        }
      }
      for (int index4 = matrix.Height * num2 - num1; index4 < height; ++index4)
      {
        for (int index5 = 0; index5 < width; ++index5)
          pixels[num5++] = num4;
      }
      writeableBitmap.Invalidate();
      return writeableBitmap;
    }
  }
}
