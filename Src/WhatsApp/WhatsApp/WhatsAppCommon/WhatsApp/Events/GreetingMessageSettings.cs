// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.GreetingMessageSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class GreetingMessageSettings : WamEvent
  {
    public wam_enum_greeting_message_settings_action? greetingMessageSettingsAction;
    public bool? greetingMessageSettingsEnabled;
    public bool? greetingMessageSettingsContentsUsingDefault;
    public wam_enum_greeting_message_audience_type? greetingMessageSettingsAudience;
    public long? greetingMessageSettingsAudienceCount;

    public void Reset()
    {
      this.greetingMessageSettingsAction = new wam_enum_greeting_message_settings_action?();
      this.greetingMessageSettingsEnabled = new bool?();
      this.greetingMessageSettingsContentsUsingDefault = new bool?();
      this.greetingMessageSettingsAudience = new wam_enum_greeting_message_audience_type?();
      this.greetingMessageSettingsAudienceCount = new long?();
    }

    public override uint GetCode() => 1612;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_greeting_message_settings_action>(this.greetingMessageSettingsAction));
      Wam.MaybeSerializeField(2, this.greetingMessageSettingsEnabled);
      Wam.MaybeSerializeField(3, this.greetingMessageSettingsContentsUsingDefault);
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_greeting_message_audience_type>(this.greetingMessageSettingsAudience));
      Wam.MaybeSerializeField(5, this.greetingMessageSettingsAudienceCount);
    }
  }
}
