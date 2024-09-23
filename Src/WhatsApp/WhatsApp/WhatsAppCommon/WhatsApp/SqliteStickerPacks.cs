// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteStickerPacks
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using WhatsAppNative;


namespace WhatsApp
{
  public class SqliteStickerPacks
  {
    private static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.SqliteStickerPacksLock");
    private static string LogHeader = "stickerpacksdb";
    private const int LatestSchemaVersion = 1;
    private static readonly string DbPath = Constants.IsoStorePath + "\\stickerpacks.db";
    private const string StickerPacksTable = "Packs";
    private const string PackId = "packid";
    private const string Name = "name";
    private const string Publisher = "publisher";
    private const string Description = "description";
    private const string FileSize = "size";
    private const string TrayImageId = "trayid";
    private const string MainPreviewId = "mainpreviewid";
    private const string DownloadDateTime = "downloadtime";
    private const string PreviewTable = "Previews";
    private const string PreviewId = "previewid";
    private const string ImageData = "imagedata";
    private const string RequestTable = "Request";
    private const string LastRequest = "lastrequest";
    private const string LastRequestTime = "lastrequesttime";
    private const string RequestState = "requeststate";
    private const string RequestETag = "etag";
    private static Dictionary<string, SqliteStickerPacks.ColumnSetter> ColumnDict = SqliteStickerPacks.BuildColumnDict();

    public static bool HasStickerPacksDb()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(SqliteStickerPacks.DbPath);
    }

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write && !SqliteStickerPacks.HasStickerPacksDb())
        return;
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(SqliteStickerPacks.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          int currentSchema = SqliteStickerPacks.GetSchemaVersion(db);
          switch (currentSchema)
          {
            case -1:
              if (currentSchema != -1)
                SqliteStickerPacks.DeleteTables(db, currentSchema);
              currentSchema = SqliteStickerPacks.CreateTables(db);
              break;
            case 1:
label_13:
              onDb(db);
              return;
            default:
              if (currentSchema <= 1)
                break;
              goto case -1;
          }
          SqliteStickerPacks.SchemaUpdate(db, currentSchema);
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
                nativeMediaStorage.DeleteFile(SqliteStickerPacks.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            SqliteStickerPacks.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            SqliteStickerPacks.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            SqliteStickerPacks.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      SqliteStickerPacks.Lock.PerformWithLock((Action) (() => SqliteStickerPacks.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      SqliteStickerPacks.PerformWithDb(onDb, true);
    }

    private static void RunStatement(Sqlite db, string stmtString)
    {
      string[] strArray = new string[1]{ stmtString };
      SqliteStickerPacks.RunStatements(db, (IEnumerable<string>) strArray);
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
      db.BeginTransaction();
      try
      {
        List<string> stringList = new List<string>()
        {
          "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
          "INSERT INTO metadata VALUES (" + (object) tables + ")",
          "CREATE TABLE IF NOT EXISTS Packs\n(Id INTEGER PRIMARY KEY, packid TEXT UNIQUE, name TEXT NOT NULL, publisher TEXT NOT NULL, description TEXT NOT NULL, size INTEGER, trayid TEXT NOT NULL, mainpreviewid TEXT NOT NULL, downloadtime INTEGER )\n",
          "CREATE UNIQUE INDEX Packs_idx0 ON Packs(packid)",
          "CREATE TABLE IF NOT EXISTS Previews\n(Id INTEGER PRIMARY KEY, previewid TEXT UNIQUE, packid TEXT NOT NULL, imagedata BLOB )\n",
          "CREATE INDEX Previews_idx0 ON Previews(packid)",
          "CREATE TABLE IF NOT EXISTS Request\n(Id INTEGER PRIMARY KEY, lastrequest TEXT NOT NULL, lastrequesttime INTEGER, etag TEXT, requeststate INTEGER )\n"
        };
        SqliteStickerPacks.RunStatements(db, (IEnumerable<string>) stringList);
        db.CommitTransaction();
        stringList.Clear();
        return tables;
      }
      catch (Exception ex)
      {
        db.RollbackTransaction();
        throw;
      }
    }

    private static void SchemaUpdate(Sqlite db, int currentSchema)
    {
    }

    private static void DeleteTables(Sqlite db, int currentSchema)
    {
      if (currentSchema < 1)
        return;
      try
      {
        db.BeginTransaction();
        List<string> stringList = new List<string>()
        {
          "DROP TABLE IF EXISTS metadata",
          "DROP TABLE IF EXISTS Packs",
          "DROP TABLE IF EXISTS Previews",
          "DROP TABLE IF EXISTS Request"
        };
        SqliteStickerPacks.RunStatements(db, (IEnumerable<string>) stringList);
        db.CommitTransaction();
        stringList.Clear();
      }
      catch (Exception ex)
      {
        db.RollbackTransaction();
        throw;
      }
    }

    public static void DeleteDb()
    {
      SqliteStickerPacks.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(SqliteStickerPacks.DbPath);
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }));
    }

    private static int GetSchemaVersion(Sqlite db)
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

    public static byte[] GetTrayImageForPack(StickerPack pack)
    {
      byte[] result = new byte[0];
      string trayImageId = pack.TrayImageId;
      SqliteStickerPacks.PerformWithDb((Action<Sqlite>) (db =>
      {
        string sql = "SELECT imagedata FROM Previews WHERE previewid = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          preparedStatement.BindObject(0, (object) trayImageId);
          preparedStatement.Step();
          result = (byte[]) preparedStatement.Columns[0];
        }
      }));
      return result;
    }

    public static List<string> GetSavedStickerPackIds()
    {
      List<string> stickerPackIds = new List<string>();
      SqliteStickerPacks.PerformWithDb((Action<Sqlite>) (db =>
      {
        string sql = "SELECT *  FROM Packs WHERE downloadtime > 0";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          while (preparedStatement.Step())
          {
            string column = (string) preparedStatement.Columns[0];
            if (!stickerPackIds.Contains(column))
              stickerPackIds.Add(column);
          }
        }
      }));
      return stickerPackIds;
    }

    private static Dictionary<string, SqliteStickerPacks.ColumnSetter> BuildColumnDict()
    {
      Dictionary<string, SqliteStickerPacks.ColumnSetter> dictionary = new Dictionary<string, SqliteStickerPacks.ColumnSetter>();
      Func<object, DateTime?> parseDt = (Func<object, DateTime?>) (o => o != null ? new DateTime?(FunXMPP.UnixEpoch.AddSeconds((double) (long) o)) : new DateTime?());
      Func<object, uint> parseUint = (Func<object, uint>) (o => o != null ? (uint) (long) o : 0U);
      dictionary["packid"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.PackId = (string) value);
      dictionary["name"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.Name = (string) value);
      dictionary["publisher"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.Publisher = (string) value);
      dictionary["description"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.Description = (string) value);
      dictionary["size"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.FileSize = parseUint(value));
      dictionary["trayid"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.TrayImageId = (string) value);
      dictionary["mainpreviewid"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.MainPreviewId = (string) value);
      dictionary["downloadtime"] = (SqliteStickerPacks.ColumnSetter) ((rec, value) => rec.DownloadDateTime = parseDt(value));
      return dictionary;
    }

    public static List<StickerPack> GetSavedStickerPacks()
    {
      List<StickerPack> stickerPacks = new List<StickerPack>();
      SqliteStickerPacks.ColumnSetter[] setters = (SqliteStickerPacks.ColumnSetter[]) null;
      SqliteStickerPacks.PerformWithDb((Action<Sqlite>) (db =>
      {
        string sql = "SELECT *  FROM Packs WHERE downloadtime > 0";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          while (preparedStatement.Step())
          {
            StickerPack pack = new StickerPack();
            if (setters == null)
            {
              setters = new SqliteStickerPacks.ColumnSetter[preparedStatement.ColumnNames.Length];
              int num = 0;
              foreach (string columnName in preparedStatement.ColumnNames)
                SqliteStickerPacks.ColumnDict.TryGetValue(columnName, out setters[num++]);
            }
            int index = 0;
            object[] columns = preparedStatement.Columns;
            foreach (SqliteStickerPacks.ColumnSetter columnSetter in setters)
            {
              if (columnSetter != null)
                columnSetter(pack, columns[index]);
              ++index;
            }
            stickerPacks.Add(pack);
          }
        }
      }));
      return stickerPacks;
    }

    public static List<StickerPack> GetAvailableStickerPacks()
    {
      List<StickerPack> stickerPacks = new List<StickerPack>();
      SqliteStickerPacks.ColumnSetter[] setters = (SqliteStickerPacks.ColumnSetter[]) null;
      SqliteStickerPacks.PerformWithDb((Action<Sqlite>) (db =>
      {
        string sql = "SELECT * FROM Packs";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          while (preparedStatement.Step())
          {
            StickerPack pack = new StickerPack();
            if (setters == null)
            {
              setters = new SqliteStickerPacks.ColumnSetter[preparedStatement.ColumnNames.Length];
              int num = 0;
              foreach (string columnName in preparedStatement.ColumnNames)
                SqliteStickerPacks.ColumnDict.TryGetValue(columnName, out setters[num++]);
            }
            int index = 0;
            object[] columns = preparedStatement.Columns;
            foreach (SqliteStickerPacks.ColumnSetter columnSetter in setters)
            {
              if (columnSetter != null)
                columnSetter(pack, columns[index]);
              ++index;
            }
            stickerPacks.Add(pack);
          }
        }
      }));
      return stickerPacks;
    }

    public static void InsertOrUpdate(StickerPack stickerPack)
    {
      bool written = false;
      string[] fields = new string[8]
      {
        "packid",
        "name",
        "publisher",
        "description",
        "size",
        "trayid",
        "mainpreviewid",
        "downloadtime"
      };
      SqliteStickerPacks.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql1 = "SELECT downloadtime FROM Packs WHERE packid = ?";
        long? nullable = new long?();
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql1))
        {
          preparedStatement.Bind(0, stickerPack.PackId);
          if (preparedStatement.Step())
            nullable = (long?) preparedStatement.Columns[0];
        }
        string sql2 = "INSERT OR REPLACE INTO Packs (" + string.Join(", ", fields) + ") VALUES (?, ?, ?, ?, ?, ?, ?, ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql2))
        {
          object[] objArray = new object[8]
          {
            (object) stickerPack.PackId,
            (object) stickerPack.Name,
            (object) stickerPack.Publisher,
            (object) stickerPack.Description,
            (object) (long) stickerPack.FileSize,
            (object) stickerPack.TrayImageId,
            (object) stickerPack.MainPreviewId,
            (object) nullable
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
          written = true;
        }
      }));
      int num1 = written ? 1 : 0;
    }

    public static void SaveStickerPreviewImage(StickerPack stickerPack, byte[] imageData)
    {
      SqliteStickerPacks.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO Previews (" + string.Join(", ", new string[3]
        {
          "previewid",
          "packid",
          "imagedata"
        }) + ") VALUES (?, ?, ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[3]
          {
            (object) stickerPack.TrayImageId,
            (object) stickerPack.PackId,
            (object) imageData
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    private delegate void ColumnSetter(StickerPack pack, object value);
  }
}
