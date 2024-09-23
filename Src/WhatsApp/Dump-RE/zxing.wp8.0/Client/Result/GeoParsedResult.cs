// Decompiled with JetBrains decompiler
// Type: ZXing.Client.Result.GeoParsedResult
// Assembly: zxing.wp8.0, Version=0.14.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD293DF0-BBAA-4BF0-BAC7-F5FAF5AC94ED
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.dll
// XML documentation location: C:\Users\Admin\Desktop\RE\WABeta\zxing.wp8.0.xml

using System;
using System.Globalization;
using System.Text;

#nullable disable
namespace ZXing.Client.Result
{
  /// <author>Sean Owen</author>
  public sealed class GeoParsedResult : ParsedResult
  {
    internal GeoParsedResult(double latitude, double longitude, double altitude, string query)
      : base(ParsedResultType.GEO)
    {
      this.Latitude = latitude;
      this.Longitude = longitude;
      this.Altitude = altitude;
      this.Query = query;
      this.GeoURI = this.getGeoURI();
      this.GoogleMapsURI = this.getGoogleMapsURI();
      this.displayResultValue = this.getDisplayResult();
    }

    /// <returns>latitude in degrees</returns>
    public double Latitude { get; private set; }

    /// <returns>longitude in degrees</returns>
    public double Longitude { get; private set; }

    /// <returns>altitude in meters. If not specified, in the geo URI, returns 0.0</returns>
    public double Altitude { get; private set; }

    /// <return> query string associated with geo URI or null if none exists</return>
    public string Query { get; private set; }

    public string GeoURI { get; private set; }

    /// <returns> a URI link to Google Maps which display the point on the Earth described
    /// by this instance, and sets the zoom level in a way that roughly reflects the
    /// altitude, if specified
    /// </returns>
    public string GoogleMapsURI { get; private set; }

    private string getDisplayResult()
    {
      StringBuilder stringBuilder = new StringBuilder(20);
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:0.0###########}", (object) this.Latitude);
      stringBuilder.Append(", ");
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:0.0###########}", (object) this.Longitude);
      if (this.Altitude > 0.0)
      {
        stringBuilder.Append(", ");
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0:0.0###########}", (object) this.Altitude);
        stringBuilder.Append('m');
      }
      if (this.Query != null)
      {
        stringBuilder.Append(" (");
        stringBuilder.Append(this.Query);
        stringBuilder.Append(')');
      }
      return stringBuilder.ToString();
    }

    private string getGeoURI()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("geo:");
      stringBuilder.Append(this.Latitude);
      stringBuilder.Append(',');
      stringBuilder.Append(this.Longitude);
      if (this.Altitude > 0.0)
      {
        stringBuilder.Append(',');
        stringBuilder.Append(this.Altitude);
      }
      if (this.Query != null)
      {
        stringBuilder.Append('?');
        stringBuilder.Append(this.Query);
      }
      return stringBuilder.ToString();
    }

    private string getGoogleMapsURI()
    {
      StringBuilder stringBuilder = new StringBuilder(50);
      stringBuilder.Append("http://maps.google.com/?ll=");
      stringBuilder.Append(this.Latitude);
      stringBuilder.Append(',');
      stringBuilder.Append(this.Longitude);
      if (this.Altitude > 0.0)
      {
        int num1 = (int) (this.Altitude * 3.28 / 1000.0);
        int num2;
        for (num2 = 0; num1 > 1 && num2 < 18; ++num2)
          num1 >>= 1;
        int num3 = 19 - num2;
        stringBuilder.Append("&z=");
        stringBuilder.Append(num3);
      }
      return stringBuilder.ToString();
    }
  }
}
