// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsCountryDetails
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Globalization;

#nullable disable
namespace WhatsApp
{
  public class PaymentsCountryDetails
  {
    private static string COUNTRY_CODE_IN = "IN";
    public string Iso3166CountryCode;
    public string PhonePrefix;
    public PaymentsMethod.PaymentTypes PrimaryPayment;
    public PaymentsMethod.PaymentTypes PrimaryPayout;
    public bool WalletCreationOnFirstPayment;
    public PaymentsCountryDetails.WalletCreationOptions[] WalletCreationInputs;
    public PaymentsMethod.PaymentTypes[] ValidSecondaryPayment;
    public PaymentsMethod.PaymentTypes[] ValidSecondaryPayout;
    public double MinTransferAmount;
    public double MaxTransferAmount;
    public string DefaultCurrency;
    private static PaymentsCountryDetails _instance = (PaymentsCountryDetails) null;

    public static string GetCountryCode()
    {
      string countryCode = PaymentsWaAdminSettings.GetInstance().CountryCode;
      if (countryCode != null)
        return countryCode;
      return JidHelper.GetPhoneNumber(Settings.MyJid, true).StartsWith("+" + CountryInfo.PhoneCountryCodeForIndia) ? PaymentsCountryDetails.COUNTRY_CODE_IN : RegionInfo.CurrentRegion.TwoLetterISORegionName;
    }

    public static PaymentsCountryDetails getPaymentCountryDetails(string countryCode = null)
    {
      if (countryCode == null)
      {
        countryCode = PaymentsCountryDetails.GetCountryCode();
        if (countryCode == null)
          return (PaymentsCountryDetails) null;
      }
      if (PaymentsCountryDetails._instance != null && PaymentsCountryDetails._instance.Iso3166CountryCode == countryCode)
        return PaymentsCountryDetails._instance;
      if (!(countryCode == PaymentsCountryDetails.COUNTRY_CODE_IN))
        return (PaymentsCountryDetails) null;
      PaymentsCountryDetails._instance = new PaymentsCountryDetails()
      {
        Iso3166CountryCode = PaymentsCountryDetails.COUNTRY_CODE_IN,
        PhonePrefix = CountryInfo.PhoneCountryCodeForIndia,
        PrimaryPayment = PaymentsMethod.PaymentTypes.Wallet,
        PrimaryPayout = PaymentsMethod.PaymentTypes.Wallet,
        WalletCreationOnFirstPayment = true,
        WalletCreationInputs = new PaymentsCountryDetails.WalletCreationOptions[2]
        {
          PaymentsCountryDetails.WalletCreationOptions.FirstNameCharsOnly,
          PaymentsCountryDetails.WalletCreationOptions.LastNameCharsOnly
        },
        ValidSecondaryPayment = new PaymentsMethod.PaymentTypes[2]
        {
          PaymentsMethod.PaymentTypes.DebitCard,
          PaymentsMethod.PaymentTypes.BankAccount
        },
        DefaultCurrency = "INR",
        MinTransferAmount = 1.0,
        MaxTransferAmount = 5000.0
      };
      return PaymentsCountryDetails._instance;
    }

    public static string GetCurrencyForCountryCode(string countryCode)
    {
      return PaymentsCountryDetails.getPaymentCountryDetails(countryCode)?.Iso3166CountryCode;
    }

    public enum WalletCreationOptions
    {
      FirstNameCharsOnly,
      LastNameCharsOnly,
    }
  }
}
