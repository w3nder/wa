// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.RegistrationComplete
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class RegistrationComplete : WamEvent
  {
    public long? registrationT;
    public long? registrationTForFillBusinessInfoScreen;
    public bool? registrationRetryFetchingBizProfile;
    public bool? registrationAttemptSkipWithNoVertical;

    public void Reset()
    {
      this.registrationT = new long?();
      this.registrationTForFillBusinessInfoScreen = new long?();
      this.registrationRetryFetchingBizProfile = new bool?();
      this.registrationAttemptSkipWithNoVertical = new bool?();
    }

    public override uint GetCode() => 1342;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.registrationT);
      Wam.MaybeSerializeField(2, this.registrationTForFillBusinessInfoScreen);
      Wam.MaybeSerializeField(3, this.registrationRetryFetchingBizProfile);
      Wam.MaybeSerializeField(4, this.registrationAttemptSkipWithNoVertical);
    }
  }
}
