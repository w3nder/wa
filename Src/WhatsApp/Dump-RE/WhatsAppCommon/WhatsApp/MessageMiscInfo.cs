// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageMiscInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;
using System.Windows;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  [Table]
  [Index(Columns = "MessageId", IsUnique = true)]
  [Index(Columns = "BackgroundId")]
  public class MessageMiscInfo : PropChangingChangedBase
  {
    private int? messageId_;
    private int? errorCode_;
    private string backgroundId_;
    private string targetFilename;
    private string alternateUploadUri;
    private string broadcastJid_;
    private int expectedDeliveryCount_;
    private double? pictureWidthHeightRatio_;
    private byte[] imageBinaryInfo_;
    private int cipherRetryCount;
    private byte[] cipherMediaHash;
    private byte[] transcoderData_;
    private int clientRetryCount;

    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    public int ID { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? MessageId
    {
      get => this.messageId_;
      set
      {
        int? messageId = this.messageId_;
        int? nullable = value;
        if ((messageId.GetValueOrDefault() == nullable.GetValueOrDefault() ? (messageId.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (MessageId));
        this.messageId_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int? ErrorCode
    {
      get => this.errorCode_;
      set
      {
        int? errorCode = this.errorCode_;
        int? nullable = value;
        if ((errorCode.GetValueOrDefault() == nullable.GetValueOrDefault() ? (errorCode.HasValue != nullable.HasValue ? 1 : 0) : 1) == 0)
          return;
        this.NotifyPropertyChanging(nameof (ErrorCode));
        this.errorCode_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string BackgroundId
    {
      get => this.backgroundId_;
      set
      {
        if (!(this.backgroundId_ != value))
          return;
        this.NotifyPropertyChanging(nameof (BackgroundId));
        this.backgroundId_ = value;
      }
    }

    [Column(DbType = "image", UpdateCheck = UpdateCheck.Never)]
    [Sensitive]
    [Deprecated]
    public byte[] LargeThumbnailData { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string TargetFilename
    {
      get => this.targetFilename;
      set
      {
        if (!(this.targetFilename != value))
          return;
        this.NotifyPropertyChanging(nameof (TargetFilename));
        this.targetFilename = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string AlternateUploadUri
    {
      get => this.alternateUploadUri;
      set
      {
        if (!(this.alternateUploadUri != value))
          return;
        this.NotifyPropertyChanging(nameof (AlternateUploadUri));
        this.alternateUploadUri = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string Recipients { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public string DeliveredRecipients { get; set; }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public string BroadcastJid
    {
      get => this.broadcastJid_;
      set
      {
        if (!(this.broadcastJid_ != value))
          return;
        this.NotifyPropertyChanging(nameof (BroadcastJid));
        this.broadcastJid_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int ExpectedDeliveryCount
    {
      get => this.expectedDeliveryCount_;
      set
      {
        if (this.expectedDeliveryCount_ == value)
          return;
        this.NotifyPropertyChanging(nameof (ExpectedDeliveryCount));
        this.expectedDeliveryCount_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    [Deprecated]
    public double? PictureWidthHeightRatio
    {
      get => this.pictureWidthHeightRatio_;
      set
      {
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] ImageBinaryInfo
    {
      get => this.imageBinaryInfo_;
      set
      {
        if (this.imageBinaryInfo_ == value)
          return;
        this.NotifyPropertyChanging(nameof (ImageBinaryInfo));
        this.imageBinaryInfo_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int CipherRetryCount
    {
      get => this.cipherRetryCount;
      set
      {
        if (this.cipherRetryCount == value)
          return;
        this.NotifyPropertyChanging(nameof (CipherRetryCount));
        this.cipherRetryCount = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] CipherMediaHash
    {
      get => this.cipherMediaHash;
      set
      {
        if (this.cipherMediaHash == value)
          return;
        this.NotifyPropertyChanging(nameof (CipherMediaHash));
        this.cipherMediaHash = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public byte[] TranscoderData
    {
      get => this.transcoderData_;
      set
      {
        if (this.transcoderData_ == value)
          return;
        this.NotifyPropertyChanging(nameof (TranscoderData));
        this.transcoderData_ = value;
      }
    }

    [Column(UpdateCheck = UpdateCheck.Never)]
    public int ClientRetryCount
    {
      get => this.clientRetryCount;
      set
      {
        if (this.clientRetryCount == value)
          return;
        this.NotifyPropertyChanging(nameof (ClientRetryCount));
        this.clientRetryCount = value;
      }
    }

    public MessageMiscInfo.ImageInfo GetImageInfo(out bool upgradeNeeded)
    {
      return MessageMiscInfo.ImageInfo.Parse(this.ImageBinaryInfo, out upgradeNeeded);
    }

    public void SetImageInfo(MessageMiscInfo.ImageInfo imgInfo)
    {
      this.ImageBinaryInfo = imgInfo == null ? (byte[]) null : imgInfo.ToBinary();
    }

    public MessageMiscInfo CreateCopy()
    {
      return new MessageMiscInfo()
      {
        AlternateUploadUri = this.AlternateUploadUri,
        BackgroundId = this.BackgroundId,
        BroadcastJid = this.BroadcastJid,
        ErrorCode = this.ErrorCode,
        TargetFilename = this.TargetFilename,
        ImageBinaryInfo = this.ImageBinaryInfo
      };
    }

    public enum MessageError
    {
      None,
      NotAuthorized,
      BadRequest,
      Forbidden,
      ItemNotFound,
      NotAllowed,
      NotAcceptable,
      ServerError,
      NotImplemented,
      FileGone,
      ParticipantsHashMismatch,
      NotAdmin,
    }

    public class ImageInfo
    {
      private const int BinaryFormatVersion = 0;

      public double PixelWidth { get; private set; }

      public double PixelHeight { get; private set; }

      public int ClockwiseRotation { get; set; }

      public System.Windows.Point? CropPoint { get; set; }

      public Size? CropSize { get; set; }

      public static MessageMiscInfo.ImageInfo Create(double pixelWidth, double pixelHeight)
      {
        return new MessageMiscInfo.ImageInfo()
        {
          PixelWidth = pixelWidth,
          PixelHeight = pixelHeight,
          ClockwiseRotation = 0
        };
      }

      public static MessageMiscInfo.ImageInfo Parse(byte[] imgBinaryInfo, out bool upgradeNeeded)
      {
        if (imgBinaryInfo == null || imgBinaryInfo.Length < 3)
        {
          upgradeNeeded = false;
          return (MessageMiscInfo.ImageInfo) null;
        }
        MessageMiscInfo.ImageInfo imageInfo = new MessageMiscInfo.ImageInfo();
        return !imageInfo.ParseImpl(imgBinaryInfo, out upgradeNeeded) ? (MessageMiscInfo.ImageInfo) null : imageInfo;
      }

      private bool ParseImpl(byte[] bytes, out bool upgradeNeeded)
      {
        bool impl = false;
        byte num1 = bytes[0];
        if (num1 == (byte) 0 && bytes.Length == 29)
        {
          BinaryData binaryData = new BinaryData(bytes);
          int offset1 = 1;
          this.PixelWidth = (double) binaryData.ReadInt32(offset1);
          int offset2 = offset1 + 4;
          this.PixelHeight = (double) binaryData.ReadInt32(offset2);
          int offset3 = offset2 + 4;
          this.ClockwiseRotation = binaryData.ReadInt32(offset3);
          int offset4 = offset3 + 4;
          int x = binaryData.ReadInt32(offset4);
          int offset5 = offset4 + 4;
          int y = binaryData.ReadInt32(offset5);
          int offset6 = offset5 + 4;
          if (x >= 0 && y >= 0)
          {
            int width = binaryData.ReadInt32(offset6);
            int offset7 = offset6 + 4;
            int height = binaryData.ReadInt32(offset7);
            int num2 = offset7 + 4;
            if (width > 0 && height > 0)
            {
              this.CropPoint = new System.Windows.Point?(new System.Windows.Point((double) x, (double) y));
              this.CropSize = new Size?(new Size((double) width, (double) height));
            }
          }
          impl = true;
        }
        upgradeNeeded = impl && num1 > (byte) 0;
        return impl;
      }

      public byte[] ToBinary()
      {
        BinaryData binaryData1 = new BinaryData();
        binaryData1.AppendByte((byte) 0);
        binaryData1.AppendInt32((int) this.PixelWidth);
        binaryData1.AppendInt32((int) this.PixelHeight);
        binaryData1.AppendInt32(this.ClockwiseRotation);
        if (this.CropPoint.HasValue && this.CropSize.HasValue)
        {
          BinaryData binaryData2 = binaryData1;
          System.Windows.Point? cropPoint = this.CropPoint;
          int x = (int) cropPoint.Value.X;
          binaryData2.AppendInt32(x);
          BinaryData binaryData3 = binaryData1;
          cropPoint = this.CropPoint;
          int y = (int) cropPoint.Value.Y;
          binaryData3.AppendInt32(y);
          binaryData1.AppendInt32((int) this.CropSize.Value.Width);
          binaryData1.AppendInt32((int) this.CropSize.Value.Height);
        }
        else
        {
          binaryData1.AppendInt32(-1);
          binaryData1.AppendInt32(-1);
          binaryData1.AppendInt32(0);
          binaryData1.AppendInt32(0);
        }
        return binaryData1.Get();
      }
    }
  }
}
