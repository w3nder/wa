// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.WhitespaceReader
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class WhitespaceReader
  {
    internal static int ReadFwsReverse(string data, int index)
    {
      bool flag = false;
      for (; index >= 0; --index)
      {
        if ((int) data[index] == (int) MailBnfHelper.CR && flag)
        {
          flag = false;
        }
        else
        {
          if ((int) data[index] == (int) MailBnfHelper.CR || flag)
            throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
          if ((int) data[index] == (int) MailBnfHelper.LF)
            flag = true;
          else if ((int) data[index] != (int) MailBnfHelper.Space && (int) data[index] != (int) MailBnfHelper.Tab)
            break;
        }
      }
      if (flag)
        throw new FormatException(SR.GetString("MailAddressInvalidFormat"));
      return index;
    }

    internal static int ReadCfwsReverse(string data, int index)
    {
      int num1 = 0;
      for (index = WhitespaceReader.ReadFwsReverse(data, index); index >= 0; index = WhitespaceReader.ReadFwsReverse(data, index))
      {
        int num2 = QuotedPairReader.CountQuotedChars(data, index, true);
        if (num1 > 0 && num2 > 0)
          index -= num2;
        else if ((int) data[index] == (int) MailBnfHelper.EndComment)
        {
          ++num1;
          --index;
        }
        else if ((int) data[index] == (int) MailBnfHelper.StartComment)
        {
          --num1;
          if (num1 < 0)
            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) MailBnfHelper.StartComment));
          --index;
        }
        else if (num1 > 0 && ((int) data[index] > MailBnfHelper.Ascii7bitMaxValue || MailBnfHelper.Ctext[(int) data[index]]))
        {
          --index;
        }
        else
        {
          if (num1 > 0)
            throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
          break;
        }
      }
      if (num1 > 0)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) MailBnfHelper.EndComment));
      return index;
    }
  }
}
