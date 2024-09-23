// Decompiled with JetBrains decompiler
// Type: ZXing.OneD.Code128EncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;

#nullable disable
namespace ZXing.OneD
{
  /// <summary>
  /// The class holds the available options for the QrCodeWriter
  /// </summary>
  [Serializable]
  public class Code128EncodingOptions : EncodingOptions
  {
    /// <summary>if true, don't switch to codeset C for numbers</summary>
    public bool ForceCodesetB
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.CODE128_FORCE_CODESET_B) && (bool) this.Hints[EncodeHintType.CODE128_FORCE_CODESET_B];
      }
      set => this.Hints[EncodeHintType.CODE128_FORCE_CODESET_B] = (object) value;
    }
  }
}
