// Decompiled with JetBrains decompiler
// Type: WhatsApp.ZoomableImageFrame
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable
namespace WhatsApp
{
  public class ZoomableImageFrame : UserControl
  {
    private ZoomableImageFrame.ZoomableImageWrapper imgWrapper_;
    private ZoomableImageFrame.Config settings_;
    private bool pinching_;
    private bool sizeReady_;
    internal Grid LayoutRoot;
    internal Rectangle Bg;
    internal Image Img;
    private bool _contentLoaded;

    public new double Width
    {
      get => base.Width;
      set
      {
        if (ZoomableImageFrame.NearlyEquals(base.Width, value))
          return;
        base.Width = value;
        this.NotifyFrameSizeChanged();
      }
    }

    public new double Height
    {
      get => base.Height;
      set
      {
        if (ZoomableImageFrame.NearlyEquals(base.Width, value))
          return;
        base.Height = value;
        this.NotifyFrameSizeChanged();
      }
    }

    public ZoomableImageFrame.FrameFitModeValue FrameFitMode { get; set; }

    public bool EnableImageScaling { get; set; }

    private bool Ready => this.sizeReady_ && this.imgWrapper_ != null;

    public event ZoomableImageFrame.FrameSizeChangedHandler FrameSizeChanged;

    protected void NotifyFrameSizeChanged()
    {
      if (this.FrameSizeChanged == null)
        return;
      this.FrameSizeChanged();
    }

    private static bool NearlyEquals(double a, double b)
    {
      return Math.Abs(a - b) < ZoomableImageFrame.Config.Precision;
    }

    public ZoomableImageFrame()
    {
      this.InitializeComponent();
      this.Init();
    }

    private void Init()
    {
      this.settings_ = new ZoomableImageFrame.Config();
      this.EnableImageScaling = true;
      this.FrameFitMode = ZoomableImageFrame.FrameFitModeValue.IMAGE_WITHIN_FRAME;
      this.FrameSizeChanged += new ZoomableImageFrame.FrameSizeChangedHandler(this.OnFrameSizeChanged);
      this.FlowDirection = FlowDirection.LeftToRight;
    }

    public void SetImageSource(WriteableBitmap bitmap)
    {
      if (this.imgWrapper_ == null)
        this.imgWrapper_ = new ZoomableImageFrame.ZoomableImageWrapper(this.Img, this.settings_);
      this.imgWrapper_.SetSource(bitmap);
    }

    public bool GetInFrameImageArea(out System.Windows.Point pos, out Size size)
    {
      pos = new System.Windows.Point(0.0, 0.0);
      size = new Size(this.imgWrapper_.PixelWidth, this.imgWrapper_.PixelHeight);
      double num1 = this.imgWrapper_.Left + this.imgWrapper_.ActualWidth;
      double num2 = this.imgWrapper_.Top + this.imgWrapper_.ActualHeight;
      if (this.imgWrapper_.Left > this.Width || num1 < 0.0 || this.imgWrapper_.Top > this.Height || num2 < 0.0)
        return false;
      if (this.imgWrapper_.Left < 0.0)
        pos.X = Math.Abs(this.imgWrapper_.Left) / this.imgWrapper_.Scale;
      if (this.imgWrapper_.Top < 0.0)
        pos.Y = Math.Abs(this.imgWrapper_.Top) / this.imgWrapper_.Scale;
      if (num1 > this.Width)
      {
        double num3 = (this.imgWrapper_.ActualWidth - (num1 - this.Width)) / this.imgWrapper_.Scale;
        size.Width = num3 - pos.X;
      }
      if (num2 > this.Height)
      {
        double num4 = (this.imgWrapper_.ActualHeight - (num2 - this.Height)) / this.imgWrapper_.Scale;
        size.Height = num4 - pos.Y;
      }
      return true;
    }

    public void SetScaleLimit(double? upperLimit = null, double? lowerLimit = null)
    {
      double? nullable1;
      if (upperLimit.HasValue && lowerLimit.HasValue)
      {
        double? nullable2 = upperLimit;
        nullable1 = lowerLimit;
        if ((nullable2.GetValueOrDefault() < nullable1.GetValueOrDefault() ? (nullable2.HasValue & nullable1.HasValue ? 1 : 0) : 0) != 0)
          throw new ArgumentException("upper limit is less than lower limit.");
      }
      if (upperLimit.HasValue && !lowerLimit.HasValue)
      {
        nullable1 = upperLimit;
        double stableLowerScaleLimit = this.imgWrapper_.StableLowerScaleLimit;
        if ((nullable1.GetValueOrDefault() < stableLowerScaleLimit ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
          lowerLimit = upperLimit;
      }
      if (lowerLimit.HasValue && !upperLimit.HasValue)
      {
        nullable1 = lowerLimit;
        double stableUpperScaleLimit = this.imgWrapper_.StableUpperScaleLimit;
        if ((nullable1.GetValueOrDefault() > stableUpperScaleLimit ? (nullable1.HasValue ? 1 : 0) : 0) != 0)
          upperLimit = lowerLimit;
      }
      if (upperLimit.HasValue)
        this.settings_.StableUpperScaleLimitToInitStateRatio = upperLimit.Value / this.imgWrapper_.InitScale;
      if (!lowerLimit.HasValue)
        return;
      this.settings_.StableLowerScaleLimitToInitStateRatio = lowerLimit.Value / this.imgWrapper_.InitScale;
    }

    public static void GetPinchPositions(
      UIElement elem,
      PinchGestureEventArgs e,
      out System.Windows.Point pos0,
      out System.Windows.Point pos1)
    {
      pos0 = elem != null ? e.GetPosition(elem, 0) : throw new ArgumentNullException();
      pos1 = e.GetPosition(elem, 1);
    }

    public static void GetPinchPositions(
      UIElement elem,
      PinchStartedGestureEventArgs e,
      out System.Windows.Point pos0,
      out System.Windows.Point pos1)
    {
      pos0 = elem != null ? e.GetPosition(elem, 0) : throw new ArgumentNullException();
      pos1 = e.GetPosition(elem, 1);
    }

    private void OnFrameSizeChanged()
    {
      if (double.IsNaN(this.Width) || double.IsNaN(this.Height) || ZoomableImageFrame.NearlyEquals(this.Width, 0.0) || ZoomableImageFrame.NearlyEquals(this.Height, 0.0))
        return;
      this.sizeReady_ = true;
      this.ResetImageInitState();
      this.RestoreInitState();
    }

    private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
    {
      if (!this.Ready)
        return;
      if (this.EnableImageScaling)
      {
        System.Windows.Point pos0;
        System.Windows.Point pos1;
        try
        {
          ZoomableImageFrame.GetPinchPositions((UIElement) this, e, out pos0, out pos1);
        }
        catch (Exception ex)
        {
          return;
        }
        this.imgWrapper_.ProcessPinchStarted(pos0, pos1);
      }
      this.pinching_ = true;
    }

    private void OnPinchDelta(object sender, PinchGestureEventArgs e)
    {
      if (!this.Ready || !this.pinching_)
        return;
      if (!this.EnableImageScaling)
        return;
      System.Windows.Point pos0;
      System.Windows.Point pos1;
      try
      {
        ZoomableImageFrame.GetPinchPositions((UIElement) this, e, out pos0, out pos1);
      }
      catch (Exception ex)
      {
        string context = string.Format("OnPinchDelta: event distance ratio={0}, imgWrapper position={1}", (object) e.DistanceRatio, (object) this.imgWrapper_.Position);
        Log.LogException(ex, context);
        return;
      }
      this.imgWrapper_.ProcessPinchDelta(e.DistanceRatio, pos0, pos1);
    }

    private void OnPinchCompleted(object sender, PinchGestureEventArgs e)
    {
      if (!this.Ready)
        return;
      this.imgWrapper_.ProcessPinchCompleted();
      this.AdjustImagePosition();
      this.pinching_ = false;
    }

    private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
    {
    }

    private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
    {
      if (!this.Ready)
        return;
      double num1 = 1.0;
      double num2 = 1.0;
      double width = this.Width;
      if (e.HorizontalChange > 0.0)
      {
        if (this.imgWrapper_.Left > 0.0)
        {
          double num3 = Math.Max(0.0, 1.0 - 3.0 * this.imgWrapper_.Left / width);
          num1 = num3 * num3;
        }
      }
      else if (e.HorizontalChange < 0.0)
      {
        double num4 = this.imgWrapper_.Left + this.imgWrapper_.ActualWidth;
        if (num4 < width)
        {
          double num5 = Math.Max(0.0, 1.0 - 3.0 * (width - num4) / width);
          num1 = num5 * num5;
        }
      }
      double height = this.Height;
      if (e.VerticalChange > 0.0)
      {
        if (this.imgWrapper_.Top > 0.0)
        {
          double num6 = Math.Max(0.0, 1.0 - 3.0 * this.imgWrapper_.Top / height);
          num2 = num6 * num6;
        }
      }
      else if (e.VerticalChange < 0.0)
      {
        double num7 = this.imgWrapper_.Top + this.imgWrapper_.ActualHeight;
        if (num7 < height)
        {
          double num8 = Math.Max(0.0, 1.0 - 3.0 * (height - num7) / height);
          num2 = num8 * num8;
        }
      }
      this.imgWrapper_.Position = new System.Windows.Point(this.imgWrapper_.Left + e.HorizontalChange * num1, this.imgWrapper_.Top + e.VerticalChange * num2);
    }

    private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
    {
      if (!this.Ready)
        return;
      if (this.FrameFitMode == ZoomableImageFrame.FrameFitModeValue.IMAGE_WITHIN_FRAME && !this.imgWrapper_.Scaled)
        this.RestoreInitState();
      else
        this.AdjustImagePosition();
    }

    private void OnDoubleTap(object sender, GestureEventArgs e)
    {
      if (!this.Ready)
        return;
      if (this.imgWrapper_.Scaled)
      {
        this.imgWrapper_.RestoreInitState();
      }
      else
      {
        System.Windows.Point position1 = e.GetPosition((UIElement) this.Img);
        if (position1.X <= 0.0 || position1.Y <= 0.0 || position1.X >= this.imgWrapper_.PixelWidth || position1.Y >= this.imgWrapper_.PixelHeight)
          return;
        System.Windows.Point position2 = e.GetPosition((UIElement) this);
        if (position2.X <= 0.0 || position2.Y <= 0.0 || position2.X >= this.Width || position2.Y >= this.Height)
          return;
        this.imgWrapper_.Position = new System.Windows.Point(this.Width / 2.0 - position1.X * this.imgWrapper_.StableUpperScaleLimit, this.Height / 2.0 - position1.Y * this.imgWrapper_.StableUpperScaleLimit);
        this.imgWrapper_.Scale = this.imgWrapper_.StableUpperScaleLimit;
        this.AdjustImagePosition();
      }
    }

    private void ResetImageInitState(bool forceScale = false)
    {
      double scale = 1.0;
      double left = 0.0;
      double top = 0.0;
      this.CalcFrameFitImageScaleAndPos(out scale, out left, out top, forceScale);
      this.imgWrapper_.SetInitState(scale, left, top);
    }

    protected void CalcFrameFitImageScaleAndPos(
      out double scale,
      out double left,
      out double top,
      bool forceScale = false)
    {
      scale = 1.0;
      left = 0.0;
      top = 0.0;
      double pixelWidth = this.imgWrapper_.PixelWidth;
      double pixelHeight = this.imgWrapper_.PixelHeight;
      double width = this.Width;
      double height = this.Height;
      if (this.FrameFitMode == ZoomableImageFrame.FrameFitModeValue.FRAME_WITHIN_IMAGE)
        forceScale = true;
      if (((pixelWidth > width ? 1 : (pixelHeight > height ? 1 : 0)) | (forceScale ? 1 : 0)) != 0)
      {
        double num1 = width / pixelWidth;
        double num2 = height / pixelHeight;
        if (this.FrameFitMode == ZoomableImageFrame.FrameFitModeValue.IMAGE_WITHIN_FRAME && num1 < num2 || this.FrameFitMode == ZoomableImageFrame.FrameFitModeValue.FRAME_WITHIN_IMAGE && num1 > num2)
        {
          scale = num1;
          top = (height - pixelHeight * scale) / 2.0;
        }
        else
        {
          scale = num2;
          left = (width - pixelWidth * scale) / 2.0;
        }
      }
      else
      {
        left = (width - pixelWidth) / 2.0;
        top = (height - pixelHeight) / 2.0;
      }
    }

    private void RestoreInitState() => this.imgWrapper_.RestoreInitState();

    private void AdjustImagePosition()
    {
      double x = this.imgWrapper_.Left;
      double y = this.imgWrapper_.Top;
      double actualHeight = this.imgWrapper_.ActualHeight;
      double height = this.Height;
      if (actualHeight < height)
        y = (height - actualHeight) / 2.0;
      else if (this.imgWrapper_.Top > 0.0)
        y = 0.0;
      else if (this.imgWrapper_.Top + actualHeight < height)
        y = height - actualHeight;
      double actualWidth = this.imgWrapper_.ActualWidth;
      double width = this.Width;
      if (actualWidth < width)
        x = (width - actualWidth) / 2.0;
      else if (this.imgWrapper_.Left > 0.0)
        x = 0.0;
      else if (this.imgWrapper_.Left + actualWidth < width)
        x = width - actualWidth;
      this.imgWrapper_.Position = new System.Windows.Point(x, y);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ZoomableImageFrame.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Bg = (Rectangle) this.FindName("Bg");
      this.Img = (Image) this.FindName("Img");
    }

    private class ZoomableImageWrapper
    {
      private Image img_;
      private ZoomableImageFrame.Config settings_;
      private System.Windows.Point position_;
      private double? scale_;
      private double initScale_ = 1.0;
      private System.Windows.Point workingFinger0_;
      private System.Windows.Point workingFinger1_;
      private double workingScale_ = 1.0;

      public System.Windows.Point Position
      {
        get => this.position_;
        set
        {
          if (ZoomableImageFrame.NearlyEquals(this.position_.X, value.X) && ZoomableImageFrame.NearlyEquals(this.position_.Y, value.Y))
            return;
          this.position_ = value;
          this.NotifyPositionChanged(true, true);
        }
      }

      public double Left
      {
        get => this.position_.X;
        set
        {
          if (ZoomableImageFrame.NearlyEquals(this.position_.X, value))
            return;
          this.position_.X = value;
          this.NotifyPositionChanged(true, false);
        }
      }

      public double Top
      {
        get => this.position_.Y;
        set
        {
          if (ZoomableImageFrame.NearlyEquals(this.position_.Y, value))
            return;
          this.position_.Y = value;
          this.NotifyPositionChanged(false, true);
        }
      }

      public double PixelWidth { get; private set; }

      public double PixelHeight { get; private set; }

      public double ActualWidth => this.PixelWidth * this.Scale;

      public double ActualHeight => this.PixelHeight * this.Scale;

      public double Scale
      {
        get => !this.scale_.HasValue ? 1.0 : this.scale_.Value;
        set
        {
          if (this.scale_.HasValue && ZoomableImageFrame.NearlyEquals(this.scale_.Value, value))
            return;
          this.scale_ = new double?(value);
          this.NotifyScaleChanged();
        }
      }

      public bool Scaled
      {
        get
        {
          return this.scale_.HasValue && !ZoomableImageFrame.NearlyEquals(this.scale_.Value, this.InitScale);
        }
      }

      public double InitScale
      {
        get => this.initScale_;
        private set
        {
          if (ZoomableImageFrame.NearlyEquals(this.initScale_, value))
            return;
          this.initScale_ = value;
        }
      }

      public System.Windows.Point InitPosition { get; private set; }

      public double StableUpperScaleLimit
      {
        get => this.InitScale * this.settings_.StableUpperScaleLimitToInitStateRatio;
      }

      public double StableLowerScaleLimit
      {
        get => this.InitScale * this.settings_.StableLowerScaleLimitToInitStateRatio;
      }

      private double ElasticScaleUpperLimit
      {
        get => this.StableUpperScaleLimit * this.settings_.ElasticToStableUpperScaleLimitRatio;
      }

      private double ElasticScaleLowerLimit
      {
        get => this.StableLowerScaleLimit * this.settings_.ElasticToStableLowerScaleLimitRatio;
      }

      public event ZoomableImageFrame.ZoomableImageWrapper.PositionChangedHandler PositionChanged;

      protected void NotifyPositionChanged(bool posXChanged, bool posYChanged)
      {
        if (this.PositionChanged == null)
          return;
        this.PositionChanged(posXChanged, posYChanged);
      }

      public event ZoomableImageFrame.ZoomableImageWrapper.ScaleChangedHandler ScaleChanged;

      protected void NotifyScaleChanged()
      {
        if (this.ScaleChanged == null)
          return;
        this.ScaleChanged();
      }

      public ZoomableImageWrapper(Image img, ZoomableImageFrame.Config zoomSettings)
      {
        this.img_ = img;
        this.settings_ = zoomSettings;
        this.ScaleChanged += new ZoomableImageFrame.ZoomableImageWrapper.ScaleChangedHandler(this.OnScaleChanged);
        this.PositionChanged += new ZoomableImageFrame.ZoomableImageWrapper.PositionChangedHandler(this.OnPositionChanged);
      }

      public void SetSource(WriteableBitmap bitmap)
      {
        this.img_.Source = (System.Windows.Media.ImageSource) bitmap;
        if (bitmap == null)
        {
          this.PixelWidth = this.PixelHeight = 0.0;
        }
        else
        {
          this.PixelWidth = (double) bitmap.PixelWidth;
          this.PixelHeight = (double) bitmap.PixelHeight;
        }
      }

      public void SetInitState(double scale, double left, double top)
      {
        this.InitScale = scale;
        this.InitPosition = new System.Windows.Point(left, top);
      }

      public void RestoreInitState()
      {
        this.Scale = this.InitScale;
        this.Position = this.InitPosition;
      }

      private void OnPositionChanged(bool posXChanged, bool posYChanged)
      {
        if (posXChanged)
          ((CompositeTransform) this.img_.RenderTransform).TranslateX = this.Left;
        if (!posYChanged)
          return;
        ((CompositeTransform) this.img_.RenderTransform).TranslateY = this.Top;
      }

      private void OnScaleChanged()
      {
        ((CompositeTransform) this.img_.RenderTransform).ScaleX = this.Scale;
        ((CompositeTransform) this.img_.RenderTransform).ScaleY = this.Scale;
      }

      public void ProcessPinchStarted(System.Windows.Point finger0, System.Windows.Point finger1)
      {
        this.workingFinger0_ = finger0;
        this.workingFinger1_ = finger1;
        this.workingScale_ = 1.0;
      }

      public void ProcessPinchDelta(double distanceRatio, System.Windows.Point newFinger0, System.Windows.Point newFinger1)
      {
        double num1 = distanceRatio / this.workingScale_;
        double num2 = this.Scale * num1;
        if (num2 < this.ElasticScaleLowerLimit || num2 > this.ElasticScaleUpperLimit)
          return;
        double num3 = newFinger0.X + (this.Left - this.workingFinger0_.X) * num1;
        double num4 = newFinger0.Y + (this.Top - this.workingFinger0_.Y) * num1;
        double num5 = newFinger1.X + (this.Left - this.workingFinger1_.X) * num1;
        double num6 = newFinger1.Y + (this.Top - this.workingFinger1_.Y) * num1;
        System.Windows.Point point = new System.Windows.Point((num3 + num5) / 2.0, (num4 + num6) / 2.0);
        this.workingScale_ = distanceRatio;
        this.workingFinger0_ = newFinger0;
        this.workingFinger1_ = newFinger1;
        this.Scale = num2;
        this.Position = point;
      }

      public void ProcessPinchCompleted()
      {
        if (this.Scale >= this.StableLowerScaleLimit && this.Scale <= this.StableUpperScaleLimit)
          return;
        this.ElasticScaleToStableState();
      }

      public void ElasticScaleToStableState()
      {
        double num1;
        if (this.Scale < this.StableLowerScaleLimit)
        {
          num1 = this.StableLowerScaleLimit;
        }
        else
        {
          if (this.Scale <= this.StableUpperScaleLimit)
            return;
          num1 = this.StableUpperScaleLimit;
        }
        double num2 = num1 / this.Scale;
        double distanceRatio = num2 * this.workingScale_;
        double num3 = this.workingFinger0_.X - this.workingFinger1_.X;
        double num4 = this.workingFinger0_.Y - this.workingFinger1_.Y;
        double num5 = 1.0 - num2;
        double num6 = num3 * num5 / 2.0;
        double num7 = num4 * (1.0 - num2) / 2.0;
        System.Windows.Point newFinger0 = new System.Windows.Point(this.workingFinger0_.X - num6, this.workingFinger0_.Y - num7);
        System.Windows.Point newFinger1 = new System.Windows.Point(this.workingFinger1_.X + num6, this.workingFinger1_.Y + num7);
        this.ProcessPinchDelta(distanceRatio, newFinger0, newFinger1);
      }

      public delegate void PositionChangedHandler(bool posXChanged, bool posYChanged);

      public delegate void ScaleChangedHandler();
    }

    private class Config
    {
      public double StableUpperScaleLimitToInitStateRatio { get; set; }

      public double ElasticToStableUpperScaleLimitRatio { get; set; }

      public double StableLowerScaleLimitToInitStateRatio { get; set; }

      public double ElasticToStableLowerScaleLimitRatio { get; set; }

      public static double Precision { get; set; }

      public Config()
      {
        this.StableUpperScaleLimitToInitStateRatio = 3.0;
        this.ElasticToStableUpperScaleLimitRatio = 1.33;
        this.StableLowerScaleLimitToInitStateRatio = 1.0;
        this.ElasticToStableLowerScaleLimitRatio = 0.33;
        ZoomableImageFrame.Config.Precision = 0.0001;
      }
    }

    public enum FrameFitModeValue
    {
      IMAGE_WITHIN_FRAME,
      FRAME_WITHIN_IMAGE,
    }

    public delegate void FrameSizeChangedHandler();
  }
}
