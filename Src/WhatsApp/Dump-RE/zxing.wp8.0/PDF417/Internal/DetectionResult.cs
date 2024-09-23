// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.DetectionResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>Guenther Grau</author>
  public class DetectionResult
  {
    private const int ADJUST_ROW_NUMBER_SKIP = 2;

    public BarcodeMetadata Metadata { get; private set; }

    public DetectionResultColumn[] DetectionResultColumns { get; set; }

    public BoundingBox Box { get; set; }

    public int ColumnCount { get; private set; }

    public int RowCount => this.Metadata.RowCount;

    public int ErrorCorrectionLevel => this.Metadata.ErrorCorrectionLevel;

    public DetectionResult(BarcodeMetadata metadata, BoundingBox box)
    {
      this.Metadata = metadata;
      this.Box = box;
      this.ColumnCount = metadata.ColumnCount;
      this.DetectionResultColumns = new DetectionResultColumn[this.ColumnCount + 2];
    }

    /// <summary>
    /// Returns the DetectionResult Columns.  This does a fair bit of calculation, so call it sparingly.
    /// </summary>
    /// <returns>The detection result columns.</returns>
    public DetectionResultColumn[] getDetectionResultColumns()
    {
      this.adjustIndicatorColumnRowNumbers(this.DetectionResultColumns[0]);
      this.adjustIndicatorColumnRowNumbers(this.DetectionResultColumns[this.ColumnCount + 1]);
      int num1 = PDF417Common.MAX_CODEWORDS_IN_BARCODE;
      int num2;
      do
      {
        num2 = num1;
        num1 = this.adjustRowNumbers();
      }
      while (num1 > 0 && num1 < num2);
      return this.DetectionResultColumns;
    }

    /// <summary>Adjusts the indicator column row numbers.</summary>
    /// <param name="detectionResultColumn">Detection result column.</param>
    private void adjustIndicatorColumnRowNumbers(DetectionResultColumn detectionResultColumn)
    {
      ((DetectionResultRowIndicatorColumn) detectionResultColumn)?.adjustCompleteIndicatorColumnRowNumbers(this.Metadata);
    }

    /// <summary>
    /// return number of codewords which don't have a valid row number. Note that the count is not accurate as codewords .
    /// will be counted several times. It just serves as an indicator to see when we can stop adjusting row numbers
    /// </summary>
    /// <returns>The row numbers.</returns>
    private int adjustRowNumbers()
    {
      int num = this.adjustRowNumbersByRow();
      if (num == 0)
        return 0;
      for (int barcodeColumn = 1; barcodeColumn < this.ColumnCount + 1; ++barcodeColumn)
      {
        Codeword[] codewords = this.DetectionResultColumns[barcodeColumn].Codewords;
        for (int codewordsRow = 0; codewordsRow < codewords.Length; ++codewordsRow)
        {
          if (codewords[codewordsRow] != null && !codewords[codewordsRow].HasValidRowNumber)
            this.adjustRowNumbers(barcodeColumn, codewordsRow, codewords);
        }
      }
      return num;
    }

    /// <summary>Adjusts the row numbers by row.</summary>
    /// <returns>The row numbers by row.</returns>
    private int adjustRowNumbersByRow()
    {
      this.adjustRowNumbersFromBothRI();
      return this.adjustRowNumbersFromLRI() + this.adjustRowNumbersFromRRI();
    }

    /// <summary>Adjusts the row numbers from both Row Indicators</summary>
    /// <returns> zero </returns>
    private void adjustRowNumbersFromBothRI()
    {
      if (this.DetectionResultColumns[0] == null || this.DetectionResultColumns[this.ColumnCount + 1] == null)
        return;
      Codeword[] codewords1 = this.DetectionResultColumns[0].Codewords;
      Codeword[] codewords2 = this.DetectionResultColumns[this.ColumnCount + 1].Codewords;
      for (int index1 = 0; index1 < codewords1.Length; ++index1)
      {
        if (codewords1[index1] != null && codewords2[index1] != null && codewords1[index1].RowNumber == codewords2[index1].RowNumber)
        {
          for (int index2 = 1; index2 <= this.ColumnCount; ++index2)
          {
            Codeword codeword = this.DetectionResultColumns[index2].Codewords[index1];
            if (codeword != null)
            {
              codeword.RowNumber = codewords1[index1].RowNumber;
              if (!codeword.HasValidRowNumber)
                this.DetectionResultColumns[index2].Codewords[index1] = (Codeword) null;
            }
          }
        }
      }
    }

    /// <summary>Adjusts the row numbers from Right Row Indicator.</summary>
    /// <returns>The unadjusted row count.</returns>
    private int adjustRowNumbersFromRRI()
    {
      if (this.DetectionResultColumns[this.ColumnCount + 1] == null)
        return 0;
      int num = 0;
      Codeword[] codewords = this.DetectionResultColumns[this.ColumnCount + 1].Codewords;
      for (int index1 = 0; index1 < codewords.Length; ++index1)
      {
        if (codewords[index1] != null)
        {
          int rowNumber = codewords[index1].RowNumber;
          int invalidRowCounts = 0;
          for (int index2 = this.ColumnCount + 1; index2 > 0 && invalidRowCounts < 2; --index2)
          {
            Codeword codeword = this.DetectionResultColumns[index2].Codewords[index1];
            if (codeword != null)
            {
              invalidRowCounts = DetectionResult.adjustRowNumberIfValid(rowNumber, invalidRowCounts, codeword);
              if (!codeword.HasValidRowNumber)
                ++num;
            }
          }
        }
      }
      return num;
    }

    /// <summary>Adjusts the row numbers from Left Row Indicator.</summary>
    /// <returns> Unadjusted row Count.</returns>
    private int adjustRowNumbersFromLRI()
    {
      if (this.DetectionResultColumns[0] == null)
        return 0;
      int num = 0;
      Codeword[] codewords = this.DetectionResultColumns[0].Codewords;
      for (int index1 = 0; index1 < codewords.Length; ++index1)
      {
        if (codewords[index1] != null)
        {
          int rowNumber = codewords[index1].RowNumber;
          int invalidRowCounts = 0;
          for (int index2 = 1; index2 < this.ColumnCount + 1 && invalidRowCounts < 2; ++index2)
          {
            Codeword codeword = this.DetectionResultColumns[index2].Codewords[index1];
            if (codeword != null)
            {
              invalidRowCounts = DetectionResult.adjustRowNumberIfValid(rowNumber, invalidRowCounts, codeword);
              if (!codeword.HasValidRowNumber)
                ++num;
            }
          }
        }
      }
      return num;
    }

    /// <summary>Adjusts the row number if valid.</summary>
    /// <returns>The invalid rows</returns>
    /// <param name="rowIndicatorRowNumber">Row indicator row number.</param>
    /// <param name="invalidRowCounts">Invalid row counts.</param>
    /// <param name="codeword">Codeword.</param>
    private static int adjustRowNumberIfValid(
      int rowIndicatorRowNumber,
      int invalidRowCounts,
      Codeword codeword)
    {
      if (codeword == null || codeword.HasValidRowNumber)
        return invalidRowCounts;
      if (codeword.IsValidRowNumber(rowIndicatorRowNumber))
      {
        codeword.RowNumber = rowIndicatorRowNumber;
        invalidRowCounts = 0;
      }
      else
        ++invalidRowCounts;
      return invalidRowCounts;
    }

    /// <summary>Adjusts the row numbers.</summary>
    /// <param name="barcodeColumn">Barcode column.</param>
    /// <param name="codewordsRow">Codewords row.</param>
    /// <param name="codewords">Codewords.</param>
    private void adjustRowNumbers(int barcodeColumn, int codewordsRow, Codeword[] codewords)
    {
      Codeword codeword = codewords[codewordsRow];
      Codeword[] codewords1 = this.DetectionResultColumns[barcodeColumn - 1].Codewords;
      Codeword[] codewordArray1 = codewords1;
      if (this.DetectionResultColumns[barcodeColumn + 1] != null)
        codewordArray1 = this.DetectionResultColumns[barcodeColumn + 1].Codewords;
      Codeword[] codewordArray2 = new Codeword[14];
      codewordArray2[2] = codewords1[codewordsRow];
      codewordArray2[3] = codewordArray1[codewordsRow];
      if (codewordsRow > 0)
      {
        codewordArray2[0] = codewords[codewordsRow - 1];
        codewordArray2[4] = codewords1[codewordsRow - 1];
        codewordArray2[5] = codewordArray1[codewordsRow - 1];
      }
      if (codewordsRow > 1)
      {
        codewordArray2[8] = codewords[codewordsRow - 2];
        codewordArray2[10] = codewords1[codewordsRow - 2];
        codewordArray2[11] = codewordArray1[codewordsRow - 2];
      }
      if (codewordsRow < codewords.Length - 1)
      {
        codewordArray2[1] = codewords[codewordsRow + 1];
        codewordArray2[6] = codewords1[codewordsRow + 1];
        codewordArray2[7] = codewordArray1[codewordsRow + 1];
      }
      if (codewordsRow < codewords.Length - 2)
      {
        codewordArray2[9] = codewords[codewordsRow + 2];
        codewordArray2[12] = codewords1[codewordsRow + 2];
        codewordArray2[13] = codewordArray1[codewordsRow + 2];
      }
      foreach (Codeword otherCodeword in codewordArray2)
      {
        if (DetectionResult.adjustRowNumber(codeword, otherCodeword))
          break;
      }
    }

    /// <summary>Adjusts the row number.</summary>
    /// <returns><c>true</c>, if row number was adjusted, <c>false</c> otherwise.</returns>
    /// <param name="codeword">Codeword.</param>
    /// <param name="otherCodeword">Other codeword.</param>
    private static bool adjustRowNumber(Codeword codeword, Codeword otherCodeword)
    {
      if (otherCodeword == null || !otherCodeword.HasValidRowNumber || otherCodeword.Bucket != codeword.Bucket)
        return false;
      codeword.RowNumber = otherCodeword.RowNumber;
      return true;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResult" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResult" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      DetectionResultColumn detectionResultColumn = this.DetectionResultColumns[0] ?? this.DetectionResultColumns[this.ColumnCount + 1];
      for (int index1 = 0; index1 < detectionResultColumn.Codewords.Length; ++index1)
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "CW {0,3}:", (object) index1);
        for (int index2 = 0; index2 < this.ColumnCount + 2; ++index2)
        {
          if (this.DetectionResultColumns[index2] == null)
          {
            stringBuilder.Append("    |   ");
          }
          else
          {
            Codeword codeword = this.DetectionResultColumns[index2].Codewords[index1];
            if (codeword == null)
              stringBuilder.Append("    |   ");
            else
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, " {0,3}|{1,3}", (object) codeword.RowNumber, (object) codeword.Value);
          }
        }
        stringBuilder.Append("\n");
      }
      return stringBuilder.ToString();
    }
  }
}
