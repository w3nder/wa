// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.EditBusinessProfile
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class EditBusinessProfile : WamEvent
  {
    public wam_enum_edit_profile_action? editProfileAction;
    public string editBusinessProfileSessionId;
    public bool? hasDescription;
    public bool? hasCategory;
    public bool? hasAddress;
    public bool? hasHours;
    public bool? hasEmail;
    public bool? hasWebsite;

    public void Reset()
    {
      this.editProfileAction = new wam_enum_edit_profile_action?();
      this.editBusinessProfileSessionId = (string) null;
      this.hasDescription = new bool?();
      this.hasCategory = new bool?();
      this.hasAddress = new bool?();
      this.hasHours = new bool?();
      this.hasEmail = new bool?();
      this.hasWebsite = new bool?();
    }

    public override uint GetCode() => 1466;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_edit_profile_action>(this.editProfileAction));
      Wam.MaybeSerializeField(2, this.editBusinessProfileSessionId);
      Wam.MaybeSerializeField(3, this.hasDescription);
      Wam.MaybeSerializeField(4, this.hasCategory);
      Wam.MaybeSerializeField(5, this.hasAddress);
      Wam.MaybeSerializeField(6, this.hasHours);
      Wam.MaybeSerializeField(7, this.hasEmail);
      Wam.MaybeSerializeField(8, this.hasWebsite);
    }
  }
}
