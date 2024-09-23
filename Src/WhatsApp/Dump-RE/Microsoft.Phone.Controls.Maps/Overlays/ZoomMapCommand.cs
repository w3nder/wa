// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Overlays.ZoomMapCommand
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Overlays
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class ZoomMapCommand : MapCommandBase
  {
    private readonly bool zoomIn;

    public ZoomMapCommand(bool zoomIn) => this.zoomIn = zoomIn;

    public override void Execute(MapBase map)
    {
      if (this.zoomIn)
        map.ZoomLevel = Math.Round(map.ZoomLevel + 1.0);
      else
        map.ZoomLevel = Math.Round(map.ZoomLevel - 1.0);
    }
  }
}
