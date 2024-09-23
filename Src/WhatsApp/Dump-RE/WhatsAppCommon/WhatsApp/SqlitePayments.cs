// Decompiled with JetBrains decompiler
// Type: WhatsApp.SqlitePayments
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
  public class SqlitePayments
  {
    public static MutexWithWatchdog Lock = new MutexWithWatchdog("WhatsApp.SqlitePaymentsLock");
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string rollbackTxStmt = "ROLLBACK TRANSACTION";
    private const int LatestSchemaVersion = 2;
    private static readonly string DbPath = Constants.IsoStorePath + "\\payments.db";
    private const string PayMethodsTab = "paymethods";
    private const string PMCredentialsCol = "cred";
    private const string PMReadableName = "rname";
    private const string PMPayType = "ptype";
    private const string PMPaySubType = "psubtype";
    private const string PMUsageMode = "payumode";
    private const int PM_USAGE_PRIMARY = 1;
    private const int PM_USAGE_NONE = 0;
    private const string PMOutUsageMode = "outumode";
    private const string PMCurrencyIso4217 = "currIso4217";
    private const string PMBalance = "bal";
    private const string PMBalanceTimestamp = "baltime";
    private const string PMFirstName = "firstName";
    private const string PMSecondName = "secondName";
    private const string PMCountryCodeIso3166 = "cc";
    private const string PayTransactionsTab = "paytransactions";
    private static readonly string PayTransactionFMsgIndex = "paytranmsgindex";
    private static readonly string PayTransactionTranidIndex = "paytrantranidindex";

    public static bool HasPaymentsDatabase()
    {
      using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
        return nativeMediaStorage.FileExists(SqlitePayments.DbPath);
    }

    private static void PerformWithDbUnlocked(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      if (!write && !SqlitePayments.HasPaymentsDatabase())
        return;
      try
      {
        Sqlite db;
        try
        {
          db = new Sqlite(SqlitePayments.DbPath, write ? SqliteOpenFlags.Defaults : SqliteOpenFlags.READONLY);
        }
        catch (Exception ex)
        {
          if (!write && (int) ex.GetHResult() == (int) Sqlite.HRForError(14U))
            return;
          throw;
        }
        using (db)
        {
          int currentSchema = SqlitePayments.GetSchemaVersion(db);
          switch (currentSchema)
          {
            case -1:
              if (currentSchema != -1)
                SqlitePayments.DeleteTables(db, currentSchema);
              currentSchema = SqlitePayments.CreateTables(db);
              break;
            case 2:
label_13:
              onDb(db);
              return;
            default:
              if (currentSchema <= 2)
                break;
              goto case -1;
          }
          SqlitePayments.SchemaUpdate(db, currentSchema);
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
                nativeMediaStorage.DeleteFile(SqlitePayments.DbPath);
              }
              catch (Exception ex2)
              {
              }
            }
            SqlitePayments.PerformWithDbUnlocked(onDb, write, false);
          }
          else if ((int) hresult == (int) Sqlite.HRForError(8U))
          {
            SqlitePayments.PerformWithDbUnlocked((Action<Sqlite>) (db => { }), true);
            SqlitePayments.PerformWithDbUnlocked(onDb, write, false);
          }
          else
            throw;
        }
      }
    }

    private static void PerformWithDb(Action<Sqlite> onDb, bool write = false, bool retry = true)
    {
      SqlitePayments.Lock.PerformWithLock((Action) (() => SqlitePayments.PerformWithDbUnlocked(onDb, write, retry)));
    }

    private static void PerformWithDbWrite(Action<Sqlite> onDb)
    {
      SqlitePayments.PerformWithDb(onDb, true);
    }

    private static void RunStatement(Sqlite db, string stmtString)
    {
      string[] strArray = new string[1]{ stmtString };
      SqlitePayments.RunStatements(db, (IEnumerable<string>) strArray);
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
      SqlitePayments.RunStatement(db, SqlitePayments.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "CREATE TABLE IF NOT EXISTS metadata (version INTEGER)",
          "INSERT INTO metadata VALUES (" + (object) tables + ")",
          "CREATE TABLE IF NOT EXISTS paymethods\n(cred TEXT NOT NULL PRIMARY KEY, rname TEXT, ptype INTEGER NOT NULL, psubtype INTEGER, payumode INTEGER NOT NULL, outumode INTEGER NOT NULL, cc TEXT, currIso4217 TEXT, bal TEXT, baltime INTEGER, firstName TEXT, secondName TEXT )"
        };
        stringList.Add(SqlitePayments.commitTxStmt);
        SqlitePayments.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
        return tables;
      }
      catch (Exception ex)
      {
        SqlitePayments.RunStatement(db, SqlitePayments.rollbackTxStmt);
        throw;
      }
    }

    private static void SchemaUpdate(Sqlite db, int currentSchema)
    {
      if (currentSchema >= 2)
        return;
      int num = 2;
      Log.l("paydb", "running schema update {0}", (object) num);
      SqlitePayments.RunStatement(db, SqlitePayments.beginTxStmt);
      try
      {
        List<string> stringList = new List<string>()
        {
          "DELETE FROM metadata",
          "INSERT OR REPLACE INTO metadata VALUES (" + (object) num + ")",
          "DROP INDEX IF EXISTS " + SqlitePayments.PayTransactionFMsgIndex,
          "DROP INDEX IF EXISTS " + SqlitePayments.PayTransactionTranidIndex,
          "DROP TABLE IF EXISTS paytransactions"
        };
        stringList.Add(SqlitePayments.commitTxStmt);
        SqlitePayments.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqlitePayments.RunStatement(db, SqlitePayments.rollbackTxStmt);
        throw;
      }
    }

    private static void DeleteTables(Sqlite db, int currentSchema)
    {
      if (currentSchema < 1)
        return;
      try
      {
        List<string> stringList = new List<string>()
        {
          SqlitePayments.beginTxStmt,
          "DROP TABLE IF EXISTS metadata",
          "DROP TABLE IF EXISTS paymethods",
          "DROP INDEX IF EXISTS " + SqlitePayments.PayTransactionFMsgIndex,
          "DROP INDEX IF EXISTS " + SqlitePayments.PayTransactionTranidIndex,
          "DROP TABLE IF EXISTS paytransactions",
          SqlitePayments.commitTxStmt
        };
        SqlitePayments.RunStatements(db, (IEnumerable<string>) stringList);
        stringList.Clear();
      }
      catch (Exception ex)
      {
        SqlitePayments.RunStatement(db, SqlitePayments.rollbackTxStmt);
        throw;
      }
    }

    public static void DeleteDb()
    {
      SqlitePayments.Lock.PerformWithLock((Action) (() =>
      {
        try
        {
          using (NativeMediaStorage nativeMediaStorage = new NativeMediaStorage())
            nativeMediaStorage.DeleteFile(SqlitePayments.DbPath);
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

    public static void StorePaymentsMethod(PaymentsMethod paymentMethod)
    {
      Log.d("paydb", "Adding/Updating payment: {0} {1} {2}", (object) paymentMethod.CredentialId, (object) paymentMethod.ReadableName, (object) paymentMethod.PaymentType);
      SqlitePayments.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "INSERT OR REPLACE INTO paymethods (cred, rname, ptype, psubtype, payumode, outumode, currIso4217, bal, baltime, firstName, secondName, cc) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? )";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[12]
          {
            (object) paymentMethod.CredentialId,
            (object) paymentMethod.ReadableName,
            (object) (long) paymentMethod.PaymentType,
            (object) paymentMethod.PaymentSubType,
            (object) (paymentMethod.isDefaultPayment() ? 1 : 0),
            (object) (paymentMethod.isDefaultPayout() ? 1 : 0),
            paymentMethod.PaymentType == PaymentsMethod.PaymentTypes.Wallet ? (object) paymentMethod.CurrencyIso4217 : (object) (string) null,
            (object) "",
            null,
            paymentMethod.PaymentType == PaymentsMethod.PaymentTypes.Wallet ? (object) paymentMethod.FirstName : (object) (string) null,
            paymentMethod.PaymentType == PaymentsMethod.PaymentTypes.Wallet ? (object) paymentMethod.SecondName : (object) (string) null,
            (object) paymentMethod.CountryCodeIso3166
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void UpdateBalanceForPaymentsMethod(PaymentsMethod paymentMethod)
    {
      Log.d("paydb", "Updating balance: {0} {1} {2}", (object) paymentMethod.CredentialId, (object) paymentMethod.ReadableName, (object) paymentMethod.PaymentType);
      SqlitePayments.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "UPDATE paymethods SET bal = ?, baltime = ?, WHERE cred = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[3]
          {
            (object) paymentMethod.Balance.ToString(),
            (object) paymentMethod.BalanceDate.ToUnixTime(),
            (object) paymentMethod.CredentialId
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static void RemovePaymentsMethod(PaymentsMethod paymentMethod)
    {
      Log.d("paydb", "Removing Payment Method: {0} {1} {2}", (object) paymentMethod.CredentialId, (object) paymentMethod.ReadableName, (object) paymentMethod.PaymentType);
      SqlitePayments.PerformWithDbWrite((Action<Sqlite>) (db =>
      {
        string sql = "DELETE FROM paymethods WHERE cred = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[1]
          {
            (object) paymentMethod.CredentialId
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }

    public static List<PaymentsMethod> GetPaymentsMethods(string credential = null)
    {
      Log.d("paydb", "Getting payments: {0}", (object) (credential ?? "null"));
      try
      {
        List<PaymentsMethod> returnList = new List<PaymentsMethod>();
        SqlitePayments.PerformWithDbWrite((Action<Sqlite>) (db =>
        {
          string sql = "SELECT cred, rname, ptype, psubtype, payumode, outumode, currIso4217, bal, baltime, firstName, secondName, cc FROM paymethods";
          string str = "";
          List<string> source = new List<string>();
          if (!string.IsNullOrEmpty(credential))
          {
            str += "cred = ? ";
            source.Add(credential);
          }
          if (source.Count<string>() > 0)
            sql = sql + " WHERE " + str;
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            for (int index = 0; index < source.Count<string>(); ++index)
              preparedStatement.Bind(index, source[index]);
            while (preparedStatement.Step())
            {
              PaymentsMethod.PaymentTypes payType = PaymentsMethod.PaymentTypes.Unknown;
              switch ((long) preparedStatement.Columns[2] - 1L)
              {
                case 0:
                  payType = PaymentsMethod.PaymentTypes.Wallet;
                  break;
                case 1:
                  payType = PaymentsMethod.PaymentTypes.CreditCard;
                  break;
                case 2:
                  payType = PaymentsMethod.PaymentTypes.DebitCard;
                  break;
                case 3:
                  payType = PaymentsMethod.PaymentTypes.BankAccount;
                  break;
              }
              PaymentsMethod paymentsMethod = new PaymentsMethod((string) preparedStatement.Columns[0], (string) preparedStatement.Columns[1], payType, (int) (long) preparedStatement.Columns[3], (long) preparedStatement.Columns[4] == 1L, (long) preparedStatement.Columns[5] == 1L, (string) preparedStatement.Columns[11]);
              if (payType == PaymentsMethod.PaymentTypes.Wallet)
              {
                paymentsMethod.CurrencyIso4217 = (string) preparedStatement.Columns[6];
                if (!string.IsNullOrEmpty((string) preparedStatement.Columns[7]))
                  paymentsMethod.UpdateBalance((long) preparedStatement.Columns[7], preparedStatement.Columns[8] == null ? FunXMPP.UnixEpoch.ToUnixTime() : (long) preparedStatement.Columns[8]);
                if (!string.IsNullOrEmpty((string) preparedStatement.Columns[9]) || !string.IsNullOrEmpty((string) preparedStatement.Columns[10]))
                  paymentsMethod.setName((string) preparedStatement.Columns[9], (string) preparedStatement.Columns[10]);
              }
              returnList.Add(paymentsMethod);
            }
          }
        }));
        return returnList;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception returning payment methods");
        return (List<PaymentsMethod>) null;
      }
    }
  }
}
