// Decompiled with JetBrains decompiler
// Type: WhatsApp.VCard
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid")]
  public class VCard : PropChangingChangedBase
  {
    private string jid;
    private int? messageId_;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int VCardId { get; set; }

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
    public int? MessageId
    {
      get => this.messageId_;
      set
      {
        int? messageId = this.messageId_;
        int? nullable = value;
        if ((messageId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (messageId.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MessageId));
        this.messageId_ = value;
      }
    }
  }
}
