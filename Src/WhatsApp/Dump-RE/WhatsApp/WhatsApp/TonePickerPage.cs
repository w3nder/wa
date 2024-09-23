// Decompiled with JetBrains decompiler
// Type: WhatsApp.TonePickerPage
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

#nullable disable
namespace WhatsApp
{
  public class TonePickerPage : PhoneApplicationPage
  {
    private static Ringtones.Tone[] nextInstanceTones;
    private static string nextInstanceSelectedTonePath;
    private static IObserver<Ringtones.Tone> nextInstanceObserver;
    private static bool nextInstanceShowResetToDefault;
    private IObserver<Ringtones.Tone> observer;
    private TonePickerPage.ToneViewModel selectedItem;
    private bool pageRemoved;
    private Storyboard slideDownSb;
    private TonePickerPageViewModel viewModel;
    internal ZoomBox RootZoomBox;
    internal Grid LayoutRoot;
    internal Grid ContentPanel;
    internal WhatsApp.CompatibilityShims.LongListSelector ToneList;
    internal MediaElement MediaElement;
    private bool _contentLoaded;

    public TonePickerPage()
    {
      this.InitializeComponent();
      this.RootZoomBox.ZoomFactor = ResolutionHelper.ZoomFactor;
      this.observer = TonePickerPage.nextInstanceObserver;
      TonePickerPage.nextInstanceObserver = (IObserver<Ringtones.Tone>) null;
      Ringtones.Tone[] nextInstanceTones = TonePickerPage.nextInstanceTones;
      TonePickerPage.nextInstanceTones = (Ringtones.Tone[]) null;
      string selectedTonePath = TonePickerPage.nextInstanceSelectedTonePath;
      TonePickerPage.nextInstanceSelectedTonePath = (string) null;
      if (this.observer != null)
        this.InitAsync(nextInstanceTones, selectedTonePath);
      this.DataContext = (object) (this.viewModel = new TonePickerPageViewModel(this.Orientation));
      this.viewModel.ShowResetToDefault = TonePickerPage.nextInstanceShowResetToDefault;
      TonePickerPage.nextInstanceShowResetToDefault = false;
    }

    public static IObservable<Ringtones.Tone> Start(
      Ringtones.Tone[] tones,
      string selectedTonePath = null,
      bool showResetToDefault = false)
    {
      return Observable.Create<Ringtones.Tone>((Func<IObserver<Ringtones.Tone>, Action>) (observer =>
      {
        TonePickerPage.nextInstanceObserver = observer;
        TonePickerPage.nextInstanceTones = tones;
        TonePickerPage.nextInstanceSelectedTonePath = selectedTonePath;
        TonePickerPage.nextInstanceShowResetToDefault = showResetToDefault;
        NavUtils.NavigateToPage(nameof (TonePickerPage));
        return (Action) (() => { });
      }));
    }

    private void InitAsync(Ringtones.Tone[] tones, string selectedTonePath)
    {
      if (tones == null || !((IEnumerable<Ringtones.Tone>) tones).Any<Ringtones.Tone>())
        return;
      WAThreadPool.Scheduler.Schedule((Action) (() =>
      {
        TonePickerPage.ToneViewModel[] array = ((IEnumerable<Ringtones.Tone>) tones).OrderBy<Ringtones.Tone, int>((Func<Ringtones.Tone, int>) (t => t.SortIndex)).Select<Ringtones.Tone, TonePickerPage.ToneViewModel>((Func<Ringtones.Tone, TonePickerPage.ToneViewModel>) (t => new TonePickerPage.ToneViewModel(t))).ToArray<TonePickerPage.ToneViewModel>();
        List<IGrouping<string, TonePickerPage.ToneViewModel>> groupedVms = ((IEnumerable<TonePickerPage.ToneViewModel>) array).GroupBy<TonePickerPage.ToneViewModel, string>((Func<TonePickerPage.ToneViewModel, string>) (vm => vm.Grouping)).ToList<IGrouping<string, TonePickerPage.ToneViewModel>>();
        TonePickerPage.ToneViewModel selItem = ((IEnumerable<TonePickerPage.ToneViewModel>) array).FirstOrDefault<TonePickerPage.ToneViewModel>((Func<TonePickerPage.ToneViewModel, bool>) (vm => vm.Tone.Filepath == selectedTonePath));
        this.Dispatcher.BeginInvoke((Action) (() => this.Show(groupedVms, selItem)));
      }));
    }

    private void Show(
      List<IGrouping<string, TonePickerPage.ToneViewModel>> vms,
      TonePickerPage.ToneViewModel selected)
    {
      if (this.pageRemoved)
        return;
      if (vms == null || !vms.Any<IGrouping<string, TonePickerPage.ToneViewModel>>())
      {
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
      }
      else
      {
        if (vms.Count <= 1)
        {
          this.ToneList.IsFlatList = true;
          this.ToneList.ItemsSource = (IList) vms.First<IGrouping<string, TonePickerPage.ToneViewModel>>().ToArray<TonePickerPage.ToneViewModel>();
        }
        if (selected == null)
          return;
        this.selectedItem = selected;
        this.selectedItem.IsSelected = true;
      }
    }

    private void Play(Ringtones.Tone tone)
    {
      if (this.MediaElement.Source != (Uri) null)
      {
        try
        {
          this.MediaElement.Stop();
        }
        catch (Exception ex)
        {
        }
      }
      if (tone == null)
        return;
      this.MediaElement.Source = new Uri(tone.Filepath.Replace('\\', '/'), UriKind.Relative);
      this.MediaElement.Play();
    }

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.ContentPanel, false, (Action) (() => NavUtils.GoBack(this.NavigationService)));
      if (this.viewModel == null)
        return;
      this.viewModel.BackgroundBrush = (Brush) UIUtils.TransparentBrush;
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
        this.observer.OnCompleted();
      e.Cancel = true;
      base.OnBackKeyPress(e);
      this.SlideDownAndBackOut();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (this.observer == null && e.NavigationMode != NavigationMode.Reset)
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.GoBack()));
      else
        Storyboarder.Perform(WaAnimations.PageTransition(PageTransitionAnimation.SlideUpFadeIn), (DependencyObject) this.ContentPanel);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      if (this.observer != null)
        this.observer.OnCompleted();
      if (this.slideDownSb != null)
        this.slideDownSb.Stop();
      base.OnRemovedFromJournal(e);
    }

    private void Play_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (!(sender is FrameworkElement frameworkElement) || !(frameworkElement.Tag is TonePickerPage.ToneViewModel tag))
        return;
      this.Play(tag.Tone);
    }

    private void Item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.observer == null || !(sender is FrameworkElement frameworkElement))
        return;
      if (this.selectedItem != null)
        this.selectedItem.IsSelected = false;
      if (frameworkElement.Tag is TonePickerPage.ToneViewModel tag && tag.Tone != null)
      {
        this.selectedItem = tag;
        this.selectedItem.IsSelected = true;
        this.observer.OnNext(tag.Tone);
        this.observer.OnCompleted();
        this.Dispatcher.BeginInvoke((Action) (() => this.SlideDownAndBackOut()));
      }
      else
      {
        this.observer.OnCompleted();
        this.SlideDownAndBackOut();
      }
    }

    private void ResetToDefault_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      this.observer.OnNext((Ringtones.Tone) null);
      this.observer.OnCompleted();
      this.Dispatcher.BeginInvoke((Action) (() => this.SlideDownAndBackOut()));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/TonePickerPage.xaml", UriKind.Relative));
      this.RootZoomBox = (ZoomBox) this.FindName("RootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.ContentPanel = (Grid) this.FindName("ContentPanel");
      this.ToneList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("ToneList");
      this.MediaElement = (MediaElement) this.FindName("MediaElement");
    }

    public class ToneViewModel : WaViewModelBase
    {
      private bool isSelected;
      private Ringtones.Tone tone;

      public System.Windows.Media.ImageSource PlayButtonIcon => (System.Windows.Media.ImageSource) ImageStore.PlayButton;

      public string ToneNameStr => this.tone.Name;

      public Brush ForegroundBrush
      {
        get => !this.IsSelected ? (Brush) UIUtils.ForegroundBrush : (Brush) UIUtils.AccentBrush;
      }

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

      public string Grouping => this.tone.Grouping;

      public Ringtones.Tone Tone => this.tone;

      public ToneViewModel(Ringtones.Tone t) => this.tone = t;
    }
  }
}
