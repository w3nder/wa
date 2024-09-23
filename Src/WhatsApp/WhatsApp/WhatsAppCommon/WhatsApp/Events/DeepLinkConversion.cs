// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.DeepLinkConversion
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class DeepLinkConversion : WamEvent
  {
    public string deepLinkConversionSource;
    public string deepLinkConversionData;

    public void Reset()
    {
      this.deepLinkConversionSource = (string) null;
      this.deepLinkConversionData = (string) null;
    }

    public override uint GetCode() => 1432;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.deepLinkConversionSource);
      Wam.MaybeSerializeField(2, this.deepLinkConversionData);
    }
  }
}
