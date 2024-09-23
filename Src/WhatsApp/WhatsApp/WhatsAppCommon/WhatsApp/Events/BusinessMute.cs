// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.BusinessMute
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class BusinessMute : WamEvent
  {
    public string muteeId;
    public long? muteT;

    public void Reset()
    {
      this.muteeId = (string) null;
      this.muteT = new long?();
    }

    public override uint GetCode() => 1376;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.muteeId);
      Wam.MaybeSerializeField(2, this.muteT);
    }
  }
}
