// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneNumber
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "RawPhoneNumber")]
  [Index(Columns = "Jid", Name = "Jid")]
  [Index(Columns = "IsNew", Name = "IsNewIdx")]
  public class PhoneNumber : PropChangingBase
  {
    private int phoneNumberId;
    private string rawPhoneNumber;
    private string jid;
    private bool? isNew;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int PhoneNumberID
    {
      get => this.phoneNumberId;
      set
      {
        this.NotifyPropertyChanging(nameof (PhoneNumberID));
        this.phoneNumberId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string RawPhoneNumber
    {
      get => this.rawPhoneNumber;
      set
      {
        this.NotifyPropertyChanging(nameof (RawPhoneNumber));
        this.rawPhoneNumber = value;
      }
    }

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
    public bool? IsNew
    {
      get => this.isNew;
      set
      {
        bool? isNew = this.isNew;
        bool? nullable = value;
        if ((isNew.GetValueOrDefault() == nullable.GetValueOrDefault() ? (isNew.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (IsNew));
        this.isNew = value;
      }
    }
  }
}
