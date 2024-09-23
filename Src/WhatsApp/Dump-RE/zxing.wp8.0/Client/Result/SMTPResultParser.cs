// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.SMTPResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// <p>Parses an "smtp:" URI result, whose format is not standardized but appears to be like:
  /// <code>smtp[:subject[:body]]}</code>.</p>
  /// <p>See http://code.google.com/p/zxing/issues/detail?id=536</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public class SMTPResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (!text.StartsWith("smtp:") && !text.StartsWith("SMTP:"))
        return (ParsedResult) null;
      string emailAddress = text.Substring(5);
      string subject = (string) null;
      string body = (string) null;
      int length1 = emailAddress.IndexOf(':');
      if (length1 >= 0)
      {
        subject = emailAddress.Substring(length1 + 1);
        emailAddress = emailAddress.Substring(0, length1);
        int length2 = subject.IndexOf(':');
        if (length2 >= 0)
        {
          body = subject.Substring(length2 + 1);
          subject = subject.Substring(0, length2);
        }
      }
      string mailtoURI = "mailto:" + emailAddress;
      return (ParsedResult) new EmailAddressParsedResult(emailAddress, subject, body, mailtoURI);
    }
  }
}
