// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.OfflineMessages
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class OfflineMessages : WamEvent
  {
    public double? offlineMessageC;
    public double? offlineNotificationC;
    public long? offlineMessagesReceiveT;
    public long? offlineMessagesOldestMsgLoginDeltaT;

    public void Reset()
    {
      this.offlineMessageC = new double?();
      this.offlineNotificationC = new double?();
      this.offlineMessagesReceiveT = new long?();
      this.offlineMessagesOldestMsgLoginDeltaT = new long?();
    }

    public override uint GetCode() => 452;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.offlineMessageC);
      Wam.MaybeSerializeField(2, this.offlineNotificationC);
      Wam.MaybeSerializeField(3, this.offlineMessagesReceiveT);
      Wam.MaybeSerializeField(4, this.offlineMessagesOldestMsgLoginDeltaT);
    }
  }
}
