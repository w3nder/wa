// Decompiled with JetBrains decompiler
// Type: WhatsApp.OneDriveManifest
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using WhatsApp.WaCollections;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class OneDriveManifest : IDisposable
  {
    private static readonly string DefaultDbPath = Constants.IsoStorePath + "\\onedrive_manifest.db";
    private object databaseLock = new object();
    private string dbPath;
    private string inProgressMarker;
    private Sqlite db;

    public OneDriveManifest(string dbPath = null)
    {
      this.dbPath = dbPath != null ? dbPath : OneDriveManifest.DefaultDbPath;
      this.inProgressMarker = this.dbPath + ".pending";
    }

    public bool IsOpen => this.db != null;

    public string FileName => this.dbPath;

    public bool Exists => this.db != null || System.IO.File.Exists(this.dbPath);

    public void Open()
    {
      lock (this.databaseLock)
      {
        if (this.db != null)
          return;
        this.db = new Sqlite(this.dbPath, SqliteOpenFlags.Defaults);
        Log.d("onedrive", "manifest DB opened");
        int schemaVersion = this.CheckSchemaVersion();
        if (schemaVersion >= 1)
          return;
        this.db.BeginTransaction();
        try
        {
          this.UpdateSchema(schemaVersion);
          this.UpdateSchemaVersion(1);
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          this.Close();
          throw;
        }
      }
    }

    public void Close()
    {
      lock (this.databaseLock)
      {
        if (this.db == null)
          return;
        this.db.Dispose();
        this.db = (Sqlite) null;
        Log.d("onedrive", "manifest DB closed");
      }
    }

    public void Dispose() => this.Close();

    public void Delete()
    {
      lock (this.databaseLock)
      {
        this.Close();
        try
        {
          this.SetOneDriveBackupInProgress(false);
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(this.dbPath);
        }
        catch (FileNotFoundException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
      }
    }

    public string ComputeManifestHash()
    {
      if (this.IsOpen)
        throw new InvalidOperationException("Cannot compute hash of open manifest file");
      try
      {
        using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        {
          using (Stream inputStream = nativeMediaStorage.OpenFile(this.dbPath, FileMode.Open, FileAccess.Read))
          {
            using (SHA1Managed shA1Managed = new SHA1Managed())
              return shA1Managed.ComputeHash(inputStream).ToHexString();
          }
        }
      }
      catch (FileNotFoundException ex)
      {
        Log.p("onedrive", "manifest file not found, returning empty hash");
        return "";
      }
    }

    private int CheckSchemaVersion()
    {
      int num = 0;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("PRAGMA user_version"))
        {
          if (preparedStatement.Step())
            num = (int) (long) preparedStatement.Columns[0];
        }
      }
      catch (Exception ex)
      {
        num = -1;
      }
      return num;
    }

    private void UpdateSchemaVersion(int version)
    {
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("PRAGMA user_version = " + (object) version))
        preparedStatement.Step();
    }

    private bool DoesTableExist(string tableName)
    {
      bool flag = false;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT Count(*) FROM sqlite_master WHERE type='table' and name='" + tableName + "'"))
        {
          if (preparedStatement.Step())
            flag = (long) preparedStatement.Columns[0] != 0L;
        }
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag;
    }

    private void UpdateSchema(int schemaVersion)
    {
      if (schemaVersion >= 1)
        return;
      this.PrepareMetadataTable();
      this.PrepareDbFileMapTable();
      this.PrepareMediaFileMapTable();
    }

    private void PrepareMetadataTable()
    {
      if (this.DoesTableExist("Metadata"))
        return;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE TABLE Metadata (Key TEXT PRIMARY KEY, Value TEXT)"))
        preparedStatement.Step();
    }

    private void PrepareDbFileMapTable()
    {
      if (this.DoesTableExist("DbFileMap"))
        return;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE TABLE DbFileMap (Id INTEGER PRIMARY KEY AUTOINCREMENT, LocalFileRoot INTEGER, LocalFilePath TEXT, LocalFileName TEXT, RemoteFileId TEXT, RemoteContentTag TEXT, RemoteFileName TEXT, Sha1Hash BLOB, Size INTEGER)"))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE UNIQUE INDEX DbFileMapLocalIndex ON DbFileMap (LocalFileRoot, LocalFilePath, LocalFileName)"))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE UNIQUE INDEX DbFileMapRemoteIndex ON DbFileMap (RemoteFileId)"))
        preparedStatement.Step();
    }

    private void PrepareMediaFileMapTable()
    {
      if (this.DoesTableExist("MediaFileMap"))
        return;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE TABLE MediaFileMap (Id INTEGER PRIMARY KEY AUTOINCREMENT, LocalFileRoot INTEGER, LocalFilePath TEXT, LocalFileName TEXT, RemoteFileId TEXT, RemoteContentTag TEXT, RemoteFileName TEXT, Sha1Hash BLOB, Size INTEGER, IgnoreForBackup INTEGER)"))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE UNIQUE INDEX MediaFileMapLocalIndex ON MediaFileMap (LocalFileRoot, LocalFilePath, LocalFileName)"))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE UNIQUE INDEX MediaFileMapRemoteIndex ON MediaFileMap (RemoteFileId)"))
        preparedStatement.Step();
    }

    public void PrepareMediaRenameJournalTable()
    {
      if (this.DoesTableExist("MediaRenameJournal"))
        return;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE TABLE MediaRenameJournal (Id INTEGER PRIMARY KEY AUTOINCREMENT, OldFileRoot INTEGER, OldFilePath TEXT, OldFileName TEXT, NewFileRoot INTEGER, NewFilePath TEXT, NewFileName TEXT, Sha1Hash BLOB)"))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("CREATE UNIQUE INDEX MediaRenameOldIndex ON MediaRenameJournal (OldFileRoot, OldFilePath, OldFileName, Sha1Hash)"))
        preparedStatement.Step();
    }

    private void CheckDbOpen()
    {
      if (this.db != null)
        return;
      this.Open();
    }

    private void SetMetadataValue(string key, object value)
    {
      object o = value == null || value.GetType() != typeof (bool) ? value : (object) ((bool) value ? 1 : 0);
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("INSERT OR REPLACE INTO Metadata (Key, Value) VALUES (?, ?)"))
      {
        preparedStatement.Bind(0, key);
        preparedStatement.BindObject(1, o);
        preparedStatement.Step();
      }
    }

    private T GetMetadataValue<T>(string key, T defaultValue = null)
    {
      object s = (object) null;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT Value FROM Metadata WHERE Key = ?"))
      {
        preparedStatement.Bind(0, key);
        if (preparedStatement.Step())
          s = preparedStatement.Columns[0];
      }
      if (s == null)
        s = (object) defaultValue;
      else if (s.GetType() == typeof (string))
      {
        if (typeof (T) == typeof (bool))
          s = (object) (int.Parse((string) s) != 0);
        else if (typeof (T) == typeof (int))
          s = (object) int.Parse((string) s);
        else if (typeof (T) == typeof (long))
          s = (object) long.Parse((string) s);
      }
      return (T) s;
    }

    public void NormalizeManifest()
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        Log.l("onedrive", "normalizing file refs in manifest");
        this.db.BeginTransaction();
        try
        {
          int num = 0;
          Dictionary<long, FileRef> dictionary = new Dictionary<long, FileRef>();
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT Id, LocalFileRoot, LocalFilePath, LocalFileName FROM MediaFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
          {
            while (preparedStatement.Step())
            {
              ++num;
              long column = (long) preparedStatement.Columns[0];
              FileRef r = new FileRef()
              {
                Root = (FileRoot) (long) preparedStatement.Columns[1],
                Subdir = (string) preparedStatement.Columns[2],
                FilePart = (string) preparedStatement.Columns[3]
              };
              FileRef fileRef = r.Normalized();
              if (r.Root != fileRef.Root || !string.Equals(r.Subdir, fileRef.Subdir) || !string.Equals(r.FilePart, fileRef.FilePart))
                dictionary.Add(column, fileRef);
            }
          }
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET LocalFileRoot = ?, LocalFilePath = ?, LocalFileName = ? WHERE Id = ?"))
          {
            foreach (KeyValuePair<long, FileRef> keyValuePair in dictionary)
            {
              preparedStatement.Bind(0, (int) keyValuePair.Value.Root, false);
              preparedStatement.Bind(1, keyValuePair.Value.Subdir);
              preparedStatement.Bind(2, keyValuePair.Value.FilePart);
              preparedStatement.Bind(3, keyValuePair.Key, false);
              preparedStatement.Step();
              preparedStatement.Reset();
            }
          }
          this.db.CommitTransaction();
          Log.l("onedrive", "normalized {0}/{1} manifest entries", (object) dictionary.Count, (object) num);
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
    }

    public bool UpdateLocalDatabaseFiles(IEnumerable<LocalBackupFile> localFiles)
    {
      int num = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        Log.l("onedrive", "update manifest from local DB entries");
        this.db.BeginTransaction();
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM DbFileMap"))
            preparedStatement.Step();
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("INSERT INTO DbFileMap (LocalFileRoot, LocalFilePath, LocalFileName, Sha1Hash, Size) VALUES (?, ?, ?, ?, ?)"))
          {
            foreach (LocalBackupFile localFile in localFiles)
            {
              if (localFile.Sha1Hash != null && localFile.Sha1Hash.Length != 0)
              {
                preparedStatement.Bind(0, (int) localFile.FileReference.Root, false);
                preparedStatement.Bind(1, localFile.FileReference.Subdir);
                preparedStatement.Bind(2, localFile.FileReference.FilePart);
                preparedStatement.Bind(3, localFile.Sha1Hash);
                preparedStatement.Bind(4, localFile.Size, false);
                preparedStatement.Step();
                preparedStatement.Reset();
                ++num;
              }
            }
          }
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      Log.l("onedrive", "update manifest | {0} updated", (object) num);
      return num > 0;
    }

    public List<LocalBackupFile> LocalDatabaseFilesToUpload()
    {
      List<LocalBackupFile> upload = new List<LocalBackupFile>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT LocalFileRoot, LocalFilePath, LocalFileName, Sha1Hash, Size FROM DbFileMap WHERE RemoteFileId IS NULL"))
        {
          while (preparedStatement.Step())
            upload.Add(new LocalBackupFile()
            {
              FileReference = new FileRef()
              {
                Root = (FileRoot) (long) preparedStatement.Columns[0],
                Subdir = (string) preparedStatement.Columns[1],
                FilePart = (string) preparedStatement.Columns[2]
              },
              Sha1Hash = (byte[]) preparedStatement.Columns[3],
              Size = (long) preparedStatement.Columns[4]
            });
        }
      }
      return upload;
    }

    public void UpdateRemoteDatabaseFile(Item fileItem)
    {
      byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(fileItem);
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE DbFileMap SET RemoteFileId = ?, RemoteContentTag = ?, RemoteFileName = ? WHERE Sha1Hash = ?"))
          {
            preparedStatement.Bind(0, fileItem.Id);
            preparedStatement.Bind(1, fileItem.CTag);
            preparedStatement.Bind(2, fileItem.Name);
            preparedStatement.Bind(3, fileHash);
            preparedStatement.Step();
          }
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
    }

    public IEnumerable<RemoteDatabaseFile> GetRemoteDatabaseFiles()
    {
      List<RemoteDatabaseFile> remoteDatabaseFiles = new List<RemoteDatabaseFile>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT RemoteFileId, LocalFileName, Sha1Hash, Size FROM DbFileMap"))
        {
          while (preparedStatement.Step())
            remoteDatabaseFiles.Add(new RemoteDatabaseFile()
            {
              RemoteFileId = (string) preparedStatement.Columns[0],
              FileName = (string) preparedStatement.Columns[1],
              Sha1Hash = (byte[]) preparedStatement.Columns[2],
              Size = (long) preparedStatement.Columns[3]
            });
        }
      }
      return (IEnumerable<RemoteDatabaseFile>) remoteDatabaseFiles;
    }

    public bool UpdateLocalMediaFiles(IEnumerable<LocalBackupFile> localFiles)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        Log.l("onedrive", "update manifest from local file entries");
        this.db.BeginTransaction();
        try
        {
          Dictionary<string, LocalBackupFile> dictionary = new Dictionary<string, LocalBackupFile>();
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT LocalFileRoot, LocalFilePath, LocalFileName, Sha1Hash, Size FROM MediaFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
          {
            while (preparedStatement.Step())
            {
              FileRef r = new FileRef();
              r.Root = (FileRoot) (long) preparedStatement.Columns[0];
              r.Subdir = (string) preparedStatement.Columns[1];
              r.FilePart = (string) preparedStatement.Columns[2];
              LocalBackupFile localBackupFile = new LocalBackupFile();
              localBackupFile.FileReference = r;
              localBackupFile.Sha1Hash = (byte[]) preparedStatement.Columns[3];
              localBackupFile.Size = (long) preparedStatement.Columns[4];
              string absolutePath = r.ToAbsolutePath();
              dictionary.Add(absolutePath, localBackupFile);
            }
          }
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("INSERT OR REPLACE INTO MediaFileMap (LocalFileRoot, LocalFilePath, LocalFileName, Sha1Hash, Size) VALUES (?, ?, ?, ?, ?)"))
          {
            foreach (LocalBackupFile localFile in localFiles)
            {
              if (localFile.Sha1Hash != null && localFile.Sha1Hash.Length != 0)
              {
                string absolutePath = localFile.FileReference.ToAbsolutePath();
                LocalBackupFile localBackupFile;
                if (dictionary.TryGetValue(absolutePath, out localBackupFile) && localBackupFile.Sha1Hash.IsEqualBytes(localFile.Sha1Hash))
                {
                  dictionary.Remove(absolutePath);
                  ++num1;
                }
                else
                {
                  preparedStatement.Bind(0, (int) localFile.FileReference.Root, false);
                  preparedStatement.Bind(1, localFile.FileReference.Subdir);
                  preparedStatement.Bind(2, localFile.FileReference.FilePart);
                  preparedStatement.Bind(3, localFile.Sha1Hash);
                  preparedStatement.Bind(4, localFile.Size, false);
                  preparedStatement.Step();
                  preparedStatement.Reset();
                  dictionary.Remove(absolutePath);
                  ++num2;
                }
              }
            }
          }
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET LocalFileRoot = NULL, LocalFilePath = NULL, LocalFileName = NULL WHERE LocalFileRoot = ? AND LocalFilePath = ? AND LocalFileName = ?"))
          {
            foreach (KeyValuePair<string, LocalBackupFile> keyValuePair in dictionary)
            {
              preparedStatement.Bind(0, (int) keyValuePair.Value.FileReference.Root, false);
              preparedStatement.Bind(1, keyValuePair.Value.FileReference.Subdir);
              preparedStatement.Bind(2, keyValuePair.Value.FileReference.FilePart);
              preparedStatement.Step();
              preparedStatement.Reset();
            }
          }
          this.RemoveEmptyMediaFileRows();
          num3 = dictionary.Count;
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      Log.l("onedrive", "update manifest | {0} unchanged, {1} updated, {2} removed", (object) num1, (object) num2, (object) num3);
      return num2 > 0 || num3 > 0;
    }

    public string GetMediaDeltaToken()
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        return this.GetMetadataValue<string>("MediaDeltaToken");
      }
    }

    public void UpdateRemoteMediaFile(FileRef fileRef, Item fileItem)
    {
      OneDriveBackupFiles.ExtractFileHash(fileItem);
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = ?, RemoteContentTag = ?, RemoteFileName = ? WHERE LocalFileRoot = ? AND LocalFilePath = ? AND LocalFileName = ?"))
          {
            preparedStatement.Bind(0, fileItem.Id);
            preparedStatement.Bind(1, fileItem.CTag);
            preparedStatement.Bind(2, fileItem.Name);
            preparedStatement.Bind(3, (int) fileRef.Root, false);
            preparedStatement.Bind(4, fileRef.Subdir);
            preparedStatement.Bind(5, fileRef.FilePart);
            preparedStatement.Step();
          }
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
    }

    public bool UpdateRemoteMediaFiles(
      string deltaToken,
      bool resyncRequired,
      IEnumerable<Item> itemList)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          if (resyncRequired)
          {
            using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = NULL, RemoteContentTag = NULL, RemoteFileName = NULL"))
              preparedStatement.Step();
          }
          Set<string> set = new Set<string>();
          foreach (Item obj in itemList)
          {
            if (obj.Deleted != null)
            {
              using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = NULL, RemoteContentTag = NULL, RemoteFileName = NULL WHERE RemoteFileId = ?"))
              {
                preparedStatement.Bind(0, obj.Id);
                preparedStatement.Step();
                ++num1;
              }
            }
            else
            {
              this.PurgeInvalidRemoteMediaRows(obj);
              if (this.FindRemoteMediaRowId(obj) == -1L)
              {
                long localMediaRowId = this.FindLocalMediaRowId(obj);
                if (localMediaRowId == -1L)
                {
                  byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(obj);
                  using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("INSERT INTO MediaFileMap (RemoteFileId, RemoteContentTag, RemoteFileName, Sha1Hash) VALUES (?, ?, ?, ?)"))
                  {
                    preparedStatement.Bind(0, obj.Id);
                    preparedStatement.Bind(1, obj.CTag);
                    preparedStatement.Bind(2, obj.Name);
                    preparedStatement.Bind(3, fileHash);
                    preparedStatement.Step();
                    ++num2;
                  }
                }
                else
                {
                  using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = ?, RemoteContentTag = ?, RemoteFileName = ? WHERE Id = ?"))
                  {
                    preparedStatement.Bind(0, obj.Id);
                    preparedStatement.Bind(1, obj.CTag);
                    preparedStatement.Bind(2, obj.Name);
                    preparedStatement.Bind(3, localMediaRowId, false);
                    preparedStatement.Step();
                    ++num3;
                  }
                }
              }
              else
                ++num4;
            }
            set.Add(obj.Id);
          }
          if (resyncRequired)
          {
            using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = NULL, RemoteContentTag = NULL, RemoteFileName = NULL WHERE RemoteFileId = ?"))
            {
              foreach (Item obj in itemList)
              {
                if (!set.Contains(obj.Id))
                {
                  preparedStatement.Bind(0, obj.Id);
                  preparedStatement.Step();
                  preparedStatement.Reset();
                  ++num1;
                }
              }
            }
          }
          this.RemoveEmptyMediaFileRows();
          string metadataValue = this.GetMetadataValue<string>("MediaDeltaToken");
          if (deltaToken != metadataValue)
            this.SetMetadataValue("MediaDeltaToken", (object) deltaToken);
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      Log.l("onedrive", "update manifest remote | {0} updated, {1} deleted, {2} inserted, {3} unchanged", (object) num3, (object) num1, (object) num2, (object) num4);
      if (resyncRequired || num1 > 0)
        this.RefreshIncrementalBackupSize();
      return num3 > 0 || num1 > 0 || num2 > 0;
    }

    private void PurgeInvalidRemoteMediaRows(Item item)
    {
      byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(item);
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM MediaFileMap WHERE RemoteFileId = ? AND (RemoteContentTag != ? OR Sha1Hash != ?) AND LocalFileRoot IS NULL AND LocalFilePath IS NULL AND LocalFileName IS NULL"))
      {
        preparedStatement.Bind(0, item.Id);
        preparedStatement.Bind(1, item.CTag);
        preparedStatement.Bind(2, fileHash);
        preparedStatement.Step();
      }
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET RemoteFileId = NULL, RemoteContentTag = NULL, RemoteFileName = NULL WHERE RemoteFileId = ? AND (RemoteContentTag != ? OR Sha1Hash != ?) "))
      {
        preparedStatement.Bind(0, item.Id);
        preparedStatement.Bind(1, item.CTag);
        preparedStatement.Bind(2, fileHash);
        preparedStatement.Step();
      }
    }

    private long FindRemoteMediaRowId(Item item)
    {
      long remoteMediaRowId = -1;
      byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(item);
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT Id FROM MediaFileMap WHERE RemoteFileId = ? AND Sha1Hash = ?"))
      {
        preparedStatement.Bind(0, item.Id);
        preparedStatement.Bind(1, fileHash);
        if (preparedStatement.Step())
          remoteMediaRowId = (long) preparedStatement.Columns[0];
      }
      return remoteMediaRowId;
    }

    private long FindLocalMediaRowId(Item item)
    {
      long localMediaRowId = -1;
      FileRef fileRef = OneDriveBackupFiles.FromRemoteFileName(item.Name);
      byte[] fileHash = OneDriveBackupFiles.ExtractFileHash(item);
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT Id FROM MediaFileMap WHERE LocalFileRoot = ? AND LocalFilePath = ? AND LocalFileName = ? AND Sha1Hash = ?"))
      {
        preparedStatement.Bind(0, (int) fileRef.Root, false);
        preparedStatement.Bind(1, fileRef.Subdir);
        preparedStatement.Bind(2, fileRef.FilePart);
        preparedStatement.Bind(3, fileHash);
        if (preparedStatement.Step())
          localMediaRowId = (long) preparedStatement.Columns[0];
      }
      return localMediaRowId;
    }

    private void RemoveEmptyMediaFileRows()
    {
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM MediaFileMap WHERE LocalFileRoot IS NULL AND LocalFilePath IS NULL AND LocalFileName IS NULL AND RemoteFileId IS NULL AND RemoteContentTag IS NULL AND RemoteFileName IS NULL"))
        preparedStatement.Step();
    }

    public List<LocalBackupFile> LocalMediaFilesToUpload()
    {
      List<LocalBackupFile> upload = new List<LocalBackupFile>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT LocalFileRoot, LocalFilePath, LocalFileName, Sha1Hash, Size FROM MediaFileMap WHERE (IgnoreForBackup IS NULL OR IgnoreForBackup != 1) AND RemoteFileId IS NULL"))
        {
          while (preparedStatement.Step())
            upload.Add(new LocalBackupFile()
            {
              FileReference = new FileRef()
              {
                Root = (FileRoot) (long) preparedStatement.Columns[0],
                Subdir = (string) preparedStatement.Columns[1],
                FilePart = (string) preparedStatement.Columns[2]
              },
              Sha1Hash = (byte[]) preparedStatement.Columns[3],
              Size = (long) preparedStatement.Columns[4]
            });
        }
      }
      return upload;
    }

    public void BlacklistLocalMediaFile(LocalBackupFile backupFile)
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET IgnoreForBackup = 1 WHERE LocalFileRoot = ? AND LocalFilePath = ? AND LocalFileName = ? AND Sha1Hash = ?"))
        {
          preparedStatement.Bind(0, (int) backupFile.FileReference.Root, false);
          preparedStatement.Bind(1, backupFile.FileReference.Subdir);
          preparedStatement.Bind(2, backupFile.FileReference.FilePart);
          preparedStatement.Bind(3, backupFile.Sha1Hash);
          preparedStatement.Step();
        }
      }
    }

    public List<RemoteMediaFile> RemoteMediaFilesToPurge()
    {
      List<RemoteMediaFile> purge = new List<RemoteMediaFile>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT RemoteFileName, RemoteFileId, Sha1Hash, Size FROM MediaFileMap WHERE RemoteFileId IS NOT NULL AND LocalFileRoot IS NULL AND LocalFilePath IS NULL AND LocalFileName IS NULL ORDER BY Id"))
        {
          while (preparedStatement.Step())
          {
            RemoteMediaFile remoteMediaFile = new RemoteMediaFile()
            {
              FileReference = OneDriveBackupFiles.FromRemoteFileName(preparedStatement.Columns[0] as string),
              RemoteFileId = (string) preparedStatement.Columns[1],
              Sha1Hash = preparedStatement.Columns[2] as byte[],
              Size = preparedStatement.Columns[3] is long ? (long) preparedStatement.Columns[3] : 0L
            };
            purge.Add(remoteMediaFile);
          }
        }
      }
      return purge;
    }

    public List<RemoteMediaFile> RemoteMediaFilesToRestore(int fileCountLimit)
    {
      Log.d("onedrive", "RemoteMediaFilesToRestore limit {0}", (object) fileCountLimit);
      List<RemoteMediaFile> restore = new List<RemoteMediaFile>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        string sql = "SELECT LocalFileRoot, LocalFilePath, LocalFileName, RemoteFileId, Sha1Hash, Size FROM MediaFileMap WHERE RemoteFileId IS NOT NULL AND LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL ORDER BY Id DESC";
        if (fileCountLimit > 0)
          sql = sql + " LIMIT " + fileCountLimit.ToString();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement(sql))
        {
          while (preparedStatement.Step())
          {
            FileRef fileRef = new FileRef()
            {
              Root = (FileRoot) (long) preparedStatement.Columns[0],
              Subdir = (string) preparedStatement.Columns[1],
              FilePart = (string) preparedStatement.Columns[2]
            };
            RemoteMediaFile remoteMediaFile = new RemoteMediaFile()
            {
              FileReference = fileRef,
              RemoteFileId = (string) preparedStatement.Columns[3],
              Sha1Hash = (byte[]) preparedStatement.Columns[4],
              Size = (long) preparedStatement.Columns[5]
            };
            restore.Add(remoteMediaFile);
          }
        }
      }
      Log.d("onedrive", "found {0} media files", (object) restore.Count);
      return restore;
    }

    public void RemoveDownloadedMediaFile(RemoteMediaFile fileEntry)
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          long metadataValue = this.GetMetadataValue<long>("RestoredSize");
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM MediaFileMap WHERE RemoteFileId = ?"))
          {
            preparedStatement.Bind(0, fileEntry.RemoteFileId);
            preparedStatement.Step();
          }
          if (fileEntry.Size > 0L)
            this.SetMetadataValue("RestoredSize", (object) (metadataValue + fileEntry.Size));
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
    }

    public void RemoveRemoteMediaFileById(string remoteFileId)
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM MediaFileMap WHERE RemoteFileId = ?"))
        {
          preparedStatement.Bind(0, remoteFileId);
          preparedStatement.Step();
        }
      }
    }

    public MediaRenameJournalEntry InsertMediaRenameIntoJournal(
      FileRef oldFileRef,
      FileRef newFileRef,
      byte[] sha1Hash)
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("INSERT OR REPLACE INTO MediaRenameJournal (OldFileRoot, OldFilePath, OldFileName, NewFileRoot, NewFilePath, NewFileName, Sha1Hash) VALUES (?, ?, ?, ?, ?, ?, ?)"))
        {
          preparedStatement.Bind(0, (int) oldFileRef.Root, false);
          preparedStatement.Bind(1, oldFileRef.Subdir);
          preparedStatement.Bind(2, oldFileRef.FilePart);
          preparedStatement.Bind(3, (int) newFileRef.Root, false);
          preparedStatement.Bind(4, newFileRef.Subdir);
          preparedStatement.Bind(5, newFileRef.FilePart);
          preparedStatement.Bind(6, sha1Hash);
          preparedStatement.Step();
        }
      }
      return new MediaRenameJournalEntry()
      {
        OldFileReference = oldFileRef,
        NewFileReference = newFileRef,
        Sha1Hash = sha1Hash
      };
    }

    public List<MediaRenameJournalEntry> GetMediaRenameJournal()
    {
      List<MediaRenameJournalEntry> mediaRenameJournal = new List<MediaRenameJournalEntry>();
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT OldFileRoot, OldFilePath, OldFileName, NewFileRoot, NewFilePath, NewFileName, Sha1Hash FROM MediaRenameJournal"))
        {
          while (preparedStatement.Step())
          {
            FileRef fileRef1 = new FileRef()
            {
              Root = (FileRoot) (long) preparedStatement.Columns[0],
              Subdir = (string) preparedStatement.Columns[1],
              FilePart = (string) preparedStatement.Columns[2]
            };
            FileRef fileRef2 = new FileRef()
            {
              Root = (FileRoot) (long) preparedStatement.Columns[3],
              Subdir = (string) preparedStatement.Columns[4],
              FilePart = (string) preparedStatement.Columns[5]
            };
            MediaRenameJournalEntry renameJournalEntry = new MediaRenameJournalEntry()
            {
              OldFileReference = fileRef1,
              NewFileReference = fileRef2,
              Sha1Hash = (byte[]) preparedStatement.Columns[6]
            };
            mediaRenameJournal.Add(renameJournalEntry);
          }
        }
      }
      return mediaRenameJournal;
    }

    public void ApplyMediaRenameFromJournal(MediaRenameJournalEntry entry)
    {
      FileRef newFileReference = entry.NewFileReference;
      FileRef oldFileReference = entry.OldFileReference;
      byte[] sha1Hash = entry.Sha1Hash;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("UPDATE MediaFileMap SET LocalFileRoot = ?, LocalFilePath = ?, LocalFileName = ? WHERE LocalFileRoot = ? AND LocalFilePath = ? AND LocalFileName = ? AND Sha1Hash = ?"))
          {
            preparedStatement.Bind(0, (int) newFileReference.Root, false);
            preparedStatement.Bind(1, newFileReference.Subdir);
            preparedStatement.Bind(2, newFileReference.FilePart);
            preparedStatement.Bind(3, (int) oldFileReference.Root, false);
            preparedStatement.Bind(4, oldFileReference.Subdir);
            preparedStatement.Bind(5, oldFileReference.FilePart);
            preparedStatement.Bind(6, sha1Hash);
            preparedStatement.Step();
          }
          using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("DELETE FROM MediaRenameJournal WHERE OldFileRoot = ? AND OldFilePath = ? AND OldFileName = ? AND NewFileRoot = ? AND NewFilePath = ? AND NewFileName = ? AND Sha1Hash = ?"))
          {
            preparedStatement.Bind(0, (int) oldFileReference.Root, false);
            preparedStatement.Bind(1, oldFileReference.Subdir);
            preparedStatement.Bind(2, oldFileReference.FilePart);
            preparedStatement.Bind(3, (int) newFileReference.Root, false);
            preparedStatement.Bind(4, newFileReference.Subdir);
            preparedStatement.Bind(5, newFileReference.FilePart);
            preparedStatement.Bind(6, sha1Hash);
            preparedStatement.Step();
          }
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
    }

    public void StartNewBackup(long backupStart, BackupSettings backupSettings)
    {
      long num1 = backupStart / 10000L;
      long num2 = backupStart / 600000000L * 10L;
      string str = num2.ToString();
      long bytes1 = 0;
      long bytes2 = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          if (this.IsOneDriveBackupInProgress())
          {
            Log.l("onedrive", "cannot start backup, already in progress");
            throw new InvalidOperationException("already in progress");
          }
          bytes1 = this.CalculateCompleteBackupSize();
          bytes2 = this.CalculateIncrementalBackupSize();
          string metadataValue1 = this.GetMetadataValue<string>("BackupId");
          long metadataValue2 = this.GetMetadataValue<long>("BackupStartTime");
          long metadataValue3 = this.GetMetadataValue<long>("BackupSize");
          if (!string.IsNullOrEmpty(metadataValue1))
          {
            for (; metadataValue1.Equals(str, StringComparison.InvariantCultureIgnoreCase); str = num2.ToString())
              ++num2;
            this.SetMetadataValue("LastBackupId", (object) metadataValue1);
            this.SetMetadataValue("LastBackupStartTime", (object) metadataValue2);
            this.SetMetadataValue("LastBackupSize", (object) metadataValue3);
          }
          else
          {
            this.SetMetadataValue("LastBackupId", (object) null);
            this.SetMetadataValue("LastBackupStartTime", (object) null);
            this.SetMetadataValue("LastBackupSize", (object) null);
          }
          this.SetMetadataValue("BackupId", (object) str);
          this.SetMetadataValue("BackupStartTime", (object) num1);
          this.SetMetadataValue("BackupSize", (object) bytes1);
          this.SetMetadataValue("BackupIncrementalSize", (object) bytes2);
          this.SetMetadataValue("BackupFrequency", (object) (long) backupSettings.BackupFrequency);
          this.SetMetadataValue("BackupNetwork", (object) (long) backupSettings.BackupNetwork);
          this.SetMetadataValue("BackupIncludeVideos", (object) backupSettings.IncludeVideos);
          this.SetOneDriveBackupInProgress(true);
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      Log.l("onedrive", "backup started, ID={0}, Time={1}, Size={2}, IncSize={3}", (object) str, (object) num1, (object) Utils.FileSizeFormatter.Format(bytes1), (object) Utils.FileSizeFormatter.Format(bytes2));
    }

    public void RefreshIncrementalBackupSize()
    {
      long bytes1 = 0;
      long bytes2 = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          if (!this.IsOneDriveBackupInProgress())
          {
            Log.l("onedrive", "cannot update backup not in progress");
            throw new InvalidOperationException("cannot update backup not in progress");
          }
          if (!string.IsNullOrEmpty(this.GetMetadataValue<string>("BackupId")))
          {
            bytes1 = this.GetMetadataValue<long>("BackupSize");
            bytes2 = this.CalculateIncrementalBackupSize();
            this.SetMetadataValue("BackupIncrementalSize", (object) bytes2);
          }
          this.db.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      if (bytes1 <= 0L && bytes2 <= 0L)
        return;
      Log.l("onedrive", "backup incremental size updated: {0} -> {1}", (object) Utils.FileSizeFormatter.Format(bytes1), (object) Utils.FileSizeFormatter.Format(bytes2));
    }

    private long CalculateCompleteBackupSize()
    {
      long num1 = 0;
      long num2 = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM DbFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
      {
        if (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is long)
            num1 = (long) preparedStatement.Columns[0];
        }
      }
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM MediaFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
      {
        if (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is long)
            num2 = (long) preparedStatement.Columns[0];
        }
      }
      return num1 + num2;
    }

    private long CalculateIncrementalBackupSize()
    {
      long num1 = 0;
      long num2 = 0;
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM DbFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL AND RemoteFileId IS NULL"))
      {
        if (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is long)
            num1 = (long) preparedStatement.Columns[0];
        }
      }
      using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM MediaFileMap WHERE LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL AND RemoteFileId IS NULL"))
      {
        if (preparedStatement.Step())
        {
          if (preparedStatement.Columns[0] is long)
            num2 = (long) preparedStatement.Columns[0];
        }
      }
      return num1 + num2;
    }

    public long GetRemoteDatabaseSizeToRestore()
    {
      long databaseSizeToRestore = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM DbFileMap WHERE RemoteFileId IS NOT NULL AND LocalFileName IS NOT NULL"))
        {
          if (preparedStatement.Step())
          {
            if (preparedStatement.Columns[0] is long)
              databaseSizeToRestore = (long) preparedStatement.Columns[0];
          }
        }
      }
      return databaseSizeToRestore;
    }

    public long GetRemoteMediaSizeToRestore()
    {
      long mediaSizeToRestore = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT SUM(Size) FROM MediaFileMap WHERE RemoteFileId IS NOT NULL AND LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
        {
          if (preparedStatement.Step())
          {
            if (preparedStatement.Columns[0] is long)
              mediaSizeToRestore = (long) preparedStatement.Columns[0];
          }
        }
      }
      return mediaSizeToRestore;
    }

    public long GetRemoteMediaFileCount()
    {
      long remoteMediaFileCount = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        using (Sqlite.PreparedStatement preparedStatement = this.db.PrepareStatement("SELECT COUNT(*) FROM MediaFileMap WHERE RemoteFileId IS NOT NULL AND LocalFileRoot IS NOT NULL AND LocalFilePath IS NOT NULL AND LocalFileName IS NOT NULL"))
        {
          if (preparedStatement.Step())
          {
            if (preparedStatement.Columns[0] is long)
              remoteMediaFileCount = (long) preparedStatement.Columns[0];
          }
        }
      }
      return remoteMediaFileCount;
    }

    public void UpdateRestoreEstimates(long sizeEstimate, int countEstimate)
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.SetMetadataValue("RestoreSizeEstimate", (object) sizeEstimate);
        this.SetMetadataValue("RestoreCountEstimate", (object) countEstimate);
      }
    }

    public void ClearCurrentOneDriveBackup()
    {
      string str = "";
      long num = 0;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          str = this.GetMetadataValue<string>("BackupId", "");
          num = this.GetMetadataValue<long>("BackupStartTime");
          this.SetMetadataValue("BackupId", (object) null);
          this.SetMetadataValue("BackupStartTime", (object) null);
          this.SetMetadataValue("BackupSize", (object) null);
          this.SetMetadataValue("BackupIncrementalSize", (object) null);
          this.SetMetadataValue("BackupFrequency", (object) null);
          this.SetMetadataValue("BackupNetwork", (object) null);
          this.SetMetadataValue("BackupIncludeVideos", (object) null);
          this.db.CommitTransaction();
          this.SetOneDriveBackupInProgress(false);
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      Log.l("onedrive", "backup cleared, ID={0}, Time={1}", (object) str, (object) num);
    }

    public bool IsOneDriveBackupInProgress()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(this.inProgressMarker);
    }

    public void SetOneDriveBackupInProgress(bool inProgress)
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
      {
        bool flag = nativeMediaStorage.FileExists(this.inProgressMarker);
        if (inProgress)
        {
          if (flag)
            return;
          Log.l("onedrive", "setting OneDrive in-progress marker");
          nativeMediaStorage.OpenFile(this.inProgressMarker, FileMode.Create, FileAccess.Read).Close();
        }
        else
        {
          if (!flag)
            return;
          Log.l("onedrive", "clearing OneDrive in-progress marker");
          nativeMediaStorage.DeleteFile(this.inProgressMarker);
        }
      }
    }

    public long CurrentOneDriveBackupSize()
    {
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        return this.GetMetadataValue<long>("BackupSize");
      }
    }

    public BackupProperties CurrentOneDriveBackupProperties()
    {
      BackupProperties backupProperties = (BackupProperties) null;
      lock (this.databaseLock)
      {
        this.CheckDbOpen();
        this.db.BeginTransaction();
        try
        {
          string metadataValue1 = this.GetMetadataValue<string>("BackupId");
          long metadataValue2 = this.GetMetadataValue<long>("BackupStartTime");
          long metadataValue3 = this.GetMetadataValue<long>("BackupSize");
          string metadataValue4 = this.GetMetadataValue<string>("LastBackupId");
          long metadataValue5 = this.GetMetadataValue<long>("LastBackupStartTime");
          long metadataValue6 = this.GetMetadataValue<long>("LastBackupSize");
          long metadataValue7 = this.GetMetadataValue<long>("BackupIncrementalSize");
          OneDriveBackupFrequency driveBackupFrequency = (OneDriveBackupFrequency) this.GetMetadataValue<long>("BackupFrequency");
          AutoDownloadSetting autoDownloadSetting = (AutoDownloadSetting) this.GetMetadataValue<long>("BackupNetwork", 1L);
          bool metadataValue8 = this.GetMetadataValue<bool>("BackupIncludeVideos");
          long metadataValue9 = this.GetMetadataValue<long>("RestoredSize");
          long metadataValue10 = this.GetMetadataValue<long>("RestoreSizeEstimate", -1L);
          long metadataValue11 = this.GetMetadataValue<long>("RestoreCountEstimate", -1L);
          bool flag = this.IsOneDriveBackupInProgress();
          long incrementalBackupSize = flag ? this.CalculateIncrementalBackupSize() : 0L;
          this.db.CommitTransaction();
          if (!string.IsNullOrEmpty(metadataValue1))
          {
            if (Array.IndexOf(Enum.GetValues(typeof (OneDriveBackupFrequency)), (object) driveBackupFrequency) < 0)
            {
              Log.d("manifest", "fixing missing or invalid frequency setting in properties: {0}", (object) (int) driveBackupFrequency);
              driveBackupFrequency = OneDriveBackupFrequency.Off;
            }
            if (autoDownloadSetting != AutoDownloadSetting.EnabledOnWifi && autoDownloadSetting != AutoDownloadSetting.Enabled)
            {
              Log.d("manifest", "fixing missing or invalid network setting in properties: {0}", (object) (int) autoDownloadSetting);
              autoDownloadSetting = AutoDownloadSetting.EnabledOnWifi;
            }
            backupProperties = new BackupProperties();
            backupProperties.BackupId = metadataValue1;
            backupProperties.StartTime = new DateTime(metadataValue2 * 10000L).ToLocalTime();
            backupProperties.Size = metadataValue3;
            backupProperties.LastBackupId = metadataValue4;
            backupProperties.LastStartTime = new DateTime(metadataValue5 * 10000L).ToLocalTime();
            backupProperties.LastSize = metadataValue6;
            backupProperties.IncrementalSize = metadataValue7;
            backupProperties.Settings = new BackupSettings()
            {
              BackupFrequency = driveBackupFrequency,
              BackupNetwork = autoDownloadSetting,
              IncludeVideos = metadataValue8
            };
            backupProperties.IncompleteSize = incrementalBackupSize;
            backupProperties.RestoredSize = metadataValue9;
            backupProperties.RestoreSizeEstimate = metadataValue10 >= 0L ? new long?(metadataValue10) : new long?();
            backupProperties.RestoreCountEstimate = metadataValue11 >= 0L ? new long?(metadataValue11) : new long?();
            backupProperties.InProgress = flag;
          }
        }
        catch (Exception ex)
        {
          this.db.RollbackTransaction(ex);
          throw;
        }
      }
      return backupProperties;
    }
  }
}
