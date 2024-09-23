// Decompiled with JetBrains decompiler
// Type: WhatsApp.SteppedSliderControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class SteppedSliderControl : UserControl
  {
    private List<Rectangle> bars = new List<Rectangle>();
    private int prevstep = -1;
    internal StackPanel LayoutRoot;
    internal TextBlock FontSizeSampleText;
    internal Canvas StepCanvas;
    internal Slider FontSizeSlider;
    private bool _contentLoaded;

    public double[] Steps { get; set; }

    public SteppedSliderControl()
    {
      this.InitializeComponent();
      this.Steps = Constants.TextSizes.Values.ToArray<double>();
      this.LayoutRoot.Width = this.FontSizeSlider.Width = this.StepCanvas.Width = 460.0;
      this.FontSizeSlider.Minimum = 0.0;
      this.FontSizeSlider.Maximum = (double) (this.Steps.Length - 1);
      int num = (int) this.FontSizeSlider.Width / (this.Steps.Length - 1);
      for (int index = 1; index < this.Steps.Length - 1; ++index)
      {
        Rectangle rectangle = new Rectangle();
        rectangle.Width = 3.0;
        rectangle.Height = 12.0;
        rectangle.Fill = (Brush) (Application.Current.Resources[(object) "PhoneBackgroundBrush"] as SolidColorBrush);
        Rectangle element = rectangle;
        this.StepCanvas.Children.Add((UIElement) element);
        this.bars.Add(element);
        Canvas.SetLeft((UIElement) element, (double) (index * num));
        Canvas.SetTop((UIElement) element, 22.0);
        Canvas.SetZIndex((UIElement) element, 1);
      }
      double systemFontSize = Settings.SystemFontSize;
      this.prevstep = 0;
      while (this.prevstep < this.Steps.Length && this.Steps[this.prevstep] < systemFontSize)
        ++this.prevstep;
      if (this.prevstep > 0 && this.prevstep < this.Steps.Length - 1)
        this.bars[this.prevstep - 1].Visibility = Visibility.Collapsed;
      this.FontSizeSlider.Value = (double) this.prevstep;
      this.FontSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.FontSizeSlider_ValueChanged);
    }

    private void FontSizeSlider_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      if (this.Steps == null || this.Steps.Length <= 1)
        return;
      int num = this.prevstep;
      if (this.FontSizeSlider.Value > (double) this.prevstep && this.prevstep < this.Steps.Length - 1)
        num = this.prevstep + 1;
      else if (this.FontSizeSlider.Value < (double) this.prevstep && this.prevstep > 0)
        num = this.prevstep - 1;
      if (num != this.prevstep)
      {
        if (num > 0 && num < this.Steps.Length - 1)
          this.bars[num - 1].Visibility = Visibility.Collapsed;
        if (this.prevstep > 0 && this.prevstep < this.Steps.Length - 1)
          this.bars[this.prevstep - 1].Visibility = Visibility.Visible;
        this.prevstep = num;
      }
      this.FontSizeSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.FontSizeSlider_ValueChanged);
      this.FontSizeSlider.Value = (double) num;
      this.FontSizeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.FontSizeSlider_ValueChanged);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/SteppedSliderControl.xaml", UriKind.Relative));
      this.LayoutRoot = (StackPanel) this.FindName("LayoutRoot");
      this.FontSizeSampleText = (TextBlock) this.FindName("FontSizeSampleText");
      this.StepCanvas = (Canvas) this.FindName("StepCanvas");
      this.FontSizeSlider = (Slider) this.FindName("FontSizeSlider");
    }
  }
}
