// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapCore
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapCore : MapBase
  {
    private static Size optimalViewportSize = new Size(256.0, 256.0);
    private static readonly TimeSpan isIdlePollInterval = TimeSpan.FromSeconds(0.1);
    private readonly MapInputManager inputManager;
    private readonly MapLayer modeForegroundLayer;
    private readonly MapLayer modeLayer;
    private bool arranged;
    private MapMode currentMode;
    private bool isResized = true;
    private ProjectionUpdateLevel pendingUpdate;
    private AnimationLevel preLoadRealAnimationLevel;
    private bool updatingViewPropertyFromMode;
    private bool viewChanging;
    private Size viewportSize;
    private DispatcherTimer isIdlePollTimer;

    private event EventHandler<LoadingErrorEventArgs> loadingErrorEvent;

    protected MapCore()
    {
      this.RootLayer.Clip = (Geometry) new RectangleGeometry();
      this.viewportSize = MapCore.optimalViewportSize;
      this.currentMode = (MapMode) new NullMode();
      this.currentMode.AnimationLevel = AnimationLevel.None;
      this.preLoadRealAnimationLevel = AnimationLevel.Full;
      this.modeLayer = new MapLayer();
      this.modeForegroundLayer = new MapLayer();
      this.RootLayer.Children.Insert(0, (UIElement) this.modeLayer);
      this.RootLayer.Children.Add((UIElement) this.modeForegroundLayer);
      this.inputManager = new MapInputManager(this);
      this.SizeChanged += new SizeChangedEventHandler(this.MapCore_SizeChanged);
      this.isIdlePollTimer = new DispatcherTimer()
      {
        Interval = MapCore.isIdlePollInterval
      };
      this.Unloaded += (RoutedEventHandler) ((sender, e) =>
      {
        if (this.isIdlePollTimer == null)
          return;
        this.isIdlePollTimer.Tick -= new EventHandler(this.isIdlePollTimer_Tick);
      });
    }

    internal MapInputManager InputManager => this.inputManager;

    public override bool IsDownloading
    {
      get => this.currentMode != null && this.currentMode.IsDownloading;
    }

    public override bool IsIdle
    {
      get
      {
        return this.currentMode != null && this.currentMode.IsIdle && !this.viewChanging && this.arranged;
      }
    }

    public override Size ViewportSize => this.viewportSize;

    public override GeoCoordinate TargetCenter => this.currentMode.TargetCenter;

    public override double TargetZoomLevel => this.currentMode.TargetZoomLevel;

    public override double TargetHeading => this.currentMode.TargetHeading;

    public override double TargetPitch => this.currentMode.TargetPitch;

    public override LocationRect BoundingRectangle => this.currentMode.BoundingRectangle;

    public override LocationRect TargetBoundingRectangle
    {
      get => this.currentMode.TargetBoundingRectangle;
    }

    public override void SetMode(MapMode newMode, bool transferSettings)
    {
      if (transferSettings && newMode.Content != null && VisualTreeHelper.GetParent((DependencyObject) newMode.Content) != null)
        throw new ArgumentException(ExceptionStrings.InvalidMode);
      GeoCoordinate targetCenter = this.currentMode.TargetCenter;
      double targetZoomLevel = this.currentMode.TargetZoomLevel;
      double targetHeading = this.currentMode.TargetHeading;
      double targetPitch = this.currentMode.TargetPitch;
      AnimationLevel animationLevel = this.currentMode.AnimationLevel;
      newMode.Activating(this.currentMode, (MapLayerBase) this.modeLayer, (MapLayerBase) this.modeForegroundLayer);
      this.currentMode.TargetViewChanged -= new EventHandler<MapEventArgs>(this.currentMode_TargetViewChanged);
      this.currentMode.ProjectionChanged -= new EventHandler<ProjectionChangedEventArgs>(this.currentMode_ProjectionChanged);
      this.currentMode.Deactivating();
      this.currentMode = newMode;
      this.currentMode.ViewportSizeChanged(this.ViewportSize);
      if (transferSettings)
      {
        this.modeLayer.Children.Clear();
        this.modeForegroundLayer.Children.Clear();
        if (this.currentMode.Content != null)
          this.modeLayer.Children.Add(this.currentMode.Content);
        if (this.currentMode.ForegroundContent != null)
          this.modeForegroundLayer.Children.Add(this.currentMode.ForegroundContent);
        this.currentMode.CredentialsProvider = this.CredentialsProvider;
        this.currentMode.Culture = this.Culture;
        this.currentMode.AnimationLevel = animationLevel;
        this.currentMode.Activated((MapLayerBase) this.modeLayer, (MapLayerBase) this.modeForegroundLayer);
        this.currentMode.SetView(targetCenter, targetZoomLevel, targetHeading, targetPitch, false);
        this.ProjectionUpdated(ProjectionUpdateLevel.Full);
      }
      else
        this.currentMode.Activated((MapLayerBase) this.modeLayer, (MapLayerBase) this.modeForegroundLayer);
      this.currentMode.TargetViewChanged += new EventHandler<MapEventArgs>(this.currentMode_TargetViewChanged);
      this.currentMode.ProjectionChanged += new EventHandler<ProjectionChangedEventArgs>(this.currentMode_ProjectionChanged);
      this.UpdateViewFromMode();
      if (this.ModeChanged == null)
        return;
      this.ModeChanged((object) this, new MapEventArgs());
    }

    public override bool TryViewportPointToLocation(Point viewportPoint, out GeoCoordinate location)
    {
      return this.currentMode.TryViewportPointToLocation(viewportPoint, out location);
    }

    public override GeoCoordinate ViewportPointToLocation(Point viewportPoint)
    {
      return this.currentMode.ViewportPointToLocation(viewportPoint);
    }

    public override bool TryLocationToViewportPoint(GeoCoordinate location, out Point viewportPoint)
    {
      return this.currentMode.TryLocationToViewportPoint(location, out viewportPoint);
    }

    public override Point LocationToViewportPoint(GeoCoordinate location)
    {
      return this.currentMode.LocationToViewportPoint(location);
    }

    public override void SetView(GeoCoordinate center, double zoomLevel)
    {
      this.SetView(center, zoomLevel, this.currentMode.TargetHeading);
    }

    public override void SetView(GeoCoordinate center, double zoomLevel, double heading)
    {
      this.SetView(center, zoomLevel, heading, this.currentMode.TargetPitch);
    }

    public override void SetView(
      GeoCoordinate center,
      double zoomLevel,
      double heading,
      double pitch)
    {
      this.currentMode.SetView(center, zoomLevel, heading, pitch, this.arranged && this.AnimationLevel == AnimationLevel.Full && !DesignerProperties.GetIsInDesignMode((DependencyObject) this));
    }

    public override void SetView(LocationRect boundingRectangle)
    {
      this.currentMode.SetView(boundingRectangle, this.arranged && this.AnimationLevel == AnimationLevel.Full && !DesignerProperties.GetIsInDesignMode((DependencyObject) this));
    }

    protected override void OnCenterChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      if (this.updatingViewPropertyFromMode || eventArgs.NewValue == null)
        return;
      GeoCoordinate newValue = eventArgs.NewValue as GeoCoordinate;
      this.currentMode.Center = newValue;
      if (!(this.currentMode.Center != newValue))
        return;
      this.updatingViewPropertyFromMode = true;
      this.Center = this.currentMode.Center;
      this.updatingViewPropertyFromMode = false;
    }

    protected override void OnZoomLevelChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      if (this.updatingViewPropertyFromMode)
        return;
      double newValue = (double) eventArgs.NewValue;
      this.currentMode.ZoomLevel = newValue;
      if (this.currentMode.ZoomLevel == newValue)
        return;
      this.updatingViewPropertyFromMode = true;
      this.ZoomLevel = this.currentMode.ZoomLevel;
      this.updatingViewPropertyFromMode = false;
    }

    protected override void OnHeadingChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      if (this.updatingViewPropertyFromMode)
        return;
      double newValue = (double) eventArgs.NewValue;
      this.currentMode.Heading = newValue;
      if (this.currentMode.Heading == newValue)
        return;
      this.updatingViewPropertyFromMode = true;
      this.Heading = this.currentMode.Heading;
      this.updatingViewPropertyFromMode = false;
    }

    protected override void OnPitchChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      if (this.updatingViewPropertyFromMode)
        return;
      double newValue = (double) eventArgs.NewValue;
      this.currentMode.Pitch = newValue;
      if (this.currentMode.Pitch == newValue)
        return;
      this.updatingViewPropertyFromMode = true;
      this.Pitch = this.currentMode.Pitch;
      this.updatingViewPropertyFromMode = false;
    }

    public override event EventHandler<MapEventArgs> ViewChangeOnFrame;

    public override event EventHandler<MapEventArgs> TargetViewChanged;

    public override event EventHandler<MapEventArgs> ViewChangeStart;

    public override event EventHandler<MapEventArgs> ViewChangeEnd;

    public override event EventHandler<MapEventArgs> ModeChanged;

    public override event EventHandler<LoadingErrorEventArgs> LoadingError
    {
      add
      {
        if (this.LoadingException != null && value != null)
          value((object) this, new LoadingErrorEventArgs(this.LoadingException));
        this.loadingErrorEvent += value;
      }
      remove => this.loadingErrorEvent -= value;
    }

    public override event EventHandler<MapDragEventArgs> MapPan
    {
      add => this.inputManager.TouchDrag += value;
      remove => this.inputManager.TouchDrag -= value;
    }

    public override event EventHandler<MapZoomEventArgs> MapZoom
    {
      add => this.inputManager.TouchZoom += value;
      remove => this.inputManager.TouchZoom -= value;
    }

    public override event EventHandler MapResolved;

    internal Point CenterPoint
    {
      get => this.LocationToViewportPoint(this.Center);
      set => this.Center = this.ViewportPointToLocation(value);
    }

    protected internal Exception LoadingException { get; internal set; }

    private static long Timestamp => DateTime.UtcNow.Ticks;

    protected internal override void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
      this.pendingUpdate |= updateLevel;
      if (this.pendingUpdate == ProjectionUpdateLevel.None)
        return;
      this.InvalidateMeasure();
    }

    private void currentMode_ProjectionChanged(object sender, ProjectionChangedEventArgs e)
    {
      this.UpdateViewFromMode();
      this.pendingUpdate |= e.UpdateLevel;
      if (this.pendingUpdate == ProjectionUpdateLevel.None)
        return;
      this.InvalidateMeasure();
    }

    private void UpdateViewFromMode()
    {
      this.updatingViewPropertyFromMode = true;
      if (this.Center != this.currentMode.Center)
        this.Center = this.currentMode.Center;
      if (this.ZoomLevel != this.currentMode.ZoomLevel)
        this.ZoomLevel = this.currentMode.ZoomLevel;
      if (this.Heading != this.currentMode.Heading)
        this.Heading = this.currentMode.Heading;
      if (this.Pitch != this.currentMode.Pitch)
        this.Pitch = this.currentMode.Pitch;
      this.updatingViewPropertyFromMode = false;
    }

    private void Update()
    {
      if (this.pendingUpdate != ProjectionUpdateLevel.None)
      {
        if (!this.viewChanging)
        {
          this.viewChanging = true;
          if (this.ViewChangeStart != null)
            this.ViewChangeStart((object) this, new MapEventArgs());
        }
        this.RootLayer.ProjectionUpdated(this.pendingUpdate);
        if (this.ViewChangeOnFrame != null)
          this.ViewChangeOnFrame((object) this, new MapEventArgs());
        this.Dispatcher.BeginInvoke((Action) (() => this.InvalidateMeasure()));
      }
      else if (this.viewChanging && this.pendingUpdate == ProjectionUpdateLevel.None)
      {
        this.viewChanging = false;
        if (this.ViewChangeEnd != null)
          this.ViewChangeEnd((object) this, new MapEventArgs());
        this.isIdlePollTimer.Tick += new EventHandler(this.isIdlePollTimer_Tick);
        this.isIdlePollTimer.Start();
      }
      this.pendingUpdate = ProjectionUpdateLevel.None;
    }

    private void isIdlePollTimer_Tick(object sender, EventArgs e)
    {
      if (!this.IsIdle)
        return;
      this.isIdlePollTimer.Stop();
      this.isIdlePollTimer.Tick -= new EventHandler(this.isIdlePollTimer_Tick);
      EventHandler mapResolved = this.MapResolved;
      if (mapResolved == null)
        return;
      mapResolved((object) this, EventArgs.Empty);
    }

    private void MapCore_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ((RectangleGeometry) this.RootLayer.Clip).Rect = new Rect(0.0, 0.0, e.NewSize.Width, e.NewSize.Height);
      this.isResized = true;
    }

    private void UpdateViewportSize(Size pendingSize)
    {
      if (!(pendingSize != this.viewportSize))
        return;
      Size viewportSize = this.viewportSize;
      this.viewportSize = new Size(double.IsInfinity(pendingSize.Width) | double.IsNaN(pendingSize.Width) ? MapCore.optimalViewportSize.Width : pendingSize.Width, double.IsInfinity(pendingSize.Height) | double.IsNaN(pendingSize.Height) ? MapCore.optimalViewportSize.Height : pendingSize.Height);
      if (!(viewportSize != this.viewportSize))
        return;
      this.currentMode.ViewportSizeChanged(this.viewportSize);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (this.isResized)
      {
        this.UpdateViewportSize(availableSize);
        this.isResized = false;
      }
      this.Update();
      ((UIElement) this.Content).Measure(this.ViewportSize);
      return new Size(Math.Min(MapCore.optimalViewportSize.Width, availableSize.Width), Math.Min(MapCore.optimalViewportSize.Height, availableSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.UpdateViewportSize(finalSize);
      ((UIElement) this.Content).Arrange(new Rect(0.0, 0.0, this.ViewportSize.Width, this.ViewportSize.Height));
      if (!this.arranged)
      {
        this.arranged = true;
        this.OnFirstFrame();
      }
      return this.ViewportSize;
    }

    protected virtual void OnFirstFrame() => this.AnimationLevel = this.preLoadRealAnimationLevel;

    protected override void OnCultureChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      base.OnCultureChanged(eventArgs);
      if (!(eventArgs.NewValue is CultureInfo newValue))
        return;
      this.currentMode.Culture = newValue;
    }

    protected override void OnCredentialsProviderChanged(
      DependencyPropertyChangedEventArgs eventArgs)
    {
      if (eventArgs.OldValue != this.currentMode.CredentialsProvider)
        return;
      this.currentMode.CredentialsProvider = eventArgs.NewValue as CredentialsProvider;
    }

    protected override void OnAnimationLevelChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      AnimationLevel newValue = (AnimationLevel) eventArgs.NewValue;
      if (this.arranged && !DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        this.currentMode.AnimationLevel = newValue;
      else
        this.preLoadRealAnimationLevel = newValue;
    }

    protected override void OnModeChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
      this.SetMode(eventArgs.NewValue as MapMode, true);
    }

    internal void ThrowLoadingException(Exception e)
    {
      if (this.LoadingException != null || e == null)
        return;
      this.LoadingException = e;
      EventHandler<LoadingErrorEventArgs> loadingErrorEvent = this.loadingErrorEvent;
      if (loadingErrorEvent == null)
        return;
      loadingErrorEvent((object) this, new LoadingErrorEventArgs(this.LoadingException));
    }

    private void currentMode_TargetViewChanged(object sender, MapEventArgs e)
    {
      EventHandler<MapEventArgs> targetViewChanged = this.TargetViewChanged;
      if (targetViewChanged == null)
        return;
      targetViewChanged((object) this, e);
    }
  }
}
