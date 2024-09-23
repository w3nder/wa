// Decompiled with JetBrains decompiler
// Type: WhatsApp.HistoryRepair
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

#nullable disable
namespace WhatsApp
{
  public class HistoryRepair : PhoneApplicationPage
  {
    internal Storyboard StartingAnimation;
    internal Storyboard SuccessAnimation;
    internal Storyboard FailureAnimation;
    internal Grid LayoutRoot;
    internal Grid RestoreVisual;
    internal ProgressBar Progressbar;
    internal WhiteBlackImage SmilingPhone;
    internal TextBlock Info;
    internal Grid Decision;
    private bool _contentLoaded;

    public HistoryRepair()
    {
      this.InitializeComponent();
      Localizable.LocalizeAppBar((PhoneApplicationPage) this);
      this.Loaded += new RoutedEventHandler(this.OnLoaded);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      this.SmilingPhone.Image = ImageStore.SmilingPhoneIcon;
    }

    private void Repair()
    {
      this.Decision.Visibility = Visibility.Collapsed;
      this.Info.Text = AppResources.RepairingProgress;
      this.AnimationObservable().CombineLatest<Unit, string, string>(this.RepairDbObservable(), (Func<Unit, string, string>) ((anim, status) => status)).ObserveOnDispatcher<string>().Subscribe<string>((Action<string>) (status =>
      {
        this.Info.Text = status;
        Log.l("DbRepair", "Completion status: {0}", (object) status);
        this.ApplicationBar.IsVisible = true;
        if (Settings.MessagesDbRepairState == SqliteRepair.SqliteRepairState.Successful)
        {
          Storyboarder.Perform(this.SuccessAnimation, false);
          AppState.ClientInstance.ResetConnection();
        }
        else
          Storyboarder.Perform(this.FailureAnimation, false);
        Settings.MessagesDbRepairState = SqliteRepair.SqliteRepairState.Unstarted;
      }));
    }

    private IObservable<Unit> AnimationObservable()
    {
      return Observable.Create<Unit>((Func<IObserver<Unit>, Action>) (observer =>
      {
        Storyboarder.Perform(this.StartingAnimation, false, (Action) (() =>
        {
          observer.OnNext(new Unit());
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
      base.OnNavigatingFrom(e);
      if (!SqliteRepair.IsRepairInProgress())
        return;
      SqliteRepair.PauseRepair();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      if (SqliteRepair.IsRepairStarted())
        SqliteRepair.ResumeRepair();
      this.Repair();
      base.OnNavigatedTo(e);
    }

    private IObservable<string> RepairDbObservable()
    {
      return Observable.Create<string>((Func<IObserver<string>, Action>) (observer =>
      {
        WAThreadPool.QueueUserWorkItem((Action) (() =>
        {
          try
          {
            bool success = false;
            MessagesContext.Run((MessagesContext.MessagesCallback) (db => success = db.Repair()));
            if (!SqliteRepair.IsPaused())
            {
              Settings.Invalidate();
              if (success)
              {
                long recovered = 0;
                MessagesContext.Run((MessagesContext.MessagesCallback) (db => recovered = db.GetMessagesCount()));
                observer.OnNext(Plurals.Instance.GetString(AppResources.RecoveredPlural, (int) recovered));
              }
              else
                observer.OnNext(AppResources.RepairFailed);
            }
          }
          catch (Exception ex)
          {
            Log.SendCrashLog(ex, "db repair");
            observer.OnNext(AppResources.RepairFailed);
          }
          observer.OnCompleted();
        }));
        return (Action) (() => { });
      }));
    }

    private void GoToNextPage()
    {
      this.NavigationService.Navigate(new Uri("/PageSelect?ClearStack=true", UriKind.Relative));
    }

    private void OnSkip(object sender, RoutedEventArgs e)
    {
      UIUtils.MessageBox((string) null, AppResources.ConfirmNotRepairing, (IEnumerable<string>) new string[2]
      {
        AppResources.RepairDbButton,
        AppResources.SkipButton
      }, (Action<int>) (buttonIdx =>
      {
        if (buttonIdx != 0)
        {
          if (buttonIdx != 1)
            return;
          this.GoToNextPage();
        }
        else
          this.Repair();
      }));
    }

    private void OnRepair(object sender, RoutedEventArgs e) => this.Repair();

    private void OnContinueClick(object sender, EventArgs e) => this.GoToNextPage();

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/HistoryRepair.xaml", UriKind.Relative));
      this.StartingAnimation = (Storyboard) this.FindName("StartingAnimation");
      this.SuccessAnimation = (Storyboard) this.FindName("SuccessAnimation");
      this.FailureAnimation = (Storyboard) this.FindName("FailureAnimation");
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.RestoreVisual = (Grid) this.FindName("RestoreVisual");
      this.Progressbar = (ProgressBar) this.FindName("Progressbar");
      this.SmilingPhone = (WhiteBlackImage) this.FindName("SmilingPhone");
      this.Info = (TextBlock) this.FindName("Info");
      this.Decision = (Grid) this.FindName("Decision");
    }
  }
}
