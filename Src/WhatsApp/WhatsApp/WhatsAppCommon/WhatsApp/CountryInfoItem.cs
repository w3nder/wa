// Decompiled with JetBrains decompiler
// Type: WhatsApp.CountryInfoItem
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace WhatsApp
{
  public class CountryInfoItem
  {
    private CountryInfoItem.TosRegions tosRegion;

    public string FullName
    {
      get
      {
        string str = AppState.GetString("Country_" + this.IsoCode);
        return !string.IsNullOrEmpty(str) ? str : this.EnglishName;
      }
    }

    private string EnglishName { get; set; }

    public string IsoCode { get; private set; }

    public string PhoneCountryCode { get; private set; }

    public List<uint> MCCs { get; private set; }

    public List<int> AllowedLengths { get; private set; }

    public List<string> StripLeadingDigits { get; private set; }

    public string MobileRegex { get; private set; }

    public string FormatRegexes { get; private set; }

    public string FormatStrings { get; private set; }

    public string PrefixRegexes { get; private set; }

    public CountryInfoItem(string line)
    {
      string[] strArray = line.Split('\t');
      this.IsoCode = strArray[0];
      this.EnglishName = strArray[1];
      this.PhoneCountryCode = strArray[2];
      this.MCCs = ((IEnumerable<string>) strArray[3].Split(',')).Select<string, uint>((Func<string, uint>) (s => uint.Parse(s))).ToList<uint>();
      string str1 = strArray[4];
      IEnumerable<int> source1;
      if (str1.Length != 0)
        source1 = ((IEnumerable<string>) str1.Split(',')).Select<string, int>((Func<string, int>) (st => int.Parse(st)));
      else
        source1 = Enumerable.Empty<int>();
      this.AllowedLengths = source1.ToList<int>();
      string str2 = strArray[5];
      IEnumerable<string> source2;
      if (str2.Length != 0)
        source2 = (IEnumerable<string>) str2.Split(',');
      else
        source2 = Enumerable.Empty<string>();
      this.StripLeadingDigits = source2.ToList<string>();
      this.MobileRegex = strArray[6];
      this.FormatRegexes = strArray[7];
      this.FormatStrings = strArray[8];
      this.PrefixRegexes = strArray[9];
      if (strArray[13] == "eu")
        this.tosRegion = CountryInfoItem.TosRegions.Eu;
      else if (strArray[13] == "row")
      {
        this.tosRegion = CountryInfoItem.TosRegions.RestOfWorld;
      }
      else
      {
        Log.l("CountryItem", "Invalid tos flow {0}.", strArray[10] == null ? (object) "null" : (object) strArray[10]);
        throw new InvalidDataException("Invalid tos data in countries file");
      }
    }

    public string ApplyLeadingDigitsFilter(string inputString)
    {
      string str;
      return string.IsNullOrEmpty(inputString) || (str = this.StripLeadingDigits.Where<string>((Func<string, bool>) (prefix => inputString.StartsWith(prefix))).FirstOrDefault<string>()) == null ? inputString : inputString.Substring(str.Length);
    }

    public bool IsTosRegionEu() => this.tosRegion == CountryInfoItem.TosRegions.Eu;

    public bool IsTosRegionRow() => this.tosRegion == CountryInfoItem.TosRegions.RestOfWorld;

    private enum TosRegions
    {
      Eu = 1,
      RestOfWorld = 2,
    }
  }
}
