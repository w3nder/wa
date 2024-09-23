// Decompiled with JetBrains decompiler
// Type: WhatsApp.StickerPicker
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WhatsApp.CommonOps;

#nullable disable
namespace WhatsApp
{
  public class StickerPicker : UserControl
  {
    private Pivot stickerPivotContainer;
    private WhatsApp.CompatibilityShims.LongListSelector stickerResultsList;
    private TextBlock titleTextBlock;
    private Grid stickersPivotGrid;
    private LinkedList<Sticker> pendingUsageList = new LinkedList<Sticker>();
    public static readonly DependencyProperty StickerCountProperty = DependencyProperty.Register(nameof (StickerCount), typeof (object), typeof (StickerPicker), new PropertyMetadata((PropertyChangedCallback) ((dep, e) =>
    {
      if (!(dep is StickerPicker stickerPicker2))
        return;
      stickerPicker2.OnStickerCountChanged();
    })));
    internal Grid LayoutRoot;
    internal Grid Tray;
    internal ListBox TrayButtonsList;
    private bool _contentLoaded;

    public StickerPickerViewModel ViewModel { get; set; }

    public int StickerCount
    {
      get => (int) this.GetValue(StickerPicker.StickerCountProperty);
      set => this.SetValue(StickerPicker.StickerCountProperty, (object) value);
    }

    public StickerPicker(Button plusButton = null)
    {
      this.InitializeComponent();
      this.ViewModel = new StickerPickerViewModel(plusButton);
      TextBlock textBlock = new TextBlock();
      textBlock.Style = this.Resources[(object) "PhoneTextTitle3Style"] as Style;
      textBlock.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock.VerticalAlignment = VerticalAlignment.Center;
      textBlock.Text = AppResources.RecentlyUsedEmojis;
      textBlock.Foreground = (Brush) UIUtils.ForegroundBrush;
      textBlock.FontSize = UIUtils.FontSizeSmall;
      textBlock.Margin = new Thickness(2.0);
      this.titleTextBlock = textBlock;
      Grid.SetRow((FrameworkElement) this.titleTextBlock, 0);
      WhatsApp.CompatibilityShims.LongListSelector longListSelector = new WhatsApp.CompatibilityShims.LongListSelector();
      longListSelector.Margin = new Thickness(0.0);
      longListSelector.Padding = new Thickness(2.0);
      longListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;
      longListSelector.IsFlatList = true;
      longListSelector.GridCellSize = new Size(118.0, 118.0);
      longListSelector.HorizontalAlignment = HorizontalAlignment.Stretch;
      longListSelector.HorizontalContentAlignment = HorizontalAlignment.Center;
      longListSelector.ItemTemplate = this.Resources[(object) "StickerItemTemplate"] as DataTemplate;
      longListSelector.OverlapScrollBar = true;
      this.stickerResultsList = longListSelector;
      Grid.SetRow((FrameworkElement) this.stickerResultsList, 1);
      this.stickersPivotGrid = new Grid();
      this.stickersPivotGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Auto)
      });
      this.stickersPivotGrid.RowDefinitions.Add(new RowDefinition()
      {
        Height = new GridLength(1.0, GridUnitType.Star)
      });
      this.stickersPivotGrid.Children.Add((UIElement) this.titleTextBlock);
      this.stickersPivotGrid.Children.Add((UIElement) this.stickerResultsList);
      Pivot pivot = new Pivot();
      pivot.HeaderTemplate = (DataTemplate) null;
      pivot.Padding = new Thickness(0.0, -10.0, 0.0, 0.0);
      pivot.Margin = new Thickness(0.0, 4.0, 0.0, 4.0);
      pivot.Background = ImageStore.IsDarkTheme() ? this.Resources[(object) "PhoneInactiveBrush"] as Brush : (Brush) UIUtils.BackgroundBrush;
      this.stickerPivotContainer = pivot;
      ScrollViewer.SetHorizontalScrollBarVisibility((DependencyObject) this.stickerPivotContainer, ScrollBarVisibility.Disabled);
      ScrollViewer.SetVerticalScrollBarVisibility((DependencyObject) this.stickerPivotContainer, ScrollBarVisibility.Visible);
      this.stickerPivotContainer.LoadingPivotItem += new EventHandler<PivotItemEventArgs>(this.StickerPivotContainer_LoadingPivotItem);
      for (int index = 0; index < 3; ++index)
        this.stickerPivotContainer.Items.Add((object) new PivotItem());
      this.LayoutRoot.Children.Add((UIElement) this.stickerPivotContainer);
      Grid.SetRow((FrameworkElement) this.stickerPivotContainer, 1);
      this.OnTrayItemSelected(0);
      this.TrayButtonsList.ItemsSource = (IEnumerable) this.ViewModel.TrayItems;
      MessagesContext.Events.SavedStickerChangedSubject.SubscribeOn<Sticker>((IScheduler) AppState.ImageWorker).ObserveOnDispatcher<Sticker>().Subscribe<Sticker>(new Action<Sticker>(this.UpdateSavedPicker));
    }

    private void UpdateSavedPicker(Sticker sticker)
    {
      if (this.ViewModel.SelectedTrayItem.GroupType != PickerGroupType.Starred)
        return;
      if (sticker.DateTimeStarred.HasValue)
      {
        StickerPickerItemViewModel.GetStickerItemObservableImpl(sticker).Subscribe<StickerPickerItemViewModel>((Action<StickerPickerItemViewModel>) (si =>
        {
          if (this.stickerResultsList == null)
            return;
          this.ProcessStickerItemViewModel(si, (ObservableCollection<StickerPickerItemViewModel>) this.stickerResultsList.ItemsSource);
          this.StickerCount = this.ViewModel.StickerCount = this.stickerResultsList.ItemsSource.Count;
        }));
      }
      else
      {
        foreach (object obj in (IEnumerable) this.stickerResultsList.ItemsSource)
        {
          if (obj is StickerPickerItemViewModel pickerItemViewModel && pickerItemViewModel.Sticker.FileHash == sticker.FileHash)
          {
            this.stickerResultsList.ItemsSource.Remove(obj);
            break;
          }
        }
        this.StickerCount = this.ViewModel.StickerCount = this.stickerResultsList.ItemsSource.Count;
      }
    }

    private void StickerPivotContainer_LoadingPivotItem(object sender, PivotItemEventArgs e)
    {
      if (!(sender is Pivot pivot))
        return;
      int pivotIndex = this.ViewModel.PivotIndex;
      int selectedIndex = pivot.SelectedIndex;
      if (pivotIndex == selectedIndex)
        return;
      if (pivot.Items[pivotIndex] is PivotItem pivotItem)
      {
        if (pivotItem.Content is WhatsApp.CompatibilityShims.LongListSelector content)
          content.ItemsSource = (IList) null;
        pivotItem.Content = (object) null;
      }
      int count = this.ViewModel.TrayItems.Count;
      this.OnTrayItemSelected(pivotIndex == selectedIndex - 1 || pivotIndex == 2 && selectedIndex == 0 ? (this.ViewModel.TrayItemIndex + (count + 1)) % count : (this.ViewModel.TrayItemIndex + (count - 1)) % count);
    }

    private void TrayButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.OnTrayItemSelected(((TrayItemViewModel) ((FrameworkElement) sender).Tag).Index);
    }

    private void OnTrayItemSelected(int newTrayIndex)
    {
      this.ViewModel.TrayItemIndex = newTrayIndex;
      this.RefreshStickers();
    }

    private void RefreshStickers()
    {
      this.stickerResultsList.ItemsSource = (IList) this.GetStickersList((Action) (() => { }));
      if (this.stickerPivotContainer.Items[this.stickerPivotContainer.SelectedIndex] is PivotItem pivotItem)
      {
        pivotItem.Name = "StickerGridContainer";
        pivotItem.Content = (object) this.stickersPivotGrid;
      }
      this.ViewModel.PivotIndex = this.stickerPivotContainer.SelectedIndex;
      this.titleTextBlock.Text = this.ViewModel.TitleTextText;
      this.titleTextBlock.Visibility = this.ViewModel.TitleTextBlockVisibility;
    }

    private void ProcessStickerItemViewModel(
      StickerPickerItemViewModel si,
      ObservableCollection<StickerPickerItemViewModel> itemsSource)
    {
      TrayItemViewModel selectedTrayItem = this.ViewModel.SelectedTrayItem;
      if (si.Thumbnail != null)
      {
        itemsSource.Insert(0, si);
        if (selectedTrayItem.GroupType == PickerGroupType.Starred)
          si.HoldAction = (Action) (() => SaveSticker.UnsaveSticker(si.Sticker));
        si.TappedAction = (Action) (() => this.pendingUsageList.AddLast(si.Sticker));
      }
      else
        SaveSticker.UnsaveSticker(si.Sticker);
    }

    private ObservableCollection<StickerPickerItemViewModel> GetStickersList(Action onComplete)
    {
      TrayItemViewModel trayItem = this.ViewModel.SelectedTrayItem;
      Sticker[] stickers = new Sticker[0];
      ObservableCollection<StickerPickerItemViewModel> itemsSource = new ObservableCollection<StickerPickerItemViewModel>();
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        if (trayItem.GroupType == PickerGroupType.Recent)
          stickers = this.ProcessPendingUsageList();
        else if (trayItem.GroupType == PickerGroupType.Starred)
          stickers = db.GetStarredStickers(new int?(), new int?());
        this.StickerCount = this.ViewModel.StickerCount = stickers.Length;
      }));
      if (trayItem.GroupType == PickerGroupType.Recent)
      {
        foreach (Sticker sticker in stickers)
          StickerPickerItemViewModel.GetStickerItemObservableImpl(sticker).Subscribe<StickerPickerItemViewModel>((Action<StickerPickerItemViewModel>) (si => this.ProcessStickerItemViewModel(si, itemsSource)));
        onComplete();
      }
      else
      {
        int num = Math.Min(this.ViewModel.StickerCount, (this.ViewModel.Orientation & PageOrientation.Portrait) == PageOrientation.Portrait ? 16 : 12);
        for (int index = 0; index < num; ++index)
          StickerPickerItemViewModel.GetStickerItemObservableImpl(stickers[index]).Subscribe<StickerPickerItemViewModel>((Action<StickerPickerItemViewModel>) (si => this.ProcessStickerItemViewModel(si, itemsSource)));
        for (int index = num; index < this.ViewModel.StickerCount; ++index)
        {
          Sticker sticker = stickers[index];
          WAThreadPool.QueueUserWorkItem((Action) (() => Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
          {
            StickerPickerItemViewModel.GetStickerItemObservableImpl(sticker).Subscribe<StickerPickerItemViewModel>((Action<StickerPickerItemViewModel>) (si => this.ProcessStickerItemViewModel(si, itemsSource)));
            onComplete();
          }))));
        }
      }
      return itemsSource;
    }

    public Sticker[] ProcessPendingUsageList()
    {
      Sticker[] r = new Sticker[0];
      int numStickers = this.ViewModel.RecentStickersRows * 4;
      MessagesContext.Run((MessagesContext.MessagesCallback) (db =>
      {
        Sticker[] stickerArray = r = db.GetRecentStickers(new int?(numStickers));
        if (this.pendingUsageList.Count <= 0)
          return;
        db.ProcessStickerUsage((IEnumerable<Sticker>) this.pendingUsageList);
        this.pendingUsageList.Clear();
        r = db.GetRecentStickers(new int?(numStickers));
        foreach (Sticker sticker in r)
          db.LocalFileAddRef(sticker.LocalFileUri, LocalFileType.Sticker);
        foreach (Sticker sticker in stickerArray)
          db.LocalFileRelease(sticker.LocalFileUri, LocalFileType.Sticker);
      }));
      return r;
    }

    private void OnStickerCountChanged()
    {
      this.titleTextBlock.Visibility = this.ViewModel.TitleTextBlockVisibility;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Controls/StickerPicker.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Tray = (Grid) this.FindName("Tray");
      this.TrayButtonsList = (ListBox) this.FindName("TrayButtonsList");
    }
  }
}
