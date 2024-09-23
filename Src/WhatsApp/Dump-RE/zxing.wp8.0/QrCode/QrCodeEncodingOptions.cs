// Decompiled with JetBrains decompiler
// Type: ZXing.QrCode.QrCodeEncodingOptions
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using ZXing.Common;
using ZXing.QrCode.Internal;

#nullable disable
namespace ZXing.QrCode
{
  /// <summary>
  /// The class holds the available options for the QrCodeWriter
  /// </summary>
  [Serializable]
  public class QrCodeEncodingOptions : EncodingOptions
  {
    /// <summary>
    /// Specifies what degree of error correction to use, for example in QR Codes.
    /// Type depends on the encoder. For example for QR codes it's type
    /// {@link com.google.zxing.qrcode.decoder.ErrorCorrectionLevel ErrorCorrectionLevel}.
    /// </summary>
    public ErrorCorrectionLevel ErrorCorrection
    {
      get
      {
        return this.Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION) ? (ErrorCorrectionLevel) this.Hints[EncodeHintType.ERROR_CORRECTION] : (ErrorCorrectionLevel) null;
      }
      set
      {
        if (value == null)
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
    /// Explicitly disables ECI segment when generating QR Code
    /// That is against the specification of QR Code but some
    /// readers have problems if the charset is switched from
    /// ISO-8859-1 (default) to UTF-8 with the necessary ECI segment.
    /// If you set the property to true you can use UTF-8 encoding
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
