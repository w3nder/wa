// Decompiled with JetBrains decompiler
// Type: WhatsApp.BlockList
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  public class BlockList : PropChangingBase
  {
    private string members;
    private DateTime? lastUpdate;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int BlockListID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Members
    {
      get => this.members;
      set
      {
        if (!(this.members != value))
          return;
        this.NotifyPropertyChanging(nameof (Members));
        this.members = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? LastUpdate
    {
      get => this.lastUpdate;
      set
      {
        DateTime? lastUpdate = this.lastUpdate;
        DateTime? nullable = value;
        if ((lastUpdate.HasValue == nullable.HasValue ? (lastUpdate.HasValue ? (lastUpdate.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (LastUpdate));
        this.lastUpdate = value;
      }
    }
  }
}
