// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageViewControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace WhatsApp
{
  public class ImageViewControl : UserControl
  {
    private BitmapSource imgSrc_;
    private double imgPixelWidth_;
    private double imgPixelHeight_;
    private double minScale_ = 1.0;
    private double maxScale_ = 1.0;
    private ImageViewControl.TransformState state_;
    private double initialScale_ = 1.0;
    private bool enableScaling_ = true;
    private bool isEverLoaded_;
    private bool isPinching_;
    private bool isPinchingCloser_;
    private System.Windows.Point relativeMidpoint_;
    private System.Windows.Point screenMidpoint_;
    private Size viewportSize_;
    private PageOrientation orientation_;
    private VerticalAlignment contentVerticalAlignment_ = VerticalAlignment.Center;
    private HorizontalAlignment contentHorizontalAlignment_ = HorizontalAlignment.Center;
    internal ViewportControl Viewport;
    internal Canvas Canvas;
    internal Image Image;
    internal CompositeTransform ImageTransform;
    private bool _contentLoaded;

    public event EventHandler ImagePinchStarted;

    protected void NotifyImagePinchStarted()
    {
      if (this.ImagePinchStarted == null)
        return;
      this.ImagePinchStarted((object) this, new EventArgs());
    }

    public event EventHandler ImageManipulationEnded;

    protected void NotifyImageManipulationEnded()
    {
      if (this.ImageManipulationEnded == null)
        return;
      this.ImageManipulationEnded((object) this, new EventArgs());
    }

    public BitmapSource ImageSource
    {
      set
      {
        if (this.imgSrc_ == value)
          return;
        this.Image.Source = (System.Windows.Media.ImageSource) (this.imgSrc_ = (BitmapSource) null);
        if (value != null)
          this.Image.Source = (System.Windows.Media.ImageSource) (this.imgSrc_ = value);
        if (this.imgSrc_ == null)
        {
          this.imgPixelWidth_ = this.imgPixelHeight_ = 0.0;
        }
        else
        {
          this.imgPixelWidth_ = (double) this.imgSrc_.PixelWidth;
          this.imgPixelHeight_ = (double) this.imgSrc_.PixelHeight;
        }
        if (!this.isEverLoaded_)
          return;
        this.SetupInitialDisplay();
      }
    }

    public double ImageScale => this.state_ != null ? this.state_.Scale : 1.0;

    public double InitialScale => this.initialScale_;

    public bool IsImageScaled => Math.Abs(this.ImageScale - this.initialScale_) > 0.0001;

    public bool EnableScaling
    {
      get => this.enableScaling_;
      set
      {
        if (this.enableScaling_ == value)
          return;
        this.enableScaling_ = value;
        if (this.enableScaling_ || !this.IsImageScaled)
          return;
        this.SetupInitialDisplay();
      }
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

    public ImageViewControl() => this.InitializeComponent();

    public void SetContentAlignments(
      HorizontalAlignment horizontalAlignment,
      VerticalAlignment verticalAlignment,
      bool updateImmediately = true)
    {
      this.contentHorizontalAlignment_ = horizontalAlignment;
      this.contentVerticalAlignment_ = verticalAlignment;
      if (!updateImmediately)
        return;
      this.SetupInitialDisplay();
    }

    public void SetupInitialDisplay()
    {
      if (this.imgSrc_ == null)
        return;
      double num = Math.Min(this.ActualWidth / this.imgPixelWidth_, this.ActualHeight / this.imgPixelHeight_);
      this.maxScale_ = 4.0 * num;
      this.minScale_ = this.initialScale_ = num;
      this.state_ = new ImageViewControl.TransformState(this.imgPixelWidth_, this.imgPixelHeight_)
      {
        Scale = num
      };
      this.ApplyTransformState(this.state_, true);
    }

    private void ApplyTransformState(ImageViewControl.TransformState state, bool center)
    {
      this.ImageTransform.ScaleX = double.IsInfinity(state.Scale) || double.IsNaN(state.Scale) ? (this.ImageTransform.ScaleY = 1.0) : (this.ImageTransform.ScaleY = state.Scale);
      double width = this.Canvas.Width = this.state_.RenderWidth;
      double height = this.Canvas.Height = this.state_.RenderHeight;
      this.Viewport.Bounds = new Rect(0.0, 0.0, width, height);
      double a = (height - this.ActualHeight) / 2.0;
      System.Windows.Point location;
      if (center)
      {
        location = new System.Windows.Point(Math.Round((width - this.ActualWidth) / 2.0), Math.Round(a));
      }
      else
      {
        System.Windows.Point point = new System.Windows.Point(width * this.relativeMidpoint_.X, height * this.relativeMidpoint_.Y);
        location = new System.Windows.Point(point.X - this.screenMidpoint_.X, point.Y - this.screenMidpoint_.Y);
      }
      this.Viewport.SetViewportOrigin(location);
      if (a < 0.0)
      {
        switch (this.contentVerticalAlignment_)
        {
          case VerticalAlignment.Top:
            this.ImageTransform.TranslateY = a;
            break;
          case VerticalAlignment.Bottom:
            this.ImageTransform.TranslateY = -a;
            break;
          default:
            this.ImageTransform.TranslateY = 0.0;
            break;
        }
      }
      else
        this.ImageTransform.TranslateY = 0.0;
    }

    private void CoerceScale(double scale, bool recompute)
    {
      if (recompute && this.imgSrc_ != null)
        this.minScale_ = Math.Min(this.Viewport.ActualWidth / this.imgPixelWidth_, this.Viewport.ActualHeight / this.imgPixelHeight_);
      this.state_.Scale = Math.Min(this.maxScale_, Math.Max(this.minScale_, scale));
      this.ApplyTransformState(this.state_, false);
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.isPinching_ = false;
      this.isPinchingCloser_ = false;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (e.PinchManipulation != null)
      {
        if (!this.EnableScaling)
          return;
        if (!this.isPinching_)
        {
          this.NotifyImagePinchStarted();
          this.isPinching_ = true;
          System.Windows.Point center = e.PinchManipulation.Original.Center;
          this.relativeMidpoint_ = new System.Windows.Point(center.X / this.Image.ActualWidth, center.Y / this.Image.ActualHeight);
          this.screenMidpoint_ = this.Image.TransformToVisual((UIElement) this.Viewport).Transform(center);
        }
        this.CoerceScale(this.state_.Scale * e.PinchManipulation.DeltaScale, false);
        this.isPinchingCloser_ = e.PinchManipulation.DeltaScale < 1.0;
        e.Handled = true;
      }
      else
      {
        if (!this.isPinching_)
          return;
        this.isPinching_ = false;
        this.isPinchingCloser_ = false;
      }
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      if (this.isPinchingCloser_ && Math.Abs(this.state_.Scale - this.initialScale_) < 0.1)
        this.SetupInitialDisplay();
      this.NotifyImageManipulationEnded();
      this.isPinching_ = false;
      this.isPinchingCloser_ = false;
    }

    private void OnViewportChanged(object sender, ViewportChangedEventArgs e)
    {
      Size size;
      ref Size local = ref size;
      Rect viewport = this.Viewport.Viewport;
      double width = viewport.Width;
      viewport = this.Viewport.Viewport;
      double height = viewport.Height;
      local = new Size(width, height);
      if (!(size != this.viewportSize_))
        return;
      this.viewportSize_ = size;
      this.SetupInitialDisplay();
    }

    private void OnManipulationStateChanged(object sender, ManipulationStateChangedEventArgs e)
    {
      if (this.Viewport.ManipulationState != System.Windows.Controls.Primitives.ManipulationState.Idle)
        return;
      Rect viewport = this.Viewport.Viewport;
      Rect bounds = this.Viewport.Bounds;
      if (viewport.Left >= bounds.Left && viewport.Top >= bounds.Top && viewport.Right <= bounds.Right && viewport.Bottom <= bounds.Bottom)
        return;
      this.Viewport.SetViewportOrigin(new System.Windows.Point(viewport.Left, viewport.Top));
    }

    private void OnDoubleTap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.IsImageScaled)
      {
        this.SetupInitialDisplay();
      }
      else
      {
        if (!this.EnableScaling)
          return;
        System.Windows.Point position = e.GetPosition((UIElement) this.Image);
        this.relativeMidpoint_ = new System.Windows.Point(position.X / this.Image.ActualWidth, position.Y / this.Image.ActualHeight);
        this.screenMidpoint_ = this.Image.TransformToVisual((UIElement) this.Viewport).Transform(position);
        this.CoerceScale(this.maxScale_, false);
      }
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => this.isEverLoaded_ = true;

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ImageViewControl.xaml", UriKind.Relative));
      this.Viewport = (ViewportControl) this.FindName("Viewport");
      this.Canvas = (Canvas) this.FindName("Canvas");
      this.Image = (Image) this.FindName("Image");
      this.ImageTransform = (CompositeTransform) this.FindName("ImageTransform");
    }

    public class TransformState
    {
      public double OriginalPixelWidth;
      public double OriginalPixelHeight;
      public double Scale = 1.0;
      public double TranslateX;
      public double TranslateY;

      public double RenderWidth => this.OriginalPixelWidth * this.Scale;

      public double RenderHeight => this.OriginalPixelHeight * this.Scale;

      public TransformState()
      {
      }

      public TransformState(double originalPixelWidth, double originalPixelHeight)
      {
        this.OriginalPixelWidth = originalPixelWidth;
        this.OriginalPixelHeight = originalPixelHeight;
      }

      public virtual void Copy(ImageViewControl.TransformState other)
      {
        this.OriginalPixelWidth = other.OriginalPixelWidth;
        this.OriginalPixelHeight = other.OriginalPixelHeight;
        this.Scale = other.Scale;
        this.TranslateX = other.TranslateX;
        this.TranslateY = other.TranslateY;
      }

      public virtual void SetScale(
        double newScale,
        ref System.Windows.Point originOnScreen,
        System.Windows.Point originOnImage,
        System.Windows.Point translateLowBound,
        System.Windows.Point translateHighBound,
        Size containerSize)
      {
        double val2_1 = originOnScreen.X - originOnImage.X * newScale;
        double val2_2 = originOnScreen.Y - originOnImage.Y * newScale;
        if (newScale <= this.Scale)
        {
          double renderWidth = this.RenderWidth;
          val2_1 = renderWidth <= containerSize.Width ? (containerSize.Width - renderWidth) * 0.5 : Math.Max(translateLowBound.X, Math.Min(translateHighBound.X, val2_1));
          double renderHeight = this.RenderHeight;
          val2_2 = renderHeight <= containerSize.Height ? (containerSize.Height - renderHeight) * 0.5 : Math.Max(translateLowBound.Y, Math.Min(translateHighBound.Y, val2_2));
          originOnScreen.X = val2_1 + originOnImage.X * newScale;
          originOnScreen.Y = val2_2 + originOnImage.Y * newScale;
        }
        this.Scale = newScale;
        this.TranslateX = val2_1;
        this.TranslateY = val2_2;
      }
    }
  }
}
