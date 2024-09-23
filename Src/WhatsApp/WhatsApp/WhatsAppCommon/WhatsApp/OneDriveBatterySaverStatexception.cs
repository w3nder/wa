// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveBatterySaverStatexception
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class OneDriveBatterySaverStatexception : Exception
  {
    public OneDriveBatterySaverStatexception()
      : this(string.Empty, (Exception) null)
    {
    }

    public OneDriveBatterySaverStatexception(string message)
      : this(message, (Exception) null)
    {
    }

    public OneDriveBatterySaverStatexception(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
