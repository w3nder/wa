// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.BookmarkDoCoMoResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class BookmarkDoCoMoResultParser : AbstractDoCoMoResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("MEBKM:"))
        return (ParsedResult) null;
      string title = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("TITLE:", text, true);
      string[] strArray = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("URL:", text, true);
      if (strArray == null)
        return (ParsedResult) null;
      string uri = strArray[0];
      return !URIResultParser.isBasicallyValidURI(uri) ? (ParsedResult) null : (ParsedResult) new URIParsedResult(uri, title);
    }
  }
}
