// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.PDF417Writer
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.PDF417.Internal;

#nullable disable
namespace ZXing.PDF417
{
  /// <summary>
  /// <author>Jacob Haynes</author>
  /// <author>qwandor@google.com (Andrew Walbran)</author>
  /// </summary>
  public sealed class PDF417Writer : Writer
  {
    /// <summary>default white space (margin) around the code</summary>
    private const int WHITE_SPACE = 30;

    /// <summary>
    /// </summary>
    /// <param name="contents">The contents to encode in the barcode</param>
    /// <param name="format">The barcode format to generate</param>
    /// <param name="width">The preferred width in pixels</param>
    /// <param name="height">The preferred height in pixels</param>
    /// <param name="hints">Additional parameters to supply to the encoder</param>
    /// <returns>
    /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
    /// </returns>
    public BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (format != BarcodeFormat.PDF_417)
        throw new ArgumentException("Can only encode PDF_417, but got " + (object) format);
      ZXing.PDF417.Internal.PDF417 encoder = new ZXing.PDF417.Internal.PDF417();
      int margin = 30;
      int errorCorrectionLevel = 2;
      if (hints != null)
      {
        if (hints.ContainsKey(EncodeHintType.PDF417_COMPACT))
          encoder.setCompact((bool) hints[EncodeHintType.PDF417_COMPACT]);
        if (hints.ContainsKey(EncodeHintType.PDF417_COMPACTION))
          encoder.setCompaction((Compaction) hints[EncodeHintType.PDF417_COMPACTION]);
        if (hints.ContainsKey(EncodeHintType.PDF417_DIMENSIONS))
        {
          Dimensions hint = (Dimensions) hints[EncodeHintType.PDF417_DIMENSIONS];
          encoder.setDimensions(hint.MaxCols, hint.MinCols, hint.MaxRows, hint.MinRows);
        }
        if (hints.ContainsKey(EncodeHintType.MARGIN))
          margin = (int) hints[EncodeHintType.MARGIN];
        if (hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
        {
          object hint = hints[EncodeHintType.ERROR_CORRECTION];
          if (hint is PDF417ErrorCorrectionLevel || hint is int)
            errorCorrectionLevel = (int) hint;
        }
        if (hints.ContainsKey(EncodeHintType.CHARACTER_SET))
        {
          string hint = (string) hints[EncodeHintType.CHARACTER_SET];
          if (hint != null)
            encoder.setEncoding(hint);
        }
        if (hints.ContainsKey(EncodeHintType.DISABLE_ECI))
          encoder.setDisableEci((bool) hints[EncodeHintType.DISABLE_ECI]);
      }
      return PDF417Writer.bitMatrixFromEncoder(encoder, contents, width, height, margin, errorCorrectionLevel);
    }

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
    /// Takes encoder, accounts for width/height, and retrieves bit matrix
    /// </summary>
    private static BitMatrix bitMatrixFromEncoder(
      ZXing.PDF417.Internal.PDF417 encoder,
      string contents,
      int width,
      int height,
      int margin,
      int errorCorrectionLevel)
    {
      encoder.generateBarcodeLogic(contents, errorCorrectionLevel);
      sbyte[][] numArray1 = encoder.BarcodeMatrix.getScaledMatrix(2, 8);
      bool flag = false;
      if (height > width ^ numArray1[0].Length < numArray1.Length)
      {
        numArray1 = PDF417Writer.rotateArray(numArray1);
        flag = true;
      }
      int num1 = width / numArray1[0].Length;
      int num2 = height / numArray1.Length;
      int num3 = num1 >= num2 ? num2 : num1;
      if (num3 <= 1)
        return PDF417Writer.bitMatrixFrombitArray(numArray1, margin);
      sbyte[][] numArray2 = encoder.BarcodeMatrix.getScaledMatrix(num3 * 2, num3 * 4 * 2);
      if (flag)
        numArray2 = PDF417Writer.rotateArray(numArray2);
      return PDF417Writer.bitMatrixFrombitArray(numArray2, margin);
    }

    /// <summary>This takes an array holding the values of the PDF 417</summary>
    /// <param name="input">a byte array of information with 0 is black, and 1 is white</param>
    /// <param name="margin">border around the barcode</param>
    /// <returns>BitMatrix of the input</returns>
    private static BitMatrix bitMatrixFrombitArray(sbyte[][] input, int margin)
    {
      BitMatrix bitMatrix = new BitMatrix(input[0].Length + 2 * margin, input.Length + 2 * margin);
      int y = bitMatrix.Height - margin - 1;
      for (int index1 = 0; index1 < input.Length; ++index1)
      {
        sbyte[] numArray = input[index1];
        int length = numArray.Length;
        for (int index2 = 0; index2 < length; ++index2)
        {
          if (numArray[index2] == (sbyte) 1)
            bitMatrix[index2 + margin, y] = true;
        }
        --y;
      }
      return bitMatrix;
    }

    /// <summary>Takes and rotates the it 90 degrees</summary>
    private static sbyte[][] rotateArray(sbyte[][] bitarray)
    {
      sbyte[][] numArray = new sbyte[bitarray[0].Length][];
      for (int index = 0; index < bitarray[0].Length; ++index)
        numArray[index] = new sbyte[bitarray.Length];
      for (int index1 = 0; index1 < bitarray.Length; ++index1)
      {
        int index2 = bitarray.Length - index1 - 1;
        for (int index3 = 0; index3 < bitarray[0].Length; ++index3)
          numArray[index3][index2] = bitarray[index1][index3];
      }
      return numArray;
    }
  }
}
