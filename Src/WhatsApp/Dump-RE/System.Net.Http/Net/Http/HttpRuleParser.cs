// Decompiled with JetBrains decompiler
// Type: System.Net.Http.HttpRuleParser
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;

#nullable disable
namespace System.Net.Http
{
  internal static class HttpRuleParser
  {
    private const int maxNestedCount = 5;
    internal const char CR = '\r';
    internal const char LF = '\n';
    internal const int MaxInt64Digits = 19;
    internal const int MaxInt32Digits = 10;
    private static readonly bool[] tokenChars;
    private static readonly string[] dateFormats = new string[15]
    {
      "ddd, d MMM yyyy H:m:s 'GMT'",
      "ddd, d MMM yyyy H:m:s",
      "d MMM yyyy H:m:s 'GMT'",
      "d MMM yyyy H:m:s",
      "ddd, d MMM yy H:m:s 'GMT'",
      "ddd, d MMM yy H:m:s",
      "d MMM yy H:m:s 'GMT'",
      "d MMM yy H:m:s",
      "dddd, d'-'MMM'-'yy H:m:s 'GMT'",
      "dddd, d'-'MMM'-'yy H:m:s",
      "ddd MMM d H:m:s yyyy",
      "ddd, d MMM yyyy H:m:s zzz",
      "ddd, d MMM yyyy H:m:s",
      "d MMM yyyy H:m:s zzz",
      "d MMM yyyy H:m:s"
    };
    internal static readonly Encoding DefaultHttpEncoding = Latin1Encoding.GetEncoding();

    static HttpRuleParser()
    {
      HttpRuleParser.tokenChars = new bool[128];
      for (int index = 33; index < (int) sbyte.MaxValue; ++index)
        HttpRuleParser.tokenChars[index] = true;
      HttpRuleParser.tokenChars[40] = false;
      HttpRuleParser.tokenChars[41] = false;
      HttpRuleParser.tokenChars[60] = false;
      HttpRuleParser.tokenChars[62] = false;
      HttpRuleParser.tokenChars[64] = false;
      HttpRuleParser.tokenChars[44] = false;
      HttpRuleParser.tokenChars[59] = false;
      HttpRuleParser.tokenChars[58] = false;
      HttpRuleParser.tokenChars[92] = false;
      HttpRuleParser.tokenChars[34] = false;
      HttpRuleParser.tokenChars[47] = false;
      HttpRuleParser.tokenChars[91] = false;
      HttpRuleParser.tokenChars[93] = false;
      HttpRuleParser.tokenChars[63] = false;
      HttpRuleParser.tokenChars[61] = false;
      HttpRuleParser.tokenChars[123] = false;
      HttpRuleParser.tokenChars[125] = false;
    }

    internal static bool IsTokenChar(char character)
    {
      return character <= '\u007F' && HttpRuleParser.tokenChars[(int) character];
    }

    [Pure]
    internal static int GetTokenLength(string input, int startIndex)
    {
      Contract.Requires(input != null);
      Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= input.Length - startIndex);
      if (startIndex >= input.Length)
        return 0;
      for (int index = startIndex; index < input.Length; ++index)
      {
        if (!HttpRuleParser.IsTokenChar(input[index]))
          return index - startIndex;
      }
      return input.Length - startIndex;
    }

    internal static int GetWhitespaceLength(string input, int startIndex)
    {
      Contract.Requires(input != null);
      Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= input.Length - startIndex);
      if (startIndex >= input.Length)
        return 0;
      int index = startIndex;
      while (index < input.Length)
      {
        switch (input[index])
        {
          case '\t':
          case ' ':
            ++index;
            continue;
          case '\r':
            if (index + 2 < input.Length && input[index + 1] == '\n')
            {
              switch (input[index + 2])
              {
                case '\t':
                case ' ':
                  index += 3;
                  continue;
              }
            }
            else
              break;
            break;
        }
        return index - startIndex;
      }
      return input.Length - startIndex;
    }

    internal static bool ContainsInvalidNewLine(string value)
    {
      return HttpRuleParser.ContainsInvalidNewLine(value, 0);
    }

    internal static bool ContainsInvalidNewLine(string value, int startIndex)
    {
      for (int index1 = startIndex; index1 < value.Length; ++index1)
      {
        if (value[index1] == '\r')
        {
          int index2 = index1 + 1;
          if (index2 < value.Length && value[index2] == '\n')
          {
            index1 = index2 + 1;
            if (index1 == value.Length)
              return true;
            switch (value[index1])
            {
              case '\t':
              case ' ':
                continue;
              default:
                return true;
            }
          }
        }
      }
      return false;
    }

    internal static int GetNumberLength(string input, int startIndex, bool allowDecimal)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0 && startIndex < input.Length);
      Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= input.Length - startIndex);
      int index = startIndex;
      bool flag = !allowDecimal;
      if (input[index] == '.')
        return 0;
      while (index < input.Length)
      {
        char ch = input[index];
        switch (ch)
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
            ++index;
            continue;
          default:
            if (!flag && ch == '.')
            {
              flag = true;
              ++index;
              continue;
            }
            goto label_7;
        }
      }
label_7:
      return index - startIndex;
    }

    internal static int GetHostLength(
      string input,
      int startIndex,
      bool allowToken,
      out string host)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0);
      Contract.Ensures(Contract.Result<int>() >= 0 && Contract.Result<int>() <= input.Length - startIndex);
      host = (string) null;
      if (startIndex >= input.Length)
        return 0;
      int index = startIndex;
      bool flag = true;
      for (; index < input.Length; ++index)
      {
        char character = input[index];
        switch (character)
        {
          case '\t':
          case '\r':
          case ' ':
          case ',':
            goto label_7;
          case '/':
            return 0;
          default:
            flag = flag && HttpRuleParser.IsTokenChar(character);
            continue;
        }
      }
label_7:
      int length = index - startIndex;
      if (length == 0)
        return 0;
      string host1 = input.Substring(startIndex, length);
      if ((!allowToken || !flag) && !HttpRuleParser.IsValidHostName(host1))
        return 0;
      host = host1;
      return length;
    }

    internal static HttpParseResult GetCommentLength(string input, int startIndex, out int length)
    {
      int nestedCount = 0;
      return HttpRuleParser.GetExpressionLength(input, startIndex, '(', ')', true, ref nestedCount, out length);
    }

    internal static HttpParseResult GetQuotedStringLength(
      string input,
      int startIndex,
      out int length)
    {
      int nestedCount = 0;
      return HttpRuleParser.GetExpressionLength(input, startIndex, '"', '"', false, ref nestedCount, out length);
    }

    internal static HttpParseResult GetQuotedPairLength(
      string input,
      int startIndex,
      out int length)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0 && startIndex < input.Length);
      Contract.Ensures(Contract.ValueAtReturn<int>(out length) >= 0 && Contract.ValueAtReturn<int>(out length) <= input.Length - startIndex);
      length = 0;
      if (input[startIndex] != '\\')
        return HttpParseResult.NotParsed;
      if (startIndex + 2 > input.Length || input[startIndex + 1] > '\u007F')
        return HttpParseResult.InvalidFormat;
      length = 2;
      return HttpParseResult.Parsed;
    }

    internal static string DateToString(DateTimeOffset dateTime)
    {
      return dateTime.ToUniversalTime().ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    internal static bool TryStringToDate(string input, out DateTimeOffset result)
    {
      return DateTimeOffset.TryParseExact(input, HttpRuleParser.dateFormats, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result);
    }

    private static HttpParseResult GetExpressionLength(
      string input,
      int startIndex,
      char openChar,
      char closeChar,
      bool supportsNesting,
      ref int nestedCount,
      out int length)
    {
      Contract.Requires(input != null);
      Contract.Requires(startIndex >= 0 && startIndex < input.Length);
      Contract.Ensures(Contract.Result<HttpParseResult>() != HttpParseResult.Parsed || Contract.ValueAtReturn<int>(out length) > 0);
      length = 0;
      if ((int) input[startIndex] != (int) openChar)
        return HttpParseResult.NotParsed;
      int num = startIndex + 1;
      while (num < input.Length)
      {
        int length1 = 0;
        if (num + 2 < input.Length && HttpRuleParser.GetQuotedPairLength(input, num, out length1) == HttpParseResult.Parsed)
        {
          num += length1;
        }
        else
        {
          if (supportsNesting && (int) input[num] == (int) openChar)
          {
            ++nestedCount;
            try
            {
              if (nestedCount > 5)
                return HttpParseResult.InvalidFormat;
              int length2 = 0;
              HttpParseResult expressionLength = HttpRuleParser.GetExpressionLength(input, num, openChar, closeChar, supportsNesting, ref nestedCount, out length2);
              switch (expressionLength)
              {
                case HttpParseResult.Parsed:
                  num += length2;
                  break;
                case HttpParseResult.NotParsed:
                  Contract.Assert(false, "'NotParsed' is unexpected: We started nested expression parsing, because we found the open-char. So either it's a valid nested expression or it has invalid format.");
                  break;
                case HttpParseResult.InvalidFormat:
                  return HttpParseResult.InvalidFormat;
                default:
                  Contract.Assert(false, "Unknown enum result: " + (object) expressionLength);
                  break;
              }
            }
            finally
            {
              --nestedCount;
            }
          }
          if ((int) input[num] == (int) closeChar)
          {
            length = num - startIndex + 1;
            return HttpParseResult.Parsed;
          }
          ++num;
        }
      }
      return HttpParseResult.InvalidFormat;
    }

    private static bool IsValidHostName(string host)
    {
      return Uri.TryCreate("http://u@" + host + "/", UriKind.Absolute, out Uri _);
    }
  }
}
