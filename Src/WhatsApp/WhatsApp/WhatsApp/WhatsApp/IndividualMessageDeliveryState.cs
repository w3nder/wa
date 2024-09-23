// Decompiled with JetBrains decompiler
// Type: WhatsApp.IndividualMessageDeliveryState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections;
using System.Collections.Generic;


namespace WhatsApp
{
  public class IndividualMessageDeliveryState : MessageDeliveryState
  {
    private MessageDeliveryState.RecipientItem item_;

    public IndividualMessageDeliveryState(Message msg)
      : base(msg)
    {
      this.item_ = new MessageDeliveryState.RecipientItem(msg.KeyRemoteJid, msg.MediaWaType);
    }

    protected override void AddTargetReceipt(ReceiptState receipt)
    {
      bool statusChanged = false;
      this.item_.AddReceipt(receipt, out statusChanged);
    }

    public override IList GetListSource()
    {
      if (this.msg_ == null)
        return (IList) new List<ReceiptViewModel>();
      List<ReceiptViewModel> receiptViewModels = this.item_.GetReceiptViewModels(true, this.msg_.IsPtt());
      receiptViewModels.ForEach((Action<ReceiptViewModel>) (vm => vm.ItemMargin = this.itemMargin_));
      return (IList) receiptViewModels;
    }
  }
}
