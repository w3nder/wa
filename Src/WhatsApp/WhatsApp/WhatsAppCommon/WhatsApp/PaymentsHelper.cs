// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsHelper
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace WhatsApp
{
  public static class PaymentsHelper
  {
    private static object initializationLock = new object();
    private static bool initialized = false;

    private static bool IsPaymentsAvailable()
    {
      if (!PaymentsSettings.IsPaymentsEnabled())
        return false;
      if (PaymentsHelper.initialized)
        return true;
      long ticks = DateTime.Now.Ticks;
      lock (PaymentsHelper.initializationLock)
      {
        long num = DateTime.Now.Ticks - ticks;
        if (num > 2000000L)
          Log.l(nameof (PaymentsHelper), "Took {0} msec to get access to Payments Initialization", (object) num);
        if (PaymentsHelper.initialized)
          return true;
        Log.l(nameof (PaymentsHelper), "Payments Initialization");
        bool shouldRequestPaymentsTransactionsSync = false;
        MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
        {
          bool deletedPersistentAction = false;
          foreach (PersistentAction pendingPaymentsRequest in db.GetPendingPaymentsRequests(out deletedPersistentAction))
            ;
          if (!deletedPersistentAction)
            return;
          shouldRequestPaymentsTransactionsSync = true;
          db.SubmitChanges();
        }));
        if (shouldRequestPaymentsTransactionsSync)
          PaymentsPersistentAction.ScheduleGetTransRequestAction();
      }
      return true;
    }

    public static void OnCashIn(
      string tranId,
      long tranTs,
      string tranStatus,
      string credentialId,
      long? balancex1000,
      string currency,
      bool defPayment,
      bool defPayout)
    {
      PaymentTransactionInfo paymentTransactionInfo = PaymentsSettings.RetrieveCashIn(tranId);
      if (paymentTransactionInfo == null)
      {
        Log.l("payments", "Received unexpected cash in notification");
      }
      else
      {
        List<PaymentsMethod> paymentsMethods = SqlitePayments.GetPaymentsMethods(credentialId);
        if (paymentsMethods != null && paymentsMethods.Count<PaymentsMethod>() == 1 && balancex1000.HasValue)
          paymentsMethods[0].UpdateBalance(balancex1000.Value, tranTs);
        if (paymentTransactionInfo == null)
          paymentTransactionInfo = PaymentTransactionStore.GetTransaction(tranId);
        if (paymentTransactionInfo != null)
          paymentTransactionInfo.UpdateStatus(PaymentTransactionInfo.GetStatusAsEnum(tranStatus));
        else
          PaymentsPersistentAction.ScheduleGetTranRequestAction(tranId);
      }
    }

    public static void OnCashOut(
      string tranId,
      long tranTs,
      string tranStatus,
      string credentialId,
      long? balancex1000,
      string currency,
      bool defPayment,
      bool defPayout)
    {
      PaymentTransactionInfo paymentTransactionInfo = PaymentsSettings.RetrieveCashOut(tranId);
      if (paymentTransactionInfo == null)
      {
        Log.l("payments", "Received unexpected cash out notification");
      }
      else
      {
        List<PaymentsMethod> paymentsMethods = SqlitePayments.GetPaymentsMethods(credentialId);
        if (paymentsMethods != null && paymentsMethods.Count<PaymentsMethod>() == 1 && balancex1000.HasValue)
          paymentsMethods[0].UpdateBalance(balancex1000.Value, tranTs);
        if (paymentTransactionInfo == null)
          paymentTransactionInfo = PaymentTransactionStore.GetTransaction(tranId);
        if (paymentTransactionInfo != null)
          paymentTransactionInfo.UpdateStatus(PaymentTransactionInfo.GetStatusAsEnum(tranStatus));
        else
          PaymentsPersistentAction.ScheduleGetTranRequestAction(tranId);
      }
    }

    public static void OnTransactionUpdate(
      FunXMPP.Connection.PaymentsTransactionUpdate tranUpdate)
    {
      if (!string.IsNullOrEmpty(tranUpdate.TranType))
      {
        if (tranUpdate.TranType == "cashin")
          PaymentsHelper.OnCashIn(tranUpdate.TranId, tranUpdate.TranTs, tranUpdate.TranStatus, tranUpdate.CredentialId, tranUpdate.Balancex1000, tranUpdate.TranCurrency, tranUpdate.DefPayment, tranUpdate.DefPayout);
        else
          PaymentsHelper.OnCashOut(tranUpdate.TranId, tranUpdate.TranTs, tranUpdate.TranStatus, tranUpdate.CredentialId, tranUpdate.Balancex1000, tranUpdate.TranCurrency, tranUpdate.DefPayment, tranUpdate.DefPayout);
      }
      else
        PaymentsHelper.OnTransactionUpdate(tranUpdate.TranId, tranUpdate.TranTs, tranUpdate.TranStatus, tranUpdate.GroupJid, tranUpdate.ReceiverJid, tranUpdate.SenderJid, tranUpdate.MessageId, tranUpdate.TranAmountx1000, tranUpdate.TranCurrency);
    }

    public static void OnTransactionUpdate(
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
      Log.d("Payments", "received update for {0} status {1}, from {2} to {3}", (object) tranId, (object) tranStatus, (object) senderJid, (object) receiverJid);
      if (PaymentsHelper.UpdateTransactionTranId(tranId, tranTs, tranStatus, groupJid, receiverJid, senderJid, msgId, tranAmountx1000, tranCurrency))
        return;
      if (tranId != null)
      {
        PaymentsPersistentAction.ScheduleGetTranRequestAction(tranId);
        PaymentsPersistentAction.ScheduleUpdateTranidAction(tranId, tranTs, tranStatus, groupJid, receiverJid, senderJid, msgId, tranAmountx1000, tranCurrency);
      }
      else
        PaymentsPersistentAction.ScheduleGetTransRequestAction();
    }

    public static bool UpdateTransactionTranId(
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
      Log.SendCrashLog((Exception) new NotImplementedException("PAY: Not tested"), "PAY: Not tested", logOnlyForRelease: true);
      return false;
    }

    public static bool IsValidLuhn(string cardNumber)
    {
      if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length < 2)
        return false;
      int num1 = 0;
      bool flag = false;
      for (int startIndex = cardNumber.Length - 1; startIndex >= 0; --startIndex)
      {
        int result;
        if (!int.TryParse(cardNumber.Substring(startIndex, 1), out result))
          return false;
        if (flag)
        {
          int num2 = 2 * result;
          num1 += num2 > 9 ? num2 - 9 : num2;
        }
        else
          num1 += result;
        flag = !flag;
      }
      return num1 % 10 == 0;
    }

    public static string FormatAmountForFB(long amountx1000)
    {
      return ((double) amountx1000 / 1000.0).ToString("F2", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static long? ConvertFBStringToLong(string number)
    {
      return string.IsNullOrEmpty(number) ? new long?() : new long?((long) (double.Parse(number, (IFormatProvider) CultureInfo.InvariantCulture) * 1000.0));
    }

    public static string SelectRemoteJid(string groupJid, string receiverJid, string senderJid)
    {
      if (groupJid != null)
        return groupJid;
      switch (PaymentsHelper.SelectP2ptype(groupJid, receiverJid, senderJid))
      {
        case PaymentTransactionInfo.TransactionTypes.p2pin:
          return senderJid;
        case PaymentTransactionInfo.TransactionTypes.p2pout:
          return receiverJid;
        default:
          return (string) null;
      }
    }

    public static PaymentTransactionInfo.TransactionTypes SelectP2ptype(
      string groupJid,
      string receiverJid,
      string senderJid)
    {
      string myJid = Settings.MyJid;
      if (myJid == receiverJid)
        return PaymentTransactionInfo.TransactionTypes.p2pin;
      if (myJid == senderJid)
        return PaymentTransactionInfo.TransactionTypes.p2pout;
      if (groupJid == null)
        Log.l("Payments", "Unexpectedly, one to one payment is not to or from us");
      return PaymentTransactionInfo.TransactionTypes.p2p;
    }

    public static void ProcessIncomingPayment(
      MessageProperties.PaymentsProperties properties,
      string chatJid,
      string fromJid,
      string msgId)
    {
      Log.l("Payments", "ProcessIncomingPayment {0} {1} {2} {3}", (object) fromJid, (object) properties.Receiver, (object) chatJid, (object) msgId);
      List<PaymentsMethod> paymentsMethods = SqlitePayments.GetPaymentsMethods(properties.CredentialId);
      if (paymentsMethods != null && paymentsMethods.Count == 1)
        new PaymentsMethodSummary[1][0] = new PaymentsMethodSummary(paymentsMethods[0]);
      long? nullable = PaymentsHelper.ConvertFBStringToLong(properties.Amount);
      if (!nullable.HasValue)
        nullable = new long?(0L);
      Log.d("Payments", "amount received {0} {1}", (object) properties.Amount, (object) nullable);
      PaymentTransactionStore.StoreTransaction(properties.Receiver == Settings.MyJid || properties.Receiver == null ? PaymentTransactionInfo.CreateReceivingPaymentTransaction(nullable.Value, properties.Currency, (byte[]) null, chatJid, msgId, fromJid) : PaymentTransactionInfo.CreateUninvolvedPaymentTransaction(nullable.Value, properties.Currency, chatJid, msgId, fromJid, properties.Receiver));
    }

    public static void ProcessIncomingDecryptedPayment(
      MessageProperties.PaymentsProperties properties,
      string chatJid,
      string msgId,
      string fromJid)
    {
      Log.l("Payments", "ProcessIncomingDecryptedPayment{0} {1} {2} {3}", (object) fromJid, (object) properties.Receiver, (object) chatJid, (object) msgId);
      Log.SendCrashLog((Exception) new NotImplementedException("PAY: Not tested"), "PAY: Not tested", logOnlyForRelease: true);
    }
  }
}
