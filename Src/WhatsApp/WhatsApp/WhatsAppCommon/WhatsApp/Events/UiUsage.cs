// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UiUsage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class UiUsage : WamEvent
  {
    public wam_enum_ui_usage_type? uiUsageType;
    public wam_enum_entry_point? entryPoint;

    public void Reset()
    {
      this.uiUsageType = new wam_enum_ui_usage_type?();
      this.entryPoint = new wam_enum_entry_point?();
    }

    public override uint GetCode() => 474;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_ui_usage_type>(this.uiUsageType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_entry_point>(this.entryPoint));
    }
  }
}
