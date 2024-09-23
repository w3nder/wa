// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ClientInsubFiltered
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class ClientInsubFiltered : WamEvent
  {
    public bool? cachedContact;

    public void Reset() => this.cachedContact = new bool?();

    public override uint GetCode() => 1260;

    public override void SerializeFields() => Wam.MaybeSerializeField(1, this.cachedContact);
  }
}
