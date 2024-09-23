// Decompiled with JetBrains decompiler
// Type: ZXing.Rendering.SvgRenderer
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using ZXing.Common;

#nullable disable
namespace ZXing.Rendering
{
  /// <summary>Renders a barcode into a Svg image</summary>
  public class SvgRenderer : IBarcodeRenderer<SvgRenderer.SvgImage>
  {
    /// <summary>Gets or sets the foreground color.</summary>
    /// <value>The foreground color.</value>
    public Color Foreground { get; set; }

    /// <summary>Gets or sets the background color.</summary>
    /// <value>The background color.</value>
    public Color Background { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Rendering.SvgRenderer" /> class.
    /// </summary>
    public SvgRenderer()
    {
      this.Foreground = Colors.Black;
      this.Background = Colors.White;
    }

    /// <summary>Renders the specified matrix.</summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The content.</param>
    /// <returns></returns>
    public SvgRenderer.SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content)
    {
      return this.Render(matrix, format, content, (EncodingOptions) null);
    }

    /// <summary>Renders the specified matrix.</summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The content.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    public SvgRenderer.SvgImage Render(
      BitMatrix matrix,
      BarcodeFormat format,
      string content,
      EncodingOptions options)
    {
      SvgRenderer.SvgImage image = new SvgRenderer.SvgImage();
      this.Create(image, matrix, format, content, options);
      return image;
    }

    private void Create(
      SvgRenderer.SvgImage image,
      BitMatrix matrix,
      BarcodeFormat format,
      string content,
      EncodingOptions options)
    {
      if (matrix == null)
        return;
      int width = matrix.Width;
      int height = matrix.Height;
      image.AddHeader();
      image.AddTag(0, 0, 10 + width, 10 + height, this.Background, this.Foreground);
      SvgRenderer.AppendDarkCell(image, matrix, 5, 5);
      image.AddEnd();
    }

    private static void AppendDarkCell(
      SvgRenderer.SvgImage image,
      BitMatrix matrix,
      int offsetX,
      int offSetY)
    {
      if (matrix == null)
        return;
      int width = matrix.Width;
      int height = matrix.Height;
      BitMatrix processed = new BitMatrix(width, height);
      bool flag = false;
      int startPosX = 0;
      int startPosY = 0;
      for (int x = 0; x < width; ++x)
      {
        int endPosX;
        for (int index = 0; index < height; ++index)
        {
          if (!processed[x, index])
          {
            processed[x, index] = true;
            if (matrix[x, index])
            {
              if (!flag)
              {
                startPosX = x;
                startPosY = index;
                flag = true;
              }
            }
            else if (flag)
            {
              SvgRenderer.FindMaximumRectangle(matrix, processed, startPosX, startPosY, index, out endPosX);
              image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, index - startPosY);
              flag = false;
            }
          }
        }
        if (flag)
        {
          SvgRenderer.FindMaximumRectangle(matrix, processed, startPosX, startPosY, height, out endPosX);
          image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, height - startPosY);
          flag = false;
        }
      }
    }

    private static void FindMaximumRectangle(
      BitMatrix matrix,
      BitMatrix processed,
      int startPosX,
      int startPosY,
      int endPosY,
      out int endPosX)
    {
      endPosX = startPosX + 1;
      for (int x = startPosX + 1; x < matrix.Width; ++x)
      {
        for (int y = startPosY; y < endPosY; ++y)
        {
          if (!matrix[x, y])
            return;
        }
        endPosX = x;
        for (int y = startPosY; y < endPosY; ++y)
          processed[x, y] = true;
      }
    }

    /// <summary>Represents a barcode as a Svg image</summary>
    public class SvgImage
    {
      private readonly StringBuilder content;

      /// <summary>Gets or sets the content.</summary>
      /// <value>The content.</value>
      public string Content
      {
        get => this.content.ToString();
        set
        {
          this.content.Length = 0;
          if (value == null)
            return;
          this.content.Append(value);
        }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="T:ZXing.Rendering.SvgRenderer.SvgImage" /> class.
      /// </summary>
      public SvgImage() => this.content = new StringBuilder();

      /// <summary>
      /// Initializes a new instance of the <see cref="T:ZXing.Rendering.SvgRenderer.SvgImage" /> class.
      /// </summary>
      /// <param name="content">The content.</param>
      public SvgImage(string content) => this.content = new StringBuilder(content);

      /// <summary>Gives the XML representation of the SVG image</summary>
      public override string ToString() => this.content.ToString();

      internal void AddHeader()
      {
        this.content.Append("<?xml version=\"1.0\" standalone=\"no\"?>");
        this.content.Append("<!-- Created with ZXing.Net (http://zxingnet.codeplex.com/) -->");
        this.content.Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
      }

      internal void AddEnd() => this.content.Append("</svg>");

      internal void AddTag(
        int displaysizeX,
        int displaysizeY,
        int viewboxSizeX,
        int viewboxSizeY,
        Color background,
        Color fill)
      {
        if (displaysizeX <= 0 || displaysizeY <= 0)
          this.content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6}>", (object) viewboxSizeX, (object) viewboxSizeY, (object) SvgRenderer.SvgImage.GetColorRgb(background), (object) SvgRenderer.SvgImage.ConvertAlpha(background), (object) SvgRenderer.SvgImage.GetColorRgb(fill), (object) SvgRenderer.SvgImage.ConvertAlpha(fill), (object) SvgRenderer.SvgImage.GetBackgroundStyle(background)));
        else
          this.content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6} width=\"{7}\" height=\"{8}\">", (object) viewboxSizeX, (object) viewboxSizeY, (object) SvgRenderer.SvgImage.GetColorRgb(background), (object) SvgRenderer.SvgImage.ConvertAlpha(background), (object) SvgRenderer.SvgImage.GetColorRgb(fill), (object) SvgRenderer.SvgImage.ConvertAlpha(fill), (object) SvgRenderer.SvgImage.GetBackgroundStyle(background), (object) displaysizeX, (object) displaysizeY));
      }

      internal void AddRec(int posX, int posY, int width, int height)
      {
        this.content.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\"/>", (object) posX, (object) posY, (object) width, (object) height);
      }

      internal static double ConvertAlpha(Color alpha)
      {
        return Math.Round((double) alpha.A / (double) byte.MaxValue, 2);
      }

      internal static string GetBackgroundStyle(Color color)
      {
        double num = SvgRenderer.SvgImage.ConvertAlpha(color);
        return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"", (object) color.R, (object) color.G, (object) color.B, (object) num);
      }

      internal static string GetColorRgb(Color color)
      {
        return color.R.ToString() + "," + (object) color.G + "," + (object) color.B;
      }

      internal static string GetColorRgba(Color color)
      {
        double num = SvgRenderer.SvgImage.ConvertAlpha(color);
        return color.R.ToString() + "," + (object) color.G + "," + (object) color.B + "," + (object) num;
      }
    }
  }
}
