// Decompiled with JetBrains decompiler
// Type: ZXing.BarcodeWriterGeneric`1
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;
using ZXing.Rendering;

#nullable disable
namespace ZXing
{
  /// <summary>
  /// A base class for specific barcode writers with specific formats of barcode images.
  /// </summary>
  /// <typeparam name="TOutput">The type of the output.</typeparam>
  public class BarcodeWriterGeneric<TOutput> : IBarcodeWriterGeneric<TOutput>
  {
    private EncodingOptions options;

    /// <summary>
    /// Gets or sets the barcode format.
    /// The value is only suitable if the MultiFormatWriter is used.
    /// </summary>
    public BarcodeFormat Format { get; set; }

    /// <summary>
    /// Gets or sets the options container for the encoding and renderer process.
    /// </summary>
    public EncodingOptions Options
    {
      get
      {
        EncodingOptions options = this.options;
        if (options != null)
          return options;
        return this.options = new EncodingOptions()
        {
          Height = 100,
          Width = 100
        };
      }
      set => this.options = value;
    }

    /// <summary>
    /// Gets or sets the writer which encodes the content to a BitMatrix.
    /// If no value is set the MultiFormatWriter is used.
    /// </summary>
    public Writer Encoder { get; set; }

    /// <summary>
    /// Gets or sets the renderer which should be used to render the encoded BitMatrix.
    /// </summary>
    public IBarcodeRenderer<TOutput> Renderer { get; set; }

    /// <summary>
    /// Encodes the specified contents and returns a BitMatrix array.
    /// That array has to be rendered manually or with a IBarcodeRenderer.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <returns></returns>
    public BitMatrix Encode(string contents)
    {
      Writer writer = this.Encoder ?? (Writer) new MultiFormatWriter();
      EncodingOptions options = this.Options;
      return writer.encode(contents, this.Format, options.Width, options.Height, options.Hints);
    }

    /// <summary>
    /// Encodes the specified contents and returns a rendered instance of the barcode.
    /// For rendering the instance of the property Renderer is used and has to be set before
    /// calling that method.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <returns></returns>
    public TOutput Write(string contents)
    {
      if (this.Renderer == null)
        throw new InvalidOperationException("You have to set a renderer instance.");
      return this.Renderer.Render(this.Encode(contents), this.Format, contents, this.Options);
    }

    /// <summary>
    /// Returns a rendered instance of the barcode which is given by a BitMatrix.
    /// For rendering the instance of the property Renderer is used and has to be set before
    /// calling that method.
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <returns></returns>
    public TOutput Write(BitMatrix matrix)
    {
      if (this.Renderer == null)
        throw new InvalidOperationException("You have to set a renderer instance.");
      return this.Renderer.Render(matrix, this.Format, (string) null, this.Options);
    }
  }
}
