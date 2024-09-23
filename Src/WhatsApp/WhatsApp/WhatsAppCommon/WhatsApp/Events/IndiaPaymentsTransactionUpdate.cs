// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsTransactionUpdate
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class IndiaPaymentsTransactionUpdate : WamEvent
  {
    public string paymentsEventId;
    public wam_enum_india_payments_psp_id_type? paymentsPspId;
    public string paymentsErrorCode;
    public string paymentsErrorText;
    public string paymentsBankId;
    public long? paymentsResponseByteSize;
    public long? paymentsResponseRtt;
    public string paymentsSeqNum;
    public wam_enum_india_payments_transaction_status_type? transactionStatusNew;
    public wam_enum_india_payments_sender_or_receiver_type? transactionSenderOrReceiver;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsPspId = new wam_enum_india_payments_psp_id_type?();
      this.paymentsErrorCode = (string) null;
      this.paymentsErrorText = (string) null;
      this.paymentsBankId = (string) null;
      this.paymentsResponseByteSize = new long?();
      this.paymentsResponseRtt = new long?();
      this.paymentsSeqNum = (string) null;
      this.transactionStatusNew = new wam_enum_india_payments_transaction_status_type?();
      this.transactionSenderOrReceiver = new wam_enum_india_payments_sender_or_receiver_type?();
    }

    public override uint GetCode() => 1566;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_india_payments_psp_id_type>(this.paymentsPspId));
      Wam.MaybeSerializeField(3, this.paymentsErrorCode);
      Wam.MaybeSerializeField(4, this.paymentsErrorText);
      Wam.MaybeSerializeField(5, this.paymentsBankId);
      Wam.MaybeSerializeField(6, this.paymentsResponseByteSize);
      Wam.MaybeSerializeField(7, this.paymentsResponseRtt);
      Wam.MaybeSerializeField(10, this.paymentsSeqNum);
      Wam.MaybeSerializeField(8, Wam.EnumToLong<wam_enum_india_payments_transaction_status_type>(this.transactionStatusNew));
      Wam.MaybeSerializeField(9, Wam.EnumToLong<wam_enum_india_payments_sender_or_receiver_type>(this.transactionSenderOrReceiver));
    }
  }
}
