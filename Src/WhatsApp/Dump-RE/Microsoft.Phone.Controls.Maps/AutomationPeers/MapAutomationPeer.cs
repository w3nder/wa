// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.AutomationPeers.MapAutomationPeer
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.Core;
using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.AutomationPeers
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public class MapAutomationPeer : MapBaseAutomationPeer
  {
    public MapAutomationPeer(Microsoft.Phone.Controls.Maps.Map element)
      : base((MapBase) element, "Map")
    {
    }

    public void DoubleTap(Point viewportPoint)
    {
      ((MapCore) this.Map).InputManager.OnTouchZoom(new MapZoomEventArgs(viewportPoint, 1.0));
    }

    public void Drag(Point viewportPoint, Point dragDelta)
    {
      ((MapCore) this.Map).InputManager.OnTouchDrag(new MapDragEventArgs(viewportPoint, dragDelta));
    }

    public void Flick(Point viewportPoint, Point velocity)
    {
      ((MapCore) this.Map).InputManager.OnTouchFlick(new MapFlickEventArgs(viewportPoint, velocity));
    }

    public void Zoom(Point viewportPoint, double zoomDelta)
    {
      ((MapCore) this.Map).InputManager.OnTouchZoom(new MapZoomEventArgs(viewportPoint, zoomDelta));
    }

    public void Scale(Point viewportPoint, double scaleDelta)
    {
      ((MapCore) this.Map).InputManager.PinchStretchReported((object) this.Map, new PinchStretchEventArgs(new PinchStretchData()
      {
        CenterPoint = viewportPoint,
        Scale = scaleDelta
      }));
    }
  }
}
