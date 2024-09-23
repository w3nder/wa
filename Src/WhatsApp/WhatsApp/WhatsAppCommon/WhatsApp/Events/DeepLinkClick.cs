// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.DeepLinkClick
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class DeepLinkClick : WamEvent
  {
    public bool? deepLinkHasText;
    public bool? deepLinkHasPhoneNumber;

    public void Reset()
    {
      this.deepLinkHasText = new bool?();
      this.deepLinkHasPhoneNumber = new bool?();
    }

    public override uint GetCode() => 1156;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.deepLinkHasText);
      Wam.MaybeSerializeField(2, this.deepLinkHasPhoneNumber);
    }
  }
}
