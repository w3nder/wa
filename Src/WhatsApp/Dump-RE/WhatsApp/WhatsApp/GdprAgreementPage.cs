// Decompiled with JetBrains decompiler
// Type: WhatsApp.GdprAgreementPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class GdprAgreementPage : PhoneApplicationPage
  {
    private const string LogHeader = "tos2";
    private GdprAgreementPageViewModel viewModel;
    private Storyboard slideDownSb;
    internal ZoomBox LayoutRootZoomBox;
    internal Grid LayoutRoot;
    internal WhatsApp.CompatibilityShims.LongListSelector Screen1ContentList;
    internal WhatsApp.CompatibilityShims.LongListSelector Screen2ContentList;
    internal Button ActionButton;
    internal StepProgressBar ProgressBar;
    private bool _contentLoaded;

    public GdprAgreementPage()
    {
      this.InitializeComponent();
      this.DataContext = (object) (this.viewModel = new GdprAgreementPageViewModel(this.Orientation));
      this.Screen1ContentList.OverlapScrollBar = true;
      this.Screen2ContentList.OverlapScrollBar = true;
      this.ProgressBar.Setup(2, 4);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    private void SlideDownAndBackOut()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.GoBack(this.NavigationService, false)));
    }

    private void SlideDownAndNavigateHome()
    {
      if (this.slideDownSb == null)
        this.slideDownSb = WaAnimations.PageTransition(PageTransitionAnimation.SlideDownFadeOut);
      Storyboarder.Perform(this.slideDownSb, (DependencyObject) this.LayoutRoot, false, (Action) (() => NavUtils.NavigateHome(this.NavigationService)));
    }

    private bool TryDismiss()
    {
      bool flag = false;
      if (this.viewModel.CanBeDismissed)
      {
        Settings.GdprTosUserDismissedUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
        this.SlideDownAndNavigateHome();
        flag = true;
      }
      else
        Settings.GdprTosUserDismissedUtc = new DateTime?();
      return flag;
    }

    private void OnLoaded(object sender, EventArgs e)
    {
      Settings.GdprTosLastShownUtc = new DateTime?(FunRunner.CurrentServerTimeUtc);
    }

    protected override void OnBackKeyPress(CancelEventArgs e)
    {
      if (this.viewModel.Screen > 1)
      {
        e.Cancel = true;
        --this.viewModel.Screen;
        this.ProgressBar.Backward(this.viewModel.Screen - 1, 250);
      }
      else if (this.viewModel.CanBeDismissed)
        e.Cancel = true;
      else
        base.OnBackKeyPress(e);
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.viewModel.SafeDispose();
      base.OnRemovedFromJournal(e);
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);
      if (e.NavigationMode != NavigationMode.Reset && e.NavigationMode != NavigationMode.Back)
        NavUtils.ClearBackStack();
      if (GdprTos.ShouldShowOnAppEntry(false))
      {
        Log.d("tos2", "check screen 1 on nav to {0}", (object) Settings.GdprTosCurrentStage);
        if (this.viewModel == null || this.viewModel.Screen == 1 || Settings.GdprTosCurrentStage == 1)
          return;
        this.viewModel.Screen = 1;
        this.ProgressBar.FillTill(0);
        this.Screen2ContentList.ScrollToPretty(0.0);
      }
      else
        this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateHome(this.NavigationService)));
    }

    private void ActionButton_Click(object sender, RoutedEventArgs e)
    {
      bool flag = false;
      Action onProgressAnimationEnded = (Action) null;
      if (this.viewModel.Screen == 2)
      {
        Log.l("tos2", "tap on agree button");
        this.viewModel.IsAcceptButtonTapped = true;
        if (this.viewModel.IsAgeConsentChecked)
        {
          GdprTos.Accept("tap accept");
          onProgressAnimationEnded = (Action) (() => this.SlideDownAndNavigateHome());
          flag = true;
        }
        else
          Log.l("tos2", "skip agree | not yet consent age");
      }
      else
      {
        Log.l("tos2", "tap on next button");
        onProgressAnimationEnded = (Action) (() =>
        {
          this.ActionButton.IsHitTestVisible = true;
          ++this.viewModel.Screen;
          Deployment.Current.Dispatcher.BeginInvoke((Action) (() => this.Screen2ContentList.ScrollToPretty(0.0)));
        });
        flag = true;
      }
      if (!flag)
        return;
      this.ActionButton.IsHitTestVisible = false;
      this.ProgressBar.Forward(this.viewModel.Screen - 1, 350);
      this.ProgressBar.GetProgressAnimationEndedObservable().Take<int>(1).ObserveOnDispatcher<int>().Subscribe<int>((Action<int>) (_ =>
      {
        Action action = onProgressAnimationEnded;
        if (action == null)
          return;
        action();
      }));
    }

    private void DismissButton_Click(object sender, RoutedEventArgs e) => this.TryDismiss();

    private void AgeConsent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
    {
      if (this.viewModel == null)
        return;
      this.viewModel.IsAgeConsentChecked = !this.viewModel.IsAgeConsentChecked;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/GdprAgreementPage.xaml", UriKind.Relative));
      this.LayoutRootZoomBox = (ZoomBox) this.FindName("LayoutRootZoomBox");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.Screen1ContentList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("Screen1ContentList");
      this.Screen2ContentList = (WhatsApp.CompatibilityShims.LongListSelector) this.FindName("Screen2ContentList");
      this.ActionButton = (Button) this.FindName("ActionButton");
      this.ProgressBar = (StepProgressBar) this.FindName("ProgressBar");
    }
  }
}
