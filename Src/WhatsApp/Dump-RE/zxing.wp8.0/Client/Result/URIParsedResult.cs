// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.URIParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class URIParsedResult : ParsedResult
  {
    private static readonly Regex USER_IN_HOST = new Regex(":/*([^/@]+)@[^/]+", RegexOptions.Compiled);

    public string URI { get; private set; }

    public string Title { get; private set; }

    /// <returns> true if the URI contains suspicious patterns that may suggest it intends to
    /// mislead the user about its true nature. At the moment this looks for the presence
    /// of user/password syntax in the host/authority portion of a URI which may be used
    /// in attempts to make the URI's host appear to be other than it is. Example:
    /// http://yourbank.com@phisher.com  This URI connects to phisher.com but may appear
    /// to connect to yourbank.com at first glance.
    /// </returns>
    public bool PossiblyMaliciousURI { get; private set; }

    public URIParsedResult(string uri, string title)
      : base(ParsedResultType.URI)
    {
      this.URI = URIParsedResult.massageURI(uri);
      this.Title = title;
      this.PossiblyMaliciousURI = URIParsedResult.USER_IN_HOST.Match(this.URI).Success;
      StringBuilder result = new StringBuilder(30);
      ParsedResult.maybeAppend(this.Title, result);
      ParsedResult.maybeAppend(this.URI, result);
      this.displayResultValue = result.ToString();
    }

    /// <summary> Transforms a string that represents a URI into something more proper, by adding or canonicalizing
    /// the protocol.
    /// </summary>
    private static string massageURI(string uri)
    {
      int protocolEnd = uri.IndexOf(':');
      if (protocolEnd < 0)
        uri = "http://" + uri;
      else if (URIParsedResult.isColonFollowedByPortNumber(uri, protocolEnd))
        uri = "http://" + uri;
      return uri;
    }

    private static bool isColonFollowedByPortNumber(string uri, int protocolEnd)
    {
      int num1 = protocolEnd + 1;
      int num2 = uri.IndexOf('/', num1);
      if (num2 < 0)
        num2 = uri.Length;
      return ResultParser.isSubstringOfDigits(uri, num1, num2 - num1);
    }
  }
}
