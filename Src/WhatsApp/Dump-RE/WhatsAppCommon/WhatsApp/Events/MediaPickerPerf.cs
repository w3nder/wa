// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.MediaPickerPerf
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class MediaPickerPerf : WamEvent
  {
    public wam_enum_media_picker_origin_type? mediaPickerPerfOrigin;
    public long? mediaPickerPerfInvocationCount;
    public long? mediaPickerPerfPresentationT;
    public long? mediaPickerPerfDismissT;
    public long? mediaPickerPerfReadyT;
    public long? mediaPickerPerfNumAdded;
    public long? mediaPickerPerfNumRemoved;
    public long? mediaPickerPerfImageCount;
    public long? mediaPickerPerfGifCount;
    public long? mediaPickerPerfVideoCount;

    public void Reset()
    {
      this.mediaPickerPerfOrigin = new wam_enum_media_picker_origin_type?();
      this.mediaPickerPerfInvocationCount = new long?();
      this.mediaPickerPerfPresentationT = new long?();
      this.mediaPickerPerfDismissT = new long?();
      this.mediaPickerPerfReadyT = new long?();
      this.mediaPickerPerfNumAdded = new long?();
      this.mediaPickerPerfNumRemoved = new long?();
      this.mediaPickerPerfImageCount = new long?();
      this.mediaPickerPerfGifCount = new long?();
      this.mediaPickerPerfVideoCount = new long?();
    }

    public override uint GetCode() => 1534;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_picker_origin_type>(this.mediaPickerPerfOrigin));
      Wam.MaybeSerializeField(2, this.mediaPickerPerfInvocationCount);
      Wam.MaybeSerializeField(3, this.mediaPickerPerfPresentationT);
      Wam.MaybeSerializeField(4, this.mediaPickerPerfDismissT);
      Wam.MaybeSerializeField(5, this.mediaPickerPerfReadyT);
      Wam.MaybeSerializeField(6, this.mediaPickerPerfNumAdded);
      Wam.MaybeSerializeField(7, this.mediaPickerPerfNumRemoved);
      Wam.MaybeSerializeField(8, this.mediaPickerPerfImageCount);
      Wam.MaybeSerializeField(9, this.mediaPickerPerfGifCount);
      Wam.MaybeSerializeField(10, this.mediaPickerPerfVideoCount);
    }
  }
}
