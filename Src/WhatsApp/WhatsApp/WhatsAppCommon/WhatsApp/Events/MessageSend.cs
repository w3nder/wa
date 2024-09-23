// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MessageSend
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class MessageSend : WamEvent
  {
    public wam_enum_message_send_result_type? messageSendResult;
    public bool? messageSendResultIsTerminal;
    public wam_enum_message_type? messageType;
    public wam_enum_media_type? messageMediaType;
    public bool? messageIsForward;
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
    public bool? messageSendOptUploadEnabled;
    public bool? stickerIsFirstParty;
    public long? messageSendT;

    public void Reset()
    {
      this.messageSendResult = new wam_enum_message_send_result_type?();
      this.messageSendResultIsTerminal = new bool?();
      this.messageType = new wam_enum_message_type?();
      this.messageMediaType = new wam_enum_media_type?();
      this.messageIsForward = new bool?();
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
      this.messageSendOptUploadEnabled = new bool?();
      this.stickerIsFirstParty = new bool?();
      this.messageSendT = new long?();
    }

    public override uint GetCode() => 854;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_message_send_result_type>(this.messageSendResult));
      Wam.MaybeSerializeField(17, this.messageSendResultIsTerminal);
      Wam.MaybeSerializeField(2, Wam.EnumToLong<wam_enum_message_type>(this.messageType));
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_media_type>(this.messageMediaType));
      Wam.MaybeSerializeField(4, this.messageIsForward);
      Wam.MaybeSerializeField(13, this.messageIsFastForward);
      Wam.MaybeSerializeField(14, this.messageForwardAgeT);
      Wam.MaybeSerializeField(15, this.fastForwardEnabled);
      Wam.MaybeSerializeField(5, this.messageIsFanout);
      Wam.MaybeSerializeField(6, this.retryCount);
      Wam.MaybeSerializeField(16, this.resendCount);
      Wam.MaybeSerializeField(7, this.messageIsInternational);
      Wam.MaybeSerializeField(8, this.mediaCaptionPresent);
      Wam.MaybeSerializeField(9, this.e2eCiphertextVersion);
      Wam.MaybeSerializeField(10, Wam.EnumToLong<wam_enum_e2e_ciphertext_type>(this.e2eCiphertextType));
      Wam.MaybeSerializeField(12, this.messageSendOptUploadEnabled);
      Wam.MaybeSerializeField(18, this.stickerIsFirstParty);
      Wam.MaybeSerializeField(11, this.messageSendT);
    }
  }
}
