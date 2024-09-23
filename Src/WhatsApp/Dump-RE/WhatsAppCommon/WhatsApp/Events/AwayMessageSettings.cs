// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.AwayMessageSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class AwayMessageSettings : WamEvent
  {
    public wam_enum_away_message_settings_action_type? awayMessageSettingsAction;
    public bool? awayMessageSettingsDefaultMessage;
    public wam_enum_away_message_audience_type? awayMessageSettingsAudience;
    public long? awayMessageSettingsAudienceCount;

    public void Reset()
    {
      this.awayMessageSettingsAction = new wam_enum_away_message_settings_action_type?();
      this.awayMessageSettingsDefaultMessage = new bool?();
      this.awayMessageSettingsAudience = new wam_enum_away_message_audience_type?();
      this.awayMessageSettingsAudienceCount = new long?();
    }

    public override uint GetCode() => 1604;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_away_message_settings_action_type>(this.awayMessageSettingsAction));
      Wam.MaybeSerializeField(2, this.awayMessageSettingsDefaultMessage);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_away_message_audience_type>(this.awayMessageSettingsAudience));
      Wam.MaybeSerializeField(4, this.awayMessageSettingsAudienceCount);
    }
  }
}
