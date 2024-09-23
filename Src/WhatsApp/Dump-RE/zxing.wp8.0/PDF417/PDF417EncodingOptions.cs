// Decompiled with JetBrains decompiler
// Type: ZXing.PDF417.PDF417EncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using ZXing.Common;
using ZXing.PDF417.Internal;

#nullable disable
namespace ZXing.PDF417
{
  /// <summary>
  /// The class holds the available options for the <see cref="T:ZXing.PDF417.PDF417Writer" />
  /// </summary>
  [Serializable]
  public class PDF417EncodingOptions : EncodingOptions
  {
    /// <summary>
    /// Specifies whether to use compact mode for PDF417 (type <see cref="T:System.Boolean" />).
    /// </summary>
    public bool Compact
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.PDF417_COMPACT) && (bool) this.Hints[EncodeHintType.PDF417_COMPACT];
      }
      set => this.Hints[EncodeHintType.PDF417_COMPACT] = (object) value;
    }

    /// <summary>
    /// Specifies what compaction mode to use for PDF417 (type
    /// <see cref="P:ZXing.PDF417.PDF417EncodingOptions.Compaction" />).
    /// </summary>
    public Compaction Compaction
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.PDF417_COMPACTION) ? (Compaction) this.Hints[EncodeHintType.PDF417_COMPACTION] : Compaction.AUTO;
      }
      set => this.Hints[EncodeHintType.PDF417_COMPACTION] = (object) value;
    }

    /// <summary>
    /// Specifies the minimum and maximum number of rows and columns for PDF417 (type
    /// <see cref="P:ZXing.PDF417.PDF417EncodingOptions.Dimensions" />).
    /// </summary>
    public Dimensions Dimensions
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.PDF417_DIMENSIONS) ? (Dimensions) this.Hints[EncodeHintType.PDF417_DIMENSIONS] : (Dimensions) null;
      }
      set => this.Hints[EncodeHintType.PDF417_DIMENSIONS] = (object) value;
    }

    /// <summary>Specifies what degree of error correction to use</summary>
    public PDF417ErrorCorrectionLevel ErrorCorrection
    {
      get
      {
        if (this.Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
        {
          object hint = this.Hints[EncodeHintType.ERROR_CORRECTION];
          switch (hint)
          {
            case PDF417ErrorCorrectionLevel errorCorrection:
              return errorCorrection;
            case int _:
              return (PDF417ErrorCorrectionLevel) Enum.Parse(typeof (PDF417ErrorCorrectionLevel), hint.ToString(), true);
          }
        }
        return PDF417ErrorCorrectionLevel.L2;
      }
      set => this.Hints[EncodeHintType.ERROR_CORRECTION] = (object) value;
    }

    /// <summary>
    /// Specifies what character encoding to use where applicable (type {@link String})
    /// </summary>
    public string CharacterSet
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.CHARACTER_SET) ? (string) this.Hints[EncodeHintType.CHARACTER_SET] : (string) null;
      }
      set
      {
        if (value == null)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
            return;
          this.Hints.Remove(EncodeHintType.CHARACTER_SET);
        }
        else
          this.Hints[EncodeHintType.CHARACTER_SET] = (object) value;
      }
    }

    /// <summary>
    /// Explicitly disables ECI segment when generating PDF417 Code
    /// That is against the specification but some
    /// readers have problems if the charset is switched from
    /// CP437 (default) to UTF-8 with the necessary ECI segment.
    /// If you set the property to true you can use different encodings
    /// and the ECI segment is omitted.
    /// </summary>
    public bool DisableECI
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.DISABLE_ECI) && (bool) this.Hints[EncodeHintType.DISABLE_ECI];
      }
      set => this.Hints[EncodeHintType.DISABLE_ECI] = (object) value;
    }
  }
}
