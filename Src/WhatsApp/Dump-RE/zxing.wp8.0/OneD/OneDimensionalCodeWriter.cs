// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.OneDimensionalCodeWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  ///   <p>Encapsulates functionality and implementation that is common to one-dimensional barcodes.</p>
  ///   <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
  /// </summary>
  public abstract class OneDimensionalCodeWriter : Writer
  {
    /// <summary>Encode a barcode using the default settings.</summary>
    /// <param name="contents">The contents to encode in the barcode</param>
    /// <param name="format">The barcode format to generate</param>
    /// <param name="width">The preferred width in pixels</param>
    /// <param name="height">The preferred height in pixels</param>
    /// <returns>
    /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
    /// </returns>
    public BitMatrix encode(string contents, BarcodeFormat format, int width, int height)
    {
      return this.encode(contents, format, width, height, (IDictionary<EncodeHintType, object>) null);
    }

    /// <summary>
    /// Encode the contents following specified format.
    /// {@code width} and {@code height} are required size. This method may return bigger size
    /// {@code BitMatrix} when specified size is too small. The user can set both {@code width} and
    /// {@code height} to zero to get minimum size barcode. If negative value is set to {@code width}
    /// or {@code height}, {@code IllegalArgumentException} is thrown.
    /// </summary>
    public virtual BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (string.IsNullOrEmpty(contents))
        throw new ArgumentException("Found empty contents");
      if (width < 0 || height < 0)
        throw new ArgumentException("Negative size is not allowed. Input: " + (object) width + (object) 'x' + (object) height);
      int defaultMargin = this.DefaultMargin;
      if (hints != null)
      {
        int? nullable = hints.ContainsKey(EncodeHintType.MARGIN) ? new int?((int) hints[EncodeHintType.MARGIN]) : new int?();
        if (nullable.HasValue)
          defaultMargin = nullable.Value;
      }
      return OneDimensionalCodeWriter.renderResult(this.encode(contents), width, height, defaultMargin);
    }

    /// <summary>
    /// <returns>a byte array of horizontal pixels (0 = white, 1 = black)</returns>
    /// </summary>
    private static BitMatrix renderResult(bool[] code, int width, int height, int sidesMargin)
    {
      int length = code.Length;
      int val2 = length + sidesMargin;
      int width1 = Math.Max(width, val2);
      int height1 = Math.Max(1, height);
      int width2 = width1 / val2;
      int num = (width1 - length * width2) / 2;
      BitMatrix bitMatrix = new BitMatrix(width1, height1);
      int index = 0;
      int left = num;
      while (index < length)
      {
        if (code[index])
          bitMatrix.setRegion(left, 0, width2, height1);
        ++index;
        left += width2;
      }
      return bitMatrix;
    }

    /// <summary>
    /// Appends the given pattern to the target array starting at pos.
    /// 
    /// <param name="startColor">starting color - false for white, true for black</param>
    /// <returns>the number of elements added to target.</returns>
    /// </summary>
    protected static int appendPattern(bool[] target, int pos, int[] pattern, bool startColor)
    {
      bool flag = startColor;
      int num1 = 0;
      foreach (int num2 in pattern)
      {
        for (int index = 0; index < num2; ++index)
          target[pos++] = flag;
        num1 += num2;
        flag = !flag;
      }
      return num1;
    }

    /// <summary>Gets the default margin.</summary>
    public virtual int DefaultMargin => 10;

    /// <summary>
    /// Encode the contents to bool array expression of one-dimensional barcode.
    /// Start code and end code should be included in result, and side margins should not be included.
    /// 
    /// <returns>a {@code bool[]} of horizontal pixels (false = white, true = black)</returns>
    /// </summary>
    public abstract bool[] encode(string contents);

    /// <summary>Calculates the checksum digit modulo10.</summary>
    /// <param name="contents">The contents.</param>
    /// <returns></returns>
    public static string CalculateChecksumDigitModulo10(string contents)
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = contents.Length - 1; index >= 0; index -= 2)
        num1 += (int) contents[index] - 48;
      for (int index = contents.Length - 2; index >= 0; index -= 2)
        num2 += (int) contents[index] - 48;
      return contents + (object) ((10 - (num1 * 3 + num2) % 10) % 10);
    }
  }
}
