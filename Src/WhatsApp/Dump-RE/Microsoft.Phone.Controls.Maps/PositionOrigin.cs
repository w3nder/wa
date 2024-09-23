// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.PositionOrigin
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [TypeConverter(typeof (PositionOriginConverter))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public struct PositionOrigin
  {
    private double x;
    private double y;
    public static readonly PositionOrigin TopLeft = new PositionOrigin(0.0, 0.0);
    public static readonly PositionOrigin TopCenter = new PositionOrigin(0.5, 0.0);
    public static readonly PositionOrigin TopRight = new PositionOrigin(1.0, 0.0);
    public static readonly PositionOrigin CenterLeft = new PositionOrigin(0.0, 0.5);
    public static readonly PositionOrigin Center = new PositionOrigin(0.5, 0.5);
    public static readonly PositionOrigin CenterRight = new PositionOrigin(1.0, 0.5);
    public static readonly PositionOrigin BottomLeft = new PositionOrigin(0.0, 1.0);
    public static readonly PositionOrigin BottomCenter = new PositionOrigin(0.5, 1.0);
    public static readonly PositionOrigin BottomRight = new PositionOrigin(1.0, 1.0);

    public PositionOrigin(double horizontalOrigin, double verticalOrigin)
    {
      this.x = horizontalOrigin;
      this.y = verticalOrigin;
    }

    public double X
    {
      get => this.x;
      set => this.x = value;
    }

    public double Y
    {
      get => this.y;
      set => this.y = value;
    }

    public override int GetHashCode() => this.x.GetHashCode() ^ this.y.GetHashCode();

    public override bool Equals(object obj) => obj is PositionOrigin origin && this.Equals(origin);

    public bool Equals(PositionOrigin origin) => this.x == origin.x && this.y == origin.y;

    public static bool operator ==(PositionOrigin origin1, PositionOrigin origin2)
    {
      return origin1.Equals(origin2);
    }

    public static bool operator !=(PositionOrigin origin1, PositionOrigin origin2)
    {
      return !origin1.Equals(origin2);
    }
  }
}
