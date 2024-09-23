// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.WpxMemUsage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class WpxMemUsage : WamEvent
  {
    public double? wpxMemUsageCurrent;
    public double? wpxMemUsagePeak;

    public void Reset()
    {
      this.wpxMemUsageCurrent = new double?();
      this.wpxMemUsagePeak = new double?();
    }

    public override uint GetCode() => 1108;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.wpxMemUsageCurrent);
      Wam.MaybeSerializeField(2, this.wpxMemUsagePeak);
    }
  }
}
