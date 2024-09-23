// Decompiled with JetBrains decompiler
// Type: WhatsApp.UserStatus
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;
using WhatsApp.ProtoBuf;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid", IsUnique = true)]
  public class UserStatus : PropChangingChangedBase
  {
    private string jid;
    private string photoPath;
    private byte[] photoHash;
    private string status;
    private DateTime? dateTimeSet;
    private string contactName;
    private string firstName;
    private string pushName;
    private bool isInDeviceContactList;
    private bool isSidelistSynced;
    private bool isInDevicePhonebook;
    private bool isWaUser;
    private PhoneNumberKind phoneNumberKind;
    private VerifiedNameState verifiedName;
    private byte[] verifiedNameCertificateDetailsSerialized;
    private VerifiedLevel verifiedLevel;
    private byte[] internalPropertiesProtoBuf;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int StatusID { get; set; }

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
    public string PhotoPath
    {
      get => this.photoPath;
      set
      {
        if (!(this.photoPath != value))
          return;
        this.NotifyPropertyChanging(nameof (PhotoPath));
        this.photoPath = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] PhotoHash
    {
      get => this.photoHash;
      set
      {
        if (this.photoHash == value)
          return;
        this.NotifyPropertyChanging(nameof (PhotoHash));
        this.photoHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Status
    {
      get => this.status;
      set
      {
        if (!(this.status != value))
          return;
        this.NotifyPropertyChanging(nameof (Status));
        this.status = value;
        this.NotifyPropertyChanged(nameof (Status));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? DateTimeSet
    {
      get => this.dateTimeSet;
      set
      {
        DateTime? dateTimeSet = this.dateTimeSet;
        DateTime? nullable = value;
        if ((dateTimeSet.HasValue == nullable.HasValue ? (dateTimeSet.HasValue ? (dateTimeSet.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (DateTimeSet));
        this.dateTimeSet = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    public string ContactName
    {
      get => this.contactName;
      set
      {
        if (!(this.contactName != value))
          return;
        this.NotifyPropertyChanging(nameof (ContactName));
        this.contactName = value;
        this.NotifyPropertyChanged(nameof (ContactName));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string FirstName
    {
      get => this.firstName;
      set
      {
        if (!(this.firstName != value))
          return;
        this.NotifyPropertyChanging(nameof (FirstName));
        this.firstName = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string PushName
    {
      get => this.pushName;
      set
      {
        if (!(this.pushName != value))
          return;
        this.NotifyPropertyChanging(nameof (PushName));
        this.pushName = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsInDeviceContactList
    {
      get => this.isInDeviceContactList;
      set
      {
        if (this.isInDeviceContactList == value)
          return;
        this.NotifyPropertyChanging(nameof (IsInDeviceContactList));
        this.isInDeviceContactList = value;
        this.NotifyPropertyChanged(nameof (IsInDeviceContactList));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsSidelistSynced
    {
      get => this.isSidelistSynced;
      set
      {
        this.NotifyPropertyChanging(nameof (IsSidelistSynced));
        this.isSidelistSynced = value;
        this.NotifyPropertyChanged(nameof (IsSidelistSynced));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsInDevicePhonebook
    {
      get => this.IsInDeviceContactList || this.isInDevicePhonebook;
      set
      {
        if (this.isInDevicePhonebook == value)
          return;
        this.NotifyPropertyChanging(nameof (IsInDevicePhonebook));
        this.isInDevicePhonebook = value;
        this.NotifyPropertyChanged(nameof (IsInDevicePhonebook));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool IsWaUser
    {
      get => this.isWaUser;
      set
      {
        if (this.isWaUser == value)
          return;
        this.NotifyPropertyChanging(nameof (IsWaUser));
        this.isWaUser = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public PhoneNumberKind PhoneNumberKind
    {
      get => this.phoneNumberKind;
      set
      {
        if (this.phoneNumberKind == value)
          return;
        this.NotifyPropertyChanging(nameof (PhoneNumberKind));
        this.phoneNumberKind = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public VerifiedNameState VerifiedName
    {
      get => this.verifiedName;
      set
      {
        if (this.verifiedName == value)
          return;
        this.NotifyPropertyChanging(nameof (VerifiedName));
        this.verifiedName = value;
        this.NotifyPropertyChanged(nameof (VerifiedName));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] VerifiedNameCertificateDetailsSerialized
    {
      get => this.verifiedNameCertificateDetailsSerialized;
      set
      {
        if (this.verifiedNameCertificateDetailsSerialized == value)
          return;
        this.NotifyPropertyChanging(nameof (VerifiedNameCertificateDetailsSerialized));
        this.verifiedNameCertificateDetailsSerialized = value;
        this.NotifyPropertyChanged(nameof (VerifiedNameCertificateDetailsSerialized));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public VerifiedLevel VerifiedLevel
    {
      get => this.verifiedLevel;
      set
      {
        if (this.verifiedLevel == value)
          return;
        this.NotifyPropertyChanging(nameof (VerifiedLevel));
        this.verifiedLevel = value;
        this.NotifyPropertyChanged(nameof (VerifiedLevel));
      }
    }

    public VerifiedNameCertificate.Details VerifiedNameCertificateDetails
    {
      get
      {
        return this.VerifiedNameCertificateDetailsSerialized == null ? (VerifiedNameCertificate.Details) null : VerifiedNameCertificate.Details.Deserialize(this.VerifiedNameCertificateDetailsSerialized);
      }
      set
      {
        this.VerifiedNameCertificateDetailsSerialized = value != null ? VerifiedNameCertificate.Details.SerializeToBytes(value) : (byte[]) null;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] InternalPropertiesProtobuf
    {
      get => this.internalPropertiesProtoBuf;
      set
      {
        if (this.internalPropertiesProtoBuf == value)
          return;
        this.NotifyPropertyChanging(nameof (InternalPropertiesProtobuf));
        this.internalPropertiesProtoBuf = value;
      }
    }

    public UserStatusProperties InternalProperties
    {
      get
      {
        return this.InternalPropertiesProtobuf == null ? (UserStatusProperties) null : UserStatusProperties.Deserialize(this.InternalPropertiesProtobuf);
      }
      set
      {
        this.InternalPropertiesProtobuf = value != null ? UserStatusProperties.SerializeToBytes(value) : (byte[]) null;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? LastSeenTimestamp { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public bool? OmitFromFavorites { get; set; }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public byte[] Photo { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? LastSeen { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public string PhotoID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public DateTime? LastPhotoCheck { get; set; }

    public string GetDisplayName(
      bool preferFirstName = false,
      bool getNumberIfNoName = true,
      bool getFormattedNumber = true,
      bool skipAddressBookCheck = false)
    {
      string str = (string) null;
      if (!this.IsInDeviceContactList && this.IsVerified() && this.VerifiedLevel == VerifiedLevel.high)
        str = this.GetVerifiedNameForDisplay();
      if (string.IsNullOrEmpty(str) && this.IsInDevicePhonebook | skipAddressBookCheck)
        str = !preferFirstName || string.IsNullOrEmpty(this.FirstName) ? this.ContactName : this.FirstName;
      if (string.IsNullOrEmpty(str) & getNumberIfNoName)
        str = JidHelper.GetPhoneNumber(this.Jid, getFormattedNumber);
      return str ?? "";
    }
  }
}
