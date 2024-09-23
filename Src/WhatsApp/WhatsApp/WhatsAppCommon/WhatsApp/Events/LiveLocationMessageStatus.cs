// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationMessageStatus
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class LiveLocationMessageStatus : WamEvent
  {
    public long? liveLocationLocationFixT;
    public wam_enum_live_location_message_status_result? liveLocationMessageStatusResult;

    public void Reset()
    {
      this.liveLocationLocationFixT = new long?();
      this.liveLocationMessageStatusResult = new wam_enum_live_location_message_status_result?();
    }

    public override uint GetCode() => 1394;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.liveLocationLocationFixT);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_live_location_message_status_result>(this.liveLocationMessageStatusResult));
    }
  }
}
