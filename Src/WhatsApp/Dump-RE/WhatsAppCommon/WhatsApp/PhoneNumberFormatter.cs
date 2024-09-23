// Decompiled with JetBrains decompiler
// Type: WhatsApp.PhoneNumberFormatter
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WhatsApp.RegularExpressions;

#nullable disable
namespace WhatsApp
{
  public class PhoneNumberFormatter
  {
    private object territoryCacheLock = new object();
    private Dictionary<string, TerritoryInfo> cachedTerritoryData_ = new Dictionary<string, TerritoryInfo>();
    private const int MAX_LENGTH_REGION_CODE = 3;
    private static PhoneNumberFormatter instance_ = new PhoneNumberFormatter();

    private static PhoneNumberFormatter Instance => PhoneNumberFormatter.instance_;

    private static string Sanitize(string r)
    {
      r = r.Replace(' ', ' ');
      r = "\u202A" + r + "\u202C";
      return r;
    }

    public static string FormatInternationalNumber(
      string number,
      string territoryCode = null,
      bool sanitize = true)
    {
      return PhoneNumberFormatter.Instance.formatInternationalNumberImpl(number, territoryCode, sanitize);
    }

    private PhoneNumberFormatter()
    {
    }

    private TerritoryInfo LoadTerritoryDataByISOCode(string isoCode)
    {
      lock (this.territoryCacheLock)
        return this.LoadTerritoryDataByISOCodeImpl(isoCode);
    }

    private TerritoryInfo LoadTerritoryDataByISOCodeImpl(string isoCode)
    {
      if (string.IsNullOrEmpty(isoCode))
        return (TerritoryInfo) null;
      TerritoryInfo territoryInfo1 = (TerritoryInfo) null;
      this.cachedTerritoryData_.TryGetValue(isoCode, out territoryInfo1);
      if (territoryInfo1 != null)
        return territoryInfo1;
      CountryInfoItem forIsoCountryCode = CountryInfo.Instance.GetCountryInfoForISOCountryCode(isoCode);
      if (forIsoCountryCode == null)
        return (TerritoryInfo) null;
      TerritoryInfo territoryInfo2 = new TerritoryInfo(forIsoCountryCode.IsoCode, forIsoCountryCode.PhoneCountryCode);
      CountryInfoItem countryInfoItem = (CountryInfoItem) null;
      if (string.IsNullOrEmpty(forIsoCountryCode.FormatRegexes))
      {
        CountryInfoItem infoByRegionCode = CountryInfo.Instance.GetFirstCountryInfoWithFormattingInfoByRegionCode(forIsoCountryCode.PhoneCountryCode);
        if (infoByRegionCode != null)
          countryInfoItem = infoByRegionCode;
      }
      else
        countryInfoItem = forIsoCountryCode;
      if (countryInfoItem != null)
      {
        char[] charArray1 = ";".ToCharArray();
        string[] strArray1 = countryInfoItem.FormatRegexes.Split(charArray1);
        string[] strArray2 = countryInfoItem.FormatStrings.Split(charArray1);
        string[] strArray3 = countryInfoItem.PrefixRegexes.Split(charArray1);
        territoryInfo2.AllowedLengths = countryInfoItem.AllowedLengths;
        int length = strArray1.Length;
        char[] charArray2 = "#".ToCharArray();
        for (int index = 0; index < length; ++index)
        {
          string[] leadingDigitsPatterns = (string[]) null;
          string matchPattern = strArray1[index];
          string format = strArray2[index];
          string str = strArray3[index].Trim();
          if (str != "N/A")
            leadingDigitsPatterns = str.Split(charArray2);
          territoryInfo2.addAvailableFormat(matchPattern, format, leadingDigitsPatterns);
        }
      }
      this.cachedTerritoryData_.Add(territoryInfo2.TerritoryCode, territoryInfo2);
      return territoryInfo2;
    }

    private string formatNationalNumber(string input, string isoCode)
    {
      return this.formatNationalNumberPart(input, this.LoadTerritoryDataByISOCode(isoCode));
    }

    private string formatNationalNumberPart(string input, TerritoryInfo territoryInfo)
    {
      foreach (FormatInfo availableFormat in territoryInfo.AvailableFormats)
      {
        Regex regex = ((IEnumerable<Regex>) availableFormat.LeadingDigitsPatterns).LastOrDefault<Regex>();
        if (regex != null)
        {
          Match match = regex.Match(input);
          if (!match.Success || match.Index != 0)
            continue;
        }
        Match match1 = availableFormat.NumberPattern.Match(input);
        if (match1.Success && match1.Length == input.Length)
          return availableFormat.NumberPattern.Replace(input, availableFormat.NumberFormat);
      }
      return input;
    }

    private string asYouTypeFormat(string input, TerritoryInfo territory)
    {
      if (input.Length > territory.AllowedLengths.Max())
        return input;
      int totalWidth = territory.AllowedLengths.Min();
      string paddedInput = input.Length < totalWidth ? input.PadRight(totalWidth, '0') : input;
      var data = territory.AvailableFormats.Where<FormatInfo>((Func<FormatInfo, bool>) (formatInfo => formatInfo.LeadingDigitsPatterns.Length == 0 || ((IEnumerable<Regex>) formatInfo.LeadingDigitsPatterns).Where<Regex>((Func<Regex, bool>) (pattern =>
      {
        Match match = pattern.Match(paddedInput);
        return match.Success && match.Index == 0;
      })).Any<Regex>())).Select(formatInfo => new
      {
        FormatInfo = formatInfo,
        Match = formatInfo.NumberPattern.Match(paddedInput)
      }).Where(p => p.Match.Success).MaxOfFunc(p => p.Match.Length);
      if (data != null)
      {
        paddedInput = data.FormatInfo.NumberPattern.Replace(paddedInput, data.FormatInfo.NumberFormat);
        int num1 = 0;
        int num2 = 0;
        while (num2 < input.Length)
        {
          if (char.IsDigit(paddedInput, num1))
            ++num2;
          ++num1;
        }
        input = paddedInput.Substring(0, num1);
      }
      return input;
    }

    private string formatInternationalNumberImpl(string input, string isoCode = null, bool sanitize = true)
    {
      if (string.IsNullOrEmpty(input))
        return input;
      string rawNumber = input[0] == '+' ? input.Substring(1) : input;
      TerritoryInfo territoryInfo = (TerritoryInfo) null;
      if (!string.IsNullOrEmpty(isoCode))
      {
        territoryInfo = this.LoadTerritoryDataByISOCode(isoCode);
        if (territoryInfo != null && rawNumber.IndexOf(territoryInfo.RegionCode) != 0)
          territoryInfo = (TerritoryInfo) null;
      }
      if (territoryInfo == null)
      {
        isoCode = this.getISOCodeFromRawNumber(rawNumber);
        if (isoCode != null)
          territoryInfo = this.LoadTerritoryDataByISOCode(isoCode);
        if (territoryInfo == null)
        {
          Log.d("formatInternationalNumber", "Failure to convert iso code: {0} {1}", (object) input, (object) isoCode);
          return "+" + rawNumber;
        }
      }
      string str = this.formatNationalNumberPart(rawNumber.Substring(territoryInfo.RegionCode.Length), territoryInfo);
      string r = "+" + territoryInfo.RegionCode + " " + str;
      return !sanitize ? r : PhoneNumberFormatter.Sanitize(r);
    }

    private string getISOCodeFromRawNumber(string rawNumber)
    {
      return PhoneNumberFormatter.getCountryInfoFromRawNumber(rawNumber)?.IsoCode;
    }

    public static CountryInfoItem getCountryInfoFromRawNumber(string rawNumber)
    {
      int length1 = rawNumber.Length;
      for (int length2 = 1; length2 <= 3 && length2 < length1; ++length2)
      {
        CountryInfoItem infoForCountryCode = CountryInfo.Instance.GetCountryInfoForCountryCode(rawNumber.Substring(0, length2));
        if (infoForCountryCode != null)
          return infoForCountryCode;
      }
      return (CountryInfoItem) null;
    }

    public static void InstallAsYouTypeFormatter(TextBox ccBox, TextBox numberBox)
    {
      string cc = ccBox.Text.ExtractDigits();
      string number = numberBox.Text.ExtractDigits();
      TerritoryInfo territory = (TerritoryInfo) null;
      Action numberUpdated = (Action) (() =>
      {
        if (territory == null)
          return;
        int num = 0;
        for (int index = 0; index < numberBox.SelectionStart; ++index)
        {
          if (char.IsDigit(numberBox.Text[index]))
            ++num;
        }
        string str = (string) null;
        try
        {
          str = PhoneNumberFormatter.Sanitize(PhoneNumberFormatter.Instance.asYouTypeFormat(number, territory));
        }
        catch (Exception ex)
        {
          Log.SendCrashLog(ex, "as you type formatting");
        }
        if (str == null)
          return;
        numberBox.Text = str;
        if (num <= 0)
          return;
        for (int index = 0; index < str.Length; ++index)
        {
          if (char.IsDigit(str[index]))
            --num;
          if (num == 0)
          {
            numberBox.SelectionStart = index + 1;
            break;
          }
        }
      });
      Action ccUpdated = (Action) (() =>
      {
        territory = (TerritoryInfo) null;
        CountryInfoItem infoForCountryCode = CountryInfo.Instance.GetCountryInfoForCountryCode(cc);
        if (infoForCountryCode == null)
        {
          numberBox.Text = numberBox.Text.ExtractDigits();
        }
        else
        {
          try
          {
            territory = PhoneNumberFormatter.Instance.LoadTerritoryDataByISOCode(infoForCountryCode.IsoCode);
            numberUpdated();
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "as you type formatting");
          }
        }
      });
      ccUpdated();
      ccBox.TextChanged += (TextChangedEventHandler) ((sender, args) =>
      {
        string digits = ccBox.Text.ExtractDigits();
        if (!(cc != digits))
          return;
        cc = digits;
        ccUpdated();
      });
      numberBox.TextChanged += (TextChangedEventHandler) ((sender, args) =>
      {
        string digits = numberBox.Text.ExtractDigits();
        if (!(digits != number))
          return;
        number = digits;
        numberUpdated();
      });
    }
  }
}
