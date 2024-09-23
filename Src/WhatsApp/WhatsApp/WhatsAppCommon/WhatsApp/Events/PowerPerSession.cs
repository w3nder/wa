// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.PowerPerSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class PowerPerSession : WamEvent
  {
    public double? cpuEnergy;
    public double? netEnergy;
    public double? totalEnergy;
    public bool? appInBackground;
    public long? periodT;

    public void Reset()
    {
      this.cpuEnergy = new double?();
      this.netEnergy = new double?();
      this.totalEnergy = new double?();
      this.appInBackground = new bool?();
      this.periodT = new long?();
    }

    public override uint GetCode() => 1556;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.cpuEnergy);
      Wam.MaybeSerializeField(2, this.netEnergy);
      Wam.MaybeSerializeField(3, this.totalEnergy);
      Wam.MaybeSerializeField(4, this.appInBackground);
      Wam.MaybeSerializeField(5, this.periodT);
    }
  }
}
