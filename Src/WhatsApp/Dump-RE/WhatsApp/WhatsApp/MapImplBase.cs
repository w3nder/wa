// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapImplBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public abstract class MapImplBase : MapImplementation
  {
    private UIElement CreateMarker() => (UIElement) new MapMyLocation();

    private UIElement CreatePin() => (UIElement) new MapPlacePin();

    private UIElement CreateSign() => (UIElement) new MapSign();

    private UIElement CreateLivePin() => (UIElement) new LiveLocationPin();

    public abstract MapPoint CreateCenterMapPoint();

    public abstract MapPoint CreateUIElementMapPoint(
      Func<UIElement> content,
      MapPointStyle style,
      System.Windows.Point? origin = null);

    public abstract MapPoint CreatePushPinMapPoint(Func<object> content);

    public abstract GeoCoordinate Center { get; set; }

    public abstract double ZoomLevel { get; set; }

    public abstract MapMode CartographicMode { get; set; }

    public abstract void SetView(
      GeoCoordinate center,
      double widthInDegrees,
      double heightInDegrees);

    public abstract double ShownAreaRadius();

    public abstract IObservable<EventArgs> ViewChanged();

    public MapPoint AddPoint(MapPointStyle style = MapPointStyle.Default, string label = null)
    {
      List<MapPoint> mapPointList = new List<MapPoint>();
      switch (style)
      {
        case MapPointStyle.Default:
          mapPointList.Add(this.CreateCenterMapPoint());
          break;
        case MapPointStyle.MyLocation:
          mapPointList.Add(this.CreateUIElementMapPoint(new Func<UIElement>(this.CreateMarker), style, new System.Windows.Point?(new System.Windows.Point(0.5, 0.5))));
          break;
        case MapPointStyle.Place:
          mapPointList.Add(this.CreateUIElementMapPoint(new Func<UIElement>(this.CreatePin), style, new System.Windows.Point?(new System.Windows.Point(0.5, 1.0))));
          break;
        case MapPointStyle.Sign:
          mapPointList.Add(this.CreateUIElementMapPoint(new Func<UIElement>(this.CreateSign), style, new System.Windows.Point?(new System.Windows.Point(0.5, 1.0))));
          break;
        case MapPointStyle.Live:
          mapPointList.Add(this.CreateUIElementMapPoint(new Func<UIElement>(this.CreateLivePin), style, new System.Windows.Point?(new System.Windows.Point(0.5, 0.5))));
          break;
        default:
          throw new Exception("Unknown map style " + (object) style);
      }
      if (label != null)
        mapPointList.Add(this.CreatePushPinMapPoint((Func<object>) (() => (object) label)));
      return mapPointList.Count == 1 ? mapPointList.First<MapPoint>() : (MapPoint) new AggregateMapPoint(mapPointList);
    }

    public abstract double GetDefaultZoom();

    public abstract UIElement Element { get; }

    public abstract IDisposable ApplyMemoryWorkaround(PhoneApplicationPage page);
  }
}
