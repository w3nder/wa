// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ColorBaseControl
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public abstract class ColorBaseControl : Control
  {
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof (Color), typeof (Color), typeof (ColorBaseControl), new PropertyMetadata(new PropertyChangedCallback(ColorBaseControl.OnColorChanged)));
    public static readonly DependencyProperty SolidColorBrushProperty = DependencyProperty.Register(nameof (SolidColorBrush), typeof (SolidColorBrush), typeof (ColorBaseControl), new PropertyMetadata((PropertyChangedCallback) null));

    public event ColorBaseControl.ColorChangedHandler ColorChanged;

    public Color Color
    {
      get => (Color) ((DependencyObject) this).GetValue(ColorBaseControl.ColorProperty);
      set => ((DependencyObject) this).SetValue(ColorBaseControl.ColorProperty, (object) value);
    }

    public SolidColorBrush SolidColorBrush
    {
      get
      {
        return (SolidColorBrush) ((DependencyObject) this).GetValue(ColorBaseControl.SolidColorBrushProperty);
      }
      private set
      {
        ((DependencyObject) this).SetValue(ColorBaseControl.SolidColorBrushProperty, (object) value);
      }
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ColorBaseControl colorBaseControl))
        return;
      colorBaseControl.UpdateLayoutBasedOnColor();
      colorBaseControl.SolidColorBrush = new SolidColorBrush((Color) e.NewValue);
    }

    protected internal virtual void UpdateLayoutBasedOnColor()
    {
    }

    protected internal void ColorChanging(Color color)
    {
      this.Color = color;
      this.SolidColorBrush = new SolidColorBrush(this.Color);
      if (this.ColorChanged == null)
        return;
      this.ColorChanged((object) this, this.Color);
    }

    public delegate void ColorChangedHandler(object sender, Color color);
  }
}
