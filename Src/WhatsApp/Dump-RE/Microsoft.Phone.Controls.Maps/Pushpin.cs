// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Maps.Pushpin
// Assembly: Microsoft.Phone.Controls.Maps, Version=8.0.0.0, Culture=neutral, PublicKeyToken=24eec0d8c86cda1e
// MVID: D3F696B0-0EFB-48F8-969B-E5D31AB1A74E
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Maps.dll

using Microsoft.Phone.Controls.Maps.AutomationPeers;
using Microsoft.Phone.Controls.Maps.Design;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

#nullable disable
namespace Microsoft.Phone.Controls.Maps
{
  [System.Windows.Markup.ContentProperty("Content")]
  [Obsolete("This class has been deprecated.  Use Microsoft.Phone.Maps.dll instead.")]
  public sealed class Pushpin : ContentControl
  {
    public static readonly DependencyProperty LocationDependencyProperty = DependencyProperty.Register(nameof (Location), typeof (GeoCoordinate), typeof (Pushpin), new PropertyMetadata((object) new GeoCoordinate(0.0, 0.0), new PropertyChangedCallback(Pushpin.OnLocationChangedCallback)));
    public static readonly DependencyProperty PositionOriginDependencyProperty = DependencyProperty.Register(nameof (PositionOrigin), typeof (PositionOrigin), typeof (Pushpin), new PropertyMetadata(new PropertyChangedCallback(Pushpin.OnPositionOriginChangedCallback)));

    private static void OnLocationChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      d.SetValue(MapLayer.PositionProperty, (object) (GeoCoordinate) eventArgs.NewValue);
    }

    private static void OnPositionOriginChangedCallback(
      DependencyObject d,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      d.SetValue(MapLayer.PositionOriginProperty, (object) (PositionOrigin) eventArgs.NewValue);
    }

    public Pushpin() => this.DefaultStyleKey = (object) typeof (Pushpin);

    [TypeConverter(typeof (LocationConverter))]
    public GeoCoordinate Location
    {
      get => (GeoCoordinate) this.GetValue(Pushpin.LocationDependencyProperty);
      set => this.SetValue(Pushpin.LocationDependencyProperty, (object) value);
    }

    public PositionOrigin PositionOrigin
    {
      get => (PositionOrigin) this.GetValue(Pushpin.PositionOriginDependencyProperty);
      set => this.SetValue(Pushpin.PositionOriginDependencyProperty, (object) value);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
      return (AutomationPeer) new BaseAutomationPeer((FrameworkElement) this, nameof (Pushpin));
    }
  }
}
