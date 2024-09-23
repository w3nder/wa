// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationReportingSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class LiveLocationReportingSession : WamEvent
  {
    public long? sessionT;
    public long? numberOfUpdates;
    public double? batteryLevelChange;

    public void Reset()
    {
      this.sessionT = new long?();
      this.numberOfUpdates = new long?();
      this.batteryLevelChange = new double?();
    }

    public override uint GetCode() => 1322;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.sessionT);
      Wam.MaybeSerializeField(2, this.numberOfUpdates);
      Wam.MaybeSerializeField(3, this.batteryLevelChange);
    }
  }
}
