// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChangeNumberRecord
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
  [Index(Columns = "OldJid")]
  [Index(Columns = "NewJid")]
  public class ChangeNumberRecord : PropChangingBase
  {
    private int recordId;
    private string oldJid;
    private string newJid;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int RecordId
    {
      get => this.recordId;
      set
      {
        this.NotifyPropertyChanging(nameof (RecordId));
        this.recordId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string OldJid
    {
      get => this.oldJid;
      set
      {
        this.NotifyPropertyChanging(nameof (OldJid));
        this.oldJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string NewJid
    {
      get => this.newJid;
      set
      {
        this.NotifyPropertyChanging(nameof (NewJid));
        this.newJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime Timestamp { get; set; }
  }
}
