// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.WamClientErrors
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class WamClientErrors : WamEvent
  {
    public long? wamClientDroppedEventCount;
    public long? wamClientDroppedEventSize;
    public bool? wamClientErrorFlags;
    public bool? wamErrorWriteFile;
    public bool? wamErrorWriteHeader;
    public bool? wamErrorWriteEventBuffer;
    public bool? wamErrorReadFile;
    public bool? wamErrorBadFileSize;
    public bool? wamErrorFseekFile;
    public bool? wamErrorOpenFile;
    public bool? wamErrorCloseFile;
    public bool? wamErrorRemoveFile;
    public bool? wamErrorOpenWamFile;
    public bool? wamErrorCreateWamFile;
    public bool? wamErrorBadFileHeader;
    public bool? wamErrorBadEventBuffer;
    public bool? wamErrorBadHeaderChecksum;
    public bool? wamErrorBadCurrentEventBufferChecksum;
    public bool? wamErrorBadRotatedEventBufferChecksum;
    public bool? wamErrorPersistence;

    public void Reset()
    {
      this.wamClientDroppedEventCount = new long?();
      this.wamClientDroppedEventSize = new long?();
      this.wamClientErrorFlags = new bool?();
      this.wamErrorWriteFile = new bool?();
      this.wamErrorWriteHeader = new bool?();
      this.wamErrorWriteEventBuffer = new bool?();
      this.wamErrorReadFile = new bool?();
      this.wamErrorBadFileSize = new bool?();
      this.wamErrorFseekFile = new bool?();
      this.wamErrorOpenFile = new bool?();
      this.wamErrorCloseFile = new bool?();
      this.wamErrorRemoveFile = new bool?();
      this.wamErrorOpenWamFile = new bool?();
      this.wamErrorCreateWamFile = new bool?();
      this.wamErrorBadFileHeader = new bool?();
      this.wamErrorBadEventBuffer = new bool?();
      this.wamErrorBadHeaderChecksum = new bool?();
      this.wamErrorBadCurrentEventBufferChecksum = new bool?();
      this.wamErrorBadRotatedEventBufferChecksum = new bool?();
      this.wamErrorPersistence = new bool?();
    }

    public override uint GetCode() => 1144;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(2, this.wamClientDroppedEventCount);
      Wam.MaybeSerializeField(3, this.wamClientDroppedEventSize);
      Wam.MaybeSerializeField(1, this.wamClientErrorFlags);
      Wam.MaybeSerializeField(4, this.wamErrorWriteFile);
      Wam.MaybeSerializeField(5, this.wamErrorWriteHeader);
      Wam.MaybeSerializeField(6, this.wamErrorWriteEventBuffer);
      Wam.MaybeSerializeField(7, this.wamErrorReadFile);
      Wam.MaybeSerializeField(8, this.wamErrorBadFileSize);
      Wam.MaybeSerializeField(9, this.wamErrorFseekFile);
      Wam.MaybeSerializeField(10, this.wamErrorOpenFile);
      Wam.MaybeSerializeField(11, this.wamErrorCloseFile);
      Wam.MaybeSerializeField(12, this.wamErrorRemoveFile);
      Wam.MaybeSerializeField(13, this.wamErrorOpenWamFile);
      Wam.MaybeSerializeField(14, this.wamErrorCreateWamFile);
      Wam.MaybeSerializeField(15, this.wamErrorBadFileHeader);
      Wam.MaybeSerializeField(16, this.wamErrorBadEventBuffer);
      Wam.MaybeSerializeField(17, this.wamErrorBadHeaderChecksum);
      Wam.MaybeSerializeField(18, this.wamErrorBadCurrentEventBufferChecksum);
      Wam.MaybeSerializeField(19, this.wamErrorBadRotatedEventBufferChecksum);
      Wam.MaybeSerializeField(20, this.wamErrorPersistence);
    }
  }
}
