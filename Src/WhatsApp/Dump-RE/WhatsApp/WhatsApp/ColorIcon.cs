// Decompiled with JetBrains decompiler
// Type: WhatsApp.ColorIcon
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class ColorIcon : Grid
  {
    private ImageBrush imgBrush;
    private Rectangle rect;
    private Ellipse background;
    private double iconWidth;
    private double iconHeight;

    public System.Windows.Media.ImageSource Source
    {
      set => this.imgBrush.SetImageSourceSafe(value);
    }

    public Brush IconForeground
    {
      set => this.rect.Fill = value;
    }

    public Brush IconBackground
    {
      set
      {
        if (this.background == null)
        {
          this.background = new Ellipse();
          this.Children.Insert(0, (UIElement) this.background);
        }
        this.background.Fill = value;
      }
    }

    public double IconWidth
    {
      get => this.iconWidth;
      set
      {
        this.iconWidth = value;
        this.rect.Width = this.iconWidth;
      }
    }

    public double IconHeight
    {
      get => this.iconHeight;
      set
      {
        this.iconHeight = value;
        this.rect.Height = this.iconHeight;
      }
    }

    public ColorIcon()
    {
      ImageBrush imageBrush = new ImageBrush();
      imageBrush.Stretch = Stretch.UniformToFill;
      this.imgBrush = imageBrush;
      Rectangle rectangle = new Rectangle();
      rectangle.Fill = (Brush) UIUtils.ForegroundBrush;
      rectangle.OpacityMask = (Brush) this.imgBrush;
      this.rect = rectangle;
      this.Children.Add((UIElement) this.rect);
      this.SizeChanged += (SizeChangedEventHandler) ((sender, e) =>
      {
        if (this.rect == null)
          return;
        this.rect.Width = this.IconWidth;
        this.rect.Height = this.IconHeight;
      });
    }
  }
}
