// Decompiled with JetBrains decompiler
// Type: WhatsApp.IqException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;

#nullable disable
namespace WhatsApp
{
  public class IqException : Exception
  {
    public int? Code;

    public IqException(string desc)
      : base(desc)
    {
    }

    public IqException(string function, int code)
      : base(function + " returned " + (object) code)
    {
      this.Code = new int?(code);
    }
  }
}
