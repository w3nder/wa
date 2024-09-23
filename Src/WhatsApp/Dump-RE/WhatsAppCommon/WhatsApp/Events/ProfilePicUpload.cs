// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ProfilePicUpload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class ProfilePicUpload : WamEvent
  {
    public wam_enum_profile_pic_upload_result? profilePicUploadResult;
    public long? retryCount;
    public string mediaException;
    public long? profilePicUploadT;
    public long? profilePicTotalT;
    public double? profilePicSize;
    public wam_enum_profile_pic_upload_type? profilePicUploadType;

    public void Reset()
    {
      this.profilePicUploadResult = new wam_enum_profile_pic_upload_result?();
      this.retryCount = new long?();
      this.mediaException = (string) null;
      this.profilePicUploadT = new long?();
      this.profilePicTotalT = new long?();
      this.profilePicSize = new double?();
      this.profilePicUploadType = new wam_enum_profile_pic_upload_type?();
    }

    public override uint GetCode() => 468;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_profile_pic_upload_result>(this.profilePicUploadResult));
      Wam.MaybeSerializeField(2, this.retryCount);
      Wam.MaybeSerializeField(7, this.mediaException);
      Wam.MaybeSerializeField(3, this.profilePicUploadT);
      Wam.MaybeSerializeField(6, this.profilePicTotalT);
      Wam.MaybeSerializeField(4, this.profilePicSize);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_profile_pic_upload_type>(this.profilePicUploadType));
    }
  }
}
