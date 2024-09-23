// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapDraggablePin
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class MapDraggablePin : UserControl
  {
    internal Storyboard Land;
    internal Storyboard Takeoff;
    internal Grid Drop;
    internal CompositeTransform MoveDrop;
    internal Grid Sign;
    internal ScaleTransform ClipScale;
    internal Polygon Post;
    internal TextBlock Caption;
    internal TextBlock Subtitle;
    private bool _contentLoaded;

    public MapDraggablePin() => this.InitializeComponent();

    public void Reset()
    {
      this.Takeoff.Stop();
      this.Land.Stop();
      this.Drop.Opacity = 1.0;
      this.MoveDrop.TranslateY = -40.0;
      this.Subtitle.Text = "";
      this.ClipScale.ScaleX = 0.0;
      this.ClipScale.ScaleY = 0.0;
      this.Sign.Opacity = 0.0;
    }

    public void AttachPin()
    {
      this.Takeoff.Stop();
      Storyboarder.Perform(this.Land, false);
    }

    public void DetachPin()
    {
      this.Land.Stop();
      Storyboarder.Perform(this.Takeoff, false);
    }

    public event EventHandler PinTap;

    private void Grid_Tap(object sender, GestureEventArgs e)
    {
      if (this.PinTap == null)
        return;
      this.PinTap(sender, (EventArgs) e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MapDraggablePin.xaml", UriKind.Relative));
      this.Land = (Storyboard) this.FindName("Land");
      this.Takeoff = (Storyboard) this.FindName("Takeoff");
      this.Drop = (Grid) this.FindName("Drop");
      this.MoveDrop = (CompositeTransform) this.FindName("MoveDrop");
      this.Sign = (Grid) this.FindName("Sign");
      this.ClipScale = (ScaleTransform) this.FindName("ClipScale");
      this.Post = (Polygon) this.FindName("Post");
      this.Caption = (TextBlock) this.FindName("Caption");
      this.Subtitle = (TextBlock) this.FindName("Subtitle");
    }
  }
}
