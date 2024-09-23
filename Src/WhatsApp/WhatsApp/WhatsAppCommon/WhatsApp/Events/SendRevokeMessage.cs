// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.SendRevokeMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class SendRevokeMessage : WamEvent
  {
    public wam_enum_message_type? messageType;
    public wam_enum_media_type? messageMediaType;
    public long? revokeSendDelay;

    public void Reset()
    {
      this.messageType = new wam_enum_message_type?();
      this.messageMediaType = new wam_enum_media_type?();
      this.revokeSendDelay = new long?();
    }

    public override uint GetCode() => 1348;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(3, this.revokeSendDelay);
    }
  }
}
