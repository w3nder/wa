// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapPoint
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Device.Location;
using System.Windows;


namespace WhatsApp
{
  public interface MapPoint
  {
    void SetCoordinate(GeoCoordinate loc);

    GeoCoordinate Position();

    UIElement Element();

    void Remove();
  }
}
