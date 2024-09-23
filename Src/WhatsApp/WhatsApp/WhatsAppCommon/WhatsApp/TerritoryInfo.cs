// Decompiled with JetBrains decompiler
// Type: WhatsApp.TerritoryInfo
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System.Collections.Generic;


namespace WhatsApp
{
  public class TerritoryInfo
  {
    private string territoryCode_;
    private string regionCode_;
    private List<FormatInfo> availableFormats_;

    public TerritoryInfo(string territoryCode, string regionCode)
    {
      this.territoryCode_ = territoryCode;
      this.regionCode_ = regionCode;
      this.availableFormats_ = new List<FormatInfo>();
    }

    public string TerritoryCode => this.territoryCode_;

    public string RegionCode => this.regionCode_;

    public List<FormatInfo> AvailableFormats => this.availableFormats_;

    public List<int> AllowedLengths { get; set; }

    public void addAvailableFormat(
      string matchPattern,
      string format,
      string[] leadingDigitsPatterns)
    {
      this.availableFormats_.Add(new FormatInfo(matchPattern, format, leadingDigitsPatterns));
    }
  }
}
