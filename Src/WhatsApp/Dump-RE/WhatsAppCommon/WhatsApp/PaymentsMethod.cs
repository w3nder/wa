// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsMethod
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace WhatsApp
{
  public class PaymentsMethod
  {
    private int paymentSubType;
    private string currencyIso4217;
    private object balanceLock = new object();
    private long balancex1000;
    private DateTime? balanceTimestamp;
    private string firstName;
    private string secondName;
    private static string[] VALID_CARD_SUBTYPES = new string[3]
    {
      "",
      "visa",
      "mastercard"
    };
    private static PaymentsMethod.PaymentTypes[] CHECKING_CARD_TYPES = new PaymentsMethod.PaymentTypes[2]
    {
      PaymentsMethod.PaymentTypes.CreditCard,
      PaymentsMethod.PaymentTypes.DebitCard
    };

    public string CredentialId { get; private set; }

    public string ReadableName { get; private set; }

    public PaymentsMethod.PaymentTypes PaymentType { get; private set; }

    public int PaymentSubType
    {
      get
      {
        return ((IEnumerable<PaymentsMethod.PaymentTypes>) PaymentsMethod.CHECKING_CARD_TYPES).Contains<PaymentsMethod.PaymentTypes>(this.PaymentType) ? this.paymentSubType : 0;
      }
      private set
      {
        if (!((IEnumerable<PaymentsMethod.PaymentTypes>) PaymentsMethod.CHECKING_CARD_TYPES).Contains<PaymentsMethod.PaymentTypes>(this.PaymentType))
          return;
        this.paymentSubType = value;
      }
    }

    private PaymentsMethod.PaymentModes paymentMode { get; set; }

    public bool isDefaultPayment() => this.paymentMode == PaymentsMethod.PaymentModes.Primary;

    public bool setDefaultPayment()
    {
      if (this.isDefaultPayment())
        return false;
      this.paymentMode = PaymentsMethod.PaymentModes.Primary;
      return true;
    }

    public bool resetDefaultPayment()
    {
      if (!this.isDefaultPayment())
        return false;
      this.paymentMode = PaymentsMethod.PaymentModes.None;
      return true;
    }

    private PaymentsMethod.PaymentModes payoutMode { get; set; }

    public bool isDefaultPayout() => this.payoutMode == PaymentsMethod.PaymentModes.Primary;

    public bool setDefaultPayout()
    {
      if (this.isDefaultPayout())
        return false;
      this.payoutMode = PaymentsMethod.PaymentModes.Primary;
      return true;
    }

    public bool resetDefaultPayout()
    {
      if (!this.isDefaultPayout())
        return false;
      this.payoutMode = PaymentsMethod.PaymentModes.None;
      return true;
    }

    public string CountryCodeIso3166 { get; set; }

    public string CurrencyIso4217
    {
      get
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "get Currency");
        return this.currencyIso4217;
      }
      set
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "set Currency");
        this.currencyIso4217 = value;
      }
    }

    public double Balance
    {
      get
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "get Balance");
        lock (this.balanceLock)
          return (double) this.balancex1000 / 1000.0;
      }
    }

    public DateTime BalanceDate
    {
      get
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "get BalanceDate");
        lock (this.balanceLock)
          return this.balanceTimestamp.HasValue ? this.balanceTimestamp.Value : throw new ArgumentException("Can't get Balance is not set - check HasBlanace");
      }
    }

    public void UpdateBalance(long newBalancex1000, long balanceTs, bool storeUpdate = false)
    {
      this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "set Balance");
      lock (this.balanceLock)
      {
        this.balanceTimestamp = new DateTime?(DateTime.FromBinary(balanceTs * 10000000L));
        this.balancex1000 = newBalancex1000;
        if (!storeUpdate)
          return;
        SqlitePayments.UpdateBalanceForPaymentsMethod(this);
      }
    }

    public bool HasBalance()
    {
      if (this.PaymentType != PaymentsMethod.PaymentTypes.Wallet)
        return false;
      lock (this.balanceLock)
        return this.balanceTimestamp.HasValue;
    }

    public string FirstName
    {
      get
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "get FirstNmae");
        return this.firstName;
      }
      private set => this.firstName = value;
    }

    public string SecondName
    {
      get
      {
        this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "get FirstNmae");
        return this.secondName;
      }
      private set => this.secondName = value;
    }

    public void setName(string firstName, string secondName)
    {
      this.AssertTypeAppropriate(PaymentsMethod.PaymentTypes.Wallet, "setNmae");
      this.FirstName = firstName;
      this.SecondName = secondName;
    }

    public PaymentsMethod(
      string credential,
      string readableName,
      PaymentsMethod.PaymentTypes payType,
      int paySubType,
      bool isPrimaryPayment,
      bool isPrimaryPayout,
      string countryCode)
    {
      this.CredentialId = credential;
      this.ReadableName = readableName;
      this.PaymentType = payType;
      this.PaymentSubType = paySubType;
      this.paymentMode = isPrimaryPayment ? PaymentsMethod.PaymentModes.Primary : PaymentsMethod.PaymentModes.None;
      this.payoutMode = isPrimaryPayout ? PaymentsMethod.PaymentModes.Primary : PaymentsMethod.PaymentModes.None;
      this.CountryCodeIso3166 = countryCode;
    }

    public static string GetPaymentCardSubTypeAsString(int subType)
    {
      return subType >= 0 && subType < ((IEnumerable<string>) PaymentsMethod.VALID_CARD_SUBTYPES).Count<string>() ? PaymentsMethod.VALID_CARD_SUBTYPES[subType] : throw new ArgumentOutOfRangeException(string.Format("Supplied integer card subType {0} is not valid", (object) subType));
    }

    public static int GetPaymentCardSubTypeAsInt(string subType)
    {
      if (string.IsNullOrEmpty(subType))
        return 0;
      subType = subType.ToLowerInvariant();
      for (int cardSubTypeAsInt = 0; cardSubTypeAsInt < ((IEnumerable<string>) PaymentsMethod.VALID_CARD_SUBTYPES).Count<string>(); ++cardSubTypeAsInt)
      {
        if (PaymentsMethod.VALID_CARD_SUBTYPES[cardSubTypeAsInt] == subType)
          return cardSubTypeAsInt;
      }
      throw new ArgumentOutOfRangeException(string.Format("Supplied string card subType {0} is not valid", (object) subType));
    }

    private void AssertTypeAppropriate(
      PaymentsMethod.PaymentTypes mustMatchPaymentType,
      string checkingString)
    {
      if (this.PaymentType != mustMatchPaymentType)
      {
        Log.l(nameof (PaymentsMethod), "unsupported action checking: {0}, for: {1}, payment type: {2}", (object) checkingString, (object) mustMatchPaymentType, (object) this.PaymentType);
        throw new ArgumentException("unsupported action for payment type");
      }
    }

    private void AssertTypeAppropriate(
      PaymentsMethod.PaymentTypes[] mustMatchPaymentTypes,
      string checkingString)
    {
      if (!((IEnumerable<PaymentsMethod.PaymentTypes>) mustMatchPaymentTypes).Contains<PaymentsMethod.PaymentTypes>(this.PaymentType))
      {
        Log.l(nameof (PaymentsMethod), "unsupported action checking: {0}, payment type: {2}", (object) checkingString, (object) this.PaymentType);
        throw new ArgumentException("unsupported action for payment type");
      }
    }

    public enum PaymentTypes
    {
      Unknown,
      Wallet,
      CreditCard,
      DebitCard,
      BankAccount,
    }

    private enum PaymentModes
    {
      None,
      Primary,
    }
  }
}
