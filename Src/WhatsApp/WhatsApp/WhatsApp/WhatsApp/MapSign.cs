// Decompiled with JetBrains decompiler
// Type: WhatsApp.MapSign
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class MapSign : UserControl
  {
    internal Storyboard ExpandPin;
    internal Storyboard CollapsePin;
    internal Grid SignPost;
    internal PlaneProjection SignProjection;
    internal Polygon Post;
    internal Grid Sign;
    internal TextBlock Caption;
    internal TextBlock Subtitle;
    private bool _contentLoaded;

    public MapSign() => this.InitializeComponent();

    private void MapSign_LayoutUpdated(object sender, EventArgs e)
    {
      this.SignPost.RenderTransform = (Transform) new CompositeTransform()
      {
        TranslateX = (-this.ActualWidth / 2.0),
        TranslateY = (-this.ActualHeight + 5.0)
      };
    }

    public string Title
    {
      get => this.Caption.Text;
      set => this.Caption.Text = value;
    }

    public string SubTitle
    {
      get => this.Subtitle.Text;
      set => this.Subtitle.Text = value;
    }

    public void Show()
    {
      this.Visibility = Visibility.Visible;
      Storyboarder.Perform(this.ExpandPin, onComplete: (Action) (() => this.SignProjection.RotationX = 0.0));
    }

    public void Hide(Action endingAction)
    {
      Storyboarder.Perform(this.CollapsePin, onComplete: (Action) (() =>
      {
        this.SignProjection.RotationX = 89.5;
        this.Visibility = Visibility.Collapsed;
        if (endingAction == null)
          return;
        endingAction();
      }));
    }

    public void Dim()
    {
      this.Caption.Opacity = 0.3;
      this.Subtitle.Opacity = 0.3;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/MapSign.xaml", UriKind.Relative));
      this.ExpandPin = (Storyboard) this.FindName("ExpandPin");
      this.CollapsePin = (Storyboard) this.FindName("CollapsePin");
      this.SignPost = (Grid) this.FindName("SignPost");
      this.SignProjection = (PlaneProjection) this.FindName("SignProjection");
      this.Post = (Polygon) this.FindName("Post");
      this.Sign = (Grid) this.FindName("Sign");
      this.Caption = (TextBlock) this.FindName("Caption");
      this.Subtitle = (TextBlock) this.FindName("Subtitle");
    }
  }
}
