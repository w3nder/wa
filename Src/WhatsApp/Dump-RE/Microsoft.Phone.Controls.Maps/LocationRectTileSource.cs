// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.LocationRectTileSource
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.ComponentModel;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class LocationRectTileSource : TileSource
  {
    public LocationRectTileSource()
    {
    }

    public LocationRectTileSource(
      string uriFormat,
      LocationRect boundingRectangle,
      Range<double> zoomRange)
      : base(uriFormat)
    {
      this.BoundingRectangle = boundingRectangle;
      this.ZoomRange = zoomRange;
    }

    [TypeConverter(typeof (LocationRectConverter))]
    public LocationRect BoundingRectangle { get; set; }

    [TypeConverter(typeof (RangeConverter))]
    public Range<double> ZoomRange { get; set; }

    public override Uri GetUri(int x, int y, int zoomLevel)
    {
      Uri uri = (Uri) null;
      if (this.Covers(x, y, zoomLevel))
        uri = base.GetUri(x, y, zoomLevel);
      return uri;
    }

    private bool Covers(int x, int y, int zoomLevel)
    {
      bool flag = true;
      if (this.ZoomRange != null)
        flag = (double) zoomLevel >= this.ZoomRange.From && (double) zoomLevel <= this.ZoomRange.To;
      if (flag && this.BoundingRectangle != null)
        flag = this.BoundingRectangle.Intersects(LocationRectTileSource.GetBoundingRectangle(x, y, zoomLevel));
      return flag;
    }

    private static LocationRect GetBoundingRectangle(int x, int y, int zoomLevel)
    {
      double num = 1.0 / Math.Pow(2.0, (double) zoomLevel);
      Rect rect = new Rect((double) x * num, (double) y * num, num, num);
      return LocationRect.CreateLocationRect(MercatorUtility.LogicalPointToLocation(new Point(rect.Left, rect.Top)), MercatorUtility.LogicalPointToLocation(new Point(rect.Right, rect.Bottom)));
    }
  }
}
