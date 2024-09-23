// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsDbSmsSent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class IndiaPaymentsDbSmsSent : WamEvent
  {
    public string paymentsEventId;
    public wam_enum_india_payments_psp_id_type? paymentsPspId;
    public string paymentsBankId;
    public bool? paymentsSmsSuccessfullySent;
    public bool? paymentsUserCancelledSms;
    public bool? paymentsSmsSendingFailed;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsPspId = new wam_enum_india_payments_psp_id_type?();
      this.paymentsBankId = (string) null;
      this.paymentsSmsSuccessfullySent = new bool?();
      this.paymentsUserCancelledSms = new bool?();
      this.paymentsSmsSendingFailed = new bool?();
    }

    public override uint GetCode() => 1688;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_india_payments_psp_id_type>(this.paymentsPspId));
      Wam.MaybeSerializeField(3, this.paymentsBankId);
      Wam.MaybeSerializeField(4, this.paymentsSmsSuccessfullySent);
      Wam.MaybeSerializeField(5, this.paymentsUserCancelledSms);
      Wam.MaybeSerializeField(6, this.paymentsSmsSendingFailed);
    }
  }
}
