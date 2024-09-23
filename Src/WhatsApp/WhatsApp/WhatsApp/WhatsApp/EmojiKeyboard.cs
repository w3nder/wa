// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiKeyboard
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WhatsApp.WaCollections;
using Windows.Foundation;
using Windows.UI.ViewManagement;


namespace WhatsApp
{
  public class EmojiKeyboard : UserControl
  {
    private PhoneApplicationPage ownerPage_;
    private IDisposable delaySub;
    public static Subject<bool> EmojiPopupManagerSizeChangedSubject = new Subject<bool>();
    public static Subject<Pair<EmojiKeyboard.Actions, object>> ActionSubject = new Subject<Pair<EmojiKeyboard.Actions, object>>();
    private string LogHeader = nameof (EmojiKeyboard);
    private Popup containerPopup_;
    private PopupManager popupManager;
    private EmojiKeyboardViewModel viewModel;
    internal UserControl EmojiKB;
    internal Grid LayoutRoot;
    internal Rectangle LandscapeSysTrayPlaceHolder;
    internal Grid BottomBar;
    internal Button SearchButton;
    internal Image SearchButtonIcon;
    internal Button EmojiButton;
    internal Ellipse EmojiButtonForeground;
    internal ImageBrush EmojiImageBrush;
    internal Button GifButton;
    internal Ellipse GifButtonForeground;
    internal ImageBrush GifImageBrush;
    internal Button StickerButton;
    internal Ellipse StickerButtonForeground;
    internal ImageBrush StickerImageBrush;
    internal Button BackspaceKeyBottom;
    internal Button AddButton;
    internal Grid SearchBar;
    internal Button CancelButton;
    internal Image CancelButtonIcon;
    internal Grid SearchFieldGrid;
    internal TextBox SearchField;
    internal TextBlock SearchFieldTooltipBlock;
    internal Image DeleteIcon;
    private bool _contentLoaded;

    public PhoneApplicationPage OwnerPage
    {
      set
      {
        if (this.ownerPage_ == value)
          return;
        this.ownerPage_ = value;
      }
    }

    public bool IsOpen => this.containerPopup_ != null && this.containerPopup_.IsOpen;

    public Popup ContainerPopup
    {
      get
      {
        Popup containerPopup1 = this.containerPopup_;
        if (containerPopup1 != null)
          return containerPopup1;
        Popup popup = new Popup();
        popup.Child = (UIElement) this;
        Popup containerPopup2 = popup;
        this.containerPopup_ = popup;
        return containerPopup2;
      }
    }

    public TextBox InsertionTextBox
    {
      get => this.EmojiPicker.InsertionTextBox;
      set => this.EmojiPicker.InsertionTextBox = value;
    }

    public event EventHandler Opened;

    protected void NotifyOpened()
    {
      if (this.Opened == null)
        return;
      this.Opened((object) this, new EventArgs());
    }

    public event EventHandler Closed;

    protected void NotifyClosed()
    {
      if (this.Closed == null)
        return;
      this.Closed((object) this, new EventArgs());
    }

    public event EventHandler<EventArgs> DismissedByBackKey;

    protected void NotifyDismissedByBackKey()
    {
      if (this.DismissedByBackKey == null)
        return;
      this.CloseGifPicker();
      this.ResetSearch();
      this.DismissedByBackKey((object) this, new EventArgs());
    }

    private void ResetSearch()
    {
      if (this.EmojiPicker != null && this.viewModel.SelectedType == EmojiKeyboardViewModel.Type.Emoji)
        this.EmojiPicker.Search((string) null);
      if (this.GifPicker != null && this.viewModel.SelectedType == EmojiKeyboardViewModel.Type.Gif)
        this.GifPicker.Search((string) null);
      this.viewModel.IsSearchOpen = false;
      this.UpdateSearchView();
    }

    private void CloseGifPicker()
    {
      if (this.GifPicker == null)
        return;
      this.GifPicker.LayoutRoot.IsHitTestVisible = true;
      this.GifPicker.LoadingPreviewPage.Visibility = Visibility.Collapsed;
    }

    public EmojiPicker EmojiPicker { get; set; }

    public GifPicker GifPicker { get; set; }

    public StickerPicker StickerPicker { get; set; }

    private GifSendingPage GifSendingPage { get; set; }

    private bool GifSearchFocused { get; set; }

    public EmojiKeyboard(
      Action<Emoji.EmojiChar> EmojiAction = null,
      GifSendingPage gifsendingpage = null,
      TextBox insertionTextBox = null,
      bool showStickerPicker = false)
    {
      this.InitializeComponent();
      this.DataContext = (object) (this.viewModel = new EmojiKeyboardViewModel());
      if (gifsendingpage != null)
        this.GifSendingPage = gifsendingpage;
      this.BottomBar.Visibility = Visibility.Visible;
      EmojiPicker emojiPicker = new EmojiPicker(EmojiAction, this.BackspaceKeyBottom, insertionTextBox);
      emojiPicker.HorizontalAlignment = HorizontalAlignment.Stretch;
      this.EmojiPicker = emojiPicker;
      this.EmojiPicker.SearchTermTextBox = this.SearchField;
      this.EmojiPicker.CacheMode = (CacheMode) new BitmapCache();
      Grid.SetRow((FrameworkElement) this.EmojiPicker, 1);
      Grid.SetColumn((FrameworkElement) this.EmojiPicker, 1);
      this.LayoutRoot.Children.Add((UIElement) this.EmojiPicker);
      if (this.GifSendingPage != null)
      {
        GifPicker gifPicker = new GifPicker(this.GifSendingPage);
        gifPicker.HorizontalAlignment = HorizontalAlignment.Stretch;
        gifPicker.Visibility = Visibility.Collapsed;
        this.GifPicker = gifPicker;
        Grid.SetRow((FrameworkElement) this.GifPicker, 1);
        Grid.SetColumn((FrameworkElement) this.GifPicker, 1);
        this.LayoutRoot.Children.Add((UIElement) this.GifPicker);
        this.GifPicker.CacheMode = (CacheMode) new BitmapCache();
        this.GifPicker.GifSearchResultsList.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.GifSearchResultsList_ManipulationStarted);
        this.GifImageBrush.ImageSource = this.viewModel.GifImage;
        this.EmojiImageBrush.ImageSource = this.viewModel.EmojiImage;
        this.EmojiButtonForeground.Fill = (Brush) UIUtils.AccentBrush;
        this.GifButtonForeground.Fill = (Brush) UIUtils.ForegroundBrush;
      }
      else
      {
        this.EmojiButton.Visibility = Visibility.Collapsed;
        this.GifButton.Visibility = Visibility.Collapsed;
      }
      if (showStickerPicker)
      {
        StickerPicker stickerPicker = new StickerPicker();
        stickerPicker.Visibility = Visibility.Collapsed;
        this.StickerPicker = stickerPicker;
        Grid.SetRow((FrameworkElement) this.StickerPicker, 1);
        Grid.SetColumn((FrameworkElement) this.StickerPicker, 1);
        this.LayoutRoot.Children.Add((UIElement) this.StickerPicker);
        this.StickerPicker.CacheMode = (CacheMode) new BitmapCache();
        this.StickerImageBrush.ImageSource = this.viewModel.StickerImage;
        this.StickerButtonForeground.Fill = (Brush) UIUtils.ForegroundBrush;
        this.StickerButton.Visibility = Visibility.Collapsed;
        this.viewModel.IsSearchSupported = this.EmojiPicker.IsSearchSupported;
        if (Settings.StickersEnabled && Settings.StickerPickerEnabled)
          this.StickerButton.Visibility = Visibility.Visible;
      }
      if (this.viewModel.IsSearchSupported)
        this.SearchButton.Visibility = Visibility.Visible;
      this.viewModel.PopupSizeChangedObservable().ObserveOnDispatcher<Unit>().Subscribe<Unit>((Action<Unit>) (_ => this.PopupManager_OrientationChanged((object) this.popupManager, (EventArgs) null)));
      this.SearchFieldTooltipBlock.Foreground = (Brush) UIUtils.BlackBrush;
      this.SearchFieldTooltipBlock.Opacity = ImageStore.IsDarkTheme() ? 0.3 : 0.5;
      this.SearchField.GetFocusChangeObservable().Subscribe<bool>((Action<bool>) (focus =>
      {
        this.viewModel.IsTextBoxFocused = focus;
        Log.d(this.LogHeader, "focus on search changed to {0}", (object) focus);
        if (this.viewModel.SelectedType != EmojiKeyboardViewModel.Type.Emoji || focus)
          return;
        if (this.EmojiPicker.ViewModel.EmojiManipulationStarted)
          this.EmojiPicker.ViewModel.EmojiManipulationStarted = false;
        else
          this.ResetSearch();
      }));
      this.SearchField.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchField_TextChanged));
      InputPane forCurrentView = InputPane.GetForCurrentView();
      // ISSUE: method pointer
      WindowsRuntimeMarshal.AddEventHandler<TypedEventHandler<InputPane, InputPaneVisibilityEventArgs>>(new Func<TypedEventHandler<InputPane, InputPaneVisibilityEventArgs>, EventRegistrationToken>(forCurrentView.add_Showing), new Action<EventRegistrationToken>(forCurrentView.remove_Showing), new TypedEventHandler<InputPane, InputPaneVisibilityEventArgs>((object) this, __methodptr(LogKeyboardSize)));
    }

    public void LogKeyboardSize(InputPane sender, InputPaneVisibilityEventArgs args)
    {
      Windows.Foundation.Rect occludedRect = args.OccludedRect;
      if (occludedRect.Height <= 0.0)
        return;
      Log.d(this.LogHeader, "Showing SIP - Occluded rectangle | Top={0}, Bottom={1}, Height={2}", (object) occludedRect.Top, (object) occludedRect.Bottom, (object) occludedRect.Height);
    }

    public void Open(string searchTerm = null)
    {
      if (this.IsOpen)
        return;
      PopupManager popupManager = new PopupManager(this.ContainerPopup, true);
      this.popupManager = popupManager;
      popupManager.OrientationChanged += new EventHandler<EventArgs>(this.PopupManager_OrientationChanged);
      popupManager.BackKeyHandled += (EventHandler<EventArgs>) ((sender, args) => this.NotifyDismissedByBackKey());
      popupManager.Show();
      if (searchTerm != null)
      {
        this.viewModel.IsSearchOpen = true;
        this.SearchField.Text = searchTerm;
        this.UpdateSearchView();
        this.GifPicker.Search(searchTerm);
      }
      if (this.viewModel.IsSearchOpen)
        this.SearchField.Focus();
      else
        this.EmojiPicker.Focus();
      popupManager.Closed += (EventHandler<EventArgs>) ((sender, args) => this.NotifyClosed());
      this.NotifyOpened();
    }

    public void Close()
    {
      if (!this.IsOpen)
        return;
      this.ContainerPopup.IsOpen = false;
      this.ResetSearch();
      this.CloseGifPicker();
    }

    private void PopupManager_OrientationChanged(object sender, EventArgs e)
    {
      if (!(sender is PopupManager popupManager))
        return;
      Popup popup = popupManager.Popup;
      this.viewModel.Orientation = popupManager.Orientation;
      Thickness thickness = new Thickness();
      for (FrameworkElement parent = VisualTreeHelper.GetParent((DependencyObject) this.ownerPage_) as FrameworkElement; parent != null; parent = VisualTreeHelper.GetParent((DependencyObject) parent) as FrameworkElement)
      {
        thickness.Top += parent.Margin.Top;
        thickness.Left += parent.Margin.Left;
        thickness.Bottom += parent.Margin.Bottom;
        thickness.Right += parent.Margin.Right;
      }
      double num1 = Math.Max(this.ownerPage_.ActualWidth, this.ownerPage_.ActualHeight);
      double num2 = Math.Min(this.ownerPage_.ActualWidth, this.ownerPage_.ActualHeight);
      Math.Max(Application.Current.Host.Content.ActualWidth, Application.Current.Host.Content.ActualHeight);
      bool flag = SystemTray.IsVisible && SystemTray.Opacity < 1.0;
      double num3 = DeviceProfile.Instance.HasSoftNavBar ? 12.0 * ResolutionHelper.ZoomMultiplier : 0.0;
      switch (popupManager.Orientation)
      {
        case PageOrientation.Portrait:
        case PageOrientation.PortraitUp:
          this.LandscapeSysTrayPlaceHolder.Visibility = Visibility.Collapsed;
          popup.HorizontalOffset = 0.0;
          popup.VerticalOffset = thickness.Top + num1 - this.viewModel.PopupHeight;
          popup.Width = num2;
          popup.Height = this.viewModel.PopupHeight;
          break;
        case PageOrientation.Landscape:
        case PageOrientation.LandscapeRight:
          if (flag)
          {
            this.LandscapeSysTrayPlaceHolder.Visibility = Visibility.Visible;
            Grid.SetColumn((FrameworkElement) this.LandscapeSysTrayPlaceHolder, 2);
          }
          else
            this.LandscapeSysTrayPlaceHolder.Visibility = Visibility.Collapsed;
          popup.HorizontalOffset = num2 - UIUtils.SIPHeightLandscape;
          popup.VerticalOffset = -thickness.Left + 2.0 - num3;
          popup.Width = num1 + 2.0 - num3 / 2.0;
          popup.Height = UIUtils.SIPHeightLandscape;
          break;
        case PageOrientation.LandscapeLeft:
          if (flag)
          {
            this.LandscapeSysTrayPlaceHolder.Visibility = Visibility.Visible;
            Grid.SetColumn((FrameworkElement) this.LandscapeSysTrayPlaceHolder, 0);
          }
          else
            this.LandscapeSysTrayPlaceHolder.Visibility = Visibility.Collapsed;
          popup.HorizontalOffset = -(num2 - UIUtils.SIPHeightLandscape);
          popup.VerticalOffset = thickness.Left;
          popup.Width = num1 + 2.0 - num3 / 5.0;
          popup.Height = UIUtils.SIPHeightLandscape;
          break;
      }
      if (popup.Child is FrameworkElement child)
      {
        child.Height = popup.Height;
        child.Width = popup.Width;
      }
      this.EmojiPicker.ViewModel.OverlayHeight = popup.Height;
      this.EmojiPicker.ViewModel.OverlayWidth = popup.Width;
      this.EmojiPicker.Orientation = popupManager.Orientation;
      EmojiKeyboard.EmojiPopupManagerSizeChangedSubject.OnNext(this.IsSearchOpen());
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      if (this.IsOpen)
        this.Close();
      this.StickerPicker?.ProcessPendingUsageList();
      this.EmojiPicker.ApplyPendingEmojiUsages();
      this.EmojiPicker.ApplyPendingEmojiSelectedIndexes();
    }

    private void GifButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.SelectedType = EmojiKeyboardViewModel.Type.Gif;
      this.UpdatePickerView();
    }

    private void EmojiButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.SelectedType = EmojiKeyboardViewModel.Type.Emoji;
      this.UpdatePickerView();
    }

    private void StickerButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.SelectedType = EmojiKeyboardViewModel.Type.Sticker;
      this.UpdatePickerView();
    }

    private void SearchButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.viewModel.IsSearchOpen = true;
      if (this.viewModel.SelectedType == EmojiKeyboardViewModel.Type.Gif)
      {
        this.SearchFieldTooltipBlock.Text = GifProviders.Instance.GetCurrentProvider()?.GetSearchTooltip() ?? "";
        this.SearchField.Text = "";
      }
      else if (this.viewModel.SelectedType == EmojiKeyboardViewModel.Type.Emoji)
      {
        this.SearchFieldTooltipBlock.Text = AppResources.SearchEmojis;
        this.SearchField.Text = "";
        this.EmojiPicker.Search("");
      }
      this.UpdateSearchView();
      this.SearchField.Focus();
    }

    private void CancelButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.ResetSearch();
    }

    private void UpdateSearchView()
    {
      this.SearchFieldTooltipBlock.Visibility = this.TextInputTooltipVisibility;
      this.SearchBar.Visibility = this.viewModel.SearchBarVisibility;
      this.BottomBar.Visibility = this.viewModel.BottomBarVisibility;
    }

    private void UpdatePickerView()
    {
      this.EmojiPicker.Visibility = this.viewModel.EmojiPickerVisibility;
      this.EmojiButtonForeground.Fill = this.viewModel.EmojiButtonForegroundBrush;
      if (this.GifPicker != null)
      {
        this.GifPicker.Visibility = this.viewModel.GifPickerVisibility;
        this.GifButtonForeground.Fill = this.viewModel.GifButtonForegroundBrush;
      }
      if (this.StickerPicker != null)
      {
        this.StickerPicker.Visibility = this.viewModel.StickerPickerVisibility;
        this.StickerButtonForeground.Fill = this.viewModel.StickerButtonForegroundBrush;
      }
      if (this.SearchButton != null)
        this.SearchButton.Visibility = this.viewModel.SearchButtonVisibility;
      if (this.AddButton == null)
        return;
      this.AddButton.Visibility = this.viewModel.AddButtonVisibility;
    }

    private void SearchField_TextChanged(TextChangedEventArgs e)
    {
      string searchTerm = this.SearchField.Text ?? "";
      this.SearchFieldTooltipBlock.Visibility = this.TextInputTooltipVisibility;
      Action searchAction;
      switch (this.viewModel.SelectedType)
      {
        case EmojiKeyboardViewModel.Type.Emoji:
          searchAction = (Action) (() => this.EmojiPicker.Search(searchTerm));
          break;
        case EmojiKeyboardViewModel.Type.Gif:
          searchAction = (Action) (() => this.GifPicker.Search(searchTerm));
          break;
        default:
          searchAction = (Action) (() => { });
          break;
      }
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      this.delaySub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
      {
        Log.d(this.LogHeader, "start search | delayed | search term:[{0}]", (object) searchTerm.Length);
        searchAction();
        this.delaySub.SafeDispose();
        this.delaySub = (IDisposable) null;
      }));
      Log.d(this.LogHeader, "delayed search | search term:[{0}]", (object) searchTerm.Length);
    }

    public Visibility TextInputTooltipVisibility
    {
      get
      {
        return (this.SearchField.Visibility == Visibility.Visible && !this.SearchField.Text.Any<char>()).ToVisibility();
      }
    }

    private void GifSearchResultsList_ManipulationStarted(object sender, EventArgs e)
    {
      this.GifPicker.GifSearchResultsList.Focus();
    }

    private void EmojiSearchResultsList_ManipulationStarted(object sender, EventArgs e)
    {
      this.EmojiPicker.ViewModel.EmojiGridContainer.Focus();
    }

    public bool IsSearchOpen() => this.viewModel.IsSearchOpen;

    public double PickerHeightVisible() => this.viewModel.PickerHeightVisible;

    private void DeleteIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.EmojiPicker.ViewModel.EmojiManipulationStarted = true;
      this.SearchField.Focus();
      this.SearchField.Text = "";
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/EmojiKeyboard.xaml", UriKind.Relative));
      this.EmojiKB = (UserControl) this.FindName("EmojiKB");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.LandscapeSysTrayPlaceHolder = (Rectangle) this.FindName("LandscapeSysTrayPlaceHolder");
      this.BottomBar = (Grid) this.FindName("BottomBar");
      this.SearchButton = (Button) this.FindName("SearchButton");
      this.SearchButtonIcon = (Image) this.FindName("SearchButtonIcon");
      this.EmojiButton = (Button) this.FindName("EmojiButton");
      this.EmojiButtonForeground = (Ellipse) this.FindName("EmojiButtonForeground");
      this.EmojiImageBrush = (ImageBrush) this.FindName("EmojiImageBrush");
      this.GifButton = (Button) this.FindName("GifButton");
      this.GifButtonForeground = (Ellipse) this.FindName("GifButtonForeground");
      this.GifImageBrush = (ImageBrush) this.FindName("GifImageBrush");
      this.StickerButton = (Button) this.FindName("StickerButton");
      this.StickerButtonForeground = (Ellipse) this.FindName("StickerButtonForeground");
      this.StickerImageBrush = (ImageBrush) this.FindName("StickerImageBrush");
      this.BackspaceKeyBottom = (Button) this.FindName("BackspaceKeyBottom");
      this.AddButton = (Button) this.FindName("AddButton");
      this.SearchBar = (Grid) this.FindName("SearchBar");
      this.CancelButton = (Button) this.FindName("CancelButton");
      this.CancelButtonIcon = (Image) this.FindName("CancelButtonIcon");
      this.SearchFieldGrid = (Grid) this.FindName("SearchFieldGrid");
      this.SearchField = (TextBox) this.FindName("SearchField");
      this.SearchFieldTooltipBlock = (TextBlock) this.FindName("SearchFieldTooltipBlock");
      this.DeleteIcon = (Image) this.FindName("DeleteIcon");
    }

    public enum Actions
    {
      ActionedEmoji,
      ActionedGif,
      ActionedSticker,
    }
  }
}
