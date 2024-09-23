// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsNavBankSelect
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class IndiaPaymentsNavBankSelect : WamEvent
  {
    public string paymentsEventId;
    public bool? paymentsBackSelected;
    public bool? paymentsAppExitSelected;
    public bool? paymentsAccountsExist;
    public bool? paymentsBanksSearchActivated;
    public string paymentsBanksSearchString;
    public bool? paymentsBanksSearchSelected;
    public bool? paymentsBanksScrolled;
    public long? paymentsBanksRowSelected;
    public string bankSelected;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsBackSelected = new bool?();
      this.paymentsAppExitSelected = new bool?();
      this.paymentsAccountsExist = new bool?();
      this.paymentsBanksSearchActivated = new bool?();
      this.paymentsBanksSearchString = (string) null;
      this.paymentsBanksSearchSelected = new bool?();
      this.paymentsBanksScrolled = new bool?();
      this.paymentsBanksRowSelected = new long?();
      this.bankSelected = (string) null;
    }

    public override uint GetCode() => 1622;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, this.paymentsBackSelected);
      Wam.MaybeSerializeField(3, this.paymentsAppExitSelected);
      Wam.MaybeSerializeField(4, this.paymentsAccountsExist);
      Wam.MaybeSerializeField(6, this.paymentsBanksSearchActivated);
      Wam.MaybeSerializeField(7, this.paymentsBanksSearchString);
      Wam.MaybeSerializeField(8, this.paymentsBanksSearchSelected);
      Wam.MaybeSerializeField(9, this.paymentsBanksScrolled);
      Wam.MaybeSerializeField(10, this.paymentsBanksRowSelected);
      Wam.MaybeSerializeField(5, this.bankSelected);
    }
  }
}
