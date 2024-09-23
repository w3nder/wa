// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusTabOpen
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class StatusTabOpen : WamEvent
  {
    public long? statusSessionId;
    public long? statusRankT;
    public long? statusAvailableUpdatesCount;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.statusRankT = new long?();
      this.statusAvailableUpdatesCount = new long?();
    }

    public override uint GetCode() => 1172;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(3, this.statusRankT);
      Wam.MaybeSerializeField(2, this.statusAvailableUpdatesCount);
    }
  }
}
