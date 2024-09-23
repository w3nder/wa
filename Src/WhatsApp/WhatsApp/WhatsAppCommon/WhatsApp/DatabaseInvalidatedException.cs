// Decompiled with JetBrains decompiler
// Type: WhatsApp.DatabaseInvalidatedException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class DatabaseInvalidatedException : Exception
  {
    public DatabaseInvalidatedException(string message)
      : base(message)
    {
    }
  }
}
