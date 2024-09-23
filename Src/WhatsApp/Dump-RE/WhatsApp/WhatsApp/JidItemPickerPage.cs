// Decompiled with JetBrains decompiler
// Type: WhatsApp.JidItemPickerPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using WhatsApp.WaViewModels;

#nullable disable
namespace WhatsApp
{
  public class JidItemPickerPage : PhoneApplicationPage
  {
    private static string nextInstanceTitle = (string) null;
    private static string nextInstanceSubtitle = (string) null;
    private static ListTabData[] nextInstanceTabs = (ListTabData[]) null;
    private static IObserver<List<string>> nextInstanceObserver = (IObserver<List<string>>) null;
    private static Func<string, IObservable<bool>> nextInstanceConfirmSelectionFunc = (Func<string, IObservable<bool>>) null;
    private static bool nextInstanceShouldCloseOnSelection = false;
    private static bool nextInstanceisMultiSelect = false;
    private static bool nextInstanceIsSearchable = true;
    private ListTab[] tabs;
    private IObserver<List<string>> jidObserver;
    private Func<string, IObservable<bool>> confirmSelectionFunc;
    private bool shouldCloseOnSelection;
    private bool isMultiSelect;
    private bool isSearchable = true;
    private ApplicationBarIconButton sendButton;
    private HashSet<string> currentMultiselectedItems;
    private bool initLoaded;
    private List<IDisposable> disposables = new List<IDisposable>();
    private Storyboard slideDownSb;
    private DateTime? lastTextChangedAt;
    private IDisposable delaySub;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal PageTitlePanel TitlePanel;
    internal EmojiTextBox SearchBox;
    internal Grid PivotHeaderPanel;
    internal Pivot Pivot;
    private bool _contentLoaded;

    public JidItemPickerPage()
    {
      this.InitializeComponent();
      this.LayoutRootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.isMultiSelect = JidItemPickerPage.nextInstanceisMultiSelect;
      JidItemPickerPage.nextInstanceisMultiSelect = false;
      this.tabs = ((IEnumerable<ListTabData>) (JidItemPickerPage.nextInstanceTabs ?? new ListTabData[0])).Where<ListTabData>((Func<ListTabData, bool>) (td => td != null)).Select<ListTabData, ListTab>((Func<ListTabData, ListTab>) (td => new ListTab(td, this.isMultiSelect))).ToArray<ListTab>();
      JidItemPickerPage.nextInstanceTabs = (ListTabData[]) null;
      this.jidObserver = JidItemPickerPage.nextInstanceObserver;
      JidItemPickerPage.nextInstanceObserver = (IObserver<List<string>>) null;
      string nextInstanceTitle = JidItemPickerPage.nextInstanceTitle;
      JidItemPickerPage.nextInstanceTitle = (string) null;
      this.confirmSelectionFunc = JidItemPickerPage.nextInstanceConfirmSelectionFunc;
      JidItemPickerPage.nextInstanceConfirmSelectionFunc = (Func<string, IObservable<bool>>) null;
      this.shouldCloseOnSelection = JidItemPickerPage.nextInstanceShouldCloseOnSelection;
      JidItemPickerPage.nextInstanceShouldCloseOnSelection = false;
      this.isSearchable = JidItemPickerPage.nextInstanceIsSearchable;
      JidItemPickerPage.nextInstanceIsSearchable = true;
      string instanceSubtitle = JidItemPickerPage.nextInstanceSubtitle;
      JidItemPickerPage.nextInstanceSubtitle = (string) null;
      if (!string.IsNullOrEmpty(nextInstanceTitle))
      {
        this.TitlePanel.SmallTitle = nextInstanceTitle;
        this.TitlePanel.Visibility = Visibility.Visible;
      }
      if (!string.IsNullOrEmpty(instanceSubtitle))
        this.TitlePanel.Subtitle = instanceSubtitle;
      if (this.tabs != null)
      {
        ListTab listTab = ((IEnumerable<ListTab>) this.tabs).FirstOrDefault<ListTab>();
        if (listTab != null)
        {
          listTab.AddToPivot(this.Pivot);
          listTab.TryLoadItems((HashSet<string>) null);
        }
        this.PivotHeaderPanel.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = new GridLength(1.0, GridUnitType.Star)
        });
        int num = 1;
        foreach (ListTab tab in this.tabs)
        {
          TextBlock textBlock = new TextBlock();
          textBlock.Text = tab.Header;
          textBlock.FontWeight = FontWeights.Medium;
          textBlock.HorizontalAlignment = HorizontalAlignment.Center;
          textBlock.Margin = new Thickness(0.0, 8.0, 0.0, 8.0);
          textBlock.FontSize = 23.0;
          textBlock.TextWrapping = TextWrapping.NoWrap;
          textBlock.Tag = (object) tab;
          TextBlock element = textBlock;
          element.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(this.TabHeader_Tap);
          this.PivotHeaderPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = GridLength.Auto
          });
          this.PivotHeaderPanel.ColumnDefinitions.Add(new ColumnDefinition()
          {
            Width = new GridLength(1.0, GridUnitType.Star)
          });
          Grid.SetColumn((FrameworkElement) element, num);
          this.PivotHeaderPanel.Children.Add((UIElement) element);
          if (this.isMultiSelect)
            this.disposables.Add(tab.SelectedItemsObservable().ObserveOnDispatcher<SelectionChangedEventArgs>().Subscribe<SelectionChangedEventArgs>(new Action<SelectionChangedEventArgs>(this.OnJidItemsSelected)));
          else
            this.disposables.Add(tab.SelectedItemObservable().ObserveOnDispatcher<JidItemViewModel>().Subscribe<JidItemViewModel>(new Action<JidItemViewModel>(this.OnJidItemSelected)));
          num += 2;
        }
      }
      if (this.isSearchable)
      {
        this.disposables.Add(this.SearchBox.GetTextChangedAsync().ObserveOnDispatcher<TextChangedEventArgs>().Subscribe<TextChangedEventArgs>(new Action<TextChangedEventArgs>(this.SearchBox_TextChanged)));
        this.SearchBox.Visibility = Visibility.Visible;
      }
      else
        this.SearchBox.Visibility = Visibility.Collapsed;
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
      if (!this.isMultiSelect)
        return;
      Microsoft.Phone.Shell.ApplicationBar bar = new Microsoft.Phone.Shell.ApplicationBar();
      bar.Buttons.Clear();
      this.sendButton = new ApplicationBarIconButton()
      {
        IconUri = new Uri("/Images/sendmsg.png", UriKind.Relative),
        Text = "Send",
        IsEnabled = false
      };
      this.sendButton.Click += new EventHandler(this.Send_Click);
      bar.Buttons.Add((object) this.sendButton);
      Localizable.LocalizeAppBar(bar);
      this.ApplicationBar = (IApplicationBar) bar;
    }

    private void TabHeader_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(((FrameworkElement) sender).Tag is ListTab tag))
        return;
      this.Pivot.SelectedItem = (object) tag.PivotItem;
    }

    private void Send_Click(object sender, EventArgs e)
    {
      List<string> stringList = new List<string>();
      foreach (string multiselectedItem in this.currentMultiselectedItems)
        stringList.Add(multiselectedItem);
      this.jidObserver.OnNext(stringList);
      this.jidObserver.OnCompleted();
      this.SlideDownAndBackOut();
    }

    public static IObservable<List<string>> Start(
      ListTabData[] tabs,
      string title,
      Func<string, IObservable<bool>> confirmSelectionFunc = null,
      bool shouldCloseOnSelection = false,
      bool clearStack = false,
      bool isMultiSelect = false,
      string subtitle = null,
      bool isSearchable = true)
    {
      return Observable.Create<List<string>>((Func<IObserver<List<string>>, Action>) (observer =>
      {
        JidItemPickerPage.nextInstanceObserver = observer;
        JidItemPickerPage.nextInstanceTitle = title;
        JidItemPickerPage.nextInstanceTabs = tabs;
        JidItemPickerPage.nextInstanceConfirmSelectionFunc = confirmSelectionFunc;
        JidItemPickerPage.nextInstanceShouldCloseOnSelection = shouldCloseOnSelection;
        JidItemPickerPage.nextInstanceisMultiSelect = Settings.IsWaAdmin && isMultiSelect;
        JidItemPickerPage.nextInstanceSubtitle = subtitle;
        JidItemPickerPage.nextInstanceIsSearchable = isSearchable;
        WaUriParams uriParams = new WaUriParams();
        if (clearStack)
          uriParams.AddBool("ClearStack", clearStack);
        NavUtils.NavigateToPage(nameof (JidItemPickerPage), uriParams);
        return (Action) (() => { });
      }));
    }

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      if (this.initLoaded || this.tabs == null || this.tabs.Length <= 1)
        return;
      this.initLoaded = true;
      foreach (ListTab listTab in ((IEnumerable<ListTab>) this.tabs).Skip<ListTab>(1).ToArray<ListTab>())
        listTab.AddToPivot(this.Pivot);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      base.OnRemovedFromJournal(e);
      foreach (IDisposable disposable in this.disposables)
        disposable.SafeDispose();
      this.disposables.Clear();
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      foreach (IDisposable tab in this.tabs)
        tab.SafeDispose();
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.SearchBox.Text))
      {
        this.SearchBox.Text = string.Empty;
        e.Cancel = true;
      }
      else
      {
        if (this.jidObserver != null)
          this.jidObserver.OnCompleted();
        e.Cancel = true;
        base.OnBackKeyPress(e);
        this.SlideDownAndBackOut();
      }
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!(this.Pivot.SelectedItem is PivotItem selectedItem) || !(selectedItem.Tag is ListTab tag))
        return;
      tag.TryLoadItems(this.currentMultiselectedItems);
      tag.TrySearch(this.SearchBox.Text);
      for (int index = 0; index < this.tabs.Length; ++index)
      {
        if (this.tabs[index] == tag)
        {
          this.SetSelectedHeader(index);
          break;
        }
      }
    }

    private void SetSelectedHeader(int index)
    {
      for (int index1 = 0; index1 < this.PivotHeaderPanel.Children.Count; ++index1)
      {
        if (this.PivotHeaderPanel.Children[index1] is TextBlock child)
          child.Foreground = (Brush) UIUtils.ForegroundBrush;
      }
      (this.PivotHeaderPanel.Children[index] as TextBlock).Foreground = (Brush) UIUtils.AccentBrush;
    }

    private void SearchBox_TextChanged(TextChangedEventArgs e)
    {
      if (!(this.Pivot.SelectedItem is PivotItem selectedItem1) || !(selectedItem1.Tag is ListTab tag1))
        return;
      string rawTerm = this.SearchBox.Text ?? "";
      DateTime utcNow = DateTime.UtcNow;
      int num = rawTerm.Length < 3 || !this.lastTextChangedAt.HasValue ? 1 : (utcNow - this.lastTextChangedAt.Value < TimeSpan.FromMilliseconds(500.0) ? 1 : 0);
      this.lastTextChangedAt = new DateTime?(utcNow);
      this.delaySub.SafeDispose();
      this.delaySub = (IDisposable) null;
      if (num != 0)
        this.delaySub = Observable.Timer(TimeSpan.FromMilliseconds(500.0)).ObserveOnDispatcher<long>().Subscribe<long>((Action<long>) (_ =>
        {
          this.delaySub.SafeDispose();
          this.delaySub = (IDisposable) null;
          if (!(this.Pivot.SelectedItem is PivotItem selectedItem3) || !(selectedItem3.Tag is ListTab tag3))
            return;
          tag3.TrySearch(this.SearchBox.Text);
        }));
      else
        tag1.TrySearch(rawTerm);
    }

    private void OnJidItemSelected(JidItemViewModel selItem)
    {
      IObservable<bool> source = this.confirmSelectionFunc == null ? Observable.Return<bool>(true) : this.confirmSelectionFunc(selItem.Jid);
      IDisposable sub = (IDisposable) null;
      sub = source.Take<bool>(1).ObserveOnDispatcher<bool>().Subscribe<bool>((Action<bool>) (confirmed =>
      {
        if (confirmed && this.jidObserver != null)
          this.Dispatcher.BeginInvoke((Action) (() =>
          {
            this.jidObserver.OnNext(new List<string>()
            {
              selItem.Jid
            });
            this.jidObserver.OnCompleted();
            if (!this.shouldCloseOnSelection)
              return;
            this.SlideDownAndBackOut();
          }));
        sub.SafeDispose();
        sub = (IDisposable) null;
      }));
    }

    private void OnJidItemsSelected(SelectionChangedEventArgs selItems)
    {
      IList addedItems = selItems.AddedItems;
      IList removedItems = selItems.RemovedItems;
      if (this.currentMultiselectedItems == null)
        this.currentMultiselectedItems = new HashSet<string>();
      foreach (JidItemViewModel jidItemViewModel in (IEnumerable) addedItems)
        this.currentMultiselectedItems.Add(jidItemViewModel.Jid);
      foreach (JidItemViewModel jidItemViewModel in (IEnumerable) removedItems)
      {
        if (this.currentMultiselectedItems.Contains(jidItemViewModel.Jid))
          this.currentMultiselectedItems.Remove(jidItemViewModel.Jid);
      }
      if (this.currentMultiselectedItems.Count > 0)
        this.sendButton.IsEnabled = true;
      else
        this.sendButton.IsEnabled = false;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/JidItemPickerPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.TitlePanel = (PageTitlePanel) this.FindName("TitlePanel");
      this.SearchBox = (EmojiTextBox) this.FindName("SearchBox");
      this.PivotHeaderPanel = (Grid) this.FindName("PivotHeaderPanel");
      this.Pivot = (Pivot) this.FindName("Pivot");
    }
  }
}
