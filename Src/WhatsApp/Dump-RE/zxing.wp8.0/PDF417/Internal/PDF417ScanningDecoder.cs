// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.PDF417ScanningDecoder
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ZXing.Common;
using ZXing.PDF417.Internal.EC;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>Guenther Grau</author>
  public static class PDF417ScanningDecoder
  {
    private const int CODEWORD_SKEW_SIZE = 2;
    private const int MAX_ERRORS = 3;
    private const int MAX_EC_CODEWORDS = 512;
    private static readonly ErrorCorrection errorCorrection = new ErrorCorrection();

    /// <summary>
    /// Decode the specified image, imageTopLeft, imageBottomLeft, imageTopRight, imageBottomRight, minCodewordWidth
    /// and maxCodewordWidth.
    /// TODO: don't pass in minCodewordWidth and maxCodewordWidth, pass in barcode columns for start and stop pattern
    /// columns. That way width can be deducted from the pattern column.
    /// This approach also allows to detect more details about the barcode, e.g. if a bar type (white or black) is wider
    /// than it should be. This can happen if the scanner used a bad blackpoint.
    /// </summary>
    /// <param name="image">Image.</param>
    /// <param name="imageTopLeft">Image top left.</param>
    /// <param name="imageBottomLeft">Image bottom left.</param>
    /// <param name="imageTopRight">Image top right.</param>
    /// <param name="imageBottomRight">Image bottom right.</param>
    /// <param name="minCodewordWidth">Minimum codeword width.</param>
    /// <param name="maxCodewordWidth">Max codeword width.</param>
    public static DecoderResult decode(
      BitMatrix image,
      ResultPoint imageTopLeft,
      ResultPoint imageBottomLeft,
      ResultPoint imageTopRight,
      ResultPoint imageBottomRight,
      int minCodewordWidth,
      int maxCodewordWidth)
    {
      BoundingBox box = BoundingBox.Create(image, imageTopLeft, imageBottomLeft, imageTopRight, imageBottomRight);
      if (box == null)
        return (DecoderResult) null;
      DetectionResultRowIndicatorColumn leftRowIndicatorColumn = (DetectionResultRowIndicatorColumn) null;
      DetectionResultRowIndicatorColumn rightRowIndicatorColumn = (DetectionResultRowIndicatorColumn) null;
      DetectionResult detectionResult = (DetectionResult) null;
      for (int index = 0; index < 2; ++index)
      {
        if (imageTopLeft != null)
          leftRowIndicatorColumn = PDF417ScanningDecoder.getRowIndicatorColumn(image, box, imageTopLeft, true, minCodewordWidth, maxCodewordWidth);
        if (imageTopRight != null)
          rightRowIndicatorColumn = PDF417ScanningDecoder.getRowIndicatorColumn(image, box, imageTopRight, false, minCodewordWidth, maxCodewordWidth);
        detectionResult = PDF417ScanningDecoder.merge(leftRowIndicatorColumn, rightRowIndicatorColumn);
        if (detectionResult == null)
          return (DecoderResult) null;
        if (index == 0 && detectionResult.Box != null && (detectionResult.Box.MinY < box.MinY || detectionResult.Box.MaxY > box.MaxY))
        {
          box = detectionResult.Box;
        }
        else
        {
          detectionResult.Box = box;
          break;
        }
      }
      int index1 = detectionResult.ColumnCount + 1;
      detectionResult.DetectionResultColumns[0] = (DetectionResultColumn) leftRowIndicatorColumn;
      detectionResult.DetectionResultColumns[index1] = (DetectionResultColumn) rightRowIndicatorColumn;
      bool leftToRight = leftRowIndicatorColumn != null;
      for (int index2 = 1; index2 <= index1; ++index2)
      {
        int barcodeColumn = leftToRight ? index2 : index1 - index2;
        if (detectionResult.DetectionResultColumns[barcodeColumn] == null)
        {
          DetectionResultColumn detectionResultColumn = barcodeColumn == 0 || barcodeColumn == index1 ? (DetectionResultColumn) new DetectionResultRowIndicatorColumn(box, barcodeColumn == 0) : new DetectionResultColumn(box);
          detectionResult.DetectionResultColumns[barcodeColumn] = detectionResultColumn;
          int num = -1;
          for (int minY = box.MinY; minY <= box.MaxY; ++minY)
          {
            int startColumn = PDF417ScanningDecoder.getStartColumn(detectionResult, barcodeColumn, minY, leftToRight);
            if (startColumn < 0 || startColumn > box.MaxX)
            {
              if (num != -1)
                startColumn = num;
              else
                continue;
            }
            Codeword codeword = PDF417ScanningDecoder.detectCodeword(image, box.MinX, box.MaxX, leftToRight, startColumn, minY, minCodewordWidth, maxCodewordWidth);
            if (codeword != null)
            {
              detectionResultColumn.setCodeword(minY, codeword);
              num = startColumn;
              minCodewordWidth = Math.Min(minCodewordWidth, codeword.Width);
              maxCodewordWidth = Math.Max(maxCodewordWidth, codeword.Width);
            }
          }
        }
      }
      return PDF417ScanningDecoder.createDecoderResult(detectionResult);
    }

    /// <summary>
    /// Merge the specified leftRowIndicatorColumn and rightRowIndicatorColumn.
    /// </summary>
    /// <param name="leftRowIndicatorColumn">Left row indicator column.</param>
    /// <param name="rightRowIndicatorColumn">Right row indicator column.</param>
    private static DetectionResult merge(
      DetectionResultRowIndicatorColumn leftRowIndicatorColumn,
      DetectionResultRowIndicatorColumn rightRowIndicatorColumn)
    {
      if (leftRowIndicatorColumn == null && rightRowIndicatorColumn == null)
        return (DetectionResult) null;
      BarcodeMetadata barcodeMetadata = PDF417ScanningDecoder.getBarcodeMetadata(leftRowIndicatorColumn, rightRowIndicatorColumn);
      if (barcodeMetadata == null)
        return (DetectionResult) null;
      BoundingBox box = BoundingBox.merge(PDF417ScanningDecoder.adjustBoundingBox(leftRowIndicatorColumn), PDF417ScanningDecoder.adjustBoundingBox(rightRowIndicatorColumn));
      return new DetectionResult(barcodeMetadata, box);
    }

    /// <summary>Adjusts the bounding box.</summary>
    /// <returns>The bounding box.</returns>
    /// <param name="rowIndicatorColumn">Row indicator column.</param>
    private static BoundingBox adjustBoundingBox(
      DetectionResultRowIndicatorColumn rowIndicatorColumn)
    {
      if (rowIndicatorColumn == null)
        return (BoundingBox) null;
      int[] rowHeights = rowIndicatorColumn.getRowHeights();
      if (rowHeights == null)
        return (BoundingBox) null;
      int max = PDF417ScanningDecoder.getMax(rowHeights);
      int missingStartRows = 0;
      foreach (int num in rowHeights)
      {
        missingStartRows += max - num;
        if (num > 0)
          break;
      }
      Codeword[] codewords = rowIndicatorColumn.Codewords;
      for (int index = 0; missingStartRows > 0 && codewords[index] == null; ++index)
        --missingStartRows;
      int missingEndRows = 0;
      for (int index = rowHeights.Length - 1; index >= 0; --index)
      {
        missingEndRows += max - rowHeights[index];
        if (rowHeights[index] > 0)
          break;
      }
      for (int index = codewords.Length - 1; missingEndRows > 0 && codewords[index] == null; --index)
        --missingEndRows;
      return rowIndicatorColumn.Box.addMissingRows(missingStartRows, missingEndRows, rowIndicatorColumn.IsLeft);
    }

    private static int getMax(int[] values)
    {
      int val1 = -1;
      for (int index = values.Length - 1; index >= 0; --index)
        val1 = Math.Max(val1, values[index]);
      return val1;
    }

    /// <summary>Gets the barcode metadata.</summary>
    /// <returns>The barcode metadata.</returns>
    /// <param name="leftRowIndicatorColumn">Left row indicator column.</param>
    /// <param name="rightRowIndicatorColumn">Right row indicator column.</param>
    private static BarcodeMetadata getBarcodeMetadata(
      DetectionResultRowIndicatorColumn leftRowIndicatorColumn,
      DetectionResultRowIndicatorColumn rightRowIndicatorColumn)
    {
      BarcodeMetadata barcodeMetadata1;
      BarcodeMetadata barcodeMetadata2;
      return leftRowIndicatorColumn == null || (barcodeMetadata1 = leftRowIndicatorColumn.getBarcodeMetadata()) == null ? rightRowIndicatorColumn?.getBarcodeMetadata() : (rightRowIndicatorColumn == null || (barcodeMetadata2 = rightRowIndicatorColumn.getBarcodeMetadata()) == null || barcodeMetadata1.ColumnCount == barcodeMetadata2.ColumnCount || barcodeMetadata1.ErrorCorrectionLevel == barcodeMetadata2.ErrorCorrectionLevel || barcodeMetadata1.RowCount == barcodeMetadata2.RowCount ? barcodeMetadata1 : (BarcodeMetadata) null);
    }

    /// <summary>Gets the row indicator column.</summary>
    /// <returns>The row indicator column.</returns>
    /// <param name="image">Image.</param>
    /// <param name="boundingBox">Bounding box.</param>
    /// <param name="startPoint">Start point.</param>
    /// <param name="leftToRight">If set to <c>true</c> left to right.</param>
    /// <param name="minCodewordWidth">Minimum codeword width.</param>
    /// <param name="maxCodewordWidth">Max codeword width.</param>
    private static DetectionResultRowIndicatorColumn getRowIndicatorColumn(
      BitMatrix image,
      BoundingBox boundingBox,
      ResultPoint startPoint,
      bool leftToRight,
      int minCodewordWidth,
      int maxCodewordWidth)
    {
      DetectionResultRowIndicatorColumn rowIndicatorColumn = new DetectionResultRowIndicatorColumn(boundingBox, leftToRight);
      for (int index = 0; index < 2; ++index)
      {
        int num = index == 0 ? 1 : -1;
        int startColumn = (int) startPoint.X;
        for (int y = (int) startPoint.Y; y <= boundingBox.MaxY && y >= boundingBox.MinY; y += num)
        {
          Codeword codeword = PDF417ScanningDecoder.detectCodeword(image, 0, image.Width, leftToRight, startColumn, y, minCodewordWidth, maxCodewordWidth);
          if (codeword != null)
          {
            rowIndicatorColumn.setCodeword(y, codeword);
            startColumn = !leftToRight ? codeword.EndX : codeword.StartX;
          }
        }
      }
      return rowIndicatorColumn;
    }

    /// <summary>Adjusts the codeword count.</summary>
    /// <param name="detectionResult">Detection result.</param>
    /// <param name="barcodeMatrix">Barcode matrix.</param>
    private static bool adjustCodewordCount(
      DetectionResult detectionResult,
      BarcodeValue[][] barcodeMatrix)
    {
      int[] numArray = barcodeMatrix[0][1].getValue();
      int num = detectionResult.ColumnCount * detectionResult.RowCount - PDF417ScanningDecoder.getNumberOfECCodeWords(detectionResult.ErrorCorrectionLevel);
      if (numArray.Length == 0)
      {
        if (num < 1 || num > PDF417Common.MAX_CODEWORDS_IN_BARCODE)
          return false;
        barcodeMatrix[0][1].setValue(num);
      }
      else if (numArray[0] != num)
        barcodeMatrix[0][1].setValue(num);
      return true;
    }

    /// <summary>Creates the decoder result.</summary>
    /// <returns>The decoder result.</returns>
    /// <param name="detectionResult">Detection result.</param>
    private static DecoderResult createDecoderResult(DetectionResult detectionResult)
    {
      BarcodeValue[][] barcodeMatrix = PDF417ScanningDecoder.createBarcodeMatrix(detectionResult);
      if (!PDF417ScanningDecoder.adjustCodewordCount(detectionResult, barcodeMatrix))
        return (DecoderResult) null;
      List<int> intList1 = new List<int>();
      int[] codewords = new int[detectionResult.RowCount * detectionResult.ColumnCount];
      List<int[]> numArrayList = new List<int[]>();
      List<int> intList2 = new List<int>();
      for (int index1 = 0; index1 < detectionResult.RowCount; ++index1)
      {
        for (int index2 = 0; index2 < detectionResult.ColumnCount; ++index2)
        {
          int[] numArray = barcodeMatrix[index1][index2 + 1].getValue();
          int index3 = index1 * detectionResult.ColumnCount + index2;
          if (numArray.Length == 0)
            intList1.Add(index3);
          else if (numArray.Length == 1)
          {
            codewords[index3] = numArray[0];
          }
          else
          {
            intList2.Add(index3);
            numArrayList.Add(numArray);
          }
        }
      }
      int[][] ambiguousIndexValues = new int[numArrayList.Count][];
      for (int index = 0; index < ambiguousIndexValues.Length; ++index)
        ambiguousIndexValues[index] = numArrayList[index];
      return PDF417ScanningDecoder.createDecoderResultFromAmbiguousValues(detectionResult.ErrorCorrectionLevel, codewords, intList1.ToArray(), intList2.ToArray(), ambiguousIndexValues);
    }

    /// <summary>
    /// This method deals with the fact, that the decoding process doesn't always yield a single most likely value. The
    /// current error correction implementation doesn't deal with erasures very well, so it's better to provide a value
    /// for these ambiguous codewords instead of treating it as an erasure. The problem is that we don't know which of
    /// the ambiguous values to choose. We try decode using the first value, and if that fails, we use another of the
    /// ambiguous values and try to decode again. This usually only happens on very hard to read and decode barcodes,
    /// so decoding the normal barcodes is not affected by this.
    /// </summary>
    /// <returns>The decoder result from ambiguous values.</returns>
    /// <param name="ecLevel">Ec level.</param>
    /// <param name="codewords">Codewords.</param>
    /// <param name="erasureArray">contains the indexes of erasures.</param>
    /// <param name="ambiguousIndexes">array with the indexes that have more than one most likely value.</param>
    /// <param name="ambiguousIndexValues">two dimensional array that contains the ambiguous values. The first dimension must
    /// be the same Length as the ambiguousIndexes array.</param>
    private static DecoderResult createDecoderResultFromAmbiguousValues(
      int ecLevel,
      int[] codewords,
      int[] erasureArray,
      int[] ambiguousIndexes,
      int[][] ambiguousIndexValues)
    {
      int[] numArray = new int[ambiguousIndexes.Length];
      int num = 100;
      while (num-- > 0)
      {
        for (int index = 0; index < numArray.Length; ++index)
          codewords[ambiguousIndexes[index]] = ambiguousIndexValues[index][numArray[index]];
        try
        {
          DecoderResult fromAmbiguousValues = PDF417ScanningDecoder.decodeCodewords(codewords, ecLevel, erasureArray);
          if (fromAmbiguousValues != null)
            return fromAmbiguousValues;
        }
        catch (ReaderException ex)
        {
        }
        if (numArray.Length == 0)
          return (DecoderResult) null;
        for (int index = 0; index < numArray.Length; ++index)
        {
          if (numArray[index] < ambiguousIndexValues[index].Length - 1)
          {
            ++numArray[index];
            break;
          }
          numArray[index] = 0;
          if (index == numArray.Length - 1)
            return (DecoderResult) null;
        }
      }
      return (DecoderResult) null;
    }

    /// <summary>Creates the barcode matrix.</summary>
    /// <returns>The barcode matrix.</returns>
    /// <param name="detectionResult">Detection result.</param>
    private static BarcodeValue[][] createBarcodeMatrix(DetectionResult detectionResult)
    {
      BarcodeValue[][] barcodeMatrix = new BarcodeValue[detectionResult.RowCount][];
      for (int index1 = 0; index1 < barcodeMatrix.Length; ++index1)
      {
        barcodeMatrix[index1] = new BarcodeValue[detectionResult.ColumnCount + 2];
        for (int index2 = 0; index2 < barcodeMatrix[index1].Length; ++index2)
          barcodeMatrix[index1][index2] = new BarcodeValue();
      }
      int index = -1;
      foreach (DetectionResultColumn detectionResultColumn in detectionResult.getDetectionResultColumns())
      {
        ++index;
        if (detectionResultColumn != null)
        {
          foreach (Codeword codeword in detectionResultColumn.Codewords)
          {
            if (codeword != null && codeword.RowNumber != -1)
              barcodeMatrix[codeword.RowNumber][index].setValue(codeword.Value);
          }
        }
      }
      return barcodeMatrix;
    }

    /// <summary>Tests to see if the Barcode Column is Valid</summary>
    /// <returns><c>true</c>, if barcode column is valid, <c>false</c> otherwise.</returns>
    /// <param name="detectionResult">Detection result.</param>
    /// <param name="barcodeColumn">Barcode column.</param>
    private static bool isValidBarcodeColumn(DetectionResult detectionResult, int barcodeColumn)
    {
      return barcodeColumn >= 0 && barcodeColumn <= detectionResult.DetectionResultColumns.Length + 1;
    }

    /// <summary>Gets the start column.</summary>
    /// <returns>The start column.</returns>
    /// <param name="detectionResult">Detection result.</param>
    /// <param name="barcodeColumn">Barcode column.</param>
    /// <param name="imageRow">Image row.</param>
    /// <param name="leftToRight">If set to <c>true</c> left to right.</param>
    private static int getStartColumn(
      DetectionResult detectionResult,
      int barcodeColumn,
      int imageRow,
      bool leftToRight)
    {
      int num1 = leftToRight ? 1 : -1;
      Codeword codeword1 = (Codeword) null;
      if (PDF417ScanningDecoder.isValidBarcodeColumn(detectionResult, barcodeColumn - num1))
        codeword1 = detectionResult.DetectionResultColumns[barcodeColumn - num1].getCodeword(imageRow);
      if (codeword1 != null)
        return !leftToRight ? codeword1.StartX : codeword1.EndX;
      Codeword codewordNearby = detectionResult.DetectionResultColumns[barcodeColumn].getCodewordNearby(imageRow);
      if (codewordNearby != null)
        return !leftToRight ? codewordNearby.EndX : codewordNearby.StartX;
      if (PDF417ScanningDecoder.isValidBarcodeColumn(detectionResult, barcodeColumn - num1))
        codewordNearby = detectionResult.DetectionResultColumns[barcodeColumn - num1].getCodewordNearby(imageRow);
      if (codewordNearby != null)
        return !leftToRight ? codewordNearby.StartX : codewordNearby.EndX;
      int num2 = 0;
      while (PDF417ScanningDecoder.isValidBarcodeColumn(detectionResult, barcodeColumn - num1))
      {
        barcodeColumn -= num1;
        foreach (Codeword codeword2 in detectionResult.DetectionResultColumns[barcodeColumn].Codewords)
        {
          if (codeword2 != null)
            return (leftToRight ? codeword2.EndX : codeword2.StartX) + num1 * num2 * (codeword2.EndX - codeword2.StartX);
        }
        ++num2;
      }
      return !leftToRight ? detectionResult.Box.MaxX : detectionResult.Box.MinX;
    }

    /// <summary>Detects the codeword.</summary>
    /// <returns>The codeword.</returns>
    /// <param name="image">Image.</param>
    /// <param name="minColumn">Minimum column.</param>
    /// <param name="maxColumn">Max column.</param>
    /// <param name="leftToRight">If set to <c>true</c> left to right.</param>
    /// <param name="startColumn">Start column.</param>
    /// <param name="imageRow">Image row.</param>
    /// <param name="minCodewordWidth">Minimum codeword width.</param>
    /// <param name="maxCodewordWidth">Max codeword width.</param>
    private static Codeword detectCodeword(
      BitMatrix image,
      int minColumn,
      int maxColumn,
      bool leftToRight,
      int startColumn,
      int imageRow,
      int minCodewordWidth,
      int maxCodewordWidth)
    {
      startColumn = PDF417ScanningDecoder.adjustCodewordStartColumn(image, minColumn, maxColumn, leftToRight, startColumn, imageRow);
      int[] moduleBitCount = PDF417ScanningDecoder.getModuleBitCount(image, minColumn, maxColumn, leftToRight, startColumn, imageRow);
      if (moduleBitCount == null)
        return (Codeword) null;
      int bitCountSum = PDF417Common.getBitCountSum(moduleBitCount);
      int endX;
      if (leftToRight)
      {
        endX = startColumn + bitCountSum;
      }
      else
      {
        for (int index = 0; index < moduleBitCount.Length >> 1; ++index)
        {
          int num = moduleBitCount[index];
          moduleBitCount[index] = moduleBitCount[moduleBitCount.Length - 1 - index];
          moduleBitCount[moduleBitCount.Length - 1 - index] = num;
        }
        endX = startColumn;
        startColumn = endX - bitCountSum;
      }
      if (!PDF417ScanningDecoder.checkCodewordSkew(bitCountSum, minCodewordWidth, maxCodewordWidth))
        return (Codeword) null;
      int decodedValue = PDF417CodewordDecoder.getDecodedValue(moduleBitCount);
      int codeword = PDF417Common.getCodeword((long) decodedValue);
      return codeword == -1 ? (Codeword) null : new Codeword(startColumn, endX, PDF417ScanningDecoder.getCodewordBucketNumber(decodedValue), codeword);
    }

    /// <summary>Gets the module bit count.</summary>
    /// <returns>The module bit count.</returns>
    /// <param name="image">Image.</param>
    /// <param name="minColumn">Minimum column.</param>
    /// <param name="maxColumn">Max column.</param>
    /// <param name="leftToRight">If set to <c>true</c> left to right.</param>
    /// <param name="startColumn">Start column.</param>
    /// <param name="imageRow">Image row.</param>
    private static int[] getModuleBitCount(
      BitMatrix image,
      int minColumn,
      int maxColumn,
      bool leftToRight,
      int startColumn,
      int imageRow)
    {
      int x = startColumn;
      int[] numArray = new int[8];
      int index = 0;
      int num = leftToRight ? 1 : -1;
      bool flag = leftToRight;
      while ((leftToRight && x < maxColumn || !leftToRight && x >= minColumn) && index < numArray.Length)
      {
        if (image[x, imageRow] == flag)
        {
          ++numArray[index];
          x += num;
        }
        else
        {
          ++index;
          flag = !flag;
        }
      }
      return index == numArray.Length || (leftToRight && x == maxColumn || !leftToRight && x == minColumn) && index == numArray.Length - 1 ? numArray : (int[]) null;
    }

    /// <summary>Gets the number of EC code words.</summary>
    /// <returns>The number of EC code words.</returns>
    /// <param name="barcodeECLevel">Barcode EC level.</param>
    private static int getNumberOfECCodeWords(int barcodeECLevel) => 2 << barcodeECLevel;

    /// <summary>Adjusts the codeword start column.</summary>
    /// <returns>The codeword start column.</returns>
    /// <param name="image">Image.</param>
    /// <param name="minColumn">Minimum column.</param>
    /// <param name="maxColumn">Max column.</param>
    /// <param name="leftToRight">If set to <c>true</c> left to right.</param>
    /// <param name="codewordStartColumn">Codeword start column.</param>
    /// <param name="imageRow">Image row.</param>
    private static int adjustCodewordStartColumn(
      BitMatrix image,
      int minColumn,
      int maxColumn,
      bool leftToRight,
      int codewordStartColumn,
      int imageRow)
    {
      int x = codewordStartColumn;
      int num = leftToRight ? -1 : 1;
      for (int index = 0; index < 2; ++index)
      {
        for (; (leftToRight && x >= minColumn || !leftToRight && x < maxColumn) && leftToRight == image[x, imageRow]; x += num)
        {
          if (Math.Abs(codewordStartColumn - x) > 2)
            return codewordStartColumn;
        }
        num = -num;
        leftToRight = !leftToRight;
      }
      return x;
    }

    /// <summary>Checks the codeword for any skew.</summary>
    /// <returns><c>true</c>, if codeword is within the skew, <c>false</c> otherwise.</returns>
    /// <param name="codewordSize">Codeword size.</param>
    /// <param name="minCodewordWidth">Minimum codeword width.</param>
    /// <param name="maxCodewordWidth">Max codeword width.</param>
    private static bool checkCodewordSkew(
      int codewordSize,
      int minCodewordWidth,
      int maxCodewordWidth)
    {
      return minCodewordWidth - 2 <= codewordSize && codewordSize <= maxCodewordWidth + 2;
    }

    /// <summary>Decodes the codewords.</summary>
    /// <returns>The codewords.</returns>
    /// <param name="codewords">Codewords.</param>
    /// <param name="ecLevel">Ec level.</param>
    /// <param name="erasures">Erasures.</param>
    private static DecoderResult decodeCodewords(int[] codewords, int ecLevel, int[] erasures)
    {
      if (codewords.Length == 0)
        return (DecoderResult) null;
      int numECCodewords = 1 << ecLevel + 1;
      int num = PDF417ScanningDecoder.correctErrors(codewords, erasures, numECCodewords);
      if (num < 0)
        return (DecoderResult) null;
      if (!PDF417ScanningDecoder.verifyCodewordCount(codewords, numECCodewords))
        return (DecoderResult) null;
      DecoderResult decoderResult = DecodedBitStreamParser.decode(codewords, ecLevel.ToString());
      if (decoderResult != null)
      {
        decoderResult.ErrorsCorrected = num;
        decoderResult.Erasures = erasures.Length;
      }
      return decoderResult;
    }

    /// <summary>
    /// Given data and error-correction codewords received, possibly corrupted by errors, attempts to
    /// correct the errors in-place.
    /// </summary>
    /// <returns>The errors.</returns>
    /// <param name="codewords">data and error correction codewords.</param>
    /// <param name="erasures">positions of any known erasures.</param>
    /// <param name="numECCodewords">number of error correction codewords that are available in codewords.</param>
    private static int correctErrors(int[] codewords, int[] erasures, int numECCodewords)
    {
      int errorLocationsCount;
      return erasures != null && erasures.Length > numECCodewords / 2 + 3 || numECCodewords < 0 || numECCodewords > 512 || !PDF417ScanningDecoder.errorCorrection.decode(codewords, numECCodewords, erasures, out errorLocationsCount) ? -1 : errorLocationsCount;
    }

    /// <summary>
    /// Verifies that all is well with the the codeword array.
    /// </summary>
    /// <param name="codewords">Codewords.</param>
    /// <param name="numECCodewords">Number EC codewords.</param>
    private static bool verifyCodewordCount(int[] codewords, int numECCodewords)
    {
      if (codewords.Length < 4)
        return false;
      int codeword = codewords[0];
      if (codeword > codewords.Length)
        return false;
      if (codeword == 0)
      {
        if (numECCodewords >= codewords.Length)
          return false;
        codewords[0] = codewords.Length - numECCodewords;
      }
      return true;
    }

    /// <summary>Gets the bit count for codeword.</summary>
    /// <returns>The bit count for codeword.</returns>
    /// <param name="codeword">Codeword.</param>
    private static int[] getBitCountForCodeword(int codeword)
    {
      int[] countForCodeword = new int[8];
      int num = 0;
      int index = countForCodeword.Length - 1;
      while (true)
      {
        if ((codeword & 1) != num)
        {
          num = codeword & 1;
          --index;
          if (index < 0)
            break;
        }
        ++countForCodeword[index];
        codeword >>= 1;
      }
      return countForCodeword;
    }

    /// <summary>Gets the codeword bucket number.</summary>
    /// <returns>The codeword bucket number.</returns>
    /// <param name="codeword">Codeword.</param>
    private static int getCodewordBucketNumber(int codeword)
    {
      return PDF417ScanningDecoder.getCodewordBucketNumber(PDF417ScanningDecoder.getBitCountForCodeword(codeword));
    }

    /// <summary>Gets the codeword bucket number.</summary>
    /// <returns>The codeword bucket number.</returns>
    /// <param name="moduleBitCount">Module bit count.</param>
    private static int getCodewordBucketNumber(int[] moduleBitCount)
    {
      return (moduleBitCount[0] - moduleBitCount[2] + moduleBitCount[4] - moduleBitCount[6] + 9) % 9;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the <see cref="T:ZXing.PDF417.Internal.BarcodeValue" /> jagged array.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents the <see cref="T:ZXing.PDF417.Internal.BarcodeValue" /> jagged array.</returns>
    /// <param name="barcodeMatrix">Barcode matrix as a jagged array.</param>
    public static string ToString(BarcodeValue[][] barcodeMatrix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index1 = 0; index1 < barcodeMatrix.Length; ++index1)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Row {0,2}: ", (object) index1);
        for (int index2 = 0; index2 < barcodeMatrix[index1].Length; ++index2)
        {
          BarcodeValue barcodeValue = barcodeMatrix[index1][index2];
          int[] numArray = barcodeValue.getValue();
          if (numArray.Length == 0)
            stringBuilder.Append("        ");
          else
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0,4}({1,2})", (object) numArray[0], (object) barcodeValue.getConfidence(numArray[0]));
        }
        stringBuilder.Append("\n");
      }
      return stringBuilder.ToString();
    }
  }
}
