// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteMessagesContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WhatsApp.CommonOps;
using WhatsApp.Events;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class SqliteMessagesContext : SqliteDataContext
  {
    private const string LogHeader = "msgdb";
    private Dictionary<string, Conversation> convoCache = new Dictionary<string, Conversation>();
    private CachedStatement getConvoStmt = new CachedStatement("SELECT * FROM Conversations WHERE Jid = ?");
    private static long MAX_MEDIA_MESSAGE_TICKS = 4000000;
    private static long MIN_MEDIA_MESSAGE_TICKS = 2000000;
    private CachedStatement getBgMessagesStmt = new CachedStatement("SELECT * FROM MessageMiscInfos WHERE BackgroundId IS NOT NULL");
    private CachedStatement getMessagesWithMediaUrlStmt = new CachedStatement("SELECT * FROM Messages WHERE MediaUrl = ?");
    private CachedStatement getMsgByIdStmt = new CachedStatement("SELECT * FROM Messages WHERE MessageID = ?");
    private CachedStatement unsentMessageStmt = new CachedStatement(string.Format("SELECT * FROM Messages WHERE ({0})", (object) string.Join(" OR ", ((IEnumerable<FunXMPP.FMessage.Status>) new FunXMPP.FMessage.Status[5]
    {
      FunXMPP.FMessage.Status.Unsent,
      FunXMPP.FMessage.Status.Uploading,
      FunXMPP.FMessage.Status.UploadingCustomHash,
      FunXMPP.FMessage.Status.Relay,
      FunXMPP.FMessage.Status.Pending
    }).Select<FunXMPP.FMessage.Status, string>((Func<FunXMPP.FMessage.Status, string>) (@enum => string.Format("Status = {0}", (object) (int) @enum))))));
    private CachedStatement dedupStmt = new CachedStatement("SELECT EXISTS(SELECT * FROM Messages WHERE KeyRemoteJid = ? AND KeyId = ? AND KeyFromMe = ?)");
    private CachedStatement getMsgStmt = new CachedStatement("SELECT * FROM Messages WHERE KeyRemoteJid = ? AND KeyId = ? AND KeyFromMe = ?");
    private CachedStatement getMsgMiscInfoStmt = new CachedStatement("SELECT * FROM MessageMiscInfos WHERE MessageId = ?");
    private Dictionary<string, LocalFile> localFilesDict = new Dictionary<string, LocalFile>();
    private CachedStatement localFileStmt = new CachedStatement("SELECT * FROM LocalFiles Where LocalFileUri = ? COLLATE NOCASE");
    private static WorkQueue hashWorkerThread;
    private CachedStatement allConvoJidsStmt = new CachedStatement("SELECT Jid FROM Conversations");
    private CachedStatement nonemptyCovnosStmt = new CachedStatement("SELECT * FROM Conversations WHERE LastMessageID IS NOT NULL");
    private CachedStatement pinnedConvosStmt = new CachedStatement("SELECT * FROM Conversations WHERE SortKey IS NOT NULL");
    private CachedStatement toDeleteConvosStmt = new CachedStatement(string.Format("SELECT * FROM Conversations WHERE Status IS {0} OR Status IS {1}", (object) 1, (object) 2));
    public static bool throttleReleasePendingClbUpload = false;
    private CachedStatement getPersistActionsNotEqualStmt = new CachedStatement("SELECT * FROM PersistentActions WHERE ActionType != ? ORDER BY ActionID ASC");
    private CachedStatement getPersistActionsEqualStmt = new CachedStatement("SELECT * FROM PersistentActions WHERE ActionType = ? ORDER BY ActionID ASC");
    private CachedStatement getPersistActionsByType = new CachedStatement("SELECT * FROM PersistentActions WHERE ActionType = ?");
    private CachedStatement getPersistActionsByTypeAndDataStmt = new CachedStatement("SELECT * FROM PersistentActions WHERE ActionType = ? AND ActionDataString = ?");
    private CachedStatement msgsByKeyIdStmt = new CachedStatement("SELECT * FROM Messages\nWHERE KeyFromMe = ?\nAND KeyId = ?");
    private Dictionary<string, EmojiUsage> cachedEmojiUsages_;
    private Dictionary<string, EmojiSelectedIndex> cachedEmojiSelectedIndexes_;
    private Dictionary<byte[], Sticker> cachedStickers;
    private CachedStatement getStickerByHashStmt = new CachedStatement("SELECT * FROM Stickers WHERE FileHash = ?");

    public Table<Message> Messages { get; set; }

    public Table<MessageMiscInfo> MessageMiscInfos { get; set; }

    public Table<Conversation> Conversations { get; set; }

    public Table<LocalFile> LocalFiles { get; set; }

    public Table<PersistentAction> PersistentActions { get; set; }

    public Table<EmojiUsage> EmojiUsages { get; set; }

    public Table<WhatsApp.ReceiptState> ReceiptState { get; set; }

    public Table<GroupParticipantState> GroupParticipants { get; set; }

    public Table<PostponedReceipt> PostponedReceipts { get; set; }

    public Table<CipherTextReceipt> CipherTextReceipts { get; set; }

    public Table<WaScheduledTask> WaScheduledTasks { get; set; }

    public Table<EmojiSelectedIndex> EmojiSelectedIndexes { get; set; }

    public Table<ParticipantsHashHistory> ParticipantsHashJournal { get; set; }

    public Table<JidInfo> JidInfos { get; set; }

    public Table<VCard> ContactVCards { get; set; }

    public Table<FrequentChatScore> FrequentChatScores { get; set; }

    public Table<WaStatus> WaStatuses { get; set; }

    public Table<PendingMessage> PendingMessages { get; set; }

    public Table<Sticker> Stickers { get; set; }

    public Subject<Sticker> SavedStickerChangedSubject
    {
      get => MessagesContext.Events.SavedStickerChangedSubject;
    }

    public Subject<Message> NewMessagesSubject => MessagesContext.Events.NewMessagesSubject;

    public Subject<DbDataUpdate> MessageUpdateSubject
    {
      get => MessagesContext.Events.MessageUpdateSubject;
    }

    public Subject<Message> UpdatedMessageMediaWaTypeSubject
    {
      get => MessagesContext.Events.UpdatedMessagesMediaWaTypeSubject;
    }

    public Subject<Message> DeletedMessagesSubject => MessagesContext.Events.DeletedMessagesSubject;

    public Subject<ConvoAndMessage> UpdatedConversationSubject
    {
      get => MessagesContext.Events.UpdatedConversationSubject;
    }

    public Subject<Conversation> DeletedConversationSubject
    {
      get => MessagesContext.Events.DeletedConversationSubject;
    }

    public Subject<WhatsApp.ReceiptState> NewReceiptSubject
    {
      get => MessagesContext.Events.NewReceiptSubject;
    }

    public SqliteMessagesContext(string filename = "messages.db", SqliteSynchronizeOptions? syncMode = null)
      : base(filename, sync: syncMode)
    {
      this.IsLockHeld = (Func<bool>) (() => MessagesContext.Mutex.IsOwner());
      this.LatestSchemaVersion = 93;
      if (!this.DatabaseExists())
        return;
      int schemaVersion = this.GetSchemaVersion();
      bool flag = this.TableExists("MessagesFts");
      if (schemaVersion >= this.LatestSchemaVersion && flag)
        return;
      this.BeginTransaction();
      try
      {
        bool shouldSubmit = false;
        if (schemaVersion < this.LatestSchemaVersion)
        {
          this.UpdateSchema(schemaVersion, ref shouldSubmit);
          this.SetSchemaVersion(this.LatestSchemaVersion);
        }
        if (!flag)
          this.CreateFtsTableImpl();
        if (shouldSubmit)
          this.SubmitChanges(false);
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        Log.d(ex, "schema update");
        this.Dispose();
        throw;
      }
    }

    protected override void OnDbOpened(Sqlite db) => db.RegisterTokenizer();

    private void UpdateSchema(int schema, ref bool shouldSubmit)
    {
      if (schema < 1)
      {
        this.AddTable("Broadcasts");
        this.AddTable("BroadcastMessages");
        this.AddColumn("Messages", "BroadcastMessageID");
      }
      if (schema < 2)
        this.AddColumn("Broadcasts", "Status");
      if (schema < 3)
        this.AddTable("EmojiUsages");
      if (schema < 4)
        this.AddColumn("Messages", "MediaOrigin");
      if (schema < 5)
      {
        this.AddColumn("PersistentActions", "MaxFailureCount");
        this.AddColumn("PersistentActions", "ExpirationTime");
      }
      if (schema < 6)
        this.AddTable("MessageMiscInfos");
      if (schema < 7)
      {
        this.AddColumn("PersistentActions", "Attempts");
        this.AddColumn("PersistentActions", "AttemptsLimit");
        this.AddIndex("PersistentActions", "TypeAndData");
      }
      if (schema < 8)
      {
        using (Sqlite.PreparedStatement preparedStatement1 = this.Db.PrepareStatement("SELECT MessageID, BackgroundId FROM Messages WHERE BackgroundId IS NOT NULL"))
        {
          while (preparedStatement1.Step())
          {
            int column1 = (int) (long) preparedStatement1.Columns[0];
            string column2 = (string) preparedStatement1.Columns[1];
            using (Sqlite.PreparedStatement preparedStatement2 = this.Db.PrepareStatement("INSERT OR REPLACE INTO MessageMiscInfos (MessageId, BackgroundId) VALUES (?, ?)"))
            {
              preparedStatement2.Bind(0, column1, false);
              preparedStatement2.Bind(1, column2);
              preparedStatement2.Step();
            }
          }
        }
      }
      if (schema < 9)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("Update Conversations SET EffectiveFirstMessageID = null"))
          preparedStatement.Step();
      }
      if (schema < 10)
        this.AddColumn("MessageMiscInfos", "LargeThumbnailData");
      if (schema < 11)
      {
        this.AddColumn("MessageMiscInfos", "TargetFilename");
        this.AddColumn("MessageMiscInfos", "AlternateUploadUri");
      }
      if (schema < 13)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM Conversations WHERE Jid = 'broadcast'"))
          preparedStatement.Step();
      }
      if (schema < 13)
      {
        List<string> stringList = new List<string>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT Jid FROM Conversations"))
        {
          while (preparedStatement.Step())
          {
            if (preparedStatement.Columns[0] is string column && column.IsBroadcastJid())
              stringList.Add(column);
          }
        }
        using (Sqlite.PreparedStatement preparedStatement3 = this.Db.PrepareStatement("SELECT RemoteResource FROM Messages WHERE KeyRemoteJid = ?"))
        {
          foreach (string val in stringList)
          {
            preparedStatement3.Bind(0, val);
            if (preparedStatement3.Step())
            {
              string column = preparedStatement3.Columns[0] as string;
              using (Sqlite.PreparedStatement preparedStatement4 = this.Db.PrepareStatement("UPDATE Messages SET KeyRemoteJid = ?, RemoteResource = ? WHERE KeyRemoteJid = ?"))
              {
                preparedStatement4.Bind(0, column);
                preparedStatement4.Bind(1, val);
                preparedStatement4.Bind(2, val);
                preparedStatement4.Step();
              }
              using (Sqlite.PreparedStatement preparedStatement5 = this.Db.PrepareStatement("DELETE FROM Conversations WHERE Jid = ?"))
              {
                preparedStatement5.Bind(0, val);
                preparedStatement5.Step();
              }
            }
            preparedStatement3.Reset();
          }
        }
      }
      if (schema < 14)
      {
        this.AddColumn("MessageMiscInfos", "Recipients");
        this.AddColumn("MessageMiscInfos", "DeliveredRecipients");
        this.AddColumn("MessageMiscInfos", "BroadcastJid");
      }
      if (schema < 15)
        this.AddColumn("MessageMiscInfos", "ExpectedDeliveryCount");
      if (schema < 16)
        this.AddColumn("MessageMiscInfos", "PictureWidthHeightRatio");
      if (schema < 17)
        this.AddIndex("Messages", "LoadPerformanceIndex");
      if (schema < 18)
        this.AddColumn("Conversations", "AutomuteTimer");
      if (schema < 19)
        this.AddColumn("Messages", "TextPerformanceHint");
      if (schema < 20)
        this.AddColumn("Messages", "TextSplittingHint");
      if (schema < 21)
        this.AddColumn("Conversations", "Wallpaper");
      if (schema < 22)
        this.AddTable("ReceiptState");
      if (schema < 23)
      {
        this.AddColumn("Conversations", "FirstUnreadMessageID");
        using (Sqlite.PreparedStatement preparedStatement6 = this.Db.PrepareStatement("SELECT Jid, UnreadMessageCount FROM Conversations\nWHERE UnreadMessageCount IS NOT NULL AND UnreadMessageCount > 0"))
        {
          while (preparedStatement6.Step())
          {
            string column3 = (string) preparedStatement6.Columns[0];
            long column4 = (long) preparedStatement6.Columns[1];
            long val = 0;
            using (Sqlite.PreparedStatement preparedStatement7 = this.Db.PrepareStatement("SELECT MessageID FROM Messages\nWHERE KeyRemoteJid = ? AND KeyFromMe = 1 AND MediaWaType IS NOT ?\nORDER BY MessageID DESC\nLIMIT 1 OFFSET ?"))
            {
              preparedStatement7.Bind(0, column3);
              preparedStatement7.Bind(1, 7, false);
              preparedStatement7.Bind(2, column4 - 1L, false);
              if (preparedStatement7.Step())
                val = (long) preparedStatement7.Columns[0];
              else
                continue;
            }
            using (Sqlite.PreparedStatement preparedStatement8 = this.Db.PrepareStatement("UPDATE Conversations\nSET FirstUnreadMessageID = ?\nWHERE Jid = ?"))
            {
              preparedStatement8.Bind(0, val, false);
              preparedStatement8.Bind(1, column3);
              preparedStatement8.Step();
            }
          }
        }
      }
      if (schema < 24)
      {
        string[] strArray = new string[3]
        {
          "Jid",
          "FromMe",
          "Id"
        };
        foreach (string columnName in strArray)
          this.AddColumn("PersistentActions", columnName);
        using (Sqlite.PreparedStatement preparedStatement9 = this.Db.PrepareStatement("SELECT ActionID, ActionType, ActionDataString FROM PersistentActions\nWHERE ActionType = ? OR ActionType = ?"))
        {
          preparedStatement9.Bind(0, 5, false);
          preparedStatement9.Bind(1, 4, false);
          while (preparedStatement9.Step())
          {
            long column5 = (long) preparedStatement9.Columns[0];
            int column6 = (int) (long) preparedStatement9.Columns[1];
            string column7 = (string) preparedStatement9.Columns[2];
            int result = 0;
            string val1 = (string) null;
            bool flag = false;
            string val2 = (string) null;
            if (column7 != null && int.TryParse(column7, out result))
            {
              using (Sqlite.PreparedStatement preparedStatement10 = this.Db.PrepareStatement("SELECT KeyRemoteJid, KeyFromMe, KeyRemoteJid FROM Messages\nWHERE MessageID = ?"))
              {
                preparedStatement10.Bind(0, (long) result, false);
                if (preparedStatement10.Step())
                {
                  val1 = (string) preparedStatement10.Columns[0];
                  flag = (long) preparedStatement10.Columns[1] != 0L;
                  val2 = (string) preparedStatement10.Columns[2];
                }
                else
                  continue;
              }
              using (Sqlite.PreparedStatement preparedStatement11 = this.Db.PrepareStatement("UPDATE PersistentActions\nSET Jid = ?, FromMe = ?, Id = ?\nWHERE ActionID = ?"))
              {
                preparedStatement11.Bind(0, val1);
                preparedStatement11.Bind(1, flag ? 1L : 0L, false);
                preparedStatement11.Bind(2, val2);
                preparedStatement11.Bind(3, column5, false);
                preparedStatement11.Step();
              }
            }
          }
        }
      }
      if (schema < 25)
        this.AddColumn("Messages", "MediaCaption");
      if (schema < 26)
        this.AddColumn("MessageMiscInfos", "ImageBinaryInfo");
      if (schema < 27)
      {
        this.AddTable("GroupParticipants");
        using (Sqlite.PreparedStatement preparedStatement12 = this.Db.PrepareStatement("SELECT Jid, GroupParticipants, GroupOwner FROM Conversations\nWHERE GroupParticipants IS NOT NULL"))
        {
          char[] chArray = new char[1]{ ' ' };
          while (preparedStatement12.Step())
          {
            string column8 = (string) preparedStatement12.Columns[0];
            string column9 = (string) preparedStatement12.Columns[1];
            string column10 = (string) preparedStatement12.Columns[2];
            if (column9.Length != 0)
            {
              foreach (string val in ((IEnumerable<string>) column9.Split(chArray)).MakeUnique<string>())
              {
                using (Sqlite.PreparedStatement preparedStatement13 = this.Db.PrepareStatement("INSERT INTO GroupParticipants (GroupJid, MemberJid, Flags) VALUES (?, ?, ?)"))
                {
                  preparedStatement13.Bind(0, column8);
                  preparedStatement13.Bind(1, val);
                  preparedStatement13.Bind(2, column10 == val ? 1L : 0L, false);
                  preparedStatement13.Step();
                }
              }
            }
          }
        }
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET GroupParticipants = NULL"))
          preparedStatement.Step();
      }
      if (schema < 28)
        this.AddColumn("Conversations", "Flags");
      if (schema < 29)
      {
        this.AddColumn("Conversations", "SortKey");
        long num = 100;
        using (Sqlite.PreparedStatement preparedStatement14 = this.Db.PrepareStatement("SELECT Jid FROM Conversations ORDER BY Timestamp ASC"))
        {
          while (preparedStatement14.Step())
          {
            string column = (string) preparedStatement14.Columns[0];
            using (Sqlite.PreparedStatement preparedStatement15 = this.Db.PrepareStatement("UPDATE Conversations SET SortKey = ? WHERE Jid = ?"))
            {
              preparedStatement15.Bind(0, num++, false);
              preparedStatement15.Bind(1, column);
              preparedStatement15.Step();
            }
          }
        }
      }
      if (schema < 30)
        this.AddColumn("Conversations", "ModifyTag");
      if (schema < 31)
        this.AddColumn("Conversations", "UnreadTileCount");
      if (schema < 32)
        this.AddTable("PostponedReceipts");
      if (schema < 33)
        this.AddTable("WaScheduledTasks");
      if (schema < 34)
        this.AddColumn("MessageMiscInfos", "CipherRetryCount");
      if (schema < 35)
      {
        this.AddTable("EmojiSelectedIndexes");
        foreach (KeyValuePair<string, EmojiUsage> cachedEmojiUsage in this.CachedEmojiUsages)
        {
          string actualCodepoint;
          if (!string.IsNullOrEmpty(cachedEmojiUsage.Key) && Emoji.TryConvertSoftbankCodepoint(cachedEmojiUsage.Key[0], out actualCodepoint))
          {
            EmojiUsage emojiUsage;
            if (!this.CachedEmojiUsages.TryGetValue(actualCodepoint, out emojiUsage))
            {
              using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("INSERT INTO EmojiUsages (EmojiCode, UsageWeight) VALUES (?, ?)"))
              {
                preparedStatement.Bind(0, actualCodepoint);
                preparedStatement.Bind(1, cachedEmojiUsage.Value.UsageWeight, false);
                preparedStatement.Step();
              }
            }
            else
              emojiUsage.UsageWeight += cachedEmojiUsage.Value.UsageWeight;
            using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM EmojiUsages WHERE EmojiCode = ?"))
            {
              preparedStatement.Bind(0, cachedEmojiUsage.Key);
              preparedStatement.Step();
            }
          }
        }
        this.cachedEmojiUsages_ = (Dictionary<string, EmojiUsage>) null;
      }
      if (schema < 36)
      {
        this.AddColumn("Messages", "FtsStatus");
        this.AddIndex("Messages", "FtsStatusIndex");
      }
      if (schema < 37)
      {
        this.AddColumn("Conversations", "ParticipantsHash");
        this.AddColumn("Messages", "ParticipantsHash");
        this.AddTable("ParticipantsHashJournal");
      }
      if (schema < 39)
      {
        this.AddTable("JidInfos");
        using (Sqlite.PreparedStatement preparedStatement16 = this.Db.PrepareStatement("SELECT Jid, MuteExpiration FROM Conversations\nWHERE MuteExpiration IS NOT NULL"))
        {
          while (preparedStatement16.Step())
          {
            string column11 = (string) preparedStatement16.Columns[0];
            long column12 = (long) preparedStatement16.Columns[1];
            using (Sqlite.PreparedStatement preparedStatement17 = this.Db.PrepareStatement("INSERT INTO JidInfos (Jid, MuteExpirationUtc) VALUES (?, ?)"))
            {
              preparedStatement17.Bind(0, column11);
              preparedStatement17.Bind(1, column12, false);
              preparedStatement17.Step();
            }
          }
        }
      }
      if (schema < 40)
      {
        this.AddColumn("LocalFiles", "Sha1Hash");
        this.AddIndex("LocalFiles", "Sha1HashIndex");
      }
      if (schema < 42)
        this.AddColumn("JidInfos", "IsSuspicious");
      if (schema < 43)
        this.AddColumn("Conversations", "GroupSubjectPerformanceHint");
      if (schema < 47)
      {
        Set<string> set = new Set<string>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT Jid FROM JidInfos WHERE Jid LIKE ?"))
        {
          preparedStatement.Bind(0, "%@g.us");
          while (preparedStatement.Step())
            set.Add((string) preparedStatement.Columns[0]);
        }
        using (Sqlite.PreparedStatement preparedStatement18 = this.Db.PrepareStatement("SELECT Jid FROM Conversations WHERE Jid LIKE ?"))
        {
          preparedStatement18.Bind(0, "%@g.us");
          while (preparedStatement18.Step())
          {
            string column = (string) preparedStatement18.Columns[0];
            if (set.Contains(column))
            {
              using (Sqlite.PreparedStatement preparedStatement19 = this.Db.PrepareStatement("UPDATE JidInfos SET IsSuspicious = ? WHERE Jid = ?"))
              {
                preparedStatement19.Bind(0, false);
                preparedStatement19.Bind(1, column);
                preparedStatement19.Step();
              }
              Log.d("msgdb", "update | mark {0} as not suspicious", (object) column);
            }
            else
            {
              using (Sqlite.PreparedStatement preparedStatement20 = this.Db.PrepareStatement("INSERT INTO JidInfos (Jid, IsSuspicious) VALUES (?, ?)"))
              {
                preparedStatement20.Bind(0, column);
                preparedStatement20.Bind(1, false);
                preparedStatement20.Step();
              }
              Log.d("msgdb", "add | mark {0} as not suspicious", (object) column);
            }
          }
        }
      }
      if (schema < 48)
      {
        this.InsertWaScheduledTaskOnSubmit(IndexMessages.CreateScheduledTask());
        shouldSubmit = true;
      }
      if (schema < 49)
        this.AddColumn("Messages", "IsStarred");
      if (schema < 50)
        this.AddIndex("Messages", "IsStarredIndex");
      if (schema < 51)
        this.AddColumn("Messages", "MediaKey");
      if (schema < 52)
        this.AddColumn("MessageMiscInfos", "TranscoderData");
      if (schema < 53)
        this.AddColumn("MessageMiscInfos", "ClientRetryCount");
      if (schema < 54)
        this.AddColumn("Messages", "MediaIp");
      if (schema < 55)
        this.AddColumn("JidInfos", "SupportsFullEncryption");
      if (schema < 56)
        this.AddColumn("MessageMiscInfos", "CipherMediaHash");
      if (schema < 57)
        this.AddColumn("JidInfos", "SaveMediaToPhone");
      if (schema < 59)
      {
        this.AddColumn("LocalFiles", "FileType");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE LocalFiles SET FileType = " + (object) 0))
          preparedStatement.Step();
        using (Sqlite.PreparedStatement preparedStatement21 = this.Db.PrepareStatement("SELECT DataFileName FROM Messages WHERE DataFileName IS NOT NULL"))
        {
          while (preparedStatement21.Step())
          {
            if (preparedStatement21.Columns[0] is string column)
            {
              using (Sqlite.PreparedStatement preparedStatement22 = this.Db.PrepareStatement("UPDATE LocalFiles SET FileType = " + (object) 1 + " WHERE LocalFileUri = ?"))
              {
                preparedStatement22.Bind(0, column);
                preparedStatement22.Step();
              }
            }
          }
        }
      }
      if (schema < 60)
      {
        this.AddColumn("Messages", "Flags");
        this.AddIndex("Messages", "FlagsIndex");
      }
      if (schema < 62)
        this.AddTable("CipherTextReceipts");
      if (schema < 63)
        this.AddIndex("Messages", "MediaHashIndex");
      if (schema < 64)
        this.AddColumn("LocalFiles", "FileSize");
      if (schema < 65)
        this.AddColumn("Messages", "ProtoBuf");
      if (schema < 66)
        this.AddColumn("Messages", "InternalPropertiesProtobuf");
      if (schema < 67)
      {
        this.AddTable("ContactVCards");
        this.AddColumn("JidInfos", "PromptedVCards");
        AppState.SchedulePersistentAction(PersistentAction.ContactVCardsIndex());
      }
      if (schema < 68)
        this.AddTable("FrequentChatScores");
      if (schema < 69)
        this.ClearLocalFileHashes();
      if (schema < 70)
      {
        this.Insert("PersistentActions", (object) new PersistentAction()
        {
          ActionType = 27,
          ExpirationTime = new DateTime?()
        });
        shouldSubmit = true;
      }
      if (schema < 72)
        this.AddTable("WaStatuses");
      if (schema < 73)
        this.AddTable("PendingMessages");
      if (schema < 74)
        this.AddColumn("Conversations", "InternalPropertiesProtobuf");
      if (schema < 75)
      {
        this.AddColumn("JidInfos", "Wallpaper");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE JidInfos " + "SET Wallpaper = (SELECT Conversations.Wallpaper FROM Conversations WHERE Conversations.Jid = JidInfos.Jid) " + "WHERE EXISTS (SELECT Conversations.Jid FROM Conversations WHERE Conversations.Jid = JidInfos.Jid) "))
          preparedStatement.Step();
      }
      if (schema < 76)
      {
        this.AddColumn("Conversations", "JidType");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET JidType = ? WHERE Jid LIKE ?"))
        {
          KeyValuePair<JidHelper.JidTypes, string>[] keyValuePairArray = new KeyValuePair<JidHelper.JidTypes, string>[3]
          {
            new KeyValuePair<JidHelper.JidTypes, string>(JidHelper.JidTypes.User, "@s.whatsapp.net"),
            new KeyValuePair<JidHelper.JidTypes, string>(JidHelper.JidTypes.Group, "@g.us"),
            new KeyValuePair<JidHelper.JidTypes, string>(JidHelper.JidTypes.Broadcast, "@broadcast")
          };
          foreach (KeyValuePair<JidHelper.JidTypes, string> keyValuePair in keyValuePairArray)
          {
            preparedStatement.Bind(0, (int) keyValuePair.Key, false);
            preparedStatement.Bind(1, string.Format("%{0}", (object) keyValuePair.Value));
            preparedStatement.Step();
            preparedStatement.Reset();
          }
        }
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations Set JidType = ? WHERE Jid = ?"))
        {
          KeyValuePair<JidHelper.JidTypes, string>[] keyValuePairArray = new KeyValuePair<JidHelper.JidTypes, string>[2]
          {
            new KeyValuePair<JidHelper.JidTypes, string>(JidHelper.JidTypes.Psa, "0@s.whatsapp.net"),
            new KeyValuePair<JidHelper.JidTypes, string>(JidHelper.JidTypes.Status, "status@broadcast")
          };
          foreach (KeyValuePair<JidHelper.JidTypes, string> keyValuePair in keyValuePairArray)
          {
            preparedStatement.Bind(0, (int) keyValuePair.Key, false);
            preparedStatement.Bind(1, keyValuePair.Value);
            preparedStatement.Step();
            preparedStatement.Reset();
          }
        }
      }
      if (schema < 77)
        this.AddIndex("Conversations", "JidTypeIndex");
      if (schema < 78)
      {
        this.AddIndex("WaStatuses", "IsViewedIndex");
        this.AddIndex("WaStatuses", "JidIndex");
      }
      if (schema < 79)
      {
        Log.l("schema update", "adding local files ref counts");
        this.AddColumn("LocalFiles", "MsgRefCount");
        this.AddColumn("LocalFiles", "StatusRefCount");
        this.AddColumn("LocalFiles", "ThumbRefCount");
        long num1 = 0;
        using (Sqlite.PreparedStatement preparedStatement23 = this.Db.PrepareStatement("SELECT LocalFileUri, FileType, ReferenceCount FROM LocalFiles"))
        {
          while (preparedStatement23.Step())
          {
            if (preparedStatement23.Columns[0] != null)
            {
              ++num1;
              string column13 = preparedStatement23.Columns[0] as string;
              long num2 = (long) (preparedStatement23.Columns[1] ?? (object) 0L);
              long column14 = (long) preparedStatement23.Columns[2];
              string[] strArray = new string[2];
              string str;
              switch (num2)
              {
                case 1:
                  strArray[0] = "MsgRefCount";
                  strArray[1] = "StatusRefCount";
                  str = "ThumbRefCount";
                  break;
                case 2:
                  strArray[0] = "MsgRefCount";
                  str = "StatusRefCount";
                  strArray[1] = "ThumbRefCount";
                  break;
                default:
                  str = "MsgRefCount";
                  strArray[0] = "StatusRefCount";
                  strArray[1] = "ThumbRefCount";
                  break;
              }
              using (Sqlite.PreparedStatement preparedStatement24 = this.Db.PrepareStatement("UPDATE LocalFiles SET " + str + " = " + (object) column14 + ", " + strArray[0] + " = " + (object) 0 + ", " + strArray[1] + " = " + (object) 0 + "  WHERE LocalFileUri = ?"))
              {
                preparedStatement24.Bind(0, column13);
                preparedStatement24.Step();
              }
            }
          }
        }
        Log.l("update schema", "Updated {0} rows in LocalFiles", (object) num1);
      }
      if (schema < 80)
        this.AddIndex("WaStatuses", "MessageIdIndex");
      if (schema < 81)
        this.AddColumn("JidInfos", "IsStatusMuted");
      if (schema < 82)
        this.AddColumn("JidInfos", "StatusAutoDownloadQuota");
      if (schema < 83)
        this.FixMissingJidTypes();
      if (schema < 84)
      {
        this.AddColumn("Conversations", "IsArchived");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET IsArchived = 1, Status = ? WHERE Status IS ?"))
        {
          preparedStatement.Bind(0, 0, false);
          preparedStatement.Bind(1, 3, false);
          preparedStatement.Step();
        }
      }
      if (schema < 85)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET SortKey = 0"))
          preparedStatement.Step();
      }
      if (schema < 86)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET SortKey = NULL WHERE SortKey = 0"))
          preparedStatement.Step();
      }
      if (schema < 87)
      {
        this.AddColumn("Conversations", "GroupDescription");
        this.AddColumn("Conversations", "GroupDescriptionT");
        this.AddColumn("Conversations", "GroupDescriptionOwner");
        this.AddColumn("Conversations", "GroupDescriptionId");
      }
      if (schema < 88)
      {
        this.AddTable("Stickers");
        this.AddColumn("LocalFiles", "StickerRefCount");
      }
      if (schema < 89)
      {
        this.AddColumn("Messages", "QuotedMediaFileUri");
        this.AddColumn("LocalFiles", "QuotedMediaRefCount");
      }
      if (schema < 90)
      {
        this.AddColumn("Stickers", "DirectPath");
        this.AddColumn("Stickers", "FileLength");
      }
      if (schema < 91)
        this.AddColumn("Stickers", "UsageWeight");
      if (schema < 92)
      {
        string sql1 = "SELECT PendingMessagesId, KeyRemoteJid, Timestamp FROM PendingMessages";
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        int num3 = 0;
        int num4 = 0;
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql1))
        {
          while (preparedStatement.Step())
          {
            ++num3;
            string column = (string) preparedStatement.Columns[1];
            if (JidHelper.IsMultiParticipantsChatJid(column))
            {
              ++num4;
              string str = "null";
              if (preparedStatement.Columns[2] != null)
              {
                try
                {
                  str = DateTime.FromFileTimeUtc((long) preparedStatement.Columns[2]).ToString();
                }
                catch (Exception ex)
                {
                  str = "Exception: " + ex.GetFriendlyMessage();
                }
              }
              Log.l("msgdb", "Found pending message for {0} from {1}", (object) column, (object) str);
              dictionary[column] = true;
            }
          }
        }
        Log.l("update schema", "Removing {0} row(s) and {1} group(s) from Pending which contains {2} message(s)", (object) num4, (object) dictionary.Count, (object) num3);
        if (dictionary.Count > 0)
          Log.SendCrashLog((Exception) new DataMisalignedException("PendingMessage table contains group data"), "Removing Pending mesasges", logOnlyForRelease: true);
        string sql2 = "DELETE FROM PendingMessages WHERE KeyRemoteJid = ?";
        foreach (string key in dictionary.Keys)
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql2))
          {
            preparedStatement.Bind(0, key);
            preparedStatement.Step();
          }
        }
        this.AddColumn("PendingMessages", "RemoteResource");
      }
      if (schema >= 93)
        return;
      this.AddColumn("PendingMessages", "PendingMsgPropertiesProtobuf");
    }

    public void CreateFtsTable()
    {
      this.BeginTransaction();
      try
      {
        this.CreateFtsTableImpl();
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
    }

    protected override void CreateTableOverride() => this.CreateFtsTableImpl();

    private void CreateFtsTableImpl()
    {
      if (!this.TableExists("MessagesFts"))
      {
        Log.WriteLineDebug("msg db: creating FTS table");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE VIRTUAL TABLE MessagesFts USING fts4(content=\"Messages\", KeyRemoteJid, Data, MediaName, MediaCaption, LocationDetails, tokenize=wa_tokenizer)"))
        {
          preparedStatement.Step();
          Log.WriteLineDebug("msg db: FTS table created");
        }
      }
      if (this.GetTableMetadata("Messages", "trigger").Contains("MessagesFtsTriggerBD"))
        return;
      Log.WriteLineDebug("msg db: creating FTS BD trigger");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE TRIGGER MessagesFtsTriggerBD BEFORE DELETE ON Messages BEGIN DELETE FROM MessagesFts WHERE docid=old.rowid; END"))
      {
        preparedStatement.Step();
        Log.WriteLineDebug("msg db: FTS BD trigger created");
      }
    }

    public void DropFtsTable()
    {
      this.BeginTransaction();
      try
      {
        this.DropFtsTableImpl();
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
    }

    private void DropFtsTableImpl()
    {
      if (!this.Db.IsTokenizerRegistered())
        this.Db.RegisterTokenizer();
      if (this.GetTableMetadata("Messages", "trigger").Contains("MessagesFtsTriggerBD"))
      {
        Log.WriteLineDebug("msg db: dropping FTS BD trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TRIGGER MessagesFtsTriggerBD"))
        {
          preparedStatement.Step();
          Log.WriteLineDebug("msg db: FTS BD trigger dropped");
        }
      }
      if (!this.TableExists("MessagesFts"))
        return;
      Log.WriteLineDebug("msg db: dropping FTS table");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TABLE MessagesFts"))
      {
        preparedStatement.Step();
        Log.WriteLineDebug("msg db: FTS table dropped");
      }
    }

    public void ClearFtsStatus()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Messages SET FtsStatus = NULL"))
      {
        preparedStatement.Step();
        Log.WriteLineDebug("msg db: FTS status cleared");
      }
    }

    public Message[] QueryFtsTable(string query, int limit = 20)
    {
      string sql = "SELECT docid FROM MessagesFts LEFT JOIN Conversations ON MessagesFts.KeyRemoteJid = Conversations.Jid WHERE MessagesFts MATCH ? AND (EffectiveFirstMessageID IS NULL OR docid >= EffectiveFirstMessageID) ORDER BY docid DESC LIMIT ?";
      List<Message> messageList;
      try
      {
        List<int> intList = new List<int>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql))
        {
          preparedStatement.Bind(0, query);
          preparedStatement.Bind(1, limit, false);
          while (preparedStatement.Step())
            intList.Add((int) (long) preparedStatement.Columns[0]);
        }
        messageList = new List<Message>(intList.Count);
        foreach (int id in intList)
        {
          Message messageById = this.GetMessageById(id);
          if (messageById != null)
            messageList.Add(messageById);
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      return messageList.ToArray();
    }

    public MessageSearchResult[] QueryFtsTableWithOffsets(
      string query,
      int? offset,
      int? limit,
      string[] jids = null,
      bool includeStarred = true,
      bool includeNonStarred = true)
    {
      if (!includeStarred && !includeNonStarred)
        return new MessageSearchResult[0];
      bool flag1 = includeStarred && !includeNonStarred;
      bool flag2 = !includeStarred & includeNonStarred;
      LinkedList<string> linkedList1 = new LinkedList<string>();
      LinkedList<object> linkedList2 = new LinkedList<object>();
      StringBuilder stringBuilder1 = new StringBuilder("SELECT docid, offsets(MessagesFts) FROM MessagesFts \n");
      stringBuilder1.Append("INNER JOIN Messages ON (docid = Messages.MessageID) \n");
      stringBuilder1.Append("LEFT JOIN Conversations ON (MessagesFts.KeyRemoteJid = Conversations.Jid) \n");
      if (jids != null && ((IEnumerable<string>) jids).Any<string>())
      {
        if (jids.Length == 1)
        {
          linkedList1.AddLast("MessagesFts.KeyRemoteJid = ?");
          linkedList2.AddLast((object) jids[0]);
        }
        else if (jids.Length > 1)
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.Append("MessagesFts.KeyRemoteJid IN (");
          bool flag3 = true;
          foreach (string jid in jids)
          {
            if (flag3)
              flag3 = false;
            else
              stringBuilder2.Append(", ");
            stringBuilder2.Append("?");
            linkedList2.AddLast((object) jid);
          }
          stringBuilder2.Append(")");
          linkedList1.AddLast(stringBuilder2.ToString());
        }
      }
      linkedList1.AddLast("MessagesFts MATCH ?");
      linkedList2.AddLast((object) query);
      if (flag1)
        linkedList1.AddLast("Messages.IsStarred = 1");
      else if (flag2)
        linkedList1.AddLast("(Messages.IsStarred is NULL OR Messages.IsStarred = 0)");
      linkedList1.AddLast("(Conversations.EffectiveFirstMessageID IS NULL OR docid >= Conversations.EffectiveFirstMessageID)");
      linkedList1.AddLast("(Messages.MediaWaType <> ?)");
      linkedList2.AddLast((object) 1006);
      if (linkedList1.Any<string>())
      {
        stringBuilder1.Append("WHERE ");
        stringBuilder1.Append(string.Join("\n AND ", (IEnumerable<string>) linkedList1));
        stringBuilder1.Append("\n");
      }
      stringBuilder1.Append("ORDER BY docid DESC \n");
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder1.Append("LIMIT ? \n");
        linkedList2.AddLast((object) limit.Value);
      }
      if (offset.HasValue && offset.Value > 0)
      {
        stringBuilder1.Append("OFFSET ? \n");
        linkedList2.AddLast((object) offset.Value);
      }
      List<MessageSearchResult> messageSearchResultList;
      try
      {
        List<int> intList = new List<int>();
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder1.ToString()))
        {
          int num = 0;
          foreach (object o in linkedList2)
            preparedStatement.BindObject(num++, o);
          while (preparedStatement.Step())
          {
            int column = (int) (long) preparedStatement.Columns[0];
            intList.Add(column);
            dictionary.Add(column, (string) preparedStatement.Columns[1]);
          }
        }
        messageSearchResultList = new List<MessageSearchResult>(intList.Count);
        foreach (int num in intList)
        {
          Message messageById = this.GetMessageById(num);
          if (messageById != null)
          {
            if (JidChecker.CheckJidProtocolString(messageById.KeyRemoteJid))
            {
              MessageSearchResult messageSearchResult = this.BuildMessageSearchResult(messageById, dictionary[num]);
              messageSearchResultList.Add(messageSearchResult);
            }
            else
              JidChecker.MaybeSendJidErrorClb("Search", messageById.KeyRemoteJid);
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "fts query");
        throw;
      }
      return messageSearchResultList.ToArray();
    }

    private MessageSearchResult BuildMessageSearchResult(Message message, string offsets)
    {
      MessageSearchResult messageSearchResult = new MessageSearchResult(message);
      string[] strArray = (offsets ?? "").Split(' ');
      if (strArray.Length == 0 || strArray.Length % 4 != 0)
      {
        Log.p("msgdb", "invalid number of search offsets: {0} -> {1}", (object) strArray.Length, (object) offsets);
        return messageSearchResult;
      }
      Set<int> set1 = (Set<int>) null;
      Set<int> set2 = (Set<int>) null;
      Set<int> set3 = (Set<int>) null;
      Set<int> set4 = (Set<int>) null;
      List<Pair<int, int>> pairList1 = (List<Pair<int, int>>) null;
      List<Pair<int, int>> pairList2 = (List<Pair<int, int>>) null;
      List<Pair<int, int>> pairList3 = (List<Pair<int, int>>) null;
      List<Pair<int, int>> pairList4 = (List<Pair<int, int>>) null;
      for (int index = 0; index < strArray.Length; index += 4)
      {
        int num1 = int.Parse(strArray[index], (IFormatProvider) CultureInfo.InvariantCulture);
        int.Parse(strArray[index + 1], (IFormatProvider) CultureInfo.InvariantCulture);
        int num2 = int.Parse(strArray[index + 2], (IFormatProvider) CultureInfo.InvariantCulture);
        int second = int.Parse(strArray[index + 3], (IFormatProvider) CultureInfo.InvariantCulture);
        switch (num1)
        {
          case 1:
            if (pairList1 == null)
            {
              pairList1 = new List<Pair<int, int>>();
              set1 = new Set<int>();
            }
            if (!set1.Contains(num2))
            {
              pairList1.Add(new Pair<int, int>(num2, second));
              set1.Add(num2);
              break;
            }
            break;
          case 2:
            if (pairList2 == null)
            {
              pairList2 = new List<Pair<int, int>>();
              set2 = new Set<int>();
            }
            if (!set2.Contains(num2))
            {
              pairList2.Add(new Pair<int, int>(num2, second));
              set2.Add(num2);
              break;
            }
            break;
          case 3:
            if (pairList3 == null)
            {
              pairList3 = new List<Pair<int, int>>();
              set3 = new Set<int>();
            }
            if (!set3.Contains(num2))
            {
              pairList3.Add(new Pair<int, int>(num2, second));
              set3.Add(num2);
              break;
            }
            break;
          case 4:
            if (pairList4 == null)
            {
              pairList4 = new List<Pair<int, int>>();
              set4 = new Set<int>();
            }
            if (!set4.Contains(num2))
            {
              pairList4.Add(new Pair<int, int>(num2, second));
              set4.Add(num2);
              break;
            }
            break;
        }
      }
      if (pairList1 != null)
      {
        messageSearchResult.DataOffsets = pairList1.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(messageSearchResult.Message.Data, messageSearchResult.DataOffsets);
      }
      if (pairList2 != null)
      {
        messageSearchResult.MediaNameOffsets = pairList2.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(messageSearchResult.Message.MediaName, messageSearchResult.MediaNameOffsets);
      }
      if (pairList3 != null)
      {
        messageSearchResult.MediaCaptionOffsets = pairList3.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(messageSearchResult.Message.MediaCaption, messageSearchResult.MediaCaptionOffsets);
      }
      if (pairList4 != null)
      {
        messageSearchResult.LocationDetailsOffsets = pairList4.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(messageSearchResult.Message.LocationDetails, messageSearchResult.LocationDetailsOffsets);
      }
      return messageSearchResult;
    }

    public void IndexMessageForSearch(string jid, string id, bool fromMe)
    {
      Sqlite db = this.Db;
      bool finished = false;
      object finishedLock = new object();
      Action a = (Action) (() =>
      {
        lock (finishedLock)
        {
          if (finished)
          {
            Log.p("msgdb", "abort query | query already finished, do nothing");
          }
          else
          {
            Log.p("msgdb", "abort query | db interrupt");
            db.Interrupt();
            this.DisposeChildren();
            finished = true;
          }
        }
      });
      this.BeginTransaction();
      try
      {
        int messageId = this.GetMessageId(jid, id, fromMe);
        if (messageId < 0)
        {
          Log.WriteLineDebug("msg db: could not find row ID for message");
          this.RollbackTransaction();
        }
        else
        {
          using (PooledTimer.Instance.Schedule(TimeSpan.FromSeconds(10.0), a))
            this.IndexMessageById(messageId);
          this.CommitTransaction();
        }
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
      finally
      {
        lock (finishedLock)
          finished = true;
        Action action = (Action) (() => { });
      }
    }

    public int IndexMessageBatchForSearch(int batchSize)
    {
      List<KeyValuePair<int, int>> keyValuePairList = new List<KeyValuePair<int, int>>();
      Sqlite db = this.Db;
      bool finished = false;
      object finishedLock = new object();
      Action a = (Action) (() =>
      {
        lock (finishedLock)
        {
          if (finished)
          {
            Log.p("msgdb", "abort batch query | query already finished, do nothing");
          }
          else
          {
            Log.p("msgdb", "abort batch query | db interrupt");
            db.Interrupt();
            this.DisposeChildren();
            finished = true;
          }
        }
      });
      this.BeginTransaction();
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT MessageID, MediaWaType FROM Messages WHERE FtsStatus IS NULL OR FtsStatus = ? LIMIT ?"))
        {
          preparedStatement.Bind(0, 0, false);
          preparedStatement.Bind(1, batchSize, false);
          while (preparedStatement.Step())
          {
            int column1 = (int) (long) preparedStatement.Columns[0];
            int column2 = (int) (long) preparedStatement.Columns[1];
            keyValuePairList.Add(new KeyValuePair<int, int>(column1, column2));
          }
        }
        Log.WriteLineDebug("msg db: collected {0} message IDs to index", (object) keyValuePairList.Count);
        foreach (KeyValuePair<int, int> keyValuePair in keyValuePairList)
        {
          using (PooledTimer.Instance.Schedule(TimeSpan.FromSeconds(10.0), a))
          {
            if (!this.IndexMessageById(keyValuePair.Key, new int?(keyValuePair.Value)))
              break;
          }
        }
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        Log.l("IndexMessages", "Exception {0}, last sql:{1}", (object) ex.GetFriendlyMessage(), (object) this.Db.MaybeLastPreparedStatementString);
        this.RollbackTransaction(ex);
        throw;
      }
      finally
      {
        lock (finishedLock)
          finished = true;
        Action action = (Action) (() => { });
      }
      return keyValuePairList.Count;
    }

    private bool IndexMessageById(int rowId, int? messageType = null)
    {
      if (!messageType.HasValue)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT MediaWaType FROM Messages WHERE MessageID = ?"))
        {
          preparedStatement.Bind(0, rowId, false);
          if (preparedStatement.Step())
            messageType = new int?((int) (long) preparedStatement.Columns[0]);
        }
        if (!messageType.HasValue)
          throw new Exception("msg db: could not find type for message");
      }
      List<string> stringList = new List<string>();
      int? nullable = messageType;
      int num1 = 0;
      string sql;
      if ((nullable.GetValueOrDefault() == num1 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
      {
        sql = "INSERT INTO MessagesFts (docid, KeyRemoteJid, Data) SELECT MessageID, KeyRemoteJid, Data FROM Messages WHERE MessageID = ?";
      }
      else
      {
        nullable = messageType;
        int num2 = 9;
        if ((nullable.GetValueOrDefault() == num2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          sql = "INSERT INTO MessagesFts (docid, KeyRemoteJid, Data, MediaName, MediaCaption) SELECT MessageID, KeyRemoteJid, Data, MediaName, MediaCaption FROM Messages WHERE MessageID = ?";
        }
        else
        {
          nullable = messageType;
          int num3 = 1;
          if ((nullable.GetValueOrDefault() == num3 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
          {
            nullable = messageType;
            int num4 = 2;
            if ((nullable.GetValueOrDefault() == num4 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
            {
              nullable = messageType;
              int num5 = 3;
              if ((nullable.GetValueOrDefault() == num5 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
              {
                nullable = messageType;
                int num6 = 10;
                if ((nullable.GetValueOrDefault() == num6 ? (nullable.HasValue ? 1 : 0) : 0) == 0)
                {
                  nullable = messageType;
                  int num7 = 4;
                  if ((nullable.GetValueOrDefault() == num7 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
                  {
                    Message messageById = this.GetMessageById(rowId);
                    if (!messageType.HasValue)
                      throw new Exception("msg db: could not find message for rowid=" + (object) rowId);
                    if (messageById.HasMultipleContacts())
                    {
                      sql = "INSERT INTO MessagesFts (docid, KeyRemoteJid, MediaName) VALUES (?, ?, ?)";
                      stringList.Add(messageById.KeyRemoteJid);
                      stringList.Add(messageById.GetContactCardNames());
                      goto label_25;
                    }
                    else
                    {
                      sql = "INSERT INTO MessagesFts (docid, KeyRemoteJid, MediaName) SELECT MessageID, KeyRemoteJid, MediaName FROM Messages WHERE MessageID = ?";
                      goto label_25;
                    }
                  }
                  else
                  {
                    nullable = messageType;
                    int num8 = 5;
                    sql = (nullable.GetValueOrDefault() == num8 ? (nullable.HasValue ? 1 : 0) : 0) == 0 ? (string) null : "INSERT INTO MessagesFts (docid, KeyRemoteJid, LocationDetails) SELECT MessageID, KeyRemoteJid, LocationDetails FROM Messages WHERE MessageID = ?";
                    goto label_25;
                  }
                }
              }
            }
          }
          sql = "INSERT INTO MessagesFts (docid, KeyRemoteJid, MediaCaption) SELECT MessageID, KeyRemoteJid, MediaCaption FROM Messages WHERE MessageID = ?";
        }
      }
label_25:
      SqliteMessagesContext.FtsStatus val1;
      if (sql != null)
      {
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql))
          {
            preparedStatement.Bind(0, rowId, false);
            int num9 = 1;
            foreach (string val2 in stringList)
              preparedStatement.Bind(num9++, val2);
            preparedStatement.Step();
          }
          val1 = SqliteMessagesContext.FtsStatus.Indexed;
        }
        catch (Exception ex)
        {
          uint hresult = ex.GetHResult();
          if ((int) hresult == (int) Sqlite.HRForError(4U))
          {
            Log.WriteLineDebug("msg db: marking message {0} as unindexable due to tokenizer failure", (object) rowId);
            val1 = SqliteMessagesContext.FtsStatus.ErrorUnknown;
          }
          else if ((int) hresult == (int) Sqlite.HRForError(200U))
          {
            Log.WriteLineDebug("msg db: marking message {0} as unindexable due to unsupported language", (object) rowId);
            val1 = SqliteMessagesContext.FtsStatus.ErrorBadLanguage;
          }
          else if ((int) hresult == (int) Sqlite.HRForError(9U))
          {
            Log.WriteLineDebug("msg db: tokenizer interrupted on message {0}", (object) rowId);
            val1 = SqliteMessagesContext.FtsStatus.NotIndexed;
          }
          else
            throw;
        }
      }
      else
        val1 = SqliteMessagesContext.FtsStatus.UnsupportedType;
      if (val1 == SqliteMessagesContext.FtsStatus.NotIndexed)
        return false;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Messages SET FtsStatus = ? WHERE MessageID = ?"))
      {
        preparedStatement.Bind(0, (int) val1, false);
        preparedStatement.Bind(1, rowId, false);
        preparedStatement.Step();
      }
      return true;
    }

    private static SqliteMessagesContext.JidCombo[] ParseMessages(
      IEnumerable<Message> msgs,
      bool forDeletion)
    {
      return (forDeletion ? (IEnumerable<Message>) msgs.OrderByDescending<Message, int>((Func<Message, int>) (m => m.MessageID)) : msgs).GroupBy<Message, string>((Func<Message, string>) (m => m.KeyRemoteJid)).Select<IGrouping<string, Message>, SqliteMessagesContext.JidCombo>((Func<IGrouping<string, Message>, SqliteMessagesContext.JidCombo>) (g =>
      {
        if (forDeletion)
          return new SqliteMessagesContext.JidCombo()
          {
            Jid = g.Key,
            Message = g.FirstOrDefault<Message>(),
            Count = g.Count<Message>()
          };
        Message[] array1 = g.ToArray<Message>();
        Message[] array2 = ((IEnumerable<Message>) array1).Where<Message>((Func<Message, bool>) (ms => !ms.KeyFromMe)).ToArray<Message>();
        return new SqliteMessagesContext.JidCombo()
        {
          Jid = g.Key,
          Message = ((IEnumerable<Message>) array1).LastOrDefault<Message>(),
          Messages = (IEnumerable<Message>) array1,
          Count = ((IEnumerable<Message>) array1).Where<Message>((Func<Message, bool>) (m => m.IsQualifiedForUnread())).Count<Message>(),
          IncomingMessages = (IEnumerable<Message>) array2
        };
      })).ToArray<SqliteMessagesContext.JidCombo>();
    }

    public override void SubmitChanges()
    {
      if (!MessagesContext.Mutex.IsOwner())
        throw new Exception("Called SubmitChanges without the lock!");
      PerformanceTimer.Mode mode = PerformanceTimer.Mode.DebugAndBeta;
      DateTime? start1 = PerformanceTimer.Start(mode);
      SqliteDataContext.ChangeSet changeSet1 = this.GetChangeSet();
      Message[] array1 = changeSet1.Inserts.Where<object>((Func<object, bool>) (o => o is Message)).Cast<Message>().ToArray<Message>();
      Dictionary<string, Message> lastMsgPerJid = new Dictionary<string, Message>();
      Message[] array2 = changeSet1.Updates.Where<Pair<object, string[]>>((Func<Pair<object, string[]>, bool>) (p => p.First is Message && ((IEnumerable<string>) p.Second).Contains<string>("MediaWaType"))).Select<Pair<object, string[]>, Message>((Func<Pair<object, string[]>, Message>) (p => p.First as Message)).ToArray<Message>();
      List<Conversation> forcedConvoUpdates = new List<Conversation>();
      List<Action> postSubmitJobs = new List<Action>();
      if (((IEnumerable<Message>) array1).Any<Message>())
        this.ProcessNewMessagesPreSubmit(array1, ref lastMsgPerJid, ref postSubmitJobs, out forcedConvoUpdates);
      if (((IEnumerable<Message>) array2).Any<Message>())
      {
        Log.d("msg db", "submitting a change for a MediaWaType update.");
        this.ProcessMediaTypeUpdateMessagesPreSubmit(array2);
      }
      SqliteDataContext.ChangeSet changeSet2 = this.GetChangeSet();
      Conversation[] array3 = changeSet2.Deletes.Where<object>((Func<object, bool>) (o => o is Conversation)).Cast<Conversation>().ToArray<Conversation>();
      WhatsApp.ReceiptState[] array4 = changeSet2.Inserts.Where<object>((Func<object, bool>) (o => o is WhatsApp.ReceiptState)).Cast<WhatsApp.ReceiptState>().ToArray<WhatsApp.ReceiptState>();
      List<DbDataUpdate> dbDataUpdateList1 = new List<DbDataUpdate>();
      List<DbDataUpdate> dbDataUpdateList2 = new List<DbDataUpdate>();
      List<DbDataUpdate> dbDataUpdateList3 = new List<DbDataUpdate>();
      foreach (Pair<object, string[]> update in changeSet2.Updates)
      {
        if (update.First is Message)
          dbDataUpdateList1.Add(new DbDataUpdate(update.First, update.Second));
        else if (update.First is Conversation)
          dbDataUpdateList2.Add(new DbDataUpdate(update.First, update.Second));
        else if (update.First is JidInfo)
          dbDataUpdateList3.Add(new DbDataUpdate(update.First, update.Second));
      }
      dbDataUpdateList2.AddRange(forcedConvoUpdates.Select<Conversation, DbDataUpdate>((Func<Conversation, DbDataUpdate>) (o => new DbDataUpdate((object) o, DbDataUpdate.Types.Modified))));
      dbDataUpdateList2.AddRange(changeSet2.Inserts.Where<object>((Func<object, bool>) (o => o is Conversation)).Select<object, DbDataUpdate>((Func<object, DbDataUpdate>) (o => new DbDataUpdate(o, DbDataUpdate.Types.Added))));
      dbDataUpdateList3.AddRange(changeSet2.Inserts.Where<object>((Func<object, bool>) (o => o is JidInfo)).Select<object, DbDataUpdate>((Func<object, DbDataUpdate>) (o => new DbDataUpdate(o, DbDataUpdate.Types.Added))));
      PerformanceTimer.End("msg db: pre-submit work", start1);
      DateTime? start2 = PerformanceTimer.Start(mode);
      this.BeginTransaction();
      bool flag1 = true;
      LinkedList<PersistentAction> persistentActionsTodo = (LinkedList<PersistentAction>) null;
      try
      {
        this.SubmitChanges(false);
        PerformanceTimer.End("msg db: first submit", start2);
        DateTime? start3 = PerformanceTimer.Start(mode);
        bool flag2 = false;
        if (this.ProcessPostSubmitJobs(postSubmitJobs, out persistentActionsTodo))
          flag2 = true;
        if (flag2)
        {
          this.SubmitChanges(false);
          PerformanceTimer.End("msg db: second submit", start3);
          start3 = PerformanceTimer.Start(mode);
        }
        this.CommitTransaction();
        flag1 = false;
        PerformanceTimer.End("msg db: commit transaction", start3);
      }
      catch (Exception ex)
      {
        Log.l(nameof (SubmitChanges), "Exception {0}, last sql:{1}, inTx:{2}", (object) ex.GetFriendlyMessage(), (object) this.Db.MaybeLastPreparedStatementString, (object) flag1);
        if (flag1)
          this.RollbackTransaction(ex);
        throw;
      }
      PerformanceTimer.Start(mode);
      if (!AppState.UIUpdatesMuted)
      {
        foreach (Message message in array1)
          this.NotifyExternal<Message>(this.NewMessagesSubject, message, "new message");
        foreach (DbDataUpdate dbDataUpdate in dbDataUpdateList1)
          this.NotifyExternal<DbDataUpdate>(this.MessageUpdateSubject, dbDataUpdate, "updated message");
        foreach (Message message in array2)
          this.NotifyExternal<Message>(this.UpdatedMessageMediaWaTypeSubject, message, "updated message media wa type");
        foreach (Conversation conversation in array3)
        {
          this.convoCache.Remove(conversation.Jid);
          if (!conversation.SkipDeleteNotification)
            this.NotifyExternal<Conversation>(this.DeletedConversationSubject, conversation, "delete conversation");
        }
        foreach (DbDataUpdate dbDataUpdate in dbDataUpdateList2)
        {
          if (dbDataUpdate.UpdatedObj is Conversation updatedObj)
          {
            Message message = (Message) null;
            lastMsgPerJid.TryGetValue(updatedObj.Jid, out message);
            if (message == null)
            {
              int? lastMessageId = updatedObj.LastMessageID;
              if (lastMessageId.HasValue)
              {
                lastMessageId = updatedObj.LastMessageID;
                message = this.GetMessageById(lastMessageId.Value);
              }
            }
            Subject<ConvoAndMessage> conversationSubject = this.UpdatedConversationSubject;
            ConvoAndMessage convoAndMessage = new ConvoAndMessage();
            convoAndMessage.Conversation = updatedObj;
            convoAndMessage.LastMessage = message;
            int num;
            if (dbDataUpdate.UpdateType == DbDataUpdate.Types.Modified)
            {
              string[] modifiedColumns = dbDataUpdate.ModifiedColumns;
              num = modifiedColumns != null ? (((IEnumerable<string>) modifiedColumns).Contains<string>("SortKey") ? 1 : 0) : 0;
            }
            else
              num = 0;
            convoAndMessage.PinStateUpdated = num != 0;
            this.NotifyExternal<ConvoAndMessage>(conversationSubject, convoAndMessage, "conversation updated");
          }
        }
        foreach (WhatsApp.ReceiptState receiptState in array4)
          this.NotifyExternal<WhatsApp.ReceiptState>(this.NewReceiptSubject, receiptState, "new receipt");
        foreach (DbDataUpdate dbDataUpdate in dbDataUpdateList3)
          this.NotifyExternal<DbDataUpdate>(MessagesContext.Events.JidInfoUpdateSubject, dbDataUpdate, "jid info update");
      }
      if (persistentActionsTodo != null && persistentActionsTodo.Any<PersistentAction>())
        AppState.Worker.Enqueue((Action) (() =>
        {
          FunXMPP.Connection connection = AppState.GetConnection();
          if (connection == null)
            return;
          foreach (PersistentAction a in persistentActionsTodo)
            AppState.AttemptPersistentAction(a, connection);
        }));
      Log.l("msg db", "submit finished");
    }

    private void NotifyExternal<T>(Subject<T> subj, T item, string context)
    {
      Log.d("msg db", "notifying subscribers: {0}", (object) context);
      try
      {
        subj.OnNext(item);
      }
      catch (DatabaseInvalidatedException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        string context1 = context;
        Log.SendCrashLog(ex, context1);
      }
    }

    private void ProcessNewMessagesPreSubmit(
      Message[] toProcess,
      ref Dictionary<string, Message> lastMsgPerJid,
      ref List<Action> postSubmitJobs,
      out List<Conversation> forcedConvoUpdates)
    {
      forcedConvoUpdates = new List<Conversation>();
      List<Action> threadPoolJobs = new List<Action>();
      List<Message> messageList1 = new List<Message>();
      List<Message> messageList2 = new List<Message>();
      foreach (Message message in toProcess)
      {
        Message m = message;
        bool flag = m.IsStatus();
        if (flag)
          messageList2.Add(m);
        else
          messageList1.Add(m);
        if (m.MediaWaType != FunXMPP.FMessage.Type.System && !flag)
          postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.IndexForSearch(this, m));
        if (m.HasText())
          threadPoolJobs.Add(SqliteMessagesContext.PostSubmitJob.DetectLinks(m));
        if (m.MediaWaType == FunXMPP.FMessage.Type.Contact)
          postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.IndexContactCard(this, m));
        else if (m.MediaWaType == FunXMPP.FMessage.Type.LiveLocation && m.InternalProperties != null && m.InternalProperties.LiveLocationPropertiesField != null)
          postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.ProcessNewLiveLocation(this, m));
        if ((!m.KeyFromMe || m.Status == FunXMPP.FMessage.Status.Relay) && m.ShouldAutoDownload(this as MessagesContext))
          postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.SaveAutodownload(this, m));
        if (m.KeyFromMe)
        {
          ConversionRecord conversionRecord = ConversionRecordHelper.MaybeUpdateConversionRecord(m.KeyRemoteJid, m.FunTimestamp.Value);
          if (conversionRecord != null && (conversionRecord.Data != null && conversionRecord.Data.Length != 0 || !string.IsNullOrEmpty(conversionRecord.Source)))
          {
            MessageProperties messageProperties = m.InternalProperties ?? new MessageProperties();
            MessageProperties.ConversionRecordProperties recordProperties = messageProperties.EnsureConversionRecordProperties;
            recordProperties.Data = conversionRecord.Data;
            recordProperties.Source = conversionRecord.Source;
            recordProperties.DelaySeconds = new uint?((uint) Math.Max(0, (int) (FunRunner.CurrentServerTimeUtc - conversionRecord.Timestamp).TotalSeconds));
            m.InternalProperties = messageProperties;
          }
          if (m.IsBroadcasted())
          {
            Conversation conversation = this.GetConversation(m.KeyRemoteJid, CreateOptions.None);
            if (conversation != null)
            {
              Message[] array = ((IEnumerable<string>) conversation.GetParticipantJids()).Select<string, Message>((Func<string, Message>) (jid =>
              {
                Message msg = new Message();
                msg.CopyFrom(this, m, true, false);
                msg.KeyRemoteJid = jid;
                msg.RemoteResource = m.KeyRemoteJid;
                msg.Status = FunXMPP.FMessage.Status.NeverSend;
                return msg;
              })).ToArray<Message>();
              foreach (Message m1 in array)
                this.InsertMessageOnSubmit(m1);
              if (m.LocalFileUri != null)
                this.LocalFileAddRef(m.LocalFileUri, m.IsStatus() ? LocalFileType.StatusMedia : LocalFileType.MessageMedia, array.Length);
              if (m.DataFileName != null)
                this.LocalFileAddRef(m.DataFileName, LocalFileType.Thumbnail, array.Length);
              if (m.QuotedMediaFileUri != null)
                this.LocalFileAddRef(m.QuotedMediaFileUri, LocalFileType.QuotedMedia, array.Length);
            }
          }
          JidInfo jidInfo = this.GetJidInfo(m.KeyRemoteJid, CreateOptions.None);
          if (jidInfo != null && ((int) jidInfo.IsSuspicious ?? 1) != 0)
          {
            jidInfo.IsSuspicious = new bool?(false);
            SuspiciousJid.MarkJidSuspiciousInCache(jidInfo.Jid, false);
          }
        }
        if (m.GetMiscInfo() != null)
          postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.SaveMiscInfo(this, m));
      }
      if (messageList2.Any<Message>())
      {
        postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.SaveWaStatuses(this, messageList2));
        Log.d("msg db", "to insert {0} status(es)", (object) messageList2.Count);
      }
      foreach (SqliteMessagesContext.JidCombo message1 in SqliteMessagesContext.ParseMessages((IEnumerable<Message>) messageList1, false))
      {
        bool flag = AppState.IsConversationOpen(message1.Jid);
        CreateOptions options = message1.Message == null || message1.Message.ShouldCreateConversation() ? CreateOptions.CreateToDbIfNotFound : CreateOptions.None;
        Conversation conversation = this.GetConversation(message1.Jid, options);
        if (conversation != null && message1.Message != null)
        {
          if (message1.Messages != null)
          {
            Message message2 = message1.Messages.LastOrDefault<Message>((Func<Message, bool>) (m => m.FunTimestamp.HasValue && m.IsNoteworthy()));
            if (message2 != null)
              conversation.Timestamp = message2.FunTimestamp;
          }
          if (flag)
          {
            if (!conversation.LastMessageID.HasValue)
              forcedConvoUpdates.Add(conversation);
            postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.SendReadReceipt(this as MessagesContext, message1.IncomingMessages));
          }
          else
          {
            if (Settings.EnableGroupAlerts && !message1.Message.KeyFromMe && JidHelper.IsGroupJid(message1.Jid))
            {
              DateTime? automuteTimer = conversation.AutomuteTimer;
              DateTime currentServerTimeUtc = FunRunner.CurrentServerTimeUtc;
              if (!automuteTimer.HasValue || automuteTimer.Value < currentServerTimeUtc)
                conversation.AutomuteTimer = new DateTime?(currentServerTimeUtc + TimeSpan.FromSeconds((double) (60 + 5 * conversation.GetParticipantCount())));
              else
                message1.Message.IsAutomuted = true;
            }
            if (message1.Count > 0)
            {
              int unreadMessagesCount = conversation.GetUnreadMessagesCount();
              conversation.UnreadMessageCount = new int?(unreadMessagesCount + message1.Count);
              Log.d("msgdb", "unread msg count | {0} -> {1} | jid:{2}", (object) unreadMessagesCount, (object) conversation.GetUnreadMessagesCount(), (object) conversation.Jid);
            }
            Message message3 = message1.IncomingMessages.FirstOrDefault<Message>((Func<Message, bool>) (m => m.IsQualifiedForUnread()));
            if (!conversation.FirstUnreadMessageID.HasValue && message3 != null)
              postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.UpdateFirstUnreadMessage(conversation, message3));
          }
          if (message1.Message != null && message1.Message.ShouldUpdatePreview())
          {
            conversation.LastMessageID = new int?();
            postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.UpdateLastMessage(conversation, message1.Message));
            lastMsgPerJid[conversation.Jid] = message1.Message;
          }
          if (conversation.Status.HasValue && conversation.Status.Value == Conversation.ConversationStatus.Deleting && options != CreateOptions.None && message1.Messages.Any<Message>((Func<Message, bool>) (m => m.IsNoteworthy())))
            conversation.Status = new Conversation.ConversationStatus?(Conversation.ConversationStatus.Clearing);
          if (conversation.IsArchived && message1.Message.IsNoteworthy() && (!conversation.IsMuted() || !Settings.VacationModeEnabled))
            conversation.IsArchived = false;
          if (((IEnumerable<Message>) message1.Messages.Where<Message>((Func<Message, bool>) (m => m.KeyFromMe)).ToArray<Message>()).Any<Message>())
            postSubmitJobs.Add(SqliteMessagesContext.PostSubmitJob.UpdateFrequentChatScore(this, message1.Jid, message1.Messages.Where<Message>((Func<Message, bool>) (m => m.KeyFromMe)).ToArray<Message>()));
        }
      }
      if (threadPoolJobs.Count != 0)
        postSubmitJobs.Add((Action) (() => WAThreadPool.QueueUserWorkItem((Action) (() => threadPoolJobs.ForEach((Action<Action>) (a => a()))))));
      if (!messageList1.Any<Message>())
        return;
      Log.d("msg db", "to insert {0} message(s)", (object) messageList1.Count);
    }

    private void ProcessMediaTypeUpdateMessagesPreSubmit(Message[] msgs)
    {
      foreach (Message msg in msgs)
      {
        string participantJid = (string) null;
        if (JidHelper.IsMultiParticipantsChatJid(msg.KeyRemoteJid) && JidHelper.IsUserJid(msg.RemoteResource))
          participantJid = msg.RemoteResource;
        this.UpdateCipherTextReceiptsDecrypted(msg.KeyRemoteJid, msg.KeyId, participantJid);
      }
    }

    private bool ProcessPostSubmitJobs(
      List<Action> postSubmitJobs,
      out LinkedList<PersistentAction> persistentActionsTodo)
    {
      bool flag = false;
      if (postSubmitJobs != null && postSubmitJobs.Any<Action>())
      {
        postSubmitJobs.ForEach((Action<Action>) (a => a()));
        flag = true;
      }
      persistentActionsTodo = SqliteMessagesContext.PostSubmitJob.PersistentActions;
      SqliteMessagesContext.PostSubmitJob.PersistentActions = (LinkedList<PersistentAction>) null;
      Log.d("msgdb", "ProcessPostSubmitJobs {0} {1}", (object) flag, persistentActionsTodo == null ? (object) "null" : (object) persistentActionsTodo.Count.ToString());
      return flag;
    }

    protected bool ResetImpl()
    {
      this.DisposeDbHandle();
      return false;
    }

    public int FixMissingJidTypes()
    {
      Conversation[] source = (Conversation[]) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM Conversations WHERE JidType IS NULL OR JidType = ?"))
      {
        stmt.Bind(0, 0, false);
        source = this.ParseTable<Conversation>(stmt, "Conversations").ToArray<Conversation>();
      }
      Log.l("msgdb", "{0} convos missing jid type", (object) source.Length);
      if (((IEnumerable<Conversation>) source).Any<Conversation>())
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Conversations SET JidType = ? WHERE Jid = ?"))
        {
          foreach (Conversation conversation in source)
          {
            JidHelper.JidTypes jidType = JidHelper.GetJidType(conversation.Jid);
            preparedStatement.Bind(0, (int) jidType, false);
            preparedStatement.Bind(1, conversation.Jid);
            preparedStatement.Step();
            Log.l("msgdb", "fixed missing jid type | jid:{0},type:{1}", (object) conversation.Jid, (object) jidType);
            preparedStatement.Reset();
          }
        }
      }
      return source.Length;
    }

    public Conversation GetConversation(string jid, CreateOptions options)
    {
      return this.GetConversation(jid, options, out CreateResult _);
    }

    public Conversation GetConversation(string jid, CreateOptions options, out CreateResult result)
    {
      result = CreateResult.None;
      return this.GetConversation(jid, options, out result, true);
    }

    public Conversation GetConversation(
      string jid,
      CreateOptions options,
      out CreateResult result,
      bool tryGetStatusConvo)
    {
      result = CreateResult.None;
      if (string.IsNullOrEmpty(jid))
        return (Conversation) null;
      if (!JidChecker.CheckJidProtocolString(jid))
      {
        JidChecker.MaybeSendJidErrorClb("Get conversation", jid);
        return (Conversation) null;
      }
      if (JidHelper.IsStatusJid(jid) && tryGetStatusConvo)
        return WaStatusHelper.GetStatusConversation(this);
      Conversation convo = (Conversation) null;
      if ((options & CreateOptions.BypassCache) != CreateOptions.None || !this.convoCache.TryGetValue(jid, out convo))
      {
        this.PrepareCachedStatement(this.getConvoStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
        {
          stmt.Bind(0, jid);
          convo = this.ParseTable<Conversation>(stmt, "Conversations").SingleOrDefault<Conversation>();
        }));
        if (convo != null)
          this.convoCache[jid] = convo;
      }
      else if (convo != null && convo.ConversationID != 0)
      {
        int? cacheVersion1 = this.GetCacheVersion("Conversations", (object) convo.ConversationID);
        int cacheVersion2 = this.GetCacheVersion();
        if ((cacheVersion1.GetValueOrDefault() == cacheVersion2 ? (!cacheVersion1.HasValue ? 1 : 0) : 1) != 0)
          return this.GetConversation(jid, options | CreateOptions.BypassCache, out result, false);
      }
      if (convo == null && (options & CreateOptions.CreateIfNotFound) != CreateOptions.None)
      {
        convo = new Conversation(jid)
        {
          JidType = JidHelper.GetJidType(jid)
        };
        result = CreateResult.Created;
        JidInfo jidInfo = this.GetJidInfo(jid, CreateOptions.None);
        if (jidInfo != null && jidInfo.IsMuted())
          convo.MuteExpiration = jidInfo.MuteExpirationUtc;
        switch (JidHelper.GetJidType(jid))
        {
          case JidHelper.JidTypes.Status:
          case JidHelper.JidTypes.Psa:
            if (jidInfo == null)
              jidInfo = this.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound);
            jidInfo.SupportsFullEncryption = JidInfo.FullEncryptionState.SupportedAndNeverNotify;
            break;
        }
        if (jidInfo == null || jidInfo.SupportsFullEncryption != JidInfo.FullEncryptionState.SupportedAndNotified && jidInfo.SupportsFullEncryption != JidInfo.FullEncryptionState.SupportedAndNeverNotify)
        {
          if ((options & (CreateOptions) 2) != CreateOptions.None)
          {
            this.InsertMessageOnSubmit(SystemMessageUtils.CreateConversationEncrypted(this as MessagesContext, jid));
            if (jidInfo == null)
              jidInfo = this.GetJidInfo(jid, CreateOptions.CreateToDbIfNotFound);
            jidInfo.SupportsFullEncryption = JidInfo.FullEncryptionState.SupportedAndNotified;
          }
          else if (jidInfo != null)
            jidInfo.SupportsFullEncryption = JidInfo.FullEncryptionState.Supported;
        }
        if ((options & (CreateOptions) 2) != CreateOptions.None)
        {
          SystemMessageUtils.TryGenerateInitialBizSystemMessage2Tier(this, jidInfo.Jid, false);
          this.InsertConversationOnSubmit(convo);
          result = CreateResult.CreatedToDb;
          if ((options & (CreateOptions) 4) != CreateOptions.None)
          {
            this.SubmitChanges();
            result = CreateResult.CreatedAndSubmitted;
          }
        }
      }
      return convo;
    }

    public void InsertMessageOnSubmit(Message m) => this.Insert("Messages", (object) m);

    public void RevokeStatusOnSubmit(WaStatus status, out Action postDbAction)
    {
      postDbAction = (Action) null;
      if (!JidHelper.IsSelfJid(status.Jid))
        return;
      Message messageById = this.GetMessageById(status.MessageId);
      if (messageById == null)
        this.Delete("WaStatuses", (object) status);
      else
        this.RevokeMessageOnSubmit(messageById, out postDbAction);
    }

    public void RevokeMessageOnSubmit(Message msg, out Action postDbAction)
    {
      postDbAction = (Action) null;
      if (msg == null || msg.MediaWaType == FunXMPP.FMessage.Type.Revoked)
        return;
      if (msg.MediaWaType == FunXMPP.FMessage.Type.Divider || msg.MediaWaType == FunXMPP.FMessage.Type.System)
        Log.l("msgdb", "revoke msg | skip | type:{0}", (object) msg.MediaWaType);
      else if (msg.KeyFromMe)
      {
        Log.l("msgdb", "revoke a sent msg | {0}", (object) msg.LogInfo());
        string keyId = msg.KeyId;
        FunXMPP.FMessage.Type mediaWaType = msg.MediaWaType;
        if (msg.isCurrentlyLiveLocationMessage())
          LiveLocationManager.Instance.DisableLocationSharing(msg.KeyRemoteJid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
        string[] retainColumns = new string[7]
        {
          "KeyId",
          "KeyRemoteJid",
          "KeyFromMe",
          "ParticipantsHash",
          "RemoteResource",
          "Status",
          "TimestampLong"
        };
        this.ClearValues("Messages", (object) msg, retainColumns);
        msg.KeyId = FunXMPP.GenerateMessageId();
        msg.MediaWaType = FunXMPP.FMessage.Type.Revoked;
        msg.Status = FunXMPP.FMessage.Status.Unsent;
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureCommonProperties.RevokedMsgId = keyId;
        forMessage.EnsureCommonProperties.RevokedMediaType = new int?((int) mediaWaType);
        forMessage.Save();
        if (msg.IsStatus())
        {
          WaStatus waStatus = this.GetWaStatus(msg.GetSenderJid(), msg.MessageID);
          if (waStatus != null)
            this.Delete("WaStatuses", (object) waStatus);
        }
        AppState.Worker.Enqueue((Action) (() => AppState.SendMessage(AppState.ClientInstance.GetConnection(), msg)));
        FieldStats.ReportRevokeSend(msg, FunRunner.CurrentServerTimeUtc);
      }
      else
      {
        Log.l("msgdb", "revoke a received msg | {0}", (object) msg.LogInfo());
        if (msg.IsStatus())
        {
          AppState.QrPersistentAction.NotifySeen(msg.KeyRemoteJid, msg.KeyId, msg.KeyFromMe, msg.GetSenderJid());
          AppState.QrPersistentAction.NotifyRevoke(msg);
          this.DeleteMessage(msg);
        }
        else
        {
          msg.MediaWaType = FunXMPP.FMessage.Type.Revoked;
          msg.IsAlerted = false;
          msg.BinaryData = (byte[]) null;
          msg.Data = (string) null;
          msg.IsStarred = false;
          msg.LocationDetails = (string) null;
          msg.LocationUrl = (string) null;
          msg.MediaCaption = (string) null;
          msg.MediaDurationSeconds = 0;
          msg.MediaName = (string) null;
          msg.TextPerformanceHint = (byte[]) null;
          msg.TextSplittingHint = (byte[]) null;
          msg.ProtoBuf = (byte[]) null;
          VCard[] contactVcardForMessage = this.GetContactVCardForMessage(msg.MessageID);
          if (((IEnumerable<VCard>) contactVcardForMessage).Any<VCard>())
          {
            Log.l("msgdb", "removing vcards for revoked msg | msg={0}, vcardCount={1}", (object) msg.LogInfo(), (object) contactVcardForMessage.Length);
            foreach (VCard vcard in contactVcardForMessage)
              this.DeleteContacVCardOnSubmit(vcard);
          }
          Log.l("msgdb", "removing file for revoked msg | msg={0}, uri={1}", (object) msg.LogInfo(), (object) msg.LocalFileUri);
          List<Triad<LocalFileType, string, bool>> source = new List<Triad<LocalFileType, string, bool>>();
          if (msg.LocalFileUri != null)
            source.Add(new Triad<LocalFileType, string, bool>(LocalFileType.MessageMedia, msg.LocalFileUri, true));
          if (msg.DataFileName != null)
            source.Add(new Triad<LocalFileType, string, bool>(LocalFileType.Thumbnail, msg.DataFileName, true));
          if (msg.QuotedMediaFileUri != null)
            source.Add(new Triad<LocalFileType, string, bool>(LocalFileType.QuotedMedia, msg.QuotedMediaFileUri, true));
          Log.l("msgdb", "{0} associated files to delete due to revoke", (object) source.Count);
          if (source.Any<Triad<LocalFileType, string, bool>>())
          {
            foreach (Triad<LocalFileType, string, bool> triad in source)
              this.LocalFileRelease(triad.Second, triad.First, deleteNative: triad.Third);
          }
        }
        FieldStats.ReportRevokeRecv(msg, FunRunner.CurrentServerTimeUtc);
      }
    }

    public void DeleteMessage(Message msg)
    {
      if (msg == null)
        return;
      this.DeleteMessages(new Message[1]{ msg });
    }

    public void DeleteMessages(Message[] msgs)
    {
      if (msgs == null || !((IEnumerable<Message>) msgs).Any<Message>())
        return;
      Log.l("msg db", "delete {0} msgs", (object) msgs.Length);
      bool flag1 = false;
      try
      {
        List<Triad<LocalFileType, string, bool>> source1 = new List<Triad<LocalFileType, string, bool>>();
        List<long> longList1 = new List<long>();
        List<long> source2 = new List<long>();
        Dictionary<long, Message> msgDict = new Dictionary<long, Message>();
        Dictionary<string, Conversation> source3 = new Dictionary<string, Conversation>();
        Dictionary<string, Conversation> dictionary = new Dictionary<string, Conversation>();
        foreach (Message msg in msgs)
        {
          if (msg != null)
          {
            longList1.Add((long) msg.MessageID);
            msgDict[(long) msg.MessageID] = msg;
            bool flag2 = JidHelper.IsStatusJid(msg.KeyRemoteJid);
            if (flag2)
              source2.Add((long) msg.MessageID);
            if (msg.LocalFileUri != null)
              source1.Add(new Triad<LocalFileType, string, bool>(flag2 ? LocalFileType.StatusMedia : LocalFileType.MessageMedia, msg.LocalFileUri, msg.IsPtt()));
            if (msg.DataFileName != null)
              source1.Add(new Triad<LocalFileType, string, bool>(LocalFileType.Thumbnail, msg.DataFileName, true));
            if (msg.QuotedMediaFileUri != null)
              source1.Add(new Triad<LocalFileType, string, bool>(LocalFileType.QuotedMedia, msg.QuotedMediaFileUri, true));
            if (msg.KeyFromMe && msg.isCurrentlyLiveLocationMessage())
              LiveLocationManager.Instance.DisableLocationSharing(msg.KeyRemoteJid, wam_enum_live_location_sharing_session_ended_reason.OTHER);
            Conversation convo = (Conversation) null;
            int? nullable1;
            if (!flag2 && !source3.ContainsKey(msg.KeyRemoteJid))
            {
              if (convo == null)
                convo = this.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
              int? nullable2 = new int?();
              if (convo != null)
              {
                int? nullable3;
                nullable1 = nullable3 = convo.MessageLoadingStart();
                if (!nullable1.HasValue || msg.MessageID >= nullable3.Value)
                  source3[convo.Jid] = convo;
              }
            }
            if (!flag2 && !dictionary.ContainsKey(msg.KeyRemoteJid))
            {
              if (convo == null)
                convo = this.GetConversation(msg.KeyRemoteJid, CreateOptions.None);
              if (convo != null)
              {
                nullable1 = convo.LastMessageID;
                if (nullable1.HasValue)
                {
                  nullable1 = convo.LastMessageID;
                  if (nullable1.Value <= msg.MessageID)
                    dictionary[convo.Jid] = convo;
                }
              }
            }
          }
        }
        Log.d("msg db", "{0} associated files to delete", (object) source1.Count);
        StringBuilder stringBuilder1 = new StringBuilder("SELECT AlternateUploadUri, MessageId FROM MessageMiscInfos ");
        string str1;
        if (longList1.Count == 1)
        {
          str1 = "WHERE MessageId = ? ";
        }
        else
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.Append("WHERE MessageId IN (");
          stringBuilder2.Append(string.Join(",", longList1.Select<long, string>((Func<long, string>) (_ => "?"))));
          stringBuilder2.Append(") ");
          str1 = stringBuilder2.ToString();
        }
        stringBuilder1.Append(str1);
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder1.ToString()))
        {
          int i = 0;
          longList1.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
          while (stmt.Step())
          {
            if (stmt.Columns[0] is string column1)
            {
              long column = (long) stmt.Columns[1];
              Message message = (Message) null;
              if (msgDict.TryGetValue(column, out message) && message != null)
                source1.Add(new Triad<LocalFileType, string, bool>((LocalFileType) (JidHelper.IsStatusJid(message.KeyRemoteJid) ? 2 : 0), column1, true));
            }
          }
        }
        this.BeginTransaction();
        flag1 = true;
        List<long> longList2 = new List<long>();
        if (source2.Any<long>())
        {
          StringBuilder stringBuilder3 = new StringBuilder();
          if (source2.Count == 1)
          {
            stringBuilder3.Append("WHERE MessageId = ? ");
          }
          else
          {
            stringBuilder3.Append("WHERE MessageId IN (");
            stringBuilder3.Append(string.Join(",", source2.Select<long, string>((Func<long, string>) (_ => "?"))));
            stringBuilder3.Append(") ");
          }
          string str2 = stringBuilder3.ToString();
          string str3 = string.Format("Select StatusId FROM WaStatuses {0}", (object) str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(str3.ToString()))
          {
            int i = 0;
            source2.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
            while (stmt.Step())
              longList2.Add((long) stmt.Columns[0]);
          }
          string sql = string.Format("DELETE FROM WaStatuses {0}", (object) str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(sql))
          {
            int i = 0;
            source2.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
            stmt.Step();
          }
        }
        StringBuilder stringBuilder4 = new StringBuilder("DELETE FROM MessageMiscInfos ");
        stringBuilder4.Append(str1);
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder4.ToString()))
        {
          int i = 0;
          longList1.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
          stmt.Step();
        }
        StringBuilder stringBuilder5 = new StringBuilder("DELETE FROM ReceiptState ");
        stringBuilder5.Append(str1);
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder5.ToString()))
        {
          int i = 0;
          longList1.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
          stmt.Step();
        }
        StringBuilder stringBuilder6 = new StringBuilder("DELETE FROM Messages ");
        stringBuilder6.Append(str1);
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder6.ToString()))
        {
          int i = 0;
          longList1.ForEach((Action<long>) (mid => stmt.Bind(i++, mid, false)));
          stmt.Step();
        }
        this.CommitTransaction();
        flag1 = false;
        this.PurgeCache("Messages", (IEnumerable<long>) longList1);
        this.PurgeCache<MessageMiscInfo>("MessageMiscInfos", (Func<MessageMiscInfo, bool>) (misc => misc.MessageId.HasValue && msgDict.ContainsKey((long) misc.MessageId.Value)));
        this.PurgeCacheAndEmpty<WhatsApp.ReceiptState>("ReceiptState", (Func<WhatsApp.ReceiptState, bool>) (rs => msgDict.ContainsKey((long) rs.MessageId)));
        if (longList2.Any<long>())
          this.PurgeCache("WaStatuses", (IEnumerable<long>) longList2);
        foreach (KeyValuePair<string, Conversation> keyValuePair in dictionary)
        {
          Conversation convo = keyValuePair.Value;
          Message message = ((IEnumerable<Message>) this.GetLatestMessages(keyValuePair.Key, convo.MessageLoadingStart(), new int?(1), new int?())).FirstOrDefault<Message>();
          convo.LastMessageID = message?.MessageID;
          convo.Timestamp = (DateTime?) message?.FunTimestamp;
          if (!AppState.UIUpdatesMuted)
            this.NotifyExternal<ConvoAndMessage>(this.UpdatedConversationSubject, new ConvoAndMessage()
            {
              Conversation = convo,
              LastMessage = message
            }, "conversation update");
        }
        foreach (KeyValuePair<string, Conversation> keyValuePair in source3)
          keyValuePair.Value.UpdateModifyTag();
        if (source3.Any<KeyValuePair<string, Conversation>>())
          this.SubmitChanges();
        foreach (Triad<LocalFileType, string, bool> triad in source1)
          this.LocalFileRelease(triad.Second, triad.First, deleteNative: triad.Third);
        if (source1.Any<Triad<LocalFileType, string, bool>>())
          this.SubmitChanges();
        if (AppState.UIUpdatesMuted)
          return;
        foreach (Message msg in msgs)
          this.NotifyExternal<Message>(this.DeletedMessagesSubject, msg, "deleted message");
      }
      catch (Exception ex)
      {
        Log.l(nameof (DeleteMessages), "Exception {0}, last sql:{1}, inTx{2}", (object) ex.GetFriendlyMessage(), (object) this.Db.MaybeLastPreparedStatementString, (object) flag1);
        if (flag1)
          this.RollbackTransaction(ex);
        throw;
      }
    }

    public Message[] GetSavedMediaMessages(
      string[] jids,
      FunXMPP.FMessage.Type[] includedTypes,
      bool excludePtt,
      bool asc,
      int? msgId = null,
      int? limit = null,
      bool includingMiscInfo = true)
    {
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(jids, asc)
      {
        WithSavedMediaOnly = true,
        WithMediaOrigin = excludePtt ? new bool?(false) : new bool?(),
        LowerBoundMsgId = msgId,
        IncludeLowerBound = false,
        Limit = limit,
        IncludeMiscInfo = includingMiscInfo,
        Types = includedTypes
      }))
        return messagesImpl.ToArray<Message>();
    }

    public long GetLastMsgId()
    {
      string sql = "SELECT MessageID FROM Messages ORDER BY MessageID DESC LIMIT 1";
      long lastMsgId = -1;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql))
      {
        if (preparedStatement.Step())
          lastMsgId = (long) preparedStatement.Columns[0];
      }
      return lastMsgId;
    }

    public bool GetMediaMessages(
      Action<string, long, string, long, int> processFileCallback,
      ref long msgIdStart,
      ref int limit)
    {
      bool mediaMessages = false;
      bool flag = false;
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT MessageID, IFNULL(KeyRemoteJid,''), MediaWaType, LocalFileUri, MediaSize FROM Messages ");
      stringBuilder.Append(" WHERE LocalFileUri IS NOT NULL AND MessageId > ? ");
      objectList.Add((object) msgIdStart);
      stringBuilder.Append(" ORDER BY MessageID ");
      stringBuilder.Append(" LIMIT " + (object) limit);
      long ticks = DateTime.Now.Ticks;
      int num1 = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        while (preparedStatement.Step())
        {
          ++num1;
          int num2 = 1;
          msgIdStart = Math.Max((long) (preparedStatement.Columns[0] ?? (object) 0), msgIdStart);
          string column1 = (string) preparedStatement.Columns[1];
          long num3 = (long) (preparedStatement.Columns[2] ?? (object) -1);
          string column2 = (string) preparedStatement.Columns[3];
          long num4 = (long) (preparedStatement.Columns[4] ?? (object) 0);
          if (!flag)
          {
            try
            {
              processFileCallback(column1, num3, column2, num4, num2);
            }
            catch (Exception ex)
            {
              Log.LogException(ex, "Exception using callback for filename, callback will be ignored");
              flag = true;
            }
          }
        }
        if (num1 < limit)
          mediaMessages = true;
      }
      long num5 = DateTime.Now.Ticks - ticks;
      if (!mediaMessages)
      {
        if (num5 > SqliteMessagesContext.MAX_MEDIA_MESSAGE_TICKS)
        {
          limit = Math.Max(1, limit / 2);
          Log.d("chats stats", "Used {0} ticks to find {1} rows, limit decreased to {2}", (object) num5, (object) num1, (object) limit);
        }
        else if (num5 < SqliteMessagesContext.MIN_MEDIA_MESSAGE_TICKS)
        {
          limit = Math.Min(1000, limit + limit / 3);
          Log.d("chats stats", "Used {0} ticks to find {1} rows, limit increased to {2}", (object) num5, (object) num1, (object) limit);
        }
      }
      return mediaMessages;
    }

    public void GetMessageCounts(string jid, ref Dictionary<long, long> chatCounts, int startMsgId)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT MediaWaType, COUNT(*) FROM Messages ");
      stringBuilder.Append("WHERE KeyRemoteJid = ? ");
      objectList.Add((object) jid);
      if (startMsgId > 0)
      {
        stringBuilder.Append("AND MessageID > ? ");
        objectList.Add((object) startMsgId);
      }
      stringBuilder.Append("GROUP BY MediaWaType ");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        while (preparedStatement.Step())
        {
          long key = (long) (preparedStatement.Columns[0] ?? (object) -1);
          long num1 = (long) (preparedStatement.Columns[1] ?? (object) 0);
          if (num1 > 0L)
          {
            long num2 = 0;
            if (chatCounts.TryGetValue(key, out num2))
            {
              long num3 = num2 + num1;
            }
            else
              chatCounts.Add(key, num1);
          }
        }
      }
    }

    public Message[] GetMessagesWithBackgroundID()
    {
      Message[] r = (Message[]) null;
      this.PrepareCachedStatement(this.getBgMessagesStmt, (Action<Sqlite.PreparedStatement>) (stmt => r = this.ParseTable<MessageMiscInfo>(stmt, "MessageMiscInfos").Where<MessageMiscInfo>((Func<MessageMiscInfo, bool>) (mmi => mmi.MessageId.HasValue)).Select<MessageMiscInfo, Message>((Func<MessageMiscInfo, Message>) (mmi => this.GetMessageById(mmi.MessageId.Value))).Where<Message>((Func<Message, bool>) (msg => msg != null)).ToArray<Message>()));
      return r;
    }

    public Message[] GetMessagesWithMediaUrl(string url)
    {
      Message[] r = (Message[]) null;
      this.PrepareCachedStatement(this.getMessagesWithMediaUrlStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, url);
        r = this.ParseTable<Message>(stmt, "Messages").ToArray<Message>();
      }));
      return r;
    }

    public bool AnyMessages(string jid) => this.AnyMessages(jid, false);

    public bool AnyMessages(string jid, bool excludingSystemMessages)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT KeyRemoteJid FROM Messages \nWHERE KeyRemoteJid = ? \n");
      if (excludingSystemMessages)
      {
        stringBuilder.Append(" AND MediaWaType != ");
        stringBuilder.Append(7.ToString());
        stringBuilder.Append(" \n");
      }
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        preparedStatement.Bind(0, jid);
        preparedStatement.Step();
        return preparedStatement.Columns[0] != null;
      }
    }

    public bool AnyPendingMessageDeletions(string jid)
    {
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      int? nullable = conversation == null ? new int?() : conversation.MessageLoadingStart();
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      objectList.Add((object) jid);
      if (nullable.HasValue)
      {
        stringBuilder.Append("AND (MessageID < ? OR Flags = ?) \n");
        objectList.Add((object) nullable.Value);
        objectList.Add((object) 1);
      }
      else
      {
        stringBuilder.Append("AND Flags = ? \n");
        objectList.Add((object) 1);
      }
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          stmt.BindObject(num++, o);
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
      }
    }

    public bool AnyPendingMessageDeletions(int? loadingStart, bool hasStarredMsgsToKeep = false)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      if (loadingStart.HasValue)
      {
        stringBuilder.Append("WHERE (MessageID < ? OR Flags = ?) \n");
        objectList.Add((object) loadingStart.Value);
      }
      else
        stringBuilder.Append("WHERE Flags = ? \n");
      objectList.Add((object) 1);
      if (hasStarredMsgsToKeep)
        stringBuilder.Append("AND IsStarred = 0 \n");
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          stmt.BindObject(num++, o);
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
      }
    }

    public bool AnyStarredMessages()
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM MESSAGES WHERE IsStarred = 1 LIMIT 1 \n"))
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
    }

    public bool AnyStarredMessagesWith(string jid)
    {
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      int? nullable = conversation == null ? new int?() : conversation.MessageLoadingStart();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      stringBuilder.Append("AND IsStarred = 1 \n");
      if (nullable.HasValue)
        stringBuilder.Append("AND MessageID >= ? \n");
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement1 = stmt;
        int idx1 = num1;
        int num2 = idx1 + 1;
        string val1 = jid;
        preparedStatement1.Bind(idx1, val1);
        if (nullable.HasValue)
        {
          Sqlite.PreparedStatement preparedStatement2 = stmt;
          int idx2 = num2;
          int num3 = idx2 + 1;
          int val2 = nullable.Value;
          preparedStatement2.Bind(idx2, val2, false);
        }
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
      }
    }

    public Message[] GetStarredMessages(string[] jids, int? limit, int? offset)
    {
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(jids, false)
      {
        WithSavedMediaOnly = false,
        LowerBoundMsgId = new int?(),
        IncludeLowerBound = true,
        UpperBoundMsgId = new int?(),
        IncludeUpperBound = false,
        Limit = limit,
        Offset = offset,
        IncludeMiscInfo = true,
        StarredOnly = true
      }))
        return messagesImpl.ToArray<Message>();
    }

    public void UnstarMessages(string[] jids)
    {
      LinkedList<string> linkedList = new LinkedList<string>();
      List<object> objectList = new List<object>();
      if (jids != null && ((IEnumerable<string>) jids).Any<string>())
      {
        if (jids.Length == 1)
        {
          linkedList.AddLast("KeyRemoteJid = ?");
          objectList.Add((object) jids[0]);
        }
        else if (jids.Length > 1)
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("KeyRemoteJid IN (");
          bool flag = true;
          foreach (string jid in jids)
          {
            if (flag)
              flag = false;
            else
              stringBuilder.Append(", ");
            stringBuilder.Append("?");
            objectList.Add((object) jid);
          }
          stringBuilder.Append(")");
          linkedList.AddLast(stringBuilder.ToString());
        }
      }
      linkedList.AddLast("IsStarred = 1");
      StringBuilder stringBuilder1 = new StringBuilder("UPDATE Messages SET IsStarred = 0 \n");
      if (linkedList.Any<string>())
      {
        stringBuilder1.Append("WHERE ");
        stringBuilder1.Append(string.Join("\n AND ", (IEnumerable<string>) linkedList));
        stringBuilder1.Append("\n");
      }
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder1.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        preparedStatement.Step();
      }
    }

    public Message GetMessageById(int id)
    {
      Message r = (Message) null;
      this.PrepareCachedStatement(this.getMsgByIdStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, id, false);
        r = this.ParseTableFirstOrDefault<Message>(stmt, "Messages");
      }));
      return r;
    }

    public Message GetFirstMessageNewerThan(string jid, DateTime newerThan)
    {
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      return conversation != null ? ((IEnumerable<Message>) this.GetMessagesAfter(jid, conversation.MessageLoadingStart(), newerThan, false, new int?(1), new int?())).FirstOrDefault<Message>() : (Message) null;
    }

    public Message[] GetUnsentMessages()
    {
      Message[] unsentMsgs = (Message[]) null;
      this.PrepareCachedStatement(this.unsentMessageStmt, (Action<Sqlite.PreparedStatement>) (stmt => unsentMsgs = this.ParseTable<Message>(stmt, "Messages").ToArray<Message>()));
      return unsentMsgs;
    }

    public bool MessageExists(string jid, string id, bool fromMe)
    {
      bool r = false;
      this.PrepareCachedStatement(this.dedupStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, id);
        stmt.Bind(2, fromMe);
        stmt.Step();
        r = (long) stmt.Columns[0] != 0L;
      }));
      return r;
    }

    public Message GetMessage(string jid, string id, bool fromMe, bool checkAvailable = true)
    {
      Message r = (Message) null;
      this.PrepareCachedStatement(this.getMsgStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, id);
        stmt.Bind(2, fromMe);
        r = this.ParseTable<Message>(stmt, "Messages").SingleOrDefault<Message>();
      }));
      if (checkAvailable && r != null && !r.IsAvailable(this))
        r = (Message) null;
      return r;
    }

    public int GetMessageId(string jid, string id, bool fromMe)
    {
      int messageId = -1;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT MessageID FROM Messages WHERE KeyRemoteJid = ? AND KeyFromMe = ? AND KeyId = ?"))
      {
        preparedStatement.Bind(0, jid);
        preparedStatement.Bind(1, fromMe);
        preparedStatement.Bind(2, id);
        if (preparedStatement.Step())
          messageId = (int) (long) preparedStatement.Columns[0];
      }
      return messageId;
    }

    public Message GetMessageForPrimaryTile()
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT c.*, '#', m.* \n");
      stringBuilder.Append("FROM Conversations as c \n");
      stringBuilder.Append("LEFT JOIN Messages as m \n");
      stringBuilder.Append("ON c.LastMessageID = m.MessageID \n");
      stringBuilder.Append("WHERE c.UnreadTileCount > 0 \n");
      stringBuilder.Append("ORDER BY c.LastMessageID DESC \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        KeyValuePair<Conversation, Message>[] array = this.ParseJoin<Conversation, Message>(stmt, "Conversations", "Messages").ToArray<KeyValuePair<Conversation, Message>>();
        return ((IEnumerable<KeyValuePair<Conversation, Message>>) array).Any<KeyValuePair<Conversation, Message>>() ? ((IEnumerable<KeyValuePair<Conversation, Message>>) array).First<KeyValuePair<Conversation, Message>>().Value : (Message) null;
      }
    }

    public Message[] GetEarliestMessages(
      string jid,
      int? loadingStart,
      int? limit,
      int? offset,
      bool includeMiscInfo = false)
    {
      int? nullable;
      if (limit.HasValue)
      {
        nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() <= num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          return new Message[0];
      }
      string jid1 = jid;
      int? loadingStart1 = loadingStart;
      nullable = new int?();
      int? lowerBoundMsgId = nullable;
      int? limit1 = limit;
      int? offset1 = offset;
      int num1 = includeMiscInfo ? 1 : 0;
      FunXMPP.FMessage.Type? mediaType = new FunXMPP.FMessage.Type?();
      return this.GetMessagesAfter(jid1, loadingStart1, lowerBoundMsgId, true, limit1, offset1, num1 != 0, mediaType: mediaType);
    }

    public Message[] GetLatestMessages(
      string jid,
      int? loadingStart,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      FunXMPP.FMessage.Type? mediaType = null,
      string participantJid = null)
    {
      int? nullable;
      if (limit.HasValue)
      {
        nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() <= num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
          return new Message[0];
      }
      string jid1 = jid;
      int? loadingStart1 = loadingStart;
      nullable = new int?();
      int? upperBoundMsgId = nullable;
      int? limit1 = limit;
      int? offset1 = offset;
      int num1 = includeMiscInfo ? 1 : 0;
      int num2 = !mediaType.HasValue || !mediaType.HasValue ? 0 : 1;
      FunXMPP.FMessage.Type? mediaType1 = mediaType;
      string participantJid1 = participantJid;
      return this.GetMessagesBefore(jid1, loadingStart1, upperBoundMsgId, limit1, offset1, num1 != 0, mediaOnly: num2 != 0, mediaType: mediaType1, participantJid: participantJid1);
    }

    public Message[] GetMessagesAfter(
      string jid,
      int? loadingStart,
      int? lowerBoundMsgId,
      bool includeBound,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      bool asc = true,
      bool mediaOnly = false,
      FunXMPP.FMessage.Type? mediaType = null)
    {
      int? nullable = lowerBoundMsgId;
      if (loadingStart.HasValue && (!lowerBoundMsgId.HasValue || loadingStart.Value > lowerBoundMsgId.Value))
      {
        nullable = loadingStart;
        includeBound = true;
      }
      SqliteMessagesContext.MessageQueryArgs queryArgs = new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, true);
      queryArgs.WithSavedMediaOnly = false;
      queryArgs.LowerBoundMsgId = nullable;
      queryArgs.IncludeLowerBound = includeBound;
      queryArgs.Limit = limit;
      queryArgs.Offset = offset;
      queryArgs.IncludeMiscInfo = includeMiscInfo;
      SqliteMessagesContext.MessageQueryArgs messageQueryArgs = queryArgs;
      FunXMPP.FMessage.Type[] typeArray;
      if (mediaOnly)
      {
        if (!mediaType.HasValue || !mediaType.HasValue)
          typeArray = new FunXMPP.FMessage.Type[6]
          {
            FunXMPP.FMessage.Type.Audio,
            FunXMPP.FMessage.Type.Image,
            FunXMPP.FMessage.Type.Video,
            FunXMPP.FMessage.Type.Gif,
            FunXMPP.FMessage.Type.Document,
            FunXMPP.FMessage.Type.Sticker
          };
        else
          typeArray = new FunXMPP.FMessage.Type[1]
          {
            mediaType.Value
          };
      }
      else
        typeArray = (FunXMPP.FMessage.Type[]) null;
      messageQueryArgs.Types = typeArray;
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(queryArgs))
        return asc ? messagesImpl.ToArray<Message>() : messagesImpl.Reverse<Message>().ToArray<Message>();
    }

    public Message[] GetMessagesAfter(
      string jid,
      int? loadingStart,
      DateTime afterUtc,
      bool includeBound,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      bool asc = true)
    {
      DateTime dateTime = afterUtc;
      if (loadingStart.HasValue)
      {
        Message message = ((IEnumerable<Message>) this.GetMessagesAfter(jid, loadingStart, new int?(), true, new int?(1), new int?())).FirstOrDefault<Message>();
        DateTime? nullable = message == null ? new DateTime?() : message.FunTimestamp;
        if (nullable.HasValue && nullable.Value > afterUtc)
        {
          dateTime = nullable.Value;
          includeBound = true;
        }
      }
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, true)
      {
        WithSavedMediaOnly = false,
        LowerBoundTimestampUtc = new DateTime?(dateTime),
        IncludeLowerBound = includeBound,
        Limit = limit,
        Offset = offset,
        IncludeMiscInfo = includeMiscInfo
      }))
        return asc ? messagesImpl.ToArray<Message>() : messagesImpl.Reverse<Message>().ToArray<Message>();
    }

    public Dictionary<string, Dictionary<long, long>> AccumulateRecentOutGoingMessages(
      DateTime dtUtc,
      int? limit = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<object> objectList = new List<object>();
      stringBuilder.Append("SELECT KeyRemoteJid, MediaWaType FROM Messages WHERE KeyFromMe = 1 AND KeyRemoteJid <> ? AND TimestampLong >= ? \n");
      objectList.Add((object) "status@broadcast");
      objectList.Add((object) dtUtc.ToUnixTime());
      stringBuilder.Append("ORDER BY MessageID DESC \n");
      Dictionary<string, Dictionary<long, long>> dictionary1 = new Dictionary<string, Dictionary<long, long>>();
      if (limit.HasValue)
      {
        stringBuilder.Append("LIMIT ? \n");
        objectList.Add((object) limit.Value);
      }
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        foreach (object o in objectList)
          preparedStatement.BindObject(num1++, o);
        while (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] != null && preparedStatement.Columns[1] != null)
          {
            string column1 = preparedStatement.Columns[0] as string;
            long column2 = (long) preparedStatement.Columns[1];
            Dictionary<long, long> dictionary2;
            if (!dictionary1.TryGetValue(column1, out dictionary2))
            {
              dictionary2 = new Dictionary<long, long>();
              dictionary1[column1] = dictionary2;
            }
            long num2;
            if (!dictionary2.TryGetValue(column2, out num2))
              num2 = 0L;
            dictionary2[column2] = ++num2;
          }
        }
      }
      return dictionary1;
    }

    public Message[] GetMessagesBefore(
      string jid,
      int? loadingStart,
      int? upperBoundMsgId,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      bool asc = false,
      bool mediaOnly = false,
      FunXMPP.FMessage.Type? mediaType = null,
      string participantJid = null)
    {
      SqliteMessagesContext.MessageQueryArgs queryArgs = new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, false);
      queryArgs.ParticipantJid = participantJid;
      queryArgs.WithSavedMediaOnly = false;
      queryArgs.LowerBoundMsgId = loadingStart;
      queryArgs.IncludeLowerBound = true;
      queryArgs.UpperBoundMsgId = upperBoundMsgId;
      queryArgs.IncludeUpperBound = false;
      queryArgs.Limit = limit;
      queryArgs.Offset = offset;
      queryArgs.IncludeMiscInfo = includeMiscInfo;
      SqliteMessagesContext.MessageQueryArgs messageQueryArgs = queryArgs;
      FunXMPP.FMessage.Type[] typeArray;
      if (mediaOnly)
      {
        if (!mediaType.HasValue || !mediaType.HasValue)
          typeArray = new FunXMPP.FMessage.Type[6]
          {
            FunXMPP.FMessage.Type.Audio,
            FunXMPP.FMessage.Type.Image,
            FunXMPP.FMessage.Type.Video,
            FunXMPP.FMessage.Type.Gif,
            FunXMPP.FMessage.Type.Document,
            FunXMPP.FMessage.Type.Sticker
          };
        else
          typeArray = new FunXMPP.FMessage.Type[1]
          {
            mediaType.Value
          };
      }
      else
        typeArray = (FunXMPP.FMessage.Type[]) null;
      messageQueryArgs.Types = typeArray;
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(queryArgs))
        return asc ? messagesImpl.Reverse<Message>().ToArray<Message>() : messagesImpl.ToArray<Message>();
    }

    public Message[] GetStarredMessagesBefore(
      string jid,
      int? loadingStart,
      int? upperBoundMsgId,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      bool asc = false,
      FunXMPP.FMessage.Type[] types = null)
    {
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, false)
      {
        WithSavedMediaOnly = false,
        LowerBoundMsgId = loadingStart,
        IncludeLowerBound = true,
        UpperBoundMsgId = upperBoundMsgId,
        IncludeUpperBound = false,
        Limit = limit,
        Offset = offset,
        IncludeMiscInfo = includeMiscInfo,
        StarredOnly = true,
        Types = types
      }))
        return asc ? messagesImpl.Reverse<Message>().ToArray<Message>() : messagesImpl.ToArray<Message>();
    }

    public Message[] GetMessagesBefore(
      string jid,
      int? loadingStart,
      DateTime beforeUtc,
      int? limit,
      int? offset,
      bool includeMiscInfo = false,
      bool asc = false)
    {
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, false)
      {
        WithSavedMediaOnly = false,
        LowerBoundMsgId = loadingStart,
        IncludeLowerBound = true,
        UpperBoundTimestampUtc = new DateTime?(beforeUtc),
        IncludeUpperBound = false,
        Limit = limit,
        Offset = offset,
        IncludeMiscInfo = includeMiscInfo
      }))
        return asc ? messagesImpl.Reverse<Message>().ToArray<Message>() : messagesImpl.ToArray<Message>();
    }

    public Message[] GetMessagesBetween(
      string jid,
      int? loadingStart,
      int firstMsgId,
      int lastMsgId,
      bool includeBounds,
      bool includeMiscInfo = false,
      bool asc = true)
    {
      if (firstMsgId > lastMsgId)
        return this.GetMessagesBetween(jid, loadingStart, lastMsgId, firstMsgId, includeBounds, includeMiscInfo, asc);
      int num = firstMsgId;
      bool flag = includeBounds;
      if (loadingStart.HasValue && loadingStart.Value > firstMsgId)
      {
        num = loadingStart.Value;
        flag = true;
      }
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs(new string[1]
      {
        jid
      }, asc)
      {
        WithSavedMediaOnly = false,
        LowerBoundMsgId = new int?(num),
        IncludeLowerBound = flag,
        UpperBoundMsgId = new int?(lastMsgId),
        IncludeUpperBound = includeBounds,
        IncludeMiscInfo = includeMiscInfo
      }))
        return messagesImpl.ToArray<Message>();
    }

    public Message GetLatestLiveLocationMessage(string jid, string participantJid, int offset = 0)
    {
      bool flag1 = JidHelper.IsGroupJid(jid);
      bool flag2 = Settings.MyJid.Equals(participantJid);
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      stringBuilder.Append("AND MediaWaType = ? \n");
      stringBuilder.Append("AND KeyFromMe = ? \n");
      if (flag1 && !flag2)
        stringBuilder.Append("AND RemoteResource = ? \n");
      stringBuilder.Append("ORDER BY MessageID DESC \n LIMIT 1 \n");
      if (offset > 0)
        stringBuilder.Append("OFFSET ? \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement1 = stmt;
        int idx1 = num1;
        int num2 = idx1 + 1;
        string val1 = jid;
        preparedStatement1.Bind(idx1, val1);
        Sqlite.PreparedStatement preparedStatement2 = stmt;
        int idx2 = num2;
        int num3 = idx2 + 1;
        preparedStatement2.Bind(idx2, 11, false);
        Sqlite.PreparedStatement preparedStatement3 = stmt;
        int idx3 = num3;
        int num4 = idx3 + 1;
        string val2 = flag2 ? "1" : "0";
        preparedStatement3.Bind(idx3, val2);
        if (flag1 && !flag2)
          stmt.Bind(num4++, participantJid);
        if (offset > 0)
        {
          Sqlite.PreparedStatement preparedStatement4 = stmt;
          int idx4 = num4;
          int num5 = idx4 + 1;
          int val3 = offset;
          preparedStatement4.Bind(idx4, val3, false);
        }
        return this.ParseTable<Message>(stmt, "Messages").FirstOrDefault<Message>();
      }
    }

    public bool HaveContactVCardForJid(string jid)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT count(*) FROM ContactVCards WHERE Jid=?"))
      {
        preparedStatement.Bind(0, jid);
        preparedStatement.Step();
        return (int) (long) preparedStatement.Columns[0] > 0;
      }
    }

    public Message[] GetContactVCardsWithJid(string jid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT m.*,'#',c.* FROM ContactVCards as c JOIN Messages as m ON c.Jid=? AND c.MessageId = m.MessageID"))
      {
        stmt.Bind(0, jid);
        return this.ParseJoin<Message, VCard>(stmt, "Messages", "ContactVCards").Select<KeyValuePair<Message, VCard>, Message>((Func<KeyValuePair<Message, VCard>, Message>) (kvp => kvp.Key)).ToArray<Message>();
      }
    }

    public Message[] GetTrustedContactVCardsWithJid(string jid)
    {
      return ((IEnumerable<Message>) this.GetContactVCardsWithJid(jid)).Where<Message>((Func<Message, bool>) (msg => JidHelper.IsJidInAddressBook(msg.GetSenderJid()))).ToArray<Message>();
    }

    public void InsertFrequentChatScore(FrequentChatScore fcs)
    {
      this.Insert("FrequentChatScores", (object) fcs);
    }

    public void ClearFrequentChats()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM FrequentChatScores"))
        preparedStatement.Step();
      this.PurgeCache("FrequentChatScores");
    }

    public FrequentChatScore GetFrequentChatScore(string jid, CreateOptions createOpts)
    {
      if (string.IsNullOrEmpty(jid))
        return (FrequentChatScore) null;
      FrequentChatScore fcs = (FrequentChatScore) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM FrequentChatScores WHERE Jid = ?"))
      {
        stmt.Bind(0, jid);
        fcs = this.ParseTable<FrequentChatScore>(stmt, "FrequentChatScores").FirstOrDefault<FrequentChatScore>();
      }
      if (fcs == null && (createOpts & CreateOptions.CreateIfNotFound) != CreateOptions.None)
      {
        fcs = new FrequentChatScore() { Jid = jid };
        if ((createOpts & (CreateOptions) 2) != CreateOptions.None)
        {
          this.InsertFrequentChatScore(fcs);
          if ((createOpts & (CreateOptions) 4) != CreateOptions.None)
            this.SubmitChanges();
        }
      }
      return fcs;
    }

    public string[] GetFrequentChats(
      FunXMPP.FMessage.Type msgType,
      int? limit,
      string[] excludedJids = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<object> objectList = new List<object>();
      stringBuilder.Append("SELECT Jid FROM FrequentChatScores ");
      if (excludedJids != null && ((IEnumerable<string>) excludedJids).Any<string>())
      {
        stringBuilder.Append("WHERE Jid NOT IN (");
        stringBuilder.Append(string.Join(", ", ((IEnumerable<string>) excludedJids).Select<string, string>((Func<string, string>) (jid => "?"))));
        stringBuilder.Append(") ");
        objectList.AddRange((IEnumerable<object>) excludedJids);
      }
      stringBuilder.Append("ORDER BY ");
      switch (msgType)
      {
        case FunXMPP.FMessage.Type.Image:
          stringBuilder.Append("ImageScore ");
          break;
        case FunXMPP.FMessage.Type.Video:
          stringBuilder.Append("VideoScore ");
          break;
        default:
          stringBuilder.Append("DefaultScore ");
          break;
      }
      stringBuilder.Append("DESC ");
      if (limit.HasValue)
      {
        int? nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          stringBuilder.Append("LIMIT ? ");
          objectList.Add((object) limit.Value);
        }
      }
      List<string> stringList = new List<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        while (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is string column)
            stringList.Add(column);
        }
      }
      return stringList.ToArray();
    }

    public Message[] GetMessagesByType(FunXMPP.FMessage.Type mediaWaType)
    {
      using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(new SqliteMessagesContext.MessageQueryArgs((string[]) null, true)
      {
        Types = new FunXMPP.FMessage.Type[1]{ mediaWaType }
      }))
        return messagesImpl.ToArray<Message>();
    }

    public int GetMessagesAfterCount(Message msg)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT count(MessageID) FROM Messages WHERE KeyRemoteJid = ? AND MessageID > ?"))
      {
        preparedStatement.Bind(0, msg.KeyRemoteJid);
        preparedStatement.Bind(1, msg.MessageID, false);
        if (preparedStatement.Step())
          return (int) (long) preparedStatement.Columns[0];
      }
      return 0;
    }

    private string CreateGetMessagesSql(
      SqliteMessagesContext.MessageQueryArgs args,
      List<object> bindParamsOut)
    {
      LinkedList<string> linkedList = new LinkedList<string>();
      string str = "";
      StringBuilder stringBuilder1 = new StringBuilder();
      if (args.CountOnly)
        stringBuilder1.Append("SELECT COUNT(*) FROM Messages\n");
      else if (args.IncludeMiscInfo)
      {
        stringBuilder1.Append("SELECT m.*, '#', misc.*\n");
        stringBuilder1.Append("FROM Messages as m \n");
        stringBuilder1.Append("LEFT JOIN MessageMiscInfos as misc ON m.MessageID = misc.MessageId \n");
        str = "m.";
      }
      else
        stringBuilder1.Append("SELECT * FROM Messages\n");
      if (args.Jids != null && ((IEnumerable<string>) args.Jids).Any<string>())
      {
        if (args.Jids.Length == 1)
        {
          linkedList.AddLast(string.Format("{0}KeyRemoteJid = ?", (object) str));
          bindParamsOut.Add((object) args.Jids[0]);
        }
        else if (args.Jids.Length > 1)
        {
          StringBuilder stringBuilder2 = new StringBuilder();
          stringBuilder2.AppendFormat("{0}KeyRemoteJid IN (", (object) str);
          bool flag = true;
          foreach (string jid in args.Jids)
          {
            if (flag)
              flag = false;
            else
              stringBuilder2.Append(", ");
            stringBuilder2.Append("?");
            bindParamsOut.Add((object) jid);
          }
          stringBuilder2.Append(")");
          linkedList.AddLast(stringBuilder2.ToString());
        }
      }
      linkedList.AddLast(string.Format("({0}Flags IS NULL OR {0}Flags <> ?)", (object) str));
      bindParamsOut.Add((object) 1);
      if (args.StarredOnly)
        linkedList.AddLast(string.Format("{0}IsStarred = 1", (object) str));
      if (args.LowerBoundMsgId.HasValue)
      {
        linkedList.AddLast(string.Format("{0}MessageID {1} ?", (object) str, args.IncludeLowerBound ? (object) ">=" : (object) ">"));
        bindParamsOut.Add((object) args.LowerBoundMsgId.Value);
      }
      else if (args.LowerBoundTimestampUtc.HasValue)
      {
        linkedList.AddLast(string.Format("{0}TimestampLong {1} ?", (object) str, args.IncludeLowerBound ? (object) ">=" : (object) ">"));
        bindParamsOut.Add((object) args.LowerBoundTimestampUtc.Value.ToUnixTime());
      }
      if (args.UpperBoundMsgId.HasValue)
      {
        linkedList.AddLast(string.Format("{0}MessageID {1} ?", (object) str, args.IncludeUpperBound ? (object) "<=" : (object) "<"));
        bindParamsOut.Add((object) args.UpperBoundMsgId.Value);
      }
      else if (args.UpperBoundTimestampUtc.HasValue)
      {
        linkedList.AddLast(string.Format("{0}TimestampLong {1} ?", (object) str, args.IncludeUpperBound ? (object) "<=" : (object) "<"));
        bindParamsOut.Add((object) args.UpperBoundTimestampUtc.Value.ToUnixTime());
      }
      if (args.WithSavedMediaOnly)
        linkedList.AddLast(str + "LocalFileUri IS NOT NULL");
      if (args.WithMediaOrigin.HasValue)
      {
        if (args.WithMediaOrigin.Value)
          linkedList.AddLast(str + "MediaOrigin IS NOT NULL");
        else
          linkedList.AddLast(str + "MediaOrigin IS NULL");
      }
      if (args.ParticipantJid != null)
      {
        linkedList.AddLast(string.Format("{0}RemoteResource = ?", (object) str));
        bindParamsOut.Add((object) args.ParticipantJid);
      }
      if (args.Types != null && ((IEnumerable<FunXMPP.FMessage.Type>) args.Types).Any<FunXMPP.FMessage.Type>())
      {
        if (args.Types.Length == 1)
        {
          linkedList.AddLast(str + "MediaWaType = ?");
          bindParamsOut.Add((object) (int) args.Types[0]);
        }
        else
        {
          StringBuilder stringBuilder3 = new StringBuilder();
          stringBuilder3.AppendFormat("{0}MediaWaType IN (", (object) str);
          bool flag = true;
          foreach (FunXMPP.FMessage.Type type in args.Types)
          {
            if (flag)
              flag = false;
            else
              stringBuilder3.Append(", ");
            stringBuilder3.Append("?");
            bindParamsOut.Add((object) (int) type);
          }
          stringBuilder3.Append(")");
          linkedList.AddLast(stringBuilder3.ToString());
        }
      }
      if (args.WithLocationDetails.HasValue)
      {
        if (args.WithLocationDetails.Value)
          linkedList.AddLast(str + "LocationDetails IS NOT NULL");
        else
          linkedList.AddLast(str + "LocationDetails IS NULL");
      }
      if (linkedList.Any<string>())
      {
        stringBuilder1.Append("WHERE ");
        stringBuilder1.Append(string.Join("\nAND ", (IEnumerable<string>) linkedList));
        stringBuilder1.Append("\n");
      }
      stringBuilder1.AppendFormat("ORDER BY {0}MessageID {1}\n", (object) str, args.Asc ? (object) "ASC" : (object) "DESC");
      if (args.Limit.HasValue && args.Limit.Value > 0)
        stringBuilder1.AppendFormat("LIMIT {0}\n", (object) args.Limit.Value);
      if (args.Offset.HasValue && args.Offset.Value > 0)
        stringBuilder1.AppendFormat("OFFSET {0}\n", (object) args.Offset.Value);
      return stringBuilder1.ToString();
    }

    public Message[] GetPendingMessages(string jid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM Messages WHERE KeyRemoteJid = ? AND KeyFromMe = 1 AND Status IN (?,?,?,?,?,?)"))
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, 20, false);
        stmt.Bind(2, 1, false);
        stmt.Bind(3, 16, false);
        stmt.Bind(4, 2, false);
        stmt.Bind(5, 9, false);
        stmt.Bind(6, 15, false);
        return this.ParseTable<Message>(stmt, "Messages").ToArray<Message>();
      }
    }

    public bool AnyIncomingMessagesFrom(string jid, bool includeSysMsgs)
    {
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      int? nullable = conversation == null ? new int?() : conversation.MessageLoadingStart();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      stringBuilder.Append("AND KeyFromMe = 0 \n");
      if (!includeSysMsgs)
        stringBuilder.Append("AND MediaWaType <> ? \n");
      if (nullable.HasValue)
        stringBuilder.Append("AND MessageID >= ? \n");
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement1 = stmt;
        int idx1 = num1;
        int num2 = idx1 + 1;
        string val1 = jid;
        preparedStatement1.Bind(idx1, val1);
        if (!includeSysMsgs)
          stmt.Bind(num2++, 7, false);
        if (nullable.HasValue)
        {
          Sqlite.PreparedStatement preparedStatement2 = stmt;
          int idx2 = num2;
          int num3 = idx2 + 1;
          int val2 = nullable.Value;
          preparedStatement2.Bind(idx2, val2, false);
        }
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
      }
    }

    public bool AnyOutgoingMessagesTo(string jid)
    {
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      int? nullable = conversation == null ? new int?() : conversation.MessageLoadingStart();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Messages \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      stringBuilder.Append("AND KeyFromMe = 1 \n");
      if (nullable.HasValue)
        stringBuilder.Append("AND MessageID >= ? \n");
      stringBuilder.Append("LIMIT 1 \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        stmt.Bind(0, jid);
        if (nullable.HasValue)
          stmt.Bind(1, nullable.Value, false);
        return this.ParseTable<Message>(stmt, "Messages").Any<Message>();
      }
    }

    public Message[] GetMessagesWithMediaHash(byte[] mediaHash, FunXMPP.FMessage.Type mediaType)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM Messages WHERE MediaHash = ? AND MediaWaType = ? \n"))
      {
        stmt.Bind(0, mediaHash);
        stmt.Bind(1, (long) mediaType, false);
        return this.ParseTable<Message>(stmt, "Messages").ToArray<Message>();
      }
    }

    private SqliteMessagesContext.IEnumerableWithDtor<Message> GetMessagesImpl(
      SqliteMessagesContext.MessageQueryArgs queryArgs)
    {
      List<object> bindParamsOut = new List<object>();
      Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(this.CreateGetMessagesSql(queryArgs, bindParamsOut));
      try
      {
        for (int index = 0; index < bindParamsOut.Count; ++index)
          preparedStatement.BindObject(index, bindParamsOut[index]);
        SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = new SqliteMessagesContext.IEnumerableWithDtor<Message>(!queryArgs.IncludeMiscInfo ? this.ParseTable<Message>(preparedStatement, "Messages") : this.ParseJoin<Message, MessageMiscInfo>(preparedStatement, "Messages", "MessageMiscInfos").Select<KeyValuePair<Message, MessageMiscInfo>, Message>((Func<KeyValuePair<Message, MessageMiscInfo>, Message>) (p =>
        {
          if (p.Value != null)
            p.Key.SetMiscInfo(p.Value);
          return p.Key;
        })), (IDisposable) preparedStatement);
        preparedStatement = (Sqlite.PreparedStatement) null;
        return messagesImpl;
      }
      finally
      {
        preparedStatement.SafeDispose();
      }
    }

    private IObservable<Message[]> GetMessagesObservableImpl(
      SqliteMessagesContext.MessageQueryArgs queryArgs)
    {
      return Observable.Create<Message[]>((Func<IObserver<Message[]>, Action>) (observer =>
      {
        bool queryFinished = false;
        object @lock = new object();
        Sqlite db = this.Db;
        try
        {
          string[] jids = queryArgs.Jids;
          if (jids != null && jids.Length == 1 && jids[0] != null)
          {
            Conversation conversation = this.GetConversation(jids[0], CreateOptions.None);
            if (conversation != null)
              queryArgs.LowerBoundMsgId = conversation.MessageLoadingStart();
          }
          using (SqliteMessagesContext.IEnumerableWithDtor<Message> messagesImpl = this.GetMessagesImpl(queryArgs))
          {
            lock (@lock)
              queryFinished = true;
            Message[] messageArray = (Message[]) null;
            if (jids != null && jids.Length > 1)
            {
              Dictionary<string, Conversation> convosInClearing = ((IEnumerable<Conversation>) ((IEnumerable<string>) jids).Where<string>((Func<string, bool>) (jid => jid != null)).MakeUnique<string>().Select<string, Conversation>((Func<string, Conversation>) (jid => this.GetConversation(jid, CreateOptions.None))).Where<Conversation>((Func<Conversation, bool>) (c => c != null)).ToArray<Conversation>()).Where<Conversation>((Func<Conversation, bool>) (c => c.IsInClearing())).ToDictionary<Conversation, string>((Func<Conversation, string>) (c => c.Jid));
              if (convosInClearing.Any<KeyValuePair<string, Conversation>>())
                messageArray = messagesImpl.Where<Message>((Func<Message, bool>) (m =>
                {
                  Conversation conversation = (Conversation) null;
                  return !convosInClearing.TryGetValue(m.KeyRemoteJid, out conversation) || m.MessageID >= conversation.EffectiveFirstMessageID.Value;
                })).ToArray<Message>();
            }
            if (messageArray == null)
              messageArray = messagesImpl.ToArray<Message>();
            observer.OnNext(messageArray);
          }
        }
        catch (Exception ex)
        {
          if ((int) ex.GetHResult() != (int) Sqlite.HRForError(9U))
            throw;
        }
        finally
        {
          observer.OnCompleted();
        }
        return (Action) (() =>
        {
          lock (@lock)
          {
            if (queryFinished)
            {
              Log.d("msgdb", "get media msgs | dispose | skip db interrupt");
            }
            else
            {
              Log.l("msgdb", "get media msgs | dispose | db interrupt");
              this.DisposeChildren();
              db.Interrupt();
              queryFinished = true;
            }
          }
        });
      }));
    }

    public IObservable<Message[]> GetMediaMessages(string[] jids, bool savedOnly, bool excludePtt)
    {
      return this.GetMessagesObservableImpl(new SqliteMessagesContext.MessageQueryArgs(jids, true)
      {
        WithSavedMediaOnly = savedOnly,
        WithMediaOrigin = excludePtt ? new bool?(false) : new bool?(),
        IncludeMiscInfo = false,
        Types = new FunXMPP.FMessage.Type[4]
        {
          FunXMPP.FMessage.Type.Audio,
          FunXMPP.FMessage.Type.Image,
          FunXMPP.FMessage.Type.Video,
          FunXMPP.FMessage.Type.Gif
        }
      });
    }

    public IObservable<Message[]> GetUrlMessages(string[] jids)
    {
      return this.GetMessagesObservableImpl(new SqliteMessagesContext.MessageQueryArgs(jids, true)
      {
        IncludeMiscInfo = false,
        WithLocationDetails = new bool?(true),
        Types = new FunXMPP.FMessage.Type[1]
        {
          FunXMPP.FMessage.Type.ExtendedText
        }
      });
    }

    public IObservable<Message[]> GetDocumentMessages(string[] jids)
    {
      return this.GetMessagesObservableImpl(new SqliteMessagesContext.MessageQueryArgs(jids, true)
      {
        IncludeMiscInfo = false,
        Types = new FunXMPP.FMessage.Type[1]
        {
          FunXMPP.FMessage.Type.Document
        }
      });
    }

    public long GetMessagesCount(string[] jids = null, bool starredOnly = false, FunXMPP.FMessage.Type[] types = null)
    {
      List<object> bindParamsOut = new List<object>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(this.CreateGetMessagesSql(new SqliteMessagesContext.MessageQueryArgs(jids, true)
      {
        CountOnly = true,
        StarredOnly = starredOnly,
        Types = types
      }, bindParamsOut)))
      {
        for (int index = 0; index < bindParamsOut.Count; ++index)
          preparedStatement.BindObject(index, bindParamsOut[index]);
        preparedStatement.Step();
        return (long) preparedStatement.Columns[0];
      }
    }

    public int GetIndexedMessagesCount()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT COUNT(*) FROM Messages WHERE FtsStatus > ?"))
      {
        preparedStatement.Bind(0, 0, false);
        preparedStatement.Step();
        return (int) (long) preparedStatement.Columns[0];
      }
    }

    public int GetIndexedMessagesUnsupportedCount()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT COUNT(*) FROM Messages WHERE FtsStatus = ?"))
      {
        preparedStatement.Bind(0, -2, false);
        preparedStatement.Step();
        return (int) (long) preparedStatement.Columns[0];
      }
    }

    public int GetIndexedMessagesErrorCount()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT COUNT(*) FROM Messages WHERE FtsStatus < ? AND FtsStatus != ?"))
      {
        preparedStatement.Bind(0, 0, false);
        preparedStatement.Bind(1, -2, false);
        preparedStatement.Step();
        return (int) (long) preparedStatement.Columns[0];
      }
    }

    public MessageMiscInfo GetMessageMiscInfo(int msgId, CreateOptions options = CreateOptions.None)
    {
      bool pendingSubmit = false;
      return this.GetMessageMiscInfo(msgId, out pendingSubmit, options);
    }

    public MessageMiscInfo GetMessageMiscInfo(
      int msgId,
      out bool pendingSubmit,
      CreateOptions options = CreateOptions.None)
    {
      pendingSubmit = false;
      MessageMiscInfo misc = (MessageMiscInfo) null;
      this.PrepareCachedStatement(this.getMsgMiscInfoStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, msgId, false);
        misc = this.ParseTable<MessageMiscInfo>(stmt, "MessageMiscInfos").SingleOrDefault<MessageMiscInfo>();
      }));
      if (misc == null && (options & CreateOptions.CreateIfNotFound) != CreateOptions.None)
      {
        misc = new MessageMiscInfo()
        {
          ID = 0,
          MessageId = new int?(msgId),
          ErrorCode = new int?(-1)
        };
        if ((options & (CreateOptions) 2) != CreateOptions.None)
        {
          this.Insert("MessageMiscInfos", (object) misc);
          if ((options & (CreateOptions) 4) != CreateOptions.None)
            this.SubmitChanges();
          else
            pendingSubmit = true;
        }
      }
      return misc;
    }

    public bool SaveMessageMiscInfoOnSubmit(MessageMiscInfo miscInfo)
    {
      bool flag = false;
      if (miscInfo.ID == 0)
      {
        this.Insert("MessageMiscInfos", (object) miscInfo);
        flag = true;
      }
      return flag;
    }

    public static IEnumerable<string> GetPossibleDbPathsForMediaFile(string path)
    {
      path = MediaStorage.GetAbsolutePath(path);
      List<string> pathsForMediaFile = new List<string>();
      pathsForMediaFile.Add(NativeMediaStorage.MakeUri(path));
      pathsForMediaFile.Add(path);
      if (path.StartsWith(Constants.IsoStorePath, StringComparison.InvariantCultureIgnoreCase))
      {
        string str = path.Substring(Constants.IsoStorePath.Length + 1);
        string[] strArray = new string[2]{ str, "\\" + str };
        pathsForMediaFile.AddRange(((IEnumerable<string>) strArray).Concat<string>(((IEnumerable<string>) strArray).Select<string, string>((Func<string, string>) (s => s.Replace('\\', '/')))));
      }
      return (IEnumerable<string>) pathsForMediaFile;
    }

    private List<LocalFile> GetLocalFilesByUri(string uri, bool firstOnly = true)
    {
      List<LocalFile> r = new List<LocalFile>();
      foreach (string str in SqliteMessagesContext.GetPossibleDbPathsForMediaFile(uri))
      {
        string path = str;
        this.PrepareCachedStatement(this.localFileStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
        {
          stmt.Bind(0, path);
          r.AddRange(this.ParseTable<LocalFile>(stmt, "LocalFiles"));
        }));
        if (firstOnly)
        {
          if (r.Count != 0)
            break;
        }
      }
      return r;
    }

    private static string NormalizeUri(string uri) => uri.ToLower();

    public void ScanLocalFiles(int offset = 0, Action<Action> schedule = null, Action onComplete = null)
    {
      int count = 0;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM LocalFiles ORDER BY LocalFileID DESC LIMIT ? OFFSET ?"))
      {
        stmt.Bind(0, 300, false);
        stmt.Bind(1, offset, false);
        foreach (LocalFile localFile in this.ParseTable<LocalFile>(stmt, "LocalFiles"))
        {
          ++count;
          foreach (string uri in SqliteMessagesContext.GetPossibleDbPathsForMediaFile(localFile.LocalFileUri))
            this.localFilesDict[SqliteMessagesContext.NormalizeUri(uri)] = localFile;
        }
      }
      if (count == 300)
      {
        (schedule ?? (Action<Action>) (a => WAThreadPool.QueueUserWorkItem(a)))((Action) (() => MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ScanLocalFiles(offset + count, schedule, onComplete)))));
      }
      else
      {
        Action action = onComplete;
        if (action == null)
          return;
        action();
      }
    }

    public LocalFile GetLocalFileByUri(string uri)
    {
      LocalFile localFileByUri;
      if (!this.localFilesDict.TryGetValue(SqliteMessagesContext.NormalizeUri(uri), out localFileByUri))
      {
        List<LocalFile> localFilesByUri = this.GetLocalFilesByUri(uri);
        if (localFilesByUri.Count != 0)
        {
          localFileByUri = localFilesByUri[0];
          foreach (string uri1 in SqliteMessagesContext.GetPossibleDbPathsForMediaFile(uri))
            this.localFilesDict[SqliteMessagesContext.NormalizeUri(uri1)] = localFileByUri;
        }
      }
      return localFileByUri;
    }

    private void PurgeLocalFileCache(IEnumerable<string> uriParams)
    {
      Set<string> possibleUris = new Set<string>(uriParams.SelectMany<string, string>((Func<string, IEnumerable<string>>) (uriParam => SqliteMessagesContext.GetPossibleDbPathsForMediaFile(uriParam).Select<string, string>(new Func<string, string>(SqliteMessagesContext.NormalizeUri)))));
      foreach (string key in possibleUris)
        this.localFilesDict.Remove(key);
      this.PurgeCache<LocalFile>("LocalFiles", (Func<LocalFile, bool>) (lf => possibleUris.Contains(SqliteMessagesContext.NormalizeUri(lf.LocalFileUri))));
    }

    private void PurgeLocalFileCache(string uriParam = null)
    {
      if (uriParam == null)
      {
        this.localFilesDict.Clear();
        this.PurgeCache("LocalFiles");
      }
      else
        this.PurgeLocalFileCache((IEnumerable<string>) new string[1]
        {
          uriParam
        });
    }

    public int LocalFileCountRef(string uri, LocalFileType fileType = LocalFileType.Unknown)
    {
      if (string.IsNullOrEmpty(uri))
        return 0;
      LocalFile localFileByUri = this.GetLocalFileByUri(uri);
      if (localFileByUri == null)
        return 0;
      int num;
      switch (fileType)
      {
        case LocalFileType.Thumbnail:
          num = localFileByUri.ThumbRefCount.HasValue ? localFileByUri.ThumbRefCount.Value : 0;
          break;
        case LocalFileType.StatusMedia:
          num = localFileByUri.StatusRefCount.HasValue ? localFileByUri.StatusRefCount.Value : 0;
          break;
        case LocalFileType.Sticker:
          num = localFileByUri.StickerRefCount.HasValue ? localFileByUri.StickerRefCount.Value : 0;
          break;
        case LocalFileType.QuotedMedia:
          num = localFileByUri.QuotedMediaRefCount.HasValue ? localFileByUri.QuotedMediaRefCount.Value : 0;
          break;
        case LocalFileType.Unknown:
          num = localFileByUri.ReferenceCount ?? 0;
          break;
        default:
          num = localFileByUri.MsgRefCount.HasValue ? localFileByUri.MsgRefCount.Value : 0;
          break;
      }
      return num;
    }

    public bool LocalFileAddRef(string uri, LocalFileType fileType, int count = 1)
    {
      if (string.IsNullOrEmpty(uri))
        return false;
      bool flag = false;
      LocalFile o = this.GetLocalFileByUri(uri);
      if (o == null)
      {
        o = new LocalFile()
        {
          LocalFileUri = uri,
          ReferenceCount = new int?(0),
          MsgRefCount = new int?(0),
          StatusRefCount = new int?(0),
          ThumbRefCount = new int?(0),
          StickerRefCount = new int?(0),
          QuotedMediaRefCount = new int?(0),
          FileType = new LocalFileType?(fileType)
        };
        this.Insert("LocalFiles", (object) o);
        this.localFilesDict[SqliteMessagesContext.NormalizeUri(uri)] = o;
        flag = true;
        AppState.ScheduleFileRehash(false);
      }
      int? nullable1;
      switch (fileType)
      {
        case LocalFileType.Thumbnail:
          LocalFile localFile1 = o;
          nullable1 = o.ThumbRefCount;
          int num1;
          if (!nullable1.HasValue)
          {
            num1 = count;
          }
          else
          {
            nullable1 = o.ThumbRefCount;
            num1 = nullable1.Value + count;
          }
          int? nullable2 = new int?(num1);
          localFile1.ThumbRefCount = nullable2;
          break;
        case LocalFileType.StatusMedia:
          LocalFile localFile2 = o;
          nullable1 = o.StatusRefCount;
          int num2;
          if (!nullable1.HasValue)
          {
            num2 = count;
          }
          else
          {
            nullable1 = o.StatusRefCount;
            num2 = nullable1.Value + count;
          }
          int? nullable3 = new int?(num2);
          localFile2.StatusRefCount = nullable3;
          break;
        case LocalFileType.Sticker:
          LocalFile localFile3 = o;
          nullable1 = o.StickerRefCount;
          int num3;
          if (!nullable1.HasValue)
          {
            num3 = count;
          }
          else
          {
            nullable1 = o.StickerRefCount;
            num3 = nullable1.Value + count;
          }
          int? nullable4 = new int?(num3);
          localFile3.StickerRefCount = nullable4;
          break;
        case LocalFileType.QuotedMedia:
          LocalFile localFile4 = o;
          nullable1 = o.QuotedMediaRefCount;
          int num4;
          if (!nullable1.HasValue)
          {
            num4 = count;
          }
          else
          {
            nullable1 = o.QuotedMediaRefCount;
            num4 = nullable1.Value + count;
          }
          int? nullable5 = new int?(num4);
          localFile4.QuotedMediaRefCount = nullable5;
          break;
        default:
          LocalFile localFile5 = o;
          nullable1 = o.MsgRefCount;
          int num5;
          if (!nullable1.HasValue)
          {
            num5 = count;
          }
          else
          {
            nullable1 = o.MsgRefCount;
            num5 = nullable1.Value + count;
          }
          int? nullable6 = new int?(num5);
          localFile5.MsgRefCount = nullable6;
          o.FileType = new LocalFileType?(LocalFileType.MessageMedia);
          break;
      }
      LocalFile localFile6 = o;
      nullable1 = localFile6.ReferenceCount;
      int num6 = count;
      localFile6.ReferenceCount = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + num6) : new int?();
      return flag;
    }

    public bool LocalFileRelease(
      string uri,
      LocalFileType fileType,
      IsoStoreMediaStorage isoStore = null,
      NativeMediaStorage nativeStore = null,
      bool deleteNative = false)
    {
      if (string.IsNullOrEmpty(uri))
        return false;
      bool flag1 = (MediaStorage.AnalyzePath(uri).Root & FileRoot.StorageMask) != 0;
      bool flag2 = false;
      bool flag3 = true;
      bool flag4 = false;
      int num1 = 0;
      int num2 = 0;
      LocalFile localFileByUri = this.GetLocalFileByUri(uri);
      if (localFileByUri != null)
      {
        int num3 = 0;
        int num4 = 0;
        switch (fileType)
        {
          case LocalFileType.MessageMedia:
            if (localFileByUri.MsgRefCount.HasValue)
            {
              num3 = localFileByUri.MsgRefCount.Value;
              localFileByUri.MsgRefCount = new int?(num4 = Math.Max(num3 - 1, 0));
              if (num4 == 0 && localFileByUri.StatusRefCount.HasValue && localFileByUri.StatusRefCount.Value > 0)
              {
                localFileByUri.FileType = new LocalFileType?(LocalFileType.StatusMedia);
                flag2 = true;
                break;
              }
              break;
            }
            break;
          case LocalFileType.Thumbnail:
            if (localFileByUri.ThumbRefCount.HasValue)
            {
              num3 = localFileByUri.ThumbRefCount.Value;
              localFileByUri.ThumbRefCount = new int?(num4 = Math.Max(num3 - 1, 0));
              break;
            }
            break;
          case LocalFileType.StatusMedia:
            if (localFileByUri.StatusRefCount.HasValue)
            {
              num3 = localFileByUri.StatusRefCount.Value;
              localFileByUri.StatusRefCount = new int?(num4 = Math.Max(num3 - 1, 0));
              break;
            }
            break;
          case LocalFileType.Sticker:
            if (localFileByUri.StickerRefCount.HasValue)
            {
              num3 = localFileByUri.StickerRefCount.Value;
              localFileByUri.StickerRefCount = new int?(num4 = Math.Max(num3 - 1, 0));
              break;
            }
            break;
          case LocalFileType.QuotedMedia:
            if (localFileByUri.QuotedMediaRefCount.HasValue)
            {
              num3 = localFileByUri.QuotedMediaRefCount.Value;
              localFileByUri.QuotedMediaRefCount = new int?(num4 = Math.Max(num3 - 1, 0));
              break;
            }
            break;
          case LocalFileType.Unknown:
            if (!localFileByUri.ThumbRefCount.HasValue || localFileByUri.ThumbRefCount.Value <= 0)
            {
              if (!localFileByUri.StatusRefCount.HasValue || localFileByUri.StatusRefCount.Value <= 0)
              {
                if (!localFileByUri.StickerRefCount.HasValue || localFileByUri.StickerRefCount.Value <= 0)
                {
                  if (!localFileByUri.QuotedMediaRefCount.HasValue || localFileByUri.QuotedMediaRefCount.Value <= 0)
                    goto case LocalFileType.MessageMedia;
                  else
                    goto case LocalFileType.QuotedMedia;
                }
                else
                  goto case LocalFileType.Sticker;
              }
              else
                goto case LocalFileType.StatusMedia;
            }
            else
              goto case LocalFileType.Thumbnail;
        }
        if (num4 != num3)
          flag2 = true;
        int? nullable = localFileByUri.MsgRefCount;
        int num5 = 0 + (nullable ?? 0);
        nullable = localFileByUri.StatusRefCount;
        int num6 = nullable ?? 0;
        int num7 = num5 + num6;
        nullable = localFileByUri.StickerRefCount;
        int num8 = nullable ?? 0;
        int num9 = num7 + num8;
        nullable = localFileByUri.QuotedMediaRefCount;
        int num10 = nullable ?? 0;
        int num11 = num9 + num10;
        nullable = localFileByUri.ThumbRefCount;
        int num12 = nullable ?? 0;
        num2 = num11 + num12;
        nullable = localFileByUri.ReferenceCount;
        num1 = nullable ?? 0;
        if (num2 != num1)
        {
          localFileByUri.ReferenceCount = new int?(num2);
          flag2 = true;
        }
        if (num2 > 0)
          flag3 = false;
      }
      if (flag3)
      {
        if (!flag1 | deleteNative)
        {
          try
          {
            IMediaStorage mediaStorage1 = flag1 ? (IMediaStorage) nativeStore : (IMediaStorage) isoStore;
            if (mediaStorage1 == null)
            {
              using (IMediaStorage mediaStorage2 = MediaStorage.Create(uri))
                mediaStorage2.DeleteFile(uri);
            }
            else
              mediaStorage1.DeleteFile(uri);
            flag4 = true;
          }
          catch (Exception ex)
          {
            Log.l("msg db", "failed to delete file: {0}", (object) uri);
          }
        }
      }
      if (flag3 && localFileByUri != null)
      {
        this.Delete("LocalFiles", (object) localFileByUri);
        this.localFilesDict.Remove(SqliteMessagesContext.NormalizeUri(uri));
        flag2 = true;
      }
      Log.d("local file", "release | do delete:{0} deleted:{1}, ref:{2}->{3}, uri:{4}", (object) flag3, (object) flag4, (object) num1, (object) num2, (object) uri);
      return flag2;
    }

    public void LocalFileRename(FileRef oldRef, FileRef newRef, byte[] sha1Hash)
    {
      IEnumerable<string> pathsForMediaFile = SqliteMessagesContext.GetPossibleDbPathsForMediaFile(oldRef.ToAbsolutePath());
      string val1 = NativeMediaStorage.MakeUri(newRef.ToAbsolutePath());
      List<string> first = new List<string>();
      int num = 0;
      long ticks1 = DateTime.Now.Ticks;
      this.BeginTransaction();
      bool flag = true;
      try
      {
        long ticks2 = DateTime.Now.Ticks;
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE LocalFiles SET LocalFileUri = ? WHERE LocalFileUri = ? COLLATE NOCASE AND Sha1Hash = ?"))
        {
          foreach (string val2 in pathsForMediaFile)
          {
            preparedStatement.Bind(0, val1);
            preparedStatement.Bind(1, val2);
            preparedStatement.Bind(2, sha1Hash);
            preparedStatement.Step();
            if (this.Db.GetChangeCount() > 0)
              first.Add(val2);
            preparedStatement.Reset();
          }
        }
        long ticks3 = DateTime.Now.Ticks;
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE Messages SET LocalFileUri = ? WHERE LocalFileUri = ?"))
        {
          foreach (string val3 in first)
          {
            preparedStatement.Bind(0, val1);
            preparedStatement.Bind(1, val3);
            preparedStatement.Step();
            num += this.Db.GetChangeCount();
            preparedStatement.Reset();
          }
        }
        long ticks4 = DateTime.Now.Ticks;
        this.CommitTransaction();
        flag = false;
        DateTime now = DateTime.Now;
        long ticks5 = now.Ticks;
        this.PurgeLocalFileCache(first.Concat<string>((IEnumerable<string>) new string[1]
        {
          val1
        }));
        if (num > 0)
          Log.l("local file", "applied rename from {0} to {1}", (object) oldRef.ToString(), (object) newRef.ToString());
        now = DateTime.Now;
        if (now.Ticks - ticks1 <= 10000000L)
          return;
        object[] objArray = new object[5]
        {
          (object) ((ticks2 - ticks1) / 10000L),
          (object) ((ticks3 - ticks2) / 10000L),
          (object) ((ticks4 - ticks3) / 10000L),
          (object) ((ticks5 - ticks4) / 10000L),
          null
        };
        now = DateTime.Now;
        objArray[4] = (object) ((now.Ticks - ticks5) / 10000L);
        Log.l("local file", "rename took too long {0}, {1}, {2}, {3}, {4}", objArray);
      }
      catch (Exception ex)
      {
        Log.l(nameof (LocalFileRename), "Exception {0}, last sql:{1}, inTx:{2}", (object) ex.GetFriendlyMessage(), (object) this.Db.MaybeLastPreparedStatementString, (object) flag);
        if (flag)
          this.RollbackTransaction(ex);
        throw;
      }
    }

    private static SqliteMessagesContext.LocalFileHash ComputeLocalFileHash(string localFileUri)
    {
      byte[] numArray = (byte[]) null;
      long? nullable = new long?();
      try
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(localFileUri))
        {
          if (mediaStorage.FileExists(localFileUri))
          {
            using (Stream inputStream = mediaStorage.OpenFile(localFileUri))
            {
              using (SHA1Managed shA1Managed = new SHA1Managed())
              {
                numArray = shA1Managed.ComputeHash(inputStream);
                nullable = new long?(inputStream.Length);
              }
            }
          }
          else
          {
            Log.WriteLineDebug("local file hash error: file does not exist");
            numArray = new byte[0];
            nullable = new long?(-1L);
          }
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        Log.WriteLineDebug("local file hash error: directory not found");
        numArray = new byte[0];
        nullable = new long?(-1L);
      }
      catch (FileNotFoundException ex)
      {
        Log.WriteLineDebug("local file hash error: file not found");
        numArray = new byte[0];
        nullable = new long?(-1L);
      }
      catch (Exception ex)
      {
        Log.WriteLineDebug("local file hash error: " + ex.ToString());
        Log.LogException(ex, "local file hash");
      }
      return new SqliteMessagesContext.LocalFileHash()
      {
        Hash = numArray,
        FileSize = nullable
      };
    }

    private static long? ComputeLocalFileSize(string localFileUri)
    {
      long? localFileSize = new long?();
      try
      {
        using (IMediaStorage mediaStorage = MediaStorage.Create(localFileUri))
        {
          if (mediaStorage.FileExists(localFileUri))
          {
            using (Stream stream = mediaStorage.OpenFile(localFileUri))
              localFileSize = new long?(stream.Length);
          }
          else
          {
            Log.WriteLineDebug("local file size error: file does not exist");
            localFileSize = new long?(-1L);
          }
        }
      }
      catch (DirectoryNotFoundException ex)
      {
        Log.WriteLineDebug("local file size error: directory not found");
        localFileSize = new long?(-1L);
      }
      catch (FileNotFoundException ex)
      {
        Log.WriteLineDebug("local file size error: file not found");
        localFileSize = new long?(-1L);
      }
      catch (Exception ex)
      {
        Log.WriteLineDebug("local file size error: " + ex.ToString());
        Log.LogException(ex, "local file size");
      }
      return localFileSize;
    }

    private void GetIncompleteLocalFiles(
      LocalFileType fileType,
      int? limit,
      out List<string> filesWithoutHashes,
      out List<string> filesWithoutSizes)
    {
      filesWithoutHashes = new List<string>();
      filesWithoutSizes = new List<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT LocalFileUri, Sha1Hash, FileSize FROM LocalFiles WHERE (Sha1Hash IS NULL OR FileSize IS NULL) AND FileType = " + (object) (int) fileType + " " + (limit.HasValue ? (object) ("LIMIT " + (object) limit.Value + " ") : (object) "")))
      {
        while (preparedStatement.Step())
        {
          string column1 = preparedStatement.Columns[0] as string;
          object column2 = preparedStatement.Columns[1];
          object column3 = preparedStatement.Columns[2];
          if (column1 != null)
          {
            if (column2 != null && column3 == null)
              filesWithoutSizes.Add(column1);
            else
              filesWithoutHashes.Add(column1);
          }
        }
      }
    }

    public void FindAndUpdateLocalFileHashes(Action finishedBatch, Action finishedAll)
    {
      List<string> filesWithoutHashes = (List<string>) null;
      List<string> filesWithoutSizes = (List<string>) null;
      int filesToCorrect = 0;
      this.GetIncompleteLocalFiles(LocalFileType.MessageMedia, new int?(100), out filesWithoutHashes, out filesWithoutSizes);
      List<string> stringList1 = filesWithoutHashes;
      // ISSUE: explicit non-virtual call
      int count1 = stringList1 != null ? __nonvirtual (stringList1.Count) : 0;
      List<string> stringList2 = filesWithoutSizes;
      // ISSUE: explicit non-virtual call
      int count2 = stringList2 != null ? __nonvirtual (stringList2.Count) : 0;
      filesToCorrect = count1 + count2;
      if (filesToCorrect == 0)
      {
        finishedAll();
      }
      else
      {
        Action decrementCount = (Action) (() =>
        {
          if (Interlocked.Decrement(ref filesToCorrect) != 0)
            return;
          GC.Collect();
          finishedBatch();
        });
        WorkQueue workerThread = Utils.LazyInit<WorkQueue>(ref SqliteMessagesContext.hashWorkerThread, (Func<WorkQueue>) (() => new WorkQueue(flags: WorkQueue.StartFlags.WatchdogExcempt)));
        ThreadPool.QueueUserWorkItem((WaitCallback) (ignored =>
        {
          List<string> stringList3 = filesWithoutHashes;
          // ISSUE: explicit non-virtual call
          if ((stringList3 != null ? (__nonvirtual (stringList3.Count) > 0 ? 1 : 0) : 0) != 0)
          {
            Log.l("local file", "build hashes | collected {0} unhashed file URIs", (object) filesWithoutHashes.Count);
            int num1 = 0;
            int num2 = 0;
            foreach (string str in filesWithoutHashes)
            {
              string filename = str;
              ++num1;
              SqliteMessagesContext.LocalFileHash fileHash = new SqliteMessagesContext.LocalFileHash();
              NativeInterfaces.Misc.LowerPriority(((Action) (() => fileHash = SqliteMessagesContext.ComputeLocalFileHash(filename))).AsComAction());
              try
              {
                if (fileHash.Hash != null)
                {
                  string filenameSnap = filename;
                  workerThread.Enqueue((Action) (() =>
                  {
                    MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.UpdateLocalFileHash(filenameSnap, fileHash.Hash, fileHash.FileSize)));
                    decrementCount();
                  }));
                }
                else
                {
                  ++num2;
                  decrementCount();
                }
              }
              catch (Exception ex)
              {
                Log.WriteLineDebug("failed to commit updated file hash" + ex.ToString());
                break;
              }
            }
            Log.l("local file", "build hashes | populated {0} of {1} file hashes", (object) (num1 - num2), (object) num1);
          }
          List<string> stringList4 = filesWithoutSizes;
          // ISSUE: explicit non-virtual call
          if ((stringList4 != null ? (__nonvirtual (stringList4.Count) > 0 ? 1 : 0) : 0) == 0)
            return;
          Log.l("local file", "build hashes | collected {0} unsized file URIs", (object) filesWithoutSizes.Count);
          int num3 = 0;
          int num4 = 0;
          foreach (string localFileUri in filesWithoutSizes)
          {
            ++num3;
            long? fileSize = SqliteMessagesContext.ComputeLocalFileSize(localFileUri);
            try
            {
              if (fileSize.HasValue)
              {
                string filenameSnap = localFileUri;
                workerThread.Enqueue((Action) (() =>
                {
                  MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.UpdateLocalFileSize(filenameSnap, fileSize.Value)));
                  decrementCount();
                }));
                Log.d("local file", "build hashes | added size for file {0}/{1}", (object) num3, (object) filesWithoutSizes.Count);
              }
              else
              {
                ++num4;
                decrementCount();
              }
            }
            catch (Exception ex)
            {
              Log.WriteLineDebug("failed to commit updated file size" + ex.ToString());
              break;
            }
          }
          Log.l("local file", "build hashes | populated {0} of {1} file sizes", (object) (num3 - num4), (object) num3);
        }));
      }
    }

    internal void UpdateLocalFileHash(string localFileUri, byte[] sha1Hash, long? fileSize)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE LocalFiles SET Sha1Hash = ?, FileSize = ? WHERE LocalFileUri = ?"))
      {
        preparedStatement.Bind(0, sha1Hash);
        if (fileSize.HasValue)
          preparedStatement.Bind(1, fileSize.Value, false);
        else
          preparedStatement.Bind(1);
        preparedStatement.Bind(2, localFileUri);
        preparedStatement.Step();
      }
      this.PurgeLocalFileCache(localFileUri);
    }

    internal void UpdateLocalFileSize(string localFileUri, long fileSize)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE LocalFiles SET FileSize = ? WHERE LocalFileUri = ?"))
      {
        preparedStatement.Bind(0, fileSize, false);
        preparedStatement.Bind(1, localFileUri);
        preparedStatement.Step();
      }
      this.PurgeLocalFileCache(localFileUri);
    }

    public void ClearLocalFileHashes()
    {
      Log.l("local file", "clear hashes | clearing file hashes");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE LocalFiles SET Sha1Hash = NULL WHERE IfNull(Length(Sha1Hash), 1) <> 0"))
        preparedStatement.Step();
      Log.l("local file", "clear hashes | cleared all file hashes, count = {0}", (object) this.Db.GetChangeCount());
      this.PurgeLocalFileCache();
    }

    public void InsertConversationOnSubmit(Conversation c)
    {
      this.Insert("Conversations", (object) c);
      this.convoCache[c.Jid] = c;
    }

    public void DeleteConversationOnSubmit(Conversation c)
    {
      if (c == null)
        return;
      this.Delete("Conversations", (object) c);
      JidInfo jidInfo = this.GetJidInfo(c.Jid, CreateOptions.None);
      if (jidInfo == null)
        return;
      if (JidHelper.IsUserJid(c.Jid))
        jidInfo.OnChatDeleted();
      else
        this.DeleteJidInfoOnSubmit(jidInfo);
    }

    public void ClearUnreadTileCounts()
    {
      StringBuilder stringBuilder = new StringBuilder("UPDATE Conversations SET UnreadTileCount = 0 \n");
      stringBuilder.Append("WHERE UnreadTileCount > 0");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
        preparedStatement.Step();
      SqliteDataContext.TableState tableState = (SqliteDataContext.TableState) null;
      foreach (Conversation cachedObject in this.GetCachedObjects<Conversation>("Conversations"))
      {
        if (cachedObject.UnreadTileCount > 0L)
        {
          if (tableState == null)
            tableState = this.GetTable("Conversations");
          SqliteDataContext.ObjectState objectState = (SqliteDataContext.ObjectState) null;
          if (tableState.GetObject((object) (long) cachedObject.ConversationID, out objectState))
          {
            objectState.TrackingChanges = false;
            cachedObject.UnreadTileCount = 0L;
            objectState.TrackingChanges = true;
          }
        }
      }
    }

    public long GetUnreadTileCountsSum()
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT UnreadTileCount FROM Conversations \n");
      stringBuilder.Append("WHERE UnreadTileCount > 0");
      long unreadTileCountsSum = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        while (preparedStatement.Step())
        {
          long column = (long) preparedStatement.Columns[0];
          unreadTileCountsSum += column;
        }
      }
      return unreadTileCountsSum;
    }

    public void InsertGroupParticipantStateOnSubmit(GroupParticipantState s)
    {
      this.Insert("GroupParticipants", (object) s);
    }

    public void DeleteGroupParticipantStateOnSubmit(GroupParticipantState s)
    {
      this.Delete("GroupParticipants", (object) s);
    }

    public Dictionary<string, GroupParticipantState> GetParticipants(string gjid, bool excludeSelf)
    {
      Dictionary<string, GroupParticipantState> participants = new Dictionary<string, GroupParticipantState>();
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM GroupParticipants WHERE GroupJid = ?"))
      {
        stmt.Bind(0, gjid);
        foreach (GroupParticipantState participantState in this.ParseTable<GroupParticipantState>(stmt, "GroupParticipants"))
          participants[participantState.MemberJid] = participantState;
      }
      int participantsCount = this.GetParticipantsCount(gjid, false);
      if (participants.Count != participantsCount)
      {
        Dictionary<string, GroupParticipantState> dictionary = new Dictionary<string, GroupParticipantState>();
        foreach (GroupParticipantState groupParticipant in this.GetAllGroupParticipants())
        {
          if (groupParticipant.GroupJid == gjid)
            dictionary[groupParticipant.MemberJid] = groupParticipant;
        }
        if (participants.Count != dictionary.Count)
        {
          Log.l("participants[" + gjid + "]", "Participant count mismatch!  {0} in first query, {1} in SELECT COUNT(*), {2} in all db", (object) participants.Count, (object) participantsCount, (object) dictionary.Count);
          Log.SendCrashLog(new Exception("Group participant issue (workaround applied)"), "group participants", logOnlyForRelease: true);
        }
        participants = dictionary;
      }
      if (excludeSelf)
        participants.Remove(Settings.MyJid);
      return participants;
    }

    public int GetParticipantsCount(string jid, bool excludeSelf)
    {
      int val2 = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT COUNT(*) FROM GroupParticipants WHERE GroupJid = ?"))
      {
        preparedStatement.Bind(0, jid);
        if (preparedStatement.Step())
          val2 = (int) (long) preparedStatement.Columns[0];
      }
      if (excludeSelf)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT EXISTS (SELECT * FROM GroupParticipants WHERE GroupJid = ? AND MemberJid = ?)"))
        {
          preparedStatement.Bind(0, jid);
          preparedStatement.Bind(1, Settings.MyJid);
          preparedStatement.Step();
          if ((long) preparedStatement.Columns[0] != 0L)
            --val2;
        }
      }
      return Math.Max(0, val2);
    }

    public GroupParticipantState[] GetAllGroupParticipants()
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM GroupParticipants"))
        return this.ParseTable<GroupParticipantState>(stmt, "GroupParticipants").ToArray<GroupParticipantState>();
    }

    public string[] GetAllGroupParticipantsJids()
    {
      HashSet<string> source = new HashSet<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT MemberJid FROM GroupParticipants"))
      {
        while (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is string column)
            source.Add(column);
        }
      }
      return source.ToArray<string>();
    }

    public GroupParticipantState GetParticipant(string jid, string pJid)
    {
      return this.GetParticipants(jid, false).GetValueOrDefault<string, GroupParticipantState>(pJid);
    }

    public IEnumerable<string> GetGroupsWithParticipants(IEnumerable<string> participantJids)
    {
      string[] array;
      if (participantJids == null || (array = participantJids.ToArray<string>()).Length == 0)
        return (IEnumerable<string>) new string[0];
      Set<string> withParticipants = new Set<string>();
      int num1 = 0;
      while (num1 < array.Length)
      {
        StringBuilder stringBuilder = new StringBuilder();
        int num2 = num1;
        int num3;
        for (num3 = Math.Min(num1 + 999, array.Length); num1 < num3; ++num1)
        {
          if (stringBuilder.Length == 0)
            stringBuilder.Append("SELECT DISTINCT GroupJid FROM GroupParticipants WHERE MemberJid IN (?");
          else
            stringBuilder.Append(", ?");
        }
        stringBuilder.Append(')');
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
        {
          for (int index = num2; index < num3; ++index)
            preparedStatement.Bind(index - num2, array[index]);
          while (preparedStatement.Step())
            withParticipants.Add(preparedStatement.Columns[0] as string);
        }
      }
      return (IEnumerable<string>) withParticipants;
    }

    public string[] GetGroupsInCommonJids(string[] participantJids)
    {
      if (participantJids == null || !((IEnumerable<string>) participantJids).Any<string>())
        return new string[0];
      StringBuilder stringBuilder = new StringBuilder("SELECT G1.GroupJid FROM GroupParticipants AS G1 ");
      stringBuilder.Append("JOIN GroupParticipants as G2 ON G2.GroupJid = G1.GroupJid ");
      stringBuilder.Append("WHERE G1.MemberJid IN (");
      stringBuilder.Append(string.Join(", ", ((IEnumerable<string>) participantJids).Select<string, string>((Func<string, string>) (pJid => "?")).ToArray<string>()));
      stringBuilder.Append(") ");
      stringBuilder.Append("AND G2.MemberJid = ?");
      HashSet<string> source = new HashSet<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int idx = 0;
        foreach (string participantJid in participantJids)
          preparedStatement.Bind(idx++, participantJid);
        preparedStatement.Bind(idx, Settings.MyJid);
        while (preparedStatement.Step())
          source.Add((string) preparedStatement.Columns[0]);
      }
      return source.ToArray<string>();
    }

    public Conversation[] GetGroupsInCommon(string[] participantJids)
    {
      string[] groupsInCommonJids = this.GetGroupsInCommonJids(participantJids);
      if (groupsInCommonJids == null || !((IEnumerable<string>) groupsInCommonJids).Any<string>())
        return new Conversation[0];
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Conversations WHERE Jid IN (");
      stringBuilder.Append(string.Join(", ", ((IEnumerable<string>) groupsInCommonJids).Select<string, string>((Func<string, string>) (gJid => "?")).ToArray<string>()));
      stringBuilder.Append(")");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (string val in groupsInCommonJids)
          stmt.Bind(num++, val);
        return this.ParseTable<Conversation>(stmt, "Conversations").ToArray<Conversation>();
      }
    }

    public GroupParticipantState[] GetGroupParticipantsInCommonGroups(string participantJid)
    {
      if (participantJid == null)
        return new GroupParticipantState[0];
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM GroupParticipants AS G1 ");
      stringBuilder.Append("JOIN GroupParticipants as G2 ON G2.GroupJid = G1.GroupJid ");
      stringBuilder.Append("WHERE G1.MemberJid = ? AND G2.MemberJid = ?");
      LinkedList<string> linkedList = new LinkedList<string>();
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement1 = stmt;
        int idx1 = num1;
        int num2 = idx1 + 1;
        string myJid = Settings.MyJid;
        preparedStatement1.Bind(idx1, myJid);
        Sqlite.PreparedStatement preparedStatement2 = stmt;
        int idx2 = num2;
        int num3 = idx2 + 1;
        string val = participantJid;
        preparedStatement2.Bind(idx2, val);
        return this.ParseTable<GroupParticipantState>(stmt, "GroupParticipants").ToArray<GroupParticipantState>();
      }
    }

    public string[] GetAdminGroups()
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT GroupJid FROM GroupParticipants ");
      stringBuilder.Append("WHERE MemberJid = ? ");
      stringBuilder.Append("AND Flags IS NOT NULL ");
      stringBuilder.Append("AND Flags <> 0");
      LinkedList<string> source = new LinkedList<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        preparedStatement.Bind(0, Settings.MyJid);
        while (preparedStatement.Step())
          source.AddLast((string) preparedStatement.Columns[0]);
        return source.ToArray<string>();
      }
    }

    public string[] GetAllConversationJids()
    {
      List<string> r = new List<string>();
      this.PrepareCachedStatement(this.allConvoJidsStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        while (stmt.Step())
          r.Add((string) stmt.Columns[0]);
      }));
      return r.ToArray();
    }

    private Conversation[] GetConvoList(CachedStatement cachedStmt)
    {
      Conversation[] r = (Conversation[]) null;
      this.PrepareCachedStatement(cachedStmt, (Action<Sqlite.PreparedStatement>) (stmt => r = this.ParseTable<Conversation>(stmt, "Conversations").ToArray<Conversation>()));
      return r;
    }

    public Conversation[] GetNonEmptyConversations() => this.GetConvoList(this.nonemptyCovnosStmt);

    public Conversation[] GetPinnedConversations() => this.GetConvoList(this.pinnedConvosStmt);

    public Conversation[] GetConversationsToDeleteOrClear()
    {
      return this.GetConvoList(this.toDeleteConvosStmt);
    }

    public List<ConversationItem> GetArchivedConversationItems()
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT c.*, '#', m.*\n");
      stringBuilder.Append("FROM Conversations as c\n");
      stringBuilder.Append("LEFT JOIN Messages as m\n");
      stringBuilder.Append("ON c.LastMessageID = m.MessageID\n");
      stringBuilder.Append("WHERE c.IsArchived = 1\n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
        return this.RemoveInvalidJids(this.ParseJoin<Conversation, Message>(stmt, "Conversations", "Messages").Select<KeyValuePair<Conversation, Message>, ConversationItem>((Func<KeyValuePair<Conversation, Message>, ConversationItem>) (p => new ConversationItem(p.Key)
        {
          Message = p.Value
        })).ToList<ConversationItem>());
    }

    private string CreateConversationQuerySqlWhere(
      JidHelper.JidTypes[] includeTypes,
      bool includeArchived,
      string conversationTablePrefix,
      out List<object> bindedParams)
    {
      bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("WHERE \n");
      stringBuilder.Append(string.Format("({0}Status IS NULL OR {0}Status <> ?) \n", (object) (conversationTablePrefix ?? "")));
      bindedParams.Add((object) 1);
      if (!includeArchived)
        stringBuilder.AppendFormat("AND ({0}IsArchived IS NULL OR {0}IsArchived = 0) \n", (object) (conversationTablePrefix ?? ""));
      List<int> list = includeTypes.Cast<int>().ToList<int>();
      if (list.Any<int>())
      {
        if (list.Count == 1)
        {
          stringBuilder.AppendFormat("AND {0}JidType = ? \n", (object) (conversationTablePrefix ?? ""));
          bindedParams.Add((object) list.First<int>());
        }
        else
        {
          stringBuilder.AppendFormat("AND {0}JidType IN ({1}) \n", (object) (conversationTablePrefix ?? ""), (object) string.Join(", ", list.Select<int, string>((Func<int, string>) (_ => "?"))));
          foreach (int num in list)
            bindedParams.Add((object) num);
        }
      }
      else
      {
        int[] source = new int[2]{ 4, 0 };
        stringBuilder.AppendFormat("AND {0}JidType NOT IN ({1}) \n", (object) (conversationTablePrefix ?? ""), (object) string.Join(", ", ((IEnumerable<int>) source).Select<int, string>((Func<int, string>) (_ => "?"))));
        foreach (int num in source)
          bindedParams.Add((object) num);
      }
      return stringBuilder.ToString();
    }

    public int GetConversationsCount(JidHelper.JidTypes[] includeTypes, bool includeArchived)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(ConversationID) FROM Conversations \n");
      stringBuilder.Append(this.CreateConversationQuerySqlWhere(includeTypes, includeArchived, (string) null, out bindedParams));
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in bindedParams)
          preparedStatement.BindObject(num++, o);
        preparedStatement.Step();
        int result = 0;
        return int.TryParse(string.Format("{0}", preparedStatement.Columns[0]), out result) && result > 0 ? result : 0;
      }
    }

    public int GetGroupsCount()
    {
      return this.GetConversationsCount(new JidHelper.JidTypes[1]
      {
        JidHelper.JidTypes.Group
      }, true);
    }

    public int GetUnreadConversationsCount(bool includingArchived)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(ConversationID) FROM Conversations \n");
      stringBuilder.Append("WHERE UnreadMessageCount IS NOT NULL AND UnreadMessageCount <> 0 \n");
      stringBuilder.Append("AND (Status IS NULL OR Status <> ?) \n");
      objectList.Add((object) 1);
      if (!includingArchived)
        stringBuilder.Append("AND (IsArchived IS NULL OR IsArchived = 0) \n");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          preparedStatement.BindObject(num++, o);
        preparedStatement.Step();
        int result = 0;
        if (int.TryParse(string.Format("{0}", preparedStatement.Columns[0]), out result))
        {
          if (result > 0)
            return result;
        }
      }
      return 0;
    }

    public List<Conversation> GetConversations(
      JidHelper.JidTypes[] includeTypes,
      bool includeArchived,
      bool sortByTimestamp = false,
      int? limit = null)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Conversations\n");
      stringBuilder.Append(this.CreateConversationQuerySqlWhere(includeTypes, includeArchived, (string) null, out bindedParams));
      if (sortByTimestamp)
        stringBuilder.Append("ORDER BY Timestamp DESC\n");
      if (limit.HasValue)
      {
        int? nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          stringBuilder.Append("LIMIT ?");
          bindedParams.Add((object) limit.Value);
        }
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in bindedParams)
          stmt.BindObject(num++, o);
        return this.RemoveInvalidJids(this.ParseTable<Conversation>(stmt, "Conversations").ToList<Conversation>());
      }
    }

    public List<Conversation> GetConversations(
      JidHelper.JidTypes[] includeTypes,
      bool includeArchived,
      bool starred)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Conversations\n");
      stringBuilder.Append(this.CreateConversationQuerySqlWhere(includeTypes, includeArchived, (string) null, out bindedParams));
      string str = "SELECT DISTINCT KeyRemoteJid FROM MESSAGES WHERE IsStarred = 1";
      if (starred)
        stringBuilder.Append("AND Jid IN (");
      else
        stringBuilder.Append("AND Jid NOT IN (");
      stringBuilder.Append(str);
      stringBuilder.Append(") \n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in bindedParams)
          stmt.BindObject(num++, o);
        return this.RemoveInvalidJids(this.ParseTable<Conversation>(stmt, "Conversations").ToList<Conversation>());
      }
    }

    public List<Conversation> GetBroadcastLists()
    {
      return this.GetConversations(new JidHelper.JidTypes[1]
      {
        JidHelper.JidTypes.Broadcast
      }, false);
    }

    public List<Conversation> GetGroups(bool includeArchived, bool sortBySubject = false, int? limit = null)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Conversations \n");
      stringBuilder.Append(this.CreateConversationQuerySqlWhere(new JidHelper.JidTypes[1]
      {
        JidHelper.JidTypes.Group
      }, includeArchived, (string) null, out bindedParams));
      if (sortBySubject)
        stringBuilder.Append("ORDER BY GroupSubject ASC \n");
      if (limit.HasValue)
      {
        int? nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          stringBuilder.Append("LIMIT ? \n");
          bindedParams.Add((object) limit.Value);
        }
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in bindedParams)
          stmt.BindObject(num++, o);
        return this.RemoveInvalidJids(this.ParseTable<Conversation>(stmt, "Conversations").ToList<Conversation>());
      }
    }

    private List<Conversation> RemoveInvalidJids(List<Conversation> fullList)
    {
      if (fullList == null)
        return (List<Conversation>) null;
      List<Conversation> conversationList = new List<Conversation>();
      foreach (Conversation full in fullList)
      {
        if (JidChecker.CheckJidProtocolString(full.Jid))
          conversationList.Add(full);
        else
          JidChecker.MaybeSendJidErrorClb("Invalid jid in Conversation", full.Jid);
      }
      return conversationList;
    }

    public ConversationItem GetConversationItem(string jid)
    {
      ConversationItem conversationItem1 = (ConversationItem) null;
      Conversation conversation = this.GetConversation(jid, CreateOptions.None);
      if (conversation != null)
      {
        conversationItem1 = new ConversationItem(conversation);
        int? lastMessageId = conversation.LastMessageID;
        if (lastMessageId.HasValue)
        {
          ConversationItem conversationItem2 = conversationItem1;
          lastMessageId = conversation.LastMessageID;
          Message messageById = this.GetMessageById(lastMessageId.Value);
          conversationItem2.Message = messageById;
        }
      }
      return conversationItem1;
    }

    public List<ConversationItem> GetConversationItems(
      JidHelper.JidTypes[] includeTypes,
      bool includeArchived,
      SqliteMessagesContext.ConversationSortTypes sortType = SqliteMessagesContext.ConversationSortTypes.None,
      int? limit = null)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT c.*, '#', m.*\n");
      stringBuilder.Append("FROM Conversations as c\n");
      stringBuilder.Append("LEFT JOIN Messages as m\n");
      stringBuilder.Append("ON c.LastMessageID = m.MessageID\n");
      stringBuilder.Append(this.CreateConversationQuerySqlWhere(includeTypes, includeArchived, "c.", out bindedParams));
      switch (sortType)
      {
        case SqliteMessagesContext.ConversationSortTypes.TimestampOnly:
          stringBuilder.Append("ORDER BY Timestamp DESC\n");
          break;
        case SqliteMessagesContext.ConversationSortTypes.SortKeyAndTimestamp:
          stringBuilder.Append("ORDER BY SortKey DESC, Timestamp DESC\n");
          break;
      }
      if (limit.HasValue)
      {
        int? nullable = limit;
        int num = 0;
        if ((nullable.GetValueOrDefault() > num ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        {
          stringBuilder.Append("LIMIT ?");
          bindedParams.Add((object) limit.Value);
        }
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in bindedParams)
          stmt.BindObject(num++, o);
        return this.RemoveInvalidJids(this.ParseJoin<Conversation, Message>(stmt, "Conversations", "Messages").Select<KeyValuePair<Conversation, Message>, ConversationItem>((Func<KeyValuePair<Conversation, Message>, ConversationItem>) (p => new ConversationItem(p.Key)
        {
          Message = p.Value
        })).ToList<ConversationItem>());
      }
    }

    private List<ConversationItem> RemoveInvalidJids(List<ConversationItem> fullList)
    {
      if (fullList == null)
        return (List<ConversationItem>) null;
      List<ConversationItem> conversationItemList = new List<ConversationItem>();
      foreach (ConversationItem full in fullList)
      {
        if (JidChecker.CheckJidProtocolString(full.Jid))
          conversationItemList.Add(full);
        else
          JidChecker.MaybeSendJidErrorClb("Invalid jid in ConversationItems", full.Jid);
      }
      return conversationItemList;
    }

    public void MarkMessagesAsDeleted(string jid, bool exceptStarred)
    {
      StringBuilder stringBuilder = new StringBuilder("UPDATE Messages SET Flags = ? \n");
      stringBuilder.Append("WHERE KeyRemoteJid = ? \n");
      if (exceptStarred)
        stringBuilder.Append("AND IsStarred = 0 \n");
      using (Sqlite.PreparedStatement preparedStatement1 = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement2 = preparedStatement1;
        int idx1 = num1;
        int num2 = idx1 + 1;
        preparedStatement2.Bind(idx1, 1, false);
        Sqlite.PreparedStatement preparedStatement3 = preparedStatement1;
        int idx2 = num2;
        int num3 = idx2 + 1;
        string val = jid;
        preparedStatement3.Bind(idx2, val);
        preparedStatement1.Step();
      }
    }

    public Message UpdateChatLastMessage(Conversation convo)
    {
      if (convo == null)
        return (Message) null;
      Message message = ((IEnumerable<Message>) this.GetLatestMessages(convo.Jid, convo.MessageLoadingStart(), new int?(1), new int?())).FirstOrDefault<Message>();
      convo.LastMessageID = message == null ? new int?() : new int?(message.MessageID);
      return message;
    }

    public void BatchDeleteMessages(
      string jid,
      int batchCount,
      int? upperBound,
      out int deleted,
      bool hasStarredMsgsToKeep = false)
    {
      deleted = 0;
      bool flag1 = false;
      try
      {
        List<object> objectList = new List<object>();
        StringBuilder stringBuilder1 = new StringBuilder();
        if (jid == null)
        {
          if (upperBound.HasValue)
          {
            stringBuilder1.Append("WHERE (MessageID < ? OR Flags = ?) \n");
            objectList.Add((object) upperBound.Value);
          }
          else
            stringBuilder1.Append("WHERE Flags = ? \n");
        }
        else
        {
          stringBuilder1.Append("WHERE KeyRemoteJid = ? \n");
          objectList.Add((object) jid);
          if (upperBound.HasValue)
          {
            stringBuilder1.Append("AND (MessageID < ? OR Flags = ?) \n");
            objectList.Add((object) upperBound.Value);
          }
          else
            stringBuilder1.Append("AND Flags = ? \n");
        }
        objectList.Add((object) 1);
        if (hasStarredMsgsToKeep)
          stringBuilder1.Append("AND IsStarred = 0 \n");
        stringBuilder1.Append("ORDER BY MessageID ASC LIMIT ? \n");
        objectList.Add((object) batchCount);
        string str1 = stringBuilder1.ToString();
        LinkedList<long> linkedList = new LinkedList<long>();
        LinkedList<Pair<string, LocalFileType>> source1 = new LinkedList<Pair<string, LocalFileType>>();
        LinkedList<Pair<string, bool>> source2 = new LinkedList<Pair<string, bool>>();
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT MessageID, LocalFileUri, DataFileName, MediaWaType, MediaOrigin, QuotedMediaFileUri FROM Messages\n" + str1))
        {
          int i = 0;
          objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
          while (stmt.Step())
          {
            linkedList.AddLast((long) stmt.Columns[0]);
            if (stmt.Columns[1] is string column3)
            {
              int column1 = (int) (long) stmt.Columns[3];
              string column2 = stmt.Columns[4] as string;
              if (column1 == 2 && column2 == "live")
                source2.AddLast(new Pair<string, bool>(column3, false));
              else
                source1.AddLast(new Pair<string, LocalFileType>(column3, LocalFileType.Unknown));
            }
            if (stmt.Columns[2] is string column4)
              source1.AddLast(new Pair<string, LocalFileType>(column4, LocalFileType.Thumbnail));
            if (stmt.Columns[5] is string column5)
              source1.AddLast(new Pair<string, LocalFileType>(column5, LocalFileType.QuotedMedia));
          }
        }
        if (!linkedList.Any<long>())
        {
          Log.d("msg db", "msg batch delete | nothing to delete");
        }
        else
        {
          string str2 = string.Format("(SELECT MessageID FROM Messages {0})", (object) str1);
          StringBuilder stringBuilder2 = new StringBuilder("SELECT AlternateUploadUri FROM MessageMiscInfos\n");
          stringBuilder2.Append("WHERE MessageId > ? AND MessageId < ? AND MessageId IN ");
          stringBuilder2.Append(str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder2.ToString()))
          {
            int i = 0;
            stmt.Bind(i++, linkedList.First<long>() - 1L, false);
            stmt.Bind(i++, linkedList.Last<long>() + 1L, false);
            objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
            while (stmt.Step())
            {
              string column = (string) stmt.Columns[0];
              if (column != null)
                source1.AddLast(new Pair<string, LocalFileType>(column, LocalFileType.Unknown));
            }
          }
          this.BeginTransaction();
          flag1 = true;
          bool flag2 = false;
          if (jid != null)
          {
            flag2 = JidHelper.IsStatusJid(jid);
            if (flag2)
            {
              StringBuilder stringBuilder3 = new StringBuilder("DELETE FROM WaStatuses\n");
              stringBuilder3.Append("WHERE MessageId IN ");
              stringBuilder3.Append(str2);
              using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder3.ToString()))
              {
                int i = 0;
                objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
                stmt.Step();
              }
            }
          }
          StringBuilder stringBuilder4 = new StringBuilder("DELETE FROM MessageMiscInfos\n");
          stringBuilder4.Append("WHERE MessageId > ? AND MessageId < ? AND MessageId IN ");
          stringBuilder4.Append(str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder4.ToString()))
          {
            int i = 0;
            stmt.Bind(i++, linkedList.First<long>() - 1L, false);
            stmt.Bind(i++, linkedList.Last<long>() + 1L, false);
            objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
            stmt.Step();
          }
          StringBuilder stringBuilder5 = new StringBuilder("DELETE FROM ReceiptState\n");
          stringBuilder5.Append("WHERE MessageId > ? AND MessageId < ? AND MessageId IN ");
          stringBuilder5.Append(str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder5.ToString()))
          {
            int i = 0;
            stmt.Bind(i++, linkedList.First<long>() - 1L, false);
            stmt.Bind(i++, linkedList.Last<long>() + 1L, false);
            objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
            stmt.Step();
          }
          StringBuilder stringBuilder6 = new StringBuilder("DELETE FROM Messages\n");
          if (jid == null)
            stringBuilder6.Append("WHERE MessageID > ? AND MessageID < ? AND MessageID IN ");
          else
            stringBuilder6.Append("WHERE KeyRemoteJid = ? AND MessageID > ? AND MessageID < ? AND MessageID IN ");
          stringBuilder6.Append(str2);
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder6.ToString()))
          {
            int i = 0;
            if (jid != null)
              stmt.Bind(i++, jid);
            stmt.Bind(i++, linkedList.First<long>() - 1L, false);
            stmt.Bind(i++, linkedList.Last<long>() + 1L, false);
            objectList.ForEach((Action<object>) (obj => stmt.BindObject(i++, obj)));
            stmt.Step();
          }
          this.CommitTransaction();
          flag1 = false;
          deleted = linkedList.Count;
          this.PurgeCache("Messages", (IEnumerable<long>) linkedList);
          Set<long> msgIdsSet = new Set<long>((IEnumerable<long>) linkedList);
          this.PurgeCache<MessageMiscInfo>("MessageMiscInfos", (Func<MessageMiscInfo, bool>) (misc => misc.MessageId.HasValue && msgIdsSet.Contains((long) misc.MessageId.Value)));
          this.PurgeCache<WhatsApp.ReceiptState>("ReceiptState", (Func<WhatsApp.ReceiptState, bool>) (rs => msgIdsSet.Contains((long) rs.MessageId)));
          if (source1.Any<Pair<string, LocalFileType>>())
          {
            using (IsoStoreMediaStorage isoStore = new IsoStoreMediaStorage())
            {
              foreach (Pair<string, LocalFileType> pair in source1)
                this.LocalFileRelease(pair.First, pair.Second != LocalFileType.Unknown ? pair.Second : (!flag2 ? LocalFileType.MessageMedia : LocalFileType.StatusMedia), isoStore);
            }
          }
          if (source2.Any<Pair<string, bool>>())
          {
            using (NativeMediaStorage nativeStore = new NativeMediaStorage())
            {
              foreach (Pair<string, bool> pair in source2)
                this.LocalFileRelease(pair.First, pair.Second ? LocalFileType.Thumbnail : (flag2 ? LocalFileType.StatusMedia : LocalFileType.MessageMedia), nativeStore: nativeStore, deleteNative: true);
            }
          }
          if (source1.Any<Pair<string, LocalFileType>>() || source2.Any<Pair<string, bool>>())
            this.SubmitChanges();
          Log.d("msg db", "msg batch delete | msg deleted:{0},local file released:{1}", (object) deleted, (object) source1.Count);
        }
      }
      catch (Exception ex)
      {
        Log.l(nameof (BatchDeleteMessages), "Exception {0}, last sql:{1}, inTx:{2}", (object) ex.GetFriendlyMessage(), (object) this.Db.MaybeLastPreparedStatementString, (object) flag1);
        if (flag1)
          this.RollbackTransaction(ex);
        throw;
      }
    }

    public void InsertPostponedReceiptsOnSubmit(IEnumerable<PostponedReceipt> receipts)
    {
      foreach (object receipt in receipts)
        this.Insert("PostponedReceipts", receipt);
    }

    public PostponedReceipt[] GetPostponedReceipts(long t0, long t1)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM PostponedReceipts WHERE TimestampLong > ? AND TimestampLong < ?"))
      {
        stmt.Bind(0, t0, false);
        stmt.Bind(1, t1, false);
        return this.ParseTable<PostponedReceipt>(stmt, "PostponedReceipts").ToArray<PostponedReceipt>();
      }
    }

    public void DeletePostponedReceipts(long t0, long t1)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM PostponedReceipts WHERE TimestampLong > ? AND TimestampLong < ?"))
      {
        preparedStatement.Bind(0, t0, false);
        preparedStatement.Bind(1, t1, false);
        preparedStatement.Step();
      }
    }

    public void InsertCipherTextReceiptsOnSubmit(IEnumerable<CipherTextReceipt> receipts)
    {
      foreach (object receipt in receipts)
        this.Insert("CipherTextReceipts", receipt);
    }

    public bool UpdateCipherTextReceiptsDecrypted(
      string keyRemoteJid,
      string keyId,
      string participantJid)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE CipherTextReceipts\nSET IsCipherText = ?\nWHERE KeyRemoteJid = ? AND KeyId = ? AND ParticipantJid = ?"))
      {
        preparedStatement.Bind(0, false);
        preparedStatement.Bind(1, keyRemoteJid);
        preparedStatement.Bind(2, keyId);
        preparedStatement.Bind(3, participantJid);
        return preparedStatement.Step();
      }
    }

    public bool HasDecryptedCipherTextReceipts(string keyRemoteJid)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT 1 FROM CipherTextReceipts WHERE KeyRemoteJid = ? AND IsCipherText = ?"))
      {
        preparedStatement.Bind(0, keyRemoteJid);
        preparedStatement.Bind(1, false);
        return preparedStatement.Step();
      }
    }

    public CipherTextReceipt[] GetDecryptedCipherTextReceipts(string keyRemoteJid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM CipherTextReceipts WHERE KeyRemoteJid = ? AND IsCipherText = ?"))
      {
        stmt.Bind(0, keyRemoteJid);
        stmt.Bind(1, false);
        return this.ParseTable<CipherTextReceipt>(stmt, "CipherTextReceipts").ToArray<CipherTextReceipt>();
      }
    }

    public void DeleteCipherTextReceiptsOnSubmit(IEnumerable<CipherTextReceipt> receipts)
    {
      foreach (object receipt in receipts)
        this.Delete("CipherTextReceipts", receipt);
    }

    public void InsertPendingMessageOnSubmit(PendingMessage m)
    {
      this.Insert("PendingMessages", (object) m);
    }

    public void ReleasePendingMessages(string senderJid)
    {
      Dictionary<FunXMPP.FMessage.Key, bool> dictionary = new Dictionary<FunXMPP.FMessage.Key, bool>();
      List<PendingMessage> pendingMessageList = new List<PendingMessage>();
      string sql = "SELECT * FROM PendingMessages WHERE KeyRemoteJid = ? OR RemoteResource = ?";
      int num1 = 0;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(sql))
      {
        stmt.Bind(0, senderJid);
        stmt.Bind(1, senderJid);
        foreach (PendingMessage pendingMessage in this.ParseTable<PendingMessage>(stmt, "PendingMessages"))
        {
          FunXMPP.FMessage fMessage = new FunXMPP.FMessage(new FunXMPP.FMessage.Key(pendingMessage.KeyRemoteJid, false, pendingMessage.KeyId));
          if (JidHelper.IsMultiParticipantsChatJid(pendingMessage.KeyRemoteJid))
            fMessage.remote_resource = senderJid;
          fMessage.timestamp = pendingMessage.Timestamp;
          WhatsApp.ProtoBuf.Message.Deserialize(pendingMessage.ProtobufMessage).PopulateFMessage(fMessage);
          if (fMessage.media_wa_type == FunXMPP.FMessage.Type.LiveLocation)
          {
            PendingMsgProperties.LiveLocationProperties locationPropertiesField = PendingMsgProperties.GetForPendingMsg(pendingMessage).LiveLocationPropertiesField;
            if (locationPropertiesField != null)
            {
              int? durationSeconds = locationPropertiesField.DurationSeconds;
              if (durationSeconds.HasValue)
              {
                FunXMPP.FMessage fmessage = fMessage;
                durationSeconds = locationPropertiesField.DurationSeconds;
                int num2 = durationSeconds.Value;
                fmessage.media_duration_seconds = num2;
                Log.d("msgdb", "duration " + (object) fMessage.media_duration_seconds);
              }
            }
          }
          Message m = new Message(fMessage);
          bool flag = false;
          if (!dictionary.TryGetValue(fMessage.key, out flag))
          {
            this.InsertMessageOnSubmit(m);
            dictionary[fMessage.key] = true;
          }
          else
            ++num1;
          pendingMessageList.Add(pendingMessage);
          this.Delete("PendingMessages", (object) pendingMessage);
        }
      }
      Log.l("msgdb", "ReleasePendingMessages for {0} found {1} message(s), {2} duplicates", (object) senderJid, (object) pendingMessageList.Count, (object) num1);
      if (pendingMessageList.Count == 0)
        return;
      try
      {
        this.SubmitChanges();
      }
      catch (Exception ex1)
      {
        string context1 = string.Format("Exception releasing {0} pending messages", (object) pendingMessageList.Count);
        Log.LogException(ex1, context1);
        foreach (PendingMessage o in pendingMessageList)
        {
          Log.l(string.Format("Attempting post failure removal of {0} {1}", (object) o.KeyId, (object) o.KeyRemoteJid));
          this.Delete("PendingMessages", (object) o);
        }
        try
        {
          this.SubmitChanges();
        }
        catch (Exception ex2)
        {
          string context2 = string.Format("Exception removing {0} pending messages", (object) pendingMessageList.Count);
          Log.LogException(ex2, context2);
        }
        if (SqliteMessagesContext.throttleReleasePendingClbUpload)
          return;
        Log.SendCrashLog(new Exception("Exception releasing pending message"), "Exception releasing pending message", logOnlyForRelease: true);
        SqliteMessagesContext.throttleReleasePendingClbUpload = true;
      }
    }

    public void AttemptScheduledTaskOnThreadPool(
      WaScheduledTask task,
      int delayInMS,
      bool ignorePendingCheck = false,
      object tag = null)
    {
      WaScheduledTask.AttemptOnThreadPool((SqliteDataContext) this, task, delayInMS, (Action<WaScheduledTask>) (taskToDel => this.DeleteWaScheduledTaskOnSubmit(taskToDel)), (Action<WaScheduledTask>) (taskToAttempt => SqliteMessagesContext.AttemptScheduledTask(taskToAttempt, tag)), ignorePendingCheck);
    }

    public static void AttemptScheduledTask(WaScheduledTask task, object tag = null)
    {
      WaScheduledTask.GetAttemptObservable(task, tag).Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => MessagesContext.Run((MessagesContext.MessagesCallback) (db => WaScheduledTask.OnTaskDone((SqliteDataContext) db, task, (Action<WaScheduledTask>) (taskToDel => db.DeleteWaScheduledTaskOnSubmit(taskToDel)))))), (Action<Exception>) (ex => MessagesContext.Run((MessagesContext.MessagesCallback) (db => WaScheduledTask.OnAttemptError((SqliteDataContext) db, task, ex, (Action<WaScheduledTask>) (taskToDel => db.DeleteWaScheduledTaskOnSubmit(taskToDel)))))), (Action) (() => WaScheduledTask.OnAttemptDone(task)));
    }

    public void InsertWaScheduledTaskOnSubmit(WaScheduledTask task)
    {
      this.Insert("WaScheduledTasks", (object) task);
    }

    public void DeleteWaScheduledTaskOnSubmit(WaScheduledTask task)
    {
      if (task.IsDeleted)
        return;
      this.Delete("WaScheduledTasks", (object) task);
      task.IsDeleted = true;
    }

    public void CleanupWaScheduledTasks()
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("DELETE FROM WaScheduledTasks WHERE ");
      stringBuilder.Append("ExpirationUtc IS NOT NULL AND ExpirationUtc < ?");
      objectList.Add((object) FunRunner.CurrentServerTimeUtc.ToFileTimeUtc());
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        preparedStatement.Step();
      }
    }

    public void RemoveScheduledTasks(WaScheduledTask.Types taskType, string lookupKey = null)
    {
      WaScheduledTask[] waScheduledTasks = this.GetWaScheduledTasks(new WaScheduledTask.Types[1]
      {
        taskType
      }, excludeExpired: false, lookupKey: lookupKey);
      bool flag = false;
      foreach (WaScheduledTask task in waScheduledTasks)
      {
        task.IsDeleted = true;
        Log.l("msgdb", "finished | id:{0},type:{1},attempts:{2}", (object) task.TaskID, (object) (WaScheduledTask.Types) task.TaskType, (object) task.Attempts);
        this.DeleteWaScheduledTaskOnSubmit(task);
        flag = true;
      }
      if (!flag)
        return;
      this.SubmitChanges();
    }

    public WaScheduledTask[] GetWaScheduledTasks(
      WaScheduledTask.Types[] includeTypes = null,
      WaScheduledTask.Types[] excludeTypes = null,
      bool excludeExpired = true,
      string lookupKey = null,
      int? limit = null,
      WaScheduledTask.Restrictions restriction = WaScheduledTask.Restrictions.None)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM WaScheduledTasks\n");
      List<object> objectList = new List<object>();
      Pair<string, List<object>> tasksWhereClauses = WaScheduledTask.GetWaScheduledTasksWhereClauses(includeTypes, excludeTypes, excludeExpired, lookupKey, restriction);
      if (!string.IsNullOrEmpty(tasksWhereClauses.First))
      {
        stringBuilder.AppendFormat("{0}\n", (object) tasksWhereClauses.First);
        objectList.AddRange((IEnumerable<object>) tasksWhereClauses.Second);
      }
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder.Append("ORDER BY TaskID ASC LIMIT ?\n");
        objectList.Add((object) limit.Value);
      }
      else
        stringBuilder.Append("ORDER BY TaskID ASC\n");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          stmt.BindObject(num++, o);
        return this.ParseTable<WaScheduledTask>(stmt, "WaScheduledTasks").ToArray<WaScheduledTask>();
      }
    }

    public long GetWaScheduledTasksCount(
      WaScheduledTask.Types[] includeTypes = null,
      WaScheduledTask.Types[] excludeTypes = null,
      bool excludeExpired = true,
      string lookupKey = null,
      WaScheduledTask.Restrictions restriction = WaScheduledTask.Restrictions.None)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(*) FROM WaScheduledTasks\n");
      List<object> objectList = new List<object>();
      Pair<string, List<object>> tasksWhereClauses = WaScheduledTask.GetWaScheduledTasksWhereClauses(includeTypes, excludeTypes, excludeExpired, lookupKey, restriction);
      if (!string.IsNullOrEmpty(tasksWhereClauses.First))
      {
        stringBuilder.AppendFormat("{0}\n", (object) tasksWhereClauses.First);
        objectList.AddRange((IEnumerable<object>) tasksWhereClauses.Second);
      }
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          preparedStatement.BindObject(num++, o);
        preparedStatement.Step();
        return (long) preparedStatement.Columns[0];
      }
    }

    public void InsertWaStatusOnSubmit(WaStatus status)
    {
      this.Insert("WaStatuses", (object) status);
    }

    public void SaveStatusForMessageOnSubmit(
      Message msg,
      out PersistentAction autoDownloadPersitAction)
    {
      autoDownloadPersitAction = (PersistentAction) null;
      if (!msg.IsStatus())
        return;
      if (!msg.MediaWaType.IsSupportedStatusType())
      {
        Log.l("msgdb", "skip saving unsupported status msg as status | type:{0}", (object) msg.MediaWaType);
      }
      else
      {
        string senderJid = msg.GetSenderJid();
        WaStatus status = new WaStatus()
        {
          MessageId = msg.MessageID,
          MessageKeyId = msg.KeyId,
          Jid = senderJid,
          Timestamp = msg.FunTimestamp ?? FunRunner.CurrentServerTimeUtc,
          IsViewed = msg.KeyFromMe
        };
        JidHelper.JidTypes jidType = JidHelper.GetJidType(status.Jid);
        switch (jidType)
        {
          case JidHelper.JidTypes.User:
          case JidHelper.JidTypes.Psa:
            this.InsertWaStatusOnSubmit(status);
            Log.d("msgdb", "saving status from {0} | mid:{1}, keyid:{2}", msg.KeyFromMe ? (object) "self" : (object) status.Jid, (object) msg.MessageID, (object) msg.KeyId);
            if (jidType == JidHelper.JidTypes.Psa)
            {
              Settings.IsStatusPSAUnseen = true;
              break;
            }
            break;
          default:
            Log.l("msgdb", "saving status aborted | invalid status sender jid");
            break;
        }
        JidInfo jidInfo = this.GetJidInfo(senderJid, CreateOptions.None);
        if ((jidInfo != null ? jidInfo.StatusAutoDownloadQuota : 0) <= 0)
          return;
        MessageProperties forMessage = MessageProperties.GetForMessage(msg);
        forMessage.EnsureMediaProperties.AutoDownloadEligible = new bool?(true);
        forMessage.Save();
        Log.l("msgdb", "download quota:{0} | should auto download | {1}", (object) jidInfo.StatusAutoDownloadQuota, (object) msg.LogInfo());
        --jidInfo.StatusAutoDownloadQuota;
        PersistentAction a = PersistentAction.AutoDownload(msg);
        this.StorePersistentAction(a);
        autoDownloadPersitAction = a;
      }
    }

    public void ClearWaStatuses(string[] jids = null)
    {
      StringBuilder stringBuilder = new StringBuilder("DELETE FROM WaStatuses ");
      string[] strArray = (string[]) null;
      IEnumerable<object> objects = (IEnumerable<object>) null;
      if (jids != null && ((IEnumerable<string>) jids).Any<string>())
      {
        if (jids.Length > 999)
        {
          jids = ((IEnumerable<string>) jids).Take<string>(999).ToArray<string>();
          strArray = ((IEnumerable<string>) jids).Skip<string>(999).ToArray<string>();
        }
        stringBuilder.AppendFormat("WHERE Jid IN ({0})", (object) string.Join(",", ((IEnumerable<string>) jids).Select<string, string>((Func<string, string>) (_ => "?"))));
        objects = (IEnumerable<object>) jids;
      }
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        if (objects != null)
        {
          int num = 0;
          foreach (object o in objects)
            preparedStatement.BindObject(num++, o);
        }
        preparedStatement.Step();
        Log.l("msgdb", "cleared {0} contacts' statuses", (object) jids.Length);
      }
      if (strArray == null || !((IEnumerable<string>) strArray).Any<string>())
        return;
      this.ClearWaStatuses(strArray);
    }

    private WaStatus GetWaStatus(int sid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM WaStatuses WHERE StatusId = ?"))
      {
        stmt.Bind(0, sid, false);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").FirstOrDefault<WaStatus>();
      }
    }

    public WaStatus GetWaStatus(string senderJid, int msgId)
    {
      if (string.IsNullOrEmpty(senderJid) || msgId < 0)
        return (WaStatus) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM WaStatuses WHERE Jid = ? AND MessageId = ?"))
      {
        stmt.Bind(0, senderJid);
        stmt.Bind(1, msgId, false);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").FirstOrDefault<WaStatus>();
      }
    }

    public WaStatus GetWaStatus(string senderJid, string msgKeyId)
    {
      if (string.IsNullOrEmpty(senderJid) || string.IsNullOrEmpty(msgKeyId))
        return (WaStatus) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM WaStatuses WHERE Jid = ? AND MessageKeyId = ?"))
      {
        stmt.Bind(0, senderJid);
        stmt.Bind(1, msgKeyId);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").FirstOrDefault<WaStatus>();
      }
    }

    public WaStatus GetLastestStatus(string jid)
    {
      if (string.IsNullOrEmpty(jid))
        return (WaStatus) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM WaStatuses WHERE Jid = ? AND Timestamp > ? ORDER BY StatusId DESC LIMIT 1"))
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, (FunRunner.CurrentServerTimeUtc - WaStatus.Expiration).ToFileTimeUtc(), false);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").FirstOrDefault<WaStatus>();
      }
    }

    public WaStatus GetOldestStatus(string jid)
    {
      if (string.IsNullOrEmpty(jid))
        return (WaStatus) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM WaStatuses WHERE Jid = ? AND Timestamp > ? ORDER BY StatusId ASC LIMIT 1"))
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, (FunRunner.CurrentServerTimeUtc - WaStatus.Expiration).ToFileTimeUtc(), false);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").FirstOrDefault<WaStatus>();
      }
    }

    public WaStatus[] GetStatuses(
      string jid,
      bool unviewedOnly,
      bool asc,
      TimeSpan? withinTimeSpan)
    {
      return this.GetStatuses(new string[1]{ jid }, (string[]) null, unviewedOnly, asc, withinTimeSpan);
    }

    public WaStatus[] GetStatuses(
      string[] includeJids,
      string[] excludeJids,
      bool unviewedOnly,
      bool asc,
      TimeSpan? withinTimeSpan,
      int limit = 0,
      int? boundId = 0,
      bool lowerBound = true)
    {
      List<string> values = new List<string>();
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM WaStatuses WHERE ");
      if (includeJids != null && ((IEnumerable<string>) includeJids).Any<string>())
      {
        if (includeJids.Length == 1)
        {
          values.Add("Jid = ?");
          objectList.Add((object) includeJids[0]);
        }
        else
        {
          values.Add(string.Format("Jid IN ({0})", (object) string.Join<char>(",", ((IEnumerable<string>) includeJids).Select<string, char>((Func<string, char>) (_ => '?')))));
          objectList.AddRange((IEnumerable<object>) includeJids);
        }
      }
      if (excludeJids != null && ((IEnumerable<string>) excludeJids).Any<string>())
      {
        if (excludeJids.Length == 1)
        {
          values.Add("Jid <> ?");
          objectList.Add((object) excludeJids[0]);
        }
        else
        {
          values.Add(string.Format("Jid NOT IN ({0})", (object) string.Join<char>(",", ((IEnumerable<string>) excludeJids).Select<string, char>((Func<string, char>) (_ => '?')))));
          objectList.AddRange((IEnumerable<object>) excludeJids);
        }
      }
      if (withinTimeSpan.HasValue)
      {
        values.Add("Timestamp > ?");
        DateTime dateTime = FunRunner.CurrentServerTimeUtc - withinTimeSpan.Value;
        objectList.Add((object) dateTime.ToFileTimeUtc());
      }
      if (unviewedOnly)
        values.Add("IsViewed = 0");
      if (boundId.HasValue)
      {
        values.Add(lowerBound ? "StatusId > ?" : "StatusId < ?");
        objectList.Add((object) boundId.Value);
      }
      stringBuilder.Append(string.Join(" AND ", (IEnumerable<string>) values));
      stringBuilder.Append("\n");
      stringBuilder.AppendFormat("ORDER BY StatusId {0} ", asc ? (object) "ASC" : (object) "DESC");
      if (limit > 0)
      {
        stringBuilder.Append("LIMIT ? ");
        objectList.Add((object) limit);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          stmt.BindObject(index, objectList[index]);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").ToArray<WaStatus>();
      }
    }

    public void DeleteStatusOnSubmit(WaStatus s) => this.Delete("WaStatuses", (object) s);

    public WaStatus[] GetExpiredStatuses(string[] includeJids, string[] excludeJids, int? limit)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM WaStatuses WHERE ");
      List<string> values = new List<string>();
      List<object> objectList = new List<object>();
      if (includeJids != null && ((IEnumerable<string>) includeJids).Any<string>())
      {
        if (includeJids.Length == 1)
        {
          values.Add("Jid = ?");
          objectList.Add((object) includeJids[0]);
        }
        else
        {
          values.Add(string.Format("Jid IN ({0})", (object) string.Join<char>(",", ((IEnumerable<string>) includeJids).Select<string, char>((Func<string, char>) (_ => '?')))));
          objectList.AddRange((IEnumerable<object>) includeJids);
        }
      }
      if (excludeJids != null && ((IEnumerable<string>) excludeJids).Any<string>())
      {
        if (excludeJids.Length == 1)
        {
          values.Add("Jid <> ?");
          objectList.Add((object) excludeJids[0]);
        }
        else
        {
          values.Add(string.Format("Jid NOT IN ({0})", (object) string.Join<char>(",", ((IEnumerable<string>) excludeJids).Select<string, char>((Func<string, char>) (_ => '?')))));
          objectList.AddRange((IEnumerable<object>) excludeJids);
        }
      }
      values.Add("Timestamp < ?");
      DateTime dateTime = FunRunner.CurrentServerTimeUtc - WaStatus.Expiration;
      objectList.Add((object) dateTime.ToFileTimeUtc());
      stringBuilder.Append(string.Join(" AND ", (IEnumerable<string>) values));
      stringBuilder.Append("\n");
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder.Append("LIMIT ? ");
        objectList.Add((object) limit.Value);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          stmt.BindObject(index, objectList[index]);
        return this.ParseTable<WaStatus>(stmt, "WaStatuses").ToArray<WaStatus>();
      }
    }

    public int GetExpiredStatusCount()
    {
      int expiredStatusCount = 0;
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(*) FROM WaStatuses WHERE Timestamp < ? ");
      List<object> objectList = new List<object>();
      DateTime dateTime = FunRunner.CurrentServerTimeUtc - WaStatus.Expiration;
      objectList.Add((object) dateTime.ToFileTimeUtc());
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          preparedStatement.BindObject(index, objectList[index]);
        if (preparedStatement.Step())
          expiredStatusCount = (int) (long) preparedStatement.Columns[0];
      }
      return expiredStatusCount;
    }

    public WaStatusThread GetStatusThread(string jid, bool excludeExpired = true)
    {
      if (jid == null)
        return (WaStatusThread) null;
      WaStatusThread waStatusThread = (WaStatusThread) null;
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(StatusId), SUM(IsViewed), * FROM WaStatuses WHERE Jid = ? ");
      objectList.Add((object) jid);
      if (excludeExpired)
      {
        stringBuilder.Append("AND Timestamp > ? ");
        objectList.Add((object) (FunRunner.CurrentServerTimeUtc - WaStatus.Expiration).ToFileTimeUtc());
      }
      stringBuilder.Append("ORDER BY StatusId DESC LIMIT 1 ");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int count = objectList.Count;
        for (int index = 0; index < count; ++index)
          stmt.BindObject(index, objectList[index]);
        if (stmt.Step())
        {
          WaStatus tableRow = this.ParseTableRow<WaStatus>(stmt, "WaStatuses", 2);
          if (tableRow != null)
            waStatusThread = new WaStatusThread(tableRow.Jid, tableRow)
            {
              Count = (int) (long) stmt.Columns[0],
              ViewedCount = (int) (long) stmt.Columns[1]
            };
        }
      }
      WaStatusThread statusThread = waStatusThread;
      if (statusThread != null)
        return statusThread;
      return new WaStatusThread(jid, (WaStatus) null)
      {
        Count = 0,
        ViewedCount = 0
      };
    }

    public List<WaStatusThread> GetStatusThreads(bool sortByTimestamp)
    {
      return this.GetStatusThreads(sortByTimestamp, WaStatus.Expiration, true);
    }

    public List<WaStatusThread> GetStatusThreads(
      bool sortByTimestamp,
      TimeSpan withinTimespan,
      bool includeMuted,
      int? limit = null)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT total, viewed, s.* FROM WaStatuses s ");
      stringBuilder.Append("JOIN (SELECT StatusId, MAX(StatusId), COUNT(StatusId) total, SUM(IsViewed) viewed FROM WaStatuses WHERE ");
      stringBuilder.Append("Timestamp > ? ");
      objectList.Add((object) (FunRunner.CurrentServerTimeUtc - withinTimespan).ToFileTimeUtc());
      stringBuilder.Append("GROUP BY Jid) USING (StatusId) ");
      if (sortByTimestamp)
        stringBuilder.Append("ORDER BY s.Timestamp DESC ");
      List<WaStatusThread> statusThreads = new List<WaStatusThread>();
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        for (int index = 0; index < objectList.Count; ++index)
          stmt.BindObject(index, objectList[index]);
        int num = 0;
        SqliteDataContext.TableState table = this.GetTable("WaStatuses");
        while (stmt.Step())
        {
          WaStatus tableRow = this.ParseTableRow<WaStatus>(stmt, table, 2);
          if (tableRow != null)
          {
            if (!includeMuted)
            {
              JidInfo jidInfo = this.GetJidInfo(tableRow.Jid, CreateOptions.None);
              if ((jidInfo != null ? (jidInfo.IsStatusMuted ? 1 : 0) : 0) == 0)
                continue;
            }
            WaStatusThread waStatusThread = new WaStatusThread(tableRow.Jid, tableRow)
            {
              Count = (int) (long) stmt.Columns[0],
              ViewedCount = (int) (long) stmt.Columns[1]
            };
            statusThreads.Add(waStatusThread);
            ++num;
            if (limit.HasValue)
            {
              if (num >= limit.Value)
                break;
            }
          }
        }
      }
      return statusThreads;
    }

    public void IndexContactVCard(ContactVCard contact, int msgId)
    {
      if (contact == null)
        return;
      foreach (ContactVCard.PhoneNumber phoneNumber in contact.PhoneNumbers)
      {
        VCard o = new VCard();
        o.MessageId = new int?(msgId);
        if (phoneNumber.Jid != null)
        {
          o.Jid = phoneNumber.Jid;
          this.Insert("ContactVCards", (object) o);
        }
      }
    }

    public VCard[] GetContactVCardForMessage(int msgId)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM ContactVCards WHERE MessageId = ?"))
      {
        stmt.Bind(0, msgId, false);
        return this.ParseTable<VCard>(stmt, "ContactVCards").ToArray<VCard>();
      }
    }

    public void DeleteContacVCardOnSubmit(VCard vcard)
    {
      this.Delete("ContactVCards", (object) vcard);
    }

    public PersistentAction[] GetQrPersistentActions()
    {
      PersistentAction[] r = (PersistentAction[]) null;
      this.PrepareCachedStatement(this.getPersistActionsEqualStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, 14, false);
        r = this.ParseTable<PersistentAction>(stmt, "PersistentActions").ToArray<PersistentAction>();
      }));
      return r;
    }

    public void StorePersistentAction(PersistentAction a)
    {
      this.Insert("PersistentActions", (object) a);
    }

    public PersistentAction[] GetPersistentActions()
    {
      PersistentAction[] r = (PersistentAction[]) null;
      this.PrepareCachedStatement(this.getPersistActionsNotEqualStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, 14, false);
        r = this.ParseTable<PersistentAction>(stmt, "PersistentActions").ToArray<PersistentAction>();
      }));
      return r;
    }

    public PersistentAction[] GetPersistentActions(
      PersistentAction.Types actionType,
      string actionDataStr = null)
    {
      PersistentAction[] r = (PersistentAction[]) null;
      if (actionDataStr == null)
        this.PrepareCachedStatement(this.getPersistActionsByType, (Action<Sqlite.PreparedStatement>) (stmt =>
        {
          stmt.Bind(0, (int) actionType, false);
          r = this.ParseTable<PersistentAction>(stmt, "PersistentActions").ToArray<PersistentAction>();
        }));
      else
        this.PrepareCachedStatement(this.getPersistActionsByTypeAndDataStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
        {
          stmt.Bind(0, (int) actionType, false);
          stmt.Bind(1, actionDataStr);
          r = this.ParseTable<PersistentAction>(stmt, "PersistentActions").ToArray<PersistentAction>();
        }));
      return r;
    }

    public PersistentAction[] GetPersistentActionsForJid(
      PersistentAction.Types actionType,
      string jid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM PersistentActions WHERE ActionType = ? AND Jid = ?"))
      {
        stmt.Bind(0, (int) actionType, false);
        stmt.Bind(1, jid);
        return this.ParseTable<PersistentAction>(stmt, "PersistentActions").ToArray<PersistentAction>();
      }
    }

    public void DeletePersistentActionOnSubmit(PersistentAction a)
    {
      if (a.Removed)
        return;
      this.Delete("PersistentActions", (object) a);
      a.Removed = true;
    }

    public Message[] GetMessagesByKeyId(string keyId, bool fromMe)
    {
      Message[] r = (Message[]) null;
      this.PrepareCachedStatement(this.msgsByKeyIdStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, fromMe);
        stmt.Bind(1, keyId);
        r = this.ParseTable<Message>(stmt, "Messages").ToArray<Message>();
      }));
      return r;
    }

    public void InsertJidInfoOnSubmit(JidInfo ji) => this.Insert("JidInfos", (object) ji);

    public void DeleteJidInfoOnSubmit(JidInfo ji) => this.Delete("JidInfos", (object) ji);

    public JidInfo GetJidInfo(string jid, CreateOptions options)
    {
      return this.GetJidInfo(jid, options, out CreateResult _);
    }

    public JidInfo GetJidInfo(string jid, CreateOptions options, out CreateResult result)
    {
      result = CreateResult.None;
      if (string.IsNullOrEmpty(jid))
        return (JidInfo) null;
      JidInfo ji = (JidInfo) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM JidInfos WHERE Jid = ?"))
      {
        stmt.Bind(0, jid);
        ji = this.ParseTable<JidInfo>(stmt, "JidInfos").SingleOrDefault<JidInfo>();
      }
      if (ji == null && (options & CreateOptions.CreateIfNotFound) != CreateOptions.None)
      {
        ji = new JidInfo(jid);
        result = CreateResult.Created;
        if ((options & (CreateOptions) 2) != CreateOptions.None)
        {
          this.InsertJidInfoOnSubmit(ji);
          result = CreateResult.CreatedToDb;
          if ((options & (CreateOptions) 4) != CreateOptions.None)
          {
            this.SubmitChanges();
            result = CreateResult.CreatedAndSubmitted;
          }
        }
      }
      return ji;
    }

    public JidInfo[] GetMutedJids()
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM JidInfos WHERE MuteExpirationUtc IS NOT NULL"))
        return this.ParseTable<JidInfo>(stmt, "JidInfos").ToArray<JidInfo>();
    }

    public HashSet<string> GetStatusMutedJids()
    {
      HashSet<string> statusMutedJids = new HashSet<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT Jid FROM JidInfos WHERE IsStatusMuted = ?"))
      {
        preparedStatement.Bind(0, true);
        while (preparedStatement.Step())
          statusMutedJids.Add((string) preparedStatement.Columns[0]);
      }
      return statusMutedJids;
    }

    public JidInfo[] GetJidsPendingEncryptedAnnouncement()
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM JidInfos WHERE SupportsFullEncryption <> ? AND SupportsFullEncryption <> ?"))
      {
        stmt.Bind(0, 3, false);
        stmt.Bind(1, 4, false);
        return this.ParseTable<JidInfo>(stmt, "JidInfos").ToArray<JidInfo>();
      }
    }

    public void ResetJidInfoCustomTones()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE JidInfos SET NotificationSound = null, RingTone = null"))
        preparedStatement.Step();
      this.PurgeCache("JidInfos");
    }

    private Dictionary<string, EmojiUsage> CachedEmojiUsages
    {
      get
      {
        if (this.cachedEmojiUsages_ == null)
        {
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM EmojiUsages"))
          {
            this.cachedEmojiUsages_ = new Dictionary<string, EmojiUsage>();
            foreach (EmojiUsage emojiUsage in this.ParseTable<EmojiUsage>(stmt, "EmojiUsages"))
            {
              if (emojiUsage != null && emojiUsage.EmojiCode != null)
                this.cachedEmojiUsages_[emojiUsage.EmojiCode] = emojiUsage;
            }
          }
        }
        return this.cachedEmojiUsages_;
      }
    }

    public IEnumerable<EmojiUsage> GetAllEmojiUsages(bool sortByWeight = true)
    {
      Dictionary<string, EmojiUsage>.ValueCollection values = this.CachedEmojiUsages.Values;
      return sortByWeight ? (IEnumerable<EmojiUsage>) values.OrderByDescending<EmojiUsage, int>((Func<EmojiUsage, int>) (eu => eu.UsageWeight)) : (IEnumerable<EmojiUsage>) values;
    }

    public EmojiUsage GetEmojiUsage(string emojiCode, bool checkInCacheOnly = false)
    {
      if (emojiCode == null)
        return (EmojiUsage) null;
      EmojiUsage emojiUsage = (EmojiUsage) null;
      if (this.CachedEmojiUsages.TryGetValue(emojiCode, out emojiUsage))
        return emojiUsage;
      if (!checkInCacheOnly)
      {
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM EmojiUsages Where EmojiCode = ?"))
        {
          stmt.Bind(0, emojiCode);
          emojiUsage = this.ParseTable<EmojiUsage>(stmt, "EmojiUsages").SingleOrDefault<EmojiUsage>();
        }
        if (emojiUsage != null)
        {
          this.CachedEmojiUsages[emojiCode] = emojiUsage;
          return emojiUsage;
        }
      }
      return (EmojiUsage) null;
    }

    public void InsertEmojiUsageOnSubmit(EmojiUsage eu, bool dupCheck = true)
    {
      if (dupCheck && this.GetEmojiUsage(eu.EmojiCode) != null)
        return;
      this.Insert("EmojiUsages", (object) eu);
      this.CachedEmojiUsages[eu.EmojiCode] = eu;
    }

    public void ProcessEmojiUsages(IEnumerable<string> emojis)
    {
      int num1 = emojis.Count<string>();
      if (num1 == 0)
        return;
      int num2 = this.CachedEmojiUsages.Count<KeyValuePair<string, EmojiUsage>>();
      if (num1 * num2 < num1 * num1 + num2)
        this.ProcessEmojiUsages1(emojis);
      else
        this.ProcessEmojiUsages2(emojis);
    }

    private void ProcessEmojiUsages2(IEnumerable<string> emojis)
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      int y = 0;
      foreach (string emoji in emojis)
      {
        int num;
        if (dictionary.ContainsKey(emoji))
        {
          num = dictionary[emoji];
        }
        else
        {
          EmojiUsage emojiUsage = this.GetEmojiUsage(emoji, true);
          num = emojiUsage == null ? 0 : (int) ((double) emojiUsage.UsageWeight * Math.Pow(0.9, (double) y));
        }
        foreach (string key in dictionary.Keys.ToArray<string>())
          dictionary[key] = (int) ((double) dictionary[key] * 0.9);
        dictionary[emoji] = num + 10000;
        ++y;
      }
      double num1 = Math.Pow(0.9, (double) y);
      foreach (EmojiUsage emojiUsage in this.CachedEmojiUsages.Values)
        emojiUsage.UsageWeight = (int) ((double) emojiUsage.UsageWeight * num1);
      foreach (string emoji in emojis)
      {
        EmojiUsage eu = this.GetEmojiUsage(emoji, true);
        if (eu == null)
        {
          eu = new EmojiUsage() { EmojiCode = emoji };
          this.InsertEmojiUsageOnSubmit(eu);
        }
        eu.UsageWeight = dictionary[emoji];
      }
      this.SubmitChanges();
    }

    private void ProcessEmojiUsages1(IEnumerable<string> emojis)
    {
      foreach (string emoji in emojis)
        this.ProcessSingleEmojiUsage(emoji);
      this.SubmitChanges();
    }

    private void ProcessSingleEmojiUsage(string emojiCode, bool submitChanges = false)
    {
      EmojiUsage eu = this.GetEmojiUsage(emojiCode);
      int num = 0;
      if (eu != null)
        num = eu.UsageWeight;
      foreach (EmojiUsage emojiUsage in this.CachedEmojiUsages.Values)
        emojiUsage.UsageWeight = (int) ((double) emojiUsage.UsageWeight * 0.9);
      if (eu == null)
      {
        eu = new EmojiUsage() { EmojiCode = emojiCode };
        this.InsertEmojiUsageOnSubmit(eu, false);
      }
      eu.UsageWeight = num + 10000;
      if (!submitChanges)
        return;
      this.SubmitChanges();
    }

    private Dictionary<string, EmojiSelectedIndex> CachedEmojiSelectedIndexes
    {
      get
      {
        if (this.cachedEmojiSelectedIndexes_ == null)
        {
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM EmojiSelectedIndexes"))
            this.cachedEmojiSelectedIndexes_ = this.ParseTable<EmojiSelectedIndex>(stmt, "EmojiSelectedIndexes").ToDictionary<EmojiSelectedIndex, string, EmojiSelectedIndex>((Func<EmojiSelectedIndex, string>) (eu => eu.EmojiCode), (Func<EmojiSelectedIndex, EmojiSelectedIndex>) (eu => eu));
        }
        return this.cachedEmojiSelectedIndexes_;
      }
    }

    public Dictionary<string, int> GetAllEmojiSelectedIndexes()
    {
      return this.CachedEmojiSelectedIndexes.ToDictionary<KeyValuePair<string, EmojiSelectedIndex>, string, int>((Func<KeyValuePair<string, EmojiSelectedIndex>, string>) (eu => eu.Key), (Func<KeyValuePair<string, EmojiSelectedIndex>, int>) (eu => eu.Value.Selection));
    }

    public int GetSingleEmojiSelectedIndex(string emojiCode, bool checkInCacheOnly = false)
    {
      int emojiSelectedIndex1 = -1;
      EmojiSelectedIndex emojiSelectedIndex2;
      if (!this.CachedEmojiSelectedIndexes.TryGetValue(emojiCode, out emojiSelectedIndex2) && !checkInCacheOnly)
      {
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM EmojiSelectedIndexes Where EmojiCode = ?"))
        {
          stmt.Bind(0, emojiCode);
          emojiSelectedIndex2 = this.ParseTable<EmojiSelectedIndex>(stmt, "EmojiSelectedIndexes").SingleOrDefault<EmojiSelectedIndex>();
        }
      }
      if (emojiSelectedIndex2 != null)
      {
        this.CachedEmojiSelectedIndexes[emojiCode] = emojiSelectedIndex2;
        emojiSelectedIndex1 = emojiSelectedIndex2.Selection;
      }
      return emojiSelectedIndex1;
    }

    public void ApplyAllEmojiSelectedIndexes(Dictionary<string, int> newEntries)
    {
      foreach (KeyValuePair<string, int> newEntry in newEntries)
      {
        EmojiSelectedIndex o;
        if (this.CachedEmojiSelectedIndexes.TryGetValue(newEntry.Key, out o))
        {
          if (this.CachedEmojiSelectedIndexes[newEntry.Key].Selection != newEntry.Value)
            this.CachedEmojiSelectedIndexes[newEntry.Key].Selection = newEntry.Value;
        }
        else
        {
          o = new EmojiSelectedIndex()
          {
            EmojiCode = newEntry.Key,
            Selection = newEntry.Value
          };
          this.CachedEmojiSelectedIndexes[newEntry.Key] = o;
          this.Insert("EmojiSelectedIndexes", (object) o);
        }
      }
      this.SubmitChanges();
    }

    public void ApplySingleEmojiSelectedIndex(string code, int selection)
    {
      EmojiSelectedIndex emojiSelectedIndex;
      if (!this.CachedEmojiSelectedIndexes.TryGetValue(code, out emojiSelectedIndex))
      {
        EmojiSelectedIndex o = new EmojiSelectedIndex()
        {
          EmojiCode = code,
          Selection = selection
        };
        this.CachedEmojiSelectedIndexes[code] = o;
        this.Insert("EmojiSelectedIndexes", (object) o);
        this.SubmitChanges();
      }
      else
        emojiSelectedIndex.Selection = selection;
    }

    public void ClearEmojiSelectedIndexes()
    {
      foreach (object o in this.CachedEmojiSelectedIndexes.Values)
        this.Delete("EmojiSelectedIndexes", o);
      this.CachedEmojiSelectedIndexes.Clear();
      this.SubmitChanges();
    }

    private Pair<string, List<object>> GetReceiptsForMessageWhereClauses(
      int msgId,
      FunXMPP.FMessage.Status[] receiptTypes = null,
      string recipientJid = null)
    {
      List<object> second = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("MessageId = ? ");
      second.Add((object) msgId);
      if (receiptTypes != null && ((IEnumerable<FunXMPP.FMessage.Status>) receiptTypes).Any<FunXMPP.FMessage.Status>())
      {
        stringBuilder.Append(" AND Status IN (");
        stringBuilder.Append(string.Join(", ", ((IEnumerable<FunXMPP.FMessage.Status>) receiptTypes).Select<FunXMPP.FMessage.Status, string>((Func<FunXMPP.FMessage.Status, string>) (rt => "?"))));
        stringBuilder.Append(") ");
        foreach (FunXMPP.FMessage.Status receiptType in receiptTypes)
          second.Add((object) (int) receiptType);
      }
      if (recipientJid != null)
      {
        stringBuilder.Append(" AND Jid = ?");
        second.Add((object) recipientJid);
      }
      return new Pair<string, List<object>>(stringBuilder.ToString(), second);
    }

    public int GetReceiptCountForMessage(
      int msgId,
      FunXMPP.FMessage.Status[] receiptTypes = null,
      string recipientJid = null)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT COUNT(*) From ReceiptState WHERE ");
      Pair<string, List<object>> messageWhereClauses = this.GetReceiptsForMessageWhereClauses(msgId, receiptTypes, recipientJid);
      stringBuilder.Append(messageWhereClauses.First);
      List<object> second = messageWhereClauses.Second;
      int receiptCountForMessage = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in second)
          preparedStatement.BindObject(num++, o);
        if (preparedStatement.Step())
          receiptCountForMessage = (int) (long) preparedStatement.Columns[0];
      }
      return receiptCountForMessage;
    }

    public List<WhatsApp.ReceiptState> GetReceiptsForMessage(
      int msgId,
      FunXMPP.FMessage.Status[] receiptTypes = null,
      string recipientJid = null,
      bool sortByTimestamp = false)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT * From ReceiptState WHERE ");
      Pair<string, List<object>> messageWhereClauses = this.GetReceiptsForMessageWhereClauses(msgId, receiptTypes, recipientJid);
      stringBuilder.Append(messageWhereClauses.First);
      List<object> second = messageWhereClauses.Second;
      if (sortByTimestamp)
        stringBuilder.Append(" ORDER BY Timestamp DESC");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in second)
          stmt.BindObject(num++, o);
        return this.ParseTable<WhatsApp.ReceiptState>(stmt, "ReceiptState").ToList<WhatsApp.ReceiptState>();
      }
    }

    public List<WhatsApp.ReceiptState> AddReceiptForMessage(
      int msgId,
      string participantJid,
      FunXMPP.FMessage.Status status,
      DateTime dt,
      out bool dirty,
      out bool createdDelivery)
    {
      dirty = false;
      createdDelivery = false;
      switch (status)
      {
        case FunXMPP.FMessage.Status.ReceivedByServer:
          participantJid = (string) null;
          goto case FunXMPP.FMessage.Status.ReceivedByTarget;
        case FunXMPP.FMessage.Status.ReceivedByTarget:
        case FunXMPP.FMessage.Status.PlayedByTarget:
        case FunXMPP.FMessage.Status.ReadByTarget:
          List<WhatsApp.ReceiptState> source = this.GetReceiptsForMessage(msgId) ?? new List<WhatsApp.ReceiptState>();
          WhatsApp.ReceiptState receiptState = source.Where<WhatsApp.ReceiptState>((Func<WhatsApp.ReceiptState, bool>) (rs => rs.Status == status && rs.Jid == participantJid)).FirstOrDefault<WhatsApp.ReceiptState>();
          if (receiptState == null)
          {
            if (status == FunXMPP.FMessage.Status.ReadByTarget && !source.Where<WhatsApp.ReceiptState>((Func<WhatsApp.ReceiptState, bool>) (rs => rs.Status == FunXMPP.FMessage.Status.ReceivedByTarget && rs.Jid == participantJid)).Any<WhatsApp.ReceiptState>())
            {
              WhatsApp.ReceiptState o = new WhatsApp.ReceiptState()
              {
                MessageId = msgId,
                Jid = participantJid,
                Status = FunXMPP.FMessage.Status.ReceivedByTarget,
                Timestamp = dt
              };
              this.Insert("ReceiptState", (object) o);
              source.Add(o);
              createdDelivery = true;
            }
            WhatsApp.ReceiptState o1 = new WhatsApp.ReceiptState()
            {
              MessageId = msgId,
              Jid = participantJid,
              Status = status,
              Timestamp = dt
            };
            this.Insert("ReceiptState", (object) o1);
            source.Add(o1);
            dirty = true;
          }
          else if (dt < receiptState.Timestamp)
          {
            Log.l("msg db", "msg {0}, jid {1}: Receipt timestamp [{2}] was older than previous timestamp [{3}]", (object) msgId, (object) participantJid, (object) dt, (object) receiptState.Timestamp);
            receiptState.Timestamp = dt;
            dirty = true;
          }
          return source;
        default:
          return (List<WhatsApp.ReceiptState>) null;
      }
    }

    public IEnumerable<ReceiptSpec> GetOutgoingReadReceipts(
      string jid,
      int? firstReadMsgId,
      int? lastReadMsgId)
    {
      if (firstReadMsgId.HasValue)
      {
        StringBuilder stringBuilder = new StringBuilder("SELECT KeyID, RemoteResource, TimestampLong, MediaWaType FROM Messages\n");
        stringBuilder.Append("WHERE KeyRemoteJid = ?\n");
        stringBuilder.Append("AND MessageID >= ?\n");
        if (lastReadMsgId.HasValue)
          stringBuilder.Append("AND MessageID <= ?\n");
        stringBuilder.Append("AND KeyFromMe = 0 AND MediaWaType NOT IN (?, ?)\n");
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
        {
          int num1 = 0;
          Sqlite.PreparedStatement preparedStatement1 = stmt;
          int idx1 = num1;
          int num2 = idx1 + 1;
          string val1 = jid;
          preparedStatement1.Bind(idx1, val1);
          Sqlite.PreparedStatement preparedStatement2 = stmt;
          int idx2 = num2;
          int num3 = idx2 + 1;
          int val2 = firstReadMsgId.Value;
          preparedStatement2.Bind(idx2, val2, false);
          if (lastReadMsgId.HasValue)
            stmt.Bind(num3++, lastReadMsgId.Value, false);
          Sqlite.PreparedStatement preparedStatement3 = stmt;
          int idx3 = num3;
          int num4 = idx3 + 1;
          preparedStatement3.Bind(idx3, 7L, false);
          Sqlite.PreparedStatement preparedStatement4 = stmt;
          int idx4 = num4;
          int num5 = idx4 + 1;
          preparedStatement4.Bind(idx4, 1006L, false);
          while (stmt.Step())
          {
            string str1 = jid;
            string column = (string) stmt.Columns[0];
            string str2 = (string) stmt.Columns[1];
            long seconds = stmt.Columns[2] == null ? DateTime.UtcNow.ToUnixTime() : (long) stmt.Columns[2];
            bool flag = (int) (long) stmt.Columns[3] == 1000;
            if (string.IsNullOrEmpty(str2))
              str2 = (string) null;
            else if (str2.IsBroadcastJid())
            {
              string str3 = str2;
              str2 = str1;
              str1 = str3;
            }
            yield return new ReceiptSpec()
            {
              Jid = str1,
              Id = column,
              Participant = str2,
              MessageTimestamp = new DateTime?(DateTimeUtils.FromUnixTime(seconds)),
              IsCipherText = flag
            };
          }
        }
      }
      CipherTextReceipt[] cipherTextReceipts = this.GetDecryptedCipherTextReceipts(jid);
      CipherTextReceipt[] cipherTextReceiptArray = cipherTextReceipts;
      for (int index = 0; index < cipherTextReceiptArray.Length; ++index)
      {
        CipherTextReceipt cipherTextReceipt = cipherTextReceiptArray[index];
        yield return new ReceiptSpec()
        {
          Jid = cipherTextReceipt.KeyRemoteJid,
          Id = cipherTextReceipt.KeyId,
          Participant = cipherTextReceipt.ParticipantJid,
          IsCipherText = false
        };
      }
      cipherTextReceiptArray = (CipherTextReceipt[]) null;
      this.DeleteCipherTextReceiptsOnSubmit((IEnumerable<CipherTextReceipt>) cipherTextReceipts);
    }

    public void AddParticipantsHistory(
      Conversation conversation,
      string participantJid,
      ParticipantsHashHistory.ParticipantActions action)
    {
      string participantsHash1 = conversation.ParticipantsHash;
      conversation.UpdateParticipantsHash();
      string participantsHash2 = conversation.ParticipantsHash;
      this.Insert("ParticipantsHashJournal", (object) new ParticipantsHashHistory()
      {
        OldHash = participantsHash1,
        NewHash = participantsHash2,
        ParticipantAction = action,
        GroupJid = conversation.Jid,
        ParticipantJid = participantJid,
        Timestamp = new DateTime?(DateTime.Now)
      });
    }

    public ParticipantsHashHistory[] GetParticipantsHistory(string groupJid)
    {
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM ParticipantsHashJournal\n");
      stringBuilder.Append("WHERE GroupJid = ?\n");
      stringBuilder.Append("ORDER BY Timestamp DESC");
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        stmt.Bind(0, groupJid);
        return this.ParseTable<ParticipantsHashHistory>(stmt, "ParticipantsHashJournal").ToArray<ParticipantsHashHistory>();
      }
    }

    public void PurgeParticipantsHashJournal(DateTime when)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM ParticipantsHashJournal WHERE Timestamp < ?"))
      {
        preparedStatement.Bind(0, when.ToFileTimeUtc(), false);
        preparedStatement.Step();
      }
    }

    private Dictionary<byte[], Sticker> CachedStickers
    {
      get
      {
        if (this.cachedStickers == null)
        {
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM Stickers"))
            this.cachedStickers = this.ParseTable<Sticker>(stmt, "Stickers").Where<Sticker>((Func<Sticker, bool>) (s => s.FileHash != null)).ToDictionary<Sticker, byte[], Sticker>((Func<Sticker, byte[]>) (s => s.FileHash), (Func<Sticker, Sticker>) (s => s));
        }
        return this.cachedStickers;
      }
    }

    public IEnumerable<Sticker> GetAllStickers(bool sortByWeight = true)
    {
      Dictionary<byte[], Sticker>.ValueCollection values = this.CachedStickers.Values;
      return sortByWeight ? (IEnumerable<Sticker>) values.OrderByDescending<Sticker, int>((Func<Sticker, int>) (s => s.UsageWeight)) : (IEnumerable<Sticker>) values;
    }

    public void ProcessStickerUsage(IEnumerable<Sticker> stickers)
    {
      int num1 = stickers.Count<Sticker>();
      if (num1 == 0)
        return;
      int num2 = this.CachedStickers.Count<KeyValuePair<byte[], Sticker>>();
      if (num1 * num2 < num1 * num1 + num2)
        this.ProcessStickerUsage1(stickers);
      else
        this.ProcessStickerUsage2(stickers);
    }

    private void ProcessStickerUsage2(IEnumerable<Sticker> stickers)
    {
      Dictionary<byte[], int> dictionary1 = new Dictionary<byte[], int>();
      int y = 0;
      foreach (Sticker sticker in stickers)
      {
        byte[] fileHash = sticker.FileHash;
        int num = !dictionary1.ContainsKey(fileHash) ? (int) ((double) sticker.UsageWeight * Math.Pow(0.9, (double) y)) : dictionary1[fileHash];
        Dictionary<byte[], int> dictionary2 = new Dictionary<byte[], int>();
        foreach (byte[] key in dictionary1.Keys)
          dictionary2.Add(key, (int) ((double) dictionary1[key] * 0.9));
        foreach (byte[] key in dictionary2.Keys)
          dictionary1[key] = dictionary2[key];
        dictionary1[fileHash] = num + 10000;
        ++y;
      }
      double num1 = Math.Pow(0.9, (double) y);
      foreach (Sticker sticker in this.CachedStickers.Values)
        sticker.UsageWeight = (int) ((double) sticker.UsageWeight * num1);
      foreach (Sticker sticker in stickers)
        sticker.UsageWeight = dictionary1[sticker.FileHash];
      this.SubmitChanges();
    }

    private void ProcessStickerUsage1(IEnumerable<Sticker> stickers)
    {
      foreach (Sticker sticker in stickers)
        this.ProcessSingleStickerUsage(sticker);
      this.SubmitChanges();
    }

    private void ProcessSingleStickerUsage(Sticker sticker)
    {
      int usageWeight = sticker.UsageWeight;
      foreach (Sticker sticker1 in this.CachedStickers.Values)
        sticker1.UsageWeight = (int) ((double) sticker1.UsageWeight * 0.9);
      sticker.UsageWeight = usageWeight + 10000;
    }

    public void SaveSticker(Sticker sticker)
    {
      if (!this.StickerExists(sticker.FileHash))
      {
        this.Insert("Stickers", (object) sticker);
      }
      else
      {
        StringBuilder stringBuilder = new StringBuilder("UPDATE Stickers");
        stringBuilder.Append(" SET LocalFileUri = ?, DateTimeStarred = ?, MediaKey = ?, EncodedFileHash = ?");
        stringBuilder.Append(" WHERE FileHash = ?");
        DateTime? dateTimeStarred = sticker.DateTimeStarred;
        ref DateTime? local = ref dateTimeStarred;
        long fileTimeUtc = local.HasValue ? local.GetValueOrDefault().ToFileTimeUtc() : 0L;
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
        {
          preparedStatement.Bind(0, sticker.LocalFileUri);
          preparedStatement.Bind(1, fileTimeUtc, false);
          preparedStatement.Bind(2, sticker.MediaKey);
          preparedStatement.Bind(3, sticker.EncodedFileHash);
          preparedStatement.Bind(4, sticker.FileHash);
          preparedStatement.Step();
        }
      }
      this.NotifyExternal<Sticker>(this.SavedStickerChangedSubject, sticker, "sticker saved");
    }

    public void UnsaveSticker(Sticker sticker)
    {
      if (!this.StickerExists(sticker.FileHash))
        return;
      StringBuilder stringBuilder = new StringBuilder("UPDATE Stickers");
      stringBuilder.Append(" SET DateTimeStarred = NULL");
      stringBuilder.Append(" WHERE FileHash = ?");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        preparedStatement.Bind(0, sticker.FileHash);
        preparedStatement.Step();
      }
      this.NotifyExternal<Sticker>(this.SavedStickerChangedSubject, sticker, "sticker unsaved");
    }

    public void ClearStickerLocalFileUri(Sticker sticker)
    {
      if (!this.StickerExists(sticker.FileHash))
        return;
      StringBuilder stringBuilder = new StringBuilder("UPDATE Stickers");
      stringBuilder.Append(" SET LocalFileUri = NULL");
      stringBuilder.Append(" WHERE FileHash = ?");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        preparedStatement.Bind(0, sticker.FileHash);
        preparedStatement.Step();
      }
    }

    public bool StickerExists(byte[] stickerFileHash)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT EXISTS(SELECT * FROM Stickers WHERE FileHash = ?)"))
      {
        preparedStatement.Bind(0, stickerFileHash);
        preparedStatement.Step();
        return (long) preparedStatement.Columns[0] != 0L;
      }
    }

    public Sticker GetStickerByFileHash(byte[] stickerFileHash)
    {
      Sticker r = (Sticker) null;
      this.PrepareCachedStatement(this.getStickerByHashStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, stickerFileHash);
        r = this.ParseTableFirstOrDefault<Sticker>(stmt, "Stickers");
      }));
      return r;
    }

    public bool StickerSaved(byte[] stickerFileHash)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT EXISTS(SELECT * FROM Stickers WHERE FileHash = ? AND DateTimeStarred NOT NULL)"))
      {
        preparedStatement.Bind(0, stickerFileHash);
        preparedStatement.Step();
        return (long) preparedStatement.Columns[0] != 0L;
      }
    }

    public Sticker[] GetRecentStickers(int? limit)
    {
      Sticker[] stickerArray = new Sticker[0];
      LinkedList<object> linkedList = new LinkedList<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Stickers\n");
      stringBuilder.Append("ORDER BY UsageWeight DESC\n");
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder.Append("LIMIT ? \n");
        linkedList.AddLast((object) limit.Value);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in linkedList)
          stmt.BindObject(num++, o);
        return this.ParseTable<Sticker>(stmt, "Stickers").ToArray<Sticker>();
      }
    }

    public Sticker[] GetStarredStickers(int? limit, int? offset)
    {
      Sticker[] stickerArray = new Sticker[0];
      LinkedList<object> linkedList = new LinkedList<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM Stickers WHERE DateTimeStarred NOT NULL\n");
      stringBuilder.Append("ORDER BY DateTimeStarred\n");
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder.Append("LIMIT ? \n");
        linkedList.AddLast((object) limit.Value);
      }
      if (offset.HasValue && offset.Value > 0)
      {
        stringBuilder.Append("OFFSET ? \n");
        linkedList.AddLast((object) offset.Value);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in linkedList)
          stmt.BindObject(num++, o);
        return this.ParseTable<Sticker>(stmt, "Stickers").ToArray<Sticker>();
      }
    }

    public void BackupRestoredPostProcessing(MessagesContext db)
    {
      Axolotl.ClearGroupParticipantStateSenderKey(db);
      this.SubmitChanges();
    }

    public bool Repair()
    {
      string filename = this.GetFilename();
      this.DisposeDbHandle();
      bool flag1 = false;
      ChatDatabaseRepairEvent databaseRepairEvent = new ChatDatabaseRepairEvent();
      Log.l("sqlite_repair: starting repair");
      if (SqliteRepair.IsRepairStarted())
      {
        Log.l("sqlite_repair: repair in progress, resuming");
        SqliteRepair.DumpAndRestore(filename);
      }
      else
      {
        SqliteRepair.IntegrityCheckResults integrityCheckResults = SqliteRepair.GetIntegrityCheckResults(filename);
        bool flag2 = false;
        if (flag2 || integrityCheckResults.indexesToRepair.Count > 0 && integrityCheckResults.nonIndexErrors.Count == 0)
        {
          databaseRepairEvent.databaseRepairSqliteIntegrityCheckResult = new bool?(false);
          databaseRepairEvent.repairHasOnlyIndexErrors = new bool?(true);
          Log.l("sqlite_repair: attempting reindex");
          foreach (string key in integrityCheckResults.indexesToRepair.Keys)
          {
            Log.l("sqlite_repair: reindexing {0}", key);
            SqliteRepair.Reindex(filename, key);
          }
          flag1 = !flag2 && SqliteRepair.IsDbHealthy(filename);
        }
        else if (integrityCheckResults.totalErrorCount == 0)
        {
          databaseRepairEvent.databaseRepairSqliteIntegrityCheckResult = new bool?(true);
          Log.l("sqlite_repair: db integrity seems to be ok...");
          flag1 = true;
        }
        else
        {
          foreach (string nonIndexError in integrityCheckResults.nonIndexErrors)
            Log.l("sqlite_repair: nonindex {0}", nonIndexError);
          databaseRepairEvent.databaseRepairSqliteIntegrityCheckResult = new bool?(false);
        }
        databaseRepairEvent.databaseRepairReindexingResult = new bool?(flag1);
        if (!flag1)
          SqliteRepair.DumpAndRestore(filename);
      }
      bool flag3 = Settings.MessagesDbRepairState == SqliteRepair.SqliteRepairState.Successful || flag1 && Settings.MessagesDbRepairState == SqliteRepair.SqliteRepairState.Unstarted;
      databaseRepairEvent.databaseRepairDumpAndRestoreInterrupted = !SqliteRepair.IsRepairInProgress() ? new bool?(false) : new bool?(true);
      if (flag3)
      {
        if (Settings.CorruptDb)
          Settings.CorruptDb = false;
        Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.Successful;
      }
      databaseRepairEvent.databaseRepairOverallResult = new bool?(flag3);
      databaseRepairEvent.SaveEvent();
      return flag3;
    }

    private static class PostSubmitJob
    {
      private static LinkedList<PersistentAction> persistentActionsTodo = (LinkedList<PersistentAction>) null;
      private static List<ReceiptSpec> ReadReceiptMessages = new List<ReceiptSpec>();

      public static LinkedList<PersistentAction> PersistentActions
      {
        get
        {
          return SqliteMessagesContext.PostSubmitJob.persistentActionsTodo ?? (SqliteMessagesContext.PostSubmitJob.persistentActionsTodo = new LinkedList<PersistentAction>());
        }
        set => SqliteMessagesContext.PostSubmitJob.persistentActionsTodo = value;
      }

      private static void AddPersistentAction(PersistentAction pa)
      {
        SqliteMessagesContext.PostSubmitJob.PersistentActions.AddLast(pa);
      }

      public static Action UpdateLastMessage(Conversation convo, Message msg)
      {
        return (Action) (() =>
        {
          convo.LastMessageID = new int?(msg.MessageID);
          if (!AppState.IsBackgroundAgent || !msg.IsNoteworthy())
            return;
          convo.SetFlag(Conversation.ConversationFlags.ResetDirty);
        });
      }

      public static Action UpdateLastMessage(SqliteMessagesContext db, string jid)
      {
        return (Action) (() =>
        {
          Conversation conversation = db.GetConversation(jid, CreateOptions.None);
          if (conversation == null)
            return;
          Message message = ((IEnumerable<Message>) db.GetLatestMessages(jid, conversation.MessageLoadingStart(), new int?(1), new int?())).FirstOrDefault<Message>();
          conversation.LastMessageID = message == null ? new int?() : new int?(message.MessageID);
          conversation.Timestamp = message == null ? new DateTime?() : message.FunTimestamp;
        });
      }

      public static Action UpdateFirstUnreadMessage(Conversation convo, Message message)
      {
        return (Action) (() =>
        {
          if (convo.FirstUnreadMessageID.HasValue)
            return;
          convo.FirstUnreadMessageID = new int?(message.MessageID);
        });
      }

      public static Action SaveAutodownload(SqliteMessagesContext db, Message msg)
      {
        return (Action) (() =>
        {
          Log.d("msgdb", "post submit | auto-download for new msg | {0}", (object) msg.LogInfo());
          PersistentAction persistentAction = PersistentAction.AutoDownload(msg);
          db.StorePersistentAction(persistentAction);
          SqliteMessagesContext.PostSubmitJob.AddPersistentAction(persistentAction);
        });
      }

      public static Action UpdateFrequentChatScore(
        SqliteMessagesContext db,
        string jid,
        Message[] msgs)
      {
        return (Action) (() =>
        {
          FrequentChatScore frequentChatScore = db.GetFrequentChatScore(jid, CreateOptions.CreateToDbIfNotFound);
          foreach (Message msg in msgs)
          {
            ++frequentChatScore.DefaultScore;
            frequentChatScore.ImageScore += msg.MediaWaType == FunXMPP.FMessage.Type.Image ? 10L : 1L;
            frequentChatScore.VideoScore += msg.MediaWaType == FunXMPP.FMessage.Type.Video ? 10L : 1L;
            Log.d("msgdb", "post submit | update frequent chat score | jid:{0},type:{1},scores:{2},{3},{4}", (object) msg.KeyRemoteJid, (object) msg.MediaWaType, (object) frequentChatScore.DefaultScore, (object) frequentChatScore.ImageScore, (object) frequentChatScore.VideoScore);
          }
        });
      }

      public static Action SaveMiscInfo(SqliteMessagesContext db, Message msg)
      {
        return (Action) (() =>
        {
          MessageMiscInfo miscInfo = msg.GetMiscInfo();
          if (miscInfo == null)
            return;
          Log.d("msgdb", "post submit | storing misc info for new message");
          miscInfo.MessageId = new int?(msg.MessageID);
          db.SaveMessageMiscInfoOnSubmit(miscInfo);
        });
      }

      public static Action SaveWaStatuses(SqliteMessagesContext db, List<Message> msgs)
      {
        return (Action) (() =>
        {
          Log.l("msgdb", "post submit | save statuses");
          foreach (Message msg in msgs)
          {
            PersistentAction autoDownloadPersitAction = (PersistentAction) null;
            db.SaveStatusForMessageOnSubmit(msg, out autoDownloadPersitAction);
            if (autoDownloadPersitAction != null)
              SqliteMessagesContext.PostSubmitJob.AddPersistentAction(autoDownloadPersitAction);
          }
          if (((IEnumerable<WaScheduledTask>) db.GetWaScheduledTasks(new WaScheduledTask.Types[1]
          {
            WaScheduledTask.Types.PurgeStatuses
          }, excludeExpired: false, lookupKey: "status@broadcast")).FirstOrDefault<WaScheduledTask>() != null)
            return;
          db.InsertWaScheduledTaskOnSubmit(WaStatusHelper.CreatePurgeStatusesTask());
        });
      }

      public static Action SendReadReceipt(MessagesContext db, IEnumerable<Message> msgs)
      {
        SqliteMessagesContext.PostSubmitJob.ReadReceiptMessages.AddRange(msgs.Where<Message>((Func<Message, bool>) (m => m.MediaWaType != FunXMPP.FMessage.Type.System)).Select<Message, ReceiptSpec>((Func<Message, ReceiptSpec>) (m =>
        {
          ReceiptSpec receiptSpec = new ReceiptSpec()
          {
            Jid = m.KeyRemoteJid,
            Id = m.KeyId,
            Participant = m.RemoteResource,
            MessageTimestamp = m.FunTimestamp,
            IsCipherText = m.MediaWaType == FunXMPP.FMessage.Type.CipherText
          };
          if (string.IsNullOrEmpty(receiptSpec.Participant))
            receiptSpec.Participant = (string) null;
          else if (receiptSpec.Participant.IsBroadcastJid())
          {
            string participant = receiptSpec.Participant;
            receiptSpec.Participant = receiptSpec.Jid;
            receiptSpec.Jid = participant;
          }
          return receiptSpec;
        })));
        return (Action) (() =>
        {
          if (!SqliteMessagesContext.PostSubmitJob.ReadReceiptMessages.Any<ReceiptSpec>())
            return;
          Log.d("post submit: sending read receipts for new message");
          LinkedList<PersistentAction> savedOutgoingTasks = (LinkedList<PersistentAction>) null;
          ReceiptSpec[] array = SqliteMessagesContext.PostSubmitJob.ReadReceiptMessages.ToArray();
          SqliteMessagesContext.PostSubmitJob.ReadReceiptMessages.Clear();
          ReadReceipts.ScheduleSend(db, array, out savedOutgoingTasks);
          foreach (PersistentAction pa in savedOutgoingTasks)
            SqliteMessagesContext.PostSubmitJob.AddPersistentAction(pa);
        });
      }

      private static byte[] GetPerformanceHint(Message m)
      {
        Log.d("db", "post submit | calculating performance hint");
        try
        {
          return LinkDetector.Result.Serialize(LinkDetector.GetMatches(m.GetTextForDisplay()));
        }
        finally
        {
          Log.d("db", "post submit | performance hint computed");
        }
      }

      public static Action DetectLinks(Message m)
      {
        if (AppState.IsBackgroundAgent || !m.KeyFromMe && !AppState.IsConversationOpen(m.KeyRemoteJid))
          return (Action) (() =>
          {
            if (m.TextPerformanceHint != null)
              return;
            byte[] hint = (byte[]) null;
            NativeInterfaces.Misc.LowerPriority(((Action) (() => hint = SqliteMessagesContext.PostSubmitJob.GetPerformanceHint(m))).AsComAction());
            try
            {
              MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
              {
                if (m.TextPerformanceHint != null)
                  return;
                m.TextPerformanceHint = hint;
                db.SubmitChanges();
              }));
            }
            catch (DatabaseInvalidatedException ex)
            {
            }
          });
        m.TextPerformanceHint = SqliteMessagesContext.PostSubmitJob.GetPerformanceHint(m);
        return (Action) (() => { });
      }

      public static Action IndexContactCard(SqliteMessagesContext db, Message msg)
      {
        return (Action) (() =>
        {
          foreach (ContactVCard contactCard in msg.GetContactCards())
            db.IndexContactVCard(contactCard, msg.MessageID);
        });
      }

      public static Action ProcessNewLiveLocation(SqliteMessagesContext db, Message m)
      {
        return (Action) (() =>
        {
          db.GetLatestLiveLocationMessage(m.KeyRemoteJid, m.GetSenderJid(), 1)?.EndLiveLocation();
          if (m.Status == FunXMPP.FMessage.Status.Pending)
            return;
          LiveLocationManager.Instance.LiveLocationMessageUpdated(m);
        });
      }

      public static Action IndexForSearch(SqliteMessagesContext db, Message msg)
      {
        return (Action) (() =>
        {
          try
          {
            Log.d("post submit: indexing message for search");
            db.IndexMessageById(msg.MessageID, new int?((int) msg.MediaWaType));
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "fts exception [handled]");
          }
        });
      }

      public static Action NotifyFrequentContacts()
      {
        return (Action) (() => AppState.QrPersistentAction.NotifyFrequentContacts());
      }
    }

    private enum FtsStatus
    {
      ErrorBadLanguage = -10, // 0xFFFFFFF6
      UnsupportedType = -2, // 0xFFFFFFFE
      ErrorUnknown = -1, // 0xFFFFFFFF
      NotIndexed = 0,
      Indexed = 1,
    }

    private struct JidCombo
    {
      public string Jid;
      public Message Message;
      public IEnumerable<Message> Messages;
      public int Count;
      public IEnumerable<Message> IncomingMessages;
    }

    private class MessageQueryArgs
    {
      public string ParticipantJid;
      public bool IncludeMiscInfo;
      public bool CountOnly;
      public bool WithSavedMediaOnly;
      public bool? WithLocationDetails;
      public bool? WithMediaOrigin;
      public bool StarredOnly;
      public int? LowerBoundMsgId;
      public int? UpperBoundMsgId;
      public DateTime? LowerBoundTimestampUtc;
      public DateTime? UpperBoundTimestampUtc;
      public bool IncludeLowerBound;
      public bool IncludeUpperBound;
      public int? Limit;
      public int? Offset;

      public string[] Jids { get; private set; }

      public bool Asc { get; private set; }

      public FunXMPP.FMessage.Type[] Types { get; set; }

      public MessageQueryArgs(string[] jids, bool asc)
      {
        this.Jids = jids == null ? (string[]) null : ((IEnumerable<string>) jids).Where<string>((Func<string, bool>) (jid => jid != null)).ToArray<string>();
        this.Asc = asc;
      }
    }

    public class IEnumerableWithDtor<T> : IEnumerable<T>, IEnumerable, IDisposable
    {
      private IEnumerable<T> e;
      private IDisposable d;

      public IEnumerableWithDtor(IEnumerable<T> e, IDisposable d)
      {
        this.e = e;
        this.d = d;
      }

      public IEnumerator<T> GetEnumerator() => this.e.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => this.e.GetEnumerator();

      public void Dispose() => this.d.Dispose();
    }

    private struct LocalFileHash
    {
      public byte[] Hash;
      public long? FileSize;
    }

    public enum ConversationSortTypes
    {
      None,
      TimestampOnly,
      SortKeyAndTimestamp,
    }
  }
}
