// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.MapForeground
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Data;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [TemplatePart(Name = "ZoomBarElement", Type = typeof (ZoomBar))]
  [TemplatePart(Name = "CopyrightElement", Type = typeof (Copyright))]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  [TemplatePart(Name = "LogoElement", Type = typeof (Logo))]
  [TemplatePart(Name = "ScaleElement", Type = typeof (Scale))]
  public class MapForeground : Overlay
  {
    internal const string ScaleElementName = "ScaleElement";
    internal const string CopyrightElementName = "CopyrightElement";
    internal const string LogoElementName = "LogoElement";
    internal const string ZoomBarElementName = "ZoomBarElement";
    private const int refreshMilliseconds = 500;
    private static readonly long refreshTicks = new TimeSpan(0, 0, 0, 0, 500).Ticks;
    private readonly MapBase targetMap;
    private Copyright copyright;
    private long lastOverlayRefresh;
    private double lastZoomLevel;
    private Logo logo;
    private MapMode previousMode;
    private Scale scale;
    private ZoomBar zoomBar;
    public static readonly DependencyProperty CultureProperty = DependencyProperty.Register(nameof (Culture), typeof (string), typeof (MapForeground), new PropertyMetadata(new PropertyChangedCallback(MapForeground.OnCultureChanged)));

    internal MapForeground(MapBase map)
    {
      this.DefaultStyleKey = (object) typeof (MapForeground);
      this.targetMap = map;
      this.logo = new Logo();
      this.copyright = new Copyright();
      this.scale = new Scale();
      this.zoomBar = new ZoomBar();
      this.zoomBar.ZoomMap += new EventHandler<MapCommandEventArgs>(this.ZoomBar_ZoomMap);
      this.Attach();
    }

    public string Culture
    {
      get => (string) this.GetValue(MapForeground.CultureProperty);
      set => this.SetValue(MapForeground.CultureProperty, (object) value);
    }

    public Copyright Copyright
    {
      get => this.copyright;
      private set
      {
        if (this.copyright != null && value != null)
        {
          value.Visibility = this.copyright.Visibility;
          value.Attributions = this.copyright.Attributions;
          value.SetForBackground = this.copyright.SetForBackground;
        }
        this.copyright = value;
      }
    }

    public Logo Logo
    {
      get => this.logo;
      private set
      {
        if (this.logo != null && value != null)
          value.Visibility = this.logo.Visibility;
        this.logo = value;
      }
    }

    public Scale Scale
    {
      get => this.scale;
      private set
      {
        if (this.scale != null && value != null)
        {
          value.Visibility = this.scale.Visibility;
          value.MetersPerPixel = this.scale.MetersPerPixel;
          value.DistanceUnit = this.scale.DistanceUnit;
          value.Culture = this.scale.Culture;
          value.SetForBackground = this.scale.SetForBackground;
        }
        this.scale = value;
      }
    }

    public ZoomBar ZoomBar
    {
      get => this.zoomBar;
      private set
      {
        if (this.zoomBar != null && value != null)
          value.Visibility = this.zoomBar.Visibility;
        if (this.zoomBar != null)
          this.zoomBar.ZoomMap -= new EventHandler<MapCommandEventArgs>(this.ZoomBar_ZoomMap);
        this.zoomBar = value;
        if (this.zoomBar == null)
          return;
        this.zoomBar.ZoomMap += new EventHandler<MapCommandEventArgs>(this.ZoomBar_ZoomMap);
      }
    }

    private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((MapForeground) d).OnCultureChanged(e.OldValue as CultureInfo, e.NewValue as CultureInfo);
    }

    protected virtual void OnCultureChanged(CultureInfo oldValue, CultureInfo newValue)
    {
      if (this.Scale == null || this.Scale.Culture != null && this.Scale.Culture != oldValue)
        return;
      this.Scale.Culture = newValue;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.Logo = this.GetTemplateChild("LogoElement") as Logo;
      this.Scale = this.GetTemplateChild("ScaleElement") as Scale;
      this.Copyright = this.GetTemplateChild("CopyrightElement") as Copyright;
      this.ZoomBar = this.GetTemplateChild("ZoomBarElement") as ZoomBar;
      this.Attach();
      this.FireTemplateApplied();
    }

    internal void UpdateScale()
    {
      this.lastOverlayRefresh = DateTime.Now.Ticks;
      this.lastZoomLevel = this.targetMap.ZoomLevel;
      if (this.Scale == null)
        return;
      this.Scale.MetersPerPixel = this.targetMap.Mode.Scale;
    }

    internal void Attach()
    {
      if (this.Logo != null)
        this.Logo.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Mode = BindingMode.TwoWay,
          Source = (object) this.targetMap,
          Path = new PropertyPath("LogoVisibility", new object[0])
        });
      if (this.Copyright != null)
        this.Copyright.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Mode = BindingMode.TwoWay,
          Source = (object) this.targetMap,
          Path = new PropertyPath("CopyrightVisibility", new object[0])
        });
      if (this.Scale != null)
        this.Scale.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Mode = BindingMode.TwoWay,
          Source = (object) this.targetMap,
          Path = new PropertyPath("ScaleVisibility", new object[0])
        });
      if (this.ZoomBar != null)
        this.ZoomBar.SetBinding(UIElement.VisibilityProperty, new Binding()
        {
          Mode = BindingMode.TwoWay,
          Source = (object) this.targetMap,
          Path = new PropertyPath("ZoomBarVisibility", new object[0])
        });
      this.Refresh();
      this.targetMap.ViewChangeStart += new EventHandler<MapEventArgs>(this.Map_ViewChangeStart);
      this.targetMap.ViewChangeOnFrame += new EventHandler<MapEventArgs>(this.Map_ViewChangeOnFrame);
      this.targetMap.ViewChangeEnd += new EventHandler<MapEventArgs>(this.Map_ViewChangeEnd);
      this.targetMap.ModeChanged += new EventHandler<MapEventArgs>(this.Map_ModeChange);
      this.Map_ModeChange((object) this.targetMap, (MapEventArgs) null);
    }

    private void Attributions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.UpdateAttributions();
    }

    private void UpdateAttributions()
    {
      MapForeground.SyncAttributions((IList<AttributionInfo>) this.Copyright.Attributions, (IList<AttributionInfo>) this.targetMap.Mode.Attributions);
    }

    private static void SyncAttributions(
      IList<AttributionInfo> list,
      IList<AttributionInfo> desired)
    {
      if (list == null)
        return;
      if (desired != null)
      {
        int index = 0;
        while (index < list.Count)
        {
          if (!desired.Contains(list[index]))
            list.RemoveAt(index);
          else
            ++index;
        }
        foreach (AttributionInfo attributionInfo in (IEnumerable<AttributionInfo>) desired)
        {
          if (!list.Contains(attributionInfo))
            list.Add(attributionInfo);
        }
      }
      else
        list.Clear();
    }

    private void Map_ViewChangeEnd(object sender, MapEventArgs e) => this.UpdateScale();

    private void Map_ViewChangeStart(object sender, MapEventArgs e)
    {
      this.lastOverlayRefresh = DateTime.Now.Ticks;
    }

    private void Map_ViewChangeOnFrame(object sender, MapEventArgs e)
    {
      if (Math.Round(this.lastZoomLevel) == Math.Round(this.targetMap.ZoomLevel) && DateTime.Now.Ticks - this.lastOverlayRefresh < MapForeground.refreshTicks)
        return;
      this.UpdateScale();
    }

    private void Map_ModeChange(object sender, MapEventArgs e)
    {
      if (this.previousMode != null)
        this.previousMode.Attributions.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Attributions_CollectionChanged);
      this.previousMode = this.targetMap.Mode;
      this.previousMode.Attributions.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Attributions_CollectionChanged);
      this.UpdateAttributions();
      this.Refresh();
    }

    private void Refresh()
    {
      this.UpdateScale();
      if (this.Copyright != null)
        this.Copyright.SetForBackground = this.targetMap.Mode.ModeBackground;
      if (this.Scale == null)
        return;
      this.Scale.SetForBackground = this.targetMap.Mode.ModeBackground;
    }

    private void ZoomBar_ZoomMap(object sender, MapCommandEventArgs e)
    {
      if (e.Command == null)
        return;
      e.Command.Execute(this.targetMap);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (MapForeground));
    }
  }
}
