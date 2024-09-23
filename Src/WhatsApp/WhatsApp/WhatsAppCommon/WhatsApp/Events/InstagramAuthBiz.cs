// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.InstagramAuthBiz
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class InstagramAuthBiz : WamEvent
  {
    public wam_enum_instagram_link_action? instagramLinkAction;

    public void Reset() => this.instagramLinkAction = new wam_enum_instagram_link_action?();

    public override uint GetCode() => 1732;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_instagram_link_action>(this.instagramLinkAction));
    }
  }
}
