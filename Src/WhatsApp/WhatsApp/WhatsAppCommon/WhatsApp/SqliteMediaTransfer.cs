// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteMediaTransfer
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhatsAppNative;


namespace WhatsApp
{
  public class SqliteMediaTransfer
  {
    public static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.SqliteMediaTransferLock");
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string rollbackTxStmt = "ROLLBACK TRANSACTION";
    private const int LatestSchemaVersion = 1;
    private static readonly string DbPath = Constants.IsoStorePath + "\\mediatransfers.db";
    private const string DownloadsTab = "downloads";
    private const string DownloadMediaType = "type";
    private const string DnId = "downloadid";
    private const string DnCreateTimestamp = "createts";
    private const string DnPlainHash = "plainhash";
    private const string DnDownloadUrl = "downloadurl";
    private const string DnLocalUrl = "localurl";
    private const string DnUpdateTimestamp = "updatets";
    private const string DnStatus = "status";
    private const int dnStatusQueued = 1;
    private const int dnStatusRunning = 2;
    private const int dnStatusPaused = 3;
    private const int dnStatusFinished = 4;
    private const string DnMethod = "method";
    private const int dnMethodInApp = 1;
    private const int dnMethodBT = 2;
    private const string DnResponseCode = "respcode";
    private const string DnDecryptionKey = "decryptionkey";
    private const string DnSize = "size";
    private const string DnMaxSize = "maxsize";
    private static readonly string DownloadsIdIndex = "dwnloadsidindex";
    private static readonly string DownloadUrlIndex = "downloadsurlindex";
    private static readonly string LogHdr = "XferDb";

    public static bool HasDatabase()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(SqliteMediaTransfer.DbPath);
    }

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write && !SqliteMediaTransfer.HasDatabase())
        return;
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(SqliteMediaTransfer.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          int currentSchema = SqliteMediaTransfer.GetSchemaVersion(db);
          switch (currentSchema)
          {
            case -1:
              if (currentSchema != -1)
                SqliteMediaTransfer.DeleteTables(db, currentSchema);
              currentSchema = SqliteMediaTransfer.CreateTables(db);
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
          SqliteMediaTransfer.SchemaUpdate(db, currentSchema);
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
                nativeMediaStorage.DeleteFile(SqliteMediaTransfer.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            SqliteMediaTransfer.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            SqliteMediaTransfer.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            SqliteMediaTransfer.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      SqliteMediaTransfer.Lock.PerformWithLock((Action) (() => SqliteMediaTransfer.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      SqliteMediaTransfer.PerformWithDb(onDb, true);
    }

    private static void RunStatement(Sqlite db, string stmtString)
    {
      string[] strArray = new string[1]{ stmtString };
      SqliteMediaTransfer.RunStatements(db, (IEnumerable<string>) strArray);
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
      SqliteMediaTransfer.RunStatement(db, SqliteMediaTransfer.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
          "INSERT INTO metadata VALUES (" + (object) tables + ")",
          "CREATE TABLE IF NOT EXISTS downloads\n(Id INTEGER PRIMARY KEY AUTOINCREMENT, type INTEGER NOT NULL, downloadid TEXT NOT NULL, plainhash BLOB, downloadurl TEXT NOT NULL, createts INTEGER NOT NULL, updatets INTEGER, status INTEGER NOT NULL, downloadurl INTEGER NOT NULL, method INTEGER, respcode TEXT, decryptionkey BLOB, sizeINTEGER NOT NULL maxsizeINTEGER NOT NULL )",
          "CREATE UNIQUE INDEX " + SqliteMediaTransfer.DownloadsIdIndex + " ON downloads (downloadid)",
          "CREATE UNIQUE INDEX " + SqliteMediaTransfer.DownloadUrlIndex + " ON downloads (downloadurl)"
        };
        stringList.Add(SqliteMediaTransfer.commitTxStmt);
        SqliteMediaTransfer.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
        return tables;
      }
      catch (Exception ex)
      {
        SqliteMediaTransfer.RunStatement(db, SqliteMediaTransfer.rollbackTxStmt);
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
        List<string> stringList = new List<string>()
        {
          SqliteMediaTransfer.beginTxStmt,
          "DROP TABLE IF EXISTS metadata",
          "DROP INDEX IF EXISTS " + SqliteMediaTransfer.DownloadsIdIndex,
          "DROP INDEX IF EXISTS " + SqliteMediaTransfer.DownloadUrlIndex,
          "DROP TABLE IF EXISTS downloads",
          SqliteMediaTransfer.commitTxStmt
        };
        SqliteMediaTransfer.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqliteMediaTransfer.RunStatement(db, SqliteMediaTransfer.rollbackTxStmt);
        throw;
      }
    }

    public static void DeleteDb()
    {
      SqliteMediaTransfer.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(SqliteMediaTransfer.DbPath);
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

    public static void StoreDownload(DownloadMedia downloadMedia)
    {
      Log.d(SqliteMediaTransfer.LogHdr, "Adding/Updating download: {0} {1} {2}", (object) downloadMedia.DownloadId, (object) downloadMedia.LocalUrl, (object) downloadMedia.DownloadStatus);
      SqliteMediaTransfer.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO downloads (type, downloadid, plainhash, downloadurl, localurl, createts, updatets, status, respcode, decryptionkey, size, maxsize ) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? )";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[12]
          {
            (object) (int) downloadMedia.DownloadType,
            (object) downloadMedia.DownloadId,
            (object) downloadMedia.PlainTextHash,
            (object) downloadMedia.DownloadUrl,
            (object) downloadMedia.LocalUrl,
            (object) downloadMedia.CreatedTimestamp,
            (object) downloadMedia.UpdatedTimestamp,
            (object) (int) downloadMedia.DownloadStatus,
            (object) downloadMedia.ResponseCode,
            (object) downloadMedia.DecryptionKey,
            (object) downloadMedia.DownloadSize,
            (object) downloadMedia.DownloadMaxSize
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void RemoveDownload(DownloadMedia downloadMedia)
    {
      Log.d(SqliteMediaTransfer.LogHdr, "Removing Download: {0} {1} {2}", (object) downloadMedia.DownloadId, (object) downloadMedia.LocalUrl, (object) downloadMedia.DownloadStatus);
      SqliteMediaTransfer.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "DELETE FROM downloads WHERE type = ?  AND downloadid = ? ";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[2]
          {
            (object) (int) downloadMedia.DownloadType,
            (object) downloadMedia.DownloadId
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static List<DownloadMedia> GetDownloads(string downloadId = null)
    {
      Log.d(SqliteMediaTransfer.LogHdr, "Getting downloads: {0}", (object) (downloadId ?? "null"));
      try
      {
        List<DownloadMedia> downloads = new List<DownloadMedia>();
        SqliteMediaTransfer.PerformWithDbWrite((Action<Sqlite>) (db =>
        {
          string sql = "SELECT *  FROM downloads";
          string str = "";
          List<string> source = new List<string>();
          if (!string.IsNullOrEmpty(downloadId))
          {
            str += "downloadid = ? ";
            source.Add(downloadId);
          }
          if (source.Count<string>() > 0)
            sql = sql + " WHERE " + str;
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            for (int index = 0; index < source.Count<string>(); ++index)
              preparedStatement.Bind(index, source[index]);
            do
              ;
            while (preparedStatement.Step());
          }
        }));
        return downloads;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception returning downloads");
        return (List<DownloadMedia>) null;
      }
    }
  }
}
