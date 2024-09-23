// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.VideoPlay
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class VideoPlay : WamEvent
  {
    public long? videoDuration;
    public double? videoSize;
    public long? videoPlayT;
    public long? videoAge;
    public wam_enum_video_play_type? videoPlayType;
    public long? videoInitialBufferingT;
    public wam_enum_video_play_result? videoPlayResult;
    public wam_enum_video_play_surface? videoPlaySurface;
    public wam_enum_video_play_origin? videoPlayOrigin;

    public void Reset()
    {
      this.videoDuration = new long?();
      this.videoSize = new double?();
      this.videoPlayT = new long?();
      this.videoAge = new long?();
      this.videoPlayType = new wam_enum_video_play_type?();
      this.videoInitialBufferingT = new long?();
      this.videoPlayResult = new wam_enum_video_play_result?();
      this.videoPlaySurface = new wam_enum_video_play_surface?();
      this.videoPlayOrigin = new wam_enum_video_play_origin?();
    }

    public override uint GetCode() => 1012;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.videoDuration);
      Wam.MaybeSerializeField(2, this.videoSize);
      Wam.MaybeSerializeField(3, this.videoPlayT);
      Wam.MaybeSerializeField(4, this.videoAge);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_video_play_type>(this.videoPlayType));
      Wam.MaybeSerializeField(6, this.videoInitialBufferingT);
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_video_play_result>(this.videoPlayResult));
      Wam.MaybeSerializeField(8, Wam.EnumToLong<wam_enum_video_play_surface>(this.videoPlaySurface));
      Wam.MaybeSerializeField(9, Wam.EnumToLong<wam_enum_video_play_origin>(this.videoPlayOrigin));
    }
  }
}
