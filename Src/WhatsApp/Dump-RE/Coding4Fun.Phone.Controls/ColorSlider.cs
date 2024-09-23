// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ColorSlider
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Coding4Fun.Phone.Controls.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ColorSlider : ColorBaseControl
  {
    private const double HueSelectorSize = 24.0;
    private const string BodyName = "Body";
    private const string SelectedColorName = "SelectedColor";
    private const string SliderName = "Slider";
    private bool _fromSliderChange;
    protected Grid Body;
    protected Rectangle SelectedColor;
    protected SuperSlider Slider;
    public static readonly DependencyProperty ThumbProperty = DependencyProperty.Register(nameof (Thumb), typeof (object), typeof (ColorSlider), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsColorVisibleProperty = DependencyProperty.Register(nameof (IsColorVisible), typeof (bool), typeof (ColorSlider), new PropertyMetadata((object) true, new PropertyChangedCallback(ColorSlider.OnIsColorVisibleChanged)));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof (Orientation), typeof (Orientation), typeof (ColorSlider), new PropertyMetadata((object) (Orientation) 0, new PropertyChangedCallback(ColorSlider.OnOrientationPropertyChanged)));

    public ColorSlider()
    {
      this.DefaultStyleKey = (object) typeof (ColorSlider);
      this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.SuperSlider_IsEnabledChanged);
    }

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this.Body = this.GetTemplateChild("Body") as Grid;
      this.Slider = this.GetTemplateChild("Slider") as SuperSlider;
      if (this.Thumb == null)
        this.Thumb = (object) new ColorSliderThumb();
      this.SelectedColor = this.GetTemplateChild("SelectedColor") as Rectangle;
      ((FrameworkElement) this).SizeChanged += new SizeChangedEventHandler(this.UserControl_SizeChanged);
      if (this.Slider != null)
      {
        this.Slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
        if (this.Color.A == (byte) 0 && this.Color.R == (byte) 0 && this.Color.G == (byte) 0 && this.Color.B == (byte) 0)
          this.Color = Color.FromArgb(byte.MaxValue, (byte) 6, byte.MaxValue, (byte) 0);
        else
          this.UpdateLayoutBasedOnColor();
      }
      this.IsEnabledVisualStateUpdate();
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.SetColorFromSlider(e.NewValue);
    }

    private void SetColorFromSlider(double value)
    {
      this._fromSliderChange = true;
      this.ColorChanging(ColorSpace.GetColorFromHueValue((float) ((int) value % 360)));
      this._fromSliderChange = false;
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.AdjustLayoutBasedOnOrientation();
    }

    private void SuperSlider_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.IsEnabledVisualStateUpdate();
    }

    public object Thumb
    {
      get => ((DependencyObject) this).GetValue(ColorSlider.ThumbProperty);
      set => ((DependencyObject) this).SetValue(ColorSlider.ThumbProperty, value);
    }

    public bool IsColorVisible
    {
      get => (bool) ((DependencyObject) this).GetValue(ColorSlider.IsColorVisibleProperty);
      set => ((DependencyObject) this).SetValue(ColorSlider.IsColorVisibleProperty, (object) value);
    }

    public Orientation Orientation
    {
      get => (Orientation) ((DependencyObject) this).GetValue(ColorSlider.OrientationProperty);
      set => ((DependencyObject) this).SetValue(ColorSlider.OrientationProperty, (object) value);
    }

    private static void OnIsColorVisibleChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ColorSlider colorSlider))
        return;
      colorSlider.AdjustLayoutBasedOnOrientation();
    }

    private static void OnOrientationPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ColorSlider colorSlider))
        return;
      colorSlider.AdjustLayoutBasedOnOrientation();
    }

    private void AdjustLayoutBasedOnOrientation()
    {
      if (this.Body == null || this.Slider == null || this.SelectedColor == null)
        return;
      bool flag = this.Orientation == 0;
      this.IsEnabledVisualStateUpdate();
      ((PresentationFrameworkCollection<RowDefinition>) this.Body.RowDefinitions).Clear();
      ((PresentationFrameworkCollection<ColumnDefinition>) this.Body.ColumnDefinitions).Clear();
      if (flag)
      {
        ((PresentationFrameworkCollection<RowDefinition>) this.Body.RowDefinitions).Add(new RowDefinition());
        ((PresentationFrameworkCollection<RowDefinition>) this.Body.RowDefinitions).Add(new RowDefinition());
      }
      else
      {
        ((PresentationFrameworkCollection<ColumnDefinition>) this.Body.ColumnDefinitions).Add(new ColumnDefinition());
        ((PresentationFrameworkCollection<ColumnDefinition>) this.Body.ColumnDefinitions).Add(new ColumnDefinition());
      }
      FrameworkElement thumb = (FrameworkElement) this.Slider.Thumb;
      if (thumb != null)
      {
        thumb.Height = flag ? 24.0 : double.NaN;
        thumb.Width = flag ? double.NaN : 24.0;
      }
      ((DependencyObject) this.SelectedColor).SetValue(Grid.RowProperty, (object) (flag ? 1 : 0));
      ((DependencyObject) this.SelectedColor).SetValue(Grid.ColumnProperty, (object) (flag ? 0 : 1));
      double actualWidth = ((FrameworkElement) this.Slider).ActualWidth;
      double actualHeight = ((FrameworkElement) this.Slider).ActualHeight;
      ((FrameworkElement) this.SelectedColor).Height = ((FrameworkElement) this.SelectedColor).Width = flag ? actualWidth : actualHeight;
      if (flag)
      {
        ((PresentationFrameworkCollection<RowDefinition>) this.Body.RowDefinitions)[0].Height = new GridLength(1.0, (GridUnitType) 2);
        ((PresentationFrameworkCollection<RowDefinition>) this.Body.RowDefinitions)[1].Height = new GridLength(1.0, (GridUnitType) 0);
      }
      else
      {
        ((PresentationFrameworkCollection<ColumnDefinition>) this.Body.ColumnDefinitions)[0].Width = new GridLength(1.0, (GridUnitType) 2);
        ((PresentationFrameworkCollection<ColumnDefinition>) this.Body.ColumnDefinitions)[1].Width = new GridLength(1.0, (GridUnitType) 0);
      }
      ((UIElement) this.SelectedColor).Visibility = this.IsColorVisible ? (Visibility) 0 : (Visibility) 1;
    }

    protected internal override void UpdateLayoutBasedOnColor()
    {
      if (this._fromSliderChange)
        return;
      base.UpdateLayoutBasedOnColor();
      if (this.Slider == null)
        return;
      this.Slider.Value = (double) this.Color.GetHue();
    }

    private void IsEnabledVisualStateUpdate()
    {
      VisualStateManager.GoToState((Control) this, this.IsEnabled ? "Normal" : "Disabled", true);
      this.Slider.Background = this.IsEnabled ? (Brush) ColorSpace.GetColorGradientBrush(this.Orientation) : (Brush) ColorSpace.GetBlackAndWhiteGradientBrush(this.Orientation);
    }
  }
}
