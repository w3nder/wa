// Decompiled with JetBrains decompiler
// Type: Windows.Devices.Geolocation.GeocoordinateExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System.Device.Location;

#nullable disable
namespace Windows.Devices.Geolocation
{
  public static class GeocoordinateExtensions
  {
    public static GeoCoordinate ToGeoCoordinate(this Geocoordinate geocoordinate)
    {
      if (geocoordinate == null)
        return (GeoCoordinate) null;
      return new GeoCoordinate()
      {
        Altitude = geocoordinate.Altitude ?? double.NaN,
        Course = geocoordinate.Heading ?? double.NaN,
        HorizontalAccuracy = geocoordinate.Accuracy,
        Latitude = geocoordinate.Latitude,
        Longitude = geocoordinate.Longitude,
        Speed = geocoordinate.Speed ?? double.NaN,
        VerticalAccuracy = geocoordinate.AltitudeAccuracy ?? double.NaN
      };
    }
  }
}
