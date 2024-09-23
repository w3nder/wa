// Decompiled with JetBrains decompiler
// Type: WhatsApp.CallLog
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class CallLog
  {
    public static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.CallLogLock");
    private static readonly string DbPath = Constants.IsoStorePath + "\\calls.db";
    private static string LogHeader = "calllogdb";
    private const int LatestSchemaVersion = 2;
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string rollbackTxStmt = "ROLLBACK TRANSACTION";
    private static Dictionary<string, CallLog.ColumnSetter> ColumnDict = CallLog.BuildColumnDict();
    private const byte participantEntryVersion = 1;
    private static Subject<CallRecordUpdate[]> callRecordUpdateSubj = new Subject<CallRecordUpdate[]>();
    private static EventWaitHandle outOfProcKillEvent = (EventWaitHandle) null;
    private static EventWaitHandle callRecordUpdated = new EventWaitHandle(false, EventResetMode.AutoReset, "WhatsApp.CallRecordUpdated");
    private static RefCountAction callRecordOutOfProc = new RefCountAction((Action) (() =>
    {
      if (CallLog.outOfProcKillEvent == null)
        CallLog.outOfProcKillEvent = (EventWaitHandle) new AutoResetEvent(false);
      new Thread((ThreadStart) (() =>
      {
        EventWaitHandle[] waitHandles = new EventWaitHandle[2]
        {
          CallLog.outOfProcKillEvent,
          CallLog.callRecordUpdated
        };
        CallLog.ReloadMaxRowId();
        while (WaitHandle.WaitAny((WaitHandle[]) waitHandles) == 1)
          CallLog.OnOutOfProcDbUpdated();
      })).Start();
    }), (Action) (() => CallLog.outOfProcKillEvent.Set()));
    private static long? maxRowId = new long?();

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write)
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          if (!nativeMediaStorage.FileExists(CallLog.DbPath))
            return;
        }
      }
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(CallLog.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          if (write)
          {
            int currentSchema = CallLog.GetSchemaVersion(db);
            switch (currentSchema)
            {
              case -1:
                if (currentSchema != -1)
                  CallLog.DeleteTables(db, currentSchema);
                currentSchema = CallLog.CreateTables(db);
                break;
              case 2:
                goto label_21;
              default:
                if (currentSchema <= 2)
                  break;
                goto case -1;
            }
            CallLog.SchemaUpdate(db, currentSchema);
          }
label_21:
          onDb(db);
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
                nativeMediaStorage.DeleteFile(CallLog.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            CallLog.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            CallLog.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            CallLog.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      CallLog.Lock.PerformWithLock((Action) (() => CallLog.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      CallLog.PerformWithDb(onDb, true);
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
      List<string> stmtStrings = new List<string>()
      {
        CallLog.beginTxStmt,
        "CREATE TABLE IF NOT EXISTS CallLog\n(CallLogId INTEGER PRIMARY KEY, PeerJid TEXT, CallId TEXT, FromMe INTEGER, StartTime INTEGER, ConnectTime INTEGER, EndTime INTEGER, Result INTEGER, DataUsageTx INTEGER, DataUsageRx INTEGER, VideoCall INTEGER)",
        "CREATE UNIQUE INDEX IF NOT EXISTS PeerJidIdIdx\nON CallLog (PeerJid, CallId)",
        "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
        CallLog.commitTxStmt
      };
      CallLog.RunStatements(db, (IEnumerable<string>) stmtStrings);
      stmtStrings.Clear();
      Dictionary<string, string> columns = db.GetColumnNames(nameof (CallLog)).ToDictionary<string, string>((Func<string, string>) (s => s));
      Action<string> action = (Action<string>) (name =>
      {
        if (columns.ContainsKey(name))
          return;
        stmtStrings.Add(string.Format("ALTER TABLE CallLog ADD COLUMN {0} INTEGER", (object) name));
      });
      action("DataUsageTx");
      action("DataUsageRx");
      action("VideoCall");
      CallLog.RunStatements(db, (IEnumerable<string>) stmtStrings);
      return 1;
    }

    private static void SchemaUpdate(Sqlite db, int currentSchema)
    {
      if (currentSchema >= 2)
        return;
      int num = 2;
      Log.l(CallLog.LogHeader, "running schema update {0}", (object) num);
      try
      {
        Dictionary<string, string> columns = db.GetColumnNames(nameof (CallLog)).ToDictionary<string, string>((Func<string, string>) (s => s));
        List<string> stmtStrings = new List<string>()
        {
          CallLog.beginTxStmt,
          "DELETE FROM metadata",
          "INSERT OR REPLACE INTO metadata VALUES (" + (object) num + ")"
        };
        ((Action<string>) (name =>
        {
          if (columns.ContainsKey(name))
            return;
          stmtStrings.Add(string.Format("ALTER TABLE CallLog ADD COLUMN {0} BLOB", (object) name));
        }))("ParticipantEntries");
        stmtStrings.Add(CallLog.commitTxStmt);
        CallLog.RunStatements(db, (IEnumerable<string>) stmtStrings);
        stmtStrings.Clear();
      }
      catch (Exception ex)
      {
        CallLog.RunStatements(db, (IEnumerable<string>) new List<string>()
        {
          CallLog.rollbackTxStmt
        });
        throw;
      }
    }

    public static void Submit(CallRecord rec)
    {
      bool written = false;
      CallLog.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql1 = "SELECT EXISTS(SELECT CallLogId FROM CallLog WHERE PeerJid = ? AND CallId = ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql1))
        {
          preparedStatement.Bind(0, rec.PeerJid);
          preparedStatement.Bind(1, rec.CallId);
          if (preparedStatement.Step())
          {
            if ((long) preparedStatement.Columns[0] != 0L)
              return;
          }
        }
        string sql2 = "INSERT INTO CallLog (PeerJid, CallId, FromMe, StartTime, ConnectTime, EndTime, Result, DataUsageTx, DataUsageRx, VideoCall, ParticipantEntries) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql2))
        {
          object[] objArray = new object[11];
          objArray[0] = (object) rec.PeerJid;
          objArray[1] = (object) rec.CallId;
          objArray[2] = (object) (rec.FromMe ? 1 : 0);
          objArray[3] = (object) rec.StartTime.ToUnixTime();
          DateTime? connectTime = rec.ConnectTime;
          long? nullable;
          if (!connectTime.HasValue)
          {
            nullable = new long?();
          }
          else
          {
            connectTime = rec.ConnectTime;
            nullable = new long?(connectTime.Value.ToUnixTime());
          }
          objArray[4] = (object) nullable;
          objArray[5] = (object) rec.EndTime.ToUnixTime();
          objArray[6] = (object) (long) rec.Result;
          objArray[7] = (object) rec.DataUsageTx;
          objArray[8] = (object) rec.DataUsageRx;
          objArray[9] = (object) (rec.VideoCall.Value ? 1 : 0);
          objArray[10] = rec.ParticipantEntries == null ? (object) (byte[]) null : (object) CallLog.ConvertParticpantEntriesToBlob(rec.ParticipantEntries);
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT last_insert_rowid()"))
        {
          if (!preparedStatement.Step())
            return;
          rec.CallRecordId = (long) preparedStatement.Columns[0];
          written = true;
        }
      }));
      if (!written)
        return;
      if (AppState.IsBackgroundAgent)
        CallLog.callRecordUpdated.Set();
      else
        CallLog.NotifyUpdate(new CallRecordUpdate(rec, DbDataUpdate.Types.Added));
    }

    public static void Delete(CallRecord record, bool notify)
    {
      CallLog.Delete(new CallRecord[1]{ record }, notify);
    }

    public static void Delete(CallRecord[] records, bool notify)
    {
      Dictionary<string, string[]> grouped = ((IEnumerable<CallRecord>) records).GroupBy<CallRecord, string>((Func<CallRecord, string>) (r => r.PeerJid)).ToDictionary<IGrouping<string, CallRecord>, string, string[]>((Func<IGrouping<string, CallRecord>, string>) (g => g.Key), (Func<IGrouping<string, CallRecord>, string[]>) (g => g.Select<CallRecord, string>((Func<CallRecord, string>) (r => r.CallId)).ToArray<string>()));
      CallLog.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        foreach (KeyValuePair<string, string[]> keyValuePair in grouped)
          CallLog.DeleteUnlocked(db, keyValuePair.Key, keyValuePair.Value);
        if (!notify)
          return;
        CallLog.NotifyUpdate(((IEnumerable<CallRecord>) records).Select<CallRecord, CallRecordUpdate>((Func<CallRecord, CallRecordUpdate>) (cr => new CallRecordUpdate(cr, DbDataUpdate.Types.Deleted))).ToArray<CallRecordUpdate>());
      }));
    }

    private static void DeleteUnlocked(Sqlite db, string peerJid, string[] callIds)
    {
      if (peerJid == null || callIds == null || !((IEnumerable<string>) callIds).Any<string>())
        return;
      StringBuilder stringBuilder = new StringBuilder("DELETE FROM CallLog WHERE PeerJid = ?\n");
      if (callIds.Length > 1)
      {
        stringBuilder.Append("AND CallId IN (");
        stringBuilder.Append(string.Join(", ", ((IEnumerable<string>) callIds).Select<string, string>((Func<string, string>) (_ => "?")).ToArray<string>()));
        stringBuilder.Append(")");
      }
      else
        stringBuilder.Append("AND CallId = ?");
      using (Sqlite.PreparedStatement preparedStatement1 = db.PrepareStatement(stringBuilder.ToString()))
      {
        int num1 = 0;
        Sqlite.PreparedStatement preparedStatement2 = preparedStatement1;
        int idx = num1;
        int num2 = idx + 1;
        string val = peerJid;
        preparedStatement2.Bind(idx, val);
        foreach (string callId in callIds)
          preparedStatement1.Bind(num2++, callId);
        preparedStatement1.Step();
      }
      if (AppState.IsBackgroundAgent)
        return;
      CallLog.ReloadMaxRowIdUnlocked(db);
    }

    public static void DeleteAll()
    {
      CallLog.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("DELETE FROM CallLog"))
          preparedStatement.Step();
        CallLog.maxRowId = new long?();
      }));
    }

    public static CallRecord[] Load(
      string jid = null,
      int? offset = null,
      int? count = null,
      CallRecord.CallResult? result = null)
    {
      List<object> binds = new List<object>();
      StringBuilder sql = new StringBuilder();
      List<CallRecord> res = (List<CallRecord>) null;
      sql.Append("SELECT * FROM CallLog");
      List<string> values = new List<string>();
      if (jid != null)
      {
        values.Add("PeerJid = ?");
        binds.Add((object) jid);
      }
      if (result.HasValue)
      {
        values.Add("Result = ?");
        binds.Add((object) (long) result.Value);
      }
      if (values.Count != 0)
        sql.AppendFormat(" WHERE {0}", (object) string.Join(" AND ", (IEnumerable<string>) values));
      sql.Append(" ORDER BY CallLogId ");
      sql.Append("DESC");
      if (offset.HasValue)
      {
        sql.Append(" OFFSET ?");
        binds.Add((object) offset.Value);
      }
      if (count.HasValue)
      {
        sql.Append(" LIMIT ?");
        binds.Add((object) count.Value);
      }
      CallLog.PerformWithDb((Action<Sqlite>) (db =>
      {
        using (Sqlite.PreparedStatement stmt = db.PrepareStatement(sql.ToString()))
        {
          sql.Clear();
          sql = (StringBuilder) null;
          int num = 0;
          foreach (object o in binds)
            stmt.BindObject(num++, o);
          binds.Clear();
          binds = (List<object>) null;
          res = CallLog.ParseLoad(stmt);
        }
      }));
      return (res ?? new List<CallRecord>()).ToArray();
    }

    public static CallRecord GetLastCall()
    {
      CallRecord[] source = CallLog.Load(count: new int?(1), result: new CallRecord.CallResult?(CallRecord.CallResult.Connected));
      return ((IEnumerable<CallRecord>) source).Count<CallRecord>() > 0 ? source[0] : (CallRecord) null;
    }

    public static int GetMissedCallCount(DateTime? after)
    {
      List<object> bindedParams = new List<object>();
      StringBuilder sqlSb = new StringBuilder("SELECT COUNT(CallLogId) FROM CallLog \n");
      sqlSb.Append("WHERE Result = ? \n");
      bindedParams.Add((object) 2L);
      if (after.HasValue)
      {
        sqlSb.Append("AND StartTime > ?");
        bindedParams.Add((object) after.Value.ToUnixTime());
      }
      int count = 0;
      CallLog.PerformWithDb((Action<Sqlite>) (db =>
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sqlSb.ToString()))
        {
          int num = 0;
          foreach (object o in bindedParams)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
          count = (int) (long) preparedStatement.Columns[0];
        }
      }));
      return Math.Max(count, 0);
    }

    private static Dictionary<string, CallLog.ColumnSetter> BuildColumnDict()
    {
      Dictionary<string, CallLog.ColumnSetter> dictionary = new Dictionary<string, CallLog.ColumnSetter>();
      Func<object, DateTime?> parseDt = (Func<object, DateTime?>) (o => o != null ? new DateTime?(FunXMPP.UnixEpoch.AddSeconds((double) (long) o)) : new DateTime?());
      dictionary["CallLogId"] = (CallLog.ColumnSetter) ((rec, value) => rec.CallRecordId = (long) value);
      dictionary["PeerJid"] = (CallLog.ColumnSetter) ((rec, value) => rec.PeerJid = (string) value);
      dictionary["CallId"] = (CallLog.ColumnSetter) ((rec, value) => rec.CallId = (string) value);
      dictionary["FromMe"] = (CallLog.ColumnSetter) ((rec, value) => rec.FromMe = (long) value != 0L);
      dictionary["StartTime"] = (CallLog.ColumnSetter) ((rec, value) => rec.StartTime = parseDt(value).Value);
      dictionary["ConnectTime"] = (CallLog.ColumnSetter) ((rec, value) => rec.ConnectTime = parseDt(value));
      dictionary["EndTime"] = (CallLog.ColumnSetter) ((rec, value) => rec.EndTime = parseDt(value).Value);
      dictionary["Result"] = (CallLog.ColumnSetter) ((rec, value) => rec.Result = (CallRecord.CallResult) (long) value);
      dictionary["DataUsageTx"] = (CallLog.ColumnSetter) ((rec, value) => rec.DataUsageTx = (long?) value ?? 0L);
      dictionary["DataUsageRx"] = (CallLog.ColumnSetter) ((rec, value) => rec.DataUsageRx = (long?) value ?? 0L);
      dictionary["VideoCall"] = (CallLog.ColumnSetter) ((rec, value) => rec.VideoCall = value == null ? new bool?() : new bool?((long) value == 1L));
      dictionary["ParticipantEntries"] = (CallLog.ColumnSetter) ((rec, value) => rec.ParticipantEntries = value == null ? (List<CallRecord.CallLogEntryParticipant>) null : CallLog.ConvertBlobToParticpantEntries((byte[]) value));
      return dictionary;
    }

    private static List<CallRecord> ParseLoad(Sqlite.PreparedStatement stmt)
    {
      List<CallRecord> load = new List<CallRecord>();
      CallLog.ColumnSetter[] columnSetterArray = (CallLog.ColumnSetter[]) null;
      while (stmt.Step())
      {
        CallRecord rec = new CallRecord();
        if (columnSetterArray == null)
        {
          columnSetterArray = new CallLog.ColumnSetter[stmt.ColumnNames.Length];
          int num = 0;
          foreach (string columnName in stmt.ColumnNames)
            CallLog.ColumnDict.TryGetValue(columnName, out columnSetterArray[num++]);
        }
        int index = 0;
        object[] columns = stmt.Columns;
        foreach (CallLog.ColumnSetter columnSetter in columnSetterArray)
        {
          if (columnSetter != null)
            columnSetter(rec, columns[index]);
          ++index;
        }
        load.Add(rec);
      }
      return load;
    }

    private static List<CallRecord.CallLogEntryParticipant> ConvertBlobToParticpantEntries(
      byte[] blob)
    {
      List<CallRecord.CallLogEntryParticipant> particpantEntries = (List<CallRecord.CallLogEntryParticipant>) null;
      try
      {
        if (blob != null & blob.Length != 0)
        {
          BinaryData binaryData = new BinaryData(blob);
          if (binaryData.ReadByte(0) == (byte) 1)
          {
            int capacity = binaryData.ReadInt32(1);
            particpantEntries = new List<CallRecord.CallLogEntryParticipant>(capacity);
            int newOffset = 5;
            for (int index = 0; index < capacity; ++index)
            {
              CallRecord.CallLogEntryParticipant entryParticipant = new CallRecord.CallLogEntryParticipant();
              entryParticipant.res = (CallRecord.CallResult) binaryData.ReadInt32(newOffset);
              newOffset += 4;
              entryParticipant.jid = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
              particpantEntries.Add(entryParticipant);
            }
          }
        }
      }
      catch (Exception ex)
      {
        string context = CallLog.LogHeader + " Exception creating Participant Entries";
        Log.LogException(ex, context);
        particpantEntries = (List<CallRecord.CallLogEntryParticipant>) null;
      }
      return particpantEntries;
    }

    private static byte[] ConvertParticpantEntriesToBlob(
      List<CallRecord.CallLogEntryParticipant> particpantEntries)
    {
      if (particpantEntries == null)
        return (byte[]) null;
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendInt32(particpantEntries.Count<CallRecord.CallLogEntryParticipant>());
      foreach (CallRecord.CallLogEntryParticipant particpantEntry in particpantEntries)
      {
        binaryData.AppendInt32((int) particpantEntry.res);
        binaryData.AppendStrWithLengthPrefix(particpantEntry.jid);
      }
      return binaryData.Get();
    }

    private static void DeleteTables(Sqlite db, int currentSchema)
    {
      if (currentSchema < 1)
        return;
      try
      {
        List<string> stringList = new List<string>()
        {
          CallLog.beginTxStmt,
          "DROP TABLE IF EXISTS metadata",
          "DROP TABLE IF EXISTS CallLog",
          CallLog.commitTxStmt
        };
        CallLog.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        CallLog.RunStatements(db, (IEnumerable<string>) new List<string>()
        {
          CallLog.rollbackTxStmt
        });
        throw;
      }
    }

    public static void DeleteDb()
    {
      CallLog.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(CallLog.DbPath);
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

    private static void ReloadMaxRowIdUnlocked(Sqlite db)
    {
      long? nullable = new long?();
      string sql = "SELECT CallLogId FROM CallLog ORDER BY CallLogId DESC LIMIT 1";
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
      {
        if (preparedStatement.Step())
          nullable = new long?((long) preparedStatement.Columns[0]);
      }
      CallLog.maxRowId = nullable;
    }

    private static void ReloadMaxRowId()
    {
      CallLog.PerformWithDb(new Action<Sqlite>(CallLog.ReloadMaxRowIdUnlocked));
    }

    private static void OnOutOfProcDbUpdated()
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<object> binds = new List<object>();
      stringBuilder.Append("SELECT * FROM CallLog");
      if (CallLog.maxRowId.HasValue)
      {
        stringBuilder.Append(" WHERE CallLogId > ?");
        binds.Add((object) CallLog.maxRowId.Value);
      }
      stringBuilder.Append(" ORDER BY CallLogId ASC");
      string sqlString = stringBuilder.ToString();
      stringBuilder.Clear();
      CallLog.PerformWithDb((Action<Sqlite>) (db =>
      {
        using (Sqlite.PreparedStatement stmt = db.PrepareStatement(sqlString))
        {
          for (int index = 0; index < binds.Count; ++index)
            stmt.BindObject(index, binds[index]);
          List<CallRecord> load = CallLog.ParseLoad(stmt);
          if (load.Count == 0)
            return;
          CallLog.maxRowId = new long?(load[load.Count - 1].CallRecordId);
          CallLog.NotifyUpdate(load.Select<CallRecord, CallRecordUpdate>((Func<CallRecord, CallRecordUpdate>) (row => new CallRecordUpdate(row, DbDataUpdate.Types.Added))).ToArray<CallRecordUpdate>());
        }
      }));
    }

    public static IObservable<CallRecordUpdate[]> GetUpdateObservable()
    {
      return Observable.CreateWithDisposable<CallRecordUpdate[]>((Func<IObserver<CallRecordUpdate[]>, IDisposable>) (observer => (IDisposable) new CompositeDisposable(new IDisposable[2]
      {
        CallLog.callRecordUpdateSubj.Subscribe(observer),
        CallLog.callRecordOutOfProc.Subscribe()
      })));
    }

    private static void NotifyUpdate(CallRecordUpdate update)
    {
      CallLog.NotifyUpdate(new CallRecordUpdate[1]{ update });
    }

    private static void NotifyUpdate(CallRecordUpdate[] updates)
    {
      CallLog.callRecordUpdateSubj.OnNext(updates);
    }

    private delegate void ColumnSetter(CallRecord rec, object value);
  }
}
