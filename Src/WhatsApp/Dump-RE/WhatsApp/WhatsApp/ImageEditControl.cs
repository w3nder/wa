// Decompiled with JetBrains decompiler
// Type: WhatsApp.ImageEditControl
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WhatsApp.WaCollections;

#nullable disable
namespace WhatsApp
{
  public class ImageEditControl : UserControl
  {
    private const double GapBetweenCropAndEdge = 56.0;
    private double maxScale_ = 1.0;
    private double minScale_ = 1.0;
    private ImageEditControl.CroppingMode cropMode_;
    private double cropRatio_ = 1.0;
    private Size? minRelativeCropSize_;
    private BitmapSource imgSrc_;
    private ImageEditControl.TransformState state_;
    private System.Windows.Point? presetCropPos_;
    private Size? presetCropSize_;
    private bool isLoaded_;
    private bool isMoving_;
    private bool isPinching_;
    private System.Windows.Point pinchCenterOnScreen_;
    private System.Windows.Point pinchCenterOnImage_;
    private IDisposable sbSub_;
    internal Image BackImage;
    internal CompositeTransform BackImageTransform;
    internal Rectangle CroppingMask;
    internal Border CroppingBorder;
    internal CompositeTransform CroppingPanelTransform;
    internal Grid CroppingGripsPanel;
    internal Image FrontImage;
    internal CompositeTransform FrontImageTransform;
    internal CompositeTransform FrontImageClipTransform;
    internal CompositeTransform HorizontalGridLine0Transform;
    internal CompositeTransform HorizontalGridLine1Transform;
    internal CompositeTransform VerticalGridLine0Transform;
    internal CompositeTransform VerticalGridLine1Transform;
    internal Ellipse TopLeftGrip;
    internal TranslateTransform TopLeftGripTranslate;
    internal Ellipse TopRightGrip;
    internal TranslateTransform TopRightGripTranslate;
    internal Ellipse BottomLeftGrip;
    internal TranslateTransform BottomLeftGripTranslate;
    internal Ellipse BottomRightGrip;
    internal TranslateTransform BottomRightGripTranslate;
    private bool _contentLoaded;

    public ImageEditControl.CroppingMode CropMode
    {
      get => this.cropMode_;
      set
      {
        if (this.cropMode_ == value)
          return;
        this.cropMode_ = value;
        this.FrontImage.Source = this.cropMode_ == ImageEditControl.CroppingMode.None ? (System.Windows.Media.ImageSource) null : (System.Windows.Media.ImageSource) this.imgSrc_;
        this.ResetControls();
      }
    }

    public double CropRatio
    {
      get => this.cropRatio_;
      set => this.cropRatio_ = value;
    }

    public ImageEditControl.RotationMode RotateMode { get; set; }

    public Size? MinRelativeCropSize
    {
      get => this.minRelativeCropSize_;
      set
      {
        Size? relativeCropSize = this.minRelativeCropSize_;
        Size? nullable = value;
        if ((relativeCropSize.HasValue == nullable.HasValue ? (relativeCropSize.HasValue ? (relativeCropSize.GetValueOrDefault() != nullable.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
          return;
        this.minRelativeCropSize_ = value;
      }
    }

    public BitmapSource ImageSource
    {
      set
      {
        if (this.imgSrc_ == value)
          return;
        this.BackImage.Source = (System.Windows.Media.ImageSource) (this.imgSrc_ = value);
        if (this.CropMode != ImageEditControl.CroppingMode.None)
          this.FrontImage.Source = (System.Windows.Media.ImageSource) this.imgSrc_;
        if (this.imgSrc_ == null)
          this.state_ = (ImageEditControl.TransformState) null;
        if (!this.isLoaded_)
          return;
        this.ResetTransform();
      }
    }

    public double CurrentScale => this.state_ != null ? this.state_.Scale : 1.0;

    public ImageEditControl()
    {
      this.InitializeComponent();
      this.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
    }

    public void SetCropping(double cropRatio, Size? targetSize = null, bool transit = false)
    {
      this.cropRatio_ = cropRatio;
      if (!this.isLoaded_)
        return;
      this.ResetTransform(targetSize: targetSize, transit: transit);
      this.ResetControls(targetSize);
    }

    public void SetCropping(System.Windows.Point cropPos, Size cropSize, bool relative, Size? targetSize = null)
    {
      if (this.CropMode != ImageEditControl.CroppingMode.Custom)
        return;
      if (relative)
      {
        cropPos.X *= this.state_.OriginalPixelWidth;
        cropPos.Y *= this.state_.OriginalPixelHeight;
        cropSize.Width *= this.state_.OriginalPixelWidth;
        cropSize.Height *= this.state_.OriginalPixelHeight;
      }
      if (this.isLoaded_)
      {
        this.ResetTransform(new System.Windows.Point?(cropPos), new Size?(cropSize), targetSize);
        this.ResetControls(targetSize);
      }
      else
      {
        this.presetCropPos_ = new System.Windows.Point?(cropPos);
        this.presetCropSize_ = new Size?(cropSize);
      }
    }

    public Pair<System.Windows.Point, Size> GetCroppingState(bool relative)
    {
      if (this.CropMode == ImageEditControl.CroppingMode.None || this.state_ == null || !this.state_.IsCropped)
        return (Pair<System.Windows.Point, Size>) null;
      System.Windows.Point first = new System.Windows.Point(this.state_.CropX, this.state_.CropY);
      Size second = new Size(this.state_.CropPixelWidth, this.state_.CropPixelHeight);
      if (relative)
      {
        first.X /= this.state_.OriginalPixelWidth;
        first.Y /= this.state_.OriginalPixelHeight;
        second.Width /= this.state_.OriginalPixelWidth;
        second.Height /= this.state_.OriginalPixelHeight;
      }
      return new Pair<System.Windows.Point, Size>(first, second);
    }

    private void TransitToTransformState(
      ImageEditControl.TransformState targetState,
      bool updateImageSizeAndPos,
      bool updateCropSizeAndPos)
    {
      this.sbSub_.SafeDispose();
      this.sbSub_ = (IDisposable) null;
      Storyboard sb = new Storyboard();
      TimeSpan timeSpan = TimeSpan.FromMilliseconds(200.0);
      if (updateImageSizeAndPos)
      {
        DoubleAnimation doubleAnimation1 = new DoubleAnimation();
        doubleAnimation1.From = new double?(this.BackImageTransform.ScaleX);
        doubleAnimation1.To = new double?(targetState.Scale);
        doubleAnimation1.Duration = (Duration) timeSpan;
        DoubleAnimation element1 = doubleAnimation1;
        Storyboard.SetTarget((Timeline) element1, (DependencyObject) this.BackImageTransform);
        Storyboard.SetTargetProperty((Timeline) element1, new PropertyPath("ScaleX", new object[0]));
        sb.Children.Add((Timeline) element1);
        DoubleAnimation doubleAnimation2 = new DoubleAnimation();
        doubleAnimation2.From = new double?(this.BackImageTransform.ScaleX);
        doubleAnimation2.To = new double?(targetState.Scale);
        doubleAnimation2.Duration = (Duration) timeSpan;
        DoubleAnimation element2 = doubleAnimation2;
        Storyboard.SetTarget((Timeline) element2, (DependencyObject) this.BackImageTransform);
        Storyboard.SetTargetProperty((Timeline) element2, new PropertyPath("ScaleY", new object[0]));
        sb.Children.Add((Timeline) element2);
        DoubleAnimation doubleAnimation3 = new DoubleAnimation();
        doubleAnimation3.From = new double?(this.BackImageTransform.ScaleX);
        doubleAnimation3.To = new double?(targetState.Scale);
        doubleAnimation3.Duration = (Duration) timeSpan;
        DoubleAnimation element3 = doubleAnimation3;
        Storyboard.SetTarget((Timeline) element3, (DependencyObject) this.FrontImageTransform);
        Storyboard.SetTargetProperty((Timeline) element3, new PropertyPath("ScaleX", new object[0]));
        sb.Children.Add((Timeline) element3);
        DoubleAnimation doubleAnimation4 = new DoubleAnimation();
        doubleAnimation4.From = new double?(this.BackImageTransform.ScaleX);
        doubleAnimation4.To = new double?(targetState.Scale);
        doubleAnimation4.Duration = (Duration) timeSpan;
        DoubleAnimation element4 = doubleAnimation4;
        Storyboard.SetTarget((Timeline) element4, (DependencyObject) this.FrontImageTransform);
        Storyboard.SetTargetProperty((Timeline) element4, new PropertyPath("ScaleY", new object[0]));
        sb.Children.Add((Timeline) element4);
        DoubleAnimation doubleAnimation5 = new DoubleAnimation();
        doubleAnimation5.From = new double?(this.BackImageTransform.TranslateX);
        doubleAnimation5.To = new double?(targetState.TranslateX);
        doubleAnimation5.Duration = (Duration) timeSpan;
        DoubleAnimation element5 = doubleAnimation5;
        Storyboard.SetTarget((Timeline) element5, (DependencyObject) this.BackImageTransform);
        Storyboard.SetTargetProperty((Timeline) element5, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element5);
        DoubleAnimation doubleAnimation6 = new DoubleAnimation();
        doubleAnimation6.From = new double?(this.BackImageTransform.TranslateX);
        doubleAnimation6.To = new double?(targetState.TranslateX);
        doubleAnimation6.Duration = (Duration) timeSpan;
        DoubleAnimation element6 = doubleAnimation6;
        Storyboard.SetTarget((Timeline) element6, (DependencyObject) this.FrontImageTransform);
        Storyboard.SetTargetProperty((Timeline) element6, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element6);
        DoubleAnimation doubleAnimation7 = new DoubleAnimation();
        doubleAnimation7.From = new double?(this.BackImageTransform.TranslateY);
        doubleAnimation7.To = new double?(targetState.TranslateY);
        doubleAnimation7.Duration = (Duration) timeSpan;
        DoubleAnimation element7 = doubleAnimation7;
        Storyboard.SetTarget((Timeline) element7, (DependencyObject) this.BackImageTransform);
        Storyboard.SetTargetProperty((Timeline) element7, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element7);
        DoubleAnimation doubleAnimation8 = new DoubleAnimation();
        doubleAnimation8.From = new double?(this.BackImageTransform.TranslateY);
        doubleAnimation8.To = new double?(targetState.TranslateY);
        doubleAnimation8.Duration = (Duration) timeSpan;
        DoubleAnimation element8 = doubleAnimation8;
        Storyboard.SetTarget((Timeline) element8, (DependencyObject) this.FrontImageTransform);
        Storyboard.SetTargetProperty((Timeline) element8, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element8);
      }
      if (this.CropMode != 0 & updateCropSizeAndPos)
      {
        DoubleAnimation doubleAnimation9 = new DoubleAnimation();
        doubleAnimation9.From = new double?(this.CroppingBorder.Width);
        doubleAnimation9.To = new double?(targetState.CropBoxSize.Width + 2.0);
        doubleAnimation9.Duration = (Duration) timeSpan;
        DoubleAnimation element9 = doubleAnimation9;
        Storyboard.SetTarget((Timeline) element9, (DependencyObject) this.CroppingBorder);
        Storyboard.SetTargetProperty((Timeline) element9, new PropertyPath("Width", new object[0]));
        sb.Children.Add((Timeline) element9);
        DoubleAnimation doubleAnimation10 = new DoubleAnimation();
        doubleAnimation10.From = new double?(this.CroppingBorder.Height);
        doubleAnimation10.To = new double?(targetState.CropBoxSize.Height + 2.0);
        doubleAnimation10.Duration = (Duration) timeSpan;
        DoubleAnimation element10 = doubleAnimation10;
        Storyboard.SetTarget((Timeline) element10, (DependencyObject) this.CroppingBorder);
        Storyboard.SetTargetProperty((Timeline) element10, new PropertyPath("Height", new object[0]));
        sb.Children.Add((Timeline) element10);
        DoubleAnimation doubleAnimation11 = new DoubleAnimation();
        doubleAnimation11.From = new double?(this.CroppingPanelTransform.TranslateX);
        doubleAnimation11.To = new double?(targetState.CropBoxPos.X - 1.0);
        doubleAnimation11.Duration = (Duration) timeSpan;
        DoubleAnimation element11 = doubleAnimation11;
        Storyboard.SetTarget((Timeline) element11, (DependencyObject) this.CroppingPanelTransform);
        Storyboard.SetTargetProperty((Timeline) element11, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element11);
        DoubleAnimation doubleAnimation12 = new DoubleAnimation();
        doubleAnimation12.From = new double?(this.CroppingPanelTransform.TranslateY);
        doubleAnimation12.To = new double?(targetState.CropBoxPos.Y - 1.0);
        doubleAnimation12.Duration = (Duration) timeSpan;
        DoubleAnimation element12 = doubleAnimation12;
        Storyboard.SetTarget((Timeline) element12, (DependencyObject) this.CroppingPanelTransform);
        Storyboard.SetTargetProperty((Timeline) element12, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element12);
        DoubleAnimation doubleAnimation13 = new DoubleAnimation();
        doubleAnimation13.From = new double?(this.FrontImageClipTransform.ScaleX);
        doubleAnimation13.To = new double?(targetState.CropPixelWidth);
        doubleAnimation13.Duration = (Duration) timeSpan;
        DoubleAnimation element13 = doubleAnimation13;
        Storyboard.SetTarget((Timeline) element13, (DependencyObject) this.FrontImageClipTransform);
        Storyboard.SetTargetProperty((Timeline) element13, new PropertyPath("ScaleX", new object[0]));
        sb.Children.Add((Timeline) element13);
        DoubleAnimation doubleAnimation14 = new DoubleAnimation();
        doubleAnimation14.From = new double?(this.FrontImageClipTransform.ScaleY);
        doubleAnimation14.To = new double?(targetState.CropPixelHeight);
        doubleAnimation14.Duration = (Duration) timeSpan;
        DoubleAnimation element14 = doubleAnimation14;
        Storyboard.SetTarget((Timeline) element14, (DependencyObject) this.FrontImageClipTransform);
        Storyboard.SetTargetProperty((Timeline) element14, new PropertyPath("ScaleY", new object[0]));
        sb.Children.Add((Timeline) element14);
        DoubleAnimation doubleAnimation15 = new DoubleAnimation();
        doubleAnimation15.From = new double?(this.FrontImageClipTransform.TranslateX);
        doubleAnimation15.To = new double?(targetState.CropX);
        doubleAnimation15.Duration = (Duration) timeSpan;
        DoubleAnimation element15 = doubleAnimation15;
        Storyboard.SetTarget((Timeline) element15, (DependencyObject) this.FrontImageClipTransform);
        Storyboard.SetTargetProperty((Timeline) element15, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element15);
        DoubleAnimation doubleAnimation16 = new DoubleAnimation();
        doubleAnimation16.From = new double?(this.FrontImageClipTransform.TranslateY);
        doubleAnimation16.To = new double?(targetState.CropY);
        doubleAnimation16.Duration = (Duration) timeSpan;
        DoubleAnimation element16 = doubleAnimation16;
        Storyboard.SetTarget((Timeline) element16, (DependencyObject) this.FrontImageClipTransform);
        Storyboard.SetTargetProperty((Timeline) element16, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element16);
        DoubleAnimation doubleAnimation17 = new DoubleAnimation();
        doubleAnimation17.From = new double?(this.HorizontalGridLine0Transform.ScaleX);
        doubleAnimation17.To = new double?(targetState.CropBoxSize.Width);
        doubleAnimation17.Duration = (Duration) timeSpan;
        DoubleAnimation element17 = doubleAnimation17;
        Storyboard.SetTarget((Timeline) element17, (DependencyObject) this.HorizontalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element17, new PropertyPath("ScaleX", new object[0]));
        sb.Children.Add((Timeline) element17);
        DoubleAnimation doubleAnimation18 = new DoubleAnimation();
        doubleAnimation18.From = new double?(this.HorizontalGridLine0Transform.TranslateX);
        doubleAnimation18.To = new double?(targetState.CropBoxPos.X);
        doubleAnimation18.Duration = (Duration) timeSpan;
        DoubleAnimation element18 = doubleAnimation18;
        Storyboard.SetTarget((Timeline) element18, (DependencyObject) this.HorizontalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element18, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element18);
        DoubleAnimation doubleAnimation19 = new DoubleAnimation();
        doubleAnimation19.From = new double?(this.HorizontalGridLine0Transform.TranslateY);
        doubleAnimation19.To = new double?(targetState.CropBoxPos.Y + targetState.CropBoxSize.Height * 0.333);
        doubleAnimation19.Duration = (Duration) timeSpan;
        DoubleAnimation element19 = doubleAnimation19;
        Storyboard.SetTarget((Timeline) element19, (DependencyObject) this.HorizontalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element19, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element19);
        DoubleAnimation doubleAnimation20 = new DoubleAnimation();
        doubleAnimation20.From = new double?(this.HorizontalGridLine1Transform.ScaleX);
        doubleAnimation20.To = new double?(targetState.CropBoxSize.Width);
        doubleAnimation20.Duration = (Duration) timeSpan;
        DoubleAnimation element20 = doubleAnimation20;
        Storyboard.SetTarget((Timeline) element20, (DependencyObject) this.HorizontalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element20, new PropertyPath("ScaleX", new object[0]));
        sb.Children.Add((Timeline) element20);
        DoubleAnimation doubleAnimation21 = new DoubleAnimation();
        doubleAnimation21.From = new double?(this.HorizontalGridLine1Transform.TranslateX);
        doubleAnimation21.To = new double?(targetState.CropBoxPos.X);
        doubleAnimation21.Duration = (Duration) timeSpan;
        DoubleAnimation element21 = doubleAnimation21;
        Storyboard.SetTarget((Timeline) element21, (DependencyObject) this.HorizontalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element21, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element21);
        DoubleAnimation doubleAnimation22 = new DoubleAnimation();
        doubleAnimation22.From = new double?(this.HorizontalGridLine1Transform.TranslateY);
        doubleAnimation22.To = new double?(targetState.CropBoxPos.Y + targetState.CropBoxSize.Height * 0.667);
        doubleAnimation22.Duration = (Duration) timeSpan;
        DoubleAnimation element22 = doubleAnimation22;
        Storyboard.SetTarget((Timeline) element22, (DependencyObject) this.HorizontalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element22, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element22);
        DoubleAnimation doubleAnimation23 = new DoubleAnimation();
        doubleAnimation23.From = new double?(this.VerticalGridLine0Transform.ScaleY);
        doubleAnimation23.To = new double?(targetState.CropBoxSize.Height);
        doubleAnimation23.Duration = (Duration) timeSpan;
        DoubleAnimation element23 = doubleAnimation23;
        Storyboard.SetTarget((Timeline) element23, (DependencyObject) this.VerticalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element23, new PropertyPath("ScaleY", new object[0]));
        sb.Children.Add((Timeline) element23);
        DoubleAnimation doubleAnimation24 = new DoubleAnimation();
        doubleAnimation24.From = new double?(this.VerticalGridLine0Transform.TranslateX);
        doubleAnimation24.To = new double?(targetState.CropBoxPos.X + targetState.CropBoxSize.Width * 0.333);
        doubleAnimation24.Duration = (Duration) timeSpan;
        DoubleAnimation element24 = doubleAnimation24;
        Storyboard.SetTarget((Timeline) element24, (DependencyObject) this.VerticalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element24, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element24);
        DoubleAnimation doubleAnimation25 = new DoubleAnimation();
        doubleAnimation25.From = new double?(this.VerticalGridLine0Transform.TranslateY);
        doubleAnimation25.To = new double?(targetState.CropBoxPos.Y);
        doubleAnimation25.Duration = (Duration) timeSpan;
        DoubleAnimation element25 = doubleAnimation25;
        Storyboard.SetTarget((Timeline) element25, (DependencyObject) this.VerticalGridLine0Transform);
        Storyboard.SetTargetProperty((Timeline) element25, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element25);
        DoubleAnimation doubleAnimation26 = new DoubleAnimation();
        doubleAnimation26.From = new double?(this.VerticalGridLine1Transform.ScaleY);
        doubleAnimation26.To = new double?(targetState.CropBoxSize.Height);
        doubleAnimation26.Duration = (Duration) timeSpan;
        DoubleAnimation element26 = doubleAnimation26;
        Storyboard.SetTarget((Timeline) element26, (DependencyObject) this.VerticalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element26, new PropertyPath("ScaleY", new object[0]));
        sb.Children.Add((Timeline) element26);
        DoubleAnimation doubleAnimation27 = new DoubleAnimation();
        doubleAnimation27.From = new double?(this.VerticalGridLine1Transform.TranslateX);
        doubleAnimation27.To = new double?(targetState.CropBoxPos.X + targetState.CropBoxSize.Width * 0.667);
        doubleAnimation27.Duration = (Duration) timeSpan;
        DoubleAnimation element27 = doubleAnimation27;
        Storyboard.SetTarget((Timeline) element27, (DependencyObject) this.VerticalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element27, new PropertyPath("TranslateX", new object[0]));
        sb.Children.Add((Timeline) element27);
        DoubleAnimation doubleAnimation28 = new DoubleAnimation();
        doubleAnimation28.From = new double?(this.VerticalGridLine1Transform.TranslateY);
        doubleAnimation28.To = new double?(targetState.CropBoxPos.Y);
        doubleAnimation28.Duration = (Duration) timeSpan;
        DoubleAnimation element28 = doubleAnimation28;
        Storyboard.SetTarget((Timeline) element28, (DependencyObject) this.VerticalGridLine1Transform);
        Storyboard.SetTargetProperty((Timeline) element28, new PropertyPath("TranslateY", new object[0]));
        sb.Children.Add((Timeline) element28);
      }
      this.sbSub_ = Storyboarder.PerformWithDisposable(sb, onComplete: (Action) (() =>
      {
        this.ApplyTransformState(targetState, updateImageSizeAndPos, updateCropSizeAndPos, true);
        this.sbSub_ = (IDisposable) null;
      }), callOnCompleteOnDisposing: true, context: "img edit: transit to state");
    }

    private void ApplyTransformState(
      ImageEditControl.TransformState state,
      bool updateImageSizeAndPos,
      bool updateCropSizeAndPos,
      bool updateInvisibleCropGrips)
    {
      if (updateImageSizeAndPos)
      {
        this.BackImageTransform.ScaleX = this.BackImageTransform.ScaleY = this.FrontImageTransform.ScaleX = this.FrontImageTransform.ScaleY = state.Scale;
        this.BackImageTransform.TranslateX = this.FrontImageTransform.TranslateX = state.TranslateX;
        this.BackImageTransform.TranslateY = this.FrontImageTransform.TranslateY = state.TranslateY;
      }
      if (!(this.CropMode != 0 & updateCropSizeAndPos))
        return;
      this.CroppingBorder.Width = state.CropBoxSize.Width + 2.0;
      this.CroppingBorder.Height = state.CropBoxSize.Height + 2.0;
      this.CroppingPanelTransform.TranslateX = state.CropBoxPos.X - 1.0;
      this.CroppingPanelTransform.TranslateY = state.CropBoxPos.Y - 1.0;
      this.FrontImageClipTransform.ScaleX = state.CropPixelWidth;
      this.FrontImageClipTransform.ScaleY = state.CropPixelHeight;
      this.FrontImageClipTransform.TranslateX = state.CropX;
      this.FrontImageClipTransform.TranslateY = state.CropY;
      this.HorizontalGridLine0Transform.ScaleX = state.CropBoxSize.Width;
      this.HorizontalGridLine0Transform.TranslateX = state.CropBoxPos.X;
      this.HorizontalGridLine0Transform.TranslateY = state.CropBoxPos.Y + state.CropBoxSize.Height * 0.333;
      this.HorizontalGridLine1Transform.ScaleX = state.CropBoxSize.Width;
      this.HorizontalGridLine1Transform.TranslateX = state.CropBoxPos.X;
      this.HorizontalGridLine1Transform.TranslateY = state.CropBoxPos.Y + state.CropBoxSize.Height * 0.667;
      this.VerticalGridLine0Transform.ScaleY = state.CropBoxSize.Height;
      this.VerticalGridLine0Transform.TranslateX = state.CropBoxPos.X + state.CropBoxSize.Width * 0.333;
      this.VerticalGridLine0Transform.TranslateY = state.CropBoxPos.Y;
      this.VerticalGridLine1Transform.ScaleY = state.CropBoxSize.Height;
      this.VerticalGridLine1Transform.TranslateX = state.CropBoxPos.X + state.CropBoxSize.Width * 0.667;
      this.VerticalGridLine1Transform.TranslateY = state.CropBoxPos.Y;
      if (updateInvisibleCropGrips)
      {
        this.TopLeftGripTranslate.X = state.CropBoxPos.X - 50.0;
        this.TopLeftGripTranslate.Y = state.CropBoxPos.Y - 50.0;
        this.TopRightGripTranslate.X = state.CropBoxPos.X + state.CropBoxSize.Width - 50.0;
        this.TopRightGripTranslate.Y = state.CropBoxPos.Y - 50.0;
        this.BottomLeftGripTranslate.X = state.CropBoxPos.X - 50.0;
        this.BottomLeftGripTranslate.Y = state.CropBoxPos.Y + state.CropBoxSize.Height - 50.0;
        this.BottomRightGripTranslate.X = state.CropBoxPos.X + state.CropBoxSize.Width - 50.0;
        this.BottomRightGripTranslate.Y = state.CropBoxPos.Y + state.CropBoxSize.Height - 50.0;
      }
      this.CropRatio = state.CropBoxSize.Width / state.CropBoxSize.Height;
    }

    private ImageEditControl.TransformState GetFittingState(
      System.Windows.Point? cropPos = null,
      Size? cropSize = null,
      Size? targetSize = null)
    {
      if (this.imgSrc_ == null)
        return (ImageEditControl.TransformState) null;
      double controlW = targetSize.HasValue ? targetSize.Value.Width : this.ActualWidth;
      double controlH = targetSize.HasValue ? targetSize.Value.Height : this.ActualHeight;
      double pixelWidth = (double) this.imgSrc_.PixelWidth;
      double pixelHeight = (double) this.imgSrc_.PixelHeight;
      ImageEditControl.TransformState fittingState = pixelHeight > 0.0 && pixelWidth > 0.0 ? new ImageEditControl.TransformState(pixelWidth, pixelHeight) : throw new ArgumentException("img edit: invalid bitmap");
      if (this.CropMode == ImageEditControl.CroppingMode.None)
      {
        double val1 = controlW / pixelWidth;
        double val2 = controlH / pixelHeight;
        fittingState.Scale = Math.Min(val1, val2);
        fittingState.TranslateX = (controlW - fittingState.RenderWidth) / 2.0;
        fittingState.TranslateY = (controlH - fittingState.RenderHeight) / 2.0;
      }
      else
      {
        Size size;
        if (cropSize.HasValue && cropPos.HasValue)
        {
          size = cropSize.Value;
          double width = size.Width;
          size = cropSize.Value;
          double height = size.Height;
          this.cropRatio_ = width / height;
        }
        Size fittingCropBoxSize = this.GetFittingCropBoxSize(controlW, controlH);
        double num1 = (controlW - fittingCropBoxSize.Width) / 2.0;
        double num2 = (controlH - fittingCropBoxSize.Height) / 2.0;
        if (cropSize.HasValue && cropPos.HasValue)
        {
          ImageEditControl.TransformState transformState1 = fittingState;
          double width1 = fittingCropBoxSize.Width;
          size = cropSize.Value;
          double width2 = size.Width;
          double num3 = width1 / width2;
          transformState1.Scale = num3;
          fittingState.CropBoxPos.X = num1;
          fittingState.CropBoxPos.Y = num2;
          ImageEditControl.TransformState transformState2 = fittingState;
          System.Windows.Point point = cropPos.Value;
          double x = point.X;
          transformState2.CropX = x;
          ImageEditControl.TransformState transformState3 = fittingState;
          point = cropPos.Value;
          double y = point.Y;
          transformState3.CropY = y;
          fittingState.TranslateX = num1 - fittingState.Scale * fittingState.CropX;
          fittingState.TranslateY = num2 - fittingState.Scale * fittingState.CropY;
          fittingState.SetCropBox(fittingCropBoxSize.Width, fittingCropBoxSize.Height, new double?(), new double?());
        }
        else
        {
          if (fittingState.OriginalPixelWidth / fittingState.OriginalPixelHeight > this.CropRatio)
            fittingState.Scale = fittingCropBoxSize.Height / fittingState.OriginalPixelHeight;
          else
            fittingState.Scale = fittingCropBoxSize.Width / fittingState.OriginalPixelWidth;
          fittingState.TranslateX = (controlW - fittingState.RenderWidth) / 2.0;
          fittingState.TranslateY = (controlH - fittingState.RenderHeight) / 2.0;
          fittingState.SetCropBox(fittingCropBoxSize.Width, fittingCropBoxSize.Height, new double?(num1), new double?(num2));
        }
      }
      return fittingState;
    }

    private double GetMinScale(ImageEditControl.TransformState state)
    {
      double minScale = this.minScale_;
      if (this.CropMode != ImageEditControl.CroppingMode.None)
        minScale = state.OriginalPixelWidth / state.OriginalPixelHeight <= this.CropRatio ? state.CropBoxSize.Width / state.OriginalPixelWidth : state.CropBoxSize.Height / state.OriginalPixelHeight;
      return minScale;
    }

    public void Setup(
      BitmapSource img,
      ImageEditControl.CroppingMode cropMode,
      double cropRatio,
      Size? targetSize = null)
    {
      this.BackImage.Source = (System.Windows.Media.ImageSource) (this.imgSrc_ = img);
      if ((this.cropMode_ = cropMode) != ImageEditControl.CroppingMode.None)
        this.FrontImage.Source = (System.Windows.Media.ImageSource) img;
      this.SetCropping(cropRatio, targetSize);
    }

    public void Setup(
      BitmapSource img,
      ImageEditControl.CroppingMode cropMode,
      System.Windows.Point relativeCropPos,
      Size relativeCropSize,
      Size? targetSize = null)
    {
      this.BackImage.Source = (System.Windows.Media.ImageSource) (this.imgSrc_ = img);
      if ((this.cropMode_ = cropMode) != ImageEditControl.CroppingMode.None)
        this.FrontImage.Source = (System.Windows.Media.ImageSource) img;
      this.state_ = new ImageEditControl.TransformState((double) img.PixelWidth, (double) img.PixelHeight);
      this.SetCropping(relativeCropPos, relativeCropSize, true, targetSize);
    }

    private void ResetControls(Size? targetSize = null)
    {
      if (!this.isLoaded_)
        return;
      if (this.CropMode == ImageEditControl.CroppingMode.None)
      {
        this.CroppingBorder.Visibility = this.CroppingMask.Visibility = this.FrontImage.Visibility = this.TopLeftGrip.Visibility = this.TopRightGrip.Visibility = this.BottomLeftGrip.Visibility = this.BottomRightGrip.Visibility = Visibility.Collapsed;
      }
      else
      {
        Size size;
        double val1;
        if (!targetSize.HasValue)
        {
          val1 = this.ActualWidth;
        }
        else
        {
          size = targetSize.Value;
          val1 = size.Width;
        }
        double num1;
        if (!targetSize.HasValue)
        {
          num1 = this.ActualHeight;
        }
        else
        {
          size = targetSize.Value;
          num1 = size.Height;
        }
        double val2 = num1;
        double num2 = Math.Max(val1, val2);
        this.CroppingMask.Width = this.CroppingMask.Height = num2 * 2.0;
        Canvas.SetLeft((UIElement) this.CroppingMask, -num2 * 0.5);
        Canvas.SetTop((UIElement) this.CroppingMask, -num2 * 0.5);
        this.CroppingBorder.Visibility = this.CroppingMask.Visibility = this.FrontImage.Visibility = Visibility.Visible;
        this.CroppingGripsPanel.Visibility = this.TopLeftGrip.Visibility = this.TopRightGrip.Visibility = this.BottomLeftGrip.Visibility = this.BottomRightGrip.Visibility = (this.CropMode == ImageEditControl.CroppingMode.Custom).ToVisibility();
        this.BackImage.Opacity = 1.0;
      }
    }

    private void ResetTransform(System.Windows.Point? cropPos = null, Size? cropSize = null, Size? targetSize = null, bool transit = false)
    {
      if (this.imgSrc_ == null)
        return;
      ImageEditControl.TransformState fittingState = this.GetFittingState(cropPos, cropSize, targetSize);
      this.minScale_ = this.CropMode != ImageEditControl.CroppingMode.None ? this.GetMinScale(fittingState) : fittingState.Scale;
      if (this.CropMode != ImageEditControl.CroppingMode.None)
      {
        Size? relativeCropSize = this.MinRelativeCropSize;
        if (relativeCropSize.HasValue)
        {
          double width = fittingState.CropBoxSize.Width;
          relativeCropSize = this.MinRelativeCropSize;
          Size size = relativeCropSize.Value;
          double num1 = size.Width * fittingState.OriginalPixelWidth;
          double val1 = width / num1;
          double height = fittingState.CropBoxSize.Height;
          relativeCropSize = this.MinRelativeCropSize;
          size = relativeCropSize.Value;
          double num2 = size.Height * fittingState.OriginalPixelHeight;
          double val2 = height / num2;
          this.maxScale_ = Math.Min(val1, val2);
          goto label_5;
        }
      }
      this.maxScale_ = 4.0 * fittingState.Scale;
label_5:
      if (transit)
        this.TransitToTransformState(this.state_ = fittingState, true, true);
      else
        this.ApplyTransformState(this.state_ = fittingState, true, true, true);
    }

    private void SetScale(double scale, ref System.Windows.Point originOnScreen, System.Windows.Point originOnImage)
    {
      double num = Math.Max(this.minScale_, Math.Min(this.maxScale_, scale));
      System.Windows.Point translateLowBound = this.GetTranslateLowBound(num);
      System.Windows.Point translateHighBound = this.GetTranslateHighBound();
      this.state_.SetScale(num, ref originOnScreen, originOnImage, translateLowBound, translateHighBound, new Size(this.ActualWidth, this.ActualHeight));
      if (this.CropMode != ImageEditControl.CroppingMode.None)
      {
        this.state_.CropPixelWidth = this.state_.CropBoxSize.Width / num;
        this.state_.CropPixelHeight = this.state_.CropBoxSize.Height / num;
        this.state_.CropX = (this.state_.CropBoxPos.X - this.state_.TranslateX) / this.state_.Scale;
        this.state_.CropY = (this.state_.CropBoxPos.Y - this.state_.TranslateY) / this.state_.Scale;
      }
      this.ApplyTransformState(this.state_, true, true, false);
    }

    private void ProcessMove(double deltaX, double deltaY)
    {
      bool flag = false;
      System.Windows.Point translateLowBound = this.GetTranslateLowBound(this.state_.Scale);
      System.Windows.Point translateHighBound = this.GetTranslateHighBound();
      double val2_1 = this.state_.TranslateX + deltaX;
      double val2_2 = this.state_.TranslateY + deltaY;
      if (this.CropMode == ImageEditControl.CroppingMode.None)
      {
        if (this.state_.RenderWidth > this.ActualWidth)
        {
          this.state_.TranslateX = Math.Max(translateLowBound.X, Math.Min(translateHighBound.X, val2_1));
          flag = true;
        }
        if (this.state_.RenderHeight > this.ActualHeight)
        {
          this.state_.TranslateY = Math.Max(translateLowBound.Y, Math.Min(translateHighBound.Y, val2_2));
          flag = true;
        }
      }
      else
      {
        this.state_.TranslateX = Math.Max(translateLowBound.X, Math.Min(translateHighBound.X, val2_1));
        this.state_.TranslateY = Math.Max(translateLowBound.Y, Math.Min(translateHighBound.Y, val2_2));
        flag = true;
        this.state_.CropX = (this.state_.CropBoxPos.X - this.state_.TranslateX) / this.state_.Scale;
        this.state_.CropY = (this.state_.CropBoxPos.Y - this.state_.TranslateY) / this.state_.Scale;
      }
      if (!flag)
        return;
      this.ApplyTransformState(this.state_, true, true, false);
    }

    private void AdjustImagePosition()
    {
      bool flag = false;
      double renderHeight = this.state_.RenderHeight;
      if (this.CropMode == ImageEditControl.CroppingMode.None && renderHeight < this.ActualHeight)
      {
        this.state_.TranslateY = (this.ActualHeight - renderHeight) * 0.5;
        flag = true;
      }
      else
      {
        double translateY1 = this.state_.TranslateY;
        System.Windows.Point translateHighBound = this.GetTranslateHighBound();
        double y1;
        double num1 = y1 = translateHighBound.Y;
        if (translateY1 > y1)
        {
          this.state_.TranslateY = num1;
          flag = true;
        }
        else
        {
          double translateY2 = this.state_.TranslateY;
          System.Windows.Point translateLowBound = this.GetTranslateLowBound(this.state_.Scale);
          double y2;
          double num2 = y2 = translateLowBound.Y;
          if (translateY2 < y2)
          {
            this.state_.TranslateY = num2;
            flag = true;
          }
        }
      }
      double renderWidth = this.state_.RenderWidth;
      if (this.CropMode == ImageEditControl.CroppingMode.None && renderWidth < this.ActualWidth)
      {
        this.state_.TranslateX = (this.ActualWidth - renderWidth) * 0.5;
        flag = true;
      }
      else
      {
        double translateX1 = this.state_.TranslateX;
        System.Windows.Point translateHighBound = this.GetTranslateHighBound();
        double x1;
        double num3 = x1 = translateHighBound.X;
        if (translateX1 > x1)
        {
          this.state_.TranslateX = num3;
          flag = true;
        }
        else
        {
          double translateX2 = this.state_.TranslateX;
          System.Windows.Point translateLowBound = this.GetTranslateLowBound(this.state_.Scale);
          double x2;
          double num4 = x2 = translateLowBound.X;
          if (translateX2 < x2)
          {
            this.state_.TranslateX = num4;
            flag = true;
          }
        }
      }
      if (!flag)
        return;
      this.ApplyTransformState(this.state_, true, true, true);
    }

    private System.Windows.Point GetTranslateHighBound()
    {
      return this.CropMode != ImageEditControl.CroppingMode.None ? new System.Windows.Point(this.state_.CropBoxPos.X, this.state_.CropBoxPos.Y) : new System.Windows.Point(0.0, 0.0);
    }

    private System.Windows.Point GetTranslateLowBound(double scale)
    {
      return this.CropMode != ImageEditControl.CroppingMode.None ? new System.Windows.Point(this.state_.CropBoxPos.X + this.state_.CropBoxSize.Width - this.state_.RenderWidth, this.state_.CropBoxPos.Y + this.state_.CropBoxSize.Height - this.state_.RenderHeight) : new System.Windows.Point(this.ActualWidth - this.state_.OriginalPixelWidth * scale, this.ActualHeight - this.state_.OriginalPixelHeight * scale);
    }

    private Size GetFittingCropBoxSize(double controlW, double controlH)
    {
      if (this.CropMode == ImageEditControl.CroppingMode.None)
        return new Size(0.0, 0.0);
      double num1 = 112.0;
      double num2 = controlW - num1;
      double num3 = controlH - num1;
      if (num2 <= 0.0 || num3 <= 0.0)
        throw new ArgumentException();
      double height;
      double width;
      if (num2 / num3 > this.CropRatio)
      {
        height = num3;
        width = height * this.CropRatio;
      }
      else
      {
        width = num2;
        height = width / this.CropRatio;
      }
      return new Size(width, height);
    }

    private Size GetMinCropBoxSize()
    {
      Size size1;
      if (!this.MinRelativeCropSize.HasValue)
      {
        size1 = new Size(1.0, 1.0);
      }
      else
      {
        Size size2 = this.MinRelativeCropSize.Value;
        double width = size2.Width * this.state_.OriginalPixelWidth;
        size2 = this.MinRelativeCropSize.Value;
        double height = size2.Height * this.state_.OriginalPixelHeight;
        size1 = new Size(width, height);
      }
      Size minCropBoxSize = size1;
      minCropBoxSize.Width *= this.state_.Scale;
      minCropBoxSize.Height *= this.state_.Scale;
      return minCropBoxSize;
    }

    private double GetTargetCropBoxXFrmDelta(double dX, Size minCropBoxSize)
    {
      double cropBoxXfrmDelta = this.state_.CropBoxPos.X + dX;
      double num1 = Math.Max(this.state_.TranslateX, 56.0);
      if (cropBoxXfrmDelta < num1)
        cropBoxXfrmDelta = num1;
      double num2 = this.state_.CropBoxPos.X + this.state_.CropBoxSize.Width - minCropBoxSize.Width;
      if (cropBoxXfrmDelta > num2)
        cropBoxXfrmDelta = num2;
      return cropBoxXfrmDelta;
    }

    private double GetTargetCropBoxYFrmDelta(double dY, Size minCropBoxSize)
    {
      double cropBoxYfrmDelta = this.state_.CropBoxPos.Y + dY;
      double num1 = Math.Max(this.state_.TranslateY, 56.0);
      if (cropBoxYfrmDelta < num1)
        cropBoxYfrmDelta = num1;
      double num2 = this.state_.CropBoxPos.Y + this.state_.CropBoxSize.Height - minCropBoxSize.Height;
      if (cropBoxYfrmDelta > num2)
        cropBoxYfrmDelta = num2;
      return cropBoxYfrmDelta;
    }

    private double AdjustTargetCropBoxWidth(double cropBoxW, Size minCropBoxSize)
    {
      if (cropBoxW < minCropBoxSize.Width)
        cropBoxW = minCropBoxSize.Width;
      double num = Math.Min(this.ActualWidth - 56.0 - this.state_.CropBoxPos.X, (this.state_.OriginalPixelWidth - this.state_.CropX) * this.state_.Scale);
      if (cropBoxW > num)
        cropBoxW = num;
      return cropBoxW;
    }

    private double AdjustTargetCropBoxHeight(double cropBoxH, Size minCropBoxSize)
    {
      if (cropBoxH < minCropBoxSize.Height)
        cropBoxH = minCropBoxSize.Height;
      double num = Math.Min(this.ActualHeight - 56.0 - this.state_.CropBoxPos.Y, (this.state_.OriginalPixelHeight - this.state_.CropY) * this.state_.Scale);
      if (cropBoxH > num)
        cropBoxH = num;
      return cropBoxH;
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
      if (!this.isLoaded_ || this.state_ == null)
        return;
      if (this.CropMode == ImageEditControl.CroppingMode.None)
        this.ResetTransform();
      else
        this.ResetTransform(new System.Windows.Point?(new System.Windows.Point(this.state_.CropX, this.state_.CropY)), new Size?(new Size(this.state_.CropPixelWidth, this.state_.CropPixelHeight)));
      this.ResetControls();
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      this.pinchCenterOnScreen_.X = this.ActualWidth * 0.5;
      this.pinchCenterOnScreen_.Y = this.ActualHeight * 0.5;
      this.pinchCenterOnImage_.X = (this.pinchCenterOnScreen_.X - this.state_.TranslateX) / this.state_.Scale;
      this.pinchCenterOnImage_.Y = (this.pinchCenterOnScreen_.Y - this.state_.TranslateY) / this.state_.Scale;
      this.isPinching_ = this.isMoving_ = false;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      if (e.PinchManipulation != null)
      {
        this.isPinching_ = true;
        this.SetScale(this.state_.Scale * e.PinchManipulation.DeltaScale, ref this.pinchCenterOnScreen_, this.pinchCenterOnImage_);
      }
      else if (e.DeltaManipulation != null)
      {
        this.isMoving_ = true;
        this.ProcessMove(e.DeltaManipulation.Translation.X * this.state_.Scale, e.DeltaManipulation.Translation.Y * this.state_.Scale);
      }
      e.Handled = true;
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.isPinching_ = this.isMoving_ = false;
      this.AdjustImagePosition();
    }

    private void OnDoubleTap(object sender, GestureEventArgs e)
    {
      if (this.state_.Scale < this.maxScale_)
      {
        System.Windows.Point position = e.GetPosition((UIElement) this);
        System.Windows.Point originOnImage = new System.Windows.Point((position.X - this.state_.TranslateX) / this.state_.Scale, (position.Y - this.state_.TranslateY) / this.state_.Scale);
        if (originOnImage.X > this.state_.OriginalPixelWidth || originOnImage.X < 0.0)
        {
          originOnImage.X = Math.Min(this.state_.OriginalPixelWidth, Math.Max(0.0, originOnImage.X));
          position.X = originOnImage.X * this.state_.Scale + this.state_.TranslateX;
        }
        if (originOnImage.Y > this.state_.OriginalPixelHeight || originOnImage.Y < 0.0)
        {
          originOnImage.Y = Math.Min(this.state_.OriginalPixelHeight, Math.Max(0.0, originOnImage.Y));
          position.Y = originOnImage.Y * this.state_.Scale + this.state_.TranslateY;
        }
        this.SetScale(this.maxScale_, ref position, originOnImage);
        this.ApplyTransformState(this.state_, true, true, true);
      }
      else
        this.ResetTransform();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      if (this.isLoaded_)
        return;
      this.isLoaded_ = true;
      this.ResetTransform(this.presetCropPos_, this.presetCropSize_);
      this.presetCropPos_ = new System.Windows.Point?();
      this.presetCropSize_ = new Size?();
      this.ResetControls();
    }

    private void TopLeftGrip_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      double x = e.DeltaManipulation.Translation.X;
      double y = e.DeltaManipulation.Translation.Y;
      Size minCropBoxSize = this.GetMinCropBoxSize();
      double cropBoxXfrmDelta = this.GetTargetCropBoxXFrmDelta(x, minCropBoxSize);
      double cropBoxYfrmDelta = this.GetTargetCropBoxYFrmDelta(y, minCropBoxSize);
      this.state_.SetCropBox(this.state_.CropBoxSize.Width - cropBoxXfrmDelta + this.state_.CropBoxPos.X, this.state_.CropBoxSize.Height - cropBoxYfrmDelta + this.state_.CropBoxPos.Y, new double?(cropBoxXfrmDelta), new double?(cropBoxYfrmDelta));
      this.ApplyTransformState(this.state_, false, true, false);
      e.Handled = true;
    }

    private void TopRightGrip_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      double x = e.DeltaManipulation.Translation.X;
      double y = e.DeltaManipulation.Translation.Y;
      Size minCropBoxSize = this.GetMinCropBoxSize();
      double cropBoxYfrmDelta = this.GetTargetCropBoxYFrmDelta(y, minCropBoxSize);
      this.state_.SetCropBox(this.AdjustTargetCropBoxWidth(this.state_.CropBoxSize.Width + x, minCropBoxSize), this.state_.CropBoxSize.Height - cropBoxYfrmDelta + this.state_.CropBoxPos.Y, new double?(), new double?(cropBoxYfrmDelta));
      this.ApplyTransformState(this.state_, false, true, false);
      e.Handled = true;
    }

    private void BottomLeftGrip_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      double x = e.DeltaManipulation.Translation.X;
      double y = e.DeltaManipulation.Translation.Y;
      Size minCropBoxSize = this.GetMinCropBoxSize();
      double cropBoxXfrmDelta = this.GetTargetCropBoxXFrmDelta(x, minCropBoxSize);
      this.state_.SetCropBox(this.state_.CropBoxSize.Width - cropBoxXfrmDelta + this.state_.CropBoxPos.X, this.AdjustTargetCropBoxHeight(this.state_.CropBoxSize.Height + y, minCropBoxSize), new double?(cropBoxXfrmDelta), new double?());
      this.ApplyTransformState(this.state_, false, true, false);
      e.Handled = true;
    }

    private void BottomRightGrip_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      double x = e.DeltaManipulation.Translation.X;
      double y = e.DeltaManipulation.Translation.Y;
      Size minCropBoxSize = this.GetMinCropBoxSize();
      this.state_.SetCropBox(this.AdjustTargetCropBoxWidth(this.state_.CropBoxSize.Width + x, minCropBoxSize), this.AdjustTargetCropBoxHeight(this.state_.CropBoxSize.Height + y, minCropBoxSize), new double?(), new double?());
      this.ApplyTransformState(this.state_, false, true, false);
      e.Handled = true;
    }

    private void Grip_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.state_ = this.GetFittingState(new System.Windows.Point?(this.state_.CropPixelPos), new Size?(this.state_.CropPixelSize));
      this.TransitToTransformState(this.state_, true, true);
      this.minScale_ = this.GetMinScale(this.state_);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/ImageEditControl.xaml", UriKind.Relative));
      this.BackImage = (Image) this.FindName("BackImage");
      this.BackImageTransform = (CompositeTransform) this.FindName("BackImageTransform");
      this.CroppingMask = (Rectangle) this.FindName("CroppingMask");
      this.CroppingBorder = (Border) this.FindName("CroppingBorder");
      this.CroppingPanelTransform = (CompositeTransform) this.FindName("CroppingPanelTransform");
      this.CroppingGripsPanel = (Grid) this.FindName("CroppingGripsPanel");
      this.FrontImage = (Image) this.FindName("FrontImage");
      this.FrontImageTransform = (CompositeTransform) this.FindName("FrontImageTransform");
      this.FrontImageClipTransform = (CompositeTransform) this.FindName("FrontImageClipTransform");
      this.HorizontalGridLine0Transform = (CompositeTransform) this.FindName("HorizontalGridLine0Transform");
      this.HorizontalGridLine1Transform = (CompositeTransform) this.FindName("HorizontalGridLine1Transform");
      this.VerticalGridLine0Transform = (CompositeTransform) this.FindName("VerticalGridLine0Transform");
      this.VerticalGridLine1Transform = (CompositeTransform) this.FindName("VerticalGridLine1Transform");
      this.TopLeftGrip = (Ellipse) this.FindName("TopLeftGrip");
      this.TopLeftGripTranslate = (TranslateTransform) this.FindName("TopLeftGripTranslate");
      this.TopRightGrip = (Ellipse) this.FindName("TopRightGrip");
      this.TopRightGripTranslate = (TranslateTransform) this.FindName("TopRightGripTranslate");
      this.BottomLeftGrip = (Ellipse) this.FindName("BottomLeftGrip");
      this.BottomLeftGripTranslate = (TranslateTransform) this.FindName("BottomLeftGripTranslate");
      this.BottomRightGrip = (Ellipse) this.FindName("BottomRightGrip");
      this.BottomRightGripTranslate = (TranslateTransform) this.FindName("BottomRightGripTranslate");
    }

    private class TransformState : ImageViewControl.TransformState
    {
      public double CropPixelWidth;
      public double CropPixelHeight;
      public double CropX;
      public double CropY;
      public System.Windows.Point CropBoxPos = new System.Windows.Point(0.0, 0.0);
      public Size CropBoxSize = new Size(0.0, 0.0);

      public Size CropPixelSize => new Size(this.CropPixelWidth, this.CropPixelHeight);

      public System.Windows.Point CropPixelPos => new System.Windows.Point(this.CropX, this.CropY);

      public bool IsCropped
      {
        get
        {
          if (this.CropPixelWidth <= 0.0 || this.CropPixelHeight <= 0.0)
            return false;
          return Math.Abs(this.CropPixelWidth - this.OriginalPixelWidth) > 1.0 || Math.Abs(this.CropPixelHeight - this.OriginalPixelHeight) > 1.0;
        }
      }

      public TransformState()
      {
      }

      public TransformState(double originalPixelWidth, double originalPixelHeight)
        : base(originalPixelWidth, originalPixelHeight)
      {
      }

      public void ClearCropping()
      {
        this.CropPixelWidth = this.CropPixelHeight = this.CropX = this.CropY = 0.0;
        this.CropBoxPos = new System.Windows.Point(0.0, 0.0);
        this.CropBoxSize = new Size(0.0, 0.0);
      }

      public override void Copy(ImageViewControl.TransformState otherBase)
      {
        base.Copy(otherBase);
        if (!(otherBase is ImageEditControl.TransformState transformState))
          return;
        this.CropPixelWidth = transformState.CropPixelWidth;
        this.CropPixelHeight = transformState.CropPixelHeight;
        this.CropX = transformState.CropX;
        this.CropY = transformState.CropY;
        this.CropBoxSize = transformState.CropBoxSize;
        this.CropBoxPos = transformState.CropBoxPos;
      }

      public override void SetScale(
        double newScale,
        ref System.Windows.Point originOnScreen,
        System.Windows.Point originOnImage,
        System.Windows.Point translateLowBound,
        System.Windows.Point translateHighBound,
        Size containerSize)
      {
        if (this.CropPixelWidth > 0.0 && this.CropPixelHeight > 0.0)
        {
          double val2_1 = originOnScreen.X - originOnImage.X * newScale;
          double val2_2 = originOnScreen.Y - originOnImage.Y * newScale;
          if (newScale <= this.Scale)
          {
            val2_1 = Math.Max(translateLowBound.X, Math.Min(translateHighBound.X, val2_1));
            val2_2 = Math.Max(translateLowBound.Y, Math.Min(translateHighBound.Y, val2_2));
            originOnScreen.X = val2_1 + originOnImage.X * newScale;
            originOnScreen.Y = val2_2 + originOnImage.Y * newScale;
          }
          this.Scale = newScale;
          this.TranslateX = val2_1;
          this.TranslateY = val2_2;
        }
        else
          base.SetScale(newScale, ref originOnScreen, originOnImage, translateLowBound, translateHighBound, containerSize);
      }

      public void SetCropBox(
        double cropBoxWidth,
        double cropBoxHeight,
        double? cropBoxX,
        double? cropBoxY)
      {
        this.CropBoxSize.Width = cropBoxWidth;
        this.CropBoxSize.Height = cropBoxHeight;
        this.CropPixelWidth = this.CropBoxSize.Width / this.Scale;
        this.CropPixelHeight = this.CropBoxSize.Height / this.Scale;
        if (cropBoxX.HasValue)
        {
          this.CropBoxPos.X = cropBoxX.Value;
          this.CropX = (cropBoxX.Value - this.TranslateX) / this.Scale;
        }
        if (!cropBoxY.HasValue)
          return;
        this.CropBoxPos.Y = cropBoxY.Value;
        this.CropY = (cropBoxY.Value - this.TranslateY) / this.Scale;
      }
    }

    public enum CroppingMode
    {
      None,
      Fixed,
      Custom,
    }

    public enum RotationMode
    {
      None,
      Rotate90,
      Custom,
    }
  }
}
