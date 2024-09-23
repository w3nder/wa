// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.URLTOResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Parses the "URLTO" result format, which is of the form "URLTO:[title]:[url]".
  /// This seems to be used sometimes, but I am not able to find documentation
  /// on its origin or official format?
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class URLTOResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("urlto:") && !text.StartsWith("URLTO:"))
        return (ParsedResult) null;
      int num = text.IndexOf(':', 6);
      if (num < 0)
        return (ParsedResult) null;
      string title = num <= 6 ? (string) null : text.Substring(6, num - 6);
      return (ParsedResult) new URIParsedResult(text.Substring(num + 1), title);
    }
  }
}
