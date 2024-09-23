// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MercatorMode
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MercatorMode : FlatMapMode
  {
    private const int refreshMilliseconds = 500;
    internal const string configurationVersion = "v1";
    internal const string configurationSection = "WP7SLMapControl";
    internal static readonly Size LogicalAreaSizeInScreenSpaceAtLevel1 = new Size(512.0, 512.0);
    private static readonly TimeSpan panningRefreshTime = new TimeSpan(0, 0, 0, 0, 100);
    private static readonly TimeSpan fullRefreshTime = new TimeSpan(0, 0, 0, 0);
    private static readonly long refreshTicks = new TimeSpan(0, 0, 0, 0, 500).Ticks;
    private readonly AttributionCollection attributions;
    private readonly ObservableCollection<AttributionInfo> attributionsBase;
    private readonly CopyrightManager copyright;
    private readonly Range<double> headingRange = new Range<double>(0.0, 0.0);
    private readonly Range<double> pitchRange = new Range<double>(-90.0, -90.0);
    private readonly Storyboard storyboardOnFrame;
    private readonly Range<double> zoomRange = new Range<double>(1.0, 21.0);
    private long lastOverlayRefresh = long.MaxValue;
    private bool projectionChangedFiredThisFrame;

    public MercatorMode()
      : base(MercatorMode.LogicalAreaSizeInScreenSpaceAtLevel1)
    {
      this.attributionsBase = new ObservableCollection<AttributionInfo>();
      this.attributions = new AttributionCollection(this.attributionsBase);
      this.storyboardOnFrame = new Storyboard();
      this.storyboardOnFrame.Completed += new EventHandler(this.storyboardOnFrame_Completed);
      this.copyright = CopyrightManager.GetInstance();
    }

    public override AttributionCollection Attributions => this.attributions;

    public override double Heading => 0.0;

    public override double Pitch => -90.0;

    public override double Scale
    {
      get
      {
        return MercatorUtility.ZoomToScale(MercatorMode.LogicalAreaSizeInScreenSpaceAtLevel1, this.ZoomLevel, this.Center);
      }
      set
      {
        this.ZoomLevel = MercatorUtility.ScaleToZoom(MercatorMode.LogicalAreaSizeInScreenSpaceAtLevel1, value, this.Center);
      }
    }

    public override double TargetScale
    {
      get
      {
        return MercatorUtility.ZoomToScale(MercatorMode.LogicalAreaSizeInScreenSpaceAtLevel1, this.TargetZoomLevel, this.TargetCenter);
      }
    }

    public override Range<double> ZoomRange => this.GetZoomRange(this.Center);

    public override Range<double> HeadingRange => this.headingRange;

    public override Range<double> PitchRange => this.pitchRange;

    public override LocationRect BoundingRectangle
    {
      get
      {
        return this.GetBoundingRectangle(this.ViewportPointToLogicalPoint(new Point(0.0, 0.0)), this.ViewportPointToLogicalPoint(new Point(this.ViewportSize.Width, this.ViewportSize.Height)));
      }
    }

    public override LocationRect TargetBoundingRectangle
    {
      get
      {
        return this.GetBoundingRectangle(this.TargetViewportDefinition.ViewportPointToLogicalPoint(new Point(0.0, 0.0)), this.TargetViewportDefinition.ViewportPointToLogicalPoint(new Point(this.ViewportSize.Width, this.ViewportSize.Height)));
      }
    }

    protected override Point LocationToLogicalPoint(GeoCoordinate location)
    {
      return MercatorUtility.NormalizeLogicalPoint(MercatorUtility.LocationToLogicalPoint(location), this.CurrentLogicalCenter);
    }

    public override IEnumerable<Point> LocationToLogicalPoint(IEnumerable<GeoCoordinate> locations)
    {
      List<Point> logicalPoint = new List<Point>();
      Point centerLogicalPoint = this.CurrentLogicalCenter;
      foreach (GeoCoordinate location in locations)
      {
        Point point = MercatorUtility.NormalizeLogicalPoint(MercatorUtility.LocationToLogicalPoint(location), centerLogicalPoint);
        logicalPoint.Add(point);
        centerLogicalPoint = point;
      }
      return (IEnumerable<Point>) logicalPoint;
    }

    public override Rect LocationRectToLogicalRect(LocationRect boundingRectangle)
    {
      Point logicalPoint1 = MercatorUtility.LocationToLogicalPoint(new GeoCoordinate()
      {
        Latitude = boundingRectangle.Center.Latitude + boundingRectangle.Height / 2.0,
        Longitude = boundingRectangle.Center.Longitude - boundingRectangle.Width / 2.0
      });
      Point logicalPoint2 = MercatorUtility.LocationToLogicalPoint(new GeoCoordinate()
      {
        Latitude = boundingRectangle.Center.Latitude - boundingRectangle.Height / 2.0,
        Longitude = boundingRectangle.Center.Longitude + boundingRectangle.Width / 2.0
      });
      Point logicalPoint3 = MercatorUtility.LocationToLogicalPoint(boundingRectangle.Center);
      Point point = MercatorUtility.NormalizeLogicalPoint(logicalPoint3, this.CurrentLogicalCenter);
      double num1 = point.X - logicalPoint3.X;
      double num2 = point.Y - logicalPoint3.Y;
      logicalPoint1.X += num1;
      logicalPoint1.Y += num2;
      logicalPoint2.X += num1;
      logicalPoint2.Y += num2;
      return new Rect(logicalPoint1, logicalPoint2);
    }

    protected override GeoCoordinate LogicalPointToLocation(Point logicalPoint)
    {
      return MercatorUtility.NormalizeLocation(MercatorUtility.LogicalPointToLocation(logicalPoint));
    }

    public override bool ConstrainView(
      GeoCoordinate center,
      ref double zoomLevel,
      ref double heading,
      ref double pitch)
    {
      bool flag = false;
      GeoCoordinate center1 = MercatorUtility.NormalizeLocation(center);
      if (center1 != center)
      {
        flag = true;
        center.Latitude = center1.Latitude;
        center.Longitude = center1.Longitude;
        center.Altitude = center1.Altitude;
      }
      double num = zoomLevel;
      Range<double> zoomRange = this.GetZoomRange(center1);
      zoomLevel = Math.Min(zoomLevel, zoomRange.To);
      zoomLevel = Math.Max(zoomLevel, zoomRange.From);
      return flag || num != zoomLevel;
    }

    protected override void OnProjectionChanged(ProjectionUpdateLevel updateLevel)
    {
      base.OnProjectionChanged(updateLevel);
      this.UpdateAttribution(updateLevel);
    }

    protected virtual Range<double> GetZoomRange(GeoCoordinate center) => this.zoomRange;

    internal Range<double> GetZoomRange(TfeTileSource tileSource, GeoCoordinate center)
    {
      Range<double> zoomRange = this.zoomRange;
      double from = zoomRange.From;
      double num1 = zoomRange.To;
      if (tileSource != null && tileSource.CoverageMapLoaded)
      {
        Point logicalPoint = this.LocationToLogicalPoint(center);
        if (logicalPoint.X < 0.0 || logicalPoint.X > 1.0)
          logicalPoint.X -= Math.Floor(logicalPoint.X);
        if (logicalPoint.Y < 0.0)
          logicalPoint.Y = 0.0;
        else if (logicalPoint.Y > 1.0)
          logicalPoint.Y = 1.0;
        int num2 = (int) Math.Floor((double) tileSource.MaxZoomLevel);
        double num3 = Math.Pow(2.0, (double) num2) - 1.0;
        QuadKey quadKey = new QuadKey((int) Math.Round(logicalPoint.X * num3), (int) Math.Round(logicalPoint.Y * num3), num2);
        num1 = Math.Min(num1, (double) tileSource.GetMaximumZoomLevel(quadKey));
      }
      return new Range<double>(from, num1);
    }

    internal virtual void UpdateAttribution()
    {
    }

    internal void UpdateAttribution(IList<string> copyrightStrings)
    {
      Collection<string> collection1 = new Collection<string>(copyrightStrings);
      Collection<AttributionInfo> collection2 = new Collection<AttributionInfo>();
      foreach (AttributionInfo attributionInfo in (Collection<AttributionInfo>) this.attributionsBase)
      {
        if (collection1.Contains(attributionInfo.Text))
          collection1.Remove(attributionInfo.Text);
        else
          collection2.Add(attributionInfo);
      }
      foreach (AttributionInfo attributionInfo in collection2)
        this.attributionsBase.Remove(attributionInfo);
      foreach (string str in collection1)
        this.attributionsBase.Add(new AttributionInfo()
        {
          Text = str
        });
    }

    internal void UpdateAttribution(ProjectionUpdateLevel updateLevel)
    {
      this.projectionChangedFiredThisFrame = true;
      long ticks = DateTime.Now.Ticks;
      if (ticks - this.lastOverlayRefresh >= MercatorMode.refreshTicks)
      {
        this.lastOverlayRefresh = ticks;
        this.UpdateAttribution();
      }
      else
      {
        if (this.lastOverlayRefresh > ticks)
          this.lastOverlayRefresh = ticks;
        if (updateLevel == ProjectionUpdateLevel.Full)
          this.storyboardOnFrame.Duration = (Duration) MercatorMode.fullRefreshTime;
        else
          this.storyboardOnFrame.Duration = (Duration) MercatorMode.panningRefreshTime;
        this.storyboardOnFrame.Begin();
      }
    }

    internal static bool TryParseSubdomains(string subdomainString, out string[][] subdomains)
    {
      bool subdomains1 = false;
      if (!string.IsNullOrEmpty(subdomainString))
      {
        string[] strArray = subdomainString.Split(' ');
        subdomains = new string[strArray.Length][];
        for (int index = 0; index < strArray.Length; ++index)
          subdomains[index] = strArray[index].Split(',');
        subdomains1 = true;
      }
      else
        subdomains = (string[][]) null;
      return subdomains1;
    }

    private void storyboardOnFrame_Completed(object sender, EventArgs e)
    {
      if (this.projectionChangedFiredThisFrame)
      {
        this.projectionChangedFiredThisFrame = false;
        this.storyboardOnFrame.Begin();
      }
      else
      {
        this.lastOverlayRefresh = long.MaxValue;
        this.UpdateAttribution();
      }
    }

    private Point CurrentLogicalCenter
    {
      get
      {
        return this.CurrentViewportDefinition.ViewportPointToLogicalPoint(new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0));
      }
    }

    private LocationRect GetBoundingRectangle(
      Point topLeftLogicalPoint,
      Point bottomRightLogicalPoint)
    {
      GeoCoordinate location1 = this.LogicalPointToLocation(topLeftLogicalPoint);
      GeoCoordinate location2 = this.LogicalPointToLocation(bottomRightLogicalPoint);
      GeoCoordinate location3 = this.LogicalPointToLocation(new Point((bottomRightLogicalPoint.X + topLeftLogicalPoint.X) / 2.0, 0.0));
      location3.Latitude = (location1.Latitude + location2.Latitude) / 2.0;
      double height = location1.Latitude - location2.Latitude;
      double num = bottomRightLogicalPoint.X - topLeftLogicalPoint.X;
      double width = num <= 1.0 ? num * 360.0 : 360.0;
      return new LocationRect(location3, width, height);
    }
  }
}
