// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapImplementation
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Device.Location;
using System.Windows;

#nullable disable
namespace WhatsApp
{
  public interface MapImplementation
  {
    UIElement Element { get; }

    MapPoint AddPoint(MapPointStyle style = MapPointStyle.Default, string label = null);

    GeoCoordinate Center { get; set; }

    double ZoomLevel { get; set; }

    MapMode CartographicMode { get; set; }

    void SetView(GeoCoordinate center, double widthInDegrees, double heightInDegrees);

    double ShownAreaRadius();

    IObservable<EventArgs> ViewChanged();

    IDisposable ApplyMemoryWorkaround(PhoneApplicationPage page);
  }
}
