// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapPlacePin
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace WhatsApp
{
  public class MapPlacePin : UserControl
  {
    internal Storyboard ExpandPin;
    internal Storyboard CollapsePin;
    internal Grid LayoutRoot;
    internal Image Shadow;
    internal Grid Panel;
    internal TranslateTransform MovePanel;
    internal PlaneProjection ProjectPanel;
    internal Image PinImage;
    internal Image Graphic;
    private bool _contentLoaded;

    public MapPlacePin() => this.InitializeComponent();

    private void MapPlacePin_LayoutUpdated(object sender, EventArgs e)
    {
      this.LayoutRoot.RenderTransform = (Transform) new CompositeTransform()
      {
        TranslateX = (-this.ActualWidth / 2.0),
        TranslateY = (-this.ActualHeight + this.Shadow.ActualHeight / 2.0)
      };
    }

    public void Collapse(Action a = null)
    {
      Storyboarder.Perform(this.CollapsePin, onComplete: (Action) (() =>
      {
        this.ProjectPanel.RotationX = 90.0;
        this.Panel.Visibility = Visibility.Collapsed;
        this.Shadow.Opacity = 0.0;
        if (a == null)
          return;
        a();
      }));
    }

    public void Expand()
    {
      this.Panel.Visibility = Visibility.Visible;
      Storyboarder.Perform(this.ExpandPin, onComplete: (Action) (() =>
      {
        this.ProjectPanel.RotationX = 0.0;
        this.Shadow.Opacity = 1.0;
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MapPlacePin.xaml", UriKind.Relative));
      this.ExpandPin = (Storyboard) this.FindName("ExpandPin");
      this.CollapsePin = (Storyboard) this.FindName("CollapsePin");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Shadow = (Image) this.FindName("Shadow");
      this.Panel = (Grid) this.FindName("Panel");
      this.MovePanel = (TranslateTransform) this.FindName("MovePanel");
      this.ProjectPanel = (PlaneProjection) this.FindName("ProjectPanel");
      this.PinImage = (Image) this.FindName("PinImage");
      this.Graphic = (Image) this.FindName("Graphic");
    }
  }
}
