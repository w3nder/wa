// Decompiled with JetBrains decompiler
// Type: WhatsApp.ListTab
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class ListTab : WaDisposable
  {
    private ProgressBar progressIndicator;
    private TextBlock footerTextBlock;
    private ListTabData tabData;
    private bool isMultiSelect;
    private ObservableCollection<JidItemViewModel> resultsSrc;
    private ObservableCollection<JidItemViewModel> allItemsSrc;
    private Subject<JidItemViewModel> selectedItemSubject = new Subject<JidItemViewModel>();
    private Subject<SelectionChangedEventArgs> selectedItemsSubject = new Subject<SelectionChangedEventArgs>();
    private HashSet<string> selectedItems = new HashSet<string>();
    private Dictionary<JidItemViewModel, WeakReference<Panel>> realizedItems = new Dictionary<JidItemViewModel, WeakReference<Panel>>();
    private IDisposable loadSub;
    private IDisposable searchSub;
    private string searchTermInUse = "";

    public PivotItem PivotItem { get; private set; }

    public WhatsApp.CompatibilityShims.LongListSelector ListControl { get; private set; }

    public string Header => this.tabData.Header;

    public bool AllLoaded { get; private set; }

    public ApplicationBar ApplicationBar { get; private set; }

    public ListTab(ListTabData data, bool isMultiSelect)
    {
      this.tabData = data;
      this.AllLoaded = false;
      this.isMultiSelect = isMultiSelect;
    }

    protected override void DisposeManagedResources()
    {
      base.DisposeManagedResources();
      this.loadSub.SafeDispose();
      this.loadSub = (IDisposable) null;
      this.searchSub.SafeDispose();
      this.searchSub = (IDisposable) null;
    }

    public IObservable<JidItemViewModel> SelectedItemObservable()
    {
      return (IObservable<JidItemViewModel>) this.selectedItemSubject;
    }

    public IObservable<SelectionChangedEventArgs> SelectedItemsObservable()
    {
      return (IObservable<SelectionChangedEventArgs>) this.selectedItemsSubject;
    }

    public void AddToPivot(Pivot pivot)
    {
      if (this.tabData == null)
        return;
      if (this.ListControl == null)
      {
        StackPanel stackPanel1 = new StackPanel();
        stackPanel1.Margin = new Thickness(24.0, 0.0, 24.0, 0.0);
        stackPanel1.Height = 12.0;
        StackPanel stackPanel2 = stackPanel1;
        UIElementCollection children1 = stackPanel2.Children;
        ProgressBar progressBar1 = new ProgressBar();
        progressBar1.IsHitTestVisible = false;
        progressBar1.IsIndeterminate = true;
        progressBar1.VerticalAlignment = VerticalAlignment.Bottom;
        ProgressBar progressBar2 = progressBar1;
        this.progressIndicator = progressBar1;
        ProgressBar progressBar3 = progressBar2;
        children1.Add((UIElement) progressBar3);
        StackPanel stackPanel3 = new StackPanel();
        stackPanel3.Margin = new Thickness(0.0, 24.0, 24.0, 112.0);
        StackPanel stackPanel4 = stackPanel3;
        UIElementCollection children2 = stackPanel4.Children;
        TextBlock textBlock1 = new TextBlock();
        textBlock1.IsHitTestVisible = false;
        textBlock1.Margin = new Thickness(0.0);
        textBlock1.TextWrapping = TextWrapping.Wrap;
        textBlock1.Opacity = 0.65;
        textBlock1.FontSize = UIUtils.FontSizeLarge;
        textBlock1.Visibility = Visibility.Collapsed;
        TextBlock textBlock2 = textBlock1;
        this.footerTextBlock = textBlock1;
        TextBlock textBlock3 = textBlock2;
        children2.Add((UIElement) textBlock3);
        WhatsApp.CompatibilityShims.LongListSelector longListSelector = new WhatsApp.CompatibilityShims.LongListSelector();
        longListSelector.Margin = new Thickness(0.0, this.tabData.Header == null ? 12.0 : 0.0, 0.0, 0.0);
        longListSelector.IsFlatList = true;
        longListSelector.ItemTemplate = this.DataTemplate;
        longListSelector.ItemsSource = (IList) (this.resultsSrc ?? this.allItemsSrc);
        longListSelector.ListHeader = (object) stackPanel2;
        longListSelector.ListFooter = (object) stackPanel4;
        this.ListControl = longListSelector;
        this.ListControl.ItemRealized += new EventHandler<ItemRealizationEventArgs>(this.ListControl_ItemRealized);
        this.ListControl.ItemUnrealized += new EventHandler<ItemRealizationEventArgs>(this.ListControl_ItemUnrealized);
      }
      if (this.PivotItem == null)
      {
        PivotItem pivotItem = new PivotItem();
        pivotItem.Header = (object) this.tabData.Header;
        pivotItem.Margin = new Thickness(24.0, 0.0, 0.0, 0.0);
        pivotItem.Tag = (object) this;
        pivotItem.Content = (object) this.ListControl;
        this.PivotItem = pivotItem;
        if (this.tabData.Header == null)
          pivot.HeaderTemplate = (DataTemplate) null;
      }
      if (this.PivotItem.Parent != null)
        return;
      pivot.Items.Add((object) this.PivotItem);
    }

    public void TryLoadItems(HashSet<string> globalSelectedItems)
    {
      if (this.tabData == null || this.tabData.ItemsObservable == null || this.loadSub != null)
        return;
      this.selectedItems.Clear();
      if (this.isMultiSelect && globalSelectedItems != null)
      {
        foreach (string globalSelectedItem in globalSelectedItems)
          this.selectedItems.Add(globalSelectedItem);
      }
      if (this.AllLoaded)
      {
        foreach (KeyValuePair<JidItemViewModel, WeakReference<Panel>> realizedItem in this.realizedItems)
        {
          Panel target = (Panel) null;
          if (realizedItem.Value.TryGetTarget(out target))
            this.RealizeItem(realizedItem.Key, target);
        }
      }
      else
      {
        this.progressIndicator.Visibility = Visibility.Visible;
        this.loadSub = this.tabData.ItemsObservable.SubscribeOn<JidItemViewModel[]>((IScheduler) AppState.Worker).ObserveOnDispatcher<JidItemViewModel[]>().Subscribe<JidItemViewModel[]>((Action<JidItemViewModel[]>) (items =>
        {
          if (this.allItemsSrc == null)
          {
            this.allItemsSrc = new ObservableCollection<JidItemViewModel>((IEnumerable<JidItemViewModel>) items);
            if (this.isMultiSelect)
            {
              if (this.searchSub != null || this.ListControl == null)
                return;
              this.ListControl.ItemsSource = (IList) this.allItemsSrc;
            }
            else
            {
              if (this.searchSub != null || this.ListControl == null)
                return;
              this.ListControl.ItemsSource = (IList) this.allItemsSrc;
            }
          }
          else
          {
            foreach (JidItemViewModel jidItemViewModel in items)
              this.allItemsSrc.Add(jidItemViewModel);
          }
        }), (Action<Exception>) (ex =>
        {
          this.AllLoaded = false;
          this.loadSub.SafeDispose();
          this.loadSub = (IDisposable) null;
        }), (Action) (() =>
        {
          this.AllLoaded = true;
          this.progressIndicator.Visibility = Visibility.Collapsed;
          this.loadSub.SafeDispose();
          this.loadSub = (IDisposable) null;
          this.tabData.OnAllLoaded((IEnumerable<JidItemViewModel>) this.allItemsSrc);
        }));
      }
    }

    public void TrySearch(string rawTerm)
    {
      if (this.tabData == null)
        return;
      string str = rawTerm == null ? "" : rawTerm.Trim();
      if (str == this.searchTermInUse)
        return;
      Action cleanup = (Action) (() =>
      {
        this.searchSub.SafeDispose();
        this.searchSub = (IDisposable) null;
        this.progressIndicator.Visibility = Visibility.Collapsed;
        this.footerTextBlock.Visibility = Visibility.Collapsed;
      });
      cleanup();
      this.searchTermInUse = str;
      if (string.IsNullOrWhiteSpace(this.searchTermInUse))
      {
        this.ListControl.ItemsSource = (IList) this.allItemsSrc;
        if (this.resultsSrc == null)
          return;
        this.resultsSrc.Clear();
        this.resultsSrc = (ObservableCollection<JidItemViewModel>) null;
      }
      else
      {
        this.progressIndicator.Visibility = Visibility.Visible;
        this.searchSub = this.tabData.GetSearchObservable(this.searchTermInUse).SubscribeOn<ListTabSearchResult>((IScheduler) AppState.Worker).ObserveOnDispatcher<ListTabSearchResult>().Subscribe<ListTabSearchResult>((Action<ListTabSearchResult>) (searchRes =>
        {
          if (searchRes == null || searchRes.SearchTerm != this.searchTermInUse)
            return;
          if (this.resultsSrc == null)
            this.resultsSrc = new ObservableCollection<JidItemViewModel>(searchRes.Items);
          else if (searchRes.Items.Any<JidItemViewModel>())
            Utils.UpdateInPlace<JidItemViewModel>((IList<JidItemViewModel>) this.resultsSrc, (IList<JidItemViewModel>) searchRes.Items, (Func<JidItemViewModel, string>) (vm => vm.Jid), (Action<JidItemViewModel>) null);
          else
            this.resultsSrc.Clear();
          if (this.ListControl.ItemsSource != this.resultsSrc)
            this.ListControl.ItemsSource = (IList) this.resultsSrc;
          cleanup();
          if (this.resultsSrc.Any<JidItemViewModel>())
            return;
          this.footerTextBlock.Text = AppResources.NoResults;
          this.footerTextBlock.Visibility = Visibility.Visible;
        }), (Action<Exception>) (ex => cleanup()), cleanup);
      }
    }

    private DataTemplate DataTemplate
    {
      get
      {
        return XamlReader.Load("\r\n                        <DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\r\n                                      xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\r\n                                      xmlns:local=\"clr-namespace:WhatsApp;assembly=WhatsApp\">\r\n                            <Grid Background=\"Transparent\">\r\n                                <Grid.ColumnDefinitions>\r\n                                    <ColumnDefinition Width=\"Auto\"/>\r\n                                    <ColumnDefinition Width=\"*\"/>\r\n                                </Grid.ColumnDefinitions>\r\n                            </Grid>\r\n                        </DataTemplate>\r\n                        ") as DataTemplate;
      }
    }

    public void Item_Tap(object sender, EventArgs e)
    {
      Grid grid = sender as Grid;
      if (!(grid.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is ItemControlBase)).FirstOrDefault<UIElement>() is ItemControlBase itemControlBase))
        return;
      JidItemViewModel viewModel = itemControlBase.ViewModel;
      if (viewModel == null)
        return;
      if (this.isMultiSelect)
      {
        CheckBox checkBox = grid.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is CheckBox)).FirstOrDefault<UIElement>() as CheckBox;
        List<JidItemViewModel> removedItems = new List<JidItemViewModel>();
        List<JidItemViewModel> addedItems = new List<JidItemViewModel>();
        bool? isChecked = checkBox.IsChecked;
        checkBox.IsChecked = isChecked.HasValue ? new bool?(!isChecked.GetValueOrDefault()) : new bool?();
        isChecked = checkBox.IsChecked;
        if (isChecked.GetValueOrDefault())
        {
          addedItems.Add(viewModel);
          this.selectedItems.Add(viewModel.Jid);
        }
        else
        {
          removedItems.Add(viewModel);
          if (this.selectedItems.Contains(viewModel.Jid))
            this.selectedItems.Remove(viewModel.Jid);
        }
        this.selectedItemsSubject.OnNext(new SelectionChangedEventArgs((IList) removedItems, (IList) addedItems));
      }
      else
        this.selectedItemSubject.OnNext(viewModel);
    }

    private void ListControl_ItemRealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind != LongListSelectorItemKind.Item)
        return;
      JidItemViewModel content = e.Container.Content as JidItemViewModel;
      Grid child = VisualTreeHelper.GetChild((DependencyObject) e.Container, 0) as Grid;
      if (!(child.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is ItemControlBase)).FirstOrDefault<UIElement>() is ItemControlBase))
      {
        ItemControlBase element = this.tabData.CreateItem() as ItemControlBase;
        element.Margin = new Thickness(0.0, 8.0, 0.0, 8.0);
        Grid.SetColumn((FrameworkElement) element, 1);
        child.Children.Add((UIElement) element);
        if (this.isMultiSelect)
        {
          CheckBox checkBox = new CheckBox();
          checkBox.IsHitTestVisible = false;
          child.Children.Add((UIElement) checkBox);
        }
        child.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Item_Tap);
        child.DoubleTap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.Item_Tap);
      }
      this.realizedItems[content] = new WeakReference<Panel>((Panel) child);
      this.RealizeItem(content, (Panel) child);
    }

    private void RealizeItem(JidItemViewModel vm, Panel container)
    {
      ItemControlBase itemControlBase = container.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is ItemControlBase)).FirstOrDefault<UIElement>() as ItemControlBase;
      if (itemControlBase.ViewModel != vm)
        itemControlBase.ViewModel = vm;
      if (!this.isMultiSelect)
        return;
      (container.Children.Where<UIElement>((Func<UIElement, bool>) (ue => ue is CheckBox)).FirstOrDefault<UIElement>() as CheckBox).IsChecked = new bool?(this.selectedItems.Contains(vm.Jid));
    }

    private void ListControl_ItemUnrealized(object sender, ItemRealizationEventArgs e)
    {
      if (e.ItemKind != LongListSelectorItemKind.Item)
        return;
      JidItemViewModel content = e.Container.Content as JidItemViewModel;
      if (!this.realizedItems.ContainsKey(content))
        return;
      this.realizedItems.Remove(content);
    }
  }
}
