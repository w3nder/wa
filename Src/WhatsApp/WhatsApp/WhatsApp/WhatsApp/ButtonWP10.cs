// Decompiled with JetBrains decompiler
// Type: WhatsApp.ButtonWP10
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class ButtonWP10 : UserControl
  {
    private double buttonSize = 48.0;
    public Brush ButtonBrush = (Brush) UIUtils.WhiteBrush;
    internal Grid ButtonContent;
    internal Ellipse ButtonBackground;
    internal Ellipse ButtonForeground;
    internal ImageBrush ButtonIcon;
    private bool _contentLoaded;

    public event EventHandler Click;

    public System.Windows.Media.ImageSource ButtonIconImage
    {
      set
      {
        if (this.ButtonIcon == null)
          return;
        this.ButtonIcon.SetImageSourceSafe(value);
      }
    }

    public double ButtonSize
    {
      get => this.buttonSize;
      set
      {
        this.buttonSize = value;
        this.UpdateButtonSize();
      }
    }

    public ButtonWP10()
    {
      this.InitializeComponent();
      this.UpdateButtonSize();
      this.ButtonForeground.Fill = this.ButtonBrush;
    }

    private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.ButtonForeground.Fill = this.ButtonBrush;
    }

    private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.ButtonForeground.Fill = (Brush) UIUtils.BlackBrush;
    }

    private void Button_Click(object sender, EventArgs e)
    {
      if (this.Click == null)
        return;
      this.Click(sender, e);
    }

    private void UpdateButtonSize()
    {
      this.ButtonContent.Width = this.ButtonContent.Height = this.ButtonSize;
      this.ButtonForeground.Height = this.ButtonForeground.Width = this.ButtonSize;
      this.ButtonBackground.Width = this.ButtonBackground.Height = this.ButtonSize;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ButtonWP10.xaml", UriKind.Relative));
      this.ButtonContent = (Grid) this.FindName("ButtonContent");
      this.ButtonBackground = (Ellipse) this.FindName("ButtonBackground");
      this.ButtonForeground = (Ellipse) this.FindName("ButtonForeground");
      this.ButtonIcon = (ImageBrush) this.FindName("ButtonIcon");
    }
  }
}
