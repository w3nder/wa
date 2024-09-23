// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationSharingSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class LiveLocationSharingSession : WamEvent
  {
    public long? liveLocationSharingSessionT;
    public wam_enum_live_location_sharing_session_ended_reason? liveLocationSharingSessionEndedReason;

    public void Reset()
    {
      this.liveLocationSharingSessionT = new long?();
      this.liveLocationSharingSessionEndedReason = new wam_enum_live_location_sharing_session_ended_reason?();
    }

    public override uint GetCode() => 1392;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.liveLocationSharingSessionT);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_live_location_sharing_session_ended_reason>(this.liveLocationSharingSessionEndedReason));
    }
  }
}
