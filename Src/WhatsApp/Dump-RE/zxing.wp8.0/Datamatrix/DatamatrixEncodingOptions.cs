// Decompiled with JetBrains decompiler
// Type: ZXing.Datamatrix.DatamatrixEncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;
using ZXing.Datamatrix.Encoder;

#nullable disable
namespace ZXing.Datamatrix
{
  /// <summary>
  /// The class holds the available options for the DatamatrixWriter
  /// </summary>
  [Serializable]
  public class DatamatrixEncodingOptions : EncodingOptions
  {
    /// <summary>Specifies the matrix shape for Data Matrix</summary>
    public SymbolShapeHint? SymbolShape
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE) ? new SymbolShapeHint?((SymbolShapeHint) this.Hints[EncodeHintType.DATA_MATRIX_SHAPE]) : new SymbolShapeHint?();
      }
      set
      {
        if (!value.HasValue)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
            return;
          this.Hints.Remove(EncodeHintType.DATA_MATRIX_SHAPE);
        }
        else
          this.Hints[EncodeHintType.DATA_MATRIX_SHAPE] = (object) value;
      }
    }

    /// <summary>Specifies a minimum barcode size</summary>
    public Dimension MinSize
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.MIN_SIZE) ? (Dimension) this.Hints[EncodeHintType.MIN_SIZE] : (Dimension) null;
      }
      set
      {
        if (value == null)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.MIN_SIZE))
            return;
          this.Hints.Remove(EncodeHintType.MIN_SIZE);
        }
        else
          this.Hints[EncodeHintType.MIN_SIZE] = (object) value;
      }
    }

    /// <summary>Specifies a maximum barcode size</summary>
    public Dimension MaxSize
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.MAX_SIZE) ? (Dimension) this.Hints[EncodeHintType.MAX_SIZE] : (Dimension) null;
      }
      set
      {
        if (value == null)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.MAX_SIZE))
            return;
          this.Hints.Remove(EncodeHintType.MAX_SIZE);
        }
        else
          this.Hints[EncodeHintType.MAX_SIZE] = (object) value;
      }
    }

    /// <summary>
    /// Specifies the default encodation
    /// Make sure that the content fits into the encodation value, otherwise there will be an exception thrown.
    /// standard value: Encodation.ASCII
    /// </summary>
    public int? DefaultEncodation
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION) ? new int?((int) this.Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION]) : new int?();
      }
      set
      {
        if (!value.HasValue)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
            return;
          this.Hints.Remove(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION);
        }
        else
          this.Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION] = (object) value;
      }
    }
  }
}
