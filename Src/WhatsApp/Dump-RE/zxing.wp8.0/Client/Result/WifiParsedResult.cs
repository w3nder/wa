// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.WifiParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary>
  /// 
  /// </summary>
  /// <author>Vikram Aggarwal</author>
  public class WifiParsedResult : ParsedResult
  {
    public WifiParsedResult(string networkEncryption, string ssid, string password)
      : this(networkEncryption, ssid, password, false)
    {
    }

    public WifiParsedResult(string networkEncryption, string ssid, string password, bool hidden)
      : base(ParsedResultType.WIFI)
    {
      this.Ssid = ssid;
      this.NetworkEncryption = networkEncryption;
      this.Password = password;
      this.Hidden = hidden;
      StringBuilder result = new StringBuilder(80);
      ParsedResult.maybeAppend(this.Ssid, result);
      ParsedResult.maybeAppend(this.NetworkEncryption, result);
      ParsedResult.maybeAppend(this.Password, result);
      ParsedResult.maybeAppend(hidden.ToString(), result);
      this.displayResultValue = result.ToString();
    }

    public string Ssid { get; private set; }

    public string NetworkEncryption { get; private set; }

    public string Password { get; private set; }

    public bool Hidden { get; private set; }
  }
}
