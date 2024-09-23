﻿// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsNavWelcome
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class IndiaPaymentsNavWelcome : WamEvent
  {
    public string paymentsEventId;
    public bool? paymentsBackSelected;
    public bool? paymentsAppExitSelected;
    public bool? paymentsAccountsExist;
    public bool? waTermsSelected;
    public bool? pspTermsSelected;
    public bool? continueSelected;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsBackSelected = new bool?();
      this.paymentsAppExitSelected = new bool?();
      this.paymentsAccountsExist = new bool?();
      this.waTermsSelected = new bool?();
      this.pspTermsSelected = new bool?();
      this.continueSelected = new bool?();
    }

    public override uint GetCode() => 1620;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, this.paymentsBackSelected);
      Wam.MaybeSerializeField(3, this.paymentsAppExitSelected);
      Wam.MaybeSerializeField(4, this.paymentsAccountsExist);
      Wam.MaybeSerializeField(5, this.waTermsSelected);
      Wam.MaybeSerializeField(6, this.pspTermsSelected);
      Wam.MaybeSerializeField(7, this.continueSelected);
    }
  }
}
