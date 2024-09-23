// Decompiled with JetBrains decompiler
// Type: WhatsApp.SegmentBar
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace WhatsApp
{
  public class SegmentBar : UserControl
  {
    private Grid baseBar;
    private Rectangle fillBar;
    private TranslateTransform fillBarTransform;
    private Size? size;
    private Storyboard pendingSb;
    private DoubleAnimation pendingAnimation;

    public new Brush Background
    {
      get => base.Background ?? (base.Background = UIUtils.PhoneInactiveBrush);
      set => this.baseBar.Background = base.Background = value;
    }

    public new Brush Foreground { get; set; } = (Brush) UIUtils.WhiteBrush;

    public SegmentBar(double height)
    {
      Grid grid1 = new Grid();
      grid1.Background = this.Background;
      grid1.Height = height;
      Grid grid2 = grid1;
      this.baseBar = grid1;
      this.Content = (UIElement) grid2;
    }

    private void ResetAnimation(bool stop = true)
    {
      if (stop)
        this.pendingSb?.Stop();
      this.pendingAnimation = (DoubleAnimation) null;
      this.pendingSb = (Storyboard) null;
    }

    public void Animate(bool forward, Duration duration, Storyboard sb, DoubleAnimation da)
    {
      if (this.fillBar == null)
      {
        this.fillBarTransform = new TranslateTransform();
        Rectangle rectangle = new Rectangle();
        rectangle.Fill = this.Foreground;
        rectangle.RenderTransform = (Transform) this.fillBarTransform;
        this.fillBar = rectangle;
        this.baseBar.Children.Add((UIElement) this.fillBar);
      }
      sb.Stop();
      Storyboard.SetTarget((Timeline) da, (DependencyObject) this.fillBarTransform);
      da.Duration = duration;
      if (this.size.HasValue)
      {
        double width = this.size.Value.Width;
        da.From = new double?(forward ? -width : 0.0);
        double num = forward ? 0.0 : -width;
        da.To = new double?(num);
        this.fillBarTransform.X = num;
        sb.Begin();
        this.pendingSb = (Storyboard) null;
        this.pendingAnimation = (DoubleAnimation) null;
      }
      else
      {
        this.pendingSb = sb;
        this.pendingAnimation = da;
      }
    }

    public void Fill()
    {
      this.ResetAnimation();
      if (this.fillBarTransform == null)
        this.baseBar.Background = this.Foreground;
      else
        this.fillBarTransform.X = 0.0;
    }

    public void Clear()
    {
      this.ResetAnimation();
      this.baseBar.Background = this.Background;
      if (this.fillBarTransform == null || !this.size.HasValue)
        return;
      this.fillBarTransform.X = -this.size.Value.Width;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.size = new Size?(finalSize);
      this.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height)
      };
      if (this.fillBarTransform != null)
        this.fillBarTransform.X = -finalSize.Width;
      if (this.pendingSb != null && this.pendingAnimation != null)
      {
        this.pendingAnimation.From = new double?(this.fillBarTransform.X = -finalSize.Width);
        this.pendingSb.Begin();
      }
      this.ResetAnimation(false);
      return base.ArrangeOverride(finalSize);
    }
  }
}
