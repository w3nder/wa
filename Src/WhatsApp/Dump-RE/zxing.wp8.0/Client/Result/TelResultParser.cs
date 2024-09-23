// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.TelResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// Parses a "tel:" URI result, which specifies a phone number.
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class TelResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("tel:") && !text.StartsWith("TEL:"))
        return (ParsedResult) null;
      string telURI = text.StartsWith("TEL:") ? "tel:" + text.Substring(4) : text;
      int num = text.IndexOf('?', 4);
      return (ParsedResult) new TelParsedResult(num < 0 ? text.Substring(4) : text.Substring(4, num - 4), telURI, (string) null);
    }
  }
}
