// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.SmbUpdateCertErrorCheckResult
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class SmbUpdateCertErrorCheckResult : WamEvent
  {
    public wam_enum_smb_update_cert_error_check_type? smbUpdateCertErrorCheckType;
    public wam_enum_smb_update_cert_error_check_source? smbUpdateCertErrorCheckSource;
    public wam_enum_smb_update_cert_error_check_stage? smbUpdateCertErrorCheckStage;
    public long? smbUpdateCertErrorCheckCode;
    public string smbUpdateCertErrorCheckContent;

    public void Reset()
    {
      this.smbUpdateCertErrorCheckType = new wam_enum_smb_update_cert_error_check_type?();
      this.smbUpdateCertErrorCheckSource = new wam_enum_smb_update_cert_error_check_source?();
      this.smbUpdateCertErrorCheckStage = new wam_enum_smb_update_cert_error_check_stage?();
      this.smbUpdateCertErrorCheckCode = new long?();
      this.smbUpdateCertErrorCheckContent = (string) null;
    }

    public override uint GetCode() => 1666;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_smb_update_cert_error_check_type>(this.smbUpdateCertErrorCheckType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_smb_update_cert_error_check_source>(this.smbUpdateCertErrorCheckSource));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_smb_update_cert_error_check_stage>(this.smbUpdateCertErrorCheckStage));
      Wam.MaybeSerializeField(4, this.smbUpdateCertErrorCheckCode);
      Wam.MaybeSerializeField(5, this.smbUpdateCertErrorCheckContent);
    }
  }
}
