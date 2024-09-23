// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.EmailAddressResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Represents a result that encodes an e-mail address, either as a plain address
  /// like "joe@example.org" or a mailto: URL like "mailto:joe@example.org".
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class EmailAddressResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null)
        return (ParsedResult) null;
      if (text.ToLower().StartsWith("mailto:"))
      {
        string escaped = text.Substring(7);
        int length = escaped.IndexOf('?');
        if (length >= 0)
          escaped = escaped.Substring(0, length);
        string emailAddress = ResultParser.urlDecode(escaped);
        IDictionary<string, string> nameValuePairs = ResultParser.parseNameValuePairs(text);
        string subject = (string) null;
        string body = (string) null;
        if (nameValuePairs != null)
        {
          if (string.IsNullOrEmpty(emailAddress))
            emailAddress = nameValuePairs["to"];
          subject = nameValuePairs["subject"];
          body = nameValuePairs["body"];
        }
        return (ParsedResult) new EmailAddressParsedResult(emailAddress, subject, body, text);
      }
      if (!EmailDoCoMoResultParser.isBasicallyValidEmailAddress(text))
        return (ParsedResult) null;
      string emailAddress1 = text;
      return (ParsedResult) new EmailAddressParsedResult(emailAddress1, (string) null, (string) null, "mailto:" + emailAddress1);
    }
  }
}
