// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.PushReceive
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class PushReceive : WamEvent
  {
    public bool? pushReceiveWhileOffline;
    public long? pushReceiveDelayT;
    public long? networkChangeDelayT;

    public void Reset()
    {
      this.pushReceiveWhileOffline = new bool?();
      this.pushReceiveDelayT = new long?();
      this.networkChangeDelayT = new long?();
    }

    public override uint GetCode() => 534;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.pushReceiveWhileOffline);
      Wam.MaybeSerializeField(2, this.pushReceiveDelayT);
      Wam.MaybeSerializeField(3, this.networkChangeDelayT);
    }
  }
}
