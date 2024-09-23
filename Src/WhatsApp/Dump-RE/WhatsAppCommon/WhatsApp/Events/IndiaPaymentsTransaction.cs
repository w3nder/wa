// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsTransaction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class IndiaPaymentsTransaction : WamEvent
  {
    public string paymentsEventId;
    public wam_enum_india_payments_psp_id_type? paymentsPspId;
    public string paymentsBankId;
    public long? paymentsResponseRtt;
    public string paymentsSeqNum;
    public long? transactionAmount1000;
    public bool? transactionSentInGroup;
    public bool? transactionIsNonWaReceiver;
    public bool? transactionIsNodalReceiver;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsPspId = new wam_enum_india_payments_psp_id_type?();
      this.paymentsBankId = (string) null;
      this.paymentsResponseRtt = new long?();
      this.paymentsSeqNum = (string) null;
      this.transactionAmount1000 = new long?();
      this.transactionSentInGroup = new bool?();
      this.transactionIsNonWaReceiver = new bool?();
      this.transactionIsNodalReceiver = new bool?();
    }

    public override uint GetCode() => 1564;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_india_payments_psp_id_type>(this.paymentsPspId));
      Wam.MaybeSerializeField(5, this.paymentsBankId);
      Wam.MaybeSerializeField(7, this.paymentsResponseRtt);
      Wam.MaybeSerializeField(19, this.paymentsSeqNum);
      Wam.MaybeSerializeField(15, this.transactionAmount1000);
      Wam.MaybeSerializeField(16, this.transactionSentInGroup);
      Wam.MaybeSerializeField(17, this.transactionIsNonWaReceiver);
      Wam.MaybeSerializeField(18, this.transactionIsNodalReceiver);
    }
  }
}
