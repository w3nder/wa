// Decompiled with JetBrains decompiler
// Type: ZXing.Common.EncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;
using System.ComponentModel;

#nullable disable
namespace ZXing.Common
{
  /// <summary>Defines an container for encoder options</summary>
  [Serializable]
  public class EncodingOptions
  {
    /// <summary>Gets the data container for all options</summary>
    [Browsable(false)]
    public IDictionary<EncodeHintType, object> Hints { get; private set; }

    /// <summary>Specifies the height of the barcode image</summary>
    public int Height
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.HEIGHT) ? (int) this.Hints[EncodeHintType.HEIGHT] : 0;
      }
      set => this.Hints[EncodeHintType.HEIGHT] = (object) value;
    }

    /// <summary>Specifies the width of the barcode image</summary>
    public int Width
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.WIDTH) ? (int) this.Hints[EncodeHintType.WIDTH] : 0;
      }
      set => this.Hints[EncodeHintType.WIDTH] = (object) value;
    }

    /// <summary>Don't put the content string into the output image.</summary>
    public bool PureBarcode
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.PURE_BARCODE) && (bool) this.Hints[EncodeHintType.PURE_BARCODE];
      }
      set => this.Hints[EncodeHintType.PURE_BARCODE] = (object) value;
    }

    /// <summary>
    /// Specifies margin, in pixels, to use when generating the barcode. The meaning can vary
    /// by format; for example it controls margin before and after the barcode horizontally for
    /// most 1D formats.
    /// </summary>
    public int Margin
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.MARGIN) ? (int) this.Hints[EncodeHintType.MARGIN] : 0;
      }
      set => this.Hints[EncodeHintType.MARGIN] = (object) value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ZXing.Common.EncodingOptions" /> class.
    /// </summary>
    public EncodingOptions()
    {
      this.Hints = (IDictionary<EncodeHintType, object>) new Dictionary<EncodeHintType, object>();
    }
  }
}
