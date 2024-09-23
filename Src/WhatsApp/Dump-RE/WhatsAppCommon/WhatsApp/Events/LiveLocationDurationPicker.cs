// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationDurationPicker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class LiveLocationDurationPicker : WamEvent
  {
    public long? liveLocationDurationPickerSelectedDuration;
    public wam_enum_live_location_duration_picker_entry_point? liveLocationDurationPickerEntryPoint;

    public void Reset()
    {
      this.liveLocationDurationPickerSelectedDuration = new long?();
      this.liveLocationDurationPickerEntryPoint = new wam_enum_live_location_duration_picker_entry_point?();
    }

    public override uint GetCode() => 1390;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.liveLocationDurationPickerSelectedDuration);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_live_location_duration_picker_entry_point>(this.liveLocationDurationPickerEntryPoint));
    }
  }
}
