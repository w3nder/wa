// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.AddressbookSync
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class AddressbookSync : WamEvent
  {
    public bool? addressbookSyncIsReg;
    public long? addressbookSyncErrorCode;
    public wam_enum_addressbook_sync_result_type? addressbookSyncResultType;
    public long? addressbookSyncT;

    public void Reset()
    {
      this.addressbookSyncIsReg = new bool?();
      this.addressbookSyncErrorCode = new long?();
      this.addressbookSyncResultType = new wam_enum_addressbook_sync_result_type?();
      this.addressbookSyncT = new long?();
    }

    public override uint GetCode() => 480;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.addressbookSyncIsReg);
      Wam.MaybeSerializeField(2, this.addressbookSyncErrorCode);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_addressbook_sync_result_type>(this.addressbookSyncResultType));
      Wam.MaybeSerializeField(4, this.addressbookSyncT);
    }
  }
}
