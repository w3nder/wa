// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.SMSTOMMSTOResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// <p>Parses an "smsto:" URI result, whose format is not standardized but appears to be like:
  /// {@code smsto:number(:body)}.</p>
  /// <p>This actually also parses URIs starting with "smsto:", "mmsto:", "SMSTO:", and
  /// "MMSTO:", and treats them all the same way, and effectively converts them to an "sms:" URI
  /// for purposes of forwarding to the platform.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public class SMSTOMMSTOResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (!text.StartsWith("smsto:") && !text.StartsWith("SMSTO:") && !text.StartsWith("mmsto:") && !text.StartsWith("MMSTO:"))
        return (ParsedResult) null;
      string number = text.Substring(6);
      string body = (string) null;
      int length = number.IndexOf(':');
      if (length >= 0)
      {
        body = number.Substring(length + 1);
        number = number.Substring(0, length);
      }
      return (ParsedResult) new SMSParsedResult(number, (string) null, (string) null, body);
    }
  }
}
