// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsDbSmsSentManual
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class IndiaPaymentsDbSmsSentManual : WamEvent
  {
    public string paymentsEventId;
    public bool? paymentsBackSelected;
    public bool? paymentsSmsSuccessfullySent;
    public bool? paymentsUserCancelledSms;
    public bool? paymentsSmsSendingFailed;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsBackSelected = new bool?();
      this.paymentsSmsSuccessfullySent = new bool?();
      this.paymentsUserCancelledSms = new bool?();
      this.paymentsSmsSendingFailed = new bool?();
    }

    public override uint GetCode() => 1690;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, this.paymentsBackSelected);
      Wam.MaybeSerializeField(3, this.paymentsSmsSuccessfullySent);
      Wam.MaybeSerializeField(4, this.paymentsUserCancelledSms);
      Wam.MaybeSerializeField(5, this.paymentsSmsSendingFailed);
    }
  }
}
