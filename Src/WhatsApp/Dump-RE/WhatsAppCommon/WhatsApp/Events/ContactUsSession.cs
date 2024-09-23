// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ContactUsSession
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class ContactUsSession : WamEvent
  {
    public wam_enum_contact_us_exit_state? contactUsExitState;
    public bool? contactUsFaq;
    public bool? contactUsAutomaticEmail;
    public bool? contactUsLogs;
    public bool? contactUsOutage;
    public bool? contactUsOutageEmail;
    public string contactUsProblemDescription;
    public long? searchFaqResultsBestId;
    public long? searchFaqResultsBestId2;
    public long? searchFaqResultsBestId3;
    public long? contactUsT;
    public long? contactUsMenuFaqT;
    public double? searchFaqResultsGeneratedC;
    public double? searchFaqResultsReadC;
    public long? searchFaqResultsBestReadT;
    public long? searchFaqResultsBestReadT2;
    public long? searchFaqResultsBestReadT3;
    public long? searchFaqResultsReadT;
    public double? contactUsScreenshotC;
    public string languageCode;

    public void Reset()
    {
      this.contactUsExitState = new wam_enum_contact_us_exit_state?();
      this.contactUsFaq = new bool?();
      this.contactUsAutomaticEmail = new bool?();
      this.contactUsLogs = new bool?();
      this.contactUsOutage = new bool?();
      this.contactUsOutageEmail = new bool?();
      this.contactUsProblemDescription = (string) null;
      this.searchFaqResultsBestId = new long?();
      this.searchFaqResultsBestId2 = new long?();
      this.searchFaqResultsBestId3 = new long?();
      this.contactUsT = new long?();
      this.contactUsMenuFaqT = new long?();
      this.searchFaqResultsGeneratedC = new double?();
      this.searchFaqResultsReadC = new double?();
      this.searchFaqResultsBestReadT = new long?();
      this.searchFaqResultsBestReadT2 = new long?();
      this.searchFaqResultsBestReadT3 = new long?();
      this.searchFaqResultsReadT = new long?();
      this.contactUsScreenshotC = new double?();
      this.languageCode = (string) null;
    }

    public override uint GetCode() => 470;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_contact_us_exit_state>(this.contactUsExitState));
      Wam.MaybeSerializeField(2, this.contactUsFaq);
      Wam.MaybeSerializeField(3, this.contactUsAutomaticEmail);
      Wam.MaybeSerializeField(4, this.contactUsLogs);
      Wam.MaybeSerializeField(5, this.contactUsOutage);
      Wam.MaybeSerializeField(6, this.contactUsOutageEmail);
      Wam.MaybeSerializeField(7, this.contactUsProblemDescription);
      Wam.MaybeSerializeField(8, this.searchFaqResultsBestId);
      Wam.MaybeSerializeField(9, this.searchFaqResultsBestId2);
      Wam.MaybeSerializeField(10, this.searchFaqResultsBestId3);
      Wam.MaybeSerializeField(11, this.contactUsT);
      Wam.MaybeSerializeField(12, this.contactUsMenuFaqT);
      Wam.MaybeSerializeField(13, this.searchFaqResultsGeneratedC);
      Wam.MaybeSerializeField(14, this.searchFaqResultsReadC);
      Wam.MaybeSerializeField(15, this.searchFaqResultsBestReadT);
      Wam.MaybeSerializeField(16, this.searchFaqResultsBestReadT2);
      Wam.MaybeSerializeField(17, this.searchFaqResultsBestReadT3);
      Wam.MaybeSerializeField(18, this.searchFaqResultsReadT);
      Wam.MaybeSerializeField(19, this.contactUsScreenshotC);
      Wam.MaybeSerializeField(21, this.languageCode);
    }
  }
}
