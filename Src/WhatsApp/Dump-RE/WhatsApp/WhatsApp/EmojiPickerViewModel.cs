// Decompiled with JetBrains decompiler
// Type: WhatsApp.EmojiPickerViewModel
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WhatsApp.Controls;

#nullable disable
namespace WhatsApp
{
  public class EmojiPickerViewModel : PropChangedBase
  {
    public EmojiPicker Driver;
    public Button[] BackspaceButtons;
    public EmojiPickerModel Model;
    private Emoji.PickerCategory currentEmojiGroupIndex = Emoji.PickerCategory.SmileysAndPeople;
    public Pivot EmojiGridContainer;
    public FrameworkElement Keyboard;
    public bool PopupOpen;
    private PageOrientation orientation;
    private static Brush inactiveBrush_;
    private int OldPivotIndex_;
    private Grid RecentEmojiGrid;
    private Grid SearchEmojiGrid;
    private TextBlock RecentEmojiTextBlock;
    private TextBlock SearchEmojiTextBlock;
    private static Dictionary<string, int> cachedEmojiSelectedIndexes_;
    private static Dictionary<string, int> pendingEmojiSelectedIndexes_;
    public Grid CurrentEmojiKeyboardItem;
    public EmojiPopup PopupSelector;
    public System.Windows.Point PopupTopLeft;
    private DispatcherTimer BackspaceHolder;

    public Emoji.PickerCategory CurrentEmojiGroupIndex
    {
      get => this.currentEmojiGroupIndex;
      set
      {
        Emoji.PickerCategory newGroup = value;
        Emoji.PickerCategory currentEmojiGroupIndex = this.currentEmojiGroupIndex;
        if (currentEmojiGroupIndex == newGroup)
          return;
        this.SaveCurrentGroupFirstVisibleOffset();
        this.currentEmojiGroupIndex = newGroup;
        this.OnGroupChanged(newGroup, currentEmojiGroupIndex);
        Settings.LastEmojiTab = (int) newGroup;
        if (newGroup == Emoji.PickerCategory.Recent)
          this.Model.UpdateRecentEmojis();
        this.ResetEmojiGrid();
      }
    }

    public EmojiViewModelGroup CurrentEmojiGroup
    {
      get => this.Model.getEmojiGroup(this.currentEmojiGroupIndex);
    }

    public string SearchTerm { get; set; }

    public TextBox InsertionTextBox { get; set; }

    public double OverlayHeight { get; set; }

    public double OverlayWidth { get; set; }

    public TextBox SearchTermTextBox { get; set; }

    public PageOrientation Orientation
    {
      get => this.orientation;
      set
      {
        if (this.orientation == value)
          return;
        this.orientation = value;
        this.SaveCurrentGroupFirstVisibleOffset();
        this.ResetEmojiGrid();
        this.OnOrientationChanged();
      }
    }

    public int GridColumns
    {
      get => (this.Orientation & PageOrientation.Portrait) != PageOrientation.Portrait ? 9 : 7;
    }

    public int RecentsGridRows
    {
      get => (this.Orientation & PageOrientation.Portrait) != PageOrientation.Portrait ? 3 : 4;
    }

    public int RecentsPageMaxSize => 3 * this.GridColumns;

    public static Brush InactiveBrush
    {
      get
      {
        return EmojiPickerViewModel.inactiveBrush_ ?? (EmojiPickerViewModel.inactiveBrush_ = (Brush) new SolidColorBrush(Colors.Transparent));
      }
    }

    public static Brush ActiveBrush => (Brush) UIUtils.AccentBrush;

    public int EmojiGridMargin
    {
      get => (this.Orientation & PageOrientation.Portrait) != PageOrientation.Portrait ? 6 : 4;
    }

    public double SideGroupHeaderItemWidth => 72.0 * ResolutionHelper.ZoomMultiplier;

    public void SaveCurrentGroupFirstVisibleOffset()
    {
      if (this.EmojiGridContainer == null)
        return;
      ListBox visualTreeByType1 = UIUtils.FindFirstInVisualTreeByType<ListBox>((DependencyObject) this.EmojiGridContainer);
      if (visualTreeByType1 == null)
        return;
      int gridColumns = this.GridColumns;
      ScrollViewer visualTreeByType2 = UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) visualTreeByType1);
      if (visualTreeByType2 == null)
        return;
      this.CurrentEmojiGroup.FirstVisibleEmoji = (int) Math.Floor(visualTreeByType2.VerticalOffset) * gridColumns;
    }

    public void ResetEmojiGrid()
    {
      if (this.PopupSelector != null)
        this.PopupSelector.IsOpen = false;
      ListBox box = new ListBox();
      box.ItemTemplate = this.Driver.Resources[(object) "EmojiRowTemplate"] as DataTemplate;
      FrameworkElement frameworkElement;
      if (this.IsSearchActive())
      {
        if (this.SearchEmojiTextBlock == null)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.Style = this.Driver.Resources[(object) "PhoneTextTitle3Style"] as Style;
          textBlock.HorizontalAlignment = HorizontalAlignment.Center;
          textBlock.VerticalAlignment = VerticalAlignment.Center;
          textBlock.Text = AppResources.NoResultsUpper;
          textBlock.Foreground = this.Driver.Resources[(object) "PhoneForegroundBrush"] as Brush;
          textBlock.FontSize = (double) this.Driver.Resources[(object) "PhoneFontSizeMedium"];
          textBlock.Margin = new Thickness(2.0);
          textBlock.Height = 60.0 * ResolutionHelper.ZoomMultiplier;
          this.SearchEmojiTextBlock = textBlock;
          this.SearchEmojiTextBlock.CacheMode = (CacheMode) new BitmapCache();
        }
        if (this.SearchEmojiGrid == null)
        {
          box.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
          this.SearchEmojiGrid = new Grid();
          this.SearchEmojiGrid.RowDefinitions.Add(new RowDefinition()
          {
            Height = new GridLength(1.0, GridUnitType.Auto)
          });
          this.SearchEmojiGrid.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          this.SearchEmojiGrid.Children.Add((UIElement) this.SearchEmojiTextBlock);
          this.SearchEmojiGrid.Children.Add((UIElement) box);
          box.CacheMode = (CacheMode) new BitmapCache();
          box.ItemsPanel = this.Driver.Resources[(object) "HorizontalItemPanelTemplate"] as ItemsPanelTemplate;
          ScrollViewer.SetHorizontalScrollBarVisibility((DependencyObject) box, ScrollBarVisibility.Hidden);
          ScrollViewer.SetVerticalScrollBarVisibility((DependencyObject) box, ScrollBarVisibility.Disabled);
        }
        frameworkElement = (FrameworkElement) this.SearchEmojiGrid;
        this.SearchEmojiTextBlock.Visibility = (this.Model.EmojiSearchGroup.Values.Length == 0).ToVisibility();
      }
      else
      {
        Emoji.PickerCategory? groupName = this.CurrentEmojiGroup.GroupName;
        Emoji.PickerCategory pickerCategory = Emoji.PickerCategory.Recent;
        if ((groupName.GetValueOrDefault() == pickerCategory ? (groupName.HasValue ? 1 : 0) : 0) != 0)
        {
          if (this.RecentEmojiTextBlock == null)
          {
            TextBlock textBlock = new TextBlock();
            textBlock.Style = this.Driver.Resources[(object) "PhoneTextTitle3Style"] as Style;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Text = AppResources.RecentlyUsedEmojis;
            textBlock.Foreground = this.Driver.Resources[(object) "PhoneForegroundBrush"] as Brush;
            textBlock.FontSize = (double) this.Driver.Resources[(object) "PhoneFontSizeSmall"];
            textBlock.Margin = new Thickness(2.0);
            this.RecentEmojiTextBlock = textBlock;
            this.RecentEmojiTextBlock.CacheMode = (CacheMode) new BitmapCache();
          }
          if (this.RecentEmojiGrid == null)
          {
            box.Margin = new Thickness(0.0, 2.0, 0.0, 0.0);
            ScrollViewer.SetVerticalScrollBarVisibility((DependencyObject) box, ScrollBarVisibility.Disabled);
            Grid.SetRow((FrameworkElement) box, 1);
            this.RecentEmojiGrid = new Grid();
            this.RecentEmojiGrid.RowDefinitions.Add(new RowDefinition()
            {
              Height = new GridLength(1.0, GridUnitType.Auto)
            });
            this.RecentEmojiGrid.RowDefinitions.Add(new RowDefinition()
            {
              Height = new GridLength(1.0, GridUnitType.Auto)
            });
            this.RecentEmojiGrid.Children.Add((UIElement) this.RecentEmojiTextBlock);
            this.RecentEmojiGrid.Children.Add((UIElement) box);
            box.CacheMode = (CacheMode) new BitmapCache();
          }
          frameworkElement = (FrameworkElement) this.RecentEmojiGrid;
        }
        else
          frameworkElement = (FrameworkElement) box;
      }
      this.Keyboard = frameworkElement;
      this.Keyboard.CacheMode = (CacheMode) new BitmapCache();
      Action onComplete1 = (Action) (() =>
      {
        int rowStart = (int) Math.Floor((double) this.CurrentEmojiGroup.FirstVisibleEmoji / (double) this.GridColumns);
        ScrollViewer visualTreeByType = UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) box);
        if (visualTreeByType != null)
          visualTreeByType.ScrollToVerticalOffset((double) rowStart);
        else
          this.Driver.Dispatcher.BeginInvoke((Action) (() => UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) box)?.ScrollToVerticalOffset((double) rowStart)));
      });
      Action onComplete2 = (Action) (() =>
      {
        ScrollViewer visualTreeByType1 = UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) this.SearchEmojiGrid);
        if (visualTreeByType1 != null)
        {
          Log.d("EmojiPicker", "Scrolling to far left");
          visualTreeByType1.ScrollToHorizontalOffset(0.0);
        }
        else
          this.Driver.Dispatcher.BeginInvoke((Action) (() =>
          {
            ScrollViewer visualTreeByType2 = UIUtils.FindFirstInVisualTreeByType<ScrollViewer>((DependencyObject) this.SearchEmojiGrid);
            if (visualTreeByType2 == null)
              return;
            Log.d("EmojiPicker", "Scrolling to far left");
            visualTreeByType2.ScrollToHorizontalOffset(0.0);
          }));
      });
      this.Driver.CategoryRow.Visibility = Visibility.Visible;
      this.EmojiGridContainer.IsLocked = false;
      if (this.IsSearchActive())
      {
        if (this.SearchEmojiGrid.Children[1] is ListBox child)
          child.ItemsSource = (IEnumerable) this.CreateEmojiGrid(onComplete2, this.Model.EmojiSearchGroup);
        this.Driver.CategoryRow.Visibility = Visibility.Collapsed;
        this.EmojiGridContainer.IsLocked = true;
      }
      else
      {
        Emoji.PickerCategory? groupName = this.CurrentEmojiGroup.GroupName;
        Emoji.PickerCategory pickerCategory = Emoji.PickerCategory.Recent;
        if ((groupName.GetValueOrDefault() == pickerCategory ? (groupName.HasValue ? 1 : 0) : 0) != 0 && this.RecentEmojiGrid != null)
        {
          if (this.RecentEmojiGrid.Children[1] is ListBox child)
            child.ItemsSource = (IEnumerable) this.CreateEmojiGrid(onComplete1);
        }
        else
          box.ItemsSource = (IEnumerable) this.CreateEmojiGrid(onComplete1);
      }
      if (this.EmojiGridContainer.Items[this.EmojiGridContainer.SelectedIndex] is PivotItem pivotItem)
      {
        pivotItem.Name = "EmojiGridContainer";
        pivotItem.Content = (object) null;
        pivotItem.Content = (object) frameworkElement;
      }
      this.OldPivotIndex_ = this.EmojiGridContainer.SelectedIndex;
    }

    private bool IsSearchActive() => this.SearchTerm != null;

    public void RefocusOnSearchTextBox()
    {
      if (!this.IsSearchActive())
        return;
      this.SearchTermTextBox.Focus();
    }

    public bool EmojiManipulationStarted { get; set; }

    private ObservableCollection<EmojiRowContext> CreateEmojiGrid(
      Action onComplete,
      EmojiViewModelGroup currentEmojiGroup = null)
    {
      if (currentEmojiGroup == null)
        currentEmojiGroup = this.CurrentEmojiGroup;
      int cols = this.GridColumns;
      int recentsGridRows = this.RecentsGridRows;
      int rows = (int) Math.Ceiling((double) currentEmojiGroup.Values.Length / (double) cols);
      int prefetch = Math.Min(rows, (this.Orientation & PageOrientation.Portrait) == PageOrientation.Portrait ? 5 : 4);
      int rowStart = (int) Math.Floor((double) currentEmojiGroup.FirstVisibleEmoji / (double) cols);
      ObservableCollection<EmojiRowContext> itemsSource = new ObservableCollection<EmojiRowContext>();
      for (int index1 = rowStart; index1 < rowStart + prefetch; ++index1)
      {
        Emoji.PickerCategory? groupName = currentEmojiGroup.GroupName;
        Emoji.PickerCategory pickerCategory = Emoji.PickerCategory.Recent;
        if ((groupName.GetValueOrDefault() == pickerCategory ? (groupName.HasValue ? 1 : 0) : 0) != 0 && index1 == recentsGridRows)
          return itemsSource;
        Emoji.EmojiChar[] emojiCharArray = new Emoji.EmojiChar[cols];
        bool[] flagArray = new bool[cols];
        for (int index2 = 0; index2 < cols && index1 * cols + index2 < currentEmojiGroup.Values.Length; ++index2)
        {
          bool openPickerOnTap;
          emojiCharArray[index2] = this.GetEmojiCharForPicker(currentEmojiGroup.Values[index1 * cols + index2], out openPickerOnTap);
          flagArray[index2] = openPickerOnTap;
        }
        EmojiRowContext emojiRowContext = new EmojiRowContext()
        {
          tapActions = flagArray,
          chars = emojiCharArray,
          action = currentEmojiGroup.OnEmojiSelectedAction,
          args = new Emoji.EmojiChar.Args[cols],
          viewmodel = this
        };
        itemsSource.Add(emojiRowContext);
      }
      if (rowStart > 0 || rows - (rowStart + prefetch) > 0)
        WAThreadPool.QueueUserWorkItem((Action) (() => Deployment.Current.Dispatcher.BeginInvoke((Action) (() =>
        {
          if (rowStart > 0)
          {
            for (int index3 = rowStart - 1; index3 >= 0; --index3)
            {
              Emoji.EmojiChar[] emojiCharArray = new Emoji.EmojiChar[cols];
              bool[] flagArray = new bool[cols];
              for (int index4 = 0; index4 < cols && index3 * cols + index4 < currentEmojiGroup.Values.Length; ++index4)
              {
                bool openPickerOnTap;
                emojiCharArray[index4] = this.GetEmojiCharForPicker(currentEmojiGroup.Values[index3 * cols + index4], out openPickerOnTap);
                flagArray[index4] = openPickerOnTap;
              }
              itemsSource.Insert(0, new EmojiRowContext()
              {
                tapActions = flagArray,
                chars = emojiCharArray,
                action = currentEmojiGroup.OnEmojiSelectedAction,
                args = new Emoji.EmojiChar.Args[cols],
                viewmodel = this
              });
            }
          }
          if (rows - (rowStart + prefetch) > 0)
          {
            for (int index5 = rowStart + prefetch; index5 < rows; ++index5)
            {
              Emoji.EmojiChar[] emojiCharArray = new Emoji.EmojiChar[cols];
              bool[] flagArray = new bool[cols];
              for (int index6 = 0; index6 < cols && index5 * cols + index6 < currentEmojiGroup.Values.Length; ++index6)
              {
                bool openPickerOnTap;
                emojiCharArray[index6] = this.GetEmojiCharForPicker(currentEmojiGroup.Values[index5 * cols + index6], out openPickerOnTap);
                flagArray[index6] = openPickerOnTap;
              }
              itemsSource.Add(new EmojiRowContext()
              {
                tapActions = flagArray,
                chars = emojiCharArray,
                action = currentEmojiGroup.OnEmojiSelectedAction,
                args = new Emoji.EmojiChar.Args[cols],
                viewmodel = this
              });
            }
          }
          onComplete();
        }))));
      return itemsSource;
    }

    public static Dictionary<string, int> CachedEmojiSelectedIndexes
    {
      set => EmojiPickerViewModel.cachedEmojiSelectedIndexes_ = value;
      get
      {
        if (EmojiPickerViewModel.cachedEmojiSelectedIndexes_ == null)
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => EmojiPickerViewModel.cachedEmojiSelectedIndexes_ = db.GetAllEmojiSelectedIndexes()));
        return EmojiPickerViewModel.cachedEmojiSelectedIndexes_;
      }
    }

    public static Dictionary<string, int> PendingEmojiSelectedIndexes
    {
      set => EmojiPickerViewModel.pendingEmojiSelectedIndexes_ = value;
      get
      {
        if (EmojiPickerViewModel.pendingEmojiSelectedIndexes_ == null)
          EmojiPickerViewModel.pendingEmojiSelectedIndexes_ = new Dictionary<string, int>();
        return EmojiPickerViewModel.pendingEmojiSelectedIndexes_;
      }
    }

    private static int GetSavedEmojiIndex(string codepoint)
    {
      int num;
      return EmojiPickerViewModel.PendingEmojiSelectedIndexes.TryGetValue(codepoint, out num) || EmojiPickerViewModel.CachedEmojiSelectedIndexes.TryGetValue(codepoint, out num) ? num : -1;
    }

    private Emoji.EmojiChar GetEmojiCharForPicker(string unicodeCodepoint, out bool openPickerOnTap)
    {
      Emoji.EmojiChar emojiChar = Emoji.GetEmojiChar(unicodeCodepoint);
      if (Emoji.IsVariantSelector(unicodeCodepoint))
      {
        string baseEmoji = Emoji.GetBaseEmoji(unicodeCodepoint);
        int savedEmojiIndex = EmojiPickerViewModel.GetSavedEmojiIndex(baseEmoji);
        if (savedEmojiIndex == -1)
        {
          openPickerOnTap = true;
          return Emoji.GetEmojiChar(unicodeCodepoint);
        }
        openPickerOnTap = false;
        return Emoji.GetEmojiChar(Emoji.GetSingleEmojiVariant(baseEmoji, savedEmojiIndex));
      }
      openPickerOnTap = false;
      return emojiChar;
    }

    public void ApplyPendingEmojiSelectedIndexes()
    {
      MessagesContext.Run((MessagesContext.MessagesCallback) (db => db.ApplyAllEmojiSelectedIndexes(EmojiPickerViewModel.PendingEmojiSelectedIndexes)));
    }

    public System.Windows.Media.ImageSource RecentImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.Recent)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiRecentButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource SmileysAndPeopleImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.SmileysAndPeople)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiPeopleButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource AnimalsAndNatureImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.AnimalsAndNature)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiNatureButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource ActivityImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.Activity)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiActivityButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource FoodAndDrinkImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.FoodAndDrink)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiFoodButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource TravelAndPlacesImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.TravelAndPlaces)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiTravelButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource ObjectsImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.Objects)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiObjectsButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource SymbolsImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.Symbols)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiSymbolsButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource FlagsImage
    {
      get
      {
        AssetStore.ThemeSetting requested = AssetStore.ThemeSetting.ToBeDetermined;
        if (this.CurrentEmojiGroupIndex == Emoji.PickerCategory.Flags)
          requested = AssetStore.ThemeSetting.Dark;
        return (System.Windows.Media.ImageSource) AssetStore.EmojiFlagsButtonIcon(requested);
      }
    }

    public System.Windows.Media.ImageSource BackspaceImage
    {
      get => (System.Windows.Media.ImageSource) AssetStore.KeypadBackSpaceIcon;
    }

    public Brush RecentBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.Recent ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush SmileysAndPeopleBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.SmileysAndPeople ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush AnimalsAndNatureBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.AnimalsAndNature ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush ActivityBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.Activity ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush FoodAndDrinkBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.FoodAndDrink ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush TravelAndPlacesBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.TravelAndPlaces ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush ObjectsBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.Objects ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush SymbolsBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.Symbols ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    public Brush FlagsBackground
    {
      get
      {
        return this.CurrentEmojiGroupIndex != Emoji.PickerCategory.Flags ? EmojiPickerViewModel.InactiveBrush : EmojiPickerViewModel.ActiveBrush;
      }
    }

    private void OnGroupChanged(Emoji.PickerCategory newGroup, Emoji.PickerCategory oldGroup)
    {
      if (newGroup == Emoji.PickerCategory.Recent || oldGroup == Emoji.PickerCategory.Recent)
      {
        this.NotifyPropertyChanged("RecentBackground");
        this.NotifyPropertyChanged("RecentImage");
      }
      if (newGroup == Emoji.PickerCategory.SmileysAndPeople || oldGroup == Emoji.PickerCategory.SmileysAndPeople)
      {
        this.NotifyPropertyChanged("SmileysAndPeopleBackground");
        this.NotifyPropertyChanged("SmileysAndPeopleImage");
      }
      if (newGroup == Emoji.PickerCategory.AnimalsAndNature || oldGroup == Emoji.PickerCategory.AnimalsAndNature)
      {
        this.NotifyPropertyChanged("AnimalsAndNatureBackground");
        this.NotifyPropertyChanged("AnimalsAndNatureImage");
      }
      if (newGroup == Emoji.PickerCategory.Activity || oldGroup == Emoji.PickerCategory.Activity)
      {
        this.NotifyPropertyChanged("ActivityBackground");
        this.NotifyPropertyChanged("ActivityImage");
      }
      if (newGroup == Emoji.PickerCategory.FoodAndDrink || oldGroup == Emoji.PickerCategory.FoodAndDrink)
      {
        this.NotifyPropertyChanged("FoodAndDrinkBackground");
        this.NotifyPropertyChanged("FoodAndDrinkImage");
      }
      if (newGroup == Emoji.PickerCategory.TravelAndPlaces || oldGroup == Emoji.PickerCategory.TravelAndPlaces)
      {
        this.NotifyPropertyChanged("TravelAndPlacesBackground");
        this.NotifyPropertyChanged("TravelAndPlacesImage");
      }
      if (newGroup == Emoji.PickerCategory.Objects || oldGroup == Emoji.PickerCategory.Objects)
      {
        this.NotifyPropertyChanged("ObjectsBackground");
        this.NotifyPropertyChanged("ObjectsImage");
      }
      if (newGroup == Emoji.PickerCategory.Symbols || oldGroup == Emoji.PickerCategory.Symbols)
      {
        this.NotifyPropertyChanged("SymbolsBackground");
        this.NotifyPropertyChanged("SymbolsImage");
      }
      if (newGroup != Emoji.PickerCategory.Flags && oldGroup != Emoji.PickerCategory.Flags)
        return;
      this.NotifyPropertyChanged("FlagsBackground");
      this.NotifyPropertyChanged("FlagsImage");
    }

    private void OnOrientationChanged()
    {
      this.NotifyPropertyChanged("SideGroupHeaderItemWidth");
      this.NotifyPropertyChanged("EmojiGridMargin");
      if (this.PopupSelector == null)
        return;
      if (this.PopupSelector.IsOpen)
        this.PopupSelector.IsOpen = false;
      this.PopupSelector.Overlay.Height = this.OverlayHeight;
      this.PopupSelector.Overlay.Width = this.OverlayWidth;
    }

    public void InsertEmoji(Emoji.EmojiChar emojiChar)
    {
      if (this.InsertionTextBox == null)
        return;
      string codepointsForInputTextBox = Emoji.GetCodepointsForInputTextBox(emojiChar);
      int selectionStart = this.InsertionTextBox.SelectionStart;
      int selectionLength = this.InsertionTextBox.SelectionLength;
      string str = this.InsertionTextBox.Text ?? "";
      this.InsertionTextBox.Text = str.Substring(0, selectionStart) + codepointsForInputTextBox + str.Substring(selectionStart + selectionLength);
      this.InsertionTextBox.SelectionLength = 0;
      this.InsertionTextBox.SelectionStart = selectionStart + codepointsForInputTextBox.Length;
    }

    public static void InsertBackspace(TextBox textBox, bool win10Workaround = false, string prevText = null)
    {
      if (textBox == null)
        return;
      int selectionStart = textBox.SelectionStart;
      int num1 = textBox.SelectionLength;
      string str1 = textBox.Text ?? "";
      if (string.IsNullOrEmpty(str1))
        return;
      if (win10Workaround && !str1.Equals(prevText))
      {
        int length1 = str1.Length;
        int? length2 = prevText?.Length;
        int valueOrDefault = length2.GetValueOrDefault();
        if ((length1 < valueOrDefault ? (length2.HasValue ? 1 : 0) : 0) != 0)
        {
          num1 = prevText.Length - str1.Length;
          str1 = prevText;
        }
      }
      if (num1 == 0)
      {
        if (selectionStart == 0)
          return;
        --selectionStart;
        num1 = 1;
      }
      bool seenRegionalFlag = false;
      int num2 = Utils.ExpandEmojiUnicodeLeft(str1, selectionStart, out seenRegionalFlag);
      int num3 = Utils.ExpandEmojiUnicodeRight(str1, selectionStart + num1, !seenRegionalFlag);
      int length = selectionStart - num2;
      int startIndex = selectionStart + num1 + num3;
      string str2 = str1?.Substring(0, length) + str1?.Substring(startIndex);
      textBox.Text = str2;
      textBox.SelectionLength = 0;
      textBox.SelectionStart = length;
    }

    public static void InsertBackspace(TextBox textBox, ref string previousText)
    {
      EmojiPickerViewModel.InsertBackspace(textBox, true, previousText);
      if ((previousText ?? "").Length >= (textBox.Text ?? "").Length)
        return;
      previousText = textBox.Text;
    }

    private void InsertBackspace() => EmojiPickerViewModel.InsertBackspace(this.InsertionTextBox);

    public void ShowVariantSelector(System.Windows.Point anchorpoint, Grid g, double emojiWidth)
    {
      if (this.CurrentEmojiKeyboardItem != g && this.CurrentEmojiKeyboardItem != null)
        this.CurrentEmojiKeyboardItem.Background = EmojiPickerViewModel.InactiveBrush;
      this.CurrentEmojiKeyboardItem = g;
      anchorpoint.Y -= (double) EmojiPopup.GridHeight;
      anchorpoint.X = anchorpoint.X - (double) (EmojiPopup.GridWidth / 2 + EmojiPopup.MarginSize) + emojiWidth / 2.0;
      if (this.PopupSelector == null)
      {
        this.PopupSelector = new EmojiPopup(this);
        this.Driver.LayoutRoot.Children.Add((UIElement) this.PopupSelector);
        Grid.SetRow((FrameworkElement) this.PopupSelector, 1);
        this.PopupSelector.Overlay.Height = this.OverlayHeight;
        this.PopupSelector.Overlay.Width = this.OverlayWidth;
      }
      this.PopupTopLeft = anchorpoint;
      this.PopupSelector.SetRelativePosition(this.PopupTopLeft.X, this.PopupTopLeft.Y);
    }

    public int GetPivotPosition(int CurrentCol)
    {
      int pivotPosition = Math.Min(5, (int) Math.Floor(this.PopupTopLeft.X / (double) (EmojiPopup.GridWidth + EmojiPopup.MarginSize)));
      if (pivotPosition < 0)
        pivotPosition = 0;
      this.PopupTopLeft.X -= (double) (pivotPosition * (EmojiPopup.GridWidth + EmojiPopup.MarginSize));
      this.PopupSelector.SetRelativePosition(this.PopupTopLeft.X, this.PopupTopLeft.Y);
      Log.d("EmojiPopup", "Pivot relative positions PopupTopLeft.X={0}, PopupTopLeft.Y={1}", (object) this.PopupTopLeft.X, (object) this.PopupTopLeft.Y);
      return pivotPosition;
    }

    public void OnEmojiSelectorTapped(Emoji.EmojiChar emojiChar, int SelectedIndex)
    {
      if (Emoji.IsVariantSelector(emojiChar.codepoints))
      {
        string baseEmoji = Emoji.GetBaseEmoji(emojiChar.codepoints);
        string singleEmojiVariant = Emoji.GetSingleEmojiVariant(baseEmoji, SelectedIndex);
        int num;
        if (!EmojiPickerViewModel.PendingEmojiSelectedIndexes.TryGetValue(baseEmoji, out num) || num != SelectedIndex)
          EmojiPickerViewModel.PendingEmojiSelectedIndexes[baseEmoji] = SelectedIndex;
        this.Model.onEmojiSelectedAction_(Emoji.GetEmojiChar(singleEmojiVariant));
      }
      else
        this.Model.onEmojiSelectedAction_(emojiChar);
    }

    public void ClosePopup()
    {
      if (this.PopupSelector == null)
        return;
      this.PopupSelector.IsOpen = false;
      EmojiRow.Emoji_ManipulationCleanup();
    }

    private void Backspace_Tap(object sender, RoutedEventArgs e) => this.InsertBackspace();

    private void BackspaceKey_Hold(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.BackspaceHolder == null)
      {
        this.BackspaceHolder = new DispatcherTimer();
        this.BackspaceHolder.Tick += (EventHandler) ((s, o) =>
        {
          if (this.InsertionTextBox.Text == "")
            this.BackspaceHolder.Stop();
          this.InsertBackspace();
        });
        this.BackspaceHolder.Interval = TimeSpan.FromMilliseconds(100.0);
      }
      this.BackspaceHolder.Start();
    }

    private void BackspaceKey_ManipulationStarted(object sender, EventArgs e)
    {
      ((Control) sender).Background = EmojiPickerViewModel.ActiveBrush;
      ((Control) sender).Foreground = EmojiPickerViewModel.InactiveBrush;
    }

    private void BackspaceKey_ManipulationCompleted(object sender, EventArgs e)
    {
      if (this.BackspaceHolder != null)
        this.BackspaceHolder.Stop();
      ((Control) sender).Background = this.Driver.Resources[(object) "PhoneInactiveBrush"] as Brush;
      ((Control) sender).Foreground = this.Driver.Resources[(object) "PhoneForegroundBrush"] as Brush;
    }

    private void EmojiGridContainer_LoadingPivotItem(object sender, PivotItemEventArgs e)
    {
      Pivot pivot = sender as Pivot;
      int oldPivotIndex = this.OldPivotIndex_;
      int selectedIndex = pivot.SelectedIndex;
      if (oldPivotIndex == selectedIndex)
        return;
      PivotItem pivotItem = pivot.Items[oldPivotIndex] as PivotItem;
      this.CurrentEmojiGroupIndex = oldPivotIndex == selectedIndex - 1 || oldPivotIndex == 2 && selectedIndex == 0 ? (Emoji.PickerCategory) ((int) (this.CurrentEmojiGroupIndex + 10) % 9) : (Emoji.PickerCategory) ((int) (this.CurrentEmojiGroupIndex + 8) % 9);
      if (pivotItem.Content is ListBox content)
        content.ItemsSource = (IEnumerable) null;
      pivotItem.Content = (object) null;
    }

    public EmojiPickerViewModel(EmojiPicker driver, Button backspacebutton)
    {
      this.Driver = driver;
      this.InsertionTextBox = driver.InsertionTextBox;
      if (backspacebutton != null)
      {
        backspacebutton.Click += new RoutedEventHandler(this.Backspace_Tap);
        backspacebutton.Hold += new EventHandler<System.Windows.Input.GestureEventArgs>(this.BackspaceKey_Hold);
        backspacebutton.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(this.BackspaceKey_ManipulationStarted);
        backspacebutton.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(this.BackspaceKey_ManipulationCompleted);
      }
      Pivot pivot = new Pivot();
      pivot.HeaderTemplate = (DataTemplate) null;
      pivot.Padding = new Thickness();
      this.EmojiGridContainer = pivot;
      this.EmojiGridContainer.LoadingPivotItem += new EventHandler<PivotItemEventArgs>(this.EmojiGridContainer_LoadingPivotItem);
      for (int index = 0; index < 3; ++index)
        this.EmojiGridContainer.Items.Add((object) new PivotItem());
      this.EmojiGridContainer.Margin = new Thickness(0.0, 4.0, 0.0, 4.0);
      this.EmojiGridContainer.Padding = new Thickness(0.0, -10.0, 0.0, 0.0);
      if (ImageStore.IsDarkTheme())
        this.EmojiGridContainer.Background = this.Driver.Resources[(object) "PhoneInactiveBrush"] as Brush;
      else
        this.EmojiGridContainer.Background = this.Driver.Resources[(object) "PhoneBackgroundBrush"] as Brush;
      this.Driver.LayoutRoot.Children.Insert(0, (UIElement) this.EmojiGridContainer);
      Grid.SetRow((FrameworkElement) this.EmojiGridContainer, 2);
      UIElementCollection children = this.Driver.LayoutRoot.Children;
    }

    public void InitCurrentGroupIndex()
    {
      Emoji.PickerCategory pickerCategory = (Emoji.PickerCategory) Settings.LastEmojiTab;
      switch (pickerCategory)
      {
        case Emoji.PickerCategory.Recent:
          MessagesContext.Run((MessagesContext.MessagesCallback) (db => this.Model.ReloadRecent(db)));
          goto case Emoji.PickerCategory.SmileysAndPeople;
        case Emoji.PickerCategory.SmileysAndPeople:
        case Emoji.PickerCategory.AnimalsAndNature:
        case Emoji.PickerCategory.FoodAndDrink:
        case Emoji.PickerCategory.Activity:
        case Emoji.PickerCategory.TravelAndPlaces:
        case Emoji.PickerCategory.Objects:
        case Emoji.PickerCategory.Symbols:
        case Emoji.PickerCategory.Flags:
          this.CurrentEmojiGroupIndex = pickerCategory;
          break;
        default:
          pickerCategory = Emoji.PickerCategory.SmileysAndPeople;
          goto case Emoji.PickerCategory.SmileysAndPeople;
      }
    }
  }
}
