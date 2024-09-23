// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TfeTileSource
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class TfeTileSource : TileSource
  {
    public TfeTileSource()
    {
    }

    public TfeTileSource(string uriFormat, Uri coverageMapUri)
      : base(uriFormat)
    {
      this.SetCoverageMapUri(coverageMapUri);
    }

    internal TfeCoverageMap TileCoverageMap { private get; set; }

    public bool CoverageMapLoaded => this.TileCoverageMap != null && this.TileCoverageMap.Loaded;

    public int MinZoomLevel
    {
      get => this.TileCoverageMap != null ? this.TileCoverageMap.MinZoomLevel : -1;
    }

    public int MaxZoomLevel
    {
      get => this.TileCoverageMap != null ? this.TileCoverageMap.MaxZoomLevel : -1;
    }

    public void SetCoverageMapUri(Uri coverageMapUri)
    {
      if (coverageMapUri != (Uri) null)
        TfeCoverageMap.GetInstance(coverageMapUri, new TfeCoverageMapCallback(this.TfeCoverageMapLoaded));
      else
        this.TileCoverageMap = (TfeCoverageMap) null;
    }

    public bool Covers(QuadKey quadKey)
    {
      return this.TileCoverageMap == null || this.TileCoverageMap.Covers(quadKey);
    }

    public int GetMaximumZoomLevel(QuadKey quadKey)
    {
      return this.TileCoverageMap != null ? this.TileCoverageMap.GetMaximumZoomLevel(quadKey) : -1;
    }

    private void TfeCoverageMapLoaded(TfeCoverageMap coverageMap, object userState)
    {
      this.TileCoverageMap = coverageMap;
    }
  }
}
