// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MercatorUtility
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public static class MercatorUtility
  {
    public const double MercatorLatitudeLimit = 85.051128;
    public const double EarthRadiusInMeters = 6378137.0;
    public const double EarthCircumferenceInMeters = 40075016.685578488;

    public static Point NormalizeLogicalPoint(Point logicalPoint, Point centerLogicalPoint)
    {
      return logicalPoint.X < centerLogicalPoint.X - 0.5 || logicalPoint.X > centerLogicalPoint.X + 0.5 ? new Point(logicalPoint.X - Math.Floor(logicalPoint.X - centerLogicalPoint.X + 0.5), logicalPoint.Y) : logicalPoint;
    }

    public static GeoCoordinate NormalizeLocation(GeoCoordinate location)
    {
      return new GeoCoordinate()
      {
        Latitude = MercatorUtility.NormalizeLatitude(location.Latitude),
        Longitude = MercatorUtility.NormalizeLongitude(location.Longitude)
      };
    }

    public static double NormalizeLatitude(double latitude)
    {
      return Math.Min(Math.Max(latitude, -85.051128), 85.051128);
    }

    public static double NormalizeLongitude(double longitude)
    {
      return longitude < -180.0 || longitude > 180.0 ? longitude - Math.Floor((longitude + 180.0) / 360.0) * 360.0 : longitude;
    }

    public static Point LocationToLogicalPoint(GeoCoordinate location)
    {
      double y;
      if (location.Latitude > 85.051128)
        y = 0.0;
      else if (location.Latitude < -85.051128)
      {
        y = 1.0;
      }
      else
      {
        double num = Math.Sin(location.Latitude * Math.PI / 180.0);
        y = 0.5 - Math.Log((1.0 + num) / (1.0 - num)) / (4.0 * Math.PI);
      }
      return new Point((location.Longitude + 180.0) / 360.0, y);
    }

    public static GeoCoordinate LogicalPointToLocation(Point logicalPoint)
    {
      return new GeoCoordinate()
      {
        Latitude = 90.0 - 360.0 * Math.Atan(Math.Exp((logicalPoint.Y * 2.0 - 1.0) * Math.PI)) / Math.PI,
        Longitude = MercatorUtility.NormalizeLongitude(360.0 * logicalPoint.X - 180.0)
      };
    }

    public static double ZoomToScale(
      Size logicalAreaSizeInScreenSpaceAtLevel1,
      double zoomLevel,
      GeoCoordinate location)
    {
      double num = 40075016.685578488 / (Math.Pow(2.0, zoomLevel - 1.0) * logicalAreaSizeInScreenSpaceAtLevel1.Width);
      return Math.Cos(MercatorUtility.DegreesToRadians(location.Latitude)) * num;
    }

    public static double ScaleToZoom(
      Size logicalAreaSizeInScreenSpaceAtLevel1,
      double scale,
      GeoCoordinate location)
    {
      return Math.Log(40075016.685578488 / (scale / Math.Cos(MercatorUtility.DegreesToRadians(location.Latitude)) * logicalAreaSizeInScreenSpaceAtLevel1.Width), 2.0) + 1.0;
    }

    public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;
  }
}
