// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.AddressBookDoCoMoResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Implements the "MECARD" address book entry format.
  /// 
  /// Supported keys: N, SOUND, TEL, EMAIL, NOTE, ADR, BDAY, URL, plus ORG
  /// Unsupported keys: TEL-AV, NICKNAME
  /// 
  /// Except for TEL, multiple values for keys are also not supported;
  /// the first one found takes precedence.
  /// 
  /// Our understanding of the MECARD format is based on this document:
  /// 
  /// http://www.mobicode.org.tw/files/OMIA%20Mobile%20Bar%20Code%20Standard%20v3.2.1.doc
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class AddressBookDoCoMoResultParser : AbstractDoCoMoResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("MECARD:"))
        return (ParsedResult) null;
      string[] strArray = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("N:", text, true);
      if (strArray == null)
        return (ParsedResult) null;
      string name = AddressBookDoCoMoResultParser.parseName(strArray[0]);
      string pronunciation = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("SOUND:", text, true);
      string[] phoneNumbers = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("TEL:", text, true);
      string[] emails = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("EMAIL:", text, true);
      string note = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("NOTE:", text, false);
      string[] addresses = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("ADR:", text, true);
      string birthday = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("BDAY:", text, true);
      if (!ResultParser.isStringOfDigits(birthday, 8))
        birthday = (string) null;
      string[] urls = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("URL:", text, true);
      string org = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("ORG:", text, true);
      return (ParsedResult) new AddressBookParsedResult(ResultParser.maybeWrap(name), (string[]) null, pronunciation, phoneNumbers, (string[]) null, emails, (string[]) null, (string) null, note, addresses, (string[]) null, org, birthday, (string) null, urls, (string[]) null);
    }

    private static string parseName(string name)
    {
      int length = name.IndexOf(',');
      return length >= 0 ? name.Substring(length + 1) + (object) ' ' + name.Substring(0, length) : name;
    }
  }
}
