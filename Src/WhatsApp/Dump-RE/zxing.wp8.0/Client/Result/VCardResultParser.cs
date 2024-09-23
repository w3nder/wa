// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.VCardResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  internal sealed class VCardResultParser : ResultParser
  {
    private static readonly Regex BEGIN_VCARD = new Regex("BEGIN:VCARD", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex VCARD_LIKE_DATE = new Regex("\\A(?:\\d{4}-?\\d{2}-?\\d{2})\\z", RegexOptions.Compiled);
    private static readonly Regex CR_LF_SPACE_TAB = new Regex("\r\n[ \t]", RegexOptions.Compiled);
    private static readonly Regex NEWLINE_ESCAPE = new Regex("\\\\[nN]", RegexOptions.Compiled);
    private static readonly Regex VCARD_ESCAPES = new Regex("\\\\([,;\\\\])", RegexOptions.Compiled);
    private static readonly Regex EQUALS = new Regex("=", RegexOptions.Compiled);
    private static readonly Regex SEMICOLON = new Regex(";", RegexOptions.Compiled);
    private static readonly Regex UNESCAPED_SEMICOLONS = new Regex("(?<!\\\\);+", RegexOptions.Compiled);
    private static readonly Regex COMMA = new Regex(",", RegexOptions.Compiled);
    private static readonly Regex SEMICOLON_OR_COMMA = new Regex("[;,]", RegexOptions.Compiled);

    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      Match match = VCardResultParser.BEGIN_VCARD.Match(text);
      if (!match.Success || match.Index != 0)
        return (ParsedResult) null;
      List<List<string>> stringListList = VCardResultParser.matchVCardPrefixedField("FN", text, true, false);
      if (stringListList == null)
      {
        stringListList = VCardResultParser.matchVCardPrefixedField("N", text, true, false);
        VCardResultParser.formatNames((IEnumerable<List<string>>) stringListList);
      }
      List<string> stringList1 = VCardResultParser.matchSingleVCardPrefixedField("NICKNAME", text, true, false);
      string[] nicknames = stringList1 == null ? (string[]) null : VCardResultParser.COMMA.Split(stringList1[0]);
      List<List<string>> lists1 = VCardResultParser.matchVCardPrefixedField("TEL", text, true, false);
      List<List<string>> lists2 = VCardResultParser.matchVCardPrefixedField("EMAIL", text, true, false);
      List<string> list1 = VCardResultParser.matchSingleVCardPrefixedField("NOTE", text, false, false);
      List<List<string>> lists3 = VCardResultParser.matchVCardPrefixedField("ADR", text, true, true);
      List<string> list2 = VCardResultParser.matchSingleVCardPrefixedField("ORG", text, true, true);
      List<string> list3 = VCardResultParser.matchSingleVCardPrefixedField("BDAY", text, true, false);
      if (list3 != null && !VCardResultParser.isLikeVCardDate(list3[0]))
        list3 = (List<string>) null;
      List<string> list4 = VCardResultParser.matchSingleVCardPrefixedField("TITLE", text, true, false);
      List<List<string>> lists4 = VCardResultParser.matchVCardPrefixedField("URL", text, true, false);
      List<string> list5 = VCardResultParser.matchSingleVCardPrefixedField("IMPP", text, true, false);
      List<string> stringList2 = VCardResultParser.matchSingleVCardPrefixedField("GEO", text, true, false);
      string[] geo = stringList2 == null ? (string[]) null : VCardResultParser.SEMICOLON_OR_COMMA.Split(stringList2[0]);
      if (geo != null && geo.Length != 2)
        geo = (string[]) null;
      return (ParsedResult) new AddressBookParsedResult(VCardResultParser.toPrimaryValues((ICollection<List<string>>) stringListList), nicknames, (string) null, VCardResultParser.toPrimaryValues((ICollection<List<string>>) lists1), VCardResultParser.toTypes((ICollection<List<string>>) lists1), VCardResultParser.toPrimaryValues((ICollection<List<string>>) lists2), VCardResultParser.toTypes((ICollection<List<string>>) lists2), VCardResultParser.toPrimaryValue(list5), VCardResultParser.toPrimaryValue(list1), VCardResultParser.toPrimaryValues((ICollection<List<string>>) lists3), VCardResultParser.toTypes((ICollection<List<string>>) lists3), VCardResultParser.toPrimaryValue(list2), VCardResultParser.toPrimaryValue(list3), VCardResultParser.toPrimaryValue(list4), VCardResultParser.toPrimaryValues((ICollection<List<string>>) lists4), geo);
    }

    public static List<List<string>> matchVCardPrefixedField(
      string prefix,
      string rawText,
      bool trim,
      bool parseFieldDivider)
    {
      List<List<string>> stringListList = (List<List<string>>) null;
      int startat = 0;
      int length = rawText.Length;
      while (startat < length)
      {
        Regex regex = new Regex("(?:^|\n)" + prefix + "(?:;([^:]*))?:", RegexOptions.IgnoreCase);
        if (startat > 0)
          --startat;
        Match match = regex.Match(rawText, startat);
        if (match.Success)
        {
          int startIndex1 = match.Index + match.Length;
          string input1 = match.Groups[1].Value;
          List<string> stringList = (List<string>) null;
          bool flag = false;
          string charset = (string) null;
          if (input1 != null)
          {
            foreach (string input2 in VCardResultParser.SEMICOLON.Split(input1))
            {
              if (stringList == null)
                stringList = new List<string>(1);
              stringList.Add(input2);
              string[] strArray = VCardResultParser.EQUALS.Split(input2, 2);
              if (strArray.Length > 1)
              {
                string strB1 = strArray[0];
                string strB2 = strArray[1];
                if (string.Compare("ENCODING", strB1, StringComparison.OrdinalIgnoreCase) == 0 && string.Compare("QUOTED-PRINTABLE", strB2, StringComparison.OrdinalIgnoreCase) == 0)
                  flag = true;
                else if (string.Compare("CHARSET", strB1, StringComparison.OrdinalIgnoreCase) == 0)
                  charset = strB2;
              }
            }
          }
          int startIndex2 = startIndex1;
          int num;
          while ((num = rawText.IndexOf('\n', startIndex1)) >= 0)
          {
            if (num < rawText.Length - 1 && (rawText[num + 1] == ' ' || rawText[num + 1] == '\t'))
              startIndex1 = num + 2;
            else if (flag && (num >= 1 && rawText[num - 1] == '=' || num >= 2 && rawText[num - 2] == '='))
              startIndex1 = num + 1;
            else
              break;
          }
          if (num < 0)
            startat = length;
          else if (num > startIndex2)
          {
            if (stringListList == null)
              stringListList = new List<List<string>>(1);
            if (num >= 1 && rawText[num - 1] == '\r')
              --num;
            string input3 = rawText.Substring(startIndex2, num - startIndex2);
            if (trim)
              input3 = input3.Trim();
            string input4;
            if (flag)
            {
              input4 = VCardResultParser.decodeQuotedPrintable(input3, charset);
              if (parseFieldDivider)
                input4 = VCardResultParser.UNESCAPED_SEMICOLONS.Replace(input4, "\n").Trim();
            }
            else
            {
              if (parseFieldDivider)
                input3 = VCardResultParser.UNESCAPED_SEMICOLONS.Replace(input3, "\n").Trim();
              string input5 = VCardResultParser.CR_LF_SPACE_TAB.Replace(input3, "");
              string input6 = VCardResultParser.NEWLINE_ESCAPE.Replace(input5, "\n");
              input4 = VCardResultParser.VCARD_ESCAPES.Replace(input6, "$1");
            }
            if (stringList == null)
            {
              stringListList.Add(new List<string>(1)
              {
                input4
              });
            }
            else
            {
              stringList.Insert(0, input4);
              stringListList.Add(stringList);
            }
            startat = num + 1;
          }
          else
            startat = num + 1;
        }
        else
          break;
      }
      return stringListList;
    }

    private static string decodeQuotedPrintable(string value, string charset)
    {
      int length = value.Length;
      StringBuilder result = new StringBuilder(length);
      MemoryStream fragmentBuffer = new MemoryStream();
      for (int index = 0; index < length; ++index)
      {
        char ch = value[index];
        switch (ch)
        {
          case '\n':
          case '\r':
            continue;
          case '=':
            if (index < length - 2)
            {
              char c1 = value[index + 1];
              switch (c1)
              {
                case '\n':
                case '\r':
                  continue;
                default:
                  char c2 = value[index + 2];
                  int hexDigit1 = ResultParser.parseHexDigit(c1);
                  int hexDigit2 = ResultParser.parseHexDigit(c2);
                  if (hexDigit1 >= 0 && hexDigit2 >= 0)
                    fragmentBuffer.WriteByte((byte) (hexDigit1 << 4 | hexDigit2));
                  index += 2;
                  continue;
              }
            }
            else
              continue;
          default:
            VCardResultParser.maybeAppendFragment(fragmentBuffer, charset, result);
            result.Append(ch);
            continue;
        }
      }
      VCardResultParser.maybeAppendFragment(fragmentBuffer, charset, result);
      return result.ToString();
    }

    private static void maybeAppendFragment(
      MemoryStream fragmentBuffer,
      string charset,
      StringBuilder result)
    {
      if (fragmentBuffer.Length <= 0L)
        return;
      byte[] array = fragmentBuffer.ToArray();
      string str;
      if (charset == null)
      {
        str = Encoding.UTF8.GetString(array, 0, array.Length);
      }
      else
      {
        try
        {
          str = Encoding.GetEncoding(charset).GetString(array, 0, array.Length);
        }
        catch (Exception ex)
        {
          str = Encoding.UTF8.GetString(array, 0, array.Length);
        }
      }
      fragmentBuffer.Seek(0L, SeekOrigin.Begin);
      fragmentBuffer.SetLength(0L);
      result.Append(str);
    }

    internal static List<string> matchSingleVCardPrefixedField(
      string prefix,
      string rawText,
      bool trim,
      bool parseFieldDivider)
    {
      List<List<string>> stringListList = VCardResultParser.matchVCardPrefixedField(prefix, rawText, trim, parseFieldDivider);
      return stringListList != null && stringListList.Count != 0 ? stringListList[0] : (List<string>) null;
    }

    private static string toPrimaryValue(List<string> list)
    {
      return list != null && list.Count != 0 ? list[0] : (string) null;
    }

    private static string[] toPrimaryValues(ICollection<List<string>> lists)
    {
      if (lists == null || lists.Count == 0)
        return (string[]) null;
      List<string> stringList = new List<string>(lists.Count);
      foreach (List<string> list in (IEnumerable<List<string>>) lists)
      {
        string str = list[0];
        if (!string.IsNullOrEmpty(str))
          stringList.Add(str);
      }
      return SupportClass.toStringArray((ICollection<string>) stringList);
    }

    private static string[] toTypes(ICollection<List<string>> lists)
    {
      if (lists == null || lists.Count == 0)
        return (string[]) null;
      List<string> stringList = new List<string>(lists.Count);
      foreach (List<string> list in (IEnumerable<List<string>>) lists)
      {
        string str1 = (string) null;
        for (int index = 1; index < list.Count; ++index)
        {
          string str2 = list[index];
          int length = str2.IndexOf('=');
          if (length < 0)
          {
            str1 = str2;
            break;
          }
          if (string.Compare("TYPE", str2.Substring(0, length), StringComparison.OrdinalIgnoreCase) == 0)
          {
            str1 = str2.Substring(length + 1);
            break;
          }
        }
        stringList.Add(str1);
      }
      return SupportClass.toStringArray((ICollection<string>) stringList);
    }

    private static bool isLikeVCardDate(string value)
    {
      return value == null || VCardResultParser.VCARD_LIKE_DATE.Match(value).Success;
    }

    /// Formats name fields of the form "Public;John;Q.;Reverend;III" into a form like
    ///              "Reverend John Q. Public III".
    ///             
    ///              @param names name values to format, in place
    private static void formatNames(IEnumerable<List<string>> names)
    {
      if (names == null)
        return;
      foreach (List<string> name in names)
      {
        string str = name[0];
        string[] components = new string[5];
        int startIndex = 0;
        int index;
        int num;
        for (index = 0; index < components.Length - 1 && (num = str.IndexOf(';', startIndex)) >= 0; startIndex = num + 1)
        {
          components[index] = str.Substring(startIndex, num - startIndex);
          ++index;
        }
        components[index] = str.Substring(startIndex);
        StringBuilder newName = new StringBuilder(100);
        VCardResultParser.maybeAppendComponent(components, 3, newName);
        VCardResultParser.maybeAppendComponent(components, 1, newName);
        VCardResultParser.maybeAppendComponent(components, 2, newName);
        VCardResultParser.maybeAppendComponent(components, 0, newName);
        VCardResultParser.maybeAppendComponent(components, 4, newName);
        name.Insert(0, newName.ToString().Trim());
      }
    }

    private static void maybeAppendComponent(string[] components, int i, StringBuilder newName)
    {
      if (string.IsNullOrEmpty(components[i]))
        return;
      if (newName.Length > 0)
        newName.Append(' ');
      newName.Append(components[i]);
    }
  }
}
