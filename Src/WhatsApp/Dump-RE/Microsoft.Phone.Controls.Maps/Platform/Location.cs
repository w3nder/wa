// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Platform.Location
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Device.Location;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Platform
{
  [DataContract(Namespace = "http://dev.virtualearth.net/webservices/v1/common")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class Location : IFormattable
  {
    [DataMember]
    public double Latitude { get; set; }

    [DataMember]
    public double Longitude { get; set; }

    [DataMember]
    public double Altitude { get; set; }

    string IFormattable.ToString(string format, IFormatProvider provider)
    {
      return string.Format(provider, "{0:" + format + "},{1:" + format + "},{2:" + format + "}", (object) this.Latitude, (object) this.Longitude, (object) this.Altitude);
    }

    public override string ToString()
    {
      return ((IFormattable) this).ToString((string) null, (IFormatProvider) null);
    }

    public string ToString(IFormatProvider provider)
    {
      return ((IFormattable) this).ToString((string) null, provider);
    }

    public static implicit operator GeoCoordinate(Microsoft.Phone.Controls.Maps.Platform.Location obj)
    {
      return new GeoCoordinate()
      {
        Latitude = obj.Latitude,
        Longitude = obj.Longitude,
        Altitude = obj.Altitude
      };
    }

    public static implicit operator Microsoft.Phone.Controls.Maps.Platform.Location(
      GeoCoordinate obj)
    {
      return new Microsoft.Phone.Controls.Maps.Platform.Location()
      {
        Latitude = obj.Latitude,
        Longitude = obj.Longitude,
        Altitude = double.IsNaN(obj.Altitude) ? 0.0 : obj.Altitude
      };
    }
  }
}
