// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaUpload2
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaUpload2 : WamEvent
  {
    public wam_enum_media_type? overallMediaType;
    public wam_enum_overall_media_key_reuse_type? overallMediaKeyReuse;
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
    public wam_enum_media_upload_result_type? overallUploadResult;
    public wam_enum_overall_last_upload_retry_phase_type? overallLastUploadRetryPhase;
    public wam_enum_optimistic_flag_type? overallOptimisticFlag;
    public bool? overallIsManual;
    public long? overallUserVisibleT;
    public long? overallCumUserVisibleT;
    public long? overallTranscodeT;
    public bool? overallIsForward;
    public wam_enum_media_upload_mode_type? overallUploadMode;
    public long? resumeConnectT;
    public long? resumeNetworkT;
    public bool? resumeIsReuse;
    public long? resumeHttpCode;
    public long? uploadResumePoint;
    public long? uploadConnectT;
    public long? uploadNetworkT;
    public bool? uploadIsReuse;
    public long? uploadHttpCode;
    public bool? uploadIsStreaming;
    public double? uploadBytesTransferred;
    public long? finalizeConnectT;
    public long? finalizeNetworkT;
    public bool? finalizeIsReuse;
    public long? finalizeHttpCode;
    public string debugMediaIp;
    public string debugUrl;
    public string debugMediaException;

    public void Reset()
    {
      this.overallMediaType = new wam_enum_media_type?();
      this.overallMediaKeyReuse = new wam_enum_overall_media_key_reuse_type?();
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
      this.overallUploadResult = new wam_enum_media_upload_result_type?();
      this.overallLastUploadRetryPhase = new wam_enum_overall_last_upload_retry_phase_type?();
      this.overallOptimisticFlag = new wam_enum_optimistic_flag_type?();
      this.overallIsManual = new bool?();
      this.overallUserVisibleT = new long?();
      this.overallCumUserVisibleT = new long?();
      this.overallTranscodeT = new long?();
      this.overallIsForward = new bool?();
      this.overallUploadMode = new wam_enum_media_upload_mode_type?();
      this.resumeConnectT = new long?();
      this.resumeNetworkT = new long?();
      this.resumeIsReuse = new bool?();
      this.resumeHttpCode = new long?();
      this.uploadResumePoint = new long?();
      this.uploadConnectT = new long?();
      this.uploadNetworkT = new long?();
      this.uploadIsReuse = new bool?();
      this.uploadHttpCode = new long?();
      this.uploadIsStreaming = new bool?();
      this.uploadBytesTransferred = new double?();
      this.finalizeConnectT = new long?();
      this.finalizeNetworkT = new long?();
      this.finalizeIsReuse = new bool?();
      this.finalizeHttpCode = new long?();
      this.debugMediaIp = (string) null;
      this.debugUrl = (string) null;
      this.debugMediaException = (string) null;
    }

    public override uint GetCode() => 1588;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.overallMediaType));
      Wam.MaybeSerializeField(40, Wam.EnumToLong<wam_enum_overall_media_key_reuse_type>(this.overallMediaKeyReuse));
      Wam.MaybeSerializeField(3, this.overallRetryCount);
      Wam.MaybeSerializeField(4, this.overallAttemptCount);
      Wam.MaybeSerializeField(5, this.overallDomain);
      Wam.MaybeSerializeField(41, this.overallConnectionClass);
      Wam.MaybeSerializeField(6, this.overallMmsVersion);
      Wam.MaybeSerializeField(7, this.overallMediaSize);
      Wam.MaybeSerializeField(36, this.overallIsFinal);
      Wam.MaybeSerializeField(8, this.overallT);
      Wam.MaybeSerializeField(37, this.overallCumT);
      Wam.MaybeSerializeField(9, this.overallQueueT);
      Wam.MaybeSerializeField(10, this.overallConnBlockFetchT);
      Wam.MaybeSerializeField(35, Wam.EnumToLong<wam_enum_media_upload_result_type>(this.overallUploadResult));
      Wam.MaybeSerializeField(11, Wam.EnumToLong<wam_enum_overall_last_upload_retry_phase_type>(this.overallLastUploadRetryPhase));
      Wam.MaybeSerializeField(12, Wam.EnumToLong<wam_enum_optimistic_flag_type>(this.overallOptimisticFlag));
      Wam.MaybeSerializeField(13, this.overallIsManual);
      Wam.MaybeSerializeField(14, this.overallUserVisibleT);
      Wam.MaybeSerializeField(38, this.overallCumUserVisibleT);
      Wam.MaybeSerializeField(15, this.overallTranscodeT);
      Wam.MaybeSerializeField(16, this.overallIsForward);
      Wam.MaybeSerializeField(39, Wam.EnumToLong<wam_enum_media_upload_mode_type>(this.overallUploadMode));
      Wam.MaybeSerializeField(17, this.resumeConnectT);
      Wam.MaybeSerializeField(18, this.resumeNetworkT);
      Wam.MaybeSerializeField(19, this.resumeIsReuse);
      Wam.MaybeSerializeField(20, this.resumeHttpCode);
      Wam.MaybeSerializeField(21, this.uploadResumePoint);
      Wam.MaybeSerializeField(22, this.uploadConnectT);
      Wam.MaybeSerializeField(23, this.uploadNetworkT);
      Wam.MaybeSerializeField(24, this.uploadIsReuse);
      Wam.MaybeSerializeField(25, this.uploadHttpCode);
      Wam.MaybeSerializeField(26, this.uploadIsStreaming);
      Wam.MaybeSerializeField(27, this.uploadBytesTransferred);
      Wam.MaybeSerializeField(28, this.finalizeConnectT);
      Wam.MaybeSerializeField(29, this.finalizeNetworkT);
      Wam.MaybeSerializeField(30, this.finalizeIsReuse);
      Wam.MaybeSerializeField(31, this.finalizeHttpCode);
      Wam.MaybeSerializeField(32, this.debugMediaIp);
      Wam.MaybeSerializeField(33, this.debugUrl);
      Wam.MaybeSerializeField(34, this.debugMediaException);
    }
  }
}
