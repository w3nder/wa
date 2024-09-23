// Decompiled with JetBrains decompiler
// Type: WhatsApp.PageWrapper
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

#nullable disable
namespace WhatsApp
{
  public class PageWrapper : WaDisposable
  {
    private PhoneApplicationPage page_;
    private PageOrientation prevOrientation_;
    private FrameworkElement pageRoot_;
    private CompositeTransform pageRootTransform_;
    private Storyboard rotationSb_;
    private DoubleAnimation rotationAnimation_;

    public PageWrapper(PhoneApplicationPage page)
    {
      this.page_ = page;
      this.SetupRotationTransition();
    }

    public static PageWrapper Create(PhoneApplicationPage page)
    {
      PageWrapper pageWrapper = (PageWrapper) null;
      if (page != null)
        pageWrapper = !(page is ChatPage) ? new PageWrapper(page) : (PageWrapper) new ChatPageWrapper(page as ChatPage);
      return pageWrapper;
    }

    public void ClosePendingContextMenu()
    {
      UIUtils.FindFirstInVisualTree((DependencyObject) this.page_, (Func<DependencyObject, bool>) (obj =>
      {
        ContextMenu contextMenu = ContextMenuService.GetContextMenu(obj);
        if (contextMenu == null || !contextMenu.IsOpen)
          return false;
        contextMenu.IsOpen = false;
        return true;
      }));
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      if (this.rotationSb_ != null)
        this.rotationSb_.Stop();
      this.page_.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
    }

    private void SetupRotationTransition()
    {
      if (this.page_ == null)
        return;
      this.prevOrientation_ = this.page_.Orientation;
      if (!(this.page_.Content is FrameworkElement content))
        return;
      content.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      content.RenderTransform = (Transform) (this.pageRootTransform_ = new CompositeTransform());
      this.pageRoot_ = content;
      this.page_.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
      this.page_.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
    }

    protected virtual bool OnRotationTransitionStarting(PageOrientation newOrientation)
    {
      this.page_.Focus();
      return true;
    }

    protected virtual void OnRotationTransitionFinished() => this.pageRootTransform_.Rotation = 0.0;

    protected virtual bool ShouldDelayRotationTransition() => false;

    private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      if (this.page_ == null || this.pageRoot_ == null || !this.OnRotationTransitionStarting(e.Orientation))
        return;
      this.PerformRotationTransition(this.prevOrientation_, e.Orientation);
      this.prevOrientation_ = e.Orientation;
    }

    private void PerformRotationTransition(
      PageOrientation oldOrientation,
      PageOrientation newOrientation)
    {
      if (this.rotationSb_ != null)
        this.rotationSb_.Stop();
      if (this.pageRoot_ == null || oldOrientation == newOrientation)
        return;
      int num = 0;
      switch (oldOrientation)
      {
        case PageOrientation.PortraitUp:
          switch (newOrientation)
          {
            case PageOrientation.LandscapeLeft:
              num = 90;
              break;
            case PageOrientation.LandscapeRight:
              num = -90;
              break;
          }
          break;
        case PageOrientation.LandscapeLeft:
          switch (newOrientation)
          {
            case PageOrientation.PortraitUp:
              num = -90;
              break;
            case PageOrientation.LandscapeRight:
              num = 180;
              break;
          }
          break;
        case PageOrientation.LandscapeRight:
          switch (newOrientation)
          {
            case PageOrientation.PortraitUp:
              num = 90;
              break;
            case PageOrientation.LandscapeLeft:
              num = -180;
              break;
          }
          break;
      }
      if (num == 0)
      {
        this.OnRotationTransitionFinished();
      }
      else
      {
        if (this.rotationSb_ == null)
        {
          this.rotationSb_ = new Storyboard();
          this.rotationAnimation_ = new DoubleAnimation()
          {
            From = new double?((double) -num),
            To = new double?(0.0),
            EasingFunction = (IEasingFunction) new SineEase()
          };
          Storyboard.SetTargetProperty((Timeline) this.rotationAnimation_, new PropertyPath("Rotation", new object[0]));
          Storyboard.SetTarget((Timeline) this.rotationAnimation_, (DependencyObject) this.pageRoot_.RenderTransform);
          this.rotationSb_.Children.Add((Timeline) this.rotationAnimation_);
          this.rotationSb_.Completed += (EventHandler) ((sender, e) =>
          {
            this.rotationSb_.Stop();
            this.OnRotationTransitionFinished();
          });
        }
        else
          this.rotationAnimation_.From = new double?((double) -num);
        this.rotationAnimation_.BeginTime = new TimeSpan?(TimeSpan.FromMilliseconds(this.ShouldDelayRotationTransition() ? 250.0 : 0.0));
        this.rotationAnimation_.Duration = (Duration) (oldOrientation == PageOrientation.PortraitUp ? TimeSpan.FromMilliseconds(500.0) : TimeSpan.FromMilliseconds(400.0));
        this.rotationSb_.Begin();
      }
    }
  }
}
