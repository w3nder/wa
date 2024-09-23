// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.BusinessUnmute
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class BusinessUnmute : WamEvent
  {
    public string muteeId;

    public void Reset() => this.muteeId = (string) null;

    public override uint GetCode() => 1378;

    public override void SerializeFields() => Wam.MaybeSerializeField(1, this.muteeId);
  }
}
