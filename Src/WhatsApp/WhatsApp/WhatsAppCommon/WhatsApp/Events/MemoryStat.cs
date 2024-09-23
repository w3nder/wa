// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MemoryStat
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MemoryStat : WamEvent
  {
    public double? workingSetSize;
    public double? workingSetPeakSize;
    public double? privateBytes;
    public double? sharedBytes;
    public string processType;
    public double? uptime;
    public bool? hasVerifiedNumber;
    public double? numMessages;

    public void Reset()
    {
      this.workingSetSize = new double?();
      this.workingSetPeakSize = new double?();
      this.privateBytes = new double?();
      this.sharedBytes = new double?();
      this.processType = (string) null;
      this.uptime = new double?();
      this.hasVerifiedNumber = new bool?();
      this.numMessages = new double?();
    }

    public override uint GetCode() => 1336;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.workingSetSize);
      Wam.MaybeSerializeField(2, this.workingSetPeakSize);
      Wam.MaybeSerializeField(3, this.privateBytes);
      Wam.MaybeSerializeField(4, this.sharedBytes);
      Wam.MaybeSerializeField(5, this.processType);
      Wam.MaybeSerializeField(6, this.uptime);
      Wam.MaybeSerializeField(7, this.hasVerifiedNumber);
      Wam.MaybeSerializeField(8, this.numMessages);
    }
  }
}
