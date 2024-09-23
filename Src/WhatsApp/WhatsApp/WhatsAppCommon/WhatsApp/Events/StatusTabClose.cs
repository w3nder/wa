// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusTabClose
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class StatusTabClose : WamEvent
  {
    public long? statusSessionId;
    public long? statusSessionTimeSpent;
    public long? statusSessionViewCount;
    public long? statusSessionReplyCount;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.statusSessionTimeSpent = new long?();
      this.statusSessionViewCount = new long?();
      this.statusSessionReplyCount = new long?();
    }

    public override uint GetCode() => 1174;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, this.statusSessionTimeSpent);
      Wam.MaybeSerializeField(3, this.statusSessionViewCount);
      Wam.MaybeSerializeField(4, this.statusSessionReplyCount);
    }
  }
}
