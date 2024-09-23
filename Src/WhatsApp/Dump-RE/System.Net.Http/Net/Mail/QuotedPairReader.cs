// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.QuotedPairReader
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class QuotedPairReader
  {
    internal static int CountQuotedChars(string data, int index, bool permitUnicodeEscaping)
    {
      if (index <= 0 || (int) data[index - 1] != (int) MailBnfHelper.Backslash)
        return 0;
      int num = QuotedPairReader.CountBackslashes(data, index - 1);
      if (num % 2 == 0)
        return 0;
      if (!permitUnicodeEscaping && (int) data[index] > MailBnfHelper.Ascii7bitMaxValue)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
      return num + 1;
    }

    private static int CountBackslashes(string data, int index)
    {
      int num = 0;
      do
      {
        ++num;
        --index;
      }
      while (index >= 0 && (int) data[index] == (int) MailBnfHelper.Backslash);
      return num;
    }
  }
}
