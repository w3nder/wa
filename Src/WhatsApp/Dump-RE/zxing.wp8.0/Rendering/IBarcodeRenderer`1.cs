// Decompiled with JetBrains decompiler
// Type: ZXing.Rendering.IBarcodeRenderer`1
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Rendering
{
  /// <summary>
  /// Interface for a class to convert a BitMatrix to an output image format
  /// </summary>
  public interface IBarcodeRenderer<TOutput>
  {
    /// <summary>
    /// Renders the specified matrix to its graphically representation
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The encoded content of the barcode which should be included in the image.
    /// That can be the numbers below a 1D barcode or something other.</param>
    /// <returns></returns>
    TOutput Render(BitMatrix matrix, BarcodeFormat format, string content);

    /// <summary>
    /// Renders the specified matrix to its graphically representation
    /// </summary>
    /// <param name="matrix">The matrix.</param>
    /// <param name="format">The format.</param>
    /// <param name="content">The encoded content of the barcode which should be included in the image.
    /// That can be the numbers below a 1D barcode or something other.</param>
    /// <param name="options">The options.</param>
    /// <returns></returns>
    TOutput Render(
      BitMatrix matrix,
      BarcodeFormat format,
      string content,
      EncodingOptions options);
  }
}
