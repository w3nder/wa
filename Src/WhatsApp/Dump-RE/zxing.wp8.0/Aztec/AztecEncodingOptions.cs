// Decompiled with JetBrains decompiler
// Type: ZXing.Aztec.AztecEncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.Aztec
{
  /// <summary>
  /// The class holds the available options for the <see cref="T:ZXing.Aztec.AztecWriter" />
  /// </summary>
  [Serializable]
  public class AztecEncodingOptions : EncodingOptions
  {
    /// <summary>
    /// Representing the minimal percentage of error correction words.
    /// Note: an Aztec symbol should have a minimum of 25% EC words.
    /// </summary>
    public int? ErrorCorrection
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION) ? new int?((int) this.Hints[EncodeHintType.ERROR_CORRECTION]) : new int?();
      }
      set
      {
        if (!value.HasValue)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
            return;
          this.Hints.Remove(EncodeHintType.ERROR_CORRECTION);
        }
        else
          this.Hints[EncodeHintType.ERROR_CORRECTION] = (object) value;
      }
    }

    /// <summary>
    /// Specifies the required number of layers for an Aztec code:
    /// a negative number (-1, -2, -3, -4) specifies a compact Aztec code
    /// 0 indicates to use the minimum number of layers (the default)
    /// a positive number (1, 2, .. 32) specifies a normal (non-compact) Aztec code
    /// </summary>
    public int? Layers
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS) ? new int?((int) this.Hints[EncodeHintType.AZTEC_LAYERS]) : new int?();
      }
      set
      {
        if (!value.HasValue)
        {
          if (!this.Hints.ContainsKey(EncodeHintType.AZTEC_LAYERS))
            return;
          this.Hints.Remove(EncodeHintType.AZTEC_LAYERS);
        }
        else
          this.Hints[EncodeHintType.AZTEC_LAYERS] = (object) value;
      }
    }
  }
}
