// Decompiled with JetBrains decompiler
// Type: WhatsApp.UTF32
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;


namespace WhatsApp
{
  public class UTF32
  {
    public static IEnumerable<char> ToUtf16(IEnumerable<uint> chars)
    {
      foreach (uint ch in chars)
      {
        if (ch >= 55296U && ch < 57344U)
          throw new InvalidOperationException("Invalid char " + ch.ToString("x"));
        if (ch >= 65536U && ch < 1114112U)
        {
          uint num1 = 55296;
          uint b = 56320;
          uint num2 = ch - 65536U;
          b |= num2 & 1023U;
          uint num3 = num2 >> 10;
          yield return (char) (num1 | num3 & 1023U);
          yield return (char) b;
        }
        else
          yield return (char) ch;
      }
    }

    public static uint ToUtf32(char highSurrogate, char lowSurrogate)
    {
      return (uint) (((int) highSurrogate << 10) + (int) lowSurrogate - 56613888);
    }
  }
}
