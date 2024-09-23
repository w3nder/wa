// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.RouteSelection
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class RouteSelection : WamEvent
  {
    public string routeHostname;
    public string routeIp;
    public long? routeSelectionT;
    public long? finalAuthcheckT;
    public long? routeClassIndex;
    public long? ipAddressIndex;

    public void Reset()
    {
      this.routeHostname = (string) null;
      this.routeIp = (string) null;
      this.routeSelectionT = new long?();
      this.finalAuthcheckT = new long?();
      this.routeClassIndex = new long?();
      this.ipAddressIndex = new long?();
    }

    public override uint GetCode() => 1228;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.routeHostname);
      Wam.MaybeSerializeField(2, this.routeIp);
      Wam.MaybeSerializeField(3, this.routeSelectionT);
      Wam.MaybeSerializeField(4, this.finalAuthcheckT);
      Wam.MaybeSerializeField(5, this.routeClassIndex);
      Wam.MaybeSerializeField(6, this.ipAddressIndex);
    }
  }
}
