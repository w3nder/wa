// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.DataMatrixWriter
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Datamatrix.Encoder;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.Datamatrix
{
  /// <summary>
  /// This object renders a Data Matrix code as a BitMatrix 2D array of greyscale values.
  /// </summary>
  /// <author>dswitkin@google.com (Daniel Switkin)</author>
  /// <author>Guillaume Le Biller Added to zxing lib.</author>
  public sealed class DataMatrixWriter : Writer
  {
    public BitMatrix encode(string contents, BarcodeFormat format, int width, int height)
    {
      return this.encode(contents, format, width, height, (IDictionary<EncodeHintType, object>) null);
    }

    public BitMatrix encode(
      string contents,
      BarcodeFormat format,
      int width,
      int height,
      IDictionary<EncodeHintType, object> hints)
    {
      if (string.IsNullOrEmpty(contents))
        throw new ArgumentException("Found empty contents", contents);
      if (format != BarcodeFormat.DATA_MATRIX)
        throw new ArgumentException("Can only encode DATA_MATRIX, but got " + (object) format);
      if (width < 0 || height < 0)
        throw new ArgumentException("Requested dimensions are too small: " + (object) width + (object) 'x' + (object) height);
      SymbolShapeHint shape = SymbolShapeHint.FORCE_NONE;
      int defaultEncodation = 0;
      Dimension minSize = (Dimension) null;
      Dimension maxSize = (Dimension) null;
      if (hints != null)
      {
        SymbolShapeHint? nullable1 = hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE) ? (SymbolShapeHint?) hints[EncodeHintType.DATA_MATRIX_SHAPE] : new SymbolShapeHint?();
        if (nullable1.HasValue)
          shape = nullable1.Value;
        Dimension hint1 = hints.ContainsKey(EncodeHintType.MIN_SIZE) ? (Dimension) hints[EncodeHintType.MIN_SIZE] : (Dimension) null;
        if (hint1 != null)
          minSize = hint1;
        Dimension hint2 = hints.ContainsKey(EncodeHintType.MAX_SIZE) ? (Dimension) hints[EncodeHintType.MAX_SIZE] : (Dimension) null;
        if (hint2 != null)
          maxSize = hint2;
        int? nullable2 = hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION) ? (int?) hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION] : new int?();
        if (nullable2.HasValue)
          defaultEncodation = nullable2.Value;
      }
      string codewords = HighLevelEncoder.encodeHighLevel(contents, shape, minSize, maxSize, defaultEncodation);
      SymbolInfo symbolInfo = SymbolInfo.lookup(codewords.Length, shape, minSize, maxSize, true);
      DefaultPlacement placement = new DefaultPlacement(ErrorCorrection.encodeECC200(codewords, symbolInfo), symbolInfo.getSymbolDataWidth(), symbolInfo.getSymbolDataHeight());
      placement.place();
      return DataMatrixWriter.encodeLowLevel(placement, symbolInfo);
    }

    /// <summary>Encode the given symbol info to a bit matrix.</summary>
    /// <param name="placement">The DataMatrix placement.</param>
    /// <param name="symbolInfo">The symbol info to encode.</param>
    /// <returns>The bit matrix generated.</returns>
    private static BitMatrix encodeLowLevel(DefaultPlacement placement, SymbolInfo symbolInfo)
    {
      int symbolDataWidth = symbolInfo.getSymbolDataWidth();
      int symbolDataHeight = symbolInfo.getSymbolDataHeight();
      ByteMatrix matrix = new ByteMatrix(symbolInfo.getSymbolWidth(), symbolInfo.getSymbolHeight());
      int y = 0;
      for (int row = 0; row < symbolDataHeight; ++row)
      {
        if (row % symbolInfo.matrixHeight == 0)
        {
          int x = 0;
          for (int index = 0; index < symbolInfo.getSymbolWidth(); ++index)
          {
            matrix.set(x, y, index % 2 == 0);
            ++x;
          }
          ++y;
        }
        int x1 = 0;
        for (int col = 0; col < symbolDataWidth; ++col)
        {
          if (col % symbolInfo.matrixWidth == 0)
          {
            matrix.set(x1, y, true);
            ++x1;
          }
          matrix.set(x1, y, placement.getBit(col, row));
          ++x1;
          if (col % symbolInfo.matrixWidth == symbolInfo.matrixWidth - 1)
          {
            matrix.set(x1, y, row % 2 == 0);
            ++x1;
          }
        }
        ++y;
        if (row % symbolInfo.matrixHeight == symbolInfo.matrixHeight - 1)
        {
          int x2 = 0;
          for (int index = 0; index < symbolInfo.getSymbolWidth(); ++index)
          {
            matrix.set(x2, y, true);
            ++x2;
          }
          ++y;
        }
      }
      return DataMatrixWriter.convertByteMatrixToBitMatrix(matrix);
    }

    /// <summary>Convert the ByteMatrix to BitMatrix.</summary>
    /// <param name="matrix">The input matrix.</param>
    /// <returns>The output matrix.</returns>
    private static BitMatrix convertByteMatrixToBitMatrix(ByteMatrix matrix)
    {
      int width = matrix.Width;
      int height = matrix.Height;
      BitMatrix bitMatrix = new BitMatrix(width, height);
      bitMatrix.clear();
      for (int x = 0; x < width; ++x)
      {
        for (int y = 0; y < height; ++y)
        {
          if (matrix[x, y] == 1)
            bitMatrix[x, y] = true;
        }
      }
      return bitMatrix;
    }
  }
}
