// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentTransactionStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public static class PaymentTransactionStore
  {
    private const string LogHdr = "PTStore";
    private const string PayTransactionsTab = "pay_transactions";
    private const string PTFmsgRemoteJid = "key_remote_jid";
    private const string PTFmsgFromMe = "key_from_me";
    private const string PTFmsgId = "key_id";
    private const string PTTranId = "tran_id";
    private const string PTTimestamp = "timestamp";
    private const string PTTranStatus = "status";
    private const string PTErrorCode = "error_code";
    private const string PTSenderJid = "sender";
    private const string PTReceiverJid = "receiver";
    private const string PTTranType = "type";
    private const string PTCurrency = "currency";
    private const string PTAmountX1000 = "amount_1000";
    private const string PTCredentialId = "credential_id";
    private const string PTTranMethods = "methods";
    private const string PTBankTxnId = "bank_transaction_id";
    private const string PTMetaData = "metadata";
    private static readonly string PayTranTabMsgIndex = "message_payment_transactions_index";
    private static readonly string PayTranTabTranIndex = "message_payment_transactions_id_index";
    private static readonly string beginTxStmt = "BEGIN TRANSACTION";
    private static readonly string commitTxStmt = "COMMIT TRANSACTION";
    private static readonly string CREATE_TRANSACTIONS_TABLE = "CREATE TABLE pay_transactions (key_remote_jid TEXT, key_from_me INTEGER, key_id TEXT, tran_id TEXT, timestamp INTEGER, status INTEGER, error_code TEXT, sender TEXT, receiver TEXT, type INTEGER, currency TEXT, amount_1000 INTEGER, credential_id TEXT, methods BLOB, bank_transaction_id TEXT, metadata BLOB)";
    private static readonly string CREATE_TRANSACTIONS_TABLE_MSG_ID_INDEX = "CREATE UNIQUE INDEX " + PaymentTransactionStore.PayTranTabMsgIndex + " ON pay_transactions (key_id)";
    private static readonly string CREATE_TRANSACTIONS_TABLE_ID_INDEX = "CREATE UNIQUE INDEX " + PaymentTransactionStore.PayTranTabTranIndex + " ON pay_transactions (tran_id)";

    public static void CreateTransactionTable(MessagesContext db)
    {
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(PaymentTransactionStore.CREATE_TRANSACTIONS_TABLE))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(PaymentTransactionStore.CREATE_TRANSACTIONS_TABLE_MSG_ID_INDEX))
        preparedStatement.Step();
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(PaymentTransactionStore.CREATE_TRANSACTIONS_TABLE_ID_INDEX))
        preparedStatement.Step();
    }

    public static PaymentTransactionInfo GetTransaction(string transId)
    {
      PaymentTransactionInfo returnTran = (PaymentTransactionInfo) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => returnTran = db.GetMessagePaymentInfoImpl((string) null, transId)));
      return returnTran;
    }

    private static PaymentTransactionInfo GetMessagePaymentInfoImpl(
      this MessagesContext db,
      string keyid,
      string transId)
    {
      PaymentTransactionInfo messagePaymentInfoImpl = (PaymentTransactionInfo) null;
      if (string.IsNullOrEmpty(keyid) && string.IsNullOrEmpty(transId))
        throw new InvalidOperationException("Must supply id and/or tran id");
      string sql = "SELECT * FROM pay_transactions WHERE ";
      List<string> source = new List<string>();
      if (!string.IsNullOrEmpty(keyid))
      {
        sql += "key_id = ?";
        source.Add(keyid);
      }
      if (!string.IsNullOrEmpty(transId))
      {
        if (!string.IsNullOrEmpty(keyid))
          sql += " OR ";
        sql += "tran_id = ?";
        source.Add(transId);
      }
      using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
      {
        for (int index = 0; index < source.Count<string>(); ++index)
          preparedStatement.Bind(index, source[index]);
        while (preparedStatement.Step())
          messagePaymentInfoImpl = PaymentTransactionStore.CreateTransactionFromRow(preparedStatement.ColumnNames, preparedStatement.Columns);
      }
      return messagePaymentInfoImpl;
    }

    public static List<PaymentTransactionInfo> GetTransactions(
      DateTime? beforeDateTime = null,
      string transactionId = null,
      string fmsgId = null,
      string fmsgRemoteJid = null,
      int limit = 100)
    {
      Log.d("PTStore", "Getting transactions: {0} {1} {2} {3} {4}", beforeDateTime.HasValue ? (object) beforeDateTime.ToString() : (object) "*", (object) transactionId, (object) fmsgId, (object) fmsgRemoteJid, (object) limit);
      try
      {
        List<PaymentTransactionInfo> returnList = new List<PaymentTransactionInfo>();
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          string sqlSelect = "SELECT * FROM pay_transactions";
          List<string> whereBindings = new List<string>();
          Action<string, string> action = (Action<string, string>) ((column, value) =>
          {
            sqlSelect = whereBindings.Count <= 0 ? sqlSelect + " WHERE " : sqlSelect + " AND ";
            sqlSelect = sqlSelect + column + " = ?";
            whereBindings.Add(value);
          });
          if (beforeDateTime.HasValue)
            action("timestamp", beforeDateTime.Value.ToUnixTime().ToString());
          if (!string.IsNullOrEmpty(transactionId))
            action("tran_id", transactionId);
          if (!string.IsNullOrEmpty(fmsgId))
            action("key_id", fmsgId);
          if (!string.IsNullOrEmpty(fmsgRemoteJid))
            action("key_remote_jid", fmsgRemoteJid);
          sqlSelect += " ORDER BY timestamp DESC";
          if (limit > 0)
            sqlSelect = sqlSelect + " LIMIT " + (object) limit;
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sqlSelect))
          {
            for (int index = 0; index < whereBindings.Count<string>(); ++index)
              preparedStatement.Bind(index, whereBindings[index]);
            while (preparedStatement.Step())
              returnList.Add(PaymentTransactionStore.CreateTransactionFromRow(preparedStatement.ColumnNames, preparedStatement.Columns));
          }
        }));
        return returnList;
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "PTStore Exception returning payment transactions");
        return (List<PaymentTransactionInfo>) null;
      }
    }

    private static PaymentTransactionInfo CreateTransactionFromRow(
      string[] columnNames,
      object[] columnValues)
    {
      PaymentTransactionInfo.TransactionTypes transactionTypes = PaymentTransactionInfo.TransactionTypes.cashin;
      string str1 = (string) null;
      long amtx1000 = 0;
      string currency = (string) null;
      long ts = 0;
      PaymentTransactionInfo.TransactionStatuses status = PaymentTransactionInfo.TransactionStatuses.PENDING;
      string fmsgKeyJid = (string) null;
      bool flag = false;
      string fmsgKeyId = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      byte[] methodSummaries = (byte[]) null;
      string str4 = (string) null;
      string str5 = (string) null;
      byte[] numArray = (byte[]) null;
      string str6 = (string) null;
      for (int index = 0; index < columnNames.Length; ++index)
      {
        string columnName1 = columnNames[index];
        try
        {
          string columnName2 = columnNames[index];
          // ISSUE: reference to a compiler-generated method
          switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(columnName2))
          {
            case 352063879:
              if (columnName2 == "error_code")
              {
                str6 = (string) columnValues[index];
                continue;
              }
              break;
            case 910995747:
              if (columnName2 == "key_remote_jid")
              {
                fmsgKeyJid = (string) columnValues[index];
                continue;
              }
              break;
            case 921221376:
              if (columnName2 == "Id")
                continue;
              break;
            case 1361572173:
              if (columnName2 == "type")
              {
                transactionTypes = (PaymentTransactionInfo.TransactionTypes) (long) columnValues[index];
                continue;
              }
              break;
            case 1551571952:
              if (columnName2 == "key_from_me")
              {
                flag = (bool) columnValues[index];
                continue;
              }
              break;
            case 1713525568:
              if (columnName2 == "metadata")
              {
                numArray = (byte[]) columnValues[index];
                continue;
              }
              break;
            case 1789683312:
              if (columnName2 == "receiver")
              {
                str3 = (string) columnValues[index];
                continue;
              }
              break;
            case 2127111244:
              if (columnName2 == "key_id")
              {
                fmsgKeyId = (string) columnValues[index];
                continue;
              }
              break;
            case 2152947512:
              if (columnName2 == "bank_transaction_id")
              {
                str5 = (string) columnValues[index];
                continue;
              }
              break;
            case 2673797988:
              if (columnName2 == "sender")
              {
                str2 = (string) columnValues[index];
                continue;
              }
              break;
            case 2861730067:
              if (columnName2 == "amount_1000")
              {
                amtx1000 = (long) columnValues[index];
                continue;
              }
              break;
            case 2994984227:
              if (columnName2 == "timestamp")
              {
                ts = columnValues[index] != null ? (long) columnValues[index] : 0L;
                continue;
              }
              break;
            case 3125508079:
              if (columnName2 == "status")
              {
                status = (PaymentTransactionInfo.TransactionStatuses) (long) columnValues[index];
                continue;
              }
              break;
            case 3623493120:
              if (columnName2 == "tran_id")
              {
                str1 = (string) columnValues[index];
                continue;
              }
              break;
            case 3723658806:
              if (columnName2 == "currency")
              {
                currency = (string) columnValues[index];
                continue;
              }
              break;
            case 3794026105:
              if (columnName2 == "methods")
              {
                methodSummaries = (byte[]) columnValues[index];
                continue;
              }
              break;
            case 4031181568:
              if (columnName2 == "credential_id")
              {
                str4 = (string) columnValues[index];
                continue;
              }
              break;
          }
          throw new ArgumentException(string.Format("Unexpected column in {0} - {1}", (object) "pay_transactions", (object) columnNames[index]));
        }
        catch (Exception ex)
        {
          string context = "PTStore Exception parsing transaction row - column " + columnNames[index] + " " + (object) index + ")";
          Log.LogException(ex, context);
        }
      }
      PaymentTransactionInfo transactionFromRow;
      switch (transactionTypes)
      {
        case PaymentTransactionInfo.TransactionTypes.p2p:
          transactionFromRow = PaymentTransactionInfo.CreateUninvolvedPaymentTransaction(amtx1000, currency, fmsgKeyJid, fmsgKeyId, str2, str3);
          transactionFromRow.UpdateStoredDetails(str1, status, ts);
          break;
        case PaymentTransactionInfo.TransactionTypes.cashin:
          transactionFromRow = PaymentTransactionInfo.CreateCashInTransaction((string) null, str1, amtx1000, currency, methodSummaries, status, ts);
          break;
        case PaymentTransactionInfo.TransactionTypes.cashout:
          transactionFromRow = PaymentTransactionInfo.CreateCashOutTransaction((string) null, str1, amtx1000, currency, methodSummaries, status, ts);
          break;
        case PaymentTransactionInfo.TransactionTypes.p2pin:
          transactionFromRow = PaymentTransactionInfo.CreateReceivingPaymentTransaction(amtx1000, currency, methodSummaries, fmsgKeyJid, fmsgKeyId, str3);
          transactionFromRow.UpdateStoredDetails(str1, status, ts);
          break;
        case PaymentTransactionInfo.TransactionTypes.p2pout:
          transactionFromRow = PaymentTransactionInfo.CreateSendingPaymentTransaction(amtx1000, currency, methodSummaries, fmsgKeyJid, fmsgKeyId, str2);
          transactionFromRow.UpdateStoredDetails(str1, status, ts);
          break;
        default:
          throw new ArgumentException(string.Format("Unexpected transaction type found converting row {0}", (object) transactionTypes));
      }
      transactionFromRow.CredentialId = str4;
      transactionFromRow.BankXtranId = str5;
      transactionFromRow.MetaData = numArray;
      transactionFromRow.TransactionErrorCode = str6;
      transactionFromRow.FMsgFromMe = flag;
      return transactionFromRow;
    }

    public static bool StoreTransaction(PaymentTransactionInfo paymentTransaction)
    {
      bool returnResult = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => returnResult = db.InsertTransaction(paymentTransaction)));
      return returnResult;
    }

    private static bool InsertTransaction(
      this MessagesContext db,
      PaymentTransactionInfo paymentTransaction)
    {
      try
      {
        string sql = "INSERT INTO pay_transactions (key_remote_jid, key_from_me, key_id, tran_id, timestamp, status, error_code, sender, receiver, type, currency, amount_1000, credential_id, methods, bank_transaction_id, metadata) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ? )";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[16]
          {
            (object) paymentTransaction.FMsgKeyRemoteJid,
            (object) paymentTransaction.FMsgFromMe,
            (object) paymentTransaction.FMsgKeyId,
            (object) paymentTransaction.TransactionId,
            (object) paymentTransaction.TransactionDate,
            (object) (int) paymentTransaction.TransactionStatus,
            (object) paymentTransaction.TransactionErrorCode,
            (object) (paymentTransaction.SenderJid ?? ""),
            (object) (paymentTransaction.ReceiverJid ?? ""),
            (object) (int) paymentTransaction.TransactionType,
            (object) (paymentTransaction.TransactionCurrency ?? ""),
            (object) paymentTransaction.TransactionAmountx1000,
            (object) paymentTransaction.CredentialId,
            (object) paymentTransaction.paymentMethodSourceBytes,
            (object) paymentTransaction.BankXtranId,
            (object) paymentTransaction.MetaData
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
          return true;
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "PTStoreException inserting transaction");
        return false;
      }
    }

    public static bool UpdateTransactionTranid(
      string tranId,
      long tranTs,
      string tranStatus,
      string groupJid,
      string receiverJid,
      string senderJid,
      string msgId,
      long tranAmountx1000,
      string tranCurrency)
    {
      Log.l("PTStore", "Adding/Updating transactions: {0} {1} {2}", (object) tranId, (object) msgId, (object) tranStatus);
      PaymentTransactionInfo.TransactionStatuses statusEnum = PaymentTransactionInfo.GetStatusAsEnum(tranStatus);
      bool returnValue = false;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        PaymentTransactionInfo paymentTransactionInfo = (PaymentTransactionInfo) null;
        if (!string.IsNullOrEmpty(tranId))
          paymentTransactionInfo = db.GetMessagePaymentInfoImpl((string) null, tranId);
        if (paymentTransactionInfo == null)
        {
          Log.l("PTStore", "updating {0} with id {1} {2}", (object) msgId, (object) "tran_id", (object) statusEnum);
          string sql = "UPDATE OR FAIL pay_transactions SET tran_id = ?, timestamp = ?, status = ?, amount_1000 = ?, currency = ? WHERE receiver = ? AND sender = ? AND key_id = ?";
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            int num = 0;
            object[] objArray = new object[8]
            {
              (object) tranId,
              (object) tranTs,
              (object) (int) statusEnum,
              (object) tranAmountx1000,
              (object) tranCurrency,
              (object) receiverJid,
              (object) senderJid,
              (object) msgId
            };
            foreach (object o in objArray)
              preparedStatement.BindObject(num++, o);
            preparedStatement.Step();
            if (db.GetChangeCount() <= 0)
              return;
            Log.d("PTStore", "Updated transaction");
            returnValue = true;
          }
        }
        else
        {
          Log.l("PTStore", "updating existing {0}, {1} {2}", (object) msgId, (object) tranId, (object) statusEnum);
          string sql = "UPDATE OR FAIL pay_transactions SET timestamp = ?, status = ?, amount_1000 = ?, currency = ? WHERE tran_id = ?";
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            int num = 0;
            object[] objArray = new object[5]
            {
              (object) tranTs,
              (object) (int) statusEnum,
              (object) tranAmountx1000,
              (object) tranCurrency,
              (object) tranId
            };
            foreach (object o in objArray)
              preparedStatement.BindObject(num++, o);
            preparedStatement.Step();
            if (db.GetChangeCount() <= 0)
              return;
            Log.d("PTStore", "Updated transaction");
            returnValue = true;
          }
        }
      }));
      return returnValue;
    }

    public static void UpdateTransaction(
      FunXMPP.Connection.PaymentsTransactionResponse paymentTransactions)
    {
      PaymentTransactionStore.UpdateTransaction(new List<FunXMPP.Connection.PaymentsTransactionResponse>()
      {
        paymentTransactions
      });
    }

    public static void UpdateTransaction(
      List<FunXMPP.Connection.PaymentsTransactionResponse> paymentTransactions)
    {
      Log.l("PTStore", "Adding/Updating transactions: {0}", (object) paymentTransactions.Count<FunXMPP.Connection.PaymentsTransactionResponse>());
      List<PaymentTransactionInfo> paymentTransactionInfoList = new List<PaymentTransactionInfo>();
      foreach (FunXMPP.Connection.PaymentsTransactionResponse paymentTransaction in paymentTransactions)
      {
        PaymentTransactionInfo transaction = PaymentTransactionStore.GetTransaction(paymentTransaction.TranId);
        if (transaction != null)
          paymentTransactionInfoList.Add(transaction);
      }
      Log.d("PTStore", "Found {0} matching transactions", (object) paymentTransactionInfoList.Count);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        foreach (FunXMPP.Connection.PaymentsTransactionResponse paymentTransaction in paymentTransactions)
        {
          if (paymentTransaction.TranId != null)
          {
            string sqlUpate = "UPDATE OR FAIL pay_transactions SET ";
            List<object> updateBinds = new List<object>();
            Action<string, object> action = (Action<string, object>) ((column, value) =>
            {
              if (updateBinds.Count > 0)
                sqlUpate += ", ";
              sqlUpate = sqlUpate + column + " = ?";
              updateBinds.Add(value);
            });
            if (paymentTransaction.Ts > 0L)
              action("timestamp", (object) paymentTransaction.Ts);
            if (paymentTransaction.Status != null)
            {
              PaymentTransactionInfo.TransactionStatuses statusAsEnum = PaymentTransactionInfo.GetStatusAsEnum(paymentTransaction.Status);
              action("status", (object) statusAsEnum);
            }
            action("amount_1000", (object) paymentTransaction.TranAmountx1000);
            if (paymentTransaction.TranCurrency != null)
              action("currency", (object) paymentTransaction.TranCurrency);
            if (paymentTransaction.SenderJid != null)
              action("sender", (object) paymentTransaction.SenderJid);
            if (paymentTransaction.ReceiverJid != null)
              action("receiver", (object) paymentTransaction.ReceiverJid);
            if (paymentTransaction.MessageId != null)
              action("key_id", (object) paymentTransaction.MessageId);
            if (updateBinds.Count >= 1)
            {
              sqlUpate += " WHERE tran_id + ?";
              updateBinds.Add((object) paymentTransaction.TranId);
              using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sqlUpate))
              {
                int num = 0;
                foreach (object o in updateBinds)
                  preparedStatement.BindObject(num++, o);
                preparedStatement.Step();
                if (preparedStatement.Count > 0)
                {
                  Log.d("PTStore", "Updated transaction row");
                  continue;
                }
              }
              string sql = "INSERT INTO pay_transactions (key_remote_jid, key_id, tran_id, timestamp, status, sender, receiver, type, currency, amount_1000) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ? )";
              using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
              {
                PaymentTransactionInfo.TransactionStatuses statusAsEnum = PaymentTransactionInfo.GetStatusAsEnum(paymentTransaction.Status);
                PaymentTransactionInfo.TransactionTypes typeAsEnum = PaymentTransactionInfo.GetTypeAsEnum(paymentTransaction.TranType);
                object[] objArray = new object[10]
                {
                  null,
                  null,
                  (object) paymentTransaction.TranId,
                  (object) paymentTransaction.Ts,
                  (object) (int) statusAsEnum,
                  (object) (paymentTransaction.SenderJid ?? ""),
                  (object) (paymentTransaction.ReceiverJid ?? ""),
                  (object) (int) typeAsEnum,
                  (object) (paymentTransaction.TranCurrency ?? ""),
                  (object) paymentTransaction.TranAmountx1000
                };
                int num = 0;
                foreach (object o in objArray)
                  preparedStatement.BindObject(num++, o);
                preparedStatement.Step();
              }
            }
          }
        }
      }));
    }

    public static void UpdateTransactionDetails(
      PaymentTransactionInfo paymentTransaction,
      string newTranId = null,
      PaymentTransactionInfo.TransactionStatuses? newStatus = null,
      PaymentTransactionInfo.TransactionTypes? newType = null)
    {
      Log.d("PTStore", "Updating transaction: {0} {1} {2} {3} {4} {5}", (object) paymentTransaction.ReceiverJid, (object) paymentTransaction.SenderJid, (object) paymentTransaction.TransactionType, !string.IsNullOrEmpty(newTranId) ? (object) newTranId : (object) "null", newStatus.HasValue ? (object) newStatus.Value.ToString() : (object) "null");
      if (string.IsNullOrEmpty(newTranId) && !newStatus.HasValue && !newType.HasValue)
        Log.l("PTStore", "Must specify at least one of tran id, status or type to update");
      else
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          string str = "UPDATE pay_transactions SET ";
          List<object> source = new List<object>();
          if (!string.IsNullOrEmpty(newTranId))
          {
            str += "tran_id = ?";
            source.Add((object) newTranId);
          }
          if (newStatus.HasValue)
          {
            if (source.Count<object>() > 0)
              str += ", ";
            str += "status = ? ";
            source.Add((object) (int) newStatus.Value);
          }
          if (newType.HasValue)
          {
            if (source.Count<object>() > 0)
              str += ", ";
            str += "type = ? ";
            source.Add((object) (int) newType.Value);
          }
          string sql = str + " WHERE tran_id = ? AND key_id = ? AND receiver = ? AND sender = ?";
          source.Add((object) paymentTransaction.TransactionId);
          source.Add((object) paymentTransaction.FMsgKeyId);
          source.Add((object) paymentTransaction.ReceiverJid);
          source.Add((object) paymentTransaction.SenderJid);
          using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
          {
            int num = 0;
            foreach (object o in source)
              preparedStatement.BindObject(num++, o);
            preparedStatement.Step();
          }
        }));
    }

    public static void RemoveTransaction(string tranId)
    {
      Log.d("PTStore", "Removing Transaction: {0}", (object) tranId);
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        string sql = "DELETE FROM pay_transactions WHERE tran_id = ?";
        using (Sqlite.PreparedStatement preparedStatement = db.PrepareStatement(sql))
        {
          object[] objArray = new object[1]
          {
            (object) tranId
          };
          int num = 0;
          foreach (object o in objArray)
            preparedStatement.BindObject(num++, o);
          preparedStatement.Step();
        }
      }));
    }
  }
}
