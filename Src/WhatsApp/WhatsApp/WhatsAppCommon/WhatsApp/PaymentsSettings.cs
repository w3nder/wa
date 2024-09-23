// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll


namespace WhatsApp
{
  public class PaymentsSettings
  {
    private static object createLock = new object();
    private static PaymentsSettings _settingsInstance;
    public PaymentTransactionInfo ActiveCashIn;
    public PaymentTransactionInfo ActiveCashOut;
    public PaymentsCountryDetails CountryDetails;
    public PaymentsCurrency CurrencyDetails;

    public static bool IsPaymentsEnabled() => false;

    public static PaymentsSettings GetInstance()
    {
      if (PaymentsSettings._settingsInstance == null)
      {
        lock (PaymentsSettings.createLock)
        {
          if (PaymentsSettings._settingsInstance == null)
          {
            PaymentsSettings._settingsInstance = new PaymentsSettings();
            PaymentsSettings._settingsInstance.CountryDetails = PaymentsCountryDetails.getPaymentCountryDetails();
            PaymentsSettings._settingsInstance.CurrencyDetails = PaymentsCurrency.Create(PaymentsSettings._settingsInstance.CountryDetails?.DefaultCurrency);
          }
        }
      }
      return PaymentsSettings._settingsInstance;
    }

    public static bool IsOkToSendCashIn()
    {
      return PaymentsSettings._settingsInstance.ActiveCashIn == null;
    }

    public static bool PersistCashIn(PaymentTransactionInfo cashIn)
    {
      int num = PaymentsSettings._settingsInstance.ActiveCashIn == null ? 1 : 0;
      if (num == 0)
        return num != 0;
      PaymentsSettings._settingsInstance.ActiveCashIn = cashIn;
      PaymentTransactionStore.StoreTransaction(cashIn);
      return num != 0;
    }

    public static PaymentTransactionInfo RetrieveCashIn(string tranId)
    {
      PaymentTransactionInfo paymentTransactionInfo = (PaymentTransactionInfo) null;
      if (PaymentsSettings._settingsInstance.ActiveCashIn == null)
        Log.l("payments", "Received unexpected cash in notification");
      else if (PaymentsSettings._settingsInstance.ActiveCashIn.TransactionId != tranId)
      {
        Log.l("payments", "Received mismatching cash in notification");
      }
      else
      {
        paymentTransactionInfo = PaymentsSettings._settingsInstance.ActiveCashIn;
        PaymentsSettings._settingsInstance.ActiveCashIn = (PaymentTransactionInfo) null;
      }
      return paymentTransactionInfo;
    }

    public static bool IsOkToSendCashOut()
    {
      return PaymentsSettings._settingsInstance.ActiveCashOut == null;
    }

    public static bool PersistCashOut(PaymentTransactionInfo cashOut)
    {
      int num = PaymentsSettings._settingsInstance.ActiveCashOut == null ? 1 : 0;
      if (num == 0)
        return num != 0;
      PaymentsSettings._settingsInstance.ActiveCashOut = cashOut;
      PaymentTransactionStore.StoreTransaction(cashOut);
      return num != 0;
    }

    public static PaymentTransactionInfo RetrieveCashOut(string tranId)
    {
      PaymentTransactionInfo paymentTransactionInfo = (PaymentTransactionInfo) null;
      if (PaymentsSettings._settingsInstance.ActiveCashOut == null)
        Log.l("payments", "Received unexpected cash out notification");
      else if (PaymentsSettings._settingsInstance.ActiveCashOut.TransactionId != tranId)
      {
        Log.l("payments", "Received mismatching cash out notification");
      }
      else
      {
        paymentTransactionInfo = PaymentsSettings._settingsInstance.ActiveCashOut;
        PaymentsSettings._settingsInstance.ActiveCashOut = (PaymentTransactionInfo) null;
      }
      return paymentTransactionInfo;
    }
  }
}
