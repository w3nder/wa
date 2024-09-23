// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.RoadMode
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class RoadMode : MercatorMode
  {
    private const string lowDPIMarketsKey = "LowDPIMarkets";
    private const string subdomainsKey = "RoadUriSubdomains";
    private const string uriFormatKey = "RoadUriFormat";
    private const string uriLowDPIFormatKey = "RoadLowDPIUriFormat";
    private const string coverageMapKey = "RoadCoverageMap";
    private readonly MapTileLayer tileLayer;
    private readonly TfeTileSource tileSource;
    private bool addedToMap;
    private Uri coverageMapUri;
    private bool restrictMaxZoomLevel;
    private string[] lowDPIMarkets = new string[0];
    private string tileUriFormat;
    private string tileLowDPIUriFormat;
    private bool configurationLoaded;
    private CopyrightManager copyright;

    public RoadMode()
    {
      this.restrictMaxZoomLevel = false;
      this.tileLayer = new MapTileLayer((MapMode) this);
      this.tileLayer.UpSampleLevelDelta = 7;
      this.tileSource = new TfeTileSource();
      this.tileLayer.TileSources.Add((TileSource) this.tileSource);
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

    public int UpSampleLevelDelta
    {
      get => this.tileLayer.UpSampleLevelDelta;
      set => this.tileLayer.UpSampleLevelDelta = value;
    }

    public override UIElement Content => (UIElement) this.tileLayer;

    public override ModeBackground ModeBackground => ModeBackground.Light;

    public override bool IsDownloading => this.tileLayer != null && this.tileLayer.IsDownloading;

    public override bool IsIdle
    {
      get => this.configurationLoaded && this.tileLayer != null && this.tileLayer.IsIdle;
    }

    protected override Range<double> GetZoomRange(GeoCoordinate center)
    {
      return this.GetZoomRange(this.tileSource, center);
    }

    private void AsynchronousConfigurationLoaded(MapConfigurationSection config, object userState)
    {
      if (config != null)
      {
        string str = config["LowDPIMarkets"];
        if (!string.IsNullOrEmpty(str))
          this.lowDPIMarkets = str.Split(';');
        this.tileUriFormat = config["RoadUriFormat"];
        this.tileLowDPIUriFormat = config["RoadLowDPIUriFormat"];
        string[][] subdomains;
        if (config.Contains("RoadUriSubdomains") && MercatorMode.TryParseSubdomains(config["RoadUriSubdomains"], out subdomains))
          this.tileSource.SetSubdomains(subdomains);
        this.UpdateTileSource();
        if (config.Contains("RoadCoverageMap"))
        {
          this.coverageMapUri = new Uri(config["RoadCoverageMap"].Replace("{UriScheme}", "HTTP"), UriKind.Absolute);
          if (this.addedToMap)
            this.SetupMode();
        }
      }
      this.configurationLoaded = true;
    }

    protected override void OnCultureChanged()
    {
      if (!this.addedToMap)
        return;
      this.SetupMode();
    }

    internal override void UpdateAttribution()
    {
      this.copyright.RequestCopyrightString(MapStyle.Road, this.Center, this.ZoomLevel, this.CredentialsProvider, this.Culture, new Action<CopyrightResult>(this.CopyrightCallback));
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

    private void SetupMode()
    {
      this.UpdateTileSource();
      this.UpdateAttribution(ProjectionUpdateLevel.None);
      this.SetTfeCoverageMap();
    }

    private void UpdateTileSource()
    {
      if (this.Culture == null)
        return;
      string lowerInvariant = this.Culture.Name.ToLowerInvariant();
      bool flag = false;
      foreach (string lowDpiMarket in this.lowDPIMarkets)
      {
        if (!string.IsNullOrEmpty(lowDpiMarket) && lowDpiMarket.ToLowerInvariant() == lowerInvariant)
        {
          flag = true;
          break;
        }
      }
      if (flag && !string.IsNullOrEmpty(this.tileLowDPIUriFormat))
      {
        this.tileSource.UriFormat = this.tileLowDPIUriFormat.Replace("{Culture}", this.Culture.Name);
      }
      else
      {
        if (string.IsNullOrEmpty(this.tileUriFormat))
          return;
        this.tileSource.UriFormat = this.tileUriFormat.Replace("{Culture}", this.Culture.Name);
      }
    }

    private void SetTfeCoverageMap()
    {
      this.tileSource.SetCoverageMapUri(this.restrictMaxZoomLevel ? this.coverageMapUri : (Uri) null);
    }
  }
}
