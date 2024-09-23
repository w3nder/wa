// Decompiled with JetBrains decompiler
// Type: WhatsApp.ChatPicture
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Data.Linq.Mapping;


namespace WhatsApp
{
  [Table]
  [Index(Columns = "Jid", IsUnique = true)]
  public class ChatPicture : PropChangingChangedBase
  {
    private string jid;
    private string waPicId;
    private DateTime? lastPicCheck;
    private DateTime? blockPicReqUntil;
    private byte[] picData;
    private byte[] thumbData;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ChatPictureId { get; set; }

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
    public string WaPhotoId
    {
      get => this.waPicId;
      set
      {
        if (!(this.waPicId != value))
          return;
        this.NotifyPropertyChanging(nameof (WaPhotoId));
        this.waPicId = value;
        this.NotifyPropertyChanged(nameof (WaPhotoId));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? LastPictureCheck
    {
      get => this.lastPicCheck;
      set
      {
        DateTime? nullable1 = value;
        DateTime? lastPicCheck = this.lastPicCheck;
        DateTime? nullable2 = nullable1;
        if ((lastPicCheck.HasValue == nullable2.HasValue ? (lastPicCheck.HasValue ? (lastPicCheck.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (LastPictureCheck));
        if (nullable1.HasValue && nullable1.Value.Kind == DateTimeKind.Local)
          nullable1 = new DateTime?(nullable1.Value.ToUniversalTime());
        this.lastPicCheck = nullable1;
        this.BlockPictureRequestUntil = new DateTime?();
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? BlockPictureRequestUntil
    {
      get
      {
        return !this.blockPicReqUntil.HasValue ? new DateTime?() : new DateTime?(this.blockPicReqUntil.Value.ToLocalTime());
      }
      set
      {
        DateTime? nullable1 = value;
        DateTime? blockPicReqUntil = this.blockPicReqUntil;
        DateTime? nullable2 = nullable1;
        if ((blockPicReqUntil.HasValue == nullable2.HasValue ? (blockPicReqUntil.HasValue ? (blockPicReqUntil.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (BlockPictureRequestUntil));
        if (nullable1.HasValue && nullable1.Value.Kind == DateTimeKind.Local)
          nullable1 = new DateTime?(nullable1.Value.ToUniversalTime());
        this.blockPicReqUntil = nullable1;
      }
    }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    public byte[] PictureData
    {
      get => this.picData;
      set
      {
        if (this.picData == value)
          return;
        this.NotifyPropertyChanging(nameof (PictureData));
        this.picData = value;
      }
    }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    public byte[] ThumbnailData
    {
      get => this.thumbData;
      set
      {
        if (this.thumbData == value)
          return;
        this.NotifyPropertyChanging(nameof (ThumbnailData));
        this.thumbData = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string SavedPhotoId
    {
      get => (string) null;
      set
      {
      }
    }

    public bool IsPictureExpired()
    {
      if (this.Jid == Settings.MyJid)
        return false;
      return !this.LastPictureCheck.HasValue || DateTime.UtcNow - this.LastPictureCheck.Value > Constants.ChatPicturePullInterval;
    }

    public bool ShouldBlockPictureRequest()
    {
      return this.BlockPictureRequestUntil.HasValue && this.BlockPictureRequestUntil.Value > DateTime.Now;
    }

    public void BlockPictureRequest(TimeSpan blockPeriod)
    {
      this.BlockPictureRequestUntil = new DateTime?(DateTime.Now + blockPeriod);
    }
  }
}
