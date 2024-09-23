// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteDataContext
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Text;
using WhatsApp.WaCollections;
using WhatsAppNative;


namespace WhatsApp
{
  public class SqliteDataContext : IDisposable
  {
    private Sqlite db;
    public const int SQLITE_MAX_VARIABLE_NUMBER = 999;
    public Func<bool> IsLockHeld;
    private string filename;
    private SqliteOpenFlags flags;
    private SqliteSynchronizeOptions? sync;
    protected List<SqliteDataContext.TableState> dirtyTables = new List<SqliteDataContext.TableState>();
    private int DbResetCount;
    private Dictionary<string, SqliteDataContext.TableState> tables = new Dictionary<string, SqliteDataContext.TableState>();
    private LinkedList<IDisposable> Disposables = new LinkedList<IDisposable>();
    protected int LatestSchemaVersion;

    protected Sqlite Db
    {
      get
      {
        this.OpenDb();
        return this.db;
      }
    }

    public SqliteDataContext(
      string filename,
      SqliteOpenFlags flags = SqliteOpenFlags.Defaults,
      SqliteSynchronizeOptions? sync = null)
    {
      this.filename = filename;
      this.flags = flags;
      this.sync = sync;
      this.OpenDb();
      this.tables = ((IEnumerable<MemberInfo>) this.GetType().GetMembers()).Where<MemberInfo>((Func<MemberInfo, bool>) (mi => mi.MemberType == MemberTypes.Property)).Cast<PropertyInfo>().Where<PropertyInfo>((Func<PropertyInfo, bool>) (pi => pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof (Table<>))).Select<PropertyInfo, SqliteDataContext.TableState>((Func<PropertyInfo, SqliteDataContext.TableState>) (pi => new SqliteDataContext.TableState()
      {
        Context = this,
        Name = pi.Name,
        ObjectType = ((IEnumerable<System.Type>) pi.PropertyType.GetGenericArguments()).First<System.Type>(),
        IndexAttributes = ((IEnumerable<System.Type>) pi.PropertyType.GetGenericArguments()).First<System.Type>().GetCustomAttributes(typeof (IndexAttribute), false).Cast<IndexAttribute>(),
        DirtyTables = this.dirtyTables
      })).ToDictionary<SqliteDataContext.TableState, string>((Func<SqliteDataContext.TableState, string>) (t => t.Name));
      foreach (SqliteDataContext.TableState tableState in this.tables.Values)
        tableState.ParseColumnState();
    }

    private void OpenDb()
    {
      if (this.db != null)
        return;
      this.db = new Sqlite(string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) this.filename), this.flags, syncMode: this.sync);
      this.OnDbOpened(this.db);
      Log.l(nameof (OpenDb), "opened {0}", (object) this.filename);
    }

    protected virtual void OnDbOpened(Sqlite db)
    {
    }

    public void ReopenDb()
    {
      this.DisposeDbHandle();
      this.OpenDb();
    }

    protected void PurgeCache(string tableName)
    {
      SqliteDataContext.TableState tableState = (SqliteDataContext.TableState) null;
      if (!this.tables.TryGetValue(tableName, out tableState) || tableState == null)
        return;
      tableState.DetachAll();
    }

    protected void PurgeCache(string tableName, IEnumerable<long> ids)
    {
      SqliteDataContext.TableState table = this.tables[tableName];
      foreach (long id in ids)
        table.DetachObject((object) id);
    }

    protected void PurgeCacheAndEmpty<T>(string tableName, Func<T, bool> func)
    {
      this.tables[tableName].DetachObject<T>(func, true);
    }

    protected void PurgeCache<T>(string tableName, Func<T, bool> func)
    {
      this.tables[tableName].DetachObject<T>(func);
    }

    protected IEnumerable<T> GetCachedObjects<T>(string tableName) where T : class
    {
      return this.tables[tableName].GetCachedObjects<T>();
    }

    protected SqliteDataContext.TableState GetTable(string tableName) => this.tables[tableName];

    public int? GetCacheVersion(string tableName, object primaryKey)
    {
      int? cacheVersion = new int?();
      SqliteDataContext.TableState tableState = (SqliteDataContext.TableState) null;
      SqliteDataContext.ObjectState objectState = (SqliteDataContext.ObjectState) null;
      if (primaryKey.GetType() == typeof (int))
        primaryKey = (object) (long) (int) primaryKey;
      if (this.tables.TryGetValue(tableName, out tableState) && tableState != null && tableState.GetObject(primaryKey, out objectState) && objectState != null)
        cacheVersion = new int?(objectState.DbResetCount);
      return cacheVersion;
    }

    public int GetCacheVersion() => this.DbResetCount;

    public void Dispose()
    {
      this.filename = (string) null;
      this.tables.Clear();
      this.DisposeDbHandle();
    }

    protected void DisposeChildren()
    {
      foreach (IDisposable disposable in this.Disposables.AsRemoveSafeEnumerator<IDisposable>())
        disposable.Dispose();
      this.Disposables.Clear();
    }

    protected void DisposeDbHandle()
    {
      this.DisposeChildren();
      if (this.db != null)
      {
        this.db.Dispose();
        this.db = (Sqlite) null;
      }
      ++this.DbResetCount;
    }

    public IDisposable AddDisposable(IDisposable d)
    {
      LinkedListNode<IDisposable> node = new LinkedListNode<IDisposable>(d);
      this.Disposables.AddLast(node);
      return (IDisposable) new DisposableAction((Action) (() => this.Disposables.Remove(node)));
    }

    public void CreateDatabase()
    {
      this.BeginTransaction();
      try
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("CREATE TABLE metadata(version int);\nINSERT INTO metadata VALUES ({0});\n", (object) this.LatestSchemaVersion);
        foreach (SqliteDataContext.TableState t in this.tables.Values)
          this.CreateTableSql(t, sb);
        string sql = sb.ToString();
        sb.Clear();
        this.ExecBatch(sql);
        this.CreateTableOverride();
        this.CommitTransaction();
      }
      catch (Exception ex)
      {
        this.RollbackTransaction(ex);
        throw;
      }
    }

    protected virtual void CreateTableOverride()
    {
    }

    private void CreateTableSql(SqliteDataContext.TableState t, StringBuilder sb)
    {
      int idxno = 0;
      sb.AppendFormat("CREATE TABLE {0}\n(\n", (object) t.Name);
      int num = 0;
      foreach (SqliteDataContext.ColumnState c in t.Columns.Values)
      {
        if (num != 0)
          sb.Append(",\n");
        this.CreateColumnDef(c, sb);
        ++num;
      }
      sb.AppendFormat("\n);\n");
      foreach (IndexAttribute indexAttribute in t.IndexAttributes)
        this.CreateIndex(sb, indexAttribute, t.Name, ref idxno);
    }

    private void CreateIndex(StringBuilder sb, IndexAttribute idx, string table, ref int idxno)
    {
      sb.AppendFormat("CREATE {0} INDEX {1}\nON {2} ({3});\n", idx.IsUnique ? (object) "UNIQUE" : (object) "", (object) (idx.Name ?? string.Format("{0}_idx{1}", (object) table, (object) idxno++)), (object) table, (object) idx.Columns);
    }

    private void CreateColumnDef(SqliteDataContext.ColumnState c, StringBuilder sb)
    {
      sb.AppendFormat("   {0} {1}", (object) c.DbName, (object) this.GetDbType(c));
      if (!c.ColumnAttribute.IsPrimaryKey)
        return;
      sb.Append(" PRIMARY KEY");
    }

    protected int GetSchemaVersion()
    {
      int schemaVersion = 0;
      if (this.DatabaseExists())
      {
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT version FROM metadata"))
        {
          preparedStatement.Step();
          schemaVersion = (int) (long) preparedStatement.Columns[0];
        }
      }
      return schemaVersion;
    }

    protected Set<string> GetTableMetadata(string tableName, string type)
    {
      Set<string> tableMetadata = new Set<string>();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT name FROM sqlite_master WHERE tbl_name = ? AND type = ?"))
      {
        preparedStatement.Bind(0, tableName);
        preparedStatement.Bind(1, type);
        while (preparedStatement.Step())
          tableMetadata.Add((string) preparedStatement.Columns[0]);
      }
      return tableMetadata;
    }

    protected void AddTable(string tableName)
    {
      SqliteDataContext.TableState t = (SqliteDataContext.TableState) null;
      if (!this.tables.TryGetValue(tableName, out t))
        return;
      StringBuilder sb = new StringBuilder();
      this.CreateTableSql(t, sb);
      this.ExecBatch(sb.ToString());
      t.TableWasAdded = true;
    }

    protected void AddColumn(string tableName, string columnName)
    {
      SqliteDataContext.TableState tableState = (SqliteDataContext.TableState) null;
      if (!this.tables.TryGetValue(tableName, out tableState) || tableState.TableWasAdded)
        return;
      if (tableState.DbHasColumn == null)
      {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        foreach (string columnName1 in this.Db.GetColumnNames(tableName))
          dictionary[columnName1] = true;
        tableState.DbHasColumn = dictionary;
      }
      if (tableState.DbHasColumn.ContainsKey(columnName))
        return;
      SqliteDataContext.ColumnState column = tableState.Columns[columnName];
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("ALTER TABLE {0}\nADD COLUMN", (object) tableName);
      this.CreateColumnDef(column, sb);
      this.ExecBatch(sb.ToString());
      tableState.DbHasColumn.Add(columnName, true);
    }

    protected void AddIndex(string tableName, string indexName)
    {
      SqliteDataContext.TableState tableState = (SqliteDataContext.TableState) null;
      if (!this.tables.TryGetValue(tableName, out tableState) || tableState.TableWasAdded)
        return;
      if (tableState.DbHasIndex == null)
      {
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        using (Sqlite.PreparedStatement preparedStatement = this.Db.PrepareStatement("SELECT name FROM sqlite_master WHERE type = ? AND tbl_name = ?"))
        {
          preparedStatement.Bind(0, "index");
          preparedStatement.Bind(1, tableName);
          while (preparedStatement.Step())
            dictionary.Add((string) preparedStatement.Columns[0], true);
        }
        tableState.DbHasIndex = dictionary;
      }
      if (tableState.DbHasIndex.ContainsKey(indexName))
        return;
      IndexAttribute idx = tableState.IndexAttributes.Where<IndexAttribute>((Func<IndexAttribute, bool>) (i => i.Name == indexName)).FirstOrDefault<IndexAttribute>();
      if (idx == null)
        throw new Exception("No such index " + indexName);
      StringBuilder sb = new StringBuilder();
      int idxno = 0;
      this.CreateIndex(sb, idx, tableName, ref idxno);
      this.ExecBatch(sb.ToString());
      tableState.DbHasIndex.Add(indexName, true);
    }

    protected void SetSchemaVersion(int version)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE metadata SET version = ?"))
      {
        preparedStatement.Bind(0, version, false);
        preparedStatement.Step();
      }
    }

    public bool DatabaseExists()
    {
      string sql = "select count(tbl_name) from sqlite_master where type = 'table' and tbl_name = 'metadata'";
      Sqlite.PreparedStatement stmt;
      this.Db.PrepareStatement(sql, sql.Length, out int _, out stmt);
      using (stmt)
      {
        stmt.Step();
        return (long) stmt.Columns[0] != 0L;
      }
    }

    protected bool TableExists(string tableName)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT COUNT(tbl_name) FROM sqlite_master WHERE tbl_name = ? AND type = 'table'"))
      {
        preparedStatement.Bind(0, tableName);
        preparedStatement.Step();
        return (long) preparedStatement.Columns[0] != 0L;
      }
    }

    public void DeleteDatabase()
    {
      string filename = this.filename;
      this.Dispose();
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        storeForApplication.DeleteFile(filename);
    }

    public int GetChangeCount() => this.db != null ? this.db.GetChangeCount() : -1;

    public virtual void SubmitChanges() => this.SubmitChanges(true);

    public virtual SqliteDataContext.ChangeSet GetChangeSet()
    {
      SqliteDataContext.ChangeSet changeSet = new SqliteDataContext.ChangeSet();
      foreach (SqliteDataContext.TableState dirtyTable in this.dirtyTables)
      {
        foreach (SqliteDataContext.ObjectState dirtyObject in dirtyTable.DirtyObjects)
        {
          if (dirtyObject.New)
            changeSet.Inserts.Add(dirtyObject.GetObject());
          else if (dirtyObject.DeletePending)
            changeSet.Deletes.Add(dirtyObject.GetObject());
          else
            changeSet.Updates.Add(new Pair<object, string[]>(dirtyObject.GetObject(), dirtyObject.DirtyColumns.Keys.ToArray<string>()));
        }
      }
      return changeSet;
    }

    protected void BeginTransaction() => this.Db.BeginTransaction();

    protected void CommitTransaction() => this.Db.CommitTransaction();

    protected void RollbackTransaction(Exception innerException = null)
    {
      try
      {
        if (this.db == null)
        {
          Log.l("rollback", "warning: tried to roll back non-open db");
          return;
        }
        this.Db.RollbackTransaction(innerException, new Action(this.ReopenDb));
      }
      catch (Exception ex)
      {
      }
      foreach (SqliteDataContext.TableState dirtyTable in this.dirtyTables)
      {
        dirtyTable.Dirty = false;
        foreach (SqliteDataContext.ObjectState dirtyObject in dirtyTable.DirtyObjects)
        {
          if (dirtyObject.New && dirtyTable.PrimaryKey != null)
          {
            object key = dirtyTable.PrimaryKey.GetValue(dirtyObject.GetObject(), (object[]) null);
            if (key.GetType() == typeof (int))
              key = (object) (long) (int) key;
            dirtyTable.DetachObject(key);
          }
          dirtyObject.Dirty = false;
          dirtyObject.ObjectStrongRef = (object) null;
        }
        dirtyTable.DirtyObjects.Clear();
      }
      this.dirtyTables.Clear();
    }

    public void SubmitChanges(bool tx)
    {
      StringBuilder stringBuilder = new StringBuilder();
      List<Action> actionList = new List<Action>();
      Sqlite.PreparedStatement preparedStatement1 = (Sqlite.PreparedStatement) null;
      bool inTx = false;
      Action action = (Action) (() =>
      {
        if (!tx || inTx)
          return;
        this.BeginTransaction();
        inTx = true;
      });
      try
      {
        foreach (SqliteDataContext.TableState dirtyTable in this.dirtyTables)
        {
          foreach (SqliteDataContext.ObjectState dirtyObject in dirtyTable.DirtyObjects)
          {
            SqliteDataContext.ObjectState o = dirtyObject;
            if (o.ObjectStrongRef == null)
              Log.WriteLineDebug("StrongRef is null!");
            if (o.New)
            {
              bool flag = o.Table.PrimaryKey != null;
              int num1 = 0;
              if (o.Table.InsertStmt == null)
              {
                stringBuilder.Clear();
                int num2 = 0;
                stringBuilder.AppendFormat("INSERT INTO {0} (", (object) o.Table.Name);
                foreach (KeyValuePair<string, SqliteDataContext.ColumnState> keyValuePair in o.Table.ColumnsByDbName)
                {
                  if (!flag || !(keyValuePair.Key == o.Table.PrimaryKeyDBName))
                  {
                    if (num2++ != 0)
                      stringBuilder.Append(", ");
                    stringBuilder.Append(keyValuePair.Key);
                  }
                }
                stringBuilder.Append(")\nVALUES (");
                for (int index = 0; index < num2; ++index)
                {
                  if (index != 0)
                    stringBuilder.Append(", ");
                  stringBuilder.Append('?');
                }
                stringBuilder.Append(')');
                SqliteDataContext.TableState table = o.Table;
                o.Table.InsertStmt = this.Db.PrepareStatement(stringBuilder.ToString(), (Action) (() => table.InsertStmt = (Sqlite.PreparedStatement) null));
                o.Table.InsertStmt.Attach(this);
              }
              else
                o.Table.InsertStmt.Reset();
              foreach (KeyValuePair<string, SqliteDataContext.ColumnState> keyValuePair in o.Table.ColumnsByDbName)
              {
                if (!flag || !(keyValuePair.Key == o.Table.PrimaryKeyDBName))
                  o.Table.InsertStmt.BindObject(num1++, this.GetValue(o.GetObject(), keyValuePair.Value), keyValuePair.Value.Sensitive);
              }
              action();
              o.Table.InsertStmt.Step();
              if (flag)
              {
                long rowId = this.Db.GetLastRowId();
                this.SetValue(o, o.Table.PrimaryKey, (object) rowId);
                o.Table.AttachObject((object) rowId, o);
                o.TrackingChanges = true;
                actionList.Add((Action) (() =>
                {
                  o.TrackingChanges = false;
                  o.Table.DetachObject((object) rowId);
                  this.SetValue(o, o.Table.PrimaryKey, (object) 0);
                }));
              }
            }
            else if (o.DeletePending)
            {
              SqliteDataContext.TableState table = o.Table;
              if (table.DeleteStmt == null)
              {
                table.DeleteStmt = this.db.PrepareStatement(string.Format("DELETE FROM {0} WHERE {1}=?", (object) o.Table.Name, (object) o.Table.PrimaryKeyDBName), (Action) (() => table.DeleteStmt = (Sqlite.PreparedStatement) null));
                table.DeleteStmt.Attach(this);
              }
              else
                table.DeleteStmt.Reset();
              object primaryKey = this.GetValue(o.GetObject(), o.Table.PrimaryKey);
              if (primaryKey.GetType() == typeof (int))
                primaryKey = (object) (long) (int) primaryKey;
              table.DeleteStmt.BindObject(0, primaryKey);
              action();
              table.DeleteStmt.Step();
              table.DetachObject(primaryKey);
              actionList.Add((Action) (() => table.AttachObject(primaryKey, o)));
            }
            else if (o.DirtyColumns.Count != 0)
            {
              bool flag = true;
              stringBuilder.Clear();
              stringBuilder.AppendFormat("UPDATE {0}\nSET ", (object) o.Table.Name);
              List<object> objectList = new List<object>();
              foreach (SqliteDataContext.ColumnState col in o.DirtyColumns.Select<KeyValuePair<string, string>, SqliteDataContext.ColumnState>((Func<KeyValuePair<string, string>, SqliteDataContext.ColumnState>) (kv => o.Table.Columns[kv.Key])))
              {
                if (!flag)
                  stringBuilder.Append(", ");
                else
                  flag = false;
                stringBuilder.AppendFormat("{0}=?", (object) col.DbName);
                objectList.Add(this.GetValue(o.GetObject(), col));
              }
              stringBuilder.AppendFormat("\nWHERE {0} = ?", (object) o.Table.PrimaryKeyDBName);
              objectList.Add(this.GetValue(o.GetObject(), o.Table.PrimaryKey));
              using (Sqlite.PreparedStatement preparedStatement2 = this.Db.PrepareStatement(stringBuilder.ToString()))
              {
                int num = 0;
                for (int count = objectList.Count; num < count; ++num)
                  preparedStatement2.BindObject(num, objectList[num]);
                action();
                preparedStatement2.Step();
              }
            }
          }
        }
        if (preparedStatement1 != null)
          return;
        if (inTx)
        {
          this.CommitTransaction();
          inTx = false;
        }
      }
      catch (Exception ex)
      {
        if (inTx)
        {
          try
          {
            actionList.Reverse();
            actionList.ForEach((Action<Action>) (a => a()));
          }
          finally
          {
            this.RollbackTransaction(ex);
          }
        }
        throw;
      }
      foreach (SqliteDataContext.TableState dirtyTable in this.dirtyTables)
      {
        foreach (SqliteDataContext.ObjectState dirtyObject in dirtyTable.DirtyObjects)
        {
          dirtyObject.Dirty = false;
          dirtyObject.ObjectStrongRef = (object) null;
          dirtyObject.New = false;
          dirtyObject.DirtyColumns.Clear();
        }
        dirtyTable.Dirty = false;
        dirtyTable.DirtyObjects.Clear();
      }
      this.dirtyTables.Clear();
    }

    public void Insert(string tableName, object o)
    {
      if (o == null)
      {
        Log.l("db", "Attempt to add null object to {0}", (object) tableName);
        throw new NullReferenceException("Null object inserted to db");
      }
      SqliteDataContext.TableState table = this.tables[tableName];
      SqliteDataContext.ObjectState objectState = new SqliteDataContext.ObjectState()
      {
        New = true,
        Dirty = true,
        ObjectStrongRef = o,
        ObjectWeakRef = new WeakReference(o),
        Table = table,
        TrackingChanges = true,
        DbResetCount = this.DbResetCount
      };
      table.OnObjectDirty(objectState);
    }

    public void Delete(string tableName, object o)
    {
      if (o == null)
        throw new NullReferenceException("Null object deleted from db");
      SqliteDataContext.TableState table = this.tables[tableName];
      object key = table.PrimaryKey.GetValue(o, (object[]) null);
      SqliteDataContext.ObjectState objectState = (SqliteDataContext.ObjectState) null;
      if (key.GetType() == typeof (int))
        key = (object) (long) (int) key;
      if (!table.GetObject(key, out objectState))
        return;
      objectState.DeletePending = true;
      if (objectState.Dirty)
        return;
      table.OnObjectDirty(objectState);
      objectState.Dirty = true;
    }

    public void ClearValues(string tableName, object o, string[] retainColumns)
    {
      SqliteDataContext.TableState table = this.tables[tableName];
      foreach (SqliteDataContext.ColumnState columnState in table.Columns.Values)
      {
        if (!((IEnumerable<string>) retainColumns).Contains<string>(columnState.DbName) && columnState.PropertyInfo != table.PrimaryKey)
        {
          if (columnState.PropertyInfo.PropertyType.IsPrimitive)
          {
            if (columnState.PropertyInfo.PropertyType == typeof (bool))
              columnState.PropertyInfo.SetValue(o, (object) false);
            else
              columnState.PropertyInfo.SetValue(o, (object) 0);
          }
          else
            columnState.PropertyInfo.SetValue(o, (object) null);
        }
      }
    }

    private IEnumerable<KeyValuePair<string, object>> ParseRow(Sqlite.PreparedStatement stmt)
    {
      int count = stmt.Count;
      KeyValuePair<string, object>[] row = new KeyValuePair<string, object>[count];
      for (int index = 0; index < count; ++index)
        row[index] = new KeyValuePair<string, object>(stmt.ColumnNames[index], stmt.Columns[index]);
      return (IEnumerable<KeyValuePair<string, object>>) row;
    }

    public IEnumerable<IEnumerable<KeyValuePair<string, object>>> ParseRows(
      Sqlite.PreparedStatement stmt)
    {
      while (stmt.Step())
        yield return this.ParseRow(stmt);
      stmt.Reset();
    }

    public T ParseTableRow<T>(
      Sqlite.PreparedStatement stmt,
      string tableName,
      int colStart = 0,
      int colsCount = -1)
    {
      SqliteDataContext.TableState table = this.tables[tableName];
      return this.ParseTableRow<T>(stmt, table, colStart, colsCount);
    }

    public T ParseTableRow<T>(
      Sqlite.PreparedStatement stmt,
      SqliteDataContext.TableState table,
      int colStart = 0,
      int colsCount = -1)
    {
      object[] source1 = stmt.Columns;
      string[] source2 = stmt.ColumnNames;
      if (colStart > 0)
      {
        source2 = ((IEnumerable<string>) source2).Skip<string>(colStart).ToArray<string>();
        source1 = ((IEnumerable<object>) source1).Skip<object>(colStart).ToArray<object>();
      }
      if (colsCount > 0)
      {
        source2 = ((IEnumerable<string>) source2).Take<string>(colsCount).ToArray<string>();
        source1 = ((IEnumerable<object>) source1).Take<object>(colsCount).ToArray<object>();
      }
      SqliteDataContext.ColumnState[] columnStateArray = new SqliteDataContext.ColumnState[source2.Length];
      int index1 = 0;
      bool flag = false;
      for (int index2 = 0; index2 < source2.Length; ++index2)
      {
        string key = source2[index2];
        SqliteDataContext.ColumnState columnState;
        if (table.ColumnsByDbName.TryGetValue(key, out columnState))
        {
          columnStateArray[index2] = columnState;
          if (!flag && key == table.PrimaryKeyDBName)
          {
            index1 = index2;
            flag = true;
          }
        }
      }
      object tableRow = (object) null;
      SqliteDataContext.ObjectState os = (SqliteDataContext.ObjectState) null;
      object key1 = source1[index1];
      if (key1 == null)
        return (T) tableRow;
      if (table.GetObject(key1, out os) && os.ObjectStrongRef == null && (os.ObjectStrongRef = os.ObjectWeakRef.Target) == null)
      {
        table.DetachObject(key1);
        os = (SqliteDataContext.ObjectState) null;
      }
      if (os != null)
      {
        if (os.DbResetCount == this.DbResetCount)
          return (T) this.GetResult(os);
        os.DbResetCount = this.DbResetCount;
        os.TrackingChanges = false;
      }
      else
      {
        object instance = Activator.CreateInstance(table.ObjectType);
        os = new SqliteDataContext.ObjectState()
        {
          TrackingChanges = false,
          ObjectStrongRef = instance,
          ObjectWeakRef = new WeakReference(instance),
          DbResetCount = this.DbResetCount
        };
        this.SetValue(os, table.PrimaryKey, key1);
        table.AttachObject(key1, os);
      }
      int length = source1.Length;
      for (int index3 = 0; index3 < length; ++index3)
      {
        SqliteDataContext.ColumnState prop = columnStateArray[index3];
        if (prop.PropertyInfo != null)
          this.SetValue(os, prop, source1[index3]);
      }
      return (T) this.GetResult(os);
    }

    public IEnumerable<object[]> ParseTable(Sqlite.PreparedStatement stmt, string[] tableNames)
    {
      SqliteDataContext.TableState[] tables = new SqliteDataContext.TableState[tableNames.Length];
      for (int index = 0; index < tables.Length; ++index)
        tables[index] = this.tables[tableNames[index]];
      if (stmt.Step())
      {
        string[] columnNames = stmt.ColumnNames;
        int[] tableStart = new int[tables.Length];
        SqliteDataContext.ColumnState[] cols = new SqliteDataContext.ColumnState[columnNames.Length];
        int[] primaryKeys = new int[tables.Length];
        bool flag = false;
        int index1 = 0;
        SqliteDataContext.TableState tableState1 = tables[index1];
        for (int index2 = 0; index2 < columnNames.Length; ++index2)
        {
          string key = columnNames[index2];
          if (key == "'#'")
          {
            ++index1;
            tableStart[index1] = index2 + 1;
            tableState1 = tables[index1];
            flag = false;
          }
          else
          {
            SqliteDataContext.ColumnState columnState;
            if (tableState1.ColumnsByDbName.TryGetValue(key, out columnState))
            {
              cols[index2] = columnState;
              if (!flag && key == tableState1.PrimaryKeyDBName)
              {
                primaryKeys[index1] = index2;
                flag = true;
              }
            }
          }
        }
        object[] r = new object[tables.Length];
        do
        {
          object[] columns = stmt.Columns;
          for (int index3 = 0; index3 < tables.Length; ++index3)
          {
            SqliteDataContext.TableState tableState2 = tables[index3];
            SqliteDataContext.ObjectState os = (SqliteDataContext.ObjectState) null;
            object key = columns[primaryKeys[index3]];
            if (key == null)
            {
              r[index3] = (object) null;
            }
            else
            {
              if (tableState2.GetObject(key, out os) && os.ObjectStrongRef == null && (os.ObjectStrongRef = os.ObjectWeakRef.Target) == null)
              {
                tableState2.DetachObject(key);
                os = (SqliteDataContext.ObjectState) null;
              }
              if (os != null)
              {
                if (os.DbResetCount == this.DbResetCount)
                {
                  r[index3] = this.GetResult(os);
                  continue;
                }
                os.DbResetCount = this.DbResetCount;
                os.TrackingChanges = false;
              }
              else
              {
                object instance = Activator.CreateInstance(tableState2.ObjectType);
                os = new SqliteDataContext.ObjectState()
                {
                  TrackingChanges = false,
                  ObjectStrongRef = instance,
                  ObjectWeakRef = new WeakReference(instance),
                  DbResetCount = this.DbResetCount
                };
                this.SetValue(os, tableState2.PrimaryKey, key);
                tableState2.AttachObject(key, os);
              }
              int num1 = tableStart[index3];
              int num2 = columns.Length;
              if (index3 + 1 < tableStart.Length)
                num2 = tableStart[index3 + 1] - 1;
              for (int index4 = num1; index4 < num2; ++index4)
              {
                SqliteDataContext.ColumnState prop = cols[index4];
                if (prop.PropertyInfo != null)
                  this.SetValue(os, prop, columns[index4]);
              }
              r[index3] = this.GetResult(os);
            }
          }
          yield return r;
        }
        while (stmt.Step());
        tableStart = (int[]) null;
        cols = (SqliteDataContext.ColumnState[]) null;
        primaryKeys = (int[]) null;
        r = (object[]) null;
      }
      stmt.Reset();
    }

    private object GetResult(SqliteDataContext.ObjectState os)
    {
      os.TrackingChanges = true;
      object result = os.GetObject();
      if (os.Dirty)
        return result;
      os.ObjectStrongRef = (object) null;
      return result;
    }

    public IEnumerable<T> ParseTable<T>(Sqlite.PreparedStatement stmt, string tableName)
    {
      return this.ParseTable(stmt, new string[1]
      {
        tableName
      }).Select<object[], T>((Func<object[], T>) (res => (T) res[0]));
    }

    public T ParseTableFirstOrDefault<T>(Sqlite.PreparedStatement stmt, string tableName)
    {
      T tableFirstOrDefault = this.ParseTable<T>(stmt, tableName).FirstOrDefault<T>();
      stmt.Reset();
      return tableFirstOrDefault;
    }

    public IEnumerable<KeyValuePair<T1, T2>> ParseJoin<T1, T2>(
      Sqlite.PreparedStatement stmt,
      string firstTableName,
      string secondTableName)
    {
      return this.ParseTable(stmt, new string[2]
      {
        firstTableName,
        secondTableName
      }).Select<object[], KeyValuePair<T1, T2>>((Func<object[], KeyValuePair<T1, T2>>) (res => new KeyValuePair<T1, T2>((T1) res[0], (T2) res[1])));
    }

    protected static void ConvertOffsetEncoding(string text, Pair<int, int>[] offsets)
    {
      if (string.IsNullOrEmpty(text) || !((IEnumerable<Pair<int, int>>) offsets).Any<Pair<int, int>>())
        return;
      char[] charArray = text.ToCharArray();
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      int key = 0;
      int num1 = 0;
      for (int index = 0; index < charArray.Length; ++index)
      {
        dictionary.Add(key, index - num1);
        if (Utils.IsLeadingSurrogate(charArray[index]) && index < charArray.Length - 1 && Utils.IsTrailingSurrogate(charArray[index + 1]))
        {
          key += Encoding.UTF8.GetByteCount(charArray, index, 2);
          ++index;
        }
        else if (Emoji.IsInvalidEmojiAndVariationPair(text, index))
        {
          ++num1;
          key += Encoding.UTF8.GetByteCount(charArray, index, 1);
        }
        else
          key += Encoding.UTF8.GetByteCount(charArray, index, 1);
      }
      if (charArray.Length != 0)
        dictionary.Add(key, charArray.Length);
      for (int index = 0; index < offsets.Length; ++index)
      {
        int first = offsets[index].First;
        int second = offsets[index].Second;
        int num2 = offsets[index].First = dictionary[first];
        int num3 = offsets[index].Second = dictionary[first + second] - offsets[index].First;
        if (num3 > 1 && Emoji.IsInvalidEmojiAndVariationPair(text, num2 + num3 - 1))
          --offsets[index].Second;
      }
    }

    private void SetValue(
      SqliteDataContext.ObjectState obj,
      SqliteDataContext.ColumnState prop,
      object value)
    {
      this.SetValue(obj, prop.PropertyInfo, value);
    }

    private void SetValue(SqliteDataContext.ObjectState obj, PropertyInfo prop, object value)
    {
      System.Type enumType = SqliteDataContext.NormalizeNullable(prop.PropertyType);
      if (value != null)
      {
        if (enumType == typeof (int) && value.GetType() == typeof (long))
          value = (object) (int) (long) value;
        else if (enumType == typeof (bool))
          value = (object) ((long) value != 0L);
        else if (enumType.IsEnum)
          value = Enum.Parse(enumType, value.ToString(), false);
        else if (enumType == typeof (DateTime))
          value = (object) DateTime.FromFileTimeUtc((long) value);
      }
      prop.SetValue(obj.GetObject(), value, (object[]) null);
    }

    private object GetValue(object o, SqliteDataContext.ColumnState col)
    {
      return this.GetValue(o, col.PropertyInfo);
    }

    private object GetValue(object o, PropertyInfo prop)
    {
      object obj = prop.GetValue(o, (object[]) null);
      if (obj != null)
      {
        System.Type type = SqliteDataContext.NormalizeNullable(prop.PropertyType);
        if (type == typeof (bool))
          obj = (object) ((bool) obj ? 1 : 0);
        else if (type.IsEnum)
          obj = (object) (int) obj;
        else if (type == typeof (DateTime) && obj != null)
          obj = (object) ((DateTime) obj).ToFileTimeUtc();
      }
      return obj;
    }

    protected void EscapeString(StringBuilder sb, string str)
    {
      sb.Append('\'');
      foreach (char ch in str)
      {
        if (ch == '\'')
          sb.Append("''");
        else
          sb.Append(ch);
      }
      sb.Append('\'');
    }

    public static System.Type NormalizeNullable(System.Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>))
        type = ((IEnumerable<System.Type>) type.GetGenericArguments()).First<System.Type>();
      return type;
    }

    private string GetDbType(SqliteDataContext.ColumnState col)
    {
      PropertyInfo propertyInfo = col.PropertyInfo;
      System.Type type = SqliteDataContext.NormalizeNullable(propertyInfo.PropertyType);
      if (type == typeof (int) || type == typeof (long) || type == typeof (bool) || type.IsEnum || type == typeof (DateTime))
        return "INTEGER";
      if (type == typeof (byte[]))
        return "BLOB";
      if (type == typeof (string))
        return "TEXT";
      if (type == typeof (double))
        return "REAL";
      throw new Exception("Unexpected type " + (object) propertyInfo.PropertyType);
    }

    private void ExecBatch(string sql, Action<Sqlite.PreparedStatement, bool> onResult = null)
    {
      if (onResult == null)
        onResult = (Action<Sqlite.PreparedStatement, bool>) ((_, __) => { });
      int sqlTailOffset;
      for (; !string.IsNullOrEmpty(sql); sql = sql.Substring(sqlTailOffset).Trim())
      {
        Sqlite.PreparedStatement stmt;
        this.Db.PrepareStatement(sql, sql.Length, out sqlTailOffset, out stmt);
        using (stmt)
        {
          bool flag = stmt.Step();
          onResult(stmt, flag);
        }
        if (sqlTailOffset >= sql.Length)
          break;
      }
    }

    public Sqlite.PreparedStatement PrepareStatement(string sql) => this.Db.PrepareStatement(sql);

    public void PrepareCachedStatement(
      CachedStatement stmt,
      Action<Sqlite.PreparedStatement> callback)
    {
      callback(stmt.Prepare(this, this.Db));
    }

    public string GetFilename() => this.filename;

    public struct ColumnState
    {
      public string DbName;
      public PropertyInfo PropertyInfo;
      public ColumnAttribute ColumnAttribute;
      public bool Sensitive;
    }

    public class ObjectState
    {
      public object ObjectStrongRef;
      public WeakReference ObjectWeakRef;
      public SqliteDataContext.TableState Table;
      public Dictionary<string, string> DirtyColumns = new Dictionary<string, string>();
      public bool TrackingChanges;
      public bool Dirty;
      public bool New;
      public bool DeletePending;
      public int DbResetCount;

      public void OnPropDirty(string propName)
      {
        if (!this.TrackingChanges || this.DeletePending)
          return;
        if (!this.Table.IsDbLockHeld())
        {
          string stackTrace = AppState.GetStackTrace();
          Log.SendCrashLog(new Exception(string.Format("Column [{0}] written without DB lock{1}", (object) propName, string.IsNullOrEmpty(stackTrace) ? (object) "" : (object) ("\n" + stackTrace))), "lock check");
        }
        this.DirtyColumns[propName] = propName;
        if (this.Dirty)
          return;
        this.Table.OnObjectDirty(this);
        this.Dirty = true;
      }

      public object GetObject()
      {
        object obj = this.ObjectStrongRef;
        if (obj == null)
        {
          Log.l("StrongRef should not be null!");
          obj = this.ObjectWeakRef.Target;
          if (obj == null)
            Log.l("Weak ref null too!  Object was GC'd");
          this.LogWeakRef();
        }
        return obj;
      }

      public void LogWeakRef()
      {
        Log.SendCrashLog(new Exception("problem with weak references"), new StackTrace().ToString());
      }
    }

    public class TableState
    {
      public SqliteDataContext Context;
      public string Name;
      public IEnumerable<IndexAttribute> IndexAttributes;
      public bool TableWasAdded;
      public Dictionary<string, bool> DbHasColumn;
      public Dictionary<string, bool> DbHasIndex;
      public string PrimaryKeyDBName;
      public PropertyInfo PrimaryKey;
      public System.Type ObjectType;
      public Dictionary<string, SqliteDataContext.ColumnState> Columns = new Dictionary<string, SqliteDataContext.ColumnState>();
      public Dictionary<string, SqliteDataContext.ColumnState> ColumnsByDbName = new Dictionary<string, SqliteDataContext.ColumnState>();
      public List<SqliteDataContext.ObjectState> DirtyObjects = new List<SqliteDataContext.ObjectState>();
      public bool Dirty;
      private Dictionary<object, SqliteDataContext.ObjectState> Objects = new Dictionary<object, SqliteDataContext.ObjectState>();
      public List<SqliteDataContext.TableState> DirtyTables;
      public Sqlite.PreparedStatement InsertStmt;
      public Sqlite.PreparedStatement DeleteStmt;

      public bool IsDbLockHeld()
      {
        Func<bool> isLockHeld = this.Context.IsLockHeld;
        return isLockHeld == null || isLockHeld();
      }

      public IEnumerable<T> GetCachedObjects<T>() where T : class
      {
        return this.Objects.Values.Select<SqliteDataContext.ObjectState, T>((Func<SqliteDataContext.ObjectState, T>) (os => (os.ObjectStrongRef ?? os.ObjectWeakRef.Target) as T)).Where<T>((Func<T, bool>) (o => (object) o != null));
      }

      public bool GetObject(object key, out SqliteDataContext.ObjectState value)
      {
        return this.Objects.TryGetValue(key, out value);
      }

      public void AttachObject(object key, SqliteDataContext.ObjectState value)
      {
        value.Table = this;
        if (this.Objects.ContainsKey(key))
        {
          Log.l("db", "Warning! object has already been added...");
          this.Objects.Remove(key);
        }
        this.Objects.Add(key, value);
        object obj = value.GetObject();
        if (obj is INotifyPropertyChanging propertyChanging)
          propertyChanging.PropertyChanging += (PropertyChangingEventHandler) ((sender, args) => value.OnPropDirty(args.PropertyName));
        else if (obj == null)
          Log.l("db", "Warning: null object attached");
        else
          Log.l("db", "Warning: {0} does not implement INotifyPropertyChanging; DB updates will break.", (object) obj.GetType().Name);
      }

      public void DetachAll() => this.Objects.Clear();

      public void DetachObject(object key) => this.Objects.Remove(key);

      public void DetachObject<T>(Func<T, bool> func, bool removeEmpty = false)
      {
        foreach (KeyValuePair<object, SqliteDataContext.ObjectState> keyValuePair in this.Objects.ToArray<KeyValuePair<object, SqliteDataContext.ObjectState>>())
        {
          object target = keyValuePair.Value.ObjectWeakRef.Target;
          if (target == null & removeEmpty || target != null && target is T obj && func(obj))
            this.Objects.Remove(keyValuePair.Key);
        }
      }

      public void OnObjectDirty(SqliteDataContext.ObjectState obj)
      {
        this.DirtyObjects.Add(obj);
        if (!this.Dirty)
        {
          this.DirtyTables.Add(this);
          this.Dirty = true;
        }
        if (obj.ObjectStrongRef != null)
          return;
        obj.ObjectStrongRef = obj.ObjectWeakRef.Target;
      }

      public void ParseColumnState()
      {
        foreach (var data in ((IEnumerable<MemberInfo>) this.ObjectType.GetMembers()).Where<MemberInfo>((Func<MemberInfo, bool>) (mi => mi.MemberType == MemberTypes.Property)).Cast<PropertyInfo>().Select(pi => new
        {
          PropertyInfo = pi,
          Attribute = pi.GetCustomAttributes(typeof (ColumnAttribute), true).Cast<ColumnAttribute>().SingleOrDefault<ColumnAttribute>(),
          Sensitive = ((IEnumerable<object>) pi.GetCustomAttributes(typeof (SensitiveAttribute), true)).Any<object>()
        }).Where(p => p.Attribute != null && !p.Attribute.IsVersion))
        {
          string name = data.PropertyInfo.Name;
          string key = data.Attribute.Name;
          if (string.IsNullOrEmpty(key))
            key = name;
          if (data.Attribute.IsPrimaryKey)
          {
            this.PrimaryKey = data.PropertyInfo;
            this.PrimaryKeyDBName = key;
          }
          SqliteDataContext.ColumnState columnState = new SqliteDataContext.ColumnState()
          {
            DbName = key,
            PropertyInfo = data.PropertyInfo,
            ColumnAttribute = data.Attribute,
            Sensitive = data.Sensitive
          };
          this.Columns.Add(name, columnState);
          this.ColumnsByDbName.Add(key, columnState);
        }
      }
    }

    public class ChangeSet
    {
      public List<object> Inserts = new List<object>();
      public List<object> Deletes = new List<object>();
      public List<Pair<object, string[]>> Updates = new List<Pair<object, string[]>>();
    }
  }
}
