// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Maps.Toolkit.MapChildControl
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Maps.Controls;
using System.ComponentModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Maps.Toolkit
{
  [System.Windows.Markup.ContentProperty("Content")]
  public class MapChildControl : ContentControl
  {
    public static readonly DependencyProperty GeoCoordinateProperty = DependencyProperty.Register(nameof (GeoCoordinate), typeof (GeoCoordinate), typeof (MapChildControl), new PropertyMetadata(new PropertyChangedCallback(MapChildControl.OnGeoCoordinateChangedCallback)));
    public static readonly DependencyProperty PositionOriginProperty = DependencyProperty.Register(nameof (PositionOrigin), typeof (Point), typeof (MapChildControl), new PropertyMetadata(new PropertyChangedCallback(MapChildControl.OnPositionOriginChangedCallback)));

    [TypeConverter(typeof (GeoCoordinateConverter))]
    public GeoCoordinate GeoCoordinate
    {
      get => (GeoCoordinate) this.GetValue(MapChildControl.GeoCoordinateProperty);
      set => this.SetValue(MapChildControl.GeoCoordinateProperty, (object) value);
    }

    public Point PositionOrigin
    {
      get => (Point) this.GetValue(MapChildControl.PositionOriginProperty);
      set => this.SetValue(MapChildControl.PositionOriginProperty, (object) value);
    }

    private static void OnGeoCoordinateChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      d.SetValue(MapChild.GeoCoordinateProperty, e.NewValue);
    }

    private static void OnPositionOriginChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      d.SetValue(MapChild.PositionOriginProperty, e.NewValue);
    }
  }
}
