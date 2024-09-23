// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.EmailDoCoMoResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// Implements the "MATMSG" email message entry format.
  /// 
  /// Supported keys: TO, SUB, BODY
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class EmailDoCoMoResultParser : AbstractDoCoMoResultParser
  {
    private static readonly Regex ATEXT_ALPHANUMERIC = new Regex("\\A(?:[a-zA-Z0-9@.!#$%&'*+\\-/=?^_`{|}~]+)\\z", RegexOptions.Compiled);

    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (!text.StartsWith("MATMSG:"))
        return (ParsedResult) null;
      string[] strArray = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("TO:", text, true);
      if (strArray == null)
        return (ParsedResult) null;
      string str = strArray[0];
      if (!EmailDoCoMoResultParser.isBasicallyValidEmailAddress(str))
        return (ParsedResult) null;
      string subject = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("SUB:", text, false);
      string body = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("BODY:", text, false);
      return (ParsedResult) new EmailAddressParsedResult(str, subject, body, "mailto:" + str);
    }

    /// This implements only the most basic checking for an email address's validity -- that it contains
    ///             an '@' and contains no characters disallowed by RFC 2822. This is an overly lenient definition of
    ///             validity. We want to generally be lenient here since this class is only intended to encapsulate what's
    ///             in a barcode, not "judge" it.
    internal static bool isBasicallyValidEmailAddress(string email)
    {
      return email != null && EmailDoCoMoResultParser.ATEXT_ALPHANUMERIC.Match(email).Success && email.IndexOf('@') >= 0;
    }
  }
}
