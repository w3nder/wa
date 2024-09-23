// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ViewBusinessProfile
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class ViewBusinessProfile : WamEvent
  {
    public wam_enum_view_business_profile_action? viewBusinessProfileAction;
    public string businessProfileJid;
    public wam_enum_website_source_type? websiteSource;

    public void Reset()
    {
      this.viewBusinessProfileAction = new wam_enum_view_business_profile_action?();
      this.businessProfileJid = (string) null;
      this.websiteSource = new wam_enum_website_source_type?();
    }

    public override uint GetCode() => 1522;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_view_business_profile_action>(this.viewBusinessProfileAction));
      Wam.MaybeSerializeField(3, this.businessProfileJid);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_website_source_type>(this.websiteSource));
    }
  }
}
