// Decompiled with JetBrains decompiler
// Type: WhatsApp.CipherTextReceipt
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "KeyRemoteJid,KeyId,ParticipantJid")]
  [Index(Columns = "KeyRemoteJid,IsCipherText")]
  public class CipherTextReceipt : PropChangingBase
  {
    private string keyRemoteJid;
    private string participantJid;
    private string keyId;
    private bool isCipherText;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int CipherTextReceiptId { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string KeyRemoteJid
    {
      get => this.keyRemoteJid;
      set
      {
        if (!(this.keyRemoteJid != value))
          return;
        this.NotifyPropertyChanging(nameof (KeyRemoteJid));
        this.keyRemoteJid = value;
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
    public bool IsCipherText
    {
      get => this.isCipherText;
      set
      {
        if (this.isCipherText == value)
          return;
        this.NotifyPropertyChanging(nameof (IsCipherText));
        this.isCipherText = value;
      }
    }

    public CipherTextReceipt()
    {
    }

    public CipherTextReceipt(ReceiptSpec receipt)
    {
      this.keyRemoteJid = receipt.Jid;
      this.participantJid = receipt.Participant;
      this.keyId = receipt.Id;
      this.isCipherText = receipt.IsCipherText;
    }
  }
}
