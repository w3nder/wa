// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationReportingSessionSegment
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class LiveLocationReportingSessionSegment : WamEvent
  {
    public long? segmentT;
    public long? segmentNumber;
    public long? segmentNumberOfUpdates;
    public wam_enum_live_location_reporting_algorithm? segmentAlgorithm;
    public double? segmentBatteryLevelChange;
    public bool? segmentBatteryCharging;
    public wam_enum_live_location_backoff_stage? segmentBackoffStage;

    public void Reset()
    {
      this.segmentT = new long?();
      this.segmentNumber = new long?();
      this.segmentNumberOfUpdates = new long?();
      this.segmentAlgorithm = new wam_enum_live_location_reporting_algorithm?();
      this.segmentBatteryLevelChange = new double?();
      this.segmentBatteryCharging = new bool?();
      this.segmentBackoffStage = new wam_enum_live_location_backoff_stage?();
    }

    public override uint GetCode() => 1324;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.segmentT);
      Wam.MaybeSerializeField(2, this.segmentNumber);
      Wam.MaybeSerializeField(3, this.segmentNumberOfUpdates);
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_live_location_reporting_algorithm>(this.segmentAlgorithm));
      Wam.MaybeSerializeField(5, this.segmentBatteryLevelChange);
      Wam.MaybeSerializeField(6, this.segmentBatteryCharging);
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_live_location_backoff_stage>(this.segmentBackoffStage));
    }
  }
}
