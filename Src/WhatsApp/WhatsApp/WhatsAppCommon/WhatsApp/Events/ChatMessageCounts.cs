// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ChatMessageCounts
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ChatMessageCounts : WamEvent
  {
    public long? startTime;
    public wam_enum_chat_type? chatTypeInd;
    public long? messagesSent;
    public long? messagesReceived;
    public bool? isAGroup;
    public bool? isAContact;

    public void Reset()
    {
      this.startTime = new long?();
      this.chatTypeInd = new wam_enum_chat_type?();
      this.messagesSent = new long?();
      this.messagesReceived = new long?();
      this.isAGroup = new bool?();
      this.isAContact = new bool?();
    }

    public override uint GetCode() => 1644;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(7, this.startTime);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_chat_type>(this.chatTypeInd));
      Wam.MaybeSerializeField(3, this.messagesSent);
      Wam.MaybeSerializeField(4, this.messagesReceived);
      Wam.MaybeSerializeField(5, this.isAGroup);
      Wam.MaybeSerializeField(6, this.isAContact);
    }
  }
}
