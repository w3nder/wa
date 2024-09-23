// Decompiled with JetBrains decompiler
// Type: WhatsApp.ReceiptState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "MessageId,Jid")]
  public class ReceiptState : PropChangingBase
  {
    private int messageId_;
    private string jid_;
    private FunXMPP.FMessage.Status status_;
    private DateTime timestamp_;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ReceiptStateId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int MessageId
    {
      get => this.messageId_;
      set
      {
        if (this.messageId_ == value)
          return;
        this.NotifyPropertyChanging(nameof (MessageId));
        this.messageId_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Jid
    {
      get => this.jid_;
      set
      {
        if (!(this.jid_ != value))
          return;
        this.NotifyPropertyChanging(nameof (Jid));
        this.jid_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public FunXMPP.FMessage.Status Status
    {
      get => this.status_;
      set
      {
        if (this.status_ == value)
          return;
        this.NotifyPropertyChanging(nameof (Status));
        this.status_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime Timestamp
    {
      get => this.timestamp_;
      set
      {
        if (!(this.timestamp_ != value))
          return;
        this.NotifyPropertyChanging("MessageId");
        this.timestamp_ = value;
      }
    }

    public DateTime LocalTimestamp => DateTimeUtils.FunTimeToPhoneTime(this.timestamp_);
  }
}
