// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.LocationRect
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [TypeConverter(typeof (LocationRectConverter))]
  public class LocationRect : IFormattable
  {
    private const double MaxLatitude = 90.0;
    private const double MinLatitude = -90.0;
    private const double MaxLongitude = 180.0;
    private const double MinLongitude = -180.0;
    private GeoCoordinate center;
    private double halfHeight;
    private double halfWidth;

    public LocationRect() => this.center = new GeoCoordinate(0.0, 0.0);

    public LocationRect(GeoCoordinate center, double width, double height)
    {
      this.center = center;
      this.halfWidth = width / 2.0;
      this.halfHeight = height / 2.0;
    }

    public LocationRect(double north, double west, double south, double east)
      : this()
    {
      this.Init(north, west, south, east);
    }

    public LocationRect(LocationRect rect)
    {
      this.center = new GeoCoordinate(rect.center.Latitude, rect.center.Longitude, rect.center.Altitude);
      this.halfHeight = rect.halfHeight;
      this.halfWidth = rect.halfWidth;
    }

    public static LocationRect CreateLocationRect(params GeoCoordinate[] locations)
    {
      return LocationRect.CreateLocationRect((IEnumerable<GeoCoordinate>) locations);
    }

    public static LocationRect CreateLocationRect(IEnumerable<GeoCoordinate> locations)
    {
      if (locations == null)
        return new LocationRect();
      double num1 = -90.0;
      double num2 = 90.0;
      double num3 = 180.0;
      double num4 = -180.0;
      foreach (GeoCoordinate location in locations)
      {
        num1 = Math.Max(num1, location.Latitude);
        num2 = Math.Min(num2, location.Latitude);
        num3 = Math.Min(num3, location.Longitude);
        num4 = Math.Max(num4, location.Longitude);
      }
      return new LocationRect(num1, num3, num2, num4);
    }

    private void Init(double north, double west, double south, double east)
    {
      if (west > east)
        east += 360.0;
      this.center = new GeoCoordinate()
      {
        Latitude = (south + north) / 2.0,
        Longitude = LocationRect.NormalizeLongitude((west + east) / 2.0)
      };
      this.halfHeight = (north - south) / 2.0;
      this.halfWidth = Math.Abs(east - west) / 2.0;
    }

    public double North
    {
      get => this.center.Latitude + this.halfHeight;
      set => this.Init(value, this.West, this.South, this.East);
    }

    public double West
    {
      get
      {
        return this.halfWidth != 180.0 ? LocationRect.NormalizeLongitude(this.center.Longitude - this.halfWidth) : -180.0;
      }
      set => this.Init(this.North, value, this.South, this.East);
    }

    public double South
    {
      get => this.center.Latitude - this.halfHeight;
      set => this.Init(this.North, this.West, value, this.East);
    }

    public double East
    {
      get
      {
        return this.halfWidth != 180.0 ? LocationRect.NormalizeLongitude(this.center.Longitude + this.halfWidth) : 180.0;
      }
      set => this.Init(this.North, this.West, this.South, value);
    }

    public GeoCoordinate Center => this.center;

    public double Width => this.halfWidth * 2.0;

    public double Height => this.halfHeight * 2.0;

    public GeoCoordinate Northwest
    {
      get
      {
        return new GeoCoordinate()
        {
          Latitude = this.North,
          Longitude = this.West
        };
      }
      set
      {
        if (this.center == (GeoCoordinate) null)
          this.Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
        else
          this.Init(value.Latitude, value.Longitude, this.South, this.East);
      }
    }

    public GeoCoordinate Northeast
    {
      get
      {
        return new GeoCoordinate()
        {
          Latitude = this.North,
          Longitude = this.East
        };
      }
      set
      {
        if (this.center == (GeoCoordinate) null)
          this.Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
        else
          this.Init(value.Latitude, this.West, this.South, value.Longitude);
      }
    }

    public GeoCoordinate Southeast
    {
      get
      {
        return new GeoCoordinate()
        {
          Latitude = this.South,
          Longitude = this.East
        };
      }
      set
      {
        if (this.center == (GeoCoordinate) null)
          this.Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
        else
          this.Init(this.North, this.West, value.Latitude, value.Longitude);
      }
    }

    public GeoCoordinate Southwest
    {
      get
      {
        return new GeoCoordinate()
        {
          Latitude = this.South,
          Longitude = this.West
        };
      }
      set
      {
        if (this.center == (GeoCoordinate) null)
          this.Init(value.Latitude, value.Longitude, value.Latitude, value.Longitude);
        else
          this.Init(this.North, value.Longitude, value.Latitude, this.East);
      }
    }

    string IFormattable.ToString(string format, IFormatProvider provider)
    {
      return string.Format(provider, "{0:" + format + "} {1:" + format + "}", (object) this.Northwest, (object) this.Southeast);
    }

    public bool Equals(LocationRect value) => this == value;

    public override int GetHashCode()
    {
      return this.center.GetHashCode() ^ this.halfWidth.GetHashCode() ^ this.halfHeight.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return obj is LocationRect locationRect && this.center == locationRect.center && this.halfWidth == locationRect.halfWidth && this.halfHeight == locationRect.halfHeight;
    }

    public bool Intersects(LocationRect rect)
    {
      double num1 = Math.Abs(this.center.Latitude - rect.center.Latitude);
      double num2 = Math.Abs(this.center.Longitude - rect.center.Longitude);
      if (num2 > 180.0)
        num2 = 360.0 - num2;
      return num1 <= this.halfHeight + rect.halfHeight && num2 <= this.halfWidth + rect.halfWidth;
    }

    public LocationRect Intersection(LocationRect rect)
    {
      LocationRect locationRect = new LocationRect();
      if (this.Intersects(rect))
      {
        double val1_1 = this.center.Longitude - this.halfWidth;
        double val2_1 = rect.center.Longitude - rect.halfWidth;
        double val1_2 = this.center.Longitude + this.halfWidth;
        double val2_2 = rect.center.Longitude + rect.halfWidth;
        if (Math.Abs(this.center.Longitude - rect.center.Longitude) > 180.0)
        {
          if (this.center.Longitude < rect.center.Longitude)
          {
            val1_1 += 360.0;
            val1_2 += 360.0;
          }
          else
          {
            val2_1 += 360.0;
            val2_2 += 360.0;
          }
        }
        double num1 = Math.Max(val1_1, val2_1);
        double num2 = Math.Min(val1_2, val2_2);
        double num3 = Math.Min(this.North, rect.North);
        double num4 = Math.Max(this.South, rect.South);
        locationRect = new LocationRect(new GeoCoordinate()
        {
          Latitude = (num3 + num4) / 2.0,
          Longitude = LocationRect.NormalizeLongitude((num1 + num2) / 2.0)
        }, num2 - num1, num3 - num4);
      }
      return locationRect;
    }

    public override string ToString()
    {
      return ((IFormattable) this).ToString((string) null, (IFormatProvider) null);
    }

    public string ToString(IFormatProvider provider)
    {
      return ((IFormattable) this).ToString((string) null, provider);
    }

    private static double NormalizeLongitude(double longitude)
    {
      return longitude < -180.0 || longitude > 180.0 ? longitude - Math.Floor((longitude + 180.0) / 360.0) * 360.0 : longitude;
    }
  }
}
