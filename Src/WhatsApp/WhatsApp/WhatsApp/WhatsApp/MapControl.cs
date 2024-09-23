// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class MapControl : UserControl
  {
    private MapImplementation impl;
    private IDisposable memoryWorkaroundSub;
    internal Grid ContentRoot;
    private bool _contentLoaded;

    public MapControl()
    {
      Log.d(nameof (MapControl), "ctor");
      if (this.impl == null)
        this.impl = (MapImplementation) new Wp8Map();
      this.InitializeComponent();
      this.ContentRoot.Children.Add(this.impl.Element);
    }

    public MapPoint AddPoint(MapPointStyle style = MapPointStyle.Default, string label = null)
    {
      return this.impl.AddPoint(style, label);
    }

    public GeoCoordinate Center
    {
      get => this.impl.Center;
      set => this.impl.Center = value;
    }

    public double ZoomLevel
    {
      get => this.impl.ZoomLevel;
      set => this.impl.ZoomLevel = value;
    }

    public MapMode CartographicMode
    {
      get => this.impl.CartographicMode;
      set => this.impl.CartographicMode = value;
    }

    public void SetView(GeoCoordinate center, double widthInDegrees, double heightInDegrees)
    {
      this.impl.SetView(center, widthInDegrees, heightInDegrees);
    }

    public double ShownAreaRadius() => this.impl.ShownAreaRadius();

    public IObservable<EventArgs> ViewChanged() => this.impl.ViewChanged();

    public void DisposeAll()
    {
      this.memoryWorkaroundSub.SafeDispose();
      this.memoryWorkaroundSub = (IDisposable) null;
    }

    public void ApplyMemoryWorkaround(PhoneApplicationPage page)
    {
      this.memoryWorkaroundSub.SafeDispose();
      this.memoryWorkaroundSub = this.impl.ApplyMemoryWorkaround(page);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MapControl.xaml", UriKind.Relative));
      this.ContentRoot = (Grid) this.FindName("ContentRoot");
    }
  }
}
