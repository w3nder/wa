// Decompiled with JetBrains decompiler
// Type: WhatsApp.ClientCapability
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
  [Index(Columns = "Jid,Category", IsUnique = true)]
  public class ClientCapability : PropChangingBase
  {
    private string jid;
    private DateTime? lastUpdate;
    private ClientCapabilityCategory category;
    private ClientCapabilitySetting value;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ClientCapID { get; set; }

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

    [Column(UpdateCheck = UpdateCheck.Never)]
    public ClientCapabilityCategory Category
    {
      get => this.category;
      set
      {
        if (this.category == value)
          return;
        this.NotifyPropertyChanging(nameof (Category));
        this.category = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public ClientCapabilitySetting Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.NotifyPropertyChanging(nameof (Value));
        this.value = value;
      }
    }
  }
}
