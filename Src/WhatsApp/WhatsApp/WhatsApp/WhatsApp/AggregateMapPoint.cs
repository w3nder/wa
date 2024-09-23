// Decompiled with JetBrains decompiler
// Type: WhatsApp.AggregateMapPoint
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Windows;


namespace WhatsApp
{
  public class AggregateMapPoint : MapPoint
  {
    private List<MapPoint> list;

    public AggregateMapPoint(List<MapPoint> list) => this.list = list;

    public GeoCoordinate Position()
    {
      return this.list.Select<MapPoint, GeoCoordinate>((Func<MapPoint, GeoCoordinate>) (point => point.Position())).FirstOrDefault<GeoCoordinate>();
    }

    public UIElement Element() => (UIElement) null;

    public void SetCoordinate(GeoCoordinate loc)
    {
      this.list.ForEach((Action<MapPoint>) (pt => pt.SetCoordinate(loc)));
    }

    public void Remove() => this.list.ForEach((Action<MapPoint>) (pt => pt.Remove()));
  }
}
