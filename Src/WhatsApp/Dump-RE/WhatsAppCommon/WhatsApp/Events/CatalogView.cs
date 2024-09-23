// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.CatalogView
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class CatalogView : WamEvent
  {
    public wam_enum_catalog_view_action? catalogViewAction;
    public string catalogSessionId;
    public string catalogReportReasonCode;
    public long? catalogIndex;
    public string productId;
    public string catalogOwnerJid;

    public void Reset()
    {
      this.catalogViewAction = new wam_enum_catalog_view_action?();
      this.catalogSessionId = (string) null;
      this.catalogReportReasonCode = (string) null;
      this.catalogIndex = new long?();
      this.productId = (string) null;
      this.catalogOwnerJid = (string) null;
    }

    public override uint GetCode() => 1630;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_catalog_view_action>(this.catalogViewAction));
      Wam.MaybeSerializeField(2, this.catalogSessionId);
      Wam.MaybeSerializeField(4, this.catalogReportReasonCode);
      Wam.MaybeSerializeField(3, this.catalogIndex);
      Wam.MaybeSerializeField(5, this.productId);
      Wam.MaybeSerializeField(6, this.catalogOwnerJid);
    }
  }
}
