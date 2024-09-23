// Decompiled with JetBrains decompiler
// Type: System.Net.Mail.DotAtomReader
// Assembly: System.Net.Http, Version=1.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1F068741-35F1-4E4D-A7D5-7D9AD60BF90D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\System.Net.Http.xml

using System.Net.Mime;

#nullable disable
namespace System.Net.Mail
{
  internal static class DotAtomReader
  {
    internal static int ReadReverse(string data, int index)
    {
      int num = index;
      while (0 <= index && ((int) data[index] > MailBnfHelper.Ascii7bitMaxValue || (int) data[index] == (int) MailBnfHelper.Dot || MailBnfHelper.Atext[(int) data[index]]))
        --index;
      if (num == index)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) data[index]));
      if ((int) data[index + 1] == (int) MailBnfHelper.Dot)
        throw new FormatException(SR.GetString("MailHeaderFieldInvalidCharacter", (object) MailBnfHelper.Dot));
      return index;
    }
  }
}
