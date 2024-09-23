// Decompiled with JetBrains decompiler
// Type: WhatsApp.Sticker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using Newtonsoft.Json;
using System;
using System.Data.Linq.Mapping;
using System.IO;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "FileHash", IsUnique = true)]
  public class Sticker : PropChangingChangedBase
  {
    private string url;
    private byte[] fileHash;
    private byte[] encodedFileHash;
    private byte[] mediaKey;
    private string mimeType;
    private int height;
    private int width;
    private string localFileUri;
    private DateTime? dateTimeStarred;
    private string packId;
    private string directPath;
    private long fileLength;
    private int usageWeight;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int StickerID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("url")]
    public string Url
    {
      get => this.url;
      set
      {
        if (!(this.url != value))
          return;
        this.NotifyPropertyChanging(nameof (Url));
        this.url = value;
      }
    }

    [JsonProperty("file-hash")]
    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] FileHash
    {
      get => this.fileHash;
      set
      {
        if (this.fileHash == value)
          return;
        this.NotifyPropertyChanging(nameof (FileHash));
        this.fileHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("enc-file-hash")]
    public byte[] EncodedFileHash
    {
      get => this.encodedFileHash;
      set
      {
        if (this.encodedFileHash == value)
          return;
        this.NotifyPropertyChanging(nameof (EncodedFileHash));
        this.encodedFileHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("media-key")]
    public byte[] MediaKey
    {
      get => this.mediaKey;
      set
      {
        if (this.mediaKey == value)
          return;
        this.NotifyPropertyChanging(nameof (MediaKey));
        this.mediaKey = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("mime-type")]
    public string MimeType
    {
      get => this.mimeType;
      set
      {
        if (!(this.mimeType != value))
          return;
        this.NotifyPropertyChanging(nameof (MimeType));
        this.mimeType = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("height")]
    public int Height
    {
      get => this.height;
      set
      {
        if (this.height == value)
          return;
        this.NotifyPropertyChanging(nameof (Height));
        this.height = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [JsonProperty("width")]
    public int Width
    {
      get => this.width;
      set
      {
        if (this.width == value)
          return;
        this.NotifyPropertyChanging(nameof (Width));
        this.width = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string LocalFileUri
    {
      get => this.localFileUri;
      set
      {
        if (!(this.localFileUri != value))
          return;
        this.NotifyPropertyChanging(nameof (LocalFileUri));
        this.localFileUri = value;
        this.NotifyPropertyChanged(nameof (LocalFileUri));
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public DateTime? DateTimeStarred
    {
      get => this.dateTimeStarred;
      set
      {
        DateTime? dateTimeStarred = this.dateTimeStarred;
        DateTime? nullable = value;
        if ((dateTimeStarred.HasValue == nullable.HasValue ? (dateTimeStarred.HasValue ? (dateTimeStarred.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (DateTimeStarred));
        this.dateTimeStarred = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string PackId
    {
      get => this.packId;
      set
      {
        if (!(this.packId != value))
          return;
        this.NotifyPropertyChanging(nameof (PackId));
        this.packId = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string DirectPath
    {
      get => this.directPath;
      set
      {
        if (!(this.directPath != value))
          return;
        this.NotifyPropertyChanging(nameof (DirectPath));
        this.directPath = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public long FileLength
    {
      get => this.fileLength;
      set
      {
        if (this.fileLength == value)
          return;
        this.NotifyPropertyChanging(nameof (FileLength));
        this.fileLength = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int UsageWeight
    {
      get => this.usageWeight;
      set
      {
        if (this.usageWeight == value)
          return;
        this.NotifyPropertyChanging(nameof (UsageWeight));
        this.usageWeight = value;
      }
    }

    public MemoryStream GetThumbnailStream()
    {
      MemoryStream destination = (MemoryStream) null;
      if (this.LocalFileUri != null)
      {
        try
        {
          using (IMediaStorage mediaStorage = MediaStorage.Create(this.LocalFileUri))
          {
            if (mediaStorage.FileExists(this.LocalFileUri))
            {
              using (Stream stream = mediaStorage.OpenFile(this.LocalFileUri))
              {
                destination = new MemoryStream();
                stream.CopyTo((Stream) destination);
              }
              Log.d("sticker", "got sticker stream | filepath:{0}", (object) this.LocalFileUri);
            }
            else
              Log.d("sticker", "sticker file not found | filepath:{0}", (object) this.LocalFileUri);
          }
        }
        catch (Exception ex)
        {
          Log.l("sticker", "get thumb stream failed | filepath:{0}", (object) this.LocalFileUri);
          Log.LogException(ex, "open thumb file");
          destination = (MemoryStream) null;
        }
      }
      return destination;
    }
  }
}
