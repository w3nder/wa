// Decompiled with JetBrains decompiler
// Type: WhatsApp.TextTrimmingControl2
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class TextTrimmingControl2 : ContentControl
  {
    private string OriginalText;
    private string TruncatedText;
    public TextTrimmingControl2.TextTrimmingMode Mode;
    private TextBlock baseBlock;
    private Storyboard scrollingStoryboard;
    private DoubleKeyFrameCollection keyFrames;

    public TextTrimmingControl2()
    {
      TextBlock textBlock1 = new TextBlock();
      textBlock1.TextWrapping = TextWrapping.NoWrap;
      textBlock1.TextTrimming = TextTrimming.None;
      textBlock1.Margin = new Thickness(0.0);
      textBlock1.Padding = new Thickness(0.0);
      textBlock1.CacheMode = (CacheMode) new BitmapCache();
      TextBlock textBlock2 = textBlock1;
      this.baseBlock = textBlock1;
      this.Content = (object) textBlock2;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      Size size = base.MeasureOverride(availableSize);
      if (this.scrollingStoryboard != null)
        this.scrollingStoryboard.Stop();
      switch (this.Mode)
      {
        case TextTrimmingControl2.TextTrimmingMode.Ellipses:
          this.baseBlock.Text = this.TruncatedText;
          this.baseBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          double actualWidth1 = this.baseBlock.ActualWidth;
          double width = availableSize.Width;
          if (actualWidth1 > width)
          {
            this.Trim(actualWidth1, width);
            break;
          }
          break;
        case TextTrimmingControl2.TextTrimmingMode.TrimPrefix:
          if (this.Prefix != null)
          {
            this.baseBlock.Text = this.Prefix;
            this.baseBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          }
          double actualWidth2 = this.baseBlock.ActualWidth;
          this.baseBlock.Text = this.OriginalText;
          this.baseBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
          if (this.baseBlock.ActualWidth > availableSize.Width && this.Prefix != null && this.OriginalText.StartsWith(this.Prefix))
          {
            double num = -actualWidth2;
            if (this.baseBlock.RenderTransform == null || !(this.baseBlock.RenderTransform is CompositeTransform))
              this.baseBlock.RenderTransform = (Transform) new CompositeTransform();
            if (this.Clip == null)
              this.Clip = (Geometry) new RectangleGeometry()
              {
                Rect = new Rect(0.0, 0.0, this.baseBlock.ActualWidth, this.baseBlock.ActualHeight)
              };
            if (this.scrollingStoryboard == null)
            {
              DoubleAnimationUsingKeyFrames animationUsingKeyFrames = new DoubleAnimationUsingKeyFrames();
              animationUsingKeyFrames.FillBehavior = FillBehavior.HoldEnd;
              DoubleAnimationUsingKeyFrames element = animationUsingKeyFrames;
              Storyboard.SetTarget((Timeline) element, (DependencyObject) this.baseBlock.RenderTransform);
              Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("TranslateX", new object[0]));
              DoubleKeyFrameCollection keyFrames1 = element.KeyFrames;
              DiscreteDoubleKeyFrame discreteDoubleKeyFrame1 = new DiscreteDoubleKeyFrame();
              discreteDoubleKeyFrame1.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0.0));
              discreteDoubleKeyFrame1.Value = 0.0;
              keyFrames1.Add((DoubleKeyFrame) discreteDoubleKeyFrame1);
              DoubleKeyFrameCollection keyFrames2 = element.KeyFrames;
              DiscreteDoubleKeyFrame discreteDoubleKeyFrame2 = new DiscreteDoubleKeyFrame();
              discreteDoubleKeyFrame2.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(750.0));
              discreteDoubleKeyFrame2.Value = 0.0;
              keyFrames2.Add((DoubleKeyFrame) discreteDoubleKeyFrame2);
              DoubleKeyFrameCollection keyFrames3 = element.KeyFrames;
              EasingDoubleKeyFrame easingDoubleKeyFrame = new EasingDoubleKeyFrame();
              easingDoubleKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(1250.0));
              easingDoubleKeyFrame.Value = num;
              ExponentialEase exponentialEase = new ExponentialEase();
              exponentialEase.Exponent = 4.0;
              exponentialEase.EasingMode = EasingMode.EaseOut;
              easingDoubleKeyFrame.EasingFunction = (IEasingFunction) exponentialEase;
              keyFrames3.Add((DoubleKeyFrame) easingDoubleKeyFrame);
              this.keyFrames = element.KeyFrames;
              this.scrollingStoryboard = new Storyboard();
              this.scrollingStoryboard.Children.Add((Timeline) element);
            }
            else
              this.keyFrames[2].Value = num;
            this.scrollingStoryboard.Begin();
            break;
          }
          break;
      }
      return size;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      if (this.Mode == TextTrimmingControl2.TextTrimmingMode.TrimPrefix)
        this.baseBlock.Arrange(new Rect(0.0, 0.0, this.baseBlock.ActualWidth, this.baseBlock.ActualHeight));
      else
        finalSize = base.ArrangeOverride(finalSize);
      return finalSize;
    }

    public string Text
    {
      set
      {
        string str = value ?? string.Empty;
        this.OriginalText = str;
        this.baseBlock.Text = str;
        this.TruncatedText = str.Replace(' ', ' ');
      }
    }

    public string Prefix { private get; set; }

    private void Trim(double desiredWidth, double availableWidth)
    {
      if (this.TruncatedText.Length < 3)
        return;
      int num = (int) Math.Max(0.0, (double) this.TruncatedText.Length - Math.Max(6.0, (1.0 - availableWidth / desiredWidth) * (double) this.TruncatedText.Length + 3.0));
      int funkynessLength = 0;
      if (Utils.IsFunkyUnicode(this.TruncatedText, num, out funkynessLength))
        num = Math.Min(this.TruncatedText.Length, num + funkynessLength);
      this.baseBlock.Text = this.TruncatedText = this.TruncatedText.Substring(0, num) + " …";
    }

    public enum TextTrimmingMode
    {
      None,
      Ellipses,
      TrimPrefix,
    }
  }
}
