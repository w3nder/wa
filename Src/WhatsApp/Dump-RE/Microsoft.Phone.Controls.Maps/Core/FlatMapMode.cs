// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.FlatMapMode
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public abstract class FlatMapMode : MapMode
  {
    private const double AnimationSnap = 0.0005;
    private const double AnimationExtensionRate = 0.1;
    private const double ZoomLevelSlowdownRangeForPan = 2.0;
    private const double KeyCountWeightCoefficientForAnimationDuration = 10.0;
    private const double SignificantZoomLevelChange = 0.5;
    private static readonly TimeSpan InitialAnimationProgress = new TimeSpan(0, 0, 0, 0, (int) Math.Round(50.0 / 3.0));
    private static readonly TimeSpan BaseAnimaitonDuration = new TimeSpan(0, 0, 0, 0, 1600);
    private static readonly double AnimationSpringWindup = Math.Log(2000.0, Math.E);
    private static double ZoomDeltaThresholdToSkipAnimation = 0.5;
    private readonly Storyboard animationStoryboard;
    private readonly List<FlatMapMode.FlatMapModeViewportDefinition> keyDefinitions = new List<FlatMapMode.FlatMapModeViewportDefinition>();
    private TimeSpan animationDuration;
    private DateTime animationStart;
    private FlatMapMode.FlatMapModeViewportDefinition currentViewportDefinition;
    private Size lastViewportSize;
    private Size logicalAreaSizeInScreenSpaceAtLevel1;
    private double nextKeyZoomLevel;
    private FlatMapMode.FlatMapModeViewportDefinition targetViewportDefinition;

    protected FlatMapMode(Size logicalAreaSizeInScreenSpaceAtLevel1)
    {
      this.logicalAreaSizeInScreenSpaceAtLevel1 = logicalAreaSizeInScreenSpaceAtLevel1;
      this.currentViewportDefinition.LogicalAreaSizeInScreenSpaceAtLevel1 = logicalAreaSizeInScreenSpaceAtLevel1;
      this.targetViewportDefinition.LogicalAreaSizeInScreenSpaceAtLevel1 = logicalAreaSizeInScreenSpaceAtLevel1;
      this.animationStoryboard = new Storyboard();
      this.animationStoryboard.Duration = new Duration(new TimeSpan(0L));
      this.animationStoryboard.Completed += new EventHandler(this.AnimationTick);
    }

    public Point ViewportPointToLogicalPoint(Point viewportPoint)
    {
      return this.currentViewportDefinition.ViewportPointToLogicalPoint(viewportPoint);
    }

    public Point LogicalPointToViewportPoint(Point logicalPoint)
    {
      return this.currentViewportDefinition.LogicalPointToViewportPoint(logicalPoint);
    }

    protected abstract Point LocationToLogicalPoint(GeoCoordinate location);

    public virtual IEnumerable<Point> LocationToLogicalPoint(IEnumerable<GeoCoordinate> locations)
    {
      return locations.Select<GeoCoordinate, Point>((Func<GeoCoordinate, Point>) (location => this.LocationToLogicalPoint(location)));
    }

    public virtual Rect LocationRectToLogicalRect(LocationRect boundingRectangle)
    {
      return new Rect(this.LocationToLogicalPoint(boundingRectangle.Northwest), this.LocationToLogicalPoint(boundingRectangle.Southeast));
    }

    protected abstract GeoCoordinate LogicalPointToLocation(Point logicalPoint);

    public virtual bool ConstrainView(
      GeoCoordinate center,
      ref double zoomLevel,
      ref double heading,
      ref double pitch)
    {
      return false;
    }

    private void ConstrainView(
      ref FlatMapMode.FlatMapModeViewportDefinition viewportDefinition)
    {
      GeoCoordinate center;
      double zoomLevel;
      this.DefinitionToView(viewportDefinition, out center, out zoomLevel);
      double heading = 0.0;
      double pitch = 0.0;
      this.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);
      viewportDefinition = this.ViewToDefinition(center, zoomLevel);
    }

    public override void OnMapDrag(MapDragEventArgs e)
    {
      Point viewportPoint = new Point(-e.DragDelta.X, -e.DragDelta.Y);
      Point point = new Point(e.ViewportPoint.X + e.DragDelta.X, e.ViewportPoint.Y + e.DragDelta.Y);
      this.currentViewportDefinition.TopLeftLogicalPoint = this.currentViewportDefinition.ViewportPointToLogicalPoint(viewportPoint);
      this.ConstrainView(ref this.currentViewportDefinition);
      double num = this.nextKeyZoomLevel - this.currentViewportDefinition.ZoomLevel;
      if (this.keyDefinitions.Count > 2 || Math.Abs(num) > 2.0)
      {
        this.keyDefinitions.Clear();
        this.keyDefinitions.Add(this.currentViewportDefinition);
        FlatMapMode.FlatMapModeViewportDefinition viewportDefinition = this.currentViewportDefinition with
        {
          ZoomLevel = Math.Round(this.currentViewportDefinition.ZoomLevel + (double) Math.Sign(num) * Math.Min(Math.Abs(num), 2.0))
        };
        this.ConstrainView(ref viewportDefinition);
        this.keyDefinitions.Add(viewportDefinition);
        this.animationDuration = FlatMapMode.BaseAnimaitonDuration;
        this.animationStart = DateTime.Now - FlatMapMode.InitialAnimationProgress;
        this.animationStoryboard.Begin();
      }
      for (int index = 0; index < this.keyDefinitions.Count; ++index)
      {
        FlatMapMode.FlatMapModeViewportDefinition keyDefinition = this.keyDefinitions[index];
        keyDefinition.SetTopLeftLogicalPoint(point, this.currentViewportDefinition.ViewportPointToLogicalPoint(point));
        this.keyDefinitions[index] = keyDefinition;
      }
      this.targetViewportDefinition = this.keyDefinitions[this.keyDefinitions.Count - 1];
      this.OnTargetViewChanged();
      this.OnProjectionChanged(ProjectionUpdateLevel.Linear);
      e.Handled = true;
    }

    public override void OnMapFlick(MapFlickEventArgs e)
    {
      Point point = e.Velocity;
      double angleFromVelocity = (double) PhysicsHelper.GetAngleFromVelocity(point);
      double val1 = Math.Sqrt(point.X * point.X + point.Y * point.Y);
      if (val1 > 1.0)
      {
        double num = Math.Min(Math.Min(val1, Math.Abs(this.ViewportSize.Width / 2.0)), Math.Abs(this.ViewportSize.Height / 2.0));
        point = new Point(num * Math.Cos(PhysicsHelper.DegreeToRadian(angleFromVelocity)), num * Math.Sin(PhysicsHelper.DegreeToRadian(angleFromVelocity)));
        FlatMapMode.FlatMapModeViewportDefinition viewportDefinition = this.currentViewportDefinition;
        viewportDefinition.SetTopLeftLogicalPoint(point, this.currentViewportDefinition.TopLeftLogicalPoint);
        if (this.targetViewportDefinition.ZoomLevel != this.currentViewportDefinition.ZoomLevel)
          viewportDefinition.ZoomLevel = Math.Round(viewportDefinition.ZoomLevel);
        this.ConstrainView(ref viewportDefinition);
        this.SetView(viewportDefinition, this.DoUserInputAnimation);
      }
      e.Handled = true;
    }

    public override void OnMapZoom(MapZoomEventArgs e)
    {
      bool animate = this.DoUserInputAnimation;
      FlatMapMode.FlatMapModeViewportDefinition viewportDefinition = this.currentViewportDefinition;
      viewportDefinition.ZoomLevel += e.ZoomDelta;
      this.ConstrainView(ref viewportDefinition);
      if (Math.Abs(e.ZoomDelta) < FlatMapMode.ZoomDeltaThresholdToSkipAnimation)
        animate = false;
      else
        viewportDefinition.ZoomLevel = Math.Round(viewportDefinition.ZoomLevel);
      viewportDefinition.SetTopLeftLogicalPoint(e.ViewportPoint, this.currentViewportDefinition.ViewportPointToLogicalPoint(e.ViewportPoint));
      this.ConstrainView(ref viewportDefinition);
      this.SetView(viewportDefinition, animate);
      e.Handled = true;
    }

    public override GeoCoordinate Center
    {
      get
      {
        return this.ViewportPointToLocation(new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0), this.currentViewportDefinition);
      }
      set
      {
        GeoCoordinate center = value;
        double targetZoomLevel = this.TargetZoomLevel;
        double targetHeading = this.TargetHeading;
        double targetPitch = this.TargetPitch;
        this.ConstrainView(center, ref targetZoomLevel, ref targetHeading, ref targetPitch);
        this.SetView(this.ViewToDefinition(center, targetZoomLevel), this.DoAnimation);
      }
    }

    public override GeoCoordinate TargetCenter
    {
      get
      {
        return this.ViewportPointToLocation(new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0), this.targetViewportDefinition);
      }
    }

    public override double ZoomLevel
    {
      get => this.currentViewportDefinition.ZoomLevel;
      set
      {
        GeoCoordinate targetCenter = this.TargetCenter;
        double zoomLevel = value;
        double targetHeading = this.TargetHeading;
        double targetPitch = this.TargetPitch;
        this.ConstrainView(targetCenter, ref zoomLevel, ref targetHeading, ref targetPitch);
        this.SetView(this.ViewToDefinition(targetCenter, zoomLevel), this.DoAnimation);
      }
    }

    public override double TargetZoomLevel => this.targetViewportDefinition.ZoomLevel;

    public override void SetView(
      GeoCoordinate center,
      double zoomLevel,
      double heading,
      double pitch,
      bool animate)
    {
      this.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);
      this.Heading = heading;
      this.Pitch = pitch;
      this.SetView(this.ViewToDefinition(center, zoomLevel), animate);
    }

    public override void SetView(LocationRect boundingRectangle, bool animate)
    {
      Rect logicalRect = this.LocationRectToLogicalRect(boundingRectangle);
      GeoCoordinate center;
      double zoomLevel;
      this.DefinitionToView(this.GetDefinitionByBounds((IList<Point>) new Point[2]
      {
        new Point(logicalRect.Left, logicalRect.Top),
        new Point(logicalRect.Right, logicalRect.Bottom)
      }), out center, out zoomLevel);
      this.SetView(center, zoomLevel, this.TargetHeading, this.TargetPitch, animate);
    }

    private void SetView(FlatMapMode.FlatMapModeViewportDefinition newView, bool animate)
    {
      this.lastViewportSize = this.ViewportSize;
      this.targetViewportDefinition = newView;
      this.keyDefinitions.Clear();
      this.keyDefinitions.Add(newView);
      if (!animate)
      {
        this.SnapToTarget();
      }
      else
      {
        this.keyDefinitions.Insert(0, this.currentViewportDefinition);
        Point viewportPoint = new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0);
        FlatMapMode.FlatMapModeViewportDefinition definitionByBounds = this.GetDefinitionByBounds((IList<Point>) new Point[2]
        {
          this.keyDefinitions[0].ViewportPointToLogicalPoint(viewportPoint),
          this.keyDefinitions[1].ViewportPointToLogicalPoint(viewportPoint)
        });
        if (definitionByBounds.ZoomLevel < this.keyDefinitions[0].ZoomLevel && definitionByBounds.ZoomLevel < this.keyDefinitions[this.keyDefinitions.Count - 1].ZoomLevel)
          this.keyDefinitions.Insert(1, definitionByBounds);
        double num = (double) (this.keyDefinitions.Count - 2) * 10.0;
        for (int index = 0; index < this.keyDefinitions.Count - 1; ++index)
          num += Math.Max(Math.Abs(this.keyDefinitions[index + 1].ZoomLevel - this.keyDefinitions[index].ZoomLevel), 1.0);
        this.animationDuration = new TimeSpan(FlatMapMode.BaseAnimaitonDuration.Ticks * (long) (0.1 * num + 0.9));
        this.animationStart = DateTime.Now - FlatMapMode.InitialAnimationProgress;
        this.animationStoryboard.Begin();
      }
      this.OnTargetViewChanged();
    }

    public override void ViewportSizeChanged(Size viewportSize)
    {
      base.ViewportSizeChanged(viewportSize);
      if (!(this.lastViewportSize != this.ViewportSize))
        return;
      Point viewportPoint = new Point((this.lastViewportSize.Width - this.ViewportSize.Width) / 2.0, (this.lastViewportSize.Height - this.ViewportSize.Height) / 2.0);
      this.currentViewportDefinition.TopLeftLogicalPoint = this.currentViewportDefinition.ViewportPointToLogicalPoint(viewportPoint);
      for (int index = 0; index < this.keyDefinitions.Count; ++index)
      {
        FlatMapMode.FlatMapModeViewportDefinition keyDefinition = this.keyDefinitions[index];
        keyDefinition.TopLeftLogicalPoint = keyDefinition.ViewportPointToLogicalPoint(viewportPoint);
        this.keyDefinitions[index] = keyDefinition;
      }
      this.targetViewportDefinition = this.keyDefinitions.Count <= 0 ? this.currentViewportDefinition : this.keyDefinitions[this.keyDefinitions.Count - 1];
      this.lastViewportSize = this.ViewportSize;
      this.OnProjectionChanged(ProjectionUpdateLevel.Linear);
    }

    public override GeoCoordinate ViewportPointToLocation(Point viewportPoint)
    {
      GeoCoordinate location;
      if (this.TryViewportPointToLocation(viewportPoint, out location))
        return location;
      throw new ArgumentException(ExceptionStrings.ViewportPointToLocation_DefaultException);
    }

    public override bool TryViewportPointToLocation(Point viewportPoint, out GeoCoordinate location)
    {
      location = this.ViewportPointToLocation(viewportPoint, this.currentViewportDefinition);
      return true;
    }

    public override Point LocationToViewportPoint(GeoCoordinate location)
    {
      Point viewportPoint;
      if (this.TryLocationToViewportPoint(location, out viewportPoint))
        return viewportPoint;
      throw new ArgumentException(ExceptionStrings.LocationToViewportPoint_DefaultException);
    }

    public override bool TryLocationToViewportPoint(GeoCoordinate location, out Point viewportPoint)
    {
      viewportPoint = this.LocationToViewportPoint(location, this.currentViewportDefinition);
      return true;
    }

    public override IEnumerable<Point> LocationToViewportPoint(IEnumerable<GeoCoordinate> locations)
    {
      return locations.Select<GeoCoordinate, Point>((Func<GeoCoordinate, Point>) (location => this.LocationToViewportPoint(location, this.currentViewportDefinition)));
    }

    public override Rect LocationToViewportPoint(LocationRect boundingRectangle)
    {
      return this.LocationToViewportPoint(boundingRectangle, this.currentViewportDefinition);
    }

    internal FlatMapMode.FlatMapModeViewportDefinition CurrentViewportDefinition
    {
      get => this.currentViewportDefinition;
    }

    internal FlatMapMode.FlatMapModeViewportDefinition TargetViewportDefinition
    {
      get => this.targetViewportDefinition;
    }

    internal GeoCoordinate ViewportPointToLocation(
      Point viewportPoint,
      FlatMapMode.FlatMapModeViewportDefinition definition)
    {
      return this.LogicalPointToLocation(definition.ViewportPointToLogicalPoint(viewportPoint));
    }

    internal Point LocationToViewportPoint(
      GeoCoordinate location,
      FlatMapMode.FlatMapModeViewportDefinition definition)
    {
      Point logicalPoint = this.LocationToLogicalPoint(location);
      return definition.LogicalPointToViewportPoint(logicalPoint);
    }

    internal Rect LocationToViewportPoint(
      LocationRect boundingRectangle,
      FlatMapMode.FlatMapModeViewportDefinition definition)
    {
      Rect logicalRect = this.LocationRectToLogicalRect(boundingRectangle);
      return new Rect(definition.LogicalPointToViewportPoint(new Point(logicalRect.Left, logicalRect.Top)), definition.LogicalPointToViewportPoint(new Point(logicalRect.Right, logicalRect.Bottom)));
    }

    private bool DoAnimation => this.AnimationLevel == AnimationLevel.Full;

    private bool DoUserInputAnimation => this.AnimationLevel != AnimationLevel.None;

    private void SnapToTarget()
    {
      this.animationStart = DateTime.MinValue;
      this.animationStoryboard.Stop();
      bool flag = this.currentViewportDefinition.ZoomLevel != this.targetViewportDefinition.ZoomLevel;
      this.currentViewportDefinition = this.targetViewportDefinition;
      this.nextKeyZoomLevel = this.currentViewportDefinition.ZoomLevel;
      this.OnProjectionChanged(flag ? ProjectionUpdateLevel.Full : ProjectionUpdateLevel.Linear);
    }

    private void AnimationTick(object sender, EventArgs e)
    {
      double num1 = (DateTime.Now - this.animationStart).TotalSeconds / this.animationDuration.TotalSeconds;
      if (num1 >= 1.0 || this.keyDefinitions.Count < 1)
      {
        this.SnapToTarget();
      }
      else
      {
        double num2 = (1.0 - Math.Exp(-num1 * FlatMapMode.AnimationSpringWindup)) / 0.9995;
        int index1 = 0;
        double num3 = 0.0;
        double num4 = 0.0;
        for (int index2 = 0; index2 < this.keyDefinitions.Count - 1; ++index2)
          num4 += Math.Max(Math.Abs(this.keyDefinitions[index2 + 1].ZoomLevel - this.keyDefinitions[index2].ZoomLevel), 1.0);
        double num5 = num2 * num4;
        double num6 = 0.0;
        for (int index3 = 0; index3 < this.keyDefinitions.Count - 1; ++index3)
        {
          double num7 = Math.Max(Math.Abs(this.keyDefinitions[index3 + 1].ZoomLevel - this.keyDefinitions[index3].ZoomLevel), 1.0);
          if (num6 + num7 > num5)
          {
            index1 = index3;
            num3 = (num5 - num6) / num7;
            break;
          }
          num6 += num7;
        }
        double y = this.keyDefinitions[index1].ZoomLevel - this.keyDefinitions[index1 + 1].ZoomLevel;
        if (Math.Abs(y) > 0.5)
        {
          double x = Math.Pow(2.0, y);
          num3 = (Math.Pow(x, num3) - 1.0) / (x - 1.0);
        }
        FlatMapMode.FlatMapModeViewportDefinition keyDefinition = this.keyDefinitions[this.keyDefinitions.Count - 1] with
        {
          ZoomLevel = Math.Log(1.0 / BezierHelper.GetSmoothedValue(num3, index1 - 1 >= 0 ? 1.0 / Math.Pow(2.0, this.keyDefinitions[index1 - 1].ZoomLevel) : double.NaN, 1.0 / Math.Pow(2.0, this.keyDefinitions[index1].ZoomLevel), 1.0 / Math.Pow(2.0, this.keyDefinitions[index1 + 1].ZoomLevel), index1 + 2 <= this.keyDefinitions.Count - 1 ? 1.0 / Math.Pow(2.0, this.keyDefinitions[index1 + 2].ZoomLevel) : double.NaN), 2.0),
          TopLeftLogicalPoint = new Point(BezierHelper.GetSmoothedValue(num3, index1 - 1 >= 0 ? this.keyDefinitions[index1 - 1].TopLeftLogicalPoint.X : double.NaN, this.keyDefinitions[index1].TopLeftLogicalPoint.X, this.keyDefinitions[index1 + 1].TopLeftLogicalPoint.X, index1 + 2 <= this.keyDefinitions.Count - 1 ? this.keyDefinitions[index1 + 2].TopLeftLogicalPoint.X : double.NaN), BezierHelper.GetSmoothedValue(num3, index1 - 1 >= 0 ? this.keyDefinitions[index1 - 1].TopLeftLogicalPoint.Y : double.NaN, this.keyDefinitions[index1].TopLeftLogicalPoint.Y, this.keyDefinitions[index1 + 1].TopLeftLogicalPoint.Y, index1 + 2 <= this.keyDefinitions.Count - 1 ? this.keyDefinitions[index1 + 2].TopLeftLogicalPoint.Y : double.NaN))
        };
        bool flag = this.currentViewportDefinition.ZoomLevel != keyDefinition.ZoomLevel;
        this.nextKeyZoomLevel = this.keyDefinitions[index1 + 1].ZoomLevel;
        this.currentViewportDefinition = keyDefinition;
        this.OnProjectionChanged(flag ? ProjectionUpdateLevel.Full : ProjectionUpdateLevel.Linear);
        this.animationStoryboard.Begin();
      }
    }

    private FlatMapMode.FlatMapModeViewportDefinition GetDefinitionByBounds(
      IList<Point> logicalPoints)
    {
      FlatMapMode.FlatMapModeViewportDefinition definitionByBounds = new FlatMapMode.FlatMapModeViewportDefinition();
      definitionByBounds.LogicalAreaSizeInScreenSpaceAtLevel1 = this.logicalAreaSizeInScreenSpaceAtLevel1;
      if (logicalPoints.Count <= 0)
        return definitionByBounds;
      Rect rect = new Rect(logicalPoints[0], new Size(0.0, 0.0));
      for (int index = 1; index < logicalPoints.Count; ++index)
        rect.Union(logicalPoints[index]);
      Size viewportSize = this.ViewportSize;
      definitionByBounds.ZoomLevel = Math.Min(Math.Log(2.0 * viewportSize.Width / (this.logicalAreaSizeInScreenSpaceAtLevel1.Width * rect.Width), 2.0), Math.Log(2.0 * viewportSize.Height / (this.logicalAreaSizeInScreenSpaceAtLevel1.Height * rect.Height), 2.0));
      definitionByBounds.ZoomLevel = Math.Floor(definitionByBounds.ZoomLevel);
      definitionByBounds.SetTopLeftLogicalPoint(new Point(viewportSize.Width / 2.0, viewportSize.Height / 2.0), new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0));
      return definitionByBounds;
    }

    private FlatMapMode.FlatMapModeViewportDefinition ViewToDefinition(
      GeoCoordinate center,
      double zoomLevel)
    {
      FlatMapMode.FlatMapModeViewportDefinition definition = new FlatMapMode.FlatMapModeViewportDefinition();
      definition.LogicalAreaSizeInScreenSpaceAtLevel1 = this.logicalAreaSizeInScreenSpaceAtLevel1;
      definition.ZoomLevel = zoomLevel;
      definition.SetTopLeftLogicalPoint(new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0), this.LocationToLogicalPoint(center));
      return definition;
    }

    private void DefinitionToView(
      FlatMapMode.FlatMapModeViewportDefinition viewportDefinition,
      out GeoCoordinate center,
      out double zoomLevel)
    {
      Point logicalPoint = viewportDefinition.ViewportPointToLogicalPoint(new Point(this.ViewportSize.Width / 2.0, this.ViewportSize.Height / 2.0));
      center = this.LogicalPointToLocation(logicalPoint);
      zoomLevel = viewportDefinition.ZoomLevel;
    }

    [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
    internal struct FlatMapModeViewportDefinition
    {
      private bool isCalculated;
      private Size logicalAreaSizeInScreenSpaceAtLevel1;
      private Size pixelSize;
      private Point topLeftLogicalPoint;
      private double zoomLevelMinusOne;

      public Size LogicalAreaSizeInScreenSpaceAtLevel1
      {
        get => this.logicalAreaSizeInScreenSpaceAtLevel1;
        set
        {
          this.logicalAreaSizeInScreenSpaceAtLevel1 = value;
          this.isCalculated = false;
        }
      }

      public double ZoomLevel
      {
        get => this.zoomLevelMinusOne + 1.0;
        set
        {
          this.zoomLevelMinusOne = value - 1.0;
          this.isCalculated = false;
        }
      }

      public Point TopLeftLogicalPoint
      {
        get => this.topLeftLogicalPoint;
        set => this.topLeftLogicalPoint = value;
      }

      public void SetTopLeftLogicalPoint(Point focusViewportPoint, Point focusLogicalPoint)
      {
        this.Recalculate();
        this.topLeftLogicalPoint = new Point(focusLogicalPoint.X - focusViewportPoint.X * this.pixelSize.Width, focusLogicalPoint.Y - focusViewportPoint.Y * this.pixelSize.Height);
      }

      public Point ViewportPointToLogicalPoint(Point viewportPoint)
      {
        this.Recalculate();
        return new Point(this.topLeftLogicalPoint.X + viewportPoint.X * this.pixelSize.Width, this.topLeftLogicalPoint.Y + viewportPoint.Y * this.pixelSize.Height);
      }

      public Point LogicalPointToViewportPoint(Point logicalPoint)
      {
        this.Recalculate();
        return new Point((logicalPoint.X - this.topLeftLogicalPoint.X) / this.pixelSize.Width, (logicalPoint.Y - this.topLeftLogicalPoint.Y) / this.pixelSize.Height);
      }

      private void Recalculate()
      {
        if (this.isCalculated)
          return;
        double num = Math.Pow(2.0, this.zoomLevelMinusOne);
        this.pixelSize.Width = 1.0 / (this.logicalAreaSizeInScreenSpaceAtLevel1.Width * num);
        this.pixelSize.Height = 1.0 / (this.logicalAreaSizeInScreenSpaceAtLevel1.Height * num);
        this.isCalculated = true;
      }
    }
  }
}
