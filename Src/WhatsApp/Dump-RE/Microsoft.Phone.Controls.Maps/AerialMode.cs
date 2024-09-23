// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AerialMode
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class AerialMode : MercatorMode
  {
    private const string lowDPIMarketsKey = "LowDPIMarkets";
    private const string aerialUriFromatKey = "AerialUriFormat";
    private const string aerialWithLabelsUriFormatKey = "AerialWithLabelsUriFormat";
    private const string aerialWithLabelsLowDPIUriFormatKey = "AerialWithLabelsLowDPIUriFormat";
    private const string aerialLabelsUriFormatKey = "AerialLabelsUriFormat";
    private const string aerialLabelsLowDPIUriFormatKey = "AerialLabelsLowDPIUriFormat";
    private const string aerialSubdomainsKey = "AerialUriSubdomains";
    private const string aerialWithLabelsSubdomainsKey = "AerialWithLabelsUriSubdomains";
    private const string aerialLabelsSubdomainsKey = "AerialLabelsUriSubdomains";
    private const string aerialCoverageMapKey = "AerialCoverageMap";
    private readonly MapTileLayer baseTileLayer;
    private readonly TfeTileSource baseTileSource;
    private readonly MapLayer contentLayer;
    private readonly CopyrightManager copyright;
    private readonly TimeSpan fadeInTime = new TimeSpan(0, 0, 0, 0, 150);
    private readonly TimeSpan fadeOutTime = new TimeSpan(0, 0, 0, 0, 1000);
    private readonly TimeSpan labelsTimeout = new TimeSpan(0, 0, 0, 0, 1100);
    private readonly MapTileLayer overlayTileLayer;
    private string[] lowDPIMarkets = new string[0];
    private readonly TfeTileSource overlayTileSource;
    private bool addedToMap;
    private string[][] aerialLabelsSubdomains;
    private string aerialLabelsUriFormat;
    private string aerialLabelsLowDPIUriFormat;
    private string[][] aerialSubdomains;
    private string aerialTileUriFormat;
    private string[][] aerialWithLabelsSubdomains;
    private string aerialWithLabelsUriFormat;
    private string aerialWithLabelsLowDPIUriFormat;
    private Uri coverageMapUri;
    private bool fadingLabels;
    private bool labels;
    private Storyboard labelsFadeOut;
    private bool restrictMaxZoomLevel;
    private bool configurationLoaded;

    public AerialMode()
      : this(false)
    {
    }

    public AerialMode(bool labels)
    {
      this.restrictMaxZoomLevel = false;
      this.labels = labels;
      this.baseTileLayer = new MapTileLayer((MapMode) this);
      this.baseTileLayer.UpSampleLevelDelta = 7;
      this.baseTileLayer.IsHitTestVisible = true;
      this.baseTileSource = new TfeTileSource();
      this.baseTileLayer.TileSources.Add((TileSource) this.baseTileSource);
      this.overlayTileLayer = new MapTileLayer((MapMode) this);
      this.overlayTileLayer.UpSampleLevelDelta = 0;
      this.overlayTileLayer.IsHitTestVisible = true;
      this.overlayTileSource = new TfeTileSource();
      this.overlayTileLayer.TileSources.Add((TileSource) this.overlayTileSource);
      this.contentLayer = new MapLayer();
      this.contentLayer.Children.Add((UIElement) this.baseTileLayer);
      this.UpdateTileLayers();
      this.copyright = CopyrightManager.GetInstance();
      MapConfiguration.GetSection("v1", "WP7SLMapControl", (string) null, new MapConfigurationCallback(this.AsynchronousConfigurationLoaded));
    }

    public bool ShouldRestrictMaxZoomLevel
    {
      get => this.restrictMaxZoomLevel;
      set
      {
        this.restrictMaxZoomLevel = value;
        this.SetTfeCoverageMap();
      }
    }

    public bool ShouldDisplayLabels
    {
      get => this.labels;
      set
      {
        this.labels = value;
        this.UpdateTileLayers();
      }
    }

    public bool ShouldDisplayFadingLabels
    {
      get => this.fadingLabels;
      set
      {
        this.fadingLabels = value;
        this.UpdateTileLayers();
      }
    }

    public int UpSampleLevelDelta
    {
      get => this.baseTileLayer.UpSampleLevelDelta;
      set => this.baseTileLayer.UpSampleLevelDelta = value;
    }

    public override UIElement Content => (UIElement) this.contentLayer;

    public override bool IsDownloading
    {
      get
      {
        if (this.baseTileLayer != null && this.baseTileLayer.IsDownloading)
          return true;
        return this.ShouldDisplayLabels && this.overlayTileLayer != null && this.overlayTileLayer.IsDownloading;
      }
    }

    public override bool IsIdle
    {
      get
      {
        if (!this.configurationLoaded)
          return false;
        if (this.baseTileLayer != null && this.baseTileLayer.IsIdle)
          return true;
        return this.ShouldDisplayLabels && this.overlayTileLayer != null && this.overlayTileLayer.IsIdle;
      }
    }

    protected override Range<double> GetZoomRange(GeoCoordinate center)
    {
      return this.GetZoomRange(this.baseTileSource, center);
    }

    protected override void OnCultureChanged()
    {
      if (!this.addedToMap)
        return;
      this.SetupMode();
    }

    internal override void UpdateAttribution()
    {
      this.copyright.RequestCopyrightString(this.ShouldDisplayLabels ? MapStyle.AerialWithLabels : MapStyle.Aerial, this.Center, this.ZoomLevel, this.CredentialsProvider, this.Culture, new Action<CopyrightResult>(this.CopyrightCallback));
    }

    private void CopyrightCallback(CopyrightResult result)
    {
      if (result.Culture != this.Culture || !(result.Location == this.Center) || result.ZoomLevel != this.ZoomLevel)
        return;
      this.UpdateAttribution(result.CopyrightStrings);
    }

    public override void Activating(
      MapMode previousMode,
      MapLayerBase modeLayer,
      MapLayerBase modeForegroundLayer)
    {
      base.Activating(previousMode, modeLayer, modeForegroundLayer);
      this.SetupMode();
      this.addedToMap = true;
    }

    private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
    {
      if (config != null)
      {
        string str = config["LowDPIMarkets"];
        if (!string.IsNullOrEmpty(str))
          this.lowDPIMarkets = str.Split(';');
        this.aerialTileUriFormat = config["AerialUriFormat"];
        this.aerialLabelsUriFormat = config["AerialLabelsUriFormat"];
        this.aerialLabelsLowDPIUriFormat = config["AerialLabelsLowDPIUriFormat"];
        this.aerialWithLabelsUriFormat = config["AerialWithLabelsUriFormat"];
        this.aerialWithLabelsLowDPIUriFormat = config["AerialWithLabelsLowDPIUriFormat"];
        string[][] subdomains;
        this.aerialSubdomains = !config.Contains("AerialUriSubdomains") || !MercatorMode.TryParseSubdomains(config["AerialUriSubdomains"], out subdomains) ? (string[][]) null : subdomains;
        this.aerialWithLabelsSubdomains = !config.Contains("AerialWithLabelsUriSubdomains") || !MercatorMode.TryParseSubdomains(config["AerialWithLabelsUriSubdomains"], out subdomains) ? (string[][]) null : subdomains;
        this.aerialLabelsSubdomains = !config.Contains("AerialLabelsUriSubdomains") || !MercatorMode.TryParseSubdomains(config["AerialLabelsUriSubdomains"], out subdomains) ? (string[][]) null : subdomains;
        this.UpdateTileLayers();
        if (config.Contains("AerialCoverageMap"))
        {
          this.coverageMapUri = new Uri(config["AerialCoverageMap"].Replace("{UriScheme}", "HTTP"), UriKind.Absolute);
          if (this.addedToMap)
            this.SetupMode();
        }
      }
      this.configurationLoaded = true;
    }

    private void SetupMode()
    {
      this.UpdateTileLayers();
      this.UpdateAttribution(ProjectionUpdateLevel.None);
      this.SetTfeCoverageMap();
    }

    private void SetTfeCoverageMap()
    {
      this.baseTileSource.SetCoverageMapUri(this.restrictMaxZoomLevel ? this.coverageMapUri : (Uri) null);
    }

    private void UpdateTileLayers()
    {
      if (this.ShouldDisplayLabels)
      {
        bool flag = false;
        if (this.Culture != null)
        {
          string lowerInvariant = this.Culture.Name.ToLowerInvariant();
          foreach (string lowDpiMarket in this.lowDPIMarkets)
          {
            if (!string.IsNullOrEmpty(lowDpiMarket) && lowDpiMarket.ToLowerInvariant() == lowerInvariant)
            {
              flag = true;
              break;
            }
          }
        }
        if (this.ShouldDisplayFadingLabels)
        {
          string uriFormat = !flag || string.IsNullOrEmpty(this.aerialLabelsLowDPIUriFormat) ? this.aerialLabelsUriFormat : this.aerialLabelsLowDPIUriFormat;
          this.SetUriFormat(this.baseTileSource, this.aerialTileUriFormat, this.aerialSubdomains);
          this.AddOverlayTileLayer();
          this.SetUriFormat(this.overlayTileSource, uriFormat, this.aerialLabelsSubdomains);
          this.EnableFadingLabels();
        }
        else
        {
          this.SetUriFormat(this.baseTileSource, !flag || string.IsNullOrEmpty(this.aerialWithLabelsLowDPIUriFormat) ? this.aerialWithLabelsUriFormat : this.aerialWithLabelsLowDPIUriFormat, this.aerialWithLabelsSubdomains);
          this.RemoveOverlayTileLayer();
          this.DisableFadingLabels();
        }
      }
      else
      {
        this.SetUriFormat(this.baseTileSource, this.aerialTileUriFormat, this.aerialSubdomains);
        this.RemoveOverlayTileLayer();
        this.DisableFadingLabels();
      }
    }

    private void SetUriFormat(TfeTileSource tileSource, string uriFormat, string[][] subdomains)
    {
      if (string.IsNullOrEmpty(uriFormat))
        return;
      tileSource.UriFormat = uriFormat.Replace("{Culture}", this.Culture.Name);
      tileSource.SetSubdomains(subdomains);
    }

    private void AddOverlayTileLayer()
    {
      if (this.contentLayer.Children.Contains((UIElement) this.overlayTileLayer))
        return;
      this.contentLayer.Children.Add((UIElement) this.overlayTileLayer);
    }

    private void RemoveOverlayTileLayer()
    {
      if (!this.contentLayer.Children.Contains((UIElement) this.overlayTileLayer))
        return;
      this.contentLayer.Children.Remove((UIElement) this.overlayTileLayer);
    }

    private void EnableFadingLabels()
    {
      if (this.labelsFadeOut == null)
      {
        this.labelsFadeOut = new Storyboard();
        DoubleAnimationUsingKeyFrames element = new DoubleAnimationUsingKeyFrames();
        DoubleKeyFrameCollection keyFrames1 = element.KeyFrames;
        LinearDoubleKeyFrame linearDoubleKeyFrame1 = new LinearDoubleKeyFrame();
        linearDoubleKeyFrame1.Value = 1.0;
        linearDoubleKeyFrame1.KeyTime = (KeyTime) this.fadeInTime;
        LinearDoubleKeyFrame linearDoubleKeyFrame2 = linearDoubleKeyFrame1;
        keyFrames1.Add((DoubleKeyFrame) linearDoubleKeyFrame2);
        DoubleKeyFrameCollection keyFrames2 = element.KeyFrames;
        LinearDoubleKeyFrame linearDoubleKeyFrame3 = new LinearDoubleKeyFrame();
        linearDoubleKeyFrame3.Value = 1.0;
        linearDoubleKeyFrame3.KeyTime = (KeyTime) (this.fadeInTime + this.labelsTimeout);
        LinearDoubleKeyFrame linearDoubleKeyFrame4 = linearDoubleKeyFrame3;
        keyFrames2.Add((DoubleKeyFrame) linearDoubleKeyFrame4);
        DoubleKeyFrameCollection keyFrames3 = element.KeyFrames;
        LinearDoubleKeyFrame linearDoubleKeyFrame5 = new LinearDoubleKeyFrame();
        linearDoubleKeyFrame5.Value = 0.0;
        linearDoubleKeyFrame5.KeyTime = (KeyTime) (this.fadeInTime + this.labelsTimeout + this.fadeOutTime);
        LinearDoubleKeyFrame linearDoubleKeyFrame6 = linearDoubleKeyFrame5;
        keyFrames3.Add((DoubleKeyFrame) linearDoubleKeyFrame6);
        Storyboard.SetTarget((Timeline) element, (DependencyObject) this.overlayTileLayer);
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath((object) UIElement.OpacityProperty));
        this.labelsFadeOut.Children.Add((Timeline) element);
      }
      this.ProjectionChanged += new EventHandler<ProjectionChangedEventArgs>(this.MapMode_ProjectionChangedForFadingLabels);
      this.contentLayer.MouseMove += new MouseEventHandler(this.ContentLayer_MouseMoveForFadingLabels);
      this.overlayTileLayer.Opacity = 1.0;
    }

    private void DisableFadingLabels()
    {
      this.ProjectionChanged -= new EventHandler<ProjectionChangedEventArgs>(this.MapMode_ProjectionChangedForFadingLabels);
      this.contentLayer.MouseMove -= new MouseEventHandler(this.ContentLayer_MouseMoveForFadingLabels);
      if (this.labelsFadeOut != null)
        this.labelsFadeOut.Stop();
      this.overlayTileLayer.Opacity = 1.0;
    }

    private void MapMode_ProjectionChangedForFadingLabels(
      object sender,
      ProjectionChangedEventArgs e)
    {
      this.ShowLabels();
    }

    private void ContentLayer_MouseMoveForFadingLabels(object sender, MouseEventArgs e)
    {
      this.ShowLabels();
    }

    private void ShowLabels()
    {
      if (this.labelsFadeOut != null)
        this.labelsFadeOut.Begin();
      else
        this.overlayTileLayer.Opacity = 1.0;
      if (this.overlayTileLayer.IsIdle)
        return;
      this.overlayTileLayer.Dispatcher.BeginInvoke(new Action(this.ShowLabels));
    }
  }
}
