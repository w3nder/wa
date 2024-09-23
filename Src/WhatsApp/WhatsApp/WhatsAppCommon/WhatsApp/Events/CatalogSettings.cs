// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.CatalogSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class CatalogSettings : WamEvent
  {
    public wam_enum_catalog_settings_action? catalogSettingsAction;
    public bool? productHasTitle;
    public bool? productHasDescription;
    public bool? productHasLink;
    public bool? productHasSku;
    public long? productImageCount;
    public long? productVideoCount;
    public string catalogAppealReason;

    public void Reset()
    {
      this.catalogSettingsAction = new wam_enum_catalog_settings_action?();
      this.productHasTitle = new bool?();
      this.productHasDescription = new bool?();
      this.productHasLink = new bool?();
      this.productHasSku = new bool?();
      this.productImageCount = new long?();
      this.productVideoCount = new long?();
      this.catalogAppealReason = (string) null;
    }

    public override uint GetCode() => 1660;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_catalog_settings_action>(this.catalogSettingsAction));
      Wam.MaybeSerializeField(2, this.productHasTitle);
      Wam.MaybeSerializeField(3, this.productHasDescription);
      Wam.MaybeSerializeField(4, this.productHasLink);
      Wam.MaybeSerializeField(5, this.productHasSku);
      Wam.MaybeSerializeField(6, this.productImageCount);
      Wam.MaybeSerializeField(7, this.productVideoCount);
      Wam.MaybeSerializeField(8, this.catalogAppealReason);
    }
  }
}
