// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.EmojiSearchSessionStopped
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class EmojiSearchSessionStopped : WamEvent
  {
    public long? emojiSearchCountEmojiSelected;
    public wam_enum_emoji_search_ui_type? emojiSearchUiId;
    public string inputLanguageCode;
    public string languageCode;

    public void Reset()
    {
      this.emojiSearchCountEmojiSelected = new long?();
      this.emojiSearchUiId = new wam_enum_emoji_search_ui_type?();
      this.inputLanguageCode = (string) null;
      this.languageCode = (string) null;
    }

    public override uint GetCode() => 1428;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(3, this.emojiSearchCountEmojiSelected);
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_emoji_search_ui_type>(this.emojiSearchUiId));
      Wam.MaybeSerializeField(6, this.inputLanguageCode);
      Wam.MaybeSerializeField(5, this.languageCode);
    }
  }
}
