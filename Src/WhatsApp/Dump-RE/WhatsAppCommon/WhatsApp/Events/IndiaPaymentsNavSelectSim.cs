// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.IndiaPaymentsNavSelectSim
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class IndiaPaymentsNavSelectSim : WamEvent
  {
    public string paymentsEventId;
    public bool? paymentsBackSelected;
    public bool? paymentsAppExitSelected;
    public long? simSlotSelected;

    public void Reset()
    {
      this.paymentsEventId = (string) null;
      this.paymentsBackSelected = new bool?();
      this.paymentsAppExitSelected = new bool?();
      this.simSlotSelected = new long?();
    }

    public override uint GetCode() => 1626;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.paymentsEventId);
      Wam.MaybeSerializeField(2, this.paymentsBackSelected);
      Wam.MaybeSerializeField(3, this.paymentsAppExitSelected);
      Wam.MaybeSerializeField(4, this.simSlotSelected);
    }
  }
}
