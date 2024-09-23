// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.ResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> <p>Abstract class representing the result of decoding a barcode, as more than
  /// a String -- as some type of structured data. This might be a subclass which represents
  /// a URL, or an e-mail address. {@link #parseResult(com.google.zxing.Result)} will turn a raw
  /// decoded string into the most appropriate type of structured representation.</p>
  /// 
  /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
  /// on exception-based mechanisms during parsing.</p>
  /// </summary>
  /// <author>Sean Owen</author>
  public abstract class ResultParser
  {
    private static readonly ResultParser[] PARSERS = new ResultParser[20]
    {
      (ResultParser) new BookmarkDoCoMoResultParser(),
      (ResultParser) new AddressBookDoCoMoResultParser(),
      (ResultParser) new EmailDoCoMoResultParser(),
      (ResultParser) new AddressBookAUResultParser(),
      (ResultParser) new VCardResultParser(),
      (ResultParser) new BizcardResultParser(),
      (ResultParser) new VEventResultParser(),
      (ResultParser) new EmailAddressResultParser(),
      (ResultParser) new SMTPResultParser(),
      (ResultParser) new TelResultParser(),
      (ResultParser) new SMSMMSResultParser(),
      (ResultParser) new SMSTOMMSTOResultParser(),
      (ResultParser) new GeoResultParser(),
      (ResultParser) new WifiResultParser(),
      (ResultParser) new URLTOResultParser(),
      (ResultParser) new URIResultParser(),
      (ResultParser) new ISBNResultParser(),
      (ResultParser) new ProductResultParser(),
      (ResultParser) new ExpandedProductResultParser(),
      (ResultParser) new VINResultParser()
    };
    private static readonly Regex DIGITS = new Regex("\\A(?:\\d+)\\z", RegexOptions.Compiled);
    private static readonly Regex AMPERSAND = new Regex("&", RegexOptions.Compiled);
    private static readonly Regex EQUALS = new Regex("=", RegexOptions.Compiled);

    /// <summary>
    /// Attempts to parse the raw {@link Result}'s contents as a particular type
    /// of information (email, URL, etc.) and return a {@link ParsedResult} encapsulating
    /// the result of parsing.
    /// </summary>
    /// <param name="theResult">The result.</param>
    /// <returns></returns>
    public abstract ParsedResult parse(ZXing.Result theResult);

    public static ParsedResult parseResult(ZXing.Result theResult)
    {
      foreach (ResultParser resultParser in ResultParser.PARSERS)
      {
        ParsedResult result = resultParser.parse(theResult);
        if (result != null)
          return result;
      }
      return (ParsedResult) new TextParsedResult(theResult.Text, (string) null);
    }

    protected static void maybeAppend(string value, StringBuilder result)
    {
      if (value == null)
        return;
      result.Append('\n');
      result.Append(value);
    }

    protected static void maybeAppend(string[] value, StringBuilder result)
    {
      if (value == null)
        return;
      for (int index = 0; index < value.Length; ++index)
      {
        result.Append('\n');
        result.Append(value[index]);
      }
    }

    protected static string[] maybeWrap(string value_Renamed)
    {
      if (value_Renamed == null)
        return (string[]) null;
      return new string[1]{ value_Renamed };
    }

    protected static string unescapeBackslash(string escaped)
    {
      if (escaped != null)
      {
        int charCount = escaped.IndexOf('\\');
        if (charCount >= 0)
        {
          int length = escaped.Length;
          StringBuilder stringBuilder = new StringBuilder(length - 1);
          stringBuilder.Append(escaped.ToCharArray(), 0, charCount);
          bool flag = false;
          for (int index = charCount; index < length; ++index)
          {
            char ch = escaped[index];
            if (flag || ch != '\\')
            {
              stringBuilder.Append(ch);
              flag = false;
            }
            else
              flag = true;
          }
          return stringBuilder.ToString();
        }
      }
      return escaped;
    }

    protected static int parseHexDigit(char c)
    {
      switch (c)
      {
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          return (int) c - 48;
        case 'A':
        case 'B':
        case 'C':
        case 'D':
        case 'E':
        case 'F':
          return 10 + ((int) c - 65);
        case 'a':
        case 'b':
        case 'c':
        case 'd':
        case 'e':
        case 'f':
          return 10 + ((int) c - 97);
        default:
          return -1;
      }
    }

    internal static bool isStringOfDigits(string value, int length)
    {
      return value != null && length > 0 && length == value.Length && ResultParser.DIGITS.Match(value).Success;
    }

    internal static bool isSubstringOfDigits(string value, int offset, int length)
    {
      if (value == null || length <= 0)
        return false;
      int num = offset + length;
      return value.Length >= num && ResultParser.DIGITS.Match(value, offset, length).Success;
    }

    internal static IDictionary<string, string> parseNameValuePairs(string uri)
    {
      int num = uri.IndexOf('?');
      if (num < 0)
        return (IDictionary<string, string>) null;
      Dictionary<string, string> result = new Dictionary<string, string>(3);
      foreach (string keyValue in ResultParser.AMPERSAND.Split(uri.Substring(num + 1)))
        ResultParser.appendKeyValue(keyValue, (IDictionary<string, string>) result);
      return (IDictionary<string, string>) result;
    }

    private static void appendKeyValue(string keyValue, IDictionary<string, string> result)
    {
      string[] strArray = ResultParser.EQUALS.Split(keyValue, 2);
      if (strArray.Length != 2)
        return;
      string key = strArray[0];
      string escaped = strArray[1];
      string str;
      try
      {
        str = ResultParser.urlDecode(escaped);
        result[key] = str;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("url decoding failed", ex);
      }
      result[key] = str;
    }

    internal static string[] matchPrefixedField(
      string prefix,
      string rawText,
      char endChar,
      bool trim)
    {
      IList<string> stringList = (IList<string>) null;
      int startIndex1 = 0;
      int length = rawText.Length;
      while (startIndex1 < length)
      {
        int num1 = rawText.IndexOf(prefix, startIndex1);
        if (num1 >= 0)
        {
          startIndex1 = num1 + prefix.Length;
          int startIndex2 = startIndex1;
          bool flag = false;
          while (!flag)
          {
            int num2 = rawText.IndexOf(endChar, startIndex1);
            if (num2 < 0)
            {
              startIndex1 = rawText.Length;
              flag = true;
            }
            else if (rawText[num2 - 1] == '\\')
            {
              startIndex1 = num2 + 1;
            }
            else
            {
              if (stringList == null)
                stringList = (IList<string>) new List<string>();
              string str = ResultParser.unescapeBackslash(rawText.Substring(startIndex2, num2 - startIndex2));
              if (trim)
                str = str.Trim();
              if (!string.IsNullOrEmpty(str))
                stringList.Add(str);
              startIndex1 = num2 + 1;
              flag = true;
            }
          }
        }
        else
          break;
      }
      return stringList == null || stringList.Count == 0 ? (string[]) null : SupportClass.toStringArray((ICollection<string>) stringList);
    }

    internal static string matchSinglePrefixedField(
      string prefix,
      string rawText,
      char endChar,
      bool trim)
    {
      return ResultParser.matchPrefixedField(prefix, rawText, endChar, trim)?[0];
    }

    protected static string urlDecode(string escaped)
    {
      if (escaped == null)
        return (string) null;
      char[] charArray = escaped.ToCharArray();
      int firstEscape = ResultParser.findFirstEscape(charArray);
      if (firstEscape < 0)
        return escaped;
      int length = charArray.Length;
      StringBuilder stringBuilder = new StringBuilder(length - 2);
      stringBuilder.Append(charArray, 0, firstEscape);
      for (int index = firstEscape; index < length; ++index)
      {
        char ch = charArray[index];
        switch (ch)
        {
          case '%':
            if (index >= length - 2)
            {
              stringBuilder.Append('%');
              break;
            }
            int num;
            int hexDigit1 = ResultParser.parseHexDigit(charArray[num = index + 1]);
            int hexDigit2 = ResultParser.parseHexDigit(charArray[index = num + 1]);
            if (hexDigit1 < 0 || hexDigit2 < 0)
            {
              stringBuilder.Append('%');
              stringBuilder.Append(charArray[index - 1]);
              stringBuilder.Append(charArray[index]);
            }
            stringBuilder.Append((char) ((hexDigit1 << 4) + hexDigit2));
            break;
          case '+':
            stringBuilder.Append(' ');
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private static int findFirstEscape(char[] escapedArray)
    {
      int length = escapedArray.Length;
      for (int firstEscape = 0; firstEscape < length; ++firstEscape)
      {
        switch (escapedArray[firstEscape])
        {
          case '%':
          case '+':
            return firstEscape;
          default:
            continue;
        }
      }
      return -1;
    }
  }
}
