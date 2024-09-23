// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapMode
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public abstract class MapMode
  {
    private static readonly Range<double> defaultZoomRange = new Range<double>(1.0, 21.0);
    private static readonly Range<double> defaultHeadingRange = new Range<double>(0.0, 360.0);
    private static readonly Range<double> defaultPitchRange = new Range<double>(-90.0, 90.0);
    private readonly AttributionCollection attributions = new AttributionCollection(new ObservableCollection<AttributionInfo>());
    private CultureInfo culture;

    public event EventHandler<MapEventArgs> TargetViewChanged;

    public event EventHandler<ProjectionChangedEventArgs> ProjectionChanged;

    public CultureInfo Culture
    {
      get => this.culture != null ? this.culture : CultureInfo.CurrentUICulture;
      set
      {
        this.culture = value;
        this.OnCultureChanged();
      }
    }

    public virtual GeoCoordinate Center { get; set; }

    public virtual GeoCoordinate TargetCenter => this.Center;

    public virtual double ZoomLevel { get; set; }

    public virtual double TargetZoomLevel => this.ZoomLevel;

    public virtual double Heading { get; set; }

    public virtual double TargetHeading => this.Heading;

    public virtual double Pitch { get; set; }

    public virtual double TargetPitch => this.Pitch;

    public virtual double Scale { get; set; }

    public virtual double TargetScale => this.Scale;

    public virtual Range<double> ZoomRange => MapMode.defaultZoomRange;

    public virtual Range<double> HeadingRange => MapMode.defaultHeadingRange;

    public virtual Range<double> PitchRange => MapMode.defaultPitchRange;

    public virtual LocationRect BoundingRectangle
    {
      get
      {
        return LocationRect.CreateLocationRect(this.ViewportPointToLocation(new Point(0.0, 0.0)), this.ViewportPointToLocation(new Point(0.0, this.ViewportSize.Height)), this.ViewportPointToLocation(new Point(this.ViewportSize.Width, this.ViewportSize.Height)), this.ViewportPointToLocation(new Point(this.ViewportSize.Width, 0.0)));
      }
    }

    public virtual LocationRect TargetBoundingRectangle => this.BoundingRectangle;

    public virtual AttributionCollection Attributions => this.attributions;

    public Size ViewportSize { get; set; }

    public AnimationLevel AnimationLevel { get; set; }

    public virtual CredentialsProvider CredentialsProvider { get; set; }

    public virtual bool IsDownloading => false;

    public virtual bool IsIdle => true;

    public virtual UIElement Content => (UIElement) null;

    public virtual UIElement ForegroundContent => (UIElement) null;

    public virtual ModeBackground ModeBackground => ModeBackground.Dark;

    public abstract void SetView(
      GeoCoordinate center,
      double zoomLevel,
      double heading,
      double pitch,
      bool animate);

    public abstract void SetView(LocationRect boundingRectangle, bool animate);

    public abstract Point LocationToViewportPoint(GeoCoordinate location);

    public virtual IEnumerable<Point> LocationToViewportPoint(IEnumerable<GeoCoordinate> locations)
    {
      return locations.Select<GeoCoordinate, Point>((Func<GeoCoordinate, Point>) (location => this.LocationToViewportPoint(location)));
    }

    public virtual Rect LocationToViewportPoint(LocationRect boundingRectangle)
    {
      return new Rect(this.LocationToViewportPoint(boundingRectangle.Northwest), this.LocationToViewportPoint(boundingRectangle.Southeast));
    }

    public abstract bool TryLocationToViewportPoint(GeoCoordinate location, out Point viewportPoint);

    public abstract GeoCoordinate ViewportPointToLocation(Point viewportPoint);

    public abstract bool TryViewportPointToLocation(Point viewportPoint, out GeoCoordinate location);

    public virtual void OnMapDrag(MapDragEventArgs e)
    {
    }

    public virtual void OnMapFlick(MapFlickEventArgs e)
    {
    }

    public virtual void OnMapZoom(MapZoomEventArgs e)
    {
    }

    public virtual void Activating(
      MapMode previousMode,
      MapLayerBase modeLayer,
      MapLayerBase modeForegroundLayer)
    {
    }

    public virtual void Activated(MapLayerBase modeLayer, MapLayerBase modeForegroundLayer)
    {
    }

    public virtual void Deactivating()
    {
    }

    public virtual void ViewportSizeChanged(Size viewportSize) => this.ViewportSize = viewportSize;

    protected virtual void OnTargetViewChanged()
    {
      EventHandler<MapEventArgs> targetViewChanged = this.TargetViewChanged;
      if (targetViewChanged == null)
        return;
      targetViewChanged((object) this, new MapEventArgs());
    }

    protected virtual void OnProjectionChanged(ProjectionUpdateLevel updateLevel)
    {
      EventHandler<ProjectionChangedEventArgs> projectionChanged = this.ProjectionChanged;
      if (projectionChanged == null)
        return;
      projectionChanged((object) this, new ProjectionChangedEventArgs(updateLevel));
    }

    protected virtual void OnCultureChanged()
    {
    }
  }
}
