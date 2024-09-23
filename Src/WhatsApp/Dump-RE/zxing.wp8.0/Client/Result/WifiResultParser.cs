// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.WifiResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// <p>Parses a WIFI configuration string. Strings will be of the form:</p>
  /// <p>{@code WIFI:T:[network type];S:[network SSID];P:[network password];H:[hidden?];;}</p>
  /// <p>The fields can appear in any order. Only "S:" is required.</p>
  /// </summary>
  /// <author>Vikram Aggarwal</author>
  /// <author>Sean Owen</author>
  public class WifiResultParser : ResultParser
  {
    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (!text.StartsWith("WIFI:"))
        return (ParsedResult) null;
      string ssid = ResultParser.matchSinglePrefixedField("S:", text, ';', false);
      if (string.IsNullOrEmpty(ssid))
        return (ParsedResult) null;
      string password = ResultParser.matchSinglePrefixedField("P:", text, ';', false);
      string networkEncryption = ResultParser.matchSinglePrefixedField("T:", text, ';', false) ?? "nopass";
      bool result1 = false;
      bool.TryParse(ResultParser.matchSinglePrefixedField("H:", text, ';', false), out result1);
      return (ParsedResult) new WifiParsedResult(networkEncryption, ssid, password, result1);
    }
  }
}
