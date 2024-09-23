// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqliteAxolotlStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using WhatsAppNative;

#nullable disable
namespace WhatsApp
{
  public class SqliteAxolotlStore : IAxolotlStore
  {
    private object databaseLock = new object();
    private string DbName = Constants.IsoStorePath + "\\axolotl.db";
    private string SessionsTableName = "sessions";
    private string PreKeyTableName = "prekeys";
    private string SignedPreKeyTableName = "signed_prekeys";
    private string IdentitiesTableName = "identities";
    private string MessageBaseKeyTableName = "message_base_key";
    private const int SIGNED_PREKEY_ROTATION_INTERVAL_DAYS = 30;
    private string SenderKeyTableName = "sender_keys";
    private string FastRatchetSenderKeyTableName = "fast_ratchet_sender_keys";
    private Axolotl Axolotl;
    private const int MyDeviceId = 0;
    private const string MyRecipientId = "-1";
    private const int PreKeyTrickleSize = 1;
    private const int libAxolotl_PRE_KEY_MEDIUM_MAX_VALUE = 16777215;
    private const int SignedPreKeyHistorySize = 5;
    private Sqlite database;

    public SqliteAxolotlStore(Axolotl axolotl)
    {
      this.Axolotl = axolotl;
      this.InitializeDatabase();
    }

    public void Initialize()
    {
      try
      {
        if (!this.RegisteredSelf)
        {
          Log.l("E2EStore", "Creating Self Registration Data");
          this.Axolotl.NativeInterface.CreateRegistrationData();
        }
        if (!this.SignedKeyGenerated)
        {
          try
          {
            this.GenerateSignedPreKey();
          }
          catch (Exception ex)
          {
            ++Settings.AxolotlRegistrationRetries;
            throw;
          }
        }
      }
      catch (Exception ex)
      {
        Log.l("E2EStore", "FAILED Creating Self Registration Data");
        Log.SendCrashLog((Exception) new Axolotl.AxolotlRegistrationException(), "E2EStore FAILED Creating Self Registration Data", false);
        this.Reset();
        ++Settings.AxolotlRegistrationRetries;
        throw ex;
      }
      if (this.HasEnoughUnsentPreKeys)
        return;
      WAThreadPool.QueueUserWorkItem(new Action(this.Axolotl.PreGeneratePreKeys));
    }

    private Sqlite Database
    {
      get
      {
        if (this.database == null)
          this.database = new Sqlite(this.DbName, SqliteOpenFlags.Defaults);
        return this.database;
      }
    }

    public void DisposeDatabase()
    {
      lock (this.databaseLock)
      {
        this.database.SafeDispose();
        this.database = (Sqlite) null;
      }
    }

    private void InitializeDatabase()
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        int schema;
        if ((schema = this.CheckSchemaVersion(database)) >= 5)
          return;
        database.BeginTransaction();
        try
        {
          this.UpdateSchema(database, schema);
          this.UpdateSchemaVersion(database, 5);
          database.CommitTransaction();
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
          throw;
        }
      }
    }

    private void UpdateSchema(Sqlite db, int schema)
    {
      if (schema < 1)
      {
        this.PrepareSessionTable(db);
        this.PreparePreKeyTable(db);
        this.PrepareSignedPreKeyTable(db);
        this.PrepareIdentitiesTable(db);
        this.PrepareMessageBaseKeyTable(db);
      }
      if (schema < 2)
        this.PrepareSenderKeyTable(db);
      if (schema < 3)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("ALTER TABLE " + this.IdentitiesTableName + " ADD COLUMN next_signed_prekey_id INTEGER"))
          preparedStatement.Step();
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("UPDATE " + this.IdentitiesTableName + " SET next_signed_prekey_id = ? WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, 1, false);
          preparedStatement.Bind(1, "-1");
          preparedStatement.Step();
        }
      }
      if (schema < 4)
        this.PrepareFastRatchetSenderKeyTable(db);
      if (schema >= 5)
        return;
      long unixTime = DateTime.UtcNow.ToUnixTime();
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("ALTER TABLE " + this.SenderKeyTableName + " ADD COLUMN creation_timestamp INTEGER DEFAULT " + unixTime.ToString()))
        preparedStatement.Step();
    }

    private bool DoesTableExist(Sqlite db, string tableName)
    {
      bool flag = false;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("SELECT Count(*) FROM sqlite_master WHERE type='table' and name='" + tableName + "'"))
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

    private int CheckSchemaVersion(Sqlite db)
    {
      int num = 0;
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("PRAGMA user_version"))
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

    private void UpdateSchemaVersion(Sqlite db, int version)
    {
      try
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("PRAGMA user_version = " + (object) version))
          preparedStatement.Step();
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    private void PrepareSessionTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.SessionsTableName))
        return;
      string sql = "CREATE TABLE IF NOT EXISTS " + this.SessionsTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, recipient_id INTEGER UNIQUE, record BLOB, timestamp INTEGER)";
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        preparedStatement.Step();
    }

    private void PreparePreKeyTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.PreKeyTableName))
        return;
      string sql = "CREATE TABLE IF NOT EXISTS " + this.PreKeyTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, prekey_id INTEGER UNIQUE, sent_to_server BOOLEAN, record BLOB)";
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        preparedStatement.Step();
    }

    private void PrepareSignedPreKeyTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.SignedPreKeyTableName))
        return;
      string sql = "CREATE TABLE IF NOT EXISTS " + this.SignedPreKeyTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, prekey_id INTEGER UNIQUE, timestamp INTEGER, record BLOB)";
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        preparedStatement.Step();
    }

    private void PrepareIdentitiesTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.IdentitiesTableName))
        return;
      string sql = "CREATE TABLE IF NOT EXISTS " + this.IdentitiesTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, recipient_id INTEGER UNIQUE, registration_id INTEGER, public_key BLOB, private_key BLOB, next_prekey_id INTEGER, timestamp INTEGER)";
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        preparedStatement.Step();
    }

    private void PrepareMessageBaseKeyTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.MessageBaseKeyTableName))
        return;
      string[] strArray = new string[2]
      {
        "CREATE TABLE IF NOT EXISTS " + this.MessageBaseKeyTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, msg_key_remote_jid TEXT NOT NULL, msg_key_from_me BOOLEAN NOT NULL, msg_key_id TEXT NOT NULL, last_alice_base_key BLOB NOT NULL, timestamp INTEGER)",
        "CREATE UNIQUE INDEX message_base_key_index ON " + this.MessageBaseKeyTableName + " (msg_key_remote_jid, msg_key_from_me, msg_key_id)"
      };
      foreach (string sql in strArray)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          preparedStatement.Step();
      }
    }

    private void PrepareSenderKeyTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.SenderKeyTableName))
        return;
      string[] strArray = new string[2]
      {
        "CREATE TABLE IF NOT EXISTS " + this.SenderKeyTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, group_id TEXT NOT NULL, sender_id INTEGER, record BLOB NOT NULL)",
        "CREATE UNIQUE INDEX sender_keys_index ON " + this.SenderKeyTableName + " (group_id, sender_id)"
      };
      foreach (string sql in strArray)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          preparedStatement.Step();
      }
    }

    private void PrepareFastRatchetSenderKeyTable(Sqlite db)
    {
      if (this.DoesTableExist(db, this.FastRatchetSenderKeyTableName))
        return;
      string[] strArray = new string[2]
      {
        "CREATE TABLE IF NOT EXISTS " + this.FastRatchetSenderKeyTableName + " (id INTEGER PRIMARY KEY AUTOINCREMENT, group_id TEXT NOT NULL, sender_id INTEGER, record BLOB NOT NULL)",
        "CREATE UNIQUE INDEX fast_ratchet_sender_keys_index ON " + this.FastRatchetSenderKeyTableName + " (group_id, sender_id)"
      };
      foreach (string sql in strArray)
      {
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          preparedStatement.Step();
      }
    }

    private bool RegisteredSelf
    {
      get
      {
        lock (this.databaseLock)
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM identities WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, "-1");
            return preparedStatement.Step();
          }
        }
      }
    }

    private bool SignedKeyGenerated
    {
      get
      {
        lock (this.databaseLock)
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM signed_prekeys"))
            return preparedStatement.Step();
        }
      }
    }

    public bool HasEverSentKeys
    {
      get
      {
        lock (this.databaseLock)
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM " + this.PreKeyTableName + " WHERE sent_to_server = 1"))
            return preparedStatement.Step();
        }
      }
    }

    public IByteBuffer SessionLoadSession(string RecipientId, int DeviceId)
    {
      this.CheckDevice(DeviceId);
      IByteBuffer bb = (IByteBuffer) null;
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT record FROM sessions WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, RecipientId);
          if (preparedStatement.Step())
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            bb.PutWithCopy(column);
          }
        }
      }
      return bb;
    }

    public IByteBuffer SessionGetSubDeviceSessions(string RecipientId) => (IByteBuffer) null;

    public void SessionStoreSession(string RecipientId, int DeviceId, IByteBuffer RecordBuffer)
    {
      this.CheckDevice(DeviceId);
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        database.BeginTransaction();
        try
        {
          bool flag = false;
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT id FROM sessions WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, RecipientId);
            flag = preparedStatement.Step();
          }
          if (flag)
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE sessions SET record = ? WHERE recipient_id = ?"))
            {
              preparedStatement.Bind(0, RecordBuffer.Get());
              preparedStatement.Bind(1, RecipientId);
              preparedStatement.Step();
            }
          }
          else
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT INTO sessions (recipient_id, timestamp, record) VALUES (?, ?, ?)"))
            {
              preparedStatement.Bind(0, RecipientId);
              preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
              preparedStatement.Bind(2, RecordBuffer.Get());
              preparedStatement.Step();
            }
          }
          database.CommitTransaction();
        }
        catch (Exception ex1)
        {
          try
          {
            database.RollbackTransaction(ex1, new Action(this.DisposeDatabase));
          }
          catch (Exception ex2)
          {
          }
        }
      }
    }

    public bool SessionContainsSession(string RecipientId)
    {
      return this.SessionContainsSession(RecipientId, 0);
    }

    public bool SessionContainsSession(string RecipientId, int DeviceId)
    {
      this.CheckDevice(DeviceId);
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM sessions WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, RecipientId);
          return preparedStatement.Step();
        }
      }
    }

    public void SessionDeleteSession(string RecipientId, int DeviceId)
    {
      this.CheckDevice(DeviceId);
      this.SessionDeleteAllSessions(RecipientId);
    }

    public void SessionDeleteAllSessions(string RecipientId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("DELETE FROM sessions WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, RecipientId);
          preparedStatement.Step();
        }
      }
    }

    public void SessionDestroy()
    {
    }

    public IByteBuffer PreKeyLoadPreKey(uint KeyId)
    {
      IByteBuffer bb = (IByteBuffer) null;
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT record FROM prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          if (preparedStatement.Step())
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            bb.PutWithCopy(column);
          }
        }
      }
      return bb;
    }

    public void PreKeyStorePreKey(uint KeyId, IByteBuffer RecordBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT INTO prekeys (prekey_id, sent_to_server, record) VALUES (?, ?, ?)"))
          {
            preparedStatement.Bind(0, (long) KeyId, false);
            preparedStatement.Bind(1, false);
            preparedStatement.Bind(2, RecordBuffer.Get());
            preparedStatement.Step();
          }
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
          throw;
        }
      }
    }

    public bool PreKeyContainsPreKey(uint KeyId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          return preparedStatement.Step();
        }
      }
    }

    public void PreKeyRemovePreKey(uint KeyId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("DELETE FROM prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          preparedStatement.Step();
        }
      }
    }

    public void PreKeyDestroy()
    {
    }

    public IByteBuffer SignedPreKeyLoadSignedPreKey(uint KeyId)
    {
      IByteBuffer bb = (IByteBuffer) null;
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT record FROM signed_prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          if (preparedStatement.Step())
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            bb.PutWithCopy(column);
          }
        }
      }
      return bb;
    }

    public void SignedPreKeyStoreSignedPreKey(uint KeyId, IByteBuffer RecordBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT OR REPLACE INTO signed_prekeys (prekey_id, timestamp, record) VALUES (?, ?, ?)"))
          {
            preparedStatement.Bind(0, (long) KeyId, false);
            preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
            preparedStatement.Bind(2, RecordBuffer.Get());
            preparedStatement.Step();
          }
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
          throw;
        }
      }
    }

    public bool SignedPreKeyContainsSignedPreKey(uint KeyId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM signed_prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          return preparedStatement.Step();
        }
      }
    }

    public void SignedPreKeyRemoveSignedPreKey(uint KeyId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("DELETE FROM signed_prekeys WHERE prekey_id = ?"))
        {
          preparedStatement.Bind(0, (long) KeyId, false);
          preparedStatement.Step();
        }
      }
    }

    public void SignedPreKeyDestroy()
    {
    }

    public void IdentityGetIdentityKeyPair(
      out IByteBuffer PublicKeyBuffer,
      out IByteBuffer PrivateKeyBuffer)
    {
      PublicKeyBuffer = (IByteBuffer) null;
      PrivateKeyBuffer = (IByteBuffer) null;
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT public_key, private_key FROM identities WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, "-1");
          if (preparedStatement.Step())
          {
            byte[] column1 = preparedStatement.Columns[0] as byte[];
            PublicKeyBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            PublicKeyBuffer.PutWithCopy(column1);
            byte[] column2 = preparedStatement.Columns[1] as byte[];
            PrivateKeyBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            PrivateKeyBuffer.PutWithCopy(column2);
          }
          else
          {
            Log.l("Axolotl", "Missing entry for self in identities table");
            throw new MissingMemberException();
          }
        }
      }
    }

    public uint IdentityGetLocalRegistrationId()
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT registration_id FROM identities WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, "-1");
          if (preparedStatement.Step())
            return (uint) (long) preparedStatement.Columns[0];
          Log.l("Axolotl", "Missing entry for self in identities table");
          throw new MissingMemberException();
        }
      }
    }

    public void IdentitySaveIdentity(string RecipientId, IByteBuffer KeyBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        database.BeginTransaction();
        try
        {
          bool flag1 = false;
          bool flag2 = false;
          byte[] a = (byte[]) null;
          byte[] numArray = KeyBuffer.Get();
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT public_key FROM identities WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, RecipientId);
            if (preparedStatement.Step())
            {
              flag1 = true;
              a = preparedStatement.Columns[0] as byte[];
            }
          }
          if (flag1)
          {
            if (numArray != null && !a.IsEqualBytes(numArray))
              flag2 = true;
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE identities SET public_key = ?, timestamp = ? WHERE recipient_id = ?"))
            {
              preparedStatement.Bind(0, numArray);
              preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
              preparedStatement.Bind(2, RecipientId);
              preparedStatement.Step();
            }
          }
          else
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT INTO identities (public_key, timestamp, recipient_id) VALUES (?, ?, ?)"))
            {
              preparedStatement.Bind(0, numArray);
              preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
              preparedStatement.Bind(2, RecipientId);
              preparedStatement.Step();
            }
          }
          if (flag2)
          {
            AppState.SchedulePersistentAction(PersistentAction.IdentityChangedForUser(RecipientId));
            AppState.QrPersistentAction.NotifyIdentityChanged(RecipientId);
          }
          database.CommitTransaction();
        }
        catch (Exception ex1)
        {
          try
          {
            database.RollbackTransaction(ex1, new Action(this.DisposeDatabase));
          }
          catch (Exception ex2)
          {
          }
        }
      }
    }

    public void IdentityClearIdentity(string RecipientId)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        database.BeginTransaction();
        try
        {
          bool flag = false;
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT id FROM identities WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, RecipientId);
            flag = preparedStatement.Step();
          }
          if (flag)
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE identities SET public_key = ?, timestamp = ? WHERE recipient_id = ?"))
            {
              preparedStatement.Bind(0, (byte[]) null);
              preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
              preparedStatement.Bind(2, RecipientId);
              preparedStatement.Step();
            }
          }
          else
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT INTO identities (public_key, timestamp, recipient_id) VALUES (?, ?, ?)"))
            {
              preparedStatement.Bind(0, (byte[]) null);
              preparedStatement.Bind(1, DateTime.Now.ToFileTimeUtc(), false);
              preparedStatement.Bind(2, RecipientId);
              preparedStatement.Step();
            }
          }
          database.CommitTransaction();
        }
        catch (Exception ex1)
        {
          try
          {
            database.RollbackTransaction(ex1, new Action(this.DisposeDatabase));
          }
          catch (Exception ex2)
          {
          }
        }
      }
    }

    public bool IdentityIsTrustedIdentity(string RecipientId, IByteBuffer PublicKeyBuffer) => true;

    public void IdentityDestroy()
    {
    }

    public byte[] IdentityGetPublicKey(string RecipientId, bool quiet = false)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT public_key FROM identities WHERE recipient_id = ?"))
        {
          preparedStatement.Bind(0, RecipientId);
          if (preparedStatement.Step())
            return preparedStatement.Columns[0] as byte[];
          Log.l("Axolotl", "Missing entry for recipient " + RecipientId + " in identities table");
          if (quiet)
            return (byte[]) null;
          throw new MissingMemberException();
        }
      }
    }

    public void IdentityGetPublicKey(string RecipientId, out IByteBuffer PublicKeyBuffer)
    {
      byte[] publicKey = this.IdentityGetPublicKey(RecipientId);
      PublicKeyBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
      PublicKeyBuffer.PutWithCopy(publicKey);
    }

    public IByteBuffer SenderKeyLoadSenderKey(string group, string senderId)
    {
      lock (this.databaseLock)
        return this.SenderKeyStoreGetSenderKey(group, senderId);
    }

    private IByteBuffer SenderKeyStoreGetSenderKey(string group, string senderId)
    {
      IByteBuffer bb = (IByteBuffer) null;
      Sqlite database = this.Database;
      bool flag = false;
      using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT record, creation_timestamp FROM " + this.SenderKeyTableName + " WHERE group_id = ? AND sender_id = ?"))
      {
        preparedStatement.Bind(0, group);
        preparedStatement.Bind(1, senderId);
        if (preparedStatement.Step())
        {
          if (JidHelper.IsSelfJid(senderId) && preparedStatement.Columns[1] != null)
          {
            long? column = preparedStatement.Columns[1] as long?;
            if (column.HasValue && column.Value < DateTime.UtcNow.AddDays(-30.0).ToUnixTime())
              flag = true;
          }
          if (!flag)
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            bb.PutWithCopy(column);
          }
        }
      }
      if (flag)
      {
        Log.d("E2EStore", "rotating sender key for {0}", (object) group);
        this.SenderKeyStoreRemoveKeyImpl(database, group, senderId);
      }
      return bb;
    }

    public void SenderKeyStoreSenderKey(string group, string senderId, IByteBuffer RecordBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        IByteBuffer senderKey = this.SenderKeyStoreGetSenderKey(group, senderId);
        try
        {
          if (senderKey != null)
          {
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE " + this.SenderKeyTableName + " SET record = ? WHERE group_id = ? AND sender_id = ? "))
            {
              byte[] val = RecordBuffer.Get();
              preparedStatement.Bind(0, val);
              preparedStatement.Bind(1, group);
              preparedStatement.Bind(2, senderId);
              preparedStatement.Step();
            }
          }
          else
          {
            long unixTime = DateTime.Now.ToUnixTime();
            using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT OR REPLACE INTO " + this.SenderKeyTableName + " (group_id, sender_id, record, creation_timestamp) VALUES (?, ?, ?, ?)"))
            {
              byte[] val = RecordBuffer.Get();
              preparedStatement.Bind(0, group);
              preparedStatement.Bind(1, senderId);
              preparedStatement.Bind(2, val);
              preparedStatement.Bind(3, unixTime, false);
              preparedStatement.Step();
            }
          }
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
          throw;
        }
      }
    }

    public bool SenderKeyStoreRemoveKey(string group, string senderId)
    {
      lock (this.databaseLock)
        return this.SenderKeyStoreRemoveKeyImpl(this.Database, group, senderId);
    }

    private bool SenderKeyStoreRemoveKeyImpl(Sqlite db, string group, string senderId)
    {
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement("DELETE FROM " + this.SenderKeyTableName + " WHERE group_id = ? AND sender_id = ?"))
      {
        preparedStatement.Bind(0, group);
        preparedStatement.Bind(1, senderId);
        return preparedStatement.Step();
      }
    }

    public bool SenderKeyContainsSenderKey(string group, string senderId)
    {
      lock (this.databaseLock)
        return this.SenderKeyStoreGetSenderKey(group, senderId) != null;
    }

    public void SenderKeyDestroy()
    {
    }

    public IByteBuffer FastRatchetSenderKeyLoadFastRatchetSenderKey(string group, string senderId)
    {
      IByteBuffer bb = (IByteBuffer) null;
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT record FROM " + this.FastRatchetSenderKeyTableName + " WHERE group_id = ? AND sender_id = ?"))
        {
          preparedStatement.Bind(0, group);
          preparedStatement.Bind(1, senderId);
          if (preparedStatement.Step())
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            bb = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            bb.PutWithCopy(column);
          }
        }
      }
      return bb;
    }

    public void FastRatchetSenderKeyStoreFastRatchetSenderKey(
      string group,
      string senderId,
      IByteBuffer RecordBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT OR REPLACE INTO " + this.FastRatchetSenderKeyTableName + " (group_id, sender_id, record) VALUES (?, ?, ?)"))
          {
            byte[] val = RecordBuffer.Get();
            preparedStatement.Bind(0, group);
            preparedStatement.Bind(1, senderId);
            preparedStatement.Bind(2, val);
            preparedStatement.Step();
          }
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
          throw;
        }
      }
    }

    public bool FastRatchetSenderKeyStoreRemoveKey(string group, string senderId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("DELETE FROM " + this.FastRatchetSenderKeyTableName + " WHERE group_id = ? AND sender_id = ?"))
        {
          preparedStatement.Bind(0, group);
          preparedStatement.Bind(1, senderId);
          return preparedStatement.Step();
        }
      }
    }

    public bool FastRatchetSenderKeyContainsFastRatchetSenderKey(string group, string senderId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT 1 FROM " + this.FastRatchetSenderKeyTableName + " WHERE group_id = ? AND sender_id = ?"))
        {
          preparedStatement.Bind(0, group);
          preparedStatement.Bind(1, senderId);
          return preparedStatement.Step();
        }
      }
    }

    public void FastRatchetSenderKeyDestroy()
    {
    }

    private void CheckDevice(int deviceId)
    {
      if (deviceId != 0)
      {
        Log.l("Axolotl", "Invalid DeviceId: Multiple device support not implemented");
        throw new ArgumentOutOfRangeException();
      }
    }

    public void IdentityRegisterSelf(
      int Registration,
      IByteBuffer PublicKeyBuffer,
      IByteBuffer PrivateKeyBuffer,
      int NextPreKeyId,
      int Timestamp)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("INSERT INTO " + this.IdentitiesTableName + " (recipient_id, registration_id, public_key, private_key, next_prekey_id, timestamp, next_signed_prekey_id)  VALUES (?, ?, ?, ?, ?, ?, ?)"))
          {
            preparedStatement.Bind(0, "-1");
            preparedStatement.Bind(1, Registration, false);
            preparedStatement.Bind(2, PublicKeyBuffer.Get());
            preparedStatement.Bind(3, PrivateKeyBuffer.Get());
            preparedStatement.Bind(4, NextPreKeyId, false);
            preparedStatement.Bind(5, DateTime.Now.ToFileTimeUtc(), false);
            preparedStatement.Bind(6, 0, false);
            preparedStatement.Step();
          }
        }
        catch (Exception ex)
        {
          this.DisposeDatabase();
        }
      }
    }

    public void IdentityDeleteSelf()
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        try
        {
          database.BeginTransaction();
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("DROP TABLE IF EXISTS " + this.IdentitiesTableName))
            preparedStatement.Step();
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("DROP TABLE IF EXISTS " + this.SignedPreKeyTableName))
            preparedStatement.Step();
          this.UpdateSchemaVersion(database, 0);
          database.CommitTransaction();
        }
        catch (Exception ex)
        {
          try
          {
            database.RollbackTransaction(ex);
          }
          finally
          {
            this.DisposeDatabase();
          }
        }
      }
    }

    public bool HasEnoughUnsentPreKeys => this.UnsentPreKeysCount >= Settings.MaxPreKeyBatchSize;

    public int UnsentPreKeysCount
    {
      get
      {
        lock (this.databaseLock)
        {
          using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT COUNT(*) FROM " + this.PreKeyTableName + " WHERE sent_to_server = 0"))
            return preparedStatement.Step() ? (int) (long) preparedStatement.Columns[0] : 0;
        }
      }
    }

    public void EnsureUnsentPreKeys()
    {
      int val2 = Settings.AxolotlRegistrationRetries == 0 ? 200 : 1;
      int batchSize;
      for (int val1 = Settings.MaxPreKeyBatchSize - this.UnsentPreKeysCount; val1 > 0; val1 -= batchSize)
      {
        batchSize = Math.Min(val1, val2);
        this.GeneratePreKeys(batchSize);
      }
    }

    public void GeneratePreKeys(int batchSize)
    {
      int NextPreKeyId = 0;
      lock (this.databaseLock)
      {
        Log.l("E2EStore", "Generating Self PreKeys");
        Sqlite database = this.Database;
        database.BeginTransaction();
        try
        {
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT next_prekey_id FROM " + this.IdentitiesTableName + " WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, "-1");
            NextPreKeyId = preparedStatement.Step() ? (int) (long) preparedStatement.Columns[0] : throw new InvalidOperationException();
          }
          int val = (NextPreKeyId + batchSize) % 16777214 + 1;
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE " + this.IdentitiesTableName + " SET next_prekey_id = ? WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, val, false);
            preparedStatement.Bind(1, "-1");
            preparedStatement.Step();
          }
          this.Axolotl.NativeInterface.GeneratePreKeys(NextPreKeyId, batchSize);
          database.CommitTransaction();
        }
        catch (Exception ex)
        {
          ++Settings.AxolotlRegistrationRetries;
          database.RollbackTransaction(ex, new Action(this.DisposeDatabase));
        }
      }
    }

    public AxolotlPreKey[] UnsentPreKeys
    {
      get
      {
        int limit = Settings.AxolotlRegistrationRetries == 0 ? 0 : 1;
        byte[] sourceArray = (byte[]) null;
        try
        {
          sourceArray = this.Axolotl.GetUnsentPreKeys(limit);
        }
        catch
        {
          ++Settings.AxolotlRegistrationRetries;
        }
        if (sourceArray == null)
          return (AxolotlPreKey[]) null;
        if (sourceArray.Length % 35 != 0)
          throw new InvalidOperationException();
        int length = sourceArray.Length / 35;
        int index = 0;
        AxolotlPreKey[] unsentPreKeys = new AxolotlPreKey[length];
        for (; index < length; ++index)
        {
          AxolotlPreKey axolotlPreKey = new AxolotlPreKey();
          axolotlPreKey.Id = new byte[3];
          Array.Copy((Array) sourceArray, index * 35, (Array) axolotlPreKey.Id, 0, 3);
          axolotlPreKey.Data = new byte[32];
          Array.Copy((Array) sourceArray, index * 35 + 3, (Array) axolotlPreKey.Data, 0, 32);
          unsentPreKeys[index] = axolotlPreKey;
        }
        return unsentPreKeys;
      }
    }

    public void UnsentPreKeysInDatabase(
      int limit,
      out int unsentPreKeysCount,
      out IByteBuffer unsentPreKeysBuffer)
    {
      lock (this.databaseLock)
      {
        Sqlite database = this.Database;
        MemoryStream memoryStream = new MemoryStream();
        int num = 0;
        string sql = "SELECT record FROM " + this.PreKeyTableName + " WHERE sent_to_server = 0" + (" LIMIT " + (limit > 0 ? limit.ToString() : Settings.MaxPreKeyBatchSize.ToString()));
        using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement(sql))
        {
          while (preparedStatement.Step())
          {
            byte[] column = preparedStatement.Columns[0] as byte[];
            int length = column.Length;
            memoryStream.WriteByte((byte) (length >> 24 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length >> 16 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length & (int) byte.MaxValue));
            memoryStream.Write(column, 0, column.Length);
            ++num;
          }
        }
        unsentPreKeysCount = num;
        unsentPreKeysBuffer = (IByteBuffer) null;
        if (num <= 0)
          return;
        unsentPreKeysBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
        unsentPreKeysBuffer.Put(memoryStream.ToArray());
      }
    }

    public void MarkAllPreKeysSentToServer()
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("UPDATE " + this.PreKeyTableName + " SET sent_to_server = 1 WHERE sent_to_server = 0"))
        {
          preparedStatement.Step();
          Log.l("E2E", "UnsentPreKeys marked as sent");
        }
      }
      Settings.AxolotlRegistrationRetries = 0;
    }

    public void MarkAllPreKeysUnsentToServer()
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("UPDATE " + this.PreKeyTableName + " SET sent_to_server = 0 WHERE sent_to_server = 1"))
        {
          preparedStatement.Step();
          Log.l("E2E", "UnsentPreKeys marked as unsent");
        }
      }
    }

    public uint LocalRegistrationId => this.IdentityGetLocalRegistrationId();

    public byte[] IdentityKeyForSending => this.Axolotl.GetIdentityKeyForSending();

    public AxolotlPreKey LatestSignedPreKey
    {
      get
      {
        byte[] latestSignedPreKey1 = this.Axolotl.GetLatestSignedPreKey();
        if (latestSignedPreKey1 == null)
          return (AxolotlPreKey) null;
        if (latestSignedPreKey1.Length != 99)
          throw new InvalidOperationException("SignedPreKey buffer length incorret");
        AxolotlPreKey latestSignedPreKey2 = new AxolotlPreKey();
        latestSignedPreKey2.Id = new byte[3];
        Array.Copy((Array) latestSignedPreKey1, 0, (Array) latestSignedPreKey2.Id, 0, 3);
        latestSignedPreKey2.Data = new byte[32];
        Array.Copy((Array) latestSignedPreKey1, 3, (Array) latestSignedPreKey2.Data, 0, 32);
        latestSignedPreKey2.Signature = new byte[64];
        Array.Copy((Array) latestSignedPreKey1, 35, (Array) latestSignedPreKey2.Signature, 0, 64);
        return latestSignedPreKey2;
      }
    }

    public IByteBuffer LatestSignedPreKeyInDatabase()
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT record FROM " + this.SignedPreKeyTableName + " ORDER BY timestamp DESC LIMIT 1"))
        {
          byte[] bytes = preparedStatement.Step() ? preparedStatement.Columns[0] as byte[] : throw new InvalidOperationException("Missing signed prekeys");
          ByteBuffer instance = NativeInterfaces.CreateInstance<ByteBuffer>();
          instance.Put(bytes);
          return (IByteBuffer) instance;
        }
      }
    }

    public void SaveMessageBaseKey(
      string keyRemoteJid,
      bool keyFromMe,
      string keyId,
      byte[] currentAliceKey)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("INSERT OR REPLACE INTO " + this.MessageBaseKeyTableName + " (msg_key_remote_jid, msg_key_from_me, msg_key_id, last_alice_base_key, timestamp) VALUES (?, ?, ?, ?, ?)"))
        {
          preparedStatement.Bind(0, keyRemoteJid);
          preparedStatement.Bind(1, keyFromMe);
          preparedStatement.Bind(2, keyId);
          preparedStatement.Bind(3, currentAliceKey);
          preparedStatement.Bind(4, DateTime.Now.ToFileTimeUtc(), false);
          preparedStatement.Step();
        }
      }
    }

    public bool IsSameBaseKey(
      string keyRemoteJid,
      bool keyFromMe,
      string keyId,
      byte[] currentAliceKey)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT last_alice_base_key FROM " + this.MessageBaseKeyTableName + " WHERE msg_key_remote_jid = ? AND msg_key_from_me = ? AND msg_key_id = ?"))
        {
          preparedStatement.Bind(0, keyRemoteJid);
          preparedStatement.Bind(1, keyFromMe);
          preparedStatement.Bind(2, keyId);
          return preparedStatement.Step() && (preparedStatement.Columns[0] as byte[]).IsEqualBytes(currentAliceKey);
        }
      }
    }

    public void DeleteMessageBaseKey(string keyRemoteJid, bool keyFromMe, string keyId)
    {
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("DELETE FROM " + this.MessageBaseKeyTableName + " WHERE msg_key_remote_jid = ? AND msg_key_from_me = ? AND msg_key_id = ?"))
        {
          preparedStatement.Bind(0, keyRemoteJid);
          preparedStatement.Bind(1, keyFromMe);
          preparedStatement.Bind(2, keyId);
          preparedStatement.Step();
        }
      }
    }

    public bool VerifyDigest(AxolotlDigest digest)
    {
      if (digest == null)
      {
        Log.l("E2EDigest", "Digest missing");
        return false;
      }
      if ((long) digest.registrationId != (long) this.LocalRegistrationId)
      {
        Log.l("E2EDigest", "RegistrationId mismatch");
        return false;
      }
      if (digest.type != (byte) 5)
      {
        Log.l("E2EDigest", "Type mismatch");
        return false;
      }
      AxolotlPreKey latestSignedPreKey = this.LatestSignedPreKey;
      if (latestSignedPreKey == null || digest.signedPreKeyId == null)
      {
        Log.l("E2EDigest", "SignedPreKey missing");
        return false;
      }
      if (!digest.signedPreKeyId.IsEqualBytes(latestSignedPreKey.Id))
      {
        Log.l("E2EDigest", "SignedPreKeyId mismatch");
        return false;
      }
      if (digest.preKeyIds == null)
      {
        Log.l("E2EDigest", "PreKeys missing");
        return false;
      }
      List<AxolotlPreKey> axolotlPreKeyList = this.SentPreKeysById(digest.preKeyIds);
      if (axolotlPreKeyList.Count != digest.preKeyIds.Count)
      {
        Log.l("E2EDigest", "PreKey count mismatch");
        return false;
      }
      MemoryStream memoryStream = new MemoryStream();
      byte[] identityKeyForSending = this.IdentityKeyForSending;
      if (identityKeyForSending == null)
      {
        Log.l("E2EDigest", "IdentityKey missing");
        return false;
      }
      if (latestSignedPreKey.Data == null || latestSignedPreKey.Signature == null)
      {
        Log.l("E2EDigest", "LatestSignedPreKey missing");
        return false;
      }
      memoryStream.Write(identityKeyForSending, 0, identityKeyForSending.Length);
      memoryStream.Write(latestSignedPreKey.Data, 0, latestSignedPreKey.Data.Length);
      memoryStream.Write(latestSignedPreKey.Signature, 0, latestSignedPreKey.Signature.Length);
      foreach (AxolotlPreKey axolotlPreKey in axolotlPreKeyList)
        memoryStream.Write(axolotlPreKey.Data, 0, axolotlPreKey.Data.Length);
      using (SHA1Managed shA1Managed = new SHA1Managed())
      {
        if (!shA1Managed.ComputeHash(memoryStream.ToArray()).IsEqualBytes(digest.digestHash))
        {
          Log.l("E2EDigest", "Digest hash mismatch");
          return false;
        }
      }
      return true;
    }

    public List<AxolotlPreKey> SentPreKeysById(List<int> ids)
    {
      MemoryStream memoryStream = new MemoryStream();
      IByteBuffer byteBuffer = (IByteBuffer) null;
      int Count = 0;
      List<AxolotlPreKey> axolotlPreKeyList = new List<AxolotlPreKey>();
      lock (this.databaseLock)
      {
        using (Sqlite.PreparedStatement preparedStatement = this.Database.PrepareStatement("SELECT prekey_id, record FROM " + this.PreKeyTableName + " WHERE sent_to_server = 1 AND prekey_id IN (" + string.Join<int>(", ", (IEnumerable<int>) ids) + ") ORDER BY prekey_id"))
        {
          while (preparedStatement.Step())
          {
            axolotlPreKeyList.Add(new AxolotlPreKey()
            {
              DigestId = (int) (long) preparedStatement.Columns[0]
            });
            byte[] column = preparedStatement.Columns[1] as byte[];
            int length = column.Length;
            memoryStream.WriteByte((byte) (length >> 24 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length >> 16 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length >> 8 & (int) byte.MaxValue));
            memoryStream.WriteByte((byte) (length & (int) byte.MaxValue));
            memoryStream.Write(column, 0, column.Length);
            ++Count;
          }
          if (Count > 0)
          {
            byteBuffer = (IByteBuffer) NativeInterfaces.CreateInstance<ByteBuffer>();
            byteBuffer.Put(memoryStream.ToArray());
          }
        }
      }
      if (byteBuffer != null)
      {
        byte[] sourceArray = this.Axolotl.NativeInterface.GetPreKeysDataFromRecord(byteBuffer, Count).Get();
        for (int index = 0; index < Count; ++index)
        {
          axolotlPreKeyList[index].Data = new byte[32];
          Array.Copy((Array) sourceArray, index * 32, (Array) axolotlPreKeyList[index].Data, 0, 32);
        }
      }
      return axolotlPreKeyList;
    }

    public bool CheckRotateSignedPreKey()
    {
      if (!Settings.LastSignedPreKeySent.HasValue)
        Settings.LastSignedPreKeySent = new DateTime?(DateTime.Now + TimeSpan.FromMinutes((double) new Random().Next(20160)));
      TimeSpan timeSpan1 = TimeSpan.FromDays(2.0);
      DateTime now = DateTime.Now;
      DateTime? signedPreKeySent = Settings.LastSignedPreKeySent;
      TimeSpan? nullable = signedPreKeySent.HasValue ? new TimeSpan?(now - signedPreKeySent.GetValueOrDefault()) : new TimeSpan?();
      TimeSpan timeSpan2 = timeSpan1;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() > timeSpan2 ? 1 : 0) : 0) == 0)
        return false;
      this.GenerateSignedPreKey();
      return true;
    }

    private void GenerateSignedPreKey()
    {
      lock (this.databaseLock)
      {
        Log.l("E2EStore", "Generating a new SignedPreKey");
        Sqlite database = this.Database;
        database.BeginTransaction();
        try
        {
          int NextSignedPreKeyId;
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("SELECT next_signed_prekey_id FROM " + this.IdentitiesTableName + " WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, "-1");
            NextSignedPreKeyId = preparedStatement.Step() ? (int) (long) preparedStatement.Columns[0] : throw new InvalidOperationException();
          }
          int val = (NextSignedPreKeyId + 1) % 16777215;
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("UPDATE " + this.IdentitiesTableName + " SET next_signed_prekey_id = ? WHERE recipient_id = ?"))
          {
            preparedStatement.Bind(0, val, false);
            preparedStatement.Bind(1, "-1");
            preparedStatement.Step();
          }
          this.Axolotl.NativeInterface.GenerateSignedPreKey(NextSignedPreKeyId);
          List<int> values = new List<int>();
          for (int index = 0; index < 5; ++index)
            values.Add((NextSignedPreKeyId + 16777215 - index) % 16777215);
          string str = string.Join<int>(", ", (IEnumerable<int>) values);
          using (Sqlite.PreparedStatement preparedStatement = database.PrepareStatement("DELETE FROM " + this.SignedPreKeyTableName + " WHERE prekey_id NOT IN (" + str + ")"))
            preparedStatement.Step();
          database.CommitTransaction();
        }
        catch (Exception ex)
        {
          Log.l("E2EStore", "FAILED Creating a new SignedPreKey");
          Log.SendCrashLog((Exception) new Axolotl.AxolotlRegistrationException(), "E2EStore FAILED Creating a new SignedPreKey", false);
          database.RollbackTransaction(ex, new Action(this.DisposeDatabase));
          throw;
        }
      }
    }

    public void Reset(Action onComplete = null)
    {
      lock (this.databaseLock)
      {
        bool flag = false;
        this.DisposeDatabase();
        try
        {
          using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
            storeForApplication.DeleteFile(this.DbName);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "Axolotl Reset Database");
          flag = true;
        }
        if (flag)
        {
          this.IdentityDeleteSelf();
          this.MarkAllPreKeysUnsentToServer();
        }
        this.InitializeDatabase();
        this.Initialize();
        this.Axolotl.SendPreKeysToServer(onComplete);
      }
    }
  }
}
