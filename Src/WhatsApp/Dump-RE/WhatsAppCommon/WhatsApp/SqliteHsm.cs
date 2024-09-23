// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteHsm
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
  public class SqliteHsm
  {
    public static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.SqliteHsmLock");
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string rollbackTxStmt = "ROLLBACK TRANSACTION";
    private const int LatestSchemaVersion = 2;
    private static readonly string DbPath = Constants.IsoStorePath + "\\hsm.db";
    private static readonly string LanguagePacksTab = "langpacks";
    private static readonly string NamespaceCol = "ns";
    private static readonly string LanguageCol = "lg";
    private static readonly string LocaleCol = "lc";
    private static readonly string TimestampCol = "time";
    private static readonly string LastUsedTimestampCol = "ltime";
    private static readonly string LpType = "lptype";
    private const int LP_TYPE_PACK = 1;
    private const int LP_TYPE_MISSING = 2;
    private static readonly string HashCol = "hash";
    private static readonly string LangaugePackIndex = "langpackx";

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write)
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          if (!nativeMediaStorage.FileExists(SqliteHsm.DbPath))
            return;
        }
      }
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(SqliteHsm.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          switch (SqliteHsm.GetSchemaVersion(db))
          {
            case -1:
              SqliteHsm.CreateTables(db);
              goto case 2;
            case 2:
              onDb(db);
              break;
            default:
              SqliteHsm.DeleteTables(db);
              goto case -1;
          }
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
                nativeMediaStorage.DeleteFile(SqliteHsm.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            SqliteHsm.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            SqliteHsm.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            SqliteHsm.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      SqliteHsm.Lock.PerformWithLock((Action) (() => SqliteHsm.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      SqliteHsm.PerformWithDb(onDb, true);
    }

    private static void RunStatement(Sqlite db, string stmtString)
    {
      string[] strArray = new string[1]{ stmtString };
      SqliteHsm.RunStatements(db, (IEnumerable<string>) strArray);
    }

    private static void RunStatements(Sqlite db, IEnumerable<string> strings)
    {
      foreach (string sql in strings)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          preparedStatement.Step();
      }
    }

    private static void CreateTables(Sqlite db)
    {
      SqliteHsm.RunStatement(db, SqliteHsm.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
          "INSERT INTO metadata VALUES (" + (object) 2 + ")",
          "CREATE TABLE IF NOT EXISTS " + SqliteHsm.LanguagePacksTab + "\n(Id INTEGER PRIMARY KEY, " + SqliteHsm.NamespaceCol + " TEXT, " + SqliteHsm.LanguageCol + " TEXT, " + SqliteHsm.LocaleCol + " TEXT, " + SqliteHsm.TimestampCol + " INTEGER, " + SqliteHsm.LastUsedTimestampCol + " INTEGER, " + SqliteHsm.LpType + " INTEGER, " + SqliteHsm.HashCol + " TEXT )",
          "CREATE UNIQUE INDEX IF NOT EXISTS " + SqliteHsm.LangaugePackIndex + "\nON " + SqliteHsm.LanguagePacksTab + " (" + SqliteHsm.NamespaceCol + ", " + SqliteHsm.LanguageCol + ", " + SqliteHsm.LocaleCol + ")"
        };
        stringList.Add(SqliteHsm.commitTxStmt);
        SqliteHsm.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqliteHsm.RunStatement(db, SqliteHsm.rollbackTxStmt);
        throw;
      }
    }

    private static void DeleteTables(Sqlite db)
    {
      try
      {
        List<string> stringList = new List<string>()
        {
          SqliteHsm.beginTxStmt,
          "DROP TABLE metadata",
          "DROP INDEX " + SqliteHsm.LangaugePackIndex,
          "DROP TABLE " + SqliteHsm.LanguagePacksTab,
          SqliteHsm.commitTxStmt
        };
        SqliteHsm.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqliteHsm.RunStatement(db, SqliteHsm.rollbackTxStmt);
        throw;
      }
    }

    public static void DeleteDb()
    {
      SqliteHsm.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(SqliteHsm.DbPath);
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

    public static void AddLanguagePack(
      string lpNamespace,
      string lpLanguage,
      string lpLocale,
      DateTime timestamp,
      string lpHash)
    {
      Log.d("hsm", "Adding/Updating language: {0} {1} {2}", (object) lpNamespace, (object) lpLanguage, (object) lpLocale);
      SqliteHsm.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO " + SqliteHsm.LanguagePacksTab + " (" + SqliteHsm.NamespaceCol + ", " + SqliteHsm.LanguageCol + ", " + SqliteHsm.LocaleCol + ", " + SqliteHsm.TimestampCol + ", " + SqliteHsm.LastUsedTimestampCol + ", " + SqliteHsm.LpType + ", " + SqliteHsm.HashCol + ") VALUES(?, ?, ?, ?, ?, ?, ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[7]
          {
            (object) lpNamespace,
            (object) lpLanguage,
            lpLocale == null ? (object) "" : (object) lpLocale,
            (object) timestamp.ToUnixTime(),
            (object) DateTime.UtcNow.ToUnixTime(),
            (object) 1,
            (object) lpHash
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void AddLanguagePackNotOnServer(
      string lpNamespace,
      string lpLanguage,
      string lpLocale,
      DateTime timestamp)
    {
      Log.d("hsm", "Adding/Updating missing: {0} {1} {2}", (object) lpNamespace, (object) lpLanguage, (object) lpLocale);
      SqliteHsm.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO " + SqliteHsm.LanguagePacksTab + " (" + SqliteHsm.NamespaceCol + ", " + SqliteHsm.LanguageCol + ", " + SqliteHsm.LocaleCol + ", " + SqliteHsm.TimestampCol + ", " + SqliteHsm.LastUsedTimestampCol + ", " + SqliteHsm.LpType + ") VALUES(?, ?, ?, ?, ?, ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[6]
          {
            (object) lpNamespace,
            (object) lpLanguage,
            lpLocale == null ? (object) "" : (object) lpLocale,
            (object) timestamp.ToUnixTime(),
            (object) DateTime.UtcNow.ToUnixTime(),
            (object) 2
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void SetLanguagePackUsed(
      string lpNamespace,
      string lpLanguage,
      string lpLocale,
      DateTime timestamp)
    {
      Log.d("hsm", "Updating timestamp for {0} {1} {2}", (object) lpNamespace, (object) lpLanguage, (object) lpLocale);
      SqliteHsm.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "UPDATE " + SqliteHsm.LanguagePacksTab + " SET " + SqliteHsm.LastUsedTimestampCol + "= ? Where " + SqliteHsm.NamespaceCol + "= ? AND " + SqliteHsm.LanguageCol + "= ? AND " + SqliteHsm.LocaleCol + "= ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[4]
          {
            (object) timestamp.ToUnixTime(),
            (object) lpNamespace,
            (object) lpLanguage,
            lpLocale == null ? (object) "" : (object) lpLocale
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static HsmLangPack GetLangPack(string lpNamespace, string lpLang, string lpLoc)
    {
      Log.d("hsm", "looking for language {0} {1} {2}", (object) lpNamespace, (object) lpLang, (object) lpLoc);
      HsmLangPack returnPack = (HsmLangPack) null;
      SqliteHsm.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "SELECT " + SqliteHsm.TimestampCol + ", " + SqliteHsm.LastUsedTimestampCol + ", " + SqliteHsm.LpType + ", " + SqliteHsm.HashCol + " FROM " + SqliteHsm.LanguagePacksTab + " WHERE " + SqliteHsm.NamespaceCol + " = ? AND " + SqliteHsm.LanguageCol + " = ? AND " + SqliteHsm.LocaleCol + " = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          preparedStatement.Bind(0, lpNamespace);
          preparedStatement.Bind(1, lpLang);
          preparedStatement.Bind(2, lpLoc == null ? "" : lpLoc);
          if (!preparedStatement.Step())
            return;
          returnPack = new HsmLangPack(lpNamespace, lpLang, lpLoc);
          returnPack.RequestedTimestamp = preparedStatement.Columns[0] == null ? FunXMPP.UnixEpoch : FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[0]);
          returnPack.LastUsedTimestamp = preparedStatement.Columns[1] == null ? FunXMPP.UnixEpoch : FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[1]);
          switch ((int) (long) preparedStatement.Columns[2])
          {
            case 1:
              returnPack.LpType = HsmLangPackTypeCode.Pack;
              returnPack.LpHash = (string) preparedStatement.Columns[3];
              break;
            case 2:
              returnPack.LpType = HsmLangPackTypeCode.NotPresent;
              break;
          }
        }
      }));
      return returnPack;
    }

    public static List<HsmLangPack> GetLangPacks(string lpNamespace = "", string lpLang = "", string lpLoc = "")
    {
      Log.d("hsm", "looking for language packs '{0}' '{1}' '{2}'", (object) lpNamespace, (object) lpLang, (object) lpLoc);
      List<HsmLangPack> returnPacks = new List<HsmLangPack>();
      SqliteHsm.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "SELECT " + SqliteHsm.TimestampCol + ", " + SqliteHsm.LastUsedTimestampCol + ", " + SqliteHsm.LpType + ", " + SqliteHsm.HashCol + ", " + SqliteHsm.NamespaceCol + ", " + SqliteHsm.LanguageCol + ", " + SqliteHsm.LocaleCol + " FROM " + SqliteHsm.LanguagePacksTab;
        string str = "";
        List<string> source = new List<string>();
        if (!string.IsNullOrEmpty(lpNamespace))
        {
          str = str + SqliteHsm.NamespaceCol + " = ? ";
          source.Add(lpNamespace);
        }
        if (!string.IsNullOrEmpty(lpLang))
        {
          if (source.Count<string>() > 0)
            str += "AND ";
          str = str + SqliteHsm.LanguageCol + " = ? ";
          source.Add(lpLang);
        }
        if (!string.IsNullOrEmpty(lpLoc))
        {
          if (source.Count<string>() > 0)
            str += "AND ";
          str = str + SqliteHsm.LocaleCol + " = ? ";
          source.Add(lpLoc);
        }
        if (source.Count<string>() > 0)
          sql = sql + " WHERE " + str;
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          for (int index = 0; index < source.Count<string>(); ++index)
            preparedStatement.Bind(index, source[index]);
          while (preparedStatement.Step())
          {
            HsmLangPack hsmLangPack = new HsmLangPack((string) preparedStatement.Columns[4], (string) preparedStatement.Columns[5], (string) preparedStatement.Columns[6]);
            hsmLangPack.RequestedTimestamp = preparedStatement.Columns[0] == null ? FunXMPP.UnixEpoch : FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[0]);
            hsmLangPack.LastUsedTimestamp = preparedStatement.Columns[1] == null ? FunXMPP.UnixEpoch : FunXMPP.UnixEpoch.AddSeconds((double) (long) preparedStatement.Columns[1]);
            switch ((int) (long) preparedStatement.Columns[2])
            {
              case 1:
                hsmLangPack.LpType = HsmLangPackTypeCode.Pack;
                hsmLangPack.LpHash = (string) preparedStatement.Columns[3];
                break;
              case 2:
                hsmLangPack.LpType = HsmLangPackTypeCode.NotPresent;
                break;
            }
            returnPacks.Add(hsmLangPack);
          }
        }
      }));
      return returnPacks;
    }
  }
}
