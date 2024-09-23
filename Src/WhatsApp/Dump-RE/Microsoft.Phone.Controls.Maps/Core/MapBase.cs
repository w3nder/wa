// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapBase
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [System.Windows.Markup.ContentProperty("Children")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public abstract class MapBase : ContentControl
  {
    private MapLayer rootLayer;
    private MapLayer userContentLayer;
    public static readonly DependencyProperty LogoVisibilityProperty = DependencyProperty.Register(nameof (LogoVisibility), typeof (Visibility), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnOverlayVisibilityChangedCallback)));
    public static readonly DependencyProperty CopyrightVisibilityProperty = DependencyProperty.Register(nameof (CopyrightVisibility), typeof (Visibility), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnOverlayVisibilityChangedCallback)));
    public static readonly DependencyProperty ScaleVisibilityProperty = DependencyProperty.Register(nameof (ScaleVisibility), typeof (Visibility), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnOverlayVisibilityChangedCallback)));
    public static readonly DependencyProperty ZoomBarVisibilityProperty = DependencyProperty.Register(nameof (ZoomBarVisibility), typeof (Visibility), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnOverlayVisibilityChangedCallback)));
    public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof (Culture), typeof (CultureInfo), typeof (MapBase), new PropertyMetadata((object) CultureInfo.CurrentUICulture, new PropertyChangedCallback(MapBase.OnCultureChangedCallback)));
    public static readonly DependencyProperty CredentialsProviderProperty = DependencyProperty.Register(nameof (CredentialsProvider), typeof (CredentialsProvider), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnCredentialsProviderChangedCallback)));
    public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof (Mode), typeof (MapMode), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnModeChangedCallback)));
    public static readonly DependencyProperty AnimationLevelProperty = DependencyProperty.Register(nameof (AnimationLevel), typeof (AnimationLevel), typeof (MapBase), new PropertyMetadata((object) AnimationLevel.None, new PropertyChangedCallback(MapBase.OnAnimationLevelChangedCallback)));
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(nameof (Center), typeof (GeoCoordinate), typeof (MapBase), new PropertyMetadata((object) new GeoCoordinate(0.0, 0.0), new PropertyChangedCallback(MapBase.OnCenterChangedCallback)));
    public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(nameof (ZoomLevel), typeof (double), typeof (MapBase), new PropertyMetadata((object) 2.0, new PropertyChangedCallback(MapBase.OnZoomLevelChangedCallback)));
    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(nameof (Heading), typeof (double), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnHeadingChangedCallback)));
    public static readonly DependencyProperty PitchProperty = DependencyProperty.Register(nameof (Pitch), typeof (double), typeof (MapBase), new PropertyMetadata(new PropertyChangedCallback(MapBase.OnPitchChangedCallback)));

    public abstract event EventHandler<MapEventArgs> ViewChangeOnFrame;

    public abstract event EventHandler<MapEventArgs> TargetViewChanged;

    public abstract event EventHandler<MapEventArgs> ViewChangeStart;

    public abstract event EventHandler<MapEventArgs> ViewChangeEnd;

    public abstract event EventHandler<MapEventArgs> ModeChanged;

    public abstract event EventHandler<LoadingErrorEventArgs> LoadingError;

    public abstract event EventHandler<MapDragEventArgs> MapPan;

    public abstract event EventHandler<MapZoomEventArgs> MapZoom;

    public abstract event EventHandler MapResolved;

    protected MapBase()
    {
      this.DefaultStyleKey = (object) typeof (MapBase);
      this.rootLayer = new MapLayer();
      this.Content = (object) this.rootLayer;
      this.userContentLayer = new MapLayer();
      this.rootLayer.Children.Add((UIElement) this.userContentLayer);
      this.rootLayer.Background = (Brush) new SolidColorBrush(Colors.Transparent);
    }

    public UIElementCollection Children => this.userContentLayer.Children;

    public Visibility LogoVisibility
    {
      get => (Visibility) this.GetValue(MapBase.LogoVisibilityProperty);
      set => this.SetValue(MapBase.LogoVisibilityProperty, (object) value);
    }

    public Visibility CopyrightVisibility
    {
      get => (Visibility) this.GetValue(MapBase.CopyrightVisibilityProperty);
      set => this.SetValue(MapBase.CopyrightVisibilityProperty, (object) value);
    }

    public Visibility ScaleVisibility
    {
      get => (Visibility) this.GetValue(MapBase.ScaleVisibilityProperty);
      set => this.SetValue(MapBase.ScaleVisibilityProperty, (object) value);
    }

    public Visibility ZoomBarVisibility
    {
      get => (Visibility) this.GetValue(MapBase.ZoomBarVisibilityProperty);
      set => this.SetValue(MapBase.ZoomBarVisibilityProperty, (object) value);
    }

    public CultureInfo Culture
    {
      get => (CultureInfo) this.GetValue(MapBase.CultureProperty);
      set => this.SetValue(MapBase.CultureProperty, (object) value);
    }

    public CredentialsProvider CredentialsProvider
    {
      get => (CredentialsProvider) this.GetValue(MapBase.CredentialsProviderProperty);
      set => this.SetValue(MapBase.CredentialsProviderProperty, (object) value);
    }

    public virtual MapMode Mode
    {
      get => (MapMode) this.GetValue(MapBase.ModeProperty);
      set => this.SetValue(MapBase.ModeProperty, (object) value);
    }

    public AnimationLevel AnimationLevel
    {
      get => (AnimationLevel) this.GetValue(MapBase.AnimationLevelProperty);
      set => this.SetValue(MapBase.AnimationLevelProperty, (object) value);
    }

    [TypeConverter(typeof (LocationConverter))]
    public GeoCoordinate Center
    {
      get => (GeoCoordinate) this.GetValue(MapBase.CenterProperty);
      set => this.SetValue(MapBase.CenterProperty, (object) value);
    }

    public abstract GeoCoordinate TargetCenter { get; }

    public double ZoomLevel
    {
      get => (double) this.GetValue(MapBase.ZoomLevelProperty);
      set => this.SetValue(MapBase.ZoomLevelProperty, (object) value);
    }

    public abstract double TargetZoomLevel { get; }

    public double Heading
    {
      get => (double) this.GetValue(MapBase.HeadingProperty);
      set => this.SetValue(MapBase.HeadingProperty, (object) value);
    }

    public abstract double TargetHeading { get; }

    public double Pitch
    {
      get => (double) this.GetValue(MapBase.PitchProperty);
      set => this.SetValue(MapBase.PitchProperty, (object) value);
    }

    public abstract double TargetPitch { get; }

    public abstract bool IsDownloading { get; }

    public abstract bool IsIdle { get; }

    public abstract Size ViewportSize { get; }

    public abstract LocationRect BoundingRectangle { get; }

    public abstract LocationRect TargetBoundingRectangle { get; }

    private static void OnOverlayVisibilityChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((MapBase) d).OnOverlayVisibilityChanged(e);
    }

    protected virtual void OnOverlayVisibilityChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
    }

    private static void OnCultureChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnCultureChanged(eventArgs);
    }

    protected virtual void OnCultureChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
    }

    private static void OnCredentialsProviderChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnCredentialsProviderChanged(eventArgs);
    }

    protected virtual void OnCredentialsProviderChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
    }

    private static void OnModeChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnModeChanged(eventArgs);
    }

    protected virtual void OnModeChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
    }

    private static void OnAnimationLevelChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnAnimationLevelChanged(eventArgs);
    }

    protected virtual void OnAnimationLevelChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
    }

    protected MapLayer RootLayer => this.rootLayer;

    protected internal virtual void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
    }

    public abstract void SetMode(MapMode newMode, bool transferSettings);

    public abstract bool TryViewportPointToLocation(Point viewportPoint, out GeoCoordinate location);

    public abstract GeoCoordinate ViewportPointToLocation(Point viewportPoint);

    public abstract bool TryLocationToViewportPoint(GeoCoordinate location, out Point viewportPoint);

    public abstract Point LocationToViewportPoint(GeoCoordinate location);

    public abstract void SetView(GeoCoordinate center, double zoomLevel);

    public abstract void SetView(GeoCoordinate center, double zoomLevel, double heading);

    public abstract void SetView(
      GeoCoordinate center,
      double zoomLevel,
      double heading,
      double pitch);

    public abstract void SetView(LocationRect boundingRectangle);

    private static void OnCenterChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnCenterChanged(eventArgs);
    }

    protected abstract void OnCenterChanged(DependencyPropertyChangedEventArgs eventArgs);

    private static void OnZoomLevelChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnZoomLevelChanged(eventArgs);
    }

    protected abstract void OnZoomLevelChanged(DependencyPropertyChangedEventArgs eventArgs);

    private static void OnHeadingChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnHeadingChanged(eventArgs);
    }

    protected abstract void OnHeadingChanged(DependencyPropertyChangedEventArgs eventArgs);

    private static void OnPitchChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      ((MapBase) d).OnPitchChanged(eventArgs);
    }

    protected abstract void OnPitchChanged(DependencyPropertyChangedEventArgs eventArgs);

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new MapBaseAutomationPeer(this);
    }
  }
}
