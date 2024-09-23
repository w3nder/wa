// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.PhoneTextBox
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
  [TemplatePart(Name = "LengthIndicator", Type = typeof (TextBlock))]
  [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
  [TemplatePart(Name = "HintContent", Type = typeof (ContentControl))]
  [TemplateVisualState(Name = "LengthIndicatorHidden", GroupName = "LengthIndicatorStates")]
  [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "LengthIndicatorVisible", GroupName = "LengthIndicatorStates")]
  [TemplateVisualState(Name = "ReadOnly", GroupName = "CommonStates")]
  [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
  [TemplatePart(Name = "Text", Type = typeof (TextBox))]
  public class PhoneTextBox : TextBox
  {
    private const string RootGridName = "RootGrid";
    private const string TextBoxName = "Text";
    private const string HintContentName = "HintContent";
    private const string HintBorderName = "HintBorder";
    private const string LengthIndicatorName = "LengthIndicator";
    private const string ActionIconCanvasName = "ActionIconCanvas";
    private const string MeasurementTextBlockName = "MeasurementTextBlock";
    private const string ActionIconBorderName = "ActionIconBorder";
    private const string LengthIndicatorStates = "LengthIndicatorStates";
    private const string LengthIndicatorVisibleState = "LengthIndicatorVisible";
    private const string LengthIndicatorHiddenState = "LengthIndicatorHidden";
    private const string CommonStates = "CommonStates";
    private const string NormalState = "Normal";
    private const string DisabledState = "Disabled";
    private const string ReadOnlyState = "ReadOnly";
    private const string FocusStates = "FocusStates";
    private const string FocusedState = "Focused";
    private const string UnfocusedState = "Unfocused";
    private Grid _rootGrid;
    private TextBox _textBox;
    private TextBlock _measurementTextBlock;
    private Brush _foregroundBrushInactive = (Brush) Application.Current.Resources[(object) "PhoneTextBoxReadOnlyBrush"];
    private Brush _foregroundBrushEdit;
    private ContentControl _hintContent;
    private Border _hintBorder;
    private TextBlock _lengthIndicator;
    private bool _ignorePropertyChange;
    private bool _ignoreFocus;
    public static readonly DependencyProperty HintProperty = DependencyProperty.Register(nameof (Hint), typeof (string), typeof (PhoneTextBox), new PropertyMetadata(new PropertyChangedCallback(PhoneTextBox.OnHintPropertyChanged)));
    public static readonly DependencyProperty HintStyleProperty = DependencyProperty.Register(nameof (HintStyle), typeof (Style), typeof (PhoneTextBox), (PropertyMetadata) null);
    public static readonly DependencyProperty ActualHintVisibilityProperty = DependencyProperty.Register(nameof (ActualHintVisibility), typeof (Visibility), typeof (PhoneTextBox), (PropertyMetadata) null);
    public static readonly DependencyProperty LengthIndicatorVisibleProperty = DependencyProperty.Register(nameof (LengthIndicatorVisible), typeof (bool), typeof (PhoneTextBox), (PropertyMetadata) null);
    public static readonly DependencyProperty LengthIndicatorThresholdProperty = DependencyProperty.Register(nameof (LengthIndicatorThreshold), typeof (int), typeof (PhoneTextBox), new PropertyMetadata(new PropertyChangedCallback(PhoneTextBox.OnLengthIndicatorThresholdChanged)));
    public static readonly DependencyProperty DisplayedMaxLengthProperty = DependencyProperty.Register(nameof (DisplayedMaxLength), typeof (int), typeof (PhoneTextBox), new PropertyMetadata(new PropertyChangedCallback(PhoneTextBox.DisplayedMaxLengthChanged)));
    public static readonly DependencyProperty ActionIconProperty = DependencyProperty.Register(nameof (ActionIcon), typeof (ImageSource), typeof (PhoneTextBox), new PropertyMetadata(new PropertyChangedCallback(PhoneTextBox.OnActionIconChanged)));
    public static readonly DependencyProperty HidesActionItemWhenEmptyProperty = DependencyProperty.Register(nameof (HidesActionItemWhenEmpty), typeof (bool), typeof (PhoneTextBox), new PropertyMetadata((object) false, new PropertyChangedCallback(PhoneTextBox.OnActionIconChanged)));

    protected Border ActionIconBorder { get; set; }

    public string Hint
    {
      get => this.GetValue(PhoneTextBox.HintProperty) as string;
      set => this.SetValue(PhoneTextBox.HintProperty, (object) value);
    }

    public Style HintStyle
    {
      get => this.GetValue(PhoneTextBox.HintStyleProperty) as Style;
      set => this.SetValue(PhoneTextBox.HintStyleProperty, (object) value);
    }

    public Visibility ActualHintVisibility
    {
      get => (Visibility) this.GetValue(PhoneTextBox.ActualHintVisibilityProperty);
      set => this.SetValue(PhoneTextBox.ActualHintVisibilityProperty, (object) value);
    }

    private static void OnHintPropertyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is PhoneTextBox phoneTextBox) || phoneTextBox._hintContent == null)
        return;
      phoneTextBox.UpdateHintVisibility();
    }

    private void UpdateHintVisibility()
    {
      if (this._hintContent == null)
        return;
      if (string.IsNullOrEmpty(this.Text))
      {
        this.ActualHintVisibility = Visibility.Visible;
        this.Foreground = this._foregroundBrushInactive;
      }
      else
      {
        this.ActualHintVisibility = Visibility.Collapsed;
        this.Foreground = this._foregroundBrushEdit;
      }
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      this.UpdateHintVisibility();
      base.OnLostFocus(e);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      if (this._ignoreFocus)
      {
        this._ignoreFocus = false;
        (Application.Current.RootVisual as Frame).Focus();
      }
      else
      {
        this.Foreground = this._foregroundBrushEdit;
        if (this._hintContent != null)
          this.ActualHintVisibility = Visibility.Collapsed;
        base.OnGotFocus(e);
      }
    }

    public bool LengthIndicatorVisible
    {
      get => (bool) this.GetValue(PhoneTextBox.LengthIndicatorVisibleProperty);
      set => this.SetValue(PhoneTextBox.LengthIndicatorVisibleProperty, (object) value);
    }

    public int LengthIndicatorThreshold
    {
      get => (int) this.GetValue(PhoneTextBox.LengthIndicatorThresholdProperty);
      set => this.SetValue(PhoneTextBox.LengthIndicatorThresholdProperty, (object) value);
    }

    private static void OnLengthIndicatorThresholdChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      PhoneTextBox phoneTextBox = sender as PhoneTextBox;
      if (phoneTextBox._ignorePropertyChange)
        phoneTextBox._ignorePropertyChange = false;
      else if (phoneTextBox.LengthIndicatorThreshold < 0)
      {
        phoneTextBox._ignorePropertyChange = true;
        phoneTextBox.SetValue(PhoneTextBox.LengthIndicatorThresholdProperty, args.OldValue);
        throw new ArgumentOutOfRangeException("LengthIndicatorThreshold", "The length indicator visibility threshold must be greater than zero.");
      }
    }

    public int DisplayedMaxLength
    {
      get => (int) this.GetValue(PhoneTextBox.DisplayedMaxLengthProperty);
      set => this.SetValue(PhoneTextBox.DisplayedMaxLengthProperty, (object) value);
    }

    private static void DisplayedMaxLengthChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      PhoneTextBox phoneTextBox = sender as PhoneTextBox;
      if (phoneTextBox.DisplayedMaxLength > phoneTextBox.MaxLength && phoneTextBox.MaxLength > 0)
        throw new ArgumentOutOfRangeException("DisplayedMaxLength", "The displayed maximum length cannot be greater than the MaxLength.");
    }

    private void UpdateLengthIndicatorVisibility()
    {
      if (this._rootGrid == null || this._lengthIndicator == null)
        return;
      bool flag = true;
      if (this.LengthIndicatorVisible)
      {
        this._lengthIndicator.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) this.Text.Length, (object) (this.DisplayedMaxLength > 0 ? this.DisplayedMaxLength : this.MaxLength));
        if (this.Text.Length >= this.LengthIndicatorThreshold)
          flag = false;
      }
      VisualStateManager.GoToState((Control) this, flag ? "LengthIndicatorHidden" : "LengthIndicatorVisible", false);
    }

    public ImageSource ActionIcon
    {
      get => this.GetValue(PhoneTextBox.ActionIconProperty) as ImageSource;
      set => this.SetValue(PhoneTextBox.ActionIconProperty, (object) value);
    }

    public bool HidesActionItemWhenEmpty
    {
      get => (bool) this.GetValue(PhoneTextBox.HidesActionItemWhenEmptyProperty);
      set => this.SetValue(PhoneTextBox.HidesActionItemWhenEmptyProperty, (object) value);
    }

    public event EventHandler ActionIconTapped;

    private static void OnActionIconChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is PhoneTextBox phoneTextBox))
        return;
      phoneTextBox.UpdateActionIconVisibility();
    }

    private void UpdateActionIconVisibility()
    {
      if (this.ActionIconBorder == null)
        return;
      if (this.ActionIcon == null || this.HidesActionItemWhenEmpty && string.IsNullOrEmpty(this.Text))
      {
        this.ActionIconBorder.Visibility = Visibility.Collapsed;
        this._hintBorder.Padding = new Thickness(0.0);
      }
      else
      {
        this.ActionIconBorder.Visibility = Visibility.Visible;
        if (this.TextWrapping == TextWrapping.Wrap)
          return;
        this._hintBorder.Padding = new Thickness(0.0, 0.0, 48.0, 0.0);
      }
    }

    private void OnActionIconTapped(object sender, RoutedEventArgs e)
    {
      this._ignoreFocus = true;
      EventHandler actionIconTapped = this.ActionIconTapped;
      if (actionIconTapped == null)
        return;
      actionIconTapped((object) this, (EventArgs) e);
    }

    private void ResizeTextBox()
    {
      if (this.ActionIcon == null || this.TextWrapping != TextWrapping.Wrap)
        return;
      this._measurementTextBlock.Width = this.ActualWidth;
      if (this._measurementTextBlock.ActualHeight > this.ActualHeight - 72.0)
      {
        this.Height = this.ActualHeight + 72.0;
      }
      else
      {
        if (this.ActualHeight <= this._measurementTextBlock.ActualHeight + 144.0)
          return;
        this.Height = this.ActualHeight - 72.0;
      }
    }

    public PhoneTextBox() => this.DefaultStyleKey = (object) typeof (PhoneTextBox);

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (this._textBox != null)
        this._textBox.TextChanged -= new TextChangedEventHandler(this.OnTextChanged);
      if (this.ActionIconBorder != null)
        this.ActionIconBorder.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnActionIconTapped);
      this._rootGrid = this.GetTemplateChild("RootGrid") as Grid;
      this._textBox = this.GetTemplateChild("Text") as TextBox;
      this._foregroundBrushEdit = this.Foreground;
      this._hintContent = this.GetTemplateChild("HintContent") as ContentControl;
      this._hintBorder = this.GetTemplateChild("HintBorder") as Border;
      if (this._hintContent != null)
        this.UpdateHintVisibility();
      this._lengthIndicator = this.GetTemplateChild("LengthIndicator") as TextBlock;
      this.ActionIconBorder = this.GetTemplateChild("ActionIconBorder") as Border;
      if (this._rootGrid != null && this._lengthIndicator != null)
        this.UpdateLengthIndicatorVisibility();
      if (this._textBox != null)
        this._textBox.TextChanged += new TextChangedEventHandler(this.OnTextChanged);
      if (this.ActionIconBorder != null)
      {
        this.ActionIconBorder.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnActionIconTapped);
        this.UpdateActionIconVisibility();
      }
      this._measurementTextBlock = this.GetTemplateChild("MeasurementTextBlock") as TextBlock;
    }

    private void OnTextChanged(object sender, RoutedEventArgs e)
    {
      this.UpdateLengthIndicatorVisibility();
      this.UpdateActionIconVisibility();
      this.UpdateHintVisibility();
      this.ResizeTextBox();
    }
  }
}
