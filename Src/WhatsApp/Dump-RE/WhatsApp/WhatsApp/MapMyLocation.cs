// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapMyLocation
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class MapMyLocation : UserControl
  {
    private bool _isAccurate;
    private double _accuracyRadius;
    internal Grid LayoutRoot;
    internal Ellipse AccuracyEllipse;
    internal Ellipse Inner;
    private bool _contentLoaded;

    public MapMyLocation() => this.InitializeComponent();

    private void MapMyLocation_LayoutUpdated(object sender, EventArgs e)
    {
      this.LayoutRoot.RenderTransform = (Transform) new CompositeTransform()
      {
        TranslateX = (-this.ActualWidth / 2.0),
        TranslateY = (-this.ActualHeight / 2.0)
      };
    }

    public bool IsAccurate
    {
      get => this._isAccurate;
      set
      {
        this._isAccurate = value;
        this.Inner.Fill = value ? (Brush) this.Resources[(object) "AccurateFill"] : (Brush) this.Resources[(object) "InaccurateFill"];
        this.AccuracyEllipse.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public double AccuracyRadius
    {
      get => this._accuracyRadius;
      set
      {
        this._accuracyRadius = value;
        double num = Math.Min(800.0, value * 2.0);
        this.AccuracyEllipse.Width = num;
        this.AccuracyEllipse.Height = num;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MapMyLocation.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.AccuracyEllipse = (Ellipse) this.FindName("AccuracyEllipse");
      this.Inner = (Ellipse) this.FindName("Inner");
    }
  }
}
