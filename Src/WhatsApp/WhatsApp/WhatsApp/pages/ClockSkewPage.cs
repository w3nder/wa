// Decompiled with JetBrains decompiler
// Type: WhatsApp.ClockSkewPage
// Assembly: WhatsApp, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 47CEDC7C-4E10-4203-B2F2-D2BD8D77633D
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsApp.dll

using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace WhatsApp
{
  public class ClockSkewPage : PhoneApplicationPage
  {
    private bool pageRemoved;
    private static DateTime? timeWhenClockUpdateInvoked;
    internal Grid LayoutRoot;
    internal Grid SysTrayPlaceHolder;
    internal PageTitlePanel PageTitle;
    internal ProgressBar LoadingProgress;
    internal StackPanel ContentPanel;
    internal TextBlock UpdateText;
    internal RoundButton ButtonIcon;
    internal TextBlock ButtonText;
    private bool _contentLoaded;

    public ClockSkewPage()
    {
      this.InitializeComponent();
      SysTrayHelper.SetForegroundColor((DependencyObject) this, Constants.SysTrayOffWhite);
      this.SysTrayPlaceHolder.Height = UIUtils.SystemTraySizePortrait;
      this.PageTitle.Mode = PageTitlePanel.Modes.Zoomed;
      this.PageTitle.SmallTitle = AppResources.ClockSkewTitle;
      this.UpdateText.Text = AppResources.ClockSkewText;
      this.ButtonText.Text = AppResources.ClockSkewUpdate;
      this.ButtonIcon.ButtonIcon = (BitmapSource) ImageStore.GetStockIcon("/Images/next.png");
    }

    private void Update_Click(object sender, EventArgs e)
    {
      ClockSkewPage.timeWhenClockUpdateInvoked = new DateTime?(DateTime.UtcNow);
      Control elem = sender as Control;
      elem.IsEnabled = false;
      Action enable = (Action) (() => this.Dispatcher.BeginInvokeIfNeeded((Action) (() => elem.IsEnabled = true)));
      NavUtils.NavigateExternal(DeepLinks.DateTimeUrl, enable, (Action<Exception>) (err => enable()));
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      string str;
      if (UriUtils.ParsePageParams(e.Uri).TryGetValue("NoNav", out str) && str == "true")
      {
        base.OnNavigatedTo(e);
      }
      else
      {
        if (e.NavigationMode == NavigationMode.Back && ClockSkewPage.timeWhenClockUpdateInvoked.HasValue)
        {
          DateTime utcNow = DateTime.UtcNow;
          Log.l(nameof (ClockSkewPage), "Time updated, was {0}, now {1}", (object) ClockSkewPage.timeWhenClockUpdateInvoked.Value, (object) utcNow);
          if (Math.Abs((utcNow - ClockSkewPage.timeWhenClockUpdateInvoked.Value).TotalDays) > 30.0)
            FunRunner.ClearSkewValues("Clock Changed");
        }
        App.LogExpiryRelatedVariables("ClockSkew page");
        if (AppState.IsPhoneTimeBadlySkewed())
        {
          if (AppState.DaysUntilExpiration(new DateTime?(DateTime.UtcNow)) <= 0.0)
            App.CheckForUpdate((Action) (() => this.Dispatcher.BeginInvoke((Action) (() =>
            {
              if (this.pageRemoved)
                return;
              this.LoadingProgress.Visibility = Visibility.Collapsed;
              if (Settings.IsUpdateAvailable)
              {
                NavUtils.NavigateToPage("UpdateVersion", "ClearStack=true");
              }
              else
              {
                this.PageTitle.Visibility = this.ContentPanel.Visibility = Visibility.Visible;
                this.LoadingProgress.Visibility = Visibility.Collapsed;
              }
            }))));
        }
        else
          this.Dispatcher.BeginInvoke((Action) (() => NavUtils.NavigateHome(this.NavigationService)));
        NavUtils.ClearBackStack();
        base.OnNavigatedTo(e);
      }
    }

    protected override void OnRemovedFromJournal(JournalEntryRemovedEventArgs e)
    {
      this.pageRemoved = true;
      base.OnRemovedFromJournal(e);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/WhatsApp;component/Pages/ClockSkewPage.xaml", UriKind.Relative));
      this.LayoutRoot = (Grid) this.FindName("LayoutRoot");
      this.SysTrayPlaceHolder = (Grid) this.FindName("SysTrayPlaceHolder");
      this.PageTitle = (PageTitlePanel) this.FindName("PageTitle");
      this.LoadingProgress = (ProgressBar) this.FindName("LoadingProgress");
      this.ContentPanel = (StackPanel) this.FindName("ContentPanel");
      this.UpdateText = (TextBlock) this.FindName("UpdateText");
      this.ButtonIcon = (RoundButton) this.FindName("ButtonIcon");
      this.ButtonText = (TextBlock) this.FindName("ButtonText");
    }
  }
}
