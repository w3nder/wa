// Decompiled with JetBrains decompiler
// Type: WhatsApp.QrListener
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Info;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WhatsApp.CommonOps;
using WhatsApp.WaCollections;
using Windows.Phone.Devices.Power;


namespace WhatsApp
{
  public class QrListener : FunXMPP.QrListener
  {
    public const string LogHeader = "web";
    private const int ResumeMessageLimit = 20;
    private const int PreemptyMessageLimit = 50;
    private QrSession session = new QrSession();
    private IDisposable newMessageSub_;
    private IDisposable updateMessageWaTypeSub;
    private IDisposable deletedMessagesSub_;
    private IDisposable userUpdatedSub_;
    private IDisposable batteryUpdatedSub_;
    private IDisposable powerConnectedSub;
    private IDisposable changeNumberActionSub;
    private Set<string> messagesPendingWebRelay;

    public static QrListener Instance => (QrListener) AppState.GetConnection().EventHandler.Qr;

    public QrSession Session => this.session;

    public bool Active => this.Session.Active;

    private FunXMPP.Connection Connection => AppState.GetConnection();

    public QrListener()
    {
      this.session.Disconnected += new EventHandler(this.OnSessionDisconnected);
    }

    public void OnSessionConnected()
    {
      if (this.newMessageSub_ == null)
        this.newMessageSub_ = MessagesContext.Events.NewMessagesSubject.Subscribe<Message>(new Action<Message>(this.OnNewMessage));
      if (this.updateMessageWaTypeSub == null)
        this.updateMessageWaTypeSub = MessagesContext.Events.UpdatedMessagesMediaWaTypeSubject.Subscribe<Message>(new Action<Message>(this.OnMessageWaMediaTypeUpdate));
      if (this.deletedMessagesSub_ == null)
        this.deletedMessagesSub_ = MessagesContext.Events.DeletedMessagesSubject.ObserveOn<Message>((IScheduler) AppState.Worker).Subscribe<Message>(new Action<Message>(this.OnDbDeletedMessage));
      if (this.userUpdatedSub_ == null)
        this.userUpdatedSub_ = ContactsContext.Events.UserStatusUpdatedSubject.ObserveOn<DbDataUpdate>((IScheduler) AppState.Worker).Subscribe<DbDataUpdate>((Action<DbDataUpdate>) (u => this.OnUserStatusUpdated(u.UpdatedObj as UserStatus, u.UpdateType)));
      if (this.batteryUpdatedSub_ == null)
        this.batteryUpdatedSub_ = Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          EventHandler<object> handler = (EventHandler<object>) ((s, e) => observer.OnNext(new Unit()));
          Battery battery = Battery.GetDefault();
          WindowsRuntimeMarshal.AddEventHandler<EventHandler<object>>(new Func<EventHandler<object>, EventRegistrationToken>(battery.add_RemainingChargePercentChanged), new Action<EventRegistrationToken>(battery.remove_RemainingChargePercentChanged), handler);
          return (Action) (() => WindowsRuntimeMarshal.RemoveEventHandler<EventHandler<object>>(new Action<EventRegistrationToken>(Battery.GetDefault().remove_RemainingChargePercentChanged), handler));
        })).ObserveOn<Unit>((IScheduler) AppState.Worker).Subscribe<Unit>((Action<Unit>) (_ => this.OnBatteryUpdated()));
      if (this.powerConnectedSub == null)
        this.powerConnectedSub = Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          EventHandler handler = (EventHandler) ((s, e) => observer.OnNext(new Unit()));
          DeviceStatus.PowerSourceChanged += handler;
          return (Action) (() => DeviceStatus.PowerSourceChanged -= handler);
        })).ObserveOn<Unit>((IScheduler) AppState.Worker).Subscribe<Unit>((Action<Unit>) (_ => this.OnBatteryUpdated()));
      if (this.changeNumberActionSub != null)
        return;
      this.changeNumberActionSub = ContactsContext.Events.ChangeNumberActionSubject.ObserveOn<SqliteContactsContext.ChangeNumberAction>((IScheduler) AppState.Worker).Subscribe<SqliteContactsContext.ChangeNumberAction>(new Action<SqliteContactsContext.ChangeNumberAction>(this.OnChangeNumberAction));
    }

    public void OnSessionDisconnected(object sender, EventArgs e)
    {
      this.newMessageSub_.SafeDispose();
      this.newMessageSub_ = (IDisposable) null;
      this.updateMessageWaTypeSub.SafeDispose();
      this.updateMessageWaTypeSub = (IDisposable) null;
      this.deletedMessagesSub_.SafeDispose();
      this.deletedMessagesSub_ = (IDisposable) null;
      this.userUpdatedSub_.SafeDispose();
      this.userUpdatedSub_ = (IDisposable) null;
      this.batteryUpdatedSub_.SafeDispose();
      this.batteryUpdatedSub_ = (IDisposable) null;
      this.powerConnectedSub.SafeDispose();
      this.powerConnectedSub = (IDisposable) null;
    }

    private void OnNewMessage(Message m)
    {
      if (m.Status == FunXMPP.FMessage.Status.Relay)
        return;
      Log.l("web", "new msg | {0}", (object) m.LogInfo());
      try
      {
        AppState.QrPersistentAction.NotifyMessage(m, QrMessageForwardType.Relay);
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "notify new message");
      }
    }

    private void OnMessageWaMediaTypeUpdate(Message m)
    {
      if (m.Status == FunXMPP.FMessage.Status.Relay)
        return;
      Log.l("web", "updated media wa type for msg | {0}", (object) m.LogInfo());
      try
      {
        if (m.MediaWaType != FunXMPP.FMessage.Type.Revoked)
          return;
        AppState.QrPersistentAction.NotifyRevoke(m);
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "notify updated media wa type for message");
      }
    }

    private void OnChangeNumberAction(SqliteContactsContext.ChangeNumberAction action)
    {
      if (action.ActionType == SqliteContactsContext.ChangeNumberAction.Type.Removed)
      {
        AppState.GetConnection()?.SendQrChangeNumberNotificationDismiss(action.OldJid);
        AppState.GetConnection()?.SendQrChangeNumberNotificationDismiss(action.NewJid);
      }
      else
      {
        if (action.ActionType != SqliteContactsContext.ChangeNumberAction.Type.Added)
          return;
        AppState.GetConnection()?.SendQrChangeNumberNotificationForward(action.OldJid, (string) null, action.NewJid);
        AppState.GetConnection()?.SendQrChangeNumberNotificationForward(action.NewJid, (string) null, action.OldJid);
      }
    }

    private void OnDbDeletedMessage(Message m)
    {
      bool notify = true;
      int newModTag = 0;
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(m.KeyRemoteJid, CreateOptions.None);
        int? nullable1 = new int?();
        int? nullable2;
        if (conversation == null || (nullable2 = conversation.MessageLoadingStart()).HasValue && m.MessageID < nullable2.Value)
          notify = false;
        else
          newModTag = (int) conversation.ModifyTag;
      }));
      if (notify)
      {
        AppState.QrPersistentAction.NotifyDelete(m, newModTag);
        Log.l("web", "on db msg deleted | jid:{0},keyid:{1},modify tag:{2}", (object) m.KeyRemoteJid, (object) m.KeyId, (object) newModTag);
      }
      else
        Log.d("web", "on db msg deleted | skip notify | jid:{0},keyid:{1}", (object) m.KeyRemoteJid, (object) m.KeyId);
    }

    private void OnUserStatusUpdated(UserStatus user, DbDataUpdate.Types updateType)
    {
      if (user == null)
        return;
      FunXMPP.ContactResponse c = new FunXMPP.ContactResponse()
      {
        Jid = user.Jid,
        DisplayName = updateType == DbDataUpdate.Types.Deleted ? (string) null : user.GetDisplayName(getNumberIfNoName: false),
        ShortName = updateType == DbDataUpdate.Types.Deleted ? (string) null : user.FirstName,
        Checksum = Settings.ContactsChecksum,
        Notify = user.PushName,
        VName = user.GetVerifiedNameForDisplay(),
        Verify = user.VerificationLevelForWeb(),
        IsEnterprise = new bool?(user.IsEnterprise()),
        Checkmark = new bool?(user.ShowVerifiedBadgeForWeb())
      };
      Log.l("web", "OnUserStatusUpdated - Jid: {0}", (object) c.Jid);
      AppState.QrPersistentAction.NotifyContactChange(c);
    }

    public void OnBatteryUpdated()
    {
      int batteryPercentage = AppState.BatteryPercentage;
      bool powerSourceConnected = AppState.PowerSourceConnected;
      bool batterySaverEnabled = AppState.BatterySaverEnabled;
      Log.l("web", "battery updated | {0}% | power connected:{1} | battery saver enabled: {2}", (object) batteryPercentage, (object) powerSourceConnected, (object) batterySaverEnabled);
      AppState.GetConnection()?.SendQrBattery(batteryPercentage, powerSourceConnected, batterySaverEnabled);
    }

    public FunXMPP.SyncResponse GetSync()
    {
      FunXMPP.SyncResponse sync = new FunXMPP.SyncResponse();
      sync.BatteryPercentage = AppState.BatteryPercentage;
      sync.PowerSourceConnected = AppState.PowerSourceConnected;
      sync.BatterySaverEnabled = AppState.BatterySaverEnabled;
      AppState.GetLangAndLocale(out sync.Language, out sync.Locale);
      sync.IsMilitaryTime = AppState.IsMilitaryTimeDisplayed();
      Log.l("web", "GetSync | battery:{0}%,power connected: {1},batterysaver:{2},lang:{3},locale:{4},mil-time:{5}", (object) sync.BatteryPercentage, (object) sync.PowerSourceConnected, (object) sync.BatterySaverEnabled, (object) sync.Language, (object) sync.Locale, (object) sync.IsMilitaryTime);
      return sync;
    }

    private Message GetMessageWithKeyFromWeb(MessagesContext db, FunXMPP.FMessage.Key webMsgKey)
    {
      if (webMsgKey.from_me)
      {
        Message message = db.GetMessage(webMsgKey.remote_jid, webMsgKey.id, false);
        if (message != null && message.MediaWaType == FunXMPP.FMessage.Type.System)
          return message;
      }
      return db.GetMessage(webMsgKey.remote_jid, webMsgKey.id, webMsgKey.from_me);
    }

    public Message[] GetMessagesAfter(
      FunXMPP.FMessage.Key webMsgKey,
      bool asc,
      int? limit,
      bool includeBound,
      bool mediaOnly = false,
      FunXMPP.FMessage.Type? mediaType = null)
    {
      Log.l("web", "get msgs after | jid: {0}, key id: {1}, asc: {2}, limit: {3}, includeBound: {4}, mediaOnly: {5}, mediaType: {6}", (object) webMsgKey.remote_jid, (object) webMsgKey.id, (object) asc, (object) (limit ?? -1), (object) includeBound, (object) mediaOnly, (object) (FunXMPP.FMessage.Type) ((int) mediaType ?? 0));
      Message[] msgs = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message messageWithKeyFromWeb = this.GetMessageWithKeyFromWeb(db, webMsgKey);
        if (messageWithKeyFromWeb == null)
        {
          Log.l("web", "get msgs after | ref msg not found");
        }
        else
        {
          msgs = db.GetMessagesAfter(messageWithKeyFromWeb.KeyRemoteJid, new int?(), new int?(messageWithKeyFromWeb.MessageID), includeBound, limit, new int?(), asc: asc, mediaOnly: mediaOnly, mediaType: mediaType);
          Log.d("web", "get msgs after | {0} msgs fetched", (object) msgs.Length);
        }
      }));
      if (msgs == null)
        msgs = new Message[0];
      return !asc ? ((IEnumerable<Message>) msgs).Reverse<Message>().ToArray<Message>() : msgs;
    }

    public Message[] GetMessagesBefore(
      string jid,
      bool fromMe,
      string keyId,
      int? count,
      bool asc,
      bool mediaOnly = false,
      FunXMPP.FMessage.Type? mediaType = null)
    {
      Log.l("web", "get msgs before | jid: {0}, key id: {1}, count: {2}, asc: {3}, mediaOnly: {4}, mediaType: {5}", (object) jid, (object) keyId, (object) (count ?? -1), (object) asc, (object) mediaOnly, (object) (FunXMPP.FMessage.Type) ((int) mediaType ?? 0));
      Message[] msgs = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = true;
        int? upperBoundMsgId = new int?();
        if (keyId != null)
        {
          Message messageWithKeyFromWeb = this.GetMessageWithKeyFromWeb(db, new FunXMPP.FMessage.Key(jid, fromMe, keyId));
          if (messageWithKeyFromWeb == null)
          {
            Log.l("web", "get msgs before | ref msg not found");
            flag = false;
          }
          else
            upperBoundMsgId = new int?(messageWithKeyFromWeb.MessageID);
        }
        if (!flag)
          return;
        msgs = db.GetMessagesBefore(jid, new int?(), upperBoundMsgId, count, new int?(), asc: asc, mediaOnly: mediaOnly, mediaType: mediaType);
        Log.d("web", "get msgs before | {0}/{1} msgs loaded", (object) msgs.Length, (object) (count ?? -1));
      }));
      return msgs ?? new Message[0];
    }

    public Message[] GetStarredMessagesBefore(
      string chat,
      string jid,
      bool fromMe,
      string keyId,
      int? count,
      bool asc,
      FunXMPP.FMessage.Type[] types)
    {
      Log.l("web", "get starred msgs before | chat: {0}, jid: {1}, key id: {2}, count: {3}, asc: {4}", (object) chat, (object) jid, (object) keyId, (object) (count ?? -1), (object) asc);
      Message[] msgs = (Message[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool flag = true;
        int? upperBoundMsgId = new int?();
        if (keyId != null)
        {
          Message messageWithKeyFromWeb = this.GetMessageWithKeyFromWeb(db, new FunXMPP.FMessage.Key(jid, fromMe, keyId));
          if (messageWithKeyFromWeb == null)
          {
            Log.l("web", "get starred msgs before | ref msg not found");
            flag = false;
          }
          else
            upperBoundMsgId = new int?(messageWithKeyFromWeb.MessageID);
        }
        if (!flag)
          return;
        msgs = db.GetStarredMessagesBefore(chat, new int?(), upperBoundMsgId, count, new int?(), asc: asc, types: types);
        Log.d("web", "get starred msgs before | {0}/{1} msgs loaded", (object) msgs.Length, (object) (count ?? -1));
      }));
      return msgs ?? new Message[0];
    }

    public FunXMPP.ChatResponse[] GetChats(bool getPreemptMessages, string jid)
    {
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      FunXMPP.ChatResponse[] results = (FunXMPP.ChatResponse[]) null;
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        List<ConversationItem> convos = (List<ConversationItem>) null;
        if (jid == null)
        {
          convos = db.GetConversationItems(new JidHelper.JidTypes[4]
          {
            JidHelper.JidTypes.User,
            JidHelper.JidTypes.Group,
            JidHelper.JidTypes.Broadcast,
            JidHelper.JidTypes.Psa
          }, true);
          convos.Sort(new Comparison<ConversationItem>(ConversationItem.CompareBySortKeyAndTimestamp));
        }
        else
          convos = new List<ConversationItem>()
          {
            db.GetConversationItem(jid)
          };
        Log.l("web", "get chats | count: {0}", (object) convos.Count);
        DateTime utcNow = DateTime.UtcNow;
        DateTime funNow = FunRunner.CurrentServerTimeUtc;
        Dictionary<string, Pair<string, string>> changeNumberInfoDictionary = new Dictionary<string, Pair<string, string>>();
        ContactsContext.Instance((Action<ContactsContext>) (cdb =>
        {
          foreach (ConversationItem conversationItem in convos)
            changeNumberInfoDictionary.Add(conversationItem.Jid, cdb.GetChangeNumberJidsForPanel(conversationItem.Jid));
        }));
        results = convos.Select<ConversationItem, FunXMPP.ChatResponse>((Func<ConversationItem, FunXMPP.ChatResponse>) (ci =>
        {
          Conversation conversation = ci.Conversation;
          FunXMPP.FMessage.Key key = (FunXMPP.FMessage.Key) null;
          List<Message> messageList;
          if (getPreemptMessages && conversation.Timestamp.HasValue && conversation.Timestamp.Value > utcNow.Subtract(TimeSpan.FromHours(24.0)))
          {
            FunXMPP.MessagesResponse latestMessagesForChat = this.GetLatestMessagesForChat(conversation, db);
            messageList = latestMessagesForChat.RecentMessages;
            key = latestMessagesForChat.UnreadMessageKey;
          }
          else
            messageList = ((IEnumerable<Message>) db.GetLatestMessages(conversation.Jid, conversation.MessageLoadingStart(), new int?(1), new int?())).ToList<Message>();
          JidInfo jidInfo = db.GetJidInfo(conversation.Jid, CreateOptions.None);
          DateTime? nullable3 = jidInfo == null || !jidInfo.MuteExpirationUtc.HasValue || !(jidInfo.MuteExpirationUtc.Value > funNow) ? new DateTime?() : jidInfo.MuteExpirationUtc;
          DateTime? nullable4 = conversation.IsPinned() ? conversation.SortKey : new DateTime?();
          Pair<string, string> pair = (Pair<string, string>) null;
          changeNumberInfoDictionary.TryGetValue(conversation.Jid, out pair);
          string first = pair == null || !(pair.First != conversation.Jid) ? (string) null : pair.First;
          string second = pair == null || !(pair.Second != conversation.Jid) ? (string) null : pair.Second;
          return new FunXMPP.ChatResponse()
          {
            Jid = conversation.Jid,
            DisplayName = conversation.Jid.IsUserJid() ? (ci.UserStatus == null ? (string) null : ci.UserStatus.GetDisplayName(getNumberIfNoName: false)) : conversation.GetName(),
            Archived = conversation.IsArchived,
            ReadOnly = JidHelper.IsGroupJid(conversation.Jid) && !conversation.IsGroupParticipant(),
            Spam = SuspiciousJid.IsJidSuspicious(db, conversation.Jid, true),
            Timestamp = conversation.Timestamp,
            MuteExpiration = nullable3,
            PinTimestamp = nullable4,
            ModifyTag = (int) conversation.ModifyTag,
            Count = conversation.IsMarkedAsUnread() ? -1 : conversation.GetUnreadMessagesCount(),
            Messages = messageList ?? new List<Message>(),
            UnreadMessageKey = key,
            OldJid = first,
            NewJid = second
          };
        })).ToArray<FunXMPP.ChatResponse>();
      }));
      PerformanceTimer.End("web > get chats", start);
      return results;
    }

    private FunXMPP.MessagesResponse GetLatestMessagesForChat(
      Conversation convo,
      MessagesContext db)
    {
      FunXMPP.MessagesResponse latestMessagesForChat = new FunXMPP.MessagesResponse();
      int? unreadMessageCount = convo.UnreadMessageCount;
      int num = 50;
      if ((unreadMessageCount.GetValueOrDefault() > num ? (unreadMessageCount.HasValue ? 1 : 0) : 0) != 0)
      {
        Message[] messagesAfter = db.GetMessagesAfter(convo.Jid, new int?(), convo.FirstUnreadMessageID, true, new int?(26), new int?());
        Message[] messagesBefore = db.GetMessagesBefore(convo.Jid, new int?(), convo.FirstUnreadMessageID, new int?(25), new int?(), asc: true);
        Message[] latestMessages = db.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(1), new int?());
        Message message = ((IEnumerable<Message>) messagesAfter).FirstOrDefault<Message>();
        latestMessagesForChat.UnreadMessageKey = new FunXMPP.FMessage.Key(message.KeyRemoteJid, message.KeyFromMe, message.KeyId);
        latestMessagesForChat.RecentMessages = ((IEnumerable<Message>) messagesBefore).Concat<Message>((IEnumerable<Message>) messagesAfter).Concat<Message>((IEnumerable<Message>) latestMessages).ToList<Message>();
      }
      else
        latestMessagesForChat.RecentMessages = ((IEnumerable<Message>) db.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(50), new int?())).Reverse<Message>().ToList<Message>();
      return latestMessagesForChat;
    }

    private void Append<T>(ref IEnumerable<T> first, IEnumerable<T> second)
    {
      if (first == null)
        first = second;
      else
        first = first.Concat<T>(second);
    }

    public FunXMPP.ContactResponse[] GetContacts()
    {
      UserStatus[] contacts = (UserStatus[]) null;
      List<Conversation> groups = (List<Conversation>) null;
      List<Conversation> broadcastLists = (List<Conversation>) null;
      HashSet<string> mutedStatusJids = (HashSet<string>) null;
      ContactsContext.Instance((Action<ContactsContext>) (db =>
      {
        db.ReadAllUserStatuses();
        contacts = db.CachedUsers.Where<UserStatus>((Func<UserStatus, bool>) (cu => cu.IsInDeviceContactList || cu.IsSidelistSynced)).ToArray<UserStatus>();
      }));
      MessagesContext.RunRecursive((MessagesContext.MessagesCallback) (db =>
      {
        groups = db.GetGroups(true);
        broadcastLists = db.GetBroadcastLists();
        mutedStatusJids = db.GetStatusMutedJids();
      }));
      Log.l("web", "GetContacts | Contacts: {0}, Groups: {1}, BroadcastLists: {2}", (object) contacts.Length, (object) groups.Count, (object) broadcastLists.Count);
      IEnumerable<FunXMPP.ContactResponse> first = (IEnumerable<FunXMPP.ContactResponse>) null;
      this.Append<FunXMPP.ContactResponse>(ref first, (groups ?? new List<Conversation>()).Select<Conversation, FunXMPP.ContactResponse>((Func<Conversation, FunXMPP.ContactResponse>) (convo => new FunXMPP.ContactResponse()
      {
        Jid = convo.Jid,
        DisplayName = convo.GroupSubject ?? ""
      })));
      this.Append<FunXMPP.ContactResponse>(ref first, ((IEnumerable<UserStatus>) (contacts ?? new UserStatus[0])).Select<UserStatus, FunXMPP.ContactResponse>((Func<UserStatus, FunXMPP.ContactResponse>) (u => new FunXMPP.ContactResponse()
      {
        Jid = u.Jid,
        DisplayName = u.GetDisplayName(getNumberIfNoName: false),
        ShortName = u.FirstName,
        Notify = u.PushName,
        VName = u.GetVerifiedNameForDisplay(),
        Verify = u.VerificationLevelForWeb(),
        IsEnterprise = new bool?(u.IsEnterprise()),
        Checkmark = new bool?(u.ShowVerifiedBadgeForWeb())
      })));
      this.Append<FunXMPP.ContactResponse>(ref first, (broadcastLists ?? new List<Conversation>()).Select<Conversation, FunXMPP.ContactResponse>((Func<Conversation, FunXMPP.ContactResponse>) (convo => new FunXMPP.ContactResponse()
      {
        Jid = convo.Jid,
        DisplayName = convo.GroupSubject ?? ""
      })));
      this.Append<FunXMPP.ContactResponse>(ref first, (mutedStatusJids ?? new HashSet<string>()).Select<string, FunXMPP.ContactResponse>((Func<string, FunXMPP.ContactResponse>) (jid => new FunXMPP.ContactResponse()
      {
        Jid = jid,
        StatusMute = new bool?(true)
      })));
      return first.ToArray<FunXMPP.ContactResponse>();
    }

    public bool ShouldAddWebRelay(string keyId)
    {
      if (this.messagesPendingWebRelay == null || !this.messagesPendingWebRelay.Contains(keyId))
        return false;
      this.messagesPendingWebRelay.Remove(keyId);
      return true;
    }

    private void AddWebRelay(string keyId)
    {
      if (this.messagesPendingWebRelay == null)
        this.messagesPendingWebRelay = new Set<string>();
      if (this.messagesPendingWebRelay.Contains(keyId))
        return;
      this.messagesPendingWebRelay.Add(keyId);
    }

    public bool OnRelay(FunXMPP.FMessage fmsg)
    {
      bool messageRelayed = false;
      Message retryMediaMessage = (Message) null;
      Action postDbAction = (Action) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (db.MessageExists(fmsg.key.remote_jid, fmsg.key.id, true))
        {
          Message message = db.GetMessage(fmsg.key.remote_jid, fmsg.key.id, true);
          if (fmsg.media_wa_type == FunXMPP.FMessage.Type.Revoked)
          {
            if (message == null)
              return;
            db.RevokeMessageOnSubmit(message, out postDbAction);
            db.SubmitChanges();
          }
          else
          {
            if (MessageExtensions.GetUploadActionState(message) != MessageExtensions.MediaUploadActionState.Retryable || message == null || !message.LocalFileExists())
              return;
            retryMediaMessage = message;
            MessageProperties forMessage = MessageProperties.GetForMessage(retryMediaMessage);
            forMessage.EnsureWebClientProperties.WebRelay = new bool?(true);
            forMessage.Save();
            db.SubmitChanges();
          }
        }
        else
        {
          Message m = new Message(fmsg)
          {
            Status = FunXMPP.FMessage.Status.Relay
          };
          if (m.MediaName == null && m.MediaUrl != null)
          {
            int num = m.MediaUrl.LastIndexOf('/');
            if (num >= 0)
              m.MediaName = m.MediaUrl.Substring(num + 1);
          }
          db.InsertMessageOnSubmit(m);
          db.SubmitChanges();
          messageRelayed = true;
        }
      }));
      Action action = postDbAction;
      if (action != null)
        action();
      if (retryMediaMessage != null)
      {
        this.AddWebRelay(retryMediaMessage.KeyId);
        MediaUploadServices.RetryMediaMessageSend(retryMediaMessage, true);
        messageRelayed = true;
      }
      PresenceState.Instance.DisposeTimer(PresenceState.ChatStateSource.Qr);
      Log.l("web", "OnRelay | Jid: {0}, KeyId: {1}, Relayed: {2}, ReUpload: {3}", (object) fmsg.key.remote_jid, (object) fmsg.key.id, (object) messageRelayed, (object) (retryMediaMessage != null));
      return messageRelayed;
    }

    public void OnReUpload(FunXMPP.FMessage.Key webMsgKey, string id)
    {
      this.OnReUpload(MessagesContext.Select<Message>((Func<MessagesContext, Message>) (db => this.GetMessageWithKeyFromWeb(db, webMsgKey))), id, persist: true);
    }

    public void OnReUpload(Message msg, string id, Action onComplete = null, bool persist = false)
    {
      if (onComplete == null)
        onComplete = (Action) (() => { });
      Action<int> error = (Action<int>) (err =>
      {
        AppState.QrPersistentAction.NotifyMedia(id, (string) null, (string) null, new int?(err));
        onComplete();
      });
      Action<string, string> onUrl = (Action<string, string>) ((url, mediaKey) =>
      {
        AppState.QrPersistentAction.NotifyMedia(id, url, mediaKey, new int?());
        onComplete();
      });
      IObservable<MediaUploadMms4.Mms4UploadResult> obs = (IObservable<MediaUploadMms4.Mms4UploadResult>) null;
      if (msg == null)
        error(404);
      else if (!QrListener.IsMediaMessage(msg))
      {
        error(400);
      }
      else
      {
        bool hadMediaKey = msg.MediaKey != null;
        Message msg1 = msg;
        int? attempts = new int?();
        if ((obs = MediaUploadMms4.GetUploadObservableForResendMaybeMms4(msg1, (string) null, true, attempts)) == null)
        {
          error(404);
        }
        else
        {
          obs = obs.Do<MediaUploadMms4.Mms4UploadResult>((Action<MediaUploadMms4.Mms4UploadResult>) (res => onUrl(res.DownloadUrl, hadMediaKey || msg.MediaKey == null ? (string) null : Convert.ToBase64String(msg.MediaKey))), (Action<Exception>) (err =>
          {
            Log.LogException(err, "   WebClient > OnReUpload Error");
            error(502);
          }));
          Log.l("web", "OnReUpload - Jid: {0} KeyId: {1}", (object) msg.KeyRemoteJid, (object) msg.KeyId);
          if (persist)
          {
            IObservable<Unit> withDisposable = Observable.CreateWithDisposable<Unit>((Func<IObserver<Unit>, IDisposable>) (observer =>
            {
              Action inner = onComplete;
              onComplete = (Action) (() =>
              {
                inner();
                observer.OnNext(new Unit());
              });
              return obs.Subscribe<MediaUploadMms4.Mms4UploadResult>();
            }));
            AppState.QrPersistentAction.NotifyReupload<Unit>(msg, id, withDisposable);
          }
          else
            obs.Subscribe<MediaUploadMms4.Mms4UploadResult>();
        }
      }
    }

    private static bool IsMediaMessage(Message m)
    {
      return m.MediaWaType != FunXMPP.FMessage.Type.Undefined && m.MediaWaType != FunXMPP.FMessage.Type.ExtendedText && m.MediaWaType != FunXMPP.FMessage.Type.System && m.MediaWaType != FunXMPP.FMessage.Type.CipherText;
    }

    public string GetProfilePhotoId(string jid)
    {
      return ContactsContext.Instance<ChatPicture>((Func<ContactsContext, ChatPicture>) (db => db.GetChatPictureState(jid, CreateOptions.None)))?.WaPhotoId;
    }

    public void OnProfilePhotoRequest(string id, string jid, bool large)
    {
      AppState.QrPersistentAction.NotifyProfilePhotoRequest(id, jid, large);
    }

    public FunXMPP.PhotoResponse GetProfilePhoto(string jid, bool large)
    {
      ChatPicture chatPicture = ContactsContext.Instance<ChatPicture>((Func<ContactsContext, ChatPicture>) (db => db.GetChatPictureState(jid, CreateOptions.None)));
      string photoId = (string) null;
      string str = (string) null;
      if (chatPicture != null)
      {
        photoId = chatPicture.WaPhotoId;
        if (photoId != null)
          str = ChatPictureStore.GeneratePictureFilepath(jid, photoId, large);
      }
      Log.l("web", "GetProfilePhoto - Jid: {0} PhotoId: {1}, Found: {2}", (object) jid, (object) chatPicture.WaPhotoId, (object) (str != null));
      FunXMPP.PhotoResponse profilePhoto = new FunXMPP.PhotoResponse();
      profilePhoto.Id = photoId;
      if (str != null)
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            profilePhoto.Stream = nativeMediaStorage.OpenFile(Constants.IsoStorePath + "\\" + str, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
          profilePhoto.Id = (string) null;
        }
      }
      return profilePhoto;
    }

    public FunXMPP.ResumeResponse GetResumeState(Dictionary<string, FunXMPP.ChatResponse> webChats)
    {
      FunXMPP.ResumeResponse resumeState = new FunXMPP.ResumeResponse();
      List<FunXMPP.ChatResponse> existingChats = new List<FunXMPP.ChatResponse>();
      List<FunXMPP.ChatResponse> newChats = new List<FunXMPP.ChatResponse>();
      resumeState.ExistingChats = existingChats;
      resumeState.NewChats = newChats;
      resumeState.Checksum = Settings.ContactsChecksum;
      Log.l("web", "get resume state for {0} web chats", (object) webChats.Count);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Dictionary<string, FunXMPP.ChatResponse> dictionary = ((IEnumerable<FunXMPP.ChatResponse>) this.GetChats(false, (string) null)).ToDictionary<FunXMPP.ChatResponse, string, FunXMPP.ChatResponse>((Func<FunXMPP.ChatResponse, string>) (cr => cr.Jid), (Func<FunXMPP.ChatResponse, FunXMPP.ChatResponse>) (cr => cr));
        foreach (FunXMPP.ChatResponse chatResponse1 in webChats.Values)
        {
          bool flag = false;
          if (dictionary.ContainsKey(chatResponse1.Jid))
          {
            FunXMPP.ChatResponse chatResponse2 = dictionary[chatResponse1.Jid];
            if (chatResponse1.ModifyTag != chatResponse2.ModifyTag)
            {
              Log.l("web", "Resume {0}, ModifyTag {1} {2}", (object) chatResponse1.Jid, (object) chatResponse1.ModifyTag, (object) chatResponse2.ModifyTag);
              flag = true;
              chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Clear);
            }
            DateTime? nullable1 = chatResponse1.MuteExpiration;
            DateTime? nullable2 = chatResponse2.MuteExpiration;
            if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            {
              Log.l("web", "Resume {0}, MuteExpiration {1} {2}", (object) chatResponse1.Jid, (object) chatResponse1.MuteExpiration, (object) chatResponse2.MuteExpiration);
              flag = true;
            }
            nullable2 = chatResponse1.PinTimestamp;
            nullable1 = chatResponse2.PinTimestamp;
            if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            {
              Log.l("web", "Resume {0}, PinTimestamp {1} {2}", (object) chatResponse1.Jid, (object) chatResponse1.PinTimestamp, (object) chatResponse2.PinTimestamp);
              flag = true;
            }
            if (chatResponse1.Archived != chatResponse2.Archived)
            {
              Log.l("web", "Resume {0}, Archived {1} {2}", (object) chatResponse1.Jid, (object) chatResponse1.Archived, (object) chatResponse2.Archived);
              flag = true;
            }
            if (chatResponse1.Count != chatResponse2.Count)
            {
              Log.l("web", "Resume {0}, UnreadCount {1} {2}", (object) chatResponse1.Jid, (object) chatResponse1.Count, (object) chatResponse2.Count);
              flag = true;
            }
            if (!chatResponse2.Type.HasValue)
            {
              Message message = chatResponse2.Messages.FirstOrDefault<Message>();
              if (message == null)
              {
                if (chatResponse1.LastMessageKey.id != null)
                {
                  Log.l("web", "Resume {0}, ClearMessages", (object) chatResponse1.Jid);
                  chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Clear);
                  flag = true;
                }
              }
              else if (message.KeyId != chatResponse1.LastMessageKey.id)
              {
                flag = true;
                Message messageWithKeyFromWeb = this.GetMessageWithKeyFromWeb(db, chatResponse1.LastMessageKey);
                if (chatResponse2.Count <= 0)
                {
                  List<Message> list = ((IEnumerable<Message>) db.GetLatestMessages(chatResponse2.Jid, messageWithKeyFromWeb?.MessageID, new int?(50), new int?())).Reverse<Message>().ToList<Message>();
                  if (messageWithKeyFromWeb == null || list[0].MessageID > messageWithKeyFromWeb.MessageID)
                  {
                    Log.l("web", "Resume {0}, NoUnread TooManyMessages", (object) chatResponse1.Jid);
                    chatResponse2.Messages = list;
                    chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Resend);
                  }
                  else
                  {
                    Log.l("web", "Resume {0}, NoUnread CatchUpMessages", (object) chatResponse1.Jid);
                    chatResponse2.Messages = this.KeepMessagesAfterMatch(list, messageWithKeyFromWeb);
                    chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Ahead);
                  }
                }
                else
                {
                  Conversation conversation = db.GetConversation(chatResponse2.Jid, CreateOptions.None);
                  Message messageById = db.GetMessageById(conversation.FirstUnreadMessageID.GetValueOrDefault());
                  if (messageWithKeyFromWeb.MessageID < messageById.MessageID)
                  {
                    FunXMPP.MessagesResponse latestMessagesForChat = this.GetLatestMessagesForChat(conversation, db);
                    List<Message> recentMessages = latestMessagesForChat.RecentMessages;
                    if (recentMessages[0].MessageID < messageWithKeyFromWeb.MessageID)
                    {
                      Log.l("web", "Resume {0}, MissingUnread CatchUpMessages", (object) chatResponse1.Jid);
                      chatResponse2.Messages = this.KeepMessagesAfterMatch(recentMessages, messageWithKeyFromWeb);
                      chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Ahead);
                    }
                    else
                    {
                      Log.l("web", "Resume {0}, MissingUnread SendUnreadMessages", (object) chatResponse1.Jid);
                      chatResponse2.Messages = recentMessages;
                      chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Resend);
                      chatResponse2.UnreadMessageKey = latestMessagesForChat.UnreadMessageKey;
                    }
                  }
                  else
                  {
                    Message[] messagesAfter = this.GetMessagesAfter(chatResponse1.LastMessageKey, true, new int?(50), false, false, new FunXMPP.FMessage.Type?());
                    if (((IEnumerable<Message>) messagesAfter).Last<Message>().MessageID == message.MessageID)
                    {
                      Log.l("web", "Resume {0}, SeenUnread CatchUpMessages", (object) chatResponse1.Jid);
                      chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Ahead);
                      chatResponse2.Messages = ((IEnumerable<Message>) messagesAfter).ToList<Message>();
                    }
                    else
                    {
                      Log.l("web", "Resume {0}, SeenUnread ResettingLatestMessage", (object) chatResponse1.Jid);
                      chatResponse2.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Resend);
                      chatResponse2.Messages = new List<Message>()
                      {
                        message
                      };
                    }
                  }
                }
              }
              else
                chatResponse2.Messages = (List<Message>) null;
            }
            else
            {
              FunXMPP.ChatResponseAction? type = chatResponse2.Type;
              FunXMPP.ChatResponseAction chatResponseAction = FunXMPP.ChatResponseAction.Clear;
              if ((type.GetValueOrDefault() == chatResponseAction ? (type.HasValue ? 1 : 0) : 0) != 0)
              {
                Conversation conversation = db.GetConversation(chatResponse2.Jid, CreateOptions.None);
                chatResponse2.Messages = conversation == null ? new List<Message>() : ((IEnumerable<Message>) conversation.GetLatestMessages(db, new int?(20), new int?(0))).Reverse<Message>().ToList<Message>();
              }
            }
            if (flag)
              existingChats.Add(chatResponse2);
          }
          else
          {
            Log.l("web", "Resume,Delete {0}", (object) chatResponse1.Jid);
            chatResponse1.Type = new FunXMPP.ChatResponseAction?(FunXMPP.ChatResponseAction.Delete);
            chatResponse1.Messages = (List<Message>) null;
            existingChats.Add(chatResponse1);
          }
        }
        foreach (FunXMPP.ChatResponse chatResponse in dictionary.Values)
        {
          if (!webChats.ContainsKey(chatResponse.Jid))
            newChats.Add(chatResponse);
        }
      }));
      Log.l("web", "GetResumeState - DifferentChats: {0}, NewChats: {1}", (object) resumeState.ExistingChats.Count, (object) resumeState.NewChats.Count);
      return resumeState;
    }

    private List<Message> KeepMessagesAfterMatch(List<Message> phoneMessages, Message webMessage)
    {
      while (phoneMessages.Count > 0 && phoneMessages[0].MessageID < webMessage.MessageID)
        phoneMessages.RemoveAt(0);
      if (phoneMessages.Count == 0)
      {
        Log.l("web", "Resume, Messages, WebClient is ahead of Phone? Web: " + webMessage.KeyId + " - " + (object) webMessage.MessageID);
        return (List<Message>) null;
      }
      if (phoneMessages[0] == webMessage)
        phoneMessages.RemoveAt(0);
      return phoneMessages;
    }

    public FunXMPP.ReceiptResponse GetReceiptStates(FunXMPP.FMessage.Key key, DateTime tsUtc)
    {
      Log.l("web", "GetReceiptStates - Jid: {0}, KeyId: {1}, Timestamp: {2}", (object) key.remote_jid, (object) key.id, (object) tsUtc);
      Message[] msgs = this.GetMessagesAfter(key, true, new int?(), true, false, new FunXMPP.FMessage.Type?());
      if (msgs.Length == 0 || !((IEnumerable<Message>) msgs).Any<Message>())
        return (FunXMPP.ReceiptResponse) null;
      FunXMPP.ReceiptResponse receiptStates = new FunXMPP.ReceiptResponse();
      DateTime? latestReceiptTimestamp = new DateTime?();
      List<FunXMPP.ReceiptStateResponse> states = new List<FunXMPP.ReceiptStateResponse>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (Message m in msgs)
        {
          if (m.KeyFromMe)
          {
            bool flag = false;
            foreach (ReceiptState receiptState in db.GetReceiptsForMessage(m.MessageID))
            {
              if (receiptState.Status == m.Status && receiptState.Timestamp > tsUtc)
              {
                if (!flag)
                {
                  states.Add(new FunXMPP.ReceiptStateResponse(m.KeyId, m.KeyFromMe, m.Status));
                  flag = true;
                }
                if (!latestReceiptTimestamp.HasValue || latestReceiptTimestamp.Value < receiptState.Timestamp)
                  latestReceiptTimestamp = new DateTime?(receiptState.Timestamp);
              }
            }
          }
          else if (m.IsPtt())
            states.Add(new FunXMPP.ReceiptStateResponse(m.KeyId, m.KeyFromMe, m.Status));
        }
      }));
      receiptStates.Timestamp = latestReceiptTimestamp;
      receiptStates.Receipts = states.ToArray();
      Log.l("web", "GetReceiptStates Result - Jid: {0}, Count: {1}, Final Timestamp: {2}", (object) key.remote_jid, (object) states.Count, (object) latestReceiptTimestamp);
      return receiptStates;
    }

    public FunXMPP.MessageInfoStateResponse GetMessageInfoState(FunXMPP.FMessage.Key key)
    {
      Log.l("web", "GetMessageInfo- Jid: {0}, KeyId: {1}", (object) key.remote_jid, (object) key.id);
      FunXMPP.MessageInfoStateResponse resp = (FunXMPP.MessageInfoStateResponse) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = db.GetMessage(key.remote_jid, key.id, key.from_me);
        if (message == null || !message.KeyFromMe)
          return;
        resp = new FunXMPP.MessageInfoStateResponse();
        resp.Count = message.GetExpectedDeliveryCount(db);
        Dictionary<string, ReceiptState> source = new Dictionary<string, ReceiptState>();
        List<ReceiptState> receiptsForMessage = db.GetReceiptsForMessage(message.MessageID);
        if (JidHelper.IsMultiParticipantsChatJid(key.remote_jid))
        {
          foreach (ReceiptState receiptState1 in receiptsForMessage.Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.Jid != null)))
          {
            ReceiptState receiptState2 = source.ContainsKey(receiptState1.Jid) ? source[receiptState1.Jid] : receiptState1;
            if (receiptState1.Status.GetOverrideWeight() > receiptState2.Status.GetOverrideWeight())
              receiptState2 = receiptState1;
            source[receiptState1.Jid] = receiptState2;
          }
          resp.Played = source.Where<KeyValuePair<string, ReceiptState>>((Func<KeyValuePair<string, ReceiptState>, bool>) (r => r.Value.Status == FunXMPP.FMessage.Status.PlayedByTarget)).Select<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>((Func<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(r.Value.Jid, r.Value.Timestamp)));
          resp.Read = source.Where<KeyValuePair<string, ReceiptState>>((Func<KeyValuePair<string, ReceiptState>, bool>) (r => r.Value.Status == FunXMPP.FMessage.Status.ReadByTarget)).Select<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>((Func<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(r.Value.Jid, r.Value.Timestamp)));
          resp.Delivered = source.Where<KeyValuePair<string, ReceiptState>>((Func<KeyValuePair<string, ReceiptState>, bool>) (r => r.Value.Status == FunXMPP.FMessage.Status.ReceivedByTarget)).Select<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>((Func<KeyValuePair<string, ReceiptState>, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(r.Value.Jid, r.Value.Timestamp)));
        }
        else
        {
          resp.Played = receiptsForMessage.Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.Status == FunXMPP.FMessage.Status.PlayedByTarget)).Select<ReceiptState, FunXMPP.MessageInfoResponse>((Func<ReceiptState, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(key.remote_jid, r.Timestamp)));
          resp.Read = receiptsForMessage.Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.Status == FunXMPP.FMessage.Status.ReadByTarget)).Select<ReceiptState, FunXMPP.MessageInfoResponse>((Func<ReceiptState, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(key.remote_jid, r.Timestamp)));
          resp.Delivered = receiptsForMessage.Where<ReceiptState>((Func<ReceiptState, bool>) (r => r.Status == FunXMPP.FMessage.Status.ReceivedByTarget)).Select<ReceiptState, FunXMPP.MessageInfoResponse>((Func<ReceiptState, FunXMPP.MessageInfoResponse>) (r => new FunXMPP.MessageInfoResponse(key.remote_jid, r.Timestamp)));
        }
      }));
      return resp;
    }

    public FunXMPP.Connection.GroupInfo GetGroupMetadata(string jid)
    {
      Log.l("web", "GetGroupMetadata - Jid: {0}", (object) jid);
      FunXMPP.Connection.GroupInfo info = (FunXMPP.Connection.GroupInfo) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation = db.GetConversation(jid, CreateOptions.None);
        if (conversation == null)
          return;
        info = new FunXMPP.Connection.GroupInfo();
        info.Subject = conversation.GetName();
        info.CreationTime = conversation.GroupCreationT;
        info.CreatorJid = conversation.GroupOwner;
        conversation.ParticipantSetAction((Action<GroupParticipants>) (gp =>
        {
          info.AdminJids = gp.Admins != null ? gp.Admins.ToList<string>() : (List<string>) null;
          info.NonadminJids = gp.NonAdmins != null ? gp.NonAdmins.ToList<string>() : (List<string>) null;
        }));
        info.AnnouncementOnly = conversation.IsAnnounceOnlyForUser();
        info.Locked = conversation.IsLocked() && !conversation.UserIsAdmin(Settings.MyJid);
      }));
      return info;
    }

    public FunXMPP.ActionResponse GetActions(string[] ids)
    {
      Log.l("web", "GetActions Query");
      FunXMPP.ActionResponse actions = new FunXMPP.ActionResponse();
      if (this.Session.PendingActions.InvalidForQuery)
      {
        actions.Replaced = true;
        Log.l("web", "GetActions Query - Replaced");
      }
      else
      {
        actions.Actions = new Dictionary<string, int>();
        if (ids != null)
        {
          foreach (string id in ids)
          {
            if (this.Session.PendingActions.Actions.ContainsKey(id))
              actions.Actions[id] = this.Session.PendingActions.Actions[id];
          }
        }
      }
      return actions;
    }

    public IEnumerable<Pair<string, double>> GetEmojis(IEnumerable<Pair<string, double>> webEmojis)
    {
      Log.l("web", "GetEmojis Query - Merge: " + (object) webEmojis != null ? "true" : "false");
      IEnumerable<Pair<string, double>> phoneEmojis = (IEnumerable<Pair<string, double>>) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (webEmojis != null && webEmojis.Any<Pair<string, double>>())
        {
          foreach (Pair<string, double> webEmoji in webEmojis)
          {
            EmojiUsage emojiUsage = db.GetEmojiUsage(webEmoji.First, true);
            if (emojiUsage != null)
            {
              emojiUsage.UsageWeight = (int) (((double) emojiUsage.UsageWeight + webEmoji.Second * 10000.0) / 2.0);
            }
            else
            {
              EmojiUsage eu = new EmojiUsage()
              {
                EmojiCode = webEmoji.First,
                UsageWeight = (int) (webEmoji.Second * 10000.0)
              };
              db.InsertEmojiUsageOnSubmit(eu);
            }
          }
          db.SubmitChanges();
        }
        phoneEmojis = db.GetAllEmojiUsages().Select<EmojiUsage, Pair<string, double>>((Func<EmojiUsage, Pair<string, double>>) (eu => new Pair<string, double>(eu.EmojiCode, (double) eu.UsageWeight / 10000.0)));
      }));
      return phoneEmojis;
    }

    public int OnMarkAsRead(
      string jid,
      string keyId,
      bool fromMe,
      string participant,
      int? count)
    {
      int status = 200;
      Log.l("web", "on mark as read | jid: {0}, key id: {1}, count: {2}", (object) jid, (object) keyId, (object) (count ?? -99));
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Conversation conversation1 = db.GetConversation(jid, CreateOptions.None);
        if (conversation1 == null)
        {
          Log.l("web", "on mark as read | convo not found");
          status = 404;
        }
        else
        {
          Message message = ((IEnumerable<Message>) db.GetLatestMessages(jid, new int?(), new int?(1), new int?())).FirstOrDefault<Message>();
          FunXMPP.FMessage.Key webMsgKey = new FunXMPP.FMessage.Key(jid, fromMe, keyId);
          int unreadMessagesCount = conversation1.GetUnreadMessagesCount();
          if (count.HasValue && count.Value == -2)
          {
            if (message != null && webMsgKey.Equals((object) new FunXMPP.FMessage.Key(message.KeyRemoteJid, message.KeyFromMe, message.KeyId)) && unreadMessagesCount == 0)
              MarkChatRead.MarkUnread(db, jid, false);
            else
              status = 409;
          }
          else if (unreadMessagesCount > 0)
          {
            Message messageWithKeyFromWeb = this.GetMessageWithKeyFromWeb(db, webMsgKey);
            if (messageWithKeyFromWeb == null)
              return;
            if (message != null && webMsgKey.Equals((object) new FunXMPP.FMessage.Key(message.KeyRemoteJid, message.KeyFromMe, message.KeyId)))
            {
              MarkChatRead.MarkRead(db, jid, false, true);
            }
            else
            {
              if (!count.HasValue)
                return;
              if (count.Value > unreadMessagesCount)
              {
                Log.l("web", "on mark as read | count exceeds unread");
                status = 409;
              }
              else
              {
                MarkChatRead.MarkReadToMessage(db, jid, new int?(messageWithKeyFromWeb.MessageID), false, true);
                Conversation conversation2 = conversation1;
                int? unreadMessageCount = conversation2.UnreadMessageCount;
                int num = count.Value;
                conversation2.UnreadMessageCount = unreadMessageCount.HasValue ? new int?(unreadMessageCount.GetValueOrDefault() - num) : new int?();
              }
            }
          }
          else
          {
            if (!conversation1.IsMarkedAsUnread() || !count.HasValue)
              return;
            int? nullable = count;
            int num = -1;
            if ((nullable.GetValueOrDefault() == num ? (nullable.HasValue ? 1 : 0) : 0) == 0)
              return;
            Log.l("web", "on mark as read | count and unread are both -1");
            MarkChatRead.MarkRead(db, jid, false, true);
          }
        }
      }));
      return status;
    }

    public int OnMarkAsSeen(string jid, string keyId, bool fromMe, string senderJid)
    {
      int result = 200;
      if (fromMe)
        return result;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = db.GetMessage(jid, keyId, fromMe);
        if (message != null)
        {
          WaStatus waStatus1 = db.GetWaStatus(senderJid, keyId);
          if (waStatus1 != null)
          {
            waStatus1.IsViewed = true;
            if (JidHelper.IsPsaJid(senderJid) && Settings.IsStatusPSAUnseen)
            {
              Settings.IsStatusPSAUnseen = false;
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              WaStatus[] statuses = db.GetStatuses("0@s.whatsapp.net", false, true, new TimeSpan?());
              foreach (WaStatus waStatus2 in statuses)
              {
                waStatus2.Timestamp = currentServerTimeUtc;
                Message messageById = db.GetMessageById(waStatus2.MessageId);
                if (messageById != null)
                  messageById.FunTimestamp = new DateTime?(currentServerTimeUtc);
              }
            }
          }
          ReadReceipts.SendMessageReceipt(db, message, message.IsPtt() ? FunXMPP.FMessage.Status.PlayedByTarget : FunXMPP.FMessage.Status.ReadByTarget);
        }
        else
          result = 404;
      }));
      return result;
    }

    public void OnAvailable(bool on, DateTime? presenceTimestamp)
    {
      Log.l("web", "OnAvailable - Timestamp: {0}", (object) presenceTimestamp.GetValueOrDefault());
      if (!on && presenceTimestamp.HasValue)
        this.Session.UpdateLastConnected((byte[]) null, presenceTimestamp.Value);
      this.Session.Available = on;
      if (on && AppState.IsBackgroundAgent)
        AppState.GetConnection()?.SendAvailableForChat(true);
      if (on)
        return;
      PresenceState.Instance.DisposeTimer(PresenceState.ChatStateSource.Qr);
    }

    public void OnComposing(string jid, string gjid, Presence type)
    {
      Log.l("web", "OnComposing - Jid: {0}, GroupJid: {1}, Type: {2}", (object) jid, (object) gjid, (object) type);
      PresenceState instance = PresenceState.Instance;
      PresenceState.ChatStateSource source = PresenceState.ChatStateSource.Qr;
      if (type == Presence.Online || type == Presence.Offline)
        instance.SendPaused(gjid ?? jid, source: source);
      else
        instance.SendComposing(gjid ?? jid, type, participant: gjid == null ? (string) null : jid, source: source);
    }

    public int OnReceipt(string type, string messageID, string remoteJid, string participant)
    {
      int num = 400;
      Log.l("web", "on receipt - jid: {0}, keyid: {1}, type: {2}", (object) remoteJid, (object) messageID, (object) type);
      if (type == "played")
      {
        Message msg = (Message) null;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db => msg = db.GetMessage(remoteJid, messageID, false)));
        if (msg != null)
        {
          ReadReceipts.SendMessageReceipt(msg, FunXMPP.FMessage.Status.PlayedByTarget);
          num = 200;
        }
      }
      return num;
    }

    public void OnSetStatus(string status, string incomingId)
    {
      Log.l("web", "SetStatus - Status: {0}", (object) status);
      if (string.IsNullOrEmpty(status))
        return;
      SetStatus.Set(status, incomingId);
    }

    public int OnSetChat(FunXMPP.ChatResponse chat)
    {
      int status = 200;
      FunXMPP.ChatSetAction? setType = chat.SetType;
      if (setType.HasValue)
      {
        switch (setType.GetValueOrDefault())
        {
          case FunXMPP.ChatSetAction.Clear:
            Log.l("web", "OnSetChat - Clear - Jid: {0}, KeyId: {1}", (object) chat.Jid, chat.LastMessageKey != null ? (object) chat.LastMessageKey.id : (object) "all");
            ClearChat.Clear(chat.Jid, false);
            break;
          case FunXMPP.ChatSetAction.Delete:
            Log.l("web", "OnSetChat - Delete - Jid: {0}", (object) chat.Jid);
            bool clearChatInstead = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Message message = ((IEnumerable<Message>) db.GetLatestMessages(chat.Jid, new int?(), new int?(1), new int?())).FirstOrDefault<Message>();
              Message messageWithKeyFromWeb = chat.LastMessageKey == null ? (Message) null : this.GetMessageWithKeyFromWeb(db, new FunXMPP.FMessage.Key(chat.Jid, chat.LastMessageKey.from_me, chat.LastMessageKey.id));
              if (chat.LastMessageKey != null && messageWithKeyFromWeb == null)
                status = 409;
              if (messageWithKeyFromWeb == message)
                return;
              clearChatInstead = true;
            }));
            if (status != 200)
              return status;
            if (!clearChatInstead)
            {
              DeleteChat.Delete(chat.Jid);
              break;
            }
            goto case FunXMPP.ChatSetAction.Clear;
          case FunXMPP.ChatSetAction.Archive:
            Log.l("web", "OnSetChat - Archive - Jid: {0}, Archive: {1}", (object) chat.Jid, (object) chat.Archived);
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Message message = ((IEnumerable<Message>) db.GetLatestMessages(chat.Jid, new int?(), new int?(1), new int?())).FirstOrDefault<Message>();
              if (message == null && chat.LastMessageKey == null)
                return;
              if (message != null && chat.LastMessageKey != null)
              {
                if (!(message.KeyId != chat.LastMessageKey.id))
                  return;
                status = 409;
              }
              else
                status = 409;
            }));
            if (status != 200)
              return status;
            if (chat.Archived)
            {
              ArchiveChat.Archive(chat.Jid, false);
              break;
            }
            ArchiveChat.Unarchive(chat.Jid, false);
            break;
          case FunXMPP.ChatSetAction.Mute:
            Log.l("web", "mute | jid:{0},exp:{1}", (object) chat.Jid, (object) chat.MuteExpiration);
            MuteChat.MuteSynchronous(new string[1]
            {
              chat.Jid
            }, chat.MuteExpiration, false);
            break;
          case FunXMPP.ChatSetAction.Unmute:
            Log.l("web", "unmute | jid:{0}", (object) chat.Jid, (object) chat.MuteExpiration);
            bool toUnmute = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              JidInfo jidInfo = db.GetJidInfo(chat.Jid, CreateOptions.None);
              if (jidInfo == null)
                return;
              ref DateTime? local1 = ref chat.MuteExpiration;
              long? nullable1 = local1.HasValue ? new long?(local1.GetValueOrDefault().ToUnixTime()) : new long?();
              DateTime? muteExpirationUtc = jidInfo.MuteExpirationUtc;
              ref DateTime? local2 = ref muteExpirationUtc;
              long? nullable2 = local2.HasValue ? new long?(local2.GetValueOrDefault().ToUnixTime()) : new long?();
              if ((nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? (nullable1.HasValue != nullable2.HasValue ? 1 : 0) : 1) != 0)
                status = 409;
              else
                toUnmute = true;
            }));
            if (toUnmute)
            {
              MuteChat.MuteSynchronous(new string[1]
              {
                chat.Jid
              }, new DateTime?(), false);
              break;
            }
            break;
          case FunXMPP.ChatSetAction.Unstar:
            Log.l("web", "OnSetChat - Unstar (All) - Jid: {0}", (object) (chat.Jid ?? "no jid"));
            AppState.Worker.Enqueue((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              MessagesContext messagesContext = db;
              string[] jids;
              if (chat.Jid != null)
                jids = new string[1]{ chat.Jid };
              else
                jids = (string[]) null;
              messagesContext.UnstarMessages(jids);
            }))));
            break;
          case FunXMPP.ChatSetAction.NotSpam:
            Log.l("web", "OnSetChat - NotSpam - Jid: {0}", (object) (chat.Jid ?? "no jid"));
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => SuspiciousJid.MarkJidSuspicious((SqliteMessagesContext) db, chat.Jid, false)));
            break;
          case FunXMPP.ChatSetAction.Pin:
            Log.l("web", "pin | jid:{0},pin:{1}", (object) chat.Jid, (object) chat.PinTimestamp);
            bool toPin = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              if (((IEnumerable<Conversation>) db.GetPinnedConversations()).Count<Conversation>() >= 3)
              {
                status = 405;
              }
              else
              {
                Conversation conversation = db.GetConversation(chat.Jid, CreateOptions.None);
                if (conversation == null)
                  return;
                if (!conversation.SortKey.HasValue)
                {
                  toPin = true;
                }
                else
                {
                  ref DateTime? local3 = ref chat.PinTimestamp;
                  long? nullable3 = local3.HasValue ? new long?(local3.GetValueOrDefault().ToUnixTime()) : new long?();
                  DateTime? sortKey = conversation.SortKey;
                  ref DateTime? local4 = ref sortKey;
                  long? nullable4 = local4.HasValue ? new long?(local4.GetValueOrDefault().ToUnixTime()) : new long?();
                  if ((nullable3.GetValueOrDefault() == nullable4.GetValueOrDefault() ? (nullable3.HasValue != nullable4.HasValue ? 1 : 0) : 1) == 0)
                    return;
                  status = 409;
                }
              }
            }));
            if (toPin)
            {
              PinChat.Pin(chat.Jid);
              break;
            }
            break;
          case FunXMPP.ChatSetAction.Unpin:
            Log.l("web", "unpin | jid:{0},pin:{1}", (object) chat.Jid, (object) chat.PinTimestamp);
            bool toUnpin = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
            {
              Conversation conversation = db.GetConversation(chat.Jid, CreateOptions.None);
              if (conversation == null)
                return;
              DateTime? sortKey = conversation.SortKey;
              if (!sortKey.HasValue)
                return;
              ref DateTime? local5 = ref chat.PinTimestamp;
              long? nullable5 = local5.HasValue ? new long?(local5.GetValueOrDefault().ToUnixTime()) : new long?();
              sortKey = conversation.SortKey;
              ref DateTime? local6 = ref sortKey;
              long? nullable6 = local6.HasValue ? new long?(local6.GetValueOrDefault().ToUnixTime()) : new long?();
              if ((nullable5.GetValueOrDefault() == nullable6.GetValueOrDefault() ? (nullable5.HasValue != nullable6.HasValue ? 1 : 0) : 1) != 0)
                status = 409;
              else
                toUnpin = true;
            }));
            if (toUnpin)
            {
              PinChat.Unpin(chat.Jid, false);
              break;
            }
            break;
        }
      }
      return status;
    }

    public void OnDeleteMessages(FunXMPP.FMessage.Key[] keys)
    {
      Log.l("web", "delete messages from chat: " + string.Join(", ", ((IEnumerable<FunXMPP.FMessage.Key>) keys).Select<FunXMPP.FMessage.Key, string>((Func<FunXMPP.FMessage.Key, string>) (k => k.remote_jid + " - " + k.id))));
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        List<Message> source = new List<Message>();
        foreach (FunXMPP.FMessage.Key key in keys)
        {
          Message message = db.GetMessage(key.remote_jid, key.id, key.from_me);
          if (message != null)
            source.Add(message);
        }
        if (!source.Any<Message>())
          return;
        db.DeleteMessages(source.ToArray());
      }));
    }

    public void OnChangeNumberNotificationDismiss(string jid)
    {
      ContactsContext.Instance((Action<ContactsContext>) (db => db.DeleteChangeNumberRecordsForJid(new string[1]
      {
        jid
      })));
    }

    public void OnStarMessages(bool star, FunXMPP.FMessage.Key[] keys)
    {
      string str = string.Join(", ", ((IEnumerable<FunXMPP.FMessage.Key>) keys).Select<FunXMPP.FMessage.Key, string>((Func<FunXMPP.FMessage.Key, string>) (k => k.remote_jid + " - " + k.id)));
      Log.l("web", (star ? nameof (star) : "unstar") + " messages from chat: " + str);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (FunXMPP.FMessage.Key key in keys)
        {
          Message message = db.GetMessage(key.remote_jid, key.id, key.from_me);
          if (message != null)
            message.IsStarred = star;
          db.GetConversation(message.KeyRemoteJid, CreateOptions.None)?.UpdateModifyTag();
        }
        db.SubmitChanges();
      }));
    }

    public void OnUnsubscribeLocation(string jid, string id)
    {
      Log.l("web", "on unsubscribe location for | jid: {0} id: {1}", (object) jid, (object) id);
      LiveLocationManager.Instance.UnsubscribeToLocationUpdates(jid, id);
    }

    public int OnDisableLocation(string jid, string id)
    {
      int num = 200;
      Log.l("web", "on disable location for | jid: {0} id: {1}", (object) jid, (object) id);
      if (LiveLocationManager.Instance.IsSharingLocationWithJid(jid))
        LiveLocationManager.Instance.DisableLocationSharing(jid, id, wam_enum_live_location_sharing_session_ended_reason.USER_CANCELED);
      return num;
    }

    public FunXMPP.VerifySessionResponse VerifyCurrent(
      FunXMPP.VerifySessionType type,
      byte[] sessionData,
      byte[] browserId,
      byte[] clientToken,
      List<Action> resultActions,
      string os = null,
      string browser = null)
    {
      bool active = this.Session.Active;
      byte[] numArray = (byte[]) null;
      if (active)
        numArray = this.Session.ActiveBrowserId;
      FunXMPP.VerifySessionResponse resp = this.Session.VerifyCurrent(ref type, sessionData, browserId, clientToken, os, browser);
      Log.l("web", "VerifyCurrent - Type: {0}, Result: {1}", (object) type, (object) resp.Response);
      if (resp.Response == FunXMPP.VerifySessionResponse.ResponseType.Accept)
      {
        if (active)
        {
          if (this.Session.Available)
            this.Session.UpdateLastConnected(numArray, DateTime.Now);
          if (!numArray.IsEqualBytes(browserId))
          {
            this.Session.Disconnect();
            this.Session.PendingActions.InvalidForQuery = true;
          }
          else
            this.Session.PendingActions.InvalidForQuery = false;
          resultActions.Add((Action) (() => AppState.GetConnection().SendQrUnsync(false)));
        }
        resultActions.Add((Action) (() =>
        {
          AppState.GetConnection().SendQrSync(sessionData, resp.ClientToken, true, onComplete: (Action<string, string, bool>) ((syncOs, syncBrowser, timeout) =>
          {
            if (type == FunXMPP.VerifySessionType.Challenge)
              this.Session.DiscardChallenge();
            this.OnQrOnline(new bool?(timeout));
          }), onError: (Action<int>) (err =>
          {
            Log.l("web", "SendQrSync - ERROR - Error: {0}", (object) err);
            if (!QrSession.IsKnownCode(err))
              this.OnLogout(browserId, (byte[]) null);
            else
              this.OnDisconnect((byte[]) null, (byte[]) null, new DateTime?());
          }));
          if (type != FunXMPP.VerifySessionType.Resume && type != FunXMPP.VerifySessionType.ForcedResume)
          {
            AppState.QrPersistentAction.NotifyPreemptiveChats();
            AppState.QrPersistentAction.NotifyPreemptiveContacts();
          }
          AppState.QrPersistentAction.NotifyFrequentContacts();
          this.OnSessionConnected();
        }));
      }
      return resp;
    }

    public FunXMPP.VerifySessionResponse VerifyQuerySync(
      FunXMPP.VerifySessionType type,
      byte[] sessionData)
    {
      Log.l("web", "Verify Ping Response");
      return this.Session.VerifyQuerySync(type, sessionData);
    }

    public void OnDisconnect(byte[] sessionData, byte[] browserid, DateTime? timestamp)
    {
      Log.l("web", "OnDisconnect - BrowserId: {0}", browserid != null ? (object) Convert.ToBase64String(browserid) : (object) "");
      if (timestamp.HasValue)
        this.Session.UpdateLastConnected(browserid, timestamp.Value);
      if (!this.Session.Active || sessionData != null && !sessionData.IsEqualBytes(this.Session.Id))
        return;
      this.Session.Disconnect();
      this.OnAvailable(false, timestamp);
    }

    public void OnLogout(byte[] browserid, byte[] code)
    {
      Log.l("web", "OnLogout - BrowserId: {0}", browserid != null ? (object) Convert.ToBase64String(browserid) : (object) "");
      if (browserid == null)
      {
        if (!this.Session.Active)
          return;
        browserid = this.session.ActiveBrowserId;
      }
      this.Session.DeleteBrowser(browserid, code);
      PresenceState.Instance.DisposeTimer(PresenceState.ChatStateSource.Qr);
    }

    public void OnQrOnline(bool? forgetMe)
    {
      Log.l("web", "OnQrOnline - ForgetMe: {0}", (object) forgetMe.GetValueOrDefault());
      if (forgetMe.HasValue)
        this.Session.SetActiveForgetMe(forgetMe.Value);
      this.OnSessionConnected();
    }

    public void OnQrError(int code)
    {
      Log.l("web", "OnQrError - Code: {0}", (object) code);
      if (code >= 500)
        return;
      this.OnDisconnect((byte[]) null, (byte[]) null, new DateTime?());
    }
  }
}
