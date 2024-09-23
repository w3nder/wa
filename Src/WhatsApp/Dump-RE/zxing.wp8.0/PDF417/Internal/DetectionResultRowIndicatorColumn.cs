// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;

#nullable disable
namespace ZXing.PDF417.Internal
{
  /// <summary>Represents a Column in the Detection Result</summary>
  /// <author>Guenther Grau</author>
  public sealed class DetectionResultRowIndicatorColumn : DetectionResultColumn
  {
    /// <summary>
    /// Gets or sets a value indicating whether this instance is the left indicator
    /// </summary>
    /// <value><c>true</c> if this instance is left; otherwise, <c>false</c>.</value>
    public bool IsLeft { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn" /> class.
    /// </summary>
    /// <param name="box">Box.</param>
    /// <param name="isLeft">If set to <c>true</c> is left.</param>
    public DetectionResultRowIndicatorColumn(BoundingBox box, bool isLeft)
      : base(box)
    {
      this.IsLeft = isLeft;
    }

    /// <summary>Sets the Row Numbers as Inidicator Columns</summary>
    public void setRowNumbers()
    {
      foreach (Codeword codeword in this.Codewords)
        codeword?.setRowNumberAsRowIndicatorColumn();
    }

    /// <summary>
    /// TODO implement properly
    /// TODO maybe we should add missing codewords to store the correct row number to make
    /// finding row numbers for other columns easier
    /// use row height count to make detection of invalid row numbers more reliable
    /// </summary>
    /// <returns>The indicator column row numbers.</returns>
    /// <param name="metadata">Metadata.</param>
    public int adjustCompleteIndicatorColumnRowNumbers(BarcodeMetadata metadata)
    {
      Codeword[] codewords = this.Codewords;
      this.setRowNumbers();
      this.removeIncorrectCodewords(codewords, metadata);
      ResultPoint resultPoint1 = this.IsLeft ? this.Box.TopLeft : this.Box.TopRight;
      ResultPoint resultPoint2 = this.IsLeft ? this.Box.BottomLeft : this.Box.BottomRight;
      int codewordIndex1 = this.imageRowToCodewordIndex((int) resultPoint1.Y);
      int codewordIndex2 = this.imageRowToCodewordIndex((int) resultPoint2.Y);
      float num1 = (float) (codewordIndex2 - codewordIndex1) / (float) metadata.RowCount;
      int num2 = -1;
      int val1 = 1;
      int val2 = 0;
      for (int index1 = codewordIndex1; index1 < codewordIndex2; ++index1)
      {
        Codeword codeword = codewords[index1];
        if (codeword != null)
        {
          int num3 = codeword.RowNumber - num2;
          switch (num3)
          {
            case 0:
              ++val2;
              continue;
            case 1:
              val1 = Math.Max(val1, val2);
              val2 = 1;
              num2 = codeword.RowNumber;
              continue;
            default:
              if (num3 < 0 || codeword.RowNumber >= metadata.RowCount || num3 > index1)
              {
                codewords[index1] = (Codeword) null;
                continue;
              }
              int num4 = val1 <= 2 ? num3 : (val1 - 2) * num3;
              bool flag = num4 > index1;
              for (int index2 = 1; index2 <= num4 && !flag; ++index2)
                flag = codewords[index1 - index2] != null;
              if (flag)
              {
                codewords[index1] = (Codeword) null;
                continue;
              }
              num2 = codeword.RowNumber;
              val2 = 1;
              continue;
          }
        }
      }
      return (int) ((double) num1 + 0.5);
    }

    /// <summary>Gets the row heights.</summary>
    /// <returns>The row heights.</returns>
    public int[] getRowHeights()
    {
      BarcodeMetadata barcodeMetadata = this.getBarcodeMetadata();
      if (barcodeMetadata == null)
        return (int[]) null;
      this.adjustIncompleteIndicatorColumnRowNumbers(barcodeMetadata);
      int[] rowHeights = new int[barcodeMetadata.RowCount];
      foreach (Codeword codeword in this.Codewords)
      {
        if (codeword != null)
        {
          int rowNumber = codeword.RowNumber;
          if (rowNumber >= rowHeights.Length)
            return (int[]) null;
          ++rowHeights[rowNumber];
        }
      }
      return rowHeights;
    }

    /// <summary>Adjusts the in omplete indicator column row numbers.</summary>
    /// <param name="metadata">Metadata.</param>
    public int adjustIncompleteIndicatorColumnRowNumbers(BarcodeMetadata metadata)
    {
      ResultPoint resultPoint1 = this.IsLeft ? this.Box.TopLeft : this.Box.TopRight;
      ResultPoint resultPoint2 = this.IsLeft ? this.Box.BottomLeft : this.Box.BottomRight;
      int codewordIndex1 = this.imageRowToCodewordIndex((int) resultPoint1.Y);
      int codewordIndex2 = this.imageRowToCodewordIndex((int) resultPoint2.Y);
      float num1 = (float) (codewordIndex2 - codewordIndex1) / (float) metadata.RowCount;
      Codeword[] codewords = this.Codewords;
      int num2 = -1;
      int val1 = 1;
      int val2 = 0;
      for (int index = codewordIndex1; index < codewordIndex2; ++index)
      {
        Codeword codeword = codewords[index];
        if (codeword != null)
        {
          codeword.setRowNumberAsRowIndicatorColumn();
          switch (codeword.RowNumber - num2)
          {
            case 0:
              ++val2;
              continue;
            case 1:
              val1 = Math.Max(val1, val2);
              val2 = 1;
              num2 = codeword.RowNumber;
              continue;
            default:
              if (codeword.RowNumber > metadata.RowCount)
              {
                this.Codewords[index] = (Codeword) null;
                continue;
              }
              num2 = codeword.RowNumber;
              val2 = 1;
              continue;
          }
        }
      }
      return (int) ((double) num1 + 0.5);
    }

    /// <summary>Gets the barcode metadata.</summary>
    /// <returns>The barcode metadata.</returns>
    public BarcodeMetadata getBarcodeMetadata()
    {
      Codeword[] codewords = this.Codewords;
      BarcodeValue barcodeValue1 = new BarcodeValue();
      BarcodeValue barcodeValue2 = new BarcodeValue();
      BarcodeValue barcodeValue3 = new BarcodeValue();
      BarcodeValue barcodeValue4 = new BarcodeValue();
      foreach (Codeword codeword in codewords)
      {
        if (codeword != null)
        {
          codeword.setRowNumberAsRowIndicatorColumn();
          int num = codeword.Value % 30;
          int rowNumber = codeword.RowNumber;
          if (!this.IsLeft)
            rowNumber += 2;
          switch (rowNumber % 3)
          {
            case 0:
              barcodeValue2.setValue(num * 3 + 1);
              continue;
            case 1:
              barcodeValue4.setValue(num / 3);
              barcodeValue3.setValue(num % 3);
              continue;
            case 2:
              barcodeValue1.setValue(num + 1);
              continue;
            default:
              continue;
          }
        }
      }
      int[] numArray1 = barcodeValue1.getValue();
      int[] numArray2 = barcodeValue2.getValue();
      int[] numArray3 = barcodeValue3.getValue();
      int[] numArray4 = barcodeValue4.getValue();
      if (numArray1.Length == 0 || numArray2.Length == 0 || numArray3.Length == 0 || numArray4.Length == 0 || numArray1[0] < 1 || numArray2[0] + numArray3[0] < PDF417Common.MIN_ROWS_IN_BARCODE || numArray2[0] + numArray3[0] > PDF417Common.MAX_ROWS_IN_BARCODE)
        return (BarcodeMetadata) null;
      BarcodeMetadata metadata = new BarcodeMetadata(numArray1[0], numArray2[0], numArray3[0], numArray4[0]);
      this.removeIncorrectCodewords(codewords, metadata);
      return metadata;
    }

    /// <summary>
    /// Prune the codewords which do not match the metadata
    /// TODO Maybe we should keep the incorrect codewords for the start and end positions?
    /// </summary>
    /// <param name="codewords">Codewords.</param>
    /// <param name="metadata">Metadata.</param>
    private void removeIncorrectCodewords(Codeword[] codewords, BarcodeMetadata metadata)
    {
      for (int index = 0; index < codewords.Length; ++index)
      {
        Codeword codeword = codewords[index];
        if (codeword != null)
        {
          int num = codeword.Value % 30;
          int rowNumber = codeword.RowNumber;
          if (rowNumber >= metadata.RowCount)
          {
            codewords[index] = (Codeword) null;
          }
          else
          {
            if (!this.IsLeft)
              rowNumber += 2;
            switch (rowNumber % 3)
            {
              case 1:
                if (num % 3 != metadata.RowCountLower || num / 3 != metadata.ErrorCorrectionLevel)
                {
                  codewords[index] = (Codeword) null;
                  continue;
                }
                continue;
              case 2:
                if (num + 1 != metadata.ColumnCount)
                {
                  codewords[index] = (Codeword) null;
                  continue;
                }
                continue;
              default:
                if (num * 3 + 1 != metadata.RowCountUpper)
                {
                  codewords[index] = (Codeword) null;
                  continue;
                }
                continue;
            }
          }
        }
      }
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResultRowIndicatorColumn" />.</returns>
    public override string ToString()
    {
      return "Is Left: " + (object) this.IsLeft + " \n" + base.ToString();
    }
  }
}
