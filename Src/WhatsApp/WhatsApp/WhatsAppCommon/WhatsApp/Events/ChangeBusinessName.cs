// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ChangeBusinessName
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ChangeBusinessName : WamEvent
  {
    public wam_enum_change_business_name_action? changeBusinessNameAction;
    public wam_enum_change_business_name_result? changeBusinessNameResult;
    public long? changeBusinessNameRetryCount;

    public void Reset()
    {
      this.changeBusinessNameAction = new wam_enum_change_business_name_action?();
      this.changeBusinessNameResult = new wam_enum_change_business_name_result?();
      this.changeBusinessNameRetryCount = new long?();
    }

    public override uint GetCode() => 1526;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_change_business_name_action>(this.changeBusinessNameAction));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_change_business_name_result>(this.changeBusinessNameResult));
      Wam.MaybeSerializeField(3, this.changeBusinessNameRetryCount);
    }
  }
}
