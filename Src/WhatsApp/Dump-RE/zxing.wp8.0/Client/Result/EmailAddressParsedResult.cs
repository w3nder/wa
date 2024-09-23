// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.EmailAddressParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class EmailAddressParsedResult : ParsedResult
  {
    public string EmailAddress { get; private set; }

    public string Subject { get; private set; }

    public string Body { get; private set; }

    public string MailtoURI { get; private set; }

    internal EmailAddressParsedResult(
      string emailAddress,
      string subject,
      string body,
      string mailtoURI)
      : base(ParsedResultType.EMAIL_ADDRESS)
    {
      this.EmailAddress = emailAddress;
      this.Subject = subject;
      this.Body = body;
      this.MailtoURI = mailtoURI;
      StringBuilder result = new StringBuilder(30);
      ParsedResult.maybeAppend(this.EmailAddress, result);
      ParsedResult.maybeAppend(this.Subject, result);
      ParsedResult.maybeAppend(this.Body, result);
      this.displayResultValue = result.ToString();
    }
  }
}
