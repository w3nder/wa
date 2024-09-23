// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusReply
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class StatusReply : WamEvent
  {
    public long? statusSessionId;
    public wam_enum_status_reply_result? statusReplyResult;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.statusReplyResult = new wam_enum_status_reply_result?();
    }

    public override uint GetCode() => 1180;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_status_reply_result>(this.statusReplyResult));
    }
  }
}
