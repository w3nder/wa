// Decompiled with JetBrains decompiler
// Type: System.Net.Mime.MailBnfHelper
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

#nullable disable
namespace System.Net.Mime
{
  internal static class MailBnfHelper
  {
    internal static bool[] Atext = new bool[128];
    internal static bool[] Qtext = new bool[128];
    internal static bool[] Dtext = new bool[128];
    internal static bool[] Ftext = new bool[128];
    internal static bool[] Ttext = new bool[128];
    internal static bool[] Ctext = new bool[128];
    internal static readonly int Ascii7bitMaxValue = (int) sbyte.MaxValue;
    internal static readonly char Quote = '"';
    internal static readonly char Space = ' ';
    internal static readonly char Tab = '\t';
    internal static readonly char CR = '\r';
    internal static readonly char LF = '\n';
    internal static readonly char StartComment = '(';
    internal static readonly char EndComment = ')';
    internal static readonly char Backslash = '\\';
    internal static readonly char At = '@';
    internal static readonly char EndAngleBracket = '>';
    internal static readonly char StartAngleBracket = '<';
    internal static readonly char StartSquareBracket = '[';
    internal static readonly char EndSquareBracket = ']';
    internal static readonly char Comma = ',';
    internal static readonly char Dot = '.';
    internal static readonly IList<char> Whitespace;
    private static string[] s_months = new string[13]
    {
      null,
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec"
    };

    static MailBnfHelper()
    {
      MailBnfHelper.Whitespace = (IList<char>) new List<char>();
      MailBnfHelper.Whitespace.Add(MailBnfHelper.Tab);
      MailBnfHelper.Whitespace.Add(MailBnfHelper.Space);
      MailBnfHelper.Whitespace.Add(MailBnfHelper.CR);
      MailBnfHelper.Whitespace.Add(MailBnfHelper.LF);
      for (int index = 48; index <= 57; ++index)
        MailBnfHelper.Atext[index] = true;
      for (int index = 65; index <= 90; ++index)
        MailBnfHelper.Atext[index] = true;
      for (int index = 97; index <= 122; ++index)
        MailBnfHelper.Atext[index] = true;
      MailBnfHelper.Atext[33] = true;
      MailBnfHelper.Atext[35] = true;
      MailBnfHelper.Atext[36] = true;
      MailBnfHelper.Atext[37] = true;
      MailBnfHelper.Atext[38] = true;
      MailBnfHelper.Atext[39] = true;
      MailBnfHelper.Atext[42] = true;
      MailBnfHelper.Atext[43] = true;
      MailBnfHelper.Atext[45] = true;
      MailBnfHelper.Atext[47] = true;
      MailBnfHelper.Atext[61] = true;
      MailBnfHelper.Atext[63] = true;
      MailBnfHelper.Atext[94] = true;
      MailBnfHelper.Atext[95] = true;
      MailBnfHelper.Atext[96] = true;
      MailBnfHelper.Atext[123] = true;
      MailBnfHelper.Atext[124] = true;
      MailBnfHelper.Atext[125] = true;
      MailBnfHelper.Atext[126] = true;
      for (int index = 1; index <= 9; ++index)
        MailBnfHelper.Qtext[index] = true;
      MailBnfHelper.Qtext[11] = true;
      MailBnfHelper.Qtext[12] = true;
      for (int index = 14; index <= 33; ++index)
        MailBnfHelper.Qtext[index] = true;
      for (int index = 35; index <= 91; ++index)
        MailBnfHelper.Qtext[index] = true;
      for (int index = 93; index <= (int) sbyte.MaxValue; ++index)
        MailBnfHelper.Qtext[index] = true;
      for (int index = 1; index <= 8; ++index)
        MailBnfHelper.Dtext[index] = true;
      MailBnfHelper.Dtext[11] = true;
      MailBnfHelper.Dtext[12] = true;
      for (int index = 14; index <= 31; ++index)
        MailBnfHelper.Dtext[index] = true;
      for (int index = 33; index <= 90; ++index)
        MailBnfHelper.Dtext[index] = true;
      for (int index = 94; index <= (int) sbyte.MaxValue; ++index)
        MailBnfHelper.Dtext[index] = true;
      for (int index = 33; index <= 57; ++index)
        MailBnfHelper.Ftext[index] = true;
      for (int index = 59; index <= 126; ++index)
        MailBnfHelper.Ftext[index] = true;
      for (int index = 33; index <= 126; ++index)
        MailBnfHelper.Ttext[index] = true;
      MailBnfHelper.Ttext[40] = false;
      MailBnfHelper.Ttext[41] = false;
      MailBnfHelper.Ttext[60] = false;
      MailBnfHelper.Ttext[62] = false;
      MailBnfHelper.Ttext[64] = false;
      MailBnfHelper.Ttext[44] = false;
      MailBnfHelper.Ttext[59] = false;
      MailBnfHelper.Ttext[58] = false;
      MailBnfHelper.Ttext[92] = false;
      MailBnfHelper.Ttext[34] = false;
      MailBnfHelper.Ttext[47] = false;
      MailBnfHelper.Ttext[91] = false;
      MailBnfHelper.Ttext[93] = false;
      MailBnfHelper.Ttext[63] = false;
      MailBnfHelper.Ttext[61] = false;
      for (int index = 1; index <= 8; ++index)
        MailBnfHelper.Ctext[index] = true;
      MailBnfHelper.Ctext[11] = true;
      MailBnfHelper.Ctext[12] = true;
      for (int index = 14; index <= 31; ++index)
        MailBnfHelper.Ctext[index] = true;
      for (int index = 33; index <= 39; ++index)
        MailBnfHelper.Ctext[index] = true;
      for (int index = 42; index <= 91; ++index)
        MailBnfHelper.Ctext[index] = true;
      for (int index = 93; index <= (int) sbyte.MaxValue; ++index)
        MailBnfHelper.Ctext[index] = true;
    }

    internal static bool SkipCFWS(string data, ref int offset)
    {
      int num = 0;
      while (offset < data.Length)
      {
        if (data[offset] > '\u007F')
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
        if (data[offset] == '\\' && num > 0)
          offset += 2;
        else if (data[offset] == '(')
          ++num;
        else if (data[offset] == ')')
          --num;
        else if (data[offset] != ' ' && data[offset] != '\t' && num == 0)
          return true;
        if (num < 0)
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
        ++offset;
      }
      return false;
    }

    internal static void ValidateHeaderName(string data)
    {
      int index;
      for (index = 0; index < data.Length; ++index)
      {
        if ((int) data[index] > MailBnfHelper.Ftext.Length || !MailBnfHelper.Ftext[(int) data[index]])
          throw new FormatException(SR.GetString("InvalidHeaderName"));
      }
      if (index == 0)
        throw new FormatException(SR.GetString("InvalidHeaderName"));
    }

    internal static string ReadQuotedString(string data, ref int offset, StringBuilder builder)
    {
      return MailBnfHelper.ReadQuotedString(data, ref offset, builder, false, false);
    }

    internal static string ReadQuotedString(
      string data,
      ref int offset,
      StringBuilder builder,
      bool doesntRequireQuotes,
      bool permitUnicodeInDisplayName)
    {
      if (!doesntRequireQuotes)
        ++offset;
      int startIndex = offset;
      StringBuilder stringBuilder = builder != null ? builder : new StringBuilder();
      while (offset < data.Length)
      {
        if (data[offset] == '\\')
        {
          stringBuilder.Append(data, startIndex, offset - startIndex);
          startIndex = ++offset;
        }
        else
        {
          if (data[offset] == '"')
          {
            stringBuilder.Append(data, startIndex, offset - startIndex);
            ++offset;
            return builder == null ? stringBuilder.ToString() : (string) null;
          }
          if (data[offset] == '=' && data.Length > offset + 3 && data[offset + 1] == '\r' && data[offset + 2] == '\n' && (data[offset + 3] == ' ' || data[offset + 3] == '\t'))
            offset += 3;
          else if (permitUnicodeInDisplayName)
          {
            if ((int) data[offset] <= MailBnfHelper.Ascii7bitMaxValue && !MailBnfHelper.Qtext[(int) data[offset]])
              throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
          }
          else if ((int) data[offset] > MailBnfHelper.Ascii7bitMaxValue || !MailBnfHelper.Qtext[(int) data[offset]])
            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
        }
        ++offset;
      }
      if (!doesntRequireQuotes)
        throw new FormatException(SR.GetString("MailHeaderFieldMalformedHeader"));
      stringBuilder.Append(data, startIndex, offset - startIndex);
      return builder == null ? stringBuilder.ToString() : (string) null;
    }

    internal static string ReadParameterAttribute(
      string data,
      ref int offset,
      StringBuilder builder)
    {
      return !MailBnfHelper.SkipCFWS(data, ref offset) ? (string) null : MailBnfHelper.ReadToken(data, ref offset, (StringBuilder) null);
    }

    internal static string ReadToken(string data, ref int offset, StringBuilder builder)
    {
      int startIndex = offset;
      while (offset < data.Length)
      {
        if ((int) data[offset] > MailBnfHelper.Ascii7bitMaxValue)
          throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
        if (MailBnfHelper.Ttext[(int) data[offset]])
          ++offset;
        else
          break;
      }
      if (startIndex == offset)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[offset]));
      return data.Substring(startIndex, offset - startIndex);
    }

    internal static string GetDateTimeString(DateTime value, StringBuilder builder)
    {
      StringBuilder stringBuilder = builder != null ? builder : new StringBuilder();
      stringBuilder.Append(value.Day);
      stringBuilder.Append(' ');
      stringBuilder.Append(MailBnfHelper.s_months[value.Month]);
      stringBuilder.Append(' ');
      stringBuilder.Append(value.Year);
      stringBuilder.Append(' ');
      if (value.Hour <= 9)
        stringBuilder.Append('0');
      stringBuilder.Append(value.Hour);
      stringBuilder.Append(':');
      if (value.Minute <= 9)
        stringBuilder.Append('0');
      stringBuilder.Append(value.Minute);
      stringBuilder.Append(':');
      if (value.Second <= 9)
        stringBuilder.Append('0');
      stringBuilder.Append(value.Second);
      string str = TimeZone.CurrentTimeZone.GetUtcOffset((object) value).ToString();
      if (str[0] != '-')
        stringBuilder.Append(" +");
      else
        stringBuilder.Append(" ");
      string[] strArray = str.Split(':');
      stringBuilder.Append(strArray[0]);
      stringBuilder.Append(strArray[1]);
      return builder == null ? stringBuilder.ToString() : (string) null;
    }

    internal static void GetTokenOrQuotedString(
      string data,
      StringBuilder builder,
      bool allowUnicode)
    {
      int index = 0;
      int startIndex = 0;
      for (; index < data.Length; ++index)
      {
        if (!MailBnfHelper.CheckForUnicode(data[index], allowUnicode) && (!MailBnfHelper.Ttext[(int) data[index]] || data[index] == ' '))
        {
          builder.Append('"');
          for (; index < data.Length; ++index)
          {
            if (!MailBnfHelper.CheckForUnicode(data[index], allowUnicode))
            {
              if (MailBnfHelper.IsFWSAt(data, index))
                index = index + 1 + 1;
              else if (!MailBnfHelper.Qtext[(int) data[index]])
              {
                builder.Append(data, startIndex, index - startIndex);
                builder.Append('\\');
                startIndex = index;
              }
            }
          }
          builder.Append(data, startIndex, index - startIndex);
          builder.Append('"');
          return;
        }
      }
      if (data.Length == 0)
        builder.Append("\"\"");
      builder.Append(data);
    }

    private static bool CheckForUnicode(char ch, bool allowUnicode)
    {
      if ((int) ch < MailBnfHelper.Ascii7bitMaxValue)
        return false;
      if (!allowUnicode)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) ch));
      return true;
    }

    internal static bool HasCROrLF(string data)
    {
      for (int index = 0; index < data.Length; ++index)
      {
        if (data[index] == '\r' || data[index] == '\n')
          return true;
      }
      return false;
    }

    internal static bool IsFWSAt(string data, int index)
    {
      if ((int) data[index] != (int) MailBnfHelper.CR || index + 2 >= data.Length || (int) data[index + 1] != (int) MailBnfHelper.LF)
        return false;
      return (int) data[index + 2] == (int) MailBnfHelper.Space || (int) data[index + 2] == (int) MailBnfHelper.Tab;
    }
  }
}
