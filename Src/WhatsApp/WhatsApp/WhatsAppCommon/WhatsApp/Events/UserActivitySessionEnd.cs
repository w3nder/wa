// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UserActivitySessionEnd
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class UserActivitySessionEnd : WamEvent
  {
    public long? userActivityStartTime;
    public long? userActivitySessionLength;

    public void Reset()
    {
      this.userActivityStartTime = new long?();
      this.userActivitySessionLength = new long?();
    }

    public override uint GetCode() => 1426;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.userActivityStartTime);
      Wam.MaybeSerializeField(2, this.userActivitySessionLength);
    }
  }
}
