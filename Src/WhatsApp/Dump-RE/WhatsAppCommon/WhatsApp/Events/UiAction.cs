// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UiAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class UiAction : WamEvent
  {
    public wam_enum_ui_action_type? uiActionType;
    public bool? uiActionPreloaded;
    public long? uiActionT;

    public void Reset()
    {
      this.uiActionType = new wam_enum_ui_action_type?();
      this.uiActionPreloaded = new bool?();
      this.uiActionT = new long?();
    }

    public override uint GetCode() => 472;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_ui_action_type>(this.uiActionType));
      Wam.MaybeSerializeField(2, this.uiActionPreloaded);
      Wam.MaybeSerializeField(3, this.uiActionT);
    }
  }
}
