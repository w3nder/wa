// Decompiled with JetBrains decompiler
// Type: WhatsApp.GroupParticipantState
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "GroupJid")]
  public class GroupParticipantState : PropChangingBase
  {
    public const int FlagAdmin = 1;
    public const int FlagDistributionMessageSent = 2;
    public const int FlagSuperAdmin = 4;
    private int rowId;
    private string groupJid;
    private string memberJid;
    private long flags;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int GroupParticipantStateId
    {
      get => this.rowId;
      set
      {
        if (this.rowId == value)
          return;
        this.NotifyPropertyChanging(nameof (GroupParticipantStateId));
        this.rowId = value;
      }
    }

    [Column]
    public string GroupJid
    {
      get => this.groupJid;
      set
      {
        if (!(this.groupJid != value))
          return;
        this.NotifyPropertyChanging(nameof (GroupJid));
        this.groupJid = value;
      }
    }

    [Column]
    public string MemberJid
    {
      get => this.memberJid;
      set
      {
        if (!(this.memberJid != value))
          return;
        this.NotifyPropertyChanging(nameof (MemberJid));
        this.memberJid = value;
      }
    }

    [Column]
    public long Flags
    {
      get => this.flags;
      set
      {
        if (this.flags == value)
          return;
        this.NotifyPropertyChanging(nameof (Flags));
        this.flags = value;
      }
    }
  }
}
