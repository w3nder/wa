// Decompiled with JetBrains decompiler
// Type: WhatsApp.AdaptiveTextBlock
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;


namespace WhatsApp
{
  public class AdaptiveTextBlock : ContentControl
  {
    private TextBlock baseBlock_;
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (AdaptiveTextBlock), new PropertyMetadata(new PropertyChangedCallback(AdaptiveTextBlock.OnTextPropertyChanged)));

    private static void OnTextPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      ((AdaptiveTextBlock) obj).OnTextPropertyChanged((string) e.OldValue, (string) e.NewValue);
    }

    private void OnTextPropertyChanged(string oldValue, string newValue)
    {
      this.baseBlock_.Text = newValue;
    }

    public string Text
    {
      get => (string) this.GetValue(AdaptiveTextBlock.TextProperty);
      set => this.SetValue(AdaptiveTextBlock.TextProperty, (object) value);
    }

    public double ActualFontSize { get; private set; }

    public AdaptiveTextBlock()
    {
      TextBlock textBlock = new TextBlock();
      textBlock.TextWrapping = TextWrapping.NoWrap;
      textBlock.TextTrimming = TextTrimming.None;
      textBlock.Margin = new Thickness(0.0);
      textBlock.Padding = new Thickness(0.0);
      this.baseBlock_ = textBlock;
      // ISSUE: explicit constructor call
      base.\u002Ector();
      this.Content = (object) this.baseBlock_;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      double fontSize = this.FontSize;
      bool flag;
      do
      {
        flag = false;
        this.baseBlock_.FontFamily = this.FontFamily;
        this.baseBlock_.FontSize = fontSize;
        this.baseBlock_.FontStyle = this.FontStyle;
        this.baseBlock_.FontWeight = this.FontWeight;
        this.baseBlock_.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        if (this.baseBlock_.ActualWidth > availableSize.Width)
        {
          flag = true;
          fontSize -= 2.0;
        }
      }
      while (flag);
      this.ActualFontSize = fontSize;
      return base.MeasureOverride(availableSize);
    }
  }
}
