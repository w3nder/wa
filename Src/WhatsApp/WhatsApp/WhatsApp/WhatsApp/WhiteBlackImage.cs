// Decompiled with JetBrains decompiler
// Type: WhatsApp.WhiteBlackImage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class WhiteBlackImage : UserControl
  {
    internal Rectangle rect;
    internal ImageBrush image;
    private bool _contentLoaded;

    public WhiteBlackImage()
    {
      this.InitializeComponent();
      this.rect.Fill = (Brush) new SolidColorBrush(ImageStore.IsDarkTheme() ? Colors.White : Colors.Black);
    }

    public new Brush Foreground
    {
      get => this.rect == null ? (Brush) null : this.rect.Fill;
      set
      {
        if (!(value is SolidColorBrush solidColorBrush) || this.rect == null)
          return;
        this.rect.Fill = (Brush) solidColorBrush;
      }
    }

    public BitmapImage Image
    {
      get => this.image.ImageSource as BitmapImage;
      set
      {
        this.image.SetImageSourceSafe((System.Windows.Media.ImageSource) value);
        this.rect.Width = (double) value.PixelWidth;
        this.rect.Height = (double) value.PixelHeight;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/WhiteBlackImage.xaml", UriKind.Relative));
      this.rect = (Rectangle) this.FindName("rect");
      this.image = (ImageBrush) this.FindName("image");
    }
  }
}
