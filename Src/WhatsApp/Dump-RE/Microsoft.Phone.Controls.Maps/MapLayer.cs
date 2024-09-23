// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.MapLayer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public sealed class MapLayer : MapLayerBase, IProjectable
  {
    private Guid lastProjectPassTag;
    private ProjectionUpdateLevel pendingUpdate = ProjectionUpdateLevel.Full;
    public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof (GeoCoordinate), typeof (MapLayer), new PropertyMetadata(new PropertyChangedCallback(MapLayer.OnPositionChanged)));
    public static readonly DependencyProperty PositionRectangleProperty = DependencyProperty.RegisterAttached("PositionRectangle", typeof (LocationRect), typeof (MapLayer), new PropertyMetadata(new PropertyChangedCallback(MapLayer.OnPositionRectangleChanged)));
    public static readonly DependencyProperty PositionOriginProperty = DependencyProperty.RegisterAttached("PositionOrigin", typeof (PositionOrigin), typeof (MapLayer), new PropertyMetadata(new PropertyChangedCallback(MapLayer.OnPositionOriginChanged)));
    public static readonly DependencyProperty PositionOffsetProperty = DependencyProperty.RegisterAttached("PositionOffset", typeof (Point), typeof (MapLayer), new PropertyMetadata(new PropertyChangedCallback(MapLayer.OnPositionOffsetChanged)));
    private static readonly DependencyProperty ProjectionUpdatedTag = DependencyProperty.RegisterAttached("ProjectionUpdatedTagProperty", typeof (Guid), typeof (MapLayer), (PropertyMetadata) null);

    public override void AddChild(UIElement element, GeoCoordinate location)
    {
      this.Children.Add(element);
      element.SetValue(MapLayer.PositionProperty, (object) location);
    }

    public override void AddChild(UIElement element, GeoCoordinate location, Point offset)
    {
      this.Children.Add(element);
      element.SetValue(MapLayer.PositionProperty, (object) location);
      element.SetValue(MapLayer.PositionOffsetProperty, (object) offset);
    }

    public override void AddChild(UIElement element, GeoCoordinate location, PositionOrigin origin)
    {
      this.Children.Add(element);
      element.SetValue(MapLayer.PositionProperty, (object) location);
      element.SetValue(MapLayer.PositionOriginProperty, (object) origin);
    }

    public override void AddChild(UIElement element, LocationRect locationRect)
    {
      this.Children.Add(element);
      element.SetValue(MapLayer.PositionRectangleProperty, (object) locationRect);
    }

    public static GeoCoordinate GetPosition(DependencyObject dependencyObject)
    {
      GeoCoordinate position = (GeoCoordinate) dependencyObject.GetValue(MapLayer.PositionProperty);
      if (position == (GeoCoordinate) null && dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, 0);
        if (child != null)
          position = MapLayer.GetPosition(child);
      }
      return position;
    }

    public static void SetPosition(DependencyObject dependencyObject, GeoCoordinate position)
    {
      dependencyObject.SetValue(MapLayer.PositionProperty, (object) position);
    }

    private static void OnPositionChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs ea)
    {
      MapLayer.InvalidateParentLayout(dependencyObject);
    }

    public static LocationRect GetPositionRectangle(DependencyObject dependencyObject)
    {
      LocationRect positionRectangle = (LocationRect) dependencyObject.GetValue(MapLayer.PositionRectangleProperty);
      if (positionRectangle == null && dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, 0);
        if (child != null)
          positionRectangle = MapLayer.GetPositionRectangle(child);
      }
      return positionRectangle;
    }

    public static void SetPositionRectangle(DependencyObject dependencyObject, LocationRect rect)
    {
      dependencyObject.SetValue(MapLayer.PositionRectangleProperty, (object) rect);
    }

    private static void OnPositionRectangleChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs ea)
    {
      MapLayer.InvalidateParentLayout(dependencyObject);
    }

    public static PositionOrigin GetPositionOrigin(DependencyObject dependencyObject)
    {
      PositionOrigin positionOrigin = (PositionOrigin) dependencyObject.GetValue(MapLayer.PositionOriginProperty);
      if (dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, 0);
        if (child != null)
          positionOrigin = MapLayer.GetPositionOrigin(child);
      }
      return positionOrigin;
    }

    public static void SetPositionOrigin(DependencyObject dependencyObject, PositionOrigin origin)
    {
      dependencyObject.SetValue(MapLayer.PositionOriginProperty, (object) origin);
    }

    private static void OnPositionOriginChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs ea)
    {
      MapLayer.InvalidateParentLayout(dependencyObject);
    }

    public static Point GetPositionOffset(DependencyObject dependencyObject)
    {
      Point positionOffset = (Point) dependencyObject.GetValue(MapLayer.PositionOffsetProperty);
      if (dependencyObject is ContentPresenter && VisualTreeHelper.GetChildrenCount(dependencyObject) > 0)
      {
        DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, 0);
        if (child != null)
          positionOffset = MapLayer.GetPositionOffset(child);
      }
      return positionOffset;
    }

    public static void SetPositionOffset(DependencyObject dependencyObject, Point point)
    {
      dependencyObject.SetValue(MapLayer.PositionOffsetProperty, (object) point);
    }

    private static void OnPositionOffsetChanged(
      DependencyObject dependencyObject,
      DependencyPropertyChangedEventArgs ea)
    {
      MapLayer.InvalidateParentLayout(dependencyObject);
    }

    public MapBase ParentMap
    {
      get
      {
        if (this.Parent is IProjectable parent1)
          return parent1.ParentMap;
        if (this.Parent is MapBase parent2)
          return parent2;
        return VisualTreeHelper.GetParent((DependencyObject) this) is ItemsPresenter parent3 && VisualTreeHelper.GetParent((DependencyObject) parent3) is IProjectable parent4 ? parent4.ParentMap : (MapBase) null;
      }
    }

    public void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
      if (updateLevel == ProjectionUpdateLevel.None)
        return;
      this.InvalidateArrange();
      this.InvalidateMeasure();
      this.pendingUpdate |= updateLevel;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      MapBase parentMap = this.ParentMap;
      if (parentMap == null)
        return new Size(0.0, 0.0);
      Guid lastProjectPassTag = this.lastProjectPassTag;
      this.lastProjectPassTag = Guid.NewGuid();
      foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.Children)
      {
        if (child is IProjectable projectable)
        {
          ProjectionUpdateLevel updateLevel = this.pendingUpdate;
          if ((Guid) child.GetValue(MapLayer.ProjectionUpdatedTag) != lastProjectPassTag)
            updateLevel = ProjectionUpdateLevel.Full;
          if (updateLevel != ProjectionUpdateLevel.None)
            projectable.ProjectionUpdated(updateLevel);
          child.SetValue(MapLayer.ProjectionUpdatedTag, (object) this.lastProjectPassTag);
        }
      }
      this.pendingUpdate = ProjectionUpdateLevel.None;
      foreach (UIElement child1 in (PresentationFrameworkCollection<UIElement>) this.Children)
      {
        LocationRect positionRectangle = MapLayer.GetPositionRectangle((DependencyObject) child1);
        if (positionRectangle != null)
        {
          Rect viewportPoint = parentMap.Mode.LocationToViewportPoint(positionRectangle);
          child1.Measure(new Size(viewportPoint.Width, viewportPoint.Height));
        }
        else
        {
          if (child1 is ContentPresenter && VisualTreeHelper.GetChildrenCount((DependencyObject) child1) > 0 && VisualTreeHelper.GetChild((DependencyObject) child1, 0) is IProjectable child2)
          {
            child2.ProjectionUpdated(ProjectionUpdateLevel.Full);
            if (child2 is UIElement uiElement)
              uiElement.InvalidateMeasure();
          }
          child1.Measure(parentMap.ViewportSize);
        }
      }
      return parentMap.ViewportSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      MapBase parentMap = this.ParentMap;
      if (parentMap != null)
      {
        foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.Children)
        {
          Rect finalRect = new Rect(0.0, 0.0, parentMap.ViewportSize.Width, parentMap.ViewportSize.Height);
          LocationRect positionRectangle = MapLayer.GetPositionRectangle((DependencyObject) child);
          if (positionRectangle != null)
          {
            finalRect = parentMap.Mode.LocationToViewportPoint(positionRectangle);
          }
          else
          {
            GeoCoordinate position = MapLayer.GetPosition((DependencyObject) child);
            Point viewportPoint;
            if (position != (GeoCoordinate) null && parentMap.TryLocationToViewportPoint(position, out viewportPoint))
            {
              PositionOrigin positionOrigin = MapLayer.GetPositionOrigin((DependencyObject) child);
              viewportPoint.X -= positionOrigin.X * child.DesiredSize.Width;
              viewportPoint.Y -= positionOrigin.Y * child.DesiredSize.Height;
              if (Math.Abs(viewportPoint.X) > 100000.0)
                viewportPoint.X = (double) (Math.Sign(viewportPoint.X) * 100000);
              if (Math.Abs(viewportPoint.Y) > 100000.0)
                viewportPoint.Y = (double) (Math.Sign(viewportPoint.Y) * 100000);
              finalRect = new Rect(viewportPoint.X, viewportPoint.Y, child.DesiredSize.Width, child.DesiredSize.Height);
            }
          }
          Point positionOffset = MapLayer.GetPositionOffset((DependencyObject) child);
          finalRect.X += positionOffset.X;
          finalRect.Y += positionOffset.Y;
          child.Arrange(finalRect);
        }
      }
      return parentMap == null ? new Size(0.0, 0.0) : parentMap.ViewportSize;
    }

    private static void InvalidateParentLayout(DependencyObject dependencyObject)
    {
      if (!(dependencyObject is FrameworkElement frameworkElement))
        return;
      if (!(frameworkElement.Parent is MapLayer parent1) && frameworkElement.Parent is ContentPresenter parent2)
        parent1 = parent2.Parent as MapLayer;
      if (parent1 == null)
        return;
      parent1.InvalidateMeasure();
      parent1.InvalidateArrange();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (MapLayer));
    }
  }
}
