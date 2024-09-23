// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class SqliteSettings : ISettings
  {
    private bool trustCache;
    private bool deleteOnCommit;
    private string dbDir;
    private string dbPath;
    private Sqlite dbForWrite;
    private Dictionary<Settings.Key, object> cache = new Dictionary<Settings.Key, object>();
    private SqliteSynchronizeOptions? sync;

    public SqliteSettings(string isoStoreDir, SqliteSynchronizeOptions? sync = null)
    {
      this.sync = sync;
      StringBuilder stringBuilder = (StringBuilder) null;
      if (isoStoreDir != null)
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          stringBuilder = this.CreateDirectory(storeForApplication, isoStoreDir);
        this.dbDir = stringBuilder.ToString();
      }
      else
        this.dbDir = "";
      this.dbPath = string.Format("{0}\\{1}settings.db", (object) Constants.IsoStorePath, (object) this.dbDir);
      this.Export((Action<Settings.Key, object>) ((key, value) =>
      {
        if (value == null)
          return;
        this.cache[key] = value;
      }));
      this.trustCache = true;
    }

    private StringBuilder CreateDirectory(IsolatedStorageFile fs, string isoStoreDir)
    {
      StringBuilder directory = new StringBuilder();
      foreach (string str1 in ((IEnumerable<string>) isoStoreDir.Split('/', '\\')).Where<string>((Func<string, bool>) (s => s.Length != 0)))
      {
        directory.Append(str1);
        string str2 = directory.ToString();
        if (!fs.DirectoryExists(str2))
          fs.CreateDirectory(str2);
        directory.Append('\\');
      }
      return directory;
    }

    public void Reset() => this.deleteOnCommit = true;

    public void Set<T>(Settings.Key Key, T obj, bool bypassCache = false)
    {
      if ((object) obj == null)
      {
        this.Delete(Key);
      }
      else
      {
        object obj1 = (object) null;
        object serialized = SqliteSettings.Serialize((object) obj);
        if (!bypassCache && this.cache.TryGetValue(Key, out obj1) && obj1.Equals(serialized))
          return;
        this.Write((Action<Sqlite>) (db =>
        {
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("INSERT OR REPLACE INTO Settings (Key, Value) VALUES (?,?)"))
          {
            preparedStatement.Bind(0, (int) Key, false);
            preparedStatement.BindObject(1, serialized);
            preparedStatement.Step();
          }
        }));
        this.cache[Key] = serialized;
      }
    }

    public void Delete(Settings.Key Key)
    {
      this.Write((Action<Sqlite>) (db =>
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("DELETE FROM Settings WHERE Key = ?"))
        {
          preparedStatement.Bind(0, (int) Key, false);
          preparedStatement.Step();
        }
      }));
      this.cache.Remove(Key);
    }

    public void InPlaceSet<T>(Settings.Key Key, T obj)
    {
      this.Set<T>(Key, obj, false);
      this.Save();
    }

    public void Save()
    {
      if (this.deleteOnCommit)
      {
        if (this.dbForWrite != null)
        {
          try
          {
            this.dbForWrite.RollbackTransaction();
          }
          finally
          {
            this.dbForWrite.SafeDispose();
            this.dbForWrite = (Sqlite) null;
          }
        }
        if (this.dbDir.Length != 0)
        {
          string dir = string.Format("{0}\\{1}", (object) Constants.IsoStorePath, (object) this.dbDir);
          this.PerformWithRetry((Action) (() =>
          {
            try
            {
              NativeInterfaces.Misc.RemoveDirectoryRecursive(dir);
            }
            catch (DirectoryNotFoundException ex)
            {
            }
          }));
          this.PerformWithRetry((Action) (() =>
          {
            using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
              this.CreateDirectory(storeForApplication, this.dbDir);
          }));
        }
        else
          this.Write((Action<Sqlite>) (db =>
          {
            using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("DELETE FROM Settings"))
              preparedStatement.Step();
            using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("COMMIT TRANSACTION"))
              preparedStatement.Step();
          }));
        this.deleteOnCommit = false;
      }
      else
      {
        if (this.dbForWrite == null)
          return;
        DateTime? start = PerformanceTimer.Start();
        try
        {
          this.dbForWrite.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.dbForWrite.RollbackTransaction(ex);
          throw;
        }
        finally
        {
          this.dbForWrite.SafeDispose();
          this.dbForWrite = (Sqlite) null;
        }
        PerformanceTimer.End("settings commit", start);
      }
    }

    public bool TryGet<T>(Settings.Key k, out T v)
    {
      object vObj = (object) null;
      bool found = false;
      if (this.cache.TryGetValue(k, out vObj))
        found = true;
      else if (!this.trustCache)
        this.Read((Action<Sqlite>) (db =>
        {
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT Value FROM Settings WHERE Key = ?"))
          {
            preparedStatement.Bind(0, (int) k, false);
            if (!preparedStatement.Step())
              return;
            vObj = preparedStatement.Columns[0];
            found = true;
            this.cache[k] = vObj;
          }
        }));
      v = !found ? default (T) : (T) SqliteSettings.Deserialize<T>(vObj);
      return found;
    }

    public T Get<T>(Settings.Key k, T defaultVal)
    {
      T v = defaultVal;
      return !this.TryGet<T>(k, out v) ? defaultVal : v;
    }

    private bool SettingsTableExists(Sqlite db, bool onError = true)
    {
      bool flag = false;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT Count(*) FROM sqlite_master WHERE type='table' and name='Settings'"))
        {
          if (preparedStatement.Step())
            flag = (long) preparedStatement.Columns[0] != 0L;
        }
      }
      catch (Exception ex)
      {
        flag = onError;
      }
      return flag;
    }

    public void Export(Action<Settings.Key, object> onItem)
    {
      bool workaround = false;
      bool retried = false;
      bool corrupt = false;
      do
      {
        if (corrupt)
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
          {
            try
            {
              nativeMediaStorage.DeleteFile(this.dbPath);
            }
            catch (Exception ex)
            {
            }
          }
        }
        if (workaround)
          this.Write((Action<Sqlite>) (db => { }));
        if (workaround | corrupt)
          retried = true;
        this.Read((Action<Sqlite>) (db =>
        {
          Sqlite.PreparedStatement preparedStatement;
          try
          {
            preparedStatement = db.PrepareStatement("SELECT Key, Value FROM Settings");
          }
          catch (Exception ex)
          {
            if (!this.SettingsTableExists(db))
              return;
            uint hresult = ex.GetHResult();
            if (retried)
            {
              throw;
            }
            else
            {
              if ((int) hresult == (int) Sqlite.HRForError(11U))
              {
                corrupt = true;
                return;
              }
              if ((int) hresult == (int) Sqlite.HRForError(8U))
              {
                workaround = true;
                return;
              }
              throw;
            }
          }
          using (preparedStatement)
          {
            while (preparedStatement.Step())
            {
              object[] columns = preparedStatement.Columns;
              onItem((Settings.Key) Enum.Parse(typeof (Settings.Key), columns[0].ToString(), false), columns[1]);
            }
          }
        }));
      }
      while (workaround | corrupt && !retried);
    }

    private static object Serialize(object o)
    {
      if (o != null)
      {
        System.Type type = SqliteDataContext.NormalizeNullable(o.GetType());
        if (type == typeof (DateTime))
        {
          DateTime dateTime = (DateTime) o;
          o = !(dateTime == DateTime.MinValue) ? (object) dateTime.ToFileTimeUtc() : (object) dateTime;
        }
        else if (type == typeof (bool))
          o = (object) ((bool) o ? 1L : 0L);
        else if (type.IsEnum)
          o = (object) (long) (int) o;
        else if (type == typeof (int))
          o = (object) (long) (int) o;
      }
      return o;
    }

    private static object Deserialize<T>(object o)
    {
      if (o != null)
      {
        System.Type enumType = SqliteDataContext.NormalizeNullable(typeof (T));
        if (enumType == typeof (DateTime))
          o = (object) DateTime.FromFileTimeUtc((long) o);
        else if (enumType == typeof (bool))
          o = (object) !o.Equals((object) 0L);
        else if (enumType.IsEnum)
          o = Enum.Parse(enumType, o.ToString(), false);
        else if (o is long && enumType == typeof (int))
          o = (object) (int) (long) o;
      }
      return o;
    }

    private void Read(Action<Sqlite> onDb)
    {
      DateTime? start = PerformanceTimer.Start();
      Sqlite sqlite;
      try
      {
        sqlite = new Sqlite(this.dbPath, SqliteOpenFlags.READONLY, syncMode: this.sync, wal: false);
      }
      catch (Exception ex)
      {
        if ((int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
          {
            if (!storeForApplication.FileExists(this.dbDir + "settings.db"))
              return;
          }
        }
        throw;
      }
      using (sqlite)
        onDb(sqlite);
      PerformanceTimer.End("settings read", start);
    }

    private void Write(Action<Sqlite> onDb)
    {
      try
      {
        if (this.dbForWrite == null)
        {
          this.dbForWrite = new Sqlite(this.dbPath, SqliteOpenFlags.Defaults, syncMode: this.sync, wal: false);
          string[] strArray = new string[1]
          {
            "CREATE TABLE IF NOT EXISTS Settings (Key INT PRIMARY KEY, Value)"
          };
          this.dbForWrite.BeginTransaction();
          foreach (string sql in strArray)
          {
            using (Sqlite.PreparedStatement preparedStatement = this.dbForWrite.PrepareStatement(sql))
              preparedStatement.Step();
          }
        }
        onDb(this.dbForWrite);
      }
      catch (Exception ex)
      {
        if (this.dbForWrite != null)
        {
          this.dbForWrite.Dispose();
          this.dbForWrite = (Sqlite) null;
        }
        throw;
      }
    }

    private void PerformWithRetry(Action a)
    {
      int num = 0;
      while (true)
      {
        try
        {
          a();
          break;
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "retry");
          if (++num >= 3)
            throw;
        }
        Thread.Sleep(500);
      }
    }
  }
}
