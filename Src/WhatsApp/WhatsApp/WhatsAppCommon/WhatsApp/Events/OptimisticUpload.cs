// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.OptimisticUpload
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp.Events
{
  public class OptimisticUpload : WamEvent
  {
    public wam_enum_media_type? mediaType;
    public long? optSupplied;
    public long? optIgnoredDisabled;
    public long? optIgnoredStopped;
    public long? optIgnoredNotAppropriate;
    public long? optAdded;
    public long? optDeletedB4Upload;
    public long? optDeletedUploadOk;
    public long? optDeletedUploadNotOk;
    public long? optSentB4Upload;
    public long? optSentUploadOk;
    public long? optSentUploadNotOk;
    public long? optSentUploadUseful;
    public wam_enum_opt_upload_context_type? optUploadContext;
    public long? optUploadAvailableT;

    public void Reset()
    {
      this.mediaType = new wam_enum_media_type?();
      this.optSupplied = new long?();
      this.optIgnoredDisabled = new long?();
      this.optIgnoredStopped = new long?();
      this.optIgnoredNotAppropriate = new long?();
      this.optAdded = new long?();
      this.optDeletedB4Upload = new long?();
      this.optDeletedUploadOk = new long?();
      this.optDeletedUploadNotOk = new long?();
      this.optSentB4Upload = new long?();
      this.optSentUploadOk = new long?();
      this.optSentUploadNotOk = new long?();
      this.optSentUploadUseful = new long?();
      this.optUploadContext = new wam_enum_opt_upload_context_type?();
      this.optUploadAvailableT = new long?();
    }

    public override uint GetCode() => 1198;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_media_type>(this.mediaType));
      Wam.MaybeSerializeField(2, this.optSupplied);
      Wam.MaybeSerializeField(3, this.optIgnoredDisabled);
      Wam.MaybeSerializeField(4, this.optIgnoredStopped);
      Wam.MaybeSerializeField(5, this.optIgnoredNotAppropriate);
      Wam.MaybeSerializeField(6, this.optAdded);
      Wam.MaybeSerializeField(7, this.optDeletedB4Upload);
      Wam.MaybeSerializeField(8, this.optDeletedUploadOk);
      Wam.MaybeSerializeField(9, this.optDeletedUploadNotOk);
      Wam.MaybeSerializeField(10, this.optSentB4Upload);
      Wam.MaybeSerializeField(11, this.optSentUploadOk);
      Wam.MaybeSerializeField(12, this.optSentUploadNotOk);
      Wam.MaybeSerializeField(13, this.optSentUploadUseful);
      Wam.MaybeSerializeField(14, Wam.EnumToLong<wam_enum_opt_upload_context_type>(this.optUploadContext));
      Wam.MaybeSerializeField(15, this.optUploadAvailableT);
    }
  }
}
