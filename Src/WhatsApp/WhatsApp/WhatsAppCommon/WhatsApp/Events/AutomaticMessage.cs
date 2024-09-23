// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.AutomaticMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class AutomaticMessage : WamEvent
  {
    public wam_enum_automatic_message_action? automaticMessageAction;
    public wam_enum_automatic_message_send_source? source;
    public wam_enum_away_message_state_type? awayMessageSubSource;

    public void Reset()
    {
      this.automaticMessageAction = new wam_enum_automatic_message_action?();
      this.source = new wam_enum_automatic_message_send_source?();
      this.awayMessageSubSource = new wam_enum_away_message_state_type?();
    }

    public override uint GetCode() => 1520;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_automatic_message_action>(this.automaticMessageAction));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_automatic_message_send_source>(this.source));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_away_message_state_type>(this.awayMessageSubSource));
    }
  }
}
