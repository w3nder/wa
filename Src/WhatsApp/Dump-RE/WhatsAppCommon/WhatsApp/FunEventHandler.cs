// Decompiled with JetBrains decompiler
// Type: WhatsApp.FunEventHandler
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using WhatsApp.CommonOps;
using WhatsApp.ProtoBuf;
using WhatsApp.WaCollections;
using WhatsAppCommon;

#nullable disable
namespace WhatsApp
{
  public class FunEventHandler : FunXMPP.Listener, FunXMPP.GroupListener
  {
    public const string LogHeader = "funhandler";
    public const int RevokeEditVersion = 7;
    public Subject<Unit> OfflineCompletedSubject = new Subject<Unit>();
    public PresenceState Presence = PresenceState.Instance;
    private List<IDisposable> toDisposeOnLostConnection = new List<IDisposable>();
    private object toDisposeLock = new object();
    private QrListener qr = new QrListener();
    public static FunEventHandler.ServerPropHandler[] ServerPropHandlers = new FunEventHandler.ServerPropHandler[51]
    {
      new FunEventHandler.ServerPropHandler("max_participants", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxGroupParticipants = i)), new Settings.Key?(Settings.Key.MaxGroupParticipants)),
      new FunEventHandler.ServerPropHandler("max_subject", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxGroupSubject = i)), new Settings.Key?(Settings.Key.MaxGroupSubject)),
      new FunEventHandler.ServerPropHandler("max_list_recipients", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxListRecipients = i)), new Settings.Key?(Settings.Key.MaxListRecipients)),
      new FunEventHandler.ServerPropHandler("media", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxMediaSize = i * 1024 * 1024)), new Settings.Key?(Settings.Key.MaxMediaSize)),
      new FunEventHandler.ServerPropHandler("media_max_autodownload", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxAutodownloadSize = Math.Max(i, 16) * 1024 * 1024)), new Settings.Key?(Settings.Key.MaxAutodownloadSize)),
      new FunEventHandler.ServerPropHandler("image_max_edge", FunEventHandler.ToInt((Action<int>) (i => Settings.ImageMaxEdge = i)), new Settings.Key?(Settings.Key.ImageMaxEdge)),
      new FunEventHandler.ServerPropHandler("image_quality", FunEventHandler.ToInt((Action<int>) (i => Settings.JpegQuality = i)), new Settings.Key?(Settings.Key.JpegQuality)),
      new FunEventHandler.ServerPropHandler("image_max_kbytes", FunEventHandler.ToInt((Action<int>) (i => Settings.ImageMaxKbytes = i)), new Settings.Key?(Settings.Key.ImageMaxKbytes)),
      new FunEventHandler.ServerPropHandler("adm", FunEventHandler.ToInt((Action<int>) (i => Settings.IsWaAdmin = i != 0)), new Settings.Key?(Settings.Key.IsWaAdmin)),
      new FunEventHandler.ServerPropHandler("gif_provider", FunEventHandler.ToInt((Action<int>) (i => Settings.GifSearchProvider = i)), new Settings.Key?(Settings.Key.GifSearchProvider)),
      new FunEventHandler.ServerPropHandler("source", FunEventHandler.ToInt((Action<int>) (i => Settings.LocationProvider = i)), new Settings.Key?(Settings.Key.LocationProvider)),
      new FunEventHandler.ServerPropHandler("video_max_edge", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxVideoEdge = i)), new Settings.Key?(Settings.Key.MaxVideoEdge)),
      new FunEventHandler.ServerPropHandler("file_max_size", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxFileSize = i * 1024 * 1024)), new Settings.Key?(Settings.Key.MaxFileSize)),
      new FunEventHandler.ServerPropHandler("max_keys", FunEventHandler.ToInt((Action<int>) (i => Settings.MaxPreKeyBatchSize = i)), new Settings.Key?(Settings.Key.MaxPreKeyBatchSize)),
      new FunEventHandler.ServerPropHandler("status_image_max_edge", FunEventHandler.ToInt((Action<int>) (i => Settings.StatusImageMaxEdge = i)), new Settings.Key?(Settings.Key.StatusImageMaxEdge)),
      new FunEventHandler.ServerPropHandler("status_image_quality", FunEventHandler.ToInt((Action<int>) (i => Settings.StatusImageQuality = i)), new Settings.Key?(Settings.Key.StatusImageQuality)),
      new FunEventHandler.ServerPropHandler("status_video_max_bitrate", FunEventHandler.ToInt((Action<int>) (i => Settings.StatusVideoMaxBitrate = i)), new Settings.Key?(Settings.Key.StatusVideoMaxBitrate)),
      new FunEventHandler.ServerPropHandler("status_video_max_duration", FunEventHandler.ToInt((Action<int>) (i => Settings.StatusVideoMaxDuration = i)), new Settings.Key?(Settings.Key.StatusVideoMaxDuration)),
      new FunEventHandler.ServerPropHandler("change_number_v2", FunEventHandler.ToInt((Action<int>) (i => Settings.ChangeNumberNotifyEnabled = i != 0)), new Settings.Key?(Settings.Key.ChangeNumberNotifyEnabled)),
      new FunEventHandler.ServerPropHandler("p2p_pay", FunEventHandler.ToInt((Action<int>) (i => Settings.P2pPaymentEnabled = i != 0)), new Settings.Key?(Settings.Key.P2pPaymentEnabled)),
      new FunEventHandler.ServerPropHandler("mms4_image", FunEventHandler.ToInt((Action<int>) (i => Settings.Mms4ServerImage = i != 0)), new Settings.Key?(Settings.Key.Mms4ServerImage)),
      new FunEventHandler.ServerPropHandler("mms4_audio", FunEventHandler.ToInt((Action<int>) (i => Settings.Mms4ServerAudio = i != 0)), new Settings.Key?(Settings.Key.Mms4ServerAudio)),
      new FunEventHandler.ServerPropHandler("mms4_ptt", FunEventHandler.ToInt((Action<int>) (i => Settings.Mms4ServerPtt = i != 0)), new Settings.Key?(Settings.Key.Mms4ServerPtt)),
      new FunEventHandler.ServerPropHandler("mms4_video", FunEventHandler.ToInt((Action<int>) (i => Settings.MmsServerVideo = i != 0)), new Settings.Key?(Settings.Key.MmsServerVideo)),
      new FunEventHandler.ServerPropHandler("mms4_gif", FunEventHandler.ToInt((Action<int>) (i => Settings.MmsServerGif = i != 0)), new Settings.Key?(Settings.Key.MmsServerGif)),
      new FunEventHandler.ServerPropHandler("mms4_doc", FunEventHandler.ToInt((Action<int>) (i => Settings.MmsServerDoc = i != 0)), new Settings.Key?(Settings.Key.MmsServerDoc)),
      new FunEventHandler.ServerPropHandler("video_max_bitrate", FunEventHandler.ToInt((Action<int>) (i => Settings.VideoMaxBitrate = i)), new Settings.Key?(Settings.Key.VideoMaxBitrate)),
      new FunEventHandler.ServerPropHandler("location", FunEventHandler.ToInt((Action<int>) (i => Settings.LiveLocationEnabled = i != 0)), new Settings.Key?(Settings.Key.LiveLocationProp)),
      new FunEventHandler.ServerPropHandler("emoji_search", FunEventHandler.ToInt((Action<int>) (i => Settings.EmojiSearchEnabled = i != 0)), new Settings.Key?(Settings.Key.EmojiSearch)),
      new FunEventHandler.ServerPropHandler("fs_time_spent", FunEventHandler.ToInt((Action<int>) (i => Settings.TimeSpentRecordOption = i)), new Settings.Key?(Settings.Key.TimeSpentRecordOption)),
      new FunEventHandler.ServerPropHandler("group_description_length", FunEventHandler.ToInt((Action<int>) (i => Settings.GroupDescriptionLength = i)), new Settings.Key?(Settings.Key.GroupDescriptionLength)),
      new FunEventHandler.ServerPropHandler("fieldstats_send_interval_seconds", FunEventHandler.ToInt((Action<int>) (i => Settings.FieldStatsSendIntervalSecs = i)), new Settings.Key?(Settings.Key.FieldStatsSendIntervalSecs)),
      new FunEventHandler.ServerPropHandler("restrict_groups", FunEventHandler.ToInt((Action<int>) (i => Settings.RestrictGroups = i != 0)), new Settings.Key?(Settings.Key.RestrictGroups)),
      new FunEventHandler.ServerPropHandler("announcement_groups", FunEventHandler.ToInt((Action<int>) (i => Settings.AnnouncementGroupSize = i)), new Settings.Key?(Settings.Key.AnnouncementGroupSize)),
      new FunEventHandler.ServerPropHandler("groups_v3", FunEventHandler.ToInt((Action<int>) (i => Settings.GroupsV3 = i != 0)), new Settings.Key?(Settings.Key.GroupsV3)),
      new FunEventHandler.ServerPropHandler("usync_sidelist", FunEventHandler.ToInt((Action<int>) (i => Settings.UsyncSidelist = i != 0)), new Settings.Key?(Settings.Key.UsyncSidelist)),
      new FunEventHandler.ServerPropHandler("announcement_toggle_time_hours", FunEventHandler.ToInt((Action<int>) (i => Settings.AnnouncementGroupToggleTimeHours = i)), new Settings.Key?(Settings.Key.AnnouncementGroupToggleTimeHours)),
      new FunEventHandler.ServerPropHandler("stickers", FunEventHandler.ToInt((Action<int>) (i => Settings.StickersEnabled = i != 0)), new Settings.Key?(Settings.Key.StickersEnabled)),
      new FunEventHandler.ServerPropHandler("gdpr_report", FunEventHandler.ToInt((Action<int>) (i => Settings.GdprReportEnabled = i != 0)), new Settings.Key?(Settings.Key.GdprReportEnabled)),
      new FunEventHandler.ServerPropHandler("tos2", (Action<string>) (s => GdprTos.ProcessServerProp(s)), new Settings.Key?()),
      new FunEventHandler.ServerPropHandler("sup_log_interval", (Action<string>) (s => Log.ProcessLogIntervalSetting(s)), new Settings.Key?(Settings.Key.CreateSupportLogIntervalHours), (Action) (() => Log.ProcessLogIntervalSetting((string) null))),
      new FunEventHandler.ServerPropHandler("fwd_ui_start_ts", FunEventHandler.ToLong((Action<long>) (l => Settings.ForwardedMsgUiTs = l)), new Settings.Key?(Settings.Key.ForwardedMsgUiTs)),
      new FunEventHandler.ServerPropHandler("multicast_limit_restricted", FunEventHandler.ToInt((Action<int>) (i => Settings.MulticastLimitRestricted = i)), new Settings.Key?(Settings.Key.MulticastLimitRestricted)),
      new FunEventHandler.ServerPropHandler("wp_ms_update_enabled", FunEventHandler.ToInt((Action<int>) (i => Settings.MSUpdateLinkEnabled = i != 0)), new Settings.Key?(Settings.Key.MSUpdateLinkEnabled)),
      new FunEventHandler.ServerPropHandler("multicast_limit_global", FunEventHandler.ToInt((Action<int>) (i => Settings.MulticastLimitGlobal = i)), new Settings.Key?(Settings.Key.MulticastLimitGlobal)),
      new FunEventHandler.ServerPropHandler("web_max_size_kb", FunEventHandler.ToInt((Action<int>) (i => Settings.WebMsgMaxSizeKb = i)), new Settings.Key?(Settings.Key.WebMsgMaxSizeKb)),
      new FunEventHandler.ServerPropHandler("wp_keep_stack", FunEventHandler.ToInt((Action<int>) (i => Settings.KeepStackTrace = i)), new Settings.Key?(Settings.Key.KeepStackTrace)),
      new FunEventHandler.ServerPropHandler("stickers_animation", FunEventHandler.ToInt((Action<int>) (i => Settings.StickerAnimationEnabled = i != 0)), new Settings.Key?(Settings.Key.StickerAnimationEnabled)),
      new FunEventHandler.ServerPropHandler("wp_inv_jid_sample", FunEventHandler.ToInt((Action<int>) (i => Settings.InvalidJidSampleRate = i)), new Settings.Key?(Settings.Key.InvalidJidSampleRate)),
      new FunEventHandler.ServerPropHandler("wp_inv_jid_secs", FunEventHandler.ToInt((Action<int>) (i => Settings.InvalidJidThrottleSeconds = i)), new Settings.Key?(Settings.Key.InvalidJidThrottleSeconds)),
      new FunEventHandler.ServerPropHandler("wp_final_rel", FunEventHandler.ToInt((Action<int>) (i => Settings.DeprecationFinalRelease = i)), new Settings.Key?(Settings.Key.DeprecationFinalRelease))
    };

    public FunEventHandler(FunXMPP.Connection c)
    {
      this.Connection = c;
      this.Presence.SetDisposeAtNextConnectionCallback((Action<IDisposable>) (d =>
      {
        lock (this.toDisposeLock)
          this.toDisposeOnLostConnection.Add(d);
      }));
    }

    public FunXMPP.Connection Connection { get; private set; }

    public FunXMPP.QrListener Qr => (FunXMPP.QrListener) this.qr;

    public void OnConnected()
    {
      Stats.OnConnected();
      VoipSignaling.OnConnected();
    }

    public void OnConnectionLost()
    {
      this.OnOfflineMessagesCompleted(true);
      lock (this.toDisposeLock)
      {
        this.toDisposeOnLostConnection.ForEach((Action<IDisposable>) (d => d.Dispose()));
        this.toDisposeOnLostConnection.Clear();
      }
      this.Presence.OnConnectionLost();
      VoipSignaling.OnDisconnected();
    }

    public void ClearLastSeenCache() => this.Presence.ClearCaches();

    public void OnOfflineMessage(FunXMPP.FMessage m)
    {
      try
      {
        Stats.OnOfflineMessage(m.key.remote_jid, m.key.id);
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "stats");
      }
    }

    public void OnOfflineMessagesCompleted(bool disconnect = false)
    {
      if (!disconnect)
      {
        Log.l("funhandler", "got offline marker");
        WAThreadPool.QueueUserWorkItem((Action) (() =>
        {
          try
          {
            FieldStatsRunner.CollectStats();
            FieldStatsRunner.TrySendStats();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "stats collect");
          }
        }));
        FunXMPP.Connection conn = AppState.ClientInstance.GetConnection();
        this.PerformWhenFullyRegistered((Action) (() =>
        {
          conn.SendPendingAcks();
          conn.SendPingAndClearDanglingAcks();
          if (!Settings.LastGroupsUpdatedUtc.HasValue || Settings.LastGroupsUpdatedUtc.Value.Add(TimeSpan.FromHours(24.0)) < FunRunner.CurrentServerTimeUtc)
          {
            Log.l("funhandler", "sync groups");
            conn.SendGetGroups();
          }
          if (Settings.ShouldQueryBroadcastLists)
            conn.SendQueryBroadcastLists((Action) (() => Settings.ShouldQueryBroadcastLists = false));
          Backup.MaybeUpdateBackupKey(this.Connection);
        }));
        this.OfflineCompletedSubject.OnNext(new Unit());
        this.Presence.OnLogin();
        if (!AppState.IsBackgroundAgent)
        {
          EmojiSearch.GetInstance();
          Log.TryUpload();
        }
        VoipSignaling.OnOfflineCompleted();
        Settings.ServerRequestedFibBackoffStateSaved = 0;
        try
        {
          if (OneDriveRestoreManager.Instance.MaybeStart())
            OneDriveBackupManager.Instance.MaybeStart();
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "exception starting OneDrive");
        }
        ChatMsgCounts.ProcessPending();
      }
      Stats.OnOfflineBurstReset();
    }

    private void PerformWhenFullyRegistered(Action a)
    {
      bool handled = false;
      IDisposable disposable = AppState.MonitorSettingsConditionObservable((Func<bool>) (() => Settings.PhoneNumberVerificationState == PhoneNumberVerificationState.Verified), Settings.Key.PhoneNumberVerificationState).Subscribe<Unit>((Action<Unit>) (unit =>
      {
        handled = true;
        a();
      }));
      if (handled)
        return;
      lock (this.toDisposeLock)
        this.toDisposeOnLostConnection.Add(disposable);
    }

    public void OnStatusUpdate(string jid, DateTime? timestamp, string @new, bool interactive)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        UserStatus userStatus = db.GetUserStatus(jid, interactive);
        if (userStatus == null)
          return;
        if (!(userStatus.Status != @new))
        {
          DateTime? dateTimeSet = userStatus.DateTimeSet;
          DateTime? nullable = timestamp;
          if ((dateTimeSet.HasValue == nullable.HasValue ? (dateTimeSet.HasValue ? (dateTimeSet.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
            return;
        }
        userStatus.Status = @new;
        userStatus.DateTimeSet = timestamp;
        db.SubmitChanges();
      }));
    }

    public void OnSyncNotification(
      string jid,
      byte[] jidHash,
      FunXMPP.Connection.ContactNotificationType type,
      Action<bool> ack)
    {
      ContactStore.ProcessContactNotificationHash(jid, jidHash, type, ack);
    }

    public void OnSidelistNotification(
      string jidHashB64,
      UsyncQuery.UsyncProtocol usyncProtocol,
      Action<bool> ack)
    {
      ContactStore.ProcessSidelistNotification(jidHashB64, usyncProtocol, ack);
    }

    public void OnChangeChatStaticKey(Action ack)
    {
      byte[] staticPrivate;
      byte[] staticPublic;
      WAProtocol.GenerateClientStaticKeyPair(out staticPublic, out staticPrivate);
      Action onComplete = (Action) (() =>
      {
        Settings.ClientStaticPrivateKey = staticPrivate;
        Settings.ClientStaticPublicKey = staticPublic;
        ack();
      });
      this.Connection.SendSetChatStaticPublicKey(staticPublic, onComplete);
    }

    public void OnMessageForMe(FunXMPP.FMessage fmsg)
    {
      bool duplicate = false;
      if (!fmsg.key.remote_jid.EndsWith("@s.us", StringComparison.Ordinal))
      {
        if (!fmsg.mms_retry)
        {
          DateTime? start = PerformanceTimer.Start();
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            Message message = db.GetMessage(fmsg.key.remote_jid, fmsg.key.id, fmsg.key.from_me);
            duplicate = message != null && message.MediaWaType != FunXMPP.FMessage.Type.CipherText;
          }));
          PerformanceTimer.End("de-dup", start);
        }
        if (!duplicate)
        {
          if (fmsg.encrypted != null)
            this.DecryptAndStoreIncomingMessage(fmsg);
          else
            this.StoreIncomingMessage(fmsg);
        }
        else
          Log.l("funhandler", "found duplicate msg | jid:{0},keyid:{1}", (object) fmsg.key.remote_jid, (object) fmsg.key.id);
      }
      if (!fmsg.wants_receipt)
        return;
      this.Connection.SendMessageReceived(fmsg);
    }

    public void OnLocationForMe(string participant, int elapsed, byte[] payload)
    {
      try
      {
        WhatsApp.ProtoBuf.Message fromPlainText = WhatsApp.ProtoBuf.Message.CreateFromPlainText(this.Connection.Encryption.DecryptFastRatchetPayload(payload, LiveLocationManager.LocationGroupJid, participant));
        LiveLocationManager.Instance.NewIncomingLocation(participant, elapsed, fromPlainText.LiveLocationMessageField);
        if (!AppState.GetConnection().EventHandler.Qr.Session.Active)
          return;
        AppState.GetConnection().SendQrLocationUpdate(participant, elapsed, fromPlainText);
      }
      catch (Axolotl.DecryptRetryException ex)
      {
        Log.l("E2EDecrypt", "Location Blob decryption fail:  - " + participant);
        if (!LiveLocationManager.HasLocationRetryDenied(participant))
        {
          int retryCount = ex.RetryCount;
          uint registrationId = this.Connection.Encryption.Store.LocalRegistrationId;
          this.Connection.Encryption.EnqueueToSendPreKeyComplete((Action) (() => this.Connection.SendLiveLocationKeyRetryNotification(participant, retryCount, registrationId)));
        }
        else
          Log.l("funhandler", "Location denied:  - " + participant);
      }
      catch (Exception ex)
      {
        Log.l(ex, "Fast ratchet decryption failed with non retriable error");
      }
    }

    public void OnLocationNotificationForMe(
      string group,
      string participant,
      int? expiration,
      int elapsed,
      byte[] payload)
    {
      this.OnLocationForMe(participant, elapsed, payload);
      LiveLocationManager.Instance.UpdateReceiverRecord(group, participant, expiration);
    }

    public void OnLocationKeyForMe(
      string group,
      string participant,
      int version,
      string type,
      int count,
      byte[] cipherText,
      Action ack)
    {
      Log.l("E2EDecrypt", "Decrypting location key from: " + group + " - " + participant);
      FunXMPP.FMessage message = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(participant, false, FunXMPP.GenerateMessageId()));
      message.encrypted = new FunXMPP.FMessage.Encrypted[1]
      {
        new FunXMPP.FMessage.Encrypted()
        {
          cipher_version = version,
          cipher_text_type = type,
          cipher_retry_count = count,
          cipher_text_bytes = cipherText
        }
      };
      try
      {
        LiveLocationManager.SetLocationRetryAllowed(participant, true);
        this.Connection.Encryption.DecryptMessage(message);
        ack();
      }
      catch (Axolotl.DecryptRetryException ex)
      {
        Log.l("E2EDecrypt", "Location key decryption fail: " + message.key.remote_jid + " - " + message.remote_resource);
        int retryCount = ex.RetryCount;
        uint registrationId = this.Connection.Encryption.Store.LocalRegistrationId;
        this.Connection.Encryption.EnqueueToSendPreKeyComplete((Action) (() =>
        {
          this.Connection.SendLiveLocationKeyRetryNotification(participant, retryCount, registrationId);
          ack();
        }));
      }
    }

    public void OnLocationKeyDenyForMe(string participant)
    {
      LiveLocationManager.SetLocationRetryAllowed(participant, false);
      LiveLocationManager.Instance.ReceiveLocationDisabled(participant);
    }

    private void DecryptAndStoreIncomingMessage(FunXMPP.FMessage message)
    {
      if (message.registrationId.HasValue)
      {
        uint? registrationId = message.registrationId;
        uint localRegistrationId = this.Connection.Encryption.Store.LocalRegistrationId;
        if (((int) registrationId.GetValueOrDefault() == (int) localRegistrationId ? (!registrationId.HasValue ? 1 : 0) : 1) != 0)
        {
          Log.l("E2EEncryption", "Local registrationid does not match server. Resending PreKeys");
          message.registrationId = new uint?();
          message.wants_receipt = false;
          this.Connection.Encryption.SendPreKeysToServer();
        }
      }
      int length = message.encrypted[0].cipher_text_bytes.Length;
      if (message.encrypted.Length > 1)
      {
        FunXMPP.FMessage.Encrypted encrypted = ((IEnumerable<FunXMPP.FMessage.Encrypted>) message.encrypted).Where<FunXMPP.FMessage.Encrypted>((Func<FunXMPP.FMessage.Encrypted, bool>) (enc => enc.cipher_text_type == "skmsg")).First<FunXMPP.FMessage.Encrypted>();
        if (encrypted != null)
          length = encrypted.cipher_text_bytes.Length;
      }
      Log.l("E2EDecrypt", "Decrypting message of size: " + (object) length + " from: " + message.key.remote_jid + " id: " + message.key.id + " - " + message.remote_resource);
      try
      {
        this.Connection.Encryption.DecryptMessage(message);
      }
      catch (Axolotl.DecryptRetryException ex)
      {
        Log.l("E2EDecrypt", "Sending retry receipt for: " + message.key.remote_jid + ", " + message.key.id + " - " + message.remote_resource);
        int retryCount = ex.RetryCount;
        uint registrationId = this.Connection.Encryption.Store.LocalRegistrationId;
        if (retryCount > 2)
          AppState.SchedulePersistentAction(PersistentAction.SendVerifyAxolotlDigest());
        this.Connection.Encryption.EnqueueToSendPreKeyComplete((Action) (() => this.Connection.SendEncryptedMessageRetry(message.key.remote_jid, message.key.id, message.remote_resource, 1, retryCount, message.timestamp, registrationId, message.mms_retry, message.edit_version)));
        if (retryCount == 1)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
          {
            if (db.MessageExists(message.key.remote_jid, message.key.id, message.key.from_me))
              return;
            db.InsertMessageOnSubmit(new Message(message)
            {
              MediaWaType = FunXMPP.FMessage.Type.CipherText
            });
            db.SubmitChanges();
          }));
        message.wants_receipt = false;
      }
      catch (Axolotl.DecryptUnknownTagsException ex)
      {
        Log.l("E2EDecrypt", "Sending unknown tags receipt for: " + message.key.remote_jid + ", " + message.key.id + " - " + message.remote_resource);
        this.Connection.SendEncryptedMessageUnknownTags(message.key.remote_jid, message.key.id, message.remote_resource, ex.UnknownTags);
        message.wants_receipt = false;
      }
    }

    public void StoreIncomingMessage(FunXMPP.FMessage fmsg)
    {
      string senderJid = fmsg.GetSenderJid();
      VerifiedNamesCertifier.OnIncomingMessage(senderJid, fmsg.verified_name, fmsg.verified_level, fmsg.verified_name_certificate);
      if (fmsg.media_wa_type == FunXMPP.FMessage.Type.HSM)
      {
        this.MarkReceivedHSMFromUser(senderJid);
        Log.l("StoreIncomingHSM", "creating persistent action");
        AppState.SchedulePersistentAction(PersistentAction.RehydrateHighlyStructuredMessage(fmsg.key.remote_jid, fmsg.key.id, fmsg.remote_resource, fmsg.timestamp ?? DateTime.UtcNow, senderJid, fmsg.verified_name, fmsg.binary_data));
        IcuDataManager.SetFullData();
      }
      else
        this.StoreIncomingMessageImpl(fmsg);
    }

    private void MarkReceivedHSMFromUser(string senderJid)
    {
      UserStatus user = UserCache.Get(senderJid, false);
      UserStatus userStatus = user;
      int num;
      if (userStatus == null)
      {
        num = 1;
      }
      else
      {
        bool? hasSentHsm = (bool?) userStatus.InternalProperties?.HasSentHsm;
        bool flag = true;
        num = hasSentHsm.GetValueOrDefault() == flag ? (!hasSentHsm.HasValue ? 1 : 0) : 1;
      }
      if (num == 0)
        return;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        user = user ?? db.GetUserStatus(senderJid);
        UserStatusProperties forUserStatus = UserStatusProperties.GetForUserStatus(user);
        forUserStatus.HasSentHsm = new bool?(true);
        forUserStatus.Save();
        db.SubmitChanges();
      }));
    }

    private void StoreIncomingMessageImpl(FunXMPP.FMessage fmsg)
    {
      bool writtenToDatabase = false;
      string updatedModifyTag = (string) null;
      Message updatedMsg = (Message) null;
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message1 = db.GetMessage(fmsg.key.remote_jid, fmsg.key.id, fmsg.key.from_me, false);
        if (fmsg.mms_retry)
        {
          Log.l("funhandler", "store incoming msg | mms retry | copying fields | keyid: {0}", (object) fmsg.key.id);
          if (message1 == null)
            return;
          MediaUpload.CopyMediaFields(message1, new Message(fmsg));
          db.SubmitChanges();
        }
        else
        {
          if (message1 != null && message1.MediaWaType == FunXMPP.FMessage.Type.ProtocolBuffer && fmsg.is_highly_structured_message_rehydrate)
            Log.l("funhandler", "store incoming msg | populating hsm forward message | keyid:{0}", (object) fmsg.key.id);
          else if (message1 != null && message1.MediaWaType != FunXMPP.FMessage.Type.CipherText)
          {
            Log.l("funhandler", "store incoming msg | skip | found duplicate msg | keyid:{0}", (object) fmsg.key.id);
            return;
          }
          switch (fmsg.media_wa_type)
          {
            case FunXMPP.FMessage.Type.ProtocolMessage:
            case FunXMPP.FMessage.Type.Empty:
              if (message1 != null && message1.MediaWaType == FunXMPP.FMessage.Type.CipherText)
                db.DeleteMessage(message1);
              if (fmsg.media_wa_type == FunXMPP.FMessage.Type.Empty)
                return;
              break;
          }
          string senderJid = fmsg.GetSenderJid();
          UserStatus user = (UserStatus) null;
          if (JidHelper.IsUserJid(senderJid))
          {
            user = UserCache.Get(senderJid, true);
            if (user.VerifiedName == VerifiedNameState.PendingCertification)
            {
              Log.l("funhandler", "Certification Pending message created {0} {1} {2}", (object) senderJid, (object) fmsg.key.remote_jid, (object) fmsg.key.id);
              WhatsApp.ProtoBuf.Message.CreateFromFMessage(fmsg, new CipherTextIncludes(true)).ToPlainText(false);
              PendingMessage pendingMessage = new PendingMessage(fmsg.key.remote_jid, fmsg.key.id, fmsg.remote_resource, fmsg.timestamp ?? DateTime.UtcNow, fmsg.SerializeToProtocolBuffer());
              if (fmsg.media_wa_type == FunXMPP.FMessage.Type.LiveLocation)
              {
                PendingMsgProperties forPendingMsg = PendingMsgProperties.GetForPendingMsg(pendingMessage);
                forPendingMsg.EnsureLiveLocationProperties.DurationSeconds = new int?(fmsg.media_duration_seconds);
                pendingMessage.InternalProperties = forPendingMsg;
              }
              db.InsertPendingMessageOnSubmit(pendingMessage);
              db.SubmitChanges();
              WAThreadPool.QueueUserWorkItem((Action) (() => VerifiedNamesCertifier.ScheduleCertifyVerifiedUserAction(senderJid, new DateTime?())));
              return;
            }
          }
          if (fmsg.edit_version == 7 && fmsg.media_wa_type == FunXMPP.FMessage.Type.ProtocolMessage)
          {
            MessageKey key = WhatsApp.ProtoBuf.Message.CreateFromPlainText(fmsg.proto_buf)?.ProtocolMessageField?.Key;
            if (key == null)
            {
              Log.l("funhandler", "skip revoke | missing revoke key | id:{0}", (object) fmsg.key.id);
              return;
            }
            Log.l("funhandler", "store incoming msg | revoke | revId:{0},origId:{1}", (object) fmsg.key.id, (object) key.Id);
            Message message2 = db.GetMessage(fmsg.key.remote_jid, key.Id, false);
            if (message2 == null)
            {
              AsyncRevoke.AddPendingRevoke(db, fmsg.key.remote_jid, key.Id, false, fmsg.key.remote_jid, fmsg.key.id, false);
              FieldStats.ReportRevokeRecv(fmsg);
              return;
            }
            if (message2.HasPaymentInfo())
            {
              Log.l("funhandler", "revoke message for payments message, ignoring -- " + message2.GetSenderJid() + " " + fmsg.GetSenderJid());
              return;
            }
            if (message2.GetSenderJid() != fmsg.GetSenderJid())
            {
              Log.l("funhandler", "revoke message sender DOESN'T match original message sender -- " + message2.GetSenderJid() + " " + fmsg.GetSenderJid());
              return;
            }
            DateTime? timestamp = fmsg.timestamp;
            DateTime? funTimestamp = message2.FunTimestamp;
            ref DateTime? local = ref funTimestamp;
            DateTime valueOrDefault;
            DateTime? nullable1;
            if (!local.HasValue)
            {
              nullable1 = new DateTime?();
            }
            else
            {
              valueOrDefault = local.GetValueOrDefault();
              nullable1 = new DateTime?(valueOrDefault.Add(Constants.RevokeIncomingExpiryTimeout));
            }
            DateTime? nullable2 = nullable1;
            if (nullable2.HasValue)
            {
              valueOrDefault = nullable2.Value;
              if (valueOrDefault.CompareTo(timestamp.Value) >= 0)
              {
                MessageProperties forMessage = MessageProperties.GetForMessage(message2);
                forMessage.EnsureCommonProperties.RevokedMsgId = key.Id;
                forMessage.Save();
                db.RevokeMessageOnSubmit(message2, out postDbAction);
                Conversation conversation = db.GetConversation(message2.KeyRemoteJid, CreateOptions.None);
                if (conversation != null)
                {
                  updatedModifyTag = conversation.UpdateModifyTag().ToString();
                  goto label_52;
                }
                else
                  goto label_52;
              }
            }
            Log.l("funhandler", "skip revoke | revoke timestamp is more than 24 hours later than original message timestamp | id:{0}, revokeTime:{1}, originalTime+1D:{2}", (object) fmsg.key.id, (object) timestamp, (object) nullable2);
            return;
          }
          if (message1 != null)
          {
            Log.l("funhandler", "store incoming msg | update place holder | original:{0}, new:{1}", (object) message1.KeyId, (object) fmsg.key.id);
            Message src = new Message(fmsg);
            long timestampLong = message1.TimestampLong;
            message1.CopyFrom((SqliteMessagesContext) db, src, true, true);
            message1.TimestampLong = timestampLong;
            if (message1.IsStatus() && db.GetWaStatus(senderJid, message1.MessageID) == null)
            {
              PersistentAction autoDownloadAction = (PersistentAction) null;
              db.SaveStatusForMessageOnSubmit(message1, out autoDownloadAction);
              if (autoDownloadAction != null)
                WAThreadPool.QueueUserWorkItem((Action) (() => AppState.AttemptPersistentAction(autoDownloadAction)));
            }
            updatedMsg = message1;
            Conversation conversation = db.GetConversation(message1.KeyRemoteJid, CreateOptions.None);
            if (conversation != null)
              updatedModifyTag = conversation.UpdateModifyTag().ToString();
          }
          else
          {
            Log.l("funhandler", "store incoming msg | insert new to db | keyid:{0}", (object) fmsg.key.id);
            Message m = new Message(fmsg);
            bool flag1 = m.IsStatus();
            bool flag2 = true;
            if (AsyncRevoke.IsRevokePending(db, m.KeyRemoteJid, m.KeyId))
            {
              if (m.HasPaymentInfo())
                Log.l("funhandler", "ignoring revoke for payments message");
              else if (flag1)
                flag2 = false;
              else
                m.MediaWaType = FunXMPP.FMessage.Type.Revoked;
              AsyncRevoke.RemovePendingRevoke(db, m.KeyRemoteJid, m.KeyId);
            }
            else if (flag1 && !JidHelper.IsPsaJid(senderJid) && m.IsStatusMessageExpired())
              flag2 = false;
            if (flag2)
            {
              db.InsertMessageOnSubmit(m);
              updatedMsg = m;
            }
          }
label_52:
          if (updatedMsg != null && (updatedMsg.MediaWaType == FunXMPP.FMessage.Type.Image || updatedMsg.MediaWaType == FunXMPP.FMessage.Type.Video || updatedMsg.MediaWaType == FunXMPP.FMessage.Type.Gif))
            updatedMsg.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateToDbIfNotFound);
          writtenToDatabase = true;
          db.SubmitChanges();
          if (updatedMsg == null || updatedMsg.PushName == null || user == null || user.IsVerified() || !(user.PushName != updatedMsg.PushName))
            return;
          this.UpdatePushName(updatedMsg.GetSenderJid(), updatedMsg.PushName);
        }
      }));
      Action action = postDbAction;
      if (action != null)
        action();
      if (!writtenToDatabase)
        return;
      if (fmsg.HasPaymentInfo() && PaymentsSettings.IsPaymentsEnabled())
        PaymentsHelper.ProcessIncomingDecryptedPayment(fmsg.message_properties?.PaymentsPropertiesField, fmsg.key.remote_jid, fmsg.remote_resource, fmsg.key.id);
      if (fmsg.offline < 0)
      {
        string participant = (string) null;
        if (JidHelper.IsGroupJid(fmsg.key.remote_jid) && !string.IsNullOrEmpty(fmsg.remote_resource))
          participant = fmsg.remote_resource;
        this.OnComposing(fmsg.key.remote_jid, participant, false, FunXMPP.FMessage.Type.Undefined);
      }
      else
        this.OnOfflineMessage(fmsg);
      FieldStats.ReportMessageReceive(fmsg);
      if (updatedModifyTag != null)
      {
        if (updatedMsg != null)
          AppState.QrPersistentAction.NotifyMessage(updatedMsg, QrMessageForwardType.Update);
        AppState.QrPersistentAction.NotifyChatStatus(fmsg.key.remote_jid, FunXMPP.ChatStatusForwardAction.ModifyTag);
      }
      Log.l("funhandler", "store incoming msg | complete | keyid:{0}", (object) fmsg.key.id);
    }

    private void UpdatePushName(string jid, string pushname)
    {
      AppState.Worker.Enqueue((Action) (() => ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        UserStatus userStatus = cdb.GetUserStatus(jid);
        if (!(userStatus.PushName != pushname))
          return;
        userStatus.PushName = pushname;
        cdb.SubmitChanges();
      }))));
    }

    public void OnEncryptionPreKeyCount(int count, Action ack)
    {
      if (this.Connection.Encryption == null)
        return;
      Log.l("E2EEncryption", "PreKeys low from server, Count: " + (object) count);
      this.Connection.Encryption.PreKeyServerCount(count, ack);
    }

    public void OnMessageRetryFromTarget(
      FunXMPP.FMessage.Key key,
      string participant,
      int version,
      int count,
      uint registration)
    {
      Log.l("E2EEncryption", "Message Encryption RetryFromTarget Jid: " + key.remote_jid + " KeyId: " + key.id + " Participant: " + participant + " Count: " + (object) count);
      if (this.Connection.Encryption == null)
        return;
      Message msg = (Message) null;
      bool retryMessage = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg = db.GetMessage(key.remote_jid, key.id, key.from_me);
        if (msg == null || !this.Connection.Encryption.ShouldSendMessageRetry(db, msg, participant))
          return;
        retryMessage = true;
        if (JidHelper.IsMultiParticipantsChatJid(key.remote_jid))
          return;
        msg.Status = FunXMPP.FMessage.Status.Unsent;
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureCommonProperties.CipherRetryCount = new int?(count);
        forMessage.Save();
        db.SubmitChanges();
      }));
      if (!retryMessage)
        return;
      if (!JidHelper.IsMultiParticipantsChatJid(key.remote_jid))
      {
        this.Connection.Encryption.MessageRetryFromTarget(msg, version, registration);
        AppState.SendMessage(this.Connection, msg);
      }
      else
        AppState.SchedulePersistentAction(PersistentAction.SendIndividualRetryForGroup(key.remote_jid, key.id, participant, count), true);
    }

    public void OnMessageReceipt(
      FunXMPP.FMessage.Key key,
      string participant,
      FunXMPP.FMessage.Status status,
      out bool readReceiptDowngraded,
      int? expectedDeliveryCount = null,
      DateTime? dt = null)
    {
      Log.d("funhandler", "receipt | jid:{0}, keyid:{1}, status:{2}, pjid:{3}, count:{4}", (object) key.remote_jid, (object) key.id, (object) status, (object) (participant ?? "-"), (object) (expectedDeliveryCount ?? -1));
      bool isReadReceiptDowngraded = false;
      Message msg = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool dirty = false;
        bool createdDelivery = false;
        msg = db.GetMessage(key.remote_jid, key.id, key.from_me);
        if (msg != null && key.from_me && (status == FunXMPP.FMessage.Status.ReceivedByServer || status == FunXMPP.FMessage.Status.ReceivedByTarget || status == FunXMPP.FMessage.Status.ReadByTarget || status == FunXMPP.FMessage.Status.PlayedByTarget))
        {
          if (status == FunXMPP.FMessage.Status.ReadByTarget && !Settings.EnableReadReceipts && !JidHelper.IsGroupJid(key.remote_jid))
          {
            isReadReceiptDowngraded = true;
            status = FunXMPP.FMessage.Status.ReceivedByTarget;
            Log.d("funhandler", "downgraded/ignored read receipt | keyid:{0}, jid:{1}", (object) key.id, (object) key.remote_jid);
          }
          List<ReceiptState> receiptsSoFar = db.AddReceiptForMessage(msg.MessageID, participant, status, dt ?? FunRunner.CurrentServerTimeUtc, out dirty, out createdDelivery);
          bool flag = false;
          if (JidHelper.IsMultiParticipantsChatJid(key.remote_jid))
          {
            if (dirty)
            {
              if (status == FunXMPP.FMessage.Status.ReceivedByServer)
              {
                this.ProcessMultiParticipantMessageServerReceipt(db, key, expectedDeliveryCount, receiptsSoFar, dt);
              }
              else
              {
                if (createdDelivery)
                  this.ProcessMultiParticipantMessageTargetReceipt(db, key, participant, FunXMPP.FMessage.Status.ReceivedByTarget, receiptsSoFar, dt);
                this.ProcessMultiParticipantMessageTargetReceipt(db, key, participant, status, receiptsSoFar, dt);
              }
            }
            else
              flag = status == FunXMPP.FMessage.Status.ReceivedByServer;
          }
          else
            flag = true;
          if (flag && this.UpdateMessageStatus(msg, status))
            dirty = true;
          if (status == FunXMPP.FMessage.Status.ReceivedByTarget && this.Connection.Encryption != null)
            this.Connection.Encryption.MessageRetryCleanup(msg, db);
          if (status == FunXMPP.FMessage.Status.ReceivedByServer)
          {
            MessageMiscInfo miscInfo = msg.GetMiscInfo((SqliteMessagesContext) db);
            if (miscInfo != null)
              miscInfo.ClientRetryCount = 0;
          }
        }
        if (!dirty)
          return;
        db.SubmitChanges();
      }));
      readReceiptDowngraded = isReadReceiptDowngraded;
      if (msg == null || status != FunXMPP.FMessage.Status.ReceivedByServer)
        return;
      FieldStats.ReportMessageSend(msg);
    }

    private bool UpdateMessageStatus(Message msg, FunXMPP.FMessage.Status status)
    {
      if (msg == null)
        return false;
      bool flag = false;
      if (msg.ShouldUpdateStatus(status))
      {
        Log.d(msg.LogInfo(), "status updated | {0}->{1}", (object) msg.Status, (object) status);
        msg.Status = status;
        flag = true;
      }
      else
        Log.d(msg.LogInfo(), "status updated skipped | {0}->{1}", (object) msg.Status, (object) status);
      return flag;
    }

    private void ProcessMultiParticipantMessageServerReceipt(
      MessagesContext db,
      FunXMPP.FMessage.Key key,
      int? ackedRecipientsCount,
      List<ReceiptState> receiptsSoFar,
      DateTime? dt)
    {
      if (db == null)
        return;
      Message message1 = db.GetMessage(key.remote_jid, key.id, key.from_me);
      if (message1 != null)
      {
        this.UpdateMessageStatus(message1, FunXMPP.FMessage.Status.ReceivedByServer);
        if (ackedRecipientsCount.HasValue)
        {
          MessageProperties forMessage = MessageProperties.GetForMessage(message1);
          forMessage.EnsureCommonProperties.AckedRecipientsCount = new int?(ackedRecipientsCount.Value);
          forMessage.Save();
          if (receiptsSoFar.Count > ackedRecipientsCount.Value)
          {
            FunXMPP.FMessage.Status status = FunXMPP.FMessage.Status.Undefined;
            if (this.IsReceiptsSoFarQualifiedForStatus(FunXMPP.FMessage.Status.ReceivedByTarget, receiptsSoFar, ackedRecipientsCount.Value))
            {
              status = FunXMPP.FMessage.Status.ReceivedByTarget;
              if (this.IsReceiptsSoFarQualifiedForStatus(FunXMPP.FMessage.Status.ReadByTarget, receiptsSoFar, ackedRecipientsCount.Value))
              {
                status = FunXMPP.FMessage.Status.ReadByTarget;
                if (this.IsReceiptsSoFarQualifiedForStatus(FunXMPP.FMessage.Status.PlayedByTarget, receiptsSoFar, ackedRecipientsCount.Value))
                  status = FunXMPP.FMessage.Status.PlayedByTarget;
              }
            }
            if (status != FunXMPP.FMessage.Status.Undefined)
            {
              Log.l("funhandler", "status correction | jid:{0},keyid:{1},new status:{2}", (object) key.remote_jid, (object) message1.KeyId, (object) status);
              this.UpdateMessageStatus(message1, status);
            }
          }
        }
        if (message1.IsStatus())
        {
          Message message2 = message1;
          DateTime? nullable1 = dt;
          DateTime? nullable2 = new DateTime?(nullable1 ?? FunRunner.CurrentServerTimeUtc);
          message2.FunTimestamp = nullable2;
          WaStatus waStatus1 = db.GetWaStatus(message1.GetSenderJid(), message1.MessageID);
          if (waStatus1 != null)
          {
            WaStatus waStatus2 = waStatus1;
            nullable1 = message1.FunTimestamp;
            DateTime dateTime = nullable1.Value;
            waStatus2.Timestamp = dateTime;
          }
        }
      }
      if (!JidHelper.IsBroadcastJid(key.remote_jid))
        return;
      foreach (Message msg in db.GetMessagesByKeyId(key.id, key.from_me))
        this.UpdateMessageStatus(msg, FunXMPP.FMessage.Status.ReceivedByServer);
    }

    private void ProcessMultiParticipantMessageTargetReceipt(
      MessagesContext db,
      FunXMPP.FMessage.Key key,
      string participant,
      FunXMPP.FMessage.Status status,
      List<ReceiptState> receiptsSoFar,
      DateTime? dt = null)
    {
      if (db == null || status != FunXMPP.FMessage.Status.ReceivedByTarget && status != FunXMPP.FMessage.Status.ReadByTarget && status != FunXMPP.FMessage.Status.PlayedByTarget)
        return;
      if (JidHelper.IsBroadcastJid(key.remote_jid))
      {
        Message message = db.GetMessage(participant, key.id, key.from_me);
        if (message != null)
        {
          bool flag = false;
          db.AddReceiptForMessage(message.MessageID, participant, status, dt ?? FunRunner.CurrentServerTimeUtc, out flag, out flag);
          this.UpdateMessageStatus(message, status);
        }
      }
      Message message1 = db.GetMessage(key.remote_jid, key.id, key.from_me);
      if (message1 == null || !message1.ShouldUpdateStatus(status) || !this.IsReceiptsSoFarQualifiedForStatus(status, receiptsSoFar, message1.GetExpectedDeliveryCount(db)))
        return;
      this.UpdateMessageStatus(message1, status);
      AppState.QrPersistentAction.NotifyStatus(key, status);
    }

    private bool IsReceiptsSoFarQualifiedForStatus(
      FunXMPP.FMessage.Status targetStatus,
      List<ReceiptState> receiptsSoFar,
      int expectedCount)
    {
      IGrouping<string, ReceiptState>[] array1 = receiptsSoFar.GroupBy<ReceiptState, string>((Func<ReceiptState, string>) (rs => rs.Jid)).ToArray<IGrouping<string, ReceiptState>>();
      if (array1.Length < expectedCount)
        return false;
      ReceiptState[] array2 = ((IEnumerable<IGrouping<string, ReceiptState>>) array1).Select<IGrouping<string, ReceiptState>, ReceiptState>((Func<IGrouping<string, ReceiptState>, ReceiptState>) (receiptsForRecipientJid => receiptsForRecipientJid.MaxOfFunc<ReceiptState, int>((Func<ReceiptState, int>) (rs => rs.Status.GetOverrideWeight())))).ToArray<ReceiptState>();
      int targetStatusWeight = targetStatus.GetOverrideWeight();
      Func<ReceiptState, bool> predicate = (Func<ReceiptState, bool>) (rs => rs.Status.GetOverrideWeight() >= targetStatusWeight);
      return ((IEnumerable<ReceiptState>) array2).Count<ReceiptState>(predicate) >= expectedCount;
    }

    public void OnMessageError(FunXMPP.FMessage.Key key, int code)
    {
      Message msg = (Message) null;
      bool resend = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        msg = db.GetMessage(key.remote_jid, key.id, key.from_me);
        if (msg == null)
        {
          Log.l("funhandler", "got error code {0} | message {1} {2} | message doesn't exist", (object) code, key.from_me ? (object) "to" : (object) "from", (object) key.remote_jid);
        }
        else
        {
          Log.l("funhandler", "got error code {0} | {1}={2}, msg_id={3}", (object) code, key.from_me ? (object) "to" : (object) "from", (object) key.remote_jid, (object) msg.MessageID);
          MessageMiscInfo.MessageError errType = MessageMiscInfo.MessageError.None;
          switch (code)
          {
            case 400:
              errType = MessageMiscInfo.MessageError.BadRequest;
              break;
            case 401:
              errType = MessageMiscInfo.MessageError.NotAuthorized;
              break;
            case 403:
              errType = MessageMiscInfo.MessageError.Forbidden;
              break;
            case 404:
              errType = MessageMiscInfo.MessageError.ItemNotFound;
              break;
            case 405:
              errType = MessageMiscInfo.MessageError.NotAllowed;
              break;
            case 406:
              errType = MessageMiscInfo.MessageError.NotAcceptable;
              break;
            case 409:
              errType = MessageMiscInfo.MessageError.ParticipantsHashMismatch;
              resend = true;
              break;
            case 410:
              errType = MessageMiscInfo.MessageError.FileGone;
              break;
            case 420:
              errType = MessageMiscInfo.MessageError.NotAdmin;
              break;
            case 500:
              errType = MessageMiscInfo.MessageError.ServerError;
              break;
            case 501:
              errType = MessageMiscInfo.MessageError.NotImplemented;
              break;
          }
          if ((errType == MessageMiscInfo.MessageError.NotAuthorized || errType == MessageMiscInfo.MessageError.NotAdmin) && JidHelper.IsGroupJid(msg.KeyRemoteJid))
          {
            Message errorMessage = SystemMessageUtils.CreateErrorMessage(db, msg.KeyRemoteJid, errType);
            db.InsertMessageOnSubmit(errorMessage);
          }
          if (errType != MessageMiscInfo.MessageError.None)
            msg.GetMiscInfo((SqliteMessagesContext) db, CreateOptions.CreateToDbIfNotFound).ErrorCode = new int?((int) errType);
          if (resend)
          {
            msg.Status = FunXMPP.FMessage.Status.Unsent;
          }
          else
          {
            msg.Status = FunXMPP.FMessage.Status.Error;
            if (AppState.GetConnection().EventHandler.Qr.Session.Active)
              AppState.GetConnection().SendQrReceived(new FunXMPP.FMessage.Key(msg.KeyRemoteJid, msg.KeyFromMe, msg.KeyId), FunXMPP.FMessage.Status.Error);
          }
          db.SubmitChanges();
        }
      }));
      if (!(msg != null & resend))
        return;
      AppState.Worker.Enqueue((Action) (() =>
      {
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.GetConversation(msg.KeyRemoteJid, CreateOptions.None)?.UpdateParticipantsHash()));
        AppState.SendMessage(this.Connection, msg);
      }));
    }

    public void OnPing(string id) => this.Connection.SendPong(id);

    public void OnPingResponseReceived()
    {
    }

    public void OnAvailable(string jid, bool isAvailable, DateTime? dt = null)
    {
      this.Presence.OnAvailable(jid, isAvailable, dt);
    }

    public void OnClientConfigReceived(string push_id)
    {
    }

    public void OnComposing(
      string jid,
      string participant,
      bool isComposing,
      FunXMPP.FMessage.Type mediaType = FunXMPP.FMessage.Type.Undefined)
    {
      this.Presence.OnComposing(jid, participant, isComposing, mediaType);
    }

    public void OnPrivacyBlockList(IEnumerable<string> jids)
    {
      ContactsContext.Instance((Action<ContactsContext>) (contacts =>
      {
        Dictionary<string, bool> blockListSet = contacts.BlockListSet;
        blockListSet.Clear();
        foreach (string jid in jids)
          blockListSet.Add(jid, true);
        contacts.FlushBlockList();
        contacts.SubmitChanges();
        Settings.LastBlockListCheckUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
      }));
    }

    public void OnPrivacySettings(Dictionary<string, PrivacyVisibility> settings)
    {
      \u003C\u003Ef__AnonymousType7<string, Action<PrivacyVisibility>>[] dataArray = new \u003C\u003Ef__AnonymousType7<string, Action<PrivacyVisibility>>[4]
      {
        new
        {
          Key = "last",
          Set = (Action<PrivacyVisibility>) (v =>
          {
            Settings.LastSeenVisibility = v;
            Settings.LastSeenVisibilityInDoubt = false;
          })
        },
        new
        {
          Key = "profile",
          Set = (Action<PrivacyVisibility>) (v =>
          {
            Settings.ProfilePhotoVisibility = v;
            Settings.ProfilePhotoVisibilityInDoubt = false;
          })
        },
        new
        {
          Key = "status",
          Set = (Action<PrivacyVisibility>) (v =>
          {
            Settings.StatusVisibility = v;
            Settings.StatusVisibilityInDoubt = false;
          })
        },
        new
        {
          Key = "readreceipts",
          Set = (Action<PrivacyVisibility>) (v =>
          {
            if (v == PrivacyVisibility.None)
              Settings.EnableReadReceipts = false;
            if (v == PrivacyVisibility.Everyone)
              Settings.EnableReadReceipts = true;
            Settings.EnableReadReceiptInDoubt = false;
          })
        }
      };
      foreach (var data in dataArray)
      {
        PrivacyVisibility privacyVisibility;
        if (settings.TryGetValue(data.Key, out privacyVisibility))
          data.Set(privacyVisibility);
      }
      Settings.LastPrivacyCheckUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
    }

    public void OnDirty(string type, long timestamp)
    {
      if (type == "groups")
        this.PerformWhenFullyRegistered((Action) (() => this.Connection.SendGetGroups()));
      else
        this.Connection.SendClearDirty(type);
    }

    public void OnSonar(string url) => SonarTest.runTest(url);

    public void OnContactChangeNumber(string oldJid, string newJid, DateTime? dtUtc)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db => db.AddChangeNumberRecord(oldJid, newJid, dtUtc ?? FunRunner.CurrentServerTimeUtc)));
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (!JidHelper.IsUserJid(oldJid) || !JidHelper.IsUserJid(newJid))
          return;
        Message waNumberChanged1 = SystemMessageUtils.CreateWaNumberChanged(db, oldJid, newJid, oldJid, dtUtc);
        Message waNumberChanged2 = SystemMessageUtils.CreateWaNumberChanged(db, newJid, newJid, oldJid, dtUtc);
        db.InsertMessageOnSubmit(waNumberChanged1);
        db.InsertMessageOnSubmit(waNumberChanged2);
        db.SubmitChanges();
      }));
    }

    public void OnGroupParticipantChangeNumber(
      string gjid,
      string oldJid,
      string newJid,
      DateTime? dt = null)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message participantNumberChanged = SystemMessageUtils.CreateGroupParticipantNumberChanged(db, gjid, newJid, oldJid, dt);
        Conversation conversation1 = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound);
        if (conversation1 != null)
        {
          bool wasAdmin = false;
          conversation1.ParticipantSetAction((Action<GroupParticipants>) (participants => wasAdmin = participants.IsAdmin(oldJid)));
          Conversation conversation2 = conversation1;
          MessagesContext db1 = db;
          Dictionary<string, bool> toAdd = new Dictionary<string, bool>();
          toAdd.Add(newJid, wasAdmin);
          string[] toRemove = new string[1]{ oldJid };
          conversation2.UpdateParticipants((SqliteMessagesContext) db1, toAdd, (IEnumerable<string>) toRemove);
        }
        db.InsertMessageOnSubmit(participantNumberChanged);
        db.SubmitChanges();
        this.NotifyGroupInfoChange(conversation1, FunEventHandler.GroupInfoDirtyFlags.GroupMembers);
      }));
    }

    public void OnGroupAddUser(
      string gjid,
      string jid,
      string author,
      DateTime? dt,
      string reason)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        CreateResult result = CreateResult.None;
        Conversation conversation = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound, out result);
        if (conversation.ContainsParticipant(jid))
        {
          if (result != CreateResult.CreatedToDb)
            return;
          db.SubmitChanges();
        }
        else
        {
          if (conversation.GetParticipantCount() == 0)
            AppState.ClientInstance?.GetConnection()?.SendGetGroupInfo(gjid);
          SystemMessageUtils.ParticipantChange change = jid == author ? SystemMessageUtils.ParticipantChange.Join : SystemMessageUtils.ParticipantChange.Added;
          if (reason == "invite")
            change = SystemMessageUtils.ParticipantChange.Invite;
          Message participantChange = this.GenerateParticipantChange(db, gjid, jid, author, change, dt);
          db.InsertMessageOnSubmit(participantChange);
          conversation.AddParticipant(db, jid);
          db.SubmitChanges();
          this.NotifyGroupInfoChange(conversation, FunEventHandler.GroupInfoDirtyFlags.GroupMembers);
        }
      }));
      this.Presence.OnUserJoinedGroup(gjid, jid);
    }

    public void OnGroupRemoveUserImpl(
      string gjid,
      string jid,
      string author,
      DateTime? dt,
      string subject = null,
      bool sysMsg = true)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.None);
        if (conversation == null || !sysMsg && !conversation.IsGroupParticipant())
          return;
        FunEventHandler.GroupInfoDirtyFlags flags = FunEventHandler.GroupInfoDirtyFlags.None;
        if (subject != null && subject != conversation.GroupSubject)
        {
          conversation.GroupSubject = subject;
          flags |= FunEventHandler.GroupInfoDirtyFlags.ConversationMetadata;
        }
        if (conversation.ContainsParticipant(jid))
        {
          SystemMessageUtils.ParticipantChange change = author != jid ? SystemMessageUtils.ParticipantChange.Removed : SystemMessageUtils.ParticipantChange.Leave;
          if (sysMsg)
          {
            Message participantChange = this.GenerateParticipantChange(db, gjid, jid, author, change, dt);
            db.InsertMessageOnSubmit(participantChange);
          }
          conversation.RemoveParticipant(db, jid);
          flags |= FunEventHandler.GroupInfoDirtyFlags.GroupMembers | FunEventHandler.GroupInfoDirtyFlags.SystemMessages;
        }
        if (flags == FunEventHandler.GroupInfoDirtyFlags.None)
          return;
        db.SubmitChanges();
        this.NotifyGroupInfoChange(conversation, flags);
      }));
    }

    public void OnGroupRemoveUser(
      string gjid,
      string jid,
      string author,
      DateTime? dt,
      string subject = null,
      bool sysMsg = true)
    {
      this.OnGroupRemoveUserImpl(gjid, jid, author, dt, subject, sysMsg);
    }

    public void OnPromoteUsers(string gjid, IEnumerable<string> jids, DateTime? dt)
    {
      List<Message> newMsgs = new List<Message>();
      Dictionary<string, bool> adminsAdded = new Dictionary<string, bool>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.CreateAndSubmitIfNotFound);
        bool dirty = false;
        conversation.ParticipantSetAction((Action<GroupParticipants>) (set =>
        {
          foreach (string jid in jids)
          {
            if (!set.IsAdmin(jid))
            {
              set.SetAdmin(jid);
              adminsAdded[jid] = true;
              dirty = true;
            }
          }
          if (!dirty)
            return;
          set.Flush();
        }));
        newMsgs.AddRange(this.GenerateAdminChanges<bool>(db, gjid, adminsAdded, (Dictionary<string, bool>) null, dt));
        newMsgs.ForEach((Action<Message>) (m => db.InsertMessageOnSubmit(m)));
        if (dirty)
          db.SubmitChanges();
        if (adminsAdded.Count <= 0)
          return;
        this.NotifyGroupInfoChange(conversation, FunEventHandler.GroupInfoDirtyFlags.GroupAdmins);
      }));
    }

    public void OnDemoteUsers(string gjid, IEnumerable<string> jids, DateTime? dt)
    {
      List<Message> newMsgs = new List<Message>();
      Dictionary<string, bool> adminsRemoved = new Dictionary<string, bool>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.CreateAndSubmitIfNotFound);
        bool dirty = false;
        conversation.ParticipantSetAction((Action<GroupParticipants>) (set =>
        {
          foreach (string jid in jids)
          {
            if (set.IsAdmin(jid))
            {
              set.RemoveAdmin(jid);
              adminsRemoved[jid] = true;
              dirty = true;
            }
          }
          if (!dirty)
            return;
          set.Flush();
        }));
        newMsgs.AddRange(this.GenerateAdminChanges<bool>(db, gjid, (Dictionary<string, bool>) null, adminsRemoved, dt));
        newMsgs.ForEach((Action<Message>) (m => db.InsertMessageOnSubmit(m)));
        if (dirty)
          db.SubmitChanges();
        if (adminsRemoved.Count == 0)
          return;
        this.NotifyGroupInfoChange(conversation, FunEventHandler.GroupInfoDirtyFlags.GroupAdmins);
      }));
    }

    public void OnGroupLocked(string gjid, string ujid, DateTime? dt)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound);
        if (conversation == null || conversation.IsLocked())
          return;
        conversation.Lock();
        Message restrictionLocked = SystemMessageUtils.CreateGroupRestrictionLocked(db, gjid, ujid, dt);
        restrictionLocked.PushName = JidHelper.GetDisplayNameForContactJid(ujid);
        db.InsertMessageOnSubmit(restrictionLocked);
        db.SubmitChanges();
      }));
    }

    public void OnGroupUnlocked(string gjid, string ujid, DateTime? dt)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.None);
        if (conversation == null || !conversation.IsLocked())
          return;
        conversation.Unlock();
        Message restrictionUnlocked = SystemMessageUtils.CreateGroupRestrictionUnlocked(db, gjid, ujid, dt);
        restrictionUnlocked.PushName = JidHelper.GetDisplayNameForContactJid(ujid);
        db.InsertMessageOnSubmit(restrictionUnlocked);
        db.SubmitChanges();
      }));
    }

    public void OnGroupAnnounceOnly(string gjid, string ujid, DateTime? dt)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound);
        if (conversation == null || conversation.IsAnnounceOnly())
          return;
        conversation.MakeAnnounceOnly();
        Message announcementOnly = SystemMessageUtils.CreateGroupMadeAnnouncementOnly(db, gjid, ujid, dt);
        announcementOnly.PushName = JidHelper.GetDisplayNameForContactJid(ujid);
        db.InsertMessageOnSubmit(announcementOnly);
        db.SubmitChanges();
      }));
    }

    public void OnGroupNotAnnounceOnly(string gjid, string ujid, DateTime? dt)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.None);
        if (conversation == null || !conversation.IsAnnounceOnly())
          return;
        conversation.MakeNotAnnounceOnly();
        Message announcementOnly = SystemMessageUtils.CreateGroupMadeNotAnnouncementOnly(db, gjid, ujid, dt);
        announcementOnly.PushName = JidHelper.GetDisplayNameForContactJid(ujid);
        db.InsertMessageOnSubmit(announcementOnly);
        db.SubmitChanges();
      }));
    }

    public void OnInvitationCode(string gjid, string author, string inviteCode, DateTime? dt)
    {
      if (!dt.HasValue)
        dt = new DateTime?(FunRunner.CurrentServerTimeUtc);
      Log.l("funhandler", "group invite changed | jid:{0}, code:{1}", (object) gjid, (object) inviteCode);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message groupInviteChanged = SystemMessageUtils.CreateGroupInviteChanged(db, gjid, author, inviteCode, dt);
        db.InsertMessageOnSubmit(groupInviteChanged);
        db.SubmitChanges();
      }));
    }

    public void OnGroupDisbanded(string gjid, string author, DateTime? dt)
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(gjid, CreateOptions.None);
        if (conversation == null || conversation.HasFlag(Conversation.ConversationFlags.Deleted))
          return;
        conversation.SetFlag(Conversation.ConversationFlags.Deleted);
        Message groupDeleted = SystemMessageUtils.CreateGroupDeleted(db, gjid, author, dt);
        db.InsertMessageOnSubmit(groupDeleted);
        db.SubmitChanges();
      }));
    }

    private Message GenerateParticipantChange(
      MessagesContext db,
      string gjid,
      string jid,
      string author,
      SystemMessageUtils.ParticipantChange change,
      DateTime? dt = null)
    {
      return SystemMessageUtils.CreateParticipantChanged(db, change, gjid, jid, author, dt);
    }

    private IEnumerable<Message> GenerateAdminChanges<T>(
      MessagesContext db,
      string gjid,
      Dictionary<string, T> adminsAdded,
      Dictionary<string, T> adminsRemoved,
      DateTime? dt)
    {
      if (adminsAdded != null && adminsAdded.ContainsKey(Settings.MyJid))
        yield return SystemMessageUtils.CreateGroupAdminGained(db, gjid, Settings.MyJid, dt);
      if (adminsRemoved != null && adminsRemoved.ContainsKey(Settings.MyJid))
        yield return SystemMessageUtils.CreateGroupAdminLost(db, gjid, Settings.MyJid, dt);
    }

    public void OnGroupNewSubject(
      string gjid,
      string ujid,
      string pushName,
      string subject,
      DateTime? timestamp)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound);
        DateTime? nullable = timestamp;
        DateTime dtUtc = nullable ?? DateTime.UtcNow;
        nullable = convo.GroupSubjectT;
        if (nullable.HasValue)
        {
          DateTime dateTime = dtUtc;
          nullable = convo.GroupSubjectT;
          if ((nullable.HasValue ? (dateTime > nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            goto label_3;
        }
        convo.GroupSubjectOwner = ujid;
        convo.GroupSubject = subject;
        convo.GroupSubjectPerformanceHint = LinkDetector.Result.Serialize(LinkDetector.GetMatches(Emoji.ConvertToUnicode(subject)));
        convo.GroupSubjectT = new DateTime?(dtUtc);
label_3:
        Message subjectChanged = SystemMessageUtils.CreateSubjectChanged(db, gjid, ujid, subject, dtUtc);
        subjectChanged.PushName = pushName;
        db.InsertMessageOnSubmit(subjectChanged);
        db.SubmitChanges();
      }));
      ITile chatTile = TileHelper.GetChatTile(gjid);
      if (chatTile == null || convo == null)
        return;
      chatTile.SetTitle(convo.GetName());
      chatTile.Update();
    }

    public void OnGroupNewDescription(
      string gjid,
      string ujid,
      string id,
      string description,
      DateTime? timestamp)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        convo = db.GetConversation(gjid, CreateOptions.CreateToDbIfNotFound);
        DateTime? nullable = timestamp;
        DateTime dtUtc = nullable ?? DateTime.UtcNow;
        nullable = convo.GroupDescriptionT;
        if (nullable.HasValue && (!string.IsNullOrEmpty(description) || !(id == convo.GroupDescriptionId)))
        {
          DateTime dateTime = dtUtc;
          nullable = convo.GroupDescriptionT;
          if ((nullable.HasValue ? (dateTime > nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0 || !(id != convo.GroupDescriptionId))
            return;
        }
        convo.GroupDescriptionOwner = ujid;
        convo.GroupDescription = description;
        convo.GroupDescriptionT = new DateTime?(dtUtc);
        convo.GroupDescriptionId = id;
        Message m = string.IsNullOrEmpty(description) ? SystemMessageUtils.CreateGroupDescriptionDeleted(db, gjid, ujid, dtUtc) : SystemMessageUtils.CreateGroupDescriptionChanged(db, gjid, ujid, dtUtc);
        m.PushName = JidHelper.GetDisplayNameForContactJid(ujid);
        db.InsertMessageOnSubmit(m);
        db.SubmitChanges();
      }));
    }

    public void OnSelfSetNewPhoto(
      string jid,
      string photoId,
      bool createSysMessage,
      byte[] smallPicBytes,
      byte[] largePicBytes,
      string context = null)
    {
      if (ChatPictureStore.IsPictureIdUptoDate(jid, photoId))
      {
        Log.l("funhandler", "self pic not changed | jid={0} pid={1}", (object) jid, (object) photoId);
      }
      else
      {
        Log.l("funhandler", "self pic changed | jid={0} pid={1}", (object) jid, (object) photoId);
        this.OnPhotoChanged(jid, photoId, smallPicBytes, largePicBytes, context);
        this.onFetchComplete(jid, photoId, Settings.MyJid, createSysMessage);
      }
    }

    public void OnNewPhotoIdFetched(
      string jid,
      string authorJid,
      string photoId,
      bool createSysMessage,
      string context = null)
    {
      Log.l("funhandler", "on new photo id fetched, jid={0}, photo_id={1}, context={2}", (object) jid, (object) photoId, (object) context);
      bool upToDate = ChatPictureStore.IsPictureIdUptoDate(jid, photoId);
      if (upToDate)
        Log.l("funhandler", "pic not changed | jid={0} pid={1}", (object) jid, (object) photoId);
      else if (photoId != null)
      {
        ChatPictureStore.fetchNewPhoto(jid, (Action) (() =>
        {
          upToDate = ChatPictureStore.IsPictureIdUptoDate(jid, photoId);
          if (upToDate)
          {
            Log.l("funhandler", "pic not changed, second check | jid={0} pid={1}", (object) jid, (object) photoId);
          }
          else
          {
            this.onFetchComplete(jid, photoId, authorJid, createSysMessage);
            Log.l("funhandler", "photo id changed | jid={0} pid={1} context={2}", (object) jid, (object) photoId, (object) context);
          }
        }));
      }
      else
      {
        ChatPictureStore.Reset(jid, new DateTime?(DateTime.UtcNow));
        this.onFetchComplete(jid, photoId, authorJid, createSysMessage);
        Log.l("funhandler", "photo id reset | jid={0} pid={1} context={2}", (object) jid, (object) photoId, (object) context);
      }
    }

    private void onFetchComplete(
      string jid,
      string photoId,
      string authorJid,
      bool createSysMessage)
    {
      if (!createSysMessage || !JidHelper.IsGroupJid(jid))
        return;
      Message sysMsg = (Message) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        CreateResult result;
        Conversation conversation = db.GetConversation(jid, CreateOptions.CreateToDbIfNotFound, out result);
        bool flag = result == CreateResult.CreatedToDb;
        if (conversation != null && authorJid != null)
        {
          sysMsg = SystemMessageUtils.CreateGroupPictureChanged(db, jid, authorJid, photoId == null);
          db.InsertMessageOnSubmit(sysMsg);
          flag = true;
        }
        if (!flag)
          return;
        db.SubmitChanges();
      }));
    }

    public void OnPhotoChanged(
      string jid,
      string photoId,
      byte[] smallPicBytes,
      byte[] largePicBytes,
      string context = null)
    {
      ChatPictureStore.UpdatePictureId(jid, photoId);
      Log.l("funhandler", "photo data for jid={0} pid={1} | schedule save", (object) jid, (object) (photoId ?? "-"));
      AppState.ImageWorker.Enqueue((Action) (() => ChatPictureStore.UpdatePictureData(jid, photoId, smallPicBytes, largePicBytes)));
    }

    public void OnPhotoReuploadRequested(string jid)
    {
      Log.l("funhandler", "photo reupload requested for jid={0}", (object) jid);
      if (jid != Settings.MyJid)
        return;
      string photoId = (string) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        ChatPicture chatPictureState = db.GetChatPictureState(jid, CreateOptions.None);
        if (chatPictureState == null)
          return;
        photoId = chatPictureState.WaPhotoId;
      }));
      if (photoId == null)
        return;
      Stream picStream = ChatPictureStore.GetStoredPictureStream(jid, photoId, true);
      if (picStream == null)
        return;
      Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
      {
        WriteableBitmap bitmap = (WriteableBitmap) null;
        using (picStream)
          bitmap = BitmapUtils.CreateBitmap(picStream);
        if (bitmap == null)
          return;
        AppState.SchedulePersistentAction(PersistentAction.SetPhoto(jid, bitmap.ToJpegByteArray(96, 96, -1, new int?(Settings.JpegQuality)), bitmap.ToJpegByteArray(640, 640, -1, new int?(Settings.JpegQuality)), false));
      }));
    }

    private static Action<string> ToInt(Action<int> intCallback)
    {
      return (Action<string>) (s =>
      {
        int result = 0;
        if (s == null || !int.TryParse(s, out result))
          return;
        intCallback(result);
      });
    }

    private static Action<string> ToLong(Action<long> longCallback)
    {
      return (Action<string>) (str =>
      {
        long result;
        if (!long.TryParse(str, out result))
          return;
        longCallback(result);
      });
    }

    public void OnServerProperties(Dictionary<string, string> nameValueMap)
    {
      try
      {
        foreach (KeyValuePair<string, string> nameValue in nameValueMap)
          Log.d("funhandler", "server prop | {0}: {1}", (object) nameValue.Key, (object) nameValue.Value);
        Set<Settings.Key> names = new Set<Settings.Key>();
        names.Add(Settings.Key.ForceServerPropsReload);
        bool flag = false;
        foreach (FunEventHandler.ServerPropHandler serverPropHandler in FunEventHandler.ServerPropHandlers)
        {
          string str;
          if (nameValueMap.TryGetValue(serverPropHandler.Key, out str))
          {
            serverPropHandler.Action(str);
          }
          else
          {
            if (serverPropHandler.AssociatedSettingsKey.HasValue)
            {
              Log.d("funhandler", "server prop | missed | {0}, database key:{1}", (object) serverPropHandler.Key, (object) serverPropHandler.AssociatedSettingsKey.Value);
              names.Add(serverPropHandler.AssociatedSettingsKey.Value);
            }
            if (serverPropHandler.PropNotSuppliedAction != null)
            {
              Log.d("funhandler", "server prop | missed | {0}, running removal action", (object) serverPropHandler.Key);
              try
              {
                serverPropHandler.PropNotSuppliedAction();
              }
              catch (Exception ex)
              {
                string context = "Exception running prop removal action for " + serverPropHandler.Key;
                Log.LogException(ex, context);
                flag = true;
              }
            }
          }
        }
        Settings.DeleteMany((IEnumerable<Settings.Key>) names);
        if (flag)
          Log.SendCrashLog((Exception) new InvalidOperationException("Exception running prop removal"), "Exception running prop removal", logOnlyForRelease: true);
        Settings.LastPropertiesQueryUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
      }
      catch (Exception ex)
      {
        Log.l(ex, "funhandler exception processing server props");
        Settings.ForceServerPropsReload = true;
        throw;
      }
    }

    public void OnBroadcastListInfo(string blJid, string blName, IEnumerable<string> recipientJids)
    {
      Log.d("funhandler", "list info | {0} {1} {2} recipients", (object) blJid, (object) blName, (object) recipientJids.Count<string>());
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        CreateResult result = CreateResult.None;
        Conversation bl = db.GetConversation(blJid, CreateOptions.CreateToDbIfNotFound, out result);
        bool dirty = result == CreateResult.CreatedToDb;
        if (string.IsNullOrEmpty(bl.GroupSubject) && !string.IsNullOrEmpty(blName))
        {
          bl.GroupSubject = blName;
          dirty = true;
        }
        bl.ParticipantSetAction((Action<GroupParticipants>) (participantSet =>
        {
          if (participantSet.Count != 0)
            return;
          bl.UpdateParticipants((SqliteMessagesContext) db, recipientJids.Where<string>((Func<string, bool>) (jid => JidHelper.IsUserJid(jid))).MakeUnique<string>().ToDictionary<string, string, bool>((Func<string, string>) (jid => jid), (Func<string, bool>) (jid => false)), (IEnumerable<string>) null);
          dirty = true;
        }));
        if (!dirty)
          return;
        db.SubmitChanges();
      }));
    }

    public void OnGroupInfo(FunXMPP.Connection.GroupInfo info, bool checkParticipants = true)
    {
      List<Action> actions = new List<Action>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        CreateResult result;
        Conversation conversation = db.GetConversation(info.Jid, CreateOptions.CreateToDbIfNotFound, out result);
        MessagesContext db1 = db;
        Conversation convo = conversation;
        FunXMPP.Connection.GroupInfo info1 = info;
        Action<Action> submitPostSubmitJob = new Action<Action>(actions.Add);
        DateTime? dt = new DateTime?();
        int num = checkParticipants ? 1 : 0;
        FunEventHandler.GroupInfoDirtyFlags flags;
        if ((flags = this.OnGroupInfo(db1, convo, info1, submitPostSubmitJob, dt, num != 0)) == FunEventHandler.GroupInfoDirtyFlags.None && result != CreateResult.CreatedToDb)
          return;
        db.SubmitChanges();
        this.NotifyGroupInfoChange(conversation, flags);
      }));
      actions.ForEach((Action<Action>) (a => a()));
    }

    private void NotifyGroupInfoChange(
      Conversation convo,
      FunEventHandler.GroupInfoDirtyFlags flags)
    {
      if ((flags & (FunEventHandler.GroupInfoDirtyFlags.GroupMembers | FunEventHandler.GroupInfoDirtyFlags.GroupAdmins)) == FunEventHandler.GroupInfoDirtyFlags.None)
        return;
      FunEventHandler.Events.GroupMembershipUpdatedSubject.OnNext(new FunEventHandler.Events.ConversationWithFlags()
      {
        Conversation = convo,
        MembersChanged = (flags & FunEventHandler.GroupInfoDirtyFlags.GroupMembers) != 0,
        AdminsChanged = (flags & FunEventHandler.GroupInfoDirtyFlags.GroupAdmins) != 0
      });
    }

    private void ComputeDeltas<T>(
      IEnumerable<T> old,
      IEnumerable<T> @new,
      Action<T> onAdded,
      Action<T> onRemoved)
    {
      Dictionary<T, bool> dictionary1 = new Dictionary<T, bool>();
      Dictionary<T, bool> dictionary2 = new Dictionary<T, bool>();
      foreach (T key in old)
        dictionary1[key] = true;
      foreach (T key in @new)
        dictionary2[key] = true;
      foreach (T key in @new)
      {
        if (!dictionary1.ContainsKey(key))
          onAdded(key);
      }
      foreach (T key in old)
      {
        if (!dictionary2.ContainsKey(key))
          onRemoved(key);
      }
    }

    private FunEventHandler.GroupInfoDirtyFlags OnGroupInfo(
      MessagesContext db,
      Conversation convo,
      FunXMPP.Connection.GroupInfo info,
      Action<Action> submitPostSubmitJob = null,
      DateTime? dt = null,
      bool checkParticipants = true)
    {
      FunEventHandler.GroupInfoDirtyFlags groupInfoDirtyFlags = FunEventHandler.GroupInfoDirtyFlags.None;
      int num = this.CheckEqual<string>(info.CreatorJid, (Func<string, bool>) (v => convo.GroupOwner != v), (Action<string>) (v => convo.GroupOwner = v)) + this.CheckEqual<DateTime?>(info.CreationTime, (Func<DateTime?, bool>) (v =>
      {
        DateTime? groupCreationT = convo.GroupCreationT;
        DateTime? nullable = v;
        if (groupCreationT.HasValue != nullable.HasValue)
          return true;
        return groupCreationT.HasValue && groupCreationT.GetValueOrDefault() != nullable.GetValueOrDefault();
      }), (Action<DateTime?>) (v => convo.GroupCreationT = v)) + this.CheckEqual<string>(info.Subject, (Func<string, bool>) (v => convo.GroupSubject != v), (Action<string>) (v =>
      {
        convo.GroupSubject = v;
        convo.GroupSubjectPerformanceHint = LinkDetector.Result.Serialize(LinkDetector.GetMatches(Emoji.ConvertToUnicode(v)));
      })) + this.CheckEqual<string>(info.SubjectOwnerJid, (Func<string, bool>) (v => convo.GroupSubjectOwner != v), (Action<string>) (v => convo.GroupSubjectOwner = v)) + this.CheckEqual<DateTime?>(info.SubjectTime, (Func<DateTime?, bool>) (v =>
      {
        DateTime? groupSubjectT = convo.GroupSubjectT;
        DateTime? nullable = v;
        if (groupSubjectT.HasValue != nullable.HasValue)
          return true;
        return groupSubjectT.HasValue && groupSubjectT.GetValueOrDefault() != nullable.GetValueOrDefault();
      }), (Action<DateTime?>) (v => convo.GroupSubjectT = v));
      if (info.Locked != convo.IsLocked())
      {
        if (info.Locked)
          convo.Lock();
        else
          convo.Unlock();
        ++num;
      }
      if (info.AnnouncementOnly != convo.IsAnnounceOnly())
      {
        if (info.AnnouncementOnly)
          convo.MakeAnnounceOnly();
        else
          convo.MakeNotAnnounceOnly();
        ++num;
      }
      if (info.Description != null)
        num += this.CheckEqual<string>(info.Description.Id, (Func<string, bool>) (v => convo.GroupDescriptionId != v), (Action<string>) (v => convo.GroupDescriptionId = v)) + this.CheckEqual<DateTime?>(info.Description.CreateTime, (Func<DateTime?, bool>) (v =>
        {
          DateTime? groupDescriptionT = convo.GroupDescriptionT;
          DateTime? nullable = v;
          if (groupDescriptionT.HasValue != nullable.HasValue)
            return true;
          return groupDescriptionT.HasValue && groupDescriptionT.GetValueOrDefault() != nullable.GetValueOrDefault();
        }), (Action<DateTime?>) (v => convo.GroupDescriptionT = v)) + this.CheckEqual<string>(info.Description.Owner, (Func<string, bool>) (v => convo.GroupDescriptionOwner != v), (Action<string>) (v => convo.GroupDescriptionOwner = v)) + this.CheckEqual<string>(info.Description.Body, (Func<string, bool>) (v => convo.GroupDescription != v), (Action<string>) (v => convo.GroupDescription = v));
      if (num != 0)
        groupInfoDirtyFlags |= FunEventHandler.GroupInfoDirtyFlags.ConversationMetadata;
      if (checkParticipants)
      {
        IEnumerable<string> @new = info.AdminJids.Concat<string>((IEnumerable<string>) info.NonadminJids);
        Set<string> admins = new Set<string>((IEnumerable<string>) info.AdminJids);
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        List<Message> source = new List<Message>();
        string[] participantJids = convo.GetParticipantJids();
        bool flag1 = false;
        foreach (string key in @new)
          dictionary[key] = true;
        if (!convo.LastMessageID.HasValue && participantJids.Length == 0 && dictionary.ContainsKey(Settings.MyJid))
        {
          source.AddRange(this.CreateWelcomeNotifications(db, convo, info, dt));
          flag1 = true;
        }
        Dictionary<string, bool> usersAdded = new Dictionary<string, bool>();
        Set<string> usersRemoved = new Set<string>();
        bool hadAdminDelta = false;
        this.ComputeDeltas<string>((IEnumerable<string>) participantJids, @new, (Action<string>) (uJid =>
        {
          usersAdded[uJid] = admins.Contains(uJid);
          if (!admins.Contains(uJid))
            return;
          hadAdminDelta = true;
        }), (Action<string>) (uJid =>
        {
          usersRemoved.Add(uJid);
          if (!admins.Contains(uJid))
            return;
          hadAdminDelta = true;
        }));
        List<string> adminsPromoted = new List<string>();
        List<string> adminsDemoted = new List<string>();
        convo.ParticipantSetAction((Action<GroupParticipants>) (set => this.ComputeDeltas<string>(set.Admins, (IEnumerable<string>) info.AdminJids, (Action<string>) (uJid =>
        {
          if (usersAdded.ContainsKey(uJid))
            return;
          adminsPromoted.Add(uJid);
        }), (Action<string>) (uJid =>
        {
          if (usersRemoved.Contains(uJid))
            return;
          adminsDemoted.Add(uJid);
        }))));
        bool flag2 = usersAdded.Any<KeyValuePair<string, bool>>() || usersRemoved.Any<string>();
        if (adminsPromoted.Any<string>() || adminsDemoted.Any<string>())
          hadAdminDelta = true;
        if (flag2 | hadAdminDelta)
        {
          if (flag2 && !flag1 && !usersAdded.ContainsKey(Settings.MyJid))
          {
            foreach (string key in usersAdded.Keys)
              source.Add(this.GenerateParticipantChange(db, convo.Jid, key, (string) null, SystemMessageUtils.ParticipantChange.Join, dt));
            foreach (string jid in usersRemoved)
              source.Add(this.GenerateParticipantChange(db, convo.Jid, jid, (string) null, SystemMessageUtils.ParticipantChange.Leave, dt));
          }
          convo.UpdateParticipants((SqliteMessagesContext) db, usersAdded, (IEnumerable<string>) usersRemoved, (IEnumerable<string>) adminsPromoted, (IEnumerable<string>) adminsDemoted);
          source.AddRange(this.GenerateAdminChanges<string>(db, convo.Jid, adminsPromoted.ToDictionary<string, string>((Func<string, string>) (v => v)), adminsDemoted.ToDictionary<string, string>((Func<string, string>) (v => v)), dt));
        }
        if (source.Any<Message>())
        {
          groupInfoDirtyFlags |= FunEventHandler.GroupInfoDirtyFlags.SystemMessages;
          source.ForEach((Action<Message>) (m => db.InsertMessageOnSubmit(m)));
        }
        if (flag2)
          groupInfoDirtyFlags |= FunEventHandler.GroupInfoDirtyFlags.GroupMembers;
        if (hadAdminDelta)
          groupInfoDirtyFlags |= FunEventHandler.GroupInfoDirtyFlags.GroupAdmins;
      }
      return groupInfoDirtyFlags;
    }

    private int CheckEqual<T>(T src, Func<T, bool> different, Action<T> set)
    {
      if (!different(src))
        return 0;
      set(src);
      return 1;
    }

    private IEnumerable<Message> CreateWelcomeNotifications(
      MessagesContext db,
      Conversation convo,
      FunXMPP.Connection.GroupInfo info,
      DateTime? dt,
      string inviterJid = null,
      bool isWelcome = false,
      bool isInvite = false,
      bool isDueToNumberChange = false)
    {
      if (!((int?) convo?.LastMessageID).HasValue)
        yield return SystemMessageUtils.CreateGroupCreated(db, info.Jid, info.CreatorJid, isWelcome ? info.Subject : "", info.CreationTime);
      string myJid = Settings.MyJid;
      if (!isDueToNumberChange && inviterJid != myJid && (isWelcome || info.CreatorJid != myJid))
      {
        Message participantChanged = SystemMessageUtils.CreateParticipantChanged(db, isInvite ? SystemMessageUtils.ParticipantChange.Invite : SystemMessageUtils.ParticipantChange.Join, info.Jid, Settings.MyJid, inviterJid, dt);
        participantChanged.ForceNotNoteworthy = !isWelcome;
        yield return participantChanged;
      }
    }

    public void OnGroupWelcome(FunXMPP.Connection.GroupCreationEventArgs args, bool isInvite)
    {
      ChatPictureStore.Delete(args.Info.Jid);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Message> messageList = new List<Message>();
        Conversation conversation = db.GetConversation(args.Info.Jid, CreateOptions.CreateToDbIfNotFound);
        conversation.SetConversationNotSeen();
        int num = (int) this.OnGroupInfo(db, conversation, args.Info, dt: args.Timestamp, checkParticipants: false);
        conversation.ParticipantSetAction((Action<GroupParticipants>) (participants =>
        {
          participants.Clear();
          foreach (string adminJid in args.Info.AdminJids)
            participants.Add(adminJid, true);
          foreach (string nonadminJid in args.Info.NonadminJids)
            participants.Add(nonadminJid, false);
          if (!string.IsNullOrEmpty(args.Info.SuperAdmin))
            participants.AddSuperAdmin(args.Info.SuperAdmin);
          participants.Flush();
        }));
        conversation.UpdateParticipantsHash();
        bool isDueToNumberChange = args.InviterJid != null && !((IEnumerable<string>) conversation.GetParticipantJids()).Contains<string>(args.InviterJid);
        messageList.AddRange(this.CreateWelcomeNotifications(db, conversation, args.Info, args.Timestamp, args.InviterJid, true, isInvite, isDueToNumberChange));
        messageList.ForEach(new Action<Message>(((SqliteMessagesContext) db).InsertMessageOnSubmit));
        db.SubmitChanges();
        this.NotifyGroupInfoChange(conversation, FunEventHandler.GroupInfoDirtyFlags.GroupMembers | FunEventHandler.GroupInfoDirtyFlags.GroupAdmins);
        PresenceState.Instance.ResetForUser(args.Info.Jid);
      }));
    }

    public void OnAddGroupParticipants(
      string gjid,
      List<string> successJids,
      List<Pair<string, int>> failList,
      DateTime? dt,
      string reason)
    {
      string me = Settings.MyJid;
      successJids.ForEach((Action<string>) (jid => this.OnGroupAddUser(gjid, jid, me, dt, reason)));
    }

    public void OnRemoveGroupParticipants(
      string gjid,
      List<string> successList,
      List<Pair<string, int>> failList,
      DateTime? dt)
    {
      string me = Settings.MyJid;
      successList.ForEach((Action<string>) (jid => this.OnGroupRemoveUser(gjid, jid, me, dt)));
    }

    public void OnParticipatingGroups(FunXMPP.Connection.GroupInfo[] groups)
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      HashSet<string> stringSet1 = new HashSet<string>((IEnumerable<string>) MessagesContext.Select<List<Conversation>>((Func<MessagesContext, List<Conversation>>) (db => db.GetGroups(true))).Select<Conversation, string>((Func<Conversation, string>) (gi => gi.Jid)).ToArray<string>());
      HashSet<string> stringSet2 = new HashSet<string>(((IEnumerable<FunXMPP.Connection.GroupInfo>) groups).Select<FunXMPP.Connection.GroupInfo, string>((Func<FunXMPP.Connection.GroupInfo, string>) (gi => gi.Jid)));
      foreach (FunXMPP.Connection.GroupInfo group in groups)
        this.OnGroupInfo(group, true);
      HashSet<string> stringSet3 = new HashSet<string>((IEnumerable<string>) stringSet2);
      stringSet3.ExceptWith((IEnumerable<string>) stringSet1);
      HashSet<string> stringSet4 = new HashSet<string>((IEnumerable<string>) stringSet1);
      stringSet4.ExceptWith((IEnumerable<string>) stringSet2);
      string myJid = Settings.MyJid;
      foreach (string gjid in stringSet4)
        this.OnGroupRemoveUserImpl(gjid, myJid, (string) null, new DateTime?(), sysMsg: false);
      int refCount = 0;
      Action action = (Action) (() =>
      {
        if (Interlocked.Decrement(ref refCount) > 0)
          return;
        this.Connection.SendClearDirty(nameof (groups));
        Settings.LastGroupsUpdatedUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
        Log.l("funhandler", "clear groups dirty");
      });
      Log.d("funhandler", string.Format("{0} new groups", (object) stringSet3.Count));
      action();
      PerformanceTimer.End("fun handler: syncing groups", start);
    }

    public void OnLeaveGroup(string groupJid, DateTime? dt, bool delete = false)
    {
      Conversation convo = (Conversation) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => convo = db.GetConversation(groupJid, CreateOptions.None)));
      if (convo == null || !convo.ContainsParticipant(Settings.MyJid))
        return;
      string myJid = Settings.MyJid;
      this.OnGroupRemoveUser(groupJid, myJid, myJid, dt, sysMsg: !delete);
      PresenceState.Instance.ResetForUser(groupJid, false);
    }

    public void OnLeaveGroupFail(string groupJid)
    {
      Log.l("funhandler", "leave group fail | jid={0}", (object) groupJid);
    }

    public void OnGroupDescriptionMismatch(string groupJid)
    {
      Log.l("funhandler", "group description id mismatch | jid={0}", (object) groupJid);
      this.Connection.SendGetGroupDescription(groupJid);
    }

    public void CombineAsync(
      IEnumerable<FunEventHandler.AsyncOpWithCompletion> ops,
      Action onComplete = null)
    {
      Func<Action> func = (Func<Action>) (() => (Action) null);
      if (onComplete != null)
      {
        RefCountAction completeCallback = new RefCountAction((Action) (() => { }), onComplete);
        func = (Func<Action>) (() => new Action(completeCallback.Subscribe().Dispose));
      }
      Action action = func();
      foreach (FunEventHandler.AsyncOpWithCompletion op in ops)
        op(func());
      if (action == null)
        return;
      action();
    }

    public void OnRemoteClientCaps(IEnumerable<RemoteClientCaps> data, Action onComplete = null)
    {
      RemoteClientCaps[] array = data.ToArray<RemoteClientCaps>();
      data = (IEnumerable<RemoteClientCaps>) array;
      int val1 = 50;
      List<RemoteClientCaps[]> source = new List<RemoteClientCaps[]>();
      if (Settings.LastFullSyncUtc.HasValue && array.Length > val1)
      {
        for (int index1 = 0; index1 < array.Length; index1 += val1)
        {
          int length = Math.Min(val1, array.Length - index1);
          RemoteClientCaps[] remoteClientCapsArray = new RemoteClientCaps[length];
          for (int index2 = 0; index2 < length; ++index2)
            remoteClientCapsArray[index2] = array[index1 + index2];
          source.Add(remoteClientCapsArray);
        }
      }
      else
        source.Add(array);
      ObservableQueue queue = new ObservableQueue();
      this.CombineAsync(source.Select<RemoteClientCaps[], FunEventHandler.AsyncOpWithCompletion>((Func<RemoteClientCaps[], FunEventHandler.AsyncOpWithCompletion>) (batch => (FunEventHandler.AsyncOpWithCompletion) (complete => Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        AppState.Worker.Enqueue((Action) (() =>
        {
          try
          {
            this.ProcessRemoteClientCaps((IEnumerable<RemoteClientCaps>) batch);
            Action action = complete;
            if (action == null)
              return;
            action();
          }
          finally
          {
            observer.OnCompleted();
          }
        }));
        return (Action) (() => { });
      })).ObserveInQueue<Unit>(queue).Subscribe<Unit>()))), onComplete);
    }

    private void ProcessRemoteClientCaps(IEnumerable<RemoteClientCaps> data)
    {
      int count = 0;
      ContactsContext.Instance((Action<ContactsContext>) (cdb =>
      {
        foreach (RemoteClientCaps remoteClientCaps in data)
        {
          string jid = remoteClientCaps.Jid;
          Func<ClientCapabilityCategory, ClientCapability> func = (Func<ClientCapabilityCategory, ClientCapability>) (type => cdb.GetClientCapabilities(jid, type, true));
          if (remoteClientCaps.Values.Count > 3)
          {
            Dictionary<ClientCapabilityCategory, ClientCapability> allCaps = ((IEnumerable<ClientCapability>) cdb.GetClientCapabilities(jid)).ToDictionary<ClientCapability, ClientCapabilityCategory>((Func<ClientCapability, ClientCapabilityCategory>) (r => r.Category));
            func = (Func<ClientCapabilityCategory, ClientCapability>) (type =>
            {
              ClientCapability o = (ClientCapability) null;
              if (!allCaps.TryGetValue(type, out o) || o == null)
              {
                o = new ClientCapability()
                {
                  Jid = jid,
                  Category = type
                };
                cdb.Insert("ClientCapabilities", (object) o);
              }
              return o;
            });
          }
          foreach (Triad<ClientCapabilityCategory, ClientCapabilitySetting, DateTime?> triad in remoteClientCaps.Values)
          {
            ClientCapabilityCategory first = triad.First;
            ClientCapabilitySetting second = triad.Second;
            DateTime dateTime = triad.Third ?? DateTime.UtcNow;
            ClientCapability clientCapability = func(first);
            if (clientCapability.Value != second)
            {
              clientCapability.Value = second;
              DateTime? lastUpdate = clientCapability.LastUpdate;
              clientCapability.LastUpdate = new DateTime?(dateTime);
            }
            else if (!clientCapability.LastUpdate.HasValue)
              clientCapability.LastUpdate = new DateTime?(dateTime);
          }
          ++count;
        }
        cdb.SubmitChanges();
      }));
      Log.p("funhandler", "client caps | saved client capabilities for {0} users", (object) count);
    }

    public bool OnGdprReportReady(
      long? creationTime,
      long? expirationTime,
      byte[] protobuf,
      bool showToast,
      Action ack)
    {
      bool flag1 = false;
      DateTime? nullable1 = new DateTime?();
      DateTime? nullable2 = new DateTime?();
      DateTime valueOrDefault;
      if (creationTime.HasValue)
      {
        nullable1 = new DateTime?(DateTimeUtils.FromUnixTime(creationTime.Value));
        object[] objArray = new object[1];
        string str;
        if (!nullable1.HasValue)
        {
          str = (string) null;
        }
        else
        {
          valueOrDefault = nullable1.GetValueOrDefault();
          str = valueOrDefault.ToLongDateString();
        }
        if (str == null)
          str = "n/a";
        objArray[0] = (object) str;
        Log.l("gdpr", "report creation date:{0} utc", objArray);
      }
      if (expirationTime.HasValue)
      {
        nullable2 = new DateTime?(DateTimeUtils.FromUnixTime(expirationTime.Value));
        object[] objArray = new object[1];
        string str;
        if (!nullable2.HasValue)
        {
          str = (string) null;
        }
        else
        {
          valueOrDefault = nullable2.GetValueOrDefault();
          str = valueOrDefault.ToLongDateString();
        }
        if (str == null)
          str = "n/a";
        objArray[0] = (object) str;
        Log.l("gdpr", "report expiration date:{0} utc", objArray);
      }
      bool flag2 = GdprReport.CreateMessageFromReportInfo(protobuf) != null;
      if (((!nullable1.HasValue ? 0 : (nullable2.HasValue ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
      {
        bool flag3 = false;
        GdprReport.States gdprReportState = Settings.GdprReportState;
        switch (gdprReportState)
        {
          case GdprReport.States.Ready:
          case GdprReport.States.Downloading:
          case GdprReport.States.Downloaded:
            DateTime? reportCreationTimeUtc = Settings.GdprReportCreationTimeUtc;
            if (reportCreationTimeUtc.HasValue && reportCreationTimeUtc.Value >= nullable1.Value)
            {
              flag3 = true;
              Log.l("gdpr", "skip gdpr report ready state update | curr state:{0},curr report created on:{1}", (object) gdprReportState, (object) reportCreationTimeUtc.Value);
              break;
            }
            break;
        }
        if (!flag3)
          GdprReport.SetStateReady(nullable1.Value, nullable2.Value, protobuf);
        flag1 = true;
      }
      if (ack != null)
        ack();
      if (showToast & flag1)
      {
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddString("clr2", "ContactsPage");
        string pageUriStr = UriUtils.CreatePageUriStr("GdprReportPage", uriParams, "Pages/Settings");
        AppState.ClientInstance.ShowToast(AppResources.GdprReportNotification, pageUriStr);
      }
      return flag1;
    }

    public static class Events
    {
      public static Subject<FunEventHandler.Events.ConversationWithFlags> GroupMembershipUpdatedSubject = new Subject<FunEventHandler.Events.ConversationWithFlags>();

      public class ConversationWithFlags
      {
        public Conversation Conversation;
        public bool MembersChanged;
        public bool AdminsChanged;
      }
    }

    public class InvalidDuplicateMessageException : Exception
    {
    }

    public class ServerPropHandler
    {
      public string Key;
      public Action<string> Action;
      public Settings.Key? AssociatedSettingsKey;
      public Action PropNotSuppliedAction;

      public ServerPropHandler(
        string key,
        Action<string> handler,
        Settings.Key? associatedSettingsKey,
        Action propNotSuppliedHandler = null)
      {
        this.Key = key;
        this.Action = handler;
        this.AssociatedSettingsKey = associatedSettingsKey;
        this.PropNotSuppliedAction = propNotSuppliedHandler;
      }
    }

    private enum GroupInfoDirtyFlags
    {
      None = 0,
      ConversationMetadata = 1,
      GroupMembers = 2,
      GroupAdmins = 4,
      SystemMessages = 8,
    }

    public delegate void AsyncOpWithCompletion(Action onComplete = null);
  }
}
