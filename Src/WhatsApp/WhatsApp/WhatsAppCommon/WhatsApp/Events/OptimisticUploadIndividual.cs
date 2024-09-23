// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.OptimisticUploadIndividual
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class OptimisticUploadIndividual : WamEvent
  {
    public wam_enum_opt_upload_end_state_type? optEndState;
    public long? optCancelTime;
    public wam_enum_media_picker_origin_type? optOrigin;
    public long? optUploadDelay;
    public double? optSuccessRate;
    public long? optTimeSinceUploadFinishedT;
    public long? optSizeDiff;

    public void Reset()
    {
      this.optEndState = new wam_enum_opt_upload_end_state_type?();
      this.optCancelTime = new long?();
      this.optOrigin = new wam_enum_media_picker_origin_type?();
      this.optUploadDelay = new long?();
      this.optSuccessRate = new double?();
      this.optTimeSinceUploadFinishedT = new long?();
      this.optSizeDiff = new long?();
    }

    public override uint GetCode() => 1488;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_opt_upload_end_state_type>(this.optEndState));
      Wam.MaybeSerializeField(2, this.optCancelTime);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_media_picker_origin_type>(this.optOrigin));
      Wam.MaybeSerializeField(4, this.optUploadDelay);
      Wam.MaybeSerializeField(5, this.optSuccessRate);
      Wam.MaybeSerializeField(6, this.optTimeSinceUploadFinishedT);
      Wam.MaybeSerializeField(7, this.optSizeDiff);
    }
  }
}
