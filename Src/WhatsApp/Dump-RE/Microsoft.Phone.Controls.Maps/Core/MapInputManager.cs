// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Core.MapInputManager
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using System;
using System.Windows;

#nullable disable
namespace Microsoft.Phone.Controls.Maps.Core
{
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  internal class MapInputManager
  {
    private readonly MapCore parentMap;
    private ManipulationHandler manipulationProcessor;
    private PinchStretchHandler pinchStretchRecognizer;

    internal MapInputManager(MapCore map)
    {
      this.parentMap = map;
      this.parentMap.Loaded += (RoutedEventHandler) ((o, e) => this.pinchStretchRecognizer.Enable(true));
      this.parentMap.Unloaded += (RoutedEventHandler) ((o, e) =>
      {
        this.pinchStretchRecognizer.Enable(false);
        if (this.manipulationProcessor == null)
          return;
        this.manipulationProcessor.Unload();
      });
      this.Initialize();
    }

    private void Initialize()
    {
      this.manipulationProcessor = new ManipulationHandler((UIElement) this.parentMap);
      this.manipulationProcessor.DoubleTap += new EventHandler<GestureEventArgs>(this.OnDoubleTap);
      this.manipulationProcessor.Pan += new EventHandler<GestureEventArgs>(this.OnPan);
      this.manipulationProcessor.Flick += new EventHandler<GestureEventArgs>(this.OnFlick);
      this.pinchStretchRecognizer = new PinchStretchHandler((UIElement) this.parentMap);
      this.pinchStretchRecognizer.PinchStretchReported += new EventHandler<PinchStretchEventArgs>(this.PinchStretchReported);
    }

    internal void OnDoubleTap(object sender, GestureEventArgs e)
    {
      Point origin = (e as DoubleTapGestureEventArgs).Origin;
      this.OnTouchZoom(new MapZoomEventArgs(e.Origin, 1.0));
    }

    internal void OnPan(object sender, GestureEventArgs e)
    {
      PanGestureEventArgs gestureEventArgs = e as PanGestureEventArgs;
      this.OnTouchDrag(new MapDragEventArgs(gestureEventArgs.Origin, gestureEventArgs.Translation));
    }

    internal void OnFlick(object sender, GestureEventArgs e)
    {
      FlickGestureEventArgs gestureEventArgs = e as FlickGestureEventArgs;
      this.OnTouchFlick(new MapFlickEventArgs(gestureEventArgs.Origin, gestureEventArgs.Velocity));
    }

    internal void OnScale(object sender, GestureEventArgs e)
    {
      ScaleGestureEventArgs gestureEventArgs = e as ScaleGestureEventArgs;
      this.OnTouchZoom(new MapZoomEventArgs(gestureEventArgs.Origin, Math.Log(gestureEventArgs.Scale.X != 1.0 ? (gestureEventArgs.Scale.Y != 1.0 ? (gestureEventArgs.Scale.X + gestureEventArgs.Scale.Y) / 2.0 : gestureEventArgs.Scale.Y) : gestureEventArgs.Scale.Y) / Math.Log(2.0)));
    }

    internal void PinchStretchReported(object sender, PinchStretchEventArgs e)
    {
      PinchStretchData data = e.Data;
      double zoomDelta = Math.Log(data.Scale) / Math.Log(2.0);
      this.OnTouchZoom(new MapZoomEventArgs(data.CenterPoint, zoomDelta));
    }

    internal event EventHandler<MapDragEventArgs> TouchDrag;

    internal void OnTouchDrag(MapDragEventArgs ea)
    {
      EventHandler<MapDragEventArgs> touchDrag = this.TouchDrag;
      if (touchDrag != null)
        touchDrag((object) this.parentMap, ea);
      if (ea.Handled)
        return;
      this.parentMap.Mode.OnMapDrag(ea);
    }

    internal event EventHandler<MapFlickEventArgs> TouchFlick;

    internal void OnTouchFlick(MapFlickEventArgs ea)
    {
      EventHandler<MapFlickEventArgs> touchFlick = this.TouchFlick;
      if (touchFlick != null)
        touchFlick((object) this.parentMap, ea);
      if (ea.Handled)
        return;
      this.parentMap.Mode.OnMapFlick(ea);
    }

    internal event EventHandler<MapZoomEventArgs> TouchZoom;

    internal void OnTouchZoom(MapZoomEventArgs ea)
    {
      EventHandler<MapZoomEventArgs> touchZoom = this.TouchZoom;
      if (touchZoom != null)
        touchZoom((object) this.parentMap, ea);
      if (ea.Handled)
        return;
      this.parentMap.Mode.OnMapZoom(ea);
    }
  }
}
