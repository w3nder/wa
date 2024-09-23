// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusDaily
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class StatusDaily : WamEvent
  {
    public long? statusAvailableRowsCountDaily;
    public long? statusViewedRowsCountDaily;
    public long? statusAvailableCountDaily;
    public long? statusViewedCountDaily;

    public void Reset()
    {
      this.statusAvailableRowsCountDaily = new long?();
      this.statusViewedRowsCountDaily = new long?();
      this.statusAvailableCountDaily = new long?();
      this.statusViewedCountDaily = new long?();
    }

    public override uint GetCode() => 1676;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusAvailableRowsCountDaily);
      Wam.MaybeSerializeField(2, this.statusViewedRowsCountDaily);
      Wam.MaybeSerializeField(3, this.statusAvailableCountDaily);
      Wam.MaybeSerializeField(4, this.statusViewedCountDaily);
    }
  }
}
