// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.AndroidVideoStreamPlayback
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class AndroidVideoStreamPlayback : WamEvent
  {
    public wam_enum_media_type? mediaType;
    public wam_enum_media_download_result_type? mediaDownloadResult;
    public double? bytesTransferred;
    public bool? mediaUsedCdn;
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
    public double? downloadedBytesAtStart;
    public double? downloadedBytesAtPlayback;
    public double? downloadedBytesAtFinish;
    public wam_enum_media_download_stage_type? downloadStageAtStart;
    public wam_enum_media_download_stage_type? downloadStageAtPlayback;
    public wam_enum_media_download_stage_type? downloadStageAtEnd;
    public long? timeSinceDownloadStartT;
    public long? initialStreamBufferingT;
    public long? timeSincePreviousFetchT;
    public long? playbackErrorCount;
    public bool? playbackErrorFatal;
    public bool? wasPlaybackReadyToPlay;
    public bool? reachedPlaybackEnd;
    public long? videoPauseT;
    public long? totalRebufferingT;
    public long? totalRebufferingCount;
    public wam_enum_playback_state_type? playbackState;
    public long? userPlaybackExitCount;
    public long? videoDuration;
    public double? videoSize;
    public long? videoPlayT;
    public long? videoAge;

    public void Reset()
    {
      this.mediaType = new wam_enum_media_type?();
      this.mediaDownloadResult = new wam_enum_media_download_result_type?();
      this.bytesTransferred = new double?();
      this.mediaUsedCdn = new bool?();
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
      this.downloadedBytesAtStart = new double?();
      this.downloadedBytesAtPlayback = new double?();
      this.downloadedBytesAtFinish = new double?();
      this.downloadStageAtStart = new wam_enum_media_download_stage_type?();
      this.downloadStageAtPlayback = new wam_enum_media_download_stage_type?();
      this.downloadStageAtEnd = new wam_enum_media_download_stage_type?();
      this.timeSinceDownloadStartT = new long?();
      this.initialStreamBufferingT = new long?();
      this.timeSincePreviousFetchT = new long?();
      this.playbackErrorCount = new long?();
      this.playbackErrorFatal = new bool?();
      this.wasPlaybackReadyToPlay = new bool?();
      this.reachedPlaybackEnd = new bool?();
      this.videoPauseT = new long?();
      this.totalRebufferingT = new long?();
      this.totalRebufferingCount = new long?();
      this.playbackState = new wam_enum_playback_state_type?();
      this.userPlaybackExitCount = new long?();
      this.videoDuration = new long?();
      this.videoSize = new double?();
      this.videoPlayT = new long?();
      this.videoAge = new long?();
    }

    public override uint GetCode() => 1298;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_download_result_type>(this.mediaDownloadResult));
      Wam.MaybeSerializeField(3, this.bytesTransferred);
      Wam.MaybeSerializeField(4, this.mediaUsedCdn);
      Wam.MaybeSerializeField(5, this.routeHostname);
      Wam.MaybeSerializeField(6, this.routeIp);
      Wam.MaybeSerializeField(7, this.isReuse);
      Wam.MaybeSerializeField(8, this.downloadResumePoint);
      Wam.MaybeSerializeField(9, this.mmsVersion);
      Wam.MaybeSerializeField(10, this.mediaDownloadT);
      Wam.MaybeSerializeField(11, this.mediaSize);
      Wam.MaybeSerializeField(12, Wam.EnumToLong<wam_enum_e2e_media_encryption>(this.e2eMediaEncryption));
      Wam.MaybeSerializeField(13, this.connectT);
      Wam.MaybeSerializeField(14, this.routeSelectionDelayT);
      Wam.MaybeSerializeField(15, this.networkDownloadT);
      Wam.MaybeSerializeField(16, this.fileValidationT);
      Wam.MaybeSerializeField(17, this.timeToFirstByteT);
      Wam.MaybeSerializeField(18, Wam.EnumToLong<wam_enum_media_download_mode_type>(this.mediaDownloadMode));
      Wam.MaybeSerializeField(19, this.downloadedBytesAtStart);
      Wam.MaybeSerializeField(20, this.downloadedBytesAtPlayback);
      Wam.MaybeSerializeField(21, this.downloadedBytesAtFinish);
      Wam.MaybeSerializeField(22, Wam.EnumToLong<wam_enum_media_download_stage_type>(this.downloadStageAtStart));
      Wam.MaybeSerializeField(23, Wam.EnumToLong<wam_enum_media_download_stage_type>(this.downloadStageAtPlayback));
      Wam.MaybeSerializeField(24, Wam.EnumToLong<wam_enum_media_download_stage_type>(this.downloadStageAtEnd));
      Wam.MaybeSerializeField(25, this.timeSinceDownloadStartT);
      Wam.MaybeSerializeField(26, this.initialStreamBufferingT);
      Wam.MaybeSerializeField(40, this.timeSincePreviousFetchT);
      Wam.MaybeSerializeField(31, this.playbackErrorCount);
      Wam.MaybeSerializeField(32, this.playbackErrorFatal);
      Wam.MaybeSerializeField(33, this.wasPlaybackReadyToPlay);
      Wam.MaybeSerializeField(34, this.reachedPlaybackEnd);
      Wam.MaybeSerializeField(39, this.videoPauseT);
      Wam.MaybeSerializeField(35, this.totalRebufferingT);
      Wam.MaybeSerializeField(36, this.totalRebufferingCount);
      Wam.MaybeSerializeField(37, Wam.EnumToLong<wam_enum_playback_state_type>(this.playbackState));
      Wam.MaybeSerializeField(38, this.userPlaybackExitCount);
      Wam.MaybeSerializeField(27, this.videoDuration);
      Wam.MaybeSerializeField(28, this.videoSize);
      Wam.MaybeSerializeField(29, this.videoPlayT);
      Wam.MaybeSerializeField(30, this.videoAge);
    }
  }
}
