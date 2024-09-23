// Decompiled with JetBrains decompiler
// Type: Coding4Fun.Phone.Controls.ColorPicker
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Coding4Fun.Phone.Controls.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

#nullable disable
namespace Coding4Fun.Phone.Controls
{
  public class ColorPicker : ColorBaseControl
  {
    private const string SampleSelectorName = "SampleSelector";
    private const string SelectedHueColorName = "SelectedHueColor";
    private const string ColorSliderName = "ColorSlider";
    private const string BodyName = "Body";
    private bool _fromMovement;
    private bool _adjustingColor;
    private Point _position;
    protected Grid SampleSelector;
    protected Rectangle SelectedHueColor;
    protected ColorSlider ColorSlider;
    private MovementMonitor _monitor;
    public static readonly DependencyProperty ThumbProperty = DependencyProperty.Register(nameof (Thumb), typeof (object), typeof (ColorPicker), new PropertyMetadata((PropertyChangedCallback) null));

    public ColorPicker()
    {
      this.DefaultStyleKey = (object) typeof (ColorPicker);
      ((FrameworkElement) this).SizeChanged += new SizeChangedEventHandler(this.ColorPicker_SizeChanged);
      this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.ColorSlider_IsEnabledChanged);
      ((FrameworkElement) this).Loaded += new RoutedEventHandler(this.ColorPicker_Loaded);
    }

    private void ColorPicker_Loaded(object sender, RoutedEventArgs e)
    {
      this.IsEnabledVisualStateUpdate();
    }

    public virtual void OnApplyTemplate()
    {
      ((FrameworkElement) this).OnApplyTemplate();
      this.SampleSelector = this.GetTemplateChild("SampleSelector") as Grid;
      this.SelectedHueColor = this.GetTemplateChild("SelectedHueColor") as Rectangle;
      if (this.GetTemplateChild("Body") is Grid templateChild)
      {
        this._monitor = new MovementMonitor();
        this._monitor.Movement += new EventHandler<MovementMonitorEventArgs>(this._monitor_Movement);
        this._monitor.MonitorControl((Panel) templateChild);
      }
      this.ColorSlider = this.GetTemplateChild("ColorSlider") as ColorSlider;
      if (this.ColorSlider == null)
        return;
      if (this.Thumb == null)
        this.Thumb = (object) new ColorSliderThumb();
      this.ColorSlider.ColorChanged += new ColorBaseControl.ColorChangedHandler(this.ColorSlider_ColorChanged);
      if (this.SelectedHueColor == null)
        return;
      Binding binding = new Binding()
      {
        Source = (object) this.ColorSlider,
        Path = new PropertyPath("SolidColorBrush", new object[0])
      };
      ((FrameworkElement) this.SelectedHueColor).SetBinding(Shape.FillProperty, binding);
    }

    private void ColorSlider_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      this.IsEnabledVisualStateUpdate();
    }

    private void ColorSlider_ColorChanged(object sender, Color color) => this.UpdateSample();

    private void ColorPicker_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.Color.A == (byte) 0 && this.Color.R == (byte) 0 && this.Color.G == (byte) 0 && this.Color.B == (byte) 0)
        this.Color = this.ColorSlider.Color;
      else
        this.UpdateLayoutBasedOnColor();
    }

    private void _monitor_Movement(object sender, MovementMonitorEventArgs e)
    {
      this._position.X = e.X;
      this._position.Y = e.Y;
      this.UpdateSample();
    }

    private void UpdateSample()
    {
      this._fromMovement = true;
      this.SetSampleLocation();
      float saturation = (float) (this._position.X / ((FrameworkElement) this.SelectedHueColor).ActualWidth);
      float num = (float) (1.0 - this._position.Y / ((FrameworkElement) this.SelectedHueColor).ActualHeight);
      if (!this._adjustingColor)
        this.ColorChanging(ColorSpace.ConvertHsvToRgb(this.ColorSlider.Color.GetHue(), saturation, num));
      this._fromMovement = false;
    }

    private void SetSampleLocation()
    {
      double actualHeight1 = ((FrameworkElement) this.SampleSelector).ActualHeight;
      double actualHeight2 = ((FrameworkElement) this.SelectedHueColor).ActualHeight;
      double actualWidth = ((FrameworkElement) this.SelectedHueColor).ActualWidth;
      this._position.X = ControlHelper.CheckBound(this._position.X, actualWidth);
      this._position.Y = ControlHelper.CheckBound(this._position.Y, actualHeight2);
      double num1 = this._position.X - actualHeight1;
      double num2 = this._position.Y - actualHeight1;
      ((FrameworkElement) this.SampleSelector).Margin = new Thickness(ControlHelper.CheckBound(num1, actualWidth), ControlHelper.CheckBound(num2, actualHeight2), 0.0, 0.0);
    }

    protected internal override void UpdateLayoutBasedOnColor()
    {
      if (this._fromMovement || this.SelectedHueColor == null)
        return;
      base.UpdateLayoutBasedOnColor();
      HSV hsv = this.Color.GetHSV();
      if (this.ColorSlider != null)
      {
        this._adjustingColor = true;
        this.ColorSlider.Color = ColorSpace.GetColorFromHueValue((float) (int) hsv.Hue);
        this._adjustingColor = false;
      }
      this._position.X = (double) hsv.Saturation * ((FrameworkElement) this.SelectedHueColor).ActualWidth;
      this._position.Y = (1.0 - (double) hsv.Value) * ((FrameworkElement) this.SelectedHueColor).ActualHeight;
      this.SetSampleLocation();
    }

    private void IsEnabledVisualStateUpdate()
    {
      VisualStateManager.GoToState((Control) this, this.IsEnabled ? "Normal" : "Disabled", true);
    }

    public object Thumb
    {
      get => ((DependencyObject) this).GetValue(ColorPicker.ThumbProperty);
      set => ((DependencyObject) this).SetValue(ColorPicker.ThumbProperty, value);
    }
  }
}
