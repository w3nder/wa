// Decompiled with JetBrains decompiler
// Type: WhatsApp.Sqlite
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WhatsAppNative;


namespace WhatsApp
{
  public class Sqlite : IDisposable
  {
    public const uint SQLITE_ERROR = 1;
    public const uint SQLITE_INTERNAL = 2;
    public const uint SQLITE_PERM = 3;
    public const uint SQLITE_ABORT = 4;
    public const uint SQLITE_BUSY = 5;
    public const uint SQLITE_LOCKED = 6;
    public const uint SQLITE_NOMEM = 7;
    public const uint SQLITE_READONLY = 8;
    public const uint SQLITE_INTERRUPT = 9;
    public const uint SQLITE_IOERR = 10;
    public const uint SQLITE_CORRUPT = 11;
    public const uint SQLITE_NOTFOUND = 12;
    public const uint SQLITE_FULL = 13;
    public const uint SQLITE_CANTOPEN = 14;
    public const uint SQLITE_PROTOCOL = 15;
    public const uint SQLITE_EMPTY = 16;
    public const uint SQLITE_SCHEMA = 17;
    public const uint SQLITE_TOOBIG = 18;
    public const uint SQLITE_CONSTRAINT = 19;
    public const uint SQLITE_MISMATCH = 20;
    public const uint SQLITE_MISUSE = 21;
    public const uint SQLITE_NOLFS = 22;
    public const uint SQLITE_AUTH = 23;
    public const uint SQLITE_FORMAT = 24;
    public const uint SQLITE_RANGE = 25;
    public const uint SQLITE_NOTADB = 26;
    public const uint SQLITE_NOTICE = 27;
    public const uint SQLITE_WARNING = 28;
    public const uint SQLITE_ROW = 100;
    public const uint SQLITE_DONE = 101;
    public const uint SQLITE_IOERR_READ = 266;
    public const uint SQLITE_IOERR_SHORT_READ = 522;
    public const uint SQLITE_IOERR_WRITE = 778;
    public const uint SQLITE_IOERR_FSYNC = 1034;
    public const uint SQLITE_IOERR_DIR_FSYNC = 1290;
    public const uint SQLITE_IOERR_TRUNCATE = 1546;
    public const uint SQLITE_IOERR_FSTAT = 1802;
    public const uint SQLITE_IOERR_UNLOCK = 2058;
    public const uint SQLITE_IOERR_RDLOCK = 2314;
    public const uint SQLITE_IOERR_DELETE = 2570;
    public const uint SQLITE_IOERR_BLOCKED = 2826;
    public const uint SQLITE_IOERR_NOMEM = 3082;
    public const uint SQLITE_IOERR_ACCESS = 3338;
    public const uint SQLITE_IOERR_CHECKRESERVEDLOCK = 3594;
    public const uint SQLITE_IOERR_LOCK = 3850;
    public const uint SQLITE_IOERR_CLOSE = 4106;
    public const uint SQLITE_IOERR_DIR_CLOSE = 4362;
    public const uint SQLITE_IOERR_SHMOPEN = 4618;
    public const uint SQLITE_IOERR_SHMSIZE = 4874;
    public const uint SQLITE_IOERR_SHMLOCK = 5130;
    public const uint SQLITE_IOERR_SHMMAP = 5386;
    public const uint SQLITE_IOERR_SEEK = 5642;
    public const uint SQLITE_IOERR_DELETE_NOENT = 5898;
    public const uint SQLITE_IOERR_MMAP = 6154;
    public const uint SQLITE_IOERR_GETTEMPPATH = 6410;
    public const uint SQLITE_LOCKED_SHAREDCACHE = 262;
    public const uint SQLITE_BUSY_RECOVERY = 261;
    public const uint SQLITE_BUSY_SNAPSHOT = 517;
    public const uint SQLITE_CANTOPEN_NOTEMPDIR = 270;
    public const uint SQLITE_CANTOPEN_ISDIR = 526;
    public const uint SQLITE_CANTOPEN_FULLPATH = 782;
    public const uint SQLITE_CORRUPT_VTAB = 267;
    public const uint SQLITE_READONLY_RECOVERY = 264;
    public const uint SQLITE_READONLY_CANTLOCK = 520;
    public const uint SQLITE_READONLY_ROLLBACK = 776;
    public const uint SQLITE_ABORT_ROLLBACK = 516;
    public const uint SQLITE_CONSTRAINT_CHECK = 275;
    public const uint SQLITE_CONSTRAINT_COMMITHOOK = 531;
    public const uint SQLITE_CONSTRAINT_FOREIGNKEY = 787;
    public const uint SQLITE_CONSTRAINT_FUNCTION = 1043;
    public const uint SQLITE_CONSTRAINT_NOTNULL = 1299;
    public const uint SQLITE_CONSTRAINT_PRIMARYKEY = 1555;
    public const uint SQLITE_CONSTRAINT_TRIGGER = 1811;
    public const uint SQLITE_CONSTRAINT_UNIQUE = 2067;
    public const uint SQLITE_CONSTRAINT_VTAB = 2323;
    public const uint SQLITE_NOTICE_RECOVER_WAL = 283;
    public const uint SQLITE_NOTICE_RECOVER_ROLLBACK = 539;
    public const uint SQLITE_WARNING_AUTOINDEX = 284;
    public const uint SQLITE_APP_BADLANGUAGE = 200;
    private ISqlite native;
    public string MaybeLastPreparedStatementString;
    public string MaybeLastBut1PreparedStatementString;
    public static Subject<string> ShellText = new Subject<string>();
    private static ISqliteShell shell;

    public Sqlite(
      string filename,
      SqliteOpenFlags flags,
      string vfs = null,
      SqliteSynchronizeOptions? syncMode = null,
      bool wal = true,
      int? busyTimeout = null)
    {
      filename = MediaStorage.GetAbsolutePath(filename);
      this.native = (ISqlite) NativeInterfaces.CreateInstance<WhatsAppNative.Sqlite>();
      try
      {
        this.native.Open(filename, flags, vfs ?? "");
        this.native.SetBusyTimeout(busyTimeout ?? (AppState.IsBackgroundAgent ? 500 : 3000));
        bool flag = (flags & SqliteOpenFlags.READWRITE) != 0;
        foreach (string sql in ((IEnumerable<string>) new string[3]
        {
          wal & flag ? "PRAGMA journal_mode=WAL" : (string) null,
          flag ? string.Format("PRAGMA synchronous={0}", (object) ((SqliteSynchronizeOptions) ((int) syncMode ?? (this.ShouldRelaxWrites ? 0 : 1))).ToString().ToUpperInvariant()) : (string) null,
          flag ? "PRAGMA secure_delete=ON" : (string) null
        }).Where<string>((Func<string, bool>) (s => s != null)))
        {
          using (Sqlite.PreparedStatement preparedStatement = this.PrepareStatement(sql))
            preparedStatement.Step();
        }
      }
      catch (Exception ex)
      {
        Exception e = ex;
        string msg = (string) null;
        Action action = e.GetRethrowAction();
        if (this.native != null)
          msg = this.native.GetError();
        if (msg != null)
          action = (Action) (() =>
          {
            throw new SqliteException("Could not open database; " + msg, e);
          });
        this.native = (ISqlite) null;
        action();
      }
    }

    public static uint HRForError(uint error) => 2684420096U | error;

    private bool ShouldRelaxWrites
    {
      get => !AppState.IsBackgroundAgent && AppState.BatteryPercentage >= 10;
    }

    private void CheckDisposed()
    {
      if (this.native == null)
        throw new ObjectDisposedException("Cannot access disposed object.");
    }

    public void Dispose()
    {
      this.CheckDisposed();
      this.native.Dispose();
      this.native = (ISqlite) null;
    }

    public void Interrupt()
    {
      this.CheckDisposed();
      this.native.Interrupt();
    }

    public void RegisterTokenizer()
    {
      this.CheckDisposed();
      this.native.RegisterTokenizer();
    }

    public bool IsTokenizerRegistered()
    {
      this.CheckDisposed();
      return this.native.IsTokenizerRegistered();
    }

    public int GetChangeCount()
    {
      this.CheckDisposed();
      return this.native.GetChangeCount();
    }

    public long GetLastRowId()
    {
      this.CheckDisposed();
      return this.native.GetLastRowId();
    }

    public IEnumerable<string> GetColumnNames(string tableName)
    {
      using (Sqlite.PreparedStatement stmt = this.PrepareStatement(string.Format("PRAGMA table_info({0})", (object) tableName)))
      {
        while (stmt.Step())
          yield return (string) stmt.Columns[1];
      }
    }

    private string GetError()
    {
      string error = (string) null;
      try
      {
        if (this.native != null)
          error = this.native.GetError();
      }
      catch (Exception ex)
      {
      }
      return error;
    }

    public void PrepareStatement(
      string sql,
      int sqlLength,
      out int sqlTailOffset,
      out Sqlite.PreparedStatement stmt)
    {
      this.MaybeLastBut1PreparedStatementString = this.MaybeLastPreparedStatementString;
      this.MaybeLastPreparedStatementString = sql;
      ISqlitePreparedStatement ReturnedObject = (ISqlitePreparedStatement) null;
      Func<string> func = (Func<string>) (() => string.Format("\nSQL = [{0}]\nError = [{1}]", (object) sql, (object) (this.GetError() ?? "(null)")));
      try
      {
        this.native.PrepareStatement(sql, sqlLength, out sqlTailOffset, out ReturnedObject);
      }
      catch (Exception ex)
      {
        throw new SqliteException(ex.GetFriendlyMessage() + func(), ex);
      }
      stmt = new Sqlite.PreparedStatement()
      {
        native = ReturnedObject
      };
    }

    public Sqlite.PreparedStatement PrepareStatement(string sql, Action dtor = null)
    {
      this.MaybeLastBut1PreparedStatementString = this.MaybeLastPreparedStatementString;
      this.MaybeLastPreparedStatementString = sql;
      Sqlite.PreparedStatement stmt = (Sqlite.PreparedStatement) null;
      int sqlTailOffset = 0;
      this.PrepareStatement(sql, sql.Length, out sqlTailOffset, out stmt);
      if (sqlTailOffset > sql.Length)
        throw new Exception("Unexpected multi-statement string");
      if (dtor != null)
        stmt.AddDisposable((IDisposable) new DisposableAction(dtor));
      return stmt;
    }

    public void BeginTransaction()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.PrepareStatement("BEGIN TRANSACTION"))
        preparedStatement.Step();
    }

    public void CommitTransaction()
    {
      string preparedStatementString1 = this.MaybeLastPreparedStatementString;
      string preparedStatementString2 = this.MaybeLastBut1PreparedStatementString;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.PrepareStatement("COMMIT TRANSACTION"))
          preparedStatement.Step();
      }
      catch (Exception ex)
      {
        Log.l("sqlite", "Commit exception:{0}, prev stmts:{1}, {2}.", (object) ex.GetHResult().ToString("x8"), (object) (preparedStatementString1 ?? "null"), (object) (preparedStatementString2 ?? "null"));
        if ((int) ex.GetHResult() == (int) Sqlite.HRForError(1U))
        {
          Log.SendCrashLog(ex, "sqlite error during commit", logOnlyForRelease: true);
          Log.l("sqlite integrity", "Running integrity check");
          using (Sqlite.PreparedStatement preparedStatement = this.PrepareStatement("PRAGMA integrity_check"))
          {
            while (preparedStatement.Step())
            {
              object[] columns = preparedStatement.Columns;
              string column = (columns != null ? (columns.Length >= 1 ? 1 : 0) : 0) != 0 ? preparedStatement.Columns[0] as string : (string) null;
              if (column != null)
                Log.l("sqlite integrity", column);
            }
          }
        }
        throw;
      }
    }

    public void RollbackTransaction(Exception innerException = null, Action onError = null)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.PrepareStatement("ROLLBACK TRANSACTION"))
      {
        try
        {
          preparedStatement.Step();
        }
        catch (Exception ex1)
        {
          Exception toLog = ex1;
          Action action = ex1.GetRethrowAction();
          List<Exception> exceptionList = new List<Exception>(3);
          if (innerException != null)
            exceptionList.Add(innerException);
          try
          {
            if (onError != null)
              onError();
          }
          catch (Exception ex2)
          {
            exceptionList.Add(ex2);
          }
          if (exceptionList.Count != 0)
          {
            exceptionList.Insert(0, ex1);
            toLog = (Exception) new AggregateException("database rollback failed", exceptionList.ToArray());
            action = (Action) (() =>
            {
              throw toLog;
            });
          }
          Log.SendCrashLog(toLog, "rollback");
          action();
        }
      }
    }

    public ISqliteBackup Backup(Sqlite targetDatabase, string srcDbName = null, string dstDbName = null)
    {
      this.CheckDisposed();
      targetDatabase.CheckDisposed();
      return this.native.InitializeBackup(srcDbName ?? "main", targetDatabase.native, dstDbName ?? "main");
    }

    public static ISqliteShell Shell
    {
      get
      {
        return Utils.LazyInit<ISqliteShell>(ref Sqlite.shell, (Func<ISqliteShell>) (() => (ISqliteShell) NativeInterfaces.CreateInstance<SqliteShell>()));
      }
    }

    public class PreparedStatement : IDisposable
    {
      internal ISqlitePreparedStatement native;
      private LinkedList<IDisposable> disposables = new LinkedList<IDisposable>();
      private IDisposable parentDisposeSubscription;
      private Dictionary<int, object> bindArgs = new Dictionary<int, object>();
      private bool debugSpewed;
      private string[] columnNames;
      private object[] columns;

      private string GetSql()
      {
        string sql = "(null)";
        if (this.native != null)
          sql = this.native.GetSql();
        return sql;
      }

      public string DebugString
      {
        get
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append(this.GetSql());
          stringBuilder.Append('\n');
          foreach (KeyValuePair<int, object> bindArg in this.bindArgs)
          {
            stringBuilder.Append(bindArg.Key);
            stringBuilder.Append(": ");
            stringBuilder.Append(bindArg.Value);
            stringBuilder.Append('\n');
          }
          stringBuilder.Remove(stringBuilder.Length - 1, 1);
          return stringBuilder.ToString();
        }
      }

      public string GetError()
      {
        string error = (string) null;
        try
        {
          if (this.native != null)
            error = this.native.GetError();
        }
        catch (Exception ex)
        {
        }
        return error;
      }

      public void Dispose()
      {
        if (this.native == null)
          throw new ObjectDisposedException("Cannot access disposed object.");
        this.native.Dispose();
        this.native = (ISqlitePreparedStatement) null;
        foreach (IDisposable disposable in this.disposables.AsRemoveSafeEnumerator<IDisposable>())
          disposable.Dispose();
        this.disposables.Clear();
      }

      public IDisposable AddDisposable(IDisposable d)
      {
        LinkedListNode<IDisposable> node = new LinkedListNode<IDisposable>(d);
        this.disposables.AddLast(node);
        return (IDisposable) new DisposableAction((Action) (() => this.disposables.Remove(node)));
      }

      public void Attach(SqliteDataContext db)
      {
        this.parentDisposeSubscription = db.AddDisposable((IDisposable) this);
      }

      public bool Detach()
      {
        bool flag = false;
        if (this.parentDisposeSubscription != null)
        {
          this.parentDisposeSubscription.Dispose();
          this.parentDisposeSubscription = (IDisposable) null;
          flag = true;
        }
        return flag;
      }

      public void Reset()
      {
        this.columns = (object[]) null;
        this.bindArgs.Clear();
        this.debugSpewed = false;
        this.native.Reset();
      }

      public bool Step()
      {
        if (!this.debugSpewed && Settings.settingsInstance != null && Settings.ChatID == "12485554321")
        {
          Log.WriteLineDebug(this.DebugString);
          this.debugSpewed = true;
        }
        this.columns = (object[]) null;
        try
        {
          return this.native.Step();
        }
        catch (Exception ex)
        {
          string message = "sqlite3_step returned error 0x" + ex.GetHResult().ToString("x") + "\n" + this.DebugString;
          string error = this.GetError();
          if (error != null)
            message = message + "\n" + error;
          if (this.Detach())
            this.Dispose();
          throw new SqliteException(message, ex);
        }
      }

      public void BindObject(int idx, object o, bool sensitive = false)
      {
        this.bindArgs[idx] = sensitive ? (object) "(stripped)" : o;
        this.native.Bind(idx, o);
      }

      public void Bind(int idx) => this.BindObject(idx, (object) null);

      public void Bind(int idx, int val, bool sensitive = false)
      {
        this.BindObject(idx, (object) val, sensitive);
      }

      public void Bind(int idx, long val, bool sensitive = false)
      {
        this.BindObject(idx, (object) val, sensitive);
      }

      public void Bind(int idx, double val, bool sensitive = false)
      {
        this.BindObject(idx, (object) val, sensitive);
      }

      public void Bind(int idx, string val, bool sensitive = false)
      {
        this.BindObject(idx, (object) val, sensitive);
      }

      public void Bind(int idx, byte[] val, bool sensitive = false)
      {
        this.BindObject(idx, (object) val, sensitive);
      }

      public void Bind(int idx, bool val, bool sensitive = false)
      {
        int val1 = val ? 1 : 0;
        this.Bind(idx, val1, sensitive);
      }

      public int Count => this.native.GetCount();

      public string[] ColumnNames
      {
        get
        {
          if (this.columnNames == null)
          {
            string[] strArray = new string[this.Count];
            for (int Col = 0; Col < strArray.Length; ++Col)
              strArray[Col] = this.native.GetColumnName(Col);
            this.columnNames = strArray;
          }
          return this.columnNames;
        }
      }

      public object[] Columns
      {
        get
        {
          if (this.columns == null)
          {
            object[] objArray = new object[this.Count];
            for (int Col = 0; Col < objArray.Length; ++Col)
              objArray[Col] = this.native.GetColumn(Col);
            this.columns = objArray;
          }
          return this.columns;
        }
      }
    }

    private class OutputCallbackWrapper : ISqliteShellOutputCallback
    {
      public Action<string> OnString;
      private MemoryStream mem = new MemoryStream();
      private byte[] prompt = Encoding.UTF8.GetBytes("sqlite> ");

      private bool IsNl(byte b) => b == (byte) 10 || b == (byte) 13;

      private void FlushBuffer(byte[] b, int offset, int length)
      {
        if (length == 0)
          return;
        try
        {
          Action<string> onString = this.OnString;
          if (onString == null)
            return;
          onString(Encoding.UTF8.GetString(b, offset, length));
        }
        catch (Exception ex)
        {
        }
      }

      private void OnDataAvailable(byte[] buffer, int offset, int length)
      {
        for (int index = offset; index - offset < length; ++index)
        {
          if (this.IsNl(buffer[index]))
          {
            if (this.mem.Length != 0L)
            {
              this.mem.Write(buffer, offset, index + 1 - offset);
              this.FlushBuffer(this.mem.GetBuffer(), 0, (int) this.mem.Length);
              this.mem.SetLength(0L);
            }
            else
              this.FlushBuffer(buffer, offset, index + 1 - offset);
            offset += index + 1;
            length -= index + 1;
          }
        }
        if (length != 0)
          this.mem.Write(buffer, offset, length);
        if (this.mem.Length != (long) this.prompt.Length)
          return;
        byte[] buffer1 = this.mem.GetBuffer();
        bool flag = true;
        for (int index = 0; index < this.prompt.Length; ++index)
        {
          if ((int) buffer1[index] != (int) this.prompt[index])
          {
            flag = false;
            break;
          }
        }
        if (!flag)
          return;
        this.FlushBuffer(this.prompt, 0, this.prompt.Length);
        this.mem.SetLength(0L);
      }

      public void OnDataAvailable(byte[] buffer) => this.OnDataAvailable(buffer, 0, buffer.Length);
    }
  }
}
