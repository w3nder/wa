// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.QuickReply
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class QuickReply : WamEvent
  {
    public wam_enum_quick_reply_origin? quickReplyOrigin;
    public wam_enum_quick_reply_action? quickReplyAction;
    public long? quickReplyCount;
    public long? quickReplyKeywordCount;
    public bool? quickReplyKeywordMatched;
    public long? attachmentImageCount;
    public long? attachmentVideoCount;
    public long? attachmentGifCount;
    public wam_enum_quick_reply_transcode_result? quickReplyTranscodeResult;

    public void Reset()
    {
      this.quickReplyOrigin = new wam_enum_quick_reply_origin?();
      this.quickReplyAction = new wam_enum_quick_reply_action?();
      this.quickReplyCount = new long?();
      this.quickReplyKeywordCount = new long?();
      this.quickReplyKeywordMatched = new bool?();
      this.attachmentImageCount = new long?();
      this.attachmentVideoCount = new long?();
      this.attachmentGifCount = new long?();
      this.quickReplyTranscodeResult = new wam_enum_quick_reply_transcode_result?();
    }

    public override uint GetCode() => 1468;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(9, Wam.EnumToLong<wam_enum_quick_reply_origin>(this.quickReplyOrigin));
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_quick_reply_action>(this.quickReplyAction));
      Wam.MaybeSerializeField(2, this.quickReplyCount);
      Wam.MaybeSerializeField(3, this.quickReplyKeywordCount);
      Wam.MaybeSerializeField(4, this.quickReplyKeywordMatched);
      Wam.MaybeSerializeField(5, this.attachmentImageCount);
      Wam.MaybeSerializeField(6, this.attachmentVideoCount);
      Wam.MaybeSerializeField(7, this.attachmentGifCount);
      Wam.MaybeSerializeField(8, Wam.EnumToLong<wam_enum_quick_reply_transcode_result>(this.quickReplyTranscodeResult));
    }
  }
}
