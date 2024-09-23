// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusPost
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class StatusPost : WamEvent
  {
    public long? statusSessionId;
    public wam_enum_media_type? mediaType;
    public long? retryCount;
    public wam_enum_status_post_result? statusPostResult;
    public wam_enum_status_post_origin? statusPostOrigin;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.mediaType = new wam_enum_media_type?();
      this.retryCount = new long?();
      this.statusPostResult = new wam_enum_status_post_result?();
      this.statusPostOrigin = new wam_enum_status_post_origin?();
    }

    public override uint GetCode() => 1176;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(5, this.retryCount);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_status_post_result>(this.statusPostResult));
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_status_post_origin>(this.statusPostOrigin));
    }
  }
}
