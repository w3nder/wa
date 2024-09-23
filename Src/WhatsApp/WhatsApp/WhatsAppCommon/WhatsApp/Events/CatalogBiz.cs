// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.CatalogBiz
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class CatalogBiz : WamEvent
  {
    public wam_enum_catalog_biz_action? catalogBizAction;
    public string productId;
    public string catalogSessionId;
    public string catalogAppealReason;
    public long? errorCode;
    public long? productCount;

    public void Reset()
    {
      this.catalogBizAction = new wam_enum_catalog_biz_action?();
      this.productId = (string) null;
      this.catalogSessionId = (string) null;
      this.catalogAppealReason = (string) null;
      this.errorCode = new long?();
      this.productCount = new long?();
    }

    public override uint GetCode() => 1722;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_catalog_biz_action>(this.catalogBizAction));
      Wam.MaybeSerializeField(2, this.productId);
      Wam.MaybeSerializeField(3, this.catalogSessionId);
      Wam.MaybeSerializeField(4, this.catalogAppealReason);
      Wam.MaybeSerializeField(5, this.errorCode);
      Wam.MaybeSerializeField(6, this.productCount);
    }
  }
}
