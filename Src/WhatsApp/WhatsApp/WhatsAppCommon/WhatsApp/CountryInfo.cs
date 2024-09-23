// Decompiled with JetBrains decompiler
// Type: WhatsApp.CountryInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WhatsApp
{
  public class CountryInfo
  {
    public static string PhoneCountryCodeForIndia = "91";
    private static string[] MultiSelectRestrictedPhoneCountryCodes = new string[1]
    {
      CountryInfo.PhoneCountryCodeForIndia
    };
    private Dictionary<string, CountryInfoItem> phoneCodeToItem = new Dictionary<string, CountryInfoItem>();
    private Dictionary<string, CountryInfoItem> isoCodeToItem = new Dictionary<string, CountryInfoItem>();
    private Dictionary<uint, CountryInfoItem> mccToItem = new Dictionary<uint, CountryInfoItem>();
    private List<CountryInfoItem> items = new List<CountryInfoItem>();
    private static string LogHdr = "CI";
    private static CountryInfo instance = (CountryInfo) null;

    public static CountryInfo Instance
    {
      get => CountryInfo.instance ?? (CountryInfo.instance = new CountryInfo());
    }

    private CountryInfo()
    {
      DateTime? start = PerformanceTimer.Start();
      using (Stream stream = AppState.OpenFromXAP("countries.tsv"))
      {
        StreamReader streamReader = new StreamReader(stream, false);
        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
          if (line.Length != 0)
            this.Add(new CountryInfoItem(line));
        }
      }
      PerformanceTimer.End("read countries.tsv", start);
    }

    private void Add(CountryInfoItem cii)
    {
      if (!this.phoneCodeToItem.ContainsKey(cii.PhoneCountryCode) && !string.IsNullOrEmpty(cii.FormatRegexes))
        this.phoneCodeToItem[cii.PhoneCountryCode] = cii;
      if (!this.isoCodeToItem.ContainsKey(cii.IsoCode))
        this.isoCodeToItem[cii.IsoCode] = cii;
      foreach (uint mcC in cii.MCCs)
      {
        if (!this.mccToItem.ContainsKey(mcC))
          this.mccToItem[mcC] = cii;
      }
      this.items.Add(cii);
    }

    public IEnumerable<CountryInfoItem> GetSortedCountryInfos()
    {
      return (IEnumerable<CountryInfoItem>) this.items;
    }

    public CountryInfoItem GetCountryInfoForCountryCode(string code)
    {
      CountryInfoItem countryInfoItem = (CountryInfoItem) null;
      return string.IsNullOrEmpty(code) || !this.phoneCodeToItem.TryGetValue(code, out countryInfoItem) ? (CountryInfoItem) null : countryInfoItem;
    }

    public CountryInfoItem GetCountryInfoForISOCountryCode(string isoCode)
    {
      CountryInfoItem countryInfoItem = (CountryInfoItem) null;
      return string.IsNullOrEmpty(isoCode) || !this.isoCodeToItem.TryGetValue(isoCode, out countryInfoItem) ? (CountryInfoItem) null : countryInfoItem;
    }

    public CountryInfoItem GetCountryForMcc(uint mcc)
    {
      CountryInfoItem countryInfoItem = (CountryInfoItem) null;
      return !this.mccToItem.TryGetValue(mcc, out countryInfoItem) ? (CountryInfoItem) null : countryInfoItem;
    }

    public static int GetMaxMessageRecipientsForCurrCountry()
    {
      int val2 = Settings.MulticastLimitGlobal;
      if (CountryInfo.Instance.IsMulticastRestrictedCountry())
        val2 = Math.Min(Settings.MulticastLimitRestricted, val2);
      return val2;
    }

    private bool IsMulticastRestrictedCountry()
    {
      bool flag = false;
      CountryInfoItem currentCountryInfo = this.GetCurrentCountryInfo();
      if (currentCountryInfo != null)
        flag = ((IEnumerable<string>) CountryInfo.MultiSelectRestrictedPhoneCountryCodes).Contains<string>(currentCountryInfo.PhoneCountryCode);
      return flag;
    }

    private CountryInfoItem GetCurrentCountryInfo()
    {
      CountryInfoItem currentCountryInfo = (CountryInfoItem) null;
      string countryCode = Settings.CountryCode;
      if (countryCode != null)
        currentCountryInfo = this.GetCountryInfoForCountryCode(countryCode);
      if (currentCountryInfo == null)
        Log.l(CountryInfo.LogHdr, "No country info found for {0}", (object) (countryCode ?? "null"));
      return currentCountryInfo;
    }

    internal CountryInfoItem GetFirstCountryInfoWithFormattingInfoByRegionCode(string regionCode)
    {
      foreach (CountryInfoItem infoByRegionCode in this.items)
      {
        if (infoByRegionCode.PhoneCountryCode == regionCode && !string.IsNullOrEmpty(infoByRegionCode.FormatRegexes))
          return infoByRegionCode;
      }
      return (CountryInfoItem) null;
    }
  }
}
