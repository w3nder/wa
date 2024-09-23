// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.EmojiSearchEmojiSelected
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class EmojiSearchEmojiSelected : WamEvent
  {
    public long? emojiSearchRankOfSelectedEmoji;
    public long? emojiSearchResultCount;
    public wam_enum_emoji_search_ui_type? emojiSearchUiId;
    public string inputLanguageCode;
    public string languageCode;

    public void Reset()
    {
      this.emojiSearchRankOfSelectedEmoji = new long?();
      this.emojiSearchResultCount = new long?();
      this.emojiSearchUiId = new wam_enum_emoji_search_ui_type?();
      this.inputLanguageCode = (string) null;
      this.languageCode = (string) null;
    }

    public override uint GetCode() => 1416;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.emojiSearchRankOfSelectedEmoji);
      Wam.MaybeSerializeField(2, this.emojiSearchResultCount);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_emoji_search_ui_type>(this.emojiSearchUiId));
      Wam.MaybeSerializeField(7, this.inputLanguageCode);
      Wam.MaybeSerializeField(6, this.languageCode);
    }
  }
}
