// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MessageReceive
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MessageReceive : WamEvent
  {
    public wam_enum_message_type? messageType;
    public wam_enum_media_type? messageMediaType;
    public long? numOfWebUrlsInTextMessage;
    public bool? messageIsInternational;
    public bool? messageIsOffline;
    public long? messageReceiveT0;
    public long? messageReceiveT1;

    public void Reset()
    {
      this.messageType = new wam_enum_message_type?();
      this.messageMediaType = new wam_enum_media_type?();
      this.numOfWebUrlsInTextMessage = new long?();
      this.messageIsInternational = new bool?();
      this.messageIsOffline = new bool?();
      this.messageReceiveT0 = new long?();
      this.messageReceiveT1 = new long?();
    }

    public override uint GetCode() => 450;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(3, this.numOfWebUrlsInTextMessage);
      Wam.MaybeSerializeField(4, this.messageIsInternational);
      Wam.MaybeSerializeField(5, this.messageIsOffline);
      Wam.MaybeSerializeField(6, this.messageReceiveT0);
      Wam.MaybeSerializeField(7, this.messageReceiveT1);
    }
  }
}
