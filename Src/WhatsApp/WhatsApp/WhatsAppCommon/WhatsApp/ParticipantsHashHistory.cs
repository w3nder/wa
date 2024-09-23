// Decompiled with JetBrains decompiler
// Type: WhatsApp.ParticipantsHashHistory
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "GroupJid")]
  public class ParticipantsHashHistory : PropChangingChangedBase
  {
    private string groupJid;
    private string participantJid;
    private ParticipantsHashHistory.ParticipantActions participantAction;
    private string oldHash;
    private string newHash;
    private DateTime? timestamp;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string GroupJid
    {
      get => this.groupJid;
      set
      {
        if (!(this.groupJid != value))
          return;
        this.NotifyPropertyChanging("GroupId");
        this.groupJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string ParticipantJid
    {
      get => this.participantJid;
      set
      {
        if (!(this.participantJid != value))
          return;
        this.NotifyPropertyChanging("ParticipantId");
        this.participantJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public ParticipantsHashHistory.ParticipantActions ParticipantAction
    {
      get => this.participantAction;
      set
      {
        if (this.participantAction == value)
          return;
        this.NotifyPropertyChanging(nameof (ParticipantAction));
        this.participantAction = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string OldHash
    {
      get => this.oldHash;
      set
      {
        if (!(this.oldHash != value))
          return;
        this.NotifyPropertyChanging(nameof (OldHash));
        this.oldHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string NewHash
    {
      get => this.newHash;
      set
      {
        if (!(this.newHash != value))
          return;
        this.NotifyPropertyChanging(nameof (NewHash));
        this.newHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? Timestamp
    {
      get => this.timestamp;
      set
      {
        DateTime? timestamp = this.timestamp;
        DateTime? nullable = value;
        if ((timestamp.HasValue == nullable.HasValue ? (timestamp.HasValue ? (timestamp.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (Timestamp));
        this.timestamp = value;
      }
    }

    public enum ParticipantActions
    {
      Added,
      Removed,
    }
  }
}
