// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MultiScaleQuadTileSource
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal sealed class MultiScaleQuadTileSource : MultiScaleTileSource
  {
    private const int minZoomLevel = 1;
    private const int maxZoomLevel = 22;
    private const int numberOfEdgeTilesAtMaxZoomLevel = 4194304;
    private readonly List<MultiScaleQuadTileSource.TileInfo>[] fakeNoTile;
    private readonly TileSourceCollection tileSources;
    private readonly int zoomLevelDelta;
    private int minTileZoomLevel;

    public MultiScaleQuadTileSource(TileSource tileSource, int tileWidth, int tileHeight)
      : this(tileWidth, tileHeight)
    {
      if (tileSource == null)
        return;
      this.tileSources.Add(tileSource);
    }

    public MultiScaleQuadTileSource(
      TileSourceCollection tileSources,
      int tileWidth,
      int tileHeight)
      : this(tileWidth, tileHeight)
    {
      if (tileSources == null)
        return;
      foreach (TileSource tileSource in (Collection<TileSource>) tileSources)
        this.tileSources.Add(tileSource);
    }

    private MultiScaleQuadTileSource(int tileWidth, int tileHeight)
      : base(4194304 * tileWidth, 4194304 * tileHeight, tileWidth, tileHeight, 0)
    {
      this.tileSources = new TileSourceCollection();
      this.zoomLevelDelta = (int) Math.Log((double) tileWidth, 2.0);
      this.TileBlendTime = new TimeSpan(0, 0, 0, 0, 250);
      this.fakeNoTile = new List<MultiScaleQuadTileSource.TileInfo>[23];
      for (int index = 0; index < this.fakeNoTile.Length; ++index)
        this.fakeNoTile[index] = new List<MultiScaleQuadTileSource.TileInfo>();
    }

    public TileSourceCollection TileSources => this.tileSources;

    public int MinTileZoomLevel
    {
      get => this.minTileZoomLevel;
      set
      {
        int minTileZoomLevel = this.minTileZoomLevel;
        this.minTileZoomLevel = value;
        if (this.minTileZoomLevel >= minTileZoomLevel)
          return;
        this.RefreshInvalidTiles();
      }
    }

    protected override void GetTileLayers(
      int tileLevel,
      int tilePositionX,
      int tilePositionY,
      IList<object> tileImageLayerSources)
    {
      int zoomLevel = tileLevel - this.zoomLevelDelta;
      int minTileZoomLevel = this.MinTileZoomLevel;
      if (zoomLevel < 1 || zoomLevel > 22)
        return;
      int num = 0;
      foreach (TileSource tileSource in (Collection<TileSource>) this.tileSources)
      {
        Uri uri;
        if (zoomLevel < minTileZoomLevel)
        {
          uri = (Uri) null;
          this.fakeNoTile[zoomLevel].Add(new MultiScaleQuadTileSource.TileInfo()
          {
            X = tilePositionX,
            Y = tilePositionY,
            Layer = num
          });
        }
        else
          uri = tileSource.GetUri(tilePositionX, tilePositionY, zoomLevel);
        tileImageLayerSources.Add((object) uri);
        ++num;
      }
    }

    private void RefreshInvalidTiles()
    {
      for (int index = this.MinTileZoomLevel < 1 ? 1 : this.MinTileZoomLevel; index < this.fakeNoTile.Length; ++index)
      {
        List<MultiScaleQuadTileSource.TileInfo> tileInfoList = this.fakeNoTile[index];
        foreach (MultiScaleQuadTileSource.TileInfo tileInfo in tileInfoList)
          this.InvalidateTileLayer(index + this.zoomLevelDelta, tileInfo.X, tileInfo.Y, tileInfo.Layer);
        tileInfoList.Clear();
      }
    }

    [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
    private struct TileInfo
    {
      public int X { get; set; }

      public int Y { get; set; }

      public int Layer { get; set; }
    }
  }
}
