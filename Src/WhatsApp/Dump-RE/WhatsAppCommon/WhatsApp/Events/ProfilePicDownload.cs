// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ProfilePicDownload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class ProfilePicDownload : WamEvent
  {
    public wam_enum_profile_pic_download_result? profilePicDownloadResult;
    public wam_enum_profile_pic_type? profilePicType;
    public long? profilePicDownloadT;
    public double? profilePicDownloadSize;

    public void Reset()
    {
      this.profilePicDownloadResult = new wam_enum_profile_pic_download_result?();
      this.profilePicType = new wam_enum_profile_pic_type?();
      this.profilePicDownloadT = new long?();
      this.profilePicDownloadSize = new double?();
    }

    public override uint GetCode() => 848;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_profile_pic_download_result>(this.profilePicDownloadResult));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_profile_pic_type>(this.profilePicType));
      Wam.MaybeSerializeField(3, this.profilePicDownloadT);
      Wam.MaybeSerializeField(4, this.profilePicDownloadSize);
    }
  }
}
