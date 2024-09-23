// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Platform.Rectangle
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Platform
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [DataContract(Namespace = "http://dev.virtualearth.net/webservices/v1/common", Name = "Rectangle")]
  public class Rectangle : ShapeBase
  {
    [DataMember]
    public Location Northeast { get; set; }

    [DataMember]
    public Location Southwest { get; set; }

    public static implicit operator LocationRect(Rectangle obj)
    {
      return new LocationRect(obj.Northeast.Latitude, obj.Southwest.Longitude, obj.Southwest.Latitude, obj.Northeast.Longitude);
    }

    public static implicit operator Rectangle(LocationRect obj)
    {
      return new Rectangle()
      {
        Northeast = (Location) obj.Northeast,
        Southwest = (Location) obj.Southwest
      };
    }
  }
}
