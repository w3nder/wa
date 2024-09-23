// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteContactsContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Text;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class SqliteContactsContext : SqliteDataContext
  {
    private const string LogHeader = "cdb";
    private Dictionary<string, UserStatus> userStatusByJidCache = new Dictionary<string, UserStatus>();
    private bool allStatusesRead;
    private CachedStatement cachedUserStmt = new CachedStatement("SELECT * FROM UserStatuses");
    private CachedStatement getUserStmt = new CachedStatement("SELECT * FROM UserStatuses WHERE Jid = ?");
    private CachedStatement notOnDeviceStmt = new CachedStatement("SELECT * FROM UserStatuses WHERE IsInDeviceContactList = 0");
    private CachedStatement usersWithCertificatesStmt = new CachedStatement("SELECT * FROM UserStatuses WHERE VerifiedNameCertificateDetailsSerialized IS NOT NULL");
    private CachedStatement phoneNumberForRawStmt = new CachedStatement("SELECT * FROM PhoneNumbers WHERE RawPhoneNumber = ?");
    private CachedStatement phoneNumberEnumStmt = new CachedStatement("SELECT * FROM PhoneNumbers");
    private CachedStatement phoneNumberEnumNewStmt = new CachedStatement("SELECT * FROM PhoneNumbers WHERE IsNew IS 1");
    private WhatsApp.BlockList blockListRecord;
    private Dictionary<string, bool> blockListSet;
    private static string CONVERSION_RECORD_TABLE = nameof (ConversionRecords);
    private const int DAYS_CONVERSION_RECORD_IS_VALID = 7;

    public Table<PhoneNumber> PhoneNumbers { get; set; }

    public Table<UserStatus> UserStatuses { get; set; }

    public Table<WhatsApp.BlockList> BlockList { get; set; }

    public Table<ChatPicture> ChatPictures { get; set; }

    public Table<ClientCapability> ClientCapabilities { get; set; }

    public Table<ChangeNumberRecord> ChangeNumberRecords { get; set; }

    public Table<ConversionRecord> ConversionRecords { get; set; }

    public Table<WaScheduledTask> WaScheduledTasks { get; set; }

    public Subject<DbDataUpdate> UserStatusUpdatedSubject
    {
      get => ContactsContext.Events.UserStatusUpdatedSubject;
    }

    public Subject<SqliteContactsContext.ChangeNumberAction> ChangeNumberActionSubject
    {
      get => ContactsContext.Events.ChangeNumberActionSubject;
    }

    public Subject<Unit> StatusUpdateSubject => ContactsContext.Events.StatusUpdateSubject;

    public ReplaySubject<Unit> ColdSyncErrorSubject => ContactsContext.Events.ColdSyncErrorSubject;

    public Subject<IEnumerable<string>> BlockListUpdateSubject
    {
      get => ContactsContext.Events.BlockListUpdateSubject;
    }

    public SqliteContactsContext(string filename = "contacts.db", SqliteSynchronizeOptions? syncMode = null)
      : base(filename, sync: syncMode)
    {
      this.IsLockHeld = (Func<bool>) (() => ContactsContext.Mutex.IsOwner());
      this.LatestSchemaVersion = 21;
      if (!this.DatabaseExists())
        return;
      int schemaVersion = this.GetSchemaVersion();
      bool flag = this.TableExists("UserStatusesFts");
      if (schemaVersion >= this.LatestSchemaVersion && flag)
        return;
      this.BeginTransaction();
      try
      {
        if (schemaVersion < this.LatestSchemaVersion)
        {
          this.UpdateSchema(schemaVersion);
          this.SetSchemaVersion(this.LatestSchemaVersion);
        }
        if (!flag)
        {
          try
          {
            this.CreateUserStatusesFtsTable();
            this.IndexAllContactsForSearch(false);
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "fts exception [handled]");
          }
        }
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.Dispose();
        throw;
      }
    }

    protected override void OnDbOpened(Sqlite db) => db.RegisterTokenizer();

    protected override void CreateTableOverride() => this.CreateUserStatusesFtsTable();

    private void UpdateSchema(int schema)
    {
      if (schema < 1)
        this.AddColumn("UserStatuses", "FirstName");
      if (schema < 2)
      {
        this.AddColumn("PhoneNumbers", "IsNew");
        this.AddIndex("PhoneNumbers", "IsNewIdx");
      }
      if (schema < 3)
        this.AddColumn("UserStatuses", "PushName");
      if (schema < 4)
        this.AddTable("ChatPictures");
      if (schema < 5)
        this.AddColumn("ChatPictures", "SavedPhotoId");
      if (schema < 6)
        this.AddColumn("ChatPictures", "BlockPictureRequestUntil");
      if (schema < 7)
      {
        this.AddColumn("ChatPictures", "PictureData");
        this.AddColumn("ChatPictures", "ThumbnailData");
      }
      if (schema < 8)
        this.AddTable("ClientCapabilities");
      if (schema < 9)
        this.AddColumn("UserStatuses", "VerifiedName");
      if (schema < 11)
        this.CleanupNonUserJids();
      if (schema < 12)
        this.AddColumn("UserStatuses", "VerifiedNameCertificateDetailsSerialized");
      if (schema < 13)
        this.AddColumn("UserStatuses", "IsWaUser");
      if (schema < 14)
        this.AddTable("ChangeNumberRecords");
      if (schema < 15)
        this.AddTable(SqliteContactsContext.CONVERSION_RECORD_TABLE);
      if (schema < 16)
        this.AddColumn("UserStatuses", "VerifiedLevel");
      if (schema < 17)
        this.AddColumn("UserStatuses", "InternalPropertiesProtobuf");
      if (schema < 18)
        this.AddColumn(SqliteContactsContext.CONVERSION_RECORD_TABLE, "LastActionTimestamp");
      if (schema < 19)
        this.AddTable("WaScheduledTasks");
      if (schema < 20)
        this.AddColumn("UserStatuses", "IsSidelistSynced");
      if (schema >= 21)
        return;
      this.AddColumn("UserStatuses", "IsInDevicePhonebook");
    }

    private void CreateUserStatusesFtsTable()
    {
      if (!this.TableExists("UserStatusesFts"))
      {
        Log.l("cdb", "creating UserStatusesFts table");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE VIRTUAL TABLE UserStatusesFts USING fts4(content=\"UserStatuses\", Jid, ContactName, Status, tokenize=wa_tokenizer simple)"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFts table created");
        }
      }
      Set<string> tableMetadata = this.GetTableMetadata("UserStatuses", "trigger");
      if (!tableMetadata.Contains("UserStatusesFtsTriggerBU"))
      {
        Log.l("cdb", "creating UserStatusesFtsTriggerBU trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE TRIGGER UserStatusesFtsTriggerBU BEFORE UPDATE ON UserStatuses BEGIN DELETE FROM UserStatusesFts WHERE docid=old.rowid; END"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerBU created");
        }
      }
      if (!tableMetadata.Contains("UserStatusesFtsTriggerBD"))
      {
        Log.l("cdb", "creating UserStatusesFtsTriggerBD trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE TRIGGER UserStatusesFtsTriggerBD BEFORE DELETE ON UserStatuses BEGIN DELETE FROM UserStatusesFts WHERE docid=old.rowid; END"))
        {
          preparedStatement.Step();
          Log.WriteLineDebug("contacts db: UserStatusesFtsTriggerBD created");
        }
      }
      if (!tableMetadata.Contains("UserStatusesFtsTriggerAU"))
      {
        Log.l("cdb", "creating UserStatusesFtsTriggerAU trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE TRIGGER UserStatusesFtsTriggerAU AFTER UPDATE ON UserStatuses WHEN new.IsInDeviceContactList = 1 BEGIN INSERT INTO UserStatusesFts (docid, Jid, ContactName, Status) VALUES (new.rowid, new.Jid, new.ContactName, new.Status); END"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerAU created");
        }
      }
      if (tableMetadata.Contains("UserStatusesFtsTriggerAI"))
        return;
      Log.l("cdb", "creating UserStatusesFtsTriggerAI trigger");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("CREATE TRIGGER UserStatusesFtsTriggerAI AFTER INSERT ON UserStatuses WHEN new.IsInDeviceContactList = 1 BEGIN INSERT INTO UserStatusesFts (docid, Jid, ContactName, Status) VALUES (new.rowid, new.Jid, new.ContactName, new.Status); END"))
      {
        preparedStatement.Step();
        Log.l("cdb", "UserStatusesFtsTriggerAI created");
      }
    }

    private void DropUserStatusesFtsTableImpl()
    {
      if (!this.Db.IsTokenizerRegistered())
        this.Db.RegisterTokenizer();
      Set<string> tableMetadata = this.GetTableMetadata("UserStatuses", "trigger");
      if (tableMetadata.Contains("UserStatusesFtsTriggerBU"))
      {
        Log.l("cdb", "dropping UserStatusesFtsTriggerBU trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TRIGGER UserStatusesFtsTriggerBU"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerBU dropped");
        }
      }
      if (tableMetadata.Contains("UserStatusesFtsTriggerBD"))
      {
        Log.l("cdb", "dropping UserStatusesFtsTriggerBD trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TRIGGER UserStatusesFtsTriggerBD"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerBD dropped");
        }
      }
      if (tableMetadata.Contains("UserStatusesFtsTriggerAU"))
      {
        Log.l("cdb", "dropping UserStatusesFtsTriggerAU trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TRIGGER UserStatusesFtsTriggerAU"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerAU dropped");
        }
      }
      if (tableMetadata.Contains("UserStatusesFtsTriggerAI"))
      {
        Log.l("cdb", "dropping UserStatusesFtsTriggerAI trigger");
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TRIGGER UserStatusesFtsTriggerAI"))
        {
          preparedStatement.Step();
          Log.l("cdb", "UserStatusesFtsTriggerAI dropped");
        }
      }
      if (!this.TableExists("UserStatusesFts"))
        return;
      Log.l("cdb", "dropping UserStatusesFts table");
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DROP TABLE UserStatusesFts"))
      {
        preparedStatement.Step();
        Log.l("cdb", "UserStatusesFts table dropped");
      }
    }

    public void ClearUserStatusesFtsTable()
    {
      this.BeginTransaction();
      try
      {
        this.DropUserStatusesFtsTableImpl();
        this.CreateUserStatusesFtsTable();
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
    }

    private void DropUserStatusesFtsTable()
    {
      this.BeginTransaction();
      try
      {
        this.DropUserStatusesFtsTableImpl();
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
    }

    public void IndexContactForSearch(string jid)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("INSERT INTO UserStatusesFts (docid, Jid, ContactName, Status) SELECT StatusID, Jid, ContactName, Status FROM UserStatuses WHERE Jid = ?"))
      {
        preparedStatement.Bind(0, jid);
        preparedStatement.Step();
      }
    }

    public void IndexAllContactsForSearch(bool tx = true)
    {
      List<string> stringList = new List<string>();
      if (tx)
        this.BeginTransaction();
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT Jid FROM UserStatuses WHERE IsInDeviceContactList = 1"))
        {
          while (preparedStatement.Step())
          {
            string column = (string) preparedStatement.Columns[0];
            stringList.Add(column);
          }
        }
        Log.l("cdb", "collected {0} contacts for search indexing", (object) stringList.Count);
        foreach (string jid in stringList)
          this.IndexContactForSearch(jid);
        if (tx)
          this.CommitTransaction();
      }
      catch (Exception ex)
      {
        if (tx)
          this.RollbackTransaction(ex);
        throw;
      }
      Log.l("cdb", "indexed {0} contacts for search", (object) stringList.Count);
    }

    public List<UserStatus> LookupUsersByName(string ftsQuery)
    {
      return this.LookupUsersImpl(ftsQuery, "ContactName");
    }

    public List<UserStatus> LookupUsers(string ftsQuery) => this.LookupUsersImpl(ftsQuery);

    public List<string> LookupJidsByName(string ftsQuery)
    {
      return this.LookupJidsImpl(ftsQuery, "ContactName");
    }

    private List<string> LookupJidsImpl(string ftsQuery, string columnName = null)
    {
      List<string> stringList = new List<string>();
      string sql = string.Format("SELECT Jid FROM UserStatusesFts WHERE {0} MATCH ? ORDER BY ContactName", columnName == null ? (object) "UserStatusesFts" : (object) columnName);
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(sql))
        {
          preparedStatement.Bind(0, ftsQuery);
          while (preparedStatement.Step())
            stringList.Add((string) preparedStatement.Columns[0]);
        }
      }
      catch (Exception ex)
      {
        stringList = new List<string>();
        Log.l(ex, "fts query user status");
      }
      return stringList;
    }

    private List<UserStatus> LookupUsersImpl(string ftsQuery, string columnName = null)
    {
      List<string> stringList = this.LookupJidsImpl(ftsQuery, columnName);
      return this.RemoveInvalidJids((stringList.Any<string>() ? this.GetUserStatuses((IEnumerable<string>) stringList, false, false) : new List<UserStatus>()).ToArray());
    }

    public UserStatusSearchResult[] QueryUserStatusesFtsTableWithOffsets(string query)
    {
      List<UserStatusSearchResult> statusSearchResultList;
      try
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT Jid, offsets(UserStatusesFts) FROM UserStatusesFts WHERE UserStatusesFts MATCH ? ORDER BY ContactName"))
        {
          preparedStatement.Bind(0, query);
          while (preparedStatement.Step())
          {
            string column1 = (string) preparedStatement.Columns[0];
            string column2 = (string) preparedStatement.Columns[1];
            dictionary[column1] = column2;
          }
        }
        statusSearchResultList = new List<UserStatusSearchResult>(dictionary.Count);
        foreach (string key in dictionary.Keys)
        {
          UserStatus userStatus = this.GetUserStatus(key, false);
          if (userStatus != null)
          {
            UserStatusSearchResult statusSearchResult = this.BuildUserStatusSearchResult(userStatus, dictionary[key]);
            statusSearchResultList.Add(statusSearchResult);
          }
        }
      }
      catch (Exception ex)
      {
        throw;
      }
      return statusSearchResultList.ToArray();
    }

    private UserStatusSearchResult BuildUserStatusSearchResult(
      UserStatus userStatus,
      string offsets)
    {
      UserStatusSearchResult statusSearchResult = new UserStatusSearchResult();
      statusSearchResult.UserStatus = userStatus;
      if (offsets == null)
        return statusSearchResult;
      string[] strArray = offsets.Split(' ');
      if (strArray.Length == 0 || strArray.Length % 4 != 0)
      {
        Log.l("cdb", "invalid number of search offsets: {0} -> {1}", (object) strArray.Length, (object) offsets);
        return statusSearchResult;
      }
      Set<int> set1 = (Set<int>) null;
      Set<int> set2 = (Set<int>) null;
      List<Pair<int, int>> pairList1 = (List<Pair<int, int>>) null;
      List<Pair<int, int>> pairList2 = (List<Pair<int, int>>) null;
      for (int index = 0; index < strArray.Length; index += 4)
      {
        int num1 = int.Parse(strArray[index], (IFormatProvider) CultureInfo.InvariantCulture);
        int.Parse(strArray[index + 1], (IFormatProvider) CultureInfo.InvariantCulture);
        int num2 = int.Parse(strArray[index + 2], (IFormatProvider) CultureInfo.InvariantCulture);
        int second = int.Parse(strArray[index + 3], (IFormatProvider) CultureInfo.InvariantCulture);
        switch (num1)
        {
          case 0:
            statusSearchResult.JidMatched = true;
            break;
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
        }
      }
      if (pairList1 != null)
      {
        statusSearchResult.ContactNameOffsets = pairList1.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(statusSearchResult.UserStatus.ContactName, statusSearchResult.ContactNameOffsets);
      }
      if (pairList2 != null)
      {
        statusSearchResult.StatusOffsets = pairList2.ToArray();
        SqliteDataContext.ConvertOffsetEncoding(statusSearchResult.UserStatus.Status, statusSearchResult.StatusOffsets);
      }
      return statusSearchResult;
    }

    public override void SubmitChanges()
    {
      PerformanceTimer.Start();
      List<WaScheduledTask> postSubmitTasks = new List<WaScheduledTask>();
      SqliteDataContext.ChangeSet changeSet = this.GetChangeSet();
      Pair<object, string[]>[] userUpdates = changeSet.Updates.Where<Pair<object, string[]>>((Func<Pair<object, string[]>, bool>) (p => p.First is UserStatus)).ToArray<Pair<object, string[]>>();
      if (((IEnumerable<Pair<object, string[]>>) userUpdates).Any<Pair<object, string[]>>())
      {
        List<WaScheduledTask> postSubmitTasks1 = (List<WaScheduledTask>) null;
        this.ProcessUserUpdatesPreSubmit(userUpdates, out postSubmitTasks1);
        if (postSubmitTasks1 != null)
          postSubmitTasks.AddRange((IEnumerable<WaScheduledTask>) postSubmitTasks1);
      }
      Action action = (Action) null;
      if (!AppState.UIUpdatesMuted)
      {
        UserStatus[] userInserts = changeSet.Inserts.Where<object>((Func<object, bool>) (o => o is UserStatus)).Cast<UserStatus>().ToArray<UserStatus>();
        UserStatus[] userDeletes = changeSet.Deletes.Where<object>((Func<object, bool>) (o => o is UserStatus)).Cast<UserStatus>().ToArray<UserStatus>();
        action = (Action) (() =>
        {
          foreach (UserStatus updatedObj in userInserts)
          {
            this.UserStatusUpdatedSubject.OnNext(new DbDataUpdate((object) updatedObj, DbDataUpdate.Types.Added));
            if (this.allStatusesRead)
              this.userStatusByJidCache[updatedObj.Jid] = updatedObj;
          }
          foreach (UserStatus updatedObj in userDeletes)
          {
            this.UserStatusUpdatedSubject.OnNext(new DbDataUpdate((object) updatedObj, DbDataUpdate.Types.Deleted));
            if (this.allStatusesRead && this.userStatusByJidCache.ContainsKey(updatedObj.Jid))
              this.userStatusByJidCache.Remove(updatedObj.Jid);
          }
          foreach (Pair<object, string[]> pair in userUpdates)
          {
            if (pair.First is UserStatus first2)
            {
              this.UserStatusUpdatedSubject.OnNext(new DbDataUpdate((object) first2, pair.Second));
              if (this.allStatusesRead)
                this.userStatusByJidCache[first2.Jid] = first2;
            }
          }
        });
      }
      this.BeginTransaction();
      bool flag = true;
      try
      {
        this.SubmitChanges(false);
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        if (flag)
          this.RollbackTransaction(ex);
        throw;
      }
      if (action != null)
        action();
      if (!postSubmitTasks.Any<WaScheduledTask>())
        return;
      AppState.Worker.Enqueue((Action) (() =>
      {
        foreach (WaScheduledTask task in postSubmitTasks)
          SqliteContactsContext.AttemptScheduledTask(task);
      }));
    }

    private void ProcessUserUpdatesPreSubmit(
      Pair<object, string[]>[] userUpdates,
      out List<WaScheduledTask> postSubmitTasks)
    {
      postSubmitTasks = new List<WaScheduledTask>();
      if (userUpdates == null)
        return;
      foreach (Pair<object, string[]> userUpdate in userUpdates)
      {
        if (((IEnumerable<string>) userUpdate.Second).Contains<string>("VerifiedLevel") && userUpdate.First is UserStatus first)
        {
          Log.l("cdb", "v level changed | jid:{0},new level:{1}", (object) first.Jid, (object) first.VerifiedLevel);
          Triad<VerifiedLevel, string, VerifiedTier> verifiedStateForDisplay = first.GetVerifiedStateForDisplay();
          Triad<VerifiedLevel, string, VerifiedTier> displayedVerifiedState = first.GetLastDisplayedVerifiedState();
          if (verifiedStateForDisplay.Third == displayedVerifiedState.Third)
          {
            Log.l("cdb", "skip sys msg generation | jid:{0},level:{1}->{2}, tier:{3}->{4}", (object) first.Jid, (object) displayedVerifiedState.First, (object) verifiedStateForDisplay.First, (object) displayedVerifiedState.Third, (object) verifiedStateForDisplay.Third);
          }
          else
          {
            Log.l("cdb", "scheduled sys msg generation | jid:{0},level:{1}->{2}", (object) first.Jid, (object) displayedVerifiedState.First, (object) verifiedStateForDisplay.First);
            WaScheduledTask systemMessageTask = SystemMessageUtils.CreateGenerateTransitBizSystemMessageTask(first.Jid, verifiedStateForDisplay.First, verifiedStateForDisplay.Second, displayedVerifiedState.First, displayedVerifiedState.Second, first.IsInDevicePhonebook, first.VerifiedNameMatchesContactName(), verifiedStateForDisplay.Third, displayedVerifiedState.Third);
            this.InsertScheduledTaskOnSubmit(systemMessageTask);
            postSubmitTasks.Add(systemMessageTask);
          }
          UserStatusProperties forUserStatus = UserStatusProperties.GetForUserStatus(first);
          forUserStatus.EnsureBusinessUserProperties.LastDisplayedVerifiedLevel = new int?((int) verifiedStateForDisplay.First);
          forUserStatus.EnsureBusinessUserProperties.LastDisplayedVerifiedName = verifiedStateForDisplay.Second;
          forUserStatus.EnsureBusinessUserProperties.LastDisplayedTier = new int?((int) verifiedStateForDisplay.Third);
          forUserStatus.Save();
        }
      }
    }

    protected bool ResetImpl()
    {
      this.DisposeDbHandle();
      return false;
    }

    public void ReadAllUserStatuses()
    {
      if (this.allStatusesRead)
        return;
      foreach (UserStatus user in this.GetUserList(this.cachedUserStmt))
        this.userStatusByJidCache[user.Jid] = user;
      this.allStatusesRead = true;
    }

    public void ClearCache()
    {
      if (!this.allStatusesRead)
        return;
      this.userStatusByJidCache.Clear();
      this.allStatusesRead = false;
    }

    public IEnumerable<UserStatus> CachedUsers
    {
      get
      {
        return this.allStatusesRead ? this.userStatusByJidCache.Values.AsEnumerable<UserStatus>() : (IEnumerable<UserStatus>) this.GetUserList(this.cachedUserStmt);
      }
    }

    private UserStatus GetCachedUserStatus(string jid)
    {
      UserStatus userStatus = (UserStatus) null;
      return jid == null || !this.userStatusByJidCache.TryGetValue(jid, out userStatus) ? (UserStatus) null : userStatus;
    }

    private UserStatus GetUserStatus(string jid, GetUserStatusFlags flags)
    {
      if (!JidChecker.CheckUserJidProtocolString(jid))
      {
        if ((flags & GetUserStatusFlags.CreateIfNotFound) != GetUserStatusFlags.None)
        {
          JidChecker.SendJidErrorClb("Get/Create User Status", jid);
        }
        else
        {
          JidChecker.MaybeSendJidErrorClb("Get User status", jid);
          return (UserStatus) null;
        }
      }
      UserStatus ret = this.GetCachedUserStatus(jid);
      if ((flags & GetUserStatusFlags.SearchCacheOnly) == GetUserStatusFlags.None)
      {
        if (ret != null)
        {
          int? cacheVersion1 = this.GetCacheVersion("UserStatuses", (object) ret.StatusID);
          int cacheVersion2 = this.GetCacheVersion();
          if ((cacheVersion1.GetValueOrDefault() == cacheVersion2 ? (cacheVersion1.HasValue ? 1 : 0) : 0) != 0)
            goto label_7;
        }
        this.PrepareCachedStatement(this.getUserStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
        {
          stmt.Bind(0, jid);
          ret = this.ParseTable<UserStatus>(stmt, "UserStatuses").SingleOrDefault<UserStatus>();
        }));
        if (ret == null && (flags & GetUserStatusFlags.CreateIfNotFound) != GetUserStatusFlags.None)
        {
          UserStatus o = new UserStatus()
          {
            Jid = jid,
            IsInDevicePhonebook = false,
            IsInDeviceContactList = false
          };
          this.Insert("UserStatuses", (object) o);
          this.SubmitChanges();
          if (Settings.LastFullSyncUtc.HasValue && !AppState.UIUpdatesMuted)
            this.StatusUpdateSubject.OnNext(new Unit());
          ret = o;
        }
        if (ret != null)
          this.userStatusByJidCache[jid] = ret;
        return ret;
      }
label_7:
      return ret;
    }

    public UserStatus GetUserStatus(string jid, bool createIfNotFound = true)
    {
      GetUserStatusFlags flags = GetUserStatusFlags.None;
      if (createIfNotFound)
        flags |= GetUserStatusFlags.CreateIfNotFound;
      return this.GetUserStatus(jid, flags);
    }

    public List<UserStatus> GetUserStatuses(
      IEnumerable<string> inputJids,
      bool createIfNotFound,
      bool knownContactsOnly)
    {
      List<string> source1 = new List<string>();
      foreach (string str in (IEnumerable<string>) ((object) inputJids ?? (object) new string[0]))
      {
        if (!JidChecker.CheckUserJidProtocolString(str))
        {
          if (createIfNotFound)
          {
            Log.l("cdb", string.Format("Required user has invalid jid [{0}]", (object) str));
            JidChecker.SendJidErrorClb("Get/Create User Statuses", str);
          }
          else
          {
            JidChecker.MaybeSendJidErrorClb("Get User statuses", str);
            continue;
          }
        }
        source1.Add(str);
      }
      if (source1 == null || !source1.Any<string>())
        return new List<UserStatus>();
      List<UserStatus> userStatuses = new List<UserStatus>(source1.Count<string>());
      HashSet<string> source2 = new HashSet<string>();
      foreach (string key in source1)
      {
        UserStatus userStatus = (UserStatus) null;
        if (this.userStatusByJidCache.TryGetValue(key, out userStatus))
          userStatuses.Add(userStatus);
        else
          source2.Add(key);
      }
      if (!source2.Any<string>())
        return userStatuses;
      int count = source2.Count;
      int num1 = 0;
      string[] array = source2.ToArray<string>();
      while (num1 < count)
      {
        int num2 = Math.Min(count - num1, 999);
        StringBuilder stringBuilder = new StringBuilder("SELECT * FROM UserStatuses \nWHERE Jid IN (");
        for (int index = 1; index < num2; ++index)
          stringBuilder.Append("?,");
        stringBuilder.Append("?) \n");
        if (knownContactsOnly)
          stringBuilder.Append("AND IsInDeviceContactList = 1");
        using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
        {
          int num3;
          for (int index = 0; index < num2; index = num3 + 1)
          {
            Sqlite.PreparedStatement preparedStatement = stmt;
            int idx = index;
            num3 = idx + 1;
            string val = array[num1++];
            preparedStatement.Bind(idx, val);
          }
          foreach (UserStatus userStatus in this.ParseTable<UserStatus>(stmt, "UserStatuses"))
          {
            this.userStatusByJidCache[userStatus.Jid] = userStatus;
            userStatuses.Add(userStatus);
            source2.Remove(userStatus.Jid);
          }
        }
      }
      if (createIfNotFound && !knownContactsOnly && source2.Any<string>())
      {
        foreach (string jid in source2)
          userStatuses.Add(this.GetUserStatus(jid));
      }
      return userStatuses;
    }

    private static string FavoritesQuery(string selection, bool count)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("SELECT ");
      if (count)
        stringBuilder.AppendFormat("COUNT({0})", (object) selection);
      else
        stringBuilder.Append(selection);
      stringBuilder.Append(" FROM UserStatuses\nWHERE IsInDeviceContactList = 1 and OmitFromFavorites IS NOT 1");
      return stringBuilder.ToString();
    }

    public List<string> GetWaContactJids(bool includeSelf)
    {
      List<string> waContactJids = new List<string>();
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT Jid FROM UserStatuses WHERE IsInDeviceContactList = 1 \n");
      if (!includeSelf)
      {
        stringBuilder.Append(" AND Jid != ?");
        objectList.Add((object) Settings.MyJid);
      }
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          preparedStatement.BindObject(num++, o);
        while (preparedStatement.Step())
        {
          string column = (string) preparedStatement.Columns[0];
          if (JidChecker.CheckUserJidProtocolString(column))
            waContactJids.Add(column);
          else
            JidChecker.MaybeSendJidErrorClb("Get Wa Contacts", column);
        }
      }
      return waContactJids;
    }

    public UserStatus[] GetWaContacts(bool includeSelf)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM UserStatuses\n");
      stringBuilder.Append("WHERE IsInDeviceContactList = 1");
      if (!includeSelf)
      {
        stringBuilder.Append(" AND Jid != ?");
        objectList.Add((object) Settings.MyJid);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          stmt.BindObject(num++, o);
        return this.RemoveInvalidJids(this.ParseTable<UserStatus>(stmt, "UserStatuses").ToArray<UserStatus>()).ToArray();
      }
    }

    private UserStatus[] GetUserList(CachedStatement cachedStmt)
    {
      UserStatus[] fullList = (UserStatus[]) null;
      this.PrepareCachedStatement(cachedStmt, (Action<Sqlite.PreparedStatement>) (stmt => fullList = this.ParseTable<UserStatus>(stmt, "UserStatuses").ToArray<UserStatus>()));
      return this.RemoveInvalidJids(fullList).ToArray();
    }

    private List<UserStatus> RemoveInvalidJids(UserStatus[] fullList)
    {
      List<UserStatus> userStatusList = new List<UserStatus>();
      if (fullList == null || fullList.Length < 1)
        return userStatusList;
      foreach (UserStatus full in fullList)
      {
        if (JidChecker.CheckJidProtocolString(full.Jid))
          userStatusList.Add(full);
        else
          JidChecker.MaybeSendJidErrorClb("Invalid jid in UserStatus", full.Jid);
      }
      return userStatusList;
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
          JidChecker.MaybeSendJidErrorClb("Invalid jid in ConversationItem", full.Jid);
      }
      return conversationItemList;
    }

    public UserStatus[] UserStatusesNotOnDevice() => this.GetUserList(this.notOnDeviceStmt);

    public UserStatus[] UserStatusesWithCertificates()
    {
      return this.GetUserList(this.usersWithCertificatesStmt);
    }

    public void CleanupNonUserJids()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM UserStatuses WHERE Jid NOT LIKE ?"))
      {
        preparedStatement.Bind(0, "%@s.whatsapp.net");
        preparedStatement.Step();
        preparedStatement.Reset();
      }
    }

    public PhoneNumber PhoneNumberForRawNumber(string raw)
    {
      PhoneNumber r = (PhoneNumber) null;
      this.PrepareCachedStatement(this.phoneNumberForRawStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, raw);
        r = this.ParseTableFirstOrDefault<PhoneNumber>(stmt, "PhoneNumbers");
      }));
      return r;
    }

    public string JidForRawNumber(string rawNumber)
    {
      string jid = (string) null;
      this.PrepareCachedStatement(this.phoneNumberForRawStmt, (Action<Sqlite.PreparedStatement>) (stmt =>
      {
        stmt.Bind(0, rawNumber);
        PhoneNumber tableFirstOrDefault = this.ParseTableFirstOrDefault<PhoneNumber>(stmt, "PhoneNumbers");
        jid = tableFirstOrDefault == null ? (string) null : tableFirstOrDefault.Jid;
      }));
      return jid;
    }

    private PhoneNumber[] GetPhoneNumberList(CachedStatement cachedStmt)
    {
      PhoneNumber[] r = (PhoneNumber[]) null;
      this.PrepareCachedStatement(cachedStmt, (Action<Sqlite.PreparedStatement>) (stmt => r = this.ParseTable<PhoneNumber>(stmt, "PhoneNumbers").ToArray<PhoneNumber>()));
      return r;
    }

    public PhoneNumber[] GetAllPhoneNumbers() => this.GetPhoneNumberList(this.phoneNumberEnumStmt);

    public PhoneNumber[] GetNewNumbers() => this.GetPhoneNumberList(this.phoneNumberEnumNewStmt);

    public void MarkNumbersAsOld()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE PhoneNumbers SET IsNew = NULL"))
        preparedStatement.Step();
    }

    public void MarkNumbersAsOld(IEnumerable<PhoneNumber> numbers)
    {
      this.MarkNumbersAsOld((IEnumerable<string>) ((IEnumerable<PhoneNumber>) ((object) numbers ?? (object) new PhoneNumber[0])).Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.Jid)).ToArray<string>());
    }

    public void MarkNumbersAsOld(IEnumerable<string> jids)
    {
      if (jids == null || jids.Count<string>() <= 0)
        return;
      this.Db.BeginTransaction();
      foreach (string jid in jids)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE PhoneNumbers SET IsNew = NULL WHERE Jid = ?"))
        {
          preparedStatement.Bind(0, jid);
          preparedStatement.Step();
        }
      }
      this.Db.CommitTransaction();
    }

    public void MarkNumbersAsNew(IEnumerable<PhoneNumber> numbers)
    {
      this.MarkNumbersAsNew((IEnumerable<string>) ((IEnumerable<PhoneNumber>) ((object) numbers ?? (object) new PhoneNumber[0])).Select<PhoneNumber, string>((Func<PhoneNumber, string>) (pn => pn.Jid)).ToArray<string>());
    }

    public void MarkNumbersAsNew(IEnumerable<string> jids)
    {
      if (jids == null || jids.Count<string>() <= 0)
        return;
      this.Db.BeginTransaction();
      foreach (string jid in jids)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("UPDATE PhoneNumbers SET IsNew = 1 WHERE Jid = ?"))
        {
          preparedStatement.Bind(0, jid);
          preparedStatement.Step();
        }
      }
      this.Db.CommitTransaction();
    }

    public void InsertPhoneNumberOnSubmit(PhoneNumber n) => this.Insert("PhoneNumbers", (object) n);

    public void DeletePhoneNumberOnSubmit(PhoneNumber n) => this.Delete("PhoneNumbers", (object) n);

    public ChangeNumberRecord[] GetChangeNumberRecords(
      string oldJid,
      string newJid,
      TimeSpan withinTimespan,
      int? limit)
    {
      List<object> objectList = new List<object>();
      StringBuilder stringBuilder = new StringBuilder("SELECT * FROM ChangeNumberRecords \n");
      List<string> stringList = new List<string>();
      if (oldJid != null)
      {
        stringList.Add("OldJid = ?");
        objectList.Add((object) oldJid);
      }
      if (newJid != null)
      {
        stringList.Add("NewJid = ?");
        objectList.Add((object) newJid);
      }
      stringList.Add("Timestamp > ?");
      objectList.Add((object) (FunRunner.CurrentServerTimeUtc - withinTimespan).ToFileTimeUtc());
      if (stringList.Any<string>())
      {
        stringBuilder.Append("WHERE ");
        stringBuilder.Append(string.Join(" AND ", (IEnumerable<string>) stringList));
        stringBuilder.Append(" \n");
      }
      if (limit.HasValue && limit.Value > 0)
      {
        stringBuilder.Append(" LIMIT ? \n");
        objectList.Add((object) limit.Value);
      }
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(stringBuilder.ToString()))
      {
        int num = 0;
        foreach (object o in objectList)
          stmt.BindObject(num++, o);
        return this.ParseTable<ChangeNumberRecord>(stmt, "ChangeNumberRecords").ToArray<ChangeNumberRecord>();
      }
    }

    private void InsertChangeNumberRecordForJid(string oldJid, string newJid, DateTime dtUtc)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("INSERT INTO ChangeNumberRecords(OldJid, NewJid, Timestamp) VALUES (?, ?, ?)"))
      {
        preparedStatement.Bind(0, oldJid);
        preparedStatement.Bind(1, newJid);
        preparedStatement.Bind(2, dtUtc.ToFileTimeUtc(), false);
        preparedStatement.Step();
      }
      this.ChangeNumberActionSubject.OnNext(new SqliteContactsContext.ChangeNumberAction(oldJid, newJid, SqliteContactsContext.ChangeNumberAction.Type.Added));
    }

    public void AddChangeNumberRecord(string oldJid, string newJid, DateTime dtUtc)
    {
      this.DeleteChangeNumberRecordsForJid(new string[2]
      {
        oldJid,
        newJid
      });
      this.InsertChangeNumberRecordForJid(oldJid, newJid, dtUtc);
      this.PurgeCache("ChangeNumberRecords");
    }

    public Pair<string, string> GetChangeNumberJidsForPanel(string currJid)
    {
      if (!JidHelper.IsUserJid(currJid))
        return (Pair<string, string>) null;
      UserStatus userStatus = this.GetUserStatus(currJid, false);
      if (userStatus != null)
      {
        string str1 = currJid;
        string newJid = ((IEnumerable<ChangeNumberRecord>) this.GetChangeNumberRecords(str1, (string) null, TimeSpan.FromDays(30.0), new int?(1))).FirstOrDefault<ChangeNumberRecord>()?.NewJid;
        if (newJid != null && JidHelper.IsUserJid(newJid))
          return new Pair<string, string>(str1, newJid);
        string str2 = currJid;
        string oldJid = ((IEnumerable<ChangeNumberRecord>) this.GetChangeNumberRecords((string) null, str2, TimeSpan.FromDays(30.0), new int?(1))).FirstOrDefault<ChangeNumberRecord>()?.OldJid;
        if (oldJid != null && JidHelper.IsUserJid(oldJid) && !userStatus.IsInDevicePhonebook)
          return new Pair<string, string>(oldJid, str2);
      }
      return (Pair<string, string>) null;
    }

    public void DeleteChangeNumberRecordsForJid(string[] jids)
    {
      foreach (string jid in jids)
      {
        foreach (ChangeNumberRecord changeNumberRecord in this.GetChangeNumberRecordsForJid(jid))
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM ChangeNumberRecords WHERE OldJid = ? AND NewJid = ?"))
          {
            preparedStatement.Bind(0, changeNumberRecord.OldJid);
            preparedStatement.Bind(1, changeNumberRecord.NewJid);
            preparedStatement.Step();
          }
          this.ChangeNumberActionSubject.OnNext(new SqliteContactsContext.ChangeNumberAction(changeNumberRecord.OldJid, changeNumberRecord.NewJid, SqliteContactsContext.ChangeNumberAction.Type.Removed));
        }
      }
      this.PurgeCache("ChangeNumberRecords");
    }

    public ChangeNumberRecord[] GetChangeNumberRecordsForJid(string jid)
    {
      string sql = "SELECT * FROM ChangeNumberRecords WHERE OldJid = ? OR NewJid = ?";
      List<string> stringList = new List<string>()
      {
        jid,
        jid
      };
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement(sql))
      {
        int num = 0;
        foreach (string o in stringList)
          stmt.BindObject(num++, (object) o);
        return this.ParseTable<ChangeNumberRecord>(stmt, "ChangeNumberRecords").ToArray<ChangeNumberRecord>();
      }
    }

    private WhatsApp.BlockList BlockListRecord
    {
      get
      {
        if (this.blockListRecord == null)
        {
          using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM BlockList"))
            this.blockListRecord = this.ParseTable<WhatsApp.BlockList>(stmt, "BlockList").SingleOrDefault<WhatsApp.BlockList>();
        }
        return this.blockListRecord;
      }
    }

    public Dictionary<string, bool> BlockListSet
    {
      get
      {
        if (this.blockListSet == null)
        {
          Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
          WhatsApp.BlockList blockListRecord = this.BlockListRecord;
          if (blockListRecord != null && !string.IsNullOrEmpty(blockListRecord.Members))
          {
            string members = blockListRecord.Members;
            char[] chArray = new char[1]{ ' ' };
            foreach (string key in members.Split(chArray))
              dictionary.Add(key, true);
          }
          this.blockListSet = dictionary;
        }
        return this.blockListSet;
      }
    }

    public void FlushBlockList()
    {
      WhatsApp.BlockList o = this.BlockListRecord;
      if (o == null)
      {
        this.blockListRecord = o = new WhatsApp.BlockList();
        this.Insert("BlockList", (object) o);
      }
      o.LastUpdate = new DateTime?(DateTime.Now);
      o.Members = this.blockListSet == null || this.blockListSet.Count == 0 ? "" : string.Join(" ", (IEnumerable<string>) this.blockListSet.Keys);
      Settings.StatusRecipientsStateDirty = true;
      if (AppState.UIUpdatesMuted)
        return;
      this.BlockListUpdateSubject.OnNext((IEnumerable<string>) this.BlockListSet.Keys);
    }

    public DateTime? BlockListLastUpdate
    {
      get
      {
        DateTime? blockListLastUpdate = new DateTime?();
        WhatsApp.BlockList blockListRecord = this.BlockListRecord;
        if (blockListRecord != null)
          blockListLastUpdate = blockListRecord.LastUpdate;
        return blockListLastUpdate;
      }
    }

    public ChatPicture GetChatPictureState(string jid, CreateOptions createOption)
    {
      return this.GetChatPictureState(jid, createOption, out CreateResult _);
    }

    public ChatPicture GetChatPictureState(
      string jid,
      CreateOptions createOption,
      out CreateResult createResult)
    {
      createResult = CreateResult.None;
      ChatPicture chatPic = (ChatPicture) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM ChatPictures WHERE Jid = ?"))
      {
        stmt.Bind(0, jid);
        chatPic = this.ParseTable<ChatPicture>(stmt, "ChatPictures").FirstOrDefault<ChatPicture>();
      }
      if (chatPic == null && (createOption & CreateOptions.CreateIfNotFound) != CreateOptions.None)
      {
        chatPic = new ChatPicture() { Jid = jid };
        createResult = CreateResult.Created;
        if ((createOption & (CreateOptions) 2) != CreateOptions.None)
        {
          this.InsertChatPictureOnSubmit(chatPic);
          createResult = CreateResult.CreatedToDb;
          if ((createOption & (CreateOptions) 4) != CreateOptions.None)
          {
            this.SubmitChanges();
            createResult = CreateResult.CreatedAndSubmitted;
          }
        }
      }
      return chatPic;
    }

    public void InsertChatPictureOnSubmit(ChatPicture chatPic)
    {
      this.Insert("ChatPictures", (object) chatPic);
    }

    public void DeleteChatPictureOnSubmit(ChatPicture chatPic)
    {
      this.Delete("ChatPictures", (object) chatPic);
    }

    public UserStatus[] GetCallableContacts()
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM UserStatuses WHERE IsInDeviceContactList = 1 ORDER BY ContactName"))
        return this.ParseTable<UserStatus>(stmt, "UserStatuses").ToArray<UserStatus>();
    }

    public ClientCapability[] GetClientCapabilities(string jid)
    {
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM ClientCapabilities WHERE Jid = ?"))
      {
        stmt.Bind(0, jid);
        return this.ParseTable<ClientCapability>(stmt, "ClientCapabilities").ToArray<ClientCapability>();
      }
    }

    public ClientCapability GetClientCapabilities(
      string jid,
      ClientCapabilityCategory category,
      bool create = false)
    {
      ClientCapability o = (ClientCapability) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM ClientCapabilities WHERE Jid = ? AND Category = ?"))
      {
        stmt.Bind(0, jid);
        stmt.Bind(1, (long) category, false);
        o = this.ParseTable<ClientCapability>(stmt, "ClientCapabilities").FirstOrDefault<ClientCapability>();
      }
      if (o == null & create)
      {
        o = new ClientCapability()
        {
          Jid = jid,
          Category = category
        };
        this.Insert("ClientCapabilities", (object) o);
      }
      return o;
    }

    public void DeleteClientCapabilityOnSubmit(ClientCapability cap)
    {
      this.Delete("ClientCapabilities", (object) cap);
    }

    public void DeleteConversionRecordForJid(string jid)
    {
      if (string.IsNullOrEmpty(jid))
        return;
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM " + SqliteContactsContext.CONVERSION_RECORD_TABLE + " WHERE ConversionJid = ?"))
      {
        preparedStatement.Bind(0, jid);
        preparedStatement.Step();
      }
      this.PurgeCache(SqliteContactsContext.CONVERSION_RECORD_TABLE);
    }

    public void AddOrReplaceConversionRecord(
      string jid,
      DateTime lastActionTime,
      DateTime timestamp,
      string phoneNumber,
      string source,
      byte[] data)
    {
      this.DeleteConversionRecordForJid(jid);
      this.Insert(SqliteContactsContext.CONVERSION_RECORD_TABLE, (object) new ConversionRecord()
      {
        ConversionJid = jid,
        LastActionTimestamp = lastActionTime,
        Timestamp = timestamp,
        PhoneNumber = phoneNumber,
        Source = source,
        Data = data
      });
      this.SubmitChanges();
      this.PurgeCache(SqliteContactsContext.CONVERSION_RECORD_TABLE);
    }

    public ConversionRecord GetConversionRecord(string jid)
    {
      ConversionRecord conversionRecord = this.GetConversionRecordImpl(jid);
      if (conversionRecord != null && conversionRecord.LastActionTimestamp < FunRunner.CurrentServerTimeUtc.AddDays(-7.0))
        conversionRecord = (ConversionRecord) null;
      return conversionRecord;
    }

    public ConversionRecord HasUpdatedConversionRecord(string jid, DateTime lastActivity)
    {
      ConversionRecord conversionRecord = this.GetConversionRecordImpl(jid);
      if (conversionRecord != null)
      {
        if (conversionRecord.LastActionTimestamp < FunRunner.CurrentServerTimeUtc.AddDays(-7.0))
        {
          this.DeleteConversionRecordForJid(jid);
          conversionRecord = (ConversionRecord) null;
        }
        else
          this.AddOrReplaceConversionRecord(jid, lastActivity, conversionRecord.Timestamp, conversionRecord.PhoneNumber, conversionRecord.Source, conversionRecord.Data);
        this.PurgeCache(SqliteContactsContext.CONVERSION_RECORD_TABLE);
      }
      return conversionRecord;
    }

    public void ClearConversionRecordTables()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("DELETE FROM " + SqliteContactsContext.CONVERSION_RECORD_TABLE))
        preparedStatement.Step();
      this.PurgeCache(SqliteContactsContext.CONVERSION_RECORD_TABLE);
    }

    private ConversionRecord GetConversionRecordImpl(string jid)
    {
      if (string.IsNullOrEmpty(jid))
        return (ConversionRecord) null;
      ConversionRecord conversionRecordImpl = (ConversionRecord) null;
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM " + SqliteContactsContext.CONVERSION_RECORD_TABLE + " WHERE ConversionJid = ?"))
      {
        stmt.Bind(0, jid);
        conversionRecordImpl = this.ParseTable<ConversionRecord>(stmt, SqliteContactsContext.CONVERSION_RECORD_TABLE).FirstOrDefault<ConversionRecord>();
        DateTime lastActionTimestamp = conversionRecordImpl.LastActionTimestamp;
        if (conversionRecordImpl.LastActionTimestamp < conversionRecordImpl.Timestamp)
          conversionRecordImpl.LastActionTimestamp = conversionRecordImpl.Timestamp;
      }
      return conversionRecordImpl;
    }

    public List<string> GetConversionRecordJids()
    {
      List<string> conversionRecordJids = new List<string>();
      using (Sqlite.PreparedStatement stmt = this.Db.PrepareStatement("SELECT * FROM " + SqliteContactsContext.CONVERSION_RECORD_TABLE))
      {
        foreach (ConversionRecord conversionRecord in this.ParseTable<ConversionRecord>(stmt, SqliteContactsContext.CONVERSION_RECORD_TABLE))
        {
          DateTime lastActionTimestamp = conversionRecord.LastActionTimestamp;
          if (conversionRecord.LastActionTimestamp < conversionRecord.Timestamp)
            conversionRecord.LastActionTimestamp = conversionRecord.Timestamp;
          if (conversionRecord.LastActionTimestamp > FunRunner.CurrentServerTimeUtc.AddDays(-7.0))
            conversionRecordJids.Add(conversionRecord.ConversionJid);
        }
      }
      return conversionRecordJids;
    }

    public void AttemptScheduledTaskOnThreadPool(
      WaScheduledTask task,
      int delayInMS,
      bool ignorePendingCheck = false,
      object tag = null)
    {
      WaScheduledTask.AttemptOnThreadPool((SqliteDataContext) this, task, delayInMS, (Action<WaScheduledTask>) (taskToDel => this.DeleteScheduledTaskOnSubmit(taskToDel)), (Action<WaScheduledTask>) (taskToAttempt => SqliteContactsContext.AttemptScheduledTask(taskToAttempt, tag)), ignorePendingCheck);
    }

    public static void AttemptScheduledTask(WaScheduledTask task, object tag = null)
    {
      WaScheduledTask.GetAttemptObservable(task, tag).Take<Unit>(1).Subscribe<Unit>((Action<Unit>) (_ => ContactsContext.Instance((Action<ContactsContext>) (db => WaScheduledTask.OnTaskDone((SqliteDataContext) db, task, (Action<WaScheduledTask>) (taskToDel => db.DeleteScheduledTaskOnSubmit(taskToDel)))))), (Action<Exception>) (ex => ContactsContext.Instance((Action<ContactsContext>) (db => WaScheduledTask.OnAttemptError((SqliteDataContext) db, task, ex, (Action<WaScheduledTask>) (taskToDel => db.DeleteScheduledTaskOnSubmit(taskToDel)))))), (Action) (() => WaScheduledTask.OnAttemptDone(task)));
    }

    public void InsertScheduledTaskOnSubmit(WaScheduledTask task)
    {
      this.Insert("WaScheduledTasks", (object) task);
    }

    public void DeleteScheduledTaskOnSubmit(WaScheduledTask task)
    {
      if (task.IsDeleted)
        return;
      this.Delete("WaScheduledTasks", (object) task);
      task.IsDeleted = true;
    }

    public void CleanupScheduledTasks()
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

    public WaScheduledTask[] GetScheduledTasks(
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

    public void RemoveScheduledTasks(WaScheduledTask.Types taskType, string lookupKey = null)
    {
      WaScheduledTask.Types[] includeTypes = new WaScheduledTask.Types[1]
      {
        taskType
      };
      foreach (WaScheduledTask scheduledTask in this.GetScheduledTasks(includeTypes, excludeExpired: false, lookupKey: lookupKey))
      {
        Log.l("cdb", "finished | id:{0},type:{1},attempts:{2}", (object) scheduledTask.TaskID, (object) (WaScheduledTask.Types) scheduledTask.TaskType, (object) scheduledTask.Attempts);
        this.DeleteScheduledTaskOnSubmit(scheduledTask);
      }
      this.SubmitChanges();
    }

    public class ChangeNumberAction
    {
      public string OldJid { get; private set; }

      public string NewJid { get; private set; }

      public SqliteContactsContext.ChangeNumberAction.Type ActionType { get; private set; }

      public ChangeNumberAction(
        string oldJid,
        string newJid,
        SqliteContactsContext.ChangeNumberAction.Type type)
      {
        this.OldJid = oldJid;
        this.NewJid = newJid;
        this.ActionType = type;
      }

      public enum Type
      {
        Added,
        Removed,
      }
    }
  }
}
