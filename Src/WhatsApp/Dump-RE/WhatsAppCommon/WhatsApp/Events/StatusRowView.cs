// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.StatusRowView
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class StatusRowView : WamEvent
  {
    public long? statusSessionId;
    public long? statusViewerSessionId;
    public wam_enum_status_row_section? statusRowSection;
    public long? statusRowIndex;
    public wam_enum_status_row_entry_method? statusRowEntryMethod;
    public long? statusRowViewCount;
    public long? statusRowUnreadItemCount;

    public void Reset()
    {
      this.statusSessionId = new long?();
      this.statusViewerSessionId = new long?();
      this.statusRowSection = new wam_enum_status_row_section?();
      this.statusRowIndex = new long?();
      this.statusRowEntryMethod = new wam_enum_status_row_entry_method?();
      this.statusRowViewCount = new long?();
      this.statusRowUnreadItemCount = new long?();
    }

    public override uint GetCode() => 1656;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, this.statusSessionId);
      Wam.MaybeSerializeField(2, this.statusViewerSessionId);
      Wam.MaybeSerializeField(3, Wam.EnumToLong<wam_enum_status_row_section>(this.statusRowSection));
      Wam.MaybeSerializeField(4, this.statusRowIndex);
      Wam.MaybeSerializeField(5, Wam.EnumToLong<wam_enum_status_row_entry_method>(this.statusRowEntryMethod));
      Wam.MaybeSerializeField(6, this.statusRowViewCount);
      Wam.MaybeSerializeField(7, this.statusRowUnreadItemCount);
    }
  }
}
