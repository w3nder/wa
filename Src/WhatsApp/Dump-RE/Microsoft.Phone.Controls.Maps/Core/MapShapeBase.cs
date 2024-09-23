// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapShapeBase
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Device.Location;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [System.Windows.Markup.ContentProperty("Content")]
  [TemplatePart(Name = "ContentGrid", Type = typeof (Grid))]
  [TemplatePart(Name = "EmbeddedShape", Type = typeof (Shape))]
  public abstract class MapShapeBase : ContentControl, IProjectable
  {
    internal const string ContentGridElementName = "ContentGrid";
    internal const string EmbeddedShapeElementName = "EmbeddedShape";
    private Grid contentGrid;
    private ProjectionUpdateLevel pendingUpdate = ProjectionUpdateLevel.Full;
    private Shape shape;
    private Point topLeftViewportPoint;
    private static readonly DependencyProperty LocationsProperty = DependencyProperty.Register(nameof (Locations), typeof (LocationCollection), typeof (MapShapeBase), new PropertyMetadata(new PropertyChangedCallback(MapShapeBase.Locations_Changed)));

    protected MapShapeBase()
    {
      this.UseLayoutRounding = false;
      this.contentGrid = new Grid();
    }

    public Brush Fill
    {
      get => this.EmbeddedShape.Fill;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.Fill = value;
      }
    }

    public Brush Stroke
    {
      get => this.EmbeddedShape.Stroke;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.Stroke = value;
      }
    }

    public double StrokeThickness
    {
      get => this.EmbeddedShape.StrokeThickness;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeThickness = value;
      }
    }

    public DoubleCollection StrokeDashArray
    {
      get => this.EmbeddedShape.StrokeDashArray;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeDashArray = value;
      }
    }

    public PenLineCap StrokeDashCap
    {
      get => this.EmbeddedShape.StrokeDashCap;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeDashCap = value;
      }
    }

    public double StrokeDashOffset
    {
      get => this.EmbeddedShape.StrokeDashOffset;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeDashOffset = value;
      }
    }

    public PenLineCap StrokeEndLineCap
    {
      get => this.EmbeddedShape.StrokeEndLineCap;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeEndLineCap = value;
      }
    }

    public PenLineJoin StrokeLineJoin
    {
      get => this.EmbeddedShape.StrokeLineJoin;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeLineJoin = value;
      }
    }

    public double StrokeMiterLimit
    {
      get => this.EmbeddedShape.StrokeMiterLimit;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeMiterLimit = value;
      }
    }

    public PenLineCap StrokeStartLineCap
    {
      get => this.EmbeddedShape.StrokeStartLineCap;
      set
      {
        if (this.EmbeddedShape == null)
          return;
        this.EmbeddedShape.StrokeStartLineCap = value;
      }
    }

    public LocationCollection Locations
    {
      get => (LocationCollection) this.GetValue(MapShapeBase.LocationsProperty);
      set => this.SetValue(MapShapeBase.LocationsProperty, (object) value);
    }

    private static void Locations_Changed(DependencyObject o, DependencyPropertyChangedEventArgs ea)
    {
      if (!(o is MapShapeBase mapShapeBase))
        return;
      if (ea.OldValue is LocationCollection oldValue)
        oldValue.CollectionChanged -= new NotifyCollectionChangedEventHandler(mapShapeBase.Locations_CollectionChanged);
      if (ea.NewValue is LocationCollection newValue)
        newValue.CollectionChanged += new NotifyCollectionChangedEventHandler(mapShapeBase.Locations_CollectionChanged);
      mapShapeBase.ProjectionUpdated(ProjectionUpdateLevel.Full);
    }

    private void Locations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.ProjectionUpdated(ProjectionUpdateLevel.Full);
    }

    protected abstract PointCollection ProjectedPoints { get; set; }

    protected Shape EmbeddedShape
    {
      get => this.shape;
      set
      {
        if (value == null)
          return;
        this.SetEmbeddedShape(value);
      }
    }

    protected virtual void SetEmbeddedShape(Shape newShape)
    {
      if (this.shape != null)
      {
        newShape.Fill = this.shape.Fill;
        newShape.Stroke = this.shape.Stroke;
        newShape.StrokeThickness = this.shape.StrokeThickness;
        foreach (double strokeDash in (PresentationFrameworkCollection<double>) this.shape.StrokeDashArray)
          newShape.StrokeDashArray.Add(strokeDash);
        newShape.StrokeDashCap = this.shape.StrokeDashCap;
        newShape.StrokeDashOffset = this.shape.StrokeDashOffset;
        newShape.StrokeEndLineCap = this.shape.StrokeEndLineCap;
        newShape.StrokeLineJoin = this.shape.StrokeLineJoin;
        newShape.StrokeMiterLimit = this.shape.StrokeMiterLimit;
        newShape.StrokeStartLineCap = this.shape.StrokeStartLineCap;
      }
      else
        this.contentGrid.Children.Add((UIElement) newShape);
      this.shape = newShape;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.contentGrid = this.GetTemplateChild("ContentGrid") as Grid;
      this.EmbeddedShape = this.GetTemplateChild("EmbeddedShape") as Shape;
      if (this.contentGrid == null)
        return;
      this.contentGrid.UseLayoutRounding = this.UseLayoutRounding;
    }

    public MapBase ParentMap
    {
      get
      {
        if (!(this.Parent is IProjectable parent1) && VisualTreeHelper.GetParent((DependencyObject) this) is ContentPresenter parent2)
          parent1 = VisualTreeHelper.GetParent((DependencyObject) parent2) as IProjectable;
        return parent1?.ParentMap;
      }
    }

    public void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
      if (updateLevel == ProjectionUpdateLevel.None)
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
      this.pendingUpdate |= updateLevel;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      MapBase parentMap = this.ParentMap;
      if (this.pendingUpdate != ProjectionUpdateLevel.None && parentMap != null)
      {
        if (this.Locations != null)
        {
          if (this.pendingUpdate == ProjectionUpdateLevel.Full && this.Locations.Count > 0)
          {
            PointCollection pointCollection = new PointCollection();
            Point point1 = new Point(double.MaxValue, double.MaxValue);
            foreach (Point point2 in parentMap.Mode.LocationToViewportPoint((IEnumerable<GeoCoordinate>) this.Locations))
            {
              point1.X = Math.Min(point1.X, point2.X);
              point1.Y = Math.Min(point1.Y, point2.Y);
              pointCollection.Add(point2);
            }
            for (int index = 0; index < pointCollection.Count; ++index)
              pointCollection[index] = new Point(pointCollection[index].X - point1.X, pointCollection[index].Y - point1.Y);
            this.ProjectedPoints = pointCollection;
          }
          Point viewportPoint;
          if (this.ProjectedPoints.Count > 0 && this.Locations.Count > 0 && parentMap.TryLocationToViewportPoint(this.Locations[0], out viewportPoint))
            this.topLeftViewportPoint = new Point(viewportPoint.X - this.ProjectedPoints[0].X, viewportPoint.Y - this.ProjectedPoints[0].Y);
        }
        else
          this.ProjectedPoints.Clear();
        this.pendingUpdate = ProjectionUpdateLevel.None;
      }
      this.contentGrid.Measure(new Size(double.MaxValue, double.MaxValue));
      return parentMap == null ? new Size(0.0, 0.0) : parentMap.ViewportSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.contentGrid.Arrange(new Rect(this.topLeftViewportPoint.X, this.topLeftViewportPoint.Y, this.contentGrid.DesiredSize.Width + 1.0, this.contentGrid.DesiredSize.Height + 1.0));
      MapBase parentMap = this.ParentMap;
      return parentMap == null ? new Size(0.0, 0.0) : parentMap.ViewportSize;
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, "MapShape");
    }
  }
}
