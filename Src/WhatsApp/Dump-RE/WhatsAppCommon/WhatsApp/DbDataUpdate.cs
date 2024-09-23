// Decompiled with JetBrains decompiler
// Type: WhatsApp.DbDataUpdate
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class DbDataUpdate
  {
    public DbDataUpdate.Types UpdateType { get; private set; }

    public object UpdatedObj { get; private set; }

    public string[] ModifiedColumns { get; private set; }

    public DbDataUpdate(object updatedObj, DbDataUpdate.Types updateType, string[] modifiedColumns = null)
    {
      this.UpdateType = updateType;
      this.UpdatedObj = updatedObj;
      this.ModifiedColumns = updateType == DbDataUpdate.Types.Modified ? modifiedColumns : (string[]) null;
    }

    public DbDataUpdate(object updatedObj, string[] modifiedColumns)
    {
      this.UpdateType = DbDataUpdate.Types.Modified;
      this.UpdatedObj = updatedObj;
      this.ModifiedColumns = modifiedColumns;
    }

    public enum Types
    {
      None,
      Added,
      Deleted,
      Modified,
    }
  }
}
