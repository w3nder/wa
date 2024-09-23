// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.QuotedStringFormatReader
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class QuotedStringFormatReader
  {
    internal static int ReadReverseQuoted(string data, int index, bool permitUnicode)
    {
      --index;
      do
      {
        index = WhitespaceReader.ReadFwsReverse(data, index);
        if (index >= 0)
        {
          int num = QuotedPairReader.CountQuotedChars(data, index, permitUnicode);
          if (num > 0)
          {
            index -= num;
          }
          else
          {
            if ((int) data[index] == (int) MailBnfHelper.Quote)
              return index - 1;
            if (!QuotedStringFormatReader.IsValidQtext(permitUnicode, data[index]))
              throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
            --index;
          }
        }
        else
          break;
      }
      while (index >= 0);
      throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) MailBnfHelper.Quote));
    }

    internal static int ReadReverseUnQuoted(
      string data,
      int index,
      bool permitUnicode,
      bool expectCommaDelimiter)
    {
      do
      {
        index = WhitespaceReader.ReadFwsReverse(data, index);
        if (index >= 0)
        {
          int num = QuotedPairReader.CountQuotedChars(data, index, permitUnicode);
          if (num > 0)
            index -= num;
          else if (!expectCommaDelimiter || (int) data[index] != (int) MailBnfHelper.Comma)
          {
            if (!QuotedStringFormatReader.IsValidQtext(permitUnicode, data[index]))
              throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
            --index;
          }
          else
            break;
        }
        else
          break;
      }
      while (index >= 0);
      return index;
    }

    private static bool IsValidQtext(bool allowUnicode, char ch)
    {
      return (int) ch > MailBnfHelper.Ascii7bitMaxValue ? allowUnicode : MailBnfHelper.Qtext[(int) ch];
    }
  }
}
