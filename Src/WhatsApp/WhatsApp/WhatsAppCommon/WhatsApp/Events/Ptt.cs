// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Ptt
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class Ptt : WamEvent
  {
    public wam_enum_ptt_result_type? pttResult;
    public wam_enum_ptt_source_type? pttSource;
    public double? pttSize;

    public void Reset()
    {
      this.pttResult = new wam_enum_ptt_result_type?();
      this.pttSource = new wam_enum_ptt_source_type?();
      this.pttSize = new double?();
    }

    public override uint GetCode() => 458;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_ptt_result_type>(this.pttResult));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_ptt_source_type>(this.pttSource));
      Wam.MaybeSerializeField(3, this.pttSize);
    }
  }
}
