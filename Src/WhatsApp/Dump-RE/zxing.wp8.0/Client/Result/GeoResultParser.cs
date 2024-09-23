// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.GeoResultParser
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text.RegularExpressions;

#nullable disable
namespace ZXing.Client.Result
{
  /// <summary> Parses a "geo:" URI result, which specifies a location on the surface of
  /// the Earth as well as an optional altitude above the surface. See
  /// <a href="http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00">
  /// http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00</a>.
  /// 
  /// </summary>
  /// <author>Sean Owen</author>
  /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source
  /// </author>
  internal sealed class GeoResultParser : ResultParser
  {
    private static readonly Regex GEO_URL_PATTERN = new Regex("\\A(?:geo:([\\-0-9.]+),([\\-0-9.]+)(?:,([\\-0-9.]+))?(?:\\?(.*))?)\\z", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public override ParsedResult parse(ZXing.Result result)
    {
      string text = result.Text;
      if (text == null)
        return (ParsedResult) null;
      Match match = GeoResultParser.GEO_URL_PATTERN.Match(text);
      if (!match.Success)
        return (ParsedResult) null;
      string query = match.Groups[4].Value;
      if (string.IsNullOrEmpty(query))
        query = (string) null;
      double result1 = 0.0;
      double result2;
      if (!double.TryParse(match.Groups[1].Value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        return (ParsedResult) null;
      if (result2 > 90.0 || result2 < -90.0)
        return (ParsedResult) null;
      double result3;
      if (!double.TryParse(match.Groups[2].Value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result3))
        return (ParsedResult) null;
      if (result3 > 180.0 || result3 < -180.0)
        return (ParsedResult) null;
      if (!string.IsNullOrEmpty(match.Groups[3].Value))
      {
        if (!double.TryParse(match.Groups[3].Value, NumberStyles.Float, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
          return (ParsedResult) null;
        if (result1 < 0.0)
          return (ParsedResult) null;
      }
      return (ParsedResult) new GeoParsedResult(result2, result3, result1, query);
    }
  }
}
