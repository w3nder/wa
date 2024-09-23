// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusItemView
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class StatusItemView : WamEvent
  {
    public long? statusViewerSessionId;
    public wam_enum_status_row_section? statusRowSection;
    public long? statusRowIndex;
    public wam_enum_status_item_view_result? statusItemViewResult;
    public wam_enum_media_type? mediaType;
    public long? statusItemLoadTime;
    public long? statusItemViewTime;
    public long? statusItemLength;
    public long? statusItemReplied;
    public long? statusItemViewCount;
    public bool? statusItemUnread;

    public void Reset()
    {
      this.statusViewerSessionId = new long?();
      this.statusRowSection = new wam_enum_status_row_section?();
      this.statusRowIndex = new long?();
      this.statusItemViewResult = new wam_enum_status_item_view_result?();
      this.mediaType = new wam_enum_media_type?();
      this.statusItemLoadTime = new long?();
      this.statusItemViewTime = new long?();
      this.statusItemLength = new long?();
      this.statusItemReplied = new long?();
      this.statusItemViewCount = new long?();
      this.statusItemUnread = new bool?();
    }

    public override uint GetCode() => 1658;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusViewerSessionId);
      Wam.MaybeSerializeField(11, Wam.EnumToLong<wam_enum_status_row_section>(this.statusRowSection));
      Wam.MaybeSerializeField(2, this.statusRowIndex);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_status_item_view_result>(this.statusItemViewResult));
      Wam.MaybeSerializeField(4, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(5, this.statusItemLoadTime);
      Wam.MaybeSerializeField(6, this.statusItemViewTime);
      Wam.MaybeSerializeField(7, this.statusItemLength);
      Wam.MaybeSerializeField(8, this.statusItemReplied);
      Wam.MaybeSerializeField(10, this.statusItemViewCount);
      Wam.MaybeSerializeField(9, this.statusItemUnread);
    }
  }
}
