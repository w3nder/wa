// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Wp8Dropped
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class Wp8Dropped : WamEvent
  {
    public double? wp8SessionDropped;
    public double? wp8TotalDropped;
    public double? wp8TotalPctDropped;
    public double? wp8TotalPushes;

    public void Reset()
    {
      this.wp8SessionDropped = new double?();
      this.wp8TotalDropped = new double?();
      this.wp8TotalPctDropped = new double?();
      this.wp8TotalPushes = new double?();
    }

    public override uint GetCode() => 1106;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.wp8SessionDropped);
      Wam.MaybeSerializeField(2, this.wp8TotalDropped);
      Wam.MaybeSerializeField(3, this.wp8TotalPctDropped);
      Wam.MaybeSerializeField(4, this.wp8TotalPushes);
    }
  }
}
