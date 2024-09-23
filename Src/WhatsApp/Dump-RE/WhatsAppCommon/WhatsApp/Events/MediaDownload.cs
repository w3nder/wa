// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaDownload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaDownload : WamEvent
  {
    public wam_enum_media_type? mediaType;
    public wam_enum_media_download_result_type? mediaDownloadResult;
    public long? retryCount;
    public long? attemptCount;
    public bool? mediaUsedCdn;
    public double? bytesTransferred;
    public bool? isStreamingMedia;
    public string routeHostname;
    public string routeIp;
    public bool? isReuse;
    public long? downloadResumePoint;
    public long? mmsVersion;
    public long? mediaDownloadT;
    public double? mediaSize;
    public wam_enum_e2e_media_encryption? e2eMediaEncryption;
    public long? connectT;
    public long? routeSelectionDelayT;
    public long? networkDownloadT;
    public long? fileValidationT;
    public long? timeToFirstByteT;
    public wam_enum_media_download_mode_type? mediaDownloadMode;
    public wam_enum_network_stack_type? networkStack;
    public string mediaIp;
    public string mediaException;
    public long? httpResponseCode;
    public long? concurrentDownloads;
    public bool? isStatus;
    public long? queueTime;
    public bool? routeRequestBypass;
    public long? routeResponseTtl;
    public bool? optPrewarmEnabled;
    public bool? allowsIpHints;

    public void Reset()
    {
      this.mediaType = new wam_enum_media_type?();
      this.mediaDownloadResult = new wam_enum_media_download_result_type?();
      this.retryCount = new long?();
      this.attemptCount = new long?();
      this.mediaUsedCdn = new bool?();
      this.bytesTransferred = new double?();
      this.isStreamingMedia = new bool?();
      this.routeHostname = (string) null;
      this.routeIp = (string) null;
      this.isReuse = new bool?();
      this.downloadResumePoint = new long?();
      this.mmsVersion = new long?();
      this.mediaDownloadT = new long?();
      this.mediaSize = new double?();
      this.e2eMediaEncryption = new wam_enum_e2e_media_encryption?();
      this.connectT = new long?();
      this.routeSelectionDelayT = new long?();
      this.networkDownloadT = new long?();
      this.fileValidationT = new long?();
      this.timeToFirstByteT = new long?();
      this.mediaDownloadMode = new wam_enum_media_download_mode_type?();
      this.networkStack = new wam_enum_network_stack_type?();
      this.mediaIp = (string) null;
      this.mediaException = (string) null;
      this.httpResponseCode = new long?();
      this.concurrentDownloads = new long?();
      this.isStatus = new bool?();
      this.queueTime = new long?();
      this.routeRequestBypass = new bool?();
      this.routeResponseTtl = new long?();
      this.optPrewarmEnabled = new bool?();
      this.allowsIpHints = new bool?();
    }

    public override uint GetCode() => 456;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_download_result_type>(this.mediaDownloadResult));
      Wam.MaybeSerializeField(3, this.retryCount);
      Wam.MaybeSerializeField(30, this.attemptCount);
      Wam.MaybeSerializeField(4, this.mediaUsedCdn);
      Wam.MaybeSerializeField(17, this.bytesTransferred);
      Wam.MaybeSerializeField(8, this.isStreamingMedia);
      Wam.MaybeSerializeField(9, this.routeHostname);
      Wam.MaybeSerializeField(10, this.routeIp);
      Wam.MaybeSerializeField(11, this.isReuse);
      Wam.MaybeSerializeField(12, this.downloadResumePoint);
      Wam.MaybeSerializeField(16, this.mmsVersion);
      Wam.MaybeSerializeField(5, this.mediaDownloadT);
      Wam.MaybeSerializeField(6, this.mediaSize);
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_e2e_media_encryption>(this.e2eMediaEncryption));
      Wam.MaybeSerializeField(13, this.connectT);
      Wam.MaybeSerializeField(14, this.routeSelectionDelayT);
      Wam.MaybeSerializeField(15, this.networkDownloadT);
      Wam.MaybeSerializeField(18, this.fileValidationT);
      Wam.MaybeSerializeField(19, this.timeToFirstByteT);
      Wam.MaybeSerializeField(20, Wam.EnumToLong<wam_enum_media_download_mode_type>(this.mediaDownloadMode));
      Wam.MaybeSerializeField(21, Wam.EnumToLong<wam_enum_network_stack_type>(this.networkStack));
      Wam.MaybeSerializeField(22, this.mediaIp);
      Wam.MaybeSerializeField(23, this.mediaException);
      Wam.MaybeSerializeField(24, this.httpResponseCode);
      Wam.MaybeSerializeField(25, this.concurrentDownloads);
      Wam.MaybeSerializeField(26, this.isStatus);
      Wam.MaybeSerializeField(27, this.queueTime);
      Wam.MaybeSerializeField(28, this.routeRequestBypass);
      Wam.MaybeSerializeField(29, this.routeResponseTtl);
      Wam.MaybeSerializeField(31, this.optPrewarmEnabled);
      Wam.MaybeSerializeField(32, this.allowsIpHints);
    }
  }
}
