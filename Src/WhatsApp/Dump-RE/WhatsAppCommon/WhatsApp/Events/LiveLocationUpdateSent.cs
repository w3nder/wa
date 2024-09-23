// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.LiveLocationUpdateSent
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class LiveLocationUpdateSent : WamEvent
  {
    public long? accuracyMeters;
    public double? timeSinceLastUpdate;
    public wam_enum_status? status;
    public wam_enum_live_location_reporting_algorithm? algorithm;
    public wam_enum_live_location_backoff_stage? backoffStage;
    public long? numberOfSenderKeyDistributionMessages;

    public void Reset()
    {
      this.accuracyMeters = new long?();
      this.timeSinceLastUpdate = new double?();
      this.status = new wam_enum_status?();
      this.algorithm = new wam_enum_live_location_reporting_algorithm?();
      this.backoffStage = new wam_enum_live_location_backoff_stage?();
      this.numberOfSenderKeyDistributionMessages = new long?();
    }

    public override uint GetCode() => 1320;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.accuracyMeters);
      Wam.MaybeSerializeField(2, this.timeSinceLastUpdate);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_status>(this.status));
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_live_location_reporting_algorithm>(this.algorithm));
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_live_location_backoff_stage>(this.backoffStage));
      Wam.MaybeSerializeField(6, this.numberOfSenderKeyDistributionMessages);
    }
  }
}
