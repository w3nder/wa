// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.Internal.DetectionResultColumn
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
  /// <summary>Represents a Column in the Detection Result</summary>
  /// <author>Guenther Grau</author>
  public class DetectionResultColumn
  {
    /// <summary>
    /// The maximum distance to search in the codeword array in both the positive and negative directions
    /// </summary>
    private const int MAX_NEARBY_DISTANCE = 5;

    /// <summary>The Bounding Box around the column (in the BitMatrix)</summary>
    /// <value>The box.</value>
    public BoundingBox Box { get; private set; }

    /// <summary>
    /// The Codewords the Box encodes for, offset by the Box minY.
    /// Remember to Access this ONLY through GetCodeword(imageRow) if you're accessing it in that manner.
    /// </summary>
    /// <value>The codewords.</value>
    public Codeword[] Codewords { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.PDF417.Internal.DetectionResultColumn" /> class.
    /// </summary>
    /// <param name="box">The Bounding Box around the column (in the BitMatrix)</param>
    public DetectionResultColumn(BoundingBox box)
    {
      this.Box = BoundingBox.Create(box);
      this.Codewords = new Codeword[this.Box.MaxY - this.Box.MinY + 1];
    }

    /// <summary>
    /// Converts the Image's Row to the index in the Codewords array
    /// </summary>
    /// <returns>The Codeword Index.</returns>
    /// <param name="imageRow">Image row.</param>
    public int IndexForRow(int imageRow) => imageRow - this.Box.MinY;

    /// <summary>
    /// Converts the Codeword array index into a Row in the Image (BitMatrix)
    /// </summary>
    /// <returns>The Image Row.</returns>
    /// <param name="codewordIndex">Codeword index.</param>
    public int RowForIndex(int codewordIndex) => this.Box.MinY + codewordIndex;

    /// <summary>Gets the codeword for a given row</summary>
    /// <returns>The codeword.</returns>
    /// <param name="imageRow">Image row.</param>
    public Codeword getCodeword(int imageRow)
    {
      return this.Codewords[this.imageRowToCodewordIndex(imageRow)];
    }

    /// <summary>
    /// Gets the codeword closest to the specified row in the image
    /// </summary>
    /// <param name="imageRow">Image row.</param>
    public Codeword getCodewordNearby(int imageRow)
    {
      Codeword codeword1 = this.getCodeword(imageRow);
      if (codeword1 != null)
        return codeword1;
      for (int index1 = 1; index1 < 5; ++index1)
      {
        int index2 = this.imageRowToCodewordIndex(imageRow) - index1;
        if (index2 >= 0)
        {
          Codeword codeword2 = this.Codewords[index2];
          if (codeword2 != null)
            return codeword2;
        }
        int index3 = this.imageRowToCodewordIndex(imageRow) + index1;
        if (index3 < this.Codewords.Length)
        {
          Codeword codeword3 = this.Codewords[index3];
          if (codeword3 != null)
            return codeword3;
        }
      }
      return (Codeword) null;
    }

    internal int imageRowToCodewordIndex(int imageRow) => imageRow - this.Box.MinY;

    /// <summary>Sets the codeword for an image row</summary>
    /// <param name="imageRow">Image row.</param>
    /// <param name="codeword">Codeword.</param>
    public void setCodeword(int imageRow, Codeword codeword)
    {
      this.Codewords[this.IndexForRow(imageRow)] = codeword;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResultColumn" />.
    /// </summary>
    /// <returns>A <see cref="T:System.String" /> that represents the current <see cref="T:ZXing.PDF417.Internal.DetectionResultColumn" />.</returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (Codeword codeword in this.Codewords)
      {
        if (codeword == null)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0,3}:    |   \n", (object) num++);
        else
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0,3}: {1,3}|{2,3}\n", (object) num++, (object) codeword.RowNumber, (object) codeword.Value);
      }
      return stringBuilder.ToString();
    }
  }
}
