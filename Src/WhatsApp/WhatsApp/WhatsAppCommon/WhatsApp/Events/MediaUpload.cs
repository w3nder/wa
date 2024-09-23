// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaUpload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MediaUpload : WamEvent
  {
    public wam_enum_media_type? mediaType;
    public wam_enum_media_upload_result_type? mediaUploadResult;
    public bool? messageIsForward;
    public long? retryCount;
    public long? attemptCount;
    public bool? mediaUsedCdn;
    public bool? transcoded;
    public string routeHostname;
    public string routeIp;
    public bool? isReuse;
    public long? uploadResumePoint;
    public long? mmsVersion;
    public long? httpResponseCode;
    public wam_enum_media_upload_mode_type? uploadMode;
    public long? mediaUploadT;
    public double? mediaSize;
    public wam_enum_e2e_media_encryption? e2eMediaEncryption;
    public wam_enum_optimistic_flag_type? optimisticFlag;
    public long? connectT;
    public long? routeSelectionDelayT;
    public long? userVisibleT;
    public long? resumeCheckT;
    public long? networkUploadT;
    public long? requestIqT;
    public long? uploadResponseWaitT;
    public bool? isStreamingUpload;
    public bool? isManual;
    public double? bytesTransferred;
    public string url;
    public wam_enum_network_stack_type? networkStack;
    public string mediaIp;
    public string mediaException;
    public long? routeResponseTtl;
    public bool? isLikeDocument;
    public long? queueTime;
    public long? concurrentUploads;
    public bool? allowsIpHints;
    public long? optUploadDelay;
    public bool? chatdResumeCheckEnabled;

    public void Reset()
    {
      this.mediaType = new wam_enum_media_type?();
      this.mediaUploadResult = new wam_enum_media_upload_result_type?();
      this.messageIsForward = new bool?();
      this.retryCount = new long?();
      this.attemptCount = new long?();
      this.mediaUsedCdn = new bool?();
      this.transcoded = new bool?();
      this.routeHostname = (string) null;
      this.routeIp = (string) null;
      this.isReuse = new bool?();
      this.uploadResumePoint = new long?();
      this.mmsVersion = new long?();
      this.httpResponseCode = new long?();
      this.uploadMode = new wam_enum_media_upload_mode_type?();
      this.mediaUploadT = new long?();
      this.mediaSize = new double?();
      this.e2eMediaEncryption = new wam_enum_e2e_media_encryption?();
      this.optimisticFlag = new wam_enum_optimistic_flag_type?();
      this.connectT = new long?();
      this.routeSelectionDelayT = new long?();
      this.userVisibleT = new long?();
      this.resumeCheckT = new long?();
      this.networkUploadT = new long?();
      this.requestIqT = new long?();
      this.uploadResponseWaitT = new long?();
      this.isStreamingUpload = new bool?();
      this.isManual = new bool?();
      this.bytesTransferred = new double?();
      this.url = (string) null;
      this.networkStack = new wam_enum_network_stack_type?();
      this.mediaIp = (string) null;
      this.mediaException = (string) null;
      this.routeResponseTtl = new long?();
      this.isLikeDocument = new bool?();
      this.queueTime = new long?();
      this.concurrentUploads = new long?();
      this.allowsIpHints = new bool?();
      this.optUploadDelay = new long?();
      this.chatdResumeCheckEnabled = new bool?();
    }

    public override uint GetCode() => 454;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_upload_result_type>(this.mediaUploadResult));
      Wam.MaybeSerializeField(3, this.messageIsForward);
      Wam.MaybeSerializeField(4, this.retryCount);
      Wam.MaybeSerializeField(33, this.attemptCount);
      Wam.MaybeSerializeField(10, this.mediaUsedCdn);
      Wam.MaybeSerializeField(5, this.transcoded);
      Wam.MaybeSerializeField(11, this.routeHostname);
      Wam.MaybeSerializeField(12, this.routeIp);
      Wam.MaybeSerializeField(13, this.isReuse);
      Wam.MaybeSerializeField(14, this.uploadResumePoint);
      Wam.MaybeSerializeField(21, this.mmsVersion);
      Wam.MaybeSerializeField(27, this.httpResponseCode);
      Wam.MaybeSerializeField(39, Wam.EnumToLong<wam_enum_media_upload_mode_type>(this.uploadMode));
      Wam.MaybeSerializeField(6, this.mediaUploadT);
      Wam.MaybeSerializeField(7, this.mediaSize);
      Wam.MaybeSerializeField(8, Wam.EnumToLong<wam_enum_e2e_media_encryption>(this.e2eMediaEncryption));
      Wam.MaybeSerializeField(9, Wam.EnumToLong<wam_enum_optimistic_flag_type>(this.optimisticFlag));
      Wam.MaybeSerializeField(15, this.connectT);
      Wam.MaybeSerializeField(16, this.routeSelectionDelayT);
      Wam.MaybeSerializeField(17, this.userVisibleT);
      Wam.MaybeSerializeField(18, this.resumeCheckT);
      Wam.MaybeSerializeField(19, this.networkUploadT);
      Wam.MaybeSerializeField(24, this.requestIqT);
      Wam.MaybeSerializeField(25, this.uploadResponseWaitT);
      Wam.MaybeSerializeField(20, this.isStreamingUpload);
      Wam.MaybeSerializeField(22, this.isManual);
      Wam.MaybeSerializeField(23, this.bytesTransferred);
      Wam.MaybeSerializeField(26, this.url);
      Wam.MaybeSerializeField(28, Wam.EnumToLong<wam_enum_network_stack_type>(this.networkStack));
      Wam.MaybeSerializeField(29, this.mediaIp);
      Wam.MaybeSerializeField(30, this.mediaException);
      Wam.MaybeSerializeField(31, this.routeResponseTtl);
      Wam.MaybeSerializeField(32, this.isLikeDocument);
      Wam.MaybeSerializeField(34, this.queueTime);
      Wam.MaybeSerializeField(35, this.concurrentUploads);
      Wam.MaybeSerializeField(36, this.allowsIpHints);
      Wam.MaybeSerializeField(37, this.optUploadDelay);
      Wam.MaybeSerializeField(38, this.chatdResumeCheckEnabled);
    }
  }
}
