// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.MapTileLayer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [TemplatePart(Name = "ContentGrid", Type = typeof (Grid))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public sealed class MapTileLayer : Control, IProjectable
  {
    internal const string ContentGridElementName = "ContentGrid";
    private const double offsetPixels = 0.75;
    private readonly Collection<MultiScaleImage> msiCollection;
    private readonly MapMode parentMode;
    private readonly TileSourceCollection tileSources;
    private readonly WeakEventListener<MapTileLayer, object, NotifyCollectionChangedEventArgs> weakCollectionListener;
    private readonly WeakEventListener<MapTileLayer, object, TileSourcePropertyChangedEventArgs> weakItemListener;
    private Grid contentGrid;
    private int tileHeight = 256;
    private int tileWidth = 256;
    private bool updated;
    public static readonly DependencyProperty UpSampleLevelDeltaProperty = DependencyProperty.Register(nameof (UpSampleLevelDelta), typeof (int), typeof (MapTileLayer), new PropertyMetadata((object) int.MaxValue));

    public MapTileLayer()
    {
      this.DefaultStyleKey = (object) typeof (MapTileLayer);
      this.IsHitTestVisible = false;
      this.contentGrid = new Grid();
      this.tileSources = new TileSourceCollection();
      this.weakItemListener = new WeakEventListener<MapTileLayer, object, TileSourcePropertyChangedEventArgs>(this);
      this.weakItemListener.OnEventAction = (Action<MapTileLayer, object, TileSourcePropertyChangedEventArgs>) ((instance, source, eventArgs) => instance.TileSources_ItemPropertyChanged(source, eventArgs));
      this.tileSources.ItemPropertyChanged += new EventHandler<TileSourcePropertyChangedEventArgs>(this.weakItemListener.OnEvent);
      this.weakCollectionListener = new WeakEventListener<MapTileLayer, object, NotifyCollectionChangedEventArgs>(this);
      this.weakCollectionListener.OnEventAction = (Action<MapTileLayer, object, NotifyCollectionChangedEventArgs>) ((instance, source, eventArgs) => instance.TileSources_CollectionChanged(source, eventArgs));
      this.tileSources.CollectionChanged += new NotifyCollectionChangedEventHandler(this.weakCollectionListener.OnEvent);
      this.msiCollection = new Collection<MultiScaleImage>();
    }

    internal MapTileLayer(MapMode mode)
      : this()
    {
      this.parentMode = mode;
      this.parentMode.ProjectionChanged += new EventHandler<ProjectionChangedEventArgs>(this.ParentMode_ProjectionChanged);
    }

    public int TileWidth
    {
      get => this.tileWidth;
      set
      {
        this.tileWidth = value;
        this.RefreshSource();
      }
    }

    public int TileHeight
    {
      get => this.tileHeight;
      set
      {
        this.tileHeight = value;
        this.RefreshSource();
      }
    }

    public TileSourceCollection TileSources => this.tileSources;

    public int UpSampleLevelDelta
    {
      get => (int) this.GetValue(MapTileLayer.UpSampleLevelDeltaProperty);
      set => this.SetValue(MapTileLayer.UpSampleLevelDeltaProperty, (object) value);
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.contentGrid = this.GetTemplateChild("ContentGrid") as Grid;
    }

    internal bool IsDownloading
    {
      get
      {
        foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.contentGrid.Children)
        {
          if (child is MultiScaleImage multiScaleImage && multiScaleImage.Visibility == Visibility.Visible && multiScaleImage.IsDownloading)
            return true;
        }
        return false;
      }
    }

    internal bool IsIdle
    {
      get
      {
        if (!this.updated)
          return false;
        foreach (UIElement child in (PresentationFrameworkCollection<UIElement>) this.contentGrid.Children)
        {
          if (child is MultiScaleImage multiScaleImage && multiScaleImage.Visibility == Visibility.Visible && !multiScaleImage.IsIdle)
            return false;
        }
        return true;
      }
    }

    internal void RefreshSource()
    {
      foreach (MultiScaleImage msi in this.msiCollection)
        msi.Source = (MultiScaleTileSource) new MultiScaleQuadTileSource(this.TileSources, this.TileWidth, this.TileHeight);
      this.InvalidateMeasure();
    }

    public void ProjectionUpdated(ProjectionUpdateLevel updateLevel)
    {
      if (this.parentMode != null)
        return;
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }

    public MapBase ParentMap
    {
      get => this.Parent is IProjectable parent ? parent.ParentMap : this.Parent as MapBase;
    }

    private MapMode ParentMode
    {
      get
      {
        if (this.parentMode != null)
          return this.parentMode;
        return this.ParentMap?.Mode;
      }
    }

    private Size ViewportSize
    {
      get
      {
        Size viewportSize;
        if (this.parentMode != null)
        {
          viewportSize = this.parentMode.ViewportSize;
        }
        else
        {
          MapBase parentMap = this.ParentMap;
          viewportSize = parentMap != null ? parentMap.ViewportSize : new Size(0.0, 0.0);
        }
        return viewportSize;
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.UpdateMSI();
      this.contentGrid.Measure(this.ViewportSize);
      return this.ViewportSize;
    }

    private void ParentMode_ProjectionChanged(object sender, ProjectionChangedEventArgs e)
    {
      this.InvalidateMeasure();
      this.InvalidateArrange();
    }

    private MultiScaleImage CreateMSI()
    {
      MultiScaleImage msi = new MultiScaleImage();
      msi.ViewportWidth = 1.0;
      msi.ViewportOrigin = new Point();
      msi.UseSprings = false;
      msi.CacheMode = (CacheMode) new BitmapCache();
      msi.Source = (MultiScaleTileSource) new MultiScaleQuadTileSource(this.TileSources, this.TileWidth, this.TileHeight);
      return msi;
    }

    private MultiScaleImage GetMSI(int key)
    {
      MultiScaleImage msi;
      if (this.msiCollection.Count <= key)
      {
        msi = this.CreateMSI();
        this.msiCollection.Add(msi);
      }
      else
        msi = this.msiCollection[key];
      if (!this.contentGrid.Children.Contains((UIElement) msi))
        this.contentGrid.Children.Add((UIElement) msi);
      return msi;
    }

    private void UpdateMSI()
    {
      if (this.ParentMode is FlatMapMode parentMode)
      {
        this.contentGrid.Visibility = Visibility.Visible;
        Rect rect = new Rect(parentMode.ViewportPointToLogicalPoint(new Point(0.0, 0.0)), parentMode.ViewportPointToLogicalPoint(new Point(this.ViewportSize.Width, this.ViewportSize.Height)));
        double zoomLevel = parentMode.ZoomLevel;
        int num1 = (int) Math.Floor(zoomLevel);
        double num2 = Math.Pow(2.0, zoomLevel - (double) num1);
        bool atIntegerZoom = Math.Round(zoomLevel) == zoomLevel;
        bool leftCenter = false;
        bool rightCenter = false;
        if (atIntegerZoom)
        {
          Size size = new Size(rect.Width / this.ViewportSize.Width, rect.Height / this.ViewportSize.Height);
          rect.X = Math.Round(rect.X / size.Width) * size.Width;
          rect.Y = Math.Round(rect.Y / size.Height) * size.Height;
        }
        double x = rect.X - Math.Floor((rect.X + rect.Width / 2.0 + 0.5) / 2.0) * 2.0;
        if (!atIntegerZoom)
        {
          if (0.0 > x + rect.Width / 2.0)
            leftCenter = true;
          else if (1.0 < x + rect.Width / 2.0)
            rightCenter = true;
        }
        int num3 = num1 - this.UpSampleLevelDelta;
        MultiScaleImage msi1 = this.GetMSI(0);
        ((MultiScaleQuadTileSource) msi1.Source).MinTileZoomLevel = num3;
        msi1.ViewportWidth = rect.Width;
        msi1.ViewportOrigin = new Point(x, rect.Y);
        msi1.BlurFactor = num2;
        msi1.RenderTransform = MapTileLayer.GetRenderTransform(msi1, 0, atIntegerZoom, leftCenter, rightCenter);
        int num4 = 1;
        for (int index = 1; (double) index + x < 1.0; ++index)
        {
          MultiScaleImage msi2 = this.GetMSI(num4);
          ((MultiScaleQuadTileSource) msi2.Source).MinTileZoomLevel = num3;
          msi2.ViewportWidth = rect.Width;
          msi2.ViewportOrigin = new Point(x + (double) index, rect.Y);
          msi2.BlurFactor = num2;
          msi2.RenderTransform = MapTileLayer.GetRenderTransform(msi2, -index, atIntegerZoom, leftCenter, rightCenter);
          ++num4;
        }
        for (int offset = 1; (double) offset < rect.Width + x; ++offset)
        {
          MultiScaleImage msi3 = this.GetMSI(num4);
          ((MultiScaleQuadTileSource) msi3.Source).MinTileZoomLevel = num3;
          msi3.ViewportWidth = rect.Width;
          msi3.ViewportOrigin = new Point(x - (double) offset, rect.Y);
          msi3.BlurFactor = num2;
          msi3.RenderTransform = MapTileLayer.GetRenderTransform(msi3, offset, atIntegerZoom, leftCenter, rightCenter);
          ++num4;
        }
        for (; num4 < this.msiCollection.Count; ++num4)
          this.contentGrid.Children.Remove((UIElement) this.msiCollection[num4]);
      }
      else
        this.contentGrid.Visibility = Visibility.Collapsed;
      this.updated = true;
    }

    private static Transform GetRenderTransform(
      MultiScaleImage msi,
      int offset,
      bool atIntegerZoom,
      bool leftCenter,
      bool rightCenter)
    {
      renderTransform = (TranslateTransform) null;
      if (!atIntegerZoom)
      {
        if (!(msi.RenderTransform is TranslateTransform renderTransform))
        {
          renderTransform = new TranslateTransform();
          msi.RenderTransform = (Transform) renderTransform;
        }
        renderTransform.X = !leftCenter ? (!rightCenter ? (double) -offset * 0.75 : (double) (-offset + 1) * 0.75) : (double) (-offset - 1) * 0.75;
      }
      return (Transform) renderTransform;
    }

    private void TileSources_ItemPropertyChanged(
      object sender,
      TileSourcePropertyChangedEventArgs e)
    {
      this.updated = false;
      try
      {
        this.contentGrid.Dispatcher.BeginInvoke((Action) (() => this.RefreshSource()));
      }
      catch (Exception ex)
      {
      }
    }

    private void TileSources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.updated = false;
      try
      {
        this.contentGrid.Dispatcher.BeginInvoke((Action) (() => this.RefreshSource()));
      }
      catch (Exception ex)
      {
      }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new MapTileLayerAutomationPeer((FrameworkElement) this);
    }
  }
}
