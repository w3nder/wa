// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.CrashLog
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class CrashLog : WamEvent
  {
    public wam_enum_crash_type? crashType;
    public string crashReason;
    public string crashContext;
    public long? crashCount;

    public void Reset()
    {
      this.crashType = new wam_enum_crash_type?();
      this.crashReason = (string) null;
      this.crashContext = (string) null;
      this.crashCount = new long?();
    }

    public override uint GetCode() => 494;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(6, Wam.EnumToLong<wam_enum_crash_type>(this.crashType));
      Wam.MaybeSerializeField(2, this.crashReason);
      Wam.MaybeSerializeField(3, this.crashContext);
      Wam.MaybeSerializeField(5, this.crashCount);
    }
  }
}
