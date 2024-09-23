// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PopupHelper
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  internal class PopupHelper
  {
    private const double PopupOffset = 2.0;
    private bool _hasControlLoaded;

    public bool UsesClosingVisualState { get; private set; }

    private Control Parent { get; set; }

    private Canvas OutsidePopupCanvas { get; set; }

    private Canvas PopupChildCanvas { get; set; }

    public double MaxDropDownHeight { get; set; }

    public Popup Popup { get; private set; }

    public bool IsOpen
    {
      get => this.Popup.IsOpen;
      set => this.Popup.IsOpen = value;
    }

    private FrameworkElement PopupChild { get; set; }

    public event EventHandler Closed;

    public event EventHandler FocusChanged;

    public event EventHandler UpdateVisualStates;

    public PopupHelper(Control parent) => this.Parent = parent;

    public PopupHelper(Control parent, Popup popup)
      : this(parent)
    {
      this.Popup = popup;
    }

    private MatrixTransform GetControlMatrixTransform()
    {
      try
      {
        return this.Parent.TransformToVisual((UIElement) null) as MatrixTransform;
      }
      catch
      {
        this.OnClosed(EventArgs.Empty);
        return (MatrixTransform) null;
      }
    }

    private static Point MatrixTransformPoint(MatrixTransform matrixTransform, Thickness margin)
    {
      return new Point(matrixTransform.Matrix.OffsetX + margin.Left, matrixTransform.Matrix.OffsetY + margin.Top);
    }

    private Size GetControlSize(Thickness margin, Size? finalSize)
    {
      return new Size((finalSize.HasValue ? finalSize.Value.Width : this.Parent.ActualWidth) - margin.Left - margin.Right, (finalSize.HasValue ? finalSize.Value.Height : this.Parent.ActualHeight) - margin.Top - margin.Bottom);
    }

    private Thickness GetMargin()
    {
      Thickness? resource = this.Popup.Resources[(object) "PhoneTouchTargetOverhang"] as Thickness?;
      return resource.HasValue ? resource.Value : new Thickness(0.0);
    }

    private static bool IsChildAbove(Size displaySize, Size controlSize, Point controlOffset)
    {
      double y = controlOffset.Y;
      double num = displaySize.Height - controlSize.Height - y;
      return y > num;
    }

    private static double Min0(double x, double y) => Math.Max(Math.Min(x, y), 0.0);

    private Size AboveChildSize(Size controlSize, Point controlPoint)
    {
      return new Size(controlSize.Width, PopupHelper.Min0(controlPoint.Y - 2.0, this.MaxDropDownHeight));
    }

    private Size BelowChildSize(Size displaySize, Size controlSize, Point controlPoint)
    {
      return new Size(controlSize.Width, PopupHelper.Min0(displaySize.Height - controlSize.Height - controlPoint.Y - 2.0, this.MaxDropDownHeight));
    }

    private Point AboveChildPoint(Thickness margin)
    {
      return new Point(margin.Left, margin.Top - this.PopupChild.ActualHeight - 2.0 + this.Parent.BorderThickness.Bottom * 2.0);
    }

    private Point BelowChildPoint(Thickness margin, Size controlSize)
    {
      return new Point(margin.Left, margin.Top + controlSize.Height + 2.0 - this.Parent.BorderThickness.Top * 2.0);
    }

    public void Arrange(Size? finalSize)
    {
      PhoneApplicationFrame phoneApplicationFrame;
      if (this.Popup == null || this.PopupChild == null || Application.Current == null || this.OutsidePopupCanvas == null || Application.Current.Host == null || Application.Current.Host.Content == null || !PhoneHelper.TryGetPhoneApplicationFrame(out phoneApplicationFrame))
        return;
      Size usefulSize = phoneApplicationFrame.GetUsefulSize();
      Thickness margin = this.GetMargin();
      Size controlSize = this.GetControlSize(margin, finalSize);
      MatrixTransform controlMatrixTransform = this.GetControlMatrixTransform();
      if (controlMatrixTransform == null)
        return;
      Point point1 = PopupHelper.MatrixTransformPoint(controlMatrixTransform, margin);
      Size sipUncoveredSize = phoneApplicationFrame.GetSipUncoveredSize();
      bool flag = PopupHelper.IsChildAbove(sipUncoveredSize, controlSize, point1);
      Size size = flag ? this.AboveChildSize(controlSize, point1) : this.BelowChildSize(sipUncoveredSize, controlSize, point1);
      if (usefulSize.Width == 0.0 || usefulSize.Height == 0.0 || size.Height == 0.0)
        return;
      Point point2 = flag ? this.AboveChildPoint(margin) : this.BelowChildPoint(margin, controlSize);
      this.Popup.HorizontalOffset = 0.0;
      this.Popup.VerticalOffset = 0.0;
      this.PopupChild.Width = this.PopupChild.MaxWidth = this.PopupChild.MinWidth = controlSize.Width;
      this.PopupChild.MinHeight = 0.0;
      this.PopupChild.MaxHeight = size.Height;
      this.PopupChild.HorizontalAlignment = HorizontalAlignment.Left;
      this.PopupChild.VerticalAlignment = VerticalAlignment.Top;
      Canvas.SetLeft((UIElement) this.PopupChild, point2.X);
      Canvas.SetTop((UIElement) this.PopupChild, point2.Y);
      this.OutsidePopupCanvas.Width = controlSize.Width;
      this.OutsidePopupCanvas.Height = usefulSize.Height;
      Matrix outputMatrix;
      controlMatrixTransform.Matrix.Invert(out outputMatrix);
      controlMatrixTransform.Matrix = outputMatrix;
      this.OutsidePopupCanvas.RenderTransform = (Transform) controlMatrixTransform;
    }

    private void OnClosed(EventArgs e)
    {
      EventHandler closed = this.Closed;
      if (closed == null)
        return;
      closed((object) this, e);
    }

    private void OnPopupClosedStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      if (e == null || e.NewState == null || !(e.NewState.Name == "PopupClosed"))
        return;
      if (this.Popup != null)
        this.Popup.IsOpen = false;
      this.OnClosed(EventArgs.Empty);
    }

    public void BeforeOnApplyTemplate()
    {
      if (this.UsesClosingVisualState)
      {
        VisualStateGroup visualStateGroup = VisualStates.TryGetVisualStateGroup((DependencyObject) this.Parent, "PopupStates");
        if (visualStateGroup != null)
        {
          visualStateGroup.CurrentStateChanged -= new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
          this.UsesClosingVisualState = false;
        }
      }
      if (this.Popup == null)
        return;
      this.Popup.Closed -= new EventHandler(this.Popup_Closed);
    }

    public void AfterOnApplyTemplate()
    {
      if (this.Popup != null)
        this.Popup.Closed += new EventHandler(this.Popup_Closed);
      VisualStateGroup visualStateGroup = VisualStates.TryGetVisualStateGroup((DependencyObject) this.Parent, "PopupStates");
      if (visualStateGroup != null)
      {
        visualStateGroup.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnPopupClosedStateChanged);
        this.UsesClosingVisualState = true;
      }
      if (this.Popup == null)
        return;
      this.PopupChild = this.Popup.Child as FrameworkElement;
      if (this.PopupChild == null || this._hasControlLoaded)
        return;
      this._hasControlLoaded = true;
      this.PopupChildCanvas = new Canvas();
      this.Popup.Child = (UIElement) this.PopupChildCanvas;
      this.OutsidePopupCanvas = new Canvas();
      this.OutsidePopupCanvas.Background = (Brush) new SolidColorBrush(Colors.Transparent);
      this.OutsidePopupCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.OutsidePopup_MouseLeftButtonDown);
      this.PopupChildCanvas.Children.Add((UIElement) this.OutsidePopupCanvas);
      this.PopupChildCanvas.Children.Add((UIElement) this.PopupChild);
      this.PopupChild.GotFocus += new RoutedEventHandler(this.PopupChild_GotFocus);
      this.PopupChild.LostFocus += new RoutedEventHandler(this.PopupChild_LostFocus);
      this.PopupChild.MouseEnter += new MouseEventHandler(this.PopupChild_MouseEnter);
      this.PopupChild.MouseLeave += new MouseEventHandler(this.PopupChild_MouseLeave);
      this.PopupChild.SizeChanged += new SizeChangedEventHandler(this.PopupChild_SizeChanged);
    }

    private void PopupChild_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.Arrange(new Size?());
    }

    private void OutsidePopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.Popup == null)
        return;
      this.Popup.IsOpen = false;
    }

    private void Popup_Closed(object sender, EventArgs e) => this.OnClosed(EventArgs.Empty);

    private void OnFocusChanged(EventArgs e)
    {
      EventHandler focusChanged = this.FocusChanged;
      if (focusChanged == null)
        return;
      focusChanged((object) this, e);
    }

    private void OnUpdateVisualStates(EventArgs e)
    {
      EventHandler updateVisualStates = this.UpdateVisualStates;
      if (updateVisualStates == null)
        return;
      updateVisualStates((object) this, e);
    }

    private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
    {
      this.OnFocusChanged(EventArgs.Empty);
    }

    private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
    {
      this.OnFocusChanged(EventArgs.Empty);
    }

    private void PopupChild_MouseEnter(object sender, MouseEventArgs e)
    {
      this.OnUpdateVisualStates(EventArgs.Empty);
    }

    private void PopupChild_MouseLeave(object sender, MouseEventArgs e)
    {
      this.OnUpdateVisualStates(EventArgs.Empty);
    }
  }
}
