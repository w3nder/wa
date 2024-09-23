// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaPicker
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaPicker : WamEvent
  {
    public wam_enum_media_type? mediaType;
    public long? mediaPickerSent;
    public long? mediaPickerSentUnchanged;
    public long? mediaPickerDeleted;
    public long? mediaPickerChanged;
    public long? mediaPickerCroppedRotated;
    public long? mediaPickerDrawing;
    public long? mediaPickerStickers;
    public long? mediaPickerText;
    public long? mediaPickerFilter;
    public long? mediaPickerLikeDoc;
    public long? mediaPickerNotLikeDoc;
    public wam_enum_media_picker_origin_type? mediaPickerOrigin;
    public long? mediaPickerT;
    public long? chatRecipients;
    public long? statusRecipients;

    public void Reset()
    {
      this.mediaType = new wam_enum_media_type?();
      this.mediaPickerSent = new long?();
      this.mediaPickerSentUnchanged = new long?();
      this.mediaPickerDeleted = new long?();
      this.mediaPickerChanged = new long?();
      this.mediaPickerCroppedRotated = new long?();
      this.mediaPickerDrawing = new long?();
      this.mediaPickerStickers = new long?();
      this.mediaPickerText = new long?();
      this.mediaPickerFilter = new long?();
      this.mediaPickerLikeDoc = new long?();
      this.mediaPickerNotLikeDoc = new long?();
      this.mediaPickerOrigin = new wam_enum_media_picker_origin_type?();
      this.mediaPickerT = new long?();
      this.chatRecipients = new long?();
      this.statusRecipients = new long?();
    }

    public override uint GetCode() => 1038;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(2, this.mediaPickerSent);
      Wam.MaybeSerializeField(5, this.mediaPickerSentUnchanged);
      Wam.MaybeSerializeField(3, this.mediaPickerDeleted);
      Wam.MaybeSerializeField(4, this.mediaPickerChanged);
      Wam.MaybeSerializeField(10, this.mediaPickerCroppedRotated);
      Wam.MaybeSerializeField(11, this.mediaPickerDrawing);
      Wam.MaybeSerializeField(12, this.mediaPickerStickers);
      Wam.MaybeSerializeField(13, this.mediaPickerText);
      Wam.MaybeSerializeField(18, this.mediaPickerFilter);
      Wam.MaybeSerializeField(19, this.mediaPickerLikeDoc);
      Wam.MaybeSerializeField(20, this.mediaPickerNotLikeDoc);
      Wam.MaybeSerializeField(14, Wam.EnumToLong<wam_enum_media_picker_origin_type>(this.mediaPickerOrigin));
      Wam.MaybeSerializeField(15, this.mediaPickerT);
      Wam.MaybeSerializeField(16, this.chatRecipients);
      Wam.MaybeSerializeField(17, this.statusRecipients);
    }
  }
}
