// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsPersistentAction
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using WhatsApp.WaCollections;


namespace WhatsApp
{
  public static class PaymentsPersistentAction
  {
    private const int PaymentsPersistedActionDataFormat = 1;

    public static void ScheduleGetTranRequestAction(string tranId)
    {
      Log.d("Payments", "Get transaction details to be scheduled {0}", (object) tranId);
      PersistentAction pa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool deletedPersistentAction = false;
        foreach (PersistentAction pendingPaymentsRequest in db.GetPendingPaymentsRequests(out deletedPersistentAction, new PaymentsPersistentAction.PaymentPersistedActions?(PaymentsPersistentAction.PaymentPersistedActions.GetTransaction)))
        {
          if ((PaymentsPersistentAction.PaymentsActionData.CreateFromBinaryData(pendingPaymentsRequest.ActionData) is PaymentsPersistentAction.PaymentPersistedGetTransActionData fromBinaryData2 ? fromBinaryData2.TransactionId : (string) null) == tranId)
            return;
        }
        PaymentsPersistentAction.PaymentPersistedGetTransActionData getTransActionData = new PaymentsPersistentAction.PaymentPersistedGetTransActionData(tranId);
        pa = new PersistentAction()
        {
          ActionType = 34,
          Jid = Settings.MyJid,
          ActionData = getTransActionData.CreateActionData(),
          ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
          Attempts = 20
        };
        db.StorePersistentAction(pa);
        db.SubmitChanges();
      }));
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }

    public static void ScheduleGetTransRequestAction(DateTime endDateTime)
    {
      Log.d("Payments", "Get transactions details to be scheduled {0}", (object) endDateTime);
      PersistentAction pa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool deletedPersistentAction = false;
        foreach (PersistentAction pendingPaymentsRequest in db.GetPendingPaymentsRequests(out deletedPersistentAction, new PaymentsPersistentAction.PaymentPersistedActions?(PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeDate)))
        {
          if (PaymentsPersistentAction.PaymentsActionData.CreateFromBinaryData(pendingPaymentsRequest.ActionData) is PaymentsPersistentAction.PaymentPersistedGetTransActionData fromBinaryData2 && fromBinaryData2.EndDateTime.HasValue && fromBinaryData2.EndDateTime.Value == endDateTime)
            return;
        }
        PaymentsPersistentAction.PaymentPersistedGetTransActionData getTransActionData = new PaymentsPersistentAction.PaymentPersistedGetTransActionData(endDateTime);
        pa = new PersistentAction()
        {
          ActionType = 34,
          Jid = Settings.MyJid,
          ActionData = getTransActionData.CreateActionData(),
          ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
          Attempts = 20
        };
        db.StorePersistentAction(pa);
        db.SubmitChanges();
      }));
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }

    public static void ScheduleGetTransRequestAction()
    {
      Log.d("Payments", "Get transactions details to be scheduled from 'now' whenever now is");
      PersistentAction pa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool deletedPersistentAction = false;
        List<PersistentAction> paymentsRequests = db.GetPendingPaymentsRequests(out deletedPersistentAction, new PaymentsPersistentAction.PaymentPersistedActions?(PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeNow));
        if (paymentsRequests != null && paymentsRequests.Count > 0)
          return;
        PaymentsPersistentAction.PaymentPersistedGetTransActionData getTransActionData = new PaymentsPersistentAction.PaymentPersistedGetTransActionData();
        pa = new PersistentAction()
        {
          ActionType = 34,
          Jid = Settings.MyJid,
          ActionData = getTransActionData.CreateActionData(),
          ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
          Attempts = 20
        };
        db.StorePersistentAction(pa);
        db.SubmitChanges();
      }));
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }

    public static void ScheduleUpdateTranidAction(
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
      Log.d("Payments", "Schedule Update transactions details {0} {1}", (object) msgId, (object) tranId);
      PersistentAction pa = (PersistentAction) null;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        bool deletedPersistentAction = false;
        foreach (PersistentAction pendingPaymentsRequest in db.GetPendingPaymentsRequests(out deletedPersistentAction, new PaymentsPersistentAction.PaymentPersistedActions?(PaymentsPersistentAction.PaymentPersistedActions.SetTransactionTranid)))
        {
          if (PaymentsPersistentAction.PaymentsActionData.CreateFromBinaryData(pendingPaymentsRequest.ActionData) is PaymentsPersistentAction.PaymentPersistedSetTranidActionData fromBinaryData2 && fromBinaryData2.TransactionId != null && fromBinaryData2.TransactionId == tranId)
            return;
        }
        PaymentsPersistentAction.PaymentPersistedSetTranidActionData tranidActionData = new PaymentsPersistentAction.PaymentPersistedSetTranidActionData(tranId, tranTs, tranStatus, groupJid, receiverJid, senderJid, msgId, tranAmountx1000, tranCurrency);
        pa = new PersistentAction()
        {
          ActionType = 34,
          Jid = Settings.MyJid,
          ActionData = tranidActionData.CreateActionData(),
          ExpirationTime = new DateTime?(FunRunner.CurrentServerTimeUtc.AddDays(1.0)),
          Attempts = 20
        };
        db.StorePersistentAction(pa);
        db.SubmitChanges();
      }));
      AppState.Worker.Enqueue((Action) (() => AppState.AttemptPersistentAction(pa)));
    }

    public static IObservable<Unit> SendPaymentsRequest(FunXMPP.Connection conn, byte[] actionData)
    {
      PaymentsPersistentAction.PaymentsActionData paymentsActionData = (PaymentsPersistentAction.PaymentsActionData) null;
      if (actionData != null && actionData.Length > 2)
        paymentsActionData = PaymentsPersistentAction.PaymentsActionData.CreateFromBinaryData(actionData);
      if (paymentsActionData == null)
      {
        Log.l("Payments", "Sending payment request ignored - {0}", (object) (actionData == null ? -1 : actionData.Length));
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
          return (Action) (() => { });
        }));
      }
      Log.l("Payments", "Sending payment request {0}", (object) paymentsActionData.ActionType);
      return paymentsActionData.Perform(conn);
    }

    public static List<PersistentAction> GetPendingPaymentsRequests(
      this MessagesContext db,
      out bool deletedPersistentAction,
      PaymentsPersistentAction.PaymentPersistedActions? matchingActionType = null)
    {
      List<PersistentAction> paymentsRequests = new List<PersistentAction>();
      deletedPersistentAction = false;
      foreach (PersistentAction persistentAction in db.GetPersistentActions(PersistentAction.Types.SendPaymentsRequest))
      {
        BinaryData binaryData = new BinaryData(persistentAction.ActionData);
        int num = (int) binaryData.ReadByte(0);
        int offset = 1;
        if (num != 1)
        {
          db.DeletePersistentActionOnSubmit(persistentAction);
          deletedPersistentAction = true;
        }
        else if (matchingActionType.HasValue)
        {
          PaymentsPersistentAction.PaymentPersistedActions persistedActions = (PaymentsPersistentAction.PaymentPersistedActions) binaryData.ReadInt32(offset);
          if (matchingActionType.Value == persistedActions)
            paymentsRequests.Add(persistentAction);
        }
        else
          paymentsRequests.Add(persistentAction);
      }
      return paymentsRequests;
    }

    public enum PaymentPersistedActions
    {
      Unknown,
      GetTransaction,
      GetTransactionBeforeDate,
      GetTransactionBeforeNow,
      SetTransactionTranid,
    }

    public abstract class PaymentsActionData
    {
      public PaymentsPersistentAction.PaymentPersistedActions ActionType { get; private set; }

      public PaymentsActionData(
        PaymentsPersistentAction.PaymentPersistedActions actionType)
      {
        this.ActionType = actionType;
      }

      public static PaymentsPersistentAction.PaymentsActionData CreateFromBinaryData(
        byte[] actionData)
      {
        BinaryData binaryData = actionData != null && actionData.Length >= 2 ? new BinaryData(actionData) : throw new ArgumentOutOfRangeException(string.Format("Invalid length action data to create PaymentsActionData {0}", (object) (actionData == null ? -1 : actionData.Length)));
        byte num = binaryData.ReadByte(0);
        if (num != (byte) 1)
          throw new ArgumentOutOfRangeException(string.Format("Invalid action format for PaymentsActionData {0}", (object) num));
        int offset1 = 1;
        PaymentsPersistentAction.PaymentPersistedActions actionType = (PaymentsPersistentAction.PaymentPersistedActions) binaryData.ReadInt32(offset1);
        int offset2 = offset1 + 12;
        if (actionType == PaymentsPersistentAction.PaymentPersistedActions.GetTransaction || actionType == PaymentsPersistentAction.PaymentPersistedActions.GetTransaction)
          return (PaymentsPersistentAction.PaymentsActionData) PaymentsPersistentAction.PaymentPersistedGetTransActionData.CreateFromBinaryData(binaryData, offset2, actionType);
        return actionType == PaymentsPersistentAction.PaymentPersistedActions.SetTransactionTranid ? (PaymentsPersistentAction.PaymentsActionData) PaymentsPersistentAction.PaymentPersistedSetTranidActionData.CreateFromBinaryData(binaryData, offset2, actionType) : (PaymentsPersistentAction.PaymentsActionData) null;
      }

      public byte[] CreateActionData()
      {
        BinaryData binaryData = new BinaryData();
        binaryData.AppendByte((byte) 1);
        binaryData.AppendInt32((int) this.ActionType);
        binaryData.AppendLong64(0L);
        this.AddTypeSpecificBytes(ref binaryData);
        return binaryData.Get();
      }

      protected abstract void AddTypeSpecificBytes(ref BinaryData binaryData);

      public abstract IObservable<Unit> Perform(FunXMPP.Connection conn);
    }

    public class PaymentPersistedGetTransActionData : PaymentsPersistentAction.PaymentsActionData
    {
      public string TransactionId;
      public DateTime? EndDateTime;

      public PaymentPersistedGetTransActionData(string tranId)
        : base(PaymentsPersistentAction.PaymentPersistedActions.GetTransaction)
      {
        this.TransactionId = tranId;
      }

      public PaymentPersistedGetTransActionData(DateTime endDateTime)
        : base(PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeDate)
      {
        this.EndDateTime = new DateTime?(endDateTime);
      }

      public PaymentPersistedGetTransActionData()
        : base(PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeNow)
      {
        this.EndDateTime = new DateTime?();
      }

      public static PaymentsPersistentAction.PaymentPersistedGetTransActionData CreateFromBinaryData(
        BinaryData binaryData,
        int offset,
        PaymentsPersistentAction.PaymentPersistedActions actionType)
      {
        switch (actionType)
        {
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransaction:
            return new PaymentsPersistentAction.PaymentPersistedGetTransActionData(binaryData.ReadStrWithLengthPrefix(offset, out offset));
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeDate:
            return new PaymentsPersistentAction.PaymentPersistedGetTransActionData(new DateTime(binaryData.ReadLong64(offset)));
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeNow:
            return new PaymentsPersistentAction.PaymentPersistedGetTransActionData();
          default:
            throw new ArgumentOutOfRangeException(string.Format("Invalid action type for PaymentPersistedGetTranActionData {0}", (object) actionType));
        }
      }

      protected override void AddTypeSpecificBytes(ref BinaryData bd)
      {
        switch (this.ActionType)
        {
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransaction:
            bd.AppendStrWithLengthPrefix(this.TransactionId);
            break;
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeDate:
            bd.AppendLong64(this.EndDateTime.Value.Ticks);
            break;
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeNow:
            break;
          default:
            throw new ArgumentException(string.Format("Unknown PaymentPersistedGetTranActionData Action type {0}", (object) this.ActionType));
        }
      }

      public override IObservable<Unit> Perform(FunXMPP.Connection conn)
      {
        switch (this.ActionType)
        {
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransaction:
            return this.PerformGetTransaction(conn);
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeDate:
            return this.PerformGetTransactions(conn);
          case PaymentsPersistentAction.PaymentPersistedActions.GetTransactionBeforeNow:
            return this.PerformGetTransactions(conn);
          default:
            throw new ArgumentException(string.Format("Unknown PaymentPersistedGetTranActionData Action type {0}", (object) this.ActionType));
        }
      }

      private IObservable<Unit> PerformGetTransactions(FunXMPP.Connection conn)
      {
        DateTime endDateTime = this.EndDateTime.HasValue ? this.EndDateTime.Value : DateTime.UtcNow;
        Log.d("Payments", "Get transactions details to be run {0}", (object) endDateTime);
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          bool cancelled = false;
          conn.SendPaymentGetTransactionsRequest(endDateTime, (Action<List<FunXMPP.Connection.PaymentsTransactionResponse>>) (transactionDetails =>
          {
            if (!cancelled)
            {
              if (transactionDetails != null)
              {
                Log.d("Payments", "Get transactions details found {0}", (object) endDateTime);
                Log.SendCrashLog((Exception) new NotImplementedException("PAY: Not tested"), "PAY: Not tested", logOnlyForRelease: true);
              }
              else
                Log.d("Payments", "Get transactions details not found {0}", (object) endDateTime);
            }
            if (cancelled)
              return;
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("SendPaymentsRequest", "Error sending get transactions {0}", (object) err);
            if (cancelled)
              return;
            if (err == 400)
              observer.OnNext(new Unit());
            observer.OnCompleted();
          }));
          return (Action) (() => cancelled = true);
        }));
      }

      private IObservable<Unit> PerformGetTransaction(FunXMPP.Connection conn)
      {
        string tranId = this.TransactionId;
        Log.d("Payments", "Get transaction details to be run {0}", (object) tranId);
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          bool cancelled = false;
          conn.SendPaymentGetTransactionRequest(tranId, (Action<FunXMPP.Connection.PaymentsTransactionResponse>) (transactionDetails =>
          {
            if (!cancelled)
              Log.SendCrashLog((Exception) new NotImplementedException("PAY: Not tested"), "PAY: Not tested", logOnlyForRelease: true);
            if (cancelled)
              return;
            observer.OnNext(new Unit());
            observer.OnCompleted();
          }), (Action<int>) (err =>
          {
            Log.l("SendPaymentsRequest", "Error sending get transaction {0}", (object) err);
            if (cancelled)
              return;
            if (err == 400)
              observer.OnNext(new Unit());
            observer.OnCompleted();
          }));
          return (Action) (() => cancelled = true);
        }));
      }
    }

    public class PaymentPersistedSetTranidActionData : PaymentsPersistentAction.PaymentsActionData
    {
      public string TransactionId;
      public long TransactionTS;
      public string TransactionStatusString;
      public string GroupJid;
      public string ReceiverJid;
      public string SenderJid;
      public string MsgId;
      public long TranAmountx1000;
      public string TranCurrency;

      public PaymentPersistedSetTranidActionData(
        string tranId,
        long tranTs,
        string tranStatusString,
        string groupJid,
        string receiverJid,
        string senderJid,
        string msgId,
        long tranAmountx1000,
        string tranCurrency)
        : base(PaymentsPersistentAction.PaymentPersistedActions.SetTransactionTranid)
      {
        this.TransactionId = tranId;
        this.TransactionTS = tranTs;
        this.TransactionStatusString = tranStatusString;
        this.GroupJid = groupJid;
        this.ReceiverJid = receiverJid;
        this.SenderJid = senderJid;
        this.MsgId = msgId;
        this.TranAmountx1000 = tranAmountx1000;
        this.TranCurrency = tranCurrency;
      }

      public static PaymentsPersistentAction.PaymentPersistedSetTranidActionData CreateFromBinaryData(
        BinaryData binaryData,
        int offset,
        PaymentsPersistentAction.PaymentPersistedActions actionType)
      {
        string tranId = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        long num1 = binaryData.ReadLong64(offset);
        offset += 8;
        string str1 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        string str2 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        string str3 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        string str4 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        string str5 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        long num2 = binaryData.ReadLong64(offset);
        offset += 8;
        string str6 = binaryData.ReadStrWithLengthPrefix(offset, out offset);
        long tranTs = num1;
        string tranStatusString = str1;
        string groupJid = str2;
        string receiverJid = str3;
        string senderJid = str4;
        string msgId = str5;
        long tranAmountx1000 = num2;
        string tranCurrency = str6;
        return new PaymentsPersistentAction.PaymentPersistedSetTranidActionData(tranId, tranTs, tranStatusString, groupJid, receiverJid, senderJid, msgId, tranAmountx1000, tranCurrency);
      }

      protected override void AddTypeSpecificBytes(ref BinaryData bd)
      {
        bd.AppendStrWithLengthPrefix(this.TransactionId);
        bd.AppendLong64(this.TransactionTS);
        bd.AppendStrWithLengthPrefix(this.TransactionStatusString);
        bd.AppendStrWithLengthPrefix(this.GroupJid);
        bd.AppendStrWithLengthPrefix(this.ReceiverJid);
        bd.AppendStrWithLengthPrefix(this.SenderJid);
        bd.AppendStrWithLengthPrefix(this.MsgId);
        bd.AppendLong64(this.TranAmountx1000);
        bd.AppendStrWithLengthPrefix(this.TranCurrency);
      }

      public override IObservable<Unit> Perform(FunXMPP.Connection conn)
      {
        Log.d("Payments", "Update transaction id {0}", (object) this.TransactionId);
        return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
        {
          try
          {
            if (PaymentsHelper.UpdateTransactionTranId(this.TransactionId, this.TransactionTS, this.TransactionStatusString, this.GroupJid, this.ReceiverJid, this.SenderJid, this.MsgId, this.TranAmountx1000, this.TranCurrency))
              observer.OnNext(new Unit());
            observer.OnCompleted();
          }
          catch (Exception ex)
          {
            Log.LogException(ex, "Exception updating transaction");
            observer.OnCompleted();
          }
          return (Action) (() => { });
        }));
      }
    }
  }
}
