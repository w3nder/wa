// Decompiled with JetBrains decompiler
// Type: WhatsApp.ItemControlBase
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WhatsApp.WaCollections;
using WhatsApp.WaViewModels;


namespace WhatsApp
{
  public class ItemControlBase : Grid
  {
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof (ViewModel), typeof (JidItemViewModel), typeof (ItemControlBase), new PropertyMetadata(new PropertyChangedCallback(ItemControlBase.OnViewModelChanged)));
    protected const int DefaultItemHeight = 68;
    protected int? itemHeight;
    private const double SelectionSize = 32.0;
    protected Grid iconPanel;
    protected Image icon;
    protected Border selectionPanel;
    protected Grid detailsPanel;
    protected Grid titleRow;
    protected JidNameControl nameBlock;
    protected Grid subtitleRow;
    protected RichTextBlock subtitleBlock;
    protected ContextMenu contextMenu;
    private IDisposable iconSub;
    private IDisposable iconFadeInDisposable;
    private IDisposable vmSub;
    private IDisposable vmLazySub;
    protected IDisposable titleSub;
    private System.Windows.Media.ImageSource picLargeSrc;
    private IDisposable picLargeSub;
    private Image picLarge;
    private bool shouldFadeInIcon = true;
    private JidItemViewModel realizedVm;
    private Pair<string, System.Windows.Media.ImageSource> lastAssignedIcon;
    private Popup _popup;
    private Border _popupContent;

    public JidItemViewModel ViewModel
    {
      get => this.GetValue(ItemControlBase.ViewModelProperty) as JidItemViewModel;
      set => this.SetValue(ItemControlBase.ViewModelProperty, (object) value);
    }

    public virtual int ItemHeight
    {
      get => this.itemHeight ?? 68;
      set
      {
        this.itemHeight = new int?(value);
        this.OnItemHeightChanged();
      }
    }

    protected virtual bool ShouldSubToVm => true;

    public ItemControlBase()
    {
      this.InitComponents();
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      this.Unloaded += new RoutedEventHandler(this.OnUnloaded);
      this.CacheMode = (CacheMode) new BitmapCache();
      this.Background = (Brush) new SolidColorBrush(Colors.Transparent);
    }

    private void Icon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      TransitionFrame rootFrame = App.CurrentApp.RootFrame;
      int num1 = (int) (0.9 * Math.Min(rootFrame.ActualWidth, rootFrame.ActualHeight));
      if (this._popup == null)
      {
        this.picLarge.Height = (double) (int) (0.8 * (double) num1);
        this.picLarge.Width = (double) (int) (0.8 * (double) num1);
        Grid grid1 = new Grid();
        grid1.Width = rootFrame.ActualWidth;
        grid1.Height = rootFrame.ActualHeight;
        Grid grid2 = grid1;
        Canvas canvas = new Canvas();
        canvas.Background = (Brush) new SolidColorBrush(Colors.Transparent);
        Grid grid3 = new Grid();
        grid3.Background = (Brush) (Application.Current.Resources[(object) "PhoneBackgroundBrush"] as SolidColorBrush);
        grid3.Width = (double) num1;
        grid3.Height = (double) num1;
        Grid grid4 = grid3;
        this._popupContent = new Border()
        {
          BorderThickness = new Thickness(1.0),
          BorderBrush = (Brush) (Application.Current.Resources[(object) "PhoneBorderBrush"] as SolidColorBrush)
        };
        Popup popup = new Popup();
        popup.HorizontalAlignment = HorizontalAlignment.Center;
        popup.VerticalAlignment = VerticalAlignment.Center;
        this._popup = popup;
        grid4.Children.Add((UIElement) this.picLarge);
        this._popupContent.Child = (UIElement) grid4;
        canvas.Children.Add((UIElement) this._popupContent);
        grid2.Children.Add((UIElement) canvas);
        this._popup.Child = (UIElement) grid2;
        this._popup.Child.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.ClosePopup);
      }
      this.picLarge.Source = this.LargePicSource;
      PageOrientation orientation = App.CurrentApp.CurrentPage.Orientation;
      double num2 = (rootFrame.ActualWidth - (double) num1) / 2.0;
      double num3 = (rootFrame.ActualHeight - (double) num1) / 2.0;
      this._popup.IsOpen = true;
      RotateTransform rotateTransform = new RotateTransform();
      switch (orientation)
      {
        case PageOrientation.LandscapeLeft:
          rotateTransform.Angle = 90.0;
          break;
        case PageOrientation.LandscapeRight:
          rotateTransform.Angle = -90.0;
          break;
      }
      this._popupContent.Child.RenderTransform = (Transform) rotateTransform;
      this._popupContent.Child.RenderTransformOrigin = new System.Windows.Point(0.5, 0.5);
      this._popupContent.RenderTransform = (Transform) new TranslateTransform()
      {
        X = num2,
        Y = num3
      };
      if (App.CurrentApp.CurrentPage is ContactsPage)
        (App.CurrentApp.CurrentPage as ContactsPage).Pivot.IsLocked = true;
      App.CurrentApp.CurrentPage.BackKeyPress += new EventHandler<CancelEventArgs>(this.ClosePopup);
      App.CurrentApp.CurrentPage.OrientationChanged += new EventHandler<OrientationChangedEventArgs>(this.ClosePopup);
      App.CurrentApp.CurrentPage.ApplicationBar.IsVisible = false;
      e.Handled = true;
    }

    private void ClosePopup(object sender, EventArgs e)
    {
      this._popup.IsOpen = false;
      App.CurrentApp.CurrentPage.BackKeyPress -= new EventHandler<CancelEventArgs>(this.ClosePopup);
      App.CurrentApp.CurrentPage.OrientationChanged -= new EventHandler<OrientationChangedEventArgs>(this.ClosePopup);
      App.CurrentApp.CurrentPage.ApplicationBar.IsVisible = true;
      if (App.CurrentApp.CurrentPage is ContactsPage)
        (App.CurrentApp.CurrentPage as ContactsPage).Pivot.IsLocked = false;
      this.DisposeLargeIconSubscription();
    }

    ~ItemControlBase() => this.DisposeSubscriptions(true);

    public static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is ItemControlBase itemControlBase))
        return;
      itemControlBase.RefreshContent(e.OldValue as JidItemViewModel, e.NewValue as JidItemViewModel);
    }

    public void RefreshContent(JidItemViewModel oldVm, JidItemViewModel newVm)
    {
      if (newVm == null)
      {
        this.DisposeSubscriptions(true);
      }
      else
      {
        this.UpdateComponents(newVm);
        this.InitSubscriptions(newVm);
      }
      this.realizedVm = newVm;
    }

    protected virtual void InitComponents()
    {
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Auto)
      });
      this.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      this.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      int itemHeight = this.ItemHeight;
      double num = (double) itemHeight * 0.5;
      Grid grid1 = new Grid();
      grid1.VerticalAlignment = VerticalAlignment.Top;
      grid1.Width = (double) itemHeight;
      grid1.Height = (double) itemHeight;
      grid1.Margin = new Thickness(0.0, 0.0, 6.0, 0.0);
      grid1.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
      this.iconPanel = grid1;
      this.Children.Add((UIElement) this.iconPanel);
      Grid.SetColumn((FrameworkElement) this.iconPanel, 0);
      Grid.SetRow((FrameworkElement) this.iconPanel, 0);
      Image image1 = new Image();
      image1.Stretch = Stretch.UniformToFill;
      image1.HorizontalAlignment = HorizontalAlignment.Center;
      image1.VerticalAlignment = VerticalAlignment.Center;
      this.icon = image1;
      this.iconPanel.Children.Add((UIElement) this.icon);
      Image image2 = new Image();
      image2.HorizontalAlignment = HorizontalAlignment.Center;
      image2.VerticalAlignment = VerticalAlignment.Center;
      this.picLarge = image2;
      Grid grid2 = new Grid();
      grid2.MaxHeight = (double) itemHeight;
      grid2.Margin = new Thickness(12.0, 0.0, 0.0, 0.0);
      this.detailsPanel = grid2;
      this.Children.Add((UIElement) this.detailsPanel);
      Grid.SetColumn((FrameworkElement) this.detailsPanel, 1);
      Grid.SetRow((FrameworkElement) this.detailsPanel, 0);
      this.detailsPanel.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      this.detailsPanel.RowDefinitions.Add(new RowDefinition()
      {
        Height = GridLength.Auto
      });
      Grid grid3 = new Grid();
      grid3.VerticalAlignment = VerticalAlignment.Stretch;
      this.titleRow = grid3;
      this.detailsPanel.Children.Add((UIElement) this.titleRow);
      Grid.SetRow((FrameworkElement) this.titleRow, 0);
      this.titleRow.ColumnDefinitions.Add(new ColumnDefinition()
      {
        Width = new GridLength(1.0, GridUnitType.Star)
      });
      JidNameControl jidNameControl = new JidNameControl();
      jidNameControl.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);
      jidNameControl.FontSize = (double) Application.Current.Resources[(object) "PhoneFontSizeLarge"];
      jidNameControl.FontWeight = FontWeights.Normal;
      jidNameControl.CacheMode = (CacheMode) new BitmapCache();
      jidNameControl.HorizontalAlignment = HorizontalAlignment.Left;
      jidNameControl.VerticalAlignment = VerticalAlignment.Center;
      this.nameBlock = jidNameControl;
      this.titleRow.Children.Add((UIElement) this.nameBlock);
      Grid.SetColumn((FrameworkElement) this.nameBlock, 0);
      Grid grid4 = new Grid();
      grid4.VerticalAlignment = VerticalAlignment.Stretch;
      grid4.MaxHeight = 28.0;
      this.subtitleRow = grid4;
      this.detailsPanel.Children.Add((UIElement) this.subtitleRow);
      Grid.SetRow((FrameworkElement) this.subtitleRow, 1);
      RichTextBlock richTextBlock = new RichTextBlock();
      richTextBlock.Margin = new Thickness(-12.0, 0.0, 0.0, 0.0);
      richTextBlock.VerticalAlignment = VerticalAlignment.Top;
      richTextBlock.FontFamily = UIUtils.FontFamilyNormal;
      richTextBlock.Foreground = UIUtils.SubtleBrush;
      richTextBlock.FontSize = (double) Application.Current.Resources[(object) "PhoneFontSizeNormal"];
      richTextBlock.TextWrapping = TextWrapping.NoWrap;
      richTextBlock.EnableMentionLinks = false;
      this.subtitleBlock = richTextBlock;
      this.subtitleRow.Children.Add((UIElement) this.subtitleBlock);
    }

    protected virtual void UpdateComponents(JidItemViewModel vm)
    {
      this.UpdateContextMenu(vm);
      this.UpdateSelection();
      if (this.lastAssignedIcon == null || !(this.lastAssignedIcon.First == vm.Key) || this.lastAssignedIcon.Second == null)
      {
        this.iconPanel.Background = vm.PictureBackgroundBrush ?? UIUtils.PhoneChromeBrush;
        this.icon.Width = this.icon.Height = vm.PictureSize;
        this.icon.Source = (System.Windows.Media.ImageSource) null;
        this.lastAssignedIcon = (Pair<string, System.Windows.Media.ImageSource>) null;
      }
      this.UpdateTitleRow(vm, true);
    }

    protected virtual void UpdateSubtitleRow(JidItemViewModel vm)
    {
    }

    protected virtual void UpdateTitleRow(JidItemViewModel vm, bool useCache)
    {
      this.nameBlock.Set(vm, useCache);
    }

    public void ShowIconPanel(bool show)
    {
      this.iconPanel.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateContextMenu(JidItemViewModel vm)
    {
      if (!vm.GenerateContextMenu)
        return;
      if (vm.EnableContextMenu && !vm.IsSelected)
      {
        if (this.contextMenu == null)
        {
          this.contextMenu = new ContextMenu()
          {
            IsZoomEnabled = false
          };
          this.contextMenu.Opened += new RoutedEventHandler(this.Menu_Opened);
          this.contextMenu.Closed += new RoutedEventHandler(this.Menu_Closed);
        }
        ContextMenuService.SetContextMenu((DependencyObject) this, this.contextMenu);
      }
      else
        ContextMenuService.SetContextMenu((DependencyObject) this, (ContextMenu) null);
    }

    public virtual void UpdateSelection()
    {
      JidItemViewModel viewModel = this.ViewModel;
      this.UpdateContextMenu(viewModel);
      if (viewModel.IsSelected)
      {
        if (this.selectionPanel == null)
        {
          Border border = new Border();
          border.Background = viewModel.SelectionBackground ?? (Brush) UIUtils.AccentBrush;
          border.VerticalAlignment = VerticalAlignment.Bottom;
          border.HorizontalAlignment = HorizontalAlignment.Right;
          border.Width = 32.0;
          border.Height = 32.0;
          border.CornerRadius = new CornerRadius(16.0);
          this.selectionPanel = border;
          this.Children.Add((UIElement) this.selectionPanel);
          Grid.SetColumn((FrameworkElement) this.selectionPanel, 0);
          Image image = new Image();
          image.Source = viewModel.SelectionIconSource ?? (System.Windows.Media.ImageSource) AssetStore.IncludeCheckIconWhite;
          image.VerticalAlignment = VerticalAlignment.Bottom;
          image.HorizontalAlignment = HorizontalAlignment.Right;
          image.Width = 32.0;
          image.Height = 32.0;
          this.selectionPanel.Child = (UIElement) image;
        }
        this.selectionPanel.Visibility = Visibility.Visible;
      }
      else
      {
        if (this.selectionPanel == null)
          return;
        this.selectionPanel.Visibility = Visibility.Collapsed;
      }
    }

    protected void InitSubscriptions(JidItemViewModel vm)
    {
      this.DisposeSubscriptions(true);
      if (vm == null)
        return;
      this.InitShortSubscriptions(vm);
      this.InitPersistSubscriptions(vm);
    }

    protected virtual void InitShortSubscriptions(JidItemViewModel vm)
    {
      this.ResetIconSubscription(vm);
      this.vmLazySub = vm.ActivateLazySubscriptions();
    }

    protected virtual void InitPersistSubscriptions(JidItemViewModel vm)
    {
      this.DisposePersistSubscriptions();
      if (!this.ShouldSubToVm)
        return;
      IObservable<KeyValuePair<string, object>> observable = vm.GetObservable();
      if (observable == null)
        return;
      this.vmSub = observable.SubscribeOn<KeyValuePair<string, object>>((IScheduler) AppState.Worker).ObserveOnDispatcher<KeyValuePair<string, object>>().Subscribe<KeyValuePair<string, object>>((Action<KeyValuePair<string, object>>) (p => this.OnVmNotified(p.Key, p.Value)));
    }

    protected void DisposeSubscriptions(bool disposePersistSubs)
    {
      this.DisposeShortSubscriptions();
      if (!disposePersistSubs)
        return;
      this.DisposePersistSubscriptions();
    }

    private void DisposeLargeIconSubscription()
    {
      this.picLargeSub.SafeDispose();
      this.picLargeSub = (IDisposable) null;
    }

    protected virtual void DisposeShortSubscriptions()
    {
      this.iconSub.SafeDispose();
      this.iconSub = (IDisposable) null;
      this.DisposeLargeIconSubscription();
      this.vmLazySub.SafeDispose();
      this.vmLazySub = (IDisposable) null;
    }

    protected virtual void DisposePersistSubscriptions()
    {
      this.vmSub.SafeDispose();
      this.vmSub = (IDisposable) null;
    }

    public System.Windows.Media.ImageSource LargePicSource
    {
      get
      {
        if (this.picLargeSrc == null && this.picLargeSub == null)
          this.InitLargePictureSubscription(this.ViewModel);
        return this.picLargeSrc ?? this.icon.Source ?? (System.Windows.Media.ImageSource) AssetStore.DefaultContactIcon;
      }
    }

    private void InitLargePictureSubscription(JidItemViewModel vm)
    {
      this.picLargeSub.SafeDispose();
      this.picLargeSub = ChatPictureStore.Get(vm.Jid, true, true, true).SubscribeOn<ChatPictureStore.PicState>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<ChatPictureStore.PicState>().Subscribe<ChatPictureStore.PicState>((Action<ChatPictureStore.PicState>) (picState =>
      {
        if ((this.picLargeSrc = (System.Windows.Media.ImageSource) picState.Image) == null)
          return;
        this.picLarge.Source = this.LargePicSource;
        vm.NotifyPropertyChanged("LargePicSource");
      }));
    }

    protected virtual void ResetIconSubscription(JidItemViewModel vm)
    {
      if (vm == null)
        return;
      this.iconSub.SafeDispose();
      this.iconSub = vm.GetPictureSourceObservable(true, true).SubscribeOn<System.Windows.Media.ImageSource>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<System.Windows.Media.ImageSource>().Subscribe<System.Windows.Media.ImageSource>((Action<System.Windows.Media.ImageSource>) (iconSrc => this.OnIconFetched(vm, iconSrc)), (Action<Exception>) (ex => this.OnIconFetchFailed(vm)));
    }

    protected virtual void OnVmNotified(string k, object v)
    {
      switch (k)
      {
        case "Refresh":
          this.lastAssignedIcon = (Pair<string, System.Windows.Media.ImageSource>) null;
          this.ResetIconSubscription(this.ViewModel);
          this.UpdateComponents(this.ViewModel);
          break;
        case "Title":
          this.UpdateTitleRow(this.ViewModel, false);
          break;
        case "Subtitle":
          this.UpdateSubtitleRow(this.ViewModel);
          break;
        case "IsSelected":
          this.UpdateSelection();
          break;
        case "EnableContextMenu":
          this.UpdateContextMenu(this.ViewModel);
          break;
      }
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.realizedVm != null)
        return;
      this.RefreshContent((JidItemViewModel) null, this.ViewModel);
    }

    private void OnUnloaded(object sender, EventArgs e)
    {
      this.DisposeSubscriptions(false);
      this.realizedVm = (JidItemViewModel) null;
    }

    protected virtual void OnItemHeightChanged()
    {
      if (this.iconPanel == null)
        return;
      int itemHeight = this.ItemHeight;
      double num = (double) itemHeight * 0.5;
      this.iconPanel.Width = this.iconPanel.Height = (double) itemHeight;
      this.iconPanel.Clip = (Geometry) new EllipseGeometry()
      {
        Center = new System.Windows.Point(num, num),
        RadiusX = num,
        RadiusY = num
      };
      this.detailsPanel.MaxHeight = (double) itemHeight;
      this.nameBlock.FontSize = (double) Application.Current.Resources[(object) "PhoneFontSizeLarge"] * ((double) itemHeight / 68.0);
      this.subtitleBlock.FontSize = (double) Application.Current.Resources[(object) "PhoneFontSizeNormal"] * ((double) itemHeight / 68.0);
    }

    private void OnIconFetched(JidItemViewModel vm, System.Windows.Media.ImageSource iconSrc)
    {
      this.picLargeSrc = (System.Windows.Media.ImageSource) null;
      if (iconSrc == null)
        iconSrc = vm.GetDefaultPicture();
      if (this.lastAssignedIcon == null || !(this.lastAssignedIcon.First == vm.Key) || this.lastAssignedIcon.Second != iconSrc)
      {
        this.icon.Source = iconSrc;
        this.lastAssignedIcon = new Pair<string, System.Windows.Media.ImageSource>(vm.Key, iconSrc);
      }
      if (!this.shouldFadeInIcon)
        return;
      this.shouldFadeInIcon = false;
      this.icon.Opacity = 0.0;
      Storyboard storyboard = WaAnimations.CreateStoryboard(WaAnimations.Fade(WaAnimations.FadeType.FadeIn, TimeSpan.FromMilliseconds(250.0), (DependencyObject) this.icon));
      this.iconFadeInDisposable.SafeDispose();
      this.iconFadeInDisposable = Storyboarder.PerformWithDisposable(storyboard, onComplete: (Action) (() =>
      {
        this.icon.Opacity = 1.0;
        this.iconFadeInDisposable = (IDisposable) null;
      }), callOnCompleteOnDisposing: true, context: "fade in icon");
    }

    private void OnIconFetchFailed(JidItemViewModel vm)
    {
      System.Windows.Media.ImageSource defaultPicture = vm.GetDefaultPicture();
      this.icon.Source = defaultPicture;
      this.lastAssignedIcon = new Pair<string, System.Windows.Media.ImageSource>(vm.Key, defaultPicture);
    }

    private void Menu_Opened(object sender, RoutedEventArgs e)
    {
      JidItemViewModel viewModel = this.ViewModel;
      if (viewModel == null)
        return;
      this.contextMenu.ItemsSource = (IEnumerable) viewModel.GetMenuItems();
    }

    private void Menu_Closed(object sender, RoutedEventArgs e)
    {
      this.contextMenu.ItemsSource = (IEnumerable) null;
    }
  }
}
