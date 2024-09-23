// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.AppLaunch
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class AppLaunch : WamEvent
  {
    public long? appLaunchT;
    public long? appLaunchCpuT;
    public long? appLaunchMainPreT;
    public long? appLaunchMainRunT;
    public wam_enum_app_launch_type? appLaunchTypeT;
    public wam_enum_app_launch_destination_type? appLaunchDestination;

    public void Reset()
    {
      this.appLaunchT = new long?();
      this.appLaunchCpuT = new long?();
      this.appLaunchMainPreT = new long?();
      this.appLaunchMainRunT = new long?();
      this.appLaunchTypeT = new wam_enum_app_launch_type?();
      this.appLaunchDestination = new wam_enum_app_launch_destination_type?();
    }

    public override uint GetCode() => 1094;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.appLaunchT);
      Wam.MaybeSerializeField(2, this.appLaunchCpuT);
      Wam.MaybeSerializeField(3, this.appLaunchMainPreT);
      Wam.MaybeSerializeField(4, this.appLaunchMainRunT);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_app_launch_type>(this.appLaunchTypeT));
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_app_launch_destination_type>(this.appLaunchDestination));
    }
  }
}
