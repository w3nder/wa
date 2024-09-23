// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.RecvRevokeMessage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class RecvRevokeMessage : WamEvent
  {
    public wam_enum_message_type? messageType;
    public wam_enum_media_type? messageMediaType;
    public long? revokeRecvDelay;
    public bool? revokeOutOfOrder;

    public void Reset()
    {
      this.messageType = new wam_enum_message_type?();
      this.messageMediaType = new wam_enum_media_type?();
      this.revokeRecvDelay = new long?();
      this.revokeOutOfOrder = new bool?();
    }

    public override uint GetCode() => 1350;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(3, this.revokeRecvDelay);
      Wam.MaybeSerializeField(5, this.revokeOutOfOrder);
    }
  }
}
