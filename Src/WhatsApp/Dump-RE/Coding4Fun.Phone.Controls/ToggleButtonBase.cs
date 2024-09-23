// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ToggleButtonBase
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public abstract class ToggleButtonBase : CheckBox, IButtonBase
  {
    protected ImageBrush OpacityImageBrush;
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (ToggleButtonBase), new PropertyMetadata((object) (Orientation) 0));
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof (Stretch), typeof (Stretch), typeof (ToggleButtonBase), new PropertyMetadata((object) (Stretch) 1, new PropertyChangedCallback(ToggleButtonBase.OnStretch)));
    public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof (ImageSource), typeof (ImageSource), typeof (ToggleButtonBase), new PropertyMetadata(new PropertyChangedCallback(ToggleButtonBase.OnImageSource)));
    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(nameof (ButtonWidth), typeof (double), typeof (ToggleButtonBase), new PropertyMetadata((object) 72.0));
    public static readonly DependencyProperty ButtonHeightProperty = DependencyProperty.Register(nameof (ButtonHeight), typeof (double), typeof (ToggleButtonBase), new PropertyMetadata((object) 72.0));

    public virtual void OnApplyTemplate()
    {
      ((ToggleButton) this).OnApplyTemplate();
      this.OpacityImageBrush = ((Control) this).GetTemplateChild("OpacityImageBrush") as ImageBrush;
      ButtonBaseHelper.ApplyTemplate((FrameworkElement) this, this.OpacityImageBrush, ((Control) this).GetTemplateChild("ContentBody") as ContentControl, this.Stretch, ToggleButtonBase.ImageSourceProperty);
    }

    public Orientation Orientation
    {
      get => (Orientation) ((DependencyObject) this).GetValue(ToggleButtonBase.OrientationProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToggleButtonBase.OrientationProperty, (object) value);
      }
    }

    public Stretch Stretch
    {
      get => (Stretch) ((DependencyObject) this).GetValue(ToggleButtonBase.StretchProperty);
      set => ((DependencyObject) this).SetValue(ToggleButtonBase.StretchProperty, (object) value);
    }

    public ImageSource ImageSource
    {
      get => (ImageSource) ((DependencyObject) this).GetValue(ToggleButtonBase.ImageSourceProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToggleButtonBase.ImageSourceProperty, (object) value);
      }
    }

    public double ButtonWidth
    {
      get => (double) ((DependencyObject) this).GetValue(ToggleButtonBase.ButtonWidthProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToggleButtonBase.ButtonWidthProperty, (object) value);
      }
    }

    public double ButtonHeight
    {
      get => (double) ((DependencyObject) this).GetValue(ToggleButtonBase.ButtonHeightProperty);
      set
      {
        ((DependencyObject) this).SetValue(ToggleButtonBase.ButtonHeightProperty, (object) value);
      }
    }

    private static void OnImageSource(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is ToggleButtonBase toggleButtonBase))
        return;
      ButtonBaseHelper.OnImageChange(e, toggleButtonBase.OpacityImageBrush);
    }

    private static void OnStretch(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      if (!(o is ToggleButtonBase toggleButtonBase))
        return;
      ButtonBaseHelper.OnStretch(e, toggleButtonBase.OpacityImageBrush);
    }
  }
}
