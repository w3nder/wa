// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.BroadcastFanout
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class BroadcastFanout : WamEvent
  {
    public double? broadcastSentRecipientCount;
    public long? broadcastFanoutT;

    public void Reset()
    {
      this.broadcastSentRecipientCount = new double?();
      this.broadcastFanoutT = new long?();
    }

    public override uint GetCode() => 492;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.broadcastSentRecipientCount);
      Wam.MaybeSerializeField(2, this.broadcastFanoutT);
    }
  }
}
