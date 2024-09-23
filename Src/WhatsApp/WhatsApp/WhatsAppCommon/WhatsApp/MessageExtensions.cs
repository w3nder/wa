// Decompiled with JetBrains decompiler
// Type: WhatsApp.MessageExtensions
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class MessageExtensions
  {
    private static readonly int MaxPreviewLength = 128;
    public static readonly int MinPossiblyTruncatedPreviewLength = MessageExtensions.MaxPreviewLength - 8;
    private static Dictionary<FunXMPP.FMessage.Status, int> statusOverrideWeightsMapping_ = (Dictionary<FunXMPP.FMessage.Status, int>) null;
    private static int LastFileIndex = 0;

    public static string LogInfo(this Message m, bool includeJid = true)
    {
      return string.Format("msg {0} {1} {2} {3} {4}", (object) (char) (m.KeyFromMe ? 111 : 105), (object) m.KeyId, (object) m.MessageID, (object) m.MediaWaType, includeJid ? (object) "" : (object) m.KeyRemoteJid);
    }

    public static bool IsBroadcasted(this Message m)
    {
      return JidHelper.IsBroadcastJid(m.KeyRemoteJid) || JidHelper.IsBroadcastJid(m.RemoteResource);
    }

    public static bool HasPaymentInfo(this FunXMPP.FMessage fmsg)
    {
      return fmsg.message_properties?.PaymentsPropertiesField != null;
    }

    public static bool HasPaymentInfo(this Message m)
    {
      return m.InternalProperties?.PaymentsPropertiesField != null;
    }

    public static bool IsStatus(this Message m) => JidHelper.IsStatusJid(m.KeyRemoteJid);

    public static bool IsTextStatus(this Message m)
    {
      if (m.MediaWaType != FunXMPP.FMessage.Type.ExtendedText)
        return false;
      MessageProperties internalProperties = m.InternalProperties;
      return ((uint?) internalProperties?.ExtendedTextPropertiesField?.BackgroundArgb).HasValue || ((int?) internalProperties?.ExtendedTextPropertiesField?.Font).HasValue;
    }

    public static bool IsStatusMessageExpired(this Message m)
    {
      if (!m.IsStatus())
        return false;
      DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
      DateTime? funTimestamp = m.FunTimestamp;
      TimeSpan? nullable = funTimestamp.HasValue ? new TimeSpan?(currentServerTimeUtc - funTimestamp.GetValueOrDefault()) : new TimeSpan?();
      TimeSpan expiration = WaStatus.Expiration;
      return nullable.HasValue && nullable.GetValueOrDefault() > expiration;
    }

    public static bool IsSupportedStatusType(this FunXMPP.FMessage.Type mType)
    {
      bool flag = false;
      switch (mType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.ExtendedText:
        case FunXMPP.FMessage.Type.Gif:
          flag = true;
          break;
        case FunXMPP.FMessage.Type.CipherText:
          flag = false;
          break;
      }
      return flag;
    }

    public static bool IsPtt(this Message m)
    {
      return m.MediaWaType == FunXMPP.FMessage.Type.Audio && m.MediaOrigin == "live";
    }

    public static bool IsCoordinateLocation(this Message m)
    {
      return m.MediaWaType == FunXMPP.FMessage.Type.Location && !Message.LocationDetailsUsable(m.LocationDetails);
    }

    public static SystemMessageWrapper.MessageTypes GetSystemMessageType(this Message m)
    {
      return m.MediaWaType != FunXMPP.FMessage.Type.System ? SystemMessageWrapper.MessageTypes.Undefined : (SystemMessageWrapper.MessageTypes) m.BinaryData[0];
    }

    private static bool AutoDownloadIsEnabled(this AutoDownloadSetting setting)
    {
      return (setting & AutoDownloadSetting.Enabled) != 0;
    }

    public static AutoDownloadSetting GetAutoDownloadSetting(this Message m)
    {
      if (m.IsStatus() && JidHelper.IsPsaJid(m.GetSenderJid()))
        return AutoDownloadSetting.Enabled;
      AutoDownloadSetting autoDownloadSetting = AutoDownloadSetting.Disabled;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
          autoDownloadSetting = Settings.AutoDownloadImage;
          break;
        case FunXMPP.FMessage.Type.Audio:
          autoDownloadSetting = Settings.AutoDownloadAudio;
          break;
        case FunXMPP.FMessage.Type.Video:
          autoDownloadSetting = !m.ShouldStartPrefetchingVideo() ? Settings.AutoDownloadVideo : Settings.AutoDownloadImage;
          break;
        case FunXMPP.FMessage.Type.Document:
          autoDownloadSetting = Settings.AutoDownloadDocument;
          break;
        case FunXMPP.FMessage.Type.Gif:
          autoDownloadSetting = m.MediaSize >= 512000L ? Settings.AutoDownloadVideo : Settings.AutoDownloadImage;
          break;
        case FunXMPP.FMessage.Type.Sticker:
          autoDownloadSetting = AutoDownloadSetting.Enabled;
          break;
      }
      return autoDownloadSetting;
    }

    public static bool ShouldStartPrefetchingVideo(this Message m)
    {
      return m.ShouldPrefetchVideo() && !m.PrefetchedVideoExists();
    }

    public static bool ShouldPrefetchVideo(this Message m)
    {
      return m.MediaWaType == FunXMPP.FMessage.Type.Video && !Settings.AutoDownloadVideo.AutoDownloadIsEnabled() && Settings.AutoDownloadImage.AutoDownloadIsEnabled() && !m.LocalFileExists();
    }

    public static bool ShouldAutoDownload(this Message m, MessagesContext db)
    {
      return NativeInterfaces.Misc.GetDiskSpace(Constants.IsoStorePath).FreeBytes >= 100UL && m.MediaWaType != FunXMPP.FMessage.Type.Revoked && (m.IsPtt() && m.MediaSize < 524288L || (!m.IsStatus() || JidHelper.IsPsaJid(m.GetSenderJid()) || ((int) m.InternalProperties?.MediaPropertiesField?.AutoDownloadEligible ?? 0) != 0) && (m.ShouldStartPrefetchingVideo() || m.GetAutoDownloadSetting().AutoDownloadIsEnabled() && (m.LocalFileUri != null || !SuspiciousJid.IsMessageSuspicious(db, m))));
    }

    public static bool ShouldDelayNotifyUntilAutoDownloadAttempt(this Message m, MessagesContext db)
    {
      bool flag = false;
      if (m.IsPtt() && string.IsNullOrEmpty(m.LocalFileUri) && m.ShouldAutoDownload(db))
      {
        PersistentAction[] persistentActions = db.GetPersistentActions(PersistentAction.Types.AutoDownload, m.MessageID.ToString());
        flag = persistentActions.Length == 1 && persistentActions[0].Attempts <= 1;
      }
      return flag;
    }

    public static bool ShouldEnableForward(this Message m)
    {
      return m.Status != FunXMPP.FMessage.Status.NeverSend && m.Status != FunXMPP.FMessage.Status.Error && m.Status != FunXMPP.FMessage.Status.Pending && m.MediaWaType != FunXMPP.FMessage.Type.System && m.MediaWaType != FunXMPP.FMessage.Type.Divider && m.MediaWaType != FunXMPP.FMessage.Type.CipherText && m.MediaWaType != FunXMPP.FMessage.Type.Revoked && (!m.ContainsMediaContent() || m.LocalFileExists()) && !m.HasPaymentInfo();
    }

    public static bool ShouldSetForwardingFlag(this Message m)
    {
      if (!m.ShouldEnableForward())
      {
        Log.l("msg", "Setting forward indication on non forwardable message!!!!!");
        return false;
      }
      return m.MediaWaType != FunXMPP.FMessage.Type.Contact && m.MediaWaType != FunXMPP.FMessage.Type.Sticker;
    }

    public static bool IsViewingSupported(this Message m)
    {
      if (m == null)
        return false;
      bool flag = true;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.CipherText:
        case FunXMPP.FMessage.Type.ProtocolBuffer:
          flag = false;
          break;
      }
      return flag;
    }

    public static bool IsMaxStatusReached(this Message m)
    {
      if (!m.KeyFromMe)
        return true;
      return !m.IsPtt() ? m.Status.IsReadByTarget() : m.Status.IsPlayedByTarget();
    }

    public static bool IsReadByTarget(this Message m) => m.Status.IsReadByTarget();

    public static bool IsRevoked(this Message m) => m.MediaWaType == FunXMPP.FMessage.Type.Revoked;

    public static bool IsPlayedByTarget(this Message m) => m.Status.IsPlayedByTarget();

    public static bool IsDeliveredToServer(this Message m)
    {
      return !m.KeyFromMe || m.Status.IsDeliveredToServer();
    }

    public static bool IsDeliveredToTarget(this Message m)
    {
      return !m.KeyFromMe || m.Status.IsDeliveredToTarget();
    }

    public static bool IsPlayedByTarget(this FunXMPP.FMessage.Status status)
    {
      return status != FunXMPP.FMessage.Status.Error && status.GetOverrideWeight() >= FunXMPP.FMessage.Status.PlayedByTarget.GetOverrideWeight();
    }

    public static bool IsReadByTarget(this FunXMPP.FMessage.Status status)
    {
      return status != FunXMPP.FMessage.Status.Error && status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReadByTarget.GetOverrideWeight();
    }

    public static bool IsDeliveredToServer(this FunXMPP.FMessage.Status status)
    {
      return status != FunXMPP.FMessage.Status.Error && status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReceivedByServer.GetOverrideWeight();
    }

    public static bool IsDeliveredToTarget(this FunXMPP.FMessage.Status status)
    {
      return status != FunXMPP.FMessage.Status.Error && status.GetOverrideWeight() >= FunXMPP.FMessage.Status.ReceivedByTarget.GetOverrideWeight();
    }

    public static bool IsAvailable(this Message msg, SqliteMessagesContext db)
    {
      bool flag = true;
      if (msg.Flags == Message.MessageFlags.Deleted)
        flag = false;
      else if (msg.IsStatus())
      {
        DateTime? funTimestamp = msg.FunTimestamp;
        DateTime dateTime = FunRunner.CurrentServerTimeUtc - WaStatus.Expiration;
        if ((funTimestamp.HasValue ? (funTimestamp.GetValueOrDefault() < dateTime ? 1 : 0) : 0) != 0)
          flag = false;
      }
      else
      {
        Conversation conversation = db.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
        if (conversation == null || conversation.EffectiveFirstMessageID.HasValue && conversation.EffectiveFirstMessageID.Value > msg.MessageID)
          flag = false;
      }
      return flag;
    }

    private static MessageExtensions.NotifyFlags GetNotifyFlags(this Message m)
    {
      MessageExtensions.NotifyFlags notifyFlags = MessageExtensions.NotifyFlags.UpdateChatPreview;
      bool flag = false;
      MessageProperties.CommonProperties.NotifyLevels? notifyLevel = (MessageProperties.CommonProperties.NotifyLevels?) m.InternalProperties?.CommonPropertiesField?.NotifyLevel;
      if (notifyLevel.HasValue)
      {
        switch (notifyLevel.Value)
        {
          case MessageProperties.CommonProperties.NotifyLevels.ExcludeAlertAndBadge:
            notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldIncreaseUnread;
            flag = true;
            break;
          case MessageProperties.CommonProperties.NotifyLevels.ExcludeAlert:
            notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldAlert | MessageExtensions.NotifyFlags.ShouldIncreaseUnread;
            flag = true;
            break;
        }
      }
      if (!flag)
      {
        if (m.MediaWaType == FunXMPP.FMessage.Type.CipherText || m.MediaWaType == FunXMPP.FMessage.Type.ProtocolBuffer)
          notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldIncreaseUnread;
        else if (m.MediaWaType == FunXMPP.FMessage.Type.Revoked)
          notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldUpdatePreviewOnly;
        else if (m.MediaWaType == FunXMPP.FMessage.Type.System)
        {
          switch (m.GetSystemMessageType())
          {
            case SystemMessageWrapper.MessageTypes.ParticipantChange:
              switch (m.GetSystemMessageChangeType())
              {
                case SystemMessageUtils.ParticipantChange.Join:
                case SystemMessageUtils.ParticipantChange.Invite:
                  if (m.GetSenderJid() == Settings.MyJid)
                  {
                    notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldAlert;
                    break;
                  }
                  break;
                case SystemMessageUtils.ParticipantChange.Removed:
                  if (m.GetSenderJid() == Settings.MyJid)
                  {
                    notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
                    break;
                  }
                  break;
              }
              break;
            case SystemMessageWrapper.MessageTypes.SubjectChange:
              notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
              break;
            case SystemMessageWrapper.MessageTypes.BroadcastListCreated:
              notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
              break;
            case SystemMessageWrapper.MessageTypes.GroupCreated:
              notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
              if (m.GetSenderJid() != Settings.MyJid)
              {
                notifyFlags |= MessageExtensions.NotifyFlags.ShouldAlert;
                break;
              }
              break;
            case SystemMessageWrapper.MessageTypes.IdentityChanged:
            case SystemMessageWrapper.MessageTypes.ConversationEncrypted:
            case SystemMessageWrapper.MessageTypes.ConvBizIsVerified:
            case SystemMessageWrapper.MessageTypes.ConvBizIsUnVerified:
            case SystemMessageWrapper.MessageTypes.ConvBizNowStandard:
            case SystemMessageWrapper.MessageTypes.ConvBizNowUnverified:
            case SystemMessageWrapper.MessageTypes.ConvBizNowVerified:
              notifyFlags = MessageExtensions.NotifyFlags.None;
              break;
            case SystemMessageWrapper.MessageTypes.MissedCall:
            case SystemMessageWrapper.MessageTypes.MissedVideoCall:
              notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
              break;
          }
        }
        else if (m.KeyFromMe)
          notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy;
        else
          notifyFlags |= MessageExtensions.NotifyFlags.Noteworthy | MessageExtensions.NotifyFlags.ShouldAlert | MessageExtensions.NotifyFlags.ShouldIncreaseUnread;
      }
      return notifyFlags;
    }

    public static bool ShouldUpdateTile(this Message m)
    {
      if (m.IsMuted())
        return false;
      if (m.IsStatus())
      {
        Log.d(m.LogInfo(), "skip update tile | status update");
        return false;
      }
      return m.IsQualifiedForUnread() || (m.GetNotifyFlags() & MessageExtensions.NotifyFlags.ShouldUpdatePreviewOnly) != 0;
    }

    public static bool IsQualifiedForUnread(this Message m)
    {
      return !m.KeyFromMe && (m.GetNotifyFlags() & MessageExtensions.NotifyFlags.ShouldIncreaseUnread) != 0;
    }

    public static bool ShouldAlert(this Message m)
    {
      if (m.IsAlerted)
      {
        Log.d(m.LogInfo(), "skip alert | already did");
        return false;
      }
      if (m.IsStatus())
      {
        Log.d(m.LogInfo(), "skip alert | status update");
        return false;
      }
      if ((m.GetNotifyFlags() & MessageExtensions.NotifyFlags.ShouldAlert) != MessageExtensions.NotifyFlags.None)
        return !m.IsMuted();
      Log.d(m.LogInfo(), "skip alert | not enough level");
      return false;
    }

    public static bool IsMuted(this Message m)
    {
      bool flag1 = false;
      Pair<bool, bool> pair = m.IsChatOrSenderMuted();
      bool first = pair.First;
      bool second = pair.Second;
      bool flag2 = JidHelper.IsGroupJid(m.KeyRemoteJid);
      if (first)
      {
        flag1 = true;
        Log.l(m.LogInfo(), "skip alert | user muted");
        if (flag2 && !second && (m.HasJidMentioned(Settings.MyJid) || m.IsReplyToJid(Settings.MyJid)))
        {
          flag1 = false;
          Log.l(m.LogInfo(), "ignore mute | got mentioned");
        }
      }
      return flag1;
    }

    public static Pair<bool, bool> IsChatOrSenderMuted(this Message m)
    {
      bool isChatMuted = false;
      bool isSenderMuted = false;
      bool isGroupChat = JidHelper.IsGroupJid(m.KeyRemoteJid);
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        JidInfo jidInfo1 = db.GetJidInfo(m.KeyRemoteJid, CreateOptions.None);
        isChatMuted = jidInfo1 != null && jidInfo1.IsMuted();
        if (!isGroupChat || m.ProtoBuf == null)
          return;
        JidInfo jidInfo2 = db.GetJidInfo(m.GetSenderJid(), CreateOptions.None);
        isSenderMuted = jidInfo2 != null && jidInfo2.IsMuted();
      }));
      return new Pair<bool, bool>(isChatMuted, isSenderMuted);
    }

    public static bool IsReplyToJid(this Message m, string jid)
    {
      return new MessageContextInfoWrapper(m).QuoteAuthorJid == jid;
    }

    public static bool HasJidMentioned(this Message m, string jid)
    {
      bool flag = false;
      if (m.GetMentionedJids().Contains(jid))
        flag = true;
      return flag;
    }

    public static bool ShouldAutoMute(this Message m)
    {
      return m.IsAutomuted && !m.HasJidMentioned(Settings.MyJid) && !m.IsReplyToJid(Settings.MyJid);
    }

    public static bool IsNoteworthy(this Message m)
    {
      return !m.ForceNotNoteworthy && (m.GetNotifyFlags() & MessageExtensions.NotifyFlags.Noteworthy) != 0;
    }

    public static bool ShouldCreateConversation(this Message m) => m.GetNotifyFlags() != 0;

    public static bool ShouldUpdatePreview(this Message m)
    {
      return (m.GetNotifyFlags() & MessageExtensions.NotifyFlags.UpdateChatPreview) != 0;
    }

    public static string GetErrorString(this MessageMiscInfo.MessageError errType, string remoteJid)
    {
      string errorString = (string) null;
      switch (errType)
      {
        case MessageMiscInfo.MessageError.NotAuthorized:
          if (JidHelper.IsGroupJid(remoteJid))
          {
            errorString = AppResources.ErrorNotYourGroup;
            break;
          }
          break;
        case MessageMiscInfo.MessageError.ItemNotFound:
        case MessageMiscInfo.MessageError.FileGone:
          errorString = string.Format(AppResources.MediaDownloadFailureResendNeeded, (object) MessageExtensions.GetSenderDisplayName(remoteJid, false));
          break;
        case MessageMiscInfo.MessageError.NotAdmin:
          if (JidHelper.IsGroupJid(remoteJid))
          {
            errorString = AppResources.AnnouncementOnlyGroupSendMessageNotAdmin;
            break;
          }
          break;
      }
      return errorString;
    }

    public static string GetErrorString(this Message m, MessagesContext db = null)
    {
      if (m.Status != FunXMPP.FMessage.Status.Error)
        return (string) null;
      MessageMiscInfo miscInfo = m.GetMiscInfo((SqliteMessagesContext) db);
      string errorString = (string) null;
      if (miscInfo != null)
      {
        int? errorCode = miscInfo.ErrorCode;
        if (errorCode.HasValue)
        {
          errorCode = miscInfo.ErrorCode;
          errorString = ((MessageMiscInfo.MessageError) errorCode.Value).GetErrorString(m.KeyRemoteJid);
        }
      }
      return errorString;
    }

    public static bool ShouldUpdateStatus(this Message m, FunXMPP.FMessage.Status newStatus)
    {
      return newStatus != m.Status && newStatus.GetOverrideWeight() > m.Status.GetOverrideWeight();
    }

    private static bool ShouldRetryFromError(this Message m, MessagesContext db = null)
    {
      bool flag = false;
      if (m.Status == FunXMPP.FMessage.Status.Error && m.KeyFromMe)
      {
        MessageMiscInfo miscInfo = m.GetMiscInfo((SqliteMessagesContext) db);
        flag = true;
        if (miscInfo != null && miscInfo.ErrorCode.HasValue)
        {
          switch ((MessageMiscInfo.MessageError) miscInfo.ErrorCode.Value)
          {
            case MessageMiscInfo.MessageError.NotAuthorized:
            case MessageMiscInfo.MessageError.NotAdmin:
              flag = false;
              break;
          }
        }
      }
      return flag;
    }

    private static bool FileExists(string localFileUri)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(localFileUri))
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(localFileUri))
        {
          try
          {
            flag = mediaStorage.FileExists(localFileUri);
          }
          catch (Exception ex)
          {
            flag = false;
          }
        }
      }
      return flag;
    }

    public static bool LocalFileExists(this Message m)
    {
      return MessageExtensions.FileExists(m.LocalFileUri);
    }

    public static bool CanRevoke(this Message m)
    {
      return m.KeyFromMe && m.MediaWaType != FunXMPP.FMessage.Type.Revoked && m.MediaWaType != FunXMPP.FMessage.Type.System && m.MediaWaType != FunXMPP.FMessage.Type.Divider && FunRunner.CurrentServerTimeUtc - m.FunTimestamp.GetValueOrDefault(FunRunner.CurrentServerTimeUtc) < Constants.RevokeExpiryTimeout && !m.HasPaymentInfo();
    }

    private static string GetPrefetchedVideoFilePath(this Message m)
    {
      string btsPath;
      return !MediaDownload.GetFileNames(m, out btsPath, out string _) ? (string) null : btsPath;
    }

    public static bool PrefetchedVideoExists(this Message m)
    {
      string prefetchedVideoFilePath = m.GetPrefetchedVideoFilePath();
      return !string.IsNullOrEmpty(prefetchedVideoFilePath) && MessageExtensions.FileExists(prefetchedVideoFilePath);
    }

    public static Stream GetPrefetchedVideoStream(this Message m)
    {
      string prefetchedVideoFilePath = m.GetPrefetchedVideoFilePath();
      using (IMediaStorage mediaStorage = MediaStorage.Create(prefetchedVideoFilePath))
      {
        try
        {
          return mediaStorage.OpenFile(prefetchedVideoFilePath);
        }
        catch (Exception ex)
        {
          return (Stream) null;
        }
      }
    }

    public static bool ContainsMediaContent(this Message m)
    {
      bool flag = false;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Document:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          flag = true;
          break;
      }
      return flag;
    }

    public static bool ShouldSaveMedia(this Message m, MessagesContext db)
    {
      if (m.IsStatus())
        return false;
      if (m.MediaWaType != FunXMPP.FMessage.Type.Image && m.MediaWaType != FunXMPP.FMessage.Type.Video)
      {
        if (m.MediaWaType == FunXMPP.FMessage.Type.Audio || m.MediaWaType == FunXMPP.FMessage.Type.Document)
          return true;
        if (m.MediaWaType == FunXMPP.FMessage.Type.Gif)
          return false;
        int mediaWaType = (int) m.MediaWaType;
        return false;
      }
      JidInfo jidInfo = db.GetJidInfo(m.KeyRemoteJid, CreateOptions.None);
      return jidInfo != null && jidInfo.SaveMediaToPhone.HasValue ? jidInfo.SaveMediaToPhone.Value : Settings.SaveIncomingMedia;
    }

    public static bool CanSaveMedia(this Message m, bool checkFileExists = true)
    {
      if (m == null || checkFileExists && !m.LocalFileExists())
        return false;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Audio:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Document:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          return true;
        default:
          return false;
      }
    }

    public static bool CopyMediaToAlbum(this Message msg, MessagesContext db, string dstAlbum)
    {
      bool album = false;
      try
      {
        if (msg.LocalFileExists())
        {
          bool flag = (MediaStorage.AnalyzePath(msg.LocalFileUri).Root & FileRoot.StorageMask) == FileRoot.IsoStore;
          album = MediaDownload.SaveMedia(msg.LocalFileUri, msg.MediaWaType, db: db, isPtt: msg.IsPtt(), saveAlbum: dstAlbum, duplicatingFile: !flag) != null;
        }
      }
      catch (Exception ex)
      {
        Log.l(ex, "copy media to album");
      }
      return album;
    }

    public static bool SaveSticker(this Message msg, MessagesContext db)
    {
      bool flag = false;
      try
      {
        Sticker savedSticker = msg.GetSavedSticker(db);
        if (msg.MediaHash != null)
        {
          if (savedSticker == null)
          {
            if (msg.LocalFileExists())
            {
              if (msg.InternalProperties != null)
              {
                MessageProperties.MediaProperties ensureMediaProperties = msg.InternalProperties.EnsureMediaProperties;
                MessageProperties.CommonProperties commonProperties = msg.InternalProperties.EnsureCommonProperties;
                Sticker sticker = new Sticker();
                uint? nullable = (uint?) ensureMediaProperties?.Width;
                sticker.Width = (int) nullable ?? 0;
                nullable = (uint?) ensureMediaProperties?.Height;
                sticker.Height = (int) nullable ?? 0;
                sticker.Url = msg.MediaUrl;
                sticker.MimeType = msg.MediaMimeType;
                sticker.FileHash = msg.MediaHash;
                sticker.MediaKey = msg.MediaKey;
                sticker.FileLength = msg.MediaSize;
                sticker.EncodedFileHash = commonProperties.CipherMediaHash;
                sticker.DateTimeStarred = new DateTime?(FunRunner.CurrentServerTimeUtc);
                sticker.LocalFileUri = msg.LocalFileUri;
                MessageExtensions.SaveStickerToDb(sticker, db);
                flag = true;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.l(ex, "saving sticker");
      }
      return flag;
    }

    private static void SaveStickerToDb(Sticker sticker, MessagesContext db)
    {
      db.SaveSticker(sticker);
      db.LocalFileAddRef(sticker.LocalFileUri, LocalFileType.Sticker);
      db.SubmitChanges(true);
    }

    public static void MoveMediaFromIsoStoreToAlbum(this Message msg)
    {
      if (msg.LocalFileUri == null || NativeMediaStorage.UriApplicable(msg.LocalFileUri))
        return;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        string uri = MediaDownload.SaveMedia(msg.LocalFileUri, msg.MediaWaType, db: db, isPtt: msg.IsPtt());
        if (uri == null || !(uri != msg.LocalFileUri))
          return;
        string localFileUri = msg.LocalFileUri;
        db.GetLocalFileByUri(localFileUri);
        msg.LocalFileUri = uri;
        db.LocalFileRelease(localFileUri, LocalFileType.MessageMedia);
        db.LocalFileAddRef(uri, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
        db.SubmitChanges();
      }));
    }

    public static MessageExtensions.MediaUploadActionState GetUploadActionState(Message msg)
    {
      MessageExtensions.MediaUploadActionState uploadActionState = MessageExtensions.MediaUploadActionState.Unknown;
      if (msg != null && msg.KeyFromMe && msg.MediaOrigin != "live" && msg.ContainsMediaContent())
      {
        uploadActionState = MessageExtensions.MediaUploadActionState.None;
        switch (msg.Status)
        {
          case FunXMPP.FMessage.Status.Uploading:
          case FunXMPP.FMessage.Status.UploadingCustomHash:
          case FunXMPP.FMessage.Status.Pending:
            uploadActionState = MessageExtensions.MediaUploadActionState.Cancellable;
            break;
          case FunXMPP.FMessage.Status.Error:
            if (msg.ShouldRetryFromError())
            {
              uploadActionState = MessageExtensions.MediaUploadActionState.Retryable;
              break;
            }
            break;
          case FunXMPP.FMessage.Status.Canceled:
            uploadActionState = MessageExtensions.MediaUploadActionState.Retryable;
            break;
        }
      }
      return uploadActionState;
    }

    public static bool HasText(this Message m)
    {
      if (m == null)
        return false;
      if ((m.MediaWaType == FunXMPP.FMessage.Type.Undefined || m.MediaWaType == FunXMPP.FMessage.Type.ExtendedText) && m.Data != null || m.MediaWaType != FunXMPP.FMessage.Type.Revoked && m.MediaCaption != null)
        return true;
      return m.MediaWaType == FunXMPP.FMessage.Type.LiveLocation && m.InternalProperties?.LiveLocationPropertiesField?.Caption != null;
    }

    public static string GetPreviewText(
      this Message m,
      bool oneLine,
      bool forExternalDisplay,
      bool isQuotePreview = false,
      bool actionable = false)
    {
      LinkDetector.Result[] formats = (LinkDetector.Result[]) null;
      return m.GetPreviewText(out formats, oneLine, forExternalDisplay, isQuotePreview, actionable);
    }

    public static string GetPreviewText(
      this Message m,
      out LinkDetector.Result[] formats,
      bool oneLine,
      bool forExternalDisplay,
      bool isQuotePreview = false,
      bool actionable = false)
    {
      formats = (LinkDetector.Result[]) null;
      if (m.MediaWaType == FunXMPP.FMessage.Type.CipherText)
        return AppResources.MessagePendingDecrypt;
      if (!m.IsViewingSupported())
        return AppResources.UnsupportedMessage;
      string previewText = (string) null;
      switch (m.MediaWaType)
      {
        case FunXMPP.FMessage.Type.Image:
        case FunXMPP.FMessage.Type.Video:
        case FunXMPP.FMessage.Type.Gif:
        case FunXMPP.FMessage.Type.Sticker:
          if (m.MediaCaption != null)
          {
            previewText = forExternalDisplay ? Emoji.ConvertToTextOnly(m.MediaCaption, (byte[]) null) : m.MediaCaption;
            formats = forExternalDisplay ? (LinkDetector.Result[]) null : m.GetRichTextFormattings(true, true);
            break;
          }
          switch (m.MediaWaType)
          {
            case FunXMPP.FMessage.Type.Image:
              previewText = AppResources.MediaImage;
              break;
            case FunXMPP.FMessage.Type.Video:
              previewText = AppResources.MediaVideo;
              break;
            case FunXMPP.FMessage.Type.Gif:
              previewText = AppResources.MediaGif;
              break;
            case FunXMPP.FMessage.Type.Sticker:
              previewText = isQuotePreview ? (string) null : AppResources.MediaSticker;
              break;
          }
          break;
        case FunXMPP.FMessage.Type.Audio:
          if (m.IsPtt())
          {
            string str = DateTimeUtils.FormatDuration(m.MediaDurationSeconds);
            previewText = oneLine ? str : string.Format("{0} {1}", (object) AppResources.MediaVoiceMessage, (object) str);
            break;
          }
          previewText = AppResources.MediaAudio;
          break;
        case FunXMPP.FMessage.Type.Contact:
          previewText = m.MediaName ?? AppResources.MediaContact;
          break;
        case FunXMPP.FMessage.Type.Location:
          if (m.IsCoordinateLocation())
          {
            previewText = AppResources.MediaLocation;
            break;
          }
          if (oneLine)
          {
            string name = m.ParsePlaceDetails()?.Name;
            previewText = string.IsNullOrEmpty(name) ? AppResources.MediaLocation : name;
            break;
          }
          previewText = m.LocationDetails;
          break;
        case FunXMPP.FMessage.Type.System:
          previewText = m.GetSystemMessage(actionable: actionable);
          if (forExternalDisplay)
          {
            previewText = Emoji.ConvertToTextOnly(previewText, (byte[]) null);
            break;
          }
          break;
        case FunXMPP.FMessage.Type.Document:
          DocumentMessageWrapper documentMessageWrapper = new DocumentMessageWrapper(m);
          string str1 = documentMessageWrapper.PageCount > 0 ? Plurals.Instance.GetString(AppResources.DocPagesPlural, documentMessageWrapper.PageCount) : (string) null;
          previewText = string.Join(string.Format("{0}•{1}", (object) ' ', (object) ' '), ((IEnumerable<string>) new string[2]
          {
            documentMessageWrapper.Title,
            str1
          }).Where<string>((Func<string, bool>) (s => !string.IsNullOrEmpty(s))));
          break;
        case FunXMPP.FMessage.Type.LiveLocation:
          string caption = m.InternalProperties?.LiveLocationPropertiesField?.Caption;
          previewText = string.IsNullOrEmpty(caption) ? AppResources.LiveLocation : caption;
          break;
        case FunXMPP.FMessage.Type.Revoked:
          previewText = m.KeyFromMe ? AppResources.SentDeleted : AppResources.ReceivedDeleted;
          break;
        default:
          previewText = m.Data;
          bool flag1 = false;
          if (forExternalDisplay)
          {
            previewText = Emoji.ConvertToTextOnly(previewText, m.TextPerformanceHint);
            flag1 = true;
          }
          List<string> mentionedJids = m.GetMentionedJids();
          if (mentionedJids.Any<string>())
          {
            previewText = WaRichText.FormatMentions(previewText, mentionedJids);
            flag1 = true;
          }
          if (!flag1)
          {
            formats = m.GetRichTextFormattings(true, true);
            break;
          }
          break;
      }
      if (previewText != null)
      {
        if (previewText.Length > MessageExtensions.MaxPreviewLength)
        {
          previewText = Utils.TruncateAtIndex(previewText, MessageExtensions.MaxPreviewLength);
          if (formats != null && formats.Length != 0)
          {
            Log.d("GetSubtitle", "last message length {0}, count {1}", (object) previewText.Length, (object) formats.Length);
            List<LinkDetector.Result> resultList = new List<LinkDetector.Result>();
            bool flag2 = true;
            for (int index = 0; flag2 && index < formats.Length; ++index)
            {
              LinkDetector.Result result = ((IEnumerable<LinkDetector.Result>) formats).ElementAt<LinkDetector.Result>(index);
              if (result.Index < previewText.Length)
                resultList.Add(result);
              else
                flag2 = false;
            }
            formats = resultList.ToArray();
          }
        }
        previewText = NotificationString.ReplaceNewlines(previewText);
      }
      return previewText;
    }

    private static string GetSenderJidImpl(string remoteJid, string participantJid, bool fromMe)
    {
      string senderJidImpl = (string) null;
      if (fromMe)
      {
        senderJidImpl = Settings.MyJid;
      }
      else
      {
        switch (JidHelper.GetJidType(remoteJid))
        {
          case JidHelper.JidTypes.User:
          case JidHelper.JidTypes.Psa:
            senderJidImpl = remoteJid;
            break;
          case JidHelper.JidTypes.Group:
          case JidHelper.JidTypes.Broadcast:
          case JidHelper.JidTypes.Status:
            senderJidImpl = participantJid;
            break;
          default:
            Log.l("msg", "invalid sender jid | remote_res:{0} | remote_jid:{1}", (object) participantJid, (object) (remoteJid ?? "null"));
            break;
        }
      }
      return senderJidImpl;
    }

    public static string GetSenderJid(this FunXMPP.FMessage m)
    {
      return MessageExtensions.GetSenderJidImpl(m.key.remote_jid, m.remote_resource, m.key.from_me);
    }

    public static string GetSenderJid(this Message m)
    {
      return MessageExtensions.GetSenderJidImpl(m.KeyRemoteJid, m.RemoteResource, m.KeyFromMe);
    }

    public static string GetSenderDisplayName(this Message m, bool usePushNameForUnknownContact)
    {
      return MessageExtensions.GetSenderDisplayName(m.GetSenderJid(), usePushNameForUnknownContact, m.PushName);
    }

    private static string GetSenderDisplayName(
      string senderJid,
      bool usePushNameForUnknownContact,
      string pushName = null)
    {
      if (JidHelper.IsPsaJid(senderJid))
        return Constants.OffcialName;
      UserStatus userStatus = string.IsNullOrEmpty(senderJid) ? (UserStatus) null : UserCache.Get(senderJid, false);
      if (userStatus == null || userStatus.ContactName == null & usePushNameForUnknownContact && pushName != null)
        return Emoji.ConvertToTextOnly(pushName, (byte[]) null);
      return userStatus != null ? userStatus.GetDisplayName() : JidHelper.GetPhoneNumber(senderJid, true);
    }

    private static Dictionary<FunXMPP.FMessage.Status, int> GetStatusOverrideWeightsMapping()
    {
      Dictionary<FunXMPP.FMessage.Status, int> overrideWeightsMapping1 = MessageExtensions.statusOverrideWeightsMapping_;
      if (overrideWeightsMapping1 != null)
        return overrideWeightsMapping1;
      Dictionary<FunXMPP.FMessage.Status, int> overrideWeightsMapping2 = new Dictionary<FunXMPP.FMessage.Status, int>();
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Error, -9);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.NeverSend, -2);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Canceled, -1);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Undefined, 0);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Pending, 1);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Uploading, 2);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.UploadingCustomHash, 2);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Uploaded, 3);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Unsent, 4);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Relay, 4);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.SentByClient, 5);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.ReceivedByServer, 6);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.ReceivedByTarget, 7);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.Downloading, 8);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.ReadByTarget, 9);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.ObsoleteReadByTargetAcked, 9);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.PlayedByTarget, 11);
      overrideWeightsMapping2.Add(FunXMPP.FMessage.Status.ObsoletePlayedByTargetAcked, 11);
      MessageExtensions.statusOverrideWeightsMapping_ = overrideWeightsMapping2;
      return overrideWeightsMapping2;
    }

    public static int GetOverrideWeight(this FunXMPP.FMessage.Status status)
    {
      int num = 0;
      return !MessageExtensions.GetStatusOverrideWeightsMapping().TryGetValue(status, out num) ? -99 : num;
    }

    public static Stream GetImageStream(this Message m)
    {
      if (m == null || m.MediaWaType != FunXMPP.FMessage.Type.Image && m.MediaWaType != FunXMPP.FMessage.Type.Sticker)
        return (Stream) null;
      MessageMiscInfo miscInfo = m.GetMiscInfo();
      string str = miscInfo == null ? (string) null : miscInfo.AlternateUploadUri;
      if (string.IsNullOrEmpty(str))
      {
        str = m.LocalFileUri;
        if (string.IsNullOrEmpty(str))
          return (Stream) null;
      }
      Stream d = (Stream) null;
      using (IMediaStorage mediaStorage = MediaStorage.Create(str))
      {
        try
        {
          d = mediaStorage.OpenFile(str);
        }
        catch (Exception ex)
        {
          d.SafeDispose();
          d = (Stream) null;
          if (mediaStorage.FileExists(str))
            Log.LogException(ex, "open stored image");
          else
            Log.l(m.LogInfo(), "get msg image | file not found | path:{0}", (object) str);
        }
      }
      return d;
    }

    public static IObservable<Stream> GetImageStreamObservable(this Message m)
    {
      return Observable.Create<Stream>((Func<IObserver<Stream>, Action>) (observer =>
      {
        observer.OnNext(m.GetImageStream());
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static BitmapSource GetThumbnail(
      this Message m,
      MessageExtensions.ThumbPreference thumbPref = MessageExtensions.ThumbPreference.None)
    {
      return m.GetThumbnail(0, 0, thumbPref);
    }

    public static BitmapSource GetThumbnail(
      this Message m,
      int maxPixelWidth,
      int maxPixelHeight,
      MessageExtensions.ThumbPreference thumbPref)
    {
      if (m == null)
        return (BitmapSource) null;
      BitmapSource thumbnail = (BitmapSource) null;
      switch (thumbPref)
      {
        case MessageExtensions.ThumbPreference.PreferSmall:
          if (m.BinaryData != null)
          {
            thumbnail = (BitmapSource) BitmapUtils.CreateBitmap(m.BinaryData, maxPixelWidth, maxPixelHeight);
            goto default;
          }
          else
            goto default;
        case MessageExtensions.ThumbPreference.OnlySmall:
          thumbnail = m.BinaryData == null ? (BitmapSource) null : (BitmapSource) BitmapUtils.CreateBitmap(m.BinaryData, maxPixelWidth, maxPixelHeight);
          break;
        default:
          if (thumbnail == null && m.DataFileName != null)
            thumbnail = (BitmapSource) BitmapUtils.LoadFromFile(m.DataFileName, maxPixelWidth, maxPixelHeight);
          if (thumbnail == null)
          {
            if (m.MediaWaType == FunXMPP.FMessage.Type.Contact)
            {
              IEnumerable<ContactVCard> source = (IEnumerable<ContactVCard>) ((object) m.GetContactCards() ?? (object) new ContactVCard[0]);
              if (source.Count<ContactVCard>() == 1)
                thumbnail = (BitmapSource) source.FirstOrDefault<ContactVCard>().GetPhotoBitmap();
            }
            if (thumbnail == null && m.BinaryData != null)
            {
              thumbnail = (BitmapSource) BitmapUtils.CreateBitmap(m.BinaryData, maxPixelWidth, maxPixelHeight);
              break;
            }
            break;
          }
          break;
      }
      return thumbnail;
    }

    public static MemoryStream GetThumbnailStream(
      this Message m,
      bool preferSmallSize,
      out bool isLargeSize)
    {
      isLargeSize = false;
      if (m == null)
        return (MemoryStream) null;
      if (preferSmallSize && m.BinaryData != null)
        return new MemoryStream(m.BinaryData);
      MemoryStream destination = (MemoryStream) null;
      if (m.MediaWaType == FunXMPP.FMessage.Type.Sticker)
      {
        if (m.LocalFileUri != null)
        {
          try
          {
            using (IMediaStorage mediaStorage = MediaStorage.Create(m.LocalFileUri))
            {
              if (mediaStorage.FileExists(m.LocalFileUri))
              {
                using (Stream stream = mediaStorage.OpenFile(m.LocalFileUri))
                {
                  destination = new MemoryStream();
                  stream.CopyTo((Stream) destination);
                  isLargeSize = true;
                }
                Log.d(m.LogInfo(), "got sticker stream | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.LocalFileUri);
              }
              else
              {
                Log.d(m.LogInfo(), "sticker file not found | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.LocalFileUri);
                MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
                {
                  db.LocalFileRelease(m.LocalFileUri, LocalFileType.Sticker);
                  m.LocalFileUri = (string) null;
                  db.SubmitChanges();
                }));
              }
            }
          }
          catch (Exception ex)
          {
            Log.l(m.LogInfo(), "get thumb stream failed | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.LocalFileUri);
            Log.LogException(ex, "open thumb file");
            destination = (MemoryStream) null;
          }
        }
      }
      else if (m.DataFileName != null)
      {
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (storeForApplication.FileExists(m.DataFileName))
            {
              using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(m.DataFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
              {
                destination = new MemoryStream();
                storageFileStream.CopyTo((Stream) destination);
                isLargeSize = true;
              }
              Log.d(m.LogInfo(), "got thumb stream | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.DataFileName);
            }
            else
              Log.d(m.LogInfo(), "datafile not found | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.DataFileName);
          }
        }
        catch (Exception ex)
        {
          Log.l(m.LogInfo(), "get thumb stream failed | type:{0},filepath:{1}", (object) m.MediaWaType, (object) m.DataFileName);
          Log.LogException(ex, "open thumb file");
          destination = (MemoryStream) null;
        }
      }
      if (destination == null)
      {
        if (m.MediaWaType == FunXMPP.FMessage.Type.Contact)
        {
          IEnumerable<ContactVCard> source = (IEnumerable<ContactVCard>) ((object) m.GetContactCards() ?? (object) new ContactVCard[0]);
          if (source.Count<ContactVCard>() == 1)
          {
            ContactVCard contactVcard = source.FirstOrDefault<ContactVCard>();
            if (contactVcard.Photo != null)
            {
              try
              {
                byte[] buffer = Convert.FromBase64String(contactVcard.Photo);
                if (buffer != null)
                {
                  if (buffer.Length != 0)
                    destination = new MemoryStream(buffer);
                }
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
        if (destination == null && m.BinaryData != null)
          destination = new MemoryStream(m.BinaryData);
      }
      return destination;
    }

    public static int GetExpectedDeliveryCount(this Message msg, MessagesContext db = null)
    {
      int expectedDeliveryCount = (int?) msg.InternalProperties?.CommonPropertiesField?.AckedRecipientsCount ?? 0;
      if (expectedDeliveryCount == 0)
      {
        MessageMiscInfo miscInfo = msg.GetMiscInfo((SqliteMessagesContext) db);
        expectedDeliveryCount = miscInfo != null ? miscInfo.ExpectedDeliveryCount : 0;
      }
      if (expectedDeliveryCount == 0)
      {
        if (JidHelper.IsBroadcastJid(msg.KeyRemoteJid))
          expectedDeliveryCount = Settings.MaxListRecipients;
        else if (JidHelper.IsGroupJid(msg.KeyRemoteJid))
          expectedDeliveryCount = Settings.MaxGroupParticipants;
        Log.l(msg.LogInfo(), "missing expected recipient count");
      }
      return expectedDeliveryCount;
    }

    public static bool SaveBinaryDataFile(this Message m, byte[] data)
    {
      string filepath = (string) null;
      bool flag = false;
      string str1 = string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) MediaDownload.SanitizeIsoStorePath(MediaDownload.GetDirectoryPath()));
      Stream stream = (Stream) null;
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        string str2 = m.KeyId.Replace('/', '_');
        string str3 = string.Format("{0}\\bin{1}-{2}-{3}", (object) str1, (object) m.KeyRemoteJid.GetHashCode(), (object) str2, m.KeyFromMe ? (object) "o" : (object) "i");
        filepath = str3;
        MediaDownload.FilenameSearchRetryState retryState = new MediaDownload.FilenameSearchRetryState();
        int lastFileIndex = MessageExtensions.LastFileIndex;
        while (stream == null)
        {
          LocalFile existingFile = (LocalFile) null;
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => existingFile = db.GetLocalFileByUri(filepath)));
          if (existingFile == null)
          {
            try
            {
              stream = nativeMediaStorage.OpenFile(filepath, FileMode.CreateNew, FileAccess.Write);
              MessageExtensions.LastFileIndex = lastFileIndex + 1;
              break;
            }
            catch (Exception ex)
            {
              if (!MediaDownload.ShouldContinueFilenameSearch(ex, retryState))
              {
                Log.LogException(ex, "filename search");
                break;
              }
            }
          }
          filepath = string.Format("{0}-{1}", (object) str3, (object) lastFileIndex);
          ++lastFileIndex;
        }
      }
      if (stream != null)
      {
        try
        {
          using (stream)
          {
            stream.Write(data, 0, data.Length);
            flag = true;
          }
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "write binary data file");
        }
      }
      if (flag)
      {
        m.DataFileName = filepath;
        Log.d(m.LogInfo(), "saved binary data | path={0}", (object) m.DataFileName);
      }
      return flag;
    }

    public static bool HasHashMismatchError(this Message msg, MessagesContext db = null)
    {
      MessageMiscInfo miscInfo = msg.GetMiscInfo((SqliteMessagesContext) db);
      return miscInfo != null && miscInfo.ErrorCode.HasValue && miscInfo.ErrorCode.Value == 10;
    }

    public static FunXMPP.FMessage ToFMessage(this Message msg)
    {
      FunXMPP.FMessage fmessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId));
      fmessage.status = msg.Status;
      fmessage.remote_resource = msg.RemoteResource;
      fmessage.wants_receipt = msg.WantsReceipt;
      fmessage.data = msg.Data;
      fmessage.binary_data = msg.BinaryData;
      fmessage.timestamp = msg.FunTimestamp;
      fmessage.media_url = msg.MediaKey != null ? msg.MediaUrl : (string.IsNullOrEmpty(msg.MediaUploadUrl) ? msg.MediaUrl : msg.MediaUploadUrl);
      fmessage.media_mime_type = msg.MediaMimeType;
      fmessage.media_wa_type = msg.MediaWaType;
      fmessage.media_size = msg.MediaSize;
      fmessage.media_duration_seconds = msg.MediaDurationSeconds;
      fmessage.media_name = msg.MediaName;
      fmessage.media_ip = msg.MediaIp;
      fmessage.latitude = msg.Latitude;
      fmessage.longitude = msg.Longitude;
      fmessage.details = msg.LocationDetails;
      fmessage.location_url = msg.LocationUrl;
      fmessage.media_origin = msg.MediaOrigin;
      fmessage.media_caption = msg.MediaCaption;
      fmessage.push_name = msg.PushName;
      fmessage.participants_hash = msg.ParticipantsHash;
      fmessage.media_key = msg.MediaKey;
      fmessage.media_hash = msg.MediaHash;
      fmessage.proto_buf = msg.ProtoBuf;
      fmessage.web_relay = msg.NeedsWebRelay();
      fmessage.multicast = msg.IsMulticast();
      fmessage.urlPhoneNumber = msg.IsC2cPhoneNumber();
      fmessage.urlText = msg.IsC2cText();
      fmessage.message_properties = msg.InternalProperties;
      if (msg.InternalProperties?.BizPropertiesField != null)
      {
        MessageProperties.BizProperties bizPropertiesField = msg.InternalProperties.BizPropertiesField;
        fmessage.verified_level = bizPropertiesField.Level;
        fmessage.verified_name = bizPropertiesField.Serial;
        fmessage.verified_name_certificate = bizPropertiesField.Cert;
        msg.InternalProperties.BizPropertiesField = (MessageProperties.BizProperties) null;
      }
      return fmessage;
    }

    public static void CopyFrom(
      this Message msg,
      SqliteMessagesContext db,
      Message src,
      bool copyMiscInfo,
      bool increaseLocalFileRef)
    {
      msg.KeyRemoteJid = src.KeyRemoteJid;
      msg.KeyFromMe = src.KeyFromMe;
      msg.KeyId = src.KeyId;
      msg.Status = src.Status;
      msg.RemoteResource = src.RemoteResource;
      msg.WantsReceipt = src.WantsReceipt;
      msg.Data = src.Data;
      msg.BinaryData = src.BinaryData;
      msg.DataFileName = src.DataFileName;
      msg.TimestampLong = src.TimestampLong;
      msg.CreationTimeLong = src.CreationTimeLong;
      msg.PushName = src.PushName;
      msg.MediaUrl = src.MediaUrl;
      msg.MediaIp = src.MediaIp;
      msg.MediaMimeType = src.MediaMimeType;
      msg.MediaWaType = src.MediaWaType;
      msg.MediaSize = src.MediaSize;
      msg.MediaDurationSeconds = src.MediaDurationSeconds;
      msg.MediaOrigin = src.MediaOrigin;
      msg.MediaName = src.MediaName;
      msg.MediaHash = src.MediaHash;
      msg.MediaIp = src.MediaIp;
      msg.MediaCaption = src.MediaCaption;
      msg.MediaKey = src.MediaKey;
      msg.Latitude = src.Latitude;
      msg.Longitude = src.Longitude;
      msg.LocalFileUri = src.LocalFileUri;
      msg.LocationDetails = src.LocationDetails;
      msg.LocationUrl = src.LocationUrl;
      msg.TextPerformanceHint = src.TextPerformanceHint;
      msg.TextSplittingHint = src.TextSplittingHint;
      msg.IsStarred = src.IsStarred;
      msg.Flags = src.Flags;
      msg.ProtoBuf = src.ProtoBuf;
      msg.InternalProperties = src.InternalProperties;
      msg.QuotedMediaFileUri = src.QuotedMediaFileUri;
      if (copyMiscInfo)
      {
        MessageMiscInfo miscInfo1 = src.GetMiscInfo(db);
        if (miscInfo1 != null)
        {
          msg.SetMiscInfo(miscInfo1.CreateCopy());
          if (increaseLocalFileRef)
          {
            MessageMiscInfo miscInfo2 = msg.GetMiscInfo();
            if (miscInfo2 != null && miscInfo2.AlternateUploadUri != null)
              db.LocalFileAddRef(miscInfo2.AlternateUploadUri, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
          }
        }
      }
      if (!increaseLocalFileRef)
        return;
      if (src.LocalFileUri != null)
        db.LocalFileAddRef(src.LocalFileUri, msg.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia);
      if (src.DataFileName != null)
        db.LocalFileAddRef(src.DataFileName, LocalFileType.Thumbnail);
      if (src.QuotedMediaFileUri == null)
        return;
      db.LocalFileAddRef(src.DataFileName, LocalFileType.QuotedMedia);
    }

    public static Message CreateForwardMessage(
      this Message src,
      string remoteJid,
      MessagesContext db)
    {
      if (remoteJid == null)
        throw new ArgumentNullException("remoteJid is null");
      Message forwardMessage = new Message(false);
      forwardMessage.CopyFrom((SqliteMessagesContext) db, src, false, true);
      forwardMessage.KeyRemoteJid = remoteJid;
      forwardMessage.KeyFromMe = true;
      forwardMessage.KeyId = FunXMPP.GenerateMessageId();
      forwardMessage.RemoteResource = (string) null;
      forwardMessage.WantsReceipt = false;
      if (forwardMessage.MediaWaType != FunXMPP.FMessage.Type.ExtendedText)
        forwardMessage.MediaCaption = (string) null;
      forwardMessage.FunTimestamp = new DateTime?(FunRunner.CurrentServerTimeUtc);
      forwardMessage.MediaOrigin = (string) null;
      forwardMessage.PushName = (string) null;
      forwardMessage.IsStarred = false;
      forwardMessage.Flags = Message.MessageFlags.None;
      bool flag1 = false;
      int num = src.KeyFromMe ? 1 : 0;
      if (forwardMessage.ProtoBuf != null)
      {
        WhatsApp.ProtoBuf.Message fromPlainText = WhatsApp.ProtoBuf.Message.CreateFromPlainText(forwardMessage.ProtoBuf);
        ContextInfo contextInfo = fromPlainText?.GetContextInfo();
        if (contextInfo != null)
        {
          flag1 = ((int) contextInfo.IsForwarded ?? 0) != 0;
          bool flag2 = false;
          if (contextInfo.MentionedJid == null || !contextInfo.MentionedJid.Any<string>() || JidHelper.IsUserJid(remoteJid))
          {
            flag2 = true;
          }
          else
          {
            contextInfo.QuotedMessage = (WhatsApp.ProtoBuf.Message) null;
            contextInfo.Participant = (string) null;
            contextInfo.StanzaId = (string) null;
            contextInfo.ConversionSource = (string) null;
            contextInfo.ConversionData = (byte[]) null;
            Conversation conversation = db.GetConversation(remoteJid, CreateOptions.None);
            if (conversation != null)
            {
              conversation.ParticipantSetAction((Action<GroupParticipants>) (participants => contextInfo.MentionedJid = contextInfo.MentionedJid.Where<string>((Func<string, bool>) (mentionedJid => participants.ContainsKey(mentionedJid))).ToList<string>()));
              flag2 = !contextInfo.MentionedJid.Any<string>();
            }
          }
          if (flag2)
            fromPlainText.RemoveContextInfo();
          forwardMessage.ProtoBuf = fromPlainText.ToPlainText();
        }
      }
      bool flag3 = num == 0;
      if ((num & (flag1 ? 1 : 0)) != 0)
        flag3 = true;
      if (flag3)
        flag3 = forwardMessage.ShouldSetForwardingFlag();
      WhatsApp.ProtoBuf.Message message = WhatsApp.ProtoBuf.Message.CreateFromPlainText(forwardMessage.ProtoBuf);
      if (message == null & flag3)
        message = WhatsApp.ProtoBuf.Message.CreateFromFMessage(forwardMessage.ToFMessage(), new CipherTextIncludes(true));
      if (message != null)
      {
        message.SetForwardedFlag(flag3);
        forwardMessage.ProtoBuf = message.ToPlainText();
      }
      MessageProperties forMessage = MessageProperties.GetForMessage(forwardMessage);
      forMessage.EnsureCommonProperties.ForwardedFlag = new bool?(true);
      forMessage.Save();
      if (src.ContainsMediaContent())
      {
        forwardMessage.Status = FunXMPP.FMessage.Status.Uploading;
        if (!forwardMessage.LocalFileExists() && forwardMessage.MediaKey == null)
          throw new FunXMPP.Connection.UnforwardableMessageException();
      }
      else
        forwardMessage.Status = FunXMPP.FMessage.Status.Unsent;
      return forwardMessage;
    }

    public static void SetMentionedJids(this Message m, string[] jids)
    {
      if (m == null)
        return;
      string[] jids1 = jids == null || !((IEnumerable<string>) jids).Any<string>() ? new string[0] : ((IEnumerable<string>) jids).Where<string>((Func<string, bool>) (jid => JidHelper.IsUserJid(jid))).ToArray<string>();
      FunXMPP.FMessage fmessage = m.ToFMessage();
      WhatsApp.ProtoBuf.Message message = m.ProtoBuf != null ? WhatsApp.ProtoBuf.Message.CreateFromPlainText(m.ProtoBuf) : WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmessage, new CipherTextIncludes(true));
      if (message == null)
        return;
      message.SetMentionedJids(jids1);
      m.ProtoBuf = message.ToPlainText();
    }

    public static void SetQuote(this Message m, Message quotedMsg, string jid = null)
    {
      if (m == null)
        return;
      WhatsApp.ProtoBuf.Message message = (WhatsApp.ProtoBuf.Message) null;
      FunXMPP.FMessage fmessage = m.ToFMessage();
      if (m.ProtoBuf == null)
      {
        if (quotedMsg != null || !string.IsNullOrEmpty(jid))
          message = WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmessage, new CipherTextIncludes(true));
      }
      else
        message = WhatsApp.ProtoBuf.Message.CreateFromPlainText(m.ProtoBuf);
      if (message == null)
        return;
      if (quotedMsg != null)
        message.SetQuote(quotedMsg.ToFMessage(), quotedMsg.GetSenderJid(), quotedMsg.KeyRemoteJid, m.KeyRemoteJid);
      else if (!string.IsNullOrEmpty(jid))
        message.SetQuote((FunXMPP.FMessage) null, (string) null, jid, m.KeyRemoteJid);
      else
        message.SetQuote((FunXMPP.FMessage) null, (string) null, (string) null, (string) null);
      m.ProtoBuf = message.ToPlainText();
    }

    public static List<string> GetMentionedJids(this Message msg)
    {
      return new MessageContextInfoWrapper(msg).MentionedJids;
    }

    public static WaRichText.Chunk[] GetMentionChunks(this Message msg)
    {
      WaRichText.Chunk[] chunkArray = (WaRichText.Chunk[]) null;
      List<string> mentionedJids = msg.GetMentionedJids();
      if (mentionedJids.Any<string>())
        chunkArray = WaRichText.GetMentionChunks(msg.GetTextForDisplay(), mentionedJids);
      return chunkArray ?? new WaRichText.Chunk[0];
    }

    public static string GetMentionsRenderedText(
      this Message msg,
      out LinkDetector.Result[] updatedFormattings)
    {
      updatedFormattings = (LinkDetector.Result[]) null;
      if (msg == null)
        return (string) null;
      string textForDisplay = msg.GetTextForDisplay();
      if (!msg.GetMentionedJids().Any<string>())
      {
        updatedFormattings = msg.GetRichTextFormattings();
        return textForDisplay;
      }
      Pair<int, LinkDetector.Result>[] array = ((IEnumerable<LinkDetector.Result>) msg.GetRichTextFormattings(true)).Select<LinkDetector.Result, Pair<int, LinkDetector.Result>>((Func<LinkDetector.Result, Pair<int, LinkDetector.Result>>) (f => new Pair<int, LinkDetector.Result>(0, f))).ToArray<Pair<int, LinkDetector.Result>>();
      StringBuilder stringBuilder = new StringBuilder();
      int startIndex = 0;
      int length = array.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        Pair<int, LinkDetector.Result> pair = array[index1];
        if ((pair.Second.type & 256) != 0)
        {
          LinkDetector.Result second = pair.Second;
          if (second.Index > startIndex)
            stringBuilder.Append(textForDisplay, startIndex, second.Index - startIndex);
          string auxiliaryInfo = second.AuxiliaryInfo;
          UserStatus userStatus = UserCache.Get(auxiliaryInfo, false);
          string str = userStatus?.GetDisplayName(getNumberIfNoName: false, getFormattedNumber: false);
          if (string.IsNullOrEmpty(str))
          {
            str = userStatus?.PushName;
            if (string.IsNullOrEmpty(str))
              str = JidHelper.GetPhoneNumber(auxiliaryInfo, true);
          }
          stringBuilder.AppendFormat("@{0}", (object) str);
          startIndex = second.Index + second.Length;
          int num = str.Length + 1 - second.Length;
          if (num != 0)
          {
            for (int index2 = index1 + 1; index2 < length; ++index2)
              array[index2].First += num;
          }
          second.Length = str.Length + 1;
        }
      }
      if (textForDisplay.Length > startIndex)
        stringBuilder.Append(textForDisplay, startIndex, textForDisplay.Length - startIndex);
      updatedFormattings = ((IEnumerable<Pair<int, LinkDetector.Result>>) array).Select<Pair<int, LinkDetector.Result>, LinkDetector.Result>((Func<Pair<int, LinkDetector.Result>, LinkDetector.Result>) (p =>
      {
        LinkDetector.Result second = p.Second;
        second.Index += p.First;
        return second;
      })).ToArray<LinkDetector.Result>();
      return stringBuilder.ToString();
    }

    public static bool NeedsWebRelay(this Message msg)
    {
      return msg.Status == FunXMPP.FMessage.Status.Relay || ((bool?) msg.InternalProperties?.WebClientPropertiesField?.WebRelay ?? false);
    }

    public static bool IsMulticast(this Message msg)
    {
      return (bool?) msg.InternalProperties?.CommonPropertiesField?.Multicast ?? false;
    }

    public static void SetC2cFlags(this Message msg, bool phoneFlag, bool textFlag = false)
    {
      if (!phoneFlag)
        return;
      MessageProperties forMessage = MessageProperties.GetForMessage(msg);
      forMessage.EnsureCommonProperties.UrlNumber = new bool?(true);
      if (textFlag)
        forMessage.EnsureCommonProperties.UrlText = new bool?(true);
      forMessage.Save();
    }

    public static bool IsC2cPhoneNumber(this Message msg)
    {
      return (bool?) msg.InternalProperties?.CommonPropertiesField?.UrlNumber ?? false;
    }

    public static bool IsC2cText(this Message msg)
    {
      return (bool?) msg.InternalProperties?.CommonPropertiesField?.UrlText ?? false;
    }

    public static FunXMPP.FMessage.FunMediaType GetFunMediaType(this Message msg)
    {
      if (msg.IsPtt())
        return FunXMPP.FMessage.FunMediaType.Ptt;
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
      {
        int? revokedMediaType = (int?) msg.InternalProperties?.CommonPropertiesField?.RevokedMediaType;
        return revokedMediaType.HasValue ? (FunXMPP.FMessage.FunMediaType) revokedMediaType.Value : FunXMPP.FMessage.FunMediaType.Image;
      }
      return msg.MediaWaType == FunXMPP.FMessage.Type.Contact && msg.HasMultipleContacts() ? FunXMPP.FMessage.FunMediaType.ContactArray : (FunXMPP.FMessage.FunMediaType) msg.MediaWaType;
    }

    public static bool ShouldSend(this Message msg)
    {
      switch (msg.MediaWaType)
      {
        case FunXMPP.FMessage.Type.CipherText:
        case FunXMPP.FMessage.Type.ProtocolBuffer:
        case FunXMPP.FMessage.Type.Unsupported:
        case FunXMPP.FMessage.Type.CallOffer:
          return false;
        default:
          return !JidHelper.IsPsaJid(msg.KeyRemoteJid);
      }
    }

    public static bool HasMultipleContacts(this Message msg)
    {
      return msg.MediaWaType == FunXMPP.FMessage.Type.Contact && msg.InternalProperties.HasMultipleContacts();
    }

    public static bool HasMultipleContacts(this MessageProperties props)
    {
      return props != null && props.ContactPropertiesField != null && props.ContactPropertiesField.Vcards != null && props.ContactPropertiesField.Vcards.Any<string>();
    }

    public static MessageProperties.MediaProperties.Attribution GetGifAttribution(
      this MessageProperties props)
    {
      return props != null && props.MediaPropertiesField != null ? props.MediaPropertiesField.GifAttribution.GetValueOrDefault() : MessageProperties.MediaProperties.Attribution.NONE;
    }

    public static string GetDirectPath(this Message msg)
    {
      return msg?.InternalProperties?.MediaPropertiesField?.MediaDirectPath;
    }

    public static IEnumerable<ContactVCard> GetContactCards(this Message msg)
    {
      IEnumerable<ContactVCard> contactCards = (IEnumerable<ContactVCard>) null;
      if (msg.HasMultipleContacts())
      {
        contactCards = MessageProperties.GetForMessage(msg).Contacts.Select<string, ContactVCard>((Func<string, ContactVCard>) (s => ContactVCardParser.Parse(s))).Where<ContactVCard>((Func<ContactVCard, bool>) (c => c != null));
      }
      else
      {
        ContactVCard contactVcard = ContactVCardParser.Parse(msg.Data);
        if (contactVcard != null)
          contactCards = (IEnumerable<ContactVCard>) new ContactVCard[1]
          {
            contactVcard
          };
      }
      if (contactCards == null)
        contactCards = (IEnumerable<ContactVCard>) new ContactVCard[0];
      return contactCards;
    }

    public static string GetContactCardNames(this Message msg)
    {
      IEnumerable<ContactVCard> contactCards = msg.GetContactCards();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (ContactVCard contactVcard in contactCards)
        stringBuilder.Append(contactVcard.GetDisplayName(true)).Append(" ");
      return stringBuilder.ToString();
    }

    public static bool HasMediaPropertiesRatio(this MessageProperties props)
    {
      if (props != null && props.MediaPropertiesField != null)
      {
        uint? nullable = props.MediaPropertiesField.Height;
        if (((int) nullable ?? 0) != 0)
        {
          nullable = props.MediaPropertiesField.Width;
          return ((int) nullable ?? 0) != 0;
        }
      }
      return false;
    }

    public static int GetCipherRetryCount(this Message message, MessagesContext db = null)
    {
      MessageProperties internalProperties = message.InternalProperties;
      if (internalProperties != null && internalProperties.CommonPropertiesField != null)
      {
        int? cipherRetryCount = internalProperties.CommonPropertiesField.CipherRetryCount;
        if (cipherRetryCount.HasValue)
        {
          cipherRetryCount = internalProperties.CommonPropertiesField.CipherRetryCount;
          return cipherRetryCount.Value;
        }
      }
      MessageMiscInfo miscInfo = message.GetMiscInfo((SqliteMessagesContext) db);
      return miscInfo != null ? miscInfo.CipherRetryCount : 0;
    }

    public static byte[] GetCipherMediaHash(this MessageProperties props)
    {
      return props != null && props.CommonPropertiesField != null && props.CommonPropertiesField.CipherMediaHash != null ? props.CommonPropertiesField.CipherMediaHash : (byte[]) null;
    }

    public static byte[] GetCipherMediaHash(this Message message, MessagesContext db = null)
    {
      byte[] cipherMediaHash = message.InternalProperties.GetCipherMediaHash();
      if (cipherMediaHash != null)
        return cipherMediaHash;
      return message.GetMiscInfo((SqliteMessagesContext) db)?.CipherMediaHash;
    }

    public static void ClearCipherMediaHash(this Message message, MessagesContext db)
    {
      MessageProperties internalProperties = message.InternalProperties;
      if (internalProperties?.CommonPropertiesField?.CipherMediaHash != null)
      {
        internalProperties.CommonPropertiesField.CipherMediaHash = (byte[]) null;
        message.InternalProperties = internalProperties;
      }
      MessageMiscInfo miscInfo = message.GetMiscInfo((SqliteMessagesContext) db);
      if (miscInfo == null)
        return;
      miscInfo.CipherMediaHash = (byte[]) null;
    }

    public static Message ToMessage(this IEnumerable<ContactVCard> cards, MessagesContext db)
    {
      if (cards == null || !cards.Any<ContactVCard>())
        return (Message) null;
      IEnumerable<string> strings = cards.Select<ContactVCard, string>((Func<ContactVCard, string>) (c => c.ToVCardData(false)));
      Message msg = new Message(true);
      msg.KeyFromMe = true;
      msg.KeyId = FunXMPP.GenerateMessageId();
      msg.Status = FunXMPP.FMessage.Status.Pending;
      msg.MediaWaType = FunXMPP.FMessage.Type.Contact;
      MessageProperties forMessage = MessageProperties.GetForMessage(msg);
      forMessage.Contacts = strings;
      forMessage.Save();
      return msg;
    }

    public static byte[] SerializeToProtocolBuffer(this FunXMPP.FMessage fmsg)
    {
      return fmsg != null ? WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmsg, new CipherTextIncludes(true)).ToPlainText(false) : (byte[]) null;
    }

    public static Matrix GetVideoRotationMatrix(this Message msg)
    {
      Matrix? r = new Matrix?();
      if (msg.LocalFileExists())
      {
        try
        {
          using (Stream str = MediaStorage.OpenFile(msg.LocalFileUri))
            Mp4Atom.GetOrientationMatrices(str, str.Length, (Mp4Atom.MatrixParserCallback) ((name, offset, m, cancel) =>
            {
              Matrix matrix = m.Matrix;
              Log.l("mp4 rotation", "Found transform matrix in [{0}]: [[{1} {2}] [{3} {4}] [{5} {6}]]", (object) name, (object) matrix.M11, (object) matrix.M12, (object) matrix.M21, (object) matrix.M22, (object) matrix.OffsetX, (object) matrix.OffsetY);
              if (m.IsIdentity)
                return;
              r = new Matrix?(matrix);
            }));
        }
        catch (Exception ex)
        {
        }
      }
      return r ?? VideoFrameGrabber.MatrixForAngle(0).Matrix;
    }

    public static IObservable<Matrix> GetVideoRotationMatrixAsync(this Message msg)
    {
      return Observable.Create<Matrix>((Func<IObserver<Matrix>, Action>) (observer =>
      {
        Matrix videoRotationMatrix = msg.GetVideoRotationMatrix();
        observer.OnNext(videoRotationMatrix);
        observer.OnCompleted();
        return (Action) (() => { });
      }));
    }

    public static double GetThumbnailRatio(this Message msg)
    {
      double thumbnailRatio = 1.0;
      MessageProperties internalProperties = msg.InternalProperties;
      if (msg.InternalProperties.HasMediaPropertiesRatio())
      {
        uint? nullable = internalProperties.MediaPropertiesField.Width;
        double num1 = (double) nullable.Value;
        nullable = internalProperties.MediaPropertiesField.Height;
        double num2 = (double) nullable.Value;
        return num1 / num2;
      }
      MessageMiscInfo misc = msg.GetMiscInfo();
      bool upgradeNeeded = false;
      MessageMiscInfo.ImageInfo imgInfo = misc == null ? (MessageMiscInfo.ImageInfo) null : misc.GetImageInfo(out upgradeNeeded);
      if (imgInfo == null | upgradeNeeded)
      {
        Log.d(nameof (msg), "calculate w/h ratio from small thumb");
        BitmapSource smallThumb = msg.GetThumbnail(MessageExtensions.ThumbPreference.OnlySmall);
        if (smallThumb != null)
        {
          thumbnailRatio = (double) smallThumb.PixelWidth / (double) smallThumb.PixelHeight;
          AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            imgInfo = MessageMiscInfo.ImageInfo.Create((double) smallThumb.PixelWidth, (double) smallThumb.PixelHeight);
            misc = msg.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateToDbIfNotFound);
            misc.SetImageInfo(imgInfo);
            db.SubmitChanges();
          }))));
        }
      }
      else
      {
        Log.d("msg vm", "calculate w/h ratio from image info");
        thumbnailRatio = imgInfo.PixelWidth / imgInfo.PixelHeight;
      }
      return thumbnailRatio;
    }

    public static Message GetMediaCipherDuplicate(
      this Message[] similarMessages,
      Message source,
      MessagesContext db)
    {
      if (similarMessages == null)
        return (Message) null;
      foreach (Message similarMessage in similarMessages)
      {
        if (similarMessage.LocalFileExists() && similarMessage.GetCipherMediaHash(db) != null && (source == null || source.MediaSize == similarMessage.MediaSize))
          return similarMessage;
      }
      return (Message) null;
    }

    public static void RecomputeVideoDimensions(this Message message)
    {
      uint? nullable = (uint?) message.InternalProperties?.MediaPropertiesField?.Width;
      if (((int) nullable ?? 0) != 0)
      {
        nullable = (uint?) message.InternalProperties?.MediaPropertiesField?.Height;
        if (((int) nullable ?? 0) != 0)
          return;
      }
      try
      {
        Mp4UtilsMetadata metadata = NativeInterfaces.Mp4Utils.GetStreamMetadata(MediaStorage.GetAbsolutePath(message.LocalFileUri));
        ref Mp4UtilsVideoMetdata? local1 = ref metadata.Video;
        if ((local1.HasValue ? (local1.GetValueOrDefault().Width > 0 ? 1 : 0) : 0) == 0)
          return;
        ref Mp4UtilsVideoMetdata? local2 = ref metadata.Video;
        if ((local2.HasValue ? (local2.GetValueOrDefault().Height > 0 ? 1 : 0) : 0) == 0)
          return;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          MessageProperties forMessage = MessageProperties.GetForMessage(message);
          switch (Math.Abs(metadata.Video.Value.RotationAngle) % 360)
          {
            case 90:
            case 270:
              forMessage.EnsureMediaProperties.Height = new uint?((uint) metadata.Video.Value.Width);
              forMessage.EnsureMediaProperties.Width = new uint?((uint) metadata.Video.Value.Height);
              break;
            default:
              forMessage.EnsureMediaProperties.Width = new uint?((uint) metadata.Video.Value.Width);
              forMessage.EnsureMediaProperties.Height = new uint?((uint) metadata.Video.Value.Height);
              break;
          }
          forMessage.Save();
          db.SubmitChanges();
        }));
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "video dimensions");
      }
    }

    public static bool IsStatusWithNoSentRecipient(this Message message)
    {
      if (message == null || !message.IsStatus())
        return false;
      int? sentRecipientsCount = (int?) message.InternalProperties?.CommonPropertiesField?.SentRecipientsCount;
      if (sentRecipientsCount.HasValue)
      {
        int? nullable = sentRecipientsCount;
        int num = 0;
        if ((nullable.GetValueOrDefault() <= num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          return true;
      }
      return false;
    }

    public static string GetPushTag(this Message m)
    {
      string s = m.KeyId + ":" + m.KeyRemoteJid;
      if (!AppState.IsWP10CreatorOrLater)
      {
        byte[] hash = MD5Core.GetHash(Encoding.UTF8.GetBytes(s));
        StringBuilder stringBuilder = new StringBuilder(16);
        for (int index = 0; index < 8; ++index)
          stringBuilder.Append(hash[index].ToString("X2"));
        s = stringBuilder.ToString();
      }
      else if (s.Length > 64)
        s = s.Substring(0, 64);
      return s;
    }

    public static bool isCurrentlyLiveLocationMessage(this Message m)
    {
      bool flag = true;
      if (m == null)
        flag = false;
      if (flag && m.MediaWaType != FunXMPP.FMessage.Type.LiveLocation)
        flag = false;
      DateTime dateTime = m.FunTimestamp.Value.AddSeconds((double) m.MediaDurationSeconds);
      if (flag && dateTime < FunRunner.CurrentServerTimeUtc)
        flag = false;
      return flag;
    }

    [Flags]
    public enum NotifyFlags
    {
      None = 0,
      UpdateChatPreview = 2,
      Noteworthy = 4,
      ShouldAlert = 8,
      ShouldIncreaseUnread = 16, // 0x00000010
      ShouldUpdatePreviewOnly = 32, // 0x00000020
    }

    public enum MediaUploadActionState
    {
      None,
      Retryable,
      Cancellable,
      Unknown,
    }

    public enum ThumbPreference
    {
      None,
      PreferSmall,
      OnlySmall,
    }
  }
}
