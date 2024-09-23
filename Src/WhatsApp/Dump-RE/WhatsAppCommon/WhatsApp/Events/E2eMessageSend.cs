// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.E2eMessageSend
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class E2eMessageSend : WamEvent
  {
    public bool? e2eSuccessful;
    public wam_enum_e2e_failure_reason? e2eFailureReason;
    public long? retryCount;
    public wam_enum_e2e_destination? e2eDestination;
    public wam_enum_e2e_ciphertext_type? e2eCiphertextType;
    public long? e2eCiphertextVersion;
    public wam_enum_media_type? messageMediaType;

    public void Reset()
    {
      this.e2eSuccessful = new bool?();
      this.e2eFailureReason = new wam_enum_e2e_failure_reason?();
      this.retryCount = new long?();
      this.e2eDestination = new wam_enum_e2e_destination?();
      this.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?();
      this.e2eCiphertextVersion = new long?();
      this.messageMediaType = new wam_enum_media_type?();
    }

    public override uint GetCode() => 476;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.e2eSuccessful);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_e2e_failure_reason>(this.e2eFailureReason));
      Wam.MaybeSerializeField(3, this.retryCount);
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_e2e_destination>(this.e2eDestination));
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_e2e_ciphertext_type>(this.e2eCiphertextType));
      Wam.MaybeSerializeField(6, this.e2eCiphertextVersion);
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
    }
  }
}
