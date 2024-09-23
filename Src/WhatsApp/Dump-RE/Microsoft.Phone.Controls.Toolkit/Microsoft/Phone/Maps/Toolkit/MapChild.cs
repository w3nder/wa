// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapChild
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  public static class MapChild
  {
    public static readonly DependencyProperty GeoCoordinateProperty = DependencyProperty.RegisterAttached("GeoCoordinate", typeof (object), typeof (MapChild), (PropertyMetadata) null);
    public static readonly DependencyProperty PositionOriginProperty = DependencyProperty.RegisterAttached("PositionOrigin", typeof (Point), typeof (MapChild), (PropertyMetadata) null);
    internal static readonly DependencyProperty ToolkitCreatedProperty = DependencyProperty.RegisterAttached("ToolkitCreated", typeof (bool), typeof (MapChild), (PropertyMetadata) null);

    [TypeConverter(typeof (GeoCoordinateConverter))]
    public static GeoCoordinate GetGeoCoordinate(DependencyObject element)
    {
      return element != null ? (GeoCoordinate) element.GetValue(MapChild.GeoCoordinateProperty) : throw new ArgumentNullException(nameof (element));
    }

    public static void SetGeoCoordinate(DependencyObject element, GeoCoordinate value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(MapChild.GeoCoordinateProperty, (object) value);
    }

    public static Point GetPositionOrigin(DependencyObject element)
    {
      return element != null ? (Point) element.GetValue(MapChild.PositionOriginProperty) : throw new ArgumentNullException(nameof (element));
    }

    public static void SetPositionOrigin(DependencyObject element, Point value)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      element.SetValue(MapChild.PositionOriginProperty, (object) value);
    }

    internal static MapOverlay CreateMapOverlay(object content, DataTemplate contentTemplate)
    {
      MapOverlay mapOverlay = new MapOverlay();
      MapOverlayItem mapOverlayItem = new MapOverlayItem(content, contentTemplate, mapOverlay);
      mapOverlay.SetValue(MapChild.ToolkitCreatedProperty, (object) true);
      mapOverlay.Content = (object) mapOverlayItem;
      return mapOverlay;
    }

    internal static void BindMapOverlayProperties(MapOverlay mapOverlay)
    {
      MapChild.BindMapOverlayProperties(mapOverlay, ((DependencyObject) mapOverlay.Content).GetVisualChildren().FirstOrDefault<DependencyObject>() ?? throw new InvalidOperationException("Could not bind the properties because there was no UI"));
    }

    internal static void BindMapOverlayProperties(
      MapOverlay mapOverlay,
      DependencyObject targetObject)
    {
      mapOverlay.GeoCoordinate = (GeoCoordinate) targetObject.GetValue(MapChild.GeoCoordinateProperty);
      MapChild.BindMapOverlayProperty(mapOverlay, "GeoCoordinate", targetObject, MapChild.GeoCoordinateProperty);
      mapOverlay.PositionOrigin = (Point) targetObject.GetValue(MapChild.PositionOriginProperty);
      MapChild.BindMapOverlayProperty(mapOverlay, "PositionOrigin", targetObject, MapChild.PositionOriginProperty);
    }

    internal static void BindMapOverlayProperty(
      MapOverlay mapOverlay,
      string mapOverlayDependencyProperty,
      DependencyObject targetObject,
      DependencyProperty targetDependencyProperty)
    {
      Binding binding = new Binding()
      {
        Source = (object) mapOverlay,
        Mode = BindingMode.TwoWay,
        Path = new PropertyPath(mapOverlayDependencyProperty, new object[0])
      };
      BindingOperations.SetBinding(targetObject, targetDependencyProperty, (BindingBase) binding);
    }

    internal static void ClearMapOverlayBindings(MapOverlay mapOverlay)
    {
      DependencyObject targetObject = ((DependencyObject) mapOverlay.Content).GetVisualChildren().FirstOrDefault<DependencyObject>();
      if (targetObject == null)
        return;
      MapChild.ClearMapOverlayBindings(mapOverlay, targetObject);
    }

    internal static void ClearMapOverlayBindings(
      MapOverlay mapOverlay,
      DependencyObject targetObject)
    {
      GeoCoordinate geoCoordinate = (GeoCoordinate) targetObject.GetValue(MapChild.GeoCoordinateProperty);
      Point point = (Point) targetObject.GetValue(MapChild.PositionOriginProperty);
      targetObject.ClearValue(MapChild.GeoCoordinateProperty);
      targetObject.ClearValue(MapChild.PositionOriginProperty);
      targetObject.SetValue(MapChild.GeoCoordinateProperty, (object) geoCoordinate);
      targetObject.SetValue(MapChild.PositionOriginProperty, (object) point);
    }
  }
}
