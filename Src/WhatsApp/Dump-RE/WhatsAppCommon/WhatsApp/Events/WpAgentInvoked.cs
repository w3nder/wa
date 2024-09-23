// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.WpAgentInvoked
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class WpAgentInvoked : WamEvent
  {
    public wam_enum_wp_agent_type? wpAgentType;

    public void Reset() => this.wpAgentType = new wam_enum_wp_agent_type?();

    public override uint GetCode() => 1008;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_wp_agent_type>(this.wpAgentType));
    }
  }
}
