// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteRepair
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using WhatsApp.Events;
using WhatsApp.RegularExpressions;
using WhatsAppNative;


namespace WhatsApp
{
  public class SqliteRepair
  {
    private const int NUM_ROWS_IN_BATCH = 20000;
    private const double SUCCESS_PERCENTAGE = 0.3;
    private const string RepairTableName = "db_repair_progress";
    private const string TableNameColumn = "table_name";
    private const string TableIndexColumn = "dump_index";
    private const string TableOrderColumn = "ascending";
    private const string CreateDumpResumeTableSql = "CREATE TABLE db_repair_progress(table_name TEXT, dump_index int, ascending int);";
    private const string InsertTableToResumeSql = "INSERT INTO db_repair_progress(table_name, dump_index, ascending) VALUES (?, 0, 1);";
    private const string GetTableDumpStateSql = "SELECT dump_index,ascending FROM db_repair_progress WHERE table_name=?;";
    private const string GetTablesLeftToDumpSql = "SELECT table_name,dump_index,ascending FROM db_repair_progress;";
    private const string MarkTableAsDumpedSql = "DELETE FROM db_repair_progress WHERE table_name=?;";
    private const string DropResumeDumpTableSql = "DROP TABLE IF EXISTS db_repair_progress;";
    private static bool pauseRepair;
    private const string LOG_TAG = "db_repair";

    public static bool IsRepairStarted()
    {
      return Settings.MessagesDbRepairState != SqliteRepair.SqliteRepairState.Unstarted;
    }

    public static bool IsRepairDone()
    {
      switch (Settings.MessagesDbRepairState)
      {
        case SqliteRepair.SqliteRepairState.Successful:
        case SqliteRepair.SqliteRepairState.Failed:
          return true;
        default:
          return false;
      }
    }

    public static bool IsRepairInProgress()
    {
      switch (Settings.MessagesDbRepairState)
      {
        case SqliteRepair.SqliteRepairState.Unstarted:
        case SqliteRepair.SqliteRepairState.Successful:
        case SqliteRepair.SqliteRepairState.Failed:
          return false;
        default:
          return true;
      }
    }

    public static void ResumeRepair() => SqliteRepair.pauseRepair = false;

    public static void PauseRepair()
    {
      Log.l("db_repair", "maybe pausing repair {0}", (object) SqliteRepair.pauseRepair);
      SqliteRepair.pauseRepair = true;
    }

    public static bool IsPaused() => SqliteRepair.pauseRepair;

    public static bool IsDbHealthy(string dbFileName)
    {
      try
      {
        using (Sqlite sqlite = new Sqlite(dbFileName, SqliteOpenFlags.Defaults))
        {
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("PRAGMA integrity_check"))
          {
            if (preparedStatement.Step())
            {
              if (!((string) preparedStatement.Columns[0]).ToLowerInvariant().Equals("ok"))
                return false;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if ((long) ex.HResult == (long) Sqlite.HRForError(11U))
          return false;
        throw;
      }
      return true;
    }

    public static bool Reindex(string dbFileName, string indexName)
    {
      DateTime? start = PerformanceTimer.Start();
      try
      {
        using (Sqlite sqlite = new Sqlite(dbFileName, SqliteOpenFlags.Defaults))
        {
          string sql = "REINDEX " + indexName;
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement(sql))
            preparedStatement.Step();
        }
      }
      catch (Exception ex)
      {
        return false;
      }
      PerformanceTimer.End(string.Format("Reindexing {0}", (object) indexName), start);
      return true;
    }

    public static SqliteRepair.IntegrityCheckResults GetIntegrityCheckResults(string dbFileName)
    {
      SqliteRepair.IntegrityCheckResults integrityCheckResults = new SqliteRepair.IntegrityCheckResults();
      integrityCheckResults.indexesToRepair = new Dictionary<string, int>();
      integrityCheckResults.nonIndexErrors = new List<string>();
      try
      {
        Regex regex = new Regex("index (\\w+)$");
        using (Sqlite sqlite = new Sqlite(dbFileName, SqliteOpenFlags.Defaults))
        {
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("PRAGMA integrity_check"))
          {
            while (preparedStatement.Step())
            {
              string column = (string) preparedStatement.Columns[0];
              if (!column.ToLower().Equals("ok"))
              {
                Match match = regex.Match(column);
                if (match.Success)
                {
                  string key = match.Groups[1].Value;
                  int num = integrityCheckResults.indexesToRepair.GetValueOrDefault<string, int>(key) + 1;
                  if (num == 1)
                    Log.l("sqlite_repair", "found corrupt index: " + key);
                  integrityCheckResults.indexesToRepair[key] = num;
                }
                else
                {
                  Log.l("sqlite_repair", "hit error: " + column);
                  integrityCheckResults.nonIndexErrors.Add(column);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (ex.HResult == (int) Sqlite.HRForError(11U))
        {
          string synopsis = ex.GetSynopsis();
          Log.l("sqlite_repair", "hit error: " + synopsis);
          integrityCheckResults.nonIndexErrors.Add(synopsis);
        }
        else
          throw;
      }
      return integrityCheckResults;
    }

    public static void DumpAndRestore(string messagesDb, ChatDatabaseRepairEvent fsevent = null)
    {
      try
      {
        string absolutePath1 = SqliteRepair.GetAbsolutePath(messagesDb);
        string relativePath = messagesDb + "_new";
        string absolutePath2 = SqliteRepair.GetAbsolutePath(relativePath);
        SqliteRepair.GoToState(new SqliteRepair.DumpParameters()
        {
          newDbName = relativePath,
          newDbPath = absolutePath2,
          oldDbName = messagesDb,
          oldDbPath = absolutePath1,
          fsevent = fsevent
        });
      }
      catch (Exception ex)
      {
        string context = string.Format("DumpAndRestore failed at state {0}", (object) Settings.MessagesDbRepairState);
        Log.SendCrashLog(ex, context);
        Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.Failed;
      }
    }

    private static void GoToState(SqliteRepair.DumpParameters parameters)
    {
      switch (Settings.MessagesDbRepairState)
      {
        case SqliteRepair.SqliteRepairState.Unstarted:
          SqliteRepair.StartDumpAndRestore(parameters);
          break;
        case SqliteRepair.SqliteRepairState.SchemaCopied:
          SqliteRepair.InitializeSqliteDumpTable(parameters);
          break;
        case SqliteRepair.SqliteRepairState.ReadyToDump:
          SqliteRepair.CopyTables(parameters);
          break;
        case SqliteRepair.SqliteRepairState.NeedsValidation:
          SqliteRepair.ValidateRestoredDb(parameters);
          break;
      }
    }

    private static void StartDumpAndRestore(SqliteRepair.DumpParameters parameters)
    {
      if (SqliteRepair.pauseRepair)
        return;
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        storeForApplication.DeleteFile(parameters.newDbName);
      Log.l("db_repair", "copying database schema");
      try
      {
        SqliteRepair.CopySchema(parameters.oldDbPath, parameters.newDbPath);
      }
      catch (Exception ex)
      {
        Log.l("db_repair", "failed to copy schema");
        throw ex;
      }
      Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.SchemaCopied;
      Log.l("db_repair", "schema copied successfully");
      SqliteRepair.GoToState(parameters);
    }

    private static void InitializeSqliteDumpTable(SqliteRepair.DumpParameters p)
    {
      if (SqliteRepair.pauseRepair)
        return;
      IEnumerable<string> tablesInDb = SqliteRepair.GetTablesInDb(p.oldDbPath);
      if (tablesInDb == null || tablesInDb.Count<string>() <= 0)
      {
        Log.l("db_repair", "no tables in db");
        throw new Exception("No tables in database");
      }
      Log.l("db_repair", "creating resume table");
      using (Sqlite sqlite = new Sqlite(p.newDbPath, SqliteOpenFlags.Defaults))
      {
        sqlite.BeginTransaction();
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("CREATE TABLE db_repair_progress(table_name TEXT, dump_index int, ascending int);"))
          preparedStatement.Step();
        foreach (string val in tablesInDb)
        {
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("INSERT INTO db_repair_progress(table_name, dump_index, ascending) VALUES (?, 0, 1);"))
          {
            preparedStatement.Bind(0, val);
            preparedStatement.Step();
          }
        }
        sqlite.CommitTransaction();
      }
      Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.ReadyToDump;
      SqliteRepair.GoToState(p);
    }

    private static void CopySchema(string oldDbPath, string newDbPath)
    {
      string str = "temp_db_schema_dump";
      string absoluteArgumentPath = SqliteRepair.GetAbsoluteArgumentPath(str);
      try
      {
        if (!Sqlite.Shell.ExecuteMetaCommandWithOutput(oldDbPath, ".schema", absoluteArgumentPath))
        {
          Log.l("db_repair", "schema dump failed");
          throw new Exception(".schema dump failure");
        }
        string ftsSql;
        SqliteRepair.CleanupSchemaDump(str, out ftsSql);
        if (!Sqlite.Shell.ExecuteMetaCommand(newDbPath, string.Format(".read {0}", (object) absoluteArgumentPath)))
        {
          Log.l("db_repair", "read of schema dump failed");
          throw new Exception("DumpAndRestore: .read meta command failed");
        }
        if (string.IsNullOrEmpty(ftsSql))
          return;
        using (Sqlite sqlite = new Sqlite(newDbPath, SqliteOpenFlags.Defaults))
        {
          sqlite.RegisterTokenizer();
          using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement(ftsSql))
            preparedStatement.Step();
        }
      }
      finally
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(str))
            storeForApplication.DeleteFile(str);
        }
      }
    }

    private static bool ShouldCopyTable(string tableName)
    {
      string upper = tableName.ToUpper();
      foreach (string str in MessagesContext.TablesOnlyCreateRepair)
      {
        if (str.ToUpper().Equals(upper))
          return false;
      }
      return true;
    }

    private static void CopyTables(SqliteRepair.DumpParameters p)
    {
      if (SqliteRepair.pauseRepair)
        return;
      foreach (SqliteRepair.TableToDump table in SqliteRepair.GetRemainingTablesInDump(p.newDbPath))
      {
        if (SqliteRepair.pauseRepair)
          return;
        if (SqliteRepair.ShouldCopyTable(table.tableName))
        {
          Log.l("db_repair", string.Format("copying table {0}", (object) table.tableName));
          DateTime? start = PerformanceTimer.Start();
          SqliteRepair.CopyTable(table, p.oldDbPath, p.newDbPath);
          PerformanceTimer.End("db_repair", string.Format("Dump and Restore of table {0}", (object) table.tableName), start);
        }
        else
          Log.l("db_repair", string.Format("ignoring table {0}", (object) table.tableName));
      }
      Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.NeedsValidation;
      SqliteRepair.GoToState(p);
    }

    private static void CopyTable(
      SqliteRepair.TableToDump table,
      string oldDbPath,
      string newDbPath)
    {
      if (SqliteRepair.pauseRepair)
        return;
      string str = "tmp_db_table_dump";
      try
      {
        using (IsolatedStorageFile.GetUserStoreForApplication())
        {
          bool flag1 = true;
          while (flag1)
          {
            bool flag2 = SqliteRepair.DumpTableData(oldDbPath, table, str);
            flag1 = SqliteRepair.DumpHasRows(str);
            if (SqliteRepair.pauseRepair)
              return;
            if (flag1)
            {
              if (Sqlite.Shell.ExecuteMetaCommand(newDbPath, string.Format(".read {0}", (object) SqliteRepair.GetAbsoluteArgumentPath(str))))
                Log.l("db_repair", string.Format("successfully read partial dump of table {0}", (object) table.tableName));
              else
                Log.l("db_repair", string.Format("failed to read partial dump of table {0}", (object) table.tableName));
            }
            SqliteRepair.UpdateTableDumpState(newDbPath, table);
            Log.l("db_repair", string.Format("copied {0} rows:{1},{2}, asc={3}", (object) table.tableName, (object) table.index, (object) (table.index + 20000), (object) table.ascending));
            if (SqliteRepair.pauseRepair)
              return;
            if (!flag2)
              break;
          }
          SqliteRepair.MarkTableAsCompleted(newDbPath, table.tableName);
        }
      }
      finally
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(str))
            storeForApplication.DeleteFile(str);
        }
      }
    }

    private static void UpdateTableDumpState(string newDbLoc, SqliteRepair.TableToDump table)
    {
      using (Sqlite sqlite = new Sqlite(newDbLoc, SqliteOpenFlags.READONLY))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("SELECT dump_index,ascending FROM db_repair_progress WHERE table_name=?;"))
        {
          preparedStatement.Bind(0, table.tableName);
          if (preparedStatement.Step())
          {
            table.index = (int) (long) preparedStatement.Columns[0];
            table.ascending = (long) preparedStatement.Columns[1] == 1L;
          }
        }
      }
      Log.l("db_repair", string.Format("on table: name={0}, index={1}, asc={2}", (object) table.tableName, (object) table.index, (object) table.ascending));
    }

    private static IEnumerable<SqliteRepair.TableToDump> GetRemainingTablesInDump(string newDbPath)
    {
      using (Sqlite sqlite = new Sqlite(newDbPath, SqliteOpenFlags.Defaults))
      {
        List<SqliteRepair.TableToDump> remainingTablesInDump = new List<SqliteRepair.TableToDump>();
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("SELECT table_name,dump_index,ascending FROM db_repair_progress;"))
        {
          while (preparedStatement.Step())
          {
            if (preparedStatement.Columns.Length == 3)
            {
              string column1 = (string) preparedStatement.Columns[0];
              int column2 = (int) (long) preparedStatement.Columns[1];
              bool flag = (long) preparedStatement.Columns[2] == 1L;
              remainingTablesInDump.Add(new SqliteRepair.TableToDump()
              {
                tableName = column1,
                index = column2,
                ascending = flag
              });
            }
          }
        }
        return (IEnumerable<SqliteRepair.TableToDump>) remainingTablesInDump;
      }
    }

    private static IEnumerable<string> GetTablesInDb(string dbPath)
    {
      string str = "tmp_db_table_dump";
      string absoluteArgumentPath = SqliteRepair.GetAbsoluteArgumentPath(str);
      if (!Sqlite.Shell.ExecuteMetaCommandWithOutput(dbPath, ".tables", absoluteArgumentPath))
        return (IEnumerable<string>) null;
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(str, FileMode.Open))
        {
          using (StreamReader streamReader = new StreamReader((Stream) storageFileStream))
            return (IEnumerable<string>) streamReader.ReadToEnd().Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);
        }
      }
    }

    private static long GetMessageCount(string dbPath)
    {
      long messageCount = -1;
      using (Sqlite sqlite = new Sqlite(dbPath, SqliteOpenFlags.READONLY))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("SELECT COUNT(*) FROM Messages"))
        {
          if (preparedStatement.Step())
            messageCount = (long) (int) (long) preparedStatement.Columns[0];
        }
      }
      return messageCount;
    }

    private static void ValidateRestoredDb(SqliteRepair.DumpParameters p)
    {
      using (Sqlite sqlite = new Sqlite(p.newDbPath, SqliteOpenFlags.Defaults))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("DROP TABLE IF EXISTS db_repair_progress;"))
          preparedStatement.Step();
      }
      Log.l("db_repair", "dropped resume table");
      bool flag = false;
      double num = 0.0;
      long messageCount1 = SqliteRepair.GetMessageCount(p.oldDbPath);
      long messageCount2 = SqliteRepair.GetMessageCount(p.newDbPath);
      if (messageCount2 < 0L)
        flag = true;
      else if (messageCount1 <= 0L && messageCount2 > 0L)
      {
        flag = false;
      }
      else
      {
        num = (double) messageCount2 / (double) messageCount1;
        if (num < 0.3)
          flag = true;
      }
      if (p.fsevent != null)
      {
        p.fsevent.databaseRepairDumpAndRestoreRecoveryPercentage = new long?((long) (int) (num * 100.0));
        p.fsevent.databaseRepairDumpAndRestoreResult = new bool?(!flag);
      }
      Log.l("db_repair", string.Format("Recovered {0} out of {1} messages", (object) messageCount2, (object) messageCount1));
      if (flag)
        throw new Exception("DumpAndRestore unsuccessful");
      SqliteRepair.CompleteRestore(p);
    }

    private static void CompleteRestore(SqliteRepair.DumpParameters p)
    {
      MessagesContext.Reset(true);
      Log.l("db_repair", "replacing old db with new");
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        nativeMediaStorage.MoveFileWithOverwrite(MediaStorage.GetAbsolutePath(p.newDbName), MediaStorage.GetAbsolutePath(p.oldDbName));
        Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.Successful;
        Log.l("db_repair", "repair successful :)");
      }
    }

    private static void MarkTableAsCompleted(string newDbPath, string tableName)
    {
      using (Sqlite sqlite = new Sqlite(newDbPath, SqliteOpenFlags.Defaults))
      {
        using (Sqlite.PreparedStatement preparedStatement = sqlite.PrepareStatement("DELETE FROM db_repair_progress WHERE table_name=?;"))
        {
          preparedStatement.Bind(0, tableName);
          preparedStatement.Step();
        }
      }
      Log.l("db_repair", string.Format("copied table {0}", (object) tableName));
    }

    private static bool DumpTableData(
      string dbPath,
      SqliteRepair.TableToDump table,
      string outputFileName)
    {
      string Command = string.Format(".ddump {0} {1} {2} {3}", (object) table.tableName, (object) table.index, (object) 20000, table.ascending ? (object) "yes" : (object) "no");
      return Sqlite.Shell.ExecuteMetaCommandWithOutput(dbPath, Command, SqliteRepair.GetAbsoluteArgumentPath(outputFileName));
    }

    private static bool DumpHasRows(string dumpFileLoc)
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        using (IsolatedStorageFileStream storageFileStream = storeForApplication.OpenFile(dumpFileLoc, FileMode.Open))
        {
          using (StreamReader streamReader = new StreamReader((Stream) storageFileStream))
          {
            for (string str = streamReader.ReadLine(); str != null; str = streamReader.ReadLine())
            {
              if (str.StartsWith("INSERT INTO"))
                return true;
            }
            return false;
          }
        }
      }
    }

    private static void CleanupSchemaDump(string schemaFile, out string ftsSql)
    {
      string str1 = schemaFile + ".tmp";
      try
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream storageFileStream1 = storeForApplication.OpenFile(str1, FileMode.Create))
          {
            using (IsolatedStorageFileStream storageFileStream2 = storeForApplication.OpenFile(schemaFile, FileMode.Open))
            {
              using (StreamReader streamReader = new StreamReader((Stream) storageFileStream2))
              {
                using (StreamWriter streamWriter = new StreamWriter((Stream) storageFileStream1))
                {
                  string str2 = streamReader.ReadLine();
                  StringBuilder stringBuilder = new StringBuilder();
                  string str3;
                  for (; str2 != null; str2 = str3)
                  {
                    str3 = streamReader.ReadLine();
                    if (str3 == null && str2.ToUpper().Contains("ROLLBACK;"))
                      str2 = str2.ToUpper().Replace("ROLLBACK;", "COMMIT TRANSACTION;");
                    foreach (string str4 in MessagesContext.TablesOnlyCopyRepair)
                    {
                      if (str2.ToUpper().StartsWith(string.Format("CREATE TABLE IF NOT EXISTS '{0}'", (object) str4.ToUpper())))
                      {
                        str2 = "";
                        break;
                      }
                    }
                    bool flag = false;
                    foreach (string str5 in MessagesContext.TablesNeedTokenizer)
                    {
                      if (str2.ToUpper().StartsWith(string.Format("CREATE VIRTUAL TABLE {0} ", (object) str5.ToUpper())))
                      {
                        stringBuilder.AppendLine(str2);
                        flag = true;
                        break;
                      }
                    }
                    if (!flag && !string.IsNullOrEmpty(str2))
                    {
                      Log.l("db_repair", "writing schema: {0}", (object) str2);
                      streamWriter.WriteLine(str2);
                    }
                  }
                  ftsSql = stringBuilder.ToString();
                }
              }
            }
          }
          storeForApplication.DeleteFile(schemaFile);
          storeForApplication.MoveFile(str1, schemaFile);
        }
      }
      finally
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(str1))
            storeForApplication.DeleteFile(str1);
        }
      }
    }

    public static string GetAbsolutePath(string relativePath)
    {
      return string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) relativePath);
    }

    public static string GetAbsoluteArgumentPath(string argumentPath)
    {
      return SqliteRepair.GetAbsolutePath(argumentPath).Replace("\\", "\\\\");
    }

    public enum SqliteRepairState
    {
      Unstarted,
      SchemaCopied,
      ReadyToDump,
      NeedsValidation,
      Successful,
      Failed,
    }

    private class DumpParameters
    {
      public string newDbName;
      public string oldDbName;
      public string newDbPath;
      public string oldDbPath;
      public ChatDatabaseRepairEvent fsevent;
    }

    private class TableToDump
    {
      public string tableName;
      public int index;
      public bool ascending;
    }

    public class IntegrityCheckResults
    {
      public Dictionary<string, int> indexesToRepair;
      public List<string> nonIndexErrors;

      public int totalErrorCount => this.indexesToRepair.Count + this.nonIndexErrors.Count;
    }
  }
}
