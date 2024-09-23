// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ReceiptsSend
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ReceiptsSend : WamEvent
  {
    public wam_enum_receipts_type? receiptsType;
    public wam_enum_message_type? messageType;
    public long? receiptsProcT;

    public void Reset()
    {
      this.receiptsType = new wam_enum_receipts_type?();
      this.messageType = new wam_enum_message_type?();
      this.receiptsProcT = new long?();
    }

    public override uint GetCode() => 490;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_receipts_type>(this.receiptsType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(3, this.receiptsProcT);
    }
  }
}
