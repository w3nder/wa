// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteEmojiSearch
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class SqliteEmojiSearch
  {
    public static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.SqliteEmojiSearchLock");
    private static string LogHeader = "emojidictdb";
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string rollbackTxStmt = "ROLLBACK TRANSACTION";
    private const int LatestSchemaVersion = 2;
    private static readonly string DbPath = Constants.IsoStorePath + "\\emojidict.db";
    private const string EmojiDictionaryTab = "emojidict";
    private const string EDLang = "l";
    private const string EDEmoji = "e";
    private const string EDText = "t";
    private const string EmojiLanguageStatusTab = "emojistate";
    private const string ELLang = "lang";
    private const string ELTimestamp = "t";
    private const string ELFetchState = "fs";
    private const string ELFetchTimestamp = "ft";
    private const string ELRequestETag = "etag";
    private const string ELTopEmoji = "top";

    public static bool HasEmojiSearchDb()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(SqliteEmojiSearch.DbPath);
    }

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write && !SqliteEmojiSearch.HasEmojiSearchDb())
        return;
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(SqliteEmojiSearch.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          int currentSchema = SqliteEmojiSearch.GetSchemaVersion(db);
          switch (currentSchema)
          {
            case -1:
              if (currentSchema != -1)
                SqliteEmojiSearch.DeleteTables(db, currentSchema);
              currentSchema = SqliteEmojiSearch.CreateTables(db);
              break;
            case 2:
label_13:
              onDb(db);
              return;
            default:
              if (currentSchema <= 2)
                break;
              goto case -1;
          }
          SqliteEmojiSearch.SchemaUpdate(db, currentSchema);
          goto label_13;
        }
      }
      catch (Exception ex1)
      {
        if (!retry)
        {
          throw;
        }
        else
        {
          uint hresult = ex1.GetHResult();
          if ((int) hresult == (int) Sqlite.HRForError(11U))
          {
            using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            {
              try
              {
                nativeMediaStorage.DeleteFile(SqliteEmojiSearch.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            SqliteEmojiSearch.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            SqliteEmojiSearch.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            SqliteEmojiSearch.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      SqliteEmojiSearch.Lock.PerformWithLock((Action) (() => SqliteEmojiSearch.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      SqliteEmojiSearch.PerformWithDb(onDb, true);
    }

    private static void RunStatement(Sqlite db, string stmtString)
    {
      string[] strArray = new string[1]{ stmtString };
      SqliteEmojiSearch.RunStatements(db, (IEnumerable<string>) strArray);
    }

    private static void RunStatements(Sqlite db, IEnumerable<string> strings)
    {
      foreach (string sql in strings)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          preparedStatement.Step();
      }
    }

    private static int CreateTables(Sqlite db)
    {
      int tables = 1;
      SqliteEmojiSearch.RunStatement(db, SqliteEmojiSearch.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
          "INSERT INTO metadata VALUES (" + (object) tables + ")",
          "CREATE TABLE IF NOT EXISTS emojidict\n(Id INTEGER PRIMARY KEY, l TEXT NOT NULL, e TEXT NOT NULL, t TEXT NOT NULL )",
          "CREATE TABLE IF NOT EXISTS emojistate\n(lang TEXT PRIMARY KEY, t INTEGER, fs INTEGER NON NULL, ft INTEGER, etag TEXT )"
        };
        stringList.Add(SqliteEmojiSearch.commitTxStmt);
        SqliteEmojiSearch.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
        return tables;
      }
      catch (Exception ex)
      {
        SqliteEmojiSearch.RunStatement(db, SqliteEmojiSearch.rollbackTxStmt);
        throw;
      }
    }

    private static void SchemaUpdate(Sqlite db, int currentSchema)
    {
      if (currentSchema >= 2)
        return;
      int num = 2;
      Log.l(SqliteEmojiSearch.LogHeader, "running schema update {0}", (object) num);
      Dictionary<string, string> dictionary = db.GetColumnNames("emojistate").ToDictionary<string, string>((Func<string, string>) (s => s));
      SqliteEmojiSearch.RunStatement(db, SqliteEmojiSearch.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "DELETE FROM metadata",
          "INSERT OR REPLACE INTO metadata VALUES (" + (object) num + ")"
        };
        if (!dictionary.ContainsKey("top"))
          stringList.Add("ALTER TABLE emojistate ADD COLUMN top TEXT");
        stringList.Add(SqliteEmojiSearch.commitTxStmt);
        SqliteEmojiSearch.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqliteEmojiSearch.RunStatement(db, SqliteEmojiSearch.rollbackTxStmt);
        throw;
      }
    }

    private static void DeleteTables(Sqlite db, int currentSchema)
    {
      if (currentSchema < 1)
        return;
      try
      {
        List<string> stringList = new List<string>()
        {
          SqliteEmojiSearch.beginTxStmt,
          "DROP TABLE IF EXISTS metadata",
          "DROP TABLE IF EXISTS emojidict",
          "DROP TABLE IF EXISTS emojistate",
          SqliteEmojiSearch.commitTxStmt
        };
        SqliteEmojiSearch.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqliteEmojiSearch.RunStatement(db, SqliteEmojiSearch.rollbackTxStmt);
        throw;
      }
    }

    public static void DeleteDb()
    {
      SqliteEmojiSearch.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(SqliteEmojiSearch.DbPath);
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }));
    }

    protected static int GetSchemaVersion(Sqlite db)
    {
      long num = 0;
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT COUNT(tbl_name) FROM sqlite_master WHERE TYPE = 'table' AND TBL_NAME = 'metadata'"))
      {
        preparedStatement.Step();
        num = (long) preparedStatement.Columns[0];
      }
      if (num != 1L)
        return -1;
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT version FROM metadata"))
      {
        preparedStatement.Step();
        return (int) (long) preparedStatement.Columns[0];
      }
    }

    private static void StoreStatusInfo(SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo)
    {
      Log.d(SqliteEmojiSearch.LogHeader, "Adding/Updating state: {0} {1} ", (object) statusInfo.EmojiLanguage, (object) statusInfo.LanguageState);
      SqliteEmojiSearch.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO emojistate (lang, t, fs, ft, etag, top) VALUES(?, ?, ?, ?, ?, ? )";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[6]
          {
            (object) statusInfo.EmojiLanguage,
            (object) statusInfo.LastUsedDate.ToUnixTime(),
            (object) (int) statusInfo.FetchState,
            (object) (statusInfo.LastFetchedTimestamp.HasValue ? statusInfo.LastFetchedTimestamp.Value.ToUnixTime() : 0L),
            (object) statusInfo.LastRequestETag,
            (object) (statusInfo.TopEmojiString ?? "")
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static List<SqliteEmojiSearch.EmojiSearchStatusInfo> GetEmojiSearchStatusInfo()
    {
      Log.d(SqliteEmojiSearch.LogHeader, "Getting Emoji status info");
      try
      {
        List<SqliteEmojiSearch.EmojiSearchStatusInfo> returnList = new List<SqliteEmojiSearch.EmojiSearchStatusInfo>();
        SqliteEmojiSearch.PerformWithDbWrite((Action<Sqlite>) (db =>
        {
          string sql = "SELECT lang, t, fs, ft, etag, top FROM emojistate";
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            while (preparedStatement.Step())
            {
              SqliteEmojiSearch.EmojiSearchStatusInfo searchStatusInfo = new SqliteEmojiSearch.EmojiSearchStatusInfo();
              searchStatusInfo.EmojiLanguage = (string) preparedStatement.Columns[0];
              searchStatusInfo.LastUsedDate = FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[1]);
              if (preparedStatement.Columns[2] != null)
              {
                switch ((long) preparedStatement.Columns[2] - 1L)
                {
                  case 0:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.NETWORK_ERROR_RETRY;
                    break;
                  case 1:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.NETWORK_ERROR_NO_RETRY;
                    break;
                  case 2:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.NOT_FOUND;
                    break;
                  case 3:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK;
                    break;
                  case 4:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.FETCHING;
                    break;
                  default:
                    searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.Unknown;
                    break;
                }
              }
              else
                searchStatusInfo.FetchState = SqliteEmojiSearch.EmojiLanguageFetchState.Unknown;
              if (preparedStatement.Columns[3] != null)
                searchStatusInfo.LastFetchedTimestamp = new DateTime?(FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[3]));
              searchStatusInfo.LastRequestETag = (string) preparedStatement.Columns[4];
              searchStatusInfo.TopEmojiString = (string) preparedStatement.Columns[5];
              returnList.Add(searchStatusInfo);
            }
          }
        }));
        return returnList;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception returning stored state");
        return (List<SqliteEmojiSearch.EmojiSearchStatusInfo>) null;
      }
    }

    public static void AddLanguageToDatabase(
      SqliteEmojiSearch.EmojiSearchStatusInfo statusInfo,
      Dictionary<string, string[]> emojiDetails)
    {
      Log.d(SqliteEmojiSearch.LogHeader, "Adding Language: {0}", (object) statusInfo.EmojiLanguage);
      SqliteEmojiSearch.RemoveDetailsForLanguage(statusInfo.EmojiLanguage);
      if (emojiDetails != null)
        SqliteEmojiSearch.StoreEmojiDetails(statusInfo.EmojiLanguage, emojiDetails);
      SqliteEmojiSearch.StoreStatusInfo(statusInfo);
    }

    public static void RemoveDetailsForLanguage(string language)
    {
      Log.d(SqliteEmojiSearch.LogHeader, "Removing Language: {0}", (object) language);
      if (language == null)
        return;
      SqliteEmojiSearch.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql1 = "DELETE FROM emojistate WHERE lang = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql1))
        {
          object[] objArray = new object[1]
          {
            (object) language
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
        string sql2 = "DELETE FROM emojidict WHERE l = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql2))
        {
          object[] objArray = new object[1]
          {
            (object) language
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void StoreEmojiDetails(string language, Dictionary<string, string[]> emojiDetails)
    {
      Log.d(SqliteEmojiSearch.LogHeader, "Adding Emoji for: {0} {1}", (object) language, (object) emojiDetails.Count);
      SqliteEmojiSearch.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT INTO emojidict (l, e, t) VALUES(?, ?, ? )";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          foreach (string key in emojiDetails.Keys)
          {
            foreach (string o in emojiDetails.GetValueOrDefault<string, string[]>(key))
            {
              preparedStatement.BindObject(0, (object) language);
              preparedStatement.BindObject(1, (object) key);
              preparedStatement.BindObject(2, (object) o);
              preparedStatement.Step();
              preparedStatement.Reset();
            }
          }
        }
      }));
    }

    public static List<string> GetMatchingFromTable(string searchTerm, int limit, bool exactMatch)
    {
      List<string> emojis = new List<string>();
      SqliteEmojiSearch.PerformWithDb((Action<Sqlite>) (db =>
      {
        string sql = "SELECT e FROM emojidict WHERE t" + (exactMatch ? " = " : " LIKE ") + "? ORDER BY Id LIMIT ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          preparedStatement.BindObject(0, (object) (searchTerm + (exactMatch ? "" : "%")));
          preparedStatement.BindObject(1, (object) limit);
          while (preparedStatement.Step())
          {
            string column = (string) preparedStatement.Columns[0];
            if (!emojis.Contains(column))
              emojis.Add(column);
          }
        }
      }));
      return emojis.Where<string>((Func<string, bool>) (e => Emoji.Mappings(e) != null)).ToList<string>();
    }

    public enum EmojiLanguageState
    {
      Unknown,
      NOT_FETCHED,
      FETCH_ERROR,
      CACHED_STALE,
      CACHED_LANGUAGE_MISMATCH,
      UPTO_DATE,
    }

    public enum EmojiLanguageFetchState
    {
      Unknown,
      NETWORK_ERROR_RETRY,
      NETWORK_ERROR_NO_RETRY,
      NOT_FOUND,
      LAST_FETCH_OK,
      FETCHING,
    }

    public class EmojiSearchStatusInfo
    {
      public string EmojiLanguage;
      public SqliteEmojiSearch.EmojiLanguageFetchState FetchState;
      public DateTime LastUsedDate;
      public DateTime? LastFetchedTimestamp;
      public string LastRequestETag;
      public string TopEmojiString;

      public void Store() => SqliteEmojiSearch.StoreStatusInfo(this);

      public SqliteEmojiSearch.EmojiLanguageState LanguageState
      {
        get
        {
          if (this.FetchState != SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK)
          {
            if (this.FetchState == SqliteEmojiSearch.EmojiLanguageFetchState.NOT_FOUND)
              return SqliteEmojiSearch.EmojiLanguageState.CACHED_LANGUAGE_MISMATCH;
            return this.FetchState == SqliteEmojiSearch.EmojiLanguageFetchState.Unknown ? SqliteEmojiSearch.EmojiLanguageState.NOT_FETCHED : SqliteEmojiSearch.EmojiLanguageState.FETCH_ERROR;
          }
          if (!this.LastFetchedTimestamp.HasValue)
            return SqliteEmojiSearch.EmojiLanguageState.NOT_FETCHED;
          return this.LastFetchedTimestamp.Value.AddDays((double) EmojiSearch.STALENESS_THRESHOLD_DAYS) < DateTime.UtcNow ? SqliteEmojiSearch.EmojiLanguageState.CACHED_STALE : SqliteEmojiSearch.EmojiLanguageState.UPTO_DATE;
        }
      }

      public bool ShouldRequestUpdate()
      {
        bool flag = this.FetchState == SqliteEmojiSearch.EmojiLanguageFetchState.LAST_FETCH_OK || this.FetchState == SqliteEmojiSearch.EmojiLanguageFetchState.NOT_FOUND ? !this.LastFetchedTimestamp.HasValue || this.LastFetchedTimestamp.Value.Ticks + EmojiSearch.STALENESS_THRESHOLD_TICKS < DateTime.UtcNow.Ticks : !this.LastFetchedTimestamp.HasValue || this.LastFetchedTimestamp.Value.Ticks + EmojiSearch.TIME_BETWEEN_FETCHES_TICKS < DateTime.UtcNow.Ticks;
        if (string.IsNullOrEmpty(this.TopEmojiString))
          flag = true;
        if (flag)
          Log.l(SqliteEmojiSearch.LogHeader, "ShouldRequestUpdate true {0}, {1} ", (object) this.EmojiLanguage, this.LastFetchedTimestamp.HasValue ? (object) this.LastFetchedTimestamp.ToString() : (object) "null");
        else
          Log.d(SqliteEmojiSearch.LogHeader, "ShouldRequestUpdate false {0}, {1} ", (object) this.EmojiLanguage, this.LastFetchedTimestamp.HasValue ? (object) this.LastFetchedTimestamp.ToString() : (object) "null");
        return flag;
      }
    }
  }
}
