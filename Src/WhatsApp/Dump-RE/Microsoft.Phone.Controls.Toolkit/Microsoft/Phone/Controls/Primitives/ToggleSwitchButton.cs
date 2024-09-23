// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.Primitives.ToggleSwitchButton
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
namespace Microsoft.Phone.Controls.Primitives
{
  [TemplateVisualState(Name = "Checked", GroupName = "CheckStates")]
  [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
  [TemplatePart(Name = "SwitchRoot", Type = typeof (Grid))]
  [TemplatePart(Name = "SwitchBackground", Type = typeof (UIElement))]
  [TemplatePart(Name = "SwitchTrack", Type = typeof (Grid))]
  [TemplatePart(Name = "SwitchThumb", Type = typeof (FrameworkElement))]
  [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Dragging", GroupName = "CheckStates")]
  [TemplateVisualState(Name = "Unchecked", GroupName = "CheckStates")]
  public class ToggleSwitchButton : ToggleButton
  {
    private const string CommonStates = "CommonStates";
    private const string NormalState = "Normal";
    private const string DisabledState = "Disabled";
    private const string CheckStates = "CheckStates";
    private const string CheckedState = "Checked";
    private const string DraggingState = "Dragging";
    private const string UncheckedState = "Unchecked";
    private const string SwitchRootPart = "SwitchRoot";
    private const string SwitchBackgroundPart = "SwitchBackground";
    private const string SwitchTrackPart = "SwitchTrack";
    private const string SwitchThumbPart = "SwitchThumb";
    private const double _uncheckedTranslation = 0.0;
    public static readonly DependencyProperty SwitchForegroundProperty = DependencyProperty.Register(nameof (SwitchForeground), typeof (Brush), typeof (ToggleSwitchButton), new PropertyMetadata((PropertyChangedCallback) null));
    private TranslateTransform _backgroundTranslation;
    private TranslateTransform _thumbTranslation;
    private Grid _root;
    private Grid _track;
    private FrameworkElement _thumb;
    private double _checkedTranslation;
    private double _dragTranslation;
    private bool _wasDragged;
    private bool _isDragging;

    public Brush SwitchForeground
    {
      get => (Brush) this.GetValue(ToggleSwitchButton.SwitchForegroundProperty);
      set => this.SetValue(ToggleSwitchButton.SwitchForegroundProperty, (object) value);
    }

    public ToggleSwitchButton() => this.DefaultStyleKey = (object) typeof (ToggleSwitchButton);

    private double Translation
    {
      get
      {
        return this._backgroundTranslation != null ? this._backgroundTranslation.X : this._thumbTranslation.X;
      }
      set
      {
        if (this._backgroundTranslation != null)
          this._backgroundTranslation.X = value;
        if (this._thumbTranslation == null)
          return;
        this._thumbTranslation.X = value;
      }
    }

    private void ChangeVisualState(bool useTransitions)
    {
      if (this.IsEnabled)
        VisualStateManager.GoToState((Control) this, "Normal", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Disabled", useTransitions);
      if (this._isDragging)
        VisualStateManager.GoToState((Control) this, "Dragging", useTransitions);
      else if (((int) this.IsChecked ?? 0) != 0)
        VisualStateManager.GoToState((Control) this, "Checked", useTransitions);
      else
        VisualStateManager.GoToState((Control) this, "Unchecked", useTransitions);
    }

    protected override void OnToggle()
    {
      this.IsChecked = new bool?(((int) this.IsChecked ?? 0) == 0);
      this.ChangeVisualState(true);
    }

    public override void OnApplyTemplate()
    {
      if (this._track != null)
        this._track.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
      if (this._thumb != null)
        this._thumb.SizeChanged -= new SizeChangedEventHandler(this.OnSizeChanged);
      if (this._root != null)
      {
        this._root.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
        this._root.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
        this._root.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
      }
      base.OnApplyTemplate();
      this._root = this.GetTemplateChild("SwitchRoot") as Grid;
      this._backgroundTranslation = !(this.GetTemplateChild("SwitchBackground") is UIElement templateChild) ? (TranslateTransform) null : templateChild.RenderTransform as TranslateTransform;
      this._track = this.GetTemplateChild("SwitchTrack") as Grid;
      this._thumb = (FrameworkElement) (this.GetTemplateChild("SwitchThumb") as Border);
      this._thumbTranslation = this._thumb == null ? (TranslateTransform) null : this._thumb.RenderTransform as TranslateTransform;
      if (this._root != null && this._track != null && this._thumb != null && (this._backgroundTranslation != null || this._thumbTranslation != null))
      {
        this._root.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.OnManipulationStarted);
        this._root.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(this.OnManipulationDelta);
        this._root.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.OnManipulationCompleted);
        this._track.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
        this._thumb.SizeChanged += new SizeChangedEventHandler(this.OnSizeChanged);
      }
      this.ChangeVisualState(false);
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      e.Handled = true;
      this._isDragging = true;
      this._dragTranslation = this.Translation;
      this.ChangeVisualState(true);
      this.Translation = this._dragTranslation;
    }

    private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
      e.Handled = true;
      double x = e.DeltaManipulation.Translation.X;
      if ((Math.Abs(x) >= Math.Abs(e.DeltaManipulation.Translation.Y) ? 1 : 0) != 1 || x == 0.0)
        return;
      this._wasDragged = true;
      this._dragTranslation += x;
      this.Translation = Math.Max(0.0, Math.Min(this._checkedTranslation, this._dragTranslation));
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      e.Handled = true;
      this._isDragging = false;
      bool flag = false;
      if (this._wasDragged)
      {
        if (this.Translation != (((int) this.IsChecked ?? 0) != 0 ? this._checkedTranslation : 0.0))
          flag = true;
      }
      else
        flag = true;
      if (flag)
        this.OnClick();
      this._wasDragged = false;
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this._track.Clip = (Geometry) new RectangleGeometry()
      {
        Rect = new Rect(0.0, 0.0, this._track.ActualWidth, this._track.ActualHeight)
      };
      this._checkedTranslation = this._track.ActualWidth - this._thumb.ActualWidth - this._thumb.Margin.Left - this._thumb.Margin.Right;
    }
  }
}
