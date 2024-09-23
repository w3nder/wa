// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Mp4Repair
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class Mp4Repair : WamEvent
  {
    public long? oldMajorVersion;
    public long? oldMinorVersion;
    public long? oldReleaseVersion;
    public wam_enum_mp4_repair_originator? oldOriginator;
    public long? newMajorVersion;
    public long? newMinorVersion;
    public long? newReleaseVersion;
    public wam_enum_mp4_repair_originator? newOriginator;
    public bool? repairRequired;
    public bool? repairSuccessful;

    public void Reset()
    {
      this.oldMajorVersion = new long?();
      this.oldMinorVersion = new long?();
      this.oldReleaseVersion = new long?();
      this.oldOriginator = new wam_enum_mp4_repair_originator?();
      this.newMajorVersion = new long?();
      this.newMinorVersion = new long?();
      this.newReleaseVersion = new long?();
      this.newOriginator = new wam_enum_mp4_repair_originator?();
      this.repairRequired = new bool?();
      this.repairSuccessful = new bool?();
    }

    public override uint GetCode() => 1066;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.oldMajorVersion);
      Wam.MaybeSerializeField(2, this.oldMinorVersion);
      Wam.MaybeSerializeField(3, this.oldReleaseVersion);
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_mp4_repair_originator>(this.oldOriginator));
      Wam.MaybeSerializeField(5, this.newMajorVersion);
      Wam.MaybeSerializeField(6, this.newMinorVersion);
      Wam.MaybeSerializeField(7, this.newReleaseVersion);
      Wam.MaybeSerializeField(8, Wam.EnumToLong<wam_enum_mp4_repair_originator>(this.newOriginator));
      Wam.MaybeSerializeField(9, this.repairRequired);
      Wam.MaybeSerializeField(10, this.repairSuccessful);
    }
  }
}
