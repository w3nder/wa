// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ChatFilterEvent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ChatFilterEvent : WamEvent
  {
    public wam_enum_chat_filter_action_types? actionType;
    public long? sessionId;
    public wam_enum_chat_filter_types? filterType;

    public void Reset()
    {
      this.actionType = new wam_enum_chat_filter_action_types?();
      this.sessionId = new long?();
      this.filterType = new wam_enum_chat_filter_types?();
    }

    public override uint GetCode() => 1616;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_chat_filter_action_types>(this.actionType));
      Wam.MaybeSerializeField(3, this.sessionId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_chat_filter_types>(this.filterType));
    }
  }
}
