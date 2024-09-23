// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.DomainLiteralReader
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class DomainLiteralReader
  {
    internal static int ReadReverse(string data, int index)
    {
      --index;
      do
      {
        index = WhitespaceReader.ReadFwsReverse(data, index);
        if (index >= 0)
        {
          int num = QuotedPairReader.CountQuotedChars(data, index, false);
          if (num > 0)
          {
            index -= num;
          }
          else
          {
            if ((int) data[index] == (int) MailBnfHelper.StartSquareBracket)
              return index - 1;
            if ((int) data[index] > MailBnfHelper.Ascii7bitMaxValue || !MailBnfHelper.Dtext[(int) data[index]])
              throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
            --index;
          }
        }
        else
          break;
      }
      while (index >= 0);
      throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) MailBnfHelper.EndSquareBracket));
    }
  }
}
