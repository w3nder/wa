// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.SMSMMSResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> <p>Parses an "sms:" URI result, which specifies a number to SMS and optional
  /// "via" number. See <a href="http://gbiv.com/protocols/uri/drafts/draft-antti-gsm-sms-url-04.txt">
  /// the IETF draft</a> on this.</p>
  /// 
  /// <p>This actually also parses URIs starting with "mms:", "smsto:", "mmsto:", "SMSTO:", and
  /// "MMSTO:", and treats them all the same way, and effectively converts them to an "sms:" URI
  /// for purposes of forwarding to the platform.</p>
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class SMSMMSResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("sms:") && !text.StartsWith("SMS:") && !text.StartsWith("mms:") && !text.StartsWith("MMS:"))
        return (ParsedResult) null;
      IDictionary<string, string> nameValuePairs = ResultParser.parseNameValuePairs(text);
      string subject = (string) null;
      string body = (string) null;
      bool flag = false;
      if (nameValuePairs != null && nameValuePairs.Count != 0)
      {
        subject = nameValuePairs["subject"];
        body = nameValuePairs["body"];
        flag = true;
      }
      int num1 = text.IndexOf('?', 4);
      string str = num1 < 0 || !flag ? text.Substring(4) : text.Substring(4, num1 - 4);
      int num2 = -1;
      List<string> numbers = new List<string>(1);
      List<string> vias = new List<string>(1);
      int length;
      for (; (length = str.IndexOf(',', num2 + 1)) > num2; num2 = length)
      {
        string numberPart = str.Substring(num2 + 1, length);
        SMSMMSResultParser.addNumberVia((ICollection<string>) numbers, (ICollection<string>) vias, numberPart);
      }
      SMSMMSResultParser.addNumberVia((ICollection<string>) numbers, (ICollection<string>) vias, str.Substring(num2 + 1));
      return (ParsedResult) new SMSParsedResult(SupportClass.toStringArray((ICollection<string>) numbers), SupportClass.toStringArray((ICollection<string>) vias), subject, body);
    }

    private static void addNumberVia(
      ICollection<string> numbers,
      ICollection<string> vias,
      string numberPart)
    {
      int length = numberPart.IndexOf(';');
      if (length < 0)
      {
        numbers.Add(numberPart);
        vias.Add((string) null);
      }
      else
      {
        numbers.Add(numberPart.Substring(0, length));
        string str1 = numberPart.Substring(length + 1);
        string str2 = !str1.StartsWith("via=") ? (string) null : str1.Substring(4);
        vias.Add(str2);
      }
    }
  }
}
