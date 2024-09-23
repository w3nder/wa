// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsVerifyDebitCard
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class IndiaPaymentsVerifyDebitCard : WamEvent
  {
    public string paymentsEventId;
    public wam_enum_india_payments_psp_id_type? paymentsPspId;
    public string paymentsBankId;
    public bool? paymentsBackSelected;
    public bool? paymentsContinueSelected;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsPspId = new wam_enum_india_payments_psp_id_type?();
      this.paymentsBankId = (string) null;
      this.paymentsBackSelected = new bool?();
      this.paymentsContinueSelected = new bool?();
    }

    public override uint GetCode() => 1694;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_india_payments_psp_id_type>(this.paymentsPspId));
      Wam.MaybeSerializeField(3, this.paymentsBankId);
      Wam.MaybeSerializeField(4, this.paymentsBackSelected);
      Wam.MaybeSerializeField(5, this.paymentsContinueSelected);
    }
  }
}
