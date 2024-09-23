// Decompiled with JetBrains decompiler
// Type: WhatsApp.PaymentsWaAdminSettings
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class PaymentsWaAdminSettings
  {
    private static object createLock = new object();
    private const byte PaymentsWaAdminSettingsDataFormat = 1;
    private static PaymentsWaAdminSettings _settingsInstance;
    private string countryCode;
    private string currencyCode;

    public string CountryCode => this.countryCode;

    public string CurrencyCode => this.currencyCode;

    public void setCountryAndCurrency(string country, string currency)
    {
      lock (PaymentsWaAdminSettings.createLock)
      {
        this.countryCode = country;
        this.currencyCode = currency;
        this.PersistPaymentsWaAdminSettings();
      }
    }

    public static PaymentsWaAdminSettings GetInstance()
    {
      if (PaymentsWaAdminSettings._settingsInstance == null)
      {
        lock (PaymentsWaAdminSettings.createLock)
        {
          if (PaymentsWaAdminSettings._settingsInstance == null)
            PaymentsWaAdminSettings._settingsInstance = PaymentsWaAdminSettings.CreateSettings();
        }
      }
      return PaymentsWaAdminSettings._settingsInstance;
    }

    private void PersistPaymentsWaAdminSettings()
    {
      BinaryData binaryData = new BinaryData();
      binaryData.AppendByte((byte) 1);
      binaryData.AppendStrWithLengthPrefix(this.CountryCode);
      binaryData.AppendStrWithLengthPrefix(this.CurrencyCode);
      Settings.PaymentsWaAdminOverrides = binaryData.Get();
    }

    private static PaymentsWaAdminSettings CreateSettings()
    {
      PaymentsWaAdminSettings settings = new PaymentsWaAdminSettings();
      try
      {
        if (Settings.IsWaAdmin)
        {
          byte[] waAdminOverrides = Settings.PaymentsWaAdminOverrides;
          if (waAdminOverrides != null)
          {
            if (waAdminOverrides.Length != 0)
            {
              BinaryData binaryData = new BinaryData(waAdminOverrides);
              if (binaryData.ReadByte(0) != (byte) 1)
              {
                Log.l("Payments", "invalid data in WaAdmin Payments settings");
              }
              else
              {
                int newOffset = 1;
                settings.countryCode = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
                settings.currencyCode = binaryData.ReadStrWithLengthPrefix(newOffset, out newOffset);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.LogException(ex, "Exception retrieving WaAdmin Payments settings");
        settings = new PaymentsWaAdminSettings();
      }
      return settings;
    }
  }
}
