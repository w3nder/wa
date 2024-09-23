// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.QrScannerUsage
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class QrScannerUsage : WamEvent
  {
    public wam_enum_scan_type? scanType;
    public long? scanReadCount;
    public long? scanValidCount;
    public long? scanTimeT;

    public void Reset()
    {
      this.scanType = new wam_enum_scan_type?();
      this.scanReadCount = new long?();
      this.scanValidCount = new long?();
      this.scanTimeT = new long?();
    }

    public override uint GetCode() => 1606;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_scan_type>(this.scanType));
      Wam.MaybeSerializeField(2, this.scanReadCount);
      Wam.MaybeSerializeField(3, this.scanValidCount);
      Wam.MaybeSerializeField(4, this.scanTimeT);
    }
  }
}
