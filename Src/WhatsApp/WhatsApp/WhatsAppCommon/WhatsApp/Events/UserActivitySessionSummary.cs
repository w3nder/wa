// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UserActivitySessionSummary
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class UserActivitySessionSummary : WamEvent
  {
    public long? userActivityStartTime;
    public long? userActivityDuration;
    public long? userActivitySessionsLength;
    public long? userActivityTimeChange;
    public long? userActivityForeground;
    public long? userSessionSummarySequence;

    public void Reset()
    {
      this.userActivityStartTime = new long?();
      this.userActivityDuration = new long?();
      this.userActivitySessionsLength = new long?();
      this.userActivityTimeChange = new long?();
      this.userActivityForeground = new long?();
      this.userSessionSummarySequence = new long?();
    }

    public override uint GetCode() => 1502;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.userActivityStartTime);
      Wam.MaybeSerializeField(2, this.userActivityDuration);
      Wam.MaybeSerializeField(3, this.userActivitySessionsLength);
      Wam.MaybeSerializeField(4, this.userActivityTimeChange);
      Wam.MaybeSerializeField(5, this.userActivityForeground);
      Wam.MaybeSerializeField(6, this.userSessionSummarySequence);
    }
  }
}
