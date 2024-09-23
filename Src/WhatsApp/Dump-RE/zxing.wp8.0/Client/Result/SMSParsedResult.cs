// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.SMSParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class SMSParsedResult : ParsedResult
  {
    public SMSParsedResult(string number, string via, string subject, string body)
      : this(new string[1]{ number }, new string[1]{ via }, subject, body)
    {
    }

    public SMSParsedResult(string[] numbers, string[] vias, string subject, string body)
      : base(ParsedResultType.SMS)
    {
      this.Numbers = numbers;
      this.Vias = vias;
      this.Subject = subject;
      this.Body = body;
      this.SMSURI = this.getSMSURI();
      StringBuilder result = new StringBuilder(100);
      ParsedResult.maybeAppend(this.Numbers, result);
      ParsedResult.maybeAppend(this.Subject, result);
      ParsedResult.maybeAppend(this.Body, result);
      this.displayResultValue = result.ToString();
    }

    private string getSMSURI()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("sms:");
      bool flag1 = true;
      for (int index = 0; index < this.Numbers.Length; ++index)
      {
        if (flag1)
          flag1 = false;
        else
          stringBuilder.Append(',');
        stringBuilder.Append(this.Numbers[index]);
        if (this.Vias != null && this.Vias[index] != null)
        {
          stringBuilder.Append(";via=");
          stringBuilder.Append(this.Vias[index]);
        }
      }
      bool flag2 = this.Body != null;
      bool flag3 = this.Subject != null;
      if (flag2 || flag3)
      {
        stringBuilder.Append('?');
        if (flag2)
        {
          stringBuilder.Append("body=");
          stringBuilder.Append(this.Body);
        }
        if (flag3)
        {
          if (flag2)
            stringBuilder.Append('&');
          stringBuilder.Append("subject=");
          stringBuilder.Append(this.Subject);
        }
      }
      return stringBuilder.ToString();
    }

    public string[] Numbers { get; private set; }

    public string[] Vias { get; private set; }

    public string Subject { get; private set; }

    public string Body { get; private set; }

    public string SMSURI { get; private set; }
  }
}
