// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusViewPost
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class StatusViewPost : WamEvent
  {
    public long? statusSessionId;
    public wam_enum_media_type? mediaType;
    public wam_enum_status_view_post_result? statusViewPostResult;
    public wam_enum_status_view_post_origin? statusViewPostOrigin;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.mediaType = new wam_enum_media_type?();
      this.statusViewPostResult = new wam_enum_status_view_post_result?();
      this.statusViewPostOrigin = new wam_enum_status_view_post_origin?();
    }

    public override uint GetCode() => 1178;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_status_view_post_result>(this.statusViewPostResult));
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_status_view_post_origin>(this.statusViewPostOrigin));
    }
  }
}
