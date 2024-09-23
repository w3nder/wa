// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallRecordUpdate
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp
{
  public class CallRecordUpdate : DbDataUpdate
  {
    public CallRecord UpdatedCallRecord => this.UpdatedObj as CallRecord;

    public CallRecordUpdate(CallRecord updatedCallRecord, DbDataUpdate.Types updateType)
      : base((object) updatedCallRecord, updateType)
    {
    }
  }
}
