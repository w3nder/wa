// Decompiled with JetBrains decompiler
// Type: WhatsApp.Events.Daily
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

#nullable disable
namespace WhatsApp.Events
{
  public class Daily : WamEvent
  {
    public bool? permissionContacts;
    public string packageName;
    public string appCodeHash;
    public long? liveLocationSharingT;
    public long? liveLocationReportingT;
    public bool? autoDlImageWifi;
    public bool? autoDlImageCellular;
    public bool? autoDlImageRoaming;
    public bool? autoDlAudioWifi;
    public bool? autoDlAudioCellular;
    public bool? autoDlAudioRoaming;
    public bool? autoDlVideoWifi;
    public bool? autoDlVideoCellular;
    public bool? autoDlVideoRoaming;
    public bool? autoDlDocWifi;
    public bool? autoDlDocCellular;
    public bool? autoDlDocRoaming;
    public bool? paymentsIsEnabled;
    public long? simMcc;
    public long? simMnc;
    public string osBuildNumber;
    public string languageCode;
    public string locationCode;
    public bool? networkIsRoaming;
    public bool? receiptsEnabled;
    public wam_enum_backup_schedule? backupSchedule;
    public wam_enum_backup_network_setting? backupNetworkSetting;
    public long? addressbookSize;
    public long? addressbookWhatsappSize;
    public long? individualChatCount;
    public long? individualArchivedChatCount;
    public long? groupChatCount;
    public long? groupArchivedChatCount;
    public long? broadcastChatCount;
    public long? broadcastArchivedChatCount;
    public long? chatDatabaseSize;
    public long? mediaFolderSize;
    public long? mediaFolderFileCount;
    public long? videoFolderSize;
    public long? videoFolderFileCount;
    public long? dbMessagesCnt;
    public long? dbMessagesUnindexedCnt;
    public long? dbMessagesStarredCnt;
    public double? dbMessagesIndexedPct;
    public string wpBatsaver;
    public string wpIsPushDaemonConnected;
    public long? wpLastBackup;
    public bool? wpScheduled;
    public string wpVoipExitReason;
    public string wpVoipException;

    public void Reset()
    {
      this.permissionContacts = new bool?();
      this.packageName = (string) null;
      this.appCodeHash = (string) null;
      this.liveLocationSharingT = new long?();
      this.liveLocationReportingT = new long?();
      this.autoDlImageWifi = new bool?();
      this.autoDlImageCellular = new bool?();
      this.autoDlImageRoaming = new bool?();
      this.autoDlAudioWifi = new bool?();
      this.autoDlAudioCellular = new bool?();
      this.autoDlAudioRoaming = new bool?();
      this.autoDlVideoWifi = new bool?();
      this.autoDlVideoCellular = new bool?();
      this.autoDlVideoRoaming = new bool?();
      this.autoDlDocWifi = new bool?();
      this.autoDlDocCellular = new bool?();
      this.autoDlDocRoaming = new bool?();
      this.paymentsIsEnabled = new bool?();
      this.simMcc = new long?();
      this.simMnc = new long?();
      this.osBuildNumber = (string) null;
      this.languageCode = (string) null;
      this.locationCode = (string) null;
      this.networkIsRoaming = new bool?();
      this.receiptsEnabled = new bool?();
      this.backupSchedule = new wam_enum_backup_schedule?();
      this.backupNetworkSetting = new wam_enum_backup_network_setting?();
      this.addressbookSize = new long?();
      this.addressbookWhatsappSize = new long?();
      this.individualChatCount = new long?();
      this.individualArchivedChatCount = new long?();
      this.groupChatCount = new long?();
      this.groupArchivedChatCount = new long?();
      this.broadcastChatCount = new long?();
      this.broadcastArchivedChatCount = new long?();
      this.chatDatabaseSize = new long?();
      this.mediaFolderSize = new long?();
      this.mediaFolderFileCount = new long?();
      this.videoFolderSize = new long?();
      this.videoFolderFileCount = new long?();
      this.dbMessagesCnt = new long?();
      this.dbMessagesUnindexedCnt = new long?();
      this.dbMessagesStarredCnt = new long?();
      this.dbMessagesIndexedPct = new double?();
      this.wpBatsaver = (string) null;
      this.wpIsPushDaemonConnected = (string) null;
      this.wpLastBackup = new long?();
      this.wpScheduled = new bool?();
      this.wpVoipExitReason = (string) null;
      this.wpVoipException = (string) null;
    }

    public override uint GetCode() => 1158;

    public override void SerializeFields()
    {
      Wam.MaybeSerializeField(104, this.permissionContacts);
      Wam.MaybeSerializeField(102, this.packageName);
      Wam.MaybeSerializeField(103, this.appCodeHash);
      Wam.MaybeSerializeField(80, this.liveLocationSharingT);
      Wam.MaybeSerializeField(81, this.liveLocationReportingT);
      Wam.MaybeSerializeField(86, this.autoDlImageWifi);
      Wam.MaybeSerializeField(87, this.autoDlImageCellular);
      Wam.MaybeSerializeField(88, this.autoDlImageRoaming);
      Wam.MaybeSerializeField(89, this.autoDlAudioWifi);
      Wam.MaybeSerializeField(90, this.autoDlAudioCellular);
      Wam.MaybeSerializeField(91, this.autoDlAudioRoaming);
      Wam.MaybeSerializeField(92, this.autoDlVideoWifi);
      Wam.MaybeSerializeField(93, this.autoDlVideoCellular);
      Wam.MaybeSerializeField(94, this.autoDlVideoRoaming);
      Wam.MaybeSerializeField(95, this.autoDlDocWifi);
      Wam.MaybeSerializeField(96, this.autoDlDocCellular);
      Wam.MaybeSerializeField(97, this.autoDlDocRoaming);
      Wam.MaybeSerializeField(100, this.paymentsIsEnabled);
      Wam.MaybeSerializeField(2, this.simMcc);
      Wam.MaybeSerializeField(3, this.simMnc);
      Wam.MaybeSerializeField(4, this.osBuildNumber);
      Wam.MaybeSerializeField(5, this.languageCode);
      Wam.MaybeSerializeField(6, this.locationCode);
      Wam.MaybeSerializeField(7, this.networkIsRoaming);
      Wam.MaybeSerializeField(8, this.receiptsEnabled);
      Wam.MaybeSerializeField(9, Wam.EnumToLong<wam_enum_backup_schedule>(this.backupSchedule));
      Wam.MaybeSerializeField(10, Wam.EnumToLong<wam_enum_backup_network_setting>(this.backupNetworkSetting));
      Wam.MaybeSerializeField(11, this.addressbookSize);
      Wam.MaybeSerializeField(12, this.addressbookWhatsappSize);
      Wam.MaybeSerializeField(13, this.individualChatCount);
      Wam.MaybeSerializeField(14, this.individualArchivedChatCount);
      Wam.MaybeSerializeField(15, this.groupChatCount);
      Wam.MaybeSerializeField(16, this.groupArchivedChatCount);
      Wam.MaybeSerializeField(17, this.broadcastChatCount);
      Wam.MaybeSerializeField(18, this.broadcastArchivedChatCount);
      Wam.MaybeSerializeField(19, this.chatDatabaseSize);
      Wam.MaybeSerializeField(20, this.mediaFolderSize);
      Wam.MaybeSerializeField(21, this.mediaFolderFileCount);
      Wam.MaybeSerializeField(22, this.videoFolderSize);
      Wam.MaybeSerializeField(23, this.videoFolderFileCount);
      Wam.MaybeSerializeField(24, this.dbMessagesCnt);
      Wam.MaybeSerializeField(25, this.dbMessagesUnindexedCnt);
      Wam.MaybeSerializeField(26, this.dbMessagesStarredCnt);
      Wam.MaybeSerializeField(27, this.dbMessagesIndexedPct);
      Wam.MaybeSerializeField(71, this.wpBatsaver);
      Wam.MaybeSerializeField(72, this.wpIsPushDaemonConnected);
      Wam.MaybeSerializeField(73, this.wpLastBackup);
      Wam.MaybeSerializeField(74, this.wpScheduled);
      Wam.MaybeSerializeField(75, this.wpVoipExitReason);
      Wam.MaybeSerializeField(76, this.wpVoipException);
    }
  }
}
