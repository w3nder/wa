// Decompiled with JetBrains decompiler
// Type: WhatsApp.FrequentChatScore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid", IsUnique = true)]
  public class FrequentChatScore : PropChangingChangedBase
  {
    private string jid;
    private long defaultScore;
    private long imageScore;
    private long videoScore;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ID { get; set; }

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
    public long DefaultScore
    {
      get => this.defaultScore;
      set
      {
        if (this.defaultScore == value)
          return;
        this.NotifyPropertyChanging(nameof (DefaultScore));
        this.defaultScore = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long ImageScore
    {
      get => this.imageScore;
      set
      {
        if (this.imageScore == value)
          return;
        this.NotifyPropertyChanging(nameof (ImageScore));
        this.imageScore = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long VideoScore
    {
      get => this.videoScore;
      set
      {
        if (this.videoScore == value)
          return;
        this.NotifyPropertyChanging(nameof (VideoScore));
        this.videoScore = value;
      }
    }
  }
}
