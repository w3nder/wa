// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Backup
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class Backup : WamEvent
  {
    public wam_enum_backup_restore_result? backupRestoreResult;
    public bool? backupRestoreIsFull;
    public long? backupRestoreIsWifi;
    public long? backupRestoreRetryCount;
    public bool? backupRestoreIncludeVideos;
    public long? backupRestoreFinishedOverWifi;
    public wam_enum_backup_restore_stage? backupRestoreStage;
    public long? backupRestoreT;
    public double? backupRestoreTotalSize;
    public double? backupRestoreChatdbSize;
    public double? backupRestoreMediaSize;
    public double? backupRestoreTransferSize;
    public double? backupRestoreTransferFailedSize;
    public double? backupRestoreMediaFileCount;
    public double? backupRestoreNetworkRequestCount;
    public long? backupFilesDeletedInScrubCount;
    public wam_enum_backup_network_setting? backupNetworkSetting;
    public wam_enum_backup_schedule? backupSchedule;

    public void Reset()
    {
      this.backupRestoreResult = new wam_enum_backup_restore_result?();
      this.backupRestoreIsFull = new bool?();
      this.backupRestoreIsWifi = new long?();
      this.backupRestoreRetryCount = new long?();
      this.backupRestoreIncludeVideos = new bool?();
      this.backupRestoreFinishedOverWifi = new long?();
      this.backupRestoreStage = new wam_enum_backup_restore_stage?();
      this.backupRestoreT = new long?();
      this.backupRestoreTotalSize = new double?();
      this.backupRestoreChatdbSize = new double?();
      this.backupRestoreMediaSize = new double?();
      this.backupRestoreTransferSize = new double?();
      this.backupRestoreTransferFailedSize = new double?();
      this.backupRestoreMediaFileCount = new double?();
      this.backupRestoreNetworkRequestCount = new double?();
      this.backupFilesDeletedInScrubCount = new long?();
      this.backupNetworkSetting = new wam_enum_backup_network_setting?();
      this.backupSchedule = new wam_enum_backup_schedule?();
    }

    public override uint GetCode() => 484;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(1, Wam.EnumToLong<wam_enum_backup_restore_result>(this.backupRestoreResult));
      Wam.MaybeSerializeField(2, this.backupRestoreIsFull);
      Wam.MaybeSerializeField(3, this.backupRestoreIsWifi);
      Wam.MaybeSerializeField(4, this.backupRestoreRetryCount);
      Wam.MaybeSerializeField(5, this.backupRestoreIncludeVideos);
      Wam.MaybeSerializeField(6, this.backupRestoreFinishedOverWifi);
      Wam.MaybeSerializeField(7, Wam.EnumToLong<wam_enum_backup_restore_stage>(this.backupRestoreStage));
      Wam.MaybeSerializeField(8, this.backupRestoreT);
      Wam.MaybeSerializeField(9, this.backupRestoreTotalSize);
      Wam.MaybeSerializeField(10, this.backupRestoreChatdbSize);
      Wam.MaybeSerializeField(11, this.backupRestoreMediaSize);
      Wam.MaybeSerializeField(12, this.backupRestoreTransferSize);
      Wam.MaybeSerializeField(13, this.backupRestoreTransferFailedSize);
      Wam.MaybeSerializeField(14, this.backupRestoreMediaFileCount);
      Wam.MaybeSerializeField(15, this.backupRestoreNetworkRequestCount);
      Wam.MaybeSerializeField(16, this.backupFilesDeletedInScrubCount);
      Wam.MaybeSerializeField(17, Wam.EnumToLong<wam_enum_backup_network_setting>(this.backupNetworkSetting));
      Wam.MaybeSerializeField(18, Wam.EnumToLong<wam_enum_backup_schedule>(this.backupSchedule));
    }
  }
}
