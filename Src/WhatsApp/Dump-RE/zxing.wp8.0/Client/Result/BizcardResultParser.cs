// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.BizcardResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Collections.Generic;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Implements the "BIZCARD" address book entry format, though this has been
  /// largely reverse-engineered from examples observed in the wild -- still
  /// looking for a definitive reference.
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class BizcardResultParser : AbstractDoCoMoResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null || !text.StartsWith("BIZCARD:"))
        return (ParsedResult) null;
      string value_Renamed1 = BizcardResultParser.buildName(AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("N:", text, true), AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("X:", text, true));
      string title = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("T:", text, true);
      string org = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("C:", text, true);
      string[] addresses = AbstractDoCoMoResultParser.matchDoCoMoPrefixedField("A:", text, true);
      string number1 = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("B:", text, true);
      string number2 = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("M:", text, true);
      string number3 = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("F:", text, true);
      string value_Renamed2 = AbstractDoCoMoResultParser.matchSingleDoCoMoPrefixedField("E:", text, true);
      return (ParsedResult) new AddressBookParsedResult(ResultParser.maybeWrap(value_Renamed1), (string[]) null, (string) null, BizcardResultParser.buildPhoneNumbers(number1, number2, number3), (string[]) null, ResultParser.maybeWrap(value_Renamed2), (string[]) null, (string) null, (string) null, addresses, (string[]) null, org, (string) null, title, (string[]) null, (string[]) null);
    }

    private static string[] buildPhoneNumbers(string number1, string number2, string number3)
    {
      List<string> stringList = new List<string>();
      if (number1 != null)
        stringList.Add(number1);
      if (number2 != null)
        stringList.Add(number2);
      if (number3 != null)
        stringList.Add(number3);
      return stringList.Count == 0 ? (string[]) null : SupportClass.toStringArray((ICollection<string>) stringList);
    }

    private static string buildName(string firstName, string lastName)
    {
      if (firstName == null)
        return lastName;
      return lastName != null ? firstName + (object) ' ' + lastName : firstName;
    }
  }
}
