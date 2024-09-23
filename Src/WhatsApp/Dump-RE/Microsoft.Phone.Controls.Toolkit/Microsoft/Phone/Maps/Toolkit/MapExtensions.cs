// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapExtensions
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  public static class MapExtensions
  {
    public static readonly DependencyProperty ChildrenProperty = DependencyProperty.RegisterAttached("Children", typeof (ObservableCollection<DependencyObject>), typeof (MapExtensions), (PropertyMetadata) null);
    private static readonly DependencyProperty ChildrenChangedManagerProperty = DependencyProperty.RegisterAttached("ChildrenChangedManager", typeof (MapExtensionsChildrenChangeManager), typeof (MapExtensions), (PropertyMetadata) null);

    public static ObservableCollection<DependencyObject> GetChildren(Map element)
    {
      ObservableCollection<DependencyObject> sourceCollection = element != null ? (ObservableCollection<DependencyObject>) element.GetValue(MapExtensions.ChildrenProperty) : throw new ArgumentNullException(nameof (element));
      if (sourceCollection == null)
      {
        sourceCollection = new ObservableCollection<DependencyObject>();
        MapExtensionsChildrenChangeManager childrenChangeManager = new MapExtensionsChildrenChangeManager((INotifyCollectionChanged) sourceCollection)
        {
          Map = element
        };
        element.SetValue(MapExtensions.ChildrenProperty, (object) sourceCollection);
        element.SetValue(MapExtensions.ChildrenChangedManagerProperty, (object) childrenChangeManager);
      }
      return sourceCollection;
    }

    public static void Add(
      this ObservableCollection<DependencyObject> childrenCollection,
      DependencyObject dependencyObject,
      GeoCoordinate geoCoordinate)
    {
      if (childrenCollection == null)
        throw new ArgumentNullException(nameof (childrenCollection));
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof (dependencyObject));
      if (geoCoordinate == (GeoCoordinate) null)
        throw new ArgumentNullException(nameof (geoCoordinate));
      dependencyObject.SetValue(MapChild.GeoCoordinateProperty, (object) geoCoordinate);
      childrenCollection.Add(dependencyObject);
    }

    public static void Add(
      this ObservableCollection<DependencyObject> childrenCollection,
      DependencyObject dependencyObject,
      GeoCoordinate geoCoordinate,
      Point positionOrigin)
    {
      if (childrenCollection == null)
        throw new ArgumentNullException(nameof (childrenCollection));
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof (dependencyObject));
      if (geoCoordinate == (GeoCoordinate) null)
        throw new ArgumentNullException(nameof (geoCoordinate));
      dependencyObject.SetValue(MapChild.GeoCoordinateProperty, (object) geoCoordinate);
      dependencyObject.SetValue(MapChild.PositionOriginProperty, (object) positionOrigin);
      childrenCollection.Add(dependencyObject);
    }
  }
}
