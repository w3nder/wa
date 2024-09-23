// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaDownload2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MediaDownload2 : WamEvent
  {
    public wam_enum_media_type? overallMediaType;
    public long? overallRetryCount;
    public long? overallAttemptCount;
    public string overallDomain;
    public string overallConnectionClass;
    public long? overallMmsVersion;
    public double? overallMediaSize;
    public bool? overallIsFinal;
    public long? overallT;
    public long? overallCumT;
    public long? overallQueueT;
    public long? overallConnBlockFetchT;
    public wam_enum_media_download_result_type? overallDownloadResult;
    public wam_enum_media_download_mode_type? overallDownloadMode;
    public long? overallDecryptT;
    public long? overallFileValidationT;
    public bool? overallIsEncrypted;
    public long? downloadResumePoint;
    public long? downloadConnectT;
    public long? downloadNetworkT;
    public bool? downloadIsReuse;
    public long? downloadHttpCode;
    public bool? downloadIsStreaming;
    public double? downloadBytesTransferred;
    public long? downloadTimeToFirstByteT;
    public string debugMediaIp;
    public string debugUrl;
    public string debugMediaException;

    public void Reset()
    {
      this.overallMediaType = new wam_enum_media_type?();
      this.overallRetryCount = new long?();
      this.overallAttemptCount = new long?();
      this.overallDomain = (string) null;
      this.overallConnectionClass = (string) null;
      this.overallMmsVersion = new long?();
      this.overallMediaSize = new double?();
      this.overallIsFinal = new bool?();
      this.overallT = new long?();
      this.overallCumT = new long?();
      this.overallQueueT = new long?();
      this.overallConnBlockFetchT = new long?();
      this.overallDownloadResult = new wam_enum_media_download_result_type?();
      this.overallDownloadMode = new wam_enum_media_download_mode_type?();
      this.overallDecryptT = new long?();
      this.overallFileValidationT = new long?();
      this.overallIsEncrypted = new bool?();
      this.downloadResumePoint = new long?();
      this.downloadConnectT = new long?();
      this.downloadNetworkT = new long?();
      this.downloadIsReuse = new bool?();
      this.downloadHttpCode = new long?();
      this.downloadIsStreaming = new bool?();
      this.downloadBytesTransferred = new double?();
      this.downloadTimeToFirstByteT = new long?();
      this.debugMediaIp = (string) null;
      this.debugUrl = (string) null;
      this.debugMediaException = (string) null;
    }

    public override uint GetCode() => 1590;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.overallMediaType));
      Wam.MaybeSerializeField(3, this.overallRetryCount);
      Wam.MaybeSerializeField(4, this.overallAttemptCount);
      Wam.MaybeSerializeField(5, this.overallDomain);
      Wam.MaybeSerializeField(29, this.overallConnectionClass);
      Wam.MaybeSerializeField(6, this.overallMmsVersion);
      Wam.MaybeSerializeField(7, this.overallMediaSize);
      Wam.MaybeSerializeField(26, this.overallIsFinal);
      Wam.MaybeSerializeField(8, this.overallT);
      Wam.MaybeSerializeField(27, this.overallCumT);
      Wam.MaybeSerializeField(9, this.overallQueueT);
      Wam.MaybeSerializeField(10, this.overallConnBlockFetchT);
      Wam.MaybeSerializeField(25, Wam.EnumToLong<wam_enum_media_download_result_type>(this.overallDownloadResult));
      Wam.MaybeSerializeField(11, Wam.EnumToLong<wam_enum_media_download_mode_type>(this.overallDownloadMode));
      Wam.MaybeSerializeField(12, this.overallDecryptT);
      Wam.MaybeSerializeField(13, this.overallFileValidationT);
      Wam.MaybeSerializeField(28, this.overallIsEncrypted);
      Wam.MaybeSerializeField(14, this.downloadResumePoint);
      Wam.MaybeSerializeField(15, this.downloadConnectT);
      Wam.MaybeSerializeField(16, this.downloadNetworkT);
      Wam.MaybeSerializeField(17, this.downloadIsReuse);
      Wam.MaybeSerializeField(18, this.downloadHttpCode);
      Wam.MaybeSerializeField(19, this.downloadIsStreaming);
      Wam.MaybeSerializeField(20, this.downloadBytesTransferred);
      Wam.MaybeSerializeField(21, this.downloadTimeToFirstByteT);
      Wam.MaybeSerializeField(22, this.debugMediaIp);
      Wam.MaybeSerializeField(23, this.debugUrl);
      Wam.MaybeSerializeField(24, this.debugMediaException);
    }
  }
}
