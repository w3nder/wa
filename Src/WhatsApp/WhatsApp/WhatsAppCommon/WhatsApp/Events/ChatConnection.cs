// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ChatConnection
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ChatConnection : WamEvent
  {
    public wam_enum_chat_state? chatState;
    public long? chatSocketConnectT;
    public long? chatLoginT;
    public long? chatPostLoginT;
    public long? chatConnectedT;
    public long? chatPort;

    public void Reset()
    {
      this.chatState = new wam_enum_chat_state?();
      this.chatSocketConnectT = new long?();
      this.chatLoginT = new long?();
      this.chatPostLoginT = new long?();
      this.chatConnectedT = new long?();
      this.chatPort = new long?();
    }

    public override uint GetCode() => 496;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_chat_state>(this.chatState));
      Wam.MaybeSerializeField(2, this.chatSocketConnectT);
      Wam.MaybeSerializeField(3, this.chatLoginT);
      Wam.MaybeSerializeField(4, this.chatPostLoginT);
      Wam.MaybeSerializeField(5, this.chatConnectedT);
      Wam.MaybeSerializeField(6, this.chatPort);
    }
  }
}
