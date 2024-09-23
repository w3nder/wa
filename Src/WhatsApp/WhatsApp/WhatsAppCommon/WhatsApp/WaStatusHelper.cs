// Decompiled with JetBrains decompiler
// Type: WhatsApp.WaStatusHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class WaStatusHelper
  {
    public const string WhiteListId = "status_white";
    public const string BlackListId = "status_black";
    public const string ContactsStr = "contacts";
    public const string BlackListStr = "blacklist";
    public const string WhiteListStr = "whitelist";
    public static long SessionId;

    public static void GenerateSessionId()
    {
      WaStatusHelper.SessionId = (long) new Random().Next(2097152);
    }

    public static string GetStrForList(WaStatusHelper.StatusPrivacySettings listType)
    {
      string str = (string) null;
      switch (listType)
      {
        case WaStatusHelper.StatusPrivacySettings.Contacts:
          str = "contacts";
          break;
        case WaStatusHelper.StatusPrivacySettings.WhiteList:
          str = "whitelist";
          break;
        case WaStatusHelper.StatusPrivacySettings.BlackList:
          str = "blacklist";
          break;
      }
      return str ?? "";
    }

    private static void UpdateStatusRecipients(SqliteMessagesContext db, Conversation convo)
    {
      if (convo == null)
      {
        CreateResult result = CreateResult.None;
        convo = db.GetConversation("status@broadcast", CreateOptions.CreateAndSubmitIfNotFound, out result, false);
      }
      DateTime? start = PerformanceTimer.Start(PerformanceTimer.Mode.DebugAndBeta);
      UserStatus[] allContacts = (UserStatus[]) null;
      Dictionary<string, bool> blocked = (Dictionary<string, bool>) null;
      string[] array;
      switch (Settings.StatusV3PrivacySetting)
      {
        case WaStatusHelper.StatusPrivacySettings.WhiteList:
          Dictionary<string, GroupParticipantState> whiteList = WaStatusHelper.GetWhiteList(db);
          ContactsContext.Instance((Action<ContactsContext>) (cdb => blocked = cdb.BlockListSet));
          Log.d("statusv3", "update recipients | white list | white:{0},blocked:{1}", (object) whiteList.Count, (object) blocked.Count);
          array = whiteList.Keys.Where<string>((Func<string, bool>) (jid => !blocked.ContainsKey(jid))).ToArray<string>();
          break;
        case WaStatusHelper.StatusPrivacySettings.BlackList:
          Dictionary<string, GroupParticipantState> excludeSet = WaStatusHelper.GetBlackList(db);
          int count = excludeSet.Count;
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            allContacts = cdb.GetWaContacts(false);
            blocked = cdb.BlockListSet;
          }));
          foreach (KeyValuePair<string, bool> keyValuePair in blocked)
            excludeSet[keyValuePair.Key] = (GroupParticipantState) null;
          Log.d("statusv3", "update recipients | black list | black list:{0},blocked:{1}", (object) count, (object) blocked.Count);
          array = ((IEnumerable<UserStatus>) allContacts).Where<UserStatus>((Func<UserStatus, bool>) (u => !excludeSet.ContainsKey(u.Jid))).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
          break;
        default:
          ContactsContext.Instance((Action<ContactsContext>) (cdb =>
          {
            allContacts = cdb.GetWaContacts(false);
            blocked = cdb.BlockListSet;
          }));
          Log.d("statusv3", "update recipients | contacts | all:{0},blocked:{1}", (object) allContacts.Length, (object) blocked.Count);
          array = ((IEnumerable<UserStatus>) allContacts).Where<UserStatus>((Func<UserStatus, bool>) (u => !blocked.ContainsKey(u.Jid))).Select<UserStatus, string>((Func<UserStatus, string>) (u => u.Jid)).ToArray<string>();
          break;
      }
      Log.d("statusv3", "updated recipients: {0}", (object) array.Length);
      convo.UpdateParticipants(db, array);
      PerformanceTimer.End("update status v3 recipients", start);
      Settings.StatusRecipientsStateDirty = false;
    }

    public static Conversation GetStatusConversation(SqliteMessagesContext db)
    {
      bool flag = Settings.StatusRecipientsStateDirty;
      if (flag)
        Log.d("statusv3", "status recipients dirty");
      CreateResult result = CreateResult.None;
      Conversation conversation = db.GetConversation("status@broadcast", CreateOptions.CreateAndSubmitIfNotFound, out result, false);
      if ((result & CreateResult.Created) != CreateResult.None)
      {
        Log.d("statusv3", "status recipients init needed");
        flag = true;
      }
      if (flag)
        WaStatusHelper.UpdateStatusRecipients(db, conversation);
      return conversation;
    }

    public static Dictionary<string, GroupParticipantState> GetWhiteList(SqliteMessagesContext db)
    {
      return db.GetParticipants("status_white", true);
    }

    public static Dictionary<string, GroupParticipantState> GetBlackList(SqliteMessagesContext db)
    {
      return db.GetParticipants("status_black", true);
    }

    public static int GetWhiteListCount(MessagesContext db)
    {
      return db.GetParticipantsCount("status_white", true);
    }

    public static int GetBlackListCount(MessagesContext db)
    {
      return db.GetParticipantsCount("status_black", true);
    }

    public static void SetWhiteList(
      MessagesContext db,
      string[] jids,
      bool syncRemote,
      bool relay = false)
    {
      GroupParticipants groupParticipants = new GroupParticipants(db, "status_white");
      if (!groupParticipants.Set(jids))
        return;
      groupParticipants.Flush();
      if (!syncRemote)
        return;
      WaStatusHelper.ScheduleSendStatusV3PrivacyListAction(db, WaStatusHelper.StatusPrivacySettings.WhiteList, jids, relay);
    }

    public static void SetBlackList(
      MessagesContext db,
      string[] jids,
      bool syncRemote,
      bool relay = false)
    {
      GroupParticipants groupParticipants = new GroupParticipants(db, "status_black");
      if (!groupParticipants.Set(jids))
        return;
      groupParticipants.Flush();
      if (!syncRemote)
        return;
      WaStatusHelper.ScheduleSendStatusV3PrivacyListAction(db, WaStatusHelper.StatusPrivacySettings.BlackList, jids, relay);
    }

    public static void SendCurrentPrivacySetting()
    {
      string[] jids = (string[]) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        switch (Settings.StatusV3PrivacySetting)
        {
          case WaStatusHelper.StatusPrivacySettings.Contacts:
            WaStatusHelper.ScheduleSendStatusV3PrivacyListAction(db, WaStatusHelper.StatusPrivacySettings.Contacts, new string[0]);
            break;
          case WaStatusHelper.StatusPrivacySettings.WhiteList:
            jids = WaStatusHelper.GetWhiteList((SqliteMessagesContext) db).Keys.ToArray<string>();
            WaStatusHelper.ScheduleSendStatusV3PrivacyListAction(db, WaStatusHelper.StatusPrivacySettings.WhiteList, jids);
            break;
          case WaStatusHelper.StatusPrivacySettings.BlackList:
            jids = WaStatusHelper.GetBlackList((SqliteMessagesContext) db).Keys.ToArray<string>();
            WaStatusHelper.ScheduleSendStatusV3PrivacyListAction(db, WaStatusHelper.StatusPrivacySettings.BlackList, jids);
            break;
        }
      }));
    }

    public static void InitPrivacySettings(
      Dictionary<WaStatusHelper.StatusPrivacySettings, string[]> privacyLists,
      WaStatusHelper.StatusPrivacySettings defaultSetting)
    {
      if (Settings.StatusV3PrivacySetting != WaStatusHelper.StatusPrivacySettings.Undefined)
        Log.l("statusv3", "ignore remote privacy settings | already initiated locally");
      else
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          if (privacyLists != null)
          {
            foreach (KeyValuePair<WaStatusHelper.StatusPrivacySettings, string[]> privacyList in privacyLists)
            {
              switch (privacyList.Key)
              {
                case WaStatusHelper.StatusPrivacySettings.WhiteList:
                  WaStatusHelper.SetWhiteList(db, privacyList.Value, false);
                  continue;
                case WaStatusHelper.StatusPrivacySettings.BlackList:
                  WaStatusHelper.SetBlackList(db, privacyList.Value, false);
                  continue;
                default:
                  continue;
              }
            }
          }
          Settings.StatusV3PrivacySetting = defaultSetting;
          Log.l("statusv3", "privacy lists synced to local");
        }));
    }

    public static WaScheduledTask CreatePurgeStatusesTask()
    {
      return new WaScheduledTask(WaScheduledTask.Types.PurgeStatuses, "status@broadcast", (byte[]) null, WaScheduledTask.Restrictions.None, new TimeSpan?());
    }

    public static IObservable<Unit> PerformPurgeStatuses()
    {
      int batchCount = 10;
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Log.l("statusv3", "status purge | started");
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          DateTime beforeUtc = FunRunner.CurrentServerTimeUtc - WaStatus.Expiration;
          Message[] messageArray = db.GetMessagesBefore("status@broadcast", new int?(), beforeUtc, new int?(batchCount), new int?(), asc: true);
          if (Settings.IsStatusPSAUnseen)
          {
            Log.d("statusv3", "status purge | exclude psa statuses");
            messageArray = ((IEnumerable<Message>) messageArray).Where<Message>((Func<Message, bool>) (m => !JidHelper.IsPsaJid(m.GetSenderJid()))).ToArray<Message>();
          }
          if (((IEnumerable<Message>) messageArray).Any<Message>())
          {
            Log.l("statusv3", "status purge | deleting {0}", (object) messageArray.Length);
            db.DeleteMessages(messageArray);
          }
          bool flag = messageArray.Length < batchCount;
          observer.OnCompleted();
          Log.l("statusv3", "status purge | deleted: {0}", (object) messageArray.Length);
          if (!((IEnumerable<Message>) messageArray).Any<Message>())
            return;
          if (!((IEnumerable<WaStatus>) db.GetExpiredStatuses((string[]) null, new string[1]
          {
            "0@s.whatsapp.net"
          }, new int?(1))).Any<WaStatus>())
            return;
          if (flag)
          {
            WaStatus[] expiredStatuses = db.GetExpiredStatuses((string[]) null, new string[1]
            {
              "0@s.whatsapp.net"
            }, new int?());
            Message[] array = ((IEnumerable<WaStatus>) expiredStatuses).Select<WaStatus, Message>((Func<WaStatus, Message>) (s => db.GetMessageById(s.MessageId))).Where<Message>((Func<Message, bool>) (m => m != null)).ToArray<Message>();
            Log.l("statusv3", "status purge | expired statuses left: {0}, expired msgs left: {1}", (object) expiredStatuses.Length, (object) array.Length);
            db.DeleteMessages(array);
            foreach (WaStatus s in expiredStatuses)
              db.DeleteStatusOnSubmit(s);
            db.SubmitChanges();
          }
          else
          {
            Log.l("statusv3", "status purge | schedule another attempt");
            WaScheduledTask task = ((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
            {
              WaScheduledTask.Types.PurgeStatuses
            }, excludeExpired: false, lookupKey: "status@broadcast")).FirstOrDefault<WaScheduledTask>();
            if (task == null)
              return;
            db.AttemptScheduledTaskOnThreadPool(task, 2000, true);
          }
        }));
        return (Action) (() => { });
      }));
    }

    public static void ScheduleSendStatusV3PrivacyListAction(
      MessagesContext db,
      WaStatusHelper.StatusPrivacySettings listType,
      string[] jids,
      bool relay = false)
    {
      foreach (PersistentAction persistentAction in db.GetPersistentActions(PersistentAction.Types.SendStatusV3PrivacyList))
      {
        if ((WaStatusHelper.StatusPrivacySettings) new BinaryData(persistentAction.ActionData).ReadInt32(0) == listType)
          db.DeletePersistentActionOnSubmit(persistentAction);
      }
      BinaryData binaryData = new BinaryData();
      binaryData.AppendInt32(relay ? 1 : 0);
      binaryData.AppendInt32((int) listType);
      binaryData.AppendInt32(jids.Length);
      foreach (string jid in jids)
        binaryData.AppendStrWithLengthPrefix(jid);
      PersistentAction pa = new PersistentAction()
      {
        ActionType = 32,
        Jid = "status@broadcast",
        ActionData = binaryData.Get(),
        ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(7.0))
      };
      db.StorePersistentAction(pa);
      db.SubmitChanges();
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }

    public static IObservable<Unit> PerformSendStatusV3PrivacyList(
      FunXMPP.Connection conn,
      PersistentAction pa)
    {
      return pa.ActionData == null ? Observable.Return<Unit>(new Unit()) : Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        BinaryData binaryData = new BinaryData(pa.ActionData);
        int offset1 = 0;
        bool relay = binaryData.ReadInt32(offset1) == 1;
        int offset2 = offset1 + 4;
        WaStatusHelper.StatusPrivacySettings listType = (WaStatusHelper.StatusPrivacySettings) binaryData.ReadInt32(offset2);
        int offset3 = offset2 + 4;
        int num = binaryData.ReadInt32(offset3);
        int newOffset = offset3 + 4;
        List<string> jids = new List<string>();
        for (int index = 0; index < num; ++index)
        {
          string jid = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
          if (JidHelper.IsUserJid(jid))
            jids.Add(jid);
        }
        string strForList = WaStatusHelper.GetStrForList(listType);
        Log.d("statusv3", "sync statuses privacy list to server | t:{0},n:{1}", (object) strForList, (object) num);
        conn.SendSetStatusV3PrivacyList(strForList, jids, (Action) (() => observer.OnNext(new Unit())), (Action) (() => observer.OnCompleted()), relay);
        return (Action) (() => { });
      }));
    }

    public static uint ColorToUint(Color c)
    {
      return (uint) (((int) c.A << 24) + ((int) c.R << 16) + ((int) c.G << 8)) + (uint) c.B;
    }

    public static Color CreateTextStatusBackgroundColor(double r, double g, double b)
    {
      return Color.FromArgb(byte.MaxValue, (byte) (r * (double) byte.MaxValue), (byte) (g * (double) byte.MaxValue), (byte) (b * (double) byte.MaxValue));
    }

    public static Color GetRandomTextStatusBackgroundColor()
    {
      List<Color> backgroundColors = WaStatusHelper.GetTextStatusBackgroundColors();
      int index = new Random((int) DateTime.UtcNow.ToUnixTime()).Next(0, backgroundColors.Count - 1);
      return backgroundColors[index];
    }

    public static List<Color> GetTextStatusBackgroundColors(bool randomStart = false)
    {
      List<Color> source = new List<Color>()
      {
        WaStatusHelper.CreateTextStatusBackgroundColor(0.65, 0.17, 0.44),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.56, 0.66, 0.25),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.76, 0.63, 0.25),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.47, 0.13, 0.22),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.68, 0.53, 0.45),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.94, 0.7, 0.19),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.71, 0.7, 0.15),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.78, 0.62, 0.8),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.55, 0.41, 0.56),
        WaStatusHelper.CreateTextStatusBackgroundColor(1.0, 0.54, 0.55),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.33, 0.76, 0.4),
        WaStatusHelper.CreateTextStatusBackgroundColor(1.0, 0.48, 0.42),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.15, 0.77, 0.86),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.34, 0.79, 1.0),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.45, 0.4, 0.42),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.49, 0.56, 0.64),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.34, 0.59, 1.0),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.43, 0.15, 0.49),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.48, 0.8, 0.65),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.14, 0.21, 0.25),
        WaStatusHelper.CreateTextStatusBackgroundColor(0.51, 0.58, 0.79)
      };
      if (randomStart)
      {
        int count = new Random((int) DateTime.UtcNow.ToUnixTime()).Next(0, source.Count - 1);
        if (count > 0)
          source = source.Skip<Color>(count).Concat<Color>(source.Take<Color>(count)).ToList<Color>();
      }
      return source;
    }

    public static List<WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType> GetTextStatusFonts()
    {
      return new List<WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType>()
      {
        WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.SANS_SERIF,
        WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.NORICAN_REGULAR,
        WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.BRYNDAN_WRITE,
        WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType.OSWALD_HEAVY
      };
    }

    public static void PostTextStatus(
      string text,
      Color bgColor,
      WhatsApp.ProtoBuf.Message.ExtendedTextMessage.FontType font,
      WebPageMetadata linkPreviewData = null)
    {
      if (linkPreviewData != null && text.IndexOf(linkPreviewData.MatchedText) < 0)
        linkPreviewData = (WebPageMetadata) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Message message = new Message(true)
        {
          KeyFromMe = true,
          KeyRemoteJid = "status@broadcast",
          KeyId = FunXMPP.GenerateMessageId(),
          Data = text,
          Status = FunXMPP.FMessage.Status.Unsent,
          MediaWaType = FunXMPP.FMessage.Type.ExtendedText
        };
        if (linkPreviewData != null)
        {
          UriMessageWrapper uriMessageWrapper = new UriMessageWrapper(message)
          {
            Title = linkPreviewData.Title,
            Description = linkPreviewData.Description,
            CanonicalUrl = linkPreviewData.CanonicalUrl,
            MatchedText = linkPreviewData.MatchedText
          };
          message.BinaryData = linkPreviewData.ThumbnailBytes;
        }
        MessageProperties forMessage = MessageProperties.GetForMessage(message);
        MessageProperties.ExtendedTextProperties extendedTextProperties = forMessage.EnsureExtendedTextProperties;
        extendedTextProperties.Font = new int?((int) font);
        extendedTextProperties.BackgroundArgb = new uint?(WaStatusHelper.ColorToUint(bgColor));
        forMessage.Save();
        db.InsertMessageOnSubmit(message);
        db.SubmitChanges();
      }));
    }

    public static Color GetLinkBackgroundColor(Color textStatusBackgroundColor)
    {
      double num = 0.2;
      return Color.FromArgb(byte.MaxValue, (byte) (int) (num * (double) byte.MaxValue + (1.0 - num) * (double) textStatusBackgroundColor.R), (byte) (int) (num * (double) byte.MaxValue + (1.0 - num) * (double) textStatusBackgroundColor.G), (byte) (int) (num * (double) byte.MaxValue + (1.0 - num) * (double) textStatusBackgroundColor.B));
    }

    public enum StatusPrivacySettings
    {
      Contacts = 0,
      WhiteList = 1,
      BlackList = 2,
      Undefined = 99, // 0x00000063
    }
  }
}
