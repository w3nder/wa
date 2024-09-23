// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.URIResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>Tries to parse results that are a URI of some kind.</summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class URIResultParser : ResultParser
  {
    private static readonly Regex URL_WITH_PROTOCOL_PATTERN = new Regex("[a-zA-Z0-9]{2,}:", RegexOptions.Compiled);
    private static readonly Regex URL_WITHOUT_PROTOCOL_PATTERN = new Regex("([a-zA-Z0-9\\-]+\\.)+[a-zA-Z]{2,}(:\\d{1,5})?(/|\\?|$)", RegexOptions.Compiled);

    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text.StartsWith("URL:") || text.StartsWith("URI:"))
        return (ParsedResult) new URIParsedResult(text.Substring(4).Trim(), (string) null);
      string uri = text.Trim();
      return !URIResultParser.isBasicallyValidURI(uri) ? (ParsedResult) null : (ParsedResult) new URIParsedResult(uri, (string) null);
    }

    internal static bool isBasicallyValidURI(string uri)
    {
      if (uri.IndexOf(" ") >= 0)
        return false;
      Match match1 = URIResultParser.URL_WITH_PROTOCOL_PATTERN.Match(uri);
      if (match1.Success && match1.Index == 0)
        return true;
      Match match2 = URIResultParser.URL_WITHOUT_PROTOCOL_PATTERN.Match(uri);
      return match2.Success && match2.Index == 0;
    }
  }
}
