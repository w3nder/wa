// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.ForwardSend
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class ForwardSend : WamEvent
  {
    public wam_enum_message_type? messageType;
    public wam_enum_media_type? messageMediaType;
    public bool? messageIsFastForward;
    public long? messageForwardAgeT;
    public bool? fastForwardEnabled;
    public bool? messageIsFanout;
    public long? retryCount;
    public long? resendCount;
    public bool? messageIsInternational;
    public bool? mediaCaptionPresent;
    public long? e2eCiphertextVersion;
    public wam_enum_e2e_ciphertext_type? e2eCiphertextType;
    public long? messageSendT;

    public void Reset()
    {
      this.messageType = new wam_enum_message_type?();
      this.messageMediaType = new wam_enum_media_type?();
      this.messageIsFastForward = new bool?();
      this.messageForwardAgeT = new long?();
      this.fastForwardEnabled = new bool?();
      this.messageIsFanout = new bool?();
      this.retryCount = new long?();
      this.resendCount = new long?();
      this.messageIsInternational = new bool?();
      this.mediaCaptionPresent = new bool?();
      this.e2eCiphertextVersion = new long?();
      this.e2eCiphertextType = new wam_enum_e2e_ciphertext_type?();
      this.messageSendT = new long?();
    }

    public override uint GetCode() => 1728;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(3, this.messageIsFastForward);
      Wam.MaybeSerializeField(4, this.messageForwardAgeT);
      Wam.MaybeSerializeField(5, this.fastForwardEnabled);
      Wam.MaybeSerializeField(6, this.messageIsFanout);
      Wam.MaybeSerializeField(7, this.retryCount);
      Wam.MaybeSerializeField(8, this.resendCount);
      Wam.MaybeSerializeField(9, this.messageIsInternational);
      Wam.MaybeSerializeField(10, this.mediaCaptionPresent);
      Wam.MaybeSerializeField(11, this.e2eCiphertextVersion);
      Wam.MaybeSerializeField(12, Wam.EnumToLong<wam_enum_e2e_ciphertext_type>(this.e2eCiphertextType));
      Wam.MaybeSerializeField(13, this.messageSendT);
    }
  }
}
