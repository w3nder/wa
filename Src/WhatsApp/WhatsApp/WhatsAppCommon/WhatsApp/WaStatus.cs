// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatus
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "IsViewed", Name = "IsViewedIndex")]
  [Index(Columns = "Jid", Name = "JidIndex")]
  [Index(Columns = "MessageId", Name = "MessageIdIndex")]
  public class WaStatus : PropChangingChangedBase
  {
    public const string LogHeader = "statusv3";
    private string jid;
    private bool isViewed;

    public static TimeSpan Expiration => TimeSpan.FromHours(24.0);

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int StatusId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int MessageId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string MessageKeyId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Jid
    {
      get => this.jid;
      set
      {
        if (!(this.jid != value))
          return;
        this.NotifyPropertyChanging(nameof (Jid));
        this.jid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime Timestamp { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsViewed
    {
      get => this.isViewed;
      set
      {
        if (this.isViewed == value)
          return;
        this.NotifyPropertyChanging(nameof (IsViewed));
        this.isViewed = value;
        this.NotifyPropertyChanged(nameof (IsViewed));
      }
    }
  }
}
