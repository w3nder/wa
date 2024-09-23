// Decompiled with JetBrains decompiler
// Type: Clarity.Phone.Extensions.DialogService
// Assembly: Coding4Fun.Phone.Controls, Version=1.6.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5583BFDF-52F3-4F66-A397-92165DEE5729
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Coding4Fun.Phone.Controls.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace Clarity.Phone.Extensions
{
  public class DialogService
  {
    private const string SlideUpStoryboard = "\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.Y)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"150\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"0\" To=\"1\" Duration=\"0:0:0.350\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>";
    private const string SlideHorizontalInStoryboard = "\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.X)\" >\r\n                    <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"-150\"/>\r\n                    <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                        <EasingDoubleKeyFrame.EasingFunction>\r\n                            <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                        </EasingDoubleKeyFrame.EasingFunction>\r\n                    </EasingDoubleKeyFrame>\r\n                </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"0\" To=\"1\" Duration=\"0:0:0.350\" >\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>";
    private const string SlideHorizontalOutStoryboard = "\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.X)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"150\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"1\" To=\"0\" Duration=\"0:0:0.25\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>";
    private const string SlideDownStoryboard = "\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.Y)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"150\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"1\" To=\"0\" Duration=\"0:0:0.25\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>";
    private const string SwivelInStoryboard = "<Storyboard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimation BeginTime=\"0:0:0\" Duration=\"0\" To=\".5\"\r\n                                Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.CenterOfRotationY)\" />\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.RotationX)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"-30\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Opacity)\">\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0\" Value=\"1\" />\r\n            </DoubleAnimationUsingKeyFrames>\r\n        </Storyboard>";
    private const string SwivelOutStoryboard = "<Storyboard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimation BeginTime=\"0:0:0\" Duration=\"0\" \r\n                                Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.CenterOfRotationY)\" \r\n                                To=\".5\"/>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.RotationX)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"45\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Opacity)\">\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0\" Value=\"1\" />\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0:0:0.267\" Value=\"0\" />\r\n            </DoubleAnimationUsingKeyFrames>\r\n        </Storyboard>";
    private Panel _popupContainer;
    private Frame _rootVisual;
    private PhoneApplicationPage _page;
    private Panel _overlay;
    private bool _deferredShowToLoaded;
    private static readonly object Lockobj = new object();

    public FrameworkElement Child { get; set; }

    public DialogService.AnimationTypes AnimationType { get; set; }

    public double VerticalOffset { get; set; }

    internal double ControlVerticalOffset { get; set; }

    public Brush BackgroundBrush { get; set; }

    internal bool IsOpen { get; set; }

    protected internal bool IsBackKeyOverride { get; set; }

    public event EventHandler Closed;

    public event EventHandler Opened;

    public bool HasPopup { get; set; }

    internal PhoneApplicationPage Page
    {
      get
      {
        return this._page ?? (this._page = this.RootVisual.GetFirstLogicalChildByType<PhoneApplicationPage>(false));
      }
    }

    internal Frame RootVisual
    {
      get => this._rootVisual ?? (this._rootVisual = Application.Current.RootVisual as Frame);
    }

    internal Panel PopupContainer
    {
      get
      {
        if (this._popupContainer == null)
        {
          IEnumerable<ContentPresenter> logicalChildrenByType1 = this.RootVisual.GetLogicalChildrenByType<ContentPresenter>(false);
          for (int index = 0; index < logicalChildrenByType1.Count<ContentPresenter>(); ++index)
          {
            IEnumerable<Panel> logicalChildrenByType2 = ((FrameworkElement) logicalChildrenByType1.ElementAt<ContentPresenter>(index)).GetLogicalChildrenByType<Panel>(false);
            if (logicalChildrenByType2.Any<Panel>())
            {
              this._popupContainer = logicalChildrenByType2.First<Panel>();
              break;
            }
          }
        }
        return this._popupContainer;
      }
    }

    public DialogService() => this.AnimationType = DialogService.AnimationTypes.Slide;

    private void InitializePopup()
    {
      Grid grid = new Grid();
      ((FrameworkElement) grid).Name = Guid.NewGuid().ToString();
      this._overlay = (Panel) grid;
      Grid.SetColumnSpan((FrameworkElement) this._overlay, int.MaxValue);
      Grid.SetRowSpan((FrameworkElement) this._overlay, int.MaxValue);
      if (this.BackgroundBrush != null)
        this._overlay.Background = this.BackgroundBrush;
      this.CalculateVerticalOffset();
      ((UIElement) this._overlay).Opacity = 0.0;
      if (this.PopupContainer != null)
      {
        ((PresentationFrameworkCollection<UIElement>) this.PopupContainer.Children).Add((UIElement) this._overlay);
        ((PresentationFrameworkCollection<UIElement>) this._overlay.Children).Add((UIElement) this.Child);
      }
      else
      {
        this._deferredShowToLoaded = true;
        this.RootVisual.Loaded += new RoutedEventHandler(this.RootVisualDeferredShowLoaded);
      }
    }

    internal void CalculateVerticalOffset()
    {
      if (this._overlay == null)
        return;
      int num = 0;
      if (SystemTray.IsVisible && SystemTray.Opacity < 1.0 && SystemTray.Opacity > 0.0)
        num += 32;
      ((FrameworkElement) this._overlay).Margin = new Thickness(0.0, this.VerticalOffset + (double) num + this.ControlVerticalOffset, 0.0, 0.0);
    }

    private void RootVisualDeferredShowLoaded(object sender, RoutedEventArgs e)
    {
      this.RootVisual.Loaded -= new RoutedEventHandler(this.RootVisualDeferredShowLoaded);
      this._deferredShowToLoaded = false;
      this.Show();
    }

    protected internal void SetAlignmentsOnOverlay(
      HorizontalAlignment horizontalAlignment,
      VerticalAlignment verticalAlignment)
    {
      if (this._overlay == null)
        return;
      ((FrameworkElement) this._overlay).HorizontalAlignment = horizontalAlignment;
      ((FrameworkElement) this._overlay).VerticalAlignment = verticalAlignment;
    }

    public void Show()
    {
      bool lockTaken = false;
      object lockobj;
      try
      {
        Monitor.Enter(lockobj = DialogService.Lockobj, ref lockTaken);
        this.IsOpen = true;
        this.InitializePopup();
        if (this._deferredShowToLoaded)
          return;
        if (!this.IsBackKeyOverride)
          this.Page.BackKeyPress += new EventHandler<CancelEventArgs>(this.OnBackKeyPress);
        this.Page.NavigationService.Navigated += new NavigatedEventHandler(this.OnNavigated);
        Storyboard storyboard;
        switch (this.AnimationType)
        {
          case DialogService.AnimationTypes.Slide:
            storyboard = XamlReader.Load("\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.Y)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"150\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"0\" To=\"1\" Duration=\"0:0:0.350\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>") as Storyboard;
            ((UIElement) this._overlay).RenderTransform = (Transform) new TranslateTransform();
            break;
          case DialogService.AnimationTypes.SlideHorizontal:
            storyboard = XamlReader.Load("\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.X)\" >\r\n                    <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"-150\"/>\r\n                    <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                        <EasingDoubleKeyFrame.EasingFunction>\r\n                            <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                        </EasingDoubleKeyFrame.EasingFunction>\r\n                    </EasingDoubleKeyFrame>\r\n                </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"0\" To=\"1\" Duration=\"0:0:0.350\" >\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>") as Storyboard;
            ((UIElement) this._overlay).RenderTransform = (Transform) new TranslateTransform();
            break;
          default:
            storyboard = XamlReader.Load("<Storyboard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimation BeginTime=\"0:0:0\" Duration=\"0\" To=\".5\"\r\n                                Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.CenterOfRotationY)\" />\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.RotationX)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"-30\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.35\" Value=\"0\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseOut\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Opacity)\">\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0\" Value=\"1\" />\r\n            </DoubleAnimationUsingKeyFrames>\r\n        </Storyboard>") as Storyboard;
            ((UIElement) this._overlay).Projection = (Projection) new PlaneProjection();
            break;
        }
        if (storyboard != null)
          this.Page.Dispatcher.BeginInvoke((Action) (() =>
          {
            foreach (Timeline child in (PresentationFrameworkCollection<Timeline>) storyboard.Children)
              Storyboard.SetTarget(child, (DependencyObject) this._overlay);
            storyboard.Begin();
          }));
        if (this.Opened == null)
          return;
        this.Opened((object) this, (EventArgs) null);
      }
      finally
      {
        if (lockTaken)
          Monitor.Exit(lockobj);
      }
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => this.Hide();

    public void Hide()
    {
      if (!this.IsOpen)
        return;
      if (this.Page != null)
      {
        this.Page.BackKeyPress -= new EventHandler<CancelEventArgs>(this.OnBackKeyPress);
        this.Page.NavigationService.Navigated -= new NavigatedEventHandler(this.OnNavigated);
        this._page = (PhoneApplicationPage) null;
      }
      Storyboard storyboard;
      switch (this.AnimationType)
      {
        case DialogService.AnimationTypes.Slide:
          storyboard = XamlReader.Load("\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.Y)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"150\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"1\" To=\"0\" Duration=\"0:0:0.25\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>") as Storyboard;
          break;
        case DialogService.AnimationTypes.SlideHorizontal:
          storyboard = XamlReader.Load("\r\n        <Storyboard  xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.RenderTransform).(TranslateTransform.X)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"150\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimation Storyboard.TargetProperty=\"(UIElement.Opacity)\" From=\"1\" To=\"0\" Duration=\"0:0:0.25\">\r\n                <DoubleAnimation.EasingFunction>\r\n                    <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                </DoubleAnimation.EasingFunction>\r\n            </DoubleAnimation>\r\n        </Storyboard>") as Storyboard;
          break;
        default:
          storyboard = XamlReader.Load("<Storyboard xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">\r\n            <DoubleAnimation BeginTime=\"0:0:0\" Duration=\"0\" \r\n                                Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.CenterOfRotationY)\" \r\n                                To=\".5\"/>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Projection).(PlaneProjection.RotationX)\">\r\n                <EasingDoubleKeyFrame KeyTime=\"0\" Value=\"0\"/>\r\n                <EasingDoubleKeyFrame KeyTime=\"0:0:0.25\" Value=\"45\">\r\n                    <EasingDoubleKeyFrame.EasingFunction>\r\n                        <ExponentialEase EasingMode=\"EaseIn\" Exponent=\"6\"/>\r\n                    </EasingDoubleKeyFrame.EasingFunction>\r\n                </EasingDoubleKeyFrame>\r\n            </DoubleAnimationUsingKeyFrames>\r\n            <DoubleAnimationUsingKeyFrames Storyboard.TargetProperty=\"(UIElement.Opacity)\">\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0\" Value=\"1\" />\r\n                <DiscreteDoubleKeyFrame KeyTime=\"0:0:0.267\" Value=\"0\" />\r\n            </DoubleAnimationUsingKeyFrames>\r\n        </Storyboard>") as Storyboard;
          break;
      }
      try
      {
        if (storyboard == null)
          return;
        ((Timeline) storyboard).Completed += new EventHandler(this._hideStoryboard_Completed);
        foreach (Timeline child in (PresentationFrameworkCollection<Timeline>) storyboard.Children)
          Storyboard.SetTarget(child, (DependencyObject) this._overlay);
        storyboard.Begin();
      }
      catch (Exception ex)
      {
        this._hideStoryboard_Completed((object) null, (EventArgs) null);
      }
    }

    private void _hideStoryboard_Completed(object sender, EventArgs e)
    {
      this.IsOpen = false;
      try
      {
        if (this.PopupContainer != null)
        {
          if (this.PopupContainer.Children != null)
            ((PresentationFrameworkCollection<UIElement>) this.PopupContainer.Children).Remove((UIElement) this._overlay);
        }
      }
      catch (Exception ex)
      {
      }
      try
      {
        if (this.Closed == null)
          return;
        this.Closed((object) this, (EventArgs) null);
      }
      catch (Exception ex)
      {
      }
    }

    public void OnBackKeyPress(object sender, CancelEventArgs e)
    {
      if (this.HasPopup)
      {
        e.Cancel = true;
      }
      else
      {
        if (!this.IsOpen)
          return;
        e.Cancel = true;
        this.Hide();
      }
    }

    public enum AnimationTypes
    {
      Slide,
      SlideHorizontal,
      Swivel,
      SwivelHorizontal,
    }
  }
}
