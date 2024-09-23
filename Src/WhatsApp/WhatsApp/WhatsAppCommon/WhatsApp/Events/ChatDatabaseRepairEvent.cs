// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ChatDatabaseRepairEvent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ChatDatabaseRepairEvent : WamEvent
  {
    public bool? databaseRepairOverallResult;
    public bool? databaseRepairSqliteIntegrityCheckResult;
    public bool? repairHasOnlyIndexErrors;
    public bool? databaseRepairReindexingResult;
    public bool? databaseRepairDumpAndRestoreResult;
    public long? databaseRepairDumpAndRestoreRecoveryPercentage;
    public bool? databaseRepairDumpAndRestoreInterrupted;

    public void Reset()
    {
      this.databaseRepairOverallResult = new bool?();
      this.databaseRepairSqliteIntegrityCheckResult = new bool?();
      this.repairHasOnlyIndexErrors = new bool?();
      this.databaseRepairReindexingResult = new bool?();
      this.databaseRepairDumpAndRestoreResult = new bool?();
      this.databaseRepairDumpAndRestoreRecoveryPercentage = new long?();
      this.databaseRepairDumpAndRestoreInterrupted = new bool?();
    }

    public override uint GetCode() => 1024;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.databaseRepairOverallResult);
      Wam.MaybeSerializeField(2, this.databaseRepairSqliteIntegrityCheckResult);
      Wam.MaybeSerializeField(3, this.repairHasOnlyIndexErrors);
      Wam.MaybeSerializeField(4, this.databaseRepairReindexingResult);
      Wam.MaybeSerializeField(5, this.databaseRepairDumpAndRestoreResult);
      Wam.MaybeSerializeField(6, this.databaseRepairDumpAndRestoreRecoveryPercentage);
      Wam.MaybeSerializeField(7, this.databaseRepairDumpAndRestoreInterrupted);
    }
  }
}
