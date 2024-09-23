// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteException
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class SqliteException : Exception
  {
    public SqliteException(string message, Exception inner = null, uint error = 1)
      : base(message, inner)
    {
      this.HResult = inner != null ? (int) inner.GetHResult() : (int) Sqlite.HRForError(error);
    }
  }
}
