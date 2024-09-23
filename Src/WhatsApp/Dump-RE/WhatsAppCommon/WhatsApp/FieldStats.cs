// Decompiled with JetBrains decompiler
// Type: WhatsApp.FieldStats
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.Events;

#nullable disable
namespace WhatsApp
{
  public static class FieldStats
  {
    private const int SAMPLE_CHAT_CONNECTION = 20;
    public const int SAMPLE_GIF_FETCH = 10;
    private const int SAMPLE_LOGIN = 20;
    public const int SAMPLE_MESSAGE_RECEIVE = 20;
    public const int SAMPLE_MESSAGE_SEND = 10;
    public const int SAMPLE_PSEUDO_HTTP_SESSION = 20;
    public const int SAMPLE_STATUS_VIEW = 10;
    private const int SAMPLE_UI_USAGE = 5;
    public const int SAMPLE_WP_AGENT = 20;
    public const int SAMPLE_WP8_DROPPED = 20;
    private const int SAMPLE_UI_ACTION = 1;
    private static long ThirtyMinInMsecs = (long) TimeSpan.FromMinutes(30.0).TotalMilliseconds;
    private static long ApplicationActivationTicks = 0;
    private static long contactsOpenStartTimeTicks = 0;
    private static long chatOpenStartTimeTicks = 0;
    private static string chatOpenChatJid = (string) null;
    private static bool ThrottleClbUpload = false;

    public static void ReportMessageReceive(FunXMPP.FMessage message)
    {
      MessageReceive messageReceive = new MessageReceive();
      messageReceive.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(message.media_wa_type, message.media_origin == "live"));
      messageReceive.messageIsOffline = new bool?(message.offline >= 0);
      if (message.timestamp.HasValue)
        messageReceive.messageReceiveT0 = new long?(message.timestamp.Value.ToUnixTime());
      messageReceive.messageReceiveT1 = new long?(DateTime.Now.ToUnixTime());
      wam_enum_message_type? nullable = FieldStats.MessageFsType(message.key.remote_jid);
      if (nullable.HasValue)
        messageReceive.messageType = new wam_enum_message_type?(nullable.Value);
      messageReceive.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(message.media_wa_type, false));
      messageReceive.SaveEventSampled(20U);
      string remoteJid = message.key.remote_jid;
      ref DateTime? local = ref message.timestamp;
      long? unixTimestamp = local.HasValue ? new long?(local.GetValueOrDefault().ToUnixTime()) : new long?();
      ChatMsgCounts.QueueToPending(remoteJid, unixTimestamp, true);
    }

    public static void ReportMessageSend(Message message)
    {
      MessageSend messageSend = new MessageSend();
      messageSend.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(message));
      messageSend.messageSendOptUploadEnabled = new bool?(MediaUpload.OptimisticUploadAllowed);
      wam_enum_message_type? nullable = FieldStats.MessageFsType(message.KeyRemoteJid);
      if (nullable.HasValue)
        messageSend.messageType = new wam_enum_message_type?(nullable.Value);
      if (message.TimestampLong > 0L)
      {
        long num = FunRunner.CurrentServerTimeUtc.ToUnixTime() - message.TimestampLong;
        messageSend.messageSendT = num > 0L ? new long?(num) : new long?();
      }
      messageSend.messageSendResult = new wam_enum_message_send_result_type?(wam_enum_message_send_result_type.OK);
      MessageProperties forMessage = MessageProperties.GetForMessage(message);
      messageSend.messageIsForward = new bool?(((int) forMessage?.CommonPropertiesField?.ForwardedFlag ?? 0) != 0);
      messageSend.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(message.MediaWaType, false));
      messageSend.SaveEventSampled(10U);
      ChatMsgCounts.QueueToPending(message.KeyRemoteJid, new long?(message.TimestampLong), false);
    }

    public static void ReportProfilePictureUpload(double length)
    {
      new ProfilePicUpload()
      {
        profilePicSize = new double?(length),
        profilePicUploadT = new long?(FieldStatsRunner.GetTime())
      }.SaveEvent();
    }

    public static void ReportLogin(wam_enum_login_result_type result)
    {
      Login login = new Login();
      login.retryCount = new long?((long) FieldStatsRunner.LoginRetryCount);
      login.loginResult = new wam_enum_login_result_type?(result);
      if (result == wam_enum_login_result_type.OK)
        login.loginT = new long?(FieldStatsRunner.GetTime());
      login.SaveEventSampled(20U);
      FieldStatsRunner.LoginRetryCount = 0;
    }

    public static void ReportUiUsage(wam_enum_ui_usage_type page)
    {
      new UiUsage()
      {
        uiUsageType = new wam_enum_ui_usage_type?(page)
      }.SaveEventSampled(5U);
    }

    public static void ReportPtt(
      wam_enum_ptt_result_type result,
      wam_enum_ptt_source_type source,
      double size)
    {
      new Ptt()
      {
        pttResult = new wam_enum_ptt_result_type?(result),
        pttSource = new wam_enum_ptt_source_type?(source),
        pttSize = new double?(size)
      }.SaveEvent();
    }

    public static void ReportRegistrationComplete(long restrationTimeMs)
    {
      new RegistrationComplete()
      {
        registrationT = new long?(restrationTimeMs)
      }.SaveEvent();
    }

    public static void MaybeSetFgAppLaunchTime()
    {
      if (FieldStats.ApplicationActivationTicks != 0L || AppState.IsBackgroundAgent)
        return;
      FieldStats.ApplicationActivationTicks = DateTime.Now.Ticks;
    }

    public static void ClearFgLaunch() => FieldStats.ApplicationActivationTicks = 0L;

    public static void MaybeReportAppLaunch(bool isAlreadyInMem)
    {
      long applicationActivationTicks = FieldStats.ApplicationActivationTicks;
      FieldStats.ApplicationActivationTicks = 0L;
      if (applicationActivationTicks == 0L)
        return;
      long num = (DateTime.Now.Ticks - applicationActivationTicks) / 10000L;
      if (num <= 0L || num > FieldStats.ThirtyMinInMsecs)
        Log.l(nameof (FieldStats), "Looks like time corruption for App Launch {0}", (object) num);
      else
        new UiAction()
        {
          uiActionType = new wam_enum_ui_action_type?(wam_enum_ui_action_type.APP_OPEN),
          uiActionT = new long?(num),
          uiActionPreloaded = new bool?(isAlreadyInMem)
        }.SaveEventSampled(1U);
    }

    public static void SetContactsOpenStartTime()
    {
      FieldStats.contactsOpenStartTimeTicks = DateTime.Now.Ticks;
    }

    public static void ReportContactsOpen()
    {
      long openStartTimeTicks = FieldStats.contactsOpenStartTimeTicks;
      FieldStats.contactsOpenStartTimeTicks = 0L;
      if (openStartTimeTicks == 0L)
        return;
      long num = (DateTime.Now.Ticks - openStartTimeTicks) / 10000L;
      if (num <= 0L || num > FieldStats.ThirtyMinInMsecs)
        Log.l(nameof (FieldStats), "Looks like time corruption for Contacts Open {0}", (object) num);
      else
        new UiAction()
        {
          uiActionType = new wam_enum_ui_action_type?(wam_enum_ui_action_type.CONTACTS_OPEN),
          uiActionT = new long?(num),
          uiActionPreloaded = new bool?(false)
        }.SaveEventSampled(1U);
    }

    public static void SetChatOpenStartTime(string chatJid)
    {
      FieldStats.chatOpenChatJid = chatJid;
      FieldStats.chatOpenStartTimeTicks = DateTime.Now.Ticks;
    }

    public static void ReportChatOpen(string chatJid)
    {
      long openStartTimeTicks = FieldStats.chatOpenStartTimeTicks;
      FieldStats.chatOpenStartTimeTicks = 0L;
      if (openStartTimeTicks == 0L || !(FieldStats.chatOpenChatJid == chatJid))
        return;
      FieldStats.chatOpenChatJid = (string) null;
      long num = (DateTime.Now.Ticks - openStartTimeTicks) / 10000L;
      if (num <= 0L || num > FieldStats.ThirtyMinInMsecs)
        Log.l(nameof (FieldStats), "Looks like time corruption for Chat Open {0}", (object) num);
      else
        new UiAction()
        {
          uiActionType = new wam_enum_ui_action_type?(wam_enum_ui_action_type.CHAT_OPEN),
          uiActionT = new long?(num),
          uiActionPreloaded = new bool?(false)
        }.SaveEventSampled(1U);
    }

    public static void ReportRevokeRecv(Message revokedMessage, DateTime currentTime)
    {
      RecvRevokeMessage recvRevokeMessage = new RecvRevokeMessage();
      wam_enum_message_type? nullable = FieldStats.MessageFsType(revokedMessage.KeyRemoteJid);
      if (nullable.HasValue)
        recvRevokeMessage.messageType = new wam_enum_message_type?(nullable.Value);
      recvRevokeMessage.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(revokedMessage));
      if (revokedMessage.TimestampLong > 0L)
        recvRevokeMessage.revokeRecvDelay = new long?((long) (int) (currentTime.ToUnixTime() - revokedMessage.TimestampLong));
      recvRevokeMessage.revokeOutOfOrder = new bool?(false);
      recvRevokeMessage.SaveEvent();
    }

    public static void ReportRevokeRecv(FunXMPP.FMessage revokingFmsg)
    {
      RecvRevokeMessage recvRevokeMessage = new RecvRevokeMessage();
      wam_enum_message_type? nullable = FieldStats.MessageFsType(revokingFmsg.key.remote_jid);
      if (nullable.HasValue)
        recvRevokeMessage.messageType = new wam_enum_message_type?(nullable.Value);
      recvRevokeMessage.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(revokingFmsg.media_wa_type, revokingFmsg.media_origin == "live"));
      recvRevokeMessage.revokeOutOfOrder = new bool?(true);
      recvRevokeMessage.SaveEvent();
    }

    public static void ReportRevokeSend(Message revokedMessage, DateTime currentTime)
    {
      SendRevokeMessage sendRevokeMessage = new SendRevokeMessage();
      wam_enum_message_type? nullable = FieldStats.MessageFsType(revokedMessage.KeyRemoteJid);
      if (nullable.HasValue)
        sendRevokeMessage.messageType = new wam_enum_message_type?(nullable.Value);
      sendRevokeMessage.messageMediaType = new wam_enum_media_type?(FieldStats.MediaFsType(revokedMessage));
      if (revokedMessage.TimestampLong > 0L)
        sendRevokeMessage.revokeSendDelay = new long?((long) (int) (currentTime.ToUnixTime() - revokedMessage.TimestampLong));
      sendRevokeMessage.SaveEvent();
    }

    public static void ReportChatConnection(int port)
    {
      new ChatConnection()
      {
        chatLoginT = new long?(FunRunner.CurrentServerTimeUtc.ToUnixTime()),
        chatPort = new long?((long) port)
      }.SaveEventSampled(20U);
    }

    public static void ReportChangeNumber() => new ChangeNumberC().SaveEvent();

    public static void ReportStatusUpdate() => new StatusUpdateC().SaveEvent();

    public static void ReportBackupConvo() => new BackupConvoC().SaveEvent();

    public static void ReportGroupCreate() => new GroupCreateC().SaveEvent();

    public static void ReportTellAFriend() => new TellAFriendC().SaveEvent();

    public static void ReportDeleteNumber() => new DeleteNumberC().SaveEvent();

    public static WhatsApp.Events.MediaDownload GetFsMediaDownloadEvent(Message msg)
    {
      WhatsApp.Events.MediaDownload fsEvent = new WhatsApp.Events.MediaDownload();
      fsEvent.mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(msg));
      fsEvent.isReuse = new bool?(false);
      fsEvent.mmsVersion = new long?(3L);
      fsEvent.mediaUsedCdn = new bool?(false);
      FieldStats.SetHostDetailsInDownloadEvent(fsEvent, msg.MediaUrl);
      fsEvent.mediaSize = new double?((double) msg.MediaSize);
      return fsEvent;
    }

    public static void SetHostDetailsInDownloadEvent(WhatsApp.Events.MediaDownload fsEvent, string mediaUrl)
    {
      if (string.IsNullOrEmpty(mediaUrl))
        return;
      if (fsEvent == null)
        return;
      try
      {
        string host = new Uri(mediaUrl).Host;
        if (string.IsNullOrEmpty(host))
          return;
        fsEvent.mediaUsedCdn = new bool?(host.EndsWith("mme.whatsapp.net") || host.EndsWith(".cdn.whatsapp.net"));
        fsEvent.routeHostname = FieldStats.ExtractFirstPartFromHostName(host);
      }
      catch (Exception ex)
      {
      }
    }

    public static WhatsApp.Events.MediaUpload GetFsMediaUploadEvent(Message msg)
    {
      FieldStats.MediaUploadLogged fsEvent = new FieldStats.MediaUploadLogged();
      fsEvent.mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(msg));
      fsEvent.isReuse = new bool?(false);
      fsEvent.retryCount = new long?(0L);
      fsEvent.mmsVersion = new long?(3L);
      fsEvent.mediaUsedCdn = new bool?(false);
      FieldStats.SetHostDetailsInUploadEvent((WhatsApp.Events.MediaUpload) fsEvent, msg.MediaUrl);
      fsEvent.mediaSize = new double?((double) msg.MediaSize);
      fsEvent.mediaUploadResult = new wam_enum_media_upload_result_type?(wam_enum_media_upload_result_type.ERROR_UNKNOWN);
      return (WhatsApp.Events.MediaUpload) fsEvent;
    }

    public static void SetHostDetailsInUploadEvent(WhatsApp.Events.MediaUpload fsEvent, string mediaUrl)
    {
      if (string.IsNullOrEmpty(mediaUrl))
        return;
      if (fsEvent == null)
        return;
      try
      {
        string host = new Uri(mediaUrl).Host;
        if (string.IsNullOrEmpty(host))
          return;
        fsEvent.mediaUsedCdn = new bool?(host.EndsWith("mme.whatsapp.net") || host.EndsWith(".cdn.whatsapp.net"));
        fsEvent.routeHostname = FieldStats.ExtractFirstPartFromHostName(host);
      }
      catch (Exception ex)
      {
      }
    }

    public static void SetResultInUploadEvent(
      WhatsApp.Events.MediaUpload fsEvent,
      wam_enum_media_upload_result_type errorCode)
    {
      if (fsEvent == null)
        return;
      wam_enum_media_upload_result_type? mediaUploadResult1 = fsEvent.mediaUploadResult;
      wam_enum_media_upload_result_type uploadResultType1 = wam_enum_media_upload_result_type.ERROR_UNKNOWN;
      if ((mediaUploadResult1.GetValueOrDefault() == uploadResultType1 ? (mediaUploadResult1.HasValue ? 1 : 0) : 0) == 0)
      {
        wam_enum_media_upload_result_type? mediaUploadResult2 = fsEvent.mediaUploadResult;
        wam_enum_media_upload_result_type uploadResultType2 = wam_enum_media_upload_result_type.ERROR_UPLOAD;
        if ((mediaUploadResult2.GetValueOrDefault() == uploadResultType2 ? (mediaUploadResult2.HasValue ? 1 : 0) : 0) == 0 && errorCode != wam_enum_media_upload_result_type.OK)
          return;
      }
      fsEvent.mediaUploadResult = new wam_enum_media_upload_result_type?(errorCode);
    }

    public static string ExtractFirstPartFromHostName(string hostname)
    {
      if (string.IsNullOrEmpty(hostname))
        return (string) null;
      int length = hostname.IndexOf('.');
      return length <= 0 ? hostname : hostname.Substring(0, length);
    }

    public static void ReportFsStatusPostEvent(
      FunXMPP.FMessage.Type waType,
      wam_enum_status_post_origin origin,
      wam_enum_status_post_result result)
    {
      new StatusPost()
      {
        statusSessionId = new long?((long) (int) WaStatusHelper.SessionId),
        mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(waType, false)),
        statusPostOrigin = new wam_enum_status_post_origin?(origin),
        statusPostResult = new wam_enum_status_post_result?(result)
      }.SaveEvent();
    }

    public static void ReportFsStatusViewEvent(
      FunXMPP.FMessage.Type waType,
      wam_enum_status_view_post_origin origin,
      wam_enum_status_view_post_result result)
    {
      new StatusViewPost()
      {
        statusSessionId = new long?((long) (int) WaStatusHelper.SessionId),
        mediaType = new wam_enum_media_type?(FieldStats.MediaFsType(waType, false)),
        statusViewPostOrigin = new wam_enum_status_view_post_origin?(origin),
        statusViewPostResult = new wam_enum_status_view_post_result?(result)
      }.SaveEventSampled(10U);
    }

    public static void ReportFsStatusReplyEvent(wam_enum_status_reply_result result)
    {
      new StatusReply()
      {
        statusSessionId = new long?((long) (int) WaStatusHelper.SessionId),
        statusReplyResult = new wam_enum_status_reply_result?(result)
      }.SaveEvent();
    }

    public static void ReportLiveLocationDurationPicker(long durationInSeconds)
    {
      new LiveLocationDurationPicker()
      {
        liveLocationDurationPickerSelectedDuration = new long?(durationInSeconds),
        liveLocationDurationPickerEntryPoint = new wam_enum_live_location_duration_picker_entry_point?(wam_enum_live_location_duration_picker_entry_point.CHAT)
      }.SaveEvent();
    }

    public static void ReportLiveLocationSharingSession(
      long? sessionTime,
      wam_enum_live_location_sharing_session_ended_reason reason)
    {
      new LiveLocationSharingSession()
      {
        liveLocationSharingSessionT = sessionTime,
        liveLocationSharingSessionEndedReason = new wam_enum_live_location_sharing_session_ended_reason?(reason)
      }.SaveEvent();
    }

    public static void ReportLiveLocationMessageStatus(
      DateTime msgTime,
      wam_enum_live_location_message_status_result result)
    {
      long totalSeconds = (long) FunRunner.CurrentServerTimeUtc.Subtract(msgTime).TotalSeconds;
      new LiveLocationMessageStatus()
      {
        liveLocationLocationFixT = new long?(totalSeconds),
        liveLocationMessageStatusResult = new wam_enum_live_location_message_status_result?(result)
      }.SaveEvent();
    }

    public static void ReportBizProfileAction(
      string jid,
      wam_enum_view_business_profile_action action,
      wam_enum_website_source_type? source = null)
    {
      if (jid == null)
        return;
      new ViewBusinessProfile()
      {
        businessProfileJid = jid,
        viewBusinessProfileAction = new wam_enum_view_business_profile_action?(action),
        websiteSource = source
      }.SaveEvent();
    }

    public static wam_enum_media_type MediaFsType(Message msg)
    {
      return FieldStats.MediaFsType(msg.MediaWaType, msg.MediaOrigin == "live");
    }

    public static wam_enum_media_type MediaFsType(FunXMPP.FMessage.Type waType, bool isLive)
    {
      switch (waType)
      {
        case FunXMPP.FMessage.Type.Undefined:
          return wam_enum_media_type.NONE;
        case FunXMPP.FMessage.Type.Image:
          return wam_enum_media_type.PHOTO;
        case FunXMPP.FMessage.Type.Audio:
          return !isLive ? wam_enum_media_type.AUDIO : wam_enum_media_type.PTT;
        case FunXMPP.FMessage.Type.Video:
          return wam_enum_media_type.VIDEO;
        case FunXMPP.FMessage.Type.Contact:
          return wam_enum_media_type.CONTACT;
        case FunXMPP.FMessage.Type.Location:
          return wam_enum_media_type.LOCATION;
        case FunXMPP.FMessage.Type.Document:
          return wam_enum_media_type.DOCUMENT;
        case FunXMPP.FMessage.Type.ExtendedText:
          return wam_enum_media_type.URL;
        case FunXMPP.FMessage.Type.Gif:
          return wam_enum_media_type.GIF;
        case FunXMPP.FMessage.Type.LiveLocation:
          return wam_enum_media_type.LIVE_LOCATION;
        case FunXMPP.FMessage.Type.Sticker:
          return wam_enum_media_type.STICKER;
        case FunXMPP.FMessage.Type.ProtocolBuffer:
          return wam_enum_media_type.FUTURE;
        case FunXMPP.FMessage.Type.Unsupported:
          return wam_enum_media_type.FUTURE;
        case FunXMPP.FMessage.Type.CallOffer:
          return wam_enum_media_type.CALL;
        default:
          return wam_enum_media_type.NONE;
      }
    }

    public static wam_enum_message_type? MessageFsType(string jid)
    {
      switch (JidHelper.GetJidType(jid))
      {
        case JidHelper.JidTypes.User:
          return new wam_enum_message_type?(wam_enum_message_type.INDIVIDUAL);
        case JidHelper.JidTypes.Group:
          return new wam_enum_message_type?(wam_enum_message_type.GROUP);
        case JidHelper.JidTypes.Broadcast:
          return new wam_enum_message_type?(wam_enum_message_type.BROADCAST);
        case JidHelper.JidTypes.Status:
          return new wam_enum_message_type?(wam_enum_message_type.STATUS);
        default:
          Log.l("Message type for fieldstats is unexpected: " + jid);
          return new wam_enum_message_type?();
      }
    }

    public static string LastDataCenterUsed
    {
      get => NonDbSettings.LastDataCenterUsed;
      set
      {
        if (!(NonDbSettings.LastDataCenterUsed != value))
          return;
        NonDbSettings.LastDataCenterUsed = value;
        Wam.SetDatacenter(NonDbSettings.LastDataCenterUsed);
      }
    }

    public class MediaUploadLogged : WhatsApp.Events.MediaUpload
    {
      private bool saved;

      public override void SaveEvent()
      {
        if (this.saved && !FieldStats.ThrottleClbUpload)
        {
          FieldStats.ThrottleClbUpload = true;
          Log.SendCrashLog((Exception) new ArgumentException("repeated save"), "Already saved field stats for upload", logOnlyForRelease: true);
        }
        this.saved = true;
        Log.d("FsUpload", "Details {0} {1} {2} {3} {4}", (object) this.mediaType, (object) this.mmsVersion, (object) this.retryCount, (object) this.mediaUploadResult, (object) this.optimisticFlag);
        base.SaveEvent();
      }
    }
  }
}
