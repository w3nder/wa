// Decompiled with JetBrains decompiler
// Type: WhatsApp.PostponedReceipt
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  public class PostponedReceipt : PropChangingBase
  {
    private string targetJid;
    private string participantJid;
    private string keyId;
    private long timestampLong;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int PostponedReceiptId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string TargetJid
    {
      get => this.targetJid;
      set
      {
        if (!(this.targetJid != value))
          return;
        this.NotifyPropertyChanging(nameof (TargetJid));
        this.targetJid = value;
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
        this.NotifyPropertyChanging(nameof (ParticipantJid));
        this.participantJid = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyId
    {
      get => this.keyId;
      set
      {
        if (!(this.keyId != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyId));
        this.keyId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long TimestampLong
    {
      get => this.timestampLong;
      set
      {
        if (this.timestampLong == value)
          return;
        this.NotifyPropertyChanging(nameof (TimestampLong));
        this.timestampLong = DateTimeUtils.SanitizeTimestamp(value);
      }
    }

    public PostponedReceipt()
    {
    }

    public PostponedReceipt(ReceiptSpec receipt, long postponeTime)
    {
      this.targetJid = receipt.Jid;
      this.participantJid = receipt.Participant;
      this.keyId = receipt.Id;
      this.timestampLong = postponeTime;
    }
  }
}
