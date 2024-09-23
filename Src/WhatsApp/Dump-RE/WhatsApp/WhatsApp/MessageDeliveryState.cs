// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageDeliveryState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public abstract class MessageDeliveryState
  {
    protected Message msg_;
    protected Thickness itemMargin_ = new Thickness(0.0);
    protected bool inited_;
    protected List<ReceiptState> receiptsAddedBeforeInit_ = new List<ReceiptState>();

    public ReceiptState ServerReceipt { get; private set; }

    protected MessageDeliveryState(Message msg) => this.msg_ = msg;

    public static MessageDeliveryState Create(Message msg, IEnumerable<ReceiptState> receipts = null)
    {
      if (msg == null)
        throw new ArgumentException();
      MessageDeliveryState messageDeliveryState = JidHelper.IsMultiParticipantsChatJid(msg.KeyRemoteJid) ? (MessageDeliveryState) new MultiParticipantsMessageDeliveryState(msg) : (MessageDeliveryState) new IndividualMessageDeliveryState(msg);
      if (receipts != null)
        messageDeliveryState.LoadInitialReceipts(receipts);
      return messageDeliveryState;
    }

    public virtual void SetParticipants(IEnumerable<UserStatus> users)
    {
    }

    public virtual void LoadInitialReceipts(IEnumerable<ReceiptState> receipts)
    {
      this.inited_ = !this.inited_ ? true : throw new InvalidOperationException();
      foreach (ReceiptState receipt in this.receiptsAddedBeforeInit_)
        this.AddReceipt(receipt);
      foreach (ReceiptState receipt in receipts)
        this.AddReceipt(receipt);
    }

    public void AddReceipt(ReceiptState receipt)
    {
      if (!this.inited_)
      {
        this.receiptsAddedBeforeInit_.Add(receipt);
      }
      else
      {
        if (this.msg_ == null || this.msg_.MessageID != receipt.MessageId)
          return;
        if (receipt.Status == FunXMPP.FMessage.Status.ReceivedByServer)
          this.ServerReceipt = receipt;
        else
          this.AddTargetReceipt(receipt);
      }
    }

    protected abstract void AddTargetReceipt(ReceiptState receipt);

    public abstract IList GetListSource();

    public virtual void SetItemMargin(Thickness val, bool notify) => this.itemMargin_ = val;

    public class ItemBase : WaViewModelBase
    {
      private Thickness itemMargin_ = new Thickness(0.0);

      public Thickness ItemMargin => this.itemMargin_;

      public virtual FunXMPP.FMessage.Status Status { get; set; }

      public virtual void SetItemMargin(Thickness val, bool notify)
      {
        this.itemMargin_ = val;
        if (!notify)
          return;
        this.NotifyPropertyChanged("ItemMargin");
      }
    }

    public class RecipientItem : MessageDeliveryState.ItemBase
    {
      private FunXMPP.FMessage.Type msgType_;
      private UserStatus user_;
      private bool isSelected_;

      public string Jid { get; private set; }

      public UserStatus User
      {
        get
        {
          UserStatus user = this.user_;
          if (user != null)
            return user;
          return this.Jid != null ? (this.user_ = ContactsContext.Instance<UserStatus>((Func<ContactsContext, UserStatus>) (cdb => cdb.GetUserStatus(this.Jid)))) : (UserStatus) null;
        }
      }

      public override FunXMPP.FMessage.Status Status
      {
        get
        {
          ReceiptState mostRecentReceipt = this.MostRecentReceipt;
          return mostRecentReceipt != null ? mostRecentReceipt.Status : FunXMPP.FMessage.Status.Undefined;
        }
        set
        {
        }
      }

      public ReceiptState MostRecentReceipt => this.Played ?? this.Read ?? this.Delivered;

      public ReceiptState Delivered { get; private set; }

      public ReceiptState Read { get; private set; }

      public ReceiptState Played { get; private set; }

      public bool IsSelected
      {
        get => this.isSelected_;
        set
        {
          if (this.isSelected_ == value || !this.Status.IsReadByTarget())
            return;
          this.isSelected_ = value;
          this.NotifyPropertyChanged(nameof (IsSelected));
        }
      }

      public int ReceiptsListHeight => !this.Status.IsPlayedByTarget() ? 60 : 80;

      public RecipientItem(string jid, FunXMPP.FMessage.Type msgType, UserStatus user = null)
      {
        this.Jid = jid;
        this.user_ = user;
        this.msgType_ = msgType;
      }

      public static bool SortFunc(
        MessageDeliveryState.RecipientItem r1,
        MessageDeliveryState.RecipientItem r2)
      {
        return MessageDeliveryState.RecipientItem.CompareFunc(r1, r2) < 0;
      }

      public static int CompareFunc(
        MessageDeliveryState.RecipientItem r1,
        MessageDeliveryState.RecipientItem r2)
      {
        if (r1 == null)
          return 1;
        return r2 == null ? -1 : ParticipantSort.CompareParticipants(r1.User, r2.User, false);
      }

      public void AddReceipts(IEnumerable<ReceiptState> receipts)
      {
        foreach (ReceiptState receipt in receipts)
        {
          switch (receipt.Status)
          {
            case FunXMPP.FMessage.Status.ReceivedByTarget:
              this.Delivered = receipt;
              continue;
            case FunXMPP.FMessage.Status.PlayedByTarget:
              this.Played = receipt;
              continue;
            case FunXMPP.FMessage.Status.ReadByTarget:
              this.Read = receipt;
              continue;
            default:
              continue;
          }
        }
        if (this.Delivered == null)
        {
          ReceiptState receiptState = this.Read ?? this.Played;
          if (receiptState != null)
            this.Delivered = new ReceiptState()
            {
              MessageId = receiptState.MessageId,
              Jid = receiptState.Jid,
              Status = FunXMPP.FMessage.Status.ReceivedByTarget,
              Timestamp = receiptState.Timestamp
            };
        }
        if (this.Read != null)
          return;
        ReceiptState played = this.Played;
        if (played == null)
          return;
        this.Read = new ReceiptState()
        {
          MessageId = played.MessageId,
          Jid = played.Jid,
          Status = FunXMPP.FMessage.Status.ReadByTarget,
          Timestamp = played.Timestamp
        };
      }

      public void AddReceipt(ReceiptState receipt, out bool statusChanged)
      {
        statusChanged = false;
        if (receipt == null)
          return;
        ReceiptState mostRecentReceipt = this.MostRecentReceipt;
        this.AddReceipts((IEnumerable<ReceiptState>) new ReceiptState[1]
        {
          receipt
        });
        if (mostRecentReceipt != null && receipt.Status.GetOverrideWeight() <= mostRecentReceipt.Status.GetOverrideWeight())
          return;
        statusChanged = true;
      }

      public ReceiptState GetReceiptByType(FunXMPP.FMessage.Status status)
      {
        switch (status)
        {
          case FunXMPP.FMessage.Status.ReceivedByTarget:
            return this.Delivered;
          case FunXMPP.FMessage.Status.PlayedByTarget:
            return this.Played;
          case FunXMPP.FMessage.Status.ReadByTarget:
            return this.Read;
          default:
            return (ReceiptState) null;
        }
      }

      public List<ReceiptViewModel> GetReceiptViewModels(bool includeNull, bool includePlayed = true)
      {
        List<ReceiptViewModel> receiptViewModels = new List<ReceiptViewModel>();
        if (includePlayed && this.Played != null | includeNull)
          receiptViewModels.Add(new ReceiptViewModel(this.Played, FunXMPP.FMessage.Status.PlayedByTarget, this.msgType_));
        if (this.Read != null | includeNull)
          receiptViewModels.Add(new ReceiptViewModel(this.Read, FunXMPP.FMessage.Status.ReadByTarget, this.msgType_));
        if (this.Delivered != null | includeNull)
          receiptViewModels.Add(new ReceiptViewModel(this.Delivered, FunXMPP.FMessage.Status.ReceivedByTarget, this.msgType_));
        return receiptViewModels;
      }
    }
  }
}
