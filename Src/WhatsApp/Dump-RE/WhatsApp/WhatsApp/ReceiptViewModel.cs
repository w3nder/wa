// Decompiled with JetBrains decompiler
// Type: WhatsApp.ReceiptViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ReceiptViewModel : WaViewModelBase
  {
    private FunXMPP.FMessage.Type msgType_;
    private FunXMPP.FMessage.Status status_;
    private ReceiptState receipt_;
    private DateTime? localTimestamp_;

    public string DateStr
    {
      get
      {
        if (!this.localTimestamp_.HasValue)
          return (string) null;
        DateTime now = DateTime.Now;
        return now.DayOfYear == this.localTimestamp_.Value.DayOfYear && (now - this.localTimestamp_.Value).TotalHours < 24.0 ? (string) null : DateTimeUtils.FormatCompactDate(this.localTimestamp_.Value);
      }
    }

    public string TimeStr
    {
      get
      {
        return this.localTimestamp_.HasValue ? DateTimeUtils.FormatCompactTime(this.localTimestamp_.Value) : "◦ ◦ ◦";
      }
    }

    public double TimeFontSize => this.localTimestamp_.HasValue ? 22.0 : 32.0;

    public string StatusStr
    {
      get
      {
        switch (this.receipt_ == null ? this.status_ : this.receipt_.Status)
        {
          case FunXMPP.FMessage.Status.ReceivedByServer:
            return AppResources.MessageStatusSent;
          case FunXMPP.FMessage.Status.ReceivedByTarget:
            return AppResources.MessageStatusDelivered;
          case FunXMPP.FMessage.Status.PlayedByTarget:
            return AppResources.MessageStatusPlayed;
          case FunXMPP.FMessage.Status.ReadByTarget:
            return this.msgType_ != FunXMPP.FMessage.Type.Undefined && this.msgType_ != FunXMPP.FMessage.Type.ExtendedText ? AppResources.MessageStatusSeen : AppResources.MessageStatusRead;
          default:
            return (string) null;
        }
      }
    }

    public System.Windows.Media.ImageSource StatusIcon
    {
      get
      {
        switch (this.status_)
        {
          case FunXMPP.FMessage.Status.ReceivedByTarget:
            return (System.Windows.Media.ImageSource) AssetStore.InlineDoubleChecks;
          case FunXMPP.FMessage.Status.PlayedByTarget:
            return (System.Windows.Media.ImageSource) AssetStore.InlineMicBlue;
          case FunXMPP.FMessage.Status.ReadByTarget:
            return (System.Windows.Media.ImageSource) AssetStore.InlineBlueChecks;
          default:
            return (System.Windows.Media.ImageSource) null;
        }
      }
    }

    public double StatusIconWidth
    {
      get => this.status_ == FunXMPP.FMessage.Status.PlayedByTarget ? 18.0 : 25.0;
    }

    public double StatusIconHeight
    {
      get => this.status_ == FunXMPP.FMessage.Status.PlayedByTarget ? 27.0 : 18.0;
    }

    public Thickness ItemMargin { get; set; }

    public ReceiptViewModel(
      ReceiptState receipt,
      FunXMPP.FMessage.Status status,
      FunXMPP.FMessage.Type msgType)
    {
      if (receipt == null)
      {
        this.status_ = status;
      }
      else
      {
        this.receipt_ = receipt;
        this.status_ = receipt.Status;
        this.localTimestamp_ = new DateTime?(this.receipt_.LocalTimestamp);
      }
      this.msgType_ = msgType;
    }
  }
}
