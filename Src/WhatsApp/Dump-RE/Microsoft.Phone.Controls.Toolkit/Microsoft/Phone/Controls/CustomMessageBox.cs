// Decompiled with JetBrains decompiler
// Type: Microsoft.Phone.Controls.CustomMessageBox
// Assembly: Microsoft.Phone.Controls.Toolkit, Version=8.0.1.0, Culture=neutral, PublicKeyToken=b772ad94eb9ca604
// MVID: C0F6E8F3-2592-47B2-BAA8-5D2702984A9A
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\Microsoft.Phone.Controls.Toolkit.dll

using Microsoft.Phone.Shell;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

#nullable disable
namespace Microsoft.Phone.Controls
{
  [TemplatePart(Name = "RightButton", Type = typeof (ButtonBase))]
  [TemplatePart(Name = "CaptionTextBlock", Type = typeof (TextBlock))]
  [TemplatePart(Name = "TitleTextBlock", Type = typeof (TextBlock))]
  [TemplatePart(Name = "MessageTextBlock", Type = typeof (TextBlock))]
  [TemplatePart(Name = "LeftButton", Type = typeof (ButtonBase))]
  public class CustomMessageBox : ContentControl
  {
    private const double _systemTrayHeightInPortrait = 32.0;
    private const double _systemTrayWidthInLandscape = 72.0;
    private const string TitleTextBlock = "TitleTextBlock";
    private const string CaptionTextBlock = "CaptionTextBlock";
    private const string MessageTextBlock = "MessageTextBlock";
    private const string LeftButton = "LeftButton";
    private const string RightButton = "RightButton";
    private static WeakReference _currentInstance;
    private static readonly double _screenWidth = Application.Current.Host.Content.ActualWidth;
    private static readonly double _screenHeight = Application.Current.Host.Content.ActualHeight;
    private static bool _mustRestore = true;
    private TextBlock _titleTextBlock;
    private TextBlock _captionTextBlock;
    private TextBlock _messageTextBlock;
    private Button _leftButton;
    private Button _rightButton;
    private Popup _popup;
    private Grid _container;
    private PhoneApplicationFrame _frame;
    private PhoneApplicationPage _page;
    private bool _hasApplicationBar;
    private Color _systemTrayColor;
    private bool _isBeingDismissed;
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof (Title), typeof (string), typeof (CustomMessageBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(CustomMessageBox.OnTitlePropertyChanged)));
    public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(nameof (Caption), typeof (string), typeof (CustomMessageBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(CustomMessageBox.OnCaptionPropertyChanged)));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register(nameof (Message), typeof (string), typeof (CustomMessageBox), new PropertyMetadata((object) string.Empty, new PropertyChangedCallback(CustomMessageBox.OnMessagePropertyChanged)));
    public static readonly DependencyProperty LeftButtonContentProperty = DependencyProperty.Register(nameof (LeftButtonContent), typeof (object), typeof (CustomMessageBox), new PropertyMetadata((object) null, new PropertyChangedCallback(CustomMessageBox.OnLeftButtonContentPropertyChanged)));
    public static readonly DependencyProperty RightButtonContentProperty = DependencyProperty.Register(nameof (RightButtonContent), typeof (object), typeof (CustomMessageBox), new PropertyMetadata((object) null, new PropertyChangedCallback(CustomMessageBox.OnRightButtonContentPropertyChanged)));
    public static readonly DependencyProperty IsLeftButtonEnabledProperty = DependencyProperty.Register(nameof (IsLeftButtonEnabled), typeof (bool), typeof (CustomMessageBox), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsRightButtonEnabledProperty = DependencyProperty.Register(nameof (IsRightButtonEnabled), typeof (bool), typeof (CustomMessageBox), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsFullScreenProperty = DependencyProperty.Register(nameof (IsFullScreen), typeof (bool), typeof (CustomMessageBox), new PropertyMetadata((object) false, new PropertyChangedCallback(CustomMessageBox.OnIsFullScreenPropertyChanged)));

    public event EventHandler<DismissingEventArgs> Dismissing;

    public event EventHandler<DismissedEventArgs> Dismissed;

    public string Title
    {
      get => (string) this.GetValue(CustomMessageBox.TitleProperty);
      set => this.SetValue(CustomMessageBox.TitleProperty, (object) value);
    }

    private static void OnTitlePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if (customMessageBox._titleTextBlock == null)
        return;
      string newValue = (string) e.NewValue;
      customMessageBox._titleTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(newValue);
    }

    public string Caption
    {
      get => (string) this.GetValue(CustomMessageBox.CaptionProperty);
      set => this.SetValue(CustomMessageBox.CaptionProperty, (object) value);
    }

    private static void OnCaptionPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if (customMessageBox._captionTextBlock == null)
        return;
      string newValue = (string) e.NewValue;
      customMessageBox._captionTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(newValue);
    }

    public string Message
    {
      get => (string) this.GetValue(CustomMessageBox.MessageProperty);
      set => this.SetValue(CustomMessageBox.MessageProperty, (object) value);
    }

    private static void OnMessagePropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if (customMessageBox._messageTextBlock == null)
        return;
      string newValue = (string) e.NewValue;
      customMessageBox._messageTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(newValue);
    }

    public object LeftButtonContent
    {
      get => this.GetValue(CustomMessageBox.LeftButtonContentProperty);
      set => this.SetValue(CustomMessageBox.LeftButtonContentProperty, value);
    }

    private static void OnLeftButtonContentPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if (customMessageBox._leftButton == null)
        return;
      customMessageBox._leftButton.Visibility = CustomMessageBox.GetVisibilityFromObject(e.NewValue);
    }

    public object RightButtonContent
    {
      get => this.GetValue(CustomMessageBox.RightButtonContentProperty);
      set => this.SetValue(CustomMessageBox.RightButtonContentProperty, value);
    }

    private static void OnRightButtonContentPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if (customMessageBox._rightButton == null)
        return;
      customMessageBox._rightButton.Visibility = CustomMessageBox.GetVisibilityFromObject(e.NewValue);
    }

    public bool IsLeftButtonEnabled
    {
      get => (bool) this.GetValue(CustomMessageBox.IsLeftButtonEnabledProperty);
      set => this.SetValue(CustomMessageBox.IsLeftButtonEnabledProperty, (object) value);
    }

    public bool IsRightButtonEnabled
    {
      get => (bool) this.GetValue(CustomMessageBox.IsRightButtonEnabledProperty);
      set => this.SetValue(CustomMessageBox.IsRightButtonEnabledProperty, (object) value);
    }

    public bool IsFullScreen
    {
      get => (bool) this.GetValue(CustomMessageBox.IsFullScreenProperty);
      set => this.SetValue(CustomMessageBox.IsFullScreenProperty, (object) value);
    }

    private static void OnIsFullScreenPropertyChanged(
      DependencyObject obj,
      DependencyPropertyChangedEventArgs e)
    {
      CustomMessageBox customMessageBox = (CustomMessageBox) obj;
      if ((bool) e.NewValue)
        customMessageBox.VerticalAlignment = VerticalAlignment.Stretch;
      else
        customMessageBox.VerticalAlignment = VerticalAlignment.Top;
    }

    private void OnBackKeyPress(object sender, CancelEventArgs e)
    {
      e.Cancel = true;
      this.Dismiss(CustomMessageBoxResult.None, true);
    }

    private void OnNavigating(object sender, NavigatingCancelEventArgs e)
    {
      this.Dismiss(CustomMessageBoxResult.None, false);
    }

    public override void OnApplyTemplate()
    {
      if (this._leftButton != null)
        this._leftButton.Click -= new RoutedEventHandler(this.LeftButton_Click);
      if (this._rightButton != null)
        this._rightButton.Click -= new RoutedEventHandler(this.RightButton_Click);
      base.OnApplyTemplate();
      this._titleTextBlock = this.GetTemplateChild("TitleTextBlock") as TextBlock;
      this._captionTextBlock = this.GetTemplateChild("CaptionTextBlock") as TextBlock;
      this._messageTextBlock = this.GetTemplateChild("MessageTextBlock") as TextBlock;
      this._leftButton = this.GetTemplateChild("LeftButton") as Button;
      this._rightButton = this.GetTemplateChild("RightButton") as Button;
      if (this._titleTextBlock != null)
        this._titleTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(this.Title);
      if (this._captionTextBlock != null)
        this._captionTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(this.Caption);
      if (this._messageTextBlock != null)
        this._messageTextBlock.Visibility = CustomMessageBox.GetVisibilityFromString(this.Message);
      if (this._leftButton != null)
      {
        this._leftButton.Click += new RoutedEventHandler(this.LeftButton_Click);
        this._leftButton.Visibility = CustomMessageBox.GetVisibilityFromObject(this.LeftButtonContent);
      }
      if (this._rightButton == null)
        return;
      this._rightButton.Click += new RoutedEventHandler(this.RightButton_Click);
      this._rightButton.Visibility = CustomMessageBox.GetVisibilityFromObject(this.RightButtonContent);
    }

    public CustomMessageBox() => this.DefaultStyleKey = (object) typeof (CustomMessageBox);

    public void Show()
    {
      if (this._popup != null && this._popup.IsOpen)
        return;
      this.LayoutUpdated += new EventHandler(this.CustomMessageBox_LayoutUpdated);
      this._frame = Application.Current.RootVisual as PhoneApplicationFrame;
      this._page = this._frame.Content as PhoneApplicationPage;
      if (SystemTray.IsVisible)
      {
        this._systemTrayColor = SystemTray.BackgroundColor;
        SystemTray.BackgroundColor = !(this.Background is SolidColorBrush) ? (Color) Application.Current.Resources[(object) "PhoneChromeColor"] : ((SolidColorBrush) this.Background).Color;
      }
      if (this._page.ApplicationBar != null)
      {
        this._hasApplicationBar = this._page.ApplicationBar.IsVisible;
        if (this._hasApplicationBar)
          this._page.ApplicationBar.IsVisible = false;
      }
      else
        this._hasApplicationBar = false;
      if (CustomMessageBox._currentInstance != null)
      {
        CustomMessageBox._mustRestore = false;
        if (CustomMessageBox._currentInstance.Target is CustomMessageBox target)
        {
          this._systemTrayColor = target._systemTrayColor;
          this._hasApplicationBar = target._hasApplicationBar;
          target.Dismiss();
        }
      }
      CustomMessageBox._mustRestore = true;
      Rectangle rectangle = new Rectangle();
      Color resource = (Color) Application.Current.Resources[(object) "PhoneBackgroundColor"];
      rectangle.Fill = (Brush) new SolidColorBrush(Color.FromArgb((byte) 153, resource.R, resource.G, resource.B));
      this._container = new Grid();
      this._container.Children.Add((UIElement) rectangle);
      this._container.Children.Add((UIElement) this);
      this._popup = new Popup();
      this._popup.Child = (UIElement) this._container;
      this.SetSizeAndOffset();
      this._popup.IsOpen = true;
      CustomMessageBox._currentInstance = new WeakReference((object) this);
      if (this._page != null)
      {
        this._page.BackKeyPress += new EventHandler<CancelEventArgs>(this.OnBackKeyPress);
        this._page.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
      }
      if (this._frame == null)
        return;
      this._frame.Navigating += new NavigatingCancelEventHandler(this.OnNavigating);
    }

    public void Dismiss() => this.Dismiss(CustomMessageBoxResult.None, true);

    private void Dismiss(CustomMessageBoxResult source, bool useTransition)
    {
      if (this._isBeingDismissed)
        return;
      this._isBeingDismissed = true;
      EventHandler<DismissingEventArgs> dismissing = this.Dismissing;
      if (dismissing != null)
      {
        DismissingEventArgs e = new DismissingEventArgs(source);
        dismissing((object) this, e);
        if (e.Cancel)
        {
          this._isBeingDismissed = false;
          return;
        }
      }
      EventHandler<DismissedEventArgs> dismissed = this.Dismissed;
      if (dismissed != null)
      {
        DismissedEventArgs e = new DismissedEventArgs(source);
        dismissed((object) this, e);
      }
      CustomMessageBox._currentInstance = (WeakReference) null;
      bool restoreOriginalValues = CustomMessageBox._mustRestore;
      if (useTransition)
      {
        ITransition swivelTransition = new SwivelTransition()
        {
          Mode = SwivelTransitionMode.BackwardOut
        }.GetTransition((UIElement) this);
        swivelTransition.Completed += (EventHandler) ((s, e) =>
        {
          swivelTransition.Stop();
          this.ClosePopup(restoreOriginalValues);
        });
        swivelTransition.Begin();
      }
      else
        this.ClosePopup(restoreOriginalValues);
      this._isBeingDismissed = false;
    }

    private void ClosePopup(bool restoreOriginalValues)
    {
      if (this._popup != null)
      {
        this._popup.IsOpen = false;
        this._popup = (Popup) null;
      }
      if (restoreOriginalValues)
      {
        if (SystemTray.IsVisible)
          SystemTray.BackgroundColor = this._systemTrayColor;
        if (this._hasApplicationBar)
        {
          this._hasApplicationBar = false;
          if (this._page.ApplicationBar != null)
            this._page.ApplicationBar.IsVisible = true;
        }
      }
      if (this._page != null)
      {
        this._page.BackKeyPress -= new EventHandler<CancelEventArgs>(this.OnBackKeyPress);
        this._page.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.OnOrientationChanged);
        this._page = (PhoneApplicationPage) null;
      }
      if (this._frame == null)
        return;
      this._frame.Navigating -= new NavigatingCancelEventHandler(this.OnNavigating);
      this._frame = (PhoneApplicationFrame) null;
    }

    private void CustomMessageBox_LayoutUpdated(object sender, EventArgs e)
    {
      ITransition swivelTransition = new SwivelTransition()
      {
        Mode = SwivelTransitionMode.BackwardIn
      }.GetTransition((UIElement) this);
      swivelTransition.Completed += (EventHandler) ((s1, e1) => swivelTransition.Stop());
      swivelTransition.Begin();
      this.LayoutUpdated -= new EventHandler(this.CustomMessageBox_LayoutUpdated);
    }

    private void LeftButton_Click(object sender, RoutedEventArgs e)
    {
      this.Dismiss(CustomMessageBoxResult.LeftButton, true);
    }

    private void RightButton_Click(object sender, RoutedEventArgs e)
    {
      this.Dismiss(CustomMessageBoxResult.RightButton, true);
    }

    private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
    {
      this.SetSizeAndOffset();
    }

    private void SetSizeAndOffset()
    {
      Rect transformedRect = CustomMessageBox.GetTransformedRect();
      if (this._container != null)
      {
        this._container.RenderTransform = CustomMessageBox.GetTransform();
        this._container.Width = transformedRect.Width;
        this._container.Height = transformedRect.Height;
      }
      if (!SystemTray.IsVisible || this._popup == null)
        return;
      switch (CustomMessageBox.GetPageOrientation())
      {
        case PageOrientation.PortraitUp:
          this._popup.HorizontalOffset = 0.0;
          this._popup.VerticalOffset = 32.0;
          this._container.Height -= 32.0;
          break;
        case PageOrientation.LandscapeLeft:
          this._popup.HorizontalOffset = 0.0;
          this._popup.VerticalOffset = 72.0;
          break;
        case PageOrientation.LandscapeRight:
          this._popup.HorizontalOffset = 0.0;
          this._popup.VerticalOffset = 0.0;
          break;
      }
    }

    private static Rect GetTransformedRect()
    {
      bool flag = CustomMessageBox.IsLandscape(CustomMessageBox.GetPageOrientation());
      return new Rect(0.0, 0.0, flag ? CustomMessageBox._screenHeight : CustomMessageBox._screenWidth, flag ? CustomMessageBox._screenWidth : CustomMessageBox._screenHeight);
    }

    private static bool IsLandscape(PageOrientation orientation)
    {
      return orientation == PageOrientation.Landscape || orientation == PageOrientation.LandscapeLeft || orientation == PageOrientation.LandscapeRight;
    }

    private static Transform GetTransform()
    {
      switch (CustomMessageBox.GetPageOrientation())
      {
        case PageOrientation.Landscape:
        case PageOrientation.LandscapeLeft:
          return (Transform) new CompositeTransform()
          {
            Rotation = 90.0,
            TranslateX = CustomMessageBox._screenWidth
          };
        case PageOrientation.LandscapeRight:
          return (Transform) new CompositeTransform()
          {
            Rotation = -90.0,
            TranslateY = CustomMessageBox._screenHeight
          };
        default:
          return (Transform) null;
      }
    }

    private static PageOrientation GetPageOrientation()
    {
      return Application.Current.RootVisual is PhoneApplicationFrame rootVisual && rootVisual.Content is PhoneApplicationPage content ? content.Orientation : PageOrientation.None;
    }

    private static Visibility GetVisibilityFromString(string str)
    {
      return string.IsNullOrEmpty(str) ? Visibility.Collapsed : Visibility.Visible;
    }

    private static Visibility GetVisibilityFromObject(object obj)
    {
      return obj == null ? Visibility.Collapsed : Visibility.Visible;
    }
  }
}
