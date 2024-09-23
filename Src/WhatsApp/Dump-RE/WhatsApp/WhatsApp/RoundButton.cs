// Decompiled with JetBrains decompiler
// Type: WhatsApp.RoundButton
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

#nullable disable
namespace WhatsApp
{
  public class RoundButton : UserControl
  {
    private const double DefaultSize = 48.0;
    private const double DefaultCircleStrokeThickness = 3.2;
    public static readonly DependencyProperty ButtonIconProperty = DependencyProperty.Register(nameof (ButtonIcon), typeof (object), typeof (RoundButton), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as RoundButton).OnButtonIconChanged())));
    public static readonly DependencyProperty ButtonIconReversedProperty = DependencyProperty.Register(nameof (ButtonIconReversed), typeof (object), typeof (RoundButton), new PropertyMetadata((PropertyChangedCallback) ((dep, e) => (dep as RoundButton).OnReversedButtonIconChanged())));
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register(nameof (ButtonSize), typeof (double), typeof (RoundButton), new PropertyMetadata((object) 48.0, (PropertyChangedCallback) ((dep, e) => (dep as RoundButton).OnButtonSizeChanged())));
    public static readonly DependencyProperty CircleStrokeThicknessProperty = DependencyProperty.Register(nameof (CircleStrokeThickness), typeof (double), typeof (RoundButton), new PropertyMetadata((object) 3.2, (PropertyChangedCallback) ((dep, e) => (dep as RoundButton).UpdateButtonBackground(true, false))));
    private SolidColorBrush buttonBrush_;
    private SolidColorBrush buttonBackgroundBrush_;
    public static readonly DependencyProperty ButtonActiveBrushProperty = DependencyProperty.Register(nameof (ButtonActiveBrush), typeof (Brush), typeof (RoundButton), new PropertyMetadata((PropertyChangedCallback) null));
    private bool isActivatable_ = true;
    private bool isCircleColorSet_;
    private DispatcherTimer manipulationTimeoutTimer_;
    public static readonly DependencyProperty ButtonEnabledProperty = DependencyProperty.Register(nameof (ButtonEnabled), typeof (bool), typeof (RoundButton), new PropertyMetadata((object) true, (PropertyChangedCallback) ((dep, e) => (dep as RoundButton).OnButtonEnabledChanged())));
    private bool toAct_ = true;
    internal Grid LayoutRoot;
    internal Ellipse ButtonBackground;
    internal Ellipse ButtonForeground;
    internal ImageBrush ButtonIconImage;
    internal Image ButtonForegroundReversed;
    private bool _contentLoaded;

    public BitmapSource ButtonIcon
    {
      get => this.GetValue(RoundButton.ButtonIconProperty) as BitmapSource;
      set => this.SetValue(RoundButton.ButtonIconProperty, (object) value);
    }

    public BitmapSource ButtonIconReversed
    {
      get => this.GetValue(RoundButton.ButtonIconReversedProperty) as BitmapSource;
      set => this.SetValue(RoundButton.ButtonIconReversedProperty, (object) value);
    }

    public double ButtonSize
    {
      get => (double) this.GetValue(RoundButton.ButtonSizeProperty);
      set => this.SetValue(RoundButton.ButtonSizeProperty, (object) value);
    }

    public double CircleStrokeThickness
    {
      get => (double) this.GetValue(RoundButton.CircleStrokeThicknessProperty);
      set => this.SetValue(RoundButton.CircleStrokeThicknessProperty, (object) value);
    }

    public Action ClickAction { get; set; }

    public Func<bool> ClickIgnoreCheck { get; set; }

    public SolidColorBrush ButtonBrush
    {
      get
      {
        return this.buttonBrush_ ?? (this.buttonBrush_ = Application.Current.Resources[(object) "PhoneForegroundBrush"] as SolidColorBrush);
      }
      set
      {
        if (this.buttonBrush_ == value)
          return;
        this.buttonBrush_ = value;
        this.OnButtonBrushChanged();
      }
    }

    public SolidColorBrush ButtonBackgroundBrush
    {
      get => this.buttonBackgroundBrush_;
      set
      {
        if (this.buttonBackgroundBrush_ == value)
          return;
        this.buttonBackgroundBrush_ = value;
        this.UpdateButtonBackground(false, true);
      }
    }

    public Brush ButtonActiveBrush
    {
      get
      {
        if (this.GetValue(RoundButton.ButtonActiveBrushProperty) is Brush buttonActiveBrush)
          return buttonActiveBrush;
        return !ImageStore.IsDarkTheme() ? (Brush) UIUtils.WhiteBrush : (Brush) UIUtils.BlackBrush;
      }
      set => this.SetValue(RoundButton.ButtonActiveBrushProperty, (object) value);
    }

    public bool IsActivatable
    {
      get => this.isActivatable_;
      set => this.isActivatable_ = value;
    }

    public event EventHandler Click;

    private DispatcherTimer ManipulationTimeoutTimer
    {
      get
      {
        if (this.manipulationTimeoutTimer_ == null)
        {
          this.manipulationTimeoutTimer_ = new DispatcherTimer()
          {
            Interval = TimeSpan.FromMilliseconds(950.0)
          };
          this.manipulationTimeoutTimer_.Tick += new EventHandler(this.ManipulationTimeoutTimer_Tick);
        }
        return this.manipulationTimeoutTimer_;
      }
    }

    public bool ButtonEnabled
    {
      get => (bool) this.GetValue(RoundButton.ButtonEnabledProperty);
      set => this.SetValue(RoundButton.ButtonEnabledProperty, (object) value);
    }

    public new bool IsEnabled
    {
      get => this.ButtonEnabled;
      set => this.ButtonEnabled = value;
    }

    private void OnButtonEnabledChanged() => this.LayoutRoot.Opacity = this.IsEnabled ? 1.0 : 0.3;

    public RoundButton()
    {
      this.InitializeComponent();
      this.UpdateButtonSize();
    }

    public void ActivateButton(bool activate)
    {
      if (!this.IsActivatable || !this.IsEnabled)
        return;
      if (this.ButtonIconReversed != null)
      {
        this.ButtonBackground.Fill = (Brush) (this.ButtonBackgroundBrush ?? UIUtils.TransparentBrush);
        this.ButtonForeground.Visibility = (!activate).ToVisibility();
        this.ButtonForegroundReversed.Visibility = activate.ToVisibility();
      }
      else
      {
        this.ButtonForegroundReversed.Visibility = Visibility.Collapsed;
        this.ButtonForeground.Fill = activate ? this.ButtonActiveBrush : (Brush) this.ButtonBrush;
        this.ButtonBackground.Fill = activate ? (Brush) (this.ButtonBackgroundBrush ?? this.ButtonBrush) : (Brush) UIUtils.TransparentBrush;
      }
    }

    private void TriggerAction()
    {
      if (!this.IsEnabled)
        return;
      if (this.Click != null)
      {
        this.Dispatcher.BeginInvoke((Action) (() => this.Click((object) this, new EventArgs())));
      }
      else
      {
        if (this.ClickAction == null)
          return;
        Action action = this.ClickAction;
        this.Dispatcher.BeginInvoke((Action) (() => action()));
      }
    }

    private void UpdateButtonBackground(bool sizeChanged, bool colorChanged)
    {
      if (sizeChanged)
      {
        this.ButtonBackground.Width = this.ButtonBackground.Height = this.ButtonSize;
        this.ButtonBackground.StrokeThickness = this.CircleStrokeThickness != -1.0 ? this.CircleStrokeThickness : 3.2 * (this.ButtonSize / 48.0);
      }
      if (!colorChanged && this.isCircleColorSet_)
        return;
      this.ButtonBackground.Stroke = (Brush) this.ButtonBrush;
      this.ButtonBackground.Fill = (Brush) (this.ButtonBackgroundBrush ?? UIUtils.TransparentBrush);
      this.isCircleColorSet_ = true;
    }

    private void UpdateButtonForeground(bool updateIcon = true, bool updateReversedIcon = true)
    {
      if (this.ButtonIcon == null)
        this.ButtonIconImage.SetImageSourceSafe((System.Windows.Media.ImageSource) null);
      else if (updateIcon)
      {
        this.ButtonIconImage.SetImageSourceSafe((System.Windows.Media.ImageSource) this.ButtonIcon);
        this.ButtonForeground.Fill = (Brush) this.ButtonBrush;
        this.ButtonForeground.Height = this.ButtonForeground.Width = this.ButtonSize;
      }
      if (this.ButtonIconReversed == null)
      {
        this.ButtonForegroundReversed.Source = (System.Windows.Media.ImageSource) null;
      }
      else
      {
        if (!updateReversedIcon)
          return;
        System.Windows.Media.ImageSource imageSource = !(this.ButtonBrush.Color == Colors.White) ? (System.Windows.Media.ImageSource) IconUtils.CreateColorIcon(this.ButtonIconReversed, (Brush) this.ButtonBrush, new double?(this.ButtonSize)) : (System.Windows.Media.ImageSource) this.ButtonIconReversed;
        this.ButtonForegroundReversed.Width = this.ButtonForegroundReversed.Height = this.ButtonSize;
        this.ButtonForegroundReversed.Source = imageSource;
      }
    }

    private void UpdateButtonSize()
    {
      this.LayoutRoot.Width = this.LayoutRoot.Height = this.ButtonSize;
      this.UpdateButtonBackground(true, false);
      this.UpdateButtonForeground();
    }

    private void OnButtonBrushChanged()
    {
      this.UpdateButtonBackground(false, true);
      this.UpdateButtonForeground();
    }

    private void OnButtonIconChanged() => this.UpdateButtonForeground(updateReversedIcon: false);

    private void OnReversedButtonIconChanged() => this.UpdateButtonForeground(false);

    private void OnButtonSizeChanged()
    {
      if (this.ButtonSize <= 0.0)
        return;
      this.UpdateButtonSize();
    }

    private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
      if (this.ClickIgnoreCheck != null && this.ClickIgnoreCheck())
      {
        this.toAct_ = false;
      }
      else
      {
        this.toAct_ = true;
        this.ActivateButton(true);
        this.ManipulationTimeoutTimer.Start();
      }
    }

    private void Button_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
    {
    }

    private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
      this.ActivateButton(false);
      if (this.toAct_ && Math.Abs(e.TotalManipulation.Translation.X) < this.ButtonSize && Math.Abs(e.TotalManipulation.Translation.Y) < this.ButtonSize)
      {
        this.toAct_ = false;
        this.TriggerAction();
      }
      if (!this.ManipulationTimeoutTimer.IsEnabled)
        return;
      this.ManipulationTimeoutTimer.Stop();
    }

    private void Button_Tap(object sender, GestureEventArgs e)
    {
      this.ActivateButton(false);
      if (!this.toAct_)
        return;
      this.toAct_ = false;
      this.TriggerAction();
    }

    private void ManipulationTimeoutTimer_Tick(object sender, EventArgs e)
    {
      this.toAct_ = false;
      if (this.ManipulationTimeoutTimer.IsEnabled)
        this.ManipulationTimeoutTimer.Stop();
      this.ActivateButton(false);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/RoundButton.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ButtonBackground = (Ellipse) this.FindName("ButtonBackground");
      this.ButtonForeground = (Ellipse) this.FindName("ButtonForeground");
      this.ButtonIconImage = (ImageBrush) this.FindName("ButtonIconImage");
      this.ButtonForegroundReversed = (Image) this.FindName("ButtonForegroundReversed");
    }
  }
}
