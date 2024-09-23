// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.UserActivity
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class UserActivity : WamEvent
  {
    public string userActivitySessionId;
    public long? userActivityStartTime;
    public long? userActivityBitmapLow;
    public long? userActivityBitmapHigh;
    public long? userActivityBitmapLen;
    public long? userActivitySessionSeq;
    public long? userActivitySessionCum;

    public void Reset()
    {
      this.userActivitySessionId = (string) null;
      this.userActivityStartTime = new long?();
      this.userActivityBitmapLow = new long?();
      this.userActivityBitmapHigh = new long?();
      this.userActivityBitmapLen = new long?();
      this.userActivitySessionSeq = new long?();
      this.userActivitySessionCum = new long?();
    }

    public override uint GetCode() => 1384;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.userActivitySessionId);
      Wam.MaybeSerializeField(2, this.userActivityStartTime);
      Wam.MaybeSerializeField(3, this.userActivityBitmapLow);
      Wam.MaybeSerializeField(4, this.userActivityBitmapHigh);
      Wam.MaybeSerializeField(5, this.userActivityBitmapLen);
      Wam.MaybeSerializeField(6, this.userActivitySessionSeq);
      Wam.MaybeSerializeField(7, this.userActivitySessionCum);
    }
  }
}
