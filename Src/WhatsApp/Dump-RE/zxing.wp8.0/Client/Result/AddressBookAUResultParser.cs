// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.AddressBookAUResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Implements KDDI AU's address book format. See
  /// <a href="http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html">
  /// http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html</a>.
  /// (Thanks to Yuzo for translating!)
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class AddressBookAUResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || text.IndexOf("MEMORY") < 0 || text.IndexOf("\r\n") < 0)
        return (ParsedResult) null;
      string value_Renamed = ResultParser.matchSinglePrefixedField("NAME1:", text, '\r', true);
      string pronunciation = ResultParser.matchSinglePrefixedField("NAME2:", text, '\r', true);
      string[] phoneNumbers = AddressBookAUResultParser.matchMultipleValuePrefix("TEL", 3, text, true);
      string[] emails = AddressBookAUResultParser.matchMultipleValuePrefix("MAIL", 3, text, true);
      string note = ResultParser.matchSinglePrefixedField("MEMORY:", text, '\r', false);
      string str = ResultParser.matchSinglePrefixedField("ADD:", text, '\r', true);
      string[] strArray;
      if (str != null)
        strArray = new string[1]{ str };
      else
        strArray = (string[]) null;
      string[] addresses = strArray;
      return (ParsedResult) new AddressBookParsedResult(ResultParser.maybeWrap(value_Renamed), (string[]) null, pronunciation, phoneNumbers, (string[]) null, emails, (string[]) null, (string) null, note, addresses, (string[]) null, (string) null, (string) null, (string) null, (string[]) null, (string[]) null);
    }

    private static string[] matchMultipleValuePrefix(
      string prefix,
      int max,
      string rawText,
      bool trim)
    {
      IList<string> stringList = (IList<string>) null;
      for (int index = 1; index <= max; ++index)
      {
        string str = ResultParser.matchSinglePrefixedField(prefix + (object) index + (object) ':', rawText, '\r', trim);
        if (str != null)
        {
          if (stringList == null)
            stringList = (IList<string>) new List<string>();
          stringList.Add(str);
        }
        else
          break;
      }
      return stringList == null ? (string[]) null : SupportClass.toStringArray((ICollection<string>) stringList);
    }
  }
}
