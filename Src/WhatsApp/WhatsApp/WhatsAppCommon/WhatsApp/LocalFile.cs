// Decompiled with JetBrains decompiler
// Type: WhatsApp.LocalFile
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "LocalFileUri", IsUnique = true)]
  [Index(Columns = "Sha1Hash", Name = "Sha1HashIndex")]
  public class LocalFile : PropChangingBase
  {
    private string localFileUri_;
    private byte[] sha1Hash_;
    private int? referenceCount_;
    private LocalFileType? fileType_;
    private long? fileSize_;
    private int? msgRefCount_;
    private int? statusRefCount_;
    private int? thumbRefCount_;
    private int? stickerRefCount_;
    private int? quotedMediaRefCount_;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int LocalFileID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LocalFileUri
    {
      get => this.localFileUri_;
      set
      {
        if (!(this.localFileUri_ != value))
          return;
        this.NotifyPropertyChanging(nameof (LocalFileUri));
        this.localFileUri_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] Sha1Hash
    {
      get => this.sha1Hash_;
      set
      {
        if (this.sha1Hash_ == value)
          return;
        this.NotifyPropertyChanging(nameof (Sha1Hash));
        this.sha1Hash_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? ReferenceCount
    {
      get => this.referenceCount_;
      set
      {
        int? referenceCount = this.referenceCount_;
        int? nullable = value;
        if ((referenceCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (referenceCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (ReferenceCount));
        this.referenceCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public LocalFileType? FileType
    {
      get => this.fileType_;
      set
      {
        LocalFileType? fileType = this.fileType_;
        LocalFileType? nullable = value;
        if ((fileType.GetValueOrDefault() == nullable.GetValueOrDefault() ? (fileType.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (FileType));
        this.fileType_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long? FileSize
    {
      get => this.fileSize_;
      set
      {
        long? fileSize = this.fileSize_;
        long? nullable = value;
        if ((fileSize.GetValueOrDefault() == nullable.GetValueOrDefault() ? (fileSize.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (FileSize));
        this.fileSize_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? MsgRefCount
    {
      get => this.msgRefCount_;
      set
      {
        int? msgRefCount = this.msgRefCount_;
        int? nullable = value;
        if ((msgRefCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (msgRefCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MsgRefCount));
        this.msgRefCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? StatusRefCount
    {
      get => this.statusRefCount_;
      set
      {
        int? statusRefCount = this.statusRefCount_;
        int? nullable = value;
        if ((statusRefCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (statusRefCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (StatusRefCount));
        this.statusRefCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? ThumbRefCount
    {
      get => this.thumbRefCount_;
      set
      {
        int? thumbRefCount = this.thumbRefCount_;
        int? nullable = value;
        if ((thumbRefCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (thumbRefCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (ThumbRefCount));
        this.thumbRefCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? StickerRefCount
    {
      get => this.stickerRefCount_;
      set
      {
        int? stickerRefCount = this.stickerRefCount_;
        int? nullable = value;
        if ((stickerRefCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (stickerRefCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (StickerRefCount));
        this.stickerRefCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? QuotedMediaRefCount
    {
      get => this.quotedMediaRefCount_;
      set
      {
        int? quotedMediaRefCount = this.quotedMediaRefCount_;
        int? nullable = value;
        if ((quotedMediaRefCount.GetValueOrDefault() == nullable.GetValueOrDefault() ? (quotedMediaRefCount.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (QuotedMediaRefCount));
        this.quotedMediaRefCount_ = value;
      }
    }
  }
}
