// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentTransactionInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class PaymentTransactionInfo
  {
    private object transactionLock = new object();
    private string fMsgKeyRemoteJid;
    private string fMsgKeyId;
    private string senderJid;
    private string receiverJid;
    public byte[] paymentMethodSourceBytes;
    private static PaymentTransactionInfo.TransactionTypes[] CHECKING_P2P_TYPES = new PaymentTransactionInfo.TransactionTypes[3]
    {
      PaymentTransactionInfo.TransactionTypes.p2p,
      PaymentTransactionInfo.TransactionTypes.p2pin,
      PaymentTransactionInfo.TransactionTypes.p2pout
    };

    public PaymentTransactionInfo.TransactionTypes TransactionType { get; private set; }

    public string TransactionId { get; private set; }

    public long TransactionAmountx1000 { get; private set; }

    public string TransactionCurrency { get; private set; }

    public DateTime TransactionDate { get; private set; }

    public PaymentTransactionInfo.TransactionStatuses TransactionStatus { get; private set; }

    public string TransactionErrorCode { get; set; }

    public string CredentialId { get; set; }

    public string BankXtranId { get; set; }

    public byte[] MetaData { get; set; }

    public string FMsgKeyRemoteJid
    {
      get
      {
        this.AssertTypeAppropriate(PaymentTransactionInfo.CHECKING_P2P_TYPES, "get remote jid");
        return this.fMsgKeyRemoteJid;
      }
      private set => this.fMsgKeyRemoteJid = value;
    }

    public bool FMsgFromMe { get; set; }

    public string FMsgKeyId
    {
      get
      {
        this.AssertTypeAppropriate(PaymentTransactionInfo.CHECKING_P2P_TYPES, "get msg id");
        return this.fMsgKeyId;
      }
      private set => this.fMsgKeyId = value;
    }

    public string SenderJid
    {
      get
      {
        this.AssertTypeAppropriate(PaymentTransactionInfo.CHECKING_P2P_TYPES, "get sender jid");
        return this.senderJid;
      }
      private set => this.senderJid = value;
    }

    public string ReceiverJid
    {
      get
      {
        this.AssertTypeAppropriate(PaymentTransactionInfo.CHECKING_P2P_TYPES, "get receiver jid");
        return this.receiverJid;
      }
      private set => this.receiverJid = value;
    }

    public PaymentsMethodSummary[] PaymentMethodDetails
    {
      get
      {
        if (this.paymentMethodSourceBytes == null)
          return (PaymentsMethodSummary[]) null;
        try
        {
          return PaymentsMethodSummary.Deserialize(this.paymentMethodSourceBytes);
        }
        catch (Exception ex)
        {
          Log.LogException(ex, "PaymentsTransaction exception creating PaymentSourceDetails");
        }
        return (PaymentsMethodSummary[]) null;
      }
    }

    public void UpdateStatus(
      PaymentTransactionInfo.TransactionStatuses newStatus)
    {
      lock (this.transactionLock)
      {
        this.TransactionStatus = newStatus;
        Log.SendCrashLog((Exception) new NotImplementedException("PAY: Not tested"), "PAY: Not tested", logOnlyForRelease: true);
      }
    }

    private PaymentTransactionInfo(
      PaymentTransactionInfo.TransactionTypes type,
      string tranId,
      long tranAmtx1000,
      string transCurr,
      DateTime tranDate,
      PaymentTransactionInfo.TransactionStatuses transStatus,
      string fmsgRemoteJid,
      string fmsgId,
      string fromJid,
      string toJid,
      byte[] paymentMethodSummary)
    {
      this.TransactionType = type;
      this.TransactionId = tranId;
      this.TransactionAmountx1000 = tranAmtx1000;
      this.TransactionCurrency = transCurr;
      this.TransactionDate = tranDate;
      this.TransactionStatus = transStatus;
      this.FMsgKeyRemoteJid = fmsgRemoteJid;
      this.FMsgKeyId = fmsgId;
      this.SenderJid = fromJid;
      this.ReceiverJid = toJid;
      this.paymentMethodSourceBytes = paymentMethodSummary;
    }

    public static PaymentTransactionInfo CreateSendingPaymentTransaction(
      long amtx1000,
      string currency,
      byte[] methodSummaries,
      string fmsgKeyJid,
      string fmsgKeyId,
      string toJid)
    {
      return new PaymentTransactionInfo(PaymentTransactionInfo.TransactionTypes.p2pout, (string) null, amtx1000, currency, DateTime.Now, PaymentTransactionInfo.TransactionStatuses.PENDING, fmsgKeyJid, fmsgKeyId, Settings.MyJid, toJid, methodSummaries);
    }

    public static PaymentTransactionInfo CreateReceivingPaymentTransaction(
      long amtx1000,
      string currency,
      byte[] methodSummaries,
      string fmsgKeyJid,
      string fmsgKeyId,
      string fromJid)
    {
      return new PaymentTransactionInfo(PaymentTransactionInfo.TransactionTypes.p2pin, (string) null, amtx1000, currency, DateTime.Now, PaymentTransactionInfo.TransactionStatuses.PENDING, fmsgKeyJid, fmsgKeyId, fromJid, Settings.MyJid, methodSummaries);
    }

    public static PaymentTransactionInfo CreateUninvolvedPaymentTransaction(
      long amtx1000,
      string currency,
      string fmsgKeyJid,
      string fmsgKeyId,
      string fromJid,
      string toJid)
    {
      return new PaymentTransactionInfo(PaymentTransactionInfo.TransactionTypes.p2p, (string) null, amtx1000, currency, DateTime.Now, PaymentTransactionInfo.TransactionStatuses.PENDING, fmsgKeyJid, fmsgKeyId, fromJid, toJid, (byte[]) null);
    }

    public static PaymentTransactionInfo CreateCashInTransaction(
      string contextUuid,
      string tranId,
      long amtx1000,
      string currency,
      byte[] methodSummaries,
      PaymentTransactionInfo.TransactionStatuses status,
      long ts)
    {
      DateTime tranDate = DateTimeUtils.FromUnixTime(ts);
      return new PaymentTransactionInfo(PaymentTransactionInfo.TransactionTypes.cashin, tranId, amtx1000, currency, tranDate, status, (string) null, (string) null, (string) null, (string) null, methodSummaries);
    }

    public static PaymentTransactionInfo CreateCashOutTransaction(
      string contextUuid,
      string tranId,
      long amtx1000,
      string currency,
      byte[] methodSummaries,
      PaymentTransactionInfo.TransactionStatuses status,
      long ts)
    {
      DateTime tranDate = DateTimeUtils.FromUnixTime(ts);
      return new PaymentTransactionInfo(PaymentTransactionInfo.TransactionTypes.cashout, tranId, amtx1000, currency, tranDate, status, (string) null, (string) null, (string) null, (string) null, methodSummaries);
    }

    public static PaymentTransactionInfo.TransactionStatuses GetStatusAsEnum(string status)
    {
      status = !string.IsNullOrEmpty(status) ? status.ToLowerInvariant() : throw new ArgumentNullException("Null status supplied to GetStatusAsEnum");
      foreach (PaymentTransactionInfo.TransactionStatuses statusAsEnum in Enum.GetValues(typeof (PaymentTransactionInfo.TransactionStatuses)))
      {
        if (statusAsEnum.ToString().ToLowerInvariant() == status)
          return statusAsEnum;
      }
      throw new ArgumentOutOfRangeException(string.Format("Supplied status {0} is not valid", (object) status));
    }

    public static PaymentTransactionInfo.TransactionTypes GetTypeAsEnum(string type)
    {
      type = !string.IsNullOrEmpty(type) ? type.ToLowerInvariant() : throw new ArgumentNullException("Null status supplied to GetTypeAsEnum");
      foreach (PaymentTransactionInfo.TransactionTypes typeAsEnum in Enum.GetValues(typeof (PaymentTransactionInfo.TransactionTypes)))
      {
        if (typeAsEnum.ToString().ToLowerInvariant() == type)
          return typeAsEnum;
      }
      throw new ArgumentOutOfRangeException(string.Format("Supplied transaction type {0} is not valid", (object) type));
    }

    public void UpdateStoredDetails(
      string tranid,
      PaymentTransactionInfo.TransactionStatuses status,
      long ts)
    {
      this.TransactionId = tranid;
      this.TransactionStatus = status;
      this.TransactionDate = DateTimeUtils.FromUnixTime(ts);
    }

    private void AssertTypeAppropriate(
      PaymentTransactionInfo.TransactionTypes mustMatchTranType,
      string checkingString)
    {
      if (this.TransactionType != mustMatchTranType)
      {
        Log.l("PaymentsTransaction", "unsupported action checking: {0}, for: {1}, transaction type: {2}", (object) checkingString, (object) mustMatchTranType, (object) this.TransactionType);
        throw new ArgumentException("unsupported action for transaction type");
      }
    }

    private void AssertTypeAppropriate(
      PaymentTransactionInfo.TransactionTypes[] mustNatchPaymentTypes,
      string checkingString)
    {
      if (!((IEnumerable<PaymentTransactionInfo.TransactionTypes>) mustNatchPaymentTypes).Contains<PaymentTransactionInfo.TransactionTypes>(this.TransactionType))
      {
        Log.l("PaymentsTransaction", "unsupported action checking: {0}, transaction type: {2}", (object) checkingString, (object) this.TransactionType);
        throw new ArgumentException("unsupported action for transaction type");
      }
    }

    public enum TransactionStatuses
    {
      Unknown,
      PENDING,
      SUCCESS,
      FAILURE,
      PENDING_RISK,
      PENDING_VERIF,
      PENDING_SETUP,
      REQUESTED,
    }

    public enum TransactionTypes
    {
      unknown,
      p2p,
      cashin,
      cashout,
      p2pin,
      p2pout,
    }
  }
}
