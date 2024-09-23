// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaStreamPlayback
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaStreamPlayback : WamEvent
  {
    public long? overallT;
    public double? mediaSize;
    public wam_enum_media_type? mediaType;
    public double? bytesDownloadedStart;
    public double? bytesTransferred;
    public long? videoDuration;
    public long? initialBufferingT;
    public long? totalRebufferingT;
    public long? totalRebufferingCount;
    public long? overallPlayT;
    public wam_enum_playback_state_type? playbackState;
    public long? forcedPlayCount;
    public long? seekCount;
    public long? playbackCount;
    public bool? didPlay;
    public wam_enum_playback_origin_type? playbackOrigin;

    public void Reset()
    {
      this.overallT = new long?();
      this.mediaSize = new double?();
      this.mediaType = new wam_enum_media_type?();
      this.bytesDownloadedStart = new double?();
      this.bytesTransferred = new double?();
      this.videoDuration = new long?();
      this.initialBufferingT = new long?();
      this.totalRebufferingT = new long?();
      this.totalRebufferingCount = new long?();
      this.overallPlayT = new long?();
      this.playbackState = new wam_enum_playback_state_type?();
      this.forcedPlayCount = new long?();
      this.seekCount = new long?();
      this.playbackCount = new long?();
      this.didPlay = new bool?();
      this.playbackOrigin = new wam_enum_playback_origin_type?();
    }

    public override uint GetCode() => 1584;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.overallT);
      Wam.MaybeSerializeField(2, this.mediaSize);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(4, this.bytesDownloadedStart);
      Wam.MaybeSerializeField(5, this.bytesTransferred);
      Wam.MaybeSerializeField(6, this.videoDuration);
      Wam.MaybeSerializeField(7, this.initialBufferingT);
      Wam.MaybeSerializeField(8, this.totalRebufferingT);
      Wam.MaybeSerializeField(9, this.totalRebufferingCount);
      Wam.MaybeSerializeField(10, this.overallPlayT);
      Wam.MaybeSerializeField(11, Wam.EnumToLong<wam_enum_playback_state_type>(this.playbackState));
      Wam.MaybeSerializeField(12, this.forcedPlayCount);
      Wam.MaybeSerializeField(13, this.seekCount);
      Wam.MaybeSerializeField(14, this.playbackCount);
      Wam.MaybeSerializeField(15, this.didPlay);
      Wam.MaybeSerializeField(16, Wam.EnumToLong<wam_enum_playback_origin_type>(this.playbackOrigin));
    }
  }
}
