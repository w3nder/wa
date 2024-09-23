// Decompiled with JetBrains decompiler
// Type: WhatsApp.CachedStatement
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;


namespace WhatsApp
{
  public class CachedStatement
  {
    private Sqlite.PreparedStatement stmt;
    private string sql;

    public CachedStatement(string sql) => this.sql = sql;

    internal Sqlite.PreparedStatement Prepare(SqliteDataContext context, Sqlite db)
    {
      if (this.stmt == null)
      {
        this.stmt = db.PrepareStatement(this.sql, (Action) (() => this.stmt = (Sqlite.PreparedStatement) null));
        this.stmt.Attach(context);
      }
      else
        this.stmt.Reset();
      return this.stmt;
    }
  }
}
