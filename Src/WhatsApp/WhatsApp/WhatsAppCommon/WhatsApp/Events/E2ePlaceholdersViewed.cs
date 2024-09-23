// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.E2ePlaceholdersViewed
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class E2ePlaceholdersViewed : WamEvent
  {
    public long? decryptionPlaceholderViews;

    public void Reset() => this.decryptionPlaceholderViews = new long?();

    public override uint GetCode() => 1136;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.decryptionPlaceholderViews);
    }
  }
}
