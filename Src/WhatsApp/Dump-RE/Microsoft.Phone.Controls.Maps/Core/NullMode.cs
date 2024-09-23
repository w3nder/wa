// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.NullMode
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
  public class NullMode : MapMode
  {
    public NullMode() => this.Center = new GeoCoordinate(0.0, 0.0);

    public override void SetView(
      GeoCoordinate center,
      double zoomLevel,
      double heading,
      double pitch,
      bool animate)
    {
      this.Center = center;
      this.ZoomLevel = zoomLevel;
      this.Heading = heading;
      this.Pitch = pitch;
      this.OnTargetViewChanged();
    }

    public override void SetView(LocationRect boundingRectangle, bool animate)
    {
      this.OnTargetViewChanged();
    }

    public override Point LocationToViewportPoint(GeoCoordinate location) => new Point();

    public override bool TryLocationToViewportPoint(GeoCoordinate location, out Point viewportPoint)
    {
      viewportPoint = new Point();
      return true;
    }

    public override GeoCoordinate ViewportPointToLocation(Point viewportPoint)
    {
      return new GeoCoordinate(0.0, 0.0);
    }

    public override bool TryViewportPointToLocation(Point viewportPoint, out GeoCoordinate location)
    {
      location = new GeoCoordinate(0.0, 0.0);
      return true;
    }
  }
}
