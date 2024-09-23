// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.TfeZoomLevelRange
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal sealed class TfeZoomLevelRange
  {
    private readonly List<int> generations = new List<int>();
    private readonly byte maxZoomLevel;
    private readonly byte minZoomLevel;
    private readonly List<string> regions = new List<string>();

    public TfeZoomLevelRange(byte minZoomLevel, byte maxZoomLevel)
    {
      this.minZoomLevel = minZoomLevel;
      this.maxZoomLevel = maxZoomLevel;
    }

    public void AddRegion(string basequadKey, int generation)
    {
      this.regions.Add(basequadKey);
      this.generations.Add(generation);
    }

    public int GetGeneration(string quadKey)
    {
      if (quadKey.Length < (int) this.minZoomLevel || quadKey.Length > (int) this.maxZoomLevel)
        return -1;
      int index = this.regions.BinarySearch(quadKey);
      if (index >= 0)
        return this.generations[index];
      int num = ~index;
      return num == 0 || num > this.regions.Count || !quadKey.StartsWith(this.regions[num - 1], StringComparison.Ordinal) ? -1 : this.generations[num - 1];
    }

    public int MinimumZoom => (int) this.minZoomLevel;

    public int MaximumZoom => (int) this.maxZoomLevel;
  }
}
