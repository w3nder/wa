// Decompiled with JetBrains decompiler
// Type: WhatsApp.ListPickerPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
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


namespace WhatsApp
{
  public class ListPickerPage : PhoneApplicationPage
  {
    private static IObserver<int> nextInstanceObserver = (IObserver<int>) null;
    private static string[] nextInstanceOptions = (string[]) null;
    private static int nextInstanceSelectedIndex = -1;
    private static string nextInstanceTitle = (string) null;
    private static string nextInstanceSubtitle = (string) null;
    private static bool nextInstanceBackOutOnExit = true;
    private IObserver<int> observer;
    private ListPickerPage.Item[] options;
    private ListPickerPageViewModel viewModel;
    private Storyboard slideDownSb;
    private bool backOutOnExit = true;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    internal WhatsApp.CompatibilityShims.LongListSelector OptionList;
    private bool _contentLoaded;

    public ListPickerPage()
    {
      this.InitializeComponent();
      this.observer = ListPickerPage.nextInstanceObserver;
      ListPickerPage.nextInstanceObserver = (IObserver<int>) null;
      if (ListPickerPage.nextInstanceOptions != null && ((IEnumerable<string>) ListPickerPage.nextInstanceOptions).Any<string>())
      {
        int i = 0;
        this.options = ((IEnumerable<string>) ListPickerPage.nextInstanceOptions).Select<string, ListPickerPage.Item>((Func<string, ListPickerPage.Item>) (s => new ListPickerPage.Item(i++, s))).ToArray<ListPickerPage.Item>();
        ListPickerPage.Item obj = ((IEnumerable<ListPickerPage.Item>) this.options).ElementAtOrDefault<ListPickerPage.Item>(ListPickerPage.nextInstanceSelectedIndex);
        if (obj != null)
          obj.IsSelected = true;
      }
      ListPickerPage.nextInstanceOptions = (string[]) null;
      ListPickerPage.nextInstanceSelectedIndex = -1;
      this.backOutOnExit = ListPickerPage.nextInstanceBackOutOnExit;
      ListPickerPage.nextInstanceBackOutOnExit = true;
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      string nextInstanceTitle = ListPickerPage.nextInstanceTitle;
      ListPickerPage.nextInstanceTitle = (string) null;
      string instanceSubtitle = ListPickerPage.nextInstanceSubtitle;
      ListPickerPage.nextInstanceSubtitle = (string) null;
      this.DataContext = (object) (this.viewModel = new ListPickerPageViewModel(this.Orientation, nextInstanceTitle, instanceSubtitle));
      this.OptionList.ItemsSource = (IList) this.options;
    }

    public static IObservable<int> Start(
      string[] options,
      int selectedIndex = -1,
      string title = null,
      bool backOutOnExit = true,
      string subtitle = null)
    {
      return Observable.Create<int>((Func<IObserver<int>, Action>) (observer =>
      {
        ListPickerPage.nextInstanceObserver = observer;
        ListPickerPage.nextInstanceOptions = options;
        ListPickerPage.nextInstanceSelectedIndex = selectedIndex;
        ListPickerPage.nextInstanceTitle = title;
        ListPickerPage.nextInstanceBackOutOnExit = backOutOnExit;
        ListPickerPage.nextInstanceSubtitle = subtitle;
        WaUriParams uriParams = new WaUriParams();
        uriParams.AddString("Timestamp", DateTimeUtils.GetShortTimestampId(FunRunner.CurrentServerTimeUtc));
        NavUtils.NavigateToPage(nameof (ListPickerPage), uriParams);
        return (Action) (() => { });
      }));
    }

    private void SlideDownAndExit()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.ContentPanel, false, (Action) (() =>
      {
        if (!this.backOutOnExit)
          return;
        NavUtils.GoBack(this.NavigationService);
      }));
      if (this.viewModel == null)
        return;
      this.viewModel.BackgroundBrush = (Brush) UIUtils.TransparentBrush;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (e.NavigationMode == NavigationMode.Reset)
        return;
      if (this.observer == null || this.options == null || !((IEnumerable<ListPickerPage.Item>) this.options).Any<ListPickerPage.Item>())
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack(this.NavigationService)));
      else
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideUpFadeIn), (DependencyObject) this.ContentPanel);
    }

    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
    {
      if (this.viewModel != null)
        this.viewModel.Orientation = this.Orientation;
      base.OnOrientationChanged(e);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.observer != null)
        this.observer.OnNext(-1);
      e.Cancel = true;
      base.OnBackKeyPress(e);
      this.SlideDownAndExit();
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      if (this.slideDownSb != null)
        this.slideDownSb.Stop();
      if (this.observer != null)
        this.observer.OnCompleted();
      base.OnRemovedFromJournal(e);
    }

    private void OptionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ListPickerPage.Item selItem = this.OptionList.SelectedItem as ListPickerPage.Item;
      this.OptionList.SelectedItem = (object) null;
      if (selItem == null)
        return;
      foreach (ListPickerPage.Item option in this.options)
        option.IsSelected = option.Index == selItem.Index;
      this.Dispatcher.BeginInvoke((Action) (() =>
      {
        if (this.observer != null)
          this.observer.OnNext(selItem.Index);
        this.SlideDownAndExit();
      }));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ListPickerPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.OptionList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("OptionList");
    }

    public class Item : WaViewModelBase
    {
      private bool isSelected;

      public string Text { get; private set; }

      public int Index { get; private set; }

      public bool IsSelected
      {
        get => this.isSelected;
        set
        {
          if (this.isSelected == value)
            return;
          this.isSelected = value;
          this.NotifyPropertyChanged("ForegroundBrush");
        }
      }

      public Brush ForegroundBrush
      {
        get => !this.IsSelected ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }

      public Item(int index, string text)
      {
        this.Text = text;
        this.Index = index;
      }
    }
  }
}
