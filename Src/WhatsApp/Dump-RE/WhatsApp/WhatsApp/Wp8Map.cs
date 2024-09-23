// Decompiled with JetBrains decompiler
// Type: WhatsApp.Wp8Map
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Reactive;
using System;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public class Wp8Map : MapImplBase
  {
    private Map Map;
    private MapLayer pinLayer_;
    private MapLayer locationLayer_;
    private MapLayer signLayer_;

    private MapLayer PinLayer
    {
      get
      {
        if (this.pinLayer_ == null)
        {
          this.pinLayer_ = new MapLayer();
          this.Map.Layers.Insert(0, this.pinLayer_);
        }
        return this.pinLayer_;
      }
    }

    private MapLayer LocationLayer
    {
      get
      {
        if (this.locationLayer_ == null)
        {
          this.locationLayer_ = new MapLayer();
          if (this.Map.Layers.Count > 0)
          {
            if (this.pinLayer_ != null)
              this.Map.Layers.Insert(1, this.locationLayer_);
            else
              this.Map.Layers.Insert(0, this.locationLayer_);
          }
          else
            this.Map.Layers.Add(this.locationLayer_);
        }
        return this.locationLayer_;
      }
    }

    private MapLayer SignLayer
    {
      get
      {
        if (this.signLayer_ == null)
        {
          this.signLayer_ = new MapLayer();
          this.Map.Layers.Add(this.signLayer_);
        }
        return this.signLayer_;
      }
    }

    public Wp8Map()
    {
      Log.d(nameof (Wp8Map), "Map being created");
      Map map = new Map();
      map.HorizontalAlignment = HorizontalAlignment.Stretch;
      map.VerticalAlignment = VerticalAlignment.Stretch;
      map.Margin = new Thickness(0.0);
      map.ZoomLevel = this.GetDefaultZoom();
      map.FlowDirection = FlowDirection.LeftToRight;
      map.Center = new GeoCoordinate(0.0, 0.0);
      map.Heading = 0.0;
      this.Map = map;
      Log.d(nameof (Wp8Map), "Map created");
    }

    public override UIElement Element => (UIElement) this.Map;

    public override MapPoint CreateCenterMapPoint() => (MapPoint) new Wp8Map.CenterMapPoint(this);

    public override GeoCoordinate Center
    {
      get => this.Map.Center;
      set => this.Map.SetView(value, this.Map.ZoomLevel, MapAnimationKind.Linear);
    }

    public override double ZoomLevel
    {
      get => this.Map.ZoomLevel;
      set => this.Map.ZoomLevel = value;
    }

    public override MapMode CartographicMode
    {
      get
      {
        switch (this.Map.CartographicMode)
        {
          case MapCartographicMode.Road:
            return MapMode.Road;
          case MapCartographicMode.Aerial:
            return MapMode.Aerial;
          case MapCartographicMode.Hybrid:
            return MapMode.Hybrid;
          case MapCartographicMode.Terrain:
            return MapMode.Terrain;
          default:
            throw new NotImplementedException();
        }
      }
      set
      {
        switch (value)
        {
          case MapMode.Road:
            this.Map.CartographicMode = MapCartographicMode.Road;
            break;
          case MapMode.Aerial:
            this.Map.CartographicMode = MapCartographicMode.Aerial;
            break;
          case MapMode.Hybrid:
            this.Map.CartographicMode = MapCartographicMode.Hybrid;
            break;
          case MapMode.Terrain:
            this.Map.CartographicMode = MapCartographicMode.Terrain;
            break;
          default:
            throw new NotImplementedException();
        }
      }
    }

    public override double ShownAreaRadius()
    {
      GeoCoordinate geoCoordinate1 = this.Map.ConvertViewportPointToGeoCoordinate(new System.Windows.Point(0.0, 0.0));
      GeoCoordinate geoCoordinate2 = this.Map.ConvertViewportPointToGeoCoordinate(new System.Windows.Point(this.Map.ActualWidth / 2.0, 0.0));
      return geoCoordinate1 != (GeoCoordinate) null && geoCoordinate2 != (GeoCoordinate) null ? geoCoordinate1.GetDistanceTo(geoCoordinate2) : 200.0;
    }

    public override double GetDefaultZoom()
    {
      double defaultZoom = 17.0;
      ResolutionHelper.ResolutionPlateau plateau = ResolutionHelper.Plateau;
      switch (plateau)
      {
        case ResolutionHelper.ResolutionPlateau.S10:
          defaultZoom = 16.0;
          break;
        case ResolutionHelper.ResolutionPlateau.S20:
          defaultZoom = 16.0;
          break;
        case ResolutionHelper.ResolutionPlateau.S30:
          defaultZoom = 18.0;
          break;
      }
      Log.d("Map", "Default zoom {0}, plateau:{1}, 10:{2}, res:{3}, zoom:{4}", (object) defaultZoom, (object) plateau, (object) AppState.IsWP10OrLater, (object) ResolutionHelper.GetResolution(), (object) ResolutionHelper.GetScaleFactor());
      return defaultZoom;
    }

    public override void SetView(
      GeoCoordinate center,
      double widthInDegrees,
      double heightInDegrees)
    {
      if (center == (GeoCoordinate) null)
        return;
      if (heightInDegrees > 0.0 && new GeoCoordinate(center.Latitude + heightInDegrees, center.Longitude).GetDistanceTo(center) < 500.0)
      {
        widthInDegrees = -1.0;
        heightInDegrees = -1.0;
      }
      if (widthInDegrees > 0.0 && heightInDegrees > 0.0)
      {
        this.Map.SetView(new LocationRectangle(center, widthInDegrees, heightInDegrees), MapAnimationKind.Linear);
      }
      else
      {
        double zoomLevel = Math.Max(this.GetDefaultZoom(), this.Map.ZoomLevel);
        if (widthInDegrees < 0.0 || heightInDegrees < 0.0 || zoomLevel > this.GetDefaultZoom())
          this.Map.SetView(center, zoomLevel, MapAnimationKind.Linear);
        else
          this.Map.SetView(center, zoomLevel, MapAnimationKind.Parabolic);
      }
    }

    public override IObservable<EventArgs> ViewChanged()
    {
      return Observable.Create<EventArgs>((Func<IObserver<EventArgs>, Action>) (observer =>
      {
        this.Map.CenterChanged += (EventHandler<MapCenterChangedEventArgs>) ((s, e) => observer.OnNext((EventArgs) e));
        this.Map.ZoomLevelChanged += (EventHandler<MapZoomLevelChangedEventArgs>) ((s, e) => observer.OnNext((EventArgs) e));
        return (Action) (() => { });
      }));
    }

    public override MapPoint CreateUIElementMapPoint(
      Func<UIElement> content,
      MapPointStyle style,
      System.Windows.Point? origin = null)
    {
      return (MapPoint) new Wp8Map.UIElementMapPoint(this, content, style, origin);
    }

    public override MapPoint CreatePushPinMapPoint(Func<object> content)
    {
      return (MapPoint) new Wp8Map.UIElementMapPoint(this, (Func<UIElement>) (() =>
      {
        return (UIElement) new Pushpin()
        {
          Content = content()
        };
      }), MapPointStyle.Default, new System.Windows.Point?(new System.Windows.Point(0.0, 1.0)));
    }

    public override IDisposable ApplyMemoryWorkaround(PhoneApplicationPage page)
    {
      return (IDisposable) null;
    }

    public class CenterMapPoint : MapPoint
    {
      private Wp8Map Map;

      public CenterMapPoint(Wp8Map map) => this.Map = map;

      public GeoCoordinate Position() => this.Map.Map.Center;

      public UIElement Element() => (UIElement) null;

      public void SetCoordinate(GeoCoordinate loc) => this.Map.Map.Center = loc;

      public void Remove() => this.Map.Map.Center = (GeoCoordinate) null;
    }

    public class UIElementMapPoint : MapPoint
    {
      private Wp8Map Map;
      private Func<UIElement> GenerateElement;
      private UIElement element;
      private MapOverlay overlay;
      private GeoCoordinate currentPos;
      private System.Windows.Point? origin;
      private MapLayer layer;

      public UIElementMapPoint(
        Wp8Map map,
        Func<UIElement> elem,
        MapPointStyle style,
        System.Windows.Point? origin = null)
      {
        this.Map = map;
        this.GenerateElement = elem;
        this.origin = origin;
        switch (style)
        {
          case MapPointStyle.MyLocation:
          case MapPointStyle.Live:
            this.layer = this.Map.LocationLayer;
            break;
          case MapPointStyle.Sign:
            this.layer = this.Map.SignLayer;
            break;
          default:
            this.layer = this.Map.PinLayer;
            break;
        }
      }

      public GeoCoordinate Position() => this.currentPos;

      public UIElement Element() => this.element;

      public void SetCoordinate(GeoCoordinate loc)
      {
        if (loc == this.currentPos)
          return;
        if (this.overlay == null)
        {
          this.overlay = new MapOverlay();
          this.element = this.GenerateElement();
          this.overlay.Content = (object) this.element;
          if (this.origin.HasValue)
            this.overlay.PositionOrigin = this.origin.Value;
          this.layer.Add(this.overlay);
        }
        this.overlay.GeoCoordinate = loc;
        this.currentPos = loc;
      }

      public void Remove()
      {
        if (this.overlay == null)
          return;
        this.layer.Remove(this.overlay);
        this.overlay = (MapOverlay) null;
        this.currentPos = (GeoCoordinate) null;
      }
    }
  }
}
