// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.GroupMute
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class GroupMute : WamEvent
  {
    public long? groupSize;
    public long? groupMuteT;

    public void Reset()
    {
      this.groupSize = new long?();
      this.groupMuteT = new long?();
    }

    public override uint GetCode() => 466;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.groupSize);
      Wam.MaybeSerializeField(2, this.groupMuteT);
    }
  }
}
