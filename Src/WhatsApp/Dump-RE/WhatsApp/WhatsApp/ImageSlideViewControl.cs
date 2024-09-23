// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageSlideViewControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class ImageSlideViewControl : UserControl
  {
    private const double GapSize = 24.0;
    private const double MinFlickSpeed = 1000.0;
    private ImageViewControl.TransformState state_ = new ImageViewControl.TransformState();
    private IDisposable centerImgSrcSub_;
    private IDisposable leftImgSrcSub_;
    private IDisposable rightImgSrcSub_;
    private BitmapSource centerImgSrc_;
    private BitmapSource leftImgSrc_;
    private BitmapSource rightImgSrc_;
    private PageOrientation orientation_;
    private Storyboard switchSb_;
    private DoubleAnimation switchAnimation_;
    private bool isMoving_;
    private IDisposable switchingSbSub_;
    internal TranslateTransform LayoutRootTransform;
    internal Rectangle BackgroundRectangle;
    internal Image LeftImage;
    internal CompositeTransform LeftImageTransform;
    internal Image RightImage;
    internal CompositeTransform RightImageTransform;
    internal ImageViewControl CenterImage;
    private bool _contentLoaded;

    public BitmapSource CenterImageSource => this.centerImgSrc_;

    public event ImageSlideViewControl.ImageSwitchingHandler ImageSwitching;

    protected void NotifyImageSwitching(int direction, double percentage)
    {
      if (this.ImageSwitching == null)
        return;
      this.ImageSwitching(direction, percentage);
    }

    public event ImageSlideViewControl.ImageSwitchedHandler ImageSwitched;

    protected void NotifyImageSwitched(int direction)
    {
      AppState.Worker.Enqueue((Action) (() => GC.Collect()));
      if (this.ImageSwitched == null)
        return;
      this.ImageSwitched(direction);
    }

    public PageOrientation Orientation
    {
      get => this.orientation_;
      set
      {
        if (this.orientation_ == value)
          return;
        this.orientation_ = value;
      }
    }

    public bool IsLocked { get; set; }

    public Thickness? ContentPadding { get; set; }

    public double EffectiveWidth
    {
      get
      {
        if (!this.ContentPadding.HasValue)
          return this.ActualWidth;
        double actualWidth = this.ActualWidth;
        Thickness? contentPadding = this.ContentPadding;
        Thickness thickness = contentPadding.Value;
        double left = thickness.Left;
        double num = actualWidth - left;
        contentPadding = this.ContentPadding;
        thickness = contentPadding.Value;
        double right = thickness.Right;
        return num - right;
      }
    }

    public double EffectiveHeight
    {
      get
      {
        if (!this.ContentPadding.HasValue)
          return this.ActualHeight;
        double actualHeight = this.ActualHeight;
        Thickness? contentPadding = this.ContentPadding;
        Thickness thickness = contentPadding.Value;
        double top = thickness.Top;
        double num = actualHeight - top;
        contentPadding = this.ContentPadding;
        thickness = contentPadding.Value;
        double bottom = thickness.Bottom;
        return num - bottom;
      }
    }

    public ImageSlideViewControl()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
    }

    private void ApplyTransformState(ImageViewControl.TransformState state)
    {
      this.LayoutRootTransform.X = state.TranslateX;
    }

    public void SetCenterImageSource(BitmapSource imgSrc)
    {
      this.centerImgSrcSub_.SafeDispose();
      this.centerImgSrcSub_ = (IDisposable) null;
      if (this.centerImgSrc_ == imgSrc && imgSrc != null)
        return;
      this.CenterImage.ImageSource = this.centerImgSrc_ = imgSrc;
    }

    public void SetCenterImageSource(IObservable<BitmapSource> src, bool async)
    {
      this.SetCenterImageSource((BitmapSource) null);
      if (src == null)
        return;
      this.centerImgSrcSub_ = (async ? src.ObserveOnDispatcher<BitmapSource>() : src).Subscribe<BitmapSource>(new Action<BitmapSource>(this.SetCenterImageSource), (Action) (() =>
      {
        this.centerImgSrcSub_.SafeDispose();
        this.centerImgSrcSub_ = (IDisposable) null;
      }));
    }

    private void UpdateLeftImageTransform()
    {
      if (this.leftImgSrc_ == null)
        return;
      int pixelWidth = this.leftImgSrc_.PixelWidth;
      int pixelHeight = this.leftImgSrc_.PixelHeight;
      double effectiveWidth = this.EffectiveWidth;
      double effectiveHeight = this.EffectiveHeight;
      double num1 = Math.Min(effectiveWidth / (double) pixelWidth, effectiveHeight / (double) pixelHeight);
      this.LeftImageTransform.ScaleX = this.LeftImageTransform.ScaleY = num1;
      Thickness? contentPadding = this.ContentPadding;
      if (contentPadding.HasValue)
      {
        CompositeTransform leftImageTransform1 = this.LeftImageTransform;
        double num2 = -24.0 - (effectiveWidth + (double) pixelWidth * num1) / 2.0;
        contentPadding = this.ContentPadding;
        double right = contentPadding.Value.Right;
        double num3 = num2 - right;
        leftImageTransform1.TranslateX = num3;
        CompositeTransform leftImageTransform2 = this.LeftImageTransform;
        double num4 = (effectiveHeight - (double) pixelHeight * num1) / 2.0;
        contentPadding = this.ContentPadding;
        double top = contentPadding.Value.Top;
        double num5 = num4 + top;
        leftImageTransform2.TranslateY = num5;
      }
      else
      {
        this.LeftImageTransform.TranslateX = -24.0 - (effectiveWidth + (double) pixelWidth * num1) / 2.0;
        this.LeftImageTransform.TranslateY = (effectiveHeight - (double) pixelHeight * num1) / 2.0;
      }
    }

    public void SetLeftImageSource(BitmapSource imgSrc)
    {
      this.leftImgSrcSub_.SafeDispose();
      this.leftImgSrcSub_ = (IDisposable) null;
      this.LeftImage.Source = (System.Windows.Media.ImageSource) (this.leftImgSrc_ = (BitmapSource) null);
      if ((this.leftImgSrc_ = imgSrc) == null)
        return;
      this.UpdateLeftImageTransform();
      this.LeftImage.Source = (System.Windows.Media.ImageSource) imgSrc;
    }

    public void SetLeftImageSource(IObservable<BitmapSource> src, bool async)
    {
      this.SetLeftImageSource((BitmapSource) null);
      if (src == null)
        return;
      this.leftImgSrcSub_ = (async ? src.ObserveOnDispatcher<BitmapSource>() : src).Subscribe<BitmapSource>((Action<BitmapSource>) (bitmap => this.SetLeftImageSource(bitmap)), (Action) (() =>
      {
        this.leftImgSrcSub_.SafeDispose();
        this.leftImgSrcSub_ = (IDisposable) null;
      }));
    }

    private void UpdateRightImageTransform()
    {
      if (this.rightImgSrc_ == null)
        return;
      int pixelWidth = this.rightImgSrc_.PixelWidth;
      int pixelHeight = this.rightImgSrc_.PixelHeight;
      double effectiveWidth = this.EffectiveWidth;
      double effectiveHeight = this.EffectiveHeight;
      double num1 = Math.Min(effectiveWidth / (double) pixelWidth, effectiveHeight / (double) pixelHeight);
      Thickness? contentPadding = this.ContentPadding;
      if (contentPadding.HasValue)
      {
        this.RightImageTransform.ScaleX = this.RightImageTransform.ScaleY = num1;
        CompositeTransform rightImageTransform1 = this.RightImageTransform;
        double num2 = 24.0 + this.ActualWidth + (effectiveWidth - (double) pixelWidth * num1) / 2.0;
        contentPadding = this.ContentPadding;
        double left = contentPadding.Value.Left;
        double num3 = num2 + left;
        rightImageTransform1.TranslateX = num3;
        CompositeTransform rightImageTransform2 = this.RightImageTransform;
        double num4 = (effectiveHeight - (double) pixelHeight * num1) / 2.0;
        contentPadding = this.ContentPadding;
        double top = contentPadding.Value.Top;
        double num5 = num4 + top;
        rightImageTransform2.TranslateY = num5;
      }
      else
      {
        this.RightImageTransform.ScaleX = this.RightImageTransform.ScaleY = num1;
        this.RightImageTransform.TranslateX = 24.0 + this.ActualWidth + (effectiveWidth - (double) pixelWidth * num1) / 2.0;
        this.RightImageTransform.TranslateY = (effectiveHeight - (double) pixelHeight * num1) / 2.0;
      }
    }

    public void SetRightImageSource(BitmapSource imgSrc, bool disposeCurrentSub = true)
    {
      if (disposeCurrentSub)
      {
        this.rightImgSrcSub_.SafeDispose();
        this.rightImgSrcSub_ = (IDisposable) null;
      }
      this.RightImage.Source = (System.Windows.Media.ImageSource) (this.rightImgSrc_ = (BitmapSource) null);
      if ((this.rightImgSrc_ = imgSrc) == null)
        return;
      this.UpdateRightImageTransform();
      this.RightImage.Source = (System.Windows.Media.ImageSource) imgSrc;
    }

    public void SetRightImageSource(IObservable<BitmapSource> src, bool async)
    {
      this.rightImgSrcSub_.SafeDispose();
      if (src == null)
        this.SetRightImageSource((BitmapSource) null);
      else
        this.rightImgSrcSub_ = (async ? src.ObserveOnDispatcher<BitmapSource>() : src).Subscribe<BitmapSource>((Action<BitmapSource>) (bitmap => this.SetRightImageSource(bitmap, false)), (Action) (() =>
        {
          this.rightImgSrcSub_.SafeDispose();
          this.rightImgSrcSub_ = (IDisposable) null;
        }));
    }

    private void ResetTransforms()
    {
      this.CenterImage.Width = this.BackgroundRectangle.Width = this.EffectiveWidth;
      this.CenterImage.Height = this.BackgroundRectangle.Height = this.EffectiveHeight;
      this.CenterImage.Orientation = this.orientation_;
      if (this.ContentPadding.HasValue)
        this.CenterImage.Margin = this.ContentPadding.Value;
      this.UpdateLeftImageTransform();
      this.UpdateRightImageTransform();
    }

    private Storyboard GetDeleteAnimation()
    {
      DoubleAnimation doubleAnimation = new DoubleAnimation();
      doubleAnimation.From = new double?(1.0);
      doubleAnimation.To = new double?(0.0);
      doubleAnimation.Duration = (Duration) TimeSpan.FromMilliseconds(300.0);
      ExponentialEase exponentialEase = new ExponentialEase();
      exponentialEase.Exponent = 3.0;
      exponentialEase.EasingMode = EasingMode.EaseIn;
      doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase;
      DoubleAnimation element = doubleAnimation;
      Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("Opacity", new object[0]));
      Storyboard.SetTarget((Timeline) element, (DependencyObject) this.CenterImage);
      Storyboard deleteAnimation = new Storyboard();
      deleteAnimation.Children.Add((Timeline) element);
      return deleteAnimation;
    }

    private Storyboard GetSwitchAnimation(double currX, double velocity, int switchDirection)
    {
      if (this.switchSb_ == null || this.switchAnimation_ == null)
      {
        DoubleAnimation doubleAnimation = new DoubleAnimation();
        ExponentialEase exponentialEase = new ExponentialEase();
        exponentialEase.Exponent = 2.0;
        exponentialEase.EasingMode = EasingMode.EaseOut;
        doubleAnimation.EasingFunction = (IEasingFunction) exponentialEase;
        DoubleAnimation element = doubleAnimation;
        Storyboard.SetTargetProperty((Timeline) element, new PropertyPath("X", new object[0]));
        Storyboard.SetTarget((Timeline) element, (DependencyObject) this.LayoutRootTransform);
        Storyboard storyboard = new Storyboard();
        storyboard.Children.Add((Timeline) element);
        this.switchSb_ = storyboard;
        this.switchAnimation_ = element;
      }
      this.switchAnimation_.From = new double?(currX);
      switch (switchDirection)
      {
        case -1:
          this.switchAnimation_.To = new double?(24.0 + this.ActualWidth);
          break;
        case 1:
          this.switchAnimation_.To = new double?(-24.0 - this.ActualWidth);
          break;
        default:
          this.switchAnimation_.To = new double?(0.0);
          break;
      }
      this.switchAnimation_.Duration = (Duration) TimeSpan.FromMilliseconds(velocity < 600.0 ? 400.0 : 300000.0 / velocity);
      return this.switchSb_;
    }

    private void SwitchToRight(double switchVelocity)
    {
      this.switchingSbSub_.SafeDispose();
      this.switchingSbSub_ = (IDisposable) null;
      if (this.rightImgSrc_ == null)
      {
        this.ResetToCurrent(switchVelocity);
      }
      else
      {
        this.NotifyImageSwitching(1, 1.0);
        this.switchingSbSub_ = Storyboarder.PerformWithDisposable(this.GetSwitchAnimation(this.state_.TranslateX, switchVelocity, 1), onComplete: (Action) (() =>
        {
          BitmapSource rightImgSrc = this.rightImgSrc_;
          BitmapSource centerImgSrc = this.centerImgSrc_;
          this.SetCenterImageSource(rightImgSrc);
          this.SetLeftImageSource(centerImgSrc);
          this.SetRightImageSource((BitmapSource) null);
          this.state_.TranslateX = 0.0;
          this.ApplyTransformState(this.state_);
          this.NotifyImageSwitched(1);
        }), callOnCompleteOnDisposing: true, context: "img slider: switch to right");
      }
    }

    private void SwitchToLeft(double switchVelocity)
    {
      this.switchingSbSub_.SafeDispose();
      this.switchingSbSub_ = (IDisposable) null;
      if (this.leftImgSrc_ == null)
      {
        this.ResetToCurrent(switchVelocity);
      }
      else
      {
        this.NotifyImageSwitching(-1, 1.0);
        this.switchingSbSub_ = Storyboarder.PerformWithDisposable(this.GetSwitchAnimation(this.state_.TranslateX, switchVelocity, -1), onComplete: (Action) (() =>
        {
          BitmapSource leftImgSrc = this.leftImgSrc_;
          BitmapSource centerImgSrc = this.centerImgSrc_;
          this.SetCenterImageSource(leftImgSrc);
          this.SetRightImageSource(centerImgSrc);
          this.SetLeftImageSource((BitmapSource) null);
          this.state_.TranslateX = 0.0;
          this.ApplyTransformState(this.state_);
          this.NotifyImageSwitched(-1);
        }), callOnCompleteOnDisposing: true, context: "img slider: switch to left");
      }
    }

    private void ResetToCurrent(double velocity)
    {
      this.switchingSbSub_.SafeDispose();
      this.switchingSbSub_ = (IDisposable) null;
      this.switchingSbSub_ = Storyboarder.PerformWithDisposable(this.GetSwitchAnimation(this.state_.TranslateX, velocity, 0), onComplete: (Action) (() =>
      {
        this.NotifyImageSwitched(0);
        this.state_.TranslateX = 0.0;
        this.ApplyTransformState(this.state_);
      }), callOnCompleteOnDisposing: true, context: "img slider: switch to left");
    }

    public IObservable<int> DeleteCurrent()
    {
      return Observable.CreateWithDisposable<int>((Func<IObserver<int>, IDisposable>) (observer => Storyboarder.PerformWithDisposable(this.GetDeleteAnimation(), (DependencyObject) null, true, (Action) (() =>
      {
        if (this.leftImgSrc_ != null)
        {
          this.SetCenterImageSource(this.leftImgSrc_);
          this.SetLeftImageSource((BitmapSource) null);
          observer.OnNext(-1);
        }
        else if (this.rightImgSrc_ != null)
        {
          this.SetCenterImageSource(this.rightImgSrc_);
          this.SetRightImageSource((BitmapSource) null);
          observer.OnNext(1);
        }
        else
          observer.OnNext(0);
      }), false, "img slider: delete")));
    }

    private void OnHorizontalGuestureEnd(double finalVelocity)
    {
      bool flag = false;
      double num = Math.Abs(finalVelocity);
      if (num > 1000.0)
      {
        if (finalVelocity > 0.0)
          this.SwitchToLeft(num);
        else
          this.SwitchToRight(num);
      }
      else
      {
        if (this.state_.TranslateX > 24.0)
        {
          if (finalVelocity >= 0.0)
          {
            this.SwitchToLeft(num);
            flag = true;
          }
        }
        else if (this.state_.TranslateX < -24.0 && finalVelocity <= 0.0)
        {
          this.SwitchToRight(num);
          flag = true;
        }
        if (flag)
          return;
        this.ResetToCurrent(num);
      }
    }

    private void OnHorizontalTranslateDelta(double deltaX)
    {
      double actualWidth = this.ActualWidth;
      if (this.leftImgSrc_ == null && deltaX > 0.0 && this.state_.TranslateX > 0.0 || this.rightImgSrc_ == null && deltaX < 0.0 && this.state_.TranslateX < 0.0)
      {
        double num = Math.Max(0.0, 1.0 - 3.0 * Math.Abs(this.state_.TranslateX) / actualWidth);
        deltaX *= num;
      }
      if (deltaX > actualWidth)
        deltaX = actualWidth;
      else if (deltaX < -actualWidth)
        deltaX = -actualWidth;
      this.state_.TranslateX += deltaX;
      this.NotifyImageSwitching((int) -this.state_.TranslateX, Math.Abs(this.state_.TranslateX) / actualWidth);
      this.ApplyTransformState(this.state_);
    }

    private void OnLoaded(object sender, EventArgs e) => this.ResetTransforms();

    private void OnUnloaded(object sender, EventArgs e)
    {
      AppState.Worker.Enqueue((Action) (() => GC.Collect()));
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (this.CenterImage.IsImageScaled || this.IsLocked)
      {
        this.isMoving_ = false;
      }
      else
      {
        if (e.DeltaManipulation == null)
          return;
        double x = e.DeltaManipulation.Translation.X;
        if (Math.Abs(x) <= 0.0)
          return;
        this.isMoving_ = true;
        this.CenterImage.EnableScaling = false;
        if (typeof (Image) == e.OriginalSource.GetType())
          x *= this.CenterImage.ImageScale;
        this.OnHorizontalTranslateDelta(x);
      }
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.CenterImage.EnableScaling = true;
      if (this.IsLocked || !this.isMoving_)
        return;
      this.OnHorizontalGuestureEnd(e.FinalVelocities.LinearVelocity.X);
      this.isMoving_ = false;
    }

    private void OnSizeChanged(object sender, EventArgs e) => this.ResetTransforms();

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ImageSlideViewControl.xaml", UriKind.Relative));
      this.LayoutRootTransform = (TranslateTransform) this.FindName("LayoutRootTransform");
      this.BackgroundRectangle = (Rectangle) this.FindName("BackgroundRectangle");
      this.LeftImage = (Image) this.FindName("LeftImage");
      this.LeftImageTransform = (CompositeTransform) this.FindName("LeftImageTransform");
      this.RightImage = (Image) this.FindName("RightImage");
      this.RightImageTransform = (CompositeTransform) this.FindName("RightImageTransform");
      this.CenterImage = (ImageViewControl) this.FindName("CenterImage");
    }

    public delegate void ImageSwitchingHandler(int direction, double percentage);

    public delegate void ImageSwitchedHandler(int direction);
  }
}
