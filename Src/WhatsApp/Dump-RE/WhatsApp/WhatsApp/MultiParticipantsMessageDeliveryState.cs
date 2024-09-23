// Decompiled with JetBrains decompiler
// Type: WhatsApp.MultiParticipantsMessageDeliveryState
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class MultiParticipantsMessageDeliveryState : MessageDeliveryState
  {
    private Dictionary<string, MessageDeliveryState.RecipientItem> recipientItems_ = new Dictionary<string, MessageDeliveryState.RecipientItem>();
    private KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> deliveredItems_;
    private KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> readItems_;
    private KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> playedItems_;
    private MessageDeliveryState.RecipientItem dotsItemForPlayed_ = new MessageDeliveryState.RecipientItem((string) null, FunXMPP.FMessage.Type.Undefined);
    private MessageDeliveryState.RecipientItem dotsItemForRead_ = new MessageDeliveryState.RecipientItem((string) null, FunXMPP.FMessage.Type.Undefined);
    private MessageDeliveryState.RecipientItem dotsItemForDelivered_ = new MessageDeliveryState.RecipientItem((string) null, FunXMPP.FMessage.Type.Undefined);

    public MultiParticipantsMessageDeliveryState(Message msg)
      : base(msg)
    {
      this.playedItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(new MultiParticipantsMessageDeliveryState.GroupItem(FunXMPP.FMessage.Status.PlayedByTarget, this.msg_.MediaWaType), (IEnumerable<MessageDeliveryState.RecipientItem>) new MessageDeliveryState.RecipientItem[0]);
      this.readItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(new MultiParticipantsMessageDeliveryState.GroupItem(FunXMPP.FMessage.Status.ReadByTarget, this.msg_.MediaWaType), (IEnumerable<MessageDeliveryState.RecipientItem>) new MessageDeliveryState.RecipientItem[0]);
      this.deliveredItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(new MultiParticipantsMessageDeliveryState.GroupItem(FunXMPP.FMessage.Status.ReceivedByTarget, this.msg_.MediaWaType), (IEnumerable<MessageDeliveryState.RecipientItem>) new MessageDeliveryState.RecipientItem[0]);
    }

    public override void SetParticipants(IEnumerable<UserStatus> users)
    {
      this.recipientItems_ = users.ToDictionary<UserStatus, string, MessageDeliveryState.RecipientItem>((Func<UserStatus, string>) (u => u.Jid), (Func<UserStatus, MessageDeliveryState.RecipientItem>) (u => new MessageDeliveryState.RecipientItem(u.Jid, this.msg_.MediaWaType, u)));
    }

    public override void LoadInitialReceipts(IEnumerable<ReceiptState> receipts)
    {
      this.inited_ = !this.inited_ ? true : throw new InvalidOperationException();
      if (this.receiptsAddedBeforeInit_.Count > 0)
        receipts = receipts.Concat<ReceiptState>((IEnumerable<ReceiptState>) this.receiptsAddedBeforeInit_);
      receipts.GroupBy<ReceiptState, string>((Func<ReceiptState, string>) (r => r.Jid)).Select<IGrouping<string, ReceiptState>, MessageDeliveryState.RecipientItem>((Func<IGrouping<string, ReceiptState>, MessageDeliveryState.RecipientItem>) (g =>
      {
        if (g.Key == null)
        {
          ReceiptState receipt = g.FirstOrDefault<ReceiptState>();
          if (receipt != null && receipt.Status == FunXMPP.FMessage.Status.ReceivedByServer)
            this.AddReceipt(receipt);
          return (MessageDeliveryState.RecipientItem) null;
        }
        MessageDeliveryState.RecipientItem recipientItem = this.GetRecipientItem(g.Key) ?? new MessageDeliveryState.RecipientItem(g.Key, this.msg_.MediaWaType);
        recipientItem.AddReceipts((IEnumerable<ReceiptState>) g);
        return recipientItem;
      })).GroupBy<MessageDeliveryState.RecipientItem, FunXMPP.FMessage.Status>((Func<MessageDeliveryState.RecipientItem, FunXMPP.FMessage.Status>) (recipient => recipient != null ? recipient.Status : FunXMPP.FMessage.Status.ReceivedByServer)).Select<IGrouping<FunXMPP.FMessage.Status, MessageDeliveryState.RecipientItem>, MultiParticipantsMessageDeliveryState.GroupItem>((Func<IGrouping<FunXMPP.FMessage.Status, MessageDeliveryState.RecipientItem>, MultiParticipantsMessageDeliveryState.GroupItem>) (g =>
      {
        if (g.Key == FunXMPP.FMessage.Status.ReceivedByServer)
          return (MultiParticipantsMessageDeliveryState.GroupItem) null;
        List<MessageDeliveryState.RecipientItem> list = g.ToList<MessageDeliveryState.RecipientItem>();
        list.Sort(new Comparison<MessageDeliveryState.RecipientItem>(MessageDeliveryState.RecipientItem.CompareFunc));
        MultiParticipantsMessageDeliveryState.GroupItem key = (MultiParticipantsMessageDeliveryState.GroupItem) null;
        switch (g.Key)
        {
          case FunXMPP.FMessage.Status.ReceivedByTarget:
            key = this.deliveredItems_.Key;
            this.deliveredItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(key, (IEnumerable<MessageDeliveryState.RecipientItem>) list);
            break;
          case FunXMPP.FMessage.Status.PlayedByTarget:
            key = this.playedItems_.Key;
            this.playedItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(key, (IEnumerable<MessageDeliveryState.RecipientItem>) list);
            break;
          case FunXMPP.FMessage.Status.ReadByTarget:
            key = this.readItems_.Key;
            this.readItems_ = new KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>(key, (IEnumerable<MessageDeliveryState.RecipientItem>) list);
            break;
        }
        key.Count = list.Count;
        return key;
      })).ToArray<MultiParticipantsMessageDeliveryState.GroupItem>();
    }

    public MessageDeliveryState.RecipientItem GetRecipientItem(string recipientJid)
    {
      MessageDeliveryState.RecipientItem recipientItem = (MessageDeliveryState.RecipientItem) null;
      this.recipientItems_.TryGetValue(recipientJid, out recipientItem);
      return recipientItem;
    }

    public KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> GetGroupByStatus(
      FunXMPP.FMessage.Status status)
    {
      switch (status)
      {
        case FunXMPP.FMessage.Status.ReceivedByTarget:
          return this.deliveredItems_;
        case FunXMPP.FMessage.Status.PlayedByTarget:
          return this.playedItems_;
        case FunXMPP.FMessage.Status.ReadByTarget:
          return this.readItems_;
        default:
          return (KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>) null;
      }
    }

    public override IList GetListSource() => (IList) this.GetGroupedItems();

    private List<KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>> GetGroupedItems()
    {
      List<KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>> groupedItems = new List<KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem>>();
      bool flag = false;
      if (this.msg_.IsPtt())
      {
        groupedItems.Add(this.playedItems_);
        this.playedItems_.Remove(this.dotsItemForPlayed_);
        if (!this.msg_.IsPlayedByTarget())
        {
          MessageDeliveryState.RecipientItem dotsItemForPlayed = this.dotsItemForPlayed_;
          ReceiptState receipt = new ReceiptState();
          receipt.Status = FunXMPP.FMessage.Status.PlayedByTarget;
          ref bool local = ref flag;
          dotsItemForPlayed.AddReceipt(receipt, out local);
          this.playedItems_.Add(this.dotsItemForPlayed_);
        }
      }
      if (!this.msg_.IsPlayedByTarget())
      {
        groupedItems.Add(this.readItems_);
        this.readItems_.Remove(this.dotsItemForRead_);
        if (!this.msg_.IsReadByTarget())
        {
          MessageDeliveryState.RecipientItem dotsItemForRead = this.dotsItemForRead_;
          ReceiptState receipt = new ReceiptState();
          receipt.Status = FunXMPP.FMessage.Status.ReadByTarget;
          ref bool local = ref flag;
          dotsItemForRead.AddReceipt(receipt, out local);
          this.readItems_.Add(this.dotsItemForRead_);
        }
      }
      if (!this.msg_.IsReadByTarget())
      {
        groupedItems.Add(this.deliveredItems_);
        this.deliveredItems_.Remove(this.dotsItemForDelivered_);
        if (!this.msg_.IsDeliveredToTarget())
        {
          MessageDeliveryState.RecipientItem itemForDelivered = this.dotsItemForDelivered_;
          ReceiptState receipt = new ReceiptState();
          receipt.Status = FunXMPP.FMessage.Status.ReceivedByTarget;
          ref bool local = ref flag;
          itemForDelivered.AddReceipt(receipt, out local);
          this.deliveredItems_.Add(this.dotsItemForDelivered_);
        }
      }
      return groupedItems;
    }

    public override void SetItemMargin(Thickness val, bool notify)
    {
      base.SetItemMargin(val, notify);
      foreach (KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> groupedItem in this.GetGroupedItems())
      {
        groupedItem.Key.SetItemMargin(val, notify);
        foreach (MessageDeliveryState.RecipientItem recipientItem in (Collection<MessageDeliveryState.RecipientItem>) groupedItem)
          recipientItem?.SetItemMargin(val, notify);
      }
    }

    protected override void AddTargetReceipt(ReceiptState receipt)
    {
      bool statusChanged = false;
      MessageDeliveryState.RecipientItem newItem = this.GetRecipientItem(receipt.Jid);
      ReceiptState receiptState = (ReceiptState) null;
      if (newItem == null)
      {
        newItem = new MessageDeliveryState.RecipientItem(receipt.Jid, this.msg_.MediaWaType);
        newItem.AddReceipt(receipt, out statusChanged);
        this.recipientItems_[receipt.Jid] = newItem;
      }
      else
      {
        receiptState = newItem.MostRecentReceipt;
        newItem.AddReceipt(receipt, out statusChanged);
      }
      if (!statusChanged)
        return;
      if (receiptState != null)
      {
        KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> groupByStatus = this.GetGroupByStatus(receiptState.Status);
        if (groupByStatus != null)
        {
          int index = 0;
          bool flag = false;
          foreach (MessageDeliveryState.RecipientItem recipientItem in (Collection<MessageDeliveryState.RecipientItem>) groupByStatus)
          {
            if (recipientItem.Jid == receipt.Jid)
            {
              flag = true;
              break;
            }
            ++index;
          }
          if (flag)
            groupByStatus.RemoveAt(index);
          if (groupByStatus.Key != null)
            groupByStatus.Key.Count = groupByStatus.Count;
        }
      }
      KeyedObservableCollection<MultiParticipantsMessageDeliveryState.GroupItem, MessageDeliveryState.RecipientItem> groupByStatus1 = this.GetGroupByStatus(newItem.MostRecentReceipt.Status);
      if (groupByStatus1 == null)
        return;
      newItem.SetItemMargin(this.itemMargin_, false);
      groupByStatus1.InsertInOrder<MessageDeliveryState.RecipientItem>(newItem, new Func<MessageDeliveryState.RecipientItem, MessageDeliveryState.RecipientItem, bool>(MessageDeliveryState.RecipientItem.SortFunc));
      if (groupByStatus1.Key == null)
        return;
      groupByStatus1.Key.Count = groupByStatus1.Count;
    }

    public class GroupItem : MessageDeliveryState.ItemBase
    {
      private FunXMPP.FMessage.Type msgType_;
      private FunXMPP.FMessage.Status status_;
      private int count_;

      public override FunXMPP.FMessage.Status Status
      {
        get => this.status_;
        set
        {
          if (this.status_ == value)
            return;
          this.status_ = value;
          this.NotifyPropertyChanged("StatusStr");
          this.NotifyPropertyChanged("StatusIcon");
          this.NotifyPropertyChanged("StatusIconWidth");
          this.NotifyPropertyChanged("StatusIconHeight");
        }
      }

      public int Count
      {
        get => this.count_;
        set
        {
          if (this.count_ == value)
            return;
          this.count_ = value;
        }
      }

      public string StatusStr
      {
        get
        {
          switch (this.status_)
          {
            case FunXMPP.FMessage.Status.ReceivedByTarget:
              return AppResources.MessageStatusDeliveredTo;
            case FunXMPP.FMessage.Status.PlayedByTarget:
              return AppResources.MessageStatusPlayedBy;
            case FunXMPP.FMessage.Status.ReadByTarget:
              return this.msgType_ != FunXMPP.FMessage.Type.Undefined && this.msgType_ != FunXMPP.FMessage.Type.ExtendedText ? AppResources.MessageStatusSeenBy : AppResources.MessageStatusReadBy;
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

      public double StatusIconWidth => this.status_.IsPlayedByTarget() ? 18.0 : 25.0;

      public double StatusIconHeight => this.status_.IsPlayedByTarget() ? 27.0 : 18.0;

      public GroupItem(FunXMPP.FMessage.Status status, FunXMPP.FMessage.Type msgType)
      {
        this.status_ = status;
        this.msgType_ = msgType;
      }

      public override void SetItemMargin(Thickness val, bool notify)
      {
        val.Top += 24.0;
        val.Bottom += 14.0;
        base.SetItemMargin(val, notify);
      }
    }
  }
}
