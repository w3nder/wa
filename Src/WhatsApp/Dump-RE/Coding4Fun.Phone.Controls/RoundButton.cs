// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.RoundButton
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class RoundButton : Button, IButtonBase
  {
    protected ImageBrush OpacityImageBrush;
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (RoundButton), new PropertyMetadata((object) (Orientation) 0));
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof (Stretch), typeof (Stretch), typeof (RoundButton), new PropertyMetadata((object) (Stretch) 1, new PropertyChangedCallback(RoundButton.OnStretch)));
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof (ImageSource), typeof (ImageSource), typeof (RoundButton), new PropertyMetadata(new PropertyChangedCallback(RoundButton.OnImageSource)));
    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof (ButtonWidth), typeof (double), typeof (RoundButton), new PropertyMetadata((object) 72.0));
    public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.Register(nameof (ButtonHeight), typeof (double), typeof (RoundButton), new PropertyMetadata((object) 72.0));

    public RoundButton() => ((Control) this).DefaultStyleKey = (object) typeof (RoundButton);

    public virtual void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      this.OpacityImageBrush = ((Control) this).GetTemplateChild("OpacityImageBrush") as ImageBrush;
      ButtonBaseHelper.ApplyTemplate((FrameworkElement) this, this.OpacityImageBrush, ((Control) this).GetTemplateChild("ContentBody") as ContentControl, this.Stretch, RoundButton.ImageSourceProperty);
    }

    public Orientation Orientation
    {
      get => (Orientation) ((DependencyObject) this).GetValue(RoundButton.OrientationProperty);
      set => ((DependencyObject) this).SetValue(RoundButton.OrientationProperty, (object) value);
    }

    public Stretch Stretch
    {
      get => (Stretch) ((DependencyObject) this).GetValue(RoundButton.StretchProperty);
      set => ((DependencyObject) this).SetValue(RoundButton.StretchProperty, (object) value);
    }

    public ImageSource ImageSource
    {
      get => (ImageSource) ((DependencyObject) this).GetValue(RoundButton.ImageSourceProperty);
      set => ((DependencyObject) this).SetValue(RoundButton.ImageSourceProperty, (object) value);
    }

    public double ButtonWidth
    {
      get => (double) ((DependencyObject) this).GetValue(RoundButton.ButtonWidthProperty);
      set => ((DependencyObject) this).SetValue(RoundButton.ButtonWidthProperty, (object) value);
    }

    public double ButtonHeight
    {
      get => (double) ((DependencyObject) this).GetValue(RoundButton.ButtonHeightProperty);
      set => ((DependencyObject) this).SetValue(RoundButton.ButtonHeightProperty, (object) value);
    }

    private static void OnImageSource(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is RoundButton roundButton))
        return;
      ButtonBaseHelper.OnImageChange(e, roundButton.OpacityImageBrush);
    }

    private static void OnStretch(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is RoundButton roundButton))
        return;
      ButtonBaseHelper.OnStretch(e, roundButton.OpacityImageBrush);
    }
  }
}
