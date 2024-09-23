// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.SmbVnameCertHealth
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class SmbVnameCertHealth : WamEvent
  {
    public wam_enum_smb_vname_cert_health_result? smbVnameCertHealthResult;

    public void Reset()
    {
      this.smbVnameCertHealthResult = new wam_enum_smb_vname_cert_health_result?();
    }

    public override uint GetCode() => 1602;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_smb_vname_cert_health_result>(this.smbVnameCertHealthResult));
    }
  }
}
